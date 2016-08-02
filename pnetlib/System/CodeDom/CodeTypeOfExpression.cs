/*
 * CodeTypeOfExpression.cs - Implementation of the
 *		System.CodeDom.CodeTypeOfExpression class.
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
public class CodeTypeOfExpression : CodeExpression
{

	// Internal state.
	private CodeTypeReference type;

	// Constructors.
	public CodeTypeOfExpression()
			{
			}
	public CodeTypeOfExpression(CodeTypeReference type)
			{
				this.type = type;
			}
	public CodeTypeOfExpression(String type)
			{
				this.type = new CodeTypeReference(type);
			}
	public CodeTypeOfExpression(Type type)
			{
				this.type = new CodeTypeReference(type);
			}

	// Properties.
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

}; // class CodeTypeOfExpression

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
