/*
 * sha512.c - Implementation of the Secure Hash Algorithm-512 (SHA-512).
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

/*
 * Some helper macros for processing 64-bit values.
 */
#define	ROTATE(x,n)		(((x) >> (n)) | ((x) << (64 - (n))))
#define	SHIFT(x,n)		((x) >> (n))

/*
 * Helper macros used by the SHA-512 computation.
 */
#define	CH(x,y,z)		(((x) & (y)) ^ ((~(x)) & (z)))
#define	MAJ(x,y,z)		(((x) & (y)) ^ ((x) & (z)) ^ ((y) & (z)))
#define	SUM0(x)			(ROTATE((x), 28) ^ ROTATE((x), 34) ^ ROTATE((x), 39))
#define	SUM1(x)			(ROTATE((x), 14) ^ ROTATE((x), 18) ^ ROTATE((x), 41))
#define	RHO0(x)			(ROTATE((x), 1) ^ ROTATE((x), 8) ^ SHIFT((x), 7))
#define	RHO1(x)			(ROTATE((x), 19) ^ ROTATE((x), 61) ^ SHIFT((x), 6))

/*
 * Constants used in each of the SHA-512 rounds.
 */
static ILUInt64 const K[80] = {
(ILUInt64)(0x428a2f98d728ae22LL), (ILUInt64)(0x7137449123ef65cdLL),
(ILUInt64)(0xb5c0fbcfec4d3b2fLL), (ILUInt64)(0xe9b5dba58189dbbcLL),
(ILUInt64)(0x3956c25bf348b538LL), (ILUInt64)(0x59f111f1b605d019LL),
(ILUInt64)(0x923f82a4af194f9bLL), (ILUInt64)(0xab1c5ed5da6d8118LL),
(ILUInt64)(0xd807aa98a3030242LL), (ILUInt64)(0x12835b0145706fbeLL),
(ILUInt64)(0x243185be4ee4b28cLL), (ILUInt64)(0x550c7dc3d5ffb4e2LL),
(ILUInt64)(0x72be5d74f27b896fLL), (ILUInt64)(0x80deb1fe3b1696b1LL),
(ILUInt64)(0x9bdc06a725c71235LL), (ILUInt64)(0xc19bf174cf692694LL),
(ILUInt64)(0xe49b69c19ef14ad2LL), (ILUInt64)(0xefbe4786384f25e3LL),
(ILUInt64)(0x0fc19dc68b8cd5b5LL), (ILUInt64)(0x240ca1cc77ac9c65LL),
(ILUInt64)(0x2de92c6f592b0275LL), (ILUInt64)(0x4a7484aa6ea6e483LL),
(ILUInt64)(0x5cb0a9dcbd41fbd4LL), (ILUInt64)(0x76f988da831153b5LL),
(ILUInt64)(0x983e5152ee66dfabLL), (ILUInt64)(0xa831c66d2db43210LL),
(ILUInt64)(0xb00327c898fb213fLL), (ILUInt64)(0xbf597fc7beef0ee4LL),
(ILUInt64)(0xc6e00bf33da88fc2LL), (ILUInt64)(0xd5a79147930aa725LL),
(ILUInt64)(0x06ca6351e003826fLL), (ILUInt64)(0x142929670a0e6e70LL),
(ILUInt64)(0x27b70a8546d22ffcLL), (ILUInt64)(0x2e1b21385c26c926LL),
(ILUInt64)(0x4d2c6dfc5ac42aedLL), (ILUInt64)(0x53380d139d95b3dfLL),
(ILUInt64)(0x650a73548baf63deLL), (ILUInt64)(0x766a0abb3c77b2a8LL),
(ILUInt64)(0x81c2c92e47edaee6LL), (ILUInt64)(0x92722c851482353bLL),
(ILUInt64)(0xa2bfe8a14cf10364LL), (ILUInt64)(0xa81a664bbc423001LL),
(ILUInt64)(0xc24b8b70d0f89791LL), (ILUInt64)(0xc76c51a30654be30LL),
(ILUInt64)(0xd192e819d6ef5218LL), (ILUInt64)(0xd69906245565a910LL),
(ILUInt64)(0xf40e35855771202aLL), (ILUInt64)(0x106aa07032bbd1b8LL),
(ILUInt64)(0x19a4c116b8d2d0c8LL), (ILUInt64)(0x1e376c085141ab53LL),
(ILUInt64)(0x2748774cdf8eeb99LL), (ILUInt64)(0x34b0bcb5e19b48a8LL),
(ILUInt64)(0x391c0cb3c5c95a63LL), (ILUInt64)(0x4ed8aa4ae3418acbLL),
(ILUInt64)(0x5b9cca4f7763e373LL), (ILUInt64)(0x682e6ff3d6b2b8a3LL),
(ILUInt64)(0x748f82ee5defb2fcLL), (ILUInt64)(0x78a5636f43172f60LL),
(ILUInt64)(0x84c87814a1f0ab72LL), (ILUInt64)(0x8cc702081a6439ecLL),
(ILUInt64)(0x90befffa23631e28LL), (ILUInt64)(0xa4506cebde82bde9LL),
(ILUInt64)(0xbef9a3f7b2c67915LL), (ILUInt64)(0xc67178f2e372532bLL),
(ILUInt64)(0xca273eceea26619cLL), (ILUInt64)(0xd186b8c721c0c207LL),
(ILUInt64)(0xeada7dd6cde0eb1eLL), (ILUInt64)(0xf57d4f7fee6ed178LL),
(ILUInt64)(0x06f067aa72176fbaLL), (ILUInt64)(0x0a637dc5a2c898a6LL),
(ILUInt64)(0x113f9804bef90daeLL), (ILUInt64)(0x1b710b35131c471bLL),
(ILUInt64)(0x28db77f523047d84LL), (ILUInt64)(0x32caab7b40c72493LL),
(ILUInt64)(0x3c9ebe0a15c9bebcLL), (ILUInt64)(0x431d67c49c100d4cLL),
(ILUInt64)(0x4cc5d4becb3e42b6LL), (ILUInt64)(0x597f299cfc657e2aLL),
(ILUInt64)(0x5fcb6fab3ad6faecLL), (ILUInt64)(0x6c44198c4a475817LL)
};

