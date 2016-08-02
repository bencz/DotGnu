/*
 * ReverseIterator.cs - Wrap an iterator to reverse its traversal direction.
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

public sealed class ReverseIterator<T> : IListIterator<T>
{
	// Internal state.
	protected IListIterator<T> iterator;

	// Constructor.
	public ReverseIterator(IListIterator<T> iterator)
			{
				this.iterator = iterator;
			}

	// Implement the IIterator<T> interface.
	public bool MoveNext()
			{
				return iterator.MovePrev();
			}
	public void Reset()
			{
				iterator.Reset();
			}
	public void Remove()
			{
				iterator.Remove();
			}
	T IIterator<T>.Current
			{
				get
				{
					return ((IIterator<T>)iterator).Current;
				}
			}

	// Implement the IListIterator<T> interface.
	public bool MovePrev()
			{
				return iterator.MoveNext();
			}
	public int Position
			{
				get
				{
					return iterator.Position;
				}
			}
	public T Current
			{
				get
				{
					return iterator.Current;
				}
				set
				{
					iterator.Current = value;
				}
			}

}; // class ReverseIterator<T>

}; // namespace Generics
