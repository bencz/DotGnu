/*
 * ExtenderProvidedPropertyAttribute.cs - Implementation of
 *	"System.ComponentModel.ComponentModel.ExtenderProvidedPropertyAttribute".
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
public sealed class ExtenderProvidedPropertyAttribute : Attribute
{
	// Internal state.
	private PropertyDescriptor extenderProperty;
	private IExtenderProvider provider;
	private Type receiverType;

	// Constructors.
	public ExtenderProvidedPropertyAttribute() {}
	internal ExtenderProvidedPropertyAttribute
				(PropertyDescriptor extenderProperty,
				 IExtenderProvider provider, Type receiverType)
			{
				this.extenderProperty = extenderProperty;
				this.provider = provider;
				this.receiverType = receiverType;
			}

	// Get this object's properties.
	public PropertyDescriptor ExtenderProperty
			{
				get
				{
					return extenderProperty;
				}
			}
	public IExtenderProvider Provider
			{
				get
				{
					return provider;
				}
			}
	public Type ReceiverType
			{
				get
				{
					return receiverType;
				}
			}

	// Determine if this attribute is a default value.
	public override bool IsDefaultAttribute()
			{
				return (receiverType == null);
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				ExtenderProvidedPropertyAttribute other;
				other = (obj as ExtenderProvidedPropertyAttribute);
				if(other != null)
				{
					if(extenderProperty != null)
					{
						if(!extenderProperty.Equals(other.extenderProperty))
						{
							return false;
						}
					}
					else if(other.extenderProperty != null)
					{
						return false;
					}
					if(provider != null)
					{
						if(!provider.Equals(other.provider))
						{
							return false;
						}
					}
					else if(other.provider != null)
					{
						return false;
					}
					if(receiverType != null)
					{
						if(!receiverType.Equals(other.receiverType))
						{
							return false;
						}
					}
					else if(other.receiverType != null)
					{
						return false;
					}
					return true;
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}

}; // class ExtenderProvidedPropertyAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
