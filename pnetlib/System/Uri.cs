/*
 * Uri.cs - Implementation of "System.Uri" class 
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 * 
 * Contributed by Stephen Compall <rushing@sigecom.net>
 * Contributions by Gerard Toonstra <toonstra@ntlworld.com>
 * Contributions by Rich Baumann <biochem333@nyc.rr.com>
 * Contributions by Gopal V <gopalv82@symonds.net>
 * Contributions by Rhys Weatherley <rweather@southern-storm.com.au>
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

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Private;
using System.Net.Sockets;
using System.Collections;
using System.Runtime.Serialization;

namespace System
{

#if CONFIG_SERIALIZATION
[Serializable]
public class Uri : MarshalByRefObject, ISerializable
#else
public class Uri : MarshalByRefObject
#endif
{

	public static readonly String SchemeDelimiter = "://";

	public static readonly String UriSchemeFile = "file";

	public static readonly String UriSchemeFtp = "ftp";

	public static readonly String UriSchemeGopher = "gopher";

	public static readonly String UriSchemeHttp = "http";

	public static readonly String UriSchemeHttps = "https";

	public static readonly String UriSchemeMailto = "mailto";

	public static readonly String UriSchemeNews = "news";

	public static readonly String UriSchemeNntp = "nntp";

	/* Fast Regex based URI parser */
	private static Regex uriRegex = null;
	private static bool hasFastRegex = false;
	private static readonly String hexChars = "0123456789ABCDEF";
	private static Hashtable schemes=new Hashtable(10);  // avoid expanding of hashtable
	

	/* State specific fields */
	private bool				userEscaped		= false ;
	private UriHostNameType		hostNameType	= UriHostNameType.Unknown;
	private String				scheme			= null;
	private String				delim			= null;
	private String				userinfo		= null;
	private String				host 			= null;
	private int					port 			= -1;
	private String				portString		= null;
	private String				path			= null;
	private String				query			= null;
	private String				fragment		= null;
	
	
	static Uri()
	{
   /* RFC2396 : Appendix B
		As described in Section 4.3, the generic URI syntax is not sufficient
		to disambiguate the components of some forms of URI.  Since the
		"greedy algorithm" described in that section is identical to the
		disambiguation method used by POSIX regular expressions, it is
		natural and commonplace to use a regular expression for parsing
		^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?
		12            3  4          5       6  7        8 9
		
		(Modified to support mailto: syntax as well)
	*/
      String regularexpression= 
	  	"^(([^:/?#]+):)?"+ // scheme
		"(" +
			"(//)?"+ // delim
			"(([^@]+)@)?" + // userinfo
			"([^/?#:]*)?" + // host
			"(:([^/?#]+))?"+ // port
		")"+
		"([^?#]*)"+  // path
		"(\\?([^#]*))?"+ // query
		"(#(.*))?$"; // fragment
			/*	
			Indexing
			0 --> full
			2 --> scheme
			4 --> delim
			6 --> userinfo
			7 --> host
			9 --> port
			10 --> path
			11 --> query
			13 --> fragment */
			
		/* Insert grouping symbols */
		regularexpression=regularexpression.Replace("(","\\("); 
		regularexpression=regularexpression.Replace(")","\\)");

		try
		{
			uriRegex = new Regex(regularexpression, RegexSyntax.PosixCommon);

			/* Really test this expression on each start-up, use the 
			 * backup SlowParse() if this library fails to work 
			 */
			String input=
				"https://gnu:gnu@www.gnu.org:443/free/software?id=for#all";
		
			RegexMatch[] matches = uriRegex.Match( input, 
												RegexMatchOptions.Default,
												16);	
			hasFastRegex = true;
			int [][] expected = new int[][] { 
							new int[]{0,56},
							new int[]{0,6},
							new int[]{0,5},
							new int[]{6,31},
							new int[]{6,8},
							new int[]{8,16},
							new int[]{8,15},
							new int[]{16,27},
							new int[]{27,31},
							new int[]{28,31},
							new int[]{31,45},
							new int[]{45,52},
							new int[]{46,52},
							new int[]{52,56},
							new int[]{53,56},
					};
			
			for(int i=0;i< matches.Length && hasFastRegex ; i++)
			{
				hasFastRegex = hasFastRegex && 
								(matches[i].start == expected[i][0]) &&
								(matches[i].end == expected[i][1]) ;
			}
		}
		catch
		{
			if(uriRegex!=null) 
			{
				uriRegex.Dispose();
				uriRegex = null;
			}
			hasFastRegex = false;
		}

		schemes.Add(UriSchemeFile,	new UriScheme(-1,	"://"));
		schemes.Add(UriSchemeFtp,		new UriScheme(23,	"://"));
		schemes.Add(UriSchemeGopher,	new UriScheme(70,	"://"));
		schemes.Add(UriSchemeHttp,	new UriScheme(80,	"://"));
		schemes.Add(UriSchemeHttps,	new UriScheme(443,	"://"));
		schemes.Add(UriSchemeNntp,	new UriScheme(119,	"://"));
		schemes.Add(UriSchemeMailto,	new UriScheme(25,	":"));
		schemes.Add(UriSchemeNews,	new UriScheme(-1,	":"));
	}

	public Uri(String uriString) : this(uriString, false)
	{
	}

	public Uri(String uriString, bool dontEscape)
	{
		userEscaped = dontEscape;
		ParseString(uriString,true);
		Escape();
		Canonicalize();
	}

