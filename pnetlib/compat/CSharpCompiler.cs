/*
 * CSharpCompiler.cs - Implement the "cscompmgd" assembly.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace Microsoft.CSharp
{

using System;
using System.Text;
using System.Collections;
using System.CodeDom.Compiler;

// Level of error that caused a message.
public enum ErrorLevel
{
	None       = 0,
	Warning    = 1,
	Error      = 2,
	FatalError = 3

} // enum ErrorLevel

// Wrapped version of a compiler error message.
public class CompilerError
{
	// Public error information.
	public ErrorLevel ErrorLevel;
	public String ErrorMessage;
	public int ErrorNumber;
	public int SourceColumn;
	public String SourceFile;
	public int SourceLine;

	// Convert the error information into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				if(SourceFile != null)
				{
					builder.AppendFormat("{0}({1},{2}): ", SourceFile,
										 SourceLine, SourceColumn);
				}
				switch(ErrorLevel)
				{
					case ErrorLevel.Warning:
					{
						builder.Append("warning ");
					}
					break;

					case ErrorLevel.Error:
					{
						builder.Append("error ");
					}
					break;

					case ErrorLevel.FatalError:
					{
						builder.Append("fatal error ");
					}
					break;

					default: break;
				}
				builder.AppendFormat("CS{0:D4}: ", ErrorNumber);
				if(ErrorMessage != null)
				{
					builder.Append(ErrorMessage);
				}
				return builder.ToString();
			}

} // class CompilerError

// Public interface to the C# compiler.
public class Compiler
{

	// Compile a number of source files.
	public static CompilerError[] Compile
				(String[] sourceTexts, String[] sourceTextNames,
				 String target, String[] imports, IDictionary options)
			{
				// Validate the parameters.
				if(sourceTexts == null)
				{
					throw new ArgumentNullException("sourceTexts");
				}
				if(sourceTextNames == null)
				{
					throw new ArgumentNullException("sourceTextNames");
				}
				if(target == null)
				{
					throw new ArgumentNullException("target");
				}
				if(sourceTexts.Length == 0)
				{
					throw new ArgumentOutOfRangeException("sourceTexts");
				}
				if(sourceTexts.Length != sourceTextNames.Length)
				{
					throw new ArgumentOutOfRangeException("sourceTextNames");
				}

#if CONFIG_CODEDOM
				// Build the compiler parameter block.
				CompilerParameters paramBlock;
				paramBlock = new CompilerParameters
					(imports, target, OptionSet(options, "debug"));
				int len = target.Length;
				if(len > 4 && target[len - 4] == '.' &&
				   (target[len - 3] == 'd' || target[len - 3] == 'D') &&
				   (target[len - 2] == 'l' || target[len - 2] == 'L') &&
				   (target[len - 1] == 'l' || target[len - 1] == 'L'))
				{
					paramBlock.GenerateExecutable = false;
				}
				else
				{
					paramBlock.GenerateExecutable = true;
				}
				if(OptionSet(options, "warnaserror"))
				{
					paramBlock.TreatWarningsAsErrors = true;
				}
				StringBuilder opts = new StringBuilder();
				if(OptionSet(options, "o"))
				{
					opts.Append(" /optimize");
				}
				if(OptionSet(options, "checked"))
				{
					opts.Append(" /checked");
				}
				String opt = (String)(options["d"]);
				if(opt != null)
				{
					opts.AppendFormat(" /define:{0}", opt);
				}
				opt = (String)(options["m"]);
				if(opt != null)
				{
					opts.AppendFormat(" /m:{0}", opt);
				}
				if(OptionSet(options, "nostdlib"))
				{
					opts.Append(" /nostdlib");
				}
				opt = (String)(options["res"]);
				if(opt != null)
				{
					opts.AppendFormat(" /resource:{0}", opt);
				}
				opt = (String)(options["target"]);
				if(opt != null)
				{
					paramBlock.GenerateExecutable = (opt != "library");
				}
				if(OptionSet(options, "unsafe"))
				{
					opts.Append(" /unsafe");
				}
				paramBlock.CompilerOptions = opts.ToString();

				// Build a new set of source texts, with the filename
				// information from "sourceTextNames" prepended.
				String[] sources = new String [sourceTexts.Length];
				int posn;
				for(posn = 0; posn < sourceTexts.Length; ++posn)
				{
					if(sourceTextNames[posn] == null)
					{
						sources[posn] = sourceTexts[posn];
					}
					else
					{
						sources[posn] = "#line 1 \"" + sourceTextNames[posn] +
										"\"" + Environment.NewLine +
										sourceTexts[posn];
					}
				}

				// Compile the source texts.
				ICodeCompiler compiler =
					(new CSharpCodeProvider()).CreateCompiler();
				CompilerResults results =
					compiler.CompileAssemblyFromSourceBatch
						(paramBlock, sources);

				// Convert the errors into the correct format and return them.
				CompilerErrorCollection errors = results.Errors;
				CompilerError[] newErrors = new CompilerError [errors.Count];
				posn = 0;
				foreach(System.CodeDom.Compiler.CompilerError error in errors)
				{
					newErrors[posn] = new CompilerError();
					newErrors[posn].ErrorLevel =
						(error.IsWarning ? ErrorLevel.Warning
										 : ErrorLevel.Error);
					newErrors[posn].ErrorMessage = error.ErrorText;
					if(error.ErrorNumber != null &&
					   error.ErrorNumber.StartsWith("CS"))
					{
						newErrors[posn].ErrorNumber =
							Int32.Parse(error.ErrorNumber.Substring(2));
					}
					newErrors[posn].SourceColumn = error.Column;
					newErrors[posn].SourceFile = error.FileName;
					newErrors[posn].SourceLine = error.Line;
					++posn;
				}
				return newErrors;
#else
				// We don't have the necessary CodeDom API's.
				throw new NotImplementedException();
#endif
			}

	// Determine if a boolean option is set.
	private static bool OptionSet(IDictionary options, String name)
			{
				Object obj = options[name];
				if(obj != null)
				{
					if(obj is Boolean)
					{
						return (bool)obj;
					}
					else if(obj is String)
					{
						String str = (String)obj;
						if(str == "true" || str == "1" || str == "+")
						{
							return true;
						}
					}
				}
				return false;
			}

} // class Compiler

} // namespace Microsoft.CSharp
