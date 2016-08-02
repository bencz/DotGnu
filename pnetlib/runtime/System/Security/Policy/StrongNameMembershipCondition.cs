/*
 * StrongNameMembershipCondition.cs - Implementation of the
 *		"System.Security.Policy.StrongNameMembershipCondition" class.
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

using System.Text;
using System.Collections;
using System.Security.Permissions;

[Serializable]
public sealed class StrongNameMembershipCondition
	: IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
{
	// Internal state.
	private StrongNamePublicKeyBlob blob;
	private String name;
	private Version version;

	// Constructors.
	internal StrongNameMembershipCondition() {}
	public StrongNameMembershipCondition
				(StrongNamePublicKeyBlob blob, String name, Version version)
			{
				if(blob == null)
				{
					throw new ArgumentNullException("blob");
				}
				this.blob = blob;
				this.name = name;
				this.version = version;
			}

	// Get or set this object's properties.
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}
	public StrongNamePublicKeyBlob PublicKey
			{
				get
				{
					return blob;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					blob = value;
				}
			}
	public Version Version
			{
				get
				{
					return version;
				}
				set
				{
					version = value;
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
					StrongName sn = (e.Current as StrongName);
					if(sn != null)
					{
						if(sn.PublicKey.Equals(blob) &&
						   sn.Name == name &&
						   sn.Version.Equals(version))
						{
							return true;
						}
					}
				}
				return false;
			}
	public IMembershipCondition Copy()
			{
				return new StrongNameMembershipCondition(blob, name, version);
			}
	public override bool Equals(Object obj)
			{
				StrongNameMembershipCondition other;
				other = (obj as StrongNameMembershipCondition);
				if(other != null)
				{
					if(other.blob.Equals(blob) && other.name == name)
					{
						if(other.version == null)
						{
							return (version == null);
						}
						else
						{
							return other.version.Equals(version);
						}
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("StrongName - ");
				builder.Append(blob.ToString());
				if(name != null)
				{
					builder.Append(" name = ");
					builder.Append(name);
				}
				if(version != null)
				{
					builder.Append(" version = ");
					builder.Append(version.ToString());
				}
				return builder.ToString();
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
				String value = et.Attribute("PublicKey");
				if(value != null)
				{
					blob = new StrongNamePublicKeyBlob(value);
				}
				else
				{
					throw new ArgumentException(_("Arg_PublicKeyBlob"));
				}
				name = et.Attribute("Name");
				value = et.Attribute("AssemblyVersion");
				if(value != null)
				{
					version = new Version(value);
				}
				else
				{
					version = null;
				}
			}
	public SecurityElement ToXml(PolicyLevel level)
			{
				SecurityElement element;
				element = new SecurityElement("IMembershipCondition");
				element.AddAttribute
					("class",
					 SecurityElement.Escape
					 		(typeof(StrongNameMembershipCondition).
					 		 AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				element.AddAttribute("PublicKey", blob.ToString());
				if(name != null)
				{
					element.AddAttribute("Name", SecurityElement.Escape(name));
				}
				if(version != null)
				{
					element.AddAttribute("AssemblyVersion", version.ToString());
				}
				return element;
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				return blob.GetHashCode();
			}

}; // class StrongNameMembershipCondition

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
