%{
/*
 * c_scanner.l - Input file for lex that defines the C token syntax.
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

/* Rename the lex/yacc symbols to support multiple parsers */
#include "c_rename.h"

#include <stdio.h>
#include <cscc/c/c_internal.h>
#include <cscc/c/c_grammar.h>

extern YYSTYPE yylval;
#ifdef YYLTYPE
extern YYLTYPE yylloc;
#endif

/*
 * Get the next line of input from the pre-processor.
 */
#define	YY_INPUT(buf,result,maxSize)	\
	{ \
		result = CCPluginInput((buf), (maxSize)); \
	}

/*
 * Update the line number information and return a token code.
 */
#ifdef YYLTYPE
#define	RETURNTOK(x)		\
			do { \
				yylloc.first_line = yycurrlinenum(); \
				return (x); \
			} while (0)
#else
#define	RETURNTOK(x)		return (x)
#endif

%}

%option outfile="lex.yy.c"
%option prefix="c_"
%option noyywrap
%option nounput

DIGIT			[0-9]
IDALPHA			[a-zA-Z_$]
HEX				[a-fA-F0-9]
EXPONENT		[Ee][+-]?{DIGIT}+
IEXPONENT		[Ii][+-]?{DIGIT}+
FTYPE			(f|F|l|L)
ITYPE			(u|U|l|L)*
WHITE			[ \t\v\r\f]

%%

^{WHITE}*"#"{WHITE}+{DIGIT}+{WHITE}+["](\\.|[^\\"])*["].*\n	  {
			CLexLineDirective(yytext);
		}
^{WHITE}*"#line"{WHITE}+{DIGIT}+{WHITE}+["](\\.|[^\\"])*["].*\n {
			CLexLineDirective(yytext);
		}
^{WHITE}*"#pragma"[^\n]*\n		{ /* Ignore pragma directives */ }
^{WHITE}*"#ident"[^\n]*\n		{ /* Ignore ident directives */ }
^{WHITE}*"#using"[^\n]*\n		{ CLexUsingDirective(yytext); }
^{WHITE}*"#"{IDALPHA}({DIGIT}|{IDALPHA})*[^\n]*\n	{
			yytext[strlen(yytext) - 1] = '\0';
			CCWarningOnLine(yycurrfilename(), yycurrlinenum(),
							_("unknown directive %s"), yytext);
		}

"++"					{ RETURNTOK(INC_OP); }
"--"					{ RETURNTOK(DEC_OP); }
"<<"					{ RETURNTOK(LEFT_OP); }
">>"					{ RETURNTOK(RIGHT_OP); }
"<="					{ RETURNTOK(LE_OP); }
">="					{ RETURNTOK(GE_OP); }
"=="					{ RETURNTOK(EQ_OP); }
"!="					{ RETURNTOK(NE_OP); }
"&&"					{ RETURNTOK(AND_OP); }
"||"					{ RETURNTOK(OR_OP); }
"*="					{ RETURNTOK(MUL_ASSIGN_OP); }
"/="					{ RETURNTOK(DIV_ASSIGN_OP); }
"%="					{ RETURNTOK(MOD_ASSIGN_OP); }
"+="					{ RETURNTOK(ADD_ASSIGN_OP); }
"-="					{ RETURNTOK(SUB_ASSIGN_OP); }
"<<="					{ RETURNTOK(LEFT_ASSIGN_OP); }
">>="					{ RETURNTOK(RIGHT_ASSIGN_OP); }
"&="					{ RETURNTOK(AND_ASSIGN_OP); }
"^="					{ RETURNTOK(XOR_ASSIGN_OP); }
"|="					{ RETURNTOK(OR_ASSIGN_OP); }
"->"					{ RETURNTOK(PTR_OP); }
"::"					{ RETURNTOK(COLON_COLON_OP); }

