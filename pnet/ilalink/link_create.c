/*
 * link_create.c - Create and destroy linker contexts.
 *
 * Copyright (C) 2001, 2008, 2009  Southern Storm Software, Pty Ltd.
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
#include "../image/image.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILLinker *ILLinkerCreate(FILE *stream, int seekable, int type, int flags)
{
	ILLinker *linker;

	/* Create the linker context */
	if((linker = (ILLinker *)ILMalloc(sizeof(ILLinker))) == 0)
	{
		return 0;
	}

	/* Create the image building context and image */
	linker->context = ILContextCreate();
	if(!(linker->context))
	{
		ILFree(linker);
		return 0;
	}
	linker->image = ILImageCreate(linker->context);
	if(!(linker->context))
	{
		ILContextDestroy(linker->context);
		ILFree(linker);
		return 0;
	}

	/* Create the image writer */
	linker->writer = ILWriterCreate(stream, seekable, type, flags);
	if(!(linker->writer))
	{
		ILContextDestroy(linker->context);
		ILFree(linker);
		return 0;
	}

	/* Initialize the other linker fields */
	linker->libraries = 0;
	linker->lastLibrary = 0;
	linker->libraryDirs = 0;
	linker->numLibraryDirs = 0;
	linker->outOfMemory = 0;
	linker->error = 0;
	linker->is32Bit = ((flags & IL_WRITEFLAG_32BIT_ONLY) != 0);
	linker->hasDebug = 0;
	linker->linkerFlags = 0;
	linker->images = 0;
	linker->lastImage = 0;
	linker->resourceRVA = 0;
	linker->entryPoint = 0;
	linker->dataLength = 0;
	linker->tlsLength = 0;
	linker->isCLink = 0;
	if(!_ILLinkerCreateSymbolHash(linker))
	{
		ILWriterDestroy(linker->writer);
		ILContextDestroy(linker->context);
		ILFree(linker);
		return 0;
	}
	if(type == IL_IMAGETYPE_DLL)
	{
		linker->moduleName = IL_LINKER_DLL_MODULE_NAME;
	}
	else
	{
		linker->moduleName = IL_LINKER_EXE_MODULE_NAME;
	}
	linker->moduleName = ILDupString(linker->moduleName);
	if(!(linker->moduleName))
	{
		ILWriterDestroy(linker->writer);
		ILContextDestroy(linker->context);
		ILFree(linker);
		return 0;
	}
	linker->moduleClass = 0;
	linker->initTempFile = 0;

	/* Ready to go */
	return linker;
}

/*
 * Determine if a class reference is "System.Array".
 */
static int IsSystemArray(ILClass *classInfo)
{
	const char *temp;
	temp = ILClass_Namespace(classInfo);
	if(!temp || strcmp(temp, "System") != 0)
	{
		return 0;
	}
	if(strcmp(ILClass_Name(classInfo), "Array") != 0)
	{
		return 0;
	}
	return (ILClass_NestedParent(classInfo) == 0);
}

/*
 * Report that a particular class is unresolved.
 */
static void ReportUnresolvedClass(ILLinker *linker, ILClass *classInfo)
{
	ILDumpClassName(stderr, ILClassToImage(classInfo), classInfo, 0);
	fputs(" : unresolved type reference\n", stderr);
	linker->error = 1;
}

/*
 * Report unresolved type and member references.
 */
