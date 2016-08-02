/*
 * PrintingPermission.cs - Implementation of the
 *			"System.Drawing.Printing.PrintingPermission" class.
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

namespace System.Drawing.Printing
{

#if CONFIG_PERMISSIONS

using System.Security;
using System.Security.Permissions;

#if !ECMA_COMPAT
[Serializable]
#endif
public sealed class PrintingPermission : CodeAccessPermission
#if !ECMA_COMPAT
	, IUnrestrictedPermission
#endif
{
	// Internal state.
	private PermissionState state;
	private PrintingPermissionLevel level;

	// Constructor.
	public PrintingPermission(PermissionState state)
			{
				this.state = state;
				this.level = PrintingPermissionLevel.NoPrinting;
			}
	public PrintingPermission(PrintingPermissionLevel level)
			{
				this.state = PermissionState.None;
				this.level = level;
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
					throw new ArgumentException(S._("Arg_PermissionVersion"));
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
				value = esd.Attribute("Level");
				if(value != null)
				{
					level = (PrintingPermissionLevel)
						Enum.Parse(typeof(PrintingPermissionLevel), value);
				}
				else
				{
					level = PrintingPermissionLevel.NoPrinting;
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(PrintingPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(level != PrintingPermissionLevel.NoPrinting)
				{
					element.AddAttribute("Level", level.ToString());
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
				if(level != PrintingPermissionLevel.NoPrinting)
				{
					return new PrintingPermission(level);
				}
				else
				{
					return new PrintingPermission(state);
				}
			}
	public override IPermission Intersect(IPermission target)
			{
				PrintingPermissionLevel newLevel;
				if(target == null)
				{
					return target;
				}
				else if(!(target is PrintingPermission))
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(((PrintingPermission)target).IsUnrestricted())
				{
					if(IsUnrestricted())
					{
						return Copy();
					}
					else
					{
						newLevel = level;
					}
				}
				else if(IsUnrestricted())
				{
					newLevel = ((PrintingPermission)target).level;
				}
				else
				{
					newLevel = ((PrintingPermission)target).level;
					if(newLevel > level)
					{
						newLevel = level;
					}
				}
				if(newLevel == PrintingPermissionLevel.NoPrinting)
				{
					return null;
				}
				else
				{
					return new PrintingPermission(newLevel);
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (level == PrintingPermissionLevel.NoPrinting);
				}
				else if(!(target is PrintingPermission))
				{
					throw new ArgumentException
						(S._("Arg_PermissionMismatch"));
				}
				else if(((PrintingPermission)target).IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				else
				{
					return (level <= ((PrintingPermission)target).level);
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is PrintingPermission))
				{
					throw new ArgumentException
						(S._("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((PrintingPermission)target).IsUnrestricted())
				{
					return new PrintingPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					PrintingPermissionLevel newLevel;
					newLevel = ((PrintingPermission)target).level;
					if(newLevel < level)
					{
						newLevel = level;
					}
					return new PrintingPermission(newLevel);
				}
			}

	// Determine if this object has unrestricted permissions.
#if !ECMA_COMPAT
	public bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}
#else
	private bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}
#endif

	// Get or set the level on this permissions object.
	public PrintingPermissionLevel Level
			{
				get
				{
					return level;
				}
				set
				{
					level = value;
				}
			}

}; // class PrintingPermission

#endif // CONFIG_PERMISSIONS

}; // namespace System.Drawing.Printing
