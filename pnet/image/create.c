/*
 * create.c - Routines for creating images in memory.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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
 * Destroy a string block list.
 */
void _ILImageDestroyBlockList(ILStringBlock *list)
{
	ILStringBlock *next;
	while(list != 0)
	{
		next = list->next;
		ILFree(list);
		list = next;
	}
}

/*
 * Destroy function for the ILImageBuilder class.
 */
static void DestroyBuilder(ILImage *image)
{
	ILImageBuilder *builder = (ILImageBuilder *)image;
	_ILImageDestroyBlockList(image->stringBlocks);
	_ILImageDestroyBlockList(image->blobBlocks);
	ILMemPoolDestroy(&(builder->hashPool));
}

ILImage *ILImageCreate(ILContext *context)
{
	ILImage *image = _ILImageCreate(context, sizeof(ILImageBuilder));
	ILImageBuilder *builder;
	if(image)
	{
		image->destroy = DestroyBuilder;
		image->type = IL_IMAGETYPE_BUILDING;
		builder = (ILImageBuilder *)image;
		ILMemPoolInitType(&(builder->hashPool), ILStringHash, 0);
	}
	return image;
}

/*
 * Look in the string hash for a particular string.
 * The supplied length must include the NUL terminator.
 * Returns NULL if not found.
 */
static ILStringHash *LookupHash(ILImageBuilder *builder,
								const void *str, ILUInt32 len,
								const void *header, ILUInt32 headerLen,
								ILUInt32 type, unsigned long hash)
{
	ILStringHash *entry;
	ILUInt32 fullLen = len + headerLen;
	entry = builder->hashTable[hash];
	while(entry != 0)
	{
		if(entry->len == (fullLen | type) &&
		   (!headerLen || !ILMemCmp(entry->value, header, headerLen)) &&
		   !ILMemCmp(entry->value + headerLen, str, len))
		{
			return entry;
		}
		entry = entry->next;
	}
	return 0;
}

/*
 * Add a string to a block list and return the offset.
 * The supplied length must include the NUL terminator.
 */
static ILUInt32 AddString(ILStringBlock **list,
							   const void *str, ILUInt32 len,
							   const void *header, ILUInt32 headerLen,
							   char **finalStr)
{
	ILStringBlock *last = *list;
	ILStringBlock *block;
	ILUInt32 offset = 0;
	ILUInt32 blockLen;
	ILUInt32 fullLen = len + headerLen;

	/* Do we have space for the string in the last block? */
	if(last != 0)
	{
		while(last->next != 0)
		{
			offset += last->used;
			last = last->next;
		}
		offset += last->used;
		if((last->len - last->used) >= fullLen)
		{
			*finalStr = (((char *)(last + 1)) + last->used);
			if(headerLen)
			{
				ILMemCpy(*finalStr, header, headerLen);
			}
			ILMemCpy((*finalStr) + headerLen, str, len);
			last->used += fullLen;
			return offset;
		}
	}

	/* Allocate a new block */
	blockLen = fullLen;
	if(!last)
	{
		/* This is the first block, so output a NUL first */
		++blockLen;
	}
	if(blockLen < IL_NORMAL_BLOCK_SIZE)
	{
		blockLen = IL_NORMAL_BLOCK_SIZE;
	}
	block = (ILStringBlock *)ILMalloc(blockLen + sizeof(ILStringBlock));
	if(!block)
	{
		return 0;
	}
	if(last)
	{
		last->next = block;
	}
	else
	{
		*list = block;
	}

	/* Initialize the block to hold the new string */
	if(last)
	{
		block->used = fullLen;
		block->len = blockLen;
		block->next = 0;
		*finalStr = ((char *)(block + 1));
		if(headerLen)
		{
			ILMemCpy(*finalStr, header, headerLen);
		}
		ILMemCpy((*finalStr) + headerLen, str, len);
		return offset;
	}
	else
	{
		/* First block, so add the empty string first */
		block->used = fullLen + 1;
		block->len = blockLen;
		block->next = 0;
		*finalStr = ((char *)(block + 1));
		(*finalStr)[0] = '\0';
		block->next = 0;
		++(*finalStr);
		if(headerLen)
		{
			ILMemCpy(*finalStr, header, headerLen);
		}
		ILMemCpy((*finalStr) + headerLen, str, len);
		return offset + 1;
	}
}

