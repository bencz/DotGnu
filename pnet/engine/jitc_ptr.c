/*
 * jitc_ptr.c - Coder implementation for JIT pointers and arrays.
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
 * Handle the ldind* instructions.
 */
static void LoadRelative(ILJITCoder *coder, ILJitType type)
{
	_ILJitStackItemNew(ptr);
	ILJitValue value;

	_ILJitStackPop(coder, ptr);
	_ILJitStackItemCheckNull(coder, ptr);
	value = jit_insn_load_relative(coder->jitFunction,
								   _ILJitStackItemValue(ptr),
								   (jit_nint)0,
								   type);
	_ILJitStackPushValue(coder, value);
}

/*
 * Handle the stind* instructions.
 */
static void StoreRelative(ILJITCoder *coder, ILJitType type)
{
	_ILJitStackItemNew(value);
	_ILJitStackItemNew(ptr);
	ILJitType valueType;
	ILJitValue temp;

	_ILJitStackPop(coder, value);
	_ILJitStackPop(coder, ptr);
	valueType = jit_value_get_type(_ILJitStackItemValue(value));
	_ILJitStackItemCheckNull(coder, ptr);
	if(valueType != type)
	{
		int valueIsStruct = (jit_type_is_struct(valueType) || jit_type_is_union(valueType));
		int destIsStruct = (jit_type_is_struct(type) || jit_type_is_union(type));

		if(valueIsStruct || destIsStruct)
		{
			int valueSize = jit_type_get_size(valueType);
			int destSize = jit_type_get_size(type);

			if(destSize == valueSize)
			{
				/* The sizes match so we can safely use store relative. */
				temp = _ILJitStackItemValue(value);
			}
			else
			{
				/* We assume that destSize is smaller than valueSize because */
				/* the values have to be assignment compatible. */
				/* But we have to use memcpy instead. */
				ILJitValue srcPtr = jit_insn_address_of(coder->jitFunction,
														_ILJitStackItemValue(value));
				ILJitValue size = jit_value_create_nint_constant(coder->jitFunction,
																 _IL_JIT_TYPE_NINT,
																 (jit_nint)destSize);

				_ILJitStackItemMemCpy(coder, ptr, srcPtr, size);
				return;
			}
		}
		else
		{
			temp = _ILJitValueConvertImplicit(coder->jitFunction,
											  _ILJitStackItemValue(value),
											  type);
		}
	}
	else
	{
		temp = _ILJitStackItemValue(value);
	}
	_ILJitStackItemStoreRelative(coder, ptr, 0, temp);
}

/*
 * Handle a pointer indirection opcode.
 */
static void JITCoder_PtrAccess(ILCoder *coder, int opcode,
							   const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	switch(opcode)
	{
		case IL_OP_LDIND_I1:
		{
			/* Load a signed byte from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_SBYTE);
		}
		break;

		case IL_OP_LDIND_U1:
		{
			/* Load an unsigned byte from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_BYTE);
		}
		break;

		case IL_OP_LDIND_I2:
		{
			/* Load a signed short from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_INT16);
		}
		break;

		case IL_OP_LDIND_U2:
		{
			/* Load an unsigned short from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_UINT16);
		}
		break;

		case IL_OP_LDIND_I4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_LDIND_I:
	#endif
		{
			/* Load an integer from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_INT32);
		}
		break;

		case IL_OP_LDIND_U4:
		{
			/* Load an unsigned integer from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_UINT32);
		}
		break;

		case IL_OP_LDIND_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_LDIND_I:
	#endif
		{
			/* Load a long from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_INT64);
		}
		break;

		case IL_OP_LDIND_R4:
		{
			/* Load a 32-bit float from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_SINGLE);
		}
		break;

		case IL_OP_LDIND_R8:
		{
			/* Load a 64-bit float from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_DOUBLE);
		}
		break;

		case IL_OP_LDIND_REF:
		{
			/* Load a pointer from a pointer */
			LoadRelative(jitCoder, _IL_JIT_TYPE_VPTR);
		}
		break;

		case IL_OP_STIND_REF:
		{
			/* Store a pointer to a pointer */
			StoreRelative(jitCoder, _IL_JIT_TYPE_VPTR);
		}
		break;

		case IL_OP_STIND_I1:
		{
			/* Store a byte to a pointer */
			StoreRelative(jitCoder, _IL_JIT_TYPE_SBYTE);
		}
		break;

		case IL_OP_STIND_I2:
		{
			/* Store a short to a pointer */
			StoreRelative(jitCoder, _IL_JIT_TYPE_INT16);
		}
		break;

		case IL_OP_STIND_I4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_STIND_I:
	#endif
		{
			/* Store an integer to a pointer */
			StoreRelative(jitCoder, _IL_JIT_TYPE_INT32);
		}
		break;

		case IL_OP_STIND_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_STIND_I:
	#endif
		{
			/* Store a long to a pointer */
			StoreRelative(jitCoder, _IL_JIT_TYPE_INT64);
		}
		break;

		case IL_OP_STIND_R4:
		{
			/* Store a 32-bit float to a pointer */
			StoreRelative(jitCoder, _IL_JIT_TYPE_SINGLE);
		}
		break;

		case IL_OP_STIND_R8:
		{
			/* Store a 64-bit float to a pointer */
			StoreRelative(jitCoder, _IL_JIT_TYPE_DOUBLE);
		}
		break;
	}
}

