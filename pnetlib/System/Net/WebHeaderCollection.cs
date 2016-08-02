/*
 * WebHeaderCollection.cs - Implementation of the "System.Net.WebHeaderCollection" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(true)]
#endif
public class WebHeaderCollection : NameValueCollection
{
	private static int[] restrictedHeaders=
	{
		"accept".GetHashCode(),
		"connection".GetHashCode(),
		"content-length".GetHashCode(),
		"content-type".GetHashCode(),
		"date".GetHashCode(),
		"expect".GetHashCode(),
		"host".GetHashCode(),
		"range".GetHashCode(),
		"referrer".GetHashCode(),
		"transfer-encoding".GetHashCode(),
		"user-agent".GetHashCode()
	};
	private static char[] tspecials= new char [] 
						{'(', ')', '<', '>', '@',
					     ',', ';', ':', '\\', '"',
					     '/', '[', ']', '?', '=',
					     '{', '}', ' ', '\t'};
	
	private bool strict=true;
	public WebHeaderCollection() 
	{	
		/* nothing here ? */	
	}
	internal void SetStrict(bool strict)
	{
		this.strict=strict;
	}
	
	public void Add(string header) 
	{
		if(header==null || header==String.Empty)
		{
			throw new ArgumentNullException("header");
		}
		int col=header.IndexOf(":");
		if(col==-1)
		{
			throw new ArgumentException("missing ':' in header"); /*TODO:I18n*/
		}
		Add(header.Substring(0,col),header.Substring(col+1));
	}

	public override void Add(string name, string value) 
	{
		if(name==null)
		{
			throw new ArgumentNullException("name");
		}
		if(strict && IsRestricted(name))
		{
			throw new ArgumentException(S._("Arg_RestrictedHeader"));
		}
		AddWithoutValidate(name,value);
	}
	
	
	protected void AddWithoutValidate(string headerName, string headerValue) 
	{
		headerName=headerName.Trim();
		if(!IsValidHeaderName(headerName))
		{
			throw new ArgumentException(S._("Arg_InvalidHeader"));
		}
		if(headerValue==null)headerValue="";
		else headerValue=headerValue.Trim(); /* remove excess LWS */
		base.Add(headerName,headerValue); /* add to NameValueCollection */
	}

	
	public override String[] GetValues(string header) 
	{ 
		if(header==null)
		{
			throw new ArgumentNullException("header");
		}
		return base.GetValues(header);
	}
	
	public static bool IsRestricted(string headerName)
	{ 
		int hash=headerName.ToLower().GetHashCode(); /* case insensitive ? */
		for(int i=0;i<restrictedHeaders.Length;i++)
		{
			if(restrictedHeaders[i]==hash)return true;
		}
		return false;
	}

	public override void Remove(string name) 
	{
		if(name==null)
		{
			throw new ArgumentNullException("name");
		}
		if(!IsValidHeaderName(name))
		{
			throw new ArgumentException(S._("Arg_InvalidHeader"));
		}
		if(strict && IsRestricted(name))
		{
			throw new ArgumentException(S._("Arg_RestrictedHeader"));
		}
		RemoveInternal(name);
	}	

	internal void RemoveInternal(String name)
	{
		base.Remove(name);
	}
	
	public override void Set(string name, string value) 
	{
		if(name==null)
		{
			throw new ArgumentNullException("name");
		}
		if(!IsValidHeaderName(name))
		{
			throw new ArgumentException(S._("Arg_InvalidHeader"));
		}
		if(strict && IsRestricted(name))
		{
			throw new ArgumentException(S._("Arg_RestrictedHeader"));
		}
		SetInternal(name,value);
	}

	internal void SetInternal(String name,String value)
	{
		if(value==null)value="";
		else value=value.Trim(); // LWS 
		base.Set(name,value);
	}

	public override String ToString()
	{
		StringBuilder builder=new StringBuilder(40*this.Count); 
		/* an assumption :-) */
		foreach(String key in this)
		{
			builder.Append(key+": "+this[key]+"\r\n");
		}
		return builder.ToString();
	}

	// private methods
	private static bool IsValidHeaderName(String name)
	{

        /*  token          = 1*<any CHAR except CTLs or tspecials> 

          tspecials      = "(" | ")" | "<" | ">" | "@"
                         | "," | ";" | ":" | "\" | <">
                         | "/" | "[" | "]" | "?" | "="
                         | "{" | "}" | SP | HT
		*/
		if(name == null || name.Length == 0) return false;
		char[] chars=name.ToCharArray(); 
		int len=chars.Length;
		for(int i=0;i< len; i++)
		{
			if(chars[i]< 0x20 || chars[i]>=0x7f) /* no Unicode here */
			{
				return false;
			}
		}
		return name.IndexOfAny(tspecials) == -1; 
	}
		
}; //class WebHeaderCollection

}; //namespace System.Net

