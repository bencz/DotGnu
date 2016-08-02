/*
 * module.c - Process module information from an image file.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

ILModule *ILModuleCreate(ILImage *image, ILToken token,
						 const char *name, const unsigned char *mvid)
{
	ILModule *module;

	/* Allocate space for the module from the memory stack */
	module = ILMemStackAlloc(&(image->memStack), ILModule);
	if(!module)
	{
		return 0;
	}
	module->programItem.image = image;

	/* Convert the module name into a persistent string */
	if(name)
	{
		module->name = _ILContextPersistString(image, name);
		if(!(module->name))
		{
			return 0;
		}
	}

	/* Assign a token code to the module */
	if(!_ILImageSetToken(image, &(module->programItem),
						 token, IL_META_TOKEN_MODULE))
	{
		return 0;
	}

	/* Create the MVID field for the module */
	module->mvid = (unsigned char *)ILMemStackAllocItem(&(image->memStack), 16);
	if(!(module->mvid))
	{
		return 0;
	}
	if(mvid)
	{
		ILMemCpy(module->mvid, mvid, 16);
	}
	else
	{
		ILGUIDGenerate(module->mvid);
	}

	/* Return the module to the caller */
	return module;
}

int ILModuleSetName(ILModule *module, const char *name)
{
	module->name = _ILContextPersistString(module->programItem.image, name);
	return (module->name != 0);
}

const char *ILModuleGetName(ILModule *module)
{
	return module->name;
}

void ILModuleSetMVID(ILModule *module, const unsigned char *mvid)
{
	/* If "module->mvid" is NULL, then this is a module reference
	   that does not have an MVID associated with it */
	if(module->mvid)
	{
		if(mvid)
		{
			ILMemCpy(module->mvid, mvid, 16);
		}
		else
		{
			ILGUIDGenerate(module->mvid);
		}
	}
}

const unsigned char *ILModuleGetMVID(ILModule *module)
{
	return module->mvid;
}

/*
 * Create the Edit & Continue information for a module.
 */
static ILModuleEnc *CreateEnc(ILModule *module)
{
	if(module->enc)
	{
		/* We already have Edit & Continue information */
		return module->enc;
	}
	else
	{
		/* Create a new Edit & Continue block */
		module->enc = ILMemStackAlloc(&(module->programItem.image->memStack),
									  ILModuleEnc);
		return module->enc;
	}
}

int ILModuleSetGeneration(ILModule *module, ILUInt32 generation)
{
	ILModuleEnc *enc = CreateEnc(module);
	if(enc)
	{
		enc->generation = generation;
		return 1;
	}
	else
	{
		return 0;
	}
}

ILUInt32 ILModuleGetGeneration(ILModule *module)
{
	if(module->enc)
	{
		return module->enc->generation;
	}
	else
	{
		return 0;
	}
}

int ILModuleSetEncId(ILModule *module, const unsigned char *id)
{
	ILModuleEnc *enc = CreateEnc(module);
	if(enc)
	{
		ILMemCpy(enc->encId, id, 16);
		enc->flags |= 1;
		return 1;
	}
	else
	{
		return 0;
	}
}

const unsigned char *ILModuleGetEncId(ILModule *module)
{
	if(module->enc && (module->enc->flags & 1) != 0)
	{
		return module->enc->encId;
	}
	else
	{
		return 0;
	}
}

int ILModuleSetEncBaseId(ILModule *module, const unsigned char *id)
{
	ILModuleEnc *enc = CreateEnc(module);
	if(enc)
	{
		ILMemCpy(enc->encBaseId, id, 16);
		enc->flags |= 2;
		return 1;
	}
	else
	{
		return 0;
	}
}

const unsigned char *ILModuleGetEncBaseId(ILModule *module)
{
	if(module->enc && (module->enc->flags & 2) != 0)
	{
		return module->enc->encBaseId;
	}
	else
	{
		return 0;
	}
}

ILModule *ILModuleRefCreate(ILImage *image, ILToken token, const char *name)
{
	ILModule *module;

	/* Allocate space for the module reference from the memory stack */
	module = ILMemStackAlloc(&(image->memStack), ILModule);
	if(!module)
	{
		return 0;
	}
	module->programItem.image = image;

	/* Convert the module name into a persistent string */
	if(name)
	{
		module->name = _ILContextPersistString(image, name);
		if(!(module->name))
		{
			return 0;
		}
	}

	/* Assign a token code to the module */
	if(!_ILImageSetToken(image, &(module->programItem),
						 token, IL_META_TOKEN_MODULE_REF))
	{
		return 0;
	}

	/* Return the module reference to the caller */
	return module;
}

ILModule *ILModuleRefCreateUnique(ILImage *image, const char *name)
{
	ILModule *module;

	/* Search for an existing reference with the given name */
	module = 0;
	while((module = (ILModule *)ILImageNextToken
					(image, IL_META_TOKEN_MODULE_REF, module)) != 0)
	{
		if(module->name && !ILStrICmp(module->name, name))
		{
			return module;
		}
	}

	/* Create a new reference with the name */
	return ILModuleRefCreate(image, 0, name);
}

int ILModuleIsRef(ILModule *module)
{
	return ((module->programItem.token & IL_META_TOKEN_MASK) ==
					IL_META_TOKEN_MODULE_REF);
}

ILImage *ILModuleToImage(ILModule *module)
{
	if(!ILModuleIsRef(module))
	{
		/* Refers to the current image */
		return module->programItem.image;
	}
	else if(module->programItem.linked)
	{
		/* We have cached a previous image */
		return _ILProgramItemResolve(&(module->programItem))->image;
	}
	else
	{
		/* Search for a matching ILModule in another image */
		ILImage *image;
		ILModule *newModule = 0;
		image = module->programItem.image->context->firstImage;
		while(image != 0 && newModule == 0)
		{
			newModule = 0;
			while((newModule = (ILModule *)ILImageNextToken
						(image, IL_META_TOKEN_MODULE, newModule)) != 0)
			{
				if(!ILStrICmp(newModule->name, module->name))
				{
					break;
				}
			}
			image = image->nextImage;
		}
		if(!newModule)
		{
			return 0;
		}

		/* Link the reference to the foreign module */
		_ILProgramItemLink(&(module->programItem), &(newModule->programItem));

		/* Return the new module's image to the caller */
		return newModule->programItem.image;
	}
}

#ifdef	__cplusplus
};
#endif
