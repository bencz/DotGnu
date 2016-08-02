/*
 * XmlCDataSection.cs - Implementation of the
 *		"System.Xml.XmlCDataSection" class.
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
using System.Text;

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlCDataSection : XmlCharacterData
{
	// Constructor.
	internal XmlCDataSection(XmlNode parent, String data)
			: base(parent, data)
			{
				// Nothing to do here.
			}
	protected internal XmlCDataSection(String data, XmlDocument doc)
			: base(data, doc)
			{
				// Nothing to do here.
			}

	// Get the local name of this node.
	public override String LocalName
			{
				get
				{
					return "#cdata-section";
				}
			}

	// Get the qualified name of this node.
	public override String Name
			{
				get
				{
					return "#cdata-section";
				}
			}

	// Get the node type.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.CDATA;
				}
			}

	// Clone this node.
	public override XmlNode CloneNode(bool deep)
			{
				return OwnerDocument.CreateCDataSection(Data);
			}

	// Writes the contents of this node to a specified XmlWriter.
	public override void WriteContentTo(XmlWriter w)
			{
				// Nothing needs to be done here for CDATA nodes.
			}

	// Write this node and all of its contents to a specified XmlWriter.
	public override void WriteTo(XmlWriter w)
			{
				w.WriteCData(Data);
			}

}; // class XmlCDataSection

}; // namespace System.Xml
