/*
 * sampleX11.c - Crayons sample X11 application.
 *
 * Copyright (C) 2006  Free Software Foundation, Inc.
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

#include <CrayonsSurfaceX11.h>
#include <stdio.h>

#include "CrayonsConfig.h"
#if STDC_HEADERS
	#include <stdlib.h>
	#include <stddef.h>
#elif HAVE_STDLIB_H
	#include <stdlib.h>
#endif

#ifdef __cplusplus
extern "C" {
#endif

typedef enum SampleMouseButton
{
	SampleMouseButton_Left   = 0,
	SampleMouseButton_Middle = 1,
	SampleMouseButton_Right  = 2
} SampleMouseButton;

typedef enum SampleMouseWheel
{
	SampleMouseWheel_Up   = 0,
	SampleMouseWheel_Down = 1
} SampleMouseWheel;

#define SampleWindowFlag_Quit 1

typedef struct _tagSampleWindow SampleWindow;
struct _tagSampleWindow
{
	Window    window;
	Display  *dpy;
	CSurface *surface;
	CUInt32   width;
	CUInt32   height;
	CUInt32   flags;
};

static void
SampleWindow_Initialize(SampleWindow *_this,
                        CUInt32       x,
                        CUInt32       y,
                        CUInt32       width,
                        CUInt32       height);
static void
SampleWindow_Finalize(SampleWindow *_this);
static void
SampleWindow_MouseDown(SampleWindow      *_this,
                       SampleMouseButton  button,
                       CUInt32            x,
                       CUInt32            y);
static void
SampleWindow_MouseUp(SampleWindow      *_this,
                     SampleMouseButton  button,
                     CUInt32            x,
                     CUInt32            y);
static void
SampleWindow_MouseWheel(SampleWindow     *_this,
                        SampleMouseWheel  direction,
                        CUInt32           x,
                        CUInt32           y);
static void
SampleWindow_Paint(SampleWindow *_this);
static void
SampleWindow_Resize(SampleWindow *_this,
                    CUInt32       width,
                    CUInt32       height);
static void
SampleWindow_Run(SampleWindow *_this);

/* Entry point. */
int
main(int argc, char *argv[])
{
	/* declarations */
	SampleWindow sample;

	/* initialize the sample window */
	SampleWindow_Initialize(&sample, 150, 150, 250, 250);

	/* run the sample window */
	SampleWindow_Run(&sample);

	/* finalize the sample window */
	SampleWindow_Finalize(&sample);

	/* exit */
	return 0;
}

/* Initialize a sample window. */
static void
SampleWindow_Initialize(SampleWindow *_this,
                        CUInt32       x,
                        CUInt32       y,
                        CUInt32       width,
                        CUInt32       height)
{
	/* declarations */
	CStatus  status;
	Screen  *screen;
	Visual  *visual;

	/* open the default display */
	if(!(_this->dpy = XOpenDisplay(getenv("DISPLAY"))))
	{
		fprintf
			(stderr, "ERROR: unable to open display (%s)\n", getenv("DISPLAY"));
		exit(1);
	}

	/* get the default screen */
	screen = XScreenOfDisplay(_this->dpy, XDefaultScreen(_this->dpy));

	/* get the default visual */
	visual = XDefaultVisualOfScreen(screen);

	/* create the window */
	_this->window =
		XCreateSimpleWindow
			(_this->dpy, XRootWindowOfScreen(screen), x, y, width, height, 2,
			 XBlackPixelOfScreen(screen), XWhitePixelOfScreen(screen));

	/* initialize the width and height */
	_this->width  = width;
	_this->height = height;

	/* initialize the flags */
	_this->flags = 0;

	/* create the surface */
	status =
		CX11Surface_Create
			((CX11Surface **)&(_this->surface),
			 _this->dpy,
			 _this->window,
			 screen,
			 visual,
			 width,
			 height);

	/* handle surface creation failures */
	if(status != CStatus_OK)
	{
		XDestroyWindow(_this->dpy, _this->window);
		XCloseDisplay(_this->dpy);
		fprintf(stderr, "ERROR: crayons surface creation failed\n");
		exit(2);
	}

	/* prepare for events */
	XSelectInput
		(_this->dpy, _this->window,
		 (ExposureMask |
		  StructureNotifyMask |
		  ButtonPressMask |
		  ButtonReleaseMask));
}

