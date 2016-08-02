/*
 * resgen_text.c - Text resource loading and writing routines.
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
 * Parse a hexadecimal escape.
 */
static unsigned long ParseHex(const char *inbuf, int inlen,
							  int hexlen, int *parsedlen)
{
	char tempch;
	unsigned long ch = 0;
	*parsedlen = 0;
	while(inlen > 0 && hexlen > 0)
	{
		tempch = *inbuf++;
		--inlen;
		if(tempch >= '0' && tempch <= '9')
		{
			ch = (ch * 16) + (tempch - '0');
		}
		else if(tempch >= 'A' && tempch <= 'F')
		{
			ch = (ch * 16) + (tempch - 'A' + 10);
		}
		else if(tempch >= 'a' && tempch <= 'f')
		{
			ch = (ch * 16) + (tempch - 'a' + 10);
		}
		else
		{
			break;
		}
		++(*parsedlen);
		--hexlen;
	}
	return ch;
}

/*
 * Convert escape sequences within a buffer into real characters.
 */
static int ConvertEscapes(const char *inbuf, int inlen,
						  char *outbuf, int latin1)
{
	int outlen = 0;
	unsigned long ch;
	int templen;

	while(inlen > 0)
	{
		if(*inbuf == '\\')
		{
			++inbuf;
			--inlen;
			if(!inlen)
			{
				break;
			}
			switch(*inbuf)
			{
				case 'n':
				{
					*outbuf++ = '\n';
					++outlen;
					++inbuf;
					--inlen;
				}
				break;

				case 'r':
				{
					*outbuf++ = '\r';
					++outlen;
					++inbuf;
					--inlen;
				}
				break;

				case 't':
				{
					*outbuf++ = '\t';
					++outlen;
					++inbuf;
					--inlen;
				}
				break;

				case 'f':
				{
					*outbuf++ = '\f';
					++outlen;
					++inbuf;
					--inlen;
				}
				break;

				case 'v':
				{
					*outbuf++ = '\v';
					++outlen;
					++inbuf;
					--inlen;
				}
				break;

				case '0': case '1': case '2': case '3':
				case '4': case '5': case '6': case '7':
				{
					/* Parse an octal character sequence */
					ch = (*inbuf++ - '0');
					--inlen;
					while(inlen > 0 && *inbuf >= '0' && *inbuf <= '7')
					{
						ch = (ch * 8) + (*inbuf++ - '0');
						--inlen;
					}

					/* Write out the character in UTF-8 form */
				writeUTF8:
					if(ch < 0x80)
					{
						*outbuf++ = (char)ch;
						++outlen;
					}
					else if(ch < (1 << 11))
					{
						*outbuf++ = (char)(0xC0 | ((ch >> 6) & 0x1F));
						*outbuf++ = (char)(0x80 | (ch & 0x3F));
						outlen += 2;
					}
					else if(ch < (1 << 16))
					{
						*outbuf++ = (char)(0xE0 | ((ch >> 12) & 0x0F));
						*outbuf++ = (char)(0x80 | ((ch >> 6) & 0x3F));
						*outbuf++ = (char)(0x80 | (ch & 0x3F));
						outlen += 3;
					}
					else if(ch < (1 << 21))
					{
						*outbuf++ = (char)(0xF0 | ((ch >> 18) & 0x07));
						*outbuf++ = (char)(0x80 | ((ch >> 12) & 0x3F));
						*outbuf++ = (char)(0x80 | ((ch >> 6) & 0x3F));
						*outbuf++ = (char)(0x80 | (ch & 0x3F));
						outlen += 4;
					}
					else if(ch < (1 << 26))
					{
						*outbuf++ = (char)(0xF8 | ((ch >> 24) & 0x03));
						*outbuf++ = (char)(0x80 | ((ch >> 18) & 0x3F));
						*outbuf++ = (char)(0x80 | ((ch >> 12) & 0x3F));
						*outbuf++ = (char)(0x80 | ((ch >> 6) & 0x3F));
						*outbuf++ = (char)(0x80 | (ch & 0x3F));
						outlen += 5;
					}
					else
					{
						*outbuf++ = (char)(0xFC | ((ch >> 30) & 0x03));
						*outbuf++ = (char)(0x80 | ((ch >> 24) & 0x3F));
						*outbuf++ = (char)(0x80 | ((ch >> 18) & 0x3F));
						*outbuf++ = (char)(0x80 | ((ch >> 12) & 0x3F));
						*outbuf++ = (char)(0x80 | ((ch >> 6) & 0x3F));
						*outbuf++ = (char)(0x80 | (ch & 0x3F));
						outlen += 6;
					}
				}
				break;

				case 'x': case 'X':
				{
					/* Parse a 2-digit hexadecimal character sequence */
					++inbuf;
					--inlen;
					ch = ParseHex(inbuf, inlen, 2, &templen);
					inbuf += templen;
					inlen -= templen;
					goto writeUTF8;
				}
				/* Not reached */

				case 'u':
				{
					/* Parse a 4-digit hexadecimal character sequence */
					++inbuf;
					--inlen;
					ch = ParseHex(inbuf, inlen, 4, &templen);
					inbuf += templen;
					inlen -= templen;
					goto writeUTF8;
				}
				/* Not reached */

				case 'U':
				{
					/* Parse an 8-digit hexadecimal character sequence */
					++inbuf;
					--inlen;
					ch = ParseHex(inbuf, inlen, 8, &templen);
					inbuf += templen;
					inlen -= templen;
					goto writeUTF8;
				}
				/* Not reached */

				default:
				{
					/* Regular character that has been escaped */
					*outbuf++ = *inbuf++;
					--inlen;
				}
				break;
			}
		}
		else if(latin1 && (*inbuf & 0x80) != 0)
		{
			/* Convert a Latin-1 character into UTF-8 */
			int tempch = ((*inbuf++) & 0xFF);
			*outbuf++ = (char)(0xC0 | ((tempch >> 6) & 0x3F));
			*outbuf++ = (char)(0x80 | (tempch & 0x3F));
			outlen += 2;
			--inlen;
		}
		else
		{
			*outbuf++ = *inbuf++;
			++outlen;
			--inlen;
		}
	}
	return outlen;
}

