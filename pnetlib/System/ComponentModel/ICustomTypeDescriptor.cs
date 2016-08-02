/*
 * ICustomTypeDescriptor.cs - Implementation of the
 *			"System.ComponentModel.ICustomTypeDescriptor" class.
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

public interface ICustomTypeDescriptor
{
	// Return the attribute collection for this object.
	AttributeCollection GetAttributes();

	// Get the class name for this object.
	String GetClassName();

	// Get the component name for this object.
	String GetComponentName();

	// Get the type converter associated with this object.
	TypeConverter GetConverter();

	// Get the default event for this object.
	EventDescriptor GetDefaultEvent();

	// Get the default property for this object.
	PropertyDescriptor GetDefaultProperty();

	// Get an editor of the specified type for this object.
	Object GetEditor(System.Type editorBaseType);

	// Get the events for this object.
	EventDescriptorCollection GetEvents();
	EventDescriptorCollection GetEvents(Attribute[] arr);

	// Get the properties for this object.
	PropertyDescriptorCollection GetProperties();
	PropertyDescriptorCollection GetProperties(Attribute[] arr);

	// Get the object that owns a particular property.
	Object GetPropertyOwner(PropertyDescriptor pd);

}; // interface ICustomTypeDescriptor

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
