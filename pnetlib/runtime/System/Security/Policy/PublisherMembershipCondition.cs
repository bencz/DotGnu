/*
 * PublisherMembershipCondition.cs - Implementation of the
 *		"System.Security.Policy.PublisherMembershipCondition" class.
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

#if CONFIG_X509_CERTIFICATES && CONFIG_POLICY_OBJECTS

using System.Collections;
using System.Security.Permissions;
using System.Security.Cryptography.X509Certificates;

[Serializable]
public sealed class PublisherMembershipCondition
	: IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
{
	// Internal state.
	private X509Certificate certificate;

	// Constructors.
	internal PublisherMembershipCondition() {}
	public PublisherMembershipCondition(X509Certificate certificate)
			{
				if(certificate == null)
				{
					throw new ArgumentNullException("certificate");
				}
				this.certificate = certificate;
			}

	// Get or set this object's properties.
	public X509Certificate Certificate
			{
				get
				{
					return certificate;
				}
				set
				{
					if(certificate == null)
					{
						throw new ArgumentNullException("certificate");
					}
					certificate = value;
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
					Publisher publisher = (e.Current as Publisher);
					if(publisher != null &&
					   publisher.Certificate.Equals(certificate))
					{
						return true;
					}
				}
				return false;
			}
	public IMembershipCondition Copy()
			{
				return new PublisherMembershipCondition(certificate);
			}
	public override bool Equals(Object obj)
			{
				PublisherMembershipCondition other;
				other = (obj as PublisherMembershipCondition);
				if(other != null)
				{
					return (other.certificate.Equals(certificate));
				}
				else
				{
					return false;
				}
			}
	public override String ToString()
			{
				return "Publisher - " + certificate.GetRawCertDataString();
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
				String value = et.Attribute("X509Certificate");
				certificate = new X509Certificate
					(StrongNamePublicKeyBlob.FromHex(value));
			}
	public SecurityElement ToXml(PolicyLevel level)
			{
				SecurityElement element;
				element = new SecurityElement("IMembershipCondition");
				element.AddAttribute
					("class",
					 SecurityElement.Escape
					 		(typeof(PublisherMembershipCondition).
					 		 AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				element.AddAttribute
					("X509Certificate", certificate.GetRawCertDataString());
				return element;
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				return certificate.GetHashCode();
			}

}; // class PublisherMembershipCondition

#endif // CONFIG_X509_CERTIFICATES && CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
