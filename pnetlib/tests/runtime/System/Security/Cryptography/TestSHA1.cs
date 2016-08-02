/*
 * TestSHA1.cs - Test the SHA1 algorithm.
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

public class TestSHA1 : CryptoTestCase
{

	// Constructor.
	public TestSHA1(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// First test vector.
	private static readonly String shaValue1 =
		"abc";
	private static readonly byte[] shaExpected1 =
		{0xA9, 0x99, 0x3E, 0x36, 0x47, 0x06, 0x81, 0x6A, 0xBA, 0x3E,
		 0x25, 0x71, 0x78, 0x50, 0xC2, 0x6C, 0x9C, 0xD0, 0xD8, 0x9D};

	// Second test vector.
	private static readonly String shaValue2 =
		"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq";
	private static readonly byte[] shaExpected2 =
		{0x84, 0x98, 0x3E, 0x44, 0x1C, 0x3B, 0xD2, 0x6E, 0xBA, 0xAE,
		 0x4A, 0xA1, 0xF9, 0x51, 0x29, 0xE5, 0xE5, 0x46, 0x70, 0xF1};

	// Test the default SHA1 implementation.
	public void TestSHA1Default()
			{
				RunHash("SHA1:", shaValue1, shaExpected1);
			}

	// Test the various vectors.
	public void TestSHA1Value1()
			{
				RunHash("SHA1", shaValue1, shaExpected1);
			}
	public void TestSHA1Value2()
			{
				RunHash("SHA1", shaValue2, shaExpected2);
			}

	// Test that the algorithm specific class cannot return
	// an object of another algorithm.
	public void TestSHA1CreateOther()
			{
				try
				{
					SHA1.Create("DES");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
				try
				{
					SHA1.Create("MD5");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
			}

	// Test the properties of the default algorithm instance.
	public void TestSHA1Properties()
			{
				HashPropertyTest(SHA1.Create(), 160);
			}

}; // TestSHA1

#endif // CONFIG_CRYPTO
