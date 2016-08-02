/*
 * Cookie.cs - Implementation of the "System.Net.Cookie" class.
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

#if !ECMA_COMPAT

using System.Globalization;
using System.Text;

[Serializable]
public sealed class Cookie
{
	// Internal state.
	private String name;
	private String value;
	private String path;
	private String domain;
	private String comment;
	private Uri commentUri;
	private bool discard;
	private bool expired;
	private bool secure;
	private bool domainImplicit;
	private bool portImplicit;
	private DateTime expires;
	private String port;
	private DateTime timeStamp;
	private int version;

	// Constructors.
	public Cookie()
			: this(null, null, null, null) {}
	public Cookie(String name, String value)
			: this(name, value, null, null) {}
	public Cookie(String name, String value, String path)
			{
				if(name != null)
				{
					this.name = name;
				}
				else
				{
					this.name = String.Empty;
				}
				if(value != null)
				{
					this.value = value;
				}
				else
				{
					this.value = String.Empty;
				}
				if(path != null)
				{
					this.path = path;
				}
				else
				{
					this.path = String.Empty;
				}
				this.comment = String.Empty;
				this.port = String.Empty;
				this.domainImplicit = true;
				this.portImplicit = true;
			}
	public Cookie(String name, String value, String path, String domain)
			: this(name, value, path)
			{
				Domain = domain;
			}

	// Get or set this object's properties.
	public String Comment
			{
				get
				{
					return comment;
				}
				set
				{
					if(value != null)
					{
						comment = value;
					}
					else
					{
						comment = String.Empty;
					}
				}
			}
	public Uri CommentUri
			{
				get
				{
					return commentUri;
				}
				set
				{
					commentUri = value;
				}
			}
	public bool Discard
			{
				get
				{
					return discard;
				}
				set
				{
					discard = value;
				}
			}
	public String Domain
			{
				get
				{
					return domain;
				}
				set
				{
					domainImplicit = false;
					if(value != null)
					{
						domain = value;
					}
					else
					{
						domain = String.Empty;
					}
				}
			}
	public bool Expired
			{
				get
				{
					return expired;
				}
				set
				{
					expired = value;
				}
			}
	public DateTime Expires
			{
				get
				{
					return expires;
				}
				set
				{
					expires = value;
				}
			}
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					if(value != null)
					{
						name = value;
					}
					else
					{
						name = String.Empty;
					}
				}
			}
	public String Path
			{
				get
				{
					return path;
				}
				set
				{
					if(value != null)
					{
						path = value;
					}
					else
					{
						path = String.Empty;
					}
				}
			}
	public String Port
			{
				get
				{
					return port;
				}
				set
				{
					portImplicit = false;
					if(value != null)
					{
						port = value;
					}
					else
					{
						port = String.Empty;
					}
				}
			}
	public bool Secure
			{
				get
				{
					return secure;
				}
				set
				{
					secure = value;
				}
			}
	public DateTime TimeStamp
			{
				get
				{
					return timeStamp;
				}
			}
	public String Value
			{
				get
				{
					return name;
				}
				set
				{
					if(value != null)
					{
						this.value = value;
					}
					else
					{
						this.value = String.Empty;
					}
				}
			}
	public int Version
			{
				get
				{
					return version;
				}
				set
				{
					version = value;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object comparand)
			{
				Cookie other = (comparand as Cookie);
				if(other != null)
				{
				#if !ECMA_COMPAT
					if(String.Compare(name, other.name, true,
								      CultureInfo.InvariantCulture) != 0)
				#else
					if(String.Compare(name, other.name, true) != 0)
				#endif
					{
						return false;
					}
					if(value != other.value)
					{
						return false;
					}
					if(path != other.path)
					{
						return false;
					}
				#if !ECMA_COMPAT
					if(String.Compare(domain, other.domain, true,
								      CultureInfo.InvariantCulture) != 0)
				#else
					if(String.Compare(domain, other.domain, true) != 0)
				#endif
					{
						return false;
					}
					return (version == other.version);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return ToString().GetHashCode();
			}

	// Special non-token characters.
	private static char[] specials =
		{'(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"',
		 '/', '[', ']', '?', '=', '{', '}', ' ', '\t'};

	// Quote a string and write it to a builder.
	private static void QuoteString(StringBuilder builder, String value)
			{
				int posn;
				char ch;

				// If the value contains token characters, then don't quote.
				for(posn = 0; posn < value.Length; ++posn)
				{
					ch = value[posn];
					if(ch < 0x20 || ch >= 0x7F)
					{
						break;
					}
				}
				if(posn >= value.Length && value.IndexOfAny(specials) == -1)
				{
					builder.Append(value);
					return;
				}

				// Quote the string.
				builder.Append('"');
				for(posn = 0; posn < value.Length; ++posn)
				{
					ch = value[posn];
					if(ch == '"')
					{
						builder.Append('\\');
						builder.Append('"');
					}
					else if(ch == '\\')
					{
						builder.Append('\\');
						builder.Append('\\');
					}
					else
					{
						builder.Append(ch);
					}
				}
				builder.Append('"');
			}

	// Convert this object into a string.
	public override String ToString()
			{
				if(name.Length == 0 && value.Length == 0)
				{
					return String.Empty;
				}
				StringBuilder builder = new StringBuilder();
				if(version != 0)
				{
					builder.Append("$Version=");
					builder.Append(version.ToString());
					builder.Append("; ");
				}
				builder.Append(name);
				builder.Append('=');
				builder.Append(value);
				if(path.Length != 0)
				{
					builder.Append("; $Path=");
					QuoteString(builder, path);
				}
				if(!domainImplicit && domain.Length != 0)
				{
					builder.Append("; $Domain=");
					QuoteString(builder, domain);
				}
				if(!portImplicit && port.Length != 0)
				{
					builder.Append("; $Port=");
					QuoteString(builder, port);
				}
				return builder.ToString();
			}

}; // class Cookie

#endif // !ECMA_COMPAT

}; // namespace System.Net