"asm"					{ RETURNTOK(K_ASM); }
"__asm__"				{ RETURNTOK(K_ASM); }
"auto"					{ RETURNTOK(K_AUTO); }
"break"					{ RETURNTOK(K_BREAK); }
"case"					{ RETURNTOK(K_CASE); }
"char"					{ RETURNTOK(K_CHAR); }
"const"					{ RETURNTOK(K_CONST); }
"__const__"				{ RETURNTOK(K_CONST); }
"continue"				{ RETURNTOK(K_CONTINUE); }
"default"				{ RETURNTOK(K_DEFAULT); }
"do"					{ RETURNTOK(K_DO); }
"double"				{ RETURNTOK(K_DOUBLE); }
"else"					{ RETURNTOK(K_ELSE); }
"enum"					{ RETURNTOK(K_ENUM); }
"extern"				{ RETURNTOK(K_EXTERN); }
"float"					{ RETURNTOK(K_FLOAT); }
"for"					{ RETURNTOK(K_FOR); }
"goto"					{ RETURNTOK(K_GOTO); }
"if"					{ RETURNTOK(K_IF); }
"inline"				{ RETURNTOK(K_INLINE); }
"__inline__"			{ RETURNTOK(K_INLINE); }
"int"					{ RETURNTOK(K_INT); }
"long"					{ RETURNTOK(K_LONG); }
"register"				{ RETURNTOK(K_REGISTER); }
"return"				{ RETURNTOK(K_RETURN); }
"short"					{ RETURNTOK(K_SHORT); }
"signed"				{ RETURNTOK(K_SIGNED); }
"__signed__"			{ RETURNTOK(K_SIGNED); }
"sizeof"				{ RETURNTOK(K_SIZEOF); }
"static"				{ RETURNTOK(K_STATIC); }
"struct"				{ RETURNTOK(K_STRUCT); }
"switch"				{ RETURNTOK(K_SWITCH); }
"typedef"				{ RETURNTOK(K_TYPEDEF); }
"typeof"				{ RETURNTOK(K_TYPEOF); }
"__typeof__"			{ RETURNTOK(K_TYPEOF); }
"union"					{ RETURNTOK(K_UNION); }
"unsigned"				{ RETURNTOK(K_UNSIGNED); }
"void"					{ RETURNTOK(K_VOID); }
"volatile"				{ RETURNTOK(K_VOLATILE); }
"__volatile__"			{ RETURNTOK(K_VOLATILE); }
"while"					{ RETURNTOK(K_WHILE); }

"..."					{ RETURNTOK(K_ELIPSIS); }
"__builtin_va_list"		{ RETURNTOK(K_VA_LIST); }
"__builtin_va_start"	{ RETURNTOK(K_VA_START); }
"__builtin_va_arg"		{ RETURNTOK(K_VA_ARG); }
"__builtin_va_end"		{ RETURNTOK(K_VA_END); }
"__builtin_setjmp"		{ RETURNTOK(K_SETJMP); }
"__builtin_alloca"		{ RETURNTOK(K_ALLOCA); }
"__attribute__"			{ RETURNTOK(K_ATTRIBUTE); }

"_Bool"					{ RETURNTOK(K_BOOL); }
"__wchar__"				{ RETURNTOK(K_WCHAR); }
"__unsigned_int__"		{ RETURNTOK(K_UINT); }

"__FUNCTION__"			{ RETURNTOK(K_FUNCTION); }
"__PRETTY_FUNCTION__"	{ RETURNTOK(K_FUNCTION); }
"__func__"				{ RETURNTOK(K_FUNC); }

"__try__"				{ RETURNTOK(K_TRY); /* Counterparts to C# keywords */ }
"__catch__"				{ RETURNTOK(K_CATCH); }
"__finally__"			{ RETURNTOK(K_FINALLY); }
"__throw__"				{ RETURNTOK(K_THROW); }
"__checked__"			{ RETURNTOK(K_CHECKED); }
"__unchecked__"			{ RETURNTOK(K_UNCHECKED); }
"__null__"				{ RETURNTOK(K_NULL); }
"__true__"				{ RETURNTOK(K_TRUE); }
"__false__"				{ RETURNTOK(K_FALSE); }
"__lock__"				{ RETURNTOK(K_LOCK); }
"__using__"				{ RETURNTOK(K_USING); }
"__namespace__"			{ RETURNTOK(K_NAMESPACE); }
"__new__"				{ RETURNTOK(K_NEW); }
"__delete__"			{ RETURNTOK(K_DELETE); }

