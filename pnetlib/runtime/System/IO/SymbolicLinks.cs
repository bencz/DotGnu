/*
 * SymbolicLinks.cs - Implementation of the "System.IO.SymbolicLinks" class.
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

// This is a private extension API, for accessing information about symbolic
// links and Windows shortcuts in the underlying filesystem.  We need it for
// the file dialogs in "System.Windows.Forms".  Use with care elsewhere.
//
// Information about the ".lnk" format can be found at "myfileformats.com".

[NonStandardExtra]
public sealed class SymbolicLinks
{
	// Cannot instantiate this class.
	private SymbolicLinks() {}

	// Determine if we appear to be running on Windows.
	private static bool IsWindows()
			{
			#if !ECMA_COMPAT
				return (InfoMethods.GetPlatformID() != PlatformID.Unix);
			#else
				return (Path.DirectorySeparatorChar == '\\');
			#endif
			}

	// Determine if a pathname ends in ".lnk".
	private static bool EndsInLnk(String pathname)
			{
				int len = pathname.Length;
				if(len < 4)
				{
					return false;
				}
				if(pathname[len - 4] == '.' &&
				   (pathname[len - 3] == 'l' || pathname[len - 3] == 'L') &&
				   (pathname[len - 2] == 'n' || pathname[len - 2] == 'N') &&
				   (pathname[len - 1] == 'k' || pathname[len - 1] == 'K'))
				{
					return true;
				}
				return false;
			}

	// Determine if a pathname is a symbolic link.
	public static bool IsSymbolicLink(String pathname)
			{
				// Validate the parameter and normalize it.
				if(pathname == null)
				{
					throw new ArgumentNullException("pathname");
				}
				pathname = Path.NormalizeSeparators(pathname);

				// If we are on Windows, then check for a ".lnk" file.
				if(IsWindows())
				{
					if(!EndsInLnk(pathname))
					{
						// We need to add ".lnk" to the pathname first.
						if(FileMethods.GetFileType(pathname)
								!= FileType.unknown)
						{
							// The path exists, so it cannot be a symlink.
							return false;
						}
						pathname += ".lnk";
					}
					if(FileMethods.GetFileType(pathname)
							!= FileType.regularFile)
					{
						// The path does not exist or is not a regular file.
						return false;
					}
					return true;
				}

				// Get the file type from the engine and check it.
				FileType type = FileMethods.GetFileType(pathname);
				return (type == FileType.symbolicLink);
			}

	// Read the contents of a symbolic link.  Returns null if not a
	// symbolic link, or throws an exception if the pathname is invalid.
	public static String ReadLink(String pathname)
			{
				// Validate the parameter and normalize it.
				Directory.ValidatePath(pathname);
				pathname = Path.NormalizeSeparators(pathname);

				// Read the contents of the symlink using the engine.
				String contents;
				Errno error = FileMethods.ReadLink(pathname, out contents);
				if(error != Errno.Success)
				{
					Directory.HandleErrorsFile(error);
				}
				if(contents != null)
				{
					return contents;
				}

				// Bail out if we aren't on Windows.
				if(!IsWindows())
				{
					return null;
				}

				// Make sure that we have ".lnk" on the end of the name.
				if(!EndsInLnk(pathname))
				{
					if(FileMethods.GetFileType(pathname) != FileType.unknown)
					{
						// Something exists here and it does not have a ".lnk"
						// extension, so it cannot possibly be a symlink.
						return null;
					}
					pathname += ".lnk";
				}
				FileType type = FileMethods.GetFileType(pathname);
				if(type == FileType.unknown)
				{
					// The pathname does not exist.
					Directory.HandleErrorsFile(Errno.ENOENT);
				}
				else if(type != FileType.regularFile)
				{
					// The pathname exists but it is not a regular file,
					// so it cannot possibly be a symbolic link.
					return null;
				}

				// Read the contents of the Windows shortcut file.
				ShortcutInformation info = ReadShortcut(pathname);
				if(info == null)
				{
					return null;
				}
				else
				{
					return info.RelativePath;
				}
			}

	// Create a symbolic link at "newpath" that points to "oldpath".
	// Throws "NotSupportedException" if the filesystem does not
	// support the creation of symbolic links or ".lnk" files.
	public static void CreateLink(String oldpath, String newpath)
			{
				// Validate and normalize the parameters.
				Directory.ValidatePath(oldpath);
				Directory.ValidatePath(newpath);
				oldpath = Path.NormalizeSeparators(oldpath);
				newpath = Path.NormalizeSeparators(newpath);

				// Try to create the symlink using the engine.
				Errno error = FileMethods.CreateLink(oldpath, newpath);
				if(error == Errno.Success)
				{
					return;
				}
				else if(error != Errno.EPERM)
				{
					Directory.HandleErrorsFile(error);
				}

				// Bail out if we are not on Windows.
				if(!IsWindows())
				{
					throw new NotSupportedException(_("IO_NotSupp_Symlinks"));
				}

				// Make sure that we have ".lnk" on the end of the pathname.
				if(!EndsInLnk(newpath))
				{
					if(FileMethods.GetFileType(newpath) != FileType.unknown)
					{
						// There already exists something at this location.
						Directory.HandleErrorsFile(Errno.EEXIST);
					}
					newpath += ".lnk";
				}
				if(FileMethods.GetFileType(newpath) != FileType.unknown)
				{
					// There already exists something at this location.
					Directory.HandleErrorsFile(Errno.EEXIST);
				}

				// Create the shortcut information to place in the file.
				ShortcutInformation info = new ShortcutInformation();
				info.Description = oldpath;
				info.RelativePath = oldpath;
				info.ShowWindow = ShowWindow.Normal;

				// Write the shortcut information to the file.
				WriteShortcut(newpath, info, false);
			}

	// Attribute information for a Windows shortcut file.
	[Flags]
	public enum ShortcutAttributes
	{
		ReadOnly		= 0x0001,
		Hidden			= 0x0002,
		System			= 0x0004,
		Volume			= 0x0008,
		Directory		= 0x0010,
		Archive			= 0x0020,
		Encrypted		= 0x0040,
		Normal			= 0x0080,
		Temporary		= 0x0100,
		Sparse			= 0x0200,
		PointData		= 0x0400,
		Compressed		= 0x0800,
		Offline			= 0x1000

	}; // enum ShortcutAttributes

	// "Show window" values for a Windows shortcut file.
	public enum ShowWindow
	{
		Hide			= 0,
		Normal			= 1,
		ShowMinimized	= 2,
		ShowMaximized	= 3,
		ShowNoActive	= 4,
		Show			= 5,
		Minimize		= 6,
		ShowMinNoActive	= 7,
		ShowNa			= 8,
		Restore			= 9,
		ShowDefault		= 10

	}; // enum ShowWindow

	// Information that may be stored in a Windows shortcut file.
	public class ShortcutInformation
	{
		// Internal state.
		private ShortcutAttributes attributes;
		private String description;
		private String relativePath;
		private String workingDirectory;
		private String commandLineArguments;
		private String icon;
		private DateTime creationTime;
		private DateTime modificationTime;
		private DateTime lastAccessTime;
		private uint fileLength;
		private uint iconNumber;
		private ShowWindow showWindow;
		private uint hotkey;
		private byte[] shellIdInfo;
		private byte[] fileLocationInfo;
		private byte[] rest;

		// Constructor.
		public ShortcutInformation() {}

		// Get or set this object's properties.
		public ShortcutAttributes Attributes
				{
					get
					{
						return attributes;
					}
					set
					{
						attributes = value;
					}
				}
		public String Description
				{
					get
					{
						return description;
					}
					set
					{
						description = value;
					}
				}
		public String RelativePath
				{
					get
					{
						return relativePath;
					}
					set
					{
						relativePath = value;
					}
				}
		public String WorkingDirectory
				{
					get
					{
						return workingDirectory;
					}
					set
					{
						workingDirectory = value;
					}
				}
		public String CommandLineArguments
				{
					get
					{
						return commandLineArguments;
					}
					set
					{
						commandLineArguments = value;
					}
				}
		public String Icon
				{
					get
					{
						return icon;
					}
					set
					{
						icon = value;
					}
				}
		public DateTime CreationTime
				{
					get
					{
						return creationTime;
					}
					set
					{
						creationTime = value;
					}
				}
		public DateTime ModificationTime
				{
					get
					{
						return modificationTime;
					}
					set
					{
						modificationTime = value;
					}
				}
		public DateTime LastAccessTime
				{
					get
					{
						return lastAccessTime;
					}
					set
					{
						lastAccessTime = value;
					}
				}
		public uint FileLength
				{
					get
					{
						return fileLength;
					}
					set
					{
						fileLength = value;
					}
				}
		public uint IconNumber
				{
					get
					{
						return iconNumber;
					}
					set
					{
						iconNumber = value;
					}
				}
		public ShowWindow ShowWindow
				{
					get
					{
						return showWindow;
					}
					set
					{
						showWindow = value;
					}
				}
		public uint Hotkey
				{
					get
					{
						return hotkey;
					}
					set
					{
						hotkey = value;
					}
				}
		public byte[] ShellIdInfo
				{
					get
					{
						return shellIdInfo;
					}
					set
					{
						shellIdInfo = value;
					}
				}
		public byte[] FileLocationInfo
				{
					get
					{
						return fileLocationInfo;
					}
					set
					{
						fileLocationInfo = value;
					}
				}
		public byte[] Rest
				{
					get
					{
						return rest;
					}
					set
					{
						rest = value;
					}
				}

	}; // class ShortcutInformation

	// Magic number for Windows shortcut files.
	private static readonly byte[] magic =
			{0x4C, 0x00, 0x00, 0x00, 0x01, 0x14, 0x02, 0x00,
			 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00,
			 0x00, 0x00, 0x00, 0x46};

	// Read a 16-bit value from a stream.
	private static int ReadInt16(Stream stream, byte[] buffer)
			{
				if(stream.Read(buffer, 0, 2) != 2)
				{
					return 0;
				}
				return (int)(buffer[0] | (buffer[1] << 8));
			}

	// Read a 32-bit value from a stream.
	private static uint ReadUInt32(Stream stream, byte[] buffer)
			{
				if(stream.Read(buffer, 0, 4) != 4)
				{
					return 0;
				}
				return (uint)(buffer[0] | (buffer[1] << 8) |
							  (buffer[2] << 16) | (buffer[3] << 24));
			}

	// Base value to turn a FILETIME into a DateTime.
	private static readonly DateTime FileTimeBase = new DateTime(1601, 1, 1);

	// Read a FILETIME value from a stream as a DateTime object.
	private static DateTime ReadFileTime(Stream stream, byte[] buffer)
			{
				uint low = ReadUInt32(stream, buffer);
				uint high = ReadUInt32(stream, buffer);
				long combined = (((long)high) << 32) | ((long)low);
				if(combined == 0)
				{
					return DateTime.MinValue;
				}
				else
				{
					return new DateTime(combined + FileTimeBase.Ticks);
				}
			}

	// Read an ASCII string value from a stream.
	private static String ReadString(Stream stream, byte[] buffer)
			{
				int len = ReadInt16(stream, buffer);
				byte[] data = new byte [len];
				stream.Read(data, 0, len);
				return Encoding.Default.GetString(data);
			}

	// Read the contents of a Windows shortcut file.  Returns null if
	// the file does not appear to be a shortcut, or throws an exception
	// if the pathname is invalid in some fashion.
	public static ShortcutInformation ReadShortcut(String pathname)
			{
				byte[] buffer = new byte [64];
				ShortcutInformation info = new ShortcutInformation();
				int posn;

				// Open the shortcut file.
				FileStream stream = new FileStream
					(pathname, FileMode.Open, FileAccess.Read);
				try
				{
					// Check the magic number in the header.
					if(stream.Read(buffer, 0, 20) != 20)
					{
						return null;
					}
					for(posn = 0; posn < 20; ++posn)
					{
						if(buffer[posn] != magic[posn])
						{
							return null;
						}
					}

					// Read the rest of the header and decode it.
					uint flags = ReadUInt32(stream, buffer);
					info.Attributes =
						(ShortcutAttributes)(ReadUInt32(stream, buffer));
					info.CreationTime = ReadFileTime(stream, buffer);
					info.ModificationTime = ReadFileTime(stream, buffer);
					info.LastAccessTime = ReadFileTime(stream, buffer);
					info.FileLength = ReadUInt32(stream, buffer);
					info.IconNumber = ReadUInt32(stream, buffer);
					info.ShowWindow =
						(ShowWindow)(ReadUInt32(stream, buffer));
					info.Hotkey = ReadUInt32(stream, buffer);
					ReadUInt32(stream, buffer);		// reserved
					ReadUInt32(stream, buffer);		// reserved

					// Read the shell item id list as an opaque blob.
					int dataLen;
					byte[] data;
					if((flags & 0x0001) != 0)
					{
						dataLen = ReadInt16(stream, buffer);
						data = new byte [dataLen];
						stream.Read(data, 0, dataLen);
						info.ShellIdInfo = data;
					}

					// Read the file location information as an opaque blob.
					if((flags & 0x0002) != 0)
					{
						dataLen = (int)(ReadUInt32(stream, buffer));
						data = new byte [dataLen + 4];
						Array.Copy(buffer, 0, data, 0, 4);
						stream.Read(data, 4, dataLen - 4);
						info.FileLocationInfo = data;
					}

					// Read the string information that is present.
					if((flags & 0x0004) != 0)
					{
						info.Description = ReadString(stream, buffer);
					}
					if((flags & 0x0008) != 0)
					{
						info.RelativePath = ReadString(stream, buffer);
					}
					if((flags & 0x0010) != 0)
					{
						info.WorkingDirectory = ReadString(stream, buffer);
					}
					if((flags & 0x0020) != 0)
					{
						info.CommandLineArguments = ReadString(stream, buffer);
					}
					if((flags & 0x0040) != 0)
					{
						info.Icon = ReadString(stream, buffer);
					}

					// Read the rest of the file as an opaque blob.
					if((flags & 0x0080) != 0 && stream.CanSeek)
					{
						dataLen = (int)(stream.Length - stream.Position);
						if(dataLen > 0)
						{
							data = new byte [dataLen];
							stream.Read(data, 0, dataLen);
							info.Rest = data;
						}
					}
				}
				finally
				{
					stream.Close();
				}
				return info;
			}

	// Write a 16-bit value to a stream.
	private static void WriteInt16(Stream stream, byte[] buffer, int value)
			{
				buffer[0] = (byte)value;
				buffer[1] = (byte)(value >> 8);
				stream.Write(buffer, 0, 2);
			}

	// Write a 32-bit value to a stream.
	private static void WriteUInt32(Stream stream, byte[] buffer, uint value)
			{
				buffer[0] = (byte)value;
				buffer[1] = (byte)(value >> 8);
				buffer[2] = (byte)(value >> 16);
				buffer[3] = (byte)(value >> 24);
				stream.Write(buffer, 0, 4);
			}

	// Write a DateTime object from a stream as a FILETIME value.
	private static void WriteFileTime
				(Stream stream, byte[] buffer, DateTime value)
			{
				long ticks = value.Ticks;
				if(ticks != 0)
				{
					ticks -= FileTimeBase.Ticks;
				}
				buffer[0] = (byte)ticks;
				buffer[1] = (byte)(ticks >> 8);
				buffer[2] = (byte)(ticks >> 16);
				buffer[3] = (byte)(ticks >> 24);
				buffer[4] = (byte)(ticks >> 32);
				buffer[5] = (byte)(ticks >> 40);
				buffer[6] = (byte)(ticks >> 48);
				buffer[7] = (byte)(ticks >> 56);
				stream.Write(buffer, 0, 8);
			}

	// Write an ASCII string value to a stream.
	private static void WriteString(Stream stream, byte[] buffer, String value)
			{
				byte[] data = Encoding.Default.GetBytes(value);
				WriteInt16(stream, buffer, data.Length);
				stream.Write(data, 0, data.Length);
			}

	// Write a Windows shortcut file.  If "createIfExists" is "false",
	// then an exception will be thrown if "pathname" already exists.
	public static void WriteShortcut(String pathname, ShortcutInformation info,
				 					 bool createIfExists)
			{
				// Validate the "info" parameter.
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}

				// Create the shortcut file stream.
				FileStream stream = new FileStream
						(pathname, (createIfExists ? FileMode.Create
												   : FileMode.CreateNew),
						 FileAccess.Write);
				byte[] buffer = new byte [64];
				try
				{
					// Write the magic number to the file.
					stream.Write(magic, 0, 20);

					// Determine the flags to put in the file header.
					uint flags = 0;
					if(info.ShellIdInfo != null)
					{
						flags |= 0x0001;
					}
					if(info.FileLocationInfo != null)
					{
						flags |= 0x0002;
					}
					if(info.Description != null)
					{
						flags |= 0x0004;
					}
					if(info.RelativePath != null)
					{
						flags |= 0x0008;
					}
					if(info.WorkingDirectory != null)
					{
						flags |= 0x0010;
					}
					if(info.CommandLineArguments != null)
					{
						flags |= 0x0020;
					}
					if(info.Icon != null)
					{
						flags |= 0x0040;
					}
					if(info.Rest != null)
					{
						flags |= 0x0080;
					}

					// Write out the file header.
					WriteUInt32(stream, buffer, flags);
					WriteUInt32(stream, buffer, (uint)(info.Attributes));
					WriteFileTime(stream, buffer, info.CreationTime);
					WriteFileTime(stream, buffer, info.ModificationTime);
					WriteFileTime(stream, buffer, info.LastAccessTime);
					WriteUInt32(stream, buffer, info.FileLength);
					WriteUInt32(stream, buffer, info.IconNumber);
					WriteUInt32(stream, buffer, (uint)(info.ShowWindow));
					WriteUInt32(stream, buffer, info.Hotkey);
					WriteUInt32(stream, buffer, 0);	// reserved
					WriteUInt32(stream, buffer, 0);	// reserved

					// Write the shell item id information.
					if(info.ShellIdInfo != null)
					{
						WriteInt16(stream, buffer, info.ShellIdInfo.Length);
						stream.Write(info.ShellIdInfo, 0,
									 info.ShellIdInfo.Length);
					}

					// Write the file location information.
					if(info.FileLocationInfo != null)
					{
						stream.Write(info.FileLocationInfo, 0,
									 info.FileLocationInfo.Length);
					}

					// Write the string information.
					if(info.Description != null)
					{
						WriteString(stream, buffer, info.Description);
					}
					if(info.RelativePath != null)
					{
						WriteString(stream, buffer, info.RelativePath);
					}
					if(info.WorkingDirectory != null)
					{
						WriteString(stream, buffer, info.WorkingDirectory);
					}
					if(info.CommandLineArguments != null)
					{
						WriteString(stream, buffer, info.CommandLineArguments);
					}
					if(info.Icon != null)
					{
						WriteString(stream, buffer, info.Icon);
					}

					// Write the rest of the data for the shortcut file.
					if(info.Rest != null)
					{
						stream.Write(info.Rest, 0, info.Rest.Length);
					}
				}
				finally
				{
					stream.Close();
				}
			}

}; // class SymbolicLinks

}; // namespace System.IO
