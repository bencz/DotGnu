/*
 * meta_build.c - Build metadata structures from a metadata index.
 *
 * Copyright (C) 2001, 2002, 2003, 2008, 2009  Southern Storm Software, Pty Ltd.
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
#include <stdio.h>

#ifdef	__cplusplus
extern	"C" {
#endif

#define	EXIT_IF_ERROR(call)	\
			do { \
				error = (call); \
				if(error != 0) \
				{ \
					return error; \
				} \
			} while (0)

typedef int (*TokenLoadFunc)(ILImage *image, ILUInt32 *values,
			  			     ILUInt32 *valuesNext, ILToken token,
							 void *userData);

/*
 * Load all tokens in a particular table.
 */
static int LoadTokens(ILImage *image, ILToken tokenType,
					  TokenLoadFunc func, void *userData)
{
	ILToken maxToken;
	ILToken token;
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 valuesNext[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 *currValues;
	ILUInt32 *nextValues;
	ILUInt32 *tempValues;
	int error = 0;
	int newError;

	/* How many tokens of this type are there? */
	maxToken = (ILToken)(ILImageNumTokens(image, tokenType));
	if(!maxToken)
	{
		/* There are no tokens, so nothing needs to be done */
		return 0;
	}

	/* Parse and load the tokens */
	maxToken |= tokenType;
	if(!_ILImageRawTokenData(image, tokenType + 1, values))
	{
		return IL_LOADERR_BAD_META;
	}
	currValues = values;
	nextValues = valuesNext;
	for(token = tokenType + 1; token <= maxToken; ++token)
	{
		if((token + 1) <= maxToken)
		{
			if(!_ILImageRawTokenData(image, token + 1, nextValues))
			{
				return IL_LOADERR_BAD_META;
			}
			newError = (*func)(image, currValues, nextValues, token, userData);
			if(newError != 0)
			{
				if(error == 0)
				{
					error = newError;
				}
			}
			tempValues = currValues;
			currValues = nextValues;
			nextValues = tempValues;
		}
		else
		{
			newError = (*func)(image, currValues, 0, token, userData);
			if(newError != 0)
			{
				if(error == 0)
				{
					error = newError;
				}
			}
		}
	}

	/* Done */
	return error;
}

/*
 * Load all tokens in a particular range within a table.
 */
static int LoadTokenRange(ILImage *image, unsigned long tokenType,
					      unsigned long firstToken, unsigned long num,
						  TokenLoadFunc func, void *userData)
{
	unsigned long maxToken;
	unsigned long tableMaxToken;
	unsigned long token;
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 valuesNext[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 *currValues;
	ILUInt32 *nextValues;
	ILUInt32 *tempValues;
	int error;

	/* Bail out now if nothing to load */
	if(!num)
	{
		return 0;
	}

	/* Parse and load the tokens */
	maxToken = firstToken + num - 1;
	tableMaxToken = image->tokenCount[tokenType >> 24] | tokenType;
	if(!_ILImageRawTokenData(image, firstToken, values))
	{
		return IL_LOADERR_BAD_META;
	}
	currValues = values;
	nextValues = valuesNext;
	for(token = firstToken; token <= maxToken; ++token)
	{
		if((token + 1) <= tableMaxToken)
		{
			if(!_ILImageRawTokenData(image, token + 1, nextValues))
			{
				return IL_LOADERR_BAD_META;
			}
			error = (*func)(image, currValues, nextValues, token, userData);
			if(error != 0)
			{
				return error;
			}
			tempValues = currValues;
			currValues = nextValues;
			nextValues = tempValues;
		}
		else
		{
			error = (*func)(image, currValues, 0, token, userData);
			if(error != 0)
			{
				return error;
			}
		}
	}

	/* Done */
	return 0;
}

#if IL_DEBUG_META

/*
 * Report an error that occurred while attempting to resolve a type.
 */
static void ReportResolveError(ILImage *image, ILToken token,
							   const char *name, const char *namespace)
{
	const char *importName;
	const char *importName2;

	/* Try to print scope information when reporting the error */
	switch(token & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_MODULE_REF:
		{
			/* The type was imported from a module scope */
			ILModule *module = ILModule_FromToken(image, token);
			if(module)
			{
				importName = ILModuleGetName(module);
				fprintf(stderr, "unresolved type: [.module %s]%s%s%s\n",
						importName, (namespace ? namespace : ""),
						(namespace ? "." : ""), name);
				return;
			}
		}
		break;

		case IL_META_TOKEN_ASSEMBLY_REF:
		{
			/* The type was imported from an assembly scope */
			ILAssembly *assem = ILAssembly_FromToken(image, token);
			if(assem)
			{
				importName = ILAssemblyGetName(assem);
				fprintf(stderr, "unresolved type: [%s]%s%s%s\n",
						importName, (namespace ? namespace : ""),
						(namespace ? "." : ""), name);
				return;
			}
		}
		break;

		case IL_META_TOKEN_TYPE_REF:
		case IL_META_TOKEN_EXPORTED_TYPE:
		{
			/* The type was nested within another type */
			ILClass *classInfo = ILClass_FromToken(image, token);
			if(classInfo)
			{
				importName = ILClass_Name(classInfo);
				importName2 = ILClass_Namespace(classInfo);
				if(importName2 && importName2 == '\0')
				{
					importName2 = 0;
				}
				fprintf(stderr, "unresolved type: %s%s%s/%s%s%s\n",
						(importName2 ? importName2 : ""),
						(importName2 ? "." : ""), importName,
						(namespace ? namespace : ""),
						(namespace ? "." : ""), name);
				return;
			}
		}
		break;

		case IL_META_TOKEN_FILE:
		{
			/* The type was imported from a file scope */
			ILFileDecl *file = ILFileDecl_FromToken(image, token);
			if(file)
			{
				importName = ILFileDecl_Name(file);
				fprintf(stderr, "unresolved type: [.file %s]%s%s%s\n",
						importName, (namespace ? namespace : ""),
						(namespace ? "." : ""), name);
				return;
			}
		}
		break;
	}

	/* Leave off the scope */
	fprintf(stderr, "unresolved type: %s%s%s\n",
			(namespace ? namespace : ""), (namespace ? "." : ""), name);
}

#endif

/*
 * Determine if a scope refers to a module, or a parent
 * class in a nesting relationship that is also in a module.
 */
static int ScopeIsModule(ILProgramItem *scope)
{
	ILToken token;
	for(;;)
	{
		token = ILProgramItem_Token(scope);
		if((token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MODULE)
		{
			return 1;
		}
		else if((token & IL_META_TOKEN_MASK) != IL_META_TOKEN_TYPE_REF)
		{
			break;
		}
		scope = ILClassGetScope((ILClass *)scope);
	}
	return 0;
}

/*
 * Search the raw values in an owned item table for a token match.
 * Returns the first token that matches in the "ownedType" table
 * in "firstMatch".  The number of tokens that match is returned
 * from the function, or zero if there are no matches.
 */
ILUInt32 _ILSearchForRawToken(ILImage *image, ILSearchRawFunc func,
							  ILToken ownedType, ILToken *firstMatch,
							  ILToken searchFor, int valueField)
{
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	ILToken minToken, maxToken, current;
	ILUInt32 count;
	int cmp;

	/* Determine how to perform the search: binary or linear */
	minToken = (ownedType | 1);
	maxToken = (ownedType | image->tokenCount[ownedType >> 24]);
	if((image->sorted & (1 << (int)(ownedType >> 24))) != 0)
	{
		/* Perform a binary search for the item */
		while(minToken <= maxToken)
		{
			current = minToken + ((maxToken - minToken) / 2);
			if(!_ILImageRawTokenData(image, current, values))
			{
				return 0;
			}
			cmp = (*func)(values, searchFor, valueField);
			if(cmp == 0)
			{
				/* We have found a match: search backwards to find the
				   start of the range */
				minToken = (ownedType | 1);
				maxToken = (ownedType | image->tokenCount[ownedType >> 24]);
				while(current > minToken)
				{
					if(!_ILImageRawTokenData(image, current - 1, values))
					{
						return 0;
					}
					if((*func)(values, searchFor, valueField) != 0)
					{
						break;
					}
					--current;
				}

			   	/* This is the first match in the table.  Determine
				   how large the range of items is */
				*firstMatch = current;
				count = 1;
				++current;
				while(current <= maxToken)
				{
					if(!_ILImageRawTokenData(image, current, values))
					{
						return 0;
					}
					if((*func)(values, searchFor, valueField) != 0)
					{
						break;
					}
					++current;
					++count;
				}
				return count;
			}
			else if(cmp < 0)
			{
				maxToken = current - 1;
			}
			else
			{
				minToken = current + 1;
			}
		}
	}
	else
	{
		/* Perform a linear search for the item */
		for(current = minToken; current <= maxToken; ++current)
		{
			if(!_ILImageRawTokenData(image, current, values))
			{
				return 0;
			}
			if((*func)(values, searchFor, valueField) == 0)
			{
				/* This is the first match in the table.  Determine
				   how large the range of items is */
				*firstMatch = current;
				count = 1;
				++current;
				while(current <= maxToken)
				{
					if(!_ILImageRawTokenData(image, current, values))
					{
						return 0;
					}
					if((*func)(values, searchFor, valueField) != 0)
					{
						break;
					}
					++current;
					++count;
				}
				return count;
			}
		}
	}

	/* If we get here, then we were unable to find a match */
	return 0;
}

/*
 * Add a program item to the redo list.  Returns zero if out of memory.
 */
static int AddToRedoList(ILContext *context, ILProgramItem *item)
{
	ILProgramItem **items;
	if(context->numRedoItems >= context->maxRedoItems)
	{
		items = (ILProgramItem **)ILRealloc
			(context->redoItems,
			 sizeof(ILProgramItem *) * (context->maxRedoItems + 64));
		if(!items)
		{
			return 0;
		}
		context->redoItems = items;
		context->maxRedoItems += 64;
	}
	context->redoItems[(context->numRedoItems)++] = item;
	return 1;
}

/*
 * Determine if a program item is currently on the redo list.
 */
static int IsOnRedoList(ILContext *context, ILProgramItem *item)
{
	ILUInt32 posn;
	for(posn = 0; posn < context->numRedoItems; ++posn)
	{
		if(context->redoItems[posn] == item)
		{
			return 1;
		}
	}
	return 0;
}

/*
 * Resolve the TypeRef's.  Returns zero if OK, -1 if a
 * second-phase scan is needed for TypeRef's to this module.
 * Returns a load error otherwise.
 */
static int ResolveTypeRefs(ILImage *image, int loadFlags)
{
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	unsigned long maxToken;
	unsigned long token;
	const char *name;
	const char *namespace;
	void **table;
	int error;
	int needPhase2;
	ILProgramItem *scope;
	ILClass *info;
	ILClass *importInfo;
	ILImage *importImage;
	ILProgramItem *importScope;

	/* Create the TypeRef table for this image */
	maxToken = ILImageNumTokens(image, IL_META_TOKEN_TYPE_REF);
	if(!maxToken)
	{
		/* There are no TypeRef's, so nothing needs to be done */
		return 0;
	}
	if((table = (void **)ILCalloc(maxToken, sizeof(void *))) == 0)
	{
		/* Not enough memory to create the TypeRef table */
		return IL_LOADERR_MEMORY;
	}
	image->tokenData[IL_META_TOKEN_TYPE_REF >> 24] = table;

	/* Walk the TypeRef table to resolve external references */
	error = 0;
	needPhase2 = 0;
	maxToken |= IL_META_TOKEN_TYPE_REF;
	for(token = (IL_META_TOKEN_TYPE_REF | 1); token <= maxToken; ++token)
	{
		/* Parse the token information */
		if(!_ILImageRawTokenData(image, token, values))
		{
			error = IL_LOADERR_BAD_META;
			continue;
		}

		/* Get the TypeRef name and namespace */
		name = ILImageGetString(image, values[IL_OFFSET_TYPEREF_NAME]);
		namespace = ILImageGetString
						(image, values[IL_OFFSET_TYPEREF_NAMESPACE]);
		if(namespace && *namespace == '\0')
		{
			namespace = 0;
		}

		/* Determine which scope to use to resolve the TypeRef */
		if(values[IL_OFFSET_TYPEREF_SCOPE] != 0)
		{
			scope = ILProgramItem_FromToken
				(image, values[IL_OFFSET_TYPEREF_SCOPE]);
		}
		else
		{
			scope = ILClassGlobalScope(image);
		}
		if(!scope)
		{
			META_VAL_ERROR("invalid scope for type reference");
			error = IL_LOADERR_BAD_META;
			continue;
		}

		/* If we already have a reference, then reuse it */
		info = ILClassLookup(scope, name, namespace);
		if(info)
		{
			table[(token & ~IL_META_TOKEN_MASK) - 1] = (void *)info;
			continue;
		}

		/* Create a new type reference */
		info = ILClassCreateRef(scope, token, name, namespace);
		if(!info)
		{
			return IL_LOADERR_MEMORY;
		}
		table[(token & ~IL_META_TOKEN_MASK) - 1] = (void *)info;

		/* If the scope is the current module, then we need to
		   perform the rest of the resolution during phase 2 */
		if(ScopeIsModule(scope))
		{
			needPhase2 = 1;
			continue;
		}

		/* If the "no resolve" flag is set, then there is nothing more to do */
		if((loadFlags & IL_LOADFLAG_NO_RESOLVE) != 0)
		{
			continue;
		}

		/* Attempt to resolve references to other images */
		importInfo = 0;
		switch(ILProgramItem_Token(scope) & IL_META_TOKEN_MASK)
		{
			case IL_META_TOKEN_TYPE_REF:
			{
				/* Nested type within a foreign assembly */
				importScope = _ILProgramItemLinkedTo(scope);
				if(importScope)
				{
					importInfo = ILClassLookup
						(importScope, name, namespace);
				}
				else if(IsOnRedoList(image->context, scope))
				{
					/* The nesting parent is marked for redo,
					   so mark the child for redo also */
					if(!AddToRedoList(image->context, &(info->programItem)))
					{
						return IL_LOADERR_MEMORY;
					}
					continue;
				}
			}
			break;

			case IL_META_TOKEN_MODULE_REF:
			{
				/* Type is imported from a foreign module */
				importImage = ILModuleToImage((ILModule *)scope);
				if(importImage)
				{
					goto moduleImport;
				}
				if(!_ILImageDynamicLinkModule(image, image->filename,
											  ILModule_Name((ILModule *)scope),
											  loadFlags, &importImage) &&
				   importImage != 0)
				{
					importImage = ILModuleToImage((ILModule *)scope);
					if(importImage)
					{
						goto moduleImport;
					}
				}
			}
			break;

			case IL_META_TOKEN_ASSEMBLY_REF:
			{
				/* Type is imported from a foreign assembly */
				importImage = ILAssemblyToImage((ILAssembly *)scope);
				if(importImage)
				{
				moduleImport:
					if(importImage == image || !(importImage->loading))
					{
						importScope = ILClassGlobalScope(importImage);
						if(importScope)
						{
							importInfo = ILClassLookup
								(importScope, name, namespace);
							if(!importInfo && image->context->numRedoItems != 0)
							{
								/* This reference may need to be redone */
								goto redo;
							}
						}
					}
					else
					{
						/* We cannot resolve this TypeRef yet.  We queue it
						   on the "redo" list to be redone later */
					redo:
						if(!AddToRedoList(image->context, &(info->programItem)))
						{
							return IL_LOADERR_MEMORY;
						}
						continue;
					}
				}
			}
			break;
		}
		if(importInfo)
		{
			/* Link "info" to "importInfo" */
			if(!_ILProgramItemLink(&(info->programItem),
								   &(importInfo->programItem)))
			{
				return IL_LOADERR_MEMORY;
			}
		}
		else
		{
			/* Could not resolve the type */
		#if IL_DEBUG_META
			ReportResolveError(image, values[IL_OFFSET_TYPEREF_SCOPE],
							   name, namespace);
		#endif
			error = IL_LOADERR_UNRESOLVED;
		}
	}

	/* Done */
	return (error ? error : (needPhase2 ? -1 : 0));
}

/*
 * Resolve the TypeRef's using the phase 2 algorithm.
 * This is used to fix up references to the current module
 * after the TypeDef table has been loaded.
 */
static int ResolveTypeRefsPhase2(ILImage *image, int loadFlags)
{
	unsigned long maxToken;
	unsigned long token;
	void **table;
	int error;
	ILClass *info;
	ILClass *importInfo;
	ILProgramItem *scope;

	/* Get the TypeRef table for this image */
	table = image->tokenData[IL_META_TOKEN_TYPE_REF >> 24];

	/* Walk the TypeRef table to resolve same-module references */
	error = 0;
	maxToken = ILImageNumTokens(image, IL_META_TOKEN_TYPE_REF) |
			   IL_META_TOKEN_TYPE_REF;
	for(token = (IL_META_TOKEN_TYPE_REF | 1); token <= maxToken; ++token)
	{
		/* Skip this token if it does not have module scope */
		info = (ILClass *)(table[(token & ~IL_META_TOKEN_MASK) - 1]);
		scope = (info ? info->className->scope : 0);
		if(!info || !ScopeIsModule(scope))
		{
			continue;
		}

		/* If the type is no longer a reference, it has been fixed up */
		if(!ILClassIsRef(info))
		{
			continue;
		}

		/* If we get here, then we have a reference to the current
		   module for a type that doesn't exist in the current module.
		   The assembler uses this in object files to mark classes
		   that are imported from other object files, but not from
		   the system library.  The linker normally fixes these up,
		   but we may see them if we are loading an object file */

		/* If we aren't resolving references, then assume that this
		   is an object file and that the dangling reference is OK */
		if((loadFlags & IL_LOADFLAG_NO_RESOLVE) != 0)
		{
			continue;
		}

		/* If the class is linked, then we already resolved it,
		   but it is present twice in the file for some reason */
		if(_ILProgramItemLinkedTo(&(info->programItem)) != 0)
		{
			continue;
		}

		/* Is this a global or a nested class? */
		if(!ILClassIsNestingScope(scope))
		{
			/* Global class: look in any image for the type */
			importInfo = ILClassLookupGlobal(image->context,
											 info->className->name,
											 info->className->namespace);
		}
		else
		{
			/* Nested class: look within the parent scope */
			scope = _ILProgramItemLinkedTo(scope);
			if(scope)
			{
				importInfo = ILClassLookup(scope, info->className->name,
										   info->className->namespace);
			}
			else
			{
				importInfo = 0;
			}
		}

		/* Link "info" to "importInfo" */
		if(importInfo)
		{
			if(!_ILProgramItemLink(&(info->programItem),
								   &(importInfo->programItem)))
			{
				return IL_LOADERR_MEMORY;
			}
		}
		else
		{
		#if IL_DEBUG_META
			ReportResolveError(image, 0, info->className->name,
							   info->className->namespace);
		#endif
			error = IL_LOADERR_UNRESOLVED;
		}
	}

	/* Done */
	return error;
}

/*
 * Load a module token.
 */
static int Load_Module(ILImage *image, ILUInt32 *values,
					   ILUInt32 *valuesNext, ILToken token,
					   void *userData)
{
	const char *name;
	unsigned char *guidBase;
	ILUInt32 guidSize;
	ILModule *module;

	/* Unpack the token values */
	name = ILImageGetString(image, values[IL_OFFSET_MODULE_NAME]);
	guidBase = (unsigned char *)ILImageGetMetaEntry(image, "#GUID", &guidSize);

	/* Create the module structure */
	if(values[IL_OFFSET_MODULE_MVID] != IL_MAX_UINT32)
	{
		if((module = ILModuleCreate
				(image, token, name,
				 guidBase + values[IL_OFFSET_MODULE_MVID])) == 0)
		{
			return IL_LOADERR_MEMORY;
		}
	}
	else
	{
		if((module = ILModuleCreate(image, token, name, 0)) == 0)
		{
			return IL_LOADERR_MEMORY;
		}
	}

	/* If we have Edit & Continue information, then add it */
	if(values[IL_OFFSET_MODULE_GENERATION] != 0)
	{
		if(!ILModuleSetGeneration
					(module, values[IL_OFFSET_MODULE_GENERATION]))
		{
			return IL_LOADERR_MEMORY;
		}
	}
	if(values[IL_OFFSET_MODULE_ENCID] != IL_MAX_UINT32)
	{
		if(!ILModuleSetEncId
					(module, guidBase + values[IL_OFFSET_MODULE_ENCID]))
		{
			return IL_LOADERR_MEMORY;
		}
	}
	if(values[IL_OFFSET_MODULE_ENCBASEID] != IL_MAX_UINT32)
	{
		if(!ILModuleSetEncBaseId
					(module, guidBase + values[IL_OFFSET_MODULE_ENCBASEID]))
		{
			return IL_LOADERR_MEMORY;
		}
	}

	/* Done */
	return 0;
}

/*
 * Load a module reference token.
 */
static int Load_ModuleRef(ILImage *image, ILUInt32 *values,
						  ILUInt32 *valuesNext, ILToken token,
						  void *userData)
{
	const char *name;

	/* Unpack the token values */
	name = ILImageGetString(image, values[IL_OFFSET_MODULEREF_NAME]);

	/* Create the module reference structure */
	if(!ILModuleRefCreate(image, token, name))
	{
		return IL_LOADERR_MEMORY;
	}
	return 0;
}

/*
 * Load an assembly token.
 */
static int Load_Assembly(ILImage *image, ILUInt32 *values,
						 ILUInt32 *valuesNext, ILToken token,
						 void *userData)
{
	const char *name;
	ILAssembly *assem;

	/* Create the assembly structure */
	name = ILImageGetString(image, values[IL_OFFSET_ASSEMBLY_NAME]);
	assem = ILAssemblyCreate(image, token, name, 0);
	if(!assem)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Set the other assembly fields */
	ILAssemblySetHashAlgorithm(assem, values[IL_OFFSET_ASSEMBLY_HASHALG]);
	ILAssemblySetVersionSplit(assem,
							  values[IL_OFFSET_ASSEMBLY_VER_1],
							  values[IL_OFFSET_ASSEMBLY_VER_2],
							  values[IL_OFFSET_ASSEMBLY_VER_3],
							  values[IL_OFFSET_ASSEMBLY_VER_4]);
	ILAssemblySetAttrs(assem, ~0, values[IL_OFFSET_ASSEMBLY_ATTRS]);
	_ILAssemblySetOrigIndex(assem, values[IL_OFFSET_ASSEMBLY_KEY_RAW]);
	if(values[IL_OFFSET_ASSEMBLY_LOCALE])
	{
		name = ILImageGetString(image, values[IL_OFFSET_ASSEMBLY_LOCALE]);
		if(!ILAssemblySetLocale(assem, name))
		{
			return IL_LOADERR_MEMORY;
		}
	}

	/* Done */
	return 0;
}

/*
 * Load an assembly reference token.
 */
static int Load_AssemblyRef(ILImage *image, ILUInt32 *values,
							ILUInt32 *valuesNext, ILToken token,
							void *userData)
{
	const char *name;
	ILAssembly *assem;

	/* Create the assembly reference structure */
	name = ILImageGetString(image, values[IL_OFFSET_ASSEMBLYREF_NAME]);
	assem = ILAssemblyCreate(image, token, name, 1);
	if(!assem)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Set the other assembly reference fields */
	ILAssemblySetVersionSplit(assem,
							  values[IL_OFFSET_ASSEMBLYREF_VER_1],
							  values[IL_OFFSET_ASSEMBLYREF_VER_2],
							  values[IL_OFFSET_ASSEMBLYREF_VER_3],
							  values[IL_OFFSET_ASSEMBLYREF_VER_4]);
	ILAssemblySetAttrs(assem, ~0, values[IL_OFFSET_ASSEMBLYREF_ATTRS]);
	_ILAssemblySetOrigIndex(assem, values[IL_OFFSET_ASSEMBLYREF_KEY_RAW]);
	if(values[IL_OFFSET_ASSEMBLYREF_LOCALE])
	{
		name = ILImageGetString(image, values[IL_OFFSET_ASSEMBLYREF_LOCALE]);
		if(!ILAssemblySetLocale(assem, name))
		{
			return IL_LOADERR_MEMORY;
		}
	}
	_ILAssemblySetHashIndex(assem, values[IL_OFFSET_ASSEMBLYREF_HASH_RAW]);

	/* Done */
	return 0;
}

/*
 * Load an OS definition.
 */
static int Load_OSDef(ILImage *image, ILUInt32 *values,
					  ILUInt32 *valuesNext, ILToken token,
					  void *userData)
{
	if(!ILOSInfoCreate(image, token,
					   values[IL_OFFSET_OSDEF_IDENTIFIER],
					   values[IL_OFFSET_OSDEF_MAJOR],
					   values[IL_OFFSET_OSDEF_MINOR],
					   ILAssembly_FromToken
					   		(image, (IL_META_TOKEN_ASSEMBLY | 1))))
	{
		return IL_LOADERR_MEMORY;
	}
	else
	{
		return 0;
	}
}

/*
 * Load a processor definition.
 */
static int Load_ProcessorDef(ILImage *image, ILUInt32 *values,
							 ILUInt32 *valuesNext, ILToken token,
					  		 void *userData)
{
	if(!ILProcessorInfoCreate(image, token,
					  		  values[IL_OFFSET_PROCESSORDEF_NUM],
					   		  ILAssembly_FromToken
					   				(image, (IL_META_TOKEN_ASSEMBLY | 1))))
	{
		return IL_LOADERR_MEMORY;
	}
	else
	{
		return 0;
	}
}

/*
 * Load an OS reference.
 */
static int Load_OSRef(ILImage *image, ILUInt32 *values,
					  ILUInt32 *valuesNext, ILToken token,
					  void *userData)
{
	ILAssembly *ref;
	ref = ILAssembly_FromToken(image, values[IL_OFFSET_OSREF_ASSEMBLY]);
	if(!ILOSInfoCreate(image, token,
					   values[IL_OFFSET_OSREF_IDENTIFIER],
					   values[IL_OFFSET_OSREF_MAJOR],
					   values[IL_OFFSET_OSREF_MINOR], ref))
	{
		return IL_LOADERR_MEMORY;
	}
	else
	{
		return 0;
	}
}

/*
 * Load a processor reference.
 */
static int Load_ProcessorRef(ILImage *image, ILUInt32 *values,
							 ILUInt32 *valuesNext, ILToken token,
					  		 void *userData)
{
	ILAssembly *ref;
	ref = ILAssembly_FromToken(image, values[IL_OFFSET_OSREF_ASSEMBLY]);
	if(!ILProcessorInfoCreate(image, token,
					  		  values[IL_OFFSET_PROCESSORREF_NUM], ref))
	{
		return IL_LOADERR_MEMORY;
	}
	else
	{
		return 0;
	}
}

/*
 * Forward definition.
 */
static int Load_TypeDef(ILImage *image, ILUInt32 *values,
						ILUInt32 *valuesNext, ILToken token,
						void *userData);

/*
 * Load a reference to a type definition token that is
 * further along in the file from where we currently are.
 */
int LoadForwardTypeDef(ILImage *image, ILToken token)
{
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 valuesNext[IL_IMAGE_TOKEN_COLUMNS];
	if(!_ILImageRawTokenData(image, token, values))
	{
		return IL_LOADERR_BAD_META;
	}
	if((token & ~IL_META_TOKEN_MASK) <
			image->tokenCount[IL_META_TOKEN_TYPE_DEF >> 24])
	{
		if(!_ILImageRawTokenData(image, token + 1, valuesNext))
		{
			return IL_LOADERR_BAD_META;
		}
		return Load_TypeDef(image, values, valuesNext, token, 0);
	}
	else
	{
		return Load_TypeDef(image, values, 0, token, 0);
	}
}

/*
 * Find the parent of a nested class by searching the NestedClass table.
 */
static ILProgramItem *FindNestedParent(ILImage *image, ILToken child,
									   ILToken *parentToken)
{
	ILToken left, right, middle;
	ILToken count, parent;
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	ILProgramItem *item;

	count = (ILToken)(image->tokenCount[IL_META_TOKEN_NESTED_CLASS >> 24]);
	if(!count)
	{
		if(parentToken)
		{
			*parentToken = 0;
		}
		return 0;
	}
	parent = 0;
	if((image->sorted &
			(((ILUInt64)1) << (IL_META_TOKEN_NESTED_CLASS >> 24))) != 0)
	{
		/* The NestedClass table is sorted, so use a binary search */
		left = 0;
		right = count - 1;
		while(left <= right)
		{
			middle = (left + right) / 2;
			if(!_ILImageRawTokenData(image, IL_META_TOKEN_NESTED_CLASS +
											middle + 1, values))
			{
				break;
			}
			if(values[IL_OFFSET_NESTEDCLASS_CHILD] == child)
			{
				parent = values[IL_OFFSET_NESTEDCLASS_PARENT];
				break;
			}
			else if(values[IL_OFFSET_NESTEDCLASS_CHILD] < child)
			{
				left = middle + 1;
			}
			else
			{
				right = middle - 1;
			}
		}
	}
	else
	{
		/* The NestedClass table is not sorted, so use a linear search */
		for(left = 0; left < count; ++left)
		{
			if(!_ILImageRawTokenData(image, IL_META_TOKEN_NESTED_CLASS +
											left + 1, values))
			{
				break;
			}
			if(values[IL_OFFSET_NESTEDCLASS_CHILD] == child)
			{
				parent = values[IL_OFFSET_NESTEDCLASS_PARENT];
				break;
			}
		}
	}
	if(parent)
	{
		if(parentToken)
		{
			/* We want the token, but not the program item */
			*parentToken = parent;
		}
		else
		{
			item = ILProgramItem_FromToken(image, parent);
			if(item)
			{
				return item;
			}
		}
	}
	else if(parentToken)
	{
		*parentToken = 0;
	}
	return 0;
}

#if 0

/*
 * Load a type reference token.
 */
static int Load_TypeRef(ILImage *image, ILUInt32 *values,
						ILUInt32 *valuesNext, ILToken token,
						void *userData)
{
	const char *name;
	const char *namespace;
	ILProgramItem *scope;
	ILClass *info;
	ILClass *importInfo;
	ILImage *importImage;
	ILProgramItem *importScope;
	ILClassName *className;

	/* Get the TypeRef name and namespace */
	name = ILImageGetString(image, values[IL_OFFSET_TYPEREF_NAME]);
	namespace = ILImageGetString
					(image, values[IL_OFFSET_TYPEREF_NAMESPACE]);
	if(namespace && *namespace == '\0')
	{
		namespace = 0;
	}

	/* Determine which scope to use to resolve the TypeRef */
	if(values[IL_OFFSET_TYPEREF_SCOPE] != 0)
	{
		scope = ILProgramItem_FromToken
			(image, values[IL_OFFSET_TYPEREF_SCOPE]);
	}
	else
	{
		scope = ILClassGlobalScope(image);
	}
	if(!scope)
	{
		META_VAL_ERROR("invalid scope for type reference");
		return IL_LOADERR_BAD_META;
	}

	/* Look for the class name locally */
	className = _ILClassNameLookup(image, scope, 0, name, namespace);
	if(!className)
	{
		/* This shouldn't happen */
		META_VAL_ERROR("could not find the type reference's name");
		return IL_LOADERR_BAD_META;
	}

	/* Resolve the class name, being careful not to create a circularity
	   where the TypeRef tries to load itself */
	if(className->token != token)
	{
		info = _ILClassNameToClass(className);
		if(info)
		{
			if(!_ILImageSetToken(image, &(info->programItem), token,
						         IL_META_TOKEN_TYPE_REF))
			{
				return IL_LOADERR_MEMORY;
			}
			return 0;
		}
	}

	/* Create a new type reference */
	info = ILClassCreateRef(scope, token, name, namespace);
	if(!info)
	{
		return IL_LOADERR_MEMORY;
	}
	if(!_ILImageSetToken(image, &(info->programItem), token,
				         IL_META_TOKEN_TYPE_REF))
	{
		return IL_LOADERR_MEMORY;
	}

	/* If the "no resolve" flag is set, then there is nothing more to do */
	if((image->loadFlags & IL_LOADFLAG_NO_RESOLVE) != 0)
	{
		return 0;
	}

	/* If the scope is the current module, then we have encountered a
	   dangling TypeRef.  Search for it through-out the system */
	if(ScopeIsModule(scope))
	{
		/* Is this a global or a nested class? */
		if(!ILClassIsNestingScope(scope))
		{
			/* Global class: look in any image for the type */
			importInfo = ILClassLookupGlobal(image->context,
											 info->className->name,
											 info->className->namespace);
		}
		else
		{
			/* Nested class: look within the parent scope */
			scope = _ILProgramItemLinkedTo(scope);
			if(scope)
			{
				importInfo = ILClassLookup(scope, info->className->name,
										   info->className->namespace);
			}
			else
			{
				importInfo = 0;
			}
		}

		/* Link "info" to "importInfo" */
		if(importInfo)
		{
			if(!_ILProgramItemLink(&(info->programItem),
								   &(importInfo->programItem)))
			{
				return IL_LOADERR_MEMORY;
			}
		}
		else
		{
		#if IL_DEBUG_META
			ReportResolveError(image, 0, info->className->name,
							   info->className->namespace);
		#endif
			return IL_LOADERR_UNRESOLVED;
		}

		/* We have resolved the dangling TypeRef */
		return 0;
	}

	/* Attempt to resolve references to other images */
	importInfo = 0;
	switch(ILProgramItem_Token(scope) & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_TYPE_REF:
		{
			/* Nested type within a foreign assembly */
			importScope = _ILProgramItemLinkedTo(scope);
			if(importScope)
			{
				importInfo = ILClassLookup
					(importScope, name, namespace);
			}
		}
		break;

		case IL_META_TOKEN_MODULE_REF:
		{
			/* Module reference imports are not currently
			   supported.  Types should be imported from
			   assemblies, not modules.  Module references
			   should only be used for PInvoke imports */
		}
		break;

		case IL_META_TOKEN_ASSEMBLY_REF:
		{
			/* Type is imported from a foreign assembly */
			importImage = ILAssemblyToImage((ILAssembly *)scope);
			if(importImage)
			{
				importScope = ILClassGlobalScope(importImage);
				if(importScope)
				{
					importInfo = ILClassLookup
						(importScope, name, namespace);
				}
			}
		}
		break;
	}
	if(importInfo)
	{
		/* Link "info" to "importInfo" */
		if(!_ILProgramItemLink(&(info->programItem),
							   &(importInfo->programItem)))
		{
			return IL_LOADERR_MEMORY;
		}
	}
	else
	{
		/* Could not resolve the type */
	#if IL_DEBUG_META
		ReportResolveError(image, values[IL_OFFSET_TYPEREF_SCOPE],
						   name, namespace);
	#endif
		return IL_LOADERR_UNRESOLVED;
	}

	/* The type reference has been created */
	return 0;
}

#endif

/*
 * Load a type reference name.
 */
static int Load_TypeRefNameInner(ILImage *image, ILUInt32 *values,
						         ILUInt32 *valuesNext, ILToken token,
						         void *userData, ILClassName **nameInfo)
{
	const char *name;
	const char *namespace;
	ILProgramItem *scope;
	ILClassName *scopeName;
	ILClassName *className;
	ILToken scopeToken;
	int error;

	/* Get the TypeRef name and namespace */
	name = ILImageGetString(image, values[IL_OFFSET_TYPEREF_NAME]);
	namespace = ILImageGetString
					(image, values[IL_OFFSET_TYPEREF_NAMESPACE]);
	if(namespace && *namespace == '\0')
	{
		namespace = 0;
	}

	/* Determine which scope to use to resolve the TypeRef */
	scopeToken = values[IL_OFFSET_TYPEREF_SCOPE];
	if(scopeToken != 0)
	{
		if((scopeToken & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_REF)
		{
			/* This is a nested TypeRef: find the name of the nesting parent */
			if(!_ILImageRawTokenData(image, scopeToken, values))
			{
				return IL_LOADERR_BAD_META;
			}
			scope = 0;
			scopeName = 0;
			error = Load_TypeRefNameInner
				(image, values, 0, scopeToken, 0, &scopeName);
			if(error != 0)
			{
				return error;
			}
		}
		else
		{
			scope = ILProgramItem_FromToken(image, scopeToken);
			scopeName = 0;
		}
	}
	else
	{
		scope = ILClassGlobalScope(image);
		scopeName = 0;
	}
	if(!scope && !scopeName)
	{
		META_VAL_ERROR("invalid scope for type reference");
		return IL_LOADERR_BAD_META;
	}

	/* Bail out if we've already seen the name before */
	className = _ILClassNameLookup(image, scope, scopeName, name, namespace);
	if(className)
	{
		*nameInfo = className;
		return 0;
	}

	/* Create the class name information */
	if(!_ILClassNameCreate(image, token, name, namespace, scope, scopeName))
	{
		return IL_LOADERR_MEMORY;
	}
	*nameInfo = className;
	return 0;
}
static int Load_TypeRefName(ILImage *image, ILUInt32 *values,
						    ILUInt32 *valuesNext, ILToken token,
						    void *userData)
{
	ILClassName *name;
	return Load_TypeRefNameInner(image, values, valuesNext, token,
								 userData, &name);
}

/*
 * Load a type definition name.
 */
static int Load_TypeDefNameInner(ILImage *image, ILUInt32 *values,
						    	 ILUInt32 *valuesNext, ILToken token,
						    	 void *userData, ILClassName **nameInfo)
{
	const char *name;
	const char *namespace;
	ILProgramItem *scope;
	ILClassName *scopeName;
	ILClassName *className;
	ILToken parentToken;
	int error;

	/* Get the name and namespace for the type */
	name = ILImageGetString(image, values[IL_OFFSET_TYPEDEF_NAME]);
	namespace = ILImageGetString(image, values[IL_OFFSET_TYPEDEF_NAMESPACE]);
	if(namespace && *namespace == '\0')
	{
		namespace = 0;
	}

	/* Determine the scope to use to define the type */
	if((values[IL_OFFSET_TYPEDEF_ATTRS] & IL_META_TYPEDEF_VISIBILITY_MASK)
			< IL_META_TYPEDEF_NESTED_PUBLIC)
	{
		/* Global scope */
		scope = ILClassGlobalScope(image);
		scopeName = 0;
	}
	else
	{
		/* Nested scope */
		FindNestedParent(image, token, &parentToken);
		if(parentToken)
		{
			if(!_ILImageRawTokenData(image, parentToken, values))
			{
				return IL_LOADERR_BAD_META;
			}
			scope = 0;
			scopeName = 0;
			error = Load_TypeDefNameInner
				(image, values, 0, parentToken, 0, &scopeName);
			if(error != 0)
			{
				return error;
			}
		}
		else
		{
			scope = 0;
			scopeName = 0;
		}
	}

	/* If we don't have a scope, then exit with an error */
	if(!scope && !scopeName)
	{
		META_VAL_ERROR("unknown type definition scope");
		return IL_LOADERR_BAD_META;
	}

	/* Bail out if we've already seen the name before (i.e. there is
	   both a TypeRef and a TypeDef for the same name, which is OK) */
	className = _ILClassNameLookup(image, scope, scopeName, name, namespace);
	if(className)
	{
		className->token = token;	/* Point the name at the TypeDef */
		*nameInfo = className;
		return 0;
	}

	/* Create the class name information */
	if(!_ILClassNameCreate(image, token, name, namespace, scope, scopeName))
	{
		return IL_LOADERR_MEMORY;
	}
	*nameInfo = className;
	return 0;
}
static int Load_TypeDefName(ILImage *image, ILUInt32 *values,
						    ILUInt32 *valuesNext, ILToken token,
						    void *userData)
{
	ILClassName *name;
	return Load_TypeDefNameInner(image, values, valuesNext, token,
								 userData, &name);
}

/*
 * Load a type specification token.
 */
static int Load_TypeSpec(ILImage *image, ILUInt32 *values,
						 ILUInt32 *valuesNext, ILToken token,
						 void *userData)
{
	ILType *type;
	ILTypeSpec *spec;

	/* Parse the type specification blob */
	type = ILTypeFromTypeSpec(image->context, image,
						      values[IL_OFFSET_TYPESPEC_TYPE],
						      values[IL_OFFSET_TYPESPEC_TYPE_LEN]);
	if(!type)
	{
		META_VAL_ERROR("invalid type specification");
		return IL_LOADERR_BAD_META;
	}

	/* Create a TypeSpec structure and attach it to the image */
	spec = ILTypeSpecCreate(image, token, type);
	if(!spec)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Set the type blob index in case this image is later written */
	_ILTypeSpecSetTypeIndex(spec, values[IL_OFFSET_TYPESPEC_TYPE_RAW]);

	/* Done */
	return 0;
}

/*
 * Search for a PInvoke token in a raw token table.
 */
static int Search_PInvoke(ILUInt32 *values, ILToken token1, int valueField)
{
	ILToken token2 = (ILToken)(values[IL_OFFSET_IMPLMAP_METHOD]);
	ILToken tokenNum1 = (token1 & ~IL_META_TOKEN_MASK);
	ILToken tokenNum2 = (token2 & ~IL_META_TOKEN_MASK);

	/* Compare the bottom parts of the token first, because
	   the table must be sorted on its encoded value, not
	   on the original value.  Encoded values put the token
	   type in the low order bits */
	if(tokenNum1 < tokenNum2)
	{
		return -1;
	}
	else if(tokenNum1 > tokenNum2)
	{
		return 1;
	}
	else if(token1 < token2)
	{
		return -1;
	}
	else if(token1 > token2)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Fetch a PInvoke declaration for a method or field token.
 */
static void FetchPInvokeForToken(ILImage *image, ILToken searchFor)
{
	ILToken token;

	/* Search the ImplMap table for a matching PInvoke declaration */
	if(_ILSearchForRawToken(image, Search_PInvoke, IL_META_TOKEN_IMPL_MAP,
						    &token, searchFor, 0) == 0)
	{
		return;
	}

	/* Load the PInvoke token information on-demand.  This will connect
	   it up to its owning method or field automatically */
	ILPInvoke_FromToken(image, token);
}

/*
 * Search comparison function for "SearchForTypeDef".
 */
static int Search_TypeDef(ILUInt32 *values, ILUInt32 *nextValues,
						  ILToken searchFor, int isField)
{
	ILToken first, last;
	if(nextValues)
	{
		if(isField)
		{
			first = values[IL_OFFSET_TYPEDEF_FIRST_FIELD];
			last = nextValues[IL_OFFSET_TYPEDEF_FIRST_FIELD];
		}
		else
		{
			first = values[IL_OFFSET_TYPEDEF_FIRST_METHOD];
			last = nextValues[IL_OFFSET_TYPEDEF_FIRST_METHOD];
		}
	}
	else
	{
		if(isField)
		{
			first = values[IL_OFFSET_TYPEDEF_FIRST_FIELD];
			last = IL_MAX_UINT32;
		}
		else
		{
			first = values[IL_OFFSET_TYPEDEF_FIRST_METHOD];
			last = IL_MAX_UINT32;
		}
	}
	if(searchFor < first)
	{
		return -1;
	}
	else if(searchFor < last)
	{
		return 0;
	}
	else
	{
		return 1;
	}
}

/*
 * Find the TypeDef associated with a field or method.
 */
static ILToken SearchForTypeDef(ILImage *image, ILToken searchFor, int isField)
{
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 nextValues[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 *nextPtr;
	ILToken minToken, maxToken, current;
	int cmp;

	/* Perform a binary search for the TypeDef: the FIRST_FIELD and
	   FIRST_METHOD columns are assumed to be in ascending order */
	minToken = (IL_META_TOKEN_TYPE_DEF | 1);
	maxToken = (IL_META_TOKEN_TYPE_DEF |
				image->tokenCount[IL_META_TOKEN_TYPE_DEF >> 24]);
	while(minToken <= maxToken)
	{
		current = minToken + ((maxToken - minToken) / 2);
		if(!_ILImageRawTokenData(image, current, values))
		{
			return 0;
		}
		if((current + 1) <= maxToken)
		{
			if(!_ILImageRawTokenData(image, current + 1, nextValues))
			{
				return 0;
			}
			nextPtr = nextValues;
		}
		else
		{
			nextPtr = 0;
		}
		cmp = Search_TypeDef(values, nextPtr, searchFor, isField);
		if(cmp == 0)
		{
			/* We have found a match */
			return current;
		}
		else if(cmp < 0)
		{
			maxToken = current - 1;
		}
		else
		{
			minToken = current + 1;
		}
	}

	/* If we get here, then we were unable to find a match */
	return 0;
}

/*
 * Load a field definition token.
 */
static int Load_FieldDef(ILImage *image, ILUInt32 *values,
						 ILUInt32 *valuesNext, ILToken token,
						 void *userData)
{
	ILField *field;

	/* Create the field and attach it to the class */
	field = ILFieldCreate((ILClass *)userData, token,
				  ILImageGetString(image, values[IL_OFFSET_FIELDDEF_NAME]),
				  values[IL_OFFSET_FIELDDEF_ATTRS]);
	if(!field)
	{
		return IL_LOADERR_MEMORY;
	}

	/* We must have a signature */
	if(!(values[IL_OFFSET_FIELDDEF_SIGNATURE]))
	{
		META_VAL_ERROR("missing signature");
		return IL_LOADERR_BAD_META;
	}

	/* Parse the signature blob */
	ILMemberSetSignature((ILMember *)field,
		ILTypeFromFieldSig(image->context, image,
						   values[IL_OFFSET_FIELDDEF_SIGNATURE],
						   values[IL_OFFSET_FIELDDEF_SIGNATURE_LEN]));
	if(!(ILMemberGetSignature((ILMember *)field)))
	{
		META_VAL_ERROR("invalid field signature");
		return IL_LOADERR_BAD_META;
	}
	_ILMemberSetSignatureIndex((ILMember *)field,
							   values[IL_OFFSET_FIELDDEF_SIGNATURE_RAW]);

	/* Fetch the PInvoke information if necessary */
	if(ILField_HasPInvokeImpl(field))
	{
		FetchPInvokeForToken(image, token);
	}

	/* Done */
	return 0;
}

/*
 * Load a field definition on-demand by loading its type.
 */
static int Load_FieldDefOnDemand(ILImage *image, ILUInt32 *values,
						         ILUInt32 *valuesNext, ILToken token,
						         void *userData)
{
	ILToken typeDef;
	ILClass *classInfo;

	/* Search the TypeDef table for the field token */
	typeDef = SearchForTypeDef(image, token, 1);
	if(!typeDef)
	{
		META_VAL_ERROR("could not find type for field definition");
		return IL_LOADERR_BAD_META;
	}

	/* Load the type */
	classInfo = ILClass_FromToken(image, typeDef);
	if(!classInfo)
	{
		META_VAL_ERROR("failed to load type for field definition");
		return IL_LOADERR_BAD_META;
	}

	/* Done */
	return 0;
}

/*
 * Load a parameter definition token.
 */
static int Load_ParamDef(ILImage *image, ILUInt32 *values,
						 ILUInt32 *valuesNext, ILToken token,
						 void *userData)
{
	const char *name = ILImageGetString(image, values[IL_OFFSET_PARAMDEF_NAME]);
	if(!ILParameterCreate((ILMethod *)userData, token, name,
						  values[IL_OFFSET_PARAMDEF_ATTRS],
						  values[IL_OFFSET_PARAMDEF_NUMBER]))
	{
		return IL_LOADERR_MEMORY;
	}
	else
	{
		return 0;
	}
}

/*
 * Determine the number of items in a token range.
 */
static int SizeOfRange(ILImage *image, unsigned long tokenKind,
					   ILUInt32 *values, ILUInt32 *valuesNext,
					   int index, ILUInt32 *num)
{
	if(!(values[index]))
	{
		*num = 0;
		return 1;
	}
	else if(valuesNext && valuesNext[index] != 0)
	{
		if(valuesNext[index] < values[index])
		{
			return 0;
		}
		*num = valuesNext[index] - values[index];
		return 1;
	}
	else
	{
		*num = (image->tokenCount[tokenKind >> 24] + 1) -
			   (values[index] & ~IL_META_TOKEN_MASK);
		return 1;
	}
}

/*
 * Load the parameter definitions for a method token on demand.
 */
void _ILMethodLoadParams(ILMethod *method)
{
	ILImage *image = method->member.programItem.image;
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 valuesNext[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 *valuesNextPtr;
	ILUInt32 num;

	/* Bail out if we are building the image or this isn't a MethodDef */
	if(image->type == IL_IMAGETYPE_BUILDING)
	{
		return;
	}
	if((method->member.programItem.token & IL_META_TOKEN_MASK)
			!= IL_META_TOKEN_METHOD_DEF)
	{
		return;
	}

	/* Fetch the MethodDef details for this method and the next one */
	if(!_ILImageRawTokenData(image, method->member.programItem.token, values))
	{
		return;
	}
	if((method->member.programItem.token & ~IL_META_TOKEN_MASK) <
			image->tokenCount[IL_META_TOKEN_METHOD_DEF >> 24])
	{
		if(!_ILImageRawTokenData(image, method->member.programItem.token + 1,
								 valuesNext))
		{
			return;
		}
		valuesNextPtr = valuesNext;
	}
	else
	{
		valuesNextPtr = 0;
	}

	/* Parse the parameter definitions */
	if(!SizeOfRange(image, IL_META_TOKEN_PARAM_DEF,
					values, valuesNextPtr,
					IL_OFFSET_METHODDEF_FIRST_PARAM, &num))
	{
		return;
	}
	LoadTokenRange(image, IL_META_TOKEN_PARAM_DEF,
				   values[IL_OFFSET_METHODDEF_FIRST_PARAM], num,
				   Load_ParamDef, method);
}

/*
 * Load a method definition token.
 */
static int Load_MethodDef(ILImage *image, ILUInt32 *values,
						  ILUInt32 *valuesNext, ILToken token,
						  void *userData)
{
	ILMethod *method;
	ILType *signature;

	/* Create the method and attach it to the class */
	method = ILMethodCreate((ILClass *)userData, token,
				  ILImageGetString(image, values[IL_OFFSET_METHODDEF_NAME]),
				  values[IL_OFFSET_METHODDEF_ATTRS]);
	if(!method)
	{
		return IL_LOADERR_MEMORY;
	}

	/* We must have a signature */
	if(!(values[IL_OFFSET_METHODDEF_SIGNATURE]))
	{
		META_VAL_ERROR("missing signature");
		return IL_LOADERR_BAD_META;
	}

	/* Parse the signature blob */
	signature =
		ILTypeFromMethodDefSig(image->context, image,
						       values[IL_OFFSET_METHODDEF_SIGNATURE],
						       values[IL_OFFSET_METHODDEF_SIGNATURE_LEN]);
	if(!signature)
	{
		META_VAL_ERROR("invalid method signature");
		return IL_LOADERR_BAD_META;
	}
	ILMemberSetSignature((ILMember *)method, signature);
	_ILMemberSetSignatureIndex((ILMember *)method,
							   values[IL_OFFSET_METHODDEF_SIGNATURE_RAW]);

	/* Set the other method fields */
	ILMethodSetImplAttrs(method, ~0,
		(values[IL_OFFSET_METHODDEF_IMPL_ATTRS] &
			~(IL_META_METHODIMPL_JAVA_FP_STRICT |
			  IL_META_METHODIMPL_JAVA)));
	ILMethodSetCallConv(method, ILType_CallConv(signature));
	ILMethodSetRVA(method, values[IL_OFFSET_METHODDEF_RVA]);

	/* Fetch the PInvoke information if necessary */
	if(ILMethod_HasPInvokeImpl(method))
	{
		FetchPInvokeForToken(image, token);
	}

	/* Done */
	return 0;
}

/*
 * Load a method definition on-demand by loading its type.
 */
static int Load_MethodDefOnDemand(ILImage *image, ILUInt32 *values,
						          ILUInt32 *valuesNext, ILToken token,
						          void *userData)
{
	ILToken typeDef;
	ILClass *classInfo;

	/* Search the TypeDef table for the method token */
	typeDef = SearchForTypeDef(image, token, 0);
	if(!typeDef)
	{
		META_VAL_ERROR("could not find type for method definition");
		return IL_LOADERR_BAD_META;
	}

	/* Load the type */
	classInfo = ILClass_FromToken(image, typeDef);
	if(!classInfo)
	{
		META_VAL_ERROR("failed to load type for method definition");
		return IL_LOADERR_BAD_META;
	}

	/* Done */
	return 0;
}

/*
 * Load the fields and methods for a type definition token.
 */
static int Load_TypeDefPhase2(ILImage *image, ILUInt32 *values,
						      ILUInt32 *valuesNext, ILToken token,
						      void *userData)
{
	ILClass *info;
	ILUInt32 num;
	int error;

	/* Get the class information block for the token */
	info = ILClass_FromToken(image, token);

	/* If the class has a nested attribute, but it is still
	   at the top-most level, then complain */
	if(ILClassGetNestedParent(info) == 0 &&
	   (ILClass_IsNestedPublic(info) ||
	    ILClass_IsNestedPrivate(info) ||
		ILClass_IsNestedFamily(info) ||
		ILClass_IsNestedAssembly(info) ||
		ILClass_IsNestedFamAndAssem(info) ||
		ILClass_IsNestedFamOrAssem(info)))
	{
		META_VAL_ERROR("nested class at outer scope");
		return IL_LOADERR_BAD_META;
	}

	/* Load the fields */
	if(!SizeOfRange(image, IL_META_TOKEN_FIELD_DEF,
					values, valuesNext, IL_OFFSET_TYPEDEF_FIRST_FIELD, &num))
	{
		META_VAL_ERROR("invalid field count");
		return IL_LOADERR_BAD_META;
	}
	EXIT_IF_ERROR(LoadTokenRange(image, IL_META_TOKEN_FIELD_DEF,
								 values[IL_OFFSET_TYPEDEF_FIRST_FIELD], num,
								 Load_FieldDef, info));

	/* Load the methods */
	if(!SizeOfRange(image, IL_META_TOKEN_METHOD_DEF,
					values, valuesNext, IL_OFFSET_TYPEDEF_FIRST_METHOD, &num))
	{
		META_VAL_ERROR("invalid method count");
		return IL_LOADERR_BAD_META;
	}
	EXIT_IF_ERROR(LoadTokenRange(image, IL_META_TOKEN_METHOD_DEF,
								 values[IL_OFFSET_TYPEDEF_FIRST_METHOD], num,
								 Load_MethodDef, info));

	/* Done: we'll get the events, properties, and interfaces later */
	return 0;
}

/*
 * Load an interface implementation token.
 */
static int Load_InterfaceImpl(ILImage *image, ILUInt32 *values,
							  ILUInt32 *valuesNext, ILToken token,
							  void *userData)
{
	ILClass *info;
	ILProgramItem *item;
	ILProgramItem *interface;
	ILClass *interfaceClass;
#if IL_VERSION_MAJOR > 1
	ILTypeSpec *spec;
#endif

	/* Get the type that is implementing the interface */
	info = ILClass_FromToken(image, values[IL_OFFSET_INTERFACE_TYPE]);
	if(!info)
	{
		META_VAL_ERROR("unknown type");
		return IL_LOADERR_BAD_META;
	}

	/* Get the interface type */
	item = ILProgramItem_FromToken(image, values[IL_OFFSET_INTERFACE_INTERFACE]);
	if(!item)
	{
		META_VAL_ERROR("invalid interface token");
		return IL_LOADERR_BAD_META;
	}
#if IL_VERSION_MAJOR > 1
	if((spec = _ILProgramItem_ToTypeSpec(item)))
	{
		interface = ILToProgramItem(spec);
	}
	else
#endif
	if((interfaceClass = _ILProgramItem_ToClass(item)))
	{
		interface = ILToProgramItem(interfaceClass);
	}
	else
	{
		/* Invalid interface token type */
		interface = 0;
	}
	if(!interface)
	{
		META_VAL_ERROR("unknown interface");
		return IL_LOADERR_BAD_META;
	}

	/* Add the "implements" clause to the class */
	if(!ILClassAddImplements(info, interface, token))
	{
		return IL_LOADERR_MEMORY;
	}

	/* Done */
	return 0;
}

/*
 * Load a PInvoke token.
 */
static int Load_PInvoke(ILImage *image, ILUInt32 *values,
						ILUInt32 *valuesNext, ILToken token,
						void *userData)
{
	ILMethod *method;
	ILField *field;
	ILModule *module;
	ILPInvoke *pinvoke;

	/* An early version supported PInvoke for fields.  That is now obsolete,
	   but we have revived it so that variables can be imported from DLL's */
	if((values[IL_OFFSET_IMPLMAP_METHOD] & IL_META_TOKEN_MASK)
				!= IL_META_TOKEN_METHOD_DEF &&
	   (values[IL_OFFSET_IMPLMAP_METHOD] & IL_META_TOKEN_MASK)
				!= IL_META_TOKEN_FIELD_DEF)
	{
		META_VAL_ERROR("pinvoke must be applied to a method or field");
		return IL_LOADERR_BAD_META;
	}

	/* Validate that the method or field is really PInvoke */
	if((values[IL_OFFSET_IMPLMAP_METHOD] & IL_META_TOKEN_MASK)
				== IL_META_TOKEN_METHOD_DEF)
	{
		method = ILMethod_FromToken(image, values[IL_OFFSET_IMPLMAP_METHOD]);
		if(!method || !ILMethod_HasPInvokeImpl(method))
		{
			META_VAL_ERROR("pinvoke token applied to a non-pinvoke method");
			return IL_LOADERR_BAD_META;
		}
		field = 0;
	}
	else
	{
		field = ILField_FromToken(image, values[IL_OFFSET_IMPLMAP_METHOD]);
		if(!field || !ILField_HasPInvokeImpl(field))
		{
			META_VAL_ERROR("pinvoke token applied to a non-pinvoke field");
			return IL_LOADERR_BAD_META;
		}
		method = 0;
	}

	/* Find the module that the function is being imported from */
	module = ILModule_FromToken(image, values[IL_OFFSET_IMPLMAP_MODULE]);
	if(!module)
	{
		META_VAL_ERROR("invalid module for pinvoke import");
		return IL_LOADERR_BAD_META;
	}

	/* Create the PInvoke record */
	if(method)
	{
		pinvoke = ILPInvokeCreate
				(method, token,
				 values[IL_OFFSET_IMPLMAP_ATTRS], module,
				 ILImageGetString(image, values[IL_OFFSET_IMPLMAP_ALIAS]));
	}
	else
	{
		pinvoke = ILPInvokeFieldCreate
				(field, token,
				 values[IL_OFFSET_IMPLMAP_ATTRS], module,
				 ILImageGetString(image, values[IL_OFFSET_IMPLMAP_ALIAS]));
	}
	if(!pinvoke)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Done */
	return 0;
}

/*
 * Load an override declaration token.
 */
static int Load_Override(ILImage *image, ILUInt32 *values,
						 ILUInt32 *valuesNext, ILToken token,
						 void *userData)
{
	ILClass *classInfo;
	ILProgramItem *decl;
	ILProgramItem *body;

	/* Locate the class that contains the method declaration */
	classInfo = ILClass_FromToken
					(image, values[IL_OFFSET_METHODIMPL_TYPE]);
	if(!classInfo)
	{
		META_VAL_ERROR("invalid class in override token");
		return IL_LOADERR_BAD_META;
	}

	/* Locate the override method declaration */
	decl = ILProgramItem_FromToken
					(image, values[IL_OFFSET_METHODIMPL_METHOD_2]);
	if(!ILProgramItemToMethod(decl))
	{
		META_VAL_ERROR("invalid method declaration in override token");
		return IL_LOADERR_BAD_META;
	}

	/* Locate the override method body */
	body = ILProgramItem_FromToken
					(image, values[IL_OFFSET_METHODIMPL_METHOD_1]);
	if(!ILProgramItemToMethod(body))
	{
		META_VAL_ERROR("invalid method body in override token");
		return IL_LOADERR_BAD_META;
	}

	/* Create the PInvoke record */
	if(!ILOverrideCreate(classInfo, token, (ILMethod *)decl, (ILMethod *)body))
	{
		return IL_LOADERR_MEMORY;
	}

	/* Done */
	return 0;
}

/*
 * Load an Event token.
 */
static int Load_Event(ILImage *image, ILUInt32 *values,
					  ILUInt32 *valuesNext, ILToken token,
					  void *userData)
{
	ILClass *type;

	/* Get the event's type */
	if(values[IL_OFFSET_EVENT_TYPE])
	{
		type = ILClass_FromToken(image, values[IL_OFFSET_EVENT_TYPE]);
		if(type)
		{
			/* Resolve TypeSpec's into ILClass structures */
			type = ILProgramItemToClass((ILProgramItem *)type);
		}
		if(!type)
		{
			META_VAL_ERROR("unknown event type");
			return IL_LOADERR_BAD_META;
		}
	}
	else
	{
		type = 0;
	}

	/* Create the event */
	if(!ILEventCreate((ILClass *)userData, token,
					  ILImageGetString
					  		(image, values[IL_OFFSET_EVENT_NAME]),
					  values[IL_OFFSET_EVENT_ATTRS], type))
	{
		return IL_LOADERR_MEMORY;
	}
	else
	{
		return 0;
	}
}

/*
 * Load an EventAssociation token, and the asssociated event.
 */
static int Load_EventAssociation(ILImage *image, ILUInt32 *values,
								 ILUInt32 *valuesNext, ILToken token,
								 void *userData)
{
	ILClass *info;
	ILUInt32 num;
	int error;

	/* Find the type that owns the event */
	info = ILClass_FromToken(image, values[IL_OFFSET_EVENTMAP_TYPE]);
	if(!info)
	{
		META_VAL_ERROR("unknown owning type");
		return IL_LOADERR_BAD_META;
	}

	/* Load the events that are covered by this association */
	if(!SizeOfRange(image, IL_META_TOKEN_EVENT,
					values, valuesNext, IL_OFFSET_EVENTMAP_EVENT, &num))
	{
		META_VAL_ERROR("invalid event count");
		return IL_LOADERR_BAD_META;
	}
	EXIT_IF_ERROR(LoadTokenRange(image, IL_META_TOKEN_EVENT,
								 values[IL_OFFSET_EVENTMAP_EVENT], num,
								 Load_Event, info));

	/* Done */
	return 0;
}

/*
 * Load a Property token.
 */
static int Load_Property(ILImage *image, ILUInt32 *values,
						 ILUInt32 *valuesNext, ILToken token,
						 void *userData)
{
	ILType *signature;
	ILProperty *property;

	/* Get the property's signature */
	signature = ILTypeFromPropertySig
						(image->context, image,
						 values[IL_OFFSET_PROPERTY_SIGNATURE],
						 values[IL_OFFSET_PROPERTY_SIGNATURE_LEN]);
	if(!signature)
	{
		META_VAL_ERROR("invalid property signature");
		return IL_LOADERR_BAD_META;
	}

	/* Create the property */
	property = ILPropertyCreate((ILClass *)userData, token,
					     ILImageGetString
					  		(image, values[IL_OFFSET_PROPERTY_NAME]),
					     values[IL_OFFSET_PROPERTY_ATTRS],
						 signature);
	if(!signature)
	{
		return IL_LOADERR_MEMORY;
	}
	_ILMemberSetSignatureIndex((ILMember *)property,
							   values[IL_OFFSET_PROPERTY_SIGNATURE_RAW]);

	/* Done */
	return 0;
}

/*
 * Load a PropertyAssociation token, and the asssociated properties.
 */
static int Load_PropertyAssociation(ILImage *image, ILUInt32 *values,
								    ILUInt32 *valuesNext, ILToken token,
								    void *userData)
{
	ILClass *info;
	ILUInt32 num;
	int error;

	/* Find the type that owns the property */
	info = ILClass_FromToken(image, values[IL_OFFSET_PROPMAP_TYPE]);
	if(!info)
	{
		META_VAL_ERROR("unknown owning type");
		return IL_LOADERR_BAD_META;
	}

	/* Load the properties that are covered by this association */
	if(!SizeOfRange(image, IL_META_TOKEN_PROPERTY,
					values, valuesNext, IL_OFFSET_PROPMAP_PROPERTY, &num))
	{
		META_VAL_ERROR("invalid property count");
		return IL_LOADERR_BAD_META;
	}
	EXIT_IF_ERROR(LoadTokenRange(image, IL_META_TOKEN_PROPERTY,
								 values[IL_OFFSET_PROPMAP_PROPERTY], num,
								 Load_Property, info));

	/* Done */
	return 0;
}

/*
 * Load a MethodAssociation token.
 */
static int Load_MethodAssociation(ILImage *image, ILUInt32 *values,
								  ILUInt32 *valuesNext, ILToken token,
								  void *userData)
{
	ILMethod *method;
	ILProgramItem *owner;
	ILProperty *property;

	/* Find the method */
	method = ILMethod_FromToken(image, values[IL_OFFSET_METHODSEM_METHOD]);
	if(!method)
	{
		META_VAL_ERROR("unknown method reference");
		return IL_LOADERR_BAD_META;
	}

	/* Get the method semantics owner */
	owner = ILProgramItem_FromToken(image, values[IL_OFFSET_METHODSEM_OWNER]);
	if(!owner)
	{
		META_VAL_ERROR("unknown owner reference for method semantics");
		return IL_LOADERR_BAD_META;
	}

	/* Create the method semantics block and attach it */
	property = _ILProgramItem_ToPropertyDef(owner);
	if(property != 0 &&
	   values[IL_OFFSET_METHODSEM_SEMANTICS] == IL_META_METHODSEM_GETTER)
	{
		property->getter = method;
	}
	else if(property != 0 &&
	        values[IL_OFFSET_METHODSEM_SEMANTICS] == IL_META_METHODSEM_SETTER)
	{
		property->setter = method;
	}
	else if(!ILMethodSemCreate(owner, token,
						       values[IL_OFFSET_METHODSEM_SEMANTICS], method))
	{
		return IL_LOADERR_MEMORY;
	}

	/* Done */
	return 0;
}

/*
 * Load a member reference token.
 */
static int Load_MemberRef(ILImage *image, ILUInt32 *values,
						  ILUInt32 *valuesNext, ILToken token,
						  void *userData)
{
	const char *name;
	ILType *type;
	ILClass *classInfo;
	ILClass *currentInfo;
	ILClass *resolvedClass;
	ILTypeSpec *spec;
	ILMember *member;
	ILMember *resolvedMember;
	int isMethod;
	ILMethod *method;
	int loadFlags = (int)(ILNativeInt)userData;
	ILType *origSig;

	/* Get the member's name */
	name = ILImageGetString(image, values[IL_OFFSET_MEMBERREF_NAME]);
	if(!name)
	{
		META_VAL_ERROR("invalid MemberRef name");
		return IL_LOADERR_BAD_META;
	}

	/* Parse the type signature */
	if(values[IL_OFFSET_MEMBERREF_SIGNATURE_LEN] > 0 &&
	   image->blobPool[values[IL_OFFSET_MEMBERREF_SIGNATURE]] ==
			(char)IL_META_CALLCONV_FIELD)
	{
		/* Refers to a field */
		type = ILTypeFromFieldSig(image->context, image,
								  values[IL_OFFSET_MEMBERREF_SIGNATURE],
								  values[IL_OFFSET_MEMBERREF_SIGNATURE_LEN]);
		isMethod = 0;
	}
	else
	{
		/* Refers to a method */
		type = ILTypeFromMethodRefSig(image->context, image,
						      values[IL_OFFSET_MEMBERREF_SIGNATURE],
						      values[IL_OFFSET_MEMBERREF_SIGNATURE_LEN]);
		isMethod = 1;
	}
	if(!type)
	{
		META_VAL_ERROR("invalid MemberRef type signature");
		return IL_LOADERR_BAD_META;
	}
	origSig = type;

	/* Determine where to look for the member */
	classInfo = 0;
	switch(values[IL_OFFSET_MEMBERREF_PARENT] & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_TYPE_DEF:
		{
			/* Refers to a member of a local type */
			classInfo = ILClass_FromToken
							(image, values[IL_OFFSET_MEMBERREF_PARENT]);
			member = 0;
			if(!classInfo)
			{
				break;
			}
		localType:
			currentInfo = classInfo;
			while(currentInfo != 0)
			{
				member = 0;
				while((member = ILClassNextMember(currentInfo, member)) != 0)
				{
					if(!strcmp(ILMember_Name(member), name))
					{
						if(isMethod && ILMember_IsMethod(member))
						{
							if(ILTypeIdentical
									(ILMember_Signature(member), type))
							{
								break;
							}
						}
						else if(!isMethod && ILMember_IsField(member))
						{
							if(ILTypeIdentical
									(ILMember_Signature(member), type))
							{
								break;
							}
						}
					}
				}
				if(member != 0)
				{
					break;
				}
				currentInfo = ILClass_ParentClass(currentInfo);
			}
			if(member == 0 && ILType_IsComplex(type) &&
			   ILType_Kind(type) == (IL_TYPE_COMPLEX_METHOD |
									 IL_TYPE_COMPLEX_METHOD_SENTINEL))
			{
				/* Create a local reference to a vararg call site */
				method = ILMethodCreate(classInfo, token, name, 0);
				if(!method)
				{
					return IL_LOADERR_MEMORY;
				}
				ILMethodSetCallConv(method, ILType_CallConv(type));
				ILMemberSetSignature((ILMember *)method, type);
				return 0;
			}
		}
		break;

	 	case IL_META_TOKEN_TYPE_REF:
		{
			/* Refers to a member of a foreign type */
			classInfo = ILClass_FromToken
							(image, values[IL_OFFSET_MEMBERREF_PARENT]);
			member = 0;
			if(!classInfo)
			{
				break;
			}
			if(!ILClassIsRef(classInfo))
			{
				/* The TypeRef was resolved to a local type */
				goto localType;
			}
		foreignType:
			if(isMethod)
			{
				/* Create a method reference within the TypeRef */
				method = ILMethodCreate(classInfo, token, name, 0);
				if(!method)
				{
					return IL_LOADERR_MEMORY;
				}
				ILMethodSetCallConv(method, ILType_CallConv(type));
				member = (ILMember *)method;
			}
			else
			{
				/* Create a field reference within the TypeRef */
				member = (ILMember *)ILFieldCreate(classInfo, token, name, 0);
				if(!member)
				{
					return IL_LOADERR_MEMORY;
				}
			}
			ILMemberSetSignature(member, type);

			/* Resolve the member reference to the foreign type */
			if((loadFlags & IL_LOADFLAG_NO_RESOLVE) == 0)
			{
				resolvedClass = ILClassResolve(classInfo);
				if(!resolvedClass || ILClassIsRef(resolvedClass))
				{
					/* Failed to resolve the class */
					if(resolvedClass &&
					   IsOnRedoList(image->context,
								    &(resolvedClass->programItem)))
					{
						/* The TypeRef is on the redo list, so add
						   the MemberRef to the redo list also */
						if(!AddToRedoList(image->context,
										  &(member->programItem)))
						{
							return IL_LOADERR_MEMORY;
						}
						break;
					}
					else
					{
						member = 0;
						break;
					}
				}
				resolvedMember = 0;
				while(resolvedClass != 0)
				{
					resolvedMember = 0;
					while((resolvedMember = ILClassNextMember
								(resolvedClass, resolvedMember)) != 0)
					{
						if(!strcmp(ILMember_Name(resolvedMember), name))
						{
							if(isMethod && ILMember_IsMethod(resolvedMember))
							{
								if(ILTypeIdentical
									(ILMember_Signature(resolvedMember), type))
								{
									break;
								}
							}
							else if(!isMethod &&
									ILMember_IsField(resolvedMember))
							{
								if(ILTypeIdentical
									(ILMember_Signature(resolvedMember), type))
								{
									break;
								}
							}
						}
					}
					if(resolvedMember != 0)
					{
						break;
					}
					resolvedClass = ILClass_ParentClass(resolvedClass);
				}
				if(!resolvedMember)
				{
					/* Failed to resolve the member */
					if(ILType_IsComplex(type) &&
					   ILType_Kind(type) == (IL_TYPE_COMPLEX_METHOD |
					   						 IL_TYPE_COMPLEX_METHOD_SENTINEL))
					{
						/* Create a local reference to a vararg call site */
						method = ILMethodCreate(classInfo, token, name, 0);
						if(!method)
						{
							return IL_LOADERR_MEMORY;
						}
						ILMethodSetCallConv(method, ILType_CallConv(type));
						ILMemberSetSignature((ILMember *)method, type);
						return 0;
					}
					member = 0;
					break;
				}
				if(!_ILProgramItemLink(&(member->programItem),
									   &(resolvedMember->programItem)))
				{
					return IL_LOADERR_MEMORY;
				}
			}
		}
		break;

	 	case IL_META_TOKEN_MODULE_REF:
		{
			/* Refers to a member of a foreign module's global type */
			member = 0;
		}
		break;

		case IL_META_TOKEN_METHOD_DEF:
		{
			/* The parent refers to a local method: ignore the other info */
			member = ILMember_FromToken
						(image, values[IL_OFFSET_MEMBERREF_PARENT]);
		}
		break;

		case IL_META_TOKEN_TYPE_SPEC:
		{
			/* Refers to a type which is named using a signature */
			spec = ILTypeSpec_FromToken
						(image, values[IL_OFFSET_MEMBERREF_PARENT]);
			classInfo = ILTypeSpecGetClassRef(spec);
			member = 0;
			if(!classInfo)
			{
				break;
			}
			if(ILClassIsRef(classInfo) && classInfo->programItem.token != 0)
			{
				goto foreignType;
			}
			else
			{
				goto localType;
			}
		}
		/* Not reached */

		default:
		{
			META_VAL_ERROR("unknown MemberRef parent");
			return IL_LOADERR_BAD_META;
		}
		/* Not reached */
	}

	/* Validate the member information */
	if(!member)
	{
	#if IL_DEBUG_META
		if(classInfo && classInfo->className)
		{
			if(classInfo->className->namespace &&
			   !strcmp(classInfo->className->namespace, "$Synthetic"))
			{
				fprintf(stderr,
						"token 0x%08lX: member `%s.%s' not found\n",
						(unsigned long)token, classInfo->className->name, name);
			}
			else
			{
				fprintf(stderr,
						"token 0x%08lX: member `%s%s%s.%s' not found\n",
						(unsigned long)token,
						(classInfo->className->namespace
								? classInfo->className->namespace : ""),
						(classInfo->className->namespace ? "." : ""),
						classInfo->className->name, name);
			}
		}
		else
		{
			fprintf(stderr,
					"token 0x%08lX: member %s not found\n",
					(unsigned long)token, name);
		}
	#endif
		return IL_LOADERR_UNRESOLVED;
	}
	if(!ILMember_IsMethod(member) && !ILMember_IsField(member))
	{
		META_VAL_ERROR("MemberRef does not refer to a method or field");
		return IL_LOADERR_BAD_META;
	}

	/* Create a reference in the MemberRef table to the actual member */
	if(member->programItem.token != token)
	{
		member = ILMemberCreateRef(member, token);
		if(!member)
		{
			return IL_LOADERR_MEMORY;
		}
		if(origSig != ILType_Invalid)
		{
			member->signature = origSig;
		}
		_ILMemberSetSignatureIndex
				(member, values[IL_OFFSET_MEMBERREF_SIGNATURE_RAW]);
	}

	/* Done */
	return 0;
}

/*
 * Load a custom attribute token.
 */
static int Load_CustomAttr(ILImage *image, ILUInt32 *values,
						   ILUInt32 *valuesNext, ILToken token,
						   void *userData)
{
	ILAttribute *attr;
	ILProgramItem *owner;
	ILProgramItem *type;

	/* Create an attribute block */
	if((attr = ILAttributeCreate(image, token)) == 0)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Set the owner for the attribute */
	owner = ILProgramItem_FromToken(image, values[IL_OFFSET_CUSTOMATTR_OWNER]);
	if(owner)
	{
		ILProgramItemAddAttribute(owner, attr);
	}

	/* Set the type of the attribute */
	if((values[IL_OFFSET_CUSTOMATTR_NAME] & IL_META_TOKEN_MASK)
			!= IL_META_TOKEN_STRING)
	{
		type = ILProgramItem_FromToken
					(image, values[IL_OFFSET_CUSTOMATTR_NAME]);
		if(type)
		{
			ILAttributeSetType(attr, type);
		}
	}
	else
	{
		ILAttributeSetString(attr);
	}

	/* Set the attribute's value */
	_ILAttributeSetValueIndex(attr, values[IL_OFFSET_CUSTOMATTR_DATA_RAW]);

	/* Done */
	return 0;
}

/*
 * Search for a custom attribute declaration in a raw token table.
 */
static int Search_CustomAttr(ILUInt32 *values, ILToken token1, int valueField)
{
	ILToken token2 = (ILToken)(values[IL_OFFSET_CUSTOMATTR_OWNER]);
	ILToken tokenNum1 = (token1 & ~IL_META_TOKEN_MASK);
	ILToken tokenNum2 = (token2 & ~IL_META_TOKEN_MASK);
	int type1, type2;

	/* Compare the bottom parts of the token first, because
	   the table must be sorted on its encoded value, not
	   on the original value.  Encoded values put the token
	   type in the low order bits */
	if(tokenNum1 < tokenNum2)
	{
		return -1;
	}
	else if(tokenNum1 > tokenNum2)
	{
		return 1;
	}

	/* Convert the token types into encoded type values */
	switch(token1 & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_METHOD_DEF:				type1 =  0; break;
		case IL_META_TOKEN_FIELD_DEF:				type1 =  1; break;
		case IL_META_TOKEN_TYPE_REF:				type1 =  2; break;
		case IL_META_TOKEN_TYPE_DEF:				type1 =  3; break;
		case IL_META_TOKEN_PARAM_DEF:				type1 =  4; break;
		case IL_META_TOKEN_INTERFACE_IMPL:			type1 =  5; break;
		case IL_META_TOKEN_MEMBER_REF:				type1 =  6; break;
		case IL_META_TOKEN_MODULE:					type1 =  7; break;
		case IL_META_TOKEN_DECL_SECURITY:			type1 =  8; break;
		case IL_META_TOKEN_PROPERTY:				type1 =  9; break;
		case IL_META_TOKEN_EVENT:					type1 = 10; break;
		case IL_META_TOKEN_STAND_ALONE_SIG:			type1 = 11; break;
		case IL_META_TOKEN_MODULE_REF:				type1 = 12; break;
		case IL_META_TOKEN_TYPE_SPEC:				type1 = 13; break;
		case IL_META_TOKEN_ASSEMBLY:				type1 = 14; break;
		case IL_META_TOKEN_ASSEMBLY_REF:			type1 = 15; break;
		case IL_META_TOKEN_FILE:					type1 = 16; break;
		case IL_META_TOKEN_EXPORTED_TYPE:			type1 = 17; break;
		case IL_META_TOKEN_MANIFEST_RESOURCE:		type1 = 18; break;
		default:									type1 = 19; break;
	}
	switch(token2 & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_METHOD_DEF:				type2 =  0; break;
		case IL_META_TOKEN_FIELD_DEF:				type2 =  1; break;
		case IL_META_TOKEN_TYPE_REF:				type2 =  2; break;
		case IL_META_TOKEN_TYPE_DEF:				type2 =  3; break;
		case IL_META_TOKEN_PARAM_DEF:				type2 =  4; break;
		case IL_META_TOKEN_INTERFACE_IMPL:			type2 =  5; break;
		case IL_META_TOKEN_MEMBER_REF:				type2 =  6; break;
		case IL_META_TOKEN_MODULE:					type2 =  7; break;
		case IL_META_TOKEN_DECL_SECURITY:			type2 =  8; break;
		case IL_META_TOKEN_PROPERTY:				type2 =  9; break;
		case IL_META_TOKEN_EVENT:					type2 = 10; break;
		case IL_META_TOKEN_STAND_ALONE_SIG:			type2 = 11; break;
		case IL_META_TOKEN_MODULE_REF:				type2 = 12; break;
		case IL_META_TOKEN_TYPE_SPEC:				type2 = 13; break;
		case IL_META_TOKEN_ASSEMBLY:				type2 = 14; break;
		case IL_META_TOKEN_ASSEMBLY_REF:			type2 = 15; break;
		case IL_META_TOKEN_FILE:					type2 = 16; break;
		case IL_META_TOKEN_EXPORTED_TYPE:			type2 = 17; break;
		case IL_META_TOKEN_MANIFEST_RESOURCE:		type2 = 18; break;
		default:									type2 = 19; break;
	}

	/* Compare the encoded token types */
	if(type1 < type2)
	{
		return -1;
	}
	else if(type1 > type2)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

void _ILProgramItemLoadAttributes(ILProgramItem *item)
{
	ILToken token;
	ILUInt32 numTokens;
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];

	/* Find the custom attribute information for this token */
	numTokens = _ILSearchForRawToken(item->image, Search_CustomAttr,
								     IL_META_TOKEN_CUSTOM_ATTRIBUTE,
								     &token, (ILToken)(item->token), 0);

	/* Load the custom attribute tokens for this item */
	while(numTokens > 0)
	{
		if(_ILImageRawTokenData(item->image, token, values))
		{
			Load_CustomAttr(item->image, values, 0, token, 0);
		}
		--numTokens;
		++token;
	}
}

/*
 * Load a stand alone signature token.
 */
static int Load_StandAloneSig(ILImage *image, ILUInt32 *values,
						      ILUInt32 *valuesNext, ILToken token,
						      void *userData)
{
	ILType *type;
	char sigType;
	ILStandAloneSig *sig;

	/* Determine if we need to parse locals or a method */
	if(values[IL_OFFSET_SIGNATURE_VALUE_LEN])
	{
		sigType = image->blobPool[values[IL_OFFSET_SIGNATURE_VALUE]];
	}
	else
	{
		sigType = 0xFF;
	}
	if(sigType == (char)IL_META_CALLCONV_LOCAL_SIG)
	{
		/* Parse locals */
		type = ILTypeFromLocalVarSig
					(image, values[IL_OFFSET_SIGNATURE_VALUE_RAW]);
	}
	else if(sigType == (char)IL_META_CALLCONV_FIELD)
	{
		/* Parse a field signature */
		type = ILTypeFromFieldSig
					(image->context, image,
					 values[IL_OFFSET_SIGNATURE_VALUE],
					 values[IL_OFFSET_SIGNATURE_VALUE_LEN]);
	}
	else
	{
		/* Parse a method */
		type = ILTypeFromStandAloneMethodSig
					(image->context, image,
					 values[IL_OFFSET_SIGNATURE_VALUE],
					 values[IL_OFFSET_SIGNATURE_VALUE_LEN]);
	}
	if(!type)
	{
		META_VAL_ERROR("invalid stand alone signature");
		return IL_LOADERR_BAD_META;
	}

	/* Create the signature block and attach it to the image */
	sig = ILStandAloneSigCreate(image, token, type);
	if(!sig)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Set the type blob index in case this image is later written */
	_ILStandAloneSigSetTypeIndex(sig, values[IL_OFFSET_SIGNATURE_VALUE_RAW]);

	/* Done */
	return 0;
}

/*
 * Load a constant token.
 */
static int Load_Constant(ILImage *image, ILUInt32 *values,
					     ILUInt32 *valuesNext, ILToken token,
					     void *userData)
{
	ILConstant *constant;

	/* Create the constant record */
	constant = ILConstantCreate(image, token,
						ILProgramItem_FromToken
							(image, values[IL_OFFSET_CONSTANT_REFERENCE]),
						values[IL_OFFSET_CONSTANT_TYPE]);
	if(!constant)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Add the value information to the constant */
	_ILConstantSetValueIndex(constant, values[IL_OFFSET_CONSTANT_DATA_RAW]);

	/* Done */
	return 0;
}

/*
 * Load a field RVA token.
 */
static int Load_FieldRVA(ILImage *image, ILUInt32 *values,
					     ILUInt32 *valuesNext, ILToken token,
					     void *userData)
{
	if(!ILFieldRVACreate(image, token,
						 ILField_FromToken
							(image, values[IL_OFFSET_FIELDRVA_FIELD]),
						 values[IL_OFFSET_FIELDRVA_RVA]))
	{
		return IL_LOADERR_MEMORY;
	}
	return 0;
}

/*
 * Load a field layout token.
 */
static int Load_FieldLayout(ILImage *image, ILUInt32 *values,
					        ILUInt32 *valuesNext, ILToken token,
					        void *userData)
{
	if(!ILFieldLayoutCreate(image, token,
						    ILField_FromToken
								(image, values[IL_OFFSET_FIELDLAYOUT_FIELD]),
						 	values[IL_OFFSET_FIELDLAYOUT_OFFSET]))
	{
		return IL_LOADERR_MEMORY;
	}
	return 0;
}

/*
 * Load a field marshal token.
 */
static int Load_FieldMarshal(ILImage *image, ILUInt32 *values,
					     	 ILUInt32 *valuesNext, ILToken token,
					     	 void *userData)
{
	ILFieldMarshal *marshal;
	ILProgramItem *owner;

	/* Get the owner */
	owner = ILProgramItem_FromToken
				(image, values[IL_OFFSET_FIELDMARSHAL_TOKEN]);

	/* If the owner is NULL, then turn off the "sorted" flag
	   within the image.  Some images seem to have invalid
	   parameter indexes, which can cause problems later
	   in ILImageSearchForToken */
	if(!owner)
	{
		image->sorted &=
			~(((ILUInt64)1) << (IL_META_TOKEN_FIELD_MARSHAL >> 24));
	}

	/* Create the field marshal record */
	marshal = ILFieldMarshalCreate(image, token, owner);
	if(!marshal)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Add the type information to the record */
	_ILFieldMarshalSetTypeIndex
			(marshal, values[IL_OFFSET_FIELDMARSHAL_TYPE_RAW]);

	/* Done */
	return 0;
}

/*
 * Load a class layout token.
 */
static int Load_ClassLayout(ILImage *image, ILUInt32 *values,
					        ILUInt32 *valuesNext, ILToken token,
					        void *userData)
{
	if(!ILClassLayoutCreate(image, token,
						    ILClass_FromToken
								(image, values[IL_OFFSET_CLASSLAYOUT_TYPE]),
						 	values[IL_OFFSET_CLASSLAYOUT_PACKING],
						 	values[IL_OFFSET_CLASSLAYOUT_SIZE]))
	{
		return IL_LOADERR_MEMORY;
	}
	return 0;
}

/*
 * Load a security token.
 */
static int Load_DeclSecurity(ILImage *image, ILUInt32 *values,
					     	 ILUInt32 *valuesNext, ILToken token,
					     	 void *userData)
{
	ILDeclSecurity *security;

	/* Create the security record */
	security = ILDeclSecurityCreate(image, token,
						ILProgramItem_FromToken
							(image, values[IL_OFFSET_DECLSECURITY_TOKEN]),
						values[IL_OFFSET_DECLSECURITY_TYPE]);
	if(!security)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Add the blob information to the record */
	_ILDeclSecuritySetBlobIndex
			(security, values[IL_OFFSET_DECLSECURITY_DATA_RAW]);

	/* Done */
	return 0;
}

/*
 * Load a file declaration token.
 */
static int Load_File(ILImage *image, ILUInt32 *values,
				   	 ILUInt32 *valuesNext, ILToken token,
				   	 void *userData)
{
	ILFileDecl *decl;

	/* Create the file declaration record */
	decl = ILFileDeclCreate(image, token,
							ILImageGetString
								(image, values[IL_OFFSET_FILE_NAME]),
							values[IL_OFFSET_FILE_ATTRS]);
	if(!decl)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Add the hash information to the record */
	_ILFileDeclSetHashIndex(decl, values[IL_OFFSET_FILE_HASH_RAW]);

	/* Done */
	return 0;
}

/*
 * Load a manifest resource token.
 */
static int Load_ManifestRes(ILImage *image, ILUInt32 *values,
				   	 		ILUInt32 *valuesNext, ILToken token,
				   	 		void *userData)
{
	ILManifestRes *res;
	ILProgramItem *owner;
	ILFileDecl *decl;
	ILAssembly *assem;

	/* Create the manifest resource record */
	res = ILManifestResCreate(image, token,
							  ILImageGetString
								(image, values[IL_OFFSET_MANIFESTRES_NAME]),
							  values[IL_OFFSET_MANIFESTRES_ATTRS],
							  0);
	if(!res)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Add the owner information to the record */
	owner = ILProgramItem_FromToken(image, values[IL_OFFSET_MANIFESTRES_IMPL]);
	if((decl = ILProgramItemToFileDecl(owner)) != 0)
	{
		ILManifestResSetOwnerFile(res, decl,
								  values[IL_OFFSET_MANIFESTRES_OFFSET]);
	}
	else if((assem = ILProgramItemToAssembly(owner)) != 0)
	{
		ILManifestResSetOwnerAssembly(res, assem);
	}

	/* Done */
	return 0;
}

/*
 * Load an exported type token.
 */
static int Load_ExportedType(ILImage *image, ILUInt32 *values,
				   	 		 ILUInt32 *valuesNext, ILToken token,
				   	 		 void *userData)
{
	ILExportedType *type;
	const char *namespace;

	/* Create the exported type record */
	namespace = ILImageGetString(image, values[IL_OFFSET_EXPTYPE_NAMESPACE]);
	if(namespace && *namespace == '\0')
	{
		namespace = 0;
	}
	type = ILExportedTypeCreate(image, token,
				  	values[IL_OFFSET_EXPTYPE_ATTRS],
				    ILImageGetString
						(image, values[IL_OFFSET_EXPTYPE_NAME]), namespace,
					ILProgramItem_FromToken
						(image, values[IL_OFFSET_EXPTYPE_FILE]));
	if(!type)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Set the foreign class identifier */
	ILExportedTypeSetId(type, values[IL_OFFSET_EXPTYPE_CLASS]);

	/* Add the exported type to the "redo" list */
	if((image->loadFlags & IL_LOADFLAG_NO_RESOLVE) == 0)
	{
		if(!AddToRedoList(image->context, &(type->classItem.programItem)))
		{
			return IL_LOADERR_MEMORY;
		}
	}

	/* Done */
	return 0;
}

/*
 * Load a generic parameter token.
 */
static int Load_GenericPar(ILImage *image, ILUInt32 *values,
				   	 	   ILUInt32 *valuesNext, ILToken token,
				   	 	   void *userData)
{
	ILProgramItem *owner;
	ILGenericPar *genPar;

	/* Get the owner */
	owner = ILProgramItem_FromToken(image, values[IL_OFFSET_GENERICPAR_OWNER]);
	if(!owner)
	{
		META_VAL_ERROR("invalid generic parameter owner");
		return IL_LOADERR_BAD_META;
	}

	/* Generate the generic parameter record */
	genPar = ILGenericParCreate
			(image, token, owner, values[IL_OFFSET_GENERICPAR_NUMBER]);
	if(!genPar)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Set the record's properties */
	genPar->flags = (ILUInt16)(values[IL_OFFSET_GENERICPAR_FLAGS]);
	if(!ILGenericParSetName(genPar, ILImageGetString
			(image, values[IL_OFFSET_GENERICPAR_NAME])))
	{
		return IL_LOADERR_MEMORY;
	}

	/* Done */
	return 0;
}

/*
 * Load a generic constraint token (non-Gyro version).
 */
static int Load_GenericConstraint(ILImage *image, ILUInt32 *values,
				   	 	          ILUInt32 *valuesNext, ILToken token,
				   	 	          void *userData)
{
	ILGenericPar *genPar;

	/* Get the generic parameter that this constraint applies to */
	genPar = ILGenericPar_FromToken(image, values[IL_OFFSET_GENERICCON_PARAM]);
	if(!genPar)
	{
		META_VAL_ERROR("invalid generic parameter");
		return IL_LOADERR_BAD_META;
	}

	/* Add the generic constraint to the parameter */
	if (!ILGenericParAddConstraint(genPar,
								   token,
								   ILProgramItem_FromToken (image, 
										values[IL_OFFSET_GENERICCON_CONSTRAINT])))
	{
		return IL_LOADERR_MEMORY;
	}

	/* Done */
	return 0;
}

/*
 * Load a type definition token.
 */
static int Load_TypeDef(ILImage *image, ILUInt32 *values,
						ILUInt32 *valuesNext, ILToken token,
						void *userData)
{
	ILClass *info;
	ILProgramItem *parent;
	const char *name;
	const char *namespace;
	int error;
	ILProgramItem *scope;
#if 0
	ILUInt32 num;
#endif

	/* If we have already loaded this type, then bail out */
	if(_ILImageTokenAlreadyLoaded(image, token))
	{
		return 0;
	}

	/* Get the name and namespace for the type */
	name = ILImageGetString(image, values[IL_OFFSET_TYPEDEF_NAME]);
	namespace = ILImageGetString(image, values[IL_OFFSET_TYPEDEF_NAMESPACE]);
	if(namespace && *namespace == '\0')
	{
		namespace = 0;
	}

	/* Determine the scope to use to define the type */
	if((values[IL_OFFSET_TYPEDEF_ATTRS] & IL_META_TYPEDEF_VISIBILITY_MASK)
			< IL_META_TYPEDEF_NESTED_PUBLIC)
	{
		/* Global scope */
		scope = ILClassGlobalScope(image);
	}
	else
	{
		/* Nested scope */
		scope = FindNestedParent(image, token, 0);
	}

	/* If we don't have a scope, then exit with an error */
	if(!scope)
	{
		META_VAL_ERROR("unknown type definition scope");
		return IL_LOADERR_BAD_META;
	}

	/* See if we already have a definition using this name */
	info = ILClassLookup(scope, name, namespace);
	if(info)
	{
		if(!ILClassIsRef(info))
		{
			META_VAL_ERROR("type defined multiple times");
			return IL_LOADERR_BAD_META;
		}
	}

	/* Create the class, which will convert the reference if necessary */
	info = ILClassCreate(scope, token, name, namespace, 0);
	if(!info)
	{
		return IL_LOADERR_MEMORY;
	}
	/* 
	   Note: We create the class before loading its parent because the 
	   class itself can be used as part of the parent class. This can happen
	   when the parent class is generic. Ex.
	   class P<T> {}
	   class E: P<E> {}
     */

	/* Locate the parent class */
	if(values[IL_OFFSET_TYPEDEF_PARENT])
	{
		parent = ILProgramItem_FromToken(image, values[IL_OFFSET_TYPEDEF_PARENT]);
		if(parent)
		{
			/* Nothing to do here */
		}
		else if((values[IL_OFFSET_TYPEDEF_PARENT] & IL_META_TOKEN_MASK)
					== IL_META_TOKEN_TYPE_DEF)
		{
			/* The class inherits from a TypeDef we haven't seen yet */
			ILClass *parentInfo;

			error = LoadForwardTypeDef(image, values[IL_OFFSET_TYPEDEF_PARENT]);
			if(error != 0)
			{
				return error;
			}
			parentInfo = ILClass_FromToken(image, values[IL_OFFSET_TYPEDEF_PARENT]);
			if(parentInfo)
			{
				parent = ILToProgramItem(parentInfo);
			}
			else
			{
				parent = 0;
			}
		}
		if(!parent)
		{
			META_VAL_ERROR("cannot locate parent type");
			return IL_LOADERR_BAD_META;
		}
	}
	else
	{
		/* No parent, so this is probably the "System.Object" class */
		parent = 0;
	}

	/* Set the class parent */
	info->parent = parent;

	/* Set the attributes for the class */
	ILClassSetAttrs(info, ~0, values[IL_OFFSET_TYPEDEF_ATTRS]);

#if 0

#if 0	/* TODO */
	/* If the class has a nested attribute, but it is
	   at the top-most level, then complain */
	if(ILClassGetNestedParent(info) == 0 &&
	   (ILClass_IsNestedPublic(info) ||
	    ILClass_IsNestedPrivate(info) ||
		ILClass_IsNestedFamily(info) ||
		ILClass_IsNestedAssembly(info) ||
		ILClass_IsNestedFamAndAssem(info) ||
		ILClass_IsNestedFamOrAssem(info)))
	{
		META_VAL_ERROR("nested class at outer scope");
		return IL_LOADERR_BAD_META;
	}
#endif

	/* Load the fields */
	if(!SizeOfRange(image, IL_META_TOKEN_FIELD_DEF,
					values, valuesNext, IL_OFFSET_TYPEDEF_FIRST_FIELD, &num))
	{
		META_VAL_ERROR("invalid field count");
		return IL_LOADERR_BAD_META;
	}
	EXIT_IF_ERROR(LoadTokenRange(image, IL_META_TOKEN_FIELD_DEF,
								 values[IL_OFFSET_TYPEDEF_FIRST_FIELD], num,
								 Load_FieldDef, info));

	/* Load the methods */
	if(!SizeOfRange(image, IL_META_TOKEN_METHOD_DEF,
					values, valuesNext, IL_OFFSET_TYPEDEF_FIRST_METHOD, &num))
	{
		META_VAL_ERROR("invalid method count");
		return IL_LOADERR_BAD_META;
	}
	EXIT_IF_ERROR(LoadTokenRange(image, IL_META_TOKEN_METHOD_DEF,
								 values[IL_OFFSET_TYPEDEF_FIRST_METHOD], num,
								 Load_MethodDef, info));
#endif

	/* Done: we'll get the other members later */
	return 0;
}

/*
 * Load a method specification token.
 */
static int Load_MethodSpec(ILImage *image, ILUInt32 *values,
				   	 	   ILUInt32 *valuesNext, ILToken token,
				   	 	   void *userData)
{
	ILMember *method;
	ILType *signature;
	ILMethodSpec *spec;

	/* Get the method */
	method = ILMember_FromToken(image, values[IL_OFFSET_METHODSPEC_METHOD]);
	if(!method)
	{
		META_VAL_ERROR("invalid method spec owner");
		return IL_LOADERR_BAD_META;
	}

	/* Get the signature instantiation information */
	signature =
		ILTypeFromMethodDefSig(image->context, image,
						       values[IL_OFFSET_METHODSPEC_INST],
						       values[IL_OFFSET_METHODSPEC_INST_LEN]);
	if(!signature)
	{
		META_VAL_ERROR("invalid method spec signature");
		return IL_LOADERR_BAD_META;
	}

	/* Generate the method specification record */
	spec = ILMethodSpecCreate(image, token, method, signature);
	if(!spec)
	{
		return IL_LOADERR_MEMORY;
	}
	_ILMethodSpecSetTypeIndex(spec, values[IL_OFFSET_METHODSPEC_INST_RAW]);

	/* Done */
	return 0;
}

int _ILImageBuildMetaStructures(ILImage *image, const char *filename,
								int loadFlags)
{
	int error;
	int needPhase2;

	/* Load class names from the TypeRef and TypeDef tables */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_TYPE_REF,
							 Load_TypeRefName, 0));
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_TYPE_DEF,
							 Load_TypeDefName, 0));

	/* Load exported type declarations */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_EXPORTED_TYPE,
							 Load_ExportedType, 0));

	/* Load the assemblies that this image depends upon */
	if((loadFlags & IL_LOADFLAG_NO_RESOLVE) == 0)
	{
		error = _ILImageDynamicLink(image, filename, loadFlags);
		if(error != 0)
		{
			if((loadFlags & IL_LOADFLAG_IGNORE_ERRORS) == 0)
			{
				return error;
			}
		}
	}

	/* Perform phase 1 type resolution on the TypeRef table */
	error = ResolveTypeRefs(image, loadFlags);
	if(error > 0)
	{
		if((loadFlags & IL_LOADFLAG_IGNORE_ERRORS) == 0)
		{
			return error;
		}
	}
	needPhase2 = (error < 0);

	/* Load the TypeDef table - phase 1 */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_TYPE_DEF,
							 Load_TypeDef, 0));

