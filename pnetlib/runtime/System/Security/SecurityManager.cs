/*
 * SecurityManager.cs - Implementation of the
 *		"System.Security.SecurityManager" class.
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

namespace System.Security
{

#if !ECMA_COMPAT && (CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS)

using System.Collections;
using System.Security.Policy;
using System.Security.Permissions;

public sealed class SecurityManager
{
	// Cannot instantiate this class.
	private SecurityManager() {}

#if CONFIG_PERMISSIONS

	// Determine if a specific permission has been granted.
	public static bool IsGranted(IPermission perm)
			{
				// Bail out if the requested permission is null.
				if(perm == null)
				{
					return true;
				}

				// Get the current permission state.
				ClrPermissions current = ClrSecurity.GetPermissionsFrom(1);
				if(current == null)
				{
					// Null is equivalent to "unrestricted".
					return true;
				}

				// Build a permission set with just this permission.
				PermissionSet set = new PermissionSet(PermissionState.None);
				set.AddPermission(perm);

				// If "PermitOnly" is set, then only check that set.
				if(current.permitOnly != null)
				{
					return set.IsSubsetOf(current.permitOnly);
				}

				// The permission must be granted, but not denied.
				if(!set.IsSubsetOf(current.granted) ||
				   set.IsSubsetOf(current.denied))
				{
					return false;
				}
				return true;
			}

#endif

#if CONFIG_POLICY_OBJECTS

	// Load policy level information from a file.
	public static PolicyLevel LoadPolicyLevelFromFile
				(String path, PolicyLevelType type)
			{
				// Not used in this implementation.
				return null;
			}

	// Load policy level information from a string.
	public static PolicyLevel LoadPolicyLevelFromString
				(String str, PolicyLevelType type)
			{
				// Not used in this implementation.
				return null;
			}

	// Get an enumerator for the policy hierarchy.
	public static IEnumerator PolicyHierarchy()
			{
				// Not used in this implementation.
				return null;
			}

	// Save a particular policy level.
	public static void SavePolicyLevel(PolicyLevel level)
			{
				// Not used in this implementation.
			}

#if CONFIG_PERMISSIONS

	// Resolve policy information.
	public static PermissionSet ResolvePolicy
				(Evidence evidence, PermissionSet reqdPset,
				 PermissionSet optPset, PermissionSet denyPset,
				 out PermissionSet denied)
			{
				// Not used in this implementation.
				denied = null;
				return null;
			}
	public static PermissionSet ResolvePolicy(Evidence evidence)
			{
				// Not used in this implementation.
				return null;
			}

#endif

	// Resolve policy group information.
	public static IEnumerator ResolvePolicyGroups(Evidence evidence)
			{
				// Not used in this implementation.
				return null;
			}

	// Save policy information.
	public static void SavePolicy()
			{
				// Not used in this implementation.
			}

	// Get or set the execution rights flag.
	public static bool CheckExecutionRights
			{
				get
				{
					// Not used in this implementation.
					return true;
				}
				set
				{
					// Not used in this implementation.
				}
			}

	// Determine if security features have been enabled.
	public static bool SecurityEnabled
			{
				get
				{
					// Not used in this implementation.
					return true;
				}
				set
				{
					// Not used in this implementation.
				}
			}

	// Get the zone and origin information
	public static void GetZoneAndOrigin(out ArrayList zone,
										out ArrayList origin)
			{
				// Not used in this implementation.
				zone = null;
				origin = null;
			}

#endif // CONFIG_POLICY_OBJECTS

}; // class SecurityManager

#endif // !ECMA_COMPAT && (CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS)


}; // namespace System.Security
