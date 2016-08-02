/*
 * des.c - Implementation of the Data Encryption Standard (DES)
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

#ifndef DESGENSP

/*
 * Define the SPBox tables.  These were generated using the
 * code listed at the end of this source file.
 */
static ILUInt32 const SPBox1[64] = {
 0x01010400, 0x00000000, 0x00010000, 0x01010404,
 0x01010004, 0x00010404, 0x00000004, 0x00010000,
 0x00000400, 0x01010400, 0x01010404, 0x00000400,
 0x01000404, 0x01010004, 0x01000000, 0x00000004,
 0x00000404, 0x01000400, 0x01000400, 0x00010400,
 0x00010400, 0x01010000, 0x01010000, 0x01000404,
 0x00010004, 0x01000004, 0x01000004, 0x00010004,
 0x00000000, 0x00000404, 0x00010404, 0x01000000,
 0x00010000, 0x01010404, 0x00000004, 0x01010000,
 0x01010400, 0x01000000, 0x01000000, 0x00000400,
 0x01010004, 0x00010000, 0x00010400, 0x01000004,
 0x00000400, 0x00000004, 0x01000404, 0x00010404,
 0x01010404, 0x00010004, 0x01010000, 0x01000404,
 0x01000004, 0x00000404, 0x00010404, 0x01010400,
 0x00000404, 0x01000400, 0x01000400, 0x00000000,
 0x00010004, 0x00010400, 0x00000000, 0x01010004,
};
static ILUInt32 const SPBox2[64] = {
 0x80108020, 0x80008000, 0x00008000, 0x00108020,
 0x00100000, 0x00000020, 0x80100020, 0x80008020,
 0x80000020, 0x80108020, 0x80108000, 0x80000000,
 0x80008000, 0x00100000, 0x00000020, 0x80100020,
 0x00108000, 0x00100020, 0x80008020, 0x00000000,
 0x80000000, 0x00008000, 0x00108020, 0x80100000,
 0x00100020, 0x80000020, 0x00000000, 0x00108000,
 0x00008020, 0x80108000, 0x80100000, 0x00008020,
 0x00000000, 0x00108020, 0x80100020, 0x00100000,
 0x80008020, 0x80100000, 0x80108000, 0x00008000,
 0x80100000, 0x80008000, 0x00000020, 0x80108020,
 0x00108020, 0x00000020, 0x00008000, 0x80000000,
 0x00008020, 0x80108000, 0x00100000, 0x80000020,
 0x00100020, 0x80008020, 0x80000020, 0x00100020,
 0x00108000, 0x00000000, 0x80008000, 0x00008020,
 0x80000000, 0x80100020, 0x80108020, 0x00108000,
};
static ILUInt32 const SPBox3[64] = {
 0x00000208, 0x08020200, 0x00000000, 0x08020008,
 0x08000200, 0x00000000, 0x00020208, 0x08000200,
 0x00020008, 0x08000008, 0x08000008, 0x00020000,
 0x08020208, 0x00020008, 0x08020000, 0x00000208,
 0x08000000, 0x00000008, 0x08020200, 0x00000200,
 0x00020200, 0x08020000, 0x08020008, 0x00020208,
 0x08000208, 0x00020200, 0x00020000, 0x08000208,
 0x00000008, 0x08020208, 0x00000200, 0x08000000,
 0x08020200, 0x08000000, 0x00020008, 0x00000208,
 0x00020000, 0x08020200, 0x08000200, 0x00000000,
 0x00000200, 0x00020008, 0x08020208, 0x08000200,
 0x08000008, 0x00000200, 0x00000000, 0x08020008,
 0x08000208, 0x00020000, 0x08000000, 0x08020208,
 0x00000008, 0x00020208, 0x00020200, 0x08000008,
 0x08020000, 0x08000208, 0x00000208, 0x08020000,
 0x00020208, 0x00000008, 0x08020008, 0x00020200,
};
static ILUInt32 const SPBox4[64] = {
 0x00802001, 0x00002081, 0x00002081, 0x00000080,
 0x00802080, 0x00800081, 0x00800001, 0x00002001,
 0x00000000, 0x00802000, 0x00802000, 0x00802081,
 0x00000081, 0x00000000, 0x00800080, 0x00800001,
 0x00000001, 0x00002000, 0x00800000, 0x00802001,
 0x00000080, 0x00800000, 0x00002001, 0x00002080,
 0x00800081, 0x00000001, 0x00002080, 0x00800080,
 0x00002000, 0x00802080, 0x00802081, 0x00000081,
 0x00800080, 0x00800001, 0x00802000, 0x00802081,
 0x00000081, 0x00000000, 0x00000000, 0x00802000,
 0x00002080, 0x00800080, 0x00800081, 0x00000001,
 0x00802001, 0x00002081, 0x00002081, 0x00000080,
 0x00802081, 0x00000081, 0x00000001, 0x00002000,
 0x00800001, 0x00002001, 0x00802080, 0x00800081,
 0x00002001, 0x00002080, 0x00800000, 0x00802001,
 0x00000080, 0x00800000, 0x00002000, 0x00802080,
};
static ILUInt32 const SPBox5[64] = {
 0x00000100, 0x02080100, 0x02080000, 0x42000100,
 0x00080000, 0x00000100, 0x40000000, 0x02080000,
 0x40080100, 0x00080000, 0x02000100, 0x40080100,
 0x42000100, 0x42080000, 0x00080100, 0x40000000,
 0x02000000, 0x40080000, 0x40080000, 0x00000000,
 0x40000100, 0x42080100, 0x42080100, 0x02000100,
 0x42080000, 0x40000100, 0x00000000, 0x42000000,
 0x02080100, 0x02000000, 0x42000000, 0x00080100,
 0x00080000, 0x42000100, 0x00000100, 0x02000000,
 0x40000000, 0x02080000, 0x42000100, 0x40080100,
 0x02000100, 0x40000000, 0x42080000, 0x02080100,
 0x40080100, 0x00000100, 0x02000000, 0x42080000,
 0x42080100, 0x00080100, 0x42000000, 0x42080100,
 0x02080000, 0x00000000, 0x40080000, 0x42000000,
 0x00080100, 0x02000100, 0x40000100, 0x00080000,
 0x00000000, 0x40080000, 0x02080100, 0x40000100,
};
static ILUInt32 const SPBox6[64] = {
 0x20000010, 0x20400000, 0x00004000, 0x20404010,
 0x20400000, 0x00000010, 0x20404010, 0x00400000,
 0x20004000, 0x00404010, 0x00400000, 0x20000010,
 0x00400010, 0x20004000, 0x20000000, 0x00004010,
 0x00000000, 0x00400010, 0x20004010, 0x00004000,
 0x00404000, 0x20004010, 0x00000010, 0x20400010,
 0x20400010, 0x00000000, 0x00404010, 0x20404000,
 0x00004010, 0x00404000, 0x20404000, 0x20000000,
 0x20004000, 0x00000010, 0x20400010, 0x00404000,
 0x20404010, 0x00400000, 0x00004010, 0x20000010,
 0x00400000, 0x20004000, 0x20000000, 0x00004010,
 0x20000010, 0x20404010, 0x00404000, 0x20400000,
 0x00404010, 0x20404000, 0x00000000, 0x20400010,
 0x00000010, 0x00004000, 0x20400000, 0x00404010,
 0x00004000, 0x00400010, 0x20004010, 0x00000000,
 0x20404000, 0x20000000, 0x00400010, 0x20004010,
};
static ILUInt32 const SPBox7[64] = {
 0x00200000, 0x04200002, 0x04000802, 0x00000000,
 0x00000800, 0x04000802, 0x00200802, 0x04200800,
 0x04200802, 0x00200000, 0x00000000, 0x04000002,
 0x00000002, 0x04000000, 0x04200002, 0x00000802,
 0x04000800, 0x00200802, 0x00200002, 0x04000800,
 0x04000002, 0x04200000, 0x04200800, 0x00200002,
 0x04200000, 0x00000800, 0x00000802, 0x04200802,
 0x00200800, 0x00000002, 0x04000000, 0x00200800,
 0x04000000, 0x00200800, 0x00200000, 0x04000802,
 0x04000802, 0x04200002, 0x04200002, 0x00000002,
 0x00200002, 0x04000000, 0x04000800, 0x00200000,
 0x04200800, 0x00000802, 0x00200802, 0x04200800,
 0x00000802, 0x04000002, 0x04200802, 0x04200000,
 0x00200800, 0x00000000, 0x00000002, 0x04200802,
 0x00000000, 0x00200802, 0x04200000, 0x00000800,
 0x04000002, 0x04000800, 0x00000800, 0x00200002,
};
static ILUInt32 const SPBox8[64] = {
 0x10001040, 0x00001000, 0x00040000, 0x10041040,
 0x10000000, 0x10001040, 0x00000040, 0x10000000,
 0x00040040, 0x10040000, 0x10041040, 0x00041000,
 0x10041000, 0x00041040, 0x00001000, 0x00000040,
 0x10040000, 0x10000040, 0x10001000, 0x00001040,
 0x00041000, 0x00040040, 0x10040040, 0x10041000,
 0x00001040, 0x00000000, 0x00000000, 0x10040040,
 0x10000040, 0x10001000, 0x00041040, 0x00040000,
 0x00041040, 0x00040000, 0x10041000, 0x00001000,
 0x00000040, 0x10040040, 0x00001000, 0x00041040,
 0x10001000, 0x00000040, 0x10000040, 0x10040000,
 0x10040040, 0x10000000, 0x00040000, 0x10001040,
 0x00000000, 0x10041040, 0x00040040, 0x10000040,
 0x10040000, 0x10001000, 0x10001040, 0x00000000,
 0x10041040, 0x00041000, 0x00041000, 0x00001040,
 0x00001040, 0x00040040, 0x10000000, 0x10041000,
};

