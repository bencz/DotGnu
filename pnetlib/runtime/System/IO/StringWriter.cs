/*
 * StringWriter.cs - Implementation of the "System.IO.StringWriter" class.
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

namespace System.IO
{

using System;
using System.Text;

public class StringWriter : TextWriter
{
	// Internal state.
	private StringBuilder builder;
	private Encoding encoding;
	private bool closed;

	// Constructors.
	public StringWriter() : base()
			{
				builder = new StringBuilder();
				closed = false;
			}
	public StringWriter(IFormatProvider provider) : base(provider)
			{
				builder = new StringBuilder();
				closed = false;
			}
	public StringWriter(StringBuilder sb) : base()
			{
				if(sb == null)
				{
					throw new ArgumentNullException("sb");
				}
				builder = sb;
				closed = false;
			}
	public StringWriter(StringBuilder sb, IFormatProvider provider)
			: base(provider)
			{
				if(sb == null)
				{
					throw new ArgumentNullException("sb");
				}
				builder = sb;
				closed = false;
			}

	// Close this writer.
	public override void Close()
			{
				Dispose(true);
			}

	// Dispose of this writer.
	protected override void Dispose(bool disposing)
			{
				closed = true;
			}

	// Get the underlying string builder.
	public virtual StringBuilder GetStringBuilder()
			{
				return builder;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return builder.ToString();
			}

	// Write values to this writer.
	public override void Write(String value)
			{
				if(closed)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				if(value != null)
				{
					builder.Append(value);
				}
			}
	public override void Write(char[] buffer, int index, int count)
			{
				Stream.ValidateBuffer(buffer, index, count);
				if(closed)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				if(count > 0)
				{
					builder.Append(buffer, index, count);
				}
			}
	public override void Write(char ch)
			{
				if(closed)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				builder.Append(ch);
			}

	// Get the encoding for this writer.
	public override Encoding Encoding
			{
				get
				{
					if(encoding == null)
					{
						encoding = new UnicodeEncoding(false, false);
					}
					return encoding;
				}
			}

}; // class StringWriter

}; // namespace System.IO
