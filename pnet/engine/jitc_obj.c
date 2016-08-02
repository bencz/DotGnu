/*
 * jitc_obj.c - Coder implementation for JIT object operations.
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

#ifdef IL_JITC_CODE

/*
 * Get a ILJitValue with a pointer to the class' static area.
 */
static ILJitValue _ILJitGetClassStaticArea(ILJitFunction func,
										   ILField *field)
{
	/* Get the static data area for a particular class */
	ILClass *classInfo = ILField_Owner(field);
	void *classStaticData = ((ILClassPrivate *)(classInfo->userData))->staticData;

	if(!classStaticData)
	{
		ILClassPrivate *classPrivate = (ILClassPrivate *)(classInfo->userData);

		if(classPrivate->managedStatic)
		{
		#ifdef	IL_USE_TYPED_ALLOCATION
			ILNativeInt staticTypeDescriptor =
				ILGCBuildStaticTypeDescriptor(classInfo, classPrivate->managedStatic);

			if(staticTypeDescriptor)
			{
				classStaticData = ILGCAllocExplicitlyTyped(classPrivate->staticSize,
														   staticTypeDescriptor);
			}
			else
			{
				classStaticData = ILGCAlloc(classPrivate->staticSize);
			}
		#else	/* !IL_USE_TYPED_ALLOCATION */
			classStaticData = ILGCAlloc(classPrivate->staticSize);
		#endif	/* !IL_USE_TYPED_ALLOCATION */
		}
		else
		{
			/* There are no managed fields in the static area, so use atomic
			   allocation */
			classStaticData = ILGCAllocAtomic(classPrivate->staticSize);
		}
		if(!classStaticData)
		{
			jit_exception_builtin(JIT_RESULT_OUT_OF_MEMORY);
		}
		else
		{
			classPrivate->staticData = classStaticData;
		}
	}
	return jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR,
												(jit_nint)classStaticData);
}

#ifdef IL_CONFIG_PINVOKE

/*
 * Get the address of a PInvoke-imported field.
 */
static ILJitValue _ILJitGetPInvokeFieldAddress(ILJitFunction func,
											   ILField *field,
											   ILPInvoke *pinvoke)
{
	char *name;
	void *handle;
	void *symbol;
	const char *symbolName;
	ILJitValue ptr;

	/* Resolve the module */
	name = ILPInvokeResolveModule(pinvoke);
	if(!name)
	{
		ptr = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, 0);
		jit_insn_check_null(func, ptr);
		return ptr;
	}

	/* Load the module into memory */
	handle = ILDynLibraryOpen(name);
	ILFree(name);
	if(!handle)
	{
		ptr = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, 0);
		jit_insn_check_null(func, ptr);
		return ptr;
	}

	/* Resolve the symbol and resolve its address */
	symbolName = ILPInvoke_Alias(pinvoke);
	if(!symbolName)
	{
		symbolName = ILField_Name(field);
	}
	symbol = ILDynLibraryGetSymbol(handle, symbolName);

	/* Check if the symbol could be resolved. */
	if(!symbol)
	{
		ptr = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, 0);
		jit_insn_check_null(func, ptr);
		return ptr;
	}
	ptr = jit_value_create_nint_constant(func,
										 _IL_JIT_TYPE_VPTR,
										 (jit_nint)symbol);
	return ptr;
}

#endif /* IL_CONFIG_PINVOKE */

/*
 * Get a ILJitValue with the address of a RVA static field.
 */
static ILJitValue _ILJitGetRVAAddress(ILJitFunction func,
									  ILField *field)
{
	ILFieldRVA *fieldRVA = ILFieldRVAGetFromOwner(field);
	ILUInt32 rva = ILFieldRVAGetRVA(fieldRVA);
	/* Eventually the address should be computed on runtime. */
	void *ptr = ILImageMapAddress(ILProgramItem_Image(field), rva);

	return jit_value_create_nint_constant(func,
										  _IL_JIT_TYPE_VPTR,
										  (jit_nint)ptr);
}

/*
 * Get an ILJitValue with a pointer to the thread static slot.
 */
