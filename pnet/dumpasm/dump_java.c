/*
 * dump_java.c - Disassemble Java method contents.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#include "il_dumpasm.h"
#include "il_jopcodes.h"
#include "il_system.h"
#include "il_opcodes.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef IL_CONFIG_JAVA

/* "image/jopcodes.c" */
extern ILOpcodeInfo const ILJavaOpcodeTable[256];

/*
 * Read big-endian quantities of various sizes.
 */
#define	IL_BREAD_INT16(buf)	((ILInt16)(_IL_READ_BYTE((buf), 1) | \
									   _IL_READ_BYTE_SHIFT((buf), 0, 8)))
#define	IL_BREAD_UINT16(buf) ((ILUInt16)(_IL_READ_BYTE((buf), 1) | \
									     _IL_READ_BYTE_SHIFT((buf), 0, 8)))
#define	IL_BREAD_INT32(buf)	((ILInt32)(_IL_READ_BYTE((buf), 3) | \
									   _IL_READ_BYTE_SHIFT((buf), 2, 8) | \
									   _IL_READ_BYTE_SHIFT((buf), 1, 16) | \
									   _IL_READ_BYTE_SHIFT((buf), 0, 24)))
#define	IL_BREAD_UINT32(buf)	((ILUInt32)(_IL_READ_BYTE((buf), 3) | \
									    _IL_READ_BYTE_SHIFT((buf), 2, 8) | \
									    _IL_READ_BYTE_SHIFT((buf), 1, 16) | \
									    _IL_READ_BYTE_SHIFT((buf), 0, 24)))
#define	IL_BREAD_INT64(buf)	\
			(((ILInt64)(IL_BREAD_UINT32((buf) + 4))) | \
			 (((ILInt64)(IL_BREAD_INT32((buf)))) << 32))
#define	IL_BREAD_UINT64(buf)	\
			(((ILUInt64)(IL_BREAD_UINT32((buf) + 4))) | \
			 (((ILUInt64)(IL_BREAD_UINT32((buf)))) << 32))

/*
 * Determine the size of a Java instruction.  Returns zero
 * if the instruction is invalid.
 */
static unsigned long JavaInsnSize(unsigned char *buf, unsigned long size,
							      unsigned long offset)
{
	unsigned long len;
	ILInt32 tempa;
	ILInt32 tempb;
	ILUInt32 swsize;
	if(*buf == JAVA_OP_WIDE)
	{
		/* Wide instruction */
		if(size < 2)
		{
			return 0;
		}
		switch(buf[1])
		{
			case JAVA_OP_ILOAD:
			case JAVA_OP_FLOAD:
			case JAVA_OP_ALOAD:
			case JAVA_OP_LLOAD:
			case JAVA_OP_DLOAD:
			case JAVA_OP_ISTORE:
			case JAVA_OP_FSTORE:
			case JAVA_OP_ASTORE:
			case JAVA_OP_LSTORE:
			case JAVA_OP_DSTORE:
			case JAVA_OP_RET:
			{
				if(size < 4)
					return 0;
				else
					return 4;
			}
			/* Not reached */

			case JAVA_OP_IINC:
			{
				if(size < 6)
					return 0;
				else
					return 6;
			}
			/* Not reached */
		}
		return 0;
	}
	else if(*buf == JAVA_OP_LOOKUPSWITCH)
	{
		/* Lookup switch instruction */
		len = 1;
		while(((offset + len) & 3) != 0)
		{
			++len;
		}
		if(size < (len + 8))
		{
			return 0;
		}
		swsize = IL_BREAD_UINT32(buf + len + 4);
		if(swsize > (ILInt32)0x20000000)
		{
			return 0;
		}
		len += 8 + swsize * 8;
		if(size < len)
		{
			return 0;
		}
		return len;
	}
	else if(*buf == JAVA_OP_TABLESWITCH)
	{
		/* Table switch instruction */
		len = 1;
		while(((offset + len) & 3) != 0)
		{
			++len;
		}
		if(size < (len + 12))
		{
			return 0;
		}
		tempa = IL_BREAD_INT32(buf + len + 4);
		tempb = IL_BREAD_INT32(buf + len + 8);
		if(tempa > tempb ||
		   (swsize = (ILUInt32)(tempb - tempa + 1)) > (ILUInt32)0x20000000)
		{
			return 0;
		}
		len += 12 + swsize * 4;
		if(size < len)
		{
			return 0;
		}
		return len;
	}
	else if(ILJavaOpcodeTable[*buf].args == IL_OPCODE_ARGS_INVALID)
	{
		/* Invalid instruction */
		return 0;
	}
	else
	{
		/* Ordinary instruction */
		return ILJavaOpcodeTable[*buf].size;
	}
}

