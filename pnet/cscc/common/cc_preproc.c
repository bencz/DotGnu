/*
 * cc_preproc.c - Pre-processor for C# source code.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

#include "il_system.h"
#include "il_utils.h"
#include "cc_preproc.h"
#include "cc_intl.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * C# uses Ctrl-Z as a whitespace character, because of its MS-DOS heritage.
 */
#define	CTRL_Z	((char)0x1A)

void CCPreProcInit(CCPreProc *preproc, FILE *stream, const char *filename,
				   int closeAtEnd)
{
	preproc->stream = stream;
	preproc->sawEOF = 0;
	preproc->utf8 = 1;
	preproc->closeAtEnd = closeAtEnd;
	preproc->docComments = 0;
	preproc->symbols = 0;
	preproc->buffer = 0;
	preproc->bufLen = 0;
	preproc->bufMax = 0;
	preproc->lines = 0;
	preproc->numLines = 0;
	preproc->maxLines = 0;
	preproc->currLine = 0;
	preproc->continueLine = 0;
	preproc->linePosn = 0;
	preproc->lineNumber = 1;
	preproc->filename =
		(filename ? (ILInternString((char *)filename, -1)).string : 0);
	preproc->defaultLinenumber = 1;
	preproc->defaultFilename =
		(filename ? (ILInternString((char *)filename, -1)).string : 0);
	preproc->currentScope = 0;
	preproc->reportedUnmatched = 0;
	preproc->lexerLineNumber = 1;
	preproc->lexerFileName = preproc->filename;
	preproc->outOfMemory = 0;
	preproc->error = 0;
}

void CCPreProcDestroy(CCPreProc *preproc)
{
	CCPreProcSymbol *symbol;
	CCPreProcSymbol *next;
	CCPreProcScope *scope;
	CCPreProcScope *nextScope;

	/* Free the line buffer */
	if(preproc->buffer)
	{
		ILFree(preproc->buffer);
	}

	/* Free the symbol list */
	symbol = preproc->symbols;
	while(symbol != 0)
	{
		next = symbol->next;
		ILFree(symbol);
		symbol = next;
	}

	/* Free the scope list */
	scope = preproc->currentScope;
	while(scope != 0)
	{
		nextScope = scope->next;
		ILFree(scope);
		scope = nextScope;
	}

	/* Close the stream */
	if(preproc->closeAtEnd)
	{
		fclose(preproc->stream);
	}
}

void CCPreProcDefine(CCPreProc *preproc, const char *name)
{
	CCPreProcSymbol *symbol;

	/* See if the symbol is already defined */
	symbol = preproc->symbols;
	while(symbol != 0)
	{
		if(!strcmp(symbol->name, name))
		{
			return;
		}
		symbol = symbol->next;
	}

	/* Add the symbol to the list */
	if((symbol = (CCPreProcSymbol *)ILMalloc(sizeof(CCPreProcSymbol) +
											 strlen(name))) == 0)
	{
		preproc->outOfMemory = 1;
		return;
	}
	strcpy(symbol->name, name);
	symbol->next = preproc->symbols;
	preproc->symbols = symbol;
}

void CCPreProcUndefine(CCPreProc *preproc, const char *name)
{
	CCPreProcSymbol *symbol;
	CCPreProcSymbol *prev;
	symbol = preproc->symbols;
	prev = 0;
	while(symbol != 0)
	{
		if(!strcmp(symbol->name, name))
		{
			if(prev)
				prev->next = symbol->next;
			else
				preproc->symbols = symbol->next;
			ILFree(symbol);
			break;
		}
		symbol = symbol->next;
	}
}

int CCPreProcIsDefined(CCPreProc *preproc, const char *name)
{
	CCPreProcSymbol *symbol = preproc->symbols;
	while(symbol != 0)
	{
		if(!strcmp(symbol->name, name))
		{
			return 1;
		}
		symbol = symbol->next;
	}
	return 0;
}

/*
 * Add a character to the input buffer.
 */
#define	ADD_CH(ch)	\
			do { \
				if(bufLen >= bufMax) \
				{ \
					newbuf = (char *)ILRealloc(buf, bufMax + 128); \
					if(!newbuf) \
						goto outOfMemory; \
					buf = newbuf; \
					bufMax += 128; \
				} \
				buf[bufLen++] = (char)(ch); \
			} while (0)

/*
 * Add line information to the "lines" buffer in "preproc".
 * Returns zero if out of memory.
 */
