/*
 * HashMembershipCondition.cs - Implementation of the
 *		"System.Security.Policy.HashMembershipCondition" class.
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

#if CONFIG_CRYPTO && CONFIG_POLICY_OBJECTS

using System.Text;
using System.Collections;
using System.Security.Permissions;
using System.Security.Cryptography;

[Serializable]
public sealed class HashMembershipCondition
	: IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
{
	// Internal state.
	private HashAlgorithm hashAlg;
	private byte[] value;

	// Constructors.
	internal HashMembershipCondition() {}
	public HashMembershipCondition(HashAlgorithm hashAlg, byte[] value)
			{
				if(hashAlg == null)
				{
					throw new ArgumentNullException("hashAlg");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.hashAlg = hashAlg;
				this.value = value;
			}

	// Get or set this object's properties.
	public HashAlgorithm HashAlgorithm
			{
				get
				{
					return hashAlg;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					hashAlg = value;
				}
			}
	public byte[] HashValue
			{
				get
				{
					return value;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					this.value = value;
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
					Hash hash = (e.Current as Hash);
					if(hash != null)
					{
						byte[] computed = hash.GenerateHash(hashAlg);
						if(computed == null || value.Length != computed.Length)
						{
							continue;
						}
						int posn;
						for(posn = 0; posn < computed.Length; ++posn)
						{
							if(computed[posn] != value[posn])
							{
								break;
							}
						}
						if(posn >= computed.Length)
						{
							return true;
						}
					}
				}
				return false;
			}
	public IMembershipCondition Copy()
			{
				return new HashMembershipCondition(hashAlg, value);
			}
	public override bool Equals(Object obj)
			{
				HashMembershipCondition other;
				other = (obj as HashMembershipCondition);
				if(other != null)
				{
					if(other.hashAlg.GetType() != hashAlg.GetType())
					{
						return false;
					}
					if(other.value.Length != value.Length)
					{
						return false;
					}
					int posn;
					for(posn = 0; posn < value.Length; ++posn)
					{
						if(other.value[posn] != value[posn])
						{
							return false;
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("Hash - ");
				builder.Append(hashAlg.GetType().AssemblyQualifiedName);
				builder.Append(" = ");
				builder.Append(StrongNamePublicKeyBlob.ToHex(value));
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
				String val = et.Attribute("HashValue");
				value = StrongNamePublicKeyBlob.FromHex(val);
				val = et.Attribute("HashAlgorithm");
				hashAlg = HashAlgorithm.Create(val);
			}
	public SecurityElement ToXml(PolicyLevel level)
			{
				SecurityElement element;
				element = new SecurityElement("IMembershipCondition");
				element.AddAttribute
					("class",
					 SecurityElement.Escape
					 		(typeof(HashMembershipCondition).
					 		 AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				element.AddAttribute
					("HashValue", StrongNamePublicKeyBlob.ToHex(value));
				element.AddAttribute
					("HashAlgorithm",
					 SecurityElement.Escape(hashAlg.GetType().FullName));
				return element;
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				int hash = 0;
				int posn;
				for(posn = 0; posn < value.Length; ++posn)
				{
					hash = (hash << 5) + hash + value[posn];
				}
				return hash;
			}

}; // class HashMembershipCondition

#endif // CONFIG_CRYPTO && CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