/*
 * Mark a destination jump point.
 */
#define	MarkDest(dest)	\
			do { \
				unsigned long _dest = (unsigned long)(dest); \
				if(_dest < size) \
				{ \
					jumpPoints[_dest / 32] |= \
						(ILUInt32)(1L << (_dest % 32)); \
				} \
			} while (0)

/*
 * Dump all Java instructions in a given buffer.  Returns zero
 * if there is something wrong with the buffer's format.
 */
static int DumpJavaInstructions(ILImage *image, ILClass *classInfo,
							    FILE *outstream,
								unsigned char *buf, unsigned long size,
							    unsigned long addr, ILException *clauses,
							    int flags)
{
	ILUInt32 *jumpPoints;
	int result = 0;
	unsigned char *temp;
	unsigned char *temp2;
	unsigned long tsize;
	unsigned long offset;
	unsigned long dest;
	unsigned long isize;
	unsigned long args;
	const ILOpcodeInfo *info;
	int argType;
	unsigned long numItems;
	unsigned long item;
	int isWide;

	/* Allocate a helper array to mark jump points within the code */
	jumpPoints = (ILUInt32 *)ILCalloc(((size + 3) & ~3), 1);
	if(!jumpPoints)
	{
		fprintf(stderr, "out of memory\n");
		exit(1);
	}

	/* Mark the entry point to the method so we get a label for it */
	jumpPoints[0] |= (ILUInt32)1;

	/* Mark the position of exception clauses */
	while(clauses != 0)
	{
		MarkDest(clauses->tryOffset);
		MarkDest(clauses->tryOffset + clauses->tryLength);
		MarkDest(clauses->handlerOffset);
		clauses = clauses->next;
	}

	/* Scan the instruction list to locate jump points */
	temp = buf;
	tsize = size;
	offset = 0;
	while(tsize > 0)
	{
		isize = JavaInsnSize(temp, tsize, offset);
		if(!isize)
		{
			fprintf(outstream, "\t\t// unknown instruction 0x%02X\n",
					((int)(temp[0])) & 0xFF);
			goto cleanup;
		}
		info = &(ILJavaOpcodeTable[((int)(temp[0])) & 0xFF]);
		if(info->args == IL_OPCODE_ARGS_SHORT_JUMP)
		{
			dest = (unsigned long)(((long)offset) +
								   (long)(IL_BREAD_INT16(temp + 1)));
			MarkDest(dest);
		}
		else if(info->args == IL_OPCODE_ARGS_LONG_JUMP)
		{
			dest = (unsigned long)(((long)offset) +
								   (long)(IL_BREAD_INT32(temp + 1)));
			MarkDest(dest);
		}
		else if(info->args == IL_OPCODE_ARGS_SWITCH)
		{
			/* Align the switch instruction's arguments */
			args = 1;
			while(((offset + args) & 3) != 0)
			{
				++args;
			}

			/* Mark the default label */
			dest = (unsigned long)(((long)offset) +
								   (long)(IL_BREAD_INT32(temp + args)));
			MarkDest(dest);

			/* Process the bulk of the switch */
			if(temp[0] == JAVA_OP_TABLESWITCH)
			{
				/* Mark all of the labels in a table-based switch */
				item = (unsigned long)(IL_BREAD_INT32(temp + args + 8) -
									   IL_BREAD_INT32(temp + args + 4) + 1);
				temp2 = temp + args + 12;
				while(item > 0)
				{
					dest = (unsigned long)(((long)offset) +
										   (long)(IL_BREAD_INT32(temp2)));
					MarkDest(dest);
					--item;
					temp2 += 4;
				}
			}
			else
			{
				/* Mark all of the labels in a lookup-based switch */
				item = (unsigned long)(IL_BREAD_UINT32(temp + args + 4));
				temp2 = temp + args + 8 + 4;
				while(item > 0)
				{
					dest = (unsigned long)(((long)offset) +
										   (long)(IL_BREAD_INT32(temp2)));
					MarkDest(dest);
					--item;
					temp2 += 8;
				}
			}
		}
		offset += isize;
		temp += isize;
		tsize -= isize;
	}

	/* Dump the instructions */
	temp = buf;
	tsize = size;
	offset = 0;
	while(tsize > 0)
	{
		/* If this is a jump point, then print a label for it */
		if((jumpPoints[offset / 32] & (ILUInt32)(1L << (offset % 32))) != 0)
		{
			fprintf(outstream, "\t?L%lx:\n", offset + addr);
		}

		/* Extract the instruction from the method input stream */
		isize = JavaInsnSize(temp, tsize, offset);
		info = &(ILJavaOpcodeTable[((int)(temp[0])) & 0xFF]);
		if(*temp == JAVA_OP_WIDE)
		{
			/* Process a wide instruction */
			info = &(ILJavaOpcodeTable[((int)(temp[1])) & 0xFF]);
			isWide = 1;
			args = 2;
		}
		else
		{
			/* Process an ordinary instruction */
			isWide = 0;
			args = 1;
		}

		/* Dump the instruction based on its argument type */
		argType = info->args;
		putc('\t', outstream);
		putc('\t', outstream);
		fputs(info->name, outstream);
		if(argType != IL_OPCODE_ARGS_INVALID &&
		   argType != IL_OPCODE_ARGS_NONE)
		{
			numItems = (unsigned long)(strlen(info->name));
			while(numItems < 10)
			{
				putc(' ', outstream);
				++numItems;
			}
			putc(' ', outstream);
		}
		switch(argType)
		{
			case IL_OPCODE_ARGS_INVALID:	break;
			case IL_OPCODE_ARGS_NONE:		break;

			case IL_OPCODE_ARGS_INT8:
			{
				fprintf(outstream, "%d", (int)(ILInt8)(temp[args]));
			}
			break;

			case IL_OPCODE_ARGS_INT16:
			{
				fprintf(outstream, "%d",
				        (int)(ILInt16)(IL_BREAD_UINT16(temp + args)));
			}
			break;

			case IL_OPCODE_ARGS_TOKEN:
			case IL_OPCODE_ARGS_CALL:
			{
				/* An instruction that takes a constant pool entry argument */
				if(temp[0] == JAVA_OP_LDC)
				{
					item = (unsigned long)(temp[args]);
				}
				else
				{
					item = (unsigned long)(IL_BREAD_UINT16(temp + args));
				}
				switch(ILJavaGetConstType(classInfo, (ILUInt32)item))
				{
					case JAVA_CONST_UTF8:
					{
						const char *str;
						ILUInt32 len;
						str = ILJavaGetUTF8String
							(classInfo, (ILUInt32)item, &len);
						if(str && len)
						{
							ILDumpStringLen(outstream, str, (int)len);
						}
						else
						{
							fputs("\"\"", outstream);
						}
					}
					break;

					case JAVA_CONST_INTEGER:
					{
						ILInt32 value;
						if(ILJavaGetInteger(classInfo, (ILUInt32)item, &value))
						{
							fprintf(outstream, "%ld", (long)value);
						}
						else
						{
							fputs("??", outstream);
						}
					}
					break;

					case JAVA_CONST_FLOAT:
					{
						ILFloat value;
						unsigned char buf[4];
						if(ILJavaGetFloat(classInfo, (ILUInt32)item, &value))
						{
							IL_WRITE_FLOAT(buf, value);
							fprintf(outstream, "float32(0x%02X%02X%02X%02X)",
									(((int)(buf[3])) & 0xFF),
									(((int)(buf[2])) & 0xFF),
									(((int)(buf[1])) & 0xFF),
									(((int)(buf[0])) & 0xFF));
						}
						else
						{
							fputs("??", outstream);
						}
					}
					break;

					case JAVA_CONST_LONG:
					{
						ILInt64 value;
						if(ILJavaGetLong(classInfo, (ILUInt32)item, &value))
						{
							fprintf(outstream, "0x%08lX%08lX",
									(long)((value >> 32) & (long)0xFFFFFFFF),
									(long)(value & (long)0xFFFFFFFF));
						}
						else
						{
							fputs("??", outstream);
						}
					}
					break;

					case JAVA_CONST_DOUBLE:
					{
						ILDouble value;
						unsigned char buf[8];
						if(ILJavaGetDouble(classInfo, (ILUInt32)item, &value))
						{
							IL_WRITE_DOUBLE(buf, value);
							fprintf(outstream,
								"float64(0x%02X%02X%02X%02X%02X%02X%02X%02X)",
								(((int)(buf[7])) & 0xFF),
								(((int)(buf[6])) & 0xFF),
								(((int)(buf[5])) & 0xFF),
								(((int)(buf[4])) & 0xFF),
								(((int)(buf[3])) & 0xFF),
								(((int)(buf[2])) & 0xFF),
								(((int)(buf[1])) & 0xFF),
								(((int)(buf[0])) & 0xFF));
						}
						else
						{
							fputs("??", outstream);
						}
					}
					break;

					case JAVA_CONST_CLASS:
					{
						ILClass *ref;
						ref = ILJavaGetClass(classInfo, (ILUInt32)item, 1);
						if(ref)
						{
							ILDumpClassName(outstream, image, ref, flags);
						}
						else
						{
							fputs("??", outstream);
						}
					}
					break;

					case JAVA_CONST_STRING:
					{
						const char *str;
						ILUInt32 len;
						str = ILJavaGetString(classInfo, (ILUInt32)item, &len);
						if(str && len)
						{
							ILDumpStringLen(outstream, str, (int)len);
						}
						else
						{
							fputs("\"\"", outstream);
						}
					}
					break;

					case JAVA_CONST_FIELDREF:
					{
						ILField *field;
						ILClass *info;
						field = ILJavaGetField(classInfo, (ILUInt32)item, 1,
									(temp[0] == JAVA_OP_GETSTATIC ||
									 temp[0] == JAVA_OP_PUTSTATIC));
						if(field)
						{
							ILDumpType(outstream, image,
									   ILField_Type(field), flags);
							putc(' ', outstream);
							info = ILField_Owner(field);
							if(ILClassIsValueType(info))
							{
								fputs("valuetype ", outstream);
							}
							else
							{
								fputs("class ", outstream);
							}
							ILDumpClassName(outstream, image, info, flags);
							fputs("::", outstream);
							ILDumpIdentifier(outstream,
											 ILField_Name(field), 0, flags);
						}
						else
						{
							fputs("??", outstream);
						}
					}
					break;

					case JAVA_CONST_METHODREF:
					{
						ILMethod *method;
						method = ILJavaGetMethod(classInfo, (ILUInt32)item, 1,
									(temp[0] == JAVA_OP_INVOKESTATIC));
						if(method)
						{
							ILDumpMethodType(outstream, image,
											 ILMethod_Signature(method), flags,
											 ILMethod_Owner(method),
											 ILMethod_Name(method),
											 method);
						}
						else
						{
							fputs("??", outstream);
						}
					}
					break;

					default:
					{
						fputs("??", outstream);
					}
					break;
				}
			}
			break;

			case IL_OPCODE_ARGS_NEW:
			{
				/* "newarray" instruction */
				switch(temp[args])
				{
					case JAVA_ARRAY_OF_BOOL:
					{
						fputs("bool", outstream);
					}
					break;

					case JAVA_ARRAY_OF_CHAR:
					{
						fputs("char", outstream);
					}
					break;

					case JAVA_ARRAY_OF_FLOAT:
					{
						fputs("float32", outstream);
					}
					break;

					case JAVA_ARRAY_OF_DOUBLE:
					{
						fputs("float64", outstream);
					}
					break;

					case JAVA_ARRAY_OF_BYTE:
					{
						fputs("int8", outstream);
					}
					break;

					case JAVA_ARRAY_OF_SHORT:
					{
						fputs("int16", outstream);
					}
					break;

					case JAVA_ARRAY_OF_INT:
					{
						fputs("int32", outstream);
					}
					break;

					case JAVA_ARRAY_OF_LONG:
					{
						fputs("int64", outstream);
					}
					break;

					default:
					{
						fprintf(outstream, "%d", ((int)(temp[args])) & 0xFF);
					}
					break;
				}
			}
			break;

			case IL_OPCODE_ARGS_SHORT_VAR:
			{
				if(!isWide)
				{
					fprintf(outstream, "%d", ((int)(temp[args])) & 0xFF);
				}
				else
				{
					fprintf(outstream, "%d",
							((int)(IL_BREAD_UINT16(temp + args))));
				}
			}
			break;

			case IL_OPCODE_ARGS_SHORT_JUMP:
			{
				dest = (unsigned long)(((long)offset) +
								       (long)(IL_BREAD_INT16(temp + 1)));
				fprintf(outstream, "?L%lx", dest + addr);
			}
			break;

			case IL_OPCODE_ARGS_LONG_JUMP:
			{
				dest = (unsigned long)(((long)offset) +
									   (long)(IL_BREAD_UINT32(temp + 1)));
				fprintf(outstream, "?L%lx", dest + addr);
			}
			break;

			case IL_OPCODE_ARGS_CALLI:
			{
				/* Dump a call to an interface method */
				ILMethod *method;
				item = (unsigned long)(IL_BREAD_UINT16(temp + args));
				method = ILJavaGetMethod(classInfo, (ILUInt32)item, 1, 0);
				if(method)
				{
					ILDumpMethodType(outstream, image,
									 ILMethod_Signature(method), flags,
									 ILMethod_Owner(method),
									 ILMethod_Name(method),
									 method);
				}
				else
				{
					fputs("??", outstream);
				}
				fprintf(outstream, " %d", (int)(ILUInt8)(temp[args + 2]));
			}
			break;

			case IL_OPCODE_ARGS_SWITCH:
			{
				/* Align the switch instruction's arguments */
				while(((offset + args) & 3) != 0)
				{
					++args;
				}

				/* Output the default label */
				dest = (unsigned long)(((long)offset) +
									   (long)(IL_BREAD_INT32(temp + args)));
				fprintf(outstream, "?L%lx (", dest + addr);

				/* Dump the bulk of the switch instruction */
				if(temp[0] == JAVA_OP_TABLESWITCH)
				{
					/* Dump the base value for the table-based switch */
					fprintf(outstream, "%ld : ",
						    (long)(IL_BREAD_INT32(temp + args + 4)));

					/* Determine the number of items */
					numItems = (unsigned long)
							(IL_BREAD_INT32(temp + args + 8) -
							 IL_BREAD_INT32(temp + args + 4) + 1);

					/* Dump the labels for the items */
					temp2 = temp + args + 12;
					for(item = 0; item < numItems; ++item)
					{
						if(item != 0)
						{
							putc(',', outstream);
							putc(' ', outstream);
						}
						dest = (unsigned long)(((long)offset) +
									   		   (long)(IL_BREAD_INT32(temp2)));
						fprintf(outstream, "?L%lx", dest + addr);
						temp2 += 4;
					}
				}
				else
				{
					/* Lookup-based switch instruction */
					numItems = (unsigned long)
							(IL_BREAD_UINT32(temp + args + 4));
					temp2 = temp + args + 8;
					for(item = 0; item < numItems; ++item)
					{
						if(item != 0)
						{
							putc(',', outstream);
							putc(' ', outstream);
						}
						fprintf(outstream, "%ld : ",
								(long)(IL_BREAD_INT32(temp2)));
						dest = (unsigned long)(((long)offset) +
								   		   (long)(IL_BREAD_INT32(temp2 + 4)));
						fprintf(outstream, "?L%lx", dest + addr);
						temp2 += 8;
					}
				}

				/* Terminate the switch instruction */
				putc(')', outstream);
			}
			break;

			case IL_OPCODE_ARGS_ANN_ARG:
			{
				/* Used to indicate the "iinc" instruction */
				if(!isWide)
				{
					fprintf(outstream, "%d, %d",
							((int)(temp[args])) & 0xFF,
							((int)((ILInt8)(temp[args + 1]))));
				}
				else
				{
					fprintf(outstream, "%d, %d",
							((int)(IL_BREAD_UINT16(temp + args))),
							((int)(IL_BREAD_INT16(temp + args + 2))));
				}
			}
			break;

			default:	break;
		}
		putc('\n', outstream);

		/* Move on to the next instruction */
		offset += isize;
		temp += isize;
		tsize -= isize;
	}
	result = 1;

	/* Clean up and exit */
cleanup:
	ILFree(jumpPoints);
	return result;
}

