/*
 * cvmc_obj.c - Coder implementation for CVM object operations.
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

static void CVMCoder_CastClass(ILCoder *coder, ILClass *classInfo,
							   int throwException,
							   const ILCoderPrefixInfo *prefixInfo)
{
	if(ILClass_IsInterface(classInfo))
	{
		/* We are casting to an interface */
		if(throwException)
		{
			CVM_OUT_PTR(COP_CASTINTERFACE, classInfo);
		}
		else
		{
			CVM_OUT_PTR(COP_ISINTERFACE, classInfo);
		}
	}
	else
	{
		/* We are casting to a class */
		if(throwException)
		{
			CVM_OUT_PTR(COP_CASTCLASS, classInfo);
		}
		else
		{
			CVM_OUT_PTR(COP_ISINST, classInfo);
		}
	}
}

/*
 * Load a simple field relative to a pointer.
 */
static void CVMLoadSimpleField(ILCoder *coder, ILUInt32 offset,
							   int fieldOpcode, int ptrOpcode,
							   int mayBeNull)
{
	if(mayBeNull)
	{
		/* The pointer may be null, so we must check it first */
		if(offset < 256 && fieldOpcode != 0)
		{
			CVM_OUT_BYTE(fieldOpcode, offset);
		}
		else if(offset < 256)
		{
			CVM_OUT_NONE(COP_CKNULL);
			if(offset != 0)
			{
				CVM_OUT_BYTE(COP_PADD_OFFSET, offset);
			}
			CVM_OUT_NONE(ptrOpcode);
		}
		else
		{
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_WORD(COP_LDC_I4, offset);
			CVM_ADJUST(1);
			CVM_OUT_NONE(COP_PADD_I4);
			CVM_ADJUST(-1);
			CVM_OUT_NONE(ptrOpcode);
		}
	}
	else
	{
		/* We can guarantee that the pointer won't be null */
		if(offset < 256)
		{
			if(offset != 0)
			{
				CVM_OUT_BYTE(COP_PADD_OFFSET, offset);
			}
		}
		else
		{
			CVM_OUT_WORD(COP_LDC_I4, offset);
			CVM_ADJUST(1);
			CVM_OUT_NONE(COP_PADD_I4);
			CVM_ADJUST(-1);
		}
		CVM_OUT_NONE(ptrOpcode);
	}
}

/*
 * Load a managed value field relative to a pointer.
 */
static void CVMLoadValueField(ILCoder *coder, ILUInt32 offset,
							  ILUInt32 size, int mayBeNull)
{
	if(mayBeNull)
	{
		/* The pointer may be null, so we must check it first */
		CVM_OUT_NONE(COP_CKNULL);
	}
	if(offset < 256)
	{
		if(offset != 0)
		{
			CVM_OUT_BYTE(COP_PADD_OFFSET, offset);
		}
	}
	else
	{
		CVM_OUT_WORD(COP_LDC_I4, offset);
		CVM_ADJUST(1);
		CVM_OUT_NONE(COP_PADD_I4);
		CVM_ADJUST(-1);
	}
	CVM_OUT_WIDE(COP_MREAD, size);
	CVM_ADJUST(-1);
	CVM_ADJUST((size + sizeof(CVMWord) - 1) / sizeof(CVMWord));
}

/*
 * Load the contents of a field, relative to a pointer.
 */
