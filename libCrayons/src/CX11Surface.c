/*
 * CX11Surface.c - X11 surface implementation.
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

#include "CX11Surface.h"
#include "CUtils.h"
#include <X11/Xutil.h>

#ifdef __cplusplus
extern "C" {
#endif

/* TODO: use XRender when available */

static CStatus
CX11Surface_GetImages(CX11Surface     *_this,
                      CUInt32          x,
                      CUInt32          y,
                      CUInt32          width,
                      CUInt32          height,
                      XImage         **imageX,
                      pixman_image_t **imageP)
{
	/* declarations */
	Pixmap           pixmap;
	pixman_format_t *format;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((imageX != 0));
	CASSERT((imageP != 0));

	/* create the pixmap */
	pixmap =
		XCreatePixmap
			(_this->dpy, _this->drawable, width, height, _this->depth);

	/* ensure we have a graphics context */
	if(_this->gc == 0)
	{
		/* declarations */
		XGCValues v;

		/* disable expose events */
		v.graphics_exposures = False;

		/* create the graphics context */
		_this->gc =
			XCreateGC
				(_this->dpy, _this->drawable, GCGraphicsExposures, &v);
	}

	/* copy the surface to the pixmap */
	XCopyArea
		(_this->dpy, _this->drawable, pixmap, _this->gc,
		 x, y, width, height, 0, 0);

	/* get the image data */
	*imageX =
		XGetImage
			(_this->dpy, pixmap, 0, 0, width, height, AllPlanes, ZPixmap);

	/* dispose of the pixmap */
	XFreePixmap(_this->dpy, pixmap);

	/* ensure we have the image data */
	CStatus_Require((*imageX != 0), CStatus_OutOfMemory);

	/* create the pixman format */
	format =
		pixman_format_create_masks
			((*imageX)->bits_per_pixel, 0, _this->visual->red_mask,
			 _this->visual->green_mask, _this->visual->blue_mask);

	/* ensure we have a format */
	if(format == 0)
	{
		XDestroyImage(*imageX);
		return CStatus_OutOfMemory;
	}

	/* create the pixman image */
	*imageP =
		pixman_image_create_for_data
			((pixman_bits_t *)(*imageX)->data, format, (*imageX)->width,
			 (*imageX)->height, (*imageX)->bits_per_pixel,
			 (*imageX)->bytes_per_line);

	/* dispose of the format */
	pixman_format_destroy(format);

	/* ensure we have an image */
	if(imageP == 0)
	{
		XDestroyImage(*imageX);
		return CStatus_OutOfMemory;
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CX11Surface_Composite(CSurface          *_this,
                      CUInt32            x,
                      CUInt32            y,
                      CUInt32            width,
                      CUInt32            height,
                      pixman_image_t    *src,
                      pixman_image_t    *mask,
                      pixman_operator_t  op)
{
	/* declarations */
	CX11Surface *surface;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((src   != 0));

	/* get this as an x surface */
	surface = (CX11Surface *)_this;

	/* perform the composite */
	{
		/* declarations */
		XImage         *imageX;
		pixman_image_t *imageP;

		/* create the images */
		CStatus_Check
			(CX11Surface_GetImages
				(surface, x, y, width, height, &imageX, &imageP));

		/* perform the composite */
		pixman_composite
			(op, src, mask, imageP, 0, 0, 0, 0, 0, 0, width, height);

		/* dispose of the pixman image */
		pixman_image_destroy(imageP);

		/* push the image data back to the server */
		XPutImage
			(surface->dpy, surface->drawable, surface->gc, imageX, 0, 0,
			 x, y, imageX->width, imageX->height);

		/* dispose of the x image */
		XDestroyImage(imageX);
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CX11Surface_Clear(CSurface *_this,
                  CColor    color)
{
	/* declarations */
	CX11Surface *surface;

	/* assertions */
	CASSERT((_this != 0));

	/* get this as an x surface */
	surface = (CX11Surface *)_this;

	/* clear the surface */
	{
		/* declarations */
		XImage          *imageX;
		pixman_image_t  *imageP;
		pixman_color_t   pixel;

		/* create the pixel */
		pixel = CUtils_ToPixmanColor(color);

		/* create the images */
		CStatus_Check
			(CX11Surface_GetImages
				(surface, 0, 0, _this->width, _this->height, &imageX, &imageP));

		/* perform the clear */
		pixman_fill_rectangle
			(PIXMAN_OPERATOR_SRC, imageP, &pixel, _this->x, _this->y,
			 _this->width, _this->height);

		/* dispose of the pixman image */
		pixman_image_destroy(imageP);

		/* push the image data back to the server */
		XPutImage
			(surface->dpy, surface->drawable, surface->gc, imageX, 0, 0,
			 _this->x, _this->y, imageX->width, imageX->height);

		/* dispose of the x image */
		XDestroyImage(imageX);
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CX11Surface_Flush(CSurface        *_this,
                  CFlushIntention  intention)
{
	/* declarations */
	CX11Surface *surface;

	/* assertions */
	CASSERT((_this != 0));

	/* get this as an x surface */
	surface = (CX11Surface *)_this;

	/* flush the surface */
	{
		if(intention == CFlushIntention_Flush)
		{
			XFlush(surface->dpy);
		}
		else
		{
			XSync(surface->dpy, False);
		}
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CX11Surface_GetDpiX(CSurface *_this,
                    CFloat   *dpiX)
{
	/* declarations */
	CX11Surface *surface;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((dpiX  != 0));

	/* get this as an x surface */
	surface = (CX11Surface *)_this;

	/* get the horizontal resolution */
	*dpiX = surface->dpiX;

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CX11Surface_GetDpiY(CSurface *_this,
                    CFloat   *dpiY)
{
	/* declarations */
	CX11Surface *surface;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((dpiY  != 0));

	/* get this as an x surface */
	surface = (CX11Surface *)_this;

	/* get the vertical resolution */
	*dpiY = surface->dpiY;

	/* return successfully */
	return CStatus_OK;
}

static void
CX11Surface_Finalize(CSurface *_this)
{
	/* declarations */
	CX11Surface *surface;

	/* assertions */
	CASSERT((_this != 0));

	/* get this as an x surface */
	surface = (CX11Surface *)_this;

	/* finalize the graphics context, as needed */
	if(surface->gc != 0)
	{
		XFreeGC(surface->dpy, surface->gc);
	}
}

CStatus
CX11Surface_Create(CX11Surface **_this,
                   Display      *dpy,
                   Drawable      drawable,
                   Screen       *screen,
                   Visual       *visual,
                   CUInt32       width,
                   CUInt32       height)
{
	/* declarations */
	CX11Surface *surface;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a display pointer */
	CStatus_Require((dpy != 0), CStatus_ArgumentNull);

	/* ensure we have a screen pointer */
	CStatus_Require((screen != 0), CStatus_ArgumentNull);

	/* ensure we have a visual pointer */
	CStatus_Require((visual != 0), CStatus_ArgumentNull);

	/* allocate the surface */
	if(!(*_this = (CX11Surface *)CMalloc(sizeof(CX11Surface))))
	{
		return CStatus_OutOfMemory;
	}

	/* get this as an x surface */
	surface = ((CX11Surface *)(*_this));

	/* initialize the base */
	{
		/* declarations */
		CStatus   status;
		CSurface *surface;

		/* get this as a surface */
		surface = (CSurface *)*_this;

		/* initialize the base */
		status =
			CSurface_Initialize
				(surface, &CX11Surface_Class, 0, 0, width, height);

		/* handle base initialization failures */
		if(status != CStatus_OK)
		{
			CFree(*_this);
			*_this = 0;
			return status;
		}
	}

	/* initialize the members */
	surface->dpy         = dpy;
	surface->screen      = screen;
	surface->visual      = visual;
	surface->gc          = 0;
	surface->drawable    = drawable;

	/* get the resolution */
	{
		/* declarations */
		CDouble px;
		CDouble mm;

		/* get the pixel width and millimeter width of the screen */
		px = XWidthOfScreen(screen);
		mm = XWidthMMOfScreen(screen);

		/* set the horizontal resolution */
		surface->dpiX = (CFloat)((px * 25.4) / mm);

		/* get the pixel height and millimeter height of the screen */
		px = XHeightOfScreen(screen);
		mm = XHeightMMOfScreen(screen);

		/* set the vertical resolution */
		surface->dpiY = (CFloat)((px * 25.4) / mm);
	}

	/* get the depth */
	{
		/* declarations */
		Depth *currD;
		Depth *endD;

		/* get the depth pointer */
		currD = screen->depths;

		/* get the end depth pointer */
		endD = (currD + screen->ndepths);

		/* search for the matching depth */
		while(currD != endD)
		{
			/* declarations */
			Visual *currV;
			Visual *endV;

			/* get the visual pointer */
			currV = currD->visuals;

			/* get the end visual pointer */
			endV = (currV + currD->nvisuals);

			/* search for the matching visual */
			while(currV != endV)
			{
				/* set the depth, if the visual matches */
				if(currV == visual)
				{
					/* get the depth */
					surface->depth = currD->depth;

					/* break out of loops */
					currD = (endD - 1);
					currV = (endV - 1);
				}

				/* move to next visual position */
				++currV;
			}

			/* move to next depth position */
			++currD;
		}
	}

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
