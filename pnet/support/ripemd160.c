/*
 * ripemd160.c - Implementation of the RIPEMD160 hash algorithm.
 *
 * Based on "http://www.esat.kuleuven.ac.be/~bosselae/ripemd160.html".
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#include "il_crypt.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Some helper macros for processing 32-bit values, while
 * being careful about 32-bit vs 64-bit system differences.
 */
#if SIZEOF_LONG > 4
	#define	TRUNCLONG(x)	((x) & IL_MAX_UINT32)
	#define	ROTATE(x,n)		(TRUNCLONG(((x) << (n))) | ((x) >> (32 - (n))))
#else
	#define	TRUNCLONG(x)	(x)
	#define	ROTATE(x,n)		(((x) << (n)) | ((x) >> (32 - (n))))
#endif

/*
 * Basic functions that make up RIPEMD160.
 */
#define RM_F(x,y,z)		((x) ^ (y) ^ (z))
#define RM_G(x,y,z)		(((x) & (y)) | TRUNCLONG(~(x) & (z)))
#define RM_H(x,y,z)		(((x) | TRUNCLONG(~(y))) ^ (z))
#define RM_I(x,y,z)		(((x) & (z)) | ((y) & TRUNCLONG(~(z))))
#define RM_J(x,y,z)		((x) ^ ((y) | TRUNCLONG(~(z))))

/*
 * Expanded functions for each of the RIPEMD160 rounds.
 */
#define RM_FF(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_F((b), (c), (d)) + (x)); \
		(a) = ROTATE((a), (s)) + (e); \
		(c) = ROTATE((c), 10); \
	}
#define RM_GG(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_G((b), (c), (d)) + (x) + \
								   ((ILUInt32)0x5a827999)); \
		(a) = ROTATE((a), (s)) + (e); \
		(c) = ROTATE((c), 10); \
	}
#define RM_HH(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_H((b), (c), (d)) + (x) + \
								   ((ILUInt32)0x6ed9eba1)); \
		(a) = ROTATE((a), (s)) + (e); \
		(c) = ROTATE((c), 10); \
	}
#define RM_II(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_I((b), (c), (d)) + (x) + \
								   ((ILUInt32)0x8f1bbcdc)); \
		(a) = ROTATE((a), (s)) + (e); \
		(c) = ROTATE((c), 10); \
	}
#define RM_JJ(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_J((b), (c), (d)) + (x) + \
								   ((ILUInt32)0xa953fd4e)); \
		(a) = ROTATE((a), (s)) + (e); \
		(c) = ROTATE((c), 10); \
	}
#define RM_FFF(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_F((b), (c), (d)) + (x)); \
		(a) = ROTATE((a), (s)) + (e); \
		(c) = ROTATE((c), 10); \
	}
#define RM_GGG(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_G((b), (c), (d)) + (x) + \
								   ((ILUInt32)0x7a6d76e9)); \
		(a) = ROTATE((a), (s)) + (e); \
		(c) = ROTATE((c), 10); \
	}
#define RM_HHH(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_H((b), (c), (d)) + (x) + \
								   ((ILUInt32)0x6d703ef3)); \
		(a) = ROTATE((a), (s)) + (e);\
		(c) = ROTATE((c), 10);\
	}
#define RM_III(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_I((b), (c), (d)) + (x) + \
								   ((ILUInt32)0x5c4dd124)); \
		(a) = ROTATE((a), (s)) + (e); \
		(c) = ROTATE((c), 10); \
	}
#define RM_JJJ(a,b,c,d,e,x,s)	\
	{ \
		(a) = TRUNCLONG((a) + RM_J((b), (c), (d)) + (x) + \
								   ((ILUInt32)0x50a28be6)); \
		(a) = ROTATE((a), (s)) + (e); \
		(c) = ROTATE((c), 10); \
	}

void ILRIPEMD160Init(ILRIPEMD160Context *ripem)
{
	ripem->inputLen = 0;
	ripem->A = 0x67452301;
	ripem->B = 0xEFCDAB89;
	ripem->C = 0x98BADCFE;
	ripem->D = 0x10325476;
	ripem->E = 0xC3D2E1F0;
	ripem->totalLen = 0;
}

/*
 * Process a single block of input using the hash algorithm.
 */
