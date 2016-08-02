/*
 * writer.c - Program writer for IL executable images.
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

#include "program.h"

#ifdef IL_USE_WRITER

#include <time.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Default version data to embed in the metadata header.
 * It will be padded with NULL bytes to a multiple of 4 in size.
 * There has to be at least one NULL byte at the end.
 */
#define	VERSION_STRING		IL_VERSION_METADATA "\0\0\0"
#define	VERSION_STRING_LEN	((sizeof(IL_VERSION_METADATA) + 3) & ~3)

void _ILWBufferListInit(ILWBufferList *list)
{
	list->firstBuffer = 0;
	list->lastBuffer = 0;
	list->bytesUsed = 0;
	list->offset = 0;
}

void _ILWBufferListDestroy(ILWBufferList *list)
{
	ILWBuffer *buffer = list->firstBuffer;
	ILWBuffer *nextBuffer;
	while(buffer != 0)
	{
		nextBuffer = buffer->next;
		ILFree(buffer);
		buffer = nextBuffer;
	}
}

int _ILWBufferListAdd(ILWBufferList *list, const void *buffer, unsigned size)
{
	ILWBuffer *wbuffer;
	unsigned temp;

	while(size > 0)
	{
		if(!(list->lastBuffer) || list->bytesUsed >= IL_WRITE_BUFFER_SIZE)
		{
			/* We need another buffer */
			wbuffer = (ILWBuffer *)ILMalloc(sizeof(ILWBuffer));
			if(!wbuffer)
			{
				return 0;
			}
			wbuffer->next = 0;
			if(list->lastBuffer)
			{
				list->lastBuffer->next = wbuffer;
			}
			else
			{
				list->firstBuffer = wbuffer;
			}
			list->lastBuffer = wbuffer;
			list->bytesUsed = 0;
		}
		temp = size;
		if(temp > (IL_WRITE_BUFFER_SIZE - list->bytesUsed))
		{
			temp = IL_WRITE_BUFFER_SIZE - list->bytesUsed;
		}
		ILMemCpy(list->lastBuffer->data + list->bytesUsed, buffer, temp);
		buffer = (void *)(((unsigned char *)buffer) + temp);
		size -= temp;
		list->bytesUsed += temp;
		list->offset += temp;
	}

	return 1;
}

ILWriter *ILWriterCreate(FILE *stream, int seekable, int type, int flags)
{
	ILWriter *writer;

	/* Create the writer control structure */
	writer = (ILWriter *)ILMalloc(sizeof(ILWriter));
	if(!writer)
	{
		return writer;
	}

	/* Initialize the writer fields */
	writer->type = type;
	writer->flags = flags;
	writer->offset = 0;
	writer->currSeek = 0;
	writer->peOffset = 0;
	writer->optOffset = 0;
	writer->firstSection = 0;
	writer->runtimeOffset = 0;
	writer->indexRVA = 0;
	writer->entryPoint = 0;
	writer->sections = 0;
	writer->stream = stream;
	writer->seekable = (stream ? seekable : 0);
	writer->outOfMemory = 0;
	writer->writeFailed = 0;
	writer->debugTokens = 0;
	writer->numDebugTokens = 0;
	writer->maxDebugTokens = 0;
	writer->debugHash = 0;
	writer->backpatching = 0;
	writer->backpatchSeek = 0;
	writer->backpatchLen = 0;
	writer->backpatchBuf = 0;
	ILMemCpy(writer->versionString, VERSION_STRING, VERSION_STRING_LEN);

	/* Initialize buffer lists */
	_ILWBufferListInit(&(writer->streamBuffer));
	_ILWBufferListInit(&(writer->indexBlob));
	_ILWBufferListInit(&(writer->guidBlob));
	_ILWBufferListInit(&(writer->debugData));
	_ILWBufferListInit(&(writer->debugStrings));

	/* Initialize the fixup list */
	ILMemPoolInitType(&(writer->fixups), ILFixup, 0);
	writer->firstFixup = 0;
	writer->lastFixup = 0;

	/* Allocate the back-patching buffer.  If we run out of
	   memory, then disable the back-patch cache */
	if(seekable)
	{
		writer->backpatchBuf = (unsigned char *)ILMalloc
			(IL_WRITE_BUFFER_SIZE);
	}

	/* Write the headers to the output stream */
#ifdef IL_CONFIG_JAVA
	if(flags & IL_WRITEFLAG_JVM_MODE)
	{
		_ILWriteJavaHeaders(writer);
	}
	else
#endif
	{
		_ILWriteHeaders(writer);
	}

	/* Ready to go now */
	return writer;
}

