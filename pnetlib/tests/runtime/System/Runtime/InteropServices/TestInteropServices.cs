/*
 * TestInteropServices.cs - Test various System.Runtime.InteropServices classes.
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

public class TestInteropServices : TestCase
{
	// Constructor.
	public TestInteropServices(String name)
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

	// Test the ExternalException class.
	public void TestExternalException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(ExternalException), unchecked((int)0x80004005));

				// Test the fourth constructor.
				ExternalException e = new ExternalException
						("foobar", 0x0BADBEEF);
				AssertEquals("COM (1)", "foobar", e.Message);
				AssertEquals("COM (2)", 0x0BADBEEF, e.ErrorCode);

				// Test that the error code is zero by default.
				e = new ExternalException("foobar");
				AssertEquals("COM (3)", "foobar", e.Message);
				AssertEquals("COM (4)", 0, e.ErrorCode);
			}

#endif // !ECMA_COMPAT

#if CONFIG_COM_INTEROP

	// Test the COMException class.
	public void TestCOMException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(COMException), unchecked((int)0x80004005));

				// Test the fourth constructor.
				COMException e = new COMException("foobar", 0x0BADBEEF);
				AssertEquals("COM (1)", "foobar", e.Message);
				AssertEquals("COM (2)", 0x0BADBEEF, e.ErrorCode);

				// Test that the error code is zero by default.
				e = new COMException("foobar");
				AssertEquals("COM (3)", "foobar", e.Message);
				AssertEquals("COM (4)", 0, e.ErrorCode);
			}

	// Test the InvalidComObjectException class.
	public void TestInvalidComObjectException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(InvalidComObjectException),
					 unchecked((int)0x80131527));
			}

	// Test the InvalidOleVariantTypeException class.
	public void TestInvalidOleVariantTypeException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(InvalidOleVariantTypeException),
					 unchecked((int)0x80131531));
			}

	// Test the MarshalDirectiveException class.
	public void TestMarshalDirectiveException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(MarshalDirectiveException),
					 unchecked((int)0x80131535));
			}

	// Test the SEHException class.
	public void TestSEHException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(SEHException), unchecked((int)0x80004005));
			}

	// Test the SafeArrayRankMismatchException class.
	public void TestSafeArrayRankMismatchException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(SafeArrayRankMismatchException),
					 unchecked((int)0x80131538));
			}

	// Test the SafeArrayTypeMismatchException class.
	public void TestSafeArrayTypeMismatchException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(SafeArrayTypeMismatchException),
					 unchecked((int)0x80131533));
			}

#if CONFIG_EXTENDED_NUMERICS

	// Test the CurrencyWrapper class.
	public void TestCurrencyWrapper()
			{
				CurrencyWrapper cw;

				// Test the (Decimal) constructor.
				cw = new CurrencyWrapper(123.45m);
				AssertEquals("CurrencyWrapper (1)", 123.45m, cw.WrappedObject);

				// Test the (Object) constructor.
				cw = new CurrencyWrapper((Object)(6123.45m));
				AssertEquals("CurrencyWrapper (2)", 6123.45m, cw.WrappedObject);

				// Test the failure cases of the (Object) constructor.
				try
				{
					cw = new CurrencyWrapper(null);
					Fail("CurrencyWrapper (3)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				try
				{
					cw = new CurrencyWrapper((Object)3);
					Fail("CurrencyWrapper (4)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
			}

#endif // CONFIG_EXTENDED_NUMERICS

	// Test the ErrorWrapper class.
	public void TestErrorWrapper()
			{
				ErrorWrapper ew;

				// Test the (Exception) constructor, which will throw
				// "NotImplementedException" because we don't support COM.
				try
				{
					ew = new ErrorWrapper(new Exception());
					Fail("ErrorWrapper (1)");
				}
				catch(NotImplementedException)
				{
					// Success.
				}

				// Test the (int) constructor.
				ew = new ErrorWrapper(123);
				AssertEquals("ErrorWrapper (2)", 123, ew.ErrorCode);

				// Test the (Object) constructor.
				ew = new ErrorWrapper((Object)(6123));
				AssertEquals("ErrorWrapper (3)", 6123, ew.ErrorCode);

				// Test the failure cases of the (Object) constructor.
				try
				{
					ew = new ErrorWrapper((Object)null);
					Fail("ErrorWrapper (4)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				try
				{
					ew = new ErrorWrapper((Object)"foo");
					Fail("ErrorWrapper (5)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
			}

#endif // CONFIG_COM_INTEROP

}; // class TestInteropServices
