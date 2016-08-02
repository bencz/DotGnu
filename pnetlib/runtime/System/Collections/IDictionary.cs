/*
 * IDictionary.cs - Implementation of the
 *		"System.Collections.IDictionary" interface.
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

using System;

public interface IDictionary : IEnumerable, ICollection
{

	void Add(Object key, Object value);
	void Clear();
	bool Contains(Object key);
	new IDictionaryEnumerator GetEnumerator();
	void Remove(Object key);
	bool IsFixedSize { get; }
	bool IsReadOnly { get; }
	Object this[Object key] { get; set; }
	ICollection Keys { get; }
	ICollection Values { get; }

}; // interface IDictionary

}; // namespace System.Collections
