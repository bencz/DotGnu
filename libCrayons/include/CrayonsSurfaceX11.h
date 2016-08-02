/*
 * CrayonsSurfaceX11.h - X11 surface header.
 *
 * Copyright (C) 2006  Free Software Foundation, Inc.
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

#ifndef _CRAYONS_SURFACE_X11_H_
#define _CRAYONS_SURFACE_X11_H_

#include <Crayons.h>
#ifndef CRAYONS_HAVE_X11_SURFACE
	#error "Crayons was not built with X11 support."
#endif

#include <X11/Xlib.h>

#ifdef __cplusplus
extern "C" {
#endif

CStatus
CX11Surface_Create(CX11Surface **_this,
                   Display      *dpy,
                   Drawable      drawable,
                   Screen       *screen,
                   Visual       *visual,
                   CUInt32       width,
                   CUInt32       height);

#ifdef __cplusplus
};
#endif

#endif /* _CRAYONS_SURFACE_X11_H_ */
