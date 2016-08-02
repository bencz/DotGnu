/*
 * winres_writer.c - Write the data in the ".rsrc" section of an IL binary.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

ILResourceSection *ILResourceSectionCreateWriter(ILImage *image)
{
	ILResourceSection *section;
	section = (ILResourceSection *)ILMalloc(sizeof(ILResourceSection));
	if(section)
	{
		section->image = image;
		section->data = 0;
		section->length = 0;
		section->rootDirectory = (ILResourceEntry *)
			ILMalloc(sizeof(ILResourceEntry));
		if(!(section->rootDirectory))
		{
			ILFree(section);
			return 0;
		}
		section->rootDirectory->isDirectory = -1;
		section->rootDirectory->isMallocData = 0;
		section->rootDirectory->isNumeric = 0;
		section->rootDirectory->name = 0;
		section->rootDirectory->nameLen = 0;
		section->rootDirectory->children = 0;
		section->rootDirectory->next = 0;
		section->rootDirectory->data = 0;
		section->rootDirectory->length = 0;
		section->rootDirectory->rva = 0;
	}
	return section;
}

/*
 * Insert an entry into the resource tree, with a specific name.
 */
static void *InsertEntry(ILResourceEntry *parent, ILResourceEntry *prev,
						 const char *name)
{
	ILResourceEntry *entry = 0;
	int posn, isNumeric;
	for(;;)
	{
		/* Extract the next name component */
		if(*name == '/')
		{
			++name;
			continue;
		}
		else if(*name == '\0')
		{
			break;
		}
		posn = 0;
		isNumeric = 1;
		while(name[posn] != '\0' && name[posn] != '/')
		{
			if(name[posn] < '0' || name[posn] > '9')
			{
				isNumeric = 0;
			}
			++posn;
		}

		/* Create a new entry under the current parent */
		entry = (ILResourceEntry *)ILMalloc(sizeof(ILResourceEntry));
		if(!entry)
		{
			return 0;
		}
		entry->isDirectory = (name[posn] == '/') ? -1 : 0;
		entry->isMallocData = 0;
		entry->isNumeric = isNumeric;
		entry->name = ILDupNString(name, posn);
		if(!(entry->name))
		{
			ILFree(entry);
			return 0;
		}
		entry->nameLen = posn;
		entry->children = 0;
		entry->next = 0;
		entry->data = 0;
		entry->length = 0;
		entry->rva = 0;
		if(prev)
		{
			prev->next = entry;
		}
		else
		{
			parent->children = entry;
		}

		/* Advance to the next name component */
		parent = entry;
		prev = 0;
		name += posn;
	}
	return entry;
}

void *ILResourceSectionAddEntry(ILResourceSection *section, const char *name)
{
	ILResourceEntry *parent;
	ILResourceEntry *entry;
	ILResourceEntry *prev;
	int posn, cmp;

	/* Search down the hierarchy for the specified entry */
	entry = section->rootDirectory;
	parent = entry;
	prev = 0;
	while(entry != 0)
	{
		if(*name == '/')
		{
			++name;
			continue;
		}
		else if(*name == '\0')
		{
			break;
		}
		posn = 0;
		while(name[posn] != '\0' && name[posn] != '/')
		{
			++posn;
		}
		parent = entry;
		entry = entry->children;
		prev = 0;
		while(entry != 0)
		{
			if(entry->nameLen == posn)
			{
				cmp = ILStrNICmp(entry->name, name, posn);
			}
			else if(entry->nameLen < posn)
			{
				cmp = ILStrNICmp(entry->name, name, entry->nameLen);
				if(!cmp)
				{
					cmp = -1;
				}
			}
			else
			{
				cmp = ILStrNICmp(entry->name, name, posn);
				if(!cmp)
				{
					cmp = 1;
				}
			}
			if(!cmp)
			{
				/* Go down a level: we already have this name */
				break;
			}
			else if(cmp < 0)
			{
				/* This is where we need to insert the new entry */
				return InsertEntry(parent, prev, name);
			}
			prev = entry;
			entry = entry->next;
		}
		if(!entry)
		{
			/* Insert the new entry at the end of the current level */
			return InsertEntry(parent, prev, name);
		}
		name += posn;
	}

	/* We already have a leaf entry with this name */
	return entry;
}

