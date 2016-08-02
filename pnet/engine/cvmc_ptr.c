/*
 * cvmc_ptr.c - Coder implementation for CVM pointers and arrays.
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
 * Load the address of an array element.
 */
static void GetArrayElementAddress(ILCoder *coder, ILEngineType indexType,
								   int size)
{
#ifdef IL_NATIVE_INT64
	if(indexType == ILEngineType_I4)
#endif
	{
		/* Check if size is a power of two */
		if((size > 0) && ((size & (size -1)) == 0))
		{
			int shift = 0;

			while((size & (1 << shift)) == 0)
			{
				++shift;
			}
			CVM_OUT_BYTE(COP_ELEM_ADDR_SHIFT_I4, shift);
			CVM_ADJUST(-1);
		}
		else
		{
			CVM_OUT_WORD(COP_ELEM_ADDR_MUL_I4, size);
			CVM_ADJUST(-1);
		}
	}
#ifdef IL_NATIVE_INT64
	else
	{
		CVM_OUT_NONE(COP_CKARRAY_LOAD_I8);
		if(size == 2)
		{
			CVM_OUT_NONE(COP_LDC_I4_1);
			CVM_ADJUST(1);
			CVM_OUT_NONE(COP_LSHL);
			CVM_ADJUST(-1);
		}
		else if(size == 4)
		{
			CVM_OUT_NONE(COP_LDC_I4_2);
			CVM_ADJUST(1);
			CVM_OUT_NONE(COP_LSHL);
			CVM_ADJUST(-1);
		}
		else if(size == 8)
		{
			CVM_OUT_NONE(COP_LDC_I4_3);
			CVM_ADJUST(1);
			CVM_OUT_NONE(COP_LSHL);
			CVM_ADJUST(-1);
		}
		else if(size != 1)
		{
			if(size < 8)
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
			CVM_OUT_NONE(COP_IU2L);
			CVM_ADJUST(CVM_WORDS_PER_LONG);
			CVM_OUT_NONE(COP_LMUL);
			CVM_ADJUST(-CVM_WORDS_PER_LONG);
		}
		CVM_OUT_NONE(COP_PADD_I8);
		CVM_ADJUST(-CVM_WORDS_PER_LONG);
	}
#endif
}

/*
 * Load elements from an array.
 */
static void LoadArrayElem(ILCoder *coder, int opcode1, int opcode2,
						  ILEngineType indexType, int shift, int stackSize)
{
#ifdef IL_NATIVE_INT64
	if(indexType == ILEngineType_I4)
#endif
	{
		if(opcode1 < COP_PREFIX)
		{
			CVM_OUT_NONE(opcode1);
		}
		else
		{
			CVMP_OUT_NONE(opcode1 - COP_PREFIX);
		}
		CVM_ADJUST(-2 + stackSize);
	}
#ifdef IL_NATIVE_INT64
	else
	{
		CVM_OUT_NONE(COP_CKARRAY_LOAD_I8);
		if(shift != 0)
		{
			CVM_OUT_NONE(COP_LDC_I4_0 + shift);
			CVM_ADJUST(1);
			CVM_OUT_NONE(COP_LSHL);
			CVM_ADJUST(-1);
		}
		CVM_OUT_NONE(COP_PADD_I8);
		if(opcode2 != COP_MREAD)
		{
			CVM_OUT_NONE(opcode2);
		}
		else
		{
			CVM_OUT_WIDE(opcode2, (1 << shift));
		}
		CVM_ADJUST(-(1 + CVM_WORDS_PER_LONG) + stackSize);
	}
#endif
}

/*
 * Store elements to an array.
 */
static void StoreArrayElem(ILCoder *coder, int opcode1, int opcode2,
						   ILEngineType indexType, int shift, int stackSize)
{
#ifdef IL_NATIVE_INT64
	if(indexType == ILEngineType_I4)
#endif
	{
		if(opcode1 < COP_PREFIX)
		{
			CVM_OUT_NONE(opcode1);
		}
		else
		{
			CVMP_OUT_NONE(opcode1 - COP_PREFIX);
		}
		CVM_ADJUST(-(2 + stackSize));
	}
#ifdef IL_NATIVE_INT64
	else
	{
		CVM_OUT_BYTE2(COP_CKARRAY_STORE_I8, stackSize, (1 << shift));
		if(opcode2 != COP_MWRITE)
		{
			CVM_OUT_NONE(opcode2);
		}
		else
		{
			CVM_OUT_WIDE(opcode2, (1 << shift));
		}
		CVM_ADJUST(-(1 + CVM_WORDS_PER_LONG + stackSize));
	}
#endif
}

