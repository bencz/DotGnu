/*
 * cvmc_arith.c - Coder implementation for CVM arithmetic operations.
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
 * Readjust the stack to normalize binary operands when
 * I and I4 are mixed together.  Also determine which of
 * I4 or I8 to use if the operation involves I.
 */
static void AdjustMixedBinary(ILCoder *coder, int isUnsigned,
							  ILEngineType *type1, ILEngineType *type2)
{
#ifdef IL_NATIVE_INT64
	/* If the arguments mix I and I4, then cast the I4 value to I */
	if(*type1 == ILEngineType_I && *type2 == ILEngineType_I4)
	{
		if(isUnsigned)
		{
			CVM_OUT_NONE(COP_IU2L);
		}
		else
		{
			CVM_OUT_NONE(COP_I2L);
		}
		CVM_ADJUST(CVM_WORDS_PER_NATIVE_INT - 1);
	}
	else if(*type1 == ILEngineType_I4 && *type2 == ILEngineType_I)
	{
		if(isUnsigned)
		{
			CVMP_OUT_NONE(COP_PREFIX_FIX_I4_U);
		}
		else
		{
			CVMP_OUT_NONE(COP_PREFIX_FIX_I4_I);
		}
		CVM_ADJUST(CVM_WORDS_PER_NATIVE_INT - 1);
	}
#endif

	/* If one of the arguments is M or T, then both should be M.
	   This is intended for use with pointer comparisons */
	if(*type1 == ILEngineType_M || *type1 == ILEngineType_T ||
	   *type2 == ILEngineType_M || *type2 == ILEngineType_T)
	{
		*type1 = ILEngineType_M;
		*type2 = ILEngineType_M;
	}

	/* If one of the arguments is I, then determine if
	   CVM needs to compile an I4 or an I8 instruction */
	if(*type1 == ILEngineType_I || *type2 == ILEngineType_I)
	{
	#ifdef IL_NATIVE_INT32
		*type1 = ILEngineType_I4;
		*type2 = ILEngineType_I4;
	#else
		*type1 = ILEngineType_I8;
		*type2 = ILEngineType_I8;
	#endif
	}
}

/*
 * Determine if a binary opcode involves the use of unsigned types.
 */
#define	IS_UNSIGNED_OPCODE(opcode)	\
				(opcode == IL_OP_ADD_OVF_UN || \
				 opcode == IL_OP_SUB_OVF_UN || \
				 opcode == IL_OP_MUL_OVF_UN || \
				 opcode == IL_OP_DIV_UN || \
				 opcode == IL_OP_REM_UN)

/*
 * Handle a binary opcode.
 */
static void CVMCoder_Binary(ILCoder *coder, int opcode,
							ILEngineType type1, ILEngineType type2)
{
	/* Adjust the arguments if I is involved */
	AdjustMixedBinary(coder, IS_UNSIGNED_OPCODE(opcode), &type1, &type2);

	/* Determine how to perform the operation */
	switch(opcode)
	{
		case IL_OP_ADD:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IADD);
				CVM_ADJUST(-1);
			}
			else if(type1 == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_LADD);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
			else
			{
				CVM_OUT_NONE(COP_FADD);
				CVM_ADJUST(-CVM_WORDS_PER_NATIVE_FLOAT);
			}
		}
		break;

		case IL_OP_ADD_OVF:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IADD_OVF);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LADD_OVF);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_ADD_OVF_UN:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IADD_OVF_UN);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LADD_OVF_UN);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_SUB:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_ISUB);
				CVM_ADJUST(-1);
			}
			else if(type1 == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_LSUB);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
			else
			{
				CVM_OUT_NONE(COP_FSUB);
				CVM_ADJUST(-CVM_WORDS_PER_NATIVE_FLOAT);
			}
		}
		break;

		case IL_OP_SUB_OVF:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_ISUB_OVF);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LSUB_OVF);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_SUB_OVF_UN:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_ISUB_OVF_UN);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LSUB_OVF_UN);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_MUL:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IMUL);
				CVM_ADJUST(-1);
			}
			else if(type1 == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_LMUL);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
			else
			{
				CVM_OUT_NONE(COP_FMUL);
				CVM_ADJUST(-CVM_WORDS_PER_NATIVE_FLOAT);
			}
		}
		break;

		case IL_OP_MUL_OVF:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IMUL_OVF);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LMUL_OVF);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_MUL_OVF_UN:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IMUL_OVF_UN);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LMUL_OVF_UN);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_DIV:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IDIV);
				CVM_ADJUST(-1);
			}
			else if(type1 == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_LDIV);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
			else
			{
				CVM_OUT_NONE(COP_FDIV);
				CVM_ADJUST(-CVM_WORDS_PER_NATIVE_FLOAT);
			}
		}
		break;

		case IL_OP_DIV_UN:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IDIV_UN);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LDIV_UN);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_REM:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IREM);
				CVM_ADJUST(-1);
			}
			else if(type1 == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_LREM);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
			else
			{
				CVM_OUT_NONE(COP_FREM);
				CVM_ADJUST(-CVM_WORDS_PER_NATIVE_FLOAT);
			}
		}
		break;

		case IL_OP_REM_UN:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IREM_UN);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LREM_UN);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_AND:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IAND);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LAND);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_OR:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IOR);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LOR);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;

		case IL_OP_XOR:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IXOR);
				CVM_ADJUST(-1);
			}
			else
			{
				CVM_OUT_NONE(COP_LXOR);
				CVM_ADJUST(-CVM_WORDS_PER_LONG);
			}
		}
		break;
	}
}