int ILResourceSectionAddBytes(void *_entry, const void *buffer, int len)
{
	ILResourceEntry *entry = (ILResourceEntry *)_entry;
	unsigned char *newData;
	if(entry && len > 0)
	{
		newData = (unsigned char *)ILRealloc
			(entry->data, entry->length + (unsigned long)(long)len);
		if(!newData)
		{
			return 0;
		}
		entry->isMallocData = -1;
		entry->data = newData;
		ILMemCpy(newData + entry->length, buffer, len);
		entry->length += (unsigned long)(long)len;
	}
	return 1;
}

/*
 * Allocate RVA's to a directory tree and fetch the total size
 * of the entry, name, and data tables.
 */
static void AllocateRVAs(ILResourceEntry *current, unsigned long *entries,
						 unsigned long *names, unsigned long *data)
{
	ILResourceEntry *entry;

	/* We need 16 bytes for the start of directory header */
	*entries += 16;

	/* Count the number of bytes needed for the entries themselves */
	entry = current->children;
	while(entry != 0)
	{
		*entries += 8;
		if(!(entry->isNumeric))
		{
			*names += (entry->nameLen + 1) * 2;
		}
		entry = entry->next;
	}

	/* Allocate RVA's to the directory entries and data blocks */
	entry = current->children;
	while(entry != 0)
	{
		if(!(entry->isNumeric))
		{
			entry->rva = *entries;
			if(!(entry->isDirectory))
			{
				*data += entry->length;
				if((entry->length % 4) != 0)
				{
					*data += 4 - (entry->length % 4);
				}
			}
			AllocateRVAs(entry, entries, names, data);
		}
		entry = entry->next;
	}
	entry = current->children;
	while(entry != 0)
	{
		if(entry->isNumeric)
		{
			entry->rva = *entries;
			if(!(entry->isDirectory))
			{
				*data += entry->length;
				if((entry->length % 4) != 0)
				{
					*data += 4 - (entry->length % 4);
				}
			}
			AllocateRVAs(entry, entries, names, data);
		}
		entry = entry->next;
	}
}

/*
 * Convert a name into a numeric identifier.
 */
static unsigned long NameToNumeric(const char *name)
{
	unsigned long num = 0;
	while(*name != '\0')
	{
		num = num * 10 + (unsigned long)(*name++ - '0');
	}
	return num;
}

/*
 * Write out the directory entries in a resource section.
 */
static void WriteDirectoryTree(ILResourceEntry *current, unsigned long *names,
						       unsigned long *data, unsigned char *buffer,
							   ILWriter *writer)
{
	ILResourceEntry *entry;
	unsigned numNamed, numId;
	unsigned long rva;

	/* Write the directory header or data block reference */
	if(current->isDirectory)
	{
		entry = current->children;
		numNamed = 0;
		numId = 0;
		while(entry != 0)
		{
			if(entry->isNumeric)
			{
				++numId;
			}
			else
			{
				++numNamed;
			}
			entry = entry->next;
		}
		ILMemSet(buffer, 0, 12);
		IL_WRITE_UINT16(buffer + 12, (ILUInt16)numNamed);
		IL_WRITE_UINT16(buffer + 14, (ILUInt16)numId);
		ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC, buffer, 16);
	}
	else
	{
		/* TODO: register a fixup for the first word */
		IL_WRITE_UINT32(buffer, *data);
		*data += current->length;
		if((current->length % 4) != 0)
		{
			*data += 4 - (current->length % 4);
		}
		IL_WRITE_UINT32(buffer, current->length);
		ILMemSet(buffer, 8, 8);
		ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC, buffer, 16);
		return;
	}

	/* Write the directory entries for this directory level */
	entry = current->children;
	while(entry != 0)
	{
		if(!(entry->isNumeric))
		{
			rva = entry->rva;
			if(entry->isDirectory)
			{
				rva |= (ILUInt32)0x80000000;
			}
			IL_WRITE_UINT32(buffer, *names);
			IL_WRITE_UINT32(buffer + 4, rva);
			ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC, buffer, 8);
			*names += (entry->nameLen + 1) * 2;
		}
		entry = entry->next;
	}
	entry = current->children;
	while(entry != 0)
	{
		if(entry->isNumeric)
		{
			rva = NameToNumeric(entry->name);
			IL_WRITE_UINT32(buffer, rva);
			rva = entry->rva;
			if(entry->isDirectory)
			{
				rva |= (ILUInt32)0x80000000;
			}
			IL_WRITE_UINT32(buffer + 4, rva);
			ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC, buffer, 8);
		}
		entry = entry->next;
	}

	/* Write the sub-directory entries */
	entry = current->children;
	while(entry != 0)
	{
		if(!(entry->isNumeric))
		{
			WriteDirectoryTree(entry, names, data, buffer, writer);
		}
		entry = entry->next;
	}
	entry = current->children;
	while(entry != 0)
	{
		if(entry->isNumeric)
		{
			WriteDirectoryTree(entry, names, data, buffer, writer);
		}
		entry = entry->next;
	}
}

