/*
 * image.c - Utility routines for manipulting IL images.
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

#include "image.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILImage *_ILImageCreate(ILContext *context, unsigned size)
{
	ILImage *image;

	/* Allocate the memory for the image structure */
	if((image = (ILImage *)ILCalloc(1, size)) == 0)
	{
		return 0;
	}

	/* Attach the image to the context */
	image->context = context;
	image->nextImage = context->firstImage;
	image->prevImage = 0;
	if(context->firstImage)
	{
		context->firstImage->prevImage = image;
	}
	context->firstImage = image;

	/* Initialize the memory stack for the image */
	ILMemStackInit(&(image->memStack), 0);

	/* Ready to go */
	return image;
}

void _ILImageDestroyBlockList(ILStringBlock *list); /* create.c */

void ILImageDestroy(ILImage *image)
{
	/* Remove all classes that were loaded from the image from the name hash */
	_ILClassRemoveAllFromHash(image);

	/* Unlink everything that points to or from the image */
	/* TODO */

	/* Free the tokens that were loaded from the image */
	_ILImageFreeTokens(image);

	/* Call the subclass destroy function, if present */
	if(image->destroy)
	{
		(*(image->destroy))(image);
	}

	/* Free the filename */
	if(image->filename)
	{
		ILFree(image->filename);
	}

	/* Remove ourselves from the context's image list */
	if(image->nextImage)
	{
		image->nextImage->prevImage = image->prevImage;
	}
	if(image->prevImage)
	{
		image->prevImage->nextImage = image->nextImage;
	}
	else
	{
		image->context->firstImage = image->nextImage;
	}

	/* Null out "systemImage" or "syntheticImage" if we are that image */
	if(image == image->context->systemImage)
	{
		image->context->systemImage = 0;
	}
	if(image == image->context->syntheticImage)
	{
		image->context->syntheticImage = 0;
	}

	/* Destroy the memory stack */
	ILMemStackDestroy(&(image->memStack));

	/* Destroy any "extra" strings */
	_ILImageDestroyBlockList(image->extraStrings);

	/* Free the memory used to hold the image */
	_ILImageFreeMemory(image);

	/* Free other data and the image structure itself */
	_ILFreeSectionMap(image->map);
	ILFree(image);
}

ILContext *ILImageToContext(ILImage *image)
{
	return image->context;
}

int ILImageType(ILImage *image)
{
	return image->type;
}

int ILImageIsSecure(ILImage *image)
{
	return image->secure;
}

int ILImageHadNative(ILImage *image)
{
	return image->hadNative;
}

int ILImageIs32Bit(ILImage *image)
{
	return image->only32Bit;
}

unsigned long ILImageLength(ILImage *image)
{
	return image->len;
}

const char *ILImageGetFileName(ILImage *image)
{
	return image->filename;
}

unsigned long ILImageGetSectionAddr(ILImage *image, int section)
{
	unsigned long virtAddr = 0;
	ILUInt32 virtSize = 0;
	if(!_ILImageGetSection(image, section, &virtAddr, &virtSize))
	{
		return 0;
	}
	return virtAddr;
}

ILUInt32 ILImageGetSectionSize(ILImage *image, int section)
{
	unsigned long virtAddr = 0;
	ILUInt32 virtSize = 0;
	if(!_ILImageGetSection(image, section, &virtAddr, &virtSize))
	{
		return 0;
	}
	return virtSize;
}

int ILImageGetSection(ILImage *image, int section,
					  void **address, ILUInt32 *size)
{
	unsigned long virtAddr = 0;
	ILUInt32 virtSize = 0;

	/* We cannot extract sections if we are building an image */
	if(image->type == IL_IMAGETYPE_BUILDING)
	{
		return 0;
	}

	/* Find the base and size of the section */
	if(!_ILImageGetSection(image, section, &virtAddr, &virtSize))
	{
		return 0;
	}

	/* Lookup the virtual address and return it to the caller */
	*address = ILImageMapAddress(image, virtAddr);
	if(*address != 0)
	{
		*size = virtSize;
		return 1;
	}
	else
	{
		return 0;
	}
}

