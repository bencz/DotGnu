/*
 * image.h - Internal definitions for IL image handling.
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

#ifndef	_IMAGE_H
#define	_IMAGE_H

#include "il_values.h"
#include "il_system.h"
#include "il_utils.h"
#include "il_program.h"
#if !defined(__palmos__)
#define	IL_USE_WRITER
#endif
#ifdef	IL_USE_WRITER
#include "il_writer.h"
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Read big-endian quantities of various sizes.
 */
#define	IL_BREAD_INT16(buf)	((ILInt16)(_IL_READ_BYTE((buf), 1) | \
									   _IL_READ_BYTE_SHIFT((buf), 0, 8)))
#define	IL_BREAD_UINT16(buf) ((ILUInt16)(_IL_READ_BYTE((buf), 1) | \
									     _IL_READ_BYTE_SHIFT((buf), 0, 8)))
#define	IL_BREAD_INT32(buf)	((ILInt32)(_IL_READ_BYTE((buf), 3) | \
									   _IL_READ_BYTE_SHIFT((buf), 2, 8) | \
									   _IL_READ_BYTE_SHIFT((buf), 1, 16) | \
									   _IL_READ_BYTE_SHIFT((buf), 0, 24)))
#define	IL_BREAD_UINT32(buf)	((ILUInt32)(_IL_READ_BYTE((buf), 3) | \
									    _IL_READ_BYTE_SHIFT((buf), 2, 8) | \
									    _IL_READ_BYTE_SHIFT((buf), 1, 16) | \
									    _IL_READ_BYTE_SHIFT((buf), 0, 24)))
#define	IL_BREAD_INT64(buf)	\
			(((ILInt64)(IL_BREAD_UINT32((buf) + 4))) | \
			 (((ILInt64)(IL_BREAD_INT32((buf)))) << 32))
#define	IL_BREAD_UINT64(buf)	\
			(((ILUInt64)(IL_BREAD_UINT32((buf) + 4))) | \
			 (((ILUInt64)(IL_BREAD_UINT32((buf)))) << 32))

/*
 * Structure of a context, which holds multiple loaded images,
 * and the information about the classes, methods, etc, in them.
 */
#define	IL_CONTEXT_HASH_SIZE	509
#define	IL_CONTEXT_NS_HASH_SIZE	67
struct _tagILContext
{
	/* Hash table that maps class names to class information blocks */
	ILHashTable    *classHash;

	/* List of images associated with the context */
	ILImage		   *firstImage;

	/* Special images within the context */
	ILImage		   *systemImage;
	ILImage		   *syntheticImage;

	/* Hash table that holds the synthetic type definitions */
	ILHashTable	   *syntheticHash;

	/* Memory pool that holds ILType records */
	ILMemPool		typePool;

	/* Library directories for pathname searching */
	char		  **libraryDirs;
	int				numLibraryDirs;

	/* Base directory of the application */
	char		   *applicationBaseDir;

	/* Directory relative to applicationBaseDir to search for assemblies */
	char		   *relativeSearchDir;

	/* cache path for shadow copies */
	char		   *cacheDir;

	/* Directory where dynamically created assemblies are stored and accessed */
	char		   *dynamicBaseDir;

	/* Flag if shadow copying is enabled */
	int				shadowCopyFiles;

	/* Directory names that are to be cached */
	char		  **shadowCopyDirs;
	int				numShadowCopyDirs;

	/* Hash table to keeps track of valid namespaces */
	ILHashTable	   *namespaceHash;

	/* User data for the runtime engine */
	void *userData;

	/* Reference "redo" table.  This is the list of references that
	   must be redone to fix up recursive TypeRef's and MemberRef's */
	ILProgramItem **redoItems;
	ILUInt32		numRedoItems;
	ILUInt32		maxRedoItems;
	ILUInt32		redoLevel;

};

/*
 * Key information to use when looking up an entry in the class hash.
 */
typedef struct _tagILClassName ILClassName;
typedef struct
{
	const char	   *name;
	const char	   *namespace;
	ILProgramItem  *scopeItem;
	ILClassName    *scopeName;
	ILImage		   *image;
	int				nameLen;
	int				namespaceLen;
	int				wantGlobal;
	int				ignoreCase;

} ILClassKeyInfo;

