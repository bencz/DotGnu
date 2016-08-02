/*
 * cvmc_var.c - Coder implementation for CVM variables.
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

#ifdef IL_CVMC_CODE

/*
 * Load the address of a local variable onto the CVM stack.
 */
static void LoadLocalAddr(ILCoder *coder, ILUInt32 offset)
{
	CVM_OUT_WIDE(COP_WADDR, offset);
	CVM_ADJUST(1);
}

/*
 * Handle a load from an argument or local variable.
 */
static void LoadLocal(ILCoder *coder, ILUInt32 offset, ILType *type)
{
	if(ILType_IsPrimitive(type))
	{
		/* Primitive element type */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_I1:
			{
				LoadLocalAddr(coder, offset);
				CVM_OUT_NONE(COP_BREAD);
			}
			break;

			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_U1:
			{
				if(offset < 256)
				{
					CVM_OUT_BYTE(COP_BLOAD, offset);
				}
				else
				{
					LoadLocalAddr(coder, offset);
					CVM_OUT_NONE(COP_UBREAD);
				}
			}
			break;

			case IL_META_ELEMTYPE_I2:
			{
				LoadLocalAddr(coder, offset);
				CVM_OUT_NONE(COP_SREAD);
			}
			break;

			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			{
				LoadLocalAddr(coder, offset);
				CVM_OUT_NONE(COP_USREAD);
			}
			break;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
		#ifdef IL_NATIVE_INT32
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				if(offset < 4)
				{
					CVM_OUT_NONE(COP_ILOAD_0 + offset);
				}
				else
				{
					CVM_OUT_WIDE(COP_ILOAD, offset);
				}
				CVM_ADJUST(1);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
		#ifdef IL_NATIVE_INT64
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				CVM_OUT_DWIDE(COP_MLOAD, offset, CVM_WORDS_PER_LONG);
				CVM_ADJUST(CVM_WORDS_PER_LONG);
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				LoadLocalAddr(coder, offset);
				CVM_OUT_NONE(COP_FREAD);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - 1);
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				LoadLocalAddr(coder, offset);
				CVM_OUT_NONE(COP_DREAD);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - 1);
			}
			break;

			case IL_META_ELEMTYPE_R:
			{
				CVM_OUT_DWIDE(COP_MLOAD, offset, CVM_WORDS_PER_NATIVE_FLOAT);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT);
			}
			break;

			case IL_META_ELEMTYPE_TYPEDBYREF:
			{
				CVM_OUT_DWIDE(COP_MLOAD, offset, CVM_WORDS_PER_TYPED_REF);
				CVM_ADJUST(CVM_WORDS_PER_TYPED_REF);
			}
			break;
		}
	}
	else if(ILType_IsValueType(type))
	{
		ILType *enumType = ILTypeGetEnumType(type);
		if(ILType_IsValueType(enumType))
		{
			/* Managed value type */
			ILUInt32 size = GetTypeSize(_ILCoderToILCVMCoder(coder)->process,
										type);
			CVM_OUT_DWIDE(COP_MLOAD, offset, size);
			CVM_ADJUST(size);
		}
		else
		{
			/* Enumerated value type: load as the underlying value */
			LoadLocal(coder, offset, enumType);
		}
	}
	else
	{
		/* Everything else must be a pointer */
		if(offset < 4)
		{
			CVM_OUT_NONE(COP_PLOAD_0 + offset);
		}
		else
		{
			CVM_OUT_WIDE(COP_PLOAD, offset);
		}
		CVM_ADJUST(1);
	}
}

/*
 * Handle a store to an argument or local variable.
 */
