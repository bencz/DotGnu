/*
 * Comparer.cs - Implementation of the
 *		"System.Collections.Generic.Comparer" class.
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

namespace System.Collections.Generic
{

#if CONFIG_GENERICS

using System.Globalization;
using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
[CLSCompliant(false)]
public class Comparer<T> : IComparer<T>
{
	// Internal state.
	private CompareInfo info;

	// Constructor.
	public Comparer(CultureInfo culture)
			{
				if(culture == null)
				{
					throw new ArgumentNullException("culture");
				}
				info = culture.CompareInfo;
			}

	// Compare two elements.
	public int Compare(T x, T y)
			{
				// Handle the null cases first if T is a reference type.
				if(!(typeof(T).IsValueType))
				{
					if(x == null)
					{
						if(y == null)
						{
							return 0;
						}
						else
						{
							return -1;
						}
					}
					else if(y == null)
					{
						return 1;
					}
				}

				// Handle the string case, using the culture.
				if(x is String && y is String)
				{
					return info.Compare((String)x, (String)y);
				}

				// See if one of the elements is IComparable<T>.
				if(x is IComparable<T>)
				{
					return ((IComparable<T>)x).CompareTo(y);
				}
				if(y is IComparable<T>)
				{
					return -(((IComparable<T>)y).CompareTo(x));
				}

				// See if one of the elements is IComparable.
				if(x is System.Collections.IComparable)
				{
					return ((System.Collections.IComparable)x).CompareTo(y);
				}
				if(y is System.Collections.IComparable)
				{
					return -(((System.Collections.IComparable)y).CompareTo(x));
				}

				// Cannot compare the elements.
				throw new ArgumentException(_("Arg_ABMustBeComparable"));
			}

}; // class Comparer<T>

#endif // CONFIG_GENERICS

}; // namespace System.Collections.Generic
