/*
 * bignum.c - Implementation of big number arithmetic.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

/*
 * Note: this implementation is designed for clarity and ease
 * of debugging, not speed.  If you really want speed, then
 * replace this entire file with assembly code.
 */

#include "il_bignum.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Representation of a big number value.  Normally this will
 * be representing positive values, but it is possible for
 * "ILBigNumInv" to need negative values temporarily.
 */
struct _tagILBigNum
{
	ILInt32		size;
	int			neg;
	ILUInt32	words[1];

};

/*
 * Normalize a big number by trimming leading zeroes.
 */
static void NormalizeBigNum(ILBigNum *num)
{
	while(num->size > 0 && num->words[num->size - 1] == 0)
	{
		--(num->size);
	}
	if(num->size == 0)
	{
		/* Zero can never be negative */
		num->neg = 0;
	}
}

void ILBigNumFree(ILBigNum *num)
{
	if(num)
	{
		/* Clear the big number, in case it contains sensitive values */
		ILMemZero(num->words, num->size * sizeof(ILUInt32));
		num->size = 0;
		num->neg = 0;
		ILFree(num);
	}
}

ILBigNum *ILBigNumFromInt(ILUInt32 num)
{
	ILBigNum *bignum = (ILBigNum *)ILMalloc(sizeof(ILBigNum));
	if(bignum)
	{
		if(num != 0)
		{
			bignum->size = 1;
			bignum->neg = 0;
			bignum->words[0] = num;
		}
		else
		{
			bignum->size = 0;
			bignum->neg = 0;
		}
	}
	return bignum;
}

ILBigNum *ILBigNumFromBytes(unsigned char *bytes, ILInt32 numBytes)
{
	ILInt32 size;
	ILBigNum *num;
	
	/* Allocate space for the result */
	size = (numBytes + 3) / 4;
	if(size <= 0)
	{
		/* We know that the value is zero */
		return ILBigNumFromInt(0);
	}
	num = (ILBigNum *)ILMalloc(sizeof(ILBigNum) +
							   (size - 1) * sizeof(ILUInt32));
	if(!num)
	{
		return 0;
	}

	/* Copy the bytes into place */
	size = (numBytes / 4);
	switch(numBytes % 4)
	{
		case 0:
		{
			num->size = size;
			num->neg = 0;
		}
		break;

		case 1:
		{
			num->size = size + 1;
			num->neg = 0;
			num->words[size] = (ILUInt32)(*bytes++);
		}
		break;

		case 2:
		{
			num->size = size + 1;
			num->neg = 0;
			num->words[size] = (((ILUInt32)(bytes[0])) << 8) |
								((ILUInt32)(bytes[1]));
			bytes += 2;
		}
		break;

		case 3:
		{
			num->size = size + 1;
			num->neg = 0;
			num->words[size] = (((ILUInt32)(bytes[0])) << 16) |
			                   (((ILUInt32)(bytes[1])) << 8) |
								((ILUInt32)(bytes[2]));
			bytes += 3;
		}
		break;
	}
	while(size > 0)
	{
		--size;
		num->words[size] = (((ILUInt32)(bytes[0])) << 24) |
		                   (((ILUInt32)(bytes[1])) << 16) |
		                   (((ILUInt32)(bytes[2])) << 8) |
							((ILUInt32)(bytes[3]));
		bytes += 4;
	}

	/* Normalize and return */
	NormalizeBigNum(num);
	return num;
}

ILInt32 ILBigNumByteCount(ILBigNum *num)
{
	ILInt32 numBytes = num->size * 4;
	ILUInt32 top;
	if(num->size > 0)
	{
		top = num->words[num->size - 1];
		if((top & 0xFF000000) == 0)
		{
			if((top & 0x00FF0000) != 0)
			{
				--numBytes;
			}
			else if((top & 0x0000FF00) != 0)
			{
				numBytes -= 2;
			}
			else
			{
				numBytes -= 3;
			}
		}
	}
	else if(!(num->size))
	{
		numBytes = 1;
	}
	return numBytes;
}