static void ReportUnresolved(ILLinker *linker)
{
	ILClass *classInfo;
	ILClass *parent;
	ILMember *member;
	int reported;
	ILType *signature;
	ILToken token;
	ILToken numTokens;
	int flags;

	/* Set the dump flags to use */
	if(linker->isCLink)
	{
		flags = IL_DUMP_C_TYPES;
	}
	else
	{
		flags = 0;
	}

	/* Scan the TypeRef table for unresolved types */
	classInfo = 0;
	while((classInfo = (ILClass *)ILImageNextToken
				(linker->image, IL_META_TOKEN_TYPE_REF, classInfo)) != 0)
	{
		/* Skip the reference if it has since been defined */
		if(!ILClassIsRef(classInfo))
		{
			continue;
		}

		/* Skip the reference if it is synthetic */
		if(ILClassGetSynType(classInfo) != 0)
		{
			continue;
		}

		/* Make sure that all nested parents are references */
		parent = classInfo;
		reported = 0;
		while((parent = ILClassGetNestedParent(parent)) != 0)
		{
			if(!ILClassIsRef(parent))
			{
				ReportUnresolvedClass(linker, classInfo);
				reported = 1;
				break;
			}
		}
		if(reported)
		{
			continue;
		}

		/* If the reference is in a module scope, then it is dangling */
		if(ILProgramItemToModule(ILClassGetScope(classInfo)) != 0)
		{
			/* Special case: "System.Array" may get linked in due to the
			   use of synthetic array types, but we don't really need it */
			if(IsSystemArray(classInfo))
			{
				linker->image->tokenData[IL_META_TOKEN_TYPE_REF >> 24]
					[(ILClass_Token(classInfo) & 0x00FFFFFF) - 1] = 0;
				continue;
			}
			ReportUnresolvedClass(linker, classInfo);
		}
	}

	/* Scan the MemberRef table for unresolved members */
	member = 0;
	numTokens = ILImageNumTokens(linker->image, IL_META_TOKEN_MEMBER_REF);
	for(token = 1; token <= numTokens; ++token)
	{
		/* Get the token and shift past if it was already set to null */
		member = (ILMember *)ILImageTokenInfo
			(linker->image, IL_META_TOKEN_MEMBER_REF | token);
		if(!member)
		{
			continue;
		}

		/* Skip method members that contain sentinels, as they
		   correspond to vararg call sites, which are OK */
		signature = ILMember_Signature(member);
		if(signature != 0 && ILType_IsComplex(signature) &&
		   ILType_Kind(signature) == (IL_TYPE_COMPLEX_METHOD |
									  IL_TYPE_COMPLEX_METHOD_SENTINEL))
		{
			if(_ILLinkerHasSymbol(linker, ILMember_Name(member)))
			{
				continue;
			}
		}

		/* Perform the normal checks */
		classInfo = ILMember_Owner(member);
		if(!ILClassIsRef(classInfo) &&
		   (ILMember_Token(member) & IL_META_TOKEN_MASK) ==
			IL_META_TOKEN_MEMBER_REF)
		{
			/* The class has been defined, but not the member */
			if(ILMember_IsMethod(member))
			{
				ILDumpMethodType(stderr, ILProgramItem_Image(classInfo),
								 ILMethod_Signature((ILMethod *)member), flags,
								 (flags ? 0 : classInfo),
								 ILMember_Name(member), (ILMethod *)member);
				fputs(" : unresolved method reference\n", stderr);
			}
			else if(ILMember_IsField(member))
			{
				ILDumpType(stderr, ILProgramItem_Image(classInfo),
						   ILFieldGetTypeWithPrefixes((ILField *)member),
						   flags);
				putc(' ', stderr);
				if(!flags)
				{
					ILDumpClassName(stderr, ILProgramItem_Image(classInfo),
									classInfo, 0);
					fputs("::", stderr);
				}
				fputs(ILMember_Name(member), stderr);
				fputs(" : unresolved field reference\n", stderr);
			}
			else
			{
				ILDumpClassName(stderr, ILProgramItem_Image(classInfo),
								classInfo, 0);
				fputs("::", stderr);
				fputs(ILMember_Name(member), stderr);
				fputs(" : unresolved member reference\n", stderr);
			}
			linker->error = 1;
		}
	}
}

int ILLinkerDestroy(ILLinker *linker)
{
	int result, index;
	ILLinkImage *image, *nextImage;

	/* Report any remaining unresolved references */
	ReportUnresolved(linker);

	/* Flush the metadata to the image writer */
	if(!(linker->outOfMemory) && !(linker->error))
	{
		ILWriterOutputMetadata(linker->writer, linker->image);
	}

	/* Destroy the global symbol pool for the linked image */
	ILHashDestroy(linker->symbolHash);
	ILMemPoolDestroy(&(linker->pool));

	/* Destroy the images */
	image = linker->images;
	while(image != 0)
	{
		nextImage = image->next;
		ILFree((void *)(image->filename));
		ILContextDestroy(image->context);
		ILFree(image);
		image = nextImage;
	}

	/* Destroy the temporary init/fini file */
	if(linker->initTempFile)
	{
		ILDeleteFile(linker->initTempFile);
	}

	/* Destroy the libraries */
	_ILLinkerDestroyLibraries(linker);

	/* Destroy the library directory list */
	for(index = 0; index < linker->numLibraryDirs; ++index)
	{
		ILFree(linker->libraryDirs[index]);
	}
	if(linker->libraryDirs)
	{
		ILFree(linker->libraryDirs);
	}

	/* Destroy the image writer and determine the result value */
	if(linker->outOfMemory)
	{
		result = -1;
	}
	else if(linker->error)
	{
		result = 0;
	}
	else
	{
		result = 1;
	}
	if(result > 0)
	{
		result = ILWriterDestroy(linker->writer);
	}
	else
	{
		ILWriterDestroy(linker->writer);
	}

	/* Destroy the image and context that we were building */
	ILContextDestroy(linker->context);

	/* Free the linker context */
	ILFree((void *)(linker->moduleName));
	ILFree(linker);

	/* Done */
	return result;
}

void _ILLinkerOutOfMemory(ILLinker *linker)
{
	linker->outOfMemory = 1;
}

/*
 * Convert a hex string into a byte array.
 */
