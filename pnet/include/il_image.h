/*
 * il_image.h - Routines for manipulating IL executable images.
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

#ifndef	_IL_IMAGE_H
#define	_IL_IMAGE_H

#include "il_meta.h"
#include <stdio.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Load errors that may be returned by "ILImageLoad".
 */
#define	IL_LOADERR_TRUNCATED	1	/* File is truncated */
#define	IL_LOADERR_NOT_PE		2	/* Not a valid PE/COFF executable */
#define	IL_LOADERR_NOT_IL		3	/* Not a valid IL binary */
#define	IL_LOADERR_VERSION		4	/* Wrong IL version */
#define	IL_LOADERR_32BIT_ONLY	5	/* Program requires a 32-bit runtime */
#define	IL_LOADERR_BACKWARDS	6	/* Required sections in reverse order */
#define	IL_LOADERR_MEMORY		7	/* Not enough memory to load the image */
#define	IL_LOADERR_BAD_ADDR		8	/* Bad addresses in the image */
#define	IL_LOADERR_BAD_META		9	/* Something wrong with the metadata */
#define	IL_LOADERR_UNDOC_META	10	/* Uses undocumented metadata feature */
#define	IL_LOADERR_UNRESOLVED	11	/* Unresolved items in metadata */
#define	IL_LOADERR_ARCHIVE		12	/* File is an "ar" archive */
#define	IL_LOADERR_MAX			12	/* Maximum error value */

/*
 * Flags that may be supplied to "ILImageLoad".
 */
#define	IL_LOADFLAG_FORCE_32BIT		1	/* Force 32-bit only images to load */
#define	IL_LOADFLAG_NO_METADATA		2	/* Don't parse the metadata */
#define	IL_LOADFLAG_PRE_VALIDATE	4	/* Pre-validate the token metadata */
#define	IL_LOADFLAG_NO_RESOLVE		8	/* Don't resolve external references */
#define	IL_LOADFLAG_INSECURE		16	/* Loaded from an insecure source */
#define	IL_LOADFLAG_NO_MAP			32	/* Don't use mmap to load image */
#define	IL_LOADFLAG_IN_PLACE		64	/* Memory load: execute in place */
#define	IL_LOADFLAG_IGNORE_ERRORS	128	/* Ignore load errors (use wiseley) */

/*
 * Image types.
 */
#define	IL_IMAGETYPE_DLL		0	/* PE/COFF dynamic link library */
#define	IL_IMAGETYPE_EXE		1	/* PE/COFF executable */
#define	IL_IMAGETYPE_OBJ		2	/* PE/COFF object file */
#define	IL_IMAGETYPE_BUILDING	3	/* Building an in-memory image */
#define	IL_IMAGETYPE_JAVA		4	/* Java .class or .jar file */

/*
 * Section identifiers for "ILImageGetSection".
 */
#define	IL_SECTION_HEADER		0	/* Raw IL runtime header */
#define	IL_SECTION_CODE			1	/* Code section */
#define	IL_SECTION_METADATA		2	/* Metadata section */
#define	IL_SECTION_RESOURCES	3	/* Resources section */
#define	IL_SECTION_STRONG_NAMES	4	/* StrongNameSignature section */
#define	IL_SECTION_CODE_MANAGER	5	/* Code Manager Table section */
#define	IL_SECTION_DEBUG		6	/* Debug section */
#define	IL_SECTION_DATA			7	/* Data section */
#define	IL_SECTION_TLS			8	/* TLS data section */
#define	IL_SECTION_WINRES		9	/* Windows resource section */

/*
 * Opaque data structure for a program context, that contains
 * all of the loaded images.
 */
typedef struct _tagILContext ILContext;

/*
 * Opaque data structure for a loaded executable image.
 */
typedef struct _tagILImage ILImage;

/*
 * Create an IL context into which multiple images can be loaded.
 * Returns NULL if out of memory.
 */
ILContext *ILContextCreate(void);

/*
 * Destroy an IL context, and any remaining images associated with it.
 */
void ILContextDestroy(ILContext *context);

