/*
 * AspNetHostingPermission.cs - Implementation of the
 *		"System.Web.AspNetHostingPermission" class.
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

namespace System.Web
{

#if CONFIG_PERMISSIONS && !ECMA_COMPAT

using System.Security;
using System.Security.Permissions;

[Serializable]
public sealed class AspNetHostingPermission
	: CodeAccessPermission, IUnrestrictedPermission
{
	// Internal state.
	private AspNetHostingPermissionLevel level;

	// Constructors.
	public AspNetHostingPermission(AspNetHostingPermissionLevel level)
			{
				this.level = level;
			}
	public AspNetHostingPermission(PermissionState state)
			{
				if(state == PermissionState.Unrestricted)
				{
					this.level = AspNetHostingPermissionLevel.Unrestricted;
				}
				else
				{
					this.level = AspNetHostingPermissionLevel.None;
				}
			}

	// Get or set the permission level.
	public AspNetHostingPermissionLevel Level
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

	// Make a copy of this permission object.
	public override IPermission Copy()
			{
				return new AspNetHostingPermission(level);
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
					throw new ArgumentException
						(S._("Arg_PermissionVersion"));
				}
				value = securityElement.Attribute("Unrestricted");
				if(value != null && Boolean.Parse(value))
				{
					level = AspNetHostingPermissionLevel.Unrestricted;
				}
				else
				{
					value = securityElement.Attribute("Level");
					if(value != null)
					{
						level = (AspNetHostingPermissionLevel)
							Enum.Parse(typeof(AspNetHostingPermissionLevel),
							           value);
					}
					else
					{
						level = AspNetHostingPermissionLevel.None;
					}
				}
			}

	// Form the intersection of two security objects.
	public override IPermission Intersect(IPermission target)
			{
				AspNetHostingPermissionLevel newLevel;
				if(target == null)
				{
					return target;
				}
				else if(!(target is AspNetHostingPermission))
				{
					throw new ArgumentException
						(S._("Arg_PermissionMismatch"));
				}
				newLevel = ((AspNetHostingPermission)target).level;
				if(newLevel > level)
				{
					newLevel = level;
				}
				return new AspNetHostingPermission(newLevel);
			}

	// Determine if this permission object is a subset of another.
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (level == AspNetHostingPermissionLevel.None);
				}
				else if(!(target is AspNetHostingPermission))
				{
					throw new ArgumentException
						(S._("Arg_PermissionMismatch"));
				}
				return (level <= ((AspNetHostingPermission)target).level);
			}

	// Determine if this permission object is unrestricted.
	public bool IsUnrestricted()
			{
				return (level >= AspNetHostingPermissionLevel.Unrestricted);
			}

	// Convert this object into an XML element.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(AspNetHostingPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(level != AspNetHostingPermissionLevel.Unrestricted)
				{
					element.AddAttribute("Level", level.ToString());
				}
				else
				{
					element.AddAttribute("Unrestricted", "true");
				}
				return element;
			}

	// Create the union of two permission objects.
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is AspNetHostingPermission))
				{
					throw new ArgumentException
						(S._("Arg_PermissionMismatch"));
				}
				else if(level > ((AspNetHostingPermission)target).level)
				{
					return Copy();
				}
				else
				{
					return target.Copy();
				}
			}

}; // class AspNetHostingPermission

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Web