static void ProcessBlock(ILRIPEMD160Context *ripem, const unsigned char *block)
{
	ILUInt32 W[16];
	ILUInt32 a, b, c, d, e;
	ILUInt32 aa, bb, cc, dd, ee;
	int t;

	/* Unpack the block into 16 32-bit words */
	for(t = 0; t < 16; ++t)
	{
		W[t] = (((ILUInt32)(block[t * 4 + 3])) << 24) |
		       (((ILUInt32)(block[t * 4 + 2])) << 16) |
		       (((ILUInt32)(block[t * 4 + 1])) <<  8) |
		        ((ILUInt32)(block[t * 4 + 0]));
	}

	/* Load the RIPEMD160 state into local variables */
	a = ripem->A;
	b = ripem->B;
	c = ripem->C;
	d = ripem->D;
	e = ripem->E;
	aa = ripem->A;
	bb = ripem->B;
	cc = ripem->C;
	dd = ripem->D;
	ee = ripem->E;

	/* Round 1 */
	RM_FF(a, b, c, d, e, W[ 0], 11);
	RM_FF(e, a, b, c, d, W[ 1], 14);
	RM_FF(d, e, a, b, c, W[ 2], 15);
	RM_FF(c, d, e, a, b, W[ 3], 12);
	RM_FF(b, c, d, e, a, W[ 4],  5);
	RM_FF(a, b, c, d, e, W[ 5],  8);
	RM_FF(e, a, b, c, d, W[ 6],  7);
	RM_FF(d, e, a, b, c, W[ 7],  9);
	RM_FF(c, d, e, a, b, W[ 8], 11);
	RM_FF(b, c, d, e, a, W[ 9], 13);
	RM_FF(a, b, c, d, e, W[10], 14);
	RM_FF(e, a, b, c, d, W[11], 15);
	RM_FF(d, e, a, b, c, W[12],  6);
	RM_FF(c, d, e, a, b, W[13],  7);
	RM_FF(b, c, d, e, a, W[14],  9);
	RM_FF(a, b, c, d, e, W[15],  8);

	/* Round 2 */
	RM_GG(e, a, b, c, d, W[ 7],  7);
	RM_GG(d, e, a, b, c, W[ 4],  6);
	RM_GG(c, d, e, a, b, W[13],  8);
	RM_GG(b, c, d, e, a, W[ 1], 13);
	RM_GG(a, b, c, d, e, W[10], 11);
	RM_GG(e, a, b, c, d, W[ 6],  9);
	RM_GG(d, e, a, b, c, W[15],  7);
	RM_GG(c, d, e, a, b, W[ 3], 15);
	RM_GG(b, c, d, e, a, W[12],  7);
	RM_GG(a, b, c, d, e, W[ 0], 12);
	RM_GG(e, a, b, c, d, W[ 9], 15);
	RM_GG(d, e, a, b, c, W[ 5],  9);
	RM_GG(c, d, e, a, b, W[ 2], 11);
	RM_GG(b, c, d, e, a, W[14],  7);
	RM_GG(a, b, c, d, e, W[11], 13);
	RM_GG(e, a, b, c, d, W[ 8], 12);

	/* Round 3 */
	RM_HH(d, e, a, b, c, W[ 3], 11);
	RM_HH(c, d, e, a, b, W[10], 13);
	RM_HH(b, c, d, e, a, W[14],  6);
	RM_HH(a, b, c, d, e, W[ 4],  7);
	RM_HH(e, a, b, c, d, W[ 9], 14);
	RM_HH(d, e, a, b, c, W[15],  9);
	RM_HH(c, d, e, a, b, W[ 8], 13);
	RM_HH(b, c, d, e, a, W[ 1], 15);
	RM_HH(a, b, c, d, e, W[ 2], 14);
	RM_HH(e, a, b, c, d, W[ 7],  8);
	RM_HH(d, e, a, b, c, W[ 0], 13);
	RM_HH(c, d, e, a, b, W[ 6],  6);
	RM_HH(b, c, d, e, a, W[13],  5);
	RM_HH(a, b, c, d, e, W[11], 12);
	RM_HH(e, a, b, c, d, W[ 5],  7);
	RM_HH(d, e, a, b, c, W[12],  5);

	/* Round 4 */
	RM_II(c, d, e, a, b, W[ 1], 11);
	RM_II(b, c, d, e, a, W[ 9], 12);
	RM_II(a, b, c, d, e, W[11], 14);
	RM_II(e, a, b, c, d, W[10], 15);
	RM_II(d, e, a, b, c, W[ 0], 14);
	RM_II(c, d, e, a, b, W[ 8], 15);
	RM_II(b, c, d, e, a, W[12],  9);
	RM_II(a, b, c, d, e, W[ 4],  8);
	RM_II(e, a, b, c, d, W[13],  9);
	RM_II(d, e, a, b, c, W[ 3], 14);
	RM_II(c, d, e, a, b, W[ 7],  5);
	RM_II(b, c, d, e, a, W[15],  6);
	RM_II(a, b, c, d, e, W[14],  8);
	RM_II(e, a, b, c, d, W[ 5],  6);
	RM_II(d, e, a, b, c, W[ 6],  5);
	RM_II(c, d, e, a, b, W[ 2], 12);

	/* Round 5 */
	RM_JJ(b, c, d, e, a, W[ 4],  9);
	RM_JJ(a, b, c, d, e, W[ 0], 15);
	RM_JJ(e, a, b, c, d, W[ 5],  5);
	RM_JJ(d, e, a, b, c, W[ 9], 11);
	RM_JJ(c, d, e, a, b, W[ 7],  6);
	RM_JJ(b, c, d, e, a, W[12],  8);
	RM_JJ(a, b, c, d, e, W[ 2], 13);
	RM_JJ(e, a, b, c, d, W[10], 12);
	RM_JJ(d, e, a, b, c, W[14],  5);
	RM_JJ(c, d, e, a, b, W[ 1], 12);
	RM_JJ(b, c, d, e, a, W[ 3], 13);
	RM_JJ(a, b, c, d, e, W[ 8], 14);
	RM_JJ(e, a, b, c, d, W[11], 11);
	RM_JJ(d, e, a, b, c, W[ 6],  8);
	RM_JJ(c, d, e, a, b, W[15],  5);
	RM_JJ(b, c, d, e, a, W[13],  6);

	/* Parallel Round 1 */
	RM_JJJ(aa, bb, cc, dd, ee, W[ 5],  8);
	RM_JJJ(ee, aa, bb, cc, dd, W[14],  9);
	RM_JJJ(dd, ee, aa, bb, cc, W[ 7],  9);
	RM_JJJ(cc, dd, ee, aa, bb, W[ 0], 11);
	RM_JJJ(bb, cc, dd, ee, aa, W[ 9], 13);
	RM_JJJ(aa, bb, cc, dd, ee, W[ 2], 15);
	RM_JJJ(ee, aa, bb, cc, dd, W[11], 15);
	RM_JJJ(dd, ee, aa, bb, cc, W[ 4],  5);
	RM_JJJ(cc, dd, ee, aa, bb, W[13],  7);
	RM_JJJ(bb, cc, dd, ee, aa, W[ 6],  7);
	RM_JJJ(aa, bb, cc, dd, ee, W[15],  8);
	RM_JJJ(ee, aa, bb, cc, dd, W[ 8], 11);
	RM_JJJ(dd, ee, aa, bb, cc, W[ 1], 14);
	RM_JJJ(cc, dd, ee, aa, bb, W[10], 14);
	RM_JJJ(bb, cc, dd, ee, aa, W[ 3], 12);
	RM_JJJ(aa, bb, cc, dd, ee, W[12],  6);

	/* Parallel Round 2 */
	RM_III(ee, aa, bb, cc, dd, W[ 6],  9); 
	RM_III(dd, ee, aa, bb, cc, W[11], 13);
	RM_III(cc, dd, ee, aa, bb, W[ 3], 15);
	RM_III(bb, cc, dd, ee, aa, W[ 7],  7);
	RM_III(aa, bb, cc, dd, ee, W[ 0], 12);
	RM_III(ee, aa, bb, cc, dd, W[13],  8);
	RM_III(dd, ee, aa, bb, cc, W[ 5],  9);
	RM_III(cc, dd, ee, aa, bb, W[10], 11);
	RM_III(bb, cc, dd, ee, aa, W[14],  7);
	RM_III(aa, bb, cc, dd, ee, W[15],  7);
	RM_III(ee, aa, bb, cc, dd, W[ 8], 12);
	RM_III(dd, ee, aa, bb, cc, W[12],  7);
	RM_III(cc, dd, ee, aa, bb, W[ 4],  6);
	RM_III(bb, cc, dd, ee, aa, W[ 9], 15);
	RM_III(aa, bb, cc, dd, ee, W[ 1], 13);
	RM_III(ee, aa, bb, cc, dd, W[ 2], 11);

	/* Parallel Round 3 */
	RM_HHH(dd, ee, aa, bb, cc, W[15],  9);
	RM_HHH(cc, dd, ee, aa, bb, W[ 5],  7);
	RM_HHH(bb, cc, dd, ee, aa, W[ 1], 15);
	RM_HHH(aa, bb, cc, dd, ee, W[ 3], 11);
	RM_HHH(ee, aa, bb, cc, dd, W[ 7],  8);
	RM_HHH(dd, ee, aa, bb, cc, W[14],  6);
	RM_HHH(cc, dd, ee, aa, bb, W[ 6],  6);
	RM_HHH(bb, cc, dd, ee, aa, W[ 9], 14);
	RM_HHH(aa, bb, cc, dd, ee, W[11], 12);
	RM_HHH(ee, aa, bb, cc, dd, W[ 8], 13);
	RM_HHH(dd, ee, aa, bb, cc, W[12],  5);
	RM_HHH(cc, dd, ee, aa, bb, W[ 2], 14);
	RM_HHH(bb, cc, dd, ee, aa, W[10], 13);
	RM_HHH(aa, bb, cc, dd, ee, W[ 0], 13);
	RM_HHH(ee, aa, bb, cc, dd, W[ 4],  7);
	RM_HHH(dd, ee, aa, bb, cc, W[13],  5);

	/* Parallel Round 4 */   
	RM_GGG(cc, dd, ee, aa, bb, W[ 8], 15);
	RM_GGG(bb, cc, dd, ee, aa, W[ 6],  5);
	RM_GGG(aa, bb, cc, dd, ee, W[ 4],  8);
	RM_GGG(ee, aa, bb, cc, dd, W[ 1], 11);
	RM_GGG(dd, ee, aa, bb, cc, W[ 3], 14);
	RM_GGG(cc, dd, ee, aa, bb, W[11], 14);
	RM_GGG(bb, cc, dd, ee, aa, W[15],  6);
	RM_GGG(aa, bb, cc, dd, ee, W[ 0], 14);
	RM_GGG(ee, aa, bb, cc, dd, W[ 5],  6);
	RM_GGG(dd, ee, aa, bb, cc, W[12],  9);
	RM_GGG(cc, dd, ee, aa, bb, W[ 2], 12);
	RM_GGG(bb, cc, dd, ee, aa, W[13],  9);
	RM_GGG(aa, bb, cc, dd, ee, W[ 9], 12);
	RM_GGG(ee, aa, bb, cc, dd, W[ 7],  5);
	RM_GGG(dd, ee, aa, bb, cc, W[10], 15);
	RM_GGG(cc, dd, ee, aa, bb, W[14],  8);

	/* Parallel Round 5 */
	RM_FFF(bb, cc, dd, ee, aa, W[12] ,  8);
	RM_FFF(aa, bb, cc, dd, ee, W[15] ,  5);
	RM_FFF(ee, aa, bb, cc, dd, W[10] , 12);
	RM_FFF(dd, ee, aa, bb, cc, W[ 4] ,  9);
	RM_FFF(cc, dd, ee, aa, bb, W[ 1] , 12);
	RM_FFF(bb, cc, dd, ee, aa, W[ 5] ,  5);
	RM_FFF(aa, bb, cc, dd, ee, W[ 8] , 14);
	RM_FFF(ee, aa, bb, cc, dd, W[ 7] ,  6);
	RM_FFF(dd, ee, aa, bb, cc, W[ 6] ,  8);
	RM_FFF(cc, dd, ee, aa, bb, W[ 2] , 13);
	RM_FFF(bb, cc, dd, ee, aa, W[13] ,  6);
	RM_FFF(aa, bb, cc, dd, ee, W[14] ,  5);
	RM_FFF(ee, aa, bb, cc, dd, W[ 0] , 15);
	RM_FFF(dd, ee, aa, bb, cc, W[ 3] , 13);
	RM_FFF(cc, dd, ee, aa, bb, W[ 9] , 11);
	RM_FFF(bb, cc, dd, ee, aa, W[11] , 11);

	/* Combine the previous RIPEMD160 state with the new state */
	dd = TRUNCLONG(dd + c + ripem->B);
	ripem->B = TRUNCLONG(ripem->C + d + ee);
	ripem->C = TRUNCLONG(ripem->D + e + aa);
	ripem->D = TRUNCLONG(ripem->E + a + bb);
	ripem->E = TRUNCLONG(ripem->A + b + cc);
	ripem->A = dd;

	/* Clear the temporary state */
	ILMemZero(W, sizeof(ILUInt32) * 16);
	a = b = c = d = e = 0;
	aa = bb = cc = dd = ee = 0;
}

