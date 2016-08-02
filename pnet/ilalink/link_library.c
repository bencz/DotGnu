/*
 * link_library.c - Process libraries within a linker context.
 *
 * Copyright (C) 2001, 2003, 2004, 2008, 2009  Southern Storm Software, Pty Ltd.
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
#include "il_crypt.h"
#ifdef HAVE_SYS_STAT_H
#include <sys/stat.h>
#endif
#ifdef HAVE_UNISTD_H
#include <unistd.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Determine if a directory exists.
 */
static int DirExists(const char *pathname)
{
#ifdef HAVE_STAT
	struct stat st;
	return (stat(pathname, &st) >= 0);
#else
#ifdef HAVE_ACCESS
	return (access(pathname, 0) >= 0);
#else
	return 1;
#endif
#endif
}

int ILLinkerAddLibraryDir(ILLinker *linker, const char *pathname)
{
	int index;
	char *dupStr;
	char **newDirs;

	/* Bail out if the directory doesn't exist, because there is
	   no point adding it to the list if it won't contain anything */
	if(!DirExists(pathname))
	{
		return 1;
	}

	/* Determine if the directory is already present.  There is
	   no point searching the same directory twice */
	for(index = 0; index < linker->numLibraryDirs; ++index)
	{
		if(!strcmp(linker->libraryDirs[index], pathname))
		{
			return 1;
		}
	}

	/* Add the directory to the list */
	dupStr = ILDupString(pathname);
	if(!dupStr)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}
	newDirs = (char **)ILRealloc(linker->libraryDirs,
								 sizeof(char *) * (linker->numLibraryDirs + 1));
	if(!newDirs)
	{
		ILFree(dupStr);
		_ILLinkerOutOfMemory(linker);
		return 0;
	}
	linker->libraryDirs = newDirs;
	newDirs[(linker->numLibraryDirs)++] = dupStr;
	return 1;
}

char *ILLinkerResolveLibrary(ILLinker *linker, const char *name)
{
	int len;
	char *newName;
	char *expanded;

	/* Does the filename contain a path specification? */
	len = strlen(name);
	while(len > 0 && name[len - 1] != '/' && name[len - 1] != '\\')
	{
		--len;
	}
	if(len > 0)
	{
		/* Yes it does, so check for direct file existence */
		if(!ILFileExists(name, (char **)0))
		{
			return 0;
		}
		newName = ILDupString(name);
		if(!newName)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		return newName;
	}

	/* Search the library directories for the name */
	newName = ILImageSearchPath
		(name, 0, 0,
		 (const char **)(linker->libraryDirs), linker->numLibraryDirs,
		 0, 0, 0, 0);
	if(newName)
	{
		return newName;
	}

	/* Prepend "lib".  This allows us to convert references
	   such as "-lm" into "-llibm" */
	if(strncmp(name, "lib", 3) == 0)
	{
		return 0;
	}
	len = strlen(name);
	expanded = (char *)ILMalloc(len + 4);
	if(!expanded)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}
	strcpy(expanded, "lib");
	strcpy(expanded + 3, name);

	/* Search the library directories for the expanded name */
	newName = ILImageSearchPath
		(expanded, 0, 0,
		 (const char **)(linker->libraryDirs), linker->numLibraryDirs,
		 0, 0, 0, 0);
	ILFree(expanded);
	return newName;
}

/*
 * Destroy a library context.
 */
static void LibraryDestroy(ILLibrary *library)
{
	ILLibrary *next;
	int isFirst = 1;
	while(library != 0)
	{
		next = library->altNames;
		ILFree((void *)(library->name));
		ILFree((void *)(library->filename));
		ILFree((void *)(library->moduleName));
		if(library->publicKey)
		{
			ILFree(library->publicKey);
		}
		ILHashDestroy(library->classHash);
		ILHashDestroy(library->symbolHash);
		ILMemPoolDestroy(&(library->pool));
		if(isFirst)
		{
			ILContextDestroy(library->context);
			isFirst = 0;
		}
		ILFree(library);
		library = next;
	}
}

/*
 * Key that is used to index the class hash table.
 */
typedef struct
{
	const char *name;
	const char *namespace;
	ILLibraryClass *parent;

} ClassHashKey;

/*
 * Compute the class hash value for an element.
 */
static unsigned long ClassHash_Compute(const ILLibraryClass *libClass)
{
	return ILHashString(0, libClass->name, strlen(libClass->name));
}

