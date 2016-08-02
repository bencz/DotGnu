/*
 * Path.cs - Implementation of the "System.IO.Path" class.
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
using System.Text;
using Platform;

public sealed class Path
{
	// Cannot instantiate this class.
	private Path() {}

	// Special operating system characters.
	public static readonly char AltDirectorySeparatorChar;
	public static readonly char DirectorySeparatorChar;
	public static readonly char PathSeparator;
#if ECMA_COMPAT
	internal static readonly char[] InvalidPathChars;
	internal static readonly char VolumeSeparatorChar;
#else
	public static readonly char[] InvalidPathChars;
	public static readonly char VolumeSeparatorChar;
#endif

	// Initialize the special operating system characters.
	static Path()
			{
				PathInfo info = DirMethods.GetPathInfo();
				AltDirectorySeparatorChar = info.altDirSeparator;
				DirectorySeparatorChar = info.dirSeparator;
				PathSeparator = info.pathSeparator;
				VolumeSeparatorChar = info.volumeSeparator;
				InvalidPathChars = info.invalidPathChars;
			}

	// Determine if a character is a directory separator.
	internal static bool IsSeparator(char ch)
			{
				// We always check for '/' and '\\' because Windows
				// programmers have been known to hard-wire them
				// into their code.
				if(ch == '/' || ch == '\\' ||
				   ch == DirectorySeparatorChar ||
				   (AltDirectorySeparatorChar != '\0' &&
				    ch == AltDirectorySeparatorChar))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Determine if a character is a directory or volume separator.
	private static bool IsDirectoryOrVolumeSeparator(char ch)
			{
				// We always check for '/', '\\', and ':' because Windows
				// programmers have been known to hard-wire them
				// into their code.
				if(ch == '/' || ch == '\\' || ch == ':' ||
				   ch == DirectorySeparatorChar ||
				   (AltDirectorySeparatorChar != '\0' &&
				    ch == AltDirectorySeparatorChar) ||
				   (VolumeSeparatorChar != '\0' &&
				    ch == VolumeSeparatorChar))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Determine if a character is a volume separator.
	internal static bool IsVolumeSeparator(char ch)
			{
				if(ch == ':' ||
				   (VolumeSeparatorChar != '\0' &&
				    ch == VolumeSeparatorChar))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Normalize a pathname to only contain valid directory separators.
	internal static String NormalizeSeparators(String path)
			{
				// See if the path needs modification or not.
				if(path == null)
				{
					return null;
				}
				if(DirectorySeparatorChar == '/')
				{
					// Convert backslashes into forward slashes.
					if(path.IndexOf('\\') == -1)
					{
						return path;
					}
				}
				else if(DirectorySeparatorChar == '\\')
				{
					// Convert forward slashes into backslashes.
					if(path.IndexOf('/') == -1)
					{
						return path;
					}
				}
				else
				{
					// Convert both forward and backslashes.
					if(path.IndexOf('/') == -1 && path.IndexOf('\\') == -1)
					{
						return path;
					}
				}

				// Normalize the path to only use "DirectorySeparatorChar".
				StringBuilder builder = new StringBuilder(path.Length);
				foreach(char ch in path)
				{
					if(IsSeparator(ch))
					{
						builder.Append(DirectorySeparatorChar);
					}
					else
					{
						builder.Append(ch);
					}
				}
				return builder.ToString();
			}

	// Change the extension on a file.
	public static String ChangeExtension(String path, String extension)
			{
				if(path == null)
				{
					return null;
				}
				int posn = path.Length;
				char ch;
				while(posn > 0)
				{
					ch = path[--posn];
					if(ch == '.')
					{
						return path.Substring(0, posn + 1) + extension;
					}
					else if(IsDirectoryOrVolumeSeparator(ch))
					{
						return path + "." + extension;
					}
				}
				return path + "." + extension;
			}

	// Combine two paths into one.
	public static String Combine(String path1, String path2)
			{
				if(path1 == null)
				{
					throw new ArgumentNullException("path1");
				}
				else if(path2 == null)
				{
					throw new ArgumentNullException("path2");
				}
				else if(path1.Length == 0)
				{
					return NormalizeSeparators(path2);
				}
				else if(path2.Length == 0)
				{
					return NormalizeSeparators(path1);
				}
				else if(IsPathRooted(path2))
				{
					return NormalizeSeparators(path2);
				}
				else if(IsDirectoryOrVolumeSeparator(path1[path1.Length - 1]))
				{
					return NormalizeSeparators(path1 + path2);
				}
				else
				{
					return NormalizeSeparators
						(path1 + DirectorySeparatorChar.ToString() + path2);
				}
			}

	// Determine if a character is a drive letter.
	private static bool IsDriveLetter(char ch)
			{
				if(ch >= 'A' && ch <= 'Z')
				{
					return true;
				}
				if(ch >= 'a' && ch <= 'z')
				{
					return true;
				}
				return false;
			}

	// Determine if a path is a root directory specification.
	// Also validates network share names to be of the form
	// "\\host\\share".  Returns the length of the root prefix
	// or -1 if it does not start with a root prefix.
	private static int GetRootPrefixLength(String path)
			{
				if(path.Length == 0)
				{
					throw new ArgumentException(_("IO_InvalidPathname"));
				}
				if(IsSeparator(path[0]))
				{
					// Check for "/" and "\\...".
					if(path.Length == 1)
					{
						return 1;
					}
					if(!IsSeparator(path[1]))
					{
						return 1;
					}

					// Validate the network share specification.
					int posn = 2;
					while(posn < path.Length && !IsSeparator(path[posn]))
					{
						++posn;
					}
					if((posn + 1) >= path.Length)
					{
						// Network shares must be "\\foo\bar", not "\\foo".
						throw new ArgumentException(_("IO_InvalidPathname"));
					}
					++posn;
					while(posn < path.Length && !IsSeparator(path[posn]))
					{
						++posn;
					}
					return posn;
				}
				else if(IsDriveLetter(path[0]) && path.Length > 1 &&
						IsVolumeSeparator(path[1]))
				{
					if(path.Length > 2 && IsSeparator(path[2]))
					{
						return 3;
					}
					return 2;
				}
				else
				{
					return -1;
				}
			}

	// Extract the directory name from a path.
	public static String GetDirectoryName(String path)
			{
				if(path == null)
				{
					return null;
				}
				int rootLen = GetRootPrefixLength(path);
				if(rootLen == path.Length)
				{
					return null;
				}
				int posn = path.Length;
				char ch;
				while(posn > 0)
				{
					ch = path[--posn];
					if(IsSeparator(ch))
					{
						if(posn == 0)
						{
							// Return a path of the form "/".
							return new String(DirectorySeparatorChar, 1);
						}
						else if(posn > 0 && IsVolumeSeparator(path[posn - 1]))
						{
							// Return a path of the form "X:\".
							return NormalizeSeparators
								(path.Substring(0, posn + 1));
						}
						else
						{
							return NormalizeSeparators
								(path.Substring(0, posn));
						}
					}
					else if(IsVolumeSeparator(ch))
					{
						// Return a path of the form "X:\".
						path = path.Substring(0, posn + 1);
						path += DirectorySeparatorChar.ToString();
						return NormalizeSeparators(path);
					}
				}
				return String.Empty;
			}

	// Extract the file name from a path.
	public static String GetFileName(String path)
			{
				if(path == null)
				{
					return null;
				}
				int posn = path.Length;
				char ch;
				while(posn > 0)
				{
					ch = path[--posn];
					if(IsDirectoryOrVolumeSeparator(ch))
					{
						return path.Substring(posn + 1);
					}
				}
				return path;
			}

	// Extract the file name from a path, without the extension.
	public static String GetFileNameWithoutExtension(String path)
			{
				if(path == null)
				{
					return null;
				}
				int posn = path.Length;
				int dot = -1;
				char ch;
				while(posn > 0)
				{
					ch = path[--posn];
					if(ch == '.')
					{
						// If there are multiple extensions, then only
						// strip the last one in the filename.
						if(dot == -1)
						{
							dot = posn;
						}
					}
					else if(IsDirectoryOrVolumeSeparator(ch))
					{
						if(dot != -1)
						{
							return path.Substring(posn + 1, dot - (posn + 1));
						}
						else
						{
							return path.Substring(posn + 1);
						}
					}
				}
				if(dot != -1)
				{
					return path.Substring(0, dot);
				}
				else
				{
					return path;
				}
			}

	// Extract the extension from a path.
	public static String GetExtension(String path)
			{
				if(path == null)
				{
					return null;
				}
				int posn = path.Length;
				char ch;
				while(posn > 0)
				{
					ch = path[--posn];
					if(ch == '.')
					{
						if((posn + 1) == path.Length)
						{
							// Ends in a dot.
							return String.Empty;
						}
						return path.Substring(posn);
					}
					else if(IsDirectoryOrVolumeSeparator(ch))
					{
						break;
					}
				}
				return String.Empty;
			}

	// Combine a parent path and a child path.
	private static String Combine(String path1, int rootLen, String path2)
			{
				if(path2.Length == 0)
				{
					return path1;
				}
				StringBuilder builder = new StringBuilder(path1);
				int posn = 0;
				int len;
				char ch;
				if(path1.Length == 0 || !IsSeparator(path1[path1.Length - 1]))
				{
					builder.Append(DirectorySeparatorChar);
				}
				while(posn < path2.Length)
				{
					ch = path2[posn];
					if(IsDirectoryOrVolumeSeparator(ch))
					{
						if(!IsSeparator(builder[builder.Length - 1]))
						{
							builder.Append(DirectorySeparatorChar);
						}
						++posn;
					}
					else if(ch == '.')
					{
						++posn;
						if(posn >= path2.Length || IsSeparator(path2[posn]))
						{
							// Remove "." components from the pathname.
							++posn;
						}
						else if(path2[posn] == '.' &&
						        ((posn + 1) >= path2.Length ||
								 IsSeparator(path2[posn + 1])))
						{
							// Strip the builder down to the parent directory.
							++posn;
							len = builder.Length;
							if(len > rootLen && IsSeparator(builder[len - 1]))
							{
								--len;
							}
							while(len > rootLen &&
							      !IsSeparator(builder[len - 1]))
							{
								--len;
							}
							if(len > rootLen && IsSeparator(builder[len - 1]))
							{
								--len;
							}
							builder.Length = len;
						}
						else
						{
							// Filename component beginning with a dot.
							builder.Append(ch);
						}
					}
					else
					{
						builder.Append(ch);
						++posn;
					}
				}
				return builder.ToString();
			}

	// Get a full absolute pathname.
	public static String GetFullPath(String path)
			{
				// Check the parameter for null.
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}
				path = path.Trim();

				// Validate zero-length and network share paths.
				int rootLen = GetRootPrefixLength(path);

				// Split into parent path and suffix.
				String parentPath;
				if(rootLen != -1)
				{
					if(IsSeparator(path[0]))
					{
						// Unix root or network share specification.
						parentPath = path.Substring(0, rootLen);
						path = path.Substring(rootLen);
						if(path.Length > 0 && IsSeparator(path[0]))
						{
							path = path.Substring(1);
						}
					}
					else if(rootLen == 2)
					{
						// Drive letter with relative path specification.
						parentPath = Directory.GetCurrentDirectory(path[0]);
						path = path.Substring(2);
					}
					else
					{
						// Drive letter with absolute path specification.
						parentPath = path.Substring(0, 3);
						path = path.Substring(3);
					}
				}
				else
				{
					parentPath = Directory.GetCurrentDirectory();
				}

				// Normalize the parent path.
				parentPath = NormalizeSeparators(parentPath);

				// Determine the length of the parent path's root portion.
				rootLen = GetRootPrefixLength(parentPath);

				// Combine the parent and actual path.
				return Combine(parentPath, rootLen, path);
			}

	// Get the root directory for a path.
	public static String GetPathRoot(String path)
			{
				// Convert the path into its full form.
				path = GetFullPath(path);

				// Extract the root information.
				int rootLen = GetRootPrefixLength(path);
				if(rootLen != -1)
				{
					return path.Substring(0, rootLen);
				}
				else
				{
					return NormalizeSeparators("/");
				}
			}

	// Number to start searching from for temporary files.
	private static int start;

	// Get the name of a unique temporary file.
	public static String GetTempFileName()
			{
				String dir = GetTempPath();
				String full;
				lock(typeof(Path))
				{
					if(start == 0)
					{
						start = ((int)(DateTime.UtcNow.Ticks)) & 0x0000FFFF;
					}
					do
					{
						full = dir + DirectorySeparatorChar + "cli" +
							   start.ToString("X4");
						++start;
					}
					while(File.Exists(full));
				}
				return full;
			}

	// Get the path to use to store temporary files.
	public static String GetTempPath()
			{
				String env;
				env = Environment.GetEnvironmentVariable("TMPDIR");
				if(env != null && env.Length > 0)
				{
					return env;
				}
				env = Environment.GetEnvironmentVariable("TEMP");
				if(env != null && env.Length > 0)
				{
					return env;
				}
				return NormalizeSeparators("/tmp");
			}

	// Determine if a path has an extension.
	public static bool HasExtension(String path)
			{
				if(path == null)
				{
					return false;
				}
				int posn = path.Length;
				char ch;
				while(posn > 0)
				{
					ch = path[--posn];
					if(ch == '.')
					{
						return (posn < (path.Length - 1));
					}
					else if(IsDirectoryOrVolumeSeparator(ch))
					{
						return false;
					}
				}
				return false;
			}

	// Determine if a path is absolute.
	public static bool IsPathRooted(String path)
			{
				if(path == null || path.Length == 0)
				{
					return false;
				}
				else if(IsSeparator(path[0]))
				{
					// Unix style ("/bar") or network share ("\\foo").
					return true;
				}
				else if(path.Length >= 2 && IsVolumeSeparator(path[1]))
				{
					// Windows style ("c:\bar").
					return true;
				}
				else
				{
					return false;
				}
			}

}; // class Path

}; // namespace System.IO
