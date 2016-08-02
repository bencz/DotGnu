/*
 * Directory.cs - Implementation of the "System.IO.Directory" class.
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
using System.Private;
using System.Collections;
using Platform;

public sealed class Directory
{
	// Cannot instantiate this class.
	private Directory() {}

	// Validate a pathname.
	internal static void ValidatePath(String path)
			{
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}
				else if(path.Length == 0)
				{
					throw new ArgumentException(_("IO_InvalidPathname"));
				}
				else if(!FileMethods.ValidatePathname(path))
				{
					throw new ArgumentException(_("IO_InvalidPathname"));
				}
			}

	// Handle errors reported by the runtime engine.
	private static void HandleErrorsDir(Errno error)
			{
				switch(error)
				{
					case Errno.Success: break;

					case Errno.ENOENT:
					case Errno.ENOTDIR:
					{
						throw new DirectoryNotFoundException
							(_("IO_DirNotFound"));
					}
					// Not reached.

					case Errno.EACCES:
					{
						throw new UnauthorizedAccessException
							(_("IO_AccessDenied"));
					}
					
					case Errno.ENAMETOOLONG:
					{
						throw new PathTooLongException
							(_("Exception_PathTooLong"));
					}
					
					default:
					{
						throw new IOException(error);
					}
					// Not reached.
				}
			}
	internal static void HandleErrorsFile(Errno error)
			{
				switch(error)
				{
					case Errno.Success: break;

					case Errno.ENOENT:
					{
						throw new FileNotFoundException
							(_("IO_PathNotFound"));
					}
					// Not reached.

					case Errno.ENOTDIR:
					{
						throw new DirectoryNotFoundException
							(_("IO_DirNotFound"));
					}
					// Not reached.

					case Errno.EACCES:
					{
						throw new UnauthorizedAccessException
							(_("IO_AccessDenied"));
					}

					case Errno.ENAMETOOLONG:
					{
						throw new PathTooLongException
							(_("Exception_PathTooLong"));
					}
					
					default:
					{
						throw new IOException(error);
					}
					// Not reached.
				}
			}

	// Delete a directory.
	public static void Delete(String path)
			{
				Delete(path, false);
			}
	public static void Delete(String path, bool recursive)
			{
				// Remove trailing directory separators.
				if(path != null)
				{
					int len = path.Length - 1;
					if(len > 0)
					{
						if(path[len] == Path.DirectorySeparatorChar ||
						   path[len] == Path.AltDirectorySeparatorChar)
						{
							path = path.Substring(0, len);
						}
					}
				}

				// Validate the pathname.
				ValidatePath(path);

				// Recursively delete the directory's contents if necessary.
				if(recursive)
				{
					InternalFileInfo[] entries;
					int posn;
					String filename;
					if(DirMethods.GetFilesInDirectory(path, out entries)
							== Errno.Success && entries != null)
					{
						for(posn = 0; posn < entries.Length; ++posn)
						{
							filename = entries[posn].fileName;
							if(filename == "." || filename == "..")
							{
								continue;
							}
							filename = path + Path.DirectorySeparatorChar +
									   filename;
							if(entries[posn].fileType == FileType.directory)
							{
								Delete(filename, true);
							}
							else
							{
								File.Delete(filename);
							}
						}
					}
				}

				// Delete the directory itself.
				HandleErrorsDir(DirMethods.Delete(path));
			}

	// Determine if a directory with a specific path exists.
	public static bool Exists(String path)
			{
				try
				{
					ValidatePath(path);
				}
				catch (ArgumentException)
				{
					return false;
				}
				
				return (FileMethods.GetFileType(path) == FileType.directory);
			}

	// Get the creation time of a directory.
	public static DateTime GetCreationTime(String path)
			{
				long time;
				ValidatePath(path);
				HandleErrorsDir(DirMethods.GetCreationTime(path, out time));
				return (new DateTime(time)).ToLocalTime();
			}

	// Get the current working directory.
	public static String GetCurrentDirectory()
			{
				String dir = DirMethods.GetCurrentDirectory();
				if(dir == null)
				{
					throw new UnauthorizedAccessException
						(_("IO_AccessDenied"));
				}
				return dir;
			}

	// Get the current working directory for a particular drive.
	internal static String GetCurrentDirectory(char drive)
			{
				if(drive >= 'a' && drive <= 'z')
				{
					drive = (char)(drive - 'a' + 'A');
				}
				String current = GetCurrentDirectory();
				if(current.Length >= 2 && Path.IsVolumeSeparator(current[1]))
				{
					char d = current[0];
					if(d >= 'a' && d <= 'z')
					{
						d = (char)(d - 'a' + 'A');
					}
					if(d == drive)
					{
						return current;
					}
				}
				return drive.ToString() + ":" +
					   Path.DirectorySeparatorChar.ToString();
			}

	// Directory scan types.
	internal enum ScanType
	{
		Directories			= 0,
		Files				= 1,
		DirectoriesAndFiles = 2
	};

	// Scan a directory to collect up all entries that match
	// the specified search criteria.
	private static String[] ScanDirectory
				(String path, String searchPattern, ScanType scanType)
			{
				Regex regex;
				ArrayList list;
				InternalFileInfo[] entries;
				Errno error;
				int posn;
				String filename;
				FileType type;

				// Get all files in the directory.
				error = DirMethods.GetFilesInDirectory(path, out entries);
				if(error != Errno.Success)
				{
					HandleErrorsDir(error);
				}
				if(entries == null)
				{
					return new String [0];
				}

				// Convert the search pattern into a regular expression.
				if(searchPattern == null)
				{
					regex = null;
				}
				else
				{
					regex = new Regex(searchPattern, RegexSyntax.Wildcard);
				}

				// Scan the file list and collect up matching entries.
				list = new ArrayList (entries.Length);
				for(posn = 0; posn < entries.Length; ++posn)
				{
					filename = entries[posn].fileName;
					if(filename == "." || filename == "..")
					{
						continue;
					}
					type = entries[posn].fileType;
					switch(scanType)
					{
						case ScanType.Directories:
						{
							if(type != FileType.directory)
							{
								continue;
							}
						}
						break;

						case ScanType.Files:
						{
							if(type == FileType.directory)
							{
								continue;
							}
						}
						break;

						default: break;
					}
					if(regex != null && !regex.Match(filename))
					{
						continue;
					}
					list.Add(Path.Combine(path, filename));
				}

				// Dispose of the regular expression.
				if(regex != null)
				{
					regex.Dispose();
				}

				// Return the list of strings to the caller.
				return (String[])(list.ToArray(typeof(String)));
			}

#if !ECMA_COMPAT

	// Scan a directory to collect up all entries that match
	// the specified search criteria, returning FileSystemInfo's.
	internal static Object ScanDirectoryForInfos
				(String path, String searchPattern,
				 ScanType scanType, Type arrayType)
			{
				Regex regex;
				ArrayList list;
				InternalFileInfo[] entries;
				Errno error;
				int posn;
				String filename;
				FileType type;

				// Get all files in the directory.
				error = DirMethods.GetFilesInDirectory(path, out entries);
				if(error != Errno.Success)
				{
					HandleErrorsDir(error);
				}
				if(entries == null)
				{
					return new String [0];
				}

				// Convert the search pattern into a regular expression.
				if(searchPattern == null)
				{
					regex = null;
				}
				else
				{
					regex = new Regex(searchPattern, RegexSyntax.Wildcard);
				}

				// Scan the file list and collect up matching entries.
				list = new ArrayList (entries.Length);
				for(posn = 0; posn < entries.Length; ++posn)
				{
					filename = entries[posn].fileName;
					if(filename == "." || filename == "..")
					{
						continue;
					}
					type = entries[posn].fileType;
					switch(scanType)
					{
						case ScanType.Directories:
						{
							if(type != FileType.directory)
							{
								continue;
							}
						}
						break;

						case ScanType.Files:
						{
							if(type == FileType.directory)
							{
								continue;
							}
						}
						break;

						default: break;
					}
					if(regex != null && !regex.Match(filename))
					{
						continue;
					}
					if(type == FileType.directory)
					{
						list.Add(new DirectoryInfo
									(Path.Combine(path, filename)));
					}
					else
					{
						list.Add(new FileInfo(Path.Combine(path, filename)));
					}
				}

				// Dispose of the regular expression.
				if(regex != null)
				{
					regex.Dispose();
				}

				// Return the list of strings to the caller.
				return list.ToArray(arrayType);
			}

#endif // !ECMA_COMPAT

	// Get the sub-directories under a specific directory.
	public static String[] GetDirectories(String path)
			{
				ValidatePath(path);
				return ScanDirectory(path, null, ScanType.Directories);
			}
	public static String[] GetDirectories(String path, String searchPattern)
			{
				ValidatePath(path);
				if(searchPattern == null)
				{
					throw new ArgumentNullException("searchPattern");
				}
				return ScanDirectory(path, searchPattern,
									 ScanType.Directories);
			}

	// Get the root directory for a specific path.
	public static String GetDirectoryRoot(String path)
			{
				return Path.GetPathRoot(path);
			}

	// Get the files under a specific directory.
	public static String[] GetFiles(String path)
			{
				ValidatePath(path);
				return ScanDirectory(path, null, ScanType.Files);
			}
	public static String[] GetFiles(String path, String searchPattern)
			{
				ValidatePath(path);
				if(searchPattern == null)
				{
					throw new ArgumentNullException("searchPattern");
				}
				return ScanDirectory(path, searchPattern,
									 ScanType.Files);
			}

	// Get all filesystem entries under a specific directory.
	public static String[] GetFileSystemEntries(String path)
			{
				ValidatePath(path);
				return ScanDirectory(path, null, ScanType.DirectoriesAndFiles);
			}
	public static String[] GetFileSystemEntries
					(String path, String searchPattern)
			{
				ValidatePath(path);
				if(searchPattern == null)
				{
					throw new ArgumentNullException("searchPattern");
				}
				return ScanDirectory(path, searchPattern,
									 ScanType.DirectoriesAndFiles);
			}

	// Get the last access time of a directory.
	public static DateTime GetLastAccessTime(String path)
			{
				long time;
				ValidatePath(path);
				HandleErrorsDir(DirMethods.GetLastAccess(path, out time));
				return (new DateTime(time)).ToLocalTime();
			}

	// Get the last modification time of a directory.
	public static DateTime GetLastWriteTime(String path)
			{
				long time;
				ValidatePath(path);
				HandleErrorsDir(DirMethods.GetLastModification(path, out time));
				return (new DateTime(time)).ToLocalTime();
			}

	// Move a file or directory to a new location.
	public static void Move(String sourceDirName, String destDirName)
			{
				ValidatePath(sourceDirName);
				ValidatePath(destDirName);
				HandleErrorsFile(DirMethods.Rename(sourceDirName, destDirName));
			}

	// Set a directory's creation time.
	public static void SetCreationTime(String path, DateTime creationTime)
			{
				ValidatePath(path);
				long ticks = creationTime.ToUniversalTime().Ticks;
				HandleErrorsDir(FileMethods.SetCreationTime(path, ticks));
			}

	// Set the current directory.
	public static void SetCurrentDirectory(String path)
			{
				ValidatePath(path);
				HandleErrorsDir(DirMethods.ChangeDirectory(path));
			}

	// Set a directory's last access time.
	public static void SetLastAccessTime(String path, DateTime lastAccessTime)
			{
				ValidatePath(path);
				long ticks = lastAccessTime.ToUniversalTime().Ticks;
				HandleErrorsDir(FileMethods.SetLastAccessTime(path, ticks));
			}

	// Set a directory's last write time.
	public static void SetLastWriteTime(String path, DateTime lastWriteTime)
			{
				ValidatePath(path);
				long ticks = lastWriteTime.ToUniversalTime().Ticks;
				HandleErrorsDir(FileMethods.SetLastWriteTime(path, ticks));
			}

#if !ECMA_COMPAT

	// Create a directory, including parent directories.
	public static DirectoryInfo CreateDirectory(String path)
			{
				// Brubbel 2004-07-01: remove slashes and backslashes at end of path, if not you'll get an exception
				while(path.EndsWith("/")) path = path.Remove(path.Length-1, 1);
				while(path.EndsWith("\\")) path = path.Remove(path.Length-1, 1);
				// Brubbel End.
				ValidatePath(path);
				Errno error = DirMethods.CreateDirectory(path);
				if(error != Errno.Success)
				{
					// The path may already exist.
					if(Exists(path))
					{
						return new DirectoryInfo(path);
					}

					// Attempt to create the parent directory.
					String parent = Path.GetDirectoryName(path);
					if(parent == null)
					{
						HandleErrorsDir(error);
					}
					CreateDirectory(parent);

					// Now try creating the child directory again.
					error = DirMethods.CreateDirectory(path);
					if(error != Errno.Success)
					{
						HandleErrorsDir(error);
					}
				}
				return new DirectoryInfo(path);
			}

	// Get the UTC creation time of a directory.
	public static DateTime GetCreationTimeUtc(String path)
			{
				long time;
				ValidatePath(path);
				HandleErrorsDir(DirMethods.GetCreationTime(path, out time));
				return new DateTime(time);
			}

	// Get the UTC last access time of a directory.
	public static DateTime GetLastAccessTimeUtc(String path)
			{
				long time;
				ValidatePath(path);
				HandleErrorsDir(DirMethods.GetLastAccess(path, out time));
				return new DateTime(time);
			}

	// Get the UTC last modification time of a directory.
	public static DateTime GetLastWriteTimeUtc(String path)
			{
				long time;
				ValidatePath(path);
				HandleErrorsDir(DirMethods.GetLastModification(path, out time));
				return new DateTime(time);
			}

	// Get the logical drive letters.
	public static String[] GetLogicalDrives()
			{
				return DirMethods.GetLogicalDrives();
			}

	// Get the parent of a specific directory.
	public static DirectoryInfo GetParent(String path)
			{
				int index, index2;
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}
				path = Path.GetFullPath(path);
				index = path.LastIndexOf(Path.DirectorySeparatorChar);
				index2 = path.LastIndexOf(Path.AltDirectorySeparatorChar);
				if(index2 > index)
				{
					index = index2;
				}
				if(index <= 0)
				{
					return null;
				}
				return new DirectoryInfo(path.Substring(0, index));
			}

	// Set a directory's UTC creation time.
	public static void SetCreationTimeUtc(String path, DateTime creationTime)
			{
				ValidatePath(path);
				long ticks = creationTime.Ticks;
				HandleErrorsDir(FileMethods.SetCreationTime(path, ticks));
			}

	// Set a directory's UTC last access time.
	public static void SetLastAccessTimeUtc
				(String path, DateTime lastAccessTime)
			{
				ValidatePath(path);
				long ticks = lastAccessTime.Ticks;
				HandleErrorsDir(FileMethods.SetLastAccessTime(path, ticks));
			}

	// Set a directory's UTC last write time.
	public static void SetLastWriteTimeUtc(String path, DateTime lastWriteTime)
			{
				ValidatePath(path);
				long ticks = lastWriteTime.Ticks;
				HandleErrorsDir(FileMethods.SetLastWriteTime(path, ticks));
			}

#endif // !ECMA_COMPAT

}; // class Directory

}; // namespace System.IO
