/*
 * AuthenticationManager.cs - Implementation of the
 *		"System.Net.AuthenticationManager" class.
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

namespace System.Net
{

using System.Collections;

public class AuthenticationManager
{
	// Internal state.
	private static ArrayList modules;

	static AuthenticationManager()
	{
		/* register the standard modules */
		Register(new BasicClient());	
	}

	// Cannot instantiate this class.
	private AuthenticationManager() {}

	// Get the module list.
	private static ArrayList ModuleList
			{
				get
				{
					if(modules == null)
					{
						modules = new ArrayList();
					}
					return modules;
				}
			}

	// Get a list of all authentication modules that are
	// registered with the authentication manager.
	public static IEnumerator RegisteredModules
			{
				get
				{
					lock(typeof(AuthenticationManager))
					{
						return ModuleList.GetEnumerator();
					}
				}
			}

	// Call each authentication module until one responds.
	public static Authorization Authenticate
				(String challenge, WebRequest request,
				 ICredentials credentials)
			{
				if(challenge == null)
				{
					throw new ArgumentNullException("challenge");
				}
				if(request == null)
				{
					throw new ArgumentNullException("request");
				}
				if(credentials == null)
				{
					throw new ArgumentNullException("credentials");
				}
				lock(typeof(AuthenticationManager))
				{
					Authorization auth;
					foreach(IAuthenticationModule module in ModuleList)
					{
						auth = module.Authenticate
							(challenge, request, credentials);
						if(auth != null)
						{
							return auth;
						}
					}
				}
				return null;
			}

	// Find an authentication module for a particular type.
	private static IAuthenticationModule FindModuleByType(String type)
			{
				foreach(IAuthenticationModule module in ModuleList)
				{
					if(module.AuthenticationType == type)
					{
						return module;
					}
				}
				return null;
			}

	// Pre-authenticate a request.
	public static Authorization PreAuthenticate
				(WebRequest request, ICredentials credentials)
			{
				if(request == null)
				{
					throw new ArgumentNullException("request");
				}
				if(credentials == null)
				{
					return null;
				}
				lock(typeof(AuthenticationManager))
				{
					Authorization auth;
					foreach(IAuthenticationModule module in ModuleList)
					{
						auth = module.PreAuthenticate
							(request, credentials);
						if(auth != null)
						{
							return auth;
						}
					}
				}
				return null;
			}

	// Register an authentication module with the authentication manager.
	public static void Register(IAuthenticationModule authenticationModule)
			{
				if(authenticationModule == null)
				{
					throw new ArgumentNullException("authenticationModule");
				}
				lock(typeof(AuthenticationManager))
				{
					IAuthenticationModule module;
					module = FindModuleByType
						(authenticationModule.AuthenticationType);
					if(module != null)
					{
						ModuleList.Remove(module);
					}
					ModuleList.Add(authenticationModule);
				}
			}

	// Unregister an authentication module from the authentication manager.
	public static void Unregister(IAuthenticationModule authenticationModule)
			{
				if(authenticationModule == null)
				{
					throw new ArgumentNullException("authenticationModule");
				}
				lock(typeof(AuthenticationManager))
				{
					if(!ModuleList.Contains(authenticationModule))
					{
						throw new InvalidOperationException
							(S._("Invalid_AuthModuleNotRegistered"));
					}
					ModuleList.Remove(authenticationModule);
				}
			}

	// Unregister an authentication module for a particular scheme.
	public static void Unregister(String authenticationScheme)
			{
				if(authenticationScheme == null)
				{
					throw new ArgumentNullException("authenticationScheme");
				}
				lock(typeof(AuthenticationManager))
				{
					IAuthenticationModule module;
					module = FindModuleByType(authenticationScheme);
					if(module != null)
					{
						ModuleList.Remove(module);
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_AuthModuleNotRegistered"));
					}
				}
			}

}; // class AuthenticationManager

}; // namespace System.Net
