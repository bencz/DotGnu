/*
 * VBCodeCompiler.cs - Implementation of the
 *		System.CodeDom.Compiler.VBCodeCompiler class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

internal class VBCodeCompiler : CodeCompiler
{

	// List of reserved words in VB.
	private static readonly String[] reservedWords = {
			"addhandler", "addressof", "alias", "and", "andalso",
			"ansi", "as", "assembly", "auto", "boolean", "byref", "byte",
			"byval", "call", "case", "catch", "cbool", "cbyte", "cchar",
			"cdate", "cdec", "cdbl", "char", "cint", "class", "clng",
			"cobj", "const", "cshort", "csng", "cstr", "ctype", "date",
			"decimal", "declare", "default", "delegate", "dim", "directcast",
			"do", "double", "each", "else", "elseif", "end", "enum",
			"erase", "error", "event", "exit", "false", "finally", "for",
			"friend", "function", "get", "gettype", "gosub", "goto",
			"handles", "if", "implements", "imports", "in", "inherits",
			"integer", "interface", "is", "let", "lib", "like", "long",
			"loop", "me", "mod", "module", "mustinherit", "mustoverride",
			"mybase", "myclass", "namespace", "new", "next", "not", "nothing",
			"notinheritable", "notoverridable", "object", "on", "option",
			"optional", "or", "orelse", "overloads", "overridable",
			"overrides", "paramarray", "preserve", "private", "property",
			"protected", "public", "raiseevent", "readonly", "redim",
			"removehandler", "resume", "return", "select", "set",
			"shadows", "shared", "short", "single", "static", "step", "stop",
			"string", "structure", "sub", "synclock", "then", "throw",
			"to", "true", "try", "typeof", "unicode", "until", "variant",
			"when", "while", "with", "withevents", "writeonly", "xor"
		};

	// Constructor.
	public VBCodeCompiler() {}

	// Get the name of the compiler.
	protected override String CompilerName
			{
				get
				{
					// Use the Portable.NET VB compiler.
					String cscc = Environment.GetEnvironmentVariable("CSCC");
					if(cscc != null)
					{
						return cscc;
					}
					else
					{
						return "cscc";
					}
				}
			}

	// Get the file extension to use for source files.
	protected override String FileExtension
			{
				get
				{
					return ".vb";
				}
			}

	// Convert compiler parameters into compiler arguments.
	protected override String CmdArgsFromParameters
				(CompilerParameters options)
			{
				return CSharpCodeCompiler.CmdArgsFromParameters(options, "vb");
			}

	// Process an output line from the compiler.
	protected override void ProcessCompilerOutputLine
				(CompilerResults results, String line)
			{
				CompilerError error = ProcessCompilerOutputLine(line);
				if(error != null)
				{
					results.Errors.Add(error);
				}
			}

	// Get the token for "null".
	protected override String NullToken
			{
				get
				{
					return "Nothing";
				}
			}

	// Determine if a string is a reserved word.
	private static bool IsReservedWord(String value)
			{
				if(value != null)
				{
					value = value.ToLower(CultureInfo.InvariantCulture);
					return (Array.IndexOf(reservedWords, value) != -1);
				}
				else
				{
					return false;
				}
			}

	// Create an escaped identifier if "value" is a language keyword.
	protected override String CreateEscapedIdentifier(String value)
			{
				if(IsReservedWord(value))
				{
					return "[" + value + "]";
				}
				else
				{
					return value;
				}
			}

	// Create a valid identifier if "value" is a language keyword.
	protected override String CreateValidIdentifier(String value)
			{
				if(IsReservedWord(value))
				{
					return "_" + value;
				}
				else
				{
					return value;
				}
			}

	// Normalize a type name to its keyword form.
	private String NormalizeTypeName(String type)
			{
				int index1, index2;
				String baseName;
				String suffixes;

				// Bail out if the type is null.
				if(type == null)
				{
					return null;
				}

				// Split the type into base and suffix parts.
				index1 = type.IndexOf('*');
				index2 = type.IndexOf('[');
				if(index1 > index2 && index2 != -1)
				{
					index1 = index2;
				}
				if(index1 != -1)
				{
					baseName = type.Substring(0, index1);
					suffixes = type.Substring(index1);
					suffixes = suffixes.Replace('[', '(');
					suffixes = suffixes.Replace(']', ')');
				}
				else
				{
					baseName = type;
					suffixes = null;
				}

				// Convert the base name.
				switch(baseName)
				{
					case "System.Boolean":	baseName = "Boolean"; break;
					case "System.Char":		baseName = "Char"; break;
					case "System.Byte":		baseName = "Byte"; break;
					case "System.Int16":	baseName = "Short"; break;
					case "System.Int32":	baseName = "Integer"; break;
					case "System.Int64":	baseName = "Long"; break;
					case "System.Single":	baseName = "Single"; break;
					case "System.Double":	baseName = "Double"; break;
					case "System.Decimal":	baseName = "Decimal"; break;
					case "System.String":	baseName = "String"; break;
					case "System.DateTime":	baseName = "Date"; break;
					case "System.Object":	baseName = "Object"; break;
					default:				break;
				}

				// Return the new type to the caller.
				return baseName + suffixes;
			}

	// Generate various expression categories.
	protected override void GenerateArgumentReferenceExpression
				(CodeArgumentReferenceExpression e)
			{
				OutputIdentifier(e.ParameterName);
			}
	protected override void GenerateArrayCreateExpression
				(CodeArrayCreateExpression e)
			{
				Output.Write("New ");
				if(e.Initializers.Count == 0)
				{
					Output.Write(NormalizeTypeName(e.CreateType.BaseType));
					Output.Write("(");
					if(e.SizeExpression != null)
					{
						GenerateExpression(e.SizeExpression);
					}
					else
					{
						Output.Write(e.Size);
					}
					Output.Write(")");
				}
				else
				{
					OutputType(e.CreateType);
					if(e.CreateType.ArrayRank == 0)
					{
						Output.Write("()");
					}
					Output.WriteLine(" {");
					Indent += 1;
					OutputExpressionList(e.Initializers, true);
					Indent -= 1;
					Output.Write("}");
				}
			}
	protected override void GenerateArrayIndexerExpression
				(CodeArrayIndexerExpression e)
			{
				GenerateExpression(e.TargetObject);
				Output.Write("(");
				OutputExpressionList(e.Indices);
				Output.Write(")");
			}
	protected override void GenerateBaseReferenceExpression
				(CodeBaseReferenceExpression e)
			{
				Output.Write("MyBase");
			}
	protected override void GenerateCastExpression
				(CodeCastExpression e)
			{
				Output.Write("CType(");
				GenerateExpression(e.Expression);
				Output.Write(", ");
				OutputType(e.TargetType);
				Output.Write(")");
			}
	protected override void GenerateDelegateCreateExpression
				(CodeDelegateCreateExpression e)
			{
				Output.Write("New ");
				OutputType(e.DelegateType);
				Output.Write("(");
				if(e.TargetObject != null)
				{
					GenerateExpression(e.TargetObject);
					Output.Write(".");
				}
				OutputIdentifier(e.MethodName);
				Output.Write(")");
			}
	protected override void GenerateDelegateInvokeExpression
				(CodeDelegateInvokeExpression e)
			{
				if(e.TargetObject != null)
				{
					GenerateExpression(e.TargetObject);
				}
				Output.Write("(");
				OutputExpressionList(e.Parameters);
				Output.Write(")");
			}
	protected override void GenerateEventReferenceExpression
				(CodeEventReferenceExpression e)
			{
				if(e.TargetObject != null)
				{
					GenerateExpression(e.TargetObject);
					Output.Write(".");
				}
				OutputIdentifier(e.EventName);
			}
	protected override void GenerateFieldReferenceExpression
				(CodeFieldReferenceExpression e)
			{
				if(e.TargetObject != null)
				{
					GenerateExpression(e.TargetObject);
					Output.Write(".");
				}
				OutputIdentifier(e.FieldName);
			}
	protected override void GenerateIndexerExpression
				(CodeIndexerExpression e)
			{
				GenerateExpression(e.TargetObject);
				Output.Write("(");
				OutputExpressionList(e.Indices);
				Output.Write(")");
			}
	protected override void GenerateMethodInvokeExpression
				(CodeMethodInvokeExpression e)
			{
				GenerateMethodReferenceExpression(e.Method);
				Output.Write("(");
				OutputExpressionList(e.Parameters);
				Output.Write(")");
			}
	protected override void GenerateMethodReferenceExpression
				(CodeMethodReferenceExpression e)
			{
				if(e.TargetObject != null)
				{
					GenerateExpression(e.TargetObject);
					Output.Write(".");
				}
				OutputIdentifier(e.MethodName);
			}
	protected override void GenerateObjectCreateExpression
				(CodeObjectCreateExpression e)
			{
				Output.Write("New ");
				OutputType(e.CreateType);
				Output.Write("(");
				OutputExpressionList(e.Parameters);
				Output.Write(")");
			}
	protected override void GeneratePropertyReferenceExpression
				(CodePropertyReferenceExpression e)
			{
				if(e.TargetObject != null)
				{
					GenerateExpression(e.TargetObject);
					Output.Write(".");
				}
				OutputIdentifier(e.PropertyName);
			}
	protected override void GeneratePropertySetValueReferenceExpression
				(CodePropertySetValueReferenceExpression e)
			{
				Output.Write("value");
			}
	protected override void GenerateSnippetExpression
				(CodeSnippetExpression e)
			{
				Output.Write(e.Value);
			}
	protected override void GenerateThisReferenceExpression
				(CodeThisReferenceExpression e)
			{
				Output.Write("Me");
			}
	protected override void GenerateVariableReferenceExpression
				(CodeVariableReferenceExpression e)
			{
				OutputIdentifier(e.VariableName);
			}

	// Generate various statement categories.
	protected override void GenerateAssignStatement
				(CodeAssignStatement e)
			{
				GenerateExpression(e.Left);
				Output.Write(" = ");
				GenerateExpression(e.Right);
				Output.WriteLine();
			}
	protected override void GenerateAttachEventStatement
				(CodeAttachEventStatement e)
			{
				Output.Write("AddHandler ");
				GenerateExpression(e.Event);
				Output.Write(", ");
				GenerateExpression(e.Listener);
				Output.WriteLine();
			}
	protected override void GenerateConditionStatement
				(CodeConditionStatement e)
			{
				Output.Write("If ");
				GenerateExpression(e.Condition);
				Output.WriteLine(" Then");
				++Indent;
				GenerateStatements(e.TrueStatements);
				--Indent;
				CodeStatementCollection stmts = e.FalseStatements;
				if(stmts.Count > 0 || Options.ElseOnClosing)
				{
					Output.WriteLine("Else");
					++Indent;
					GenerateStatements(stmts);
					--Indent;
				}
				Output.WriteLine("End If");
			}
	protected override void GenerateExpressionStatement
				(CodeExpressionStatement e)
			{
				GenerateExpression(e.Expression);
				Output.WriteLine();
			}
	protected override void GenerateGotoStatement
				(CodeGotoStatement e)
			{
				Output.Write("Goto ");
				Output.Write(e.Label);
				Output.WriteLine();
			}
	protected override void GenerateIterationStatement
				(CodeIterationStatement e)
			{
				if(e.InitStatement != null)
				{
					GenerateStatement(e.InitStatement);
				}
				Output.Write("While ");
				if(e.TestExpression != null)
				{
					GenerateExpression(e.TestExpression);
				}
				else
				{
					Output.Write("True");
				}
				Output.WriteLine();
				++Indent;
				GenerateStatements(e.Statements);
				if(e.IncrementStatement != null)
				{
					GenerateStatement(e.IncrementStatement);
				}
				--Indent;
				Output.WriteLine("End While");
			}
	protected override void GenerateLabeledStatement
				(CodeLabeledStatement e)
			{
				Indent -= 1;
				Output.Write(e.Label);
				Output.WriteLine(":");
				Indent += 1;
				GenerateStatement(e.Statement);
			}
	protected override void GenerateMethodReturnStatement
				(CodeMethodReturnStatement e)
			{
				CodeExpression expr = e.Expression;
				if(expr == null)
				{
					Output.WriteLine("Return");
				}
				else
				{
					Output.Write("Return ");
					GenerateExpression(expr);
					Output.WriteLine();
				}
			}
	protected override void GenerateRemoveEventStatement
				(CodeRemoveEventStatement e)
			{
				Output.Write("RemoveHandler ");
				GenerateExpression(e.Event);
				Output.Write(", ");
				GenerateExpression(e.Listener);
				Output.WriteLine();
			}
	protected override void GenerateThrowExceptionStatement
				(CodeThrowExceptionStatement e)
			{
				CodeExpression expr = e.ToThrow;
				if(expr == null)
				{
					Output.WriteLine("Throw");
				}
				else
				{
					Output.Write("Throw ");
					GenerateExpression(expr);
					Output.WriteLine();
				}
			}
	protected override void GenerateTryCatchFinallyStatement
				(CodeTryCatchFinallyStatement e)
			{
				Output.WriteLine("Try");
				++Indent;
				GenerateStatements(e.TryStatements);
				--Indent;
				CodeCatchClauseCollection clauses = e.CatchClauses;
				if(clauses.Count > 0)
				{
					foreach(CodeCatchClause clause in clauses)
					{
						if(clause.CatchExceptionType != null)
						{
							Output.Write("Catch ");
							OutputIdentifier(clause.LocalName);
							Output.Write(" As ");
							OutputType(clause.CatchExceptionType);
							Output.WriteLine();
						}
						else
						{
							Output.WriteLine("Catch");
						}
						++Indent;
						GenerateStatements(clause.Statements);
						--Indent;
					}
				}
				CodeStatementCollection fin = e.FinallyStatements;
				if(fin.Count > 0)
				{
					Output.WriteLine("Finally");
					++Indent;
					GenerateStatements(fin);
					--Indent;
				}
				Output.WriteLine("End Try");
			}
	protected override void GenerateVariableDeclarationStatement
				(CodeVariableDeclarationStatement e)
			{
				Output.Write("Dim ");
				OutputTypeNamePair(e.Type, e.Name);
				if(e.InitExpression != null)
				{
					Output.Write(" = ");
					GenerateExpression(e.InitExpression);
				}
				Output.WriteLine();
			}

	// Generate various declaration categories.
	protected override void GenerateAttributeDeclarationsStart
				(CodeAttributeDeclarationCollection attributes)
			{
				Output.Write("<");
			}
	protected override void GenerateAttributeDeclarationsEnd
				(CodeAttributeDeclarationCollection attributes)
			{
				Output.Write(">");
			}
	protected override void GenerateConstructor
				(CodeConstructor e, CodeTypeDeclaration c)
			{
				// Bail out if not a class or struct.
				if(!IsCurrentClass && !IsCurrentStruct)
				{
					return;
				}

				// Output the attributes and constructor signature.
				OutputAttributeDeclarations(e.CustomAttributes);
				OutputMemberAccessModifier(e.Attributes);
				Output.Write("Sub New ");
				Output.Write("(");
				OutputParameters(e.Parameters);
				Output.WriteLine(")");

				// Output the body of the constructor.
				++Indent;
				GenerateStatements(e.Statements);
				--Indent;
				Output.WriteLine("End Sub");
			}
	protected override void GenerateEntryPointMethod
				(CodeEntryPointMethod e, CodeTypeDeclaration c)
			{
				Output.WriteLine("Public Shared Sub Main()");
				++Indent;
				GenerateStatements(e.Statements);
				--Indent;
				Output.WriteLine("End Sub");
			}
	protected override void GenerateEvent
				(CodeMemberEvent e, CodeTypeDeclaration c)
			{
				// Bail out if not a class, struct, or interface.
				if(!IsCurrentClass && !IsCurrentStruct && !IsCurrentInterface)
				{
					return;
				}

				// Output the event definition.
				OutputAttributeDeclarations(e.CustomAttributes);
				if(e.PrivateImplementationType == null)
				{
					OutputMemberAccessModifier(e.Attributes);
					OutputMemberScopeModifier(e.Attributes);
					Output.Write("Event ");
					OutputTypeNamePair(e.Type, e.Name);
				}
				else
				{
					Output.Write("Event ");
					OutputTypeNamePair(e.Type, e.Name);
					Output.Write(" Implements ");
					OutputType(e.PrivateImplementationType);
				}
				Output.WriteLine();
			}
	protected override void GenerateField(CodeMemberField e)
			{
				// Bail out if not a class, struct, or enum.
				if(!IsCurrentClass && !IsCurrentStruct && !IsCurrentEnum)
				{
					return;
				}

				// Generate information about the field.
				if(!IsCurrentEnum)
				{
					OutputAttributeDeclarations(e.CustomAttributes);
					OutputMemberAccessModifier(e.Attributes);
					OutputFieldScopeModifier(e.Attributes);
					OutputTypeNamePair(e.Type, e.Name);
					if(e.InitExpression != null)
					{
						Output.Write(" = ");
						GenerateExpression(e.InitExpression);
					}
					Output.WriteLine();
				}
				else
				{
					OutputAttributeDeclarations(e.CustomAttributes);
					OutputIdentifier(e.Name);
					if(e.InitExpression != null)
					{
						Output.Write(" = ");
						GenerateExpression(e.InitExpression);
					}
					Output.WriteLine();
				}
			}
	protected override void GenerateMethod
				(CodeMemberMethod e, CodeTypeDeclaration c)
			{
				// Bail out if not a class, struct, or interface.
				if(!IsCurrentClass && !IsCurrentStruct && !IsCurrentInterface)
				{
					return;
				}

				// Output the attributes and method signature.
				OutputAttributeDeclarations(e.CustomAttributes);
				if(!IsCurrentInterface)
				{
					if(e.PrivateImplementationType == null)
					{
						OutputMemberAccessModifier(e.Attributes);
						OutputMemberScopeModifier(e.Attributes);
					}
				}
				else if((e.Attributes & MemberAttributes.VTableMask)
							== MemberAttributes.New)
				{
					Output.Write("Shadows ");
				}
				if(e.ReturnType != null)
				{
					Output.Write("Function ");
				}
				else
				{
					Output.Write("Sub ");
				}
				Output.Write(" ");
				OutputIdentifier(e.Name);
				Output.Write("(");
				OutputParameters(e.Parameters);
				Output.Write(")");
				if(e.ReturnType != null)
				{
					Output.Write(" As ");
					OutputType(e.ReturnType);
				}
				Output.WriteLine();
				if(e.PrivateImplementationType != null && !IsCurrentInterface)
				{
					Output.Write("Implements ");
					Output.Write(e.PrivateImplementationType.BaseType);
					Output.WriteLine();
				}

				// Output the body of the method.
				if(IsCurrentInterface ||
				   (e.Attributes & MemberAttributes.ScopeMask) ==
				   		MemberAttributes.Abstract)
				{
					// Nothing to do here.
				}
				else
				{
					++Indent;
					GenerateStatements(e.Statements);
					--Indent;
					if(e.ReturnType != null)
					{
						Output.WriteLine("End Function");
					}
					else
					{
						Output.WriteLine("End Sub");
					}
				}
			}
	protected override void GenerateProperty
				(CodeMemberProperty e, CodeTypeDeclaration c)
			{
				// Bail out if not a class, struct, or interface.
				if(!IsCurrentClass && !IsCurrentStruct && !IsCurrentInterface)
				{
					return;
				}

				// Output the attributes and property signature.
				OutputAttributeDeclarations(e.CustomAttributes);
				if(!IsCurrentInterface)
				{
					if(e.PrivateImplementationType == null)
					{
						OutputMemberAccessModifier(e.Attributes);
						OutputMemberScopeModifier(e.Attributes);
					}
				}
				else if((e.Attributes & MemberAttributes.VTableMask)
							== MemberAttributes.New)
				{
					Output.Write("Shadows ");
				}
				Output.Write("Property ");
				OutputIdentifier(e.Name);
				if(e.Parameters.Count != 0)
				{
					Output.Write("(");
					OutputParameters(e.Parameters);
					Output.Write(")");
				}
				Output.Write(" As ");
				OutputType(e.Type);
				if(e.PrivateImplementationType != null && !IsCurrentInterface)
				{
					Output.Write(" Implements ");
					Output.Write(e.PrivateImplementationType.BaseType);
				}
				Output.WriteLine();

				// Output the body of the property.
				++Indent;
				if(e.HasGet)
				{
					if(IsCurrentInterface ||
					   (e.Attributes & MemberAttributes.ScopeMask)
							== MemberAttributes.Abstract)
					{
						Output.WriteLine("Get");
					}
					else
					{
						Output.WriteLine("Get");
						++Indent;
						GenerateStatements(e.GetStatements);
						--Indent;
						Output.WriteLine("End Get");
					}
				}
				if(e.HasSet)
				{
					if(IsCurrentInterface ||
					   (e.Attributes & MemberAttributes.ScopeMask)
							== MemberAttributes.Abstract)
					{
						Output.WriteLine("Set");
					}
					else
					{
						Output.WriteLine("Set");
						++Indent;
						GenerateStatements(e.SetStatements);
						--Indent;
						Output.WriteLine("End Set");
					}
				}
				++Indent;
				Output.WriteLine("End Property");
			}
	protected override void GenerateNamespaceStart(CodeNamespace e)
			{
				Output.Write("Namespace ");
				OutputIdentifier(e.Name);
				Output.WriteLine();
			}
	protected override void GenerateNamespaceEnd(CodeNamespace e)
			{
				Output.WriteLine("End Namespace");
			}
	protected override void GenerateNamespaceImport(CodeNamespaceImport e)
			{
				Output.Write("Imports ");
				OutputIdentifier(e.Namespace);
				Output.WriteLine();
			}
	protected override void GenerateSnippetMember
				(CodeSnippetTypeMember e)
			{
				Output.Write(e.Text);
			}
	protected override void GenerateTypeConstructor
				(CodeTypeConstructor e)
			{
				Output.WriteLine("Shared Sub New ");
				++Indent;
				GenerateStatements(e.Statements);
				--Indent;
				Output.WriteLine("End Sub");
			}
	protected override void GenerateTypeStart(CodeTypeDeclaration e)
			{
				OutputAttributeDeclarations(e.CustomAttributes);
				if(!IsCurrentDelegate)
				{
					OutputTypeAttributes
						(e.TypeAttributes, IsCurrentStruct, IsCurrentEnum);
					OutputIdentifier(e.Name);
					Output.WriteLine();
					Indent += 2;
					String sep = " Inherits ";
					bool needeol = false;
					foreach(CodeTypeReference type in e.BaseTypes)
					{
						Output.Write(sep);
						OutputType(type);
						if(sep == " Inherits ")
						{
							Output.WriteLine();
							sep = "Implements ";
						}
						else
						{
							sep = ", ";
							needeol = true;
						}
					}
					if(needeol)
					{
						Output.WriteLine();
					}
					--Indent;
				}
				else
				{
					switch(e.TypeAttributes & TypeAttributes.VisibilityMask)
					{
						case TypeAttributes.NestedPrivate:
							Output.Write("Private "); break;
						case TypeAttributes.Public:
						case TypeAttributes.NestedPublic:
							Output.Write("Public "); break;
					}
					Output.Write("Delegate ");
					CodeTypeDelegate d = (CodeTypeDelegate)e;
					if(d.ReturnType != null)
					{
						Output.Write("Function ");
						OutputType(d.ReturnType);
					}
					else
					{
						Output.Write("Sub ");
					}
					OutputIdentifier(d.Name);
					Output.Write("(");
					OutputParameters(d.Parameters);
					Output.Write(")");
					if(d.ReturnType != null)
					{
						Output.Write(" As ");
						OutputType(d.ReturnType);
						Output.WriteLine();
						Output.WriteLine("End Function");
					}
					else
					{
						Output.WriteLine();
						Output.WriteLine("End Sub");
					}
				}
			}
	protected override void GenerateTypeEnd(CodeTypeDeclaration e)
			{
				if(IsCurrentClass)
				{
					--Indent;
					Output.WriteLine("End Class");
				}
				else if(IsCurrentStruct)
				{
					--Indent;
					Output.WriteLine("End Structure");
				}
				else if(IsCurrentInterface)
				{
					--Indent;
					Output.WriteLine("End Interface");
				}
				else if(IsCurrentEnum)
				{
					--Indent;
					Output.WriteLine("End Enum");
				}
			}

	// Generate various misc categories.
	protected override void GenerateComment(CodeComment e)
			{
				String text = e.Text;
				if(text == null)
				{
					return;
				}
				int posn = 0;
				int end, next;
				while(posn < text.Length)
				{
					end = posn;
					next = end;
					while(end < text.Length)
					{
						if(text[end] == '\r')
						{
							if((end + 1) < text.Length &&
							   text[end + 1] == '\n')
							{
								next = end + 1;
							}
							break;
						}
						else if(text[end] == '\n' ||
								text[end] == '\u2028' ||
								text[end] == '\u2029')
						{
							break;
						}
						++end;
						next = end;
					}
					Output.Write("'");
					Output.WriteLine(text.Substring(posn, end - posn));
					posn = next + 1;
				}
			}
	protected override void GenerateLinePragmaStart(CodeLinePragma e)
			{
				Output.WriteLine();
				Output.Write("#ExternalSource(\"");
				Output.Write(e.FileName);
				Output.Write(",");
				Output.Write(e.LineNumber);
				Output.WriteLine(")");
			}
	protected override void GenerateLinePragmaEnd(CodeLinePragma e)
			{
				Output.WriteLine();
				Output.WriteLine("#End ExternalSource");
			}
	protected override String GetTypeOutput(CodeTypeReference value)
			{
				String baseType;
				if(value.ArrayElementType != null)
				{
					baseType = GetTypeOutput(value.ArrayElementType);
				}
				else
				{
					baseType = value.BaseType;
				}
				baseType = NormalizeTypeName(baseType);
				int rank = value.ArrayRank;
				if(rank > 0)
				{
					baseType += "(";
					while(rank > 1)
					{
						baseType += ",";
						--rank;
					}
					baseType += ")";
				}
				return baseType;
			}

	// Determine if "value" is a valid identifier.
	protected override bool IsValidIdentifier(String value)
			{
				if(value == null || value.Length == 0)
				{
					return false;
				}
				switch(value[value.Length - 1])
				{
					case '%': case '&': case '@': case '!':
					case '#': case '$':
					{
						// Strip the type suffix character from the identifier.
						value = value.Substring(0, value.Length - 1);
						if(value.Length == 0)
						{
							return false;
						}
					}
					break;

					default: break;
				}
				if(Array.IndexOf(reservedWords, value.ToLower
									(CultureInfo.InvariantCulture)) != -1)
				{
					return false;
				}
				else
				{
					return IsValidLanguageIndependentIdentifier(value);
				}
			}

	// Generate a direction expression.
	protected override void GenerateDirectionExpression
				(CodeDirectionExpression e)
			{
				if(e.Direction == FieldDirection.Out ||
				   e.Direction == FieldDirection.Ref)
				{
					Output.Write("AddressOf ");
				}
				GenerateExpression(e.Expression);
			}

	// Output a field direction value.
	protected override void OutputDirection(FieldDirection dir)
			{
				if(dir == FieldDirection.Out)
				{
					Output.Write("ByRef ");
				}
				else if(dir == FieldDirection.Ref)
				{
					Output.Write("ByRef ");
				}
				else
				{
					Output.Write("ByVal ");
				}
			}

	// Output a field scope modifier.
	protected override void OutputFieldScopeModifier
				(MemberAttributes attributes)
			{
				if((attributes & MemberAttributes.VTableMask) ==
						MemberAttributes.New)
				{
					Output.Write("Shadows ");
				}
				if((attributes & MemberAttributes.ScopeMask) ==
						MemberAttributes.Static)
				{
					Output.Write("Shared ");
				}
				else if((attributes & MemberAttributes.ScopeMask) ==
							MemberAttributes.Const)
				{
					Output.Write("Const ");
				}
			}

	// Output a member access modifier.
	protected override void OutputMemberAccessModifier
				(MemberAttributes attributes)
			{
				String access;
				switch(attributes & MemberAttributes.AccessMask)
				{
					case MemberAttributes.Assembly:
					case MemberAttributes.FamilyAndAssembly:
						access = "Friend "; break;
					case MemberAttributes.Family:
						access = "Protected "; break;
					case MemberAttributes.FamilyOrAssembly:
						access = "Protected Friend "; break;
					case MemberAttributes.Private:
						access = "Private "; break;
					case MemberAttributes.Public:
						access = "Public "; break;
					default: return;
				}
				Output.Write(access);
			}

	// Output a member scope modifier.
	protected override void OutputMemberScopeModifier
				(MemberAttributes attributes)
			{
				if((attributes & MemberAttributes.VTableMask) ==
						MemberAttributes.New)
				{
					Output.Write("Shadows ");
				}
				switch(attributes & MemberAttributes.ScopeMask)
				{
					case MemberAttributes.Abstract:
						Output.Write("MustOverride "); break;;
					case MemberAttributes.Final:
						break;;
					case MemberAttributes.Static:
						Output.Write("Shared "); break;;
					case MemberAttributes.Override:
						Output.Write("Overrides "); break;;

					default:
					{
						if((attributes & MemberAttributes.AccessMask) ==
								MemberAttributes.Public ||
						   (attributes & MemberAttributes.AccessMask) ==
								MemberAttributes.Family)
						{
							Output.Write("Overridable ");
						}
					}
					break;
				}
			}

	// Output a binary operator.
	protected override void OutputOperator(CodeBinaryOperatorType op)
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
						oper = "Mod"; break;
					case CodeBinaryOperatorType.Assign:
						oper = "="; break;
					case CodeBinaryOperatorType.IdentityInequality:
						oper = "<>"; break;
					case CodeBinaryOperatorType.IdentityEquality:
						oper = "=="; break;
					case CodeBinaryOperatorType.ValueEquality:
						oper = "="; break;
					case CodeBinaryOperatorType.BitwiseOr:
						oper = "Or"; break;
					case CodeBinaryOperatorType.BitwiseAnd:
						oper = "And"; break;
					case CodeBinaryOperatorType.BooleanOr:
						oper = "OrElse"; break;
					case CodeBinaryOperatorType.BooleanAnd:
						oper = "AndAlso"; break;
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

	// Output a type.
	protected override void OutputType(CodeTypeReference typeRef)
			{
				Output.Write(GetTypeOutput(typeRef));
			}

	// Output the attributes for a type.
	protected override void OutputTypeAttributes
				(TypeAttributes attributes, bool isStruct, bool isEnum)
			{
				switch(attributes & TypeAttributes.VisibilityMask)
				{
					case TypeAttributes.NestedPrivate:
						Output.Write("Private "); break;
					case TypeAttributes.Public:
					case TypeAttributes.NestedPublic:
						Output.Write("Public "); break;
				}
				if(isStruct)
				{
					Output.Write("Structure ");
				}
				else if(isEnum)
				{
					Output.Write("Enum ");
				}
				else
				{
					if((attributes & TypeAttributes.ClassSemanticsMask) ==
							TypeAttributes.Interface)
					{
						Output.Write("Interface ");
					}
					else
					{
						if((attributes & TypeAttributes.Sealed) != 0)
						{
							Output.Write("NotInheritable ");
						}
						if((attributes & TypeAttributes.Abstract) != 0)
						{
							Output.Write("MustInherit ");
						}
						Output.Write("Class ");
					}
				}
			}

	// Output a type name pair.
	protected override void OutputTypeNamePair
				(CodeTypeReference typeRef, String name)
			{
				OutputIdentifier(name);
				Output.Write(" As ");
				OutputType(typeRef);
			}

	// Quote a snippet string.
	protected override String QuoteSnippetString(String value)
			{
				return "\"" + value.Replace("\"", "\"\"") + "\"";
			}

	// Determine if this code generator supports a particular
	// set of generator options.
	protected override bool Supports(GeneratorSupport supports)
			{
				return ((supports & (GeneratorSupport)0x001FFFFF) == supports);
			}

}; // class VBCodeCompiler

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
