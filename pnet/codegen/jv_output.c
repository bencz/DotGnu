/*
 * jv_output.c - Assembly code output routines for Java bytecode.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#include "cg_nodes.h"
#include "il_dumpasm.h"

#ifdef	__cplusplus
extern	"C" {
#endif

extern ILOpcodeInfo const ILJavaOpcodeTable[];

/*
 * Java-specific information for code generation.
 */
struct _tagILJavaGenInfo
{
	unsigned  numArgs;
	unsigned  totalLocals;
	unsigned  maxLocals;
	unsigned  nextLocalSlot;
	unsigned *localMap;
};

void JavaGenInit(ILGenInfo *info)
{
	info->javaInfo = (ILJavaGenInfo *)ILCalloc(1, sizeof(ILJavaGenInfo));
	if(!(info->javaInfo))
	{
		ILGenOutOfMemory(info);
	}
}

void JavaGenDestroy(ILGenInfo *info)
{
	if(info->javaInfo)
	{
		if(info->javaInfo->localMap)
		{
			ILFree(info->javaInfo->localMap);
		}
		ILFree(info->javaInfo);
	}
}

void JavaGenClear(ILGenInfo *info)
{
	/* Nothing to do here at the moment */
}

void JavaGenSimple(ILGenInfo *info, int opcode)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\t%s\n", ILJavaOpcodeTable[opcode].name);
	}
}

void JavaGenInt32(ILGenInfo *info, ILInt32 value)
{
	if(!(info->asmOutput))
	{
		return;
	}
	if(value >= ((ILInt32)(-1)) && value <= ((ILInt32)5))
	{
		fprintf(info->asmOutput, "\t%s\n",
				ILJavaOpcodeTable[JAVA_OP_ICONST_0 + value].name);
	}
	else if(value >= ((ILInt32)(-128)) && value <= ((ILInt32)127))
	{
		fprintf(info->asmOutput, "\tbipush\t%ld\n", (long)value);
	}
	else if(value >= ((ILInt32)(-32768)) && value <= ((ILInt32)32767))
	{
		fprintf(info->asmOutput, "\tsipush\t%ld\n", (long)value);
	}
	else
	{
		fprintf(info->asmOutput, "\tldc\tint32(%ld)\n", (long)value);
	}
}

void JavaGenUInt32(ILGenInfo *info, ILUInt32 value)
{
	JavaGenInt32(info, (ILInt32)value);
}

void JavaGenInt64(ILGenInfo *info, ILInt64 value)
{
	if(!(info->asmOutput))
	{
		return;
	}
	if(value == 0)
	{
		fputs("\tlconst_0\n", info->asmOutput);
	}
	else if(value == 1)
	{
		fputs("\tlconst_1\n", info->asmOutput);
	}
	else if(value >= ((ILInt64)IL_MIN_INT32) &&
	        value <= ((ILInt64)IL_MAX_INT32))
	{
		JavaGenInt32(info, (ILInt32)value);
		fputs("\ti2l\n", info->asmOutput);
	}
	else
	{
		fprintf(info->asmOutput, "\tldc2_w\tint64(0x%08lX%08lX)\n",
				(long)((value >> 32) & (ILInt64)0xFFFFFFFF),
				(long)(value & (ILInt64)0xFFFFFFFF));
	}
}

void JavaGenUInt64(ILGenInfo *info, ILUInt64 value)
{
	JavaGenInt64(info, (ILInt64)value);
}

void JavaGenFloat32(ILGenInfo *info, ILFloat value)
{
	if(!(info->asmOutput))
	{
		return;
	}
	if(value == (ILFloat)0.0)
	{
		fputs("\tfconst_0\n", info->asmOutput);
	}
	else if(value == (ILFloat)1.0)
	{
		fputs("\tfconst_1\n", info->asmOutput);
	}
	else if(value == (ILFloat)2.0)
	{
		fputs("\tfconst_2\n", info->asmOutput);
	}
	else
	{
		unsigned char bytes[4];
		IL_WRITE_FLOAT(bytes, value);
		fprintf(info->asmOutput, "\tldc\tfloat32(0x%02X%02X%02X%02X)\n",
				bytes[3], bytes[2], bytes[1], bytes[0]);
	}
}