/*
 * Get an image from a context that has a particular module name.
 * Returns NULL if no such image.
 */
ILImage *ILContextGetModule(ILContext *context, const char *name);

/*
 * Get an image from a context that has a particular assembly name.
 * Returns NULL if no such image.
 */
ILImage *ILContextGetAssembly(ILContext *context, const char *name);

/*
 * Get an image from a context that has a particular file name.
 * Returns NULL if no such image.
 */
ILImage *ILContextGetFile(ILContext *context, const char *name);

/*
 * Iterate through the images that are associated with a context.
 */
ILImage *ILContextNextImage(ILContext *context, ILImage *image);

/*
 * Get the image to use for creating synthetic types.  If such
 * an image does not yet exist, then create it.  Returns NULL
 * if out of memory.
 */
ILImage *ILContextGetSynthetic(ILContext *context);

/*
 * Set the image to use as the system library, for resolving
 * standard types.  Once this image has been set, all system
 * type resolutions will use this image.  If it is not set,
 * then system types will be resolved in any image.  Runtime
 * engines must set this image to prevent applications from
 * substituting their own system types and thereby circumventing
 * the system's security.
 */
void ILContextSetSystem(ILContext *context, ILImage *image);

/*
 * Get the image that is being used as the system library.
 * Returns NULL if no image has been set yet.
 */
ILImage *ILContextGetSystem(ILContext *context);

/*
 * Set the base directory of the application.
 * The old applicationBaseDir will be freed.
 */
 void ILContextSetApplicationBaseDir(ILContext *context, char *applicationBaseDir);

/*
 * Get the base directory of the application.
 * The caller has to make sure that this string exists for the
 * time it is used. It might be destroyed if the Set function is
 * called by an other thread.
 */
const char *ILContextGetApplicationBaseDir(ILContext *context);

/*
 * Set the directory where the shadow copies will be copied to.
 * The old cacheDir will be freed.
 */
void ILContextSetCacheDir(ILContext *context, char *cacheDir);

/*
 * Get the directory where the shadow copies will be copied to.
 * The caller has to make sure that this string exists for the
 * time it is used. It might be destroyed if the Set function is
 * called by an other thread.
 */
const char *ILContextGetCacheDir(ILContext *context);

/*
 * Set the directory where dynamically created files are stored and
 * accessed..
 * The old dynamicBaseDir will be freed.
 */
void ILContextSetDynamicBaseDir(ILContext *context, char *dynamicBaseDir);

/*
 * Get the directory where dynamically created files are stored and
 * accessed..
 * The caller has to make sure that this string exists for the
 * time it is used. It might be destroyed if the Set function is
 * called by an other thread.
 */
const char *ILContextGetDynamicBaseDir(ILContext *context);

/*
 * Set the list of directories to be used for library path
 * searching, before inspecting the standard directories.
 * The list and the paths must be allocated by ILMalloc. 
 * The old list and the containing paths will be freed when
 * this function is called again.
 */
void ILContextSetLibraryDirs(ILContext *context,
							 char **libraryDirs,
							 int numLibraryDirs);

/*
 * Get the list of directories to be used for library path
 * searching, before inspecting the standard directories.
 * The caller has to make sure that the list and the paths
 * will not be freed by an other thread calling the Set or
 * Clear function.
 */
void ILContextGetLibraryDirs(ILContext *context,
							 char ***libraryDirs,
							 int *numLibraryDirs);

/*
 * Clear the list of directories to be used for library path
 * searching.
 * The old list and the containing paths will be freed.
 */
void ILContextClearLibraryDirs(ILContext *context);

/*
 * Get the directory relative to the applicationBaseDir where
 * should be searched for Assemblies.
 * The caller has to make sure that this string exists for the
 * time it is used. It might be destroyed if the Set function is
 * called by an other thread.
 */
const char *ILContextGetRelativeSearchDir(ILContext *context);

/*
 * Set the directory relative to the applicationBaseDir where
 * should be searched for Assemblies.
 * The old relativeSearchDir will be freed.
 */
