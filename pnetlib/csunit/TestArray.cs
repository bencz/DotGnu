/*
 * TestArray.cs - Implementation of the "CSUnit.TestArray" class.
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

namespace CSUnit
{

using System;

// This class implements a simple growable array.  We could
// use "System.Collections.ArrayList", except we need this test
// framework to test that "ArrayList" actually works!  So we use
// a simpler implementation here that is highly likely to work
// even if the underlying class library is severely broken.

public sealed class TestArray
{
	// Amount to grow the array by each time.
	private const int GrowAmount = 32;

	// Internal state.
	private Object[] values;
	private int      numValues;
	private int      maxValues;

	// Constructor.
	public TestArray()
			{
				values = null;
				numValues = 0;
				maxValues = 0;
			}

	// Add a new object to this array.
	public void Add(Object value)
			{
				if(numValues >= maxValues)
				{
					// Allocate a new array.
					Object[] newValues = new Object [maxValues + GrowAmount];

					// Copy the old elements.  We do this element by
					// element, because "Array.Copy" may not work!
					int posn;
					for(posn = 0; posn < numValues; ++posn)
					{
						newValues[posn] = values[posn];
					}

					// Swap in the new array.
					values = newValues;
					maxValues += GrowAmount;
				}
				values[numValues++] = value;
			}

	// Get the current length of the array.
	public int Length
			{
				get
				{
					return numValues;
				}
			}

	// Get or set a specific array element.
	public Object this[int index]
			{
				get
				{
					if(index >= 0 || index < numValues)
					{
						return values[index];
					}
					else
					{
						throw new IndexOutOfRangeException();
					}
				}
				set
				{
					if(index >= 0 || index < numValues)
					{
						values[index] = value;
					}
					else
					{
						throw new IndexOutOfRangeException();
					}
				}
			}

}; // class TestArray

}; // namespace CSUnit
