
/*
 * scanner.c - C# source file to HTML translator
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Author: Jeff Post
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 *
 */

// Scanner/parser for cssrc2html

#include	<stdio.h>
#include	<stdlib.h>
#include	<ctype.h>
#include	<string.h>

#include	"cssrc2html.h"
#include	"tokens.h"
#include	"scanner.h"

char *SpecialNameTable[] = {		// identifiers for a refs
	"namespace", "class"
};

char *DeclKeywordTable[] = {		// C# declaration key words
	"abstract", "__arglist", "class", "const", "delegate",
	"enum", "event", "explicit", "extern", "implicit",
	"interface", "internal", "__module", "namespace",
	"operator", "out", "override", "params", "private",
	"protected", "public", "readonly", "ref", "sealed",
	"static", "struct", "unsafe", "using", "virtual",
	"volatile",
	NULL
};

char *TypeKeywordTable[] = {		// C# type key words
	"bool", "byte", "char", "decimal", "double", "float",
	"int", "long", "__long_double", "object", "sbyte",
	"short", "string", "uint", "ulong", "ushort", "void",
	NULL
};

char *StmtKeywordTable[] = {		// C# statement key words
	"break", "case", "catch", "checked", "continue",
	"default", "do", "else", "endif", "finally", "fixed",
	"for", "foreach", "goto", "if", "lock", "return",
	"switch", "throw", "try", "unchecked", "while",
	NULL
};

char *ExprKeywordTable[] = {		// C# expression key words
	"as", "base", "false", "in", "is", "__makeref", "new",
	"null", "__reftype", "refvalue", "sizeof", "stackalloc",
	"this", "true", "typeof",
	NULL
};

int	linePosition = 0;
char	lastchar;

char	namespace[MAX_LENGTH];			// token string for namespace
char	className[MAX_LENGTH];			// token string for current class
char	methodName[MAX_LENGTH];			// token string for method name
int	depth = 0;							// for determining if a token is a method name

char	tokenString[MAX_LENGTH];		// next token string from scanner
char	expandText[8];						// for expanding special characters
int	expandPosition = 0;
bool	expand = FALSE;
int	token;								// current token for scanner/parser
int	error = NoError;					// scanner error code
bool	inCcomment = FALSE;				// true if parsing C style comment
bool	inCPPcomment = FALSE;			// true if parsing C++ style comment
bool	inString = FALSE;					// true if parsing quoted string
bool	namespacePending = FALSE;
bool	classPending = FALSE;
bool	methodPending = FALSE;

/* Return Token type if current tokenString is a special keyword,
	or return false if not */

bool tokenIsSpecial(void)
{
	int	i, type;

	i = 0;
	type = TokenNAMESPACE;
	while (SpecialNameTable[i])
	{
		if (strcmp(tokenString, SpecialNameTable[i]) == 0)
		{
			return type;
		}
		i++;
		type++;
	}
	return FALSE;
}

/* Return Token type if current tokenString is a keyword,
   or return false if not a keyword. */

bool tokenIsKeyword(void)
{
	int	i;

	i = 0;
	while (DeclKeywordTable[i])
	{
		if (strcmp(tokenString, DeclKeywordTable[i]) == 0)
			return TokenDeclKEYWORD;
		i++;
	}
	i = 0;
	while (TypeKeywordTable[i])
	{
		if (strcmp(tokenString, TypeKeywordTable[i]) == 0)
			return TokenTypeKEYWORD;
		i++;
	}
	i = 0;
	while (StmtKeywordTable[i])
	{
		if (strcmp(tokenString, StmtKeywordTable[i]) == 0)
			return TokenStmtKEYWORD;
		i++;
	}
	i = 0;
	while (ExprKeywordTable[i])
	{
		if (strcmp(tokenString, ExprKeywordTable[i]) == 0)
			return TokenExprKEYWORD;
		i++;
	}
	return FALSE;
}

/* Return True if valid identifier character */

bool isIdChar(char c)
{
	if (isalnum(c) || c == '_')
		return TRUE;
	if (c == '.' && namespacePending)
		return TRUE;
	return FALSE;
}

/* Fetch the next character from the input stream for the scanner */

