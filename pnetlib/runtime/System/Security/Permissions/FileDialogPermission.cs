/*
 * FileDialogPermission.cs - Implementation of the
 *		"System.Security.Permissions.FileDialogPermission" class.
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

using System;
using System.Security;

public sealed class FileDialogPermission
	: CodeAccessPermission, IUnrestrictedPermission
{
	// Internal state.
	private PermissionState state;
	private FileDialogPermissionAccess flags;

	// Constructor.
	public FileDialogPermission(PermissionState state)
			{
				this.state = state;
				this.flags = FileDialogPermissionAccess.OpenSave;
			}
	public FileDialogPermission(FileDialogPermissionAccess flags)
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
				value = esd.Attribute("Access");
				if(value != null)
				{
					flags = (FileDialogPermissionAccess)
						Enum.Parse(typeof(FileDialogPermissionAccess), value);
				}
				else
				{
					flags = FileDialogPermissionAccess.None;
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(FileDialogPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(flags != FileDialogPermissionAccess.None)
				{
					element.AddAttribute("Access", flags.ToString());
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
				if(state != PermissionState.Unrestricted)
				{
					return new FileDialogPermission(flags);
				}
				else
				{
					return new FileDialogPermission(state);
				}
			}
	public override IPermission Intersect(IPermission target)
			{
				FileDialogPermissionAccess newFlags;
				if(target == null)
				{
					return target;
				}
				else if(!(target is FileDialogPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((FileDialogPermission)target).IsUnrestricted())
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
					newFlags = ((FileDialogPermission)target).flags;
				}
				else
				{
					newFlags = ((FileDialogPermission)target).flags & flags;
				}
				if(newFlags == 0)
				{
					return null;
				}
				else
				{
					return new FileDialogPermission(newFlags);
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (flags == FileDialogPermissionAccess.None);
				}
				else if(!(target is FileDialogPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((FileDialogPermission)target).IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				else
				{
					return ((flags & ~(((FileDialogPermission)target).flags))
								== 0);
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is FileDialogPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((FileDialogPermission)target).IsUnrestricted())
				{
					return new FileDialogPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new FileDialogPermission
						(flags | ((FileDialogPermission)target).flags);
				}
			}

	// Determine if this object has unrestricted permissions.
	public bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}

	// Get or set the flags on this permissions object.
	public FileDialogPermissionAccess Access
			{
				get
				{
					return flags;
				}
				set
				{
					if((flags & ~(FileDialogPermissionAccess.OpenSave)) != 0)
					{
						throw new ArgumentException(_("Arg_FileDialogAccess"));
					}
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return ToXml().ToString();
			}

}; // class FileDialogPermission

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
