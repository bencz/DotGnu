/*
 * resgen_binary.c - Binary resource loading and writing routines.
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

#include <stdio.h>
#include "resgen.h"
#include "il_system.h"
#include "il_image.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Read a string from binary resource data.
 * Returns zero if the data is badly formatted.
 */
static int readString(const char *filename,
					  unsigned char **data, unsigned long *size,
					  char **str, int *len)
{
	unsigned char *d = *data;
	unsigned long s = *size;
	unsigned char ch;
	int shift;
	if(s != 0)
	{
		ch = *d++;
		--s;
		*len = (int)(ch & 0x7F);
		shift = 7;
		while((ch & (unsigned char)0x80) != 0)
		{
			if(s == 0)
			{
				goto invalid;
			}
			ch = *d++;
			--s;
			*len |= (((int)(ch & 0x7F)) << shift);
			shift += 7;
		}
		*str = (char *)d;
		if(((unsigned long)(*len)) <= s)
		{
			*data = d + *len;
			*size = s - (unsigned long)(*len);
			return 1;
		}
	}
invalid:
	fprintf(stderr, "%s: invalid binary resource string\n", filename);
	return 0;
}

/*
 * Read a Unicode string from binary resource data.
 * Returns zero if the data is badly formatted.
 */
static int readUnicodeString(const char *filename,
					         unsigned char **data, unsigned long *size,
					         char **str, int *len)
{
	unsigned char *d = *data;
	unsigned long s = *size;
	unsigned char ch;
	unsigned unich;
	int length;
	int shift;
	if(s != 0)
	{
		/* Parse the length, in bytes, of the Unicode string */
		ch = *d++;
		--s;
		length = (int)(ch & 0x7F);
		shift = 7;
		while((ch & (unsigned char)0x80) != 0)
		{
			if(s == 0)
			{
				goto invalid;
			}
			ch = *d++;
			--s;
			length |= (((int)(ch & 0x7F)) << shift);
			shift += 7;
		}
		if(length >= 0x10000000 || ((unsigned long)length) > s)
		{
			goto invalid;
		}

		/* Allocate space for the converted string */
		if((*str = (char *)ILMalloc(length * 2 + 1)) == 0)
		{
			ILResOutOfMemory();
			return 0;
		}

		/* Convert the string from Unicode into UTF-8 */
		*len = 0;
		while(length >= 2)
		{
			unich = IL_READ_UINT16(d);
			d += 2;
			s -= 2;
			if(unich < 0x80)
			{
				(*str)[*len] = (char)unich;
				++(*len);
			}
			else if(unich < (1 << 11))
			{
				(*str)[*len] = (char)(0xC0 | (unich >> 6));
				++(*len);
				(*str)[*len] = (char)(0x80 | (unich & 0x3F));
				++(*len);
			}
			else
			{
				(*str)[*len] = (char)(0xE0 | (unich >> 12));
				++(*len);
				(*str)[*len] = (char)(0x80 | ((unich >> 6) & 0x3F));
				++(*len);
				(*str)[*len] = (char)(0x80 | (unich & 0x3F));
				++(*len);
			}
			length -= 2;
		}
		(*str)[*len] = '\0';
		*data = d;
		*size = s;
		return 1;
	}
invalid:
	fprintf(stderr, "%s: invalid binary resource string\n", filename);
	return 0;
}

/*
 * Magic strings used by the binary resource format.
 */
static char const className1[] =
			"System.Resources.ResourceReader, mscorlib";
static char const className2[] =
			"System.String, mscorlib";
static char const className3[] =
			"System.Resources.RuntimeResourceSet, mscorlib";

/*
 * Parse binary resource data from a buffer.
 */
