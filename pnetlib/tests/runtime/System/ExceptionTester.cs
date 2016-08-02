/*
 * ExceptionTester.cs - Test common exception functionality.
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
using System.Reflection;
using System.Runtime.Serialization;

// This isn't actually a test class.  It provides utility methods
// for other test classes.  It inherits from TestCase so that we
// pick up the "AssertXXX" methods.

public class ExceptionTester : TestCase
{
	// Constructor.
	public ExceptionTester(String name)
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

#if CONFIG_SERIALIZATION

	// Extract the HResult from an exception object.  We have to
	// do it this way because HResult is "protected".
	public static int GetHResult(Exception e)
			{
				SerializationInfo info =
					new SerializationInfo(typeof(Exception),
										  new FormatterConverter());
				StreamingContext context = new StreamingContext();
				e.GetObjectData(info, context);
				return info.GetInt32("HResult");
			}

#endif // CONFIG_SERIALIZATION

	// Check the three main constructors that most exception classes have.
	public static void CheckMain(Type exceptionType, int hresult)
			{
				ConstructorInfo ctor;
				Exception e;
				Exception e2;
				Exception e3;

				// Test the zero-argument constructor.
				ctor = exceptionType.GetConstructor(new Type [0]);
				AssertNotNull(ctor);
				e = (Exception)(ctor.Invoke(new Object [0]));
				AssertNotNull("Msg (1)", e.Message);
			#if CONFIG_SERIALIZATION
				if(hresult != 0)
				{
					AssertEquals("HResult (1)", hresult, GetHResult(e));
				}
			#endif
				AssertSame("BaseException (1)", e, e.GetBaseException());
				AssertNull("InnerException (1)", e.InnerException);

				// Test the single-argument constructor with a null message.
				ctor = exceptionType.GetConstructor
					(new Type [] { typeof(String) });
				AssertNotNull(ctor);
				e = (Exception)(ctor.Invoke(new Object [] { null }));
				AssertNotNull("Msg (2)", e.Message);
			#if CONFIG_SERIALIZATION
				if(hresult != 0)
				{
					AssertEquals("HResult (2)", hresult, GetHResult(e));
				}
			#endif
				AssertSame("BaseException (2)", e, e.GetBaseException());
				AssertNull("InnerException (2)", e.InnerException);

				// Test the single-argument constructor with a given message.
				e = (Exception)(ctor.Invoke(new Object [] { "foobar" }));
				AssertEquals("Msg (3)", "foobar", e.Message);
			#if CONFIG_SERIALIZATION
				if(hresult != 0)
				{
					AssertEquals("HResult (3)", hresult, GetHResult(e));
				}
			#endif
				AssertSame("BaseException (3)", e, e.GetBaseException());
				AssertNull("InnerException (3)", e.InnerException);

				// Test the double-argument constructor with null values.
				ctor = exceptionType.GetConstructor
					(new Type [] { typeof(String), typeof(Exception) });
				AssertNotNull(ctor);
				e = (Exception)(ctor.Invoke(new Object [] { null, null }));
				AssertNotNull("Msg (4)", e.Message);
			#if CONFIG_SERIALIZATION
				if(hresult != 0)
				{
					AssertEquals("HResult (4)", hresult, GetHResult(e));
				}
			#endif
				AssertSame("BaseException (4)", e, e.GetBaseException());
				AssertNull("InnerException (4)", e.InnerException);

				// Test the double-argument constructor with a message.
				e = (Exception)(ctor.Invoke(new Object [] { "foobar", null }));
				AssertEquals("Msg (5)", "foobar", e.Message);
			#if CONFIG_SERIALIZATION
				if(hresult != 0)
				{
					AssertEquals("HResult (5)", hresult, GetHResult(e));
				}
			#endif
				AssertSame("BaseException (5)", e, e.GetBaseException());
				AssertNull("InnerException (5)", e.InnerException);

				// Test the double-argument constructor with an inner exception.
				e2 = e;
				e = (Exception)(ctor.Invoke(new Object [] { "foobar", e2 }));
				AssertEquals("Msg (6)", "foobar", e.Message);
			#if CONFIG_SERIALIZATION
				if(hresult != 0)
				{
					AssertEquals("HResult (6)", hresult, GetHResult(e));
				}
			#endif
				AssertSame("BaseException (6)", e2, e.GetBaseException());
				AssertSame("InnerException (6)", e2, e.InnerException);

				// Test two levels of inner exception.
				e3 = e;
				e = (Exception)(ctor.Invoke(new Object [] { "foobar", e3 }));
				AssertEquals("Msg (7)", "foobar", e.Message);
			#if CONFIG_SERIALIZATION
				if(hresult != 0)
				{
					AssertEquals("HResult (7)", hresult, GetHResult(e));
				}
			#endif
				AssertSame("BaseException (7)", e2, e.GetBaseException());
				AssertSame("InnerException (7)", e3, e.InnerException);
			}

	// Check the HResult value on an exception.
	public static void CheckHResult(String name, Exception e, int hresult)
			{
			#if CONFIG_SERIALIZATION
				AssertEquals(name, hresult, GetHResult(e));
			#endif
			}

}; // class ExceptionTester
