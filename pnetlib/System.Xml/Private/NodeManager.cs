/*
 * NodeManager.cs - Implementation of the
 *		"System.Xml.Private.NodeManager" class.
 *
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

namespace System.Xml.Private
{

using System;
using System.Xml;

internal sealed class NodeManager
{
	// Internal state.
	private int current;
	private Attributes attributes;
	private ErrorHandler error;
	private NodeInfo[] nodes;
	private String xml;


	// Constructor.
	public NodeManager(XmlNameTable nt, ErrorHandler eh)
			{
				current = (int)Type.None;
				attributes = null;
				error = eh;
				nodes = new NodeInfo[11];
				nodes[current] = new DefaultNodeInfo();
				xml = nt.Add("xml");
			}


	// Get an attribute information node.
	public AttributeInfo Attribute
			{
				get
				{
					current = (int)Type.Attribute;
					if(nodes[current] == null)
					{
						nodes[current] = new AttributeInfo();
					}
					return (AttributeInfo)nodes[current];
				}
			}

	// Get or set an array of attribute information nodes.
	public Attributes Attributes
			{
				get
				{
					if(attributes == null)
					{
						attributes = new Attributes(error);
					}
					return attributes;
				}
				set { attributes = value; }
			}

	// Get a character data information node.
	public CDATAInfo CDATA
			{
				get
				{
					current = (int)Type.CDATA;
					if(nodes[current] == null)
					{
						nodes[current] = new CDATAInfo();
					}
					return (CDATAInfo)nodes[current];
				}
			}

	// Get a comment information node.
	public CommentInfo Comment
			{
				get
				{
					current = (int)Type.Comment;
					if(nodes[current] == null)
					{
						nodes[current] = new CommentInfo();
					}
					return (CommentInfo)nodes[current];
				}
			}

	// Get the current node information.
	public NodeInfo Current
			{
				get { return nodes[current]; }
			}

	// Get a doctype declaration information node.
	public DoctypeDeclarationInfo DoctypeDeclaration
			{
				get
				{
					current = (int)Type.DoctypeDeclaration;
					if(nodes[current] == null)
					{
						nodes[current] = new DoctypeDeclarationInfo();
					}
					return (DoctypeDeclarationInfo)nodes[current];
				}
			}

	// Get an element information node.
	public ElementInfo Element
			{
				get
				{
					current = (int)Type.Element;
					if(nodes[current] == null)
					{
						nodes[current] = new ElementInfo();
					}
					return (ElementInfo)nodes[current];
				}
			}

	// Get an end element information node.
	public EndElementInfo EndElement
			{
				get
				{
					current = (int)Type.EndElement;
					if(nodes[current] == null)
					{
						nodes[current] = new EndElementInfo();
					}
					return (EndElementInfo)nodes[current];
				}
			}

	// Get a processing instruction information node.
	public ProcessingInstructionInfo ProcessingInstruction
			{
				get
				{
					current = (int)Type.ProcessingInstruction;
					if(nodes[current] == null)
					{
						nodes[current] = new ProcessingInstructionInfo();
					}
					return (ProcessingInstructionInfo)nodes[current];
				}
			}

	// Get a text information node.
	public TextInfo Text
			{
				get
				{
					current = (int)Type.Text;
					if(nodes[current] == null)
					{
						nodes[current] = new TextInfo();
					}
					return (TextInfo)nodes[current];
				}
			}

	// Get a whitespace information node.
	public WhitespaceInfo Whitespace
			{
				get
				{
					current = (int)Type.Whitespace;
					if(nodes[current] == null)
					{
						nodes[current] = new WhitespaceInfo();
					}
					return (WhitespaceInfo)nodes[current];
				}
			}

	// Get an xml declaration information node.
	public XmlDeclarationInfo XmlDeclaration
			{
				get
				{
					current = (int)Type.XmlDeclaration;
					if(nodes[current] == null)
					{
						nodes[current] = new XmlDeclarationInfo(xml);
					}
					return (XmlDeclarationInfo)nodes[current];
				}
			}


	// Reset to the default state.
	public void Reset()
			{
				current = (int)Type.None;
			}









	private enum Type
	{
		None                  = 0x00,
		Attribute             = 0x01,
		CDATA                 = 0x02,
		Comment               = 0x03,
		DoctypeDeclaration    = 0x04,
		Element               = 0x05,
		EndElement            = 0x06,
		ProcessingInstruction = 0x07,
		Text                  = 0x08,
		Whitespace            = 0x09,
		XmlDeclaration        = 0x0A

	}; // enum Type

	private sealed class DefaultNodeInfo : NodeInfo
	{
		// Constructor.
		public DefaultNodeInfo()
				{
				}

	}; // class DefaultNodeInfo

}; // class NodeManager

}; // namespace System.Xml.Private
