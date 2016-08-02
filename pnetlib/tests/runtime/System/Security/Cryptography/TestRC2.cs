/*
 * TestRC2.cs - Test the RC2 algorithm.
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

public class TestRC2 : CryptoTestCase
{

	// Constructor.
	public TestRC2(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// First test vector.
	private static readonly byte[] rc2Key1 =
		{0x88, 0xbc, 0xa9, 0x0e, 0x90, 0x87, 0x5a, 0x7f,
		 0x0f, 0x79, 0xc3, 0x84, 0x62, 0x7b, 0xaf, 0xb2};
	private static readonly byte[] rc2Plaintext1 =
		{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
	private static readonly byte[] rc2Expected1 =
		{0x22, 0x69, 0x55, 0x2a, 0xb0, 0xf8, 0x5c, 0xa6};

	// Test the default RC2 implementation.
	public void TestRC2Default()
			{
				RunSymmetric("RC2:", rc2Key1, rc2Plaintext1,
							 rc2Expected1);
			}

	// Test the first key vector.
	public void TestRC2Key1()
			{
				RunSymmetric("RC2", rc2Key1, rc2Plaintext1,
							 rc2Expected1);
			}

	// Test that the algorithm specific class cannot return
	// an object of another algorithm.
	public void TestRC2CreateOther()
			{
				try
				{
					RC2.Create("DES");
					Fail();
				}
				catch(InvalidCastException)
				{
					// Success case.
				}
			}

	// Test the properties of the default algorithm instance.
	public void TestRC2Properties()
			{
				SymmetricPropertyTest(RC2.Create(), 128, 64);
			}

	// Run mode tests.
	public void TestRC2ECB()
			{
				RunModeTest(RC2.Create(), CipherMode.ECB);
			}
	public void TestRC2CBC()
			{
				RunModeTest(RC2.Create(), CipherMode.CBC);
			}
	public void TestRC2OFB()
			{
				RunModeTest(RC2.Create(), CipherMode.OFB);
			}
	public void TestRC2CFB()
			{
				RunModeTest(RC2.Create(), CipherMode.CFB);
			}
	public void TestRC2CTS()
			{
				RunModeTest(RC2.Create(), CipherMode.CTS);
			}

}; // TestRC2

#endif // CONFIG_CRYPTO
