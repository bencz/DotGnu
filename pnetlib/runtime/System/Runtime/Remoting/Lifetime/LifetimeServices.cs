/*
 * LifetimeServices.cs - Implementation of the
 *			"System.Runtime.Remoting.Lifetime.LifetimeServices" class.
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

using System.Security.Permissions;
using System.Collections;
using System.Threading;

[SecurityPermission(SecurityAction.LinkDemand,
					Flags=SecurityPermissionFlag.Infrastructure)]
[TODO]
public sealed class LifetimeServices
{
	// Internal state.
	private static TimeSpan leaseManagerPollTime = new TimeSpan(0, 10, 0);
	private static TimeSpan leaseTime = new TimeSpan(0, 5, 0);
	private static TimeSpan renewOnCallTime = new TimeSpan(0, 2, 0);
	private static TimeSpan sponsorshipTimeout = new TimeSpan(0, 2, 0);

	// Get or set the global lease manager poll time setting.
	public static TimeSpan LeaseManagerPollTime
			{
				get
				{
					return leaseManagerPollTime;
				}
				set
				{
					lock(typeof(LifetimeServices))
					{
						leaseManagerPollTime = value;
						AppDomain current = AppDomain.CurrentDomain;
						if(current.lifetimeManager != null)
						{
							current.lifetimeManager.PollTime = value;
						}
					}
				}
			}

	// Get or set the global lease time setting.
	public static TimeSpan LeaseTime
			{
				get
				{
					return leaseTime;
				}
				set
				{
					leaseTime = value;
				}
			}

	// Get or set the global renew on call time setting.
	public static TimeSpan RenewOnCallTime
			{
				get
				{
					return renewOnCallTime;
				}
				set
				{
					renewOnCallTime = value;
				}
			}

	// Get or set the global sponsorship timeout setting.
	public static TimeSpan SponsorshipTimeout
			{
				get
				{
					return sponsorshipTimeout;
				}
				set
				{
					sponsorshipTimeout = value;
				}
			}

	// Get the lifetime manager for the current application domain.
	private static Manager GetLifetimeManager()
			{
				lock(typeof(LifetimeServices))
				{
					AppDomain current = AppDomain.CurrentDomain;
					if(current.lifetimeManager == null)
					{
						current.lifetimeManager = new Manager
							(leaseManagerPollTime);
					}
					return current.lifetimeManager;
				}
			}

	// Get the default lifetime service object for a marshal-by-ref object.
	internal static Object GetLifetimeService(MarshalByRefObject obj)
			{
				return GetLifetimeManager().GetLeaseForObject(obj);
			}

	// Initialize a lifetime service object for a marshal-by-ref object.
	internal static Object InitializeLifetimeService(MarshalByRefObject obj)
			{
				Manager manager = GetLifetimeManager();
				ILease lease = manager.GetLeaseForObject(obj);
				if(lease != null)
				{
					return lease;
				}
				return new Lease(obj, LeaseTime, RenewOnCallTime,
								 SponsorshipTimeout);
			}

	// Lifetime lease manager for an application domain.
	internal class Manager
	{
		// Internal state.
		private TimeSpan pollTime;
		private Hashtable leases;
		private Timer timer;

		// Constructor.
		public Manager(TimeSpan pollTime)
				{
					this.pollTime = pollTime;
					this.leases = new Hashtable();
					this.timer = new Timer
						(new TimerCallback(Callback), null,
						 pollTime, pollTime);
				}

		// Get or set the poll time.
		public TimeSpan PollTime
				{
					get
					{
						lock(this)
						{
							return pollTime;
						}
					}
					set
					{
						lock(this)
						{
							pollTime = value;
							timer.Change(pollTime, pollTime);
						}
					}
				}

		// Get an active lease for an object.
		public ILease GetLeaseForObject(MarshalByRefObject obj)
				{
					// TODO
					return null;
				}

		// Callback for processing lease timeouts.
		private void Callback(Object state)
				{
					// TODO
				}

	}; // class Manager

	// Lease control object.
	private class Lease : MarshalByRefObject, ILease
	{
		// Internal state.
		private MarshalByRefObject obj;
		private DateTime leaseTimeout;
		private TimeSpan initialLeaseTime;
		private TimeSpan renewOnCallTime;
		private TimeSpan sponsorshipTimeout;
		private LeaseState state;

		// Constructor.
		public Lease(MarshalByRefObject obj, TimeSpan leaseTime,
					 TimeSpan renewOnCallTime, TimeSpan sponsorshipTimeout)
				{
					this.obj = obj;
					this.initialLeaseTime = leaseTime;
					this.renewOnCallTime = renewOnCallTime;
					this.sponsorshipTimeout = sponsorshipTimeout;
					this.state = LeaseState.Initial;
				}

		// Cannot have a lease for a lease!
		public override Object InitializeLifetimeService()
				{
					return null;
				}

		// Implement the ILease interface.
		public TimeSpan CurrentLeaseTime
				{
					get
					{
						return leaseTimeout - DateTime.UtcNow;
					}
				}
		public LeaseState CurrentState
				{
					get
					{
						return state;
					}
				}
		public TimeSpan InitialLeaseTime
				{
					get
					{
						return initialLeaseTime;
					}
					set
					{
						if(state != LeaseState.Initial)
						{
							throw new RemotingException
								(_("Invalid_ModifyLease"));
						}
						initialLeaseTime = value;
						if(value <= TimeSpan.Zero)
						{
							// Disable the lease.
							state = LeaseState.Null;
						}
					}
				}
		public TimeSpan RenewOnCallTime
				{
					get
					{
						return renewOnCallTime;
					}
					set
					{
						if(state != LeaseState.Initial)
						{
							throw new RemotingException
								(_("Invalid_ModifyLease"));
						}
						renewOnCallTime = value;
					}
				}
		public TimeSpan SponsorshipTimeout
				{
					get
					{
						return renewOnCallTime;
					}
					set
					{
						if(state != LeaseState.Initial)
						{
							throw new RemotingException
								(_("Invalid_ModifyLease"));
						}
						renewOnCallTime = value;
					}
				}
		public void Register(ISponsor obj)
				{
					Register(obj, new TimeSpan(0));
				}
		public void Register(ISponsor obj, TimeSpan renewalTime)
				{
					// TODO
				}
		public TimeSpan Renew(TimeSpan renewalTime)
				{
					// TODO
					return renewalTime;
				}
		public void Unregister(ISponsor obj)
				{
					// TODO
				}

	}; // class Lease

}; // class LifetimeServices

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Lifetime