/* Finalize a sample window. */
static void
SampleWindow_Finalize(SampleWindow *_this)
{
	/* destroy the surface */
	CSurface_Destroy(&(_this->surface));

	/* destroy the window */
	XDestroyWindow(_this->dpy, _this->window);

	/* close the display */
	XCloseDisplay(_this->dpy);
}

/* Handle paint events. */
static void
SampleWindow_Paint(SampleWindow *_this)
{
	/* declarations */
	CGraphics   *gc;
	CBrush      *black;
	CBrush      *white;
	CPen        *wide;
	CPen        *thin;
	CRectangleF  rect;
	CPointF      pts[2];
	int          err;

	/* error messages */
	static const char *errors[] =
	{
		"crayons graphics context creation failed",
		"crayons black brush creation failed",
		"crayons white brush creation failed",
		"crayons wide pen creation failed",
		"crayons thin pen creation failed",
		"crayons white rectangle fill failed",
		"crayons black rectangle fill failed",
		"crayons wide line draw failed",
		"crayons thin line draw failed",
	};

	/* set the error number to the default */
	err = 0;

	/* status checking */
	#define _SWPaint_StatusCheck(status, num, target) \
		do { if((status) != CStatus_OK) { err = num; goto GOTO_Cleanup ## target; } } while(0)

	/* create the graphics context */
	_SWPaint_StatusCheck(CGraphics_Create(&gc, _this->surface), 1, A);

	/* create the black brush */
	_SWPaint_StatusCheck
		(CSolidBrush_Create((CSolidBrush **)(void *)&black, 0xFF000000), 2, B);

	/* create the white brush */
	_SWPaint_StatusCheck
		(CSolidBrush_Create((CSolidBrush **)(void *)&white, 0xFFFFFFFF), 3, C);

	/* create the wide pen */
	_SWPaint_StatusCheck(CPen_Create(&wide, black, 2), 4, D);

	/* create the thin pen */
	_SWPaint_StatusCheck(CPen_Create(&thin, black, -1), 5, E);

	/* fill a rectangle with white */
	rect.x      = 0;
	rect.y      = 0;
	rect.width  = _this->width;
	rect.height = _this->height;
	_SWPaint_StatusCheck(CGraphics_FillRectangle(gc, white, rect), 6, F);

	/* fill a rectangle with black */
	rect.x      = rect.width  / 4;
	rect.y      = rect.height / 4;
	rect.width  = rect.width  / 2;
	rect.height = rect.height / 2;
	_SWPaint_StatusCheck(CGraphics_FillRectangle(gc, black, rect), 7, F);

	/* draw a wide line */
	pts[0].x = 0;
	pts[0].y = 0;
	pts[1].x = _this->width;
	pts[1].y = _this->height;
	_SWPaint_StatusCheck(CGraphics_DrawLine(gc, wide, pts[0], pts[1]), 8, F);

	/* draw a thin line */
	pts[0].x = _this->width;
	pts[0].y = 0;
	pts[1].x = 0;
	pts[1].y = _this->height;
	_SWPaint_StatusCheck(CGraphics_DrawLine(gc, thin, pts[0], pts[1]), 9, F);

	/* handle failures */
	{
	GOTO_CleanupF:
		/* destroy the thin pen */
		CPen_Destroy(&thin);

	GOTO_CleanupE:
		/* destroy the wide pen */
		CPen_Destroy(&wide);

	GOTO_CleanupD:
		/* destroy the white brush */
		CBrush_Destroy(&white);

	GOTO_CleanupC:
		/* destroy the black brush */
		CBrush_Destroy(&black);

	GOTO_CleanupB:
		/* destroy the graphics context */
		CGraphics_Destroy(&gc);

	GOTO_CleanupA:
		if(err)
		{
			/* finalize the sample window */
			SampleWindow_Finalize(_this);

			/* print error message */
			fprintf(stderr, "ERROR: %s\n", errors[err - 1]);

			/* exit with failure status */
			exit(err + 2);
		}
	}
}

/* Handle mouse down events. */
static void
SampleWindow_MouseDown(SampleWindow      *_this,
                       SampleMouseButton  button,
                       CUInt32            x,
                       CUInt32            y)
{
	/* nothing to do here */
}

/* Handle mouse up events. */
static void
SampleWindow_MouseUp(SampleWindow      *_this,
                     SampleMouseButton  button,
                     CUInt32            x,
                     CUInt32            y)
{
	if(button == SampleMouseButton_Right)
	{
		_this->flags |= SampleWindowFlag_Quit;
	}
}

/* Handle mouse wheel events. */
static void
SampleWindow_MouseWheel(SampleWindow     *_this,
                        SampleMouseWheel  direction,
                        CUInt32           x,
                        CUInt32           y)
{
	/* nothing to do here */
}

/* Handle resize events. */
static void
SampleWindow_Resize(SampleWindow *_this,
                    CUInt32       width,
                    CUInt32       height)
{
	/* update the width and height */
	_this->width  = width;
	_this->height = height;

	/* update the surface bounds */
	CSurface_SetBounds(_this->surface, 0, 0, width, height);
}

/* Run a sample window. */
static void
SampleWindow_Run(SampleWindow *_this)
{
	/* initialize the quit flag */
	_this->flags &= ~SampleWindowFlag_Quit;

	/* map the window to the display */
	XMapWindow(_this->dpy, _this->window);

	/* handle events */
	while((_this->flags & SampleWindowFlag_Quit) == 0)
	{
		/* declarations */
		XEvent event;

		/* get the current event */
		XNextEvent(_this->dpy, &event);

		/* handle the current event */
		switch(event.type)
		{
			case Expose:
			{
				/* skip past extra expose events */
				if(event.xexpose.count > 0) { continue; }

				/* paint the window */
				SampleWindow_Paint(_this);
			}
			break;
			case ConfigureNotify:
			{
				/* get the size */
				CUInt32 width  = (CUInt32)event.xconfigure.width;
				CUInt32 height = (CUInt32)event.xconfigure.height;

				/* resize the window */
				SampleWindow_Resize(_this, width, height);
			}
			break;
			case ButtonPress:
			{
				/* get the position */
				CUInt32 x = (CUInt32)event.xbutton.x;
				CUInt32 y = (CUInt32)event.xbutton.y;

				/* make this easier to read */
				#define _SWRun_MouseDown(button) \
					(SampleWindow_MouseDown \
						(_this, SampleMouseButton_ ## button, x, y))

				/* handle mouse down events */
				switch(event.xbutton.button)
				{
					case Button1: { _SWRun_MouseDown(Left);   } break;
					case Button2: { _SWRun_MouseDown(Middle); } break;
					case Button3: { _SWRun_MouseDown(Right);  } break;
				}
			}
			break;
			case ButtonRelease:
			{
				/* get the position */
				CUInt32 x = (CUInt32)event.xbutton.x;
				CUInt32 y = (CUInt32)event.xbutton.y;

				/* make this easier to read */
				#define _SWRun_MouseUp(button) \
					(SampleWindow_MouseUp \
						(_this, SampleMouseButton_ ## button, x, y))

				/* make this easier to read */
				#define _SWRun_MouseWheel(direction) \
					(SampleWindow_MouseWheel \
						(_this, SampleMouseWheel_ ## direction, x, y))

				/* handle mouse up and mouse wheel events */
				switch(event.xbutton.button)
				{
					case Button1: { _SWRun_MouseUp(Left);    } break;
					case Button2: { _SWRun_MouseUp(Middle);  } break;
					case Button3: { _SWRun_MouseUp(Right);   } break;
					case Button4: { _SWRun_MouseWheel(Up);   } break;
					case Button5: { _SWRun_MouseWheel(Down); } break;
				}
			}
			break;
			default: break;
		}
	}

	/* unmap the window from the display */
	XUnmapWindow(_this->dpy, _this->window);
}


#ifdef __cplusplus
};
#endif
