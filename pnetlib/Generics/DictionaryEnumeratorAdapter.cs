/*
 * DictionaryEnumeratorAdapter.cs - Adapt a generic dictionary enumerator
 *		into a non-generic one.
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

public sealed class DictionaryEnumeratorAdapter<KeyT, ValueT>
	: System.Collections.IDictionaryEnumerator, System.Collections.IEnumerator
{

	// Internal state.
	private IDictionaryIterator<KeyT, ValueT> e;

	// Constructor.
	public DictionaryEnumeratorAdapter(IDictionaryIterator<KeyT, ValueT> e)
			{
				if(e == null)
				{
					throw new ArgumentNullException("e");
				}
				this.e = e;
			}

	// Implement the non-generic IEnumerator interface.
	public bool MoveNext()
			{
				return e.MoveNext();
			}
	public void Reset()
			{
				e.Reset();
			}
	public Object Current
			{
				get
				{
					return e.Current;
				}
			}

	// Implement the non-generic IDictionaryEnumerator interface.
	public System.Collections.DictionaryEntry Entry
			{
				get
				{
					DictionaryEntry<KeyT, ValueT> entry = e.Current;
					return new System.Collections.DictionaryEntry
						(e.Key, e.Value);
				}
			}
	public Object Key
			{
				get
				{
					return e.Key;
				}
			}
	public Object Value
			{
				get
				{
					return e.Value;
				}
			}

}; // class DictionaryEnumeratorAdapter<KeyT, ValueT>

}; // namespace Generics
