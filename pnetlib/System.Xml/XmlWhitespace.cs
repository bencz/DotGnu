/*
 * XmlWhitespace.cs - Implementation of the "System.Xml.XmlWhitespace" class.
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
class XmlWhitespace : XmlCharacterData
{
	// Constructor.
	internal XmlWhitespace(XmlNode parent, String text)
			: base(parent, null)
			{
				// Set the whitespace via "Value" so that it will
				// be properly checked for whitespace characters.
				if(text != null)
				{
					Value = text;
				}
			}
	protected internal XmlWhitespace(String data, XmlDocument doc)
			: this(doc, data)
			{
				// Nothing to do here.
			}

	// Get the local name of this node.
	public override String LocalName
			{
				get
				{
					return "#whitespace";
				}
			}

	// Get the qualified name of this node.
	public override String Name
			{
				get
				{
					return "#whitespace";
				}
			}

	// Get the node type.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.Whitespace;
				}
			}

	// Get or set the value of this node.
	public override String Value
			{
				get
				{
					return Data;
				}
				set
				{
					if(value != null)
					{
						foreach(char ch in value)
						{
							if(!Char.IsWhiteSpace(ch))
							{
								throw new ArgumentException
									(S._("Xml_InvalidWhitespace"));
							}
						}
					}
					Data = value;
				}
			}

	// Clone this node.
	public override XmlNode CloneNode(bool deep)
			{
				return OwnerDocument.CreateWhitespace(Data);
			}

	// Writes the contents of this node to a specified XmlWriter.
	public override void WriteContentTo(XmlWriter w)
			{
				// Nothing needs to be done here for whitespace nodes.
			}

	// Write this node and all of its contents to a specified XmlWriter.
	public override void WriteTo(XmlWriter w)
			{
				w.WriteWhitespace(Data);
			}

}; // class XmlWhitespace

}; // namespace System.Xml
