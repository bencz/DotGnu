/*
 * LinkArea.cs - Implementation of the
 *		"System.Windows.Forms.LinkArea" class.
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

namespace System.Windows.Forms
{

public struct LinkArea
{
	// Internal state.
	private int start;
	private int length;

	// Constructor.
	public LinkArea(int start, int length)
			{
				this.start = start;
				this.length = length;
			}

	// Get or set this object's properties.
	public bool IsEmpty
			{
				get
				{
					return (start == 0 && length == 0);
				}
			}
	public int Start
			{
				get
				{
					return start;
				}
				set
				{
					start = value;
				}
			}
	public int Length
			{
				get
				{
					return length;
				}
				set
				{
					length = value;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object o)
			{
				if(o is LinkArea)
				{
					LinkArea link = (LinkArea)o;
					return (start == link.start && length == link.length);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return ((start << 8) ^ length);
			}

}; // struct LinkArea

}; // namespace System.Windows.Forms
