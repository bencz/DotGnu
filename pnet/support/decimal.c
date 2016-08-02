/*
 * decimal.c - Operations on the "decimal" type.
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

#include "il_decimal.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Determine if a decimal value is negative.
 */
#define	DECIMAL_IS_NEG(value)	\
			(((value)->flags & (ILUInt32)0x80000000) != 0)

/*
 * Get the decimal point position from a decimal value.
 */
#define	DECIMAL_GETPT(value)	\
			((int)(((value)->flags >> 16) & (ILUInt32)0xFF))

/*
 * Make a "flags" value from a sign and a decimal point position.
 */
#define	DECIMAL_MKFLAGS(sign,decpt)	\
			((((ILUInt32)(decpt)) << 16) | \
				((sign) ? (ILUInt32)0x80000000 : (ILUInt32)0))

/*
 * Divide a value by ten, returning the result and a remainder.
 */
static int DivByTen(ILUInt32 *result, ILUInt32 *value, int size)
{
	ILUInt32 remainder;
	ILUInt64 product;
	int posn;
	remainder = 0;
	for(posn = 0; posn < size; ++posn)
	{
		product = (((ILUInt64)remainder) << 32) + ((ILUInt64)(value[posn]));
		result[posn] = (ILUInt32)(product / (ILUInt64)10);
		remainder = (ILUInt32)(product % (ILUInt64)10);
	}
	return (int)remainder;
}

/*
 * Normalize a decimal value and return the new position
 * of the decimal point within the representation.  Returns
 * -1 if an overflow has occurred.
 */
static int Normalize(ILUInt32 *value, int size, int decpt, int roundMode)
{
	ILUInt32 intermediate[6];
	int remainder, temp, carry;

	/* Strip leading zeros */
	while(size > 0 && *value == 0)
	{
		++value;
		--size;
	}

	/* If the result is zero, then bail out now */
	if(!size)
	{
		return 0;
	}

	/* Keep dividing by 10 until the size is 3 or less, or
	   until the decimal point returns to a useful position */
	remainder = 0;
	while((size > 3 && decpt > 0) || decpt > 28)
	{
		remainder = DivByTen(intermediate, value, size);
		--decpt;
		for(temp = 0; temp < size; ++temp)
		{
			value[temp] = intermediate[temp];
		}
		while(size > 0 && *value == 0)
		{
			++value;
			--size;
		}
	}

	/* We need at least 3 words when rounding */
	if(size < 3)
	{
		value -= (3 - size);
		size = 3;
	}

	/* Round the value according to the rounding mode */
	if((roundMode == IL_DECIMAL_ROUND_HALF_UP && remainder >= 5) ||
	   (roundMode == IL_DECIMAL_ROUND_HALF_EVEN && remainder > 5) ||
	   (roundMode == IL_DECIMAL_ROUND_HALF_EVEN && remainder == 5 &&
	    (value[size - 1] & ((ILUInt32)1)) != 0))
	{
		/* Perform the rounding operation */
		carry = 1;
		for(temp = size - 1; temp >= 0; --temp)
		{
			if((value[temp] += 1) != (ILUInt32)0)
			{
				carry = 0;
				break;
			}
		}

		/* If we have a carry out, then we must divide by 10 again.
		   If the decimal point is already at the right-most
		   position, then the mantissa has overflowed */
		if(carry)
		{
			if(decpt <= 0)
			{
				return -1;
			}
			remainder = DivByTen(intermediate, value, size);
			--decpt;
			for(temp = 0; temp < size; ++temp)
			{
				value[temp] = intermediate[temp];
			}
			while(size > 0 && *value == 0)
			{
				++value;
				--size;
			}
		}
	}

	/* Remove trailing zeroes from the fractional part */
	while(decpt > 0 && size > 0)
	{
		remainder = DivByTen(intermediate, value, size);
		if(remainder != 0)
		{
			break;
		}
		--decpt;
		for(temp = 0; temp < size; ++temp)
		{
			value[temp] = intermediate[temp];
		}
		while(size > 0 && *value == 0)
		{
			++value;
			--size;
		}
	}

	/* If we have more than 3 words, then the result is too big */
	if(size > 3)
	{
		return -1;
	}

	/* If the size is zero, then the answer was rounded down to zero */
	if(!size)
	{
		return 0;
	}

	/* Done */
	return decpt;
}

/*
 * Multiply a 192-bit value by power of ten.  Returns non-zero on carry out.
 */
static int MulByPowOfTen(ILUInt32 *value, int power, ILUInt32 digit)
{
	static ILUInt32 const powersOf10[9] = {
		10,
		100,
		1000,
		10000,
		100000,
		1000000,
		10000000,
		100000000,
		1000000000
	};
	ILUInt32 temp[6];
	ILUInt64 multiplier;
	ILUInt64 product;
	ILUInt32 prev;
	int posn;

	while(power > 0)
	{
		/* Get the multiplier to use on this iteration */
		if(power > 9)
		{
			multiplier = (ILUInt64)(powersOf10[8]);
			power -= 9;
		}
		else
		{
			multiplier = (ILUInt64)(powersOf10[power - 1]);
			power = 0;
		}

		/* Compute "value * multiplier" and put it into "temp" */
		product = (ILUInt64)digit;
		temp[0] = temp[1] = temp[2] = temp[3] = temp[4] = temp[5] = 0;
		for(posn = 5; posn >= 0; --posn)
		{
			product += ((ILUInt64)(value[posn])) * multiplier;
			prev = temp[posn];
			if((temp[posn] += (ILUInt32)product) < prev)
			{
				product = ((product >> 32) + 1);
			}
			else
			{
				product >>= 32;
			}
		}
		if(product != 0)
		{
			return 1;
		}

		/* Copy "temp" into "value" */
		value[0] = temp[0];
		value[1] = temp[1];
		value[2] = temp[2];
		value[3] = temp[3];
		value[4] = temp[4];
		value[5] = temp[5];
	}
	return 0;
}

