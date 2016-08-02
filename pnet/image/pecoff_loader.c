/*
 * pecoff_loader.c - Deal with the ugly parts of loading PE/COFF images.
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
#if defined(IL_CONFIG_GZIP) && defined(HAVE_ZLIB_H) && defined(HAVE_LIBZ)
	#define	IL_USE_GZIP	1
#endif
#ifdef IL_USE_GZIP
	#include <zlib.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Input context.
 */
typedef struct _tagILInputContext ILInputContext;
struct _tagILInputContext
{
#ifndef REDUCED_STDIO
	FILE		   *stream;
#endif
	const char	   *buffer;
	unsigned long	bufLen;
	int (*readFunc)(ILInputContext *ctx, void *buf, unsigned len);
#ifdef IL_USE_GZIP
	int				sawEOF;
	z_streamp		gzStream;
	ILInputContext *linked;
#endif
};

#ifndef REDUCED_STDIO

/*
 * Stdio-based read operation.
 */
static int StdioRead(ILInputContext *ctx, void *buf, unsigned len)
{
	return (int)fread(buf, 1, len, ctx->stream);
}

#else

#define	fileno(f)	0

#endif

/*
 * Memory-based read operation.
 */
static int MemoryRead(ILInputContext *ctx, void *buf, unsigned len)
{
	if(ctx->bufLen < len)
	{
		len = (unsigned)(ctx->bufLen);
	}
	if(len > 0)
	{
		ILMemCpy(buf, ctx->buffer, len);
		ctx->buffer += len;
		ctx->bufLen -= len;
	}
	return (int)len;
}

#ifdef IL_USE_GZIP

/*
 * Gzip flag bits in the header.
 */
#define GZ_ASCII_FLAG   0x01
#define GZ_HEAD_CRC     0x02
#define GZ_EXTRA_FIELD  0x04
#define GZ_ORIG_NAME    0x08
#define GZ_COMMENT      0x10
#define GZ_RESERVED     0xE0

/*
 * Fill the gzip read buffer if necessary.
 */
static void GzipFillRead(ILInputContext *ctx)
{
	if(ctx->gzStream->avail_in == 0 && !(ctx->sawEOF))
	{
		int temp = (*(ctx->linked->readFunc))
			(ctx->linked, (void *)(ctx->buffer), BUFSIZ);
		if(temp <= 0)
		{
			ctx->gzStream->avail_in = 0;
			ctx->sawEOF = 1;
		}
		else
		{
			ctx->gzStream->avail_in = (unsigned)temp;
		}
		ctx->gzStream->next_in = (Bytef *)(ctx->buffer);
	}
}

/*
 * Read a single byte from the read buffer.
 */
static int GzipReadByte(ILInputContext *ctx)
{
	GzipFillRead(ctx);
	if(ctx->gzStream->avail_in > 0)
	{
		--(ctx->gzStream->avail_in);
		return *(ctx->gzStream->next_in)++;
	}
	else
	{
		return -1;
	}
}

/*
 * Read a short value from the read buffer.
 */
static int GzipReadShort(ILInputContext *ctx)
{
	int b1 = GzipReadByte(ctx);
	int b2 = GzipReadByte(ctx);
	return (b1 | (b2 << 8));
}

/*
 * Skip a number of bytes in the read buffer.
 */
static void GzipSkip(ILInputContext *ctx, int len)
{
	while(len > 0 && GzipReadByte(ctx) != -1)
	{
		--len;
	}
}

/*
 * Skip a NUL-terminated string in the read buffer.
 */
static void GzipSkipString(ILInputContext *ctx)
{
	int b;
	while((b = GzipReadByte(ctx)) != -1 && b != 0)
	{
		/* Nothing to do here */
	}
}

/*
 * Gzip-based read operation.  This version doesn't check CRC's
 * or handle concatenated gzip files.  Fix later if required.
 */
static int GzipRead(ILInputContext *ctx, void *buf, unsigned len)
{
	int error;

	/* Initialize the zlib output buffer */
	ctx->gzStream->next_out = (Bytef *)buf;
	ctx->gzStream->avail_out = len;

	/* Read data and write it to the output buffer */
	while(ctx->gzStream->avail_out != 0 && !(ctx->sawEOF))
	{
		/* Read more data from the input stream if necessary */
		GzipFillRead(ctx);

		/* Inflate the input data */
		error = inflate(ctx->gzStream, Z_NO_FLUSH);
		if(error == Z_STREAM_END)
		{
			/* We've reached the end of the gzip data: skip the CRC value */
			GzipSkip(ctx, 4);
			ctx->sawEOF = 1;
		}
		else if(error != Z_OK)
		{
			/* Some kind of error occurred in the input data */
			ctx->sawEOF = 1;
		}
	}

	/* Read the length of the uncompressed data to the caller */
	return (int)(len - ctx->gzStream->avail_out);
}


/*
 * Finalize gzip control structures in an input context.
 */
static void GzipFinalize(ILInputContext *ctx)
{
	ILInputContext *linked;
	if(ctx->gzStream)
	{
		inflateEnd(ctx->gzStream);
		ILFree(ctx->gzStream);
		ILFree((char *)(ctx->buffer));
		linked = ctx->linked;
		*ctx = *linked;
		ILFree(linked);
	}
}

