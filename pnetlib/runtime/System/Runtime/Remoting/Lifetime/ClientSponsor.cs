/*
 * ClientSponsor.cs - Implementation of the
 *			"System.Runtime.Remoting.Lifetime.ClientSponsor" class.
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

namespace System.Runtime.Remoting.Lifetime
{

#if CONFIG_REMOTING

using System.Collections;

public class ClientSponsor : MarshalByRefObject, ISponsor
{
	// Internal state.
	private Hashtable sponsoredObjects;
	private TimeSpan renewalTime;

	// Constructors.
	public ClientSponsor() : this(new TimeSpan(0, 2, 0)) {}
	public ClientSponsor(TimeSpan renewalTime)
			{
				this.sponsoredObjects = new Hashtable();
				this.renewalTime = renewalTime;
			}

	// Destructor.
	~ClientSponsor()
			{
				// Nothing to do here except declare the finalizer.
			}

	// Get or set the renewal time.
	public TimeSpan RenewalTime
			{
				get
				{
					return renewalTime;
				}
				set
				{
					renewalTime = value;
				}
			}

	// Empty the registration list.
	public void Close()
			{
				lock(this)
				{
					IDictionaryEnumerator e = sponsoredObjects.GetEnumerator();
					while(e.MoveNext())
					{
						((ILease)(e.Value)).Unregister(this);
					}
					sponsoredObjects.Clear();
				}
			}

	// Initialize the lifetime service for this object.
	public override Object InitializeLifetimeService()
			{
				// Nothing to do here.
				return null;
			}

	// Register an object for sponsorship.
	public bool Register(MarshalByRefObject obj)
			{
				// If there is no lease on the object, then bail out.
				ILease lease = (ILease)(obj.GetLifetimeService());
				if(lease == null)
				{
					return false;
				}

				// Inform the lease about the registered sponsor.
				lease.Register(this);
				
				// Add the lease to the sponsor table.
				lock(this)
				{
					sponsoredObjects[obj] = lease;
				}
				return true;
			}

	// Request renewal of a lease.
	public TimeSpan Renewal(ILease lease)
			{
				return renewalTime;
			}

	// Unregister an object from the sponsorship list.
	public void Unregister(MarshalByRefObject obj)
			{
				ILease lease;
				lock(this)
				{
					lease = (ILease)(sponsoredObjects[obj]);
					if(lease == null)
					{
						return;
					}
					sponsoredObjects.Remove(obj);
				}
				lease.Unregister(this);
			}

}; // class ClientSponsor

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Lifetime
