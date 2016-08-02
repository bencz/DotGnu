/*
 * TestMain.cs - Implementation of the "CSUnit.TestMain" class.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

namespace CSUnit
{

using System;
using System.IO;
using System.Text;
using System.Reflection;

public sealed class TestMain
{
	// Main entry point for the program.
	public static int Main(String[] args)
			{
				int numArgs = args.Length;
				int argNum = 0;
				String arg;
				bool stopAtFail = false;
				bool showOnlyFailed = false;
				bool listTests = false;
				String filename;
				String typeName = null;
				Assembly assembly;
				Type type;
				MethodInfo method;
				Test test;
				Test specificTest;
				TestResult result;

				// Parse the command-line options.
				while(argNum < numArgs && args[argNum].StartsWith("-"))
				{
					arg = args[argNum];
					if(arg == "-s" || arg == "--stop-at-fail")
					{
						stopAtFail = true;
					}
					else if(arg == "-f" || arg == "--show-failed")
					{
						showOnlyFailed = true;
					}
					else if(arg == "-l" || arg == "--list")
					{
						listTests = true;
					}
					else if(arg == "-t" || arg == "--type-name")
					{
						if((argNum + 1) >= numArgs)
						{
							ShowUsage();
							return 1;
						}
						++argNum;
						typeName = args[argNum];
					}
					else if(arg.StartsWith("-t"))
					{
						typeName = arg.Substring(2);
					}
					else if(arg == "-v" || arg == "--version")
					{
						ShowVersion();
						return 0;
					}
					else
					{
						ShowUsage();
						return 1;
					}
					++argNum;
				}

				// Get the filename for the test assembly.
				if(argNum >= numArgs)
				{
					ShowUsage();
					return 1;
				}
				filename = args[argNum++];

				// If the type name is not specified, then derive it
				// from the name of the test assembly.
				if(typeName == null)
				{
					int index;
					if(filename.EndsWith(".dll"))
					{
						typeName = filename.Substring(0, filename.Length - 4);
					}
					else
					{
						typeName = filename;
					}
					index = typeName.LastIndexOf('/');
					if(index == -1)
					{
						index = typeName.LastIndexOf('\\');
					}
					if(index != -1)
					{
						typeName = typeName.Substring(index + 1);
					}
				}

				// Load the test assembly.  This will throw an
				// exception if something went wrong, which will
				// cause the program to exit with an explaination.
				// Use "Assembly.LoadFrom" if present (it may not
				// be present if mscorilb.dll is ECMA-compatible,
				// so we have to be careful how we invoke it).
				MethodInfo loadFrom =
					typeof(Assembly).GetMethod
							("LoadFrom", new Type [] {typeof(String)});
				if(loadFrom != null)
				{
					Object[] invokeArgs = new Object [1];
					invokeArgs[0] = filename;
					assembly = (Assembly)(loadFrom.Invoke(null, invokeArgs));
				}
				else
				{
					assembly = Assembly.Load("file://" + filename);
				}

				// Look for the test type within the assembly.
				try
				{
					type = assembly.GetType(typeName);
				}
				catch(TypeLoadException)
				{
					type = null;
				}
				if(type == null)
				{
					ErrorWriteLine
						(typeName + ": type does not exist in " + filename);
					return 1;
				}

				// Call the "type.Suite()" method to construct the
				// top-level test object, which is normally a suite.
				method = type.GetMethod("Suite", BindingFlags.Public |
												 BindingFlags.Static,
									    null, Type.EmptyTypes, null);
				if(method == null)
				{
					// Try again, in case the user prefers lower case names.
					method = type.GetMethod("suite", Type.EmptyTypes);
				}
				if(method == null)
				{
					ErrorWriteLine
						(typeName + ".Suite(): method does not exist in " + filename);
					return 1;
				}
				test = (Test)(method.Invoke(null, null));
				if(test == null)
				{
					ErrorWriteLine
						(typeName + ".Suite(): method returned null");
					return 1;
				}

				// Construct the TestResult class to collect up the results.
				result = new TestWriterResult
					(ConsoleOut, stopAtFail, showOnlyFailed);

				// List or run the tests.
				if(listTests)
				{
					if(argNum < numArgs)
					{
						// List only the specified tests.
						while(argNum < numArgs)
						{
							specificTest = test.Find(args[argNum]);
							if(specificTest == null)
							{
								ErrorWriteLine
									(args[argNum] + ": unknown test name");
							}
							else
							{
								specificTest.List(result);
							}
							++argNum;
						}
					}
					else
					{
						// List all tests.
						test.List(result);
					}
				}
				else if(argNum < numArgs)
				{
					// Run only the specified tests.
					try
					{
						while(argNum < numArgs)
						{
							specificTest = test.Find(args[argNum]);
							if(specificTest == null)
							{
								ErrorWriteLine
									(args[argNum] + ": unknown test name");
							}
							else
							{
								specificTest.Run(result);
							}
							++argNum;
						}
					}
					catch(TestStop)
					{
						// Thrown by "TestWriterResult" to stop
						// testing at the first failure.
					}
					result.ReportSummary();
				}
				else
				{
					// Run all tests.
					try
					{
						test.Run(result);
					}
					catch(TestStop)
					{
						// Thrown by "TestWriterResult" to stop
						// testing at the first failure.
					}
					result.ReportSummary();
				}

				// Done.
				return (result.HadFailures ? 1 : 0);
			}

	// Show the version information for this program.
	private static void ShowVersion()
			{
				Console.WriteLine
					("CSUNIT " + TestVersion.Version +
					 " - C# Unit Testing Framework");
				Console.WriteLine
					("Copyright (c) 2001, 2002 Southern Storm Software, Pty Ltd.");
				Console.WriteLine();
				Console.WriteLine
					("CSUNIT comes with ABSOLUTELY NO WARRANTY.  This is free software,");
				Console.WriteLine
					("and you are welcome to redistribute it under the terms of the");
				Console.WriteLine
					("GNU General Public License.  See the file COPYING for further details.");
				Console.WriteLine();
				Console.WriteLine
					("Use the `--help' option to get help on the command-line options.");
			}

	// Show the usage message for this program.
	private static void ShowUsage()
			{
				Console.WriteLine
					("CSUNIT " + TestVersion.Version +
					 " - C# Unit Testing Framework");
				Console.WriteLine
					("Copyright (c) 2001, 2002 Southern Storm Software, Pty Ltd.");
				Console.WriteLine();
				Console.WriteLine
					("Usage: csunit [options] assembly [test ...]");
				Console.WriteLine();
				Console.WriteLine
					("    --stop-at-fail    or -s");
				Console.WriteLine
					("        Stop at the first failed test.");
				Console.WriteLine
					("    --show-failed     or -f");
				Console.WriteLine
					("        Only show tests that have failed.");
				Console.WriteLine
					("    --list            or -l");
				Console.WriteLine
					("        List all test suites and tests that are registered.");
				Console.WriteLine
					("    --type-name name  or -t name");
				Console.WriteLine
					("        Specify the suite registration type.");
				Console.WriteLine
					("    --version         or -v");
				Console.WriteLine
					("        Print the version of the program.");
				Console.WriteLine
					("    --help");
				Console.WriteLine
					("        Print this help message.");
			}

	// Write a string to standard error.
	private static void ErrorWriteLine(String str)
			{
			#if CONFIG_SMALL_CONSOLE
				Console.WriteLine(str);
			#else
				Console.Error.WriteLine(str);
			#endif
			}

	// Get a TextWriter stream for Console.Out.
	private static TextWriter ConsoleOut
			{
				get
				{
				#if CONFIG_SMALL_CONSOLE
					return new ConsoleWriter();
				#else
					return Console.Out;
				#endif
				}
			}

#if CONFIG_SMALL_CONSOLE

	// Helper class for writing to the console when Console.Out doesn't exist.
	private sealed class ConsoleWriter : TextWriter
	{
		// Constructor.
		public ConsoleWriter() {}

		// Get the encoding in use by this text writer.
		public override System.Text.Encoding Encoding
				{
					get
					{
						return Encoding.Default;
					}
				}

		// Write to this text writer.
		public override void Write(char value)
				{
					Console.Write(value);
				}
		public override void Write(String value)
				{
					Console.Write(value);
				}
		public override void WriteLine()
				{
					Console.WriteLine();
				}

	}; // class ConsoleWriter

#endif

}; // class TestMain

}; // namespace CSUnit
