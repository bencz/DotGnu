/*
 * CompilerErrorCollection.cs - Implementation of the
 *		System.CodeDom.Compiler.CompilerErrorCollection class.
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

namespace System.CodeDom.Compiler
{

#if CONFIG_CODEDOM

using System.Collections;

public class CompilerErrorCollection : CollectionBase
{

	// Constructors.
	public CompilerErrorCollection()
			{
			}
	public CompilerErrorCollection(CompilerError[] value)
			{
				AddRange(value);
			}
	public CompilerErrorCollection(CompilerErrorCollection value)
			{
				AddRange(value);
			}

	// Properties.
	public bool HasErrors
			{
				get
				{
					if(Count == 0)
					{
						return false;
					}
					foreach(CompilerError error in this)
					{
						if(!(error.IsWarning))
						{
							return true;
						}
					}
					return false;
				}
			}
	public bool HasWarnings
			{
				get
				{
					if(Count == 0)
					{
						return false;
					}
					foreach(CompilerError error in this)
					{
						if(error.IsWarning)
						{
							return true;
						}
					}
					return false;
				}
			}
	public CompilerError this[int index]
			{
				get
				{
					return (CompilerError)(List[index]);
				}
				set
				{
					List[index] = value;
				}
			}

	// Add an error to this collection.
	public int Add(CompilerError value)
			{
				return List.Add(value);
			}

	// Add a range of errors to this collection.
	public void AddRange(CompilerError[] value)
			{
				foreach(CompilerError error in value)
				{
					List.Add(error);
				}
			}
	public void AddRange(CompilerErrorCollection value)
			{
				foreach(CompilerError error in value)
				{
					List.Add(error);
				}
			}

	// Determine if this collection contains a specific error.
	public bool Contains(CompilerError value)
			{
				return List.Contains(value);
			}

	// Copy the contents of this collection to an array.
	public void CopyTo(CompilerError[] array, int index)
			{
				List.CopyTo(array, index);
			}

	// Get the index of a specific error in this collection.
	public int IndexOf(CompilerError value)
			{
				return List.IndexOf(value);
			}

	// Insert an error into this collection.
	public void Insert(int index, CompilerError value)
			{
				List.Insert(index, value);
			}

	// Remove an error from this collection.
	public void Remove(CompilerError value)
			{
				int index = List.IndexOf(value);
				if(index < 0)
				{
					throw new ArgumentException
						(S._("Arg_NotCollMember"), "value");
				}
				List.RemoveAt(index);
			}

}; // class CompilerErrorCollection

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
