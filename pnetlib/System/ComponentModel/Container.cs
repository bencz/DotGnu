/*
 * Container.cs - Implementation of the
 *		"System.ComponentModel.Container" class.
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

public class Container : IContainer, IDisposable
{
	// Internal state.
	private ISite[] sites;
	private int numSites;

	// Constructor.
	public Container() {}

	// Destructor.
	~Container()
			{
				Dispose(false);
			}

	// Get a collection of all components within this container.
	// The collection is a copy, not live.
	public virtual ComponentCollection Components
			{
				get
				{
					lock(this)
					{
						IComponent[] components;
						int posn;
						components = new IComponent [numSites];
						for(posn = 0; posn < numSites; ++posn)
						{
							components[posn] = sites[posn].Component;
						}
						return new ComponentCollection(components);
					}
				}
			}

	// Add a component to this container.
	public virtual void Add(IComponent component)
			{
				Add(component, null);
			}
	public virtual void Add(IComponent component, String name)
			{
				ISite site;
				int posn;

				// Nothing to do if component is null or already in the list.
				if(component == null)
				{
					return;
				}
				site = component.Site;
				if(site != null && site.Container == this)
				{
					return;
				}

				// Check for name duplicates and add the new component.
				lock(this)
				{
					if(name != null)
					{
						for(posn = 0; posn < numSites; ++posn)
						{
							if(String.Compare
								(sites[posn].Name, name, true) == 0)
							{
								throw new ArgumentException
									(S._("Arg_DuplicateComponent"));
							}
						}
					}
					if(site != null)
					{
						site.Container.Remove(component);
					}
					if(sites == null)
					{
						sites = new ISite [4];
					}
					else if(numSites >= sites.Length)
					{
						ISite[] newList = new ISite [numSites * 2];
						Array.Copy(sites, 0, newList, 0, numSites);
						sites = newList;
					}
					site = CreateSite(component, name);
					sites[numSites++] = site;
				}
			}

	// Create a site for a component within this container.
	protected virtual ISite CreateSite(IComponent component, String name)
			{
				return new ContainSite(this, component, name);
			}

	// Implement the IDisposable interface.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	// Dispose of this object.
	protected virtual void Dispose(bool disposing)
			{
				if(disposing)
				{
					// Dispose all of the components in reverse order.
					lock(this)
					{
						IComponent component;
						while(numSites > 0)
						{
							--numSites;
							component = sites[numSites].Component;
							component.Site = null;
							component.Dispose();
						}
						sites = null;
					}
				}
			}

	// Get a service from this object (the only service we support
	// in the base class is "get container".
	protected virtual Object GetService(Type service)
			{
				if(service == typeof(IContainer))
				{
					return this;
				}
				else
				{
					return null;
				}
			}

	// Remove a component from this container.
	public virtual void Remove(IComponent component)
			{
				// Bail out if the component is not in this container.
				if(component == null)
				{
					return;
				}
				ISite site = component.Site;
				if(site == null || site.Container != this)
				{
					return;
				}

				// Lock down the container and remove the component.
				lock(this)
				{
					component.Site = null;
					int posn = 0;
					while(posn < numSites)
					{
						if(sites[posn] == site)
						{
							Array.Copy(sites, posn + 1, sites, posn,
									   numSites - (posn + 1));
							sites[numSites - 1] = null;
							--numSites;
							break;
						}
						++posn;
					}
				}
			}

	// Site information for this type of container.
	private sealed class ContainSite : ISite
	{
		// Internal state.
		private Container container;
		private IComponent component;
		private String name;

		// Constructor
		public ContainSite(Container container, IComponent component,
						   String name)
				{
					this.container = container;
					this.component = component;
					this.name = name;
					component.Site = this;
				}

		// Get the component associated with this site.
		public IComponent Component
				{
					get
					{
						return component;
					}
				}

		// Get the container associated with this site.
		public IContainer Container
				{
					get
					{
						return container;
					}
				}

		// Determine if the component is in design mode.
		public bool DesignMode
				{
					get
					{
						return false;
					}
				}

		// Get or set the name of the component.
		public String Name
				{
					get
					{
						return name;
					}
					set
					{
						name = value;
					}
				}

		// Get a service that is provided by this object.
		public Object GetService(Type serviceType)
				{
					if(serviceType == typeof(ISite))
					{
						return this;
					}
					else
					{
						return container.GetService(serviceType);
					}
				}

	}; // class ContainSite

}; // class Container

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
