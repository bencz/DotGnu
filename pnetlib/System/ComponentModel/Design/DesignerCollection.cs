/*
 * DesignerCollection.cs - Implementation of the
 *		"System.ComponentModel.Design.DesignerCollection" class.
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

using System.Collections;

public class DesignerCollection : ICollection, IEnumerable
{
	// Internal state.
	private IList list;

	// Constructors.
	public DesignerCollection(IDesignerHost[] designers)
			{
				if(designers == null)
				{
					list = new ArrayList();
				}
				else
				{
					list = new ArrayList(designers);
				}
			}
	public DesignerCollection(IList designers)
			{
				list = designers;
			}

	// Get the number of elements in the collection.
	public int Count
			{
				get
				{
					return list.Count;
				}
			}

	// Get the designer at a specific index.
	public virtual IDesignerHost this[int index]
			{
				get
				{
					return (IDesignerHost)(list[index]);
				}
			}

	// Get an enumerator for this collection.
	public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				list.CopyTo(array, index);
			}
	int ICollection.Count
			{
				get
				{
					return Count;
				}
			}
	bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}
	Object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

}; // class DesignerCollection

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
