/*
 * StringEnumerator.cs - Implementation of
 *		"System.Collections.Specialized.StringEnumerator".
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

namespace System.Collections.Specialized
{

#if !ECMA_COMPAT

using System;
using System.Collections;

public class StringEnumerator
{
	// Internal state;
	private IEnumerator e;

	// Constructor.
	internal StringEnumerator(IEnumerator e)
			{
				this.e = e;
			}

	// Get the current element as a string.
	public String Current
			{
				get
				{
					return (String)(e.Current);
				}
			}

	// Move to the next string in the collection.
	public bool MoveNext()
			{
				return e.MoveNext();
			}

	// Reset to the beginning of the collection.
	public void Reset()
			{
				e.Reset();
			}

}; // class StringEnumerator

#endif // !ECMA_COMPAT

}; // namespace System.Collections.Specialized
