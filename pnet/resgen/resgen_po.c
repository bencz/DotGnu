/*
 * resgen_po.c - PO resource loading and writing routines.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

#include <stdio.h>
#include "resgen.h"
#include "il_system.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Token information.
 */
#define	PO_TOKEN_EOF		0
#define	PO_TOKEN_ERROR		1
#define	PO_TOKEN_MSGID		2
#define	PO_TOKEN_MSGSTR		3
#define	PO_TOKEN_STRING		4
#define	PO_TOKEN_START		5
#define	PO_TOKEN_FUZZY		6
typedef struct
{
	FILE       *stream;
	int			sawEOF;
	int			token;
	char       *text;
	int			posn;
	int			len;
	const char *filename;
	long		linenum;

} POTokenInfo;

/*
 * Add a character to the text buffer that is being built.
 */
static void addPOChar(POTokenInfo *tokenInfo, int ch)
{
	char *newText;
	newText = (char *)ILRealloc(tokenInfo->text, tokenInfo->len + 64);
	if(!newText)
	{
		ILResOutOfMemory();
	}
	tokenInfo->text = newText;
	tokenInfo->len += 64;
	tokenInfo->text[(tokenInfo->posn)++] = (char)ch;
}
#define	PO_ADD_CH(ch)	\
			do { \
				if(tokenInfo->posn < tokenInfo->len) \
				{ \
					tokenInfo->text[(tokenInfo->posn)++] = (char)(ch); \
				} \
				else \
				{ \
					addPOChar(tokenInfo, (ch)); \
				} \
			} while (0)

/*
 * Write a UTF-8 character to the text output buffer.
 */
static void writePOUtf8(POTokenInfo *tokenInfo, unsigned long ch)
{
	if(ch < 0x80)
	{
		PO_ADD_CH((char)ch);
	}
	else if(ch < (1 << 11))
	{
		PO_ADD_CH((char)(0xC0 | ((ch >> 6) & 0x1F)));
		PO_ADD_CH((char)(0x80 | (ch & 0x3F)));
	}
	else if(ch < (1 << 16))
	{
		PO_ADD_CH((char)(0xE0 | ((ch >> 12) & 0x0F)));
		PO_ADD_CH((char)(0x80 | ((ch >> 6) & 0x3F)));
		PO_ADD_CH((char)(0x80 | (ch & 0x3F)));
	}
	else if(ch < (1 << 21))
	{
		PO_ADD_CH((char)(0xF0 | ((ch >> 18) & 0x07)));
		PO_ADD_CH((char)(0x80 | ((ch >> 12) & 0x3F)));
		PO_ADD_CH((char)(0x80 | ((ch >> 6) & 0x3F)));
		PO_ADD_CH((char)(0x80 | (ch & 0x3F)));
	}
	else if(ch < (1 << 26))
	{
		PO_ADD_CH((char)(0xF8 | ((ch >> 24) & 0x03)));
		PO_ADD_CH((char)(0x80 | ((ch >> 18) & 0x3F)));
		PO_ADD_CH((char)(0x80 | ((ch >> 12) & 0x3F)));
		PO_ADD_CH((char)(0x80 | ((ch >> 6) & 0x3F)));
		PO_ADD_CH((char)(0x80 | (ch & 0x3F)));
	}
	else
	{
		PO_ADD_CH((char)(0xFC | ((ch >> 30) & 0x03)));
		PO_ADD_CH((char)(0x80 | ((ch >> 24) & 0x3F)));
		PO_ADD_CH((char)(0x80 | ((ch >> 18) & 0x3F)));
		PO_ADD_CH((char)(0x80 | ((ch >> 12) & 0x3F)));
		PO_ADD_CH((char)(0x80 | ((ch >> 6) & 0x3F)));
		PO_ADD_CH((char)(0x80 | (ch & 0x3F)));
	}
}

/*
 * Parse a hexadecimal escape.
 */
