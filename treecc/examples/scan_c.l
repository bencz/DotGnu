/*
 * scan_c.l - Input file for lex that defines the expression tokens.
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
#include "expr_c.h"
#include "gram_c.h"

extern YYSTYPE yylval;

extern char *filename;
extern long  linenum;

/*
 * Parse a floating point quantity into yylval.
 */
static void ParseFloat(char *text)
{
	double result;
	int num;
	if(sscanf(text, "%lf%n", &result, &num) <= 0)
	{
		/* Shouldn't happen, but do something reasonable */
		yylval.fnum = (float)0.0;
	}
	else
	{
		yylval.fnum = (float)result;
	}
}

/*
 * Parse an integer quantity into yylval.
 */
static void ParseInt(char *text)
{
	int value = 0;
	int digit;

	/* Parse the main parse of the value */
	if(*text == '0' && (text[1] == 'x' || text[1] == 'X'))
	{
		/* Hexadecimal integer constant */
		text += 2;
		while(*text != '\0')
		{
			if(*text >= 'A' && *text <= 'F')
			{
				digit = (int)(*text - 'A' + 10);
			}
			else if(*text >= 'a' && *text <= 'f')
			{
				digit = (int)(*text - 'a' + 10);
			}
			else if(*text >= '0' && *text <= '9')
			{
				digit = (int)(*text - '0');
			}
			else
			{
				break;
			}
			value = (value << 4) + digit;
			++text;
		}
	}
	else if(*text == '0')
	{
		/* Octal integer constant */
		while(*text != '\0')
		{
			if(*text >= '0' && *text <= '7')
			{
				digit = (int)(*text - '0');
			}
			else
			{
				break;
			}
			value = (value << 3) + digit;
			++text;
		}
	}
	else
	{
		/* Decimal integer constant */
		while(*text != '\0')
		{
			if(*text >= '0' && *text <= '9')
			{
				value = (value * 10) + (int)(*text - '0');
			}
			else
			{
				break;
			}
			++text;
		}
	}

	/* Set the return value */
	yylval.inum = value;
}

%}

%option outfile="lex.yy.c"
%option noyywrap
%option nounput

DIGIT			[0-9]
HEX				[a-fA-F0-9]
EXPONENT		[Ee][+-]?{DIGIT}+
WHITE			[ \t\r\f\v\032]

%%

"int"								{ return INT_TYPE; }
"float"								{ return FLOAT_TYPE; }

{DIGIT}+{EXPONENT}					{ ParseFloat(yytext); return FLOAT; }
{DIGIT}*"."{DIGIT}+({EXPONENT})?	{ ParseFloat(yytext); return FLOAT; }
{DIGIT}+"."{DIGIT}*({EXPONENT})?	{ ParseFloat(yytext); return FLOAT; }

0[xX]{HEX}+							{ ParseInt(yytext); return INT; }
{DIGIT}+							{ ParseInt(yytext); return INT; }

{WHITE}+							;

\n									{ ++linenum; }

.									{ return (((int)(yytext[0])) & 0xFF); }

%%