static ILJitValue _ILJitGetThreadStaticSlot(ILJITCoder *jitCoder,
										    ILField *field)
{
	ILJitValue args[3];
	ILJitValue slot;
	jit_label_t label = jit_label_undefined;

	args[0] = _ILJitCoderGetThread(jitCoder);
	args[1] = jit_value_create_nint_constant(jitCoder->jitFunction,
											 _IL_JIT_TYPE_UINT32,
											 (jit_nint)field->offset);
	args[2] = jit_value_create_nint_constant(jitCoder->jitFunction,
											 _IL_JIT_TYPE_UINT32,
											 (jit_nint)field->nativeOffset);
	slot = jit_insn_call_native(jitCoder->jitFunction,
								"ILRuntimeGetThreadStatic",
								ILRuntimeGetThreadStatic,
								_ILJitSignature_ILRuntimeGetThreadStatic,
								args, 3, JIT_CALL_NOTHROW);
	jit_insn_branch_if(jitCoder->jitFunction, slot, &label);
	_ILJitThrowCurrentException(jitCoder);
	jit_insn_label(jitCoder->jitFunction, &label);
	return slot;
}

/*
 * Get the value pointer from a typed reference.
 * A check is made if the type of typed reference matches the given classInfo.
 * If they don't match an InvalidCastEWxception is thrown.
 */
static ILJitValue _ILJitGetValFromRef(ILJITCoder *jitCoder, ILJitValue refValue,
									  ILClass *classInfo)
{
	jit_label_t label1 = jit_label_undefined;
	ILJitValue info = jit_value_create_nint_constant(jitCoder->jitFunction,
				    								 _IL_JIT_TYPE_VPTR,
								    				 (jit_nint)classInfo);
	ILJitValue ptr = jit_insn_address_of(jitCoder->jitFunction, refValue);;
	ILJitValue type = jit_insn_load_relative(jitCoder->jitFunction,
											 ptr,
											 offsetof(ILTypedRef, type),
											 _IL_JIT_TYPE_VPTR);
	ILJitValue temp;
	ILJitValue valuePtr;

	temp = jit_insn_eq(jitCoder->jitFunction, type, info);
	jit_insn_branch_if(jitCoder->jitFunction, temp, &label1);
	/* Throw an InvalidCastException */
	_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_INVALID_CAST);
	jit_insn_label(jitCoder->jitFunction, &label1);
	valuePtr = jit_insn_load_relative(jitCoder->jitFunction, ptr,
									  offsetof(ILTypedRef, value),
									  _IL_JIT_TYPE_VPTR);
	return valuePtr;
}

/*
 * Process the ldflda / ldsflda opcodes.
 * Returns the ILJitValue with the field address.
 */
static void _ILJitLoadFieldAddress(ILJITCoder *coder,
								   ILJitStackItem *dest,
								   ILJitStackItem *base,
								   ILUInt32 offset,
								   int mayBeNull)
{
	if(mayBeNull)
	{
		_ILJitStackItemCheckNull(coder, *base);
	}
	_ILJitStackItemFieldAddress(coder, *dest, *base, offset);
}

/*
 * Process the ldfld / ldsfld opcodes.
 * Returns the ILJitValue with the field contents.
 */
static ILJitValue _ILJitLoadField(ILJITCoder *coder, ILJitStackItem *base,
								  ILType *fieldType, ILUInt32 offset,
								  int mayBeNull)
{
	ILJitType type = _ILJitGetReturnType(fieldType, coder->process);

	if(mayBeNull)
	{
		_ILJitStackItemCheckNull(coder, *base);
	}
	return jit_insn_load_relative(coder->jitFunction,
								  _ILJitStackItemValue(*base),
								  offset,
								  type);
}

/*
 * Process the stfld / stsfld opcodes.
 */
static void _ILJitStoreField(ILJITCoder *coder, ILJitStackItem *base,
								   ILJitValue value, ILType *fieldType,
								   ILUInt32 offset, int mayBeNull)
{
	ILJitType type = _ILJitGetReturnType(fieldType, coder->process);

	if(mayBeNull)
	{
		_ILJitStackItemCheckNull(coder, *base);
	}
	if(jit_value_get_type(value) != type)
	{
		value = _ILJitValueConvertImplicit(coder->jitFunction, value, type);
	}
	_ILJitStackItemStoreRelative(coder, *base, offset, value);
}