/*
 * Compute the class hash value for a key.
 */
static unsigned long ClassHash_KeyCompute(const ClassHashKey *key)
{
	return ILHashString(0, key->name, strlen(key->name));
}

/*
 * Match a class key against an element.
 */
static int ClassHash_Match(const ILLibraryClass *libClass,
						   const ClassHashKey *key)
{
	if(strcmp(libClass->name, key->name) != 0)
	{
		return 0;
	}
	if(libClass->namespace)
	{
		if(key->namespace)
		{
			if(strcmp(libClass->namespace, key->namespace) != 0)
			{
				return 0;
			}
		}
		else
		{
			return 0;
		}
	}
	else
	{
		if(key->namespace != 0)
		{
			return 0;
		}
	}
	return (key->parent == libClass->parent);
}

/*
 * Compute the symbol hash value for an element.
 */
static unsigned long SymbolHash_Compute(const ILLibrarySymbol *libSymbol)
{
	return ILHashString(0, libSymbol->name, strlen(libSymbol->name));
}

/*
 * Compute the symbol hash value for a key.
 */
static unsigned long SymbolHash_KeyCompute(const char *key)
{
	return ILHashString(0, key, strlen(key));
}

/*
 * Match a symbol key against an element.
 */
static int SymbolHash_Match(const ILLibrarySymbol *libSymbol,
						    const char *key)
{
	return (strcmp(libSymbol->name, key) == 0);
}

/*
 * Create ILLibrary records for all assembly definitions
 * within a supplied library image.  Returns the list,
 * or NULL if an error occurred.
 */
static ILLibrary *ScanAssemblies(ILLinker *linker, ILContext *context,
								 ILImage *image, const char *filename)
{
	ILLibrary *library;
	ILLibrary *lastLibrary;
	ILLibrary *nextLibrary;
	ILAssembly *assem;
	const void *originator;
	ILUInt32 originatorLen;

	/* Scan the assembly definitions */
	library = 0;
	lastLibrary = 0;
	assem = 0;
	while((assem = (ILAssembly *)ILImageNextToken
				(image, IL_META_TOKEN_ASSEMBLY, (void *)assem)) != 0)
	{
		nextLibrary = (ILLibrary *)ILMalloc(sizeof(ILLibrary));
		if(!nextLibrary)
		{
			_ILLinkerOutOfMemory(linker);
			LibraryDestroy(library);
			return 0;
		}
		nextLibrary->publicKey = 0;
		nextLibrary->publicKeyLen = 0;
		nextLibrary->name = ILDupString(ILAssembly_Name(assem));
		if(!(nextLibrary->name))
		{
			ILFree(nextLibrary);
			_ILLinkerOutOfMemory(linker);
			LibraryDestroy(library);
			return 0;
		}
		nextLibrary->filename = ILDupString(filename);
		if(!(nextLibrary->filename))
		{
			ILFree((void *)(nextLibrary->name));
			ILFree(nextLibrary);
			_ILLinkerOutOfMemory(linker);
			LibraryDestroy(library);
			return 0;
		}
		nextLibrary->moduleName = ILDupString(IL_LINKER_DLL_MODULE_NAME);
		if(!(nextLibrary->moduleName))
		{
			ILFree((void *)(nextLibrary->filename));
			ILFree((void *)(nextLibrary->name));
			ILFree(nextLibrary);
			_ILLinkerOutOfMemory(linker);
			LibraryDestroy(library);
			return 0;
		}
		ILMemCpy(nextLibrary->version, ILAssemblyGetVersion(assem),
				 4 * sizeof(ILUInt16));
		nextLibrary->altNames = 0;
		if(library)
		{
			nextLibrary->classHash = 0;
			nextLibrary->symbolHash = 0;
			nextLibrary->isCImage = library->isCImage;
		}
		else
		{
			nextLibrary->isCImage = ILLinkerIsCObject(image);
			originator = ILAssemblyGetOriginator(assem, &originatorLen);
			if(originator && originatorLen)
			{
				nextLibrary->publicKey =
					(unsigned char *)ILMalloc(originatorLen + 1);
				if(nextLibrary->publicKey)
				{
					ILMemCpy(nextLibrary->publicKey, originator, originatorLen);
					nextLibrary->publicKeyLen = originatorLen;
				}
			}
			nextLibrary->classHash =
				ILHashCreate(0, (ILHashComputeFunc)ClassHash_Compute,
								(ILHashKeyComputeFunc)ClassHash_KeyCompute,
								(ILHashMatchFunc)ClassHash_Match,
								(ILHashFreeFunc)0);
			if(!(nextLibrary->classHash))
			{
				if(nextLibrary->publicKey)
				{
					ILFree(nextLibrary->publicKey);
				}
				ILFree((void *)(nextLibrary->moduleName));
				ILFree((void *)(nextLibrary->filename));
				ILFree((void *)(nextLibrary->name));
				ILFree(nextLibrary);
				LibraryDestroy(library);
				return 0;
			}
			nextLibrary->symbolHash =
				ILHashCreate(0, (ILHashComputeFunc)SymbolHash_Compute,
								(ILHashKeyComputeFunc)SymbolHash_KeyCompute,
								(ILHashMatchFunc)SymbolHash_Match,
								(ILHashFreeFunc)0);
			if(!(nextLibrary->symbolHash))
			{
				ILHashDestroy(nextLibrary->classHash);
				if(nextLibrary->publicKey)
				{
					ILFree(nextLibrary->publicKey);
				}
				ILFree((void *)(nextLibrary->moduleName));
				ILFree((void *)(nextLibrary->filename));
				ILFree((void *)(nextLibrary->name));
				ILFree(nextLibrary);
				LibraryDestroy(library);
				return 0;
			}
		}
		ILMemPoolInitType(&(nextLibrary->pool), ILLibraryClassOrSymbol, 0);
		nextLibrary->next = 0;
		nextLibrary->context = context;
		nextLibrary->image = image;
		if(lastLibrary)
		{
			lastLibrary->altNames = nextLibrary;
		}
		else
		{
			library = nextLibrary;
		}
		lastLibrary = nextLibrary;
	}

	/* Bail out if there are no assembly definitions */
	if(!library)
	{
		fprintf(stderr, "%s: missing assembly definition tokens", filename);
		linker->error = 1;
		return 0;
	}

	/* Return the library list to the caller */
	return library;
}

