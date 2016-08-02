/*
 * IsolatedStorageFilePermission.cs - Implementation of the
 *		"System.Security.Permissions.IsolatedStorageFilePermission" class.
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

namespace System.Security.Permissions
{

#if CONFIG_PERMISSIONS && !ECMA_COMPAT

using System;
using System.Security;

public sealed class IsolatedStorageFilePermission : IsolatedStoragePermission
{

	// Constructors.
	public IsolatedStorageFilePermission(PermissionState state)
			: base(state)
			{
				// Nothing to do here.
			}
	internal IsolatedStorageFilePermission(IsolatedStoragePermission copyFrom)
			: base(copyFrom)
			{
				// Nothing to do here.
			}
	internal IsolatedStorageFilePermission
					(IsolatedStoragePermissionAttribute copyFrom)
			: base(copyFrom)
			{
				// Nothing to do here.
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new IsolatedStorageFilePermission(this);
			}
	public override IPermission Intersect(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return target;
				}
				else if(!(target is IsolatedStorageFilePermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((IsolatedStorageFilePermission)target)
							.IsUnrestricted())
				{
					return Copy();
				}
				else if(IsUnrestricted())
				{
					return target.Copy();
				}

				// Get the minimum quota and containment values.
				long quota = ((IsolatedStorageFilePermission)target).userQuota;
				if(quota > userQuota)
				{
					quota = userQuota;
				}
				IsolatedStorageContainment allowed;
				allowed = ((IsolatedStorageFilePermission)target).usageAllowed;
				if(((int)allowed) > ((int)usageAllowed))
				{
					allowed = usageAllowed;
				}

				// Create a new object and intersect the lists.
				IsolatedStorageFilePermission perm;
				perm = new IsolatedStorageFilePermission(PermissionState.None);
				perm.userQuota = quota;
				perm.usageAllowed = allowed;
				return perm;
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (state == PermissionState.None &&
							userQuota == 0 && usageAllowed ==
								IsolatedStorageContainment.None);
				}
				else if(!(target is IsolatedStorageFilePermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((IsolatedStorageFilePermission)target)
							.IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				else if(userQuota > ((IsolatedStorageFilePermission)target)
										.userQuota)
				{
					return false;
				}
				else if(((int)usageAllowed) >
							((int)(((IsolatedStorageFilePermission)target)
										.usageAllowed)))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
	public override IPermission Union(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is IsolatedStorageFilePermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((IsolatedStorageFilePermission)target)
							.IsUnrestricted())
				{
					return new IsolatedStorageFilePermission
						(PermissionState.Unrestricted);
				}

				// Get the maximum quota and containment values.
				long quota = ((IsolatedStorageFilePermission)target).userQuota;
				if(quota < userQuota)
				{
					quota = userQuota;
				}
				IsolatedStorageContainment allowed;
				allowed = ((IsolatedStorageFilePermission)target).usageAllowed;
				if(((int)allowed) < ((int)usageAllowed))
				{
					allowed = usageAllowed;
				}

				// Create a new object and intersect the lists.
				IsolatedStorageFilePermission perm;
				perm = new IsolatedStorageFilePermission(PermissionState.None);
				perm.userQuota = quota;
				perm.usageAllowed = allowed;
				return perm;
			}

}; // class IsolatedStorageFilePermission

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
