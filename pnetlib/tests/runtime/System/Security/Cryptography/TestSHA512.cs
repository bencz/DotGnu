/*
 * TestSHA512.cs - Test the SHA512 algorithm.
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

using CSUnit;
using System;
using System.Security.Cryptography;

#if CONFIG_CRYPTO

public class TestSHA512 : CryptoTestCase
{

	// Constructor.
	public TestSHA512(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// First test vector.
	private static readonly String shaValue1 =
		"abc";
	private static readonly byte[] shaExpected1 =
		{0xdd, 0xaf, 0x35, 0xa1, 0x93, 0x61, 0x7a, 0xba,
		 0xcc, 0x41, 0x73, 0x49, 0xae, 0x20, 0x41, 0x31,
		 0x12, 0xe6, 0xfa, 0x4e, 0x89, 0xa9, 0x7e, 0xa2,
		 0x0a, 0x9e, 0xee, 0xe6, 0x4b, 0x55, 0xd3, 0x9a,
		 0x21, 0x92, 0x99, 0x2a, 0x27, 0x4f, 0xc1, 0xa8,
		 0x36, 0xba, 0x3c, 0x23, 0xa3, 0xfe, 0xeb, 0xbd,
		 0x45, 0x4d, 0x44, 0x23, 0x64, 0x3c, 0xe8, 0x0e,
		 0x2a, 0x9a, 0xc9, 0x4f, 0xa5, 0x4c, 0xa4, 0x9f};

	// Second test vector.
	private static readonly String shaValue2 =
		"abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmn" +
		"hijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu";
	private static readonly byte[] shaExpected2 =
		{0x8e, 0x95, 0x9b, 0x75, 0xda, 0xe3, 0x13, 0xda,
		 0x8c, 0xf4, 0xf7, 0x28, 0x14, 0xfc, 0x14, 0x3f,
		 0x8f, 0x77, 0x79, 0xc6, 0xeb, 0x9f, 0x7f, 0xa1,
		 0x72, 0x99, 0xae, 0xad, 0xb6, 0x88, 0x90, 0x18,
		 0x50, 0x1d, 0x28, 0x9e, 0x49, 0x00, 0xf7, 0xe4,
		 0x33, 0x1b, 0x99, 0xde, 0xc4, 0xb5, 0x43, 0x3a,
		 0xc7, 0xd3, 0x29, 0xee, 0xb6, 0xdd, 0x26, 0x54,
		 0x5e, 0x96, 0xe5, 0x5b, 0x87, 0x4b, 0xe9, 0x09};

	// Test the default SHA512 implementation.
	public void TestSHA512Default()
			{
				RunHash("SHA512:", shaValue1, shaExpected1);
			}

	// Test the various vectors.
	public void TestSHA512Value1()
			{
				RunHash("SHA512", shaValue1, shaExpected1);
			}
	public void TestSHA512Value2()
			{
				RunHash("SHA512", shaValue2, shaExpected2);
			}

	// Test that the algorithm specific class cannot return
	// an object of another algorithm.
	public void TestSHA512CreateOther()
			{
				try
				{
					SHA512.Create("DES");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
				try
				{
					SHA512.Create("SHA1");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
			}

	// Test the properties of the default algorithm instance.
	public void TestSHA512Properties()
			{
				HashPropertyTest(SHA512.Create(), 512);
			}

}; // TestSHA512

#endif // CONFIG_CRYPTO
