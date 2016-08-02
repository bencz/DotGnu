/*
 * InstanceDataCollectionCollection.cs - Implementation of the
 *			"System.Diagnostics.InstanceDataCollectionCollection" class.
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

public class InstanceDataCollectionCollection : DictionaryBase
{
	// Constructor.
	public InstanceDataCollectionCollection() : base() {}

	// Get the counter information associated with a particular instance.
	public InstanceDataCollection this[String counterName]
			{
				get
				{
					return (InstanceDataCollection)
						(((IDictionary)this)[counterName]);
				}
			}

	// Get the list of keys within this collection.
	public ICollection Keys
			{
				get
				{
					return ((IDictionary)this).Keys;
				}
			}

	// Get the list of values within this collection.
	public ICollection Values
			{
				get
				{
					return ((IDictionary)this).Values;
				}
			}

	// Determine if this collection contains a specific instance.
	public bool Contains(String counterName)
			{
				return ((IDictionary)this).Contains(counterName);
			}

	// Copy the contents of this collection to an array.
	public void CopyTo(InstanceDataCollection[] array, int index)
			{
				((IDictionary)this).CopyTo(array, index);
			}

}; // class InstanceDataCollectionCollection

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
