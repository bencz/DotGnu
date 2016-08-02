/*
 * IContainer.cs - Implementation of the
 *		"System.ComponentModel.IContainer" interface.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
using System.Runtime.InteropServices;

[ComVisible(true)]
public interface IContainer : IDisposable
{

	// Get a collection of all components in this container.
	ComponentCollection Components { get; }

	// Add a component to this container.
	void Add(IComponent component);
	void Add(IComponent component, String name);

	// Remove a component from this container.
	void Remove(IComponent component);

}; // interface IContainer

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
