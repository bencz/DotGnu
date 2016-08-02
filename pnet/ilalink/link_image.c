/*
 * link_image.c - Process object images within a linker context.
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

#include "linker.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Builtin public key values, to be used to fool Microsoft runtime
 * engines into loading our assemblies without complaining about
 * unimportant linking policy issues.
 */
char const ILLinkerNeutralKey[] = "00000000000000000400000000000000";
char const ILLinkerNeutralKeyToken[] = "b77a5c561934e089";
char const ILLinkerMicrosoftKey[] =
	"002400000480000094000000060200000024000052534131000400000100010007d1"
	"fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79a"
	"d9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea"
	"05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5"
	"f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293";
char const ILLinkerMicrosoftKeyToken[] = "b03f5f7f11d50a3a";

/*
 * Process a single image that is to be linked into the final binary.
 */
static int ProcessImage(ILLinker *linker, ILImage *image,
					 	const char *filename)
{
	ILModule *thisModule;
	ILAssembly *thisAssem;
	ILModule *module;
	ILAssembly *assem;
	const ILUInt16 *version;
	const char *locale;
	const void *blob;
	ILUInt32 blobLen;
	ILUInt32 thisCompat;
	ILUInt32 compat;
	void *dataSection;
	ILUInt32 dataLen;
	char *attrVersionString;
	ILUInt16 attrVersion[4];

	/* Get the module and assembly for the final output image */
	thisModule = (ILModule *)ILImageTokenInfo(linker->image,
											  (IL_META_TOKEN_MODULE | 1));
	thisAssem = (ILAssembly *)ILImageTokenInfo(linker->image,
											   (IL_META_TOKEN_ASSEMBLY | 1));

	/* Copy module information from the image if not a C object file */
	if(!ILLinkerIsCObject(image))
	{
		module = 0;
		while((module = (ILModule *)ILImageNextToken
					(image, IL_META_TOKEN_MODULE, module)) != 0)
		{
			if(!_ILLinkerConvertAttrs(linker, (ILProgramItem *)module,
									  (ILProgramItem *)thisModule))
			{
				return 0;
			}
		}
	}

	/* Copy assembly information from the image */
	assem = 0;
	while((assem = (ILAssembly *)ILImageNextToken
				(image, IL_META_TOKEN_ASSEMBLY, assem)) != 0)
	{
		/* Convert the assembly attributes */
		if(!_ILLinkerConvertAttrs(linker, (ILProgramItem *)assem,
								  (ILProgramItem *)thisAssem))
		{
			return 0;
		}

		/* Convert the assembly security declarations */
		if(!_ILLinkerConvertSecurity(linker, (ILProgramItem *)assem,
								     (ILProgramItem *)thisAssem))
		{
			return 0;
		}

		/* Copy the version information */
		version = ILAssemblyGetVersion(thisAssem);
		if(version[0] == 0 && version[1] == 0 &&
		   version[2] == 0 && version[3] == 0)
		{
			ILAssemblySetVersion(thisAssem, ILAssemblyGetVersion(assem));
			attrVersionString = ILLinkerGetStringAttribute
				(ILToProgramItem(assem), "AssemblyVersionAttribute",
				 "System.Reflection");
			if(attrVersionString)
			{
				/* The assembly contains an "AssemblyVersion" attribute */
				if(ILLinkerParseVersion(attrVersion, attrVersionString))
				{
					ILAssemblySetVersion(thisAssem, attrVersion);
				}
				ILFree(attrVersionString);
			}
		}

		/* Copy the locale information */
		if(ILAssembly_Locale(thisAssem) == 0)
		{
			locale = ILAssembly_Locale(assem);
			if(locale != 0)
			{
				if(!ILAssemblySetLocale(thisAssem, locale))
				{
					_ILLinkerOutOfMemory(linker);
					return 0;
				}
			}
		}

		/* Copy the originator information */
		if(!ILAssembly_HasPublicKey(thisAssem) &&
		   ILAssembly_HasPublicKey(assem))
		{
			blob = ILAssemblyGetOriginator(assem, &blobLen);
			if(blob)
			{
				if(!ILAssemblySetOriginator(thisAssem, blob, blobLen))
				{
					_ILLinkerOutOfMemory(linker);
					return 0;
				}
				ILAssemblySetAttrs(thisAssem, IL_META_ASSEM_PUBLIC_KEY,
								   IL_META_ASSEM_PUBLIC_KEY);
			}
		}

		/* Copy the compatibility mask if the new image is
		   more restrictive than what we have so far */
		thisCompat = (ILAssemblyGetAttrs(thisAssem) &
						IL_META_ASSEM_COMPATIBILITY_MASK);
		compat = (ILAssemblyGetAttrs(assem) &
						IL_META_ASSEM_COMPATIBILITY_MASK);
		if(compat > thisCompat)
		{
			ILAssemblySetAttrs(thisAssem, IL_META_ASSEM_COMPATIBILITY_MASK,
							   compat);
		}

		/* Copy the retargetable flag */
		if(ILAssembly_Retargetable(assem))
		{
			ILAssemblySetAttrs(thisAssem, IL_META_ASSEM_RETARGETABLE,
							   IL_META_ASSEM_RETARGETABLE);
		}

		/* Copy the JIT flags */
		if(ILAssembly_EnableJITTracking(assem))
		{
			ILAssemblySetAttrs(thisAssem, IL_META_ASSEM_ENABLE_JIT_TRACKING,
							   IL_META_ASSEM_ENABLE_JIT_TRACKING);
		}
		if(ILAssembly_DisableJITOptimizer(assem))
		{
			ILAssemblySetAttrs(thisAssem, IL_META_ASSEM_DISABLE_JIT_OPTIMIZER,
							   IL_META_ASSEM_DISABLE_JIT_OPTIMIZER);
		}
	}

	/* Convert the classes in the image */
	if(!_ILLinkerConvertClasses(linker, image))
	{
		return 0;
	}

	/* Copy the contents of the ".sdata" and ".tls" sections */
	if(ILImageGetSection(image, IL_SECTION_DATA, &dataSection, &dataLen))
	{
		ILWriterOtherWrite(linker->writer, ".sdata", IL_IMAGESECT_SDATA,
						   dataSection, dataLen);
		linker->dataLength += dataLen;
	}
	if(ILImageGetSection(image, IL_SECTION_TLS, &dataSection, &dataLen))
	{
		ILWriterOtherWrite(linker->writer, ".tls", IL_IMAGESECT_TLS,
						   dataSection, dataLen);
		linker->tlsLength += dataLen;
	}

	/* Done */
	return 1;
}

