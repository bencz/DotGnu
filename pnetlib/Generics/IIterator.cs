/*
 * IIterator.cs - Generic collection iterators.
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

public interface IIterator<T>
{

	// Move to the next element in the current iteration.
	bool MoveNext();

	// Reset the iterator back to its starting position.
	void Reset();

	// Remove the current element (InvalidOperationException if not supported).
	// The iterator is invalid until the next call to "MoveNext".
	void Remove();

	// Get the value of the current element.
	T Current { get; }

}; // interface IIterator<T>

}; // namespace Generics
