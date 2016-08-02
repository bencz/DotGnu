/*
 * CSharpCodeProvider.cs - Implementation of the
 *		Microsoft.CSharp.CSharpCodeProvider class.
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

// This class should probably be in "System.CodeDom.Compiler",
// but Microsoft put it in "Microsoft.CSharp".  We put it there
// also for compatibility sake.

namespace Microsoft.CSharp
{

#if CONFIG_CODEDOM

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.ComponentModel;

public class CSharpCodeProvider : CodeDomProvider
{
	// Internal state.
	private CSharpCodeCompiler csharpCodeCompiler;

	// Constructor.
	public CSharpCodeProvider()
			{
				csharpCodeCompiler = new CSharpCodeCompiler();
			}

	// Get the file extension that is used by this provider.
	public override String FileExtension
			{
				get
				{
					return "cs";
				}
			}

	// Create a code compiler for this language.
	public override ICodeCompiler CreateCompiler()
			{
				return csharpCodeCompiler;
			}

	// Create a code generator for this language.
	public override ICodeGenerator CreateGenerator()
			{
				return csharpCodeCompiler;
			}

#if CONFIG_COMPONENT_MODEL

	// Get a type converter.
	public override TypeConverter GetConverter(Type type)
			{
				return base.GetConverter(type);
			}

#endif

}; // class CSharpCodeProvider

#endif // CONFIG_CODEDOM

}; // namespace Microsoft.CSharp
