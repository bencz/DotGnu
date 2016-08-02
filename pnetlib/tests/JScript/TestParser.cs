/*
 * TestParser.cs - Tests for the "JSParser" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

using CSUnit;
using System;
using Microsoft.JScript;

public class TestParser : TestCase
{
	// Constructor.
	public TestParser(String name)
			: base(name)
			{
				// Nothing to do here.
			}

	// Set up for the tests.
	protected override void Setup()
			{
				// Nothing to do here.
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}

	// Test parser creation.
	public void TestParserCreate()
			{
				JSParser parser = JSParserTest.TestCreateParser("");
				AssertNotNull("Create (1)", parser);
				AssertNotNull("Create (2)", parser.Parse());
				parser = JSParserTest.TestCreateParser("");
				AssertNotNull("Create (3)", parser);
				AssertNotNull("Create (4)", parser.ParseEvalBody());
			}

	// Test a simple parse to see if the JNode tree is created correctly.
	public void TestParserSimple()
			{
				JSParser parser = JSParserTest.TestCreateParser("x = 3");
				Block block = parser.ParseEvalBody();
				Object jnode = JSParserTest.TestJNodeGet(block);

				// Top-level jnode should be JBlock.
				AssertEquals("Simple (1)", "JBlock",
							 JSParserTest.TestJNodeGetKind(jnode));

				// Should have a single expression statement.
				Object statements =
					JSParserTest.TestJNodeGetField(jnode, "statements");
				AssertEquals("Simple (2)", "JExprStmt",
							 JSParserTest.TestJNodeGetKind(statements));

				// The expression statement should contain an assignment.
				Object assign =
					JSParserTest.TestJNodeGetField(statements, "expr");
				AssertEquals("Simple (3)", "JAssign",
							 JSParserTest.TestJNodeGetKind(assign));

				// Left-hand side should be the identifier "x".
				Object lhs =
					JSParserTest.TestJNodeGetField(assign, "expr1");
				AssertEquals("Simple (4)", "JIdentifier",
							 JSParserTest.TestJNodeGetKind(lhs));
				AssertEquals("Simple (5)", "x",
							 JSParserTest.TestJNodeGetField(lhs, "name"));

				// Right-hand side should be the integer 3.
				Object rhs =
					JSParserTest.TestJNodeGetField(assign, "expr2");
				AssertEquals("Simple (6)", "JConstant",
							 JSParserTest.TestJNodeGetKind(rhs));
				Object value =
					JSParserTest.TestJNodeGetField(rhs, "value");
				Assert("Simple (7)", (value is Int32));
				AssertEquals("Simple (7)", 3, (int)value);
			}

	// Parse an eval statement and check for success.
	private static void Yes(String tag, String source)
			{
				// Parse the source.
				JSParser parser = JSParserTest.TestCreateParser(source);
				Block block = parser.ParseEvalBody();
				Object jnode = JSParserTest.TestJNodeGet(block);

				// Determine the location of the source end.
				int posn = 0;
				int line = 1;
				int temp;
				int endColumn = source.Length;
				while(posn < source.Length)
				{
					temp = source.IndexOf('\n', posn);
					if(temp == -1)
					{
						endColumn = source.Length - posn;
						break;
					}
					++line;
					posn = temp + 1;
				}

				// Check that the full node's context is appropriate.
				Context context = JSParserTest.TestJNodeGetContext(jnode);
				AssertEquals(tag + " [1]", 0, context.StartColumn);
				AssertEquals(tag + " [2]", 1, context.StartLine);
				AssertEquals(tag + " [3]", 0, context.StartPosition);
				AssertEquals(tag + " [4]", endColumn, context.EndColumn);
				AssertEquals(tag + " [5]", line, context.EndLine);
				AssertEquals(tag + " [6]", source.Length, context.EndPosition);
				AssertEquals(tag + " [7]", source, context.GetCode());
			}

	// Parse an eval statement and check for failure.
	private static void No(String tag, String source)
			{
				try
				{
					JSParser parser = JSParserTest.TestCreateParser(source);
					Block block = parser.ParseEvalBody();
					Fail(tag + " [1]");
				}
				catch(JScriptException e)
				{
					AssertEquals(tag + "[2]",
								 ((int)(JSError.SyntaxError)) +
								 unchecked((int)0x800A0000),
								 e.ErrorNumber);
				}
			}

	// Test various successful parses.
	public void TestParserSuccess()
			{
				Yes("Success (1)", "2 + 2");
				Yes("Success (2)", "2 +\n2");
				Yes("Success (3)", "{ var x = 3 }");
				Yes("Success (4)", ";;;;;;;;;;");
			}

	// Test various unsuccessful parses.
	public void TestParserFail()
			{
				No("Fail (1)", "+");
			}

}; // class TestParser