static void CVMLoadField(ILCoder *coder, ILEngineType ptrType,
						 ILType *objectType, ILField *field,
						 ILType *fieldType, ILUInt32 offset,
						 int mayBeNull)
{
	ILUInt32 fieldSize;
	ILUInt32 ptrSize;

	/* Convert the type into its non-enumerated form */
	fieldType = ILTypeGetEnumType(fieldType);

	/* If "ptrType" is MV, then we need to get a pointer
	   to the start of the managed value on the stack */
	if(ptrType == ILEngineType_MV)
	{
		ptrSize = GetTypeSize(_ILCoderToILCVMCoder(coder)->process, objectType);
		CVM_OUT_WIDE(COP_MADDR, ptrSize);
	}
	else
	{
		ptrSize = 0;
	}

	/* Access the field relative to the pointer */
	if(ILType_IsPrimitive(fieldType))
	{
		/* Access a primitive type */
		switch(ILType_ToElement(fieldType))
		{
			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_I1:
			{
				CVMLoadSimpleField(coder, offset, COP_BREAD_FIELD,
								   COP_BREAD, mayBeNull);
			}
			break;

			case IL_META_ELEMTYPE_U1:
			{
				CVMLoadSimpleField(coder, offset, COP_UBREAD_FIELD,
								   COP_UBREAD, mayBeNull);
			}
			break;

			case IL_META_ELEMTYPE_I2:
			{
				CVMLoadSimpleField(coder, offset, COP_SREAD_FIELD,
								   COP_SREAD, mayBeNull);
			}
			break;

			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			{
				CVMLoadSimpleField(coder, offset, COP_USREAD_FIELD,
								   COP_USREAD, mayBeNull);
			}
			break;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
		#ifdef IL_NATIVE_INT32
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				CVMLoadSimpleField(coder, offset, COP_IREAD_FIELD,
								   COP_IREAD, mayBeNull);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
		#ifdef IL_NATIVE_INT64
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				CVMLoadValueField(coder, offset, sizeof(ILInt64), mayBeNull);
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				CVMLoadSimpleField(coder, offset, 0, COP_FREAD, mayBeNull);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - 1);
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				CVMLoadSimpleField(coder, offset, 0, COP_DREAD, mayBeNull);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - 1);
			}
			break;

			case IL_META_ELEMTYPE_R:
			{
				CVMLoadValueField(coder, offset,
								  sizeof(ILNativeFloat), mayBeNull);
			}
			break;

			case IL_META_ELEMTYPE_TYPEDBYREF:
			{
				CVMLoadValueField(coder, offset,
								  sizeof(ILTypedRef), mayBeNull);
			}
			break;
		}
	}
	else if(ILType_IsValueType(fieldType))
	{
		/* Access a value type */
		CVMLoadValueField(coder, offset,
						  _ILSizeOfTypeLocked(((ILCVMCoder *)coder)->process,
												fieldType), mayBeNull);
	}
	else
	{
		/* Access a reference type */
		CVMLoadSimpleField(coder, offset, COP_PREAD_FIELD,
						   COP_PREAD, mayBeNull);
	}

	/* If "ptrType" is MV, then we need to squash the stack
	   to remove the managed value */
	if(ptrType == ILEngineType_MV)
	{
		fieldSize = GetStackTypeSize(_ILCoderToILCVMCoder(coder)->process,
									 fieldType);
		CVM_OUT_DWIDE(COP_SQUASH, fieldSize, ptrSize);
		CVM_ADJUST(-((ILInt32)ptrSize));
	}
}

static void CVMCoder_LoadField(ILCoder *coder, ILEngineType ptrType,
							   ILType *objectType, ILField *field,
							   ILType *fieldType,
							   const ILCoderPrefixInfo *prefixInfo)
{
#ifdef IL_NATIVE_INT64
	/* Convert I4 to I if necessary */
	if(ptrType == ILEngineType_I4)
	{
		CVM_OUT_NONE(COP_IU2L);
	}
#endif
	CVMLoadField(coder, ptrType, objectType, field,
				 fieldType, field->offset, 1);
}

static void CVMCoder_LoadThisField(ILCoder *coder, ILField *field,
								   ILType *fieldType,
								   const ILCoderPrefixInfo *prefixInfo)
{
	ILType *enumType;

	/* Determine if the field type and offset are suitable for
	   use with the optimised "*read_this" instructions */
	enumType = ILTypeGetEnumType(fieldType);
	if(enumType == ILType_Int32 || enumType == ILType_UInt32)
	{
		if(field->offset < 256)
		{
			CVM_OUT_BYTE(COP_IREAD_THIS, field->offset);
			CVM_ADJUST(1);
			return;
		}
	}
	else if(ILType_IsClass(enumType) || ILType_IsArray(enumType))
	{
		if(field->offset < 256)
		{
			CVM_OUT_BYTE(COP_PREAD_THIS, field->offset);
			CVM_ADJUST(1);
			return;
		}
	}

	/* Fall back to the normal code because we cannot optimise this case */
	CVM_OUT_NONE(COP_PLOAD_0);
	CVM_ADJUST(1);
	CVMLoadField(coder, ILEngineType_O, ILType_Invalid, field,
				 fieldType, field->offset, 1);
}

