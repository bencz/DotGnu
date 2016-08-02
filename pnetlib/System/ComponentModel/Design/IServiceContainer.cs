/*
 * IServiceContainer.cs - Implementation of the
 *		"System.ComponentModel.Design.IServiceContainer" class.
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

using System.Runtime.InteropServices;

[ComVisible(true)]
public interface IServiceContainer : IServiceProvider
{
	// Add a service to this container.
	void AddService(Type serviceType, Object serviceInstance);
	void AddService(Type serviceType, ServiceCreatorCallback callback);
	void AddService(Type serviceType, Object serviceInstance, bool promote);
	void AddService
			(Type serviceType, ServiceCreatorCallback callback, bool promote);

	// Remove a service from this container.
	void RemoveService(Type serviceType);
	void RemoveService(Type serviceType, bool promote);

}; // interface IServiceContainer

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