static int AddLine(CCPreProc *preproc, int posn, int len, int directive)
{
	CCPreProcLine *newLines;
	if(preproc->numLines >= preproc->maxLines)
	{
		newLines = (CCPreProcLine *)ILRealloc
			(preproc->lines, sizeof(CCPreProcLine) * (preproc->maxLines + 8));
		if(!newLines)
		{
			return 0;
		}
		preproc->lines = newLines;
		preproc->maxLines += 8;
	}
	preproc->lines[preproc->numLines].posn = posn;
	preproc->lines[preproc->numLines].len = len;
	preproc->lines[preproc->numLines].directive = directive;
	preproc->lines[preproc->numLines].number = 0;
	preproc->lines[preproc->numLines].filename = 0;
	++(preproc->numLines);
	return 1;
}

/*
 * Get the next few lines of input from the input stream.
 * Enough lines are retrieved to skip past multi-line comments
 * and multi-line string literals.  Returns zero at EOF.
 */
static int GetLines(CCPreProc *preproc)
{
	FILE *stream = preproc->stream;
	int ch;
	char *buf, *newbuf;
	int bufLen, bufMax;
	int comment, scomment;
	int dcomment, quote;
	int lineStart, sawEOF;
	int literal;
	int directive;

	/* Bail out early if we have already seen EOF */
	if(preproc->sawEOF)
	{
		return 0;
	}

	/* Load the buffer information for faster processing in ADD_CH */
	buf = preproc->buffer;
	bufLen = 0;
	bufMax = preproc->bufMax;

	/* Parse lines until no longer in a multi-line comment or string */
	sawEOF = 0;
	comment = 0;
	scomment = 0;
	dcomment = 0;
	quote = 0;
	literal = 0;
	lineStart = bufLen;
	directive = 0;
	preproc->numLines = 0;
	preproc->currLine = 0;
	do
	{
		/* Get the next character */
		ch = getc(stream);
		if(ch == EOF)
		{
			sawEOF = 1;
			continue;
		}

		/* Process end of line sequences */
		if(ch == 0x0A)
		{
			/* Unix-style end of line sequence */
		endLine:
			ADD_CH('\n');
			if(!AddLine(preproc, lineStart, bufLen - lineStart, directive))
			{
				goto outOfMemory;
			}

			/* Reset the state for a new line */
			lineStart = bufLen;
			directive = 0;

			/* Turn off quoting if "'" - sometimes this is used in
			   pre-processor #error directives without a matching
			   end-"'".  We use end of line to detect when to stop */
			if(quote == '\'')
			{
				quote = 0;
			}

			/* If no longer in a comment or string, then we are finished */
			if(!comment && !quote)
			{
				break;
			}
			continue;
		}
		if(ch == 0x0D)
		{
			ch = getc(stream);
			if(ch == 0x0A)
			{
				/* MS-DOS-style end of line sequence */
				goto endLine;
			}
			else
			{
				/* Macintosh-style end of line sequence */
				if(ch != EOF)
				{
					ungetc(ch, stream);
				}
				else
				{
					sawEOF = 1;
				}
				goto endLine;
			}
		}
		if((ch & 0xFF) == 0xE2 && preproc->utf8)
		{
			/* This may be a Unicode line separator or
			   paragraph separator sequence, which are
			   also "end of line" in the C# standard.  In
			   UTF-8, they are "E2 80 A8" and "E2 80 A9" */
			ch = getc(stream);
			if(ch == EOF)
			{
				if(!comment && !scomment)
				{
					ADD_CH(0xE2);
				}
				sawEOF = 1;
				continue;
			}
			if((ch & 0xFF) == 0x80)
			{
				ch = getc(stream);
				if(ch == EOF)
				{
					if(!comment && !scomment)
					{
						ADD_CH(0xE2);
						ADD_CH(0x80);
					}
					sawEOF = 1;
					continue;
				}
				if((ch & 0xFF) == 0xA8 || (ch & 0xFF) == 0xA9)
				{
					goto endLine;
				}
				ungetc(ch, stream);
				if(!comment && !scomment)
				{
					ADD_CH(0xE2);
					ADD_CH(0x80);
				}
			}
			else
			{
				ungetc(ch, stream);
				if(!comment && !scomment)
				{
					ADD_CH(0xE2);
				}
			}
			continue;
		}
		else if((ch & 0xFF) == 0xEF && preproc->utf8)
		{
			/* This may be a Unicode 0xFEFF designator.  For some
			   reason, Visual Studio's editor insists on inserting
			   this at the start of text files; presumably to indicate
			   that they are in UTF-8 format.  Check for "EF BB BF" */
			ch = getc(stream);
			if(ch == EOF)
			{
				if(!comment && !scomment)
				{
					ADD_CH(0xEF);
				}
				sawEOF = 1;
				continue;
			}
			if((ch & 0xFF) == 0xBB)
			{
				ch = getc(stream);
				if(ch == EOF)
				{
					if(!comment && !scomment)
					{
						ADD_CH(0xEF);
						ADD_CH(0xBB);
					}
					sawEOF = 1;
					continue;
				}
				if((ch & 0xFF) == 0xBF)
				{
					continue;
				}
				ungetc(ch, stream);
				if(!comment && !scomment)
				{
					ADD_CH(0xEF);
					ADD_CH(0xBB);
				}
			}
			else
			{
				ungetc(ch, stream);
				if(!comment && !scomment)
				{
					ADD_CH(0xEF);
				}
			}
			continue;
		}

		/* Process comments, strings, and normal characters */
		if(scomment)
		{
			/* Skip the remainder of a single-line comment */
			continue;
		}
		if(dcomment)
		{
			/* Process a documentation comment */
			ADD_CH(ch);
			continue;
		}
		if(comment)
		{
			/* Look for the end of a multi-line comment */
			if(ch == '*')
			{
				ch = getc(stream);
				while(ch == '*')
				{
					ch = getc(stream);
				}
				if(ch == '/')
				{
					/* End of the multi-line comment */
					comment = 0;
					ch = getc(stream);
					if(ch != ' ' && ch != '\t' && ch != '\r' &&
					   ch != '\n' && ch != '\f' && ch != '\v' &&
					   ch != CTRL_Z)
					{
						/* The comment is not followed by white space,
						   so insert a space character.  This ensures
						   that sequences like "A\*comment*\B" will be
						   expanded to "A B", and not "AB" */
						ADD_CH(' ');
					}
					if(ch != EOF)
					{
						ungetc(ch, stream);
					}
					else
					{
						sawEOF = 1;
					}
				}
				else if(ch == EOF)
				{
					/* EOF encountered inside a comment */
					sawEOF = 1;
				}
				else
				{
					ungetc(ch, stream);
				}
			}
		}
		else if(quote)
		{
			/* Still inside a string literal */
			if(ch == quote)
			{
				/* End of the string literal */
				ADD_CH(ch);
				if(literal)
				{
					ch=getc(stream);	
					if(ch==quote)
					{
						ADD_CH(quote);
					}
					else
					{
						ungetc(ch,stream);
						quote=0;
						literal=0;
					}
				}
				else
				{
					quote = 0;
				}
			}
			else if(ch == '\\')
			{
				ADD_CH('\\');
				if(!literal)
				{
					/* Escape sequence of some type */
					ch = getc(stream);
					if(ch == '"' || ch == '\'' || ch == '\\')
					{
						ADD_CH(ch);
					}
					else if(ch != EOF)
					{
						ungetc(ch, stream);
					}
					else
					{
						sawEOF = 1;
					}
				}
			}
			else
			{
				/* Normal character within a string */
				ADD_CH(ch);
			}
		}
		else if(ch == '/')
		{
			ch = getc(stream);
			if(ch == '*')
			{
				/* Start of a multi-line comment */
				comment = 1;
			}
			else if(ch == '/')
			{
				if(preproc->docComments)
				{
					ch = getc(stream);
					if(ch == '/')
					{
						/* Start of a documentation comment */
						ADD_CH('/');
						ADD_CH('/');
						ADD_CH('/');
						dcomment = 1;
					}
					else if(ch != EOF)
					{
						/* Start of a single-line comment */
						ungetc(ch, stream);
						scomment = 1;
					}
					else
					{
						/* Single-line comment that ends in EOF */
						sawEOF = 1;
					}
				}
				else
				{
					/* Start of a single-line comment */
					scomment = 1;
				}
			}
			else if(ch != EOF)
			{
				/* Normal '/' character */
				ungetc(ch, stream);
				ADD_CH('/');
			}
			else
			{
				/* Normal '/' character followed by EOF */
				ADD_CH('/');
				sawEOF = 1;
			}
		}
		else if(ch == '"' || ch == '\'')
		{
			/* Don't parse comments within string literals */
			quote = ch;
			ADD_CH(ch);
		}
		else if(ch == '#')
		{
			/* This line may be a pre-processor directive */
			directive = 1;
			ADD_CH(ch);
		}
		else if(ch == '@')
		{
			/* This may be the start of a literal string */
			ADD_CH(ch);
			ch = getc(stream);
			if(ch == '"' || ch == '\'')
			{
				quote = ch;
				literal = 1;
				ADD_CH(ch);
			}
			else
			{
				ungetc(ch, stream);
			}
		}
		else
		{
			/* Normal character: add it to the current line */
			ADD_CH(ch);
		}
	}
	while(!sawEOF);

	/* Terminate the last line if we saw EOF on a non-empty line */
	if(sawEOF && lineStart < bufLen)
	{
		ADD_CH('\n');
	}

	/* Add the final line to the "lines" buffer */
	if(lineStart < bufLen)
	{
		if(!AddLine(preproc, lineStart, bufLen - lineStart, directive))
		{
			goto outOfMemory;
		}
	}

	/* Restore cached information to "preproc" */
	preproc->sawEOF = sawEOF;
	preproc->buffer = buf;
	preproc->bufLen = bufLen;
	preproc->bufMax = bufMax;

	/* Done */
	return (bufLen > 0);

	/* ADD_CH jumps here on out of memory */
outOfMemory:
	preproc->sawEOF = 1;
	preproc->outOfMemory = 1;
	preproc->buffer = buf;
	preproc->bufLen = bufLen;
	preproc->bufMax = bufMax;
	return (bufLen > 0);
}

