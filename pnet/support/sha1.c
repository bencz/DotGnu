/*
 * sha1.c - Implementation of the Secure Hash Algorithm 1 (SHA1).
 *
 * Implemented from the description on pages 442-444 in Bruce Schneier,
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
 * Functions for each of the SHA rounds.
 */
#define	FROUND1(x,y,z)	(((x) & (y)) | (TRUNCLONG(~(x)) & (z)))
#define	FROUND2(x,y,z)	((x) ^ (y) ^ (z))
#define	FROUND3(x,y,z)	(((x) & (y)) | ((x) & (z)) | ((y) & (z)))
#define	FROUND4(x,y,z)	((x) ^ (y) ^ (z))

/*
 * Constants used in each of the SHA rounds.
 */
#define	KROUND1			0x5A827999
#define	KROUND2			0x6ED9EBA1
#define	KROUND3			0x8F1BBCDC
#define	KROUND4			0xCA62C1D6

void ILSHAInit(ILSHAContext *sha)
{
	sha->inputLen = 0;
	sha->A = 0x67452301;
	sha->B = 0xEFCDAB89;
	sha->C = 0x98BADCFE;
	sha->D = 0x10325476;
	sha->E = 0xC3D2E1F0;
	sha->totalLen = 0;
}

/*
 * Process a single block of input using the hash algorithm.
 */
static void ProcessBlock(ILSHAContext *sha, const unsigned char *block)
{
	ILUInt32 W[80];
	ILUInt32 a, b, c, d, e;
	ILUInt32 temp;
	int t;

	/* Unpack the block into 80 32-bit words */
	for(t = 0; t < 16; ++t)
	{
		W[t] = (((ILUInt32)(block[t * 4 + 0])) << 24) |
		       (((ILUInt32)(block[t * 4 + 1])) << 16) |
		       (((ILUInt32)(block[t * 4 + 2])) <<  8) |
		        ((ILUInt32)(block[t * 4 + 3]));
	}
	for(t = 16; t < 80; ++t)
	{
		temp = W[t - 3] ^ W[t - 8] ^ W[t - 14] ^ W[t - 16];
		W[t] = ROTATE(temp, 1);
	}

	/* Load the SHA state into local variables */
	a = sha->A;
	b = sha->B;
	c = sha->C;
	d = sha->D;
	e = sha->E;

	/* Round 1 */
	for(t = 0; t < 20; ++t)
	{
		temp = TRUNCLONG(ROTATE(a, 5) + FROUND1(b, c, d) +
						 e + W[t] + KROUND1);
		e = d;
		d = c;
		c = ROTATE(b, 30);
		b = a;
		a = temp;
	}

	/* Round 2 */
	for(t = 0; t < 20; ++t)
	{
		temp = TRUNCLONG(ROTATE(a, 5) + FROUND2(b, c, d) +
						 e + W[t + 20] + KROUND2);
		e = d;
		d = c;
		c = ROTATE(b, 30);
		b = a;
		a = temp;
	}

	/* Round 3 */
	for(t = 0; t < 20; ++t)
	{
		temp = TRUNCLONG(ROTATE(a, 5) + FROUND3(b, c, d) +
						 e + W[t + 40] + KROUND3);
		e = d;
		d = c;
		c = ROTATE(b, 30);
		b = a;
		a = temp;
	}

	/* Round 4 */
	for(t = 0; t < 20; ++t)
	{
		temp = TRUNCLONG(ROTATE(a, 5) + FROUND4(b, c, d) +
						 e + W[t + 60] + KROUND4);
		e = d;
		d = c;
		c = ROTATE(b, 30);
		b = a;
		a = temp;
	}

	/* Combine the previous SHA state with the new state */
	sha->A = TRUNCLONG(sha->A + a);
	sha->B = TRUNCLONG(sha->B + b);
	sha->C = TRUNCLONG(sha->C + c);
	sha->D = TRUNCLONG(sha->D + d);
	sha->E = TRUNCLONG(sha->E + e);

	/* Clear the temporary state */
	ILMemZero(W, sizeof(ILUInt32) * 80);
	a = b = c = d = e = temp = 0;
}