void ILBigNumToBytes(ILBigNum *num, unsigned char *bytes)
{
	ILInt32 size = num->size;
	ILUInt32 top;
	if(!size)
	{
		*bytes = 0;
		return;
	}
	top = num->words[size - 1];
	if((top & 0xFF000000) == 0)
	{
		if((top & 0x00FF0000) != 0)
		{
			*bytes++ = (unsigned char)(top >> 16);
			*bytes++ = (unsigned char)(top >> 8);
			*bytes++ = (unsigned char)top;
		}
		else if((top & 0x0000FF00) != 0)
		{
			*bytes++ = (unsigned char)(top >> 8);
			*bytes++ = (unsigned char)top;
		}
		else
		{
			*bytes++ = (unsigned char)top;
		}
		--size;
	}
	while(size > 0)
	{
		--size;
		top = num->words[size];
		*bytes++ = (unsigned char)(top >> 24);
		*bytes++ = (unsigned char)(top >> 16);
		*bytes++ = (unsigned char)(top >> 8);
		*bytes++ = (unsigned char)top;
	}
}

/*
 * Perform an addition, when we know that both values
 * have the same sign.
 */
static ILBigNum *BigNumAdd(ILBigNum *numx, ILBigNum *numy,
						   ILBigNum *modulus, int resultNeg)
{
	ILBigNum *sum;
	ILInt32 size;
	int carry;

	/* Determine the likely size of the sum, including potentional carries */
	if(numx->size > numy->size)
	{
		size = numx->size;
		if(numx->words[size - 1] == IL_MAX_UINT32)
		{
			++size;
		}
	}
	else if(numy->size > numx->size)
	{
		size = numy->size;
		if(numy->words[size - 1] == IL_MAX_UINT32)
		{
			++size;
		}
	}
	else if(numx->size > 0)
	{
		size = numx->size;
		if((((ILUInt64)(numx->words[size - 1])) +
		    ((ILUInt64)(numy->words[size - 1])) + ((ILUInt64)1))
				> (ILUInt64)IL_MAX_UINT32)
		{
			++size;
		}
	}
	else
	{
		size = 1;
	}

	/* Allocate space for the sum */
	sum = (ILBigNum *)ILMalloc(sizeof(ILBigNum) +
							   (size - 1) * sizeof(ILUInt32));
	if(!sum)
	{
		return 0;
	}
	if(!size)
	{
		/* We know already that the answer will be zero */
		sum->size = 0;
		return sum;
	}

	/* Perform the addition */
	size = 0;
	carry = 0;
	while(size < numx->size && size < numy->size)
	{
		if(carry)
		{
			if((sum->words[size] = (numx->words[size] + numy->words[size] + 1))
					> numx->words[size])
			{
				carry = 0;
			}
		}
		else
		{
			if((sum->words[size] = (numx->words[size] + numy->words[size]))
					< numx->words[size])
			{
				carry = 1;
			}
		}
		++size;
	}
	while(size < numx->size)
	{
		if(carry)
		{
			if((sum->words[size] = (numx->words[size] + 1)) != 0)
			{
				carry = 0;
			}
		}
		else
		{
			sum->words[size] = numx->words[size];
		}
		++size;
	}
	while(size < numy->size)
	{
		if(carry)
		{
			if((sum->words[size] = (numy->words[size] + 1)) != 0)
			{
				carry = 0;
			}
		}
		else
		{
			sum->words[size] = numy->words[size];
		}
		++size;
	}
	if(carry)
	{
		sum->words[size++] = 1;
	}
	sum->size = size;
	sum->neg = resultNeg;

	/* Normalize the result */
	NormalizeBigNum(sum);

	/* Do we need to adjust the result by a modulus? */
	if(modulus != 0)
	{
		/* Subtract the modulus from the sum until it is in range.
		   Normally this will only need to be done once because
		   "numx" and "numy" will already have been in range */
		while(ILBigNumCompareAbs(sum, modulus) >= 0)
		{
			/* Subtract the modulus from the sum */
			size = 0;
			carry = 0;
			while(size < modulus->size)
			{
				if(carry)
				{
					if((sum->words[size] -= modulus->words[size] + 1) <
								modulus->words[size])
					{
						carry = 0;
					}
				}
				else
				{
					if((sum->words[size] -= modulus->words[size]) >
								modulus->words[size])
					{
						carry = 1;
					}
				}
				++size;
			}
			while(size < sum->size && carry)
			{
				if((sum->words[size] -= 1) != IL_MAX_UINT32)
				{
					carry = 0;
				}
				++size;
			}

			/* Re-normalize the sum */
			NormalizeBigNum(sum);
		}
	}

	/* Return the result to the caller */
	return sum;
}

