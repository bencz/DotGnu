/*
 * StringReader.cs - Implementation of the "System.IO.StringReader" class.
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

public class StringReader : TextReader
{
	// Internal state.
	private String str;
	private int posn;
	private bool closed;

	// Constructor.
	public StringReader(String s)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				this.str = s;
				this.posn = 0;
				this.closed = false;
			}

	// Close the reader.
	public override void Close()
			{
				closed = true;
			}

	// Dispose of this reader.
	protected override void Dispose(bool disposing)
			{
				closed = true;
			}

	// Peek at the next character in the stream.
	public override int Peek()
			{
				if(closed)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				else if(posn < str.Length)
				{
					return (int)(str[posn]);
				}
				else
				{
					return -1;
				}
			}

	// Read a buffer of characters.
	public override int Read(char[] buffer, int index, int count)
			{
				Stream.ValidateBuffer(buffer, index, count);
				if(closed)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				int left = (str.Length - posn);
				if(count > left)
				{
					count = left;
				}
				if(count > 0)
				{
					str.CopyTo(posn, buffer, index, count);
					posn += count;
				}
				return count;
			}

	// Read the next character in the stream.
	public override int Read()
			{
				if(closed)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				else if(posn < str.Length)
				{
					return (int)(str[posn++]);
				}
				else
				{
					return -1;
				}
			}

	// Read the next line of input.
	public override String ReadLine()
			{
				if(closed)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				if(posn >= str.Length)
				{
					return null;
				}
				int index1 = str.IndexOf('\n', posn);
				int index2 = str.IndexOf('\r', posn);
				String line;
				if(index1 == -1 && index2 == -1)
				{
					// No end of line marker - return the rest of the string.
					line = str.Substring(posn);
					posn = str.Length;
				}
				else if(index1 != -1 &&
				        (index2 == -1 || index1 <= index2))
				{
					// Line is terminated by LF.
					line = str.Substring(posn, index1 - posn);
					posn = index1 + 1;
				}
				else
				{
					// Line is terminated by CR or CRLF.
					line = str.Substring(posn, index2 - posn);
					posn = index2 + 1;
					if(posn < str.Length && str[posn] == '\n')
					{
						++posn;
					}
				}
				return line;
			}

	// Read the remainder of the input stream.
	public override String ReadToEnd()
			{
				if(closed)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				return str.Substring(posn);
			}

}; // class StringReader

}; // namespace System.IO
