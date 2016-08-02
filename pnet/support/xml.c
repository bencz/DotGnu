/*
 * xml.c - Light-weight routines for reading XML documents.
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

#include "il_xml.h"
#include "il_system.h"

/*

This XML parser assumes that the input is well-formed, that DTD
validation is not required, and that the input is NOT encoded using
UCS-2, UCS-4, EBCDIC, or some other stupid character encoding
(UTF-8 or a byte-based character encoding is preferred).  If this
is not the case, the results will be unexpected.

*/

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * XML reader control data structure.
 */
#define	IL_XML_BUFFER_SIZE	1024
struct _tagILXMLReader
{
	ILXMLReadFunc		readFunc;
	void			   *data;
	char				buffer[IL_XML_BUFFER_SIZE];
	int					bufPosn, bufLen;
	char			   *text;
	int					textLen, textMax;
	int					withoutNamespace;
	ILXMLItem			currentItem;
	int					whiteSpace : 1;
	int					fixedTextMax : 1;
	int					sawEOF : 1;
};

/*
 * Get the next character from an XML input stream.
 */
static int XMLGetCh(ILXMLReader *reader)
{
	if(!(reader->sawEOF))
	{
		/* Read the next buffer from the input stream */
		reader->bufLen = (*(reader->readFunc))(reader->data, reader->buffer,
									           IL_XML_BUFFER_SIZE);
		reader->bufPosn = 0;
		if(reader->bufPosn < reader->bufLen)
		{
			return (((int)(reader->buffer[(reader->bufPosn)++])) & 0xFF);
		}
		else
		{
			/* We have reached EOF, or a read error occurred */
			reader->sawEOF = 1;
			return -1;
		}
	}
	else
	{
		/* We are already at EOF */
		return -1;
	}
}
#define	XML_GETCH(ch)	\
			do { \
				if(reader->bufPosn < reader->bufLen) \
				{ \
					(ch) = (((int)(reader->buffer \
									[(reader->bufPosn)++])) & 0xFF); \
				} \
				else \
				{ \
					(ch) = XMLGetCh(reader); \
				} \
			} while (0)

/*
 * Unget the last character that was read.
 */
#define	XML_UNGETCH()	\
			do { \
				--(reader->bufPosn); \
			} while (0)

/*
 * Determine if a character looks like white space.  We deliberately
 * avoid using "isspace" because it doesn't work properly with 8-bit
 * characters on some systems.
 */
#define	XML_ISSPACE(ch)		\
			((ch) == ' ' || (ch) == '\t' || (ch) == '\r' || \
			 (ch) == '\n' || (ch) == '\f' || (ch) == '\v')

/*
 * Reset the text output buffer.
 */
#define	TEXT_RESET()	\
			do { \
				reader->textLen = 0; \
			} while (0)

/*
 * Add a character to the text output buffer.
 */
static void TextAddCh(ILXMLReader *reader, int ch)
{
	if(!(reader->fixedTextMax))
	{
		/* We leave one extra character for the terminating NUL */
		char *newText = (char *)ILRealloc(reader->text, reader->textMax + 257);
		if(newText)
		{
			reader->text = newText;
			reader->textMax += 256;
			reader->text[(reader->textLen)++] = (char)ch;
		}
	}
}
#define	TEXT_ADDCH(ch)	\
			do { \
				if(reader->textLen < reader->textMax) \
				{ \
					reader->text[(reader->textLen)++] = (char)(ch); \
				} \
				else \
				{ \
					TextAddCh(reader, (ch)); \
				} \
			} while (0)

/*
 * Finalize the text output buffer.
 */
#define	TEXT_FINALIZE()	\
			do { \
				reader->text[reader->textLen] = '\0'; \
			} while (0)

