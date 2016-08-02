/*
 * EndElementInfo.cs - Implementation of the
 *		"System.Xml.Private.EndElementInfo" class.
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

internal sealed class EndElementInfo : NodeInfo
{
	// Internal state.
	private String localName;
	private String name;
	private String namespaceURI;
	private String prefix;


	// Constructor.
	public EndElementInfo()
			{
				localName = null;
				name = null;
				namespaceURI = null;
				prefix = null;
			}


	// Get the local name.
	public override String LocalName
			{
				get { return localName; }
			}

	// Get the fully-qualified name.
	public override String Name
			{
				get { return name; }
			}

	// Get the namespace URI.
	public override String NamespaceURI
			{
				get { return namespaceURI; }
			}

	// Get the namespace prefix.
	public override String Prefix
			{
				get { return prefix; }
			}

	// Get the type of the current node.
	public override XmlNodeType NodeType
			{
				get { return XmlNodeType.EndElement; }
			}


	// Set the node information.
	public void SetInfo
				(String localName, String name, String namespaceURI,
				 String prefix)
			{
				this.localName = localName;
				this.name = name;
				this.namespaceURI = namespaceURI;
				this.prefix = prefix;
			}

}; // class EndElementInfo

}; // namespace System.Xml.Private
