/*
 * XmlAttribute.cs - Implementation of the "System.Xml.XmlAttribute" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Xml
{

using System;
using System.Xml.Private;

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlAttribute : XmlNode
{
	// Internal state.
	private NameCache.NameInfo name;
	internal bool isDefault;

	// Constructors.
	internal XmlAttribute(XmlNode parent, NameCache.NameInfo name)
			: base(parent)
			{
				this.name = name;
				this.isDefault = false;
			}
	protected internal XmlAttribute(String prefix, String localName,
									String namespaceURI, XmlDocument doc)
			: base(doc)
			{
				this.name = doc.nameCache.Add(localName, prefix, namespaceURI);
				this.isDefault = false;
			}

	// Get the base URI for this document.
	public override String BaseURI
			{
				get
				{
					return parent.BaseURI;
				}
			}

	// Get the inner text version of this node.
	public override String InnerText
			{
				get
				{
					return base.InnerText;
				}
				set
				{
					base.InnerText = value;
				}
			}

	// Get the inner XML version of this node.
	[TODO]
	public override String InnerXml
			{
				get
				{
					return base.InnerXml;
				}
				set
				{
					// TODO: if this attribute is a special attribute
					//       (xmlns/xmlns:/xml:) then the new value
					//       needs to be reflected in the rest of the
					//       document tree

					// get the owner document
					XmlDocument doc = FindOwnerQuick();
					if(doc == null)
					{
						// TODO: figure out what to do here
						throw new InvalidOperationException();
					}

					// remove all the old children before continuing
					RemoveAll();

					// create the namespace manager
					XmlNamespaceManager ns = new XmlNamespaceManager
						(doc.NameTable);

					// get the ancestor information
					String lang; XmlSpace space;
					CollectAncestorInformation(ref ns, out lang, out space);

					// create the parser context
					XmlParserContext context = new XmlParserContext
						(doc.NameTable, ns, lang, space);

					// create the parser
					XmlTextReader r = new XmlTextReader
						(value, XmlNodeType.Attribute, context);

					// read the new children into this attribute
					doc.ReadChildren(r, this);
				}
			}

	// Get the local name associated with this node.
	public override String LocalName
			{
				get
				{
					return name.localName;
				}
			}

	// Get the name associated with this node.
	public override String Name
			{
				get
				{
					return name.name;
				}
			}

	// Get the namespace URI associated with this node.
	public override String NamespaceURI
			{
				get
				{
					return name.ns;
				}
			}

	// Get the type that is associated with this node.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.Attribute;
				}
			}

	// Get the document that owns this node.
	public override XmlDocument OwnerDocument
			{
				get
				{
					return base.OwnerDocument;
				}
			}

	// Get the element that owns this attribute.
	public virtual XmlElement OwnerElement
			{
				get
				{
					if(parent == null || parent.IsPlaceholder)
					{
						return null;
					}
					else
					{
						return (parent as XmlElement);
					}
				}
			}

	// Get the parent of this node.
	public override XmlNode ParentNode
			{
				get
				{
					return OwnerElement;
				}
			}

	// Get the prefix associated with this node.
	public override String Prefix
			{
				get
				{
					return name.prefix;
				}
			}

	// Determine if the attribute value was explictly specified.
	public virtual bool Specified
			{
				get { return !isDefault; }
			}

	// Get or set the value associated with this node.
	public override String Value
			{
				get
				{
					return InnerText;
				}
				set
				{
					InnerText = value;
				}
			}

	// Clone this node in either shallow or deep mode.
	public override XmlNode CloneNode(bool deep)
			{
				XmlAttribute attr = OwnerDocument.CreateAttribute
					(Prefix, LocalName, NamespaceURI);
				attr.CloneChildrenFrom(this, true);
				return attr;
			}

	// Writes the contents of this node to a specified XmlWriter.
	public override void WriteContentTo(XmlWriter w)
			{
				WriteChildrenTo(w);
			}

	// Write this node and all of its contents to a specified XmlWriter.
	public override void WriteTo(XmlWriter w)
			{
				w.WriteStartAttribute(Prefix, LocalName, NamespaceURI);
				WriteContentTo(w);
				w.WriteEndAttribute();
			}

	// Determine if a particular node type can be inserted as
	// a child of the current node.
	internal override bool CanInsert(XmlNodeType type)
			{
				return (type == XmlNodeType.Text ||
						type == XmlNodeType.EntityReference);
			}

}; // class XmlAttribute

}; // namespace System.Xml
