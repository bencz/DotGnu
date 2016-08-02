/*
 * link_res.c - Process binary resources within a linker context.
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

#include "linker.h"

#ifdef	__cplusplus
extern	"C" {
#endif

int ILLinkerAddResource(ILLinker *linker, const char *name,
						int isPrivate, FILE *stream)
{
	unsigned long rva;
	unsigned long length;
	char buffer[BUFSIZ];
	int len;
	ILManifestRes *res;

	/* Initialize the resources section, if necessary */
	if(!(linker->resourceRVA))
	{
		ILWriterTextAlign(linker->writer);
		linker->resourceRVA = ILWriterGetTextRVA(linker->writer);
	}

	/* Write a place-holder for the resource length */
	rva = ILWriterGetTextRVA(linker->writer);
	ILMemZero(buffer, 4);
	ILWriterTextWrite(linker->writer, buffer, 4);

	/* Copy the contents of the input stream to the resource section */
	length = 0;
	while((len = fread(buffer, 1, BUFSIZ, stream)) > 0)
	{
		ILWriterTextWrite(linker->writer, buffer, len);
		length += (unsigned long)len;
		if(len < BUFSIZ)
		{
			break;
		}
	}

	/* Pad the resource section to a multiple of 4 bytes */
	if((length % 4) != 0)
	{
		ILMemZero(buffer, 4);
		ILWriterTextWrite(linker->writer, buffer, 4 - (int)(length % 4));
	}

	/* Back-patch the place-holder with the resource length */
	ILWriterTextWrite32Bit(linker->writer, rva, length);

	/* Update the total size of the resources section */
	ILWriterUpdateHeader(linker->writer, IL_IMAGEENTRY_RESOURCES,
						 linker->resourceRVA,
						 ILWriterGetTextRVA(linker->writer) -
						 		linker->resourceRVA);

	/* Add a manifest resource record to the metadata */
	res = ILManifestResCreate(linker->image, 0, name,
							  (isPrivate ? IL_META_MANIFEST_PRIVATE
							  			 : IL_META_MANIFEST_PUBLIC),
							  			 rva - linker->resourceRVA);
	if(!res)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}

	/* Done */
	return 1;
}

int ILLinkerAddWin32Resource(ILLinker *linker, const char *filename)
{
	/* TODO */
	return 1;
}

int ILLinkerAddWin32Icon(ILLinker *linker, const char *filename)
{
	/* TODO */
	return 1;
}

int ILLinkerAddWin32Version(ILLinker *linker)
{
	/* TODO */
	return 1;
}

#ifdef	__cplusplus
};
#endif
