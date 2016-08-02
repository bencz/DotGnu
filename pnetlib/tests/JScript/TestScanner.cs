/*
 * TestScanner.cs - Tests for the "JSScanner" class.
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

public class TestScanner : TestCase
{
	// Constructor.
	public TestScanner(String name)
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

	// Test scanner creation.
	public void TestScannerCreate()
			{
				JSScanner scanner;
				Context token;
				scanner = JSScannerTest.TestCreateScanner("// comment\n");
				scanner.GetNextToken();
				token = JSScannerTest.TestGetTokenContext(scanner);
				AssertEquals("Create (1)", JSToken.EndOfFile, token.GetToken());
			}

	// Test keyword recognition.
	public void TestScannerKeywords()
			{
				JSScanner scanner;
				Context token;
				scanner = JSScannerTest.TestCreateScanner
					("hello get switch assert \\assert switch0");
				scanner.GetNextToken();
				token = JSScannerTest.TestGetTokenContext(scanner);
				AssertEquals("Keywords (1)", JSToken.Identifier,
							 token.GetToken());
				AssertEquals("Keywords (2)", "hello",
							 JSScannerTest.TestGetIdentifierName(scanner));
				scanner.GetNextToken();
				AssertEquals("Keywords (3)", JSToken.Get,
							 token.GetToken());
				AssertNull("Keywords (4)",
						   JSScannerTest.TestGetIdentifierName(scanner));
				scanner.GetNextToken();
				AssertEquals("Keywords (5)", JSToken.Switch,
							 token.GetToken());
				AssertNull("Keywords (6)",
						   JSScannerTest.TestGetIdentifierName(scanner));
				scanner.GetNextToken();
				AssertEquals("Keywords (7)", JSToken.Assert,
							 token.GetToken());
				AssertNull("Keywords (8)",
						   JSScannerTest.TestGetIdentifierName(scanner));
				scanner.GetNextToken();
				AssertEquals("Keywords (9)", JSToken.Identifier,
							 token.GetToken());
				AssertEquals("Keywords (10)", "assert",
							 JSScannerTest.TestGetIdentifierName(scanner));
				scanner.GetNextToken();
				AssertEquals("Keywords (11)", JSToken.Identifier,
							 token.GetToken());
				AssertEquals("Keywords (12)", "switch0",
							 JSScannerTest.TestGetIdentifierName(scanner));
				scanner.GetNextToken();
				AssertEquals("Keywords (13)",
							 JSToken.EndOfFile, token.GetToken());
			}

	// Test comment recognition.
	public void TestScannerComments()
			{
				JSScanner scanner;
				Context token;
				scanner = JSScannerTest.TestCreateScanner
					("#!/usr/local/bin/ilrun\n" +
					 "hello// single-line comment\r" +
					 "/* multi-line 1 */ switch /* multi-line\r\n" +
					 "2 */ assert /= / /*");
				scanner.GetNextToken();
				token = JSScannerTest.TestGetTokenContext(scanner);
				AssertEquals("Comments (1)", JSToken.Identifier,
							 token.GetToken());
				AssertEquals("Comments (2)", "hello",
							 JSScannerTest.TestGetIdentifierName(scanner));
				AssertEquals("Comments (3)", 2, token.StartLine);
				AssertEquals("Comments (4)", 2, token.EndLine);
				AssertEquals("Comments (5)", 23, token.StartPosition);
				AssertEquals("Comments (6)", 28, token.EndPosition);
				AssertEquals("Comments (7)", 0, token.StartColumn);
				AssertEquals("Comments (8)", 5, token.EndColumn);
				scanner.GetNextToken();
				AssertEquals("Comments (9)", JSToken.Switch,
							 token.GetToken());
				AssertNull("Comments (10)",
						   JSScannerTest.TestGetIdentifierName(scanner));
				scanner.GetNextToken();
				AssertEquals("Comments (11)", JSToken.Assert,
							 token.GetToken());
				AssertNull("Comments (12)",
						   JSScannerTest.TestGetIdentifierName(scanner));
				AssertEquals("Comments (13)", 4, token.StartLine);
				AssertEquals("Comments (14)", 4, token.EndLine);
				scanner.GetNextToken();
				AssertEquals("Comments (15)", JSToken.DivideAssign,
							 token.GetToken());
				AssertNull("Comments (16)",
						   JSScannerTest.TestGetIdentifierName(scanner));
				scanner.GetNextToken();
				AssertEquals("Comments (17)", JSToken.Divide,
							 token.GetToken());
				AssertNull("Comments (18)",
						   JSScannerTest.TestGetIdentifierName(scanner));
				try
				{
					scanner.GetNextToken();
					Fail("Comments (19)");
				}
				catch(Exception e)
				{
					AssertEquals("Comments (20)", JSError.NoCommentEnd,
								 JSScannerTest.TestExtractError(e));
					AssertEquals("Comments (21)", JSToken.UnterminatedComment,
							 	 token.GetToken());
				}
			}

	// Test a single operator.
	private void TestOp(JSScanner scanner, String ops,
					    ref int posn, JSToken expected)
			{
				Context token = JSScannerTest.TestGetTokenContext(scanner);
				scanner.GetNextToken();
				int next = ops.IndexOf(' ', posn);
				String thisop = ops.Substring(posn, next - posn);
				AssertEquals("TestOp[" + thisop + "] (1)",
							 expected, token.GetToken());
				AssertEquals("TestOp[" + thisop + "] (2)",
							 thisop, token.GetCode());
				AssertEquals("TestOp[" + thisop + "] (3)",
							 posn, token.StartPosition);
				AssertEquals("TestOp[" + thisop + "] (4)",
							 next, token.EndPosition);
				posn = next + 1;
			}

	// Test operator recognition.
	public void TestScannerOperators()
			{
				JSScanner scanner;
				String ops = "{ } ( ) [ ] . ; , < > <= >= == != " +
							 "=== !== + - * / % ++ -- << >> >>> " +
							 "& | ^ ! ~ && || ? : = += -= *= /= " +
							 "%= <<= >>= >>>= &= |= ^= :: ";
				int posn = 0;
				scanner = JSScannerTest.TestCreateScanner(ops);
				TestOp(scanner, ops, ref posn, JSToken.LeftCurly);
				TestOp(scanner, ops, ref posn, JSToken.RightCurly);
				TestOp(scanner, ops, ref posn, JSToken.LeftParen);
				TestOp(scanner, ops, ref posn, JSToken.RightParen);
				TestOp(scanner, ops, ref posn, JSToken.LeftBracket);
				TestOp(scanner, ops, ref posn, JSToken.RightBracket);
				TestOp(scanner, ops, ref posn, JSToken.AccessField);
				TestOp(scanner, ops, ref posn, JSToken.Semicolon);
				TestOp(scanner, ops, ref posn, JSToken.Comma);
				TestOp(scanner, ops, ref posn, JSToken.LessThan);
				TestOp(scanner, ops, ref posn, JSToken.GreaterThan);
				TestOp(scanner, ops, ref posn, JSToken.LessThanEqual);
				TestOp(scanner, ops, ref posn, JSToken.GreaterThanEqual);
				TestOp(scanner, ops, ref posn, JSToken.Equal);
				TestOp(scanner, ops, ref posn, JSToken.NotEqual);
				TestOp(scanner, ops, ref posn, JSToken.StrictEqual);
				TestOp(scanner, ops, ref posn, JSToken.StrictNotEqual);
				TestOp(scanner, ops, ref posn, JSToken.Plus);
				TestOp(scanner, ops, ref posn, JSToken.Minus);
				TestOp(scanner, ops, ref posn, JSToken.Multiply);
				TestOp(scanner, ops, ref posn, JSToken.Divide);
				TestOp(scanner, ops, ref posn, JSToken.Modulo);
				TestOp(scanner, ops, ref posn, JSToken.Increment);
				TestOp(scanner, ops, ref posn, JSToken.Decrement);
				TestOp(scanner, ops, ref posn, JSToken.LeftShift);
				TestOp(scanner, ops, ref posn, JSToken.RightShift);
				TestOp(scanner, ops, ref posn, JSToken.UnsignedRightShift);
				TestOp(scanner, ops, ref posn, JSToken.BitwiseAnd);
				TestOp(scanner, ops, ref posn, JSToken.BitwiseOr);
				TestOp(scanner, ops, ref posn, JSToken.BitwiseXor);
				TestOp(scanner, ops, ref posn, JSToken.LogicalNot);
				TestOp(scanner, ops, ref posn, JSToken.BitwiseNot);
				TestOp(scanner, ops, ref posn, JSToken.LogicalAnd);
				TestOp(scanner, ops, ref posn, JSToken.LogicalOr);
				TestOp(scanner, ops, ref posn, JSToken.ConditionalIf);
				TestOp(scanner, ops, ref posn, JSToken.Colon);
				TestOp(scanner, ops, ref posn, JSToken.Assign);
				TestOp(scanner, ops, ref posn, JSToken.PlusAssign);
				TestOp(scanner, ops, ref posn, JSToken.MinusAssign);
				TestOp(scanner, ops, ref posn, JSToken.MultiplyAssign);
				TestOp(scanner, ops, ref posn, JSToken.DivideAssign);
				TestOp(scanner, ops, ref posn, JSToken.ModuloAssign);
				TestOp(scanner, ops, ref posn, JSToken.LeftShiftAssign);
				TestOp(scanner, ops, ref posn, JSToken.RightShiftAssign);
				TestOp(scanner, ops, ref posn,
					   JSToken.UnsignedRightShiftAssign);
				TestOp(scanner, ops, ref posn, JSToken.BitwiseAndAssign);
				TestOp(scanner, ops, ref posn, JSToken.BitwiseOrAssign);
				TestOp(scanner, ops, ref posn, JSToken.BitwiseXorAssign);
				TestOp(scanner, ops, ref posn, JSToken.DoubleColon);
			}

	// Parse a number.
	private void TestNum(String value, JSToken expected)
			{
				JSScanner scanner;
				Context token;
				scanner = JSScannerTest.TestCreateScanner(value);
				token = JSScannerTest.TestGetTokenContext(scanner);
				scanner.GetNextToken();
				AssertEquals("TestNum[" + value + "] (1)",
							 value, token.GetCode());
				AssertEquals("TestNum[" + value + "] (2)",
							 expected, token.GetToken());
			}

	// Parse an invalid number.
	private void TestInvalidNum(String value, JSError expected)
			{
				JSScanner scanner;
				Context token;
				scanner = JSScannerTest.TestCreateScanner(value);
				token = JSScannerTest.TestGetTokenContext(scanner);
				try
				{
					scanner.GetNextToken();
					Fail("TestInvalidNum[" + value + "] (1)");
				}
				catch(Exception e)
				{
					AssertEquals("TestNum[" + value + "] (2)", expected,
								 JSScannerTest.TestExtractError(e));
				}
			}

	// Test number recognition.
	public void TestScannerNumbers()
			{
				TestNum("0", JSToken.IntegerLiteral);
				TestNum("0x17a", JSToken.IntegerLiteral);
				TestNum("0X17A", JSToken.IntegerLiteral);
				TestNum("123", JSToken.IntegerLiteral);
				TestNum("123.0", JSToken.NumericLiteral);
				TestNum(".123", JSToken.NumericLiteral);
				TestNum("1e123", JSToken.NumericLiteral);
				TestNum("1E-123", JSToken.NumericLiteral);
				TestNum("1.5E-123", JSToken.NumericLiteral);
				TestNum(".5E+123", JSToken.NumericLiteral);
				TestInvalidNum("0x", JSError.BadHexDigit);
				TestInvalidNum("0xG", JSError.BadHexDigit);
			}

}; // class TestScanner