/*
 * Definition of the PC1 table from FIPS 46-3.
 */
static unsigned char const PC1[56] = {
57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18,
10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22,
14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4
};

/*
 * Definition of the PC2 table from FIPS 46-3.
 */
static unsigned char const PC2[48] = {
14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10,
23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2,
41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48,
44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
};

/*
 * Map an index to a bit position within a C or D value.
 */
static ILUInt32 const BitToKS[28] = {
	((ILUInt32)1) << 27,
	((ILUInt32)1) << 26,
	((ILUInt32)1) << 25,
	((ILUInt32)1) << 24,
	((ILUInt32)1) << 23,
	((ILUInt32)1) << 22,
	((ILUInt32)1) << 21,
	((ILUInt32)1) << 20,
	((ILUInt32)1) << 19,
	((ILUInt32)1) << 18,
	((ILUInt32)1) << 17,
	((ILUInt32)1) << 16,
	((ILUInt32)1) << 15,
	((ILUInt32)1) << 14,
	((ILUInt32)1) << 13,
	((ILUInt32)1) << 12,
	((ILUInt32)1) << 11,
	((ILUInt32)1) << 10,
	((ILUInt32)1) <<  9,
	((ILUInt32)1) <<  8,
	((ILUInt32)1) <<  7,
	((ILUInt32)1) <<  6,
	((ILUInt32)1) <<  5,
	((ILUInt32)1) <<  4,
	((ILUInt32)1) <<  3,
	((ILUInt32)1) <<  2,
	((ILUInt32)1) <<  1,
	((ILUInt32)1) <<  0
};

