/*
 * cg_gen.c - Helper routines for "ILGenInfo".
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
#include "cg_scope.h"
#include "cg_resolve.h"
#include "cg_nodemap.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILImage *ILGenCreateBasicImage(ILContext *context, const char *assemName)
{
	ILImage *image;

	/* Create the image */
	if((image = ILImageCreate(context)) == 0)
	{
		return 0;
	}

	/* Create the module definition */
	if(!ILModuleCreate(image, 0, "<Module>", 0))
	{
		ILImageDestroy(image);
		return 0;
	}

	/* Create the assembly definition */
	if(!ILAssemblyCreate(image, 0, assemName, 0))
	{
		ILImageDestroy(image);
		return 0;
	}

	/* Create the module type */
	if(!ILClassCreate(ILClassGlobalScope(image), 0, "<Module>", 0, 0))
	{
		ILImageDestroy(image);
		return 0;
	}

	/* Done */
	return image;
}

void ILGenInfoInit(ILGenInfo *info, char *progname,
				   const char *assemName,
				   FILE *asmOutput, int useBuiltinLibrary)
{
	info->progname = progname;
	info->asmOutput = asmOutput;
	if((info->context = ILContextCreate()) == 0)
	{
		ILGenOutOfMemory(info);
	}
	if(!assemName)
	{
		assemName = "<Assembly>";
	}
	if((info->image = ILGenCreateBasicImage(info->context, assemName)) == 0)
	{
		ILGenOutOfMemory(info);
	}
	if(useBuiltinLibrary)
	{
		if((info->libImage =
				ILGenCreateBasicImage(info->context, ".library")) == 0)
		{
			ILGenOutOfMemory(info);
		}
	}
	else
	{
		info->libImage = 0;
	}
	ILMemPoolInitType(&(info->nodePool), ILNode, 0);
	ILScopeInit(info);
	info->nextLabel = 1;
	info->overflowInsns = -1;
	info->overflowGlobal = -1;
	info->overflowChanged = 0;
	info->pedanticArith = 0;
	info->clsCompliant = 0;
	info->semAnalysis = 0;
	info->typeGather = 0;
	info->inSemType = 0;
	info->inSemStatic = 0;
	info->inAttrArg = 0;
	info->useJavaLib = 0;
	info->outputIsJava = 0;
	info->debugFlag = 0;
	info->hasUnsafe = 0;
	info->needSwitchPop = 0;
	info->hasGotoScopes = -1;
	info->resolvingAlias = 0;
	info->inFixed = 0;
	info->decimalRoundMode = IL_DECIMAL_ROUND_HALF_EVEN;
	info->stackHeight = 0;
	info->maxStackHeight = 0;
	info->loopStack = 0;
	info->loopStackSize = 0;
	info->loopStackMax = 0;
	info->returnType = ILType_Void;
	info->returnVar = -1;
	info->returnLabel = ILLabel_Undefined;
	info->throwVariable = -1;
	info->gotoList = 0;
	info->scopeLevel = 0;
	info->tempVars = 0;
	info->numTempVars = 0;
	info->maxTempVars = 0;
	info->tempLocalBase = 0;
	info->currentScope = 0;
	info->globalScope = 0;
	info->javaInfo = 0;
	info->unsafeLevel = 0;
	info->contextStack = 0;
	info->contextStackSize = 0;
	info->contextStackMax = 0;
	info->currentClass = 0;
	info->currentMethod = 0;
	info->currentNamespace = 0;
	info->arrayInit = 0;
	info->itemHash = 0;
#if IL_VERSION_MAJOR > 1
	info->currentTypeFormals = 0;
	info->currentMethodFormals = 0;
#endif	/* IL_VERSION_MAJOR > 1 */
	info->gotoPtrLabel = ILLabel_Undefined;
	info->accessCheck = ILClassAccessible;
	info->errFunc = 0;
	info->warnFunc = 0;
	if(useBuiltinLibrary)
	{
		ILGenMakeLibrary(info);
	}
	ILProgramItemHashCreate(info);
}

void ILGenInfoToJava(ILGenInfo *info)
{
	info->outputIsJava = -1;
	JavaGenInit(info);
}

