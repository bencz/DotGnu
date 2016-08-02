/*
 * il_bignum.h - Implementation of big number arithmetic.
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

#ifndef	_IL_BIGNUM_H
#define	_IL_BIGNUM_H

#include "il_values.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Opaque definition of the "big number" type.
 */
typedef struct _tagILBigNum ILBigNum;

/*
 * Free a big number.
 */
void ILBigNumFree(ILBigNum *num);

/*
 * Convert an unsigned integer into a big number value.
 * Return NULL if out of memory.
 */
ILBigNum *ILBigNumFromInt(ILUInt32 num);

/*
 * Convert a big-endian byte array into a big number value.
 * Returns NULL if out of memory.
 */
ILBigNum *ILBigNumFromBytes(unsigned char *bytes, ILInt32 numBytes);

/*
 * Determine the number of bytes that are required to represent
 * a big number value as a big-endian byte array.
 */
ILInt32 ILBigNumByteCount(ILBigNum *num);

/*
 * Convert a big number value into a big-endian byte array.
 */
void ILBigNumToBytes(ILBigNum *num, unsigned char *bytes);

/*
 * Add two big numbers and optionally divide by a modulus.
 * Returns NULL if insufficient memory to allocate the result.
 */
ILBigNum *ILBigNumAdd(ILBigNum *numx, ILBigNum *numy, ILBigNum *modulus);

/*
 * Subtract two big numbers and optionally divide by a modulus.
 * Returns NULL if insufficient memory to allocate the result.
 */
ILBigNum *ILBigNumSub(ILBigNum *numx, ILBigNum *numy, ILBigNum *modulus);

/*
 * Multiply two big numbers and optionally divide by a modulus.
 * Returns NULL if insufficient memory to allocate the result.
 */
ILBigNum *ILBigNumMul(ILBigNum *numx, ILBigNum *numy, ILBigNum *modulus);

/*
 * Divide a number by a modulus and return the remainder.
 * Returns NULL if insufficient memory to allocate the result.
 */
ILBigNum *ILBigNumMod(ILBigNum *num, ILBigNum *modulus);

/*
 * Get the modulo inverse of a big number.  Returns NULL if
 * insufficient memory to allocate the result.  This will only
 * give correct results if "x" and "modulus" are relatively prime,
 * which is normally the case for cryptographic use.
 */
ILBigNum *ILBigNumInv(ILBigNum *num, ILBigNum *modulus);

/*
 * Raise a big number to the power of an exponent and divide by a modulus.
 * Returns NULL if insufficient memory to allocate the result.
 */
ILBigNum *ILBigNumPow(ILBigNum *numx, ILBigNum *numy, ILBigNum *modulus);

/*
 * Compare the absolute values of two big numbers.  Returns -1, 0, or 1.
 */
int ILBigNumCompareAbs(ILBigNum *numx, ILBigNum *numy);

/*
 * Compare two big numbers.  Returns -1, 0, or 1.
 */
int ILBigNumCompare(ILBigNum *numx, ILBigNum *numy);

/*
 * Determine if a big number is equal to zero.
 */
int ILBigNumIsZero(ILBigNum *num);

/*
 * Make a copy of a big number.  Returns NULL if insufficient memory.
 */
ILBigNum *ILBigNumCopy(ILBigNum *num);

#ifdef	__cplusplus
};
#endif

#endif /* _IL_BIGNUM_H */