/*
 * Left rotate values for each of the rounds.
 */
static unsigned char const LRotate[16] = {
1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1,
};

/*
 * Helper macros for reading and writing big-endian values.
 */
#define	IL_BREAD_UINT32(buf)	\
			((ILUInt32)(_IL_READ_BYTE((buf), 3) | \
					    _IL_READ_BYTE_SHIFT((buf), 2, 8) | \
					    _IL_READ_BYTE_SHIFT((buf), 1, 16) | \
					    _IL_READ_BYTE_SHIFT((buf), 0, 24)))
#define	IL_BWRITE_UINT32(buf, value)	\
			do { \
				(buf)[0] = (unsigned char)((value) >> 24); \
				(buf)[1] = (unsigned char)((value) >> 16); \
				(buf)[2] = (unsigned char)((value) >> 8); \
				(buf)[3] = (unsigned char)(value); \
			} while (0)

/*
 * Other helper macros.
 */
#define	ROTATE(x,n)		(((x) << (n)) | ((x) >> (32 - n)))
#define	ROTATE28(x,n)	((((x) << (n)) | ((x) >> (28 - n))) & 0x0FFFFFFF)
#define	IPSTEP(l,r,shift,mask)	\
			do { \
				temp = (((l) >> (shift)) ^ (r)) & (mask); \
				(r) ^= temp; \
				(l) ^= (temp << (shift)); \
			} while (0)
