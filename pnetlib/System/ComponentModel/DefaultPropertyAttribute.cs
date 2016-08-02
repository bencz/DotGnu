/*
 * DefaultPropertyAttribute.cs - Implementation of the
 *			"System.ComponentModel.DefaultPropertyAttribute" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

[AttributeUsage(AttributeTargets.Class)]
public sealed class DefaultPropertyAttribute : Attribute
{
	// Internal state.
	private String name;

	// Constructor.
	public DefaultPropertyAttribute(String name)
			{
				this.name = name;
			}

	// Get the attribute's value.
	public String Name
			{
				get
				{
					return name;
				}
			}

	// Determine if two instances of this class are equal.
	public override bool Equals(Object obj)
			{
				DefaultPropertyAttribute other =
						(obj as DefaultPropertyAttribute);
				if(other != null)
				{
					return (name == other.name);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this attribute.
	public override int GetHashCode()
			{
				return name.GetHashCode();
			}

	// Predefined instances of this class.
	public static readonly DefaultPropertyAttribute Default =
		new DefaultPropertyAttribute(null);

}; // class DefaultPropertyAttribute

#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.ComponentModel
