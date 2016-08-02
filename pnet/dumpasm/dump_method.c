/*
 * dump_method.c - Disassemble method contents.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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
#include "il_opcodes.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Get the size of a special instruction that does not
 * have a fixed-length representation.
 */
static unsigned long GetSpecialSize(unsigned char *temp, unsigned long tsize)
{
	unsigned long numItems;
	if(temp[0] == (unsigned char)IL_OP_SWITCH)
	{
		/* Switch lookup table */
		if(tsize < 5)
		{
			return 0;
		}
		numItems = (unsigned long)(IL_READ_UINT32(temp + 1));
		if(numItems >= ((unsigned long)0x40000000) ||
		   (numItems * 4) > (tsize - 5))
		{
			return 0;
		}
		return numItems * 4 + 5;
	}
	else if(temp[0] == (unsigned char)IL_OP_ANN_DATA_S)
	{
		/* Short form of annotation data */
		if(tsize < 2)
		{
			return 0;
		}
		numItems = (((unsigned long)(temp[1])) & 0xFF);
		if((tsize - 2) < numItems)
		{
			return 0;
		}
		return numItems + 2;
	}
	else if(temp[0] == (unsigned char)IL_OP_PREFIX &&
			temp[1] == (unsigned char)IL_PREFIX_OP_ANN_DATA)
	{
		/* Long form of annotation data */
		if(tsize < 6)
		{
			return 0;
		}
		numItems = (unsigned long)(IL_READ_UINT32(temp + 2));
		if((tsize - 6) < numItems)
		{
			return 0;
		}
		return numItems + 6;
	}
	else if(temp[0] == (unsigned char)IL_OP_ANN_PHI)
	{
		/* Static single assignment annotation data */
		if(tsize < 3)
		{
			return 0;
		}
		numItems = (unsigned long)(IL_READ_UINT16(temp + 1));
		if((tsize - 3) < (numItems * 2))
		{
			return 0;
		}
		return numItems * 2 + 3;
	}
	return 0;
}

/*
 * Dump a token.
 */
