/*
 * BitArray.cs - Implementation of the "System.Collections.BitArray" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#if !ECMA_COMPAT

using System;
using System.Runtime.CompilerServices;

public sealed class BitArray : ICollection, IEnumerable, ICloneable
{
	// Internal state.
	private int[] bitArray;
	private int   numBits;
	private int   generation;

	// Constructors.
	public BitArray(BitArray bits)
			{
				if(bits == null)
				{
					throw new ArgumentNullException("bits");
				}
				bitArray = (int[])(bits.bitArray.Clone());
				numBits = bits.numBits;
				generation = 0;
			}
	public BitArray(bool[] values)
			{
				if(values == null)
				{
					throw new ArgumentNullException("values");
				}
				numBits = values.Length;
				bitArray = new int [(numBits + 31) / 32];
				int posn;
				for(posn = 0; posn < numBits; ++posn)
				{
					if(values[posn])
					{
						bitArray[posn >> 5] |= (1 << (posn & 31));
					}
				}
				generation = 0;
			}
	public BitArray(byte[] values)
			{
				if(values == null)
				{
					throw new ArgumentNullException("values");
				}
				numBits = values.Length * 8;
				bitArray = new int [(numBits + 31) / 32];
				int posn;
				for(posn = 0; posn < values.Length; ++posn)
				{
					bitArray[posn >> 2] |= (values[posn] << (8 * (posn & 3)));
				}
				generation = 0;
			}
	public BitArray(int length)
			{
				if(length < 0)
				{
					throw new ArgumentOutOfRangeException
						(_("ArgRange_NonNegative"));
				}
				numBits = length;
				bitArray = new int [(numBits + 31) / 32];
				generation = 0;
			}
	public BitArray(int[] values)
			{
				if(values == null)
				{
					throw new ArgumentNullException("values");
				}
				numBits = values.Length * 32;
				bitArray = (int[])(values.Clone());
				generation = 0;
			}
	public BitArray(int length, bool defaultValue)
			{
				if(length < 0)
				{
					throw new ArgumentOutOfRangeException
						(_("ArgRange_NonNegative"));
				}
				numBits = length;
				bitArray = new int [(numBits + 31) / 32];
				if(defaultValue)
				{
					int posn;
					for(posn = 0; posn < bitArray.Length; ++posn)
					{
						bitArray[posn] = -1;
					}
				}
				generation = 0;
			}

	// Implement the ICollection interface.
	public int Count
			{
				get
				{
					return numBits;
				}
			}
	public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}
	public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}
	public bool this[int index]
			{
				get
				{
					return Get(index);
				}
				set
				{
					Set(index, value);
				}
			}
	public Object SyncRoot
			{
				get
				{
					return this;
				}
			}
	public void CopyTo(Array array, int index)
			{
				if(array == null)
				{
					throw new ArgumentNullException("array");
				}
				else if(array.Rank != 1)
				{
					throw new ArgumentException(_("Arg_RankMustBe1"));
				}
				else if(index < array.GetLowerBound(0))
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				else if(index > (array.GetLength(0) - numBits))
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				else
				{
					int posn;
					for(posn = 0; posn < numBits; ++posn)
					{
						array.SetValue
							(((bitArray[posn >> 5] & (1 << (posn & 31))) != 0),
							 index++);
					}
				}
			}

	// Get or set the length of the bit array.
	public int Length
			{
				get
				{
					return numBits;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException
							(_("ArgRange_NonNegative"));
					}
					else if(value == numBits)
					{
						// No change to the current array size.
						return;
					}
					++generation;
					if(value < numBits)
					{
						// We are making the array smaller, which
						// means we can keep the same bit buffer.
						numBits = value;
					}
					else if(((value + 31) / 32) <= bitArray.Length)
					{
						// The array will still fit within the current buffer,
						// so just clear any additional bits that we need.
						ClearBits(numBits, value - numBits);
					}
					else
					{
						// We need to extend the current buffer.
						ClearBits(numBits, bitArray.Length * 32 - numBits);
						int[] newArray = new int [(value + 31) / 32];
						Array.Copy(bitArray, newArray, (numBits + 31) / 32);
						bitArray = newArray;
						numBits = value;
					}
				}
			}

	// Clear a range of bits.
	private void ClearBits(int start, int num)
			{
				int posn = (start >> 5);
				int leftOver;
				while(num > 0)
				{
					leftOver = 32 - (start & 31);
					if(leftOver > num)
					{
						leftOver = num;
					}
					if(leftOver == 32)
					{
						bitArray[posn] = 0;
					}
					else
					{
						bitArray[posn] &=
							~(((1 << leftOver) - 1) << (start & 31));
					}
					start += leftOver;
					num -= leftOver;
					++posn;
				}
			}

	// And two bit arrays together.
	public BitArray And(BitArray value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(numBits != value.numBits)
				{
					throw new ArgumentException(_("Arg_BitArrayLengths"));
				}
				BitArray result = new BitArray(numBits);
				int posn = ((numBits + 31) / 32);
				while(posn > 0)
				{
					--posn;
					result.bitArray[posn] =
						(bitArray[posn] & value.bitArray[posn]);
				}
				return result;
			}

	// Or two bit arrays together.
	public BitArray Or(BitArray value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(numBits != value.numBits)
				{
					throw new ArgumentException(_("Arg_BitArrayLengths"));
				}
				BitArray result = new BitArray(numBits);
				int posn = ((numBits + 31) / 32);
				while(posn > 0)
				{
					--posn;
					result.bitArray[posn] =
						(bitArray[posn] | value.bitArray[posn]);
				}
				return result;
			}

	// Xor two bit arrays together.
	public BitArray Xor(BitArray value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(numBits != value.numBits)
				{
					throw new ArgumentException(_("Arg_BitArrayLengths"));
				}
				BitArray result = new BitArray(numBits);
				int posn = ((numBits + 31) / 32);
				while(posn > 0)
				{
					--posn;
					result.bitArray[posn] =
						(bitArray[posn] ^ value.bitArray[posn]);
				}
				return result;
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				BitArray array = (BitArray)MemberwiseClone();
				array.bitArray = (int[])(bitArray.Clone());
				return array;
			}

	// Get an element from this bit array.
	public bool Get(int index)
			{
				if(index < 0 || index >= numBits)
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				return ((bitArray[index >> 5] & (1 << (index & 31))) != 0);
			}

	// Implement the IEnumerable interface.
	public IEnumerator GetEnumerator()
			{
				return new BitArrayEnumerator(this);
			}

	// Invert all of the bits in this bit array.
	public BitArray Not()
			{
				BitArray result = new BitArray(numBits);
				int posn = ((numBits + 31) / 32);
				while(posn > 0)
				{
					--posn;
					result.bitArray[posn] = ~(bitArray[posn]);
				}
				return result;
			}

	// Set a particular bit in this bit array.
	public void Set(int index, bool value)
			{
				if(index < 0 || index >= numBits)
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				if(value)
				{
					bitArray[index >> 5] |= (1 << (index & 31));
				}
				else
				{
					bitArray[index >> 5] &= ~(1 << (index & 31));
				}
				++generation;
			}

	// Set all bits in this bit array to a specific value.
	public void SetAll(bool value)
			{
				int posn = ((numBits + 31) / 32);
				if(value)
				{
					while(posn > 0)
					{
						--posn;
						bitArray[posn] = -1;
					}
				}
				else
				{
					while(posn > 0)
					{
						--posn;
						bitArray[posn] = 0;
					}
				}
				++generation;
			}

	// Bit array enumerator class.
	private sealed class BitArrayEnumerator : IEnumerator
	{
		// Internal state.
		private BitArray array;
		private int      generation;
		private int		 posn;

		// Constructor.
		public BitArrayEnumerator(BitArray array)
				{
					this.array = array;
					generation = array.generation;
					posn = -1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(array.generation != generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					if(++posn < array.numBits)
					{
						return true;
					}
					posn = array.numBits;
					return false;
				}
		public void Reset()
				{
					if(array.generation != generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					posn = -1;
				}
		public Object Current
				{
					get
					{
						if(array.generation != generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(posn < 0 || posn >= array.numBits)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return ((array.bitArray[posn >> 5] &
									(1 << (posn & 31))) != 0);
					}
				}

	}; // BitArrayEnumerator

}; // class BitArray

#endif // !ECMA_COMPAT

}; // namespace System.Collections
