/*
 * IEventBindingService.cs - Implementation of the
 *		"System.ComponentModel.Design.IEventBindingService" class.
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

namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN

using System.Collections;
using System.Runtime.InteropServices;

[ComVisible(true)]
public interface IEventBindingService
{
	// Create a unique method name.
	String CreateUniqueMethodName(IComponent component, EventDescriptor e);

	// Get the methods that are compatible with a specified event.
	ICollection GetCompatibleMethods(EventDescriptor e);

	// Get the event that is represented by a specific property.
	EventDescriptor GetEvent(PropertyDescriptor property);

	// Convert a set of event descriptors into corresponding properties.
	PropertyDescriptorCollection GetEventProperties
				(EventDescriptorCollection events);

	// Convert an event into a property descriptor.
	PropertyDescriptor GetEventProperty(EventDescriptor e);

	// Display the user code for the designer.
	bool ShowCode();
	bool ShowCode(int lineNumber);
	bool ShowCode(IComponent component, EventDescriptor e);

}; // interface IEventBindingService

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