/*
 * Gzip initialize operation.  It is assumed that the first two
 * bytes of the gzip stream (containing the magic number) have
 * already been read.
 */
static int GzipInitialize(ILInputContext *ctx)
{
	ILInputContext *linked;
	char *buffer;
	z_streamp gzStream;
	int method, flags, len;

	/* Make a copy of the context for performing linked reads */
	linked = (ILInputContext *)ILMalloc(sizeof(ILInputContext));
	if(!linked)
	{
		return 0;
	}
	*linked = *ctx;

	/* Allocate a buffer for holding the gzip input data */
	buffer = (char *)ILMalloc(BUFSIZ);
	if(!buffer)
	{
		ILFree(linked);
		return 0;
	}

	/* Initialize the gzip stream details */
	gzStream = (z_streamp)ILCalloc(1, sizeof(z_stream));
	if(!gzStream)
	{
		ILFree(buffer);
		ILFree(linked);
		return 0;
	}
	if(inflateInit2(gzStream, -MAX_WBITS) != Z_OK)
	{
		ILFree(gzStream);
		ILFree(buffer);
		ILFree(linked);
		return 0;
	}

	/* Set up the input context for gzip-based reads */
	ctx->stream = 0;
	ctx->buffer = (const char *)buffer;
	ctx->bufLen = 0;
	ctx->readFunc = GzipRead;
	ctx->gzStream = gzStream;
	ctx->linked = linked;
	ctx->sawEOF = 0;

	/* Skip past the gzip header */
	method = GzipReadByte(ctx);
	flags = GzipReadByte(ctx);
	if(method != Z_DEFLATED || (flags & GZ_RESERVED) != 0)
	{
		GzipFinalize(ctx);
		return 0;
	}
	GzipSkip(ctx, 6);		/* Time, extra flags, and OS code */
	if((flags & GZ_EXTRA_FIELD) != 0)
	{
		len = GzipReadShort(ctx);
		GzipSkip(ctx, len);
	}
	if((flags & GZ_ORIG_NAME) != 0)
	{
		GzipSkipString(ctx);
	}
	if((flags & GZ_COMMENT) != 0)
	{
		GzipSkipString(ctx);
	}
	if((flags & GZ_HEAD_CRC) != 0)
	{
		GzipSkip(ctx, 2);
	}

	/* The stream is ready to go */
	return 1;
}

#endif /* IL_USE_GZIP */

/*
 * Seek to a particular offset within a stream by reading
 * and discarding data.  This is designed to work on streams
 * that may not necessarily be file-based.  Returns zero if
 * the seek failed.
 */
static int SeekWithinStream(ILInputContext *ctx, char *buffer,
							unsigned long currentOffset,
							unsigned long destOffset)
{
	unsigned size;
	while(currentOffset < destOffset)
	{
		if((destOffset - currentOffset) > 1024)
		{
			size = 1024;
		}
		else
		{
			size = (unsigned)(destOffset - currentOffset);
		}
		if((*(ctx->readFunc))(ctx, buffer, size) != (int)size)
		{
			return 0;
		}
		currentOffset += size;
	}
	return (currentOffset == destOffset);
}

/*
 * Convert a virtual address into a real address using a section map.
 * Returns IL_BAD_ADDRESS if the virtual address is outside all sections.
 */
static unsigned long ConvertVirtAddrToReal(ILSectionMap *map,
									   	   unsigned long addr)
{
	while(map != 0)
	{
		if(addr >= map->virtAddr &&
		   addr < (map->virtAddr + map->virtSize))
		{
			return (addr - map->virtAddr + map->realAddr);
		}
		map = map->next;
	}
	return IL_BAD_ADDRESS;
}

/*
 * Free a section map.
 */
void _ILFreeSectionMap(ILSectionMap *map)
{
	ILSectionMap *next;
	while(map != 0)
	{
		next = map->next;
		ILFree(map);
		map = next;
	}
}

/*
 * Find the top of a virtual address range that contains
 * a particular address.  Returns zero if none of the
 * ranges contain the address.
 */
static unsigned long TopOfVirtualRange(ILSectionMap *map, unsigned long addr)
{
	while(map != 0)
	{
		if(addr >= map->virtAddr &&
		   addr < (map->virtAddr + map->virtSize))
		{
			return map->virtAddr + map->virtSize;
		}
		map = map->next;
	}
	return 0;
}

void _ILImageFreeMemory(ILImage *image)
{
	if(image->mapped)
	{
		ILUnmapFileFromMemory(image->mapAddress, image->mapLength);
	}
	else if(image->data && !(image->inPlace))
	{
		ILFree(image->data);
	}
}

