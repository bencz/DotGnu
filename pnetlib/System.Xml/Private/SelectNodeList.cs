/*
 * SelectNodeList.cs - Implementation of the 
 *			"System.Xml.Private.SelectNodeList" class.
 *
 * Copyright (C) 2002-2004 Southern Storm Software, Pty Ltd.
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

#if CONFIG_XPATH

using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

internal sealed class SelectNodeList : XmlNodeList
{
	// Internal state.
	internal XPathNodeIterator iterator;
	internal XPathNodeIterator current;
	internal ArrayList cached;
	private int count = 0;
	private bool finished;

	// Create a new node list.
	public SelectNodeList(XPathNodeIterator iterator)
			{
				if(iterator != null)
				{
					this.iterator = iterator.Clone();
					this.current = iterator.Clone();
					finished = false;
				}
				else
				{
					finished = true;
				}
				cached = new ArrayList();
			}

	// Get the number of entries in the node list.
	public override int Count
			{
				get
				{
					if(!finished)
					{
						Read(Int32.MaxValue);
					}
					return count;
				}
			}

	// Get a particular item within this node list.
	public override XmlNode Item(int i)
			{
				if(count > i)
				{
					return (XmlNode)(cached[i]);
				}
				else if(!finished)
				{
					Read(i-count+1);
					if(count > i)
					{
						return (XmlNode)(cached[i]);
					}
				}
				
				return null;
			}

	// Implement the "IEnumerable" interface.
	public override IEnumerator GetEnumerator()
			{
				if(!finished)
				{
					Read(Int32.MaxValue);
				}
				
				/* Reset has problems if we read it one by one */
				return cached.GetEnumerator();
			}

	// read n more entries
	private void Read(int n)
			{
				while(!finished && n != 0)
				{
					if(current.MoveNext())
					{
						XmlDocumentNavigator navigator = 
									((current.Current) as XmlDocumentNavigator);
						cached.Add(navigator.CurrentNode);
						count++;
					}
					else
					{
						finished = true;
						break;
					}
					n--;
				}
			}
}; // class SelectNodeList

#endif // CONFIG_XPATH

}; // namespace System.Xml.Private
