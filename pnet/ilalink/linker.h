/*
 * linker.h - Internal definitions for image linking.
 *
 * Copyright (C) 2001, 2003, 2008  Southern Storm Software, Pty Ltd.
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

#ifndef	_ILALINK_LINKER_H
#define	_ILALINK_LINKER_H

#include "il_linker.h"
#include "il_program.h"
#include "il_writer.h"
#include "il_program.h"
#include "il_system.h"
#include "il_utils.h"
#include "il_dumpasm.h"
#include "il_debug.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Information about a class in a library assembly.
 */
typedef struct _tagILLibraryClass ILLibraryClass;
struct _tagILLibraryClass
{
	const char	   *name;			/* Intern'ed name of the class */
	const char	   *namespace;		/* Intern'ed namespace of the class */
	ILLibraryClass *parent;			/* Parent for nesting purposes */

};

/*
 * Information about a global symbol in a library assembly.
 */
typedef struct _tagILLibrarySymbol ILLibrarySymbol;
struct _tagILLibrarySymbol
{
	const char	   *name;			/* Intern'ed name of the symbol */
	const char	   *aliasFor;		/* Intern'ed name of the aliased symbol */
	int				flags;			/* Flags that define the symbol kind */
	ILMember       *member;			/* Member reference information */
};

/*
 * Global symbol flags.
 */
#define	IL_LINKSYM_FUNCTION		(1<<0)
#define	IL_LINKSYM_VARIABLE		(1<<1)
#define	IL_LINKSYM_WEAK			(1<<2)
#define	IL_LINKSYM_STRONG		(1<<3)
#define	IL_LINKSYM_HAVE_REF		(1<<4)
#define	IL_LINKSYM_SAW_UNDEF	(1<<5)

/*
 * Combined structure, for allocation within "ILLibrary::pool".
 */
typedef union
{
	ILLibraryClass	classInfo;
	ILLibrarySymbol	symbolInfo;

} ILLibraryClassOrSymbol;

/*
 * Information that is stored for a library assembly.
 */
typedef struct _tagILLibrary ILLibrary;
struct _tagILLibrary
{
	const char	   *name;			/* Name of the library's assembly */
	const char	   *filename;		/* Filename for the library's assembly */
	const char	   *moduleName;		/* Name of the library's main module */
	ILUInt16		version[4];		/* Version of the library's assembly */
	ILLibrary	   *altNames;		/* Alternative names for the library */
	unsigned char  *publicKey;		/* Public key value for the library */
	ILUInt32		publicKeyLen;	/* Length of the public key value */
	ILHashTable    *classHash;		/* Hash table for class name lookup */
	ILHashTable    *symbolHash;		/* Hash table for global symbol lookup */
	int				isCImage;		/* Non-zero if a C image */
	ILMemPool		pool;			/* Memory pool for symbol allocation */
	ILContext      *context;		/* Context containing the library image */
	ILImage        *image;			/* Image that corresponds to the library */
	ILLibrary	   *next;			/* Next library used by the linker */

};

/*
 * Information that is stored for an image to be linked.
 */
typedef struct _tagILLinkImage ILLinkImage;
struct _tagILLinkImage
{
	const char	   *filename;		/* Name of the image file */
	ILContext	   *context;		/* Context that contains the image */
	ILImage		   *image;			/* The image itself */
	ILLinkImage    *next;			/* Next image to be linked */
};

/*
 * Internal structure of the linker context.
 */
struct _tagILLinker
{
	ILContext	   *context;		/* Main context for the final image */
	ILImage        *image;			/* Final image that is being built */
	ILWriter	   *writer;			/* Writer being used by the linker */
	ILLibrary	   *libraries;		/* Libraries being used by the linker */
	ILLibrary      *lastLibrary;	/* Last library being used by the linker */
	char          **libraryDirs;	/* List of library directories */
	int				numLibraryDirs;	/* Number of library directories */
	int				outOfMemory;	/* Set to non-zero when out of memory */
	int				error;			/* Some other error occurred */
	int				is32Bit;		/* Non-zero if "-m32bit-only" supplied */
	int				linkerFlags;	/* Extra flags for the linker */
	int				hasDebug;		/* Non-zero if an image has debug info */
	ILLinkImage    *images;			/* List of images to be linked */
	ILLinkImage    *lastImage;		/* Last image on the "images" list */
	ILUInt32		imageNum;		/* Number of the image being linked */
	unsigned long	resourceRVA;	/* RVA of resource section start */
	ILMethod       *entryPoint;		/* Current entry point that is set */
	ILUInt32		dataLength;		/* Length of ".sdata" section */
	ILUInt32		tlsLength;		/* Length of ".tls" section */
	int				isCLink;		/* Non-zero if linking a C image */
	ILHashTable    *symbolHash;		/* Hash table for global symbol lookup */
	ILMemPool		pool;			/* Memory pool for symbol allocation */
	const char	   *moduleName;		/* Name of the "<Module>" class */
	ILClass        *moduleClass;	/* Reference to the "<Module>" class */
	const char	   *initTempFile;	/* Temporary object file for init/fini */

};

