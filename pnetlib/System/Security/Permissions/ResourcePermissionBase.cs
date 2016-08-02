/*
 * ResourcePermissionBase.cs - Implementation of the
 *			"System.Diagnostics.ResourcePermissionBase" class.
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

using System.Collections;

[Serializable]
public abstract class ResourcePermissionBase
	: CodeAccessPermission, IUnrestrictedPermission
{
	// Internal state.
	private PermissionState state;
	private Type accessType;
	private String[] tagNames;
	private ArrayList permissions;

	// Special resource values.
	public const String Any = "*";
	public const String Local = ".";

	// Constructors.
	protected ResourcePermissionBase()
			{
				this.state = PermissionState.None;
				this.permissions = new ArrayList();
			}
	protected ResourcePermissionBase(PermissionState state)
			{
				this.state = state;
				this.permissions = new ArrayList();
			}

	// Make a copy of this permission object.
	public override IPermission Copy()
			{
				ResourcePermissionBase perm =
					(ResourcePermissionBase)
						Activator.CreateInstance(GetType());
				perm.state = state;
				perm.accessType = accessType;
				perm.tagNames = tagNames;
				perm.permissions.AddRange(permissions);
				return perm;
			}

	// Convert an XML element into a permission object.
	public override void FromXml(SecurityElement securityElement)
			{
				String value;
				if(securityElement == null)
				{
					throw new ArgumentNullException("securityElement");
				}
				if(securityElement.Attribute("version") != "1")
				{
					throw new ArgumentException(S._("Arg_PermissionVersion"));
				}
				value = securityElement.Attribute("Unrestricted");
				Clear();
				if(value != null && Boolean.Parse(value))
				{
					state = PermissionState.Unrestricted;
				}
				else
				{
					// TODO: read the children
				}
			}

	// Form the intersection of two permission objects.
	public override IPermission Intersect(IPermission target)
			{
				if(target == null)
				{
					return target;
				}
				else if(target.GetType() != GetType())
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(((ResourcePermissionBase)target).IsUnrestricted())
				{
					return Copy();
				}
				else if(IsUnrestricted())
				{
					return target.Copy();
				}
				ResourcePermissionBase perm;
				perm = (ResourcePermissionBase)(target.Copy());
				perm.Clear();
				foreach(ResourcePermissionBaseEntry entry in permissions)
				{
					if(((ResourcePermissionBase)target).Contains(entry))
					{
						perm.AddPermissionAccess(entry);
					}
				}
				return perm;
			}

	// Determine if this permission object is a subset of another.
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (state == PermissionState.None &&
							permissions.Count == 0);
				}
				else if(target.GetType() != GetType())
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(((ResourcePermissionBase)target).IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				foreach(ResourcePermissionBaseEntry entry in permissions)
				{
					if(!((ResourcePermissionBase)target).Contains(entry))
					{
						return false;
					}
				}
				return true;
			}

	// Determine if this permission object is unrestricted.
	public bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}

	// Convert this permission object into an XML element.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(GetType().AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(state == PermissionState.Unrestricted)
				{
					element.AddAttribute("Unrestricted", "true");
				}
				else
				{
					// TODO: add the children.
				}
				return element;
			}

	// Form the union of two permission objects.
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(target.GetType() != GetType())
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((ResourcePermissionBase)target).IsUnrestricted())
				{
					return (ResourcePermissionBase)Activator.CreateInstance
						(GetType(),
						 new Object [] {PermissionState.Unrestricted});
				}
				else
				{
					ResourcePermissionBase perm;
					perm = (ResourcePermissionBase)(target.Copy());
					foreach(ResourcePermissionBaseEntry entry in permissions)
					{
						if(!perm.Contains(entry))
						{
							perm.AddPermissionAccess(entry);
						}
					}
					return perm;
				}
			}

	// Get or set the permission access type.
	protected Type PermissionAccessType
			{
				get
				{
					return accessType;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					else if(!(value.IsEnum))
					{
						throw new ArgumentException
							(S._("Arg_NotEnumType"));
					}
					accessType = value;
				}
			}

	// Get or set the tag names used by this type of permission.
	protected String[] TagNames
			{
				get
				{
					return tagNames;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					else if(value.Length == 0)
					{
						throw new ArgumentException
							(S._("Arg_EmptyArray"));
					}
					tagNames = value;
				}
			}

	// Determine if a particular entry is in this permission set.
	private bool Contains(ResourcePermissionBaseEntry entry)
			{
				foreach(ResourcePermissionBaseEntry e in permissions)
				{
					if(String.Compare(e.StringPath, entry.StringPath, true)
							== 0)
					{
						return true;
					}
				}
				return false;
			}

	// Add a permission entry to this object.
	protected void AddPermissionAccess(ResourcePermissionBaseEntry entry)
			{
				if(entry == null)
				{
					throw new ArgumentNullException("entry");
				}
				else if(tagNames == null ||
				        entry.PermissionAccessPath.Length != tagNames.Length)
				{
					throw new InvalidOperationException
						(S._("Invalid_PathMismatch"));
				}
				else if(Contains(entry))
				{
					throw new InvalidOperationException
						(S._("Invalid_PermissionPresent"));
				}
				permissions.Add(entry);
			}

	// Clear all permission entries from this object.
	protected void Clear()
			{
				permissions.Clear();
			}

	// Get the permission entries in this object.
	protected ResourcePermissionBaseEntry[] GetPermissionEntries()
			{
				ResourcePermissionBaseEntry[] array;
				array = new ResourcePermissionBaseEntry [permissions.Count];
				permissions.CopyTo(array, 0);
				return array;
			}

	// Remove a permission entry from this object.
	protected void RemovePermissionAccess(ResourcePermissionBaseEntry entry)
			{
				if(entry == null)
				{
					throw new ArgumentNullException("entry");
				}
				else if(tagNames == null ||
				        entry.PermissionAccessPath.Length != tagNames.Length)
				{
					throw new InvalidOperationException
						(S._("Invalid_PathMismatch"));
				}
				else if(!permissions.Contains(entry))
				{
					throw new InvalidOperationException
						(S._("Invalid_PermissionNotPresent"));
				}
				permissions.Remove(entry);
			}

}; // class ResourcePermissionBase

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
