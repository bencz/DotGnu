/*
 * c_internal.c - Internal definitions for the C compiler front end.
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

#include <cscc/c/c_internal.h>

#ifdef	__cplusplus
extern	"C" {
#endif

CSemValue CSemValueDefault = {C_SEMKIND_VOID, ILType_Void, 0};
CSemValue CSemValueBool = {C_SEMKIND_RVALUE | C_SEMKIND_BOOLEAN,
						   ILType_Int32, 0};
CSemValue CSemValueError = {C_SEMKIND_ERROR, ILType_Void, 0};
CAddress CAddressDefault = {0, 0};

void *CSemDupExtra(const void *buf, unsigned int len)
{
	void *temp = yynodealloc(len);
	ILMemCpy(temp, buf, len);
	return temp;
}

void CGenCloneLine(ILNode *newNode, ILNode *oldNode)
{
	yysetfilename(newNode, yygetfilename(oldNode));
	yysetlinenum(newNode, yygetlinenum(oldNode));
}

CSemValue CSemInlineAnalysis(ILGenInfo *info, ILNode *node,
							 ILNode **parent, ILScope *scope)
{
	ILScope *currentScope = info->currentScope;
	CSemValue result;
	info->currentScope = scope;
	result = ILNode_CSemAnalysis(node, info, parent, 1);
	info->currentScope = currentScope;
	return result;
}

int CSemIsZero(CSemValue value)
{
	ILEvalValue *evalValue = CSemGetConstant(value);
	if(evalValue)
	{
		switch(evalValue->valueType)
		{
			case ILMachineType_Int8:
			case ILMachineType_UInt8:
			case ILMachineType_Int16:
			case ILMachineType_UInt16:
			case ILMachineType_Char:
			case ILMachineType_Int32:
			case ILMachineType_UInt32:
			case ILMachineType_NativeInt:
			case ILMachineType_NativeUInt:
				return (evalValue->un.i4Value == 0);

			case ILMachineType_Int64:
			case ILMachineType_UInt64:
				return (evalValue->un.i8Value == 0);

			default: break;
		}
	}
	return 0;
}

void CGenBeginCode(ILGenInfo *info)
{
	/* Register the builtin library */
	if(ILContextGetAssembly(info->context, "OpenSystem.C") == 0)
	{
		CGenRegisterLibrary(info);
	}

	/* C doesn't have goto label scopes */
	info->hasGotoScopes = 0;

	/* Tag the module with "ModuleAttribute", which tells the linker
	   that this is a C module requiring special treatment */
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, ".custom instance void "
				"OpenSystem.C.ModuleAttribute::.ctor() = (01 00 00 00)\n");
	}

	/* Initialize the global definition scope */
	CScopeGlobalInit(info);

	/* We don't use CCParseTree, but we need to initialize it
	   to make sure that "common/cc_main.c" has something */
	CCParseTree = ILNode_Empty_create();

	/* Mark the current treecc node pool location */
	yynodepush();

	/* Pre-declare builtin definitions */
	CFunctionPredeclare(info);
}

void CGenEndCode(ILGenInfo *info)
{
	FILE *stream = info->asmOutput;

	/* Output pending class definitions */
	if(stream != 0)
	{
		CTypeOutputPending(info, stream);
	}

	/* Output the string constant pool */
	CGenStringPool(info);

	/* Output the "crt0" code if this module has a "main" function */
	if(stream != 0)
	{
		CGenCrt0(info, stream);
	}
}

void CGenAddress(ILGenInfo *info, ILNode *node)
{
	CAddress addr;
	if(yyisa(node, ILNode_LValue))
	{
		addr = ILNode_CGenAddress((ILNode_LValue *)node, info);
		if(addr.ptrOnStack)
		{
			/* Add the offset to the pointer */
			if(addr.offset)
			{
				ILGenIntNative(info, addr.offset);
				ILGenSimple(info, IL_OP_ADD);
				ILGenExtend(info, 1);
			}
		}
		else
		{
			/* Push the literal offset onto the stack */
			ILGenIntNative(info, addr.offset);
			ILGenAdjust(info, 1);
		}
	}
}

void CGenSizeOf(ILGenInfo *info, ILType *type)
{
	CTypeLayoutInfo layout;
	ILClass *classInfo;

	CTypeGetLayoutInfo(type, &layout);
	if(layout.category == C_TYPECAT_FIXED)
	{
		/* Output the fixed size as a constant */
		ILGenUInt32(info, layout.size);
		ILGenAdjust(info, 1);
	}
	else if(layout.category == C_TYPECAT_DYNAMIC)
	{
		/* We need to get the size dynamically from "layout.measureType" */
		type = layout.measureType;
		if(ILType_IsPrimitive(type) || ILType_IsValueType(type))
		{
			/* Calculate the size of the underlying value type */
			ILGenClassToken(info, IL_OP_PREFIX + IL_PREFIX_OP_SIZEOF,
						    ILTypeToClass(info, type));
		}
		else
		{
			/* Assume that everything else is pointer-sized */
			ILGenClassToken(info, IL_OP_PREFIX + IL_PREFIX_OP_SIZEOF,
						    ILTypeToClass(info, ILType_Int));
		}
		ILGenAdjust(info, 1);
	}
	else if(layout.category == C_TYPECAT_COMPLEX)
	{
		/* Complex types: load the read-only "size.of" field onto the stack */
		classInfo = ILTypeToClass(info, ILTypeStripPrefixes(type));
		if(classInfo && info->asmOutput)
		{
			fputs("\tldsfld\tunsigned int32 ", info->asmOutput);
			ILDumpClassName(info->asmOutput, info->image, classInfo,
							IL_DUMP_QUOTE_NAMES);
			fputs("::'size.of'\n", info->asmOutput);
		}
		ILGenAdjust(info, 1);
	}
}

#ifdef	__cplusplus
};
#endif