#define	GETSUB(n)		((temp >> (n)) & 0x3F)

/*
 * Create a key schedule from a 64-bit DES key.  Algorithm
 * based on the description in FIPS 46-3.
 */
static void CreateDESSchedule(ILDESContext *des, unsigned char *key,
							  int decrypt)
{
	ILUInt32 C, D;
	int bit, select, round, posn;
	ILUInt32 schedA, schedB;

	/* Create C and D from the input key */
	C = D = 0;
	for(bit = 0; bit < (56 / 2); ++bit)
	{
		select = PC1[bit] - 1;
		if((key[select / 8] & (0x80 >> (select % 8))) != 0)
		{
			C |= BitToKS[bit];
		}
	}
	for(bit = (56 / 2); bit < 56; ++bit)
	{
		select = PC1[bit] - 1;
		if((key[select / 8] & (0x80 >> (select % 8))) != 0)
		{
			D |= BitToKS[bit - (56 / 2)];
		}
	}

	/* Compute the key schedule from C and D */
	for(round = 0; round < 16; ++round)
	{
		/* Rotate C and D */
		C = ROTATE28(C, LRotate[round]);
		D = ROTATE28(D, LRotate[round]);

		/* Apply PC2 to C and D to get the key schedule for this round */
		schedA = 0;
		schedB = 0;
		for(bit = 0; bit < 24; ++bit)
		{
			/* Select the bits for "schedA" */
			select = PC2[bit] - 1;
			if(select < 28)
			{
				/* Pull the bit out of C */
				if((C & BitToKS[select]) != 0)
				{
					schedA |= BitToKS[bit + 4];
				}
			}
			else
			{
				/* Pull the bit out of D */
				if((D & BitToKS[select - 28]) != 0)
				{
					schedA |= BitToKS[bit + 4];
				}
			}

			/* Select the bits for "schedB" */
			select = PC2[bit + 24] - 1;
			if(select < 28)
			{
				/* Pull the bit out of C */
				if((C & BitToKS[select]) != 0)
				{
					schedB |= BitToKS[bit + 4];
				}
			}
			else
			{
				/* Pull the bit out of D */
				if((D & BitToKS[select - 28]) != 0)
				{
					schedB |= BitToKS[bit + 4];
				}
			}
		}

		/* Shift the 48-bit schedule into its final position */
		if(decrypt)
		{
			posn = 30 - round * 2;
		}
		else
		{
			posn = round * 2;
		}
		des->ks[posn] = ((schedA & 0x00FC0000) << 6) |
						((schedA & 0x00000FC0) << 10) |
						((schedB & 0x00FC0000) >> 10) |
						((schedB & 0x00000FC0) >> 6);
		des->ks[posn + 1] = ((schedA & 0x0003F000) << 12) |
							((schedA & 0x0000003F) << 16) |
							((schedB & 0x0003F000) >> 4) |
							 (schedB & 0x0000003F);
	}

	/* Clear temporary values */
	C = D = schedA = schedB = 0;
}

/*
 * Process a 64-bit block using a key schedule.
 *
 * The implementation of this function was inspired by
 * "Applied Cryptography", Second Edition.
 */
