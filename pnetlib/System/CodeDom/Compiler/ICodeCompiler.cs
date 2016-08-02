/*
 * ICodeCompiler.cs - Implementation of the
 *		System.CodeDom.Compiler.ICodeCompiler class.
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

public interface ICodeCompiler
{
	// Compile an assembly from a CodeDom compile unit.
	CompilerResults CompileAssemblyFromDom
			(CompilerParameters options, CodeCompileUnit compilationUnit);

	// Compile an assembly from an array of CodeDom compile units.
	CompilerResults CompileAssemblyFromDomBatch
			(CompilerParameters options, CodeCompileUnit[] compilationUnits);

	// Compile an assembly from the contents of a source file.
	CompilerResults CompileAssemblyFromFile
			(CompilerParameters options, String fileName);

	// Compile an assembly from the contents of an array of source files.
	CompilerResults CompileAssemblyFromFileBatch
			(CompilerParameters options, String[] fileNames);

	// Compile an assembly from the contents of a source string.
	CompilerResults CompileAssemblyFromSource
			(CompilerParameters options, String source);

	// Compile an assembly from the contents of an array of source strings.
	CompilerResults CompileAssemblyFromSourceBatch
			(CompilerParameters options, String[] sources);

}; // interface ICodeCompiler

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