#if 0
	/* Load the TypeSpec table */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_TYPE_SPEC,
							 Load_TypeSpec, 0));
#endif

	/* Perform phase 2 type resolution on the TypeRef table */
	if(needPhase2)
	{
		EXIT_IF_ERROR(ResolveTypeRefsPhase2(image, loadFlags));
	}

	/* Load the TypeDef table - phase 2 (fields and methods) */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_TYPE_DEF,
							 Load_TypeDefPhase2, 0));

	/* Load the InterfaceImpl table */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_INTERFACE_IMPL,
							 Load_InterfaceImpl, 0));

	/* Load events and properties for all of the types */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_EVENT_MAP,
							 Load_EventAssociation, 0));
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_PROPERTY_MAP,
							 Load_PropertyAssociation, 0));
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_METHOD_SEMANTICS,
							 Load_MethodAssociation, 0));

	/* Load member references to other images */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_MEMBER_REF,
							 Load_MemberRef, (void *)(ILNativeInt)loadFlags));

	/* Load the override declarations */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_METHOD_IMPL,
							 Load_Override, 0));

	/* Load generic type parameters */
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_GENERIC_PAR,
							 Load_GenericPar, 0));
	EXIT_IF_ERROR(LoadTokens(image, IL_META_TOKEN_GENERIC_CONSTRAINT,
							 Load_GenericConstraint, 0));

	/* Done */
	return 0;
}