/*
 * Check a line to see if it is a directive, and then find the name.
 * Returns NULL if not really a directive line.  i.e. it contains '#',
 * but it isn't the first non-white character on the line.
 */
static char *DirectiveName(char *line)
{
	/* Skip white space to find the '#' */
	while(*line == ' ' || *line == '\t' || *line == '\f' ||
	      *line == '\v' || *line == CTRL_Z)
	{
		++line;
	}
	if(*line != '#')
	{
		return 0;
	}

	/* Skip the '#' and any following white space */
	++line;
	while(*line == ' ' || *line == '\t' || *line == '\f' ||
	      *line == '\v' || *line == CTRL_Z)
	{
		++line;
	}

	/* The name starts here */
	return line;
}

/*
 * Match a directive name.
 */
#define	MATCH_DIRECTIVE(dirname,name,len)	\
			(!strncmp((dirname), (name), (len)) && \
			 ((dirname)[(len)] == ' ' || (dirname)[(len)] == '\t' || \
			  (dirname)[(len)] == '\n' || (dirname)[(len)] == '\f' || \
			  (dirname)[(len)] == '\v' || (dirname)[(len)] == CTRL_Z || \
			  (dirname)[(len)] == ';'))

/*
 * Determine if the current line is active according to
 * the current #if context.
 */
