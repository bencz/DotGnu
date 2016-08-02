/*
 * CollectionsUtil.cs - Implementation of
 *		"System.Collections.Specialized.CollectionsUtil".
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Collections.Specialized
{

#if !ECMA_COMPAT

using System;
using System.Collections;

public class CollectionsUtil
{

	// Create a case-insensitive hash table.
	public static Hashtable CreateCaseInsensitiveHashtable()
			{
				return new Hashtable(CaseInsensitiveHashCodeProvider.Default,
									 CaseInsensitiveComparer.Default);
			}

	// Create a case-insensitive hash table and add the contents
	// of a dictionary.
	public static Hashtable CreateCaseInsensitiveHashtable(IDictionary d)
			{
				return new Hashtable
						(d, CaseInsensitiveHashCodeProvider.Default,
						 CaseInsensitiveComparer.Default);
			}

	// Create a case-insensitive hash table with a specific initial capacity.
	public static Hashtable CreateCaseInsensitiveHashtable(int capacity)
			{
				return new Hashtable
						(capacity, CaseInsensitiveHashCodeProvider.Default,
						 CaseInsensitiveComparer.Default);
			}

	// Create a case-insensitive sorted list.
	public static SortedList CreateCaseInsensitiveSortedList()
			{
				return new SortedList(CaseInsensitiveComparer.Default);
			}

}; // struct CollectionsUtil

#endif // !ECMA_COMPAT

}; // namespace System.Collections.Specialized
