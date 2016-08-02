/*
 * XmlNodeList.cs - Implementation of the "System.Xml.XmlNodeList" class.
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
using System.Collections;
using System.Runtime.CompilerServices;

#if ECMA_COMPAT
internal
#else
public
#endif
abstract class XmlNodeList : IEnumerable
{
	// Create a new node list.
	protected XmlNodeList() {}

	// Get the number of entries in the node list.
	public abstract int Count { get; }

	// Get a particular node in this node list.
	[IndexerName("ItemOf")]
	public virtual XmlNode this[int i]
			{
				get
				{
					return Item(i);
				}
			}

	// Get a particular item within this node list.
	public abstract XmlNode Item(int i);

	// Implement the "IEnumerable" interface.
	public abstract IEnumerator GetEnumerator();

}; // class XmlNodeList

}; // namespace System.Xml
