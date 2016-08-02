/*
 * RootDesignerSerializerAttribute.cs - Implementation of
 * "System.ComponentModel.Design.Serialization.RootDesignerSerializerAttribute"
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

namespace System.ComponentModel.Design.Serialization
{

#if CONFIG_COMPONENT_MODEL_DESIGN

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
				AllowMultiple=true, Inherited=true)]
public sealed class RootDesignerSerializerAttribute : Attribute
{
	// Internal state.
	private String serializerTypeName;
	private String baseSerializerTypeName;
	private bool reloadable;

	// Constructors.
	public RootDesignerSerializerAttribute
				(String serializerTypeName, String baseSerializerTypeName,
				 bool reloadable)
			{
				this.serializerTypeName = serializerTypeName;
				this.baseSerializerTypeName = baseSerializerTypeName;
				this.reloadable = reloadable;
			}
	public RootDesignerSerializerAttribute
				(String serializerTypeName, Type baseSerializerType,
				 bool reloadable)
			{
				this.serializerTypeName = serializerTypeName;
				this.baseSerializerTypeName =
					baseSerializerType.AssemblyQualifiedName;
				this.reloadable = reloadable;
			}
	public RootDesignerSerializerAttribute
				(Type serializerType, Type baseSerializerType,
				 bool reloadable)
			{
				this.serializerTypeName =
					serializerType.AssemblyQualifiedName;
				this.baseSerializerTypeName =
					baseSerializerType.AssemblyQualifiedName;
				this.reloadable = reloadable;
			}

	// Get this attribute's properties.
	public bool Reloadable
			{
				get
				{
					return reloadable;
				}
			}
	public String SerializerBaseTypeName
			{
				get
				{
					return baseSerializerTypeName;
				}
			}
	public String SerializerTypeName
			{
				get
				{
					return serializerTypeName;
				}
			}
	public override Object TypeId
			{
				get
				{
					return base.TypeId;
				}
			}

}; // class RootDesignerSerializerAttribute

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design.Serialization
