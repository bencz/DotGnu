/*
 * CodeGenerator.cs - Implementation of the
 *		System.CodeDom.Compiler.CodeGenerator class.
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

public abstract class CodeGenerator : ICodeGenerator
{
	// Internal state.
	private CodeTypeMember currentMember;
	private CodeTypeDeclaration currentType;
	private IndentedTextWriter writer;
	private CodeGeneratorOptions options;

	// Constructor.
	protected CodeGenerator()
			{
			}

	// Determine if an identifier is valid for language-independent use.
	public static bool IsValidLanguageIndependentIdentifier(String value)
			{
				if(value == null || value.Length == 0)
				{
					return false;
				}
				int posn = 0;
				if(Char.GetUnicodeCategory(value[0]) ==
						UnicodeCategory.DecimalDigitNumber)
				{
					return false;
				}
				for(posn = 0; posn < value.Length; ++posn)
				{
					switch(Char.GetUnicodeCategory(value[posn]))
					{
						case UnicodeCategory.UppercaseLetter:
						case UnicodeCategory.LowercaseLetter:
						case UnicodeCategory.TitlecaseLetter:
						case UnicodeCategory.ModifierLetter:
						case UnicodeCategory.OtherLetter:
						case UnicodeCategory.NonSpacingMark:
						case UnicodeCategory.SpacingCombiningMark:
						case UnicodeCategory.DecimalDigitNumber:
						case UnicodeCategory.ConnectorPunctuation: break;

						default: return false;
					}
				}
				return true;
			}

	// Get the current class member.
	protected CodeTypeMember CurrentMember
			{
				get
				{
					return currentMember;
				}
			}

	// Get the current class member's name.
	protected String CurrentMemberName
			{
				get
				{
					if(currentMember != null)
					{
						return currentMember.Name;
					}
					else
					{
						return "<% unknown %>";
					}
				}
			}

	// Get the current class type's name.
	protected String CurrentTypeName
			{
				get
				{
					if(currentType != null)
					{
						return currentType.Name;
					}
					else
					{
						return "<% unknown %>";
					}
				}
			}

	// Get or set the current indent level.
	protected int Indent
			{
				get
				{
					return writer.Indent;
				}
				set
				{
					writer.Indent = value;
				}
			}

	// Determine if the current type is a class.
	protected bool IsCurrentClass
			{
				get
				{
					return (currentType != null &&
							!(currentType is CodeTypeDelegate) &&
							currentType.IsClass);
				}
			}

	// Determine if the current type is a delegate.
	protected bool IsCurrentDelegate
			{
				get
				{
					return (currentType != null &&
							currentType is CodeTypeDelegate);
				}
			}

	// Determine if the current type is an enumeration.
	protected bool IsCurrentEnum
			{
				get
				{
					return (currentType != null &&
							!(currentType is CodeTypeDelegate) &&
							currentType.IsEnum);
				}
			}

	// Determine if the current type is an interface.
	protected bool IsCurrentInterface
			{
				get
				{
					return (currentType != null &&
							!(currentType is CodeTypeDelegate) &&
							currentType.IsInterface);
				}
			}

	// Determine if the current type is a struct.
	protected bool IsCurrentStruct
			{
				get
				{
					return (currentType != null &&
							!(currentType is CodeTypeDelegate) &&
							currentType.IsStruct);
				}
			}

	// Get the token for "null".
	protected abstract String NullToken { get; }

	// Get the current code generation options.
	protected CodeGeneratorOptions Options
			{
				get
				{
					if(options == null)
					{
						options = new CodeGeneratorOptions();
					}
					return options;
				}
			}

	// Get the output text writer.
	protected TextWriter Output
			{
				get
				{
					return writer;
				}
			}

	// Output a continuation on a new line if the language requires it.
	protected virtual void ContinueOnNewLine(String st)
			{
				writer.WriteLine(st);
			}

	// Create an escaped identifier if "value" is a language keyword.
	protected abstract String CreateEscapedIdentifier(String value);

	// Create a valid identifier if "value" is a language keyword.
	protected abstract String CreateValidIdentifier(String value);

	// Generate various expression categories.
	protected abstract void GenerateArgumentReferenceExpression
				(CodeArgumentReferenceExpression e);
	protected abstract void GenerateArrayCreateExpression
				(CodeArrayCreateExpression e);
	protected abstract void GenerateArrayIndexerExpression
				(CodeArrayIndexerExpression e);
	protected abstract void GenerateBaseReferenceExpression
				(CodeBaseReferenceExpression e);
	protected abstract void GenerateCastExpression
				(CodeCastExpression e);
	protected abstract void GenerateDelegateCreateExpression
				(CodeDelegateCreateExpression e);
	protected abstract void GenerateDelegateInvokeExpression
				(CodeDelegateInvokeExpression e);
	protected abstract void GenerateEventReferenceExpression
				(CodeEventReferenceExpression e);
	protected abstract void GenerateFieldReferenceExpression
				(CodeFieldReferenceExpression e);
	protected abstract void GenerateIndexerExpression
				(CodeIndexerExpression e);
	protected abstract void GenerateMethodInvokeExpression
				(CodeMethodInvokeExpression e);
	protected abstract void GenerateMethodReferenceExpression
				(CodeMethodReferenceExpression e);
	protected abstract void GenerateObjectCreateExpression
				(CodeObjectCreateExpression e);
	protected abstract void GeneratePropertyReferenceExpression
				(CodePropertyReferenceExpression e);
	protected abstract void GeneratePropertySetValueReferenceExpression
				(CodePropertySetValueReferenceExpression e);
	protected abstract void GenerateSnippetExpression
				(CodeSnippetExpression e);
	protected abstract void GenerateThisReferenceExpression
				(CodeThisReferenceExpression e);
	protected abstract void GenerateVariableReferenceExpression
				(CodeVariableReferenceExpression e);

	// Generate various statement categories.
	protected abstract void GenerateAssignStatement
				(CodeAssignStatement e);
	protected abstract void GenerateAttachEventStatement
				(CodeAttachEventStatement e);
	protected abstract void GenerateConditionStatement
				(CodeConditionStatement e);
	protected abstract void GenerateExpressionStatement
				(CodeExpressionStatement e);
	protected abstract void GenerateGotoStatement
				(CodeGotoStatement e);
	protected abstract void GenerateIterationStatement
				(CodeIterationStatement e);
	protected abstract void GenerateLabeledStatement
				(CodeLabeledStatement e);
	protected abstract void GenerateMethodReturnStatement
				(CodeMethodReturnStatement e);
	protected abstract void GenerateRemoveEventStatement
				(CodeRemoveEventStatement e);
	protected abstract void GenerateThrowExceptionStatement
				(CodeThrowExceptionStatement e);
	protected abstract void GenerateTryCatchFinallyStatement
				(CodeTryCatchFinallyStatement e);
	protected abstract void GenerateVariableDeclarationStatement
				(CodeVariableDeclarationStatement e);

	// Generate various declaration categories.
	protected abstract void GenerateAttributeDeclarationsStart
				(CodeAttributeDeclarationCollection attributes);
	protected abstract void GenerateAttributeDeclarationsEnd
				(CodeAttributeDeclarationCollection attributes);
	protected abstract void GenerateConstructor
				(CodeConstructor e, CodeTypeDeclaration c);
	protected abstract void GenerateEntryPointMethod
				(CodeEntryPointMethod e, CodeTypeDeclaration c);
	protected abstract void GenerateEvent
				(CodeMemberEvent e, CodeTypeDeclaration c);
	protected abstract void GenerateField(CodeMemberField e);
	protected abstract void GenerateMethod
				(CodeMemberMethod e, CodeTypeDeclaration c);
	protected abstract void GenerateProperty
				(CodeMemberProperty e, CodeTypeDeclaration c);
	protected abstract void GenerateNamespaceStart(CodeNamespace e);
	protected abstract void GenerateNamespaceEnd(CodeNamespace e);
	protected abstract void GenerateNamespaceImport(CodeNamespaceImport e);
	protected abstract void GenerateSnippetMember
				(CodeSnippetTypeMember e);
	protected abstract void GenerateTypeConstructor
				(CodeTypeConstructor e);
	protected abstract void GenerateTypeStart(CodeTypeDeclaration e);
	protected abstract void GenerateTypeEnd(CodeTypeDeclaration e);

	// Generate various misc categories.
	protected abstract void GenerateComment(CodeComment e);
	protected abstract void GenerateLinePragmaStart(CodeLinePragma e);
	protected abstract void GenerateLinePragmaEnd(CodeLinePragma e);
	protected abstract String GetTypeOutput(CodeTypeReference value);

	// Generate code for a binary operator expression.
	protected virtual void GenerateBinaryOperatorExpression
				(CodeBinaryOperatorExpression e)
			{
				Output.Write("(");
				GenerateExpression(e.Left);
				Output.Write(" ");
				OutputOperator(e.Operator);
				Output.Write(" ");
				GenerateExpression(e.Right);
				Output.Write(")");
			}

	// Generate code for comment statements.
	protected virtual void GenerateCommentStatement
				(CodeCommentStatement e)
			{
				GenerateComment(e.Comment);
			}
	protected virtual void GenerateCommentStatements
				(CodeCommentStatementCollection e)
			{
				foreach(CodeCommentStatement comment in e)
				{
					GenerateCommentStatement(comment);
				}
			}

	// Generate code for a compilation unit.
	protected virtual void GenerateCompileUnit(CodeCompileUnit e)
			{
				GenerateCompileUnitStart(e);
				GenerateNamespaces(e);
				GenerateCompileUnitEnd(e);
			}
	protected virtual void GenerateCompileUnitStart(CodeCompileUnit e)
			{
				// Nothing to do here.
			}
	protected virtual void GenerateCompileUnitEnd(CodeCompileUnit e)
			{
				// Nothing to do here.
			}

	// Generate code for constants.
	protected virtual void GenerateDecimalValue(Decimal d)
			{
				writer.Write(d.ToString(CultureInfo.InvariantCulture));
			}
	protected virtual void GenerateDoubleValue(double d)
			{
				writer.Write(d.ToString("R", CultureInfo.InvariantCulture));
			}
	protected virtual void GenerateSingleFloatValue(float s)
			{
				writer.Write(s.ToString(CultureInfo.InvariantCulture));
			}

	// Generate code for an expression.
	protected void GenerateExpression(CodeExpression e)
			{
				if(e is CodeArrayCreateExpression)
				{
					GenerateArrayCreateExpression
						((CodeArrayCreateExpression)e);
				}
				else if(e is CodeCastExpression)
				{
					GenerateCastExpression((CodeCastExpression)e);
				}
				else if(e is CodeMethodInvokeExpression)
				{
					GenerateMethodInvokeExpression
						((CodeMethodInvokeExpression)e);
				}
				else if(e is CodeObjectCreateExpression)
				{
					GenerateObjectCreateExpression
						((CodeObjectCreateExpression)e);
				}
				else if(e is CodeParameterDeclarationExpression)
				{
					GenerateParameterDeclarationExpression
						((CodeParameterDeclarationExpression)e);
				}
				else if(e is CodeTypeOfExpression)
				{
					GenerateTypeOfExpression
						((CodeTypeOfExpression)e);
				}
				else if(e is CodeTypeReferenceExpression)
				{
					GenerateTypeReferenceExpression
						((CodeTypeReferenceExpression)e);
				}
				else if(e is CodeArgumentReferenceExpression)
				{
					GenerateArgumentReferenceExpression
						((CodeArgumentReferenceExpression)e);
				}
				else if(e is CodeArrayIndexerExpression)
				{
					GenerateArrayIndexerExpression
						((CodeArrayIndexerExpression)e);
				}
				else if(e is CodeBaseReferenceExpression)
				{
					GenerateBaseReferenceExpression
						((CodeBaseReferenceExpression)e);
				}
				else if(e is CodeBinaryOperatorExpression)
				{
					GenerateBinaryOperatorExpression
						((CodeBinaryOperatorExpression)e);
				}
				else if(e is CodeDelegateCreateExpression)
				{
					GenerateDelegateCreateExpression
						((CodeDelegateCreateExpression)e);
				}
				else if(e is CodeDelegateInvokeExpression)
				{
					GenerateDelegateInvokeExpression
						((CodeDelegateInvokeExpression)e);
				}
				else if(e is CodeDirectionExpression)
				{
					GenerateDirectionExpression
						((CodeDirectionExpression)e);
				}
				else if(e is CodeEventReferenceExpression)
				{
					GenerateEventReferenceExpression
						((CodeEventReferenceExpression)e);
				}
				else if(e is CodeFieldReferenceExpression)
				{
					GenerateFieldReferenceExpression
						((CodeFieldReferenceExpression)e);
				}
				else if(e is CodeIndexerExpression)
				{
					GenerateIndexerExpression
						((CodeIndexerExpression)e);
				}
				else if(e is CodeMethodReferenceExpression)
				{
					GenerateMethodReferenceExpression
						((CodeMethodReferenceExpression)e);
				}
				else if(e is CodePrimitiveExpression)
				{
					GeneratePrimitiveExpression
						((CodePrimitiveExpression)e);
				}
				else if(e is CodePropertyReferenceExpression)
				{
					GeneratePropertyReferenceExpression
						((CodePropertyReferenceExpression)e);
				}
				else if(e is CodePropertySetValueReferenceExpression)
				{
					GeneratePropertySetValueReferenceExpression
						((CodePropertySetValueReferenceExpression)e);
				}
				else if(e is CodeSnippetExpression)
				{
					GenerateSnippetExpression
						((CodeSnippetExpression)e);
				}
				else if(e is CodeThisReferenceExpression)
				{
					GenerateThisReferenceExpression
						((CodeThisReferenceExpression)e);
				}
				else if(e is CodeVariableReferenceExpression)
				{
					GenerateVariableReferenceExpression
						((CodeVariableReferenceExpression)e);
				}
				else
				{
					throw new ArgumentException
						(S._("Arg_InvalidCodeDom"), "e");
				}
			}

	// Helper methods for line pragmas.
	private void LinePragmaStart(CodeLinePragma pragma)
			{
				if(pragma != null)
				{
					GenerateLinePragmaStart(pragma);
				}
			}
	private void LinePragmaEnd(CodeLinePragma pragma)
			{
				if(pragma != null)
				{
					GenerateLinePragmaEnd(pragma);
				}
			}

	// Generate code for namespace information.
	protected virtual void GenerateNamespace(CodeNamespace e)
			{
				GenerateCommentStatements(e.Comments);
				GenerateNamespaceStart(e);
				GenerateNamespaceImports(e);
				Output.WriteLine();
				GenerateTypes(e);
				GenerateNamespaceEnd(e);
			}
	protected void GenerateNamespaceImports(CodeNamespace e)
			{
				foreach(CodeNamespaceImport ns in e.Imports)
				{
					LinePragmaStart(ns.LinePragma);
					GenerateNamespaceImport(ns);
					LinePragmaEnd(ns.LinePragma);
				}
			}
	protected void GenerateNamespaces(CodeCompileUnit e)
			{
				foreach(CodeNamespace ns in e.Namespaces)
				{
					((ICodeGenerator)this).GenerateCodeFromNamespace
							(ns, writer.InnerWriter, Options);
				}
			}

	// Generate a parameter declaration expression.
	protected virtual void GenerateParameterDeclarationExpression
				(CodeParameterDeclarationExpression e)
			{
				if(e.CustomAttributes.Count != 0)
				{
					OutputAttributeDeclarations(e.CustomAttributes);
					Output.Write(" ");
				}
				OutputDirection(e.Direction);
				OutputTypeNamePair(e.Type, e.Name);
			}

	// Hex characters for encoding "char" constants.
	private static readonly String hexchars = "0123456789abcdef";

	// Generate code for a primitive expression.
	protected virtual void GeneratePrimitiveExpression
				(CodePrimitiveExpression e)
			{
				Object value = e.Value;
				if(value == null)
				{
					Output.Write(NullToken);
				}
				else if(value is String)
				{
					Output.Write(QuoteSnippetString((String)value));
				}
				else if(value is Char)
				{
					char ch = (char)value;
					String chvalue;
					switch(ch)
					{
						case '\'':	chvalue = "'\\''"; break;
						case '\\':	chvalue = "'\\\\'"; break;
						case '\n':	chvalue = "'\\n'"; break;
						case '\r':	chvalue = "'\\r'"; break;
						case '\t':	chvalue = "'\\t'"; break;
						case '\f':	chvalue = "'\\f'"; break;
						case '\v':	chvalue = "'\\v'"; break;
						case '\a':	chvalue = "'\\a'"; break;
						case '\b':	chvalue = "'\\b'"; break;
						default:
						{
							if(ch >= ' ' && ch <= 0x7E)
							{
								chvalue = "\"" + ch.ToString() + "\"";
							}
							else if(ch < 0x0100)
							{
								chvalue = "\"\\x" +
									hexchars[(ch >> 4) & 0x0F] +
									hexchars[ch & 0x0F] + "\"";
							}
							else
							{
								chvalue = "\"\\u" +
									hexchars[(ch >> 12) & 0x0F] +
									hexchars[(ch >> 8) & 0x0F] +
									hexchars[(ch >> 4) & 0x0F] +
									hexchars[ch & 0x0F] + "\"";
							}
						}
						break;
					}
					Output.Write(chvalue);
				}
				else if(value is SByte)
				{
					Output.Write((int)(sbyte)value);
				}
				else if(value is Byte)
				{
					Output.Write((int)(byte)value);
				}
				else if(value is Int16)
				{
					Output.Write((int)(short)value);
				}
				else if(value is UInt16)
				{
					Output.Write((int)(ushort)value);
				}
				else if(value is Int32)
				{
					Output.Write((int)value);
				}
				else if(value is UInt32)
				{
					Output.Write((uint)value);
				}
				else if(value is Int64)
				{
					Output.Write((long)value);
				}
				else if(value is UInt64)
				{
					Output.Write((ulong)value);
				}
				else if(value is Single)
				{
					GenerateSingleFloatValue((float)value);
				}
				else if(value is Double)
				{
					GenerateDoubleValue((double)value);
				}
				else if(value is Decimal)
				{
					GenerateDecimalValue((Decimal)value);
				}
				else if(value is Boolean)
				{
					if((bool)value)
					{
						Output.Write("true");
					}
					else
					{
						Output.Write("false");
					}
				}
				else
				{
					throw new ArgumentException
						(S._("Arg_InvalidCodeDom"), "e");
				}
			}

	// Generate a compile unit snippet.
	protected virtual void GenerateSnippetCompileUnit
				(CodeSnippetCompileUnit e)
			{
				LinePragmaStart(e.LinePragma);
				Output.WriteLine(e.Value);
				LinePragmaEnd(e.LinePragma);
			}

	// Generate a code statement snippet.
	protected virtual void GenerateSnippetStatement(CodeSnippetStatement e)
			{
				Output.WriteLine(e.Value);
			}

	// Generate code for a statement.
	protected void GenerateStatement(CodeStatement e)
			{
				LinePragmaStart(e.LinePragma);
				if(e is CodeAttachEventStatement)
				{
					GenerateAttachEventStatement
						((CodeAttachEventStatement)e);
				}
				else if(e is CodeCommentStatement)
				{
					GenerateCommentStatement
						((CodeCommentStatement)e);
				}
				else if(e is CodeConditionStatement)
				{
					GenerateConditionStatement
						((CodeConditionStatement)e);
				}
				else if(e is CodeRemoveEventStatement)
				{
					GenerateRemoveEventStatement
						((CodeRemoveEventStatement)e);
				}
				else if(e is CodeVariableDeclarationStatement)
				{
					GenerateVariableDeclarationStatement
						((CodeVariableDeclarationStatement)e);
				}
				else if(e is CodeAssignStatement)
				{
					GenerateAssignStatement
						((CodeAssignStatement)e);
				}
				else if(e is CodeExpressionStatement)
				{
					GenerateExpressionStatement
						((CodeExpressionStatement)e);
				}
				else if(e is CodeGotoStatement)
				{
					GenerateGotoStatement
						((CodeGotoStatement)e);
				}
				else if(e is CodeIterationStatement)
				{
					GenerateIterationStatement
						((CodeIterationStatement)e);
				}
				else if(e is CodeLabeledStatement)
				{
					GenerateLabeledStatement
						((CodeLabeledStatement)e);
				}
				else if(e is CodeMethodReturnStatement)
				{
					GenerateMethodReturnStatement
						((CodeMethodReturnStatement)e);
				}
				else if(e is CodeSnippetStatement)
				{
					GenerateSnippetStatement
						((CodeSnippetStatement)e);
				}
				else if(e is CodeThrowExceptionStatement)
				{
					GenerateThrowExceptionStatement
						((CodeThrowExceptionStatement)e);
				}
				else if(e is CodeTryCatchFinallyStatement)
				{
					GenerateTryCatchFinallyStatement
						((CodeTryCatchFinallyStatement)e);
				}
				else
				{
					throw new ArgumentException
						(S._("Arg_InvalidCodeDom"), "e");
				}
				LinePragmaEnd(e.LinePragma);
			}

	// Generate code for a statement collection.
	protected void GenerateStatements(CodeStatementCollection e)
			{
				if(e == null)
				{
					return;
				}
				foreach(CodeStatement stmt in e)
				{
					((ICodeGenerator)this).GenerateCodeFromStatement
							(stmt, writer.InnerWriter, Options);
				}
			}

	// Generate a typeof expression.
	protected virtual void GenerateTypeOfExpression
				(CodeTypeOfExpression e)
			{
				Output.Write("typeof(");
				OutputType(e.Type);
				Output.Write(")");
			}

	// Generate a type reference expression.
	protected virtual void GenerateTypeReferenceExpression
				(CodeTypeReferenceExpression e)
			{
				OutputType(e.Type);
			}

	// Generate various categories of type member.
	private void TypeMemberStart(CodeTypeMember member)
			{
				currentMember = member;
				if(options.BlankLinesBetweenMembers)
				{
					Output.WriteLine();
				}
				GenerateCommentStatements(member.Comments);
				LinePragmaStart(member.LinePragma);
			}
	private void TypeMemberEnd(CodeTypeMember member)
			{
				LinePragmaEnd(member.LinePragma);
				currentMember = null;
			}
	private void TypeFields(CodeTypeDeclaration e)
			{
				foreach(CodeTypeMember member in e.Members)
				{
					if(member is CodeMemberField)
					{
						TypeMemberStart(member);
						GenerateField((CodeMemberField)member);
						TypeMemberEnd(member);
					}
				}
			}
	private void TypeSnippets(CodeTypeDeclaration e)
			{
				foreach(CodeTypeMember member in e.Members)
				{
					if(member is CodeSnippetTypeMember)
					{
						TypeMemberStart(member);
						GenerateSnippetMember((CodeSnippetTypeMember)member);
						TypeMemberEnd(member);
					}
				}
			}
	private void TypeStaticConstructors(CodeTypeDeclaration e)
			{
				foreach(CodeTypeMember member in e.Members)
				{
					if(member is CodeTypeConstructor)
					{
						TypeMemberStart(member);
						GenerateTypeConstructor((CodeTypeConstructor)member);
						TypeMemberEnd(member);
					}
				}
			}
	private void TypeConstructors(CodeTypeDeclaration e)
			{
				foreach(CodeTypeMember member in e.Members)
				{
					if(member is CodeConstructor)
					{
						TypeMemberStart(member);
						GenerateConstructor((CodeConstructor)member, e);
						TypeMemberEnd(member);
					}
				}
			}
	private void TypeProperties(CodeTypeDeclaration e)
			{
				foreach(CodeTypeMember member in e.Members)
				{
					if(member is CodeMemberProperty)
					{
						TypeMemberStart(member);
						GenerateProperty((CodeMemberProperty)member, e);
						TypeMemberEnd(member);
					}
				}
			}
	private void TypeEvents(CodeTypeDeclaration e)
			{
				foreach(CodeTypeMember member in e.Members)
				{
					if(member is CodeMemberEvent)
					{
						TypeMemberStart(member);
						GenerateEvent((CodeMemberEvent)member, e);
						TypeMemberEnd(member);
					}
				}
			}
	private void TypeMethods(CodeTypeDeclaration e)
			{
				foreach(CodeTypeMember member in e.Members)
				{
					if(member is CodeMemberMethod &&
					   !(member is CodeConstructor) &&
					   !(member is CodeTypeConstructor))
					{
						TypeMemberStart(member);
						GenerateMethod((CodeMemberMethod)member, e);
						TypeMemberEnd(member);
					}
				}
			}
	private void TypeNestedTypes(CodeTypeDeclaration e)
			{
				foreach(CodeTypeMember member in e.Members)
				{
					if(member is CodeTypeDeclaration)
					{
						if(options.BlankLinesBetweenMembers)
						{
							Output.WriteLine();
						}
						((ICodeGenerator)this).GenerateCodeFromType
								((CodeTypeDeclaration)member,
								 writer.InnerWriter, Options);
					}
				}
				currentType = e;
			}

	// Generate a particular type declaration.
	private void GenerateType(CodeTypeDeclaration e)
			{
				currentType = e;
				GenerateCommentStatements(e.Comments);
				GenerateTypeStart(e);
				TypeFields(e);
				TypeSnippets(e);
				TypeStaticConstructors(e);
				TypeConstructors(e);
				TypeProperties(e);
				TypeEvents(e);
				TypeMethods(e);
				TypeNestedTypes(e);
				GenerateTypeEnd(e);
				currentType = null;
			}

	// Generate the types in a namespace.
	protected void GenerateTypes(CodeNamespace e)
			{
				foreach(CodeTypeDeclaration type in e.Types)
				{
					if(options.BlankLinesBetweenMembers)
					{
						Output.WriteLine();
					}
					((ICodeGenerator)this).GenerateCodeFromType
							(type, writer.InnerWriter, Options);
				}
			}

	// Determine if "value" is a valid identifier.
	protected abstract bool IsValidIdentifier(String value);

	// Output an attribute argument.
	protected virtual void OutputAttributeArgument
				(CodeAttributeArgument arg)
			{
				if(arg.Name != null)
				{
					OutputIdentifier(arg.Name);
					Output.Write("=");
				}
				((ICodeGenerator)this).GenerateCodeFromExpression
						(arg.Value, writer.InnerWriter, Options);
			}

	// Output attribute declarations.
	protected virtual void OutputAttributeDeclarations
				(CodeAttributeDeclarationCollection attributes)
			{
				OutputAttributeDeclarations(null, attributes);
			}
	internal void OutputAttributeDeclarations
				(String prefix, CodeAttributeDeclarationCollection attributes)
			{
				if(attributes.Count == 0)
				{
					return;
				}
				bool first = true;
				bool first2;
				CodeAttributeArgumentCollection args;
				GenerateAttributeDeclarationsStart(attributes);
				foreach(CodeAttributeDeclaration attr in attributes)
				{
					if(!first)
					{
						ContinueOnNewLine(", ");
					}
					else
					{
						first = false;
					}
					if(prefix != null)
					{
						Output.Write(prefix);
					}
					Output.Write(attr.Name);
					args = attr.Arguments;
					if(args.Count != 0)
					{
						Output.Write("(");
						first2 = true;
						foreach(CodeAttributeArgument arg in args)
						{
							if(!first2)
							{
								Output.Write(", ");
							}
							else
							{
								first2 = false;
							}
							OutputAttributeArgument(arg);
						}
						Output.Write(")");
					}
				}
				GenerateAttributeDeclarationsEnd(attributes);
			}

	// Generate a direction expression.
	protected virtual void GenerateDirectionExpression
				(CodeDirectionExpression e)
			{
				OutputDirection(e.Direction);
				GenerateExpression(e.Expression);
			}

	// Output a field direction value.
	protected virtual void OutputDirection(FieldDirection dir)
			{
				if(dir == FieldDirection.Out)
				{
					Output.Write("out ");
				}
				else if(dir == FieldDirection.Ref)
				{
					Output.Write("ref ");
				}
			}

	// Output an expression list.
	protected virtual void OutputExpressionList
				(CodeExpressionCollection expressions)
			{
				OutputExpressionList(expressions, false);
			}
	protected virtual void OutputExpressionList
				(CodeExpressionCollection expressions,
				 bool newlineBetweenItems)
			{
				writer.Indent += 1;
				bool first = true;
				foreach(CodeExpression expr in expressions)
				{
					if(!first)
					{
						if(newlineBetweenItems)
						{
							ContinueOnNewLine(",");
						}
						else
						{
							Output.Write(", ");
						}
					}
					else
					{
						first = false;
					}
					((ICodeGenerator)this).GenerateCodeFromExpression
						(expr, writer.InnerWriter, Options);
				}
				writer.Indent -= 1;
			}

	// Output a field scope modifier.
	protected virtual void OutputFieldScopeModifier
				(MemberAttributes attributes)
			{
				if((attributes & MemberAttributes.VTableMask) ==
						MemberAttributes.New)
				{
					Output.Write("new ");
				}
				if((attributes & MemberAttributes.ScopeMask) ==
						MemberAttributes.Static)
				{
					Output.Write("static ");
				}
				else if((attributes & MemberAttributes.ScopeMask) ==
							MemberAttributes.Const)
				{
					Output.Write("const ");
				}
			}

	// Output an identifier.
	protected virtual void OutputIdentifier(String ident)
			{
				Output.Write(ident);
			}

	// Output a member access modifier.
	protected virtual void OutputMemberAccessModifier
				(MemberAttributes attributes)
			{
				String access;
				switch(attributes & MemberAttributes.AccessMask)
				{
					case MemberAttributes.Assembly:
						access = "internal "; break;
					case MemberAttributes.FamilyAndAssembly:
						access = "/*FamANDAssem*/ internal "; break;
					case MemberAttributes.Family:
						access = "protected "; break;
					case MemberAttributes.FamilyOrAssembly:
						access = "protected internal "; break;
					case MemberAttributes.Private:
						access = "private "; break;
					case MemberAttributes.Public:
						access = "public "; break;
					default: return;
				}
				Output.Write(access);
			}

	// Output a member scope modifier.
	protected virtual void OutputMemberScopeModifier
				(MemberAttributes attributes)
			{
				if((attributes & MemberAttributes.VTableMask) ==
						MemberAttributes.New)
				{
					Output.Write("new ");
				}
				switch(attributes & MemberAttributes.ScopeMask)
				{
					case MemberAttributes.Abstract:
						Output.Write("abstract "); break;;
					case MemberAttributes.Final:
						break;;
					case MemberAttributes.Static:
						Output.Write("static "); break;;
					case MemberAttributes.Override:
						Output.Write("override "); break;;

					default:
					{
						if((attributes & MemberAttributes.AccessMask) ==
								MemberAttributes.Public ||
						   (attributes & MemberAttributes.AccessMask) ==
								MemberAttributes.Family)
						{
							Output.Write("virtual ");
						}
					}
					break;
				}
			}

	// Output a binary operator.
	protected virtual void OutputOperator(CodeBinaryOperatorType op)
			{
				String oper;
				switch(op)
				{
					case CodeBinaryOperatorType.Add:
						oper = "+"; break;
					case CodeBinaryOperatorType.Subtract:
						oper = "-"; break;
					case CodeBinaryOperatorType.Multiply:
						oper = "*"; break;
					case CodeBinaryOperatorType.Divide:
						oper = "/"; break;
					case CodeBinaryOperatorType.Modulus:
						oper = "%"; break;
					case CodeBinaryOperatorType.Assign:
						oper = "="; break;
					case CodeBinaryOperatorType.IdentityInequality:
						oper = "!="; break;
					case CodeBinaryOperatorType.IdentityEquality:
						oper = "=="; break;
					case CodeBinaryOperatorType.ValueEquality:
						oper = "=="; break;
					case CodeBinaryOperatorType.BitwiseOr:
						oper = "|"; break;
					case CodeBinaryOperatorType.BitwiseAnd:
						oper = "&"; break;
					case CodeBinaryOperatorType.BooleanOr:
						oper = "||"; break;
					case CodeBinaryOperatorType.BooleanAnd:
						oper = "&&"; break;
					case CodeBinaryOperatorType.LessThan:
						oper = "<"; break;
					case CodeBinaryOperatorType.LessThanOrEqual:
						oper = "<="; break;
					case CodeBinaryOperatorType.GreaterThan:
						oper = ">"; break;
					case CodeBinaryOperatorType.GreaterThanOrEqual:
						oper = ">="; break;
					default: return;
				}
				Output.Write(oper);
			}

	// Output a list of parameters.
	protected virtual void OutputParameters
				(CodeParameterDeclarationExpressionCollection parameters)
			{
				bool splitLines = (parameters.Count >= 16);
				bool first = true;
				if(splitLines)
				{
					writer.Indent += 1;
				}
				foreach(CodeParameterDeclarationExpression expr in parameters)
				{
					if(!first)
					{
						Output.Write(", ");
						if(splitLines)
						{
							ContinueOnNewLine("");
						}
					}
					else
					{
						first = false;
					}
					GenerateParameterDeclarationExpression(expr);
				}
				if(splitLines)
				{
					writer.Indent -= 1;
				}
			}

	// Output a type.
	protected abstract void OutputType(CodeTypeReference typeRef);

	// Output the attributes for a type.
	protected virtual void OutputTypeAttributes
				(TypeAttributes attributes, bool isStruct, bool isEnum)
			{
				switch(attributes & TypeAttributes.VisibilityMask)
				{
					case TypeAttributes.NestedPrivate:
						Output.Write("private "); break;
					case TypeAttributes.Public:
					case TypeAttributes.NestedPublic:
						Output.Write("public "); break;
				}
				if(isStruct)
				{
					Output.Write("struct ");
				}
				else if(isEnum)
				{
					Output.Write("enum ");
				}
				else
				{
					if((attributes & TypeAttributes.ClassSemanticsMask) ==
							TypeAttributes.Interface)
					{
						Output.Write("interface ");
					}
					else
					{
						if((attributes & TypeAttributes.Sealed) != 0)
						{
							Output.Write("sealed ");
						}
						if((attributes & TypeAttributes.Abstract) != 0)
						{
							Output.Write("abstract ");
						}
						Output.Write("class ");
					}
				}
			}

	// Output a type name pair.
	protected virtual void OutputTypeNamePair
				(CodeTypeReference typeRef, String name)
			{
				OutputType(typeRef);
				Output.Write(" ");
				OutputIdentifier(name);
			}

	// Quote a snippet string.
	protected abstract String QuoteSnippetString(String value);

	// Determine if this code generator supports a particular
	// set of generator options.
	protected abstract bool Supports(GeneratorSupport supports);

	// Validate an identifier and throw an exception if not valid.
	protected virtual void ValidateIdentifier(String value)
			{
				if(!IsValidIdentifier(value))
				{
					throw new ArgumentException(S._("Arg_InvalidIdentifier"));
				}
			}

	// Validate all identifiers in a CodeDom tree.
	public static void ValidateIdentifiers(CodeObject e)
			{
				Validator.Validate(e);
			}

	// Create an escaped identifier if "value" is a language keyword.
	String ICodeGenerator.CreateEscapedIdentifier(String value)
			{
				return CreateEscapedIdentifier(value);
			}

	// Create a valid identifier if "value" is a language keyword.
	String ICodeGenerator.CreateValidIdentifier(String value)
			{
				return CreateValidIdentifier(value);
			}

	// Set up this object for code generation.
	private bool SetupForCodeGeneration(TextWriter w, CodeGeneratorOptions o)
			{
				if(writer != null)
				{
					if(writer.InnerWriter != w)
					{
						throw new InvalidOperationException
							(S._("Invalid_CGWriterExists"));
					}
					return false;
				}
				else
				{
					if(o != null)
					{
						options = o;
					}
					else
					{
						options = new CodeGeneratorOptions();
					}
					writer = new IndentedTextWriter(w, options.IndentString);
					return true;
				}
			}

	// Finalize code generation.
	private void FinalizeCodeGeneration(bool initialized)
			{
				if(initialized)
				{
					writer = null;
					options = null;
				}
			}

	// Generate code from a CodeDom compile unit.
	void ICodeGenerator.GenerateCodeFromCompileUnit(CodeCompileUnit e,
									                TextWriter w,
									                CodeGeneratorOptions o)
			{
				bool initialized = SetupForCodeGeneration(w, o);
				try
				{
					if(e is CodeSnippetCompileUnit)
					{
						GenerateSnippetCompileUnit((CodeSnippetCompileUnit)e);
					}
					else
					{
						GenerateCompileUnit(e);
					}
				}
				finally
				{
					FinalizeCodeGeneration(initialized);
				}
			}

	// Generate code from a CodeDom expression.
	void ICodeGenerator.GenerateCodeFromExpression(CodeExpression e,
												   TextWriter w,
												   CodeGeneratorOptions o)
			{
				bool initialized = SetupForCodeGeneration(w, o);
				try
				{
					GenerateExpression(e);
				}
				finally
				{
					FinalizeCodeGeneration(initialized);
				}
			}

	// Generate code from a CodeDom namespace.
	void ICodeGenerator.GenerateCodeFromNamespace(CodeNamespace e,
								   				  TextWriter w,
								   				  CodeGeneratorOptions o)
			{
				bool initialized = SetupForCodeGeneration(w, o);
				try
				{
					GenerateNamespace(e);
				}
				finally
				{
					FinalizeCodeGeneration(initialized);
				}
			}

	// Generate code from a CodeDom statement.
	void ICodeGenerator.GenerateCodeFromStatement(CodeStatement e,
								   				  TextWriter w,
								   				  CodeGeneratorOptions o)
			{
				bool initialized = SetupForCodeGeneration(w, o);
				try
				{
					GenerateStatement(e);
				}
				finally
				{
					FinalizeCodeGeneration(initialized);
				}
			}

	// Generate code from a CodeDom type declaration.
	void ICodeGenerator.GenerateCodeFromType(CodeTypeDeclaration e,
							  				 TextWriter w,
							  				 CodeGeneratorOptions o)
			{
				bool initialized = SetupForCodeGeneration(w, o);
				try
				{
					GenerateType(e);
				}
				finally
				{
					FinalizeCodeGeneration(initialized);
				}
			}

	// Get the type indicated by a CodeDom type reference.
	String ICodeGenerator.GetTypeOutput(CodeTypeReference type)
			{
				return GetTypeOutput(type);
			}

	// Determine if "value" is a valid identifier.
	bool ICodeGenerator.IsValidIdentifier(String value)
			{
				return IsValidIdentifier(value);
			}

	// Determine if this code generator supports a particular
	// set of generator options.
	bool ICodeGenerator.Supports(GeneratorSupport supports)
			{
				return Supports(supports);
			}

	// Validate an identifier and throw an exception if not valid.
	void ICodeGenerator.ValidateIdentifier(String value)
			{
				ValidateIdentifier(value);
			}

}; // class CodeGenerator

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
