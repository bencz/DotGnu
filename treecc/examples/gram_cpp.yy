/*
 * gram_cpp.yy - Expression example grammar file for C++.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

%{
#include <iostream.h>
#include <stdio.h>
#include <stdlib.h>

#define	YYPARSE_PARAM	state
#define	YYLEX_PARAM		state

#include "expr_cpp.h"

void yyerror_print(NodeState *state, char *msg)
{
	cerr << state->currFilename() << ":" << state->currLinenum()
		 << ": " << msg << endl;
}

#define	yyerror(msg)	yyerror_print((NodeState *)state, (msg))

#define	STATE	((NodeState *)state)

%}

%pure_parser

%union {
    expression *node;
    int         inum;
    float       fnum;
}

%{

extern int yylex(YYSTYPE *yylval, void *state);

%}

%token INT FLOAT INT_TYPE FLOAT_TYPE

%type <node> pexpr uexpr mexpr expr
%type <inum> INT
%type <fnum> FLOAT

%start file
%%

/* Primary Expressions */
pexpr: INT              { $$ = STATE->intnumCreate($1); }
     | FLOAT            { $$ = STATE->floatnumCreate($1); }
     | '(' expr ')'     { $$ = $2; }
	 ;

/* Unary Expressions */
uexpr: pexpr			{ $$ = $1; }
     | '-' uexpr        { $$ = STATE->negateCreate($2); }
	 | '(' INT_TYPE ')' uexpr {
	 			$$ = STATE->castCreate(int_type, $4);
	 		}
	 | '(' FLOAT_TYPE ')' uexpr {
	 			$$ = STATE->castCreate(float_type, $4);
	 		}
	 ;

/* Multiplicative Expressions */
mexpr: uexpr            { $$ = $1; }
     | mexpr '*' uexpr  { $$ = STATE->multiplyCreate($1, $3); }
     | mexpr '/' uexpr  { $$ = STATE->divideCreate($1, $3); }
     | mexpr '^' uexpr  { $$ = STATE->powerCreate($1, $3); }
	 ;

/* Additive Expressions */
expr: mexpr             { $$ = $1; }
    | expr '+' mexpr    { $$ = STATE->plusCreate($1, $3); }
    | expr '-' mexpr    { $$ = STATE->minusCreate($1, $3); }
    ;

/* Expression Evaluation Statement */
stmt: expr ';'			{
			/* Perform type inferencing on the expression */
			expression *e = $1;
			infer_type(e);
			if(e->type != error_type)
			{
				/* Evaluate the expression */
				eval_value value;
				value = e->eval_expr();

				/* Print the result to stdout */
				if(e->type == int_type)
				{
					cout << value.int_value << endl;
				}
				else
				{
					cout << value.float_value << endl;
				}
			}

			/* Reclaim the space used by the expression's nodes */
			STATE->pop();
		}
    | error ';'         { /* recovers from syntax errors */ }
	;

file: stmt
    | file stmt
	;
