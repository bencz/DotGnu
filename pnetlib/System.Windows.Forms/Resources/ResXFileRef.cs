/*
 * ResXFileRef.cs - Implementation of the
 *			"System.Resources.ResXFileRef" class. 
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

namespace System.Resources
{

#if !ECMA_COMPAT

using System;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Reflection;

[Serializable]
#if CONFIG_COMPONENT_MODEL
[TypeConverter(typeof(ResXFileRef.Converter))]
#endif
public class ResXFileRef
{
	// Internal state.
	private String fileName;
	private String typeName;

	// Constructor.
	public ResXFileRef(String fileName, String typeName)
			{
				this.fileName = fileName;
				this.typeName = typeName;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return fileName + ";" + typeName;
			}

#if CONFIG_COMPONENT_MODEL

	// Type converter for "ResXFileRef" instances.
	public class Converter : TypeConverter
	{
		// Constructor.
		public Converter() {}

		// Determine if we can convert from a specific type to this one.
		public override bool CanConvertFrom
					(ITypeDescriptorContext context, Type sourceType)
				{
					return (sourceType == typeof(String));
				}
	
		// Determine if we can convert from this type to a specific type.
		public override bool CanConvertTo
					(ITypeDescriptorContext context, Type destinationType)
				{
					return (destinationType == typeof(String));
				}
	
		// Convert from another type to the one represented by this class.
		public override Object ConvertFrom(ITypeDescriptorContext context,
										   CultureInfo culture,
										   Object value)
				{
					string s = value as String;
					if (s == null)
						return base.ConvertFrom(context, culture, value);
					else
					{
						string[] split = s.Split(';');
						string fileName = split[0];
						byte[] bytes;
						using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							bytes = new byte[fileStream.Length];
							fileStream.Read(bytes, 0, (int)fileStream.Length);
						}
						Type type = Type.GetType(split[1]);
						return Activator.CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, new object[]{new MemoryStream(bytes)}, null);
					}
				}
	
		// Convert this object into another type.
		public override Object ConvertTo(ITypeDescriptorContext context,
										 CultureInfo culture,
										 Object value, Type destinationType)
				{
					if(destinationType == typeof(String))
					{
						return ((ResXFileRef)value).ToString();
					}
					else
					{
						return base.ConvertTo(context, culture, value,
											  destinationType);
					}
				}

	}; // class Converter

#endif // CONFIG_COMPONENT_MODEL

}; // class ResXFileRef

#endif // !ECMA_COMPAT

}; // namespace System.Resources
