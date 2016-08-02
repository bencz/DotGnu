/*
 * XmlValidatingReader.cs - Implementation of the
 *		"System.Xml.XmlValidatingReader" class.
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

#if !ECMA_COMPAT

using System;
using System.IO;
using System.Text;
using System.Xml.Schema;

// Note: this class doesn't actually do any validation yet.  It acts as
// a pass-through to "XmlTextReader" to fake out applications that ask
// for a validating reader when a non-validating one will normally do.
// Patches are welcome to make this really do validation.

public class XmlValidatingReader : XmlReader, IXmlLineInfo
{
	public event ValidationEventHandler ValidationEventHandler;

	// Internal state.
	private XmlTextReader reader;
	private EntityHandling entityHandling;
	private ValidationType validationType;

	// Constructors.
	public XmlValidatingReader(XmlReader reader)
			{
				this.reader = (reader as XmlTextReader);
				if(this.reader == null)
				{
					throw new ArgumentException
						(S._("Xml_NotTextReader"), "reader");
				}
				entityHandling = EntityHandling.ExpandEntities;
				validationType = ValidationType.Auto;
				this.reader.Normalization = true;
			}
	public XmlValidatingReader(Stream xmlFragment, XmlNodeType fragType,
							   XmlParserContext context)
			{
				reader = new XmlTextReader(xmlFragment, fragType, context);
				entityHandling = EntityHandling.ExpandEntities;
				validationType = ValidationType.Auto;
				reader.Normalization = true;
			}
	public XmlValidatingReader(String xmlFragment, XmlNodeType fragType,
							   XmlParserContext context)
			{
				reader = new XmlTextReader(xmlFragment, fragType, context);
				entityHandling = EntityHandling.ExpandEntities;
				validationType = ValidationType.Auto;
				reader.Normalization = true;
			}

	// Clean up the resources that were used by this XML reader.
	public override void Close()
			{
				reader.Close();
			}

	// Returns the value of an attribute with a specific index.
	public override String GetAttribute(int i)
			{
				return reader.GetAttribute(i);
			}

	// Returns the value of an attribute with a specific name.
	public override String GetAttribute(String name, String namespaceURI)
			{
				return reader.GetAttribute(name, namespaceURI);
			}

	// Returns the value of an attribute with a specific qualified name.
	public override String GetAttribute(String name)
			{
				return reader.GetAttribute(name);
			}

	// Resolve a namespace in the scope of the current element.
	public override String LookupNamespace(String prefix)
			{
				return reader.LookupNamespace(prefix);
			}

	// Move the current position to a particular attribute.
	public override void MoveToAttribute(int i)
			{
				reader.MoveToAttribute(i);
			}

	// Move the current position to an attribute with a particular name.
	public override bool MoveToAttribute(String name, String ns)
			{
				return reader.MoveToAttribute(name, ns);
			}

	// Move the current position to an attribute with a qualified name.
	public override bool MoveToAttribute(String name)
			{
				return reader.MoveToAttribute(name);
			}

	// Move to the element that owns the current attribute.
	public override bool MoveToElement()
			{
				return reader.MoveToElement();
			}

	// Move to the first attribute owned by the current element.
	public override bool MoveToFirstAttribute()
			{
				return reader.MoveToFirstAttribute();
			}

	// Move to the next attribute owned by the current element.
	public override bool MoveToNextAttribute()
			{
				return reader.MoveToNextAttribute();
			}

	// Read the next node in the input stream.
	public override bool Read()
			{
				return reader.Read();
			}

	// Read the next attribute value in the input stream.
	public override bool ReadAttributeValue()
			{
				return reader.ReadAttributeValue();
			}

	// Read the contents of the current node, including all markup.
	public override String ReadInnerXml()
			{
				return reader.ReadInnerXml();
			}

	// Read the current node, including all markup.
	public override String ReadOuterXml()
			{
				return reader.ReadOuterXml();
			}

	// Read the contents of an element or text node as a string.
	public override String ReadString()
			{
				return reader.ReadString();
			}

	// Read a typed value.
	[TODO]
	public Object ReadTypedValue()
			{
				// TODO
				return ReadString();
			}

	// Resolve an entity reference.
	public override void ResolveEntity()
			{
				reader.ResolveEntity();
			}

	// Get the number of attributes on the current node.
	public override int AttributeCount
			{
				get
				{
					return reader.AttributeCount;
				}
			}

	// Get the base URI for the current node.
	public override String BaseURI
			{
				get
				{
					return reader.BaseURI;
				}
			}

	// Determine if this reader can parse and resolve entities.
	public override bool CanResolveEntity
			{
				get
				{
					return true;
				}
			}

	// Get the depth of the current node.
	public override int Depth
			{
				get
				{
					return reader.Depth;
				}
			}

	// Determine if we have reached the end of the input stream.
	public override bool EOF
			{
				get
				{
					return reader.EOF;
				}
			}

	// Get the encoding that is in use by the XML stream.
	public Encoding Encoding
			{
				get
				{
					return reader.Encoding;
				}
			}

	// Get or set the entity handling mode.
	public EntityHandling EntityHandling
			{
				get
				{
					return entityHandling;
				}
				set
				{
					if(value != EntityHandling.ExpandEntities &&
					   value != EntityHandling.ExpandCharEntities)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("Xml_InvalidEntityHandling"));
					}
					entityHandling = value;
				}
			}

	// Determine if the current node can have an associated text value.
	public override bool HasValue
			{
				get
				{
					return reader.HasValue;
				}
			}

	// Determine if the current node's value was generated from a DTD default.
	public override bool IsDefault
			{
				get
				{
					return reader.IsDefault;
				}
			}

	// Determine if the current node is an empty element.
	public override bool IsEmptyElement
			{
				get
				{
					return reader.IsEmptyElement;
				}
			}

	// Retrieve an attribute value with a specified index.
	public override String this[int i]
			{
				get
				{
					return reader.GetAttribute(i);
				}
			}

	// Retrieve an attribute value with a specified name.
	public override String this[String localname, String namespaceURI]
			{
				get
				{
					return reader.GetAttribute(localname, namespaceURI);
				}
			}

	// Retrieve an attribute value with a specified qualified name.
	public override String this[String name]
			{
				get
				{
					return reader.GetAttribute(name);
				}
			}

	// Determine if we have line information.
	bool IXmlLineInfo.HasLineInfo()
			{
				return ((IXmlLineInfo)reader).HasLineInfo();
			}

	// Get the current line number.
	public int LineNumber
			{
				get
				{
					return reader.LineNumber;
				}
			}

	// Get the current line position.
	public int LinePosition
			{
				get
				{
					return reader.LinePosition;
				}
			}

	// Get the local name of the current node.
	public override String LocalName
			{
				get
				{
					return reader.LocalName;
				}
			}

	// Get the fully-qualified name of the current node.
	public override String Name
			{
				get
				{
					return reader.Name;
				}
			}

	// Get the name that that is used to look up and resolve names.
	public override XmlNameTable NameTable
			{
				get
				{
					return reader.NameTable;
				}
			}

	// Get the namespace URI associated with the current node.
	public override String NamespaceURI
			{
				get
				{
					return reader.NamespaceURI;
				}
			}

	// Get or set the "namespace support" flag for this reader.
	public bool Namespaces
			{
				get
				{
					return reader.Namespaces;
				}
				set
				{
					reader.Namespaces = value;
				}
			}

	// Get the type of the current node.
	public override XmlNodeType NodeType
			{
				get
				{
					return reader.NodeType;
				}
			}

	// Get the namespace prefix associated with the current node.
	public override String Prefix
			{
				get
				{
					return reader.Prefix;
				}
			}

	// Get the quote character that was used to enclose an attribute value.
	public override char QuoteChar
			{
				get
				{
					return reader.QuoteChar;
				}
			}

	// Get the reader underlying this validating reader.
	public XmlReader Reader
			{
				get
				{
					return reader;
				}
			}

	// Get the current read state of the reader.
	public override ReadState ReadState
			{
				get
				{
					return reader.ReadState;
				}
			}

	// Get the schemas that are being used to validate.
	[TODO]
	public XmlSchemaCollection Schemas
			{
				get
				{
					// TODO
					return null;
				}
			}

	// Get the schema type for the current node.
	[TODO]
	public Object SchemaType
			{
				get
				{
					// TODO
					return null;
				}
			}

	// Get or set the validation type.
	public ValidationType ValidationType
			{
				get
				{
					return validationType;
				}
				set
				{
					if(value < ValidationType.None ||
					   value > ValidationType.Schema)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("Xml_InvalidValidationType"));
					}
					validationType = value;
				}
			}

	// Get the text value of the current node.
	public override String Value
			{
				get
				{
					return reader.Value;
				}
			}

	// Get the current xml:lang scope.
	public override String XmlLang
			{
				get
				{
					return reader.XmlLang;
				}
			}

	// Set the resolver to use to resolve DTD references.
	public XmlResolver XmlResolver
			{
				set
				{
					reader.XmlResolver = value;
				}
			}

	// Get the current xml:space scope.
	public override XmlSpace XmlSpace
			{
				get
				{
					return reader.XmlSpace;
				}
			}

}; // class XmlValidatingReader

#endif // !ECMA_COMPAT

}; // namespace System.Xml
