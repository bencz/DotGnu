/*
 * ServiceContainer.cs - Implementation of the
 *		"System.ComponentModel.Design.ServiceContainer" class.
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

public sealed class ServiceContainer : IServiceContainer, IServiceProvider
{
	// Internal state.
	private IServiceProvider parentProvider;
	private Hashtable table;

	// Constructors.
	public ServiceContainer() : this(null) {}
	public ServiceContainer(IServiceProvider parentProvider)
			{
				this.parentProvider = parentProvider;
				this.table = new Hashtable();
			}

	// Add a service to this container.
	public void AddService(Type serviceType, Object serviceInstance)
			{
				AddService(serviceType, serviceInstance, false);
			}
	public void AddService(Type serviceType, ServiceCreatorCallback callback)
			{
				AddService(serviceType, callback, false);
			}
	public void AddService
				(Type serviceType, Object serviceInstance, bool promote)
			{
				// Validate the parameters.
				if(serviceType == null)
				{
					throw new ArgumentNullException("serviceType");
				}
				if(serviceInstance == null)
				{
					throw new ArgumentNullException("serviceInstance");
				}

				// Promote the service to the parent if necessary.
				if(promote && parentProvider != null)
				{
					IServiceContainer parent;
					parent = (IServiceContainer)(parentProvider.GetService
						(typeof(IServiceContainer)));
					if(parent != null)
					{
						parent.AddService
							(serviceType, serviceInstance, promote);
						return;
					}
				}

				// Add the service to this container.
				table[serviceType] = serviceInstance;
			}
	public void AddService
				(Type serviceType, ServiceCreatorCallback callback,
				 bool promote)
			{
				// Validate the parameters.
				if(serviceType == null)
				{
					throw new ArgumentNullException("serviceType");
				}
				if(callback == null)
				{
					throw new ArgumentNullException("callback");
				}

				// Promote the service to the parent if necessary.
				if(promote && parentProvider != null)
				{
					IServiceContainer parent;
					parent = (IServiceContainer)(parentProvider.GetService
						(typeof(IServiceContainer)));
					if(parent != null)
					{
						parent.AddService(serviceType, callback, promote);
						return;
					}
				}

				// Add the service to this container.
				table[serviceType] = callback;
			}

	// Remove a service from this container.
	public void RemoveService(Type serviceType)
			{
				RemoveService(serviceType, false);
			}
	public void RemoveService(Type serviceType, bool promote)
			{
				// Validate the parameters.
				if(serviceType == null)
				{
					throw new ArgumentNullException("serviceType");
				}

				// Promote the removal to the parent if necessary.
				if(promote && parentProvider != null)
				{
					IServiceContainer parent;
					parent = (IServiceContainer)(parentProvider.GetService
						(typeof(IServiceContainer)));
					if(parent != null)
					{
						parent.RemoveService(serviceType, promote);
						return;
					}
				}

				// Remove the service from this container.
				table.Remove(serviceType);
			}

	// Get a particular service.
	public Object GetService(Type serviceType)
			{
				if(serviceType == typeof(IServiceContainer))
				{
					return this;
				}
				else
				{
					Object service = table[serviceType];
					if(service == null && parentProvider != null)
					{
						service = parentProvider.GetService(serviceType);
					}
					if(service is ServiceCreatorCallback)
					{
						ServiceCreatorCallback callback;
						callback = ((ServiceCreatorCallback)service);
						return callback(this, serviceType);
					}
					return service;
				}
			}

}; // class ServiceContainer

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
