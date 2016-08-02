/*
 * TestCompilerServices.cs - Test System.Runtime.CompilerServices classes.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// Note: this class tests everything in "RuntimeHelpers" except
// "InitializeArray".  That is tested indirectly by many other
// tests that make use of statically-defined arrays.  Those tests
// will undoubtedly fail if "InitializeArray" doesn't work.

public class TestCompilerServices : TestCase
{
	// Constructor.
	public TestCompilerServices(String name)
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

#if !ECMA_COMPAT

	// Test the DateTimeConstantAttribute class.
	public void TestDateTimeConstantAttribute()
			{
				DateTimeConstantAttribute attr;

				attr = new DateTimeConstantAttribute(0);
				AssertNotNull("DTC (1)", attr.Value);
				Assert("DTC (2)", (attr.Value is DateTime));
				AssertEquals("DTC (3)", 0, ((DateTime)(attr.Value)).Ticks);

				DateTime now = DateTime.Now;
				attr = new DateTimeConstantAttribute(now.Ticks);
				AssertNotNull("DTC (4)", attr.Value);
				Assert("DTC (5)", (attr.Value is DateTime));
				AssertEquals("DTC (6)", now.Ticks,
							 ((DateTime)(attr.Value)).Ticks);
			}

#endif // !ECMA_COMPAT

#if CONFIG_EXTENDED_NUMERICS

	// Test the DecimalConstantAttribute class.
	public void TestDecimalConstantAttribute()
			{
				DecimalConstantAttribute attr;
				Decimal value;
				int[] bits;

				value = 123.45m;
				bits = Decimal.GetBits(value);
				attr = new DecimalConstantAttribute
					((byte)(bits[3] >> 16),
					 (byte)((bits[3] & unchecked((int)0x80000000)) != 0
					 			? 1 : 0),
					 unchecked((uint)(bits[2])),
					 unchecked((uint)(bits[1])),
					 unchecked((uint)(bits[0])));
				AssertEquals("DC (1)", value, attr.Value);

				value = -7655578123.45m;
				bits = Decimal.GetBits(value);
				attr = new DecimalConstantAttribute
					((byte)(bits[3] >> 16),
					 (byte)((bits[3] & unchecked((int)0x80000000)) != 0
					 			? 1 : 0),
					 unchecked((uint)(bits[2])),
					 unchecked((uint)(bits[1])),
					 unchecked((uint)(bits[0])));
				AssertEquals("DC (2)", value, attr.Value);
			}

#endif // !CONFIG_EXTENDED_NUMERICS

#if CONFIG_COM_INTEROP

	// Test the IDispatchConstantAttribute class.
	public void TestIDispatchConstantAttribute()
			{
				IDispatchConstantAttribute attr;
				DispatchWrapper wrapper;

				attr = new IDispatchConstantAttribute();
				AssertNotNull("DCA (1)", attr.Value);
				Assert("DCA (2)", (attr.Value is DispatchWrapper));
				wrapper = (DispatchWrapper)(attr.Value);
				AssertNull("DCA (3)", wrapper.WrappedObject);
			}

	// Test the IUnknownConstantAttribute class.
	public void TestIUnknownConstantAttribute()
			{
				IUnknownConstantAttribute attr;
				UnknownWrapper wrapper;

				attr = new IUnknownConstantAttribute();
				AssertNotNull("DCA (1)", attr.Value);
				Assert("DCA (2)", (attr.Value is UnknownWrapper));
				wrapper = (UnknownWrapper)(attr.Value);
				AssertNull("DCA (3)", wrapper.WrappedObject);
			}

#endif // CONFIG_COM_INTEROP

	public static int value;

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void RunClassConstructor(RuntimeTypeHandle type)
	{
		RuntimeHelpers.RunClassConstructor(type);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void CallConstructorTestDummy()
	{
		ConstructorTest.Dummy();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void CallConstructorTest2Dummy()
	{
		ConstructorTest2.Dummy();
	}

	// Test the "RunClassConstructor" method in "RuntimeHelpers".
	public void TestHelpersRunClassConstructor()
			{
				// The value should initially be zero.
				AssertEquals("RCC (1)", 0, value);

				// Run the class constructor and re-test the value.
				RunClassConstructor(typeof(ConstructorTest).TypeHandle);
				AssertEquals("RCC (2)", 1, value);

				// Calling a static method shouldn't cause the value
				// to increase again.
				CallConstructorTestDummy();
				AssertEquals("RCC (3)", 1, value);

				// Re-calling the class constructor shouldn't cause
				// the value to increase again.
				RunClassConstructor(typeof(ConstructorTest).TypeHandle);
				AssertEquals("RCC (4)", 1, value);

				// Reset the value.
				value = 0;

				// Call the static method in "ConstructorTest2".
				// This should implicitly call the class constructor.
				CallConstructorTest2Dummy();
				AssertEquals("RCC (5)", 1, value);

				// Call the class constructor manually, which should
				// not result in an increase of the value.
				RunClassConstructor(typeof(ConstructorTest2).TypeHandle);
				AssertEquals("RCC (6)", 1, value);
			}

	// Test "RuntimeHelpers.OffsetToStringData".
	public void TestHelpersOffsetToStringData()
			{
				Assert("OffsetToStringData (1)",
					   (RuntimeHelpers.OffsetToStringData > 0));
			}

#if !ECMA_COMPAT

	// Test "RuntimeHelpers.GetObjectValue".
	public void TestHelpersGetObjectValue()
			{
				// Null's value is itself.
				AssertNull("GetObjectValue (1)",
						   RuntimeHelpers.GetObjectValue(null));

				// Object references map to themselves.
				Object obj = new Object();
				AssertSame("GetObjectValue (2)", obj,
						   RuntimeHelpers.GetObjectValue(obj));

				// Immutable primitive types map to themselves.
				obj = (Object)3;
				AssertSame("GetObjectValue (3)", obj,
						   RuntimeHelpers.GetObjectValue(obj));
				obj = (Object)(AttributeTargets.Assembly);
				AssertSame("GetObjectValue (4)", obj,
						   RuntimeHelpers.GetObjectValue(obj));

				// Other value types are cloned.
				GetObjectValueTest t = new GetObjectValueTest(3);
				obj = (Object)t;
				Object clone = RuntimeHelpers.GetObjectValue(obj);
				AssertNotSame("GetObjectValue (5)", obj, clone);
				AssertEquals("GetObjectValue (6)",
							 ((GetObjectValueTest)obj).value,
							 ((GetObjectValueTest)clone).value);
				obj.GetHashCode();	// side-effect value.
				AssertEquals("GetObjectValue (7)", 4,
							 ((GetObjectValueTest)obj).value);
				AssertEquals("GetObjectValue (8)", 3,
							 ((GetObjectValueTest)clone).value);
			}

	// Test that "RuntimeHelpers.Equals" bypasses virtual overrides
	// on "Object.Equals(Object)" to perform the base class identity
	// check on value types.
	public void TestHelpersEquals()
			{
				EqualsTest eq = new EqualsTest();
				EqualsTest eq2 = new EqualsTest();
				Assert("Equals (1)", !(eq.Equals(eq)));
				Assert("Equals (2)", !(Object.Equals(eq, eq)));
				Assert("Equals (3)", RuntimeHelpers.Equals(eq, eq));
			}

	// Test that "RuntimeHelpers.GetHashCode" bypasses virtual overrides
	// on "Object.GetHashCode()" to perform the base class hash directly.
	public void TestHelpersGetHashCode()
			{
				GetHashCodeTest gh = new GetHashCodeTest();
				AssertEquals("GetHashCode (1)",
							 RuntimeHelpers.GetHashCode(gh),
							 gh.GetHashCode() - 1);

				AssertEquals("GetHashCode (2)", 0,
							 RuntimeHelpers.GetHashCode(null));
			}

#endif // !ECMA_COMPAT

}; // class TestCompilerServices

// Helper class used by "TestHelpersRunClassConstructor".
internal class ConstructorTest
{
	static ConstructorTest()
			{
				++(TestCompilerServices.value);
			}

	public static void Dummy() {}

};

// Another helper class used by "TestHelpersRunClassConstructor".
internal class ConstructorTest2
{
	static ConstructorTest2()
			{
				++(TestCompilerServices.value);
			}

	public static void Dummy() {}

};

// Helper class for "TestHelpersGetObjectValue".
internal struct GetObjectValueTest
{
	public int value;

	public GetObjectValueTest(int value) { this.value = value; }

	public override int GetHashCode()
			{
				++value;
				return value;
			}

};

// Helper class for "TestHelpersEquals".
internal struct EqualsTest
{
	public override bool Equals(Object obj)
			{
				return false;
			}

};

// Helper class for "TestHelpersGetHashCode".
internal class GetHashCodeTest
{
	public override int GetHashCode()
			{
				return base.GetHashCode() + 1;
			}
};
