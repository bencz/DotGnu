/*
 * ClrResourceStream.cs - Implementation of the
 *			"System.Reflection.ClrResourceStream" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection
{

using System;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;

// This class helps with reading from manifest resources.

internal sealed class ClrResourceStream : Stream
{
	// Internal state.
	private IntPtr handle;
	private long start;
	private long length;
	private long position;

	// Constructor.
	public ClrResourceStream(IntPtr handle, long start, long length)
			: base()
			{
				this.handle = handle;
				this.start  = start;
				this.length = length;
				this.position = 0;
			}

	// This class does not support asynchronous file operations.
	public override IAsyncResult BeginRead
				(byte[] buffer, int offset, int count,
				 AsyncCallback callback, Object state)
			{
				throw new NotSupportedException(_("IO_NotSupp_Async"));
			}
	public override int EndRead(IAsyncResult asyncResult)
			{
				throw new NotSupportedException(_("IO_NotSupp_Async"));
			}
	public override IAsyncResult BeginWrite
				(byte[] buffer, int offset, int count,
				 AsyncCallback callback, Object state)
			{
				throw new NotSupportedException(_("IO_NotSupp_Async"));
			}
	public override void EndWrite(IAsyncResult asyncResult)
			{
				throw new NotSupportedException(_("IO_NotSupp_Async"));
			}

	// Set up for a read.
	// removed for speedup
	/*
	private void SetupRead()
			{
				if(handle == IntPtr.Zero)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
			}
			*/

	// Close the stream.
	public override void Close()
			{
				lock(this)
				{
					handle = IntPtr.Zero;
				}
			}

	// Create a wait handle for asynchronous operations.
	protected override WaitHandle CreateWaitHandle()
			{
				throw new NotSupportedException(_("IO_NotSupp_Async"));
			}

	// Flush the pending contents in this stream.
	public override void Flush()
			{
				if(handle == IntPtr.Zero)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
			}

	// Read data from this stream.
	public override int Read(byte[] buffer, int offset, int count)
			{
				int result;

				// Validate the parameters and setup the object for reading.
				ValidateBuffer(buffer, offset, count);
				if(((long)count) > (length - position))
				{
					count = (int)(length - position);
				}
				// SetupRead();
				if(handle == IntPtr.Zero)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				

				// Read data into the caller's buffer.
				result = ResourceRead(handle, start + position,
									  buffer, offset, count);
				if(result > 0)
				{
					position += result;
				}

				// Return the number of bytes that were read to the caller.
				return result;
			}

	// Read a single byte from this stream.
	public override int ReadByte()
			{
				int byteval;

				// Setup the object for reading.
				// SetupRead();
				if(handle == IntPtr.Zero)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}

				// Bail out if we are already at the end of the stream.
				if(position >= length)
				{
					return -1;
				}

				// Read a single byte from the resource.
				byteval = ResourceReadByte(handle, start + position);
				if(byteval != -1)
				{
					++position;
				}
				return byteval;
			}

	// Seek to a new position within this stream.
	public override long Seek(long offset, SeekOrigin origin)
			{
				// Bail out if the handle is invalid.
				if(handle == IntPtr.Zero)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}

				// Move the position based on the origin type.
				if(origin == SeekOrigin.Begin)
				{
					position = offset;
				}
				else if(origin == SeekOrigin.Current)
				{
					position += offset;
				}
				else if(origin == SeekOrigin.End)
				{
					position = length + offset;
				}

				// Clamp the position to the extents of the resource.
				if(position < 0)
				{
					position = 0;
				}
				else if(position > length)
				{
					position = length;
				}

				// Return the new position.
				return position;
			}

	// Set the length of this stream.
	public override void SetLength(long value)
			{
				throw new NotSupportedException(_("IO_NotSupp_SetLength"));
			}

	// Write a buffer of bytes to this stream.
	public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException(_("IO_NotSupp_Write"));
			}

	// Write a single byte to this stream.
	public override void WriteByte(byte value)
			{
				throw new NotSupportedException(_("IO_NotSupp_Write"));
			}

	// Get the raw address of a specific position in this stream.
	public unsafe byte *GetAddress(long position)
			{
				return ResourceGetAddress(handle, start + position);
			}

	// Determine if we can read from this stream.
	public override bool CanRead
			{
				get
				{
					return true;
				}
			}

	// Determine if we can write to this stream.
	public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

	// Determine if we can seek within this stream.
	public override bool CanSeek
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
					return length;
				}
			}

	// Get the current position within the stream.
	public override long Position
			{
				get
				{
					return position;
				}
				set
				{
					Seek(value, SeekOrigin.Begin);
				}
			}

	// Read a buffer of bytes from a manifest resource.  The engine can
	// assume that the position and count are valid for the resource.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int ResourceRead
		(IntPtr handle, long position, byte[] buffer, int offset, int count);

	// Read a single byte from a manifest resource.  The engine can assume
	// that the position is valid for the resource, with an implied count of 1.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int ResourceReadByte(IntPtr handle, long position);

	// Get the raw address of a position within a manifest resource.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private unsafe static byte *ResourceGetAddress
				(IntPtr handle, long position);

}; // class ClrResourceStream

}; // namespace System.Reflection
