/*
 * lib_delegate.c - Delegate handling for the runtime engine.
 *
 * Copyright (C) 2002, 2011  Southern Storm Software, Pty Ltd.
 *
 * Contributions:  Thong Nguyen (tum@veridicus.com)
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

#include "engine_private.h"
#include "coder.h"
#include "lib_defs.h"
#include "il_opcodes.h"

#ifdef	__cplusplus
extern	"C" {
#endif

void *_ILDelegateGetClosure(ILExecThread *thread, ILObject *delegate)
{
#if defined(HAVE_LIBFFI) && defined(IL_CONFIG_RUNTIME_INFRA)
	ILMethod *methodInfo;

	/* See if we have a cached closure from last time */
	if(!delegate)
	{
		return 0;
	}
	if(((System_Delegate *)delegate)->closure)
	{
		return ((System_Delegate *)delegate)->closure;
	}

	/* If we don't have a method, then the delegate is invalid */
	methodInfo = ((System_Delegate *)delegate)->methodInfo;
	if(!methodInfo)
	{
		return 0;
	}

	/* Nail down the delegate object, to protect it from garbage collection */
	_IL_GCHandle_GCAlloc(thread, delegate, 2 /* Normal */);

	/* Make a native closure and cache it for next time */
	((System_Delegate *)delegate)->closure =
		_ILMakeClosureForDelegate(_ILExecThreadProcess(thread), delegate,
									methodInfo);
	return ((System_Delegate *)delegate)->closure;
#else
	/* We don't have support for creating closures on this system */
	fprintf(stderr, "We don't have support for creating closures on this system\n");
	return 0;
#endif
}

/*
 * private static Delegate CreateBlankDelegate(Type type, ClrMethod method);
 */
ILObject *_IL_Delegate_CreateBlankDelegate(ILExecThread *_thread,
										   ILObject *type,
										   ILObject *method)
{
	ILClass *classInfo;
	ILMethod *methodInfo;

	/* Convert the type into a delegate class descriptor */
	classInfo = _ILGetClrClass(_thread, type);
	if(!classInfo)
	{
		return 0;
	}

	/* Convert the "ClrMethod" instance into a method descriptor */
	if(!method)
	{
		return 0;
	}
	methodInfo = ((System_Reflection *)method)->privateData;
	if(!methodInfo)
	{
		return 0;
	}

	/* Check that the delegate signatures match */
	if(!ILTypeDelegateSignatureMatch(ILType_FromClass(classInfo), methodInfo))
	{
		return 0;
	}

	/* Create the delegate object and return */
	return _ILEngineAllocObject(_thread, classInfo);
}

/*
 * private static void SetOutParams(Delegate del, Object[] args,
 *									Object[] outParams);
 */
void _IL_AsyncResult_SetOutParams(ILExecThread *_thread, ILObject *del,
								  System_Array *args, System_Array *outParams)
{
	int i, j, paramcount;
	ILObject **argsArray, **outArray;
	ILType *invokeSignature, *paramType;

	invokeSignature = ILMethod_Signature(((System_Delegate *)del)->methodInfo);

	paramcount = ILTypeNumParams(invokeSignature);

	if(paramcount == 0 || outParams == 0)
	{
		/*
		 * The called method has no arguments so there is nothing to do.
		 * Or the outParams Array is null. This might be a programming error.
		 */
		return;
	}

	if (ArrayLength(args) < paramcount || ArrayLength(outParams) < paramcount)
	{
		return;
	}

	j = 0;

	argsArray = ((ILObject **)ArrayToBuffer(args));
	outArray = ((ILObject **)ArrayToBuffer(outParams));

	for (i = 1; i <= paramcount; i++)
	{
		paramType = ILTypeGetParam(invokeSignature, i);

		if (ILType_IsComplex(paramType) 
			&& ILType_Kind(paramType) == IL_TYPE_COMPLEX_BYREF)
		{
			paramType = ILType_Ref(paramType);

			if (j >= ArrayLength(outParams))
			{
				break;
			}

			outArray[j++] = argsArray[i - 1];
		}
	}
}

static ILMethod *_GetMethod(ILClass *classInfo, const char *name,
							ILType *signature)
{
	return (ILMethod *)ILClassNextMemberMatch(classInfo, 0,
											  IL_META_MEMBERKIND_METHOD,
											  name, signature);
}

static ILField *_GetField(ILClass *classInfo, const char *name)
{
	ILField *field;

	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind(classInfo, (ILMember *)field,
													  IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!strcmp(ILField_Name(field), name))
		{
			return field;
		}
	}
	return 0;
}

static void _ILCoderLoadInt32Constant(ILCoder *coder, ILUInt32 value)
{
	if(value < 9)
	{
		ILCoderConstant(coder, IL_OP_LDC_I4_0 + value, 0);
	}
	else
	{
		unsigned char int32Constant[4];

		IL_WRITE_UINT32(int32Constant, value);
		ILCoderConstant(coder, IL_OP_LDC_I4, int32Constant);
	}
}

static int _ILCoderCreateSimpleArrayType(ILMethod *method,
										 ILType *elemType,
										 ILType **arrayType,
										 ILClass **arrayClass)
{
	ILType *typeInfo;
	ILClass *classInfo;	
	ILImage *image;
	ILContext *context;

	image = ILProgramItem_Image(method);
	context = ILImageToContext(image);
	typeInfo = ILTypeFindOrCreateArray(context, 1, elemType);
	if(!typeInfo)
	{
		return 0;
	}
	classInfo = ILClassFromType(image, 0, typeInfo, 0);
	if(!classInfo)
	{
		return 0;
	}
	classInfo = ILClassResolve(classInfo);
	*arrayType = typeInfo;
	*arrayClass = classInfo;
	return 1;
}

