/*
 * AttributeInfo.cs - Implementation of the
 *		"System.Xml.Private.AttributeInfo" class.
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

internal sealed class AttributeInfo : IterableNodeInfo
{
	// Internal state.
	private bool useIter;
	private char quoteChar;
	private String localName;
	private String name;
	private String namespaceURI;
	private String prefix;
	private String value;
	private TextInfo iter;


	// Constructor.
	public AttributeInfo()
			{
				useIter = false;
				quoteChar = (char)0;
				localName = null;
				name = null;
				namespaceURI = null;
				prefix = null;
				value = null;
				iter = new TextInfo(4);
			}

	// Get the amount to add to the depth.
	public override int DepthOffset
			{
				get
				{
					if(!useIter) { return 0; }
					return 1;
				}
			}

	// Get the quote character.
	public override char QuoteChar
			{
				get
				{
					if(!useIter) { return quoteChar; }
					return iter.QuoteChar;
				}
			}

	// Get the local name.
	public override String LocalName
			{
				get
				{
					if(!useIter) { return localName; }
					return iter.LocalName;
				}
			}

	// Get the fully-qualified name.
	public override String Name
			{
				get
				{
					if(!useIter) { return name; }
					return iter.Name;
				}
			}

	// Get the namespace URI.
	public override String NamespaceURI
			{
				get
				{
					if(!useIter) { return namespaceURI; }
					return iter.NamespaceURI;
				}
			}

	// Get the namespace prefix.
	public override String Prefix
			{
				get
				{
					if(!useIter) { return prefix; }
					return iter.Prefix;
				}
			}

	// Get the segments array.
	public Segments Segments
			{
				get { return iter.Segments; }
			}

	// Get the text value.
	public override String Value
			{
				get
				{
					if(!useIter) { return value; }
					return iter.Value;
				}
			}

	// Get the type of the current node.
	public override XmlNodeType NodeType
			{
				get
				{
					if(!useIter) { return XmlNodeType.Attribute; }
					return iter.NodeType;
				}
			}


	// Move to the next node in the iteration, returning false on end.
	public override bool Next()
			{
				if(useIter) { return iter.Next(); }
				iter.Reset();
				useIter = true;
				return true;
			}

	// Read the next attribute value in the input stream.
	public override bool ReadAttributeValue()
			{
				return Next();
			}

	// Reset the iteration.
	public override void Reset()
			{
				useIter = false;
			}

	// Update the namespace information.
	public void UpdateNamespaceInfo
				(String localName, String namespaceURI, String prefix)
			{
				this.localName = localName;
				this.namespaceURI = namespaceURI;
				this.prefix = prefix;
			}

	// Set the node information.
	public void SetInfo
				(Segments segments, char quoteChar, String name, String value)
			{
				useIter = false;
				iter.SetInfo(segments);
				this.quoteChar = quoteChar;
				this.localName = name;
				this.name = name;
				this.namespaceURI = String.Empty;
				this.prefix = String.Empty;
				this.value = value;
			}

}; // class AttributeInfo

}; // namespace System.Xml.Private