static int parseBinaryResources(const char *filename, unsigned char *data,
								unsigned long size)
{
	unsigned long numStrings;
	char *str, *str2;
	int len, len2;
	unsigned char *fullData = data - 4;
	unsigned long fullSize = size + 4;
	unsigned long tempNum;
	unsigned char *valueData;
	unsigned long valueSize;
	unsigned long dataSection;
	unsigned long offset;
	int error = 0;

	/* Check the version information at the start of the stream */
	if(size < 8)
	{
		fprintf(stderr,"%s: truncated resource data\n", filename);
		return 1;
	}
	if(IL_READ_UINT32(data) != 1)
	{
		fprintf(stderr, "%s: invalid resource version\n", filename);
		return 1;
	}
	data += 8;
	size -= 8;
	if(!readString(filename, &data, &size, &str, &len))
	{
		return 1;
	}
	len2 = strlen(className1);
	if(len < len2 || ILMemCmp(str, className1, len2) != 0)
	{
		/* This isn't a set of string resources, so skip it */
		return 0;
	}

	/* Skip the next string */
	if(!readString(filename, &data, &size, &str, &len))
	{
		return 1;
	}

	/* Parse the secondary resource header */
	if(size < 12)
	{
		fprintf(stderr,"%s: truncated resource data\n", filename);
		return 1;
	}
	else if(IL_READ_UINT32(data) != 1)
	{
		fprintf(stderr, "%s: invalid resource version\n", filename);
		return 1;
	}
	numStrings = IL_READ_UINT32(data + 4);
	tempNum = IL_READ_UINT32(data + 8);
	data += 12;
	size -= 12;

	/* Skip the representation table */
	while(tempNum > 0)
	{
		if(!readString(filename, &data, &size, &str, &len))
		{
			return 1;
		}
		len2 = strlen(className2);
		if(len < len2 || ILMemCmp(str, className2, len2) != 0)
		{
			/* This isn't a set of string resources, so skip it */
			return 0;
		}
		--tempNum;
	}

	/* Align on an 8-byte boundary */
	offset = fullSize - size;
	if((offset % 8) != 0)
	{
		offset = 8 - (offset % 8);
		if(offset > size)
		{
			fprintf(stderr,"%s: truncated resource data\n", filename);
			return 1;
		}
		data += offset;
		size -= offset;
	}

	/* Skip the name hash and name position tables */
	if(numStrings > (size / 8))
	{
		fprintf(stderr,"%s: truncated resource data\n", filename);
		return 1;
	}
	data += numStrings * 8;
	size -= numStrings * 8;

	/* Get the data section offset */
	if(size < 4)
	{
		fprintf(stderr,"%s: truncated resource data\n", filename);
		return 1;
	}
	dataSection = IL_READ_UINT32(data);
	data += 4;
	size -= 4;

	/* Process the name table */
	tempNum = numStrings;
	while(tempNum > 0)
	{
		/* Read the name string, and convert from its Unicode form */
		if(!readUnicodeString(filename, &data, &size, &str, &len))
		{
			return 1;
		}

		/* Read and validate the offset of the value */
		if(size < 4)
		{
			ILFree(str);
			fprintf(stderr,"%s: truncated resource data\n", filename);
			return 1;
		}
		offset = IL_READ_UINT32(data) + dataSection;
		data += 4;
		size -= 4;
		if(offset >= fullSize)
		{
			ILFree(str);
			fprintf(stderr,"%s: invalid offset to resource value\n", filename);
			return 1;
		}
		valueData = fullData + offset;
		valueSize = fullSize - offset;

		/* Skip the type table index (which will always be "System.String") */
		while(valueSize > 0 && (*valueData & 0x80) != 0)
		{
			++valueData;
			--valueSize;
		}
		if(valueSize > 0)
		{
			++valueData;
			--valueSize;
		}

		/* Read the string value */
		if(!readString(filename, &valueData, &valueSize, &str2, &len2))
		{
			ILFree(str);
			return 1;
		}

		/* Add the string to the resource table */
		error |= ILResAddResource(filename, -1, str, len, str2, len2);

		/* Free the temporary string and advance */
		ILFree(str);
		--tempNum;
	}

	/* Done */
	return error;
}

int ILResLoadBinary(const char *filename, FILE *stream)
{
	char buffer[BUFSIZ];
	unsigned char *data = 0;
	unsigned long dataLen = 0;
	unsigned long dataMax = 0;
	unsigned len;
	int error;

	/* Load the entire contents of the input file into memory */
	while((len = fread(buffer, 1, sizeof(buffer), stream)) > 0)
	{
		if((dataLen + len) > dataMax)
		{
			dataMax = (dataLen + len + 4095) & ~4095;
			if((data = (unsigned char *)ILRealloc(data, dataMax)) == 0)
			{
				ILResOutOfMemory();
			}
		}
		ILMemCpy(data + dataLen, buffer, len);
		dataLen += len;
		if(len < sizeof(buffer))
		{
			break;
		}
	}

	/* Check for the magic number */
	if(dataLen < 4 || IL_READ_UINT32(data) != (unsigned long)0xBEEFCACE)
	{
		fprintf(stderr, "%s: not a binary resource file\n", filename);
		if(data != 0)
		{
			ILFree(data);
		}
		return 1;
	}

	/* Parse the resources */
	error = parseBinaryResources(filename, data + 4, dataLen - 4);

	/* Free the memory that was used for the loaded data */
	ILFree(data);

	/* Done */
	return error;
}