/*
 * Handle an array access opcode.
 */
static void CVMCoder_ArrayAccess(ILCoder *coder, int opcode,
								 ILEngineType indexType, ILType *elemType,
								 const ILCoderPrefixInfo *prefixInfo)
{
	ILUInt32 size;

	switch(opcode)
	{
		case IL_OP_LDELEMA:
		{
			/* Load the address of an array element */
			size = _ILSizeOfTypeLocked(_ILCoderToILCVMCoder(coder)->process,
									   elemType);
			GetArrayElementAddress(coder, indexType, size);
		}
		break;

		case IL_OP_LDELEM_I1:
		{
			/* Load a signed byte from an array */
			LoadArrayElem(coder, COP_BREAD_ELEM, COP_BREAD,
						  indexType, 0, 1);
		}
		break;

		case IL_OP_LDELEM_U1:
		{
			/* Load an unsigned byte from an array */
			LoadArrayElem(coder, COP_UBREAD_ELEM, COP_UBREAD,
						  indexType, 0, 1);
		}
		break;

		case IL_OP_LDELEM_I2:
		{
			/* Load a signed short from an array */
			LoadArrayElem(coder, COP_SREAD_ELEM, COP_SREAD,
						  indexType, 1, 1);
		}
		break;

		case IL_OP_LDELEM_U2:
		{
			/* Load an unsigned short from an array */
			LoadArrayElem(coder, COP_USREAD_ELEM, COP_USREAD,
						  indexType, 1, 1);
		}
		break;

		case IL_OP_LDELEM_I4:
		case IL_OP_LDELEM_U4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_LDELEM_I:
	#endif
		{
			/* Load an integer from an array */
			LoadArrayElem(coder, COP_IREAD_ELEM, COP_IREAD,
						  indexType, 2, 1);
		}
		break;

		case IL_OP_LDELEM_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_LDELEM_I:
	#endif
		{
			/* Load a long from an array */
			LoadArrayElem(coder, COP_PREFIX + COP_PREFIX_LREAD_ELEM, COP_MREAD,
						  indexType, 3, CVM_WORDS_PER_LONG);
		}
		break;

		case IL_OP_LDELEM_R4:
		{
			/* Load a 32-bit float from an array */
			LoadArrayElem(coder, COP_PREFIX + COP_PREFIX_FREAD_ELEM, COP_FREAD,
						  indexType, 2, CVM_WORDS_PER_NATIVE_FLOAT);
		}
		break;

		case IL_OP_LDELEM_R8:
		{
			/* Load a 64-bit float from an array */
			LoadArrayElem(coder, COP_PREFIX + COP_PREFIX_DREAD_ELEM, COP_DREAD,
						  indexType, 3, CVM_WORDS_PER_NATIVE_FLOAT);
		}
		break;

		case IL_OP_LDELEM_REF:
		{
			/* Load a pointer from an array */
			if(sizeof(void *) == 4)
			{
				LoadArrayElem(coder, COP_PREAD_ELEM, COP_PREAD,
				              indexType, 2, 1);
			}
			else
			{
				LoadArrayElem(coder, COP_PREAD_ELEM, COP_PREAD,
				              indexType, 3, 1);
			}
		}
		break;

		case IL_OP_STELEM_I1:
		{
			/* Store a byte value to an array */
			StoreArrayElem(coder, COP_BWRITE_ELEM, COP_BWRITE,
			               indexType, 0, 1);
		}
		break;

		case IL_OP_STELEM_I2:
		{
			/* Store a short value to an array */
			StoreArrayElem(coder, COP_SWRITE_ELEM, COP_SWRITE,
			               indexType, 1, 1);
		}
		break;

		case IL_OP_STELEM_I4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_STELEM_I:
	#endif
		{
			/* Store an integer value to an array */
			StoreArrayElem(coder, COP_IWRITE_ELEM, COP_IWRITE,
			               indexType, 2, 1);
		}
		break;

		case IL_OP_STELEM_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_STELEM_I:
	#endif
		{
			/* Store a long value to an array */
			StoreArrayElem(coder, COP_PREFIX + COP_PREFIX_LWRITE_ELEM,
						   COP_MWRITE, indexType, 3, CVM_WORDS_PER_LONG);
		}
		break;

		case IL_OP_STELEM_R4:
		{
			/* Store a 32-bit floating point value to an array */
			StoreArrayElem(coder, COP_PREFIX + COP_PREFIX_FWRITE_ELEM,
						   COP_FWRITE, indexType, 2,
						   CVM_WORDS_PER_NATIVE_FLOAT);
		}
		break;

		case IL_OP_STELEM_R8:
		{
			/* Store a 64-bit floating point value to an array */
			StoreArrayElem(coder, COP_PREFIX + COP_PREFIX_DWRITE_ELEM,
						   COP_DWRITE, indexType, 3,
						   CVM_WORDS_PER_NATIVE_FLOAT);
		}
		break;

		case IL_OP_STELEM_REF:
		{
			/* Store a pointer to an array */
			if(sizeof(void *) == 4)
			{
				StoreArrayElem(coder, COP_PWRITE_ELEM, COP_PWRITE,
							   indexType, 2, 1);
			}
			else
			{
				StoreArrayElem(coder, COP_PWRITE_ELEM, COP_PWRITE,
							   indexType, 3, 1);
			}
		}
		break;
	}
}