/*
 * Structure of a section map entry.  This is used to map
 * virtual addresses within the executable image into real
 * seek offsets within the file or the final ILImage object.
 */
typedef struct _tagILSectionMap
{
	unsigned long virtAddr;
	unsigned long virtSize;
	unsigned long realAddr;
	unsigned long realSize;
	struct _tagILSectionMap *next;

} ILSectionMap;
#define	IL_BAD_ADDRESS	((unsigned long)0xFFFFFFFF)

/*
 * String block.
 */
#define	IL_NORMAL_BLOCK_SIZE	1024
typedef struct _tagILStringBlock ILStringBlock;
struct _tagILStringBlock
{
	ILStringBlock  *next;
	ILUInt32		used;
	ILUInt32		len;

};

/*
 * Definition of the loaded image data structure.
 */
struct _tagILImage
{
	/* Context linkage */
	ILContext	   *context;		/* Context associated with the image */
	ILImage		   *nextImage;		/* Next image in the context */
	ILImage		   *prevImage;		/* Previous image in the context */

	/* Destroy function that can be overridden by subclasses */
	void		  (*destroy)(ILImage *image);

	/* File that the image was loaded file */
	char		   *filename;		/* NULL if filename is unknown */

	/* Flags that were used during loading */
	int				loadFlags;

	/* Housekeeping for managing the raw PE/COFF for of an image */
	int				type : 8;		/* Type of image */
	int				secure : 1;		/* Non-zero if image source is secure */
	int				hadNative : 1;	/* Non-zero if image had native code */
	int				only32Bit : 1;	/* Non-zero if image is 32-bit only */
	int				mapped : 1;		/* Non-zero if mmap used to load image */
	int				inPlace : 1;	/* Non-zero if in-place execution */
	int				strRefBig : 1;	/* Non-zero if STRREF's are 32-bit */
	int				blobRefBig : 1;	/* Non-zero if BLOBREF's are 32-bit */
	int				guidRefBig : 1;	/* Non-zero if GUIDREF's are 32-bit */
	int				loading : 1;	/* Non-zero if still loading metadata */
	ILSectionMap   *map;			/* Maps virtual to real addresses */
	char		   *data;			/* Data that makes up the IL image */
	unsigned long	len;			/* Length of the IL image */
	void		   *mapAddress;		/* Address file is mmap'ed to */
	unsigned long	mapLength;		/* Length of mmap'ed region */
	unsigned long	headerAddr;		/* Virtual address of the header */
	ILUInt32		headerSize;		/* Size of the runtime header */
	unsigned long	realStart;		/* Real file offset of "data" */
	unsigned long	debugRVA;		/* RVA for the start of the debug data */
	ILUInt32		debugSize;		/* Size of the debug data */
	unsigned long	dataRVA;		/* RVA for the start of ".sdata" */
	ILUInt32		dataSize;		/* Size of ".sdata" */
	unsigned long	tlsRVA;			/* RVA for the start of ".tls" */
	ILUInt32		tlsSize;		/* Size of ".tls" */
	unsigned long	rsrcRVA;		/* RVA for the start of ".rsrc" */
	ILUInt32		rsrcSize;		/* Size of ".rsrc" */

	/* Memory stack that is used to allocate program objects */
	ILMemStack		memStack;

	/* String pool information from the metadata section */
	char		   *stringPool;
	ILUInt32		stringPoolSize;
	ILStringBlock  *stringBlocks;
	ILStringBlock  *extraStrings;

	/* Blob pool information from the metadata section */
	char		   *blobPool;
	ILUInt32		blobPoolSize;
	ILStringBlock  *blobBlocks;

	/* User string pool information from the metadata section */
	char		   *userStringPool;
	ILUInt32		userStringPoolSize;
	ILStringBlock  *userStringBlocks;

	/* Token table */
	ILUInt32		tokenCount[64];	/* Number of tokens of each type */
	unsigned char	tokenSize[64];	/* Size of each token's record */
	unsigned char  *tokenStart[64];	/* Start of each token table */
	void		  **tokenData[64];	/* Data associated with the tokens */
	ILUInt64		sorted;			/* Token tables that are sorted */

};

/*
 * Hash entry information for string block lists.
 */
