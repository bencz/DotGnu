/*
 * assembly.c - Process assembly information from an image file.
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

ILAssembly *ILAssemblyCreate(ILImage *image, ILToken token,
						     const char *name, int isRef)
{
	ILAssembly *assem;

	/* Allocate space for the assembly from the memory stack */
	assem = ILMemStackAlloc(&(image->memStack), ILAssembly);
	if(!assem)
	{
		return 0;
	}
	assem->programItem.image = image;

	/* Convert the assembly name into a persistent string */
	if(name)
	{
		assem->name = _ILContextPersistString(image, name);
		if(!(assem->name))
		{
			return 0;
		}
	}

	/* Assign a token code to the assembly */
	if(token != IL_MAX_UINT32 &&
	   (token != 0 || image->type == IL_IMAGETYPE_BUILDING))
	{
		if(!_ILImageSetToken(image, &(assem->programItem),
							 token, (isRef ? IL_META_TOKEN_ASSEMBLY_REF
							               : IL_META_TOKEN_ASSEMBLY)))
		{
			return 0;
		}
	}

	/* Return the assembly to the caller */
	return assem;
}

int ILAssemblyIsRef(ILAssembly *assem)
{
	return ((assem->programItem.token & IL_META_TOKEN_MASK)
					== IL_META_TOKEN_ASSEMBLY_REF);
}

ILAssembly *ILAssemblyCreateImport(ILImage *image, ILImage *fromImage)
{
	ILAssembly *assem;
	ILAssembly *newAssem;

	/* Get the assembly record for "fromImage" */
	assem = (ILAssembly *)ILImageTokenInfo(fromImage,
										   (IL_META_TOKEN_ASSEMBLY | 1));
	if(!assem)
	{
		return 0;
	}

	/* See if we have a link back to "image" already */
	newAssem = (ILAssembly *)_ILProgramItemLinkedBackTo
									(&(assem->programItem), image);
	if(newAssem)
	{
		return newAssem;
	}

	/* Create a new assembly reference locally */
	if(fromImage != image->context->syntheticImage)
	{
		newAssem = ILAssemblyCreate(image, 0, assem->name, 1);
	}
	else
	{
		newAssem = ILAssemblyCreate(image, IL_MAX_UINT32, assem->name, 1);
	}
	if(!newAssem)
	{
		return 0;
	}

	/* Copy across interesting information */
	if(assem->originator)
	{
		const void *orig;
		ILUInt32 len;
		orig = ILAssemblyGetOriginator(assem, &len);
		if(!ILAssemblySetOriginator(newAssem, orig, len))
		{
			return 0;
		}
	}
	if(assem->locale)
	{
		if(!ILAssemblySetLocale(newAssem, assem->locale))
		{
			return 0;
		}
	}
	newAssem->version[0] = assem->version[0];
	newAssem->version[1] = assem->version[1];
	newAssem->version[2] = assem->version[2];
	newAssem->version[3] = assem->version[3];

	/* Link "newAssem" to "assem" */
	if(!_ILProgramItemLink(&(newAssem->programItem), &(assem->programItem)))
	{
		return 0;
	}
	return newAssem;
}

ILImage *ILAssemblyToImage(ILAssembly *assem)
{
	if(!ILAssemblyIsRef(assem))
	{
		/* Refers to the current image */
		return assem->programItem.image;
	}
	else if(assem->programItem.linked)
	{
		/* We have cached a previous image */
		return _ILProgramItemResolve(&(assem->programItem))->image;
	}
	else
	{
		/* Search for a matching ILAssembly in another image */
		ILImage *image;
		ILAssembly *newAssem = 0;
		image = assem->programItem.image->context->firstImage;
		while(image != 0 && newAssem == 0)
		{
			newAssem = 0;
			while((newAssem = (ILAssembly *)ILImageNextToken
						(image, IL_META_TOKEN_ASSEMBLY, newAssem)) != 0)
			{
				if(!ILStrICmp(newAssem->name, assem->name))
				{
					break;
				}
			}
			image = image->nextImage;
		}
		if(!newAssem)
		{
			return 0;
		}

		/* Link the reference to the foreign assembly */
		_ILProgramItemLink(&(assem->programItem), &(newAssem->programItem));

		/* Return the new assembly's image to the caller */
		return newAssem->programItem.image;
	}
}

void ILAssemblySetHashAlgorithm(ILAssembly *assem, ILUInt32 hashAlg)
{
	assem->hashAlgorithm = hashAlg;
}

ILUInt32 ILAssemblyGetHashAlgorithm(ILAssembly *assem)
{
	return assem->hashAlgorithm;
}

