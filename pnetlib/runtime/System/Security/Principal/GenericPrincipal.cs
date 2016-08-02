/*
 * GenericPrincipal.cs - Implementation of the
 *		"System.Security.Principal.GenericPrincipal" class.
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

namespace System.Security.Principal
{

#if CONFIG_POLICY_OBJECTS

[Serializable]
public class GenericPrincipal : IPrincipal
{
	// Internal state.
	private IIdentity identity;
	private String[] roles;

	// Constructor.
	public GenericPrincipal(IIdentity identity, String[] roles)
			{
				if(identity == null)
				{
					throw new ArgumentNullException("identity");
				}
				this.identity = identity;
				if(roles != null)
				{
					this.roles = (String[])(roles.Clone());
				}
				else
				{
					this.roles = null;
				}
			}

	// Get the identity of the principal.
	public virtual IIdentity Identity
			{
				get
				{
					return identity;
				}
			}

	// Determine whether the principal belongs to a specific role.
	public virtual bool IsInRole(String role)
			{
				if(role != null && roles != null)
				{
					return (Array.IndexOf(roles, role) != -1);
				}
				else
				{
					return false;
				}
			}

}; // class GenericPrincipal

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Principal
