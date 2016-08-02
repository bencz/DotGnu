/*
 * NodeWithAttributesInfo.cs - Implementation of the
 *		"System.Xml.Private.NodeWithAttributesInfo" class.
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

internal abstract class NodeWithAttributesInfo : NodeInfo
{
	// Internal state.
	protected int index;
	protected Attributes attributes;


	// Constructor.
	protected NodeWithAttributesInfo()
			{
				index = -1;
				attributes = null;
			}


	// Get the number of attributes.
	public override int AttributeCount
			{
				get { return attributes.Count; }
			}

	// Get the amount to add to the depth.
	public override int DepthOffset
			{
				get
				{
					if(index == -1) { return 0; }
					return 1 + attributes[index].DepthOffset;
				}
			}

	// Return an attribute with the given index.
	public override NodeInfo GetAttribute(int i)
			{
				if(i < 0 || i >= attributes.Count) { return null; }
				return attributes[i];
			}
	// Return an attribute with the given name and namespace uri.
	public override NodeInfo GetAttribute(String localName, String namespaceURI)
			{
				return GetAttribute(attributes.Find(localName, namespaceURI));
			}
	// Return an attribute with the given fully-qualified name.
	public override NodeInfo GetAttribute(String name)
			{
				return GetAttribute(attributes.Find(name));
			}

	// Move the position to an attribute with the given index.
	public override bool MoveToAttribute(int i)
			{
				if(i < 0 || i >= attributes.Count) { return false; }
				if(index != -1) { attributes[index].Reset(); }
				index = i;
				return true;
			}
	// Move the position to an attribute with the given name and namespace uri.
	public override bool MoveToAttribute(String localName, String namespaceURI)
			{
				return MoveToAttribute(attributes.Find(localName, namespaceURI));
			}
	// Move the position to an attribute with the given fully-qualified name.
	public override bool MoveToAttribute(String name)
			{
				return MoveToAttribute(attributes.Find(name));
			}

	// Move the position to before the first attribute.
	public override bool MoveToElement()
			{
				if(index == -1) { return false; }
				attributes[index].Reset();
				index = -1;
				return true;
			}

	// Move the position to the next attribute.
	public override bool MoveToNextAttribute()
			{
				return MoveToAttribute(index+1);
			}

	// Read the next attribute value in the input stream.
	public override bool ReadAttributeValue()
			{
				if(index == -1) { return false; }
				return attributes[index].Next();
			}

	// Reset the index and attribute information.
	protected void Reset(Attributes attributes)
			{
				this.index = -1;
				this.attributes = attributes;
			}

}; // class NodeWithAttributesInfo

}; // namespace System.Xml.Private
