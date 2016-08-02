/*
 * ReadOnlyQueue.cs - Wrap a queue to make it read-only.
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

public class ReadOnlyQueue<T> : ReadOnlyCollection<T>, IQueue<T>
{
	// Internal state.
	protected IQueue<T> queue;

	// Constructor.
	public ReadOnlyQueue(IQueue<T> queue) : base(queue)
			{
				this.queue = queue;
			}

	// Implement the IQueue<T> interface.
	public void Clear()
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public bool Contains(T value)
			{
				return queue.Contains();
			}
	public void Enqueue(T value)
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public T Dequeue()
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public T Peek()
			{
				return queue.Peek();
			}
	public T[] ToArray()
			{
				return queue.ToArray();
			}

	// Implement the ICloneable interface.
	public override Object Clone()
			{
				if(queue is ICloneable)
				{
					return new ReadOnlyQueue<T>
						((IQueue<T>)(((ICloneable)queue).Clone()));
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_NotCloneable"));
				}
			}

}; // class ReadOnlyQueue<T>

}; // namespace Generics
