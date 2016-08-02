/*
 * CodeTypeReference.cs - Implementation of the
 *		System.CodeDom.CodeTypeReference class.
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
public class CodeTypeReference : CodeObject
{

	// Internal state.
	private String baseType;
	private CodeTypeReference arrayElementType;
	private int arrayRank;

	// Constructors.
	public CodeTypeReference(String typeName)
			{
				baseType = typeName;
				arrayElementType = null;
				arrayRank = 0;
			}
	public CodeTypeReference(Type type)
			{
				if(!(type.IsArray))
				{
					baseType = type.FullName;
					arrayElementType = null;
					arrayRank = 0;
				}
				else
				{
					baseType = null;
					arrayElementType =
						new CodeTypeReference(type.GetElementType());
					arrayRank = type.GetArrayRank();
				}
			}
	public CodeTypeReference(CodeTypeReference arrayType, int rank)
			{
				baseType = null;
				arrayElementType = arrayType;
				arrayRank = rank;
			}
	public CodeTypeReference(String baseType, int rank)
			{
				this.baseType = null;
				arrayElementType = new CodeTypeReference(baseType);
				arrayRank = rank;
			}

	// Properties.
	public CodeTypeReference ArrayElementType
			{
				get
				{
					return arrayElementType;
				}
				set
				{
					arrayElementType = value;
				}
			}
	public int ArrayRank
			{
				get
				{
					return arrayRank;
				}
				set
				{
					arrayRank = value;
				}
			}
	public String BaseType
			{
				get
				{
					if(arrayRank != 0 && arrayElementType != null)
					{
						return arrayElementType.BaseType;
					}
					else if(baseType != null)
					{
						return baseType;
					}
					else
					{
						return String.Empty;
					}
				}
				set
				{
					baseType = value;
				}
			}

}; // class CodeTypeReference

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
