/*
 * rc2.c - Implementation of the RC2 encryption algorithm.
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
 * This file implements the RC2 symmetric encryption algorithm,
 * based on the description in RFC 2268.
 */

#include "il_crypt.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Some alpha versions of gcc have problems assigning to unsigned short's,
 * which causes gcc to lock-up.  We work around this using "ILUInt32".
 * This isn't terribly elegant - any suggestions as to what is really
 * happening are welcome.
 *
 * ARM machines also seem to have problems with unsigned short arithmetic,
 * so we apply the same technique and it appears to address the problem.
 */
#if defined(__alpha__) || defined(__arm__)
typedef	ILUInt32	RWorkType;
#define	NORM(x)		((x) &= 0xFFFF)
#else
typedef	ILUInt16	RWorkType;
#define	NORM(x)
#endif

/*
 * Perform a "mix" operation.
 */
#define	MIX(rindex,kindex,svalue)	\
			do { \
				R[(rindex)] += K[(kindex)] + \
					(R[((rindex) - 1) & 3] & R[((rindex) - 2) & 3]) + \
					((~(R[((rindex) - 1) & 3])) & R[((rindex) - 3) & 3]); \
				NORM(R[(rindex)]); \
				R[(rindex)] = (R[(rindex)] << (svalue)) | \
							  (R[(rindex)] >> (16 - (svalue))); \
				NORM(R[(rindex)]); \
			} while (0)
#define	MIXROUND(kindex)	\
			do { \
				MIX(0, (kindex), 1); \
				MIX(1, (kindex) + 1, 2); \
				MIX(2, (kindex) + 2, 3); \
				MIX(3, (kindex) + 3, 5); \
			} while (0)

/*
 * Perform a "mash" operation.
 */
#define	MASH(rindex)	\
			do { \
				R[(rindex)] += K[R[((rindex) - 1) & 3] & 63]; \
				NORM(R[(rindex)]); \
			} while (0)
#define	MASHROUND()		\
			do { \
				MASH(0); \
				MASH(1); \
				MASH(2); \
				MASH(3); \
			} while (0)

/*
 * Perform a "reverse mix" operation.
 */
#define	RMIX(rindex,kindex,svalue)	\
			do { \
				R[(rindex)] = (R[(rindex)] >> (svalue)) | \
							  (R[(rindex)] << (16 - (svalue))); \
				NORM(R[(rindex)]); \
				R[(rindex)] -= K[(kindex)] + \
					(R[((rindex) - 1) & 3] & R[((rindex) - 2) & 3]) + \
					((~(R[((rindex) - 1) & 3])) & R[((rindex) - 3) & 3]); \
				NORM(R[(rindex)]); \
			} while (0)
#define	RMIXROUND(kindex)	\
			do { \
				RMIX(3, (kindex) + 3, 5); \
				RMIX(2, (kindex) + 2, 3); \
				RMIX(1, (kindex) + 1, 2); \
				RMIX(0, (kindex), 1); \
			} while (0)

/*
 * Perform a "reverse mash" operation.
 */
#define	RMASH(rindex)	\
			do { \
				R[(rindex)] -= K[R[((rindex) - 1) & 3] & 63]; \
				NORM(R[(rindex)]); \
			} while (0)
#define	RMASHROUND()		\
			do { \
				RMASH(3); \
				RMASH(2); \
				RMASH(1); \
				RMASH(0); \
			} while (0)

/*
 * Contents of "PITABLE" for the key expansion algorithm.
 */
