/*
 * NodeList.cs - Implementation of the "System.Xml.Private.NodeList" class.
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
using System.Runtime.CompilerServices;

internal sealed class NodeList : XmlNodeList
{
	// Internal state.
	internal XmlNode first;
	internal XmlNode last;
	internal XmlNode nextSibling;
	internal XmlNode prevSibling;
	internal int count;
	internal int generation;

	// Create a new node list.
	public NodeList()
			{
				first = null;
				last = null;
				nextSibling = null;
				prevSibling = null;
				count = 0;
				generation = 0;
			}

	// Get the number of entries in the node list.
	public override int Count
			{
				get
				{
					return count;
				}
			}

	// Get a particular item within this node list.
	public override XmlNode Item(int i)
			{
				if(i >= 0 && i < count)
				{
					XmlNode child = first;
					while(i > 0)
					{
						child = child.list.nextSibling;
						--i;
					}
					return child;
				}
				else
				{
					return null;
				}
			}

	// Implement the "IEnumerable" interface.
	public override IEnumerator GetEnumerator()
			{
				return new NodeListEnumerator(this);
			}

	// Get the node list for a node, creating it if necessary.
	public static NodeList GetList(XmlNode node)
			{
				if(node.list == null)
				{
					node.list = new NodeList();
				}
				return node.list;
			}

	// Quick access to the first child of a node.
	public static XmlNode GetFirstChild(XmlNode node)
			{
				if(node.list != null)
				{
					return node.list.first;
				}
				else
				{
					return null;
				}
			}

	// Quick access to the last child.
	public static XmlNode GetLastChild(XmlNode node)
			{
				if(node.list != null)
				{
					return node.list.last;
				}
				else
				{
					return null;
				}
			}

	// Quick access to the next sibling.
	public static XmlNode GetNextSibling(XmlNode node)
			{
				if(node.list != null)
				{
					return node.list.nextSibling;
				}
				else
				{
					return null;
				}
			}

	// Quick access to the previous sibling.
	public static XmlNode GetPreviousSibling(XmlNode node)
			{
				if(node.list != null)
				{
					return node.list.prevSibling;
				}
				else
				{
					return null;
				}
			}

	// Insert a child into a node list just after a given node.
	public void InsertAfter(XmlNode newNode, XmlNode refNode)
			{
				if(refNode != null)
				{
					NodeList refList = GetList(refNode);
					NodeList newList = GetList(newNode);
					if(refList.nextSibling != null)
					{
						newList.nextSibling = refList.nextSibling;
						newList.prevSibling = refNode;
						refList.nextSibling = newNode;
					}
					else
					{
						refList.nextSibling = newNode;
						newList.prevSibling = refNode;
						last = newNode;
					}
				}
				else if(first != null)
				{
					GetList(first).prevSibling = newNode;
					GetList(newNode).nextSibling = first;
				}
				else
				{
					first = newNode;
					last = newNode;
				}
				++count;
				++generation;
			}

	// Remove a child from underneath its current parent.
	public void RemoveChild(XmlNode node)
			{
				NodeList nodeList = GetList(node);
				bool changed = false;
				
				if(nodeList.nextSibling != null)
				{
					GetList(nodeList.nextSibling).prevSibling =
							nodeList.prevSibling;
					changed = true;
				}
				else if(last == node)
				{
					last = nodeList.prevSibling;
					changed = true;
				}
				
				if(nodeList.prevSibling != null)
				{
					GetList(nodeList.prevSibling).nextSibling =
							nodeList.nextSibling;
					changed = true;
				}
				else if(first == node)
				{
					first = nodeList.nextSibling;
					changed = true;
				}

				nodeList.nextSibling = null;
				nodeList.prevSibling = null;

				if(changed)
				{
					--count;
					++generation;
				}
			}

	// Implementation of the node list enumerator.
	private sealed class NodeListEnumerator : IEnumerator
	{
		// Internal state.
		private NodeList list;
		private XmlNode current;
		private int generation;
		private bool done;

		// Constructor.
		public NodeListEnumerator(NodeList list)
				{
					this.list = list;
					this.current = null;
					this.generation = list.generation;
					this.done = false;
				}

		// Implement the "IEnumerator" interface.
		public bool MoveNext()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(S._("Invalid_CollectionModified"));
					}
					if(current == null)
					{
						if(done)
						{
							return false;
						}
						current = list.first;
						if(current == null)
						{
							done = true;
							return false;
						}
						else
						{
							return true;
						}
					}
					if(current.list != null)
					{
						current = current.list.nextSibling;
					}
					else
					{
						current = null;
					}
					if(current != null)
					{
						return true;
					}
					else
					{
						done = true;
						return false;
					}
				}
		public void Reset()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(S._("Invalid_CollectionModified"));
					}
					current = null;
					done = false;
				}
		public Object Current
				{
					get
					{
						if(generation != list.generation)
						{
							throw new InvalidOperationException
								(S._("Invalid_CollectionModified"));
						}
						if(current != null)
						{
							return current;
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_BadEnumeratorPosition"));
						}
					}
				}

	}; // class NodeListEnumerator

}; // class NodeList

}; // namespace System.Xml.Private
