/*
 * Crt0.cs - Program startup support definitions.
 *
 * This file is part of the Portable.NET "C language support" library.
 * Copyright (C) 2002, 2004  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace OpenSystem.C
{

using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;
using System.Reflection;

//
// The "Crt0" class contains methods that support the "crt0" code
// that the compiler outputs into modules with a "main" function.
// The "crt0" code looks something like this:
//
//		public static void .start(String[] args)
//		{
//			try
//			{
//				int argc;
//				IntPtr argv;
//				IntPtr envp;
//				argv = Crt0.GetArgV(args, out argc);
//				envp = Crt0.GetEnvironment();
//				Crt0.Startup();
//				Crt0.Shutdown(main(argc, argv, envp));
//			}
//			catch(OutOfMemoryException)
//			{
//				throw;
//			}
//			catch(Object e)
//			{
//				throw Crt0.ShutdownWithException(e);
//			}
//		}
//
// If the "main" function does not have all three arguments, the
// compiler will only pass those arguments that "main" specifies.
//
// The "crt0" code is kept deliberately simple, with the bulk of the
// hard work done in this class.  This makes it easier to change the
// startup behaviour by altering "Crt0", without requiring every
// application to be recompiled.
//
// Note: the "crt0" code catches all exceptions, including those
// that don't inherit from "System.Exception".  Out of memory
// exceptions are explicitly excluded because there's nothing that
// can be done in that situation anyway.
//
public unsafe sealed class Crt0
{
	// Internal state.
	private static IntPtr argV;
	private static int argC;
	private static IntPtr environ;
	private static Module libcModule;
	private static MethodInfo finiMethod;
	private static bool startupDone = false;
	private static bool finiDone = false;

	// Get the "argv" and "argc" values for the running task.
	// "mainArgs" is the value passed to the program's entry point.
	public static IntPtr GetArgV(String[] mainArgs, out int argc)
			{
				lock(typeof(Crt0))
				{
					// Bail out if we were already called previously.
					if(argV != IntPtr.Zero)
					{
						argc = argC;
						return argV;
					}

					// Get the actual command-line arguments, including
					// the application name.  The application name isn't
					// normally included in "mainArgs", so we have to
					// do the following instead.
					String[] args;
					try
					{
						args = Environment.GetCommandLineArgs();
					}
					catch(NotSupportedException)
					{
						args = null;
					}
					if(args == null)
					{
						// We couldn't get the arguments from the runtime
						// engine directly, so use "mainArgs" and simulate
						// the application name.  This may happen in embedded
						// environments that don't have a real command-line.
						if(mainArgs != null)
						{
							args = new String [mainArgs.Length + 1];
							Array.Copy(mainArgs, 0, args, 1,
									   mainArgs.Length);
						}
						else
						{
							args = new String [1];
						}
						args[0] = "cliapp.exe";
					}

					// Convert "args" into an array of native strings,
					// terminated by a NULL pointer.
					int ptrSize = (int)(sizeof(void *));
					argV = Marshal.AllocHGlobal
						(new IntPtr(ptrSize * (args.Length + 1)));
					if(argV == IntPtr.Zero)
					{
						// We probably don't have permission to allocate
						// memory using "AllocHGlobal".  If that is the
						// case, then bail out.  C is useless without it.
						throw new NotSupportedException
							("Fatal error: cannot allocate unmanaged memory");
					}
					argC = args.Length;
					for(int index = 0; index < argC; ++index)
					{
						Marshal.WriteIntPtr(argV, ptrSize * index,
							Marshal.StringToHGlobalAnsi(args[index]));
					}
					Marshal.WriteIntPtr(argV, ptrSize * argC, IntPtr.Zero);

					// Return the final values to the caller.
					argc = argC;
					return argV;
				}
			}

	// Get the "environ" value for the running task.
	public static IntPtr GetEnvironment()
			{
				lock(typeof(Crt0))
				{
					// Bail out if we were already called previously.
					if(environ != IntPtr.Zero)
					{
						return environ;
					}

					// Get the environment variables for the running task.
					IDictionary env;
					int count;
					IDictionaryEnumerator e;
					try
					{
						env = Environment.GetEnvironmentVariables();
					}
					catch(SecurityException)
					{
						// The runtime engine has decided that we don't have
						// sufficient permissions to get the environment.
						// We continue with an empty environment.
						env = null;
					}
					if(env != null)
					{
						count = env.Count;
						e = env.GetEnumerator();
					}
					else
					{
						count = 0;
						e = null;
					}

					// Allocate an array to hold the converted values.
					int pointerSize = (int)(sizeof(void *));
					environ = Marshal.AllocHGlobal
						(new IntPtr(pointerSize * (count + 1)));

					// Convert the environment variables into native strings.
					int index = 0;
					String value;
					while(index < count && e != null && e.MoveNext())
					{
						value = String.Concat((String)(e.Key), "=",
											  (String)(e.Value));
						Marshal.WriteIntPtr(environ, pointerSize * index,
							Marshal.StringToHGlobalAnsi(value));
						++index;
					}
					Marshal.WriteIntPtr
						(environ, pointerSize * count, IntPtr.Zero);

					// The environment is ready to go.
					return environ;
				}
			}

	// Perform system library startup tasks.  This is normally
	// called just before invoking the program's "main" function.
	public static void Startup()
			{
				Module mainModule;
				Assembly assembly;
				Type type;
				FieldInfo field;

				// Bail out if we've already done the startup code.
				lock(typeof(Crt0))
				{
					if(startupDone)
					{
						return;
					}
					startupDone = true;
				}

				// Find the module that contains the "main" function.
				mainModule = null;
				try
				{
					assembly = Assembly.GetCallingAssembly();
					type = assembly.GetType("<Module>");
					if(type != null)
					{
						mainModule = type.Module;
					}
				}
				catch(NotImplementedException)
				{
					// The runtime engine probably does not have support
					// for reflection, so there's nothing we can do.
				}

				// Find standard C library's global module.
				libcModule = null;
				try
				{
					assembly = Assembly.Load("libc");
					type = assembly.GetType("libc");
					if(type != null)
					{
						libcModule = type.Module;
					}
				}
				catch(OutOfMemoryException)
				{
					// Send out of memory conditions back up the stack.
					throw;
				}
				catch(Exception)
				{
					// We weren't able to load "libc" for some reason.
				}

				// Set the global "__environ" variable within "libc".
				if(libcModule != null)
				{
					field = libcModule.GetField("__environ");
					if(field != null)
					{
						field.SetValue(null, (Object)environ);
					}
				}

				// Initialize the stdin, stdout, and stderr file descriptors.
			#if CONFIG_SMALL_CONSOLE
				FileTable.SetFileDescriptor(0, Stream.Null);
				FileTable.SetFileDescriptor(1, new ConsoleStream());
				FileTable.SetFileDescriptor(2, new ConsoleStream());
			#else
				FileTable.SetFileDescriptor
					(0, new FDStream(0, Console.OpenStandardInput()));
				FileTable.SetFileDescriptor
					(1, new FDStream(1, Console.OpenStandardOutput()));
				FileTable.SetFileDescriptor
					(2, new FDStream(2, Console.OpenStandardError()));
			#endif

				// Invoke the application's ".init" function, if present.
				if(mainModule != null)
				{
					MethodInfo initMethod = mainModule.GetMethod(".init");
					if(initMethod != null)
					{
						initMethod.Invoke(null, null);
					}
				}

				// Locate the application's ".fini" function.
				if(mainModule != null)
				{
					finiMethod = mainModule.GetMethod(".fini");
				}
				else
				{
					finiMethod = null;
				}
			}

	// Perform system library shutdown tasks and exit the application.
	// This is normally called after invoking the program's "main" function.
	public static void Shutdown(int status)
			{
				// Find the global "exit" function and call it.
				if(libcModule != null)
				{
					MethodInfo method = libcModule.GetMethod("exit");
					if(method != null)
					{
						Object[] args = new Object [1];
						args[0] = (Object)(status);
						method.Invoke(null, args);
					}
				}

				// If we get here, then we weren't able to find "exit",
				// or it returned to us by mistake.  Bail out through
				// "System.Environment".
				Environment.Exit(status);
			}

	// Perform system library shutdown tasks when an exception occurs.
	public static Object ShutdownWithException(Object e)
			{
				// Nothing to do here yet, so return the exception object
				// to the caller to be rethrown to the runtime engine.
				return e;
			}

	// Invoke the ".fini" function at system shutdown.
	public static void InvokeFini()
			{
				lock(typeof(Crt0))
				{
					if(finiDone)
					{
						return;
					}
					finiDone = true;
				}
				if(finiMethod != null)
				{
					finiMethod.Invoke(null, null);
				}
			}

	// Get the module that contains "libc".  Returns "null" if unknown.
	public static Module LibC
			{
				get
				{
					return libcModule;
				}
			}

	// Set the contents of a wide character string during initialization.
	public unsafe static void SetWideString(char *dest, String src)
			{
				int index = 0;
				foreach(char ch in src)
				{
					dest[index++] = ch;
				}
				dest[index] = '\0';
			}

	// Alignment flags.  Keep in sync with "c_types.h" in the pnet C compiler.
	private const uint C_ALIGN_BYTE			= 0x0001;
	private const uint C_ALIGN_2			= 0x0002;
	private const uint C_ALIGN_4			= 0x0004;
	private const uint C_ALIGN_8			= 0x0008;
	private const uint C_ALIGN_16			= 0x0010;
	private const uint C_ALIGN_SHORT		= 0x0020;
	private const uint C_ALIGN_INT			= 0x0040;
	private const uint C_ALIGN_LONG			= 0x0080;
	private const uint C_ALIGN_FLOAT		= 0x0100;
	private const uint C_ALIGN_DOUBLE		= 0x0200;
	private const uint C_ALIGN_POINTER		= 0x0400;
	private const uint C_ALIGN_UNKNOWN		= 0x0800;

	// Align a size value according to a set of flags.  Used by the
	// compiler to help compute the size of complicated types.
	public unsafe static uint Align(uint size, uint flags)
			{
				uint align;
				uint temp;

				// Get the basic alignment value.
				align = 1;
				if((flags & C_ALIGN_2) != 0)
				{
					align = 2;
				}
				if((flags & C_ALIGN_4) != 0)
				{
					align = 4;
				}
				if((flags & C_ALIGN_8) != 0)
				{
					align = 8;
				}
				if((flags & C_ALIGN_16) != 0)
				{
					align = 16;
				}

				// Adjust for specific types that appear.
				if((flags & C_ALIGN_SHORT) != 0)
				{
					temp = (uint)(int)(IntPtr)(void *)
						(&(((align_short *)(void *)(IntPtr.Zero))->value));
					if(temp > align)
					{
						align = temp;
					}
				}
				if((flags & C_ALIGN_INT) != 0)
				{
					temp = (uint)(int)(IntPtr)(void *)
						(&(((align_int *)(void *)(IntPtr.Zero))->value));
					if(temp > align)
					{
						align = temp;
					}
				}
				if((flags & C_ALIGN_LONG) != 0)
				{
					temp = (uint)(int)(IntPtr)(void *)
						(&(((align_long *)(void *)(IntPtr.Zero))->value));
					if(temp > align)
					{
						align = temp;
					}
				}
				if((flags & C_ALIGN_FLOAT) != 0)
				{
					temp = (uint)(int)(IntPtr)(void *)
						(&(((align_float *)(void *)(IntPtr.Zero))->value));
					if(temp > align)
					{
						align = temp;
					}
				}
				if((flags & C_ALIGN_DOUBLE) != 0)
				{
					temp = (uint)(int)(IntPtr)(void *)
						(&(((align_double *)(void *)(IntPtr.Zero))->value));
					if(temp > align)
					{
						align = temp;
					}
				}
				if((flags & C_ALIGN_POINTER) != 0)
				{
					temp = (uint)(int)(IntPtr)(void *)
						(&(((align_pointer *)(void *)(IntPtr.Zero))->value));
					if(temp > align)
					{
						align = temp;
					}
				}

				// Align the final size value and return it.
				if((size % align) != 0)
				{
					size += align - (size % align);
				}
				return size;
			}

	// Types that are used to aid with alignment computations.
	[StructLayout(LayoutKind.Sequential)]
	private struct align_short
	{
		public byte pad;
		public short value;

	}; // struct align_short
	[StructLayout(LayoutKind.Sequential)]
	private struct align_int
	{
		public byte pad;
		public int value;

	}; // struct align_int
	[StructLayout(LayoutKind.Sequential)]
	private struct align_long
	{
		public byte pad;
		public long value;

	}; // struct align_long
	[StructLayout(LayoutKind.Sequential)]
	private struct align_float
	{
		public byte pad;
		public float value;

	}; // struct align_float
	[StructLayout(LayoutKind.Sequential)]
	private struct align_double
	{
		public byte pad;
		public double value;

	}; // struct align_double
	[StructLayout(LayoutKind.Sequential)]
	private unsafe struct align_pointer
	{
		public byte pad;
		public void *value;

	}; // struct align_pointer

#if CONFIG_SMALL_CONSOLE

	// Helper class for writing to stdout when the System.Console
	// class does not have "OpenStandardOutput".
	private sealed class ConsoleStream : Stream
	{
		// Constructor.
		public ConsoleStream() {}

		// Stub out all stream functionality.
		public override void Flush() {}
		public override int Read(byte[] buffer, int offset, int count)
				{
					throw new NotSupportedException();
				}
		public override int ReadByte()
				{
					throw new NotSupportedException();
				}
		public override long Seek(long offset, SeekOrigin origin)
				{
					throw new NotSupportedException();
				}
		public override void SetLength(long value)
				{
					throw new NotSupportedException();
				}
		public override void Write(byte[] buffer, int offset, int count)
				{
					// Validate the buffer argument.
					if(buffer == null)
					{
						throw new ArgumentNullException("buffer");
					}
					else if(offset < 0 || offset > buffer.Length)
					{
						throw new ArgumentOutOfRangeException();
					}
					else if(count < 0)
					{
						throw new ArgumentOutOfRangeException();
					}
					else if((buffer.Length - offset) < count)
					{
						throw new ArgumentException();
					}

					// Write the contents of the buffer.
					while(count > 0)
					{
						Console.Write((char)(buffer[offset]));
						++offset;
						--count;
					}
				}
		public override void WriteByte(byte value)
				{
					Console.Write((char)value);
				}
		public override bool CanRead { get { return false; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return true; } }
		public override long Length
				{
					get
					{
						throw new NotSupportedException();
					}
				}
		public override long Position
				{
					get
					{
						throw new NotSupportedException();
					}
					set
					{
						throw new NotSupportedException();
					}
				}

	}; // class ConsoleStream

#endif // !CONFIG_SMALL_CONSOLE

} // class Crt0

} // namespace OpenSystem.C
