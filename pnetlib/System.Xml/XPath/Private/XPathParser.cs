// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 2 "XPathParser.jay"
/*
 * XPathParser.jay - Grammar for XPath
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
using System.Xml.XPath;
using System.Diagnostics;

#if CONFIG_XPATH

namespace System.Xml.XPath.Private
{
	internal class XPathParser
	{
		internal XPathParser()
		{
		}
		internal XPathExpression Parse(String path)
		{
			XPathTokenizer tokenizer = new XPathTokenizer(path);
			return (XPathExpression) yyparse(tokenizer);
		}
	
		private static void ThrowSyntaxException(Expression expr, 
													String message)
		{
			// TODO: I18n and column numbers for expression
			throw new XPathException(message, null);
		}

		private static void CheckArguments(FunctionCallExpression expr,
										   ArgumentList list, 
										   int minCount,
										   int maxCount)
		{
			bool valid = true;
			int count = 0;
			
			if(maxCount == 0)
			{
				valid = (list == null);
			}
			else
			{
				for(ArgumentList entry = list; entry != null; entry = entry.next)
				{
					count++;
				}
				valid = (count >= minCount) && (count <= maxCount);
			}

			if(!valid)
			{
				String errorMsg;
				if(maxCount == minCount)
				{
					errorMsg = 	String.Format(
						"{0} expects exactly {1} arguments, got {2} argument",
						expr.name, 
						maxCount,
						count);
				}
				else
				{
					errorMsg = 	String.Format(
						"{0} expects {1}-{2} arguments, got {3} argument",
						expr.name, 
						minCount,
						maxCount,
						count);
				}
				
				ThrowSyntaxException(expr, errorMsg); 
			}
			else
			{
				expr.argCount = count;
			}
		}
#line default

  /** error output stream.
      It should be changeable.
    */
#if CONFIG_SMALL_CONSOLE
  public System.IO.TextWriter ErrorOutput = System.IO.TextWriter.Null;
#else
  public System.IO.TextWriter ErrorOutput = System.Console.Out;
