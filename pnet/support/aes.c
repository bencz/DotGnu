/*
 * aes.c - Implementation of the "Advanced Encryption Standard".
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
 * This file implements the AES symmetric encryption algorithm for
 * 128-bit, 192-bit, and 256-bit keys, based on the description that
 * can be found on the NIST Web site at "http://www.nist.gov/aes/".
 * This implementation is designed for correctness, not speed.
 */

#include "il_crypt.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Determine if this compiler supports inline functions.
 */
#ifdef __GNUC__
	#define	IL_INLINE	__inline__
#else
	#define	IL_INLINE
#endif

/*
 * Helper macros for reading and writing big-endian values.
 */
#define	IL_BREAD_INT32(buf)\
			((ILInt32)(_IL_READ_BYTE((buf), 3) | \
					   _IL_READ_BYTE_SHIFT((buf), 2, 8) | \
					   _IL_READ_BYTE_SHIFT((buf), 1, 16) | \
					   _IL_READ_BYTE_SHIFT((buf), 0, 24)))
#define	IL_BWRITE_INT32(buf, value)	\
			do { \
				(buf)[0] = (unsigned char)((value) >> 24); \
				(buf)[1] = (unsigned char)((value) >> 16); \
				(buf)[2] = (unsigned char)((value) >> 8); \
				(buf)[3] = (unsigned char)(value); \
			} while (0)

/*
 * S-box that is used in encryption operations.
 */
static unsigned char const sbox[256] =
	{0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5,	/* 0x00 */
	 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
	 0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0,	/* 0x10 */
	 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
	 0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc,	/* 0x20 */
	 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
	 0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a,	/* 0x30 */
	 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
	 0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0,	/* 0x40 */
	 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
	 0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b,	/* 0x50 */
	 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
	 0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85,	/* 0x60 */
	 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
	 0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5,	/* 0x70 */
	 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
	 0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17,	/* 0x80 */
	 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
	 0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88,	/* 0x90 */
	 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
	 0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c,	/* 0xA0 */
	 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
	 0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9,	/* 0xB0 */
	 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
	 0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6,	/* 0xC0 */
	 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
	 0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e,	/* 0xD0 */
	 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
	 0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94,	/* 0xE0 */
	 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
	 0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68,	/* 0xF0 */
	 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16,
	};

/*
 * Inverse S-box that is used in decryption operations.
 */
static unsigned char const invsbox[256] =
	{0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38,	/* 0x00 */
	 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
	 0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87,	/* 0x10 */
	 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
	 0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d,	/* 0x20 */
	 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
	 0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2,	/* 0x30 */
	 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
	 0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16,	/* 0x40 */
	 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
	 0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda,	/* 0x50 */
	 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
	 0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a,	/* 0x60 */
	 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
	 0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02,	/* 0x70 */
	 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
	 0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea,	/* 0x80 */
	 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
	 0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85,	/* 0x90 */
	 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
	 0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89,	/* 0xA0 */
	 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
	 0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20,	/* 0xB0 */
	 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
	 0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31,	/* 0xC0 */
	 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
	 0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d,	/* 0xD0 */
	 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
	 0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0,	/* 0xE0 */
	 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
	 0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26,	/* 0xF0 */
	 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
	};

/*
 * Perform a finite field multiplication in GF(2^8).
 */
static IL_INLINE ILInt32 mult(ILInt32 a, ILInt32 b)
{
	ILInt32 result = (((a & 1) != 0) ? b : 0);
	ILInt32 temp = b;
	a >>= 1;
	while(a != 0)
	{
		temp <<= 1;
		if(temp >= 0x100)
		{
			temp ^= 0x11B;
		}
		if((a & 1) != 0)
		{
			result ^= temp;
		}
		a >>= 1;
	}
	return result;
}

/*
 * Perform a "MixColumns" operation on a column value.
 */