#define	LINE_IS_ACTIVE()	\
			(!(preproc->currentScope) || \
			   (preproc->currentScope->active && \
			    preproc->currentScope->ancestor))

/*
 * Report an error or warning message for a specific line.
 */
static void Message(CCPreProcLine *line, const char *msg1, const char *msg2)
{
	if(line->filename)
	{
		fprintf(stderr, "%s:%lu: %s%s\n", line->filename,
			    line->number, msg1, (msg2 ? msg2 : ""));
	}
	else
	{
		fprintf(stderr, "%lu: %s%s\n", line->number, msg1, (msg2 ? msg2 : ""));
	}
}

/*
 * Skip white space on a directive line.
 */
static void SkipWhite(const char **_line)
{
	const char *line = (*_line);
	while(*line == ' ' || *line == '\t' || *line == '\f' ||
	      *line == '\v' || *line == CTRL_Z)
	{
		++line;
	}
	(*_line) = line;
}

/*
 * Parse an identifier from a directive line.
 */
static const char *ParseIdentifier(CCPreProc *preproc, CCPreProcLine *info,
							 const char **_line)
{
	const char *line;
	int len;
	const char *identifier;

	/* Skip white space before the identifier */
	SkipWhite(_line);
	line = (*_line);

	/* Is this a legal identifier start character? */
	if((*line < 'A' || *line > 'Z') &&
	   (*line < 'a' || *line > 'z') && *line != '_')
	{
		Message(info, _("pre-processor identifier expected"), 0);
		preproc->error = 1;
		return 0;
	}

	/* Parse the identifier */
	++line;
	len = 1;
	while((*line >= 'A' && *line <= 'Z') ||
	      (*line >= 'a' && *line <= 'z') ||
	      (*line >= '0' && *line <= '9') ||
		  *line == '_')
	{
		++line;
		++len;
	}
	(*_line) = line;

	/* Intern the identifier */
	identifier = (ILInternString(line - len, len)).string;
	if(!identifier)
	{
		preproc->outOfMemory = 1;
		return 0;
	}

	/* Check for invalid identifiers */
	if(!strcmp(identifier, "true") || !strcmp(identifier, "false"))
	{
		Message(info, _("invalid pre-processor identifier: "), identifier);
		preproc->error = 1;
		return 0;
	}

	/* Done */
	return identifier;
}

/*
 * Token definition for pre-processor expression tokens.
 */
