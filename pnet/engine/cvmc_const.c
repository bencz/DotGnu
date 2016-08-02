/*
 * cvmc_const.c - Coder implementation for CVM constants.
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
 * Handle a numeric constant opcode.
 */
static void CVMCoder_Constant(ILCoder *coder, int opcode, unsigned char *arg)
{
	if(opcode >= IL_OP_LDNULL && opcode <= IL_OP_LDC_I4_8)
	{
		CVM_OUT_NONE(opcode - IL_OP_LDNULL + COP_LDNULL);
		CVM_ADJUST(1);
	}
	else if(opcode == IL_OP_LDC_I4_S)
	{
	#ifdef IL_CVM_DIRECT
		/* In direct mode, "ldc_i4" is more efficient than "ldc_i4_s" */
		CVM_OUT_WORD(COP_LDC_I4, (ILInt32)(ILInt8)(arg[0]));
	#else
		CVM_OUT_BYTE(COP_LDC_I4_S, arg[0]);
	#endif
		CVM_ADJUST(1);
	}
	else if(opcode == IL_OP_LDC_I4)
	{
		CVM_OUT_WORD(COP_LDC_I4, IL_READ_UINT32(arg));
		CVM_ADJUST(1);
	}
	else if(opcode == IL_OP_LDC_R4)
	{
		CVM_OUT_FLOAT(COP_LDC_R4, arg);
		CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT);
	}
	else if(opcode == IL_OP_LDC_I8)
	{
		CVM_OUT_LONG(COP_LDC_I8, arg);
		CVM_ADJUST(CVM_WORDS_PER_LONG);
	}
	else if(opcode == IL_OP_LDC_R8)
	{
		CVM_OUT_DOUBLE(COP_LDC_R8, arg);
		CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT);
	}
}

/*
 * Handle a string constant opcode.
 */
static void CVMCoder_StringConstant(ILCoder *coder, ILToken token, void *object)
{
	if(object)
	{
		/* Push the object pointer directly, to save time at runtime */
		CVM_OUT_PTR(COP_LDTOKEN, object);
		CVM_ADJUST(1);
	}
	else
	{
		CVM_OUT_WORD(COP_LDSTR, token);
		CVM_ADJUST(1);
	}
}

#endif	/* IL_CVMC_CODE */