static unsigned char *HexToBytes(ILLinker *linker, const char *str, int *len)
{
	unsigned char *bytes = 0;
	unsigned char *newBytes;
	int value = 0;
	int temp;
	int digit = 0;
	*len = 0;
	while(*str != '\0')
	{
		if(*str >= '0' && *str <= '9')
		{
			temp = (*str - '0');
		}
		else if(*str >= 'A' && *str <= 'F')
		{
			temp = (*str - 'A' + 10);
		}
		else if(*str >= 'a' && *str <= 'f')
		{
			temp = (*str - 'a' + 10);
		}
		else
		{
			++str;
			continue;
		}
		if(!digit)
		{
			value = temp;
			digit = 1;
		}
		else
		{
			value = value * 16 + temp;
			digit = 0;
			if((newBytes = (unsigned char *)ILRealloc(bytes, *len + 1)) == 0)
			{
				ILFree(bytes);
				_ILLinkerOutOfMemory(linker);
				*len = 0;
				return 0;
			}
			bytes = newBytes;
			bytes[(*len)++] = (unsigned char)value;
		}
		++str;
	}
	return bytes;
}

int ILLinkerCreateModuleAndAssembly(ILLinker *linker,
									const char *moduleName,
									const char *assemblyName,
									ILUInt16 *assemblyVersion,
									const char *assemblyKey,
									int hashAlgorithm)
{
	ILModule *module;
	ILAssembly *assembly;
	unsigned char *bytes;
	int lenBytes;
	char *name;

	/* Create the module */
	module = ILModuleCreate(linker->image, 0, moduleName, 0);
	if(!module)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}

	/* Create the assembly */
	assembly = ILAssemblyCreate(linker->image, 0, assemblyName, 0);
	if(!assembly)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}
	ILAssemblySetVersion(assembly, assemblyVersion);
	ILAssemblySetHashAlgorithm(assembly, hashAlgorithm);
	if(assemblyKey)
	{
		/* Recognise special builtin key values */
		if(!strcmp(assemblyKey, "neutral"))
		{
			assemblyKey = ILLinkerNeutralKey;
		}
		else if(!strcmp(assemblyKey, "ms"))
		{
			assemblyKey = ILLinkerMicrosoftKey;
		}

		/* Convert the key from hex into a byte array */
		bytes = HexToBytes(linker, assemblyKey, &lenBytes);
		if(bytes)
		{
			if(!ILAssemblySetOriginator
					(assembly, bytes, (ILUInt32)lenBytes))
			{
				_ILLinkerOutOfMemory(linker);
			}
			ILAssemblySetAttrs(assembly, IL_META_ASSEM_PUBLIC_KEY,
										 IL_META_ASSEM_PUBLIC_KEY);
			ILFree(bytes);
		}
	}

	/* Create the "<Module>" type, which holds global functions and variables */
	if(!ILClassCreate((ILProgramItem *)module, 0, "<Module>", 0, 0))
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}

	/* Set the C module name for library assemblies.  The module name
	   is the name of the assembly */
	if(!strcmp(linker->moduleName, IL_LINKER_DLL_MODULE_NAME))
	{
		lenBytes = strlen(assemblyName);
		name = (char *)malloc(lenBytes + 1);
		if(!name)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		strcpy(name, assemblyName);
		ILFree((void *)(linker->moduleName));
		linker->moduleName = name;
	}

	/* Ready to go */
	return 1;
}

void ILLinkerSetMetadataVersion(ILLinker *linker, const char *version,
								const char *stdLibrary)
{
	ILLibrary *library;
	if(version)
	{
		ILWriterSetVersionString(linker->writer, version);
	}
	else if((library = _ILLinkerFindLibrary(linker, stdLibrary)) != 0)
	{
		ILWriterInferVersionString(linker->writer, library->image);
	}
}

void ILLinkerSetCulture(ILLinker *linker, const char *culture)
{
	ILAssemblySetLocale
		(ILAssembly_FromToken(linker->image, IL_META_TOKEN_ASSEMBLY | 1),
		 culture);
}

int ILLinkerSetFlags(ILLinker *linker, int flags)
{
	int oldFlags = linker->linkerFlags;
	linker->linkerFlags = flags;
	return oldFlags;
}

int ILLinkerParseVersion(ILUInt16 *version, const char *str)
{
	int posn;
	ILUInt32 value;
	version[0] = 0;
	version[1] = 0;
	version[2] = 0;
	version[3] = 0;
	for(posn = 0; posn < 4; ++posn)
	{
		if(*str < '0' || *str > '9')
		{
			return 0;
		}
		value = (ILUInt32)(*str++ - '0');
		while(*str >= '0' && *str <= '9')
		{
			value = value * ((ILUInt32)10) + (ILUInt32)(*str++ - '0');
			if(value >= (ILUInt32)0x10000)
			{
				return 0;
			}
		}
		version[posn] = (ILUInt16)value;
		if(posn == 3)
		{
			break;
		}
		if(*str != ':' && *str != '.')
		{
			return 0;
		}
		++str;
	}
	return (*str == '\0');
}

#ifdef	__cplusplus
};
#endif