static void DumpToken(ILImage *image, FILE *outstream,
					  int flags, unsigned long token,
					  int prefixWithKind)
{
	ILClass *info;
	ILField *field;
	ILMethod *method;
	ILMember *member;
	ILMember *origMember;
	ILTypeSpec *spec;
	ILStandAloneSig *sig;
	ILType *type;
	ILMethodSpec *mspec;

	switch(token & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_TYPE_REF:
		case IL_META_TOKEN_TYPE_DEF:
		{
			/* A reference to a type */
			info = ILClass_FromToken(image, token);
			if(info)
			{
				if((flags & ILDASM_SUPPRESS_PREFIX) == 0)
				{
					if(ILClassIsValueType(info))
					{
						fputs("valuetype ", outstream);
					}
					else
					{
						fputs("class ", outstream);
					}
				}
				ILDumpClassName(outstream, image, info, flags);
			}
			else
			{
				fprintf(outstream, "#%lx", token);
			}
		}
		break;

		case IL_META_TOKEN_FIELD_DEF:
		{
			/* A reference to a field */
			field = ILField_FromToken(image, token);
			if(field)
			{
			dumpField:
				if(prefixWithKind)
				{
					fputs("field ", outstream);
				}
				ILDumpType(outstream, image, ILField_Type(field), flags);
				putc(' ', outstream);
				info = ILField_Owner(field);
				ILDumpClassName(outstream, image, info, flags);
				fputs("::", outstream);
				ILDumpIdentifier(outstream, ILField_Name(field), 0, flags);
			}
			else
			{
				fprintf(outstream, "#%lx", token);
			}
		}
		break;

		case IL_META_TOKEN_METHOD_DEF:
		{
			/* A reference to a method */
			method = ILMethod_FromToken(image, token);
			if(method)
			{
				if(prefixWithKind)
				{
					fputs("method ", outstream);
				}
				ILDumpMethodType(outstream, image,
								 ILMethod_Signature(method), flags,
								 ILMethod_Owner(method),
								 ILMethod_Name(method),
								 0/*method*/);
			}
			else
			{
				fprintf(outstream, "#%lx", token);
			}
		}
		break;

		case IL_META_TOKEN_MEMBER_REF:
		{
			/* A reference to an external method or field */
			member = ILMember_FromToken(image, token);
			origMember = member;
			member = (member ? ILMemberResolveRef(member) : 0);
			if(member)
			{
				if(ILMember_IsMethod(member))
				{
					/* Use the signature from the original member,
					   because this may be a "vararg" call that has
					   type information supplied in the call site */
					if(prefixWithKind)
					{
						fputs("method ", outstream);
					}
					method = (ILMethod *)member;
					ILDumpMethodType(outstream, image,
									 ILMember_Signature(origMember), flags,
									 ILMethod_Owner(method),
									 ILMethod_Name(method),
									 0/*method*/);
				}
				else if(ILMember_IsField(member))
				{
					field = (ILField *)member;
					goto dumpField;
				}
				else
				{
					fprintf(outstream, "#%lx", token);
				}
			}
			else
			{
				fprintf(outstream, "#%lx", token);
			}
		}
		break;

		case IL_META_TOKEN_TYPE_SPEC:
		{
			/* A reference to a type specification */
			spec = ILTypeSpec_FromToken(image, token);
			if(spec)
			{
				ILDumpType(outstream, image, ILTypeSpec_Type(spec), flags);
			}
			else
			{
				fprintf(outstream, "#%lx", token);
			}
		}
		break;

		case IL_META_TOKEN_STAND_ALONE_SIG:
		{
			/* A reference to a stand-alone signature */
			sig = ILStandAloneSig_FromToken(image, token);
			if(sig)
			{
				type = ILStandAloneSig_Type(sig);
				if(ILType_IsMethod(type))
				{
					ILDumpMethodType(outstream, image, type, flags, 0, "", 0);
				}
				else
				{
					ILDumpType(outstream, image, type, flags);
				}
			}
			else
			{
				fprintf(outstream, "#%lx", token);
			}
		}
		break;

		case IL_META_TOKEN_METHOD_SPEC:
		{
			/* A reference to a method with generic parameters */
			mspec = ILMethodSpec_FromToken(image, token);
			if(mspec)
			{
				if(prefixWithKind)
				{
					fputs("method ", outstream);
				}
				ILDumpMethodSpec(outstream, image, mspec, flags);
			}
			else
			{
				fprintf(outstream, "#%lx", token);
			}
		}
		break;

		default:
		{
			fprintf(outstream, "#%lx", token);
		}
		break;
	}

	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(outstream, " /*%08lX*/", token);
	}
}

/*
 * Dump all IL instructions in a given buffer.  Returns zero
 * if there is something wrong with the buffer's format.
 */
