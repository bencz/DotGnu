/*
 * CSolidBrush.c - Solid brush implementation.
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

#include "CSolidBrush.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Initialize this solid brush. */
static void
CSolidBrush_Initialize(CSolidBrush *_this,
                       CColor       color)
{
	/* assertions */
	CASSERT((_this != 0));

	/* intialize the members */
	_this->color = color;

	/* initialize the base */
	CBrush_Initialize((CBrush *)_this, &CSolidBrush_Class);
}

/* Finalize this solid brush. */
static void
CSolidBrush_Finalize(CBrush *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* nothing to do here */
}

/* Clone this solid brush. */
static CStatus
CSolidBrush_Clone(CBrush  *_this,
                  CBrush **_clone)
{
	/* declarations */
	CSolidBrush  *brush;
	CSolidBrush **clone;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((_clone != 0));

	/* get this as a solid brush */
	brush = (CSolidBrush  *)_this;

	/* get the clone as a solid brush */
	clone = (CSolidBrush **)_clone;

	/* clone this brush */
	return CSolidBrush_Create(clone, brush->color);
}

/* Create a pattern for this brush. */
static CStatus
CSolidBrush_CreatePattern(CBrush   *_this,
                          CPattern *pattern)
{
	/* declarations */
	CSolidBrush *brush;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));

	/* get this as a solid brush */
	brush = (CSolidBrush *)_this;

	/* set the pattern transformation */
	pattern->transform = 0;

	/* create the pattern */
	return CUtils_CreateSolidPattern(&(pattern->image), brush->color);
}

/* Create a solid brush. */
CStatus
CSolidBrush_Create(CSolidBrush **_this,
                   CColor        color)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the brush */
	if(!(*_this = (CSolidBrush *)CMalloc(sizeof(CSolidBrush))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the brush */
	CSolidBrush_Initialize(*_this, color);

	/* return successfully */
	return CStatus_OK;
}

/* Get the color of this brush. */
CStatus
CSolidBrush_GetColor(CSolidBrush *_this,
                     CColor      *color)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a color pointer */
	CStatus_Require((color != 0), CStatus_ArgumentNull);

	/* get the color */
	*color = _this->color;

	/* return successfully */
	return CStatus_OK;
}

/* Set the color of this brush. */
CStatus
CSolidBrush_SetColor(CSolidBrush *_this,
                     CColor       color)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the color */
	_this->color = color;

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