typedef struct
{
	int			type;
	const char *name;

} CCPreProcToken;
#define	CSPP_TOKEN_END			0
#define	CSPP_TOKEN_TRUE			1
#define	CSPP_TOKEN_FALSE		2
#define	CSPP_TOKEN_IDENTIFIER	3
#define	CSPP_TOKEN_LPAREN		4
#define	CSPP_TOKEN_RPAREN		5
#define	CSPP_TOKEN_NOT			6
#define	CSPP_TOKEN_AND			7
#define	CSPP_TOKEN_OR			8
#define	CSPP_TOKEN_EQ			9
#define	CSPP_TOKEN_NE			10
#define	CSPP_TOKEN_ERROR		11

/*
 * Get the next token from a directive line.
 */
static void NextToken(const char **_line, CCPreProcToken *token)
{
	const char *line;
	int len;

	/* Skip white space before the token */
	SkipWhite(_line);
	line = (*_line);

	/* What type of token do we have? */
	if(*line == '\n')
	{
		/* End of directive line */
		token->type = CSPP_TOKEN_END;
	}
	else if((*line >= 'A' && *line <= 'Z') ||
			(*line >= 'a' && *line <= 'z') || *line == '_')
	{
		/* Identifier */
		++line;
		len = 1;
		while((*line >= 'A' && *line <= 'Z') ||
		      (*line >= 'a' && *line <= 'z') ||
		      (*line >= '0' && *line <= '9') ||
			  *line == '_')
		{
			++line;
			++len;
		}
		token->name = (ILInternString(line - len, len)).string;
		if(!strcmp(token->name, "true"))
		{
			token->type = CSPP_TOKEN_TRUE;
		}
		else if(!strcmp(token->name, "false"))
		{
			token->type = CSPP_TOKEN_FALSE;
		}
		else
		{
			token->type = CSPP_TOKEN_IDENTIFIER;
		}
	}
	else if(*line == '(')
	{
		/* Left parenthesis */
		++line;
		token->type = CSPP_TOKEN_LPAREN;
	}
	else if(*line == ')')
	{
		/* Right parenthesis */
		++line;
		token->type = CSPP_TOKEN_RPAREN;
	}
	else if(*line == '=' && line[1] == '=')
	{
		/* Equality */
		line += 2;
		token->type = CSPP_TOKEN_EQ;
	}
	else if(*line == '!' && line[1] == '=')
	{
		/* Inequality */
		line += 2;
		token->type = CSPP_TOKEN_NE;
	}
	else if(*line == '!')
	{
		/* Logical negation */
		++line;
		token->type = CSPP_TOKEN_NOT;
	}
	else if(*line == '&' && line[1] == '&')
	{
		/* Logical AND */
		line += 2;
		token->type = CSPP_TOKEN_AND;
	}
	else if(*line == '|' && line[1] == '|')
	{
		/* Logical OR */
		line += 2;
		token->type = CSPP_TOKEN_OR;
	}
	else
	{
		/* Unknown token type */
		token->type = CSPP_TOKEN_ERROR;
	}
	(*_line) = line;
}

/*
 * Forward declaration.
 */
static int ParseOrExpression(CCPreProc *preproc, const char **line,
						     CCPreProcToken *token);

/*
 * Parse a primary expression.
 */
static int ParsePrimaryExpression(CCPreProc *preproc, const char **line,
								  CCPreProcToken *token)
{
	int result;
	if(token->type == CSPP_TOKEN_TRUE)
	{
		NextToken(line, token);
		return 1;
	}
	else if(token->type == CSPP_TOKEN_FALSE)
	{
		NextToken(line, token);
		return 0;
	}
	else if(token->type == CSPP_TOKEN_IDENTIFIER)
	{
		result = CCPreProcIsDefined(preproc, token->name);
		NextToken(line, token);
		return result;
	}
	else if(token->type == CSPP_TOKEN_LPAREN)
	{
		NextToken(line, token);
		result = ParseOrExpression(preproc, line, token);
		if(token->type == CSPP_TOKEN_RPAREN)
		{
			NextToken(line, token);
		}
		else
		{
			token->type = CSPP_TOKEN_ERROR;
		}
		return result;
	}
	else
	{
		token->type = CSPP_TOKEN_ERROR;
		return 0;
	}
}

/*
 * Parse a unary expression.
 */
static int ParseUnaryExpression(CCPreProc *preproc, const char **line,
								CCPreProcToken *token)
{
	/* Negated or normal sub-expression? */
	if(token->type == CSPP_TOKEN_NOT)
	{
		NextToken(line, token);
		return !(ParseUnaryExpression(preproc, line, token));
	}
	else
	{
		return ParsePrimaryExpression(preproc, line, token);
	}
}

/*
 * Parse an equality expression.
 */
