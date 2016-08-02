/*
 * ReflectionPermission.cs - Implementation of the
 *		"System.Security.Permissions.ReflectionPermission" class.
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

#if CONFIG_PERMISSIONS && CONFIG_REFLECTION

using System;
using System.Security;

public sealed class ReflectionPermission
	: CodeAccessPermission, IUnrestrictedPermission
{
	// Internal state.
	private PermissionState state;
	private ReflectionPermissionFlag flags;

	// Constructor.
	public ReflectionPermission(PermissionState state)
			{
				this.state = state;
			}
	public ReflectionPermission(ReflectionPermissionFlag flags)
			{
				this.flags = flags;
			}

	// Convert an XML value into a permissions value.
	public override void FromXml(SecurityElement esd)
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
				value = esd.Attribute("Flags");
				if(value != null)
				{
					flags = (ReflectionPermissionFlag)
						Enum.Parse(typeof(ReflectionPermissionFlag), value);
				}
				else
				{
					flags = ReflectionPermissionFlag.NoFlags;
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(ReflectionPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(flags != ReflectionPermissionFlag.NoFlags)
				{
					element.AddAttribute("Flags", flags.ToString());
				}
				else if(state == PermissionState.Unrestricted)
				{
					element.AddAttribute("Unrestricted", "true");
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				if(flags != ReflectionPermissionFlag.NoFlags)
				{
					return new ReflectionPermission(flags);
				}
				else
				{
					return new ReflectionPermission(state);
				}
			}
	public override IPermission Intersect(IPermission target)
			{
				ReflectionPermissionFlag newFlags;
				if(target == null)
				{
					return target;
				}
				else if(!(target is ReflectionPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((ReflectionPermission)target).IsUnrestricted())
				{
					if(IsUnrestricted())
					{
						return Copy();
					}
					else
					{
						newFlags = flags;
					}
				}
				else if(IsUnrestricted())
				{
					newFlags = ((ReflectionPermission)target).flags;
				}
				else
				{
					newFlags = ((ReflectionPermission)target).flags & flags;
				}
				if(newFlags == 0)
				{
					return null;
				}
				else
				{
					return new ReflectionPermission(newFlags);
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (flags == ReflectionPermissionFlag.NoFlags);
				}
				else if(!(target is ReflectionPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((ReflectionPermission)target).IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				else
				{
					return ((flags & ~(((ReflectionPermission)target).flags))
								== 0);
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is ReflectionPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((ReflectionPermission)target).IsUnrestricted())
				{
					return new ReflectionPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new ReflectionPermission
						(flags | ((ReflectionPermission)target).flags);
				}
			}

	// Determine if this object has unrestricted permissions.
#if ECMA_COMPAT
	private bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}
	bool IUnrestrictedPermission.IsUnrestricted()
#else
	public bool IsUnrestricted()
#endif
			{
				return (state == PermissionState.Unrestricted);
			}

#if !ECMA_COMPAT

	// Get or set the flags on this permissions object.
	public ReflectionPermissionFlag Flags
			{
				get
				{
					return flags;
				}
				set
				{
					if((flags & ~(ReflectionPermissionFlag.AllFlags)) != 0)
					{
						throw new ArgumentException(_("Arg_ReflectionFlag"));
					}
				}
			}

#endif // !ECMA_COMPAT

}; // class ReflectionPermission

#endif // CONFIG_PERMISSIONS && CONFIG_REFLECTION

}; // namespace System.Security.Permissions
