/*
 * BasicClient.cs - Implementation of the
 *			"System.Net.BasicClient" class.
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

using System;
using System.Text;

internal class BasicClient : IAuthenticationModule
{
	// Authenticate a challenge from the server.
	public Authorization Authenticate
		(String challenge, WebRequest request, ICredentials credentials)
	{
		if(credentials==null || challenge==null || request==null
			|| (!challenge.ToLower().StartsWith("basic")))
		{
			return null;
		}
		return AuthenticateInternal(request, credentials);
	}

	// Pre-authenticate a request.
	public Authorization PreAuthenticate
		(WebRequest request, ICredentials credentials)
	{
		if(request==null || credentials==null)
		{
			return null;
		}
		return AuthenticateInternal(request, credentials);
	}

	private Authorization AuthenticateInternal(WebRequest request, 
			ICredentials credentials)
	{
		String user,password,domain;
		NetworkCredential netcredentials=credentials.GetCredential(
					request.RequestUri, "Basic");
		user=netcredentials.UserName;
		password=netcredentials.Password;
		domain=netcredentials.Domain;
		String response=((domain==null || domain=="") ? "" : 
					(domain + "\\"))
					+ user + ":" + password;
		byte[] buf=Encoding.Default.GetBytes(response);
		
		return new Authorization("Basic "+ToBase64String(buf));
	}


	// Get the authentiation type supported by this module.
	public String AuthenticationType
	{
		get
		{
			return "Basic";
		}
	}

	// Determine if this module supports pre-authentication.
	public bool CanPreAuthenticate 
	{
		get
		{
			return true;
		}
	}

	// Characters to use to encode 6-bit values in base64.
	private const String base64Chars =
		"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

	// Convert an array of bytes into a base64 string.
	private static String ToBase64String(byte[] inArray)
			{
				if(inArray == null)
				{
					throw new ArgumentNullException("inArray");
				}
				return ToBase64String(inArray, 0, inArray.Length);
			}
	private static String ToBase64String(byte[] inArray, int offset, int length)
			{
				// Validate the parameters.
				if(inArray == null)
				{
					throw new ArgumentNullException("inArray");
				}
				if(offset < 0 || offset > inArray.Length)
				{
					throw new ArgumentOutOfRangeException
						("offset", S._("ArgRange_Array"));
				}
				if(length < 0 || length > (inArray.Length - offset))
				{
					throw new ArgumentOutOfRangeException
						("length", S._("ArgRange_Array"));
				}

				// Convert the bytes.
				StringBuilder builder =
					new StringBuilder
						((int)(((((long)length) + 2L) * 4L) / 3L));
				int bits = 0;
				int numBits = 0;
				String base64 = base64Chars;
				int size = length;
				while(size > 0)
				{
					bits = (bits << 8) + inArray[offset++];
					numBits += 8;
					--size;
					while(numBits >= 6)
					{
						numBits -= 6;
						builder.Append(base64[bits >> numBits]);
						bits &= ((1 << numBits) - 1);
					}
				}
				length %= 3;
				if(length == 1)
				{
					builder.Append(base64[bits << (6 - numBits)]);
					builder.Append('=');
					builder.Append('=');
				}
				else if(length == 2)
				{
					builder.Append(base64[bits << (6 - numBits)]);
					builder.Append('=');
				}

				// Finished.
				return builder.ToString();
			}

}; // class BasicClient

}; // namespace System.Net