void ILRIPEMD160Data(ILRIPEMD160Context *ripem,
					 const void *buffer, unsigned long len)
{
	unsigned long templen;

	/* Add to the total length of the input stream */
	ripem->totalLen += (ILUInt64)len;

	/* Copy the blocks into the input buffer and process them */
	while(len > 0)
	{
		if(!(ripem->inputLen) && len >= 64)
		{
			/* Short cut: no point copying the data twice */
			ProcessBlock(ripem, (const unsigned char *)buffer);
			buffer = (const void *)(((const unsigned char *)buffer) + 64);
			len -= 64;
		}
		else
		{
			templen = len;
			if(templen > (64 - ripem->inputLen))
			{
				templen = 64 - ripem->inputLen;
			}
			ILMemCpy(ripem->input + ripem->inputLen, buffer, templen);
			if((ripem->inputLen += templen) >= 64)
			{
				ProcessBlock(ripem, ripem->input);
				ripem->inputLen = 0;
			}
			buffer = (const void *)(((const unsigned char *)buffer) + templen);
			len -= templen;
		}
	}
}

/*
 * Write a 32-bit little-endian long value to a buffer.
 */
static void WriteLong(unsigned char *buf, ILUInt32 value)
{
	buf[0] = (unsigned char)value;
	buf[1] = (unsigned char)(value >> 8);
	buf[2] = (unsigned char)(value >> 16);
	buf[3] = (unsigned char)(value >> 24);
}

