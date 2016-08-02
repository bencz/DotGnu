/*
 * XmlNamedNodeMap.cs - Implementation of the
 *		"System.Xml.XmlNamedNodeMap" class.
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
using System.Collections;

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlNamedNodeMap : IEnumerable
{
	// Internal state.
	internal ArrayList map;
	private XmlNode parent;

	// Constructor.
	internal XmlNamedNodeMap(XmlNode parent)
			{
				this.map = new ArrayList();
				this.parent = parent;
			}

	// Get the number of items in the map.
	public virtual int Count
			{
				get
				{
					return map.Count;
				}
			}

	// Enumerate over this map.
	public virtual IEnumerator GetEnumerator()
			{
				return map.GetEnumerator();
			}

	// Get a particular node by name from this map.
	public virtual XmlNode GetNamedItem(String name)
			{
				if(name == null)
				{
					name = String.Empty;
				}
				int posn, count;
				XmlNode node;
				count = map.Count;
				for(posn = 0; posn < count; ++posn)
				{
					node = (XmlNode)(map[posn]);
					if(node.Name == name)
					{
						return node;
					}
				}
				return null;
			}
	public virtual XmlNode GetNamedItem(String localName, String namespaceURI)
			{
				if(localName == null)
				{
					localName = String.Empty;
				}
				if(namespaceURI == null)
				{
					namespaceURI = String.Empty;
				}
				int posn, count;
				XmlNode node;
				count = map.Count;
				for(posn = 0; posn < count; ++posn)
				{
					node = (XmlNode)(map[posn]);
					if(node.LocalName == localName &&
					   node.NamespaceURI == namespaceURI)
					{
						return node;
					}
				}
				return null;
			}

	// Retrieve a particular item.
	public virtual XmlNode Item(int index)
			{
				if(index < 0 || index >= map.Count)
				{
					return null;
				}
				else
				{
					return (XmlNode)(map[index]);
				}
			}

	// Remove a particular node by name from this map.
	public virtual XmlNode RemoveNamedItem(String name)
			{
				if(name == null)
				{
					name = String.Empty;
				}
				int posn, count;
				XmlNode node;
				count = map.Count;
				for(posn = 0; posn < count; ++posn)
				{
					node = (XmlNode)(map[posn]);
					if(node.Name == name)
					{
						map.RemoveAt(posn);
						return node;
					}
				}
				return null;
			}
	public virtual XmlNode RemoveNamedItem
				(String localName, String namespaceURI)
			{
				if(localName == null)
				{
					localName = String.Empty;
				}
				if(namespaceURI == null)
				{
					namespaceURI = String.Empty;
				}
				int posn, count;
				XmlNode node;
				count = map.Count;
				for(posn = 0; posn < count; ++posn)
				{
					node = (XmlNode)(map[posn]);
					if(node.LocalName == localName &&
					   node.NamespaceURI == namespaceURI)
					{
						map.RemoveAt(posn);
						return node;
					}
				}
				return null;
			}

	// Set or append an item into this map.
	internal XmlNode SetOrAppend(XmlNode node, bool append)
			{
				XmlNode oldNode;
				String name;
				int posn, count;
				if(node == null)
				{
					return null;
				}
				if(node.OwnerDocument != parent.OwnerDocument)
				{
					throw new ArgumentException
						(S._("Xml_NotSameDocument"), "node");
				}
				if(parent.IsReadOnly)
				{
					throw new ArgumentException(S._("Xml_ReadOnly"));
				}
				count = map.Count;
				name = node.Name;
				for(posn = 0; posn < count; ++posn)
				{
					oldNode = (XmlNode)(map[posn]);
					if(oldNode.Name == name)
					{
						if(append && posn < (count - 1))
						{
							map.RemoveAt(posn);
							map.Add(node);
						}
						else
						{
							map[posn] = node;
						}
						return oldNode;
					}
				}
				map.Add(node);
				return null;
			}

	// Set an item into this map.
	public virtual XmlNode SetNamedItem(XmlNode node)
			{
				return SetOrAppend(node, false);
			}

}; // class XmlNamedNodeMap

}; // namespace System.Xml
