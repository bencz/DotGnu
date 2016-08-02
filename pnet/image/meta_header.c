/*
 * meta_header.c - Routines for walking the header of the metadata section.
 *
 * Copyright (C) 2001, 2003, 2009  Southern Storm Software, Pty Ltd.
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

#include "image.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Get the metadata section and validate it.  Returns
 * the number of entries, or 0xFFFFFFFF if invalid.
 */
static ILUInt32 GetMetadata(ILImage *image, unsigned char **addr,
							ILUInt32 *len, ILUInt32 *headerLen)
{
	void *address;
	ILUInt32 versionLen;

	/* Find the metadata section in the IL image */
	if(!ILImageGetSection(image, IL_SECTION_METADATA, &address, len))
	{
		return (ILUInt32)0xFFFFFFFF;
	}

	/* Is this really a metadata directory header? */
	*addr = (unsigned char *)address;
	if(*len >= 16 &&
	   (*addr)[0] == (unsigned char)'B' &&
	   (*addr)[1] == (unsigned char)'S' &&
	   (*addr)[2] == (unsigned char)'J' &&
	   (*addr)[3] == (unsigned char)'B')
	{
		if(IL_READ_UINT32(*addr + 4) != 0x00010001)
		{
			/* Incorrect version */
			return 0xFFFFFFFF;
		}
		versionLen = IL_READ_UINT32(*addr + 12);
		if((versionLen % 4) != 0)
		{
			versionLen += 4 - (versionLen % 4);
		}
		if(versionLen > (*len - 16))
		{
			return 0xFFFFFFFF;
		}
		if((*len - 16 - versionLen) < 4)
		{
			return 0xFFFFFFFF;
		}
		*headerLen = 16 + versionLen + 4;
		return IL_READ_UINT16(*addr + 16 + versionLen + 2);
	}
	else
	{
		/* Invalid metadata header */
		return 0xFFFFFFFF;
	}
}

/*
 * Get the next entry from a metadata directory.
 * Returns non-zero if a new entry has been found.
 */
static int GetNextEntry(unsigned char **addrRef, unsigned long *lenLeftRef,
						unsigned long *offsetRef, ILUInt32 *numEntries,
						unsigned long totalLen, ILUInt32 *entryOffset,
						ILUInt32 *entrySize, char **entryName)
{
	unsigned char *addr = *addrRef;
	unsigned long lenLeft = *lenLeftRef;
	unsigned long offset = *offsetRef;
	unsigned long extOffset;
	unsigned long extSize;

	/* Bail out if the directory has been exhausted */
	if(!(*numEntries))
	{
		return 0;
	}

	/* Check for truncation of the table */
	if(lenLeft < 9)
	{
		return 0;
	}

	/* Extract the information from the entry */
	extOffset = (unsigned long)(IL_READ_UINT32(addr));
	extSize   = (unsigned long)(IL_READ_UINT32(addr + 4));
	if(extOffset >= totalLen || !extSize ||
	   (extOffset + extSize) > totalLen ||
	   (extOffset + extSize) < extOffset)	/* Wrap-around check */
	{
		/* The index information is invalid */
		return 0;
	}
	if(entryOffset)
	{
		*entryOffset = IL_READ_UINT32(addr);
	}
	if(entrySize)
	{
		*entrySize = IL_READ_UINT32(addr + 4);
	}
	if(entryName)
	{
		*entryName = (char *)(addr + 8);
	}

	/* Skip to the next entry in the table */
	addr += 8;
	offset += 8;
	lenLeft -= 8;
	while(lenLeft > 0 && addr[0] != 0)
	{
		/* Skip the name of the entry */
		++addr;
		++offset;
		--lenLeft;
	}
	if(lenLeft > 0)
	{
		/* Skip the zero byte at the end of the name */
		++addr;
		++offset;
		--lenLeft;
	}
	else
	{
		/* Badly formatted name */
		return 0;
	}
	while(lenLeft > 0 && (offset & 3) != 0)
	{
		/* Skip bytes until we are DWORD-aligned again */
		++addr;
		++offset;
		--lenLeft;
	}
	*addrRef = addr;
	*lenLeftRef = lenLeft;
	*offsetRef = offset;
	--(*numEntries);
	return 1;
}

