/*
 * ImageConverter.cs - Implementation of the "System.Drawing.ImageConverter" class.
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
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;

	public class ImageConverter : TypeConverter
	{

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(byte[]))
				return true;
			else
				return CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(byte[]))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is byte[])
				return Image.FromStream(new MemoryStream(value as byte[]));
			else
				return base.ConvertFrom(context, culture, value);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(Image), attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value != null)
					return (value as Image).ToString();
				else
					return String.Empty;
			}
			if (destinationType != typeof(byte[]))
				return base.ConvertTo(context, culture, value, destinationType);
			else if (value == null)
				return new byte[0];
			MemoryStream memoryStream = new MemoryStream();
			if ((value as Image).RawFormat == ImageFormat.Icon)
				using (Image image = new Bitmap(value as Image, (value as Image).Width, (value as Image).Height))
					image.Save(memoryStream,(value as Image).RawFormat);
			else
				(value as Image).Save(memoryStream, (value as Image).RawFormat);
			memoryStream.Close();
			return memoryStream.ToArray();
		}
	}
#endif
}
