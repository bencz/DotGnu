/*
 * cvmc_conv.c - Coder implementation for CVM conversions.
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
 * Handle a conversion opcode.
 */
static void CVMCoder_Conv(ILCoder *coder, int opcode, ILEngineType type)
{
	/* Change I into either I4 or I8, depending on the platform */
	if(type == ILEngineType_I)
	{
	#ifdef IL_NATIVE_INT32
		type = ILEngineType_I4;
	#else
		type = ILEngineType_I8;
	#endif
	}

	/* Determine how to convert the value */
	switch(opcode)
	{
		case IL_OP_CONV_I1:
		{
			/* Convert to "int8" */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_I2B);
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_L2I);
				CVM_OUT_NONE(COP_I2B);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVM_OUT_NONE(COP_F2I);
				CVM_OUT_NONE(COP_I2B);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_I2:
		{
			/* Convert to "int16" */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_I2S);
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_L2I);
				CVM_OUT_NONE(COP_I2S);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVM_OUT_NONE(COP_F2I);
				CVM_OUT_NONE(COP_I2S);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_I4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_CONV_I:
	#endif
		{
			/* Convert to "int32" */
			if(type == ILEngineType_I4)
			{
				/* Nothing to do here */
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_L2I);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVM_OUT_NONE(COP_F2I);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_CONV_I:
	#endif
		{
			/* Convert to "int64" */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_I2L);
				CVM_ADJUST(CVM_WORDS_PER_LONG - 1);
			}
			else if(type == ILEngineType_I8)
			{
				/* Nothing to do here */
			}
			else
			{
				CVM_OUT_NONE(COP_F2L);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - CVM_WORDS_PER_LONG));
			}
		}
		break;

		case IL_OP_CONV_R4:
		{
			/* Convert to "float32" */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_I2F);
				CVM_OUT_NONE(COP_F2F);	/* Round "native float" to "float32" */
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - 1);
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_L2F);
				CVM_OUT_NONE(COP_F2F);	/* Round "native float" to "float32" */
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - CVM_WORDS_PER_LONG);
			}
			else
			{
				CVM_OUT_NONE(COP_F2F);
			}
		}
		break;

		case IL_OP_CONV_R8:
		{
			/* Convert to "float64" */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_I2F);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - 1);
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_L2F);
				CVM_OUT_NONE(COP_F2D);	/* Round "native float" to "float64" */
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - CVM_WORDS_PER_LONG);
			}
			else
			{
				CVM_OUT_NONE(COP_F2D);
			}
		}
		break;

		case IL_OP_CONV_U1:
		{
			/* Convert to "unsigned int8" */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_I2UB);
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_L2I);
				CVM_OUT_NONE(COP_I2UB);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVM_OUT_NONE(COP_F2I);
				CVM_OUT_NONE(COP_I2UB);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_U2:
		{
			/* Convert to "unsigned int16" */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_I2US);
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_L2I);
				CVM_OUT_NONE(COP_I2US);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVM_OUT_NONE(COP_F2I);
				CVM_OUT_NONE(COP_I2US);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_U4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_CONV_U:
	#endif
		{
			/* Convert to "unsigned int32" */
			if(type == ILEngineType_I4)
			{
				/* Nothing to do here */
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_L2I);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVM_OUT_NONE(COP_F2IU);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_U8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_CONV_U:
	#endif
		{
			/* Convert to "unsigned int64" */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IU2L);
				CVM_ADJUST(CVM_WORDS_PER_LONG - 1);
			}
			else if(type == ILEngineType_I8)
			{
				/* Nothing to do here */
			}
			else
			{
				CVM_OUT_NONE(COP_F2LU);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - CVM_WORDS_PER_LONG));
			}
		}
		break;

		case IL_OP_CONV_R_UN:
		{
			/* Convert to "native float" with unsigned input */
			if(type == ILEngineType_I4)
			{
				CVM_OUT_NONE(COP_IU2F);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - 1);
			}
			else if(type == ILEngineType_I8)
			{
				CVM_OUT_NONE(COP_LU2F);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT - CVM_WORDS_PER_LONG);
			}
			else
			{
				/* Nothing to do here */
			}
		}
		break;

		case IL_OP_CONV_OVF_I1_UN:
		{
			/* Convert to "int8" with unsigned input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_IU2B_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_LU2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2B_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2B_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_I2_UN:
		{
			/* Convert to "int16" with unsigned input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_IU2S_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_LU2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2S_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2S_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_I4_UN:
	#ifdef IL_NATIVE_INT32
		case IL_OP_CONV_OVF_I_UN:
	#endif
		{
			/* Convert to "int32" with unsigned input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_IU2I_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_LU2I_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_I8_UN:
	#ifdef IL_NATIVE_INT64
		case IL_OP_CONV_OVF_I_UN:
	#endif
		{
			/* Convert to "int64" with unsigned input and overflow */
			if(type == ILEngineType_I4)
			{
				/* This operation can never overflow */
				CVM_OUT_NONE(COP_IU2L);
				CVM_ADJUST(CVM_WORDS_PER_LONG - 1);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_LU2L_OVF);
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2L_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - CVM_WORDS_PER_LONG));
			}
		}
		break;

		case IL_OP_CONV_OVF_U1_UN:
		{
			/* Convert to "unsigned int8" with unsigned input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_IU2UB_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_LU2IU_OVF);
				CVMP_OUT_NONE(COP_PREFIX_IU2UB_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2UB_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_U2_UN:
		{
			/* Convert to "unsigned int16" with unsigned input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_IU2US_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_LU2IU_OVF);
				CVMP_OUT_NONE(COP_PREFIX_IU2US_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2US_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_U4_UN:
	#ifdef IL_NATIVE_INT32
		case IL_OP_CONV_OVF_U_UN:
	#endif
		{
			/* Convert to "unsigned int32" with unsigned input and overflow */
			if(type == ILEngineType_I4)
			{
				/* Nothing to do here */
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_LU2IU_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2IU_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_U8_UN:
	#ifdef IL_NATIVE_INT64
		case IL_OP_CONV_OVF_U_UN:
	#endif
		{
			/* Convert to "unsigned int64" with unsigned input and overflow */
			if(type == ILEngineType_I4)
			{
				/* This operation can never overflow */
				CVM_OUT_NONE(COP_IU2L);
				CVM_ADJUST(CVM_WORDS_PER_LONG - 1);
			}
			else if(type == ILEngineType_I8)
			{
				/* Nothing to do here */
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2LU_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - CVM_WORDS_PER_LONG));
			}
		}
		break;

		case IL_OP_CONV_OVF_I1:
		{
			/* Convert to "int8" with signed input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_I2B_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_L2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2B_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2B_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_U1:
		{
			/* Convert to "unsigned int8" with signed input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_I2UB_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_L2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2UB_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2UB_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_I2:
		{
			/* Convert to "int16" with signed input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_I2S_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_L2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2S_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2S_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_U2:
		{
			/* Convert to "unsigned int16" with signed input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_I2US_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_L2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2US_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVMP_OUT_NONE(COP_PREFIX_I2US_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_I4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_CONV_OVF_I:
	#endif
		{
			/* Convert to "int32" with signed input and overflow */
			if(type == ILEngineType_I4)
			{
				/* Nothing to do here */
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_L2I_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2I_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_U4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_CONV_OVF_U:
	#endif
		{
			/* Convert to "unsigned int32" with signed input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_I2IU_OVF);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_L2UI_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG - 1));
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2IU_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_CONV_OVF_I:
	#endif
		{
			/* Convert to "int64" with signed input and overflow */
			if(type == ILEngineType_I4)
			{
				/* This operation will never overflow */
				CVM_OUT_NONE(COP_I2L);
			}
			else if(type == ILEngineType_I8)
			{
				/* Nothing to do here */
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2L_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;

		case IL_OP_CONV_OVF_U8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_CONV_OVF_U:
	#endif
		{
			/* Convert to "unsigned int64" with signed input and overflow */
			if(type == ILEngineType_I4)
			{
				CVMP_OUT_NONE(COP_PREFIX_I2UL_OVF);
				CVM_ADJUST(CVM_WORDS_PER_LONG - 1);
			}
			else if(type == ILEngineType_I8)
			{
				CVMP_OUT_NONE(COP_PREFIX_L2UL_OVF);
			}
			else
			{
				CVMP_OUT_NONE(COP_PREFIX_F2LU_OVF);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT - 1));
			}
		}
		break;
	}
}