ILXMLReader *ILXMLCreate(ILXMLReadFunc readFunc, void *data, int maxText)
{
	ILXMLReader *reader;

	/* Allocate and initialize the reader */
	if((reader = (ILXMLReader *)ILMalloc(sizeof(ILXMLReader))) == 0)
	{
		return 0;
	}
	reader->readFunc = readFunc;
	reader->data = data;
	reader->bufPosn = 0;
	reader->bufLen = 0;
	reader->text = 0;
	reader->textLen = 0;
	reader->withoutNamespace = 0;
	reader->currentItem = ILXMLItem_EOF;
	reader->whiteSpace = 0;
	reader->sawEOF = 0;

	/* Allocate the initial text buffer */
	if(maxText > 0)
	{
		if((reader->text = (char *)ILMalloc(maxText)) == 0)
		{
			ILFree(reader);
			return 0;
		}
		reader->textMax = maxText - 1;
		reader->fixedTextMax = 1;
	}
	else
	{
		if((reader->text = (char *)ILMalloc(257)) == 0)
		{
			ILFree(reader);
			return 0;
		}
		reader->textMax = 256;
		reader->fixedTextMax = 0;
	}

	/* Ready to go */
	return reader;
}

void ILXMLDestroy(ILXMLReader *reader)
{
	if(reader->text)
	{
		ILFree(reader->text);
	}
	ILFree(reader);
}

/*
 * Parse a named entity.
 */
static int ParseEntity(ILXMLReader *reader)
{
	int ch, ch2;
	char name[16];
	int namelen = 0;
	XML_GETCH(ch);
	while(ch != -1 && ch != ';')
	{
		if(ch != 0 && namelen < 15)
		{
			name[namelen++] = (char)ch;
		}
		XML_GETCH(ch);
	}
	name[namelen] = '\0';
	if(!strcmp(name, "lt"))
	{
		return '<';
	}
	else if(!strcmp(name, "gt"))
	{
		return '>';
	}
	else if(!strcmp(name, "amp"))
	{
		return '&';
	}
	else if(!strcmp(name, "apos"))
	{
		return '\'';
	}
	else if(!strcmp(name, "quot"))
	{
		return '"';
	}
	else if(name[0] == '#' && name[1] == 'x')
	{
		ch = 0;
		namelen = 2;
		while((ch2 = name[namelen++]) != '\0')
		{
			ch <<= 4;
			if(ch2 >= '0' && ch2 <= '9')
			{
				ch += (ch2 - '0');
			}
			else if(ch2 >= 'A' && ch2 <= 'F')
			{
				ch += (ch2 - 'A' + 10);
			}
			else if(ch2 >= 'a' && ch2 <= 'f')
			{
				ch += (ch2 - 'a' + 10);
			}
			else
			{
				break;
			}
		}
		return ch;
	}
	else if(name[0] == '#')
	{
		ch = 0;
		namelen = 1;
		while((ch2 = name[namelen++]) != '\0')
		{
			ch *= 10;
			if(ch2 >= '0' && ch2 <= '9')
			{
				ch += (ch2 - '0');
			}
			else
			{
				break;
			}
		}
		return ch;
	}
	else
	{
		return 0;
	}
}

