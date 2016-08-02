%{
/*
 * vb_scanner.l - Input file for lex that defines the VB token syntax.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
#include "vb_rename.h"

#include <stdio.h>
#include <cscc/vb/vb_internal.h>
#include <cscc/vb/vb_grammar.h>

extern YYSTYPE yylval;
#ifdef YYLTYPE
extern YYLTYPE yylloc;
#endif

int VBPrevWasEOL = 1;

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
				VBPrevWasEOL = 0;  \
				return (x); \
			} while (0)
#define	RETURNTOKNORESET(x)		\
			do { \
				yylloc.first_line = yycurrlinenum(); \
				return (x); \
			} while (0)
#else
#define	RETURNTOK(x)		\
			do { \
				VBPrevWasEOL = 0;  \
				return (x); \
			} while (0)
#define	RETURNTOKNORESET(x)		return (x)
#endif

/*
 * Parse a floating point quantity into yylval.
 */
static int VBParseFloat(char *text)
{
	char *endptr;

#ifdef HAVE_STRTOD
	/* Use "strtod" to parse the value */
	yylval.real.value = (ILDouble)(strtod(text, &endptr));
#else
	/* Use "sscanf" to parse the value */
	double result;
	int num;
	if(sscanf(text, "%lf%n", &result, &num) <= 0)
	{
		/* Shouldn't happen, but do something reasonable */
		yylval.real.value = (ILDouble)0.0;
		yylval.real.type = CS_NUMTYPE_FLOAT64;
		return FLOAT_CONSTANT;
	}
	yylval.real.value = (ILDouble)result;
	endptr = text + num;
#endif

	/* Process the floating point suffix, if present */
	if(*endptr == 'f' || *endptr == 'F')
	{
		yylval.real.type = CS_NUMTYPE_FLOAT32;
		return FLOAT_CONSTANT;
	}
	else if(*endptr == 'd' || *endptr == 'D')
	{
		/* Decimal values cannot be represented by the ILDouble
		   type, so we use a different conversion routine */
		if(!ILDecimalParse(&(yylval.decimal), text, IL_DECIMAL_ROUND_HALF_EVEN))
		{
			CCError(_("decimal constant is out of range"));
			ILDecimalFromInt32(&(yylval.decimal), 0);
		}
		return DECIMAL_CONSTANT;
	}
	else
	{
		yylval.real.type = CS_NUMTYPE_FLOAT64;
		return FLOAT_CONSTANT;
	}
}

/*
 * Parse an integer value into "yylval".
 */
static void VBParseInt(char *text)
{
	ILInt64 value = 0;
	int prefixed;
	int wentNeg = 0;
	int outOfRange;
	int ch;
	int isShort, isLong, isInt;
	int numDigits = 0;

	/* Parse the main part of the value */
	if(*text == '&' && (*text == 'H' || *text == 'h'))
	{
		/* Hexadecimal constant value */
		prefixed = 1;
		text += 2;
		while(*text != '\0')
		{
			ch = *text;
			if(ch >= '0' && ch <= '9')
			{
				value = value * 16 + (ILInt64)(ch - '0');
			}
			else if(ch >= 'A' && ch <= 'F')
			{
				value = value * 16 + (ILInt64)(ch - 'A' + 10);
			}
			else if(ch >= 'a' && ch <= 'f')
			{
				value = value * 16 + (ILInt64)(ch - 'a' + 10);
			}
			else
			{
				break;
			}
			++numDigits;
			if(value < 0)
			{
				wentNeg = 1;
			}
			++text;
		}
	}
	else if(*text == '&' && (*text == 'O' || *text == 'o'))
	{
		/* Octal constant value */
		prefixed = 1;
		text += 2;
		while(*text >= '0' && *text <= '7')
		{
			value = value * 8 + (ILInt64)(*text - '0');
			if(value < 0)
			{
				wentNeg = 1;
			}
			++numDigits;
			++text;
		}
	}
	else
	{
		/* Decimal constant value */
		prefixed = 0;
		while(*text >= '0' && *text <= '9')
		{
			value = value * 10 + (ILInt64)(*text - '0');
			if(value < 0)
			{
				wentNeg = 1;
			}
			++numDigits;
			++text;
		}
	}
	if(wentNeg && !prefixed)
	{
		CCError(_("integer constant is too large"));
	}

	/* Parse the integer type suffixes */
	isShort = 0;
	isInt = 0;
	isLong = 0;
	while((ch = *text++) != '\0')
	{
		if(ch == 's' || ch == 'S')
		{
			isShort = 1;
		}
		else if(ch == 'i' || ch == 'I')
		{
			isInt = 1;
		}
		else if(ch == 'l' || ch == 'L')
		{
			isLong = 1;
		}
	}
	if((isShort + isInt + isLong) > 1)
	{
		CCError(_("multiple integer type suffixes supplied"));
		isShort = 0;
		isInt = 0;
		isLong = 0;
	}

	/* Determine the type of the final value */
	outOfRange = 0;
	yylval.integer.numDigits = numDigits;
	if(isShort)
	{
		if(prefixed)
		{
			if(value < 0 || value > 65535)
			{
				outOfRange = 1;
			}
		}
		else
		{
			if(value < 0 || value > 32767)
			{
				outOfRange = 1;
			}
		}
		yylval.integer.value = (ILInt64)(ILInt16)value;
		yylval.integer.type = CS_NUMTYPE_INT32;
	}
	else if(isInt)
	{
		if(prefixed)
		{
			if(value < 0 || value > (ILInt64)IL_MAX_UINT32)
			{
				outOfRange = 1;
			}
		}
		else
		{
			if(value < 0 || value > (ILInt64)IL_MAX_INT32)
			{
				outOfRange = 1;
			}
		}
		yylval.integer.value = (ILInt64)(ILInt32)value;
		yylval.integer.type = CS_NUMTYPE_INT32;
	}
	else if(isLong)
	{
		yylval.integer.value = value;
		yylval.integer.type = CS_NUMTYPE_INT64;
	}
	else if(value >= 0 && value <= (ILInt64)IL_MAX_INT32)
	{
		yylval.integer.value = (ILInt64)(ILInt32)value;
		yylval.integer.type = CS_NUMTYPE_INT32;
	}
	else
	{
		yylval.integer.value = value;
		yylval.integer.type = CS_NUMTYPE_INT64;
	}
	if(outOfRange)
	{
		CCError(_("integer value is out of range"));
	}
}

/*
 * Parse a string value.
 */
static void VBParseString(char *text, ILIntString *str)
{
	int posn, outposn;
	char *buf;

	/* TODO: Latin-1 vs UTF-8 issues */

	/* If it does not have embedded '""' sequences, then this is easy */
	posn = 1;
	while(text[posn] != '"')
	{
		++posn;
	}
	if(text[posn + 1] == '\0' || text[posn + 1] == 'C' ||
	   text[posn + 1] == 'c')
	{
		*str = ILInternString(text + 1, posn - 1);
		return;
	}

	/* Allocate a temporary buffer and expand '""' sequences */
	buf = (char *)ILMalloc(strlen(text) + 1);
	if(!buf)
	{
		CCOutOfMemory();
	}
	outposn = 0;
	posn = 1;
	for(;;)
	{
		if(text[posn] == '"')
		{
			++posn;
			if(text[posn] == '\0' || text[posn] == 'C' || text[posn] == 'c')
			{
				break;
			}
		}
		buf[outposn++] = text[posn++];
	}
	*str = ILInternString(buf, outposn);
	ILFree(buf);
}

/*
 * Parse a character value into yylval.
 */
static void VBParseChar(char *text)
{
	ILIntString str;
	int posn;
	unsigned long ch;

	/* Parse the entire string */
	VBParseString(text, &str);

	/* Bail out with an error if the string is empty */
	if(str.len == 0)
	{
		CCError(_("empty character constant"));
		yylval.integer.value = 0;
		yylval.integer.type = CS_NUMTYPE_INT32;
		return;
	}

	/* Parse the first UTF-8 character from the string */
	posn = 0;
	ch = ILUTF8ReadChar(str.string, str.len, &posn);
	if(ch > 0xFFFF)
	{
		CCError(_("character constant is not 16-bit"));
	}
	yylval.integer.value = (ILInt64)(ch & 0xFFFF);
	yylval.integer.type = CS_NUMTYPE_INT32;
	if(posn != str.len)
	{
		CCError(_("multi-character constant"));
	}
}

%}

