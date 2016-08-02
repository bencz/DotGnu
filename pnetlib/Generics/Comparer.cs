/*
 * Comparer.cs - Default generic comparer implementation.
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

public sealed class Comparer<T> : IComparer<T>
{

	// Compare two items using the IComparable<T> or IComparable interfaces.
	public int Compare(T a, T b)
			{
				IComparable<T> cmp;
				System.IComparable cmp2;

				// Deal with the "null" cases if T is not a value type.
				if(!(typeof(T).IsValueType))
				{
					if(a == null)
					{
						if(b == null)
						{
							return 0;
						}
						else
						{
							return -1;
						}
					}
					else if(b == null)
					{
						return 1;
					}
				}

				// Try using the IComparable<T> interface.
				cmp = (a as IComparable<T>);
				if(cmp != null)
				{
					return cmp.CompareTo(b);
				}
				cmp = (b as IComparable<T>);
				if(cmp != null)
				{
					return -(cmp.CompareTo(a));
				}

				// Try using the System.IComparable interface.
				cmp2 = (a as System.IComparable);
				if(cmp2 != null)
				{
					return cmp2.CompareTo(b);
				}
				cmp2 = (b as System.IComparable);
				if(cmp2 != null)
				{
					return -(cmp2.CompareTo(a));
				}

				// We were unable to compare the values.
				throw new ArgumentException(_("Arg_ABMustBeComparable"));
			}

}; // class Comparer<T>

}; // namespace Generics
