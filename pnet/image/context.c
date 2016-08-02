/*
 * context.c - Utility routines for manipulting IL contexts.
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

#include "program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Free a path list and the containing paths.
 */
static void PathListFree(char **dirs, int numDirs)
{
	if(dirs)
	{
		if(numDirs > 0)
		{
			int i; /* a loop counter */
			for(i = 0; i < numDirs; i++)
			{
				if(dirs[i])
				{
					ILFree(dirs[i]);
				}
			}
		}
		/* we don't know anything about the size so free at least the array */
		ILFree(dirs);
	}
}

/*
 * Hash a string, while ignoring case.
 */
static unsigned long HashIgnoreCase(unsigned long start,
									const char *str, int len)
{
	unsigned long hash = start;
	int ch;
	while(len > 0)
	{
		ch = (*str++ & 0xFF);
		if(ch >= 'A' && ch <= 'Z')
		{
			hash = (hash << 5) + hash + (unsigned long)(ch - 'A' + 'a');
		}
		else
		{
			hash = (hash << 5) + hash + (unsigned long)ch;
		}
		--len;
	}
	return hash;
}

/*
 * Compute the hash value for a class.
 */
static unsigned long ClassHash_Compute(const ILClassName *classInfo)
{
	unsigned long hash;
	if(classInfo->namespace)
	{
		hash = HashIgnoreCase(0, classInfo->namespace,
							  strlen(classInfo->namespace));
		hash = HashIgnoreCase(hash, ".", 1);
	}
	else
	{
		hash = 0;
	}
	return HashIgnoreCase(hash, classInfo->name, strlen(classInfo->name));
}

/*
 * Compute the hash value for a class key.
 */
static unsigned long ClassHash_KeyCompute(const ILClassKeyInfo *key)
{
	unsigned long hash;
	if(key->namespace)
	{
		hash = HashIgnoreCase(0, key->namespace, key->namespaceLen);
		hash = HashIgnoreCase(hash, ".", 1);
	}
	else
	{
		hash = 0;
	}
	return HashIgnoreCase(hash, key->name, key->nameLen);
}

/*
 * Match a hash table element against a supplied key.
 */
