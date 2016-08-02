/*
 * XmlDocument.cs - Implementation of the "System.Xml.XmlDocument" class.
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
using System.IO;
using System.Text;
using System.Xml.Private;
using System.Xml.XPath;

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlDocument : XmlNode
{
	// Internal state.
	private String baseURI;
	private bool preserveWhitespace;
	private XmlResolver xmlResolver;
	internal NameCache nameCache;
	internal XmlDocumentFragment placeholder;
	internal XmlImplementation implementation;
	internal static readonly String xmlns = "http://www.w3.org/2000/xmlns/";
	/* is it me or did they a miss a trailing '/' here ? */
	internal static readonly String xmlnsXml = "http://www.w3.org/XML/1998/namespace";

	// Constructors.
	public XmlDocument() : base(null)
			{
				implementation = new XmlImplementation();
				Initialize();
			}
	protected internal XmlDocument(XmlImplementation imp) : base(null)
			{
				if(imp != null)
				{
					implementation = imp;
				}
				else
				{
					implementation = new XmlImplementation();
				}
				Initialize();
			}
	public XmlDocument(XmlNameTable nt) : base(null)
			{
				if(nt != null)
				{
					implementation = new XmlImplementation(nt);
				}
				else
				{
					implementation = new XmlImplementation();
				}
				Initialize();
			}

	// Initialize the document.
	private void Initialize()
			{
				baseURI = String.Empty;
				preserveWhitespace = false;
				placeholder = new XmlDocumentFragment(this);
				nameCache = new NameCache(implementation.nameTable);
			}

	// Get the base URI for the document.
	public override String BaseURI
			{
				get
				{
					return baseURI;
				}
			}

	// Get a document child by type.
	private XmlNode GetChildByType(XmlNodeType type)
			{
				XmlNode child = NodeList.GetFirstChild(this);
				while(child != null)
				{
					if(child.NodeType == type)
					{
						return child;
					}
					child = NodeList.GetNextSibling(child);
				}
				return null;
			}

	// Get the root element for the document.
	public XmlElement DocumentElement
			{
				get
				{
					return (XmlElement)(GetChildByType(XmlNodeType.Element));
				}
			}

	// Get the document type declaration.
	public virtual XmlDocumentType DocumentType
			{
				get
				{
					return (XmlDocumentType)
						(GetChildByType(XmlNodeType.DocumentType));
				}
			}

	// Get the implementation associated with this document.
	public XmlImplementation Implementation
			{
				get
				{
					return implementation;
				}
			}

	// Get the markup that represents the children of this node.
	public override String InnerXml
			{
				get
				{
					return base.InnerXml;
				}
				set
				{
					LoadXml(value);
				}
			}

	// Determine if this document is read-only.
	public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

	// Get the local name of this node.
	public override String LocalName
			{
				get
				{
					return "#document";
				}
			}

	// Get the name of this node.
	public override String Name
			{
				get
				{
					return "#document";
				}
			}

	// Get the name table associated with this document.
	public XmlNameTable NameTable
			{
				get
				{
					return implementation.nameTable;
				}
			}

	// Get the type of this node.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.Document;
				}
			}

	// Get the document that owns this node.
	public override XmlDocument OwnerDocument
			{
				get
				{
					return null;
				}
			}

	// Get or set the whitespace preservation flag.
	public bool PreserveWhitespace
			{
				get
				{
					return preserveWhitespace;
				}
				set
				{
					preserveWhitespace = value;
				}
			}

	// Set the resolver to use for external resources.
	public virtual XmlResolver XmlResolver
			{
				set
				{
					xmlResolver = value;
				}
			}

	// Clone this document node.
	public override XmlNode CloneNode(bool deep)
			{
				XmlDocument doc = Implementation.CreateDocument();
				doc.baseURI = baseURI;
				if(deep)
				{
					doc.CloneChildrenFrom(this, deep);
				}
				return doc;
			}

	// Create an attribute and associate it with this document.
	public XmlAttribute CreateAttribute(String name)
			{
				int colon = name.LastIndexOf(':');
				if(colon == -1)
				{
					return CreateAttribute(String.Empty, name, String.Empty);
				}
				else
				{
					String prefix = name.Substring(0, colon);
					String localName = name.Substring(colon + 1);
					if(prefix == "xmlns")
					{
						return CreateAttribute(prefix, localName, xmlns);
					}
					else
					{
						return CreateAttribute(prefix, localName, String.Empty);
					}
				}
			}
	public XmlAttribute CreateAttribute(String qualifiedName,
										String namespaceURI)
			{
				int colon = qualifiedName.LastIndexOf(':');
				if(colon == -1)
				{
					return CreateAttribute
						(String.Empty, qualifiedName, namespaceURI);
				}
				else
				{
					String prefix = qualifiedName.Substring(0, colon);
					String localName = qualifiedName.Substring(colon + 1);
					return CreateAttribute(prefix, localName, namespaceURI);
				}
			}
	public virtual XmlAttribute CreateAttribute
				(String prefix, String localName, String namespaceURI)
			{
				if(prefix == "xmlns" && namespaceURI != xmlns)
				{
					throw new ArgumentException
						(S._("Xml_InvalidNamespaceURI"), "namespaceURI");
				}
				NameCache.NameInfo info =
					nameCache.Add(localName, prefix, namespaceURI);
				return new XmlAttribute(placeholder, info);
			}

	// Create a CDATA section.
	public virtual XmlCDataSection CreateCDataSection(String data)
			{
				return new XmlCDataSection(placeholder, data);
			}

	// Create a comment node.
	public virtual XmlComment CreateComment(String text)
			{
				return new XmlComment(placeholder, text);
			}

	// Create a default attribute and associate it with this document.
	protected internal virtual XmlAttribute CreateDefaultAttribute
				(String prefix, String localName, String namespaceURI)
			{
				XmlAttribute att = CreateAttribute
					(prefix, localName, namespaceURI);
				att.isDefault = true;
				return att;
			}

	// Create a document fragment that is attached to this node.
	public virtual XmlDocumentFragment CreateDocumentFragment()
			{
				return new XmlDocumentFragment(this);
			}

	// Create a document type that is attached to this node.
	public virtual XmlDocumentType CreateDocumentType
				(String name, String publicId,
				 String systemId, String internalSubset)
			{
				return new XmlDocumentType
					(placeholder, name, publicId, systemId, internalSubset);
			}

	// Create an element that is attached to this node.
	public XmlElement CreateElement(String name)
			{
				int colon = name.LastIndexOf(':');
				if(colon == -1)
				{
					return CreateElement(String.Empty, name, String.Empty);
				}
				else
				{
					String prefix = name.Substring(0, colon);
					String localName = name.Substring(colon + 1);
					if(prefix == "xmlns")
					{
						return CreateElement(prefix, localName, xmlns);
					}
					else
					{
						return CreateElement(prefix, localName, String.Empty);
					}
				}
			}
	public XmlElement CreateElement(String qualifiedName, String namespaceURI)
			{
				int colon = qualifiedName.LastIndexOf(':');
				if(colon == -1)
				{
					return CreateElement
						(String.Empty, qualifiedName, namespaceURI);
				}
				else
				{
					String prefix = qualifiedName.Substring(0, colon);
					String localName = qualifiedName.Substring(colon + 1);
					return CreateElement(prefix, localName, namespaceURI);
				}
			}
	public virtual XmlElement CreateElement(String prefix,
											String localName,
											String namespaceURI)
			{
				if(prefix == "xmlns" && namespaceURI != xmlns)
				{
					throw new ArgumentException
						(S._("Xml_InvalidNamespaceURI"), "namespaceURI");
				}
				NameCache.NameInfo info =
					nameCache.Add(localName, prefix, namespaceURI);
				return new XmlElement(placeholder, info);
			}

	// Create an entity reference and attach it to this node.
	public virtual XmlEntityReference CreateEntityReference(String name)
			{
				return new XmlEntityReference(placeholder, name);
			}

	// Create a node of a given dynamic type.
	public virtual XmlNode CreateNode(String nodeTypeString,
									  String name,
									  String namespaceURI)
			{
				XmlNodeType type;
				switch(nodeTypeString)
				{
					case "attribute":
					{
						type = XmlNodeType.Attribute;
					}
					break;

					case "cdatasection":
					{
						type = XmlNodeType.CDATA;
					}
					break;

					case "comment":
					{
						type = XmlNodeType.Comment;
					}
					break;

					case "document":
					{
						type = XmlNodeType.Document;
					}
					break;

					case "documentfragment":
					{
						type = XmlNodeType.DocumentFragment;
					}
					break;

					case "documenttype":
					{
						type = XmlNodeType.DocumentType;
					}
					break;

					case "element":
					{
						type = XmlNodeType.Element;
					}
					break;

					case "entityreference":
					{
						type = XmlNodeType.EntityReference;
					}
					break;

					case "processinginstruction":
					{
						type = XmlNodeType.ProcessingInstruction;
					}
					break;

					case "significantwhitespace":
					{
						type = XmlNodeType.SignificantWhitespace;
					}
					break;

					case "text":
					{
						type = XmlNodeType.Text;
					}
					break;

					case "whitespace":
					{
						type = XmlNodeType.Whitespace;
					}
					break;

					default:
					{
						throw new ArgumentException
							(S._("Xml_InvalidNodeType"), "nodeTypeString");
					}
					// Not reached
				}
				return CreateNode(type, name, namespaceURI);
			}
	public virtual XmlNode CreateNode(XmlNodeType type,
									  String name,
									  String namespaceURI)
			{
				return CreateNode(type, null, name, namespaceURI);
			}
	public virtual XmlNode CreateNode(XmlNodeType type,
									  String prefix, String name,
									  String namespaceURI)
			{
				switch(type)
				{
					case XmlNodeType.Element:
					{
						if(prefix == null)
							return CreateElement(name, namespaceURI);
						else
							return CreateElement(prefix, name, namespaceURI);
					}
					// Not reached

					case XmlNodeType.Attribute:
					{
						if(prefix == null)
							return CreateAttribute(name, namespaceURI);
						else
							return CreateAttribute(prefix, name, namespaceURI);
					}
					// Not reached

					case XmlNodeType.Text:
					{
						return CreateTextNode(String.Empty);
					}
					// Not reached

					case XmlNodeType.CDATA:
					{
						return CreateCDataSection(String.Empty);
					}
					// Not reached

					case XmlNodeType.EntityReference:
					{
						return CreateEntityReference(name);
					}
					// Not reached

					case XmlNodeType.ProcessingInstruction:
					{
						return CreateProcessingInstruction(name, String.Empty);
					}
					// Not reached

					case XmlNodeType.Comment:
					{
						return CreateComment(String.Empty);
					}
					// Not reached

					case XmlNodeType.Document:
					{
						return new XmlDocument();
					}
					// Not reached

					case XmlNodeType.DocumentType:
					{
						return CreateDocumentType(name, String.Empty,
												  String.Empty, String.Empty);
					}
					// Not reached

					case XmlNodeType.DocumentFragment:
					{
						return CreateDocumentFragment();
					}
					// Not reached

					case XmlNodeType.Whitespace:
					{
						return CreateWhitespace(String.Empty);
					}
					// Not reached

					case XmlNodeType.SignificantWhitespace:
					{
						return CreateSignificantWhitespace(String.Empty);
					}
					// Not reached
				}
				throw new ArgumentOutOfRangeException
					("type", S._("Xml_InvalidNodeType"));
			}

	// Create a processing instruction node.
	public virtual XmlProcessingInstruction CreateProcessingInstruction
				(String target, String data)
			{
				return new XmlProcessingInstruction(placeholder, target, data);
			}

	// Create a significant whitespace node.
	public virtual XmlSignificantWhitespace CreateSignificantWhitespace
				(String text)
			{
				return new XmlSignificantWhitespace(placeholder, text);
			}

	// Create a text node.
	public virtual XmlText CreateTextNode(String text)
			{
				return new XmlText(placeholder, text);
			}

	// Create a whitespace node.
	public virtual XmlWhitespace CreateWhitespace(String text)
			{
				return new XmlWhitespace(placeholder, text);
			}

	// Create an XML declaration node.
	public virtual XmlDeclaration CreateXmlDeclaration
				(String version, String encoding, String standalone)
			{
				return new XmlDeclaration
					(placeholder, version, encoding, standalone);
			}

