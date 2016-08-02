/*
 * XmlWriter.cs - Implementation of the "System.Xml.XmlWriter" class.
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

public abstract class XmlWriter
{
	// Constructor.
	protected XmlWriter() {}

	// Close this writer and free all resources.
	public abstract void Close();

	// Flush the buffers that are used by this writer.
	public abstract void Flush();

	// Look up the namespace URI for a specified namespace prefix.
	public abstract String LookupPrefix(String ns);

	// Write an attribute string with a local name and namespace.
	public void WriteAttributeString(String localName, String ns, String value)
			{
				if(((Object)localName) == null || localName.Length == 0)
				{
					throw new ArgumentNullException("localName");
				}
				WriteStartAttribute(null, localName, ns);
				WriteString(value);
				WriteEndAttribute();
			}

	// Write an attribute string with a local name.
	public void WriteAttributeString(String localName, String value)
			{
				if(((Object)localName) == null || localName.Length == 0)
				{
					throw new ArgumentNullException("localName");
				}
				WriteStartAttribute(null, localName, null);
				WriteString(value);
				WriteEndAttribute();
			}

	// Write an attribute string with prefix, local name, and namespace.
	public void WriteAttributeString
				(String prefix, String localName, String ns, String value)
			{
				// get the length of local name
				int len = (((Object)localName) == null ? 0 : localName.Length);

				// ensure we have a local name
				if(len == 0) { throw new ArgumentNullException("localName"); }

				// write the start of the attribute
				WriteStartAttribute(prefix, localName, ns);

				// write the attribute value
				WriteString(value);

				// write the end of the attribute
				WriteEndAttribute();
			}

	// Write the attributes at the current position of an XmlReader.
	public virtual void WriteAttributes(XmlReader reader, bool defattr)
			{
				// Validate the parameters.
				if(reader == null)
				{
					throw new ArgumentNullException("reader");
				}

				// Determine if we are on an element/xmldecl node or
				// on an attribute.  If we are on an element/xmldecl,
				// then we need to reset at the end of the output.
				XmlNodeType type = reader.NodeType;
				bool reset;
				if(type == XmlNodeType.Element ||
				   type == XmlNodeType.XmlDeclaration)
				{
					if(!reader.MoveToFirstAttribute())
					{
						return;
					}
					reset = true;
				}
				else if(type != XmlNodeType.Attribute)
				{
					throw new XmlException(S._("Xml_IncorrectNode"));
				}
				else
				{
					reset = false;
				}

				// Output the attributes in order.
				do
				{
					if(defattr || !(reader.IsDefault))
					{
						WriteStartAttribute(reader.Prefix, reader.LocalName,
											reader.NamespaceURI);
						while(reader.ReadAttributeValue())
						{
							if(reader.NodeType == XmlNodeType.EntityReference)
							{
								WriteEntityRef(reader.Name);
							}
							else
							{
								WriteString(reader.Value);
							}
						}
						WriteEndAttribute();
					}
				}
				while(reader.MoveToNextAttribute());

				// Move back to the element if we started on one.
				if(reset)
				{
					reader.MoveToElement();
				}
			}

	// Encode an array as base64 and write it out as text.
	public abstract void WriteBase64(byte[] buffer, int index, int count);

	// Encode an array as BinHex and write it out as text.
	public abstract void WriteBinHex(byte[] buffer, int index, int count);

	// Write out a CDATA block.
	public abstract void WriteCData(String text);

	// Write a character entity.
	public abstract void WriteCharEntity(char ch);

	// Write a text buffer.
	public abstract void WriteChars(char[] buffer, int index, int count);

	// Write a comment.
	public abstract void WriteComment(String text);

	// Write a document type specification.
	public abstract void WriteDocType(String name, String pubid,
									  String sysid, String subset);

	// Write an element with a specified name and value.
	public void WriteElementString(String localName, String value)
			{
				WriteElementString(localName, null, value);
			}

	// Write an element with a specified name, namespace, and value.
	public void WriteElementString(String localName, String ns, String value)
			{
				WriteStartElement(localName, ns);
				if(((Object)value) != null && value.Length != 0)
				{
					WriteString(value);
				}
				WriteEndElement();
			}

	// Write the end of an attribute.
	public abstract void WriteEndAttribute();

	// Write the document end information and reset to the start state.
	public abstract void WriteEndDocument();

	// Write the end of an element and pop the namespace scope.
	public abstract void WriteEndElement();

	// Write an entity reference.
	public abstract void WriteEntityRef(String name);

	// Write a full end element tag, even if there is no content.
	public abstract void WriteFullEndElement();

	// Write a name, as long as it is XML-compliant.
	public abstract void WriteName(String name);

	// Write a name token, as long as it is XML-compliant.
	public abstract void WriteNmToken(String name);

	// Write the contents of a node from an XmlReader.
	public virtual void WriteNode(XmlReader reader, bool defattr)
			{
				// Validate the parameters.
				if(reader == null)
				{
					throw new ArgumentNullException("reader");
				}

				// Loop until we have finished with the current node.
				uint level = 0;
				do
				{
					switch(reader.NodeType)
					{
						case XmlNodeType.Attribute:
						{
							// Write this attribute only - we only get here
							// if we started on an attribute.
							if(defattr || !(reader.IsDefault))
							{
								WriteStartAttribute
									(reader.Prefix, reader.LocalName,
									 reader.NamespaceURI);
								while(reader.ReadAttributeValue())
								{
									if(reader.NodeType ==
											XmlNodeType.EntityReference)
									{
										WriteEntityRef(reader.Name);
									}
									else
									{
										WriteString(reader.Value);
									}
								}
								WriteEndAttribute();
							}
							reader.MoveToNextAttribute();
							return;
						}
						// Not reached.

						case XmlNodeType.CDATA:
						{
							WriteCData(reader.Value);
						}
						break;

						case XmlNodeType.Comment:
						{
							WriteComment(reader.Value);
						}
						break;

						case XmlNodeType.DocumentType:
						{
							WriteDocType
								(reader.Name, reader["PUBLIC"],
								 reader["SYSTEM"], reader.Value);
						}
						break;

						case XmlNodeType.Element:
						{
							// Write the starting information for the element.
							WriteStartElement
								(reader.Prefix, reader.LocalName,
								 reader.NamespaceURI);

							// Write all of the element attributes.
							WriteAttributes(reader, defattr);

							// End the element, or go in a level.
							if(reader.IsEmptyElement)
							{
								WriteEndElement();
							}
							else
							{
								++level;
							}
						}
						break;

						case XmlNodeType.EndElement:
						{
							WriteFullEndElement();
							--level;
						}
						break;

						case XmlNodeType.EntityReference:
						{
							WriteEntityRef(reader.Name);
						}
						break;

						case XmlNodeType.None:
						{
							// We are probably at EOF, so bail out.
							return;
						}
						// Not reached.

						case XmlNodeType.ProcessingInstruction:
						case XmlNodeType.XmlDeclaration:
						{
							WriteProcessingInstruction
								(reader.Name, reader.Value);
						}
						break;

						case XmlNodeType.SignificantWhitespace:
						case XmlNodeType.Whitespace:
						{
							WriteWhitespace(reader.Value);
						}
						break;

						case XmlNodeType.Text:
						{
							WriteString(reader.Value);
						}
						break;

						default: break;
					}
				}
				while(reader.Read() && level > 0);
			}

	// Write a processing instruction.
	public abstract void WriteProcessingInstruction(String name, String text);

	// Write a qualified name.
	public abstract void WriteQualifiedName(String localName, String ns);

	// Write raw string data.
	public abstract void WriteRaw(String data);

	// Write raw data from an array.
	public abstract void WriteRaw(char[] buffer, int index, int count);

	// Write the start of an attribute with a full name.
	public abstract void WriteStartAttribute(String prefix, String localName,
										     String ns);

	// Write the start of an attribute with a local name and namespace.
	public void WriteStartAttribute(String localName, String ns)
			{
				WriteStartAttribute(null, localName, ns);
			}
	
	// Write the start of an XML document.
	public abstract void WriteStartDocument(bool standalone);

	// Write the start of an XML document with no standalone attribute.
	public abstract void WriteStartDocument();

	// Write the start of an element with a full name.
	public abstract void WriteStartElement(String prefix, String localName,
										   String ns);

	// Write the start of an element with a local name and namespace.
	public void WriteStartElement(String localName, String ns)
			{
				WriteStartElement(null, localName, ns);
			}

	// Write the start of an element with a local name.
	public void WriteStartElement(String localName)
			{
				WriteStartElement(null, localName, null);
			}

	// Write a string.
	public abstract void WriteString(String text);

	// Write a surrogate character entity.
	public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);

	// Write a sequence of white space.
	public abstract void WriteWhitespace(String ws);

	// Get the current write state.
	public abstract WriteState WriteState { get; }

	// Get the xml:lang attribute.
	public abstract String XmlLang { get; }

	// Get the xml:space attribute.
	public abstract XmlSpace XmlSpace { get; }

}; // class XmlWriter

}; // namespace System.Xml
