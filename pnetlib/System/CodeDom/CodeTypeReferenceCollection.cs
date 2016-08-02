/*
 * CodeTypeReferenceCollection.cs - Implementation of the
 *		System.CodeDom.CodeTypeReferenceCollection class.
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
public class CodeTypeReferenceCollection : CollectionBase
{

	// Constructors.
	public CodeTypeReferenceCollection()
			{
			}
	public CodeTypeReferenceCollection(CodeTypeReference[] value)
			{
				AddRange(value);
			}
	public CodeTypeReferenceCollection(CodeTypeReferenceCollection value)
			{
				AddRange(value);
			}

	// Properties.
	public CodeTypeReference this[int index]
			{
				get
				{
					return (CodeTypeReference)(List[index]);
				}
				set
				{
					List[index] = value;
				}
			}

	// Methods.
	public int Add(CodeTypeReference value)
			{
				return List.Add(value);
			}
	public void Add(String value)
			{
				Add(new CodeTypeReference(value));
			}
	public void Add(Type value)
			{
				Add(new CodeTypeReference(value));
			}
	public void AddRange(CodeTypeReference[] value)
			{
				foreach(CodeTypeReference e in value)
				{
					List.Add(e);
				}
			}
	public void AddRange(CodeTypeReferenceCollection value)
			{
				foreach(CodeTypeReference e in value)
				{
					List.Add(e);
				}
			}
	public bool Contains(CodeTypeReference value)
			{
				return List.Contains(value);
			}
	public void CopyTo(CodeTypeReference[] array, int index)
			{
				List.CopyTo(array, index);
			}
	public int IndexOf(CodeTypeReference value)
			{
				return List.IndexOf(value);
			}
	public void Insert(int index, CodeTypeReference value)
			{
				List.Insert(index, value);
			}
	public void Remove(CodeTypeReference value)
			{
				int index = List.IndexOf(value);
				if(index < 0)
				{
					throw new ArgumentException
						(S._("Arg_NotCollMember"), "value");
				}
				List.RemoveAt(index);
			}

}; // class CodeTypeReferenceCollection

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