static int parsePOHex(POTokenInfo *tokenInfo, int hexlen)
{
	int ch;
	unsigned long finalch;

	/* Parse the hexadecimal representation of the character */
	finalch = 0;
	ch = getc(tokenInfo->stream);
	while(hexlen > 0)
	{
		if(ch >= '0' && ch <= '9')
		{
			finalch = (finalch * 16) + (ch - '0');
		}
		else if(ch >= 'A' && ch <= 'F')
		{
			finalch = (finalch * 16) + (ch - 'A' + 10);
		}
		else if(ch >= 'a' && ch <= 'f')
		{
			finalch = (finalch * 16) + (ch - 'a' + 10);
		}
		else
		{
			break;
		}
		ch = getc(tokenInfo->stream);
		--hexlen;
	}

	/* Add the UTF-8 form of "finalch" to the text string buffer */
	writePOUtf8(tokenInfo, finalch);

	/* Return the next character from the input stream */
	return ch;
}

/*
 * Get the next token from a .po input stream.
 */
static void nextPOToken(POTokenInfo *tokenInfo, int latin1)
{
	int ch, fuzzy;
	unsigned long octalch;

	/* We stop reading if we have already seen EOF or an error */
	if(tokenInfo->token == PO_TOKEN_EOF ||
	   tokenInfo->token == PO_TOKEN_ERROR)
	{
		return;
	}
	else if(tokenInfo->sawEOF)
	{
		tokenInfo->token = PO_TOKEN_EOF;
		return;
	}

	/* Skip white space and comments in the input */
	ch = getc(tokenInfo->stream);
	while(ch != EOF && (ILResIsWhiteSpace(ch) || ch == '#'))
	{
		if(ch == '\n')
		{
			++(tokenInfo->linenum);
		}
		if(ch == '#')
		{
			ch = getc(tokenInfo->stream);
			fuzzy = 0;
			if(ch == ',')
			{
				/* This is an option line.  We search for the "fuzzy"
				   option because that indicates that we should ignore
				   the message string that follows as it isn't properly
				   translated yet */
				char options[256];
				int posn = 0;
				ch = getc(tokenInfo->stream);
				while(ch != EOF && ch != '\n')
				{
					if(posn < sizeof(options))
					{
						options[posn++] = (char)ch;
					}
					ch = getc(tokenInfo->stream);
				}
				while(posn >= 5)
				{
					if(!ILMemCmp(options + posn - 5, "fuzzy", 5))
					{
						fuzzy = 1;
						break;
					}
					--posn;
				}
			}
			while(ch != EOF && ch != '\n')
			{
				ch = getc(tokenInfo->stream);
			}
			if(ch == EOF)
			{
				break;
			}
			if(fuzzy)
			{
				tokenInfo->posn = 0;
				tokenInfo->token = PO_TOKEN_FUZZY;
				return;
			}
		}
		else
		{
			ch = getc(tokenInfo->stream);
		}
	}
	if(ch == EOF)
	{
		tokenInfo->sawEOF = 1;
		tokenInfo->token = PO_TOKEN_EOF;
		return;
	}

	/* Is this a string or a keyword? */
	if(ch == '"')
	{
		/* Parse a literal string value */
		tokenInfo->posn = 0;
		ch = getc(tokenInfo->stream);
		while(ch != EOF && ch != '"')
		{
			if(ch == '\\')
			{
				ch = getc(tokenInfo->stream);
				switch(ch)
				{
					case 'n': PO_ADD_CH('\n'); break;
					case 'r': PO_ADD_CH('\r'); break;
					case 'f': PO_ADD_CH('\f'); break;
					case 't': PO_ADD_CH('\t'); break;
					case 'v': PO_ADD_CH('\v'); break;

					case 'x': case 'X':
					{
						/* Hexadecimal character */
						ch = parsePOHex(tokenInfo, 8);
						continue;
					}
					/* Not reached */

					case '0': case '1': case '2': case '3':
					case '4': case '5': case '6': case '7':
					{
						/* Octal character */
						octalch = (unsigned long)(ch - '0');
						ch = getc(tokenInfo->stream);
						if(ch >= '0' && ch <= '7')
						{
							octalch = octalch * 8 +
									  ((unsigned long)(ch - '0'));
							ch = getc(tokenInfo->stream);
						}
						if(ch >= '0' && ch <= '7')
						{
							octalch = octalch * 8 +
									  ((unsigned long)(ch - '0'));
							ch = getc(tokenInfo->stream);
						}
						writePOUtf8(tokenInfo, octalch);
						continue;
					}
					/* Not reached */

					case EOF: break;

					default:
					{
						/* Quoted character */
						PO_ADD_CH(ch);
					}
					break;
				}
				if(ch != EOF)
				{
					ch = getc(tokenInfo->stream);
				}
			}
			else if(ch == '\n')
			{
				++(tokenInfo->linenum);
				PO_ADD_CH(ch);
				ch = getc(tokenInfo->stream);
			}
			else if(latin1)
			{
				/* Convert the latin1 character into UTF-8 */
				if(ch < 0x80)
				{
					PO_ADD_CH(ch);
				}
				else
				{
					PO_ADD_CH(0xC0 | ((ch >> 6) & 0x3F));
					PO_ADD_CH(0x80 | (ch & 0x3F));
				}
			}
			else
			{
				PO_ADD_CH(ch);
				ch = getc(tokenInfo->stream);
			}
		}
		if(ch == '"')
		{
			PO_ADD_CH('\0');
			--(tokenInfo->posn);
			tokenInfo->token = PO_TOKEN_STRING;
		}
		else
		{
			fprintf(stderr, "%s:%ld: end of file encountered inside string",
					tokenInfo->filename, tokenInfo->linenum);
			tokenInfo->token = PO_TOKEN_ERROR;
			tokenInfo->sawEOF = 1;
		}
		return;
	}
	else if(ch == 'm')
	{
		/* Parse a keyword, which should be "msgid" or "msgstr" */
		tokenInfo->posn = 0;
		PO_ADD_CH('m');
		ch = getc(tokenInfo->stream);
		while(ch != EOF && ch >= 'a' && ch <= 'z' && tokenInfo->posn < 16)
		{
			PO_ADD_CH(ch);
			ch = getc(tokenInfo->stream);
		}
		if(ch != EOF)
		{
			ungetc(ch, tokenInfo->stream);
		}
		else
		{
			tokenInfo->sawEOF = 1;
		}
		PO_ADD_CH('\0');
		if(!strcmp(tokenInfo->text, "msgid"))
		{
			tokenInfo->token = PO_TOKEN_MSGID;
			return;
		}
		else if(!strcmp(tokenInfo->text, "msgstr"))
		{
			tokenInfo->token = PO_TOKEN_MSGSTR;
			return;
		}
	}

	/* If we get here, then an error has occurred */
	fprintf(stderr, "%s:%ld: unknown token in .po input stream\n",
			tokenInfo->filename, tokenInfo->linenum);
	tokenInfo->token = PO_TOKEN_ERROR;
}