static int _ILCoderUnboxValue(ILCoder *coder, ILImage *image, ILType *type,
							  ILCoderPrefixInfo *prefixInfo)
{
	ILClass *classInfo;

	type = ILTypeGetEnumType(type);
	classInfo = ILClassFromType(image, 0, type, 0);
	if(!classInfo)
	{
		return 0;
	}
	
	if(ILType_IsPrimitive(type))
	{
		/*
		 * Unbox the object to produce a managed pointer
		 */
		ILCoderUnbox(coder, classInfo, prefixInfo);
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_I1:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_I1, prefixInfo);
			}
			break;

			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_U1:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_U1, prefixInfo);
			}
			break;

			case IL_META_ELEMTYPE_I2:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_I2, prefixInfo);
			}
			break;

			case IL_META_ELEMTYPE_CHAR:
			case IL_META_ELEMTYPE_U2:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_U2, prefixInfo);
			}
			break;

			case IL_META_ELEMTYPE_I4:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_I4, prefixInfo);
			}
			break;

			case IL_META_ELEMTYPE_U4:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_U4, prefixInfo);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_I8, prefixInfo);
			}
			break;

			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_I, prefixInfo);
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_R4, prefixInfo);
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				ILCoderPtrAccess(coder, IL_OP_LDIND_R8, prefixInfo);
			}
			break;

			default:
			{
				/*
				 * Copy the value type on the stack.
				 */
				ILCoderPtrAccessManaged(coder, IL_OP_LDOBJ, classInfo, prefixInfo);
			}
		}
	}
	else if(ILType_IsValueType(type))
	{
		/*
		 * Unbox the object to produce a managed pointer
		 */
		ILCoderUnbox(coder, classInfo, prefixInfo);
		/*
		 * And copy the value type.
		 */
				ILCoderPtrAccessManaged(coder, IL_OP_LDOBJ, classInfo, prefixInfo);
	}
	else
	{
		/*
		 * Everything else is an object reference.
		 * 
		 * So there is nothing to do.
		 */
	}
	return 1;
}

static ILType *CreateDelegateSignature(ILType *invokeSignature, ILImage *image)
{
	ILType *signature;
	ILType *returnType;
	ILUInt32 numParams;
	ILUInt32 param;
	ILContext *context;

	context = ILImageToContext(image);
	returnType = ILTypeGetReturn(invokeSignature);
	/*
	objectClass = ILClassResolveSystem(image, 0, "Object", "System");
	if(!objectClass)
	{
		return 0;
	}
	*/
	signature = ILTypeCreateMethod(context, returnType);
	if(!signature)
	{
		return 0;
	}
	/*
	if(!ILTypeAddParam(context, signature, ILType_FromClass(objectClass)))
	{
		return 0;
	}
	*/
	numParams = ILTypeNumParams(invokeSignature);
	param = 1;
	while(param <= numParams)
	{
		ILType *paramType;

		paramType = ILTypeGetParam(invokeSignature, param);
		if(!ILTypeAddParam(context, signature, paramType))
		{
			return 0;
		}
		++param;
	}
	return signature;
}

static ILStandAloneSig* _ILCoderCreateLocalSignature(ILCoder *coder,
													 ILImage *image,
													 ILType **locals,
													 ILUInt32 numLocals)
{
	ILImage *syntheticImage;
	ILContext *context;
	ILType *localSig;
	ILUInt32 local;

	context = ILImageToContext(image);
	syntheticImage = ILContextGetSynthetic(context);
	if(!syntheticImage)
	{
		return 0;
	}
	localSig = ILTypeCreateLocalList(context);
	if(!localSig)
	{
		return 0;
	}
	local = 0;
	while(local < numLocals)
	{
		if(!ILTypeAddLocal(context, localSig, locals[local]))
		{
			return 0;
		}
		++local;
	}
	return ILStandAloneSigCreate(syntheticImage, 0, localSig);
}

static int _ILCoderGenDelegateCtor(ILCoder *coder, ILMethod *method,
								   unsigned char **start,
								   ILCoderExceptions *coderExceptions)
{
	ILClass *delegateClassInfo;
	ILField *targetField;
	ILField *methodField;
	ILType *signature;
	ILCoderPrefixInfo prefixInfo;
	ILMethodCode code;
	ILEngineStackItem stack[2];

	delegateClassInfo = ILClassResolveSystem(ILProgramItem_Image(method),
											 0, "Delegate", "System");
	if(!delegateClassInfo)
	{
		/* Ran out of memory trying to create "System.Delegate" */
		return IL_CODER_END_TOO_BIG;
	}
	targetField = _GetField(delegateClassInfo, "target");
	if(!targetField)
	{
		/* Field "target" is missing in the System.Delegate class */
		return IL_CODER_END_TOO_BIG;
	}
	methodField = _GetField(delegateClassInfo, "method");
	if(!methodField)
	{
		/* Field "target" is missing in the System.Delegate class */
		return IL_CODER_END_TOO_BIG;
	}

	signature = ILMethod_Signature(method);

	/* Initialize the prefix information */
	ILMemZero(&prefixInfo, sizeof(ILCoderPrefixInfo));
	ILMemZero(&code, sizeof(ILMethodCode));
	code.maxStack = 2;
	
	if(!ILCoderSetup(coder, start, method, &code, coderExceptions, 0))
	{
		return IL_CODER_END_TOO_BIG;
	}

	/* Load the this pointer on the stack */
	stack[0].engineType = ILEngineType_O;
	stack[0].typeInfo = ILType_FromClass(delegateClassInfo);
	ILCoderLoadArg(coder, 0, stack[0].typeInfo);

	/* Load the target argument on the stack */
	_ILCoderLoadArgs(coder, &(stack[1]), method, signature, 1, 1);
	
	ILCoderStoreField(coder, ILEngineType_O, stack[0].typeInfo,
					  targetField, ILField_Type(targetField),
					  stack[1].engineType, &prefixInfo);
	
	/* Load the this pointer on the stack */
	stack[0].engineType = ILEngineType_O;
	stack[0].typeInfo = ILType_FromClass(delegateClassInfo);
	ILCoderLoadArg(coder, 0, stack[0].typeInfo);

	/* Load the method pointer argument on the stack */
	_ILCoderLoadArgs(coder, &(stack[1]), method, signature, 2, 2);
	
	ILCoderStoreField(coder, ILEngineType_O, stack[0].typeInfo,
					  methodField, stack[1].typeInfo,
					  stack[1].engineType, &prefixInfo);
	
	/* And return from this method */
	ILCoderReturnInsn(coder, ILEngineType_Invalid, ILType_Void);

	/* Mark the end of the method */
	ILCoderMarkEnd(coder);

	return ILCoderFinish(coder);
}

