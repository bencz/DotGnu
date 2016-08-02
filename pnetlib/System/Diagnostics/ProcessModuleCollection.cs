/*
 * ProcessModuleCollection.cs - Implementation of the
 *			"System.Diagnostics.ProcessModuleCollection" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Diagnostics
{

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.Collections;

public class ProcessModuleCollection : ReadOnlyCollectionBase
{
	// Constructor.
	protected ProcessModuleCollection() {}
	public ProcessModuleCollection(ProcessModule[] processModules)
			{
				InnerList.AddRange(processModules);
			}
	internal ProcessModuleCollection(ProcessModule module)
			{
				InnerList.Add(module);
			}

	// Get collection member.
	public ProcessModule this[int index]
			{
				get
				{
					return (ProcessModule)(InnerList[index]);
				}
			}

	// Add an element to this collection.
	internal void Add(ProcessModule value)
			{
				InnerList.Add(value);
			}

	// Determine if an item exists in this collection.
	public bool Contains(ProcessModule value)
			{
				return InnerList.Contains(value);
			}

	// Copy the elements in this collection to an array.
	public void CopyTo(ProcessModule[] array, int index)
			{
				InnerList.CopyTo(array, index);
			}

	// Get the index of a specific element in this collection.
	public int IndexOf(ProcessModule value)
			{
				return InnerList.IndexOf(value);
			}

}; // class ProcessModuleCollection

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
