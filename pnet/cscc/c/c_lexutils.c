/*
 * c_lexutils.c - Utility functions for assisting the C lexer.
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

#include <cscc/c/c_internal.h>
#include <cscc/c/c_lexutils.h>

#ifdef	__cplusplus
extern	"C" {
#endif

void CLexLineDirective(const char *text)
{
	int posn;
	char filename[BUFSIZ];
	long linenum;
	while(*text != '\0' && !(*text >= '0' && *text <= '9'))
	{
		++text;
	}
	linenum = 0;
	while(*text != '\0' && *text >= '0' && *text <= '9')
	{
		linenum = (linenum * 10) + (long)(*text - '0');
		++text;
	}
	while(*text != '\0' && *text != '"')
	{
		++text;
	}
	if(*text != '\0')
	{
		++text;
		posn = 0;
		while(*text != '\0' && *text != '"' && posn < (BUFSIZ - 1))
		{
			if(*text == '\\' && text[1] != '\0')
			{
				filename[posn++] = text[1];
				text += 2;
			}
			else
			{
				filename[posn++] = *text++;
			}
		}
	}
	else
	{
		posn = 0;
	}
	CCPreProcessorStream.filename = ILInternString(filename, posn).string;
	CCPreProcessorStream.lineNumber = linenum;
}

void CLexUsingDirective(const char *text)
{
	int endquote, len;
	char *filename;
	while(*text != '\0' && *text != '"' && *text != '<')
	{
		++text;
	}
	if(*text == '"')
	{
		endquote = '"';
	}
	else if(*text == '<')
	{
		endquote = '>';
	}
	else
	{
		return;
	}
	++text;
	len = 0;
	while(text[len] != '\0' && text[len] != endquote)
	{
		++len;
	}
	filename = ILDupNString(text, len);
	if(!filename)
	{
		CCOutOfMemory();
	}
	if(!CCLoadLibrary(filename))
	{
		CCError(_("could not load the assembly `%s'"), filename);
	}
	ILFree(filename);
}

static void InferIntType(const char *text, CLexIntConst *value)
{
	int numl, numu;

	/* Infer a default type for the value */
#if 0
	if(value->value <= (ILUInt64)127)
	{
		value->type = ILMachineType_Int8;
	}
	else if(value->value <= (ILUInt64)255)
	{
		value->type = ILMachineType_UInt8;
	}
	else if(value->value <= (ILUInt64)32767)
	{
		value->type = ILMachineType_Int16;
	}
	else if(value->value <= (ILUInt64)65535)
	{
		value->type = ILMachineType_UInt16;
	}
#endif
	if(value->value <= (ILUInt64)(ILInt64)IL_MAX_INT32)
	{
		value->type = ILMachineType_Int32;
	}
	else if(value->value <= (ILUInt64)IL_MAX_UINT32)
	{
		value->type = ILMachineType_UInt32;
	}
	else if(value->value <= (ILUInt64)IL_MAX_INT64)
	{
		value->type = ILMachineType_Int64;
	}
	else
	{
		value->type = ILMachineType_UInt64;
	}

	/* Parse the type suffixes and determine the final type */
	numu = 0;
	numl = 0;
	while(*text != '\0')
	{
		if(*text == 'u' || *text == 'U')
			++numu;
		else if(*text == 'l' || *text == 'L')
			++numl;
		++text;
	}
	if(numu > 0)
	{
		/* Convert the type into its unsigned counterpart */
		if(value->type == ILMachineType_Int8)
			value->type = ILMachineType_UInt8;
		else if(value->type == ILMachineType_Int16)
			value->type = ILMachineType_UInt16;
		else if(value->type == ILMachineType_Int32)
			value->type = ILMachineType_UInt32;
		else if(value->type == ILMachineType_Int64)
			value->type = ILMachineType_UInt64;
	}
	if(numl >= 2)
	{
		/* Convert the type into its "long long" version */
		if(value->type == ILMachineType_Int8 ||
		   value->type == ILMachineType_Int16 ||
		   value->type == ILMachineType_Int32)
		{
			value->type = ILMachineType_Int64;
		}
		else if(value->type != ILMachineType_UInt64 && !numu)
		{
			value->type = ILMachineType_Int64;
		}
		else
		{
			value->type = ILMachineType_UInt64;
		}
	}
	else if(numl > 0)
	{
		/* Convert the type into its "long" version */
		if(value->type == ILMachineType_Int8 ||
		   value->type == ILMachineType_Int16 ||
		   value->type == ILMachineType_Int32)
		{
			value->type = ILMachineType_NativeInt;
		}
		else if(value->type == ILMachineType_UInt8 ||
		        value->type == ILMachineType_UInt16 ||
		        value->type == ILMachineType_UInt32)
		{
			value->type = ILMachineType_NativeUInt;
		}
	}
}