static int ImageLoad(ILInputContext *ctx, const char *filename,
					 ILContext *context, ILImage **image, int flags)
{
	char buffer[1024];
	int isDLL;
	int isOBJ = 0;
	int hadNative;
	int only32Bit = 0;
	unsigned headerSize;
	unsigned numSections;
	unsigned long numDirectories;
	unsigned long virtualBase;
	unsigned long base;
	unsigned long size;
	unsigned long runtimeHdrSize;
	unsigned long offset;
	unsigned long minAddress;
	unsigned long maxAddress;
	unsigned long debugRVA;
	unsigned long debugSize;
	unsigned long dataRVA;
	unsigned long dataSize;
	unsigned long tlsRVA;
	unsigned long tlsSize;
	unsigned long rsrcRVA;
	unsigned long rsrcSize;
	ILSectionMap *map;
	ILSectionMap *newMap;
	char *data;
	unsigned char *runtimeHdr;
	int isMapped;
	int isInPlace;
	int isCorMeta;
	void *mapAddress;
	unsigned long mapLength;
	int error;

	/* Read the first 2 bytes and look for either an MS-DOS
	   stub (executables and DLL's), or the beginning of a
	   PE/COFF header (object files) */
	if((*(ctx->readFunc))(ctx, buffer, 2) != 2)
	{
		return IL_LOADERR_TRUNCATED;
	}
#ifdef IL_USE_GZIP
	if(buffer[0] == (char)0x1F && buffer[1] == (char)0x8B)
	{
		/* The image has been compressed with gzip, so layer a decompression
		   object on top of the underlying input context */
		if(!GzipInitialize(ctx))
		{
			return IL_LOADERR_NOT_PE;
		}

		/* Suppress the use of mmap to load the stream's contents */
		flags |= IL_LOADFLAG_NO_MAP;

		/* Disable in-place execution */
		flags &= ~IL_LOADFLAG_IN_PLACE;

		/* Re-read the magic number bytes from the uncompressed data */
		if((*(ctx->readFunc))(ctx, buffer, 2) != 2)
		{
			return IL_LOADERR_TRUNCATED;
		}
	}
#endif
	if(buffer[0] == 'M' && buffer[1] == 'Z')
	{
		/* Read the MS-DOS stub and find the start of the PE header */
		if((*(ctx->readFunc))(ctx, buffer + 2, 62) != 62)
		{
			return IL_LOADERR_TRUNCATED;
		}
		offset = 64;
		base = IL_READ_UINT32(buffer + 60);
		if(base < offset)
		{
			return IL_LOADERR_BACKWARDS;
		}
		if(!SeekWithinStream(ctx, buffer, offset, base))
		{
			return IL_LOADERR_TRUNCATED;
		}
		offset = base;
		if((*(ctx->readFunc))(ctx, buffer, 4) != 4)
		{
			return IL_LOADERR_TRUNCATED;
		}
		offset += 4;
		if(buffer[0] != 'P' || buffer[1] != 'E' ||
		   buffer[2] != '\0' || buffer[3] != '\0')
		{
			return IL_LOADERR_NOT_PE;
		}
		if((*(ctx->readFunc))(ctx, buffer, 20) != 20)
		{
			return IL_LOADERR_TRUNCATED;
		}
		offset += 20;
	}
	else if(buffer[0] == (char)0x4C && buffer[1] == (char)0x01)
	{
		/* This is an i386 PE/COFF object file: read the rest of the header */
		if((*(ctx->readFunc))(ctx, buffer + 2, 18) != 18)
		{
			return IL_LOADERR_TRUNCATED;
		}
		offset = 20;
		isOBJ = 1;
	}
#ifdef IL_CONFIG_JAVA
	else if((buffer[0] == (char)0xCA && buffer[1] == (char)0xFE) ||
	        (buffer[0] == 'P' && buffer[1] == 'K'))
	{
		/* This looks like a Java ".class" or ".jar" file, which
		   we need to pass off to "_ILImageJavaLoad" to handle */
		if(ctx->stream)
		{
			return _ILImageJavaLoad(ctx->stream, filename, context,
									image, flags, buffer);
		}
		else
		{
			return IL_LOADERR_NOT_PE;
		}
	}
#endif
	else if(buffer[0] == '!' && buffer[1] == '<')
	{
		/* This may be an "ar" archive file: read the rest of the header */
		if((*(ctx->readFunc))(ctx, buffer, 6) != 6)
		{
			return IL_LOADERR_NOT_PE;
		}
		if(buffer[0] == 'a' && buffer[1] == 'r' && buffer[2] == 'c' &&
		   buffer[3] == 'h' && buffer[4] == '>' && buffer[5] == '\n')
		{
			return IL_LOADERR_ARCHIVE;
		}
		return IL_LOADERR_NOT_PE;
	}
	else
	{
		/* Unknown file format */
		return IL_LOADERR_NOT_PE;
	}

	/* Extract interesting information from the PE/COFF header */
	isDLL = ((IL_READ_UINT16(buffer + 18) & 0x2000) != 0);
	headerSize = (unsigned)(IL_READ_UINT16(buffer + 16));
	numSections = (unsigned)(IL_READ_UINT16(buffer + 2));
	if(headerSize != 0 && (headerSize < 216 || headerSize > 1024))
	{
		return IL_LOADERR_NOT_IL;
	}
	if(numSections == 0)
	{
		return IL_LOADERR_NOT_IL;
	}

	/* Read the optional header into memory.  This should contain
	   the data directory information for the IL runtime header */
	if(headerSize != 0)
	{
		if((*(ctx->readFunc))(ctx, buffer, headerSize) != headerSize)
		{
			return IL_LOADERR_TRUNCATED;
		}
		offset += headerSize;
		if(buffer[0] != 0x0B || buffer[1] != 0x01)
		{
			return IL_LOADERR_NOT_PE;
		}
		numDirectories = IL_READ_UINT32(buffer + 92);
		if(numDirectories < 15)
		{
			return IL_LOADERR_NOT_IL;
		}
		base = IL_READ_UINT32(buffer + 208);
		runtimeHdrSize = IL_READ_UINT32(buffer + 212);
		if(runtimeHdrSize < 48 || runtimeHdrSize > 1024)
		{
			return IL_LOADERR_NOT_IL;
		}
	}
	else
	{
		/* We don't have an optional header, so we will need to
		   extract the runtime header from the ".text$il" section */
		base = 0;
		runtimeHdrSize = 0;
	}

	/* Read the COFF section table.  We need this to be able
	   to convert virtual addresses into file seek offsets */
	map = 0;
	minAddress = (unsigned long)IL_MAX_UINT32;
	maxAddress = 0;
	debugRVA = 0;
	debugSize = 0;
	dataRVA = 0;
	dataSize = 0;
	tlsRVA = 0;
	tlsSize = 0;
	rsrcRVA = 0;
	rsrcSize = 0;
	isCorMeta = 0;
	while(numSections > 0)
	{
		if((*(ctx->readFunc))(ctx, buffer, 40) != 40)
		{
			_ILFreeSectionMap(map);
			return IL_LOADERR_TRUNCATED;
		}
		offset += 40;
		if((newMap = (ILSectionMap *)ILMalloc(sizeof(ILSectionMap))) == 0)
		{
			_ILFreeSectionMap(map);
			return IL_LOADERR_MEMORY;
		}
		newMap->virtAddr = IL_READ_UINT32(buffer + 12);
		newMap->virtSize = IL_READ_UINT32(buffer + 8);
		newMap->realAddr = IL_READ_UINT32(buffer + 20);
		newMap->realSize = IL_READ_UINT32(buffer + 16);
		if(newMap->virtSize > newMap->realSize)
		{
			/* Paranoia check - usually won't happen */
			newMap->virtSize = newMap->realSize;
		}
		else if(!(newMap->virtSize))
		{
			/* Object files may set this field to zero */
			newMap->virtSize = newMap->realSize;
			if(!(newMap->virtAddr))
			{
				newMap->virtAddr = newMap->realAddr;
			}
		}
		if(((newMap->virtAddr + newMap->virtSize) &
					(unsigned long)IL_MAX_UINT32) < newMap->virtAddr)
		{
			/* The virtual size is too big */
			_ILFreeSectionMap(map);
			return IL_LOADERR_NOT_PE;
		}
		if(((newMap->realAddr + newMap->realSize) &
					(unsigned long)IL_MAX_UINT32) < newMap->realAddr)
		{
			/* The real size is too big */
			_ILFreeSectionMap(map);
			return IL_LOADERR_NOT_PE;
		}
		newMap->next = map;
		map = newMap;
		if(!runtimeHdrSize && !ILMemCmp(buffer, ".text$il", 8))
		{
			/* We are processing an object file that has the
			   IL data embedded within the ".text$il" section */
			base = newMap->virtAddr;
		}
		else if(!runtimeHdrSize && !ILMemCmp(buffer, ".cormeta", 8))
		{
			/* We are processing an object file that has the
			   IL data embedded within the ".cormeta" section */
			base = newMap->virtAddr;
			isCorMeta = 1;
		}
		else if(!ILMemCmp(buffer, ".ildebug", 8))
		{
			debugRVA = newMap->virtAddr;
			debugSize = newMap->virtSize;
		}
		else if(!ILMemCmp(buffer, ".sdata\0\0", 8) && !dataRVA)
		{
			dataRVA = newMap->virtAddr;
			dataSize = newMap->virtSize;
		}
		else if(!ILMemCmp(buffer, ".data\0\0\0", 8) && !dataRVA)
		{
			dataRVA = newMap->virtAddr;
			dataSize = newMap->virtSize;
		}
		else if(!ILMemCmp(buffer, ".tls\0\0\0\0", 8))
		{
			tlsRVA = newMap->virtAddr;
			tlsSize = newMap->virtSize;
		}
		else if(!ILMemCmp(buffer, ".rsrc\0\0\0", 8))
		{
			rsrcRVA = newMap->virtAddr;
			rsrcSize = newMap->virtSize;
		}
		else if(!ILMemCmp(buffer, ".bss\0\0\0\0", 8))
		{
			/* Object files with ".cormeta" sections may have an
			   empty ".bss" section present.  Ignore it when getting
			   the minimum and maximum sizes */
			if(!(newMap->realAddr))
			{
				--numSections;
				continue;
			}
		}
		if(newMap->realSize > 0)
		{
			if(newMap->realAddr < minAddress)
			{
				minAddress = newMap->realAddr;
			}
			if((newMap->realAddr + newMap->realSize) > maxAddress)
			{
				maxAddress = newMap->realAddr + newMap->realSize;
			}
		}
		--numSections;
	}

	/* If the maximum address is less than the minimum, then there
	   are no sections in the file, and it cannot possibly be IL */
	if(maxAddress <= minAddress)
	{
		_ILFreeSectionMap(map);
		return IL_LOADERR_NOT_IL;
	}

	/* If we don't have a runtime header yet, then bail out.
	   This can happen if we are processing an object file that
	   does not have a ".text$il" section within it */
	if(!runtimeHdrSize && !base)
	{
		_ILFreeSectionMap(map);
		return IL_LOADERR_NOT_IL;
	}

	/* Seek to the beginning of the first section */
	if(!SeekWithinStream(ctx, buffer, offset, minAddress))
	{
		_ILFreeSectionMap(map);
		return IL_LOADERR_TRUNCATED;
	}

	/* Map the contents of every section into memory.  We would like
	   to only map those parts of the file that are relevant to IL, but
	   there is too much variation in how compilers lay out binaries.
	   In particular, the IL bytecode can be pretty much anywhere */
	if((flags & IL_LOADFLAG_NO_MAP) == 0 &&
	   ILMapFileToMemory(fileno(ctx->stream), minAddress, maxAddress,
					     &mapAddress, &mapLength, &data))
	{
		isMapped = 1;
		isInPlace = 0;
	}
#ifndef REDUCED_STDIO
	else if(!(ctx->stream) && (flags & IL_LOADFLAG_IN_PLACE) != 0)
#else
	else if((flags & IL_LOADFLAG_IN_PLACE) != 0)
#endif
	{
		/* Execute directly from the supplied buffer */
		data = (char *)(ctx->buffer);
		isMapped = 0;
		mapAddress = 0;
		mapLength = 0;
		isInPlace = 1;
	}
	else
	{
		/* Read the IL program data into memory */
		if((data = (char *)ILMalloc(maxAddress - minAddress)) == 0)
		{
			_ILFreeSectionMap(map);
			return IL_LOADERR_MEMORY;
		}
		if((*(ctx->readFunc))(ctx, data, maxAddress - minAddress) !=
					(maxAddress - minAddress))
		{
			ILFree(data);
			_ILFreeSectionMap(map);
			return IL_LOADERR_TRUNCATED;
		}
		isMapped = 0;
		mapAddress = 0;
		mapLength = 0;
		isInPlace = 0;
	}

	/* Adjust the section map to account for the new location of the program */
	newMap = map;
	while(newMap != 0)
	{
		if(newMap->realAddr)
		{
			newMap->realAddr -= minAddress;
		}
		newMap = newMap->next;
	}

	/* Convert the virtual address of the runtime header into a real address */
	virtualBase = base;
	base = ConvertVirtAddrToReal(map, base);
	if(base == IL_BAD_ADDRESS)
	{
		_ILFreeSectionMap(map);
		return IL_LOADERR_BAD_ADDR;
	}

	/* Find the IL runtime header, read it, and validate it */
	runtimeHdr = (unsigned char *)data + base;
	size = (maxAddress - minAddress) - base;
	if(runtimeHdrSize)
	{
		/* We already know how big the runtime header is from
		   the contents of the optional header */
		if(size < runtimeHdrSize)
		{
			_ILFreeSectionMap(map);
			return IL_LOADERR_TRUNCATED;
		}
		if(IL_READ_UINT32(runtimeHdr) != runtimeHdrSize)
		{
			_ILFreeSectionMap(map);
			return IL_LOADERR_NOT_IL;
		}
	}
	else if(isCorMeta)
	{
		/* We are processing an object file that stores the metadata
		   header at the start of the ".cormeta" section */
		runtimeHdrSize = 0;
		hadNative = 0;
		only32Bit = 0;
		goto noRuntimeHeader;
	}
	else
	{
		/* We are processing an object file that stores the runtime
		   header at the start of the ".text$il" section */
		if(size < 4)
		{
			_ILFreeSectionMap(map);
			return IL_LOADERR_TRUNCATED;
		}
		runtimeHdrSize = IL_READ_UINT32(runtimeHdr);
		if(runtimeHdrSize < 48 || runtimeHdrSize > 1024)
		{
			_ILFreeSectionMap(map);
			return IL_LOADERR_NOT_IL;
		}
		if(size < runtimeHdrSize)
		{
			_ILFreeSectionMap(map);
			return IL_LOADERR_TRUNCATED;
		}
	}
	if(IL_READ_UINT16(runtimeHdr + 4) != 2) /* ||
	   IL_READ_UINT16(runtimeHdr + 6) != 0)*/
	{
		_ILFreeSectionMap(map);
		return IL_LOADERR_VERSION;
	}
	hadNative = ((IL_READ_UINT32(runtimeHdr + 16) & 0x00000001) == 0);
#ifdef IL_NATIVE_INT64
	if((IL_READ_UINT32(runtimeHdr + 16) & 0x00000002) != 0)
	{
		if((flags & IL_LOADFLAG_FORCE_32BIT) != 0)
		{
			only32Bit = 1;
		}
		else
		{
			_ILFreeSectionMap(map);
			return IL_LOADERR_32BIT_ONLY;
		}
	}
#endif
noRuntimeHeader:

	/* Create and populate the ILImage structure */
	if((*image = _ILImageCreate(context, sizeof(ILImage))) == 0)
	{
		if(isMapped)
		{
			ILUnmapFileFromMemory(mapAddress, mapLength);
		}
		else if(!isInPlace)
		{
			ILFree(data);
		}
		_ILFreeSectionMap(map);
		return IL_LOADERR_MEMORY;
	}
	if(filename)
	{
		/* Save the filename for later use in dynamic linking */
		(*image)->filename = ILExpandFilename(filename, (char *)0);
		if(!((*image)->filename))
		{
			if(isMapped)
			{
				ILUnmapFileFromMemory(mapAddress, mapLength);
			}
			else if(!isInPlace)
			{
				ILFree(data);
			}
			_ILFreeSectionMap(map);
			return IL_LOADERR_MEMORY;
		}
	}
	(*image)->loadFlags = flags;
	(*image)->type = (isOBJ ? IL_IMAGETYPE_OBJ :
						(isDLL ? IL_IMAGETYPE_DLL : IL_IMAGETYPE_EXE));
	(*image)->secure = ((flags & IL_LOADFLAG_INSECURE) ? 0 : -1);
	(*image)->hadNative = hadNative ? -1 : 0;
	(*image)->only32Bit = only32Bit ? -1 : 0;
	(*image)->mapped = isMapped ? -1 : 0;
	(*image)->inPlace = isInPlace ? -1 : 0;
	(*image)->map = map;
	(*image)->data = data;
	(*image)->len = (maxAddress - minAddress);
	(*image)->mapAddress = mapAddress;
	(*image)->mapLength = mapLength;
	(*image)->headerAddr = virtualBase;
	(*image)->headerSize = runtimeHdrSize;
	(*image)->realStart = minAddress;
	(*image)->debugRVA = debugRVA;
	(*image)->debugSize = debugSize;
	(*image)->dataRVA = dataRVA;
	(*image)->dataSize = dataSize;
	(*image)->tlsRVA = tlsRVA;
	(*image)->tlsSize = tlsSize;
	(*image)->rsrcRVA = rsrcRVA;
	(*image)->rsrcSize = rsrcSize;

	/* Mark the metadata as loading so that we can detect TypeRef recursion */
	(*image)->loading = -1;
	++(context->redoLevel);

	/* Load the meta information from the image */
	if((flags & IL_LOADFLAG_NO_METADATA) == 0)
	{
		error = _ILImageParseMeta(*image, filename, flags);
	}
	else
	{
		error = 0;
	}

	/* The metadata is now fully loaded for this image */
	(*image)->loading = 0;
	if(--(context->redoLevel) == 0 && context->numRedoItems > 0)
	{
		/* We've exited all recursive loading levels, so redo queued items */
		if(!error || (flags & IL_LOADFLAG_IGNORE_ERRORS) != 0)
		{
			error = _ILImageRedoReferences(context);
		}

		/* Free the "redo" table, which we no longer need */
		ILFree(context->redoItems);
		context->redoItems = 0;
		context->numRedoItems = 0;
		context->maxRedoItems = 0;
	}
	if((flags & IL_LOADFLAG_IGNORE_ERRORS) != 0)
	{
		/* We don't care about metadata errors */
		error = 0;
	}

	/* The image is loaded and ready to go */
	if(error)
	{
		ILImageDestroy(*image);
	}
	return error;
}

