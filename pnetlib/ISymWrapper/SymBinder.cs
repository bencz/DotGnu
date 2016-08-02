/*
 * SymBinder.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.SymBinder" class.
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

namespace System.Diagnostics.SymbolStore
{

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.IO;

public class SymBinder : ISymbolBinder
{
	// Constructor.
	public SymBinder() {}

	// Destructor (C++ style).
	~SymBinder() {}
	public void __dtor()
			{
				GC.SuppressFinalize(this);
				Finalize();
			}

	// Create a symbol reader for a file.  Will set "wasBinary" to
	// true if the file is a PE/COFF binary that does not contain
	// debug symbol information.  Returns null if the file does not
	// contain debug symbol information.
	private static ISymbolReader CreateReader
				(String filename, out bool wasBinary)
			{
				FileStream stream;
				byte[] buffer = new byte [1024];
				byte[] section;
				int len, offset;

				// Clear the "wasBinary" flag before we start.
				wasBinary = false;

				// Open the file.
				try
				{
					stream = new FileStream
						(filename, FileMode.Open, FileAccess.Read);
				}
				catch(Exception)
				{
					return null;
				}
				try
				{
					// Check the magic number to determine the file type.
					if(stream.Read(buffer, 0, 8) != 8)
					{
						return null;
					}
					if(buffer[0] == (byte)'I' &&
					   buffer[1] == (byte)'L' &&
					   buffer[2] == (byte)'D' &&
					   buffer[3] == (byte)'B' &&
					   buffer[4] == (byte)0x01 &&
					   buffer[5] == (byte)0x00 &&
					   buffer[6] == (byte)0x00 &&
					   buffer[7] == (byte)0x00)
					{
						// This is a standalone debug symbol file.
						len = (int)(stream.Length);
						section = new byte [len];
						Array.Copy(buffer, 0, section, 0, 8);
						stream.Read(section, 8, len - 8);
						return new SymReader(filename, section);
					}
					else if(buffer[0] == (byte)'M' &&
							buffer[1] == (byte)'Z')
					{
						// We are processing a binary for embedded symbols.
						wasBinary = true;

						// Skip past the MS-DOS stub portion of the binary.
						stream.Read(buffer, 8, 64 - 8);
						offset = Utils.ReadInt32(buffer, 60);
						if(offset < 64)
						{
							return null;
						}
						stream.Position = offset;

						// Read the PE/COFF header.
						stream.Read(buffer, 0, 24);
						if(buffer[0] != (byte)'P' ||
						   buffer[1] != (byte)'E' ||
						   buffer[2] != (byte)0x00 ||
						   buffer[3] != (byte)0x00)
						{
							return null;
						}
						Array.Copy(buffer, 4, buffer, 0, 20);
					}
					else if(buffer[0] == (byte)0x4C &&
							buffer[1] == (byte)0x01)
					{
						// This is a PE/COFF object file: read the rest
						// of the PE/COFF header into memory.
						stream.Read(buffer, 8, 20 - 8);
					}
					else
					{
						// We don't know what format the file is in.
						return null;
					}

					// If we get here, then we have a PE/COFF header
					// in "buffer", minus the "PE".
					int numSections = Utils.ReadUInt16(buffer, 2);
					int headerSize = Utils.ReadUInt16(buffer, 16);
					if(headerSize != 0 &&
					   (headerSize < 216 || headerSize > 1024))
					{
						return null;
					}
					if(numSections == 0)
					{
						return null;
					}

					// Skip the optional header.
					stream.Seek(headerSize, SeekOrigin.Current);

					// Search the section table for ".ildebug".
					while(numSections > 0)
					{
						if(stream.Read(buffer, 0, 40) != 40)
						{
							return null;
						}
						if(buffer[0] == (byte)'.' &&
						   buffer[1] == (byte)'i' &&
						   buffer[2] == (byte)'l' &&
						   buffer[3] == (byte)'d' &&
						   buffer[4] == (byte)'e' &&
						   buffer[5] == (byte)'b' &&
						   buffer[6] == (byte)'u' &&
						   buffer[7] == (byte)'g')
						{
							// Skip to the debug data and read it.
							offset = Utils.ReadInt32(buffer, 20);
							len = Utils.ReadInt32(buffer, 8);
							stream.Position = offset;
							section = new byte [len];
							stream.Read(section, 0, len);
							return new SymReader(filename, section);
						}
						--numSections;
					}
				}
				finally
				{
					stream.Close();
				}

				// We were unable to find the debug symbols if we get here.
				return null;
			}

	// Get a symbol reader for a particular file.
	public virtual ISymbolReader GetReader
				(int importer, String filename, String searchPath)
			{
				// Try the file in its specified location.
				if(File.Exists(filename))
				{
					return GetReader(filename);
				}

				// Bail out if the filename was absolute, or there is no path.
				if(Path.IsPathRooted(filename))
				{
					throw new ArgumentException();
				}
				else if(searchPath == null || searchPath.Length == 0)
				{
					throw new ArgumentException();
				}

				// Search the path for the file.
				int posn = 0;
				int index1, index2;
				String dir;
				String combined;
				while(posn < searchPath.Length)
				{
					// Find the next path separator.
					index1 = searchPath.IndexOf(Path.PathSeparator, posn);
					if(Path.PathSeparator == ':')
					{
						// Unix-like system: use either ":" or ";" to separate.
						index2 = searchPath.IndexOf(';', posn);
						if(index2 != -1 && index2 < index1)
						{
							index1 = index2;
						}
					}
					if(index1 == -1)
					{
						index1 = searchPath.Length;
					}

					// Extract the directory from the path.
					dir = searchPath.Substring(posn, index1 - posn).Trim();
					posn = index1 + 1;
					if(dir.Length == 0)
					{
						continue;
					}

					// See if the specified file exists.
					combined = Path.Combine(dir, filename);
					if(File.Exists(combined))
					{
						return GetReader(combined);
					}
				}

				// We were unable to find the file.
				throw new ArgumentException();
			}
	private static ISymbolReader GetReader(String filename)
			{
				ISymbolReader reader;
				bool wasBinary;

				// Try to create a symbol reader for the file itself.
				reader = CreateReader(filename, out wasBinary);
				if(reader != null)
				{
					return reader;
				}

				// If the file was a PE/COFF binary that does not
				// include debug symbol information, then look for
				// the "*.pdb" file corresponding to the binary.
				if(wasBinary)
				{
					filename = Path.ChangeExtension(filename, "pdb");
					reader = CreateReader(filename, out wasBinary);
					if(reader != null)
					{
						return reader;
					}
				}

				// We were unable to locate the debug symbols.
				throw new ArgumentException();
			}

}; // class SymBinder

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
