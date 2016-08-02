/*
 * Context.cs - Implementation of the
 *			"System.Runtime.Remoting.Contexts.Context" class.
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

namespace System.Runtime.Remoting.Contexts
{

#if CONFIG_REMOTING

using System.Threading;

public class Context
{
	// Internal state.
	private static int nextContextID = 0;
	private int contextID;
	private IContextProperty[] properties;
	private bool freeze;

	// Constructor.
	public Context()
			{
				lock(typeof(Context))
				{
					contextID = ++nextContextID;
				}
			}

	// Destructor.
	~Context()
			{
				// Nothing to do here.
			}

	// Allocate a local data slot.
	public static LocalDataStoreSlot AllocateDataSlot()
			{
				return Thread.AllocateDataSlot();
			}

	// Allocate a named local data slot.
	public static LocalDataStoreSlot AllocateNamedDataSlot(String name)
			{
				return Thread.AllocateNamedDataSlot("context_" + name);
			}

	// Perform a cross-context callback.
	[TODO]
	public void DoCallBack(CrossContextDelegate deleg)
			{
				// TODO
			}

	// Free a named local data slot.
	public static void FreeNamedDataSlot(String name)
			{
				Thread.FreeNamedDataSlot("context_" + name);
			}

	// Freeze this context.
	public virtual void Freeze()
			{
				lock(this)
				{
					if(freeze)
					{
						throw new InvalidOperationException
							(_("Invalid_ContextFrozen"));
					}
					freeze = true;
					if(properties != null)
					{
						foreach(IContextProperty prop in properties)
						{
							prop.Freeze(this);
						}
					}
				}
			}

	// Get the data stored in a particular data store slot.
	public static Object GetData(LocalDataStoreSlot slot)
			{
				return Thread.GetData(slot);
			}

	// Get a named data store slot.
	public static LocalDataStoreSlot GetNamedDataSlot(String name)
			{
				return Thread.GetNamedDataSlot("context_" + name);
			}

	// Get a property from this context.
	public virtual IContextProperty GetProperty(String name)
			{
				lock(this)
				{
					if(name == null || properties == null)
					{
						return null;
					}
					foreach(IContextProperty prop in properties)
					{
						if(prop.Name == name)
						{
							return prop;
						}
					}
				}
				return null;
			}

	// Register a dynamic property.
	[TODO]
	public static bool RegisterDynamicProperty
				(IDynamicProperty prop, ContextBoundObject obj, Context ctx)
			{
				// TODO
				return false;
			}

	// Set the value of a local data store slot.
	public static void SetData(LocalDataStoreSlot slot, Object value)
			{
				Thread.SetData(slot, value);
			}

	// Set a property on this context.
	public virtual void SetProperty(IContextProperty prop)
			{
				if(prop == null)
				{
					throw new ArgumentNullException("prop");
				}
				if(prop.Name == null)
				{
					throw new ArgumentNullException("prop.Name");
				}
				lock(this)
				{
					// The context must not be currently frozen.
					if(freeze)
					{
						throw new InvalidOperationException
							(_("Invalid_ContextFrozen"));
					}

					// Bail out if there is already a property with this name.
					if(properties != null)
					{
						foreach(IContextProperty prop2 in properties)
						{
							if(prop.Name == prop2.Name)
							{
								throw new InvalidOperationException
									(_("Invalid_PropertyClash"));
							}
						}
					}

					// Expand the property array and add the new element.
					if(properties == null)
					{
						properties = new IContextProperty [1];
						properties[0] = prop;
					}
					else
					{
						IContextProperty[] newProperties;
						newProperties = new IContextProperty
							[properties.Length + 1];
						Array.Copy(properties, 0, newProperties, 0,
								   properties.Length);
						properties[properties.Length] = prop;
						newProperties = properties;
					}
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return "ContextID: " + contextID.ToString();
			}

	// Unregister a dynamic property.
	[TODO]
	public static bool UnregisterDynamicProperty
				(String name, ContextBoundObject obj, Context ctx)
			{
				// TODO
				return false;
			}

	// Get the identifier for this context.
	public virtual int ContextID
			{
				get
				{
					return contextID;
				}
			}

	// Get the properties on this context.
	public virtual IContextProperty[] ContextProperties
			{
				get
				{
					lock(this)
					{
						if(properties == null)
						{
							return null;
						}
						return (IContextProperty[])(properties.Clone());
					}
				}
			}

	// Get the default context for the current application domain.
	public static Context DefaultContext
			{
				get
				{
					AppDomain domain = Thread.GetDomain();
					lock(typeof(Context))
					{
						if(domain.defaultContext == null)
						{
							domain.defaultContext = new Context();
						}
						return domain.defaultContext;
					}
				}
			}

}; // class Context

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Contexts