void ILWriterSetVersionString(ILWriter *writer, const char *version)
{
	int len;
	if(!writer || !version)
	{
		return;
	}
	len = strlen(version);
	if(len >= VERSION_STRING_LEN)
	{
		len = VERSION_STRING_LEN - 1;
	}
	ILMemZero(writer->versionString, VERSION_STRING_LEN);
	ILMemCpy(writer->versionString, version, len);
}

void ILWriterInferVersionString(ILWriter *writer, ILImage *image)
{
	const char *version;
	int len;
	if(!writer || !image)
	{
		return;
	}
	version = ILImageMetaRuntimeVersion(image, &len);
	if(!version || len <= 0)
	{
		return;
	}
	if(len >= VERSION_STRING_LEN)
	{
		len = VERSION_STRING_LEN - 1;
	}
	ILMemZero(writer->versionString, VERSION_STRING_LEN);
	ILMemCpy(writer->versionString, version, len);
}

/*
 * Write the contents of a buffer list to the ".text" section.
 */
static void WriteWBufferList(ILWriter *writer, ILWBufferList *list)
{
	ILWBuffer *buffer = list->firstBuffer;
	while(buffer != 0)
	{
		if(buffer->next)
		{
			ILWriterTextWrite(writer, buffer->data, IL_WRITE_BUFFER_SIZE);
		}
		else
		{
			ILWriterTextWrite(writer, buffer->data, list->bytesUsed);
		}
		buffer = buffer->next;
	}
}

/*
 * Write a byte pool from an ILImage structure to a writer.
 */
static void WriteImagePool(ILWriter *writer, char *pool,
						   unsigned long poolSize,
						   ILStringBlock *poolBlocks)
{
	if(pool)
	{
		ILWriterTextWrite(writer, pool, poolSize);
	}
	else
	{
		while(poolBlocks != 0)
		{
			ILWriterTextWrite(writer, (const void *)(poolBlocks + 1),
							  poolBlocks->used);
			poolBlocks = poolBlocks->next;
		}
	}
}

static void AssignMemberTokens(ILImage *image, ILClass *info,
							   ILToken *nextField, ILToken *nextMethod,
							   ILToken *nextParam, ILToken *nextProperty,
							   ILToken *nextEvent)
{
	ILMember *member;
	ILParameter *param;
	ILToken firstProperty = *nextProperty;
	ILToken firstEvent = *nextEvent;

	/* Scan the members of this class */
	member = info->firstMember;
	while(member != 0)
	{
		if((member->programItem.token & IL_META_TOKEN_MASK) ==
					IL_META_TOKEN_MEMBER_REF)
		{
			/* Ignore MemberRef's, as they are probably vararg signatures */
			member = member->nextMember;
			continue;
		}
		if(member->kind == IL_META_MEMBERKIND_METHOD)
		{
			member->programItem.token = *nextMethod;
			member->programItem.image->tokenData
				[IL_META_TOKEN_METHOD_DEF >> 24]
				[(*nextMethod & ~IL_META_TOKEN_MASK) - 1] = (void *)member;
			*nextMethod += 1;
			param = ((ILMethod *)member)->parameters;
			while(param != 0)
			{
				param->programItem.token = *nextParam;
				member->programItem.image->tokenData
					[IL_META_TOKEN_PARAM_DEF >> 24]
					[(*nextParam & ~IL_META_TOKEN_MASK) - 1] = (void *)param;
				*nextParam += 1;
				param = param->next;
			}
		}
		else if(member->kind == IL_META_MEMBERKIND_FIELD)
		{
			member->programItem.token = *nextField;
			member->programItem.image->tokenData
				[IL_META_TOKEN_FIELD_DEF >> 24]
				[(*nextField & ~IL_META_TOKEN_MASK) - 1] = (void *)member;
			*nextField += 1;
		}
		else if(member->kind == IL_META_MEMBERKIND_PROPERTY)
		{
			member->programItem.token = *nextProperty;
			member->programItem.image->tokenData
				[IL_META_TOKEN_PROPERTY >> 24]
				[(*nextProperty & ~IL_META_TOKEN_MASK) - 1] = (void *)member;
			*nextProperty += 1;
		}
		else if(member->kind == IL_META_MEMBERKIND_EVENT)
		{
			member->programItem.token = *nextEvent;
			member->programItem.image->tokenData
				[IL_META_TOKEN_EVENT >> 24]
				[(*nextEvent & ~IL_META_TOKEN_MASK) - 1] = (void *)member;
			*nextEvent += 1;
		}
		member = member->nextMember;
	}

	/* Create PropertyMap and EventMap entries if necessary */
	if(firstProperty < *nextProperty)
	{
		ILPropertyMapCreate(image, 0, info,
							(ILProperty *)ILProperty_FromToken
								(image, firstProperty));
	}
	if(firstEvent < *nextEvent)
	{
		ILEventMapCreate(image, 0, info,
						 (ILEvent *)ILEvent_FromToken(image, firstEvent));
	}
}

