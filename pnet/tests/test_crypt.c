/*
 * test_crypt.c - Test the cryptographic primitives in "support".
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

#include "ilunit.h"
#include "il_crypt.h"
#include "il_bignum.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Test information record for hash algorithms.
 */
typedef struct
{
	const char   *input;
	unsigned char output[64];

} HashTestInfo;

/*
 * Test information record for block cipher algorithms.
 */
typedef struct
{
	unsigned char key[32];
	int keyBits;
	unsigned char plaintext[16];
	unsigned char expected[16];

} BlockTestInfo;

/*
 * Test information record for big number operations.
 */
typedef ILBigNum *(*BigNumFunc)(ILBigNum *x, ILBigNum *y, ILBigNum *modulus);
typedef ILBigNum *(*BigNumUnaryFunc)(ILBigNum *x, ILBigNum *modulus);
typedef struct
{
	BigNumFunc  func;
	const char *x;
	const char *y;
	const char *modulus;
	const char *result;

} BigNumTestInfo;

/*
 * Test vectors for the MD5 algorithm.
 */
static HashTestInfo md5_hash_1 = {
	"",
	{0xD4, 0x1D, 0x8C, 0xD9, 0x8F, 0x00, 0xB2, 0x04,
	 0xE9, 0x80, 0x09, 0x98, 0xEC, 0xF8, 0x42, 0x7E}
};
static HashTestInfo md5_hash_2 = {
	"a",
	{0x0C, 0xC1, 0x75, 0xB9, 0xC0, 0xF1, 0xB6, 0xA8,
	 0x31, 0xC3, 0x99, 0xE2, 0x69, 0x77, 0x26, 0x61}
};
static HashTestInfo md5_hash_3 = {
	"abc",
	{0x90, 0x01, 0x50, 0x98, 0x3C, 0xD2, 0x4F, 0xB0,
	 0xD6, 0x96, 0x3F, 0x7D, 0x28, 0xE1, 0x7F, 0x72}
};
static HashTestInfo md5_hash_4 = {
	"message digest",
	{0xF9, 0x6B, 0x69, 0x7D, 0x7C, 0xB7, 0x93, 0x8D,
	 0x52, 0x5A, 0x2F, 0x31, 0xAA, 0xF1, 0x61, 0xD0}
};
static HashTestInfo md5_hash_5 = {
	"abcdefghijklmnopqrstuvwxyz",
	{0xC3, 0xFC, 0xD3, 0xD7, 0x61, 0x92, 0xE4, 0x00,
	 0x7D, 0xFB, 0x49, 0x6C, 0xCA, 0x67, 0xE1, 0x3B}
};
static HashTestInfo md5_hash_6 = {
	"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
	{0xD1, 0x74, 0xAB, 0x98, 0xD2, 0x77, 0xD9, 0xF5,
	 0xA5, 0x61, 0x1C, 0x2C, 0x9F, 0x41, 0x9D, 0x9F}
};
static HashTestInfo md5_hash_7 = {
	"123456789012345678901234567890123456789012345678901234567890123456"
	"78901234567890",
	{0x57, 0xED, 0xF4, 0xA2, 0x2B, 0xE3, 0xC9, 0x55,
	 0xAC, 0x49, 0xDA, 0x2E, 0x21, 0x07, 0xB6, 0x7A}
};

/*
 * Test the MD5 hash algorithm.
 */
static void test_md5_hash(HashTestInfo *arg)
{
	ILMD5Context md5;
	unsigned char hash[IL_MD5_HASH_SIZE];

	/* Compute the hash over the input */
	ILMD5Init(&md5);
	ILMD5Data(&md5, arg->input, strlen(arg->input));
	ILMD5Finalize(&md5, hash);

	/* Compare the outputs */
	if(ILMemCmp(hash, arg->output, IL_MD5_HASH_SIZE) != 0)
	{
		ILUnitFail();
	}
}

/*
 * Test vectors for the SHA-1 algorithm.
 */
static HashTestInfo sha1_hash_1 = {
	"abc",
	{0xA9, 0x99, 0x3E, 0x36, 0x47, 0x06, 0x81, 0x6A, 0xBA, 0x3E,
	 0x25, 0x71, 0x78, 0x50, 0xC2, 0x6C, 0x9C, 0xD0, 0xD8, 0x9D}
};
static HashTestInfo sha1_hash_2 = {
	"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq",
	{0x84, 0x98, 0x3E, 0x44, 0x1C, 0x3B, 0xD2, 0x6E, 0xBA, 0xAE,
	 0x4A, 0xA1, 0xF9, 0x51, 0x29, 0xE5, 0xE5, 0x46, 0x70, 0xF1}
};

/*
 * Test the SHA-1 hash algorithm.
 */
static void test_sha1_hash(HashTestInfo *arg)
{
	ILSHAContext sha1;
	unsigned char hash[IL_SHA_HASH_SIZE];

	/* Compute the hash over the input */
	ILSHAInit(&sha1);
	ILSHAData(&sha1, arg->input, strlen(arg->input));
	ILSHAFinalize(&sha1, hash);

	/* Compare the outputs */
	if(ILMemCmp(hash, arg->output, IL_SHA_HASH_SIZE) != 0)
	{
		ILUnitFail();
	}
}

/*
 * Test vectors for the SHA-256 algorithm.
 */