static int GenDelegateInvoke(ILCoder *coder, ILMethod *method,
							 ILType *signature, ILUInt32 numParams,
							 ILEngineStackItem *stack,
							 ILField *targetField, ILField *methodField)
{
	ILCoderPrefixInfo prefixInfo;
	ILCoderMethodInfo coderMethodInfo;
	ILType *returnType;
	
	/* Initialize the prefix information */
	ILMemZero(&prefixInfo, sizeof(ILCoderPrefixInfo));
	returnType = ILTypeGetReturn(signature);

	ILCoderLoadThisField(coder, targetField,
						 ILField_Type(targetField), &prefixInfo);
	stack[0].typeInfo = ILField_Type(targetField);
	stack[0].engineType = _ILTypeToEngineType(stack[0].typeInfo);
	ILCoderBranch(coder, IL_OP_BRTRUE, 1, ILEngineType_O, ILEngineType_O);
	/* The stack is empty now */
	_ILCoderLoadArgs(coder, stack, method, signature, 1, numParams - 1);
	ILCoderLoadThisField(coder, methodField, ILType_Int, &prefixInfo);

	coderMethodInfo.args = stack;
	coderMethodInfo.numBaseArgs = numParams - 1;
	coderMethodInfo.numVarArgs = 0;
	coderMethodInfo.hasParamArray = 0;
	coderMethodInfo.tailCall = 0;
	coderMethodInfo.signature = CreateDelegateSignature(signature,
														ILProgramItem_Image(method));
	if(!coderMethodInfo.signature)
	{
		return 0;
	}

	_ILCoderSetReturnType(&(stack[numParams - 1]), returnType);
	ILCoderCallIndirect(coder, &coderMethodInfo,
						&(stack[numParams - 1]));
	if(returnType != ILType_Void)
	{
		ILCoderReturnInsn(coder, _ILTypeToEngineType(returnType),
						  returnType);
	}
	else
	{
		ILCoderReturnInsn(coder, ILEngineType_Invalid, ILType_Void);
	}
	ILCoderLabel(coder, 1);
	ILCoderLoadThisField(coder, targetField,
						 ILField_Type(targetField), &prefixInfo);
	stack[0].typeInfo = ILField_Type(targetField);
	stack[0].engineType = _ILTypeToEngineType(stack[0].typeInfo);
	_ILCoderLoadArgs(coder, &(stack[1]), method, signature, 1, numParams - 1);
	ILCoderLoadThisField(coder, methodField, ILType_Int, &prefixInfo);

	coderMethodInfo.args = stack;
	coderMethodInfo.numBaseArgs = numParams;
	coderMethodInfo.numVarArgs = 0;
	coderMethodInfo.hasParamArray = 0;
#ifdef IL_USE_CVM
	coderMethodInfo.tailCall = 1;
#else /* IL_USE_JIT */
	/*
	 * TODO: Make this a tail call if tail calls are fixed in libjit.
	 */
	coderMethodInfo.tailCall = 0;
#endif /* !IL_USE_JIT */
	coderMethodInfo.signature = signature;
	
	_ILCoderSetReturnType(&(stack[numParams]), returnType);
	ILCoderCallIndirect(coder, &coderMethodInfo, &(stack[numParams]));
	if(returnType != ILType_Void)
	{
		ILCoderReturnInsn(coder, _ILTypeToEngineType(returnType),
						  returnType);
	}
	else
	{
		ILCoderReturnInsn(coder, ILEngineType_Invalid, ILType_Void);
	}
	return 1;
}