/*
 * Sort the field and method members of all classes in
 * an image so that token codes for nested classes come
 * after those for their nesting parents.  This will
 * also make the tokens for fields and methods in each
 * class contiguous within the metadata index.
 */
static void SortClasses(ILImage *image)
{
	ILToken nextField;
	ILToken nextMethod;
	ILToken nextParam;
	ILToken nextProperty;
	ILToken nextEvent;
	ILClass *info;

	/* Set up the initial field, method, and parameter indices */
	nextField = IL_META_TOKEN_FIELD_DEF | 1;
	nextMethod = IL_META_TOKEN_METHOD_DEF | 1;
	nextParam = IL_META_TOKEN_PARAM_DEF | 1;
	nextProperty = IL_META_TOKEN_PROPERTY | 1;
	nextEvent = IL_META_TOKEN_EVENT | 1;

	/* Assign token codes to all classes in the correct order */
	info = 0;
	while((info = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, info)) != 0)
	{
		AssignMemberTokens(image, info, &nextField, &nextMethod,
						   &nextParam, &nextProperty, &nextEvent);
	}
}

#ifdef IL_CONFIG_JAVA
/*
 * Check if the class has a java const pool.
 */
static ILBool JavaHasConstPool(ILClass *info)
{
	ILClassExt *ext;

	if((ext = _ILClassExtFind(info, _IL_EXT_JAVA_CONSTPOOL)) != 0)
	{
		return (ext->un.javaConstPool.entries != 0);
	}
	return 0;
}
#endif

