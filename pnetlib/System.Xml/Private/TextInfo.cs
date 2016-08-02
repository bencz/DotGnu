/*
 * TextInfo.cs - Implementation of the "System.Xml.Private.TextInfo" class.
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

internal sealed class TextInfo : IterableNodeInfo
{
	// Internal state.
	private int index;
	private Segments segments;


	// Constructors.
	public TextInfo()
			{
				this.index = 0;
				this.segments = new Segments();
			}
	public TextInfo(int capacity)
			{
				this.index = 0;
				this.segments = new Segments(capacity);
			}


	// Get the local name.
	public override String LocalName
			{
				get { return segments[index].LocalName; }
			}

	// Get the fully-qualified name.
	public override String Name
			{
				get { return segments[index].Name; }
			}

	// Get the segments array.
	public Segments Segments
			{
				get { return segments; }
			}

	// Get the text value.
	public override String Value
			{
				get { return segments[index].Value; }
			}

	// Get the type of the current node.
	public override XmlNodeType NodeType
			{
				get { return segments[index].NodeType; }
			}


	// Move to the next node in the iteration, returning false on end.
	public override bool Next()
			{
				if(index >= Segments.Count-1) { return false; }
				++index;
				return true;
			}

	// Reset the iteration.
	public override void Reset()
			{
				index = 0;
			}

	// Set the node information.
	public void SetInfo(Segments segments)
			{
				this.index = 0;
				this.segments = segments;
			}

}; // class TextInfo

}; // namespace System.Xml.Private
