/*
 * dump_guid.c - Dump GUID's in assembly format.
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

#ifdef	__cplusplus
extern	"C" {
#endif

void ILDumpGUID(FILE *stream, const unsigned char *guid)
{
	fprintf(stream, "{%08lX-%04lX-%04lX-%02X%02X-%02X%02X%02X%02X%02X%02X}",
			(unsigned long)(IL_READ_UINT32(guid)),
			(unsigned long)(IL_READ_UINT16(guid + 4)),
			(unsigned long)(IL_READ_UINT16(guid + 6)),
			(int)(guid[8]), (int)(guid[9]),
			(int)(guid[10]), (int)(guid[11]),
			(int)(guid[12]), (int)(guid[13]),
			(int)(guid[14]), (int)(guid[15]));
}

#ifdef	__cplusplus
};
#endif