void ILContextSetRelativeSearchDir(ILContext *context, char *relativeSearchDir);

/*
 * Set the list of directories that have to be cached in the
 * directory specified in cacheDir.
 * The list and the paths must be allocated by ILMalloc. 
 * The old list and the containing paths will be freed when
 * this function is called again.
 */
void ILContextSetShadowCopyDirs(ILContext *context,
								 char **shadowCopyDirs,
								 int numShadowCopyDirs);

/*
 * Get the list of directories that have to be cached in the
 * directory specified in cacheDir.
 * The caller has to make sure that the list and the paths
 * will not be freed by an other thread calling the Set or
 * Clear function.
 */
void ILContextGetShadowCopyDirs(ILContext *context,
							 	char ***shadowCopyDirs,
							 	int *numShadowCopyDirs);

/*
 * Crear the list of directories that have to be cached.
 * The old list and the containing paths will be freed.
 */
void ILContextClearShadowCopyDirs(ILContext *context);

/*
 * Set shadowCopyFiles to a 0 to disable shadow copies or a value != 0
 * to enable shadow copies.
 */
void ILContextSetShadowCopyFiles(ILContext *context, int shadowCopyFiles);

/*
 * Get the shadowCopyFiles setting of the context
 */
int ILContextGetShadowCopyFiles(ILContext *context);

/*
 * Used by the engine to attach user data to the context instance.
 */
void ILContextSetUserData(ILContext *context, void *userData);

/*
 * Used by the engine to get attached user data from the context instance.
 */
void *ILContextGetUserData(ILContext *context);

/*
 * Create an IL image.  This is typically used by compilers
 * when building an image in-memory in preparation for writing
 * it to an object file or executable.  Loaders should use
 * "ILImageLoad" instead.  Returns NULL if out of memory.
 */
ILImage *ILImageCreate(ILContext *context);

#ifndef REDUCED_STDIO

/*
 * Load an IL image into memory.  The specified "file" can
 * be a non-seekable stream.  The file is assumed to be positioned
 * at the beginning of the stream.  Returns 0 if OK (with an
 * image descriptor in "*image"), or an error code otherwise.
 * If "filename" is non-NULL, it is used to locate the directory
 * from which the image is loaded for linking purposes.
 */
int ILImageLoad(FILE *file, const char *filename, ILContext *context,
				ILImage **image, int flags);

#endif

/*
 * Load an IL image from a file.  Returns -1 if the file
 * could not be opened (reason in "errno"), or a load
 * error otherwise.  If the filename is "-", then the
 * input stream is read from stdin.  If "printErrors" is
 * non-zero, then this function will report load errors
 * to stderr, so that the caller doesn't have to.
 */
int ILImageLoadFromFile(const char *filename, ILContext *context,
						ILImage **image, int flags, int printErrors);

/*
 * Load an IL image from a memory buffer.  Returns a load error.
 * IL_LOADFLAG_IN_PLACE can be used to execute directly from the
 * supplied buffer, without making a copy first.  The caller is
 * responsible for ensuring that the buffer persists for the
 * lifetime of the image.  "filename" is optional: it is used
 * for error reporting and to indicate the load directory.
 */
int ILImageLoadFromMemory(const void *buffer, unsigned long bufLen,
						  ILContext *context, ILImage **image,
						  int flags, const char *filename);

/*
 * Load an assembly by name into an existing context, relative
 * to a particular parent image.  Returns a load error, 0 if OK,
 * or -1 if the assembly file was not found.
 */
int ILImageLoadAssembly(const char *name, ILContext *context,
						ILImage *parentImage, ILImage **image);

/*
 * Destroy an IL image and all memory associated with it.
 */
void ILImageDestroy(ILImage *image);

/*
 * Get the filename from which an image was loaded.
 */
const char *ILImageGetFileName(ILImage *image);

/*
 * Get the context associated with an IL image.
 */
ILContext *ILImageToContext(ILImage *image);

/*
 * Get a loaded image's type.
 */
int ILImageType(ILImage *image);

/*
 * Determine if an image was loaded from a secure source.
 */
