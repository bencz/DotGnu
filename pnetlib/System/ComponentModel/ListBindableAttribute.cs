/*
 * ListBindableAttribute.cs - Implementation of the
 *			"System.ComponentModel.ListBindableAttribute" class.
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
public sealed class ListBindableAttribute : Attribute
{
	// Internal state.
	private BindableSupport flags;

	// Pre-defined attribute values.
	public static readonly ListBindableAttribute Default
			= new ListBindableAttribute(BindableSupport.Default);
	public static readonly ListBindableAttribute No
			= new ListBindableAttribute(BindableSupport.No);
	public static readonly ListBindableAttribute Yes
			= new ListBindableAttribute(BindableSupport.Yes);

	// Constructors.
	public ListBindableAttribute(bool listBindable)
			{
				this.flags = (listBindable ? BindableSupport.Yes
										   : BindableSupport.No);
			}
	public ListBindableAttribute(BindableSupport flags)
			{
				this.flags = flags;
			}

	// Get the attribute's value.
	public bool ListBindable
			{
				get
				{
					return (flags == BindableSupport.Yes);
				}
			}

	// Determine if two attribute values are equal.
	public override bool Equals(Object obj)
			{
				ListBindableAttribute other = (obj as ListBindableAttribute);
				if(other != null)
				{
					return (flags == other.flags);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this value.
	public override int GetHashCode()
			{
				return GetType().GetHashCode() + (int)flags;
			}

	// Determine if this is a default attribute value.
	public override bool IsDefaultAttribute()
			{
				return (flags == BindableSupport.Default);
			}

}; // class ListBindableAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
