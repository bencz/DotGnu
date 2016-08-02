/*
 * PublisherIdentityPermissionAttribute.cs - Implementation of the
 *  "System.Security.Permissions.PublisherIdentityPermissionAttribute" class.
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

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public sealed class PublisherIdentityPermissionAttribute
	: CodeAccessSecurityAttribute
{
	// Internal state.
	private String certFile;
	private String signedFile;
	private String cert;

	// Constructors.
	public PublisherIdentityPermissionAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the certificate file value.
	public String CertFile
			{
				get
				{
					return certFile;
				}
				set
				{
					certFile = value;
				}
			}

	// Get or set the signed certificate file value.
	public String SignedFile
			{
				get
				{
					return signedFile;
				}
				set
				{
					signedFile = value;
				}
			}

	// Get or set the X509 certificate value.
	public String X509Certificate
			{
				get
				{
					return cert;
				}
				set
				{
					cert = value;
				}
			}

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				X509Certificate certificate;
				if(Unrestricted)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				else if(certFile == null && signedFile == null && cert == null)
				{
					return new PublisherIdentityPermission
						(PermissionState.None);
				}
				else if(cert != null)
				{
					certificate = new X509Certificate
						(StrongNamePublicKeyBlob.FromHex(cert));
				}
				else if(certFile != null)
				{
					certificate =
						System.Security.Cryptography.X509Certificates
							.X509Certificate.CreateFromCertFile(certFile);
				}
				else
				{
					certificate = 
						System.Security.Cryptography.X509Certificates
							.X509Certificate.CreateFromSignedFile(signedFile);
				}
				return new PublisherIdentityPermission(certificate);
			}

}; // class PublisherIdentityPermissionAttribute

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && CONFIG_X509_CERTIFICATES

}; // namespace System.Security.Permissions
