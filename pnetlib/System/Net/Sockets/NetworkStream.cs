/*
 * NetworkStream.cs - Implementation of the
 *			"System.Net.Sockets.NetworkStream" class.
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

namespace System.Net.Sockets
{

using System;
using System.IO;
using System.Threading;

public class NetworkStream : Stream
{
	// Internal state.
	private Socket socket;
	private FileAccess access;
	private bool ownsSocket;
	private Object readLock;

	// Constructors.
	public NetworkStream(Socket socket)
			: this(socket, FileAccess.ReadWrite, false)
			{
				// Nothing to do here.
			}
	public NetworkStream(Socket socket, bool ownsSocket)
			: this(socket, FileAccess.ReadWrite, ownsSocket)
			{
				// Nothing to do here.
			}
	public NetworkStream(Socket socket, FileAccess access)
			: this(socket, access, false)
			{
				// Nothing to do here.
			}
	public NetworkStream(Socket socket, FileAccess access, bool ownsSocket)
			{
				// Validate the parameters.
				if(socket == null)
				{
					throw new ArgumentNullException("socket");
				}
				if(!(socket.Blocking))
				{
					throw new IOException(S._("IO_SocketNotBlocking"));
				}
				if(!(socket.Connected))
				{
					throw new IOException(S._("IO_SocketNotConnected"));
				}
				if(socket.SocketType != SocketType.Stream)
				{
					throw new IOException(S._("IO_SocketIncorrectType"));
				}

				// Initialize the internal state.
				this.socket = socket;
				this.access = access;
				this.ownsSocket = ownsSocket;
				this.readLock = new Object();
			}

	// Destructor.
	~NetworkStream()
			{
				Dispose(false);
			}

	// Handle asynchronous operations by passing them back to the base class.
	// Note: we deliberately don't use the "BeginReceive" and "BeginSend"
	// methods on "Socket", because those methods will bypass the locking and
	// integrity checks that "NetworkStream" imposes.
	public override IAsyncResult BeginRead
				(byte[] buffer, int offset, int count,
				 AsyncCallback callback, Object state)
			{
				return base.BeginRead(buffer, offset, count, callback, state);
			}
	public override int EndRead(IAsyncResult asyncResult)
			{
				return base.EndRead(asyncResult);
			}
	public override IAsyncResult BeginWrite
				(byte[] buffer, int offset, int count,
				 AsyncCallback callback, Object state)
			{
				return base.BeginWrite(buffer, offset, count, callback, state);
			}
	public override void EndWrite(IAsyncResult asyncResult)
			{
				base.EndWrite(asyncResult);
			}

	// Close this stream.
	public override void Close()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	// Dispose of this stream.
	protected virtual void Dispose(bool disposing)
			{
				lock(this)
				{
					if(socket != null)
					{
						if(ownsSocket)
						{
							socket.Close();
						}
						socket = null;
					}
				}
			}

	// Flush this stream.
	public override void Flush()
			{
				// Nothing to do here.
			}

	// Read data from this stream.
	public override int Read(byte[] buffer, int offset, int count)
			{
				lock(readLock)
				{
					// Validate the stream state first.
					if(socket == null)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					else if((access & FileAccess.Read) == 0)
					{
						throw new NotSupportedException
							(S._("IO_NotSupp_Read"));
					}

					// Read directly from the socket.
					return socket.Receive
						(buffer, offset, count, SocketFlags.None);
				}
			}

	// Seek to a new location in the stream.
	public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException(S._("IO_NotSupp_Seek"));
			}

	// Set the length of this stream.
	public override void SetLength(long length)
			{
				throw new NotSupportedException(S._("IO_NotSupp_SetLength"));
			}

	// Write data to this stream.
	public override void Write(byte[] buffer, int offset, int count)
			{
				lock(this)
				{
					// Validate the stream state first.
					if(socket == null)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					else if((access & FileAccess.Write) == 0)
					{
						throw new NotSupportedException
							(S._("IO_NotSupp_Write"));
					}

					// Write directly to the socket.
					socket.Send(buffer, offset, count, SocketFlags.None);
				}
			}

	// Determine if we can read from this stream.
	public override bool CanRead
			{
				get
				{
					return ((access & FileAccess.Read) != 0);
				}
			}

	// Determine if we can seek within this stream.
	public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

	// Determine if we can write to this stream.
	public override bool CanWrite
			{
				get
				{
					return ((access & FileAccess.Write) != 0);
				}
			}

#if !ECMA_COMPAT

	// Get or set the readable state for this stream.
	protected bool Readable
			{
				get
				{
					return ((access & FileAccess.Read) != 0);
				}
				set
				{
					if(value)
					{
						access |= FileAccess.Read;
					}
					else
					{
						access &= ~(FileAccess.Read);
					}
				}
			}

	// Get or set the writable state for this stream.
	protected bool Writeable
			{
				get
				{
					return ((access & FileAccess.Write) != 0);
				}
				set
				{
					if(value)
					{
						access |= FileAccess.Write;
					}
					else
					{
						access &= ~(FileAccess.Write);
					}
				}
			}

	// Get the underlying socket.
	protected Socket Socket
			{
				get
				{
					return socket;
				}
			}

#endif // !ECMA_COMPAT

	// Determine if the underlying socket has data available.
	public virtual bool DataAvailable
			{
				get
				{
					lock(readLock)
					{
						if(socket == null)
						{
							throw new ObjectDisposedException
								(S._("Exception_Disposed"));
						}
						else
						{
							return (socket.Available > 0);
						}
					}
				}
			}

	// Get the length of this stream.
	public override long Length
			{
				get
				{
					throw new NotSupportedException(S._("IO_NotSupp_Seek"));
				}
			}

	// Get or set the position within this stream.
	public override long Position
			{
				get
				{
					throw new NotSupportedException(S._("IO_NotSupp_Seek"));
				}
				set
				{
					throw new NotSupportedException(S._("IO_NotSupp_Seek"));
				}
			}

}; // class NetworkStream

}; // namespace System.Net.Sockets