#if CONFIG_SERIALIZATION
	protected Uri(SerializationInfo info, StreamingContext context)
		: this(info.GetString("AbsoluteUri"), true)
	{
		// Nothing to do here.
	}
#endif

	private Uri()
	{
		/* Be warned , using this is kinda ugly in the end */
	}

	public Uri(Uri baseUri, String relativeUri) : 
				this(baseUri, relativeUri, false)
	{
	}

	public Uri(Uri baseUri, String relativeUri, bool dontEscape)
	{
		
		if(relativeUri == null)
		{
			throw new ArgumentNullException("relativeUri");
		}
		
		userEscaped = dontEscape;
		this.scheme = baseUri.scheme;
		this.delim = baseUri.delim;
		this.host = baseUri.host;
		this.port = baseUri.port;
		this.userinfo = baseUri.userinfo;

		if(relativeUri == String.Empty)
		{
			this.path = baseUri.path;
			this.query = baseUri.query;
			this.fragment = baseUri.fragment;
			return;
		}
		
		Uri uri=new Uri();

		uri.ParseString(relativeUri,false);
		
		if(uri.scheme == null)
		{
			this.path = this.path + uri.path;	
			this.query = uri.query;
			this.fragment = uri.fragment;
		}
		else if(uri.scheme == this.scheme && uri.delim == ":")
		{
			this.path = this.path + uri.Authority + uri.path;	
			this.query = uri.query;
			this.fragment = uri.fragment;
		}
		else if(uri.scheme == this.scheme && uri.delim == "://")
		{
			ParseString(relativeUri,true);
		}
		else if(uri.scheme != this.scheme)
		{
			ParseString(relativeUri,true);
		}
		Escape();
		Canonicalize();
	}

	protected virtual void Canonicalize()
	{
		if(this.path!=null)
		{
			this.path = this.path.Replace('\\', '/');
			while (this.path.IndexOf("//") >= 0) // double-slashes to strip
			{
				this.path = this.path.Replace("//", "/");
			}
			path = StripMetaDirectories(path);
		}
	}

	private String StripMetaDirectories(String oldpath)
	{
		int toBeRemoved = 0;

		String[] dirs = oldpath.Split('/');

		for (int curDir = dirs.Length - 1; curDir > 0 ; curDir--)
		{
			if (dirs[curDir] == "..")
			{
				toBeRemoved++;
				dirs[curDir] = null;
			}
			else if (dirs[curDir] == ".")
			{
				dirs[curDir] = null; 
			}
			else if (toBeRemoved > 0) // remove this one
			{
				toBeRemoved--;
				dirs[curDir] = null;
			}
		}
		
		if(dirs[0].Length==0) // leading slash
		{
			dirs[0]=null;
		}

		if (toBeRemoved > 0) // wants to delete root
			throw new UriFormatException
			  (S._("Arg_UriPathAbs"));

		StringBuilder newpath = new StringBuilder(oldpath.Length);
		foreach (String dir in dirs)
		{
			if (dir!=null)
			{
				newpath.Append('/').Append(dir);
			}
		}

		// we always must have at least a slash
		// general assumption that path based systems use "://" instead
		// of the ":" only delimiter
		if (delim=="://")
		{
			if(newpath.Length == 0)
			{
				newpath.Append('/');
			}
		}
		return newpath.ToString();
	}

	private static bool IsDnsName(String name)
	{
		foreach(String tok in name.Split('.'))
		{
			if(tok.Length==0 || !Char.IsLetterOrDigit(tok[0])) return false;
			for(int i=1; i< tok.Length ; i++)
			{
				if(!Char.IsLetterOrDigit(tok[i]) && tok[i]!='-' && tok[i]!='_')
				{
					return false;
				}
			}
		}
		return true;
	}

	public static UriHostNameType CheckHostName(String name)
	{
		if (name == null || name.Length == 0)
			return UriHostNameType.Unknown;
		
		try
		{
			switch(IPAddress.Parse(name).AddressFamily)
			{
				case AddressFamily.InterNetwork:
					return UriHostNameType.IPv4;
				case AddressFamily.InterNetworkV6:
					return UriHostNameType.IPv6;
			}
		}
		catch (FormatException)
		{
		}

		if(IsDnsName(name))
		{
			return UriHostNameType.Dns;
		}

		return UriHostNameType.Unknown;
	}

	public static bool CheckSchemeName(String schemeName)
	{
		if (schemeName == null || schemeName.Length == 0)
			return false;

		if (!Char.IsLetter(schemeName[0]))
		{
			return false;
		}

		for (int i = 1; i < schemeName.Length ; i++) 
		{
			if(!Char.IsLetterOrDigit(schemeName[i]))
			{
				switch(schemeName[i])
				{
					case '+':
					case '.':
					case '-':
					{
						// Nothing to do 
					}
					break;

					default:
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	protected virtual void CheckSecurity()
	{
		 // Nothing to do here.
	}

	public override bool Equals(Object comparand)
	{
		if(comparand == null)
		{
			return false;
		}
		Uri uri = (comparand as Uri);
		if(uri == null)
		{
			String s = (comparand as String);
			
			if(s==null) return false;
			try
			{
				uri = new Uri(s);
			}
			catch
			{
				return false;
			}
		}
		return 
			((this.Authority == uri.Authority) &&
			 (this.path == uri.path) &&
			 (this.scheme == uri.scheme) &&
			 (this.delim == uri.delim));
	}

	protected virtual void Escape()
	{
		if(!userEscaped)
		{
			if(this.host!=null)
			{
				this.host = EscapeStringInternal(this.host, true, false);
			}
			if(this.path!=null)
			{
				this.path = EscapeStringInternal(this.path, true, true);
			}
			if(this.query!=null)
			{
				this.query = EscapeStringInternal(this.query, true, true);
			}
			if(this.fragment!=null)
			{
				this.fragment = 
						EscapeStringInternal(this.fragment, false, true);
			}
		}
	}

	protected static String EscapeString(String str)
	{
		if (str == null || str.Length == 0)
			return String.Empty;
		return EscapeStringInternal(str,true, true);
	}
	
	internal static String EscapeStringInternal (String str, 
											bool escapeHex, 
											bool escapeBrackets) 
	{
		StringBuilder sb = new StringBuilder();
		
		for(int i=0; i < str.Length; i++)
		{
			char c = str[i];

			if(c <= 0x20 || c >= 0x7f)
			{
				/* non-ascii */
				sb.Append(HexEscape(c));
			}
			else
			{
				switch(c)
				{
					case '<':
					case '>':
					case '%':
					case '"':
					case '{':
					case '}':
					case '|':
					case '\\':
					case '^':
					case '`':
					{
						sb.Append(HexEscape(c));
					}
					break;

					case '#':
					{
						if(escapeHex)
						{
							sb.Append(HexEscape(c));
						}
						else
						{
							sb.Append(c);
						}
					}
					break;

					case '[':
					case ']':
					{
						if(escapeBrackets)
						{
							sb.Append(HexEscape(c));
						}
						else
						{
							sb.Append(c);
						}
					}
					break;

					default:
					{
						sb.Append(c);
					}
					break;
				}
			}
		}
		return sb.ToString();
	}

	public static int FromHex(char digit)
	{
		switch(digit)
		{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			{
				return digit - '0';
			}
			break;

			case 'A':
			case 'B':
			case 'C':
			case 'D':
			case 'E':
			case 'F':
			{
				return digit - 'A' + 10;
			}
			break;

			case 'a':
			case 'b':
			case 'c':
			case 'd':
			case 'e':
			case 'f':
			{
				return digit - 'a' + 10;
			}
			break;
		}
		throw new ArgumentException(S._("Arg_HexDigit"), "digit");
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

	public String GetLeftPart(UriPartial part)
	{
		switch (part)
		{
		case UriPartial.Path:
			return String.Concat(this.scheme , this.delim, 
								this.Authority, this.path);
		case UriPartial.Authority:
			return String.Concat(this.scheme,
					     this.delim,
					     this.Authority);
		case UriPartial.Scheme:
			return String.Concat(this.scheme,
					     this.delim);
		default:
			throw new ArgumentException(S._("Arg_UriPartial"));
		}
	}

	public static String HexEscape(char character)
	{
		if (character > 255)
			throw new ArgumentOutOfRangeException("character");
		String retval="%";
		retval += hexChars[(character >> 4) & 0x0F];
		retval += hexChars[(character) & 0x0F];
		return retval;
	}
	
	private static char BinHexToChar(char hex1, char hex2)
	{
		hex1=Char.ToUpper(hex1);
		hex2=Char.ToUpper(hex2);
		return (char)((hexChars.IndexOf(hex1) << 4) | 
					(hexChars.IndexOf(hex2)));
	}

	public static char HexUnescape(String pattern, ref int index)
	{
		if(pattern == null)
		{
			throw new ArgumentNullException("pattern");
		}
		if(pattern.Length == 0)
		{
			throw new ArgumentException("pattern");
		}
		if(((pattern.Length < index) || (index < 0)))
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if(pattern.Length >= (index+3) && pattern[index] == '%')
		{
			if(IsHexDigit(pattern[index+1]) && IsHexDigit(pattern[index+2]))
			{
				index+=3;	
				return BinHexToChar(pattern[index-2],pattern[index-1]);
			}
		}
		index++;
		return pattern[index-1];
	}

	protected virtual bool IsBadFileSystemCharacter(char character)
	{
		switch(character)
		{
			case '"':
			case '<':
			case '>':
			case '|':
			case '\r':
			case '\n':
			{
				return true;
			}
			break;
		}
		return false;
	}

	protected static bool IsExcludedCharacter(char character)
	{
		if(character < 0x20 || character > 0x7F)
		{
			/* non-ascii */
			return true;
		}
		switch(character)
		{
			case '<':
			case '>':
			case '#':
			case '%':
			case '"':
			case '{':
			case '}':
			case '|':
			case '\\':
			case '^':
			case '[':
			case ']':
			case '`':
			{
				/* excluded ascii */
				return true;
			}
		}
		return false;
	}

	public static bool IsHexDigit(char character)
	{
		return
		(
			(character >= '0' && character <= '9')
			|| (character >= 'A' && character <= 'F')
			|| (character >= 'a' && character <= 'f')
		);
	}

	public static bool IsHexEncoding(String pattern, int index)
	{
		if (index >= 0 && pattern.Length - index >= 3)
			return ((pattern[index] == '%') &&
			    IsHexDigit(pattern[index+1]) &&
			    IsHexDigit(pattern[index+2]));
		else
			return false;
	}

	protected virtual bool IsReservedCharacter(char character)
	{
		return (";/:@&=+$,".IndexOf(character) != -1);
	}
	
	public String MakeRelative(Uri toUri)
	{
		if((this.Scheme != toUri.Scheme) ||
			(this.Authority != this.Authority))
		{
			return toUri.ToString();
		}
		if(this.path == toUri.path) 
		{
			return String.Empty;
		}
		String []seg1 = this.Segments;
		String []seg2 = toUri.Segments;

		int k;	
		int min = seg1.Length;
		if(min > seg2.Length)
			min = seg2.Length;
		for(k=0;k < min ; k++)
		{
			if(seg1[k] != seg2[k])
			{
				break;
			}
		}

		StringBuilder sb = new StringBuilder();
		for(int i = k ; i < seg1.Length ; i++)
		{
			if(seg1[i].EndsWith("/"))
			{
				sb.Append("../");
			}
		}
		for(int i = k ; i < seg2.Length ; i++)
		{
			sb.Append(seg2[i]);
		}
		return sb.ToString();
	}

	protected virtual void Parse()
	{
	}

	private void ParseString(String uriString,bool reportErrors)
	{
		if(hasFastRegex)
		{
			FastParse(uriString);
		}
		else
		{
			SlowParse(uriString);
		}
		if(reportErrors)
		{
			CheckParsed();
		}
	}

	private void CheckParsed()
	{
		if((this.host==null || this.host=="") && 
			(this.scheme=="file" || this.scheme==null))
		{
			this.scheme=UriSchemeFile;
			this.delim="://";
		}
		else
		{
			this.hostNameType = CheckHostName(this.host);
			if(hostNameType==UriHostNameType.Unknown)
			{
				throw new UriFormatException(S._("Arg_UriHostName"));
			}
		}

		if(!CheckSchemeName(this.scheme))
		{
			throw new UriFormatException(S._("Arg_UriScheme"));
		}
		if(portString!= null)
		{
			try
			{
				int value=Int32.Parse(portString);
				port = value;
			}
			catch(FormatException)
			{
				this.port = -1; 
			}
			catch(OverflowException)
			{
				throw new UriFormatException (S._("Arg_UriPort"));
			}
		}
		else
		{
			this.port = DefaultPortForScheme(this.scheme);
		}
	}

	private static String MatchToString(String str, RegexMatch[] matches, 
										int index)
	{
		if(matches==null || matches.Length <= index) return null;
		if(matches[index].start == -1) return null;
		return str.Substring(matches[index].start, 
							matches[index].end - matches[index].start);
	}

	private void FastParse(String uriString)
	{
		RegexMatch[] matches = uriRegex.Match( uriString, 
											RegexMatchOptions.Default,
											16);
		if(matches == null)
		{
			throw new UriFormatException();
		}

		/*
			0 --> full
			2 --> scheme
			4 --> delim
			6 --> userinfo
			7 --> host
			9 --> port
			10 --> path
			11 --> query
			13 --> fragment */
		this.scheme = MatchToString(uriString, matches,2);
		this.delim = ":"+MatchToString(uriString, matches, 4);
		this.userinfo = MatchToString(uriString, matches, 6);
		this.host = MatchToString(uriString, matches,7);
		this.portString = MatchToString(uriString, matches,9);
		this.path = MatchToString(uriString, matches, 10);
		this.query = MatchToString(uriString, matches, 11);
		this.fragment = MatchToString(uriString, matches,13);
	}

	private void SlowParse(String uriString)
	{
		throw new NotImplementedException("SlowParse");
	}

	public override String ToString()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append(this.scheme);

		sb.Append(this.delim);

		sb.Append(Authority);

		sb.Append(PathAndQuery);

		sb.Append(this.fragment);
		
		if(this.userEscaped)
		{
			return sb.ToString();
		}
		else
		{
			return Unescape(sb.ToString());
		}
	}

	protected virtual String Unescape(String str)
	{
		if(str == null || str.Length==0) return String.Empty;
		StringBuilder sb = new StringBuilder(str.Length);
			
		for(int i=0; i < str.Length;)
		{
			sb.Append(HexUnescape(str, ref i)); 
		}
		return sb.ToString();
	}

	public String AbsolutePath 
	{
		get
		{
			if(path == null) return "/";
			return path;
		}
	}

	public String AbsoluteUri 
	{
 		get
		{
			return this.ToString();	
		}
	}

	public String Authority 
	{
 		get
		{
			StringBuilder sb=new StringBuilder();
			if(this.userinfo!=null)
			{
				sb.Append(this.userinfo);
				sb.Append('@');
			}
			if(this.host!=null)
			{
				sb.Append(this.host);
			}
			if(this.port!=-1 && this.port!=DefaultPortForScheme(this.scheme))
			{
				sb.Append(':');
				sb.Append(this.port);
			}
			return sb.ToString();
		}
	}

	public String Fragment 
	{
 		get
		{
			return fragment;
		}
	}

	public String Host 
	{
		get
		{
			return host;
		}
	}

	public UriHostNameType HostNameType 
	{
 		get
		{
			return hostNameType;
		}
	}

	public bool IsDefaultPort 
	{
 		get
		{
			if(port == DefaultPortForScheme(scheme))
			{
				return true;
			}
			return false;
		}
	}

	public bool IsFile 
	{
		get
		{
			return String.Equals(this.scheme, Uri.UriSchemeFile);
		}
	}

	public bool IsLoopback 
	{
 		get
		{
			try
			{
				IPAddress ip=IPAddress.Parse(this.host);
				return IPAddress.IsLoopback(ip);
			}
			catch(FormatException) // should be a name
			{
				try
				{
					IPHostEntry iph = Dns.GetHostByName(this.host);
					foreach(IPAddress ip in iph.AddressList)
					{
						if(IPAddress.IsLoopback(ip))return true;
					}
				}
				catch(SocketException) // cannot resolve name either
				{
					return false;
				}
			}
			return false; // no way out now 
		}
	}

	public String LocalPath 
	{
 		get
		{
			String retval = this.AbsolutePath;
			if (this.IsFile)
			{
			    if (Path.DirectorySeparatorChar != '/')
					retval = retval.Replace('/', Path.DirectorySeparatorChar);
				retval = this.Unescape(retval);
			}
			return retval;
		}
	}

	public String PathAndQuery 
	{
 		get
		{
			String abspath = this.AbsolutePath;
			if (String.Equals(abspath, ""))
				return this.Query;
			else if (String.Equals(this.query, ""))
				return abspath;
			else
				return String.Concat(this.path, this.query);
		}
	}

	public int Port 
	{
 		get
		{
			return port;
		}
	}

	public String Query 
	{
 		get
		{
			return this.query;
		}
	}

	public String Scheme 
	{
 		get
		{
			return this.scheme;
		}
	}

#if !ECMA_COMPAT
	public
#else
	private
#endif 
	String [] Segments
	{
		get
		{
			if(path == null || path == String.Empty)
			{
				return new String[0];
			}
			if(path == "/")
			{
				return new String[]{"/"};
			}
			String [] toks = path.Split('/');
			bool endSlash = path.EndsWith("/");
			for(int i=0;i<toks.Length-1; i++)
			{
				toks[i]+="/";
			}
			String [] segments = new String[toks.Length - (endSlash ? 1 : 0)]; 
			Array.Copy(toks, segments, toks.Length - (endSlash ? 1 :0));
			return segments;
		}
	}


	public bool UserEscaped 
	{
 		get
		{
			return this.userEscaped;
		}
	}

	public String UserInfo 
	{
 		get
		{
			return this.userinfo;
		}
	}

	private class UriScheme
	{
		public String delim;
		public int port;
		
		public UriScheme(int port, String delim)
		{
			this.port = port;
			this.delim = delim;
		}
	}

	internal static int DefaultPortForScheme(String name)
	{
		UriScheme entry=(UriScheme) schemes[name];
		if((entry=(UriScheme)schemes[name])!=null)
		{
			return entry.port;
		}
		return -1;
	}

	internal static String DefaultDelimiterForScheme (String name)
	{
		UriScheme entry;
		if((entry=(UriScheme)schemes[name])!=null)
		{
			return entry.delim;
		}
		return "://";
	}	

	// Determine if this URI is a prefix of a specified URI.
	internal bool IsPrefix(Uri uri)
	{
		String strhost = this.Host;
		String specifiedstrhost = uri.Host;
		String strpath = this.LocalPath;
		String specifiedstrpath=uri.LocalPath;
		String strscheme = this.Scheme;
		String specifiedstrscheme = uri.Scheme;
		
		if(String.Compare(strscheme, specifiedstrscheme, true) == 0 && String.Compare(strhost, specifiedstrhost, true) == 0)
		{
			if(String.CompareOrdinal(strpath, specifiedstrpath) == 0)
			{
				//if paths exactly the same, return true
				return true;
					
			} else if(String.CompareOrdinal(strpath, strpath.Length, "/", 0, 1) == 0 )
			{
				//path string has / at the end of it, so direct comparison can be made.
				if (String.CompareOrdinal(strpath, 0, specifiedstrpath, 0, strpath.Length) == 0)
				{
					return true;
				} else {
					return false;
				}
													
			} else {

				// a / must be appended to this.LocalPath to do comparison
				strpath = strpath + "/";
				if (String.CompareOrdinal(strpath, 0, specifiedstrpath, 0, strpath.Length) == 0)
				{
					return true;
				} else {
					return false;
				} 
			}
		} else {
			return false;
		}
	}


	// Determine if this is an UNC path.
	public bool IsUnc
	{
		get
		{
			// We don't support UNC paths at present.
			return false;
		}
	}

#if CONFIG_SERIALIZATION

	// Serialize this URI object.
	void ISerializable.GetObjectData(SerializationInfo serializationInfo,
									 StreamingContext streamingContext)
	{
		serializationInfo.AddValue("AbsoluteUri", AbsoluteUri);
	}

#endif

}
}//namespace
