/*
 * LocalizableAttribute.cs - Implementation of the
 *			"System.ComponentModel.LocalizableAttribute" class.
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
public sealed class LocalizableAttribute : Attribute
{
	// Internal state.
	private bool localizable;

	// Pre-defined attribute values.
	public static readonly LocalizableAttribute Default
			= new LocalizableAttribute(false);
	public static readonly LocalizableAttribute No
			= new LocalizableAttribute(false);
	public static readonly LocalizableAttribute Yes
			= new LocalizableAttribute(true);

	// Constructor.
	public LocalizableAttribute(bool localizable)
			{
				this.localizable = localizable;
			}

	// Get the attribute's value.
	public bool IsLocalizable
			{
				get
				{
					return localizable;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				LocalizableAttribute other = (obj as LocalizableAttribute);
				if(other != null)
				{
					return (localizable == other.localizable);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return (localizable ? 1 : 0);
			}

	// Determine if this attribute has the default value.
	public override bool IsDefaultAttribute()
			{
				return !localizable;
			}

}; // class LocalizableAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
