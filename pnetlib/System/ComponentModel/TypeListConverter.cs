/*
 * TypeListConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.TypeListConverter" class.
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

public abstract class TypeListConverter : TypeConverter
{
	// Internal state.
	private Type[] types;

	// Constructor.
	protected TypeListConverter(Type[] types)
			{
				this.types = types;
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
					String val = (String)value;
					if(val == "(none)")
					{
						return null;
					}
					if(types != null)
					{
						foreach(Type t in types)
						{
							if(t.FullName == val)
							{
								return t;
							}
						}
					}
				}
				return base.ConvertFrom(context, culture, value);
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
					if(value != null)
					{
						return ((Type)value).FullName;
					}
					else
					{
						return "(none)";
					}
				}
			#if CONFIG_COMPONENT_MODEL_DESIGN
				if(destinationType == typeof(InstanceDescriptor) &&
				   value is Type)
				{
					MethodInfo method;
					method = typeof(Type).GetMethod
							("GetType", new Type [] {typeof(String)});
					if(method == null)
					{
						return null;
					}
					return new InstanceDescriptor
						(method, new Object []
							{((Type)value).AssemblyQualifiedName});
				}
			#endif
				return base.ConvertTo
					(context, culture, value, destinationType);
			}

	// Return a collection of standard values for this data type.
	public override StandardValuesCollection GetStandardValues
				(ITypeDescriptorContext context)
			{
				if(types != null)
				{
					return new StandardValuesCollection
						((Type[])(types.Clone()));
				}
				else
				{
					return new StandardValuesCollection(null);
				}
			}

	// Determine if the list of standard values is an exclusive list.
	public override bool GetStandardValuesExclusive
				(ITypeDescriptorContext context)
			{
				return true;
			}

	// Determine if "GetStandardValues" is supported.
	public override bool GetStandardValuesSupported
				(ITypeDescriptorContext context)
			{
				return true;
			}

}; // class TypeListConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