"__typeof"				{ RETURNTOK(K_CS_TYPEOF); /* Managed C++ compat */ }
"__int64"				{ RETURNTOK(K_INT64); }
"__finally"				{ RETURNTOK(K_FINALLY); }
"__box"					{ RETURNTOK(K_BOX); }
"__declspec"			{ RETURNTOK(K_DECLSPEC); }
"__gc"					{ RETURNTOK(K_GC); }
"__nogc"				{ RETURNTOK(K_NOGC); }

\"(\\.|[^\\"])*\"				{ CLexParseString(yytext, &(yylval.string));
								  RETURNTOK(STRING_LITERAL); }

L\"(\\.|[^\\"])*\"				{ CLexParseString(yytext, &(yylval.string));
								  RETURNTOK(WSTRING_LITERAL); }

l\"(\\.|[^\\"])*\"				{ CLexParseString(yytext, &(yylval.string));
								  RETURNTOK(WSTRING_LITERAL); }

S\"(\\.|[^\\"])*\"				{ CLexParseString(yytext, &(yylval.string));
								  RETURNTOK(CS_STRING_LITERAL); }

s\"(\\.|[^\\"])*\"				{ CLexParseString(yytext, &(yylval.string));
								  RETURNTOK(CS_STRING_LITERAL); }

'(\\.|[^\\'])+'					{ CLexParseChar(yytext, &(yylval.integer));
								  RETURNTOK(INTEGER_CONSTANT); }

L'(\\.|[^\\'])+'				{ CLexParseChar(yytext, &(yylval.integer));
								  RETURNTOK(INTEGER_CONSTANT); }

l'(\\.|[^\\'])+'				{ CLexParseChar(yytext, &(yylval.integer));
								  RETURNTOK(INTEGER_CONSTANT); }

{IDALPHA}({DIGIT}|{IDALPHA})*	{
			/* Use the scope to determine if this is a typedef'ed name */
			yylval.name = ILInternString(yytext, strlen(yytext)).string;
			if(CScopeIsTypedef(yylval.name))
			{
				RETURNTOK(TYPE_NAME);
			}
			else if(CScopeIsNamespace(yylval.name))
			{
				RETURNTOK(NAMESPACE_NAME);
			}
			else
			{
				RETURNTOK(IDENTIFIER);
			}
		}

{DIGIT}+{EXPONENT}{FTYPE}?		{ CLexParseFloat(yytext, &(yylval.real));
								  RETURNTOK(FLOAT_CONSTANT); }
{DIGIT}*"."{DIGIT}+({EXPONENT})?{FTYPE}?	{
			CLexParseFloat(yytext, &(yylval.real));
			RETURNTOK(FLOAT_CONSTANT);
		}
{DIGIT}+"."{DIGIT}*({EXPONENT})?{FTYPE}?	{
			CLexParseFloat(yytext, &(yylval.real));
			RETURNTOK(FLOAT_CONSTANT);
		}

{DIGIT}+{IEXPONENT}{FTYPE}?		{ CLexParseFloat(yytext, &(yylval.real));
								  RETURNTOK(IMAG_CONSTANT); }
{DIGIT}*"."{DIGIT}+{IEXPONENT}{FTYPE}?	{
			CLexParseFloat(yytext, &(yylval.real));
			RETURNTOK(IMAG_CONSTANT);
		}
{DIGIT}+"."{DIGIT}*{IEXPONENT}{FTYPE}?	{
			CLexParseFloat(yytext, &(yylval.real));
			RETURNTOK(IMAG_CONSTANT);
		}

0[xX]{HEX}+{ITYPE}?				{ CLexParseInt(yytext, &(yylval.integer));
								  RETURNTOK(INTEGER_CONSTANT); }
0[0-7]*{ITYPE}?					{ CLexParseInt(yytext, &(yylval.integer));
								  RETURNTOK(INTEGER_CONSTANT); }
{DIGIT}+{ITYPE}?				{ CLexParseInt(yytext, &(yylval.integer));
								  RETURNTOK(INTEGER_CONSTANT); }

{WHITE}+						;

\n								;

.								{ RETURNTOK(((int)(yytext[0])) & 0xFF); }

%%