typedef struct _tagILStringHash ILStringHash;
struct _tagILStringHash
{
	char		   *value;
	ILUInt32		len;
	ILUInt32		offset;
	ILStringHash   *next;

};
#define	IL_STRING_HASH_SIZE		512	/* Must be a power of 2 */

/*
 * String types within the string hash table.  Stored in "ILStringHash::len".
 */
#define	IL_STRING_HASH_NORMAL	((ILUInt32)0x00000000)
#define	IL_STRING_HASH_UNICODE	((ILUInt32)0x40000000)
#define	IL_STRING_HASH_BLOB		((ILUInt32)0x80000000)

/*
 * Information about a resource entry.
 */
typedef struct _tagILResourceEntry ILResourceEntry;
struct _tagILResourceEntry
{
	int					isDirectory : 1;
	int					isMallocData : 1;
	int					isNumeric : 1;
	char			   *name;
	int					nameLen;
	ILResourceEntry	   *children;
	ILResourceEntry	   *next;
	unsigned char	   *data;
	ILUInt32			length;
	ILUInt32			rva;

};

/*
 * Information about the resource section.
 */
struct _tagILResourceSection
{
	ILImage			   *image;
	unsigned char	   *data;
	ILResourceEntry	   *rootDirectory;
	ILUInt32			length;

};

/*
 * Subclass of ILImage which is used when building an image in memory.
 */
typedef struct _tagILImageBuilder
{
	/* Inherited fields */
	ILImage			base;

	/* Hash table information */
	ILMemPool		hashPool;		/* Memory pool for hash entries */
	ILStringHash   *hashTable[IL_STRING_HASH_SIZE];

} ILImageBuilder;

/*
 * Information about an extra (non-text) section.
 */
typedef struct _tagILWSection ILWSection;
struct _tagILWSection
{
	char			name[9];		/* Name of the section */
	unsigned char  *buffer;			/* Buffer containing section data */
	ILUInt32		length;			/* Length of the buffer */
	ILUInt32		maxLength;		/* Maximum length of the buffer */
	unsigned long	flags;			/* Flags associated with the section */
	ILWSection	   *next;			/* Next section in the section list */

};

/*
 * Data buffer list entry.
 */
#define	IL_WRITE_BUFFER_SIZE	2048
typedef struct _tagILWBuffer ILWBuffer;
struct _tagILWBuffer
{
	unsigned char	data[IL_WRITE_BUFFER_SIZE];
	ILWBuffer      *next;

};

/*
 * Data buffer list.
 */
typedef struct _tagILWBufferList
{
	ILWBuffer	   *firstBuffer;
	ILWBuffer	   *lastBuffer;
	unsigned		bytesUsed;
	ILUInt32		offset;

} ILWBufferList;

/*
 * Information that is kept for a debug token index entry.
 */
typedef struct _tagILDebugToken
{
	ILProgramItem  *item;
	unsigned long	pseudo;
	ILUInt32		offset;

} ILDebugToken;

/*
 * Information that is stored for a fixup entry.
 */
typedef struct _tagILFixup ILFixup;
struct _tagILFixup
{
	int					kind;
	ILUInt32			rva;
	union
	{
		ILProgramItem  *item;
		ILUInt32		value;
	} un;
	ILFixup        *next;

};
#define	IL_FIXUP_TOKEN		0
#define	IL_FIXUP_FIELD_RVA	1

#ifdef IL_USE_WRITER

/*
 * Internal structure used when writing IL binaries.
 */
struct _tagILWriter
{
	int				flags;			/* Flags that control the output */
	int				type;			/* Type of image (DLL, EXE, or OBJ) */
	unsigned long	offset;			/* Current output offset */
	unsigned long	currSeek;		/* Current seek offset */

	/* Offset information for the various image sections */
	ILUInt32		peOffset;		/* Offset of PE/COFF header */
	ILUInt32		optOffset;		/* Offset of optional header */
	ILUInt32		sectOffset;		/* Offset of the section table */
	ILUInt32		firstSection;	/* Offset of first section */
	ILUInt32		runtimeOffset;	/* Offset of the IL runtime header */
	ILUInt32		indexRVA;		/* RVA of the index blob */

	/* Entry point token */
	ILMethod	   *entryPoint;

	/* List of extra non-text sections within the image */
	ILWSection	   *sections;

