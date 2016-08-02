/*
 * ImageIndexConverter.cs - Implementation of the
 *			"System.Windows.Forms.ImageIndexConverter" class.
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

namespace System.Windows.Forms
{

#if !CONFIG_COMPACT_FORMS

using System.ComponentModel;
using System.Globalization;

[TODO]
public class ImageIndexConverter : Int32Converter
{

	// constructor
	[TODO]
	public ImageIndexConverter(): base()
	{
	}

	[TODO]
	public override Object ConvertFrom(ITypeDescriptorContext context, 
					CultureInfo culture, Object value)	
	{
		return base.ConvertFrom(context, culture, value);
	}

	[TODO]
	public override Object ConvertTo(ITypeDescriptorContext context,
					 CultureInfo culture, Object value, Type destinationType)
	{
		return base.ConvertTo(context, culture, value, destinationType);
	}

	[TODO]
	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		return base.GetStandardValues(context);
	}

	[TODO]
	public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
	{
		return base.GetStandardValuesExclusive(context);
	}

	[TODO]
	public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	{
		return base.GetStandardValuesSupported(context);
	}

	[TODO]
	protected virtual bool IncludeNoneAsStandardValue
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

}; // class ImageIndexConverter

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