static HashTestInfo sha256_hash_1 = {
	"abc",
	{0xba, 0x78, 0x16, 0xbf, 0x8f, 0x01, 0xcf, 0xea,
	 0x41, 0x41, 0x40, 0xde, 0x5d, 0xae, 0x22, 0x23,
	 0xb0, 0x03, 0x61, 0xa3, 0x96, 0x17, 0x7a, 0x9c,
	 0xb4, 0x10, 0xff, 0x61, 0xf2, 0x00, 0x15, 0xad}
};
static HashTestInfo sha256_hash_2 = {
	"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq",
	{0x24, 0x8d, 0x6a, 0x61, 0xd2, 0x06, 0x38, 0xb8,
	 0xe5, 0xc0, 0x26, 0x93, 0x0c, 0x3e, 0x60, 0x39,
	 0xa3, 0x3c, 0xe4, 0x59, 0x64, 0xff, 0x21, 0x67,
	 0xf6, 0xec, 0xed, 0xd4, 0x19, 0xdb, 0x06, 0xc1}
};

/*
 * Test the SHA-256 hash algorithm.
 */
static void test_sha256_hash(HashTestInfo *arg)
{
	ILSHA256Context sha256;
	unsigned char hash[IL_SHA256_HASH_SIZE];

	/* Compute the hash over the input */
	ILSHA256Init(&sha256);
	ILSHA256Data(&sha256, arg->input, strlen(arg->input));
	ILSHA256Finalize(&sha256, hash);

	/* Compare the outputs */
	if(ILMemCmp(hash, arg->output, IL_SHA256_HASH_SIZE) != 0)
	{
		ILUnitFail();
	}
}

/*
 * Test vectors for the SHA-384 algorithm.
 */