ILUInt32 ILImageAddString(ILImage *image, const char *str)
{
	ILImageBuilder *builder = (ILImageBuilder *)image;
	ILUInt32 len;
	unsigned long offset;
	char *finalStr;
	ILStringHash *entry;
	unsigned long hash;

	/* Bail out early if this is the null string or if
	   we are not in the process of building an image */
	if(image->type != IL_IMAGETYPE_BUILDING || !str)
	{
		return 0;
	}

	/* Search the hash table to see if we already have this string */
	len = strlen(str) + 1;
	hash = ILHashString(0, str, (int)(len - 1));
	hash &= (IL_STRING_HASH_SIZE - 1);
	entry = LookupHash(builder, str, len, 0, 0, IL_STRING_HASH_NORMAL, hash);
	if(entry)
	{
		return entry->offset;
	}

	/* Add the string to the block list */
	offset = AddString(&(image->stringBlocks), str, len, 0, 0, &finalStr);
	if(!offset)
	{
		return 0;
	}

	/* Add the string to the hash table */
	entry = ILMemPoolAlloc(&(builder->hashPool), ILStringHash);
	if(!entry)
	{
		return 0;
	}
	entry->value = finalStr;
	entry->len = len | IL_STRING_HASH_NORMAL;
	entry->offset = (ILUInt32)offset;
	entry->next = builder->hashTable[hash];
	builder->hashTable[hash] = entry;
	image->stringPoolSize = offset + len;
	return offset;
}

ILUInt32 ILImageAddBlob(ILImage *image, const void *blob,
							 ILUInt32 len)
{
	ILImageBuilder *builder = (ILImageBuilder *)image;
	unsigned long offset;
	char *finalStr;
	ILStringHash *entry;
	unsigned long hash;
	unsigned char header[IL_META_COMPRESS_MAX_SIZE];
	ILUInt32 headerLen;

	/* Bail out if we are not in the process of building an image */
	if(image->type != IL_IMAGETYPE_BUILDING)
	{
		return 0;
	}

	/* Build the blob header, which encodes its length */
	headerLen = (ILUInt32)(ILMetaCompressData(header, len));

	/* Search the hash table to see if we already have this blob */
	hash = ILHashString(0, (const char *)header, headerLen);
	hash = ILHashString(hash, (const char *)blob, (int)len);
	hash &= (IL_STRING_HASH_SIZE - 1);
	entry = LookupHash(builder, blob, len, header, headerLen,
					   IL_STRING_HASH_BLOB, hash);
	if(entry)
	{
		return entry->offset;
	}

	/* Add the blob to the block list */
	offset = AddString(&(image->blobBlocks), blob, len,
					   header, headerLen, &finalStr);
	if(!offset)
	{
		return 0;
	}

	/* Add the blob to the hash table */
	entry = ILMemPoolAlloc(&(builder->hashPool), ILStringHash);
	if(!entry)
	{
		return 0;
	}
	entry->value = finalStr;
	entry->len = len | IL_STRING_HASH_BLOB;
	entry->offset = (ILUInt32)offset;
	entry->next = builder->hashTable[hash];
	builder->hashTable[hash] = entry;
	image->blobPoolSize = offset + headerLen + len;
	return offset;
}

