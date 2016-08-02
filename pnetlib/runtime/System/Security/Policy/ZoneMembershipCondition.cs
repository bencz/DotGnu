/*
 * ZoneMembershipCondition.cs - Implementation of the
 *		"System.Security.Policy.ZoneMembershipCondition" class.
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
public sealed class ZoneMembershipCondition
	: IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
{
	// Internal state.
	private SecurityZone zone;

	// Constructors.
	internal ZoneMembershipCondition() {}
	public ZoneMembershipCondition(SecurityZone zone)
			{
				if(zone < SecurityZone.MyComputer ||
				   zone > SecurityZone.Untrusted)
				{
					throw new ArgumentException(_("Arg_SecurityZone"));
				}
				this.zone = zone;
			}

	// Get or set this object's properties.
	public SecurityZone SecurityZone
			{
				get
				{
					return zone;
				}
				set
				{
					if(zone < SecurityZone.MyComputer ||
					   zone > SecurityZone.Untrusted)
					{
						throw new ArgumentException(_("Arg_SecurityZone"));
					}
					zone = value;
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
					Zone z = (e.Current as Zone);
					if(z != null && z.SecurityZone == zone)
					{
						return true;
					}
				}
				return false;
			}
	public IMembershipCondition Copy()
			{
				return new ZoneMembershipCondition(zone);
			}
	public override bool Equals(Object obj)
			{
				ZoneMembershipCondition other;
				other = (obj as ZoneMembershipCondition);
				if(other != null)
				{
					return (other.zone == zone);
				}
				else
				{
					return false;
				}
			}
	public override String ToString()
			{
				return "Zone - " + zone.ToString();
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
				String value = et.Attribute("Zone");
				if(value != null)
				{
					zone = (SecurityZone)
						Enum.Parse(typeof(SecurityZone), value);
				}
				else
				{
					throw new ArgumentException(_("Arg_SecurityZone"));
				}
			}
	public SecurityElement ToXml(PolicyLevel level)
			{
				SecurityElement element;
				element = new SecurityElement("IMembershipCondition");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(ZoneMembershipCondition).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				element.AddAttribute("Zone", zone.ToString());
				return element;
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				return ((int)zone);
			}

}; // class ZoneMembershipCondition

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
