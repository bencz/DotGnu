/*
 * ProxyAttribute.cs - Implementation of the
 *			"System.Runtime.Remoting.Proxies.ProxyAttribute" class.
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

namespace System.Runtime.Remoting.Proxies
{

#if CONFIG_REMOTING

using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
public class ProxyAttribute : Attribute, IContextAttribute
{
	// Constructor.
	public ProxyAttribute() {}

	// Create a proxy instance.
	[TODO]
	public virtual MarshalByRefObject CreateInstance(Type serverType)
			{
				// TODO
				return null;
			}

	// Create a real proxy.
	[TODO]
	public virtual RealProxy CreateProxy(ObjRef objRef, Type serverType,
										 Object serverObject,
										 Context serverContext)
			{
				// TODO
				return null;
			}

	// Get the properties for a new construction context.
	public void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
			{
				// Nothing to do here.
			}

	// Determine if a context is OK with respect to this attribute.
	public bool IsContextOK(Context ctx, IConstructionCallMessage msg)
			{
				return true;
			}

}; // class ProxyAttribute

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Proxies
