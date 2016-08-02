/*
 * XmlElement.cs - Implementation of the "System.Xml.XmlElement" class.
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
using System.Xml.Private;

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlElement : XmlLinkedNode
{
	// Internal state.
	private NameCache.NameInfo name;
	private XmlAttributeCollection attributes;
	private bool isEmpty;

	// Constructor.
	internal XmlElement(XmlNode parent, NameCache.NameInfo name)
			: base(parent)
			{
				this.name = name;
				this.attributes = null;
				this.isEmpty = true;
			}
	protected internal XmlElement(String prefix, String localName,
								  String namespaceURI, XmlDocument doc)
			: base(doc)
			{
				this.name = doc.nameCache.Add(localName, prefix, namespaceURI);
			}

	// Get a collection that contains all of the attributes for this node.
	public override XmlAttributeCollection Attributes
			{
				get
				{
					if(attributes == null)
					{
						attributes = new XmlAttributeCollection(this);
					}
					return attributes;
				}
			}

	// Determine if this element node has attributes.
	public virtual bool HasAttributes
			{
				get
				{
					return (attributes != null && attributes.Count != 0);
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
					XmlNode child = NodeList.GetFirstChild(this);
					if(child != null &&
					   NodeList.GetNextSibling(child) == null &&
					   child.NodeType == XmlNodeType.Text)
					{
						// Special-case the case of a single text child.
						child.Value = value;
					}
					else
					{
						// Remove the children and create a new text node.
						RemoveContents();
						AppendChild(OwnerDocument.CreateTextNode(value));
						IsEmpty=false;
					}
				}
			}

	// Get the markup that represents the children of this node.
	public override String InnerXml
			{
				get { return base.InnerXml; }
				set
				{
					// remove the children from this element
					RemoveContents();

					// bail out now if there's nothing to parse
					if(value == null || value.Length == 0) { return; }

					// get the owner document
					XmlDocument doc = FindOwnerQuick();

					// get the name table
					XmlNameTable nt = doc.NameTable;

					// declare the lang and space
					String lang; XmlSpace space;

					// create the namespace manager
					XmlNamespaceManager nm = new XmlNamespaceManager(nt);

					// collect all the ancestor information needed for parsing
					CollectAncestorInformation(ref nm, out lang, out space);

					// create the parser context
					XmlParserContext context = new XmlParserContext
						(nt, nm, lang, space);

					// set the base uri in the parser context
					context.BaseURI = BaseURI;

					// create the reader
					XmlTextReader r = new XmlTextReader
						(value, XmlNodeType.Element, context);

					// move to the first node
					r.Read();

					// add the child nodes
					do
					{
						// read the next child node
						XmlNode child = doc.ReadNodeInternal(r);

						// append the child (ReadNodeInternal should
						// advance reader)
						if(child != null)
						{
							// append the new child node
							AppendChild(child);
						}
					}
					while(r.ReadState == ReadState.Interactive);
				}
			}

	// Get or set the empty state of this element.
	public bool IsEmpty
			{
				get
				{
					// only override the flag if there's content
					if(FirstChild != null) { isEmpty = false; }

					// return the empty flag
					return isEmpty;
				}
				set
				{
					if(value)
					{
						RemoveContents();
					}
					isEmpty = value;
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

	// Get the next node immediately following this one.
	public override XmlNode NextSibling
			{
				get
				{
					return NodeList.GetNextSibling(this);
				}
			}

	// Get the type that is associated with this node.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.Element;
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

	// Get the previous node immediately preceding this one.
	public override XmlNode PreviousSibling
			{
				get
				{
					return NodeList.GetPreviousSibling(this);
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

	// Clone this node in either shallow or deep mode.
	public override XmlNode CloneNode(bool deep)
			{
				XmlElement clone = OwnerDocument.CreateElement
					(Prefix, LocalName, NamespaceURI);
				if(attributes != null)
				{
					foreach(XmlAttribute attr in Attributes)
					{
						clone.Attributes.Append
							((XmlAttribute)(attr.CloneNode(true)));
					}
				}
				if(deep)
				{
					clone.CloneChildrenFrom(this, deep);
				}
				return clone;
			}

	// Get the value of an attribute with a specific name.
	public virtual String GetAttribute(String name)
			{
				XmlAttribute attr = GetAttributeNode(name);
				if(attr != null)
				{
					return attr.Value;
				}
				else
				{
					return String.Empty;
				}
			}

	// Get the value of an attribute with a specific name and namespace.
	public virtual String GetAttribute(String localName, String namespaceURI)
			{
				XmlAttribute attr = GetAttributeNode(localName, namespaceURI);
				if(attr != null)
				{
					return attr.Value;
				}
				else
				{
					return String.Empty;
				}
			}

	// Get the node of an attribute with a specific name.
	public virtual XmlAttribute GetAttributeNode(String name)
			{
				if(attributes != null)
				{
					return attributes[name];
				}
				else
				{
					return null;
				}
			}

	// Get the node of an attribute with a specific name and namespace.
	public virtual XmlAttribute GetAttributeNode
				(String localName, String namespaceURI)
			{
				if(attributes != null)
				{
					return attributes[localName, namespaceURI];
				}
				else
				{
					return null;
				}
			}

	// Get a list of all descendents that match a particular name.
	public virtual XmlNodeList GetElementsByTagName(String name)
			{
				name = (FindOwnerQuick()).NameTable.Add(name);
				return new ElementList(this, name);
			}

	// Get a list of all descendents that match a particular name and namespace.
	public virtual XmlNodeList GetElementsByTagName
				(String localName, String namespaceURI)
			{
				XmlNameTable nt = FindOwnerQuick().NameTable;
				localName = nt.Add(localName);
				namespaceURI = nt.Add(namespaceURI);
				return new ElementList(this, localName, namespaceURI);
			}

	// Determine if this element has a particular attribute.
	public virtual bool HasAttribute(String name)
			{
				return (GetAttributeNode(name) != null);
			}

	// Determine if this element has a particular attribute.
	public virtual bool HasAttribute(String localName, String namespaceURI)
			{
				return (GetAttributeNode(localName, namespaceURI) != null);
			}

	// Remove all children and attributes from this node.
	public override void RemoveAll()
			{
				RemoveAllAttributes();
				base.RemoveAll();
			}

	// Remove all of the attributes from this node.
	public virtual void RemoveAllAttributes()
			{
				if(attributes != null)
				{
					attributes.RemoveAll();
				}
			}

	// Remove the element contents, but not the attributes.
	private void RemoveContents()
			{
				base.RemoveAll();
			}

	// Remove a specified attribute by name.
	public virtual void RemoveAttribute(String name)
			{
				if(attributes != null)
				{
					attributes.RemoveNamedItem(name);
				}
			}

	// Remove a specified attribute by name and namespace.
	public virtual void RemoveAttribute(String localName, String namespaceURI)
			{
				if(attributes != null)
				{
					attributes.RemoveNamedItem(localName, namespaceURI);
				}
			}

	// Remove a specified attribute by index.
	public virtual XmlNode RemoveAttributeAt(int i)
			{
				if(attributes != null)
				{
					return attributes.RemoveAt(i);
				}
				else
				{
					return null;
				}
			}

	// Remove a particular attribute node and return the node.
	public virtual XmlAttribute RemoveAttributeNode(XmlAttribute oldAttr)
			{
				if(attributes != null)
				{
					return (XmlAttribute)(attributes.Remove(oldAttr));
				}
				else
				{
					return null;
				}
			}

	// Remove a particular attribute by name and return the node.
	public virtual XmlAttribute RemoveAttributeNode
				(String localName, String namespaceURI)
			{
				if(attributes != null)
				{
					return (XmlAttribute)(attributes.RemoveNamedItem
								(localName, namespaceURI));
				}
				else
				{
					return null;
				}
			}

	// Set an attribute to a specific value.
	public virtual void SetAttribute(String name, String value)
			{
				XmlAttribute attr = GetAttributeNode(name);
				if(attr != null)
				{
					attr.Value = value;
				}
				else
				{
					attr = OwnerDocument.CreateAttribute(name);
					attr.Value = value;
					/* TODO : figure out if this is better done lower down */
					attr.parent = this;
					Attributes.Append(attr);
				}
			}

	// Set an attribute to a specific value.
	public virtual String SetAttribute
				(String localName, String namespaceURI, String value)
			{
				XmlAttribute attr = GetAttributeNode(localName, namespaceURI);
				if(attr != null)
				{
					attr.Value = value;
				}
				else
				{
					attr = OwnerDocument.CreateAttribute
						(localName, namespaceURI);
					attr.Value = value;
					Attributes.Append(attr);
				}
				return attr.Value;
			}

	// Set an attribute by node.
	public virtual XmlAttribute SetAttributeNode(XmlAttribute newAttr)
			{
				if(newAttr.OwnerElement == null)
				{
					/* TODO : figure out if this is better done lower down */
					newAttr.parent = this; 
					return (XmlAttribute)(Attributes.SetNamedItem(newAttr));
				}
				else
				{
					throw new InvalidOperationException
						(S._("Xml_AttrAlreadySet"));
				}
			}

	// Create a new attribute node and return it.
	public virtual XmlAttribute SetAttributeNode
				(String localName, String namespaceURI)
			{
				XmlAttribute attr = GetAttributeNode(localName, namespaceURI);
				if(attr != null)
				{
					attr = OwnerDocument.CreateAttribute
						(localName, namespaceURI);
					/* TODO : figure out if this is better done lower down */
					attr.parent = this;
					Attributes.Append(attr);
				}
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
				w.WriteStartElement(Prefix, LocalName, NamespaceURI);
				if(attributes != null)
				{
					foreach(XmlAttribute attr in attributes)
					{
						attr.WriteTo(w);
					}
				}
				if(!IsEmpty)
				{
					WriteContentTo(w);
					w.WriteFullEndElement();
				}
				else
				{
					w.WriteEndElement();
				}
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

}; // class XmlElement

}; // namespace System.Xml