int ILImageIsSecure(ILImage *image);

/*
 * Determine if the loaded image had non-stub native code
 * associated with it, which was stripped during loading.
 */
int ILImageHadNative(ILImage *image);

/*
 * Determine if the loaded image was 32-bit only, but the image
 * was forced to load anyway.
 */
int ILImageIs32Bit(ILImage *image);

/*
 * Get the length of IL image.
 */
unsigned long ILImageLength(ILImage *image);

/*
 * Map a virtual address to a real address within a loaded image.
 * Returns NULL if the virtual address is invalid for the image.
 */
void *ILImageMapAddress(ILImage *image, unsigned long address);

/*
 * Map a method or data RVA to an address and a maximum
 * length.  The method or data begins at the return value
 * and can occupy up to "*len" bytes within the image.
 * Returns NULL if the RVA is invalid.  This function works
 * even if "ILImageGetSection" cannot find the code section.
 */
void *ILImageMapRVA(ILImage *image, unsigned long rva, unsigned long *len);

/*
 * Get the real file seek offset that corresponds to a virtual
 * address within a loaded image.  Returns 0 if the virtual
 * address is invalid for the image.
 */
unsigned long ILImageRealOffset(ILImage *image, unsigned long address);

/*
 * Get the virtual address of a particular section.
 */
unsigned long ILImageGetSectionAddr(ILImage *image, int section);

/*
 * Get the virtual size of a particular section.
 */
ILUInt32 ILImageGetSectionSize(ILImage *image, int section);

/*
 * Get the address and size of a particular image section.
 * Returns zero if the section was not found.
 */
int ILImageGetSection(ILImage *image, int section,
					  void **address, ILUInt32 *size);

/*
 * Get the entry point token for the image.  Returns zero if
 * there is no entry point token.
 */
ILToken ILImageGetEntryPoint(ILImage *image);

/*
 * Get a specific entry from the directory that is stored
 * in the metadata section of an IL image.  Returns the address
 * of the entry, or NULL if the entry is not present.  The size
 * of the entry is returned in "*size" if the entry is found.
 */
void *ILImageGetMetaEntry(ILImage *image, const char *name,
						  ILUInt32 *size);

/*
 * Get the number of entries in the directory that is stored
 * in the metadata section of an IL image.
 */
ILUInt32 ILImageNumMetaEntries(ILImage *image);

/*
 * Information about a numbered entry in the directory that is
 * stored in the metadata section of an IL image.  The address of
 * the data is returned, or NULL if the entry number is invalid.
 */
void *ILImageMetaEntryInfo(ILImage *image, unsigned long entry,
						   char **name, unsigned long *virtAddr,
						   ILUInt32 *size);

/*
 * Get the size in bytes of the directory header that is stored in
 * the metadata section of an IL image.
 */
unsigned long ILImageMetaHeaderSize(ILImage *image);

/*
 * Get the runtime version that was written to the metadata
 * header by the compiler.
 */
const char *ILImageMetaRuntimeVersion(ILImage *image, int *length);

/*
 * Get a string from the string pool.  Returns NULL if "offset" is invalid.
 * The return pointer is guaranteed to be fixed for the lifetime of the image.
 */
const char *ILImageGetString(ILImage *image, ILUInt32 offset);

/*
 * Add a string to the string pool.  This can only be used if
 * the image is being built.  Returns the offset, or zero if
 * the string is empty or we are out of memory.
 */
ILUInt32 ILImageAddString(ILImage *image, const char *str);

/*
 * Get a blob from the blob pool.  Returns NULL if "offset" is invalid.
 * The return pointer is guaranteed to be fixed for the lifetime of the image.
 * The length of the blob is returned in "*len".
 */
const void *ILImageGetBlob(ILImage *image, ILUInt32 offset,
						   ILUInt32 *len);

/*
 * Add a blob to the blob pool.  This can only be used if
 * the image is being built.  Returns the offset, or zero if
 * we are out of memory.
 */
ILUInt32 ILImageAddBlob(ILImage *image, const void *blob,
							 ILUInt32 len);

