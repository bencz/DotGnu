/*
 * UrlIdentityPermission.cs - Implementation of the
 *		"System.Security.Permissions.UrlIdentityPermission" class.
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

public sealed class UrlIdentityPermission : CodeAccessPermission
{
	// Internal state.
	private String url;

	// Constructor.
	public UrlIdentityPermission(PermissionState state)
			{
				if(state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				url = null;
			}
	public UrlIdentityPermission(String site)
			{
				if(site == null)
				{
					throw new ArgumentNullException("site");
				}
				url = site;
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
				url = esd.Attribute("Url");
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(UrlIdentityPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(url != null)
				{
					element.AddAttribute
						("Url", SecurityElement.Escape(url));
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new UrlIdentityPermission(url);
			}
	public override IPermission Intersect(IPermission target)
			{
				if(target == null)
				{
					return target;
				}
				else if(!(target is UrlIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(url == ((UrlIdentityPermission)target).url)
				{
					return new UrlIdentityPermission(url);
				}
				else
				{
					return new UrlIdentityPermission(PermissionState.None);
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (url == null || url.Length == 0);
				}
				else if(!(target is UrlIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(url == null ||
						url == ((UrlIdentityPermission)target).url)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is UrlIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(url != null)
				{
					// Return the first one, because union'ing two
					// non-empty url identities doesn't make sense.
					return Copy();
				}
				else
				{
					return target.Copy();
				}
			}

	// Get or set the url string.
	public String Url
			{
				get
				{
					return url;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					url = value;
				}
			}

}; // class UrlIdentityPermission

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
