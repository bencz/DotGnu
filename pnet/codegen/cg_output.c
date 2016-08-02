/*
 * cg_output.c - Assembly code output routines.
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

static ILType *GetLastNestedArray(ILType *arrayType)
{
	while(arrayType)
	{
		ILType *elementType = ILTypeGetElemType(arrayType);

		if(!ILType_IsArray(elementType))
		{
			break;
		}
		arrayType = elementType;
	}
	return arrayType;
}

void ILGenSimple(ILGenInfo *info, int opcode)
{
	if(info->asmOutput)
	{
		if(opcode < IL_OP_PREFIX)
		{
			fprintf(info->asmOutput, "\t%s\n",
					ILMainOpcodeTable[opcode].name);
		}
		else
		{
			fprintf(info->asmOutput, "\t%s\n",
					ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name);
		}
	}
}

void ILGenByteInsn(ILGenInfo *info, int opcode, int arg)
{
	if(info->asmOutput)
	{
		if(opcode < IL_OP_PREFIX)
		{
			fprintf(info->asmOutput, "\t%s\t%d\n",
					ILMainOpcodeTable[opcode].name, arg);
		}
		else
		{
			fprintf(info->asmOutput, "\t%s\t%d\n",
					ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name, arg);
		}
	}
}

void ILGenShortInsn(ILGenInfo *info, int opcode, ILUInt32 arg)
{
	if(info->asmOutput)
	{
		if(opcode < IL_OP_PREFIX)
		{
			fprintf(info->asmOutput, "\t%s\t%lu\n",
					ILMainOpcodeTable[opcode].name,
					(unsigned long)(arg & 0xFFFF));
		}
		else
		{
			fprintf(info->asmOutput, "\t%s\t%lu\n",
					ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name,
					(unsigned long)(arg & 0xFFFF));
		}
	}
}

void ILGenWordInsn(ILGenInfo *info, int opcode, ILUInt32 arg)
{
	if(info->asmOutput)
	{
		if(opcode < IL_OP_PREFIX)
		{
			fprintf(info->asmOutput, "\t%s\t%lu\n",
					ILMainOpcodeTable[opcode].name,
					(unsigned long)arg);
		}
		else
		{
			fprintf(info->asmOutput, "\t%s\t%lu\n",
					ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name,
					(unsigned long)arg);
		}
	}
}

void ILGenDWordInsn(ILGenInfo *info, int opcode, ILUInt64 arg)
{
	if(info->asmOutput)
	{
		if(opcode < IL_OP_PREFIX)
		{
			fprintf(info->asmOutput, "\t%s\t0x%08lx%08lx\n",
					ILMainOpcodeTable[opcode].name,
					(unsigned long)(arg >> 32),
					(unsigned long)(arg & 0xFFFFFFFF));
		}
		else
		{
			fprintf(info->asmOutput, "\t%s\t0x%08lx%08lx\n",
					ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name,
					(unsigned long)(arg >> 32),
					(unsigned long)(arg & 0xFFFFFFFF));
		}
	}
}

void ILGenLoadFloat32(ILGenInfo *info, ILFloat value)
{
	if(info->asmOutput)
	{
		unsigned char bytes[4];
		IL_WRITE_FLOAT(bytes, value);
		fprintf(info->asmOutput, "\tldc.r4\tfloat32(0x%02X%02X%02X%02X)\n",
				bytes[3], bytes[2], bytes[1], bytes[0]);
	}
}

void ILGenLoadFloat64(ILGenInfo *info, ILDouble value)
{
	if(info->asmOutput)
	{
		unsigned char bytes[8];
		if(value == (ILDouble)(ILFloat)value)
		{
			/* We can represent the constant as float32 */
			IL_WRITE_FLOAT(bytes, value);
			fprintf(info->asmOutput, "\tldc.r4\tfloat32(0x%02X%02X%02X%02X)\n",
					bytes[3], bytes[2], bytes[1], bytes[0]);
		}
		else
		{
			IL_WRITE_DOUBLE(bytes, value);
			fprintf(info->asmOutput,
					"\tldc.r8\tfloat64(0x%02X%02X%02X%02X%02X%02X%02X%02X)\n",
					bytes[7], bytes[6], bytes[5], bytes[4],
					bytes[3], bytes[2], bytes[1], bytes[0]);
		}
	}
}

