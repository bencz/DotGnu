/*
 * NetworkCredential.cs - Implementation of the
 *			"System.Net.NetworkCredential" class.
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

public class NetworkCredential : ICredentials
{
	// Internal state.
	private String userName;
	private String password;
	private String domain;

	// Constructors.
	public NetworkCredential() {}
	public NetworkCredential(String userName, String password)
			{
				this.userName = userName;
				this.password = password;
			}
	public NetworkCredential(String userName, String password, String domain)
			{
				this.userName = userName;
				this.password = password;
				this.domain = domain;
			}

	// Get or set this object's properties.
	public String Domain
			{
				get
				{
					return domain;
				}
				set
				{
					domain = value;
				}
			}
	public String Password
			{
				get
				{
					return password;
				}
				set
				{
					password = value;
				}
			}
	public String UserName
			{
				get
				{
					return userName;
				}
				set
				{
					userName = value;
				}
			}

	// Get credential information for a URI and authentication type.
	public NetworkCredential GetCredential(Uri uri, String authType)
			{
				return this;
			}
	
}; // class NetworkCredential

}; // namespace System.Net