static unsigned char const PITABLE[256] = {
	0xd9, 0x78, 0xf9, 0xc4, 0x19, 0xdd, 0xb5, 0xed,
	0x28, 0xe9, 0xfd, 0x79, 0x4a, 0xa0, 0xd8, 0x9d,
	0xc6, 0x7e, 0x37, 0x83, 0x2b, 0x76, 0x53, 0x8e,
	0x62, 0x4c, 0x64, 0x88, 0x44, 0x8b, 0xfb, 0xa2,
	0x17, 0x9a, 0x59, 0xf5, 0x87, 0xb3, 0x4f, 0x13,
	0x61, 0x45, 0x6d, 0x8d, 0x09, 0x81, 0x7d, 0x32,
	0xbd, 0x8f, 0x40, 0xeb, 0x86, 0xb7, 0x7b, 0x0b,
	0xf0, 0x95, 0x21, 0x22, 0x5c, 0x6b, 0x4e, 0x82,
	0x54, 0xd6, 0x65, 0x93, 0xce, 0x60, 0xb2, 0x1c,
	0x73, 0x56, 0xc0, 0x14, 0xa7, 0x8c, 0xf1, 0xdc,
	0x12, 0x75, 0xca, 0x1f, 0x3b, 0xbe, 0xe4, 0xd1,
	0x42, 0x3d, 0xd4, 0x30, 0xa3, 0x3c, 0xb6, 0x26,
	0x6f, 0xbf, 0x0e, 0xda, 0x46, 0x69, 0x07, 0x57,
	0x27, 0xf2, 0x1d, 0x9b, 0xbc, 0x94, 0x43, 0x03,
	0xf8, 0x11, 0xc7, 0xf6, 0x90, 0xef, 0x3e, 0xe7,
	0x06, 0xc3, 0xd5, 0x2f, 0xc8, 0x66, 0x1e, 0xd7,
	0x08, 0xe8, 0xea, 0xde, 0x80, 0x52, 0xee, 0xf7,
	0x84, 0xaa, 0x72, 0xac, 0x35, 0x4d, 0x6a, 0x2a,
	0x96, 0x1a, 0xd2, 0x71, 0x5a, 0x15, 0x49, 0x74,
	0x4b, 0x9f, 0xd0, 0x5e, 0x04, 0x18, 0xa4, 0xec,
	0xc2, 0xe0, 0x41, 0x6e, 0x0f, 0x51, 0xcb, 0xcc,
	0x24, 0x91, 0xaf, 0x50, 0xa1, 0xf4, 0x70, 0x39,
	0x99, 0x7c, 0x3a, 0x85, 0x23, 0xb8, 0xb4, 0x7a,
	0xfc, 0x02, 0x36, 0x5b, 0x25, 0x55, 0x97, 0x31,
	0x2d, 0x5d, 0xfa, 0x98, 0xe3, 0x8a, 0x92, 0xae,
	0x05, 0xdf, 0x29, 0x10, 0x67, 0x6c, 0xba, 0xc9,
	0xd3, 0x00, 0xe6, 0xcf, 0xe1, 0x9e, 0xa8, 0x2c,
	0x63, 0x16, 0x01, 0x3f, 0x58, 0xe2, 0x89, 0xa9,
	0x0d, 0x38, 0x34, 0x1b, 0xab, 0x33, 0xff, 0xb0,
	0xbb, 0x48, 0x0c, 0x5f, 0xb9, 0xb1, 0xcd, 0x2e,
	0xc5, 0xf3, 0xdb, 0x47, 0xe5, 0xa5, 0x9c, 0x77,
	0x0a, 0xa6, 0x20, 0x68, 0xfe, 0x7f, 0xc1, 0xad
};

void ILRC2Init(ILRC2Context *rc2, unsigned char *key, int keyBits)
{
	int size, posn, mask;
	unsigned char sched[128];

	/* Copy the key to the front of the expanded key schedule */
	if(keyBits <= (128 * 8))
	{
		size = (keyBits + 7) / 8;
		mask = ((1 << (8 - (8 * size - keyBits))) - 1);
	}
	else
	{
		size = 128;
		mask = 0xFF;
	}
	ILMemCpy(sched, key, size);

	/* Expand the key to the rest of the schedule */
	for(posn = size; posn < 128; ++posn)
	{
		sched[posn] = PITABLE[(sched[posn - 1] + sched[posn - size]) & 0xFF];
	}
	sched[128 - size] = PITABLE[sched[128 - size] & mask];
	for(posn = 128 - size - 1; posn >= 0; --posn)
	{
		sched[posn] = PITABLE[sched[posn + 1] ^ sched[posn + size]];
	}

	/* Convert the key schedule bytes into a list of 16-bit words */
	for(posn = 0; posn < 64; ++posn)
	{
		rc2->key[posn] = IL_READ_UINT16(sched + posn * 2);
	}
	ILMemZero(sched, sizeof(sched));
}

void ILRC2Encrypt(ILRC2Context *rc2, unsigned char *input,
				  unsigned char *output)
{
	register const ILUInt16 *K = rc2->key;
	RWorkType R[4];

	/* Copy the input buffer into the "R" array */
	R[0] = IL_READ_UINT16(input);
	R[1] = IL_READ_UINT16(input + 2);
	R[2] = IL_READ_UINT16(input + 4);
	R[3] = IL_READ_UINT16(input + 6);

	/* Perform five mixing rounds */
	MIXROUND(0);
	MIXROUND(4);
	MIXROUND(8);
	MIXROUND(12);
	MIXROUND(16);

	/* Perform one mashing round */
	MASHROUND();

	/* Perform six mixing rounds */
	MIXROUND(20);
	MIXROUND(24);
	MIXROUND(28);
	MIXROUND(32);
	MIXROUND(36);
	MIXROUND(40);

	/* Perform one mashing round */
	MASHROUND();

	/* Perform five mixing rounds */
	MIXROUND(44);
	MIXROUND(48);
	MIXROUND(52);
	MIXROUND(56);
	MIXROUND(60);

	/* Copy the final "R" state into the output buffer */
	IL_WRITE_UINT16(output, R[0]);
	IL_WRITE_UINT16(output + 2, R[1]);
	IL_WRITE_UINT16(output + 4, R[2]);
	IL_WRITE_UINT16(output + 6, R[3]);
}

