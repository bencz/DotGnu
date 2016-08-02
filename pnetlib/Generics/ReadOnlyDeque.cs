/*
 * ReadOnlyDeque.cs - Wrap a deque to make it read-only.
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

public class ReadOnlyDeque<T> : ReadOnlyCollection<T>, IDeque<T>
{
	// Internal state.
	protected IDeque<T> deque;

	// Constructor.
	public ReadOnlyDeque(IDeque<T> deque) : base(deque)
			{
				this.deque = deque;
			}

	// Implement the IDeque<T> interface.
	public void PushFront(T value)
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public void PushBack(T value)
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public T PopFront()
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public T PopBack()
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public T PeekFront()
			{
				return deque.PeekFront();
			}
	public T PeekEnd()
			{
				return deque.PeekEnd();
			}
	public T[] ToArray()
			{
				return deque.ToArray();
			}

	// Implement the ICloneable interface.
	public override Object Clone()
			{
				if(deque is ICloneable)
				{
					return new ReadOnlyDeque<T>
						((IDeque<T>)(((ICloneable)deque).Clone()));
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_NotCloneable"));
				}
			}

}; // class ReadOnlyDeque<T>

}; // namespace Generics