int ILLinkerAddImage(ILLinker *linker, ILContext *context,
					 ILImage *image, const char *filename)
{
	ILLinkImage *linkImage;
	linkImage = (ILLinkImage *)ILMalloc(sizeof(ILLinkImage));
	if(!linkImage)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}
	linkImage->filename = ILDupString(filename);
	if(!(linkImage->filename))
	{
		ILFree(linkImage);
		_ILLinkerOutOfMemory(linker);
		return 0;
	}
	linkImage->context = context;
	linkImage->image = image;
	linkImage->next = 0;
	if(linker->images)
	{
		linker->lastImage->next = linkImage;
	}
	else
	{
		linker->images = linkImage;
	}
	linker->lastImage = linkImage;
	_ILLinkerAddSymbols(linker, image);
	if(ILDebugPresent(image))
	{
		linker->hasDebug = 1;
	}
	return 1;
}

int ILLinkerAddCObject(ILLinker *linker, ILContext *context,
					   ILImage *image, const char *filename)
{
	ILProgramItem *oldItem;
	ILProgramItem *newItem;

	/* Check that the memory model is consistent with the
	   values from previous object files that we linked */
	if(!(linker->isCLink))
	{
		/* This is the first C object, so set the module flag
		   locally and in the final image */
		linker->isCLink = 1;
		oldItem = ILProgramItem_FromToken(image, IL_META_TOKEN_MODULE | 1);
		newItem = ILProgramItem_FromToken
			(linker->image, IL_META_TOKEN_MODULE | 1);
		if(oldItem && newItem)
		{
			_ILLinkerConvertAttrs(linker, oldItem, newItem);
		}
	}

	/* Add the image to the linker */
	return ILLinkerAddImage(linker, context, image, filename);
}

int ILLinkerPerformLink(ILLinker *linker)
{
	ILLinkImage *image = linker->images;
	ILImage *initImage;
	char *curdir;
	unsigned char buffer[IL_META_COMPRESS_MAX_SIZE];
	int ok = 1;

	/* Process the main images */
	linker->imageNum = 1;
	while(image != 0)
	{
		if(!ProcessImage(linker, image->image, image->filename))
		{
			ok = 0;
		}
		++(linker->imageNum);
		image = image->next;
	}

	/* Create the initializers and finalizers for C applications */
	initImage = _ILLinkerCreateInitFini(linker);
	if(initImage)
	{
		if(!ProcessImage(linker, initImage, "init-link"))
		{
			ok = 0;
		}
	}

	/* Add the link directory name if there was debug information */
	if(ok && linker->hasDebug)
	{
		curdir = ILGetCwd();
		if(curdir)
		{
			ILUInt32 curdirOffset;
			ILUInt32 buflen;

			curdirOffset = ILWriterDebugString(linker->writer, curdir);
			buflen = ILMetaCompressData(buffer, curdirOffset);
			ILWriterDebugAddPseudo(linker->writer,
								   ILDebugGetPseudo("LDIR"), 0,
								   buffer, buflen);
		}
	}

	/* Finished the link process */
	return ok;
}

#ifdef	__cplusplus
};
#endif
