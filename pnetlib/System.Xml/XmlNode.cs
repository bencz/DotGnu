/*
 * XmlNode.cs - Implementation of the "System.Xml.XmlNode" class.
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
using System.Collections;
using System.Text;
using System.Xml.Private;

#if CONFIG_XPATH
using System.Xml.XPath;
#endif

#if ECMA_COMPAT
internal
#else
public
#endif
abstract class XmlNode : ICloneable, IEnumerable
#if CONFIG_XPATH
, IXPathNavigable
#endif
{
	// Internal state.
	internal XmlNode  parent;
	internal NodeList list;

	// Constructor.  Only accessible to internal subclasses.
	internal XmlNode(XmlNode parent)
			{
				this.parent = parent;
				this.list = null;		// Created on demand to save memory.
			}

	// Get a collection that contains all of the attributes for this node.
	public virtual XmlAttributeCollection Attributes
			{
				get
				{
					return null;
				}
			}

	// Get an internal attribute collection.  This will also work on
	// nodes such as "XmlDeclaration" that aren't normally specified
	// to have an attribute collection like "XmlElement" nodes.
	internal virtual XmlAttributeCollection AttributesInternal
			{
				get
				{
					return Attributes;
				}
			}

	// Get the base URI for this document.
	public virtual String BaseURI
			{
				get
				{
					if(parent != null)
					{
						return parent.BaseURI;
					}
					else
					{
						return String.Empty;
					}
				}
			}

	// Get the children of this node.
	public virtual XmlNodeList ChildNodes
			{
				get
				{
					return NodeList.GetList(this);
				}
			}

	// Get the first child of this node.
	public virtual XmlNode FirstChild
			{
				get
				{
					return NodeList.GetFirstChild(this);
				}
			}

	// Determine if this node has child nodes.
	public virtual bool HasChildNodes
			{
				get
				{
					return (NodeList.GetFirstChild(this) != null);
				}
			}

	// Collect the inner text versions of a node and all of its children.
	private void CollectInner(StringBuilder builder)
			{
				XmlNode current = NodeList.GetFirstChild(this);
				while(current != null)
				{
					if(NodeList.GetFirstChild(current) == null)
					{
						switch(current.NodeType)
						{
							case XmlNodeType.Text:
							case XmlNodeType.CDATA:
							case XmlNodeType.SignificantWhitespace:
							case XmlNodeType.Whitespace:
							{
								builder.Append(current.InnerText);
							}
							break;

							default: break;
						}
					}
					else
					{
						current.CollectInner(builder);
					}
					current = NodeList.GetNextSibling(current);
				}
			}

	// Get the inner text version of this node.
	public virtual String InnerText
			{
				get
				{
					XmlNode child = NodeList.GetFirstChild(this);
					XmlNode next;
					if(child == null)
					{
						return String.Empty;
					}
					next = NodeList.GetNextSibling(child);
					if(next == null)
					{
						// Special-case the case of a single text child.
						switch(child.NodeType)
						{
							case XmlNodeType.Text:
							case XmlNodeType.CDATA:
							case XmlNodeType.SignificantWhitespace:
							case XmlNodeType.Whitespace:
							{
								return child.Value;
							}
							// Not reached

							default: break;
						}
					}
					StringBuilder builder = new StringBuilder();
					CollectInner(builder);
					return builder.ToString();
				}
				set
				{
					XmlNode child = NodeList.GetFirstChild(this);
					if(child != null && NodeList.GetNextSibling(child) == null)
					{
						// Special-case the case of a single text child.
						switch(child.NodeType)
						{
							case XmlNodeType.Text:
							case XmlNodeType.CDATA:
							case XmlNodeType.SignificantWhitespace:
							case XmlNodeType.Whitespace:
							{
								child.Value = value;
								return;
							}
							// Not reached

							default: break;
						}
					}
					RemoveAll();
					AppendChild(OwnerDocument.CreateTextNode(value));
				}
			}

	// Get the markup that represents the children of this node.
	public virtual String InnerXml
			{
				get
				{
					XmlFragmentTextWriter writer = new XmlFragmentTextWriter();
					WriteContentTo(writer);
					return writer.ToString();
				}
				set
				{
					throw new InvalidOperationException
						(S._("Xml_CannotSetInnerXml"));
				}
			}

	// Determine if this node is read-only.
	public virtual bool IsReadOnly
			{
				get
				{
					if(parent != null)
					{
						return parent.IsReadOnly;
					}
					else
					{
						return false;
					}
				}
			}

	// Get the first child element with a specified name.
	public virtual XmlElement this[String name]
			{
				get
				{
					if(list == null)
					{
						return null;
					}
					foreach(XmlNode node in list)
					{
						if(node is XmlElement)
						{
							if(node.Name == name)
							{
								return (XmlElement)node;
							}
						}
					}
					return null;
				}
			}

	// Get the first child element with a specified name and namespace.
	public virtual XmlElement this[String localname, String ns]
			{
				get
				{
					if(list == null)
					{
						return null;
					}
					foreach(XmlNode node in list)
					{
						if(node is XmlElement)
						{
							if(node.LocalName == localname &&
							   node.NamespaceURI == ns)
							{
								return (XmlElement)node;
							}
						}
					}
					return null;
				}
			}

	// Get the last child of this node.
	public virtual XmlNode LastChild
			{
				get
				{
					return NodeList.GetLastChild(this);
				}
			}

	// Get the local name associated with this node.
	public abstract String LocalName { get; }

	// Get the name associated with this node.
	public abstract String Name { get; }

	// Get the namespace URI associated with this node.
	public virtual String NamespaceURI
			{
				get
				{
					return String.Empty;
				}
			}

	// Get the next node immediately following this one.
	public virtual XmlNode NextSibling
			{
				get
				{
					return null;
				}
			}

	// Get the type that is associated with this node.
	public abstract XmlNodeType NodeType { get; }

	// Get the XML markup representing this node and all of its children.
	public virtual String OuterXml
			{
				get
				{
					XmlFragmentTextWriter writer = new XmlFragmentTextWriter();
					WriteTo(writer);
					return writer.ToString();
				}
			}

	// Get the document that owns this node.
	public virtual XmlDocument OwnerDocument
			{
				get
				{
					if(parent != null)
					{
						if(parent is XmlDocument)
						{
							return (XmlDocument)parent;
						}
						else
						{
							return parent.OwnerDocument;
						}
					}
					else
					{
						return null;
					}
				}
			}

	// Get the parent of this node.
	public virtual XmlNode ParentNode
			{
				get
				{
					if(parent != null && !parent.IsPlaceholder)
					{
						return parent;
					}
					else
					{
						return null;
					}
				}
			}

	// Get the prefix associated with this node.
	public virtual String Prefix
			{
				get
				{
					return String.Empty;
				}
				set
				{
					throw new NotSupportedException("Prefix");
				}
			}

	// Get the previous sibling of this node.
	public virtual XmlNode PreviousSibling
			{
				get
				{
					return null;
				}
			}

	// Get or set the value associated with this node.
	public virtual String Value
			{
				get
				{
					return String.Empty;
				}
				set
				{
					throw new InvalidOperationException
						(S._("Xml_CannotSetValue"));
				}
			}

	// Determine if one node is an ancestor of another.
	private static bool IsAncestorOf(XmlNode node1, XmlNode node2)
			{
				while(node2 != null)
				{
					if(node2 == node1)
					{
						return true;
					}
					node2 = node2.parent;
				}
				return false;
			}

	// Append a new child to this node.
	public virtual XmlNode AppendChild(XmlNode newChild)
			{
				XmlDocument doc;
				XmlNode parentNode;

				// Validate the parameters.
				if(!CanInsertAfter(newChild.NodeType, LastChild))
				{
					throw new InvalidOperationException
						(S._("Xml_CannotInsert"));
				}
				if(IsAncestorOf(newChild, this))
				{
					throw new InvalidOperationException(S._("Xml_IsAncestor"));
				}
				if(this is XmlDocument)
				{
					doc = (XmlDocument)this;
				}
				else
				{
					doc = OwnerDocument;
				}
				if(newChild.OwnerDocument != doc)
				{
					throw new ArgumentException
						(S._("Xml_NotSameDocument"), "newChild");
				}
				if(IsReadOnly)
				{
					throw new ArgumentException(S._("Xml_ReadOnly"));
				}

				// Remove the child from underneath its current parent.
				parentNode = newChild.ParentNode;
				if(parentNode != null)
				{
					parentNode.RemoveChild(newChild);
				}

				// If the node is a document fragment, then add its
				// children instead of the node itself.
				if(newChild.NodeType == XmlNodeType.DocumentFragment)
				{
					XmlNode firstChild = NodeList.GetFirstChild(newChild);
					XmlNode current, next;
					current = firstChild;
					while(current != null)
					{
						next = NodeList.GetNextSibling(current);
						newChild.RemoveChild(current);
						AppendChild(current);
						current = next;
					}
					return firstChild;
				}

				// Notify the document that we are about to do an insert.
				XmlNodeChangedEventArgs args;
				args = EmitBefore(XmlNodeChangedAction.Insert,
								  parentNode, this);

				// Perform the insert.
				newChild.parent = this;
				NodeList.GetList(this).InsertAfter(newChild, LastChild);

				// Notify the document after the insert.
				EmitAfter(args);

				// The child has been inserted into its new position.
				return newChild;
			}

	// Implement the ICloneable interface.
	Object ICloneable.Clone()
			{
				return CloneNode(true);
			}

	// Clone this node.
	public virtual XmlNode Clone()
			{
				return CloneNode(true);
			}

	// Clone this node in either shallow or deep mode.
	public abstract XmlNode CloneNode(bool deep);

#if CONFIG_XPATH
	// Implement the IXPathNavigator interface.
	public virtual XPathNavigator CreateNavigator()
			{
				return new XmlDocumentNavigator(this);
			}
#endif // CONFIG_XPATH

	// Implement the IEnumerable interface,
	public IEnumerator GetEnumerator()
			{
				return (NodeList.GetList(this)).GetEnumerator();
			}

	// Get the namespace that corresponds to a particular prefix.
	public virtual String GetNamespaceOfPrefix(String prefix)
			{
				// Bail out if no prefix supplied.
				if(prefix == null)
				{
					return String.Empty;
				}

				// Find the document that owns this node.
				XmlDocument doc = FindOwnerQuick();
				if(doc == null)
				{
					return String.Empty;
				}

				// Look up the prefix in the name table.  If it isn't
				// present in the table, then it won't be present in
				// the element node tree either.
				prefix = doc.NameTable.Get(prefix);
				if(prefix == null)
				{
					return String.Empty;
				}

				// Search for an element with this prefix.
				XmlNode node = this;
				XmlElement element;
				XmlAttributeCollection attributes;
				XmlAttribute attr;
				int posn, count;
				do
				{
					if(node.NodeType == XmlNodeType.Element)
					{
						// If the node's name includes the prefix,
						// then return the namespace for the node.
						if(((Object)(node.Prefix)) == (Object)prefix)
						{
							return node.NamespaceURI;
						}

						// Is there an explicit "xmlns:prefix" declaration?
						element = (XmlElement)node;
						attributes = element.Attributes;
						count = attributes.Count;
						for(posn = 0; posn < count; ++posn)
						{
							attr = attributes[posn];
							if(((Object)(attr.LocalName)) == (Object)prefix &&
							   attr.Prefix == "xmlns")
							{
								return attr.Value;
							}
						}
					}
					node = node.ParentNode;
				}
				while(node != null);
				return String.Empty;
			}

	// Get the prefix that corresponds to a particular namespace.
	public virtual String GetPrefixOfNamespace(String namespaceURI)
			{
				// Find the document that owns this node.
				XmlDocument doc = FindOwnerQuick();
				if(doc == null)
				{
					return String.Empty;
				}

				// Handle the builtin "xmlns" namespace.
				if(namespaceURI == XmlDocument.xmlns)
				{
					return "xmlns";
				}

				// Look up the namespace in the name table.
				namespaceURI = doc.NameTable.Add(namespaceURI);

				// Search for an element with this namespace.
				XmlNode node = this;
				XmlElement element;
				XmlAttributeCollection attributes;
				XmlAttribute attr;
				int posn, count;
				do
				{
					if(node.NodeType == XmlNodeType.Element)
					{
						// If the node's name includes the namespace,
						// then return the prefix for the node.
						if(((Object)(node.NamespaceURI)) ==
								(Object)namespaceURI)
						{
							return node.Prefix;
						}

						// Is there an explicit "xmlns:prefix" declaration?
						element = (XmlElement)node;
						attributes = element.Attributes;
						count = attributes.Count;
						for(posn = 0; posn < count; ++posn)
						{
							attr = attributes[posn];
							if(attr.Prefix == "xmlns" &&
							   attr.Value == namespaceURI)
							{
								return attr.LocalName;
							}
						}
					}
					node = node.ParentNode;
				}
				while(node != null);
				return String.Empty;
			}

	// Insert a new child under this node just after a reference child.
	public virtual XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
			{
				XmlDocument doc;
				XmlNode parentNode;

				// Validate the parameters.
				if(refChild == null)
				{
					return PrependChild(newChild);
				}
				if(IsAncestorOf(newChild, this))
				{
					throw new InvalidOperationException(S._("Xml_IsAncestor"));
				}
				if(refChild.ParentNode != this)
				{
					throw new ArgumentException
						(S._("Xml_RefNotChild"), "refChild");
				}
				if(!CanInsertAfter(newChild.NodeType, refChild))
				{
					throw new InvalidOperationException
						(S._("Xml_CannotInsert"));
				}
				if(this is XmlDocument)
				{
					doc = (XmlDocument)this;
				}
				else
				{
					doc = OwnerDocument;
				}
				if(newChild.OwnerDocument != doc)
				{
					throw new ArgumentException
						(S._("Xml_NotSameDocument"), "newChild");
				}
				if(IsReadOnly)
				{
					throw new ArgumentException(S._("Xml_ReadOnly"));
				}

				// If the two nodes are identical, then bail out.
				if(newChild == refChild)
				{
					return newChild;
				}

				// Remove the child from underneath its current parent.
				parentNode = newChild.ParentNode;
				if(parentNode != null)
				{
					parentNode.RemoveChild(newChild);
				}

				// If the node is a document fragment, then add its
				// children instead of the node itself.
				if(newChild.NodeType == XmlNodeType.DocumentFragment)
				{
					XmlNode firstChild = NodeList.GetFirstChild(newChild);
					XmlNode current, next;
					current = firstChild;
					while(current != null)
					{
						next = NodeList.GetNextSibling(current);
						newChild.RemoveChild(current);
						refChild = InsertAfter(current, refChild);
						current = next;
					}
					return firstChild;
				}

				// Notify the document that we are about to do an insert.
				XmlNodeChangedEventArgs args;
				args = EmitBefore(XmlNodeChangedAction.Insert,
								  parentNode, this);

				// Perform the insert.
				newChild.parent = this;
				NodeList.GetList(this).InsertAfter(newChild, refChild);

				// Notify the document after the insert.
				EmitAfter(args);

				// The child has been inserted into its new position.
				return newChild;
			}

	// Insert a new child under this node just before a reference child.
	public virtual XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
			{
				XmlDocument doc;
				XmlNode parentNode;

				// Validate the parameters.
				if(refChild == null)
				{
					return AppendChild(newChild);
				}
				if(IsAncestorOf(newChild, this))
				{
					throw new InvalidOperationException(S._("Xml_IsAncestor"));
				}
				if(refChild.ParentNode != this)
				{
					throw new ArgumentException
						(S._("Xml_RefNotChild"), "refChild");
				}
				if(!CanInsertBefore(newChild.NodeType, refChild))
				{
					throw new InvalidOperationException
						(S._("Xml_CannotInsert"));
				}
				if(this is XmlDocument)
				{
					doc = (XmlDocument)this;
				}
				else
				{
					doc = OwnerDocument;
				}
				if(newChild.OwnerDocument != doc)
				{
					throw new ArgumentException
						(S._("Xml_NotSameDocument"), "newChild");
				}
				if(IsReadOnly)
				{
					throw new ArgumentException(S._("Xml_ReadOnly"));
				}

				// If the two nodes are identical, then bail out.
				if(newChild == refChild)
				{
					return newChild;
				}

				// Remove the child from underneath its current parent.
				parentNode = newChild.ParentNode;
				if(parentNode != null)
				{
					parentNode.RemoveChild(newChild);
				}

				// If the node is a document fragment, then add its
				// children instead of the node itself.
				if(newChild.NodeType == XmlNodeType.DocumentFragment)
				{
					XmlNode firstChild = NodeList.GetFirstChild(newChild);
					XmlNode current, next;
					current = firstChild;
					while(current != null)
					{
						next = NodeList.GetNextSibling(current);
						newChild.RemoveChild(current);
						refChild = InsertBefore(current, refChild);
						current = next;
					}
					return firstChild;
				}

				// Notify the document that we are about to do an insert.
				XmlNodeChangedEventArgs args;
				args = EmitBefore(XmlNodeChangedAction.Insert,
								  parentNode, this);

				// Perform the insert.
				newChild.parent = this;
				if( refChild == this.FirstChild ) {
					NodeList.GetList(this).first = newChild;
					NodeList.GetList(newChild).nextSibling = refChild;
					NodeList.GetList(refChild).prevSibling = newChild;
				}
				else {
					refChild = NodeList.GetPreviousSibling(refChild);
					NodeList.GetList(this).InsertAfter(newChild, refChild);
				}

				// Notify the document after the insert.
				EmitAfter(args);

				// The child has been inserted into its new position.
				return newChild;
			}

	// Normalize the text nodes underneath this node.
	public virtual void Normalize()
			{
				XmlNode current = NodeList.GetFirstChild(this);
				XmlNode next, setNode;
				StringBuilder builder = null;
				setNode = null;
				while(current != null)
				{
					next = NodeList.GetNextSibling(this);
					switch(current.NodeType)
					{
						case XmlNodeType.Text:
						case XmlNodeType.Whitespace:
						case XmlNodeType.SignificantWhitespace:
						{
							if(setNode != null)
							{
								builder.Append(current.Value);
								RemoveChild(current);
							}
							else
							{
								setNode = current;
								builder = new StringBuilder(current.Value);
							}
						}
						break;

						case XmlNodeType.Element:
						{
							if(setNode != null)
							{
								setNode.Value = builder.ToString();
								setNode = null;
								builder = null;
							}
							current.Normalize();
						}
						break;

						default:
						{
							if(setNode != null)
							{
								setNode.Value = builder.ToString();
								setNode = null;
								builder = null;
							}
						}
						break;
					}
					current = next;
				}
				if(setNode != null)
				{
					setNode.Value = builder.ToString();
				}
			}

	// Prepend a specific child at the start of this node's child list.
	public virtual XmlNode PrependChild(XmlNode newChild)
			{
				XmlDocument doc;
				XmlNode parentNode;

				// Validate the parameters.
				if(!CanInsertBefore(newChild.NodeType, FirstChild))
				{
					throw new InvalidOperationException
						(S._("Xml_CannotInsert"));
				}
				if(IsAncestorOf(newChild, this))
				{
					throw new InvalidOperationException(S._("Xml_IsAncestor"));
				}
				if(this is XmlDocument)
				{
					doc = (XmlDocument)this;
				}
				else
				{
					doc = OwnerDocument;
				}
				if(newChild.OwnerDocument != doc)
				{
					throw new ArgumentException
						(S._("Xml_NotSameDocument"), "newChild");
				}
				if(IsReadOnly)
				{
					throw new ArgumentException(S._("Xml_ReadOnly"));
				}

				// Remove the child from underneath its current parent.
				parentNode = newChild.ParentNode;
				if(parentNode != null)
				{
					parentNode.RemoveChild(newChild);
				}

				// If the node is a document fragment, then add its
				// children instead of the node itself.
				if(newChild.NodeType == XmlNodeType.DocumentFragment)
				{
					XmlNode lastChild = NodeList.GetLastChild(newChild);
					XmlNode current, next;
					current = lastChild;
					while(current != null)
					{
						next = NodeList.GetPreviousSibling(current);
						newChild.RemoveChild(current);
						PrependChild(current);
						current = next;
					}
					return lastChild;
				}

				// Notify the document that we are about to do an insert.
				XmlNodeChangedEventArgs args;
				args = EmitBefore(XmlNodeChangedAction.Insert,
								  parentNode, this);

				// Perform the insert.
				newChild.parent = this;
				NodeList.GetList(this).InsertAfter(newChild, null);

				// Notify the document after the insert.
				EmitAfter(args);

				// The child has been inserted into its new position.
				return newChild;
			}

	// Remove all children and attributes from this node.
	public virtual void RemoveAll()
			{
				XmlNode current = FirstChild;
				XmlNode next;
				while(current != null)
				{
					next = NodeList.GetNextSibling(current);
					RemoveChild(current);
					current = next;
				}
			}

	// Remove a child from this node.
	public virtual XmlNode RemoveChild(XmlNode oldChild)
			{
				XmlDocument doc;

				// Validate the parameters.
				if(oldChild.ParentNode != this)
				{
					throw new ArgumentException
						(S._("Xml_CannotRemove"), "oldChild");
				}

				// Notify the document that we are about to do a remove.
				XmlNodeChangedEventArgs args;
				args = EmitBefore(XmlNodeChangedAction.Remove, this, null);

				// Remove the child from this node and re-associate
				// it with the placeholder document fragment.
				if(this is XmlDocument)
				{
					doc = (XmlDocument)this;
				}
				else
				{
					doc = OwnerDocument;
				}
				oldChild.parent = doc.Placeholder;
				NodeList.GetList(this).RemoveChild(oldChild);

				// Notify the document after the remove.
				EmitAfter(args);

				// Return the child that was removed.
				return oldChild;
			}

	// Replace a child of this node.
	public virtual XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
			{
				XmlNode next = oldChild.NextSibling;
				RemoveChild(oldChild);
				InsertBefore(newChild, next);
				return oldChild;
			}

#if CONFIG_XPATH

	// Select a list of nodes matching a particular XPath expression.
	public XmlNodeList SelectNodes(String xpath)
			{
				return SelectNodes(xpath, null);
			}

	// Select a list of nodes matching a particular XPath expression.
	public XmlNodeList SelectNodes(String xpath, XmlNamespaceManager nsmgr)
			{
				XPathNavigator nav = CreateNavigator();
				XPathExpression expr = nav.Compile(xpath);
				if(nsmgr != null)
				{
					expr.SetContext(nsmgr);
				}
				return new SelectNodeList(nav.Select(expr));
			}

	// Select a single node matching a particular XPath expression.
	public XmlNode SelectSingleNode(String xpath)
			{
				return SelectSingleNode(xpath, null);
			}

	// Select a single node matching a particular XPath expression.
	public XmlNode SelectSingleNode(String xpath, XmlNamespaceManager nsmgr)
			{
				XPathNavigator nav = CreateNavigator();
				XPathExpression expr = nav.Compile(xpath);
				if(nsmgr != null)
				{
					expr.SetContext(nsmgr);
				}
				SelectNodeList list = new SelectNodeList(nav.Select(expr));
				return (list.Count == 0 ? null : list[0]);
			}

#endif // CONFIG_XPATH

	// Test if this implementation supports a particular DOM feature.
	public virtual bool Supports(String feature, String version)
			{
				if(feature == "XML" &&
				   (version == "1.0" || version == "2.0"))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Writes the contents of this node to a specified XmlWriter.
	public abstract void WriteContentTo(XmlWriter w);

	// Write this node and all of its contents to a specified XmlWriter.
	public abstract void WriteTo(XmlWriter w);

	// Write the children of this node to a specified XmlWriter.
	internal void WriteChildrenTo(XmlWriter w)
			{
				XmlNode child = NodeList.GetFirstChild(this);
				while(child != null)
				{
					child.WriteTo(w);
					child = NodeList.GetNextSibling(child);
				}
			}

	// Clone the children from another node into this node.
	internal void CloneChildrenFrom(XmlNode other, bool deep)
			{
				XmlNode child = NodeList.GetFirstChild(other);
				while(child != null)
				{
					AppendChild(child.CloneNode(deep));
					child = NodeList.GetNextSibling(child);
				}
			}

	// Determine if this node is a placeholder fragment for nodes that have
	// not yet been added to the main document.
	internal virtual bool IsPlaceholder
			{
				get
				{
					return false;
				}
			}

	// Quickly find the document that owns a node, without recursion.
	internal XmlDocument FindOwnerQuick()
			{
				XmlNode node = this;
				while(node != null)
				{
					if(node is XmlDocument)
					{
						return (XmlDocument)node;
					}
					node = node.parent;
				}
				return null;
			}

	// Quickly collect namespace, xml:lang, and xml:space information.
	internal void CollectAncestorInformation
				(ref XmlNamespaceManager ns, out String lang,
				 out XmlSpace space)
			{
				// get the first element up the chain
				XmlElement elem;
				XmlNodeType ntype = NodeType;
				if(ntype == XmlNodeType.Attribute)
				{
					elem = ((XmlAttribute)this).OwnerElement;
				}
				else if(ntype == XmlNodeType.Element)
				{
					elem = (XmlElement)this;
				}
				else
				{
					elem = (XmlElement)ParentNode;
				}

				// set up for reading the attributes
				lang = null;
				space = XmlSpace.None;
				bool sawLang = false;
				bool sawSpace = false;
				Object langQuick = ns.NameTable.Add("lang");
				Object spaceQuick = ns.NameTable.Add("space");
				Object xmlQuick = ns.NameTable.Add("xml");
				Object xmlnsQuick = ns.NameTable.Add("xmlns");

				// read elements and their attributes until we hit the top
				for(; elem != null; elem = (elem.parent as XmlElement))
				{
					foreach(XmlAttribute att in elem.Attributes)
					{
						if(((Object)att.Name) == xmlnsQuick)
						{
							if(ns.LookupNamespace(String.Empty) == null)
							{
								ns.AddNamespace(String.Empty, att.Value);
							}
						}
						else if(((Object)att.Prefix) == xmlnsQuick)
						{
							if(ns.LookupNamespace(att.LocalName) == null)
							{
								ns.AddNamespace(att.LocalName, att.Value);
							}
						}
						else if(((Object)att.Prefix) == xmlQuick)
						{
							if(!sawLang)
							{
								if(((Object)att.LocalName) == langQuick)
								{
									lang = att.Value;
									sawLang = true;
									continue;
								}
							}
							if(!sawSpace)
							{
								if(((Object)att.LocalName) == spaceQuick)
								{
									if(att.Value == "preserve")
									{
										space = XmlSpace.Preserve;
									}
									else
									{
										space = XmlSpace.Default;
									}
									sawSpace = true;
									continue;
								}
							}
						}
					}
				}
			}

	// Emit an event just before a change.  Returns an argument
	// block if an after event will also need to be emitted.
	internal XmlNodeChangedEventArgs EmitBefore
				(XmlNodeChangedAction action,
			     XmlNode oldParent, XmlNode newParent)
			{
				XmlDocument doc = FindOwnerQuick();
				if(doc != null)
				{
					return doc.EmitBefore(action, this, oldParent, newParent);
				}
				else
				{
					return null;
				}
			}
	internal XmlNodeChangedEventArgs EmitBefore(XmlNodeChangedAction action)
			{
				XmlDocument doc = FindOwnerQuick();
				if(doc != null)
				{
					XmlNode parent = ParentNode;
					return doc.EmitBefore(action, this, parent, parent);
				}
				else
				{
					return null;
				}
			}

	// Emit an event just after a change.
	internal void EmitAfter(XmlNodeChangedEventArgs args)
			{
				if(args != null)
				{
					FindOwnerQuick().EmitAfter(args);
				}
			}

	// Get and set special attributes on this node.
	internal virtual String GetSpecialAttribute(String name)
			{
				// Nothing to do here.
				return String.Empty;
			}
	internal virtual void SetSpecialAttribute(String name, String value)
			{
				// Nothing to do here.
			}

	// Determine if a particular node type can be inserted as
	// a child of the current node.
	internal virtual bool CanInsert(XmlNodeType type)
			{
				return false;
			}

	// Determine if a particular node type can be inserted after another,
	// which may be null if the list is currently empty.
	internal virtual bool CanInsertAfter(XmlNodeType type, XmlNode refNode)
			{
				return CanInsert(type);
			}

	// Determine if a particular node type can be inserted before another,
	// which may be null if the list is currently empty.
	internal virtual bool CanInsertBefore(XmlNodeType type, XmlNode refNode)
			{
				return CanInsert(type);
			}

}; // class XmlNode

}; // namespace System.Xml
