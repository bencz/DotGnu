/*
 * MemoryStream.cs - Implementation of the "System.IO.MemoryStream" class.
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

public class MemoryStream : Stream
{
	// Internal state.
	private byte[] buffer;
	private int start, capacity;
	private int position, length;
	private bool resizable;
	private bool writable;
	private bool publiclyVisible;
	private bool closed;

	// Constructors.
	public MemoryStream() : this(0) {}
	public MemoryStream(int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_NonNegative"));
				}
				this.buffer = new byte [capacity];
				this.start = 0;
				this.capacity = capacity;
				this.position = 0;
				this.length = 0;
				this.resizable = true;
				this.writable = true;
				this.publiclyVisible = true;
				this.closed = false;
			}
	public MemoryStream(byte[] buffer) : this(buffer, true) {}
	public MemoryStream(byte[] buffer, bool writable)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				this.buffer = buffer;
				this.start = 0;
				this.capacity = buffer.Length;
				this.position = 0;
				this.length = buffer.Length;
				this.resizable = false;
				this.writable = writable;
				this.publiclyVisible = false;
				this.closed = false;
			}
	public MemoryStream(byte[] buffer, int index, int count)
			: this(buffer, index, count, true, false) {}
	public MemoryStream(byte[] buffer, int index, int count, bool writable)
			: this(buffer, index, count, writable, false) {}
	public MemoryStream(byte[] buffer, int index, int count,
						bool writable, bool publiclyVisible)
			{
				ValidateBuffer(buffer, index, count);
				this.buffer = buffer;
				this.start = index;
				this.capacity = index + count;
				this.position = index;
				this.length = index + count;
				this.resizable = false;
				this.writable = writable;
				this.publiclyVisible = publiclyVisible;
				this.closed = false;
			}

	// Close this stream.
	public override void Close()
			{
				closed = true;
			}

	// Flush this stream.  Nothing to do here.
	public override void Flush() {}

	// Get the buffer underlying this stream.
	public virtual byte[] GetBuffer()
			{
				if(publiclyVisible)
				{
					return buffer;
				}
				else
				{
					throw new UnauthorizedAccessException
						(_("Exception_HiddenBuffer"));
				}
			}

	// Determine if the memory stream is still open and readable.
	private void CheckForRead()
			{
				if(closed)
				{
					throw new ObjectDisposedException
						(null, _("IO_StreamClosed"));
				}
			}

	// Determine if the memory stream is still open and writable.
	private void CheckForWrite()
			{
				if(closed)
				{
					throw new ObjectDisposedException
						(null, _("IO_StreamClosed"));
				}
				if(!writable)
				{
					throw new NotSupportedException(_("IO_NotSupp_Write"));
				}
			}

	// Increase the capacity of the buffer to contain position "n".
	private void IncreaseCapacity(int n)
			{
				// Bail out if the buffer is not resizable.
				if(!resizable)
				{
					throw new NotSupportedException
						(_("IO_FixedCapacity"));
				}

				// Determine the new buffer capacity.  Note: "start" will
				// normally be zero, but we handle non-zero just in case
				// this needs to change in the future.
				n -= start;
				if(n < 256)
				{
					n = 256;
				}
				if(n < (capacity - start) * 2)
				{
					n = (capacity - start) * 2;
				}

				// Copy the data into a new buffer.
				byte[] newBuffer = new byte [n];
				if(length > start)
				{
					Array.Copy(buffer, start, newBuffer, 0, length - start);
				}

				// Update this object with the new buffer's details.
				buffer = newBuffer;
				capacity = n;
				position -= start;
				length -= start;
				start = 0;
			}

	// Read from the memory stream.
	public override int Read(byte[] buffer, int offset, int count)
			{
				CheckForRead();
				ValidateBuffer(buffer, offset, count);
				int len = this.length - this.position;
				if(len > count)
				{
					len = count;
				}
				if(len <= 0)
				{
					// May be less than zero if the seek position is beyond
					// the end of the memory stream's length.
					return 0;
				}
				Array.Copy(this.buffer, this.position, buffer, offset, len);
				this.position += len;
				return len;
			}

	// Read a single byte from the memory stream.
	public override int ReadByte()
			{
				if(!closed)
				{
					if(position < length)
					{
						return buffer[position++];
					}
					else
					{
						return -1;
					}
				}
				else
				{
					throw new ObjectDisposedException
						(null, _("IO_StreamClosed"));
				}
			}

	// Seek to a new position within the memory stream.
	public override long Seek(long offset, SeekOrigin loc)
			{
				CheckForRead();
				if(offset > Int32.MaxValue)
				{
					throw new ArgumentOutOfRangeException
						("offset", _("IO_InvalidSeekPosition"));
				}
				if(loc == SeekOrigin.Begin)
				{
					// Seek from the beginning of the stream.
					if(offset < 0)
					{
						throw new IOException(_("IO_InvalidSeekPosition"));
					}
					try
					{
						position = checked(start + ((int)offset));
					}
					catch(OverflowException)
					{
						throw new IOException(_("IO_InvalidSeekPosition"));
					}
				}
				else if(loc == SeekOrigin.Current)
				{
					// Seek relative to the current position in the stream.
					if((offset + position) < start)
					{
						throw new IOException(_("IO_InvalidSeekPosition"));
					}
					try
					{
						position = checked(position + ((int)offset));
					}
					catch(OverflowException)
					{
						throw new IOException(_("IO_InvalidSeekPosition"));
					}
				}
				else if(loc == SeekOrigin.End)
				{
					// Seek relative to the end of the stream.
					if((offset + length) < start)
					{
						throw new IOException(_("IO_InvalidSeekPosition"));
					}
					try
					{
						position = checked(length + ((int)offset));
					}
					catch(OverflowException)
					{
						throw new IOException(_("IO_InvalidSeekPosition"));
					}
				}
				else
				{
					throw new ArgumentException(_("IO_InvalidWhence"));
				}
				return position;
			}

	// Set the length of this stream.
	public override void SetLength(long value)
			{
				if(closed || !writable)
				{
					throw new NotSupportedException(_("IO_NotSupp_SetLength"));
				}
				if(value < 0 || value > (Int32.MaxValue - start))
				{
					throw new ArgumentOutOfRangeException
						("value", _("IO_InvalidLength"));
				}
				int len = start + (int)value;
				if(len > capacity)
				{
					IncreaseCapacity(len);
				}
				else if(len > length)
				{
					Array.Clear(buffer, length, len - length);
				}
				length = len;
				if(position > len)
				{
					position = len;
				}
			}

	// Get the contents of the memory stream as an array.
	public virtual byte[] ToArray()
			{
				byte[] array = new byte [length - start];
				if(length > start)
				{
					Array.Copy(buffer, start, array, 0, length - start);
				}
				return array;
			}

	// Write the contents of a buffer to the memory stream.
	public override void Write(byte[] buffer, int offset, int count)
			{
				// Validate the object state and the parameters.
				CheckForWrite();
				ValidateBuffer(buffer, offset, count);
				int newPosition = position + count;
				if(newPosition < 0)
				{
					// Writing these bytes would cause the buffer to
					// exceed Int32.MaxValue in size.
					throw new IOException(_("IO_WriteFailed"));
				}

				// Determine if we need to change the stream's length.
				if(newPosition > length)
				{
					if(newPosition > capacity)
					{
						IncreaseCapacity(newPosition);
					}
					else if(position > length)
					{
						Array.Clear(this.buffer, length, position - length);
					}
					length = newPosition;
				}

				// Copy the data into place and return.
				Array.Copy(buffer, offset, this.buffer, position, count);
				position = newPosition;
			}

	// Write a single byte to the memory stream.
	public override void WriteByte(byte value)
			{
				// Validate the object state.
				CheckForWrite();
				if(position == Int32.MaxValue)
				{
					// Writing this byte would cause the buffer to
					// exceed Int32.MaxValue in size.
					throw new IOException(_("IO_WriteFailed"));
				}

				// Determine if we need to change the stream's length.
				if(position >= length)
				{
					if(position >= capacity)
					{
						IncreaseCapacity(position + 1);
					}
					else if(position > length)
					{
						Array.Clear(buffer, length, position - length);
					}
					length = position + 1;
				}

				// Copy the byte into place and return.
				buffer[position++] = value;
			}

	// Write the entire contents of this memory stream to another stream.
	public virtual void WriteTo(Stream stream)
			{
				CheckForRead();
				if(stream == null)
				{
					throw new ArgumentNullException("stream");
				}
				stream.Write(buffer, start, length - start);
			}

	// Check the available stream behaviours.
	public override bool CanRead
			{
				get
				{
					return !closed;
				}
			}
	public override bool CanSeek
			{
				get
				{
					return !closed;
				}
			}
	public override bool CanWrite
			{
				get
				{
					return (writable && !closed);
				}
			}

	// Get or set the memory stream's capacity.
	public virtual int Capacity
			{
				get
				{
					CheckForRead();
					return capacity - start;
				}
				set
				{
					CheckForRead();

					// Note: there is a design bug here.  We should really
					// check for "value != (capacity - start)", to be
					// consistent with the get method.  However, we need
					// to implement this bug to be compatible with third
					// party implementations of MemoryStream.  It turns out
					// not to matter in practice because memory streams will
					// only be resizable if "start == 0".  We're documenting
					// this design bug here to prevent people from "fixing it".
					if(value != capacity)
					{
						if(!resizable)
						{
							throw new NotSupportedException
								(_("IO_FixedCapacity"));
						}
						if(value < length)
						{
							throw new ArgumentOutOfRangeException
								("value", _("IO_CannotReduceCapacity"));
						}
						if(value > 0)
						{
							// Try to account for "start" being non-zero, just
							// in case we need to handle it in the future.
							byte[] newBuffer = new byte [value];
							if(buffer != null)
							{
								Array.Copy(buffer, start, newBuffer, 0,
										   length - start);
							}
							buffer = newBuffer;
							position -= start;
							length -= start;
							start = 0;
						}
						else
						{
							// Other implementations set the buffer to null
							// when the capacity is reduced to zero.  Provided
							// for backwards compatibility only.
							buffer = null;
							position = 0;
							length = 0;
							start = 0;
						}
						capacity = value;
					}
				}
			}

	// Get the length of this memory stream.
	public override long Length
			{
				get
				{
					CheckForRead();
					return length - start;
				}
			}

	// Get or set the position within this stream.
	public override long Position
			{
				get
				{
					CheckForRead();
					return position - start;
				}
				set
				{
					CheckForRead();
					if(value < 0 || value > Int32.MaxValue)
					{
						throw new ArgumentOutOfRangeException
							("value", _("IO_InvalidSeekPosition"));
					}
					try
					{
						position = checked(start + ((int)value));
					}
					catch(OverflowException)
					{
						throw new ArgumentOutOfRangeException
							("value", _("IO_InvalidSeekPosition"));
					}
				}
			}

}; // class MemoryStream

}; // namespace System.IO
