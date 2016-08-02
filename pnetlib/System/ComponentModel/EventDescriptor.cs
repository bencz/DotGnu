/*
 * EventDescriptor.cs - Implementation of the
 *			"System.ComponentModel.EventDescriptor" class.
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

using System;

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System.Runtime.InteropServices;

[ComVisible(true)]
public abstract class EventDescriptor : MemberDescriptor
{
	// Constructors.
	protected EventDescriptor(MemberDescriptor descr) : base(descr) {}
	protected EventDescriptor(MemberDescriptor descr, Attribute[] attrs)
			: base(descr, attrs) {}
	protected EventDescriptor(String name, Attribute[] attrs)
			: base(name, attrs) {}

	// Get the type of component that this event is bound to.
	public abstract Type ComponentType { get; }

	// Get the delegate type associated with the event.
	public abstract Type EventType { get; }

	// Determine if the event delegate is multicast.
	public abstract bool IsMulticast { get; }

	// Add an event handler to a component.
	public abstract void AddEventHandler(Object component, Delegate value);

	// Remove an event handler from a component.
	public abstract void RemoveEventHandler(Object component, Delegate value);

}; // class EventDescriptor

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
