/*
 * FileStream.cs - Implementation of the "System.IO.FileStream" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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
using System.Security;
using System.Threading;
using Platform;

public class FileStream : Stream
{
	// Default buffer size used for files.
	internal const int BUFSIZ = 4096;

	// Invalid handle value.
	private static readonly IntPtr invalidHandle =
		FileMethods.GetInvalidHandle();

	// Internal state.
	private IntPtr handle;
	private FileAccess access;
	private bool ownsHandle;
	private bool isAsync;
	private String path;

	// Buffer information.
	private int bufferSize;
	private byte[] buffer;
	private int bufferPosn;
	private int bufferLen;
	private long position;
	private bool bufferOwnedByWrite;
	private bool canSeek;

	// Constructor.
	public FileStream(String path, FileMode mode)
			: this(path, mode, FileAccess.ReadWrite,
				   FileShare.Read, BUFSIZ, false)
			{
				// Nothing to do here.
			}
	public FileStream(String path, FileMode mode, FileAccess access)
			: this(path, mode, access, FileShare.Read, BUFSIZ, false)
			{
				// Nothing to do here.
			}
	public FileStream(String path, FileMode mode,
					  FileAccess access, FileShare share)
			: this(path, mode, access, share, BUFSIZ, false)
			{
				// Nothing to do here.
			}
	public FileStream(String path, FileMode mode,
					  FileAccess access, FileShare share,
					  int bufferSize)
			: this(path, mode, access, share, bufferSize, false)
			{
				// Nothing to do here.
			}
	public FileStream(String path, FileMode mode,
					  FileAccess access, FileShare share,
					  int bufferSize, bool useAsync)
			{
				// Validate the parameters.
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}
				if(!FileMethods.ValidatePathname(path))
				{
					throw new ArgumentException(_("IO_InvalidPathname"));
				}
				if(bufferSize <= 0)
				{
					throw new ArgumentOutOfRangeException
						("bufferSize", _("ArgRange_BufferSize"));
				}
				if(access < FileAccess.Read ||
				   access > FileAccess.ReadWrite)
				{
					throw new ArgumentOutOfRangeException
						("access", _("IO_FileAccess"));
				}
				if(mode < FileMode.CreateNew ||
				   mode > FileMode.Append)
				{
					throw new ArgumentOutOfRangeException
						("mode", _("IO_FileMode"));
				}
			#if ECMA_COMPAT
				if((((int)share) & ~0x03) != 0)
			#else
				if((((int)share) & ~0x13) != 0)
			#endif
				{
					throw new ArgumentOutOfRangeException
						("share", _("IO_FileShare"));
				}

				// Attempt to open the file.
				if(!FileMethods.Open(path, mode, access, share, out handle))
				{
					Errno errno = FileMethods.GetErrno();
					if(errno == Errno.ENOENT)
					{
						//
						// Under UNIX ENOENT is returned if the
						// directory the file lives in doesn't exist.
						// ECMA requires DirectoryNotFountException
						// in that case.
						//
						String dirname = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path));
						if (!System.IO.Directory.Exists(dirname))
							throw new DirectoryNotFoundException();
						throw new FileNotFoundException(null, path);
					}
					else if(errno == Errno.ENOTDIR)
					{
						throw new DirectoryNotFoundException();
					}
					else if(errno == Errno.ENAMETOOLONG)
					{
						throw new PathTooLongException();
					}
					else if(errno == Errno.EACCES)
					{
						throw new UnauthorizedAccessException();
					}
					else
					{
						throw new IOException(errno);
					}
				}

				// Initialize the object state.
				this.access = access;
				this.ownsHandle = true;
				this.isAsync = useAsync;
				this.path = path;
				this.bufferSize = bufferSize;
				this.buffer = new byte [bufferSize];
				this.bufferPosn = 0;
				this.bufferLen = 0;
				this.bufferOwnedByWrite = false;
				this.canSeek = FileMethods.CanSeek(handle);
				this.position = 0;
			}

	// Non-ECMA constructors.
#if !ECMA_COMPAT
	public FileStream(IntPtr handle, FileAccess access)
			: this(handle, access, true, BUFSIZ, false)
			{
				// Nothing to do here.
			}
	public FileStream(IntPtr handle, FileAccess access, bool ownsHandle)
			: this(handle, access, ownsHandle, BUFSIZ, false)
			{
				// Nothing to do here.
			}
	public FileStream(IntPtr handle, FileAccess access,
					  bool ownsHandle, int bufferSize)
			: this(handle, access, ownsHandle, bufferSize, false)
			{
				// Nothing to do here.
			}
	public FileStream(IntPtr handle, FileAccess access,
					  bool ownsHandle, int bufferSize,
					  bool isAsync)
			{
				// Validate the parameters.
				if(bufferSize <= 0)
				{
					throw new ArgumentOutOfRangeException
						("bufferSize", _("ArgRange_BufferSize"));
				}
				if(access < FileAccess.Read ||
				   access > FileAccess.ReadWrite)
				{
					throw new ArgumentOutOfRangeException
						("access", _("IO_FileAccess"));
				}
			#if false
				if(!FileMethods.CheckHandleAccess(handle, access))
				{
					throw new UnauthorizedAccessException
						(_("IO_IncorrectAccess"));
				}
			#endif

				// Initialize the object state.
				this.handle = handle;
				this.access = access;
				this.ownsHandle = ownsHandle;
				this.isAsync = isAsync;
				this.bufferSize = bufferSize;
				this.buffer = new byte [bufferSize];
				this.bufferPosn = 0;
				this.bufferLen = 0;
				this.bufferOwnedByWrite = false;
				this.canSeek = FileMethods.CanSeek(handle);
				if(canSeek)
				{
					this.position = FileMethods.Seek
						(handle, 0, SeekOrigin.Current);
				}
				else
				{
					this.position = 0;
				}
			}

	// Internal constructor used by IsolatedStorageFileStream.
	internal FileStream(String path)
			{
				this.path = path;
			}
#endif

	// Destructor.
	~FileStream()
			{
				Dispose(false);
			}

	// Asynchronous operations - let the base class do the work.
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

	// Flush read data from the buffer.
	private void FlushReadBuffer()
			{
				if(canSeek)
				{
					if(bufferPosn < bufferLen)
					{
						FileMethods.Seek(handle, bufferPosn - bufferLen,
														SeekOrigin.Current);
					}
					bufferPosn = 0;
					bufferLen = 0;
				}
			}

	// Flush any buffered write data to the file.
	private void FlushWriteBuffer()
			{
				if(bufferPosn > 0)
				{
					if(!FileMethods.Write(handle, buffer, 0, bufferPosn))
					{
						throw new IOException
							(FileMethods.GetErrno(), _("IO_WriteFailed"));
					}
					bufferPosn = 0;
				}
				bufferLen = 0;
			}

	// Set up for a read.
	private void SetupRead()
			{
				if((access & FileAccess.Read) == 0)
				{
					throw new NotSupportedException
						(_("IO_NotSupp_Read"));
				}
				if(handle == invalidHandle)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				if(bufferOwnedByWrite)
				{
					FlushWriteBuffer();
					bufferOwnedByWrite = false;
				}
			}

	// Set up for a write.
	private void SetupWrite()
			{
				if((access & FileAccess.Write) == 0)
				{
					throw new NotSupportedException(_("IO_NotSupp_Write"));
				}
				if(handle == invalidHandle)
				{
					throw new ObjectDisposedException(_("IO_StreamClosed"));
				}
				if(!bufferOwnedByWrite)
				{
					FlushReadBuffer();
					FileMethods.Seek(handle, position, SeekOrigin.Begin);
					bufferOwnedByWrite = true;
				}
			}

	// Close the stream.
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
					if(handle != invalidHandle)
					{
						if(bufferOwnedByWrite)
						{
							FlushWriteBuffer();
						}
						if(ownsHandle)
						{
							FileMethods.Close(handle);
						}
						handle = invalidHandle;
					}
				}
			}

	// Flush the pending contents in this stream.
	public override void Flush()
			{
				lock(this)
				{
					if(handle != invalidHandle)
					{
						if(bufferOwnedByWrite)
						{
							FlushWriteBuffer();
							if(!FileMethods.FlushWrite(handle))
							{
								throw new IOException
									(FileMethods.GetErrno(),
									_("IO_FlushFailed"));
							}
						}
						else
						{
							FlushReadBuffer();
						}
					}
					else
					{
						throw new ObjectDisposedException
							(_("IO_StreamClosed"));
					}
				}
			}

	// Read data from this stream.
	public override int Read(byte[] buffer, int offset, int count)
			{
				int readLen = 0;
				int tempLen;

				// Validate the parameters and setup the object for reading.
				ValidateBuffer(buffer, offset, count);

				// Lock down the file stream while we do this.
				lock(this)
				{
					// Set up for the read operation.
					SetupRead();

					// Read data into the caller's buffer.
					while(count > 0)
					{
						// How much data do we have available in the buffer?
						tempLen = bufferLen - bufferPosn;
						if(tempLen <= 0)
						{
							bufferPosn = 0;
							bufferLen = FileMethods.Read
								(handle, this.buffer, 0, bufferSize);
							if(bufferLen < 0)
							{
								bufferLen = 0;
								throw new IOException
									(FileMethods.GetErrno(),
									 _("IO_ReadFailed"));
							}
							else if(bufferLen == 0)
							{
								break;
							}
							else
							{
								tempLen = bufferLen;
							}
						}

						// Don't read more than the caller wants.
						if(tempLen > count)
						{
							tempLen = count;
						}

						// Copy stream data to the caller's buffer.
						Array.Copy(this.buffer, bufferPosn,
								   buffer, offset, tempLen);

						// Advance to the next buffer positions.
						readLen += tempLen;
						offset += tempLen;
						count -= tempLen;
						bufferPosn += tempLen;
						position += tempLen;
					}
				}

				// Return the number of bytes that were read to the caller.
				return readLen;
			}

	// Read a single byte from this stream.
	public override int ReadByte()
			{
				// Lock down the file stream while we do this.
				lock(this)
				{
					// Setup the object for reading.
					SetupRead();

					// Read more data into the internal buffer if necessary.
					if(bufferPosn >= bufferLen)
					{
						bufferPosn = 0;
						bufferLen = FileMethods.Read
							(handle, buffer, 0, bufferSize);
						if(bufferLen < 0)
						{
							bufferLen = 0;
							throw new IOException
								(FileMethods.GetErrno(), _("IO_ReadFailed"));
						}
						else if(bufferLen == 0)
						{
							// We've reached EOF.
							return -1;
						}
					}

					// Extract the next byte from the buffer.
					++position;
					return buffer[bufferPosn++];
				}
			}

	// Seek to a new position within this stream.
	public override long Seek(long offset, SeekOrigin origin)
			{
				long newPosn;

				// Bail out if this stream is not capable of seeking.
				if(!canSeek)
				{
					throw new NotSupportedException(_("IO_NotSupp_Seek"));
				}

				// Lock down the file stream while we do this.
				lock(this)
				{
					// Bail out if the handle is invalid.
					if(handle == invalidHandle)
					{
						throw new ObjectDisposedException
							(_("IO_StreamClosed"));
					}

					// Don't do anything if the position won't be moving.
					if(origin == SeekOrigin.Begin && offset == position)
					{
						return offset;
					}
					else if(origin == SeekOrigin.Current && offset == 0)
					{
						return position;
					}

					// The behaviour depends upon the read/write mode.
					if(bufferOwnedByWrite)
					{
						// Flush the write buffer and then seek.
						FlushWriteBuffer();
						newPosn = FileMethods.Seek(handle, offset, origin);
						if(newPosn == -1)
						{
							throw new EndOfStreamException
								(_("IO_EndOfStream"));
						}
						position = newPosn;
					}
					else
					{
						// Determine if the seek is to somewhere inside
						// the current read buffer bounds.
						if(origin == SeekOrigin.Begin)
						{
							newPosn = position - bufferPosn;
							if(offset >= newPosn && offset <
									(newPosn + bufferLen))
							{
								bufferPosn = (int)(offset - newPosn);
								position = offset;
								return position;
							}
						}
						else if(origin == SeekOrigin.Current)
						{
							newPosn = position + offset;
							if(newPosn >= (position - bufferPosn) &&
							   newPosn < (position - bufferPosn + bufferLen))
							{
								bufferPosn =
									(int)(newPosn - (position - bufferPosn));
								position = newPosn;
								return position;
							}
						}

						// Abandon the read buffer.
						bufferPosn = 0;
						bufferLen = 0;

						// Seek to the new position.
						newPosn = FileMethods.Seek(handle, offset, origin);
						if(newPosn == -1)
						{
							throw new EndOfStreamException
								(_("IO_EndOfStream"));
						}
						position = newPosn;
					}
					return position;
				}
			}

	// Set the length of this stream.
	public override void SetLength(long value)
			{
				// Validate the parameters and setup the object for writing.
				if(value < 0)
				{
					throw new ArgumentOutOfRangeException
						("value", _("ArgRange_NonNegative"));
				}
				if(!canSeek)
				{
					throw new NotSupportedException(_("IO_NotSupp_Seek"));
				}

				// Lock down the file stream while we do this.
				lock(this)
				{
					// Setup this object for writing.
					SetupWrite();

					// Call the underlying platform's "SetLength" method.
					if(!FileMethods.SetLength(handle, value))
					{
						throw new IOException
							(FileMethods.GetErrno(), _("IO_SetLengthFailed"));
					}
				}
			}

	// Write a buffer of bytes to this stream.
	public override void Write(byte[] buffer, int offset, int count)
			{
				int tempLen;

				// Validate the parameters and setup the object for writing.
				ValidateBuffer(buffer, offset, count);

				// Lock down the file stream while we do this.
				lock(this)
				{
					// Setup this object for writing.
					SetupWrite();

					// Write data to the file stream.
					while(count > 0)
					{
						// Determine how many bytes we can write to the buffer.
						tempLen = bufferSize - bufferPosn;
						if(tempLen <= 0)
						{
							// Flush the current buffer contents.
							if(!FileMethods.Write
									(handle, this.buffer, 0, bufferPosn))
							{
								throw new IOException
									(FileMethods.GetErrno(),
									 _("IO_WriteFailed"));
							}
							bufferPosn = 0;
							tempLen = bufferSize;
						}
						if(tempLen > count)
						{
							tempLen = count;
						}

						// Can we short-cut the internal buffer?
						if(bufferPosn == 0 && tempLen == bufferSize)
						{
							// Yes: write the data directly to the file.
							if(!FileMethods.Write
									(handle, buffer, offset, tempLen))
							{
								throw new IOException
									(FileMethods.GetErrno(),
									 _("IO_WriteFailed"));
							}
						}
						else
						{
							// No: copy the data to the write buffer first.
							Array.Copy(buffer, offset, this.buffer,
									   bufferPosn, tempLen);
							bufferPosn += tempLen;
						}

						// Advance the buffer and stream positions.
						position += tempLen;
						offset += tempLen;
						count -= tempLen;
					}

					// If the buffer is full, then do a speculative flush now,
					// rather than waiting for the next call to this method.
					if(bufferPosn >= bufferSize)
					{
						if(!FileMethods.Write
							(handle, this.buffer, 0, bufferPosn))
						{
							throw new IOException
								(FileMethods.GetErrno(), _("IO_WriteFailed"));
						}
						bufferPosn = 0;
					}
				}
			}

	// Write a single byte to this stream.
	public override void WriteByte(byte value)
			{
				// Lock down the file stream while we do this.
				lock(this)
				{
					// Setup the object for writing.
					SetupWrite();

					// Flush the current buffer if it is full.
					if(bufferPosn >= bufferSize)
					{
						if(!FileMethods.Write
								(handle, this.buffer, 0, bufferPosn))
						{
							throw new IOException
								(FileMethods.GetErrno(), _("IO_WriteFailed"));
						}
						bufferPosn = 0;
					}

					// Write the byte into the buffer and advance the posn.
					buffer[bufferPosn++] = value;
					++position;
				}
			}

	// Determine if it is possible to read from this stream.
	public override bool CanRead
			{
				get
				{
					return ((access & FileAccess.Read) != 0);
				}
			}

	// Determine if it is possible to seek within this stream.
	public override bool CanSeek
			{
				get
				{
					return canSeek;
				}
			}

	// Determine if it is possible to write to this stream.
	public override bool CanWrite
			{
				get
				{
					return ((access & FileAccess.Write) != 0);
				}
			}

	// Get the length of this stream.
	public override long Length
			{
				get
				{
					// Validate that the object can actually do this.
					if(!canSeek)
					{
						throw new NotSupportedException (_("IO_NotSupp_Seek"));
					}

					// Lock down the file stream while we do this.
					lock(this)
					{
						if(handle == invalidHandle)
						{
							// ECMA says this should be IOException even though
							// everywhere else uses ObjectDisposedException.
							throw new IOException
								(FileMethods.GetErrno(), _("IO_StreamClosed"));
						}

						// Flush the write buffer, because it may
						// affect the length of the stream.
						if(bufferOwnedByWrite)
						{
							FlushWriteBuffer();
						}

						// Seek to the end to get the stream length.
						long posn = FileMethods.Seek
							(handle, 0, SeekOrigin.End);

						// Seek back to where we used to be.
						if(bufferOwnedByWrite)
						{
							FileMethods.Seek
								(handle, position - bufferPosn,
								 SeekOrigin.Begin);
						}
						else
						{
							FileMethods.Seek
								(handle, position - bufferPosn + bufferLen,
								 SeekOrigin.Begin);
						}
	
						// Decode the result.
						if(posn == -1)
						{
							throw new IOException
								(FileMethods.GetErrno(), _("IO_SeekFailed"));
						}
						return posn;
					}
				}
			}

	// Get the current position within the stream.
	public override long Position
			{
				get
				{
					if(!canSeek)
					{
						throw new NotSupportedException(_("IO_NotSupp_Seek"));
					}
					return position;
				}
				set
				{
					Seek(value, SeekOrigin.Begin);
				}
			}

	// Determine if this file stream was created in asynchronous mode.
	public virtual bool IsAsync
			{
				get
				{
					return isAsync;
				}
			}

#if !ECMA_COMPAT

	// Get the name of the file underlying this stream object.
	public String Name
			{
				get
				{
					if(path != null)
					{
						return path;
					}
					else
					{
						return _("IO_UnknownFile");
					}
				}
			}

	// Lock a region of the file stream.
	public virtual void Lock(long position, long length)
			{
				if(position < 0)
				{
					throw new ArgumentOutOfRangeException
						("position", _("ArgRange_NonNegative"));
				}
				if(length < 0)
				{
					throw new ArgumentOutOfRangeException
						("position", _("ArgRange_NonNegative"));
				}
				lock(this)
				{
					if(handle == invalidHandle)
					{
						throw new ObjectDisposedException(_("IO_StreamClosed"));
					}
					if(!FileMethods.Lock(handle, position, length))
					{
						throw new IOException
							(FileMethods.GetErrno(), _("IO_LockFailed"));
					}
				}
			}

	// Unlock a region of the file stream.
	public virtual void Unlock(long position, long length)
			{
				if(position < 0)
				{
					throw new ArgumentOutOfRangeException
						("position", _("ArgRange_NonNegative"));
				}
				if(length < 0)
				{
					throw new ArgumentOutOfRangeException
						("position", _("ArgRange_NonNegative"));
				}
				lock(this)
				{
					if(handle == invalidHandle)
					{
						throw new ObjectDisposedException(_("IO_StreamClosed"));
					}
					if(!FileMethods.Unlock(handle, position, length))
					{
						throw new IOException
							(FileMethods.GetErrno(), _("IO_UnlockFailed"));
					}
				}
			}

	// Get the underlying file stream handle.
	public virtual IntPtr Handle
			{
				get
				{
					Flush();
					return handle;
				}
			}

#endif // !ECMA_COMPAT

}; // class FileStream

}; // namespace System.IO
