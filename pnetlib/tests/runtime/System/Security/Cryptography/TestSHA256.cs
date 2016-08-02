/*
 * TestSHA256.cs - Test the SHA256 algorithm.
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

public class TestSHA256 : CryptoTestCase
{

	// Constructor.
	public TestSHA256(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// First test vector.
	private static readonly String shaValue1 =
		"abc";
	private static readonly byte[] shaExpected1 =
		{0xba, 0x78, 0x16, 0xbf, 0x8f, 0x01, 0xcf, 0xea,
		 0x41, 0x41, 0x40, 0xde, 0x5d, 0xae, 0x22, 0x23,
		 0xb0, 0x03, 0x61, 0xa3, 0x96, 0x17, 0x7a, 0x9c,
		 0xb4, 0x10, 0xff, 0x61, 0xf2, 0x00, 0x15, 0xad};

	// Second test vector.
	private static readonly String shaValue2 =
		"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq";
	private static readonly byte[] shaExpected2 =
		{0x24, 0x8d, 0x6a, 0x61, 0xd2, 0x06, 0x38, 0xb8,
		 0xe5, 0xc0, 0x26, 0x93, 0x0c, 0x3e, 0x60, 0x39,
		 0xa3, 0x3c, 0xe4, 0x59, 0x64, 0xff, 0x21, 0x67,
		 0xf6, 0xec, 0xed, 0xd4, 0x19, 0xdb, 0x06, 0xc1};

	// Test the default SHA256 implementation.
	public void TestSHA256Default()
			{
				RunHash("SHA256:", shaValue1, shaExpected1);
			}

	// Test the various vectors.
	public void TestSHA256Value1()
			{
				RunHash("SHA256", shaValue1, shaExpected1);
			}
	public void TestSHA256Value2()
			{
				RunHash("SHA256", shaValue2, shaExpected2);
			}

	// Test that the algorithm specific class cannot return
	// an object of another algorithm.
	public void TestSHA256CreateOther()
			{
				try
				{
					SHA256.Create("DES");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
				try
				{
					SHA256.Create("SHA1");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
			}

	// Test the properties of the default algorithm instance.
	public void TestSHA256Properties()
			{
				HashPropertyTest(SHA256.Create(), 256);
			}

}; // TestSHA256

#endif // CONFIG_CRYPTO
