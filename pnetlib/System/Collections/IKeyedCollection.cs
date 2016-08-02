/*
 * IKeyedCollection.cs - Implementation of
 *		"System.Collections.IKeyedCollection".
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

namespace System.Collections
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

public interface IKeyedCollection
{
	// Add an item to this keyed collection.
	int Add(Object value, Object key);

	// Determine if this collection contains a particular key.
	bool ContainsKey(Object key);

	// Get the value associated with a particular key.
	Object GetValue(Object key);

	// Get the index associated with a particular key.
	int IndexOfKey(Object key);

	// Insert a new keyed value into this collection.
	void Insert(int index, Object value, Object key);
	void InsertAfter(Object afterKey, Object value, Object key);
	void InsertBefore(Object beforeKey, Object value, Object key);

	// Remove the item with a particular key.
	void RemoveByKey(Object key);

	// Set a particular key to a value.
	void SetValue(Object value, Object key);

}; // interface IKeyedCollection

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

}; // namespace System.Collections
