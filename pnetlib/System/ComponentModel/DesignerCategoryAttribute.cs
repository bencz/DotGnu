/*
 * DesignerCategoryAttribute.cs - Implementation of the
 *			"System.ComponentModel.DesignerCategoryAttribute" class.
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

[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
public sealed class DesignerCategoryAttribute : Attribute
{
	// Internal state.
	private String category;

	// Pre-defined attribute values.
	public static readonly DesignerCategoryAttribute Component =
			new DesignerCategoryAttribute("Component");
	public static readonly DesignerCategoryAttribute Default =
			new DesignerCategoryAttribute();
	public static readonly DesignerCategoryAttribute Form =
			new DesignerCategoryAttribute("Form");
	public static readonly DesignerCategoryAttribute Generic =
			new DesignerCategoryAttribute("Generic");

	// Constructors.
	public DesignerCategoryAttribute()
			{
				this.category = String.Empty;
			}
	public DesignerCategoryAttribute(String category)
			{
				this.category = category;
			}

	// Get the attribute's value.
	public String Category
			{
				get
				{
					return category;
				}
			}

	// Get the type identifier for this attribute.
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
				DesignerCategoryAttribute other =
						(obj as DesignerCategoryAttribute);
				if(other != null)
				{
					return (category == other.category);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this attribute.
	public override int GetHashCode()
			{
				if(category != null)
				{
					return category.GetHashCode();
				}
				else
				{
					return 0;
				}
			}

	// Determine if this is the default attribute value.
	public override bool IsDefaultAttribute()
			{
				return (category == null || category == String.Empty);
			}

}; // class DesignerCategoryAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
