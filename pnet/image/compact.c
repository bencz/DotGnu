/*
 * compact.c - Compact token references that are now definitions.
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

void _ILCompactReferences(ILImage *image)
{
	unsigned long numTypeRefs;
	unsigned long numMemberRefs;
	unsigned long numNestedClasses;
	ILToken token, newToken;
	ILProgramItem *item;
	ILNestedInfo *nested;

	/* Bail out if the image is not being built */
	if(image->type != IL_IMAGETYPE_BUILDING)
	{
		return;
	}

	/* How many type and member references do we have before compaction? */
	numTypeRefs = image->tokenCount[IL_META_TOKEN_TYPE_REF >> 24];
	numMemberRefs = image->tokenCount[IL_META_TOKEN_MEMBER_REF >> 24];

	/* Compact the type reference table */
	newToken = 0;
	for(token = 0; token < numTypeRefs; ++token)
	{
		item = (ILProgramItem *)
			(image->tokenData[IL_META_TOKEN_TYPE_REF >> 24][token]);
		if(item && (item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_REF)
		{
			image->tokenData[IL_META_TOKEN_TYPE_REF >> 24][newToken] = item;
			item->token = (IL_META_TOKEN_TYPE_REF | (newToken + 1));
			++newToken;
		}
	}
	image->tokenCount[IL_META_TOKEN_TYPE_REF >> 24] = newToken;

	/* Compact the member reference table */
	newToken = 0;
	for(token = 0; token < numMemberRefs; ++token)
	{
		item = (ILProgramItem *)
			(image->tokenData[IL_META_TOKEN_MEMBER_REF >> 24][token]);
		if(item && (item->token & IL_META_TOKEN_MASK)
						== IL_META_TOKEN_MEMBER_REF)
		{
			image->tokenData[IL_META_TOKEN_MEMBER_REF >> 24][newToken] = item;
			item->token = (IL_META_TOKEN_MEMBER_REF | (newToken + 1));
			++newToken;
		}
	}
	image->tokenCount[IL_META_TOKEN_MEMBER_REF >> 24] = newToken;

	/* Convert the NestedInfo table into the NestedClass table,
	   by copying the items that correspond to nested TypeDef's */
	numNestedClasses = image->tokenCount[IL_META_TOKEN_NESTED_INFO >> 24];
	for(token = 0; token < numNestedClasses; ++token)
	{
		nested = (ILNestedInfo *)(image->tokenData
						[IL_META_TOKEN_NESTED_INFO >> 24][token]);
		if(nested &&
		   (nested->child->programItem.token & IL_META_TOKEN_MASK)
						== IL_META_TOKEN_TYPE_DEF &&
		   (nested->parent->programItem.token & IL_META_TOKEN_MASK)
						== IL_META_TOKEN_TYPE_DEF)
		{
			_ILImageSetToken(image, &(nested->programItem), 0,
							 IL_META_TOKEN_NESTED_CLASS);
		}
	}
}

#ifdef	__cplusplus
};
#endif
