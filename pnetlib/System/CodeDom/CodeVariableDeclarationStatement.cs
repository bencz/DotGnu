/*
 * CodeVariableDeclarationStatement.cs - Implementation of the
 *		System.CodeDom.CodeVariableDeclarationStatement class.
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
public class CodeVariableDeclarationStatement : CodeStatement
{

	// Internal state.
	private CodeTypeReference type;
	private String name;
	private CodeExpression initExpression;

	// Constructors.
	public CodeVariableDeclarationStatement()
			{
			}
	public CodeVariableDeclarationStatement(CodeTypeReference type,
											String name)
			{
				this.type = type;
				this.name = name;
			}
	public CodeVariableDeclarationStatement(String type, String name)
			{
				this.type = new CodeTypeReference(type);
				this.name = name;
			}
	public CodeVariableDeclarationStatement(Type type, String name)
			{
				this.type = new CodeTypeReference(type);
				this.name = name;
			}
	public CodeVariableDeclarationStatement(CodeTypeReference type,
											String name,
											CodeExpression initExpression)
			{
				this.type = type;
				this.name = name;
				this.initExpression = initExpression;
			}
	public CodeVariableDeclarationStatement(String type, String name,
											CodeExpression initExpression)
			{
				this.type = new CodeTypeReference(type);
				this.name = name;
				this.initExpression = initExpression;
			}
	public CodeVariableDeclarationStatement(Type type, String name,
											CodeExpression initExpression)
			{
				this.type = new CodeTypeReference(type);
				this.name = name;
				this.initExpression = initExpression;
			}

	// Properties.
	public CodeExpression InitExpression
			{
				get
				{
					return initExpression;
				}
				set
				{
					initExpression = value;
				}
			}
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}
	public CodeTypeReference Type
			{
				get
				{
					return type;
				}
				set
				{
					type = value;
				}
			}

}; // class CodeVariableDeclarationStatement

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
