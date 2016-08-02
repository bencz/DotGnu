/*
 * cs_rename.h - Helper file for renaming yacc symbols for multiple parsers.
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

#ifndef	_CSCC_CS_RENAME_H
#define	_CSCC_CS_RENAME_H

/*
 * This list was extracted from the GNU automake documentation,
 * and is supposed to be reasonably complete for all known
 * yacc/lex implementations.
 */
#define yymaxdepth	cs_maxdepth
#define yyparse		cs_parse
#define yylex		cs_lex
#define yyerror		cs_error
#define yylval		cs_lval
#define yychar		cs_char
#define yydebug		cs_debug
#define yypact		cs_pact
#define yyr1		cs_r1
#define yyr2		cs_r2
#define yydef		cs_def
#define yychk		cs_chk
#define yypgo		cs_pgo
#define yyact		cs_act
#define yyexca		cs_exca
#define yyerrflag	cs_errflag
#define yynerrs     cs_nerrs
#define yyps		cs_ps
#define yypv		cs_pv
#define yys			cs_s
#define yy_yys		cs_yys
#define yystate		cs_state
#define yytmp		cs_tmp
#define yyv			cs_v
#define yy_yyv		cs_yyv
#define yyval		cs_val
#define yylloc		cs_lloc
#define yyreds		cs_reds
#define yytoks		cs_toks
#define yylhs		cs_yylhs
#define yylen		cs_yylen
#define yydefred	cs_yydefred
#define yydgoto		cs_yydgoto
#define yysindex	cs_yysindex
#define yyrindex	cs_yyrindex
#define yygindex	cs_yygindex
#define yytable     cs_yytable
#define yycheck     cs_yycheck
#define yyname		cs_yyname
#define yyrule		cs_yyrule

#endif	/* _CSCC_CS_RENAME_H */
