/*
 * md5.c - Implementation of the MD5 hash algorithm.
 *
 * Implemented from the description on pages 436-440 in Bruce Schneier,
 * "Applied Cryptography", Second Edition, John Wiley & Sons, Inc., 1996.
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
 * Functions for each of the MD5 rounds.
 */
#define	FROUND1(x,y,z)	(((x) & (y)) | (TRUNCLONG(~(x)) & (z)))
#define	FROUND2(x,y,z)	(((x) & (z)) | ((y) & TRUNCLONG(~(z))))
#define	FROUND3(x,y,z)	((x) ^ (y) ^ (z))
#define	FROUND4(x,y,z)	((y) ^ ((x) | TRUNCLONG(~(z))))

/*
 * Expanded functions for each of the MD5 rounds.
 */
#define	FFROUND1(a,b,c,d,m,s,t)		\
	do { \
		(a) = TRUNCLONG((a) + FROUND1((b), (c), (d)) + (m) + (t)); \
		(a) = TRUNCLONG(ROTATE((a), (s)) + (b)); \
	} while (0)
#define	FFROUND2(a,b,c,d,m,s,t)		\
	do { \
		(a) = TRUNCLONG((a) + FROUND2((b), (c), (d)) + (m) + (t)); \
		(a) = TRUNCLONG(ROTATE((a), (s)) + (b)); \
	} while (0)
#define	FFROUND3(a,b,c,d,m,s,t)		\
	do { \
		(a) = TRUNCLONG((a) + FROUND3((b), (c), (d)) + (m) + (t)); \
		(a) = TRUNCLONG(ROTATE((a), (s)) + (b)); \
	} while (0)
#define	FFROUND4(a,b,c,d,m,s,t)		\
	do { \
		(a) = TRUNCLONG((a) + FROUND4((b), (c), (d)) + (m) + (t)); \
		(a) = TRUNCLONG(ROTATE((a), (s)) + (b)); \
	} while (0)

void ILMD5Init(ILMD5Context *md5)
{
	md5->inputLen = 0;
	md5->A = 0x67452301;
	md5->B = 0xEFCDAB89;
	md5->C = 0x98BADCFE;
	md5->D = 0x10325476;
	md5->totalLen = 0;
}

/*
 * Process a single block of input using the hash algorithm.
 */
