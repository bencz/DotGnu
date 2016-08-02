/*
 * ToolboxItemFilterAttribute.cs - Implementation of the
 *			"System.ComponentModel.ToolboxItemFilterAttribute" class.
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

[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
public sealed class ToolboxItemFilterAttribute : Attribute
{
	// Internal state.
	private String filterString;
	private ToolboxItemFilterType filterType;

	// Constructors.
	public ToolboxItemFilterAttribute(String filterString)
			{
				this.filterString = filterString;
			}
	public ToolboxItemFilterAttribute(String filterString,
									  ToolboxItemFilterType filterType)
			{
				this.filterString = filterString;
				this.filterType = filterType;
			}

	// Get the attribute's properties.
	public ToolboxItemFilterType FilterType
			{
				get
				{
					return filterType;
				}
			}
	public String FilterString
			{
				get
				{
					return filterString;
				}
			}
	public override Object TypeId
			{
				get
				{
					return GetType().FullName + filterString;
				}
			}

	// Determine if two attribute values are equal.
	public override bool Equals(Object obj)
			{
				ToolboxItemFilterAttribute other =
					(obj as ToolboxItemFilterAttribute);
				if(other != null)
				{
					return (filterType == other.filterType &&
							filterString == other.filterString);
				}
				else
				{
					return false;
				}
			}

	// Determine if two attribute values have matching filter strings.
	public override bool Match(Object obj)
			{
				ToolboxItemFilterAttribute other =
					(obj as ToolboxItemFilterAttribute);
				if(other != null)
				{
					return (filterString == other.filterString);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this value.
	public override int GetHashCode()
			{
				if(filterString != null)
				{
					return filterString.GetHashCode();
				}
				else
				{
					return 0;
				}
			}

}; // class ToolboxItemFilterAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
