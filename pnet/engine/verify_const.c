/*
 * verify_const.c - Verify instructions related to constants.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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

#if defined(IL_VERIFY_GLOBALS)

/* No globals required */

#elif defined(IL_VERIFY_LOCALS)

ILClass *stringClass = 0;
ILUInt32 strLen;

#else /* IL_VERIFY_CODE */

case IL_OP_LDNULL:
{
	/* Load the "null" constant onto the stack */
	ILCoderConstant(coder, opcode, pc + 1);
	stack[stackSize].engineType = ILEngineType_O;
	stack[stackSize].typeInfo = 0;
	++stackSize;
}
break;

case IL_OP_LDC_I4_M1:
case IL_OP_LDC_I4_0:
case IL_OP_LDC_I4_1:
case IL_OP_LDC_I4_2:
case IL_OP_LDC_I4_3:
case IL_OP_LDC_I4_4:
case IL_OP_LDC_I4_5:
case IL_OP_LDC_I4_6:
case IL_OP_LDC_I4_7:
case IL_OP_LDC_I4_8:
case IL_OP_LDC_I4_S:
case IL_OP_LDC_I4:
{
	/* 32-bit integer constants */
	ILCoderConstant(coder, opcode, pc + 1);
	stack[stackSize].engineType = ILEngineType_I4;
	stack[stackSize].typeInfo = 0;
	++stackSize;
}
break;

case IL_OP_LDC_I8:
{
	/* 64-bit integer constants */
	ILCoderConstant(coder, opcode, pc + 1);
	stack[stackSize].engineType = ILEngineType_I8;
	stack[stackSize].typeInfo = 0;
	++stackSize;
}
break;

case IL_OP_LDC_R4:
{
	/* 32-bit floating point constants */
	ILCoderConstant(coder, opcode, pc + 1);
	stack[stackSize].engineType = ILEngineType_F;
	stack[stackSize].typeInfo = 0;
	++stackSize;
}
break;

case IL_OP_LDC_R8:
{
	/* 64-bit floating point constants */
	ILCoderConstant(coder, opcode, pc + 1);
	stack[stackSize].engineType = ILEngineType_F;
	stack[stackSize].typeInfo = 0;
	++stackSize;
}
break;

case IL_OP_LDSTR:
{
	/* String constants */
	if(!stringClass)
	{
		stringClass = ILClassResolveSystem(ILProgramItem_Image(method), 0,
										   "String", "System");
		if(!stringClass)
		{
			goto cleanup;
		}
	}
	argNum = IL_READ_UINT32(pc + 1);
	if((argNum & IL_META_TOKEN_MASK) != IL_META_TOKEN_STRING ||
	   !ILImageGetUserString(ILProgramItem_Image(method),
	   						 argNum & ~IL_META_TOKEN_MASK, &strLen))
	{
		VERIFY_INSN_ERROR();
	}
	if(thread)
	{
		ILCoderStringConstant(coder, (ILToken)argNum,
				_ILStringInternFromImage(thread, ILProgramItem_Image(method),
										 (ILToken)argNum));
	}
	else
	{
		ILCoderStringConstant(coder, (ILToken)argNum, 0);
	}
	stack[stackSize].engineType = ILEngineType_O;
	stack[stackSize].typeInfo = ILType_FromClass(stringClass);
	++stackSize;
}
break;

#endif /* IL_VERIFY_CODE */
