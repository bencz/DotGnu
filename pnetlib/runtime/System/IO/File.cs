/*
 * File.cs - Implementation of the "System.IO.File" class.
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
using Platform;

public sealed class File
{
	// Cannot instantiate this class.
	private File() {}

	// Open a file for text appending.
	public static StreamWriter AppendText(String path)
			{
				return new StreamWriter(path, true);
			}

	// Make a copy of a file.
	public static void Copy(String sourceFileName, String destFileName)
			{
				Copy(sourceFileName, destFileName, false);
			}
	public static void Copy(String sourceFileName, String destFileName,
							bool overwrite)
			{
				// Open the source to be copied.
				FileStream src = new FileStream
					(sourceFileName, FileMode.Open, FileAccess.Read);

				// Open the destination to be copied to.
				FileStream dest;
				try
				{
					if(overwrite)
					{
						dest = new FileStream
							(destFileName, FileMode.Create,
							 FileAccess.Write);
					}
					else
					{
						dest = new FileStream
							(destFileName, FileMode.CreateNew,
							 FileAccess.Write);
					}
				}
				catch
				{
					// Could not open the destination, so close the source.
					src.Close();
					throw;
				}

				// Copy the contents of the file.
				try
				{
					byte[] buffer = new byte [FileStream.BUFSIZ];
					int len;
					while((len = src.Read(buffer, 0, FileStream.BUFSIZ)) > 0)
					{
						dest.Write(buffer, 0, len);
					}
				}
				finally
				{
					src.Close();
					dest.Close();
				}
			}

	// Create a stream for a file.
	public static FileStream Create(String path)
			{
				return Create(path, FileStream.BUFSIZ);
			}
	public static FileStream Create(String path, int bufferSize)
			{
				return new FileStream
					(path, FileMode.Create, FileAccess.ReadWrite,
					 FileShare.None, bufferSize);
			}

	// Create a file for text writing.
	public static StreamWriter CreateText(String path)
			{
				return new StreamWriter(path, false);
			}

	// Delete a file.
	public static void Delete(String path)
			{
				Directory.ValidatePath(path);
				Errno errno = DirMethods.Delete(path);
				if (errno != Errno.ENOENT)
					Directory.HandleErrorsFile(errno);
			}

	// Determine whether a file exists.
	public static bool Exists(String path)
			{
				try
				{
					Directory.ValidatePath(path);
				}
				catch(Exception)
				{
					return false;
				}
				FileType type = FileMethods.GetFileType(path);
				return (type != FileType.directory &&
						type != FileType.unknown);
			}

	// Get a file's creation time.
	public static DateTime GetCreationTime(String path)
			{
				long ticks;
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(DirMethods.GetCreationTime(path, out ticks));
				return (new DateTime(ticks)).ToLocalTime();
			}

	// Get a file's last access time.
	public static DateTime GetLastAccessTime(String path)
			{
				long ticks;
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(DirMethods.GetLastAccess(path, out ticks));
				return (new DateTime(ticks)).ToLocalTime();
			}

	// Get a file's last modification time.
	public static DateTime GetLastWriteTime(String path)
			{
				long ticks;
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(DirMethods.GetLastModification(path, out ticks));
				return (new DateTime(ticks)).ToLocalTime();
			}

	// Move a file to a new location.
	public static void Move(String sourceFileName, String destFileName)
			{
				Directory.Move(sourceFileName, destFileName);
			}

	// Open a file stream.
	public static FileStream Open(String path, FileMode mode)
			{
				return new FileStream
					(path, mode, FileAccess.ReadWrite, FileShare.None);
			}
	public static FileStream Open
				(String path, FileMode mode, FileAccess access)
			{
				return new FileStream(path, mode, access, FileShare.None);
			}
	public static FileStream Open
				(String path, FileMode mode,
				 FileAccess access, FileShare share)
			{
				return new FileStream(path, mode, access, share);
			}

	// Open a file for reading.
	public static FileStream OpenRead(String path)
			{
				return new FileStream
					(path, FileMode.Open, FileAccess.Read, FileShare.None);
			}

	// Open a text file for reading.
	public static StreamReader OpenText(String path)
			{
				return new StreamReader(path);
			}

	// Open a file for writing.
	public static FileStream OpenWrite(String path)
			{
				return new FileStream
					(path, FileMode.OpenOrCreate,
					 FileAccess.Write, FileShare.None);
			}

	// Set the creation time on a file.
	public static void SetCreationTime(String path, DateTime creationTime)
			{
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(FileMethods.SetCreationTime
						(path, creationTime.ToUniversalTime().Ticks));
			}

	// Set the last access time on a file.
	public static void SetLastAccessTime(String path, DateTime lastAccessTime)
			{
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(FileMethods.SetLastAccessTime
						(path, lastAccessTime.ToUniversalTime().Ticks));
			}

	// Set the last modification time on a file.
	public static void SetLastWriteTime(String path, DateTime lastWriteTime)
			{
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(FileMethods.SetLastWriteTime
						(path, lastWriteTime.ToUniversalTime().Ticks));
			}

#if !ECMA_COMPAT

	// Get the attributes for a file.
	public static FileAttributes GetAttributes(String path)
			{
				int attrs;
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(FileMethods.GetAttributes(path, out attrs));
				return (FileAttributes)attrs;
			}

	// Get a file's UTC creation time.
	public static DateTime GetCreationTimeUtc(String path)
			{
				long ticks;
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(DirMethods.GetCreationTime(path, out ticks));
				return new DateTime(ticks);
			}

	// Get a file's UTC last access time.
	public static DateTime GetLastAccessTimeUtc(String path)
			{
				long ticks;
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(DirMethods.GetLastAccess(path, out ticks));
				return new DateTime(ticks);
			}

	// Get a file's UTC last modification time.
	public static DateTime GetLastWriteTimeUtc(String path)
			{
				long ticks;
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(DirMethods.GetLastModification(path, out ticks));
				return new DateTime(ticks);
			}

	// Set the attributes on a file.
	public static void SetAttributes
				(String path, FileAttributes fileAttributes)
			{
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(FileMethods.SetAttributes(path, (int)fileAttributes));
			}

	// Set the UTC creation time on a file.
	public static void SetCreationTimeUtc(String path, DateTime creationTime)
			{
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(FileMethods.SetCreationTime(path, creationTime.Ticks));
			}

	// Set the UTC last access time on a file.
	public static void SetLastAccessTimeUtc
				(String path, DateTime lastAccessTime)
			{
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(FileMethods.SetLastAccessTime
						(path, lastAccessTime.Ticks));
			}

	// Set the UTC last modification time on a file.
	public static void SetLastWriteTimeUtc(String path, DateTime lastWriteTime)
			{
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(FileMethods.SetLastWriteTime
						(path, lastWriteTime.Ticks));
			}

	// Get the length of a file.
	internal static long GetLength(String path)
			{
				long length;
				Directory.ValidatePath(path);
				Directory.HandleErrorsFile
					(FileMethods.GetLength(path, out length));
				return length;
			}

#endif // !ECMA_COMPAT

}; // class File

}; // namespace System.IO