static void StoreLocal(ILCoder *coder, ILUInt32 offset,
					   ILEngineType engineType, ILType *type)
{
	if(ILType_IsPrimitive(type))
	{
		/* Primitive element type */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			{
			#ifdef IL_NATIVE_INT64
				if(engineType == ILEngineType_I)
				{
					CVM_OUT_NONE(COP_L2I);
					CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
				}
			#endif
				if(offset < 256)
				{
					CVM_OUT_BYTE(COP_BSTORE, offset);
					CVM_ADJUST(-1);
				}
				else
				{
					LoadLocalAddr(coder, offset);
					CVM_OUT_NONE(COP_BWRITE_R);
					CVM_ADJUST(-2);
				}
			}
			break;

			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			{
			#ifdef IL_NATIVE_INT64
				if(engineType == ILEngineType_I)
				{
					CVM_OUT_NONE(COP_L2I);
					CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
				}
			#endif
				LoadLocalAddr(coder, offset);
				CVM_OUT_NONE(COP_SWRITE_R);
				CVM_ADJUST(-2);
			}
			break;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
		#ifdef IL_NATIVE_INT32
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
			#ifdef IL_NATIVE_INT64
				if(engineType == ILEngineType_I)
				{
					CVM_OUT_NONE(COP_L2I);
					CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
				}
			#endif
				if(offset < 4)
				{
					CVM_OUT_NONE(COP_ISTORE_0 + offset);
				}
				else
				{
					CVM_OUT_WIDE(COP_ISTORE, offset);
				}
				CVM_ADJUST(-1);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
		#ifdef IL_NATIVE_INT64
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				if(engineType == ILEngineType_I4)
				{
					CVM_OUT_NONE(COP_I2L);
					CVM_ADJUST(CVM_WORDS_PER_LONG - 1);
				}
				CVM_OUT_DWIDE(COP_MSTORE, offset, CVM_WORDS_PER_LONG);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				LoadLocalAddr(coder, offset);
				CVM_OUT_NONE(COP_FWRITE_R);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT + 1));
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				LoadLocalAddr(coder, offset);
				CVM_OUT_NONE(COP_DWRITE_R);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT + 1));
			}
			break;

			case IL_META_ELEMTYPE_R:
			{
				CVM_OUT_DWIDE(COP_MSTORE, offset, CVM_WORDS_PER_NATIVE_FLOAT);
				CVM_ADJUST(-CVM_WORDS_PER_NATIVE_FLOAT);
			}
			break;

			case IL_META_ELEMTYPE_TYPEDBYREF:
			{
				CVM_OUT_DWIDE(COP_MSTORE, offset, CVM_WORDS_PER_TYPED_REF);
				CVM_ADJUST(-CVM_WORDS_PER_TYPED_REF);
			}
			break;
		}
	}
	else if(ILType_IsValueType(type))
	{
		ILType *enumType = ILTypeGetEnumType(type);
		if(ILType_IsValueType(enumType))
		{
			/* Managed value type */
			ILUInt32 size = GetTypeSize(_ILCoderToILCVMCoder(coder)->process,
										type);
			CVM_OUT_DWIDE(COP_MSTORE, offset, size);
			CVM_ADJUST(-((ILInt32)size));
		}
		else
		{
			/* Enumerated value type: store as the underlying value */
			StoreLocal(coder, offset, engineType, enumType);
		}
	}
	else
	{
		/* Everything else must be a pointer */
		if(offset < 4)
		{
			CVM_OUT_NONE(COP_PSTORE_0 + offset);
		}
		else
		{
			CVM_OUT_WIDE(COP_PSTORE, offset);
		}
		CVM_ADJUST(-1);
	}
}

/*
 * Handle a load from an argument.
 */
static void CVMCoder_LoadArg(ILCoder *coder, ILUInt32 argNum, ILType *type)
{
	ILUInt32 offset = ((ILCVMCoder *)coder)->argOffsets[argNum];
	LoadLocal(coder, offset, type);
}

/*
 * Handle a load from a local variable.
 */
static void CVMCoder_LoadLocal(ILCoder *coder, ILUInt32 localNum, ILType *type)
{
	ILUInt32 offset = ((ILCVMCoder *)coder)->localOffsets[localNum];
	LoadLocal(coder, offset, type);
}

/*
 * Handle a store to an argument.
 */
static void CVMCoder_StoreArg(ILCoder *coder, ILUInt32 argNum,
							  ILEngineType engineType, ILType *type)
{
	ILUInt32 offset = ((ILCVMCoder *)coder)->argOffsets[argNum];
	StoreLocal(coder, offset, engineType, type);
}

/*
 * Handle a store to a local variable.
 */
static void CVMCoder_StoreLocal(ILCoder *coder, ILUInt32 localNum,
								ILEngineType engineType, ILType *type)
{
	ILUInt32 offset = ((ILCVMCoder *)coder)->localOffsets[localNum];
	StoreLocal(coder, offset, engineType, type);
}

/*
 * Load the address of an argument onto the stack.
 */
static void CVMCoder_AddrOfArg(ILCoder *coder, ILUInt32 argNum)
{
	ILUInt32 offset = ((ILCVMCoder *)coder)->argOffsets[argNum];
	LoadLocalAddr(coder, offset);
}

/*
 * Load the address of a local onto the stack.
 */
static void CVMCoder_AddrOfLocal(ILCoder *coder, ILUInt32 localNum)
{
	ILUInt32 offset = ((ILCVMCoder *)coder)->localOffsets[localNum];
	LoadLocalAddr(coder, offset);
}

/*
 * Allocate local stack space.
 */
static void CVMCoder_LocalAlloc(ILCoder *coder, ILEngineType sizeType)
{
#ifdef IL_NATIVE_INT64
	if(sizeType == ILEngineType_I)
	{
		CVM_OUT_NONE(COP_L2I);
		CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
	}
#endif
	CVMP_OUT_NONE(COP_PREFIX_LOCAL_ALLOC);
}

#endif	/* IL_CVMC_CODE */
