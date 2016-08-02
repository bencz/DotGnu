/*
 * CodeTypeDeclaration.cs - Implementation of the
 *		System.CodeDom.CodeTypeDeclaration class.
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
using System.Reflection;

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeTypeDeclaration : CodeTypeMember
{

	// Type category.
	private enum CodeTypeCategory
	{
		Class,
		Enum,
		Interface,
		Struct

	};

	// Internal state.
	private CodeTypeReferenceCollection baseTypes;
	private CodeTypeMemberCollection members;
	private CodeTypeCategory category;
	private TypeAttributes typeAttributes;

	// Constructors.
	public CodeTypeDeclaration()
			{
			}
	public CodeTypeDeclaration(String name)
			{
				Name = name;
			}

	// Properties.
	public CodeTypeReferenceCollection BaseTypes
			{
				get
				{
					if(baseTypes == null)
					{
						baseTypes = new CodeTypeReferenceCollection();
						if(PopulateBaseTypes != null)
						{
							PopulateBaseTypes(this, EventArgs.Empty);
						}
					}
					return baseTypes;
				}
			}
	public CodeTypeMemberCollection Members
			{
				get
				{
					if(members == null)
					{
						members = new CodeTypeMemberCollection();
						if(PopulateMembers != null)
						{
							PopulateMembers(this, EventArgs.Empty);
						}
					}
					return members;
				}
			}
	public bool IsClass
			{
				get
				{
					return (category == CodeTypeCategory.Class);
				}
				set
				{
					if(value)
					{
						category = CodeTypeCategory.Class;
					}
				}
			}
	public bool IsEnum
			{
				get
				{
					return (category == CodeTypeCategory.Enum);
				}
				set
				{
					if(value)
					{
						category = CodeTypeCategory.Enum;
					}
				}
			}
	public bool IsStruct
			{
				get
				{
					return (category == CodeTypeCategory.Struct);
				}
				set
				{
					if(value)
					{
						category = CodeTypeCategory.Struct;
					}
				}
			}
	public bool IsInterface
			{
				get
				{
					return (category == CodeTypeCategory.Interface);
				}
				set
				{
					if(value)
					{
						category = CodeTypeCategory.Interface;
					}
				}
			}
	public TypeAttributes TypeAttributes
			{
				get
				{
					return typeAttributes;
				}
				set
				{
					typeAttributes = value;
				}
			}

	// Events.
	public event EventHandler PopulateBaseTypes;
	public event EventHandler PopulateMembers;

}; // class CodeTypeDeclaration

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