static int _ILCoderGenDelegateInvoke(ILCoder *coder, ILMethod *method,
									 unsigned char **start,
									 ILCoderExceptions *coderExceptions)
{
	ILType *signature;

	signature = ILMethod_Signature(method);
	if(signature)
	{
		ILUInt32 numParams;

		numParams = ILTypeNumParams(signature);
		if(ILType_HasThis(signature))
		{
			/* Add the hidden this parameter */
			++numParams;
		}
		{
			ILClass *delegateClassInfo;
			ILField *targetField;
			ILField *methodField;
			ILMethodCode code;
			ILEngineStackItem stack[numParams + 3];
			
			delegateClassInfo = ILClassResolveSystem(ILProgramItem_Image(method),
													 0, "Delegate", "System");
			if(!delegateClassInfo)
			{
				/* Ran out of memory trying to create "System.Delegate" */
				return IL_CODER_END_TOO_BIG;
			}
			targetField = _GetField(delegateClassInfo, "target");
			if(!targetField)
			{
				/* Field "target" is missing in the System.Delegate class */
				return IL_CODER_END_TOO_BIG;
			}
			methodField = _GetField(delegateClassInfo, "method");
			if(!methodField)
			{
				/* Field "target" is missing in the System.Delegate class */
				return IL_CODER_END_TOO_BIG;
			}
			ILMemZero(&code, sizeof(ILMethodCode));
			code.maxStack = numParams + 3;
			
			if(!ILCoderSetup(coder, start, method, &code, coderExceptions, 0))
			{
				/* The coder setup failed */
				return IL_CODER_END_TOO_BIG;
			}
			if(!GenDelegateInvoke(coder, method, signature, numParams,
							  stack, targetField, methodField))
			{
				return IL_CODER_END_TOO_BIG;
			}

			/* Mark the end of the method */
			ILCoderMarkEnd(coder);
			
			return ILCoderFinish(coder);
		}
	}
	return IL_CODER_END_TOO_BIG;
}

static int _ILCoderGenMulticastDelegateInvoke(ILCoder *coder, ILMethod *method,
											  unsigned char **start,
											  ILCoderExceptions *coderExceptions)
{
	ILType *signature;

	signature = ILMethod_Signature(method);
	if(signature)
	{
		ILUInt32 numParams;

		numParams = ILTypeNumParams(signature);
		if(ILType_HasThis(signature))
		{
			/* Add the hidden this parameter */
			++numParams;
		}
		{
			ILCoderPrefixInfo prefixInfo;
			ILCoderMethodInfo coderMethodInfo;
			ILType *returnType;
			ILType *thisType;
			ILClass *multicastDelegateClassInfo;
			ILClass *delegateClassInfo;
			ILField *prevField;
			ILField *targetField;
			ILField *methodField;
			ILMethodCode code;
			ILEngineStackItem stack[numParams + 3];
			
			multicastDelegateClassInfo = ILClassResolveSystem(ILProgramItem_Image(method),
															  0, "MulticastDelegate",
															  "System");
			if(!multicastDelegateClassInfo)
			{
				/* Ran out of memory trying to create "System.MulticastDelegate" */
				return IL_CODER_END_TOO_BIG;
			}
			prevField = _GetField(multicastDelegateClassInfo, "prev");
			if(!prevField)
			{
				/* Field "prev" is missing in the System.MulticastDelegate class */
				return IL_CODER_END_TOO_BIG;
			}

			delegateClassInfo = ILClassResolveSystem(ILProgramItem_Image(method),
													 0, "Delegate", "System");
			if(!delegateClassInfo)
			{
				/* Ran out of memory trying to create "System.Delegate" */
				return IL_CODER_END_TOO_BIG;
			}
			targetField = _GetField(delegateClassInfo, "target");
			if(!targetField)
			{
				/* Field "target" is missing in the System.Delegate class */
				return IL_CODER_END_TOO_BIG;
			}
			methodField = _GetField(delegateClassInfo, "method");
			if(!methodField)
			{
				/* Field "target" is missing in the System.Delegate class */
				return IL_CODER_END_TOO_BIG;
			}
			returnType = ILTypeGetReturn(signature);
			thisType = ILType_FromClass(ILMethod_Owner(method));
			ILMemZero(&code, sizeof(ILMethodCode));
			code.maxStack = numParams + 3;
			
			if(!ILCoderSetup(coder, start, method, &code, coderExceptions, 0))
			{
				/* The coder setup failed */
				return IL_CODER_END_TOO_BIG;
			}

			ILCoderLoadThisField(coder, prevField,
								 ILField_Type(prevField), &prefixInfo);
			ILCoderBranch(coder, IL_OP_BRFALSE, 2, ILEngineType_O,
						  ILEngineType_O);
			ILCoderLoadThisField(coder, prevField,
								 ILField_Type(prevField), &prefixInfo);
			stack[0].typeInfo = thisType;
			stack[0].engineType = ILEngineType_O;
			_ILCoderLoadArgs(coder, &(stack[1]), method, signature, 1, numParams - 1);

			coderMethodInfo.args = stack;
			coderMethodInfo.numBaseArgs = numParams;
			coderMethodInfo.numVarArgs = 0;
			coderMethodInfo.hasParamArray = 0;
			coderMethodInfo.tailCall = 0;
			coderMethodInfo.signature = signature;

			_ILCoderSetReturnType(&(stack[numParams]), returnType);
			ILCoderCallMethod(coder, &coderMethodInfo, &(stack[numParams]),
							  method);
			/*
			 * If the delegate returns a valoe pop the value from the stack.
			 */
			if(returnType != ILType_Void)
			{
				ILCoderPop(coder, stack[numParams].engineType, returnType);
			}

			ILCoderLabel(coder, 2);
			/*
			 * Now generate the Delegate.Invoke code.
			 */
			if(!GenDelegateInvoke(coder, method, signature, numParams,
							  stack, targetField, methodField))
			{
				return IL_CODER_END_TOO_BIG;
			}

			/* Mark the end of the method */
			ILCoderMarkEnd(coder);
			
			return ILCoderFinish(coder);
		}
	}
	return IL_CODER_END_TOO_BIG;
}

