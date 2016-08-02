/*
 * GeneratorSupport.cs - Implementation of the
 *		System.CodeDom.Compiler.GeneratorSupport class.
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

[Flags]
public enum GeneratorSupport
{
	ArraysOfArrays              = 0x00000001,
	EntryPointMethod            = 0x00000002,
	GotoStatements              = 0x00000004,
	MultidimensionalArrays      = 0x00000008,
	StaticConstructors          = 0x00000010,
	TryCatchStatements          = 0x00000020,
	ReturnTypeAttributes        = 0x00000040,
	DeclareValueTypes           = 0x00000080,
	DeclareEnums                = 0x00000100,
	DeclareDelegates            = 0x00000200,
	DeclareInterfaces           = 0x00000400,
	DeclareEvents               = 0x00000800,
	AssemblyAttributes          = 0x00001000,
	ParameterAttributes         = 0x00002000,
	ReferenceParameters         = 0x00004000,
	ChainedConstructorArguments = 0x00008000,
	NestedTypes                 = 0x00010000,
	MultipleInterfaceMembers    = 0x00020000,
	PublicStaticMembers         = 0x00040000,
	ComplexExpressions          = 0x00080000,
	Win32Resources              = 0x00100000

}; // enum GeneratorSupport

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
