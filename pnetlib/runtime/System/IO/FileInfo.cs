/*
 * FileInfo.cs - Implementation of the "System.IO.FileInfo" class.
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

#if !ECMA_COMPAT

using System;
using System.Runtime.Serialization;

[Serializable]
public sealed class FileInfo : FileSystemInfo
{
	// Constructor.
	public FileInfo(String path)
			{
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}
				else if(path.IndexOfAny(Path.InvalidPathChars) != -1)
				{
					throw new ArgumentException
						(_("IO_InvalidPathname"), "path");
				}
				OriginalPath = path;
				FullPath = Path.GetFullPath(path);
			}
#if CONFIG_SERIALIZATION
	internal FileInfo(SerializationInfo info, StreamingContext context)
			: base(info, context)
			{
				// Nothing to do here.
			}
#endif

	// Properties.
	public String DirectoryName
			{
				get
				{
					return Path.GetDirectoryName(FullPath);
				}
			}
	public DirectoryInfo Directory
			{
				get
				{
					return new DirectoryInfo(DirectoryName);
				}
			}
	public override bool Exists
			{
				get
				{
					return File.Exists(FullPath);
				}
			}
	public long Length
			{
				get
				{
					return File.GetLength(FullPath);
				}
			}
	public override String Name
			{
				get
				{
					return Path.GetFileName(FullPath);
				}
			}

	// Delete the file represented by this object.
	public override void Delete()
			{
				if(Exists)
				{
					File.Delete(FullPath);
				}
			}

	// Open the file as a text stream.
	public StreamReader OpenText()
			{
				return new StreamReader(Open(FileMode.Open, FileAccess.Read));
			}
	public StreamWriter CreateText()
			{
				return new StreamWriter
					(Open(FileMode.Create, FileAccess.Write));
			}
	public StreamWriter AppendText()
			{
				return new StreamWriter
					(Open(FileMode.Append, FileAccess.Write));
			}

	// Open the file as a binary stream.
	public FileStream Create()
			{
				return File.Create(FullPath);
			}
	public FileStream OpenRead()
			{
				return Open(FileMode.Open, FileAccess.Read);
			}
	public FileStream OpenWrite()
			{
				return Open(FileMode.OpenOrCreate, FileAccess.Write);
			}
	public FileStream Open(FileMode mode)
			{
				return Open(mode, FileAccess.ReadWrite);
			}
	public FileStream Open(FileMode mode, FileAccess access)
			{
				return Open(mode, access, FileShare.None);
			}
	public FileStream Open(FileMode mode, FileAccess access, FileShare share)
			{
				return new FileStream(FullPath, mode, access, share);
			}

	// Move or copy this file.
	public void MoveTo(String dest)
			{
				File.Move(FullPath, dest);
			}
	public FileInfo CopyTo(String path)
			{
				return CopyTo(path, false);
			}
	public FileInfo CopyTo(String path, bool overwrite)
			{
				String fullPath = Path.GetFullPath(path);
				File.Copy(FullPath, fullPath);
				return new FileInfo(fullPath);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return OriginalPath;
			}

}; // class FileInfo

#endif // !ECMA_COMPAT

}; // namespace System.IO
