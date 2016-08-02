/*
 * CryptoMethods.cs - Implementation of the "Platform.CryptoMethods" class.
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

namespace Platform
{

#if CONFIG_CRYPTO

using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

internal sealed class CryptoMethods
{
	// Identifiers for the algorithms.
	public const int MD5        = 0;
	public const int SHA1       = 1;
	public const int SHA256     = 2;
	public const int SHA384     = 3;
	public const int SHA512     = 4;
	public const int DES        = 5;
	public const int TripleDES  = 6;
	public const int RC2        = 7;
	public const int Rijndael   = 8;
	public const int DSASign    = 9;
	public const int RSAEncrypt = 10;
	public const int RSASign    = 11;
	public const int RIPEMD160  = 12;

	// Determine if a particular algorithm is supported.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool AlgorithmSupported(int algorithm);

	// Create a new hash algorithm context.  Throws "NotImplementedException"
	// if the algorithm is not supported.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr HashNew(int algorithm);

	// Reset a hash context back to its original state.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void HashReset(IntPtr state);

	// Update a hash context with a block of bytes.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void HashUpdate(IntPtr state, byte[] buffer,
										 int offset, int count);

	// Finalize a hash context and get the hash value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void HashFinal(IntPtr state, byte[] hash);

	// Free a hash context that is no longer required.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void HashFree(IntPtr state);

	// Determine if a DES key value is "semi-weak".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool IsSemiWeakKey(byte[] key, int offset);

	// Determine if a DES key value is "weak".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool IsWeakKey(byte[] key, int offset);

	// Determine if two DES keys are the same.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool SameKey(byte[] key1, int offset1,
									  byte[] key2, int offset2);

	// Generate a number of bytes of random material.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void GenerateRandom
				(byte[] buf, int offset, int count);

	// Create a symmetric block encryption context.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr EncryptCreate(int algorithm, byte[] key);

	// Create a symmetric block decryption context.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr DecryptCreate(int algorithm, byte[] key);

	// Encrypt a single block.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Encrypt(IntPtr state, byte[] inBuffer,
									  int inOffset, byte[] outBuffer,
									  int outOffset);

	// Decrypt a single block.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Decrypt(IntPtr state, byte[] inBuffer,
									  int inOffset, byte[] outBuffer,
									  int outOffset);

	// Free a symmetric block context.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void SymmetricFree(IntPtr state);

	// Perform a big number addition.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static byte[] NumAdd(byte[] x, byte[] y, byte[] modulus);

	// Perform a big number subtraction.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static byte[] NumSub(byte[] x, byte[] y, byte[] modulus);

	// Perform a big number multiplication.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static byte[] NumMul(byte[] x, byte[] y, byte[] modulus);

	// Perform a big number exponentiation.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static byte[] NumPow(byte[] x, byte[] y, byte[] modulus);

	// Perform a big number inverse.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static byte[] NumInv(byte[] x, byte[] modulus);

	// Perform a big number modulus.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static byte[] NumMod(byte[] x, byte[] modulus);

	// Compute two big numbers for equality.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool NumEq(byte[] x, byte[] y);

	// Determine if a big number is zero.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool NumZero(byte[] x);

	// Retrieve a user's named asymmetric key.  The value is formatted
	// in ASN.1, according to the relevant PKCS standard for the key type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static byte[] GetKey(int algorithm, String name,
									   CspProviderFlags flag,
									   out int result);

	// Store a named asymmetric key for the user.  The value is formatted
	// in ASN.1, according to the relevatn PKCS standard for the key type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void StoreKey(int algorithm, String name, byte[] key);

	// Result codes for when "GetUserKey" returns "null".
	public const int UnknownKey   = 1;	// Couldn't find key.
	public const int NotPermitted = 2;	// Not permitted to access.
	public const int GenerateKey  = 3;	// Generate a new key for the user.

}; // class CryptoMethods

#endif // CONFIG_CRYPTO

}; // namespace Platform
