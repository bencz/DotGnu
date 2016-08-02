/*
 * JSParser.cs - Main interface to the JScript parsing routines.
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
using System.Globalization;

public class JSParser
{
	// Internal state.
	private Context context;
	private JSScanner scanner;
	private Context tokenInfo;
	private JSToken token;
	internal bool printSupported;
	internal int numErrors;

	// Constructor.
	public JSParser(Context context)
			{
				this.context = context;
				if(context != null)
				{
					scanner = new JSScanner(context);
				}
				else
				{
					scanner = new JSScanner();
				}
				tokenInfo = scanner.GetTokenContext();
				token = tokenInfo.token;
				numErrors = 0;
			}

	// Parse all of the input source and return the root of the AST.
	public ScriptBlock Parse()
			{
				JNode root = ParseSource(false);
				return new ScriptBlock(root);
			}

	// Parse input source for an "eval" statement and return the AST.
	public Block ParseEvalBody()
			{
				JNode root = ParseSource(true);
				return new Block(root);
			}

#if DEBUG
	// Tokenize the input stream.  API provided for backwards-compatibility.
	public void Tokenize()
			{
				try
				{
					long num = 1;
					NextToken();
					while(token != JSToken.EndOfFile)
					{
						++num;
						NextToken();
					}
					Console.WriteLine("Number of tokens: {0}", num);
				}
				catch(JSScanner.ScannerFailure e)
				{
					Console.Write("Scanner failed: ");
					Console.WriteLine(e.ToString());
				}
			}
#endif

	// Get the next token from the input stream.
	private void NextToken()
			{
				scanner.GetNextToken();
				token = tokenInfo.token;
			}

	// Report a syntax error at the current token, and start error recovery.
	private void SyntaxError(Context tokenInfo, String msg)
			{
				++numErrors;
				JScriptException e = new JScriptException
					(JSError.SyntaxError, tokenInfo.MakeCopy());
				e.message = msg;
				if(context.codebase != null && context.codebase.site != null)
				{
					if(context.codebase.site.OnCompilerError(e))
					{
						// The site wants us to perform error recovery.
						throw new ErrorRecovery(e);
					}
				}
				throw e;
			}
	private void SyntaxError(String msg)
			{
				SyntaxError(tokenInfo, msg);
			}

	// Expect a particular token, and skip it if found.
	private void Expect(JSToken expected, String msg)
			{
				if(token != expected)
				{
					SyntaxError(msg);
				}
				NextToken();
			}

	// Parse an array literal.
	private JNode ArrayLiteral()
			{
				Context save = tokenInfo.MakeCopy();
				JArrayLiteral literal = new JArrayLiteral(save, 0);
				JNode expr;
				int size = 0;
				bool lastWasComma = false;
				NextToken();		// Skip the '['.
				while(token != JSToken.RightBracket)
				{
					if(token != JSToken.Comma)
					{
						expr = AssignmentExpression(false);
						if(token == JSToken.Comma)
						{
							NextToken();
							lastWasComma = true;
						}
						else
						{
							lastWasComma = false;
						}
					}
					else
					{
						expr = new JUndefined(tokenInfo.MakeCopy());
						NextToken();
						lastWasComma = true;
					}
					Support.AddExprToList(literal, null, expr);
					++size;
				}
				if(lastWasComma)
				{
					Support.AddExprToList
						(literal, null, new JUndefined(tokenInfo.MakeCopy()));
					++size;
				}
				literal.context = Context.BuildRange(save, tokenInfo);
				literal.size = size;
				Expect(JSToken.RightBracket, "`]' expected");
				return literal;
			}

	// Parse a numeric value from the scanner.
	private Object NumericValue(String text)
			{
				if(text.Length > 2 && text[1] == 'x' && text[0] == '0')
				{
					long longValue = Int64.Parse(text.Substring(2), 
												NumberStyles.HexNumber);
				
					if( longValue >= Int32.MinValue &&
					    longValue <= Int32.MaxValue)
					{
						return (int)longValue;
					}
					
					return longValue;
				}
				else
				{
					double value = Double.Parse(text);
					if(value >= (double)(Int32.MinValue) &&
					   value <= (double)(Int32.MaxValue) &&
					   Math.Round(value) == value)
					{
						return (int)value;
					}
					else if(value >= (double)(Int64.MinValue) &&
							value <= (double)(Int64.MaxValue) &&
							Math.Round(value) == value)
					{
						return (long)value;
					}
					else
					{
						return value;
					}
				}
			}

	// Parse an object literal.
	private JNode ObjectLiteral()
			{
				Context save = tokenInfo.MakeCopy();
				JObjectLiteral literal = new JObjectLiteral(save);
				Object name = null;
				JNode expr;
				NextToken();		// Skip the '{'.
				while(token != JSToken.RightCurly)
				{
					if(token == JSToken.Identifier)
					{
						name = scanner.GetIdentifierName(); 
					}
					else if(token == JSToken.StringLiteral)
					{
						name = scanner.GetStringLiteral();
					}
					else if(token == JSToken.IntegerLiteral ||
							token == JSToken.NumericLiteral)
					{
						name = NumericValue(tokenInfo.GetCode());
					}
					else
					{
						SyntaxError("property name expected");
					}
					NextToken();
					Expect(JSToken.Colon, "`:' expected");
					expr = AssignmentExpression(false);
					Support.AddExprToList(literal, name, expr);
					if(token == JSToken.Comma)
					{
						NextToken();
					}
					else if(token != JSToken.RightCurly)
					{
						SyntaxError("`,' or `}' expected");
					}
				}
				literal.context = Context.BuildRange(save, tokenInfo);
				Expect(JSToken.RightCurly, "`}' expected");
				return literal;
			}

	// Parse a primary expression.
	private JNode PrimaryExpression()
			{
				JNode expr = null;
				switch(token)
				{
					case JSToken.This:
					{
						// "this" reference.
						expr = new JThis(tokenInfo.MakeCopy());
						NextToken();
					}
					break;

					case JSToken.Super:
					{
						// "super" reference.
						expr = new JThis(tokenInfo.MakeCopy());
						NextToken();
					}
					break;

					case JSToken.Identifier:
					{
						// Identifier reference.
						expr = new JIdentifier
							(tokenInfo.MakeCopy(), scanner.GetIdentifierName());
						NextToken();
					}
					break;

					case JSToken.IntegerLiteral:
					case JSToken.NumericLiteral:
					{
						// Numeric literal value.
						expr = new JConstant
								(tokenInfo.MakeCopy(),
								 NumericValue(tokenInfo.GetCode()));
						NextToken();
					}
					break;

					case JSToken.StringLiteral:
					{
						// String literal value.
						expr = new JConstant
							(tokenInfo.MakeCopy(), scanner.GetStringLiteral());
						NextToken();
					}
					break;

					case JSToken.Null:
					{
						expr = new JConstant
							(tokenInfo.MakeCopy(), DBNull.Value);
						NextToken();
					}
					break;

					case JSToken.True:
					{
						expr = new JConstant(tokenInfo.MakeCopy(), true);
						NextToken();
					}
					break;

					case JSToken.False:
					{
						expr = new JConstant(tokenInfo.MakeCopy(), false);
						NextToken();
					}
					break;

					case JSToken.LeftBracket:
					{
						// Array literal.
						return ArrayLiteral();
					}
					// Not reached.

					case JSToken.LeftCurly:
					{
						// Object literal.
						return ObjectLiteral();
					}
					// Not reached.

					case JSToken.LeftParen:
					{
						// Parse an expression of the form "(e)".
						NextToken();
						expr = Expression();
						Expect(JSToken.RightParen, "`)' expected");
						return expr;
					}
					// Not reached.

					default:
					{
						// This is not a valid primary expression.
						SyntaxError("`this', `super', literal, identifier, " +
									"or parenthesized expression expected");
					}
					break;
				}
				return expr;
			}

	// Parse an argument list.
	private JNode Arguments()
			{
				if(token == JSToken.RightParen)
				{
					return null;
				}
				JNode expr = AssignmentExpression(false);
				JNode expr2;
				while(token == JSToken.Comma)
				{
					NextToken();
					expr2 = AssignmentExpression(false);
					expr = new JArgList(Context.BuildRange
										  (expr.context, expr2.context),
										expr, expr2);
				}
				return expr;
			}

	// Parse a member expression.
	private JNode MemberExpression()
			{
				JNode expr = null;
				JNode expr2;
				Context save;
				switch(token)
				{
					case JSToken.Function:
					{
						// Inline function definition.
						expr = FunctionExpression();
					}
					break;

					case JSToken.New:
					{
						// Object construction, with arguments.
						save = tokenInfo.MakeCopy();
						NextToken();
						expr = MemberExpression();
						if(token == JSToken.LeftParen)
						{
							NextToken();
							expr2 = Arguments();
							if(token == JSToken.RightParen)
							{
								expr = new JNew(Context.BuildRange
													(save, tokenInfo),
												expr, expr2);
								NextToken();
							}
							else
							{
								SyntaxError("`)' expected");
							}
						}
						else
						{
							expr = new JNew(Context.BuildRange
												(save, expr.context),
											expr, null);
						}
					}
					break;

					default:
					{
						// Parse a primary expression.
						expr = PrimaryExpression();
					}
					break;
				}
				while(token == JSToken.LeftBracket ||
				      token == JSToken.AccessField)
				{
					if(token == JSToken.LeftBracket)
					{
						// Parse an array access expression.
						NextToken();
						expr2 = Expression();
						if(token == JSToken.RightBracket)
						{
							expr = new JArrayAccess
									(Context.BuildRange(expr.context,
														tokenInfo),
									 expr, expr2);
							NextToken();
						}
						else
						{
							SyntaxError("`]' expected");
						}
					}
					else
					{
						// Parse a field access expression.
						NextToken();
						if(token == JSToken.Identifier)
						{
							expr = new JFieldAccess
									(Context.BuildRange(expr.context,
														tokenInfo),
									 expr, scanner.GetIdentifierName());
							NextToken();
						}
						else
						{
							SyntaxError("identifier expected");
						}
					}
				}
				return expr;
			}

	// Determine if a node corresponds to a builtin function reference.
	private bool IsBuiltinCall(JNode node, String name)
			{
				return (node is JIdentifier &&
						((JIdentifier)node).name == name);
			}

	// Parse an argument list.
	private JNode ArgumentList()
			{
				JNode args, arg;
				if(token == JSToken.RightParen)
				{
						return null;
				}
				args = AssignmentExpression(false);
				while(token == JSToken.Comma)
				{
					NextToken();
					arg = AssignmentExpression(false);
					args = new JArgList(Context.BuildRange
											(args.context, arg.context),
										args, arg);
				}
				return args;
			}

	// Parse a left hand side expression.
	private JNode LeftHandSideExpression()
			{
				JNode expr = MemberExpression();
				JNode expr2;
				while(token == JSToken.LeftParen ||
				      token == JSToken.LeftBracket ||
					  token == JSToken.AccessField)
				{
					if(token == JSToken.LeftParen)
					{
						// Parse a function call expression.
						NextToken();
						expr2 = ArgumentList();
						if(token == JSToken.RightParen)
						{
							if(IsBuiltinCall(expr, "eval"))
							{
								expr = new JEval
										(Context.BuildRange
											(expr.context, tokenInfo),
										 expr2);
							}
							else if(IsBuiltinCall(expr, "print") &&
									printSupported)
							{
								expr = new JPrint
										(Context.BuildRange
											(expr.context, tokenInfo),
										 expr2);
							}
							else
							{
								expr = new JCall
										(Context.BuildRange(expr.context,
															tokenInfo),
										 expr, expr2);
							}
							NextToken();
						}
						else
						{
							SyntaxError("`)' expected");
						}
					}
					else if(token == JSToken.LeftBracket)
					{
						// Parse an array access expression.
						NextToken();
						expr2 = Expression();
						if(token == JSToken.RightBracket)
						{
							expr = new JArrayAccess
									(Context.BuildRange(expr.context,
														tokenInfo),
									 expr, expr2);
							NextToken();
						}
						else
						{
							SyntaxError("`]' expected");
						}
					}
					else
					{
						// Parse a field access expression.
						NextToken();
						if(token == JSToken.Identifier)
						{
							expr = new JFieldAccess
									(Context.BuildRange(expr.context,
														tokenInfo),
									 expr, scanner.GetIdentifierName());
							NextToken();
						}
						else
						{
							SyntaxError("identifier expected");
						}
					}
				}
				return expr;
			}

	// Determine if a node is a left-hand side expression.
	private void CheckLeftHandSideExpression(JNode node)
			{
				if(!(node is JConstant) &&
				   !(node is JArrayLiteral) &&
				   !(node is JThis) &&
				   !(node is JSuper) &&
				   !(node is JIdentifier) &&
				   !(node is JNew) &&
				   !(node is JArrayAccess) &&
				   !(node is JFieldAccess) &&
				   !(node is JCall))
				{
					SyntaxError(node.context, "invalid l-value");
				}
			}

	// Parse a postfix expression.
	private JNode PostfixExpression()
			{
				JNode expr = LeftHandSideExpression();
				if(token == JSToken.Increment && !scanner.GotEndOfLine())
				{
					NextToken();
					expr = new JPostInc(Context.BuildRange
										(expr.context, tokenInfo), expr);
				}
				else if(token == JSToken.Decrement && !scanner.GotEndOfLine())
				{
					NextToken();
					expr = new JPostDec(Context.BuildRange
										(expr.context, tokenInfo), expr);
				}
				return expr;
			}

	// Parse a unary expression.
	private JNode UnaryExpression()
			{
				Context save = tokenInfo.MakeCopy();
				JNode expr = null;
				switch(token)
				{
					case JSToken.Delete:
					{
						NextToken();
						expr = UnaryExpression();
						expr = new JDelete(Context.BuildRange
											(save, expr.context), expr);
					}
					break;

					case JSToken.Void:
					{
						NextToken();
						expr = UnaryExpression();
						expr = new JVoid(Context.BuildRange
											(save, expr.context), expr);
					}
					break;

					case JSToken.Typeof:
					{
						NextToken();
						expr = UnaryExpression();
						expr = new JTypeof(Context.BuildRange
											(save, expr.context), expr);
					}
					break;

					case JSToken.Increment:
					{
						NextToken();
						expr = UnaryExpression();
						expr = new JPreInc(Context.BuildRange
											(save, expr.context), expr);
					}
					break;

					case JSToken.Decrement:
					{
						NextToken();
						expr = UnaryExpression();
						expr = new JPreDec(Context.BuildRange
											(save, expr.context), expr);
					}
					break;

					case JSToken.Plus:
					{
						NextToken();
						expr = UnaryExpression();
						expr = new JUnaryPlus(Context.BuildRange
										(save, expr.context), expr);
					}
					break;

					case JSToken.Minus:
					{
						NextToken();
						expr = UnaryExpression();
						expr = new JNeg(Context.BuildRange
											(save, expr.context), expr);
					}
					break;

					case JSToken.BitwiseNot:
					{
						NextToken();
						expr = UnaryExpression();
						expr = new JBitwiseNot(Context.BuildRange
										(save, expr.context), expr);
					}
					break;

					case JSToken.LogicalNot:
					{
						NextToken();
						expr = UnaryExpression();
						expr = new JLogicalNot(Context.BuildRange
										(save, expr.context), expr);
					}
					break;

					default:
					{
						return PostfixExpression();
					}
					// Not reached.
				}
				return expr;
			}

	// Parse a multiplicative expression.
	private JNode MultiplicativeExpression()
			{
				JNode expr, expr2;
				expr = UnaryExpression();
				for(;;)
				{
					if(token == JSToken.Multiply)
					{
						NextToken();
						expr2 = UnaryExpression();
						expr = new JMul(Context.BuildRange
										  (expr.context, expr2.context),
									    expr, expr2);
					}
					else if(token == JSToken.Divide)
					{
						NextToken();
						expr2 = UnaryExpression();
						expr = new JDiv(Context.BuildRange
										  (expr.context, expr2.context),
									    expr, expr2);
					}
					else if(token == JSToken.Modulo)
					{
						NextToken();
						expr2 = UnaryExpression();
						expr = new JRem(Context.BuildRange
										  (expr.context, expr2.context),
									    expr, expr2);
					}
					else
					{
						break;
					}
				}
				return expr;
			}

	// Parse an additive expression.
	private JNode AdditiveExpression()
			{
				JNode expr, expr2;
				expr = MultiplicativeExpression();
				for(;;)
				{
					if(token == JSToken.Plus)
					{
						NextToken();
						expr2 = MultiplicativeExpression();
						expr = new JAdd(Context.BuildRange
										  (expr.context, expr2.context),
									    expr, expr2);
					}
					else if(token == JSToken.Minus)
					{
						NextToken();
						expr2 = MultiplicativeExpression();
						expr = new JSub(Context.BuildRange
										  (expr.context, expr2.context),
									    expr, expr2);
					}
					else
					{
						break;
					}
				}
				return expr;
			}

	// Parse a relational expression.
	private JNode ShiftExpression()
			{
				JNode expr, expr2;
				expr = AdditiveExpression();
				for(;;)
				{
					if(token == JSToken.LeftShift)
					{
						NextToken();
						expr2 = AdditiveExpression();
						expr = new JShl(Context.BuildRange
										  (expr.context, expr2.context),
									    expr, expr2);
					}
					else if(token == JSToken.RightShift)
					{
						NextToken();
						expr2 = AdditiveExpression();
						expr = new JShr(Context.BuildRange
										  (expr.context, expr2.context),
									    expr, expr2);
					}
					else if(token == JSToken.UnsignedRightShift)
					{
						NextToken();
						expr2 = AdditiveExpression();
						expr = new JUShr(Context.BuildRange
										   (expr.context, expr2.context),
									     expr, expr2);
					}
					else
					{
						break;
					}
				}
				return expr;
			}

	// Parse a relational expression.
	private JNode RelationalExpression(bool noIn)
			{
				JNode expr, expr2;
				expr = ShiftExpression();
				for(;;)
				{
					if(token == JSToken.LessThan)
					{
						NextToken();
						expr2 = ShiftExpression();
						expr = new JLt(Context.BuildRange
										  (expr.context, expr2.context),
									   expr, expr2);
					}
					else if(token == JSToken.LessThanEqual)
					{
						NextToken();
						expr2 = ShiftExpression();
						expr = new JLe(Context.BuildRange
										  (expr.context, expr2.context),
									   expr, expr2);
					}
					else if(token == JSToken.GreaterThan)
					{
						NextToken();
						expr2 = ShiftExpression();
						expr = new JGt(Context.BuildRange
										  (expr.context, expr2.context),
									   expr, expr2);
					}
					else if(token == JSToken.GreaterThanEqual)
					{
						NextToken();
						expr2 = ShiftExpression();
						expr = new JGe(Context.BuildRange
										  (expr.context, expr2.context),
									   expr, expr2);
					}
					else if(token == JSToken.Instanceof)
					{
						NextToken();
						expr2 = ShiftExpression();
						expr = new JInstanceof(Context.BuildRange
										  		 (expr.context, expr2.context),
									   		   expr, expr2);
					}
					else if(token == JSToken.In && !noIn)
					{
						NextToken();
						expr2 = ShiftExpression();
						expr = new JIn(Context.BuildRange
										 (expr.context, expr2.context),
									   expr, expr2);
					}
					else
					{
						break;
					}
				}
				return expr;
			}

	// Parse an equality expression.
	private JNode EqualityExpression(bool noIn)
			{
				JNode expr, expr2;
				expr = RelationalExpression(noIn);
				for(;;)
				{
					if(token == JSToken.Equal)
					{
						NextToken();
						expr2 = RelationalExpression(noIn);
						expr = new JEq(Context.BuildRange
										  (expr.context, expr2.context),
									   expr, expr2);
					}
					else if(token == JSToken.NotEqual)
					{
						NextToken();
						expr2 = RelationalExpression(noIn);
						expr = new JNe(Context.BuildRange
										  (expr.context, expr2.context),
									   expr, expr2);
					}
					else if(token == JSToken.StrictEqual)
					{
						NextToken();
						expr2 = RelationalExpression(noIn);
						expr = new JStrictEq(Context.BuildRange
										  		(expr.context, expr2.context),
									   		 expr, expr2);
					}
					else if(token == JSToken.StrictNotEqual)
					{
						NextToken();
						expr2 = RelationalExpression(noIn);
						expr = new JStrictNe(Context.BuildRange
										  		(expr.context, expr2.context),
									   		 expr, expr2);
					}
					else
					{
						break;
					}
				}
				return expr;
			}

	// Parse a bitwise AND expression.
	private JNode BitwiseAndExpression(bool noIn)
			{
				JNode expr, expr2;
				expr = EqualityExpression(noIn);
				while(token == JSToken.BitwiseAnd)
				{
					NextToken();
					expr2 = EqualityExpression(noIn);
					expr = new JBitwiseAnd(Context.BuildRange
											  (expr.context, expr2.context),
									       expr, expr2);
				}
				return expr;
			}

	// Parse a bitwise XOR expression.
	private JNode BitwiseXorExpression(bool noIn)
			{
				JNode expr, expr2;
				expr = BitwiseAndExpression(noIn);
				while(token == JSToken.BitwiseXor)
				{
					NextToken();
					expr2 = BitwiseAndExpression(noIn);
					expr = new JBitwiseXor(Context.BuildRange
											(expr.context, expr2.context),
									       expr, expr2);
				}
				return expr;
			}

	// Parse a bitwise OR expression.
	private JNode BitwiseOrExpression(bool noIn)
			{
				JNode expr, expr2;
				expr = BitwiseXorExpression(noIn);
				while(token == JSToken.BitwiseOr)
				{
					NextToken();
					expr2 = BitwiseXorExpression(noIn);
					expr = new JBitwiseOr(Context.BuildRange
											(expr.context, expr2.context),
									      expr, expr2);
				}
				return expr;
			}

	// Parse a logical AND expression.
	private JNode LogicalAndExpression(bool noIn)
			{
				JNode expr, expr2;
				expr = BitwiseOrExpression(noIn);
				while(token == JSToken.LogicalAnd)
				{
					NextToken();
					expr2 = BitwiseOrExpression(noIn);
					expr = new JLogicalAnd(Context.BuildRange
											(expr.context, expr2.context),
									       expr, expr2);
				}
				return expr;
			}

	// Parse a logical OR expression.
	private JNode LogicalOrExpression(bool noIn)
			{
				JNode expr, expr2;
				expr = LogicalAndExpression(noIn);
				while(token == JSToken.LogicalOr)
				{
					NextToken();
					expr2 = LogicalAndExpression(noIn);
					expr = new JLogicalOr(Context.BuildRange
											(expr.context, expr2.context),
									      expr, expr2);
				}
				return expr;
			}

	// Parse a conditional expression.
	private JNode ConditionalExpression(bool noIn)
			{
				JNode expr1 = LogicalOrExpression(noIn);
				JNode expr2, expr3;
				if(token == JSToken.ConditionalIf)
				{
					NextToken();
					expr2 = AssignmentExpression(false);
					Expect(JSToken.Colon, "`:' expected");
					expr3 = AssignmentExpression(noIn);
					expr1 = new JIfExpr(Context.BuildRange
											(expr1.context, expr3.context),
										expr1, expr2, expr3);
				}
				return expr1;
			}

	// Parse an assignment expression.
	private JNode AssignmentExpression(bool noIn)
			{
				JNode expr = ConditionalExpression(noIn);
				if(token >= JSToken.Assign && token <= JSToken.LastAssign)
				{
					CheckLeftHandSideExpression(expr);
					JSToken assignOp = token;
					NextToken();
					JNode rhs = AssignmentExpression(noIn);
					Context context = Context.BuildRange
						(expr.context, rhs.context);
					if(assignOp == JSToken.Assign)
					{
						expr = new JAssign(context, expr, rhs);
					}
					else
					{
						expr = new JAssignOp(context, assignOp, expr, rhs);
					}
				}
				return expr;
			}

	// Parse an expression.
	private JNode Expression()
			{
				JNode expr = AssignmentExpression(false);
				JNode expr2;
				while(token == JSToken.Comma)
				{
					NextToken();
					expr2 = AssignmentExpression(false);
					expr = new JComma(Context.BuildRange
										(expr.context, expr2.context),
									  expr, expr2);
				}
				return expr;
			}

	// Parse an expression that does not contain the "in" operator.
	private JNode ExpressionNoIn()
			{
				JNode expr = AssignmentExpression(true);
				JNode expr2;
				while(token == JSToken.Comma)
				{
					NextToken();
					expr2 = AssignmentExpression(true);
					expr = new JComma(Context.BuildRange
										(expr.context, expr2.context),
									  expr, expr2);
				}
				return expr;
			}

	// Parse a bracketed expression: "(e)".
	private JNode BracketedExpression()
			{
				JNode expr;
				Expect(JSToken.LeftParen, "`(' expected");
				expr = Expression();
				Expect(JSToken.RightParen, "`)' expected");
				return expr;
			}

	// Parse a single variable declaration statement.
	private JNode VariableDeclaration(bool noIn)
			{
				String name;
				Context save;
				JNode expr;
				if(token != JSToken.Identifier)
				{
					SyntaxError("variable identifier expected");
				}
				save = tokenInfo.MakeCopy();
				name = scanner.GetIdentifierName();
				NextToken();
				if(token == JSToken.Assign)
				{
					NextToken();
					expr = AssignmentExpression(noIn);
					return new JVarDecl(Context.BuildRange(save, expr.context),
										name, expr);
				}
				else
				{
					return new JVarDecl(save, name, null);
				}
			}

	// Parse a variable declaration list.
	private JNode VariableDeclarationList(bool noIn)
			{
				JNode stmt, stmt2;
				stmt = VariableDeclaration(noIn);
				while(token == JSToken.Comma)
				{
					NextToken();
					stmt2 = VariableDeclaration(noIn);
					stmt = Support.CreateCompound(stmt, stmt2);
				}
				return stmt;
			}

	// Determine if the current token is a semi-colon or if a
	// semi-colon can be implicitly inserted here.
	private void MatchSemiColon()
			{
				if(token == JSToken.Semicolon)
				{
					// We got a real semi-colon.
					NextToken();
				}
				else if(token == JSToken.RightCurly ||
						token == JSToken.EndOfFile ||
						scanner.GotEndOfLine())
				{
					// We can implicitly insert a semi-colon.
					return;
				}
				else
				{
					// Nothing that looks like a semi-colon was found here.
					SyntaxError("`;' expected");
				}
			}

	// Determine if we have a semi-colon, either explicitly or implicitly.
	private bool HaveSemiColon()
			{
				if(token == JSToken.Semicolon)
				{
					return true;
				}
				else if(token == JSToken.RightCurly ||
						token == JSToken.EndOfFile ||
						scanner.GotEndOfLine())
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Parse a statement expression.  Same as an expression, except
	// that "(e)" is handled as a special case to disambiguate labels.
	private JNode StatementExpression()
			{
				if(token == JSToken.LeftParen)
				{
					NextToken();
					JNode expr = Expression();
					Expect(JSToken.RightParen, "`)' expected");
					return new JAsIs(expr.context.MakeCopy(), expr);
				}
				else
				{
					return Expression();
				}
			}

	// Parse a list of statements within a "switch" case block.
	private JNode CaseStatementList()
			{
				JNode stmt = null;
				JNode stmt2;
				while(token != JSToken.RightCurly &&
					  token != JSToken.Case &&
					  token != JSToken.Default)
				{
					stmt2 = Statement();
					stmt = Support.CreateCompound(stmt, stmt2);
				}
				if(stmt == null)
				{
					stmt = new JEmpty(tokenInfo.MakeCopy());
				}
				return stmt;
			}

	// Update a case node with fall-through information.
	private void UpdateFallThrough(JNode caseStmt, JNode stmt)
			{
				if(caseStmt is JCase)
				{
					((JCase)caseStmt).body =
						Support.CreateCompound
							(((JCase)caseStmt).body,
							 new JFallThrough(tokenInfo.MakeCopy(), stmt));
				}
				else if(caseStmt is JDefault)
				{
					((JDefault)caseStmt).body =
						Support.CreateCompound
							(((JDefault)caseStmt).body,
							 new JFallThrough(tokenInfo.MakeCopy(), stmt));
				}
			}

	// Parse a case block, consisting of a list of case clauses.
	private JNode CaseBlock()
			{
				JNode stmt = null;
				JNode expr = null;
				JNode caseStmt, body;
				Context save = null;
				JNode defCase = null;
				JNode prev = null;
				while(token != JSToken.RightCurly)
				{
					if(token == JSToken.Case)
					{
						save = tokenInfo.MakeCopy();
						NextToken();
						expr = Expression();
						Expect(JSToken.Colon, "`:' expected");
						body = CaseStatementList();
						caseStmt = new JCase(Context.BuildRange
												(save, expr.context),
											 expr, body);
						stmt = Support.CreateCompound(stmt, caseStmt);
						if(prev != null)
						{
							UpdateFallThrough(prev, caseStmt);
						}
						prev = caseStmt;
					}
					else if(token == JSToken.Default)
					{
						NextToken();
						Expect(JSToken.Colon, "`:' expected");
						body = CaseStatementList();
						caseStmt = new JDefault(Context.BuildRange
												  (save, expr.context), body);
						stmt = Support.CreateCompound(stmt, caseStmt);
						if(prev != null)
						{
							UpdateFallThrough(prev, caseStmt);
						}
						prev = caseStmt;
						if(defCase == null)
						{
							defCase = caseStmt;
						}
						else
						{
							SyntaxError("multiple `default' clauses in " +
										"`switch' body");
						}
					}
					else
					{
						SyntaxError("`case' or `default' expected");
					}
				}
				return stmt;
			}

	// Parse a block.
	private JNode Block()
			{
				if(token != JSToken.LeftCurly)
				{
					SyntaxError("`{' expected");
				}
				return Statement();
			}

	// Parse a statement.
	private JNode Statement()
			{
				JNode stmt = null;
				JNode temp, temp2;
				Context save;

				// Save the current context, so that we know
				// where the statement starts.
				save = tokenInfo.MakeCopy();

				// Determine what kind of statement to parse.
				switch(token)
				{
					case JSToken.LeftCurly:
					{
						// Compound statement encountered.
						NextToken();
						while(token != JSToken.RightCurly &&
							  token != JSToken.EndOfFile)
						{
							temp = Statement();
							stmt = Support.CreateCompound(stmt, temp);
						}
						if(stmt == null)
						{
							stmt = new JEmpty(Context.BuildRange
												(save, tokenInfo));
						}
						else if(stmt is JCompound)
						{
							// Make sure the compound statement includes
							// the curly brackets.
							stmt.context = Context.BuildRange
												(save, tokenInfo);
						}
						if(token == JSToken.EndOfFile)
						{
							SyntaxError("`}' expected");
						}
						else
						{
							NextToken();
						}
					}
					break;

					case JSToken.Semicolon:
					{
						// Bare semi-colon for an empty statement.
						stmt = new JEmpty(save);
						NextToken();
					}
					break;

					case JSToken.If:
					{
						// Parse an "if" statement.
						NextToken();
						temp = BracketedExpression();
						stmt = Statement();
						if(token != JSToken.Else)
						{
							stmt = new JIf(Context.BuildRange
												(save, stmt.context),
										   temp, stmt, null);
						}
						else
						{
							NextToken();
							temp2 = Statement();
							stmt = new JIf(Context.BuildRange
												(save, temp2.context),
										   temp, stmt, temp2);
						}
					}
					break;

					case JSToken.While:
					{
						// Parse a "while" statement.
						NextToken();
						temp = BracketedExpression();
						stmt = Statement();
						stmt = new JWhile(Context.BuildRange
											(save, stmt.context),
										  temp, stmt);
					}
					break;

					case JSToken.Do:
					{
						// Parse a "do" statement.
						NextToken();
						stmt = Statement();
						Expect(JSToken.While, "`while' expected");
						temp = BracketedExpression();
						stmt = new JDo(Context.BuildRange(save, temp.context),
									   stmt, temp);
					}
					break;

					case JSToken.Continue:
					{
						// Parse a "continue" statement.
						NextToken();
						if(HaveSemiColon())
						{
							// Continue with no label.
							stmt = new JContinue(save, null);
							MatchSemiColon();
						}
						else if(token == JSToken.Identifier)
						{
							// Extract the label.
							stmt = new JContinue
								(Context.BuildRange(save, tokenInfo),
								 scanner.GetIdentifierName());
							NextToken();
							MatchSemiColon();
						}
						else
						{
							SyntaxError("`continue' label expected");
						}
					}
					break;

					case JSToken.Break:
					{
						// Parse a "break" statement.
						NextToken();
						if(HaveSemiColon())
						{
							// Break with no label.
							stmt = new JBreak(save, null);
							MatchSemiColon();
						}
						else if(token == JSToken.Identifier)
						{
							// Extract the label.
							stmt = new JBreak
								(Context.BuildRange(save, tokenInfo),
								 scanner.GetIdentifierName());
							NextToken();
							MatchSemiColon();
						}
						else
						{
							SyntaxError("`break' label expected");
						}
					}
					break;

					case JSToken.Return:
					{
						// Parse a "return" statement.
						NextToken();
						if(HaveSemiColon())
						{
							// Return with no expression.
							stmt = new JReturn(save);
							MatchSemiColon();
						}
						else
						{
							// Return with an expression.
							temp = Expression();
							stmt = new JReturnExpr
								(Context.BuildRange(save, temp.context), temp);
							MatchSemiColon();
						}
					}
					break;

					case JSToken.Throw:
					{
						// Parse a "throw" statement.
						NextToken();
						if(HaveSemiColon())
						{
							// Throw with missing expression.
							SyntaxError("expression expected");
						}
						else
						{
							// Parse the expression for the "throw".
							temp = Expression();
							stmt = new JThrow
								(Context.BuildRange(save, temp.context), temp);
							MatchSemiColon();
						}
					}
					break;

					case JSToken.With:
					{
						// Parse a "with" statement.
						NextToken();
						temp = BracketedExpression();
						stmt = Statement();
						stmt = new JWith(Context.BuildRange(save, stmt.context),
										 temp, stmt);
					}
					break;

					case JSToken.Var:
					{
						// Parse a variable declaration.
						NextToken();
						stmt = VariableDeclarationList(false);
						MatchSemiColon();
					}
					break;

					case JSToken.For:
					{
						// Parse a "for" statement.
						JNode init, cond, incr;
						NextToken();
						Expect(JSToken.LeftParen, "`(' expected");

						// Parse the initialization expression.
						if(token == JSToken.Var)
						{
							// Starts with a variable declaration.
							NextToken();
							init = VariableDeclarationList(true);
						}
						else if(token == JSToken.Semicolon)
						{
							// No initialization expression.
							init = null;
						}
						else
						{
							// No variable declarations.
							init = ExpressionNoIn();
							init = new JExprStmt
								(init.context.MakeCopy(), init);
						}

						// Is this the normal or "in" form?
						if(token != JSToken.In)
						{
							Expect(JSToken.Semicolon, "`;' expected");
							if(token != JSToken.Semicolon)
							{
								cond = Expression();
							}
							else
							{
								cond = null;
							}
							Expect(JSToken.Semicolon, "`;' expected");
							if(token != JSToken.RightParen)
							{
								incr = Expression();
							}
							else
							{
								incr = null;
							}
							Expect(JSToken.RightParen, "`)' expected");
							temp = Statement();
							stmt = new JFor(Context.BuildRange
												(save, temp.context),
											init, cond, incr, temp);
						}
						else
						{
							if(init is JCompound)
							{
								SyntaxError("only one variable name can be " +
											"used with for(var ... in ...)");
							}
							else if(init != null && !(init is JVarDecl))
							{
								CheckLeftHandSideExpression(init);
							}
							NextToken();
							cond = Expression();
							Expect(JSToken.RightParen, "`)' expected");
							temp = Statement();
							stmt = new JForIn(Context.BuildRange
												(save, temp.context),
											  init, cond, temp);
						}

					}
					break;

					case JSToken.Switch:
					{
						// Parse a "switch" statement.
						NextToken();
						temp = BracketedExpression();
						Expect(JSToken.LeftCurly, "`{' expected");
						stmt = CaseBlock();
						Expect(JSToken.RightCurly, "`}' expected");
						if(stmt != null)
						{
							stmt = new JSwitch(Context.BuildRange
													(save, stmt.context),
											   temp, stmt);
						}
						else
						{
							// No clauses, so evaluate the expression
							// for its side-effects only.
							stmt = new JExprStmt
								(temp.context.MakeCopy(), temp);
						}
					}
					break;

					case JSToken.Try:
					{
						// Parse a "try" statement.
						String catchName = null;
						JNode catchClause = null;
						JNode finallyClause = null;
						NextToken();
						stmt = Block();
						if(token == JSToken.Catch)
						{
							NextToken();
							Expect(JSToken.LeftParen, "`(' expected");
							if(token != JSToken.Identifier)
							{
								SyntaxError("identifier expected");
							}
							catchName = scanner.GetIdentifierName();
							NextToken();
							Expect(JSToken.RightParen, "`)' expected");
							catchClause = Block();
						}
						else if(token != JSToken.Finally)
						{
							SyntaxError("`catch' or `finally' expected");
						}
						if(token == JSToken.Finally)
						{
							NextToken();
							finallyClause = Block();
							save = Context.BuildRange
								(save, finallyClause.context);
						}
						else
						{
							save = Context.BuildRange
								(save, catchClause.context);
						}
						stmt = new JTry(save, stmt, catchName,
										catchClause, finallyClause);
					}
					break;

					default:
					{
						// Should be an expression.
						temp = StatementExpression();
						if(token == JSToken.Colon)
						{
							// The expression is followed by a colon,
							// so it is probably a label.
							if(!(temp is JIdentifier))
							{
								SyntaxError("invalid statement label");
							}
							NextToken();
							stmt = Statement();
							Support.AddLabel
								(((JIdentifier)temp).name, stmt);
							stmt.context = Context.BuildRange
								(save, stmt.context);
						}
						else
						{
							// Ordinary expression statement.
							stmt = new JExprStmt
								(Context.BuildRange(save, temp.context), temp);
							MatchSemiColon();
						}
					}
					break;
				}

				// Return the statement to the caller.
				return stmt;
			}

	// Parse a function declaration or expression.
	private JNode Function(bool needIdentifier)
			{
				Context save = tokenInfo.MakeCopy();
				String name = null;
				JFormalParams fparams;
				JNode body;

				// Skip the "function" keyword.
				NextToken();

				// Get the function name.
				if(token == JSToken.Identifier)
				{
					name = scanner.GetIdentifierName();
					NextToken();
				}
				else if(needIdentifier)
				{
					SyntaxError("function name expected");
				}

				// Parse the formal parameter list.
				fparams = new JFormalParams(tokenInfo.MakeCopy());
				Expect(JSToken.LeftParen, "`(' expected");
				while(token == JSToken.Identifier)
				{
					Support.AddExprToList
						(fparams, scanner.GetIdentifierName(), null);
					NextToken();
					if(token == JSToken.Comma)
					{
						NextToken();
					}
					else if(token != JSToken.RightParen)
					{
						SyntaxError("`)' or ',' expected");
					}
				}
				fparams.context = Context.BuildRange
						(fparams.context, tokenInfo);
				Expect(JSToken.RightParen, "`)' expected");

				// Parse the function body.
				body = Block();

				// Build the final function definition node.
				return new JFunction(Context.BuildRange(save, body.context),
									 name, fparams, body);
			}

	// Parse a function declaration.
	private JNode FunctionDeclaration()
			{
				return Function(true);
			}

	// Parse a function expression.
	private JNode FunctionExpression()
			{
				return Function(false);
			}

	// Parse a top-level source element.
	private JNode SourceElement(bool eval)
			{
				if(token == JSToken.Function)
				{
					if(eval)
					{
						SyntaxError
							("functions not permitted in eval expressions");
					}
					return FunctionDeclaration();
				}
				else
				{
					return Statement();
				}
			}

	// Parse the input source, and return a JNode for it.
	internal JNode ParseSource(bool eval)
			{
				JNode root = null;
				JNode functions = null;
				JNode element;
				Context start = context.MakeCopy();
				try
				{
					// Pre-fetch the first token.
					if(token == JSToken.None)
					{
						NextToken();
					}

					// Process source elements until we hit EOF or error.
					while(token != JSToken.EndOfFile)
					{
						element = SourceElement(eval);
						if(!eval && element is JFunction)
						{
							functions = Support.CreateCompound
								(functions, element);
						}
						else if(element != null)
						{
							root = Support.CreateCompound(root, element);
						}
					}
				}
				catch(JSScanner.ScannerFailure)
				{
					// The scanner detected an error in the input stream.
					throw new JScriptException(JSError.SyntaxError,
											   tokenInfo.MakeCopy());
				}
				catch(ErrorRecovery er)
				{
					// No error recovery done, so re-throw the syntax error.
					throw er.error;
				}
				if(root != null)
				{
					start = Context.BuildRange(start, root.context);
				}
				else
				{
					start = Context.BuildRange(start, tokenInfo);
				}
				if(eval)
				{
					return new JBlock(start, root);
				}
				else
				{
					return new JScriptBlock(start, functions, root);
				}
			}

	// Parse the input source to get a function definition.
	internal JFunction ParseFunctionSource()
			{
				JNode function;
				Context start = context.MakeCopy();
				try
				{
					// Pre-fetch the first token.
					if(token == JSToken.None)
					{
						NextToken();
					}

					// Parse the function definition.
					function = Function(false);

					// We need to be at EOF now.
					Expect(JSToken.EndOfFile, "end of file expected");
				}
				catch(JSScanner.ScannerFailure)
				{
					// The scanner detected an error in the input stream.
					throw new JScriptException(JSError.SyntaxError,
											   tokenInfo.MakeCopy());
				}
				catch(ErrorRecovery er)
				{
					// No error recovery done, so re-throw the syntax error.
					throw er.error;
				}
				return (JFunction)function;
			}

	// Exception that is used to handle error recovery in the parser.
	private sealed class ErrorRecovery : Exception
	{
		// Accessible internal state.
		public JScriptException error;

		// Constructor.
		public ErrorRecovery(JScriptException error)
				{
					this.error = error;
				}

	}; // class ErrorRecovery

}; // class JSParser

}; // namespace Microsoft.JScript
