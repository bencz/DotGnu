/*
 * SymInfoEnumerator.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.SymInfoEnumerator" class.
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

using System.Collections;

// Utility class for enumerating over all of the data blocks
// that are associated with a particular token.

internal class SymInfoEnumerator : IEnumerator
{
	// Internal state.
	private SymReader reader;
	private int token;
	private int start;
	private int num;
	private int index;
	private int type;
	private int offset;
	private int length;
	private int readPosn;

	// Constructors.
	public SymInfoEnumerator(SymReader reader, String name)
			: this(reader, Utils.CreatePseudoToken(name)) {}
	public SymInfoEnumerator(SymReader reader, int token)
			{
				// Save the reader and token information for later.
				this.reader = reader;
				this.token = token;
				this.index = -1;
				if(reader == null || reader.data == null)
				{
					// There is no symbol information to be processed.
					this.start = 0;
					this.num = 0;
					return;
				}

				// Locate the token information in the symbol data.
				int left = 0;
				int right = reader.numIndexEntries - 1;
				int middle, temp;
				while(left <= right)
				{
					middle = left + (right - left) / 2;
					temp = Utils.ReadInt32
						(reader.data, reader.indexOffset + middle * 8);
					if(((uint)temp) < ((uint)token))
					{
						left = middle + 1;
					}
					else if(((uint)temp) > ((uint)token))
					{
						right = middle - 1;
					}
					else
					{
						// We've found an entry: search forwards and
						// backwards to find the extent of the token.
						left = middle;
						while(left > 0)
						{
							temp = Utils.ReadInt32
								(reader.data,
								 reader.indexOffset + (left - 1) * 8);
							if(temp == token)
							{
								--left;
							}
							else
							{
								break;
							}
						}
						right = middle;
						while(right < (reader.numIndexEntries - 1))
						{
							temp = Utils.ReadInt32
								(reader.data,
								 reader.indexOffset + (right + 1) * 8);
							if(temp == token)
							{
								++right;
							}
							else
							{
								break;
							}
						}
						this.start = left;
						this.num = right - left + 1;
						return;
					}
				}

				// We were unable to find the token data.
				this.start = 0;
				this.num = 0;
			}
	public SymInfoEnumerator(SymReader reader)
			{
				// This version enumerates over all of the data blocks.
				this.reader = reader;
				this.token = 0;
				this.start = 0;
				if(reader != null)
				{
					this.num = reader.numIndexEntries;
				}
				else
				{
					this.num = 0;
				}
				this.index = -1;
			}

	// Implement the IEnumerator interface.
	public bool MoveNext()
			{
				if(++index < num)
				{
					// Read the information about this data block.
					token = Utils.ReadInt32
						(reader.data,
						 reader.indexOffset + (start + index) * 8);
					offset = Utils.ReadInt32
						(reader.data,
						 reader.indexOffset + (start + index) * 8 + 4);
					if(offset < 0 || offset >= reader.data.Length)
					{
						return false;
					}

					// Get the type and length information.
					int size;
					type = Utils.ReadMetaInt
						(reader.data, offset, out size);
					offset += size;
					length = Utils.ReadMetaInt
						(reader.data, offset, out size);
					offset += size;
					if(length < 0 || (reader.data.Length - offset) < length)
					{
						return false;
					}

					// Ready to process this data item.
					readPosn = offset;
					return true;
				}
				else
				{
					return false;
				}
			}
	public void Reset()
			{
				index = -1;
			}
	public Object Current
			{
				get
				{
					return token;
				}
			}

	// Get the number of data blocks that will be enumerated.
	public int Count
			{
				get
				{
					return num;
				}
			}

	// Get additional information about the data block.
	public int Token
			{
				get
				{
					return token;
				}
			}
	public int Type
			{
				get
				{
					return type;
				}
			}
	public int Offset
			{
				get
				{
					return offset;
				}
			}
	public int Length
			{
				get
				{
					return length;
				}
			}
	public byte[] Data
			{
				get
				{
					byte[] data = new byte [length];
					Array.Copy(reader.data, offset, data, 0, length);
					return data;
				}
			}

	// Get the next metadata-encoded integer from the data section.
	public int GetNextInt()
			{
				int value, size;
				if(readPosn < (offset + length))
				{
					value = Utils.ReadMetaInt(reader.data, readPosn, out size);
					readPosn += size;
					return value;
				}
				else
				{
					return -1;
				}
			}

}; // class SymInfoEnumerator

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