/*
 * Table of token loading functions that can be used
 * for on-demand loading of token blocks.
 */
static TokenLoadFunc const TokenLoadFunctions[] = {
	Load_Module,				/* 00 */
	0,
	0,
	0,
	Load_FieldDefOnDemand,
	0,
	Load_MethodDefOnDemand,
	0,
	0,							/* 08 */
	0,
	0,
	Load_Constant,
	0,
	Load_FieldMarshal,
	Load_DeclSecurity,
	Load_ClassLayout,
	Load_FieldLayout,			/* 10 */
	Load_StandAloneSig,
	0,
	0,
	0,
	0,
	0,
	0,
	0,							/* 18 */
	0,
	Load_ModuleRef,
	Load_TypeSpec,
	Load_PInvoke,
	Load_FieldRVA,
	0,
	0,
	Load_Assembly,				/* 20 */
	Load_ProcessorDef,
	Load_OSDef,
	Load_AssemblyRef,
	Load_ProcessorRef,
	Load_OSRef,
	Load_File,
	0,
	Load_ManifestRes,			/* 28 */
	0,
	0,
	Load_MethodSpec,
	0,
	0,
	0,
	0,
	0,							/* 30 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,							/* 38 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
};

void *_ILImageLoadOnDemand(ILImage *image, ILToken token)
{
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 valuesNext[IL_IMAGE_TOKEN_COLUMNS];
	ILUInt32 *pvaluesNext;
	TokenLoadFunc func;
	void **data;

	/* Load the values for the token */
	if(!_ILImageRawTokenData(image, token, values))
	{
		return 0;
	}

	/* Load the values for the following token, in case it is a range */
	if((token + 1) <= ((token & IL_META_TOKEN_MASK) |
					   image->tokenCount[token >> 24]))
	{
		if(!_ILImageRawTokenData(image, token + 1, valuesNext))
		{
			return 0;
		}
		pvaluesNext = valuesNext;
	}
	else
	{
		pvaluesNext = 0;
	}

	/* Find the loading function for this token type */
	func = TokenLoadFunctions[token >> 24];
	if(!func)
	{
		return 0;
	}

	/* Load the token information */
	if((*func)(image, values, pvaluesNext, token, 0) != 0)
	{
		/* A metadata error was detected */
		return 0;
	}

	/* Retrieve the program item from the token table */
	data = image->tokenData[token >> 24];
	if(data)
	{
		return data[(token & ~IL_META_TOKEN_MASK) - 1];
	}
	return 0;
}

