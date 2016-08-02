/*
 * verify_arith.c - Verify instructions related to arithmetic.
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

#if defined(IL_VERIFY_GLOBALS)

/*
 * Helper macros for defining type inference matrices.
 */
#define	T_I4		(char)ILEngineType_I4
#define	T_I8		(char)ILEngineType_I8
#define	T_I			(char)ILEngineType_I
#define	T_F			(char)ILEngineType_F
#define	T_U			(char)ILEngineType_U
#define	T_M			(char)ILEngineType_M
#define	T_O			(char)ILEngineType_O
#define	T_T			(char)ILEngineType_T
#define	T_MV		(char)ILEngineType_MV
#define	T_NO		(char)ILEngineType_Invalid

/*
 * Type inference matrix for binary numeric operations.
 */
static char const binaryNumericMatrix
			[ILEngineType_ValidTypes][ILEngineType_ValidTypes] =
{
		    /* I4    I8    I     F     &     O     *     MV */
	/* I4: */ {T_I4, T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I8: */ {T_NO, T_I8, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I:  */ {T_I,  T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* F:  */ {T_NO, T_NO, T_NO, T_F,  T_NO, T_NO, T_NO, T_NO},
	/* &:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* O:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* *:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* MV: */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
};

/*
 * Type inference matrix for binary integer operations.
 */
static char const binaryIntegerMatrix
			[ILEngineType_ValidTypes][ILEngineType_ValidTypes] =
{
		    /* I4    I8    I     F     &     O     *     MV */
	/* I4: */ {T_I4, T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I8: */ {T_NO, T_I8, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I:  */ {T_I,  T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* F:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* &:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* O:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* *:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* MV: */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
};

/*
 * Type inference matrix for binary shift operations.
 */
static char const binaryShiftMatrix
			[ILEngineType_ValidTypes][ILEngineType_ValidTypes] =
{
		    /* I4    I8    I     F     &     O     *     MV */
	/* I4: */ {T_I4, T_NO, T_I4, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I8: */ {T_I8, T_NO, T_I8, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I:  */ {T_I,  T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* F:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* &:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* O:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* *:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* MV: */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
};

/*
 * Type inference matrix for addition when unsafe types are permitted.
 */
static char const addUnsafeMatrix
			[ILEngineType_ValidTypes][ILEngineType_ValidTypes] =
{
		    /* I4    I8    I     F     &     O     *     MV */
	/* I4: */ {T_I4, T_NO, T_I,  T_NO, T_M,  T_NO, T_T,  T_NO},
	/* I8: */ {T_NO, T_I8, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I:  */ {T_I,  T_NO, T_I,  T_NO, T_M,  T_NO, T_T,  T_NO},
	/* F:  */ {T_NO, T_NO, T_NO, T_F,  T_NO, T_NO, T_NO, T_NO},
	/* &:  */ {T_M,  T_NO, T_M,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* O:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* *:  */ {T_T,  T_NO, T_T,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* MV: */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
};

/*
 * Type inference matrix for subtraction when unsafe types are permitted.
 */
static char const subUnsafeMatrix
			[ILEngineType_ValidTypes][ILEngineType_ValidTypes] =
{
		    /* I4    I8    I     F     &     O     *     MV */
	/* I4: */ {T_I4, T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I8: */ {T_NO, T_I8, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I:  */ {T_I,  T_NO, T_I,  T_NO, T_I,  T_NO, T_I,  T_NO},
	/* F:  */ {T_NO, T_NO, T_NO, T_F,  T_NO, T_NO, T_NO, T_NO},
	/* &:  */ {T_M,  T_NO, T_M,  T_NO, T_I,  T_NO, T_I,  T_NO},
	/* O:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* *:  */ {T_T,  T_NO, T_T,  T_NO, T_I,  T_NO, T_I,  T_NO},
	/* MV: */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
};

/*
 * Type inference matrix for the negate operator.
 */
static char const negateMatrix[ILEngineType_ValidTypes] =
{
    /* I4    I8    I     F     &     O     *     MV */
     T_I4, T_I8, T_I,  T_F,  T_NO, T_NO, T_NO, T_NO,
};

/*
 * Type inference matrix for the bitwise NOT operator.
 */
static char const notMatrix[ILEngineType_ValidTypes] =
{
    /* I4    I8    I     F     &     O     *     MV */
     T_I4, T_I8, T_I, T_NO,  T_NO, T_NO, T_NO, T_NO,
};

/*
 * Helper macros for getting the types of the top two
 * stack elements for binary and unary operators.
 */
#define	STK_BINARY_1		(stack[stackSize - 2].engineType)
#define	STK_TYPEINFO_1		(stack[stackSize - 2].typeInfo)
#define	STK_BINARY_2		(stack[stackSize - 1].engineType)
#define	STK_TYPEINFO_2		(stack[stackSize - 1].typeInfo)
#define	STK_UNARY			(stack[stackSize - 1].engineType)
#define	STK_UNARY_TYPEINFO	(stack[stackSize - 1].typeInfo)

#elif defined(IL_VERIFY_LOCALS)

ILEngineType commonType;

#else /* IL_VERIFY_CODE */

case IL_OP_ADD:
case IL_OP_ADD_OVF_UN:
{
	/* Addition operators that may involve pointers */
	if(unsafeAllowed)
	{
		commonType = addUnsafeMatrix[STK_BINARY_1][STK_BINARY_2];
	}
	else
	{
		commonType = binaryNumericMatrix
						[STK_BINARY_1][STK_BINARY_2];
	}
	if(commonType == ILEngineType_F && opcode == IL_OP_ADD_OVF_UN)
	{
		/* Cannot use float values with overflow instructions */
		VERIFY_TYPE_ERROR();
	}
	else if(commonType == ILEngineType_M ||
	        commonType == ILEngineType_T)
	{
		ILCoderBinaryPtr(coder, opcode, STK_BINARY_1, STK_BINARY_2);
		if(STK_BINARY_1 == ILEngineType_M ||
		   STK_BINARY_1 == ILEngineType_T)
		{
			STK_BINARY_1 = commonType;
		}
		else
		{
			STK_BINARY_1 = commonType;
			STK_TYPEINFO_1 = STK_TYPEINFO_2;
		}
		--stackSize;
	}
	else if(commonType != ILEngineType_Invalid)
	{
		ILCoderBinary(coder, opcode, STK_BINARY_1, STK_BINARY_2);
		STK_BINARY_1 = commonType;
		STK_TYPEINFO_1 = 0;
		--stackSize;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_SUB:
case IL_OP_SUB_OVF_UN:
{
	/* Subtraction operators that may involve pointers */
	if(unsafeAllowed)
	{
		commonType = subUnsafeMatrix[STK_BINARY_1][STK_BINARY_2];
	}
	else
	{
		commonType = binaryNumericMatrix
						[STK_BINARY_1][STK_BINARY_2];
	}
	if(commonType == ILEngineType_F && opcode == IL_OP_SUB_OVF_UN)
	{
		/* Cannot use float values with overflow instructions */
		VERIFY_TYPE_ERROR();
	}
	else if(commonType == ILEngineType_M ||
	        commonType == ILEngineType_T)
	{
		ILCoderBinaryPtr(coder, opcode, STK_BINARY_1, STK_BINARY_2);
		STK_BINARY_1 = commonType;
		--stackSize;
	}
	else if(commonType != ILEngineType_Invalid)
	{
		ILCoderBinary(coder, opcode, STK_BINARY_1, STK_BINARY_2);
		STK_BINARY_1 = commonType;
		STK_TYPEINFO_1 = 0;
		--stackSize;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_MUL:
case IL_OP_DIV:
case IL_OP_REM:
{
	/* Arithmetic operators that do not involve pointers */
	commonType = binaryNumericMatrix[STK_BINARY_1][STK_BINARY_2];
	if(commonType != ILEngineType_Invalid)
	{
		ILCoderBinary(coder, opcode, STK_BINARY_1, STK_BINARY_2);
		STK_BINARY_1 = commonType;
		STK_TYPEINFO_1 = 0;
		--stackSize;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_DIV_UN:
case IL_OP_REM_UN:
case IL_OP_ADD_OVF:
case IL_OP_SUB_OVF:
case IL_OP_MUL_OVF:
case IL_OP_MUL_OVF_UN:
case IL_OP_AND:
case IL_OP_OR:
case IL_OP_XOR:
{
	/* Arithmetic operators that only apply to integers */
	commonType = binaryIntegerMatrix[STK_BINARY_1][STK_BINARY_2];
	if(commonType != ILEngineType_Invalid)
	{
		ILCoderBinary(coder, opcode, STK_BINARY_1, STK_BINARY_2);
		STK_BINARY_1 = commonType;
		STK_TYPEINFO_1 = 0;
		--stackSize;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_SHL:
case IL_OP_SHR:
case IL_OP_SHR_UN:
{
	/* Shift operators */
	commonType = binaryShiftMatrix[STK_BINARY_1][STK_BINARY_2];
	if(commonType != ILEngineType_Invalid)
	{
		ILCoderShift(coder, opcode, STK_BINARY_1, STK_BINARY_2);
		STK_BINARY_1 = commonType;
		STK_TYPEINFO_1 = 0;
		--stackSize;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_NEG:
{
	/* Negate operator */
	commonType = negateMatrix[STK_UNARY];
	if(commonType != ILEngineType_Invalid)
	{
		ILCoderUnary(coder, opcode, STK_UNARY);
		STK_UNARY = commonType;
		STK_UNARY_TYPEINFO = 0;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_NOT:
{
	/* Bitwise not operator */
	commonType = notMatrix[STK_UNARY];
	if(commonType != ILEngineType_Invalid)
	{
		ILCoderUnary(coder, opcode, STK_UNARY);
		STK_UNARY = commonType;
		STK_UNARY_TYPEINFO = 0;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_CKFINITE:
{
	/* Check whether a floating point value is finite or not */
	if(STK_UNARY == ILEngineType_F)
	{
		ILCoderUnary(coder, opcode, STK_UNARY);
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

#endif /* IL_VERIFY_CODE */
