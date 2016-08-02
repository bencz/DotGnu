/*
 * ComparerWrapper.cs - Wrap a non-generic comparer and make it generic.
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

public sealed class ComparerWrapper<T> : IComparer<T>
{

	// Internal state.
	private System.Collections.IComparer cmp;

	// Constructor.
	public ComparerWrapper(System.Collections.IComparer cmp)
			{
				if(cmp == null)
				{
					throw new ArgumentNullException("cmp");
				}
				this.cmp = cmp;
			}

	// Implement the IComparer<T> interface.
	public int Compare(T x, T y)
			{
				return cmp.Compare(x, y);
			}

}; // class ComparerWrapper<T>

}; // namespace Generics
