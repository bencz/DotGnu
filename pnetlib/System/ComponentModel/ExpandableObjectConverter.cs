/*
 * ExpandableObjectConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.ExpandableObjectConverter" class.
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
using System.ComponentModel.Design;

public class ExpandableObjectConverter : TypeConverter
{
	// Constructor.
	public ExpandableObjectConverter() {}

	// Get the properties for an object.
	public override PropertyDescriptorCollection GetProperties
				(ITypeDescriptorContext context, Object value,
				 Attribute[] attributes)
			{
				return TypeDescriptor.GetProperties(value, attributes);
			}

	// Determine if the "GetProperties" method is supported.
	public override bool GetPropertiesSupported
				(ITypeDescriptorContext context)
			{
				return true;
			}

}; // class ExpandableObjectConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