ILUInt32 ILImageAddUserString(ILImage *image, const char *str, int len)
{
	int posn;
	ILUInt32 wlen;
	unsigned char *buf;
	int outposn;
	ILImageBuilder *builder = (ILImageBuilder *)image;
	unsigned long offset;
	char *finalStr;
	ILStringHash *entry;
	unsigned long hash;

	/* Bail out if we are not in the process of building an image */
	if(image->type != IL_IMAGETYPE_BUILDING)
	{
		return 0;
	}

	/* Compute the length if not specified */
	if(len < 0)
	{
		len = strlen(str);
	}

	/* How long is the string in 16-bit Unicode characters? */
	posn = 0;
	wlen = 0;
	while(posn < len)
	{
		wlen += ILUTF16WriteChar(0, ILUTF8ReadChar(str, len, &posn));
	}

	/* Convert the string into its 16-bit Unicode form */
	if((buf = (unsigned char *)ILMalloc(wlen * 2 + 6)) == 0)
	{
		return 0;
	}
	outposn = ILMetaCompressData(buf, wlen * 2 + 1);
	posn = 0;
	while(posn < len)
	{
		outposn += ILUTF16WriteCharAsBytes
			(buf + outposn, ILUTF8ReadChar(str, len, &posn));
	}
	buf[outposn++] = 0;

	/* Search the hash table to see if we already have this string */
	hash = ILHashString(0, (const char *)buf, (int)outposn);
	hash &= (IL_STRING_HASH_SIZE - 1);
	entry = LookupHash(builder, buf, outposn, 0, 0,
					   IL_STRING_HASH_UNICODE, hash);
	if(entry)
	{
		ILFree(buf);
		return entry->offset;
	}

	/* Add the string to the block list */
	offset = AddString(&(image->userStringBlocks), buf, outposn,
					   0, 0, &finalStr);
	if(!offset)
	{
		ILFree(buf);
		return 0;
	}

	/* Add the string to the hash table */
	entry = ILMemPoolAlloc(&(builder->hashPool), ILStringHash);
	if(!entry)
	{
		ILFree(buf);
		return 0;
	}
	entry->value = finalStr;
	entry->len = outposn | IL_STRING_HASH_UNICODE;
	entry->offset = (ILUInt32)offset;
	entry->next = builder->hashTable[hash];
	builder->hashTable[hash] = entry;
	image->userStringPoolSize = offset + outposn;
	ILFree(buf);
	return offset;
}

ILUInt32 ILImageAddEncodedUserString(ILImage *image,
										  const void *str, int len)
{
	ILImageBuilder *builder = (ILImageBuilder *)image;
	ILUInt32 offset;
	char *finalStr;
	ILStringHash *entry;
	unsigned long hash;
	unsigned char header[IL_META_COMPRESS_MAX_SIZE];
	ILUInt32 headerLen;
	ILUInt32 strLen;

	/* Bail out if we are not in the process of building an image */
	if(image->type != IL_IMAGETYPE_BUILDING)
	{
		return 0;
	}

	/* Encode the string header */
	strLen = ((ILUInt32)len) * 2 + 1;
	headerLen = (ILUInt32)ILMetaCompressData(header, strLen);

	/* Search the hash table to see if we already have this string */
	hash = ILHashString(0, (const char *)header, headerLen);
	hash = ILHashString(hash, (const char *)str, (int)strLen);
	hash &= (IL_STRING_HASH_SIZE - 1);
	entry = LookupHash(builder, str, strLen, header, headerLen,
					   IL_STRING_HASH_UNICODE, hash);
	if(entry)
	{
		return entry->offset;
	}

	/* Add the string to the block list */
	offset = AddString(&(image->userStringBlocks), str, strLen,
					   header, headerLen, &finalStr);
	if(!offset)
	{
		return 0;
	}

	/* Add the string to the hash table */
	entry = ILMemPoolAlloc(&(builder->hashPool), ILStringHash);
	if(!entry)
	{
		return 0;
	}
	entry->value = finalStr;
	entry->len = (headerLen + strLen) | IL_STRING_HASH_UNICODE;
	entry->offset = (ILUInt32)offset;
	entry->next = builder->hashTable[hash];
	builder->hashTable[hash] = entry;
	image->userStringPoolSize = offset + headerLen + strLen;
	return offset;
}

#ifdef	__cplusplus
};
#endif
