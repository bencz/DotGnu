/*
 * JSParserTest.cs - Test support routines for JSParser.
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
using System.Reflection;

#if TEST

// This class is provided for the benefit of the test suite, which
// needs to access the internal details of "JSParser".  This class
// MUST NOT be used in application code as it is unique to this
// implementation and will not exist elsewhere.  It is also subject
// to change without notice.  You have been warned!

public sealed class JSParserTest
{

	// Create a parser from a string.
	public static JSParser TestCreateParser(String source)
			{
				return new JSParser(new Context(source));
			}

	// Get the JNode associated with an AST object.
	public static Object TestJNodeGet(AST ast)
			{
				return ast.jnode;
			}

	// Get the kind of a JNode.
	public static String TestJNodeGetKind(Object node)
			{
				return ((JNode)node).getKindName();
			}

	// Get a field within a JNode.
	public static Object TestJNodeGetField(Object node, String name)
			{
				if(node == null)
				{
					return null;
				}
				FieldInfo field = node.GetType().GetField(name);
				if(field == null)
				{
					return null;
				}
				return field.GetValue(node);
			}

	// Get the context associated with a JNode.
	public static Context TestJNodeGetContext(Object node)
			{
				return ((JNode)node).context;
			}

}; // class JSParserTest

#endif // TEST

}; // namespace Microsoft.JScript
