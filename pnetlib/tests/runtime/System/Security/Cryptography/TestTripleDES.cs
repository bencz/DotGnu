/*
 * TestTripleDES.cs - Test the TripleDES algorithm.
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

public class TestTripleDES : CryptoTestCase
{

	// Constructor.
	public TestTripleDES(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// Keys to use to help test TripleDES.
	private static readonly byte[] des3Key1 =
		{0x10, 0x31, 0x6E, 0x02, 0x8C, 0x8F, 0x3B, 0x4A,
		 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
		 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef};
	private static readonly byte[] des3Key2 =
		{0x10, 0x31, 0x6E, 0x02, 0x8C, 0x8F, 0x3B, 0x4A,
		 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef};

	// Sample plaintexts.
	private static readonly byte[] des3Plaintext1 =
		{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
	private static readonly byte[] des3Plaintext2 =
		{0x10, 0x31, 0x6E, 0x02, 0x8C, 0x8F, 0x3B, 0x4A};

	// Extract a sub-key.
	private static byte[] SubKey(byte[] key, int offset, int length)
			{
				byte[] sub = new byte [length];
				Array.Copy(key, offset, sub, 0, length);
				return sub;
			}

	// Check a TripleDES instance against a particular key and plaintext.
	// We do this by comparing against what DES would do, if applied manually.
	// This assumes that DES has already been tested and found to be OK.
	private static void CheckTripleDES(TripleDES alg, byte[] key,
									   byte[] plaintext)
			{
				// Set up the algorithm the way we want.
				alg.Mode = CipherMode.ECB;
				alg.Padding = PaddingMode.None;

				// Create an encryptor and determine the output ciphertext.
				ICryptoTransform encryptor = alg.CreateEncryptor(key, null);
				byte[] ciphertext = new byte [plaintext.Length * 2];
				byte[] tail;
				int len = encryptor.TransformBlock
						(plaintext, 0, plaintext.Length,
						 ciphertext, 0);
				AssertEquals("ECB encrypt length mismatch",
							 len, plaintext.Length);
				tail = encryptor.TransformFinalBlock
						(plaintext, 0, 0);
				AssertNotNull("ECB encrypt tail should be non-null");
				AssertEquals("ECB encrypt tail should be zero length",
							 tail.Length, 0);

				// Create a decryptor and run the test backwards.
				ICryptoTransform decryptor = alg.CreateDecryptor(key, null);
				byte[] original = new byte [plaintext.Length * 2];
				len = decryptor.TransformBlock
						(ciphertext, 0, plaintext.Length, original, 0);
				AssertEquals("ECB decrypt length mismatch",
							 len, plaintext.Length);
				tail = decryptor.TransformFinalBlock
						(ciphertext, 0, 0);
				AssertNotNull("ECB decrypt tail should be non-null");
				AssertEquals("ECB decrypt tail should be zero length",
							 tail.Length, 0);
				if(!IdenticalBlock(plaintext, 0, original, 0,
								   plaintext.Length))
				{
					Fail("did not decrypt to the original plaintext");
				}

				// Now see what DES would say on the same input to make
				// sure that TripleDES is giving the correct behaviour.
				DES des = DES.Create();
				des.Mode = CipherMode.ECB;
				des.Padding = PaddingMode.None;
				ICryptoTransform encrypt1;
				ICryptoTransform decrypt2;
				ICryptoTransform encrypt3;
				if(key.Length == 16)
				{
					encrypt1 = des.CreateEncryptor(SubKey(key, 0, 8), null);
					decrypt2 = des.CreateDecryptor(SubKey(key, 8, 8), null);
					encrypt3 = des.CreateEncryptor(SubKey(key, 0, 8), null);
				}
				else
				{
					encrypt1 = des.CreateEncryptor(SubKey(key, 0, 8), null);
					decrypt2 = des.CreateDecryptor(SubKey(key, 8, 8), null);
					encrypt3 = des.CreateEncryptor(SubKey(key, 16, 8), null);
				}
				byte[] block = new byte [plaintext.Length];
				encrypt1.TransformBlock
						(plaintext, 0, plaintext.Length, block, 0);
				tail = encrypt1.TransformFinalBlock
						(plaintext, 0, 0);
				decrypt2.TransformBlock
						(block, 0, plaintext.Length, block, 0);
				tail = decrypt2.TransformFinalBlock
						(block, 0, 0);
				encrypt3.TransformBlock
						(block, 0, plaintext.Length, block, 0);
				tail = encrypt3.TransformFinalBlock
						(block, 0, 0);
				if(!IdenticalBlock(ciphertext, 0, block, 0, plaintext.Length))
				{
					Fail("TripleDES does not have the correct behaviour");
				}
			}

	// Test the TripleDES implementation.
	public void TestTripleDES1()
			{
				CheckTripleDES(TripleDES.Create(), des3Key1, des3Plaintext1);
				CheckTripleDES(TripleDES.Create(), des3Key1, des3Plaintext2);
				CheckTripleDES(TripleDES.Create(), des3Key2, des3Plaintext1);
				CheckTripleDES(TripleDES.Create(), des3Key2, des3Plaintext2);
			}

	// Test that the algorithm specific class cannot return
	// an object of another algorithm.
	public void TestTripleDESCreateOther()
			{
				try
				{
					TripleDES.Create("RC2");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
				try
				{
					TripleDES.Create("DES");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
			}

	// Test the properties of the default algorithm instance.
	public void TestTripleDESProperties()
			{
				SymmetricPropertyTest(TripleDES.Create(), 192, 64);
			}

	// Check that a key is weak.
	private void CheckWeak(String name, byte[] key)
			{
				if(!TripleDES.IsWeakKey(key))
				{
					Fail(name + ": key wasn't recognized as weak");
				}
				TripleDES alg = TripleDES.Create();
				try
				{
					alg.Key = key;
					Fail(name + ": key was supposed to be detected as weak");
				}
				catch(CryptographicException)
				{
					// Success
				}
			}

	// Check that a key is non-weak.
	private void CheckNonWeak(String name, byte[] key)
			{
				CheckNonWeak(name, key, false);
			}
	private void CheckNonWeak(String name, byte[] key, bool suppressCreate)
			{
				if(TripleDES.IsWeakKey(key))
				{
					Fail(name + "key was recognized as weak");
				}
				if(!suppressCreate)
				{
					TripleDES alg = TripleDES.Create();
					try
					{
						alg.Key = key;
					}
					catch(CryptographicException)
					{
						Fail(name +
							 ": key was not supposed to be detected as weak");
					}
				}
			}

	// Test for weak keys.
	public void TestTripleDESWeak()
			{
				// Test exception behaviour of "TripleDES.IsWeakKey".
				try
				{
					TripleDES.IsWeakKey(null);
					Fail("null");
				}
				catch(CryptographicException)
				{
					// success
				}
				try
				{
					TripleDES.IsWeakKey(new byte [0]);
					Fail("wrong size");
				}
				catch(CryptographicException)
				{
					// success
				}

				// These keys are weak.
				CheckWeak("weak1", new byte[]
					{0x01, 0x01, 0x01, 0x01,  0x01, 0x01, 0x01, 0x01,
					 0x01, 0x01, 0x01, 0x01,  0x01, 0x01, 0x01, 0x01});
				CheckWeak("weak2", new byte[]
					{0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E,
					 0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E,
					 0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E});
				CheckWeak("weak3", new byte[]
					{0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E,
					 0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E,
					 0x01, 0x01, 0x01, 0x01,  0x01, 0x01, 0x01, 0x01});
				CheckWeak("weak4", new byte[]
					{0x01, 0x01, 0x01, 0x01,  0x01, 0x01, 0x01, 0x01,
					 0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E,
					 0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E});

				// Test weak keys whose components differ only in parity,
				// because parity bits aren't used in the key schedules.
				CheckWeak("weak5", new byte[]
					{0x01, 0x00, 0x01, 0x00,  0x01, 0x00, 0x01, 0x00,
					 0x00, 0x01, 0x00, 0x01,  0x00, 0x01, 0x00, 0x01});
				CheckWeak("weak6", new byte[]
					{0x1E, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E,
					 0x1F, 0x1E, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E,
					 0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E});

				// Test normal keys.
				CheckNonWeak("weak7", new byte[]
					{0x01, 0x23, 0x45, 0x67,  0x89, 0xab, 0xcd, 0xef,
					 0xef, 0xcd, 0xab, 0x89,  0x67, 0x45, 0x23, 0x01});
				CheckNonWeak("weak8", new byte[]
					{0x01, 0x23, 0x45, 0x67,  0x89, 0xab, 0xcd, 0xef,
					 0xef, 0xcd, 0xab, 0x89,  0x67, 0x45, 0x23, 0x01,
					 0x11, 0x22, 0x33, 0x44,  0x55, 0x66, 0x77, 0x88});
				CheckNonWeak("weak9", new byte[]
					{0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E,
					 0xef, 0xcd, 0xab, 0x89,  0x67, 0x45, 0x23, 0x01,
					 0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E});

				// Test normal keys made up of weak DES components.
				// The Triple-DES form is not weak.
				CheckNonWeak("weak10", new byte[]
					{0x01, 0x01, 0x01, 0x01,  0x01, 0x01, 0x01, 0x01,
					 0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E});
				CheckNonWeak("weak11", new byte[]
					{0x01, 0x01, 0x01, 0x01,  0x01, 0x01, 0x01, 0x01,
					 0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E,
					 0xE0, 0xE0, 0xE0, 0xE0,  0xF1, 0xF1, 0xF1, 0xF1});
			}

	// Run mode tests.
	public void TestTripleDESECB()
			{
				RunModeTest(TripleDES.Create(), CipherMode.ECB);
			}
	public void TestTripleDESCBC()
			{
				RunModeTest(TripleDES.Create(), CipherMode.CBC);
			}
	public void TestTripleDESOFB()
			{
				RunModeTest(TripleDES.Create(), CipherMode.OFB);
			}
	public void TestTripleDESCFB()
			{
				RunModeTest(TripleDES.Create(), CipherMode.CFB);
			}
	public void TestTripleDESCTS()
			{
				RunModeTest(TripleDES.Create(), CipherMode.CTS);
			}

}; // TestTripleDES

#endif // CONFIG_CRYPTO
