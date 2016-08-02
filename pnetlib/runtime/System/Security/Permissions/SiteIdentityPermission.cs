/*
 * SiteIdentityPermission.cs - Implementation of the
 *		"System.Security.Permissions.SiteIdentityPermission" class.
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

public sealed class SiteIdentityPermission : CodeAccessPermission
{
	// Internal state.
	private String[] sites;

	// Constructor.
	public SiteIdentityPermission(PermissionState state)
			{
				if(state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				sites = null;
			}
	public SiteIdentityPermission(String site)
			{
				sites = SplitSite(site);
			}
	internal SiteIdentityPermission(String[] sites)
			{
				this.sites = sites;
			}

	// Split a site value into individual pieces.
	private static String[] SplitSite(String site)
			{
				if(site == null || site == String.Empty)
				{
					throw new ArgumentException(_("Arg_InvalidSite"));
				}
				String[] sites = site.Split('.');
				foreach(String s in sites)
				{
					if(s == null || s == String.Empty)
					{
						throw new ArgumentException(_("Arg_InvalidSite"));
					}
				}
				return sites;
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
				value = esd.Attribute("Site");
				if(value == null)
				{
					sites = null;
				}
				else
				{
					sites = SplitSite(value);
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(SiteIdentityPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(sites != null)
				{
					element.AddAttribute
						("Site", SecurityElement.Escape(Site));
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new SiteIdentityPermission(sites);
			}
	public override IPermission Intersect(IPermission target)
			{
				if(target == null)
				{
					return target;
				}
				else if(!(target is SiteIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else
				{
					return new SiteIdentityPermission
						(EnvironmentPermission.Intersect
							(sites, ((SiteIdentityPermission)target).sites));
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (sites == null || sites.Length == 0);
				}
				else if(!(target is SiteIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else
				{
					return EnvironmentPermission.IsSubsetOf
						(sites, ((SiteIdentityPermission)target).sites);
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is SiteIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else
				{
					return new SiteIdentityPermission
						(EnvironmentPermission.Union
							(sites, ((SiteIdentityPermission)target).sites,
							 false));
				}
			}

	// Get or set the site string.
	public String Site
			{
				get
				{
					if(sites == null)
					{
						return null;
					}
					else
					{
						return String.Join(".", sites);
					}
				}
				set
				{
					sites = SplitSite(value);
				}
			}

}; // class SiteIdentityPermission

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
