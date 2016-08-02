/*
 * ToolboxItemAttribute.cs - Implementation of the
 *			"System.ComponentModel.ToolboxItemAttribute" class.
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
public class ToolboxItemAttribute : Attribute
{
	// Internal state.
	private Type toolboxItemType;
	private String toolboxItemTypeName;

	// Pre-defined attribute values.
	public static readonly ToolboxItemAttribute Default
			= new ToolboxItemAttribute(true);
	public static readonly ToolboxItemAttribute None
			= new ToolboxItemAttribute(false);

	// Constructors.
	public ToolboxItemAttribute(bool defaultType)
			{
				if(defaultType)
				{
					toolboxItemTypeName =
						"System.Drawing.Design.ToolboxItem,System.Drawing";
				}
			}
	public ToolboxItemAttribute(String toolboxItemName)
			{
				this.toolboxItemTypeName = toolboxItemName;
			}
	public ToolboxItemAttribute(Type toolboxItemType)
			{
				this.toolboxItemType = toolboxItemType;
			}

	// Get the attribute's value.
	public Type ToolboxItemType
			{
				get
				{
					if(toolboxItemType != null)
					{
						return toolboxItemType;
					}
					else
					{
						toolboxItemType = Type.GetType(toolboxItemTypeName);
						return toolboxItemType;
					}
				}
			}
	public String ToolboxItemTypeName
			{
				get
				{
					if(toolboxItemTypeName != null)
					{
						return toolboxItemTypeName;
					}
					else if(toolboxItemType != null)
					{
						toolboxItemTypeName =
							toolboxItemType.AssemblyQualifiedName;
						return toolboxItemTypeName;
					}
					else
					{
						return String.Empty;
					}
				}
			}

	// Determine if two attribute values are equal.
	public override bool Equals(Object obj)
			{
				ToolboxItemAttribute other = (obj as ToolboxItemAttribute);
				if(other != null)
				{
					return (ToolboxItemTypeName == other.ToolboxItemTypeName);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this value.
	public override int GetHashCode()
			{
				return ToolboxItemTypeName.GetHashCode();
			}

	// Determine if this is a default attribute value.
	public override bool IsDefaultAttribute()
			{
				return (ToolboxItemTypeName ==
						  "System.Drawing.Design.ToolboxItem,System.Drawing");
			}

}; // class ToolboxItemAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
