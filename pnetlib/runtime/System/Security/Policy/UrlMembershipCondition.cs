/*
 * UrlMembershipCondition.cs - Implementation of the
 *		"System.Security.Policy.UrlMembershipCondition" class.
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

using System.Collections;
using System.Security.Permissions;

[Serializable]
public sealed class UrlMembershipCondition
	: IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
{
	// Internal state.
	private UrlParser parser;

	// Constructors.
	internal UrlMembershipCondition() {}
	public UrlMembershipCondition(String url)
			{
				if(url == null)
				{
					throw new ArgumentNullException("url");
				}
				parser = new UrlParser(url);
			}

	// Get or set this object's properties.
	public String Url
			{
				get
				{
					return parser.URL;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					parser = new UrlParser(value);
				}
			}

	// Implement the IMembership interface.
	public bool Check(Evidence evidence)
			{
				if(evidence == null)
				{
					return false;
				}
				IEnumerator e = evidence.GetHostEnumerator();
				while(e.MoveNext())
				{
					Url url = (e.Current as Url);
					if(url != null)
					{
						if(parser.Matches(url.parser))
						{
							return true;
						}
					}
				}
				return false;
			}
	public IMembershipCondition Copy()
			{
				return new UrlMembershipCondition(parser.URL);
			}
	public override bool Equals(Object obj)
			{
				UrlMembershipCondition other;
				other = (obj as UrlMembershipCondition);
				if(other != null)
				{
					return (other.parser.URL == parser.URL);
				}
				else
				{
					return false;
				}
			}
	public override String ToString()
			{
				return "Url - " + parser.URL;
			}

	// Implement the ISecurityEncodable interface.
	public void FromXml(SecurityElement et)
			{
				FromXml(et, null);
			}
	public SecurityElement ToXml()
			{
				return ToXml(null);
			}

	// Implement the ISecurityPolicyEncodable interface.
	public void FromXml(SecurityElement et, PolicyLevel level)
			{
				if(et == null)
				{
					throw new ArgumentNullException("et");
				}
				if(et.Tag != "IMembershipCondition")
				{
					throw new ArgumentException(_("Security_PolicyName"));
				}
				if(et.Attribute("version") != "1")
				{
					throw new ArgumentException(_("Security_PolicyVersion"));
				}
				String value = et.Attribute("Url");
				if(value != null)
				{
					parser = new UrlParser(value);
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidUrl"));
				}
			}
	public SecurityElement ToXml(PolicyLevel level)
			{
				SecurityElement element;
				element = new SecurityElement("IMembershipCondition");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(UrlMembershipCondition).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				element.AddAttribute("Url", SecurityElement.Escape(parser.URL));
				return element;
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				return parser.URL.GetHashCode();
			}

}; // class UrlMembershipCondition

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