	/* Output information */
	FILE		   *stream;			/* Stream to write to */
	int				seekable;		/* Non-zero if stream is seekable */

	/* Buffer list that is used for non-seekable streams */
	ILWBufferList	streamBuffer;

	/* Buffer lists for metadata sections */
	ILWBufferList	indexBlob;
	ILWBufferList	guidBlob;

	/* Debug information */
	ILWBufferList	debugData;
	ILWBufferList	debugStrings;
	ILDebugToken   *debugTokens;
	ILUInt32		numDebugTokens;
	ILUInt32		maxDebugTokens;
	ILHashTable	   *debugHash;

	/* Fixup information */
	ILMemPool		fixups;
	ILFixup        *firstFixup;
	ILFixup        *lastFixup;

	/* Error indicators */
	int				outOfMemory;	/* We ran out of memory during the write */
	int				writeFailed;	/* The write failed at some point */

	/* Back-patch cache */
	int				backpatching;	/* Non-zero if last write was back-patch */
	unsigned long	backpatchSeek;	/* Seek position of back-patch cache */
	unsigned long	backpatchLen;	/* Length of back-patch cache */
	unsigned char  *backpatchBuf;	/* Buffer containing back-patch data */

	/* Version string for the runtime header (e.g. "v1.1.4322") */
	unsigned char	versionString[12];

};

#endif /* IL_USE_WRITER */

/*
 * Create a persistent version of a string if necessary.
 * This is typically used on class names and the like which
 * may have been loaded from an image and so don't always
 * need to be copied into the dynamic heap.
 */
const char *_ILContextPersistString(ILImage *image, const char *str);

/*
 * Create a persistent version of a malloc'ed string.
 * This kind of string usually arises from somewhere
 * other than the "#Strings" metadata pool.
 */
const char *_ILContextPersistMalloc(ILImage *image, char *str);

/*
 * Internal image creation routine.
 */
ILImage *_ILImageCreate(ILContext *context, unsigned size);

#ifdef IL_CONFIG_JAVA

/*
 * Declare the Java image loading function for use by "ILImageLoad".
 */
int _ILImageJavaLoad(FILE *file, const char *filename, ILContext *context,
					 ILImage **image, int flags, char *buffer);

#endif /* IL_CONFIG_JAVA */

/*
 * Free a section map.
 */
void _ILFreeSectionMap(ILSectionMap *map);

/*
 * Free the loaded memory for an image.  The mechanism used
 * depends upon whether the memory was mmap'ed or malloc'ed.
 */
void _ILImageFreeMemory(ILImage *image);

/*
 * Get the starting address and size of a particular image section.
 */
int _ILImageGetSection(ILImage *image, int section,
				  	   unsigned long *address, ILUInt32 *size);

/*
 * Parse metadata and other information from a PE/COFF image
 * that has just been loaded into memory.  Returns a load
 * error, or zero if everything is OK.
 */
int _ILImageParseMeta(ILImage *image, const char *filename, int flags);

/*
 * Build the metadata structures after the index has been parsed.
 */
int _ILImageBuildMetaStructures(ILImage *image, const char *filename,
								int loadFlags);

/*
 * Redo TypeRef and MemberRef references that resulted from recursive
 * assembly references.
 */
int _ILImageRedoReferences(ILContext *context);

/*
 * Load a token information block on demand.
 */
void *_ILImageLoadOnDemand(ILImage *image, ILToken token);

/*
 * Determine if a token has already been loaded.
 */
int _ILImageTokenAlreadyLoaded(ILImage *image, ILToken token);

/*
 * Perform dynamic linking on an image to resolve all
 * assemblies that it refers to.  Returns a load error.
 */
int _ILImageDynamicLink(ILImage *image, const char *filename, int flags);

/*
 * Perform dynamic linking on a module reference.
 * Returns a load error
 */
int _ILImageDynamicLinkModule(ILImage *image, const char *filename,
							  const char *moduleName, int flags,
							  ILImage **newImage);

/*
 * Free the information associated with an image's tokens.
 */
void _ILImageFreeTokens(ILImage *image);

/*
 * Set the token details for a particular program item.
 * Returns zero if out of memory or if the token is invalid.
 */
