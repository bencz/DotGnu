/*
 * PublisherIdentityPermission.cs - Implementation of the
 *		"System.Security.Permissions.PublisherIdentityPermission" class.
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

namespace System.Security.Permissions
{

#if CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && CONFIG_X509_CERTIFICATES

using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;

public sealed class PublisherIdentityPermission : CodeAccessPermission
{
	// Internal state.
	private X509Certificate certificate;

	// Constructor.
	public PublisherIdentityPermission(PermissionState state)
			{
				if(state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				certificate = null;
			}
	public PublisherIdentityPermission(X509Certificate certificate)
			{
				if(certificate == null)
				{
					throw new ArgumentNullException("certificate");
				}
				this.certificate = certificate;
			}

	// Convert an XML value into a permissions value.
	public override void FromXml(SecurityElement esd)
			{
				if(esd == null)
				{
					throw new ArgumentNullException("esd");
				}
				if(esd.Attribute("version") != "1")
				{
					throw new ArgumentException(_("Arg_PermissionVersion"));
				}
				String value = esd.Attribute("X509v3Certificate");
				if(value != null)
				{
					certificate = new X509Certificate
						(StrongNamePublicKeyBlob.FromHex(value));
				}
				else
				{
					certificate = null;
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape
					 	(typeof(PublisherIdentityPermission).
								AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(certificate != null)
				{
					element.AddAttribute
						("X509v3Certificate",
						 certificate.GetRawCertDataString());
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				if(certificate == null)
				{
					return new PublisherIdentityPermission
						(PermissionState.None);
				}
				else
				{
					return new PublisherIdentityPermission(certificate);
				}
			}
	public override IPermission Intersect(IPermission target)
			{
				if(target == null)
				{
					return target;
				}
				else if(!(target is PublisherIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsSubsetOf(target))
				{
					return Copy();
				}
				else if(target.IsSubsetOf(this))
				{
					return target.Copy();
				}
				else
				{
					return null;
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (certificate == null);
				}
				else if(!(target is PublisherIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(certificate != null &&
						!certificate.Equals
							(((PublisherIdentityPermission)target).certificate))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is PublisherIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsSubsetOf(target))
				{
					return target.Copy();
				}
				else if(target.IsSubsetOf(this))
				{
					return Copy();
				}
				else
				{
					return null;
				}
			}

	// Get or set the certificate.
	public X509Certificate Certificate
			{
				get
				{
					return certificate;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					certificate = value;
				}
			}

}; // class PublisherIdentityPermission

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && CONFIG_X509_CERTIFICATES

}; // namespace System.Security.Permissions
