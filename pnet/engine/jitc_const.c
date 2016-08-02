/*
 * jitc_const.c - Coder implementation for JIT constants.
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
 * Handle a numeric constant opcode.
 */
static void JITCoder_Constant(ILCoder *coder, int opcode, unsigned char *arg)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitValue value;

	if(opcode == IL_OP_LDNULL)
	{
		value = jit_value_create_nint_constant(jitCoder->jitFunction,
											   _IL_JIT_TYPE_VPTR,
											   (jit_nint)0);
		_ILJitStackPushValue(jitCoder, value);
	}
	else if(opcode >= IL_OP_LDC_I4_M1 && opcode <= IL_OP_LDC_I4_8)
	{
		ILInt32 temp = opcode - IL_OP_LDC_I4_M1 - 1;

		value = jit_value_create_nint_constant(jitCoder->jitFunction,
											   _IL_JIT_TYPE_INT32,
											   (jit_nint)temp);
		if(temp == 0)
		{
			_ILJitStackPushValue(jitCoder, value);
		}
		else
		{
			_ILJitStackPushNotNullValue(jitCoder, value);
		}
	}
	else if(opcode == IL_OP_LDC_I4_S)
	{
		ILInt32 temp = (ILInt32)((char)arg[0]);

		value = jit_value_create_nint_constant(jitCoder->jitFunction,
											   _IL_JIT_TYPE_INT32,
											   (jit_nint)temp);
		if(temp == 0)
		{
			_ILJitStackPushValue(jitCoder, value);
		}
		else
		{
			_ILJitStackPushNotNullValue(jitCoder, value);
		}
	}
	else if(opcode == IL_OP_LDC_I4)
	{
		ILUInt32 temp = IL_READ_UINT32(arg);

		value = jit_value_create_nint_constant(jitCoder->jitFunction,
											   _IL_JIT_TYPE_INT32,
											   (jit_nint)temp);
		if(temp == 0)
		{
			_ILJitStackPushValue(jitCoder, value);
		}
		else
		{
			_ILJitStackPushNotNullValue(jitCoder, value);
		}
	}
	else if(opcode == IL_OP_LDC_R4)
	{
		ILFloat temp;

		JITC_GET_FLOAT32(arg, temp);
		value = jit_value_create_float32_constant(jitCoder->jitFunction,
												  _IL_JIT_TYPE_SINGLE,
												  (jit_float32)temp);
		_ILJitStackPushValue(jitCoder, value);
	}
	else if(opcode == IL_OP_LDC_I8)
	{
		ILInt64 temp;

		JITC_GET_INT64(arg, temp);
		value = jit_value_create_long_constant(jitCoder->jitFunction,
											   _IL_JIT_TYPE_INT64,
											   (jit_long)temp);
		if(temp == 0)
		{
			_ILJitStackPushValue(jitCoder, value);
		}
		else
		{
			_ILJitStackPushNotNullValue(jitCoder, value);
		}
	}
	else if(opcode == IL_OP_LDC_R8)
	{
		ILDouble temp;

		JITC_GET_FLOAT64(arg, temp);
		value = jit_value_create_float64_constant(jitCoder->jitFunction,
												  _IL_JIT_TYPE_DOUBLE,
												  (jit_float64)temp);
		_ILJitStackPushValue(jitCoder, value);
	}
}

/*
 * Handle a string constant opcode.
 */
static void JITCoder_StringConstant(ILCoder *coder, ILToken token, void *object)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitValue value;

	if(object)
	{
		/* Push the object pointer directly, to save time at runtime */
		value = jit_value_create_nint_constant(jitCoder->jitFunction,
											   _IL_JIT_TYPE_VPTR,
											   (jit_nint)object);
		if(object == 0)
		{
			_ILJitStackPushValue(jitCoder, value);
		}
		else
		{
			_ILJitStackPushNotNullValue(jitCoder, value);
		}
	}
	else
	{
		value = jit_value_create_nint_constant(jitCoder->jitFunction,
											   _IL_JIT_TYPE_VPTR, 
								    		   (jit_nint)token);

		if(token == 0)
		{
			_ILJitStackPushValue(jitCoder, value);
		}
		else
		{
			_ILJitStackPushNotNullValue(jitCoder, value);
		}
	}
}

#endif	/* IL_JITC_CODE */
