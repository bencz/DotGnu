/*
 * Url.cs - Implementation of the
 *		"System.Security.Policy.Url" class.
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
public sealed class Url
#if CONFIG_PERMISSIONS
	: IIdentityPermissionFactory
#endif
{
	// Internal state.
	internal UrlParser parser;

	// Constructor.
	public Url(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				parser = new UrlParser(name);
			}

	// Get this object's value.
	public String Value
			{
				get
				{
					return parser.URL;
				}
			}

	// Make a copy of this object.
	public Object Copy()
			{
				return new Url(parser.URL);
			}

#if CONFIG_PERMISSIONS

	// Implement the IIdentityPermissionFactory interface
	public IPermission CreateIdentityPermission(Evidence evidence)
			{
				return new UrlIdentityPermission(parser.URL);
			}

#endif

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				Url other = (obj as Url);
				if(other != null)
				{
					return (other.parser.URL == parser.URL);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return parser.URL.GetHashCode();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				SecurityElement element = new SecurityElement
					("System.Security.Policy.Url");
				SecurityElement child;
				element.AddAttribute("version", "1");
				child = new SecurityElement
					("Url", SecurityElement.Escape(parser.URL));
				element.AddChild(child);
				return element.ToString();
			}

}; // class Url

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