static int _ILCoderGenDelegateBeginInvoke(ILCoder *coder, ILMethod *method,
										unsigned char **start,
										ILCoderExceptions *coderExceptions)
{
	ILType *signature;

	signature = ILMethod_Signature(method);
	if(signature)
	{
		ILImage *image;
		ILType *returnType;
		ILClass *objectClassInfo;
		ILType *arrayType;
		ILClass *arrayClass;
		ILClass *asyncResultClass;
		ILMethod *asyncResultCtorInfo;
		ILUInt32 numParams;
		ILMethodCode code;
		ILCoderPrefixInfo prefixInfo;
		ILCoderMethodInfo coderMethodInfo;
		ILType *locals[2];
		ILEngineStackItem stack[4];

		image = ILProgramItem_Image(method);
		returnType = ILTypeGetReturn(signature);
		numParams = ILTypeNumParams(signature);

		objectClassInfo = ILClassResolveSystem(image, 0, "Object", "System");
		if(!objectClassInfo)
		{
			/* Ran out of memory trying to create "System.Delegate" */
			return IL_CODER_END_TOO_BIG;
		}

		asyncResultClass = ILClassResolveSystem(image, 0, "AsyncResult",
												"System.Runtime.Remoting.Messaging");
		if(!asyncResultClass)
		{
			/* Ran out of memory trying to create "System.Delegate" */
			return IL_CODER_END_TOO_BIG;
		}

		/* Get the AsyncResult.BeginInvoke method */
		asyncResultCtorInfo = _GetMethod(asyncResultClass, ".ctor", 0);
		if(asyncResultCtorInfo == 0)
		{
			return IL_CODER_END_TOO_BIG;
		}

		if(!_ILCoderCreateSimpleArrayType(method,
										  ILType_FromClass(objectClassInfo),
										  &arrayType, &arrayClass))
		{
			/* Ran out of memory trying to create "System.Delegate" */
			return IL_CODER_END_TOO_BIG;
		}

		ILMemZero(&code, sizeof(ILMethodCode));
		if(numParams > 2)
		{
			/*
			 * The delegate has arguments so we'll need an array for the
			 * boxed arguments.
			 */
			locals[0] = arrayType;
			code.localVarSig = _ILCoderCreateLocalSignature(coder, image, locals, 1);
			if(!code.localVarSig)
			{
				/* Ran out of memory trying to create te locals signature */
				return IL_CODER_END_TOO_BIG;
			}
		}
		code.maxStack = 4;

		ILMemZero(&prefixInfo, sizeof(ILCoderPrefixInfo));
		if(!ILCoderSetup(coder, start, method, &code, coderExceptions, 0))
		{
			/* The coder setup failed */
			return IL_CODER_END_TOO_BIG;
		}

		if(numParams > 2)
		{
			ILUInt32 currentParam;
			ILUInt32 currentLabel;
			ILExecThread *thread;
			
			thread = ILExecThreadCurrent();
			/*
			 * Setup the labels used to handle byref arguments.
			 */
			currentLabel = 0;

			/*
			 * We have arguments so create an array to hold the
			 * boxed values.
			 * Load the number of elements for the array to be created on the stack.
			 */
			_ILCoderLoadInt32Constant(coder, numParams - 2);
			/*
			 * Create the array for the params.
			 */
			ILCoderNewArray(coder, arrayType, arrayClass, ILEngineType_I4);
			/*
			 * Save the array in the local slot 0
			 */
			ILCoderStoreLocal(coder, 0, ILEngineType_O, locals[0]);
			currentParam = 1;
			while(currentParam <= (numParams - 2))
			{
				ILType *paramType;

				paramType = ILTypeGetParam(signature, currentParam);
				if(ILType_IsRef(paramType))
				{
					ILType *refType;
					ILClass *refClassInfo;
					int refTypeIsValue;
					
					refType = ILType_Ref(paramType);
					refClassInfo = ILClassFromType(image, 0, refType, 0);
					if(!refClassInfo)
					{
						return IL_CODER_END_TOO_BIG;
					}
					refTypeIsValue = ILTypeIsValue(refType);
					/*
					 * Load the argument on the stack.
					 */
					_ILCoderLoadArgs(coder, stack, method, signature,
									 currentParam, currentParam);
					ILCoderBranch(coder, IL_OP_BRFALSE, currentLabel + 1,
								  ILEngineType_O, ILEngineType_O);
					/*
					 * Load the array and the index on the stack for the
					 * store to the array.
					 */
					ILCoderLoadLocal(coder, 0, locals[0]);
					_ILCoderLoadInt32Constant(coder, currentParam - 1);
					if(refTypeIsValue)
					{
						/*
						 * Load the managed pointer on the stack.
						 */
						_ILCoderLoadArgs(coder, &stack[2], method, signature,
										 currentParam, currentParam);
						/*
						 * And box the value the pointer references.
						 */
						if(!_ILCoderBoxPtr(_ILExecThreadProcess(thread),
										   refType, refClassInfo, 0))
						{
							return IL_CODER_END_TOO_BIG;
						}
					}
					else
					{
						/*
						 * Load the object reference from the pointer.
						 */
						_ILCoderLoadArgs(coder, &stack[2], method, signature,
										 currentParam, currentParam);
						/*
						 * And get the referenced object.
						 */
						ILCoderPtrAccess(coder, IL_OP_LDIND_REF, &prefixInfo);
					}
					/* 
					 * Store the pointer on the stack in the array.
					 */
					ILCoderArrayAccess(coder, IL_OP_STELEM_REF,
									   ILEngineType_I4,
									   ILType_FromClass(objectClassInfo),
									   &prefixInfo);
					ILCoderBranch(coder, IL_OP_BR, currentLabel + 2,
								  ILEngineType_O, ILEngineType_O);
					ILCoderLabel(coder, currentLabel + 1);
					/*
					 * Handle the case where the ref pointer is null.
					 */
					/*
					 * Load the array and the index on the stack for the
					 * store to the array.
					 */
					ILCoderLoadLocal(coder, 0, locals[0]);
					_ILCoderLoadInt32Constant(coder, currentParam - 1);
					if(refTypeIsValue)
					{
						/*
						 * TODO: Create a 0 boxed value
						 */
						ILCoderConstant(coder, IL_OP_LDNULL, 0);
					}
					else
					{
						/*
						 * Load a null reference and store it in the array.
						 */
						ILCoderConstant(coder, IL_OP_LDNULL, 0);
					}
					ILCoderArrayAccess(coder, IL_OP_STELEM_REF,
									   ILEngineType_I4,
									   ILType_FromClass(objectClassInfo),
									   &prefixInfo);
					ILCoderLabel(coder, currentLabel + 2);
					currentLabel += 2;
				}
				else
				{
					/*
					 * Load the array and the index on the stack for the
					 * store to the array.
					 */
					ILCoderLoadLocal(coder, 0, locals[0]);
					_ILCoderLoadInt32Constant(coder, currentParam - 1);
					if(ILTypeIsValue(paramType))
					{
						ILClass *paramClassInfo;
						ILExecThread *thread;

						thread = ILExecThreadCurrent();
						paramClassInfo = ILClassFromType(image, 0, paramType, 0);
						if(!paramClassInfo)
						{
							return IL_CODER_END_TOO_BIG;
						}
						/*
						 * Get the address of the argument.
						 */
						ILCoderAddrOfArg(coder, currentParam);
						
						/*
						 * And box the value the pointer references.
						 */
						if(!_ILCoderBoxPtr(_ILExecThreadProcess(thread),
										   paramType, paramClassInfo, 0))
						{
							return IL_CODER_END_TOO_BIG;
						}
					}
					else
					{
						/*
						 * Load the argument on the stack.
						 */
						_ILCoderLoadArgs(coder, &stack[2], method, signature,
										 currentParam, currentParam);
					}
					ILCoderArrayAccess(coder, IL_OP_STELEM_REF, ILEngineType_I4,
									   ILType_FromClass(objectClassInfo),
									   &prefixInfo);
				}
				++currentParam;
			}
		}

		/*
		 * Create the AsyncResult object. 
		 */
		/*
		 * The current delegate object.
		 */
		_ILCoderLoadArgs(coder, stack, method, signature, 0, 0);
		/*
		 * The array with the arguments
		 */
		if(numParams > 2)
		{
			ILCoderLoadLocal(coder, 0, locals[0]);
		}
		else
		{
			/*
			 * No arguments so the array to hold the values is not needed.
			 */
			ILCoderConstant(coder, IL_OP_LDNULL, 0);
		}
		stack[1].typeInfo = arrayType;
		stack[1].engineType = ILEngineType_O;
		/*
		 * The callback and the state object.
		 */
		_ILCoderLoadArgs(coder, &stack[2], method, signature, numParams - 1, numParams);

		coderMethodInfo.args = stack;
		coderMethodInfo.numBaseArgs = 4;
		coderMethodInfo.numVarArgs = 0;
		coderMethodInfo.hasParamArray = 0;
		coderMethodInfo.tailCall = 0;
		coderMethodInfo.signature = ILMethod_Signature(asyncResultCtorInfo);

		ILCoderCallCtor(coder, &coderMethodInfo, asyncResultCtorInfo);
		/*
		 * Return the AsyncResult to the caller.
		 */
		ILCoderReturnInsn(coder, _ILTypeToEngineType(returnType),
						  returnType);
		/* Mark the end of the method */
		ILCoderMarkEnd(coder);

		return ILCoderFinish(coder);
	}
	return IL_CODER_END_TOO_BIG;
}