/*
 * Walk a public type and all of its visible nested types.
 */
static int WalkTypeAndNested(ILLinker *linker, ILImage *image,
							 ILLibrary *library, ILClass *classInfo,
							 ILLibraryClass *parent)
{
	ILNestedInfo *nestedInfo;
	ILClass *child;
	ILLibraryClass *libClass;
	const char *name;
	const char *namespace;

	/* Add the name of this type to the library's hash table */
	name = (ILInternString((char *)(ILClass_Name(classInfo)), -1)).string;
	namespace = (const char *)(ILClass_Namespace(classInfo));
	if(namespace)
	{
		namespace = (ILInternString(namespace, -1)).string;
	}
	if((libClass = ILMemPoolAlloc(&(library->pool), ILLibraryClass)) == 0)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}
	libClass->name = name;
	libClass->namespace = namespace;
	libClass->parent = parent;
	if(!ILHashAdd(library->classHash, libClass))
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}

	/* Walk the visible nested types */
	nestedInfo = 0;
	while((nestedInfo = ILClassNextNested(classInfo, nestedInfo)) != 0)
	{
		child = ILNestedInfoGetChild(nestedInfo);
		switch(ILClass_Attrs(child) & IL_META_TYPEDEF_VISIBILITY_MASK)
		{
			case IL_META_TYPEDEF_PUBLIC:
			case IL_META_TYPEDEF_NESTED_PUBLIC:
			case IL_META_TYPEDEF_NESTED_FAMILY:
			case IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM:
			{
				if(!WalkTypeAndNested(linker, image, library, child, libClass))
				{
					return 0;
				}
			}
			break;
		}
	}

	/* Done */
	return 1;
}

/*
 * Add a global symbol definition to a library.
 */
