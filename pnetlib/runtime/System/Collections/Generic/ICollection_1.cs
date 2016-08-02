/*
 * ICollection_1.cs - Implementation of the
 *		"System.Collections.Generic.ICollection<T>" class.
 *
 * Copyright (C) 2003, 2008  Southern Storm Software, Pty Ltd.
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

namespace System.Collections.Generic
{

#if CONFIG_FRAMEWORK_2_0

public interface ICollection<T> : IEnumerable<T>
{
	void   Add(T item);
	void   Clear();
	bool   Contains(T item);
	void   CopyTo(T[] array, int index);
	bool   Remove(T item);
	int    Count { get; }
	bool   IsReadOnly { get; }

}; // interface ICollection<T>

#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System.Collections.Generic