ILToken ILImageGetEntryPoint(ILImage *image)
{
	if(image->type != IL_IMAGETYPE_BUILDING)
	{
		unsigned char *runtimeHdr =
			ILImageMapAddress(image, image->headerAddr);
		if(runtimeHdr)
		{
			return IL_READ_UINT32(runtimeHdr + 20);
		}
		else
		{
			return 0;
		}
	}
	else
	{
		return 0;
	}
}

const char *ILImageGetString(ILImage *image, ILUInt32 offset)
{
	if(offset < image->stringPoolSize)
	{
		if(image->stringPool)
		{
			return image->stringPool + offset;
		}
		else if(image->stringBlocks)
		{
			/* This is an image we are building, so walk the
			   block list to find the string */
			ILStringBlock *block = image->stringBlocks;
			while(block != 0 && offset >= (unsigned long)(block->used))
			{
				offset -= block->used;
				block = block->next;
			}
			if(block != 0)
			{
				return (((char *)(block + 1)) + offset);
			}
		}
	}
	else if(!offset)
	{
		/* Offset zero always represents the empty string */
		return "";
	}
	return 0;
}

const void *ILImageGetBlob(ILImage *image, ILUInt32 offset,
						   ILUInt32 *len)
{
	ILMetaDataRead meta;

	/* Find the starting position for the blob, and the
	   total length of the blob pool from that point onwards */
	if(offset > 0 && offset < image->blobPoolSize)
	{
		if(image->blobPool)
		{
			meta.data = (const unsigned char *)(image->blobPool + offset);
			meta.len = image->blobPoolSize - offset;
		}
		else if(image->blobBlocks)
		{
			/* This is an image we are building, so walk the
			   block list to find the blob */
			ILStringBlock *block = image->blobBlocks;
			while(block != 0 && offset >= block->used)
			{
				offset -= block->used;
				block = block->next;
			}
			if(block != 0)
			{
				meta.data = (const unsigned char *)
									(((char *)(block + 1)) + offset);
				meta.len = image->blobPoolSize - offset;
			}
			else
			{
				return 0;
			}
		}
	}
	else
	{
		return 0;
	}

	/* Extract the length of the blob */
	meta.error = 0;
	*len = (unsigned long)ILMetaUncompressData(&meta);
	if(meta.error)
	{
		return 0;
	}

	/* Validate the length of the blob */
	if(*len > meta.len)
	{
		return 0;
	}

	/* Done */
	return (const void *)(meta.data);
}

const char *ILImageGetUserString(ILImage *image, ILUInt32 offset,
								 ILUInt32 *len)
{
	ILStringBlock *block;
	ILMetaDataRead reader;
	ILUInt32 slen;

	if(offset > 0 && offset < image->userStringPoolSize)
	{
		/* Find the start and maximum extent of the string */
		if(image->userStringPool)
		{
			reader.data = (unsigned char *)(image->userStringPool + offset);
			reader.len = image->userStringPoolSize - offset;
		}
		else if(image->userStringBlocks)
		{
			/* This is an image we are building, so walk the
			   block list to find the user string */
			block = image->userStringBlocks;
			while(block != 0 && offset >= block->used)
			{
				offset -= block->used;
				block = block->next;
			}
			if(!block)
			{
				return 0;
			}
			reader.data = ((unsigned char *)(block + 1)) + offset;
			reader.len = image->userStringPoolSize - offset;
		}
		else
		{
			return 0;
		}

		/* Extract the length of the string */
		reader.error = 0;
		slen = ILMetaUncompressData(&reader);
		if((slen & 1) == 0 || slen > reader.len || reader.error)
		{
			return 0;
		}

		/* Return the string to the caller */
		*len = (slen / 2);
		return (const char *)(reader.data);
	}
	else
	{
		return 0;
	}
}

const char *ILImageGetModuleName(ILImage *image)
{
	ILModule *module;
	module = (ILModule *)ILImageTokenInfo(image, (IL_META_TOKEN_MODULE | 1));
	if(module)
	{
		return ILModule_Name(module);
	}
	else
	{
		return 0;
	}
}

const char *ILImageGetAssemblyName(ILImage *image)
{
	ILAssembly *assem;
	assem = (ILAssembly *)ILImageTokenInfo(image, (IL_META_TOKEN_ASSEMBLY | 1));
	if(assem)
	{
		return ILAssembly_Name(assem);
	}
	else
	{
		return 0;
	}
}

#ifdef	__cplusplus
};
#endif