/*
 * Handle a binary opcode when pointer arithmetic is involved.
 */
static void CVMCoder_BinaryPtr(ILCoder *coder, int opcode,
							   ILEngineType type1, ILEngineType type2)
{
	switch(opcode)
	{
		case IL_OP_ADD:
		case IL_OP_ADD_OVF_UN:
		{
			if(type1 == ILEngineType_M || type1 == ILEngineType_T)
			{
				/* Add a pointer and an integer */
			#ifdef IL_NATIVE_INT64
				if(type2 == ILEngineType_I4)
				{
					CVM_OUT_NONE(COP_PADD_I4);
					CVM_ADJUST(-1);
				}
				else
				{
					CVM_OUT_NONE(COP_PADD_I8);
					CVM_ADJUST(-CVM_WORDS_PER_LONG);
				}
			#else
				CVM_OUT_NONE(COP_PADD_I4);
				CVM_ADJUST(-1);
			#endif
			}
			else
			{
				/* Add an integer and a pointer */
			#ifdef IL_NATIVE_INT64
				if(type1 == ILEngineType_I4)
				{
					CVM_OUT_NONE(COP_PADD_I4_R);
					CVM_ADJUST(-1);
				}
				else
				{
					CVM_OUT_NONE(COP_PADD_I8_R);
					CVM_ADJUST(-CVM_WORDS_PER_LONG);
				}
			#else
				CVM_OUT_NONE(COP_PADD_I4_R);
				CVM_ADJUST(-1);
			#endif
			}
		}
		break;

		case IL_OP_SUB:
		case IL_OP_SUB_OVF_UN:
		{
			if(type2 == ILEngineType_M || type2 == ILEngineType_T)
			{
				/* Subtract two pointers */
				CVM_OUT_NONE(COP_PSUB);
				CVM_ADJUST(-1);
			}
			else
			{
				/* Subtract an integer from a pointer */
			#ifdef IL_NATIVE_INT64
				if(type2 == ILEngineType_I4)
				{
					CVM_OUT_NONE(COP_PSUB_I4);
					CVM_ADJUST(-1);
				}
				else
				{
					CVM_OUT_NONE(COP_PSUB_I8);
					CVM_ADJUST(-CVM_WORDS_PER_LONG);
				}
			#else
				CVM_OUT_NONE(COP_PSUB_I4);
				CVM_ADJUST(-1);
			#endif
			}
		}
		break;
	}
}

/*
 * Handle a shift opcode.
 */
static void CVMCoder_Shift(ILCoder *coder, int opcode,
						   ILEngineType type1, ILEngineType type2)
{
#ifdef IL_NATIVE_INT64
	/* Convert the shift count down to 32 bits if necessary */
	if(type2 == ILEngineType_I)
	{
		CVM_OUT_NONE(COP_L2I);
		CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
	}
#endif

	/* If the first type is I, then convert it into I4 or I8 */
	if(type1 == ILEngineType_I)
	{
	#ifdef IL_NATIVE_INT32
		type1 = ILEngineType_I4;
	#else
		type1 = ILEngineType_I8;
	#endif
	}

	/* Determine how to perform the operation */
	switch(opcode)
	{
		case IL_OP_SHL:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_ISHL);
			}
			else
			{
				CVM_OUT_NONE(COP_LSHL);
			}
		}
		break;

		case IL_OP_SHR:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_ISHR);
			}
			else
			{
				CVM_OUT_NONE(COP_LSHR);
			}
		}
		break;

		case IL_OP_SHR_UN:
		{
			if(type1 == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_ISHR_UN);
			}
			else
			{
				CVM_OUT_NONE(COP_LSHR_UN);
			}
		}
		break;
	}
	CVM_ADJUST(-1);
}

/*
 * Handle a unary opcode.
 */
static void CVMCoder_Unary(ILCoder *coder, int opcode, ILEngineType type)
{
	switch(opcode)
	{
		case IL_OP_NEG:
		{
			/* Negate the top-most stack item */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_INEG);
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_LNEG);
			}
			else if(type == ILEngineType_I)
			{
			#ifdef IL_NATIVE_INT32
				CVM_OUT_NONE(COP_INEG);
			#else
				CVM_OUT_NONE(COP_LNEG);
			#endif
			}
			else if(type == ILEngineType_F)
			{
				CVM_OUT_NONE(COP_FNEG);
			}
		}
		break;

		case IL_OP_NOT:
		{
			/* Perform a bitwise NOT on the top-most stack item */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_INOT);
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_LNOT);
			}
			else if(type == ILEngineType_I)
			{
			#ifdef IL_NATIVE_INT32
				CVM_OUT_NONE(COP_INOT);
			#else
				CVM_OUT_NONE(COP_LNOT);
			#endif
			}
		}
		break;

		case IL_OP_CKFINITE:
		{
			/* Check the top-most F value to see if it is finite */
			CVMP_OUT_NONE(COP_PREFIX_CKFINITE);
		}
		break;
	}
}

#endif	/* IL_CVMC_CODE */