int ILResLoadBinaryIL(const char *filename, unsigned char *address,
					  unsigned long size)
{
	unsigned long len;
	unsigned long magic;
	unsigned long pad;
	while(size >= 8)
	{
		/* Read the header for the next resource sub-section */
		len = IL_READ_UINT32(address);
		magic = IL_READ_UINT32(address + 4);
		if(len > (size - 4))
		{
			fprintf(stderr, "%s: badly formatted resources\n", filename);
			return 1;
		}
		address += 8;
		size -= 8;
		len -= 4;
		if((len % 4) != 0)
		{
			pad = 4 - (len % 4);
		}
		else
		{
			pad = 0;
		}

		/* Skip if not string resources */
		if(magic != (unsigned long)0xBEEFCACE)
		{
			address += len + pad;
			size -= len + pad;
			continue;
		}

		/* Parse the binary resource data */
		if(parseBinaryResources(filename, address, len))
		{
			return 1;
		}
		address += len + pad;
		size -= len + pad;
	}
	return 0;
}

/*
 * Determine the number of bytes that are necessary to represent a length.
 */
static int lengthSize(int len)
{
	int size = 1;
	while(len >= 0x80)
	{
		++size;
		len >>= 7;
	}
	return size;
}

/*
 * Get the number of bytes necessary to represent a Unicode string.
 */
static int unicodeLength(const char *str, int len)
{
	unsigned long ch;
	int posn = 0;
	int nbytes = 0;
	while(posn < len)
	{
		ch = ILUTF8ReadChar(str, len, &posn);
		if(ch < (unsigned long)0x10000)
		{
			nbytes += 2;
		}
		else if(ch < (((unsigned long)1) << 20))
		{
			nbytes += 4;
		}
	}
	return nbytes;
}

/*
 * Write a 32-bit word to a binary output stream.
 */
static void writeWord(FILE *stream, unsigned long word)
{
	putc((word & 0xFF), stream);
	putc(((word >> 8) & 0xFF), stream);
	putc(((word >> 16) & 0xFF), stream);
	putc(((word >> 24) & 0xFF), stream);
}

/*
 * Write a length value to a binary output stream.
 */
static void writeLength(FILE *stream, int len)
{
	while(len >= 0x80)
	{
		putc(((len & 0x7F) | 0x80), stream);
		len >>= 7;
	}
	putc(len, stream);
}

/*
 * Write a length-specified string to a binary output stream.
 */
static void writeString(FILE *stream, const char *str, int len)
{
	/* Write out the length */
	writeLength(stream, len);

	/* Write out the string data */
	if(len)
	{
		fwrite(str, 1, len, stream);
	}
}

/*
 * Write a length-specified unicode string to a binary output stream.
 */
static void writeUnicodeString(FILE *stream, const char *str, int len)
{
	int posn = 0;
	unsigned char buf[4];
	int templen;

	/* Write out the length */
	writeLength(stream, unicodeLength(str, len));

	/* Write out the contents of the string */
	while(posn < len)
	{
		templen = ILUTF16WriteCharAsBytes(buf, ILUTF8ReadChar(str, len, &posn));
		if(templen != 0)
		{
			fwrite(buf, 1, templen, stream);
		}
	}
}

/*
 * Hash a unicode string.
 */
static ILInt32 hashUnicodeString(const char *str, int len)
{
	int posn = 0;
	unsigned long ch;
	unsigned long tempch;
	ILUInt32 hash = 0x1505;
	while(posn < len)
	{
		ch = ILUTF8ReadChar(str, len, &posn);
		if(ch < (unsigned long)0x10000)
		{
			hash = ((hash << 5) + hash) ^ (ILUInt32)ch;
		}
		else if(ch < ((unsigned long)0x110000))
		{
			ch -= 0x10000;
			tempch = ((ch >> 10) + 0xD800);
			hash = ((hash << 5) + hash) ^ (ILUInt32)tempch;
			tempch = ((ch & ((((ILUInt32)1) << 10) - 1)) + 0xDC00);
			hash = ((hash << 5) + hash) ^ (ILUInt32)tempch;
		}
	}
	return (ILInt32)hash;
}

