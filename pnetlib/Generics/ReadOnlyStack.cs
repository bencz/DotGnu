/*
 * ReadOnlyStack.cs - Wrap a stack to make it read-only.
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

public class ReadOnlyStack<T> : ReadOnlyCollection<T>, IStack<T>
{
	// Internal state.
	protected IStack<T> stack;

	// Constructor.
	public ReadOnlyStack(IStack<T> stack) : base(stack)
			{
				this.stack = stack;
			}

	// Implement the IStack<T> interface.
	public void Clear()
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public bool Contains(T value)
			{
				return stack.Contains(value);
			}
	public void Push(T value)
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public T Pop()
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public T Peek()
			{
				return stack.Peek();
			}
	public T[] ToArray()
			{
				return stack.ToArray();
			}

	// Implement the ICloneable interface.
	public override Object Clone()
			{
				if(stack is ICloneable)
				{
					return new ReadOnlyStack<T>
						((IStack<T>)(((ICloneable)stack).Clone()));
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_NotCloneable"));
				}
			}

}; // class ReadOnlyStack<T>

}; // namespace Generics