/*
 * Name of the "<Module>" class for executables and libraries.
 */
#define	IL_LINKER_EXE_MODULE_NAME	"<Module>"
#define	IL_LINKER_DLL_MODULE_NAME	"$Module$"

/*
 * Context that is used to find a class within the libraries.
 */
typedef struct
{
	ILLinker	   *linker;			/* Linker to use */
	ILLibrary	   *library;		/* Library to look within */
	ILLibraryClass *libClass;		/* Data pertaining to the class */
	ILLibraryClass *prevClass;		/* Previous class in find context */

} ILLibraryFind;

/*
 * Report that the linker is out of memory.
 */
void _ILLinkerOutOfMemory(ILLinker *linker);

/*
 * Destroy the libraries associated with a linker.
 */
void _ILLinkerDestroyLibraries(ILLinker *linker);

/*
 * Find a library given its assembly name.
 */
ILLibrary *_ILLinkerFindLibrary(ILLinker *linker, const char *name);

/*
 * Initialize a library find context.  If "library" is
 * NULL, then any library with the class will be used.
 */
void _ILLinkerFindInit(ILLibraryFind *find, ILLinker *linker,
					   ILLibrary *library);

/*
 * Locate a class within a library find context.
 */
int _ILLinkerFindClass(ILLibraryFind *find, const char *name,
					   const char *namespace);

/*
 * Find a class within one of the libraries and convert it
 * into a TypeRef in the current image.
 */
ILClass *_ILLinkerFindByName(ILLinker *linker, const char *name,
							 const char *namespace);

/*
 * Print the name of a class that could not be found.
 */
void _ILLinkerPrintClass(ILLibraryFind *find, const char *name,
						 const char *namespace);

/*
 * Make a TypeRef for a class that was found within "image".
 */
ILClass *_ILLinkerMakeTypeRef(ILLibraryFind *find, ILImage *image);

/*
 * Locate a global symbol definition within all libraries.
 * Returns the symbol flags, or zero if not found.  "*library"
 * will be NULL if the symbol is in the image being linked.
 */
int _ILLinkerFindSymbol(ILLinker *linker, const char *name,
						const char **aliasFor, ILLibrary **library,
						ILMember **memberRef);

/*
 * Update a global definition that was found.
 */
void _ILLinkerUpdateSymbol(ILLinker *linker, const char *name,
						   ILMember *member);

/*
 * Determine if we have a definition for a specific symbol.
 */
int _ILLinkerHasSymbol(ILLinker *linker, const char *name);

/*
 * Create the global symbol hash for the image being linked.
 */
int _ILLinkerCreateSymbolHash(ILLinker *linker);

/*
 * Add the global symbols in a link image to a linker context.
 */
void _ILLinkerAddSymbols(ILLinker *linker, ILImage *image);

/*
 * Convert a class reference in a foreign image into a
 * reference in the output image.
 */
ILClass *_ILLinkerConvertClassRef(ILLinker *linker, ILClass *classInfo);

/*
 * Convert a class ore typespec reference in a foreign image to a 
 * class reference or typespec in the output image.
 */
ILProgramItem *_ILLinkerConvertProgramItemRef(ILLinker *linker,
											  ILProgramItem *item);

/*
 * Convert a member reference in a foreign image into a
 * reference in the output image.
 */
ILMember *_ILLinkerConvertMemberRef(ILLinker *linker, ILMember *member);

/*
 * Convert a type in a foreign image into a type in
 * the output image.
 */
ILType *_ILLinkerConvertType(ILLinker *linker, ILType *type);

/*
 * Convert a synthetic type reference in a foreign image into
 * a type specification in the output image.
 */
