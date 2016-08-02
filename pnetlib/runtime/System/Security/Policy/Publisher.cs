/*
 * Publisher.cs - Implementation of the
 *		"System.Security.Policy.Publisher" class.
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

using System.Security.Permissions;
using System.Security.Cryptography.X509Certificates;

[Serializable]
public sealed class Publisher
#if CONFIG_PERMISSIONS
	: IIdentityPermissionFactory
#endif
{
	// Internal state.
	private X509Certificate cert;

	// Constructor.
	public Publisher(X509Certificate cert)
			{
				if(cert == null)
				{
					throw new ArgumentNullException("cert");
				}
				this.cert = cert;
			}

	// Get this object's value.
	public X509Certificate Certificate
			{
				get
				{
					return cert;
				}
			}

	// Make a copy of this object.
	public Object Copy()
			{
				return new Publisher(cert);
			}

#if CONFIG_PERMISSIONS

	// Implement the IIdentityPermissionFactory interface
	public IPermission CreateIdentityPermission(Evidence evidence)
			{
				return new PublisherIdentityPermission(cert);
			}

#endif

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				Publisher other = (obj as Publisher);
				if(other != null)
				{
					return (other.cert.Equals(cert));
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return cert.GetHashCode();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				SecurityElement element = new SecurityElement
					("System.Security.Policy.Publisher");
				SecurityElement child;
				element.AddAttribute("version", "1");
				child = new SecurityElement
					("X509v3Certificate", cert.GetRawCertDataString());
				element.AddChild(child);
				return element.ToString();
			}

}; // class Publisher

#endif // CONFIG_X509_CERTIFICATES && CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
