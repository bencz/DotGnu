/*
 * LicenseProviderAttribute.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.LicenseProviderAttribute" class.
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

using System;

[AttributeUsage(AttributeTargets.Class,
				AllowMultiple=false, Inherited=false)]
public sealed class LicenseProviderAttribute : Attribute
{
	// Internal state.
	private Type type;

	// The default attribute value.
	public static readonly LicenseProviderAttribute Default =
			new LicenseProviderAttribute();

	// Constructors.
	public LicenseProviderAttribute() {}
	public LicenseProviderAttribute(String typeName)
			{
				this.type = Type.GetType(typeName);
			}
	public LicenseProviderAttribute(Type type)
			{
				this.type = type;
			}

	// Get this attribute's values.
	public Type LicenseProvider
			{
				get
				{
					return type;
				}
			}
	public override Object TypeId
			{
				get
				{
					if(type != null)
					{
						return GetType().FullName + type.FullName;
					}
					else
					{
						return GetType().FullName;
					}
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				LicenseProviderAttribute other =
					(obj as LicenseProviderAttribute);
				if(other != null)
				{
					return (other.type == type);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}

}; // class LicenseProviderAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