void *ILImageGetMetaEntry(ILImage *image, const char *name,
						  ILUInt32 *size)
{
	unsigned char *address;
	unsigned char *addr;
	ILUInt32 len;
	ILUInt32 headerLen;
	unsigned long lenLeft;
	ILUInt32 numEntries;
	unsigned long offset;
	ILUInt32 entryOffset;
	ILUInt32 entrySize;
	char *entryName;

	/* Find the metadata section and validate it */
	numEntries = GetMetadata(image, &address, &len, &headerLen);
	if(numEntries == 0xFFFFFFFF)
	{
		return 0;
	}

	/* Scan the entry table for the name */
	addr = address + headerLen;
	lenLeft = len - headerLen;
	offset = headerLen;
	while(GetNextEntry(&addr, &lenLeft, &offset, &numEntries, len,
					   &entryOffset, &entrySize, &entryName))
	{
		/* Is this the entry we are looking for? */
		if(!strcmp(entryName, name))
		{
			*size = entrySize;
			return (void *)(address + entryOffset);
		}
	}

	/* We could not find the entry */
	return 0;
}

ILUInt32 ILImageNumMetaEntries(ILImage *image)
{
	unsigned char *addr;
	ILUInt32 len;
	ILUInt32 headerLen;
	ILUInt32 numEntries;

	numEntries = GetMetadata(image, &addr, &len, &headerLen);
	if(numEntries != 0xFFFFFFFF)
	{
		return numEntries;
	}
	else
	{
		return 0;
	}
}

void *ILImageMetaEntryInfo(ILImage *image, unsigned long entry,
						   char **name, unsigned long *virtAddr,
						   ILUInt32 *size)
{
	unsigned char *address;
	unsigned char *addr;
	ILUInt32 len;
	ILUInt32 headerLen;
	unsigned long lenLeft;
	ILUInt32 numEntries;
	unsigned long offset;
	ILUInt32 entryOffset;
	ILUInt32 entrySize;
	char *entryName;

	/* Find the metadata section and validate it */
	numEntries = GetMetadata(image, &address, &len, &headerLen);
	if(numEntries == 0xFFFFFFFF)
	{
		return 0;
	}

	/* Scan the entry table for one we are looking for */
	addr = address + headerLen;
	lenLeft = len - headerLen;
	offset = headerLen;
	while(GetNextEntry(&addr, &lenLeft, &offset, &numEntries, len,
					   &entryOffset, &entrySize, &entryName))
	{
		/* Is this the entry we are looking for? */
		if(entry == 0)
		{
			if(name)
			{
				*name = entryName;
			}
			if(size)
			{
				*size = entrySize;
			}
			if(virtAddr)
			{
				*virtAddr = ILImageGetSectionAddr(image, IL_SECTION_METADATA) +
							entryOffset;
			}
			return (void *)(address + entryOffset);
		}
		--entry;
	}

	/* We could not find the entry */
	return 0;
}

unsigned long ILImageMetaHeaderSize(ILImage *image)
{
	unsigned char *addr;
	ILUInt32 len;
	ILUInt32 headerLen;
	unsigned long lenLeft;
	ILUInt32 numEntries;
	unsigned long offset;

	/* Find the metadata section and validate it */
	numEntries = GetMetadata(image, &addr, &len, &headerLen);
	if(numEntries == 0xFFFFFFFF)
	{
		return 0;
	}

	/* Scan the entry table to find the end */
	addr += headerLen;
	lenLeft = len - headerLen;
	offset = headerLen;
	while(GetNextEntry(&addr, &lenLeft, &offset, &numEntries, len, 0, 0, 0))
	{
		/* Nothing to do here: we are just calculating the final offset */
	}

	/* The offset is the header's size */
	return offset;
}

const char *ILImageMetaRuntimeVersion(ILImage *image, int *length)
{
	unsigned char *addr;
	ILUInt32 len;
	ILUInt32 headerLen;
	ILUInt32 numEntries;
	ILUInt32 versionLen;
	const char *version;

	/* Find the metadata section and validate it */
	numEntries = GetMetadata(image, &addr, &len, &headerLen);
	if(numEntries == 0xFFFFFFFF)
	{
		return 0;
	}

	/* Determine the size of the version string */
	versionLen = headerLen - 20;

	/* The version is at offset 16 within the metadata header */
	version = (const char *)(addr + 16);

	/* Trim trailing zeros from the version string */
	while(versionLen > 0 && version[versionLen - 1] == '\0')
	{
		--versionLen;
	}
	if(!versionLen)
	{
		return 0;
	}

	/* Return the version information to the caller */
	*length = (int)versionLen;
	return version;
}

#ifdef	__cplusplus
};
#endif
