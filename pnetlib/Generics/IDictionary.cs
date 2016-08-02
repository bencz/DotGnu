/*
 * IDictionary.cs - Generic dictionaries.
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

public interface IDictionary<KeyT, ValueT>
	: ICollection< DictionaryEntry<KeyT, ValueT> >
{

	void Add(KeyT key, ValueT value);
	void Clear();
	bool Contains(KeyT key);
	new IDictionaryIterator<KeyT, ValueT> GetIterator();
	void Remove(KeyT key);
	ValueT this[KeyT key] { get; set; }
	ICollection<KeyT> Keys { get; }
	ICollection<ValueT> Values { get; }

}; // interface IDictionary<KeyT, ValueT>

}; // namespace Generics