/*
 * Handle a pointer indirection opcode.
 */
static void CVMCoder_PtrAccess(ILCoder *coder, int opcode,
							   const ILCoderPrefixInfo *prefixInfo)
{
	switch(opcode)
	{
		case IL_OP_LDIND_I1:
		{
			/* Load a signed byte from a pointer */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_NONE(COP_BREAD);
		}
		break;

		case IL_OP_LDIND_U1:
		{
			/* Load an unsigned byte from a pointer */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_NONE(COP_UBREAD);
		}
		break;

		case IL_OP_LDIND_I2:
		{
			/* Load a signed short from a pointer */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_NONE(COP_SREAD);
		}
		break;

		case IL_OP_LDIND_U2:
		{
			/* Load an unsigned short from a pointer */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_NONE(COP_USREAD);
		}
		break;

		case IL_OP_LDIND_I4:
		case IL_OP_LDIND_U4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_LDIND_I:
	#endif
		{
			/* Load an integer from a pointer */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_NONE(COP_IREAD);
		}
		break;

		case IL_OP_LDIND_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_LDIND_I:
	#endif
		{
			/* Load a long from a pointer */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_WIDE(COP_MREAD, 8);
			CVM_ADJUST(CVM_WORDS_PER_LONG - 1);
		}
		break;

		case IL_OP_LDIND_R4:
		{
			/* Load a 32-bit float from a pointer */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_NONE(COP_FREAD);
			CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - 1);
		}
		break;

		case IL_OP_LDIND_R8:
		{
			/* Load a 64-bit float from a pointer */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_NONE(COP_DREAD);
			CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - 1);
		}
		break;

		case IL_OP_LDIND_REF:
		{
			/* Load a pointer from a pointer */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_NONE(COP_PREAD);
		}
		break;

		case IL_OP_STIND_REF:
		{
			/* Store a pointer to a pointer */
			CVM_OUT_WIDE(COP_CKNULL_N, 1);
			CVM_OUT_NONE(COP_PWRITE);
			CVM_ADJUST(-2);
		}
		break;

		case IL_OP_STIND_I1:
		{
			/* Store a byte to a pointer */
			CVM_OUT_WIDE(COP_CKNULL_N, 1);
			CVM_OUT_NONE(COP_BWRITE);
			CVM_ADJUST(-2);
		}
		break;

		case IL_OP_STIND_I2:
		{
			/* Store a short to a pointer */
			CVM_OUT_WIDE(COP_CKNULL_N, 1);
			CVM_OUT_NONE(COP_SWRITE);
			CVM_ADJUST(-2);
		}
		break;

		case IL_OP_STIND_I4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_STIND_I:
	#endif
		{
			/* Store an integer to a pointer */
			CVM_OUT_WIDE(COP_CKNULL_N, 1);
			CVM_OUT_NONE(COP_IWRITE);
			CVM_ADJUST(-2);
		}
		break;

		case IL_OP_STIND_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_STIND_I:
	#endif
		{
			/* Store a long to a pointer */
			CVM_OUT_WIDE(COP_CKNULL_N, CVM_WORDS_PER_LONG);
			CVM_OUT_WIDE(COP_MWRITE, 8);
			CVM_ADJUST(-(CVM_WORDS_PER_LONG + 1));
		}
		break;

		case IL_OP_STIND_R4:
		{
			/* Store a 32-bit float to a pointer */
			CVM_OUT_WIDE(COP_CKNULL_N, CVM_WORDS_PER_NATIVE_FLOAT);
			CVM_OUT_NONE(COP_FWRITE);
			CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT + 1));
		}
		break;

		case IL_OP_STIND_R8:
		{
			/* Store a 64-bit float to a pointer */
			CVM_OUT_WIDE(COP_CKNULL_N, CVM_WORDS_PER_NATIVE_FLOAT);
			CVM_OUT_NONE(COP_DWRITE);
			CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT + 1));
		}
		break;
	}
}

