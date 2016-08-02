/*
 * SegmentInfo.cs - Implementation of the
 *		"System.Xml.Private.SegmentInfo" class.
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

internal sealed class SegmentInfo : NodeInfo
{
	// Internal state.
	private bool text;
	private String data;


	// Constructor.
	public SegmentInfo()
			{
				text = false;
				data = null;
			}


	// Get the fully-qualified name.
	public override String Name
			{
				get
				{
					if(text) { return String.Empty; }
					return data;
				}
			}

	// Get the text value.
	public override String Value
			{
				get
				{
					if(text) { return data; }
					return String.Empty;
				}
			}

	// Get the type of the current node.
	public override XmlNodeType NodeType
			{
				get
				{
					if(text) { return XmlNodeType.Text; }
					return XmlNodeType.EntityReference;
				}
			}


	// Set the node information.
	public void SetInfo(bool text, String data)
			{
				this.text = text;
				this.data = data;
			}

}; // class SegmentInfo

}; // namespace System.Xml.Private