/*
 * Redo a "TypeRef" token that was left dangling because of
 * recursive assembly references.
 */
static int RedoTypeRef(ILImage *image, ILClass *classInfo)
{
	ILProgramItem *scope = ILClassGetScope(classInfo);
	ILProgramItem *importScope;
	ILImage *importImage = 0;
	ILClass *importInfo = 0;
	switch(ILProgramItem_Token(scope) & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_TYPE_REF:
		case IL_META_TOKEN_EXPORTED_TYPE:
		{
			/* Nested type within a foreign assembly */
			importScope = _ILProgramItemLinkedTo(scope);
			if(importScope)
			{
				importInfo = ILClassLookup
					(importScope, ILClass_Name(classInfo),
					 ILClass_Namespace(classInfo));
			}
		}
		break;

		case IL_META_TOKEN_MODULE_REF:
		{
			/* Type is imported from a foreign module */
			importImage = ILModuleToImage((ILModule *)scope);
		}
		break;

		case IL_META_TOKEN_ASSEMBLY_REF:
		{
			/* Type is imported from a foreign assembly */
			importImage = ILAssemblyToImage((ILAssembly *)scope);
		}
		break;

		case IL_META_TOKEN_FILE:
		{
			/* Type is imported from a foreign file */
			importImage = ILFileDeclToImage((ILFileDecl *)scope);
		}
		break;
	}
	if(importImage)
	{
		importScope = ILClassGlobalScope(importImage);
		if(importScope)
		{
			importInfo = ILClassLookup
				(importScope, ILClass_Name(classInfo),
				 ILClass_Namespace(classInfo));
		}
	}
	if(importInfo)
	{
		/* Link "info" to "importInfo" */
		if(!_ILProgramItemLink(&(classInfo->programItem),
							   &(importInfo->programItem)))
		{
			return IL_LOADERR_MEMORY;
		}
		else
		{
			return 0;
		}
	}
	else
	{
		/* Could not resolve the type */
	#if IL_DEBUG_META
		ReportResolveError(image, scope->token,
						   ILClass_Name(classInfo),
						   ILClass_Namespace(classInfo));
	#endif
		return IL_LOADERR_UNRESOLVED;
	}
}