/*
+ * Handle a pointer indirection opcode.
+ */
void CVMCoder_PtrDeref(ILCoder *coder, int pos)
{
	if(pos == 0)
	{
		CVMCoder_PtrAccess(coder, IL_OP_LDIND_REF, 0);
	}
	else
	{
		CVM_OUT_WIDE(COP_DUP_WORD_N, pos);
		CVMCoder_PtrAccess(coder, IL_OP_LDIND_REF, 0);
		CVMP_OUT_WORD(COP_PREFIX_REPL_WORD_N, pos+1);
	}
}

/*
 * Handle a pointer indirection opcode for a managed value.
 */
static void CVMCoder_PtrAccessManaged(ILCoder *coder, int opcode,
									  ILClass *classInfo,
									  const ILCoderPrefixInfo *prefixInfo)
{
	/* Compute the size of the managed value in memory and on the stack */
	ILUInt32 memorySize = _ILSizeOfTypeLocked(((ILCVMCoder *)coder)->process, ILType_FromValueType(classInfo));
	ILUInt32 stackSize = (memorySize + sizeof(CVMWord) - 1) / sizeof(CVMWord);

	/* Generate the bytecode for the instruction */
	if(opcode == IL_OP_LDOBJ)
	{
		/* Load from a pointer */
		CVM_OUT_NONE(COP_CKNULL);
		if(memorySize != 0)
		{
			CVM_OUT_WIDE(COP_MREAD, memorySize);
			CVM_ADJUST(stackSize - 1);
		}
		else
		{
			/* Because the object is empty, there's no point
			   performing the "mread" instruction */
			CVM_OUT_NONE(COP_POP);
			CVM_ADJUST(-1);
		}
	}
	else
	{
		/* Store to a pointer */
		if(memorySize != 0)
		{
			CVM_OUT_WIDE(COP_CKNULL_N, stackSize);
			CVM_OUT_WIDE(COP_MWRITE, memorySize);
			CVM_ADJUST(stackSize + 1);
		}
		else
		{
			/* Because the object is empty, there's no point
			   performing the "mwrite" instruction */
			CVM_OUT_NONE(COP_CKNULL);
			CVM_OUT_NONE(COP_POP);
			CVM_ADJUST(-1);
		}
	}
}

/*
 * Get the length of an array.
 */
static void CVMCoder_ArrayLength(ILCoder *coder)
{
	CVM_OUT_NONE(COP_ARRAY_LEN);
	CVM_ADJUST(CVM_WORDS_PER_NATIVE_INT - 1);
}

/*
 * Construct a new array, given a type and length value.
 */
static void CVMCoder_NewArray(ILCoder *coder, ILType *arrayType,
					 		  ILClass *arrayClass, ILEngineType lengthType)
{
	ILMethod *ctor;

#ifdef IL_NATIVE_INT64
	/* Convert the length into I4 */
	if(lengthType == ILEngineType_I)
	{
		CVM_OUT_NONE(COP_L2I);
	}
#endif

	/* Find the allocation constructor within the array class.
	   We know that the class only has one method, so it must
	   be the constructor that we are looking for */
	ctor = (ILMethod *)ILClassNextMemberByKind
					(arrayClass, 0, IL_META_MEMBERKIND_METHOD);

	/* Output code to call the array type's constructor */
	CVM_OUT_PTR(COP_CALL_CTOR, ctor);
}

/*
 * Check the top of stack value for NULL.
 */
static void CVMCoder_CheckNull(ILCoder *coder)
{
	CVM_OUT_NONE(COP_CKNULL);
}

#endif	/* IL_CVMC_CODE */