ILXMLItem ILXMLReadNext(ILXMLReader *reader)
{
	int ch, quote;
	unsigned level;
	XML_GETCH(ch);
	while(ch != -1)
	{
		if(ch == '<')
		{
			/* Start of a tag of some kind */
			XML_GETCH(ch);
			if(ch == '/')
			{
				/* End tag */
				TEXT_RESET();
				XML_GETCH(ch);
				reader->withoutNamespace = 0;
				while(ch != -1 && ch != '>' && !XML_ISSPACE(ch))
				{
					if(ch != 0)
					{
						TEXT_ADDCH(ch);
						if(ch == ':')
						{
							reader->withoutNamespace = reader->textLen;
						}
					}
					XML_GETCH(ch);
				}
				while(ch != -1 && ch != '>')
				{
					/* Skip the rest of the tag after the name,
					   which we don't care about */
					XML_GETCH(ch);
				}
				TEXT_FINALIZE();
				reader->currentItem = ILXMLItem_EndTag;
				return ILXMLItem_EndTag;
			}
			else if(ch == '?')
			{
				/* Processing instruction: skip it */
				XML_GETCH(ch);
				while(ch != -1 && ch != '>')
				{
					XML_GETCH(ch);
				}
				XML_GETCH(ch);
			}
			else if(ch == '!')
			{
				/* Comment or document type definition command: skip it */
				XML_GETCH(ch);
				if(ch == '-')
				{
					XML_GETCH(ch);
					if(ch == '-')
					{
						/* Skip the comment */
						XML_GETCH(ch);
						while(ch != -1)
						{
							if(ch == '-')
							{
								XML_GETCH(ch);
								while(ch == '-')
								{
									XML_GETCH(ch);
									if(ch == '>')
									{
										ch = -2;
										break;
									}
								}
								if(ch == -2)
								{
									break;
								}
							}
							XML_GETCH(ch);
						}
						XML_GETCH(ch);
						continue;
					}
				}

				/* Probably some kind of DTD definition tag */
				level = 0;
				while(ch != -1 && (ch != '>' || level > 0))
				{
					if(ch == '[')
					{
						++level;
					}
					else if(ch == ']' && level > 0)
					{
						--level;
					}
					XML_GETCH(ch);
				}
				XML_GETCH(ch);
			}
			else
			{
				/* Start or singleton tag: parse the name */
				TEXT_RESET();
				reader->withoutNamespace = 0;
				while(ch != -1 && ch != '>' && ch != '/' && !XML_ISSPACE(ch))
				{
					if(ch != 0)
					{
						TEXT_ADDCH(ch);
						if(ch == ':')
						{
							reader->withoutNamespace = reader->textLen;
						}
					}
					XML_GETCH(ch);
				}
				TEXT_ADDCH('\0');

				/* Parse the tag's parameters */
				while(ch != -1 && ch != '>' && ch != '/')
				{
					/* Skip white space before the next parameter */
					while(XML_ISSPACE(ch))
					{
						XML_GETCH(ch);
					}
					if(ch == -1 || ch == '>' || ch == '/')
					{
						break;
					}

					/* Parse the parameter name */
					while(ch != -1 && ch != '>' && ch != '/' && ch != '=')
					{
						if(!XML_ISSPACE(ch) && ch != 0)
						{
							TEXT_ADDCH(ch);
						}
						XML_GETCH(ch);
					}
					TEXT_ADDCH('\0');

					/* Parse the parameter value */
					if(ch == '=')
					{
						do
						{
							XML_GETCH(ch);
						}
						while(XML_ISSPACE(ch));
						if(ch == '"' || ch == '\'')
						{
							quote = ch;
							XML_GETCH(ch);
							while(ch != -1 && ch != '>' && ch != quote)
							{
								if(ch != '&')
								{
									if(ch != 0)
									{
										TEXT_ADDCH(ch);
									}
								}
								else
								{
									ch = ParseEntity(reader);
									if(ch != 0)
									{
										TEXT_ADDCH(ch);
									}
								}
								XML_GETCH(ch);
							}
							if(ch == quote)
							{
								XML_GETCH(ch);
							}
						}
					}
					TEXT_ADDCH('\0');
				}

				/* Terminate the parameter list */
				TEXT_FINALIZE();

				/* Parse the end of the tag */
				if(ch == '/')
				{
					do
					{
						XML_GETCH(ch);
					}
					while(ch != -1 && ch != '>');
					reader->currentItem = ILXMLItem_SingletonTag;
					return ILXMLItem_SingletonTag;
				}
				else
				{
					reader->currentItem = ILXMLItem_StartTag;
					return ILXMLItem_StartTag;
				}
			}
		}
		else if(reader->whiteSpace || !XML_ISSPACE(ch))
		{
			/* Start of a text block, which may include entities */
			TEXT_RESET();
			TEXT_ADDCH(ch);
			XML_GETCH(ch);
			while(ch != -1 && ch != '<')
			{
				if(reader->fixedTextMax &&
				   reader->textLen >= reader->textMax)
				{
					/* Split large text blocks if we are using
					   a fixed-sized buffer */
					break;
				}
				if(ch != '&')
				{
					/* Ordinary character */
					if(ch != 0)
					{
						TEXT_ADDCH(ch);
					}
				}
				else
				{
					/* Parse a named entity */
					ch = ParseEntity(reader);
					if(ch != 0)
					{
						TEXT_ADDCH(ch);
					}
				}
				XML_GETCH(ch);
			}
			if(ch != -1)
			{
				XML_UNGETCH();
			}
			TEXT_FINALIZE();
			reader->currentItem = ILXMLItem_Text;
			return ILXMLItem_Text;
		}
		else
		{
			/* Skip non-significant white space */
			do
			{
				XML_GETCH(ch);
			}
			while(XML_ISSPACE(ch));
		}
	}
	reader->currentItem = ILXMLItem_EOF;
	return ILXMLItem_EOF;
}