static int ParseEqualityExpression(CCPreProc *preproc, const char **line,
								   CCPreProcToken *token)
{
	int result1, result2, iseq;

	/* Parse the first sub-expression */
	result1 = ParseUnaryExpression(preproc, line, token);

	/* Process trailing '==' or '!=' operators */
	while(token->type == CSPP_TOKEN_EQ ||
	      token->type == CSPP_TOKEN_NE)
	{
		iseq = (token->type == CSPP_TOKEN_EQ);
		NextToken(line, token);
		result2 = ParseUnaryExpression(preproc, line, token);
		if(iseq)
		{
			result1 = (result1 == result2);
		}
		else
		{
			result1 = (result1 != result2);
		}
	}

	/* Return the result to the caller */
	return result1;
}

/*
 * Parse an AND expression.
 */
static int ParseAndExpression(CCPreProc *preproc, const char **line,
							  CCPreProcToken *token)
{
	int result1, result2;

	/* Parse the first sub-expression */
	result1 = ParseEqualityExpression(preproc, line, token);

	/* Process trailing '&&' operators */
	while(token->type == CSPP_TOKEN_AND)
	{
		NextToken(line, token);
		result2 = ParseEqualityExpression(preproc, line, token);
		result1 = (result1 && result2);
	}

	/* Return the result to the caller */
	return result1;
}

/*
 * Parse an OR expression.
 */
static int ParseOrExpression(CCPreProc *preproc, const char **line,
							 CCPreProcToken *token)
{
	int result1, result2;

	/* Parse the first sub-expression */
	result1 = ParseAndExpression(preproc, line, token);

	/* Process trailing '||' operators */
	while(token->type == CSPP_TOKEN_OR)
	{
		NextToken(line, token);
		result2 = ParseAndExpression(preproc, line, token);
		result1 = (result1 || result2);
	}

	/* Return the result to the caller */
	return result1;
}

/*
 * Parse an expression.  Returns 0 or 1 for the value of the expression.
 */
static int ParseExpression(CCPreProc *preproc, CCPreProcLine *info,
						   const char **line)
{
	CCPreProcToken token;
	int result;

	/* Fetch the first token */
	NextToken(line, &token);

	/* Perform the parse */
	result = ParseOrExpression(preproc, line, &token);

	/* If the token is not "END", or there is data left on the line,
	   then the expression is invalid */
	SkipWhite(line);
	if(token.type != CSPP_TOKEN_END || (*(*line)) != '\n')
	{
		Message(info, _("invalid pre-processor expression"), 0);
		preproc->error = 1;
		result = 0;
	}

	/* Done */
	return result;
}

/*
 * Determine if we are at the end of a directive line.
 * If not, report an error.
 */
static void CheckAtEnd(CCPreProc *preproc, CCPreProcLine *info,
					   const char **line)
{
	SkipWhite(line);
	if((*(*line)) != '\n')
	{
		Message(info, _("spurious data on pre-processor directive line"), 0);
		preproc->error = 1;
	}
}

/*
 * Refill the line buffer.
 */