#endif

  /** simplified error message.
      @see <a href="#yyerror(java.lang.String, java.lang.String[])">yyerror</a>
    */
  public void yyerror (string message) {
    yyerror(message, null);
  }

  /** (syntax) error message.
      Can be overwritten to control message format.
      @param message text to be displayed.
      @param expected vector of acceptable tokens, if available.
    */
  public void yyerror (string message, string[] expected) {
    if ((expected != null) && (expected.Length  > 0)) {
      ErrorOutput.Write (message+", expecting");
      for (int n = 0; n < expected.Length; ++ n)
        ErrorOutput.Write (" "+expected[n]);
        ErrorOutput.WriteLine ();
    } else
      ErrorOutput.WriteLine (message);
  }

  /** debugging support, requires the package jay.yydebug.
      Set to null to suppress debugging messages.
    */
  protected yydebug.yyDebug debug;

  protected static  int yyFinal = 13;
  public static  string [] yyRule = {
    "$accept : Expr",
    "LocationPath : RelativeLocationPath",
    "LocationPath : AbsoluteLocationPath",
    "AbsoluteLocationPath : '/'",
    "AbsoluteLocationPath : '/' RelativeLocationPath",
    "AbsoluteLocationPath : AbbreviatedAbsoluteLocationPath",
    "RelativeLocationPath : Step",
    "RelativeLocationPath : RelativeLocationPath '/' Step",
    "RelativeLocationPath : AbbreviatedRelativeLocationPath",
    "Step : AxisTest OptPredicates",
    "Step : AbbreviatedStep",
    "AxisTest : AxisSpecifier QNAME",
    "AxisTest : AxisSpecifier WILDCARD",
    "AxisTest : AxisSpecifier WILDCARDNAME",
    "AxisTest : AxisSpecifier NODETYPE '(' OptLiteral ')'",
    "OptPredicates :",
    "OptPredicates : Predicate OptPredicates",
    "AxisSpecifier : AXISNAME",
    "AxisSpecifier : AbbreviatedAxisSpecifier",
    "OptLiteral :",
    "OptLiteral : LITERAL",
    "Predicate : '[' Expr ']'",
    "AbbreviatedAbsoluteLocationPath : RECURSIVE_DESCENT RelativeLocationPath",
    "AbbreviatedRelativeLocationPath : RelativeLocationPath RECURSIVE_DESCENT Step",
    "AbbreviatedStep : '.'",
    "AbbreviatedStep : PARENT_NODE",
    "AbbreviatedAxisSpecifier :",
    "AbbreviatedAxisSpecifier : '@'",
    "Expr : OrExpr",
    "PrimaryExpr : VariableReference",
    "PrimaryExpr : '(' Expr ')'",
    "PrimaryExpr : LITERAL",
    "PrimaryExpr : NUMBER",
    "PrimaryExpr : FunctionCall",
    "VariableReference : '$' QNAME",
    "FunctionCall : FUNCTIONNAME '(' OptArgumentList ')'",
    "OptArgumentList :",
    "OptArgumentList : ArgumentList",
    "ArgumentList : Argument",
    "ArgumentList : Argument ',' ArgumentList",
    "Argument : Expr",
    "UnionExpr : PathExpr",
    "UnionExpr : UnionExpr '|' PathExpr",
    "PathExpr : LocationPath",
    "PathExpr : FilterExpr",
    "PathExpr : FilterExpr '/' RelativeLocationPath",
    "PathExpr : FilterExpr RECURSIVE_DESCENT RelativeLocationPath",
    "FilterExpr : PrimaryExpr",
    "FilterExpr : FilterExpr Predicate",
    "OrExpr : AndExpr",
    "OrExpr : OrExpr OP_OR AndExpr",
    "AndExpr : EqualityExpr",
    "AndExpr : AndExpr OP_AND EqualityExpr",
    "EqualityExpr : RelationalExpr",
    "EqualityExpr : EqualityExpr OP_EQ RelationalExpr",
    "EqualityExpr : EqualityExpr OP_NE RelationalExpr",
    "RelationalExpr : AdditiveExpr",
    "RelationalExpr : RelationalExpr OP_LT AdditiveExpr",
    "RelationalExpr : RelationalExpr OP_GT AdditiveExpr",
    "RelationalExpr : RelationalExpr OP_LE AdditiveExpr",
    "RelationalExpr : RelationalExpr OP_GE AdditiveExpr",
    "AdditiveExpr : MultiplicativeExpr",
    "AdditiveExpr : AdditiveExpr OP_PLUS MultiplicativeExpr",
    "AdditiveExpr : AdditiveExpr OP_MINUS MultiplicativeExpr",
    "MultiplicativeExpr : UnaryExpr",
    "MultiplicativeExpr : MultiplicativeExpr OP_MUL UnaryExpr",
    "MultiplicativeExpr : MultiplicativeExpr OP_DIV UnaryExpr",
    "MultiplicativeExpr : MultiplicativeExpr OP_MOD UnaryExpr",
    "UnaryExpr : UnionExpr",
    "UnaryExpr : OP_MINUS UnaryExpr",
  };
  protected static  string [] yyNames = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,"'$'",null,null,
    null,"'('","')'","'*'","'+'","','","'-'","'.'","'/'",null,null,null,
    null,null,null,null,null,null,null,null,null,"'<'","'='","'>'",null,
    "'@'",null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    "'['",null,"']'",null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,"'|'",null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    "ERROR","EOF","AXISNAME","\"axisname\"","OP_OR","\"or\"","OP_AND",
    "\"and\"","OP_EQ","OP_NE","\"!=\"","OP_LT","OP_GT","OP_LE","\"<=\"",
    "OP_GE","\">=\"","OP_PLUS","OP_MINUS","OP_MUL","OP_DIV","\"div\"",
    "OP_MOD","\"mod\"","LITERAL","\"quoted-string\"","NUMBER",
    "\"number\"","QNAME","\"qualified name\"","WILDCARD","WILDCARDNAME",
    "\"NCName:*\"","FUNCTIONNAME","\"function-name\"","NODETYPE",
    "\"node type\"","PARENT_NODE","\"..\"","RECURSIVE_DESCENT","\"//\"",
  };

  /** index-checked interface to yyNames[].
      @param token single character or %token value.
      @return token name or [illegal] or [unknown].
    */
  public static string yyname (int token) {
    if ((token < 0) || (token > yyNames.Length)) return "[illegal]";
    string name;
    if ((name = yyNames[token]) != null) return name;
    return "[unknown]";
  }

  /** computes list of expected tokens on error by tracing the tables.
      @param state for which to compute the list.
      @return list of token names.
    */
  protected string[] yyExpecting (int state) {
    int token, n, len = 0;
    bool[] ok = new bool[yyNames.Length];

    if ((n = yySindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }
    if ((n = yyRindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }

    string [] result = new string[len];
    for (n = token = 0; n < len;  ++ token)
      if (ok[token]) result[n++] = yyNames[token];
    return result;
  }

  /** the generated parser, with debugging messages.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @param yydebug debug message writer implementing yyDebug, or null.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  public Object yyparse (yyParser.yyInput yyLex, Object yyd)
				 {
    this.debug = (yydebug.yyDebug)yyd;
    return yyparse(yyLex);
  }

  /** initial size and increment of the state/value stack [default 256].
      This is not final so that it can be overwritten outside of invocations
      of yyparse().
    */
  protected int yyMax;

  /** executed at the beginning of a reduce action.
      Used as $$ = yyDefault($1), prior to the user-specified action, if any.
      Can be overwritten to provide deep copy, etc.
      @param first value for $1, or null.
      @return first.
    */
  protected Object yyDefault (Object first) {
    return first;
  }

  /** the generated parser.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  public Object yyparse (yyParser.yyInput yyLex)
				{
    if (yyMax <= 0) yyMax = 256;			// initial size
    int yyState = 0;                                   // state stack ptr
    int [] yyStates = new int[yyMax];	                // state stack 
    Object yyVal = null;                               // value stack ptr
    Object [] yyVals = new Object[yyMax];	        // value stack
    int yyToken = -1;					// current input
    int yyErrorFlag = 0;				// #tks to shift

    int yyTop = 0;
    goto skip;
    yyLoop:
    yyTop++;
    skip:
    for (;; ++ yyTop) {
      if (yyTop >= yyStates.Length) {			// dynamically increase
        int[] i = new int[yyStates.Length+yyMax];
        yyStates.CopyTo (i, 0);
        yyStates = i;
        Object[] o = new Object[yyVals.Length+yyMax];
        yyVals.CopyTo (o, 0);
        yyVals = o;
      }
      yyStates[yyTop] = yyState;
      yyVals[yyTop] = yyVal;
      if (debug != null) debug.push(yyState, yyVal);

      yyDiscarded: for (;;) {	// discarding a token does not change stack
        int yyN;
        if ((yyN = yyDefRed[yyState]) == 0) {	// else [default] reduce (yyN)
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
            if (debug != null)
              debug.lex(yyState, yyToken, yyname(yyToken), yyLex.value());
          }
          if ((yyN = yySindex[yyState]) != 0 && ((yyN += yyToken) >= 0)
              && (yyN < yyTable.Length) && (yyCheck[yyN] == yyToken)) {
            if (debug != null)
              debug.shift(yyState, yyTable[yyN], yyErrorFlag-1);
            yyState = yyTable[yyN];		// shift to yyN
            yyVal = yyLex.value();
            yyToken = -1;
            if (yyErrorFlag > 0) -- yyErrorFlag;
            goto yyLoop;
          }
          if ((yyN = yyRindex[yyState]) != 0 && (yyN += yyToken) >= 0
              && yyN < yyTable.Length && yyCheck[yyN] == yyToken)
            yyN = yyTable[yyN];			// reduce (yyN)
          else
            switch (yyErrorFlag) {
  
            case 0:
              yyerror(String.Format ("syntax error, got token `{0}'", yyname (yyToken)), yyExpecting(yyState));
              if (debug != null) debug.error("syntax error");
              goto case 1;
            case 1: case 2:
              yyErrorFlag = 3;
              do {
                if ((yyN = yySindex[yyStates[yyTop]]) != 0
                    && (yyN += Token.yyErrorCode) >= 0 && yyN < yyTable.Length
                    && yyCheck[yyN] == Token.yyErrorCode) {
                  if (debug != null)
                    debug.shift(yyStates[yyTop], yyTable[yyN], 3);
                  yyState = yyTable[yyN];
                  yyVal = yyLex.value();
                  goto yyLoop;
                }
                if (debug != null) debug.pop(yyStates[yyTop]);
              } while (-- yyTop >= 0);
              if (debug != null) debug.reject();
              throw new yyParser.yyException("irrecoverable syntax error");
  
            case 3:
              if (yyToken == 0) {
                if (debug != null) debug.reject();
                throw new yyParser.yyException("irrecoverable syntax error at end-of-file");
              }
              if (debug != null)
                debug.discard(yyState, yyToken, yyname(yyToken),
  							yyLex.value());
              yyToken = -1;
              goto yyDiscarded;		// leave stack alone
            }
        }
        int yyV = yyTop + 1-yyLen[yyN];
        if (debug != null)
          debug.reduce(yyState, yyStates[yyV-1], yyN, yyRule[yyN], yyLen[yyN]);
        yyVal = yyDefault(yyV > yyTop ? null : yyVals[yyV]);
        switch (yyN) {
case 3:
#line 149 "XPathParser.jay"
  {
		yyVal = new RootPathExpression();
	}
  break;
case 4:
#line 153 "XPathParser.jay"
  {
		yyVal = new SlashExpression(new RootPathExpression(), (Expression) yyVals[0+yyTop]);
	}
  break;
case 7:
#line 163 "XPathParser.jay"
  {
		yyVal = new SlashExpression((Expression)yyVals[-2+yyTop] , (Expression)yyVals[0+yyTop]);
	}
  break;
case 9:
#line 172 "XPathParser.jay"
  {
		if(yyVals[0+yyTop] != null)
		{
			yyVal = new FilterExpression((Expression)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
		}
		else
		{
			yyVal = yyVals[-1+yyTop];
		}
	}
  break;
case 11:
#line 189 "XPathParser.jay"
  {
		yyVal = new NodeTest((XPathAxis)yyVals[-1+yyTop], 
						 	XPathNodeType.All, (XmlQualifiedName)yyVals[0+yyTop]);
	}
  break;
case 12:
#line 194 "XPathParser.jay"
  {
		yyVal = new NodeTest((XPathAxis)yyVals[-1+yyTop], 
						 	XPathNodeType.All, null);
	}
  break;
case 13:
#line 199 "XPathParser.jay"
  {
		yyVal = new NodeTest((XPathAxis)yyVals[-1+yyTop], 
						 	XPathNodeType.All, (XmlQualifiedName)yyVals[0+yyTop]);
	}
  break;
case 14:
#line 204 "XPathParser.jay"
  {
		XmlQualifiedName nodeName = null;
		if(yyVals[-1+yyTop] != null)
		{
			nodeName = new XmlQualifiedName((String)yyVals[-1+yyTop]);
		}
		if((XPathNodeType)yyVals[-3+yyTop] != XPathNodeType.ProcessingInstruction)
		{
			/* error */
		}
		
		yyVal = new NodeTest((XPathAxis)yyVals[-4+yyTop], 
						 	(XPathNodeType)yyVals[-3+yyTop], nodeName);
	}
  break;