/*
 * Redo a "MemberRef" token that was left dangling because of
 * recursive assembly references.
 */
static int RedoMemberRef(ILImage *image, ILMember *member)
{
	ILToken token = ILMember_Token(member);
	ILClass *classInfo = ILMember_Owner(member);
	const char *name = ILMember_Name(member);
	ILClass *resolvedClass;
	ILMember *resolvedMember;
	int isMethod = ILMember_IsMethod(member);
	ILType *type = ILMember_Signature(member);
	ILMethod *method;

	/* Resolve the class the contains the member */
	resolvedClass = ILClassResolve(classInfo);
	if(!resolvedClass || ILClassIsRef(resolvedClass))
	{
		goto reportError;
	}

	/* Walk the class hierarchy looking for the member */
	resolvedMember = 0;
	while(resolvedClass != 0)
	{
		resolvedMember = 0;
		while((resolvedMember = ILClassNextMember
					(resolvedClass, resolvedMember)) != 0)
		{
			if(!strcmp(ILMember_Name(resolvedMember), name))
			{
				if(isMethod && ILMember_IsMethod(resolvedMember))
				{
					if(ILTypeIdentical
						(ILMember_Signature(resolvedMember), type))
					{
						break;
					}
				}
				else if(!isMethod && ILMember_IsField(resolvedMember))
				{
					if(ILTypeIdentical
						(ILMember_Signature(resolvedMember), type))
					{
						break;
					}
				}
			}
		}
		if(resolvedMember != 0)
		{
			break;
		}
		resolvedClass = ILClass_ParentClass(resolvedClass);
	}
	if(!resolvedMember)
	{
		/* Failed to resolve the member */
		if(ILType_IsComplex(type) &&
		   ILType_Kind(type) == (IL_TYPE_COMPLEX_METHOD |
		   						 IL_TYPE_COMPLEX_METHOD_SENTINEL))
		{
			/* Create a local reference to a vararg call site */
			method = ILMethodCreate(classInfo, token, name, 0);
			if(!method)
			{
				return IL_LOADERR_MEMORY;
			}
			ILMethodSetCallConv(method, ILType_CallConv(type));
			ILMemberSetSignature((ILMember *)method, type);
			return 0;
		}
		goto reportError;
	}
	if(!_ILProgramItemLink(&(member->programItem),
						   &(resolvedMember->programItem)))
	{
		return IL_LOADERR_MEMORY;
	}
	return 0;

	/* Report an error in the member resolution process */
reportError:
#if IL_DEBUG_META
	if(classInfo)
	{
		if(classInfo->className->namespace &&
		   !strcmp(classInfo->className->namespace, "$Synthetic"))
		{
			fprintf(stderr,
					"token 0x%08lX: member `%s.%s' not found\n",
					(unsigned long)token, classInfo->className->name, name);
		}
		else
		{
			fprintf(stderr,
					"token 0x%08lX: member `%s%s%s.%s' not found\n",
					(unsigned long)token,
					(classInfo->className->namespace
							? classInfo->className->namespace : ""),
					(classInfo->className->namespace ? "." : ""),
					classInfo->className->name, name);
		}
	}
	else
	{
		fprintf(stderr,
				"token 0x%08lX: member %s not found\n",
				(unsigned long)token, name);
	}
#endif
	return IL_LOADERR_UNRESOLVED;
}