/*
 * Compare the absolute magnitude of two decimal values.
 */
static int CmpAbs(const ILDecimal *valuea, const ILDecimal *valueb, int sign)
{
	ILUInt32 tempa[6];
	ILUInt32 tempb[6];
	int decpta, decptb;
	int posn;

	/* Load "valuea" and "valueb" into 192-bit temporary registers */
	tempa[0] = tempa[1] = tempa[2] = 0;
	tempa[3] = valuea->high;
	tempa[4] = valuea->middle;
	tempa[5] = valuea->low;
	decpta = DECIMAL_GETPT(valuea);
	tempb[0] = tempb[1] = tempb[2] = 0;
	tempb[3] = valueb->high;
	tempb[4] = valueb->middle;
	tempb[5] = valueb->low;
	decptb = DECIMAL_GETPT(valueb);

	/* Adjust for the decimal point positions */
	if(decpta < decptb)
	{
		/* Shift "valuea" up until the decimal points align */
		MulByPowOfTen(tempa, (decptb - decpta), 0);
	}
	else if(decpta > decptb)
	{
		/* Shift "valueb" up until the decimal points align */
		MulByPowOfTen(tempb, (decpta - decptb), 0);
	}

	/* Compare the 192-bit adjusted values */
	for(posn = 0; posn < 6; ++posn)
	{
		if(tempa[posn] > tempb[posn])
		{
			return (sign ? -1 : 1);
		}
		else if(tempa[posn] < tempb[posn])
		{
			return (sign ? 1 : -1);
		}
	}
	return 0;
}

/*
 * Add two 96-bit decimal values to get a 192-bit result.
 * Return the new position of the decimal point.
 */
static int AddValues(ILUInt32 *tempa, const ILDecimal *valuea,
					 const ILDecimal *valueb)
{
	ILUInt32 tempb[6];
	ILUInt32 prev;
	int posn, carry;
	int decpta, decptb;

	/* Load "valuea" and "valueb" into 192-bit temporary registers */
	tempa[0] = tempa[1] = tempa[2] = 0;
	tempa[3] = valuea->high;
	tempa[4] = valuea->middle;
	tempa[5] = valuea->low;
	decpta = DECIMAL_GETPT(valuea);
	tempb[0] = tempb[1] = tempb[2] = 0;
	tempb[3] = valueb->high;
	tempb[4] = valueb->middle;
	tempb[5] = valueb->low;
	decptb = DECIMAL_GETPT(valueb);

	/* Adjust for the decimal point positions */
	if(decpta < decptb)
	{
		/* Shift "valuea" up until the decimal points align */
		MulByPowOfTen(tempa, (decptb - decpta), 0);
		decpta = decptb;
	}
	else if(decpta > decptb)
	{
		/* Shift "valueb" up until the decimal points align */
		MulByPowOfTen(tempb, (decpta - decptb), 0);
		decptb = decpta;
	}

	/* Add the two values */
	carry = 0;
	for(posn = 5; posn >= 0; --posn)
	{
		prev = tempa[posn];
		if(carry)
		{
			if((tempa[posn] += tempb[posn] + 1) <= prev)
			{
				carry = 1;
			}
			else
			{
				carry = 0;
			}
		}
		else
		{
			if((tempa[posn] += tempb[posn]) < prev)
			{
				carry = 1;
			}
			else
			{
				carry = 0;
			}
		}
	}
	return decpta;
}

/*
 * Subtract two 96-bit decimal values to get a 192-bit result.
 * Return the new position of the decimal point.  "valuea" will
 * always be greater than "valueb" on entry.
 */
static int SubValues(ILUInt32 *tempa, const ILDecimal *valuea,
					 const ILDecimal *valueb)
{
	ILUInt32 tempb[6];
	ILUInt32 prev;
	int posn, carry;
	int decpta, decptb;

	/* Load "valuea" and "valueb" into 192-bit temporary registers */
	tempa[0] = tempa[1] = tempa[2] = 0;
	tempa[3] = valuea->high;
	tempa[4] = valuea->middle;
	tempa[5] = valuea->low;
	decpta = DECIMAL_GETPT(valuea);
	tempb[0] = tempb[1] = tempb[2] = 0;
	tempb[3] = valueb->high;
	tempb[4] = valueb->middle;
	tempb[5] = valueb->low;
	decptb = DECIMAL_GETPT(valueb);

	/* Adjust for the decimal point positions */
	if(decpta < decptb)
	{
		/* Shift "valuea" up until the decimal points align */
		MulByPowOfTen(tempa, (decptb - decpta), 0);
		decpta = decptb;
	}
	else if(decpta > decptb)
	{
		/* Shift "valueb" up until the decimal points align */
		MulByPowOfTen(tempb, (decpta - decptb), 0);
		decptb = decpta;
	}

	/* Subtract the two values */
	carry = 0;
	for(posn = 5; posn >= 0; --posn)
	{
		prev = tempa[posn];
		if(carry)
		{
			if((tempa[posn] -= tempb[posn] + 1) >= prev)
			{
				carry = 1;
			}
			else
			{
				carry = 0;
			}
		}
		else
		{
			if((tempa[posn] -= tempb[posn]) > prev)
			{
				carry = 1;
			}
			else
			{
				carry = 0;
			}
		}
	}
	return decpta;
}