void ILSHA512Init(ILSHA512Context *sha)
{
	sha->inputLen = 0;
	sha->A = (ILUInt64)(0x6a09e667f3bcc908LL);
	sha->B = (ILUInt64)(0xbb67ae8584caa73bLL);
	sha->C = (ILUInt64)(0x3c6ef372fe94f82bLL);
	sha->D = (ILUInt64)(0xa54ff53a5f1d36f1LL);
	sha->E = (ILUInt64)(0x510e527fade682d1LL);
	sha->F = (ILUInt64)(0x9b05688c2b3e6c1fLL);
	sha->G = (ILUInt64)(0x1f83d9abfb41bd6bLL);
	sha->H = (ILUInt64)(0x5be0cd19137e2179LL);
	sha->totalLen = 0;
}

/*
 * Process a single block of input using the hash algorithm.
 */
static void ProcessBlock(ILSHA512Context *sha, const unsigned char *block)
{
	ILUInt64 W[80];
	ILUInt64 a, b, c, d, e, f, g, h;
	ILUInt64 temp, temp2;
	int t;

	/* Unpack the block into 80 64-bit words */
	for(t = 0; t < 16; ++t)
	{
		W[t] = (((ILUInt64)(block[t * 8 + 0])) << 56) |
		       (((ILUInt64)(block[t * 8 + 1])) << 48) |
		       (((ILUInt64)(block[t * 8 + 2])) << 40) |
		       (((ILUInt64)(block[t * 8 + 3])) << 32) |
		       (((ILUInt64)(block[t * 8 + 4])) << 24) |
		       (((ILUInt64)(block[t * 8 + 5])) << 16) |
		       (((ILUInt64)(block[t * 8 + 6])) <<  8) |
		        ((ILUInt64)(block[t * 8 + 7]));
	}
	for(t = 16; t < 80; ++t)
	{
		W[t] = RHO1(W[t - 2]) + W[t - 7] +
			   RHO0(W[t - 15]) + W[t - 16];
	}

	/* Load the SHA-512 state into local variables */
	a = sha->A;
	b = sha->B;
	c = sha->C;
	d = sha->D;
	e = sha->E;
	f = sha->F;
	g = sha->G;
	h = sha->H;

	/* Perform 80 rounds of hash computations */
	for(t = 0; t < 80; ++t)
	{
		temp = h + SUM1(e) + CH(e, f, g) + K[t] + W[t];
		temp2 = SUM0(a) + MAJ(a, b, c);
		h = g;
		g = f;
		f = e;
		e = d + temp;
		d = c;
		c = b;
		b = a;
		a = temp + temp2;
	}

	/* Combine the previous SHA-512 state with the new state */
	sha->A = sha->A + a;
	sha->B = sha->B + b;
	sha->C = sha->C + c;
	sha->D = sha->D + d;
	sha->E = sha->E + e;
	sha->F = sha->F + f;
	sha->G = sha->G + g;
	sha->H = sha->H + h;

	/* Clear the temporary state */
	ILMemZero(W, sizeof(ILUInt64) * 80);
	a = b = c = d = e = f = g = h = temp = temp2 = 0;
}