static void JITCoder_CastClass(ILCoder *coder, ILClass *classInfo,
							   int throwException,
							   const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitValue classTo = jit_value_create_nint_constant(jitCoder->jitFunction,
														_IL_JIT_TYPE_VPTR,
														(jit_nint)classInfo);
	jit_label_t label = jit_label_undefined;
	_ILJitStackItemNew(object);
	ILJitValue args[3];
	ILJitValue returnValue;
	ILJitValue result = jit_value_create(jitCoder->jitFunction,
										 _IL_JIT_TYPE_VPTR);

	_ILJitStackPop(jitCoder, object);
	if(!throwException)
	{
		jit_insn_store(jitCoder->jitFunction,
					   result,
					   _ILJitStackItemValue(object));
	}
	jit_insn_branch_if_not(jitCoder->jitFunction,
						   _ILJitStackItemValue(object),
						   &label);
	if(ILClass_IsInterface(classInfo))
	{
		/* We are casting to an interface */
		args[0] = _ILJitStackItemValue(object);
		args[1] = classTo;
		returnValue = jit_insn_call_native(jitCoder->jitFunction,
										   "ILRuntimeClassImplements",
										   ILRuntimeClassImplements,
										   _ILJitSignature_ILRuntimeClassImplements,
										   args, 2, JIT_CALL_NOTHROW);
	}
	else
	{
		args[0] = jit_value_create_nint_constant(jitCoder->jitFunction,
												 _IL_JIT_TYPE_VPTR,
												 (jit_nint)ILCCtorMgr_GetCurrentMethod(&(jitCoder->cctorMgr)));
		args[1] = _ILJitStackItemValue(object);
		args[2] = classTo;
		returnValue = jit_insn_call_native(jitCoder->jitFunction,
										   "ILRuntimeCanCastClass",
										   ILRuntimeCanCastClass,
										   _ILJitSignature_ILRuntimeCanCastClass,
										   args, 3, JIT_CALL_NOTHROW);
	}
	if(throwException)
	{
		_ILJitStackPush(jitCoder, object);
		jit_insn_branch_if(jitCoder->jitFunction, returnValue, &label);
		/* Throw an InvalidCastException. */
		_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_INVALID_CAST);

	}
	else
	{
		_ILJitStackPushValue(jitCoder, result);
		ILJitValue nullPointer = 
				jit_value_create_nint_constant(jitCoder->jitFunction,
											   _IL_JIT_TYPE_VPTR,
											   (jit_nint)0);
	
		jit_insn_branch_if(jitCoder->jitFunction, returnValue, &label);
		jit_insn_store(jitCoder->jitFunction, result, nullPointer);
	}
	jit_insn_label(jitCoder->jitFunction, &label);
}

static void JITCoder_LoadField(ILCoder *coder, ILEngineType ptrType,
							   ILType *objectType, ILField *field,
							   ILType *fieldType,
							   const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitValue value = 0;
	_ILJitStackItemNew(ptr);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"LoadField: %s.%s at offset %i\n", 
			ILClass_Name(ILField_Owner(field)),
			ILField_Name(field),
			field->offset);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	_ILJitStackPop(jitCoder, ptr);
	if(ptrType == ILEngineType_MV)
	{
		/* We load a field from a valuetype on the stack. */
		value = jit_insn_address_of(jitCoder->jitFunction,
									_ILJitStackItemValue(ptr));
		_ILJitStackItemSetValue(ptr, value);
		value = _ILJitLoadField(jitCoder,
								&ptr,
								fieldType, field->offset, 0);
	}
	else
	{
		value = _ILJitLoadField(jitCoder,
								&ptr,
								fieldType, field->offset, 1);
	}
	_ILJitStackPushValue(jitCoder, value);
}

static void JITCoder_LoadThisField(ILCoder *coder, ILField *field,
								   ILType *fieldType,
								   const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(param);
	ILJitValue value = 0;

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"LoadThisField: %s.%s at offset %i\n", 
			ILClass_Name(ILField_Owner(field)),
			ILField_Name(field),
			field->offset);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	_ILJitStackItemLoadArg(jitCoder, param, 0);
	value = _ILJitLoadField(jitCoder, &param, fieldType, field->offset, 1);
	_ILJitStackPushValue(jitCoder, value);
}

