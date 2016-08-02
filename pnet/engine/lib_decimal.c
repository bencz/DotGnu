/*
 * decimal.c - Internalcall methods for "System.Decimal".
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

#include "engine.h"
#include "il_decimal.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Rounding mode to use for all operations below.
 */
#define	DECIMAL_ROUND_MODE		IL_DECIMAL_ROUND_HALF_EVEN

/*
 * Throw a decimal overflow exception.
 */
static void ThrowDecimalOverflow(ILExecThread *thread)
{
	ILExecThreadCallNamed(thread, "System.Decimal", "ThrowOverflow",
						  "()V", (void *)0);
}

/*
 * Throw a decimal division by zero exception.
 */
static void ThrowDecimalDivZero(ILExecThread *thread)
{
	ILExecThreadCallNamed(thread, "System.Decimal", "ThrowDivZero",
						  "()V", (void *)0);
}

/*
 * public Decimal(float value);
 */
void _IL_Decimal_ctor_f(ILExecThread *thread, ILDecimal *_this, ILFloat value)
{
	if(!ILDecimalFromFloat(_this, value))
	{
		ThrowDecimalOverflow(thread);
	}
}

/*
 * public Decimal(double value);
 */
void _IL_Decimal_ctor_d(ILExecThread *thread, ILDecimal *_this, ILDouble value)
{
	if(!ILDecimalFromDouble(_this, value))
	{
		ThrowDecimalOverflow(thread);
	}
}

/*
 * public static float ToSingle(decimal value);
 */
ILFloat _IL_Decimal_ToSingle(ILExecThread *thread, ILDecimal *value)
{
	return ILDecimalToFloat(value);
}

/*
 * public static double ToDouble(decimal value);
 */
ILDouble _IL_Decimal_ToDouble(ILExecThread *thread, ILDecimal *value)
{
	return ILDecimalToDouble(value);
}

/*
 * public static decimal Add(decimal x, decimal y);
 */
void _IL_Decimal_Add(ILExecThread *thread, ILDecimal *result,
				     ILDecimal *valuea, ILDecimal *valueb)
{
	if(!ILDecimalAdd(result, valuea, valueb, DECIMAL_ROUND_MODE))
	{
		ThrowDecimalOverflow(thread);
	}
}

/*
 * public static int Compare(decimal x, decimal y);
 */
ILInt32 _IL_Decimal_Compare(ILExecThread *thread, ILDecimal *valuea,
				            ILDecimal *valueb)
{
	return ILDecimalCmp(valuea, valueb);
}

/*
 * public static decimal Divide(decimal x, decimal y);
 */
void _IL_Decimal_Divide(ILExecThread *thread, ILDecimal *result,
				        ILDecimal *valuea, ILDecimal *valueb)
{
	int divResult;
	divResult = ILDecimalDiv(result, valuea, valueb, DECIMAL_ROUND_MODE);
	if(!divResult)
	{
		ThrowDecimalDivZero(thread);
	}
	else if(divResult < 0)
	{
		ThrowDecimalOverflow(thread);
	}
}

/*
 * public static decimal Floor(decimal x);
 */
void _IL_Decimal_Floor(ILExecThread *thread, ILDecimal *result,
					   ILDecimal *value)
{
	ILDecimalFloor(result, value);
}

/*
 * public static decimal Remainder(decimal x, decimal y);
 */
void _IL_Decimal_Remainder(ILExecThread *thread, ILDecimal *result,
				           ILDecimal *valuea, ILDecimal *valueb)
{
	int divResult;
	divResult = ILDecimalRem(result, valuea, valueb, DECIMAL_ROUND_MODE);
	if(!divResult)
	{
		ThrowDecimalDivZero(thread);
	}
	else if(divResult < 0)
	{
		ThrowDecimalOverflow(thread);
	}
}

/*
 * public static decimal Multiply(decimal x, decimal y);
 */
void _IL_Decimal_Multiply(ILExecThread *thread, ILDecimal *result,
				          ILDecimal *valuea, ILDecimal *valueb)
{
	if(!ILDecimalMul(result, valuea, valueb, DECIMAL_ROUND_MODE))
	{
		ThrowDecimalOverflow(thread);
	}
}

/*
 * public static decimal Negate(decimal x);
 */
void _IL_Decimal_Negate(ILExecThread *thread, ILDecimal *result,
					    ILDecimal *value)
{
	ILDecimalNeg(result, value);
}

/*
 * public static decimal Round(decimal x, int decimals);
 */
void _IL_Decimal_Round(ILExecThread *thread, ILDecimal *result,
					   ILDecimal *value, ILInt32 decimals)
{
	if(decimals < 0 || decimals > 28)
	{
		ILExecThreadCallNamed(thread, "System.Decimal", "ThrowDecimals",
							  "()V", (void *)0);
	}
	else if(!ILDecimalRound(result, value, decimals, DECIMAL_ROUND_MODE))
	{
		ThrowDecimalOverflow(thread);
	}
}

/*
 * public static decimal Subtract(decimal x, decimal y);
 */
void _IL_Decimal_Subtract(ILExecThread *thread, ILDecimal *result,
				          ILDecimal *valuea, ILDecimal *valueb)
{
	if(!ILDecimalSub(result, valuea, valueb, DECIMAL_ROUND_MODE))
	{
		ThrowDecimalOverflow(thread);
	}
}

/*
 * public static decimal Truncate(decimal x);
 */
void _IL_Decimal_Truncate(ILExecThread *thread, ILDecimal *result,
					      ILDecimal *value)
{
	ILDecimalTruncate(result, value);
}

#ifdef	__cplusplus
};
#endif
