/*
 * null_coder.c - Null coder that is used by standalone verifiers.
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

#include <stdio.h>
#include "engine.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Stubs for coder functions.
 */
static ILCoder *Coder_Create(ILExecProcess *process, ILUInt32 size,
							 unsigned long cachePageSize)
{
	return 0;
}
static void Coder_EnableDebug(ILCoder *coder)
{
}
static void *Coder_Alloc(ILCoder *coder, ILUInt32 size)
{
	return 0;
}
static unsigned long Coder_GetCacheSize(ILCoder *coder)
{
	return 0;
}
static int Coder_Setup(ILCoder *coder, unsigned char **start,
					   ILMethod *method, ILMethodCode *code,
					   ILCoderExceptions *coderExceptions,
					   int hasRethrow)
{
	return 1;
}
static int Coder_SetupExtern(ILCoder *coder, unsigned char **start,
							 ILMethod *method, void *fn, void *cif,
							 int isInternal)
{
	return 1;
}
static int Coder_SetupExternCtor(ILCoder *coder, unsigned char **start,
							     ILMethod *method, void *fn, void *cif,
							     void *ctorfn, void *ctorcif, int isInternal)
{
	return 1;
}
static int Coder_CtorOffset(ILCoder *coder)
{
	return 0;
}
static void Coder_Destroy(ILCoder *coder)
{
}
static int Coder_Finish(ILCoder *coder)
{
	return IL_CODER_END_OK;
}
static void Coder_Label(ILCoder *coder, ILUInt32 offset)
{
}
static void Coder_StackRefresh(ILCoder *coder, ILEngineStackItem *stack,
							   ILUInt32 stackSize)
{
}
static void Coder_Constant(ILCoder *coder, int opcode, unsigned char *arg)
{
}
static void Coder_StringConstant(ILCoder *coder, ILToken token, void *object)
{
}
static void Coder_Binary(ILCoder *coder, int opcode, ILEngineType type1,
				   	     ILEngineType type2)
{
}
static void Coder_BinaryPtr(ILCoder *coder, int opcode, ILEngineType type1,
				      		ILEngineType type2)
{
}
static void Coder_Shift(ILCoder *coder, int opcode, ILEngineType type1,
				  		ILEngineType type2)
{
}
static void Coder_Unary(ILCoder *coder, int opcode, ILEngineType type)
{
}
static void Coder_LoadArg(ILCoder *coder, ILUInt32 num, ILType *type)
{
}
static void Coder_StoreArg(ILCoder *coder, ILUInt32 num,
					 	   ILEngineType engineType, ILType *type)
{
}
static void Coder_AddrOfArg(ILCoder *coder, ILUInt32 num)
{
}
static void Coder_LoadLocal(ILCoder *coder, ILUInt32 num, ILType *type)
{
}
static void Coder_StoreLocal(ILCoder *coder, ILUInt32 num,
							 ILEngineType engineType, ILType *type)
{
}
static void Coder_AddrOfLocal(ILCoder *coder, ILUInt32 num)
{
}
static void Coder_Dup(ILCoder *coder, ILEngineType valueType, ILType *type)
{
}
static void Coder_Pop(ILCoder *coder, ILEngineType valueType, ILType *type)
{
}
static void Coder_ArrayAccess(ILCoder *coder, int opcode,
							  ILEngineType indexType, ILType *elemType,
							  const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_PtrAccess(ILCoder *coder, int opcode,
							const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_PtrDeref(ILCoder *coder, int pos)
{
}
static void Coder_PtrAccessManaged(ILCoder *coder, int opcode,
								   ILClass *classInfo,
								   const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_Branch(ILCoder *coder, int opcode, ILUInt32 dest,
						 ILEngineType type1, ILEngineType type2)
{
}
static void Coder_SwitchStart(ILCoder *coder, ILUInt32 numEntries)
{
}
static void Coder_SwitchEntry(ILCoder *coder, ILUInt32 dest)
{
}
static void Coder_SwitchEnd(ILCoder *coder)
{
}
static void Coder_Compare(ILCoder *coder, int opcode,
						  ILEngineType type1, ILEngineType type2,
						  int invertTest)
{
}
static void Coder_Conv(ILCoder *coder, int opcode, ILEngineType type)
{
}
static void Coder_ToPointer(ILCoder *coder, ILEngineType type1,
							ILEngineStackItem *type2)
{
}
static void Coder_ArrayLength(ILCoder *coder)
{
}
static void Coder_NewArray(ILCoder *coder, ILType *arrayType,
						   ILClass *arrayClass, ILEngineType lengthType)
{
}
static void Coder_LocalAlloc(ILCoder *coder, ILEngineType sizeType)
{
}
static void Coder_CastClass(ILCoder *coder, ILClass *classInfo,
							int throwException,
							const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_LoadField(ILCoder *coder, ILEngineType ptrType,
							ILType *objectType, ILField *field,
							ILType *fieldType,
							const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_LoadThisField(ILCoder *coder, ILField *field,
								ILType *fieldType,
								const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_LoadStaticField(ILCoder *coder, ILField *field,
								  ILType *fieldType,
								  const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_LoadFieldAddr(ILCoder *coder, ILEngineType ptrType,
							    ILType *objectType, ILField *field,
							    ILType *fieldType)
{
}
static void Coder_LoadStaticFieldAddr(ILCoder *coder, ILField *field,
							          ILType *fieldType)
{
}
static void Coder_StoreField(ILCoder *coder, ILEngineType ptrType,
							 ILType *objectType, ILField *field,
							 ILType *fieldType, ILEngineType valueType,
							 const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_StoreStaticField(ILCoder *coder, ILField *field,
							       ILType *fieldType, ILEngineType valueType,
								   const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_CopyObject(ILCoder *coder, ILEngineType destPtrType,
							 ILEngineType srcPtrType, ILClass *classInfo)
{
}
static void Coder_CopyBlock(ILCoder *coder, ILEngineType destPtrType,
							ILEngineType srcPtrType,
							const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_InitObject(ILCoder *coder, ILEngineType ptrType,
							 ILClass *classInfo)
{
}
static void Coder_InitBlock(ILCoder *coder, ILEngineType ptrType,
							const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_Box(ILCoder *coder, ILClass *boxClass,
					  ILEngineType valueType, ILUInt32 size)
{
}
static void Coder_BoxPtr(ILCoder *coder, ILClass *boxClass,
					     ILUInt32 size, ILUInt32 pos)
{
}
static void Coder_BoxSmaller(ILCoder *coder, ILClass *boxClass,
					   		 ILEngineType valueType, ILType *smallerType)
{
}
static void Coder_Unbox(ILCoder *coder, ILClass *boxClass,
						const ILCoderPrefixInfo *prefixInfo)
{
}
static void Coder_MakeTypedRef(ILCoder *coder, ILClass *classInfo)
{
}
static void Coder_RefAnyVal(ILCoder *coder, ILClass *classInfo)
{
}
static void Coder_RefAnyType(ILCoder *coder)
{
}
static void Coder_PushToken(ILCoder *coder, ILProgramItem *item)
{
}
static void Coder_SizeOf(ILCoder *coder, ILType *type)
{
}
static void Coder_ArgList(ILCoder *coder)
{
}
static void Coder_UpConvertArg(ILCoder *coder, ILEngineStackItem *args,
						       ILUInt32 numArgs, ILUInt32 param,
						       ILType *paramType)
{
}
static void Coder_DownConvertArg(ILCoder *coder, ILEngineStackItem *args,
						         ILUInt32 numArgs, ILUInt32 param,
						         ILType *paramType)
{
}
static void Coder_PackVarArgs(ILCoder *coder, ILType *callSiteSig,
					          ILUInt32 firstParam, ILEngineStackItem *args,
						      ILUInt32 numArgs)
{
}
static void Coder_ValueCtorArgs(ILCoder *coder, ILClass *classInfo,
								ILEngineStackItem *args, ILUInt32 numArgs)
{
}
static void Coder_CheckCallNull(ILCoder *coder, ILCoderMethodInfo *info)
{
}
static void Coder_CallMethod(ILCoder *coder, ILCoderMethodInfo *info,
					   		 ILEngineStackItem *returnItem,
							 ILMethod *methodInfo)
{
}
static void Coder_CallIndirect(ILCoder *coder, ILCoderMethodInfo *info,
							   ILEngineStackItem *returnItem)
{
}
static void Coder_CallCtor(ILCoder *coder, ILCoderMethodInfo *info,
					   	   ILMethod *methodInfo)
{
}
static void Coder_CallVirtual(ILCoder *coder, ILCoderMethodInfo *info,
					    	  ILEngineStackItem *returnItem,
							  ILMethod *methodInfo)
{
}
static void Coder_CallInterface(ILCoder *coder, ILCoderMethodInfo *info,
					      		ILEngineStackItem *returnItem,
								ILMethod *methodInfo)
{
}
static int Coder_CallInlineable(ILCoder *coder, int inlineType,
								ILMethod *methodInfo, ILInt32 elementSize)
{
	return 0;
}
static void Coder_JumpMethod(ILCoder *coder, ILMethod *methodInfo)
{
}
static void Coder_ReturnInsn(ILCoder *coder, ILEngineType engineType,
							 ILType *returnType)
{
}
static void Coder_LoadFuncAddr(ILCoder *coder, ILMethod *methodInfo)
{
}
static void Coder_LoadVirtualAddr(ILCoder *coder, ILMethod *methodInfo)
{
}
static void Coder_LoadInterfaceAddr(ILCoder *coder, ILMethod *methodInfo)
{
}
static void Coder_Throw(ILCoder *coder, int inCurrentMethod)
{
}
static void Coder_SetStackTrace(ILCoder *coder)
{
}
static void Coder_Rethrow(ILCoder *coder, ILCoderExceptionBlock *exception)
{
}
static void Coder_CallFinally(ILCoder *coder, ILCoderExceptionBlock *exception,
							  ILUInt32 dest)
{
}
static void Coder_RetFromFinally(ILCoder *coder)
{
}
static void Coder_LeaveCatch(ILCoder *coder, ILCoderExceptionBlock *exception)
{
}
static void Coder_RetFromFilter(ILCoder *coder)
{
}
static void Coder_OutputExceptionTable(ILCoder *coder,
									   ILCoderExceptions *exceptions)
{
}
static void *Coder_PCToHandler(ILCoder *coder, void *pc, int beyond)
{
	return 0;
}
static ILMethod *Coder_PCToMethod(ILCoder *coder, void *pc, int beyond)
{
	return 0;
}
static ILUInt32 Coder_GetILOffset(ILCoder *coder, void *start,
								  ILUInt32 offset, int exact)
{
	return IL_MAX_UINT32;
}
static ILUInt32 Coder_GetNativeOffset(ILCoder *coder, void *start,
								      ILUInt32 offset, int exact)
{
	return IL_MAX_UINT32;
}
static void Coder_MarkBytecode(ILCoder *coder, ILUInt32 offset)
{
}
static void Coder_MarkEnd(ILCoder *coder)
{
}
static void Coder_SetFlags(ILCoder *coder, int flags)
{
}
static int Coder_GetFlags(ILCoder *coder)
{
	return 0;
}
static ILUInt32 Coder_AllocExtraLocal(ILCoder *coder, ILType *type)
{
	return 0;
}
static void Coder_PushThread(ILCoder *coder, int useRawCalls)
{
}
static void Coder_LoadNativeArgAddr(ILCoder *coder, ILUInt32 num)
{
}
static void Coder_LoadNativeLocalAddr(ILCoder *coder, ILUInt32 num)
{
}
static void Coder_StartFfiArgs(ILCoder *coder)
{
}
static void Coder_PushRawArgPointer(ILCoder *coder)
{
}
static void Coder_CallFfi(ILCoder *coder, void *fn, void *cif,
				  		  int useRawCalls, int hasReturn)
{
}
static void Coder_CheckNull(ILCoder *coder)
{
}
static void Coder_Convert(ILCoder *coder, int opcode)
{
}
static void Coder_ConvertCustom(ILCoder *coder, int opcode,
						    	ILUInt32 customNameLen,
								ILUInt32 customCookieLen,
						    	void *customName, void *customCookie)
{
}
static ILInt32 Coder_RunCCtors(ILCoder *coder, void *userData)
{
	return 1;
}
static ILInt32 Coder_RunCCtor(ILCoder *coder, ILClass *classInfo)
{
	return 1;
}
static void *Coder_HandleLockedMethod(ILCoder *coder, ILMethod *method)
{
	return 0;
}
static void Coder_ProfileStart(ILCoder *coder)
{
}
static void Coder_ProfileEnd(ILCoder *coder)
{
}
static void	Coder_SetOptimizationLevel(ILCoder *coder,
									   ILUInt32 optimizationLevel)
{
}
static ILUInt32 Coder_GetOptimizationLevel(ILCoder *coder)
{
	return 0;
}
/*
 * Null coder class and instance.
 */
ILCoderClass const _ILNullCoderClass = {
	Coder_Create,
	Coder_EnableDebug,
	Coder_Alloc,
	Coder_GetCacheSize,
	Coder_Setup,
	Coder_SetupExtern,
	Coder_SetupExternCtor,
	Coder_CtorOffset,
	Coder_Destroy,
	Coder_Finish,
	Coder_Label,
	Coder_StackRefresh,
	Coder_Constant,
	Coder_StringConstant,
	Coder_Binary,
	Coder_BinaryPtr,
	Coder_Shift,
	Coder_Unary,
	Coder_LoadArg,
	Coder_StoreArg,
	Coder_AddrOfArg,
	Coder_LoadLocal,
	Coder_StoreLocal,
	Coder_AddrOfLocal,
	Coder_Dup,
	Coder_Pop,
	Coder_ArrayAccess,
	Coder_PtrAccess,
	Coder_PtrDeref,
	Coder_PtrAccessManaged,
	Coder_Branch,
	Coder_SwitchStart,
	Coder_SwitchEntry,
	Coder_SwitchEnd,
	Coder_Compare,
	Coder_Conv,
	Coder_ToPointer,
	Coder_ArrayLength,
	Coder_NewArray,
	Coder_LocalAlloc,
	Coder_CastClass,
	Coder_LoadField,
	Coder_LoadStaticField,
	Coder_LoadThisField,
	Coder_LoadFieldAddr,
	Coder_LoadStaticFieldAddr,
	Coder_StoreField,
	Coder_StoreStaticField,
	Coder_CopyObject,
	Coder_CopyBlock,
	Coder_InitObject,
	Coder_InitBlock,
	Coder_Box,
	Coder_BoxPtr,
	Coder_BoxSmaller,
	Coder_Unbox,
	Coder_MakeTypedRef,
	Coder_RefAnyVal,
	Coder_RefAnyType,
	Coder_PushToken,
	Coder_SizeOf,
	Coder_ArgList,
	Coder_UpConvertArg,
	Coder_DownConvertArg,
	Coder_PackVarArgs,
	Coder_ValueCtorArgs,
	Coder_CheckCallNull,
	Coder_CallMethod,
	Coder_CallIndirect,
	Coder_CallCtor,
	Coder_CallVirtual,
	Coder_CallInterface,
	Coder_CallInlineable,
	Coder_JumpMethod,
	Coder_ReturnInsn,
	Coder_LoadFuncAddr,
	Coder_LoadVirtualAddr,
	Coder_LoadInterfaceAddr,
	Coder_Throw,
	Coder_SetStackTrace,
	Coder_Rethrow,
	Coder_CallFinally,
	Coder_RetFromFinally,
	Coder_LeaveCatch,
	Coder_RetFromFilter,
	Coder_OutputExceptionTable,
	Coder_PCToHandler,
	Coder_PCToMethod,
	Coder_GetILOffset,
	Coder_GetNativeOffset,
	Coder_MarkBytecode,
	Coder_MarkEnd,
	Coder_SetFlags,
	Coder_GetFlags,
	Coder_AllocExtraLocal,
	Coder_PushThread,
	Coder_LoadNativeArgAddr,
	Coder_LoadNativeLocalAddr,
	Coder_StartFfiArgs,
	Coder_PushRawArgPointer,
	Coder_CallFfi,
	Coder_CheckNull,
	Coder_Convert,
	Coder_ConvertCustom,
	Coder_RunCCtors,
	Coder_RunCCtor,
	Coder_HandleLockedMethod,
	Coder_ProfileStart,
	Coder_ProfileEnd,
	Coder_SetOptimizationLevel,
	Coder_GetOptimizationLevel,
	"sentinel"
};
ILCoder _ILNullCoder = {&_ILNullCoderClass};

#ifdef	__cplusplus
};
#endif
