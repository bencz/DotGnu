/*
 * XmlDocumentFragment.cs - Implementation of the
 *		"System.Xml.XmlDocumentFragment" class.
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
class XmlDocumentFragment : XmlNode
{
	// Internal state.
	private XmlDocument document;

	// Constructors.
	protected internal XmlDocumentFragment(XmlDocument doc) : base(doc)
			{
				document = doc;
			}

	// Get the markup that represents the children of this node.
	[TODO]
	public override String InnerXml
			{
				get
				{
					return base.InnerXml;
				}
				set
				{
					// TODO
				}
			}

	// Get the local name of this node.
	public override String LocalName
			{
				get
				{
					return "#document-fragment";
				}
			}

	// Get the name of this node.
	public override String Name
			{
				get
				{
					return "#document-fragment";
				}
			}

	// Get the type of this node.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.DocumentFragment;
				}
			}

	// Get the document that owns this node.
	public override XmlDocument OwnerDocument
			{
				get
				{
					return document;
				}
			}

	// Get the parent of this node.
	public override XmlNode ParentNode
			{
				get
				{
					return null;
				}
			}

	// Determine if this node is a placeholder fragment for nodes that have
	// not yet been added to the main document.
	internal override bool IsPlaceholder
			{
				get
				{
					return (document.placeholder == this);
				}
			}

	// Clone this document fragment node.
	public override XmlNode CloneNode(bool deep)
			{
				XmlDocumentFragment frag =
					document.CreateDocumentFragment();
				frag.CloneChildrenFrom(this, deep);
				return frag;
			}

	// Write the contents of this document to an XML writer.
	public override void WriteContentTo(XmlWriter w)
			{
				WriteTo(w);
			}

	// Write this document to an XML writer.
	public override void WriteTo(XmlWriter w)
			{
				WriteChildrenTo(w);
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

}; // class XmlDocumentFragment

}; // namespace System.Xml