void ILResWriteBinary(FILE *stream)
{
	int hash;
	long offset;
	long position;
	long numStrings;
	long dataSection;
	int unicodeLen;
	ILResHashEntry *entry;
	ILInt32 *hashes;
	ILInt32 *positions;
	long index, index2;
	ILInt32 temp;

	/* Calculate the offsets of all strings from the start
	   of the data section, the position of all names from
	   the start of the name section, and the total number
	   of strings */
	offset = 0;
	position = 0;
	numStrings = 0;
	for(hash = 0; hash < IL_RES_HASH_TABLE_SIZE; ++hash)
	{
		entry = ILResHashTable[hash];
		while(entry != 0)
		{
			entry->offset = offset;
			entry->position = position;
			offset += lengthSize(0) + lengthSize(entry->valueLen) +
					  entry->valueLen;
			unicodeLen = unicodeLength(entry->data, entry->nameLen);
			position += lengthSize(unicodeLen) + unicodeLen + 4;
			++numStrings;
			entry = entry->next;
		}
	}

	/* Create the name hash and position tables */
	if(numStrings > 0)
	{
		/* Allocate the tables */
		if((hashes = (ILInt32 *)ILCalloc(numStrings, sizeof(ILInt32))) == 0)
		{
			ILResOutOfMemory();
		}
		if((positions = (ILInt32 *)ILCalloc(numStrings, sizeof(ILInt32))) == 0)
		{
			ILResOutOfMemory();
		}

		/* Compute the hash values and positions */
		index = 0;
		for(hash = 0; hash < IL_RES_HASH_TABLE_SIZE; ++hash)
		{
			entry = ILResHashTable[hash];
			while(entry != 0)
			{
				hashes[index] = hashUnicodeString(entry->data, entry->nameLen);
				positions[index] = (ILInt32)(entry->position);
				++index;
				entry = entry->next;
			}
		}

		/* Sort the tables into ascending order of hash */
		for(index = 0; index < (numStrings - 1); ++index)
		{
			for(index2 = (index + 1); index2 < numStrings; ++index2)
			{
				if(hashes[index] > hashes[index2])
				{
					temp = hashes[index];
					hashes[index] = hashes[index2];
					hashes[index2] = temp;
					temp = positions[index];
					positions[index] = positions[index2];
					positions[index2] = temp;
				}
			}
		}
	}
	else
	{
		hashes = 0;
		positions = 0;
	}

	/* Write out the header */
	writeWord(stream, (unsigned long)0xBEEFCACE);	/* Magic number */
	writeWord(stream, (unsigned long)1);			/* Version */
	writeWord(stream, (unsigned long)(lengthSize(strlen(className1)) +
									  strlen(className1) +
									  lengthSize(strlen(className3)) +
									  strlen(className3)));
	writeString(stream, className1, strlen(className1));
	writeString(stream, className3, strlen(className3));
	writeWord(stream, (unsigned long)1);			/* Secondary version */
	writeWord(stream, (unsigned long)numStrings);
	writeWord(stream, (unsigned long)1);			/* Number of types */
	writeString(stream, className2, strlen(className2));

	/* Align on an 8-byte boundary */
	while((ftell(stream) % 8) != 0)
	{
		putc(0, stream);
	}

	/* Write out the name hash table */
	for(index = 0; index < numStrings; ++index)
	{
		writeWord(stream, (unsigned long)(hashes[index]));
	}
	if(hashes)
	{
		ILFree(hashes);
	}

	/* Write out the name position table */
	for(index = 0; index < numStrings; ++index)
	{
		writeWord(stream, (unsigned long)(positions[index]));
	}
	if(positions)
	{
		ILFree(positions);
	}

	/* Write the offset of the data section */
	dataSection = position + ftell(stream) + 4;
	writeWord(stream, (unsigned long)dataSection);

	/* Write out the resource names */
	for(hash = 0; hash < IL_RES_HASH_TABLE_SIZE; ++hash)
	{
		entry = ILResHashTable[hash];
		while(entry != 0)
		{
			writeUnicodeString(stream, entry->data, entry->nameLen);
			writeWord(stream, (unsigned long)(entry->offset));
			entry = entry->next;
		}
	}

	/* Write out the resource values */
	for(hash = 0; hash < IL_RES_HASH_TABLE_SIZE; ++hash)
	{
		entry = ILResHashTable[hash];
		while(entry != 0)
		{
			writeLength(stream, (unsigned long)0);
			writeString(stream, entry->data + entry->nameLen, entry->valueLen);
			entry = entry->next;
		}
	}
}

#ifdef	__cplusplus
};
#endif
