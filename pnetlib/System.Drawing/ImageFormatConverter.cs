/*
 * ImageFormatConverter.cs - Implementation of the "System.Drawing.ImageFormatConverter" class.
 *
 * Copyright (C) 2003  Neil Cawse.
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

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;

	public class ImageFormatConverter : TypeConverter
	{
		private StandardValuesCollection values;

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
		#if CONFIG_COMPONENT_MODEL_DESIGN
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			else
		#endif
				return base.CanConvertTo(context, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is String)
			{
				PropertyInfo[] properties = GetProperties();
				for (int i = 0; i < properties.Length; i++)
				{
					if (String.Compare(properties[i].Name, value as String, true, CultureInfo.InvariantCulture) == 0)
						return properties[i].GetValue(null, null as object[]);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is ImageFormat)
			{
				PropertyInfo[] properties = GetProperties();
				PropertyInfo property = null;
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].GetValue(null, null) == value)
					{
						property = properties[i];
						break;
					}
				}
				if (property != null)
				{
					if (destinationType == typeof(string))
						return property.Name;
				#if CONFIG_COMPONENT_MODEL_DESIGN
					else if (destinationType == typeof(InstanceDescriptor))
						return new InstanceDescriptor(property, null);
				#endif
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (values == null)
			{
				PropertyInfo[] properties = GetProperties();
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < properties.Length; i++)
					arrayList.Add(properties[i].GetValue(null, null as object[]));
				values = new StandardValuesCollection
					((ICollection)(arrayList.ToArray()));
			}
			return values;
		}

		private PropertyInfo[] GetProperties()
		{
			return typeof(ImageFormat).GetProperties(BindingFlags.Public | BindingFlags.Static);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
#endif
}
