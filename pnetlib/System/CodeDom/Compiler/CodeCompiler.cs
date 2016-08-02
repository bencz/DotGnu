/*
 * CodeCompiler.cs - Implementation of the
 *		System.CodeDom.Compiler.CodeCompiler class.
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

namespace System.CodeDom.Compiler
{

#if CONFIG_CODEDOM

using System.IO;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Text;

public abstract class CodeCompiler : CodeGenerator, ICodeCompiler
{

	// Constructor.
	protected CodeCompiler() : base() {}

	// Join an array of strings with a separator.
	protected static String JoinStringArray(String[] sa, String separator)
			{
				if(sa == null || sa.Length == 0)
				{
					return String.Empty;
				}
				return ProcessStartInfo.ArgVToArguments(sa, 0, separator);
			}

	// Get the name of the compiler.
	protected abstract String CompilerName { get; }

	// Get the file extension to use for source files.
	protected abstract String FileExtension { get; }

	// Convert compiler parameters into compiler arguments.
	protected abstract String CmdArgsFromParameters
				(CompilerParameters options);

	// Compile a CodeDom compile unit.
	protected virtual CompilerResults FromDom
				(CompilerParameters options, CodeCompileUnit e)
			{
				CodeCompileUnit[] list = new CodeCompileUnit [1];
				list[0] = e;
				return FromDomBatch(options, list);
			}

	// Compile an array of CodeDom compile units.
	protected virtual CompilerResults FromDomBatch
				(CompilerParameters options, CodeCompileUnit[] ea)
			{
				// Write all of the CodeDom units to temporary files.
				String[] tempFiles = new String [ea.Length];
				int src;
				Stream stream;
				StreamWriter writer;
				for(src = 0; src < ea.Length; ++src)
				{
					foreach(String assembly in ea[src].ReferencedAssemblies)
					{
						if(!options.ReferencedAssemblies.Contains(assembly))
						{
							options.ReferencedAssemblies.Add(assembly);
						}
					}
					tempFiles[src] = options.TempFiles.AddExtension
							(src + FileExtension);
					stream = new FileStream(tempFiles[src], FileMode.Create,
											FileAccess.Write, FileShare.Read);
					try
					{
						writer = new StreamWriter(stream, Encoding.UTF8);
						((ICodeGenerator)this).GenerateCodeFromCompileUnit
								(ea[src], writer, Options);
						writer.Flush();
						writer.Close();
					}
					finally
					{
						stream.Close();
					}
				}
				
				// Compile the temporary files.
				return FromFileBatch(options, tempFiles);
			}

	// Compile a file.
	protected virtual CompilerResults FromFile
				(CompilerParameters options, String fileName)
			{
				// Verify that the file can be read, throwing an
				// exception to the caller if it cannot.
				(new FileStream(fileName, FileMode.Open, FileAccess.Read))
						.Close();

				// Pass the filename to "FromFileBatch".
				String[] list = new String [1];
				list[0] = fileName;
				return FromFileBatch(options, list);
			}

	// Compile an array of files.
	protected virtual CompilerResults FromFileBatch
				(CompilerParameters options, String[] fileNames)
			{
				// Add an output filename to the options if necessary.
				if(options.OutputAssembly == null ||
				   options.OutputAssembly.Length == 0)
				{
					if(options.GenerateExecutable)
					{
						options.OutputAssembly = options.TempFiles.AddExtension
							("exe", !(options.GenerateInMemory));
					}
					else
					{
						options.OutputAssembly = options.TempFiles.AddExtension
							("dll", !(options.GenerateInMemory));
					}
				}

				// Build the full command-line to pass to the compiler.
				String args = CmdArgsFromParameters(options);
				args = args + " " + JoinStringArray(fileNames, " ");

				// If the argument array is too long, then write the
				// command-line to a temporary file and use "@file".
				if(args.Length > 8192) // string length > 8k
				{
					args = GetResponseFileCmdArgs(options, args);
				}

				// Create a compiler results block.
				CompilerResults results;
				results = new CompilerResults(options.TempFiles);

				// Build the process start info block.
				ProcessStartInfo startInfo;
				startInfo = new ProcessStartInfo(CompilerName, args);
				startInfo.RedirectStandardError = true;
				startInfo.UseShellExecute = false;

				// Create and run the process.
				Process process = Process.Start(startInfo);

				// Read the stderr stream until EOF.
				String line;
				while((line = process.StandardError.ReadLine()) != null)
				{
					results.Output.Add(line);
					ProcessCompilerOutputLine(results, line);
				}

				// Wait for the process to exit and record the return code.
				process.WaitForExit();
				results.NativeCompilerReturnValue = process.ExitCode;
				process.Close();

				// Load the assembly into memory if necessary.
				if(!(results.Errors.HasErrors) && options.GenerateInMemory)
				{
					FileStream stream;
					stream = new FileStream(options.OutputAssembly,
											FileMode.Open, FileAccess.Read,
											FileShare.Read);
					try
					{
						// We must use the memory-based load method,
						// because "OutputAssembly" may be deleted
						// before we are done with the assembly.
						byte[] buf = new byte [(int)(stream.Length)];
						stream.Read(buf, 0, buf.Length);
						results.CompiledAssembly = Assembly.Load(buf);
					}
					finally
					{
						stream.Close();
					}
				}
				else
				{
					results.PathToAssembly = options.OutputAssembly;
				}

				// Return the final results to the caller.
				return results;
			}

	// Compile a source string.
	protected virtual CompilerResults FromSource
				(CompilerParameters options, String source)
			{
				String[] list = new String [1];
				list[0] = source;
				return FromSourceBatch(options, list);
			}

	// Compile an array of source strings.
	protected virtual CompilerResults FromSourceBatch
				(CompilerParameters options, String[] sources)
			{
				// Write all of the sources to temporary files.
				String[] tempFiles = new String [sources.Length];
				int src;
				Stream stream;
				StreamWriter writer;
				for(src = 0; src < sources.Length; ++src)
				{
					tempFiles[src] = options.TempFiles.AddExtension
							(src + "." + FileExtension);
					stream = new FileStream(tempFiles[src], FileMode.Create,
											FileAccess.Write, FileShare.Read);
					try
					{
						writer = new StreamWriter(stream, Encoding.UTF8);
						writer.Write(sources[src]);
						writer.Flush();
						writer.Close();
					}
					finally
					{
						stream.Close();
					}
				}
				
				// Compile the temporary files.
				return FromFileBatch(options, tempFiles);
			}

	// Get the response file command arguments.
	protected virtual String GetResponseFileCmdArgs
				(CompilerParameters options, String cmdArgs)
			{
				// Get a temporary file to use for the response file.
				String responseFile = options.TempFiles.AddExtension("cmdline");

				// Write the arguments to the response file.
				Stream stream = new FileStream(responseFile, FileMode.Create,
											   FileAccess.Write,
											   FileShare.Read);
				try
				{
					StreamWriter writer = new StreamWriter
							(stream, Encoding.UTF8);
					writer.Write(cmdArgs);
					writer.Flush();
					writer.Close();
				}
				finally
				{
					stream.Close();
				}

				// Build the new command-line containing the response file.
				return "@\"" + responseFile + "\"";
			}

	// Internal version of "ProcessCompilerOutputLine".
	// The line may have one of the following forms:
	//
	//		FILENAME(LINE,COLUMN): error CODE: message
	//		FILENAME(LINE,COLUMN): fatal error CODE: message
	//		FILENAME(LINE,COLUMN): warning CODE: message
	//		FILENAME:LINE: message
	//		FILENAME:LINE:COLUMN: message
	//		FILENAME:LINE: warning: message
	//		FILENAME:LINE:COLUMN: warning: message
	//
	internal static CompilerError ProcessCompilerOutputLine(String line)
			{
				CompilerError error;
				int posn, start;

				// Bail out if the line is empty.
				if(line == null || line.Length == 0)
				{
					return null;
				}

				// Create the error block.
				error = new CompilerError();

				// Parse out the filename.
				posn = 0;
				if(line.Length >= 3 && Char.IsLetter(line[0]) &&
				   line[1] == ':' && (line[2] == '/' || line[2] == '\\'))
				{
					// Filename starting with a Windows drive specification.
					posn += 3;
				}
				while(posn < line.Length && line[posn] != ':' &&
					  line[posn] != '(')
				{
					++posn;
				}
				if(posn >= line.Length)
				{
					return null;
				}
				error.FileName = line.Substring(0, posn);

				// Parse out the line and column numbers.
				if(line[posn] == '(')
				{
					// (LINE,COLUMN) format.
					++posn;
					start = posn;
					while(posn < line.Length && line[posn] != ')' &&
					      line[posn] != ',')
					{
						++posn;
					}
					error.Line = Int32.Parse
						(line.Substring(start, posn - start));
					if(posn < line.Length && line[posn] == ',')
					{
						++posn;
						start = posn;
						while(posn < line.Length && line[posn] != ')' &&
						      line[posn] != ',')
						{
							++posn;
						}
						error.Column = Int32.Parse
							(line.Substring(start, posn - start));
					}
					while(posn < line.Length && line[posn] != ')')
					{
						++posn;
					}
					if(posn < line.Length)
					{
						++posn;
					}
					if(posn < line.Length && line[posn] == ':')
					{
						++posn;
					}
				}
				else
				{
					// LINE:COLUMN format.
					++posn;
					start = posn;
					while(posn < line.Length && line[posn] != ':')
					{
						++posn;
					}
					try {
						error.Line = Int32.Parse
							(line.Substring(start, posn - start));
						if(posn < line.Length && line[posn + 1] != ' ')
						{
							++posn;
							start = posn;
							while(posn < line.Length && line[posn] != ':')
							{
								++posn;
							}
							error.Column = Int32.Parse
									(line.Substring(start, posn - start));
						}
						if(posn < line.Length)
						{
							++posn;
						}
					}
					catch {
						// compiler output could be XXXX: no such library.
						// so Int32.Parse(XXXX: no such library) would throw an exception
						error.Line = 0;
						error.Column = 0;
						error.ErrorText = line;
						return error;
					}
				}

				// Skip white space.
				while(posn < line.Length && line[posn] == ' ')
				{
					++posn;
				}

				// Parse the error type.
				bool needCode = true;
				if((line.Length - posn) >= 6 &&
				   String.CompareOrdinal("error ", 0, line, posn, 6) == 0)
				{
					posn += 6;
				}
				else if((line.Length - posn) >= 12 &&
				        String.CompareOrdinal
							("fatal error ", 0, line, posn, 12) == 0)
				{
					posn += 12;
				}
				else if((line.Length - posn) >= 8 &&
				        String.CompareOrdinal
							("warning ", 0, line, posn, 8) == 0)
				{
					error.IsWarning = true;
					posn += 8;
				}
				else if((line.Length - posn) >= 8 &&
				        String.CompareOrdinal
							("warning:", 0, line, posn, 8) == 0)
				{
					error.IsWarning = true;
					posn += 8;
					needCode = false;
				}
				else
				{
					needCode = false;
				}

				// Parse the error code.
				if(needCode)
				{
					start = posn;
					while(posn < line.Length && line[posn] != ':')
					{
						++posn;
					}
					error.ErrorNumber = line.Substring(posn, posn - start);
					if(posn < line.Length)
					{
						++posn;
					}
				}

				// Skip white space.
				while(posn < line.Length && line[posn] == ' ')
				{
					++posn;
				}

				// Extract the error text.
				error.ErrorText = line.Substring(posn);

				// Return the error block to the caller.
				return error;
			}

	// Process an output line from the compiler.
	protected abstract void ProcessCompilerOutputLine
				(CompilerResults results, String line);

	// Compile an assembly from a CodeDom compile unit.
	CompilerResults ICodeCompiler.CompileAssemblyFromDom
			(CompilerParameters options, CodeCompileUnit compilationUnit)
			{
				try
				{
					return FromDom(options, compilationUnit);
				}
				finally
				{
					options.TempFiles.Delete();
				}
			}

	// Compile an assembly from an array of CodeDom compile units.
	CompilerResults ICodeCompiler.CompileAssemblyFromDomBatch
			(CompilerParameters options, CodeCompileUnit[] compilationUnits)
			{
				try
				{
					return FromDomBatch(options, compilationUnits);
				}
				finally
				{
					options.TempFiles.Delete();
				}
			}

	// Compile an assembly from the contents of a source file.
	CompilerResults ICodeCompiler.CompileAssemblyFromFile
			(CompilerParameters options, String fileName)
			{
				try
				{
					return FromFile(options, fileName);
				}
				finally
				{
					options.TempFiles.Delete();
				}
			}

	// Compile an assembly from the contents of an array of source files.
	CompilerResults ICodeCompiler.CompileAssemblyFromFileBatch
			(CompilerParameters options, String[] fileNames)
			{
				try
				{
					return FromFileBatch(options, fileNames);
				}
				finally
				{
					options.TempFiles.Delete();
				}
			}

	// Compile an assembly from the contents of a source string.
	CompilerResults ICodeCompiler.CompileAssemblyFromSource
			(CompilerParameters options, String source)
			{
				try
				{
					return FromSource(options, source);
				}
				finally
				{
					options.TempFiles.Delete();
				}
			}

	// Compile an assembly from the contents of an array of source strings.
	CompilerResults ICodeCompiler.CompileAssemblyFromSourceBatch
			(CompilerParameters options, String[] sources)
			{
				try
				{
					return FromSourceBatch(options, sources);
				}
				finally
				{
					options.TempFiles.Delete();
				}
			}

}; // class CodeCompiler

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