#ifndef REDUCED_STDIO

int ILImageLoad(FILE *file, const char *filename,
				ILContext *context, ILImage **image, int flags)
{
	ILInputContext ctx;
	int error;
	ctx.stream = file;
	ctx.buffer = 0;
	ctx.bufLen = 0;
	ctx.readFunc = StdioRead;
#ifdef IL_USE_GZIP
	ctx.sawEOF = 0;
	ctx.gzStream = 0;
	ctx.linked = 0;
#endif
	error = ImageLoad(&ctx, filename, context, image, flags);
#ifdef IL_USE_GZIP
	GzipFinalize(&ctx);
#endif
	return error;
}

int ILImageLoadFromFile(const char *filename, ILContext *context,
						ILImage **image, int flags, int printErrors)
{
	FILE *file;
	int closeStream;
	int loadError;

	/* Open the specified file */
	if(!strcmp(filename, "-"))
	{
		file = stdin;
		closeStream = 0;
	}
	else if((file = fopen(filename, "rb")) == NULL)
	{
		/* Try again, in case libc does not understand "rb" */
		if((file = fopen(filename, "r")) == NULL)
		{
			if(printErrors)
			{
				perror(filename);
			}
			return -1;
		}
		closeStream = 1;
	}
	else
	{
		closeStream = 1;
	}

	/* Load the file as an image */
	loadError = ILImageLoad(file, filename, context, image, flags);
	if(closeStream)
	{
		fclose(file);
	}

	/* Report errors to stderr, if necessary */
	if(loadError != 0 && printErrors)
	{
		fprintf(stderr, "%s: %s\n", (closeStream ? filename : "stdin"),
				ILImageLoadError(loadError));
	}

	/* Done */
	return loadError;
}