static int AddGlobalSymbol(ILLinker *linker, ILLibrary *library,
						   const char *name, char *aliasFor, int flags,
						   ILMember *member)
{
	ILLibrarySymbol *libSymbol;
	if(library)
	{
		if((libSymbol = ILMemPoolAlloc(&(library->pool), ILLibrarySymbol)) == 0)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		libSymbol->name = name;
		libSymbol->aliasFor = aliasFor;
		libSymbol->flags = flags;
		libSymbol->member = member;
		if(!ILHashAdd(library->symbolHash, libSymbol))
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
	}
	else
	{
		/* Search for an existing definition */
		libSymbol = ILHashFindType(linker->symbolHash, name, ILLibrarySymbol);
		if(libSymbol)
		{
			/* Don't report an error if it looks like a PInvoke or
			   field RVA definition.  This can happen with code
			   generated by certain C compilers */
			if(ILMember_IsMethod(member))
			{
				if(ILMethod_HasPInvokeImpl((ILMethod *)member))
				{
					return 1;
				}
			}
			if(ILMember_IsField(member))
			{
				if((ILField_Attrs((ILField *)member) &
						IL_META_FIELDDEF_HAS_FIELD_RVA) != 0)
				{
					return 1;
				}
			}
			if((libSymbol->flags & IL_LINKSYM_SAW_UNDEF) == 0)
			{
				fprintf(stderr, "%s : multiply defined\n", name);
				libSymbol->flags |= IL_LINKSYM_SAW_UNDEF;
			}
			linker->error = 1;
			return 1;
		}

		/* Add the definition to the image being linked */
		if((libSymbol = ILMemPoolAlloc(&(linker->pool), ILLibrarySymbol)) == 0)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		libSymbol->name = name;
		libSymbol->aliasFor = aliasFor;
		libSymbol->flags = flags;
		libSymbol->member = member;
		if(!ILHashAdd(linker->symbolHash, libSymbol))
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
	}
	return 1;
}

/*
 * Walk the global symbol definitions in a "<Module>" type.
 */
static int WalkGlobals(ILLinker *linker, ILImage *image,
					   ILLibrary *library, ILClass *classInfo)
{
	ILMember *member;
	ILField *field;
	ILMethod *method;
	const char *name;
	char *aliasFor;
	int flags;

	/* Find all public static fields and methods in the type */
	member = 0;
	while((member = ILClassNextMember(classInfo, member)) != 0)
	{
		if(ILMember_IsField(member))
		{
			field = (ILField *)member;
			if(ILField_IsPublic(field) && ILField_IsStatic(field))
			{
				name = (ILInternString
					((char *)(ILField_Name(field)), -1)).string;
				flags = IL_LINKSYM_VARIABLE;
				aliasFor = ILLinkerGetStringAttribute
					(ILToProgramItem(field),
					 "StrongAliasForAttribute", "OpenSystem.C");
				if(aliasFor)
				{
					flags |= IL_LINKSYM_STRONG;
				}
				if(!AddGlobalSymbol(linker, library, name,
									aliasFor, flags, member))
				{
					return 0;
				}
			}
		}
		else if(ILMember_IsMethod(member))
		{
			method = (ILMethod *)member;
			if(ILMethod_IsPublic(method) && ILMethod_IsStatic(method))
			{
				name = (ILInternString
					(ILMethod_Name(method), -1)).string;
				flags = IL_LINKSYM_FUNCTION;
				aliasFor = ILLinkerGetStringAttribute
					(ILToProgramItem(method),
					 "StrongAliasForAttribute", "OpenSystem.C");
				if(aliasFor)
				{
					flags |= IL_LINKSYM_STRONG;
				}
				else
				{
					aliasFor = ILLinkerGetStringAttribute
						(ILToProgramItem(method),
						 "WeakAliasForAttribute", "OpenSystem.C");
					if(aliasFor)
					{
						flags |= IL_LINKSYM_WEAK;
					}
				}
				if(!AddGlobalSymbol(linker, library, name,
									aliasFor, flags, member))
				{
					return 0;
				}
			}
		}
	}

	/* Done */
	return 1;
}

/*
 * Walk the type definitions within an image to build
 * a list of names that can be linked against.
 */
static int WalkTypeDefs(ILLinker *linker, ILImage *image, ILLibrary *library)
{
	ILClass *classInfo = 0;
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, (void *)classInfo)) != 0)
	{
		/* Nested classes are processed when their parent is scanned */
		if(!ILClass_NestedParent(classInfo))
		{
			/* Only "public" classes are visible outside the assembly */
			if(ILClass_IsPublic(classInfo))
			{
				if(!WalkTypeAndNested(linker, image, library, classInfo, 0))
				{
					return 0;
				}
			}

			/* If this is the "<Module>" type, then walk the global symbols */
			if(_ILLinkerIsModule(classInfo))
			{
				if(library)
				{
					/* Set the module name for the library */
					char *name = ILDupString(ILClass_Name(classInfo));
					if(!name)
					{
						_ILLinkerOutOfMemory(linker);
						return 0;
					}
					ILFree((void *)(library->moduleName));
					library->moduleName = name;
				}
				if(!WalkGlobals(linker, image, library, classInfo))
				{
					return 0;
				}
				continue;
			}

			/* If this type is marked with "OpenSystem.C.GlobalScope", then
			   it should also be inspected for global symbol definitions */
			if(_ILLinkerIsGlobalScope(classInfo))
			{
				if(!WalkGlobals(linker, image, library, classInfo))
				{
					return 0;
				}
			}
		}
	}
	return 1;
}