void ILRC2Decrypt(ILRC2Context *rc2, unsigned char *input,
				  unsigned char *output)
{
	register const ILUInt16 *K = rc2->key;
	RWorkType R[4];

	/* Copy the input buffer into the "R" array */
	R[0] = IL_READ_UINT16(input);
	R[1] = IL_READ_UINT16(input + 2);
	R[2] = IL_READ_UINT16(input + 4);
	R[3] = IL_READ_UINT16(input + 6);

	/* Perform five reverse mixing rounds */
	RMIXROUND(60);
	RMIXROUND(56);
	RMIXROUND(52);
	RMIXROUND(48);
	RMIXROUND(44);

	/* Perform one reverse mashing round */
	RMASHROUND();

	/* Perform six reverse mixing rounds */
	RMIXROUND(40);
	RMIXROUND(36);
	RMIXROUND(32);
	RMIXROUND(28);
	RMIXROUND(24);
	RMIXROUND(20);

	/* Perform one reverse mashing round */
	RMASHROUND();

	/* Perform five reverse mixing rounds */
	RMIXROUND(16);
	RMIXROUND(12);
	RMIXROUND(8);
	RMIXROUND(4);
	RMIXROUND(0);

	/* Copy the final "R" state into the output buffer */
	IL_WRITE_UINT16(output, R[0]);
	IL_WRITE_UINT16(output + 2, R[1]);
	IL_WRITE_UINT16(output + 4, R[2]);
	IL_WRITE_UINT16(output + 6, R[3]);
}

void ILRC2Finalize(ILRC2Context *rc2)
{
	ILMemZero(rc2, sizeof(ILRC2Context));
}

#ifdef TEST_RC2

#include <stdio.h>

/*
 * Define the test vectors from RFC 2268.  We only use the vectors
 * where the actual and effective key lengths are similar.
 */
typedef struct
{
	unsigned char key[33];
	int keyBits;
	unsigned char plaintext[8];
	unsigned char expected[8];

} RC2TestVector;
static RC2TestVector vector1 = {
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	63,
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0xeb, 0xb7, 0x73, 0xf9, 0x93, 0x27, 0x8e, 0xff}
};
static RC2TestVector vector2 = {
	{0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff},
	64,
	{0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff},
	{0x27, 0x8b, 0x27, 0xe4, 0x2e, 0x2f, 0x0d, 0x49}
};
static RC2TestVector vector3 = {
	{0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	64,
	{0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01},
	{0x30, 0x64, 0x9e, 0xdf, 0x9b, 0xe7, 0xd2, 0xc2}
};
static RC2TestVector vector4 = {
	{0x88, 0xbc, 0xa9, 0x0e, 0x90, 0x87, 0x5a, 0x7f,
	 0x0f, 0x79, 0xc3, 0x84, 0x62, 0x7b, 0xaf, 0xb2},
	128,
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0x22, 0x69, 0x55, 0x2a, 0xb0, 0xf8, 0x5c, 0xa6}
};

/*
 * Print a hex buffer.
 */
static void PrintHex(unsigned char *buffer, int numBits)
{
	int count = 0;
	while(numBits > 0)
	{
		printf("%02X", buffer[0]);
		++count;
		if((count % 2) == 0)
		{
			putchar(' ');
		}
		++buffer;
		numBits -= 8;
	}
	putchar('\n');
}

/*
 * Process a test vector.
 */
static void ProcessVector(RC2TestVector *vector)
{
	ILRC2Context rc2;
	unsigned char ciphertext[8];
	unsigned char reverse[8];

	/* Encrypt and decrypt the plaintext */
	ILRC2Init(&rc2, vector->key, vector->keyBits);
	ILRC2Encrypt(&rc2, vector->plaintext, ciphertext);
	ILRC2Decrypt(&rc2, ciphertext, reverse);
	ILRC2Finalize(&rc2);

	/* Report the results */
	printf("Key                 = ");
	PrintHex(vector->key, vector->keyBits);
	printf("Plaintext           = ");
	PrintHex(vector->plaintext, 64);
	printf("Expected Ciphertext = ");
	PrintHex(vector->expected, 64);
	printf("Actual Ciphertext   = ");
	PrintHex(ciphertext, 64);
	printf("Reverse Plaintext   = ");
	PrintHex(reverse, 64);
	if(ILMemCmp(vector->expected, ciphertext, 8) != 0)
	{
		printf("*** Test failed: ciphertexts do not match ***\n");
	}
	if(ILMemCmp(vector->plaintext, reverse, 8) != 0)
	{
		printf("*** Test failed: plaintexts do not match ***\n");
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
	return 0;
}

#endif /* TEST_RC2 */

#ifdef	__cplusplus
};
#endif
