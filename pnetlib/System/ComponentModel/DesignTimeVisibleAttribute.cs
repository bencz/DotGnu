/*
 * DesignTimeVisibleAttribute.cs - Implementation of the
 *			"System.ComponentModel.DesignTimeVisibleAttribute" class.
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

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class DesignTimeVisibleAttribute : Attribute
{
	// Internal state.
	private bool visible;

	// Pre-defined values.
	public static readonly DesignTimeVisibleAttribute No =
			new DesignTimeVisibleAttribute(false);
	public static readonly DesignTimeVisibleAttribute Yes =
			new DesignTimeVisibleAttribute(true);
	public static readonly DesignTimeVisibleAttribute Default = Yes;

	// Constructors.
	public DesignTimeVisibleAttribute()
			{
				visible = true;
			}
	public DesignTimeVisibleAttribute(bool visible)
			{
				this.visible = visible;
			}

	// Get this attribute's value.
	public bool Visible 
			{
				get
				{
					return visible;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object value)
			{
				DesignTimeVisibleAttribute other;
				other = (value as DesignTimeVisibleAttribute);
				if(other != null)
				{
					return (other.visible == visible);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return visible.GetHashCode();
			}

	// Determine if this attribute has the default value.
	public override bool IsDefaultAttribute()
			{
				return visible;
			}

}; // class DesignTimeVisibleAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
