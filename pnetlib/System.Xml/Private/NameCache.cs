/*
 * NameCache.cs - Implementation of the "System.Xml.Private.NameCache" class.
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
 
namespace System.Xml.Private
{

using System;
using System.Collections;

// The name cache keeps track of "prefix, localname, namespaceURI"
// tuples so that there is only one of each tuple in a document.
// This can substantially reduce the memory requirements.

internal sealed class NameCache : Hashtable
{
	// Internal state.
	private XmlNameTable nameTable;
	private String emptyString;

	// Constructor.
	public NameCache(XmlNameTable nt) : base(128)  // avoid expanding of hashtable
			{
				nameTable = nt;

				// only NameTable is guaranted to use String.Empty
				emptyString = nt.Add(String.Empty);
			}

	// Add an entry to the name cache.
	public NameInfo Add(String localName, String prefix, String ns)
			{
				// Intern the strings into the name table.
				String name;
				if(localName != null)
				{
					localName = nameTable.Add(localName);
				}
				else
				{
					localName = emptyString;
				}
				if(prefix != null)
				{
					prefix = nameTable.Add(prefix);
				}
				else
				{
					prefix = emptyString;
				}
				if(ns != null)
				{
					ns = nameTable.Add(ns);
				}
				else
				{
					ns = emptyString;
				}
				if(prefix.Length > 0)
				{
					name = nameTable.Add(prefix + ":" + localName);
				}
				else
				{
					name = localName;
				}

				// Search for an existing entry with this name.
				NameInfo info = (NameInfo)(this[localName]);
				NameInfo first = info;
				while(info != null)
				{
					if(((Object)(info.localName)) == ((Object)localName) &&
					   ((Object)(info.prefix)) == ((Object)prefix) &&
					   ((Object)(info.ns)) == ((Object)ns))
					{
						return info;
					}
					info = info.next;
				}

				// Add a new entry to the hash table.
				info = new NameInfo(localName, prefix, name, ns, first);
				this[localName] = info;
				return info;
			}

	// Short-cut the hash table so that it compares keys much more quickly.
	protected override bool KeyEquals(Object item, Object key)
			{
				return (item == key);
			}
	protected override int GetHash(Object key)
			{
				return key.GetHashCode();
			}

	// Name information.
	public sealed class NameInfo
	{
		// Internal state.
		public String localName;
		public String prefix;
		public String name;
		public String ns;
		public NameInfo next;

		// Constructor.
		public NameInfo(String localName, String prefix,
						String name, String ns, NameInfo next)
			{
				this.localName = localName;
				this.prefix = prefix;
				this.name = name;
				this.ns = ns;
				this.next = next;
			}

	}; // class NameInfo

}; // class NameCache

}; // namespace System.Xml.Private