/*
 * Handle a pointer indirection opcode.
 */
static void JITCoder_PtrDeref(ILCoder *coder, int pos)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitStackItem *stackItem;
	ILJitValue obj;

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
				"PtrDeref: %i\n",
				pos);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	stackItem = _ILJitStackItemGetTop(jitCoder, pos);

	/* Do a check for Null even if this shouldn't be needed here. */
	_ILJitStackItemCheckNull(jitCoder, *stackItem);

	/* Dereference the pointer */
	obj = jit_insn_load_relative(jitCoder->jitFunction,
								 _ILJitStackItemValue(*stackItem),
								 (jit_nint)0,
								 _IL_JIT_TYPE_VPTR);

	/* And replace the pointer with the dereferenced object reference. */
	_ILJitStackItemInitWithValue(*stackItem, obj);
}

/*
 * Handle a pointer indirection opcode for a managed value.
 */
static void JITCoder_PtrAccessManaged(ILCoder *coder, int opcode,
									  ILClass *classInfo,
									  const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILType *type = ILClassToType(classInfo);
	ILJitType jitType = _ILJitGetReturnType(type, jitCoder->process);
	_ILJitStackItemNew(address);
	_ILJitStackItemNew(object);
	ILJitValue value;

	if(opcode == IL_OP_LDOBJ)
	{
		/* Load from a pointer */
		/* Pop the address off the evaluation stack. */
		_ILJitStackPop(jitCoder, address);
		_ILJitStackItemCheckNull(jitCoder, address);
		value = jit_insn_load_relative(jitCoder->jitFunction,
									   _ILJitStackItemValue(address),
									   (jit_nint)0,
									   jitType);
		_ILJitStackPushValue(jitCoder, value);
	}
	else
	{
		/* Store to a pointer */
		ILJitType valueType;
		/* Pop the object off the evaluation stack. */
		_ILJitStackPop(jitCoder, object);
		/* Pop the address off the evaluation stack. */
		_ILJitStackPop(jitCoder, address);
		valueType = jit_value_get_type(_ILJitStackItemValue(object));
		_ILJitStackItemCheckNull(jitCoder, address);
		
		if(valueType != jitType)
		{
			int valueIsStruct = (jit_type_is_struct(valueType) || jit_type_is_union(valueType));
			int destIsStruct = (jit_type_is_struct(jitType) || jit_type_is_union(jitType));

			if(valueIsStruct || destIsStruct)
			{
				int valueSize = jit_type_get_size(valueType);
				int destSize = jit_type_get_size(jitType);

				if(destSize == valueSize)
				{
					/* The sizes match so we can safely use store relative. */
					value = _ILJitStackItemValue(object);
				}
				else
				{
					/* We assume that destSize is smaller than valueSize because */
					/* the values have to be assignment compatible. */
					/* But we have to use memcpy instead. */
					ILJitValue srcPtr = jit_insn_address_of(jitCoder->jitFunction,
															_ILJitStackItemValue(object));
					ILJitValue size = jit_value_create_nint_constant(jitCoder->jitFunction,
																	 _IL_JIT_TYPE_NINT,
																	 (jit_nint)destSize);

					_ILJitStackItemMemCpy(jitCoder, address, srcPtr, size);
					return;
				}
			}
			else
			{
				value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
													_ILJitStackItemValue(object),
													jitType);
			}
		}
		else
		{
			value = _ILJitStackItemValue(object);
		}
		_ILJitStackItemStoreRelative(jitCoder, address, 0, value);
	}
}

/*
 * Check the top of stack value for NULL.
 */
static void JITCoder_CheckNull(ILCoder *coder)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(value);

	_ILJitStackGetTop(jitCoder, value);
	_ILJitStackItemCheckNull(jitCoder, value);
}

#endif	/* IL_JITC_CODE */