static int _ILCoderGenDelegateEndInvoke(ILCoder *coder, ILMethod *method,
										unsigned char **start,
										ILCoderExceptions *coderExceptions)
{
	ILType *signature;

	signature = ILMethod_Signature(method);
	if(signature)
	{
		ILImage *image;
		ILClass *delegateClass;
		ILType *arrayType;
		ILClass *arrayClass;
		ILClass *objectClassInfo;
		ILType *returnType;
		ILClass *returnClass;
		ILClass *asyncResultClass;
		ILMethod *asyncEndInvokeMethodInfo;
		ILMethod *invokeMethodInfo;
		ILType *invokeSignature;
		ILUInt32 numParams;
		ILMethodCode code;
		ILType *locals[2];

		image = ILProgramItem_Image(method);
		numParams = ILTypeNumParams(signature);
		returnType = ILTypeGetReturn(signature);
		delegateClass = ILMethod_Owner(method);
		ILMemZero(&code, sizeof(ILMethodCode));
		objectClassInfo = ILClassResolveSystem(image, 0, "Object", "System");
		if(!objectClassInfo)
		{
			/* Ran out of memory trying to create "System.Delegate" */
			return IL_CODER_END_TOO_BIG;
		}

		asyncResultClass = ILClassResolveSystem(image, 0, "AsyncResult",
												"System.Runtime.Remoting.Messaging");
		if(!asyncResultClass)
		{
			/* Ran out of memory trying to create "System.Delegate" */
			return IL_CODER_END_TOO_BIG;
		}
		/* Get the AsyncResult.EndInvoke method */
		asyncEndInvokeMethodInfo = _GetMethod(asyncResultClass, "EndInvoke", 0);
		if(asyncEndInvokeMethodInfo == 0)
		{
			return IL_CODER_END_TOO_BIG;
		}

		returnClass = ILClassFromType(image, 0, returnType, 0);
		if(!returnClass)
		{
			return IL_CODER_END_TOO_BIG;
		}

		invokeMethodInfo = _GetMethod(delegateClass, "Invoke", 0);
		if(!invokeMethodInfo)
		{
			return IL_CODER_END_TOO_BIG;
		}
		invokeSignature = ILMethod_Signature(invokeMethodInfo);

		if(!_ILCoderCreateSimpleArrayType(method,
										  ILType_FromClass(objectClassInfo),
										  &arrayType, &arrayClass))
		{
			/* Ran out of memory trying to create "System.Delegate" */
			return IL_CODER_END_TOO_BIG;
		}

		locals[0] = arrayType;
		if(numParams > 1 && returnType != ILType_Void)
		{
			locals[1] = ILType_FromClass(objectClassInfo);
			code.localVarSig = _ILCoderCreateLocalSignature(coder, image, locals, 2);
		}
		else
		{
			code.localVarSig = _ILCoderCreateLocalSignature(coder, image, locals, 1);
		}
		if(!code.localVarSig)
		{
			/* Ran out of memory trying to create te locals signature */
			return IL_CODER_END_TOO_BIG;
		}

		{
			ILCoderPrefixInfo prefixInfo;
			ILCoderMethodInfo coderMethodInfo;
			ILEngineStackItem stack[3];

			ILMemZero(&prefixInfo, sizeof(ILCoderPrefixInfo));
			code.maxStack = 3;
			if(!ILCoderSetup(coder, start, method, &code, coderExceptions, 0))
			{
				/* The coder setup failed */
				return IL_CODER_END_TOO_BIG;
			}
			
			if(numParams > 1)
			{
				/*
				 * We have byref arguments so create an array to hold the
				 * boxed byref values.
				 * Load the number of elements for the array to be created on the stack.
				 */
				_ILCoderLoadInt32Constant(coder, numParams - 1);
				/*
				 * Create the array for the out params.
				 */
				ILCoderNewArray(coder, arrayType, arrayClass, ILEngineType_I4);
			}
			else
			{
				/*
				 * No ref arguments so the array to hold the values is not needed.
				 */
				ILCoderConstant(coder, IL_OP_LDNULL, 0);
			}
			/*
			 * Save the array in the local slot 0
			 */
			ILCoderStoreLocal(coder, 0, ILEngineType_O, locals[0]);
			
			/*
			 * Call the IAsyncResult.EndInvoke method.
			 */
			_ILCoderLoadArgs(coder, stack, method, signature,
							 numParams, numParams);
			ILCoderLoadLocal(coder, 0, locals[0]);
			stack[1].typeInfo = arrayType;
			stack[1].engineType = ILEngineType_O;
			_ILCoderSetReturnType(&stack[2], returnType);

			coderMethodInfo.args = stack;
			coderMethodInfo.numBaseArgs = 2;
			coderMethodInfo.numVarArgs = 0;
			coderMethodInfo.hasParamArray = 0;
			coderMethodInfo.tailCall = 0;
			coderMethodInfo.signature = ILMethod_Signature(asyncEndInvokeMethodInfo);

			ILCoderCallMethod(coder, &coderMethodInfo, &(stack[2]),
							  asyncEndInvokeMethodInfo);

			if(returnType == ILType_Void)
			{
				/*
				 * Pop the object returned from the stack.
				 */
				ILCoderPop(coder, ILEngineType_O, ILType_Invalid);
			}
			if(numParams > 1)
			{
				ILUInt32 paramCount;
				ILUInt32 paramNum;
				ILUInt32 currentParam;

				if(returnType != ILType_Void)
				{
					/*
					 * Save the object with the return value in the local slot 1
					 */
					ILCoderStoreLocal(coder, 1, ILEngineType_O, locals[1]);
				}				
				/*
				 * Copy the ref values.
				 */
				paramCount = ILTypeNumParams(invokeSignature);
				paramNum = 1;
				currentParam = 0;
				while(paramNum <= paramCount)
				{
					ILType *paramType;

					paramType = ILTypeGetParam(invokeSignature, paramNum);
					if(ILType_IsRef(paramType))
					{
						ILType *refType;
						ILClass *refClass;

						refType = ILType_Ref(paramType);
						refType = ILTypeGetEnumType(refType);
						refClass = ILClassFromType(image, 0, refType, 0);
						if(!refClass)
						{
							return IL_CODER_END_TOO_BIG;
						}

						/*
						 * Load the ref address for this argument on the stack
						 * for the store to this addtess.
						 */
						_ILCoderLoadArgs(coder, stack, method, signature,
										 currentParam + 1, currentParam + 1);
						/*
						 * Load the array on the stack.
						 */
						ILCoderLoadLocal(coder, 0, locals[0]);
						/*
						 * Load the index on the stack.
						 */
						_ILCoderLoadInt32Constant(coder, currentParam);
						/*
						 * And get the object from the array.
						 */
						ILCoderArrayAccess(coder, IL_OP_LDELEM_REF,
										   ILEngineType_I4,
										   ILType_FromClass(objectClassInfo),
										   &prefixInfo);
						/*
						 * Cast the class to the expected class and throw an
						 * exception if that's not possible.
						 */
						ILCoderCastClass(coder, refClass, 1, &prefixInfo);
						if(ILType_IsPrimitive(refType))
						{
							/*
							 * Unbox the object to produce a managed pointer
							 */
							ILCoderUnbox(coder, refClass, &prefixInfo);
							switch(ILType_ToElement(refType))
							{
								case IL_META_ELEMTYPE_I1:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_I1,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_I1,
													 &prefixInfo);
								}
								break;
								
								case IL_META_ELEMTYPE_BOOLEAN:
								case IL_META_ELEMTYPE_U1:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_U1,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_I1,
													 &prefixInfo);
								}
								break;

								case IL_META_ELEMTYPE_I2:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_I2,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_I2,
													 &prefixInfo);
								}
								break;

								case IL_META_ELEMTYPE_CHAR:
								case IL_META_ELEMTYPE_U2:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_U2,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_I2,
													 &prefixInfo);
								}
								break;

								case IL_META_ELEMTYPE_I4:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_I4,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_I4,
													 &prefixInfo);
								}
								break;

								case IL_META_ELEMTYPE_U4:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_U4,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_I4,
													 &prefixInfo);
								}
								break;
								
								case IL_META_ELEMTYPE_I8:
								case IL_META_ELEMTYPE_U8:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_I8,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_I8,
													 &prefixInfo);
								}
								break;

								case IL_META_ELEMTYPE_I:
								case IL_META_ELEMTYPE_U:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_I,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_I,
													 &prefixInfo);
								}
								break;

								case IL_META_ELEMTYPE_R4:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_R4,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_R4,
													 &prefixInfo);
								}
								break;

								case IL_META_ELEMTYPE_R8:
								{
									ILCoderPtrAccess(coder, IL_OP_LDIND_R8,
													 &prefixInfo);
									ILCoderPtrAccess(coder, IL_OP_STIND_R8,
													 &prefixInfo);
								}
								break;

								default:
								{
									/*
									 * Copy the value type.
									 */
									ILCoderCopyObject(coder, ILEngineType_M,
													  ILEngineType_M, refClass);
								}
							}
						}
						else if(ILType_IsValueType(refType))
						{
							/*
							 * Unbox the object to produce a managed pointer
							 */
							ILCoderUnbox(coder, refClass, &prefixInfo);
							/*
							 * And copy the value type.
							 */
							ILCoderCopyObject(coder, ILEngineType_M,
											  ILEngineType_M, refClass);
						}
						else
						{
							/*
							 * Everything else is an object reference.
							 */
							ILCoderPtrAccess(coder, IL_OP_STIND_REF,
											 &prefixInfo);
						}
						++ currentParam;
					}
					++paramNum;
				}
				if(returnType != ILType_Void)
				{
					/*
					 * Reload the return value on the stack.
					 */
					ILCoderLoadLocal(coder, 1, locals[1]);
				}
			}
			if(returnType != ILType_Void)
			{
				/*
				 * Get the return value.
				 * If the value returned is a value type we have to unbox the
				 * value prior to returning it.
				 */
				ILCoderCastClass(coder, returnClass, 1, &prefixInfo);
				if(!_ILCoderUnboxValue(coder, image, returnType, &prefixInfo))
				{
					return IL_CODER_END_TOO_BIG;
				}
				ILCoderReturnInsn(coder, _ILTypeToEngineType(returnType),
								  returnType);
			}
			else
			{
				ILCoderReturnInsn(coder, ILEngineType_Invalid, ILType_Void);
			}
			/* Mark the end of the method */
			ILCoderMarkEnd(coder);
			
			return ILCoderFinish(coder);
		}
	}
	return IL_CODER_END_TOO_BIG;
}

