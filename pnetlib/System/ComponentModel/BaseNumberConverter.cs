/*
 * BaseNumberConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.BaseNumberConverter" class.
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

public abstract class BaseNumberConverter : TypeConverter
{
	// Constructor.
	protected BaseNumberConverter()
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

	// Determine if we can convert from this type to a specific type.
	public override bool CanConvertTo
				(ITypeDescriptorContext context, Type destinationType)
			{
				if(destinationType.IsPrimitive)
				{
					return true;
				}
				else
				{
					return base.CanConvertTo(context, destinationType);
				}
			}

	// Internal conversion from a string.
	internal abstract Object DoConvertFrom(String value, NumberFormatInfo nfi);
	internal virtual Object DoConvertFromHex(String value)
			{
				throw new FormatException();
			}

	// Convert from another type to the one represented by this class.
	public override Object ConvertFrom(ITypeDescriptorContext context,
									   CultureInfo culture,
									   Object value)
			{
				String val = (value as String);
				if(val != null)
				{
					val = val.Trim();
					if(val.StartsWith("0x") || val.StartsWith("0X") ||
					   val.StartsWith("&h") || val.StartsWith("&H"))
					{
						return DoConvertFromHex(val.Substring(2));
					}
					else if(val.StartsWith("#"))
					{
						return DoConvertFromHex(val.Substring(1));
					}
					else
					{
						return DoConvertFrom
							(val, NumberFormatInfo.GetInstance(culture));
					}
				}
				else
				{
					return base.ConvertFrom(context, culture, value);
				}
			}

	// Internal convert to a string.
	internal abstract String DoConvertTo(Object value, NumberFormatInfo nfi);

	// Convert this object into another type.
	public override Object ConvertTo(ITypeDescriptorContext context,
									 CultureInfo culture,
									 Object value, Type destinationType)
			{
				if(destinationType == null)
				{
					throw new ArgumentNullException("destinationType");
				}
				if(destinationType == typeof(String))
				{
					if(value != null)
					{
						return DoConvertTo
							(value, NumberFormatInfo.GetInstance(culture));
					}
					else
					{
						return String.Empty;
					}
				}
				else if(destinationType.IsPrimitive)
				{
					return Convert.ChangeType(value, destinationType);
				}
				else
				{
					return base.ConvertTo(context, culture, value,
										  destinationType);
				}
			}

}; // class BaseNumberConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
