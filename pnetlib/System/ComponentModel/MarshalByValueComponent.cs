/*
 * MarshalByValueComponent.cs - Implementation of the
 *		"System.ComponentModel.MarshalByValueComponent" class.
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
using System.ComponentModel.Design;

[DesignerCategory("Component")]
[TypeConverter(typeof(ComponentConverter))]
#if CONFIG_COMPONENT_MODEL_DESIGN
[Designer
	("System.Windows.Forms.Design.ComponentDocumentDesigner, System.Design",
	 typeof(IRootDesigner))]
#endif
public class MarshalByValueComponent
	: IComponent, IDisposable, IServiceProvider
{
	// Internal state.
	private static readonly Object disposedId = new Object();
	private EventHandlerList events;
	private ISite site;

	// Constructor.
	public MarshalByValueComponent() {}

	// Destructor.
	~MarshalByValueComponent()
			{
				Dispose(false);
			}

	// Get this component's container.
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual IContainer Container
			{
				get
				{
					if(site != null)
					{
						return site.Container;
					}
					else
					{
						return null;
					}
				}
			}

	// Determine if this component is in "design mode".
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual bool DesignMode
			{
				get
				{
					if(site != null)
					{
						return site.DesignMode;
					}
					else
					{
						return false;
					}
				}
			}

	// Get the event handler list for this component.
	protected EventHandlerList Events
			{
				get
				{
					lock(this)
					{
						if(events == null)
						{
							events = new EventHandlerList();
						}
						return events;
					}
				}
			}

	// Get or set the site associated with this component.
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual ISite Site
			{
				get
				{
					return site;
				}
				set
				{
					site = value;
				}
			}

	// Event that is raised when a component is disposed.
	public event EventHandler Disposed
			{
				add
				{
					Events.AddHandler(disposedId, value);
				}
				remove
				{
					Events.RemoveHandler(disposedId, value);
				}
			}

	// Implement the IDisposable interface.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	// Dispose this component.
	protected virtual void Dispose(bool disposing)
			{
				if(disposing)
				{
					lock(this)
					{
						EventHandler dispose;
						if(site != null && site.Container != null)
						{
							site.Container.Remove(this);
						}
						if(events != null)
						{
							dispose = (EventHandler)(events[disposedId]);
							if(dispose != null)
							{
								dispose(this, EventArgs.Empty);
							}
						}
					}
				}
			}

	// Get a service that is implemented by this component.
	public virtual Object GetService(Type service)
			{
				if(site != null)
				{
					return site.GetService(service);
				}
				else
				{
					return null;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				if(site != null)
				{
					return site.Name + "[" + GetType().ToString() + "]";
				}
				else
				{
					return GetType().ToString();
				}
			}

}; // class MarshalByValueComponent

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