/*
 * Add a library image to the linker context.
 */
static ILLibrary *AddLibrary(ILLinker *linker, ILContext *context,
							 ILImage *image, const char *filename)
{
	ILLibrary *library;

	/* Create ILLibrary records for the assembly definitions */
	library = ScanAssemblies(linker, context, image, filename);
	if(!library)
	{
		return 0;
	}

	/* Add the library to the linker's list */
	if(linker->lastLibrary)
	{
		linker->lastLibrary->next = library;
	}
	else
	{
		linker->libraries = library;
	}
	linker->lastLibrary = library;
	return library;
}

/*
 * Find a library by filename.
 */
static ILLibrary *FindLibraryByFilename(ILLinker *linker, const char *filename)
{
	ILLibrary *library;
	library = linker->libraries;
	while(library != 0)
	{
		if(!strcmp(library->filename, filename))
		{
			return library;
		}
		library = library->next;
	}
	return 0;
}

int ILLinkerAddLibrary(ILLinker *linker, const char *name)
{
	FILE *file;
	ILContext *context;
	int loadError;
	ILImage *image;
	int result = 1;
	char *newFilename;
	ILLibrary *library;
	ILAssembly *assem;

	/* Bail out if we already have a library with this assembly name */
	if(_ILLinkerFindLibrary(linker, name) != 0)
	{
		return 1;
	}

	/* Resolve the library name */
	newFilename = ILLinkerResolveLibrary(linker, name);
	if(!newFilename)
	{
		fprintf(stderr, "%s: library not found\n", name);
		linker->error = 1;
		return 0;
	}

	/* Bail out if we already have a library with this filename */
	if(FindLibraryByFilename(linker, newFilename) != 0)
	{
		ILFree(newFilename);
		return 1;
	}

	/* Open the library file */
	if((file = fopen(newFilename, "rb")) == NULL)
	{
		/* Try again in case libc does not understand "rb" */
		if((file = fopen(newFilename, "r")) == NULL)
		{
			perror(newFilename);
			ILFree(newFilename);
			linker->error = 1;
			return 0;
		}
	}

	/* Load the library as an image */
	context = ILContextCreate();
	if(!context)
	{
		_ILLinkerOutOfMemory(linker);
		ILFree(newFilename);
		return 0;
	}
	loadError = ILImageLoad(file, newFilename, context, &image,
							IL_LOADFLAG_FORCE_32BIT | IL_LOADFLAG_NO_RESOLVE);
	if(loadError)
	{
		linker->error = 1;
		fprintf(stderr, "%s: %s\n", newFilename, ILImageLoadError(loadError));
		ILContextDestroy(context);
		fclose(file);
		ILFree(newFilename);
		return 0;
	}
	fclose(file);

	/* Add the library image to the linker context */
	library = AddLibrary(linker, context, image, newFilename);
	if(!library)
	{
		linker->error = 1;
		result = 0;
	}

	/* Clean up temporary memory that we used */
	ILFree(newFilename);

	/* Add assemblies that were referenced by this library, because
	   this library may contain TypeRef's in its public signatures
	   to some other assembly, and we need to import them too */
	if(result)
	{
		assem = 0;
		while((assem = (ILAssembly *)ILImageNextToken
					(image, IL_META_TOKEN_ASSEMBLY_REF, (void *)assem)) != 0)
		{
			if(!ILLinkerAddLibrary(linker, ILAssembly_Name(assem)))
			{
				result = 0;
				break;
			}
		}
	}

	/* Walk the type definitions in the library to discover what is present */
	if(result && !WalkTypeDefs(linker, image, library))
	{
		result = 0;
	}

	/* Return the final result to the caller */
	return result;
}

void _ILLinkerDestroyLibraries(ILLinker *linker)
{
	ILLibrary *library;
	ILLibrary *next;
	library = linker->libraries;
	while(library != 0)
	{
		next = library->next;
		LibraryDestroy(library);
		library = next;
	}
}