void ILSHAData(ILSHAContext *sha, const void *buffer, unsigned long len)
{
	unsigned long templen;

	/* Add to the total length of the input stream */
	sha->totalLen += (ILUInt64)len;

	/* Copy the blocks into the input buffer and process them */
	while(len > 0)
	{
		if(!(sha->inputLen) && len >= 64)
		{
			/* Short cut: no point copying the data twice */
			ProcessBlock(sha, (const unsigned char *)buffer);
			buffer = (const void *)(((const unsigned char *)buffer) + 64);
			len -= 64;
		}
		else
		{
			templen = len;
			if(templen > (64 - sha->inputLen))
			{
				templen = 64 - sha->inputLen;
			}
			ILMemCpy(sha->input + sha->inputLen, buffer, templen);
			if((sha->inputLen += templen) >= 64)
			{
				ProcessBlock(sha, sha->input);
				sha->inputLen = 0;
			}
			buffer = (const void *)(((const unsigned char *)buffer) + templen);
			len -= templen;
		}
	}
}

/*
 * Write a 32-bit big-endian long value to a buffer.
 */
static void WriteLong(unsigned char *buf, ILUInt32 value)
{
	buf[0] = (unsigned char)(value >> 24);
	buf[1] = (unsigned char)(value >> 16);
	buf[2] = (unsigned char)(value >> 8);
	buf[3] = (unsigned char)value;
}

void ILSHAFinalize(ILSHAContext *sha, unsigned char hash[IL_SHA_HASH_SIZE])
{
	ILUInt64 totalBits;

	/* Compute the final hash if necessary */
	if(hash)
	{
		/* Pad the input data to a multiple of 512 bits */
		if(sha->inputLen >= 56)
		{
			/* Need two blocks worth of padding */
			sha->input[(sha->inputLen)++] = (unsigned char)0x80;
			while(sha->inputLen < 64)
			{
				sha->input[(sha->inputLen)++] = (unsigned char)0x00;
			}
			ProcessBlock(sha, sha->input);
			sha->inputLen = 0;
		}
		else
		{
			/* Need one block worth of padding */
			sha->input[(sha->inputLen)++] = (unsigned char)0x80;
		}
		while(sha->inputLen < 56)
		{
			sha->input[(sha->inputLen)++] = (unsigned char)0x00;
		}
		totalBits = (sha->totalLen << 3);
		WriteLong(sha->input + 56, (ILUInt32)(totalBits >> 32));
		WriteLong(sha->input + 60, (ILUInt32)totalBits);
		ProcessBlock(sha, sha->input);

		/* Write the final hash value to the supplied buffer */
		WriteLong(hash,      sha->A);
		WriteLong(hash + 4,  sha->B);
		WriteLong(hash + 8,  sha->C);
		WriteLong(hash + 12, sha->D);
		WriteLong(hash + 16, sha->E);
	}

	/* Fill the entire context structure with zeros to blank it */
	ILMemZero(sha, sizeof(ILSHAContext));
}

#ifdef TEST_SHA

#include <stdio.h>

/*
 * Define the test vectors and the expected answers.
 */
typedef struct
{
	const char *value;
	unsigned char expected[20];

} SHATestVector;
static SHATestVector vector1 = {
	"abc",
	{0xA9, 0x99, 0x3E, 0x36, 0x47, 0x06, 0x81, 0x6A, 0xBA, 0x3E,
	 0x25, 0x71, 0x78, 0x50, 0xC2, 0x6C, 0x9C, 0xD0, 0xD8, 0x9D}
};
static SHATestVector vector2 = {
	"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq",
	{0x84, 0x98, 0x3E, 0x44, 0x1C, 0x3B, 0xD2, 0x6E, 0xBA, 0xAE,
	 0x4A, 0xA1, 0xF9, 0x51, 0x29, 0xE5, 0xE5, 0x46, 0x70, 0xF1}
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
static void ProcessVector(SHATestVector *vector)
{
	ILSHAContext sha;
	unsigned char hash[20];

	/* Compute the hash */
	ILSHAInit(&sha);
	ILSHAData(&sha, vector->value, strlen(vector->value));
	ILSHAFinalize(&sha, hash);

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
	return 0;
}

#endif /* TEST_SHA */

#ifdef	__cplusplus
};
#endif
