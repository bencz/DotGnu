/*
 * TestMath.cs - Test class for "System.Math" 
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

public class TestMath : TestCase
 {
	// Constructor.
	public TestMath(String name)	: base(name)
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

	public void TestMathCeilingDouble()
	{
		// Checks for non finite values
		Assert("NaN", Double.IsNaN(Math.Ceiling(Double.NaN)));
		Assert("NegativeInfinity", Double.IsNegativeInfinity
			(Math.Ceiling(Double.NegativeInfinity)));
		Assert("PositiveInfinity", Double.IsPositiveInfinity
			(Math.Ceiling(Double.PositiveInfinity)));
		// Checks for finite values
		AssertEquals("1.0", 1.0, Math.Ceiling(1.0));
		AssertEquals("1.1", 2.0, Math.Ceiling(1.1));
		AssertEquals("1.5", 2.0, Math.Ceiling(1.5));
		AssertEquals("1.7", 2.0, Math.Ceiling(1.7));
		AssertEquals("-1.0", -1.0, Math.Ceiling(-1.0));
		AssertEquals("-1.1", -1.0, Math.Ceiling(-1.1));
		AssertEquals("-1.5", -1.0, Math.Ceiling(-1.5));
		AssertEquals("-1.7", -1.0, Math.Ceiling(-1.7));
	}

	public void TestMathFloorDouble()
	{
		// Checks for non finite values
		Assert("NaN", Double.IsNaN(Math.Floor(Double.NaN)));
		Assert("NegativeInfinity", Double.IsNegativeInfinity
			(Math.Floor(Double.NegativeInfinity)));
		Assert("PositiveInfinity", Double.IsPositiveInfinity
			(Math.Floor(Double.PositiveInfinity)));
		// Checks for finite values
		AssertEquals("1.0", 1.0, Math.Floor(1.0));
		AssertEquals("1.1", 1.0, Math.Floor(1.1));
		AssertEquals("1.5", 1.0, Math.Floor(1.5));
		AssertEquals("1.7", 1.0, Math.Floor(1.7));
		AssertEquals("-1.0", -1.0, Math.Floor(-1.0));
		AssertEquals("-1.1", -2.0, Math.Floor(-1.1));
		AssertEquals("-1.5", -2.0, Math.Floor(-1.5));
		AssertEquals("-1.7", -2.0, Math.Floor(-1.7));
	}

	public void TestMathRoundDouble()
	{
		// Checks for non finite values
		Assert("NaN", Double.IsNaN(Math.Round(Double.NaN)));
		Assert("NegativeInfinity", Double.IsNegativeInfinity
			(Math.Round(Double.NegativeInfinity)));
		Assert("PositiveInfinity", Double.IsPositiveInfinity
			(Math.Round(Double.PositiveInfinity)));
		// Checks for finite values
		AssertEquals("1.0", 1.0, Math.Round(1.0));
		AssertEquals("1.1", 1.0, Math.Round(1.1));
		AssertEquals("1.5", 2.0, Math.Round(1.5));
		AssertEquals("1.7", 2.0, Math.Round(1.7));
		AssertEquals("2.5", 2.0, Math.Round(2.5));
		AssertEquals("-1.0", -1.0, Math.Round(-1.0));
		AssertEquals("-1.1", -1.0, Math.Round(-1.1));
		AssertEquals("-1.5", -2.0, Math.Round(-1.5));
		AssertEquals("-1.7", -2.0, Math.Round(-1.7));
		AssertEquals("-2.5", -2.0, Math.Round(-2.5));
	}

	public void TestMathTanDouble()
	{
		AssertEquals("Tan(0.25 * Math.PI)", 1.0, Math.Round(Math.Tan(0.25 * Math.PI) * 100000000) / 100000000);
		AssertEquals("Tan(Double.NaN)", Double.NaN, Math.Tan(Double.NaN));
		AssertEquals("Tan(Double.PositiveInfinity)", Double.NaN, Math.Tan(Double.PositiveInfinity));
		AssertEquals("Tan(Double.NegativeInfinity)", Double.NaN, Math.Tan(Double.NegativeInfinity));
	}

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

	public void TestMathRoundDoubleMidpointRounding()
	{
		// Tests for Rounding to even (results have to be the same
		// as for the simple Round method)
		// Checks for non finite values
		Assert("NaN", Double.IsNaN(Math.Round(Double.NaN,
											  MidpointRounding.ToEven)));
		Assert("NegativeInfinity", Double.IsNegativeInfinity
			(Math.Round(Double.NegativeInfinity, MidpointRounding.ToEven)));
		Assert("PositiveInfinity", Double.IsPositiveInfinity
			(Math.Round(Double.PositiveInfinity, MidpointRounding.ToEven)));
		// Checks for finite values
		AssertEquals("1.0", 1.0, Math.Round(1.0, MidpointRounding.ToEven));
		AssertEquals("1.1", 1.0, Math.Round(1.1, MidpointRounding.ToEven));
		AssertEquals("1.5", 2.0, Math.Round(1.5, MidpointRounding.ToEven));
		AssertEquals("1.7", 2.0, Math.Round(1.7, MidpointRounding.ToEven));
		AssertEquals("2.5", 2.0, Math.Round(2.5, MidpointRounding.ToEven));
		AssertEquals("-1.0", -1.0, Math.Round(-1.0, MidpointRounding.ToEven));
		AssertEquals("-1.1", -1.0, Math.Round(-1.1, MidpointRounding.ToEven));
		AssertEquals("-1.5", -2.0, Math.Round(-1.5, MidpointRounding.ToEven));
		AssertEquals("-1.7", -2.0, Math.Round(-1.7, MidpointRounding.ToEven));
		AssertEquals("-2.5", -2.0, Math.Round(-2.5, MidpointRounding.ToEven));

		// Tests for Rounding away from zero.
		// Checks for non finite values
		Assert("NaN", Double.IsNaN(Math.Round(Double.NaN,
											  MidpointRounding.AwayFromZero)));
		Assert("NegativeInfinity", Double.IsNegativeInfinity
			(Math.Round(Double.NegativeInfinity, MidpointRounding.AwayFromZero)));
		Assert("PositiveInfinity", Double.IsPositiveInfinity
			(Math.Round(Double.PositiveInfinity, MidpointRounding.AwayFromZero)));
		// Checks for finite values
		AssertEquals("1.0", 1.0, Math.Round(1.0, MidpointRounding.AwayFromZero));
		AssertEquals("1.1", 1.0, Math.Round(1.1, MidpointRounding.AwayFromZero));
		AssertEquals("1.5", 2.0, Math.Round(1.5, MidpointRounding.AwayFromZero));
		AssertEquals("1.7", 2.0, Math.Round(1.7, MidpointRounding.AwayFromZero));
		AssertEquals("2.5", 3.0, Math.Round(2.5, MidpointRounding.AwayFromZero));
		AssertEquals("-1.0", -1.0, Math.Round(-1.0, MidpointRounding.AwayFromZero));
		AssertEquals("-1.1", -1.0, Math.Round(-1.1, MidpointRounding.AwayFromZero));
		AssertEquals("-1.5", -2.0, Math.Round(-1.5, MidpointRounding.AwayFromZero));
		AssertEquals("-1.7", -2.0, Math.Round(-1.7, MidpointRounding.AwayFromZero));
		AssertEquals("-2.5", -3.0, Math.Round(-2.5, MidpointRounding.AwayFromZero));
	}

	public void TestMathTruncateDouble()
	{
		// Checks for non finite values
		Assert("NaN", Double.IsNaN(Math.Truncate(Double.NaN)));
		Assert("NegativeInfinity", Double.IsNegativeInfinity
			(Math.Truncate(Double.NegativeInfinity)));
		Assert("PositiveInfinity", Double.IsPositiveInfinity
			(Math.Truncate(Double.PositiveInfinity)));
		// Checks for finite values
		AssertEquals("1.0", 1.0, Math.Truncate(1.0));
		AssertEquals("1.1", 1.0, Math.Truncate(1.1));
		AssertEquals("1.5", 1.0, Math.Truncate(1.5));
		AssertEquals("1.7", 1.0, Math.Truncate(1.7));
		AssertEquals("-1.0", -1.0, Math.Truncate(-1.0));
		AssertEquals("-1.1", -1.0, Math.Truncate(-1.1));
		AssertEquals("-1.5", -1.0, Math.Truncate(-1.5));
		AssertEquals("-1.7", -1.0, Math.Truncate(-1.7));
	}

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

#endif // CONFIG_EXTENDED_NUMERICS
}