static void ProcessBlock(ILMD5Context *md5, const unsigned char *block)
{
	ILUInt32 W[16];
	ILUInt32 a, b, c, d;
	int t;

	/* Unpack the block into 16 32-bit words */
	for(t = 0; t < 16; ++t)
	{
		W[t] = (((ILUInt32)(block[t * 4 + 3])) << 24) |
		       (((ILUInt32)(block[t * 4 + 2])) << 16) |
		       (((ILUInt32)(block[t * 4 + 1])) <<  8) |
		        ((ILUInt32)(block[t * 4 + 0]));
	}

	/* Load the MD5 state into local variables */
	a = md5->A;
	b = md5->B;
	c = md5->C;
	d = md5->D;

	/* Round 1 */
	FFROUND1(a, b, c, d, W[0],   7, 0xD76AA478);
	FFROUND1(d, a, b, c, W[1],  12, 0xE8C7B756);
	FFROUND1(c, d, a, b, W[2],  17, 0x242070DB);
	FFROUND1(b, c, d, a, W[3],  22, 0xC1BDCEEE);
	FFROUND1(a, b, c, d, W[4],   7, 0xF57C0FAF);
	FFROUND1(d, a, b, c, W[5],  12, 0x4787C62A);
	FFROUND1(c, d, a, b, W[6],  17, 0xA8304613);
	FFROUND1(b, c, d, a, W[7],  22, 0xFD469501);
	FFROUND1(a, b, c, d, W[8],   7, 0x698098D8);
	FFROUND1(d, a, b, c, W[9],  12, 0x8B44F7AF);
	FFROUND1(c, d, a, b, W[10], 17, 0xFFFF5BB1);
	FFROUND1(b, c, d, a, W[11], 22, 0x895CD7BE);
	FFROUND1(a, b, c, d, W[12],  7, 0x6B901122);
	FFROUND1(d, a, b, c, W[13], 12, 0xFD987193);
	FFROUND1(c, d, a, b, W[14], 17, 0xA679438E);
	FFROUND1(b, c, d, a, W[15], 22, 0x49B40821);

	/* Round 2 */
	FFROUND2(a, b, c, d, W[1],   5, 0xF61E2562);
	FFROUND2(d, a, b, c, W[6],   9, 0xC040B340);
	FFROUND2(c, d, a, b, W[11], 14, 0x265E5A51);
	FFROUND2(b, c, d, a, W[0],  20, 0xE9B6C7AA);
	FFROUND2(a, b, c, d, W[5],   5, 0xD62F105D);
	FFROUND2(d, a, b, c, W[10],  9, 0x02441453);
	FFROUND2(c, d, a, b, W[15], 14, 0xD8A1E681);
	FFROUND2(b, c, d, a, W[4],  20, 0xE7D3FBC8);
	FFROUND2(a, b, c, d, W[9],   5, 0x21E1CDE6);
	FFROUND2(d, a, b, c, W[14],  9, 0xC33707D6);
	FFROUND2(c, d, a, b, W[3],  14, 0xF4D50D87);
	FFROUND2(b, c, d, a, W[8],  20, 0x455A14ED);
	FFROUND2(a, b, c, d, W[13],  5, 0xA9E3E905);
	FFROUND2(d, a, b, c, W[2],   9, 0xFCEFA3F8);
	FFROUND2(c, d, a, b, W[7],  14, 0x676F02D9);
	FFROUND2(b, c, d, a, W[12], 20, 0x8D2A4C8A);

	/* Round 3 */
	FFROUND3(a, b, c, d, W[5],   4, 0xFFFA3942);
	FFROUND3(d, a, b, c, W[8],  11, 0x8771F681);
	FFROUND3(c, d, a, b, W[11], 16, 0x6D9D6122);
	FFROUND3(b, c, d, a, W[14], 23, 0xFDE5380C);
	FFROUND3(a, b, c, d, W[1],   4, 0xA4BEEA44);
	FFROUND3(d, a, b, c, W[4],  11, 0x4BDECFA9);
	FFROUND3(c, d, a, b, W[7],  16, 0xF6BB4B60);
	FFROUND3(b, c, d, a, W[10], 23, 0xBEBFBC70);
	FFROUND3(a, b, c, d, W[13],  4, 0x289B7EC6);
	FFROUND3(d, a, b, c, W[0],  11, 0xEAA127FA);
	FFROUND3(c, d, a, b, W[3],  16, 0xD4EF3085);
	FFROUND3(b, c, d, a, W[6],  23, 0x04881D05);
	FFROUND3(a, b, c, d, W[9],   4, 0xD9D4D039);
	FFROUND3(d, a, b, c, W[12], 11, 0xE6DB99E5);
	FFROUND3(c, d, a, b, W[15], 16, 0x1FA27CF8);
	FFROUND3(b, c, d, a, W[2],  23, 0xC4AC5665);

	/* Round 4 */
	FFROUND4(a, b, c, d, W[0],   6, 0xF4292244);
	FFROUND4(d, a, b, c, W[7],  10, 0x432AFF97);
	FFROUND4(c, d, a, b, W[14], 15, 0xAB9423A7);
	FFROUND4(b, c, d, a, W[5],  21, 0xFC93A039);
	FFROUND4(a, b, c, d, W[12],  6, 0x655B59C3);
	FFROUND4(d, a, b, c, W[3],  10, 0x8F0CCC92);
	FFROUND4(c, d, a, b, W[10], 15, 0xFFEFF47D);
	FFROUND4(b, c, d, a, W[1],  21, 0x85845DD1);
	FFROUND4(a, b, c, d, W[8],   6, 0x6FA87E4F);
	FFROUND4(d, a, b, c, W[15], 10, 0xFE2CE6E0);
	FFROUND4(c, d, a, b, W[6],  15, 0xA3014314);
	FFROUND4(b, c, d, a, W[13], 21, 0x4E0811A1);
	FFROUND4(a, b, c, d, W[4],   6, 0xF7537E82);
	FFROUND4(d, a, b, c, W[11], 10, 0xBD3AF235);
	FFROUND4(c, d, a, b, W[2],  15, 0x2AD7D2BB);
	FFROUND4(b, c, d, a, W[9],  21, 0xEB86D391);

	/* Combine the previous MD5 state with the new state */
	md5->A = TRUNCLONG(md5->A + a);
	md5->B = TRUNCLONG(md5->B + b);
	md5->C = TRUNCLONG(md5->C + c);
	md5->D = TRUNCLONG(md5->D + d);

	/* Clear the temporary state */
	ILMemZero(W, sizeof(ILUInt32) * 16);
	a = b = c = d = 0;
}

