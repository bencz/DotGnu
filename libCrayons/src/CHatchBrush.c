/*
 * CHatchBrush.c - Hatch brush implementation.
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

#include "CHatchBrush.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Clone this hatch brush. */
static CStatus
CHatchBrush_Clone(CBrush  *_this,
                  CBrush **_clone)
{
	/* declarations */
	CHatchBrush  *brush;
	CHatchBrush **clone;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((_clone != 0));

	/* get this as a hatch brush */
	brush = (CHatchBrush *)_this;

	/* get the clone as a hatch brush */
	clone = (CHatchBrush **)_clone;

	/* create the clone */
	return
		CHatchBrush_Create
			(clone, brush->style, brush->foreground, brush->background);
}

/* Finalize this hatch brush. */
static void
CHatchBrush_Finalize(CBrush *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* nothing to do here */
}

/* Create a pattern for this brush. */
static CStatus
CHatchBrush_CreatePattern(CBrush   *_this,
                          CPattern *pattern)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));

	/* create the pattern */
	{
		/* declarations */
		CHatchBrush *brush;

		/* get this as a hatch brush */
		brush = (CHatchBrush *)_this;

		/* set the pattern transformation */
		pattern->transform = 0;

		/* create the hatch pattern */
		CStatus_Check
			(CUtils_CreateHatchPattern
				(&pattern->image,
				 CHatchBrush_Styles[brush->style],
				 CHatchBrush_StyleWidth,
				 CHatchBrush_StyleHeight,
				 brush->foreground,
				 brush->background,
				 1));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Create a hatch brush. */
CStatus
CHatchBrush_Create(CHatchBrush **_this,
                   CHatchStyle   style,
                   CColor        foreground,
                   CColor        background)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a valid hatch style */
	CStatus_Require((CHatchStyle_IsValid(style)), CStatus_Argument);

	/* allocate the brush */
	if(!(*_this = (CHatchBrush *)CMalloc(sizeof(CHatchBrush))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the members */
	(*_this)->style      = style;
	(*_this)->foreground = foreground;
	(*_this)->background = background;

	/* initialize the base */
	CBrush_Initialize((CBrush *)(*_this), &CHatchBrush_Class);

	/* return successfully */
	return CStatus_OK;
}

/* Get the background color of the hatch. */
CStatus
CHatchBrush_GetBackgroundColor(CHatchBrush *_this,
                               CColor      *background)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a background color pointer */
	CStatus_Require((background != 0), CStatus_ArgumentNull);

	/* get the background color */
	*background = _this->background;

	/* return successfully */
	return CStatus_OK;
}

/* Get the foreground color of the hatch. */
CStatus
CHatchBrush_GetForegroundColor(CHatchBrush *_this,
                               CColor      *foreground)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a foreground color pointer */
	CStatus_Require((foreground != 0), CStatus_ArgumentNull);

	/* get the foreground color */
	*foreground = _this->foreground;

	/* return successfully */
	return CStatus_OK;
}

/* Get the style of the hatch. */
CStatus
CHatchBrush_GetHatchStyle(CHatchBrush *_this,
                          CHatchStyle *style)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a style pointer */
	CStatus_Require((style != 0), CStatus_ArgumentNull);

	/* get the style */
	*style = _this->style;

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
