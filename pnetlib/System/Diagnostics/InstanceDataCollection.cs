/*
 * InstanceDataCollection.cs - Implementation of the
 *			"System.Diagnostics.InstanceDataCollection" class.
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

public class InstanceDataCollection : DictionaryBase
{
	// Internal state.
	private String counterName;

	// Constructor.
	public InstanceDataCollection(String counterName)
			{
				if(counterName == null)
				{
					throw new ArgumentNullException("counterName");
				}
				this.counterName = counterName;
			}

	// Get the name of this counter.
	public String CounterName
			{
				get
				{
					return counterName;
				}
			}

	// Get the counter information associated with a particular instance.
	public InstanceData this[String instanceName]
			{
				get
				{
					return (InstanceData)(((IDictionary)this)[instanceName]);
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
	public bool Contains(String instanceName)
			{
				return ((IDictionary)this).Contains(instanceName);
			}

	// Copy the contents of this collection to an array.
	public void CopyTo(InstanceData[] array, int index)
			{
				((IDictionary)this).CopyTo(array, index);
			}

}; // class InstanceDataCollection

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
