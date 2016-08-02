/*
 * PropertyTabAttribute.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.PropertyTabAttribute" class.
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

[AttributeUsage(AttributeTargets.All)]
public class PropertyTabAttribute : Attribute
{
	// Internal state.
	private Type[] tabClasses;
	private String[] tabClassNames;
	private PropertyTabScope[] tabScopes;

	// Constructors.
	public PropertyTabAttribute()
			{
				tabClasses = new Type [0];
				tabClassNames = new String [0];
				tabScopes = new PropertyTabScope [0];
			}
	public PropertyTabAttribute(String tabClassName)
			{
				InitializeArrays
					(new String[] {tabClassName},
					 new PropertyTabScope[] {PropertyTabScope.Component});
			}
	public PropertyTabAttribute(Type tabClass)
			{
				InitializeArrays
					(new Type[] {tabClass},
					 new PropertyTabScope[] {PropertyTabScope.Component});
			}
	public PropertyTabAttribute(String tabClassName,
								PropertyTabScope tabScope)
			{
				InitializeArrays
					(new String[] {tabClassName},
					 new PropertyTabScope[] {tabScope});
			}
	public PropertyTabAttribute(Type tabClass,
								PropertyTabScope tabScope)
			{
				InitializeArrays
					(new Type[] {tabClass},
					 new PropertyTabScope[] {tabScope});
			}

	// Get this attribute's values.
	public Type[] TabClasses
			{
				get
				{
					return tabClasses;
				}
			}
	public PropertyTabScope[] TabScopes
			{
				get
				{
					return tabScopes;
				}
			}
	protected String[] TabClassNames
			{
				get
				{
					return tabClassNames;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				return Equals(obj as PropertyTabAttribute);
			}
	public bool Equals(PropertyTabAttribute other)
			{
				if(other != null)
				{
					if(other.tabClasses.Length != tabClasses.Length)
					{
						return false;
					}
					int index;
					for(index = 0; index < tabClasses.Length; ++index)
					{
						if(other.tabClasses[index] != tabClasses[index])
						{
							return false;
						}
						if(other.tabClassNames[index] != tabClassNames[index])
						{
							return false;
						}
						if(other.tabScopes[index] != tabScopes[index])
						{
							return false;
						}
					}
					return true;
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

	// Initialize the arrays within this attribute.
	protected void InitializeArrays(String[] tabClassNames,
									PropertyTabScope[] tabScopes)
			{
				if(tabClassNames.Length != tabScopes.Length)
				{
					throw new ArgumentException(S._("Arg_ArraysSameSize"));
				}
				this.tabClassNames = tabClassNames;
				this.tabScopes = tabScopes;
				this.tabClasses = new Type [tabClassNames.Length];
				int index;
				for(index = 0; index < tabClassNames.Length; ++index)
				{
					tabClasses[index] = Type.GetType
						(tabClassNames[index], true);
				}
			}
	protected void InitializeArrays(Type[] tabClasses,
									PropertyTabScope[] tabScopes)
			{
				if(tabClasses.Length != tabScopes.Length)
				{
					throw new ArgumentException(S._("Arg_ArraysSameSize"));
				}
				this.tabClasses = tabClasses;
				this.tabScopes = tabScopes;
				this.tabClassNames = new String [tabClasses.Length];
				int index;
				for(index = 0; index < tabClasses.Length; ++index)
				{
					tabClassNames[index] =
						tabClasses[index].AssemblyQualifiedName;
				}
			}

}; // class PropertyTabAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