#ifdef IL_CONFIG_PINVOKE

/*
 * Load the address of a PInvoke-imported field onto the stack.
 */
static void LoadPInvokeFieldAddress(ILCoder *coder, ILField *field,
									ILPInvoke *pinvoke)
{
	char *name;
	void *handle;
	void *symbol;
	const char *symbolName;

	/* Resolve the module */
	name = ILPInvokeResolveModule(pinvoke);
	if(!name)
	{
		CVM_OUT_NONE(COP_LDNULL);
		CVM_OUT_NONE(COP_CKNULL);
		CVM_ADJUST(1);
		return;
	}

	/* Load the module into memory */
	handle = ILDynLibraryOpen(name);
	ILFree(name);
	if(!handle)
	{
		CVM_OUT_NONE(COP_LDNULL);
		CVM_OUT_NONE(COP_CKNULL);
		CVM_ADJUST(1);
		return;
	}

	/* Resolve the symbol and push its address */
	symbolName = ILPInvoke_Alias(pinvoke);
	if(!symbolName)
	{
		symbolName = ILField_Name(field);
	}
	symbol = ILDynLibraryGetSymbol(handle, symbolName);
	CVM_OUT_PTR(COP_LDTOKEN, symbol);
	CVM_OUT_NONE(COP_CKNULL);
	CVM_ADJUST(1);
}

#endif /* IL_CONFIG_PINVOKE */

static void CVMCoder_LoadStaticField(ILCoder *coder, ILField *field,
									 ILType *fieldType,
									 const ILCoderPrefixInfo *prefixInfo)
{
	ILClass *classInfo;

#ifdef IL_CONFIG_PINVOKE
	ILPInvoke *pinvoke;
	if((field->member.attributes & IL_META_FIELDDEF_PINVOKE_IMPL) != 0 &&
	   (pinvoke = ILPInvokeFindField(field)) != 0)
	{
		/* Field that is imported via PInvoke */
		LoadPInvokeFieldAddress(coder, field, pinvoke);
		CVMLoadField(coder, ILEngineType_M, 0, field, fieldType, 0, 0);
	}
	else
#endif
	if((field->member.attributes & IL_META_FIELDDEF_HAS_FIELD_RVA) == 0)
	{
		classInfo = ILField_Owner(field);

		/* Queue the cctor to run. */
		ILCCtorMgr_OnStaticFieldAccess(&(((ILCVMCoder *)coder)->cctorMgr), field);

		/* Regular or thread-static field? */
		if(!ILFieldIsThreadStatic(field))
		{
			/* Push a pointer to the class's static data area */
			CVM_OUT_PTR(COP_GET_STATIC, classInfo);
			CVM_ADJUST(1);

			/* Load the field relative to the pointer */
			CVMLoadField(coder, ILEngineType_M, 0, field,
						 fieldType, field->offset, 0);
		}
		else
		{
			/* Extract the pointer from a thread-static data slot */
			CVMP_OUT_WORD2(COP_PREFIX_THREAD_STATIC,
						   field->offset, field->nativeOffset);
			CVM_ADJUST(1);

			/* Load the field relative to the pointer */
			CVMLoadField(coder, ILEngineType_M, 0, field, fieldType, 0, 0);
		}
	}
	else
	{
		/* RVA-based static field */
		ILFieldRVA *fieldRVA = ILFieldRVAGetFromOwner(field);
		ILUInt32 rva = ILFieldRVAGetRVA(fieldRVA);
		CVM_OUT_WORD(COP_LDRVA, rva);
		CVM_ADJUST(1);

		/* Load the field directly from the pointer */
		CVMLoadField(coder, ILEngineType_M, 0, field, fieldType, 0, 0);
	}
}

static void CVMCoder_LoadFieldAddr(ILCoder *coder, ILEngineType ptrType,
							       ILType *objectType, ILField *field,
							       ILType *fieldType)
{
#ifdef IL_NATIVE_INT64
	/* Convert I4 to I if necessary */
	if(ptrType == ILEngineType_I4)
	{
		CVM_OUT_NONE(COP_IU2L);
	}
#endif
	if(ptrType == ILEngineType_O)
	{
		CVM_OUT_NONE(COP_CKNULL);
	}
	if(field->offset < 256)
	{
		if(field->offset != 0)
		{
			CVM_OUT_BYTE(COP_PADD_OFFSET, field->offset);
		}
	}
	else
	{
		CVM_OUT_WORD(COP_LDC_I4, field->offset);
		CVM_ADJUST(1);
		CVM_OUT_NONE(COP_PADD_I4);
		CVM_ADJUST(-1);
	}
}