static IL_INLINE ILInt32 mix(ILInt32 col)
{
	ILInt32 byte0, byte1, byte2, byte3;
	ILInt32 result;

	/* Unpack "col" into separate bytes */
	byte0 = ((col >> 24) & 0xFF);
	byte1 = ((col >> 16) & 0xFF);
	byte2 = ((col >>  8) & 0xFF);
	byte3 = (col & 0xFF);

	/* Calculate the result */
	result  = (mult(2, byte0) ^ mult(3, byte1) ^ byte2 ^ byte3) << 24;
	result |= (byte0 ^ mult(2, byte1) ^ mult(3, byte2) ^ byte3) << 16;
	result |= (byte0 ^ byte1 ^ mult(2, byte2) ^ mult(3, byte3)) << 8;
	result |= (mult(3, byte0) ^ byte1 ^ byte2 ^ mult(2, byte3));

	/* Clear values we don't need any more */
	col = byte0 = byte1 = byte2 = byte3 = 0;

	/* Return the result to the caller */
	return result;
}

/*
 * Perform the inverse of the "MixColumns" operation.
 */
static IL_INLINE ILInt32 unmix(ILInt32 col)
{
	ILInt32 byte0, byte1, byte2, byte3;
	ILInt32 result;

	/* Unpack "col" into separate bytes */
	byte0 = ((col >> 24) & 0xFF);
	byte1 = ((col >> 16) & 0xFF);
	byte2 = ((col >>  8) & 0xFF);
	byte3 = (col & 0xFF);

	/* Calculate the result */
	result  = (mult(14, byte0) ^ mult(11, byte1) ^
			   mult(13, byte2) ^ mult(9, byte3)) << 24;
	result |= (mult(9, byte0) ^ mult(14, byte1) ^
			   mult(11, byte2) ^ mult(13, byte3)) << 16;
	result |= (mult(13, byte0) ^ mult(9, byte1) ^
			   mult(14, byte2) ^ mult(11, byte3)) << 8;
	result |= (mult(11, byte0) ^ mult(13, byte1) ^
			   mult(9, byte2) ^ mult(14, byte3));

	/* Clear values we don't need any more */
	col = byte0 = byte1 = byte2 = byte3 = 0;

	/* Return the result to the caller */
	return result;
}

void ILAESInit(ILAESContext *aes, unsigned char *key, int keyBits)
{
	const unsigned char *s = sbox;
	int i, nk, total, bit;
	ILInt32 temp;

	/* Determine the number of rounds from the length of the key */
	if(keyBits == 128)
	{
		aes->numRounds = 10;
	}
	else if(keyBits == 192)
	{
		aes->numRounds = 12;
	}
	else
	{
		aes->numRounds = 14;
	}

	/* Copy the key into the first (keyLength / 4) words */
	nk = keyBits / 32;
	for(i = 0; i < nk; ++i)
	{
		aes->keySchedule[i] = IL_BREAD_INT32(key + i * 4);
	}

	/* Expand the key to fill the rest of the schedule */
	total = (aes->numRounds + 1) * 4;
	bit = 1;
	for(i = nk; i < total; ++i)
	{
		temp = aes->keySchedule[i - 1];
		if((i % nk) == 0)
		{
			/* Perform "temp = SubWord(RotWord(temp))" in one step */
			temp = (((ILInt32)(s[(temp >> 16) & 0xFF])) << 24) |
				   (((ILInt32)(s[(temp >>  8) & 0xFF])) << 16) |
				   (((ILInt32)(s[temp & 0xFF])) << 8) |
				   (((ILInt32)(s[(temp >> 24) & 0xFF])));

			/* Perform "temp = temp ^ Rcon[i / nk]" */
			temp ^= (((ILInt32)bit) << 24);
			bit <<= 1;
			if(bit >= 0x100)
			{
				bit ^= 0x11B;
			}
		}
		else if(nk == 8 && (i % nk) == 4)
		{
			/* Special case for 256-bit keys: perform "SubWord(temp)" only */
			temp = (((ILInt32)(s[(temp >> 24) & 0xFF])) << 24) |
				   (((ILInt32)(s[(temp >> 16) & 0xFF])) << 16) |
				   (((ILInt32)(s[(temp >>  8) & 0xFF])) << 8) |
				   (((ILInt32)(s[temp & 0xFF])));
		}
		aes->keySchedule[i] = aes->keySchedule[i - nk] ^ temp;
	}

	/* Clear temporary values */
	temp = 0;
}