/*
 * Get a string value from a .po input stream.  This may
 * involve appending multiple strings into a single result.
 */
static char *getPOString(POTokenInfo *tokenInfo, int *len, int latin1)
{
	char *result;
	int resultLen;
	int resultMax;
	char *newResult;

	/* Save the current text buffer */
	result = tokenInfo->text;
	resultLen = tokenInfo->posn;
	resultMax = tokenInfo->len;
	tokenInfo->text = 0;
	tokenInfo->len = 0;

	/* Collect up any other strings that are appended to the first */
	nextPOToken(tokenInfo, latin1);
	while(tokenInfo->token == PO_TOKEN_STRING)
	{
		if((resultLen + tokenInfo->posn) > resultMax)
		{
			newResult = (char *)ILRealloc(result, resultLen + tokenInfo->posn);
			if(!newResult)
			{
				ILResOutOfMemory();
			}
			result = newResult;
			resultMax = resultLen + tokenInfo->posn;
		}
		if(tokenInfo->posn > 0)
		{
			ILMemCpy(result + resultLen, tokenInfo->text, tokenInfo->posn);
			resultLen += tokenInfo->posn;
		}
		nextPOToken(tokenInfo, latin1);
	}

	/* Return the results to the caller */
	*len = resultLen;
	return result;
}

