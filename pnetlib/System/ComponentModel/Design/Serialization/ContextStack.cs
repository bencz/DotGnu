/*
 * ContextStack.cs - Implementation of the
 *		"System.ComponentModel.Design.Serialization.ContextStack" class.
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

namespace System.ComponentModel.Design.Serialization
{

#if CONFIG_COMPONENT_MODEL_DESIGN

public sealed class ContextStack
{
	// Internal state.
	private Object[] stack;
	private int size;

	// Constructor.
	public ContextStack()
			{
				stack = null;
				size = 0;
			}

	// Get the current object on the context stack.
	public Object Current
			{
				get
				{
					if(size > 0)
					{
						return stack[size - 1];
					}
					else
					{
						return null;
					}
				}
			}

	// Get the first object on the context stack of a particular type.
	public Object this[Type type]
			{
				get
				{
					int posn = size - 1;
					while(posn >= 0)
					{
						if(type.IsAssignableFrom(stack[posn].GetType()))
						{
							return stack[posn];
						}
						--posn;
					}
					return null;
				}
			}

	// Get the object "level" items down the context stack.
	public Object this[int level]
			{
				get
				{
					if(level < size)
					{
						return stack[size - 1 - level];
					}
					else
					{
						return null;
					}
				}
			}

	// Pop the top-most item from the context stack.
	public Object Pop()
			{
				if(size > 0)
				{
					--size;
					return stack[size];
				}
				else
				{
					return null;
				}
			}

	// Push a new object onto the context stack.
	public void Push(Object context)
			{
				if(context == null)
				{
					throw new ArgumentNullException("context");
				}
				if(stack == null)
				{
					stack = new Object [16];
				}
				else if(size >= stack.Length)
				{
					Object[] newStack = new Object [size * 2];
					Array.Copy(stack, 0, newStack, 0, size);
					stack = newStack;
				}
				stack[size++] = context;
			}

}; // class ContextStack

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design.Serialization