void JavaGenFloat64(ILGenInfo *info, ILDouble value)
{
	if(!(info->asmOutput))
	{
		return;
	}
	if(value == (ILDouble)0.0)
	{
		fputs("\tdconst_0\n", info->asmOutput);
	}
	else if(value == (ILDouble)1.0)
	{
		fputs("\tdconst_1\n", info->asmOutput);
	}
	else
	{
		unsigned char bytes[8];
		IL_WRITE_DOUBLE(bytes, value);
		fprintf(info->asmOutput,
				"\tldc2_w\tfloat64(0x%02X%02X%02X%02X%02X%02X%02X%02X)\n",
				bytes[7], bytes[6], bytes[5], bytes[4],
				bytes[3], bytes[2], bytes[1], bytes[0]);
	}
}

void JavaGenStringConst(ILGenInfo *info, const char *str, int len)
{
	if(len < 0)
	{
		len = strlen(str);
	}
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tldc\t\"");
		while(len > 0)
		{
			if(*str == '"' || *str == '\\')
			{
				putc('\\', info->asmOutput);
				putc(*str, info->asmOutput);
			}
			else if(*str >= ' ' && *str < (char)0x7F)
			{
				putc(*str, info->asmOutput);
			}
			else
			{
				putc('\\', info->asmOutput);
				putc('0' + ((*str >> 6) & 0x03), info->asmOutput);
				putc('0' + ((*str >> 3) & 0x07), info->asmOutput);
				putc('0' + (*str & 0x07), info->asmOutput);
			}
			++str;
			--len;
		}
		putc('"', info->asmOutput);
		putc('\n', info->asmOutput);
	}
}

void JavaGenJump(ILGenInfo *info, int opcode, ILLabel *label)
{
	if(*label == ILLabel_Undefined)
	{
		*label = (info->nextLabel)++;
	}
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\t%s\t?L%lu\n",
				ILJavaOpcodeTable[opcode].name, *label);
	}
}

void JavaGenLabel(ILGenInfo *info, ILLabel *label)
{
	if(*label == ILLabel_Undefined)
	{
		*label = (info->nextLabel)++;
	}
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "?L%lu:\n", *label);
	}
}

static void StoreLocal(ILGenInfo *info, unsigned varNum,
					   ILMachineType type)
{
	if(!(info->asmOutput))
	{
		return;
	}
	switch(type)
	{
		case ILMachineType_Void:	break;

		case ILMachineType_Boolean:
		case ILMachineType_Int8:
		case ILMachineType_UInt8:
		case ILMachineType_Int16:
		case ILMachineType_UInt16:
		case ILMachineType_Char:
		case ILMachineType_Int32:
		case ILMachineType_UInt32:
		case ILMachineType_NativeInt:
		case ILMachineType_NativeUInt:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\tistore_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\tistore\t%u\n", varNum);
			}
		}
		break;

		case ILMachineType_Int64:
		case ILMachineType_UInt64:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\tlstore_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\tlstore\t%u\n", varNum);
			}
		}
		break;

		case ILMachineType_Float32:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\tfstore_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\tfstore\t%u\n", varNum);
			}
		}
		break;

		case ILMachineType_Float64:
		case ILMachineType_NativeFloat:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\tdstore_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\tdstore\t%u\n", varNum);
			}
		}
		break;

		case ILMachineType_Decimal:
		case ILMachineType_String:
		case ILMachineType_ObjectRef:
		case ILMachineType_UnmanagedPtr:
		case ILMachineType_ManagedPtr:
		case ILMachineType_ManagedValue:
		case ILMachineType_TransientPtr:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\tastore_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\tastore\t%u\n", varNum);
			}
		}
		break;
	}
}

