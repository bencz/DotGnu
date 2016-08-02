/*
 * ImmutableObjectAttribute.cs - Implementation of the
 *		"System.ComponentModel.ImmutableObjectAttribute" class.
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

#if !ECMA_COMPAT

using System;

[AttributeUsage(AttributeTargets.All)]
public sealed class ImmutableObjectAttribute : Attribute
{
	// Internal state.
	private bool immutable;

	// Builtin attribute values.
	public static readonly ImmutableObjectAttribute Default
			= new ImmutableObjectAttribute(false);
	public static readonly ImmutableObjectAttribute No
			= new ImmutableObjectAttribute(false);
	public static readonly ImmutableObjectAttribute Yes
			= new ImmutableObjectAttribute(true);

	// Constructor.
	public ImmutableObjectAttribute(bool immutable)
			{
				this.immutable = immutable;
			}

	// Determine if the installer should be run.
	public bool Immutable
			{
				get
				{
					return immutable;
				}
			}

	// Determine if two object are equal.
	public override bool Equals(Object obj)
			{
				ImmutableObjectAttribute other =
					(obj as ImmutableObjectAttribute);
				if(other != null)
				{
					return (other.immutable == immutable);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return (immutable ? 1 : 0);
			}

	// Determine if this is a default attribute value.
	public override bool IsDefaultAttribute()
			{
				return !immutable;
			}

}; // class ImmutableObjectAttribute

#endif // !ECMA_COMPAT

}; // namespace System.ComponentModel
