/*
 * ILease.cs - Implementation of the
 *			"System.Runtime.Remoting.Lifetime.ILease" class.
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

public interface ILease
{
	// Get the current lease time.
	TimeSpan CurrentLeaseTime { get; }

	// Get the current lease state.
	LeaseState CurrentState { get; }

	// Get or set the initial lease time.
	TimeSpan InitialLeaseTime { get; set; }

	// Get or set the "renew on call" time.
	TimeSpan RenewOnCallTime { get; set; }

	// Get or set the sponsorship timeout.
	TimeSpan SponsorshipTimeout { get; set; }

	// Register a sponsor without renewing the lease.
	void Register(ISponsor obj);

	// Register a sponsor and renew the lease.
	void Register(ISponsor obj, TimeSpan renewalTime);

	// Renew the lease.
	TimeSpan Renew(TimeSpan renewalTime);

	// Unregister a sponsor.
	void Unregister(ISponsor obj);

}; // interface ILease

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Lifetime