void ILGenInfoDestroy(ILGenInfo *info)
{
	ILGotoEntry *gotoEntry, *nextGoto;

	/* Destroy the memory pools */
	ILMemPoolDestroy(&(info->nodePool));
	ILMemPoolDestroy(&(info->scopePool));
	ILMemPoolDestroy(&(info->scopeDataPool));

	/* Destroy the loop stack */
	if(info->loopStack)
	{
		ILFree(info->loopStack);
	}

	/* Destroy the context stack */
	if(info->contextStack)
	{
		ILFree(info->contextStack);
	}

	/* Destroy the goto list */
	gotoEntry = info->gotoList;
	while(gotoEntry != 0)
	{
		nextGoto = gotoEntry->next;
		ILFree(gotoEntry);
		gotoEntry = nextGoto;
	}

	/* Free the temporary variable array */
	if(info->tempVars)
	{
		ILFree(info->tempVars);
	}

	/* Destroy Java-specific information */
	JavaGenDestroy(info);

	/* Destroy the program item hash */
	ILHashDestroy(info->itemHash);

	/* Destroy the image and context */
	ILImageDestroy(info->image);
	ILContextDestroy(info->context);
}

void ILGenOutOfMemory(ILGenInfo *info)
{
	if(info->progname)
	{
		fputs(info->progname, stderr);
		fputs(": ", stderr);
	}
	fputs("virtual memory exhausted\n", stderr);
	exit(1);
}

ILType *ILFindSystemType(ILGenInfo *info, const char *name)
{
	return ILFindNonSystemType(info, name, "System");
}

ILType *ILFindNonSystemType(ILGenInfo *info, const char *name,
							const char *namespace)
{
	ILClass *classInfo;
	ILProgramItem *scope;

	/* Look in the program itself first */
	scope = ILClassGlobalScope(info->image);
	if(scope)
	{
		classInfo = ILClassLookup(scope, name, namespace);
		if(classInfo)
		{
			return ILClassToTypeDirect(classInfo);
		}
	}

	/* Look in any image that is linked against the program */
	classInfo = ILClassLookupGlobal(info->context, name, namespace);
	if(classInfo)
	{
		classInfo = ILClassImport(info->image, classInfo);
		if(!classInfo)
		{
			ILGenOutOfMemory(info);
		}
		return ILClassToTypeDirect(classInfo);
	}

	/* Look in the library image */
	if(!(info->libImage))
	{
		return 0;
	}
	scope = ILClassGlobalScope(info->libImage);
	if(scope)
	{
		classInfo = ILClassLookup(scope, name, namespace);
		if(classInfo)
		{
			classInfo = ILClassImport(info->image, classInfo);
			if(!classInfo)
			{
				ILGenOutOfMemory(info);
			}
		}
		if(classInfo)
		{
			return ILClassToTypeDirect(classInfo);
		}
		else
		{
			return 0;
		}
	}
	else
	{
		return 0;
	}
}

/*
 * System type resolver that is used by "ILTypeToClass".
 */
static ILClass *TypeResolver(ILImage *image, void *data,
							 const char *name,
							 const char *namespace)
{
	return ILType_ToClass(ILFindNonSystemType
				((ILGenInfo *)data, name, namespace));
}

ILClass *ILTypeToClass(ILGenInfo *info, ILType *type)
{
	if(ILType_IsComplex(type))
	{
		ILProgramItem *item;

		item = ILProgramItemFromType(info->image, type);
		if(item)
		{
			return ILProgramItemToClass(item);
		}
		return 0;
	}
	else
	{
		return ILClassFromType(info->image, info, type, TypeResolver);
	}
}

ILProgramItem *ILTypeToProgramItem(ILGenInfo *info, ILType *type)
{
	if(ILType_IsComplex(type))
	{
		return ILProgramItemFromType(info->image, type);
	}
	else
	{
		ILClass *classInfo;

		classInfo = ILClassFromType(info->image, info, type, TypeResolver);
		return ILToProgramItem(classInfo);
	}
}