#endif /* !REDUCED_STDIO */

int ILImageLoadFromMemory(const void *buffer, unsigned long bufLen,
						  ILContext *context, ILImage **image,
						  int flags, const char *filename)
{
	ILInputContext ctx;
	int error;
#ifndef REDUCED_STDIO
	ctx.stream = 0;
#endif
	ctx.buffer = (const char *)buffer;
	ctx.bufLen = bufLen;
	ctx.readFunc = MemoryRead;
#ifdef IL_USE_GZIP
	ctx.sawEOF = 0;
	ctx.gzStream = 0;
	ctx.linked = 0;
#endif
	error = ImageLoad(&ctx, filename, context,
					  image, flags | IL_LOADFLAG_NO_MAP);
#ifdef IL_USE_GZIP
	GzipFinalize(&ctx);
#endif
	return error;
}

void *ILImageMapAddress(ILImage *image, unsigned long address)
{
	unsigned long realAddr = ConvertVirtAddrToReal(image->map, address);
	if(realAddr != IL_BAD_ADDRESS)
	{
		return (void *)(image->data + realAddr);
	}
	else
	{
		return (void *)0;
	}
}

void *ILImageMapRVA(ILImage *image, unsigned long rva, unsigned long *len)
{
	ILSectionMap *map = image->map;
	while(map != 0)
	{
		if(rva >= map->virtAddr &&
		   rva < (map->virtAddr + map->virtSize))
		{
			/* The RVA is within this PE/COFF section */
			rva = rva - map->virtAddr + map->realAddr;
			*len = map->realSize - (rva - map->realAddr);
			return (void *)(image->data + rva);
		}
		map = map->next;
	}
	return 0;
}

