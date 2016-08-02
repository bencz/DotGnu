%{
/*
 * bf_grammar.y - BF grammar 
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

%token LOOP_START LOOP_END
%token PREV NEXT
%token PLUS MINUS
%token READ WRITE

%type <node> Instruction InstructionList 
%type <node> Arith Move IO Loop 

%{
#include <stdio.h>
#include "il_system.h"
#include "il_opcodes.h"
#include "il_meta.h"
#include "il_utils.h"

#include "bf_rename.h"
#include "bf_internal.h"

int yydebug;

/*
 * Global code generator object.
 */
ILGenInfo BFCodeGen;

/*
 * Imports from the lexical analyser.
 */

extern int yylex();


#ifdef YYTEXT_POINTER
extern char *bf_text;
#else
extern char bf_text[];
#endif

static void yyerror(char *msg)
{
	CCPluginParseError(msg, bf_text);
}
%}

%union	{
	ILNode	*node;
};

%start CompilationUnit

%%

CompilationUnit
	: /* empty */ {
		CCError("input file is empty");
	}
	|
	InstructionList {
		CCParseTree = ILNode_BFProgram_create($1);
	} 
	;

InstructionList
	:Instruction {
		$$ = ILNode_List_create();
		ILNode_List_Add($$,$1);
	}
	| InstructionList Instruction {
		$$ = $1;
		ILNode_List_Add($$,$2);
	}
	;

Loop
	: LOOP_START InstructionList LOOP_END {
		$$ = ILNode_BFLoop_create($2);
	}
	;
	
Arith
	: PLUS {
		$$ = ILNode_BFArith_create(1);
	}
	| MINUS{
		$$ = ILNode_BFArith_create(-1);
	}
	;

IO
	: READ{	
		$$ = ILNode_BFRead_create();
	}
	| WRITE{
		$$ = ILNode_BFWrite_create();
	}
	;

Move
	: NEXT {
		$$ = ILNode_BFMove_create(1);
	}
	| PREV {
		$$ = ILNode_BFMove_create(-1);
	}
	;
	

Instruction
	: Arith
	| Move
	| IO
	| Loop
	;
%%