int _ILImageSetToken(ILImage *image, ILProgramItem *item,
					 unsigned long token, unsigned long tokenKind);

/*
 * Search the raw values in an owned item table for a token match.
 * Returns the first token that matches in the "ownedType" table
 * in "firstMatch".  The number of tokens that match is returned
 * from the function, or zero if there are no matches.
 */
typedef int (*ILSearchRawFunc)(ILUInt32 *values, ILToken searchFor,
							   int valueField);
ILUInt32 _ILSearchForRawToken(ILImage *image, ILSearchRawFunc func,
							  ILToken ownedType, ILToken *firstMatch,
							  ILToken searchFor, int valueField);

#ifdef IL_USE_WRITER

/*
 * Compute the size of all token types prior to writing
 * the metadata to an image.
 */
void _ILImageComputeTokenSizes(ILImage *image);

/*
 * Write all headers and section details to an output image.
 */
void _ILWriteHeaders(ILWriter *writer);

/*
 * Write the metadata index information to an output image.
 * Returns zero if there are invalid records within "image".
 */
int _ILWriteMetadataIndex(ILWriter *writer, ILImage *image);

/*
 * Write debug information.
 */
void _ILWriteDebug(ILWriter *writer);

/*
 * Write the information that is necessary to finalize output of an image.
 */
void _ILWriteFinal(ILWriter *writer);

/*
 * Initialize a buffer list.
 */
void _ILWBufferListInit(ILWBufferList *list);

/*
 * Destroy a buffer list.
 */
void _ILWBufferListDestroy(ILWBufferList *list);

/*
 * Add a block of bytes to a buffer list.  Returns zero if out of memory.
 */
int _ILWBufferListAdd(ILWBufferList *list, const void *buffer, ILUInt32 size);

#endif /* IL_USE_WRITER */

/*
 * Remove all classes for an image from the class lookup hash.
 */
void _ILClassRemoveAllFromHash(ILImage *image);

/*
 * Convert an array type into a synthetic class that represents it.
 * Returns NULL if not possible, or out of memory.
 */
ILClass *_ILTypeToSyntheticArray(ILImage *image, ILType *type, int singleDim);

/*
 * Convert a non-array type into a synthetic class that represents it.
 * Returns NULL if not possible, or out of memory.
 */
ILClass *_ILTypeToSyntheticOther(ILImage *image, ILType *type);

/*
 * Look up a synthetic class for an instantiated type.
 * Returns NULL if not possible, or out of memory.
 */
ILClass *_ILTypeToSyntheticInstantiation
		(ILImage *image, ILType *type, ILType *classArgs, ILType *methodArgs);

#ifdef IL_USE_WRITER

/*
 * Compact all type and member references in an image to
 * remove tokens that have been replaced with definitions.
 */
void _ILCompactReferences(ILImage *image);

/*
 * Write token fixups to the ".text" section.
 */
void _ILWriteTokenFixups(ILWriter *writer);

/*
 * Write field RVA fixups to the ".text" section.
 */
void _ILWriteFieldRVAFixups(ILWriter *writer, unsigned long dataSection,
							unsigned long tlsSection);

/*
 * Write java image header
 */
void _ILWriteJavaHeaders(ILWriter *writer);

/*
 * Write a java class
 */
void WriteJavaClass(ILWriter *writer, ILClass *class);

#endif /* IL_USE_WRITER */

/*
 * Recommended number of token columns for use with "ILImageRawTokenData".
 */
#define	IL_IMAGE_TOKEN_COLUMNS		16

/*
 * Offsets into the array returned by "ILImageRawTokenData"
 * for various kinds of tokens.
 */
#define	IL_OFFSET_MODULE_GENERATION			0
#define	IL_OFFSET_MODULE_NAME				1
#define	IL_OFFSET_MODULE_MVID				2
#define	IL_OFFSET_MODULE_ENCID				3
#define	IL_OFFSET_MODULE_ENCBASEID			4

#define	IL_OFFSET_TYPEREF_SCOPE				0
#define	IL_OFFSET_TYPEREF_NAME				1
#define	IL_OFFSET_TYPEREF_NAMESPACE			2

