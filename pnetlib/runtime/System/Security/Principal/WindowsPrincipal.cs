/*
 * WindowsPrincipal.cs - Implementation of the
 *		"System.Security.Principal.WindowsPrincipal" class.
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
public class WindowsPrincipal : IPrincipal
{
	// Internal state.
	private IIdentity identity;

	// Constructor.
	public WindowsPrincipal(WindowsIdentity ntIdentity)
			{
				if(ntIdentity == null)
				{
					throw new ArgumentNullException("ntIdentity");
				}
				identity = ntIdentity;
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
	// Always returns false because we don't support Windows principals.
	public virtual bool IsInRole(String role)
			{
				return false;
			}
	public virtual bool IsInRole(int rid)
			{
				return false;
			}
	public virtual bool IsInRole(WindowsBuiltInRole role)
			{
				return false;
			}

}; // class WindowsPrincipal

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Principal