case 15:
#line 223 "XPathParser.jay"
  {
		yyVal = null;
	}
  break;
case 16:
#line 227 "XPathParser.jay"
  {
		/* TODO predicate list */
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 17:
#line 236 "XPathParser.jay"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 19:
#line 244 "XPathParser.jay"
  {
		yyVal = null;
	}
  break;
case 21:
#line 253 "XPathParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	}
  break;
case 22:
#line 261 "XPathParser.jay"
  {
		yyVal = new RecursiveDescentPathExpression(new RootPathExpression(), 
												(Expression)yyVals[0+yyTop]); 
	}
  break;
case 23:
#line 270 "XPathParser.jay"
  {
		yyVal = new RecursiveDescentPathExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]); 
	}
  break;
case 24:
#line 278 "XPathParser.jay"
  {
		yyVal = new NodeTest(XPathAxis.Self, XPathNodeType.All, null);
	}
  break;
case 25:
#line 282 "XPathParser.jay"
  {
		yyVal = new NodeTest(XPathAxis.Parent, XPathNodeType.All, null);
	}
  break;
case 26:
#line 290 "XPathParser.jay"
  {
		yyVal = XPathAxis.Child;
	}
  break;
case 27:
#line 294 "XPathParser.jay"
  {
		yyVal = XPathAxis.Attribute;
	}
  break;
