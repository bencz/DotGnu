/*
 * Validator.cs - Implementation of the
 *		System.CodeDom.Compiler.Validator class.
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

namespace System.CodeDom.Compiler
{

#if CONFIG_CODEDOM

using System.IO;
using System.Reflection;
using System.Globalization;
using System.Collections;

// Helper class for validating the contents of a CodeDom tree.

internal sealed class Validator
{
	// Validate an identifier.
	private static void ValidateIdentifier(String value)
			{
				if(!CodeGenerator.IsValidLanguageIndependentIdentifier(value))
				{
					throw new ArgumentException(S._("Arg_InvalidIdentifier"));
				}
			}

	// Validate a qualified identifier.
	private static void ValidateQualifiedIdentifier
				(String value, bool canBeNull)
			{
				if(value == null)
				{
					if(!canBeNull)
					{
						throw new ArgumentException
							(S._("Arg_InvalidIdentifier"));
					}
				}
				else
				{
					int posn = 0;
					int index;
					String component;
					while((index = value.IndexOf('.', posn)) != -1)
					{
						component = value.Substring(posn, index - posn);
						ValidateIdentifier(component);
						posn = index + 1;
					}
					component = value.Substring(posn);
					ValidateIdentifier(component);
				}
			}

	// Validate a code object.
	public static void Validate(Object e)
			{
				if(e == null)
				{
					// Nothing to do here.
				}
				else if(e is ICollection)
				{
					IEnumerator en = ((ICollection)e).GetEnumerator();
					while(en.MoveNext())
					{
						Validate(en.Current);
					}
				}
				else if(e is CodeArrayCreateExpression)
				{
					Validate(((CodeArrayCreateExpression)e).CreateType);
					Validate(((CodeArrayCreateExpression)e).Initializers);
					Validate(((CodeArrayCreateExpression)e).SizeExpression);
				}
				else if(e is CodeAttachEventStatement)
				{
					Validate(((CodeAttachEventStatement)e).Event);
					Validate(((CodeAttachEventStatement)e).Listener);
				}
				else if(e is CodeCastExpression)
				{
					Validate(((CodeCastExpression)e).TargetType);
					Validate(((CodeCastExpression)e).Expression);
				}
				else if(e is CodeCatchClause)
				{
					ValidateIdentifier(((CodeCatchClause)e).LocalName);
					Validate(((CodeCatchClause)e).CatchExceptionType);
					Validate(((CodeCatchClause)e).Statements);
				}
				else if(e is CodeConditionStatement)
				{
					Validate(((CodeConditionStatement)e).Condition);
					Validate(((CodeConditionStatement)e).TrueStatements);
					Validate(((CodeConditionStatement)e).FalseStatements);
				}
				else if(e is CodeGotoStatement)
				{
					ValidateIdentifier(((CodeGotoStatement)e).Label);
				}
				else if(e is CodeMemberEvent)
				{
					Validate(((CodeMemberEvent)e).CustomAttributes);
					ValidateIdentifier(((CodeMemberEvent)e).Name);
					Validate(((CodeMemberEvent)e).ImplementationTypes);
					Validate(((CodeMemberEvent)e).PrivateImplementationType);
					Validate(((CodeMemberEvent)e).Type);
				}
				else if(e is CodeMemberField)
				{
					Validate(((CodeMemberField)e).CustomAttributes);
					ValidateIdentifier(((CodeMemberField)e).Name);
					Validate(((CodeMemberField)e).Type);
					Validate(((CodeMemberField)e).InitExpression);
				}
				else if(e is CodeConstructor)
				{
					Validate(((CodeConstructor)e).CustomAttributes);
					ValidateIdentifier(((CodeConstructor)e).Name);
					Validate(((CodeConstructor)e).ImplementationTypes);
					Validate(((CodeConstructor)e).Parameters);
					Validate(((CodeConstructor)e).PrivateImplementationType);
					Validate(((CodeConstructor)e).ReturnType);
					Validate(((CodeConstructor)e).ReturnTypeCustomAttributes);
					Validate(((CodeConstructor)e).Statements);
					Validate(((CodeConstructor)e).BaseConstructorArgs);
					Validate(((CodeConstructor)e).ChainedConstructorArgs);
				}
				else if(e is CodeMemberMethod)
				{
					Validate(((CodeMemberMethod)e).CustomAttributes);
					ValidateIdentifier(((CodeMemberMethod)e).Name);
					Validate(((CodeMemberMethod)e).ImplementationTypes);
					Validate(((CodeMemberMethod)e).Parameters);
					Validate(((CodeMemberMethod)e).PrivateImplementationType);
					Validate(((CodeMemberMethod)e).ReturnType);
					Validate(((CodeMemberMethod)e).ReturnTypeCustomAttributes);
					Validate(((CodeMemberMethod)e).Statements);
				}
				else if(e is CodeMemberProperty)
				{
					Validate(((CodeMemberProperty)e).CustomAttributes);
					ValidateIdentifier(((CodeMemberProperty)e).Name);
					Validate(((CodeMemberProperty)e).GetStatements);
					Validate(((CodeMemberProperty)e).ImplementationTypes);
					Validate(((CodeMemberProperty)e).Parameters);
					Validate(((CodeMemberProperty)e).PrivateImplementationType);
					Validate(((CodeMemberProperty)e).SetStatements);
					Validate(((CodeMemberProperty)e).Type);
				}
				else if(e is CodeMethodInvokeExpression)
				{
					Validate(((CodeMethodInvokeExpression)e).Method);
					Validate(((CodeMethodInvokeExpression)e).Parameters);
				}
				else if(e is CodeNamespace)
				{
					Validate(((CodeNamespace)e).Imports);
					ValidateQualifiedIdentifier(((CodeNamespace)e).Name, false);
					Validate(((CodeNamespace)e).Types);
				}
				else if(e is CodeNamespaceImport)
				{
					ValidateQualifiedIdentifier
						(((CodeNamespaceImport)e).Namespace, false);
				}
				else if(e is CodeObjectCreateExpression)
				{
					Validate(((CodeObjectCreateExpression)e).CreateType);
					Validate(((CodeObjectCreateExpression)e).Parameters);
				}
				else if(e is CodeParameterDeclarationExpression)
				{
					Validate(((CodeParameterDeclarationExpression)e)
								.CustomAttributes);
					ValidateIdentifier
						(((CodeParameterDeclarationExpression)e).Name);
					Validate(((CodeParameterDeclarationExpression)e).Type);
				}
				else if(e is CodeRemoveEventStatement)
				{
					Validate(((CodeRemoveEventStatement)e).Event);
					Validate(((CodeRemoveEventStatement)e).Listener);
				}
				else if(e is CodeTypeDelegate)
				{
					Validate(((CodeTypeDelegate)e).CustomAttributes);
					ValidateIdentifier(((CodeTypeDelegate)e).Name);
					Validate(((CodeTypeDelegate)e).BaseTypes);
					Validate(((CodeTypeDelegate)e).Members);
					Validate(((CodeTypeDelegate)e).Parameters);
					Validate(((CodeTypeDelegate)e).ReturnType);
				}
				else if(e is CodeTypeDeclaration)
				{
					Validate(((CodeTypeDeclaration)e).CustomAttributes);
					ValidateIdentifier(((CodeTypeDeclaration)e).Name);
					Validate(((CodeTypeDeclaration)e).BaseTypes);
					Validate(((CodeTypeDeclaration)e).Members);
				}
				else if(e is CodeTypeOfExpression)
				{
					Validate(((CodeTypeOfExpression)e).Type);
				}
				else if(e is CodeTypeReference)
				{
					Validate(((CodeTypeReference)e).ArrayElementType);
					ValidateQualifiedIdentifier
						(((CodeTypeReference)e).BaseType, true);
				}
				else if(e is CodeTypeReferenceExpression)
				{
					Validate(((CodeTypeReferenceExpression)e).Type);
				}
				else if(e is CodeVariableDeclarationStatement)
				{
					Validate
						(((CodeVariableDeclarationStatement)e).InitExpression);
					ValidateIdentifier
						(((CodeVariableDeclarationStatement)e).Name);
					Validate(((CodeVariableDeclarationStatement)e).Type);
				}
				else if(e is CodeArgumentReferenceExpression)
				{
					ValidateIdentifier
						(((CodeArgumentReferenceExpression)e).ParameterName);
				}
				else if(e is CodeArrayIndexerExpression)
				{
					Validate(((CodeArrayIndexerExpression)e).TargetObject);
					Validate(((CodeArrayIndexerExpression)e).Indices);
				}
				else if(e is CodeAssignStatement)
				{
					Validate(((CodeAssignStatement)e).Left);
					Validate(((CodeAssignStatement)e).Right);
				}
				else if(e is CodeAttributeArgument)
				{
					ValidateIdentifier(((CodeAttributeArgument)e).Name);
					Validate(((CodeAttributeArgument)e).Value);
				}
				else if(e is CodeAttributeDeclaration)
				{
					ValidateIdentifier(((CodeAttributeDeclaration)e).Name);
					Validate(((CodeAttributeDeclaration)e).Arguments);
				}
				else if(e is CodeBinaryOperatorExpression)
				{
					Validate(((CodeBinaryOperatorExpression)e).Left);
					Validate(((CodeBinaryOperatorExpression)e).Right);
				}
				else if(e is CodeCompileUnit)
				{
					Validate(((CodeCompileUnit)e).AssemblyCustomAttributes);
					Validate(((CodeCompileUnit)e).Namespaces);
				}
				else if(e is CodeDelegateCreateExpression)
				{
					Validate(((CodeDelegateCreateExpression)e).DelegateType);
					Validate(((CodeDelegateCreateExpression)e).TargetObject);
					ValidateIdentifier
						(((CodeDelegateCreateExpression)e).MethodName);
				}
				else if(e is CodeDelegateInvokeExpression)
				{
					Validate(((CodeDelegateInvokeExpression)e).TargetObject);
					Validate(((CodeDelegateInvokeExpression)e).Parameters);
				}
				else if(e is CodeDirectionExpression)
				{
					Validate(((CodeDirectionExpression)e).Expression);
				}
				else if(e is CodeEventReferenceExpression)
				{
					Validate(((CodeEventReferenceExpression)e).TargetObject);
					ValidateIdentifier
						(((CodeEventReferenceExpression)e).EventName);
				}
				else if(e is CodeExpressionStatement)
				{
					Validate(((CodeExpressionStatement)e).Expression);
				}
				else if(e is CodeFieldReferenceExpression)
				{
					Validate(((CodeFieldReferenceExpression)e).TargetObject);
					ValidateIdentifier
						(((CodeFieldReferenceExpression)e).FieldName);
				}
				else if(e is CodeIndexerExpression)
				{
					Validate(((CodeIndexerExpression)e).TargetObject);
					Validate(((CodeIndexerExpression)e).Indices);
				}
				else if(e is CodeIterationStatement)
				{
					Validate(((CodeIterationStatement)e).InitStatement);
					Validate(((CodeIterationStatement)e).TestExpression);
					Validate(((CodeIterationStatement)e).IncrementStatement);
					Validate(((CodeIterationStatement)e).Statements);
				}
				else if(e is CodeLabeledStatement)
				{
					ValidateIdentifier(((CodeLabeledStatement)e).Label);
					Validate(((CodeLabeledStatement)e).Statement);
				}
				else if(e is CodeMethodReferenceExpression)
				{
					Validate(((CodeMethodReferenceExpression)e).TargetObject);
					ValidateIdentifier
						(((CodeMethodReferenceExpression)e).MethodName);
				}
				else if(e is CodeMethodReturnStatement)
				{
					Validate(((CodeMethodReturnStatement)e).Expression);
				}
				else if(e is CodePropertyReferenceExpression)
				{
					Validate(((CodePropertyReferenceExpression)e).TargetObject);
					ValidateIdentifier
						(((CodePropertyReferenceExpression)e).PropertyName);
				}
				else if(e is CodeThrowExceptionStatement)
				{
					Validate(((CodeThrowExceptionStatement)e).ToThrow);
				}
				else if(e is CodeTryCatchFinallyStatement)
				{
					Validate(((CodeTryCatchFinallyStatement)e).TryStatements);
					Validate(((CodeTryCatchFinallyStatement)e).CatchClauses);
					Validate
						(((CodeTryCatchFinallyStatement)e).FinallyStatements);
				}
				else if(e is CodeVariableReferenceExpression)
				{
					ValidateIdentifier
						(((CodeVariableReferenceExpression)e).VariableName);
				}
			}

}; // class Validator

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
