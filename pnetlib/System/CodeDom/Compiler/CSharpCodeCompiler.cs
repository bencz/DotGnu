/*
 * CSharpCodeCompiler.cs - Implementation of the
 *		System.CodeDom.Compiler.CSharpCodeCompiler class.
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
using System.Text;
using System.Diagnostics;

internal class CSharpCodeCompiler : CodeCompiler
{

	// List of reserved words in C#.
	private static readonly String[] reservedWords = {
				"abstract", "__arglist", "as", "base", "bool",
				"break", "__builtin", "byte", "case", "catch",
				"char", "checked", "class", "const", "continue",
				"decimal", "default", "delegate", "do", "double",
				"else", "enum", "event", "explicit", "extern",
				"false", "finally", "fixed", "float", "for",
				"foreach", "goto", "if", "implicit", "in", "int",
				"interface", "internal", "is", "lock", "long",
				"__long_double", "__makeref", "__module", "namespace",
				"new", "null", "object", "operator", "out", "override",
				"params", "private", "protected", "public", "readonly",
				"ref", "__reftype", "__refvalue", "return", "sbyte",
				"sealed", "short", "sizeof", "stackalloc", "static",
				"string", "struct", "switch", "this", "throw", "true",
				"try", "typeof", "uint", "ulong", "unchecked", "unsafe",
				"ushort", "using", "virtual", "void", "volatile", "while"
			};

	// Internal state.
	private bool outputForInit;

	// Constructor.
	public CSharpCodeCompiler() : base()
			{
				outputForInit = false;
			}

	// Get the name of the compiler.
	protected override String CompilerName
			{
				get
				{
					// Use the Portable.NET C# compiler.
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
					return ".cs";
				}
			}

	// Add an argument to an argument array.
	private static void AddArgument(ref String[] args, String arg)
			{
				String[] newArgs = new String [args.Length + 1];
				Array.Copy(args, newArgs, args.Length);
				newArgs[args.Length] = arg;
				args = newArgs;
			}

	// Build a list of arguments from an option string.  The arguments
	// are assumed to use the "csc" syntax, which we will convert into
	// the "cscc" syntax within "CmdArgsFromParameters".
	private static String[] ArgsFromOptions(String options)
			{
				return ProcessStartInfo.ArgumentsToArgV(options);
			}

	// Determine if a string looks like a "csc" option.
	private static bool IsOption(String opt, String name)
			{
				return (String.Compare(opt, name, true,
									   CultureInfo.InvariantCulture) == 0);
			}

	// Add a list of "csc" defines to a "cscc" command-line.
	private static void AddDefines(ref String[] args, String defines)
			{
				if(defines != String.Empty)
				{
					String[] defs = defines.Split(',', ';');
					foreach(String def in defs)
					{
						AddArgument(ref args, "-D" + def);
					}
				}
			}

	// Convert compiler parameters into compiler arguments (common code).
	internal static String CmdArgsFromParameters
				(CompilerParameters options, String language)
			{
				String[] args = new String [0];
				int posn, posn2;
				AddArgument(ref args, "-x");
				AddArgument(ref args, language);
				if(options.OutputAssembly != null)
				{
					AddArgument(ref args, "-o");
					AddArgument(ref args, options.OutputAssembly);
				}
				if(options.IncludeDebugInformation)
				{
					AddArgument(ref args, "-g");
				}
				if(!(options.GenerateExecutable))
				{
					AddArgument(ref args, "-shared");
				}
				if(options.TreatWarningsAsErrors)
				{
					AddArgument(ref args, "-Werror");
				}
				if(options.WarningLevel >= 0)
				{
					AddArgument(ref args, "-Wall");
				}
				if(options.MainClass != null)
				{
					AddArgument(ref args, "-e");
					AddArgument(ref args, options.MainClass);
				}
				foreach(String _refAssem in options.ReferencedAssemblies)
				{
					// Strip ".dll" from the end of the assembly name.
					String refAssem = _refAssem;
					if(refAssem.Length > 4 &&
					   String.Compare(refAssem, refAssem.Length - 4,
					   				  ".dll", 0, 4, true,
									  CultureInfo.InvariantCulture) == 0)
					{
						refAssem = refAssem.Substring(0, refAssem.Length - 4);
					}

					// Split the assembly into its path and base name.
					posn = refAssem.LastIndexOf('/');
					posn2 = refAssem.LastIndexOf('\\');
					if(posn2 > posn)
					{
						posn = posn2;
					}
					if(posn != -1)
					{
						// Add "-L" and "-l" options to the command-line.
						AddArgument(ref args,
							"-L" + refAssem.Substring(0, posn));
						AddArgument(ref args,
							"-l" + refAssem.Substring(posn + 1));
					}
					else
					{
						// Add just a "-l" option to the command-line.
						AddArgument(ref args, "-l" + refAssem);
					}
				}
				if(options.Win32Resource != null)
				{
					AddArgument(ref args,
						"-fresources=" + options.Win32Resource);
				}
				if(options.CompilerOptions != null)
				{
					String[] cscArgs = ArgsFromOptions(options.CompilerOptions);
					foreach(String opt in cscArgs)
					{
						if(IsOption(opt, "/optimize") ||
						   IsOption(opt, "/optimize+") ||
						   IsOption(opt, "/o") ||
						   IsOption(opt, "/o+"))
						{
							AddArgument(ref args, "-O2");
						}
						else if(IsOption(opt, "/checked") ||
						        IsOption(opt, "/checked+"))
						{
							AddArgument(ref args, "-fchecked");
						}
						else if(IsOption(opt, "/checked-"))
						{
							AddArgument(ref args, "-funchecked");
						}
						else if(IsOption(opt, "/unsafe") ||
						        IsOption(opt, "/unsafe+"))
						{
							AddArgument(ref args, "-funsafe");
						}
						else if(IsOption(opt, "/nostdlib") ||
						        IsOption(opt, "/nostdlib+"))
						{
							AddArgument(ref args, "-fnostdlib");
						}
						else if(String.Compare(opt, 0, "/define:", 0, 8, true,
											   CultureInfo.InvariantCulture)
									== 0)
						{
							AddDefines(ref args, opt.Substring(8));
						}
						else if(String.Compare(opt, 0, "/d:", 0, 3, true,
											   CultureInfo.InvariantCulture)
									== 0)
						{
							AddDefines(ref args, opt.Substring(3));
						}
					}
				}

				return JoinStringArray(args, " ");
			}

	// Convert compiler parameters into compiler arguments.
	protected override String CmdArgsFromParameters
				(CompilerParameters options)
			{
				return CmdArgsFromParameters(options, "csharp");
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
					return "null";
				}
			}

	// Create an escaped identifier if "value" is a language keyword.
	protected override String CreateEscapedIdentifier(String value)
			{
				if(Array.IndexOf(reservedWords, value) != -1)
				{
					return "@" + value;
				}
				else
				{
					return value;
				}
			}

	// Create a valid identifier if "value" is a language keyword.
	protected override String CreateValidIdentifier(String value)
			{
				if(Array.IndexOf(reservedWords, value) != -1)
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
				switch(type)
				{
					case "System.Void":		type = "void"; break;
					case "System.Boolean":	type = "bool"; break;
					case "System.Char":		type = "char"; break;
					case "System.Byte":		type = "byte"; break;
					case "System.SByte":	type = "sbyte"; break;
					case "System.Int16":	type = "short"; break;
					case "System.UInt16":	type = "ushort"; break;
					case "System.Int32":	type = "int"; break;
					case "System.UInt32":	type = "uint"; break;
					case "System.Int64":	type = "long"; break;
					case "System.UInt64":	type = "ulong"; break;
					case "System.Single":	type = "float"; break;
					case "System.Double":	type = "double"; break;
					case "System.Decimal":	type = "decimal"; break;
					case "System.String":	type = "string"; break;
					case "System.Object":	type = "object"; break;
					default:				break;
				}
				return type;
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
				Output.Write("new ");
				if(e.Initializers.Count == 0)
				{
					Output.Write(NormalizeTypeName(e.CreateType.BaseType));
					Output.Write("[");
					if(e.SizeExpression != null)
					{
						GenerateExpression(e.SizeExpression);
					}
					else
					{
						Output.Write(e.Size);
					}
					Output.Write("]");
				}
				else
				{
					OutputType(e.CreateType);
					if(e.CreateType.ArrayRank == 0)
					{
						Output.Write("[]");
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
				Output.Write("[");
				OutputExpressionList(e.Indices);
				Output.Write("]");
			}
	protected override void GenerateBaseReferenceExpression
				(CodeBaseReferenceExpression e)
			{
				Output.Write("base");
			}
	protected override void GenerateCastExpression
				(CodeCastExpression e)
			{
				// Heavily bracket the cast to prevent the possibility
				// of ambiguity issues within the compiler.  See the
				// Portable.NET "cs_grammar.y" file for a description of
				// the possible conflicts that may arise without brackets.
				Output.Write("((");
				OutputType(e.TargetType);
				Output.Write(")(");
				GenerateExpression(e.Expression);
				Output.Write("))");
			}
	protected override void GenerateDelegateCreateExpression
				(CodeDelegateCreateExpression e)
			{
				Output.Write("new ");
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
				Output.Write("[");
				OutputExpressionList(e.Indices);
				Output.Write("]");
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
				Output.Write("new ");
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
				Output.Write("this");
			}
	protected override void GenerateVariableReferenceExpression
				(CodeVariableReferenceExpression e)
			{
				OutputIdentifier(e.VariableName);
			}

	// Start a new indented block.
	private void StartBlock()
			{
				if(Options.BracingStyle == "C")
				{
					Output.WriteLine();
					Output.WriteLine("{");
				}
				else
				{
					Output.WriteLine(" {");
				}
				Indent += 1;
			}

	// End an indented block.
	private void EndBlock()
			{
				Indent -= 1;
				Output.WriteLine("}");
			}

	// Generate various statement categories.
	protected override void GenerateAssignStatement
				(CodeAssignStatement e)
			{
				GenerateExpression(e.Left);
				Output.Write(" = ");
				GenerateExpression(e.Right);
				if(!outputForInit)
				{
					Output.WriteLine(";");
				}
			}
	protected override void GenerateAttachEventStatement
				(CodeAttachEventStatement e)
			{
				GenerateExpression(e.Event);
				Output.Write(" += ");
				GenerateExpression(e.Listener);
				Output.WriteLine(";");
			}
	protected override void GenerateConditionStatement
				(CodeConditionStatement e)
			{
				Output.Write("if (");
				GenerateExpression(e.Condition);
				Output.Write(")");
				StartBlock();
				GenerateStatements(e.TrueStatements);
				EndBlock();
				CodeStatementCollection stmts = e.FalseStatements;
				if(stmts.Count > 0 || Options.ElseOnClosing)
				{
					Output.Write("else");
					StartBlock();
					GenerateStatements(stmts);
					EndBlock();
				}
			}
	protected override void GenerateExpressionStatement
				(CodeExpressionStatement e)
			{
				GenerateExpression(e.Expression);
				if(!outputForInit)
				{
					Output.WriteLine(";");
				}
			}
	protected override void GenerateGotoStatement
				(CodeGotoStatement e)
			{
				Output.Write("goto ");
				Output.Write(e.Label);
				Output.WriteLine(";");
			}
	protected override void GenerateIterationStatement
				(CodeIterationStatement e)
			{
				if(e.InitStatement == null &&
				   e.TestExpression != null &&
				   e.IncrementStatement == null)
				{
					// Special case - output a "while" statement.
					Output.Write("while (");
					GenerateExpression(e.TestExpression);
					Output.Write(")");
					StartBlock();
					GenerateStatements(e.Statements);
					EndBlock();
				}
				else
				{
					// Output a "for" statement.
					Output.Write("for (");
					outputForInit = true;
					if(e.InitStatement != null)
					{
						GenerateStatement(e.InitStatement);
					}
					Output.Write("; ");
					if(e.TestExpression != null)
					{
						GenerateExpression(e.TestExpression);
					}
					Output.Write("; ");
					if(e.IncrementStatement != null)
					{
						GenerateStatement(e.IncrementStatement);
					}
					outputForInit = false;
					Output.Write(")");
					StartBlock();
					GenerateStatements(e.Statements);
					EndBlock();
				}
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
				if(e.Expression != null)
				{
					Output.Write("return ");
					GenerateExpression(e.Expression);
					Output.WriteLine(";");
				}
				else
				{
					Output.WriteLine("return;");
				}
			}
	protected override void GenerateRemoveEventStatement
				(CodeRemoveEventStatement e)
			{
				GenerateExpression(e.Event);
				Output.Write(" -= ");
				GenerateExpression(e.Listener);
				Output.WriteLine(";");
			}
	protected override void GenerateThrowExceptionStatement
				(CodeThrowExceptionStatement e)
			{
				if(e.ToThrow != null)
				{
					Output.Write("throw ");
					GenerateExpression(e.ToThrow);
					Output.WriteLine(";");
				}
				else
				{
					Output.WriteLine("throw;");
				}
			}
	protected override void GenerateTryCatchFinallyStatement
				(CodeTryCatchFinallyStatement e)
			{
				Output.Write("try");
				StartBlock();
				GenerateStatements(e.TryStatements);
				EndBlock();
				CodeCatchClauseCollection clauses = e.CatchClauses;
				if(clauses.Count > 0)
				{
					foreach(CodeCatchClause clause in clauses)
					{
						if(clause.CatchExceptionType != null)
						{
							Output.Write("catch (");
							OutputType(clause.CatchExceptionType);
							if(clause.LocalName != null)
							{
								Output.Write(" ");
								OutputIdentifier(clause.LocalName);
							}
							Output.Write(")");
						}
						else
						{
							Output.Write("catch");
						}
						StartBlock();
						GenerateStatements(clause.Statements);
						EndBlock();
					}
				}
				CodeStatementCollection fin = e.FinallyStatements;
				if(fin.Count > 0)
				{
					Output.Write("finally");
					StartBlock();
					GenerateStatements(fin);
					EndBlock();
				}
			}
	protected override void GenerateVariableDeclarationStatement
				(CodeVariableDeclarationStatement e)
			{
				OutputTypeNamePair(e.Type, e.Name);
				if(e.InitExpression != null)
				{
					Output.Write(" = ");
					GenerateExpression(e.InitExpression);
				}
				if(!outputForInit)
				{
					Output.WriteLine(";");
				}
			}

	// Generate various declaration categories.
	protected override void GenerateAttributeDeclarationsStart
				(CodeAttributeDeclarationCollection attributes)
			{
				Output.Write("[");
			}
	protected override void GenerateAttributeDeclarationsEnd
				(CodeAttributeDeclarationCollection attributes)
			{
				Output.Write("]");
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
				OutputIdentifier(CurrentTypeName);
				Output.Write("(");
				OutputParameters(e.Parameters);
				Output.Write(")");

				// Output the ": base" or ": this" expressions.
				if(e.BaseConstructorArgs.Count > 0)
				{
					Output.WriteLine(" : ");
					Indent += 2;
					Output.Write("base(");
					OutputExpressionList(e.BaseConstructorArgs);
					Output.Write(")");
					Indent -= 2;
				}
				if(e.ChainedConstructorArgs.Count > 0)
				{
					Output.WriteLine(" : ");
					Indent += 2;
					Output.Write("base(");
					OutputExpressionList(e.ChainedConstructorArgs);
					Output.Write(")");
					Indent -= 2;
				}

				// Output the body of the constructor.
				StartBlock();
				GenerateStatements(e.Statements);
				EndBlock();
			}
	protected override void GenerateEntryPointMethod
				(CodeEntryPointMethod e, CodeTypeDeclaration c)
			{
				Output.Write("public static void Main()");
				StartBlock();
				GenerateStatements(e.Statements);
				EndBlock();
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
					Output.Write("event ");
					OutputTypeNamePair(e.Type, e.Name);
				}
				else
				{
					Output.Write("event ");
					OutputTypeNamePair
						(e.Type, e.PrivateImplementationType + "." + e.Name);
				}
				Output.WriteLine(";");
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
					Output.WriteLine(";");
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
					Output.WriteLine(",");
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
				if(e.ReturnTypeCustomAttributes.Count > 0)
				{
					OutputAttributeDeclarations
						("return: ", e.ReturnTypeCustomAttributes);
				}
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
					Output.Write("new ");
				}
				if(e.ReturnType != null)
				{
					OutputType(e.ReturnType);
				}
				else
				{
					Output.Write("void");
				}
				Output.Write(" ");
				if(e.PrivateImplementationType != null && !IsCurrentInterface)
				{
					Output.Write(e.PrivateImplementationType.BaseType);
					Output.Write(".");
				}
				OutputIdentifier(e.Name);
				Output.Write("(");
				OutputParameters(e.Parameters);
				Output.Write(")");

				// Output the body of the method.
				if(IsCurrentInterface ||
				   (e.Attributes & MemberAttributes.ScopeMask) ==
				   		MemberAttributes.Abstract)
				{
					Output.WriteLine(";");
				}
				else
				{
					StartBlock();
					GenerateStatements(e.Statements);
					EndBlock();
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
					Output.Write("new ");
				}
				OutputType(e.Type);
				Output.Write(" ");
				if(e.PrivateImplementationType != null && !IsCurrentInterface)
				{
					Output.Write(e.PrivateImplementationType.BaseType);
					Output.Write(".");
				}
				if(e.Parameters.Count == 0)
				{
					OutputIdentifier(e.Name);
				}
				else
				{
					Output.Write("this[");
					OutputParameters(e.Parameters);
					Output.Write("]");
				}

				// Output the body of the property.
				StartBlock();
				if(e.HasGet)
				{
					if(IsCurrentInterface ||
					   (e.Attributes & MemberAttributes.ScopeMask)
							== MemberAttributes.Abstract)
					{
						Output.WriteLine("get;");
					}
					else
					{
						Output.Write("get");
						StartBlock();
						GenerateStatements(e.GetStatements);
						EndBlock();
					}
				}
				if(e.HasSet)
				{
					if(IsCurrentInterface ||
					   (e.Attributes & MemberAttributes.ScopeMask)
							== MemberAttributes.Abstract)
					{
						Output.WriteLine("set;");
					}
					else
					{
						Output.Write("set");
						StartBlock();
						GenerateStatements(e.SetStatements);
						EndBlock();
					}
				}
				EndBlock();
			}
	protected override void GenerateNamespaceStart(CodeNamespace e)
			{
				String name = e.Name;
				if(name != null && name.Length != 0)
				{
					Output.Write("namespace ");
					OutputIdentifier(name);
					StartBlock();
				}
			}
	protected override void GenerateNamespaceEnd(CodeNamespace e)
			{
				String name = e.Name;
				if(name != null && name.Length != 0)
				{
					EndBlock();
				}
			}
	protected override void GenerateNamespaceImport(CodeNamespaceImport e)
			{
				Output.Write("using ");
				OutputIdentifier(e.Namespace);
				Output.WriteLine(";");
			}
	protected override void GenerateSnippetMember
				(CodeSnippetTypeMember e)
			{
				Output.Write(e.Text);
			}
	protected override void GenerateTypeConstructor
				(CodeTypeConstructor e)
			{
				Output.Write("static ");
				OutputIdentifier(CurrentTypeName);
				Output.Write("()");
				StartBlock();
				GenerateStatements(e.Statements);
				EndBlock();
			}
	protected override void GenerateTypeStart(CodeTypeDeclaration e)
			{
				OutputAttributeDeclarations(e.CustomAttributes);
				if(!IsCurrentDelegate)
				{
					OutputTypeAttributes
						(e.TypeAttributes, IsCurrentStruct, IsCurrentEnum);
					OutputIdentifier(e.Name);
					String sep = " : ";
					foreach(CodeTypeReference type in e.BaseTypes)
					{
						Output.Write(sep);
						OutputType(type);
						sep = ",";
					}
					StartBlock();
				}
				else
				{
					switch(e.TypeAttributes & TypeAttributes.VisibilityMask)
					{
						case TypeAttributes.NestedPrivate:
							Output.Write("private "); break;
						case TypeAttributes.Public:
						case TypeAttributes.NestedPublic:
							Output.Write("public "); break;
					}
					Output.Write("delegate ");
					CodeTypeDelegate d = (CodeTypeDelegate)e;
					if(d.ReturnType != null)
					{
						OutputType(d.ReturnType);
					}
					else
					{
						Output.Write("void");
					}
					Output.Write(" ");
					OutputIdentifier(d.Name);
					Output.Write("(");
					OutputParameters(d.Parameters);
					Output.WriteLine(");");
				}
			}
	protected override void GenerateTypeEnd(CodeTypeDeclaration e)
			{
				if(!IsCurrentDelegate)
				{
					EndBlock();
				}
			}

	// Generate various misc categories.
	protected override void GenerateComment(CodeComment e)
			{
				String text = e.Text;
				String commentSeq = (e.DocComment ? "/// " : "// ");
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
					Output.Write(commentSeq);
					Output.WriteLine(text.Substring(posn, end - posn));
					posn = next + 1;
				}
			}
	protected override void GenerateLinePragmaStart(CodeLinePragma e)
			{
				Output.WriteLine();
				Output.WriteLine("#line {0} \"{1}\"",
								 e.LineNumber, e.FileName);
			}
	protected override void GenerateLinePragmaEnd(CodeLinePragma e)
			{
				Output.WriteLine();
				Output.WriteLine("#line default");
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
					baseType += "[";
					while(rank > 1)
					{
						baseType += ",";
						--rank;
					}
					baseType += "]";
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
				else if(Array.IndexOf(reservedWords, value) != -1)
				{
					return false;
				}
				else
				{
					return IsValidLanguageIndependentIdentifier(value);
				}
			}

	// Output an identifier.
	protected override void OutputIdentifier(String ident)
			{
				Output.Write(CreateEscapedIdentifier(ident));
			}

	// Output a type.
	protected override void OutputType(CodeTypeReference typeRef)
			{
				Output.Write(GetTypeOutput(typeRef));
			}

	// Hex characters for use in "QuoteSnippetString".
	private const String hexchars = "0123456789abcdef";

	// Quote a snippet string.
	protected override String QuoteSnippetString(String value)
			{
				StringBuilder builder = new StringBuilder(value.Length + 16);
				builder.Append('"');
				int length = 0;
				foreach(char ch in value)
				{
					if(ch == '\0')
					{
						builder.Append("\\0");
						length += 2;
					}
					else if(ch == '\r')
					{
						builder.Append("\\r");
						length += 2;
					}
					else if(ch == '\n')
					{
						builder.Append("\\n");
						length += 2;
					}
					else if(ch == '\t')
					{
						builder.Append("\\t");
						length += 2;
					}
					else if(ch == '\\' || ch == '"')
					{
						builder.Append('\\');
						builder.Append(ch);
						length += 2;
					}
					else if(ch < 0x0020 || ch > 0x007E)
					{
						builder.Append('\\');
						builder.Append('u');
						builder.Append(hexchars[(ch >> 12) & 0x0F]);
						builder.Append(hexchars[(ch >> 8) & 0x0F]);
						builder.Append(hexchars[(ch >> 4) & 0x0F]);
						builder.Append(hexchars[ch & 0x0F]);
						length += 6;
					}
					else
					{
						builder.Append(ch);
						++length;
					}
					if(length >= 60)
					{
						builder.Append("\" +" + Output.NewLine + "\"");
						length = 0;
					}
				}
				builder.Append('"');
				return builder.ToString();
			}

	// Determine if this code generator supports a particular
	// set of generator options.
	protected override bool Supports(GeneratorSupport supports)
			{
				return ((supports & (GeneratorSupport)0x001FFFFF) == supports);
			}

}; // class CSharpCodeCompiler

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