int ILDecimalAdd(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode)
{
	ILUInt32 temp[6];
	int decpt;
	int sign;

	/* Determine how to perform the addition */
	if(!DECIMAL_IS_NEG(valuea) && !DECIMAL_IS_NEG(valueb))
	{
		/* Both values are positive */
		decpt = AddValues(temp, valuea, valueb);
		sign = 0;
	}
	else if(DECIMAL_IS_NEG(valuea) && DECIMAL_IS_NEG(valueb))
	{
		/* Both values are negative */
		decpt = AddValues(temp, valuea, valueb);
		sign = 1;
	}
	else if(DECIMAL_IS_NEG(valuea))
	{
		/* The first value is negative, and the second is positive */
		if(CmpAbs(valuea, valueb, 0) >= 0)
		{
			/* Negative result */
			decpt = SubValues(temp, valuea, valueb);
			sign = 1;
		}
		else
		{
			/* Positive result */
			decpt = SubValues(temp, valueb, valuea);
			sign = 0;
		}
	}
	else
	{
		/* The first value is positive, and the second is negative */
		if(CmpAbs(valuea, valueb, 0) <= 0)
		{
			/* Negative result */
			decpt = SubValues(temp, valueb, valuea);
			sign = 1;
		}
		else
		{
			/* Positive result */
			decpt = SubValues(temp, valuea, valueb);
			sign = 0;
		}
	}

	/* Normalize and return the result to the caller */
	decpt = Normalize(temp, 6, decpt, roundMode);
	if(decpt < 0)
	{
		return 0;
	}
	result->high = temp[3];
	result->middle = temp[4];
	result->low = temp[5];
	if(result->high == 0 && result->middle == 0 && result->low == 0)
	{
		/* The sign of zero must always be positive */
		sign = 0;
	}
	result->flags = DECIMAL_MKFLAGS(sign, decpt);
	return 1;
}

int ILDecimalSub(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode)
{
	ILUInt32 temp[6];
	int decpt;
	int sign;

	/* Determine how to perform the subtraction */
	if(!DECIMAL_IS_NEG(valuea) && DECIMAL_IS_NEG(valueb))
	{
		/* The first value is positive, and the second is negative */
		decpt = AddValues(temp, valuea, valueb);
		sign = 0;
	}
	else if(DECIMAL_IS_NEG(valuea) && !DECIMAL_IS_NEG(valueb))
	{
		/* The first value is negative, and the second is positive */
		decpt = AddValues(temp, valuea, valueb);
		sign = 1;
	}
	else if(DECIMAL_IS_NEG(valuea))
	{
		/* Both values are negative */
		if(CmpAbs(valuea, valueb, 0) >= 0)
		{
			/* Negative result */
			decpt = SubValues(temp, valuea, valueb);
			sign = 1;
		}
		else
		{
			/* Positive result */
			decpt = SubValues(temp, valueb, valuea);
			sign = 0;
		}
	}
	else
	{
		/* Both values are positive */
		if(CmpAbs(valuea, valueb, 0) <= 0)
		{
			/* Negative result */
			decpt = SubValues(temp, valueb, valuea);
			sign = 1;
		}
		else
		{
			/* Positive result */
			decpt = SubValues(temp, valuea, valueb);
			sign = 0;
		}
	}

	/* Normalize and return the result to the caller */
	decpt = Normalize(temp, 6, decpt, roundMode);
	if(decpt < 0)
	{
		return 0;
	}
	result->high = temp[3];
	result->middle = temp[4];
	result->low = temp[5];
	if(result->high == 0 && result->middle == 0 && result->low == 0)
	{
		/* The sign of zero must always be positive */
		sign = 0;
	}
	result->flags = DECIMAL_MKFLAGS(sign, decpt);
	return 1;
}

/*
 * Multiply a value by a single word and add it to an accumulated result.
 */
static void MulByWord(ILUInt32 *result, int base,
					  const ILDecimal *valuea, ILUInt32 valueb)
{
	ILUInt64 product;

	/* valuea->low * valueb */
	product = ((ILUInt64)(valuea->low)) * ((ILUInt64)valueb);
	product += (ILUInt64)(result[base]);
	result[base] = (ILUInt32)product;
	product >>= 32;
	--base;

	/* valuea->middle * valueb */
	product += ((ILUInt64)(valuea->middle)) * ((ILUInt64)valueb);
	product += (ILUInt64)(result[base]);
	result[base] = (ILUInt32)product;
	product >>= 32;
	--base;

	/* valuea->high * valueb */
	product += ((ILUInt64)(valuea->high)) * ((ILUInt64)valueb);
	product += (ILUInt64)(result[base]);
	result[base] = (ILUInt32)product;
	product >>= 32;
	--base;

	/* Propagate the carry */
	while(base >= 0 && product != 0)
	{
		product += (ILUInt64)(result[base]);
		result[base] = (ILUInt32)product;
		product >>= 32;
		--base;
	}
}

