/*
 * UriBuilder.cs - Implementation of "System.UriBuilder".
 *
 * Copyright (C) 2002 Free Software Foundation, Inc.
 *
 * Contributed by Stephen Compall <rushing@sigecom.net>
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

/*
Some implementation details.

This class and Uri have a very close relationship. This is because they rely
on the same parsing mechanisms. After mulling over whether to put the parsers
in either this class or Uri (for efficiency), I chose this class because it
needs the verifiers anyway for the setty properties.

Also note that there is a special package constructor in Uri for use by this
class. This is because, upon getting the this.Uri property, these methods have
already done the necessary verification.

One more thing...the whole "escaping" thing makes things about a million times
more complicated.

The reason this.path can be String.Empty, not per spec, is so that ToString() will
know if the user doesn't really want the path to be "/".
*/

namespace System
{

// StringBuilders are cool
using System.Text;

public class UriBuilder
{
	// State. After construction, these are never null!

	// Holds the scheme information. (search 0->:)
	private String scheme;

	// authority = userinfo+host+port
	// userinfo = username:password
	private String username;
	private String password;
	private String host;
	private int port;

	// technically optional, but they want a path :)
	// contains the slash
	private String path;

	// contains the ? mark
	private String query;

	// remember: this is not part of the uri
	// contains the #
	private String fragment;
	// also known as hash

	// Constructors.
	public UriBuilder()
	{
		this.fragment = "";
		// localhost, not loopback!
		this.host = "localhost";
		this.password = "";
		this.path = "";
		this.port = 80;
		this.scheme = "http";
		this.username = "";
	}

	public UriBuilder(String uri) : this(new Uri(uri))
	{
	}

	public UriBuilder(Uri uri)
	{
		scheme = uri.Scheme;
		host = uri.Host;
		port = uri.Port;
		path = uri.AbsolutePath;
		query = uri.Query;
		fragment = uri.Fragment;

		// set username&passwd from UserInfo
		String userinfo = uri.UserInfo;
		int passSplit = userinfo.IndexOf(':');
		if (passSplit >= 0) // there is a password
		{
			username = userinfo.Substring(0, passSplit);
			password = userinfo.Substring(passSplit + 1);
		}
		else
		{
			username = userinfo;
			password = "";
		}
	}

	public UriBuilder(String schemeName, String hostName) : this(schemeName,
		hostName, 80, "", "")
	{
		// no implementation needed :)
	}

	public UriBuilder(String scheme, String host, int portNumber) :
		this(scheme, host, portNumber, "", "")
	{
	}

	public UriBuilder(String scheme, String host, int port, String pathValue) :
		this(scheme, host, port, pathValue, "")
	{
	}

	public UriBuilder(String scheme, String host, int port, String path,
		String extraValue)
	{
		scheme = scheme.ToLower();
		if (!Uri.CheckSchemeName(scheme))
			throw new ArgumentException
				(S._("Arg_UriScheme"));
		this.scheme = scheme;
		this.host = host;
		if (port < 0 || (port < -1 && scheme == "file"))
			throw new ArgumentOutOfRangeException("port");
		this.port = port;
		this.path = path;
		if (extraValue[0] == '?')
			this.query = extraValue;
		else if (extraValue[0] == '#')
			this.fragment = extraValue;
		else if (extraValue != null && extraValue.Length != 0)
			throw new ArgumentException
				(S._("Exception_Argument"), "extraValue");
		this.username = "";
		this.password = "";
	}

	// Methods.

	public override bool Equals(Object rparam)
	{
		return this.Uri.Equals(rparam);
	}

	public override int GetHashCode()
	{
		String full = this.ToString();
		int hash = full.IndexOf('#');
		if (hash == -1)
			return full.GetHashCode();
		else
			return full.Substring(0, hash).GetHashCode();
	}

	public override String ToString()
	{
		StringBuilder maybeuri = new StringBuilder();
		maybeuri.Append(Scheme).Append("://");
		if (UserName.Length > 0)
		{
			maybeuri.Append(UserName);
			if (Password.Length > 0)
				maybeuri.Append(':').Append(Password);
			maybeuri.Append('@');
		}
		maybeuri.Append(Host);
		try
		{
			if (Port != Uri.DefaultPortForScheme(Scheme))
				throw new ArgumentException();
		}
		catch (ArgumentException)
		{
			if (Scheme != "file")
				maybeuri.Append(':').Append(Port);
		}

		String esc_path = Path;
		esc_path = Uri.EscapeStringInternal(esc_path,true,true);
		maybeuri.Append(esc_path);

		if (Query.Length > 0)
			maybeuri.Append(Query);
		else
			if (Fragment.Length > 0)
				maybeuri.Append(Fragment);

		return maybeuri.ToString();
	}

	// Properties.
	// note: property setties don't do validation, for some stupid reason
	// and to whoever thought of it: 'ni! ni ni ni! ni ni ni ni ni!'
	// all unescaped?

	// the fragment is not technically part of the URI. So when anything
	// says, "end of the URI", it means before this, if present. It is
	// separated from the URI by the # character; thus, this is reserved.
	public String Fragment
	{
		get
		{
			// gets with the #
			return this.fragment;

		}
		set
		{
			if (value == null)
				this.fragment = "";
			else
				this.fragment = String.Concat("#", value);
			this.query = "";
		}
	}

	public String Host
	{
		get
		{
			return this.host;
		}
		set
		{
			this.host = value;
		}
	}

	public String Password
	{
		get
		{
			return this.password;
		}
		set
		{
			if (value == null)
				this.password = "";
			else
				this.password = value;
		}
	}

	public String Path
	{
		get
		{
			return this.path;
		}
		set
		{
			if (value.Length == 0)
				this.path = "/";
			else if (value[0] == '/')
				this.path = value;
			else
				this.path = String.Concat("/", value);
		}
	}

	public int Port
	{
		get
		{
			return this.port;
		}
		set
		{
			if (value < 0 && !String.Equals(this.scheme, "file"))
				throw new ArgumentOutOfRangeException("value");
			else
				this.port = value;
		}
	}

	public String Query
	{
		get
		{
			// gets with the ?
			return (this.query != null) ? this.query : String.Empty;
		}
		set
		{
			if (value == null)
				this.query = "";
			else
				this.query = String.Concat("?", value);
			this.fragment = "";
		}
	}

	public String Scheme
	{
		get
		{
			return this.scheme;
		}
		set
		{
			if (value == null)
				this.scheme = "";
			else
			{
				int colon = value.IndexOf(':');
				if (colon <= -1)
					this.scheme = value.ToLower();
				else
					this.scheme = value.Substring(0, colon).
						ToLower();
			}
		}
	}

	public Uri Uri
	{
		get
		{
			return new Uri(this.ToString(), true);
		}
	}

	public String UserName
	{
		get
		{
			return this.username;
		}
		set
		{
			this.username = value;
		}
	}

}; // class UriBuilder

}; // namespace System
