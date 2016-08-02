/*
 * TypeConverterAttribute.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.TypeConverterAttribute" class.
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

using System;
using System.Globalization;

[AttributeUsage(AttributeTargets.All)]
public sealed class TypeConverterAttribute : Attribute
{
	// Internal state.
	private String typeName;

	// Pre-defined attribute values.
	public static readonly TypeConverterAttribute Default
			= new TypeConverterAttribute();

	// Constructors.
	public TypeConverterAttribute() : this("") {}
	public TypeConverterAttribute(String typeName)
			{
				if(typeName == null)
				{
					this.typeName = "";
				}
				else
				{
					this.typeName = typeName;
				}
			}
	public TypeConverterAttribute(Type type)
			{
				if(type == null)
				{
					typeName = "";
				}
				else
				{
					typeName = type.FullName;
				}
			}

	// Get the name of the converter type.
	public String ConverterTypeName
			{
				get
				{
					return typeName;
				}
			}

	// Determine if two type converter attributes are equal.
	public override bool Equals(Object obj)
			{
				TypeConverterAttribute a = (obj as TypeConverterAttribute);
				if(a != null)
				{
					return (a.typeName == typeName);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this instance.
	public override int GetHashCode()
			{
				return typeName.GetHashCode();
			}

}; // class TypeConverterAttribute

#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.ComponentModel
