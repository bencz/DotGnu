/*
 * StringConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.StringConverter" class.
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

public class StringConverter : TypeConverter
{
	// Constructor.
	public StringConverter()
			{
				// Nothing to do here.
			}

	// Determine if we can convert from a specific type to this one.
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

	// Convert from another type to the one represented by this class.
	public override Object ConvertFrom(ITypeDescriptorContext context,
									   CultureInfo culture,
									   Object value)
			{
				if(value == null)
				{
					return String.Empty;
				}
				else if(value is String)
				{
					return (String)value;
				}
				else
				{
					return base.ConvertFrom(context, culture, value);
				}
			}

}; // class StringConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
