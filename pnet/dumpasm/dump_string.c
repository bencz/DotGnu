/*
 * dump_string.c - Dump strings in assembly format.
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

#include "il_dumpasm.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

void ILDumpString(FILE *stream, const char *str)
{
	ILDumpStringLen(stream, str, strlen(str));
}

void ILDumpStringLen(FILE *stream, const char *str, int len)
{
	int ch;
	putc('"', stream);
	while(len > 0)
	{
		ch = (*str++ & 0xFF);
		if(ch == '"' || ch == '\\')
		{
			putc('\\', stream);
			putc(ch, stream);
		}
		else if(ch < 0x20 || ch == 0x7F)
		{
			putc('0' + ((ch >> 6) & 0x03), stream);
			putc('0' + ((ch >> 3) & 0x07), stream);
			putc('0' + (ch & 0x07), stream);
		}
		else
		{
			putc(ch, stream);
		}
		--len;
	}
	putc('"', stream);
}

void ILDumpUnicodeString(FILE *stream, const char *str,
						 unsigned long numChars)
{
	ILUInt16 ch;
	putc('"', stream);
	while(numChars > 0)
	{
		ch = IL_READ_UINT16(str);
		if(ch < 32)
		{
			if(ch == (ILUInt32)'\n')
			{
				fputs("\\n", stream);
			}
			else if(ch == (ILUInt32)'\r')
			{
				fputs("\\r", stream);
			}
			else if(ch == (ILUInt32)'\t')
			{
				fputs("\\t", stream);
			}
			else
			{
				fprintf(stream, "\\x%02X", (int)ch);
			}
		}
		else if(ch == '"' || ch == '\\')
		{
			putc('\\', stream);
			putc((int)ch, stream);
		}
		else if(ch < 0x80)
		{
			putc((int)ch, stream);
		}
		else if(ch < 0x100)
		{
			fprintf(stream, "\\x%02X", (int)ch);
		}
		else
		{
			fprintf(stream, "\\u%04X", (int)ch);
		}
		str += 2;
		--numChars;
	}
	putc('"', stream);
}

void ILDumpXmlString(FILE *stream, const char *str)
{
	ILDumpXmlStringLen(stream, str, strlen(str));
}

void ILDumpXmlStringLen(FILE *stream, const char *str, int len)
{
	int ch;
	fputs("&quot;", stream);
	while(len > 0)
	{
		ch = (*str++ & 0xFF);
		if(ch == '<')
		{
			fputs("&lt;", stdout);
		}
		else if(ch == '>')
		{
			fputs("&gt;", stdout);
		}
		else if(ch == '&')
		{
			fputs("&amp;", stdout);
		}
		else if(ch == '"')
		{
			fputs("&quot;", stdout);
		}
		else if(ch == '\'')
		{
			fputs("&apos;", stdout);
		}
		else
		{
			putc(ch, stream);
		}
		--len;
	}
	fputs("&quot;", stream);
}

#ifdef	__cplusplus
};
#endif
