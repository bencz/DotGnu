/*
 * c_rename.h - Helper file for renaming yacc symbols for multiple parsers.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#ifndef	_CSCC_C_RENAME_H
#define	_CSCC_C_RENAME_H

/*
 * This list was extracted from the GNU automake documentation,
 * and is supposed to be reasonably complete for all known
 * yacc/lex implementations.
 */
#define yymaxdepth	c_maxdepth
#define yyparse		c_parse
#define yylex		c_lex
#define yyerror		c_error
#define yylval		c_lval
#define yychar		c_char
#define yydebug		c_debug
#define yypact		c_pact
#define yyr1		c_r1
#define yyr2		c_r2
#define yydef		c_def
#define yychk		c_chk
#define yypgo		c_pgo
#define yyact		c_act
#define yyexca		c_exca
#define yyerrflag	c_errflag
#define yynerrs     c_nerrs
#define yyps		c_ps
#define yypv		c_pv
#define yys			c_s
#define yy_yys		c_yys
#define yystate		c_state
#define yytmp		c_tmp
#define yyv			c_v
#define yy_yyv		c_yyv
#define yyval		c_val
#define yylloc		c_lloc
#define yyreds		c_reds
#define yytoks		c_toks
#define yylhs		c_yylhs
#define yylen		c_yylen
#define yydefred	c_yydefred
#define yydgoto		c_yydgoto
#define yysindex	c_yysindex
#define yyrindex	c_yyrindex
#define yygindex	c_yygindex
#define yytable     c_yytable
#define yycheck     c_yycheck
#define yyname		c_yyname
#define yyrule		c_yyrule

#endif	/* _CSCC_C_RENAME_H */
