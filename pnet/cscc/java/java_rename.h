/*
 * java_rename.h - Helper file for renaming yacc symbols for multiple parsers.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Gopal.V
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

#ifndef	_CSCC_JAVA_RENAME_H
#define	_CSCC_JAVA_RENAME_H

/*
 * This list was extracted from the GNU automake documentation,
 * and is supposed to be reasonably complete for all known
 * yacc/lex implementations.
 */
#define yymaxdepth	java_maxdepth
#define yyparse		java_parse
#define yylex		java_lex
#define yyerror		java_error
#define yylval		java_lval
#define yychar		java_char
#define yydebug		java_debug
#define yypact		java_pact
#define yyr1		java_r1
#define yyr2		java_r2
#define yydef		java_def
#define yychk		java_chk
#define yypgo		java_pgo
#define yyact		java_act
#define yyexca		java_exca
#define yyerrflag	java_errflag
#define yynerrs     java_nerrs
#define yyps		java_ps
#define yypv		java_pv
#define yys			java_s
#define yy_yys		java_yys
#define yystate		java_state
#define yytmp		java_tmp
#define yyv			java_v
#define yy_yyv		java_yyv
#define yyval		java_val
#define yylloc		java_lloc
#define yyreds		java_reds
#define yytoks		java_toks
#define yylhs		java_yylhs
#define yylen		java_yylen
#define yydefred	java_yydefred
#define yydgoto		java_yydgoto
#define yysindex	java_yysindex
#define yyrindex	java_yyrindex
#define yygindex	java_yygindex
#define yytable     java_yytable
#define yycheck     java_yycheck
#define yyname		java_yyname
#define yyrule		java_yyrule

#endif	/* _CSCC_JAVA_RENAME_H */
