/*
 * DescriptionAttribute.cs - Implementation of the
 *			"System.ComponentModel.DescriptionAttribute" class.
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

[AttributeUsage(AttributeTargets.All)]
public class DescriptionAttribute : Attribute
{
	// Internal state.
	private String desc;

	// Default description value.
	public static readonly DescriptionAttribute Default
			= new DescriptionAttribute();

	// Constructor.
	public DescriptionAttribute()
			{
				desc = String.Empty;
			}
	public DescriptionAttribute(String description)
			{
				desc = description;
			}

	// Get the attribute's value.
	public virtual String Description
			{
				get
				{
					return DescriptionValue;
				}
			}
	protected String DescriptionValue
			{
				get
				{
					return desc;
				}
				set
				{
					desc = value;
				}
			}

	// Determine if two instances of this class are equal.
	public override bool Equals(Object obj)
			{
				DescriptionAttribute other = (obj as DescriptionAttribute);
				if(other != null)
				{
					return (Description == other.Description);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this attribute.
	public override int GetHashCode()
			{
				String value = Description;
				if(value != null)
				{
					return value.GetHashCode();
				}
				else
				{
					return 0;
				}
			}

}; // class DescriptionAttribute

#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.ComponentModel
