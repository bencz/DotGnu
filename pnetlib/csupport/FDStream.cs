/*
 * FDStream.cs - A stream that is wrapped with a native file descriptor.
 *
 * This file is part of the Portable.NET "C language support" library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace OpenSystem.C
{

using System;
using System.IO;

public class FDStream : Stream, IFDOperations
{
	// Internal state.
	private int fd;
	private Stream stream;

	// Constructor.
	public FDStream(int fd, Stream stream)
			{
				this.fd = fd;
				this.stream = stream;
			}

	// Implement the FDStream interface.
	public bool NonBlocking
			{
				get
				{
					return false;
				}
				set
				{
					throw new NotSupportedException();
				}
			}
	public int NativeFd
			{
				get
				{
					return fd;
				}
			}
	public int SelectFd
			{
				get
				{
					if(Environment.OSVersion.Platform != PlatformID.Unix)
					{
						// Win32-style system: cannot select.
						return -1;
					}
					else
					{
						// Unix-style system: select with underlying fd.
						return fd;
					}
				}
			}

	// Implement pass throughs for "Stream" methods.
	public override void Close()
			{
				stream.Close();
			}
	public override void Flush()
			{
				stream.Flush();
			}
	public override int Read(byte[] buffer, int offset, int count)
			{
				return stream.Read(buffer, offset, count);
			}
	public override int ReadByte()
			{
				return stream.ReadByte();
			}
	public override long Seek(long offset, SeekOrigin origin)
			{
				return stream.Seek(offset, origin);
			}
	public override void SetLength(long value)
			{
				stream.SetLength(value);
			}
	public override void Write(byte[] buffer, int offset, int count)
			{
				stream.Write(buffer, offset, count);
			}
	public override void WriteByte(byte value)
			{
				stream.WriteByte(value);
			}
	public override bool CanRead
			{
				get
				{
					return stream.CanRead;
				}
			}
	public override bool CanSeek
			{
				get
				{
					return stream.CanSeek;
				}
			}
	public override bool CanWrite
			{
				get
				{
					return stream.CanWrite;
				}
			}
	public override long Length
			{
				get
				{
					return stream.Length;
				}
			}
	public override long Position
			{
				get
				{
					return stream.Position;
				}
				set
				{
					stream.Position = value;
				}
			}

} // class FDStream

} // namespace OpenSystem.C