void CLexParseInt(const char *text, CLexIntConst *value)
{
	/* Initialize the value */
	value->value = (ILUInt64)0;
	value->isneg = 0;

	/* Parse the main part of the integer */
	if(text[0] == '0' && (text[1] == 'x' || text[1] == 'X'))
	{
		/* Hexadecimal constant */
		text += 2;
		while(*text != '\0')
		{
			if(*text >= '0' && *text <= '9')
			{
				value->value = (value->value << 4) + (ILUInt64)(*text - '0');
			}
			else if(*text >= 'A' && *text <= 'F')
			{
				value->value = (value->value << 4) +
							   (ILUInt64)(*text - 'A' + 10);
			}
			else if(*text >= 'a' && *text <= 'f')
			{
				value->value = (value->value << 4) +
							   (ILUInt64)(*text - 'a' + 10);
			}
			else
			{
				break;
			}
			++text;
		}
	}
	else if(text[0] == '0')
	{
		/* Octal constant */
		++text;
		while(*text != '\0')
		{
			if(*text >= '0' && *text <= '7')
			{
				value->value = (value->value << 3) + (ILUInt64)(*text - '0');
			}
			else
			{
				break;
			}
			++text;
		}
	}
	else
	{
		/* Decimal constant */
		while(*text != '\0')
		{
			if(*text >= '0' && *text <= '9')
			{
				value->value = (value->value * 10) + (ILUInt64)(*text - '0');
			}
			else
			{
				break;
			}
			++text;
		}
	}

	/* Infer the type of the integer from the suffixes and its value */
	InferIntType(text, value);
}

void CLexParseFloat(const char *text, CLexFloatConst *value)
{
	char *endptr;
	int numf, numl;

	/* Parse the main part of the value */
#ifdef HAVE_STRTOD
	/* Use "strtod" to parse the value */
	value->value = (ILDouble)(strtod(text, &endptr));
#else
	/* Use "sscanf" to parse the value */
	double result;
	int num;
	if(sscanf(text, "%lf%n", &result, &num) <= 0)
	{
		/* Shouldn't happen, but do something reasonable */
		value->value = (ILDouble)0.0;
		value->type = ILMachineType_Float64;
		return;
	}
	value->value = (ILDouble)result;
	endptr = text + num;
#endif

	/* Process the suffixes */
	numf = 0;
	numl = 0;
	while(*endptr != '\0')
	{
		if(*endptr == 'f' || *endptr == 'F')
			++numf;
		else if(*endptr == 'l' || *endptr == 'L')
			++numl;
		++endptr;
	}
	if(numf > 0)
	{
		value->value = (ILDouble)(ILFloat)(value->value);
		value->type = ILMachineType_Float32;
	}
	else if(numl > 0)
	{
		value->type = ILMachineType_Float64;
	}
	else if(((ILDouble)(ILFloat)(value->value)) == value->value)
	{
		value->type = ILMachineType_Float32;
	}
	else
	{
		value->type = ILMachineType_Float64;
	}
}

static int ParseEscape(const char *temp, ILUInt64 *value, int *isUnicode)
{
	const char *begin = temp;
	static char escapechars[] =
		"\a\bcd\033\fghijklm\nopq\rs\tu\vwxyz";

	*isUnicode = 0;
	if(*temp == 'x')
	{
		++temp;
		*value = (ILUInt64)0;
		if(*temp >= '0' && *temp <= '9')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - '0');
			++temp;
		}
		else if(*temp >= 'A' && *temp <= 'F')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'A' + 10);
			++temp;
		}
		else if(*temp >= 'a' && *temp <= 'f')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'a' + 10);
			++temp;
		}
		if(*temp >= '0' && *temp <= '9')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - '0');
			++temp;
		}
		else if(*temp >= 'A' && *temp <= 'F')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'A' + 10);
			++temp;
		}
		else if(*temp >= 'a' && *temp <= 'f')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'a' + 10);
			++temp;
		}
	}
	else if(*temp == 'u')
	{
		*isUnicode = 1;
		++temp;
		*value = (ILUInt64)0;
		if(*temp >= '0' && *temp <= '9')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - '0');
			++temp;
		}
		else if(*temp >= 'A' && *temp <= 'F')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'A' + 10);
			++temp;
		}
		else if(*temp >= 'a' && *temp <= 'f')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'a' + 10);
			++temp;
		}
		if(*temp >= '0' && *temp <= '9')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - '0');
			++temp;
		}
		else if(*temp >= 'A' && *temp <= 'F')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'A' + 10);
			++temp;
		}
		else if(*temp >= 'a' && *temp <= 'f')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'a' + 10);
			++temp;
		}
		if(*temp >= '0' && *temp <= '9')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - '0');
			++temp;
		}
		else if(*temp >= 'A' && *temp <= 'F')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'A' + 10);
			++temp;
		}
		else if(*temp >= 'a' && *temp <= 'f')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'a' + 10);
			++temp;
		}
		if(*temp >= '0' && *temp <= '9')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - '0');
			++temp;
		}
		else if(*temp >= 'A' && *temp <= 'F')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'A' + 10);
			++temp;
		}
		else if(*temp >= 'a' && *temp <= 'f')
		{
			*value = (*value << 4) + (ILUInt64)(*temp - 'a' + 10);
			++temp;
		}
	}
	else if(*temp >= 'a' && *temp <= 'z')
	{
		*value = (ILUInt64)(escapechars[*temp - 'a']);
		++temp;
	}
	else if(*temp >= '0' && *temp <= '7')
	{
		*value = (ILUInt64)0;
		do
		{
			*value = (*value << 3) + (ILUInt64)(*temp - '0');
			++temp;
		}
		while(*temp >= '0' && *temp <= '7');
	}
	else
	{
		*value = (ILUInt64)(ILInt64)(((int)((signed char)(*temp))));
		++temp;
	}
	return (int)(temp - begin);
}

