//#define CHECK_FINALIZERS
/*
 * Component.cs - Implementation of the
 *		"System.ComponentModel.Component" class.
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
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System;

[DesignerCategory("Component")]
public class Component : MarshalByRefObject, IComponent, IDisposable
{
	// Internal state.
	private static readonly Object disposedId = new Object();
	private EventHandlerList events;
	private ISite site;

#if CHECK_FINALIZERS
	static int iCount = 0;
#endif
	
	// Constructor.
	public Component() {
#if CHECK_FINALIZERS
		++iCount;
		try {
			Console.WriteLine( "++CO {0} {1} {2}", iCount, GetHashCode(), this );
		}
		catch( Exception ) {
			Console.WriteLine( "++CO {0} {1}", iCount, GetHashCode() );
		}
#endif
	}

	// Destructor.
	~Component()
			{
				Dispose(false);
#if CHECK_FINALIZERS
				--iCount;
				try {
					Console.WriteLine( "--CO {0} {1} {2}", iCount, GetHashCode(), this);
				}
				catch( Exception ) {
					Console.WriteLine( "--CO {0} {1}", iCount, GetHashCode() );
				}
#endif
			}

	// Get this component's container.
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public IContainer Container
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
	protected bool DesignMode
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
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
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
					//Console.WriteLine( "DISPOSE {0}", this );
					//ReflectComponent( this );
				}
			}

			static void ReflectComponent( Component co ) {
				Type t = co.GetType();

      
      //FieldInfo [] fields = t.GetFields(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static );
				FieldInfo [] fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				foreach( FieldInfo fld in fields ) {
					object o = fld.GetValue(co);
					if( null != o ) {
						Type tt = o.GetType();
						if( Component.CheckType(tt) ) {
							Console.WriteLine( "Field {0} [{1}] value {2}", fld.Name, tt, o );
						}
					}
				}
			}

			static bool CheckType( Type t ) {
				if( t.IsEnum ) return false;
				if( t.IsPrimitive ) return false;
				/*
				if( t == typeof(String) ) return false;
				if( t == typeof(Color) ) return false;
				if( t == typeof(Size) ) return false;
				if( t == typeof(Rectangle) ) return false;
				if( t == typeof(Point) ) return false;
				*/
				return true;
			}
			
	// Get a service that is implemented by this component.
	protected virtual Object GetService(Type service)
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

}; // class Component

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
