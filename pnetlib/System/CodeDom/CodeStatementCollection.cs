/*
 * CodeStatementCollection.cs - Implementation of the
 *		System.CodeDom.CodeStatementCollection class.
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
public class CodeStatementCollection : CollectionBase
{

	// Constructors.
	public CodeStatementCollection()
			{
			}
	public CodeStatementCollection(CodeStatement[] value)
			{
				AddRange(value);
			}
	public CodeStatementCollection(CodeStatementCollection value)
			{
				AddRange(value);
			}

	// Properties.
	public CodeStatement this[int index]
			{
				get
				{
					return (CodeStatement)(List[index]);
				}
				set
				{
					List[index] = value;
				}
			}

	// Methods.
	public int Add(CodeStatement value)
			{
				return List.Add(value);
			}
	public int Add(CodeExpression value)
			{
				return Add(new CodeExpressionStatement(value));
			}
	public void AddRange(CodeStatement[] value)
			{
				foreach(CodeStatement e in value)
				{
					List.Add(e);
				}
			}
	public void AddRange(CodeStatementCollection value)
			{
				foreach(CodeStatement e in value)
				{
					List.Add(e);
				}
			}
	public bool Contains(CodeStatement value)
			{
				return List.Contains(value);
			}
	public void CopyTo(CodeStatement[] array, int index)
			{
				List.CopyTo(array, index);
			}
	public int IndexOf(CodeStatement value)
			{
				return List.IndexOf(value);
			}
	public void Insert(int index, CodeStatement value)
			{
				List.Insert(index, value);
			}
	public void Remove(CodeStatement value)
			{
				int index = List.IndexOf(value);
				if(index < 0)
				{
					throw new ArgumentException
						(S._("Arg_NotCollMember"), "value");
				}
				List.RemoveAt(index);
			}

}; // class CodeStatementCollection

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
