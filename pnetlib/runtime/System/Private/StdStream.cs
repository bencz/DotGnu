/*
 * StdStream.cs - Implementation of the "System.Private.StdStream" class.
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

namespace System.Private
{

#if !CONFIG_SMALL_CONSOLE

using System;
using System.IO;
using Platform;

// This class differs from "StdReader" and "StdWriter"
// in that it manipulates binary data, not textual.
// It is provided to support "Console.OpenStandard*".

internal sealed class StdStream : Stream
{
	// Local state.
	private int fd;

	// Constructor.
	public StdStream(int fd)
			{
				this.fd = fd;
			}

	// Close this stream.
	public override void Close()
			{
				if(fd != -1)
				{
					Stdio.StdClose(fd);
					fd = -1;
				}
			}

	// Flush this stream.
	public override void Flush()
			{
				if(fd != -1)
				{
					Stdio.StdFlush(fd);
				}
			}

	// Read data from this stream.
	public override int Read(byte[] buffer, int offset, int count)
			{
				ValidateBuffer(buffer, offset, count);
				if(fd == 0)
				{
					return Stdio.StdRead(fd, buffer, offset, count);
				}
				else if(fd != -1)
				{
					throw new NotSupportedException
						(_("IO_NotSupp_Read"));
				}
				else
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
			}

	// Seek to a new position within this stream.
	public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException
					(_("IO_NotSupp_Seek"));
			}

	// Set the length of this stream.
	public override void SetLength(long value)
			{
				throw new NotSupportedException
					(_("IO_NotSupp_SetLength"));
			}

	// Write data to this stream.
	public override void Write(byte[] buffer, int offset, int count)
			{
				ValidateBuffer(buffer, offset, count);
				if(fd == 1 || fd == 2)
				{
					Stdio.StdWrite(fd, buffer, offset, count);
				}
				else if(fd != -1)
				{
					throw new NotSupportedException
						(_("IO_NotSupp_Write"));
				}
				else
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
			}

	// Stream properties.
	public override bool CanRead
			{
				get
				{
					return (fd == 0);
				}
			}
	public override bool CanSeek
			{
				get
				{
					return false;
				}
			}
	public override bool CanWrite
			{
				get
				{
					return (fd == 1 || fd == 2);
				}
			}
	public override long Length
			{
				get
				{
					throw new NotSupportedException
						(_("IO_NotSupp_Seek"));
				}
			}
	public override long Position
			{
				get
				{
					if(fd != -1)
					{
						throw new NotSupportedException
							(_("IO_NotSupp_Seek"));
					}
					else
					{
						throw new ObjectDisposedException(_("IO_StreamClosed"));
					}
				}
				set
				{
					if(fd != -1)
					{
						throw new NotSupportedException
							(_("IO_NotSupp_Seek"));
					}
					else
					{
						throw new ObjectDisposedException(_("IO_StreamClosed"));
					}
				}
			}

}; // class StdStream

#endif // !CONFIG_SMALL_CONSOLE

}; // namespace System.Private
