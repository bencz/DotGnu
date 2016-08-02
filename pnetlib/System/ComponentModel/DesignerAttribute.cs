/*
 * DesignerAttribute.cs - Implementation of the
 *			"System.ComponentModel.DesignerAttribute" class.
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

using System.ComponentModel.Design;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
			    AllowMultiple=true, Inherited=true)]
public sealed class DesignerAttribute : Attribute
{
	// Internal state.
	private String type;
	private String baseType;

	// Constructor.
	public DesignerAttribute(String designerTypeName)
			{
				type = designerTypeName;
			#if CONFIG_COMPONENT_MODEL_DESIGN
				baseType = typeof(IDesigner).FullName;
			#else
				baseType = "System.ComponentModel.Design.IDesigner";
			#endif
			}
	public DesignerAttribute(Type designerType)
			{
				type = designerType.AssemblyQualifiedName;
			#if CONFIG_COMPONENT_MODEL_DESIGN
				baseType = typeof(IDesigner).FullName;
			#else
				baseType = "System.ComponentModel.Design.IDesigner";
			#endif
			}
	public DesignerAttribute(String designerTypeName,
							 String designerBaseTypeName)
			{
				type = designerTypeName;
				baseType = designerBaseTypeName;
			}
	public DesignerAttribute(String designerTypeName,
							 Type designerBaseType)
			{
				type = designerTypeName;
				baseType = designerBaseType.AssemblyQualifiedName;
			}
	public DesignerAttribute(Type designerType,
							 Type designerBaseType)
			{
				type = designerType.AssemblyQualifiedName;
				baseType = designerBaseType.AssemblyQualifiedName;
			}

	// Get the attribute's value.
	public String DesignerBaseTypeName
			{
				get
				{
					return baseType;
				}
			}
	public String DesignerTypeName
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
					String type = baseType;
					int index = type.IndexOf(',');
					if(index != -1)
					{
						type = type.Substring(0, index);
					}
					return GetType().FullName + type;
				}
			}

	// Determine if two instances of this class are equal.
	public override bool Equals(Object obj)
			{
				DesignerAttribute other = (obj as DesignerAttribute);
				if(other != null)
				{
					return (type == other.type &&
							baseType == other.baseType);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this attribute.
	public override int GetHashCode()
			{
				if(type != null)
				{
					return type.GetHashCode();
				}
				else
				{
					return 0;
				}
			}

}; // class DesignerAttribute

#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.ComponentModel
