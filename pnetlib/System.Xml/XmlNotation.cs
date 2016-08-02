/*
 * XmlNotation.cs - Implementation of the "System.Xml.XmlNotation" class.
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
class XmlNotation : XmlNode
{
	// Internal state.
	private String name;
	private String publicId;
	private String systemId;

	// Constructor.
	internal XmlNotation(XmlNode parent, String name,
					     String publicId, String systemId)
			: base(parent)
			{
				this.name = name;
				this.publicId = publicId;
				this.systemId = systemId;
			}

	// Get the markup that represents the children of this node.
	public override String InnerXml
			{
				get
				{
					return String.Empty;
				}
				set
				{
					throw new InvalidOperationException
						(S._("Xml_CannotSetInnerXml"));
				}
			}

	// Determine if this entity reference is read-only.
	public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

	// Get the local name associated with this node.
	public override String LocalName
			{
				get
				{
					return name;
				}
			}

	// Get the name associated with this node.
	public override String Name
			{
				get
				{
					return name;
				}
			}

	// Get the type that is associated with this node.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.Notation;
				}
			}

	// Get the XML markup representing this node and all of its children.
	public override String OuterXml
			{
				get
				{
					return String.Empty;
				}
			}

	// Get the public identifier for this notation.
	public String PublicId
			{
				get
				{
					return publicId;
				}
			}

	// Get the system identifier for this notation.
	public String SystemId
			{
				get
				{
					return systemId;
				}
			}

	// Clone this node in either shallow or deep mode.
	public override XmlNode CloneNode(bool deep)
			{
				throw new InvalidOperationException(S._("Xml_CannotClone"));
			}

	// Writes the contents of this node to a specified XmlWriter.
	public override void WriteContentTo(XmlWriter w)
			{
				// Nothing to do here.
			}

	// Write this node and all of its contents to a specified XmlWriter.
	public override void WriteTo(XmlWriter w)
			{
				// Nothing to do here.
			}

}; // class XmlNotation

}; // namespace System.Xml
