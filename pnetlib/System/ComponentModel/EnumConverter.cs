/*
 * EnumConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.EnumConverter" class.
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

public class EnumConverter : TypeConverter
{
	// Internal state.
	private Type type;
	private StandardValuesCollection standardValues;

	// Constructor.
	public EnumConverter(Type type)
			{
				this.type = type;
			}

	// Get a comparer to use to check the values.
	protected virtual IComparer Comparer
			{
				get
				{
					return Collections.Comparer.DefaultInvariant;
				}
			}

	// Get the enumerated type underlying this converter.
	protected Type EnumType
			{
				get
				{
					return type;
				}
			}

	// Get or set the standard value list.
	protected StandardValuesCollection Values
			{
				get
				{
					return standardValues;
				}
				set
				{
					standardValues = value;
				}
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
			#if CONFIG_COMPONENT_MODEL_DESIGN
				if(destinationType == typeof(InstanceDescriptor))
				{
					return true;
				}
			#endif
				return base.CanConvertTo(context, destinationType);
			}

	// Convert from another type to the one represented by this class.
	public override Object ConvertFrom(ITypeDescriptorContext context,
									   CultureInfo culture,
									   Object value)
			{
				if(value is String)
				{
					return Enum.Parse(type, (String)value, true);
				}
				else
				{
					return base.ConvertFrom(context, culture, value);
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
				if(destinationType == typeof(String))
				{
					return value.ToString();
				}
			#if CONFIG_COMPONENT_MODEL_DESIGN
				if(destinationType == typeof(InstanceDescriptor) &&
				   type.IsInstanceOfType(value))
				{
					MethodInfo method;
					method = typeof(Enum).GetMethod
							("ToObject",
							 new Type [] {typeof(Type), typeof(Object)});
					if(method == null)
					{
						return null;
					}
					FieldInfo field = value.GetType().GetField("value__");
					if(field == null)
					{
						return null;
					}
					return new InstanceDescriptor
						(method, new Object []
							{type, field.GetValue(value)});
				}
			#endif
				return base.ConvertTo
					(context, culture, value, destinationType);
			}

	// Return a collection of standard values for this data type.
	public override StandardValuesCollection GetStandardValues
				(ITypeDescriptorContext context)
			{
				if(standardValues != null)
				{
					return standardValues;
				}
				Array array = Enum.GetValues(type);
				IComparer comparer = Comparer;
				if(comparer != null)
				{
					Array.Sort(array, 0, array.Length, comparer);
				}
				standardValues = new StandardValuesCollection(array);
				return standardValues;
			}

	// Determine if the list of standard values is an exclusive list.
	public override bool GetStandardValuesExclusive
				(ITypeDescriptorContext context)
			{
				// The list is exclusive if it does not contain flags.
				return !(type.IsDefined(typeof(FlagsAttribute), false));
			}

	// Determine if "GetStandardValues" is supported.
	public override bool GetStandardValuesSupported
				(ITypeDescriptorContext context)
			{
				return true;
			}

	// Determine if an object is valid for this type.
	public override bool IsValid(ITypeDescriptorContext context, Object value)
			{
				return Enum.IsDefined(type, value);
			}

}; // class EnumConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
