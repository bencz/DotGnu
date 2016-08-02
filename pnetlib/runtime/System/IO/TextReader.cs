/*
 * TextReader.cs - Implementation of the "System.IO.TextReader" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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
using System.Globalization;
using System.Text;

public abstract class TextReader : MarshalByRefObject, IDisposable
{

	// The "Null" text reader that always reports EOF.
	public static readonly TextReader Null = new NullTextReader();

	// Constructor.
	protected TextReader() : base() {}

	// Close this text reader.
	public virtual void Close()
			{
				Dispose(true);
			}

	// Dispose this text reader.  Normally overridden by subclasses.
	protected virtual void Dispose(bool disposing)
			{
				// Nothing to do in the base class.
			}

	// Implement IDisposable.
	void IDisposable.Dispose()
			{
				Dispose(true);
			}

	// Peek at the next character.  Normally overridden by subclasses.
	public virtual int Peek()
			{
				return -1;
			}

	// Read the next character.  Normally overridden by subclasses.
	public virtual int Read()
			{
				return -1;
			}

	// Read a buffer of characters.  Normally overridden by subclasses.
	public virtual int Read(char[] buffer, int index, int count)
			{
				int readLen;
				int ch;
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((buffer.Length - index) < count)
				{
					throw new ArgumentException
						(_("Arg_InvalidArrayRange"));
				}
				readLen = 0;
				while(count > 0)
				{
					ch = Read();
					if(ch == -1)
					{
						break;
					}
					buffer[index++] = (char)ch;
					--count;
					++readLen;
				}
				return readLen;
			}

	// Read a buffer of characters, and fill the entire block.
	// Normally overridden by subclasses.
	public virtual int ReadBlock(char[] buffer, int index, int count)
			{
				int readLen = 0;
				int tempLen;
				do
				{
					tempLen = Read(buffer, index + readLen, count - readLen);
					if(tempLen != 0)
					{
						readLen += tempLen;
					}
					else
					{
						break;
					}
				}
				while(readLen < count);
				return readLen;
			}

	// Read the next line from this reader.
	public virtual String ReadLine()
			{
				StringBuilder builder = new StringBuilder();
				int ch;
				while((ch = Read()) != -1)
				{
					if(ch == 13)
					{
						if(Peek() == 10)
						{
							Read();
						}
						return builder.ToString();
					}
					else if(ch == 10)
					{
						return builder.ToString();
					}
					else
					{
						builder.Append((char)ch);
					}
				}
				if(builder.Length != 0)
				{
					return builder.ToString();
				}
				else
				{
					return null;
				}
			}

	// Read until the end of the stream.
	public virtual String ReadToEnd()
			{
				StringBuilder builder = new StringBuilder();
				char[] buf = new char [128];
				int readLen;
				while((readLen = Read(buf, 0, 128)) > 0)
				{
					builder.Append(buf, 0, readLen);
				}
				if(builder.Length != 0)
				{
					return builder.ToString();
				}
				else
				{
					return String.Empty;
				}
			}

	// Wrap a text reader to make it synchronized.
	public static TextReader Synchronized(TextReader reader)
			{
				if(reader == null)
				{
					throw new ArgumentNullException("reader");
				}
				if(reader is SyncTextReader)
				{
					return reader;
				}
				else
				{
					return new SyncTextReader(reader);
				}
			}

	// Private class that implements the null text reader.
	private sealed class NullTextReader : TextReader
	{
		// Constructor.
		public NullTextReader() : base() {}

		// Override common reading methods to null them out.
		public override int Read(char[] value, int index, int count)
				{ return 0; }
		public override int ReadBlock(char[] value, int index, int count)
				{ return 0; }
		public override String ReadLine() { return null; }
		public override String ReadToEnd() { return String.Empty; }

	}; // class NullTextReader

	// Private class that implements synchronized text readers.
	private sealed class SyncTextReader : TextReader
	{
		// Private storage for the underlying reader.
		private TextReader reader;

		// Constructor.
		public SyncTextReader(TextReader reader)
				{
					this.reader = reader;
				}

		// Close this text reader.
		public override void Close()
				{
					lock(reader)
					{
						reader.Close();
					}
				}
	
		// Dispose this text reader.
		protected override void Dispose(bool disposing)
				{
					lock(reader)
					{
						reader.Dispose(disposing);
					}
				}
	
		// Peek at the next character.
		public override int Peek()
				{
					lock(reader)
					{
						return reader.Peek();
					}
				}

		// Read the next character.
		public override int Read()
				{
					lock(reader)
					{
						return reader.Read();
					}
				}

		// Read a buffer of characters.
		public override int Read(char[] buffer, int index, int count)
				{
					lock(reader)
					{
						return reader.Read(buffer, index, count);
					}
				}

		// Read a buffer of characters, and fill the entire block.
		public override int ReadBlock(char[] buffer, int index, int count)
				{
					lock(reader)
					{
						return reader.ReadBlock(buffer, index, count);
					}
				}

		// Read the next line from this reader.
		public override String ReadLine()
				{
					lock(reader)
					{
						return reader.ReadLine();
					}
				}

		// Read until the end of the stream.
		public override String ReadToEnd()
				{
					lock(reader)
					{
						return reader.ReadToEnd();
					}
				}

	}; // class SyncTextReader
	
}; // class TextReader

}; // namespace System.IO
