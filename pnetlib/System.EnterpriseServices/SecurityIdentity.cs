/*
 * SecurityIdentity.cs - Implementation of the
 *			"System.EnterpriseServices.SecurityIdentity" class.
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

namespace System.EnterpriseServices
{

public sealed class SecurityIdentity
{
	// Internal state.
	private String accountName;
	private AuthenticationOption authenticationLevel;
	private int authenticationService;
	private ImpersonationLevelOption impersonationLevel;

	// Constructor.
	internal SecurityIdentity(String accountName,
							  AuthenticationOption authenticationLevel,
							  int authenticationService,
							  ImpersonationLevelOption impersonationLevel)
			{
				this.accountName = accountName;
				this.authenticationLevel = authenticationLevel;
				this.authenticationService = authenticationService;
				this.impersonationLevel = impersonationLevel;
			}
	internal SecurityIdentity()
			{
			#if !ECMA_COMPAT
				this.accountName = Environment.UserName;
			#else
				this.accountName = "nobody";
			#endif
			}

	// Get this object's properties.
	public String AccountName
			{
				get
				{
					return accountName;
				}
			}
	public AuthenticationOption AuthenticationLevel
			{
				get
				{
					return authenticationLevel;
				}
			}
	public int AuthenticationService
			{
				get
				{
					return authenticationService;
				}
			}
	public ImpersonationLevelOption ImpersonationLevel
			{
				get
				{
					return impersonationLevel;
				}
			}

}; // class SecurityIdentity

}; // namespace System.EnterpriseServices
