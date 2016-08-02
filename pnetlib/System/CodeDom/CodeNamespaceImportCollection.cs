/*
 * CodeNamespaceImportCollection.cs - Implementation of the
 *		System.CodeDom.CodeNamespaceImportCollection class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.CodeDom
{

#if CONFIG_CODEDOM

using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeNamespaceImportCollection : IList, ICollection, IEnumerable
{

	// Internal state.
	private ArrayList list;

	// Constructors.
	public CodeNamespaceImportCollection()
			{
				list = new ArrayList();
			}

	// Properties.
	public CodeNamespaceImport this[int index]
			{
				get
				{
					return (CodeNamespaceImport)(list[index]);
				}
				set
				{
					list[index] = value;
				}
			}

	// Add a new namespace import to this collection.
	public void Add(CodeNamespaceImport value)
			{
				list.Add(value);
			}

	// Add a range of namespace imports to this collection.
	public void AddRange(CodeNamespaceImport[] value)
			{
				list.AddRange(value);
			}

	// Get the number of items in this collection.
	public int Count
			{
				get
				{
					return list.Count;
				}
			}

	// Clear this collection.
	public void Clear()
			{
				list.Clear();
			}

	// Get an enumerator for this collection.
	public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}

	// Implement the IList interface.
	int IList.Add(Object value)
			{
				return list.Add((CodeNamespaceImport)value);
			}
	void IList.Clear()
			{
				Clear();
			}
	bool IList.Contains(Object value)
			{
				return list.Contains(value);
			}
	int IList.IndexOf(Object value)
			{
				return list.IndexOf((CodeNamespaceImport)value);
			}
	void IList.Insert(int index, Object value)
			{
				list.Insert(index, (CodeNamespaceImport)value);
			}
	void IList.Remove(Object value)
			{
				list.Remove((CodeNamespaceImport)value);
			}
	void IList.RemoveAt(int index)
			{
				list.RemoveAt(index);
			}
	bool IList.IsFixedSize
			{
				get
				{
					return false;
				}
			}
	bool IList.IsReadOnly
			{
				get
				{
					return false;
				}
			}
	Object IList.this[int index]
			{
				get
				{
					return list[index];
				}
				set
				{
					list[index] = (CodeNamespaceImport)value;
				}
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
					return list.Count;
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
					return null;
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

}; // class CodeNamespaceImportCollection

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
