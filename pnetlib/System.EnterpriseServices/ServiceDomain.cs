/*
 * ServiceDomain.cs - Implementation of the
 *			"System.EnterpriseServices.ServiceDomain" class.
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

namespace System.EnterpriseServices
{

using System.Collections;
using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public sealed class ServiceDomain
{
	// Internal state.
	[ThreadStatic] private static ArrayList configList;
	[ThreadStatic] private static TransactionStatus status;

	// Cannot instantiate this class.
	private ServiceDomain() {}

	// Enter a new service configuration.
	public static void Enter(ServiceConfig cfg)
			{
				if(configList == null)
				{
					configList = new ArrayList();
				}
				configList.Add(cfg);
			}

	// Leave the current service configuration.
	public static TransactionStatus Leave()
			{
				// Bail out if there was no context currently active.
				if(configList == null || configList.Count == 0)
				{
					return TransactionStatus.NoTransaction;
				}

				// Pop the top-most configuration context.
				configList.RemoveAt(configList.Count - 1);

				// Return the transaction status.
				return status;
			}

}; // class ServiceDomain

}; // namespace System.EnterpriseServices