ILLibrary *_ILLinkerFindLibrary(ILLinker *linker, const char *name)
{
	ILLibrary *library;
	ILLibrary *altName;
	library = linker->libraries;
	while(library != 0)
	{
		altName = library;
		do
		{
			if(!ILStrICmp(altName->name, name))
			{
				return library;
			}
			altName = altName->altNames;
		}
		while(altName != 0);
		library = library->next;
	}
	return 0;
}

void _ILLinkerFindInit(ILLibraryFind *find, ILLinker *linker,
					   ILLibrary *library)
{
	find->linker = linker;
	find->library = library;
	find->libClass = 0;
	find->prevClass = 0;
}

/*
 * Find a class by name within a library.
 */
static ILLibraryClass *FindClass(ILLibrary *library,
								 const char *name, const char *namespace,
								 ILLibraryClass *parent)
{
	ClassHashKey key;
	key.name = name;
	key.namespace = namespace;
	key.parent = parent;
	return ILHashFindType(library->classHash, &key, ILLibraryClass);
}

int _ILLinkerFindClass(ILLibraryFind *find, const char *name,
					   const char *namespace)
{
	ILLibraryClass *parent;
	find->prevClass = find->libClass;
	if(!(find->libClass))
	{
		/* Look for a top-level class within the library */
		if(find->library)
		{
			/* We already know which library to look in */
			find->libClass = FindClass(find->library,
									   name, namespace, 0);
		}
		else
		{
			/* Look for the class in any library */
			find->library = find->linker->libraries;
			while(find->library != 0)
			{
				find->libClass = FindClass(find->library,
										   name, namespace, 0);
				if(find->libClass)
				{
					break;
				}
				find->library = find->library->next;
			}
		}
	}
	else
	{
		/* Look for a nested class within the last class found */
		parent = find->libClass;
		find->libClass = FindClass(find->library,
								   name, namespace, parent);
	}
	return (find->libClass != 0);
}

ILClass *_ILLinkerFindByName(ILLinker *linker, const char *name,
							 const char *namespace)
{
	ILLibraryFind find;
	_ILLinkerFindInit(&find, linker, 0);
	if(_ILLinkerFindClass(&find, name, namespace))
	{
		return _ILLinkerMakeTypeRef(&find, linker->image);
	}
	else
	{
		return 0;
	}
}

static void PrintParentClass(ILLibraryClass *classInfo)
{
	if(classInfo)
	{
		PrintParentClass(classInfo->parent);
		if(classInfo->namespace)
		{
			fputs(classInfo->namespace, stderr);
			putc('.', stderr);
		}
		fputs(classInfo->name, stderr);
		putc('/', stderr);
	}
}

void _ILLinkerPrintClass(ILLibraryFind *find, const char *name,
						 const char *namespace)
{
	if(find->library)
	{
		putc('[', stderr);
		fputs(find->library->name, stderr);
		putc(']', stderr);
	}
	PrintParentClass(find->prevClass);
	if(namespace)
	{
		fputs(namespace, stderr);
		putc('.', stderr);
	}
	fputs(name, stderr);
}

/*
 * Set the public key token for an assembly reference.
 */
static int SetAssemblyRefToken(ILAssembly *assem, unsigned char *key,
							   ILUInt32 keyLen)
{
	ILSHAContext sha;
	unsigned char hash[IL_SHA_HASH_SIZE];
	unsigned char token[8];
	int posn;

	/* Compute the SHA1 hash of the key value */
	ILSHAInit(&sha);
	ILSHAData(&sha, key, (unsigned long)keyLen);
	ILSHAFinalize(&sha, hash);

	/* The token is the last 8 bytes of the hash, reversed */
	for(posn = 0; posn < 8; ++posn)
	{
		token[posn] = hash[IL_SHA_HASH_SIZE - posn - 1];
	}

	/* Set the token on the assembly reference record */
	return ILAssemblySetOriginator(assem, token, 8);
}

/*
 * Make a type reference within an image.
 */
