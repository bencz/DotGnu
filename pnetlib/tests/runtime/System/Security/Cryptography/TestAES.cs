/*
 * TestAES.cs - Test the AES algorithm.
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

public class TestAES : CryptoTestCase
{

	// Constructor.
	public TestAES(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// 128-bit test vectors.
	private static readonly byte[] aes128Key =
		{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
		 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F};
	private static readonly byte[] aes128Plaintext =
		{0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
		 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF};
	private static readonly byte[] aes128Expected =
		{0x69, 0xC4, 0xE0, 0xD8, 0x6A, 0x7B, 0x04, 0x30,
		 0xD8, 0xCD, 0xB7, 0x80, 0x70, 0xB4, 0xC5, 0x5A};

	// 192-bit test vectors.
	private static readonly byte[] aes192Key =
		{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
		 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
		 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17};
	private static readonly byte[] aes192Plaintext =
		{0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
		 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF};
	private static readonly byte[] aes192Expected =
		{0xDD, 0xA9, 0x7C, 0xA4, 0x86, 0x4C, 0xDF, 0xE0,
		 0x6E, 0xAF, 0x70, 0xA0, 0xEC, 0x0D, 0x71, 0x91};

	// 256-bit test vectors.
	private static readonly byte[] aes256Key =
		{0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
		 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
		 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17,
		 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F};
	private static readonly byte[] aes256Plaintext =
		{0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
	 	 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF};
	private static readonly byte[] aes256Expected =
		{0x8E, 0xA2, 0xB7, 0xCA, 0x51, 0x67, 0x45, 0xBF,
		 0xEA, 0xFC, 0x49, 0x90, 0x4B, 0x49, 0x60, 0x89};

	// Test the default AES implementation.
	public void TestAESDefault()
			{
				RunSymmetric("Rijndael:", aes128Key, aes128Plaintext,
							 aes128Expected);
			}

	// Test a 128-bit key.
	public void TestAES128()
			{
				RunSymmetric("Rijndael", aes128Key,
							 aes128Plaintext, aes128Expected);
			}

	// Test a 192-bit key.
	public void TestAES192()
			{
				RunSymmetric("Rijndael", aes192Key,
							 aes192Plaintext, aes192Expected);
			}

	// Test a 256-bit key.
	public void TestAES256()
			{
				RunSymmetric("Rijndael", aes256Key,
							 aes256Plaintext, aes256Expected);
			}

	// Test that the algorithm specific class cannot return
	// an object of another algorithm.
	public void TestAESCreateOther()
			{
				try
				{
					Rijndael.Create("DES");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
			}

	// Test the properties of the default algorithm instance.
	public void TestAESProperties()
			{
				SymmetricPropertyTest(Rijndael.Create(), 128, 128);
			}

	// Run mode tests.
	public void TestAESECB()
			{
				RunModeTest(Rijndael.Create(), CipherMode.ECB);
			}
	public void TestAESCBC()
			{
				RunModeTest(Rijndael.Create(), CipherMode.CBC);
			}
	public void TestAESOFB()
			{
				RunModeTest(Rijndael.Create(), CipherMode.OFB);
			}
	public void TestAESCFB()
			{
				RunModeTest(Rijndael.Create(), CipherMode.CFB);
			}
	public void TestAESCTS()
			{
				RunModeTest(Rijndael.Create(), CipherMode.CTS);
			}

}; // TestAES

#endif // CONFIG_CRYPTO