static void LoadLocal(ILGenInfo *info, unsigned varNum,
					  ILMachineType type)
{
	if(!(info->asmOutput))
	{
		return;
	}
	switch(type)
	{
		case ILMachineType_Void:	break;

		case ILMachineType_Boolean:
		case ILMachineType_Int8:
		case ILMachineType_UInt8:
		case ILMachineType_Int16:
		case ILMachineType_UInt16:
		case ILMachineType_Char:
		case ILMachineType_Int32:
		case ILMachineType_UInt32:
		case ILMachineType_NativeInt:
		case ILMachineType_NativeUInt:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\tiload_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\tiload\t%u\n", varNum);
			}
		}
		break;

		case ILMachineType_Int64:
		case ILMachineType_UInt64:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\tlload_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\tlload\t%u\n", varNum);
			}
		}
		break;

		case ILMachineType_Float32:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\tfload_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\tfload\t%u\n", varNum);
			}
		}
		break;

		case ILMachineType_Float64:
		case ILMachineType_NativeFloat:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\tdload_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\tdload\t%u\n", varNum);
			}
		}
		break;

		case ILMachineType_Decimal:
		case ILMachineType_String:
		case ILMachineType_ObjectRef:
		case ILMachineType_UnmanagedPtr:
		case ILMachineType_ManagedPtr:
		case ILMachineType_ManagedValue:
		case ILMachineType_TransientPtr:
		{
			if(varNum < 4)
			{
				fprintf(info->asmOutput, "\taload_%u\n", varNum);
			}
			else
			{
				fprintf(info->asmOutput, "\taload\t%u\n", varNum);
			}
		}
		break;
	}
}

void JavaGenStoreLocal(ILGenInfo *info, unsigned varNum,
					   ILMachineType type)
{
	StoreLocal(info, info->javaInfo->localMap
						[varNum + info->javaInfo->numArgs], type);
}

void JavaGenLoadLocal(ILGenInfo *info, unsigned varNum,
					  ILMachineType type)
{
	LoadLocal(info, info->javaInfo->localMap
						[varNum + info->javaInfo->numArgs], type);
}

void JavaGenStoreArg(ILGenInfo *info, unsigned varNum,
					 ILMachineType type)
{
	StoreLocal(info, info->javaInfo->localMap[varNum], type);
}

void JavaGenLoadArg(ILGenInfo *info, unsigned varNum,
				    ILMachineType type)
{
	LoadLocal(info, info->javaInfo->localMap[varNum], type);
}

void JavaGenIncLocal(ILGenInfo *info, unsigned varNum, ILInt32 amount)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tiinc\t%u %ld\n",
				info->javaInfo->localMap[varNum + info->javaInfo->numArgs],
				(long)amount);
	}
}

void JavaGenRet(ILGenInfo *info, unsigned varNum)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tret\t%u\n",
				info->javaInfo->localMap[varNum + info->javaInfo->numArgs]);
	}
}

int JavaGenTypeSize(ILMachineType type)
{
	if(type == ILMachineType_Void)
	{
		return 0;
	}
	else if(type == ILMachineType_Int64 ||
			type == ILMachineType_UInt64 ||
			type == ILMachineType_Float64 ||
			type == ILMachineType_NativeFloat)
	{
		return 2;
	}
	else
	{
		return 1;
	}
}

void JavaGenDup(ILGenInfo *info, ILMachineType type)
{
	int size = JavaGenTypeSize(type);
	if(size == 1)
	{
		JavaGenSimple(info, JAVA_OP_DUP);
	}
	else if(size == 2)
	{
		JavaGenSimple(info, JAVA_OP_DUP2);
	}
	JavaGenAdjust(info, size);
}

