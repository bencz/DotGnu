/*
 * CodeTypeReferenceExpression.cs - Implementation of the
 *		System.CodeDom.CodeTypeReferenceExpression class.
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
public class CodeTypeReferenceExpression : CodeExpression
{

	// Internal state.
	private CodeTypeReference type;

	// Constructors.
	public CodeTypeReferenceExpression()
			{
			}
	public CodeTypeReferenceExpression(CodeTypeReference type)
			{
				this.type = type;
			}
	public CodeTypeReferenceExpression(String type)
			{
				this.type = new CodeTypeReference(type);
			}
	public CodeTypeReferenceExpression(Type type)
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

}; // class CodeTypeReferenceExpression

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
