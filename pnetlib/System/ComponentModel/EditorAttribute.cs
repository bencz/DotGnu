/*
 * EditorAttribute.cs - Implementation of the
 *			"System.ComponentModel.EditorAttribute" class.
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

[AttributeUsage(AttributeTargets.All, AllowMultiple=true, Inherited=true)]
public sealed class EditorAttribute : Attribute
{
	// Internal state.
	private String typeName;
	private String baseTypeName;

	// Constructors.
	public EditorAttribute()
			{
				this.typeName = String.Empty;
			}
	public EditorAttribute(String typeName, String baseTypeName)
			{
				this.typeName = typeName;
				this.baseTypeName = baseTypeName;
			}
	public EditorAttribute(String typeName, Type baseType)
			{
				this.typeName = typeName;
				this.baseTypeName = baseType.AssemblyQualifiedName;
			}
	public EditorAttribute(Type type, Type baseType)
			{
				this.typeName = type.AssemblyQualifiedName;
				this.baseTypeName = baseType.AssemblyQualifiedName;
			}

	// Get the attribute's properties.
	public String EditorBaseTypeName
			{
				get
				{
					return baseTypeName;
				}
			}
	public String EditorTypeName
			{
				get
				{
					return typeName;
				}
			}
	public override Object TypeId
			{
				get
				{
					return GetType();
				}
			}

	// Determine if two instances of this class are equal.
	public override bool Equals(Object obj)
			{
				EditorAttribute other = (obj as EditorAttribute);
				if(other != null)
				{
					return (typeName == other.typeName &&
							baseTypeName == other.baseTypeName);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this attribute.
	public override int GetHashCode()
			{
				if(typeName != null)
				{
					return typeName.GetHashCode();
				}
				else
				{
					return 0;
				}
			}

}; // class EditorAttribute

#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.ComponentModel
