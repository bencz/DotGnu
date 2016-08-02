/*
 * CursorConverter.cs - Implementation of the
 *			"System.Windows.Forms.CursorConverter" class.
 *
 * Copyright (C) 2003 Neil Cawse.
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

namespace System.Windows.Forms
{
#if CONFIG_COMPONENT_MODEL
using System;
using System.ComponentModel;
using System.Globalization;

	public class CursorConverter : TypeConverter
	{
		[TODO]
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return ConvertTo(context, culture, value, destinationType);
		}

		[TODO]
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return null;
		}

		[TODO]
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		
		[TODO]
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return CanConvertFrom(context, sourceType);
		}

		[TODO]
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return CanConvertTo(context, destinationType);
		}

		[TODO]
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return ConvertFrom(context, culture, value);
		}

	}
#endif
}
