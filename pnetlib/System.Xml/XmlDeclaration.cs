/*
 * XmlDeclaration.cs - Implementation of the
 *		"System.Xml.XmlDeclaration" class.
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

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlDeclaration : XmlLinkedNode
{
	// Internal state.
	private String encoding;
	private String standalone;
	private XmlAttributeCollection attributes;

	// Constructor.
	internal XmlDeclaration(XmlNode parent, String version,
							String encoding, String standalone)
			: base(parent)
			{
				Encoding = encoding;
				Standalone = standalone;
				if(version != "1.0")
				{
					throw new ArgumentException
						(S._("Xml_InvalidVersion"), "version");
				}
				this.attributes = null;
			}
	protected internal XmlDeclaration(String version, String encoding,
									  String standalone, XmlDocument doc)
			: this(doc, version, encoding, standalone)
			{
				// Nothing to do here.
			}

	// Get or set the document encoding.
	public String Encoding
			{
				get
				{
					return encoding;
				}
				set
				{
					if(value != null)
					{
						encoding = value;
					}
					else
					{
						encoding = String.Empty;
					}
				}
			}

	// Get or set the inner text.
	public override String InnerText
			{
				get
				{
					StringBuilder builder = new StringBuilder();
					builder.Append("version=\"");
					builder.Append(Version);
					builder.Append("\"");
					if(Encoding != String.Empty)
					{
						builder.Append(" encoding=\"");
						builder.Append(Encoding);
						builder.Append("\"");
					}
					if(Standalone != String.Empty)
					{
						builder.Append(" standalone=\"");
						builder.Append(Standalone);
						builder.Append("\"");
					}
					return builder.ToString();
				}
				set
				{
					// find the document root
					XmlDocument doc = FindOwnerQuick();

					// get the name table
					XmlNameTable nt;
					if(doc == null)
					{
						nt = new NameTable();
					}
					else
					{
						nt = ((XmlDocument)doc).implementation.nameTable;
					}

					// create the reader
					XmlTextReader r = new XmlTextReader
						(new StringReader("<?xml "+value+"?>"), nt);

					// read the value
					r.Read();

					// read the version attribute
					if(r["version"] != "1.0")
					{
						throw new ArgumentException
							(S._("Xml_InvalidVersion"), "value");
					}

					// read the optional attributes
					Encoding = r["encoding"];
					Standalone = r["standalone"];
				}
			}

	// Get the local name of this node.
	public override String LocalName
			{
				get
				{
					return "xml";
				}
			}

	// Get the qualified name of this node.
	public override String Name
			{
				get
				{
					return "xml";
				}
			}

	// Get the type that is associated with this node.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.XmlDeclaration;
				}
			}

	// Get or set the standalone property of the document.
	public String Standalone
			{
				get
				{
					return standalone;
				}
				set
				{
					if(value == null)
					{
						standalone = String.Empty;
					}
					else if(value == String.Empty || value == "yes" ||
							value == "no")
					{
						standalone = value;
					}
					else
					{
						throw new ArgumentException
							(S._("Xml_InvalidStandalone"), "value");
					}
				}
			}

	// Get or set the value of this node.
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

	// Get the XML document version.
	public String Version
			{
				get
				{
					return "1.0";
				}
			}

	// Clone this node.
	public override XmlNode CloneNode(bool deep)
			{
				return OwnerDocument.CreateXmlDeclaration
						("1.0", encoding, standalone);
			}

	// Writes the contents of this node to a specified XmlWriter.
	public override void WriteContentTo(XmlWriter w)
			{
				// Nothing needs to be done here for text nodes.
			}

	// Write this node and all of its contents to a specified XmlWriter.
	public override void WriteTo(XmlWriter w)
			{
				w.WriteProcessingInstruction(Name, Value);
			}

	// Get and set special attributes on this node.
	internal override String GetSpecialAttribute(String name)
			{
				if(name == "version")
				{
					return Version;
				}
				else if(name == "encoding")
				{
					return Encoding;
				}
				else if(name == "standalone")
				{
					return Standalone;
				}
				else
				{
					return String.Empty;
				}
			}
	internal override void SetSpecialAttribute(String name, String value)
			{
				if(name == "version")
				{
					if(value != "1.0")
					{
						throw new ArgumentException
							(S._("Xml_InvalidVersion"), "value");
					}
				}
				else if(name == "encoding")
				{
					Encoding = value;
				}
				else if(name == "standalone")
				{
					Standalone = value;
				}
				else
				{
					throw new ArgumentException
						(S._("Xml_InvalidSpecialAttribute"), "name");
				}
			}

	// Get the internal attribute collection for this node.
	internal override XmlAttributeCollection AttributesInternal
			{
				get
				{
					if(attributes == null)
					{
						attributes = new XmlAttributeCollection(this);
						attributes.Append
							(new XmlSpecialAttribute(this, "version"));
						attributes.Append
							(new XmlSpecialAttribute(this, "encoding"));
						attributes.Append
							(new XmlSpecialAttribute(this, "standalone"));
					}
					return attributes;
				}
			}

}; // class XmlDeclaration

}; // namespace System.Xml