int ILResLoadPO(const char *filename, FILE *stream, int latin1)
{
	POTokenInfo tokenInfo;
	char *msgid;
	int msgidLen;
	char *msgstr;
	int msgstrLen;
	long linenum;
	int haveAddError;
	int fuzzy;

	/* Initialize the token information block and read the first token */
	tokenInfo.stream = stream;
	tokenInfo.sawEOF = 0;
	tokenInfo.token = PO_TOKEN_START;
	tokenInfo.text = 0;
	tokenInfo.posn = 0;
	tokenInfo.len = 0;
	tokenInfo.filename = filename;
	tokenInfo.linenum = 1;
	nextPOToken(&tokenInfo, latin1);

	/* Parse strings from the input stream */
	haveAddError = 0;
	fuzzy = 0;
	while(tokenInfo.token == PO_TOKEN_MSGID ||
	      tokenInfo.token == PO_TOKEN_FUZZY)
	{
		/* Process "fuzzy" markers */
		if(tokenInfo.token == PO_TOKEN_FUZZY)
		{
			fuzzy = 1;
			nextPOToken(&tokenInfo, latin1);
			continue;
		}

		/* Get the message identifier */
		nextPOToken(&tokenInfo, latin1);
		if(tokenInfo.token != PO_TOKEN_STRING)
		{
			if(tokenInfo.token != PO_TOKEN_ERROR)
			{
				fprintf(stderr, "%s:%ld: message identifier expected\n",
						tokenInfo.filename, tokenInfo.linenum);
				tokenInfo.token = PO_TOKEN_ERROR;
			}
			break;
		}
		msgid = getPOString(&tokenInfo, &msgidLen, latin1);

		/* Check for the "msgstr" keyword */
		if(tokenInfo.token != PO_TOKEN_MSGSTR)
		{
			if(tokenInfo.token != PO_TOKEN_ERROR)
			{
				fprintf(stderr, "%s:%ld: `msgstr' expected\n",
						tokenInfo.filename, tokenInfo.linenum);
				tokenInfo.token = PO_TOKEN_ERROR;
			}
			ILFree(msgid);
			break;
		}

		/* Get the message string */
		nextPOToken(&tokenInfo, latin1);
		if(tokenInfo.token != PO_TOKEN_STRING)
		{
			if(tokenInfo.token != PO_TOKEN_ERROR)
			{
				fprintf(stderr, "%s:%ld: message string expected\n",
						tokenInfo.filename, tokenInfo.linenum);
				tokenInfo.token = PO_TOKEN_ERROR;
			}
			ILFree(msgid);
			break;
		}
		linenum = tokenInfo.linenum;
		msgstr = getPOString(&tokenInfo, &msgstrLen, latin1);

		/* Add a new resource to the global hash table */
		if(!fuzzy)
		{
			if(ILResAddResource(filename, linenum,
								msgid, msgidLen, msgstr, msgstrLen))
			{
				haveAddError = 1;
			}
		}
		else
		{
			/* This string is ignored because it is "fuzzy" */
			fuzzy = 0;
		}

		/* Free the temporary strings */
		ILFree(msgid);
		ILFree(msgstr);
	}
	if(tokenInfo.token != PO_TOKEN_EOF &&
	   tokenInfo.token != PO_TOKEN_ERROR)
	{
		fprintf(stderr, "%s:%ld: `msgid' expected\n",
				tokenInfo.filename, tokenInfo.linenum);
		tokenInfo.token = PO_TOKEN_ERROR;
	}

	/* Clean up and exit */
	if(tokenInfo.text)
	{
		ILFree(tokenInfo.text);
	}
	return (tokenInfo.token == PO_TOKEN_ERROR || haveAddError);
}

/*
 * Write a quoted string to a .po file.
 */