void JavaGenLoadArray(ILGenInfo *info, ILMachineType type)
{
	switch(type)
	{
		case ILMachineType_Void: break;

		case ILMachineType_Boolean:
		case ILMachineType_Int8:
		{
			JavaGenSimple(info, JAVA_OP_BALOAD);
		}
		break;

		case ILMachineType_UInt8:
		{
			JavaGenSimple(info, JAVA_OP_BALOAD);
			JavaGenInt32(info, 0xFF);
			JavaGenSimple(info, JAVA_OP_IAND);
		}
		break;

		case ILMachineType_Int16:
		{
			JavaGenSimple(info, JAVA_OP_SALOAD);
		}
		break;

		case ILMachineType_UInt16:
		case ILMachineType_Char:
		{
			JavaGenSimple(info, JAVA_OP_CALOAD);
		}
		break;

		case ILMachineType_Int32:
		case ILMachineType_UInt32:
		{
			JavaGenSimple(info, JAVA_OP_IALOAD);
		}
		break;

		case ILMachineType_Int64:
		case ILMachineType_UInt64:
		{
			JavaGenSimple(info, JAVA_OP_LALOAD);
		}
		break;

		case ILMachineType_NativeInt:
		case ILMachineType_NativeUInt:
		{
			JavaGenSimple(info, JAVA_OP_IALOAD);
		}
		break;

		case ILMachineType_Float32:
		{
			JavaGenSimple(info, JAVA_OP_FALOAD);
		}
		break;

		case ILMachineType_Float64:
		case ILMachineType_NativeFloat:
		{
			JavaGenSimple(info, JAVA_OP_DALOAD);
		}
		break;

		case ILMachineType_Decimal:
		case ILMachineType_ManagedValue:
		case ILMachineType_String:
		case ILMachineType_ObjectRef:
		case ILMachineType_UnmanagedPtr:
		case ILMachineType_ManagedPtr:
		case ILMachineType_TransientPtr:
		{
			JavaGenSimple(info, JAVA_OP_AALOAD);
		}
		break;
	}
}

void JavaGenStoreArray(ILGenInfo *info, ILMachineType type)
{
	switch(type)
	{
		case ILMachineType_Void: break;

		case ILMachineType_Boolean:
		case ILMachineType_Int8:
		case ILMachineType_UInt8:
		{
			JavaGenSimple(info, JAVA_OP_BASTORE);
		}
		break;

		case ILMachineType_Int16:
		{
			JavaGenSimple(info, JAVA_OP_SASTORE);
		}
		break;

		case ILMachineType_UInt16:
		case ILMachineType_Char:
		{
			JavaGenSimple(info, JAVA_OP_CASTORE);
		}
		break;

		case ILMachineType_Int32:
		case ILMachineType_UInt32:
		{
			JavaGenSimple(info, JAVA_OP_IASTORE);
		}
		break;

		case ILMachineType_Int64:
		case ILMachineType_UInt64:
		{
			JavaGenSimple(info, JAVA_OP_LASTORE);
		}
		break;

		case ILMachineType_NativeInt:
		case ILMachineType_NativeUInt:
		{
			JavaGenSimple(info, JAVA_OP_IASTORE);
		}
		break;

		case ILMachineType_Float32:
		{
			JavaGenSimple(info, JAVA_OP_FASTORE);
		}
		break;

		case ILMachineType_Float64:
		case ILMachineType_NativeFloat:
		{
			JavaGenSimple(info, JAVA_OP_DASTORE);
		}
		break;

		case ILMachineType_Decimal:
		case ILMachineType_ManagedValue:
		case ILMachineType_String:
		case ILMachineType_ObjectRef:
		case ILMachineType_UnmanagedPtr:
		case ILMachineType_ManagedPtr:
		case ILMachineType_TransientPtr:
		{
			JavaGenSimple(info, JAVA_OP_AASTORE);
		}
		break;
	}
}

void JavaGenCallByName(ILGenInfo *info, const char *className,
					   const char *methodName, const char *signature)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tinvokestatic\t\"%s\" \"%s\" \"%s\"\n",
				className, methodName, signature);
	}
}

void JavaGenCallIntrinsic(ILGenInfo *info, const char *methodName,
						  const char *signature)
{
	JavaGenCallByName(info, "System/Intrinsics/Operations",
			          methodName, signature);
}

void JavaGenCallVirtual(ILGenInfo *info, const char *className,
						const char *methodName, const char *signature)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tinvokevirtual\t\"%s\" \"%s\" \"%s\"\n",
				className, methodName, signature);
	}
}

