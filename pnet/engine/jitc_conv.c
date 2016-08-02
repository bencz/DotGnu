/*
 * jitc_conv.c - Coder implementation for JIT conversions.
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
 * Handle a conversion opcode.
 */
static void JITCoder_Conv(ILCoder *coder, int opcode, ILEngineType type)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitValue value = 0;
	ILJitStackItem *stackItem;

	/* Get the topmost item on the stack. */
	stackItem = _ILJitStackItemGetTop(jitCoder, 0);

	/* Determine how to convert the value */
	switch(opcode)
	{
		case IL_OP_CONV_I1:
		{
			/* Convert to "int8" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_SBYTE,
											   0, 0);
		}
		break;

		case IL_OP_CONV_I2:
		{
			/* Convert to "int16" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_INT16,
											   0, 0);
		}
		break;

		case IL_OP_CONV_I4:
		{
			/* Convert to "int32" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_INT32,
											   0, 0);
		}
		break;

		case IL_OP_CONV_I8:
		{
			/* Convert to "int64" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_INT64,
											   0, 0);
		}
		break;

		case IL_OP_CONV_I:
		{
			/* Convert to "native int" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_NINT,
											   0, 0);
		}
		break;

		case IL_OP_CONV_R4:
		{
			/* Convert to "float32" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_SINGLE,
											   0, 0);
		}
		break;

		case IL_OP_CONV_R8:
		{
			/* Convert to "float64" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_DOUBLE,
											   0, 0);
		}
		break;

		case IL_OP_CONV_U1:
		{
			/* Convert to "unsigned int8" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_BYTE,
											   0, 0);
		}
		break;

		case IL_OP_CONV_U2:
		{
			/* Convert to "unsigned int16" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_UINT16,
											   0, 0);
		}
		break;

		case IL_OP_CONV_U4:
		{
			/* Convert to "unsigned int32" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_UINT32,
											   0, 0);
		}
		break;

		case IL_OP_CONV_U8:
		{
			/* Convert to "unsigned int64" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_UINT64,
											   0, 0);
		}
		break;

		case IL_OP_CONV_U:
		{
			/* Convert to "unsigned native int" */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_NUINT,
											   0, 0);
		}
		break;

		case IL_OP_CONV_R_UN:
		{
			/* Convert to "native float" with unsigned input */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_NFLOAT,
											   1, 0);
		}
		break;

		case IL_OP_CONV_OVF_I1_UN:
		{
			/* Convert to "int8" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_SBYTE,
											   1, 1);
		}
		break;

		case IL_OP_CONV_OVF_I2_UN:
		{
			/* Convert to "int16" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_INT16,
											   1, 1);
		}
		break;

		case IL_OP_CONV_OVF_I4_UN:
		{
			/* Convert to "int32" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_INT32,
											   1, 1);
		}
		break;

		case IL_OP_CONV_OVF_I8_UN:
		{
			/* Convert to "int64" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_INT64,
											   1, 1);
		}
		break;

		case IL_OP_CONV_OVF_I_UN:
		{
			/* Convert to "native int" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_NINT,
											   1, 1);
		}
		break;

		case IL_OP_CONV_OVF_U1_UN:
		{
			/* Convert to "unsigned int8" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_BYTE,
											   1, 1);
		}
		break;

		case IL_OP_CONV_OVF_U2_UN:
		{
			/* Convert to "unsigned int16" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_UINT16,
											   1, 1);
		}
		break;

		case IL_OP_CONV_OVF_U4_UN:
		{
			/* Convert to "unsigned int32" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_UINT32,
											   1, 1);
		}
		break;

		case IL_OP_CONV_OVF_U8_UN:
		{
			/* Convert to "unsigned int64" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_UINT64,
											   1, 1);
		}
		break;

		case IL_OP_CONV_OVF_U_UN:
		{
			/* Convert to "unsigned native int" with unsigned input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_NUINT,
											   1, 1);

		}
		break;

		case IL_OP_CONV_OVF_I1:
		{
			/* Convert to "int8" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_SBYTE,
											   0, 1);
		}
		break;

		case IL_OP_CONV_OVF_U1:
		{
			/* Convert to "unsigned int8" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_BYTE,
											   0, 1);
		}
		break;

		case IL_OP_CONV_OVF_I2:
		{
			/* Convert to "int16" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_INT16,
											   0, 1);
		}
		break;

		case IL_OP_CONV_OVF_U2:
		{
			/* Convert to "unsigned int16" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_UINT16,
											   0, 1);
		}
		break;

		case IL_OP_CONV_OVF_I4:
		{
			/* Convert to "int32" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_INT32,
											   0, 1);
		}
		break;

		case IL_OP_CONV_OVF_U4:
		{
			/* Convert to "unsigned int32" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_UINT32,
											   0, 1);
		}
		break;

		case IL_OP_CONV_OVF_I:
		{
			/* Convert to "native int" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_NINT,
											   0, 1);
		}
		break;

		case IL_OP_CONV_OVF_U:
		{
			/* Convert to "unsigned native int" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_NUINT,
											   0, 1);
		}
		break;

		case IL_OP_CONV_OVF_I8:
		{
			/* Convert to "int64" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_INT64,
											   0, 1);
		}
		break;

		case IL_OP_CONV_OVF_U8:
		{
			/* Convert to "unsigned int64" with signed input and overflow */
			value = _ILJitValueConvertExplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(*stackItem),
											   _IL_JIT_TYPE_UINT64,
											   0, 1);
		}
		break;
	}
	_ILJitStackItemSetValue(*stackItem, value);
}

/*
 * Convert an I or I4 integer into a pointer.
 */
static void JITCoder_ToPointer(ILCoder *coder, ILEngineType type1,
							   ILEngineStackItem *type2)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitStackItem *stackItem;
	ILJitValue value;

	if(!type2)
	{
		stackItem = _ILJitStackItemGetTop(jitCoder, 0);
	}
	else
	{
		stackItem = _ILJitStackItemGetTop(jitCoder, 1);
	}
	value = jit_insn_convert(jitCoder->jitFunction,
							 _ILJitStackItemValue(*stackItem),
							 _IL_JIT_TYPE_VPTR, 0);
	_ILJitStackItemSetValue(*stackItem, value);
}

/*
 * Output an instruction to convert the top of stack according
 * to a PInvoke marshalling rule.
 */
static void JITCoder_Convert(ILCoder *coder, int opcode)
{
}

/*
 * Output an instruction to convert the top of stack according
 * to a custom marshalling rule.
 */
static void JITCoder_ConvertCustom(ILCoder *coder, int opcode,
						    	   ILUInt32 customNameLen,
								   ILUInt32 customCookieLen,
						    	   void *customName, void *customCookie)
{
}

#endif	/* IL_JITC_CODE */
