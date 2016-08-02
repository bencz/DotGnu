/*
 * SecurityRoleAttribute.cs - Implementation of the
 *			"System.EnterpriseServices.SecurityRoleAttribute" class.
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

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
[AttributeUsage(AttributeTargets.Assembly |
				AttributeTargets.Class |
				AttributeTargets.Method |
				AttributeTargets.Interface,
				Inherited=true, AllowMultiple=true)]
public sealed class SecurityRoleAttribute : Attribute
{
	// Internal state.
	private String description;
	private String role;
	private bool everyone;

	// Constructor.
	public SecurityRoleAttribute(String role) : this(role, false) {}
	public SecurityRoleAttribute(String role, bool everyone)
			{
				this.role = role;
				this.everyone = everyone;
			}

	// Get or set this attribute's values.
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
	public String Role
			{
				get
				{
					return role;
				}
				set
				{
					role = value;
				}
			}
	public bool SetEveryoneAccess
			{
				get
				{
					return everyone;
				}
				set
				{
					everyone = value;
				}
			}

}; // class SecurityRoleAttribute

}; // namespace System.EnterpriseServices
