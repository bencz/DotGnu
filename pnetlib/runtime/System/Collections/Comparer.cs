/*
 * Comparer.cs - Implementation of the "System.Collections.Comparer" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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
using System.Globalization;

public sealed class Comparer : IComparer
{
	// Default comparers.
	public static readonly Comparer Default = new Comparer();
#if !ECMA_COMPAT
	public static readonly Comparer DefaultInvariant
		= new Comparer(CultureInfo.InvariantCulture);
#endif

	// Private constructor for the default comparer.
	private Comparer()
	{
		// Nothing to do here.
	}

#if !ECMA_COMPAT
	// Internal state.
	private CompareInfo info;

	// Public constructor for a culture-based comparer.
	public Comparer(CultureInfo culture)
	{
		if(culture == null)
		{
			throw new ArgumentNullException("culture");
		}
		this.info = culture.CompareInfo;
	}
#endif

	// Implement the IComparer interface.
	public int Compare(Object a, Object b)
	{
		IComparable cmp;
		if(a != null && b != null)
		{
		#if !ECMA_COMPAT
			if(info != null && a is String && b is String)
			{
				return info.Compare((String)a, (String)b);
			}
		#endif
			cmp = (a as IComparable);
			if(cmp != null)
			{
				return cmp.CompareTo(b);
			}
			cmp = (b as IComparable);
			if(cmp != null)
			{
				return -(cmp.CompareTo(a));
			}
			throw new ArgumentException(_("Arg_ABMustBeComparable"));
		}
		else if(a != null)
		{
			return 1;
		}
		else if(b != null)
		{
			return -1;
		}
		else
		{
			return 0;
		}
	}

}; // class Comparer

}; // namespace System.Collections