ILMachineType ILTypeToMachineType(ILType *type)
{
	ILClass *classInfo;
	const char *namespace;

	/* Convert enumerated types into their underlying type */
	type = ILTypeGetEnumType(ILTypeStripPrefixes(type));

	if(ILType_IsPrimitive(type))
	{
		/* Convert a primitive type into a machine type */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_BOOLEAN:	return ILMachineType_Boolean;
			case IL_META_ELEMTYPE_I1:		return ILMachineType_Int8;
			case IL_META_ELEMTYPE_U1:		return ILMachineType_UInt8;
			case IL_META_ELEMTYPE_I2:		return ILMachineType_Int16;
			case IL_META_ELEMTYPE_U2:		return ILMachineType_UInt16;
			case IL_META_ELEMTYPE_CHAR:		return ILMachineType_Char;
			case IL_META_ELEMTYPE_I4:		return ILMachineType_Int32;
			case IL_META_ELEMTYPE_U4:		return ILMachineType_UInt32;
			case IL_META_ELEMTYPE_I8:		return ILMachineType_Int64;
			case IL_META_ELEMTYPE_U8:		return ILMachineType_UInt64;
			case IL_META_ELEMTYPE_I:		return ILMachineType_NativeInt;
			case IL_META_ELEMTYPE_U:		return ILMachineType_NativeUInt;
			case IL_META_ELEMTYPE_R4:		return ILMachineType_Float32;
			case IL_META_ELEMTYPE_R8:		return ILMachineType_Float64;
			case IL_META_ELEMTYPE_R:		return ILMachineType_NativeFloat;
			case IL_META_ELEMTYPE_TYPEDBYREF:
					return ILMachineType_ManagedValue;
			case IL_META_ELEMTYPE_STRING:
			case IL_META_ELEMTYPE_OBJECT:	return ILMachineType_ObjectRef;
			default: break;
		}
		return ILMachineType_Void;
	}
	else if(ILType_IsValueType(type))
	{
		/* Check for "System.Decimal", which has a special machine type */
		classInfo = ILType_ToClass(type);
		namespace = ILClass_Namespace(classInfo);
		if(namespace && !strcmp(namespace, "System") &&
		   !strcmp(ILClass_Name(classInfo), "Decimal") &&
		   ILClass_NestedParent(classInfo) == 0)
		{
			return ILMachineType_Decimal;
		}

		/* Everything else is a managed value */
		return ILMachineType_ManagedValue;
	}
	else if(ILType_IsClass(type))
	{
		/* Check for "System.String", which has a special machine type */
		classInfo = ILType_ToClass(type);
		namespace = ILClass_Namespace(classInfo);
		if(namespace && !strcmp(namespace, "System") &&
		   !strcmp(ILClass_Name(classInfo), "String") &&
		   ILClass_NestedParent(classInfo) == 0)
		{
			return ILMachineType_String;
		}

		/* Everything else is an object reference */
		return ILMachineType_ObjectRef;
	}
	else if(ILType_IsArray(type))
	{
		/* Array types are always object references */
		return ILMachineType_ObjectRef;
	}
	else if(type == ILType_Invalid)
	{
		/* Invalid types are treated as "void" */
		return ILMachineType_Void;
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		/* Check for pointer types */
		if(ILType_Kind(type) == IL_TYPE_COMPLEX_PTR ||
		   (ILType_Kind(type) & IL_TYPE_COMPLEX_METHOD) != 0)
		{
			return ILMachineType_UnmanagedPtr;
		}
	}

	/* Everything else is treated as a managed pointer for now */
	return ILMachineType_ManagedPtr;
}

ILType *ILValueTypeToType(ILGenInfo *info, ILMachineType valueType)
{
	switch(valueType)
	{
		case ILMachineType_Boolean:		return ILType_Boolean;
		case ILMachineType_Int8:		return ILType_Int8;
		case ILMachineType_UInt8:		return ILType_UInt8;
		case ILMachineType_Int16:		return ILType_Int16;
		case ILMachineType_UInt16:		return ILType_UInt16;
		case ILMachineType_Char:		return ILType_Char;
		case ILMachineType_Int32:		return ILType_Int32;
		case ILMachineType_UInt32:		return ILType_UInt32;
		case ILMachineType_NativeInt:	return ILType_Int;
		case ILMachineType_NativeUInt:	return ILType_UInt;
		case ILMachineType_Int64:		return ILType_Int64;
		case ILMachineType_UInt64:		return ILType_UInt64;
		case ILMachineType_Float32:		return ILType_Float32;
		case ILMachineType_Float64:		return ILType_Float64;
		case ILMachineType_NativeFloat:	return ILType_Float;
		case ILMachineType_String:		return ILFindSystemType(info, "String");
		case ILMachineType_Decimal:		return ILFindSystemType(info, "Decimal");
		case ILMachineType_ObjectRef:	return ILFindSystemType(info, "Object");
		default:						break;
	}
	return ILType_Invalid;
}

unsigned ILGenTempVar(ILGenInfo *info, ILMachineType type)
{
	return ILGenTempTypedVar(info, ILValueTypeToType(info, type));
}