static void JITCoder_LoadStaticField(ILCoder *coder, ILField *field,
									 ILType *fieldType,
									 const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitValue value = 0;

#ifdef IL_CONFIG_PINVOKE
	ILPInvoke *pinvoke;
	if((field->member.attributes & IL_META_FIELDDEF_PINVOKE_IMPL) != 0 &&
	   (pinvoke = ILPInvokeFindField(field)) != 0)
	{
		/* Field that is imported via PInvoke */
		_ILJitStackItemNew(stackItem);
		ILJitValue ptr = _ILJitGetPInvokeFieldAddress(jitCoder->jitFunction,
													  field,
													  pinvoke);

		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"LoadStaticPinvokeField: %s.%s\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field));
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
		_ILJitStackItemInitWithValue(stackItem, ptr);
		value = _ILJitLoadField(jitCoder, &stackItem, fieldType, 0, 0);
		_ILJitStackPushValue(jitCoder, value);
	}
	else
#endif
	if((field->member.attributes & IL_META_FIELDDEF_HAS_FIELD_RVA) == 0)
	{
		/* Queue the cctor to run. */
		ILCCtorMgr_OnStaticFieldAccess(&(jitCoder->cctorMgr), field);	

		/* Regular or thread-static field? */
		if(!ILFieldIsThreadStatic(field))
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue ptr = _ILJitGetClassStaticArea(jitCoder->jitFunction,
													  field);

		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"LoadStaticField: %s.%s at offset %i\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field),
					field->offset);
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			_ILJitStackItemInitWithValue(stackItem, ptr);
			value = _ILJitLoadField(jitCoder, &stackItem, fieldType, field->offset, 0);
			_ILJitStackPushValue(jitCoder, value);
		}
		else
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue ptr = _ILJitGetThreadStaticSlot(jitCoder, field);

		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"LoadThreadStaticField: %s.%s\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field));
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			_ILJitStackItemInitWithValue(stackItem, ptr);
			value = _ILJitLoadField(jitCoder, &stackItem, fieldType, 0, 0);
			_ILJitStackPushValue(jitCoder, value);
		}
	}
	else
	{
		_ILJitStackItemNew(stackItem);
		ILJitValue ptr = _ILJitGetRVAAddress(jitCoder->jitFunction, field);

	#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
		if (jitCoder->flags & IL_CODER_FLAG_STATS)
		{
			ILMutexLock(globalTraceMutex);
			fprintf(stdout,
					"LoadRVAStaticField: %s.%s\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field));
			ILMutexUnlock(globalTraceMutex);
		}
	#endif
		_ILJitStackItemInitWithValue(stackItem, ptr);
		value = _ILJitLoadField(jitCoder, &stackItem, fieldType, 0, 0);
		_ILJitStackPushValue(jitCoder, value);
	}
}

static void JITCoder_LoadFieldAddr(ILCoder *coder, ILEngineType ptrType,
							       ILType *objectType, ILField *field,
							       ILType *fieldType)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(address);
	_ILJitStackItemNew(ptr);
	
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"LoadFieldAddr: %s.%s at offset %i\n", 
			ILClass_Name(ILField_Owner(field)),
			ILField_Name(field),
			field->offset);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	_ILJitStackPop(jitCoder, ptr);
	_ILJitLoadFieldAddress(jitCoder,
						   &address,	
						   &ptr,
						   field->offset, 1);
	_ILJitStackPush(jitCoder, address);
}

static void JITCoder_LoadStaticFieldAddr(ILCoder *coder, ILField *field,
					 ILType *fieldType)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(address);

#ifdef IL_CONFIG_PINVOKE
	ILPInvoke *pinvoke;
	if((field->member.attributes & IL_META_FIELDDEF_PINVOKE_IMPL) != 0 &&
	   (pinvoke = ILPInvokeFindField(field)) != 0)
	{
		/* Field that is imported via PInvoke */
		_ILJitStackItemNew(stackItem);
		ILJitValue ptr = _ILJitGetPInvokeFieldAddress(jitCoder->jitFunction,
													  field,
													  pinvoke);

	#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
		if (jitCoder->flags & IL_CODER_FLAG_STATS)
		{
			ILMutexLock(globalTraceMutex);
			fprintf(stdout,
					"LoadStaticPivokeFieldAddr: %s.%s\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field));
			ILMutexUnlock(globalTraceMutex);
		}
	#endif
		_ILJitStackItemInitWithNotNullValue(stackItem, ptr);
		_ILJitLoadFieldAddress(jitCoder,
							   &address,
							   &stackItem,
							   0,
							   0);
		_ILJitStackPush(jitCoder, address);
		return;
	}
#endif

	/* Queue the cctor to run. */
	ILCCtorMgr_OnStaticFieldAccess(&(jitCoder->cctorMgr), field);	

	/* Regular or RVA field? */
	if((field->member.attributes & IL_META_FIELDDEF_HAS_FIELD_RVA) == 0)
	{
		/* Regular or thread-static field? */
		if(!ILFieldIsThreadStatic(field))
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue ptr = _ILJitGetClassStaticArea(jitCoder->jitFunction,
													  field);

		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"LoadStaticFieldAddr: %s.%s at offset %i\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field),
					field->offset);
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			_ILJitStackItemInitWithNotNullValue(stackItem, ptr);
			_ILJitLoadFieldAddress(jitCoder,
								   &address,
								   &stackItem,
								   field->offset,
								   0);
			_ILJitStackPush(jitCoder, address);
		}
		else
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue ptr = _ILJitGetThreadStaticSlot(jitCoder, field);

		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"LoadThreadStaticFieldAddr: %s.%s\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field));
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			_ILJitStackItemInitWithNotNullValue(stackItem, ptr);
			_ILJitLoadFieldAddress(jitCoder,
								   &address,
								   &stackItem,
								   0,
								   0);
			_ILJitStackPush(jitCoder, address);
		}
	}
	else
	{
		_ILJitStackItemNew(stackItem);
		ILJitValue ptr = _ILJitGetRVAAddress(jitCoder->jitFunction, field);

	#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
		if (jitCoder->flags & IL_CODER_FLAG_STATS)
		{
			ILMutexLock(globalTraceMutex);
			fprintf(stdout,
					"LoadRVAStaticFieldAddr: %s.%s\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field));
			ILMutexUnlock(globalTraceMutex);
		}
	#endif
		_ILJitStackItemInitWithNotNullValue(stackItem, ptr);
		_ILJitLoadFieldAddress(jitCoder,
							   &address,
							   &stackItem,
							   0,
							   0);
		_ILJitStackPush(jitCoder, address);
	}
}

