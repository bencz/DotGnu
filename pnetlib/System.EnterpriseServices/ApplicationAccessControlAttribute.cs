/*
 * ApplicationAccessControlAttribute.cs - Implementation of the
 *		"System.EnterpriseServices.ApplicationAccessControlAttribute" class.
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
[AttributeUsage(AttributeTargets.Assembly, Inherited=true)]
public sealed class ApplicationAccessControlAttribute : Attribute
{
	// Internal state.
	private bool val;
	private AccessChecksLevelOption accessChecksLevel;
	private AuthenticationOption authentication;
	private ImpersonationLevelOption impersonationLevel;

	// Constructors.
	public ApplicationAccessControlAttribute() : this(true) {}
	public ApplicationAccessControlAttribute(bool val)
			{
				this.val = val;
			}

	// Get or set this attribute's values.
	public AccessChecksLevelOption AccessChecksLevel
			{
				get
				{
					return accessChecksLevel;
				}
				set
				{
					accessChecksLevel = value;
				}
			}
	public AuthenticationOption Authentication
			{
				get
				{
					return authentication;
				}
				set
				{
					authentication = value;
				}
			}
	public ImpersonationLevelOption ImpersonationLevel
			{
				get
				{
					return impersonationLevel;
				}
				set
				{
					impersonationLevel = value;
				}
			}
	public bool Value
			{
				get
				{
					return val;
				}
			}

}; // class ApplicationAccessControlAttribute

}; // namespace System.EnterpriseServices
