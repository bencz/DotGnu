/*
 * TraceListenerCollection.cs - Implementation of the
 *			"System.Diagnostics.TraceListenerCollection" class.
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

#if !ECMA_COMPAT

using System.Collections;

public class TraceListenerCollection : IList, ICollection, IEnumerable
{
	// Internal state.
	private ArrayList list;

	// Constructor.
	internal TraceListenerCollection()
			{
				list = new ArrayList();
			}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				lock(this)
				{
					list.CopyTo(array, index);
				}
			}
	public int Count
			{
				get
				{
					lock(this)
					{
						return list.Count;
					}
				}
			}
	bool ICollection.IsSynchronized
			{
				get
				{
					return true;
				}
			}
	Object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Implement the IList interface.
	int IList.Add(Object value)
			{
				lock(this)
				{
					return list.Add((TraceListener)value);
				}
			}
	public void Clear()
			{
				lock(this)
				{
					list.Clear();
				}
			}
	bool IList.Contains(Object value)
			{
				lock(this)
				{
					return list.Contains(value);
				}
			}
	int IList.IndexOf(Object value)
			{
				lock(this)
				{
					return list.IndexOf(value);
				}
			}
	void IList.Insert(int index, Object value)
			{
				lock(this)
				{
					list.Insert(index, (TraceListener)value);
				}
			}
	void IList.Remove(Object value)
			{
				lock(this)
				{
					list.Remove(value);
				}
			}
	public void RemoveAt(int index)
			{
				lock(this)
				{
					list.RemoveAt(index);
				}
			}
	bool IList.IsFixedSize
			{
				get
				{
					return false;
				}
			}
	bool IList.IsReadOnly
			{
				get
				{
					return false;
				}
			}
	Object IList.this[int index]
			{
				get
				{
					lock(this)
					{
						return list[index];
					}
				}
				set
				{
					lock(this)
					{
						list[index] = (TraceListener)value;
					}
				}
			}

	// Implement the IEnumerable interface.
	public IEnumerator GetEnumerator()
			{
				lock(this)
				{
					// Clone the list and enumerate that so that modifications
					// to the trace listener list are not reflected in the
					// enumeration elements, as per the specification.
					TraceListener[] listeners = new TraceListener [list.Count];
					list.CopyTo(listeners, 0);
					return listeners.GetEnumerator();
				}
			}

	// Clone the global trace settings onto a new listener.
	private static void CloneSettings(TraceListener listener)
			{
				listener.IndentLevel = Trace.IndentLevel;
				listener.IndentSize = Trace.IndentSize;
			}

	// Get or set a particular element in this collection.
	public TraceListener this[int index]
			{
				get
				{
					lock(this)
					{
						return (TraceListener)(list[index]);
					}
				}
				set
				{
					CloneSettings(value);
					lock(this)
					{
						list[index] = value;
					}
				}
			}

	// Get the trace listener with a specific name.
	public TraceListener this[String name]
			{
				get
				{
					lock(this)
					{
						foreach(TraceListener listener in list)
						{
							if(listener.Name == name)
							{
								return listener;
							}
						}
						return null;
					}
				}
			}

	// Add a trace listener to this collection.
	public int Add(TraceListener listener)
			{
				CloneSettings(listener);
				lock(this)
				{
					return list.Add(listener);
				}
			}

	// Add a range of trace listeners to this collection.
	public void AddRange(TraceListener[] value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(TraceListener listener in value)
				{
					Add(listener);
				}
			}
	public void AddRange(TraceListenerCollection value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(TraceListener listener in value)
				{
					Add(listener);
				}
			}

	// Determine if the list contains a specific listener.
	public bool Contains(TraceListener listener)
			{
				lock(this)
				{
					return list.Contains(listener);
				}
			}

	// Copy the contents of this collection to an array.
	public void CopyTo(TraceListener[] listeners, int index)
			{
				lock(this)
				{
					list.CopyTo(listeners, index);
				}
			}

	// Get the index of a specific listener.
	public int IndexOf(TraceListener listener)
			{
				lock(this)
				{
					return list.IndexOf(listener);
				}
			}

	// Insert a listener into this collection.
	public void Insert(int index, TraceListener listener)
			{
				lock(this)
				{
					list.Insert(index, listener);
				}
			}

	// Remove a specific trace listener.
	public void Remove(TraceListener listener)
			{
				lock(this)
				{
					list.Remove(listener);
				}
			}
	public void Remove(String name)
			{
				lock(this)
				{
					int posn = 0;
					foreach(TraceListener listener in list)
					{
						if(listener.Name == name)
						{
							list.RemoveAt(posn);
							break;
						}
						++posn;
					}
				}
			}

}; // class TraceListenerCollection

#endif // !ECMA_COMPAT

}; // namespace System.Diagnostics
