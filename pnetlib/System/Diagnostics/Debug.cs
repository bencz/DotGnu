/*
 * Debug.cs - Implementation of the
 *			"System.Diagnostics.Debug" class.
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

#define	DEBUG
#define	TRACE

namespace System.Diagnostics
{

#if !ECMA_COMPAT

public sealed class Debug
{
	// This class cannot be instantiated.
	private Debug() {}

	// Global trace properties.
	public static bool AutoFlush
			{
				get
				{
					return Trace.AutoFlush;
				}
				set
				{
					Trace.AutoFlush = value;
				}
			}
	public static int IndentLevel
			{
				get
				{
					return Trace.IndentLevel;
				}
				set
				{
					Trace.IndentLevel = value;
				}
			}
	public static int IndentSize
			{
				get
				{
					return Trace.IndentSize;
				}
				set
				{
					Trace.IndentSize = value;
				}
			}
	public static TraceListenerCollection Listeners
			{
				get
				{
					return Trace.Listeners;
				}
			}

	// Assert on a particular condition.
	[Conditional("DEBUG")]
	public static void Assert(bool condition)
			{
				Trace.Assert(condition);
			}
	[Conditional("DEBUG")]
	public static void Assert(bool condition, String message)
			{
				Trace.Assert(condition, message);
			}
	[Conditional("DEBUG")]
	public static void Assert(bool condition, String message,
							  String detailMessage)
			{
				Trace.Assert(condition, message, detailMessage);
			}

	// Flush and close all listeners.
	[Conditional("DEBUG")]
	public static void Close()
			{
				Trace.Close();
			}

	// Record that some condition has failed.
	[Conditional("DEBUG")]
	public static void Fail(String message)
			{
				Trace.Fail(message);
			}
	[Conditional("DEBUG")]
	public static void Fail(String message, String detailMessage)
			{
				Trace.Fail(message, detailMessage);
			}

	// Flush all trace listeners.
	[Conditional("DEBUG")]
	public static void Flush()
			{
				Trace.Flush();
			}

	// Increase the indent level by one.
	[Conditional("DEBUG")]
	public static void Indent()
			{
				Trace.Indent();
			}

	// Decrease the indent level by one.
	[Conditional("DEBUG")]
	public static void Unindent()
			{
				Trace.Unindent();
			}

	// Write a message to all trace listeners.
	[Conditional("DEBUG")]
	public static void Write(Object value)
			{
				Trace.Write(value);
			}
	[Conditional("DEBUG")]
	public static void Write(String message)
			{
				Trace.Write(message);
			}
	[Conditional("DEBUG")]
	public static void Write(Object value, String category)
			{
				Trace.Write(value, category);
			}
	[Conditional("DEBUG")]
	public static void Write(String message, String category)
			{
				Trace.Write(message, category);
			}

	// Write a message to all trace listeners if a condition is true.
	[Conditional("DEBUG")]
	public static void WriteIf(bool condition, Object value)
			{
				Trace.WriteIf(condition, value);
			}
	[Conditional("DEBUG")]
	public static void WriteIf(bool condition, String message)
			{
				Trace.WriteIf(condition, message);
			}
	[Conditional("DEBUG")]
	public static void WriteIf(bool condition, Object value, String category)
			{
				Trace.WriteIf(condition, value, category);
			}
	[Conditional("DEBUG")]
	public static void WriteIf(bool condition, String message, String category)
			{
				Trace.WriteIf(condition, message, category);
			}

	// Write a message to all trace listeners.
	[Conditional("DEBUG")]
	public static void WriteLine(Object value)
			{
				Trace.WriteLine(value);
			}
	[Conditional("DEBUG")]
	public static void WriteLine(String message)
			{
				Trace.WriteLine(message);
			}
	[Conditional("DEBUG")]
	public static void WriteLine(Object value, String category)
			{
				Trace.WriteLine(value, category);
			}
	[Conditional("DEBUG")]
	public static void WriteLine(String message, String category)
			{
				Trace.WriteLine(message, category);
			}

	// Write a message to all trace listeners if a condition is true.
	[Conditional("DEBUG")]
	public static void WriteLineIf(bool condition, Object value)
			{
				Trace.WriteLineIf(condition, value);
			}
	[Conditional("DEBUG")]
	public static void WriteLineIf(bool condition, String message)
			{
				Trace.WriteLineIf(condition, message);
			}
	[Conditional("DEBUG")]
	public static void WriteLineIf
				(bool condition, Object value, String category)
			{
				Trace.WriteLineIf(condition, value, category);
			}
	[Conditional("DEBUG")]
	public static void WriteLineIf
				(bool condition, String message, String category)
			{
				Trace.WriteLineIf(condition, message, category);
			}

}; // class Debug

#endif // !ECMA_COMPAT

}; // namespace System.Diagnostics