void ILRIPEMD160Finalize(ILRIPEMD160Context *ripem,
						 unsigned char hash[IL_RIPEMD160_HASH_SIZE])
{
	ILUInt64 totalBits;

	/* Compute the final hash if necessary */
	if(hash)
	{
		/* Pad the input data to a multiple of 512 bits */
		if(ripem->inputLen >= 56)
		{
			/* Need two blocks worth of padding */
			ripem->input[(ripem->inputLen)++] = (unsigned char)0x80;
			while(ripem->inputLen < 64)
			{
				ripem->input[(ripem->inputLen)++] = (unsigned char)0x00;
			}
			ProcessBlock(ripem, ripem->input);
			ripem->inputLen = 0;
		}
		else
		{
			/* Need one block worth of padding */
			ripem->input[(ripem->inputLen)++] = (unsigned char)0x80;
		}
		while(ripem->inputLen < 56)
		{
			ripem->input[(ripem->inputLen)++] = (unsigned char)0x00;
		}
		totalBits = (ripem->totalLen << 3);
		WriteLong(ripem->input + 56, (ILUInt32)totalBits);
		WriteLong(ripem->input + 60, (ILUInt32)(totalBits >> 32));
		ProcessBlock(ripem, ripem->input);

		/* Write the final hash value to the supplied buffer */
		WriteLong(hash,      ripem->A);
		WriteLong(hash + 4,  ripem->B);
		WriteLong(hash + 8,  ripem->C);
		WriteLong(hash + 12, ripem->D);
		WriteLong(hash + 16, ripem->E);
	}

	/* Fill the entire context structure with zeros to blank it */
	ILMemZero(ripem, sizeof(ILRIPEMD160Context));
}