static void ProcessDESBlock(ILUInt32 *ks, unsigned char *input,
							unsigned char *output)
{
	ILUInt32 L, R, temp, temp2;
	int round;

	/* Read the input block into temporary variables */
	L = IL_BREAD_UINT32(input);
	R = IL_BREAD_UINT32(input + 4);

	/* Perform the input permutation */
	IPSTEP(L, R,  4, 0x0F0F0F0F);
	IPSTEP(L, R, 16, 0x0000FFFF);
	IPSTEP(R, L,  2, 0x33333333);
	IPSTEP(R, L,  8, 0x00FF00FF);
	R = ROTATE(R, 1);
	temp = (L ^ R) & 0xAAAAAAAA;
	L ^= temp;
	R ^= temp;
	L = ROTATE(L, 1);

	/* Perform 16 rounds of SP computations, two at a time */
	for(round = 0; round < 8; ++round)
	{
		temp = ROTATE(R, 28) ^ (*ks++);
		temp2 = SPBox7[GETSUB(0)]  | SPBox5[GETSUB(8)] |
		        SPBox3[GETSUB(16)] | SPBox1[GETSUB(24)];
		temp = R ^ (*ks++); 
		temp2 |= SPBox8[GETSUB(0)]  | SPBox6[GETSUB(8)] |
		         SPBox4[GETSUB(16)] | SPBox2[GETSUB(24)];
		L ^= temp2;
		temp = ROTATE(L, 28) ^ (*ks++);
		temp2 = SPBox7[GETSUB(0)]  | SPBox5[GETSUB(8)] |
		        SPBox3[GETSUB(16)] | SPBox1[GETSUB(24)];
		temp = L ^ (*ks++); 
		temp2 |= SPBox8[GETSUB(0)]  | SPBox6[GETSUB(8)] |
		         SPBox4[GETSUB(16)] | SPBox2[GETSUB(24)];
		R ^= temp2;
	}

	/* Perform the output permutation */
	R = ROTATE(R, 31);
	temp = (L ^ R) & 0xAAAAAAAA;
	L ^= temp;
	R ^= temp;
	L = ROTATE(L, 31);
	IPSTEP(L, R,  8, 0x00FF00FF);
	IPSTEP(L, R,  2, 0x33333333);
	IPSTEP(R, L, 16, 0x0000FFFF);
	IPSTEP(R, L,  4, 0x0F0F0F0F);

	/* Write the final state to the output block */
	IL_BWRITE_UINT32(output, R);
	IL_BWRITE_UINT32(output + 4, L);

	/* Clear sensitive information and exit */
	L = R = temp = temp2 = 0;
}

void ILDESInit(ILDESContext *des, unsigned char *key, int decrypt)
{
	CreateDESSchedule(des, key, decrypt);
}

void ILDESProcess(ILDESContext *des, unsigned char *input,
				  unsigned char *output)
{
	ProcessDESBlock(des->ks, input, output);
}

void ILDESFinalize(ILDESContext *des)
{
	ILMemZero(des, sizeof(ILDESContext));
}

void ILDES3Init(ILDES3Context *des3, unsigned char *key,
				int keyBits, int decrypt)
{
	if(keyBits == 192)
	{
		if(decrypt)
		{
			CreateDESSchedule(&(des3->k1), key + 16, 1);
			CreateDESSchedule(&(des3->k2), key + 8, 0);
			CreateDESSchedule(&(des3->k3), key, 1);
		}
		else
		{
			CreateDESSchedule(&(des3->k1), key, 0);
			CreateDESSchedule(&(des3->k2), key + 8, 1);
			CreateDESSchedule(&(des3->k3), key + 16, 0);
		}
	}
	else
	{
		CreateDESSchedule(&(des3->k1), key, decrypt);
		CreateDESSchedule(&(des3->k2), key + 8, !decrypt);
		ILMemCpy(&(des3->k3), &(des3->k1), sizeof(des3->k1));
	}
}

void ILDES3Process(ILDES3Context *des3, unsigned char *input,
				   unsigned char *output)
{
	ProcessDESBlock(des3->k1.ks, input, output);
	ProcessDESBlock(des3->k2.ks, output, output);
	ProcessDESBlock(des3->k3.ks, output, output);
}

void ILDES3Finalize(ILDES3Context *des3)
{
	ILMemZero(des3, sizeof(ILDES3Context));
}

/*
 * Tables of weak and semi-weak keys, including parity.  Extracted
 * from "Applied Cryptography", Second Edition.
 */