static int DumpInstructions(ILImage *image, FILE *outstream,
							unsigned char *buf, unsigned long size,
							unsigned long addr, ILException *clauses,
							int flags, int dumpOffsets)
{
	ILUInt32 *jumpPoints;
	int result = 0;
	unsigned char *temp;
	unsigned long tsize;
	unsigned long offset;
	unsigned long dest;
	unsigned long isize;
	unsigned long args;
	const ILOpcodeInfo *info;
	int argType;
	unsigned long numItems;
	unsigned long item;

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
		dest = clauses->tryOffset;
		if(dest < size)
		{
			jumpPoints[dest / 32] |= (ILUInt32)(1L << (dest % 32));
		}
		dest = clauses->tryOffset + clauses->tryLength;
		if(dest < size)
		{
			jumpPoints[dest / 32] |= (ILUInt32)(1L << (dest % 32));
		}
		dest = clauses->handlerOffset;
		if(dest < size)
		{
			jumpPoints[dest / 32] |= (ILUInt32)(1L << (dest % 32));
		}
		dest = clauses->handlerOffset + clauses->handlerLength;
		if(dest < size)
		{
			jumpPoints[dest / 32] |= (ILUInt32)(1L << (dest % 32));
		}
		if((clauses->flags & IL_META_EXCEPTION_FILTER) != 0)
		{
			dest = clauses->extraArg;
			if(dest < size)
			{
				jumpPoints[dest / 32] |= (ILUInt32)(1L << (dest % 32));
			}
		}
		clauses = clauses->next;
	}

	/* Scan the instruction list to locate jump points */
	temp = buf;
	tsize = size;
	offset = 0;
	while(tsize > 0)
	{
		if(*temp == (unsigned char)0xFE)
		{
			/* Prefixed instruction */
			if(tsize < 2)
			{
				break;
			}
			info = &(ILPrefixOpcodeTable[((int)(temp[1])) & 0xFF]);
			if(!strncmp(info->name, "unused_", 7))
			{
				break;
			}
			if(!(info->size))
			{
				isize = GetSpecialSize(temp, tsize);
			}
			else
			{
				isize = info->size;
			}
			if(tsize < isize)
			{
				break;
			}
		}
		else
		{
			/* Regular instruction */
			info = &(ILMainOpcodeTable[((int)(*temp)) & 0xFF]);
			if(!strncmp(info->name, "unused_", 7))
			{
				break;
			}
			if(!(info->size))
			{
				isize = GetSpecialSize(temp, tsize);
			}
			else
			{
				isize = info->size;
			}
			if(tsize < isize)
			{
				break;
			}
			if(info->args == IL_OPCODE_ARGS_SHORT_JUMP)
			{
				dest = (unsigned long)(((long)offset) + 2 +
									   (long)(ILInt8)(temp[1]));
				if(dest < size)
				{
					jumpPoints[dest / 32] |= (ILUInt32)(1L << (dest % 32));
				}
			}
			else if(info->args == IL_OPCODE_ARGS_LONG_JUMP)
			{
				dest = (unsigned long)(((long)offset) + 5 +
									   (long)(IL_READ_UINT32(temp + 1)));
				if(dest < size)
				{
					jumpPoints[dest / 32] |= (ILUInt32)(1L << (dest % 32));
				}
			}
			else if(info->args == IL_OPCODE_ARGS_SWITCH)
			{
				numItems = (unsigned long)(IL_READ_UINT32(temp + 1));
				for(item = 0; item < numItems; ++item)
				{
					dest = (unsigned long)(IL_READ_UINT32(temp + 5 + item * 4));
					dest = (unsigned long)(((long)offset) + 5 +
										   ((long)numItems) * 4 +
										   ((long)dest));
					if(dest < size)
					{
						jumpPoints[dest / 32] |= (ILUInt32)(1L << (dest % 32));
					}
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

		if(dumpOffsets)
		{
			fprintf(outstream, "%ld", offset);
		}

		/* Extract the instruction from the method input stream */
		if(*temp == (unsigned char)0xFE)
		{
			/* Prefixed instruction */
			if(tsize < 2)
			{
				goto truncated;
			}
			info = &(ILPrefixOpcodeTable[((int)(temp[1])) & 0xFF]);
			if(!strncmp(info->name, "unused_", 7))
			{
				fprintf(outstream, "\t\t// unknown instruction 0xFE 0x%02X\n",
						((int)(temp[1])) & 0xFF);
				goto cleanup;
			}
			if(!(info->size))
			{
				isize = GetSpecialSize(temp, tsize);
			}
			else
			{
				isize = info->size;
			}
			if(tsize < isize)
			{
				goto truncated;
			}
			args = 2;
		}
		else
		{
			/* Regular instruction */
			info = &(ILMainOpcodeTable[((int)(*temp)) & 0xFF]);
			if(!strncmp(info->name, "unused_", 7))
			{
				fprintf(outstream, "\t\t// unknown instruction 0x%02X\n",
						((int)(*temp)) & 0xFF);
				goto cleanup;
			}
			if(!(info->size))
			{
				isize = GetSpecialSize(temp, tsize);
			}
			else
			{
				isize = info->size;
			}
			if(tsize < isize)
			{
				goto truncated;
			}
			args = 1;
		}

		/* Dump the instruction based on its argument type */
		argType = info->args;
		putc('\t', outstream);
		putc('\t', outstream);
		if((flags & ILDASM_INSTRUCTION_BYTES) != 0)
		{
			/* Dump the bytes of the instruction */
			int posn;
			putc('/', outstream);
			putc('*', outstream);
			putc(' ', outstream);
			posn = 0;
			while(posn < 6 && posn < isize)
			{
				fprintf(outstream, "%02X ", ((int)(temp[posn]) & 0xFF));
				++posn;
			}
			while(posn < 6)
			{
				fputs("   ", outstream);
				++posn;
			}
			putc(' ', outstream);
			putc('*', outstream);
			putc('/', outstream);
			putc(' ', outstream);
		}
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

			case IL_OPCODE_ARGS_UINT8:
			{
				fprintf(outstream, "%d", ((int)(temp[args])) & 0xFF);
			}
			break;

			case IL_OPCODE_ARGS_INT16:
			{
				fprintf(outstream, "%d",
				        (int)(ILInt16)(IL_READ_UINT16(temp + args)));
			}
			break;

			case IL_OPCODE_ARGS_UINT16:
			{
				fprintf(outstream, "%lu",
				        (unsigned long)(IL_READ_UINT16(temp + args)));
			}
			break;

			case IL_OPCODE_ARGS_INT32:
			{
				fprintf(outstream, "%ld",
				        (long)(ILInt32)(IL_READ_UINT32(temp + args)));
			}
			break;

			case IL_OPCODE_ARGS_INT64:
			{
				fprintf(outstream, "0x%08lx%08lX",
				        (unsigned long)(IL_READ_UINT32(temp + args + 4)),
						(unsigned long)(IL_READ_UINT32(temp + args)));
			}
			break;

			case IL_OPCODE_ARGS_FLOAT32:
			{
				fprintf(outstream, "float32(0x%02X%02X%02X%02X)",
						(((int)(temp[args + 3])) & 0xFF),
						(((int)(temp[args + 2])) & 0xFF),
						(((int)(temp[args + 1])) & 0xFF),
						(((int)(temp[args + 0])) & 0xFF));
			}
			break;

			case IL_OPCODE_ARGS_FLOAT64:
			{
				fprintf(outstream,
						"float64(0x%02X%02X%02X%02X%02X%02X%02X%02X)",
						(((int)(temp[args + 7])) & 0xFF),
						(((int)(temp[args + 6])) & 0xFF),
						(((int)(temp[args + 5])) & 0xFF),
						(((int)(temp[args + 4])) & 0xFF),
						(((int)(temp[args + 3])) & 0xFF),
						(((int)(temp[args + 2])) & 0xFF),
						(((int)(temp[args + 1])) & 0xFF),
						(((int)(temp[args + 0])) & 0xFF));
			}
			break;

			case IL_OPCODE_ARGS_TOKEN:
			case IL_OPCODE_ARGS_NEW:
			{
				DumpToken(image, outstream, flags,
					      (unsigned long)(IL_READ_UINT32(temp + args)), 0);
			}
			break;

			case IL_OPCODE_ARGS_LDTOKEN:
			{
				DumpToken(image, outstream, flags,
					      (unsigned long)(IL_READ_UINT32(temp + args)), 1);
			}
			break;

			case IL_OPCODE_ARGS_SHORT_VAR:
			case IL_OPCODE_ARGS_SHORT_ARG:
			{
				fprintf(outstream, "%d", ((int)(temp[args])) & 0xFF);
			}
			break;

			case IL_OPCODE_ARGS_LONG_VAR:
			case IL_OPCODE_ARGS_LONG_ARG:
			case IL_OPCODE_ARGS_ANN_DEAD:
			case IL_OPCODE_ARGS_ANN_LIVE:
			case IL_OPCODE_ARGS_ANN_ARG:
			{
				fprintf(outstream, "%lu",
						(unsigned long)(IL_READ_UINT16(temp + args)));
			}
			break;

			case IL_OPCODE_ARGS_SHORT_JUMP:
			{
				dest = (unsigned long)(((long)offset) + 2 +
									   (long)(ILInt8)(temp[1]));
				fprintf(outstream, "?L%lx", dest + addr);
			}
			break;

			case IL_OPCODE_ARGS_LONG_JUMP:
			{
				dest = (unsigned long)(((long)offset) + 5 +
									   (long)(IL_READ_UINT32(temp + 1)));
				fprintf(outstream, "?L%lx", dest + addr);
			}
			break;

			case IL_OPCODE_ARGS_CALL:
			{
				DumpToken(image, outstream, flags,
					      (unsigned long)(IL_READ_UINT32(temp + args)), 0);
			}
			break;

			case IL_OPCODE_ARGS_CALLI:
			{
				DumpToken(image, outstream, flags,
					      (unsigned long)(IL_READ_UINT32(temp + args)), 0);
			}
			break;

			case IL_OPCODE_ARGS_CALLVIRT:
			{
				DumpToken(image, outstream, flags,
					      (unsigned long)(IL_READ_UINT32(temp + args)), 0);
			}
			break;

			case IL_OPCODE_ARGS_SWITCH:
			{
				putc('(', outstream);
				numItems = (unsigned long)(IL_READ_UINT32(temp + args));
				for(item = 0; item < numItems; ++item)
				{
					dest = (unsigned long)(((long)offset) + 5 +
							   ((long)numItems) * 4 +
						   	   (long)(IL_READ_UINT32(temp + args + 4 +
							   						 item * 4)));
					if(item != 0)
					{
						putc(',', outstream);
						putc(' ', outstream);
					}
					fprintf(outstream, "?L%lx", dest + addr);
				}
				putc(')', outstream);
			}
			break;

			case IL_OPCODE_ARGS_STRING:
			{
			    dest = (unsigned long)(IL_READ_UINT32(temp + args));
				if((dest & IL_META_TOKEN_MASK) == IL_META_TOKEN_STRING)
				{
					const char *str;
					ILUInt32 strLen;
					dest &= ~IL_META_TOKEN_MASK;
					str = ILImageGetUserString(image, dest, &strLen);
					if(str)
					{
						ILDumpUnicodeString(outstream, str, strLen);
					}
					else
					{
						fprintf(outstream, "#%lx",
							    (unsigned long)(IL_READ_UINT32(temp + args)));
					}
				}
				else
				{
					fprintf(outstream, "#%lx",
						    (unsigned long)(IL_READ_UINT32(temp + args)));
				}
			}
			break;

			case IL_OPCODE_ARGS_ANN_DATA:
			{
				if(temp[0] == (unsigned char)IL_OP_ANN_DATA_S)
				{
					numItems = (((unsigned long)(temp[args])) & 0xFF);
					++args;
				}
				else
				{
					numItems = (unsigned long)(IL_READ_UINT32(temp + args));
					args += 4;
				}
				putc('(', outstream);
				for(item = 0; item < numItems; ++item)
				{
					if(item != 0)
					{
						putc(' ', outstream);
					}
					fprintf(outstream, "%02X",
							(((int)(temp[args + item])) & 0xFF));
				}
				putc(')', outstream);
			}
			break;

			case IL_OPCODE_ARGS_ANN_REF:
			{
				if(temp[0] == (unsigned char)IL_OP_ANN_REF_S)
				{
					fprintf(outstream, "%d",
							(((int)(temp[args])) & 0xFF));
				}
				else
				{
					fprintf(outstream, "%lu",
							(unsigned long)(IL_READ_UINT16(temp + args)));
				}
			}
			break;

			case IL_OPCODE_ARGS_ANN_PHI:
			{
				numItems =
					(((unsigned long)(IL_READ_UINT16(temp + args))) & 0xFF);
				++args;
				fprintf(outstream, "%lu", numItems);
				for(item = 0; item < numItems; ++item)
				{
					fprintf(outstream, " %lu",
							(unsigned long)(IL_READ_UINT16(temp + args +
														   item * 2)));
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
truncated:
	fprintf(outstream, "\t\t// truncated instruction\n");
	goto cleanup;
}

/*
 * Dump the local variables associated with a method.
 */
static void DumpLocals(ILImage *image, FILE *outstream,
					   ILStandAloneSig *sig, int flags)
{
	ILType *locals;
	ILType *type;
	unsigned long num;
	unsigned long index;

	/* Dump the locals */
	locals = ILStandAloneSigGetType(sig);
	num = ILTypeNumLocals(locals);
	for(index = 0; index < num; ++index)
	{
		if(index != 0)
		{
			fputs(",\n\t\t            ", outstream);
		}
		type = ILTypeGetLocalWithPrefixes(locals, index);
		ILDumpType(outstream, image, type, flags);
	}
}

/*
 * Dump the custom attributes on the parameters.
 */
static void DumpParameterAttributes(ILImage *image, FILE *outstream,
									ILMethod *method, int flags)
{
	ILParameter *param = 0;
	ILAttribute *attr;
	while((param = ILMethodNextParam(method, param)) != 0)
	{
		attr = ILProgramItemNextAttribute(ILToProgramItem(param), 0);
		if(attr || ILConstantGetFromOwner(ILToProgramItem(param)) != 0)
		{
			fprintf(outstream, "\t\t.param [%ld]",
					(long)(ILParameter_Num(param)));
			ILDumpConstant(outstream, ILToProgramItem(param), 0);
			putc('\n', outstream);
			ILDAsmDumpCustomAttrs(image, outstream, flags, 2,
								  ILToProgramItem(param));
		}
	}
}

void ILDAsmDumpMethod(ILImage *image, FILE *outstream,
					  ILMethod *method, int flags,
					  int isEntryPoint, int dumpOffsets)
{
	unsigned long addr;
	ILMethodCode code;
	ILException *clauses;
	ILException *tempClause;

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
	DumpParameterAttributes(image, outstream, method, flags);
	if(isEntryPoint)
	{
		fputs("\t\t.entrypoint\n", outstream);
	}
	fprintf(outstream, "\t\t.maxstack  %lu\n", (unsigned long)(code.maxStack));
	if(code.localVarSig)
	{
		fprintf(outstream, "\t\t.locals    %s(",
				(code.initLocals ? "init " : ""));
		DumpLocals(image, outstream, code.localVarSig, flags);
		fputs(")\n", outstream);
	}

	/* Dump the instructions within the method */
	if(!DumpInstructions(image, outstream, (unsigned char *)(code.code),
						 code.codeLen, addr, clauses, flags, dumpOffsets))
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
		if((tempClause->flags & IL_META_EXCEPTION_FILTER) != 0)
		{
			/* Filter clause */
			fprintf(outstream, " filter ?L%lx", tempClause->extraArg + addr);
		}
		else if((tempClause->flags & IL_META_EXCEPTION_FINALLY) != 0)
		{
			/* Finally clause */
			fprintf(outstream, " finally");
		}
		else if((tempClause->flags & IL_META_EXCEPTION_FAULT) != 0)
		{
			/* Fault clause */
			fprintf(outstream, " fault");
		}
		else
		{
			/* Catch clause */
			fputs(" catch ", outstream);
			DumpToken(image, outstream, flags | ILDASM_SUPPRESS_PREFIX,
					  tempClause->extraArg, 0);
		}
		fprintf(outstream, " handler ?L%lx to ?L%lx\n",
				tempClause->handlerOffset + addr,
				tempClause->handlerOffset + tempClause->handlerLength + addr);
		tempClause = tempClause->next;
	}

	/* Free the exception list and exit */
	ILMethodFreeExceptions(clauses);
}

#ifdef	__cplusplus
};
#endif
