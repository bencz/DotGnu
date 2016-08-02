/*
 * Activity.cs - Implementation of the
 *			"System.EnterpriseServices.Activity" class.
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

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public sealed class Activity
{
	// Constructor.
	public Activity(ServiceConfig cfg) {}

	// Run a batch item asychronously.
	public void AsynchronousCall(IServiceCall serviceCall)
			{
				// We do everything synchronously in this implementation.
				SynchronousCall(serviceCall);
			}

	// Bind the user-defined work to the current thread.
	public void BindToCurrentThread()
			{
				// Nothing to do in this implementation.
			}

	// Run a batch item sychronously.
	public void SynchronousCall(IServiceCall serviceCall)
			{
				if(serviceCall != null)
				{
					serviceCall.OnCall();
				}
			}

	// Unbind the user-defined work from whatever thread it is running on.
	public void UnbindFromThread()
			{
				// Nothing to do in this implementation.
			}

}; // class Activity

}; // namespace System.EnterpriseServices
