/*
 * BitVector32.cs - Implementation of
 *		"System.Collections.Specialized.BitVector32".
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Collections.Specialized
{

#if !ECMA_COMPAT

using System;
using System.Text;

public struct BitVector32
{
	// Internal state.
	private int data;

	// Constructors.
	public BitVector32(int data)
			{
				this.data = data;
			}
	public BitVector32(BitVector32 value)
			{
				this.data = value.data;
			}

	// Create masks.
	public static int CreateMask()
			{
				return 1;
			}
	public static int CreateMask(int previous)
			{
				if(previous == 0)
				{
					return 1;
				}
				else if(previous == unchecked((int)0x80000000))
				{
					throw new Exception(S._("Arg_MaskDone"));
				}
				else
				{
					return (previous << 1);
				}
			}

	// Get the number of bits in a value.
	private static int GetNumBits(int value)
			{
				int bits = 0;
				value &= 0xFFFF;
				while(value != 0)
				{
					++bits;
					value >>= 1;
				}
				return bits;
			}

	// Create a bit vector section.
	public static Section CreateSection(short maxValue)
			{
				return new Section(GetNumBits(maxValue), 0);
			}
	public static Section CreateSection(short maxValue, Section section)
			{
				return new Section(GetNumBits(maxValue),
								   section.offset + GetNumBits(section.mask));
			}

	// Return the full data for this bit vector.
	public int Data
			{
				get
				{
					return data;
				}
			}

	// Get or set a bit.
	public bool this[int bit]
			{
				get
				{
					return ((data & bit) == bit);
				}
				set
				{
					if(value)
						data |= bit;
					else
						data &= ~bit;
				}
			}
	public int this[Section section]
			{
				get
				{
					return (data >> section.offset) & section.mask;
				}
				set
				{
					int mask = section.mask;
					int offset = section.offset;
					data = (data & ~(mask << offset)) |
						   ((value & mask) << offset);
				}
			}

	// Determine if two bit vectors are equal.
	public override bool Equals(Object obj)
			{
				if(obj is BitVector32)
				{
					return (data == ((BitVector32)obj).data);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this bit vector.
	public override int GetHashCode()
			{
				return data;
			}

	// Convert a bit vector into a string.
	public static String ToString(BitVector32 value)
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("BitVector32{");
				uint data = (uint)(value.data);
				uint mask = 0x80000000;
				int bit;
				for(bit = 0; bit < 32; ++bit)
				{
					if((data & mask) != 0)
					{
						builder.Append('1');
					}
					else
					{
						builder.Append('0');
					}
					mask >>= 1;
				}
				builder.Append('}');
				return builder.ToString();
			}
	public override String ToString()
			{
				return ToString(this);
			}

	// Structure that represents a bit vector section.
	public struct Section
	{
		// Internal state.
		internal short mask, offset;

		// Constructor.
		internal Section(int bits, int offset)
				{
					if((bits + offset) > 32)
					{
						throw new Exception(S._("Arg_MaskDone"));
					}
					this.mask = (short)(bits != 0 ? (1 << (bits - 1)) : 0);
					this.offset = (short)offset;
				}

		// Properties.
		public short Mask
				{
					get
					{
						return mask;
					}
				}
		public short Offset
				{
					get
					{
						return offset;
					}
				}

		// Determine if two sections are equal.
		public override bool Equals(Object obj)
				{
					if(obj is Section)
					{
						Section sect = (Section)obj;
						return (mask == sect.mask && offset == sect.offset);
					}
					else
					{
						return false;
					}
				}

		// Get the hash code for this section.
		public override int GetHashCode()
				{
					return mask + offset;
				}

		// Convert a section into a string.
		public static String ToString(Section value)
				{
					return "Section{0x" + Convert.ToString(value.mask, 16) +
							", 0x" + Convert.ToString(value.offset, 16);
				}
		public override String ToString()
				{
					return ToString(this);
				}

	}; // struct Section

}; // struct BitVector32

#endif // !ECMA_COMPAT

}; // namespace System.Collections.Specialized
