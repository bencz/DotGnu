/*
 * DictionaryEnumeratorWrapper.cs - Wrap a non-generic dictionary enumerator
 *		to turn it into a generic one.
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

public sealed class DictionaryEnumeratorWrapper<KeyT, ValueT>
	: IDictionaryIterator<KeyT, ValueT>
{

	// Internal state.
	private System.Collections.IDictionaryEnumerator e;

	// Constructor.
	public DictionaryEnumeratorWrapper
				(System.Collections.IDictionaryEnumerator e)
			{
				if(e == null)
				{
					throw new ArgumentNullException("e");
				}
				this.e = e;
			}

	// Implement the IIterator<ValueT> interface.
	public bool MoveNext()
			{
				return e.MoveNext();
			}
	public void Reset()
			{
				e.Reset();
			}
	public void Remove()
			{
				throw new InvalidOperationException(S._("NotSupp_Remove"));
			}
	public DictionaryEntry<KeyT, ValueT> Current
			{
				get
				{
					System.Collections.DictionaryEntry entry = e.Entry;
					return new DictionaryEntry<KeyT, ValueT>
						((KeyT)(e.Key), (ValueT)(e.Value));
				}
			}

	// Implement the IDictionaryIterator<KeyT, ValueT> interface.
	public KeyT Key
			{
				get
				{
					return (KeyT)(e.Key);
				}
			}
	public ValueT Value
			{
				get
				{
					return (ValueT)(e.Value);
				}
			}

}; // class DictionaryEnumeratorAdapter<KeyT, ValueT>

}; // namespace Generics