/*
 * Write out the name table entries in a resource section.
 */
static void WriteNameTable(ILResourceEntry *current, unsigned char *buffer,
						   ILWriter *writer)
{
	ILResourceEntry *entry;
	int posn;

	/* Write the names for this directory level */
	entry = current->children;
	while(entry != 0)
	{
		if(!(entry->isNumeric))
		{
			IL_WRITE_UINT16(buffer, entry->nameLen);
			ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC, buffer, 2);
			for(posn = 0; posn < entry->nameLen; ++posn)
			{
				IL_WRITE_UINT16(buffer, entry->name[posn]);
				ILWriterOtherWrite
					(writer, ".rsrc", IL_IMAGESECT_RSRC, buffer, 2);
			}
		}
		entry = entry->next;
	}

	/* Write the names for the next directory level down */
	entry = current->children;
	while(entry != 0)
	{
		if(!(entry->isNumeric))
		{
			WriteNameTable(entry, buffer, writer);
		}
		entry = entry->next;
	}
	entry = current->children;
	while(entry != 0)
	{
		if(entry->isNumeric)
		{
			WriteNameTable(entry, buffer, writer);
		}
		entry = entry->next;
	}
}

/*
 * Write out the data blocks in a resource section.
 */
static void WriteDataEntries(ILResourceEntry *current, unsigned char *buffer,
						     ILWriter *writer)
{
	ILResourceEntry *entry;

	/* Write the data blocks for this directory level */
	ILMemSet(buffer, 0, 4);
	entry = current->children;
	while(entry != 0)
	{
		if(!(entry->isNumeric))
		{
			ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC,
							   entry->data, entry->length);
			if((entry->length % 4) != 0)
			{
				ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC,
								   buffer, 4 - (entry->length % 4));
			}
		}
		entry = entry->next;
	}
	entry = current->children;
	while(entry != 0)
	{
		if(entry->isNumeric)
		{
			ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC,
							   entry->data, entry->length);
			if((entry->length % 4) != 0)
			{
				ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC,
								   buffer, 4 - (entry->length % 4));
			}
		}
		entry = entry->next;
	}

	/* Write the names for the next directory level down */
	entry = current->children;
	while(entry != 0)
	{
		if(!(entry->isNumeric))
		{
			WriteDataEntries(entry, buffer, writer);
		}
		entry = entry->next;
	}
	entry = current->children;
	while(entry != 0)
	{
		if(entry->isNumeric)
		{
			WriteDataEntries(entry, buffer, writer);
		}
		entry = entry->next;
	}
}

void ILResourceSectionFlush(ILResourceSection *section, ILWriter *writer)
{
	unsigned long entries = 0;
	unsigned long names = 0;
	unsigned long data = 0;
	unsigned long startNames;
	unsigned long startData;
	unsigned char buffer[16];

	/* Allocate RVA's to the values in the directory tree */
	AllocateRVAs(section->rootDirectory, &entries, &names, &data);

	/* Determine the real starting RVA's for the name table and data area */
	startNames = entries;
	startData = entries + names;
	if((names % 4) != 0)
	{
		startData += 4 - (names % 4);
	}

	/* Write out the directory entries */
	WriteDirectoryTree(section->rootDirectory, &startNames, &startData,
					   buffer, writer);

	/* Write out the name table */
	WriteNameTable(section->rootDirectory, buffer, writer);
	if((names % 4) != 0)
	{
		ILMemSet(buffer, 0, 4);
		ILWriterOtherWrite(writer, ".rsrc", IL_IMAGESECT_RSRC, buffer,
						   4 - (names % 4));
	}

	/* Write out the data section */
	WriteDataEntries(section->rootDirectory, buffer, writer);
}

#ifdef	__cplusplus
};
#endif
