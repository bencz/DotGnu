/*
 * XmlDocumentType.cs - Implementation of the
 *		"System.Xml.XmlDocumentType" class.
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
class XmlDocumentType : XmlLinkedNode
{
	// Internal state.
	private String name;
	private String publicId;
	private String systemId;
	private String internalSubset;
	private XmlNamedNodeMap entities;
	private XmlNamedNodeMap notations;
	private XmlAttributeCollection attributes;

	// Constructor.
	internal XmlDocumentType(XmlNode parent, String name, String publicId,
							 String systemId, String internalSubset)
			: base(parent)
			{
				XmlNameTable nt = parent.FindOwnerQuick().NameTable;
				this.name = name;
				this.publicId = publicId;
				this.systemId = systemId;
				this.internalSubset = internalSubset;
				entities = new XmlNamedNodeMap(this);
				notations = new XmlNamedNodeMap(this);
				attributes = null;
			}
	protected internal XmlDocumentType(String name, String publicId,
							 		   String systemId, String internalSubset,
									   XmlDocument doc)
			: this(doc, name, publicId, systemId, internalSubset)
			{
				// Nothing to do here.
			}

	// Get the entity list for this document type.
	public XmlNamedNodeMap Entities
			{
				get
				{
					return entities;
				}
			}

	// Get the internal subset information for this document type.
	public String InternalSubset
			{
				get
				{
					return internalSubset;
				}
			}

	// Determine if this node is read-only.
	public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

	// Get the local name of the document type.
	public override String LocalName
			{
				get
				{
					return name;
				}
			}

	// Get the qualified name of the document type.
	public override String Name
			{
				get
				{
					return name;
				}
			}

	// Get the type of this node.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.DocumentType;
				}
			}

	// Get the notation list for this document type.
	public XmlNamedNodeMap Notations
			{
				get
				{
					return notations;
				}
			}

	// Get the public identifier for this document type.
	public String PublicId
			{
				get
				{
					return publicId;
				}
			}

	// Get the system identifier for this document type.
	public String SystemId
			{
				get
				{
					return systemId;
				}
			}

	// Clone this node.
	public override XmlNode CloneNode(bool deep)
			{
				return OwnerDocument.CreateDocumentType
					(name, publicId, systemId, internalSubset);
			}

	// Writes the contents of this node to a specified XmlWriter.
	public override void WriteContentTo(XmlWriter w)
			{
				// Nothing needs to be done here for DTD nodes.
			}

	// Write this node and all of its contents to a specified XmlWriter.
	public override void WriteTo(XmlWriter w)
			{
				w.WriteDocType(name, publicId, systemId, internalSubset);
			}

	// Get and set special attributes on this node.
	internal override String GetSpecialAttribute(String name)
			{
				if(name == "PUBLIC")
				{
					return PublicId;
				}
				else if(name == "SYSTEM")
				{
					return SystemId;
				}
				else
				{
					return String.Empty;
				}
			}
	internal override void SetSpecialAttribute(String name, String value)
			{
				XmlNameTable nt = FindOwnerQuick().NameTable;
				if(name == "PUBLIC")
				{
					publicId = ((value != null) ? nt.Add(value) : String.Empty);
				}
				else if(name == "SYSTEM")
				{
					systemId = ((value != null) ? nt.Add(value) : String.Empty);
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
							(new XmlSpecialAttribute(this, "PUBLIC"));
						attributes.Append
							(new XmlSpecialAttribute(this, "SYSTEM"));
					}
					return attributes;
				}
			}

}; // class XmlDocumentType

}; // namespace System.Xml