#define	IL_OFFSET_TYPEDEF_ATTRS				0
#define	IL_OFFSET_TYPEDEF_NAME				1
#define	IL_OFFSET_TYPEDEF_NAMESPACE			2
#define	IL_OFFSET_TYPEDEF_PARENT			3
#define	IL_OFFSET_TYPEDEF_FIRST_FIELD		4
#define	IL_OFFSET_TYPEDEF_FIRST_METHOD		5

#define	IL_OFFSET_FIELDPTR_REFERENCE		0

#define	IL_OFFSET_FIELDDEF_ATTRS			0
#define	IL_OFFSET_FIELDDEF_NAME				1
#define	IL_OFFSET_FIELDDEF_SIGNATURE_RAW	2
#define	IL_OFFSET_FIELDDEF_SIGNATURE		3
#define	IL_OFFSET_FIELDDEF_SIGNATURE_LEN	4

#define	IL_OFFSET_METHODPTR_REFERENCE		0

#define	IL_OFFSET_METHODDEF_RVA				0
#define	IL_OFFSET_METHODDEF_IMPL_ATTRS		1
#define	IL_OFFSET_METHODDEF_ATTRS			2
#define	IL_OFFSET_METHODDEF_NAME			3
#define	IL_OFFSET_METHODDEF_SIGNATURE_RAW	4
#define	IL_OFFSET_METHODDEF_SIGNATURE		5
#define	IL_OFFSET_METHODDEF_SIGNATURE_LEN	6
#define	IL_OFFSET_METHODDEF_FIRST_PARAM		7

#define	IL_OFFSET_PARAMPTR_REFERENCE		0

#define	IL_OFFSET_PARAMDEF_ATTRS			0
#define	IL_OFFSET_PARAMDEF_NUMBER			1
#define	IL_OFFSET_PARAMDEF_NAME				2

#define	IL_OFFSET_INTERFACE_TYPE			0
#define	IL_OFFSET_INTERFACE_INTERFACE		1

#define	IL_OFFSET_MEMBERREF_PARENT			0
#define	IL_OFFSET_MEMBERREF_NAME			1
#define	IL_OFFSET_MEMBERREF_SIGNATURE_RAW	2
#define	IL_OFFSET_MEMBERREF_SIGNATURE		3
#define	IL_OFFSET_MEMBERREF_SIGNATURE_LEN	4

#define	IL_OFFSET_CONSTANT_TYPE				0
#define	IL_OFFSET_CONSTANT_REFERENCE		1
#define	IL_OFFSET_CONSTANT_DATA_RAW			2
#define	IL_OFFSET_CONSTANT_DATA				3
#define	IL_OFFSET_CONSTANT_DATA_LEN			4

#define	IL_OFFSET_CUSTOMATTR_OWNER			0
#define	IL_OFFSET_CUSTOMATTR_NAME			1
#define	IL_OFFSET_CUSTOMATTR_DATA_RAW		2
#define	IL_OFFSET_CUSTOMATTR_DATA			3
#define	IL_OFFSET_CUSTOMATTR_DATA_LEN		4

#define	IL_OFFSET_FIELDMARSHAL_TOKEN		0
#define	IL_OFFSET_FIELDMARSHAL_TYPE_RAW		1
#define	IL_OFFSET_FIELDMARSHAL_TYPE			2
#define	IL_OFFSET_FIELDMARSHAL_TYPE_LEN		3

#define	IL_OFFSET_DECLSECURITY_TYPE			0
#define	IL_OFFSET_DECLSECURITY_TOKEN		1
#define	IL_OFFSET_DECLSECURITY_DATA_RAW		2
#define	IL_OFFSET_DECLSECURITY_DATA			3
#define	IL_OFFSET_DECLSECURITY_DATA_LEN		4

#define	IL_OFFSET_CLASSLAYOUT_PACKING		0
#define	IL_OFFSET_CLASSLAYOUT_SIZE			1
#define	IL_OFFSET_CLASSLAYOUT_TYPE			2

#define	IL_OFFSET_FIELDLAYOUT_OFFSET		0
#define	IL_OFFSET_FIELDLAYOUT_FIELD			1

#define	IL_OFFSET_SIGNATURE_VALUE_RAW		0
#define	IL_OFFSET_SIGNATURE_VALUE			1
#define	IL_OFFSET_SIGNATURE_VALUE_LEN		2