unsigned ILGenTempTypedVar(ILGenInfo *info, ILType *type)
{
	unsigned varNum;

	/* See if we can re-use a free temporary variable */
	for(varNum = 0; varNum < info->numTempVars; ++varNum)
	{
		if(!(info->tempVars[varNum].allocated) &&
		   ILTypeIdentical(info->tempVars[varNum].type, type))
		{
			info->tempVars[varNum].allocated = 1;
			return varNum + info->tempLocalBase;
		}
	}

	/* Abort if too many temporary variables */
	if((info->numTempVars + info->tempLocalBase) == (unsigned)0xFFFF)
	{
		fprintf(stderr, "%s: too many local variables - aborting\n",
				info->progname);
		exit(1);
	}

	/* Add a new temporary variable to the current method */
	if(info->numTempVars >= info->maxTempVars)
	{
		ILLocalVar *newvars;
		newvars = (ILLocalVar *)ILRealloc(info->tempVars,
										  sizeof(ILLocalVar) *
										  		(info->maxTempVars + 16));
		if(!newvars)
		{
			ILGenOutOfMemory(info);
		}
		info->tempVars = newvars;
		info->maxTempVars += 16;
	}
	info->tempVars[info->numTempVars].name = 0;
	info->tempVars[info->numTempVars].scopeLevel = -1;
	info->tempVars[info->numTempVars].type = type;
	info->tempVars[info->numTempVars].allocated = 1;

	/* Generate assembly code to allocate the local */
	if(info->outputIsJava)
	{
		JavaGenAddFrameSlot(info, ILTypeToMachineType(type));
	}
	else
	{
		ILGenAllocLocal(info, type, (const char *)0);
	}

	/* Return the new variable index to the caller */
	return (info->tempLocalBase + (info->numTempVars)++);
}

void ILGenReleaseTempVar(ILGenInfo *info, unsigned localNum)
{
	if(localNum >= info->tempLocalBase &&
	   localNum < (info->tempLocalBase + info->numTempVars) &&
	   info->tempVars[localNum - info->tempLocalBase].scopeLevel == -1)
	{
		info->tempVars[localNum - info->tempLocalBase].allocated = 0;
	}
}

int ILGenItemHasAttribute(ILProgramItem *item, const char *name)
{
	ILAttribute *attr = 0;
	ILMethod *method;
	ILClass *classInfo;
	const char *namespace;
	while((attr = ILProgramItemNextAttribute(item, attr)) != 0)
	{
		method = ILProgramItemToMethod(ILAttribute_TypeAsItem(attr));
		if(method && !strcmp(ILMethod_Name(method), ".ctor"))
		{
			classInfo = ILMethod_Owner(method);
			if(!strcmp(ILClass_Name(classInfo), name))
			{
				namespace = ILClass_Namespace(classInfo);
				if(namespace && !strcmp(namespace, "System"))
				{
					return 1;
				}
			}
		}
	}
	return 0;
}

void ILGenItemAddAttribute(ILGenInfo *info, ILProgramItem *item,
						   const char *name)
{
	ILType *typeInfo;
	ILClass *classInfo;
	ILClass *scopeInfo;
	ILMethod *ctor;
	ILAttribute *attr;
	ILType *signature;
	static unsigned char const blob[4] = {1, 0, 0, 0};

	/* Find the attribute class */
	typeInfo = ILFindSystemType(info, name);
	if(!typeInfo)
	{
		return;
	}
	classInfo = ILType_ToClass(typeInfo);

	/* Use the "<Module>" type of the program as the lookup scope */
	scopeInfo = ILClassLookup(ILClassGlobalScope(info->image),
							  "<Module>", 0);
	if(!scopeInfo)
	{
		return;
	}

	/* Find the zero-argument constructor for the class */
	ctor = ILResolveConstructor(info, classInfo, scopeInfo, 0, 0);
	if(!ctor)
	{
		/* We are probably compiling "mscorlib.dll", and so we need to
		   create a method reference until we compile the class later */
		ctor = ILMethodCreate(classInfo, IL_MAX_UINT32, ".ctor",
							  IL_META_METHODDEF_PUBLIC |
							  IL_META_METHODDEF_HIDE_BY_SIG |
							  IL_META_METHODDEF_SPECIAL_NAME |
							  IL_META_METHODDEF_RT_SPECIAL_NAME);
		if(!ctor)
		{
			ILGenOutOfMemory(info);
		}
		signature = ILTypeCreateMethod(info->context, ILType_Void);
		if(!signature)
		{
			ILGenOutOfMemory(info);
		}
		ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
		ILMethodSetCallConv(ctor, IL_META_CALLCONV_HASTHIS);
		ILMemberSetSignature((ILMember *)ctor, signature);
	}
	ctor = (ILMethod *)ILMemberImport(info->image, (ILMember *)ctor);
	if(!ctor)
	{
		ILGenOutOfMemory(info);
	}

	/* Create an attribute object */
	attr = ILAttributeCreate(info->image, 0);
	if(!attr)
	{
		ILGenOutOfMemory(info);
	}
	ILAttributeSetType(attr, (ILProgramItem *)ctor);
	if(!ILAttributeSetValue(attr, blob, 4))
	{
		ILGenOutOfMemory(info);
	}

	/* Attach the attribute to the program item */
	ILProgramItemAddAttribute(item, attr);
}