/*
 * Get a Unicode string from the user string pool.  Returns NULL if
 * "offset" is invalid.  The return pointer is guaranteed to be
 * fixed for the lifetime of the image.  The number of characters in
 * the string is returned in "*len".  The characters themselves are
 * stored in little-endian order beginning at the return pointer.
 */
const char *ILImageGetUserString(ILImage *image, ILUInt32 offset,
								 ILUInt32 *len);

/*
 * Add a UTF-8 string to the user string pool.  This can only be
 * used if the image is being built.  Returns the offset, or zero
 * if we are out of memory.
 */
ILUInt32 ILImageAddUserString(ILImage *image, const char *str, int len);

/*
 * Add a string to the user string pool that is already encoded
 * as little-endian character values.  The string is assumed to
 * be "len" Unicode characters in length, and be followed by a
 * '\0' byte. Returns the offset, or zero if we are out of memory.
 */
ILUInt32 ILImageAddEncodedUserString(ILImage *image,
										  const void *str, int len);

/*
 * Get the module name from an image.  Returns NULL if no such name.
 * If there are multiple module names, this returns the first.
 */
const char *ILImageGetModuleName(ILImage *image);

/*
 * Get the assembly name from an image.  Returns NULL if no such name.
 * If there are multiple assembly names, this returns the first.
 */
const char *ILImageGetAssemblyName(ILImage *image);

/*
 * Get the number of tokens of a particular type.
 */
unsigned long ILImageNumTokens(ILImage *image, ILToken tokenType);

/*
 * Get the information block associated with a token.
 * Returns the block, or NULL if the token is invalid.
 */
void *ILImageTokenInfo(ILImage *image, ILToken token);

/*
 * Iterate through the tokens in a specific token table.
 */
void *ILImageNextToken(ILImage *image, ILToken tokenType, void *prev);

/*
 * Search through the tokens in a specific token table for a match.
 * Returns the information block, or NULL if no match.
 */
typedef int (*ILImageCompareFunc)(void *item, void *userData);
void *ILImageSearchForToken(ILImage *image, ILToken tokenType,
							ILImageCompareFunc compareFunc,
							void *userData);

/*
 * Get an error message string for an "IL_LOADERR_*" value.
 * Returns a default message if the value is invalid.
 */
const char *ILImageLoadError(int error);

/*
 * Search along the standard library path for an assembly with
 * a specific name and version.  If "version" is NULL or all-zeroes,
 * then use any version.  If "numBeforePaths" is non-zero, then
 * "beforePaths" contains a list of paths to be searched before
 * the standard paths.  If "numAfterPaths" is non-zero, then
 * "afterPaths" contains a list of paths to be searched after
 * the standard paths.  If "suppressStandardPaths" is non-zero,
 * then the standard paths will be omitted.  Returns NULL if
 * the assembly could not be resolved, or an ILMalloc'ed string
 * containing the full pathname otherwise.
 */
char *ILImageSearchPath(const char *name, const ILUInt16 *version,
						const char *parentAssemblyPath,
						const char **beforePaths, unsigned long numBeforePaths,
						const char **afterPaths, unsigned long numAfterPaths,
						int suppressStandardPaths, int *sameDir);

/*
 * Information about the resource section.
 */
typedef struct _tagILResourceSection ILResourceSection;

/*
 * Create a resource section handler for an image.
 * Returns NULL if out of memory.
 */
ILResourceSection *ILResourceSectionCreate(ILImage *image);

/*
 * Destroy a resource section handler.
 */
void ILResourceSectionDestroy(ILResourceSection *section);

/*
 * Get a named entry from a resource section.  Returns NULL if not found.
 */
void *ILResourceSectionGetEntry(ILResourceSection *section, const char *name,
								unsigned long *length);

/*
 * Get the first leaf entry under a specific resource directory in a
 * resource section.  Returns NULL if not found.
 */
void *ILResourceSectionGetFirstEntry(ILResourceSection *section,
									 const char *name, unsigned long *length);

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_IMAGE_H */
