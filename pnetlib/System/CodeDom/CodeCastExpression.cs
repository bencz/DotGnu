/*
 * CodeCastExpression.cs - Implementation of the
 *		System.CodeDom.CodeCastExpression class.
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
public class CodeCastExpression : CodeExpression
{

	// Internal state.
	private CodeTypeReference targetType;
	private CodeExpression expression;

	// Constructors.
	public CodeCastExpression()
			{
			}
	public CodeCastExpression(CodeTypeReference targetType,
							  CodeExpression expression)
			{
				this.targetType = targetType;
				this.expression = expression;
			}
	public CodeCastExpression(String targetType,
							  CodeExpression expression)
			{
				this.targetType = new CodeTypeReference(targetType);
				this.expression = expression;
			}
	public CodeCastExpression(Type targetType,
							  CodeExpression expression)
			{
				this.targetType = new CodeTypeReference(targetType);
				this.expression = expression;
			}

	// Properties.
	public CodeTypeReference TargetType
			{
				get
				{
					return targetType;
				}
				set
				{
					targetType = value;
				}
			}
	public CodeExpression Expression
			{
				get
				{
					return expression;
				}
				set
				{
					expression = value;
				}
			}

}; // class CodeCastExpression

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
