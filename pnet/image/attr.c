/*
 * attr.c - Process attribute information from an image file.
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

ILAttribute *ILAttributeCreate(ILImage *image, ILToken token)
{
	ILAttribute *attr;

	/* Allocate memory for the attribute */
	attr = ILMemStackAlloc(&(image->memStack), ILAttribute);
	if(!attr)
	{
		return 0;
	}

	/* Initialize the attribute fields */
	attr->programItem.image = image;

	/* Assign the token code to the attribute */
	if(!_ILImageSetToken(image, &(attr->programItem),
						 token, IL_META_TOKEN_CUSTOM_ATTRIBUTE))
	{
		return 0;
	}

	/* Return the attribute to the caller */
	return attr;
}

void ILAttributeSetType(ILAttribute *attr, ILProgramItem *type)
{
	attr->type = type;
}

void ILAttributeSetString(ILAttribute *attr)
{
	attr->type = 0;
}

ILProgramItem *ILAttributeGetOwner(ILAttribute *attr)
{
	return attr->owner;
}

int ILAttributeTypeIsString(ILAttribute *attr)
{
	return (attr->type == 0);
}

int ILAttributeTypeIsItem(ILAttribute *attr)
{
	return (attr->type != 0);
}

ILProgramItem *ILAttributeTypeAsItem(ILAttribute *attr)
{
	return attr->type;
}

int ILAttributeSetValue(ILAttribute *attr, const void *blob,
						ILUInt32 len)
{
	if(attr->programItem.image->type == IL_IMAGETYPE_BUILDING)
	{
		attr->value = ILImageAddBlob(attr->programItem.image, blob, len);
		return (attr->value != 0);
	}
	else
	{
		/* We cannot use this function when loading images.
		   Use "_ILAttributeSetValueIndex" instead */
		return 1;
	}
}

void _ILAttributeSetValueIndex(ILAttribute *attr, ILUInt32 index)
{
	attr->value = index;
}

const void *ILAttributeGetValue(ILAttribute *attr, ILUInt32 *len)
{
	return ILImageGetBlob(attr->programItem.image, attr->value, len);
}

#ifdef	__cplusplus
};
#endif
