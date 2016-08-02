/*
 * TestMD5.cs - Test the MD5 algorithm.
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

public class TestMD5 : CryptoTestCase
{

	// Constructor.
	public TestMD5(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// First test vector.
	private static readonly String md5Value1 =
		"";
	private static readonly byte[] md5Expected1 =
		{0xD4, 0x1D, 0x8C, 0xD9, 0x8F, 0x00, 0xB2, 0x04,
		 0xE9, 0x80, 0x09, 0x98, 0xEC, 0xF8, 0x42, 0x7E};

	// Second test vector.
	private static readonly String md5Value2 =
		"a";
	private static readonly byte[] md5Expected2 =
		{0x0C, 0xC1, 0x75, 0xB9, 0xC0, 0xF1, 0xB6, 0xA8,
		 0x31, 0xC3, 0x99, 0xE2, 0x69, 0x77, 0x26, 0x61};

	// Third test vector.
	private static readonly String md5Value3 =
		"abc";
	private static readonly byte[] md5Expected3 =
		{0x90, 0x01, 0x50, 0x98, 0x3C, 0xD2, 0x4F, 0xB0,
		 0xD6, 0x96, 0x3F, 0x7D, 0x28, 0xE1, 0x7F, 0x72};

	// Fourth test vector.
	private static readonly String md5Value4 =
		"message digest";
	private static readonly byte[] md5Expected4 =
		{0xF9, 0x6B, 0x69, 0x7D, 0x7C, 0xB7, 0x93, 0x8D,
		 0x52, 0x5A, 0x2F, 0x31, 0xAA, 0xF1, 0x61, 0xD0};

	// Fifth test vector.
	private static readonly String md5Value5 =
		"abcdefghijklmnopqrstuvwxyz";
	private static readonly byte[] md5Expected5 =
		{0xC3, 0xFC, 0xD3, 0xD7, 0x61, 0x92, 0xE4, 0x00,
		 0x7D, 0xFB, 0x49, 0x6C, 0xCA, 0x67, 0xE1, 0x3B};

	// Sixth test vector.
	private static readonly String md5Value6 =
		"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
	private static readonly byte[] md5Expected6 =
		{0xD1, 0x74, 0xAB, 0x98, 0xD2, 0x77, 0xD9, 0xF5,
		 0xA5, 0x61, 0x1C, 0x2C, 0x9F, 0x41, 0x9D, 0x9F};

	// Seventh test vector.
	private static readonly String md5Value7 =
		"123456789012345678901234567890123456789012345678901234567890123456" +
		"78901234567890";
	private static readonly byte[] md5Expected7 =
		{0x57, 0xED, 0xF4, 0xA2, 0x2B, 0xE3, 0xC9, 0x55,
		 0xAC, 0x49, 0xDA, 0x2E, 0x21, 0x07, 0xB6, 0x7A};

	// Test the default MD5 implementation.
	public void TestMD5Default()
			{
				RunHash("MD5:", md5Value1, md5Expected1);
			}

	// Test the various vectors.
	public void TestMD5Value1()
			{
				RunHash("MD5", md5Value1, md5Expected1);
			}
	public void TestMD5Value2()
			{
				RunHash("MD5", md5Value2, md5Expected2);
			}
	public void TestMD5Value3()
			{
				RunHash("MD5", md5Value3, md5Expected3);
			}
	public void TestMD5Value4()
			{
				RunHash("MD5", md5Value4, md5Expected4);
			}
	public void TestMD5Value5()
			{
				RunHash("MD5", md5Value5, md5Expected5);
			}
	public void TestMD5Value6()
			{
				RunHash("MD5", md5Value6, md5Expected6);
			}
	public void TestMD5Value7()
			{
				RunHash("MD5", md5Value7, md5Expected7);
			}

	// Test that the algorithm specific class cannot return
	// an object of another algorithm.
	public void TestMD5CreateOther()
			{
				try
				{
					MD5.Create("DES");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
				try
				{
					MD5.Create("SHA1");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
			}

	// Test the properties of the default algorithm instance.
	public void TestMD5Properties()
			{
				HashPropertyTest(MD5.Create(), 128);
			}

}; // TestMD5

#endif // CONFIG_CRYPTO