void ILAESEncrypt(ILAESContext *aes, unsigned char *input,
				  unsigned char *output)
{
	ILInt32 *ks = aes->keySchedule;
	int nr = aes->numRounds;
	const unsigned char *s = sbox;
	ILInt32 col0, col1, col2, col3;
	ILInt32 ncol0, ncol1, ncol2, ncol3;
	int keyIndex, round;

	/* Unpack the input block into the state columns */
	col0 = IL_BREAD_INT32(input);
	col1 = IL_BREAD_INT32(input + 4);
	col2 = IL_BREAD_INT32(input + 8);
	col3 = IL_BREAD_INT32(input + 12);
	
	/* Add the first round key to the state */
	col0 ^= ks[0];
	col1 ^= ks[1];
	col2 ^= ks[2];
	col3 ^= ks[3];

	/* Perform "nr - 1" rounds on the state */
	keyIndex = 4;
	for(round = nr - 1; round > 0; --round)
	{
		/* Perform SubBytes() and ShiftRows() in one step */
		ncol0 = (((ILInt32)(s[(col0 >> 24) & 0xFF])) << 24) |
				(((ILInt32)(s[(col1 >> 16) & 0xFF])) << 16) |
				(((ILInt32)(s[(col2 >>  8) & 0xFF])) << 8) |
				(((ILInt32)(s[col3 & 0xFF])));
		ncol1 = (((ILInt32)(s[(col1 >> 24) & 0xFF])) << 24) |
				(((ILInt32)(s[(col2 >> 16) & 0xFF])) << 16) |
				(((ILInt32)(s[(col3 >>  8) & 0xFF])) << 8) |
				(((ILInt32)(s[col0 & 0xFF])));
		ncol2 = (((ILInt32)(s[(col2 >> 24) & 0xFF])) << 24) |
				(((ILInt32)(s[(col3 >> 16) & 0xFF])) << 16) |
				(((ILInt32)(s[(col0 >>  8) & 0xFF])) << 8) |
				(((ILInt32)(s[col1 & 0xFF])));
		ncol3 = (((ILInt32)(s[(col3 >> 24) & 0xFF])) << 24) |
				(((ILInt32)(s[(col0 >> 16) & 0xFF])) << 16) |
				(((ILInt32)(s[(col1 >>  8) & 0xFF])) << 8) |
				(((ILInt32)(s[col2 & 0xFF])));

		/* Perform MixColumns() */
		col0 = mix(ncol0);
		col1 = mix(ncol1);
		col2 = mix(ncol2);
		col3 = mix(ncol3);

		/* Add the next round key to the state */
		col0 ^= ks[keyIndex++];
		col1 ^= ks[keyIndex++];
		col2 ^= ks[keyIndex++];
		col3 ^= ks[keyIndex++];
	}

	/* Perform the last round, which omits MixColumns() */
	ncol0 = (((ILInt32)(s[(col0 >> 24) & 0xFF])) << 24) |
			(((ILInt32)(s[(col1 >> 16) & 0xFF])) << 16) |
			(((ILInt32)(s[(col2 >>  8) & 0xFF])) << 8) |
			(((ILInt32)(s[col3 & 0xFF])));
	ncol1 = (((ILInt32)(s[(col1 >> 24) & 0xFF])) << 24) |
			(((ILInt32)(s[(col2 >> 16) & 0xFF])) << 16) |
			(((ILInt32)(s[(col3 >>  8) & 0xFF])) << 8) |
			(((ILInt32)(s[col0 & 0xFF])));
	ncol2 = (((ILInt32)(s[(col2 >> 24) & 0xFF])) << 24) |
			(((ILInt32)(s[(col3 >> 16) & 0xFF])) << 16) |
			(((ILInt32)(s[(col0 >>  8) & 0xFF])) << 8) |
			(((ILInt32)(s[col1 & 0xFF])));
	ncol3 = (((ILInt32)(s[(col3 >> 24) & 0xFF])) << 24) |
			(((ILInt32)(s[(col0 >> 16) & 0xFF])) << 16) |
			(((ILInt32)(s[(col1 >>  8) & 0xFF])) << 8) |
			(((ILInt32)(s[col2 & 0xFF])));
	ncol0 ^= ks[keyIndex++];
	ncol1 ^= ks[keyIndex++];
	ncol2 ^= ks[keyIndex++];
	ncol3 ^= ks[keyIndex];

	/* Pack the state columns into the output block */
	IL_BWRITE_INT32(output, ncol0);
	IL_BWRITE_INT32(output + 4, ncol1);
	IL_BWRITE_INT32(output + 8, ncol2);
	IL_BWRITE_INT32(output + 12, ncol3);

	/* Destroy sensitive state variables */
	col0 = col1 = col2 = col3 = 0;
	ncol0 = ncol1 = ncol2 = ncol3 = 0;
}

