/*
 * ArrayWithOffset.cs - Implementation of the
 *			"System.Runtime.InteropServices.ArrayWithOffset" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

public struct ArrayWithOffset
{
	// Internal state.
	private Object array;
	private int offset;

	// Constructor.
	public ArrayWithOffset(Object array, int offset)
			{
				this.array = array;
				this.offset = offset;
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				if(obj is ArrayWithOffset)
				{
					ArrayWithOffset other = (ArrayWithOffset)obj;
					return (array == other.array && offset == other.offset);
				}
				else
				{
					return false;
				}
			}

	// Get the array from this structure.
	public Object GetArray()
			{
				return array;
			}

	// Get a hash code for this instance.
	public override int GetHashCode()
			{
				return offset;
			}

	// Get the offset from this structure.
	public int GetOffset()
			{
				return offset;
			}

}; // struct ArrayWithOffset

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