#ifdef TEST_RIPEMD160

#include <stdio.h>

/*
 * Define the test vectors and the expected answers.
 */
typedef struct
{
	const char *value;
	unsigned char expected[20];

} RIPEMD160TestVector;
static RIPEMD160TestVector vector1 = {
	"",
	{0x9c, 0x11, 0x85, 0xa5, 0xc5, 0xe9, 0xfc, 0x54, 0x61, 0x28,
	 0x08, 0x97, 0x7e, 0xe8, 0xf5, 0x48, 0xb2, 0x25, 0x8d, 0x31}
};
static RIPEMD160TestVector vector2 = {
	"a",
	{0x0b, 0xdc, 0x9d, 0x2d, 0x25, 0x6b, 0x3e, 0xe9, 0xda, 0xae,
	 0x34, 0x7b, 0xe6, 0xf4, 0xdc, 0x83, 0x5a, 0x46, 0x7f, 0xfe}
};
static RIPEMD160TestVector vector3 = {
	"abc",
	{0x8e, 0xb2, 0x08, 0xf7, 0xe0, 0x5d, 0x98, 0x7a, 0x9b, 0x04,
	 0x4a, 0x8e, 0x98, 0xc6, 0xb0, 0x87, 0xf1, 0x5a, 0x0b, 0xfc}
};
static RIPEMD160TestVector vector4 = {
	"message digest",
	{0x5d, 0x06, 0x89, 0xef, 0x49, 0xd2, 0xfa, 0xe5, 0x72, 0xb8,
	 0x81, 0xb1, 0x23, 0xa8, 0x5f, 0xfa, 0x21, 0x59, 0x5f, 0x36}
};
static RIPEMD160TestVector vector5 = {
	"abcdefghijklmnopqrstuvwxyz",
	{0xf7, 0x1c, 0x27, 0x10, 0x9c, 0x69, 0x2c, 0x1b, 0x56, 0xbb,
	 0xdc, 0xeb, 0x5b, 0x9d, 0x28, 0x65, 0xb3, 0x70, 0x8d, 0xbc}
};
static RIPEMD160TestVector vector6 = {
	"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq",
	{0x12, 0xa0, 0x53, 0x38, 0x4a, 0x9c, 0x0c, 0x88, 0xe4, 0x05,
	 0xa0, 0x6c, 0x27, 0xdc, 0xf4, 0x9a, 0xda, 0x62, 0xeb, 0x2b}
};
static RIPEMD160TestVector vector7 = {
	"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
	{0xb0, 0xe2, 0x0b, 0x6e, 0x31, 0x16, 0x64, 0x02, 0x86, 0xed,
	 0x3a, 0x87, 0xa5, 0x71, 0x30, 0x79, 0xb2, 0x1f, 0x51, 0x89}
};
static RIPEMD160TestVector vector8 = {
	"1234567890123456789012345678901234567890123456789012345678901234567890"
	"1234567890",
	{0x9b, 0x75, 0x2e, 0x45, 0x57, 0x3d, 0x4b, 0x39, 0xf4, 0xdb,
	 0xd3, 0x32, 0x3c, 0xab, 0x82, 0xbf, 0x63, 0x32, 0x6b, 0xfb}
};

