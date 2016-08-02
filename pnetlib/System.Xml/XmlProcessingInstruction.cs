/*
 * XmlProcessingInstruction.cs - Implementation of the
 *		"System.Xml.XmlProcessingInstruction" class.
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
class XmlProcessingInstruction : XmlLinkedNode
{
	// Internal state.
	private String target;
	private String data;

	// Constructor.
	internal XmlProcessingInstruction(XmlNode parent, String target,
									  String data)
			: base(parent)
			{
				this.target = ((target != null) ? target : String.Empty);
				this.data = data;
			}
	protected internal XmlProcessingInstruction(String target, String data,
												XmlDocument doc)
			: this(doc, target, data)
			{
				// Nothing to do here.
			}

	// Get or set the data associated with a processing instruction.
	public String Data
			{
				get
				{
					return data;
				}
				set
				{
					XmlNodeChangedEventArgs args;
					args = EmitBefore(XmlNodeChangedAction.Change);
					data = value;
					EmitAfter(args);
				}
			}

	// Get or set the inner text associated with this processing instruction.
	public override String InnerText
			{
				get
				{
					return Data;
				}
				set
				{
					Data = value;
				}
			}

	// Get the local name associated with this node.
	public override String LocalName
			{
				get
				{
					return target;
				}
			}

	// Get the name associated with this node.
	public override String Name
			{
				get
				{
					return target;
				}
			}

	// Get the type that is associated with this node.
	public override XmlNodeType NodeType
			{
				get
				{
					return XmlNodeType.ProcessingInstruction;
				}
			}

	// Get the target associated with this processing instruction.
	public String Target
			{
				get
				{
					return target;
				}
			}

	// Get or set the value associated with this node.
	public override String Value
			{
				get
				{
					return Data;
				}
				set
				{
					Data = value;
				}
			}

	// Clone this node in either shallow or deep mode.
	public override XmlNode CloneNode(bool deep)
			{
				return OwnerDocument.CreateProcessingInstruction(target, data);
			}

	// Writes the contents of this node to a specified XmlWriter.
	public override void WriteContentTo(XmlWriter w)
			{
				// Nothing to do here for processing instructions.
			}

	// Write this node and all of its contents to a specified XmlWriter.
	public override void WriteTo(XmlWriter w)
			{
				w.WriteProcessingInstruction(target, data);
			}

}; // class XmlProcessingInstruction

}; // namespace System.Xml
