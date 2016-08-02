/*
 * ilasm_rename.h - Helper file for renaming yacc symbols for multiple parsers.
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

#ifndef	__ILASM_RENAME_H__
#define	__ILASM_RENAME_H__

/*
 * This list was extracted from the GNU automake documentation,
 * and is supposed to be reasonably complete for all known
 * yacc/lex implementations.
 */
#define yymaxdepth	ilasm_maxdepth
#define yyparse		ilasm_parse
#define yylex		ilasm_lex
#define yyerror		ilasm_error
#define yylval		ilasm_lval
#define yychar		ilasm_char
#define yydebug		ilasm_debug
#define yypact		ilasm_pact
#define yyr1		ilasm_r1
#define yyr2		ilasm_r2
#define yydef		ilasm_def
#define yychk		ilasm_chk
#define yypgo		ilasm_pgo
#define yyact		ilasm_act
#define yyexca		ilasm_exca
#define yyerrflag	ilasm_errflag
#define yynerrs     ilasm_nerrs
#define yyps		ilasm_ps
#define yypv		ilasm_pv
#define yys			ilasm_s
#define yy_yys		ilasm_yys
#define yystate		ilasm_state
#define yytmp		ilasm_tmp
#define yyv			ilasm_v
#define yy_yyv		ilasm_yyv
#define yyval		ilasm_val
#define yylloc		ilasm_lloc
#define yyreds		ilasm_reds
#define yytoks		ilasm_toks
#define yylhs		ilasm_yylhs
#define yylen		ilasm_yylen
#define yydefred	ilasm_yydefred
#define yydgoto		ilasm_yydgoto
#define yysindex	ilasm_yysindex
#define yyrindex	ilasm_yyrindex
#define yygindex	ilasm_yygindex
#define yytable     ilasm_yytable
#define yycheck     ilasm_yycheck
#define yyname		ilasm_yyname
#define yyrule		ilasm_yyrule

#endif	/* __ILASM_RENAME_H__ */