void ILAESDecrypt(ILAESContext *aes, unsigned char *input,
				  unsigned char *output)
{
	ILInt32 *ks = aes->keySchedule;
	int nr = aes->numRounds;
	const unsigned char *s = invsbox;
	ILInt32 col0, col1, col2, col3;
	ILInt32 ncol0, ncol1, ncol2, ncol3;
	int keyIndex, round;

	/* Unpack the input block into the state columns */
	col0 = IL_BREAD_INT32(input);
	col1 = IL_BREAD_INT32(input + 4);
	col2 = IL_BREAD_INT32(input + 8);
	col3 = IL_BREAD_INT32(input + 12);

	/* Add the last round key to the state */
	keyIndex = nr * 4;
	col0 ^= ks[keyIndex];
	col1 ^= ks[keyIndex + 1];
	col2 ^= ks[keyIndex + 2];
	col3 ^= ks[keyIndex + 3];

	/* Perform "nr - 1" rounds on the state */
	for(round = nr - 1; round > 0; --round)
	{
		/* Perform InvShiftRows() and InvSubBytes() in one step */
		ncol0 = (((ILInt32)(s[(col0 >> 24) & 0xFF])) << 24) |
				(((ILInt32)(s[(col3 >> 16) & 0xFF])) << 16) |
				(((ILInt32)(s[(col2 >>  8) & 0xFF])) << 8) |
				(((ILInt32)(s[col1 & 0xFF])));
		ncol1 = (((ILInt32)(s[(col1 >> 24) & 0xFF])) << 24) |
				(((ILInt32)(s[(col0 >> 16) & 0xFF])) << 16) |
				(((ILInt32)(s[(col3 >>  8) & 0xFF])) << 8) |
				(((ILInt32)(s[col2 & 0xFF])));
		ncol2 = (((ILInt32)(s[(col2 >> 24) & 0xFF])) << 24) |
				(((ILInt32)(s[(col1 >> 16) & 0xFF])) << 16) |
				(((ILInt32)(s[(col0 >>  8) & 0xFF])) << 8) |
				(((ILInt32)(s[col3 & 0xFF])));
		ncol3 = (((ILInt32)(s[(col3 >> 24) & 0xFF])) << 24) |
				(((ILInt32)(s[(col2 >> 16) & 0xFF])) << 16) |
				(((ILInt32)(s[(col1 >>  8) & 0xFF])) << 8) |
				(((ILInt32)(s[col0 & 0xFF])));

		/* Add the previous round key to the state */
		ncol3 ^= ks[--keyIndex];
		ncol2 ^= ks[--keyIndex];
		ncol1 ^= ks[--keyIndex];
		ncol0 ^= ks[--keyIndex];

		/* Perform InvMixColumns() */
		col0 = unmix(ncol0);
		col1 = unmix(ncol1);
		col2 = unmix(ncol2);
		col3 = unmix(ncol3);
	}

	/* Perform the last round, which omits InvMixColumns() */
	ncol0 = (((ILInt32)(s[(col0 >> 24) & 0xFF])) << 24) |
			(((ILInt32)(s[(col3 >> 16) & 0xFF])) << 16) |
			(((ILInt32)(s[(col2 >>  8) & 0xFF])) << 8) |
			(((ILInt32)(s[col1 & 0xFF])));
	ncol1 = (((ILInt32)(s[(col1 >> 24) & 0xFF])) << 24) |
			(((ILInt32)(s[(col0 >> 16) & 0xFF])) << 16) |
			(((ILInt32)(s[(col3 >>  8) & 0xFF])) << 8) |
			(((ILInt32)(s[col2 & 0xFF])));
	ncol2 = (((ILInt32)(s[(col2 >> 24) & 0xFF])) << 24) |
			(((ILInt32)(s[(col1 >> 16) & 0xFF])) << 16) |
			(((ILInt32)(s[(col0 >>  8) & 0xFF])) << 8) |
			(((ILInt32)(s[col3 & 0xFF])));
	ncol3 = (((ILInt32)(s[(col3 >> 24) & 0xFF])) << 24) |
			(((ILInt32)(s[(col2 >> 16) & 0xFF])) << 16) |
			(((ILInt32)(s[(col1 >>  8) & 0xFF])) << 8) |
			(((ILInt32)(s[col0 & 0xFF])));
	ncol3 ^= ks[--keyIndex];
	ncol2 ^= ks[--keyIndex];
	ncol1 ^= ks[--keyIndex];
	ncol0 ^= ks[--keyIndex];

	/* Pack the state columns into the output block */
	IL_BWRITE_INT32(output, ncol0);
	IL_BWRITE_INT32(output + 4, ncol1);
	IL_BWRITE_INT32(output + 8, ncol2);
	IL_BWRITE_INT32(output + 12, ncol3);

	/* Destroy sensitive state variables */
	col0 = col1 = col2 = col3 = 0;
	ncol0 = ncol1 = ncol2 = ncol3 = 0;
}