static void JITCoder_StoreField(ILCoder *coder, ILEngineType ptrType,
								ILType *objectType, ILField *field,
								ILType *fieldType, ILEngineType valueType,
								const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(value);
	_ILJitStackItemNew(ptr);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"StoreField: %s.%s at offset %i\n", 
			ILClass_Name(ILField_Owner(field)),
			ILField_Name(field),
			field->offset);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPop(jitCoder, value);
	_ILJitStackPop(jitCoder, ptr);
	_ILJitStoreField(jitCoder,
					 &ptr,
					 _ILJitStackItemValue(value),
					 fieldType, field->offset, 1);
}

static void JITCoder_StoreStaticField(ILCoder *coder, ILField *field,
									  ILType *fieldType,
									  ILEngineType valueType,
									  const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(stackItem);
#ifdef IL_CONFIG_PINVOKE
	ILPInvoke *pinvoke;
#endif

	/* Pop the value off the stack. */
	_ILJitStackPop(jitCoder, stackItem);

	/* Queue the cctor to run. */
	ILCCtorMgr_OnStaticFieldAccess(&(jitCoder->cctorMgr), field);	

#ifdef IL_CONFIG_PINVOKE
	if((field->member.attributes & IL_META_FIELDDEF_PINVOKE_IMPL) != 0 &&
	   (pinvoke = ILPInvokeFindField(field)) != 0)
	{
		/* Field that is imported via PInvoke */
		_ILJitStackItemNew(ptrItem);
		ILJitValue ptr = _ILJitGetPInvokeFieldAddress(jitCoder->jitFunction,
													  field,
													  pinvoke);

	#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
		if (jitCoder->flags & IL_CODER_FLAG_STATS)
		{
			ILMutexLock(globalTraceMutex);
			fprintf(stdout,
					"StoreStaticPinvokeField: %s.%s\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field));
			ILMutexUnlock(globalTraceMutex);
		}
	#endif
		_ILJitStackItemInitWithValue(ptrItem, ptr);
		_ILJitStoreField(jitCoder,
						 &ptrItem,
						 _ILJitStackItemValue(stackItem),
						 fieldType, 0, 0);
	}
	else
#endif
	if((field->member.attributes & IL_META_FIELDDEF_HAS_FIELD_RVA) == 0)
	{
		/* Regular or thread-static field? */
		if(!ILFieldIsThreadStatic(field))
		{
			_ILJitStackItemNew(ptrItem);
			ILJitValue ptr = _ILJitGetClassStaticArea(jitCoder->jitFunction,
													  field);

		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"StoreStaticField: %s.%s at offset %i\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field),
					field->offset);
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			_ILJitStackItemInitWithValue(ptrItem, ptr);
			_ILJitStoreField(jitCoder,
							 &ptrItem,
							 _ILJitStackItemValue(stackItem),
							 fieldType, field->offset, 1);
		}
		else
		{
			_ILJitStackItemNew(ptrItem);
			ILJitValue ptr = _ILJitGetThreadStaticSlot(jitCoder, field);

		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"StoreThreadStaticField: %s.%s\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field));
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			_ILJitStackItemInitWithValue(ptrItem, ptr);
			_ILJitStoreField(jitCoder,
							 &ptrItem,
							 _ILJitStackItemValue(stackItem),
							 fieldType, 0, 0);
		}
	}
	else
	{
		_ILJitStackItemNew(ptrItem);
		ILJitValue ptr = _ILJitGetRVAAddress(jitCoder->jitFunction, field);

		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"StoreRVAStaticField: %s.%s\n", 
					ILClass_Name(ILField_Owner(field)),
					ILField_Name(field));
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			_ILJitStackItemInitWithValue(ptrItem, ptr);
			_ILJitStoreField(jitCoder,
							 &ptrItem,
							 _ILJitStackItemValue(stackItem),
							 fieldType, 0, 0);
	}
}

