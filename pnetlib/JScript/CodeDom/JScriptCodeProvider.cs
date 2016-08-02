/*
 * JScriptCodeProvider.cs - CodeDom provider for JScript.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.CodeDom.Compiler;

#if CONFIG_CODEDOM

public class JScriptCodeProvider : CodeDomProvider
{
	// Constructor.
	public JScriptCodeProvider()
			{
				// TODO
			}

	// Create a JScript code compiler.
	public override ICodeCompiler CreateCompiler()
			{
				// TODO
				return null;
			}

	// Create a JScript code generator.
	public override ICodeGenerator CreateGenerator()
			{
				// TODO
				return null;
			}

	// Get the file extension for this language.
	public override String FileExtension
			{
				get
				{
					return "js";
				}
			}

}; // class JScriptCodeProvider

#endif // CONFIG_CODEDOM

}; // namespace Microsoft.JScript