#ifdef IL_USE_JIT

static ILObject *Delegate_BeginInvoke(ILExecThread *thread, ILObject *_this)
{	
	/* This is a dummy because the real function is generated by the */
	/* jit coder. */
	return 0;
}

#endif /* IL_USE_JIT */
 
int _ILGetInternalDelegate(ILMethod *method, int *isCtor,
						   ILInternalInfo *info)
{
	ILClass *classInfo;
	const char *name;
	ILType *type;

	/* Determine if the method's class is indeed a delegate */
	classInfo = ILMethod_Owner(method);
	if(!classInfo)
	{
		return 0;
	}
	if(!ILTypeIsDelegate(ILType_FromClass(classInfo)))
	{
		return 0;
	}

	/* Determine which method we are looking for */
	name = ILMethod_Name(method);
	type = ILMethod_Signature(method);
	if(!strcmp(name, ".ctor"))
	{
		/* Check that the constructor has the correct signature */
		if(_ILLookupTypeMatch(type, "(ToSystem.Object;j)V"))
		{
			*isCtor = 1;
			info->un.gen = _ILCoderGenDelegateCtor;
			info->flags = _IL_INTERNAL_GENCODE;
			return 1;
		}
	}
	else if(!strcmp(name, "Invoke") && ILType_HasThis(type))
	{
		/* This is the delegate invocation method */
		ILClass *parent;
		*isCtor = 0;

		parent = ILClass_ParentClass(classInfo);
		name = ILClass_Name(parent);
		if(!strcmp(name, "MulticastDelegate"))
		{
			info->un.gen = _ILCoderGenMulticastDelegateInvoke;
			info->flags = _IL_INTERNAL_GENCODE;
			return 1;
		}
		if(!strcmp(name, "Delegate"))
		{
			info->un.gen = _ILCoderGenDelegateInvoke;
			info->flags = _IL_INTERNAL_GENCODE;
			return 1;
		}
	}
	else if(!strcmp(name, "BeginInvoke"))
	{
		*isCtor = 0;

		info->un.gen = (void *)_ILCoderGenDelegateBeginInvoke;
		info->flags = _IL_INTERNAL_GENCODE;
		return 1;
	}
	else if(!strcmp(name, "EndInvoke"))
	{
		/* This is the delegate invocation method */
		*isCtor = 0;
		
		info->un.gen = (void *)_ILCoderGenDelegateEndInvoke;
		info->flags = _IL_INTERNAL_GENCODE;
		return 1;
	}

	return 0;
}

#ifdef	__cplusplus
};
#endif
