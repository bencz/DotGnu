/*
 * COMCharStream.cs - stream object wrapped around an IMessageReceiver.
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
 
namespace Microsoft.JScript
{

using System;
using System.IO;
using System.Text;

public class COMCharStream : Stream
{
	// Internal state.
	private IMessageReceiver messageReceiver;
	private StringBuilder builder;

	// Constructor.
	public COMCharStream(IMessageReceiver messageReceiver)
			{
				this.messageReceiver = messageReceiver;
				this.builder = new StringBuilder();
			}

	// Close the stream.
	public override void Close()
			{
				Flush();
			}

	// Flush the pending contents in this stream.
	public override void Flush()
			{
				messageReceiver.Message(builder.ToString());
				builder = new StringBuilder();
			}

	// Read data from this stream.
	public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

	// Seek to a new position within this stream.
	public override long Seek(long offset, SeekOrigin origin)
			{
				return 0;
			}

	// Set the length of this stream.
	public override void SetLength(long value)
			{
				builder.Length = (int)value;
			}

	// Write a buffer of bytes to this stream.
	public override void Write(byte[] buffer, int offset, int count)
			{
				while(count > 0)
				{
					builder.Append((char)(buffer[offset++]));
					--count;
				}
			}

	// Determine if it is possible to read from this stream.
	public override bool CanRead
			{
				get
				{
					return false;
				}
			}

	// Determine if it is possible to seek within this stream.
	public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

	// Determine if it is possible to write to this stream.
	public override bool CanWrite
			{
				get
				{
					return true;
				}
			}

	// Get the length of this stream.
	public override long Length
			{
				get
				{
					return builder.Length;
				}
			}

	// Get the current position within the stream.
	public override long Position
			{
				get
				{
					return builder.Length;
				}
				set
				{
					// Nothing to do here.
				}
			}

}; // class COMCharStream

}; // namespace Microsoft.JScript.Vsa