void ILMD5Data(ILMD5Context *md5, const void *buffer, unsigned long len)
{
	unsigned long templen;

	/* Add to the total length of the input stream */
	md5->totalLen += (ILUInt64)len;

	/* Copy the blocks into the input buffer and process them */
	while(len > 0)
	{
		if(!(md5->inputLen) && len >= 64)
		{
			/* Short cut: no point copying the data twice */
			ProcessBlock(md5, (const unsigned char *)buffer);
			buffer = (const void *)(((const unsigned char *)buffer) + 64);
			len -= 64;
		}
		else
		{
			templen = len;
			if(templen > (64 - md5->inputLen))
			{
				templen = 64 - md5->inputLen;
			}
			ILMemCpy(md5->input + md5->inputLen, buffer, templen);
			if((md5->inputLen += templen) >= 64)
			{
				ProcessBlock(md5, md5->input);
				md5->inputLen = 0;
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

void ILMD5Finalize(ILMD5Context *md5, unsigned char hash[IL_MD5_HASH_SIZE])
{
	ILUInt64 totalBits;

	/* Compute the final hash if necessary */
	if(hash)
	{
		/* Pad the input data to a multiple of 512 bits */
		if(md5->inputLen >= 56)
		{
			/* Need two blocks worth of padding */
			md5->input[(md5->inputLen)++] = (unsigned char)0x80;
			while(md5->inputLen < 64)
			{
				md5->input[(md5->inputLen)++] = (unsigned char)0x00;
			}
			ProcessBlock(md5, md5->input);
			md5->inputLen = 0;
		}
		else
		{
			/* Need one block worth of padding */
			md5->input[(md5->inputLen)++] = (unsigned char)0x80;
		}
		while(md5->inputLen < 56)
		{
			md5->input[(md5->inputLen)++] = (unsigned char)0x00;
		}
		totalBits = (md5->totalLen << 3);
		WriteLong(md5->input + 56, (ILUInt32)totalBits);
		WriteLong(md5->input + 60, (ILUInt32)(totalBits >> 32));
		ProcessBlock(md5, md5->input);

		/* Write the final hash value to the supplied buffer */
		WriteLong(hash,      md5->A);
		WriteLong(hash + 4,  md5->B);
		WriteLong(hash + 8,  md5->C);
		WriteLong(hash + 12, md5->D);
	}

	/* Fill the entire context structure with zeros to blank it */
	ILMemZero(md5, sizeof(ILMD5Context));
}

#ifdef TEST_MD5

#include <stdio.h>

/*
 * Define the test vectors from RFC 1321 and the expected answers.
 */
typedef struct
{
	const char *value;
	unsigned char expected[16];

} MD5TestVector;
static MD5TestVector vector1 = {
	"",
	{0xD4, 0x1D, 0x8C, 0xD9, 0x8F, 0x00, 0xB2, 0x04,
	 0xE9, 0x80, 0x09, 0x98, 0xEC, 0xF8, 0x42, 0x7E}
};
static MD5TestVector vector2 = {
	"a",
	{0x0C, 0xC1, 0x75, 0xB9, 0xC0, 0xF1, 0xB6, 0xA8,
	 0x31, 0xC3, 0x99, 0xE2, 0x69, 0x77, 0x26, 0x61}
};
static MD5TestVector vector3 = {
	"abc",
	{0x90, 0x01, 0x50, 0x98, 0x3C, 0xD2, 0x4F, 0xB0,
	 0xD6, 0x96, 0x3F, 0x7D, 0x28, 0xE1, 0x7F, 0x72}
};
static MD5TestVector vector4 = {
	"message digest",
	{0xF9, 0x6B, 0x69, 0x7D, 0x7C, 0xB7, 0x93, 0x8D,
	 0x52, 0x5A, 0x2F, 0x31, 0xAA, 0xF1, 0x61, 0xD0}
};
static MD5TestVector vector5 = {
	"abcdefghijklmnopqrstuvwxyz",
	{0xC3, 0xFC, 0xD3, 0xD7, 0x61, 0x92, 0xE4, 0x00,
	 0x7D, 0xFB, 0x49, 0x6C, 0xCA, 0x67, 0xE1, 0x3B}
};
static MD5TestVector vector6 = {
	"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
	{0xD1, 0x74, 0xAB, 0x98, 0xD2, 0x77, 0xD9, 0xF5,
	 0xA5, 0x61, 0x1C, 0x2C, 0x9F, 0x41, 0x9D, 0x9F}
};
static MD5TestVector vector7 = {
	"123456789012345678901234567890123456789012345678901234567890123456"
	"78901234567890",
	{0x57, 0xED, 0xF4, 0xA2, 0x2B, 0xE3, 0xC9, 0x55,
	 0xAC, 0x49, 0xDA, 0x2E, 0x21, 0x07, 0xB6, 0x7A}
};

/*
 * Print a 16-byte hash value.
 */
static void PrintHash(unsigned char *hash)
{
	printf("%02X%02X %02X%02X %02X%02X %02X%02X %02X%02X "
	       "%02X%02X %02X%02X %02X%02X\n",
		   hash[0], hash[1], hash[2], hash[3],
		   hash[4], hash[5], hash[6], hash[7],
		   hash[8], hash[9], hash[10], hash[11],
		   hash[12], hash[13], hash[14], hash[15]);
}

/*
 * Process a test vector.
 */
static void ProcessVector(MD5TestVector *vector)
{
	ILMD5Context md5;
	unsigned char hash[16];

	/* Compute the hash */
	ILMD5Init(&md5);
	ILMD5Data(&md5, vector->value, strlen(vector->value));
	ILMD5Finalize(&md5, hash);

	/* Report the results */
	printf("Value    = %s\n", vector->value);
	printf("Expected = ");
	PrintHash(vector->expected);
	printf("Actual   = ");
	PrintHash(hash);
	if(ILMemCmp(vector->expected, hash, 16) != 0)
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
	return 0;
}

#endif /* TEST_MD5 */

#ifdef	__cplusplus
};
#endif