static unsigned char const WeakKeys[4][8] = {
	{0x01, 0x01, 0x01, 0x01,  0x01, 0x01, 0x01, 0x01},
	{0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E},
	{0xE0, 0xE0, 0xE0, 0xE0,  0xF1, 0xF1, 0xF1, 0xF1},
	{0xFE, 0xFE, 0xFE, 0xFE,  0xFE, 0xFE, 0xFE, 0xFE},
};
static unsigned char const SemiWeakKeys[12][8] = {
	{0x01, 0xFE, 0x01, 0xFE,  0x01, 0xFE, 0x01, 0xFE},
	{0xFE, 0x01, 0xFE, 0x01,  0xFE, 0x01, 0xFE, 0x01},
	{0x1F, 0xE0, 0x1F, 0xE0,  0x0E, 0xF1, 0x0E, 0xF1},
	{0xE0, 0x1F, 0xE0, 0x1F,  0xF1, 0x0E, 0xF1, 0x0E},

	{0x01, 0xE0, 0x01, 0xE0,  0x01, 0xF1, 0x01, 0xF1},
	{0xE0, 0x01, 0xE0, 0x01,  0xF1, 0x01, 0xF1, 0x01},
	{0x1F, 0xFE, 0x1F, 0xFE,  0x0E, 0xFE, 0x0E, 0xFE},
	{0xFE, 0x1F, 0xFE, 0x1F,  0xFE, 0x0E, 0xFE, 0x0E},

	{0x01, 0x1F, 0x01, 0x1F,  0x01, 0x0E, 0x01, 0x0E},
	{0x1F, 0x01, 0x1F, 0x01,  0x0E, 0x01, 0x0E, 0x01},
	{0xE0, 0xFE, 0xE0, 0xFE,  0xF1, 0xFE, 0xF1, 0xFE},
	{0xFE, 0xE0, 0xFE, 0xE0,  0xFE, 0xF1, 0xFE, 0xF1},
};

int ILDESIsWeakKey(unsigned char *key)
{
	int index1, index2;
	for(index1 = 0; index1 < 4; ++index1)
	{
		for(index2 = 0; index2 < 8; ++index2)
		{
			if(((key[index2] ^ WeakKeys[index1][index2]) & 0xFE) != 0)
			{
				break;
			}
		}
		if(index2 >= 8)
		{
			return 1;
		}
	}
	return 0;
}

int ILDESIsSemiWeakKey(unsigned char *key)
{
	int index1, index2;
	for(index1 = 0; index1 < 12; ++index1)
	{
		for(index2 = 0; index2 < 8; ++index2)
		{
			if(((key[index2] ^ SemiWeakKeys[index1][index2]) & 0xFE) != 0)
			{
				break;
			}
		}
		if(index2 >= 8)
		{
			return 1;
		}
	}
	return 0;
}

#else /* DESGENSP */

/*
 * Code to generate the SPBox tables from the raw values in
 * the FIPS 46-3 standard.
 */

/*
 * Definition of the S-boxes from the standard.
 */
static int const SBox1[64] = {
14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,
0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,
15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13
};
static int const SBox2[64] = {
15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,
3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,
13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9
};
static int const SBox3[64] = {
10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,
13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,
1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12
};
static int const SBox4[64] = {
7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,
13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,
3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14
};
static int const SBox5[64] = {
2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,
14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,
11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3
};
static int const SBox6[64] = {
12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,
10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,
4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13
};
static int const SBox7[64] = {
4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,
13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,
6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12
};
static int const SBox8[64] = {
13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,
1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,
2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11
};

/*
 * Definition of the P function from the standard.
 */
static int const P[32] = {
16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10,
2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25
};

/*
 * Permute a 32-bit value using P, and then rotate by 1 bit to
 * get it into the final position required by "ProcessDESBlock".
 */
static ILUInt32 Permute(ILUInt32 value)
{
	ILUInt32 result = 0;
	int posn;
	for(posn = 0; posn < 32; ++posn)
	{
		if((value & (((ILUInt32)1) << (32 - P[posn]))) != 0)
		{
			result |= (((ILUInt32)1) << (31 - posn));
		}
	}
	return (result << 1) | (result >> 31);
}

/*
 * Apply an S-box to a 6-bit value to get a 4-bit result, shifted
 * into its final position within a 32-bit result.
 */
