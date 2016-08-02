/*
 * FileSystem.cs - Implementation of the
 *			"Microsoft.VisualBasic.FileSystem" class.
 *
 * Copyright (C) 2003, 2004  Southern Storm Software, Pty Ltd.
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

namespace Microsoft.VisualBasic
{

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
public sealed class FileSystem
{
	// This class cannot be instantiated.
	private FileSystem() {}

	// Change directories.
	public static void ChDir(String Path)
			{
				System.IO.Directory.SetCurrentDirectory(Path);
			}

	// Change drives.
	public static void ChDrive(String Drive)
			{
				if(Drive != null && Drive.Length > 0)
				{
					ChDrive(Drive[0]);
				}
			}
	public static void ChDrive(char Drive)
			{
				if(Drive >= 'a' && Drive <= 'z')
				{
					Drive = (char)(Drive - 'a' + 'A');
				}
				if(Drive < 'A' || Drive > 'Z')
				{
					// Invalid drive letter value.
					Utils.ThrowException(68);
				}
			#if !ECMA_COMPAT
				char volumeSep = Path.VolumeSeparatorChar;
			#else
				char volumeSep = ':';
			#endif
				if(volumeSep != 0)
				{
					System.IO.Directory.SetCurrentDirectory
						(Drive.ToString() + volumeSep.ToString());
				}
				else
				{
					// The operating system doesn't use drive letters.
					Utils.ThrowException(68);
				}
			}

	// Get the current directory.
	public static String CurDir(char Drive)
			{
				if(Drive >= 'a' && Drive <= 'z')
				{
					Drive = (char)(Drive - 'a' + 'A');
				}
				if(Drive < 'A' || Drive > 'Z')
				{
					// Invalid drive letter value.
					Utils.ThrowException(68);
				}
			#if !ECMA_COMPAT
				char volumeSep = Path.VolumeSeparatorChar;
			#else
				char volumeSep = ':';
			#endif
				if(volumeSep != 0)
				{
					return System.IO.Path.GetFullPath(Drive.ToString() +
												      volumeSep.ToString() +
													  ".");
				}
				else
				{
					// The operating system doesn't use drive letters.
					Utils.ThrowException(68);
					return null;
				}
			}
	public static String CurDir()
			{
				return Directory.GetCurrentDirectory();
			}

#if !ECMA_COMPAT

	// Directory scan information.
	private sealed class DirScanInfo
	{
		public Assembly assembly;
		public FileSystemInfo[] contents;
		public int posn;
		public FileAttribute searchAttrs;
		public DirScanInfo next;

		public DirScanInfo(Assembly assembly, DirScanInfo next)
				{
					this.assembly = assembly;
					this.next = next;
				}

	}; // class DirScanInfo

	// List of directory scan information blocks for each assembly.
	private static DirScanInfo dirScanList;

	// Get the directory scan information block for an assembly.
	private static DirScanInfo GetScanInfo(Assembly assembly)
			{
				lock(typeof(FileSystem))
				{
					DirScanInfo info = dirScanList;
					while(info != null)
					{
						if(info.assembly == assembly)
						{
							return info;
						}
						info = info.next;
					}
					info = new DirScanInfo(assembly, dirScanList);
					dirScanList = info;
					return info;
				}
			}

	// Get the next item in a directory.
	private static String NextDirItem(DirScanInfo info)
			{
				// Bail out if the previous directory scan is finished.
				if(info.contents == null)
				{
					return "";
				}

				// Search for the next entry that matches the attributes.
				FileAttribute searchAttrs = info.searchAttrs;
				FileAttribute attrs;
				String result;
				while(info.posn < info.contents.Length)
				{
					attrs = (FileAttribute)
						(info.contents[info.posn].Attributes);
					attrs = attrs & (FileAttribute)0x3F;
					if((searchAttrs & FileAttribute.Archive) != 0)
					{
						// Skip entries that don't have the archive bit set.
						if((attrs & FileAttribute.Archive) == 0)
						{
							++(info.posn);
							continue;
						}
					}
					if((attrs & ~FileAttribute.Archive) ==
							FileAttribute.Normal)
					{
						// Normal entries are always included.
						result = info.contents[info.posn].Name;
						++(info.posn);
						return result;
					}
					if((searchAttrs & ~FileAttribute.Archive) ==
							FileAttribute.Normal)
					{
						// We don't want anything but normal entries.
						++(info.posn);
						continue;
					}
					if((attrs & searchAttrs & ~FileAttribute.Archive) != 0)
					{
						// This entry matches the attribute conditions.
						result = info.contents[info.posn].Name;
						++(info.posn);
						return result;
					}
				}

				// The directory scan has been completed.  Null out
				// the "contents" list to allow it to be GC'ed.
				info.contents = null;
				info.posn = 0;
				return "";
			}

	// Scan through a directory.
	public static String Dir
				(String Pathname,
				 [Optional] [DefaultValue(FileAttribute.Normal)]
				 		FileAttribute Attributes)
			{
				DirScanInfo info;
				String dir;
				String pattern;

				// Get the directory scan information and clear it.
				info = GetScanInfo(Assembly.GetCallingAssembly());
				info.contents = null;
				info.posn = 0;

				// Split the pathname into directory and pattern components.
				if(Pathname == null || Pathname.Length == 0 ||
				   Pathname == ".")
				{
					dir = ".";
					pattern = "*.*";
				}
				else if(Pathname[Pathname.Length - 1] ==
							Path.DirectorySeparatorChar ||
				        (Path.AltDirectorySeparatorChar != 0 &&
						 Pathname[Pathname.Length - 1] ==
							Path.AltDirectorySeparatorChar))
				{
					dir = Pathname;
					pattern = "*.*";
				}
				else
				{
					dir = Path.GetDirectoryName(Pathname);
					pattern = Path.GetFileName(Pathname);
					if(pattern.Length == 0)
					{
						pattern = "*.*";
					}
				}

				// Scan the directory.
				try
				{
					info.contents = (new DirectoryInfo(dir)).
						GetFileSystemInfos(pattern);
				}
				catch(DirectoryNotFoundException)
				{
					// Report an empty file list if the path wasn't found.
					return "";
				}

				// Return the first matching entry.
				info.searchAttrs = Attributes;
				return NextDirItem(info);
			}
	public static String Dir()
			{
				return NextDirItem(GetScanInfo(Assembly.GetCallingAssembly()));
			}

#endif // !ECMA_COMPAT

	// Determine if a particular file is at EOF.
	public static bool EOF(int FileNumber)
			{
				return File.GetFile
					(FileNumber, Assembly.GetCallingAssembly()).EOF;
			}

	// Get the attributes that were used to open a file.
	public static OpenMode FileAttr(int FileNumber)
			{
				return File.GetFile
					(FileNumber, Assembly.GetCallingAssembly()).Mode;
			}

	// Close a group of files.
	public static void FileClose(params int[] FileNumbers)
			{
				Assembly assembly = Assembly.GetCallingAssembly();
				if(FileNumbers.Length == 0)
				{
					File.CloseAll(assembly);
				}
				else
				{
					foreach(int number in FileNumbers)
					{
						File.GetFile(number, assembly).Close();
					}
				}
			}

	// Copy a file.
	public static void FileCopy(String Source, String Destination)
			{
				System.IO.File.Copy(Source, Destination, true);
			}

	// Get the date and time for a specific file.
	public static DateTime FileDateTime(String PathName)
			{
				return System.IO.File.GetLastWriteTime(PathName);
			}

	// Get a value from a file.
	public static void FileGet
				(int FileNumber, ref Decimal Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = file.Reader.ReadDecimal();
			}
	public static void FileGet
				(int FileNumber, ref double Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = file.Reader.ReadDouble();
			}
	public static void FileGet
				(int FileNumber, ref float Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = file.Reader.ReadSingle();
			}
	public static void FileGet
				(int FileNumber, ref DateTime Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = Utils.FromOADate(file.Reader.ReadDouble());
			}
	public static void FileGet
				(int FileNumber, ref char Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = file.Reader.ReadChar();
			}
	public static void FileGet
				(int FileNumber, ref long Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = file.Reader.ReadInt64();
			}
	[TODO]
	public static void FileGet
				(int FileNumber, ref ValueType Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				// TODO
			}
	[TODO]
	public static void FileGet
				(int FileNumber, ref Array Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber,
				 [Optional] [DefaultValue(false)] bool ArrayIsDynamic,
				 [Optional] [DefaultValue(false)] bool StringIsFixedLength)
			{
				// TODO
			}
	public static void FileGet
				(int FileNumber, ref bool Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = (file.Reader.ReadInt16() != 0);
			}
	public static void FileGet
				(int FileNumber, ref byte Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = file.Reader.ReadByte();
			}
	public static void FileGet
				(int FileNumber, ref short Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = file.Reader.ReadInt16();
			}
	public static void FileGet
				(int FileNumber, ref int Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				Value = file.Reader.ReadInt32();
			}
	[TODO]
	public static void FileGet
				(int FileNumber, ref String Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber,
				 [Optional] [DefaultValue(false)] bool StringIsFixedLength)
			{
				// TODO
			}
	[TODO]
	public static void FileGetObject
				(int FileNumber, ref Object Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				// TODO
			}

#if !ECMA_COMPAT

	// Get the length of a file.
	public static long FileLen(String PathName)
			{
				return (new FileInfo(PathName)).Length;
			}

#endif

	// Open a file.
	public static void FileOpen
				(int FileNumber, String FileName, OpenMode Mode,
				 [Optional] [DefaultValue(OpenAccess.Default)]
				 	OpenAccess Access,
				 [Optional] [DefaultValue(OpenShare.Default)]
				 	OpenShare Share,
				 [Optional] [DefaultValue(-1)] int RecordLength)
			{
				FileMode mode;
				FileAccess access;
				FileShare share;
				int recordLength;
				int bufferSize;

				// Validate the parameters and convert them into
				// something that System.IO is more comfortable with.
				if(Mode != OpenMode.Input && Mode != OpenMode.Output &&
				   Mode != OpenMode.Random && Mode != OpenMode.Append &&
				   Mode != OpenMode.Binary)
				{
					throw new ArgumentException
						(S._("VB_InvalidFileMode"), "Mode");
				}
				if(Access == OpenAccess.Default)
				{
					access = FileAccess.ReadWrite;
				}
				else if(Access == OpenAccess.Read)
				{
					access = FileAccess.Read;
				}
				else if(Access == OpenAccess.Write)
				{
					access = FileAccess.Write;
				}
				else if(Access == OpenAccess.ReadWrite)
				{
					access = FileAccess.ReadWrite;
				}
				else
				{
					throw new ArgumentException
						(S._("VB_InvalidFileAccess"), "Access");
				}
				if(Share == OpenShare.Default)
				{
					share = FileShare.ReadWrite;
				}
				else if(Share == OpenShare.LockReadWrite)
				{
					share = FileShare.ReadWrite;
				}
				else if(Share == OpenShare.LockRead)
				{
					share = FileShare.Read;
				}
				else if(Share == OpenShare.LockWrite)
				{
					share = FileShare.Write;
				}
				else if(Share == OpenShare.Shared)
				{
					share = FileShare.None;
				}
				else
				{
					throw new ArgumentException
						(S._("VB_InvalidFileShare"), "Share");
				}
				if(RecordLength != -1)
				{
					if(RecordLength < 1 || RecordLength > 32767)
					{
						throw new ArgumentException
							(S._("VB_InvalidRecordLength"), "RecordLength");
					}
					if(Mode == OpenMode.Random)
					{
						recordLength = RecordLength;
						bufferSize = -1;
					}
					else
					{
						recordLength = 1;
						bufferSize = RecordLength;
					}
				}
				else
				{
					recordLength = 1;
					bufferSize = -1;
				}

				// Determine the appropriate FileMode value.
				if(Mode == OpenMode.Random || Mode == OpenMode.Binary)
				{
					if(System.IO.File.Exists(FileName))
					{
						mode = FileMode.Open;
					}
					else if(access == FileAccess.Read)
					{
						mode = FileMode.OpenOrCreate;
					}
					else
					{
						mode = FileMode.Create;
					}
				}
				else if(Mode == OpenMode.Input)
				{
					mode = FileMode.Open;
				}
				else if(Mode == OpenMode.Output)
				{
					mode = FileMode.Create;
				}
				else
				{
					mode = FileMode.Append;
				}

				// Allocate the file number.
				File file = File.AllocateFile
					(FileNumber, Mode, Assembly.GetCallingAssembly());
				file.recordLength = recordLength;

				// Attempt to open the file.
				FileStream stream = null;
				try
				{
					if(bufferSize != -1)
					{
						stream = new FileStream(FileName, mode, access,
												share, bufferSize);
					}
					else
					{
						stream = new FileStream(FileName, mode, access, share);
					}
				}
				finally
				{
					if(stream == null)
					{
						// The create failed, so clean up the allocated file.
						file.Close();
					}
				}
				file.stream = stream;
			}

	// Put a value to a file.
	public static void FilePut
				(int FileNumber, Decimal Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write(Value);
			}
	public static void FilePut
				(int FileNumber, double Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write(Value);
			}
	public static void FilePut
				(int FileNumber, float Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write(Value);
			}
	public static void FilePut
				(int FileNumber, DateTime Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write(Utils.ToOADate(Value));
			}
	public static void FilePut
				(int FileNumber, char Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write(Value);
			}
	public static void FilePut
				(int FileNumber, long Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write(Value);
			}
	[TODO]
	public static void FilePut
				(int FileNumber, ValueType Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				// TODO
			}
	[TODO]
	public static void FilePut
				(int FileNumber, Array Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber,
				 [Optional] [DefaultValue(false)] bool ArrayIsDynamic,
				 [Optional] [DefaultValue(false)] bool StringIsFixedLength)
			{
				// TODO
			}
	public static void FilePut
				(int FileNumber, bool Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				// VB boolean values are two bytes in size.
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write((short)(Value ? -1 : 0));
			}
	public static void FilePut
				(int FileNumber, byte Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write(Value);
			}
	public static void FilePut
				(int FileNumber, short Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write(Value);
			}
	public static void FilePut
				(int FileNumber, int Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				file.Writer.Write(Value);
			}
	public static void FilePut
				(int FileNumber, String Value,
			     [Optional] [DefaultValue(-1L)] long RecordNumber,
				 [Optional] [DefaultValue(false)] bool StringIsFixedLength)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Binary | OpenMode.Random);
				file.SetRecord(RecordNumber);
				if(Value == null)
				{
					Value = String.Empty;
				}
				Encoding enc = file.Encoding;
				int nbytes = enc.GetByteCount(Value);
				if(!StringIsFixedLength)
				{
					if(file.Mode == OpenMode.Random &&
					   (nbytes + 2) > file.recordLength)
					{
						Utils.ThrowException(59);	// IOException
					}
					file.Writer.Write(checked((short)nbytes));
				}
				byte[] buf = enc.GetBytes(Value);
				file.stream.Write(buf, 0, nbytes);
			}
	[Obsolete("Use FilePutObject to write Object types, or coerce FileNumber and RecordNumber to Integer for writing non-Object types")]
	public static void FilePut
				(Object FileNumber, Object Value,
			     [Optional] [DefaultValue(-1L)] Object RecordNumber)
			{
				throw new ArgumentException(S._("VB_Obsolete"));
			}
	[TODO]
	public static void FilePutObject
				(int FileNumber, Object Value,
			     [Optional] [DefaultValue(0)] long RecordNumber)
			{
				// TODO
			}

	// Set the record width for a file.
	public static void FileWidth(int FileNumber, int RecordWidth)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly());
				if(RecordWidth < 0 || RecordWidth > 255)
				{
					Utils.ThrowException(5);	// ArgumentException
				}
				file.lineLength = RecordWidth;
			}

	// Find a free file number.
	public static int FreeFile()
			{
				return File.FindFreeFile(Assembly.GetCallingAssembly());
			}

#if !ECMA_COMPAT

	// Get the attributes on a specific file.
	public static FileAttribute GetAttr(String PathName)
			{
				FileAttributes attrs;
				attrs = System.IO.File.GetAttributes(PathName);
				return (FileAttribute)(((int)attrs) & 0x3F);
			}

#endif

	// Input a value from a file.
	[TODO]
	public static void Input(int FileNumber, ref short Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref byte Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref bool Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref char Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref float Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref double Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref Decimal Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref String Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref DateTime Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref Object Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref long Value)
			{
				// TODO
			}
	[TODO]
	public static void Input(int FileNumber, ref int Value)
			{
				// TODO
			}

	// Input a string from a file.
	[TODO]
	public static String InputString(int FileNumber, int CharCount)
			{
				// TODO
				return null;
			}

	// Kill (delete) a file or directory tree.
	[TODO]
	public static void Kill(String PathName)
			{
				// TODO
			}

	// Get the length of an open file.
	public static long LOF(int FileNumber)
			{
				return File.GetFile
					(FileNumber, Assembly.GetCallingAssembly())
					.stream.Length;
			}

	// Input a line from a file.
	[TODO]
	public static String LineInput(int FileNumber)
			{
				// TODO
				return null;
			}

	// Get the current location within an open file.
	public static long Loc(int FileNumber)
			{
				return File.GetFile
					(FileNumber, Assembly.GetCallingAssembly()).Location;
			}

	// Lock a record within a file.
	public static void Lock(int FileNumber, long Record)
			{
				Lock(FileNumber, Record, Record);
			}

	// Lock the entire contents of a file.
	public static void Lock(int FileNumber)
			{
				Lock(FileNumber, 1, Int64.MaxValue);
			}

	// Lock a region within a file.
	public static void Lock(int FileNumber, long FromRecord, long ToRecord)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly());
				file.Lock(FromRecord, ToRecord);
			}

#if !ECMA_COMPAT

	// Make a new directory.
	public static void MkDir(String Path)
			{
				System.IO.Directory.CreateDirectory(Path);
			}

#endif

	// Inner version of "Print" and "Write".
	private static void Print(File file, Object[] Output,
							  CultureInfo culture, bool print)
			{
				NumberFormatInfo nfi = culture.NumberFormat;
				if(Output == null)
				{
					return;
				}
				bool first = true;
				foreach(Object obj in Output)
				{
					if(!first && !print)
					{
						file.Write(",");
					}
					else
					{
						first = false;
					}
					if(obj == null)
					{
						if(!print)
						{
							file.Write("#NULL#");
						}
						continue;
					}
					if(obj is TabInfo)
					{
						file.Tab((TabInfo)obj);
						continue;
					}
					else if(obj is SpcInfo)
					{
						file.Space((SpcInfo)obj);
						continue;
					}
					else if(obj is char[])
					{
						file.Write(new String((char[])obj));
						continue;
					}
					else if(obj is ErrObject)
					{
						if(print)
						{
							file.Write(String.Format("Error {0}",
									   ((ErrObject)obj).Number));
						}
						else
						{
							file.Write(String.Format("#ERROR {0}#",
									   ((ErrObject)obj).Number));
						}
						continue;
					}
					switch(ObjectType.GetTypeCode(obj))
					{
						case TypeCode.DBNull:
						{
							if(print)
							{
								file.Write("Null");
							}
							else
							{
								file.Write("#NULL#");
							}
						}
						break;

						case TypeCode.Boolean:
						{
							bool b = BooleanType.FromObject(obj);
							if(print)
							{
								file.Write(b.ToString(culture));
							}
							else if(b)
							{
								file.Write("#TRUE#");
							}
							else
							{
								file.Write("#FALSE#");
							}
						}
						break;

						case TypeCode.Byte:
						{
							byte by = ByteType.FromObject(obj);
							file.Write(by.ToString(nfi));
						}
						break;

						case TypeCode.Int16:
						{
							short s = ShortType.FromObject(obj);
							file.Write(s.ToString(nfi));
						}
						break;

						case TypeCode.Int32:
						{
							int i = IntegerType.FromObject(obj);
							file.Write(i.ToString(nfi));
						}
						break;

						case TypeCode.Int64:
						{
							long l = LongType.FromObject(obj);
							file.Write(l.ToString(nfi));
						}
						break;

						case TypeCode.Single:
						{
							float f = SingleType.FromObject(obj, nfi);
							file.Write(f.ToString(nfi));
						}
						break;

						case TypeCode.Double:
						{
							double d = DoubleType.FromObject(obj, nfi);
							file.Write(d.ToString(nfi));
						}
						break;

						case TypeCode.Decimal:
						{
							Decimal dc = DecimalType.FromObject(obj, nfi);
							file.Write(dc.ToString(nfi));
						}
						break;

						case TypeCode.DateTime:
						{
							DateTime dt = DateType.FromObject(obj);
							if(print)
							{
								file.Write(StringType.FromDate(dt) + " ");
							}
							else
							{
								String format;
								long dayTicks = dt.Ticks % TimeSpan.TicksPerDay;
								if(dt.Ticks == dayTicks)
								{
									format = "T";
								}
								else if(dayTicks == 0)
								{
									format = "d";
								}
								else
								{
									format = "F";
								}
								file.Write(dt.ToString
									(format, culture.DateTimeFormat));
							}
						}
						break;

						case TypeCode.String:
						{
							file.Write(StringType.FromObject(obj));
						}
						break;

						default:
						{
							Utils.ThrowException(5);	// ArgumentException.
						}
						break;
					}
				}
			}

	// Print data to a file.
	public static void Print(int FileNumber, params Object[] Output)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Output | OpenMode.Append);
				Print(file, Output, CultureInfo.CurrentCulture, true);
			}
	public static void PrintLine(int FileNumber, params Object[] Output)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Output | OpenMode.Append);
				Print(file, Output, CultureInfo.CurrentCulture, true);
				file.WriteLine();
			}

	// Rename a file or directory.
	public static void Rename(String OldPath, String NewPath)
			{
				System.IO.File.Move(OldPath, NewPath);
			}

	// Close all files and reset the filesystem.
	public static void Reset()
			{
				File.CloseAll(Assembly.GetCallingAssembly());
			}

	// Remove a directory.
	public static void RmDir(String Path)
			{
				System.IO.Directory.Delete(Path);
			}

	// Convert a count value into a "SpcInfo" value.
	public static SpcInfo SPC(short count)
			{
				return new SpcInfo(count);
			}

	// Seek to a new file position.
	public static void Seek(int FileNumber, long Position)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly());
				file.SetRecord(Position);
			}

	// Get the current seek position within a file.
	public static long Seek(int FileNumber)
			{
				return File.GetFile
					(FileNumber, Assembly.GetCallingAssembly()).Location;
			}

#if !ECMA_COMPAT

	// Set the attributes on a file.
	public static void SetAttr(String PathName, FileAttribute Attributes)
			{
				System.IO.File.SetAttributes
					(PathName, (FileAttributes)Attributes);
			}

#endif

	// Create a "TabInfo" object.
	public static TabInfo TAB()
			{
				return new TabInfo(-1);
			}
	public static TabInfo TAB(short Column)
			{
				if(Column >= 1)
				{
					return new TabInfo(Column);
				}
				else
				{
					return new TabInfo(1);
				}
			}

	// Unlock a record within a file.
	public static void Unlock(int FileNumber, long Record)
			{
				Unlock(FileNumber, Record, Record);
			}

	// Unlock the entire contents of a file.
	public static void Unlock(int FileNumber)
			{
				Unlock(FileNumber, 1, Int64.MaxValue);
			}

	// Unlock a region within a file.
	public static void Unlock(int FileNumber, long FromRecord, long ToRecord)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly());
				file.Unlock(FromRecord, ToRecord);
			}

	// Write data to a file.
	public static void Write(int FileNumber, params Object[] Output)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Output | OpenMode.Append);
				Print(file, Output, CultureInfo.InvariantCulture, false);
				file.Write(",");
			}
	public static void WriteLine(int FileNumber, params Object[] Output)
			{
				File file = File.GetFile
					(FileNumber, Assembly.GetCallingAssembly(),
					 OpenMode.Output | OpenMode.Append);
				Print(file, Output, CultureInfo.InvariantCulture, false);
				file.WriteLine();
			}

}; // class FileSystem

}; // namespace Microsoft.VisualBasic
