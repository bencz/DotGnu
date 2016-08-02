/*
 * TestSingle.cs - Test class for "System.Single" 
 *
 * Copyright (C) 2009  Free Software Foundation Inc.
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

public class TestSingle : TestCase
 {
	// Constructor.
	public TestSingle(String name)	: base(name)
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

#if CONFIG_EXTENDED_NUMERICS

	public void TestSingleIsNaN()
	{
		Assert("!IsNaN(1.0)", !Single.IsNaN(1.0f));
		Assert("IsNaN(Single.NaN)", Single.IsNaN(Single.NaN));
	}

	public void TestSingleIsNegativeInfinity()
	{
		Assert("!IsNegativeInfinity(1.0)", !Single.IsNegativeInfinity(1.0f));
		Assert("!IsNegativeInfinity(Single.NaN)", !Single.IsNegativeInfinity(Single.NaN));
		Assert("!IsNegativeInfinity(Single.PositiveInfinity)", !Single.IsNegativeInfinity(Single.PositiveInfinity));
		Assert("IsNegativeInfinity(Single.NegativeInfinity)", Single.IsNegativeInfinity(Single.NegativeInfinity));
	}

	public void TestSingleIsPositiveInfinity()
	{
		Assert("!IsPositiveInfinity(1.0)", !Single.IsPositiveInfinity(1.0f));
		Assert("!IsPositiveInfinity(Single.NaN)", !Single.IsPositiveInfinity(Single.NaN));
		Assert("!IsPositiveInfinity(Single.NegativeInfinity)", !Single.IsPositiveInfinity(Single.NegativeInfinity));
		Assert("IsPositiveInfinity(Single.PositiveInfinity)", Single.IsPositiveInfinity(Single.PositiveInfinity));
	}

#endif // CONFIG_EXTENDED_NUMERICS
}
