/*
 * CX11Surface.h - X11 surface header.
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

#ifndef _C_X11SURFACE_H_
#define _C_X11SURFACE_H_

#include "CrayonsSurfaceX11.h"
#include "CSurface.h"

#ifdef __cplusplus
extern "C" {
#endif

struct _tagCX11Surface
{
	CSurface  _base;
	Display  *dpy;
	Screen   *screen;
	Visual   *visual;
	GC        gc;
	Drawable  drawable;
	CFloat    dpiX;
	CFloat    dpiY;
	int       depth;
};

static CStatus
CX11Surface_Composite(CSurface          *_this,
                      CUInt32            x,
                      CUInt32            y,
                      CUInt32            width,
                      CUInt32            height,
                      pixman_image_t    *src,
                      pixman_image_t    *mask,
                      pixman_operator_t  op);
static CStatus
CX11Surface_Clear(CSurface *_this,
                  CColor    color);
static CStatus
CX11Surface_Flush(CSurface        *_this,
                  CFlushIntention  intention);
static CStatus
CX11Surface_GetDpiX(CSurface *_this,
                    CFloat   *dpiX);
static CStatus
CX11Surface_GetDpiY(CSurface *_this,
                    CFloat   *dpiY);
static void
CX11Surface_Finalize(CSurface *_this);


static const CSurfaceClass CX11Surface_Class =
{
	CX11Surface_Composite,
	CX11Surface_Clear,
	CX11Surface_Flush,
	CX11Surface_GetDpiX,
	CX11Surface_GetDpiY,
	CX11Surface_Finalize,
	"sentinel"
};

#ifdef __cplusplus
};
#endif

#endif /* _C_X11SURFACE_H_ */
