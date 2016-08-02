/*
 * MergablePropertyAttribute.cs - Implementation of the
 *			"System.ComponentModel.MergablePropertyAttribute" class.
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

[AttributeUsage(AttributeTargets.All)]
public sealed class MergablePropertyAttribute : Attribute
{
	// Internal state.
	private bool allowMerge;

	// Pre-defined attribute values.
	public static readonly MergablePropertyAttribute Default
			= new MergablePropertyAttribute(false);
	public static readonly MergablePropertyAttribute No
			= new MergablePropertyAttribute(false);
	public static readonly MergablePropertyAttribute Yes
			= new MergablePropertyAttribute(true);

	// Constructor.
	public MergablePropertyAttribute(bool allowMerge)
			{
				this.allowMerge = allowMerge;
			}

	// Get the attribute's value.
	public bool AllowMerge
			{
				get
				{
					return allowMerge;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				MergablePropertyAttribute other =
					(obj as MergablePropertyAttribute);
				if(other != null)
				{
					return (allowMerge == other.allowMerge);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return (allowMerge ? 1 : 0);
			}

	// Determine if this attribute has the default value.
	public override bool IsDefaultAttribute()
			{
				return !allowMerge;
			}

}; // class MergablePropertyAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
