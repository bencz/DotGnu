/*
 * ElementList.cs - Implementation of the
 *		"System.Xml.Private.ElementList" class.
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

	internal class ElementList : XmlNodeList
	{
		// Internal state.
		private XmlNode parent;
		private XmlDocument doc;
		private String name;
		private String namespaceURI;
		private String matchAll;
		private bool uriForm;
		private bool docChanged;
		private XmlNode lastItem;
		private int lastItemAt;

		// Create a new element list.
		private ElementList(XmlNode parent)
		{
			this.parent = parent;
			this.doc = parent.OwnerDocument;
			this.matchAll = doc.NameTable.Add("*");
			this.docChanged = false;
			this.lastItem = null;
			this.lastItemAt = -1;
			this.doc.NodeInserted +=
				new XmlNodeChangedEventHandler(DocumentChanged);
			this.doc.NodeRemoved +=
				new XmlNodeChangedEventHandler(DocumentChanged);
		}
		public ElementList(XmlNode parent, String name)
			: this(parent)
			{
				this.name = doc.NameTable.Add(name);
				this.namespaceURI = null;
				this.uriForm = false;
			}
		public ElementList(XmlNode parent, String localName,
				String namespaceURI)
			: this(parent)
			{
				this.name =
					(localName != null ? doc.NameTable.Add(localName) : null);
				this.namespaceURI =
					(namespaceURI != null
					 ? doc.NameTable.Add(namespaceURI) : null);
				this.uriForm = true;
			}

		// Track changes to the document that may affect the search order.
		private void DocumentChanged(Object sender, XmlNodeChangedEventArgs args)
		{
			docChanged = true;
		}

		// Get the node that follows another in pre-order traversal.
		private XmlNode GetFollowingNode(XmlNode node)
		{
			XmlNode current = node.FirstChild;
			if(current == null)
			{
				// We don't have any children, so look for a next sibling.
				current = node;
				while(current != null && current != parent &&
						current.NextSibling == null)
				{
					current = current.ParentNode;
				}
				if(current != null && current != parent)
				{
					current = current.NextSibling;
				}
			}
			if(current == parent)
			{
				// We've finished the traversal.
				return null;
			}
			else
			{
				// This is the next node in sequence.
				return current;
			}
		}

		// Determine if a node matches the selection criteria.
		private bool NodeMatches(XmlNode node)
		{
			if(node.NodeType != XmlNodeType.Element)
			{
				return false;
			}
			if(!uriForm)
			{
				if(((Object)name) == ((Object)matchAll) ||
						((Object)name) == ((Object)(node.Name)))
				{
					return true;
				}
			}
			else
			{
				if(((Object)name) == ((Object)matchAll) ||
						((Object)name) == ((Object)(node.LocalName)))
				{
					if(((Object)namespaceURI) == ((Object)matchAll) ||
							((Object)namespaceURI) ==
							((Object)(node.NamespaceURI)))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Get the number of entries in the node list.
		public override int Count
		{
			get
			{
				int count = 0;
				XmlNode current = parent;
				while((current = GetFollowingNode(current)) != null)
				{
					if(NodeMatches(current))
					{
						++count;
					}
				}
				return count;
			}
		}

		// Get a particular item within this node list.
		public override XmlNode Item(int i)
		{
			// Checking for i >= Count is stupid here
			// as Count does a full iteration anyway
			XmlNode item = parent;
			int a = -1;
			if(i == 0 && NodeMatches(item))
			{
				return item;
			}
			while((item = GetFollowingNode(item)) != null)
			{
				if(NodeMatches(item))
				{
					a++;
					if(i == a) return item;
				}
			}
			return null;
		}

		// Implement the "IEnumerable" interface.
		public override IEnumerator GetEnumerator()
		{
			return new NodeListEnumerator(this);
		}

		// Tell if document has been modified
		internal bool IsModified
		{
			get
			{
				return docChanged;
			}
		}
		
		// Implementation of the node list enumerator.
		private sealed class NodeListEnumerator : IEnumerator
		{
			// Internal state.
			private ElementList list;
			private XmlNode current;
			private bool isModified;
			private bool done;

			// Constructor.
			public NodeListEnumerator(ElementList list)
			{
				this.list = list;
				this.current = null;
				this.isModified = false;
				this.done = false;
			}

			// Implement the "IEnumerator" interface.
			public bool MoveNext()
			{
				if(isModified != list.IsModified)
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
					current = list.parent;
					if(current == null)
					{
						done = true;
						return false;
					}
					else if(list.NodeMatches(current))
					{
						return true;
					}
				}

				do
				{
					current = list.GetFollowingNode(current);
				}
				while(current != null && list.NodeMatches(current) == false);
				
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
				if(isModified != list.IsModified)
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
					if(isModified != list.IsModified)
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

	}; // class ElementList

}; // namespace System.Xml.Private
