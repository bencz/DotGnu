/*
 * CodeConditionStatement.cs - Implementation of the
 *		System.CodeDom.CodeConditionStatement class.
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
public class CodeConditionStatement : CodeStatement
{

	// Internal state.
	private CodeExpression condition;
	private CodeStatementCollection trueStatements;
	private CodeStatementCollection falseStatements;

	// Constructors.
	public CodeConditionStatement()
			{
			}
	public CodeConditionStatement(CodeExpression condition,
								  params CodeStatement[] trueStatements)
			{
				this.condition = condition;
				TrueStatements.AddRange(trueStatements);
			}
	public CodeConditionStatement(CodeExpression condition,
								  CodeStatement[] trueStatements,
								  CodeStatement[] falseStatements)
			{
				this.condition = condition;
				TrueStatements.AddRange(trueStatements);
				FalseStatements.AddRange(falseStatements);
			}

	// Properties.
	public CodeExpression Condition
			{
				get
				{
					return condition;
				}
				set
				{
					condition = value;
				}
			}
	public CodeStatementCollection TrueStatements
			{
				get
				{
					if(trueStatements == null)
					{
						trueStatements = new CodeStatementCollection();
					}
					return trueStatements;
				}
			}
	public CodeStatementCollection FalseStatements
			{
				get
				{
					if(falseStatements == null)
					{
						falseStatements = new CodeStatementCollection();
					}
					return falseStatements;
				}
			}

}; // class CodeConditionStatement

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
