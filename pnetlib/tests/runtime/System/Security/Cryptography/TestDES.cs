/*
 * TestDES.cs - Test the DES algorithm.
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

public class TestDES : CryptoTestCase
{

	// Constructor.
	public TestDES(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// First test vector.
	private static readonly byte[] desKey1 =
		{0x10, 0x31, 0x6E, 0x02, 0x8C, 0x8F, 0x3B, 0x4A};
	private static readonly byte[] desPlaintext1 =
		{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
	private static readonly byte[] desExpected1 =
		{0x82, 0xDC, 0xBA, 0xFB, 0xDE, 0xAB, 0x66, 0x02};

	// Second test vector.
	private static readonly byte[] desKey2 =
		{0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01};
	private static readonly byte[] desPlaintext2 =
		{0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
	private static readonly byte[] desExpected2 =
		{0x95, 0xF8, 0xA5, 0xE5, 0xDD, 0x31, 0xD9, 0x00};

	// Third test vector.
	private static readonly byte[] desKey3 =
		{0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01};
	private static readonly byte[] desPlaintext3 =
		{0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
	private static readonly byte[] desExpected3 =
		{0xDD, 0x7F, 0x12, 0x1C, 0xA5, 0x01, 0x56, 0x19};

	// Fourth test vector.
	private static readonly byte[] desKey4 =
		{0x80, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01};
	private static readonly byte[] desPlaintext4 =
		{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
	private static readonly byte[] desExpected4 =
		{0x95, 0xA8, 0xD7, 0x28, 0x13, 0xDA, 0xA9, 0x4D};

	// Fifth test vector.
	private static readonly byte[] desKey5 =
		{0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef};
	private static readonly byte[] desPlaintext5 =
		{0x4e, 0x6f, 0x77, 0x20, 0x69, 0x73, 0x20, 0x74};
	private static readonly byte[] desExpected5 =
		{0x3f, 0xa4, 0x0e, 0x8a, 0x98, 0x4d, 0x48, 0x15};

	// Test the default DES implementation.
	public void TestDESDefault()
			{
				RunSymmetric("DES:", desKey1, desPlaintext1,
							 desExpected1);
			}

	// Test the various key vectors.
	public void TestDESKey1()
			{
				RunSymmetric("DES", desKey1, desPlaintext1,
							 desExpected1);
			}
	public void TestDESKey2()
			{
				RunSymmetric("DES", desKey2, desPlaintext2,
							 desExpected2);
			}
	public void TestDESKey3()
			{
				RunSymmetric("DES", desKey3, desPlaintext3,
							 desExpected3);
			}
	public void TestDESKey4()
			{
				RunSymmetric("DES", desKey4, desPlaintext4,
							 desExpected4);
			}
	public void TestDESKey5()
			{
				RunSymmetric("DES", desKey5, desPlaintext5,
							 desExpected5);
			}

	// Test that the algorithm specific class cannot return
	// an object of another algorithm.
	public void TestDESCreateOther()
			{
				try
				{
					DES.Create("RC2");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
				try
				{
					DES.Create("TripleDES");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
			}

	// Check that a key is weak.
	private void CheckWeak(String name, byte[] key)
			{
				if(!DES.IsWeakKey(key))
				{
					Fail(name + ": key wasn't recognized as weak");
				}
				DES alg = DES.Create();
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
				if(DES.IsWeakKey(key))
				{
					Fail(name + "key was recognized as weak");
				}
				if(!suppressCreate)
				{
					DES alg = DES.Create();
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

	// Check that a key is semi-weak.
	private void CheckSemiWeak(String name, byte[] key)
			{
				if(!DES.IsSemiWeakKey(key))
				{
					Fail(name + ": key wasn't recognized as semi-weak");
				}
				DES alg = DES.Create();
				try
				{
					alg.Key = key;
					Fail(name +
						 ": key was supposed to be detected as semi-weak");
				}
				catch(CryptographicException)
				{
					// Success
				}
			}

	// Check that a key is non-semi-weak.
	private void CheckNonSemiWeak(String name, byte[] key)
			{
				CheckNonSemiWeak(name, key, false);
			}
	private void CheckNonSemiWeak(String name, byte[] key, bool suppressCreate)
			{
				if(DES.IsSemiWeakKey(key))
				{
					Fail(name + "key was recognized as semi-weak");
				}
				if(!suppressCreate)
				{
					DES alg = DES.Create();
					try
					{
						alg.Key = key;
					}
					catch(CryptographicException)
					{
						Fail(name +
						 ": key was not supposed to be detected as semi-weak");
					}
				}
			}

	// Test for weak and semi-weak keys.
	public void TestDESWeak()
			{
				// Test exception behaviour of "DES.IsWeakKey".
				try
				{
					DES.IsWeakKey(null);
					Fail("null");
				}
				catch(CryptographicException)
				{
					// success
				}
				try
				{
					DES.IsWeakKey(new byte [0]);
					Fail("wrong size");
				}
				catch(CryptographicException)
				{
					// success
				}

				// These keys are weak.
				CheckWeak("weak1", new byte[]
					{0x01, 0x01, 0x01, 0x01,  0x01, 0x01, 0x01, 0x01});
				CheckWeak("weak2", new byte[]
					{0x1F, 0x1F, 0x1F, 0x1F,  0x0E, 0x0E, 0x0E, 0x0E});
				CheckWeak("weak3", new byte[]
					{0xE0, 0xE0, 0xE0, 0xE0,  0xF1, 0xF1, 0xF1, 0xF1});
				CheckWeak("weak4", new byte[]
					{0xFE, 0xFE, 0xFE, 0xFE,  0xFE, 0xFE, 0xFE, 0xFE});

				// Test a semi-weak key.
				CheckNonWeak("weak5", new byte[]
					{0x01, 0xFE, 0x01, 0xFE,  0x01, 0xFE, 0x01, 0xFE}, true);

				// Test a normal key.
				CheckNonWeak("weak6", new byte[]
					{0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef});
			}
	public void TestDESSemiWeak()
			{
				// Test exception behaviour of "DES.IsSemiWeakKey".
				try
				{
					DES.IsSemiWeakKey(null);
					Fail("null");
				}
				catch(CryptographicException)
				{
					// success
				}
				try
				{
					DES.IsSemiWeakKey(new byte [0]);
					Fail("wrong size");
				}
				catch(CryptographicException)
				{
					// success
				}

				// These keys are semi-weak.
				CheckSemiWeak("semi1", new byte[]
					{0x01, 0xFE, 0x01, 0xFE,  0x01, 0xFE, 0x01, 0xFE});
				CheckSemiWeak("semi2", new byte[]
					{0xFE, 0x01, 0xFE, 0x01,  0xFE, 0x01, 0xFE, 0x01});
				CheckSemiWeak("semi3", new byte[]
					{0x1F, 0xE0, 0x1F, 0xE0,  0x0E, 0xF1, 0x0E, 0xF1});
				CheckSemiWeak("semi4", new byte[]
					{0xE0, 0x1F, 0xE0, 0x1F,  0xF1, 0x0E, 0xF1, 0x0E});
				CheckSemiWeak("semi5", new byte[]
					{0x01, 0xE0, 0x01, 0xE0,  0x01, 0xF1, 0x01, 0xF1});
				CheckSemiWeak("semi6", new byte[]
					{0xE0, 0x01, 0xE0, 0x01,  0xF1, 0x01, 0xF1, 0x01});
				CheckSemiWeak("semi7", new byte[]
					{0x1F, 0xFE, 0x1F, 0xFE,  0x0E, 0xFE, 0x0E, 0xFE});
				CheckSemiWeak("semi8", new byte[]
					{0xFE, 0x1F, 0xFE, 0x1F,  0xFE, 0x0E, 0xFE, 0x0E});
				CheckSemiWeak("semi9", new byte[]
					{0x01, 0x1F, 0x01, 0x1F,  0x01, 0x0E, 0x01, 0x0E});
				CheckSemiWeak("semi10", new byte[]
					{0x1F, 0x01, 0x1F, 0x01,  0x0E, 0x01, 0x0E, 0x01});
				CheckSemiWeak("semi11", new byte[]
					{0xE0, 0xFE, 0xE0, 0xFE,  0xF1, 0xFE, 0xF1, 0xFE});
				CheckSemiWeak("semi12", new byte[]
					{0xFE, 0xE0, 0xFE, 0xE0,  0xFE, 0xF1, 0xFE, 0xF1});

				// Test a weak key.
				CheckNonSemiWeak("semi13", new byte[]
					{0xE0, 0xE0, 0xE0, 0xE0,  0xF1, 0xF1, 0xF1, 0xF1}, true);

				// Test a normal key.
				CheckNonSemiWeak("semi14", new byte[]
					{0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef});
			}

	// Test the properties of the default algorithm instance.
	public void TestDESProperties()
			{
				SymmetricPropertyTest(DES.Create(), 64, 64);
			}

	// Run mode tests.
	public void TestDESECB()
			{
				RunModeTest(DES.Create(), CipherMode.ECB);
			}
	public void TestDESCBC()
			{
				RunModeTest(DES.Create(), CipherMode.CBC);
			}
	public void TestDESOFB()
			{
				RunModeTest(DES.Create(), CipherMode.OFB);
			}
	public void TestDESCFB()
			{
				RunModeTest(DES.Create(), CipherMode.CFB);
			}
	public void TestDESCTS()
			{
				RunModeTest(DES.Create(), CipherMode.CTS);
			}

}; // TestDES

#endif // CONFIG_CRYPTO
