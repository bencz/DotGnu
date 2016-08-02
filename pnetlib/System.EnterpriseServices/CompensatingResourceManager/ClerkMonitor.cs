/*
 * ClerkMonitor.cs - Implementation of the
 *			"System.EnterpriseServices.CompensatingResourceManager."
 *			"ClerkMonitor" class.
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

namespace System.EnterpriseServices.CompensatingResourceManager
{

using System.Collections;

public sealed class ClerkMonitor : IEnumerable
{
	// Internal state.
	private Hashtable list;

	// Constructor.
	public ClerkMonitor()
			{
				list = new Hashtable();
			}

	// Destructor.
	~ClerkMonitor()
			{
				// Nothing to do in this implementation.
			}

	// Get the number of clerks within this monitor.
	public int Count
			{
				get
				{
					return list.Count;
				}
			}

	// Get a clerk at a particular position.
	public ClerkInfo this[String index]
			{
				get
				{
					return (ClerkInfo)(list[index]);
				}
			}
	public ClerkInfo this[int index]
			{
				get
				{
					return (ClerkInfo)(list[index]);
				}
			}

	// Populate the monitor from the system.
	public void Populate()
			{
				// Nothing to do in this implementation.
			}

	// Enumerate over the elements in this collection.
	public IEnumerator GetEnumerator()
			{
				return new ClerkMonitorEnumerator(list.GetEnumerator());
			}

	// Enumerator for this class.
	private sealed class ClerkMonitorEnumerator : IEnumerator
	{
		// Internal state.
		private IDictionaryEnumerator e;

		// Constructor.
		public ClerkMonitorEnumerator(IDictionaryEnumerator e)
				{
					this.e = e;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					return e.MoveNext();
				}
		public void Reset()
				{
					e.Reset();
				}
		public Object Current
				{
					get
					{
						return e.Value;
					}
				}

	}; // class ClerkMonitorEnumerator

}; // class ClerkMonitor

}; // namespace System.EnterpriseServices.CompensatingResourceManager
