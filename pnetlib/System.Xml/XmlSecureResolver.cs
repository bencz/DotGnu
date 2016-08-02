/*
 * XmlSecureResolver.cs - Implementation of the
 *						 "System.Xml.XmlSecureResolver" class.
 *
 * Copyright (C) 2010 Southern Storm Software, Pty Ltd.
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
 
namespace System.Xml
{

#if !ECMA_COMPAT && CONFIG_PERMISSIONS && CONFIG_POLICY_OBJECTS

	using System.Net;
	using System.Security;
	using System.Security.Permissions;
	using System.Security.Policy;

	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class XmlSecureResolver : XmlResolver
	{

		private XmlResolver resolver;
		private PermissionSet permissionSet;

		// Constructors
		public XmlSecureResolver(XmlResolver resolver,
								 PermissionSet permissionSet)
		{
			this.resolver = resolver;
			this.permissionSet = permissionSet;
		}

		public XmlSecureResolver(XmlResolver resolver,
								 Evidence evidence)
		{
			this.resolver = resolver;
			if(SecurityManager.SecurityEnabled)
			{
				this.permissionSet = SecurityManager.ResolvePolicy(evidence);
			}
		}

		public XmlSecureResolver(XmlResolver resolver,
								 string securityUrl)
		{
			this.resolver = resolver;
			if(SecurityManager.SecurityEnabled)
			{
				this.permissionSet =
					SecurityManager.ResolvePolicy
						(CreateEvidenceForUrl(securityUrl));
			}
		}

		// static members
		public static Evidence CreateEvidenceForUrl(string securityUrl)
		{
			Evidence evidence = new Evidence();

			if((securityUrl != null) && (securityUrl.Length > 0))
			{
				try
				{
					Url url = new Url(securityUrl);
					evidence.AddHost(url);
				}
				catch(ArgumentException)
				{
				}

				try
				{
					Zone zone = Zone.CreateFromUrl(securityUrl);
					evidence.AddHost(zone);
				}
				catch (ArgumentException)
				{
				}

				try
				{
					Site site = Site.CreateFromUrl(securityUrl);
					evidence.AddHost(site);
				}
				catch (ArgumentException)
				{
				}
			}

			return evidence;
		}

		// instance members
		public override object GetEntity(Uri absoluteUri, string role,
										 Type ofObjectToReturn)
		{
			if(SecurityManager.SecurityEnabled)
			{
				// in case the security manager was switched after the constructor was called
				if(permissionSet == null)
				{
					throw new SecurityException(
						S._("Security Manager wasn't active when instance was created."));
				}
				permissionSet.PermitOnly();
			}
			return resolver.GetEntity(absoluteUri, role, ofObjectToReturn);
		}

		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			return resolver.ResolveUri(baseUri, relativeUri);
		}

		public override ICredentials Credentials
		{
			set
			{
				resolver.Credentials = value;
			}
		}

	}; // class XmlSecureResolver

#endif // !ECMA_COMPAT && CONFIG_PERMISSIONS && CONFIG_POLICY_OBJECTS

}; // namespace System.Xml