void ILWriterOutputMetadata(ILWriter *writer, ILImage *image)
{
	unsigned long start;
	unsigned long current;
	int numSections;
	unsigned char header[256];
	unsigned size;
	unsigned indexOffset;
	unsigned stringsOffset;
	unsigned usOffset;
	unsigned blobOffset;
	unsigned guidOffset;

#ifdef IL_CONFIG_JAVA
	if (writer->flags & IL_WRITEFLAG_JVM_MODE)
	{
		unsigned long numTokens;
		unsigned long numClasses;
		unsigned long token;
		ILClass *class;

		numTokens = ILImageNumTokens(image, IL_META_TOKEN_TYPE_DEF);

		/* count the number of classes */
		numClasses = 0;
		for(token = 1; token <= numTokens; ++token)
		{
			class = (ILClass*)ILImageTokenInfo(image, IL_META_TOKEN_TYPE_DEF | token);

			if(!strcmp(ILClass_Name(class), "<Module>") ||
			   (!ILClass_IsPublic(class) && !ILClass_IsPrivate(class)) ||
			   !JavaHasConstPool(class))
			{
				continue;
			}
			numClasses++;
		}
		
		if(numClasses > 1)
		{
			fprintf(stdout, "write failed: Multiple class output not yet supported\n");
			writer->writeFailed = 1;
			return;
		}

		for(token = 1; token <= numTokens; ++token)
		{
			class = (ILClass*)ILImageTokenInfo(image, IL_META_TOKEN_TYPE_DEF | token);

			if(!strcmp(ILClass_Name(class), "<Module>") ||
			   (!ILClass_IsPublic(class) && !ILClass_IsPrivate(class)) ||
			   !JavaHasConstPool(class))
			{
				continue;
			}
			WriteJavaClass(writer, class);
		}
	}
	else
#endif
	{

	/* Sort the FieldDef, MethodDef, and ParamDef tables to put all
	   of the class members into their final order */
	SortClasses(image);

	/* Compact the TypeRef and MethodRef tables */
	if(!(writer->outOfMemory) && !(writer->writeFailed))
	{
		_ILCompactReferences(image);
	}

	/* Apply token fixups to the code section */
	_ILWriteTokenFixups(writer);

	/* Write all metadata structures to their respective buffers */
	_ILWriteMetadataIndex(writer, image);

	/* Align the ".text" section on a 4-byte boundary */
	ILWriterTextAlign(writer);

	/* Count the number of metadata sections that we need */
	numSections = 0;
	if(writer->indexBlob.firstBuffer)
	{
		++numSections;
	}
	if(image->stringPoolSize)
	{
		++numSections;
	}
	if(image->blobPoolSize)
	{
		++numSections;
	}
	if(image->userStringPoolSize)
	{
		++numSections;
	}
	if(writer->guidBlob.offset)
	{
		++numSections;
	}

	/* Build the header for the metadata section */
	header[0]  = (unsigned char)'B';			/* Magic number */
	header[1]  = (unsigned char)'S';
	header[2]  = (unsigned char)'J';
	header[3]  = (unsigned char)'B';
	IL_WRITE_UINT16(header + 4, 1);				/* Major version */
	IL_WRITE_UINT16(header + 6, 1);				/* Minor version */
	IL_WRITE_UINT32(header + 8, 0);				/* Reserved */
	IL_WRITE_UINT32(header + 12, VERSION_STRING_LEN);
	ILMemCpy(header + 16, writer->versionString, VERSION_STRING_LEN);
	size = 16 + VERSION_STRING_LEN;
	IL_WRITE_UINT16(header + size, 0);			/* Flags */
	IL_WRITE_UINT16(header + size + 2, numSections);
	size += 4;

	/* Output the metadata directory entries */
	if(writer->indexBlob.firstBuffer)
	{
		indexOffset = size;
		size += 4;
		IL_WRITE_UINT32(header + size, writer->indexBlob.offset);
		size += 4;
		header[size++] = (unsigned char)'#';
		header[size++] = (unsigned char)'~';
		header[size++] = 0;
		header[size++] = 0;
	}
	else
	{
		indexOffset = 0;
	}
	if(image->stringPoolSize)
	{
		stringsOffset = size;
		size += 4;
		IL_WRITE_UINT32(header + size, image->stringPoolSize);
		size += 4;
		header[size++] = (unsigned char)'#';
		header[size++] = (unsigned char)'S';
		header[size++] = (unsigned char)'t';
		header[size++] = (unsigned char)'r';
		header[size++] = (unsigned char)'i';
		header[size++] = (unsigned char)'n';
		header[size++] = (unsigned char)'g';
		header[size++] = (unsigned char)'s';
		header[size++] = 0;
		header[size++] = 0;
		header[size++] = 0;
		header[size++] = 0;
	}
	else
	{
		stringsOffset = 0;
	}
	if(image->blobPoolSize)
	{
		blobOffset = size;
		size += 4;
		IL_WRITE_UINT32(header + size, image->blobPoolSize);
		size += 4;
		header[size++] = (unsigned char)'#';
		header[size++] = (unsigned char)'B';
		header[size++] = (unsigned char)'l';
		header[size++] = (unsigned char)'o';
		header[size++] = (unsigned char)'b';
		header[size++] = 0;
		header[size++] = 0;
		header[size++] = 0;
	}
	else
	{
		blobOffset = 0;
	}
	if(image->userStringPoolSize)
	{
		usOffset = size;
		size += 4;
		IL_WRITE_UINT32(header + size, image->userStringPoolSize);
		size += 4;
		header[size++] = (unsigned char)'#';
		header[size++] = (unsigned char)'U';
		header[size++] = (unsigned char)'S';
		header[size++] = 0;
	}
	else
	{
		usOffset = 0;
	}
	if(writer->guidBlob.offset)
	{
		guidOffset = size;
		size += 4;
		IL_WRITE_UINT32(header + size, writer->guidBlob.offset);
		size += 4;
		header[size++] = (unsigned char)'#';
		header[size++] = (unsigned char)'G';
		header[size++] = (unsigned char)'U';
		header[size++] = (unsigned char)'I';
		header[size++] = (unsigned char)'D';
		header[size++] = 0;
		header[size++] = 0;
		header[size++] = 0;
	}
	else
	{
		guidOffset = 0;
	}

	/* Back-patch the offsets into the directory entries */
	current = (unsigned long)size;
	if(indexOffset)
	{
		IL_WRITE_UINT32(header + indexOffset, current);
		current += ((writer->indexBlob.offset + 3) & ~3);
	}
	if(stringsOffset)
	{
		IL_WRITE_UINT32(header + stringsOffset, current);
		current += ((image->stringPoolSize + 3) & ~3);
	}
	if(blobOffset)
	{
		IL_WRITE_UINT32(header + blobOffset, current);
		current += ((image->blobPoolSize + 3) & ~3);
	}
	if(usOffset)
	{
		IL_WRITE_UINT32(header + usOffset, current);
		current += ((image->userStringPoolSize + 3) & ~3);
	}
	if(guidOffset)
	{
		IL_WRITE_UINT32(header + guidOffset, current);
	}

	/* Write the metadata section header */
	start = ILWriterGetTextRVA(writer);
	ILWriterTextWrite(writer, header, size);

	/* Write the contents of the blobs */
	writer->indexRVA = ILWriterGetTextRVA(writer);
	WriteWBufferList(writer, &(writer->indexBlob));
	ILWriterTextAlign(writer);
	WriteImagePool(writer, image->stringPool,
				   image->stringPoolSize, image->stringBlocks);
	ILWriterTextAlign(writer);
	WriteImagePool(writer, image->blobPool,
				   image->blobPoolSize, image->blobBlocks);
	ILWriterTextAlign(writer);
	WriteImagePool(writer, image->userStringPool,
				   image->userStringPoolSize, image->userStringBlocks);
	ILWriterTextAlign(writer);
	WriteWBufferList(writer, &(writer->guidBlob));
	ILWriterTextAlign(writer);

	/* Update the IL runtime header with the metadata section's position */
	ILWriterUpdateHeader(writer, IL_IMAGEENTRY_METADATA,
						 start, ILWriterGetTextRVA(writer) - start);
	}
}

