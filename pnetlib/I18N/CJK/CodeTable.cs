/*
 * CodeTable.cs - Implementation of the "System.Text.CodeTable" class.
 *
 * Copyright (c) 2002  Southern Storm Software, Pty Ltd
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

namespace I18N.CJK
{

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using I18N.Common;

// This class assists encoding classes for the large CJK character
// sets by providing pointer access to table data in the resource
// section of the current assembly.
//
// Code tables are named by their resource (e.g. "jis.table") and
// contain one or more sections.  Each section has an 8-byte header,
// consisting of a 32-bit section number and a 32-bit section length.
// The alignment of the data in the table is not guaranteed.

internal unsafe sealed class CodeTable : IDisposable
{
	// Internal state.
	private Stream stream;

	// Load a code table from the resource section of this assembly.
	public CodeTable(String name)
			{
				stream = (Assembly.GetExecutingAssembly()
							 .GetManifestResourceStream(name));
				if(stream == null)
				{
					throw new NotSupportedException
						(String.Format
							(Strings.GetString("NotSupp_MissingCodeTable"),
							 name));
				}
			}

	// Implement the IDisposable interface.
	public void Dispose()
			{
				if(stream != null)
				{
					stream.Close();
					stream = null;
				}
			}

	// Get the starting address for a particular section within
	// the code table.  This address is guaranteed to persist
	// after "Dispose" is called.
	public byte *GetSection(int num)
			{
				// If the table has been disposed, then bail out.
				if(stream == null)
				{
					return null;
				}

				// Scan through the stream looking for the section.
				long posn = 0;
				long length = stream.Length;
				byte[] header = new byte [8];
				int sectNum, sectLen;
				while((posn + 8) <= length)
				{
					// Read the next header block.
					stream.Position = posn;
					if(stream.Read(header, 0, 8) != 8)
					{
						break;
					}

					// Decode the fields in the header block.
					sectNum = ((int)(header[0])) |
							  (((int)(header[1])) << 8) |
							  (((int)(header[2])) << 16) |
							  (((int)(header[3])) << 24);
					sectLen = ((int)(header[4])) |
							  (((int)(header[5])) << 8) |
							  (((int)(header[6])) << 16) |
							  (((int)(header[7])) << 24);

					// Is this the section we are looking for?
					if(sectNum == num)
					{
						return GetAddress(stream, posn + 8);
					}

					// Advance to the next section.
					posn += 8 + sectLen;
				}

				// We were unable to find the requested section.
				return null;
			}

	// Back door access into the engine to get the address of
	// an offset within a manifest resource stream.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static byte *GetAddress(Stream stream, long position);

}; // class CodeTable

}; // namespace I18N.CJK
