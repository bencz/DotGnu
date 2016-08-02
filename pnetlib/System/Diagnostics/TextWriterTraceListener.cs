/*
 * TextWriterTraceListener.cs - Implementation of the
 *			"System.Diagnostics.TextWriterTraceListener" class.
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

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.IO;

public class TextWriterTraceListener : TraceListener
{
	// Internal state.
	private TextWriter writer;

	// Constructor.
	public TextWriterTraceListener() : base("TextWriter") {}
	public TextWriterTraceListener(Stream stream) : base("")
			{
				writer = new StreamWriter(stream);
			}
	public TextWriterTraceListener(String fileName) : base("")
			{
				writer = new StreamWriter(fileName);
			}
	public TextWriterTraceListener(TextWriter writer) : base("")
			{
				this.writer = writer;
			}
	public TextWriterTraceListener(Stream stream, String name)
			: base(name == null ? String.Empty : name)
			{
				writer = new StreamWriter(stream);
			}
	public TextWriterTraceListener(String fileName, String name)
			: base(name == null ? String.Empty : name)
			{
				writer = new StreamWriter(fileName);
			}
	public TextWriterTraceListener(TextWriter writer, String name)
			: base(name == null ? String.Empty : name)
			{
				this.writer = writer;
			}

	// Get or set the writer being used by this trace listener.
	public TextWriter Writer
			{
				get
				{
					return writer;
				}
				set
				{
					writer = value;
				}
			}

	// Close this trace listener.
	public override void Close()
			{
				if(writer != null)
				{
					writer.Close();
				}
			}

	// Dispose this trace listener.
	protected override void Dispose(bool disposing)
			{
				if(disposing)
				{
					Close();
				}
			}

	// Flush this trace listener's output stream.
	public override void Flush()
			{
				if(writer != null)
				{
					writer.Flush();
				}
			}

	// Write data to this trace listener's output stream.
	public override void Write(String message)
			{
				if(NeedIndent)
				{
					WriteIndent();
				}
				if(writer != null)
				{
					writer.Write(message);
				}
			}

	// Write data to this trace listener's output stream followed by newline.
	public override void WriteLine(String message)
			{
				if(NeedIndent)
				{
					WriteIndent();
				}
				if(writer != null)
				{
					writer.WriteLine(message);
				}
				NeedIndent = true;
			}

}; // class TextWriterTraceListener

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