#define	IL_OFFSET_EVENTMAP_TYPE				0
#define	IL_OFFSET_EVENTMAP_EVENT			1

#define	IL_OFFSET_EVENTPTR_REFERENCE		0

#define	IL_OFFSET_EVENT_ATTRS				0
#define	IL_OFFSET_EVENT_NAME				1
#define	IL_OFFSET_EVENT_TYPE				2

#define	IL_OFFSET_PROPMAP_TYPE				0
#define	IL_OFFSET_PROPMAP_PROPERTY			1

#define	IL_OFFSET_PROPERTYPTR_REFERENCE		0

#define	IL_OFFSET_PROPERTY_ATTRS			0
#define	IL_OFFSET_PROPERTY_NAME				1
#define	IL_OFFSET_PROPERTY_SIGNATURE_RAW	2
#define	IL_OFFSET_PROPERTY_SIGNATURE		3
#define	IL_OFFSET_PROPERTY_SIGNATURE_LEN	4

#define	IL_OFFSET_METHODSEM_SEMANTICS		0
#define	IL_OFFSET_METHODSEM_METHOD			1
#define	IL_OFFSET_METHODSEM_OWNER			2

#define	IL_OFFSET_METHODIMPL_TYPE			0
#define	IL_OFFSET_METHODIMPL_METHOD_1		1
#define	IL_OFFSET_METHODIMPL_METHOD_2		2

#define	IL_OFFSET_MODULEREF_NAME			0

#define	IL_OFFSET_TYPESPEC_TYPE_RAW			0
#define	IL_OFFSET_TYPESPEC_TYPE				1
#define	IL_OFFSET_TYPESPEC_TYPE_LEN			2

#define	IL_OFFSET_IMPLMAP_ATTRS				0
#define	IL_OFFSET_IMPLMAP_METHOD			1
#define	IL_OFFSET_IMPLMAP_ALIAS				2
#define	IL_OFFSET_IMPLMAP_MODULE			3

#define	IL_OFFSET_FIELDRVA_RVA				0
#define	IL_OFFSET_FIELDRVA_FIELD			1

#define	IL_OFFSET_ENCLOG_ITEM_1				0
#define	IL_OFFSET_ENCLOG_ITEM_2				1

#define	IL_OFFSET_ENCASSOC_VALUE			0

#define	IL_OFFSET_ASSEMBLY_HASHALG			0
#define	IL_OFFSET_ASSEMBLY_VER_1			1
#define	IL_OFFSET_ASSEMBLY_VER_2			2
#define	IL_OFFSET_ASSEMBLY_VER_3			3
#define	IL_OFFSET_ASSEMBLY_VER_4			4
#define	IL_OFFSET_ASSEMBLY_ATTRS			5
#define	IL_OFFSET_ASSEMBLY_KEY_RAW			6
#define	IL_OFFSET_ASSEMBLY_KEY				7
#define	IL_OFFSET_ASSEMBLY_KEY_LEN			8
#define	IL_OFFSET_ASSEMBLY_NAME				9
#define	IL_OFFSET_ASSEMBLY_LOCALE			10

#define	IL_OFFSET_PROCESSORDEF_NUM			0

#define	IL_OFFSET_OSDEF_IDENTIFIER			0
#define	IL_OFFSET_OSDEF_MAJOR				1
#define	IL_OFFSET_OSDEF_MINOR				2

#define	IL_OFFSET_ASSEMBLYREF_VER_1			0
#define	IL_OFFSET_ASSEMBLYREF_VER_2			1
#define	IL_OFFSET_ASSEMBLYREF_VER_3			2
#define	IL_OFFSET_ASSEMBLYREF_VER_4			3
#define	IL_OFFSET_ASSEMBLYREF_ATTRS			4
#define	IL_OFFSET_ASSEMBLYREF_KEY_RAW		5
#define	IL_OFFSET_ASSEMBLYREF_KEY			6
#define	IL_OFFSET_ASSEMBLYREF_KEY_LEN		7
#define	IL_OFFSET_ASSEMBLYREF_NAME			8
#define	IL_OFFSET_ASSEMBLYREF_LOCALE		9
#define	IL_OFFSET_ASSEMBLYREF_HASH_RAW		10
#define	IL_OFFSET_ASSEMBLYREF_HASH			11
#define	IL_OFFSET_ASSEMBLYREF_HASH_LEN		12

