/*
 * MarshalByRefObject.cs - Implementation of "System.MarshalByRefObject".
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System
{

using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;

public abstract class MarshalByRefObject
{

	// Constructor.
	protected MarshalByRefObject() : base() {}

#if CONFIG_REMOTING

	// Identity of this object.
	private RemotingServices.Identity identity;

	// Create a marshalable reference for this object.
	public virtual ObjRef CreateObjRef(Type requestedType)
			{
				if(identity == null)
				{
					throw new RemotingException(_("Remoting_NoIdentity"));
				}
				return new ObjRef(this, requestedType);
			}

	// Get a lifetime service object for this object.
	public Object GetLifetimeService()
			{
				return LifetimeServices.GetLifetimeService(this);
			}

	// Initialize the lifetime service for this object.
	public virtual Object InitializeLifetimeService()
			{
				return LifetimeServices.InitializeLifetimeService(this);
			}

	// Get this object's identity.
	internal RemotingServices.Identity GetIdentity()
			{
				lock(this)
				{
					if(RemotingServices.IsTransparentProxy(this))
					{
						return RemotingServices.GetRealProxy(this).Identity;
					}
					else
					{
						return identity;
					}
				}
			}

	// Clear this object's identity.
	internal void ClearIdentity()
			{
				lock(this)
				{
					identity = null;
				}
			}

	// Set this object's identity.
	internal void SetIdentity(RemotingServices.Identity identity)
			{
				lock(this)
				{
					if(RemotingServices.IsTransparentProxy(this))
					{
						RemotingServices.GetRealProxy(this).Identity
							= identity;
					}
					else
					{
						this.identity = identity;
					}
				}
			}

#endif // CONFIG_REMOTING

}; // class MarshalByRefObject

}; // namespace System