static void CVMCoder_LoadStaticFieldAddr(ILCoder *coder, ILField *field,
							             ILType *fieldType)
{
	ILClass *classInfo;

#ifdef IL_CONFIG_PINVOKE
	ILPInvoke *pinvoke;
	if((field->member.attributes & IL_META_FIELDDEF_PINVOKE_IMPL) != 0 &&
	   (pinvoke = ILPInvokeFindField(field)) != 0)
	{
		/* Field that is imported via PInvoke */
		LoadPInvokeFieldAddress(coder, field, pinvoke);
		return;
	}
#endif

	classInfo = ILField_Owner(field);

	/* Queue the cctor to run. */
	ILCCtorMgr_OnStaticFieldAccess(&(((ILCVMCoder *)coder)->cctorMgr), field);

	/* Regular or RVA field? */
	if((field->member.attributes & IL_META_FIELDDEF_HAS_FIELD_RVA) == 0)
	{
		/* Regular or thread-static field? */
		if(!ILFieldIsThreadStatic(field))
		{
			/* Push a pointer to the class's static data area */
			CVM_OUT_PTR(COP_GET_STATIC, classInfo);
			CVM_ADJUST(1);
		}
		else
		{
			/* Extract the pointer from a thread-static data slot */
			CVMP_OUT_WORD2(COP_PREFIX_THREAD_STATIC,
						   field->offset, field->nativeOffset);
			CVM_ADJUST(1);
			return;
		}

		/* Add the offset to the pointer */
		if(field->offset < 256)
		{
			if(field->offset != 0)
			{
				CVM_OUT_BYTE(COP_PADD_OFFSET, field->offset);
			}
		}
		else
		{
			CVM_OUT_WORD(COP_LDC_I4, field->offset);
			CVM_ADJUST(1);
			CVM_OUT_NONE(COP_PADD_I4);
			CVM_ADJUST(-1);
		}
	}
	else
	{
		/* RVA-based static field */
		ILFieldRVA *fieldRVA = ILFieldRVAGetFromOwner(field);
		ILUInt32 rva = ILFieldRVAGetRVA(fieldRVA);
		CVM_OUT_WORD(COP_LDRVA, rva);
		CVM_ADJUST(1);
	}
}

/*
 * Store into a field with reversed pointer and value arguments.
 */
static void CVMStoreFieldReverse(ILCoder *coder, ILField *field,
								 ILType *fieldType, ILUInt32 offset,
								 int mayBeNull)
{
	ILUInt32 size;

	/* Check for null and adjust the pointer for the offset */
	if(mayBeNull)
	{
		CVM_OUT_NONE(COP_CKNULL);
	}
	if(offset < 256)
	{
		if(offset != 0)
		{
			CVM_OUT_BYTE(COP_PADD_OFFSET, offset);
		}
	}
	else
	{
		CVM_OUT_WORD(COP_LDC_I4, offset);
		CVM_ADJUST(1);
		CVM_OUT_NONE(COP_PADD_I4);
		CVM_ADJUST(-1);
	}

	/* Convert the type into its non-enumerated form */
	fieldType = ILTypeGetEnumType(fieldType);

	/* Store the value into the pointer */
	if(ILType_IsPrimitive(fieldType))
	{
		/* Store a primitive value */
		switch(ILType_ToElement(fieldType))
		{
			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			{
				CVM_OUT_NONE(COP_BWRITE_R);
			}
			break;

			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			{
				CVM_OUT_NONE(COP_SWRITE_R);
			}
			break;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
		#ifdef IL_NATIVE_INT32
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				CVM_OUT_NONE(COP_IWRITE_R);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
		#ifdef IL_NATIVE_INT64
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				CVM_OUT_WIDE(COP_MWRITE_R, sizeof(ILInt64));
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				CVM_OUT_NONE(COP_FWRITE_R);
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				CVM_OUT_NONE(COP_DWRITE_R);
			}
			break;

			case IL_META_ELEMTYPE_R:
			{
				CVM_OUT_WIDE(COP_MWRITE_R, sizeof(ILNativeFloat));
			}
			break;

			case IL_META_ELEMTYPE_TYPEDBYREF:
			{
				CVM_OUT_WIDE(COP_MWRITE_R, sizeof(ILTypedRef));
			}
			break;
		}
	}
	else if(ILType_IsValueType(fieldType))
	{
		size = _ILSizeOfTypeLocked(_ILCoderToILCVMCoder(coder)->process,
								   fieldType);
		CVM_OUT_WIDE(COP_MWRITE_R, size);
	}
	else
	{
		CVM_OUT_NONE(COP_PWRITE_R);
	}
}

