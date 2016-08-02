/*
 * vb_rename.h - Helper file for renaming yacc symbols for multiple parsers.
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

#ifndef	_CSCC_VB_RENAME_H
#define	_CSCC_VB_RENAME_H

/*
 * This list was extracted from the GNU automake documentation,
 * and is supposed to be reasonably complete for all known
 * yacc/lex implementations.
 */
#define yymaxdepth	vb_maxdepth
#define yyparse		vb_parse
#define yylex		vb_lex
#define yyerror		vb_error
#define yylval		vb_lval
#define yychar		vb_char
#define yydebug		vb_debug
#define yypact		vb_pact
#define yyr1		vb_r1
#define yyr2		vb_r2
#define yydef		vb_def
#define yychk		vb_chk
#define yypgo		vb_pgo
#define yyact		vb_act
#define yyexca		vb_exca
#define yyerrflag	vb_errflag
#define yynerrs     vb_nerrs
#define yyps		vb_ps
#define yypv		vb_pv
#define yys			vb_s
#define yy_yys		vb_yys
#define yystate		vb_state
#define yytmp		vb_tmp
#define yyv			vb_v
#define yy_yyv		vb_yyv
#define yyval		vb_val
#define yylloc		vb_lloc
#define yyreds		vb_reds
#define yytoks		vb_toks
#define yylhs		vb_yylhs
#define yylen		vb_yylen
#define yydefred	vb_yydefred
#define yydgoto		vb_yydgoto
#define yysindex	vb_yysindex
#define yyrindex	vb_yyrindex
#define yygindex	vb_yygindex
#define yytable     vb_yytable
#define yycheck     vb_yycheck
#define yyname		vb_yyname
#define yyrule		vb_yyrule

#endif	/* _CSCC_VB_RENAME_H */
