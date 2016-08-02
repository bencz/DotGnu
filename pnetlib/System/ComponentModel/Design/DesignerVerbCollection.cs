/*
 * DesignerVerbCollection.cs - Implementation of the
 *		"System.ComponentModel.Design.DesignerVerbCollection" class.
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

namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN

using System.Runtime.InteropServices;
using System.Collections;

[ComVisible(true)]
public class DesignerVerbCollection : CollectionBase
{
	// Constructors.
	public DesignerVerbCollection() {}
	public DesignerVerbCollection(DesignerVerb[] value)
			{
				AddRange(value);
			}

	// Get or set a collection element.
	public DesignerVerb this[int index]
			{
				get
				{
					return (DesignerVerb)(((IList)this)[index]);
				}
				set
				{
					((IList)this)[index] = value;
				}
			}

	// Add an element to this collection.
	public int Add(DesignerVerb value)
			{
				return ((IList)this).Add(value);
			}

	// Add a range of values to this collection.
	public void AddRange(DesignerVerb[] value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(DesignerVerb verb in value)
				{
					Add(verb);
				}
			}
	public void AddRange(DesignerVerbCollection value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(DesignerVerb verb in value)
				{
					Add(verb);
				}
			}

	// Determine if a particular element is contained in this collection.
	public bool Contains(DesignerVerb value)
			{
				return ((IList)this).Contains(value);
			}

	// Copy the contents of this collection into an array.
	public void CopyTo(DesignerVerb[] array, int index)
			{
				((IList)this).CopyTo(array, index);
			}

	// Get the index of a specific collection element.
	public int IndexOf(DesignerVerb value)
			{
				return ((IList)this).IndexOf(value);
			}

	// Insert an element into this collection.
	public void Insert(int index, DesignerVerb value)
			{
				((IList)this).Insert(index, value);
			}

	// Remove an element from this collection.
	public void Remove(DesignerVerb value)
			{
				((IList)this).Remove(value);
			}

	// Trap modifications to the collection.  We don't actually
	// need to do anything except just override the methods.
	protected override void OnClear() {}
	protected override void OnInsert(int index, Object value) {}
	protected override void OnRemove(int index, Object value) {}
	protected override void OnSet
				(int index, Object oldValue, Object newValue) {}
	protected override void OnValidate(Object value) {}

}; // class DesignerVerbCollection

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
