/*
 * bf_rename.h - lex & yacc renaming of functions 
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
#ifndef	_CSCC_BF_RENAME_H
#define	_CSCC_BF_RENAME_H

/*
 * This list was extracted from the GNU automake documentation,
 * and is supposed to be reasonably complete for all known
 * yacc/lex implementations.
 */
#define yymaxdepth	bf_maxdepth
#define yyparse		bf_parse
#define yylex		bf_lex
#define yyerror		bf_error
#define yylval		bf_lval
#define yychar		bf_char
#define yydebug		bf_debug
#define yypact		bf_pact
#define yyr1		bf_r1
#define yyr2		bf_r2
#define yydef		bf_def
#define yychk		bf_chk
#define yypgo		bf_pgo
#define yyact		bf_act
#define yyexca		bf_exca
#define yyerrflag	bf_errflag
#define yynerrs     bf_nerrs
#define yyps		bf_ps
#define yypv		bf_pv
#define yys			bf_s
#define yy_yys		bf_yys
#define yystate		bf_state
#define yytmp		bf_tmp
#define yyv			bf_v
#define yy_yyv		bf_yyv
#define yyval		bf_val
#define yylloc		bf_lloc
#define yyreds		bf_reds
#define yytoks		bf_toks
#define yylhs		bf_yylhs
#define yylen		bf_yylen
#define yydefred	bf_yydefred
#define yydgoto		bf_yydgoto
#define yysindex	bf_yysindex
#define yyrindex	bf_yyrindex
#define yygindex	bf_yygindex
#define yytable     bf_yytable
#define yycheck     bf_yycheck
#define yyname		bf_yyname
#define yyrule		bf_yyrule

#endif	/* _CSCC_BF_RENAME_H */
