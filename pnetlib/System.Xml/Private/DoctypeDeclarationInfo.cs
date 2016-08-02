/*
 * DoctypeDeclarationInfo.cs - Implementation of the
 *		"System.Xml.Private.DoctypeDeclarationInfo" class.
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

internal sealed class DoctypeDeclarationInfo : NodeWithAttributesInfo
{
	// Internal state.
	private String name;
	private String value;


	// Constructor.
	public DoctypeDeclarationInfo()
			: base()
			{
				name = null;
				value = null;
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

	// Get the fully-qualified name.
	public override String Name
			{
				get
				{
					if(index == -1) { return name; }
					return attributes[index].Name;
				}
			}

	// Get the text value.
	public override String Value
			{
				get
				{
					if(index == -1) { return value; }
					return attributes[index].Value;
				}
			}

	// Get the type of the current node.
	public override XmlNodeType NodeType
			{
				get
				{
					if(index == -1) { return XmlNodeType.DocumentType; }
					return XmlNodeType.Attribute;
				}
			}


	// Set the node information.
	public void SetInfo(Attributes attributes, String name, String value)
			{
				base.Reset(attributes);
				this.name = name;
				this.value = value;
			}

}; // class DoctypeDeclarationInfo

}; // namespace System.Xml.Private