char getNextChar(void)
{
	char	c;

	if (expand)
	{
		c = expandText[expandPosition];
		if (c)
		{
			expandPosition++;
			return c;
		}
		else
		{
			expand = FALSE;
			expandPosition = 0;
		}
	}
	if (linePosition < strlen(lineBuffer))			// not at end of line
	{
		c = lineBuffer[linePosition++];
		switch (c)
		{
			case '<':
				strcpy(expandText, "&lt;");
				expandPosition = 1;
				expand = TRUE;
				c = '&';
				break;

			case '>':
				strcpy(expandText, "&gt;");
				expandPosition = 1;
				expand = TRUE;
				c = '&';
				break;

			case '&':
				strcpy(expandText, "&amp;");
				expandPosition = 1;
				expand = TRUE;
				c = '&';
				break;
		}
		return c;
	}
	linePosition = 0;			// end of line
	return ('\n');
}

/* Push current character back into the input stream - used by getToken() scanner */

void ungetNextChar(void)
{
	if (expand && expandPosition > 0)
	{
		--expandPosition;
		return;
	}

	if (linePosition > 0)
		--linePosition;
	else
		printf("DEBUG - Why are we pushing onto an EMPTY line?\n");
}

/*-----------------------------------------
 *
 * This is the scanner.
 * Returns next token from the source file.
 *
 *----------------------------------------*/