void ILGenLoadString(ILGenInfo *info, const char *str, int len)
{
	if(len < 0)
	{
		len = strlen(str);
	}
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tldstr\t\"");
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

void ILGenAllocLocal(ILGenInfo *info, ILType *type, const char *name)
{
	if(info->asmOutput)
	{
		fputs("\t.locals init\t(", info->asmOutput);
		ILDumpType(info->asmOutput, info->image, type, IL_DUMP_QUOTE_NAMES);
		if(name)
		{
			putc(' ', info->asmOutput);
			putc('\'', info->asmOutput);
			fputs(name, info->asmOutput);
			putc('\'', info->asmOutput);
		}
		putc(')', info->asmOutput);
		putc('\n', info->asmOutput);
	}
}

void ILGenJump(ILGenInfo *info, int opcode, ILLabel *label)
{
	if(*label == ILLabel_Undefined)
	{
		*label = (info->nextLabel)++;
	}
	if(info->asmOutput)
	{
		if(opcode < IL_OP_PREFIX)
		{
			fprintf(info->asmOutput, "\t%s\t?L%lu\n",
					ILMainOpcodeTable[opcode].name, *label);
		}
		else
		{
			fprintf(info->asmOutput, "\t%s\t?L%lu\n",
					ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name, *label);
		}
	}
}

void ILGenLabel(ILGenInfo *info, ILLabel *label)
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

void ILGenLeaveLabel(ILGenInfo *info, ILLabel *label)
{
	if(*label == ILLabel_Undefined)
	{
		*label = (info->nextLabel)++;
	}
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, ".leave ?L%lu:\n", *label);
	}
}

ILLabel ILGenNewLabel(ILGenInfo *info)
{
	return (info->nextLabel)++;
}

void ILGenCallByName(ILGenInfo *info, const char *name)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tcall\t%s\n", name);
	}
}

void ILGenCallVirtual(ILGenInfo *info, const char *name)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tcallvirt\tinstance %s\n", name);
	}
}