static int ClassHash_Match(const ILClassName *classInfo,
						   const ILClassKeyInfo *key)
{
	/* Match the namespace */
	if(classInfo->namespace)
	{
		if(!(key->namespace))
		{
			return 0;
		}
		if(strncmp(classInfo->namespace, key->namespace,
				   key->namespaceLen) != 0 ||
		   classInfo->namespace[key->namespaceLen] != '\0')
		{
			return 0;
		}
	}

	/* Match the name */
	if(strncmp(classInfo->name, key->name, key->nameLen) != 0 ||
	   classInfo->name[key->nameLen] != '\0')
	{
		return 0;
	}

	/* Match the scope */
	if(key->scopeItem &&
	   (key->scopeItem->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MODULE)
	{
		/* Exported types in a file scope will also match module requests */
		if(classInfo->scope &&
	       (classInfo->scope->token & IL_META_TOKEN_MASK) ==
		   			IL_META_TOKEN_FILE &&
		   key->scopeItem->image == classInfo->scope->image)
		{
			goto moduleScope;
		}
	}
	if(key->scopeItem && key->scopeItem != classInfo->scope)
	{
		return 0;
	}
	else if(key->scopeName && key->scopeName != classInfo->scopeName)
	{
		return 0;
	}
moduleScope:

	/* Match the image */
	if(key->image && key->image != classInfo->image)
	{
		return 0;
	}

	/* Do we only want types at the global level? */
	if(key->wantGlobal)
	{
		if(classInfo->scopeName)
		{
			/* Nested type */
			return 0;
		}
		if((classInfo->scope->token & IL_META_TOKEN_MASK) !=
					IL_META_TOKEN_MODULE)
		{
			/* Imported type */
			return 0;
		}
	}

	/* We have a match */
	return 1;
}

/*
 * Compute the hash value for a namespace.
 */
static unsigned long NamespaceHash_Compute(const ILClassName *classInfo)
{
	if(classInfo->namespace)
	{
		return HashIgnoreCase(0, classInfo->namespace,
							  strlen(classInfo->namespace));
	}
	else
	{
		return 0;
	}
}

/*
 * Compute the hash value for a namespace key.
 */
static unsigned long NamespaceHash_KeyCompute(const char *key)
{
	if(key)
	{
		return HashIgnoreCase(0, key, strlen(key));
	}
	else
	{
		return 0;
	}
}

/*
 * Match a hash table element against a supplied namespace key.
 */
static int NamespaceHash_Match(const ILClassName *classInfo, const char *key)
{
	int len;
	if(classInfo->namespace)
	{
		if(!key)
		{
			return 0;
		}
		len = strlen(key);
		if(strncmp(classInfo->namespace, key, len) != 0 ||
		   classInfo->namespace[len] != '\0')
		{
			return 0;
		}
		return 1;
	}
	else
	{
		return (key == 0);
	}
}

/* Defined in "synthetic.c" */
int _ILContextSyntheticInit(ILContext *context);

ILContext *ILContextCreate(void)
{
	ILContext *context = (ILContext *)ILCalloc(1, sizeof(ILContext));
	if(!context)
	{
		return 0;
	}
	if((context->classHash = ILHashCreate
				(IL_CONTEXT_HASH_SIZE,
			     (ILHashComputeFunc)ClassHash_Compute,
				 (ILHashKeyComputeFunc)ClassHash_KeyCompute,
				 (ILHashMatchFunc)ClassHash_Match,
				 (ILHashFreeFunc)0)) == 0)
	{
		ILFree(context);
		return 0;
	}
	ILMemPoolInitType(&(context->typePool), ILType, 0);
	if(!_ILContextSyntheticInit(context))
	{
		ILContextDestroy(context);
		return 0;
	}
	if((context->namespaceHash = ILHashCreate
				(IL_CONTEXT_NS_HASH_SIZE,
			     (ILHashComputeFunc)NamespaceHash_Compute,
				 (ILHashKeyComputeFunc)NamespaceHash_KeyCompute,
				 (ILHashMatchFunc)NamespaceHash_Match,
				 (ILHashFreeFunc)0)) == 0)
	{
		ILFree(context);
		return 0;
	}
	return context;
}

void ILContextDestroy(ILContext *context)
{
	/* Destroy the images */
	while(context->firstImage != 0)
	{
		ILImageDestroy(context->firstImage);
	}

	/* Destroy the class hash */
	ILHashDestroy(context->classHash);

	/* Destroy the namespace hash */
	ILHashDestroy(context->namespaceHash);

	/* Destroy the synthetic types hash */
	ILHashDestroy(context->syntheticHash);

	/* Destroy the type pool */
	ILMemPoolDestroy(&(context->typePool));

	/* Destory the redo table */
	if(context->redoItems)
	{
		ILFree(context->redoItems);
	}

	/* destroy the list of search directories */
	PathListFree(context->libraryDirs, context->numLibraryDirs);

	/* destroy the list of shadow copy directories */
	PathListFree(context->shadowCopyDirs, context->numShadowCopyDirs);

	/* destroy the cache directory */
	if(context->cacheDir)
	{
		ILFree(context->cacheDir);
	}

	/* destroy the applicationBaseDir */
	if(context->applicationBaseDir)
	{
		ILFree(context->applicationBaseDir);
	}

	/* Destroy the context itself */
	ILFree(context);
}

const char *_ILContextPersistString(ILImage *image, const char *str)
{
	if(str)
	{
		if(image->type == IL_IMAGETYPE_BUILDING)
		{
			/* We need to create a persistent version in the "#Strings" blob */
			ILUInt32 offset;
			offset = ILImageAddString(image, str);
			if(offset)
			{
				return ILImageGetString(image, offset);
			}
		}
		else
		{
			/* The string is assumed to already be persistent */
			return str;
		}
	}
	return 0;
}

const char *_ILContextPersistMalloc(ILImage *image, char *str)
{
	if(str)
	{
		ILUInt32 len = sizeof(ILStringBlock) + strlen(str) + 1;
		ILStringBlock *block = (ILStringBlock *)ILMalloc(len);
		if(!block)
		{
			ILFree(str);
			return 0;
		}
		block->next = image->extraStrings;
		image->extraStrings = block;
		block->used = len;
		block->len = len;
		strcpy((char *)(block + 1), str);
		ILFree(str);
		return (const char *)(block + 1);
	}
	return 0;
}

static ILImage *GetImageByName(ILContext *context, const char *name,
							   unsigned long tokenType)
{
	ILImage *image;
	unsigned long numTokens;
	unsigned long token;
	const char *imageName;
	void *data;

	image = context->firstImage;
	while(image != 0)
	{
		numTokens = ILImageNumTokens(image, tokenType);
		for(token = 1; token <= numTokens; ++token)
		{
			data = ILImageTokenInfo(image, tokenType | token);
			if(data)
			{
				if(tokenType == IL_META_TOKEN_MODULE)
				{
					imageName = ((ILModule *)data)->name;
				}
				else
				{
					imageName = ((ILAssembly *)data)->name;
				}
				if(imageName && !ILStrICmp(imageName, name))
				{
					return image;
				}
			}
		}
		image = image->nextImage;
	}

	return 0;
}

ILImage *ILContextGetModule(ILContext *context, const char *name)
{
	return GetImageByName(context, name, IL_META_TOKEN_MODULE);
}

ILImage *ILContextGetAssembly(ILContext *context, const char *name)
{
	ILImage *image = GetImageByName(context, name, IL_META_TOKEN_ASSEMBLY);
	if(!image && !strcmp(name, "mscorlib"))
	{
		/* Handle systems that use "corlib.dll" instead of "mscorlib.dll" */
		image = GetImageByName(context, "corlib", IL_META_TOKEN_ASSEMBLY);
	}
	return image;
}

ILImage *ILContextGetFile(ILContext *context, const char *name)
{
	ILImage *image;
	const char *filename;
	int len;
	image = context->firstImage;
	while(image != 0)
	{
		filename = image->filename;
		if(filename)
		{
			len = strlen(filename);
			if(!ILStrICmp(filename, name))
			{
				return image;
			}
			while(len > 0 && filename[len - 1] != '/' &&
				  filename[len - 1] != '\\')
			{
				--len;
			}
			if(!ILStrICmp(filename + len, name))
			{
				return image;
			}
		}
		image = image->nextImage;
	}
	return 0;
}

ILImage *ILContextNextImage(ILContext *context, ILImage *image)
{
	if(image)
	{
		return image->nextImage;
	}
	else
	{
		return context->firstImage;
	}
}

ILImage *ILContextGetSynthetic(ILContext *context)
{
	ILImage *image;

	/* If we already have a synthetic types image, then return it */
	if(context->syntheticImage)
	{
		return context->syntheticImage;
	}

	/* Create a new image */
	image = ILImageCreate(context);
	if(!image)
	{
		return 0;
	}

	/* Create the "Module" record for the image */
	if(!ILModuleCreate(image, 0, "$Synthetic", 0))
	{
		ILImageDestroy(image);
		return 0;
	}

	/* Create the "Assembly" record for the image */
	if(!ILAssemblyCreate(image, 0, "$Synthetic", 0))
	{
		ILImageDestroy(image);
		return 0;
	}

	/* Create the main "<Module>" type for the image */
	if(!ILClassCreate(ILClassGlobalScope(image), 0, "<Module>", 0, 0))
	{
		ILImageDestroy(image);
		return 0;
	}

	/* Attach the synthetic image to the context and return */
	context->syntheticImage = image;
	return image;
}

void ILContextSetSystem(ILContext *context, ILImage *image)
{
	context->systemImage = image;
}

ILImage *ILContextGetSystem(ILContext *context)
{
	return context->systemImage;
}

void ILContextSetUserData(ILContext *context, void *userData)
{
	context->userData = userData;
}

void *ILContextGetUserData(ILContext *context)
{
	return context->userData;
}

void ILContextSetApplicationBaseDir(ILContext *context, char *applicationBaseDir)
{
	if(context->applicationBaseDir &&
	   (context->applicationBaseDir != applicationBaseDir))
	{
		ILFree(context->applicationBaseDir);
	}
	context->applicationBaseDir = applicationBaseDir;

}

const char *ILContextGetApplicationBaseDir(ILContext *context)
{
	return context->applicationBaseDir;
}

void ILContextSetCacheDir(ILContext *context, char *cacheDir)
{
	if(context->cacheDir && (context->cacheDir != cacheDir))
	{
		ILFree(context->cacheDir);
	}
	context->cacheDir = cacheDir;
}

const char *ILContextGetCacheDir(ILContext *context)
{
	return context->cacheDir;
}

void ILContextSetDynamicBaseDir(ILContext *context, char *dynamicBaseDir)
{
	if(context->dynamicBaseDir && (context->dynamicBaseDir != dynamicBaseDir))
	{
		ILFree(context->dynamicBaseDir);
	}
	context->dynamicBaseDir = dynamicBaseDir;
}

const char *ILContextGetDynamicBaseDir(ILContext *context)
{
	return context->dynamicBaseDir;
}

void ILContextSetLibraryDirs(ILContext *context,
							 char **libraryDirs,
							 int numLibraryDirs)
{
	if(context->libraryDirs && (context->libraryDirs != libraryDirs))
	{
		PathListFree(context->libraryDirs, context->numLibraryDirs);
	}
	context->libraryDirs = libraryDirs;
	context->numLibraryDirs = numLibraryDirs;
}

void ILContextGetLibraryDirs(ILContext *context,
							 char ***libraryDirs,
							 int *numLibraryDirs)
{
	*libraryDirs = context->libraryDirs;
	*numLibraryDirs = context->numLibraryDirs;
}

void ILContextClearLibraryDirs(ILContext *context)
{
	if(context->libraryDirs)
	{
		PathListFree(context->libraryDirs, context->numLibraryDirs);
	}
	context->libraryDirs = 0;
	context->numLibraryDirs = 0;
}

void ILContextSetRelativeSearchDir(ILContext *context, char *relativeSearchDir)
{
	if(context->relativeSearchDir &&
	   (context->relativeSearchDir != relativeSearchDir))
	{
		ILFree(context->relativeSearchDir);
	}
	context->relativeSearchDir = relativeSearchDir;

}

const char *ILContextGetRelativeSearchDir(ILContext *context)
{
	return context->relativeSearchDir;
}

void ILContextSetShadowCopyDirs(ILContext *context,
								 char **shadowCopyDirs,
								 int numShadowCopyDirs)
{
	if(context->shadowCopyDirs &&
	  (context->numShadowCopyDirs != numShadowCopyDirs))
	{
		PathListFree(context->shadowCopyDirs, context->numShadowCopyDirs);
	}
	context->shadowCopyDirs = shadowCopyDirs;
	context->numShadowCopyDirs = numShadowCopyDirs;
}

void ILContextGetShadowCopyDirs(ILContext *context,
							 	char ***shadowCopyDirs,
							 	int *numShadowCopyDirs)
{
	*shadowCopyDirs = context->shadowCopyDirs;
	*numShadowCopyDirs = context->numShadowCopyDirs;
}

void ILContextClearShadowCopyDirs(ILContext *context)
{
	if(context->shadowCopyDirs)
	{
		PathListFree(context->shadowCopyDirs, context->numShadowCopyDirs);
	}
	context->shadowCopyDirs = 0;
	context->numShadowCopyDirs = 0;
}

void ILContextSetShadowCopyFiles(ILContext *context, int shadowCopyFiles)
{
	context->shadowCopyFiles = shadowCopyFiles;
}

int ILContextGetShadowCopyFiles(ILContext *context)
{
	return context->shadowCopyFiles;
}

#ifdef	__cplusplus
};
#endif