#if CONFIG_XPATH
	// NOTE: should be protected internal virtual 
	// but for ml-pnet's System.Data the internal modifier has to be removed
	protected virtual XPathNavigator CreateNavigator(XmlNode node)
			{
				return node.CreateNavigator();
			}
#endif

	// Get an element by identifier.
	[TODO]
	public virtual XmlElement GetElementById(String elementId)
			{
				// TODO
				return null;
			}

	// Get a list of elements by tag name.
	public virtual XmlNodeList GetElementsByTagName(String name)
			{
				return DocumentElement.GetElementsByTagName(name);
			}

	// Get a list of elements by local name and namespace.
	public virtual XmlNodeList GetElementsByTagName
				(String name, String namespaceURI)
			{
				return DocumentElement.GetElementsByTagName(name, namespaceURI);
			}

	// Import a node from another document.
	[TODO]
	public virtual XmlNode ImportNode(XmlNode node, bool deep)
			{
				// TODO: this may not be the best way to do this.
				// xml namespaces, dtds and other interesting things
				// have to be taken care of
				XmlNode imported = this.CreateNode(node.NodeType,
													node.Name,
													node.NamespaceURI);
				
				if(deep)
				{
					foreach(XmlNode child in imported.ChildNodes)
					{
						ImportNode(child, deep);
					}
				}
				return imported;
			}

	// Load XML into this document.
	public virtual void Load(Stream inStream)
			{
				Load(new XmlTextReader(inStream));
			}
	public virtual void Load(String filename)
			{
				if(!File.Exists(filename))
				{
					throw new XmlException
						(S._("XmlException_LoadError"));
				}
				Load(new XmlTextReader(filename));
			}
	public virtual void Load(TextReader txtReader)
			{
				Load(new XmlTextReader(txtReader));
			}
	public virtual void Load(XmlReader reader)
			{
				RemoveAll();
				if(!BuildStructure(reader))
				{
					throw new XmlException(S._("XmlException_NoXml"));
				}
			}

	// Load XML into this document from a string.
	public virtual void LoadXml(String xml)
			{
				if(xml == null || xml == String.Empty)
				{
					throw new XmlException(S._("XmlException_NoXml"));
				}
				
				RemoveAll();
				XmlTextReader reader = new XmlTextReader(xml, XmlNodeType.Document, null);
				BuildStructure(reader);
			}

	// Used by Load/LoadXml methods to do the DOM structure
	private bool BuildStructure(XmlReader reader)
			{
				// check if we should read in a full document
				bool complete = (reader.Depth == 0);

				// read the first child
				XmlNode node = ReadNode(reader);
				if(node == null) { return false; }

				// return now if we don't need to read any more
				if(!complete) { return true; }

				// read until we've consumed the full document and return
				while(node != null) { node = ReadNode(reader); }
				return true;
			}

	// Read the children of a node.
	internal void ReadChildren(XmlReader r, XmlNode parent)
			{
				if(parent is XmlAttribute)
				{
					while(r.ReadAttributeValue())
					{
						XmlNodeType nodeType = r.NodeType;
						if(nodeType == XmlNodeType.Text)
						{
							parent.AppendChild(CreateTextNode(r.Value));
						}
						else if(nodeType == XmlNodeType.EntityReference)
						{
							parent.AppendChild(CreateEntityReference(r.Name));
						}
						else
						{
							throw new InvalidOperationException(/* TODO */);
						}
					}
				}
				else
				{
					int startDepth = r.Depth;

					r.Read();
					while(r.ReadState == ReadState.Interactive &&
					      r.Depth != startDepth)
					{
						// read child
						XmlNode child = ReadNodeInternal(r);

						// ignore nulls, at least for now
						if(child != null)
						{
							// append child
							parent.AppendChild(child);
						}
					}

					// if we hit end-of-file, give an error
					if(r.Depth != startDepth)
					{
						throw new InvalidOperationException(/* TODO */);
					}

					// if we have an element, read past the end element node
					if(parent is XmlElement) { r.Read(); }
				}
			}

	// Used by the ReadNode method for recursively building a DOM subtree.
	internal XmlNode ReadNodeInternal(XmlReader r)
			{
				switch(r.NodeType)
				{
					case XmlNodeType.Attribute:
					{
						// create the attribute
						XmlAttribute att = CreateAttribute
							(r.Prefix, r.LocalName, r.NamespaceURI);

						// read and append the children
						ReadChildren(r, att);

						// return the attribute
						return att;
					}
					// Not reached.

					case XmlNodeType.CDATA:
					{
						// create the character data section
						XmlCDataSection cdata = CreateCDataSection(r.Value);

						// advance the reader to the next node
						r.Read();

						// return the character data section
						return cdata;
					}
					// Not reached.

					case XmlNodeType.Comment:
					{
						// create the comment
						XmlComment comment = CreateComment(r.Value);

						// advance the reader to the next node
						r.Read();

						// return the comment
						return comment;
					}
					// Not reached.

					case XmlNodeType.DocumentType:
					{
						// create the document type
						XmlDocumentType doctype = CreateDocumentType
							(r.Name, r["PUBLIC"], r["SYSTEM"], r.Value);

						// advance the reader to the next node
						r.Read();

						// return the document type
						return doctype;
					}
					// Not reached.

					case XmlNodeType.Element:
					{
						// create the element
						XmlElement elem = CreateElement
							(r.Prefix, r.LocalName, r.NamespaceURI);
						bool isEmptyElement = r.IsEmptyElement;

						// read the attributes
						while(r.MoveToNextAttribute())
						{
							XmlAttribute att = (XmlAttribute)ReadNodeInternal(r);
							elem.SetAttributeNode(att);
						}
						r.MoveToElement();

						// return now if there are no children
						if(isEmptyElement)
						{
							// advance the reader to the next node
							r.Read();

							// return the empty element
							return elem;
						}

						elem.IsEmpty = isEmptyElement;

						// read and append the children
						ReadChildren(r, elem);

						// return the element
						return elem;
					}
					// Not reached.

					case XmlNodeType.DocumentFragment:
					case XmlNodeType.EndElement:
					case XmlNodeType.EndEntity:
					case XmlNodeType.Entity:
					case XmlNodeType.Notation:
					{
						// TODO

						// advance the reader to the next node
						r.Read();

						// nothing to return
						return null;
					}
					// Not reached.

					case XmlNodeType.EntityReference:
					{
						// create the entity reference
						XmlEntityReference er = CreateEntityReference(r.Name);

						// advance the reader to the next node
						r.Read();

						// return the entity reference
						return er;
					}
					// Not reached.

					case XmlNodeType.ProcessingInstruction:
					{
						// create the processing instruction
						XmlProcessingInstruction pi = CreateProcessingInstruction
							(r.Name, r.Value);

						// advance the reader to the next node
						r.Read();

						// return the processing instruction
						return pi;
					}
					// Not reached.

					case XmlNodeType.SignificantWhitespace:
					{
						// create the significant whitespace
						XmlSignificantWhitespace sws = CreateSignificantWhitespace
							(r.Value);

						// advance the reader to the next node
						r.Read();

						// return the significant whitespace
						return sws;
					}
					// Not reached.

					case XmlNodeType.Text:
					{
						// create the text
						XmlText text = CreateTextNode(r.Value);

						// advance the reader to the next node
						r.Read();

						// return the text
						return text;
					}
					// Not reached.

					case XmlNodeType.Whitespace:
					{
						// if whitespace preservation is off, skip whitespace
						if(!preserveWhitespace)
						{
							while(r.Read())
							{
								if(r.NodeType != XmlNodeType.Whitespace)
								{
									return null;
								}
							}
							return null;
						}

						// create the whitespace
						XmlWhitespace ws = CreateWhitespace(r.Value);

						// advance the reader to the next node
						r.Read();

						// return the whitespace
						return ws;
					}
					// Not reached.

					case XmlNodeType.XmlDeclaration:
					{
						// set up the defaults
						String e = String.Empty;
						String s = String.Empty;

						// read the version
						r.MoveToNextAttribute();

						// read the optional attributes
						if(r.MoveToNextAttribute())
						{
							if(r.Name == "encoding")
							{
								e = r.Value;
								if(r.MoveToNextAttribute())
								{
									s = r.Value;
								}
							}
							else
							{
								s = r.Value;
							}
						}

						// create the xml declaration
						XmlDeclaration xml = CreateXmlDeclaration("1.0", e, s);

						// advance the reader to the next node
						r.Read();

						// return the xml declaration
						return xml;
					}
					// Not reached.

					default:
					{
						throw new InvalidOperationException(/* TODO */);
					}
					// Not reached.
				}
			}

	// Read a node into this document.
	public virtual XmlNode ReadNode(XmlReader reader)
			{
				// if there's no current node, read one
				if(reader.ReadState == ReadState.Initial)
				{
					reader.Read();

					// if the reader's input was empty, return null
					if(reader.NodeType == XmlNodeType.None)
					{
						return null;
					}
				}
				else if(reader.ReadState != ReadState.Interactive)
				{
					// if the reader is not in a readable state, return null
					return null;
				}

				XmlNode result = null;
				switch(reader.NodeType)
				{
					case XmlNodeType.Attribute:
					{
						// read the attribute and its children
						result = ReadNodeInternal(reader);
					}
					break;

					case XmlNodeType.CDATA:
					{
						// read the cdata
						result = ReadNodeInternal(reader);
					}
					break;

					case XmlNodeType.Comment:
					{
						// read the comment
						result = ReadNodeInternal(reader);

						// append the comment to this document
						AppendChild(result);
					}
					break;

					case XmlNodeType.DocumentType:
					{
						// read the doctype
						result = ReadNodeInternal(reader);

						// append the doctype to this document
						AppendChild(result);
					}
					break;

					case XmlNodeType.Element:
					{
						// read the element and its children
						result = ReadNodeInternal(reader);

						// append the element to this document
						AppendChild(result);
					}
					break;

					case XmlNodeType.EndElement:
					case XmlNodeType.EndEntity:
					{
						throw new InvalidOperationException(/* TODO */);
					}
					// Not reached.

					case XmlNodeType.ProcessingInstruction:
					{
						// read the processing instruction
						result = ReadNodeInternal(reader);

						// append the processing instruction to this document
						AppendChild(result);
					}
					break;

					case XmlNodeType.SignificantWhitespace:
					{
						// read the significant whitespace
						result = ReadNodeInternal(reader);
					}
					break;

					case XmlNodeType.Text:
					{
						// read the text
						result = ReadNodeInternal(reader);
					}
					break;

					case XmlNodeType.Whitespace:
					{
						// if whitespace preservation is off, skip whitespace
						if(!preserveWhitespace)
						{
							while(reader.Read())
							{
								if(reader.NodeType != XmlNodeType.Whitespace)
								{
									return ReadNode(reader);
								}
							}
							return null;
						}

						// read the whitespace
						result = ReadNodeInternal(reader);

						// append the whitespace to this document
						AppendChild(result);
					}
					break;

					case XmlNodeType.XmlDeclaration:
					{
						// read the xml declaration
						result = ReadNodeInternal(reader);

						// append the xml declaration to this document
						AppendChild(result);
					}
					break;
				}

				// return the result node
				return result;
			}

	// Save the contents of this document to a stream.
	public virtual void Save(Stream outStream)
			{
				// check for a root element
				if(DocumentElement == null)
				{
					throw new XmlException
						(S._("Xml_NoRootElement"));
				}

				// get the xml declaration
				XmlDeclaration dec = (XmlDeclaration)GetChildByType
					(XmlNodeType.XmlDeclaration);

				// determine the encoding
				Encoding enc = null;
				if(dec != null)
				{
					String encName = dec.Encoding;
					if(encName != null && encName.Length != 0)
					{
						enc = Encoding.GetEncoding(encName);
					}
				}

				// create the writer
				XmlTextWriter w = new XmlTextWriter(outStream, enc);

				// turn on formatting if whitespace preservation is off
				if(!preserveWhitespace) { w.Formatting = Formatting.Indented; }

				// make sure we have an xml declaration
				if(dec == null) { w.WriteStartDocument(); }

				// write this document to the writer
				WriteTo(w);

				// flush the output
				w.Flush();
			}

	// Save the contents of this document to a file.
	public virtual void Save(String filename)
			{
				// create a stream for the file
				FileStream fs = new FileStream
					(filename, FileMode.Create, FileAccess.Write);

				// save using the stream version of save
				Save(fs);

				// close the file stream
				fs.Close();
			}

	// Save the contents of this document to a writer.
	public virtual void Save(TextWriter writer)
			{
				// create a new xml text writer using the given writer
				XmlTextWriter w = new XmlTextWriter(writer);

				// turn on formatting if whitespace preservation is off
				if(!preserveWhitespace) { w.Formatting = Formatting.Indented; }

				// get the xml declaration
				XmlDeclaration dec = (XmlDeclaration)GetChildByType
					(XmlNodeType.XmlDeclaration);

				// reset the declaration's encoding to that of the writer
				if(dec != null)
				{
					Encoding enc = writer.Encoding;
					String encName = String.Empty;
					if(enc != null) { encName = enc.WebName; }
					dec.Encoding = encName;
				}

				// save using the xml text writer version of save
				Save(w);
			}

	// Save the contents of this document to an XML writer.
	public virtual void Save(XmlWriter w)
			{
				// check for a root element
				if(DocumentElement == null)
				{
					throw new XmlException
						(S._("Xml_NoRootElement"));
				}

				// get the xml declaration
				XmlDeclaration dec = (XmlDeclaration)GetChildByType
					(XmlNodeType.XmlDeclaration);

				// handle the encoding and xml declaration stuff
				if(dec == null)
				{
					w.WriteStartDocument();
				}

				// write this document to the writer
				WriteTo(w);

				// flush the output
				w.Flush();
			}

	// Write the contents of this document to an XML writer.
	public override void WriteContentTo(XmlWriter xw)
			{
				WriteChildrenTo(xw);
			}

	// Write this document to an XML writer.
	public override void WriteTo(XmlWriter w)
			{
				WriteContentTo(w);
			}

	// Events.
	public event XmlNodeChangedEventHandler NodeChanged;
	public event XmlNodeChangedEventHandler NodeChanging;
	public event XmlNodeChangedEventHandler NodeInserted;
	public event XmlNodeChangedEventHandler NodeInserting;
	public event XmlNodeChangedEventHandler NodeRemoved;
	public event XmlNodeChangedEventHandler NodeRemoving;

	// Emit an event before a particular node change.  Returns an
	// argument block if "EmitAfter" may be needed, or null if not.
	internal XmlNodeChangedEventArgs EmitBefore
					(XmlNodeChangedAction action, XmlNode node,
				 	 XmlNode oldParent, XmlNode newParent)
			{
				XmlNodeChangedEventArgs args = null;
				switch(action)
				{
					case XmlNodeChangedAction.Insert:
					{
						if(NodeInserting != null)
						{
							args = new XmlNodeChangedEventArgs
								(action, node, oldParent, newParent);
							NodeInserting(this, args);
						}
						else if(NodeInserted != null)
						{
							args = new XmlNodeChangedEventArgs
								(action, node, oldParent, newParent);
						}
					}
					break;

					case XmlNodeChangedAction.Remove:
					{
						if(NodeRemoving != null)
						{
							args = new XmlNodeChangedEventArgs
								(action, node, oldParent, newParent);
							NodeRemoving(this, args);
						}
						else if(NodeRemoved != null)
						{
							args = new XmlNodeChangedEventArgs
								(action, node, oldParent, newParent);
						}
					}
					break;

					case XmlNodeChangedAction.Change:
					{
						if(NodeChanging != null)
						{
							args = new XmlNodeChangedEventArgs
								(action, node, oldParent, newParent);
							NodeChanging(this, args);
						}
						else if(NodeChanged != null)
						{
							args = new XmlNodeChangedEventArgs
								(action, node, oldParent, newParent);
						}
					}
					break;
				}
				return args;
			}

	// Emit an event after a particular node change.
	internal new void EmitAfter(XmlNodeChangedEventArgs args)
			{
				switch(args.Action)
				{
					case XmlNodeChangedAction.Insert:
					{
						if(NodeInserted != null)
						{
							NodeInserted(this, args);
						}
					}
					break;

					case XmlNodeChangedAction.Remove:
					{
						if(NodeRemoved != null)
						{
							NodeRemoved(this, args);
						}
					}
					break;

					case XmlNodeChangedAction.Change:
					{
						if(NodeChanged != null)
						{
							NodeChanged(this, args);
						}
					}
					break;
				}
			}

	// Determine if a particular node type can be inserted as
	// a child of the current node.
	internal override bool CanInsert(XmlNodeType type)
			{
				switch(type)
				{
					case XmlNodeType.Element:
					case XmlNodeType.DocumentType:
					case XmlNodeType.XmlDeclaration:
					{
						return (GetChildByType(type) == null);
					}
					// Not reached.

					case XmlNodeType.ProcessingInstruction:
					case XmlNodeType.Comment:
					case XmlNodeType.Whitespace:
					{
						return true;
					}
					// Not reached.

					default: break;
				}
				return false;
			}

	// Determine if a particular node type can be inserted after another,
	// which may be null if the list is currently empty.
	internal override bool CanInsertAfter(XmlNodeType type, XmlNode refNode)
			{
				XmlNode temp;

				// Filter out types that are definitely not allowed.
				if(!CanInsert(type))
				{
					return false;
				}

				// If nothing in the list yet, then we can insert anything.
				if(refNode == null)
				{
					return true;
				}

				// Handle special node categories.
				switch(type)
				{
					case XmlNodeType.DocumentType:
					{
						// Must not have an element before this position.
						temp = refNode;
						while(temp != null)
						{
							if(temp.NodeType == XmlNodeType.Element)
							{
								return false;
							}
							temp = NodeList.GetPreviousSibling(temp);
						}

						// Must not have an XmlDeclaration after this position.
						refNode = NodeList.GetNextSibling(refNode);
						while(refNode != null)
						{
							if(refNode.NodeType == XmlNodeType.XmlDeclaration)
							{
								return false;
							}
							refNode = NodeList.GetNextSibling(refNode);
						}
					}
					break;

					case XmlNodeType.Element:
					{
						// Must not have a document type or XmlDeclaration
						// after this position.
						refNode = NodeList.GetNextSibling(refNode);
						while(refNode != null)
						{
							if(refNode.NodeType == XmlNodeType.DocumentType ||
							   refNode.NodeType == XmlNodeType.XmlDeclaration)
							{
								return false;
							}
							refNode = NodeList.GetNextSibling(refNode);
						}
					}
					break;

					case XmlNodeType.XmlDeclaration:
					{
						// Xml declarations can never come after other nodes.
						return false;
					}
					// Not reached.
				}

				// If we get here, then the node is allowed.
				return true;
			}

	// Determine if a particular node type can be inserted before another,
	// which may be null if the list is currently empty.
	internal override bool CanInsertBefore(XmlNodeType type, XmlNode refNode)
			{
				XmlNode temp;

				// Filter out types that are definitely not allowed.
				if(!CanInsert(type))
				{
					return false;
				}

				// If nothing in the list yet, then we can insert anything.
				if(refNode == null)
				{
					return true;
				}

				// Handle special node categories.
				switch(type)
				{
					case XmlNodeType.DocumentType:
					{
						// Must not have an element before this position.
						temp = NodeList.GetPreviousSibling(refNode);
						while(temp != null)
						{
							if(temp.NodeType == XmlNodeType.Element)
							{
								return false;
							}
							temp = NodeList.GetPreviousSibling(temp);
						}

						// Must not have an XmlDeclaration after this position.
						while(refNode != null)
						{
							if(refNode.NodeType == XmlNodeType.XmlDeclaration)
							{
								return false;
							}
							refNode = NodeList.GetNextSibling(refNode);
						}
					}
					break;

					case XmlNodeType.Element:
					{
						// Must not have a document type or XmlDeclaration
						// after this position.
						while(refNode != null)
						{
							if(refNode.NodeType == XmlNodeType.DocumentType ||
							   refNode.NodeType == XmlNodeType.XmlDeclaration)
							{
								return false;
							}
							refNode = NodeList.GetNextSibling(refNode);
						}
					}
					break;

					case XmlNodeType.XmlDeclaration:
					{
						// Xml declarations must come before everything else.
						return (refNode == NodeList.GetFirstChild(this));
					}
					// Not reached.
				}

				// If we get here, then the node is allowed.
				return true;
			}

	// Get the placeholder document fragment associated with a document.
	internal XmlDocumentFragment Placeholder
			{
				get
				{
					return placeholder;
				}
			}

}; // class XmlDocument

}; // namespace System.Xml