void ILDAsmDumpJavaMethod(ILImage *image, FILE *outstream,
					      ILMethod *method, int flags)
{
	unsigned long addr;
	ILMethodCode code;
	ILException *clauses;
	ILException *tempClause;
	ILClass *catchClass;

	/* Read the method code and exception information */
	if(!ILMethodGetCode(method, &code))
	{
		/* If we get here, then probably the method had an RVA,
		   but the code was not IL */
		fputs("\t\t// Cannot dump the code for native methods\n", outstream);
		return;
	}
	if(!ILMethodGetExceptions(method, &code, &clauses))
	{
		return;
	}

	/* Determine the address of the first instruction in the method */
	addr = ILMethod_RVA(method);
	if((flags & ILDASM_REAL_OFFSETS) != 0)
	{
		addr = ILImageRealOffset(image, addr) + code.headerSize;
	}
	else
	{
		addr += code.headerSize;
	}

	/* Output method header information */
	fprintf(outstream, "\t\t// Start of method header: %lx\n",
			(unsigned long)(addr - code.headerSize));
	fprintf(outstream, "\t\t.maxstack  %lu\n", (unsigned long)(code.maxStack));
	fprintf(outstream, "\t\t.locals  %lu\n", (unsigned long)(code.javaLocals));

	/* Dump the instructions within the method */
	if(!DumpJavaInstructions(image, ILMethod_Owner(method),
							 outstream, (unsigned char *)(code.code),
						     code.codeLen, addr, clauses, flags))
	{
		ILMethodFreeExceptions(clauses);
		return;
	}

	/* Dump information about the exceptions */
	tempClause = clauses;
	while(tempClause != 0)
	{
		fprintf(outstream, "\t\t.try ?L%lx to ?L%lx",
				tempClause->tryOffset + addr,
				tempClause->tryOffset + tempClause->tryLength + addr);
		if((tempClause->flags & IL_META_EXCEPTION_FINALLY) != 0)
		{
			/* Finally clause */
			fprintf(outstream, " finally");
		}
		else
		{
			/* Catch clause */
			fputs(" catch ", outstream);
			catchClass = ILClass_FromToken(image, tempClause->extraArg);
			if(catchClass)
			{
				ILDumpClassName(outstream, image, catchClass, flags);
			}
			else
			{
				fputs("??", outstream);
			}
		}
		fprintf(outstream, " handler ?L%lx\n",
				tempClause->handlerOffset + addr);
		tempClause = tempClause->next;
	}

	/* Free the exception list and exit */
	ILMethodFreeExceptions(clauses);
}

#endif /* IL_CONFIG_JAVA */

#ifdef	__cplusplus
};
#endif
