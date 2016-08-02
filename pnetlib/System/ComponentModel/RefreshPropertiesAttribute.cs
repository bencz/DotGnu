/*
 * RefreshPropertiesAttribute.cs - Implementation of the
 *			"System.ComponentModel.RefreshPropertiesAttribute" class.
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
public sealed class RefreshPropertiesAttribute : Attribute
{
	// Internal state.
	private RefreshProperties refresh;

	// Pre-defined attribute values.
	public static readonly RefreshPropertiesAttribute All
			= new RefreshPropertiesAttribute(RefreshProperties.All);
	public static readonly RefreshPropertiesAttribute Default
			= new RefreshPropertiesAttribute(RefreshProperties.None);
	public static readonly RefreshPropertiesAttribute Repaint
			= new RefreshPropertiesAttribute(RefreshProperties.Repaint);

	// Constructor.
	public RefreshPropertiesAttribute(RefreshProperties refresh)
			{
				this.refresh = refresh;
			}

	// Get the attribute's value.
	public RefreshProperties RefreshProperties
			{
				get
				{
					return refresh;
				}
			}

	// Determine if two attribute values are equal.
	public override bool Equals(Object obj)
			{
				RefreshPropertiesAttribute other =
						(obj as RefreshPropertiesAttribute);
				if(other != null)
				{
					return (refresh == other.refresh);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this value.
	public override int GetHashCode()
			{
				return GetType().GetHashCode() + (int)refresh;
			}

	// Determine if this is a default attribute value.
	public override bool IsDefaultAttribute()
			{
				return (refresh == RefreshProperties.None);
			}

}; // class RefreshPropertiesAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
