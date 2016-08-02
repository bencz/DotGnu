/*
 * Segments.cs - Implementation of the "System.Xml.Private.Segments" class.
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
using System.Collections;

internal sealed class Segments
{
	// Internal state.
	private int count;
	private ArrayList segments;


	// Constructors.
	public Segments()
			{
				count = 0;
				segments = new ArrayList();
			}
	public Segments(int capacity)
			{
				count = 0;
				segments = new ArrayList(capacity);
			}


	// Get or set the count of segment information nodes.
	public int Count
			{
				get { return count; }
				set { count = value; }
			}

	// Get the segment information node at the given index.
	public SegmentInfo this[int index]
			{
				get
				{
					if(index >= segments.Count)
					{
						segments.Add(new SegmentInfo());
					}
					return (SegmentInfo)segments[index];
				}
			}


	// Reset the segment information.
	public void Reset()
			{
				count = 0;
			}

}; // class Segments

}; // namespace System.Xml.Private
