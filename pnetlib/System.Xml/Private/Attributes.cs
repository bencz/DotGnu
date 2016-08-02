/*
 * Attributes.cs - Implementation of the "System.Xml.Private.Attributes" class.
 *
 * Copyright (C) 2004  Free Software Foundation, Inc.
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
using System.Xml;
using System.Collections;

internal sealed class Attributes : XmlErrorProcessor
{
	// Internal state.
	private int count;
	private ArrayList attributes;
	private Hashtable names;
	private XmlNameTable nt;
	private XmlNamespaceManager nm;

	// Constructor.
	public Attributes(ErrorHandler error)
			: base(error)
			{
				count = 0;
				nt = null;
				nm = null;
				attributes = new ArrayList(8);
				names = new Hashtable(new Key(null), new Key(null));
			}


	// Get or set the count of attribute information nodes.
	public int Count
			{
				get { return count; }
				set { count = value; }
			}

	// Get the attribute information node at the given index.
	public AttributeInfo this[int index]
			{
				get
				{
					if(index >= attributes.Count)
					{
						attributes.Add(new AttributeInfo());
					}
					return (AttributeInfo)attributes[index];
				}
			}


	// Find the index of the attribute with the given name information.
	public int Find(String localName, String namespaceURI)
			{
				// create the search key
				Key key;
				if(nm == null && (namespaceURI == null || namespaceURI == ""))
				{
					key = new Key(nt.Get(localName));
				}
				else
				{
					key = new Key(nt.Get(localName), nt.Get(namespaceURI));
				}

				// perform the lookup and return the results
				Object val = names[key];
				if(val == null) { return -1; }
				return (int)val;
			}
	public int Find(String name)
			{
				// create the search key
				Key key;
				if(nm == null)
				{
					key = new Key(nt.Get(name));
				}
				else
				{
					int index = name.LastIndexOf(':');
					if(index == 0) { return -1; }
					if(index > 0)
					{
						String prefix = nt.Get(name.Substring(0, index));
						String localName = nt.Get(name.Substring(index + 1));
						String namespaceURI = nm.LookupNamespace(prefix);
						key = new Key(localName, namespaceURI);
					}
					else
					{
						key = new Key(nt.Get(name), String.Empty);
					}
				}

				// perform the lookup and return the results
				Object val = names[key];
				if(val == null) { return -1; }
				return (int)val;
			}

	// Reset the attribute information.
	public void Reset()
			{
				count = 0;
				names.Clear();
				nt = null;
				nm = null;
			}

	// Update the search information.
	public void UpdateInfo(XmlNameTable nt)
			{
				this.nt = nt;
				this.nm = null;
				names.Clear();

				for(int i = 0; i < count; ++i)
				{
					// get the attribute name information
					AttributeInfo info = (AttributeInfo)attributes[i];
					String name = info.Name;

					// create the key
					Key key = new Key(name);

					// check for duplicate
					if(names.ContainsKey(key))
					{
						Error(/* TODO */);
					}

					// add the key and index to the hash table
					names.Add(key, i);
				}
			}

	// Update the search and namespace information.
	public void UpdateInfo(XmlNameTable nt, XmlNamespaceManager nm)
			{
				this.nt = nt;
				this.nm = nm;
				names.Clear();

				for(int i = 0; i < count; ++i)
				{
					// get the attribute name information
					AttributeInfo info = (AttributeInfo)attributes[i];
					String name = info.Name;

					// set the defaults
					String localName = name;
					String prefix = String.Empty;
					String namespaceURI = String.Empty;

					// find the namespace separator
					int index = name.LastIndexOf(':');

					// give an error if there's no name before the separator
					if(index == 0)
					{
						Error(/* TODO */);
					}

					// set the namespace information
					if(index > 0)
					{
						// add the prefix and local name to the name table
						prefix = nt.Add(name.Substring(0, index));
						localName = nt.Add(name.Substring(index + 1));

						// check for a valid prefix
						if(prefix.IndexOf(':') != -1)
						{
							Error(/* TODO */);
						}

						// set the namespace uri based on the prefix
						namespaceURI = nm.LookupNamespace(prefix);
					}
					else if(localName == "xmlns")
					{
						namespaceURI = nt.Add(XmlDocument.xmlns);
					}

					// create the key
					Key key = new Key(localName, namespaceURI);

					// check for duplicate
					if(names.ContainsKey(key))
					{
						Error(/* TODO */);
					}

					// add the key and index to the hash table
					names.Add(key, i);

					// update the current attribute's namespace information
					info.UpdateNamespaceInfo(localName, namespaceURI, prefix);
				}
			}










	private struct Key : IComparer, IHashCodeProvider
	{
		// Publicly-accessible state.
		public Object localName;
		public Object namespaceURI;


		// Constructors.
		public Key(String localName, String namespaceURI)
				{
					this.localName = localName;
					this.namespaceURI = namespaceURI;
				}
		public Key(String name)
				{
					this.localName = name;
					this.namespaceURI = null;
				}

		// Explicit IComparer implementation.
		int IComparer.Compare(Object x, Object y)
				{
					if((x is Key) && (y is Key))
					{
						Key one = (Key)x;
						Key two = (Key)y;
						if((one.localName == two.localName) &&
						   (one.namespaceURI == two.namespaceURI))
						{
							return 0;
						}
					}
					return 1;
				}

		// Explicit IHashCodeProvider implementation.
		int IHashCodeProvider.GetHashCode(Object obj)
				{
					if(!(obj is Key)) { return 0; }
					Key k = (Key)obj;
					String ln = (String)k.localName;
					String ns = (String)k.namespaceURI;
					if(ln == null) { return 0; }
					int hash = ln.GetHashCode();
					if(ns != null)
					{
						hash = (hash << 16) + ns.GetHashCode();
					}
					return hash;
				}

	}; // class Key

}; // class Attributes

}; // namespace System.Xml.Private