static int RefillLineBuffer(CCPreProc *preproc)
{
	CCPreProcLine *lines;
	int line, cond;
	const char *dirname;
	const char *symbol;
	CCPreProcScope *scope;
	unsigned long num;
	int len;

	/* Get more lines from the input and strip comments from them */
	if(!GetLines(preproc))
	{
		if(!(preproc->reportedUnmatched))
		{
			/* Report errors for any #if directives that are still open */
			preproc->reportedUnmatched = 1;
			scope = preproc->currentScope;
			while(scope != 0)
			{
				if(scope->filename)
				{
					fputs(scope->filename, stderr);
					putc(':', stderr);
				}
				fprintf(stderr, "%lu: ", scope->number);
				fputs(_("#if without matching #endif\n"), stderr);
				preproc->error = 1;
				scope = scope->next;
			}
		}
		return 0;
	}

	/* Process directives and add line number information */
	lines = preproc->lines;
	for(line = 0; line < preproc->numLines; ++line)
	{
		/* Set the line number information for this line */
		lines[line].number = (preproc->lineNumber)++;
		lines[line].filename = preproc->filename;
		preproc->defaultLinenumber++;

		/* Is this a directive? */
		if(lines[line].directive &&
		   (dirname = DirectiveName(preproc->buffer + lines[line].posn)) != 0)
		{
			/* Determine what type of directive we are processing */
			if(MATCH_DIRECTIVE(dirname, "define", 6))
			{
				/* Define a symbol */
				if(LINE_IS_ACTIVE())
				{
					dirname += 6;
					symbol = ParseIdentifier(preproc, &(lines[line]), &dirname);
					if(symbol)
					{
						CheckAtEnd(preproc, &(lines[line]), &dirname);
						CCPreProcDefine(preproc, symbol);
					}
				}
			}
			else if(MATCH_DIRECTIVE(dirname, "undef", 5))
			{
				/* Undefine a symbol */
				if(LINE_IS_ACTIVE())
				{
					dirname += 5;
					symbol = ParseIdentifier(preproc, &(lines[line]), &dirname);
					if(symbol)
					{
						CheckAtEnd(preproc, &(lines[line]), &dirname);
						CCPreProcUndefine(preproc, symbol);
					}
				}
			}
			else if(MATCH_DIRECTIVE(dirname, "if", 2))
			{
				/* Test for conditional inclusion */
				dirname += 2;
				cond = ParseExpression(preproc, &(lines[line]), &dirname);
				scope = (CCPreProcScope *)ILMalloc(sizeof(CCPreProcScope));
				if(scope)
				{
					scope->active = cond;
					scope->previous = 0;
					if(preproc->currentScope)
					{
						scope->ancestor = (preproc->currentScope->active &&
										   preproc->currentScope->ancestor);
					}
					else
					{
						scope->ancestor = 1;
					}
					scope->sawElse = 0;
					scope->number = lines[line].number;
					scope->filename = lines[line].filename;
					scope->next = preproc->currentScope;
					preproc->currentScope = scope;
				}
				else
				{
					preproc->outOfMemory = 1;
				}
			}
			else if(MATCH_DIRECTIVE(dirname, "elif", 4))
			{
				/* Start of an "else if" block of a conditional inclusion */
				dirname += 4;
				cond = ParseExpression(preproc, &(lines[line]), &dirname);
				if(preproc->currentScope)
				{
					dirname += 4;
					preproc->currentScope->previous |=
							preproc->currentScope->active;
					if(preproc->currentScope->previous)
					{
						preproc->currentScope->active = 0;
					}
					else
					{
						preproc->currentScope->active = cond;
					}
					if(preproc->currentScope->sawElse)
					{
						Message(&(lines[line]), _("#elif used after #else"), 0);
						preproc->error = 1;
					}
				}
				else
				{
					Message(&(lines[line]), _("unmatched #elif"), 0);
					preproc->error = 1;
				}
			}
			else if(MATCH_DIRECTIVE(dirname, "else", 4))
			{
				/* Start of an "else" block of a conditional inclusion */
				dirname += 4;
				CheckAtEnd(preproc, &(lines[line]), &dirname);
				if(preproc->currentScope)
				{
					preproc->currentScope->previous |=
							preproc->currentScope->active;
					if(preproc->currentScope->previous)
					{
						preproc->currentScope->active = 0;
					}
					else
					{
						preproc->currentScope->active = 1;
					}
					if(preproc->currentScope->sawElse)
					{
						Message(&(lines[line]), _("#else used after #else"), 0);
						preproc->error = 1;
					}
					preproc->currentScope->sawElse = 1;
				}
				else
				{
					Message(&(lines[line]), _("unmatched #else"), 0);
					preproc->error = 1;
				}
			}
			else if(MATCH_DIRECTIVE(dirname, "endif", 5))
			{
				/* End of a conditional inclusion */
				dirname += 5;
				CheckAtEnd(preproc, &(lines[line]), &dirname);
				if(preproc->currentScope)
				{
					CCPreProcScope *next = preproc->currentScope->next;
					ILFree(preproc->currentScope);
					preproc->currentScope = next;
				}
				else
				{
					Message(&(lines[line]), _("unmatched #endif"), 0);
					preproc->error = 1;
				}
			}
			else if(MATCH_DIRECTIVE(dirname, "error", 5))
			{
				/* Emit a compiler error if this #if scope is active */
				if(LINE_IS_ACTIVE())
				{
					preproc->buffer[lines[line].posn +
									lines[line].len - 1] = '\0';
					dirname += 5;
					while(*dirname == ' ' || *dirname == '\t' ||
					      *dirname == '\f' || *dirname == '\v' ||
						  *dirname == CTRL_Z)
					{
						++dirname;
					}
					Message(&(lines[line]), _("error: "), dirname);
					preproc->error = 1;
				}
			}
			else if(MATCH_DIRECTIVE(dirname, "warning", 7))
			{
				/* Emit a compiler warning if this #if scope is active */
				if(LINE_IS_ACTIVE())
				{
					dirname += 7;
					preproc->buffer[lines[line].posn +
									lines[line].len - 1] = '\0';
					while(*dirname == ' ' || *dirname == '\t' ||
					      *dirname == '\f' || *dirname == '\v' ||
						  *dirname == CTRL_Z)
					{
						++dirname;
					}
					Message(&(lines[line]), _("warning: "), dirname);
				}
			}
			else if(MATCH_DIRECTIVE(dirname, "line", 4))
			{
				/* Change the line number information */
				dirname += 4;
				SkipWhite(&dirname);
				if(*dirname >= '0' && *dirname <= '9')
				{
					num = 0;
					while(*dirname >= '0' && *dirname <= '9')
					{
						num = num * 10 + (unsigned long)(*dirname++ - '0');
					}
					preproc->lineNumber = num;
					SkipWhite(&dirname);
					if(*dirname == '"')
					{
						++dirname;
						len = 0;
						while(*dirname != '"' && *dirname != '\n')
						{
							++len;
							++dirname;
						}
						preproc->filename =
							(ILInternString(dirname - len, len)).string;
						if(*dirname == '\n')
						{
							Message(&(lines[line]),
									_("badly formatted #line directive"), 0);
							preproc->error = 1;
						}
						else
						{
							++dirname;
						}
					}
					CheckAtEnd(preproc, &(lines[line]), &dirname);
				}
				else if(!strncmp(dirname,"default",7))
				{
					preproc->lineNumber = preproc->defaultLinenumber;
					preproc->filename = preproc->defaultFilename;
					dirname+=7;
					CheckAtEnd(preproc, &(lines[line]), &dirname);
				}
				else
				{
					Message(&(lines[line]),
							_("badly formatted #line directive"), 0);
					preproc->error = 1;
				}
			}
			else if(MATCH_DIRECTIVE(dirname, "region", 6) ||
					MATCH_DIRECTIVE(dirname, "endregion", 9))
			{
				/* Start or end of a marked region.  Regions exist for the
				   purpose of source-level tools, and are of no interest
				   to the compiler.  So we simply skip the line.  We probably
				   should also check that #region and #endregion lines
				   are properly matched, but we don't do that yet */
			}
#if IL_VERSION_MAJOR > 1
			else if(MATCH_DIRECTIVE(dirname, "pragma", 6))
			{
				/*
				 * We don't handle #pragma at the moment so simply skip this
				 * line.
				 */
			}
#endif /* IL_VERSION_MAJOR > 1 */
			else
			{
				/* Unknown directive */
				preproc->buffer[lines[line].posn + lines[line].len - 1] = '\0';
				Message(&(lines[line]), _("unknown pre-processor directive: "),
						preproc->buffer + lines[line].posn);
				preproc->error = 1;
			}

			/* Replace the directive with an empty line */
			preproc->buffer[lines[line].posn] = '\n';
			lines[line].len = 1;
		}
		else
		{
			/* Normal line: has it been suppressed by conditions? */
			if(!LINE_IS_ACTIVE())
			{
				/* Replace this suppressed line with an empty line */
				preproc->buffer[lines[line].posn] = '\n';
				lines[line].len = 1;
			}
		}
	}

	/* Ready to go: all pre-processing has been done */
	return 1;
}

