/*
 * DecimalConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.DecimalConverter" class.
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
using System.Reflection;
using System.ComponentModel.Design.Serialization;

public class DecimalConverter : BaseNumberConverter
{
	// Constructor.
	public DecimalConverter()
			{
				// Nothing to do here.
			}

	// Internal conversion from a string.
	internal override Object DoConvertFrom(String value, NumberFormatInfo nfi)
			{
				return Decimal.Parse(value, NumberStyles.Float, nfi);
			}

	// Internal convert to a string.
	internal override String DoConvertTo(Object value, NumberFormatInfo nfi)
			{
				return ((Decimal)value).ToString(null, nfi);
			}

	// Determine if we can convert from this type to a specific type.
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

	// Convert this object into another type.
	public override Object ConvertTo(ITypeDescriptorContext context,
									 CultureInfo culture,
									 Object value, Type destinationType)
			{
				if(destinationType == null)
				{
					throw new ArgumentNullException("destinationType");
				}
			#if CONFIG_COMPONENT_MODEL_DESIGN
				if(destinationType == typeof(InstanceDescriptor) &&
				   value is Decimal)
				{
					ConstructorInfo ctor;
					ctor = typeof(Decimal).GetConstructor
							(new Type [] {typeof(int[])});
					if(ctor == null)
					{
						return null;
					}
					return new InstanceDescriptor
						(ctor, new Object []
							{Decimal.GetBits((Decimal)value)});
				}
			#endif
				return base.ConvertTo(context, culture, value,
									  destinationType);
			}

}; // class DecimalConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