static ILClass *MakeTypeRef(ILLibraryFind *find, ILLibraryClass *libClass,
							ILImage *image)
{
	ILClass *parent;
	ILClass *classInfo;
	ILAssembly *assem;

	/* Make a new type reference */
	if(libClass->parent)
	{
		/* Make a reference to a nested class */
		parent = MakeTypeRef(find, libClass->parent, image);
		if(!parent)
		{
			return 0;
		}
		classInfo = ILClassLookup(ILToProgramItem(parent),
								  libClass->name, libClass->namespace);
		if(classInfo)
		{
			return classInfo;
		}
		classInfo = ILClassCreateRef(ILToProgramItem(parent), 0,
								     libClass->name, libClass->namespace);
		if(!classInfo)
		{
			_ILLinkerOutOfMemory(find->linker);
		}
		return classInfo;
	}
	else
	{
		/* Look for an AssemblyRef to use to import the class */
		assem = 0;
		while((assem = (ILAssembly *)ILImageNextToken
					(image, IL_META_TOKEN_ASSEMBLY_REF, (void *)assem)) != 0)
		{
			if(!ILStrICmp(find->library->name, ILAssembly_Name(assem)) &&
			   !ILMemCmp(find->library->version,
						 ILAssemblyGetVersion(assem), sizeof(ILUInt16) * 4))
			{
				break;
			}
		}
		if(!assem)
		{
			/* Create an AssemblyRef to use to import the class */
			assem = ILAssemblyCreate(image, 0, find->library->name, 1);
			if(!assem)
			{
				_ILLinkerOutOfMemory(find->linker);
				return 0;
			}
			ILAssemblySetVersion(assem, find->library->version);
			if(find->library->publicKey)
			{
				if(!SetAssemblyRefToken(assem, find->library->publicKey,
										find->library->publicKeyLen))
				{
					_ILLinkerOutOfMemory(find->linker);
					return 0;
				}
			}
		}

		/* See if we already have a reference */
		classInfo = ILClassLookup(ILToProgramItem(assem),
								  libClass->name, libClass->namespace);
		if(classInfo)
		{
			return classInfo;
		}

		/* Import the class using the AssemblyRef */
		classInfo = ILClassCreateRef(ILToProgramItem(assem), 0,
									 libClass->name,
									 libClass->namespace);
		if(!classInfo)
		{
			_ILLinkerOutOfMemory(find->linker);
		}
		return classInfo;
	}
}

ILClass *_ILLinkerMakeTypeRef(ILLibraryFind *find, ILImage *image)
{
	if(find->libClass)
	{
		return MakeTypeRef(find, find->libClass, image);
	}
	else
	{
		return 0;
	}
}

/*
 * Create a "MemberRef" token that refers to a global symbol in a library.
 */
static ILMember *CreateSymbolRef(ILLinker *linker, ILLibrary *library,
								 ILLibrarySymbol *libSymbol)
{
	ILLibraryFind find;
	ILLibraryClass libClass;
	ILClass *classInfo;
	ILType *signature;
	ILField *field;
	ILMethod *method;

	/* See if we have a cached "MemberRef" from last time */
	if((libSymbol->flags & IL_LINKSYM_HAVE_REF) != 0)
	{
		return libSymbol->member;
	}

	/* Make a "TypeRef" for the library's "<Module>" type */
	if(libSymbol->member &&
	   !_ILLinkerIsModule(ILMember_Owner(libSymbol->member)))
	{
		/* The symbol is in a C# class marked with "GlobalScope" */
		classInfo = _ILLinkerConvertClassRef
			(linker, ILMember_Owner(libSymbol->member));
	}
	else if(library)
	{
		find.linker = linker;
		find.library = library;
		libClass.name = library->moduleName;
		libClass.namespace = 0;
		libClass.parent = 0;
		classInfo = MakeTypeRef(&find, &libClass, linker->image);
		if(!classInfo)
		{
			return 0;
		}
	}
	else
	{
		classInfo = _ILLinkerModuleClass(linker);
	}

	/* Create a "MemberRef" token and cache it */
	if((libSymbol->flags & IL_LINKSYM_VARIABLE) != 0)
	{
		field = ILFieldCreate(classInfo, (ILToken)IL_MAX_UINT32,
							  libSymbol->name, 0);
		if(!field)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		signature = _ILLinkerConvertType
			(linker, ILMember_Signature(libSymbol->member));
		if(!signature)
		{
			return 0;
		}
		ILMemberSetSignature((ILMember *)field, signature);
		libSymbol->member = (ILMember *)field;
		libSymbol->flags |= IL_LINKSYM_HAVE_REF;
		return (ILMember *)field;
	}
	else
	{
		method = ILMethodCreate(classInfo, (ILToken)IL_MAX_UINT32,
							    libSymbol->name, 0);
		if(!method)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		signature = _ILLinkerConvertType
			(linker, ILMember_Signature(libSymbol->member));
		if(!signature)
		{
			return 0;
		}
		ILMemberSetSignature((ILMember *)method, signature);
		ILMethodSetCallConv(method, ILType_CallConv(signature));
		libSymbol->member = (ILMember *)method;
		libSymbol->flags |= IL_LINKSYM_HAVE_REF;
		return (ILMember *)method;
	}
}

