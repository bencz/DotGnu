/*
 * SessionProviderFactory.cs - Implementation of the
 *		"DotGNU.SSL.SessionProviderFactory" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace DotGNU.SSL
{

using System;

/// <summary>
/// <para>The <see cref="T:DotGNU.SSL.SessionProviderFactory"/> class is used
/// to get an initial <see cref="T:DotGNU.SSL.ISecureSessionProvider"/>
/// instance, with which secure sessions can be established.</para>
/// </summary>
public sealed class SessionProviderFactory
{
	// This class cannot be instantiated.
	private SessionProviderFactory() {}

	/// <summary>
	/// <para>Get a secure session provider with a specific name.</para>
	/// </summary>
	///
	/// <param name="name">
	/// <para>The name of the provider to use, or <see langword="null"/>
	/// to get the default provider.  Examples of a name might be
	/// <c>"GNUTLS"</c> or <c>"OpenSSL"</c>.</para>
	/// </param>
	///
	/// <returns>
	/// <para>The selected provider instance.</para>
	/// </returns>
	///
	/// <exception cref="T:System.NotSupportedException">
	/// <para>The specified provider is not supported, or there are
	/// no secure session providers on the system.</para>
	/// </exception>
	public static ISecureSessionProvider GetProvider(String name)
			{
			#if CONFIG_RUNTIME_INFRA
				if(name == "GNUTLS")
				{
					// The caller explicitly asked for "GNUTLS".
					return new GNUTLS();
				}
				else if(name == "OpenSSL")
				{
					// The caller explicitly asked for "OpenSSL".
					return new OpenSSL();
				}
				else if(name == null)
				{
					// Try GNUTLS first, and then fall back to OpenSSL.
					try
					{
						return new GNUTLS();
					}
					catch(NotSupportedException)
					{
						return new OpenSSL();
					}
				}
			#endif // CONFIG_RUNTIME_INFRA
				throw new NotSupportedException();
			}

	/// <summary>
	/// <para>Get the default secure session provider.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>The default provider instance.</para>
	/// </returns>
	///
	/// <remarks>
	/// <para>This method is equivalent to <c>GetProvider(null)</c>.</para>
	/// </remarks>
	///
	/// <exception cref="T:System.NotSupportedException">
	/// <para>There are no secure session providers on the system.</para>
	/// </exception>
	public static ISecureSessionProvider GetProvider()
			{
				return GetProvider(null);
			}

	/// <summary>
	/// <para>Get the names of all session providers on the system.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>An array of session provider names.</para>
	/// </returns>
	public static String[] GetProviders()
			{
			#if CONFIG_RUNTIME_INFRA
				// Probe the handlers to see which ones are available.
				bool haveGNUTLS, haveOpenSSL;
				ISecureSessionProvider provider;
				try
				{
					provider = new GNUTLS();
					haveGNUTLS = true;
				}
				catch(NotSupportedException)
				{
					haveGNUTLS = false;
				}
				try
				{
					provider = new OpenSSL();
					haveOpenSSL = true;
				}
				catch(NotSupportedException)
				{
					haveOpenSSL = false;
				}
				if(haveGNUTLS && haveOpenSSL)
				{
					return new String[] {"GNUTLS", "OpenSSL"};
				}
				else if(haveGNUTLS)
				{
					return new String[] {"GNUTLS"};
				}
				else if(haveOpenSSL)
				{
					return new String[] {"OpenSSL"};
				}
			#endif
				return new String [0];
			}

}; // class SessionProviderFactory

}; // namespace DotGNU.SSL