ILTypeSpec *_ILLinkerConvertTypeSpec(ILLinker *linker, ILType *type);

/*
 * Convert custom attributes from an old item in a link
 * image, and attach them to a new item in the final image.
 */
int _ILLinkerConvertAttrs(ILLinker *linker, ILProgramItem *oldItem,
						  ILProgramItem *newItem);

/*
 * Convert security declarations from an old item in a link
 * image, and attach them to a new item in the final image.
 */
int _ILLinkerConvertSecurity(ILLinker *linker, ILProgramItem *oldItem,
						     ILProgramItem *newItem);

/*
 * Convert debug information from an old item to a new item.
 */
int _ILLinkerConvertDebug(ILLinker *linker, ILProgramItem *oldItem,
						  ILProgramItem *newItem);

/*
 * Convert all classes from a link image.
 */
int _ILLinkerConvertClasses(ILLinker *linker, ILImage *image);

/*
 * Convert a method from a link image into a method underneath
 * a specified new class in the final image.
 */
int _ILLinkerConvertMethod(ILLinker *linker, ILMethod *method,
						   ILClass *newClass);

/*
 * Convert a field from a link image into a field underneath
 * a specified new class in the final image.
 */
int _ILLinkerConvertField(ILLinker *linker, ILField *field,
						  ILClass *newClass);

/*
 * Convert constant data from an old item in a link
 * image, and attach them to a new item in the final image.
 */
int _ILLinkerConvertConstant(ILLinker *linker, ILProgramItem *oldItem,
							 ILProgramItem *newItem);

/*
 * Convert field marshal and constant data from an old item in a link
 * image, and attach them to a new item in the final image.
 */
int _ILLinkerConvertMarshal(ILLinker *linker, ILProgramItem *oldItem,
						    ILProgramItem *newItem, int isParam);

/*
 * Convert a property from a link image into a property underneath
 * a specified new class in the final image.
 */
int _ILLinkerConvertProperty(ILLinker *linker, ILProperty *property,
						     ILClass *newClass);

/*
 * Convert an event from a link image into an event underneath
 * a specified new class in the final image.
 */
int _ILLinkerConvertEvent(ILLinker *linker, ILEvent *event,
						  ILClass *newClass);

/*
 * Get the name of the module class for the current image.
 */
const char *_ILLinkerModuleName(ILLinker *linker);

/*
 * Get the module class information record for the current image.
 */
ILClass *_ILLinkerModuleClass(ILLinker *linker);

/*
 * Determine if a class looks like a "<Module>" type.
 */
int _ILLinkerIsModule(ILClass *classInfo);

/*
 * Determine if a class is marked with "OpenSystem.C.GlobalScope".
 */
int _ILLinkerIsGlobalScope(ILClass *classInfo);

/*
 * Get a new name for a private class, that must be renamed
 * to prevent clashes with similar classes in other modules.
 */
char *_ILLinkerNewClassName(ILLinker *linker, ILClass *classInfo);

/*
 * Get a new name for a private member, that must be renamed
 * to prevent clashes with similar names in other modules.
 */
char *_ILLinkerNewMemberName(ILLinker *linker, ILMember *member);

/*
 * Find a library class that is identical to a specified local class.
 * This allows us to favour the library forms of types like FILE instead
 * of picking up the redefined version in the compiled program.
 */
int _ILLinkerLibraryReplacement(ILLinker *linker, ILLibraryFind *find,
								ILClass *classInfo);

/*
 * Create the global initializer and finalizer methods for C applications.
 * Returns a new image to be linked into the application, or NULL if
 * the application does not need initializers or finalizers.
 */
ILImage *_ILLinkerCreateInitFini(ILLinker *linker);

/*
 * Create an attribute on a particular program item.
 */
void _ILLinkerCreateAttribute(ILLinker *linker, ILProgramItem *item,
							  const char *name, const char *namespace,
							  ILType *arg1Type, ILType *arg2Type,
							  void *data, int len);

#if IL_VERSION_MAJOR > 1
/*
 * Copy the generic parameters for one program item to an other one.
 */
int _ILLinkerConvertGenerics(ILLinker *linker, ILProgramItem *oldItem,
						     ILProgramItem *newItem);
#endif /* IL_VERSION_MAJOR > 1 */

#ifdef	__cplusplus
};
#endif

#endif	/* _ILALINK_LINKER_H */