int ILResLoadText(const char *filename, FILE *stream, int latin1)
{
	char buffer[4096];
	char buffer2[4096];
	int posn, nameLen, valueLen;
	const char *name;
	const char *value;
	long linenum = 0;
	int error = 0;

	while(fgets(buffer, sizeof(buffer), stream))
	{
		/* Advance the line counter */
		++linenum;

		/* Strip trailing end of line characters */
		posn = strlen(buffer);
		while(posn > 0 &&
		      (buffer[posn - 1] == '\n' || buffer[posn - 1] == '\r'))
		{
			--posn;
		}
		buffer[posn] = '\0';

		/* Skip leading white space */
		posn = 0;
		while(buffer[posn] != '\0' && ILResIsWhiteSpace(buffer[posn]))
		{
			++posn;
		}

		/* Ignore this line if it is empty or a comment */
		if(buffer[posn] == '\0' || buffer[posn] == '#')
		{
			continue;
		}

		/* Parse the line into "name=value" */
		name = buffer + posn;
		nameLen = 0;
		while(buffer[posn] != '\0' && buffer[posn] != '=')
		{
			++nameLen;
			++posn;
		}
		if(buffer[posn] == '\0' || nameLen == 0)
		{
			fprintf(stderr, "%s:%ld: invalid resource format\n",
					filename, linenum);
			error = 1;
			continue;
		}
		++posn;
		value = buffer + posn;
		valueLen = strlen(buffer + posn);

		/* Convert escape sequences in the value into real characters */
		valueLen = ConvertEscapes(value, valueLen, buffer2, latin1);
		value = buffer2;

		/* Add the resource to the hash table */
		error |= ILResAddResource(filename, linenum, name, nameLen,
							      value, valueLen);
	}

	return error;
}

/*
 * Write a single hash entry to an output stream.
 */
static void writeEntry(FILE *stream, ILResHashEntry *entry, int latin1)
{
	const char *str;
	int len, posn;
	unsigned long ch;
	int needEscape;

	fwrite(entry->data, 1, entry->nameLen, stream);
	putc('=', stream);
	str = entry->data + entry->nameLen;
	len = entry->valueLen;
	posn = 0;
	needEscape = 0;
	while(posn < len)
	{
		ch = ILUTF8ReadChar(str, len, &posn);
		switch(ch)
		{
			case '\n':	fputs("\\n", stream); needEscape = 0; break;
			case '\r':	fputs("\\r", stream); needEscape = 0; break;
			case '\t':	fputs("\\t", stream); needEscape = 0; break;
			case '\f':	fputs("\\f", stream); needEscape = 0; break;
			case '\v':	fputs("\\v", stream); needEscape = 0; break;
			case '\\':	fputs("\\\\", stream); needEscape = 0; break;
			case '\0':	fputs("\\0", stream); needEscape = 1; break;

			default:
			{
				if(ch < 0x20 || (ch >= 0x7F && ch <= 0xFF))
				{
					fprintf(stream, "\\x%02lX", ch);
					needEscape = 0;
				}
				else if(ch < 0x80)
				{
					if(needEscape && ch >= '0' && ch <= '7')
					{
						fputs("\\06", stream);
						putc(ch, stream);
					}
					else
					{
						putc((int)ch, stream);
					}
					needEscape = 0;
				}
				else if(latin1 && ch < 0x0100)
				{
					/* Convert a UTF-8 character into Latin-1 */
					putc((int)ch, stream);
					needEscape = 0;
				}
				else if(ch < (unsigned long)0x10000)
				{
					fprintf(stream, "\\u%04lX", ch);
					needEscape = 0;
				}
				else
				{
					fprintf(stream, "\\U%08lX", ch);
					needEscape = 0;
				}
			}
			break;
		}
	}
	putc('\n', stream);
}

void ILResWriteText(FILE *stream, int latin1)
{
	int hash;
	ILResHashEntry *entry;
	for(hash = 0; hash < IL_RES_HASH_TABLE_SIZE; ++hash)
	{
		entry = ILResHashTable[hash];
		while(entry != 0)
		{
			writeEntry(stream, entry, latin1);
			entry = entry->next;
		}
	}
}

void ILResWriteSortedText(FILE *stream, int latin1)
{
	ILResHashEntry **table;
	unsigned long posn;

	/* Sort the hash table */
	table = ILResCreateSortedArray();

	/* Write out the entries */
	for(posn = 0; posn < ILResNumStrings; ++posn)
	{
		writeEntry(stream, table[posn], latin1);
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
