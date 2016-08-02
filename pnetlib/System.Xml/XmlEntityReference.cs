/*
 * XmlEntityReference.cs - Implementation of the
 *		"System.Xml.XmlEntityReference" class.
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

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlEntityReference : XmlLinkedNode
{
	// Internal state.
	private String name;

	// Constructor.
	internal XmlEntityReference(XmlNode parent, String name)
			: base(parent)
			{
				this.name = name;
			}
	protected internal XmlEntityReference(String name, XmlDocument doc)
			: base(doc)
			{
				this.name = name;
			}

	// Get the base URI for this node.
	public override String BaseURI
			{
				get
				{
					return base.BaseURI;
				}
			}

	// Determine if this entity reference is read-only.
	public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

	// Get the local name associated with this node.
	public override String LocalName
			{
				get
				{
					return name;
				}
			}

	// Get the name associated with this node.
	public override String Name
			{
				get
				{
					return name;
				}
			}

	// Get the type that is associated with this node.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.EntityReference;
				}
			}

	// Get or set the value associated with this node.
	public override String Value
			{
				get
				{
					return null;
				}
				set
				{
					throw new InvalidOperationException(S._("Xml_ReadOnly"));
				}
			}

	// Clone this node in either shallow or deep mode.
	public override XmlNode CloneNode(bool deep)
			{
				return OwnerDocument.CreateEntityReference(name);
			}

	// Writes the contents of this node to a specified XmlWriter.
	public override void WriteContentTo(XmlWriter w)
			{
				foreach(XmlNode node in this)
				{
					node.WriteTo(w);
				}
			}

	// Write this node and all of its contents to a specified XmlWriter.
	public override void WriteTo(XmlWriter w)
			{
				w.WriteEntityRef(name);
			}

	// Determine if a particular node type can be inserted as
	// a child of the current node.
	internal override bool CanInsert(XmlNodeType type)
			{
				switch(type)
				{
					case XmlNodeType.Element:
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
					case XmlNodeType.EntityReference:
					case XmlNodeType.ProcessingInstruction:
					case XmlNodeType.Comment:
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
					{
						return true;
					}
					// Not reached.

					default: break;
				}
				return false;
			}

}; // class XmlEntityReference

}; // namespace System.Xml