int ILWriterDestroy(ILWriter *writer)
{
	int retval;
	ILWSection *section, *nextSection;

	if (writer->flags & IL_WRITEFLAG_JVM_MODE)
	{

	}
	else
	{
		/* Write the debug information */
		if(writer->debugTokens)
		{
			_ILWriteDebug(writer);
		}
		
		/* Finalize the PE/COFF output */
		_ILWriteFinal(writer);
	}

	/* Free the section table */
	section = writer->sections;
	while(section != 0)
	{
		nextSection = section->next;
		if(section->buffer)
		{
			ILFree(section->buffer);
		}
		ILFree(section);
		section = nextSection;
	}

	/* Free the buffer lists */
	_ILWBufferListDestroy(&(writer->streamBuffer));
	_ILWBufferListDestroy(&(writer->indexBlob));
	_ILWBufferListDestroy(&(writer->guidBlob));
	_ILWBufferListDestroy(&(writer->debugData));
	_ILWBufferListDestroy(&(writer->debugStrings));

	/* Free the fixup pool */
	ILMemPoolDestroy(&(writer->fixups));

	/* Free the debug token table */
	if(writer->debugTokens)
	{
		ILFree(writer->debugTokens);
	}
	if(writer->debugHash)
	{
		ILHashDestroy(writer->debugHash);
	}

	/* Free the back-patch buffer */
	if(writer->backpatchBuf)
	{
		ILFree(writer->backpatchBuf);
	}

	/* Determine the return value */
	if(writer->outOfMemory)
	{
		retval = -1;
	}
	else if(writer->writeFailed)
	{
		retval = 0;
	}
	else
	{
		retval = 1;
	}

	/* Free the writer itself and then exit */
	ILFree(writer);
	return retval;
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_USE_WRITER */