case 30:
#line 307 "XPathParser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 31:
#line 308 "XPathParser.jay"
  { yyVal = new LiteralExpression((String)yyVals[0+yyTop]); }
  break;
case 32:
#line 309 "XPathParser.jay"
  { yyVal = new NumberExpression((Double)yyVals[0+yyTop]); }
  break;
case 34:
#line 315 "XPathParser.jay"
  {
		/* TODO: neede for XSL */
		yyVal = null;
	}
  break;
case 35:
#line 324 "XPathParser.jay"
  {
		String name = (String)yyVals[-3+yyTop];
		FunctionCallExpression expr = null;
		ArgumentList args = (ArgumentList)yyVals[-1+yyTop];
		
		switch(name)
		{
			case "count":
			{
				expr = new XPathCountFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "id":
			{
				expr = new XPathIdFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "last":
			{
				expr = new XPathLastFunction(name, args);
				CheckArguments(expr, args, 0, 0);
			}
			break;
			
			case "local-name":
			{
				expr = new XPathLocalNameFunction(name, args);
				CheckArguments(expr, args, 0, 1);
			}
			break;
			
			case "name":
			{
				expr = new XPathNameFunction(name, args);
				CheckArguments(expr, args, 0, 1);
			}
			break;
			
			case "namespace-uri":
			{
				expr = new XPathNamespaceUriFunction(name, args);
				CheckArguments(expr, args, 0, 1);
			}
			break;
			
			case "position":
			{
				expr = new XPathPositionFunction(name, args);
				CheckArguments(expr, args, 0, 0);
			}
			break;
			
			case "concat":
			{
				expr = new XPathConcatFunction(name, args);
				CheckArguments(expr, args, 1, Int32.MaxValue);
			}
			break;
			
			case "contains":
			{
				expr = new XPathContainsFunction(name, args);
				CheckArguments(expr, args, 2, 2);
			}
			break;
			
			case "normalize-space":
			{
				expr = new XPathNormalizeFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "starts-with":
			{
				expr = new XPathStartsWithFunction(name, args);
				CheckArguments(expr, args, 2, 2);
			}
			break;
			
			case "string":
			{
				expr = new XPathStringFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "string-length":
			{
				expr = new XPathStringLengthFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "substring":
			{
				expr = new XPathSubstringFunction(name, args);
				CheckArguments(expr, args, 2, 2);
			}
			break;
			
			case "substring-after":
			{
				expr = new XPathSubstringAfterFunction(name, args);
				CheckArguments(expr, args, 2, 2);
			}
			break;
			
			case "substring-before":
			{
				expr = new XPathSubstringBeforeFunction(name, args);
				CheckArguments(expr, args, 2, 2);
			}
			break;
			
			case "translate":
			{
				expr = new XPathTranslateFunction(name, args);
				CheckArguments(expr, args, 3, 3);
			}
			break;
			
			case "ceiling":
			{
				expr = new XPathCeilingFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "floor":
			{
				expr = new XPathFloorFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "number":
			{
				expr = new XPathNumberFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "round":
			{
				expr = new XPathRoundFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "sum":
			{
				expr = new XPathSumFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "boolean":
			{
				expr = new XPathBooleanFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "false":
			{
				expr = new XPathFalseFunction(name, args);
				CheckArguments(expr, args, 0, 0);
			}
			break;
			
			case "lang":
			{
				expr = new XPathLangFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "not":
			{
				expr = new XPathNotFunction(name, args);
				CheckArguments(expr, args, 1, 1);
			}
			break;
			
			case "true":
			{
				expr = new XPathTrueFunction(name, args);
				CheckArguments(expr, args, 0, 0);
			}
			break;
			default:
			{
				expr = new XPathExternalFunction(name, args);
			}
			break;
		}

		yyVal = expr;
	}
  break;
case 36:
#line 532 "XPathParser.jay"
  {
		yyVal = null;
	}
  break;
case 38:
#line 541 "XPathParser.jay"
  {
		yyVal = new ArgumentList((Expression)yyVals[0+yyTop]);
	}
  break;
case 39:
#line 545 "XPathParser.jay"
  {
		ArgumentList list = new ArgumentList((Expression)yyVals[-2+yyTop]);
		list.next = (ArgumentList)yyVals[0+yyTop];
		yyVal = list;
	}
  break;
case 42:
#line 561 "XPathParser.jay"
  {
		yyVal = new UnionExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 45:
#line 571 "XPathParser.jay"
  {
		yyVal = new SlashExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[-1+yyTop]);
	}
  break;
case 46:
#line 575 "XPathParser.jay"
  {
		yyVal = null; /* TODO */
	}
  break;
case 48:
#line 584 "XPathParser.jay"
  {
		yyVal = new FilterExpression((Expression)yyVals[-1+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 52:
#line 599 "XPathParser.jay"
  {
		yyVal = new AndExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop]);
	}
  break;
case 54:
#line 608 "XPathParser.jay"
  {
		yyVal = new EqualityExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop], false); 
	}
  break;
case 55:
#line 612 "XPathParser.jay"
  {
		yyVal = new EqualityExpression((Expression)yyVals[-2+yyTop], (Expression)yyVals[0+yyTop], true);
	}
  break;
#line default
        }
        yyTop -= yyLen[yyN];
        yyState = yyStates[yyTop];
        int yyM = yyLhs[yyN];
        if (yyState == 0 && yyM == 0) {
          if (debug != null) debug.shift(0, yyFinal);
          yyState = yyFinal;
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
            if (debug != null)
               debug.lex(yyState, yyToken,yyname(yyToken), yyLex.value());
          }
          if (yyToken == 0) {
            if (debug != null) debug.accept(yyVal);
            return yyVal;
          }
          goto yyLoop;
        }
        if (((yyN = yyGindex[yyM]) != 0) && ((yyN += yyState) >= 0)
            && (yyN < yyTable.Length) && (yyCheck[yyN] == yyState))
          yyState = yyTable[yyN];
        else
          yyState = yyDgoto[yyM];
        if (debug != null) debug.shift(yyStates[yyTop], yyState);
	 goto yyLoop;
      }
    }
  }

   static  short [] yyLhs  = {              -1,
    1,    1,    3,    3,    3,    2,    2,    2,    5,    5,
    7,    7,    7,    7,    8,    8,   10,   10,   11,   11,
   12,    4,    6,    9,    9,   13,   13,    0,   15,   15,
   15,   15,   15,   16,   17,   18,   18,   19,   19,   20,
   21,   21,   22,   22,   22,   22,   23,   23,   14,   14,
   24,   24,   25,   25,   25,   26,   26,   26,   26,   26,
   27,   27,   27,   28,   28,   28,   28,   29,   29,
  };
   static  short [] yyLen = {           2,
    1,    1,    1,    2,    1,    1,    3,    1,    2,    1,
    2,    2,    2,    5,    0,    2,    1,    1,    0,    1,
    3,    2,    3,    1,    1,    0,    1,    1,    1,    3,
    1,    1,    1,    2,    4,    0,    1,    1,    3,    1,
    1,    3,    1,    1,    3,    3,    1,    2,    1,    3,
    1,    3,    1,    3,    3,    1,    3,    3,    3,    3,
    1,    3,    3,    1,    3,    3,    3,    1,    2,
  };
   static  short [] yyDefRed = {            0,
   17,    0,   31,   32,    0,   25,    0,    0,    0,   24,
   27,    0,    0,   43,    0,    2,    5,    6,    8,    0,
   10,    0,   18,    0,   47,   29,   33,    0,   41,    0,
    0,    0,    0,    0,    0,   64,   69,    0,    0,    0,
    0,   34,    0,    0,    0,    9,    0,   11,   12,   13,
    0,    0,    0,    0,    0,   48,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   40,    0,
   37,    0,   30,   23,    7,    0,   16,    0,    0,   42,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   65,   66,   67,   35,    0,   21,   20,    0,   39,
   14,
  };
  protected static  short [] yyDgoto  = {            69,
   14,   15,   16,   17,   18,   19,   20,   46,   21,   22,
   99,   47,   23,   24,   25,   26,   27,   70,   71,   72,
   28,   29,   30,   31,   32,   33,   34,   35,   36,
  };
  protected static  short [] yySindex = {          491,
    0,  491,    0,    0,  -40,    0,  -35,  -35,  491,    0,
    0, -276,    0,    0,  -39,    0,    0,    0,    0,  -70,
    0, -265,    0, -220,    0,    0,    0,  -80,    0,  -41,
 -214, -262, -236, -260, -202,    0,    0,  491,  -39,  -39,
   17,    0,  -35,  -35,  491,    0,  -70,    0,    0,    0,
   11,  491,  505,  -35,  -35,    0,  491,  491,  491,  491,
  491,  491,  491,  491,  491,  491,  491,  491,    0,   29,
    0,   32,    0,    0,    0,  -15,    0, -201, -214,    0,
  -39,  -39, -262, -236, -236, -260, -260, -260, -260, -202,
 -202,    0,    0,    0,    0,  491,    0,    0,   38,    0,
    0,
  };
  protected static  short [] yyRindex = {         -257,
    0, -257,    0,    0,    0,    0, -257,   24, -257,    0,
    0,    0,    0,    0,   52,    0,    0,    0,    0,    1,
    0,    0,    0,   12,    0,    0,    0,  154,    0,   69,
   13,    2,  473,  240,  175,    0,    0,  -36,   86,  103,
    0,    0, -257, -257, -257,    0,    1,    0,    0,    0,
    0, -257, -257, -257, -257,    0, -257, -257, -257, -257,
 -257, -257, -257, -257, -257, -257, -257, -257,    0,    0,
    0,   40,    0,    0,    0,    0,    0,   41,   18,    0,
  120,  137,   47,  480,  488,  393,  425,  446,  463,  190,
  209,    0,    0,    0,    0, -257,    0,    0,    0,    0,
    0,
  };
  protected static  short [] yyGindex = {           10,
    0,    9,    0,    0,  -18,    0,    0,   36,    0,    0,
    0,   54,    0,    0,    0,    0,    0,    0,  -11,    0,
    0,   34,    0,   37,   33,    8,  -23,   -4,    5,
  };
  protected static  short [] yyTable = {            38,
   15,   51,   58,   59,   36,   55,   37,   44,   42,   13,
   10,   28,   49,   64,   65,   39,   40,   50,   41,   48,
   45,   49,   50,    3,   74,   75,   51,   26,   11,   26,
   26,   60,   61,   62,   26,   63,   86,   87,   88,   89,
   52,   15,   51,   53,   15,   51,   52,   15,   57,   45,
   78,    1,   28,   49,   76,   28,   49,   73,   50,   90,
   91,   50,   81,   82,    3,   84,   85,    3,   44,   95,
   92,   93,   94,   66,   67,   96,   68,   97,  101,   98,
   38,   19,   77,   56,  100,   22,   80,   52,   79,   83,
   52,    0,    1,   15,   51,    1,    0,    0,    0,    0,
    0,    0,    4,    0,   28,   49,    0,    0,    0,   44,
   50,    0,   44,    0,    0,    0,    3,    0,    0,   46,
    0,    0,    0,    0,   15,    0,   22,    0,    0,   22,
    0,    0,    0,    0,    0,    0,   45,    0,    0,   52,
    0,    0,    0,    4,    1,    0,    4,    3,    0,    0,
    0,    0,    0,   68,    0,    0,    0,    0,    0,    0,
   46,   44,    0,   46,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   61,    1,    0,   45,   22,    0,
   45,    0,    0,    0,    0,    0,    0,    0,    0,   62,
    0,    0,   44,    0,   68,    4,    0,   68,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   63,   22,
    0,    0,   46,    0,    0,   61,    0,    0,   61,    0,
    0,    0,    0,    1,    0,    0,    4,    0,    0,   45,
   62,    0,    0,   62,    0,    0,    0,    0,    0,   56,
    0,    0,    0,   46,    0,    0,   68,    0,   26,   63,
   26,   26,   63,    0,   54,   26,   43,    0,    6,    0,
   45,   15,   51,   15,   51,   15,   15,   61,   15,   15,
   15,    0,   15,   49,   15,   15,   15,   15,   50,   15,
   56,    0,   62,   56,    3,    0,    3,    0,    3,    3,
    0,    3,    3,    3,    0,    3,   15,    3,    3,    3,
    3,   63,    3,    0,    0,    0,    0,   52,   26,   52,
   26,   26,    1,    0,    1,   26,    1,    1,    0,    1,
    1,    1,    0,    1,    0,    1,    1,    1,    1,   44,
    1,   44,   56,   44,   44,    0,   44,   44,   44,    0,
   44,    0,   44,   44,   44,   44,   22,   44,   22,    0,
   22,   22,    0,   22,   22,   22,    0,   22,    0,   22,
   22,   22,   22,    4,   22,    4,    0,    4,    4,    0,
    4,    4,    4,    0,    4,    0,    4,    4,    4,    4,
   46,    4,   46,    0,   46,   46,    0,   46,   46,   46,
    0,   46,   57,   46,   46,   46,   46,   45,   46,   45,
    0,   45,   45,    0,   45,   45,   45,    0,   45,    0,
   45,   45,   45,   45,   68,   45,   68,    0,   68,   68,
    0,   68,   68,   68,   58,   68,    0,   68,   68,   68,
   68,    0,   68,   57,    0,   61,   57,   61,    0,   61,
   61,    0,   61,   61,   61,   59,   61,    0,   61,   61,
   62,    0,   62,    0,   62,   62,    0,   62,   62,   62,
    0,   62,   60,   62,   62,   58,    0,    0,   58,   63,
    0,   63,   53,   63,   63,    0,   63,   63,   63,   54,
   63,    0,   63,   63,    0,   57,   59,   55,    0,   59,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   56,    0,   56,   60,   56,   56,   60,   56,   56,   56,
    0,   56,    0,   53,    0,    0,   53,   58,    0,    0,
   54,    0,    0,   54,    0,    0,   12,    0,   55,    0,
    9,   55,    0,    0,    0,    0,   10,    8,   59,    0,
   12,    0,    0,    0,    9,    0,    0,    0,    0,    0,
   10,    8,    0,    0,   11,   60,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   53,    0,    0,   11,    0,
    0,    0,   54,    0,    0,    0,    0,    0,    0,    0,
   55,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   57,    0,   57,    0,   57,   57,    0,
   57,   57,   57,    0,   57,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   58,    0,   58,    0,   58,
   58,    0,   58,   58,   58,    0,   58,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   59,    0,   59,    0,
   59,   59,    0,   59,   59,   59,    0,   59,    0,    0,
    0,    0,    0,   60,    0,   60,    0,   60,   60,    0,
   60,   60,   60,   53,   60,   53,    0,   53,   53,    0,
   54,    0,   54,    0,   54,   54,    0,    0,   55,    1,
   55,    0,   55,   55,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    1,    0,    2,    0,    0,    0,    0,
    0,    3,    0,    4,    0,    0,    0,    0,    0,    0,
    5,    0,    0,    0,    6,    3,    7,    4,    0,    0,
    0,    0,    0,    0,    5,    0,    0,    0,    6,    0,
    7,
  };
  protected static  short [] yyCheck = {            40,
    0,    0,  265,  266,   41,   47,    2,   47,  285,    0,
   46,    0,    0,  274,  275,    7,    8,    0,    9,  285,
   91,  287,  288,    0,   43,   44,  292,  285,   64,  287,
  288,  268,  269,  270,  292,  272,   60,   61,   62,   63,
  261,   41,   41,  124,   44,   44,    0,   47,  263,   91,
   40,    0,   41,   41,   45,   44,   44,   41,   41,   64,
   65,   44,   54,   55,   41,   58,   59,   44,    0,   41,
   66,   67,   68,  276,  277,   44,  279,   93,   41,  281,
   41,   41,   47,   30,   96,    0,   53,   41,   52,   57,
   44,   -1,   41,   93,   93,   44,   -1,   -1,   -1,   -1,
   -1,   -1,    0,   -1,   93,   93,   -1,   -1,   -1,   41,
   93,   -1,   44,   -1,   -1,   -1,   93,   -1,   -1,    0,
   -1,   -1,   -1,   -1,  124,   -1,   41,   -1,   -1,   44,
   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,   93,
   -1,   -1,   -1,   41,   93,   -1,   44,  124,   -1,   -1,
   -1,   -1,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   93,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,    0,  124,   -1,   41,   93,   -1,
   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,
   -1,   -1,  124,   -1,   41,   93,   -1,   44,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,  124,
   -1,   -1,   93,   -1,   -1,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,  259,   -1,   -1,  124,   -1,   -1,   93,
   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,    0,
   -1,   -1,   -1,  124,   -1,   -1,   93,   -1,  285,   41,
  287,  288,   44,   -1,  296,  292,  296,   -1,  294,   -1,
  124,  261,  261,  263,  263,  265,  266,   93,  268,  269,
  270,   -1,  272,  261,  274,  275,  276,  277,  261,  279,
   41,   -1,   93,   44,  261,   -1,  263,   -1,  265,  266,
   -1,  268,  269,  270,   -1,  272,  296,  274,  275,  276,
  277,   93,  279,   -1,   -1,   -1,   -1,  261,  285,  263,
  287,  288,  261,   -1,  263,  292,  265,  266,   -1,  268,
  269,  270,   -1,  272,   -1,  274,  275,  276,  277,  261,
  279,  263,   93,  265,  266,   -1,  268,  269,  270,   -1,
  272,   -1,  274,  275,  276,  277,  261,  279,  263,   -1,
  265,  266,   -1,  268,  269,  270,   -1,  272,   -1,  274,
  275,  276,  277,  261,  279,  263,   -1,  265,  266,   -1,
  268,  269,  270,   -1,  272,   -1,  274,  275,  276,  277,
  261,  279,  263,   -1,  265,  266,   -1,  268,  269,  270,
   -1,  272,    0,  274,  275,  276,  277,  261,  279,  263,
   -1,  265,  266,   -1,  268,  269,  270,   -1,  272,   -1,
  274,  275,  276,  277,  261,  279,  263,   -1,  265,  266,
   -1,  268,  269,  270,    0,  272,   -1,  274,  275,  276,
  277,   -1,  279,   41,   -1,  261,   44,  263,   -1,  265,
  266,   -1,  268,  269,  270,    0,  272,   -1,  274,  275,
  261,   -1,  263,   -1,  265,  266,   -1,  268,  269,  270,
   -1,  272,    0,  274,  275,   41,   -1,   -1,   44,  261,
   -1,  263,    0,  265,  266,   -1,  268,  269,  270,    0,
  272,   -1,  274,  275,   -1,   93,   41,    0,   -1,   44,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  261,   -1,  263,   41,  265,  266,   44,  268,  269,  270,
   -1,  272,   -1,   41,   -1,   -1,   44,   93,   -1,   -1,
   41,   -1,   -1,   44,   -1,   -1,   36,   -1,   41,   -1,
   40,   44,   -1,   -1,   -1,   -1,   46,   47,   93,   -1,
   36,   -1,   -1,   -1,   40,   -1,   -1,   -1,   -1,   -1,
   46,   47,   -1,   -1,   64,   93,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   64,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  261,   -1,  263,   -1,  265,  266,   -1,
  268,  269,  270,   -1,  272,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  261,   -1,  263,   -1,  265,
  266,   -1,  268,  269,  270,   -1,  272,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  261,   -1,  263,   -1,
  265,  266,   -1,  268,  269,  270,   -1,  272,   -1,   -1,
   -1,   -1,   -1,  261,   -1,  263,   -1,  265,  266,   -1,
  268,  269,  270,  261,  272,  263,   -1,  265,  266,   -1,
  261,   -1,  263,   -1,  265,  266,   -1,   -1,  261,  259,
  263,   -1,  265,  266,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  259,   -1,  275,   -1,   -1,   -1,   -1,
   -1,  281,   -1,  283,   -1,   -1,   -1,   -1,   -1,   -1,
  290,   -1,   -1,   -1,  294,  281,  296,  283,   -1,   -1,
   -1,   -1,   -1,   -1,  290,   -1,   -1,   -1,  294,   -1,
  296,
  };

#line 648 "XPathParser.jay"
}
#line default
namespace yydebug {
        using System;
	 public interface yyDebug {
		 void push (int state, Object value);
		 void lex (int state, int token, string name, Object value);
		 void shift (int from, int to, int errorFlag);
		 void pop (int state);
		 void discard (int state, int token, string name, Object value);
		 void reduce (int from, int to, int rule, string text, int len);
		 void shift (int from, int to);
		 void accept (Object value);
		 void error (string message);
		 void reject ();
	 }
	 
	 class yyDebugSimple : yyDebug {
		 void println (string s){
		 #if CONFIG_SMALL_CONSOLE
			 Console.WriteLine (s);
		 #else
			 Console.Error.WriteLine (s);
		 #endif
		 }
		 
		 public void push (int state, Object value) {
			 println ("push\tstate "+state+"\tvalue "+value);
		 }
		 
		 public void lex (int state, int token, string name, Object value) {
			 println("lex\tstate "+state+"\treading "+name+"\tvalue "+value);
		 }
		 
		 public void shift (int from, int to, int errorFlag) {
			 switch (errorFlag) {
			 default:				// normally
				 println("shift\tfrom state "+from+" to "+to);
				 break;
			 case 0: case 1: case 2:		// in error recovery
				 println("shift\tfrom state "+from+" to "+to
					     +"\t"+errorFlag+" left to recover");
				 break;
			 case 3:				// normally
				 println("shift\tfrom state "+from+" to "+to+"\ton error");
				 break;
			 }
		 }
		 
		 public void pop (int state) {
			 println("pop\tstate "+state+"\ton error");
		 }
		 
		 public void discard (int state, int token, string name, Object value) {
			 println("discard\tstate "+state+"\ttoken "+name+"\tvalue "+value);
		 }
		 
		 public void reduce (int from, int to, int rule, string text, int len) {
			 println("reduce\tstate "+from+"\tuncover "+to
				     +"\trule ("+rule+") "+text);
		 }
		 
		 public void shift (int from, int to) {
			 println("goto\tfrom state "+from+" to "+to);
		 }
		 
		 public void accept (Object value) {
			 println("accept\tvalue "+value);
		 }
		 
		 public void error (string message) {
			 println("error\t"+message);
		 }
		 
		 public void reject () {
			 println("reject");
		 }
		 
	 }
}
// %token constants
 class Token {
  public const int ERROR = 257;
  public const int EOF = 258;
  public const int AXISNAME = 259;
  public const int axisname = 260;
  public const int OP_OR = 261;
  public const int or = 262;
  public const int OP_AND = 263;
  public const int and = 264;
  public const int OP_EQ = 265;
  public const int OP_NE = 266;
  public const int OP_LT = 268;
  public const int OP_GT = 269;
  public const int OP_LE = 270;
  public const int OP_GE = 272;
  public const int OP_PLUS = 274;
  public const int OP_MINUS = 275;
  public const int OP_MUL = 276;
  public const int OP_DIV = 277;
  public const int div = 278;
  public const int OP_MOD = 279;
  public const int mod = 280;
  public const int LITERAL = 281;
  public const int NUMBER = 283;
  public const int number = 284;
  public const int QNAME = 285;
  public const int WILDCARD = 287;
  public const int WILDCARDNAME = 288;
  public const int FUNCTIONNAME = 290;
  public const int NODETYPE = 292;
  public const int PARENT_NODE = 294;
  public const int RECURSIVE_DESCENT = 296;
  public const int yyErrorCode = 256;
 }
 namespace yyParser {
  using System;
  /** thrown for irrecoverable syntax errors and stack overflow.
    */
  public class yyException : System.Exception {
    public yyException (string message) : base (message) {
    }
  }

  /** must be implemented by a scanner object to supply input to the parser.
    */
  public interface yyInput {
    /** move on to next token.
        @return false if positioned beyond tokens.
        @throws IOException on input error.
      */
    bool advance (); // throws java.io.IOException;
    /** classifies current token.
        Should not be called if advance() returned false.
        @return current %token or single character.
      */
    int token ();
    /** associated with current token.
        Should not be called if advance() returned false.
        @return value for token().
      */
    Object value ();
  }
 }
} // close outermost namespace, that MUST HAVE BEEN opened in the prolog

#endif // CONFIG_XPATH
