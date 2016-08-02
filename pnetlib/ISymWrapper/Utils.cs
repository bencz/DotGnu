/*
 * Utils.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.Utils" class.
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

internal sealed class Utils
{
	// Read an int32 value from a buffer.
	public static int ReadInt32(byte[] buffer, int offset)
			{
				return (buffer[offset] |
				        (buffer[offset + 1] << 8) |
				        (buffer[offset + 2] << 16) |
				        (buffer[offset + 3] << 24));
			}

	// Read a uint16 value from a buffer.
	public static int ReadUInt16(byte[] buffer, int offset)
			{
				return (buffer[offset] |
				        (buffer[offset + 1] << 8));
			}

	// Read a metadata-encoded integer.
	public static int ReadMetaInt(byte[] buffer, int offset, out int size)
			{
				int value;
				if(offset >= buffer.Length)
				{
					size = 0;
					return 0;
				}
				value = buffer[offset++];
				if(value < 128)
				{
					size = 1;
					return value;
				}
				else if((value & 0xC0) == 0x80)
				{
					if(offset >= buffer.Length)
					{
						size = 0;
						return 0;
					}
					size = 2;
					return ((value & 0x3F) << 8) | buffer[offset];
				}
				else if((value & 0xE0) == 0xC0)
				{
					if((offset + 2) >= buffer.Length)
					{
						size = 0;
						return 0;
					}
					size = 4;
					return ((value & 0x1F) << 24) |
						   (buffer[offset] << 16) |
						   (buffer[offset + 1] << 8) |
						   buffer[offset + 2];
				}
				else if((value & 0xE0) == 0xE0)
				{
					if((offset + 3) >= buffer.Length)
					{
						size = 0;
						return 0;
					}
					size = 5;
					return (buffer[offset] << 24) |
						   (buffer[offset + 1] << 16) |
						   (buffer[offset + 2] << 8) |
						   buffer[offset + 3];
				}
				else
				{
					size = 0;
					return 0;
				}
			}

	// Create a pseudo-token value.  The "name" must be at least
	// four characters in length.
	public static int CreatePseudoToken(String name)
			{
				return ((int)(name[0])) |
					   (((int)(name[1])) << 8) |
					   (((int)(name[2])) << 16) |
					   (((int)(name[3])) << 24) |
					   unchecked((int)0x80000000);
			}

}; // class Utils

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