static ILUInt32 ApplySBox(const int *sbox, int boxNum, ILUInt32 value)
{
	ILUInt32 index = 0;
	if((value & 0x20) != 0)
	{
		index |= 2;
	}
	if((value & 0x01) != 0)
	{
		index |= 1;
	}
	index = (index << 4) | ((value >> 1) & 0x0F);
	return (((ILUInt32)(sbox[index])) << (28 - (boxNum * 4)));
}

/*
 * Generate an SPBox array.
 */
static void GenSPBox(const int *sbox, int boxNum)
{
	ILUInt32 index;
	ILUInt32 value;
	printf("static ILUInt32 const SPBox%d[64] = {\n", boxNum + 1);
	for(index = 0; index < 64; ++index)
	{
		value = Permute(ApplySBox(sbox, boxNum, index));
		printf(" 0x%08lX,", (unsigned long)value);
		if((index & 3) == 3)
		{
			printf("\n");
		}
	}
	printf("};\n");
}

/*
 * Generate all of the SPBox arrays.
 */
int main(int argc, char *argv[])
{
	GenSPBox(SBox1, 0);
	GenSPBox(SBox2, 1);
	GenSPBox(SBox3, 2);
	GenSPBox(SBox4, 3);
	GenSPBox(SBox5, 4);
	GenSPBox(SBox6, 5);
	GenSPBox(SBox7, 6);
	GenSPBox(SBox8, 7);
	return 0;
}

#endif /* DESGENSP */

#ifdef TEST_DES

#include <stdio.h>

/*
 * Selected test vectors from various NIST publications.
 */
typedef struct
{
	unsigned char key[24];
	int keyBits;
	unsigned char plaintext[8];
	unsigned char expected[8];

} DESTestVector;
static DESTestVector vector1 = {
	{0x10, 0x31, 0x6E, 0x02, 0x8C, 0x8F, 0x3B, 0x4A},
	64,
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0x82, 0xDC, 0xBA, 0xFB, 0xDE, 0xAB, 0x66, 0x02}
};
static DESTestVector vector2 = {
	{0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01},
	64,
	{0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0x95, 0xF8, 0xA5, 0xE5, 0xDD, 0x31, 0xD9, 0x00}
};
static DESTestVector vector3 = {
	{0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01},
	64,
	{0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0xDD, 0x7F, 0x12, 0x1C, 0xA5, 0x01, 0x56, 0x19}
};
static DESTestVector vector4 = {
	{0x80, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01},
	64,
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0x95, 0xA8, 0xD7, 0x28, 0x13, 0xDA, 0xA9, 0x4D}
};
static DESTestVector vector5 = {
	{0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef},
	64,
	{0x4e, 0x6f, 0x77, 0x20, 0x69, 0x73, 0x20, 0x74},
	{0x3f, 0xa4, 0x0e, 0x8a, 0x98, 0x4d, 0x48, 0x15}
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
static void ProcessVector(DESTestVector *vector)
{
	ILDES3Context des3enc;
	ILDES3Context des3dec;
	unsigned char ciphertext[8];
	unsigned char reverse[8];

	/* Encrypt and decrypt the plaintext */
	if(vector->keyBits == 64)
	{
		/* Use single DES */
		ILDESInit(&(des3enc.k1), vector->key, 0);
		ILDESInit(&(des3dec.k1), vector->key, 1);
		ILDESProcess(&(des3enc.k1), vector->plaintext, ciphertext);
		ILDESProcess(&(des3dec.k1), ciphertext, reverse);
		ILDESFinalize(&(des3enc.k1));
		ILDESFinalize(&(des3dec.k1));
	}
	else
	{
		/* Use Triple-DES */
		ILDES3Init(&des3enc, vector->key, vector->keyBits, 0);
		ILDES3Init(&des3dec, vector->key, vector->keyBits, 1);
		ILDES3Process(&des3enc, vector->plaintext, ciphertext);
		ILDES3Process(&des3dec, ciphertext, reverse);
		ILDES3Finalize(&des3enc);
		ILDES3Finalize(&des3dec);
	}

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
	ProcessVector(&vector5);
	return 0;
}

#endif /* TEST_DES */

#ifdef	__cplusplus
};
#endif