%option outfile="lex.yy.c"
%option prefix="vb_"
%option noyywrap
%option nounput
%option caseless

DIGIT			[0-9]
IDALPHA			[a-zA-Z_]
HEX				[a-fA-F0-9]
OCTAL			[0-7]
EXPONENT		[Ee][+-]?{DIGIT}+
FTYPE			(f|F|r|R|d|D)
ITYPE			(s|S|i|I|l|L)*
WHITE			[ \t\v\r\f]
TYPECHAR		[%&@!#$]

%%

"&="					{ RETURNTOK(CONCAT_ASSIGN_OP); }
"*="					{ RETURNTOK(MUL_ASSIGN_OP); }
"/="					{ RETURNTOK(DIV_ASSIGN_OP); }
"\\="					{ RETURNTOK(IDIV_ASSIGN_OP); }
"^="					{ RETURNTOK(POW_ASSIGN_OP); }
"+="					{ RETURNTOK(ADD_ASSIGN_OP); }
"-="					{ RETURNTOK(SUB_ASSIGN_OP); }
"<>"					{ RETURNTOK(NE_OP); }
"<="					{ RETURNTOK(LE_OP); }
"=<"					{ RETURNTOK(LE_OP); }
">="					{ RETURNTOK(GE_OP); }
"=="					{ RETURNTOK(EQ_OP); }
"<<"					{ RETURNTOK(LEFT_OP); }
">>"					{ RETURNTOK(RIGHT_OP); }

"AddHandler"			{ RETURNTOK(K_ADDHANDLER); }
"AddressOf"				{ RETURNTOK(K_ADDRESSOF); }
"Alias"					{ RETURNTOK(K_ALIAS); }
"And"					{ RETURNTOK(K_AND); }
"AndAlso"				{ RETURNTOK(K_ANDALSO); }
"Ansi"					{ RETURNTOK(K_ANSI); }
"As"					{ RETURNTOK(K_AS); }
"Assembly"				{ RETURNTOK(K_ASSEMBLY); }
"Auto"					{ RETURNTOK(K_AUTO); }
"Boolean"				{ RETURNTOK(K_BOOLEAN); }
"ByRef"					{ RETURNTOK(K_BYREF); }
"Byte"					{ RETURNTOK(K_BYTE); }
"ByVal"					{ RETURNTOK(K_BYVAL); }
"Call"					{ RETURNTOK(K_CALL); }
"Case"					{ RETURNTOK(K_CASE); }
"Catch"					{ RETURNTOK(K_CATCH); }
"CBool"					{ RETURNTOK(K_CBOOL); }
"CByte"					{ RETURNTOK(K_CBYTE); }
"CChar"					{ RETURNTOK(K_CCHAR); }
"CDate"					{ RETURNTOK(K_CDATE); }
"CDec"					{ RETURNTOK(K_CDEC); }
"CDbl"					{ RETURNTOK(K_CDBL); }
"Char"					{ RETURNTOK(K_CHAR); }
"CInt"					{ RETURNTOK(K_CINT); }
"Class"					{ RETURNTOK(K_CLASS); }
"CLng"					{ RETURNTOK(K_CLNG); }
"CObj"					{ RETURNTOK(K_COBJ); }
"Const"					{ RETURNTOK(K_CONST); }
"CShort"				{ RETURNTOK(K_CSHORT); }
"CSng"					{ RETURNTOK(K_CSNG); }
"CStr"					{ RETURNTOK(K_CSTR); }
"CType"					{ RETURNTOK(K_CTYPE); }
"Date"					{ RETURNTOK(K_DATE); }
"Decimal"				{ RETURNTOK(K_DECIMAL); }
"Declare"				{ RETURNTOK(K_DECLARE); }
"Default"				{ RETURNTOK(K_DEFAULT); }
"Delegate"				{ RETURNTOK(K_DELEGATE); }
"Dim"					{ RETURNTOK(K_DIM); }
"DirectCast"			{ RETURNTOK(K_DIRECTCAST); }
"Do"					{ RETURNTOK(K_DO); }
"Double"				{ RETURNTOK(K_DOUBLE); }
"Each"					{ RETURNTOK(K_EACH); }
"Else"					{ RETURNTOK(K_ELSE); }
"ElseIf"				{ RETURNTOK(K_ELSEIF); }
"End"					{ RETURNTOK(K_END); }
"Enum"					{ RETURNTOK(K_ENUM); }
"Erase"					{ RETURNTOK(K_ERASE); }
"Error"					{ RETURNTOK(K_ERROR); }
"Event"					{ RETURNTOK(K_EVENT); }
"Exit"					{ RETURNTOK(K_EXIT); }
"False"					{ RETURNTOK(K_FALSE); }
"Finally"				{ RETURNTOK(K_FINALLY); }
"For"					{ RETURNTOK(K_FOR); }
"Friend"				{ RETURNTOK(K_FRIEND); }
"Function"				{ RETURNTOK(K_FUNCTION); }
"Get"					{ RETURNTOK(K_GET); }
"GetType"				{ RETURNTOK(K_GETTYPE); }
"GoSub"					{ RETURNTOK(K_GOSUB); }
"GoTo"					{ RETURNTOK(K_GOTO); }
"Handles"				{ RETURNTOK(K_HANDLES); }
"If"					{ RETURNTOK(K_IF); }
"Implements"			{ RETURNTOK(K_IMPLEMENTS); }
"Imports"				{ RETURNTOK(K_IMPORTS); }
"In"					{ RETURNTOK(K_IN); }
"Inherits"				{ RETURNTOK(K_INHERITS); }
"Integer"				{ RETURNTOK(K_INTEGER); }
"Interface"				{ RETURNTOK(K_INTERFACE); }
"Is"					{ RETURNTOK(K_IS); }
"Let"					{ RETURNTOK(K_LET); }
"Lib"					{ RETURNTOK(K_LIB); }
"Like"					{ RETURNTOK(K_LIKE); }
"Long"					{ RETURNTOK(K_LONG); }
"Loop"					{ RETURNTOK(K_LOOP); }
"Me"					{ RETURNTOK(K_ME); }
"Mod"					{ RETURNTOK(K_MOD); }
"Module"				{ RETURNTOK(K_MODULE); }
"MustInherit"			{ RETURNTOK(K_MUSTINHERIT); }
"MustOverride"			{ RETURNTOK(K_MUSTOVERRIDE); }
"MyBase"				{ RETURNTOK(K_MYBASE); }
"MyClass"				{ RETURNTOK(K_MYCLASS); }
"Namespace"				{ RETURNTOK(K_NAMESPACE); }
"New"					{ RETURNTOK(K_NEW); }
"Next"					{ RETURNTOK(K_NEXT); }
"Not"					{ RETURNTOK(K_NOT); }
"Nothing"				{ RETURNTOK(K_NOTHING); }
"NotInheritable"		{ RETURNTOK(K_NOTINHERITABLE); }
"NotOverridable"		{ RETURNTOK(K_NOTOVERRIDABLE); }
"Object"				{ RETURNTOK(K_OBJECT); }
"On"					{ RETURNTOK(K_ON); }
"Option"				{ RETURNTOK(K_OPTION); }
"Optional"				{ RETURNTOK(K_OPTIONAL); }
"Or"					{ RETURNTOK(K_OR); }
"OrElse"				{ RETURNTOK(K_ORELSE); }
"Overloads"				{ RETURNTOK(K_OVERLOADS); }
"Overridable"			{ RETURNTOK(K_OVERRIDABLE); }
"Overrides"				{ RETURNTOK(K_OVERRIDES); }
"ParamArray"			{ RETURNTOK(K_PARAMARRAY); }
"Preserve"				{ RETURNTOK(K_PRESERVE); }
"Private"				{ RETURNTOK(K_PRIVATE); }
"Property"				{ RETURNTOK(K_PROPERTY); }
"Protected"				{ RETURNTOK(K_PROTECTED); }
"Public"				{ RETURNTOK(K_PUBLIC); }
"RaiseEvent"			{ RETURNTOK(K_RAISEEVENT); }
"ReadOnly"				{ RETURNTOK(K_READONLY); }
"ReDim"					{ RETURNTOK(K_REDIM); }
"RemoveHandler"			{ RETURNTOK(K_REMOVEHANDLER); }
"Resume"				{ RETURNTOK(K_RESUME); }
"Return"				{ RETURNTOK(K_RETURN); }
"Select"				{ RETURNTOK(K_SELECT); }
"Set"					{ RETURNTOK(K_SET); }
"Shadows"				{ RETURNTOK(K_SHADOWS); }
"Shared"				{ RETURNTOK(K_SHARED); }
"Short"					{ RETURNTOK(K_SHORT); }
"Single"				{ RETURNTOK(K_SINGLE); }
"Static"				{ RETURNTOK(K_STATIC); }
"Step"					{ RETURNTOK(K_STEP); }
"Stop"					{ RETURNTOK(K_STOP); }
"String"				{ RETURNTOK(K_STRING); }
"Structure"				{ RETURNTOK(K_STRUCTURE); }
"Sub"					{ RETURNTOK(K_SUB); }
"SyncLock"				{ RETURNTOK(K_SYNCLOCK); }
"Then"					{ RETURNTOK(K_THEN); }
"Throw"					{ RETURNTOK(K_THROW); }
"To"					{ RETURNTOK(K_TO); }
"True"					{ RETURNTOK(K_TRUE); }
"Try"					{ RETURNTOK(K_TRY); }
"TypeOf"				{ RETURNTOK(K_TYPEOF); }
"Unicode"				{ RETURNTOK(K_UNICODE); }
"Until"					{ RETURNTOK(K_UNTIL); }
"Variant"				{ RETURNTOK(K_VARIANT); }
"When"					{ RETURNTOK(K_WHEN); }
"While"					{ RETURNTOK(K_WHILE); }
"With"					{ RETURNTOK(K_WITH); }
"WithEvents"			{ RETURNTOK(K_WITHEVENTS); }
"WriteOnly"				{ RETURNTOK(K_WRITEONLY); } 
"Xor"					{ RETURNTOK(K_XOR); } 

{IDALPHA}({DIGIT}|{IDALPHA})*	{
			yylval.name = ILInternString(yytext, strlen(yytext)).string;
			RETURNTOK(IDENTIFIER);
		}

{IDALPHA}({DIGIT}|{IDALPHA})*{TYPECHAR}	{
			yylval.name = ILInternString(yytext, strlen(yytext)).string;
			RETURNTOK(TYPED_IDENTIFIER);
		}

"["{IDALPHA}({DIGIT}|{IDALPHA})*"]"	{
			/* Escaped identifier */
			yylval.name = ILInternString(yytext + 1, strlen(yytext) - 2).string;
			RETURNTOK(IDENTIFIER);
		}

{DIGIT}+{EXPONENT}{FTYPE}?				 { RETURNTOK(VBParseFloat(yytext)); }
{DIGIT}*"."{DIGIT}+({EXPONENT})?{FTYPE}? { RETURNTOK(VBParseFloat(yytext)); }
{DIGIT}+"."{DIGIT}*({EXPONENT})?{FTYPE}? { RETURNTOK(VBParseFloat(yytext)); }

"&H"{HEX}+{ITYPE}?				{ VBParseInt(yytext);
								  RETURNTOK(INTEGER_CONSTANT); }
"&O"{OCTAL}+{ITYPE}?			{ VBParseInt(yytext);
								  RETURNTOK(INTEGER_CONSTANT); }
{DIGIT}+{ITYPE}?				{ VBParseInt(yytext);
								  RETURNTOK(INTEGER_CONSTANT); }

\"([^"\n]|\"\")*\"C				{ VBParseChar(yytext);
								  RETURNTOK(CHAR_LITERAL); }

\"([^"\n]|\"\")*\"				{ VBParseString(yytext, &(yylval.string));
								  RETURNTOK(STRING_LITERAL); }

("Rem"|"'")						{
									/* Skip the comment line */
									int ch;
									while((ch = input()) != '\n' && ch != EOF)
									{
										/* Skip all characters until EOL */
									}
									if(!VBPrevWasEOL)
									{
										VBPrevWasEOL = 1;
										RETURNTOKNORESET(END_LINE);
									}
								}

"_"{WHITE}*\n					{ /* Line continuation */ }

{WHITE}+						;

\n								{
									if(!VBPrevWasEOL)
									{
										VBPrevWasEOL = 1;
										RETURNTOKNORESET(END_LINE);
									}
								}

.								{ VBPrevWasEOL = 0;
								  return (((int)(yytext[0])) & 0xFF); }

%%
