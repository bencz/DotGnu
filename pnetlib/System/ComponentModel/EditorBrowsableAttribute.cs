/*
 * EditorBrowsableAttribute.cs - Implementation of the
 *			"System.ComponentModel.EditorBrowsableAttribute" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

[AttributeUsage(AttributeTargets.Class |
			    AttributeTargets.Constructor |
				AttributeTargets.Delegate |
				AttributeTargets.Enum |
				AttributeTargets.Event |
				AttributeTargets.Field |
				AttributeTargets.Interface |
				AttributeTargets.Method |
				AttributeTargets.Property |
				AttributeTargets.Struct)]
public sealed class EditorBrowsableAttribute : Attribute
{
	// Internal state.
	private EditorBrowsableState state;

	// Constructors.
	public EditorBrowsableAttribute()
			{
				this.state = EditorBrowsableState.Always;
			}
	public EditorBrowsableAttribute(EditorBrowsableState state)
			{
				this.state = state;
			}

	// Get the attribute's properties.
	public EditorBrowsableState State
			{
				get
				{
					return state;
				}
			}

	// Determine if two instances of this class are equal.
	public override bool Equals(Object obj)
			{
				EditorBrowsableAttribute other =
					(obj as EditorBrowsableAttribute);
				if(other != null)
				{
					return (state == other.state);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this attribute.
	public override int GetHashCode()
			{
				return (int)state;
			}

}; // class EditorBrowsableAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