void ILGenCallByMethod(ILGenInfo *info, ILMethod *method)
{
	if(info->asmOutput)
	{
		fputs("\tcall\t", info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image,
						 ILMethod_Signature(method),
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		putc('\n', info->asmOutput);
	}
}

void ILGenCallByMethodSig(ILGenInfo *info, ILMethod *method,
						  ILType *callSiteSig)
{
	if(info->asmOutput)
	{
		if(!callSiteSig)
		{
			callSiteSig = ILMethod_Signature(method);
		}
		fputs("\tcall\t", info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image, callSiteSig,
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		putc('\n', info->asmOutput);
	}
}

void ILGenCtorByMethod(ILGenInfo *info, ILMethod *method,
					   ILType *callSiteSig)
{
	if(info->asmOutput)
	{
		if(!callSiteSig)
		{
			callSiteSig = ILMethod_Signature(method);
		}
		fputs("\tnewobj\t", info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image, callSiteSig,
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		putc('\n', info->asmOutput);
	}
}

void ILGenCallVirtByMethod(ILGenInfo *info, ILMethod *method)
{
	if(info->asmOutput)
	{
		fputs("\tcallvirt\t", info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image,
	 					 ILMethod_Signature(method),
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		putc('\n', info->asmOutput);
	}
}

void ILGenCallVirtByMethodSig(ILGenInfo *info, ILMethod *method,
							  ILType *callSiteSig)
{
	if(info->asmOutput)
	{
		if(!callSiteSig)
		{
			callSiteSig = ILMethod_Signature(method);
		}
		fputs("\tcallvirt\t", info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image, callSiteSig,
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		putc('\n', info->asmOutput);
	}
}

void ILGenCallMethod(ILGenInfo *info, ILMethod *method)
{
	if(ILMethod_IsVirtual(method) &&
	   !ILClassIsValueType(ILMethod_Owner(method)))
	{
		ILGenCallVirtByMethod(info, method);
	}
	else
	{
		ILGenCallByMethod(info, method);
	}
}

void ILGenNewObj(ILGenInfo *info, const char *className,
				 const char *signature)
{
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tnewobj\tinstance void %s::.ctor%s\n",
				className, signature);
	}
}

void ILGenNewDelegate(ILGenInfo *info, ILClass *classInfo)
{
	if(info->asmOutput)
	{
		fputs("\tnewobj\tinstance void ", info->asmOutput);
		ILDumpClassName(info->asmOutput, info->image,
						classInfo, IL_DUMP_QUOTE_NAMES);
		fputs("::.ctor(class [.library]System.Object, native int)\n",
			  info->asmOutput);
	}
}

void ILGenLoadMethod(ILGenInfo *info, int opcode, ILMethod *method)
{
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		if(opcode < IL_OP_PREFIX)
		{
			fputs(ILMainOpcodeTable[opcode].name, info->asmOutput);
		}
		else
		{
			fputs(ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name,
				  info->asmOutput);
		}
		putc('\t', info->asmOutput);
		ILDumpMethodType(info->asmOutput, info->image,
	 					 ILMethod_Signature(method),
						 IL_DUMP_QUOTE_NAMES, ILMethod_Owner(method),
						 ILMethod_Name(method), 0);
		putc('\n', info->asmOutput);
	}
}

void ILGenClassToken(ILGenInfo *info, int opcode, ILClass *classInfo)
{
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		if(opcode < IL_OP_PREFIX)
		{
			fputs(ILMainOpcodeTable[opcode].name, info->asmOutput);
		}
		else
		{
			fputs(ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name,
				  info->asmOutput);
		}
		putc('\t', info->asmOutput);
		ILDumpClassName(info->asmOutput, info->image,
						classInfo, IL_DUMP_QUOTE_NAMES);
		putc('\n', info->asmOutput);
	}
}

void ILGenClassName(ILGenInfo *info, int opcode, const char *className)
{
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		if(opcode < IL_OP_PREFIX)
		{
			fputs(ILMainOpcodeTable[opcode].name, info->asmOutput);
		}
		else
		{
			fputs(ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name,
				  info->asmOutput);
		}
		putc('\t', info->asmOutput);
		fputs(className, info->asmOutput);
		putc('\n', info->asmOutput);
	}
}

