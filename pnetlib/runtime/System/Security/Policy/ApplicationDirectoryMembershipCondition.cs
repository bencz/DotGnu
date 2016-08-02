/*
 * ApplicationDirectoryMembershipCondition.cs - Implementation of the
 *		"System.Security.Policy.ApplicationDirectoryMembershipCondition" class.
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

[Serializable]
public sealed class ApplicationDirectoryMembershipCondition
	: IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
{
	// Constructor.
	public ApplicationDirectoryMembershipCondition() {}

	// Determine if we have an application directory match.
	private static bool Match(UrlParser url, String dir)
			{
				if(dir.EndsWith("/"))
				{
					dir = dir + "*";
				}
				else
				{
					dir = dir + "/*";
				}
				UrlParser parser = new UrlParser(dir);
				return parser.Matches(url);
			}

	// Implement the IMembership interface.
	public bool Check(Evidence evidence)
			{
				if(evidence == null)
				{
					return false;
				}
				IEnumerator e = evidence.GetHostEnumerator();
				IEnumerator e2;
				while(e.MoveNext())
				{
					ApplicationDirectory appDir =
						(e.Current as ApplicationDirectory);
					if(appDir != null)
					{
						e2 = evidence.GetHostEnumerator();
						while(e2.MoveNext())
						{
							Url url = (e2.Current as Url);
							if(url != null)
							{
								if(Match(url.parser, appDir.Directory))
								{
									return true;
								}
							}
						}
					}
				}
				return false;
			}
	public IMembershipCondition Copy()
			{
				return new ApplicationDirectoryMembershipCondition();
			}
	public override bool Equals(Object obj)
			{
				return (obj is ApplicationDirectoryMembershipCondition);
			}
	public override String ToString()
			{
				return _("Security_AppDirMembershipCondition");
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
					throw new ArgumentException
						(_("Security_PolicyName"));
				}
				if(et.Attribute("version") != "1")
				{
					throw new ArgumentException
						(_("Security_PolicyVersion"));
				}
			}
	public SecurityElement ToXml(PolicyLevel level)
			{
				SecurityElement element;
				element = new SecurityElement("IMembershipCondition");
				element.AddAttribute
					("class",
					 SecurityElement.Escape
					 	(typeof(ApplicationDirectoryMembershipCondition).
		 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				return element;
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				// All instances of this type are identical.
				return GetType().GetHashCode();
			}

}; // class ApplicationDirectoryMembershipCondition

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