static void JITCoder_CopyObject(ILCoder *coder, ILEngineType destPtrType,
							    ILEngineType srcPtrType, ILClass *classInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILType *type = ILClassToType(classInfo);
    ILUInt32 size = _ILSizeOfTypeLocked(jitCoder->process, type);
	ILJitValue  memSize = jit_value_create_nint_constant(jitCoder->jitFunction,
														 _IL_JIT_TYPE_UINT32,
														 (jit_nint)size);
	_ILJitStackItemNew(src);
	_ILJitStackItemNew(dest);

	_ILJitStackPop(jitCoder, src);
	_ILJitStackPop(jitCoder, dest);
	/*
	 * Do the verification early.
	 */
	_ILJitStackItemCheckNull(jitCoder, dest);
	_ILJitStackItemCheckNull(jitCoder, src);

	_ILJitStackItemMemCpy(jitCoder,
						  dest,
						  _ILJitStackItemValue(src),
						  memSize);
}

static void JITCoder_CopyBlock(ILCoder *coder, ILEngineType destPtrType,
							   ILEngineType srcPtrType,
							   const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(size);
	_ILJitStackItemNew(src);
	_ILJitStackItemNew(dest);

	_ILJitStackPop(jitCoder, size);
	_ILJitStackPop(jitCoder, src);
	_ILJitStackPop(jitCoder, dest);
	/*
	 * Do the verification early.
	 */
	_ILJitStackItemCheckNull(jitCoder, dest);
	_ILJitStackItemCheckNull(jitCoder, src);

	_ILJitStackItemMemCpy(jitCoder,
						  dest,
						  _ILJitStackItemValue(src),
						  _ILJitStackItemValue(size));
}

static void JITCoder_InitObject(ILCoder *coder, ILEngineType ptrType,
								ILClass *classInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILType *type = ILClassToType(classInfo);
    ILUInt32 size = _ILSizeOfTypeLocked(jitCoder->process, type);
	ILJitValue  value = jit_value_create_nint_constant(jitCoder->jitFunction,
													   _IL_JIT_TYPE_BYTE,
													   (jit_nint)0);
	ILJitValue  memSize = jit_value_create_nint_constant(jitCoder->jitFunction,
														 _IL_JIT_TYPE_UINT32,
														 (jit_nint)size);
	_ILJitStackItemNew(dest);

	_ILJitStackPop(jitCoder, dest);
	/*
	 * Do the verification early.
	 */
	_ILJitStackItemCheckNull(jitCoder, dest);

	_ILJitStackItemMemSet(jitCoder,
						  dest,
						  value,
						  memSize);
}

static void JITCoder_InitBlock(ILCoder *coder, ILEngineType ptrType,
							   const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(size);
	_ILJitStackItemNew(value);
	_ILJitStackItemNew(dest);
	ILJitValue filler;

	_ILJitStackPop(jitCoder, size);
	_ILJitStackPop(jitCoder, value);
	_ILJitStackPop(jitCoder, dest);
	/*
	 * Do the verification early.
	 */
	_ILJitStackItemCheckNull(jitCoder, dest);

	filler = _ILJitValueConvertImplicit(jitCoder->jitFunction,
									   _ILJitStackItemValue(value),
									   _IL_JIT_TYPE_BYTE);
	_ILJitStackItemMemSet(jitCoder,
						  dest, 
						  filler,
						  _ILJitStackItemValue(size));
}

