/*
 * CultureInfoConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.CultureInfoConverter" class.
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

public class CultureInfoConverter : TypeConverter
{
	// Constructor.
	public CultureInfoConverter()
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
					String name = (String)value;
					if(name == "(Default)")
					{
						return CultureInfo.InvariantCulture;
					}
					CultureInfo[] cultures;
					cultures = CultureInfo.GetCultures
						(CultureTypes.AllCultures);
					foreach(CultureInfo cultureInfo in cultures)
					{
						if(String.Compare(cultureInfo.DisplayName, name, true) == 0)
						{
							return cultureInfo;
						}
					}
					throw new ArgumentException
						(S._("Arg_InvalidCultureName"));
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
					if(value is CultureInfo)
					{
						if(((CultureInfo)value).LCID == 0x007F)
						{
							return "(Default)";
						}
						else
						{
							return ((CultureInfo)value).DisplayName;
						}
					}
					return String.Empty;
				}
			#if CONFIG_COMPONENT_MODEL_DESIGN
				if(destinationType == typeof(InstanceDescriptor) &&
				   value is CultureInfo)
				{
					ConstructorInfo ctor;
					ctor = typeof(CultureInfo).GetConstructor
							(new Type [] {typeof(int)});
					if(ctor == null)
					{
						return null;
					}
					return new InstanceDescriptor
						(ctor, new Object [] {((CultureInfo)value).LCID});
				}
			#endif
				return base.ConvertTo
					(context, culture, value, destinationType);
			}

	// Return a collection of standard values for this data type.
	public override StandardValuesCollection GetStandardValues
				(ITypeDescriptorContext context)
			{
				ArrayList list = new ArrayList();
				list.Add("(Default)");
				CultureInfo[] cultures =
					CultureInfo.GetCultures(CultureTypes.AllCultures);
				foreach(CultureInfo culture in cultures)
				{
					list.Add(culture.DisplayName);
				}
				return new StandardValuesCollection(list);
			}

	// Determine if the list of standard values is an exclusive list.
	public override bool GetStandardValuesExclusive
				(ITypeDescriptorContext context)
			{
				return false;
			}

	// Determine if "GetStandardValues" is supported.
	public override bool GetStandardValuesSupported
				(ITypeDescriptorContext context)
			{
				return true;
			}

}; // class CultureInfoConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