int ILDecimalMul(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode)
{
	ILUInt32 temp[6];
	int decpt;
	int sign;

	/* Calculate the intermediate result */
	temp[0] = temp[1] = temp[2] = temp[3] = temp[4] = temp[5] = 0;
	MulByWord(temp, 5, valuea, valueb->low);
	MulByWord(temp, 4, valuea, valueb->middle);
	MulByWord(temp, 3, valuea, valueb->high);

	/* Build the result value */
	sign = (DECIMAL_IS_NEG(valuea) ^ DECIMAL_IS_NEG(valueb));
	decpt = Normalize(temp, 6, DECIMAL_GETPT(valuea) + DECIMAL_GETPT(valueb),
					  roundMode);
	if(decpt < 0)
	{
		return 0;
	}
	result->high = temp[3];
	result->middle = temp[4];
	result->low = temp[5];
	if(result->high == 0 && result->middle == 0 && result->low == 0)
	{
		/* The sign of zero must always be positive */
		sign = 0;
	}
	result->flags = DECIMAL_MKFLAGS(sign, decpt);
	return 1;
}

/*
 * Shift a value left by a number of bits.
 */
static void ShiftLeft(ILUInt32 *value, int size, int shift)
{
	ILUInt64 carry = 0;
	while(size > 0)
	{
		--size;
		carry |= (((ILUInt64)(value[size])) << shift);
		value[size] = (ILUInt32)carry;
		carry >>= 32;
	}
}

/*
 * Compute "valuea -= valueb * valuec".  Returns non-zero
 * if the result went negative.  We assume that sizeb is
 * greater than 0 and less than sizea.
 */
static int MulAndSub(ILUInt32 *valuea, int sizea,
					 ILUInt32 *valueb, int sizeb,
					 ILUInt32 valuec)
{
	int posn;
	ILUInt64 carry = 0;
	ILUInt64 valuec64 = (ILUInt64)valuec;
	ILUInt32 temp;
	for(posn = sizeb; posn > 0; --posn)
	{
		carry += ((ILUInt64)(valueb[posn - 1])) * valuec64;
		temp = valuea[posn] - (ILUInt32)carry;
		if(temp > valuea[posn])
		{
			carry = ((carry >> 32) + 1);
		}
		else
		{
			carry >>= 32;
		}
		valuea[posn] = temp;
	}
	/* process word 0 */
	temp = valuea[0] - (ILUInt32)carry;
	if(temp > valuea[0])
	{
		carry = ((carry >> 32) + 1);
	}
	else
	{
		carry >>= 32;
	}
	valuea[0] = temp;
	return (carry != 0);
}

/*
 * Compute "valuea += valueb".  Returns non-zero if
 * the result is still negative.  We assume that sizeb
 * is greater than 0 and less than sizea.
 */
static int AddBack(ILUInt32 *valuea, int sizea,
				   ILUInt32 *valueb, int sizeb)
{
	int posn;
	ILUInt64 carry = 0;
	for(posn = sizeb; posn > 0; --posn)
	{
		carry += (((ILUInt64)(valuea[posn])) +
				  ((ILUInt64)(valueb[posn - 1])));
		valuea[posn] = (ILUInt32)carry;
		carry >>= 32;
	}
	/* process word 0 */
	carry += (ILUInt64)valuea[0];
	valuea[0] = (ILUInt32)carry;
	carry >>= 32;
	return (carry != 0);
}

/*
 * Divide valuea by valueb giving a 228-bit quotient.
 * Returns zero if attempting to divide by zero.
 */
static int Divide(ILUInt32 *quotient, const ILDecimal *valuea,
				  const ILDecimal *valueb)
{
	ILUInt32 tempa[7];
	ILUInt32 tempb[3];
	int bsize, shift;
	int posn, isneg, limit;
	ILUInt32 bittest;
	ILUInt32 testquot;

	/* Expand valuea to a 228-bit value, shifted up by 29 places */
	tempa[0] = tempa[1] = tempa[2] = tempa[3] = 0;
	tempa[4] = valuea->high;
	tempa[5] = valuea->middle;
	tempa[6] = valuea->low;
	MulByPowOfTen(tempa + 1, 29, 0);

	/* Convert valueb into its intermediate form */
	if(valueb->high != 0)
	{
		tempb[0] = valueb->high;
		tempb[1] = valueb->middle;
		tempb[2] = valueb->low;
		bsize = 3;
	}
	else if(valueb->middle != 0)
	{
		tempb[0] = valueb->middle;
		tempb[1] = valueb->low;
		bsize = 2;
	}
	else if(valueb->low != 0)
	{
		tempb[0] = valueb->low;
		bsize = 1;
	}
	else
	{
		return 0;
	}

	/* Shift tempa and tempb so that tempb[0] >= 0x80000000 */
	shift = 0;
	bittest = tempb[0];
	while((bittest & (ILUInt32)0x80000000) == 0)
	{
		++shift;
		bittest <<= 1;
	}
	if(shift != 0)
	{
		ShiftLeft(tempa, 7, shift);
		ShiftLeft(tempb, bsize, shift);
	}

	/* Perform the division */
	limit = 7 - bsize;
	for(posn = 0; posn < bsize; ++posn)
	{
		quotient[posn] = 0;
	}
	for(posn = 0; posn < limit; ++posn)
	{
		/* Get the test quotient for the current word */
		if(tempa[posn] >= tempb[0])
		{
			testquot = (ILUInt32)0xFFFFFFFF;
		}
		else if(posn < 6)
		{
			testquot = (ILUInt32)
				(((((ILUInt64)(tempa[posn])) << 32) |
					((ILUInt64)(tempa[posn + 1]))) / (ILUInt64)(tempb[0]));
		}
		else
		{
			testquot = (ILUInt32)
				((((ILUInt64)(tempa[posn])) << 32) / (ILUInt64)(tempb[0]));
		}

		/* Multiply tempb by testquot and subtract from tempa */
		isneg = MulAndSub(tempa + posn, 7 - posn, tempb, bsize, testquot);

		/* Add back if the result went negative.  This loop will
		   iterate for no more than 2 iterations */
		while(isneg)
		{
			--testquot;
			isneg = AddBack(tempa + posn, 7 - posn, tempb, bsize);
		}

		/* Set the current quotient word to the test quotient */
		quotient[posn + bsize] = testquot;
	}

	/* Done */
	return 1;
}

