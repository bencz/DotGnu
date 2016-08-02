/*
 * CodeGeneratorOptions.cs - Implementation of the
 *		System.CodeDom.Compiler.CodeGeneratorOptions class.
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

using System.Collections.Specialized;

public class CodeGeneratorOptions
{
	// Internal state.
	private ListDictionary list;

	// Constructor.
	public CodeGeneratorOptions()
			{
				list = new ListDictionary();
			}

	// Properties.
	public bool BlankLinesBetweenMembers
			{
				get
				{
					Object value = list["BlankLinesBetweenMembers"];
					if(value != null)
					{
						return (bool)value;
					}
					else
					{
						return true;
					}
				}
				set
				{
					list["BlankLinesBetweenMembers"] = value;
				}
			}
	public String BracingStyle
			{
				get
				{
					Object value = list["BracingStyle"];
					if(value != null)
					{
						return (String)value;
					}
					else
					{
						return "Block";
					}
				}
				set
				{
					list["BracingStyle"] = value;
				}
			}
	public bool ElseOnClosing
			{
				get
				{
					Object value = list["ElseOnClosing"];
					if(value != null)
					{
						return (bool)value;
					}
					else
					{
						return false;
					}
				}
				set
				{
					list["ElseOnClosing"] = value;
				}
			}
	public String IndentString
			{
				get
				{
					Object value = list["IndentString"];
					if(value != null)
					{
						return (String)value;
					}
					else
					{
						return "    ";
					}
				}
				set
				{
					list["IndentString"] = value;
				}
			}
	public Object this[String index]
			{
				get
				{
					return list[index];
				}
				set
				{
					list[index] = value;
				}
			}

}; // class CodeGeneratorOptions

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