void ILAESFinalize(ILAESContext *aes)
{
	ILMemZero(aes, sizeof(ILAESContext));
}

#ifdef TEST_AES

#include <stdio.h>

/*
 * Define the test vectors from the NIST specification.
 */
typedef struct
{
	unsigned char key[32];
	int keyBits;
	unsigned char plaintext[16];
	unsigned char expected[16];

} AESTestVector;
static AESTestVector vector1 = {
	{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
	 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F},
	128,
	{0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
	 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF},
	{0x69, 0xC4, 0xE0, 0xD8, 0x6A, 0x7B, 0x04, 0x30,
	 0xD8, 0xCD, 0xB7, 0x80, 0x70, 0xB4, 0xC5, 0x5A}
};
static AESTestVector vector2 = {
	{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
	 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
	 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17},
	192,
	{0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
	 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF},
	{0xDD, 0xA9, 0x7C, 0xA4, 0x86, 0x4C, 0xDF, 0xE0,
	 0x6E, 0xAF, 0x70, 0xA0, 0xEC, 0x0D, 0x71, 0x91}
};
static AESTestVector vector3 = {
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
 * Print a hex buffer.
 */
static void PrintHex(unsigned char *buffer, int numBits)
{
	printf("%02X%02X %02X%02X %02X%02X %02X%02X "
		   "%02X%02X %02X%02X %02X%02X %02X%02X",
		   buffer[0], buffer[1], buffer[2], buffer[3],
		   buffer[4], buffer[5], buffer[6], buffer[7],
		   buffer[8], buffer[9], buffer[10], buffer[11],
		   buffer[12], buffer[13], buffer[14], buffer[15]);
	if(numBits > 128)
	{
		printf(" %02X%02X %02X%02X %02X%02X %02X%02X",
		       buffer[16], buffer[17], buffer[18], buffer[19],
		       buffer[20], buffer[21], buffer[22], buffer[23]);
	}
	if(numBits > 192)
	{
		printf(" %02X%02X %02X%02X %02X%02X %02X%02X",
		       buffer[24], buffer[25], buffer[26], buffer[27],
		       buffer[28], buffer[29], buffer[30], buffer[31]);
	}
	putchar('\n');
}

/*
 * Process a test vector.
 */
static void ProcessVector(AESTestVector *vector)
{
	ILAESContext aes;
	unsigned char ciphertext[16];
	unsigned char reverse[16];

	/* Encrypt and decrypt the plaintext */
	ILAESInit(&aes, vector->key, vector->keyBits);
	ILAESEncrypt(&aes, vector->plaintext, ciphertext);
	ILAESDecrypt(&aes, ciphertext, reverse);
	ILAESFinalize(&aes);

	/* Report the results */
	printf("Key                 = ");
	PrintHex(vector->key, vector->keyBits);
	printf("Plaintext           = ");
	PrintHex(vector->plaintext, 128);
	printf("Expected Ciphertext = ");
	PrintHex(vector->expected, 128);
	printf("Actual Ciphertext   = ");
	PrintHex(ciphertext, 128);
	printf("Reverse Plaintext   = ");
	PrintHex(reverse, 128);
	if(ILMemCmp(vector->expected, ciphertext, 16) != 0)
	{
		printf("*** Test failed: ciphertexts do not match ***\n");
	}
	if(ILMemCmp(vector->plaintext, reverse, 16) != 0)
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
	return 0;
}

#endif /* TEST_AES */

#ifdef	__cplusplus
};
#endif
