/*
 * IBindingList.cs - Implementation of the
 *		"System.ComponentModel.IBindingList" interface.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System;
using System.Collections;

public interface IBindingList : IList, ICollection, IEnumerable
{

	// Determine if editing is allowed.
	bool AllowEdit { get; }

	// Determine if it is possible to add new items to the list.
	bool AllowNew { get; }

	// Determine if it is possible to remove items from the list.
	bool AllowRemove { get; }

	// Determine if the list is sorted.
	bool IsSorted { get; }

	// Get the list sort direction.
	ListSortDirection SortDirection { get; }

	// Get the property that is being used for sorting.
	PropertyDescriptor SortProperty { get; }

	// Determine if this list supports change notification.
	bool SupportsChangeNotification { get; }

	// Determine if this list supports searching.
	bool SupportsSearching { get; }

	// Determine if this list supports sorting.
	bool SupportsSorting { get; }

	// Add a property descriptor to the search indexes.
	void AddIndex(PropertyDescriptor property);

	// Add a new item to the list.
	Object AddNew();

	// Sort based on a property and sort direction.
	void ApplySort(PropertyDescriptor property, ListSortDirection direction);

	// Find the index of an object with a particular property value.
	int Find(PropertyDescriptor property, Object key);

	// Remove a property descriptor from the search indexes.
	void RemoveIndex(PropertyDescriptor property);

	// Remove any sorting that was applied by "ApplySort".
	void RemoveSort();

	// Event that is raised when the list changes.
	event ListChangedEventHandler ListChanged;

}; // interface IBindingList

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