/*
 * Subtract one value from another, when we know that
 * the second is less than or equal to the first.
 */
static ILBigNum *BigNumSub(ILBigNum *numx, ILBigNum *numy,
						   ILBigNum *modulus, int resultNeg)
{
	ILBigNum *diff;
	ILInt32 size;
	int borrow;

	/* Allocate space for the difference */
	size = numx->size;
	if(!size)
	{
		/* "numx" is zero and since we know that "numy <= numx",
		   the result must also be zero */
		return ILBigNumFromInt(0);
	}
	diff = (ILBigNum *)ILMalloc(sizeof(ILBigNum) +
							    (size - 1) * sizeof(ILUInt32));
	if(!diff)
	{
		return 0;
	}

	/* Perform the subtraction */
	size = 0;
	borrow = 0;
	while(size < numy->size)
	{
		if(borrow)
		{
			if((diff->words[size] = (numx->words[size] - numy->words[size] - 1))
					< numx->words[size])
			{
				borrow = 0;
			}
		}
		else
		{
			if((diff->words[size] = (numx->words[size] - numy->words[size]))
					> numx->words[size])
			{
				borrow = 1;
			}
		}
		++size;
	}
	while(size < numx->size)
	{
		if(borrow)
		{
			if((diff->words[size] = (numx->words[size] - 1)) != IL_MAX_UINT32)
			{
				borrow = 0;
			}
		}
		else
		{
			diff->words[size] = numx->words[size];
		}
		++size;
	}
	diff->size = size;
	diff->neg = resultNeg;

	/* Normalize the result */
	NormalizeBigNum(diff);

	/* Do we need to adjust the result by a modulus? */
	if(modulus != 0 && diff->neg)
	{
		/* Subtract the negative result from the modulus to get
		   a positive value in the modulo range */
		ILBigNum *diffmod;
		diffmod = BigNumSub(modulus, diff, (ILBigNum *)0, 0);
		ILBigNumFree(diff);
		return diffmod;
	}

	/* Return the result to the caller */
	return diff;
}

ILBigNum *ILBigNumAdd(ILBigNum *numx, ILBigNum *numy, ILBigNum *modulus)
{
	if(numx->neg == numy->neg)
	{
		/* "numx" and "numy" have the same signs */
		return BigNumAdd(numx, numy, modulus, numx->neg);
	}
	else if(numx->neg)
	{
		/* "numx" is negative and "numy" is positive */
		if(ILBigNumCompareAbs(numx, numy) >= 0)
		{
			return BigNumSub(numx, numy, modulus, 1);
		}
		else
		{
			return BigNumSub(numy, numx, modulus, 0);
		}
	}
	else
	{
		/* "numx" is positive and "numy" is negative */
		if(ILBigNumCompareAbs(numx, numy) >= 0)
		{
			return BigNumSub(numx, numy, modulus, 0);
		}
		else
		{
			return BigNumSub(numy, numx, modulus, 1);
		}
	}
}

