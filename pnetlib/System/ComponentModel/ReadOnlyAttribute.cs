/*
 * ReadOnlyAttribute.cs - Implementation of the
 *			"System.ComponentModel.ReadOnlyAttribute" class.
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

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

[AttributeUsage(AttributeTargets.All)]
public sealed class ReadOnlyAttribute : Attribute
{
	// Internal state.
	private bool flag;

	// Pre-defined attribute values.
	public static readonly ReadOnlyAttribute Default
			= new ReadOnlyAttribute(false);
	public static readonly ReadOnlyAttribute No
			= new ReadOnlyAttribute(false);
	public static readonly ReadOnlyAttribute Yes
			= new ReadOnlyAttribute(true);

	// Constructors.
	public ReadOnlyAttribute(bool flag)
			{
				this.flag = flag;
			}

	// Get the attribute's value.
	public bool IsReadOnly
			{
				get
				{
					return flag;
				}
			}

	// Determine if two attribute values are equal.
	public override bool Equals(Object obj)
			{
				ReadOnlyAttribute other = (obj as ReadOnlyAttribute);
				if(other != null)
				{
					return (flag == other.flag);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this value.
	public override int GetHashCode()
			{
				return GetType().GetHashCode() + (flag ? 1 : 0);
			}

	// Determine if this is a default attribute value.
	public override bool IsDefaultAttribute()
			{
				return Equals(Default);
			}

}; // class ReadOnlyAttribute

#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.ComponentModel
