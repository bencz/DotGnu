/*
 * XPathTokenizer.cs - Scanner for XPath expressions
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Xml.XPath;
using System.Xml.XPath.Private;

#if CONFIG_XPATH

namespace System.Xml.XPath.Private
{
	public class XPathTokenizer : System.Xml.XPath.Private.yyParser.yyInput
	{
		private char[] data;
		private int currIndex;
		private int maxLength;
		private Object currValue;
		private int currToken; 
		private bool prevOperator;
		private bool inOperator;

		public XPathTokenizer(String xpath)
		{
			// a string reader would result in a cleaner design 
			// but array accesses are very fast compared to method calls
			data = xpath.ToCharArray();
			currIndex = -1;
			maxLength = data.Length;
			currValue = null;
			prevOperator = false;
			inOperator = false;
		}

		private static void Assert(bool condition)
		{
			#if !ECMA_COMPAT		
				Debug.Assert(condition);
			#endif
		}

		/* <Prefix>-<Suffix> format detection */
		private static bool IsSpecialTokenPrefix(String text)
		{
			return 
				/* Axes */
				(text == "processing" || 
				 text == "preceding"  ||
				 text == "following"  ||
				 text == "descendant" ||
				 text == "ancestor"   || 
				 text == "ancestor-or"|| 
				 text == "descendant-or")
				 
				 || 
				
				/* Functions */
				 (text == "local"     ||
				  text == "namespace" ||
				  text == "normalize" ||
				  text == "starts"    ||
				  text == "string"    ||
				  text == "substring");
		}
		
		private int StringToToken(String text)
		{
			switch(text)
			{
				case "and":
				{
					return Token.OP_AND;
				}
				case "or":
				{
					return Token.OP_OR;
				}
				case "div":
				{
					return Token.OP_DIV;
				}
				case "mod":
				{
					return Token.OP_MOD;
				}

				/* xpath axes */
				case "ancestor":
				{
					Value = XPathAxis.Ancestor;
					return Token.AXISNAME;
				}
				case "ancestor-or-self":
				{
					Value = XPathAxis.AncestorOrSelf;
					return Token.AXISNAME;
				}
				case "attribute":
				{
					Value = XPathAxis.Attribute;
					return Token.AXISNAME;
				}
				case "child":
				{
					Value = XPathAxis.Child;
					return Token.AXISNAME;
				}
				case "descendant":
				{
					Value = XPathAxis.Descendant;
					return Token.AXISNAME;
				}
				case "descendant-or-self":
				{
					Value = XPathAxis.DescendantOrSelf;
					return Token.AXISNAME;
				}
				break;
				case "following":
				{
					Value = XPathAxis.Following;
					return Token.AXISNAME;
				}
				case "following-sibling":
				{
					Value = XPathAxis.FollowingSibling;
					return Token.AXISNAME;
				}
				case "namespace":
				{
					Value = XPathAxis.Namespace;
					return Token.AXISNAME;
				}
				case "parent":
				{
					Value = XPathAxis.Parent;
					return Token.AXISNAME;
				}
				case "preceding":
				{
					Value = XPathAxis.Preceding;
					return Token.AXISNAME;
				}
				case "preceding-sibling":
				{
					Value = XPathAxis.PrecedingSibling;
					return Token.AXISNAME;
				}
				case "self":
				{
					Value = XPathAxis.Self;
					return Token.AXISNAME;
				}
				
				/* node types */
				case "comment":
				{
					Value = XPathNodeType.Comment;
					return Token.NODETYPE;
				}
				case "text":
				{
					Value = XPathNodeType.Text;
					return Token.NODETYPE;
				}
				case "processing-instruction":
				{
					Value = XPathNodeType.ProcessingInstruction;
					return Token.NODETYPE;
				}
				case "node":
				{
					Value = XPathNodeType.All;
					return Token.NODETYPE;
				}

				/* function names */

				case "count":
				case "id":
				case "last":
				case "local-name":
				case "name":
				case "namespace-uri":
				case "position":
				case "concat":
				case "contains":
				case "normalize-space":
				case "starts-with":
				case "string":
				case "string-length":
				case "substring":
				case "substring-before":
				case "substring-after":
				case "translate":
				case "ceiling":
				case "floor":
				case "number":
				case "round":
				case "sum":
				case "boolean":
				case "lang":
				case "true":
				case "false":
				case "not":
				{
					Value = text;
					return Token.FUNCTIONNAME;
				}

			}

			return -1;
		}
		
		private int Read()
		{
			if(maxLength <= (currIndex+1))
			{
				return -1;
			}
			currIndex++;
			return data[currIndex];
		}

		private int Peek()
		{
			if(maxLength <= (currIndex+1))
			{
				return -1;
			}
			return data[currIndex+1];
		}

	/*

   [28]    ExprToken            ::=    '(' | ')' | '[' | ']' | '.' | '..' |  
                                       '@' | ',' | '::'                      
                                       | NameTest                            
                                       | NodeType                            
                                       | Operator                            
                                       | FunctionName                        
                                       | AxisName                            
                                       | Literal                             
                                       | Number                              
                                       | VariableReference          */ 
		private int GetNext()
		{
			int ch = Peek();
			prevOperator = inOperator;
			inOperator = false;
			
			// skip whitespace
			while(Char.IsWhiteSpace((char)ch))
			{
				Assert(Read() == ch);
				ch = Peek();
			}

			switch(ch)
			{
				case -1:
				{
					return Token.EOF;
				}
				case '(':
				case '[':
				case '@':
				case ',':
				case '$':
				case '|':
				{
					inOperator = true;
					goto case ')'; // too bad we don't have fall throughs
				}
				case ')':
				case ']':
				{
					Assert(Read() == ch);
					return (int)ch;
				}

				case '.':
				{
					Assert(Read() == ch);
					if(Peek() == ch)
					{
						Assert(Read() == ch);
						return Token.PARENT_NODE;
					}
					else if (Char.IsDigit((char)Peek()))
					{
						return ReadNumber(ch);
					}
					return (int)'.';
				}

				/* [29] Literal */
				case '"':
				case '\'':
				{
					StringBuilder sb = new StringBuilder();
					Assert(Read() == ch);
					while(Peek() != ch)
					{
						// read till end quote
						sb.Append((char)Read());
					}
					Assert(Read() == ch);
					Value = sb.ToString();
					return Token.LITERAL;
				}

				case '+':
				{
					inOperator = true;
					Assert(Read() == ch);
					return Token.OP_PLUS;
				}

				case '-':
				{
					inOperator = true;
					Assert(Read() == ch);
					return Token.OP_MINUS;
				}
				
				case '=':
				{
					Assert(Read() == ch);
					inOperator = true;
					return Token.OP_EQ;
				}

				case '!':
				{
					Assert(Read() == ch);
					if(Peek() == '=')
					{						
						inOperator = true;
						Assert(Read() == '=');
						return Token.OP_NE;
					}
					return Token.ERROR;
				}

				case '<':
				case '>':
				{
					inOperator = true;
					Assert(Read() == ch);
					if(Peek() == '=')
					{
						Assert(Read() == '=');
						return (ch == '>') ? Token.OP_GE : Token.OP_LE;
					}
					else
					{
						return (ch == '>') ? Token.OP_GT : Token.OP_LT;
					}
				}
				break;

				case '/':
				{
					inOperator = true;
					Assert(Read() == ch);
					if(Peek() == '/')
					{
						Assert(Read() == '/');
						return Token.RECURSIVE_DESCENT;
					}
					return (int)ch;
				}

				case '*':
				{
					Assert(Read() == ch);
					if(currIndex != 0 && !prevOperator)
					{
						inOperator = true;
						return Token.OP_MUL;
					}
					Value = new XmlQualifiedName();
					return Token.WILDCARD;
				}

				default:
				{
					if(Char.IsDigit((char)ch))
					{
						Assert(Read() == ch);
						return ReadNumber(ch);
					}
					else if(Char.IsLetter((char)ch) || ch == '_')
					{
						Assert(Read() == ch);
						return ReadName(ch);
					}
				}
				break;
			}
			throw new Exception("Error at '" + (char)ch + "'");
		}

		private int ReadNumber(int lookahead)
		{
			StringBuilder sb = new StringBuilder();
			// lookahead is already read
			sb.Append((char)lookahead);
			while(Char.IsDigit((char)Peek()))
			{
				sb.Append(Read());
			}
			if(Peek() == '.')
			{
				if(lookahead == '.')
				{
					return Token.ERROR;
				}
				Assert(Read() == '.');
				while(Char.IsDigit((char)Peek()))
				{
					sb.Append(Read());
				}
			}
			Value = Double.Parse(sb.ToString());
			return Token.NUMBER;
		}

		private int ReadName(int lookahead)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append((char)lookahead);
			int ch = Peek();
			int token = -1;
			while(Char.IsLetter((Char)ch) || Char.IsDigit((Char)ch) 
					|| ch == '_' || 
					(ch == '-' && IsSpecialTokenPrefix(sb.ToString())))
			{
				Assert(Read() == ch);
				sb.Append((char)ch);
				ch = Peek();
			}

			token = StringToToken(sb.ToString());

			if(token != -1)
			{
				if((token == Token.FUNCTIONNAME || 
						token == Token.NODETYPE))
				{
					if(Peek() == '(')
					{
						return token;	
					}
				}
				else if(token != Token.AXISNAME) // axis name falls through
				{
					return token;
				}
			}

			if(Peek() == ':')
			{
				// QNAME | WILDCARD_NAME | AXISNAME
				Assert(Read() == ':');
				ch = Peek(); 
				if(token == Token.AXISNAME && ch == ':')
				{
					Assert(Read() == ':');
					inOperator = true;
					return token;
				}
				if(Char.IsLetter((Char)ch) || Char.IsDigit((Char)ch) 
					|| ch == '_')
				{
					String ns=sb.ToString();
					sb = new StringBuilder();
					do
					{
						Assert(Read() == ch);
						sb.Append((char)ch);
						ch = Peek();
					}while(Char.IsLetter((Char)ch) || Char.IsDigit((Char)ch)
							|| ch == '_');
					Value = new XmlQualifiedName(sb.ToString(), ns);
					return Token.QNAME;
				}
				else if(ch == '*')
				{
					Value = new XmlQualifiedName("*", sb.ToString());
					return Token.WILDCARDNAME;
				}
				else
				{
					// not a QName or WILDCARD_NAME
					return Token.ERROR;
				}
			}

			Value = new XmlQualifiedName(sb.ToString());
			
			return Token.QNAME;
		}

		public Object Value
		{
			get
			{
				return currValue;
			}
			set
			{
				// so that we can easily debug this
				// this is inlined by the VM
				currValue = value;
			}
		}

		public int CurrentToken
		{
			get
			{
				return currToken;
			}
		}
		

		public bool advance()
		{
			currToken = GetNext();
#if XPATH_DEBUG			
			Debug.Write(XPathParser.yyname(currToken) + " ");
			if(currToken == Token.EOF)
			{
				Debug.Write("\n");
			}
#endif			
			return (currToken != Token.EOF);
		}

		public Object value()
		{
			return Value;
		}

		public int token()
		{
			return CurrentToken;
		}

	}
}

#endif // CONFIG_XPATH