int ILDecimalDiv(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode)
{
	ILUInt32 quotient[7];
	int decpt, sign;

	/* Compute the 228-bit quotient of the fractional parts */
	if(!Divide(quotient, valuea, valueb))
	{
		/* Division by zero error */
		return 0;
	}

	/* Normalize and return the result */
	sign = (DECIMAL_IS_NEG(valuea) ^ DECIMAL_IS_NEG(valueb));
	decpt = Normalize(quotient, 7,
					  DECIMAL_GETPT(valuea) - DECIMAL_GETPT(valueb) + 29,
					  roundMode);
	if(decpt < 0)
	{
		return 0;
	}
	result->high = quotient[4];
	result->middle = quotient[5];
	result->low = quotient[6];
	if(result->high == 0 && result->middle == 0 && result->low == 0)
	{
		/* The sign of zero must always be positive */
		sign = 0;
	}
	result->flags = DECIMAL_MKFLAGS(sign, decpt);
	return 1;
}

int ILDecimalRem(ILDecimal *result, const ILDecimal *valuea,
				 const ILDecimal *valueb, int roundMode)
{
	ILDecimal quotient;

	/* Divide valuea by valueb to get the quotient */
	if(!ILDecimalDiv(&quotient, valuea, valueb, roundMode))
	{
		return 0;
	}

	/* Truncate the quotient to get its integer part */
	ILDecimalTruncate(&quotient, &quotient);

	/* Compute "valuea - valueb * quotient" to get the remainder */
	if(!ILDecimalMul(&quotient, valueb, &quotient, roundMode))
	{
		return 0;
	}
	return ILDecimalSub(result, valuea, &quotient, roundMode);
}

void ILDecimalNeg(ILDecimal *result, const ILDecimal *value)
{
	/* Invert the sign if the value is not zero (zero is always positive) */
	if(value->high != 0 || value->middle != 0 || value->low != 0)
	{
		result->flags = (value->flags ^ (ILUInt32)0x80000000);
	}
	else
	{
		result->flags = value->flags;
	}
	result->high = value->high;
	result->middle = value->middle;
	result->low = value->low;
}

int ILDecimalCmp(const ILDecimal *valuea, const ILDecimal *valueb)
{
	if(DECIMAL_IS_NEG(valuea) && !DECIMAL_IS_NEG(valueb))
	{
		return -1;
	}
	else if(!DECIMAL_IS_NEG(valuea) && DECIMAL_IS_NEG(valueb))
	{
		return 1;
	}
	else
	{
		return CmpAbs(valuea, valueb, DECIMAL_IS_NEG(valuea));
	}
}

int ILDecimalIsZero(const ILDecimal *value)
{
	return (value->high == 0 && value->middle == 0 && value->low == 0);
}

void ILDecimalFloor(ILDecimal *result, const ILDecimal *value)
{
	if(DECIMAL_IS_NEG(value))
	{
		/* The value is negative */
		if(DECIMAL_GETPT(value))
		{
			/* We have decimals, so truncate and subtract 1 */
			static ILDecimal const one = {1, 0, 0, 0};
			ILDecimalTruncate(result, value);
			ILDecimalSub(result, result, &one, IL_DECIMAL_ROUND_DOWN);
		}
		else
		{
			/* No decimals, so the result is the same as the value */
			*result = *value;
		}
	}
	else
	{
		/* The value is positive, so floor is the same as truncate */
		ILDecimalTruncate(result, value);
	}
}

void ILDecimalTruncate(ILDecimal *result, const ILDecimal *value)
{
	ILUInt32 tempa[3];
	ILUInt32 tempb[3];
	int decpt = DECIMAL_GETPT(value);
	if(decpt)
	{
		/* Expand the value to the intermediate form */
		tempa[0] = value->high;
		tempa[1] = value->middle;
		tempa[2] = value->low;

		/* Keep dividing by 10 until the decimal point position is reached */
		while(decpt > 0)
		{
			DivByTen(tempb, tempa, 3);
			tempa[0] = tempb[0];
			tempa[1] = tempb[1];
			tempa[2] = tempb[2];
			--decpt;
		}

		/* Build the result value */
		result->high = tempa[0];
		result->middle = tempa[1];
		result->low = tempa[2];
		result->flags = (value->flags & (ILUInt32)0x80000000);
	}
	else
	{
		/* No decimals, so the result is the same as the value */
		*result = *value;
	}
}

