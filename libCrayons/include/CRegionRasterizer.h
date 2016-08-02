/*
 * CRegionRasterizer.h - Region rasterizer header.
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

#ifndef _C_REGIONRASTERIZER_H_
#define _C_REGIONRASTERIZER_H_

#include "CRegionInterpreter.h"
#include "CFiller.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCRegionRasterizer CRegionRasterizer;
struct _tagCRegionRasterizer
{
	CRegionInterpreter  _base;
	pixman_format_t    *format;
	CAffineTransformF  *transform;
	CTrapezoids         trapezoids;
	CFiller             filler;
	CPointArrayF        array;
	CUInt32             width;
	CUInt32             height;
	CUInt32             size;
};

CINTERNAL CStatus
CRegionRasterizer_Initialize(CRegionRasterizer *_this,
                             CAffineTransformF *transform,
                             CFloat             width,
                             CFloat             height,
                             CBool              gray);
CINTERNAL void
CRegionRasterizer_Finalize(CRegionRasterizer *_this);

#ifdef __cplusplus
};
#endif

#endif /* _C_REGIONRASTERIZER_H_ */