ILXMLItem ILXMLGetItem(ILXMLReader *reader)
{
	return reader->currentItem;
}

const char *ILXMLTagName(ILXMLReader *reader)
{
	if(reader->currentItem == ILXMLItem_StartTag ||
	   reader->currentItem == ILXMLItem_SingletonTag ||
	   reader->currentItem == ILXMLItem_EndTag)
	{
		return reader->text + reader->withoutNamespace;
	}
	else
	{
		return 0;
	}
}

const char *ILXMLTagNameWithNS(ILXMLReader *reader)
{
	if(reader->currentItem == ILXMLItem_StartTag ||
	   reader->currentItem == ILXMLItem_SingletonTag ||
	   reader->currentItem == ILXMLItem_EndTag)
	{
		return reader->text;
	}
	else
	{
		return 0;
	}
}

int ILXMLIsStartTag(ILXMLReader *reader, const char *name)
{
	if(reader->currentItem == ILXMLItem_StartTag)
	{
		/* Check the name directly */
		if(!strcmp(reader->text, name))
		{
			return 1;
		}

		/* If the tag has a namespace, then strip it and retry */
		if(reader->withoutNamespace)
		{
			return (strcmp(reader->text + reader->withoutNamespace, name) == 0);
		}
		else
		{
			return 0;
		}
	}
	else
	{
		return 0;
	}
}

int ILXMLIsTag(ILXMLReader *reader, const char *name)
{
	if(reader->currentItem == ILXMLItem_StartTag ||
	   reader->currentItem == ILXMLItem_SingletonTag)
	{
		/* Check the name directly */
		if(!strcmp(reader->text, name))
		{
			return 1;
		}

		/* If the tag has a namespace, then strip it and retry */
		if(reader->withoutNamespace)
		{
			return (strcmp(reader->text + reader->withoutNamespace, name) == 0);
		}
		else
		{
			return 0;
		}
	}
	else
	{
		return 0;
	}
}

/*
 * Find the next string in a reader's text buffer.
 */
static char *NextString(ILXMLReader *reader, char *prev)
{
	int posn = (int)(prev - reader->text);
	while(posn < reader->textLen && reader->text[posn] != '\0')
	{
		++posn;
	}
	if(posn < reader->textLen)
	{
		return reader->text + posn + 1;
	}
	else
	{
		/* Return a pointer to the NUL at the end of the buffer */
		return reader->text + reader->textLen;
	}
}

const char *ILXMLGetParam(ILXMLReader *reader, const char *name)
{
	if(reader->currentItem == ILXMLItem_StartTag ||
	   reader->currentItem == ILXMLItem_SingletonTag)
	{
		char *str = NextString(reader, reader->text);
		while(*str != '\0')
		{
			if(!strcmp(str, name))
			{
				return NextString(reader, str);
			}
			str = NextString(reader, NextString(reader, str));
		}
		return 0;
	}
	else
	{
		return 0;
	}
}