#define	IL_OFFSET_PROCESSORREF_NUM			0
#define	IL_OFFSET_PROCESSORREF_ASSEMBLY		1

#define	IL_OFFSET_OSREF_IDENTIFIER			0
#define	IL_OFFSET_OSREF_MAJOR				1
#define	IL_OFFSET_OSREF_MINOR				2
#define	IL_OFFSET_OSREF_ASSEMBLY			3

#define	IL_OFFSET_FILE_ATTRS				0
#define	IL_OFFSET_FILE_NAME					1
#define	IL_OFFSET_FILE_HASH_RAW				2
#define	IL_OFFSET_FILE_HASH					3
#define	IL_OFFSET_FILE_HASH_LEN				4

#define	IL_OFFSET_EXPTYPE_ATTRS				0
#define	IL_OFFSET_EXPTYPE_CLASS				1
#define	IL_OFFSET_EXPTYPE_NAME				2
#define	IL_OFFSET_EXPTYPE_NAMESPACE			3
#define	IL_OFFSET_EXPTYPE_FILE				4

#define	IL_OFFSET_MANIFESTRES_OFFSET		0
#define	IL_OFFSET_MANIFESTRES_ATTRS			1
#define	IL_OFFSET_MANIFESTRES_NAME			2
#define	IL_OFFSET_MANIFESTRES_IMPL			3

#define	IL_OFFSET_EXELOC_NAME_1				1
#define	IL_OFFSET_EXELOC_NAME_2				2
#define	IL_OFFSET_EXELOC_NAME_3				3

#define	IL_OFFSET_NESTEDCLASS_CHILD			0
#define	IL_OFFSET_NESTEDCLASS_PARENT		1

#define	IL_OFFSET_GENERICPAR_NUMBER			0
#define	IL_OFFSET_GENERICPAR_FLAGS			1
#define	IL_OFFSET_GENERICPAR_OWNER			2
#define	IL_OFFSET_GENERICPAR_NAME			3
#define	IL_OFFSET_GENERICPAR_KIND			4
#define	IL_OFFSET_GENERICPAR_CONSTRAINT		5

#define	IL_OFFSET_METHODSPEC_METHOD			0
#define	IL_OFFSET_METHODSPEC_INST_RAW		1
#define	IL_OFFSET_METHODSPEC_INST			2
#define	IL_OFFSET_METHODSPEC_INST_LEN		3

#define	IL_OFFSET_GENERICCON_PARAM			0
#define	IL_OFFSET_GENERICCON_CONSTRAINT		1

/*
 * Get the raw data associated with a particular token.
 * Returns zero if the token does not exist.  The "values"
 * array must have at least IL_IMAGE_TOKEN_COLUMNS elements.
 */
int _ILImageRawTokenData(ILImage *image, ILToken token,
						 ILUInt32 *values);

/*
 * Encode the raw data associated with a particular token
 * into a buffer prior to writing to an output image.
 */
void _ILImageRawTokenEncode(ILImage *image, unsigned char *ptr,
							ILToken token, ILUInt32 *values);

/*
 * Define this to 1 to enable debugging.
 */
#ifndef	REDUCED_STDIO
#define	IL_DEBUG_META		1
#else
#define	IL_DEBUG_META		0
#endif

/*
 * Debugging support.
 */
#if	IL_DEBUG_META
#define	META_ERROR(msg)		\
			do { \
				fprintf(stderr, "metadata error: %s\n", (msg)); \
			} while (0)
#define	META_VAL_ERROR(msg)		\
			do { \
				fprintf(stderr, "metadata error in token 0x%08lX: %s\n", \
						(unsigned long)token, (msg)); \
			} while (0)
#define	META_INDEX_ERROR(msg)		\
			do { \
				fprintf(stderr, "metadata error in token 0x%08lX: " \
								"bad %s index\n", \
						(unsigned long)token, (msg)); \
			} while (0)
#else
#define	META_ERROR(msg)			do { } while (0)
#define	META_VAL_ERROR(msg)		do { } while (0)
#define	META_INDEX_ERROR(msg)	do { } while (0)
#endif

#ifdef	__cplusplus
};
#endif

#endif	/* _IMAGE_H */