void JavaGenCallVirtIntrinsic(ILGenInfo *info, const char *className,
						      const char *methodName, const char *signature)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tinvokevirtual\t\""
						"System/Intrinsics/%s\" \"%s\" \"%s\"\n",
				className, methodName, signature);
	}
}

void JavaGenCallInterface(ILGenInfo *info, const char *className,
						  const char *methodName, const char *signature,
						  long numArgs)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput,
				"\tinvokeinterface\t\"%s\" \"%s\" \"%s\" %ld\n",
				className, methodName, signature, numArgs);
	}
}

void JavaGenCallByMethod(ILGenInfo *info, ILMethod *method)
{
	if(info->asmOutput)
	{
		fputs("\tinvokestatic\t", info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image,
						 ILMethod_Signature(method),
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		putc('\n', info->asmOutput);
	}
}

void JavaGenCallVirtByMethod(ILGenInfo *info, ILMethod *method)
{
	if(info->asmOutput)
	{
		fputs("\tinvokevirtual\t", info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image,
						 ILMethod_Signature(method),
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		putc('\n', info->asmOutput);
	}
}

void JavaGenCallInterfaceByMethod(ILGenInfo *info, ILMethod *method,
								  long numArgs)
{
	if(info->asmOutput)
	{
		fputs("\tinvokeinterface\t", info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image,
						 ILMethod_Signature(method),
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		fprintf(info->asmOutput, " %ld\n", numArgs);
	}
}

void JavaGenCallSpecialByMethod(ILGenInfo *info, ILMethod *method)
{
	if(info->asmOutput)
	{
		fputs("\tinvokespecial\t", info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image,
						 ILMethod_Signature(method),
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		putc('\n', info->asmOutput);
	}
}

void JavaGenCallMethod(ILGenInfo *info, ILMethod *method, long startArgs)
{
	if(ILMethod_IsVirtual(method))
	{
		if(ILClass_IsInterface(ILMethod_Owner(method)))
		{
			JavaGenCallInterfaceByMethod(info, method,
										 info->stackHeight - startArgs);
		}
		else
		{
			JavaGenCallVirtByMethod(info, method);
		}
	}
	else if(ILMethod_IsStatic(method))
	{
		JavaGenCallByMethod(info, method);
	}
	else
	{
		JavaGenCallSpecialByMethod(info, method);
	}
}

void JavaGenNewObj(ILGenInfo *info, const char *className)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tnew\t\"%s\"\n", className);
	}
}

void JavaGenNewIntrinsic(ILGenInfo *info, const char *className)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tnew\t\"System/Intrinsics/%s\"\n",
				className);
	}
}

void JavaGenCallCtor(ILGenInfo *info, const char *className,
					 const char *methodName, const char *signature)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tinvokespecial\t\"%s\" \"%s\" \"%s\"\n",
				className, methodName, signature);
	}
}

void JavaGenCallCtorIntrinsic(ILGenInfo *info, const char *className,
					 		  const char *methodName, const char *signature)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tinvokespecial\t"
						"\"System/Intrinsics/%s\" \"%s\" \"%s\"\n",
				className, methodName, signature);
	}
}

void JavaGenClassRef(ILGenInfo *info, int opcode, ILClass *classInfo)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\t%s\t", ILJavaOpcodeTable[opcode].name);
		ILDumpClassName(info->asmOutput, info->image,
						classInfo, IL_DUMP_QUOTE_NAMES);
		putc('\n', info->asmOutput);
	}
}

void JavaGenTypeRef(ILGenInfo *info, int opcode, ILType *type)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\t%s\t", ILJavaOpcodeTable[opcode].name);
		if(ILType_IsClass(type) || ILType_IsValueType(type))
		{
			ILDumpClassName(info->asmOutput, info->image,
							ILType_ToClass(type), IL_DUMP_QUOTE_NAMES);
		}
		else
		{
			ILDumpType(info->asmOutput, info->image, type, IL_DUMP_QUOTE_NAMES);
		}
		putc('\n', info->asmOutput);
	}
}

