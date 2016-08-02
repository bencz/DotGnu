/*
 * XmlReader.cs - Implementation of the "System.Xml.XmlReader" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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

public abstract class XmlReader
{
	// Constructor.
	protected XmlReader() {}

	// Clean up the resources that were used by this XML reader.
	public abstract void Close();

	// Returns the value of an attribute with a specific index.
	public abstract String GetAttribute(int i);

	// Returns the value of an attribute with a specific name.
	public abstract String GetAttribute(String name, String namespaceURI);

	// Returns the value of an attribute with a specific qualified name.
	public abstract String GetAttribute(String name);

	// Determine if a string is a valid XML name.
	public static bool IsName(String name)
			{
				int posn;
				if(((Object)name) == null || name.Length == 0)
				{
					return false;
				}
				else if(!XmlConvert.IsNameStart(name[0], true))
				{
					return false;
				}
				for(posn = 1; posn < name.Length; ++posn)
				{
					if(!XmlConvert.IsNameNonStart(name[posn], true))
					{
						return false;
					}
				}
				return true;
			}

	// Determine if a string is a valid XML name token.
	public static bool IsNameToken(String name)
			{
				int posn;
				if(((Object)name) == null || name.Length == 0)
				{
					return false;
				}
				for(posn = 0; posn < name.Length; ++posn)
				{
					if(!XmlConvert.IsNameNonStart(name[posn], true))
					{
						return false;
					}
				}
				return true;
			}

	// Determine if we are positioned on a start element with
	// a particular name and namespace.
	public virtual bool IsStartElement(String localname, String ns)
			{
				if(MoveToContent() != XmlNodeType.Element)
				{
					return false;
				}
				return (LocalName == localname && NamespaceURI == ns);
			}

	// Determine if we are positioned on a start element with
	// a particular qualified name.
	public virtual bool IsStartElement(String name)
			{
				if(MoveToContent() != XmlNodeType.Element)
				{
					return false;
				}
				return (Name == name);
			}

	// Determine if we are positioned on a start element.
	public virtual bool IsStartElement()
			{
				return (MoveToContent() == XmlNodeType.Element);
			}

	// Resolve a namespace in the scope of the current element.
	public abstract String LookupNamespace(String prefix);

	// Move the current position to a particular attribute.
	public abstract void MoveToAttribute(int i);

	// Move the current position to an attribute with a particular name.
	public abstract bool MoveToAttribute(String name, String ns);

	// Move the current position to an attribute with a qualified name.
	public abstract bool MoveToAttribute(String name);

	// Move to the content part of an element node.
	public virtual XmlNodeType MoveToContent()
			{
				XmlNodeType type;
				do
				{
					type = NodeType;
					if(type == XmlNodeType.Attribute)
					{
						MoveToElement();
						return NodeType;
					}
					else if(type == XmlNodeType.Element ||
							type == XmlNodeType.EndElement ||
							type == XmlNodeType.Text ||
							type == XmlNodeType.CDATA ||
							type == XmlNodeType.Entity ||
							type == XmlNodeType.EntityReference)
					{
						return type;
					}
				}
				while(Read());
				return NodeType;
			}

	// Move to the element that owns the current attribute.
	public abstract bool MoveToElement();

	// Move to the first attribute owned by the current element.
	public abstract bool MoveToFirstAttribute();

	// Move to the next attribute owned by the current element.
	public abstract bool MoveToNextAttribute();

	// Read the next node in the input stream.
	public abstract bool Read();

	// Read the next attribute value in the input stream.
	public abstract bool ReadAttributeValue();

	// Read the contents of a text element string using a name.
	public virtual String ReadElementString(String localname, String ns)
			{
				if( (MoveToContent() != XmlNodeType.Element) ||
				    (LocalName != localname ) || 
						(NamespaceURI == ns) )
				{
					throw new XmlException(S._("Xml_IncorrectNode"));
				}
				
				if( !this.IsEmptyElement ) 
				{
					string ret = ReadString();
					if (this.NodeType != XmlNodeType.EndElement)
					{
						throw new XmlException(S._("Xml_IncorrectNode"));
					}
					this.Read();
					return ret;
				}
				this.Read();
				return string.Empty;
			}

	// Read the contents of a text element string using a qualified name.
	public virtual String ReadElementString(String name)
			{
				if(MoveToContent() != XmlNodeType.Element || Name != name)
				{
					throw new XmlException(S._("Xml_IncorrectNode"));	
				}
				
				if( !this.IsEmptyElement ) 
				{
					string ret = ReadString();
					if (this.NodeType != XmlNodeType.EndElement)
					{
						throw new XmlException(S._("Xml_IncorrectNode"));
					}
					this.Read();
					return ret;
				}
				this.Read();
				return string.Empty;
			}

	// Read the contents of a text element.
	public virtual String ReadElementString()
			{
				if(MoveToContent() != XmlNodeType.Element)
				{
					throw new XmlException(S._("Xml_IncorrectNode"));
				}
				if( !this.IsEmptyElement ) 
				{
					this.Read();
					string ret = ReadString();
					if (this.NodeType != XmlNodeType.EndElement)
					{
						throw new XmlException(S._("Xml_IncorrectNode"));
					}
					this.Read();
					return ret;
				}
				this.Read();
				return string.Empty;
			}

	// Read an end element node and advance to the next node.
	public virtual void ReadEndElement()
			{
				if(MoveToContent() == XmlNodeType.EndElement)
				{
					Read();
				}
				else
				{
					throw new XmlException(S._("Xml_IncorrectNode"));
				}
			}

	// Read the contents of the current node, including all markup.
	public virtual String ReadInnerXml()
			{
				// bail out now if there's nothing to read
				if(ReadState != ReadState.Interactive) { return String.Empty; }

				if(NodeType == XmlNodeType.Attribute)
				{
					// create the writer
					StringWriter sw = new StringWriter();

					// store these so we can reset the ReadAttributeValue state
					String namespaceURI = NamespaceURI;
					String localName = LocalName;

					// read until we consume all of the attribute value
					while(ReadAttributeValue())
					{
						WriteAttributeValue(sw);
					}

					// reset the ReadAttributeValue state
					MoveToAttribute(localName, namespaceURI);

					// return the full attribute value
					return sw.ToString();
				}
				else if(NodeType == XmlNodeType.Element)
				{
					// handle the empty element case
					if(IsEmptyElement)
					{
						Read();
						return String.Empty;
					}

					// create the writers
					StringWriter sw = new StringWriter();
					XmlTextWriter w = new XmlTextWriter(sw);

					// store the start depth so we know when to stop
					int startDepth = Depth;

					// read past the start tag
					Read();

					// read until we consume all the children
					while(startDepth != Depth)
					{
						w.WriteNode(this, false);
					}

					// read past the end tag
					Read();

					// return the element content
					return sw.ToString();
				}
				else
				{
					Read();
					return String.Empty;
				}
			}

	// Read the current node, including all markup.
	public virtual String ReadOuterXml()
			{
				// bail out now if there's nothing to read
				if(ReadState != ReadState.Interactive) { return String.Empty; }

				if(NodeType == XmlNodeType.Attribute)
				{
					// create the writers
					StringWriter sw = new StringWriter();
					XmlTextWriter w = new XmlTextWriter(sw);

					// store these so we can reset the ReadAttributeValue state
					String namespaceURI = NamespaceURI;
					String localName = LocalName;

					// set the quote character
					w.QuoteChar = QuoteChar;

					// start writing the attribute
					w.WriteStartAttribute(Prefix, localName, namespaceURI);

					// read until we consume all of the attribute value
					while(ReadAttributeValue())
					{
						WriteAttributeValue(sw);
					}

					// finish writing the attribute
					w.WriteEndAttribute();

					// reset the ReadAttributeValue state
					MoveToAttribute(localName, namespaceURI);

					// return the full attribute value
					return sw.ToString();
				}
				else if(NodeType == XmlNodeType.Element)
				{
					// create the writers
					StringWriter sw = new StringWriter();
					XmlTextWriter w = new XmlTextWriter(sw);

					// write the element and its children
					w.WriteNode(this, false);

					// return the element content
					return sw.ToString();
				}
				else
				{
					Read();
					return String.Empty;
				}
			}

	// Read a start element with a particular name.
	public virtual void ReadStartElement(String localname, String ns)
			{
				if(MoveToContent() == XmlNodeType.Element &&
				   LocalName == localname && NamespaceURI == ns)
				{
					Read();
				}
				else
				{
					throw new XmlException(S._("Xml_IncorrectNode"));
				}
			}

	// Read a start element with a particular qualified name.
	public virtual void ReadStartElement(String name)
			{
				if(MoveToContent() == XmlNodeType.Element && Name == name)
				{
					Read();
				}
				else
				{
					throw new XmlException(S._("Xml_IncorrectNode"));
				}
			}

	// Read a start element.
	public virtual void ReadStartElement()
			{
				if(MoveToContent() == XmlNodeType.Element)
				{
					Read();
				}
				else
				{
					throw new XmlException(S._("Xml_IncorrectNode"));
				}
			}

	// Read the contents of an element or text node as a string.
	public virtual String ReadString()
			{
				// bail out now if there's nothing to read
				if(ReadState != ReadState.Interactive) { return String.Empty; }

				// move to the element (in case we're on one of its attributes)
				MoveToElement();

				// move past the element, if we're positioned on it
				if(NodeType == XmlNodeType.Element)
				{
					// bail out now if there's nothing to read
					if(IsEmptyElement)
					{
						return String.Empty;
					}
					Read();
					if(NodeType == XmlNodeType.EndElement)
					{
						return String.Empty;
					}
				}

				// create the content log
				StringBuilder log = new StringBuilder();

				// read until we consume all the text content
				bool stop = false;
				do
				{
					XmlNodeType nodeType = NodeType;
					if(nodeType == XmlNodeType.Text ||
					   nodeType == XmlNodeType.CDATA ||
					   nodeType == XmlNodeType.Whitespace ||
					   nodeType == XmlNodeType.SignificantWhitespace)
					{
						log.Append(Value);
					}
					else
					{
						stop = true;
					}
				}
				while(!stop && Read());

				// return the text content
				return log.ToString();
			}

	// Resolve an entity reference.
	public abstract void ResolveEntity();

	// Skip the current element in the input.
	public virtual void Skip()
			{
				if(!IsEmptyElement)
				{
					int level = Depth;
					XmlNodeType type = NodeType;
					if(type != XmlNodeType.Element &&
						type != XmlNodeType.EndElement)
					{
						level--;
					}
					do
					{
						Read();
						type = NodeType;
						if(type == XmlNodeType.Element)
						{
							if(level == Depth)
							{
								return;
							}
						}
						else if(type == XmlNodeType.None)
						{
							throw new XmlException(S._("Xml_UnexpectedEOF"));
						}
					}
					while(level <= Depth);
				}
				else
				{
					Read();
				}
			}

	// Get the number of attributes on the current node.
	public abstract int AttributeCount { get; }

	// Get the base URI for the current node.
	public abstract String BaseURI { get; }

	// Determine if this reader can parse and resolve entities.
	public virtual bool CanResolveEntity
			{
				get
				{
					return false;
				}
			}

	// Get the depth of the current node.
	public abstract int Depth { get; }

	// Determine if we have reached the end of the input stream.
	public abstract bool EOF { get; }

	// Determine if the current node has attributes.
	public virtual bool HasAttributes
			{
				get
				{
					return (AttributeCount > 0);
				}
			}

	// Determine if the current node can have an associated text value.
	public abstract bool HasValue { get; }

	// Determine if the current node's value was generated from a DTD default.
	public abstract bool IsDefault { get; }

	// Determine if the current node is an empty element.
	public abstract bool IsEmptyElement { get; }

	// Retrieve an attribute value with a specified index.
	public abstract String this[int i] { get; }

	// Retrieve an attribute value with a specified name.
	public abstract String this[String localname, String namespaceURI] { get; }

	// Retrieve an attribute value with a specified qualified name.
	public abstract String this[String name] { get; }

	// Get the local name of the current node.
	public abstract String LocalName { get; }

	// Get the fully-qualified name of the current node.
	public abstract String Name { get; }

	// Get the name that that is used to look up and resolve names.
	public abstract XmlNameTable NameTable { get; }

	// Get the namespace URI associated with the current node.
	public abstract String NamespaceURI { get; }

	// Get the type of the current node.
	public abstract XmlNodeType NodeType { get; }

	// Get the namespace prefix associated with the current node.
	public abstract String Prefix { get; }

	// Get the quote character that was used to enclose an attribute value.
	public abstract char QuoteChar { get; }

	// Get the current read state of the reader.
	public abstract ReadState ReadState { get; }

	// Get the text value of the current node.
	public abstract String Value { get; }

	// Get the current xml:lang scope.
	public abstract String XmlLang { get; }

	// Get the current xml:space scope.
	public abstract XmlSpace XmlSpace { get; }



	private static readonly String builtinChars = "'\"<>&";
	private static readonly String[] builtinRefs = new String[]
	{
		"&apos;",
		"&quot;",
		"&lt;",
		"&gt;",
		"&amp;"
	};

	// Write the value of an attribute node to a text writer.
	private void WriteAttributeValue(TextWriter w)
			{
				if(NodeType == XmlNodeType.EntityReference)
				{
					// write a general entity reference
					w.Write("&{0};", Name);
				}
				else
				{
					String v = Value;
					int len = v.Length;

					// read until we consume all of the value
					int textStart = 0;
					for(int i = 0; i < len; ++i)
					{
						// if we have a builtin for the character, escape it
						int pos = builtinChars.IndexOf(v[i]);
						if(pos != -1)
						{
							// if we have text, write it
							if(i > textStart)
							{
								w.Write(v.Substring(textStart, i - textStart));
							}

							// write the builtin reference
							w.Write(builtinRefs[pos]);

							// store the start index of the next segment
							textStart = i+1;
						}
					}

					// if we have text, write it
					if(len > textStart)
					{
						w.Write(v.Substring(textStart, len - textStart));
					}
				}
			}

}; // class XmlReader

}; // namespace System.Xml