ILBigNum *ILBigNumSub(ILBigNum *numx, ILBigNum *numy, ILBigNum *modulus)
{
	if(numx->neg && !(numy->neg))
	{
		/* "numx" is negative and "numy" is positive */
		return BigNumAdd(numx, numy, modulus, 1);
	}
	else if(!(numx->neg) && numy->neg)
	{
		/* "numx" is positive and "numy" is negative */
		return BigNumAdd(numx, numy, modulus, 0);
	}
	else if(numx->neg)
	{
		/* "numx" and "numy" are both negative */
		if(ILBigNumCompareAbs(numx, numy) >= 0)
		{
			return BigNumSub(numx, numy, modulus, 1);
		}
		else
		{
			return BigNumSub(numy, numx, modulus, 0);
		}
	}
	else
	{
		/* "numx" and "numy" are both positive */
		if(ILBigNumCompareAbs(numx, numy) >= 0)
		{
			return BigNumSub(numx, numy, modulus, 0);
		}
		else
		{
			return BigNumSub(numy, numx, modulus, 1);
		}
	}
}

/*
 * Determine if a word range in one big number is greater than
 * or equal to a word range in another big number.
 */
static int BigRangeGe(ILUInt32 *words1, ILUInt32 *words2, ILInt32 size)
{
	while(size > 0)
	{
		--size;
		if(words1[size] > words2[size])
		{
			return 1;
		}
		else if(words1[size] < words2[size])
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Internal division implementation.  Returns zero if out of
 * memory or "numy" is zero.
 */
static int DivRem(ILBigNum *numx, ILBigNum *numy,
				  ILBigNum **quotient, ILBigNum **remainder)
{
	ILUInt32 *quotWords;
	ILUInt32 *remWords;
	ILInt32 size, j;
	ILUInt64 temp;
	ILUInt32 divtemp;
	ILUInt32 topy, word1, word2;
	ILBigNum *normx;
	ILBigNum *normy;
	int shift, carry;

	/* Bail out if "numy" or "numx" is zero */
	if(numy->size == 0)
	{
		return 0;
	}
	else if(numx->size == 0)
	{
		if(quotient)
		{
			*quotient = ILBigNumFromInt(0);
			if(!(*quotient))
			{
				return 0;
			}
		}
		if(remainder)
		{
			*remainder = ILBigNumFromInt(0);
			if(!(*remainder))
			{
				if(quotient)
				{
					ILBigNumFree(*quotient);
				}
				return 0;
			}
		}
		return 1;
	}

	/* We can take a short-cut if "numy" is a single word in size */
	if(numy->size == 1)
	{
		if(quotient)
		{
			*quotient = (ILBigNum *)ILMalloc
				(sizeof(ILBigNum) + (numx->size - 1) * sizeof(ILUInt32));
			if(!(*quotient))
			{
				return 0;
			}
			(*quotient)->size = numx->size;
			(*quotient)->neg = 0;
			quotWords = (*quotient)->words;
		}
		else
		{
			quotWords = 0;
		}
		size = numx->size;
		temp = 0;
		divtemp = numy->words[0];
		while(size > 0)
		{
			--size;
			temp = (temp << 32) | (ILUInt64)(numx->words[size]);
			if(quotWords)
			{
				quotWords[size] = (ILUInt32)(temp / (ILUInt64)divtemp);
			}
			temp %= (ILUInt64)divtemp;
		}
		if(remainder)
		{
			*remainder = ILBigNumFromInt((ILUInt32)temp);
			if(!(*remainder))
			{
				if(quotient)
				{
					ILBigNumFree(*quotient);
				}
				return 0;
			}
		}
		if(quotient)
		{
			NormalizeBigNum(*quotient);
		}
		temp = 0;
		divtemp = 0;
		return 1;
	}

	/* If "numx" is less than "numy", then the quotient is
	   zero and the remainder is "numx" */
	if(ILBigNumCompare(numx, numy) < 0)
	{
		if(quotient)
		{
			*quotient = ILBigNumFromInt(0);
			if(!(*quotient))
			{
				return 0;
			}
		}
		if(remainder)
		{
			*remainder = ILBigNumCopy(numx);
			if(!(*remainder))
			{
				if(quotient)
				{
					ILBigNumFree(*quotient);
				}
				return 0;
			}
		}
		return 1;
	}

	/* Allocate space for the temporary big numbers that we need */
	normx = (ILBigNum *)ILMalloc(sizeof(ILBigNum) +
								 numx->size * sizeof(ILUInt32));
	if(!normx)
	{
		return 0;
	}
	normx->size = numx->size + 1;
	normx->neg = 0;
	normy = (ILBigNum *)ILMalloc(sizeof(ILBigNum) +
								 (numy->size - 1) * sizeof(ILUInt32));
	if(!normy)
	{
		ILBigNumFree(normx);
		return 0;
	}
	normy->size = numy->size;
	normy->neg = 0;
	if(quotient)
	{
		size = normx->size - normy->size;
		*quotient = (ILBigNum *)ILMalloc(sizeof(ILBigNum) +
										 (size - 1) * sizeof(ILUInt32));
		if(!(*quotient))
		{
			ILBigNumFree(normx);
			ILBigNumFree(normy);
			return 0;
		}
		(*quotient)->size = size;
		(*quotient)->neg = 0;
		quotWords = (*quotient)->words;
	}
	else
	{
		quotWords = 0;
	}
	if(remainder)
	{
		*remainder = (ILBigNum *)ILMalloc(sizeof(ILBigNum) +
										  (normy->size - 1) * sizeof(ILUInt32));
		if(!(*remainder))
		{
			ILBigNumFree(normx);
			ILBigNumFree(normy);
			if(quotient)
			{
				ILBigNumFree(*quotient);
			}
			return 0;
		}
		(*remainder)->size = normy->size;
		(*remainder)->neg = 0;
		remWords = (*remainder)->words;
	}

	/* Normalize "numx" and "numy" so that the high bit of "normy" is set */
	shift = 0;
	while((numy->words[numy->size - 1] & (((ILUInt32)0x80000000) >> shift))
				== 0)
	{
		++shift;
	}
	temp = 0;
	for(size = 0; size < numx->size; ++size)
	{
		temp |= (((ILUInt64)(numx->words[size])) << shift);
		normx->words[size] = (ILUInt32)temp;
		temp >>= 32;
	}
	normx->words[size] = (ILUInt32)temp;
	temp = 0;
	for(size = 0; size < numy->size; ++size)
	{
		temp |= (((ILUInt64)(numy->words[size])) << shift);
		normy->words[size] = (ILUInt32)temp;
		temp >>= 32;
	}

	/* Perform the division.  Based on the algorithm given in
	   "Multiple-Precision Arithmetic in C", Burton S. Kaliski, Jr,
	   Doctor Dobb's Journal, August 1992 */
	topy = normy->words[normy->size - 1];
	for(j = (numx->size - numy->size); j >= 0; --j)
	{
		/* Underestimate the quotient digit */
		if(topy == IL_MAX_UINT32)
		{
			divtemp = normx->words[j + numy->size];
		}
		else
		{
			temp = (((ILUInt64)(normx->words[j + numy->size])) << 32) |
			       ((ILUInt64)(normx->words[j + numy->size - 1]));
			divtemp = (ILUInt32)(temp / (topy + 1));
		}

		/* Subtract "normy * divtemp" from "normx" */
		temp = 0;
		carry = 0;
		for(size = 0; size < normy->size; ++size)
		{
			/* Put the next word of "normy * divtemp" into "topx" */
			temp += ((ILUInt64)(normy->words[size])) * (ILUInt64)divtemp;
			word1 = (ILUInt32)temp;
			temp >>= 32;

			/* Subtract "topx" from the corresponding word in "normx" */
			word2 = normx->words[j + size];
			if(carry)
			{
				if((normx->words[j + size] -= word1 + 1) < word2)
				{
					carry = 0;
				}
			}
			else
			{
				if((normx->words[j + size] -= word1) > word2)
				{
					carry = 1;
				}
			}
		}
		normx->words[j + size] -= ((ILUInt32)temp) + ((ILUInt32)carry);

		/* Correct the estimate */
		while(normx->words[j + size] != 0 ||
		      BigRangeGe(normx->words + j, normy->words, normy->size))
		{
			++divtemp;
			carry = 0;
			for(size = 0; size < normy->size; ++size)
			{
				word2 = normx->words[j + size];
				if(carry)
				{
					if((normx->words[j + size] -= normy->words[size] + 1)
							< word2)
					{
						carry = 0;
					}
				}
				else
				{
					if((normx->words[j + size] -= normy->words[size])
							> word2)
					{
						carry = 1;
					}
				}
			}
			if(carry)
			{
				--(normx->words[j + size]);
			}
		}

		/* Set the quotient digit */
		if(quotWords)
		{
			quotWords[j] = divtemp;
		}
	}

	/* Set the remainder by shifting "normx" back by "shift" bits */
	if(remainder)
	{
		temp = 0;
		for(size = (*remainder)->size - 1; size >= 0; --size)
		{
			temp = (temp << 32) | ((ILUInt64)(normx->words[size]));
			(*remainder)->words[size] = (ILUInt32)(temp >> shift);
		}
	}

	/* Normalize the quotient and remainder to remove leading zeroes */
	if(quotient)
	{
		NormalizeBigNum(*quotient);
	}
	if(remainder)
	{
		NormalizeBigNum(*remainder);
	}

	/* Free temporary values and clear sensitive values */
	ILBigNumFree(normx);
	ILBigNumFree(normy);
	word1 = word2 = 0;
	topy = 0;
	temp = 0;
	divtemp = 0;

	/* Finished */
	return 1;
}

ILBigNum *ILBigNumMul(ILBigNum *numx, ILBigNum *numy, ILBigNum *modulus)
{
	ILBigNum *product;
	ILBigNum *modProduct;
	ILInt32 size;
	ILInt32 xposn, yposn, posn;
	ILUInt64 temp;

	/* Allocate space for the intermediate product */
	size = numx->size + numy->size;
	if(!size)
	{
		/* We know that the answer will be zero */
		return ILBigNumFromInt(0);
	}
	product = (ILBigNum *)ILMalloc(sizeof(ILBigNum) +
								   (size - 1) * sizeof(ILUInt32));
	if(!product)
	{
		return 0;
	}
	product->size = size;
	product->neg = (numx->neg ^ numy->neg);
	ILMemZero(product->words, size * sizeof(ILUInt32));

	/* Calculate the intermediate product */
	for(xposn = 0; xposn < numx->size; ++xposn)
	{
		temp = 0;
		posn = xposn;
		for(yposn = 0; yposn < numy->size; ++yposn)
		{
			temp += ((ILUInt64)(numx->words[xposn])) *
			        ((ILUInt64)(numy->words[yposn])) +
					((ILUInt64)(product->words[posn]));
			product->words[posn] = (ILUInt32)temp;
			temp >>= 32;
			++posn;
		}
		while(temp != 0)
		{
			/* Propagate the carry */
			temp += product->words[posn];
			product->words[posn] = (ILUInt32)temp;
			temp >>= 32;
			++posn;
		}
	}

	/* Normalize the intermediate product */
	NormalizeBigNum(product);

	/* Do we need to adjust the product by a modulus? */
	if(modulus != 0 && ILBigNumCompare(product, modulus) >= 0)
	{
		if(DivRem(product, modulus, (ILBigNum **)0, &modProduct))
		{
			ILBigNumFree(product);
			return modProduct;
		}
		else
		{
			ILBigNumFree(product);
			return 0;
		}
	}

	/* Return the non-modulo product to the caller */
	return product;
}

ILBigNum *ILBigNumMod(ILBigNum *num, ILBigNum *modulus)
{
	ILBigNum *remainder;
	if(DivRem(num, modulus, (ILBigNum **)0, &remainder))
	{
		return remainder;
	}
	else
	{
		return 0;
	}
}

/*
 * Compute the value "t = u - v * q".
 */
static ILBigNum *GCDSubtract(ILBigNum *u, ILBigNum *v, ILBigNum *q)
{
	ILBigNum *temp, *temp2;
	temp = ILBigNumMul(v, q, (ILBigNum *)0);
	if(!temp)
	{
		return 0;
	}
	temp2 = ILBigNumSub(u, temp, (ILBigNum *)0);
	ILBigNumFree(temp);
	return temp2;
}

/*
 * This function uses the Extended Euclid Algorithm from
 * section 4.5.2, Volume 2 of "The Art of Computer Programming",
 * by Donald E. Knuth, Second Edition, Addison-Wesley.
 */
ILBigNum *ILBigNumInv(ILBigNum *num, ILBigNum *modulus)
{
	ILBigNum *u1, *u2, *u3;
	ILBigNum *v1, *v2, *v3;
	ILBigNum *t1, *t2, *t3;
	ILBigNum *q;

	/* Initialize */
	u1 = ILBigNumFromInt(1);
	u2 = ILBigNumFromInt(0);
	u3 = ILBigNumCopy(num);
	v1 = ILBigNumFromInt(0);
	v2 = ILBigNumFromInt(1);
	v3 = ILBigNumCopy(modulus);
	if(!u1 || !u2 || !u3 || !v1 || !v2 || !v3)
	{
		goto cleanup;
	}

	/* Loop until v3 is zero */
	while(!ILBigNumIsZero(v3))
	{
		/* Compute "q = floor(u3 / v3)" */
		if(!DivRem(u3, v3, &q, (ILBigNum **)0))
		{
			goto cleanup;
		}

		/* Compute "(t1, t2, t3) = (u1, u2, u3) - (v1, v2, v3) * q" */
		t1 = GCDSubtract(u1, v1, q);
		if(!t1)
		{
			ILBigNumFree(q);
			goto cleanup;
		}
		t2 = GCDSubtract(u2, v2, q);
		if(!t2)
		{
			ILBigNumFree(q);
			ILBigNumFree(t1);
			goto cleanup;
		}
		t3 = GCDSubtract(u3, v3, q);
		if(!t3)
		{
			ILBigNumFree(q);
			ILBigNumFree(t1);
			ILBigNumFree(t2);
			goto cleanup;
		}
		ILBigNumFree(q);

		/* Rotate the values */
		ILBigNumFree(u1);
		ILBigNumFree(u2);
		ILBigNumFree(u3);
		u1 = v1;
		u2 = v2;
		u3 = v3;
		v1 = t1;
		v2 = t2;
		v3 = t3;
	}

	/* The answer is in u1: free everything else */
	ILBigNumFree(u2);
	ILBigNumFree(u3);
	ILBigNumFree(v1);
	ILBigNumFree(v2);
	ILBigNumFree(v3);
	if(u1->neg)
	{
		/* Convert negative integers into positive modulo values */
		t1 = ILBigNumAdd(modulus, u1, (ILBigNum *)0);
		ILBigNumFree(u1);
		return t1;
	}
	else
	{
		return u1;
	}

	/* We jump here if we run out of memory during processing */
cleanup:
	ILBigNumFree(u1);
	ILBigNumFree(u2);
	ILBigNumFree(u3);
	ILBigNumFree(v1);
	ILBigNumFree(v2);
	ILBigNumFree(v3);
	return 0;
}

ILBigNum *ILBigNumPow(ILBigNum *numx, ILBigNum *numy, ILBigNum *modulus)
{
	ILBigNum *power;
	ILBigNum *result;
	ILBigNum *temp;
	ILInt32 posn;
	ILUInt32 mask;
	int bit;
	
	/* Set the initial power value to "numx" */
	power = ILBigNumCopy(numx);
	if(!power)
	{
		return 0;
	}

	/* Set the initial result to 1 */
	result = ILBigNumFromInt(1);
	if(!result)
	{
		ILBigNumFree(power);
		return 0;
	}

	/* Compute the result using the binary method */
	for(posn = 0; posn < numy->size; ++posn)
	{
		for(bit = 0, mask = (ILUInt32)0x00000001; bit < 32; ++bit, mask <<= 1)
		{
			/* Multiply "result" by "power" if the bit is 1 */
			if((numy->words[posn] & mask) != 0)
			{
				temp = ILBigNumMul(result, power, modulus);
				ILBigNumFree(result);
				if(!temp)
				{
					ILBigNumFree(power);
					return 0;
				}
				result = temp;
			}

			/* Bail out if we've reached the top-most bit.  This prevents
			   the "power" value from unnecessarily continuing to double
			   in size when "numy" is small */
			if(posn == (numy->size - 1) &&
			   (numy->words[posn] & (mask - 1)) == numy->words[posn])
			{
				break;
			}

			/* Square the "power" value */
			temp = ILBigNumMul(power, power, modulus);
			ILBigNumFree(power);
			if(!temp)
			{
				ILBigNumFree(result);
				return 0;
			}
			power = temp;
		}
	}

	/* If "numy" is negative, then invert the result */
	if(numy->neg && modulus != 0)
	{
		temp = ILBigNumInv(result, modulus);
		ILBigNumFree(result);
		if(!temp)
		{
			ILBigNumFree(power);
			return 0;
		}
		result = temp;
	}

	/* Clean up and return the result */
	ILBigNumFree(power);
	return result;
}

int ILBigNumCompareAbs(ILBigNum *numx, ILBigNum *numy)
{
	ILInt32 index;

	/* Check the magnitude of the numbers first */
	if(numx->size < numy->size)
	{
		return -1;
	}
	else if(numx->size > numy->size)
	{
		return 1;
	}
	else if(numx->size == 0)
	{
		return 0;
	}

	/* Compare the words in the numbers, from most significant to least */
	index = numx->size - 1;
	while(index >= 0)
	{
		if(numx->words[index] < numy->words[index])
		{
			return -1;
		}
		else if(numx->words[index] > numy->words[index])
		{
			return 1;
		}
		--index;
	}
	return 0;
}

int ILBigNumCompare(ILBigNum *numx, ILBigNum *numy)
{
	if(numx->neg && !(numy->neg))
	{
		return -1;
	}
	else if(!(numx->neg) && numy->neg)
	{
		return 1;
	}
	else if(numx->neg)
	{
		return -ILBigNumCompareAbs(numx, numy);
	}
	else
	{
		return ILBigNumCompareAbs(numx, numy);
	}
}

int ILBigNumIsZero(ILBigNum *num)
{
	return (num->size == 0);
}

ILBigNum *ILBigNumCopy(ILBigNum *num)
{
	if(num->size == 0)
	{
		return ILBigNumFromInt(0);
	}
	else
	{
		ILBigNum *copy;
		copy = (ILBigNum *)ILMalloc(sizeof(ILBigNum) +
									(num->size - 1) * sizeof(ILUInt32));
		if(copy)
		{
			copy->size = num->size;
			copy->neg = num->neg;
			ILMemCpy(copy->words, num->words, num->size * sizeof(ILUInt32));
		}
		return copy;
	}
}

#ifdef	__cplusplus
};
#endif