int CCPreProcGetBuffer(CCPreProc *preproc, char *buf, int maxSize)
{
	int len;

	/* Refill the line buffer if it is empty */
	if(preproc->currLine >= preproc->numLines)
	{
		if(!RefillLineBuffer(preproc))
		{
			return 0;
		}
	}

	/* Are we continuing to return a long line from last time? */
	if(preproc->continueLine)
	{
		len = preproc->lines[preproc->currLine].len - preproc->linePosn;
		if(len <= maxSize)
		{
			ILMemCpy(buf, preproc->buffer + preproc->linePosn +
						  preproc->lines[preproc->currLine].posn, len);
			++(preproc->currLine);
			preproc->continueLine = 0;
			preproc->linePosn = 0;
			return len;
		}
		else
		{
			ILMemCpy(buf, preproc->buffer + preproc->linePosn +
						  preproc->lines[preproc->currLine].posn, maxSize);
			preproc->linePosn += maxSize;
			return maxSize;
		}
	}

	/* Set the filename and line number information for the lexer */
	preproc->lexerFileName = preproc->lines[preproc->currLine].filename;
	preproc->lexerLineNumber = preproc->lines[preproc->currLine].number;

	/* Copy the contents of the line to the caller's buffer */
	len = preproc->lines[preproc->currLine].len;
	if(len <= maxSize)
	{
		ILMemCpy(buf, preproc->buffer +
					  preproc->lines[preproc->currLine].posn, len);
		++(preproc->currLine);
		return len;
	}
	else
	{
		ILMemCpy(buf, preproc->buffer +
					  preproc->lines[preproc->currLine].posn, maxSize);
		preproc->continueLine = 1;
		preproc->linePosn = maxSize;
		return maxSize;
	}
}

#ifdef	__cplusplus
};
#endif
