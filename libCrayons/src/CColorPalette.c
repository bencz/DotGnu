/*
 * CColorPalette.c - Color palette implementation.
 *
 * Copyright (C) 2005  Free Software Foundation, Inc.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

#include "CColorPalette.h"

#ifdef __cplusplus
extern "C" {
#endif

CStatus
CColorPalette_Create(CColorPalette **_this,
                     CColor         *colors,
                     CUInt32         count,
                     CPaletteFlag    flags)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a color table */
	CStatus_Require((colors != 0), CStatus_ArgumentNull);

	/* ensure we have a valid count */
	CStatus_Require
		((count == 256 || count == 128 || count == 2), CStatus_Argument);

	/* allocate the palette */
	if(!(*_this = (CColorPalette *)CMalloc(sizeof(CColorPalette))))
	{
		return CStatus_OutOfMemory;
	}

	/* allocate the color table */
	if(!((*_this)->colors = (CColor *)CMalloc(count * sizeof(CColor))))
	{
		CFree(*_this);
		*_this = 0;
		return CStatus_OutOfMemory;
	}

	/* copy the color entries */
	CMemCopy((*_this)->colors, colors, (count * sizeof(CColor)));

	/* set the count */
	(*_this)->count = count;

	/* set the flags */
	(*_this)->flags = flags;

	/* return successfully */
	return CStatus_OK;
}

CStatus
CColorPalette_Destroy(CColorPalette **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* finalize the members */
	if((*_this)->colors != 0)
	{
		CFree((*_this)->colors);
	}

	/* dispose of this color palette */
	CFree(*_this);

	/* null the this pointer */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CBool
CColorPalette_CheckFormat(CColorPalette *_this,
                          CPixelFormat   format)
{
	/* assertions */
	CASSERT((_this != 0));

	/* determine and return if the format is ok */
	switch(format)
	{
		default: return 1;
		case CPixelFormat_1bppIndexed: { return (_this->count >=   2); }
		case CPixelFormat_4bppIndexed: { return (_this->count >=  16); }
		case CPixelFormat_8bppIndexed: { return (_this->count >= 256); }
	}
}

CINTERNAL CColor
CColorPalette_GetColor(CColorPalette *_this,
                       CUInt32        index)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((index < _this->count));

	/* return the color table entry */
	return _this->colors[index];
}

CINTERNAL CUInt32
CColorPalette_FindBestMatch(CColorPalette *_this,
                            CColor         color)
{
	/* declarations */
	CByte    a;
	CByte    r;
	CByte    g;
	CByte    b;
	CUInt32  index;
	CUInt32  best;
	CUInt32  distance;
	CColor  *curr;
	CColor  *end;

	/* assertions */
	CASSERT((_this != 0));

	/* get the color components */
	a = CColor_A(color);
	r = CColor_R(color);
	g = CColor_G(color);
	b = CColor_B(color);

	/* set the index to the default */
	index = 0;

	/* set the best match to the default */
	best = -1;

	/* set the distance to the default */
	distance = 200000;

	/* get the color entry pointer */
	curr = _this->colors;

	/* get the end pointer */
	end = (curr + _this->count);

	/* find the best match */
	while(curr != end)
	{
		/* declarations */
		CInt32 distA, distR, distG, distB, dist;

		/* get the color components for the current entry */
		distA = CColor_A(*curr) - a;
		distR = CColor_R(*curr) - r;
		distG = CColor_G(*curr) - g;
		distB = CColor_B(*curr) - b;

		/* calculate the distance */
		dist = ((distA * distA) +
		        (distR * distR) +
		        (distG * distG) +
		        (distB * distB));

		/* set the best match to the current entry, as needed */
		if(dist < distance)
		{
			/* reset the distance */
			distance = (CUInt32)dist;

			/* reset the best match */
			best = index;

			/* bail out now if we have a perfect match */
			if(distance == 0) { break; }
		}

		/* move to the next entry position */
		++index;
		++curr;
	}

	/* return the best matching entry */
	return best;
}


#ifdef __cplusplus
};
#endif
