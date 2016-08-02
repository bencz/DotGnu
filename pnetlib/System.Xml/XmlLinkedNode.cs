/*
 * XmlLinkedNode.cs - Implementation of the "System.Xml.XmlLinkedNode" class.
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
abstract class XmlLinkedNode : XmlNode
{
	// Constructor.  Only accessible to internal subclasses.
	internal XmlLinkedNode(XmlNode parent)
			: base(parent)
			{
				// Nothing to do here.
			}


	// Get the next node immediately following this one.
	public override XmlNode NextSibling
			{
				get
				{
					return NodeList.GetNextSibling(this);
				}
			}

	// Get the previous sibling of this node.
	public override XmlNode PreviousSibling
			{
				get
				{
					return NodeList.GetPreviousSibling(this);
				}
			}

}; // class XmlElement

}; // namespace System.Xml