int _ILImageRedoReferences(ILContext *context)
{
	ILUInt32 posn;
	ILProgramItem *item;
	int error = 0;
	int error2;

	/* Process the TypeRef's that are registered to be redone */
	for(posn = 0; posn < context->numRedoItems; ++posn)
	{
		item = context->redoItems[posn];
		switch(item->token & IL_META_TOKEN_MASK)
		{
			case IL_META_TOKEN_TYPE_REF:
			case IL_META_TOKEN_EXPORTED_TYPE:
			{
				error2 = RedoTypeRef(item->image, (ILClass *)item);
				if(!error)
				{
					error = error2;
				}
			}
			break;
		}
	}

	/* Process the MemberRef's that are registered to be redone.
	   Must be done after all TypeRef's because a member's signature
	   may refer to a TypeRef that appears later in the redo list */
	for(posn = 0; posn < context->numRedoItems; ++posn)
	{
		item = context->redoItems[posn];
		switch(item->token & IL_META_TOKEN_MASK)
		{
			case IL_META_TOKEN_MEMBER_REF:
			{
				error2 = RedoMemberRef(item->image, (ILMember *)item);
				if(!error)
				{
					error = error2;
				}
			}
			break;
		}
	}
	return error;
}

#ifdef	__cplusplus
};
#endif