void ILGenTypeToken(ILGenInfo *info, int opcode, ILType *type)
{
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		if(opcode < IL_OP_PREFIX)
		{
			fputs(ILMainOpcodeTable[opcode].name, info->asmOutput);
		}
		else
		{
			fputs(ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name,
				  info->asmOutput);
		}
		putc('\t', info->asmOutput);
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

void ILGenArrayNew(ILGenInfo *info, ILType *type)
{
	/* Convert primitive element types into their class form */
	if(ILType_IsPrimitive(type))
	{
		type = ILType_FromClass(ILTypeToClass(info, type));
	}

	/* Output the "newarr" instruction */
	ILGenTypeToken(info, IL_OP_NEWARR, type);
}

void ILGenArrayCtor(ILGenInfo *info, ILType *type)
{
	if(info->asmOutput)
	{
		int dim;
		fputs("\tnewobj\tinstance void ", info->asmOutput);
		ILDumpType(info->asmOutput, info->image, type, IL_DUMP_QUOTE_NAMES);
		fputs("::.ctor(", info->asmOutput);
		type = GetLastNestedArray(type);
		dim = ILTypeGetRank(type);
		while(dim > 0)
		{
			fputs("int32", info->asmOutput);
			--dim;
			if(dim > 0)
			{
				fputs(", ", info->asmOutput);
			}
		}
		fputs(")\n", info->asmOutput);
	}
}

void ILGenArrayGet(ILGenInfo *info, ILType *type)
{
	if(info->asmOutput)
	{
		int dim;
		fputs("\tcall\tinstance ", info->asmOutput);
		ILDumpType(info->asmOutput, info->image, ILTypeGetElemType(type),
				   IL_DUMP_QUOTE_NAMES);
		putc(' ', info->asmOutput);
		ILDumpType(info->asmOutput, info->image, type, IL_DUMP_QUOTE_NAMES);
		fputs("::Get(", info->asmOutput);
		dim = ILTypeGetRank(type);
		while(dim > 0)
		{
			fputs("int32", info->asmOutput);
			--dim;
			if(dim > 0)
			{
				fputs(", ", info->asmOutput);
			}
		}
		fputs(")\n", info->asmOutput);
	}
}

void ILGenArraySet(ILGenInfo *info, ILType *type)
{
	if(info->asmOutput)
	{
		int dim;
		fputs("\tcall\tinstance void ", info->asmOutput);
		ILDumpType(info->asmOutput, info->image, type, IL_DUMP_QUOTE_NAMES);
		fputs("::Set(", info->asmOutput);
		dim = ILTypeGetRank(type);
		while(dim > 0)
		{
			fputs("int32, ", info->asmOutput);
			--dim;
		}
		ILDumpType(info->asmOutput, info->image, ILTypeGetElemType(type),
				   IL_DUMP_QUOTE_NAMES);
		fputs(")\n", info->asmOutput);
	}
}

void ILGenFieldRef(ILGenInfo *info, int opcode, ILField *field)
{
	if(info->asmOutput)
	{
		ILType *type = ILFieldGetTypeWithPrefixes(field);
		ILType *stripped = ILTypeStripPrefixes(type);
		if(type != stripped &&
		   opcode != IL_OP_LDFLDA &&
		   opcode != IL_OP_LDSFLDA)
		{
			/* This field may need a "volatile" instruction to access it */
			ILType *modifier = ILFindNonSystemType
				(info, "IsVolatile", "System.Runtime.CompilerServices");
			if(ILType_IsClass(modifier) &&
			   ILTypeHasModifier(type, ILType_ToClass(modifier)))
			{
				fputs("\tvolatile.\n", info->asmOutput);
			}
		}
		putc('\t', info->asmOutput);
		if(opcode < IL_OP_PREFIX)
		{
			fputs(ILMainOpcodeTable[opcode].name, info->asmOutput);
		}
		else
		{
			fputs(ILPrefixOpcodeTable[opcode - IL_OP_PREFIX].name,
				  info->asmOutput);
		}
		putc('\t', info->asmOutput);
		ILDumpType(info->asmOutput, info->image, stripped,
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

void ILGenFlush(ILGenInfo *info)
{
	/* Peephole optimization not yet implemented */
}

void ILGenModulesAndAssemblies(ILGenInfo *info)
{
	ILModule *module;
	ILAssembly *assem;
	const ILUInt16 *version;

	/* Bail out if no assembly code stream */
	if(!(info->asmOutput))
	{
		return;
	}

	/* Dump module references */
	module = 0;
	while((module = (ILModule *)ILImageNextToken
				(info->image, IL_META_TOKEN_MODULE_REF, module)) != 0)
	{
		fputs(".module extern ", info->asmOutput);
		ILDumpIdentifier(info->asmOutput, ILModule_Name(module), 0,
						 IL_DUMP_QUOTE_NAMES);
		putc('\n', info->asmOutput);
	}

	/* Dump assembly references */
	assem = 0;
	while((assem = (ILAssembly *)ILImageNextToken
				(info->image, IL_META_TOKEN_ASSEMBLY_REF, assem)) != 0)
	{
		fputs(".assembly extern ", info->asmOutput);
		ILDumpFlags(info->asmOutput, ILAssembly_RefAttrs(assem),
					ILAssemblyRefFlags, 0);
		ILDumpIdentifier(info->asmOutput, ILAssembly_Name(assem), 0,
						 IL_DUMP_QUOTE_NAMES);
		fputs("\n{\n", info->asmOutput);
		version = ILAssemblyGetVersion(assem);
		fprintf(info->asmOutput, "\t.ver %lu:%lu:%lu:%lu\n",
				(unsigned long)(version[0]), (unsigned long)(version[1]),
				(unsigned long)(version[2]), (unsigned long)(version[3]));
		fputs("}\n", info->asmOutput);
	}

	/* Dump assembly definitions */
	while((assem = (ILAssembly *)ILImageNextToken
				(info->image, IL_META_TOKEN_ASSEMBLY, assem)) != 0)
	{
		ILInt32 hashAlgorithm;
		const ILUInt16 *assemblyVersion;
		const char *locale;

		fputs(".assembly ", info->asmOutput);
		ILDumpFlags(info->asmOutput, ILAssembly_Attrs(assem),
					ILAssemblyFlags, 0);
		ILDumpIdentifier(info->asmOutput, ILAssembly_Name(assem), 0,
						 IL_DUMP_QUOTE_NAMES);
		fputs("\n{\n", info->asmOutput);
		hashAlgorithm = ILAssemblyGetHashAlgorithm(assem);
		if(hashAlgorithm)
		{
			fprintf(info->asmOutput, "\t.hash algorithm %u\n",
					(unsigned)hashAlgorithm);
		}
		locale = ILAssemblyGetLocale(assem);
		if(locale)
		{
			fprintf(info->asmOutput, "\t.locale \"%s\"\n", locale);
		}
		assemblyVersion = ILAssemblyGetVersion(assem);
		fprintf(info->asmOutput, "\t.ver %u:%u:%u:%u\n",
				(unsigned)(assemblyVersion[0]),
				(unsigned)(assemblyVersion[1]),
				(unsigned)(assemblyVersion[2]),
				(unsigned)(assemblyVersion[3]));
		if(info->hasUnsafe)
		{
			/* Output the "SkipVerification" permissions block */
			fputs("\t.custom instance void [.library]"
			 "System.Security.Permissions.SecurityPermissionAttribute::"
			 ".ctor(valuetype "
			 	"[.library]System.Security.Permissions.SecurityAction) =\n"
			 "\t\t(01 00 08 00 00 00 01 00 54 02 10 53 6B 69 70 56\n"
			 "\t\t 65 72 69 66 69 63 61 74 69 6F 6E 01)\n", info->asmOutput);
		}
		ILGenOutputAttributes(info, info->asmOutput, ILToProgramItem(assem));
		fputs("}\n", info->asmOutput);
	}

	/* Dump module definitions */
	module = 0;
	while((module = (ILModule *)ILImageNextToken
				(info->image, IL_META_TOKEN_MODULE, module)) != 0)
	{
		fputs(".module ", info->asmOutput);
		ILDumpIdentifier(info->asmOutput, ILModule_Name(module), 0,
						 IL_DUMP_QUOTE_NAMES);
		putc('\n', info->asmOutput);
		if(info->hasUnsafe)
		{
			/* Output the "UnverifiableCode" attribute */
			fputs(".custom instance void "
					"[.library]System.Security.UnverifiableCodeAttribute"
					"::.ctor() = (01 00 00 00)\n", info->asmOutput);
		}
		ILGenOutputAttributes(info, info->asmOutput, ILToProgramItem(module));
	}
}

void ILGenSwitchStart(ILGenInfo *info)
{
	if(info->asmOutput)
	{
		fputs("\tswitch (\n", info->asmOutput);
	}
}

void ILGenSwitchRef(ILGenInfo *info, ILLabel *label, int comma)
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

void ILGenSwitchEnd(ILGenInfo *info)
{
	if(info->asmOutput)
	{
		fputs("\t)\n", info->asmOutput);
	}
}

#ifdef	__cplusplus
};
#endif
