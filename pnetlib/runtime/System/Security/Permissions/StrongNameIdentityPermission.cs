/*
 * StrongNameIdentityPermission.cs - Implementation of the
 *		"System.Security.Permissions.StrongNameIdentityPermission" class.
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
using System.Security;

public sealed class StrongNameIdentityPermission : CodeAccessPermission
{
	// Internal state.
	private StrongNamePublicKeyBlob blob;
	private String name;
	private Version version;

	// Constructor.
	public StrongNameIdentityPermission(PermissionState state)
			{
				if(state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				blob = null;
				name = "";
				version = new Version();
			}
	public StrongNameIdentityPermission(StrongNamePublicKeyBlob blob,
										String name, Version version)
			{
				if(blob == null)
				{
					throw new ArgumentNullException("blob");
				}
				this.blob = blob;
				this.name = name;
				this.version = version;
			}

	// Convert an XML value into a permissions value.
	public override void FromXml(SecurityElement esd)
			{
				if(esd == null)
				{
					throw new ArgumentNullException("esd");
				}
				if(esd.Attribute("version") != "1")
				{
					throw new ArgumentException(_("Arg_PermissionVersion"));
				}
				name = esd.Attribute("Name");
				String value = esd.Attribute("Version");
				if(value != null)
				{
					version = new Version(value);
				}
				else
				{
					version = null;
				}
				value = esd.Attribute("PublicKeyBlob");
				if(value != null)
				{
					blob = new StrongNamePublicKeyBlob(value);
				}
				else
				{
					blob = null;
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape
					 	(typeof(StrongNameIdentityPermission).
								AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(blob != null)
				{
					element.AddAttribute("PublicKeyBlob", blob.ToString());
				}
				if(name != null)
				{
					element.AddAttribute
						("Name", SecurityElement.Escape(name));
				}
				if(version != null)
				{
					element.AddAttribute("Version", version.ToString());
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				if(blob == null)
				{
					return new StrongNameIdentityPermission
						(PermissionState.None);
				}
				else
				{
					return new StrongNameIdentityPermission
						(blob, name, version);
				}
			}
	public override IPermission Intersect(IPermission target)
			{
				if(target == null)
				{
					return target;
				}
				else if(!(target is StrongNameIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsSubsetOf(target))
				{
					return Copy();
				}
				else if(target.IsSubsetOf(this))
				{
					return target.Copy();
				}
				else
				{
					return null;
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return (blob == null);
				}
				else if(!(target is StrongNameIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}

				// Check blob subset conditions.
				StrongNameIdentityPermission t;
				t = ((StrongNameIdentityPermission)target);
				if(blob != null && !blob.Equals(t.blob))
				{
					return false;
				}

				// Check name subset conditions.
				if(name != null && name != t.name)
				{
					return false;
				}

				// Check version subset conditions.
				if(version != null && version != t.version)
				{
					return false;
				}

				// It is a subset.
				return true;
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is StrongNameIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsSubsetOf(target))
				{
					return target.Copy();
				}
				else if(target.IsSubsetOf(this))
				{
					return Copy();
				}
				else
				{
					return null;
				}
			}

	// Get or set the public key blob value.
	public StrongNamePublicKeyBlob PublicKey
			{
				get
				{
					return blob;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					blob = value;
				}
			}

	// Get or set the name.
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

	// Get or set the version.
	public Version Version
			{
				get
				{
					return version;
				}
				set
				{
					version = value;
				}
			}

}; // class StrongNameIdentityPermission

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