static void writePOString(FILE *stream, const char *str, int len, int latin1)
{
	int posn;
	unsigned long ch;
	int needEscape;
	int lineLen;

	/* Flush down to the next line if the string is long */
	if(len >= 60)
	{
		fputs("\"\"\n\"", stream);
	}
	else
	{
		putc('"', stream);
	}

	/* Write out the contents of the string */
	posn = 0;
	needEscape = 0;
	lineLen = 0;
	while(posn < len)
	{
		ch = ILUTF8ReadChar(str, len, &posn);
		switch(ch)
		{
			case '\n':
			{
				if(posn < len)
				{
					fputs("\\n\"\n\"", stream);
					lineLen = 0;
				}
				else
				{
					fputs("\\n", stream);
					lineLen += 2;
				}
				needEscape = 0;
			}
			break;

			case '\r':
			{
				fputs("\\r", stream);
				needEscape = 0;
				lineLen += 2;
			}
			break;

			case '\t':
			{
				fputs("\\t", stream);
				needEscape = 0;
				lineLen += 2;
			}
			break;

			case '\f':
			{
				fputs("\\f", stream);
				needEscape = 0;
				lineLen += 2;
			}
			break;

			case '\v':
			{
				fputs("\\v", stream);
				needEscape = 0;
				lineLen += 2;
			}
			break;

			case '"':
			{
				fputs("\\\"", stream);
				needEscape = 0;
				lineLen += 2;
			}
			break;

			case '\\':
			{
				fputs("\\\\", stream);
				needEscape = 0;
				lineLen += 2;
			}
			break;

			default:
			{
				if(ch < 0x20 || (ch >= 0x7F && ch <= 0xFF))
				{
					fprintf(stream, "\\x%02lX", ch);
					needEscape = 1;
					lineLen += 4;
				}
				else if(ch < 0x80)
				{
					if(needEscape &&
					   ((ch >= '0' && ch <= '9') ||
					    (ch >= 'A' && ch <= 'F') ||
						(ch >= 'a' && ch <= 'f')))
					{
						fprintf(stream, "\\x%02lX", ch);
						lineLen += 4;
						needEscape = 1;
					}
					else
					{
						putc((int)ch, stream);
						++lineLen;
						needEscape = 0;
					}
				}
				else if(latin1 && ch < 0x0100)
				{
					putc((int)ch, stream);
					++lineLen;
					needEscape = 0;
				}
				else if(ch < (unsigned long)0x10000)
				{
					fprintf(stream, "\\x%04lX", ch);
					needEscape = 1;
					lineLen += 6;
				}
				else
				{
					fprintf(stream, "\\x%08lX", ch);
					needEscape = 1;
					lineLen += 10;
				}
			}
			break;
		}
		if(lineLen >= 60 && posn < len)
		{
			fputs("\"\n\"", stream);
			lineLen = 0;
		}
	}

	/* Terminate the string */
	fputs("\"\n", stream);
}

/*
 * Write a single hash entry to an output stream as .po data.
 */
static void writePOEntry(FILE *stream, ILResHashEntry *entry, int latin1)
{
	fputs("msgid ", stream);
	writePOString(stream, entry->data, entry->nameLen, latin1);
	fputs("msgstr ", stream);
	writePOString(stream, entry->data + entry->nameLen,
				  entry->valueLen, latin1);
}

/*
 * Write the ".po" header.
 */
static void writePOHeader(FILE *stream, int latin1)
{
	fputs("msgid \"\"\n", stream);
	fputs("msgstr \"\"\n", stream);
	fputs("\"MIME-Version: 1.0\\n\"\n", stream);
	fputs("\"Content-Type: text/plain; charset=", stream);
	if(latin1)
	{
		fputs("ISO-8859-1", stream);
	}
	else
	{
		fputs("UTF-8", stream);
	}
	fputs("\\n\"\n", stream);
	fputs("\"Content-Transfer-Encoding: 8bit\\n\"\n", stream);
	fputs("\n", stream);
}

void ILResWritePO(FILE *stream, int latin1)
{
	int hash;
	ILResHashEntry *entry;
	int first = 1;
	writePOHeader(stream, latin1);
	for(hash = 0; hash < IL_RES_HASH_TABLE_SIZE; ++hash)
	{
		entry = ILResHashTable[hash];
		while(entry != 0)
		{
			if(first)
			{
				first = 0;
			}
			else
			{
				putc('\n', stream);
			}
			writePOEntry(stream, entry, latin1);
			entry = entry->next;
		}
	}
}

void ILResWriteSortedPO(FILE *stream, int latin1)
{
	ILResHashEntry **table;
	unsigned long posn;

	/* Write the PO header */
	writePOHeader(stream, latin1);

	/* Sort the hash table */
	table = ILResCreateSortedArray();

	/* Write out the entries */
	for(posn = 0; posn < ILResNumStrings; ++posn)
	{
		if(posn > 0)
		{
			putc('\n', stream);
		}
		writePOEntry(stream, table[posn], latin1);
	}

	/* Free the table */
	if(table)
	{
		ILFree(table);
	}
}

#ifdef	__cplusplus
};
#endif
