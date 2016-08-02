/*
 * TestConvert.cs - Tests for the "System.Convert" class.
 *
 * Copyright (C) 2004  Free Software Foundation, Inc.
 *
 * Authors : Stephen Compall
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

public class TestConvert : TestCase
{
	// Constructor.
	public TestConvert(String name)
			: base(name)
			{
				// Nothing to do here.
			}

	// Set up for the tests.
	protected override void Setup()
			{
				// Nothing to do here.
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}

	//Methods
	public void TestConvertToByte()
	{
#if !ECMA_COMPAT
		AssertEquals ("f0_16 => 240", 240, Convert.ToByte ("f0", 16));
#endif // !ECMA_COMPAT
	}

	public void TestConvertToSByte()
	{
#if !ECMA_COMPAT
		AssertEquals ("f0_16 => -16", -16, Convert.ToSByte ("f0", 16));
#endif // !ECMA_COMPAT
	}

	public void TestConvertToInt16()
	{
#if !ECMA_COMPAT
		AssertEquals ("8000_16 => -32768",
					  -32768, Convert.ToInt16 ("8000", 16));
#endif // !ECMA_COMPAT
	}

	public void TestConvertToUInt16()
	{
#if !ECMA_COMPAT
		AssertEquals ("8000_16 => 32768",
					  32768, Convert.ToUInt16 ("8000", 16));
#endif // !ECMA_COMPAT
	}

	public void TestConvertToInt32()
	{
#if !ECMA_COMPAT
		AssertEquals ("-0xff => -255",
					  -255, Convert.ToInt32 ("-0xff", 16));
		AssertEquals ("ffff0000_16 => -65536",
					  -65536, Convert.ToInt32 ("ffff0000", 16));
		AssertEquals ("0x80000000_16 => -2147483648",
					  -2147483648, Convert.ToInt32 ("0x80000000", 16));
		AssertEquals ("8000_16 => 32768",
					  32768, Convert.ToInt32 ("8000", 16));
#endif // !ECMA_COMPAT
	}

	public void TestConvertToUInt32()
	{
#if !ECMA_COMPAT
		AssertEquals ("ffff0000_16 => 4294901760",
					  4294901760, Convert.ToUInt32 ("ffff0000", 16));
		AssertEquals ("80000000_16 => 2147483648",
					  2147483648, Convert.ToUInt32 ("80000000", 16));
#endif // !ECMA_COMPAT
	}

	public void TestConvertToInt64()
	{
#if !ECMA_COMPAT
	AssertEquals ("93286ab100ae4211_16 => -7842901442612542959",
				  -7842901442612542959,
				  Convert.ToInt64("93286ab100ae4211", 16));
#endif // !ECMA_COMPAT
	}

	public void TestConvertToUInt64()
	{
#if !ECMA_COMPAT
		AssertEquals ("ffffffffffffffff_16 => 18446744073709551615",
					  18446744073709551615,
					  Convert.ToUInt64 ("ffffffffffffffff", 16));
#endif // !ECMA_COMPAT
	}



}; // class TestConvert