void ILAssemblySetVersion(ILAssembly *assem, const ILUInt16 *version)
{
	if(version)
	{
		assem->version[0] = version[0];
		assem->version[1] = version[1];
		assem->version[2] = version[2];
		assem->version[3] = version[3];
	}
	else
	{
		assem->version[0] = 0;
		assem->version[1] = 0;
		assem->version[2] = 0;
		assem->version[3] = 0;
	}
}

void ILAssemblySetVersionSplit(ILAssembly *assem,
							   ILUInt32 ver1, ILUInt32 ver2,
							   ILUInt32 ver3, ILUInt32 ver4)
{
	assem->version[0] = ver1;
	assem->version[1] = ver2;
	assem->version[2] = ver3;
	assem->version[3] = ver4;
}

const ILUInt16 *ILAssemblyGetVersion(ILAssembly *assem)
{
	return assem->version;
}

void ILAssemblySetAttrs(ILAssembly *assem, ILUInt32 mask, ILUInt32 values)
{
	assem->attributes = ((assem->attributes & ~mask) | values);
}

void ILAssemblySetRefAttrs(ILAssembly *assem, ILUInt32 mask, ILUInt32 values)
{
	assem->refAttributes = ((assem->refAttributes & ~mask) | values);
}

ILUInt32 ILAssemblyGetAttrs(ILAssembly *assem)
{
	return assem->attributes;
}

ILUInt32 ILAssemblyGetRefAttrs(ILAssembly *assem)
{
	return assem->refAttributes;
}

int ILAssemblySetOriginator(ILAssembly *assem, const void *key,
						    ILUInt32 len)
{
	if(assem->programItem.image->type == IL_IMAGETYPE_BUILDING)
	{
		assem->originator = (ILUInt32)(ILImageAddBlob(assem->programItem.image,
													  key, len));
		return (assem->originator != 0);
	}
	else
	{
		/* We cannot use this function when loading images.
		   Use "_ILAssemblySetOrigIndex" instead */
		return 1;
	}
}

void _ILAssemblySetOrigIndex(ILAssembly *assem, ILUInt32 index)
{
	assem->originator = index;
}

const void *ILAssemblyGetOriginator(ILAssembly *assem, ILUInt32 *len)
{
	return ILImageGetBlob(assem->programItem.image, assem->originator, len);
}

int ILAssemblySetName(ILAssembly *assem, const char *name)
{
	assem->name = _ILContextPersistString(assem->programItem.image, name);
	return (assem->name != 0);
}

const char *ILAssemblyGetName(ILAssembly *assem)
{
	return assem->name;
}

int ILAssemblySetLocale(ILAssembly *assem, const char *locale)
{
	assem->locale = _ILContextPersistString(assem->programItem.image, locale);
	return (assem->locale != 0);
}

const char *ILAssemblyGetLocale(ILAssembly *assem)
{
	return assem->locale;
}

int ILAssemblySetHash(ILAssembly *assem, const void *hash, ILUInt32 len)
{
	if(assem->programItem.image->type == IL_IMAGETYPE_BUILDING)
	{
		assem->hashValue = (ILUInt32)(ILImageAddBlob
											(assem->programItem.image,
											 hash, len));
		return (assem->hashValue != 0);
	}
	else
	{
		/* We cannot use this function when loading images.
		   Use "_ILAssemblySetHashIndex" instead */
		return 1;
	}
}

void _ILAssemblySetHashIndex(ILAssembly *assem, ILUInt32 index)
{
	assem->hashValue = index;
}

const void *ILAssemblyGetHash(ILAssembly *assem, ILUInt32 *len)
{
	return ILImageGetBlob(assem->programItem.image,
						  assem->hashValue, len);
}

ILOSInfo *ILOSInfoCreate(ILImage *image, ILToken token,
					     ILUInt32 identifier, ILUInt32 major,
					     ILUInt32 minor, ILAssembly *assem)
{
	ILOSInfo *osinfo;

	/* Allocate space for the OS information block from the memory stack */
	osinfo = ILMemStackAlloc(&(image->memStack), ILOSInfo);
	if(!osinfo)
	{
		return 0;
	}

	/* Set the OS information block fields */
	osinfo->programItem.image = image;
	osinfo->identifier = identifier;
	osinfo->major = major;
	osinfo->minor = minor;
	osinfo->assembly = assem;

	/* Assign a token code to the OS definition */
	if(!_ILImageSetToken(image, &(osinfo->programItem), token,
						 (ILAssemblyIsRef(assem) ? IL_META_TOKEN_OS_REF
						 						 : IL_META_TOKEN_OS_DEF)))
	{
		return 0;
	}

	/* Return the OS information block to the caller */
	return osinfo;
}

