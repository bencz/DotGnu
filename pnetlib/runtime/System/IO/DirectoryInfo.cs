/*
 * DirectoryInfo.cs - Implementation of the "System.IO.DirectoryInfo" class.
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
public sealed class DirectoryInfo : FileSystemInfo
{
	// Constructor.
	public DirectoryInfo(String path)
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
	internal DirectoryInfo(SerializationInfo info, StreamingContext context)
			: base(info, context)
			{
				// Nothing to do here.
			}
#endif

	// Properties.
	public DirectoryInfo Parent
			{
				get
				{
					return new DirectoryInfo(Path.GetDirectoryName(FullPath));
				}
			}
	public override bool Exists
			{
				get
				{
					return Directory.Exists(FullPath);
				}
			}
	public override String Name
			{
				get
				{
					return Path.GetFileName(FullPath);
				}
			}
	public DirectoryInfo Root
			{
				get
				{
					return new DirectoryInfo(Path.GetPathRoot(FullPath));
				}
			}

	// Create a directory.
	public void Create()
			{
				Directory.CreateDirectory(FullPath);
			}
	public DirectoryInfo CreateSubdirectory(String name)
			{
				String dir = Path.Combine(FullPath, Path.GetFileName(name));
				Directory.CreateDirectory(dir);
				return new DirectoryInfo(dir);
			}

	// Delete the directory represented by this object.
	public override void Delete()
			{
				Directory.Delete(FullPath, false);
			}
	public void Delete(bool recurse)
			{
				Directory.Delete(FullPath, recurse);
			}

	// Move this directory to a new location.
	public void MoveTo(String dest)
			{
				Directory.Move(FullPath, dest);
			}

	// Get the contents of this directory.
	public FileInfo[] GetFiles()
			{
				return (FileInfo[])(Directory.ScanDirectoryForInfos
					(FullPath, null, Directory.ScanType.Files,
					 typeof(FileInfo)));
			}
	public FileInfo[] GetFiles(String pattern)
			{
				if(pattern == null)
				{
					throw new ArgumentNullException("pattern");
				}
				return (FileInfo[])(Directory.ScanDirectoryForInfos
					(FullPath, pattern, Directory.ScanType.Files,
					 typeof(FileInfo)));
			}
	public DirectoryInfo[] GetDirectories()
			{
				return (DirectoryInfo[])(Directory.ScanDirectoryForInfos
					(FullPath, null, Directory.ScanType.Directories,
					 typeof(DirectoryInfo)));
			}
	public DirectoryInfo[] GetDirectories(String pattern)
			{
				if(pattern == null)
				{
					throw new ArgumentNullException("pattern");
				}
				return (DirectoryInfo[])(Directory.ScanDirectoryForInfos
					(FullPath, pattern, Directory.ScanType.Directories,
					 typeof(DirectoryInfo)));
			}
	public FileSystemInfo[] GetFileSystemInfos()
			{
				return (FileSystemInfo[])(Directory.ScanDirectoryForInfos
					(FullPath, null, Directory.ScanType.DirectoriesAndFiles,
					 typeof(FileSystemInfo)));
			}
	public FileSystemInfo[] GetFileSystemInfos(String pattern)
			{
				if(pattern == null)
				{
					throw new ArgumentNullException("pattern");
				}
				return (FileSystemInfo[])(Directory.ScanDirectoryForInfos
					(FullPath, pattern, Directory.ScanType.DirectoriesAndFiles,
					 typeof(FileSystemInfo)));
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return OriginalPath;
			}

}; // class DirectoryInfo

#endif // !ECMA_COMPAT

}; // namespace System.IO
