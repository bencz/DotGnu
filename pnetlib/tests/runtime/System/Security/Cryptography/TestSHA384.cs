/*
 * TestSHA384.cs - Test the SHA384 algorithm.
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

public class TestSHA384 : CryptoTestCase
{

	// Constructor.
	public TestSHA384(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// First test vector.
	private static readonly String shaValue1 =
		"abc";
	private static readonly byte[] shaExpected1 =
		{0xcb, 0x00, 0x75, 0x3f, 0x45, 0xa3, 0x5e, 0x8b,
		 0xb5, 0xa0, 0x3d, 0x69, 0x9a, 0xc6, 0x50, 0x07,
		 0x27, 0x2c, 0x32, 0xab, 0x0e, 0xde, 0xd1, 0x63,
		 0x1a, 0x8b, 0x60, 0x5a, 0x43, 0xff, 0x5b, 0xed,
		 0x80, 0x86, 0x07, 0x2b, 0xa1, 0xe7, 0xcc, 0x23,
		 0x58, 0xba, 0xec, 0xa1, 0x34, 0xc8, 0x25, 0xa7};

	// Second test vector.
	private static readonly String shaValue2 =
		"abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmn" +
		"hijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu";
	private static readonly byte[] shaExpected2 =
		{0x09, 0x33, 0x0c, 0x33, 0xf7, 0x11, 0x47, 0xe8,
		 0x3d, 0x19, 0x2f, 0xc7, 0x82, 0xcd, 0x1b, 0x47,
		 0x53, 0x11, 0x1b, 0x17, 0x3b, 0x3b, 0x05, 0xd2,
		 0x2f, 0xa0, 0x80, 0x86, 0xe3, 0xb0, 0xf7, 0x12,
		 0xfc, 0xc7, 0xc7, 0x1a, 0x55, 0x7e, 0x2d, 0xb9,
		 0x66, 0xc3, 0xe9, 0xfa, 0x91, 0x74, 0x60, 0x39};

	// Test the default SHA384 implementation.
	public void TestSHA384Default()
			{
				RunHash("SHA384:", shaValue1, shaExpected1);
			}

	// Test the various vectors.
	public void TestSHA384Value1()
			{
				RunHash("SHA384", shaValue1, shaExpected1);
			}
	public void TestSHA384Value2()
			{
				RunHash("SHA384", shaValue2, shaExpected2);
			}

	// Test that the algorithm specific class cannot return
	// an object of another algorithm.
	public void TestSHA384CreateOther()
			{
				try
				{
					SHA384.Create("DES");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
				try
				{
					SHA384.Create("SHA1");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
			}

	// Test the properties of the default algorithm instance.
	public void TestSHA384Properties()
			{
				HashPropertyTest(SHA384.Create(), 384);
			}

}; // TestSHA384

#endif // CONFIG_CRYPTO
