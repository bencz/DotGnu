/*
 * WhitespaceInfo.cs - Implementation of the
 *		"System.Xml.Private.WhitespaceInfo" class.
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

internal sealed class WhitespaceInfo : NodeInfo
{
	// Internal state.
	private bool significant;
	private String value;

	// Constructor.
	public WhitespaceInfo()
			{
				significant = false;
				value = null;
			}


	// Get the text value.
	public override String Value
			{
				get { return value; }
			}

	// Get the type of the current node.
	public override XmlNodeType NodeType
			{
				get
				{
					if(!significant) { return XmlNodeType.Whitespace; }
					return XmlNodeType.SignificantWhitespace;
				}
			}


	// Set the node information.
	public void SetInfo(bool significant, String value)
			{
				this.significant = significant;
				this.value = value;
			}

}; // class WhitespaceInfo

}; // namespace System.Xml.Private
