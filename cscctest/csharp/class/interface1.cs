/*
 * interface1.cs - Test interface method resolution.
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

using System;

public interface IEnumerator
{

	bool MoveNext();
	void Reset();
	Object Current { get; }

}

public struct DictionaryEntry
{
	// Instance fields.
	private Object key__;
	private Object value__;

	// Construct a dictionary entry.
	public DictionaryEntry(Object key, Object value)
	{
		key__ = key;
		value__ = value;
	}

	// DictionaryEntry properties.
	public Object Key
	{
		get
		{
			return key__;
		}
		set
		{
			key__ = value;
		}
	}
	public Object Value
	{
		get
		{
			return value__;
		}
		set
		{
			value__ = value;
		}
	}

}

public interface IDictionaryEnumerator : IEnumerator
{

	DictionaryEntry Entry { get; }
	Object Key { get; }
	Object Value { get; }

}

public interface IEnumerable
{

	IEnumerator GetEnumerator();

}

public interface ICollection : IEnumerable
{

	int    Count { get; }
	bool   IsSynchronized { get; }
	Object SyncRoot { get; }

}

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

}

class Test
{
	void m1(IDictionary dict)
	{
		IDictionaryEnumerator e;

		// "GetEnumerator" is declared along multiple paths,
		// which makes its resolution a little tricky.
		e = dict.GetEnumerator();
	}
}
