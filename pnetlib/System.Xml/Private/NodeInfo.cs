/*
 * NodeInfo.cs - Implementation of the "System.Xml.Private.NodeInfo" class.
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

internal abstract class NodeInfo
{
	// Get the number of attributes.
	public virtual int AttributeCount
			{
				get { return 0; }
			}

	// Get the amount to add to the depth.
	public virtual int DepthOffset
			{
				get { return 0; }
			}

	// Get the empty element flag.
	public virtual bool IsEmptyElement
			{
				get { return false; }
			}

	// Get the quote character.
	public virtual char QuoteChar
			{
				get { return '"'; }
			}

	// Get the local name.
	public virtual String LocalName
			{
				get { return Name; }
			}

	// Get the fully-qualified name.
	public virtual String Name
			{
				get { return String.Empty; }
			}

	// Get the namespace URI.
	public virtual String NamespaceURI
			{
				get { return String.Empty; }
			}

	// Get the namespace prefix.
	public virtual String Prefix
			{
				get { return String.Empty; }
			}

	// Get the text value.
	public virtual String Value
			{
				get { return String.Empty; }
			}

	// Get the type of the current node.
	public virtual XmlNodeType NodeType
			{
				get { return XmlNodeType.None; }
			}

	// Return an attribute with the given index.
	public virtual NodeInfo GetAttribute(int i)
			{
				return null;
			}
	// Return an attribute with the given name and namespace uri.
	public virtual NodeInfo GetAttribute(String localName, String namespaceURI)
			{
				return null;
			}
	// Return an attribute with the given fully-qualified name.
	public virtual NodeInfo GetAttribute(String name)
			{
				return null;
			}

	// Move the position to an attribute with the given index.
	public virtual bool MoveToAttribute(int i)
			{
				return false;
			}
	// Move the position to an attribute with the given name and namespace uri.
	public virtual bool MoveToAttribute(String localName, String namespaceURI)
			{
				return false;
			}
	// Move the position to an attribute with the given fully-qualified name.
	public virtual bool MoveToAttribute(String name)
			{
				return false;
			}

	// Move the position to before the first attribute.
	public virtual bool MoveToElement()
			{
				return false;
			}

	// Move the position to the first attribute.
	public virtual bool MoveToFirstAttribute()
			{
				return MoveToAttribute(0);
			}

	// Move the position to the next attribute.
	public virtual bool MoveToNextAttribute()
			{
				return false;
			}

	// Read the next attribute value in the input stream.
	public virtual bool ReadAttributeValue()
			{
				return false;
			}

}; // class NodeInfo

}; // namespace System.Xml.Private
