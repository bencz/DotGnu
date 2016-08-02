/*
 * CodeCatchClause.cs - Implementation of the
 *		System.CodeDom.CodeCatchClause class.
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
public class CodeCatchClause
{

	// Internal state.
	private String localName;
	private CodeTypeReference catchExceptionType;
	private CodeStatementCollection statements;

	// Constructors.
	public CodeCatchClause()
			{
			}
	public CodeCatchClause(String localName)
			{
				this.localName = localName;
			}
	public CodeCatchClause(String localName,
						   CodeTypeReference catchExceptionType)
			{
				this.localName = localName;
				this.catchExceptionType = catchExceptionType;
			}
	public CodeCatchClause(String localName,
						   CodeTypeReference catchExceptionType,
						   params CodeStatement[] statements)
			{
				this.localName = localName;
				this.catchExceptionType = catchExceptionType;
				Statements.AddRange(statements);
			}

	// Properties.
	public String LocalName
			{
				get
				{
					return localName;
				}
				set
				{
					localName = value;
				}
			}
	public CodeTypeReference CatchExceptionType
			{
				get
				{
					return catchExceptionType;
				}
				set
				{
					catchExceptionType = value;
				}
			}
	public CodeStatementCollection Statements
			{
				get
				{
					if(statements == null)
					{
						statements = new CodeStatementCollection();
					}
					return statements;
				}
			}

}; // class CodeCatchClause

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
