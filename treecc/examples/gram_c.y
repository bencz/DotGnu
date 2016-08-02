/*
 * gram_c.y - Expression example grammar file for C.
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
#include <stdio.h>
#include <stdlib.h>

#include "expr_c.h"

extern char *filename;
extern long  linenum;

void yyerror(char *msg)
{
	fprintf(stderr, "%s:%ld: %s\n", filename, linenum, msg);
}

extern int yylex(void);

%}

%union {
    expression *node;
    int         inum;
    float       fnum;
}

%token INT FLOAT INT_TYPE FLOAT_TYPE

%type <node> pexpr uexpr mexpr expr
%type <inum> INT
%type <fnum> FLOAT

%start file
%%

/* Primary Expressions */
pexpr: INT              { $$ = intnum_create($1); }
     | FLOAT            { $$ = floatnum_create($1); }
     | '(' expr ')'     { $$ = $2; }
	 ;

/* Unary Expressions */
uexpr: pexpr			{ $$ = $1; }
     | '-' uexpr        { $$ = negate_create($2); }
	 | '(' INT_TYPE ')' uexpr {
	 			$$ = cast_create(int_type, $4);
	 		}
	 | '(' FLOAT_TYPE ')' uexpr {
	 			$$ = cast_create(float_type, $4);
	 		}
	 ;

/* Multiplicative Expressions */
mexpr: uexpr            { $$ = $1; }
     | mexpr '*' uexpr  { $$ = multiply_create($1, $3); }
     | mexpr '/' uexpr  { $$ = divide_create($1, $3); }
     | mexpr '^' uexpr  { $$ = power_create($1, $3); }
	 ;

/* Additive Expressions */
expr: mexpr             { $$ = $1; }
    | expr '+' mexpr    { $$ = plus_create($1, $3); }
    | expr '-' mexpr    { $$ = minus_create($1, $3); }
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
				value = eval_expr(e);

				/* Print the result to stdout */
				if(e->type == int_type)
				{
					printf("%d\n", value.int_value);
				}
				else
				{
					printf("%g\n", (double)(value.float_value));
				}
			}

			/* Reclaim the space used by the expression's nodes */
			yynodepop();
		}
    | error ';'         { /* recovers from syntax errors */ }
	;

file: stmt
    | file stmt
	;
