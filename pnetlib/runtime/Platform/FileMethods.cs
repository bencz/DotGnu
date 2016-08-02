/*
 * FileMethods.cs - Implementation of the "Platform.FileMethods" class.
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

namespace Platform
{

using System;
using System.IO;
using System.Runtime.CompilerServices;

internal class FileMethods
{

	// Get the value that represents an invalid file handle.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr GetInvalidHandle();

	// Validate a pathname to ensure that it doesn't contain
	// characters that are illegal for this platform's filesystem.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool ValidatePathname(String path);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static FileType GetFileType(String path);

	// Open a raw binary file.  Returns false if the file
	// could not be opened.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool Open(String path, FileMode mode,
								   FileAccess access, FileShare share,
								   out IntPtr handle);

	// Determine if this platform has support for asynchronous
	// file handle operations.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool HasAsync();

	// Determine if it is possible to seek on a file handle.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool CanSeek(IntPtr handle);

	// Check that a pre-existing file handle was opened
	// with a specified access.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool CheckHandleAccess
			(IntPtr handle, FileAccess access);

	// Seek to a particular position on a file handle.
	// Returns -1 if the offset is out of range, or an
	// error occurred.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static long Seek(IntPtr handle, long offset,
								   SeekOrigin origin);

	// Write the contents of a buffer to a file handle.
	// Returns false if an I/O error of some kind occurred.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool Write(IntPtr handle, byte[] buffer,
									int offset, int count);

	// Read data from a file handle into a supplied buffer.
	// Returns -1 if an I/O error of some kind occurred,
	// 0 at EOF, or the number of bytes read otherwise.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int Read(IntPtr handle, byte[] buffer,
								  int offset, int count);

	// Close a file handle.  Returns false if some kind of
	// I/O error occurred.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool Close(IntPtr handle);

	// Flush all data that was previously written, in response
	// to a user-level "Flush" request.  Normally this will do
	// nothing unless the platform provides its own buffering.
	// Returns false if an I/O error occurred.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool FlushWrite(IntPtr handle);

	// Set the length of a file to a new value.  Returns false
	// if an I/O error occurred.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool SetLength(IntPtr handle, long value);

	// Lock a region of a file.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool Lock(IntPtr handle, long position, long length);

	// Unlock a region of a file.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool Unlock
			(IntPtr handle, long position, long length);

	// Get the last-occurring system error code for the current thread.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno GetErrno();

	// Get a descriptive message for an error from the underlying platform.
	// Returns null if the platform doesn't have an appropriate message.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetErrnoMessage(Errno errno);

	// Copies a file from src to dest
	// Returns an Errno for the call status
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno Copy(string src, string dest);

	// Sets the last write time of a file
	// Returns an Errno for the call status
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno SetLastWriteTime(string path, long ticks);

	// Sets the last access time for a file
	// Returns an Errno for the call status
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno SetLastAccessTime(string path, long ticks);

	// Sets the creation time for a file
	// Returns an Errno for the call status
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno SetCreationTime(string path, long ticks);
	
	// Get the attributes on a file.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno GetAttributes(string path, out int attrs);

	// Set the attributes on a file.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno SetAttributes(string path, int attrs);

	// Get the length of a file.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno GetLength(string path, out long length);

	// Read the contents of a symlink.  Returns "Errno.Success" and
	// "null" in "contents" if the path exists but is not a symlink.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno ReadLink(String path, out String contents);

	// Create a symbolic link.  Returns "Errno.EPERM" if the filesystem
	// does not support the creation of symbolic links.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno CreateLink(String oldpath, String newpath);

}; // class FileMethods

}; // namespace Platform
