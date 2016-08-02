/*
 * IconConverter.cs - Implementation of the "System.Drawing.IconConverter" class.
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
using System.Globalization;
using System.IO;

	public class IconConverter : ExpandableObjectConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(byte[]))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
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
				return new Icon(new MemoryStream(value as byte[]));
			else
				return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType != typeof(byte[]))
				return base.ConvertTo(context, culture, value, destinationType);
			else if (destinationType == typeof(string))
				return (value as Icon).ToString();
			else if (value == null)
				return new byte[0];
			else
			{
				MemoryStream memoryStream = new MemoryStream();
				(value as Icon).Save(memoryStream);
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}
	}
#endif
}