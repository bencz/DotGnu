/*
 * Array.cs - Implementation of the "System.Array" class.
 *
 * Copyright (C) 2001, 2002, 2003, 2009  Southern Storm Software, Pty Ltd.
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

namespace System
{

using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public abstract class Array : ICloneable, ICollection, IEnumerable, IList
{

	// Constructor.
	private Array()
	{
		// Nothing to do here.
	}

	// Inner version of "BinarySearch" used by the methods below
	// once the arguments have been validated.
	private static int InnerBinarySearch(Array array, int lower,
										 int upper, Object value,
										 IComparer comparer)
	{
		int left, right, middle, cmp;
		Object elem;
		IComparable icmp;
		left = lower;
		right = upper;
		if(right == Int32.MaxValue)
		{
			// We cannot report the correct answer when "right" is maxint
			// and the value is not found in the array.
			throw new ArgumentException(_("Arg_InvalidArrayRange"));
		}
		while((right - left) >= 0)
		{
			middle = left + ((right - left) / 2);
			elem = array.GetValue(middle);
			if(elem != null && value != null)
			{
				if(comparer != null)
				{
					cmp = comparer.Compare(value, elem);
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
					throw new ArgumentException(_("Arg_SearchCompare"));
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

	// Perform a binary search within an array for a value.
	public static int BinarySearch(Array array, Object value)
	{
		return BinarySearch(array, value, (IComparer)null);
	}

	// Perform a binary search within an array sub-range for a value.
	public static int BinarySearch(Array array, int index, int length,
								   Object value)
	{
		return BinarySearch(array, index, length, value, (IComparer)null);
	}

	// Perform a binary search within an array for a value,
	// using a specific element comparer.
	public static int BinarySearch(Array array, Object value,
								   IComparer comparer)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		else if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		return InnerBinarySearch(array, array.GetLowerBound(0),
								 array.GetUpperBound(0), value, comparer);
	}

	// Perform a binary search within an array sub-range for a value,
	// using a specific element comparer.
	public static int BinarySearch(Array array, int index, int length,
								   Object value, IComparer comparer)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		else if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		else if(index < array.GetLowerBound(0))
		{
			throw new ArgumentOutOfRangeException
				("index", _("ArgRange_Array"));
		}
		else if(length < 0)
		{
			throw new ArgumentOutOfRangeException
				("length", _("ArgRange_Array"));
		}
		else if((index - 1) > array.GetUpperBound(0) ||
		        length > (array.GetUpperBound(0) - index + 1))
		{
			throw new ArgumentException(_("Arg_InvalidArrayRange"));
		}
		return InnerBinarySearch(array, index, index + length - 1,
								 value, null);
	}

	// Clear the contents of an array.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Clear(Array array, int index, int length);

#if CONFIG_RUNTIME_INFRA
	// Initialize the contents of an array of value types.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public void Initialize();
#endif

	// Clone this array.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual Object Clone();

	// Copy the contents of one array into another.
	public static void Copy(Array sourceArray, Array destinationArray,
							int length)
	{
		if(sourceArray == null)
		{
			throw new ArgumentNullException("sourceArray");
		}
		else if(destinationArray == null)
		{
			throw new ArgumentNullException("destinationArray");
		}
		Copy(sourceArray, sourceArray.GetLowerBound(0),
		     destinationArray, destinationArray.GetLowerBound(0),
			 length);
	}

	// Internal array copy method for similarly-typed arrays.
	// The engine can assume that the parameters have been validated,
	// and indicate the relative indices into the arrays.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static void InternalCopy
					(Array sourceArray, int sourceIndex,
				     Array destinationArray,
					 int destinationIndex, int length);

	// Determine if it is possible to cast between two types,
	// without using a narrowing conversion.
	private static bool ArrayTypeCompatible(Type src, Type dest)
			{
				if(dest.IsAssignableFrom(src))
				{
					// Direct assignment with or without boxing conversion.
					return true;
				}
				else if(dest.IsValueType && src.IsAssignableFrom(dest))
				{
					// Unboxing conversion.
					return true;
				}
				else if(src.IsPrimitive && dest.IsPrimitive)
				{
					// Primitive numeric cast.
					return Convert.HasWideningConversion(src, dest);
				}
				else if(!src.IsValueType && !dest.IsValueType)
				{
					// Try using an explicit cast up the tree.
					return src.IsAssignableFrom(dest);
				}
				else
				{
					// No conversion possible.
					return false;
				}
			}

	// Copy the contents of one array into another (general-purpose version).
	public static void Copy(Array sourceArray, int sourceIndex,
						    Array destinationArray,
						    int destinationIndex, int length)
	{
		// Validate the parameters.
		if(sourceArray == null)
		{
			throw new ArgumentNullException("sourceArray");
		}
		if(destinationArray == null)
		{
			throw new ArgumentNullException("destinationArray");
		}
		if(sourceArray.GetRank() != destinationArray.GetRank())
		{
			throw new RankException(_("Arg_MustBeSameRank"));
		}
		int srcLower = sourceArray.GetLowerBound(0);
		int srcLength = sourceArray.GetLength();
		int dstLower = destinationArray.GetLowerBound(0);
		int dstLength = destinationArray.GetLength();
		if(sourceIndex < srcLower)
		{
			throw new ArgumentOutOfRangeException
				("sourceIndex", _("ArgRange_Array"));
		}
		if(destinationIndex < dstLower)
		{
			throw new ArgumentOutOfRangeException
				("destinationIndex", _("ArgRange_Array"));
		}
		if(length < 0)
		{
			throw new ArgumentOutOfRangeException
				("length", _("ArgRange_NonNegative"));
		}
		int srcRelative = sourceIndex - srcLower;
		int dstRelative = destinationIndex - dstLower;
		if((srcLength - (srcRelative)) < length ||
		   (dstLength - (dstRelative)) < length)
		{
			throw new ArgumentException(_("Arg_InvalidArrayRange"));
		}

		// Get the array element types.
		Type arrayType1 = sourceArray.GetType().GetElementType();
		Type arrayType2 = destinationArray.GetType().GetElementType();

		// Is this a simple array copy of the same element type?
		if(arrayType1 == arrayType2)
		{
			InternalCopy
				(sourceArray, srcRelative,
				 destinationArray, dstRelative,
				 length);
			return;
		}

		// Check that casting between the types is possible,
		// without using a narrowing conversion.
		if(!ArrayTypeCompatible(arrayType1, arrayType2))
		{
			throw new ArrayTypeMismatchException
				(_("Exception_ArrayTypeMismatch")); 
		}

		// Copy the array contents the hard way.  We don't have to
		// worry about overlapping ranges because there is no way
		// to get here if the source and destination are the same.
		int index;
		for(index = 0; index < length; ++index)
		{
			try
			{
				destinationArray.SetRelative(
					Convert.ConvertObject(
						sourceArray.GetRelative(srcRelative + index), 
						arrayType2), dstRelative + index);
			}
			catch(FormatException e)
			{
				throw new InvalidCastException(String.Format(_("InvalidCast_FromTo"),
 						     arrayType1, arrayType2), e);
			}
		}
	}

	// Implement the ICollection interface.
	public virtual void CopyTo(Array array, int index)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		else if(GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		else if(array.GetRank() > 1)
		{
			throw new ArgumentException("array", _("Arg_RankMustBe1"));
		}
		else
		{
			Copy(this, GetLowerBound(0), array,
			     index + array.GetLowerBound(0), GetLength());
		}
	}
	int ICollection.Count
	{
		get
		{
			return GetLength();
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

	// Internal versions of "CreateInstance".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Array CreateArray(IntPtr elementType,
										    int rank, int length1,
										    int length2, int length3);
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Array CreateArray(IntPtr elementType,
										    int[] lengths,
										    int[] lowerBounds);

	// Create a single-dimensional array instance.
	public static Array CreateInstance(Type elementType, int length)
	{
		IntPtr typeHandle;
		typeHandle = ClrHelpers.TypeToClrHandle(elementType, "elementType");
		if(length < 0)
		{
			throw new ArgumentOutOfRangeException
				("length", _("ArgRange_NonNegative"));
		}
		return CreateArray(typeHandle, 1, length, 0, 0);
	}

	// Create a double-dimensional array instance.
	public static Array CreateInstance(Type elementType,
									   int length1, int length2)
	{
		IntPtr typeHandle;
		typeHandle = ClrHelpers.TypeToClrHandle(elementType, "elementType");
		if(length1 < 0)
		{
			throw new ArgumentOutOfRangeException
				("length1", _("ArgRange_NonNegative"));
		}
		if(length2 < 0)
		{
			throw new ArgumentOutOfRangeException
				("length2", _("ArgRange_NonNegative"));
		}
		return CreateArray(typeHandle, 2, length1, length2, 0);
	}

	// Create a triple-dimensional array instance.
	public static Array CreateInstance(Type elementType, int length1,
									   int length2, int length3)
	{
		IntPtr typeHandle;
		typeHandle = ClrHelpers.TypeToClrHandle(elementType, "elementType");
		if(length1 < 0)
		{
			throw new ArgumentOutOfRangeException
				("length1", _("ArgRange_NonNegative"));
		}
		if(length2 < 0)
		{
			throw new ArgumentOutOfRangeException
				("length2", _("ArgRange_NonNegative"));
		}
		if(length3 < 0)
		{
			throw new ArgumentOutOfRangeException
				("length3", _("ArgRange_NonNegative"));
		}
		return CreateArray(typeHandle, 3, length1, length2, length3);
	}

	// Create an array instance from an array of length values.
	public static Array CreateInstance(Type elementType, int[] lengths)
	{
		int index;
		IntPtr typeHandle;
		typeHandle = ClrHelpers.TypeToClrHandle(elementType, "elementType");
		if(lengths == null)
		{
			throw new ArgumentNullException("lengths");
		}
		if(lengths.GetLength() < 1)
		{
			throw new ArgumentException(_("Arg_MustHaveOneElement"));
		}
		for(index = lengths.GetLength() - 1; index >= 0; --index)
		{
			if(lengths[index] < 0)
			{
				throw new ArgumentOutOfRangeException
					("lengths[" + (lengths[index]).ToString() + "]",
					 _("ArgRange_NonNegative"));
			}
		}
		return CreateArray(typeHandle, lengths, null);
	}

	// Create an array instance from an array of length values,
	// and an array of lower bounds.
	public static Array CreateInstance(Type elementType, int[] lengths,
									   int[] lowerBounds)
	{
		int index;
		IntPtr typeHandle;
		typeHandle = ClrHelpers.TypeToClrHandle(elementType, "elementType");
		if(lengths == null)
		{
			throw new ArgumentNullException("lengths");
		}
		if(lowerBounds == null)
		{
			throw new ArgumentNullException("lowerBounds");
		}
		if(lengths.GetLength() != lowerBounds.GetLength())
		{
			throw new ArgumentException(_("Arg_MustBeSameSize"));
		}
		if(lengths.GetLength() < 1)
		{
			throw new ArgumentException(_("Arg_MustHaveOneElement"));
		}
		for(index = lengths.GetLength() - 1; index >= 0; --index)
		{
			if(lengths[index] < 0)
			{
				throw new ArgumentOutOfRangeException
					("lengths[" + (lengths[index]).ToString() + "]",
					 _("ArgRange_NonNegative"));
			}
		}
		return CreateArray(typeHandle, lengths, lowerBounds);
	}

	// Implement the IEnumerable interface.
	public virtual IEnumerator GetEnumerator()
	{
		int rank = GetRank();
		if(rank == 1)
		{
			return new ArrayEnumerator1(this);
		}
		else if(rank == 2)
		{
			return new ArrayEnumerator2(this);
		}
		else
		{
			return new ArrayEnumeratorN(this, rank);
		}
	}

	// Private class for enumerating a single-dimensional array's contents.
	private sealed class ArrayEnumerator1 : IEnumerator
	{
		private Array array;
		private int   lower;
		private int   upper;
		private int   posn;

		// Constructor.
		public ArrayEnumerator1(Array array)
				{
					this.array = array;
					this.lower = array.GetLowerBound(0);
					this.upper = array.GetUpperBound(0);
					this.posn  = this.lower - 1;
				}

		// Move to the next element in the array.
		public bool MoveNext()
				{
					if(posn < upper)
					{
						++posn;
						return true;
					}
					else
					{
						return false;
					}
				}

		// Reset the enumerator.
		public void Reset()
				{
					posn = lower - 1;
				}

		// Get the current object.
		public Object Current
				{
					get
					{
						if(posn >= lower && posn <= upper)
						{
							return array.GetValue(posn);
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}

	};

	// Private class for enumerating a double-dimensional array's contents.
	private sealed class ArrayEnumerator2 : IEnumerator
	{
		private Array array;
		private int   lower1;
		private int   lower2;
		private int   upper1;
		private int   upper2;
		private int   posn1;
		private int   posn2;

		// Constructor.
		public ArrayEnumerator2(Array array)
				{
					this.array  = array;
					this.lower1 = array.GetLowerBound(0);
					this.lower2 = array.GetLowerBound(1);
					this.upper1 = array.GetUpperBound(0);
					this.upper2 = array.GetUpperBound(1);
					this.posn1 = lower1 - 1;
					this.posn2 = upper2;
				}

		// Move to the next element in the array.
		public bool MoveNext()
				{
					if(posn2 >= upper2)
					{
						// Start a new row.
						if(posn1 < upper1)
						{
							posn2 = lower2;
							if(posn2 >= upper2)
							{
								// The inner dimension is zero, so the
								// enumerator never returns anything.
								return false;
							}
							++posn1;
							return true;
						}
						else
						{
							return false;
						}
					}
					else
					{
						// Advance within the current row.
						++posn2;
						return true;
					}
				}

		// Reset the enumerator.
		public void Reset()
				{
					posn1 = lower1 - 1;
					posn2 = upper2;
				}

		// Get the current object.
		public Object Current
				{
					get
					{
						if(posn1 >= lower1 && posn1 <= upper1 &&
						   posn2 >= lower2 && posn2 <= upper2)
						{
							return array.GetValue(posn1, posn2);
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}

	};

	// Private class for enumerating a multi-dimensional array's contents.
	private sealed class ArrayEnumeratorN : IEnumerator
	{
		private Array array;
		private int   rank;
		private long  length;
		private long  posn;
		private int[] lower;
		private int[] upper;
		private int[] aposn;

		// Constructor.
		public ArrayEnumeratorN(Array array, int rank)
				{
					int dim;
					this.array = array;
					this.rank  = rank;
					length = array.LongLength;
					posn = -1;
					this.lower = new int [rank];
					this.upper = new int [rank];
					this.aposn = new int [rank];
					for(dim = 0; dim < rank; ++dim)
					{
						this.lower[dim] = array.GetLowerBound(dim);
						this.upper[dim] = array.GetUpperBound(dim);
					}
				}

		// Move to the next element in the array.
		public bool MoveNext()
				{
					if(posn < length)
					{
						++posn;
						return true;
					}
					else
					{
						return false;
					}
				}

		// Reset the enumerator.
		public void Reset()
				{
					posn = -1;
				}

		// Get the current object.
		public Object Current
				{
					get
					{
						if(posn >= 0 && posn < length)
						{
							int dim;
							long temp = posn;
							long len;
							for(dim = rank - 1; dim >= 0; --dim)
							{
								len = ((long)(upper[dim])) -
								      ((long)(lower[dim]));
								aposn[dim] = unchecked((int)(temp % len)) +
											 lower[dim];
								temp /= len;
							}
							return array.GetValue(aposn);
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}

	};

	// Get the length of an array rank.
	[MethodImpl(MethodImplOptions.InternalCall)]
#if ECMA_COMPAT
	internal
#else
	public
#endif
	extern int GetLength(int dimension);

#if CONFIG_RUNTIME_INFRA

	// Get the lower bound of an array rank.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public int GetLowerBound(int dimension);

	// Get the upper bound of an array rank.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public int GetUpperBound(int dimension);

#else // !CONFIG_RUNTIME_INFRA

	// Get the lower bound of an array rank.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal int GetLowerBound(int dimension);

	// Get the upper bound of an array rank.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal int GetUpperBound(int dimension);

#endif // !CONFIG_RUNTIME_INFRA

	// Internal versions of "GetValue".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Object Get(int index1, int index2, int index3);
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Object Get(int[] indices);

	// Internal version of "GetValue" that uses relative indexing.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Object GetRelative(int index);

	// Get the value at a particular index within a multi-dimensional array.
	public Object GetValue(int[] indices)
	{
		if(indices == null)
		{
			throw new ArgumentNullException("indices");
		}
		if(indices.GetLength() != GetRank())
		{
			throw new ArgumentException(_("Arg_MustBeSameSize"));
		}
		return Get(indices);
	}

	// Get the value at a particular index within a single-dimensional array.
	public Object GetValue(int index)
	{
		if(GetRank() != 1)
		{
			throw new ArgumentException(_("Arg_RankMustBe1"));
		}
		return Get(index, 0, 0);
	}

	// Get the value at a particular index within a double-dimensional array.
	public Object GetValue(int index1, int index2)
	{
		if(GetRank() != 2)
		{
			throw new ArgumentException(_("Arg_RankMustBe2"));
		}
		return Get(index1, index2, 0);
	}

	// Get the value at a particular index within a triple-dimensional array.
	public Object GetValue(int index1, int index2, int index3)
	{
		if(GetRank() != 3)
		{
			throw new ArgumentException(_("Arg_RankMustBe3"));
		}
		return Get(index1, index2, index3);
	}

	// Inner version of "IndexOf".
	private static int InnerIndexOf(Array array, Object value,
								    int start, int length)
	{
		Object elem;
		while(length > 0)
		{
			elem = array.GetValue(start);
			if(value != null && elem != null)
			{
				if(value.Equals(elem))
				{
					return start;
				}
			}
			else if(value == null && elem == null)
			{
				return start;
			}
			++start;
			--length;
		}
		return array.GetLowerBound(0) - 1;
	}

	// Inner version of "LastIndexOf".
	private static int InnerLastIndexOf(Array array, Object value,
								        int start, int length)
	{
		Object elem;
		start += length - 1;
		while(length > 0)
		{
			elem = array.GetValue(start);
			if(value != null && elem != null)
			{
				if(value.Equals(elem))
				{
					return start;
				}
			}
			else if(value == null && elem == null)
			{
				return start;
			}
			--start;
			--length;
		}
		return array.GetLowerBound(0) - 1;
	}

	// Get the first index of a specific value within an array.
	public static int IndexOf(Array array, Object value)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		return InnerIndexOf(array, value, array.GetLowerBound(0),
					array.GetUpperBound(0) - array.GetLowerBound(0) + 1);
	}

	// Get the last index of a specific value within an array.
	public static int LastIndexOf(Array array, Object value)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		return InnerLastIndexOf(array, value, array.GetLowerBound(0),
					array.GetUpperBound(0) - array.GetLowerBound(0) + 1);
	}

	// Get the first index of a specific value within an array,
	// starting at a particular index.
	public static int IndexOf(Array array, Object value, int startIndex)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		if(startIndex < array.GetLowerBound(0) ||
		   startIndex > array.GetUpperBound(0) + 1)
		{
			throw new ArgumentOutOfRangeException
				("startIndex", _("Arg_InvalidArrayIndex"));
		}
		return InnerIndexOf(array, value, startIndex,
							array.GetUpperBound(0) - startIndex + 1);
	}

	// Get the last index of a specific value within an array,
	// starting at a particular index.
	public static int LastIndexOf(Array array, Object value, int startIndex)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		if(startIndex < array.GetLowerBound(0) ||
		   startIndex > array.GetUpperBound(0) + 1)
		{
			throw new ArgumentOutOfRangeException
				("startIndex", _("Arg_InvalidArrayIndex"));
		}
		return InnerLastIndexOf(array, value, array.GetLowerBound(0),
								startIndex - array.GetLowerBound(0) + 1);
	}

	// Get the first index of a specific value within an array,
	// starting at a particular index, searching for "count" items.
	public static int IndexOf(Array array, Object value,
							  int startIndex, int count)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		if(startIndex < array.GetLowerBound(0) ||
		   startIndex + count > array.GetUpperBound(0) + 1 ||
		   count < 0)
		{
			throw new ArgumentOutOfRangeException(_("Arg_InvalidArrayRange"));
		}
		return InnerIndexOf(array, value, startIndex, count);
	}

	// Get the last index of a specific value within an array,
	// starting at a particular index, searching for "count" items.
	public static int LastIndexOf(Array array, Object value,
							      int startIndex, int count)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		if(startIndex < array.GetLowerBound(0) ||
		   startIndex + count > array.GetUpperBound(0) + 1 ||
		   count < 0)
		{
			throw new ArgumentOutOfRangeException(_("Arg_InvalidArrayRange"));
		}
		return InnerLastIndexOf(array, value, startIndex - count + 1, count);
	}

	// Inner version of "Reverse".
	private static void InnerReverse(Array array, int lower, int upper)
	{
		Object temp;
		while(lower < upper)
		{
			temp = array.GetValue(lower);
			array.SetValue(array.GetValue(upper), lower);
			array.SetValue(temp, upper);
			++lower;
			--upper;
		}
	}

	// Reverse the order of elements in an array.
	public static void Reverse(Array array)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		InnerReverse(array, array.GetLowerBound(0), array.GetUpperBound(0));
	}

	// Reverse the order of elements in an array sub-range.
	public static void Reverse(Array array, int index, int length)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		if(index < array.GetLowerBound(0))
		{
			throw new ArgumentOutOfRangeException
				("index", _("ArgRange_Array"));
		}
		if(length < 0)
		{
			throw new ArgumentOutOfRangeException
				("length", _("ArgRange_Array"));
		}
		if(index > array.GetUpperBound(0) ||
	       length > (array.GetUpperBound(0) - index + 1))
		{
			throw new ArgumentException(_("Arg_InvalidArrayRange"));
		}
		InnerReverse(array, index, index + length - 1);
	}

	// Internal versions of "SetValue".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private void Set(Object value, int index1, int index2, int index3);
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private void Set(Object value, int[] indices);

	// Internal version of "SetValue" that uses relative indexing.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private void SetRelative(Object value, int index);

	// Set the value at a particular index within a multi-dimensional array.
	public void SetValue(Object value, int[] indices)
	{
		if(indices == null)
		{
			throw new ArgumentNullException("indices");
		}
		if(indices.GetLength() != GetRank())
		{
			throw new ArgumentException(_("Arg_MustBeSameSize"));
		}
		Set(value, indices);
	}

	// Set the value at a particular index within a single-dimensional array.
	public void SetValue(Object value, int index)
	{
		if(GetRank() != 1)
		{
			throw new ArgumentException(_("Arg_RankMustBe1"));
		}
		Set(value, index, 0, 0);
	}

	// Set the value at a particular index within a double-dimensional array.
	public void SetValue(Object value, int index1, int index2)
	{
		if(GetRank() != 2)
		{
			throw new ArgumentException(_("Arg_RankMustBe2"));
		}
		Set(value, index1, index2, 0);
	}

	// Set the value at a particular index within a triple-dimensional array.
	public void SetValue(Object value, int index1, int index2, int index3)
	{
		if(GetRank() != 3)
		{
			throw new ArgumentException(_("Arg_RankMustBe3"));
		}
		Set(value, index1, index2, index3);
	}

	// Inner version of "Sort".
	private static void InnerSort(Array keys, Array items,
						          int lower, int upper,
						          IComparer comparer)
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
			pivot = keys.GetValue(i + ((j - i) / 2));

			// Partition the range.
			do
			{
				// Find two values to be swapped.
				try
				{
					while(comparer.Compare(keys.GetValue(i), pivot) < 0)
					{
						++i;
					}
					while(comparer.Compare(keys.GetValue(j), pivot) > 0)
					{
						--j;
					}
				}
				catch(ArgumentException e)
				{
					throw new InvalidOperationException(e.Message);
				}
				if(i > j)
				{
					break;
				}

				// Swap the values.
				if(i < j)
				{
					temp = keys.GetValue(i);
					keys.SetValue(keys.GetValue(j), i);
					keys.SetValue(temp, j);
					if(items != null)
					{
						temp = items.GetValue(i);
						items.SetValue(items.GetValue(j), i);
						items.SetValue(temp, j);
					}
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
					InnerSort(keys, items, lower, j, comparer);
				}
				lower = i;
			}
			else
			{
				if(i < upper)
				{
					InnerSort(keys, items, i, upper, comparer);
				}
				upper = j;
			}
		}
		while(lower < upper);
	}

	// Sort an array of keys.
	public static void Sort(Array array)
	{
		Sort(array, (IComparer)null);
	}

	// Sort an array of keys using a comparer.
	public static void Sort(Array array, IComparer comparer)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		if(comparer == null)
		{
			comparer = Comparer.Default;
		}
		InnerSort(array, null, array.GetLowerBound(0),
				  array.GetUpperBound(0), comparer);
	}

	// Sort an array of keys and items.
	public static void Sort(Array keys, Array items)
	{
		Sort(keys, items, (IComparer)null);
	}

	// Sort an array of keys and items using a comparer.
	public static void Sort(Array keys, Array items, IComparer comparer)
	{
		if(keys == null)
		{
			throw new ArgumentNullException("keys");
		}
		if(keys.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		if(items != null)
		{
			if(items.GetRank() != 1)
			{
				throw new RankException(_("Arg_RankMustBe1"));
			}
			if(items.GetLowerBound(0) != keys.GetLowerBound(0))
			{
				throw new ArgumentException(_("Arg_LowBoundsMustMatch"));
			}
			if(items.GetLength() < keys.GetLength())
			{
				throw new ArgumentException(_("Arg_ShortItemsArray"));
			}
		}
		if(comparer == null)
		{
			comparer = Comparer.Default;
		}
		InnerSort(keys, items, keys.GetLowerBound(0),
				  keys.GetUpperBound(0), comparer);
	}

	// Sort an array sub-range of keys.
	public static void Sort(Array array, int index, int length)
	{
		Sort(array, index, length, (IComparer)null);
	}

	// Sort an array sub-range of keys using a comparer.
	public static void Sort(Array array, int index, int length,
							IComparer comparer)
	{
		if(array == null)
		{
			throw new ArgumentNullException("array");
		}
		if(array.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		if(index < array.GetLowerBound(0))
		{
			throw new ArgumentOutOfRangeException
				("index", _("ArgRange_Array"));
		}
		if(length < 0)
		{
			throw new ArgumentOutOfRangeException
				("length", _("ArgRange_Array"));
		}
		if(index > array.GetUpperBound(0) + 1 ||
		   length > (array.GetUpperBound(0) - index + 1))
		{
			throw new ArgumentException(_("Arg_InvalidArrayRange"));
		}
		if(comparer == null)
		{
			comparer = Comparer.Default;
		}
		InnerSort(array, null, index, index + length - 1, comparer);
	}

	// Sort an array sub-range of keys and items.
	public static void Sort(Array keys, Array items, int index, int length)
	{
		Sort(keys, items, index, length, (IComparer)null);
	}

	// Sort an array sub-range of keys and items using a comparer.
	public static void Sort(Array keys, Array items,
							int index, int length,
							IComparer comparer)
	{
		if(keys == null)
		{
			throw new ArgumentNullException("keys");
		}
		if(keys.GetRank() != 1)
		{
			throw new RankException(_("Arg_RankMustBe1"));
		}
		if(index < keys.GetLowerBound(0))
		{
			throw new ArgumentOutOfRangeException
				("index", _("ArgRange_Array"));
		}
		if(length < 0)
		{
			throw new ArgumentOutOfRangeException
				("length", _("ArgRange_Array"));
		}
		if(index > keys.GetUpperBound(0) ||
		   length > (keys.GetUpperBound(0) - index + 1))
		{
			throw new ArgumentException(_("Arg_InvalidArrayRange"));
		}
		if(items != null)
		{
			if(items.GetRank() != 1)
			{
				throw new RankException(_("Arg_RankMustBe1"));
			}
			if(index < items.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException
					("index", _("ArgRange_Array"));
			}
			if(index > items.GetUpperBound(0) ||
			   length > (items.GetUpperBound(0) - index + 1))
			{
				throw new ArgumentException(_("Arg_InvalidArrayRange"));
			}
		}
		if(comparer == null)
		{
			comparer = Comparer.Default;
		}
		InnerSort(keys, items, index, index + length - 1, comparer);
	}

	// Implement the "IList" interface.
	int IList.Add(Object value)
	{
		throw new NotSupportedException(_("NotSupp_FixedSizeCollection"));
	}
	void IList.Clear()
	{
		Clear(this, 0, GetLength());
	}
	bool IList.Contains(Object value)
	{
		return (IndexOf(this, value) >= GetLowerBound(0));
	}
	int IList.IndexOf(Object value)
	{
		return IndexOf(this, value);
	}
	void IList.Insert(int index, Object value)
	{
		throw new NotSupportedException(_("NotSupp_FixedSizeCollection"));
	}
	void IList.Remove(Object value)
	{
		throw new NotSupportedException(_("NotSupp_FixedSizeCollection"));
	}
	void IList.RemoveAt(int index)
	{
		throw new NotSupportedException(_("NotSupp_FixedSizeCollection"));
	}
	public virtual bool IsFixedSize
	{
		get
		{
			return true;
		}
	}
	public virtual bool IsReadOnly
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
			return GetValue(index);
		}
		set
		{
			SetValue(value, index);
		}
	}

	// Internal implementation of the "Length" and "Rank" properties.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private int GetLength();
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private int GetRank();

	// Properties.
	public int Length
	{
		get
		{
			return GetLength();
		}
	}
#if !ECMA_COMPAT
	[ComVisible(false)]
#endif
	public long LongLength
	{
		get
		{
			// Normally this will return the same as "Length", but it
			// is theoretically possible that really huge arrays could
			// exist on 64-bit systems in the future, so we calculate
			// the value carefully.
			long len = 1;
			int ranks = GetRank();
			while(ranks > 0)
			{
				--ranks;
				len *= (long)(GetUpperBound(ranks) - GetLowerBound(ranks) + 1);
			}
			return len;
		}
	}
	public int Rank
	{
		get
		{
			return GetRank();
		}
	}

#if !ECMA_COMPAT

	// Downgrade a 64-bit index to 32-bit.
	private static int DowngradeIndex(String name, long index)
	{
		if(index < (long)(Int32.MinValue) || index > (long)(Int32.MaxValue))
		{
			throw new ArgumentOutOfRangeException
				("name", _("ArgRange_Array"));
		}
		return unchecked((int)index);
	}
	private static int DowngradeIndex2(String name, long index)
	{
		if(index < (long)(Int32.MinValue) || index > (long)(Int32.MaxValue))
		{
			throw new IndexOutOfRangeException(_("Arg_InvalidArrayIndex"));
		}
		return unchecked((int)index);
	}

	// Downgrade a 64-bit length to 32-bit.
	private static int DowngradeLength(String name, long length)
	{
		if(length < 0)
		{
			throw new ArgumentOutOfRangeException
				(name, _("ArgRange_NonNegative"));
		}
		else if(length > (long)(Int32.MaxValue))
		{
			throw new ArgumentException(_("Arg_InvalidArrayRange"));
		}
		return unchecked((int)length);
	}

	// 64-bit versions of the array manipulation methods,
	// implemented in terms of their 32-bit counterparts.
	public static void Copy(Array sourceArray, Array destinationArray,
							long length)
	{
		Copy(sourceArray, destinationArray, DowngradeLength("length", length));
	}
	public static void Copy(Array sourceArray, long sourceIndex,
							Array destinationArray, long destinationIndex,
							long length)
	{
		Copy(sourceArray,
		     DowngradeIndex("sourceIndex", sourceIndex),
			 destinationArray,
			 DowngradeIndex("destinationIndex", destinationIndex),
			 DowngradeLength("length", length));
	}
	[ComVisible(false)]
	public virtual void CopyTo(Array array, long index)
	{
		CopyTo(array, DowngradeIndex("index", index));
	}
	public static Array CreateInstance(Type elementType, long[] lengths)
	{
		if(lengths == null)
		{
			throw new ArgumentNullException("lengths");
		}
		int[] ilengths = new int [lengths.GetLength()];
		int posn;
		for(posn = 0; posn < lengths.GetLength(); ++posn)
		{
			ilengths[posn] = DowngradeLength("lengths", lengths[posn]);
		}
		return CreateInstance(elementType, ilengths);
	}
	[ComVisible(false)]
	public long GetLongLength(int dimension)
	{
		return GetLength(dimension);
	}
	[ComVisible(false)]
	public Object GetValue(long index)
	{
		return GetValue(DowngradeIndex2("index", index));
	}
	[ComVisible(false)]
	public Object GetValue(long index1, long index2)
	{
		return GetValue(DowngradeIndex2("index1", index1),
						DowngradeIndex2("index2", index2));
	}
	[ComVisible(false)]
	public Object GetValue(long index1, long index2, long index3)
	{
		return GetValue(DowngradeIndex2("index1", index1),
						DowngradeIndex2("index2", index2),
						DowngradeIndex2("index3", index3));
	}
	[ComVisible(false)]
	public Object GetValue(long[] indices)
	{
		if(indices == null)
		{
			throw new ArgumentNullException("indices");
		}
		int[] iindices = new int [indices.GetLength()];
		int posn;
		for(posn = 0; posn < indices.GetLength(); ++posn)
		{
			iindices[posn] = DowngradeIndex2("indices", indices[posn]);
		}
		return GetValue(iindices);
	}
	[ComVisible(false)]
	public void SetValue(Object value, long index)
	{
		SetValue(value, DowngradeIndex2("index", index));
	}
	[ComVisible(false)]
	public void SetValue(Object value, long index1, long index2)
	{
		SetValue(value,
		         DowngradeIndex2("index1", index1),
				 DowngradeIndex2("index2", index2));
	}
	[ComVisible(false)]
	public void SetValue(Object value, long index1, long index2, long index3)
	{
		SetValue(value,
				 DowngradeIndex2("index1", index1),
				 DowngradeIndex2("index2", index2),
				 DowngradeIndex2("index3", index3));
	}
	[ComVisible(false)]
	public void SetValue(Object value, long[] indices)
	{
		if(indices == null)
		{
			throw new ArgumentNullException("indices");
		}
		int[] iindices = new int [indices.GetLength()];
		int posn;
		for(posn = 0; posn < indices.GetLength(); ++posn)
		{
			iindices[posn] = DowngradeIndex2("indices", indices[posn]);
		}
		SetValue(value, iindices);
	}

#endif // !ECMA_COMPAT

}; // class Array

}; // namespace System
