/*
 * Trace.cs - Implementation of the
 *			"System.Diagnostics.Trace" class.
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

#define	TRACE

namespace System.Diagnostics
{

#if !ECMA_COMPAT

using System.Configuration;
using System.Collections;

public sealed class Trace
{
	// Internal state.
	private static bool initialized = false;
	private static bool autoFlush;
	private static int indentLevel;
	private static int indentSize = 4;
	private static TraceListenerCollection listeners;
	internal static Hashtable switches;

	// This class cannot be instantiated.
	private Trace() {}

	// Make sure that the trace configuration is loaded.
	internal static void Initialize()
			{
				// Bail out if already initialized, or called recursively.
				if(initialized)
				{
					return;
				}
				initialized = true;

				// Create the default trace listener.
				DefaultTraceListener defListener =
					new DefaultTraceListener();

				// Create the initial listeners collection.
				listeners = new TraceListenerCollection();
				listeners.Add(defListener);

				// Get the diagnostics configuration options.
				Hashtable options = (Hashtable)
					ConfigurationSettings.GetConfig
						("system.diagnostics",
						 new DiagnosticsConfigurationHandler());
				if(options == null)
				{
					options = new Hashtable();
				}

				// Process the options for the default trace listener.
				Object value = options["assertuienabled"];
				if(value != null)
				{
					defListener.AssertUiEnabled = (bool)value;
				}
				value = options["logfilename"];
				if(value != null)
				{
					defListener.LogFileName = (String)value;
				}

				// Process the trace options.
				value = options["autoflush"];
				if(value != null)
				{
					autoFlush = (bool)value;
				}
				value = options["indentsize"];
				if(value != null)
				{
					indentSize = (int)value;
				}
				switches = (Hashtable)(options["switches"]);
			}

	// Global trace properties.
	public static bool AutoFlush
			{
				get
				{
					lock(typeof(Trace))
					{
						Initialize();
						return autoFlush;
					}
				}
				set
				{
					lock(typeof(Trace))
					{
						Initialize();
						autoFlush = value;
					}
				}
			}
	public static int IndentLevel
			{
				get
				{
					lock(typeof(Trace))
					{
						Initialize();
						return indentLevel;
					}
				}
				set
				{
					lock(typeof(Trace))
					{
						Initialize();
						if(value < 0)
						{
							value = 0;
						}
						indentLevel = value;
						foreach(TraceListener listener in Listeners)
						{
							listener.IndentLevel = value;
						}
					}
				}
			}
	public static int IndentSize
			{
				get
				{
					lock(typeof(Trace))
					{
						Initialize();
						return indentSize;
					}
				}
				set
				{
					lock(typeof(Trace))
					{
						Initialize();
						if(value < 0)
						{
							value = 0;
						}
						indentSize = value;
						foreach(TraceListener listener in Listeners)
						{
							listener.IndentSize = value;
						}
					}
				}
			}
	public static TraceListenerCollection Listeners
			{
				get
				{
					lock(typeof(Trace))
					{
						Initialize();
						return listeners;
					}
				}
			}

	// Assert on a particular condition.
	[Conditional("TRACE")]
	public static void Assert(bool condition)
			{
				if(!condition)
				{
					Fail(String.Empty, null);
				}
			}
	[Conditional("TRACE")]
	public static void Assert(bool condition, String message)
			{
				if(!condition)
				{
					Fail(message, null);
				}
			}
	[Conditional("TRACE")]
	public static void Assert(bool condition, String message,
							  String detailMessage)
			{
				if(!condition)
				{
					Fail(message, detailMessage);
				}
			}

	// Flush and close all listeners.
	[Conditional("TRACE")]
	public static void Close()
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.Close();
				}
			}

	// Record that some condition has failed.
	[Conditional("TRACE")]
	public static void Fail(String message)
			{
				Fail(message, null);
			}
	[Conditional("TRACE")]
	public static void Fail(String message, String detailMessage)
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.Fail(message, detailMessage);
				}
			}

	// Flush all trace listeners.
	[Conditional("TRACE")]
	public static void Flush()
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.Flush();
				}
			}

	// Increase the indent level by one.
	[Conditional("TRACE")]
	public static void Indent()
			{
				IndentLevel = IndentLevel + 1;
			}

	// Decrease the indent level by one.
	[Conditional("TRACE")]
	public static void Unindent()
			{
				int level = IndentLevel - 1;
				if(level >= 0)
				{
					IndentLevel = level;
				}
			}

	// Write a message to all trace listeners.
	[Conditional("TRACE")]
	public static void Write(Object value)
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.Write(value);
					if(AutoFlush)
					{
						listener.Flush();
					}
				}
			}
	[Conditional("TRACE")]
	public static void Write(String message)
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.Write(message);
					if(AutoFlush)
					{
						listener.Flush();
					}
				}
			}
	[Conditional("TRACE")]
	public static void Write(Object value, String category)
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.Write(value, category);
					if(AutoFlush)
					{
						listener.Flush();
					}
				}
			}
	[Conditional("TRACE")]
	public static void Write(String message, String category)
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.Write(message, category);
					if(AutoFlush)
					{
						listener.Flush();
					}
				}
			}

	// Write a message to all trace listeners if a condition is true.
	[Conditional("TRACE")]
	public static void WriteIf(bool condition, Object value)
			{
				if(condition)
				{
					Write(value);
				}
			}
	[Conditional("TRACE")]
	public static void WriteIf(bool condition, String message)
			{
				if(condition)
				{
					Write(message);
				}
			}
	[Conditional("TRACE")]
	public static void WriteIf(bool condition, Object value, String category)
			{
				if(condition)
				{
					Write(value, category);
				}
			}
	[Conditional("TRACE")]
	public static void WriteIf(bool condition, String message, String category)
			{
				if(condition)
				{
					Write(message, category);
				}
			}

	// Write a message to all trace listeners.
	[Conditional("TRACE")]
	public static void WriteLine(Object value)
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.WriteLine(value);
					if(AutoFlush)
					{
						listener.Flush();
					}
				}
			}
	[Conditional("TRACE")]
	public static void WriteLine(String message)
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.WriteLine(message);
					if(AutoFlush)
					{
						listener.Flush();
					}
				}
			}
	[Conditional("TRACE")]
	public static void WriteLine(Object value, String category)
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.WriteLine(value, category);
					if(AutoFlush)
					{
						listener.Flush();
					}
				}
			}
	[Conditional("TRACE")]
	public static void WriteLine(String message, String category)
			{
				foreach(TraceListener listener in Listeners)
				{
					listener.WriteLine(message, category);
					if(AutoFlush)
					{
						listener.Flush();
					}
				}
			}

	// Write a message to all trace listeners if a condition is true.
	[Conditional("TRACE")]
	public static void WriteLineIf(bool condition, Object value)
			{
				if(condition)
				{
					WriteLine(value);
				}
			}
	[Conditional("TRACE")]
	public static void WriteLineIf(bool condition, String message)
			{
				if(condition)
				{
					WriteLine(message);
				}
			}
	[Conditional("TRACE")]
	public static void WriteLineIf
				(bool condition, Object value, String category)
			{
				if(condition)
				{
					WriteLine(value, category);
				}
			}
	[Conditional("TRACE")]
	public static void WriteLineIf
				(bool condition, String message, String category)
			{
				if(condition)
				{
					WriteLine(message, category);
				}
			}

}; // class Trace

#endif // !ECMA_COMPAT

}; // namespace System.Diagnostics
