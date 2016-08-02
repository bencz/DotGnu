/*
 * NamedPermissionSet.cs - Implementation of the
 *		"System.Security.NamedPermissionSet" class.
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

#if CONFIG_PERMISSIONS && !ECMA_COMPAT

using System;
using System.Collections;
using System.Security.Permissions;

public sealed class NamedPermissionSet : PermissionSet
{

	// Internal state.
	private String name;
	private String description;

	// Constructors.
	public NamedPermissionSet(String name)
			: base(PermissionState.Unrestricted)
			{
				if(name == null || name == String.Empty)
				{
					throw new ArgumentException(_("Invalid_PermissionSetName"));
				}
				this.name = name;
			}
	public NamedPermissionSet(String name, PermissionState state)
			: base(state)
			{
				if(name == null || name == String.Empty)
				{
					throw new ArgumentException(_("Invalid_PermissionSetName"));
				}
				this.name = name;
			}
	public NamedPermissionSet(String name, PermissionSet permSet)
			: base(permSet)
			{
				if(name == null || name == String.Empty)
				{
					throw new ArgumentException(_("Invalid_PermissionSetName"));
				}
				this.name = name;
			}
	public NamedPermissionSet(NamedPermissionSet permSet)
			: base(permSet)
			{
				this.name = permSet.name;
				this.description = permSet.description;
			}

	// Get or set the name of this permission set.
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					if(value == null || value == String.Empty)
					{
						throw new ArgumentException
							(_("Invalid_PermissionSetName"));
					}
					name = value;
				}
			}

	// Get or set the description of this permission set.
	public String Description
			{
				get
				{
					return description;
				}
				set
				{
					description = value;
				}
			}

	// Return a copy of this permission set.
	public override PermissionSet Copy()
			{
				return new NamedPermissionSet(this);
			}

	// Copy this permission set and give the copy a new name.
	public NamedPermissionSet Copy(String name)
			{
				NamedPermissionSet permSet = new NamedPermissionSet(this);
				permSet.Name = name;
				return permSet;
			}

	// Convert this permission set into an XML security element.
	public override SecurityElement ToXml()
			{
				SecurityElement elem = base.ToXml();
				if(name != null && name != String.Empty)
				{
					elem.AddAttribute("Name", SecurityElement.Escape(name));
				}
				if(description != null && description != String.Empty)
				{
					elem.AddAttribute
						("Description", SecurityElement.Escape(description));
				}
				return elem;
			}

	// Convert an XML security element into a permission set.
	public override void FromXml(SecurityElement et)
			{
				base.FromXml(et);
				name = et.Attribute("Name");
				description = et.Attribute("Description");
				if(description == null)
				{
					description = String.Empty;
				}
			}

	// Copy the contents of another permission set into this one.
	internal override void CopyFrom(PermissionSet pSet)
			{
				base.CopyFrom(pSet);
				if(pSet is NamedPermissionSet)
				{
					description = ((NamedPermissionSet)pSet).Description;
				}
			}

}; // class NamedPermissionSet

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security
