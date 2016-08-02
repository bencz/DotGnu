/*
 * DnsPermission.cs - Implementation of the "System.Net.DnsPermission" class.
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 *
 * Contributed by Jason Lee <jason.lee@mac.com>
 * 
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.Net
{

#if CONFIG_PERMISSIONS

using System;
using System.Security;
using System.Security.Permissions;

public 
#if !ECMA_COMPAT
sealed
#endif
class DnsPermission : CodeAccessPermission
#if !ECMA_COMPAT
	, IUnrestrictedPermission
#endif
{

	bool restrictedState = true;

	// Default constructor with option of setting the restricted state
	// to either unrestricted (1) or fully restricted (0)
	public DnsPermission (PermissionState state)
	{
		if(state == PermissionState.Unrestricted)
		{
			restrictedState = false;
		}
		else
		{
			if(state != PermissionState.None)
			{
				throw new ArgumentException("Not a valid state value");
			}
		}
		
	}

	// Methods

	public override IPermission Copy()
	{
		return this;
	}

	public override void FromXml(SecurityElement esd)
	{
		String value;
		if(esd == null)
		{
			throw new ArgumentNullException("esd");
		}
		if(esd.Attribute("version") != "1")
		{
			throw new ArgumentException(S._("Arg_PermissionVersion"));
		}
		value = esd.Attribute("Unrestricted");
		if(value != null && Boolean.Parse(value))
		{
			restrictedState = false;
		}
		else
		{
			restrictedState = true;
		}
	}


	public override IPermission Intersect(IPermission target)
	{
		if(target == null)
		{
			throw new ArgumentNullException("IPermission");
		}

		DnsPermission newTarget = target as DnsPermission;
		
		if(newTarget == null)
		{
			throw new ArgumentException();
		}

		if(!restrictedState)
		{
			return newTarget;
		}

		if(!newTarget.restrictedState)
		{
			return this;
		}

		return null;
		
	}

	public override bool IsSubsetOf(IPermission target)
	{
		if(target == null)
		{
			return false;
		}

		DnsPermission newTarget = target as DnsPermission;
		
		if(newTarget == null)
		{
			throw new ArgumentException();
		}
	
		if(!restrictedState)
		{
			if(!newTarget.restrictedState)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		return true;
		
	}

	public override SecurityElement ToXml()
	{
		SecurityElement element;
		element = new SecurityElement("IPermission");
		element.AddAttribute
			("class",
			 SecurityElement.Escape(typeof(DnsPermission).
			 						AssemblyQualifiedName));
		element.AddAttribute("version", "1");
		if(!restrictedState)
		{
			element.AddAttribute("Unrestricted", "true");
		}
		return element;
	}

	public override IPermission Union(IPermission target)
	{
		if(target == null)
		{
			throw new ArgumentNullException("IPermission");
		}

		DnsPermission newTarget = target as DnsPermission;
		if(newTarget == null)
		{
			throw new ArgumentException();
		}
	
		if(IsSubsetOf(newTarget))
		{
			if((!newTarget.restrictedState) || !restrictedState)
			{
				return newTarget;
			}		
		}
		
		return null;
	}

#if !ECMA_COMPAT
	// Determine if this permission object is unrestricted.
	public bool IsUnrestricted()
	{
		return !restrictedState;
	}
#endif

} // class DnsPermission

#endif // CONFIG_PERMISSIONS

} // namespace System.Net
