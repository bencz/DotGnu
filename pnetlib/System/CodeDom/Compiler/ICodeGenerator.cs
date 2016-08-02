/*
 * ICodeGenerator.cs - Implementation of the
 *		System.CodeDom.Compiler.ICodeGenerator class.
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

public interface ICodeGenerator
{
	// Create an escaped identifier if "value" is a language keyword.
	String CreateEscapedIdentifier(String value);

	// Create a valid identifier if "value" is a language keyword.
	String CreateValidIdentifier(String value);

	// Generate code from a CodeDom compile unit.
	void GenerateCodeFromCompileUnit(CodeCompileUnit e,
									 TextWriter w,
									 CodeGeneratorOptions o);

	// Generate code from a CodeDom expression.
	void GenerateCodeFromExpression(CodeExpression e,
									TextWriter w,
									CodeGeneratorOptions o);

	// Generate code from a CodeDom namespace.
	void GenerateCodeFromNamespace(CodeNamespace e,
								   TextWriter w,
								   CodeGeneratorOptions o);

	// Generate code from a CodeDom statement.
	void GenerateCodeFromStatement(CodeStatement e,
								   TextWriter w,
								   CodeGeneratorOptions o);

	// Generate code from a CodeDom type declaration.
	void GenerateCodeFromType(CodeTypeDeclaration e,
							  TextWriter w,
							  CodeGeneratorOptions o);

	// Get the type indicated by a CodeDom type reference.
	String GetTypeOutput(CodeTypeReference type);

	// Determine if "value" is a valid identifier.
	bool IsValidIdentifier(String value);

	// Determine if this code generator supports a particular
	// set of generator options.
	bool Supports(GeneratorSupport supports);

	// Validate an identifier and throw an exception if not valid.
	void ValidateIdentifier(String value);

}; // interface ICodeGenerator

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
