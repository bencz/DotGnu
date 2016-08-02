/*
 * DefaultTraceListener.cs - Implementation of the
 *			"System.Diagnostics.DefaultTraceListener" class.
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

namespace System.Diagnostics
{

#if !ECMA_COMPAT

using System.IO;
using System.Runtime.InteropServices;

[ComVisible(false)]
public class DefaultTraceListener : TraceListener
{
	// Internal state.
	private bool loadDone;
	private bool assertUiEnabled;
	private String logFileName;

	// Constructor.
	public DefaultTraceListener() : base("Default") {}

	// Get or set the "UI enabled" flag (not used in this implementation).
	public bool AssertUiEnabled
			{
				get
				{
					lock(this)
					{
						return assertUiEnabled;
					}
				}
				set
				{
					lock(this)
					{
						assertUiEnabled = value;
					}
				}
			}

	// Get or set the name of the log file.
	public String LogFileName
			{
				get
				{
					lock(this)
					{
						return logFileName;
					}
				}
				set
				{
					lock(this)
					{
						logFileName = value;
					}
				}
			}

	// Log a failure message to this trace listener.
	public override void Fail(String message)
			{
				Fail(message, null);
			}
	public override void Fail(String message, String detailMessage)
			{
				// Write the failure message, but don't pop up a dialog.
				// Dialog support is not available on all platforms.
				WriteLine(String.Format(S._("Arg_AssertFailed"), message));
				if(detailMessage != null && detailMessage != String.Empty)
				{
					WriteLine(detailMessage);
				}
			#if CONFIG_EXTENDED_DIAGNOSTICS
				String trace = (new StackTrace(true)).ToString();
				if(trace != null && trace != String.Empty)
				{
					Write(trace);
				}
			#endif

				// Exit from the application after reporting the failure.
				Environment.Exit(1);
			}

	// Write data to this trace listener's output stream.
	public override void Write(String message)
			{
				lock(this)
				{
					if(NeedIndent)
					{
						WriteIndent();
					}
				#if CONFIG_SMALL_CONSOLE
					Console.Write(message);
				#else
					Console.Error.Write(message);
				#endif
					WriteLog(message, false);
				}
			}

	// Write data to this trace listener's output stream followed by newline.
	public override void WriteLine(String message)
			{
				lock(this)
				{
					if(NeedIndent)
					{
						WriteIndent();
					}
				#if CONFIG_SMALL_CONSOLE
					Console.WriteLine(message);
				#else
					Console.Error.WriteLine(message);
				#endif
					WriteLog(message, true);
					NeedIndent = true;
				}
			}

	// Write a string to the log file.
	private void WriteLog(String message, bool eol)
			{
				if(logFileName != null && logFileName.Length > 0)
				{
					StreamWriter writer = new StreamWriter
						(logFileName, true);
					if(eol)
					{
						writer.WriteLine(message);
					}
					else
					{
						writer.Write(message);
					}
					writer.Flush();
					writer.Close();
				}
			}

}; // class DefaultTraceListener

#endif // !ECMA_COMPAT

}; // namespace System.Diagnostics