static HashTestInfo sha384_hash_1 = {
	"abc",
	{0xcb, 0x00, 0x75, 0x3f, 0x45, 0xa3, 0x5e, 0x8b,
	 0xb5, 0xa0, 0x3d, 0x69, 0x9a, 0xc6, 0x50, 0x07,
	 0x27, 0x2c, 0x32, 0xab, 0x0e, 0xde, 0xd1, 0x63,
	 0x1a, 0x8b, 0x60, 0x5a, 0x43, 0xff, 0x5b, 0xed,
	 0x80, 0x86, 0x07, 0x2b, 0xa1, 0xe7, 0xcc, 0x23,
	 0x58, 0xba, 0xec, 0xa1, 0x34, 0xc8, 0x25, 0xa7}
};
static HashTestInfo sha384_hash_2 = {
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
 * Test the SHA-384 hash algorithm.
 */
static void test_sha384_hash(HashTestInfo *arg)
{
	ILSHA384Context sha384;
	unsigned char hash[IL_SHA384_HASH_SIZE];

	/* Compute the hash over the input */
	ILSHA384Init(&sha384);
	ILSHA384Data(&sha384, arg->input, strlen(arg->input));
	ILSHA384Finalize(&sha384, hash);

	/* Compare the outputs */
	if(ILMemCmp(hash, arg->output, IL_SHA384_HASH_SIZE) != 0)
	{
		ILUnitFail();
	}
}

/*
 * Test vectors for the SHA-512 algorithm.
 */
static HashTestInfo sha512_hash_1 = {
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
static HashTestInfo sha512_hash_2 = {
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
 * Test the SHA-512 hash algorithm.
 */
static void test_sha512_hash(HashTestInfo *arg)
{
	ILSHA512Context sha512;
	unsigned char hash[IL_SHA512_HASH_SIZE];

	/* Compute the hash over the input */
	ILSHA512Init(&sha512);
	ILSHA512Data(&sha512, arg->input, strlen(arg->input));
	ILSHA512Finalize(&sha512, hash);

	/* Compare the outputs */
	if(ILMemCmp(hash, arg->output, IL_SHA512_HASH_SIZE) != 0)
	{
		ILUnitFail();
	}
}

/*
 * Define test vectors for the AES algorithm.
 */
static BlockTestInfo aes_block_1 = {
	{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
	 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F},
	128,
	{0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
	 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF},
	{0x69, 0xC4, 0xE0, 0xD8, 0x6A, 0x7B, 0x04, 0x30,
	 0xD8, 0xCD, 0xB7, 0x80, 0x70, 0xB4, 0xC5, 0x5A}
};
static BlockTestInfo aes_block_2 = {
	{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
	 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
	 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17},
	192,
	{0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
	 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF},
	{0xDD, 0xA9, 0x7C, 0xA4, 0x86, 0x4C, 0xDF, 0xE0,
	 0x6E, 0xAF, 0x70, 0xA0, 0xEC, 0x0D, 0x71, 0x91}
};
static BlockTestInfo aes_block_3 = {
	{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
	 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
	 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17,
	 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F},
	256,
	{0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
	 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF},
	{0x8E, 0xA2, 0xB7, 0xCA, 0x51, 0x67, 0x45, 0xBF,
	 0xEA, 0xFC, 0x49, 0x90, 0x4B, 0x49, 0x60, 0x89}
};

/*
 * Test the AES block cipher algorithm.
 */
static void test_aes_block(BlockTestInfo *arg)
{
	ILAESContext aes;
	unsigned char ciphertext[16];
	unsigned char reverse[16];

	/* Encrypt and decrypt the plaintext */
	ILAESInit(&aes, arg->key, arg->keyBits);
	ILAESEncrypt(&aes, arg->plaintext, ciphertext);
	ILAESDecrypt(&aes, ciphertext, reverse);
	ILAESFinalize(&aes);

	/* Compare the outputs with the expected values */
	if(ILMemCmp(ciphertext, arg->expected, 16) != 0)
	{
		ILUnitFailed("ciphertexts don't match");
	}
	if(ILMemCmp(reverse, arg->plaintext, 16) != 0)
	{
		ILUnitFailed("plaintexts don't match");
	}
}

/*
 * Define test vectors for the DES algorithm.
 */
static BlockTestInfo des_block_1 = {
	{0x10, 0x31, 0x6E, 0x02, 0x8C, 0x8F, 0x3B, 0x4A},
	64,
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0x82, 0xDC, 0xBA, 0xFB, 0xDE, 0xAB, 0x66, 0x02}
};
static BlockTestInfo des_block_2 = {
	{0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01},
	64,
	{0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0x95, 0xF8, 0xA5, 0xE5, 0xDD, 0x31, 0xD9, 0x00}
};
static BlockTestInfo des_block_3 = {
	{0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01},
	64,
	{0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0xDD, 0x7F, 0x12, 0x1C, 0xA5, 0x01, 0x56, 0x19}
};
static BlockTestInfo des_block_4 = {
	{0x80, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01},
	64,
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0x95, 0xA8, 0xD7, 0x28, 0x13, 0xDA, 0xA9, 0x4D}
};
static BlockTestInfo des_block_5 = {
	{0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef},
	64,
	{0x4e, 0x6f, 0x77, 0x20, 0x69, 0x73, 0x20, 0x74},
	{0x3f, 0xa4, 0x0e, 0x8a, 0x98, 0x4d, 0x48, 0x15}
};

/*
 * Test the DES block cipher algorithm.
 */
static void test_des_block(BlockTestInfo *arg)
{
	ILDESContext des;
	unsigned char ciphertext[8];
	unsigned char reverse[8];

	/* Encrypt and decrypt the plaintext */
	ILDESInit(&des, arg->key, 0);
	ILDESProcess(&des, arg->plaintext, ciphertext);
	ILDESFinalize(&des);
	ILDESInit(&des, arg->key, 1);
	ILDESProcess(&des, ciphertext, reverse);
	ILDESFinalize(&des);

	/* Compare the outputs with the expected values */
	if(ILMemCmp(ciphertext, arg->expected, 8) != 0)
	{
		ILUnitFailed("ciphertexts don't match");
	}
	if(ILMemCmp(reverse, arg->plaintext, 8) != 0)
	{
		ILUnitFailed("plaintexts don't match");
	}
}

/*
 * Define test vectors for the Triple-DES algorithm.
 */
static BlockTestInfo des3_block_1 = {
	{0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef,
	 0xef, 0xcd, 0xab, 0x89, 0x67, 0x54, 0x32, 0x10},
	128,
	{0x4e, 0x6f, 0x77, 0x20, 0x69, 0x73, 0x20, 0x74},
	{0x00}
};
static BlockTestInfo des3_block_2 = {
	{0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef,
	 0xef, 0xcd, 0xab, 0x89, 0x67, 0x54, 0x32, 0x10,
	 0x11, 0x33, 0x66, 0x99, 0xCC, 0xFF, 0x22, 0x44},
	192,
	{0x4e, 0x6f, 0x77, 0x20, 0x69, 0x73, 0x20, 0x74},
	{0x00}
};

/*
 * Test the Triple-DES block cipher algorithm.
 */
static void test_des3_block(BlockTestInfo *arg)
{
	ILDES3Context des3;
	unsigned char ciphertext1[8];
	unsigned char ciphertext2[8];
	unsigned char reverse1[8];
	unsigned char reverse2[8];

	/* Verify that the Triple-DES implementation is consistent
	   with manual application of DES three times.  This checks
	   that the structure of the Triple-DES code is correct */
	if(arg->keyBits == 192)
	{
		/* Encrypt using three applications of DES */
		ILDESInit(&(des3.k1), arg->key, 0);
		ILDESInit(&(des3.k2), arg->key + 8, 1);
		ILDESInit(&(des3.k3), arg->key + 16, 0);
		ILDESProcess(&(des3.k1), arg->plaintext, ciphertext1);
		ILDESProcess(&(des3.k2), ciphertext1, ciphertext1);
		ILDESProcess(&(des3.k3), ciphertext1, ciphertext1);
		ILDESFinalize(&(des3.k1));
		ILDESFinalize(&(des3.k2));
		ILDESFinalize(&(des3.k3));

		/* Encrypt using 1 application of Triple-DES */
		ILDES3Init(&des3, arg->key, arg->keyBits, 0);
		ILDES3Process(&des3, arg->plaintext, ciphertext2);
		ILDES3Finalize(&des3);

		/* Decrypt using three applications of DES */
		ILDESInit(&(des3.k1), arg->key, 1);
		ILDESInit(&(des3.k2), arg->key + 8, 0);
		ILDESInit(&(des3.k3), arg->key + 16, 1);
		ILDESProcess(&(des3.k3), ciphertext1, reverse1);
		ILDESProcess(&(des3.k2), reverse1, reverse1);
		ILDESProcess(&(des3.k1), reverse1, reverse1);
		ILDESFinalize(&(des3.k1));
		ILDESFinalize(&(des3.k2));
		ILDESFinalize(&(des3.k3));

		/* Decrypt using 1 application of Triple-DES */
		ILDES3Init(&des3, arg->key, arg->keyBits, 1);
		ILDES3Process(&des3, ciphertext2, reverse2);
		ILDES3Finalize(&des3);
	}
	else
	{
		/* Encrypt using three applications of DES */
		ILDESInit(&(des3.k1), arg->key, 0);
		ILDESInit(&(des3.k2), arg->key + 8, 1);
		ILDESInit(&(des3.k3), arg->key, 0);
		ILDESProcess(&(des3.k1), arg->plaintext, ciphertext1);
		ILDESProcess(&(des3.k2), ciphertext1, ciphertext1);
		ILDESProcess(&(des3.k3), ciphertext1, ciphertext1);
		ILDESFinalize(&(des3.k1));
		ILDESFinalize(&(des3.k2));
		ILDESFinalize(&(des3.k3));

		/* Encrypt using 1 application of Triple-DES */
		ILDES3Init(&des3, arg->key, arg->keyBits, 0);
		ILDES3Process(&des3, arg->plaintext, ciphertext2);
		ILDES3Finalize(&des3);

		/* Decrypt using three applications of DES */
		ILDESInit(&(des3.k1), arg->key, 1);
		ILDESInit(&(des3.k2), arg->key + 8, 0);
		ILDESInit(&(des3.k3), arg->key, 1);
		ILDESProcess(&(des3.k3), ciphertext1, reverse1);
		ILDESProcess(&(des3.k2), reverse1, reverse1);
		ILDESProcess(&(des3.k1), reverse1, reverse1);
		ILDESFinalize(&(des3.k1));
		ILDESFinalize(&(des3.k2));
		ILDESFinalize(&(des3.k3));

		/* Decrypt using 1 application of Triple-DES */
		ILDES3Init(&des3, arg->key, arg->keyBits, 1);
		ILDES3Process(&des3, ciphertext2, reverse2);
		ILDES3Finalize(&des3);
	}

	/* Compare the DES and Triple-DES outputs */
	if(ILMemCmp(ciphertext1, ciphertext2, 8) != 0)
	{
		ILUnitFailed("ciphertexts don't match");
	}
	if(ILMemCmp(reverse1, reverse2, 8) != 0)
	{
		ILUnitFailed("plaintexts don't match");
	}
	if(ILMemCmp(reverse2, arg->plaintext, 8) != 0)
	{
		ILUnitFailed("did not return to original plaintext");
	}
}

/*
 * Define test vectors for the RC2 algorithm.
 */
static BlockTestInfo rc2_block_1 = {
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	63,
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0xeb, 0xb7, 0x73, 0xf9, 0x93, 0x27, 0x8e, 0xff}
};
static BlockTestInfo rc2_block_2 = {
	{0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff},
	64,
	{0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff},
	{0x27, 0x8b, 0x27, 0xe4, 0x2e, 0x2f, 0x0d, 0x49}
};
static BlockTestInfo rc2_block_3 = {
	{0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	64,
	{0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01},
	{0x30, 0x64, 0x9e, 0xdf, 0x9b, 0xe7, 0xd2, 0xc2}
};
static BlockTestInfo rc2_block_4 = {
	{0x88, 0xbc, 0xa9, 0x0e, 0x90, 0x87, 0x5a, 0x7f,
	 0x0f, 0x79, 0xc3, 0x84, 0x62, 0x7b, 0xaf, 0xb2},
	128,
	{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
	{0x22, 0x69, 0x55, 0x2a, 0xb0, 0xf8, 0x5c, 0xa6}
};

/*
 * Test the RC2 block cipher algorithm.
 */
static void test_rc2_block(BlockTestInfo *arg)
{
	ILRC2Context rc2;
	unsigned char ciphertext[8];
	unsigned char reverse[8];

	/* Encrypt and decrypt the plaintext */
	ILRC2Init(&rc2, arg->key, arg->keyBits);
	ILRC2Encrypt(&rc2, arg->plaintext, ciphertext);
	ILRC2Decrypt(&rc2, ciphertext, reverse);
	ILRC2Finalize(&rc2);

	/* Compare the outputs with the expected values */
	if(ILMemCmp(ciphertext, arg->expected, 8) != 0)
	{
		ILUnitFailed("ciphertexts don't match");
	}
	if(ILMemCmp(reverse, arg->plaintext, 8) != 0)
	{
		ILUnitFailed("plaintexts don't match");
	}
}

/*
 * Convert a decimal string into a big number value.
 */
static ILBigNum *StrToBigNum(const char *str)
{
	unsigned char *temp;
	ILBigNum *num;
	ILBigNum *zero;
	ILBigNum *negnum;
	int len, size, posn;
	ILUInt32 carry;
	int negative;

	/* Allocate a buffer to hold the initial value */
	len = strlen(str);
	if((temp = (unsigned char *)ILMalloc(len)) == 0)
	{
		ILUnitOutOfMemory();
	}
	size = 0;

	/* Convert the string into the buffer, one digit at a time */
	if(*str == '-')
	{
		negative = 1;
		++str;
	}
	else
	{
		negative = 0;
	}
	while(*str != '\0')
	{
		/* Skip non-digits */
		if(*str < '0' || *str > '9')
		{
			++str;
			continue;
		}

		/* Multiply "temp" by 10 and add the new digit */
		carry = (ILUInt32)(*str++ - '0');
		posn = 0;
		while(posn < size)
		{
			carry += 10 * (ILUInt32)(temp[len - 1 - posn]);
			temp[len - 1 - posn] = (unsigned char)carry;
			carry >>= 8;
			++posn;
		}
		if(carry != 0)
		{
			temp[len - 1 - posn] = (unsigned char)carry;
			++size;
		}
	}

	/* Convert the buffer into a big number */
	num = ILBigNumFromBytes(temp + len - size, size);
	if(!num)
	{
		ILUnitOutOfMemory();
	}

	/* Negate the value if necessary */
	if(negative)
	{
		zero = ILBigNumFromInt(0);
		if(!zero)
		{
			ILUnitOutOfMemory();
		}
		negnum = ILBigNumSub(zero, num, (ILBigNum *)0);
		if(!negnum)
		{
			ILUnitOutOfMemory();
		}
		ILBigNumFree(num);
		num = negnum;
	}

	/* Clean up and exit */
	ILFree(temp);
	return num;
}

/*
 * Determine if a big number is equal to a specified string's value.
 */
static int BigNumEq(ILBigNum *num, const char *str)
{
	ILBigNum *num2 = StrToBigNum(str);
	int cmp = ILBigNumCompare(num, num2);
	ILBigNumFree(num2);
	return (cmp == 0);
}

/*
 * Test big number creation via "ILBigNumFromInt".
 */
static void bignum_from_int(void *arg)
{
	ILBigNum *num;

	num = ILBigNumFromInt(0);
	if(!num)
	{
		ILUnitOutOfMemory();
	}
	if(!BigNumEq(num, "0"))
	{
		ILUnitFailed("num != 0");
	}
	if(BigNumEq(num, "1"))
	{
		/* Shouldn't happen - checks for bad "ILBigNumCompare" */
		ILUnitFailed("num == 1");
	}
	ILBigNumFree(num);

	num = ILBigNumFromInt(1);
	if(!num)
	{
		ILUnitOutOfMemory();
	}
	if(!BigNumEq(num, "1"))
	{
		ILUnitFailed("num != 1");
	}
	ILBigNumFree(num);

	num = ILBigNumFromInt(2147483647);
	if(!num)
	{
		ILUnitOutOfMemory();
	}
	if(!BigNumEq(num, "2147483647"))
	{
		ILUnitFailed("num != 2147483647");
	}
	ILBigNumFree(num);

	num = ILBigNumFromInt(IL_MAX_UINT32);
	if(!num)
	{
		ILUnitOutOfMemory();
	}
	if(!BigNumEq(num, "4294967295"))
	{
		ILUnitFailed("num != 4294967295");
	}
	ILBigNumFree(num);
}

/*
 * Useful byte arrays for big number testing.
 */
static unsigned char num1bytes[] = {0x00};
static unsigned char num2bytes[] = {0x00, 0x00, 0x00, 0x00, 0x10, 0x00};
static unsigned char num2bytes_trimmed[] = {0x10, 0x00};
static unsigned char num3bytes[] = {0x7F, 0xFF, 0xFF, 0xFF};
static unsigned char num4bytes[] = {0x03, 0x7F, 0xFF, 0xFF, 0xFF};

/*
 * Test big number creation via "ILBigNumFromBytes".
 */
static void bignum_from_bytes(void *arg)
{
	ILBigNum *num;

	num = ILBigNumFromBytes(num1bytes, sizeof(num1bytes));
	if(!num)
	{
		ILUnitOutOfMemory();
	}
	if(!BigNumEq(num, "0"))
	{
		ILUnitFailed("num != 0");
	}
	ILBigNumFree(num);

	num = ILBigNumFromBytes(num2bytes, sizeof(num2bytes));
	if(!num)
	{
		ILUnitOutOfMemory();
	}
	if(!BigNumEq(num, "4096"))
	{
		ILUnitFailed("num != 4096");
	}
	ILBigNumFree(num);

	num = ILBigNumFromBytes(num3bytes, sizeof(num3bytes));
	if(!num)
	{
		ILUnitOutOfMemory();
	}
	if(!BigNumEq(num, "2147483647"))
	{
		ILUnitFailed("num != 2147483647");
	}
	ILBigNumFree(num);

	num = ILBigNumFromBytes(num4bytes, sizeof(num4bytes));
	if(!num)
	{
		ILUnitOutOfMemory();
	}
	if(!BigNumEq(num, "15032385535"))
	{
		ILUnitFailed("num != 15032385535");
	}
	ILBigNumFree(num);
}

/*
 * Test big number output via "ILBigNumToBytes".
 */
static void bignum_to_bytes(void *arg)
{
	ILBigNum *num;
	unsigned char buf[32];

	num = StrToBigNum("0");
	if(ILBigNumByteCount(num) != sizeof(num1bytes))
	{
		ILUnitFailed("num1 byte count incorrect");
	}
	ILBigNumToBytes(num, buf);
	if(ILMemCmp(buf, num1bytes, sizeof(num1bytes)) != 0)
	{
		ILUnitFailed("num1 byte representation incorrect");
	}
	ILBigNumFree(num);

	num = StrToBigNum("4096");
	if(ILBigNumByteCount(num) != sizeof(num2bytes_trimmed))
	{
		ILUnitFailed("num2 byte count incorrect");
	}
	ILBigNumToBytes(num, buf);
	if(ILMemCmp(buf, num2bytes_trimmed, sizeof(num2bytes_trimmed)) != 0)
	{
		ILUnitFailed("num2 byte representation incorrect");
	}
	ILBigNumFree(num);

	num = StrToBigNum("2147483647");
	if(ILBigNumByteCount(num) != sizeof(num3bytes))
	{
		ILUnitFailed("num3 byte count incorrect");
	}
	ILBigNumToBytes(num, buf);
	if(ILMemCmp(buf, num3bytes, sizeof(num3bytes)) != 0)
	{
		ILUnitFailed("num3 byte representation incorrect");
	}
	ILBigNumFree(num);

	num = StrToBigNum("15032385535");
	if(ILBigNumByteCount(num) != sizeof(num4bytes))
	{
		ILUnitFailed("num4 byte count incorrect");
	}
	ILBigNumToBytes(num, buf);
	if(ILMemCmp(buf, num4bytes, sizeof(num4bytes)) != 0)
	{
		ILUnitFailed("num4 byte representation incorrect");
	}
	ILBigNumFree(num);
}

/*
 * Test vectors for "ILBigNumAdd".
 */
static BigNumTestInfo bignum_add_1 = {		/* 2 + 2 = 4 */
	ILBigNumAdd,
	"2", "2", 0, "4"
};
static BigNumTestInfo bignum_add_2 = {		/* size(x) < size(y) */
	ILBigNumAdd,
	"0", "2", 0, "2"
};
static BigNumTestInfo bignum_add_3 = {		/* size(x) > size(y) */
	ILBigNumAdd,
	"2", "0", 0, "2"
};
static BigNumTestInfo bignum_add_4 = {		/* potentional carry from x */
	ILBigNumAdd,
	"4294967295", "0", 0, "4294967295"
};
static BigNumTestInfo bignum_add_5 = {		/* potentional carry from y */
	ILBigNumAdd,
	"0", "4294967295", 0, "4294967295"
};
static BigNumTestInfo bignum_add_6 = {		/* potentional carry from both */
	ILBigNumAdd,
	"2147483647", "2147483648", 0, "4294967295"
};
static BigNumTestInfo bignum_add_7 = {		/* carry to second word */
	ILBigNumAdd,
	"2147483648", "2147483648", 0, "4294967296"
};
static BigNumTestInfo bignum_add_8 = {		/* double carry */
	ILBigNumAdd,
	"18446744073709551615", "1", 0, "18446744073709551616"
};
static BigNumTestInfo bignum_add_9 = {		/* 4 + -3 = 1 */
	ILBigNumAdd,
	"4", "-3", 0, "1"
};
static BigNumTestInfo bignum_add_10 = {		/* 3 + -4 = -1 */
	ILBigNumAdd,
	"3", "-4", 0, "-1"
};
static BigNumTestInfo bignum_add_11 = {		/* -3 + 4 = 1 */
	ILBigNumAdd,
	"-3", "4", 0, "1"
};
static BigNumTestInfo bignum_add_12 = {		/* -3 + -4 = -7 */
	ILBigNumAdd,
	"-3", "-4", 0, "-7"
};

/*
 * Test vectors for "ILBigNumSub".
 */
static BigNumTestInfo bignum_sub_1 = {		/* 4 - 2 = 2 */
	ILBigNumSub,
	"4", "2", 0, "2"
};
static BigNumTestInfo bignum_sub_2 = {		/* 2 - 4 = -2 */
	ILBigNumSub,
	"2", "4", 0, "-2"
};
static BigNumTestInfo bignum_sub_3 = {		/* 2 - -4 = 6 */
	ILBigNumSub,
	"2", "-4", 0, "6"
};
static BigNumTestInfo bignum_sub_4 = {		/* -4 - 2 = -6 */
	ILBigNumSub,
	"-4", "2", 0, "-6"
};
static BigNumTestInfo bignum_sub_5 = {		/* -4 - -2 = -2 */
	ILBigNumSub,
	"-4", "-2", 0, "-2"
};
static BigNumTestInfo bignum_sub_6 = {		/* -2 - -4 = 2 */
	ILBigNumSub,
	"-2", "-4", 0, "2"
};
static BigNumTestInfo bignum_sub_7 = {		/* 0 - 0 = 0 */
	ILBigNumSub,
	"0", "0", 0, "0"
};
static BigNumTestInfo bignum_sub_8 = {		/* 0 - 10 = -10 */
	ILBigNumSub,
	"0", "10", 0, "-10"
};
static BigNumTestInfo bignum_sub_9 = {		/* borrow into second word */
	ILBigNumSub,
	"4294967296", "1", 0, "4294967295"
};
static BigNumTestInfo bignum_sub_10 = {		/* double borrow */
	ILBigNumSub,
	"18446744073709551616", "1", 0, "18446744073709551615"
};

/*
 * Test vectors for "ILBigNumMul".
 */
static BigNumTestInfo bignum_mul_1 = {		/* 2 * 3 = 6 */
	ILBigNumMul,
	"2", "3", 0, "6"
};
static BigNumTestInfo bignum_mul_2 = {		/* -2 * 3 = -6 */
	ILBigNumMul,
	"-2", "3", 0, "-6"
};
static BigNumTestInfo bignum_mul_3 = {		/* 2 * -3 = -6 */
	ILBigNumMul,
	"2", "-3", 0, "-6"
};
static BigNumTestInfo bignum_mul_4 = {		/* -2 * -3 = 6 */
	ILBigNumMul,
	"-2", "-3", 0, "6"
};
static BigNumTestInfo bignum_mul_5 = {		/* 0 * 0 = 0 */
	ILBigNumMul,
	"0", "0", 0, "0"
};
static BigNumTestInfo bignum_mul_6 = {		/* carry into second word */
	ILBigNumMul,
	"2147483648", "2147483648", 0, "4611686018427387904"
};
static BigNumTestInfo bignum_mul_7 = {		/* multiply double-word values */
	ILBigNumMul,
	"8589934595", "8589934599", 0, "73786976380737552405"
};

/*
 * Test vectors for "ILBigNumMod".
 */
static BigNumTestInfo bignum_mod_1 = {		/* 6 % 5 = 1 */
	(BigNumFunc)ILBigNumMod,
	"6", 0, "5", "1"
};
static BigNumTestInfo bignum_mod_2 = {		/* 6 % 0 = divzero */
	(BigNumFunc)ILBigNumMod,
	"6", 0, "0", 0
};
static BigNumTestInfo bignum_mod_3 = {		/* 0 % 6 = 0 */
	(BigNumFunc)ILBigNumMod,
	"0", 0, "6", "0"
};
static BigNumTestInfo bignum_mod_4 = {		/* shortcut single-word modulus */
	(BigNumFunc)ILBigNumMod,
	"68719475971", 0, "65432", "41427"
};
static BigNumTestInfo bignum_mod_5 = {		/* x < modulus */
	(BigNumFunc)ILBigNumMod,
	"68719475971", 0, "68719475972", "68719475971"
};
static BigNumTestInfo bignum_mod_6 = {		/* x > two-word modulus */
	(BigNumFunc)ILBigNumMod,
	"68719475972", 0, "68719475971", "1"
};
static BigNumTestInfo bignum_mod_7 = {		/* top words not the same */
	(BigNumFunc)ILBigNumMod,
	"168719475972", 0, "68719475971", "31280524030"
};
static BigNumTestInfo bignum_mod_8 = {
	(BigNumFunc)ILBigNumMod,
	"1237940039285380277784805376", 0, "9223372041082634240",
			"8655918490919632896"
};
static BigNumTestInfo bignum_mod_9 = {
	(BigNumFunc)ILBigNumMod,
	"21044980677363067087499558912", 0, "9223372041082634240",
			"9088264053469478912"
};

/*
 * Test vectors for "ILBigNumInv".
 */
static BigNumTestInfo bignum_inv_1 = {
	(BigNumFunc)ILBigNumInv,
	"123464321", 0, "87654799657", "77297000318"
};
static BigNumTestInfo bignum_inv_2 = {
	(BigNumFunc)ILBigNumInv,
	"8845687365823749572489562465786", 0,
		"786762344653457615451453456235462534536457623547",
		"610921967315598200582789257447220428606668978817"
};

/*
 * Test vectors for "ILBigNumPow".
 */
static BigNumTestInfo bignum_pow_1 = {
	ILBigNumPow,
	"34", "23", 0, "167500108222301408246337399112597504"
};
static BigNumTestInfo bignum_pow_2 = {
	ILBigNumPow,
	"34", "23", "67", "48"
};
static BigNumTestInfo bignum_pow_3 = {
	ILBigNumPow,
	"34", "0", 0, "1"
};
static BigNumTestInfo bignum_pow_4 = {
	ILBigNumPow,
	"34", "1", 0, "34"
};

/*
 * Test big number operations.
 */
static void test_bignum_oper(BigNumTestInfo *arg)
{
	ILBigNum *x;
	ILBigNum *y;
	ILBigNum *modulus;
	ILBigNum *result;

	/* Convert the arguments into big numbers */
	x = StrToBigNum(arg->x);
	if(arg->y)
	{
		y = StrToBigNum(arg->y);
	}
	else
	{
		y = 0;
	}
	if(arg->modulus)
	{
		modulus = StrToBigNum(arg->modulus);
	}
	else
	{
		modulus = 0;
	}

	/* Perform the operation */
	if(y)
	{
		result = (*(arg->func))(x, y, modulus);
	}
	else
	{
		result = (*(((BigNumUnaryFunc)(arg->func))))(x, modulus);
	}

	/* Check the result against the expected value */
	ILBigNumFree(x);
	ILBigNumFree(y);
	ILBigNumFree(modulus);
	if(arg->result)
	{
		if(!result)
		{
			ILUnitOutOfMemory();
		}
		if(!BigNumEq(result, arg->result))
		{
			ILUnitFailed("result is incorrect");
		}
		ILBigNumFree(result);
	}
	else
	{
		/* We are expecting "divsion by zero" */
		if(result != 0)
		{
			ILUnitFailed("did not give `division by zero'");
		}
	}
}

/*
 * Test registration macro for cryptographic tests.
 */
#define	RegisterCrypt(func,name)	\
	(ILUnitRegister(#name, (ILUnitTestFunc)func, &name))

/*
 * Simple test registration macro.
 */
#define	RegisterSimple(name)	(ILUnitRegister(#name, name, 0))

/*
 * Register all unit tests.
 */
void ILUnitRegisterTests(void)
{
	/*
	 * Test the properties of the MD5 algorithm.
	 */
	ILUnitRegisterSuite("MD5");
	RegisterCrypt(test_md5_hash, md5_hash_1);
	RegisterCrypt(test_md5_hash, md5_hash_2);
	RegisterCrypt(test_md5_hash, md5_hash_3);
	RegisterCrypt(test_md5_hash, md5_hash_4);
	RegisterCrypt(test_md5_hash, md5_hash_5);
	RegisterCrypt(test_md5_hash, md5_hash_6);
	RegisterCrypt(test_md5_hash, md5_hash_7);

	/*
	 * Test the properties of the SHA-1 algorithm.
	 */
	ILUnitRegisterSuite("SHA-1");
	RegisterCrypt(test_sha1_hash, sha1_hash_1);
	RegisterCrypt(test_sha1_hash, sha1_hash_2);

	/*
	 * Test the properties of the SHA-256 algorithm.
	 */
	ILUnitRegisterSuite("SHA-256");
	RegisterCrypt(test_sha256_hash, sha256_hash_1);
	RegisterCrypt(test_sha256_hash, sha256_hash_2);

	/*
	 * Test the properties of the SHA-384 algorithm.
	 */
	ILUnitRegisterSuite("SHA-384");
	RegisterCrypt(test_sha384_hash, sha384_hash_1);
	RegisterCrypt(test_sha384_hash, sha384_hash_2);

	/*
	 * Test the properties of the SHA-512 algorithm.
	 */
	ILUnitRegisterSuite("SHA-512");
	RegisterCrypt(test_sha512_hash, sha512_hash_1);
	RegisterCrypt(test_sha512_hash, sha512_hash_2);

	/*
	 * Test the properties of the AES algorithm.
	 */
	ILUnitRegisterSuite("AES");
	RegisterCrypt(test_aes_block, aes_block_1);
	RegisterCrypt(test_aes_block, aes_block_2);
	RegisterCrypt(test_aes_block, aes_block_3);

	/*
	 * Test the properties of the DES algorithm.
	 */
	ILUnitRegisterSuite("DES");
	RegisterCrypt(test_des_block, des_block_1);
	RegisterCrypt(test_des_block, des_block_2);
	RegisterCrypt(test_des_block, des_block_3);
	RegisterCrypt(test_des_block, des_block_4);
	RegisterCrypt(test_des_block, des_block_5);

	/*
	 * Test the properties of the Triple-DES algorithm.
	 */
	ILUnitRegisterSuite("Triple-DES");
	RegisterCrypt(test_des3_block, des3_block_1);
	RegisterCrypt(test_des3_block, des3_block_2);

	/*
	 * Test the properties of the RC2 algorithm.
	 */
	ILUnitRegisterSuite("RC2");
	RegisterCrypt(test_rc2_block, rc2_block_1);
	RegisterCrypt(test_rc2_block, rc2_block_2);
	RegisterCrypt(test_rc2_block, rc2_block_3);
	RegisterCrypt(test_rc2_block, rc2_block_4);

	/*
	 * Test the big number arithmetic routines.
	 */
	ILUnitRegisterSuite("BigNum");
	RegisterSimple(bignum_from_int);
	RegisterSimple(bignum_from_bytes);
	RegisterSimple(bignum_to_bytes);

	RegisterCrypt(test_bignum_oper, bignum_add_1);
	RegisterCrypt(test_bignum_oper, bignum_add_2);
	RegisterCrypt(test_bignum_oper, bignum_add_3);
	RegisterCrypt(test_bignum_oper, bignum_add_4);
	RegisterCrypt(test_bignum_oper, bignum_add_5);
	RegisterCrypt(test_bignum_oper, bignum_add_6);
	RegisterCrypt(test_bignum_oper, bignum_add_7);
	RegisterCrypt(test_bignum_oper, bignum_add_8);
	RegisterCrypt(test_bignum_oper, bignum_add_9);
	RegisterCrypt(test_bignum_oper, bignum_add_10);
	RegisterCrypt(test_bignum_oper, bignum_add_11);
	RegisterCrypt(test_bignum_oper, bignum_add_12);

	RegisterCrypt(test_bignum_oper, bignum_sub_1);
	RegisterCrypt(test_bignum_oper, bignum_sub_2);
	RegisterCrypt(test_bignum_oper, bignum_sub_3);
	RegisterCrypt(test_bignum_oper, bignum_sub_4);
	RegisterCrypt(test_bignum_oper, bignum_sub_5);
	RegisterCrypt(test_bignum_oper, bignum_sub_6);
	RegisterCrypt(test_bignum_oper, bignum_sub_7);
	RegisterCrypt(test_bignum_oper, bignum_sub_8);
	RegisterCrypt(test_bignum_oper, bignum_sub_9);
	RegisterCrypt(test_bignum_oper, bignum_sub_10);

	RegisterCrypt(test_bignum_oper, bignum_mul_1);
	RegisterCrypt(test_bignum_oper, bignum_mul_2);
	RegisterCrypt(test_bignum_oper, bignum_mul_3);
	RegisterCrypt(test_bignum_oper, bignum_mul_4);
	RegisterCrypt(test_bignum_oper, bignum_mul_5);
	RegisterCrypt(test_bignum_oper, bignum_mul_6);
	RegisterCrypt(test_bignum_oper, bignum_mul_7);

	RegisterCrypt(test_bignum_oper, bignum_mod_1);
	RegisterCrypt(test_bignum_oper, bignum_mod_2);
	RegisterCrypt(test_bignum_oper, bignum_mod_3);
	RegisterCrypt(test_bignum_oper, bignum_mod_4);
	RegisterCrypt(test_bignum_oper, bignum_mod_5);
	RegisterCrypt(test_bignum_oper, bignum_mod_6);
	RegisterCrypt(test_bignum_oper, bignum_mod_7);
	RegisterCrypt(test_bignum_oper, bignum_mod_8);
	RegisterCrypt(test_bignum_oper, bignum_mod_9);

	RegisterCrypt(test_bignum_oper, bignum_inv_1);
	RegisterCrypt(test_bignum_oper, bignum_inv_2);

	RegisterCrypt(test_bignum_oper, bignum_pow_1);
	RegisterCrypt(test_bignum_oper, bignum_pow_2);
	RegisterCrypt(test_bignum_oper, bignum_pow_3);
	RegisterCrypt(test_bignum_oper, bignum_pow_4);
}

void ILUnitCleanupTests(void)
{
	/*
	 * Nothing to do here.
	 */
}

#ifdef	__cplusplus
};
#endif
