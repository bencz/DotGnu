/*
 * SecurityCallContext.cs - Implementation of the
 *			"System.EnterpriseServices.SecurityCallContext" class.
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

public sealed class SecurityCallContext
{
	// Internal state.
	[ThreadStatic] private static SecurityCallContext currentCall;
	private SecurityCallers callers;

	// Constructor.
	private SecurityCallContext()
			{
				// Create the caller chain and push the current user onto it.
				callers = new SecurityCallers();
				callers.PushCaller(new SecurityIdentity());
			}

	// Get the current security call context.
	public static SecurityCallContext CurrentCall
			{
				get
				{
					if(currentCall == null)
					{
						currentCall = new SecurityCallContext();
					}
					return currentCall;
				}
			}

	// Get this object's properties.
	public SecurityCallers Callers
			{
				get
				{
					return callers;
				}
			}
	public SecurityIdentity DirectCaller
			{
				get
				{
					return callers[0];
				}
			}
	public bool IsSecurityEnabled
			{
				get
				{
					return true;
				}
			}
	public int MinAuthenticationLevel
			{
				get
				{
					return 0;
				}
			}
	public int NumCallers
			{
				get
				{
					return Callers.Count;
				}
			}
	public SecurityIdentity OriginalCaller
			{
				get
				{
					return callers[callers.Count - 1];
				}
			}

	// Determine if the current caller is in a particular role.
	public bool IsCallerInRole(String role)
			{
				// Not used in this implementation.
				return false;
			}

	// Determine if the user is in a particular role.
	public bool IsUserInRole(String user, String role)
			{
				// Not used in this implementation.
				return false;
			}

}; // class SecurityCallContext

}; // namespace System.EnterpriseServices