unsigned long ILImageRealOffset(ILImage *image, unsigned long address)
{
	unsigned long realAddr = ConvertVirtAddrToReal(image->map, address);
	if(realAddr != IL_BAD_ADDRESS)
	{
		return image->realStart + realAddr;
	}
	else
	{
		return 0;
	}
}

/*
 * Range data structure.
 */
#define	IL_MAX_RANGES		16
typedef struct
{
	unsigned long	start;
	unsigned long	end;

} ILRange;
typedef struct
{
	ILRange	range[IL_MAX_RANGES];
	int		numRanges;

} ILRanges;

/*
 * Subtract a consequetive sequence of bytes from a range list.
 */
static void SubtractFromRange(ILRanges *ranges,
							  unsigned long start,
							  unsigned long end)
{
	int posn = 0;
	while(posn < ranges->numRanges)
	{
		if(start <= ranges->range[posn].start &&
		   end > ranges->range[posn].start)
		{
			/* Overlap with the start of the range, so shorten at the start */
			if(end >= ranges->range[posn].end)
			{
				ranges->range[posn].start = ranges->range[posn].end;
			}
			else
			{
				ranges->range[posn].start = end;
			}
		}
		else if(start < ranges->range[posn].end &&
		        end >= ranges->range[posn].end)
		{
			/* Overlap with the end of the range, so shorten at the end */
			if(start <= ranges->range[posn].start)
			{
				ranges->range[posn].end = ranges->range[posn].start;
			}
			else
			{
				ranges->range[posn].end = start;
			}
		}
		else if(start >= ranges->range[posn].start &&
				end <= ranges->range[posn].end)
		{
			/* Overlap with the middle of the range, so split into two.
			   If we have run out of space, then leave the list as-is */
			if(ranges->numRanges < IL_MAX_RANGES)
			{
				ranges->range[ranges->numRanges].start = end;
				ranges->range[ranges->numRanges].end = ranges->range[posn].end;
				ranges->range[posn].end = start;
				++(ranges->numRanges);
			}
		}
		++posn;
	}
}

