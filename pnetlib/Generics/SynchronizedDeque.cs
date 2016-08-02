/*
 * SynchronizedDeque.cs - Wrap a deque to make it synchronized.
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

public class SynchronizedDeque<T> : SynchronizedCollection<T>, IDeque<T>
{
	// Internal state.
	protected IDeque<T> deque;

	// Constructor.
	public SynchronizedDeque(IDeque<T> deque) : base(deque)
			{
				this.deque = deque;
			}

	// Implement the IDeque<T> interface.
	public void PushFront(T value)
			{
				lock(SyncRoot)
				{
					deque.PushFront(value);
				}
			}
	public void PushBack(T value)
			{
				lock(SyncRoot)
				{
					deque.PushBack(value);
				}
			}
	public T PopFront()
			{
				lock(SyncRoot)
				{
					return deque.PopFront();
				}
			}
	public T PopBack()
			{
				lock(SyncRoot)
				{
					return deque.PopBack();
				}
			}
	public T PeekFront()
			{
				lock(SyncRoot)
				{
					return deque.PeekFront();
				}
			}
	public T PeekEnd()
			{
				lock(SyncRoot)
				{
					return deque.PeekEnd();
				}
			}
	public T[] ToArray()
			{
				lock(SyncRoot)
				{
					return deque.ToArray();
				}
			}

	// Implement the ICloneable interface.
	public override Object Clone()
			{
				lock(SyncRoot)
				{
					if(deque is ICloneable)
					{
						return new SynchronizedDeque<T>
							((IDeque<T>)(((ICloneable)deque).Clone()));
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_NotCloneable"));
					}
				}
			}

}; // class SynchronizedDeque<T>

}; // namespace Generics
