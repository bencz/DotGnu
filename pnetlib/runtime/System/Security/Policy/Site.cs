/*
 * Site.cs - Implementation of the
 *		"System.Security.Policy.Site" class.
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

namespace System.Security.Policy
{

#if CONFIG_POLICY_OBJECTS

using System.Security.Permissions;

[Serializable]
public sealed class Site
#if CONFIG_PERMISSIONS
	: IIdentityPermissionFactory
#endif
{
	// Internal state.
	private String name;

	// Constructor.
	public Site(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				this.name = name;
			}

	// Get this object's value.
	public String Name
			{
				get
				{
					return name;
				}
			}

	// Make a copy of this object.
	public Object Copy()
			{
				return new Site(name);
			}

	// Create a new site from a URL.
	public static Site CreateFromUrl(String url)
			{
				UrlParser parser = new UrlParser(url);
				return new Site(parser.Host);
			}

#if CONFIG_PERMISSIONS

	// Implement the IIdentityPermissionFactory interface
	public IPermission CreateIdentityPermission(Evidence evidence)
			{
				return new SiteIdentityPermission(name);
			}

#endif

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				Site other = (obj as Site);
				if(other != null)
				{
					return (other.name == name);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return name.GetHashCode();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				SecurityElement element = new SecurityElement
					("System.Security.Policy.Site");
				SecurityElement child;
				element.AddAttribute("version", "1");
				child = new SecurityElement
					("Name", SecurityElement.Escape(name));
				element.AddChild(child);
				return element.ToString();
			}

}; // class Site

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