/*
 * Print a 20-byte hash value.
 */
static void PrintHash(unsigned char *hash)
{
	printf("%02X%02X %02X%02X %02X%02X %02X%02X %02X%02X "
	       "%02X%02X %02X%02X %02X%02X %02X%02X %02X%02X\n",
		   hash[0], hash[1], hash[2], hash[3],
		   hash[4], hash[5], hash[6], hash[7],
		   hash[8], hash[9], hash[10], hash[11],
		   hash[12], hash[13], hash[14], hash[15],
		   hash[16], hash[17], hash[18], hash[19]);
}

/*
 * Process a test vector.
 */
static void ProcessVector(RIPEMD160TestVector *vector)
{
	ILRIPEMD160Context sha;
	unsigned char hash[20];

	/* Compute the hash */
	ILRIPEMD160Init(&sha);
	ILRIPEMD160Data(&sha, vector->value, strlen(vector->value));
	ILRIPEMD160Finalize(&sha, hash);

	/* Report the results */
	printf("Value    = %s\n", vector->value);
	printf("Expected = ");
	PrintHash(vector->expected);
	printf("Actual   = ");
	PrintHash(hash);
	if(ILMemCmp(vector->expected, hash, 20) != 0)
	{
		printf("*** test failed ***\n");
	}
	printf("\n");
}

int main(int argc, char *argv[])
{
	printf("\n");
	ProcessVector(&vector1);
	ProcessVector(&vector2);
	ProcessVector(&vector3);
	ProcessVector(&vector4);
	ProcessVector(&vector5);
	ProcessVector(&vector6);
	ProcessVector(&vector7);
	ProcessVector(&vector8);
	return 0;
}

#endif /* TEST_RIPEMD160 */

#ifdef	__cplusplus
};
#endif