int ILDecimalRound(ILDecimal *result, const ILDecimal *value,
				   int places, int roundMode)
{
	ILUInt32 tempa[3];
	ILUInt32 tempb[3];
	int remainder, posn, carry;
	int decpt = DECIMAL_GETPT(value);

	/* Bail out early if we already have fewer than the requested places */
	if(places < 0 || decpt <= places)
	{
		*result = *value;
		return 1;
	}

	/* Keep dividing by 10 until we have the requested number of places */
	tempa[0] = value->high;
	tempa[1] = value->middle;
	tempa[2] = value->low;
	remainder = 0;
	while(decpt > places)
	{
		remainder = DivByTen(tempb, tempa, 3);
		tempa[0] = tempb[0];
		tempa[1] = tempb[1];
		tempa[2] = tempb[2];
		--decpt;
	}

	/* Round according to the rounding mode */
	if((roundMode == IL_DECIMAL_ROUND_HALF_UP && remainder >= 5) ||
	   (roundMode == IL_DECIMAL_ROUND_HALF_EVEN && remainder > 5) ||
	   (roundMode == IL_DECIMAL_ROUND_HALF_EVEN && remainder == 5 &&
	    (tempa[2] & ((ILUInt32)1)) != 0))
	{
		/* Perform the rounding operation */
		carry = 1;
		for(posn = 2; posn >= 0; --posn)
		{
			if((tempa[posn] += 1) != (ILUInt32)0)
			{
				carry = 0;
				break;
			}
		}

		/* If we have a carry out, then we must divide by 10 again.
		   If the decimal point is already at the right-most
		   position, then the mantissa has overflowed */
		if(carry)
		{
			if(decpt <= 0)
			{
				return 0;
			}
			remainder = DivByTen(tempb, tempa, 3);
			tempa[0] = tempb[0];
			tempa[1] = tempb[1];
			tempa[2] = tempb[2];
			--decpt;
		}
	}

	/* Remove trailing zeros after the decimal point */
	while(decpt > 0)
	{
		remainder = DivByTen(tempb, tempa, 3);
		if(remainder != 0)
		{
			break;
		}
		tempa[0] = tempb[0];
		tempa[1] = tempb[1];
		tempa[2] = tempb[2];
		--decpt;
	}

	/* Build and return the result to the caller */
	result->high = tempa[0];
	result->middle = tempa[1];
	result->low = tempa[2];
	if(result->high == 0 && result->middle == 0 && result->low == 0)
	{
		/* The sign of zero must always be positive */
		result->flags = 0;
	}
	else
	{
		result->flags = DECIMAL_MKFLAGS(DECIMAL_IS_NEG(value), decpt);
	}
	return 1;
}

int ILDecimalParse(ILDecimal *value, const char *str, int roundMode)
{
	ILUInt32 temp[6];
	int totalDigits;
	int decimalPlaces;
	int exponent;
	int negexp;
	int sawLeading;
	int remainder;
	int posn;
	int sign;

	/* Skip white space at the start of the number */
	while(*str == ' ' || *str == '\t' || *str == '\r' || *str == '\n' ||
	      *str == '\f' || *str == '\v' || *str == (char)0x1A) /* Ctrl-Z */
	{
		++str;
	}

	/* Parse the sign */
	if(*str == '-')
	{
		sign = 1;
		++str;
	}
	else if(*str == '+')
	{
		sign = 0;
		++str;
	}
	else
	{
		sign = 0;
	}

	/* Clear the 192-bit intermediate value */
	temp[0] = temp[1] = temp[2] = temp[3] = temp[4] = temp[5] = 0;
	totalDigits = 0;
	decimalPlaces = 0;
	exponent = 0;

	/* Parse the part before the decimal point */
	sawLeading = 0;
	while(*str >= '0' && *str <= '9')
	{
		if(sawLeading || *str != '0')
		{
			MulByPowOfTen(temp, 1, (ILUInt32)(*str - '0'));
			++str;
			++totalDigits;
			if(totalDigits > 30)
			{
				return 0;
			}
			sawLeading = 1;
		}
		else
		{
			++str;
		}
	}

	/* Parse the fractional part */
	if(*str == '.')
	{
		++str;
		while(*str >= '0' && *str <= '9')
		{
			MulByPowOfTen(temp, 1, (ILUInt32)(*str - '0'));
			++str;
			++totalDigits;
			++decimalPlaces;
			if(totalDigits > 30)
			{
				/* Round the remainder of the fractional part */
				if(*str >= '0' && *str <= '9')
				{
					remainder = (*str++ - '0');
					while(*str >= '0' && *str <= '9')
					{
						++str;
					}
					if((roundMode == IL_DECIMAL_ROUND_HALF_UP &&
								remainder >= 5) ||
					   (roundMode == IL_DECIMAL_ROUND_HALF_EVEN &&
					   			remainder > 5) ||
					   (roundMode == IL_DECIMAL_ROUND_HALF_EVEN &&
					   			remainder == 5 &&
					    		(temp[5] & ((ILUInt32)1)) != 0))
					{
						/* Add 1 to the current value to round it */
						for(posn = 5; posn >= 0; --posn)
						{
							if((temp[posn] += 1) != (ILUInt32)0)
							{
								break;
							}
						}
					}
				}
				break;
			}
		}
	}

	/* If the temporary value is still zero, then the result is zero */
	if(!(temp[0]) && !(temp[1]) && !(temp[2]) &&
	   !(temp[3]) && !(temp[4]) && !(temp[5]))
	{
		value->flags = 0;
		value->high = 0;
		value->middle = 0;
		value->low = 0;
		return 1;
	}

	/* Parse the exponent */
	if(*str == 'e' || *str == 'E')
	{
		++str;
		if(*str == '-')
		{
			negexp = 1;
			++str;
		}
		else if(*str == '+')
		{
			negexp = 0;
			++str;
		}
		else
		{
			negexp = 0;
		}
		while(*str >= '0' && *str <= '9' && exponent < 1000)
		{
			exponent = exponent * 10 + (int)(*str++ - '0');
		}
		if(negexp)
		{
			exponent = -exponent;
		}
	}

	/* Adjust the position of the decimal point by the exponent */
	decimalPlaces -= exponent;
	if(decimalPlaces < 0)
	{
		if(decimalPlaces < -29)
		{
			/* The result will definitely be too big to fit */
			return 0;
		}
		if(MulByPowOfTen(temp, -decimalPlaces, 0))
		{
			/* A carry out occurred: the result is too big */
			return 0;
		}
		decimalPlaces = 0;
	}

	/* Normalize the value */
	decimalPlaces = Normalize(temp, 6, decimalPlaces, roundMode);
	if(decimalPlaces < 0)
	{
		return 0;
	}

	/* Build the final value and return it */
	value->high = temp[3];
	value->middle = temp[4];
	value->low = temp[5];
	if(value->high == 0 && value->middle == 0 && value->low == 0)
	{
		/* The sign of zero must always be positive */
		sign = 0;
	}
	value->flags = DECIMAL_MKFLAGS(sign, decimalPlaces);
	return 1;
}

