/*
 * SiteMembershipCondition.cs - Implementation of the
 *		"System.Security.Policy.SiteMembershipCondition" class.
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
public sealed class SiteMembershipCondition
	: IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
{
	// Internal state.
	private String site;

	// Constructors.
	internal SiteMembershipCondition() {}
	public SiteMembershipCondition(String site)
			{
				if(site == null)
				{
					throw new ArgumentNullException("site");
				}
				this.site = site;
			}

	// Get or set this object's properties.
	public String Site
			{
				get
				{
					return site;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					site = value;
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
					Site s = (e.Current as Site);
					if(s != null)
					{
						if(UrlParser.HostMatches(site, s.Name))
						{
							return true;
						}
					}
				}
				return false;
			}
	public IMembershipCondition Copy()
			{
				return new SiteMembershipCondition(site);
			}
	public override bool Equals(Object obj)
			{
				SiteMembershipCondition other;
				other = (obj as SiteMembershipCondition);
				if(other != null)
				{
					return (other.site == site);
				}
				else
				{
					return false;
				}
			}
	public override String ToString()
			{
				return "Site - " + site;
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
				String value = et.Attribute("Site");
				if(value != null)
				{
					site = value;
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidSite"));
				}
			}
	public SecurityElement ToXml(PolicyLevel level)
			{
				SecurityElement element;
				element = new SecurityElement("IMembershipCondition");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(SiteMembershipCondition).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				element.AddAttribute("Site", SecurityElement.Escape(site));
				return element;
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				return site.GetHashCode();
			}

}; // class SiteMembershipCondition

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
