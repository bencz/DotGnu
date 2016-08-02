/*
 * sha384.c - Implementation of the Secure Hash Algorithm-384 (SHA-384).
 *
 * Implemented from the description on the NIST Web site:
 *		http://csrc.nist.gov/cryptval/shs.html
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


#include "il_crypt.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

void ILSHA384Init(ILSHA384Context *sha)
{
	sha->inputLen = 0;
	sha->A = (ILUInt64)(0xcbbb9d5dc1059ed8LL);
	sha->B = (ILUInt64)(0x629a292a367cd507LL);
	sha->C = (ILUInt64)(0x9159015a3070dd17LL);
	sha->D = (ILUInt64)(0x152fecd8f70e5939LL);
	sha->E = (ILUInt64)(0x67332667ffc00b31LL);
	sha->F = (ILUInt64)(0x8eb44a8768581511LL);
	sha->G = (ILUInt64)(0xdb0c2e0d64f98fa7LL);
	sha->H = (ILUInt64)(0x47b5481dbefa4fa4LL);
	sha->totalLen = 0;
}

/* Note: ILSHA384Data is #ifdef'ed to ILSHA512Data */

void ILSHA384Finalize(ILSHA384Context *sha,
					  unsigned char hash[IL_SHA384_HASH_SIZE])
{
	unsigned char sha512hash[IL_SHA512_HASH_SIZE];
	ILSHA512Finalize(sha, sha512hash);
	if(hash)
	{
		ILMemCpy(hash, sha512hash, IL_SHA384_HASH_SIZE);
	}
	ILMemZero(sha512hash, IL_SHA512_HASH_SIZE);
}

#ifdef TEST_SHA384

#include <stdio.h>

/*
 * Define the test vectors and the expected answers.
 */
typedef struct
{
	const char *value;
	unsigned char expected[48];

} SHATestVector;
static SHATestVector vector1 = {
	"abc",
	{0xcb, 0x00, 0x75, 0x3f, 0x45, 0xa3, 0x5e, 0x8b,
	 0xb5, 0xa0, 0x3d, 0x69, 0x9a, 0xc6, 0x50, 0x07,
	 0x27, 0x2c, 0x32, 0xab, 0x0e, 0xde, 0xd1, 0x63,
	 0x1a, 0x8b, 0x60, 0x5a, 0x43, 0xff, 0x5b, 0xed,
	 0x80, 0x86, 0x07, 0x2b, 0xa1, 0xe7, 0xcc, 0x23,
	 0x58, 0xba, 0xec, 0xa1, 0x34, 0xc8, 0x25, 0xa7}
};
static SHATestVector vector2 = {
	"abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmn"
	"hijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu",
	{0x09, 0x33, 0x0c, 0x33, 0xf7, 0x11, 0x47, 0xe8,
	 0x3d, 0x19, 0x2f, 0xc7, 0x82, 0xcd, 0x1b, 0x47,
	 0x53, 0x11, 0x1b, 0x17, 0x3b, 0x3b, 0x05, 0xd2,
	 0x2f, 0xa0, 0x80, 0x86, 0xe3, 0xb0, 0xf7, 0x12,
	 0xfc, 0xc7, 0xc7, 0x1a, 0x55, 0x7e, 0x2d, 0xb9,
	 0x66, 0xc3, 0xe9, 0xfa, 0x91, 0x74, 0x60, 0x39}
};

/*
 * Print a 48-byte hash value.
 */
static void PrintHash(unsigned char *hash)
{
	printf("%02X%02X %02X%02X %02X%02X %02X%02X "
	       "%02X%02X %02X%02X %02X%02X %02X%02X "
	       "%02X%02X %02X%02X %02X%02X %02X%02X "
	       "%02X%02X %02X%02X %02X%02X %02X%02X "
	       "%02X%02X %02X%02X %02X%02X %02X%02X "
	       "%02X%02X %02X%02X %02X%02X %02X%02X\n",
		   hash[0], hash[1], hash[2], hash[3],
		   hash[4], hash[5], hash[6], hash[7],
		   hash[8], hash[9], hash[10], hash[11],
		   hash[12], hash[13], hash[14], hash[15],
		   hash[16], hash[17], hash[18], hash[19],
		   hash[20], hash[21], hash[22], hash[23],
		   hash[24], hash[25], hash[26], hash[27],
		   hash[28], hash[29], hash[30], hash[31],
		   hash[32], hash[33], hash[34], hash[35],
		   hash[36], hash[37], hash[38], hash[39],
		   hash[40], hash[41], hash[42], hash[43],
		   hash[44], hash[45], hash[46], hash[47]);
}

/*
 * Process a test vector.
 */
static void ProcessVector(SHATestVector *vector)
{
	ILSHA512Context sha;
	unsigned char hash[48];

	/* Compute the hash */
	ILSHA384Init(&sha);
	ILSHA384Data(&sha, vector->value, strlen(vector->value));
	ILSHA384Finalize(&sha, hash);

	/* Report the results */
	printf("Value    = %s\n", vector->value);
	printf("Expected = ");
	PrintHash(vector->expected);
	printf("Actual   = ");
	PrintHash(hash);
	if(ILMemCmp(vector->expected, hash, 48) != 0)
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

#endif /* TEST_SHA384 */

#ifdef	__cplusplus
};
#endif