int getToken(void)
{
	char	c;
	bool	save;
	int	kword = TokenNULL;
	int	tsoffset;
	int	currentToken;
	int	scan_state;

	scan_state = START;
	tokenString[0] = '\0';				// init token string to null
	tsoffset = 0;
	currentToken = TokenNULL;
	while (scan_state != DONE)			// until we've got a complete token...
	{
		save = TRUE;
		c = getNextChar();

		if (c == '{')
			depth++;
		else if (c == '}')
			--depth;

		if (c == (char) 0)
		{
			scan_state = DONE;
			currentToken = TokenERROR;
			linePosition = 0;
			break;
		}

		if (inCcomment || inCPPcomment)		// if we were processing a comment,
			scan_state = INCOMMENT;				// remain in that state until end of comment

		switch (scan_state)
		{
			case START:								// beginning of token
				if (methodPending)					// if last token was a method name, make a tag for it
				{
					ungetNextChar();
					strcpy(tokenString, "<a name=\"");
					if (strlen(namespace) > 0)
					{
						strcat(tokenString, namespace);
						strcat(tokenString, ".");
					}
					strcat(tokenString, className);
					strcat(tokenString, ".");
					strcat(tokenString, methodName);
					strcat(tokenString, "\">");
					save = FALSE;
					scan_state = DONE;
					currentToken = TokenMETHOD;
					methodPending = FALSE;
				}
				else										// if last token not a method name, then start new scan
				{
					if (isIdChar(c))
						scan_state = INID;			// is possible keyword
					else if (c == '/')
							scan_state = INCOMMENT;		// may be a comment
					else if (c == '"')
						scan_state = INSTRING;		// is a quoted string
					else if (c == '\t')				// return tab as a token so it can be expanded
					{
						scan_state = DONE;
						currentToken = TokenTAB;
					}
					else if (c == '\n')				// newline at start of token
					{
						linePosition = 0;
						currentToken = TokenEND;
						scan_state = DONE;
					}
					else									// if none of the above, must be normal text
						scan_state = INTEXT;
				}			
				break;

			case INID:
				if (!isIdChar(c))					// if not a valid keyword character,
				{
					if (namespacePending)		// if it's the namespace, make a tag for it
					{
						ungetNextChar();
						strcpy(namespace, tokenString);
						strcpy(tokenString, "<a name=\"");
						strcat(tokenString, namespace);
						strcat(tokenString, "\">");
						strcat(tokenString, namespace);
						save = FALSE;
						scan_state = DONE;
						currentToken = TokenNAMESPACE;
						namespacePending = FALSE;
						depth = 0;
					}
					else if (classPending)		// it it's a class name, make a tag for it
					{
						ungetNextChar();
						strcpy(className, tokenString);
						strcpy(tokenString, "<a name=\"");
						if (strlen(namespace) > 0)
						{
							strcat(tokenString, namespace);
							strcat(tokenString, ".");
						}
						strcat(tokenString, className);
						strcat(tokenString, "\">");
						strcat(tokenString, className);
						save = FALSE;
						scan_state = DONE;
						currentToken = TokenCLASS;
						classPending = FALSE;
						depth = 1;
					}
					else
					{					// If character is '(' and last token was not an expression keyword,
										// and current token string is not the class name nor "this", nor "base",
										// then this token string is a method name.
						if (c == '(' && depth == 2 && kword != TokenExprKEYWORD &&
								strcmp(tokenString, className) &&
								strcmp(tokenString, "this") &&
								strcmp(tokenString, "base") &&
								strcmp(tokenString, methodName))
						{
							strcpy(methodName, tokenString);		// save method name for tag
							methodPending = TRUE;
						}

						ungetNextChar();				// push it back, and check if what we have so
						save = FALSE;					// far is a C# keyword
						scan_state = DONE;

						if (namespacePending)
							strcpy(namespace, tokenString);
						else if (classPending)
						strcpy(className, tokenString);

						kword = tokenIsSpecial();	// check for 'namespace', 'class' keywords
						if (kword)
						{
							switch (kword)
							{
								case TokenNAMESPACE:				// if next token is an ID,
									namespacePending = TRUE;	// then save it as namespace
									break;

								case TokenCLASS:				// if next token is an ID,
									classPending = TRUE;		// then save it as className
									break;
							}
						}
						kword = tokenIsKeyword();	// check keyword tables
						if (kword)
							currentToken = kword;	// is one of the defined key words
						else
							currentToken = TokenTEXT;	// else is normal text
					}				
				}
				break;

			case INTEXT:
				if (c == '\n' || c == '/' || c == '\t' || c == '"' || isIdChar(c))
				{
					save = FALSE;					// may be part of key word or invalid text character
					ungetNextChar();				// so push it back
					scan_state = DONE;			// and return text token
					currentToken = TokenTEXT;
				}
				break;

			case INSTRING:
				if (c == '"')						// end of quoted string
				{
					scan_state = DONE;
					currentToken = TokenSTRING;
				}
				break;

			case INCOMMENT:
				if (c == '\t')						// expand tabs separately from comment
				{
					if (tokenString[0])
					{
						save = FALSE;
						ungetNextChar();
						scan_state = DONE;
						currentToken = TokenTEXT;
					}
					else
					{
						scan_state = DONE;
						currentToken = TokenTAB;
					}
				}

				if (!inCcomment && !inCPPcomment)	// possible start of comment
				{
					if (c == '/')							// C++ style comment
					{
						inCPPcomment = TRUE;
					}
					else if (c == '*')					// C style comment
					{
						inCcomment = TRUE;
					}
					if (inCcomment || inCPPcomment)	// let caller know about it
					{
						scan_state = DONE;
						currentToken = TokenCOMMENT;
					}
					else
					{
						save = FALSE;
						ungetNextChar();
						scan_state = DONE;
						currentToken = TokenTEXT;
					}
				}
				else								// already in comment text, check for end of comment
				{
					if (inCcomment)
					{
						if (c == '\n')
						{
							linePosition = 0;
							scan_state = DONE;
							currentToken = TokenEND;
						}
						else if (lastchar == '*' && c == '/')	// end of C comment
						{
							scan_state = DONE;
							currentToken = TokenCOMMENTEND;
							inCcomment = FALSE;
						}
					}
					else if (inCPPcomment)
					{
						if (c == '\n')				// newline ends C++ comment
						{
							save = FALSE;
							ungetNextChar();
							scan_state = DONE;
							currentToken = TokenCOMMENTEND;
							inCPPcomment = FALSE;
						}
					}
				}
				lastchar = c;
				break;

			case DONE:
				break;

			default:
				printf("Scanner state error\n");
				error = ScannerStateError;
				break;
		}

		if (save)
		{
			tokenString[tsoffset++] = c;
			tokenString[tsoffset] = '\0';
		}
	}
	return currentToken;
}

// end of file