int ILGenNumUsableParams(ILType *signature)
{
	ILUInt32 num;

	/* Get the basic number of parameters */
	num = (ILUInt32)(ILTypeNumParams(signature));

	/* If the signature is "vararg", and we don't have an explicit
	   sentinel, then add one extra for the "arglist" parameter */
	if((ILType_CallConv(signature) & IL_META_CALLCONV_MASK) ==
			IL_META_CALLCONV_VARARG)
	{
		if((ILType_Kind(signature) & IL_TYPE_COMPLEX_METHOD_SENTINEL) == 0)
		{
			++num;
		}
	}

	/* Return the number of parameters to the caller */
	return num;
}

ILParameterModifier ILGenGetParamInfo(ILMethod *method, ILType *signature,
									  ILUInt32 num, ILType **type)
{
	ILParameter *param;
	int isByRef;

	/* Get the signature if the input is NULL */
	if(!signature)
	{
		signature = ILMethod_Signature(method);
	}

	/* Get the parameter's type */
	*type = ILTypeGetParam(signature, num);

	/* Check for the vararg "arglist" parameter */
	if((ILType_CallConv(signature) & IL_META_CALLCONV_MASK) ==
			IL_META_CALLCONV_VARARG)
	{
		if(ILType_IsSentinel(*type) || num > ILTypeNumParams(signature))
		{
			*type = ILType_Null;
			return ILParamMod_arglist;
		}
	}

	/* Find the parameter information block for the method */
	if(method)
	{
		method = (ILMethod *)ILMemberResolve((ILMember *)method);
		param = 0;
		while((param = ILMethodNextParam(method, param)) != 0)
		{
			if(ILParameter_Num(param) == num)
			{
				/* Check for a "params" parameter by looking for the
				   "System.ParamArrayAttribute" attribute on the
				   last parameter supplied to the method */
				if(num == ILTypeNumParams(signature))
				{
					if(ILGenItemHasAttribute((ILProgramItem *)param,
											 "ParamArrayAttribute"))
					{
						/* Make sure that this parameter has a
						   single-dimensional array type */
						if(*type != 0 &&
						   ILType_IsComplex(*type) &&
						   ILType_Kind(*type) == IL_TYPE_COMPLEX_ARRAY)
						{
							*type = ILType_ElemType(*type);
							return ILParamMod_params;
						}
					}
				}
	
				/* Determine if the parameter is "byref" */
				if(*type != 0 &&
				   ILType_IsComplex(*type) &&
				   ILType_Kind(*type) == IL_TYPE_COMPLEX_BYREF)
				{
					*type = ILType_Ref(*type);
					isByRef = 1;
				}
				else
				{
					isByRef = 0;
				}
	
				/* Check for "out" parameters */
				if(ILParameter_IsOut(param) && isByRef)
				{
					return ILParamMod_out;
				}
	
				/* Check for "ref" parameters */
				if(isByRef)
				{
					return ILParamMod_ref;
				}
	
				/* This is a normal parameter */
				return ILParamMod_empty;
			}
		}
	}

	/* Check for by-ref parameters.  This can happen if there is
	   no ILParameter record for the parameter, but it is BYREF. */
	if(*type != 0 && ILType_IsComplex(*type) &&
	   ILType_Kind(*type) == IL_TYPE_COMPLEX_BYREF)
	{
		*type = ILType_Ref(*type);
		return ILParamMod_ref;
	}

	/* If we get here, the parameter is normal */
	return ILParamMod_empty;
}

#ifdef	__cplusplus
};
#endif