static void JITCoder_Box(ILCoder *coder, ILClass *boxClass,
					     ILEngineType valueType, ILUInt32 size)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(stackItem);
	ILJitValue value = 0;
	ILJitValue newObj;
	ILType *type = ILClassToType(boxClass);
	ILJitType jitType = _ILJitGetReturnType(type, jitCoder->process);

	_ILJitStackPop(jitCoder, stackItem);
	if(valueType == ILEngineType_TypedRef)
	{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
		if (jitCoder->flags & IL_CODER_FLAG_STATS)
		{
			ILMutexLock(globalTraceMutex);
			fprintf(stdout,
				"BoxTypedRef: %s Size: %i\n", 
				ILClass_Name(boxClass),
				size);
			ILMutexUnlock(globalTraceMutex);
		}
#endif
		/* We have to unpack the value first. */	
		ILJitValue ptr = _ILJitGetValFromRef(jitCoder,
											 _ILJitStackItemValue(stackItem),
											 boxClass);
		value = jit_insn_load_relative(jitCoder->jitFunction, ptr, 0, jitType);
	}
	else
	{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
		if (jitCoder->flags & IL_CODER_FLAG_STATS)
		{
			ILMutexLock(globalTraceMutex);
			fprintf(stdout,
				"Box: %s Size: %i\n", 
				ILClass_Name(boxClass),
				size);
			ILMutexUnlock(globalTraceMutex);
		}
#endif
		value = _ILJitStackItemValue(stackItem);
	}

	/* Allocate the object. */
	newObj = _ILJitAllocObjectGen(jitCoder->jitFunction, boxClass);

	if(jit_value_get_type(value) != jitType)
	{
		value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
										   value,
										   jitType);
	}
	/* Box a managed value */
	jit_insn_store_relative(jitCoder->jitFunction, newObj, 0, value);

	/* and push the boxed object onto the stack. */
	_ILJitStackPushNotNullValue(jitCoder, newObj);
}

static void JITCoder_BoxPtr(ILCoder *coder, ILClass *boxClass,
							ILUInt32 size, ILUInt32 pos)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitStackItem *stackItem;
	ILJitValue ptr;
	ILJitValue newObj;
	ILJitValue jitSize = jit_value_create_nint_constant(jitCoder->jitFunction,
														_IL_JIT_TYPE_NINT,
														(jit_nint)size);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
				"BoxPtr: %s Size: %i\n", 
				ILClass_Name(boxClass),
				size);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	stackItem = _ILJitStackItemGetTop(jitCoder, pos);
	ptr = _ILJitStackItemValue(*stackItem);

	/* Allocate the object. */
	newObj = _ILJitAllocObjectGen(jitCoder->jitFunction, boxClass);

	/* replace the pointer on the stack with the boxed value. */
	_ILJitStackItemInitWithNotNullValue(*stackItem, newObj);

	/* and copy the value to the boxed representation */
	_ILJitStackItemMemCpy(jitCoder, *stackItem, ptr, jitSize);
}

static void JITCoder_BoxSmaller(ILCoder *coder, ILClass *boxClass,
					   		    ILEngineType valueType, ILType *smallerType)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitType jitType = _ILJitGetReturnType(smallerType, jitCoder->process);
	_ILJitStackItemNew(stackItem);
	ILJitValue value;
	ILJitValue newObj;
	ILJitType jitValueType;

	/* Pop the value off the stack. */
	_ILJitStackPop(jitCoder, stackItem);

	/* Get the type of the popped object. */
	jitValueType = jit_value_get_type(_ILJitStackItemValue(stackItem));

	/* Allocate memory */
	newObj = _ILJitAllocObjectGen(jitCoder->jitFunction,
								  boxClass);
	
	/* If the smallerType is smaller then the initiale type then convert to it. */
	if(jitValueType != jitType)
	{
		int valueIsStruct = (jit_type_is_struct(jitValueType) || jit_type_is_union(jitValueType));
		int destIsStruct = (jit_type_is_struct(jitType) || jit_type_is_union(jitType));

		if(valueIsStruct || destIsStruct)
		{
			int valueSize = jit_type_get_size(jitValueType);
			int destSize = jit_type_get_size(jitType);

			if(destSize == valueSize)
			{
				/* The sizes match so we can safely use store relative. */
				value = _ILJitStackItemValue(stackItem);
			}
			else
			{
				/* We assume that destSize is smaller than valueSize because */
				/* the values have to be assignment compatible. */
				/* But we have to use memcpy instead. */
				ILJitValue srcPtr = jit_insn_address_of(jitCoder->jitFunction,
														_ILJitStackItemValue(stackItem));
				ILJitValue size = jit_value_create_nint_constant(jitCoder->jitFunction,
																 _IL_JIT_TYPE_NINT,
																 (jit_nint)destSize);

				_ILJitStackItemInitWithNotNullValue(stackItem, newObj);
				_ILJitStackItemMemCpy(jitCoder, stackItem, srcPtr, size);
				return;
			}
		}
		else
		{
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   jitType);
		}
	}
	else
	{
		value = _ILJitStackItemValue(stackItem);
	}
	/* Store the value in the boxed object. */
	jit_insn_store_relative(jitCoder->jitFunction, newObj, 0, value);

	/* and push the boxed object onto the stack. */
	_ILJitStackPushNotNullValue(jitCoder, newObj);
}

