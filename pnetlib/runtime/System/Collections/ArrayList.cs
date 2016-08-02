/*
 * ArrayList.cs - Implementation of the "System.Collections.ArrayList" class.
 *
 * Copyright (C) 2001, 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Collections
{

using System;
using System.Private;

public class ArrayList : ICloneable, ICollection, IEnumerable, IList
{
	// Internal state.
	private int count;
	private Object[] store;
	private int generation;

	// Simple constructors.
	public ArrayList()
			{
				count = 0;
				store = new Object [16];
				generation = 0;
			}
	public ArrayList(int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_NonNegative"));
				}
				count = 0;
				store = new Object [capacity];
				generation = 0;
			}

	// Construct an array list from the contents of a collection.
	public ArrayList(ICollection c)
			{
				if(c == null)
				{
					throw new ArgumentNullException("c");
				}
				count = 0;
				store = new Object[c.Count];
				this.InsertRange( count, c );
			}

	// Reallocate the array to accomodate "n" new entries at "index".
	// This leaves "count" unchanged.
	private void Realloc(int n, int index)
			{
				if((count + n) <= store.Length)
				{
					// The current capacity is sufficient, so just
					// shift the contents of the array upwards.
					int posn = count - 1;
					while(posn >= index)
					{
						store[posn + n] = store[posn];
						store[posn] = null; // Brubbel: set the new free entry to zero
						--posn;
					}
				}
				else
				{
					// We need to allocate a new array.
					int newCapacity = (((count + n) + 31) & ~31);
					int newCapacity2 = count * 2;
					if(newCapacity2 > newCapacity)
					{
						newCapacity = newCapacity2;
					}
					Object[] newStore = new Object [newCapacity];
					if(index != 0)
					{
						Array.Copy(store, 0, newStore, 0, index);
					}
					if(count != index)
					{
						Array.Copy(store, index, newStore, index + n,
								   count - index);
					}
					store = newStore;
				}
			}

	// Delete "n" entries from the list at "index".
	// This modifies "count".
	private void Delete(int n, int index)
			{
				while((index + n) <= count) // brubbel : must be <=
				{
					if( (index + n) == count ) {
						store[index] = null;
					}
					else {
						store[index] = store[index + n];
						store[index + n] = null; // brubbel : set the new free entries to zero to avoid mem leaks
					}
					++index;
				}
				count -= n;
			}

	// Implement the IList interface.
	public virtual int Add(Object value)
			{
				if(count >= store.Length)
				{
					Realloc(1, count);
				}
				store[count] = value;
				++generation;
				return count++;
			}
	public virtual void Clear()
			{
				Array.Clear(store, 0, count);
				count = 0;
				++generation;
			}
	public virtual bool Contains(Object item)
			{
				int index;
				if(item != null)
				{
					for(index = 0; index < count; ++index)
					{
						if(item.Equals(store[index]))
						{
							return true;
						}
					}
					return false;
				}
				else
				{
					for(index = 0; index < count; ++index)
					{
						if(store[index] == null)
						{
							return true;
						}
					}
					return false;
				}
			}
	public virtual int IndexOf(Object value)
			{
				if(Count <= 0) return -1;

				return IndexOf(value, 0, Count);
			}
	public virtual void Insert(int index, Object value)
			{
				if(index < 0 || index > count)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				Realloc(1, index);
				store[index] = value;
				++count;
				++generation;
			}
	public virtual void Remove(Object value)
			{
				int index = Array.IndexOf(store, value, 0, count);
				if(index != -1)
				{
					Delete(1, index);
					++generation;
				}
			}
	public virtual void RemoveAt(int index)
			{
				if(index < 0 || index > count)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				Delete(1, index);
				++generation;
			}
	public virtual bool IsFixedSize
			{
				get
				{
					return false;
				}
			}
	public virtual bool IsReadOnly
			{
				get
				{
					return false;
				}
			}
	public virtual Object this[int index]
			{
				get
				{
					if(index < 0 || index >= count)
					{
						throw new ArgumentOutOfRangeException
							("index", _("ArgRange_Array"));
					}
					return store[index];
				}
				set
				{
					if(index < 0 || index >= count)
					{
						throw new ArgumentOutOfRangeException
							("index", _("ArgRange_Array"));
					}
					store[index] = value;
					++generation;
				}
			}

	// Add the contents of a collection as a range.
	public virtual void AddRange(ICollection c)
			{
				// use nsert range to ensure the SyncRoot check
				this.InsertRange( count, c );
			}

	// Insert the contents of a collection as a range.
	public virtual void InsertRange(int index, ICollection c)
			{
				if(c == null)
				{
					throw new ArgumentNullException("c");
				}
				if(index < 0 || index > count)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				int cCount = c.Count;
				
				if( cCount > 0 ) {
					if((count + cCount) > store.Length)
					{
						Realloc(cCount, count);
					}
					if( index < this.count ) {
						Array.Copy( this.store, index, this.store, index+cCount, this.count - index ); 
					}
					if( c.SyncRoot == this.SyncRoot ) {
						Array.Copy( this.store, 0, this.store, index, index );
						Array.Copy( this.store, index+cCount, this.store, 2*index, this.count - index );
					}
					else {
						c.CopyTo( this.store, index );
					}
					this.count += cCount;
					++generation;
				}
			}

	// Remove a range of elements from an array list.
	public virtual void RemoveRange(int index, int count)
			{
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((this.count - index) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				Delete(count, index);
				++generation;
			}

	// Perform a binary search on an array list.
	public virtual int BinarySearch(Object value)
			{
				return BinarySearch(0, Count, value, null);
			}
	public virtual int BinarySearch(Object value, IComparer comparer)
			{
				return BinarySearch(0, Count, value, comparer);
			}
	public virtual int BinarySearch(int index, int count,
								    Object value, IComparer comparer)
			{
				// Validate the arguments.
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((Count - index) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}

				// Perform the binary search.
				int left, right, middle, cmp;
				Object elem;
				IComparable icmp;
				left = index;
				right = index + count - 1;
				while(left <= right)
				{
					middle = (left + right) / 2;
					elem = this[middle];
					if(elem != null && value != null)
					{
						if(comparer != null)
						{
							cmp = comparer.Compare(elem, value);
						}
						else if((icmp = (elem as IComparable)) != null)
						{
							cmp = -(icmp.CompareTo(value));
						}
						else if((icmp = (value as IComparable)) != null)
						{
							cmp = icmp.CompareTo(elem);
						}
						else
						{
							throw new ArgumentException
								(_("Arg_SearchCompare"));
						}
					}
					else if(elem != null)
					{
						cmp = -1;
					}
					else if(value != null)
					{
						cmp = 1;
					}
					else
					{
						cmp = 0;
					}
					if(cmp == 0)
					{
						return middle;
					}
					else if(cmp < 0)
					{
						right = middle - 1;
					}
					else
					{
						left = middle + 1;
					}
				}
				return ~left;
			}

	// Implement the ICloneable interface.
	public virtual Object Clone()
			{
				ArrayList clone = new ArrayList(count);
				clone.count = count;
				clone.generation = generation;
				Array.Copy(store, 0, clone.store, 0, count);
				return clone;
			}

	// Implement the ICollection interface.
	public virtual void CopyTo(Array array, int arrayIndex)
			{
				// addidtonal checks to throw the correct exception
				if(array == null)
				{
					throw new ArgumentNullException("array");
				}
				if(array.Rank > 1)
				{
					throw new ArgumentException("array", _("Arg_RankMustBe1"));
				}
				Array.Copy(store, 0, array, arrayIndex, count);
			}
	public virtual int Count
			{
				get
				{
					return count;
				}
			}
	public virtual bool IsSynchronized
			{
				get
				{
					return false;
				}
			}
	public virtual Object SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Copy from this array list to another array.
	public virtual void CopyTo(Array array)
			{
				Array.Copy(store, 0, array, 0, count);
			}
	public virtual void CopyTo(int index, Array array,
							   int arrayIndex, int count)
			{
				// Validate the parameters.
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((Count - index) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}

				// Perform the copy.
				if(GetType() == typeof(ArrayList))
				{
					// We can use a short-cut because we know that
					// the list elements are in "store".
					Array.Copy(store, index, array, arrayIndex, count);
				}
				else
				{
					// The list elements may be elsewhere.
					while(count > 0)
					{
						array.SetValue(this[index], arrayIndex);
						++index;
						++arrayIndex;
						--count;
					}
				}
			}

	// Get the index of a value within an array list.
	public virtual int IndexOf(Object value, int startIndex)
			{
				int count = Count;
				if(startIndex < 0 || startIndex >= count)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				return IndexOf(value, startIndex, count - startIndex);
			}
	public virtual int IndexOf(Object value, int startIndex, int count)
			{
				// Validate the parameters.
				int thisCount = Count;
				if(startIndex < 0 || startIndex >= thisCount)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((thisCount - startIndex) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}

				// Perform the search.
				Object elem;
				while(count > 0)
				{
					elem = this[startIndex];
					if(elem != null && value != null)
					{
						if(value.Equals(elem))
						{
							return startIndex;
						}
					}
					else if(elem == value && value == null)
					{
						return startIndex;
					}
					++startIndex;
					--count;
				}
				return -1;
			}

	// Get the last index of a value within an array list.
	public virtual int LastIndexOf(Object value)
			{
				int count = Count;
				return LastIndexOf(value, count - 1, count);
			}
	public virtual int LastIndexOf(Object value, int startIndex)
			{
				int count = Count;
				if(startIndex < 0 || startIndex >= count)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				return LastIndexOf(value, startIndex, startIndex + 1);
			}
	public virtual int LastIndexOf(Object value, int startIndex, int count)
			{
				// Validate the parameters.
				if(startIndex < 0 || startIndex >= Count)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if(count > (startIndex + 1))
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}

				// Perform the search.
				Object elem;
				while(count > 0)
				{
					elem = this[startIndex];
					if(elem != null && value != null)
					{
						if(value.Equals(elem))
						{
							return startIndex;
						}
					}
					else if(elem == value && value == null)
					{
						return startIndex;
					}
					--startIndex;
					--count;
				}
				return -1;
			}

	// Construct an array list with repeated copies of the same element.
	public static ArrayList Repeat(Object value, int count)
			{
				ArrayList list;
				int index;
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_NonNegative"));
				}
				if(count < 16)
				{
					list = new ArrayList();
				}
				else
				{
					list = new ArrayList(count);
				}
				list.Realloc(count, 0);
				for(index = 0; index < count; ++index)
				{
					list.store[index] = value;
				}
				list.count = count;
				return list;
			}

	// Reverse the contents of this array list.
	public virtual void Reverse()
			{
				Array.Reverse(store, 0, count);
				++generation;
			}
	public virtual void Reverse(int index, int count)
			{
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((this.count - index) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				Array.Reverse(store, index, count);
				++generation;
			}

	// Set a range of array list elements to the members of a collection.
	public virtual void SetRange(int index, ICollection c)
			{
				if(c == null)
				{
					throw new ArgumentNullException("c");
				}
				int cCount = c.Count;
				if( (index < 0) || (index > (this.count - cCount ) ) ) {
					throw new ArgumentOutOfRangeException
							("index", _("ArgRange_Array"));
				}
				
				if( cCount > 0 ) {
					c.CopyTo( this.store, index );
					++generation;
				}
			}

	// Inner version of "Sort".
	private void InnerSort(int lower, int upper, IComparer comparer)
			{
				int i, j;
				Object pivot, temp;
				if((upper - lower) < 1)
				{
					// Zero or one elements - this partition is already sorted.
					return;
				}
				do
				{
					// Pick the middle of the range as the pivot value.
					i = lower;
					j = upper;
					pivot = this[i + ((j - i) / 2)];
		
					// Partition the range.
					do
					{
						// Find two values to be swapped.
						while(comparer.Compare(this[i], pivot) < 0)
						{
							++i;
						}
						while(comparer.Compare(this[j], pivot) > 0)
						{
							--j;
						}
						if(i > j)
						{
							break;
						}
		
						// Swap the values.
						if(i < j)
						{
							temp = this[i];
							this[i] = this[j];
							this[j] = temp;
						}
						++i;
						--j;
					}
					while(i <= j);
		
					// Sort the partitions.
					if((j - lower) <= (upper - i))
					{
						if(lower < j)
						{
							InnerSort(lower, j, comparer);
						}
						lower = i;
					}
					else
					{
						if(i < upper)
						{
							InnerSort(i, upper, comparer);
						}
						upper = j;
					}
				}
				while(lower < upper);
			}

	// Sort the contents of this array list.
	public virtual void Sort()
			{
				InnerSort(0, Count - 1, Comparer.Default);
			}
	public virtual void Sort(IComparer comparer)
			{
				if(comparer == null)
				{
					comparer = Comparer.Default;
				}
				InnerSort(0, Count - 1, comparer);
			}
	public virtual void Sort(int index, int count, IComparer comparer)
			{
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((Count - index) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				if(comparer == null)
				{
					comparer = Comparer.Default;
				}
				InnerSort(index, index + count - 1, comparer);
			}

	// Create an array that contains the elements of this array list.
	public virtual Object[] ToArray()
			{
				int count = Count;
				Object[] array = new Object[count];
				int index;
				for(index = 0; index < count; ++index)
				{
					array[index] = this[index];
				}
				return array;
			}
	public virtual Array ToArray(Type type)
			{
				int count = Count;
				Array array = Array.CreateInstance(type, count);
				int index;
				for(index = 0; index < count; ++index)
				{
					array.SetValue(this[index], index);
				}
				return array;
			}

	// Trim the array list to its actual size.
	public virtual void TrimToSize()
			{
				if(count != 0)
				{
					if(count != store.Length)
					{
						Object[] newStore = new Object[count];
						int index;
						for(index = 0; index < count; ++index)
						{
							newStore[index] = store[index];
						}
						store = newStore;
					}
				}
				else if(store.Length != 16)
				{
					store = new Object[16];
				}
				++generation;
			}

	// Get or set the current capacity of the array list.
	public virtual int Capacity
			{
				get
				{
					return store.Length;
				}
				set
				{
					if(value <= 0)
					{
						value = 16;
					}
					if(value < count)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_CannotReduceCapacity"));
					}
					if(value != store.Length)
					{
						Object[] newStore = new Object[value];
						int index;
						for(index = 0; index < count; ++index)
						{
							newStore[index] = store[index];
						}
						store = newStore;
					}
					++generation;
				}
			}

	// Get an enumerator for this array list.
	public virtual IEnumerator GetEnumerator()
			{
				return new ArrayListEnumerator(this, 0, Count);
			}
	public virtual IEnumerator GetEnumerator(int index, int count)
			{
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((Count - index) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				return new ArrayListEnumerator(this, index, index + count);
			}

	// Array list enumerator class.
	private class ArrayListEnumerator : IEnumerator
	{
		private ArrayList list;
		private int start;
		private int finish;
		private int position;
		private int generation;

		public ArrayListEnumerator(ArrayList list, int start, int finish)
				{
					this.list = list;
					this.start = start;
					this.finish = finish;
					position = start - 1;
					generation = list.generation;
				}

		public bool MoveNext()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					++position;
					return (position < finish);
				}

		public void Reset()
				{
					position = start - 1;
				}

		public Object Current
				{
					get
					{
						if(generation != list.generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						else if(position < start || position >= finish)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return list[position];
					}
				}

	}; // class ArrayListEnumerator

	// Adapt an IList to appear to look like an ArrayList.
	public static ArrayList Adapter(IList list)
		{
			if(list == null)
			{
				throw new ArgumentNullException("list");
			}
			else if(list is ArrayList)
			{
				return (ArrayList)list;
			}
			else
			{
				return new IListWrapper(list);
			}
		}

	// Wrapper class for IList.
	private class IListWrapper : ArrayList
	{

		// Internal state.
		private IList list;

		// Constructor
		public IListWrapper(IList list)
				{
					this.list = list;
				}

		// Implement the IList interface.
		public override int Add(Object value)
				{
					return list.Add(value);
				}
		public override void Clear()
				{
					list.Clear();
				}
		public override bool Contains(Object item)
				{
					return list.Contains(item);
				}
		public override int IndexOf(Object value)
				{
					return list.IndexOf(value);
				}
		public override void Insert(int index, Object value)
				{
					list.Insert(index, value);
				}
		public override void Remove(Object value)
				{
					list.Remove(value);
				}
		public override void RemoveAt(int index)
				{
					list.RemoveAt(index);
				}
		public override bool IsFixedSize
				{
					get
					{
						return list.IsFixedSize;
					}
				}
		public override bool IsReadOnly
				{
					get
					{
						return list.IsReadOnly;
					}
				}
		public override Object this[int index]
				{
					get
					{
						return list[index];
					}
					set
					{
						list[index] = value;
					}
				}

		// Implement the ICloneable interface.
		public override Object Clone()
				{
					return new IListWrapper(list);
				}

		// Range-related methods.
		public override void AddRange(ICollection c)
				{
					if(c == null)
					{
						throw new ArgumentNullException("c");
					}
					IEnumerator enumerator = c.GetEnumerator();
					while(enumerator.MoveNext())
					{
						list.Add(enumerator.Current);
					}
				}
		public override void InsertRange(int index, ICollection c)
				{
					if(c == null)
					{
						throw new ArgumentNullException("c");
					}
					IEnumerator enumerator = c.GetEnumerator();
					while(enumerator.MoveNext())
					{
						list.Insert(index++, enumerator.Current);
					}
				}
		public override void RemoveRange(int index, int count)
				{
					if(index < 0 || index >= list.Count)
					{
						throw new ArgumentOutOfRangeException
							("index", _("ArgRange_Array"));
					}
					if(count < 0)
					{
						throw new ArgumentOutOfRangeException
							("count", _("ArgRange_Array"));
					}
					while(count > 0)
					{
						list.RemoveAt(index);
						--count;
					}
				}
		public override void SetRange(int index, ICollection c)
				{
					if(c == null)
					{
						throw new ArgumentNullException("c");
					}
					IEnumerator enumerator = c.GetEnumerator();
					while(enumerator.MoveNext())
					{
						list[index++] = enumerator.Current;
					}
				}

		// Implement the ICollection interface.
		public override void CopyTo(Array array, int arrayIndex)
				{
					list.CopyTo(array, arrayIndex);
				}
		public override int Count
				{
					get
					{
						return list.Count;
					}
				}
		public override bool IsSynchronized
				{
					get
					{
						return list.IsSynchronized;
					}
				}
		public override Object SyncRoot
				{
					get
					{
						return list.SyncRoot;
					}
				}

		// Copy from this array list to another array.
		public override void CopyTo(Array array)
				{
					list.CopyTo(array, 0);
				}

		// Reverse the contents of this array list.
		public override void Reverse()
				{
					Reverse(0, Count);
				}
		public override void Reverse(int index, int count)
				{
					// Validate the parameters.
					if(index < 0)
					{
						throw new ArgumentOutOfRangeException
							("index", _("ArgRange_Array"));
					}
					if(count < 0)
					{
						throw new ArgumentOutOfRangeException
							("count", _("ArgRange_Array"));
					}
					if((Count - index) < count)
					{
						throw new ArgumentException(_("Arg_InvalidArrayRange"));
					}

					// Perform the reversal.
					Object temp;
					int lower = index;
					int upper = index + count - 1;
					while(lower < upper)
					{
						temp = list[lower];
						list[lower] = list[upper];
						list[upper] = temp;
						++lower;
						--upper;
					}
				}

		// Trim the array list to its actual size.
		public override void TrimToSize()
				{
					// Nothing can be done here.
				}

		// Get or set the current capacity of the array list.
		public override int Capacity
				{
					get
					{
						// Return the IList's count as the capacity.
						return list.Count;
					}
					set
					{
						// IList does not have a capacity, so just validate.
						if(value < list.Count)
						{
							throw new ArgumentOutOfRangeException
								("value", _("Arg_CannotReduceCapacity"));
						}
					}
				}

	}; // class IListWrapper

	// Pass-through wrapper class that encapsulates another array list.
	private class PassThroughWrapper : ArrayList
	{
		protected ArrayList list;

		public PassThroughWrapper(ArrayList list)
				{
					this.list = list;
				}

		// Implement the IList interface.
		public override int Add(Object value)
				{
					return list.Add(value);
				}
		public override void Clear()
				{
					list.Clear();
				}
		public override bool Contains(Object item)
				{
					return list.Contains(item);
				}
		public override int IndexOf(Object value)
				{
					return list.IndexOf(value);
				}
		public override void Insert(int index, Object value)
				{
					list.Insert(index, value);
				}
		public override void Remove(Object value)
				{
					list.Remove(value);
				}
		public override void RemoveAt(int index)
				{
					list.RemoveAt(index);
				}
		public override bool IsFixedSize
				{
					get
					{
						return list.IsFixedSize;
					}
				}
		public override bool IsReadOnly
				{
					get
					{
						return list.IsReadOnly;
					}
				}
		public override Object this[int index]
				{
					get
					{
						return list[index];
					}
					set
					{
						list[index] = value;
					}
				}

		// Range-related methods.
		public override void AddRange(ICollection c)
				{
					list.AddRange(c);
				}
		public override void InsertRange(int index, ICollection c)
				{
					list.InsertRange(index, c);
				}
		public override void RemoveRange(int index, int count)
				{
					list.RemoveRange(index, count);
				}
		public override void SetRange(int index, ICollection c)
				{
					list.SetRange(index, c);
				}

		// Searching methods.
		public override int BinarySearch(Object value)
				{
					return list.BinarySearch(value);
				}
		public override int BinarySearch(Object value, IComparer comparer)
				{
					return list.BinarySearch(value, comparer);
				}
		public override int BinarySearch(int index, int count,
									    Object value, IComparer comparer)
				{
					return list.BinarySearch(index, count, value, comparer);
				}
		public override int IndexOf(Object value, int startIndex)
				{
					return list.IndexOf(value, startIndex);
				}
		public override int IndexOf(Object value, int startIndex, int count)
				{
					return list.IndexOf(value, startIndex, count);
				}
		public override int LastIndexOf(Object value)
				{
					return list.LastIndexOf(value);
				}
		public override int LastIndexOf(Object value, int startIndex)
				{
					return list.LastIndexOf(value, startIndex);
				}
		public override int LastIndexOf(Object value, int startIndex, int count)
				{
					return list.LastIndexOf(value, startIndex, count);
				}

		// Implement the ICollection interface.
		public override void CopyTo(Array array, int arrayIndex)
				{
					list.CopyTo(array, arrayIndex);
				}
		public override int Count
				{
					get
					{
						return list.count;
					}
				}
		public override bool IsSynchronized
				{
					get
					{
						return list.IsSynchronized;
					}
				}
		public override Object SyncRoot
				{
					get
					{
						return list.SyncRoot;
					}
				}

		// Copy from this array list to another array.
		public override void CopyTo(Array array)
				{
					list.CopyTo(array);
				}
		public override void CopyTo(int index, Array array,
							   	   int arrayIndex, int count)
				{
					list.CopyTo(index, array, arrayIndex, count);
				}

		// Reverse the contents of this array list.
		public override void Reverse()
				{
					list.Reverse();
				}
		public override void Reverse(int index, int count)
				{
					list.Reverse(index, count);
				}

		// Sort the contents of this array list.
		public override void Sort()
				{
					list.Sort();
				}
		public override void Sort(IComparer comparer)
				{
					list.Sort(comparer);
				}
		public override void Sort(int index, int count, IComparer comparer)
				{
					list.Sort(index, count, comparer);
				}

		// Create an array that contains the elements of this array list.
		public override Object[] ToArray()
				{
					return list.ToArray();
				}
		public override Array ToArray(Type type)
				{
					return list.ToArray(type);
				}

		// Trim the array list to its actual size.
		public override void TrimToSize()
				{
					list.TrimToSize();
				}

		// Get or set the current capacity of the array list.
		public override int Capacity
				{
					get
					{
						return list.Capacity;
					}
					set
					{
						list.Capacity = value;
					}
				}

		// Get an enumerator for this array list.
		public override IEnumerator GetEnumerator()
				{
					return list.GetEnumerator();
				}
		public override IEnumerator GetEnumerator(int index, int count)
				{
					return list.GetEnumerator(index, count);
				}

	}; // class PassThroughWrapper

	// Adapt an array list to appear to have a fixed size.
	public static ArrayList FixedSize(ArrayList list)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				else if(list.IsFixedSize)
				{
					return list;
				}
				else
				{
					return new FixedSizeWrapper(list);
				}
			}

	// Wrapper class for fixed size lists.
	private class FixedSizeWrapper : PassThroughWrapper
	{
		public FixedSizeWrapper(ArrayList list)
				: base(list)
				{
				}

		// Implement the IList interface.
		public override int Add(Object value)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public override void Clear()
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public override void Insert(int index, Object value)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public override void Remove(Object value)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public override void RemoveAt(int index)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public override bool IsFixedSize
				{
					get
					{
						return true;
					}
				}
		public override bool IsReadOnly
				{
					get
					{
						return list.IsReadOnly;
					}
				}
		public override Object this[int index]
				{
					get
					{
						return list[index];
					}
					set
					{
						list[index] = value;
					}
				}

		// Range-related methods.
		public override void AddRange(ICollection c)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public override void InsertRange(int index, ICollection c)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public override void RemoveRange(int index, int count)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}

		// Implement the ICloneable interface.
		public override Object Clone()
				{
					return new FixedSizeWrapper((ArrayList)(list.Clone()));
				}

		// Trim the array list to its actual size.
		public override void TrimToSize()
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}

	}; // class FixedSizeWrapper

#if !ECMA_COMPAT

	// Adapt an ordinary list to appear to have a fixed size.
	public static IList FixedSize(IList list)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				else
				{
					return new FixedSizeListWrapper(list);
				}
			}

	// Wrapper class for fixed-sized lists.
	private sealed class FixedSizeListWrapper : IList
	{
		// Internal state.
		private IList list;

		// Constructor.
		public FixedSizeListWrapper(IList list)
				{
					this.list = list;
				}

		// Implement the IList interface.
		public int Add(Object value)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public void Clear()
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public bool Contains(Object value)
				{
					return list.Contains(value);
				}
		public int  IndexOf(Object value)
				{
					return list.IndexOf(value);
				}
		public void Insert(int index, Object value)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public void Remove(Object value)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public void RemoveAt(int index)
				{
					throw new NotSupportedException
						(_("NotSupp_FixedSizeCollection"));
				}
		public bool IsFixedSize
				{
					get
					{
						return true;
					}
				}
		public bool IsReadOnly
				{
					get
					{
						return list.IsReadOnly;
					}
				}
		public Object this[int index]
				{
					get
					{
						return list[index];
					}
					set
					{
						list[index] = value;
					}
				}

		// Implement the ICollection interface.
		public void CopyTo(Array array, int index)
				{
					list.CopyTo(array, index);
				}
		public int Count
				{
					get
					{
						return list.Count;
					}
				}
		public bool IsSynchronized
				{
					get
					{
						return list.IsSynchronized;
					}
				}
		public Object SyncRoot
				{
					get
					{
						return list.SyncRoot;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return list.GetEnumerator();
				}

	}; // class FixedSizeListWrapper

#endif // !ECMA_COMPAT

	// Adapt an array list to get access to a sub-range.
	public virtual ArrayList GetRange(int index, int count)
			{
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((this.count - index) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				return new RangeWrapper(this, index, count);
			}

	// Wrapper class for sub-range array lists.
	private class RangeWrapper : ArrayList
	{
		private ArrayList list;
		private int index;
		private new int count;

		public RangeWrapper(ArrayList list, int index, int count)
				: base(list)
				{
					this.list  = list;
					this.index = index;
					this.count = count;
					generation = list.generation;
				}

		// Determine if the underlying array list has been changed.
		private void UnderlyingCheck()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_UnderlyingModified"));
					}
				}

		// Implement the IList interface.
		public override int Add(Object value)
				{
					UnderlyingCheck();
					list.Insert(index + count, value);
					generation = list.generation;
					return index + count;
				}
		public override void Clear()
				{
					UnderlyingCheck();
					list.Clear();
					generation = list.generation;
				}
		public override bool Contains(Object item)
				{
					UnderlyingCheck();
					return (list.IndexOf(item) != -1);
				}
		public override int IndexOf(Object value)
				{
					UnderlyingCheck();
					return list.IndexOf(value, index, count);
				}
		public override void Insert(int index, Object value)
				{
					UnderlyingCheck();
					if(index < 0 || index >= count)
					{
						throw new ArgumentOutOfRangeException
							("index", _("ArgRange_Array"));
					}
					list.Insert(index + this.index, value);
					generation = list.generation;
				}
		public override void Remove(Object value)
				{
					UnderlyingCheck();
					int ind = list.IndexOf(value, index, count);
					if(ind != -1)
					{
						list.RemoveAt(ind);
					}
					generation = list.generation;
				}
		public override void RemoveAt(int index)
				{
					UnderlyingCheck();
					list.RemoveAt(index + this.index);
					generation = list.generation;
				}
		public override bool IsFixedSize
				{
					get
					{
						return list.IsFixedSize;
					}
				}
		public override bool IsReadOnly
				{
					get
					{
						return list.IsReadOnly;
					}
				}
		public override Object this[int index]
				{
					get
					{
						UnderlyingCheck();
						if(index < 0 || index >= count)
						{
							throw new ArgumentOutOfRangeException
								("index", _("ArgRange_Array"));
						}
						return list[index + this.index];
					}
					set
					{
						UnderlyingCheck();
						if(index < 0 || index >= count)
						{
							throw new ArgumentOutOfRangeException
								("index", _("ArgRange_Array"));
						}
						list[index + this.index] = value;
						generation = list.generation;
					}
				}

		// Range-related methods.
		public override void AddRange(ICollection c)
				{
					UnderlyingCheck();
					list.InsertRange(index + count, c);
					generation = list.generation;
				}
		public override void InsertRange(int index, ICollection c)
				{
					UnderlyingCheck();
					list.InsertRange(index + this.index, c);
					generation = list.generation;
				}
		public override void RemoveRange(int index, int count)
				{
					UnderlyingCheck();
					list.RemoveRange(index + this.index, count);
					generation = list.generation;
				}
		public override void SetRange(int index, ICollection c)
				{
					UnderlyingCheck();
					list.SetRange(index + this.index, c);
					generation = list.generation;
				}

		// Implement the ICloneable interface.
		public override Object Clone()
				{
					return new RangeWrapper
						((ArrayList)(list.Clone()), index, count);
				}

		// Searching methods.
		public override int BinarySearch(Object value)
				{
					UnderlyingCheck();
					return list.BinarySearch
						(index, count, value, (IComparer)null);
				}
		public override int BinarySearch(Object value, IComparer comparer)
				{
					UnderlyingCheck();
					return list.BinarySearch(index, count, value, comparer);
				}
		public override int BinarySearch(int index, int count,
									     Object value, IComparer comparer)
				{
					UnderlyingCheck();
					return list.BinarySearch(index + this.index, count,
											 value, comparer);
				}
		public override int IndexOf(Object value, int startIndex)
				{
					UnderlyingCheck();
					return list.IndexOf(value, startIndex);
				}
		public override int IndexOf(Object value, int startIndex, int count)
				{
					UnderlyingCheck();
					return list.IndexOf(value, startIndex, count);
				}
		public override int LastIndexOf(Object value)
				{
					UnderlyingCheck();
					return list.LastIndexOf(value);
				}
		public override int LastIndexOf(Object value, int startIndex)
				{
					UnderlyingCheck();
					return list.LastIndexOf(value, startIndex);
				}
		public override int LastIndexOf(Object value, int startIndex, int count)
				{
					UnderlyingCheck();
					return list.LastIndexOf(value, startIndex, count);
				}

		// Implement the ICollection interface.
		public override void CopyTo(Array array, int arrayIndex)
				{
					list.CopyTo(array, arrayIndex);
				}
		public override int Count
				{
					get
					{
						UnderlyingCheck();
						return count;
					}
				}
		public override bool IsSynchronized
				{
					get
					{
						return list.IsSynchronized;
					}
				}
		public override Object SyncRoot
				{
					get
					{
						return list.SyncRoot;
					}
				}

		// Copy from this array list to another array.
		public override void CopyTo(Array array)
				{
					list.CopyTo(array);
				}
		public override void CopyTo(int index, Array array,
							   	   int arrayIndex, int count)
				{
					list.CopyTo(index, array, arrayIndex, count);
				}

		// Reverse the contents of this array list.
		public override void Reverse()
				{
					list.Reverse();
				}
		public override void Reverse(int index, int count)
				{
					list.Reverse(index, count);
				}

		// Sort the contents of this array list.
		public override void Sort()
				{
					list.Sort();
				}
		public override void Sort(IComparer comparer)
				{
					list.Sort(comparer);
				}
		public override void Sort(int index, int count, IComparer comparer)
				{
					list.Sort(index, count, comparer);
				}

		// Create an array that contains the elements of this array list.
		public override Object[] ToArray()
				{
					return list.ToArray();
				}
		public override Array ToArray(Type type)
				{
					return list.ToArray(type);
				}

		// Trim the array list to its actual size.
		public override void TrimToSize()
				{
					list.TrimToSize();
				}

		// Get or set the current capacity of the array list.
		public override int Capacity
				{
					get
					{
						return list.Capacity;
					}
					set
					{
						list.Capacity = value;
					}
				}

		// Get an enumerator for this array list.
		public override IEnumerator GetEnumerator()
				{
					return list.GetEnumerator();
				}
		public override IEnumerator GetEnumerator(int index, int count)
				{
					return list.GetEnumerator(index, count);
				}

	}; // class RangeWrapper

	// Adapt an array list to appear to be read-only.
	public static ArrayList ReadOnly(ArrayList list)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				else if(list.IsReadOnly)
				{
					return list;
				}
				else
				{
					return new ReadOnlyWrapper(list);
				}
			}

	// Wrapper class for read-only lists.
	private class ReadOnlyWrapper : PassThroughWrapper
	{
		public ReadOnlyWrapper(ArrayList list)
				: base(list)
				{
				}

		// Implement the IList interface.
		public override int Add(Object value)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void Clear()
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void Insert(int index, Object value)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void Remove(Object value)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void RemoveAt(int index)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override bool IsReadOnly
				{
					get
					{
						return true;
					}
				}
		public override Object this[int index]
				{
					get
					{
						return list[index];
					}
					set
					{
						throw new NotSupportedException(_("NotSupp_ReadOnly"));
					}
				}

		// Range-related methods.
		public override void AddRange(ICollection c)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void InsertRange(int index, ICollection c)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void RemoveRange(int index, int count)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void SetRange(int index, ICollection c)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}

		// Implement the ICloneable interface.
		public override Object Clone()
				{
					return new ReadOnlyWrapper((ArrayList)(list.Clone()));
				}

		// Reverse the contents of this array list.
		public override void Reverse()
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void Reverse(int index, int count)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}

		// Sort the contents of this array list.
		public override void Sort()
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void Sort(IComparer comparer)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}
		public override void Sort(int index, int count, IComparer comparer)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}

		// Trim the array list to its actual size.
		public override void TrimToSize()
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}

		// Get or set the current capacity of the array list.
		public override int Capacity
				{
					get
					{
						return list.Capacity;
					}
					set
					{
						throw new NotSupportedException(_("NotSupp_ReadOnly"));
					}
				}

	}; // class ReadOnlyWrapper

#if !ECMA_COMPAT

	// Adapt an ordinary list to appear to be read-only.
	public static IList ReadOnly(IList list)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				else
				{
					return new ReadOnlyListWrapper(list);
				}
			}

	// Wrapper class for read-only lists.
	private sealed class ReadOnlyListWrapper : IList
	{
		// Internal state.
		private IList list;

		// Constructor.
		public ReadOnlyListWrapper(IList list)
				{
					this.list = list;
				}

		// Implement the IList interface.
		public int Add(Object value)
				{
					throw new NotSupportedException
						(_("NotSupp_ReadOnly"));
				}
		public void Clear()
				{
					throw new NotSupportedException
						(_("NotSupp_ReadOnly"));
				}
		public bool Contains(Object value)
				{
					return list.Contains(value);
				}
		public int  IndexOf(Object value)
				{
					return list.IndexOf(value);
				}
		public void Insert(int index, Object value)
				{
					throw new NotSupportedException
						(_("NotSupp_ReadOnly"));
				}
		public void Remove(Object value)
				{
					throw new NotSupportedException
						(_("NotSupp_ReadOnly"));
				}
		public void RemoveAt(int index)
				{
					throw new NotSupportedException
						(_("NotSupp_ReadOnly"));
				}
		public bool IsFixedSize
				{
					get
					{
						return list.IsFixedSize;
					}
				}
		public bool IsReadOnly
				{
					get
					{
						return true;
					}
				}
		public Object this[int index]
				{
					get
					{
						return list[index];
					}
					set
					{
						throw new NotSupportedException
							(_("NotSupp_ReadOnly"));
					}
				}

		// Implement the ICollection interface.
		public void CopyTo(Array array, int index)
				{
					list.CopyTo(array, index);
				}
		public int Count
				{
					get
					{
						return list.Count;
					}
				}
		public bool IsSynchronized
				{
					get
					{
						return list.IsSynchronized;
					}
				}
		public Object SyncRoot
				{
					get
					{
						return list.SyncRoot;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return list.GetEnumerator();
				}

	}; // class ReadOnlyListWrapper

#endif // !ECMA_COMPAT

	// Adapt an array list to appear to be synchonrized
	public static ArrayList Synchronized(ArrayList list)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				else if(list.IsSynchronized)
				{
					return list;
				}
				else
				{
					return new SynchronizedWrapper(list);
				}
			}

	// Wrapper class for synchronized lists.
	private class SynchronizedWrapper : ArrayList
	{
		// Internal state.
		private ArrayList list;

		// Constructor.
		public SynchronizedWrapper(ArrayList list)
				{
					this.list = list;
				}

		// Implement the IList interface.
		public override int Add(Object value)
				{
					lock(SyncRoot)
					{
						return list.Add(value);
					}
				}
		public override void Clear()
				{
					lock(SyncRoot)
					{
						list.Clear();
					}
				}
		public override bool Contains(Object item)
				{
					lock(SyncRoot)
					{
						return list.Contains(item);
					}
				}
		public override int IndexOf(Object value)
				{
					lock(SyncRoot)
					{
						return list.IndexOf(value);
					}
				}
		public override void Insert(int index, Object value)
				{
					lock(SyncRoot)
					{
						list.Insert(index, value);
					}
				}
		public override void Remove(Object value)
				{
					lock(SyncRoot)
					{
						list.Remove(value);
					}
				}
		public override void RemoveAt(int index)
				{
					lock(SyncRoot)
					{
						list.RemoveAt(index);
					}
				}
		public override bool IsFixedSize
				{
					get
					{
						lock(SyncRoot)
						{
							return list.IsFixedSize;
						}
					}
				}
		public override bool IsReadOnly
				{
					get
					{
						lock(SyncRoot)
						{
							return list.IsReadOnly;
						}
					}
				}
		public override Object this[int index]
				{
					get
					{
						lock(SyncRoot)
						{
							return list[index];
						}
					}
					set
					{
						lock(SyncRoot)
						{
							list[index] = value;
						}
					}
				}

		// Range-related methods.
		public override void AddRange(ICollection c)
				{
					lock(SyncRoot)
					{
						list.AddRange(c);
					}
				}
		public override void InsertRange(int index, ICollection c)
				{
					lock(SyncRoot)
					{
						list.InsertRange(index, c);
					}
				}
		public override void RemoveRange(int index, int count)
				{
					lock(SyncRoot)
					{
						list.RemoveRange(index, count);
					}
				}
		public override void SetRange(int index, ICollection c)
				{
					lock(SyncRoot)
					{
						list.SetRange(index, c);
					}
				}

		// Searching methods.
		public override int BinarySearch(Object value)
				{
					lock(SyncRoot)
					{
						return list.BinarySearch(value);
					}
				}
		public override int BinarySearch(Object value, IComparer comparer)
				{
					lock(SyncRoot)
					{
						return list.BinarySearch(value, comparer);
					}
				}
		public override int BinarySearch(int index, int count,
									    Object value, IComparer comparer)
				{
					lock(SyncRoot)
					{
						return list.BinarySearch(index, count, value, comparer);
					}
				}
		public override int IndexOf(Object value, int startIndex)
				{
					lock(SyncRoot)
					{
						return list.IndexOf(value, startIndex);
					}
				}
		public override int IndexOf(Object value, int startIndex, int count)
				{
					lock(SyncRoot)
					{
						return list.IndexOf(value, startIndex, count);
					}
				}
		public override int LastIndexOf(Object value)
				{
					lock(SyncRoot)
					{
						return list.LastIndexOf(value);
					}
				}
		public override int LastIndexOf(Object value, int startIndex)
				{
					lock(SyncRoot)
					{
						return list.LastIndexOf(value, startIndex);
					}
				}
		public override int LastIndexOf(Object value, int startIndex, int count)
				{
					lock(SyncRoot)
					{
						return list.LastIndexOf(value, startIndex, count);
					}
				}

		// Implement the ICollection interface.
		public override void CopyTo(Array array, int arrayIndex)
				{
					lock(SyncRoot)
					{
						list.CopyTo(array, arrayIndex);
					}
				}
		public override int Count
				{
					get
					{
						lock(SyncRoot)
						{
							return list.count;
						}
					}
				}
		public override bool IsSynchronized
				{
					get
					{
						return true;
					}
				}
		public override Object SyncRoot
				{
					get
					{
						return list.SyncRoot;
					}
				}

		// Copy from this array list to another array.
		public override void CopyTo(Array array)
				{
					lock(SyncRoot)
					{
						list.CopyTo(array);
					}
				}
		public override void CopyTo(int index, Array array,
							   	   int arrayIndex, int count)
				{
					lock(SyncRoot)
					{
						list.CopyTo(index, array, arrayIndex, count);
					}
				}

		// Reverse the contents of this array list.
		public override void Reverse()
				{
					lock(SyncRoot)
					{
						list.Reverse();
					}
				}
		public override void Reverse(int index, int count)
				{
					lock(SyncRoot)
					{
						list.Reverse(index, count);
					}
				}

		// Sort the contents of this array list.
		public override void Sort()
				{
					lock(SyncRoot)
					{
						list.Sort();
					}
				}
		public override void Sort(IComparer comparer)
				{
					lock(SyncRoot)
					{
						list.Sort(comparer);
					}
				}
		public override void Sort(int index, int count, IComparer comparer)
				{
					lock(SyncRoot)
					{
						list.Sort(index, count, comparer);
					}
				}

		// Create an array that contains the elements of this array list.
		public override Object[] ToArray()
				{
					lock(SyncRoot)
					{
						return list.ToArray();
					}
				}
		public override Array ToArray(Type type)
				{
					lock(SyncRoot)
					{
						return list.ToArray(type);
					}
				}

		// Trim the array list to its actual size.
		public override void TrimToSize()
				{
					lock(SyncRoot)
					{
						list.TrimToSize();
					}
				}

		// Get or set the current capacity of the array list.
		public override int Capacity
				{
					get
					{
						lock(SyncRoot)
						{
							return list.Capacity;
						}
					}
					set
					{
						lock(SyncRoot)
						{
							list.Capacity = value;
						}
					}
				}

		// Get an enumerator for this array list.
		public override IEnumerator GetEnumerator()
				{
					lock(SyncRoot)
					{
						return new SynchronizedEnumerator
							(SyncRoot, list.GetEnumerator());
					}
				}
		public override IEnumerator GetEnumerator(int index, int count)
				{
					lock(SyncRoot)
					{
						return new SynchronizedEnumerator
							(SyncRoot, list.GetEnumerator(index, count));
					}
				}

	}; // class SynchronizedWrapper

#if !ECMA_COMPAT

	// Adapt an ordinary list to appear to be synchronized.
	public static IList Synchronized(IList list)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				else
				{
					return new SynchronizedListWrapper(list);
				}
			}

	// Wrapper class for synchronized lists.
	private sealed class SynchronizedListWrapper : IList
	{
		// Internal state.
		private IList list;

		// Constructor.
		public SynchronizedListWrapper(IList list)
				{
					this.list = list;
				}

		// Implement the IList interface.
		public int Add(Object value)
				{
					lock(SyncRoot)
					{
						return list.Add(value);
					}
				}
		public void Clear()
				{
					lock(SyncRoot)
					{
						list.Clear();
					}
				}
		public bool Contains(Object value)
				{
					lock(SyncRoot)
					{
						return list.Contains(value);

					}
				}
		public int  IndexOf(Object value)
				{
					lock(SyncRoot)
					{
						return list.IndexOf(value);
					}
				}
		public void Insert(int index, Object value)
				{
					lock(SyncRoot)
					{
						list.Insert(index, value);
					}
				}
		public void Remove(Object value)
				{
					lock(SyncRoot)
					{
						list.Remove(value);
					}
				}
		public void RemoveAt(int index)
				{
					lock(SyncRoot)
					{
						list.RemoveAt(index);
					}
				}
		public bool IsFixedSize
				{
					get
					{
						lock(SyncRoot)
						{
							return list.IsFixedSize;
						}
					}
				}
		public bool IsReadOnly
				{
					get
					{
						lock(SyncRoot)
						{
							return list.IsReadOnly;
						}
					}
				}
		public Object this[int index]
				{
					get
					{
						lock(SyncRoot)
						{
							return list[index];
						}
					}
					set
					{
						lock(SyncRoot)
						{
							list[index] = value;
						}
					}
				}

		// Implement the ICollection interface.
		public void CopyTo(Array array, int index)
				{
					lock(SyncRoot)
					{
						list.CopyTo(array, index);
					}
				}
		public int Count
				{
					get
					{
						lock(SyncRoot)
						{
							return list.Count;
						}
					}
				}
		public bool IsSynchronized
				{
					get
					{
						return true;
					}
				}
		public Object SyncRoot
				{
					get
					{
						return list.SyncRoot;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					lock(SyncRoot)
					{
						return new SynchronizedEnumerator
							(SyncRoot, list.GetEnumerator());
					}
				}

	}; // class SynchronizedListWrapper

#endif // !ECMA_COMPAT

}; // class ArrayList

}; // namespace System.Collections
