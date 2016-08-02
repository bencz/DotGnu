/*
 * InterfaceQueuingAttribute.cs - Implementation of the
 *			"System.EnterpriseServices.InterfaceQueuingAttribute" class.
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
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
				Inherited=true, AllowMultiple=true)]
public sealed class InterfaceQueuingAttribute : Attribute
{
	// Internal state.
	private bool enabled;
	private String interfaceString;

	// Constructors.
	public InterfaceQueuingAttribute() : this(true) {}
	public InterfaceQueuingAttribute(bool val)
			{
				this.enabled = enabled;
			}

	// Get this attribute's value.
	public bool Enabled
			{
				get
				{
					return enabled;
				}
				set
				{
					enabled = value;
				}
			}
	public String Interface
			{
				get
				{
					return interfaceString;
				}
				set
				{
					interfaceString = value;
				}
			}

}; // class IInterfaceQueuingAttribute

}; // namespace System.EnterpriseServices
