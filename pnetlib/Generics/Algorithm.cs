/*
 * Algorithm.cs - Useful generic algorithms.
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

namespace Generics
{

using System;

public class Algorithm
{

	// Swap two items.
	public static void Swap<T>(ref T x, ref T y)
			{
				T temp = x;
				x = y;
				y = temp;
			}

	// Swap two items, defined by iterator positions.
	public static void Swap<T>(IIterator<T> iter1, IIterator<T> iter2)
			{
				T temp = iter1.Current;
				iter1.Current = iter2.Current;
				iter2.Current = temp;
			}

	// Determine the minimum of two items.
	public static T Min<T>(T x, T y)
			{
				if(x < y)
				{
					return x;
				}
				else
				{
					return y;
				}
			}
	public static T Min<T>(T x, T y, IComparer<T> cmp)
			{
				if(cmp.Compare(x, y) < 0)
				{
					return x;
				}
				else
				{
					return y;
				}
			}

	// Determine the maximum of two items.
	public static T Max<T>(T x, T y)
			{
				if(x > y)
				{
					return x;
				}
				else
				{
					return y;
				}
			}
	public static T Max<T>(T x, T y, IComparer<T> cmp)
			{
				if(cmp.Compare(x, y) > 0)
				{
					return x;
				}
				else
				{
					return y;
				}
			}

	// Determine the median of three values.
	public static T Median<T>(T x, T y, T z)
			{
				if(x < y)
				{
					if(y < z)
					{
						return y;
					}
					else if(x < z)
					{
						return z;
					}
					else
					{
						return x;
					}
				}
				else if(x < z)
				{
					return x;
				}
				else if(y < z)
				{
					return z;
				}
				else
				{
					return y;
				}
			}
	public static T Median<T>(T x, T y, T z, IComparer<T> cmp)
			{
				if(cmp.Compare(x, y) < 0)
				{
					if(cmp.Compare(y, z) < 0)
					{
						return y;
					}
					else if(cmp.Compare(x, z) < 0)
					{
						return z;
					}
					else
					{
						return x;
					}
				}
				else if(cmp.Compare(x, z) < 0)
				{
					return x;
				}
				else if(cmp.Compare(y, z) < 0)
				{
					return z;
				}
				else
				{
					return y;
				}
			}

	// Determine if the content of two iterations are equal.
	public static bool Equals<T>(IIterator<T> e1, IIterator<T> e2)
			{
				for(;;)
				{
					if(!e1.MoveNext())
					{
						return !(e2.MoveNext());
					}
					if(!e2.MoveNext())
					{
						return false;
					}
					if(e1.Current != e2.Current)
					{
						return false;
					}
				}
			}
	public static bool Equals<T>(IIterator<T> e1, IIterator<T> e2,
								 IComparer<T> cmp)
			{
				for(;;)
				{
					if(!e1.MoveNext())
					{
						return !(e2.MoveNext());
					}
					if(!e2.MoveNext())
					{
						return false;
					}
					if(cmp.Compare(e1.Current, e2.Current) != 0)
					{
						return false;
					}
				}
			}

	// Determine if the content of two iterations are not equal.
	public static bool NotEquals<T>(IIterator<T> e1, IIterator<T> e2)
			{
				for(;;)
				{
					if(!e1.MoveNext())
					{
						return e2.MoveNext();
					}
					if(!e2.MoveNext())
					{
						return true;
					}
					if(e1.Current == e2.Current)
					{
						return false;
					}
				}
			}
	public static bool NotEquals<T>(IIterator<T> e1, IIterator<T> e2,
								    IComparer<T> cmp)
			{
				for(;;)
				{
					if(!e1.MoveNext())
					{
						return e2.MoveNext();
					}
					if(!e2.MoveNext())
					{
						return true;
					}
					if(cmp.Compare(e1.Current, e2.Current) == 0)
					{
						return false;
					}
				}
			}

	// Determine if two collections are equal.
	public static bool Equals<T>(ICollection<T> c1, ICollection<T> c2)
			{
				return Equals<T>(c1.GetIterator(), c2.GetIterator());
			}
	public static bool Equals<T>(ICollection<T> c1, ICollection<T> c2,
								 IComparer<T> cmp)
			{
				return Equals<T>(c1.GetIterator(), c2.GetIterator(), cmp);
			}

	// Determine if two collections are not equal.
	public static bool NotEquals<T>(ICollection<T> c1, ICollection<T> c2)
			{
				return NotEquals<T>(c1.GetIterator(), c2.GetIterator());
			}
	public static bool NotEquals<T>(ICollection<T> c1, ICollection<T> c2,
								    IComparer<T> cmp)
			{
				return NotEquals<T>(c1.GetIterator(),
									c2.GetIterator(), cmp);
			}

	// Find a particular value in an iteration, beginning with
	// the next item.  The iterator will be positioned on the
	// found value, or the end of the iteration.
	public static bool Find<T>(IIterator<T> e, T value)
			{
				while(e.MoveNext())
				{
					if(e.Current == value)
					{
						return true;
					}
				}
				return false;
			}
	public static bool Find<T>(IIterator<T> e, T value, IComparer<T> cmp)
			{
				while(e.MoveNext())
				{
					if(cmp.Compare(e.Current, value) == 0)
					{
						return true;
					}
				}
				return false;
			}

	// Find the position within an iteration that satisfies a
	// particular predicate condition.
	public static bool Find<T>(IIterator<T> e, Predicate<T> pred)
			{
				while(e.MoveNext())
				{
					if(pred(e.Current))
					{
						return true;
					}
				}
				return false;
			}

}; // class Algorithm

}; // namespace Generics
