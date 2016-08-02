/*
 * CaseInsensitiveComparer.cs - Implementation of the
 *			"System.Collections.CaseInsensitiveComparer" class.
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

#if !ECMA_COMPAT

using System;
using System.Globalization;

public class CaseInsensitiveComparer : IComparer
{
	// The default case insensitive comparer instances.
	private static readonly CaseInsensitiveComparer defaultComparer =
		new CaseInsensitiveComparer();
	private static readonly CaseInsensitiveComparer defaultInvariantComparer =
		new CaseInsensitiveComparer(CultureInfo.InvariantCulture);

	// Internal state.
	private CompareInfo compare;

	// Get the default comparer instances.
	public static CaseInsensitiveComparer Default
			{
				get
				{
					return defaultComparer;
				}
			}
	public static CaseInsensitiveComparer DefaultInvariant
			{
				get
				{
					return defaultInvariantComparer;
				}
			}

	// Constructors.
	public CaseInsensitiveComparer()
			{
				compare = CultureInfo.CurrentCulture.CompareInfo;
			}
	public CaseInsensitiveComparer(CultureInfo culture)
			{
				if(culture == null)
				{
					throw new ArgumentNullException("culture");
				}
				compare = culture.CompareInfo;
			}

	// Implement the IComparer interface.
	public int Compare(Object a, Object b)
			{
				String stra = (a as String);
				String strb = (b as String);
				if(stra != null && strb != null)
				{
					return compare.Compare
						(stra, strb, CompareOptions.IgnoreCase);
				}
				else
				{
					return Comparer.Default.Compare(a, b);
				}
			}

}; // class CaseInsensitiveComparer

#endif // !ECMA_COMPAT

}; // namespace System.Collections
