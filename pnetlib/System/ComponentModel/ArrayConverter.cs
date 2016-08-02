/*
 * ArrayConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.ArrayConverter" class.
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
using System.Collections;
using System.Globalization;

public class ArrayConverter : CollectionConverter
{
	// Constructor.
	public ArrayConverter() {}

	// Convert this object into another type.
	public override Object ConvertTo(ITypeDescriptorContext context,
									 CultureInfo culture,
									 Object value, Type destinationType)
			{
				if(destinationType == null)
				{
					throw new ArgumentNullException("destinationType");
				}
				else if(destinationType == typeof(String) &&
						value is Array)
				{
					return value.GetType().ToString() + " Array";
				}
				else
				{
					return base.ConvertTo(context, culture, value,
										  destinationType);
				}
			}

	// Get the properties for an object.
	public override PropertyDescriptorCollection GetProperties
				(ITypeDescriptorContext context, Object value,
				 Attribute[] attributes)
			{
				Array array = (value as Array);
				if(array == null)
				{
					return new PropertyDescriptorCollection(null);
				}
				int len = array.GetLength(0);
				PropertyDescriptor[] descs;
				descs = new PropertyDescriptor [len];
				int index;
				for(index = 0; index < len; ++index)
				{
					descs[index] = new ArrayElementDescriptor
						(array.GetType(),
						 array.GetType().GetElementType(),
						 index, "[" + index.ToString() + "]");
				}
				return new PropertyDescriptorCollection(descs);
			}

	// Determine if the "GetProperties" method is supported.
	public override bool GetPropertiesSupported
				(ITypeDescriptorContext context)
			{
				return true;
			}

	// Descriptor for an array element.
	private sealed class ArrayElementDescriptor
			: TypeConverter.SimplePropertyDescriptor
	{
		// Internal state.
		private int index;

		// Constructor.
		public ArrayElementDescriptor(Type componentType,
									  Type propertyType,
									  int index, String name)
				: base(componentType, name, propertyType)
				{
					this.index = index;
				}

		// Get the property value associated with a component.
		public override Object GetValue(Object component)
				{
					Array array = (component as Array);
					if(array != null)
					{
						if(index < array.GetLength(0))
						{
							return array.GetValue(index);
						}
					}
					return null;
				}

		// Set the property value associated with a component.
		public override void SetValue(Object component, Object value)
				{
					Array array = (component as Array);
					if(array != null)
					{
						if(index < array.GetLength(0))
						{
							array.SetValue(value, index);
						}
						OnValueChanged(component, EventArgs.Empty);
					}
				}

	}; // class ArrayElementDescriptor

}; // class ArrayConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
