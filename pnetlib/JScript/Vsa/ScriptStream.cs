/*
 * ScriptStream.cs - capture stream output from an engine.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript.Vsa
{

using System;
using System.IO;

public class ScriptStream
{

#if !CONFIG_SMALL_CONSOLE

	// Global stdout and stderr streams.
	public static TextWriter Out = Console.Out;
	public static TextWriter Error = Console.Error;

	// Print an exception stack trace.
	public static void PrintStackTrace(Exception e)
			{
				Out.WriteLine(e.StackTrace);
				Out.Flush();
			}
	public static void PrintStackTrace()
			{
				// Get the stack trace for the current method
				// by throwing a dummy exception.
				try
				{
					throw new Exception();
				}
				catch(Exception e)
				{
					PrintStackTrace(e);
				}
			}

	// Write a string to stdout.
	public static void Write(String str)
			{
				Out.Write(str);
			}
	public static void WriteLine(String str)
			{
				Out.WriteLine(str);
			}

#else // CONFIG_SMALL_CONSOLE

	// Print an exception stack trace.
	public static void PrintStackTrace(Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
	public static void PrintStackTrace()
			{
				// Get the stack trace for the current method
				// by throwing a dummy exception.
				try
				{
					throw new Exception();
				}
				catch(Exception e)
				{
					PrintStackTrace(e);
				}
			}

	// Write a string to stdout.
	public static void Write(String str)
			{
				Console.Write(str);
			}
	public static void WriteLine(String str)
			{
				Console.WriteLine(str);
			}

#endif // CONFIG_SMALL_CONSOLE

}; // class ScriptStream

}; // namespace Microsoft.JScript.Vsa
