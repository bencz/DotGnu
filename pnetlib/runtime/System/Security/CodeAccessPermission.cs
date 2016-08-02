/*
 * CodeAccessPermission.cs - Implementation of the
 *		"System.Security.CodeAccessPermission" class.
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

namespace System.Security
{

#if CONFIG_PERMISSIONS

using System;
using System.Security.Permissions;
using System.Runtime.CompilerServices;

public abstract class CodeAccessPermission
	: IPermission, IStackWalk, ISecurityEncodable
{

	// Constructor.
	protected CodeAccessPermission() {}

	// Assert permissions for the caller.
	internal void Assert(int skipFrames)
			{
				// Add the permission to the granted permissions set.  If there
				// are no permissions at all, then assume we are unrestricted.
				ClrPermissions current;
				current = ClrSecurity.GetPermissionsFrom(skipFrames);
				if(current != null)
				{
					PermissionSet set = new PermissionSet(PermissionState.None);
					set.AddPermission(this.Copy());
					set = set.Union(current.granted);
					ClrSecurity.SetPermissions
						(current.SetGranted(set), skipFrames);
				}
			}
	public void Assert()
			{
				// We must have the "Assertion" security flag for this to work.
				SecurityPermission perm;
				perm = new SecurityPermission(SecurityPermissionFlag.Assertion);
				perm.Demand();

				// Assert this permission.
				Assert(2);
			}

	// Deny permissions to the caller.
	internal void Deny(int skipFrames)
			{
				// Add the permission to the denied permissions set.
				ClrPermissions current;
				current = ClrSecurity.GetPermissionsFrom(skipFrames);
				PermissionSet set = new PermissionSet(PermissionState.None);
				set.AddPermission(this.Copy());
				if(current == null)
				{
					// Initialize the permissions context to "allow
					// everything except this permission object".
					current = new ClrPermissions
						(new PermissionSet(PermissionState.Unrestricted),
						 set, null);
				}
				else
				{
					current = current.SetDenied(set.Union(current.denied));
				}
				ClrSecurity.SetPermissions(current, skipFrames);
			}
	public void Deny()
			{
				Deny(2);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return ToXml().ToString();
			}

	// Convert an XML value into a permissions value.
	public abstract void FromXml(SecurityElement elem);

	// Convert this permissions object into an XML value.
	public abstract SecurityElement ToXml();

	// Implement the IPermission interface.
	public abstract IPermission Copy();
	public void Demand()
			{
				// Get the current permission state.
				ClrPermissions current = ClrSecurity.GetPermissionsFrom(1);
				if(current == null)
				{
					// Null is equivalent to "unrestricted".
					return;
				}

				// Build a permission set with just this permission.
				PermissionSet set = new PermissionSet(PermissionState.None);
				set.AddPermission(this);

				// If "PermitOnly" is set, then only check that set.
				if(current.permitOnly != null)
				{
					if(!set.IsSubsetOf(current.permitOnly))
					{
						throw new SecurityException
							(_("Exception_SecurityNotGranted"));
					}
					return;
				}

				// The permission must be granted, but not denied.
				if(!set.IsSubsetOf(current.granted) ||
				   set.IsSubsetOf(current.denied))
				{
					throw new SecurityException
						(_("Exception_SecurityNotGranted"));
				}
			}
	public abstract IPermission Intersect(IPermission target);
	public abstract bool IsSubsetOf(IPermission target);
	public virtual IPermission Union(IPermission other)
			{
				if(other == null)
				{
					return Copy();
				}
				else
				{
					throw new NotSupportedException
						(_("NotSupp_IPermissionUnion"));
				}
			}

#if !ECMA_COMPAT

	// Revert all permissions for the caller.
	public static void RevertAll()
			{
				ClrPermissions current = ClrSecurity.GetPermissions(1);
				ClrPermissions parent = ClrSecurity.GetPermissionsFrom(2);
				if(current != null)
				{
					ClrSecurity.SetPermissions(parent, 1);
				}
			}

	// Revert all assertions for the caller.
	public static void RevertAssert()
			{
				ClrPermissions current = ClrSecurity.GetPermissions(1);
				ClrPermissions parent = ClrSecurity.GetPermissionsFrom(2);
				if(current != null)
				{
					if(parent != null)
					{
						ClrSecurity.SetPermissions
							(current.SetGranted(parent.granted), 1);
					}
					else
					{
						ClrSecurity.SetPermissions
							(current.SetGranted
								(new PermissionSet
									(PermissionState.Unrestricted)), 1);
					}
				}
			}

	// Revert all denials for the caller.
	public static void RevertDeny()
			{
				ClrPermissions current = ClrSecurity.GetPermissions(1);
				ClrPermissions parent = ClrSecurity.GetPermissionsFrom(2);
				if(current != null)
				{
					if(parent != null)
					{
						ClrSecurity.SetPermissions
							(current.SetDenied(parent.denied), 1);
					}
					else
					{
						ClrSecurity.SetPermissions
							(current.SetDenied
								(new PermissionSet(PermissionState.None)), 1);
					}
				}
			}

	// Revert all "PermitOnly" permissions for the caller.
	public static void RevertPermitOnly()
			{
				ClrPermissions current = ClrSecurity.GetPermissions(1);
				ClrPermissions parent = ClrSecurity.GetPermissionsFrom(2);
				if(current != null)
				{
					if(parent != null)
					{
						ClrSecurity.SetPermissions
							(current.SetPermitOnly(parent.permitOnly), 1);
					}
					else
					{
						ClrSecurity.SetPermissions
							(current.SetPermitOnly(null), 1);
					}
				}
			}

#endif // !ECMA_COMPAT

	// Set the caller's permissions to only this object.
	internal static void PermitOnly(PermissionSet set, int skipFrames)
			{
				// Make sure that we don't already have a "PermitOnly" value.
				ClrPermissions current;
				current = ClrSecurity.GetPermissionsFrom(skipFrames);
				if(current != null && current.permitOnly != null)
				{
					throw new SecurityException(_("Exception_PermitOnly"));
				}

				// Add the "PermitOnly" set to the call stack.
				if(current == null)
				{
					// Initialize the permissions context to "allow
					// only this permission object".
					current = new ClrPermissions
						(new PermissionSet(PermissionState.Unrestricted),
						 new PermissionSet(PermissionState.None),
						 set);
				}
				else
				{
					current = current.SetPermitOnly(set);
				}
				ClrSecurity.SetPermissions(current, skipFrames);
			}
#if ECMA_COMPAT
	void IStackWalk.PermitOnly()
#else
	public void PermitOnly()
#endif
			{
				// Demand the permission first, because we cannot permit it
				// for exclusive access if we are not allowed have it at all.
				Demand();

				// Create a permission set for this object and then add it.
				PermissionSet set = new PermissionSet(PermissionState.None);
				set.AddPermission(this.Copy());
				PermitOnly(set, 2);
			}

}; // class CodeAccessPermission

#endif // CONFIG_PERMISSIONS

}; // namespace System.Security