void ILDecimalFormat(char *buffer, const ILDecimal *value, int source)
{
	ILUInt32 temp[6];
	int numDigits, posn, outposn, remainder;
	int wroteDec, decpt;

	/* Output the sign */
	if(DECIMAL_IS_NEG(value))
	{
		*buffer++ = '-';
	}

	/* Find the position of the highest digit */
	temp[0] = temp[1] = temp[2] = temp[3] = temp[4] = 0;
	temp[5] = 1;
	numDigits = 0;
	while(temp[2] == 0 &&
	      (temp[3] < value->high ||
		   (temp[3] == value->high &&
		    (temp[4] < value->middle ||
			 (temp[4] == value->middle &&
			  temp[5] <= value->low)))))
	{
		MulByPowOfTen(temp, 1, 0);
		++numDigits;
	}

	/* Format the value, starting at the least significant digit */
	wroteDec = 0;
	if(numDigits)
	{
		decpt = DECIMAL_GETPT(value);
		if(numDigits <= decpt)
		{
			*buffer++ = '0';
			numDigits = decpt;
		}
		temp[0] = value->high;
		temp[1] = value->middle;
		temp[2] = value->low;
		outposn = numDigits + ((decpt > 0) ? 1 : 0);
		for(posn = numDigits - 1; posn >= 0; --posn)
		{
			remainder = DivByTen(temp + 3, temp, 3);
			temp[0] = temp[3];
			temp[1] = temp[4];
			temp[2] = temp[5];
			buffer[outposn--] = (char)('0' + remainder);
			if((numDigits - posn) == decpt)
			{
				buffer[outposn--] = '.';
				wroteDec = 1;
			}
		}
		buffer += numDigits + ((decpt > 0) ? 1 : 0);
	}
	else
	{
		*buffer++ = '0';
	}

	/* Add suffix characters if outputting in source form */
	if(source)
	{
		if(!wroteDec)
		{
			*buffer++ = '.';
			*buffer++ = '0';
		}
		*buffer++ = 'm';
	}

	/* Terminate the formatted value */
	*buffer = '\0';
}

void ILDecimalFromInt32(ILDecimal *value, ILInt32 intValue)
{
	if(intValue >= 0)
	{
		value->flags = 0;
		value->high = 0;
		value->middle = 0;
		value->low = (ILUInt32)intValue;
	}
	else
	{
		value->flags = (ILUInt32)0x80000000;
		value->high = 0;
		value->middle = 0;
		value->low = (ILUInt32)(-intValue);
	}
}

void ILDecimalFromUInt32(ILDecimal *value, ILUInt32 intValue)
{
	value->flags = 0;
	value->high = 0;
	value->middle = 0;
	value->low = intValue;
}

void ILDecimalFromInt64(ILDecimal *value, ILInt64 intValue)
{
	if(intValue >= 0)
	{
		value->flags = 0;
		value->high = 0;
		value->middle = (ILUInt32)(intValue >> 32);
		value->low = (ILUInt32)intValue;
	}
	else
	{
		intValue = -intValue;
		value->flags = (ILUInt32)0x80000000;
		value->high = 0;
		value->middle = (ILUInt32)(intValue >> 32);
		value->low = (ILUInt32)intValue;
	}
}

