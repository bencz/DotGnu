/*
 * XmlAttributeCollection.cs - Implementation of the
 *		"System.Xml.XmlAttributeCollection" class.
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
using System.Runtime.CompilerServices;

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlAttributeCollection : XmlNamedNodeMap, ICollection
{

	// Constructor.
	internal XmlAttributeCollection(XmlNode parent) : base(parent) {}

	// Retrieve items from this attribute collection.
	[IndexerName("ItemOf")]
	public virtual XmlAttribute this[int i]
			{
				get
				{
					return (XmlAttribute)(Item(i));
				}
			}
	[IndexerName("ItemOf")]
	public virtual XmlAttribute this[String name]
			{
				get
				{
					return (XmlAttribute)(GetNamedItem(name));
				}
			}
	[IndexerName("ItemOf")]
	public virtual XmlAttribute this[String name, String ns]
			{
				get
				{
					return (XmlAttribute)(GetNamedItem(name, ns));
				}
			}

	// Append an attribute to this collection.
	public virtual XmlAttribute Append(XmlAttribute node)
			{
				SetOrAppend(node, true);
				return node;
			}

	// Copy the attributes in this collection to an array.
	public void CopyTo(XmlAttribute[] array, int index)
			{
				int count = Count;
				int posn;
				for(posn = 0; posn < count; ++posn)
				{
					array[index++] = (XmlAttribute)(Item(posn));
				}
			}

	// Get the index of a specific node within this collection.
	internal int IndexOf(XmlAttribute refNode)
			{
				int count = Count;
				int posn;
				for(posn = 0; posn < count; ++posn)
				{
					if(Item(posn) == refNode)
					{
						return posn;
					}
				}
				return -1;
			}

	// Get the position of a reference node within this collection.
	private int GetItemPosition(XmlAttribute refNode)
			{
				int posn = IndexOf(refNode);
				if(posn != -1)
				{
					return posn;
				}
				throw new ArgumentException
					(S._("Xml_NotAttrCollectionMember"), "refNode");
			}

	// Insert a node after another one.
	public virtual XmlAttribute InsertAfter
				(XmlAttribute newNode, XmlAttribute refNode)
			{
				if(newNode == null)
				{
					throw new ArgumentException
						(S._("Xml_NotSameDocument"), "node");
				}
				RemoveNamedItem(newNode.Name);
				if(refNode == null)
				{
					map.Insert(0, newNode);
				}
				else
				{
					map.Insert(GetItemPosition(refNode) + 1, newNode);
				}
				return newNode;
			}

	// Insert a node before another one.
	public virtual XmlAttribute InsertBefore
				(XmlAttribute newNode, XmlAttribute refNode)
			{
				if(newNode == null)
				{
					throw new ArgumentException
						(S._("Xml_NotSameDocument"), "node");
				}
				RemoveNamedItem(newNode.Name);
				if(refNode == null)
				{
					map.Insert(Count, newNode);
				}
				else
				{
					map.Insert(GetItemPosition(refNode), newNode);
				}
				return newNode;
			}

	// Prepend a node to this collection.
	public virtual XmlAttribute Prepend(XmlAttribute node)
			{
				return InsertAfter(node, null);
			}

	// Remove an attribute from this collection.
	public virtual XmlAttribute Remove(XmlAttribute node)
			{
				if(node == null)
				{
					return null;
				}
				else
				{
					return (XmlAttribute)(RemoveNamedItem(node.Name));
				}
			}

	// Remove all attributes from this collection.
	public virtual void RemoveAll()
			{
				map.Clear();
			}

	// Remove the attribute at a particular index.
	public virtual XmlAttribute RemoveAt(int i)
			{
				if(i < 0 || i >= map.Count)
				{
					return null;
				}
				XmlAttribute attr = (XmlAttribute)(map[i]);
				map.RemoveAt(i);
				return attr;
			}

	// Set a named item into this node map, after making sure
	// that it is indeed an attribute.
	public override XmlNode SetNamedItem(XmlNode node)
			{
				if(node != null && !(node is XmlAttribute))
				{
					throw new ArgumentException
						(S._("Xml_AttrCollectionWrongNode"), "node");
				}
				return SetOrAppend(node, false);
			}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				int count = Count;
				int posn;
				for(posn = 0; posn < count; ++posn)
				{
					array.SetValue(Item(posn), index);
					++index;
				}
			}
	int ICollection.Count
			{
				get
				{
					return Count;
				}
			}
	bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}
	Object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

}; // class XmlAttributeCollection

}; // namespace System.Xml
