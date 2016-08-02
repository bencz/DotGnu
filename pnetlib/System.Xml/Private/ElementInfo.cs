/*
 * ElementInfo.cs - Implementation of the
 *		"System.Xml.Private.ElementInfo" class.
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

internal sealed class ElementInfo : NodeWithAttributesInfo
{
	// Internal state.
	private bool empty;
	private String localName;
	private String name;
	private String namespaceURI;
	private String prefix;


	// Constructor.
	public ElementInfo()
			: base()
			{
				empty = false;
				localName = null;
				name = null;
				namespaceURI = null;
				prefix = null;
			}


	// Get the empty element flag.
	public override bool IsEmptyElement
			{
				get
				{
					if(index == -1) { return empty; }
					return false;
				}
			}

	// Get the quote character.
	public override char QuoteChar
			{
				get
				{
					if(index == -1) { return '"'; }
					return attributes[index].QuoteChar;
				}
			}

	// Get the local name.
	public override String LocalName
			{
				get
				{
					if(index == -1) { return localName; }
					return attributes[index].LocalName;
				}
			}

	// Get the fully-qualified name.
	public override String Name
			{
				get
				{
					if(index == -1) { return name; }
					return attributes[index].Name;
				}
			}

	// Get the namespace URI.
	public override String NamespaceURI
			{
				get
				{
					if(index == -1) { return namespaceURI; }
					return attributes[index].NamespaceURI;
				}
			}

	// Get the namespace prefix.
	public override String Prefix
			{
				get
				{
					if(index == -1) { return prefix; }
					return attributes[index].Prefix;
				}
			}

	// Get the text value.
	public override String Value
			{
				get
				{
					if(index == -1) { return String.Empty; }
					return attributes[index].Value;
				}
			}

	// Get the type of the current node.
	public override XmlNodeType NodeType
			{
				get
				{
					if(index == -1) { return XmlNodeType.Element; }
					return attributes[index].NodeType;
				}
			}


	// Set the node information.
	public void SetInfo
				(bool empty, Attributes attributes, String localName,
				 String name, String namespaceURI, String prefix)
			{
				base.Reset(attributes);
				this.empty = empty;
				this.localName = localName;
				this.name = name;
				this.namespaceURI = namespaceURI;
				this.prefix = prefix;
			}

}; // class ElementInfo

}; // namespace System.Xml.Private
