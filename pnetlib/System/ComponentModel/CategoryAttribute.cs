/*
 * CategoryAttribute.cs - Implementation of the
 *			"System.ComponentModel.CategoryAttribute" class.
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

using System.Collections;

[AttributeUsage(AttributeTargets.All)]
public class CategoryAttribute : Attribute
{
	// Internal state.
	private String category;
	private bool alreadyLocalized;

	// Constructor.
	public CategoryAttribute()
			{
				category = "Default";
				alreadyLocalized = false;
			}
	public CategoryAttribute(String category)
			{
				this.category = category;
				alreadyLocalized = false;
			}

	// Get the attribute's value.
	public String Category
			{
				get
				{
					if(alreadyLocalized)
					{
						return category;
					}
					alreadyLocalized = true;
					String newCat = GetLocalizedString(category);
					if(newCat != null)
					{
						category = newCat;
					}
					return category;
				}
			}

	// Get the localized form of a category name.
	protected virtual String GetLocalizedString(String value)
			{
				return S._("Category_" + value);
			}

	// Determine if two instances of this class are equal.
	public override bool Equals(Object obj)
			{
				CategoryAttribute other = (obj as CategoryAttribute);
				if(other != null)
				{
					return (Category == other.Category);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this attribute.
	public override int GetHashCode()
			{
				return Category.GetHashCode();
			}

	// Determine if this is the default attribute value.
	public override bool IsDefaultAttribute()
			{
				return Equals(Default);
			}

	// Get a predefined instance.
	private static Hashtable stdCategories = null;
	private static CategoryAttribute GetStdCategory(String name)
			{
				lock(typeof(CategoryAttribute))
				{
					CategoryAttribute attr;
					if(stdCategories == null)
					{
						stdCategories = new Hashtable();
					}
					else
					{
						attr = (CategoryAttribute)(stdCategories[name]);
						if(attr != null)
						{
							return attr;
						}
					}
					attr = new CategoryAttribute(name);
					stdCategories[name] = attr;
					return attr;
				}
			}

	// Predefined instances of this class.
	public static CategoryAttribute Action
			{
				get
				{
					return GetStdCategory("Action");
				}
			}
	public static CategoryAttribute Appearance
			{
				get
				{
					return GetStdCategory("Appearance");
				}
			}
	public static CategoryAttribute Behavior
			{
				get
				{
					return GetStdCategory("Behavior");
				}
			}
	public static CategoryAttribute Data
			{
				get
				{
					return GetStdCategory("Data");
				}
			}
	public static CategoryAttribute Default
			{
				get
				{
					return GetStdCategory("Default");
				}
			}
	public static CategoryAttribute Design
			{
				get
				{
					return GetStdCategory("Design");
				}
			}
	public static CategoryAttribute DragDrop
			{
				get
				{
					return GetStdCategory("DragDrop");
				}
			}
	public static CategoryAttribute Focus
			{
				get
				{
					return GetStdCategory("Focus");
				}
			}
	public static CategoryAttribute Format
			{
				get
				{
					return GetStdCategory("Format");
				}
			}
	public static CategoryAttribute Key
			{
				get
				{
					return GetStdCategory("Key");
				}
			}
	public static CategoryAttribute Layout
			{
				get
				{
					return GetStdCategory("Layout");
				}
			}
	public static CategoryAttribute Mouse
			{
				get
				{
					return GetStdCategory("Mouse");
				}
			}
	public static CategoryAttribute WindowStyle
			{
				get
				{
					return GetStdCategory("WindowStyle");
				}
			}

}; // class CategoryAttribute

#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.ComponentModel
