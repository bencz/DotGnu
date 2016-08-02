/*
 * PrincipalPermission.cs - Implementation of the
 *		"System.Security.Permissions.PrincipalPermission" class.
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

#if CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && !ECMA_COMPAT

using System;
using System.Collections;
using System.Security;

public sealed class PrincipalPermission
	: IPermission, ISecurityEncodable, IUnrestrictedPermission
{
	// Principal information.
	private sealed class PrincipalInfo
	{
		// Accessible internal state.
		public String name;
		public String role;
		public bool isAuthenticated;

		// Constructors.
		public PrincipalInfo() {}
		public PrincipalInfo(String name, String role, bool isAuthenticated)
				{
					this.name = name;
					this.role = role;
					this.isAuthenticated = isAuthenticated;
				}
		public PrincipalInfo(SecurityElement elem)
				{
					name = elem.Attribute("ID");
					role = elem.Attribute("Role");
					String value = elem.Attribute("Authenticated");
					isAuthenticated = (value != null && Boolean.Parse(value));
				}

		// Convert this principal information block into an XML blob.
		public SecurityElement ToXml()
				{
					SecurityElement elem = new SecurityElement("identity");
					if(name != null)
					{
						elem.AddAttribute
							("ID", SecurityElement.Escape(name));
					}
					if(role != null)
					{
						elem.AddAttribute
							("Role", SecurityElement.Escape(role));
					}
					elem.AddAttribute
						("Authenticated", isAuthenticated.ToString());
					return elem;
				}

	}; // class PrincipalInfo

	// Internal state.
	private PermissionState state;
	private ArrayList principals;

	// Constructor.
	public PrincipalPermission(PermissionState state)
			{
				if(state != PermissionState.Unrestricted &&
				   state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				this.state = state;
				principals = new ArrayList();
				if(state == PermissionState.Unrestricted)
				{
					principals.Add(new PrincipalInfo(null, null, true));
				}
				else
				{
					principals.Add(new PrincipalInfo("", "", false));
				}
			}
	public PrincipalPermission(String name, String role)
			{
				this.state = PermissionState.None;
				principals = new ArrayList();
				principals.Add(new PrincipalInfo(name, role, true));
			}
	public PrincipalPermission(String name, String role, bool isAuthenticated)
			{
				this.state = PermissionState.None;
				principals = new ArrayList();
				principals.Add(new PrincipalInfo(name, role, isAuthenticated));
			}
	private PrincipalPermission(PrincipalPermission copyFrom, bool copyChildren)
			{
				state = copyFrom.state;
				if(copyChildren)
				{
					principals = (ArrayList)(copyFrom.principals.Clone());
				}
				else
				{
					principals = new ArrayList();
				}
			}

	// Convert an XML value into a permissions value.
	public void FromXml(SecurityElement esd)
			{
				String value;
				if(esd == null)
				{
					throw new ArgumentNullException("esd");
				}
				if(esd.Attribute("version") != "1")
				{
					throw new ArgumentException(_("Arg_PermissionVersion"));
				}
				value = esd.Attribute("Unrestricted");
				if(value != null && Boolean.Parse(value))
				{
					state = PermissionState.Unrestricted;
				}
				else
				{
					state = PermissionState.None;
				}
				principals.Clear();
				ArrayList children = esd.Children;
				if(children != null)
				{
					foreach(SecurityElement e in children)
					{
						if(e.Tag != "Identity")
						{
							continue;
						}
						principals.Add(new PrincipalInfo(e));
					}
				}
			}

	// Convert this permissions object into an XML value.
	public SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(PrincipalPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(state == PermissionState.Unrestricted)
				{
					element.AddAttribute("Unrestricted", "true");
				}
				foreach(PrincipalInfo prin in principals)
				{
					element.AddChild(prin.ToXml());
				}
				return element;
			}

	// Find a specific role object.
	private PrincipalInfo Find(String name, String role)
			{
				foreach(PrincipalInfo prin in principals)
				{
					if(prin.name == name && prin.role == role)
					{
						return prin;
					}
				}
				return null;
			}

	// Throw an exception if the caller does not have
	// the specified permissions.
	public void Demand()
			{
				// We don't use principals for security purposes here.
			}

	// Implement the IPermission interface.
	public IPermission Copy()
			{
				return new PrincipalPermission(this, true);
			}
	public IPermission Intersect(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return target;
				}
				else if(!(target is PrincipalPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((PrincipalPermission)target).IsUnrestricted())
				{
					if(IsUnrestricted())
					{
						return Copy();
					}
				}
				else if(IsUnrestricted())
				{
					return target.Copy();
				}

				// Form the intersection of the two principal lists.
				PrincipalPermission perm = new PrincipalPermission(this, false);
				PrincipalInfo other, newPrin;
				foreach(PrincipalInfo prin in principals)
				{
					other = Find(prin.name, prin.role);
					if(other != null)
					{
						newPrin = new PrincipalInfo();
						newPrin.name = prin.name;
						newPrin.role = prin.role;
						newPrin.isAuthenticated = prin.isAuthenticated &&
												  other.isAuthenticated;
						perm.principals.Add(newPrin);
					}
				}
				return perm;
			}
	public bool IsSubsetOf(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return (principals.Count == 0);
				}
				else if(!(target is PrincipalPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((PrincipalPermission)target).IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}

				// Check that all source roles are in the target.
				PrincipalInfo other;
				foreach(PrincipalInfo prin in principals)
				{
					other = ((PrincipalPermission)target).Find
						(prin.name, prin.role);
					if(other == null)
					{
						return false;
					}
					if(other.isAuthenticated != prin.isAuthenticated)
					{
						return false;
					}
				}

				// This is a subset.
				return true;
			}
	public IPermission Union(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is PrincipalPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((PrincipalPermission)target).IsUnrestricted())
				{
					return new PrincipalPermission
						(PermissionState.Unrestricted);
				}

				// Form the union of the two lists.
				PrincipalPermission perm =
					new PrincipalPermission(this, true);
				PrincipalInfo other, newPrin;
				foreach(PrincipalInfo prin in ((PrincipalPermission)target)
													.principals)
				{
					other = perm.Find(prin.name, prin.role);
					if(other == null)
					{
						newPrin = new PrincipalInfo();
						newPrin.name = prin.name;
						newPrin.role = prin.role;
						newPrin.isAuthenticated = prin.isAuthenticated;
						perm.principals.Add(newPrin);
					}
					else
					{
						other.isAuthenticated =
							(prin.isAuthenticated || other.isAuthenticated);
					}
				}
				return perm;
			}

	// Determine if this object has unrestricted permissions.
	public bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return ToXml().ToString();
			}

}; // class PrincipalPermission

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