void ILSHA512Data(ILSHA512Context *sha, const void *buffer, unsigned long len)
{
	unsigned long templen;

	/* Add to the total length of the input stream */
	sha->totalLen += (ILUInt64)len;

	/* Copy the blocks into the input buffer and process them */
	while(len > 0)
	{
		if(!(sha->inputLen) && len >= 128)
		{
			/* Short cut: no point copying the data twice */
			ProcessBlock(sha, (const unsigned char *)buffer);
			buffer = (const void *)(((const unsigned char *)buffer) + 128);
			len -= 128;
		}
		else
		{
			templen = len;
			if(templen > (128 - sha->inputLen))
			{
				templen = 128 - sha->inputLen;
			}
			ILMemCpy(sha->input + sha->inputLen, buffer, templen);
			if((sha->inputLen += templen) >= 128)
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
 * Write a 64-bit big-endian long value to a buffer.
 */
static void WriteLong(unsigned char *buf, ILUInt64 value)
{
	buf[0] = (unsigned char)(value >> 56);
	buf[1] = (unsigned char)(value >> 48);
	buf[2] = (unsigned char)(value >> 40);
	buf[3] = (unsigned char)(value >> 32);
	buf[4] = (unsigned char)(value >> 24);
	buf[5] = (unsigned char)(value >> 16);
	buf[6] = (unsigned char)(value >> 8);
	buf[7] = (unsigned char)value;
}

void ILSHA512Finalize(ILSHA512Context *sha,
					  unsigned char hash[IL_SHA512_HASH_SIZE])
{
	ILUInt64 totalBits;

	/* Compute the final hash if necessary */
	if(hash)
	{
		/* Pad the input data to a multiple of 1024 bits */
		if(sha->inputLen >= (128 - 16))
		{
			/* Need two blocks worth of padding */
			sha->input[(sha->inputLen)++] = (unsigned char)0x80;
			while(sha->inputLen < 128)
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
		while(sha->inputLen < (128 - 16))
		{
			sha->input[(sha->inputLen)++] = (unsigned char)0x00;
		}
		totalBits = (sha->totalLen << 3);
		ILMemZero(sha->input + (128 - 16), 8);
		WriteLong(sha->input + (128 - 8), totalBits);
		ProcessBlock(sha, sha->input);

		/* Write the final hash value to the supplied buffer */
		WriteLong(hash,      sha->A);
		WriteLong(hash + 8,  sha->B);
		WriteLong(hash + 16, sha->C);
		WriteLong(hash + 24, sha->D);
		WriteLong(hash + 32, sha->E);
		WriteLong(hash + 40, sha->F);
		WriteLong(hash + 48, sha->G);
		WriteLong(hash + 56, sha->H);
	}

	/* Fill the entire context structure with zeros to blank it */
	ILMemZero(sha, sizeof(ILSHA512Context));
}

#ifdef TEST_SHA512

#include <stdio.h>

/*
 * Define the test vectors and the expected answers.
 */
typedef struct
{
	const char *value;
	unsigned char expected[64];

} SHATestVector;
static SHATestVector vector1 = {
	"abc",
	{0xdd, 0xaf, 0x35, 0xa1, 0x93, 0x61, 0x7a, 0xba,
	 0xcc, 0x41, 0x73, 0x49, 0xae, 0x20, 0x41, 0x31,
	 0x12, 0xe6, 0xfa, 0x4e, 0x89, 0xa9, 0x7e, 0xa2,
	 0x0a, 0x9e, 0xee, 0xe6, 0x4b, 0x55, 0xd3, 0x9a,
	 0x21, 0x92, 0x99, 0x2a, 0x27, 0x4f, 0xc1, 0xa8,
	 0x36, 0xba, 0x3c, 0x23, 0xa3, 0xfe, 0xeb, 0xbd,
	 0x45, 0x4d, 0x44, 0x23, 0x64, 0x3c, 0xe8, 0x0e,
	 0x2a, 0x9a, 0xc9, 0x4f, 0xa5, 0x4c, 0xa4, 0x9f}
};
static SHATestVector vector2 = {
	"abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmn"
	"hijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu",
	{0x8e, 0x95, 0x9b, 0x75, 0xda, 0xe3, 0x13, 0xda,
	 0x8c, 0xf4, 0xf7, 0x28, 0x14, 0xfc, 0x14, 0x3f,
	 0x8f, 0x77, 0x79, 0xc6, 0xeb, 0x9f, 0x7f, 0xa1,
	 0x72, 0x99, 0xae, 0xad, 0xb6, 0x88, 0x90, 0x18,
	 0x50, 0x1d, 0x28, 0x9e, 0x49, 0x00, 0xf7, 0xe4,
	 0x33, 0x1b, 0x99, 0xde, 0xc4, 0xb5, 0x43, 0x3a,
	 0xc7, 0xd3, 0x29, 0xee, 0xb6, 0xdd, 0x26, 0x54,
	 0x5e, 0x96, 0xe5, 0x5b, 0x87, 0x4b, 0xe9, 0x09}
};

/*
 * Print a 64-byte hash value.
 */
static void PrintHash(unsigned char *hash)
{
	printf("%02X%02X %02X%02X %02X%02X %02X%02X "
	       "%02X%02X %02X%02X %02X%02X %02X%02X "
	       "%02X%02X %02X%02X %02X%02X %02X%02X "
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
		   hash[44], hash[45], hash[46], hash[47],
		   hash[48], hash[49], hash[50], hash[51],
		   hash[52], hash[53], hash[54], hash[55],
		   hash[56], hash[57], hash[58], hash[59],
		   hash[60], hash[61], hash[62], hash[63]);
}

/*
 * Process a test vector.
 */
static void ProcessVector(SHATestVector *vector)
{
	ILSHA512Context sha;
	unsigned char hash[64];

	/* Compute the hash */
	ILSHA512Init(&sha);
	ILSHA512Data(&sha, vector->value, strlen(vector->value));
	ILSHA512Finalize(&sha, hash);

	/* Report the results */
	printf("Value    = %s\n", vector->value);
	printf("Expected = ");
	PrintHash(vector->expected);
	printf("Actual   = ");
	PrintHash(hash);
	if(ILMemCmp(vector->expected, hash, 64) != 0)
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

#endif /* TEST_SHA512 */

#ifdef	__cplusplus
};
#endif