/*
 * Convert an I or I4 integer into a pointer.
 */
static void CVMCoder_ToPointer(ILCoder *coder, ILEngineType type1,
							   ILEngineStackItem *type2)
{
#ifdef IL_NATIVE_INT64
	/* We only need this on a 64-bit platform when the type is I4 */
	ILUInt32 size;
	if(type1 == ILEngineType_I4)
	{
		if(type2)
		{
			/* Convert a value which is lower down the stack */
			size = ComputeStackSize(coder, type2, 1);
			CVM_OUT_WIDE(COP_I2P_LOWER, size);
		}
		else
		{
			/* Convert the top of the stack into a pointer */
			CVM_OUT_WIDE(COP_I2P_LOWER, 0);
		}
	}
#endif
}

/*
 * Output an instruction to convert the top of stack according
 * to a PInvoke marshalling rule.
 */
static void CVMCoder_Convert(ILCoder *coder, int opcode)
{
	CVMP_OUT_NONE(opcode);
}

/*
 * Output an instruction to convert the top of stack according
 * to a custom marshalling rule.
 */
static void CVMCoder_ConvertCustom(ILCoder *coder, int opcode,
						    	   ILUInt32 customNameLen,
								   ILUInt32 customCookieLen,
						    	   void *customName, void *customCookie)
{
	CVMP_OUT_WORD2_PTR2(COP_PREFIX_TOCUSTOM,
					    customNameLen, customCookieLen,
					    customName, customCookie);
}

#endif	/* IL_CVMC_CODE */