static void JITCoder_Unbox(ILCoder *coder, ILClass *boxClass,
						   const ILCoderPrefixInfo *prefixInfo)
{
	/* We don't have to do anything here: the object reference
	   points at the start of the object's fields, which is
	   exactly the pointer that we need for the unboxed value */
}

static void JITCoder_MakeTypedRef(ILCoder *coder, ILClass *classInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(stackItem);
	/* Create the structure */
	ILJitValue typedRef = jit_value_create(jitCoder->jitFunction,
										   _ILJitTypedRef);
	ILJitValue ptr = jit_insn_address_of(jitCoder->jitFunction,
										 typedRef);
	ILJitValue typeConst = jit_value_create_nint_constant(jitCoder->jitFunction,
														  _IL_JIT_TYPE_VPTR,
														  (jit_nint)classInfo);
	
	_ILJitStackPop(jitCoder, stackItem);
	jit_insn_store_relative(jitCoder->jitFunction, ptr,
							offsetof(ILTypedRef, type), typeConst);
				
	jit_insn_store_relative(jitCoder->jitFunction, ptr,
							offsetof(ILTypedRef, value),
							_ILJitStackItemValue(stackItem));

	_ILJitStackPushValue(jitCoder, typedRef);
}

static void JITCoder_RefAnyVal(ILCoder *coder, ILClass *classInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(typedRef);
	ILJitValue value;

	_ILJitStackPop(jitCoder, typedRef);
	value = _ILJitGetValFromRef(jitCoder,
								_ILJitStackItemValue(typedRef),
								classInfo);

	_ILJitStackPushValue(jitCoder, value);
}

static void JITCoder_RefAnyType(ILCoder *coder)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(typedRef);
	ILJitValue ptr;
	ILJitValue type;

	_ILJitStackPop(jitCoder, typedRef);
	ptr = jit_insn_address_of(jitCoder->jitFunction,
							  _ILJitStackItemValue(typedRef));
	type = jit_insn_load_relative(jitCoder->jitFunction,
								  ptr,
								  offsetof(ILTypedRef, type),
								  _IL_JIT_TYPE_VPTR);

	_ILJitStackPushValue(jitCoder, type);
}

static void JITCoder_PushToken(ILCoder *coder, ILProgramItem *item)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitValue value;

	value = jit_value_create_nint_constant(jitCoder->jitFunction,
										   _IL_JIT_TYPE_VPTR, 
							    		   (jit_nint)item);

	if(item)
	{
		_ILJitStackPushNotNullValue(jitCoder, value);
	}
	else
	{
		_ILJitStackPushValue(jitCoder, value);
	}
}

static void JITCoder_SizeOf(ILCoder *coder, ILType *type)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
    ILUInt32 size = _ILSizeOfTypeLocked(jitCoder->process, type);
	ILJitValue constSize = jit_value_create_nint_constant(jitCoder->jitFunction,
														  _IL_JIT_TYPE_INT32,
														  (jit_nint)size);
	_ILJitStackPushValue(jitCoder, constSize);
}

static void JITCoder_ArgList(ILCoder *coder)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitType signature = jit_function_get_signature(jitCoder->jitFunction);
	ILInt32 numArgs = jit_type_num_params(signature);
	_ILJitStackItemNew(stackItem);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
				"ArgList: Arg = %i\n",
				numArgs - 1);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	/* We have to push the last argument for this function on the stack. */
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	_ILJitStackItemLoadArg(jitCoder, stackItem, numArgs - 2);
#else
	_ILJitStackItemLoadArg(jitCoder, stackItem, numArgs - 1);
#endif
	_ILJitStackPush(jitCoder, stackItem);
}

#endif	/* IL_JITC_CODE */
