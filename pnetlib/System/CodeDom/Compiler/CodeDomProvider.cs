/*
 * CodeDomProvider.cs - Implementation of the
 *		System.CodeDom.Compiler.CodeDomProvider class.
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
using System.ComponentModel;

#if CONFIG_COMPONENT_MODEL
[ToolboxItem(false)]
#endif
public abstract class CodeDomProvider
#if CONFIG_COMPONENT_MODEL
	: Component
#endif
{

	// Constructor.
	protected CodeDomProvider() {}

	// Get the file extension that is used by this provider.
	public virtual String FileExtension
			{
				get
				{
					return String.Empty;
				}
			}

	// Get the language options that are supported by this provider.
	public virtual LanguageOptions LanguageOptions
			{
				get
				{
					return LanguageOptions.None;
				}
			}

	// Create a code compiler for this language.
	public abstract ICodeCompiler CreateCompiler();

	// Create a code generator for this language.
	public abstract ICodeGenerator CreateGenerator();
	public virtual ICodeGenerator CreateGenerator(String fileName)
			{
				return CreateGenerator();
			}
	public virtual ICodeGenerator CreateGenerator(TextWriter output)
			{
				return CreateGenerator();
			}

	// Create a code parser for this language.
	public virtual ICodeParser CreateParser()
			{
				return null;
			}

#if CONFIG_COMPONENT_MODEL

	// Get a type converter.
	public virtual TypeConverter GetConverter(Type type)
			{
				return TypeDescriptor.GetConverter(type);
			}

#endif

}; // class CodeDomProvider

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