void JavaGenClassName(ILGenInfo *info, int opcode, const char *className)
{
	/* TODO */
}

void JavaGenFieldRef(ILGenInfo *info, int opcode, ILField *field)
{
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		fputs(ILJavaOpcodeTable[opcode].name, info->asmOutput);
		putc('\t', info->asmOutput);
		ILDumpType(info->asmOutput, info->image, ILField_Type(field),
				   IL_DUMP_QUOTE_NAMES);
		putc(' ', info->asmOutput);
		ILDumpClassName(info->asmOutput, info->image, ILField_Owner(field),
						IL_DUMP_QUOTE_NAMES);
		fputs("::", info->asmOutput);
		ILDumpIdentifier(info->asmOutput, ILField_Name(field), 0,
						 IL_DUMP_QUOTE_NAMES);
		putc('\n', info->asmOutput);
	}
}

void JavaGenNewArray(ILGenInfo *info, ILType *elemType)
{
	if(!(info->asmOutput))
	{
		return;
	}
	if(ILType_IsPrimitive(elemType))
	{
		switch(ILType_ToElement(elemType))
		{
			case IL_META_ELEMTYPE_BOOLEAN:
			{
				fputs("\tnewarray bool\n", info->asmOutput);
			}
			break;

			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			{
				fputs("\tnewarray int8\n", info->asmOutput);
			}
			break;

			case IL_META_ELEMTYPE_I2:
			{
				fputs("\tnewarray int16\n", info->asmOutput);
			}
			break;

			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			{
				fputs("\tnewarray char\n", info->asmOutput);
			}
			break;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
			{
				fputs("\tnewarray int32\n", info->asmOutput);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
			{
				fputs("\tnewarray int64\n", info->asmOutput);
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				fputs("\tnewarray float32\n", info->asmOutput);
			}
			break;

			case IL_META_ELEMTYPE_R8:
			case IL_META_ELEMTYPE_R:
			{
				fputs("\tnewarray float64\n", info->asmOutput);
			}
			break;
		}
	}
	else
	{
		fputs("\tanewarray ", info->asmOutput);
		ILDumpType(info->asmOutput, info->image, elemType,
				   IL_DUMP_QUOTE_NAMES);
		putc('\n', info->asmOutput);
	}
}

void JavaGenNewMultiArray(ILGenInfo *info, ILType *type, int rank)
{
	if(info->asmOutput)
	{
		fputs("\tmultianewarray\t", info->asmOutput);
		ILDumpType(info->asmOutput, info->image, type,
				   IL_DUMP_QUOTE_NAMES);
		fprintf(info->asmOutput, " %d\n", rank);
	}
}

void JavaGenReturnInsn(ILGenInfo *info, ILMachineType type)
{
	switch(type)
	{
		case ILMachineType_Void:
		{
			JavaGenSimple(info, JAVA_OP_RETURN);
		}
		break;

		case ILMachineType_Boolean:
		case ILMachineType_Int8:
		case ILMachineType_UInt8:
		case ILMachineType_Int16:
		case ILMachineType_UInt16:
		case ILMachineType_Char:
		case ILMachineType_Int32:
		case ILMachineType_UInt32:
		case ILMachineType_NativeInt:
		case ILMachineType_NativeUInt:
		{
			JavaGenSimple(info, JAVA_OP_IRETURN);
		}
		break;

		case ILMachineType_Int64:
		case ILMachineType_UInt64:
		{
			JavaGenSimple(info, JAVA_OP_LRETURN);
		}
		break;

		case ILMachineType_Float32:
		{
			JavaGenSimple(info, JAVA_OP_FRETURN);
		}
		break;

		case ILMachineType_Float64:
		case ILMachineType_NativeFloat:
		{
			JavaGenSimple(info, JAVA_OP_DRETURN);
		}
		break;

		default:
		{
			JavaGenSimple(info, JAVA_OP_ARETURN);
		}
		break;
	}
}

void JavaGenStartFrame(ILGenInfo *info)
{
	info->javaInfo->numArgs = 0;
	info->javaInfo->totalLocals = 0;
	info->javaInfo->nextLocalSlot = 0;
}

void JavaGenStartLocals(ILGenInfo *info)
{
	info->javaInfo->numArgs = info->javaInfo->totalLocals;
}

void JavaGenAddFrameSlot(ILGenInfo *info, ILMachineType machineType)
{
	if(info->javaInfo->totalLocals >= info->javaInfo->maxLocals)
	{
		unsigned *newMap =
			(unsigned *)ILRealloc(info->javaInfo->localMap,
					  sizeof(unsigned) * (info->javaInfo->maxLocals + 8));
		if(!newMap)
		{
			ILGenOutOfMemory(info);
		}
		info->javaInfo->maxLocals += 8;
		info->javaInfo->localMap = newMap;
	}
	info->javaInfo->localMap[(info->javaInfo->totalLocals)++] =
			info->javaInfo->nextLocalSlot;
	info->javaInfo->nextLocalSlot += (unsigned)(JavaGenTypeSize(machineType));
}

unsigned JavaGenNumLocals(ILGenInfo *info)
{
	return info->javaInfo->nextLocalSlot;
}

void JavaGenFlush(ILGenInfo *info)
{
}

char *JavaGetClassName(ILGenInfo *info, ILClass *classInfo)
{
	const char *name = ILClass_Name(classInfo);
	const char *namespace = ILClass_Namespace(classInfo);
	char *result;
	int len;
	char ch;
	if(!namespace)
	{
		return JavaStrAppend(info, 0, name);
	}
	len = strlen(namespace) + strlen(name) + 2;
	result = (char *)ILMalloc(len);
	if(!result)
	{
		ILGenOutOfMemory(info);
	}
	len = 0;
	while((ch = namespace[len]) != '\0')
	{
		if(ch == '.')
		{
			result[len++] = '/';
		}
		else
		{
			result[len++] = ch;
		}
	}
	result[len++] = '/';
	strcpy(result + len, name);
	return result;
}

char *JavaStrAppend(ILGenInfo *info, char *str1, const char *str2)
{
	int len;
	char *result;
	if(!str1)
	{
		len = strlen(str2) + 1;
		result = (char *)ILMalloc(len);
		if(!result)
		{
			ILGenOutOfMemory(info);
		}
		strcpy(result, str2);
		return result;
	}
	else
	{
		len = strlen(str1);
		result = (char *)ILRealloc(str1, len + strlen(str2) + 1);
		if(!result)
		{
			ILGenOutOfMemory(info);
		}
		strcpy(result + len, str2);
		return result;
	}
}

void JavaGenSwitchStart(ILGenInfo *info, int opcode, 
						ILLabel *label, ILUInt32 offset)
{
	if(*label == ILLabel_Undefined)
	{
		*label = (info->nextLabel)++;
	}
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\t%s ?L%lu (", 
				ILJavaOpcodeTable[opcode].name, *label);
		if(opcode == JAVA_OP_TABLESWITCH)
		{
			fprintf(info->asmOutput, " %u :", offset);
		}
		putc('\n', info->asmOutput);
	}
}

void JavaGenSwitchRef(ILGenInfo *info, ILLabel *label, int comma)
{
	if(*label == ILLabel_Undefined)
	{
		*label = (info->nextLabel)++;
	}
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\t\t?L%lu%s\n", *label,
				(comma ? "," : ""));
	}
}

void JavaGenSwitchLookupRef(ILGenInfo *info, ILInt32 value,
						    ILLabel *label, int comma)
{
	if(*label == ILLabel_Undefined)
	{
		*label = (info->nextLabel)++;
	}
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\t\t%ld : ?L%lu%s\n",
				(long)value, *label, (comma ? "," : ""));
	}
}

void JavaGenSwitchEnd(ILGenInfo *info)
{
	if(info->asmOutput)
	{
		fputs("\t)\n", info->asmOutput);
	}
}

#ifdef	__cplusplus
};
#endif
