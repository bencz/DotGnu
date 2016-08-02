/*
 * Label.cs - Implementation of the
 *			"System.Reflection.Emit.Label" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

public struct Label
{
	// Internal state.
	internal int index;

	// Constructor.
	internal Label(int index)
			{
				this.index = index;
			}

	// Determine if this label is identical to another.
	public override bool Equals(Object obj)
			{
				if(obj is Label)
				{
					return (index == ((Label)obj).index);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this token.
	public override int GetHashCode()
			{
				return index;
			}

}; // struct Label

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
