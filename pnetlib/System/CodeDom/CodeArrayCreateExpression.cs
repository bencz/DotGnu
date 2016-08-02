/*
 * CodeArrayCreateExpression.cs - Implementation of the
 *		System.CodeDom.CodeArrayCreateExpression class.
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
public class CodeArrayCreateExpression : CodeExpression
{

	// Internal state.
	private CodeTypeReference createType;
	private int size;
	private CodeExpression sizeExpr;
	private CodeExpressionCollection initializers;

	// Constructors.
	public CodeArrayCreateExpression()
			{
			}
	public CodeArrayCreateExpression(CodeTypeReference createType,
									 CodeExpression size)
			{
				this.createType = createType;
				this.sizeExpr = size;
			}
	public CodeArrayCreateExpression(CodeTypeReference createType,
									 params CodeExpression[] initializers)
			{
				this.createType = createType;
				Initializers.AddRange(initializers);
			}
	public CodeArrayCreateExpression(CodeTypeReference createType,
									 int size)
			{
				this.createType = createType;
				this.size = size;
			}
	public CodeArrayCreateExpression(String createType,
									 CodeExpression size)
			{
				this.createType = new CodeTypeReference(createType);
				this.sizeExpr = size;
			}
	public CodeArrayCreateExpression(String createType,
									 params CodeExpression[] initializers)
			{
				this.createType = new CodeTypeReference(createType);
				Initializers.AddRange(initializers);
			}
	public CodeArrayCreateExpression(String createType, int size)
			{
				this.createType = new CodeTypeReference(createType);
				this.size = size;
			}
	public CodeArrayCreateExpression(Type createType,
									 CodeExpression size)
			{
				this.createType = new CodeTypeReference(createType);
				this.sizeExpr = size;

			}
	public CodeArrayCreateExpression(Type createType,
									 params CodeExpression[] initializers)
			{
				this.createType = new CodeTypeReference(createType);
				Initializers.AddRange(initializers);
			}
	public CodeArrayCreateExpression(Type createType, int size)
			{
				this.createType = new CodeTypeReference(createType);
				this.size = size;
			}

	// Properties.
	public CodeTypeReference CreateType
			{
				get
				{
					return createType;
				}
				set
				{
					createType = value;
				}
			}
	public CodeExpressionCollection Initializers
			{
				get
				{
					if(initializers == null)
					{
						initializers = new CodeExpressionCollection();
					}
					return initializers;
				}
			}
	public int Size
			{
				get
				{
					return size;
				}
				set
				{
					size = value;
				}
			}
	public CodeExpression SizeExpression
			{
				get
				{
					return sizeExpr;
				}
				set
				{
					sizeExpr = value;
				}
			}

}; // class CodeArrayCreateExpression

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
