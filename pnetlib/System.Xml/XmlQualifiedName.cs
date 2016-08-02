/*
 * XmlQualifiedName.cs - Implementation of the "System.Xml.XmlQualifiedName" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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

using System;
using System.Text;

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlQualifiedName
{
	// Internal state.
	private String name;
	private String ns;
	public static readonly XmlQualifiedName Empty = new XmlQualifiedName();

	// Constructors.
	public XmlQualifiedName()
			{
				this.name = String.Empty;
				this.ns = String.Empty;
			}
	public XmlQualifiedName(String name)
			{
				this.name = ((name != null) ? name : String.Empty);
				this.ns = String.Empty;
			}
	public XmlQualifiedName(String name, String ns)
			{
				this.name = ((name != null) ? name : String.Empty);
				this.ns = ((ns != null) ? ns : String.Empty);
			}

	// Determine if this qualified name is empty.
	public bool IsEmpty
			{
				get
				{
					return (name == String.Empty && ns == String.Empty);
				}
			}

	// Get the name from this object.
	public String Name
			{
				get
				{
					return name;
				}
			}

	// Get the namespace from this object.
	public String Namespace
			{
				get
				{
					return ns;
				}
			}

	// Determine if two qualified names are equal.
	public override bool Equals(Object obj)
			{
				XmlQualifiedName other = (obj as XmlQualifiedName);
				if(other != null)
				{
					return (name == other.name && ns == other.ns);
				}
				return false;
			}

	// Get the hash code for this qualified name.
	public override int GetHashCode()
			{
				return name.GetHashCode();
			}

	// Convert this qualified name into a string.
	public override String ToString()
			{
				if(ns.Length > 0)
				{
					return ns + ":" + name;
				}
				else
				{
					return name;
				}
			}

	// Convert a name/namespace pair into a qualified name.
	public static String ToString(String name, String ns)
			{
				if(ns != null && ns.Length > 0)
				{
					return ns + ":" + ((name != null) ? name : String.Empty);
				}
				else
				{
					return ((name != null) ? name : String.Empty);
				}
			}

	// Determine if two qualified names are equal.
	public static bool operator==(XmlQualifiedName a, XmlQualifiedName b)
			{
				if(ReferenceEquals(a,null))	
				{
					return (ReferenceEquals(b,null));
				}
				else if (ReferenceEquals(b,null))
				{
					return false;
				}
				else
				{
					return (a.name == b.name && a.ns == b.ns);
				}
			}

	// Determine if two qualified names are not equal.
	public static bool operator!=(XmlQualifiedName a, XmlQualifiedName b)
			{
				if(ReferenceEquals(a,null))
				{
					return (!ReferenceEquals(b,null));
				}
				else if(ReferenceEquals(b,null))
				{
					return true;
				}
				else
				{
					return (a.name != b.name || a.ns != b.ns);
				}
			}

}; // class XmlQualifiedName

}; // namespace System.Xml