/*
 * Store into a field.
 */
static void CVMStoreField(ILCoder *coder, ILField *field,
						  ILType *fieldType, ILUInt32 offset,
						  ILUInt32 valueSize)
{
	ILUInt32 size;

	/* Convert the type into its non-enumerated form */
	fieldType = ILTypeGetEnumType(fieldType);

	/* Determine if we can use a direct field write instruction */
	if(offset < 256)
	{
		if(ILType_IsPrimitive(fieldType))
		{
			switch(ILType_ToElement(fieldType))
			{
				case IL_META_ELEMTYPE_BOOLEAN:
				case IL_META_ELEMTYPE_I1:
				case IL_META_ELEMTYPE_U1:
				{
					CVM_OUT_BYTE(COP_BWRITE_FIELD, offset);
					return;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I2:
				case IL_META_ELEMTYPE_U2:
				case IL_META_ELEMTYPE_CHAR:
				{
					CVM_OUT_BYTE(COP_SWRITE_FIELD, offset);
					return;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I4:
				case IL_META_ELEMTYPE_U4:
			#ifdef IL_NATIVE_INT32
				case IL_META_ELEMTYPE_I:
				case IL_META_ELEMTYPE_U:
			#endif
				{
					CVM_OUT_BYTE(COP_IWRITE_FIELD, offset);
					return;
				}
				/* Not reached */

				default: break;
			}
		}
		else if(!ILType_IsValueType(fieldType))
		{
			/* Store a reference value */
			CVM_OUT_BYTE(COP_PWRITE_FIELD, offset);
			return;
		}
	}

	/* Check the pointer value for null */
	CVM_OUT_WIDE(COP_CKNULL_N, valueSize);

	/* Add the offset to the pointer value */
	if(offset != 0)
	{
		CVM_OUT_DWIDE(COP_PADD_OFFSET_N, valueSize, offset);
	}

	/* Perform the store */
	if(ILType_IsPrimitive(fieldType))
	{
		/* Store a primitive value */
		switch(ILType_ToElement(fieldType))
		{
			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			{
				CVM_OUT_NONE(COP_BWRITE);
			}
			break;

			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			{
				CVM_OUT_NONE(COP_SWRITE);
			}
			break;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
		#ifdef IL_NATIVE_INT32
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				CVM_OUT_NONE(COP_IWRITE);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
		#ifdef IL_NATIVE_INT64
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				CVM_OUT_WIDE(COP_MWRITE, sizeof(ILInt64));
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				CVM_OUT_NONE(COP_FWRITE);
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				CVM_OUT_NONE(COP_DWRITE);
			}
			break;

			case IL_META_ELEMTYPE_R:
			{
				CVM_OUT_WIDE(COP_MWRITE, sizeof(ILNativeFloat));
			}
			break;

			case IL_META_ELEMTYPE_TYPEDBYREF:
			{
				CVM_OUT_WIDE(COP_MWRITE, sizeof(ILTypedRef));
			}
			break;
		}
	}
	else if(ILType_IsValueType(fieldType))
	{
		/* Store a managed value */
		size = _ILSizeOfTypeLocked(_ILCoderToILCVMCoder(coder)->process,
								   fieldType);
		CVM_OUT_WIDE(COP_MWRITE, size);
	}
	else
	{
		/* Store a reference value */
		CVM_OUT_NONE(COP_PWRITE);
	}
}

static void CVMCoder_StoreField(ILCoder *coder, ILEngineType ptrType,
								ILType *objectType, ILField *field,
								ILType *fieldType, ILEngineType valueType,
								const ILCoderPrefixInfo *prefixInfo)
{
	ILUInt32 valueSize = GetStackTypeSize(((ILCVMCoder *)coder)->process, fieldType);

#ifdef IL_NATIVE_INT64
	/* Convert I4 to I if necessary */
	if(ptrType == ILEngineType_I4)
	{
		CVM_OUT_WIDE(COP_I2P_LOWER, valueSize);
	}
#endif

	/* Store into the field */
	CVMStoreField(coder, field, fieldType, field->offset, valueSize);

	/* Adjust the stack to compensate for the store */
	CVM_ADJUST(-((ILInt32)(valueSize + 1)));
}

static void CVMCoder_StoreStaticField(ILCoder *coder, ILField *field,
									  ILType *fieldType, ILEngineType valueType,
									  const ILCoderPrefixInfo *prefixInfo)
{
	ILUInt32 valueSize = GetStackTypeSize(_ILCoderToILCVMCoder(coder)->process,
										  fieldType);
	ILClass *classInfo;
#ifdef IL_CONFIG_PINVOKE
	ILPInvoke *pinvoke;
#endif

	classInfo = ILField_Owner(field);

	/* Queue the cctor to run. */
	ILCCtorMgr_OnStaticFieldAccess(&(((ILCVMCoder *)coder)->cctorMgr), field);

	/* Regular or RVA field? */
#ifdef IL_CONFIG_PINVOKE
	if((field->member.attributes & IL_META_FIELDDEF_PINVOKE_IMPL) != 0 &&
	   (pinvoke = ILPInvokeFindField(field)) != 0)
	{
		/* Field that is imported via PInvoke */
		LoadPInvokeFieldAddress(coder, field, pinvoke);
		CVMStoreFieldReverse(coder, field, fieldType, 0, 0);
	}
	else
#endif
	if((field->member.attributes & IL_META_FIELDDEF_HAS_FIELD_RVA) == 0)
	{
		/* Regular or thread-static field? */
		if(!ILFieldIsThreadStatic(field))
		{
			/* Push a pointer to the class's static data area */
			CVM_OUT_PTR(COP_GET_STATIC, classInfo);
			CVM_ADJUST(1);

			/* Store the field relative to the pointer */
			CVMStoreFieldReverse(coder, field, fieldType, field->offset, 0);
		}
		else
		{
			/* Extract the pointer from a thread-static data slot */
			CVMP_OUT_WORD2(COP_PREFIX_THREAD_STATIC,
						   field->offset, field->nativeOffset);
			CVM_ADJUST(1);

			/* Store the field relative to the pointer */
			CVMStoreFieldReverse(coder, field, fieldType, 0, 0);
		}
	}
	else
	{
		/* RVA-based static field */
		ILFieldRVA *fieldRVA = ILFieldRVAGetFromOwner(field);
		ILUInt32 rva = ILFieldRVAGetRVA(fieldRVA);
		CVM_OUT_WORD(COP_LDRVA, rva);
		CVM_ADJUST(1);

		/* Store the field directly to the pointer */
		CVMStoreFieldReverse(coder, field, fieldType, 0, 0);
	}

	/* Adjust the stack to compensate for the store */
	CVM_ADJUST(-((ILInt32)(valueSize + 1)));
}

static void CVMCoder_CopyObject(ILCoder *coder, ILEngineType destPtrType,
							    ILEngineType srcPtrType, ILClass *classInfo)
{
	ILUInt32 size;

#ifdef IL_NATIVE_INT64
	/* Normalize the pointers */
	if(destPtrType == ILEngineType_I4)
	{
		CVM_OUT_WIDE(COP_I2P_LOWER, 1);
	}
	if(srcPtrType == ILEngineType_I4)
	{
		CVM_OUT_WIDE(COP_I2P_LOWER, 0);
	}
#endif

	/* Check the values for null */
	if(destPtrType != ILEngineType_M && destPtrType != ILEngineType_T)
	{
		CVM_OUT_WIDE(COP_CKNULL_N, 1);
	}
	if(srcPtrType != ILEngineType_M && srcPtrType != ILEngineType_T)
	{
		CVM_OUT_NONE(COP_CKNULL);
	}

	/* Copy the memory block */
	size = _ILSizeOfTypeLocked(_ILCoderToILCVMCoder(coder)->process,
								ILType_FromValueType(classInfo));
	CVM_OUT_WIDE(COP_MEMCPY, size);
	CVM_ADJUST(-2);
}

static void CVMCoder_CopyBlock(ILCoder *coder, ILEngineType destPtrType,
							   ILEngineType srcPtrType,
							   const ILCoderPrefixInfo *prefixInfo)
{
#ifdef IL_NATIVE_INT64
	/* Normalize the pointers */
	if(destPtrType == ILEngineType_I4)
	{
		CVM_OUT_WIDE(COP_I2P_LOWER, 2);
	}
	if(srcPtrType == ILEngineType_I4)
	{
		CVM_OUT_WIDE(COP_I2P_LOWER, 1);
	}
#endif

	/* Check the values for null */
	if(destPtrType != ILEngineType_M && destPtrType != ILEngineType_T)
	{
		CVM_OUT_WIDE(COP_CKNULL_N, 2);
	}
	if(srcPtrType != ILEngineType_M && srcPtrType != ILEngineType_T)
	{
		CVM_OUT_WIDE(COP_CKNULL_N, 1);
	}

	/* Copy the memory block */
	CVM_OUT_NONE(COP_MEMMOVE);
	CVM_ADJUST(-3);
}

static void CVMCoder_InitObject(ILCoder *coder, ILEngineType ptrType,
							    ILClass *classInfo)
{
	ILUInt32 size;

#ifdef IL_NATIVE_INT64
	/* Normalize the pointer */
	if(ptrType == ILEngineType_I4)
	{
		CVM_OUT_WIDE(COP_I2P_LOWER, 0);
	}
#endif

	/* Check the pointer value for null */
	if(ptrType != ILEngineType_M && ptrType != ILEngineType_T)
	{
		CVM_OUT_NONE(COP_CKNULL);
	}

	/* Initialize the block to all-zeroes */
	size = _ILSizeOfTypeLocked(_ILCoderToILCVMCoder(coder)->process,
								ILType_FromValueType(classInfo));
	CVM_OUT_WIDE(COP_MEMZERO, size);
	CVM_ADJUST(-1);
}

static void CVMCoder_InitBlock(ILCoder *coder, ILEngineType ptrType,
							   const ILCoderPrefixInfo *prefixInfo)
{
#ifdef IL_NATIVE_INT64
	/* Normalize the pointer */
	if(ptrType == ILEngineType_I4)
	{
		CVM_OUT_WIDE(COP_I2P_LOWER, 2);
	}
#endif

	/* Check the pointer value for null */
	if(ptrType != ILEngineType_M && ptrType != ILEngineType_T)
	{
		CVM_OUT_WIDE(COP_CKNULL_N, 2);
	}

	/* Initialize the block to bytes of the same value */
	CVM_OUT_NONE(COP_MEMSET);
	CVM_ADJUST(-3);
}

static void CVMCoder_Box(ILCoder *coder, ILClass *boxClass,
						 ILEngineType valueType, ILUInt32 size)
{
	ILUInt32 sizeInWords;

	if(valueType != ILEngineType_TypedRef)
	{
		/* Box a managed value */
		sizeInWords = (size + sizeof(CVMWord) - 1) / sizeof(CVMWord);
		CVM_OUT_WIDE_PTR(COP_BOX, size, boxClass);
		CVM_ADJUST(-((ILInt32)sizeInWords));
		CVM_ADJUST(1);
	}
	else
	{
		/* Box a typed reference after we unpack it */
		CVMP_OUT_PTR(COP_PREFIX_REFANYVAL, boxClass);
		CVM_ADJUST(-(CVM_WORDS_PER_TYPED_REF - 1));
		CVM_OUT_WIDE_PTR(COP_BOX_PTR, size, boxClass);
	}
}

static void CVMCoder_BoxSmaller(ILCoder *coder, ILClass *boxClass,
								ILEngineType valueType, ILType *smallerType)
{
	/* Align the value on the proper stack word boundary and then box it */
	switch(ILType_ToElement(smallerType))
	{
		case IL_META_ELEMTYPE_I1:
		{
			CVMP_OUT_NONE(COP_PREFIX_I2B_ALIGNED);
			CVMCoder_Box(coder, boxClass, valueType, 1);
		}
		break;

		case IL_META_ELEMTYPE_I2:
		{
			CVMP_OUT_NONE(COP_PREFIX_I2S_ALIGNED);
			CVMCoder_Box(coder, boxClass, valueType, 2);
		}
		break;

		case IL_META_ELEMTYPE_R4:
		{
			CVMP_OUT_NONE(COP_PREFIX_F2F_ALIGNED);
			CVM_ADJUST(CVM_WORDS_PER_FLOAT - CVM_WORDS_PER_NATIVE_FLOAT);
			CVMCoder_Box(coder, boxClass, valueType, sizeof(ILFloat));
		}
		break;

		case IL_META_ELEMTYPE_R8:
		{
			CVMP_OUT_NONE(COP_PREFIX_F2D_ALIGNED);
			CVM_ADJUST(CVM_WORDS_PER_DOUBLE - CVM_WORDS_PER_NATIVE_FLOAT);
			CVMCoder_Box(coder, boxClass, valueType, sizeof(ILDouble));
		}
		break;
	}
}

static void CVMCoder_BoxPtr(ILCoder *coder, ILClass *boxClass,
							ILUInt32 size, ILUInt32 pos)
{
	if(pos == 0)
	{
		CVM_OUT_WIDE_PTR(COP_BOX_PTR, size, boxClass);
	}
	else
	{
		CVM_OUT_WIDE(COP_DUP_WORD_N, pos);
		CVM_OUT_WIDE_PTR(COP_BOX_PTR, size, boxClass);
		CVMP_OUT_WORD(COP_PREFIX_REPL_WORD_N, pos+1);		
	}
}

static void CVMCoder_Unbox(ILCoder *coder, ILClass *boxClass,
						   const ILCoderPrefixInfo *prefixInfo)
{
	/* We don't have to do anything here: the object reference
	   points at the start of the object's fields, which is
	   exactly the pointer that we need for the unboxed value */
}

static void CVMCoder_MakeTypedRef(ILCoder *coder, ILClass *classInfo)
{
	CVM_OUT_NONE(COP_CKNULL);
	CVMP_OUT_PTR(COP_PREFIX_MKREFANY, classInfo);
	CVM_ADJUST(CVM_WORDS_PER_TYPED_REF - 1);
}

static void CVMCoder_RefAnyVal(ILCoder *coder, ILClass *classInfo)
{
	CVMP_OUT_PTR(COP_PREFIX_REFANYVAL, classInfo);
	CVM_ADJUST(-(CVM_WORDS_PER_TYPED_REF - 1));
}

static void CVMCoder_RefAnyType(ILCoder *coder)
{
	CVMP_OUT_NONE(COP_PREFIX_REFANYTYPE);
	CVM_ADJUST(-(CVM_WORDS_PER_TYPED_REF - 1));
}

static void CVMCoder_PushToken(ILCoder *coder, ILProgramItem *item)
{
	CVM_OUT_PTR(COP_LDTOKEN, item);
	CVM_ADJUST(1);
}

static void CVMCoder_SizeOf(ILCoder *coder, ILType *type)
{
	ILUInt32 size = _ILSizeOfTypeLocked(_ILCoderToILCVMCoder(coder)->process,
										type);
	if(size <= 8)
	{
		CVM_OUT_NONE(COP_LDC_I4_0 + size);
	}
	else if(size < 128)
	{
		CVM_OUT_BYTE(COP_LDC_I4_S, size);
	}
	else
	{
		CVM_OUT_WORD(COP_LDC_I4, size);
	}
	CVM_ADJUST(1);
}

static void CVMCoder_ArgList(ILCoder *coder)
{
	/* Load the argument that contains the "Object[]" array with
	   the extra parameters that were passed to the method */
	ILUInt32 num = ((ILCVMCoder *)coder)->varargIndex;
	if(num < 4)
	{
		CVM_OUT_NONE(COP_PLOAD_0 + num);
	}
	else
	{
		CVM_OUT_WIDE(COP_PLOAD, num);
	}
}

#endif	/* IL_CVMC_CODE */