int _ILLinkerFindSymbol(ILLinker *linker, const char *name,
						const char **aliasFor, ILLibrary **library,
						ILMember **memberRef)
{
	ILLibrary *lib;
	ILLibrarySymbol *libSymbol;

	/* Search the image that is being linked for the symbol */
	libSymbol = ILHashFindType(linker->symbolHash, name, ILLibrarySymbol);
	if(libSymbol)
	{
		/* Resolve strong symbol references within the image */
		while(libSymbol && (libSymbol->flags & IL_LINKSYM_STRONG) != 0)
		{
			libSymbol = ILHashFindType(linker->symbolHash,
									   libSymbol->aliasFor,
									   ILLibrarySymbol);
		}

		/* Return the symbol information to the caller */
		if(libSymbol)
		{
			*aliasFor = libSymbol->aliasFor;
			*library = 0;
			*memberRef = CreateSymbolRef(linker, 0, libSymbol);
			if(!(*memberRef))
			{
				return 0;
			}
			return libSymbol->flags;
		}
	}

	/* Search the libraries for the symbol */
	lib = linker->libraries;
	while(lib != 0)
	{
		libSymbol = ILHashFindType(lib->symbolHash, name, ILLibrarySymbol);
		if(libSymbol)
		{
			/* Resolve strong symbol references within the same library */
			while(libSymbol && (libSymbol->flags & IL_LINKSYM_STRONG) != 0)
			{
				libSymbol = ILHashFindType(lib->symbolHash,
										   libSymbol->aliasFor,
										   ILLibrarySymbol);
			}

			/* Return the symbol information to the caller */
			if(libSymbol)
			{
				*aliasFor = libSymbol->aliasFor;
				*library = lib;
				*memberRef = CreateSymbolRef(linker, lib, libSymbol);
				if(!(*memberRef))
				{
					return 0;
				}
				return libSymbol->flags;
			}
		}
		lib = lib->next;
	}

	/* We could not find the symbol */
	return 0;
}

void _ILLinkerUpdateSymbol(ILLinker *linker, const char *name,
						   ILMember *member)
{
	ILLibrarySymbol *libSymbol;
	libSymbol = ILHashFindType(linker->symbolHash, name, ILLibrarySymbol);
	if(libSymbol && (libSymbol->flags & IL_LINKSYM_HAVE_REF) == 0)
	{
		libSymbol->member = member;
		libSymbol->flags |= IL_LINKSYM_HAVE_REF;
	}
}

int _ILLinkerHasSymbol(ILLinker *linker, const char *name)
{
	ILLibrarySymbol *libSymbol;
	libSymbol = ILHashFindType(linker->symbolHash, name, ILLibrarySymbol);
	return (libSymbol != 0);
}

int _ILLinkerCreateSymbolHash(ILLinker *linker)
{
	linker->symbolHash =
		ILHashCreate(0, (ILHashComputeFunc)SymbolHash_Compute,
						(ILHashKeyComputeFunc)SymbolHash_KeyCompute,
						(ILHashMatchFunc)SymbolHash_Match,
						(ILHashFreeFunc)0);
	if(!(linker->symbolHash))
	{
		return 0;
	}
	ILMemPoolInitType(&(linker->pool), ILLibraryClassOrSymbol, 0);
	return 1;
}

void _ILLinkerAddSymbols(ILLinker *linker, ILImage *image)
{
	ILClass *classInfo = 0;
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, (void *)classInfo)) != 0)
	{
		/* If this is the "<Module>" type, then walk the global symbols */
		if(_ILLinkerIsModule(classInfo))
		{
			if(!WalkGlobals(linker, image, 0, classInfo))
			{
				return;
			}
			continue;
		}

		/* If this type is marked with "OpenSystem.C.GlobalScope", then
		   it should also be inspected for global symbol definitions */
		if(_ILLinkerIsGlobalScope(classInfo))
		{
			if(!WalkGlobals(linker, image, 0, classInfo))
			{
				return;
			}
		}
	}
}

#ifdef	__cplusplus
};
#endif
