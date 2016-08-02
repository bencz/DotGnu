/*
 * IsolatedStorageFileStream.cs - Implementation of the
 *		"System.IO.IsolatedStorage.IsolatedStorageFileStream" class.
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

namespace System.IO.IsolatedStorage
{

#if CONFIG_ISOLATED_STORAGE

using System.IO;
using System.Security.Permissions;

public class IsolatedStorageFileStream : FileStream
{
	// Internal state.
	private FileStream realStream;
	private IsolatedStorageFile storeToClose;

	// Constructors.
	public IsolatedStorageFileStream(String path, FileMode mode)
			: this(path, mode, (mode == FileMode.Append
									? FileAccess.Write
									: FileAccess.ReadWrite),
				   FileShare.None, BUFSIZ, null)
			{
				// Nothing to do here.
			}
	public IsolatedStorageFileStream(String path, FileMode mode,
									 IsolatedStorageFile sf)
			: this(path, mode, (mode == FileMode.Append
									? FileAccess.Write
									: FileAccess.ReadWrite),
				   FileShare.None, BUFSIZ, sf)
			{
				// Nothing to do here.
			}
	public IsolatedStorageFileStream(String path, FileMode mode,
									 FileAccess access)
			: this(path, mode, access, FileShare.None, BUFSIZ, null)
			{
				// Nothing to do here.
			}
	public IsolatedStorageFileStream(String path, FileMode mode,
									 FileAccess access,
									 IsolatedStorageFile sf)
			: this(path, mode, access, FileShare.None, BUFSIZ, sf)
			{
				// Nothing to do here.
			}
	public IsolatedStorageFileStream(String path, FileMode mode,
									 FileAccess access, FileShare share)
			: this(path, mode, access, share, BUFSIZ, null)
			{
				// Nothing to do here.
			}
	public IsolatedStorageFileStream(String path, FileMode mode,
									 FileAccess access, FileShare share,
									 IsolatedStorageFile sf)
			: this(path, mode, access, share, BUFSIZ, sf)
			{
				// Nothing to do here.
			}
	public IsolatedStorageFileStream(String path, FileMode mode,
									 FileAccess access, FileShare share,
									 int bufferSize)
			: this(path, mode, access, share, bufferSize, null)
			{
				// Nothing to do here.
			}
	public IsolatedStorageFileStream(String path, FileMode mode,
									 FileAccess access, FileShare share,
									 int bufferSize,
									 IsolatedStorageFile sf)
			: base(path)
			{
				// Validate the parameters.
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}
				if(sf == null)
				{
					sf = IsolatedStorageFile.GetUserStoreForDomain();
					storeToClose = sf;
				}

				// Get the base directory for the isolated storage area.
				String baseDir = sf.BaseDirectory;

#if CONFIG_PERMISSIONS
				// Assert that we have permission to do this.
				(new FileIOPermission
					(FileIOPermissionAccess.AllAccess, baseDir)).Assert();
#endif

				// Open the real stream.
				realStream = new FileStream
					(baseDir + Path.DirectorySeparatorChar + path,
					 mode, access, share, bufferSize, false);
			}

	// Properties.
	public override bool CanRead
			{
				get
				{
					return realStream.CanRead;
				}
			}
	public override bool CanSeek
			{
				get
				{
					return realStream.CanSeek;
				}
			}
	public override bool CanWrite
			{
				get
				{
					return realStream.CanWrite;
				}
			}
	public override IntPtr Handle
			{
				get
				{
					// Cannot get isolated storage file handles.
					throw new IsolatedStorageException
						(_("Exception_IsolatedStorage"));
				}
			}
	public override bool IsAsync
			{
				get
				{
					return realStream.IsAsync;
				}
			}
	public override long Length
			{
				get
				{
					return realStream.Length;
				}
			}
	public override long Position
			{
				get
				{
					return realStream.Position;
				}
				set
				{
					realStream.Position = value;
				}
			}

	// Begin an asynchronous read operation.
	public override IAsyncResult BeginRead
				(byte[] buffer, int offset, int numBytes,
				 AsyncCallback userCallback, Object stateObject)
			{
				return realStream.BeginRead
					(buffer, offset, numBytes, userCallback, stateObject);
			}

	// Begin an asynchronous write operation.
	public override IAsyncResult BeginWrite
				(byte[] buffer, int offset, int numBytes,
				 AsyncCallback userCallback, Object stateObject)
			{
				return realStream.BeginWrite
					(buffer, offset, numBytes, userCallback, stateObject);
			}

	// Close this stream.
	public override void Close()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	// Dispose of this stream.
	protected override void Dispose(bool disposing)
			{
				realStream.Close();
				if(storeToClose != null)
				{
					storeToClose.Close();
				}
				base.Dispose(disposing);
			}

	// End an asynchronous read operation.
	public override int EndRead(IAsyncResult asyncResult)
			{
				return realStream.EndRead(asyncResult);
			}

	// End an asynchronous write operation
	public override void EndWrite(IAsyncResult asyncResult)
			{
				realStream.EndWrite(asyncResult);
			}

	// Flush this stream.
	public override void Flush()
			{
				realStream.Flush();
			}

	// Read from this stream.
	public override int Read(byte[] buffer, int offset, int count)
			{
				return realStream.Read(buffer, offset, count);
			}

	// Read a byte from this stream.
	public override int ReadByte()
			{
				return realStream.ReadByte();
			}

	// Seek within this stream.
	public override long Seek(long offset, SeekOrigin origin)
			{
				return realStream.Seek(offset, origin);
			}

	// Set the length of this stream.
	public override void SetLength(long value)
			{
				realStream.SetLength(value);
			}

	// Write to this stream.
	public override void Write(byte[] buffer, int offset, int count)
			{
				realStream.Write(buffer, offset, count);
			}

	// Write a byte to this stream.
	public override void WriteByte(byte value)
			{
				realStream.WriteByte(value);
			}

}; // class IsolatedStorageFileStream

#endif // CONFIG_ISOLATED_STORAGE

}; // namespace System.IO.IsolatedStorage