const char *ILXMLGetText(ILXMLReader *reader)
{
	if(reader->currentItem == ILXMLItem_Text)
	{
		return reader->text;
	}
	else
	{
		return 0;
	}
}

char *ILXMLGetContents(ILXMLReader *reader, int whiteSpace)
{
	int oldWhite;
	char *value;
	int valueLen;
	char *newValue;
	ILXMLItem item;
	unsigned long depth;

	/* Bail if immediately if we are not positioned on a start tag */
	if(reader->currentItem != ILXMLItem_StartTag)
	{
		return ILDupString("");
	}

	/* Append the text items within the element, skipping other item kinds */
	value = 0;
	valueLen = 0;
	depth = 0;
	oldWhite = reader->whiteSpace;
	reader->whiteSpace = whiteSpace;
	while((item = ILXMLReadNext(reader)) != ILXMLItem_EOF)
	{
		if(item == ILXMLItem_StartTag)
		{
			++depth;
		}
		else if(item == ILXMLItem_EndTag)
		{
			if(!depth)
			{
				break;
			}
			--depth;
		}
		else if(item == ILXMLItem_Text)
		{
			if(!whiteSpace)
			{
				/* Strip white space from the end of the text.
				   ILXMLReadItem has already stripped any white
				   space from the front of the text */
				while(reader->textLen > 0 &&
					  XML_ISSPACE(reader->text[reader->textLen - 1]))
				{
					--(reader->textLen);
				}
			}
			newValue = (char *)ILRealloc(value, valueLen + reader->textLen + 1);
			if(!newValue)
			{
				if(value)
				{
					ILFree(value);
				}
				reader->whiteSpace = oldWhite;
				return 0;
			}
			value = newValue;
			ILMemCpy(value + valueLen, reader->text, reader->textLen);
			valueLen += reader->textLen;
		}
	}

	/* Finalize the return buffer and exit */
	reader->whiteSpace = oldWhite;
	if(value)
	{
		value[valueLen] = '\0';
		return value;
	}
	else
	{
		return ILDupString("");
	}
}

void ILXMLSkip(ILXMLReader *reader)
{
	if(reader->currentItem == ILXMLItem_StartTag)
	{
		unsigned long depth = 0;
		ILXMLItem item;
		while((item = ILXMLReadNext(reader)) != ILXMLItem_EOF)
		{
			if(item == ILXMLItem_StartTag)
			{
				++depth;
			}
			else if(item == ILXMLItem_EndTag)
			{
				if(!depth)
				{
					break;
				}
				else
				{
					--depth;
				}
			}
		}
	}
}

void ILXMLWhiteSpace(ILXMLReader *reader, int flag)
{
	reader->whiteSpace = flag;
}

int ILXMLGetPackedSize(ILXMLReader *reader)
{
	return reader->textLen + 1;
}

void ILXMLGetPacked(ILXMLReader *reader, void *buffer)
{
	ILMemCpy(buffer, reader->text, reader->textLen + 1);
}

/*
 * Find the next string in a packed text buffer.
 */
static char *NextPacked(char *buffer, int len, char *prev)
{
	int posn = (int)(prev - buffer);
	while(posn < len && buffer[posn] != '\0')
	{
		++posn;
	}
	if(posn < len)
	{
		return buffer + posn + 1;
	}
	else
	{
		/* Return a pointer to the NUL at the end of the buffer */
		return buffer + len;
	}
}

const char *ILXMLGetPackedParam(void *buffer, int len, const char *name)
{
	char *str;
	--len;	/* Remove trailing NUL before we start */
	str = NextPacked((char *)buffer, len, (char *)buffer);
	while(*str != '\0')
	{
		if(!strcmp(str, name))
		{
			return NextPacked((char *)buffer, len, str);
		}
		str = NextPacked((char *)buffer, len,
						 NextPacked((char *)buffer, len, str));
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
