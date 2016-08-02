/*
 * ToolboxItemCollection.cs - Implementation of the
 *		"System.Drawing.Design.ToolboxItemCollection" class.
 *
 * Copyright (C) 2005  Deryk Robosson  <deryk@0x0a.com>
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

namespace System.Drawing.Design
{
#if CONFIG_COMPONENT_MODEL_DESIGN

using System.Collections;

public sealed class ToolboxItemCollection : ReadOnlyCollectionBase
{
	// Constructors
	public ToolboxItemCollection(ToolboxItem[] value)
	{
		InnerList.AddRange(value);
	}

	public ToolboxItemCollection(ToolboxItemCollection value)
	{
		InnerList.AddRange(value);
	}

	// Properties
	public ToolboxItem this[int index]
	{
		get
		{
			return (ToolboxItem)InnerList[index];
		}
	}

	// Methods

	// determine if collection contains provided item
	public bool Contains(ToolboxItem value)
	{
		return InnerList.Contains(value);
	}

	// copy collection to specified array beginning at destination index
	public void CopyTo(ToolboxItem[] array, int index)
	{
		InnerList.CopyTo(array, index);
	}

	// return the index of the provided item in the collection if it exists
	public int IndexOf(ToolboxItem value)
	{
		return InnerList.IndexOf(value);
	}

} // class ToolboxItemCollection
#endif
} // namespace System.Drawing.Design
