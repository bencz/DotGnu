/*
 * ForIn.cs - Node type that implements "for ... in ..." statements.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.Collections;

// Dummy class for backwards-compatibility.

public sealed class ForIn : AST
{
	// Constructor.
	internal ForIn() : base() {}

	// Get an appropriate JScript enumerator for a collection.
	public static IEnumerator JScriptGetEnumerator(Object coll)
			{
				if(coll is IEnumerator)
				{
					return (IEnumerator)coll;
				}
				else if(coll is ScriptObject)
				{
					return ((ScriptObject)coll).GetPropertyEnumerator();
				}
				else if(coll is Array)
				{
					Array array = (Array)coll;
					return new ArrayIndexEnumerator(array.GetLowerBound(0),
													array.GetUpperBound(0));
				}
				else if(coll is IEnumerable)
				{
					IEnumerator e = ((IEnumerable)coll).GetEnumerator();
					if(e == null)
					{
						return new NullEnumerator();
					}
					else
					{
						return e;
					}
				}
				else
				{
					throw new JScriptException(JSError.NotCollection);
				}
			}

	// Enumerator class for indexes in an array range.
	private sealed class ArrayIndexEnumerator : IEnumerator
	{
		// Internal state.
		private int lower;
		private int upper;
		private int current;

		// Constructor.
		public ArrayIndexEnumerator(int lower, int upper)
				{
					this.lower = lower;
					this.upper = upper;
					this.current = lower - 1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					++current;
					return (current <= upper);
				}
		public void Reset()
				{
					current = lower - 1;
				}
		public Object Current
				{
					get
					{
						return current;
					}
				}

	}; // class ArrayIndexEnumerator

}; // class ForIn

}; // namespace Microsoft.JScript
