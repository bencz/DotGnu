/*
 * il_decimal.h - Operations on the "decimal" type.
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

#ifndef	_IL_DECIMAL_H
#define	_IL_DECIMAL_H

#include "il_values.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Representation of the decimal type.
 */
typedef struct
{
	ILUInt32		flags;
	ILUInt32		high;
	ILUInt32		middle;
	ILUInt32		low;

} ILDecimal;

/*
 * Permitted decimal rounding modes.
 */
#define	IL_DECIMAL_ROUND_DOWN			0
#define	IL_DECIMAL_ROUND_HALF_UP		1
#define	IL_DECIMAL_ROUND_HALF_EVEN		2

/*
 * Add two decimal numbers.
 */
int ILDecimalAdd(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode);

/*
 * Subtract two decimal numbers.
 */
int ILDecimalSub(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode);

/*
 * Multiply two decimal numbers.
 */
int ILDecimalMul(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode);

/*
 * Divide two decimal numbers.
 */
int ILDecimalDiv(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode);

/*
 * Compute the remainder from dividing two decimal numbers.
 */
int ILDecimalRem(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode);

/*
 * Negate a decimal number.
 */
void ILDecimalNeg(ILDecimal *result, const ILDecimal *value);

/*
 * Compare two decimal numbers.
 */
int ILDecimalCmp(const ILDecimal *valuea, const ILDecimal *valueb);

/*
 * Determine if a decimal number is zero.
 */
int ILDecimalIsZero(const ILDecimal *value);

/*
 * Compute the floor of a decimal number.
 */
void ILDecimalFloor(ILDecimal *result, const ILDecimal *value);

/*
 * Truncate a decimal number to its integer component.
 */
void ILDecimalTruncate(ILDecimal *result, const ILDecimal *value);

/*
 * Round a decial number to a particular number of places.
 */
int ILDecimalRound(ILDecimal *result, const ILDecimal *value,
				   int places, int roundMode);

/*
 * Parse the string representation of a decimal number.
 */
int ILDecimalParse(ILDecimal *value, const char *str, int roundMode);

/*
 * Format a decimal number to an ASCII string buffer.
 * If "source" is non-zero, then format the value in
 * such a way that it can be written out as C# source.
 * i.e. it must include a decimal point, and end in 'm'.
 */
void ILDecimalFormat(char *buffer, const ILDecimal *value, int source);

/*
 * Convert numeric values of various sizes into a decimal number.
 */
void ILDecimalFromInt32(ILDecimal *value, ILInt32 intValue);
void ILDecimalFromUInt32(ILDecimal *value, ILUInt32 intValue);
void ILDecimalFromInt64(ILDecimal *value, ILInt64 intValue);
void ILDecimalFromUInt64(ILDecimal *value, ILUInt64 intValue);
int  ILDecimalFromFloat(ILDecimal *value, ILFloat floatValue);
int  ILDecimalFromDouble(ILDecimal *value, ILDouble floatValue);

/*
 * Convert a decimal value into an integer value.
 * Returns zero if not an integer in the correct range.
 */
int ILDecimalToInt64(const ILDecimal *value, ILInt64 *intValue);
int ILDecimalToUInt64(const ILDecimal *value, ILUInt64 *intValue);

/*
 * Convert a decimal value into a floating point value.
 */
ILFloat  ILDecimalToFloat(const ILDecimal *value);
ILDouble ILDecimalToDouble(const ILDecimal *value);

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_DECIMAL_H */
