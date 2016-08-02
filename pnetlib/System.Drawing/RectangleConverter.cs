/*
 * RectangleConverter.cs - Implementation of the
 *			"System.Drawing.Printing.RectangleConverter" class.
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

namespace System.Drawing
{

#if CONFIG_COMPONENT_MODEL

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Collections;

public class RectangleConverter : TypeConverter
{
	// Constructor.
	public RectangleConverter() {}

	// Determine if we can convert from a given type to "Rectangle".
	public override bool CanConvertFrom
				(ITypeDescriptorContext context, Type sourceType)
			{
				if(sourceType == typeof(String))
				{
					return true;
				}
				else
				{
					return base.CanConvertFrom(context, sourceType);
				}
			}

	// Determine if we can convert to a given type from "Rectangle".
	public override bool CanConvertTo
				(ITypeDescriptorContext context, Type destinationType)
			{
			#if CONFIG_COMPONENT_MODEL_DESIGN
				if(destinationType == typeof(InstanceDescriptor))
				{
					return true;
				}
				else
			#endif
				{
					return base.CanConvertTo(context, destinationType);
				}
			}

	// Convert from a source type to "Rectangle".
	public override Object ConvertFrom
				(ITypeDescriptorContext context,
				 CultureInfo culture, Object value)
			{
				// Pass control to the base class if we weren't given a string.
				if(!(value is String))
				{
					return base.ConvertFrom(context, culture, value);
				}

				// Extract the string and trim it.
				String str = ((String)value).Trim();
				if(str.Length == 0)
				{
					return null;
				}

				// Parse "x, y, width, height" components from the string.
				String[] components = str.Split(',');
				if(components.Length != 4)
				{
					throw new ArgumentException(S._("Arg_InvalidRectangle"));
				}
				TypeConverter converter;
				converter = TypeDescriptor.GetConverter(typeof(int));
				int x = (int)(converter.ConvertFrom(components[0]));
				int y = (int)(converter.ConvertFrom(components[1]));
				int width = (int)(converter.ConvertFrom(components[2]));
				int height = (int)(converter.ConvertFrom(components[3]));
				return new Rectangle(x, y, width, height);
			}

	// Convert from "Rectangle" to a destination type.
	[TODO]
	public override Object ConvertTo
				(ITypeDescriptorContext context,
				 CultureInfo culture, Object value,
				 Type destinationType)
			{
				Rectangle rect = (Rectangle)value;
				if(destinationType == typeof(String))
				{
					return String.Format("{0}, {1}, {2}, {3}",
										 rect.X, rect.Y,
										 rect.Width, rect.Height);
				}
			#if CONFIG_COMPONENT_MODEL_DESIGN
				else if(destinationType == typeof(InstanceDescriptor))
				{
					// TODO
					return null;
				}
			#endif
				else
				{
					return base.ConvertTo
						(context, culture, value, destinationType);
				}
			}

	// Create an instance of this type of object.
	public override Object CreateInstance
				(ITypeDescriptorContext context, IDictionary propertyValues)
			{
				return new Rectangle((int)(propertyValues["X"]),
								     (int)(propertyValues["Y"]),
								     (int)(propertyValues["Width"]),
								     (int)(propertyValues["Height"]));
			}

	// Determine if creating new instances is supported.
	public override bool GetCreateInstanceSupported
				(ITypeDescriptorContext context)
			{
				return true;
			}

	// Get the properties for an object.
	public override PropertyDescriptorCollection GetProperties
				(ITypeDescriptorContext context, Object value,
				 Attribute[] attributes)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Rectangle), attributes);
				return properties.Sort(new String[4] { "X", "Y", "Width", "Height" });
			}

	// Determine if the "GetProperties" method is supported.
	public override bool GetPropertiesSupported
				(ITypeDescriptorContext context)
			{
				return true;
			}

}; // class RectangleConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.Drawing