void ILDecimalFromUInt64(ILDecimal *value, ILUInt64 intValue)
{
	value->flags = 0;
	value->high = 0;
	value->middle = (ILUInt32)(intValue >> 32);
	value->low = (ILUInt32)intValue;
}

int ILDecimalFromFloat(ILDecimal *value, ILFloat floatValue)
{
	return ILDecimalFromDouble(value, (ILDouble)floatValue);
}

/*
 * The value 2^96 as a double value.
 */
#define	MAX_DECIMAL_AS_DOUBLE	((ILDouble)79228162514264337593543950336.0)

/*
 * The value 2^64 as a double value.
 */
#define	TWO_TO_64				((ILDouble)18446744073709551616.0)

/*
 * Powers of 10 as double values.
 */
static ILDouble const powOfTen[29] =
		{1.0,
		 10.0,
		 100.0,
		 1000.0,
		 10000.0,
		 100000.0,
		 1000000.0,
		 10000000.0,
		 100000000.0,
		 1000000000.0,
		 10000000000.0,
		 100000000000.0,
		 1000000000000.0,
		 10000000000000.0,
		 100000000000000.0,
		 1000000000000000.0,
		 10000000000000000.0,
		 100000000000000000.0,
		 1000000000000000000.0,
		 10000000000000000000.0,
		 100000000000000000000.0,
		 1000000000000000000000.0,
		 10000000000000000000000.0,
		 100000000000000000000000.0,
		 1000000000000000000000000.0,
		 10000000000000000000000000.0,
		 100000000000000000000000000.0,
		 1000000000000000000000000000.0,
		 10000000000000000000000000000.0};

int ILDecimalFromDouble(ILDecimal *value, ILDouble floatValue)
{
	int isNeg;
	int scale;
	ILUInt64 temp;
	ILUInt32 values[3];

	/* Bail out if the number is out of range */
	if(ILNativeFloatIsNaN((ILNativeFloat)floatValue) ||
	   floatValue >= MAX_DECIMAL_AS_DOUBLE ||
	   floatValue <= -MAX_DECIMAL_AS_DOUBLE)
	{
		return 0;
	}

	/* Extract the sign */
	isNeg = (floatValue < (ILDouble)0.0);
	if(isNeg)
	{
		floatValue = -floatValue;
	}

	/* Determine the scale factor to use with the number */
	for(scale = 0; scale < 28; ++scale)
	{
		if(floatValue < powOfTen[scale])
		{
			break;
		}
	}
	scale = 28 - scale;

	/* Re-scale the value to convert it into an integer */
	floatValue *= powOfTen[scale];

	/* Extract the 96-bit integer component */
	values[0] = (ILUInt32)ILFloatToUInt64(floatValue / TWO_TO_64);
	floatValue = ILNativeFloatRem(floatValue, TWO_TO_64);
	temp = ILFloatToUInt64(floatValue);
	values[1] = (ILUInt32)(temp >> 32);
	values[2] = (ILUInt32)temp;

	/* Normalize the result and return it */
	scale = Normalize(values, 3, scale, IL_DECIMAL_ROUND_HALF_UP);
	if(scale < 0)
	{
		return 0;
	}
	value->flags = DECIMAL_MKFLAGS(isNeg, scale);
	value->high = values[0];
	value->middle = values[1];
	value->low = values[2];
	return 1;
}

int ILDecimalToInt64(const ILDecimal *value, ILInt64 *intValue)
{
	if(DECIMAL_GETPT(value) != 0)
	{
		return 0;
	}
	if(DECIMAL_IS_NEG(value))
	{
		if(value->high != 0 || value->middle > (ILUInt32)0x80000000)
		{
			return 0;
		}
		*intValue = -((ILInt64)((((ILUInt64)(value->middle)) << 32) |
			                     ((ILUInt64)(value->low))));
		return 1;
	}
	else
	{
		if(value->high != 0 || value->middle >= (ILUInt32)0x80000000)
		{
			return 0;
		}
		*intValue = ((ILInt64)((((ILUInt64)(value->middle)) << 32) |
			                    ((ILUInt64)(value->low))));
		return 1;
	}
}

int ILDecimalToUInt64(const ILDecimal *value, ILUInt64 *intValue)
{
	if(DECIMAL_GETPT(value) != 0 || DECIMAL_IS_NEG(value))
	{
		return 0;
	}
	if(value->high != 0)
	{
		return 0;
	}
	*intValue = (((ILUInt64)(value->middle)) << 32) | ((ILUInt64)(value->low));
	return 1;
}

ILFloat ILDecimalToFloat(const ILDecimal *value)
{
	return (ILFloat)(ILDecimalToDouble(value));
}

ILDouble ILDecimalToDouble(const ILDecimal *value)
{
	ILDouble temp;
	int decpt;

	/* Convert the fractional part of the value */
	temp = (ILDouble)(value->low);
	temp += ((ILDouble)(value->middle)) * 4294967296.0;				/* 2^32 */
	temp += ((ILDouble)(value->high)) * 18446744073709551616.0;		/* 2^64 */

	/* Adjust for the position of the decimal point */
	decpt = DECIMAL_GETPT(value);
	temp /= powOfTen[decpt];

	/* Apply the sign and return */
	if(DECIMAL_IS_NEG(value))
	{
		return -temp;
	}
	else
	{
		return temp;
	}
}

#ifdef	__cplusplus
};
#endif