void ILOSInfoSetInfo(ILOSInfo *osinfo, ILUInt32 identifier,
					 ILUInt32 major, ILUInt32 minor)
{
	osinfo->identifier = identifier;
	osinfo->major = major;
	osinfo->minor = minor;
}

ILUInt32 ILOSInfoGetIdentifier(ILOSInfo *osinfo)
{
	return osinfo->identifier;
}

ILUInt32 ILOSInfoGetMajor(ILOSInfo *osinfo)
{
	return osinfo->major;
}

ILUInt32 ILOSInfoGetMinor(ILOSInfo *osinfo)
{
	return osinfo->minor;
}

ILAssembly *ILOSInfoGetAssembly(ILOSInfo *osinfo)
{
	return osinfo->assembly;
}

ILProcessorInfo *ILProcessorInfoCreate(ILImage *image, ILToken token,
									   ILUInt32 number, ILAssembly *assem)
{
	ILProcessorInfo *procinfo;

	/* Allocate space for the processor block from the memory stack */
	procinfo = ILMemStackAlloc(&(image->memStack), ILProcessorInfo);
	if(!procinfo)
	{
		return 0;
	}

	/* Set the processor definition fields */
	procinfo->programItem.image = image;
	procinfo->number = number;
	procinfo->assembly = assem;

	/* Assign a token code to the processor information block */
	if(!_ILImageSetToken(image, &(procinfo->programItem), token,
				 (ILAssemblyIsRef(assem) ? IL_META_TOKEN_PROCESSOR_REF
				 						 : IL_META_TOKEN_PROCESSOR_DEF)))
	{
		return 0;
	}

	/* Return the processor information block to the caller */
	return procinfo;
}

void ILProcessorInfoSetNumber(ILProcessorInfo *procinfo, ILUInt32 number)
{
	procinfo->number = number;
}

ILUInt32 ILProcessorInfoGetNumber(ILProcessorInfo *procinfo)
{
	return procinfo->number;
}

ILAssembly *ILProcessorInfoGetAssembly(ILProcessorInfo *procinfo)
{
	return procinfo->assembly;
}

ILOSInfo *ILOSInfoNext(ILAssembly *assem, ILOSInfo *osinfo)
{
	ILToken token;
	ILToken maxToken;
	ILOSInfo *testinfo;

	/* Determine which token to begin looking at */
	if(ILAssemblyIsRef(assem))
	{
		if(osinfo)
		{
			token = osinfo->programItem.token + 1;
		}
		else
		{
			token = (IL_META_TOKEN_OS_REF | 1);
		}
		maxToken = IL_META_TOKEN_OS_REF;
	}
	else
	{
		if(osinfo)
		{
			token = osinfo->programItem.token + 1;
		}
		else
		{
			token = (IL_META_TOKEN_OS_DEF | 1);
		}
		maxToken = IL_META_TOKEN_OS_DEF;
	}

	/* Scan through the OSRef table for the next matching item */
	maxToken |= ILImageNumTokens(assem->programItem.image, maxToken);
	while(token <= maxToken)
	{
		testinfo = (ILOSInfo *)(ILImageTokenInfo(assem->programItem.image,
											     token));
		if(testinfo && testinfo->assembly == assem)
		{
			return testinfo;
		}
		++token;
	}

	/* There are no more OS information blocks for this assembly */
	return 0;
}

ILProcessorInfo *ILProcessorInfoNext(ILAssembly *assem,
								     ILProcessorInfo *procinfo)
{
	ILToken token;
	ILToken maxToken;
	ILProcessorInfo *testinfo;

	/* Determine which token to begin looking at */
	if(ILAssemblyIsRef(assem))
	{
		if(procinfo)
		{
			token = procinfo->programItem.token + 1;
		}
		else
		{
			token = (IL_META_TOKEN_PROCESSOR_REF | 1);
		}
		maxToken = IL_META_TOKEN_PROCESSOR_REF;
	}
	else
	{
		if(procinfo)
		{
			token = procinfo->programItem.token + 1;
		}
		else
		{
			token = (IL_META_TOKEN_PROCESSOR_DEF | 1);
		}
		maxToken = IL_META_TOKEN_PROCESSOR_DEF;
	}

	/* Scan through the ProcessorRef table for the next matching item */
	maxToken |= ILImageNumTokens(assem->programItem.image, maxToken);
	while(token <= maxToken)
	{
		testinfo = (ILProcessorInfo *)(ILImageTokenInfo
						(assem->programItem.image, token));
		if(testinfo && testinfo->assembly == assem)
		{
			return testinfo;
		}
		++token;
	}

	/* There are no more processor information block for this assembly */
	return 0;
}

#ifdef	__cplusplus
};
#endif