/*
 * Trim any range that ends at a particular high address.
 */
static void TrimHighRange(ILRanges *ranges, unsigned long address)
{
	int posn;
	for(posn = 0; posn < ranges->numRanges; ++posn)
	{
		if(ranges->range[posn].end == address)
		{
			ranges->range[posn].start = ranges->range[posn].end;
		}
	}
}

/*
 * Find the largest range that still exists.  This is probably the code.
 */
static void FindLargestRange(ILRanges *ranges, unsigned long *start,
							 unsigned long *size)
{
	unsigned long end;
	unsigned long newSize;
	int posn;
	*start = ranges->range[0].start;
	end = ranges->range[0].end;
	*size = (end - *start);
	for(posn = 1; posn < ranges->numRanges; ++posn)
	{
		newSize = ranges->range[posn].end - ranges->range[posn].start;
		if(newSize > *size)
		{
			*start = ranges->range[posn].start;
			end = ranges->range[posn].end;
		}
	}
}

int _ILImageGetSection(ILImage *image, int section,
				  	   unsigned long *address, ILUInt32 *size)
{
	unsigned char *runtimeHdr;
	unsigned char *probe;

	/* Find the runtime header */
	runtimeHdr = (unsigned char *)ILImageMapAddress(image, image->headerAddr);
	if(!runtimeHdr)
	{
		return 0;
	}

	/* Find the section from the index information in the runtime header */
	switch(section)
	{
		case IL_SECTION_HEADER:
		{
			/* Get the address and size of the IL runtime header */
			*address = image->headerAddr;
			*size = image->headerSize;
		}
		break;

		case IL_SECTION_CODE:
		{
			/* The code section is the odd one out in IL binaries,
			   because it doesn't actually have a header field
			   associated with it.  We start with the full range
			   and subtract sections we know about.  Whatever is
			   left must be the code section */
			ILRanges ranges;
			unsigned long addrLowest;
			unsigned long addrHighest;
			unsigned long addrTest;
			unsigned long sizeTest;
			unsigned long headerPosn;

			/* Find the initial lower and upper bounds on the entire image */
			addrLowest = image->headerAddr + image->headerSize;
			addrHighest = TopOfVirtualRange(image->map, addrLowest);
			ranges.range[0].start = addrLowest;
			ranges.range[0].end   = addrHighest;
			ranges.numRanges = 1;

			/* Subtract out the resources section, if present */
			if(image->headerSize > 0)
			{
				addrTest = IL_READ_UINT32(runtimeHdr + 24);
				sizeTest = IL_READ_UINT32(runtimeHdr + 28);
			}
			else
			{
				addrTest = 0;
				sizeTest = 0;
			}
			if(addrTest != 0 && sizeTest != 0 && addrTest < addrHighest)
			{
				if((addrTest + 136) <= addrHighest && sizeTest >= 136)
				{
					/* It looks like there were bugs in some very early
					   code generators that output the start of the
					   resource section as 128 bytes before it actually
					   starts.  This overlaps with the end of the code
					   section, making it appear as though the code section
					   is truncated.  We probe the resources to see if this
					   is the case.  If so, we add an extra 128 bytes to
					   the code section size.  Yuk!! */
					probe = ILImageMapAddress(image, addrTest);
					if(probe &&
					   probe[132] == (unsigned char)0xCE &&
					   probe[133] == (unsigned char)0xCA &&
					   probe[134] == (unsigned char)0xEF &&
					   probe[135] == (unsigned char)0xBE)
					{
						/* We found the magic number, so adjust the size */
						addrTest += 128;
						sizeTest -= 128;
					}
				}
			}
			SubtractFromRange(&ranges, addrTest, addrTest + sizeTest);

			/* Subtract out the metadata section, if present */
			if(image->headerSize > 0)
			{
				addrTest = IL_READ_UINT32(runtimeHdr + 8);
				sizeTest = IL_READ_UINT32(runtimeHdr + 12);
				if(addrTest != 0 && sizeTest != 0)
				{
					SubtractFromRange(&ranges, addrTest, addrTest + sizeTest);
				}
			}

			/* Subtract out other sections, starting at the
			   strong name signature section */
			headerPosn = 32;
			while((headerPosn + 8) <= image->headerSize)
			{
				addrTest = IL_READ_UINT32(runtimeHdr + headerPosn);
				sizeTest = IL_READ_UINT32(runtimeHdr + headerPosn + 4);
				if(addrTest != 0 && sizeTest != 0)
				{
					SubtractFromRange(&ranges, addrTest, addrTest + sizeTest);
				}
				headerPosn += 8;
			}

			/* Trim any range that ends at "addrHighest", because
			   that will contain left-over data of no interest */
			TrimHighRange(&ranges, addrHighest);

			/* Return whatever is left to the caller */
			FindLargestRange(&ranges, &addrTest, &sizeTest);
			if(!sizeTest)
			{
				/* Somehow we ended up with no data at all.  This
				   may happen if the code is in the range that we
				   trimmed above.  This is a very rare case, but if
				   it occurs, then return everything to the caller.
				   This is the safest fallback position */
				addrTest = addrLowest;
				sizeTest = addrHighest - addrLowest;
			}
			if(sizeTest)
			{
				*address = addrTest;
				*size = sizeTest;
				return 1;
			}
			else
			{
				return 0;
			}
		}
		break;

		case IL_SECTION_METADATA:
		{
			/* Locate the metadata section */
			if(image->headerSize)
			{
				*address = IL_READ_UINT32(runtimeHdr + 8);
				*size = IL_READ_UINT32(runtimeHdr + 12);
			}
			else
			{
				*address = image->headerAddr;
				*size = TopOfVirtualRange(image->map, *address);
				if(*size > 0)
				{
					*size -= *address;
				}
			}
		}
		break;

		case IL_SECTION_RESOURCES:
		{
			/* Locate the resources section */
			if(!(image->headerSize))
			{
				*address = 0;
				*size = 0;
				break;
			}
			*address = IL_READ_UINT32(runtimeHdr + 24);
			*size = IL_READ_UINT32(runtimeHdr + 28);
			if(*size >= 136)
			{
				/* Do we need to correct for buggy code generators
				   that overlap the code and resource sections? */
				probe = ILImageMapAddress(image, *address);
				if(probe &&
				   probe[132] == (unsigned char)0xCE &&
				   probe[133] == (unsigned char)0xCA &&
				   probe[134] == (unsigned char)0xEF &&
				   probe[135] == (unsigned char)0xBE)
				{
					/* We found the magic number, so correct the size */
					*address += 128;
					*size -= 128;
				}
			}
		}
		break;

		case IL_SECTION_STRONG_NAMES:
		{
			/* Locate the strong name signature section */
			if(!(image->headerSize))
			{
				*address = 0;
				*size = 0;
				break;
			}
			*address = IL_READ_UINT32(runtimeHdr + 32);
			*size = IL_READ_UINT32(runtimeHdr + 36);
		}
		break;

		case IL_SECTION_CODE_MANAGER:
		{
			/* Locate the code manager table section */
			if(!(image->headerSize))
			{
				*address = 0;
				*size = 0;
				break;
			}
			*address = IL_READ_UINT32(runtimeHdr + 40);
			*size = IL_READ_UINT32(runtimeHdr + 44);
		}
		break;

		case IL_SECTION_DEBUG:
		{
			/* Debug information section */
			*address = image->debugRVA;
			*size = image->debugSize;
		}
		break;

		case IL_SECTION_DATA:
		{
			/* Data section */
			*address = image->dataRVA;
			*size = image->dataSize;
		}
		break;

		case IL_SECTION_TLS:
		{
			/* TLS data section */
			*address = image->tlsRVA;
			*size = image->tlsSize;
		}
		break;

		case IL_SECTION_WINRES:
		{
			/* Windows resource section */
			*address = image->rsrcRVA;
			*size = image->rsrcSize;
		}
		break;

		default: return 0;
	}
	return (*address != 0 && *size != 0);
}

#ifdef	__cplusplus
};
#endif
