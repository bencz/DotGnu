%{
/*
 * bf_scanner.l - BF Scanner 
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V
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
%}

%{
/* Rename the lex/yacc symbols to support multiple parsers */
#include "bf_rename.h"
#include "bf_internal.h"

#include <stdio.h>
#include "bf_grammar.h"

/*
 * Get the next line of input from the C# pre-processor.
 */
#define	YY_INPUT(buf,result,maxSize)	\
	{ \
		result = CCPluginInput((buf), (maxSize)); \
	}

#define	RETURNTOK(x)		return (x)

%}

%option outfile="lex.yy.c"
%option prefix="bf_"
%option noyywrap
%option nounput
%%

"["					{ RETURNTOK(LOOP_START); }
"]"					{ RETURNTOK(LOOP_END); }
","					{ RETURNTOK(READ); }
"."					{ RETURNTOK(WRITE); }
"<"					{ RETURNTOK(PREV); }
">"					{ RETURNTOK(NEXT); }
"+"					{ RETURNTOK(PLUS); }
"-"					{ RETURNTOK(MINUS); }

\n					;

.					;

%%
