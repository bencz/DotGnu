/*
 * CodeMemberMethod.cs - Implementation of the
 *		System.CodeDom.CodeMemberMethod class.
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
public class CodeMemberMethod : CodeTypeMember
{

	// Internal state.
	private CodeTypeReferenceCollection implementationTypes;
	private CodeParameterDeclarationExpressionCollection parameters;
	private CodeTypeReference privateImplementationType;
	private CodeAttributeDeclarationCollection returnAttrs;
	private CodeTypeReference returnType;
	private CodeStatementCollection statements;

	// Constructors.
	public CodeMemberMethod()
			{
			}

	// Properties.
	public CodeTypeReferenceCollection ImplementationTypes
			{
				get
				{
					if(implementationTypes == null)
					{
						implementationTypes = new CodeTypeReferenceCollection();
						if(PopulateImplementationTypes != null)
						{
							PopulateImplementationTypes(this, EventArgs.Empty);
						}
					}
					return implementationTypes;
				}
			}
	public CodeParameterDeclarationExpressionCollection Parameters
			{
				get
				{
					if(parameters == null)
					{
						parameters =
							new CodeParameterDeclarationExpressionCollection();
						if(PopulateParameters != null)
						{
							PopulateParameters(this, EventArgs.Empty);
						}
					}
					return parameters;
				}
			}
	public CodeTypeReference PrivateImplementationType
			{
				get
				{
					return privateImplementationType;
				}
				set
				{
					privateImplementationType = value;
				}
			}
	public CodeTypeReference ReturnType
			{
				get
				{
					return returnType;
				}
				set
				{
					returnType = value;
				}
			}
	public CodeAttributeDeclarationCollection ReturnTypeCustomAttributes
			{
				get
				{
					if(returnAttrs == null)
					{
						returnAttrs = new CodeAttributeDeclarationCollection();
					}
					return returnAttrs;
				}
			}
	public CodeStatementCollection Statements
			{
				get
				{
					if(statements == null)
					{
						statements = new CodeStatementCollection();
						if(PopulateStatements != null)
						{
							PopulateStatements(this, EventArgs.Empty);
						}
					}
					return statements;
				}
			}

	// Events.
	public event EventHandler PopulateImplementationTypes;
	public event EventHandler PopulateParameters;
	public event EventHandler PopulateStatements;

}; // class CodeMemberMethod

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
