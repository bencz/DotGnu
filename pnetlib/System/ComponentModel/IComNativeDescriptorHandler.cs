/*
 * IComNativeDescriptorHandler.cs - Implementation of the
 *		"System.ComponentModel.IComNativeDescriptorHandler" interface.
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

public interface IComNativeDescriptorHandler
{
	AttributeCollection GetAttributes(Object component);
	String GetClassName(Object component);
	TypeConverter GetConverter(Object component);
	EventDescriptor GetDefaultEvent(Object component);
	PropertyDescriptor GetDefaultProperty(Object component);
	Object GetEditor(Object component, Type baseEditorType);
	EventDescriptorCollection GetEvents(Object component);
	EventDescriptorCollection GetEvents
			(Object component, Attribute[] attributes);
	String GetName(Object component);
	PropertyDescriptorCollection GetProperties
			(Object component, Attribute[] attributes);
	Object GetPropertyValue
			(Object component, int dispid, ref bool success);
	Object GetPropertyValue
			(Object component, String propertyName, ref bool success);

}; // interface IComNativeDescriptorHandler

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