void CLexParseChar(const char *text, CLexIntConst *value)
{
	int isUnicode;

	/* Skip the leading "'" */
	if(*text == 'L' || *text == 'l')
	{
		++text;
	}
	++text;

	/* Parse the character */
	if(*text == '\\')
	{
		/* Escaped character */
		++text;
		text += ParseEscape(text, &(value->value), &isUnicode);
		if(isUnicode)
		{
			/* Unicode escapes aren't strictly legal in signed 8-bit
			   characters, but do something useful anyway */
			if(value->value <= 127)
			{
				value->isneg = 0;
				value->type = ILMachineType_Int8;
			}
			else
			{
				value->isneg = 0;
				value->type = ILMachineType_UInt16;
			}
		}
		else
		{
			value->isneg = ((value->value & 0x80) != 0);
			if(value->isneg)
			{
				value->value =
					(ILUInt64)(-((ILInt64)(signed char)(value->value)));
			}
			value->type = ILMachineType_Int8;
		}
	}
	else
	{
		/* Normal character */
		value->value = (ILUInt64)(ILInt64)((int)(signed char)(*text++));
		value->isneg = ((value->value & 0x80) != 0);
		if(value->isneg)
		{
			value->value = (ILUInt64)(-((ILInt64)(value->value)));
		}
		value->type = ILMachineType_Int8;
	}

	/* Make sure that the constant is terminated correctly */
	if(*text != '\'')
	{
		CCWarningOnLine(yycurrfilename(), yycurrlinenum(),
					    _("multi-character constant"));
	}
}

void CLexParseString(char *text, ILIntString *value)
{
	char *dest;
	int isUnicode;
	ILUInt64 ch;
	unsigned ch2;

	/* Initialize the value to be returned */
	if(*text == 'L' || *text == 'l' || *text == 's' || *text == 'S')
	{
		++text;
	}
	++text;
	value->string = text;
	value->len = 0;

	/* Expand any escape sequences that exist in the string */
	dest = text;
	while(*text != '"')
	{
		if(*text == '\\')
		{
			++text;
			text += ParseEscape(text, &ch, &isUnicode);
			if(isUnicode)
			{
				/* Expand the Unicode sequence to UTF-8 */
				if((text - dest) < 3)
				{
					CCWarningOnLine(yycurrfilename(), yycurrlinenum(),
								 	_("ignoring short '\\u' sequence"));
				}
				else
				{
					ch2 = (((unsigned)(ch)) & 0xFFFF);
					if(ch2 < 0x80)
					{
						*dest++ = (char)ch2;
						++(value->len);
					}
					else if(ch2 < (1 << 11))
					{
						*dest++ = (char)((ch2 >> 6) | 0xC0);
						*dest++ = (char)((ch2 & 0x3F) | 0x80);
						value->len += 2;
					}
					else
					{
						*dest++ = (char)((ch2 >> 12) | 0xE0);
						*dest++ = (char)(((ch2 >> 6) & 0x3F) | 0x80);
						*dest++ = (char)((ch2 & 0x3F) | 0x80);
						value->len += 3;
					}
				}
			}
			else
			{
				/* Normal escaped character */
				*dest++ = (char)ch;
				++(value->len);
			}
		}
		else
		{
			*dest++ = *text++;
			++(value->len);
		}
	}

	/* Intern the string for the higher layers */
	*value = ILInternString(value->string, value->len);
}

#ifdef	__cplusplus
};
#endif
