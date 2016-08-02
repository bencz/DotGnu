/*
 * TestSystemExceptions.cs - Tests for exception classes in "System".
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
using System.Runtime.Serialization;

public class TestSystemExceptions : TestCase
{
	// Constructor.
	public TestSystemExceptions(String name)
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

	// Test the AppDomainUnloadedException class.
	public void TestAppDomainUnloadedException()
			{
				ExceptionTester.CheckMain(typeof(AppDomainUnloadedException),
										  unchecked((int)0x80131014));
			}

#endif

	// Test the ApplicationException class.
	public void TestApplicationException()
			{
				ExceptionTester.CheckMain(typeof(ApplicationException),
										  unchecked((int)0x80131600));
			}

	// Test the ArgumentException class.
	public void TestArgumentException()
			{
				ArgumentException e;
				ExceptionTester.CheckMain(typeof(ArgumentException),
										  unchecked((int)0x80070057));
				e = new ArgumentException();
				AssertNull("ArgumentException (1)", e.ParamName);
				e = new ArgumentException("msg");
				AssertNull("ArgumentException (2)", e.ParamName);
				e = new ArgumentException("msg", "p");
				AssertEquals("ArgumentException (3)", "p", e.ParamName);
				e = new ArgumentException("msg", "p", e);
				AssertEquals("ArgumentException (4)", "p", e.ParamName);
			}

	// Test the ArgumentNullException class.
	public void TestArgumentNullException()
			{
				ArgumentNullException e;

				e = new ArgumentNullException();
				AssertNull("ArgumentNullException (1)", e.ParamName);
				AssertNotNull("ArgumentNullException (2)", e.Message);
				ExceptionTester.CheckHResult
						("ArgumentNullException (3)", e,
						 unchecked((int)0x80004003));

				e = new ArgumentNullException("p");
				AssertEquals("ArgumentNullException (4)", "p", e.ParamName);
				AssertNotNull("ArgumentNullException (5)", e.Message);
				ExceptionTester.CheckHResult
						("ArgumentNullException (6)", e,
						 unchecked((int)0x80004003));

				e = new ArgumentNullException("p", "msg");
				AssertEquals("ArgumentNullException (7)", "p", e.ParamName);
				AssertEquals("ArgumentNullException (8)", "msg", e.Message);
				ExceptionTester.CheckHResult
						("ArgumentNullException (9)", e,
						 unchecked((int)0x80004003));
			}

	// Test the ArgumentOutOfRangeException class.
	public void TestArgumentOutOfRangeException()
			{
				ArgumentOutOfRangeException e;

				e = new ArgumentOutOfRangeException();
				AssertNull("ArgumentOutOfRangeException (1)", e.ParamName);
				AssertNotNull("ArgumentOutOfRangeException (2)", e.Message);
				AssertNull("ArgumentOutOfRangeException (3)", e.ActualValue);
				ExceptionTester.CheckHResult
						("ArgumentOutOfRangeException (4)", e,
						 unchecked((int)0x80131502));

				e = new ArgumentOutOfRangeException("p");
				AssertEquals("ArgumentOutOfRangeException (5)",
							 "p", e.ParamName);
				AssertNotNull("ArgumentOutOfRangeException (6)", e.Message);
				AssertNull("ArgumentOutOfRangeException (7)", e.ActualValue);
				ExceptionTester.CheckHResult
						("ArgumentOutOfRangeException (8)", e,
						 unchecked((int)0x80131502));

				e = new ArgumentOutOfRangeException("p", "msg");
				AssertEquals("ArgumentOutOfRangeException (9)",
							 "p", e.ParamName);
				AssertEquals("ArgumentOutOfRangeException (10)",
							 "msg", e.Message);
				AssertNull("ArgumentOutOfRangeException (11)", e.ActualValue);
				ExceptionTester.CheckHResult
						("ArgumentOutOfRangeException (12)", e,
						 unchecked((int)0x80131502));

				e = new ArgumentOutOfRangeException("p", 3, "msg");
				AssertEquals("ArgumentOutOfRangeException (13)",
							 "p", e.ParamName);
				Assert("ArgumentOutOfRangeException (14)",
					   e.Message.StartsWith("msg"));
				AssertEquals("ArgumentOutOfRangeException (15)",
							 3, e.ActualValue);
				ExceptionTester.CheckHResult
						("ArgumentOutOfRangeException (16)", e,
						 unchecked((int)0x80131502));
			}

	// Test the ArithmeticException class.
	public void TestArithmeticException()
			{
				ExceptionTester.CheckMain(typeof(ArithmeticException),
										  unchecked((int)0x80070216));
			}

	// Test the ArrayTypeMismatchException class.
	public void TestArrayTypeMismatchException()
			{
				ExceptionTester.CheckMain(typeof(ArrayTypeMismatchException),
										  unchecked((int)0x80131503));
			}

	// Test the BadImageFormatException class.
	public void TestBadImageFormatException()
			{
				BadImageFormatException e;
				ExceptionTester.CheckMain(typeof(BadImageFormatException),
										  unchecked((int)0x8007000b));
				e = new BadImageFormatException();
				AssertNull("BadImageFormatException (1)", e.FileName);
				e = new BadImageFormatException("msg");
				AssertNull("BadImageFormatException (2)", e.FileName);
				e = new BadImageFormatException("msg", "file");
				AssertEquals("BadImageFormatException (3)", "file", e.FileName);
				e = new BadImageFormatException("msg", "file", e);
				AssertEquals("BadImageFormatException (4)", "file", e.FileName);
			}

	// Test the CannotUnloadAppDomainException class.
	public void TestCannotUnloadAppDomainException()
			{
				ExceptionTester.CheckMain
						(typeof(CannotUnloadAppDomainException),
						 unchecked((int)0x80131015));
			}

#if CONFIG_REMOTING

	// Test the ContextMarshalException class.
	public void TestContextMarshalException()
			{
				ExceptionTester.CheckMain(typeof(ContextMarshalException),
										  unchecked((int)0x80131504));
			}

#endif

	// Test the DivideByZeroException class.
	public void TestDivideByZeroException()
			{
				ExceptionTester.CheckMain(typeof(DivideByZeroException),
										  unchecked((int)0x80020012));
			}

	// Test the DllNotFoundException class.
	public void TestDllNotFoundException()
			{
				ExceptionTester.CheckMain(typeof(DllNotFoundException),
										  unchecked((int)0x80131524));
			}

	// Test the DuplicateWaitObjectException class.
	public void TestDuplicateWaitObjectException()
			{
				DuplicateWaitObjectException e;

				e = new DuplicateWaitObjectException();
				AssertNull("DuplicateWaitObjectException (1)", e.ParamName);
				AssertNotNull("DuplicateWaitObjectException (2)", e.Message);
				ExceptionTester.CheckHResult
						("DuplicateWaitObjectException (3)", e,
						 unchecked((int)0x80131529));

				e = new DuplicateWaitObjectException("p");
				AssertEquals("DuplicateWaitObjectException (4)",
							 "p", e.ParamName);
				AssertNotNull("DuplicateWaitObjectException (5)", e.Message);
				ExceptionTester.CheckHResult
						("DuplicateWaitObjectException (6)", e,
						 unchecked((int)0x80131529));

				e = new DuplicateWaitObjectException("p", "msg");
				AssertEquals("DuplicateWaitObjectException (7)",
							 "p", e.ParamName);
				AssertEquals("DuplicateWaitObjectException (8)",
							 "msg", e.Message);
				ExceptionTester.CheckHResult
						("DuplicateWaitObjectException (9)", e,
						 unchecked((int)0x80131529));
			}

	// Test the EntryPointNotFoundException class.
	public void TestEntryPointNotFoundException()
			{
				ExceptionTester.CheckMain(typeof(EntryPointNotFoundException),
										  unchecked((int)0x80131523));
			}

	// Test the Exception class.
	public void TestException()
			{
				ExceptionTester.CheckMain(typeof(Exception),
										  unchecked((int)0x80131500));
			#if !ECMA_COMPAT
				// Test the properties of a non-thrown exception.
				Exception e = new Exception();
				AssertNull("TestException (1)", e.HelpLink);
				e.HelpLink = "foo";
				AssertEquals("TestException (2)", "foo", e.HelpLink);
				e.HelpLink = "bar";
				AssertEquals("TestException (3)", "bar", e.HelpLink);
				AssertNull("TestException (4)", e.Source);
				AssertNull("TestException (5)", e.TargetSite);
				AssertEquals("TestException (6)",
							 String.Empty, e.StackTrace);
				e.Source = "src";
				AssertEquals("TestException (7)", "src", e.Source);

				// Test the properties of a thrown exception.
				Exception e2 = null;
				try
				{
					throw new Exception();
				}
				catch(Exception e3)
				{
					e2 = e3;
				}
				AssertNotNull("TestException (8)", e2);
				AssertNotNull("TestException (9)", e2.Source);
				Assert("TestException (10)",
					   e2.Source.StartsWith("Testruntime, Version="));
				AssertNotNull("TestException (11)", e2.TargetSite);
				AssertEquals("TestException (12)",
							 "Void TestException()", e2.TargetSite.ToString());
				Assert("TestException (13)",
					   e2.StackTrace != String.Empty);
			#endif
			}

	// Test the ExecutionEngineException class.
	public void TestExecutionEngineException()
			{
				ExceptionTester.CheckMain(typeof(ExecutionEngineException),
										  unchecked((int)0x80131506));
			}

	// Test the FieldAccessException class.
	public void TestFieldAccessException()
			{
				ExceptionTester.CheckMain(typeof(FieldAccessException),
										  unchecked((int)0x80131507));
			}

	// Test the FormatException class.
	public void TestFormatException()
			{
				ExceptionTester.CheckMain(typeof(FormatException),
										  unchecked((int)0x80131537));
			}

	// Test the IndexOutOfRangeException class.
	public void TestIndexOutOfRangeException()
			{
				ExceptionTester.CheckMain(typeof(IndexOutOfRangeException),
										  unchecked((int)0x80131508));
			}

	// Test the InvalidCastException class.
	public void TestInvalidCastException()
			{
				ExceptionTester.CheckMain(typeof(InvalidCastException),
										  unchecked((int)0x80004002));
			}

	// Test the InvalidOperationException class.
	public void TestInvalidOperationException()
			{
				ExceptionTester.CheckMain(typeof(InvalidOperationException),
										  unchecked((int)0x80131509));
			}

	// Test the InvalidProgramException class.
	public void TestInvalidProgramException()
			{
				ExceptionTester.CheckMain(typeof(InvalidProgramException),
										  unchecked((int)0x8013153a));
			}

	// Test the MemberAccessException class.
	public void TestMemberAccessException()
			{
				ExceptionTester.CheckMain(typeof(MemberAccessException),
										  unchecked((int)0x8013151a));
			}

	// Test the MethodAccessException class.
	public void TestMethodAccessException()
			{
				ExceptionTester.CheckMain(typeof(MethodAccessException),
										  unchecked((int)0x80131510));
			}

	// Test the MissingFieldException class.
	public void TestMissingFieldException()
			{
				ExceptionTester.CheckMain(typeof(MissingFieldException),
										  unchecked((int)0x80131511));
			#if !ECMA_COMPAT && CONFIG_SERIALIZATION
				MissingFieldException e;
				e = new MissingFieldException("x", "y");
				SerializationInfo info =
					new SerializationInfo(typeof(MissingFieldException),
										  new FormatterConverter());
				StreamingContext context = new StreamingContext();
				e.GetObjectData(info, context);
				AssertEquals("MissingFieldException (1)",
							 "x", info.GetString("MMClassName"));
				AssertEquals("MissingFieldException (2)",
							 "y", info.GetString("MMMemberName"));
			#endif
			}

	// Test the MissingMemberException class.
	public void TestMissingMemberException()
			{
				ExceptionTester.CheckMain(typeof(MissingMemberException),
										  unchecked((int)0x80131512));
			#if !ECMA_COMPAT && CONFIG_SERIALIZATION
				MissingMemberException e;
				e = new MissingMemberException("x", "y");
				SerializationInfo info =
					new SerializationInfo(typeof(MissingMemberException),
										  new FormatterConverter());
				StreamingContext context = new StreamingContext();
				e.GetObjectData(info, context);
				AssertEquals("MissingMemberException (1)",
							 "x", info.GetString("MMClassName"));
				AssertEquals("MissingMemberException (2)",
							 "y", info.GetString("MMMemberName"));
			#endif
			}

	// Test the MissingMethodException class.
	public void TestMissingMethodException()
			{
				ExceptionTester.CheckMain(typeof(MissingMethodException),
										  unchecked((int)0x80131513));
			#if !ECMA_COMPAT && CONFIG_SERIALIZATION
				MissingMethodException e;
				e = new MissingMethodException("x", "y");
				SerializationInfo info =
					new SerializationInfo(typeof(MissingMethodException),
										  new FormatterConverter());
				StreamingContext context = new StreamingContext();
				e.GetObjectData(info, context);
				AssertEquals("MissingMethodException (1)",
							 "x", info.GetString("MMClassName"));
				AssertEquals("MissingMethodException (2)",
							 "y", info.GetString("MMMemberName"));
			#endif
			}

	// Test the MulticastNotSupportedException class.
	public void TestMulticastNotSupportedException()
			{
				ExceptionTester.CheckMain
						(typeof(MulticastNotSupportedException),
						 unchecked((int)0x80131514));
			}

#if CONFIG_EXTENDED_NUMERICS

	// Test the NotFiniteNumberException class.
	public void TestNotFiniteNumberException()
			{
				NotFiniteNumberException e;

				e = new NotFiniteNumberException();
				AssertEquals("NotFiniteNumberException (1)",
							 0.0, e.OffendingNumber);
				AssertNotNull("NotFiniteNumberException (2)", e.Message);
				ExceptionTester.CheckHResult
						("NotFiniteNumberException (3)", e,
						 unchecked((int)0x80131528));

				e = new NotFiniteNumberException("msg");
				AssertEquals("NotFiniteNumberException (4)",
							 0.0, e.OffendingNumber);
				AssertEquals("NotFiniteNumberException (5)", "msg", e.Message);
				ExceptionTester.CheckHResult
						("NotFiniteNumberException (6)", e,
						 unchecked((int)0x80131528));

				e = new NotFiniteNumberException("msg", 2.0);
				AssertEquals("NotFiniteNumberException (7)",
							 2.0, e.OffendingNumber);
				AssertEquals("NotFiniteNumberException (8)",
							 "msg", e.Message);
				ExceptionTester.CheckHResult
						("NotFiniteNumberException (9)", e,
						 unchecked((int)0x80131528));
			}

#endif

	// Test the NotImplementedException class.
	public void TestNotImplementedException()
			{
				ExceptionTester.CheckMain(typeof(NotImplementedException),
										  unchecked((int)0x80004001));
			}

	// Test the NotSupportedException class.
	public void TestNotSupportedException()
			{
				ExceptionTester.CheckMain(typeof(NotSupportedException),
										  unchecked((int)0x80131515));
			}

	// Test the NullReferenceException class.
	public void TestNullReferenceException()
			{
				ExceptionTester.CheckMain(typeof(NullReferenceException),
										  unchecked((int)0x80004003));
			}

	// Test the ObjectDisposedException class.
	public void TestObjectDisposedException()
			{
				ObjectDisposedException e;

				e = new ObjectDisposedException("obj");
				AssertEquals("ObjectDisposedException (1)",
							 "obj", e.ObjectName);
				AssertNotNull("ObjectDisposedException (2)", e.Message);
				ExceptionTester.CheckHResult
						("ObjectDisposedException (3)", e,
						 unchecked((int)0x80131509));

				e = new ObjectDisposedException("obj", "msg");
				AssertEquals("ObjectDisposedException (4)",
							 "obj", e.ObjectName);
				AssertEquals("ObjectDisposedException (5)",
							 "obj: msg", e.Message);
				ExceptionTester.CheckHResult
						("ObjectDisposedException (6)", e,
						 unchecked((int)0x80131509));

				e = new ObjectDisposedException(null, "msg");
				AssertNull("ObjectDisposedException (7)", e.ObjectName);
				AssertEquals("ObjectDisposedException (8)",
							 "msg", e.Message);
				ExceptionTester.CheckHResult
						("ObjectDisposedException (9)", e,
						 unchecked((int)0x80131509));
			}

	// Test the OutOfMemoryException class.
	public void TestOutOfMemoryException()
			{
				ExceptionTester.CheckMain(typeof(OutOfMemoryException),
										  unchecked((int)0x8007000e));
			}

	// Test the OverflowException class.
	public void TestOverflowException()
			{
				ExceptionTester.CheckMain(typeof(OverflowException),
										  unchecked((int)0x80131516));
			}

#if !ECMA_COMPAT

	// Test the PlatformNotSupportedException class.
	public void TestPlatformNotSupportedException()
			{
				ExceptionTester.CheckMain(typeof(PlatformNotSupportedException),
										  unchecked((int)0x80131539));
			}

#endif

	// Test the RankException class.
	public void TestRankException()
			{
				ExceptionTester.CheckMain(typeof(RankException),
										  unchecked((int)0x80131517));
			}

	// Test the StackOverflowException class.
	public void TestStackOverflowException()
			{
				ExceptionTester.CheckMain(typeof(StackOverflowException),
										  unchecked((int)0x800703e9));
			}

	// Test the SystemException class.
	public void TestSystemException()
			{
				ExceptionTester.CheckMain(typeof(SystemException),
										  unchecked((int)0x80131501));
			}

#if !ECMA_COMPAT

	// Test the TypeInitializationException class.
	public void TestTypeInitializationException()
			{
				TypeInitializationException e;

				e = new TypeInitializationException(null, null);
				AssertEquals("TypeInitializationException (1)",
							 String.Empty, e.TypeName);
				AssertNotNull("TypeInitializationException (2)", e.Message);
				ExceptionTester.CheckHResult
						("TypeInitializationException (3)", e,
						 unchecked((int)0x80131534));

				e = new TypeInitializationException("type", null);
				AssertEquals("TypeInitializationException (4)",
							 "type", e.TypeName);
				AssertNotNull("TypeInitializationException (5)", e.Message);
				ExceptionTester.CheckHResult
						("TypeInitializationException (6)", e,
						 unchecked((int)0x80131534));

				e = new TypeInitializationException("type", e);
				AssertEquals("TypeInitializationException (7)",
							 "type", e.TypeName);
				AssertNotNull("TypeInitializationException (8)", e.Message);
				ExceptionTester.CheckHResult
						("TypeInitializationException (9)", e,
						 unchecked((int)0x80131534));
			}

#endif

	// Test the TypeLoadException class.
	public void TestTypeLoadException()
			{
				ExceptionTester.CheckMain(typeof(TypeLoadException),
										  unchecked((int)0x80131522));
			}

	// Test the TypeUnloadedException class.
	public void TestTypeUnloadedException()
			{
				ExceptionTester.CheckMain(typeof(TypeUnloadedException),
										  unchecked((int)0x80131013));
			}

	// Test the UnauthorizedAccessException class.
	public void TestUnauthorizedAccessException()
			{
				ExceptionTester.CheckMain(typeof(UnauthorizedAccessException),
										  unchecked((int)0x80070005));
			}

}; // class TestSystemExceptions
