/*
 * XsharpSupport.c - C support code for Xsharp.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#if !defined(X_DISPLAY_MISSING) && HAVE_SELECT

#include "XsharpSupport.h"


int XSharpSupportPresent(void)
{
	return 1;
}

int XNextEventWithTimeout(Display *dpy, XEvent *event, int timeout)
{
	int fd = ConnectionNumber(dpy);
	fd_set readSet;
	struct timeval tv;
	int result;

	/* Initialize the read set */
	FD_ZERO(&readSet);
	FD_SET(fd, &readSet);

	/* Select on the display connection */
	if(timeout >= 0)
	{
		tv.tv_sec = timeout / 1000;
		tv.tv_usec = (timeout % 1000) * 1000;
		result = select(fd + 1, &readSet, (fd_set *)0, (fd_set *)0, &tv);
	}
	else
	{
		result = select(fd + 1, &readSet, (fd_set *)0, (fd_set *)0,
					    (struct timeval *)0);
	}

	/* If there was activity on the connection, then read the event */
	if(result > 0)
	{
		if( XPending( dpy ) <= 0 ) {
			// printf( "??? XPending<=0 ???" );
			result = 0; // no event is here to process, even select told that there should be an event
		}
		else {
			XNextEvent(dpy, event);
		}
	}

	/* Return the final result to the caller */
	return result;
}

/*
 * Determine if we can use the Xft extension.
 */
int XSharpUseXft()
{
#ifdef USE_XFT_EXTENSION
	return 1;
#else
	return 0;
#endif
}

/*
 * Try to create a font with specific parameters.
 */
static void *TryCreateFont(Display *dpy, const char *family,
						   int pointSize, int style)
{
	XFontSet fontSet;
	char *name;
	const char *weight;
	const char *slant;
	char **missingCharsets;
	int missingCharsetCount;
	char *defStringReturn;

	/* Create a buffer big enough to hold the base XLFD */
	name = (char *)malloc((family ? strlen(family) : 1) + 128);
	if(!name)
	{
		return 0;
	}

	/* Determine the weight and slant to use */
	if((style & FontStyle_Bold) != 0)
	{
		weight = "bold";
	}
	else
	{
		weight = "medium";
	}
	if((style & FontStyle_Italic) != 0)
	{
		slant = "i";
	}
	else
	{
		slant = "r";
	}

	/* Create the base XLFD */
	if(pointSize != -1)
	{
		sprintf(name, "-*-%s-%s-%s-*-*-*-%d-*-*-*-*-*-*",
				(family ? family : "*"), weight, slant, pointSize);
	}
	else
	{
		sprintf(name, "-*-%s-%s-%s-*-*-*-*-*-*-*-*-*-*",
				(family ? family : "*"), weight, slant);
	}

	/* Use the Latin1 XFontStruct creation method if requested */
	if((style & FontStyle_FontStruct) != 0)
	{
		XFontStruct *fs;
		fs = XLoadQueryFont(dpy, name);
		if(fs)
		{
			free(name);
			return (void *)fs;
		}
		else
		{
			free(name);
			return 0;
		}
	}

	/* Try to create the font set using just the base name */
	missingCharsets = 0;
	missingCharsetCount = 0;
	defStringReturn = 0;
	fontSet = XCreateFontSet(dpy, name, &missingCharsets,
							 &missingCharsetCount, &defStringReturn);
	if(fontSet)
	{
		free(name);
		return (void *)fontSet;
	}
	/* TODO: process the missing charsets */

	/* Give up - we couldn't create this variant */
	free(name);
	return 0;
}

/*
 * Create a font set from a description.
 */
void *XSharpCreateFontSet(Display *dpy, const char *family,
					      int pointSize, int style)
{
	XFontSet fontSet;
	int structStyle = (style & FontStyle_FontStruct);

	/* Try with the actual parameters first */
	fontSet = TryCreateFont(dpy, family, pointSize, style);
	if(fontSet)
	{
		return fontSet;
	}

	/* Remove the style, but keep the family */
	fontSet = TryCreateFont(dpy, family, pointSize,
							FontStyle_Normal | structStyle);
	if(fontSet)
	{
		return fontSet;
	}

	/* Remove the family, but keep the style */
	if((style & FontStyle_NoDefault) == 0)
	{
		fontSet = TryCreateFont(dpy, "fixed", pointSize, style);
		if(fontSet)
		{
			return fontSet;
		}
	}

	/* Remove the point size and the style, but keep the family */
	fontSet = TryCreateFont(dpy, family, -1, FontStyle_Normal | structStyle);
	if(fontSet)
	{
		return fontSet;
	}

	/* Remove everything - this will succeed unless X has no fonts at all! */
	if((style & FontStyle_NoDefault) != 0)
	{
		return 0;
	}
	return TryCreateFont(dpy, "fixed", -1, FontStyle_Normal | structStyle);
}

/*
 * Create a font struct from a description.
 */
void *XSharpCreateFontStruct(Display *dpy, const char *family,
					         int pointSize, int style)
{
	return XSharpCreateFontSet(dpy, family, pointSize,
							   style | FontStyle_FontStruct);
}

/*
 * Create an Xft font from a description.
 */
void *XSharpCreateFontXft(Display *dpy, const char *family,
                          const char *fallbacks, int pointSize,
                          int style)
{
#ifdef USE_XFT_EXTENSION

	XftPattern *pattern;
	XftPattern *matched;
	XftFont *font;
	XftResult result;

	/* Create the font pattern to be used */
	pattern = XftPatternCreate();
	if(!pattern)
	{
		return 0;
	}
	if(!XftPatternAddString(pattern, XFT_FAMILY, family))
	{
		XftPatternDestroy(pattern);
		return 0;
	}
	if(fallbacks)
	{
		if(!XftPatternAddString(pattern, XFT_FAMILY, fallbacks))
		{
			XftPatternDestroy(pattern);
			return 0;
		}
	}
	if(!XftPatternAddDouble(pattern, XFT_SIZE, ((double)pointSize) / 10.0))
	{
		XftPatternDestroy(pattern);
		return 0;
	}
	if((style & FontStyle_Bold) != 0)
	{
		if(!XftPatternAddInteger(pattern, XFT_WEIGHT, XFT_WEIGHT_BOLD))
		{
			XftPatternDestroy(pattern);
			return 0;
		}
	}
	if((style & FontStyle_Italic) != 0)
	{
		if(!XftPatternAddInteger(pattern, XFT_SLANT, XFT_SLANT_ITALIC))
		{
			XftPatternDestroy(pattern);
			return 0;
		}
	}

	/* Perform font matching to find the closest possible font */
	matched = XftFontMatch(dpy, DefaultScreen(dpy), pattern, &result);
	XftPatternDestroy(pattern);
	if(!matched)
	{
		return 0;
	}

	/* Create an Xft font based on the matched pattern */
	return XftFontOpenPattern(dpy, matched);

#else /* !USE_XFT_EXTENSION */

	/* Don't have Xft support on this platform */
	return 0;

#endif /* !USE_XFT_EXTENSION */
}

/*
 * Free a font set.
 */
void XSharpFreeFontSet(Display *dpy, void *fontSet)
{
	XFreeFontSet(dpy, (XFontSet)fontSet);
}

/*
 * Free a font struct.
 */
void XSharpFreeFontStruct(Display *dpy, void *fontSet)
{
	XFreeFont(dpy, (XFontStruct *)fontSet);
}

/*
 * Free an Xft font.
 */
void XSharpFreeFont(Display *dpy, void *fontSet)
{
#ifdef USE_XFT_EXTENSION
	XftFontClose(dpy, (XftFont *)fontSet);
#endif
}

/*
 * Forward declarations.
 */
void XSharpTextExtentsSet(Display *dpy, void *fontSet, const char *str,
					      XRectangle *overall_ink_return,
					      XRectangle *overall_logical_return);
void XSharpTextExtentsStruct(Display *dpy, void *fontSet,
					         ILString *str, long offset, long count,
							 XRectangle *overall_ink_return,
					         XRectangle *overall_logical_return);
void XSharpTextExtentsXft(Display *dpy, void *fontSet, const char *str,
					      XRectangle *overall_ink_return,
					      XRectangle *overall_logical_return);

/*
 * Draw a string using a font set.
 */
void XSharpDrawStringSet(Display *dpy, Drawable drawable, GC gc,
					     void *fontSet, int x, int y,
					     const char *str, int style)
{
	XRectangle overall_ink_return;
	XRectangle overall_logical_return;
	XFontSetExtents *extents;
	int line1, line2;

	/* Draw the string using the core API */
	XmbDrawString(dpy, drawable, (XFontSet)fontSet, gc, x, y,
				  str, strlen(str));

	/* Calculate the positions of the underline and strike-out */
	if((style & FontStyle_Underline) != 0)
	{
		line1 = y + 1;
	}
	else
	{
		line1 = y;
	}
	if((style & FontStyle_StrikeOut) != 0)
	{
		extents = XExtentsOfFontSet((XFontSet)fontSet);
		if(extents)
		{
			line2 = y + (extents->max_logical_extent.y / 2);
		}
		else
		{
			line2 = y;
		}
	}
	else
	{
		line2 = y;
	}

	/* Draw the underline and strike-out */
	if(line1 != y || line2 != y)
	{
		XSharpTextExtentsSet(dpy, fontSet, str,
				 	         &overall_ink_return, &overall_logical_return);
		XSetLineAttributes(dpy, gc, 1, LineSolid, CapNotLast, JoinMiter);
		if(line1 != y)
		{
			XDrawLine(dpy, drawable, gc, x, line1,
					  x + overall_logical_return.width, line1);
		}
		if(line2 != y)
		{
			XDrawLine(dpy, drawable, gc, x, line2,
					  x + overall_logical_return.width, line2);
		}
	}
}

#define	XSHARP_BUFSIZ	128

/*
 * Draw a string using a font struct.
 */
void XSharpDrawStringStruct(Display *dpy, Drawable drawable, GC gc,
					        void *fontSet, int x, int y, ILString *str,
							long offset, long count, int style)
{
	XFontStruct *fs = (XFontStruct *)fontSet;
	ILChar *buffer = ILStringToBuffer(str) + offset;
	XRectangle overall_ink_return;
	XRectangle overall_logical_return;
	int line1, line2;
	ILChar ch;
	long ct;
	int posn;
	char latinBuffer[XSHARP_BUFSIZ];

	/* Draw the string using the core API */
	XSetFont(dpy, gc, fs->fid);
	ct = count;
	while(ct > 0)
	{
		/* Convert a run of text into Latin1 */
		posn = 0;
		while(ct > 0 && posn < XSHARP_BUFSIZ)
		{
			ch = *buffer++;
			--ct;
			if(ch < 0x0100)
			{
				latinBuffer[posn++] = (char)ch;
			}
			else
			{
				latinBuffer[posn++] = (char)'?';
			}
		}

		/* Draw the text run */
		XDrawString(dpy, drawable, gc, x, y, latinBuffer, posn);

		/* Advance past the text run so that we can draw the next one */
		if(ct > 0)
		{
			x += XTextWidth(fs, latinBuffer, posn);
		}
	}

	/* Calculate the positions of the underline and strike-out */
	if((style & FontStyle_Underline) != 0)
	{
		line1 = y + 1;
	}
	else
	{
		line1 = y;
	}
	if((style & FontStyle_StrikeOut) != 0)
	{
		line2 = y - (fs->ascent / 2);
	}
	else
	{
		line2 = y;
	}

	/* Draw the underline and strike-out */
	if(line1 != y || line2 != y)
	{
		XSharpTextExtentsStruct(dpy, fontSet, str, offset, count,
				 				&overall_ink_return, &overall_logical_return);
		XSetLineAttributes(dpy, gc, 1, LineSolid, CapNotLast, JoinMiter);
		if(line1 != y)
		{
			XDrawLine(dpy, drawable, gc, x, line1,
					  x + overall_logical_return.width, line1);
		}
		if(line2 != y)
		{
			XDrawLine(dpy, drawable, gc, x, line2,
					  x + overall_logical_return.width, line2);
		}
	}
}

/*
 * Draw a string using an Xft font.
 */
void XSharpDrawStringXft(Display *dpy, Drawable drawable, GC gc,
					     void *fontSet, int x, int y,
					     const char *str, int style, Region clipRegion,
					     unsigned long colorValue)
{
#ifdef USE_XFT_EXTENSION

	XRectangle overall_ink_return;
	XRectangle overall_logical_return;
	XftDraw *draw;
	XftColor color;
	XGCValues values;
	int line1, line2;

	/* Set up the Xft color value to draw with */
	XGetGCValues(dpy, gc, GCForeground, &values);
	color.pixel = values.foreground;
	color.color.red = (unsigned short)(((colorValue >> 16) & 0xFF) << 8);
	color.color.green = (unsigned short)(((colorValue >> 8) & 0xFF) << 8);
	color.color.blue = (unsigned short)((colorValue & 0xFF) << 8);
	color.color.alpha = (unsigned short)0xFFFF;

	/* Draw the string */
	draw = XftDrawCreate(dpy, drawable,
						 DefaultVisual(dpy, DefaultScreen(dpy)),
						 DefaultColormap(dpy, DefaultScreen(dpy)));
	if(draw)
	{
		if(clipRegion)
		{
			XftDrawSetClip(draw, clipRegion);
		}

		XftDrawStringUtf8(draw, &color, (XftFont *)fontSet,
			x, y, (XftChar8 *)str, strlen(str));
		
		XftDrawDestroy(draw);
	}

	/* Calculate the positions of the underline and strike-out */
	if((style & FontStyle_Underline) != 0)
	{
		line1 = y + 1;
	}
	else
	{
		line1 = y;
	}
	if((style & FontStyle_StrikeOut) != 0)
	{
		line2 = y + (((XftFont *)fontSet)->height / 2);
	}
	else
	{
		line2 = y;
	}

	/* Draw the underline and strike-out */
	if(line1 != y || line2 != y)
	{
		XSharpTextExtentsXft(dpy, fontSet, str,
				 	         &overall_ink_return, &overall_logical_return);
		XSetLineAttributes(dpy, gc, 1, LineSolid, CapNotLast, JoinMiter);
		if(line1 != y)
		{
			XDrawLine(dpy, drawable, gc, x, line1,
					  x + overall_logical_return.width, line1);
		}
		if(line2 != y)
		{
			XDrawLine(dpy, drawable, gc, x, line2,
					  x + overall_logical_return.width, line2);
		}
	}
#endif /* USE_XFT_EXTENSION */
}

/*
 * Calculate the extent information for a string in a font set.
 */
void XSharpTextExtentsSet(Display *dpy, void *fontSet, const char *str,
					      XRectangle *overall_ink_return,
					      XRectangle *overall_logical_return)
{
	XmbTextExtents((XFontSet)fontSet, str, strlen(str),
			 	   overall_ink_return, overall_logical_return);
}

/*
 * Some helper macros from the Xlib code.  Copyright by various authors
 * and distributed under the MIT/X11 license.  See the Xlib source for
 * further details (Xlibint.h and TextExt.c).
 */
#define CI_NONEXISTCHAR(cs) (((cs)->width == 0) && \
			     (((cs)->rbearing|(cs)->lbearing| \
			       (cs)->ascent|(cs)->descent) == 0))
#define CI_GET_CHAR_INFO_1D(fs,col,def,cs) \
{ \
    cs = def; \
    if (col >= fs->min_char_or_byte2 && col <= fs->max_char_or_byte2) { \
	if (fs->per_char == NULL) { \
	    cs = &fs->min_bounds; \
	} else { \
	    cs = &fs->per_char[(col - fs->min_char_or_byte2)]; \
	    if (CI_NONEXISTCHAR(cs)) cs = def; \
	} \
    } \
}
#define CI_GET_DEFAULT_INFO_1D(fs,cs) \
  CI_GET_CHAR_INFO_1D (fs, fs->default_char, NULL, cs)
#define CI_GET_CHAR_INFO_2D(fs,row,col,def,cs) \
{ \
    cs = def; \
    if (row >= fs->min_byte1 && row <= fs->max_byte1 && \
	col >= fs->min_char_or_byte2 && col <= fs->max_char_or_byte2) { \
	if (fs->per_char == NULL) { \
	    cs = &fs->min_bounds; \
	} else { \
	    cs = &fs->per_char[((row - fs->min_byte1) * \
			        (fs->max_char_or_byte2 - \
				 fs->min_char_or_byte2 + 1)) + \
			       (col - fs->min_char_or_byte2)]; \
	    if (CI_NONEXISTCHAR(cs)) cs = def; \
        } \
    } \
}
#define CI_GET_DEFAULT_INFO_2D(fs,cs) \
{ \
    unsigned int r = (fs->default_char >> 8); \
    unsigned int c = (fs->default_char & 0xff); \
    CI_GET_CHAR_INFO_2D (fs, r, c, NULL, cs); \
}
#define CI_GET_ROWZERO_CHAR_INFO_2D(fs,col,def,cs) \
{ \
    cs = def; \
    if (fs->min_byte1 == 0 && \
	col >= fs->min_char_or_byte2 && col <= fs->max_char_or_byte2) { \
	if (fs->per_char == NULL) { \
	    cs = &fs->min_bounds; \
	} else { \
	    cs = &fs->per_char[(col - fs->min_char_or_byte2)]; \
	    if (CI_NONEXISTCHAR(cs)) cs = def; \
	} \
    } \
}

/*
 * Calculate the extent information for a string in a font struct.
 *
 * This is based on the code for the "XTextExtents" function in Xlib.
 * We expand it inline so that we can convert from Unicode to Latin1 on
 * the fly, rather than having to convert into a separate buffer first.
 */
void XSharpTextExtentsStruct(Display *dpy, void *fontSet, ILString *str,
					         long offset, long count,
							 XRectangle *overall_ink_return,
					         XRectangle *overall_logical_return)
{
	ILChar *buffer = ILStringToBuffer(str) + offset;
	XFontStruct *fs = (XFontStruct *)fontSet;
	int isSingleRow = (fs->max_byte1 == 0);
	int first = 1;
	XCharStruct *defaultSize;
	XCharStruct *charSize;
	XCharStruct overall;
	unsigned ch;
	int temp;

	/* Get the metrics for the default character */
	if(isSingleRow)
	{
		CI_GET_DEFAULT_INFO_1D(fs, defaultSize);
	}
	else
	{
		CI_GET_DEFAULT_INFO_2D(fs, defaultSize);
	}

	/* Iterate over the characters and measure them */
	overall.width = 0;
	overall.ascent = 0;
	overall.descent = 0;
	overall.lbearing = 0;
	overall.rbearing = 0;
	while(count-- > 0)
	{
		ch = (unsigned)(*buffer++);
		if(ch >= 0x0100)
		{
			/* Convert non-Latin1 characters into '?' */
			ch = '?';
		}
		if(isSingleRow)
		{
	    	CI_GET_CHAR_INFO_1D(fs, ch, defaultSize, charSize);
		}
		else
		{
	    	CI_GET_ROWZERO_CHAR_INFO_2D(fs, ch, defaultSize, charSize);
		}
		if(charSize)
		{
			if(first)
			{
				overall = *charSize;
				first = 0;
			}
			else
			{
				if(charSize->ascent > overall.ascent)
				{
					overall.ascent = charSize->ascent;
				}
				if(charSize->descent > overall.descent)
				{
					overall.descent = charSize->descent;
				}
				temp = overall.width + charSize->lbearing;
				if(temp < overall.lbearing)
				{
					overall.lbearing = temp;
				}
				temp = overall.width + charSize->rbearing;
				if(temp > overall.rbearing)
				{
					overall.rbearing = temp;
				}
				overall.width += charSize->width;
		    }
		}
    }

	/* Convert the raw extent information into the desired return values */
	overall_ink_return->x = overall.lbearing;
	overall_ink_return->y = -(overall.ascent);
	overall_ink_return->width = overall.rbearing - overall.lbearing;
	overall_ink_return->height = overall.ascent + overall.descent;
	overall_logical_return->x = 0;
	overall_logical_return->y = -(fs->ascent);
	overall_logical_return->width = overall.width;
	overall_logical_return->height = fs->ascent + fs->descent;
}

/*
 * Calculate the extent information for a string in an Xft font.
 */
void XSharpTextExtentsXft(Display *dpy, void *fontSet, const char *str,
					      XRectangle *overall_ink_return,
					      XRectangle *overall_logical_return)
{
#ifdef USE_XFT_EXTENSION

	XGlyphInfo extents;
	
	XftTextExtentsUtf8(dpy, fontSet, (XftChar8 *)str, strlen(str), &extents);

	overall_ink_return->x = -(extents.x);
	overall_ink_return->y = -(extents.y);
	overall_ink_return->width = extents.width;
	overall_ink_return->height = extents.height;

	overall_logical_return->x = -(extents.x);
	overall_logical_return->y = -(extents.y);
	overall_logical_return->width = extents.x + extents.xOff;
	overall_logical_return->height = extents.y + extents.yOff;

#endif
}

/*
 * Calculate the extent information for a font set.
 */
void XSharpFontExtentsSet(void *fontSet,
					      XRectangle *max_ink_return,
					      XRectangle *max_logical_return)
{
	XFontSetExtents *extents;
	extents = XExtentsOfFontSet((XFontSet)fontSet);
	if(extents)
	{
		*max_ink_return = extents->max_ink_extent;
		*max_logical_return = extents->max_logical_extent;
	}
}

/*
 * Calculate the extent information for a font struct.
 */
void XSharpFontExtentsStruct(void *fontSet,
					         XRectangle *max_ink_return,
					         XRectangle *max_logical_return)
{
	XFontStruct *fs = (XFontStruct *)fontSet;
	max_ink_return->x = fs->min_bounds.lbearing;
	max_ink_return->y = -(fs->max_bounds.ascent);
	max_ink_return->width =
		fs->max_bounds.rbearing - fs->min_bounds.lbearing;
	max_ink_return->height =
		fs->max_bounds.ascent + fs->max_bounds.descent;
	max_logical_return->x = 0;
	max_logical_return->y = -(fs->ascent);
	max_logical_return->width = fs->max_bounds.width;
	max_logical_return->height = fs->ascent + fs->descent;
}

/*
 * Calculate the extent information for an Xft font.
 */
void XSharpFontExtentsXft(void *fontSet,
					      XRectangle *max_ink_return,
					      XRectangle *max_logical_return)
{
#ifdef USE_XFT_EXTENSION

	/* Synthesize enough information to keep "Xsharp.FontExtents" happy */
	max_logical_return->x = 0;
	max_logical_return->y = -(((XftFont *)fontSet)->ascent);
	max_logical_return->width = ((XftFont *)fontSet)->max_advance_width;
	max_logical_return->height = ((XftFont *)fontSet)->ascent +
								 ((XftFont *)fontSet)->descent;
	*max_ink_return = *max_logical_return;

#endif
}

/*
 * Get the contents of the RESOURCE_MANAGER property on the root window.
 */
char *XSharpGetResources(Display *dpy, Window root)
{
	Atom resourceManager;
	Atom actualTypeReturn;
	int actualFormatReturn;
	unsigned long nitemsReturn;
	unsigned long bytesAfterReturn;
	unsigned char *propReturn;

	/* Register the RESOURCE_MANAGER atom with the X server */
	resourceManager = XInternAtom(dpy, "RESOURCE_MANAGER", False);

	/* Get the property, stopping at 1k characters */
	propReturn = 0;
	nitemsReturn = 0;
	bytesAfterReturn = 0;
	XGetWindowProperty(dpy, root, resourceManager, 0, 1024, False,
					   XA_STRING, &actualTypeReturn, &actualFormatReturn,
					   &nitemsReturn, &bytesAfterReturn, &propReturn);
	if(bytesAfterReturn > 0)
	{
		/* We now know the real size, so fetch it properly this time */
		if(propReturn)
		{
			XFree((void *)propReturn);
		}
		propReturn = 0;
		XGetWindowProperty(dpy, root, resourceManager, 0,
						   (long)(1024 + bytesAfterReturn), False,
						   XA_STRING, &actualTypeReturn, &actualFormatReturn,
						   &nitemsReturn, &bytesAfterReturn, &propReturn);
	}
	return (char *)propReturn;
}

/*
 * Free a return value from "XSharpGetResources".
 */
void XSharpFreeResources(char *value)
{
	if(value)
	{
		XFree((void *)value);
	}
}

/*
 * Pixel formats, which must match "DotGNU.Images.PixelFormat".
 */
#define	PF_Undefined				0x00000000
#define	PF_DontCare					0x00000000
#define	PF_Max						0x0000000F
#define	PF_Indexed					0x00010000
#define	PF_Gdi						0x00020000
#define	PF_Format16bppRgb555		0x00021005
#define	PF_Format16bppRgb565		0x00021006
#define	PF_Format24bppRgb			0x00021808
#define	PF_Format32bppRgb			0x00022009
#define	PF_Format1bppIndexed		0x00030101
#define	PF_Format4bppIndexed		0x00030402
#define PF_Format8bppIndexed		0x00030803
#define PF_Alpha					0x00040000
#define PF_Format16bppArgb1555		0x00061007
#define PF_PAlpha					0x00080000
#define PF_Format32bppPArgb			0x000E200B
#define PF_Extended					0x00100000
#define PF_Format16bppGrayScale		0x00101004
#define PF_Format48bppRgb			0x0010300C
#define PF_Format64bppPArgb			0x001C400E
#define PF_Canonical				0x00200000
#define PF_Format32bppArgb			0x0026200A
#define PF_Format64bppArgb			0x0034400D

/*
 * Read the contents of a scan line as ARGB values.  The alpha component
 * can be omitted if the format does not include "PF_Alpha".
 */
static void Read_16bppRgb555(unsigned char *input,
							 unsigned long *output, int num)
{
	unsigned short value;
	while(num > 0)
	{
		value = *((unsigned short *)input);
		*output++ = (((unsigned long)(value & 0x7C00)) << 9) |
					(((unsigned long)(value & 0x03E0)) << 6) |
					(((unsigned long)(value & 0x001F)) << 3);
		input += 2;
		--num;
	}
}
static void Read_16bppRgb565(unsigned char *input,
							 unsigned long *output, int num)
{
	unsigned short value;
	while(num > 0)
	{
		value = *((unsigned short *)input);
		*output++ = (((unsigned long)(value & 0xF800)) << 8) |
					(((unsigned long)(value & 0x07E0)) << 5) |
					(((unsigned long)(value & 0x001F)) << 3);
		input += 2;
		--num;
	}
}
static void Read_16bppArgb1555(unsigned char *input,
							   unsigned long *output, int num)
{
	unsigned short value;
	while(num > 0)
	{
		value = *((unsigned short *)input);
		*output = (((unsigned long)(value & 0x7C00)) << 9) |
				  (((unsigned long)(value & 0x03E0)) << 6) |
				  (((unsigned long)(value & 0x001F)) << 3);
		if((value & 0x8000) != 0)
		{
			*output |= (unsigned long)0xFF000000;
		}
		input += 2;
		--num;
	}
}
static void Read_16bppGrayScale(unsigned char *input,
							    unsigned long *output, int num)
{
	unsigned short value;
	while(num > 0)
	{
		value = (*((unsigned short *)input) >> 8);
		*output++ = (((unsigned long)value) << 16) |
					(((unsigned long)value) << 8) |
					 ((unsigned long)value);
		input += 2;
		--num;
	}
}
static void Read_24bppRgb(unsigned char *input,
					      unsigned long *output, int num)
{
	while(num > 0)
	{
		*output++ = ((unsigned long)(input[0])) |
					(((unsigned long)(input[1])) << 8) |
					(((unsigned long)(input[2])) << 16);
		input += 3;
		--num;
	}
}
static void Read_32bppRgb(unsigned char *input,
					      unsigned long *output, int num)
{
	while(num > 0)
	{
		*output++ = ((unsigned long)(input[0])) |
					(((unsigned long)(input[1])) << 8) |
					(((unsigned long)(input[2])) << 16);
		input += 4;
		--num;
	}
}
static void Read_32bppArgb(unsigned char *input,
					       unsigned long *output, int num)
{
	while(num > 0)
	{
		*output++ = ((unsigned long)(input[0])) |
					(((unsigned long)(input[1])) << 8) |
					(((unsigned long)(input[2])) << 16) |
					(((unsigned long)(input[3])) << 24);
		input += 4;
		--num;
	}
}
static void Read_48bppRgb(unsigned char *input,
					      unsigned long *output, int num)
{
	while(num > 0)
	{
		*output++ = ((unsigned long)(input[1])) |
					(((unsigned long)(input[3])) << 8) |
					(((unsigned long)(input[5])) << 16);
		input += 6;
		--num;
	}
}
static void Read_64bppArgb(unsigned char *input,
					       unsigned long *output, int num)
{
	while(num > 0)
	{
		*output++ = ((unsigned long)(input[1])) |
					(((unsigned long)(input[3])) << 8) |
					(((unsigned long)(input[5])) << 16) |
					(((unsigned long)(input[7])) << 24);
		input += 8;
		--num;
	}
}

/*
 * Get the read function for a format.
 */
typedef void (*ReadFunc)(unsigned char *input, unsigned long *output, int num);
static ReadFunc GetReadFunc(int pixelFormat)
{
	switch(pixelFormat)
	{
		case PF_Format16bppRgb555:		return Read_16bppRgb555;
		case PF_Format16bppRgb565:		return Read_16bppRgb565;
		case PF_Format24bppRgb:			return Read_24bppRgb;
		case PF_Format32bppRgb:			return Read_32bppRgb;
		case PF_Format16bppArgb1555:	return Read_16bppArgb1555;
		case PF_Format32bppPArgb:		return Read_32bppArgb;
		case PF_Format16bppGrayScale:	return Read_16bppGrayScale;
		case PF_Format48bppRgb:			return Read_48bppRgb;
		case PF_Format64bppPArgb:		return Read_64bppArgb;
		case PF_Format32bppArgb:		return Read_32bppArgb;
		case PF_Format64bppArgb:		return Read_64bppArgb;
		default:						return 0;
	}
}

/*
 * Write RGB data to an indexed XImage structure.
 */
static void Write_Indexed(XImage *image, int line, unsigned long *input,
						  int num, unsigned long *palette)
{
	int column;
	unsigned long pixel;
	for(column = 0; column < num; ++column)
	{
		pixel = *input++;
		pixel = (((pixel & 0x00FF0000) >> 16) * 5 / 255) * 36 +
		        (((pixel & 0x0000FF00) >> 8) * 5 / 255) * 6 +
		        ((pixel & 0x000000FF) * 5 / 255);
		XPutPixel(image, column, line, palette[pixel]);
	}
}

/*
 * Write RGB data directly to an XImage.
 */
static void Write_Direct(Display *dpy, Colormap colormap, XImage *image,
						 int line, unsigned long *input, int num)
{
	int column;
	if(sizeof(unsigned int) == 4)
	{
		unsigned int *output = (unsigned int *)
			(image->data + line * image->bytes_per_line);
		for(column = 0; column < num; ++column)
		{
			*output++ = (unsigned int)(*input++);
		}
	}
	else
	{
		unsigned long *output = (unsigned long *)
			(image->data + line * image->bytes_per_line);
		for(column = 0; column < num; ++column)
		{
			*output++ = *input++;
		}
	}
}

/*
 * Write RGB data directly to an XImage, and perform an endian byteswap.
 */
static void Write_DirectSwap(Display *dpy, Colormap colormap, XImage *image,
						     int line, unsigned long *input, int num)
{
	int column;
	if(sizeof(unsigned int) == 4)
	{
		unsigned int *output = (unsigned int *)
			(image->data + line * image->bytes_per_line);
		for(column = 0; column < num; ++column)
		{
			unsigned int value = (unsigned int)(*input++);
			value = ((value & 0x00FF0000) >> 8) |
					((value & 0x0000FF00) << 8) |
					((value & 0x000000FF) << 24);
			*output++ = value;
		}
	}
	else
	{
		unsigned long *output = (unsigned long *)
			(image->data + line * image->bytes_per_line);
		for(column = 0; column < num; ++column)
		{
			unsigned int value = (unsigned int)(*input++);
			value = ((value & 0x00FF0000) >> 8) |
					((value & 0x0000FF00) << 8) |
					((value & 0x000000FF) << 24);
			*output++ = (unsigned long)value;
		}
	}
}

/*
 * Write RGB data to a 16 bits per pixel XImage structure.
 */
static void Write_16bpp(Display *dpy, Colormap colormap, XImage *image,
					    int line, unsigned long *input, int num)
{
	int column;
	unsigned short *output = (unsigned short *)
		(image->data + line * image->bytes_per_line);
	for(column = 0; column < num; ++column)
	{
		unsigned int value = (unsigned int)(*input++);
		value = ((value & 0x00F80000) >> 8) |
				((value & 0x0000FC00) >> 5) |
				((value & 0x000000F8) >> 3);
		*output++ = (unsigned short)value;
	}
}

/*
 * Default writing routine, doing per-pixel color allocations.
 */
static void Write_Default(Display *dpy, Colormap colormap, XImage *image,
						  int line, unsigned long *input, int num)
{
	int column;
	unsigned long rgb;
	XColor xcolor;
	for(column = 0; column < num; ++column)
	{
		rgb = *input++;
		xcolor.pixel = 0;
		xcolor.red = (unsigned short)((rgb >> 8) & 0xFF00);
		xcolor.green = (unsigned short)(rgb & 0xFF00);
		xcolor.blue = (unsigned short)((rgb << 8) & 0xFF00);
		xcolor.flags = DoRed | DoGreen | DoBlue;
		xcolor.pad = 0;
		XAllocColor(dpy, colormap, &xcolor);
		XPutPixel(image, column, line, xcolor.pixel);
	}
}

/*
 * Get the write function for an image.
 */
typedef void (*WriteFunc)(Display *dpy, Colormap colormap, XImage *image,
						  int len, unsigned long *input, int num);
static WriteFunc GetWriteFunc(XImage *image)
{
	/* Determine if the client machine is little-endian or big-endian */
	union
	{
		short volatile value;
		unsigned char volatile buf[2];
	} un;
	int littleEndian;
	un.value = 0x0102;
	littleEndian = (un.buf[0] == 0x02);

	/* Check for well-known special cases */
	if(image->depth == 24 && image->red_mask == 0x00FF0000 &&
	   image->green_mask == 0x0000FF00 && image->blue_mask == 0x000000FF &&
	   image->byte_order == LSBFirst && image->bitmap_bit_order == LSBFirst)
	{
		if(littleEndian)
		{
			return Write_Direct;
		}
		else
		{
			return Write_DirectSwap;
		}
	}
	if(image->depth == 24 && image->red_mask == 0x00FF0000 &&
	   image->green_mask == 0x0000FF00 && image->blue_mask == 0x000000FF &&
	   image->byte_order == MSBFirst && image->bitmap_bit_order == MSBFirst)
	{
		if(littleEndian)
		{
			return Write_DirectSwap;
		}
		else
		{
			return Write_Direct;
		}
	}
	if(image->depth == 16 && image->red_mask == 0x0000F800 &&
	   image->green_mask == 0x000007E0 && image->blue_mask == 0x0000001F &&
	   image->byte_order == LSBFirst && image->bitmap_bit_order == LSBFirst)
	{
		if(littleEndian)
		{
			return Write_16bpp;
		}
	}

	/* Fall back to the generic writer, using XPutPixel */
	return Write_Default;
}

/*
 * Forward declaration.
 */
void XSharpDestroyImage(XImage *image);

/*
 * Table that helps reverse the order of bits in a byte.
 */
static unsigned char const reverseByte[256] = {
	0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 
	0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 
	0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 
	0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8, 
	0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 
	0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 
	0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 
	0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC, 
	0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 
	0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 
	0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 
	0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA, 
	0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 
	0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6, 
	0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 
	0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE, 
	0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 
	0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1, 
	0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 
	0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 
	0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 
	0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5, 
	0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 
	0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD, 
	0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 
	0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 
	0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 
	0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB, 
	0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 
	0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 
	0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 
	0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF, 
};

/*
 * Create an XImage structure from a device-independent bitmap.
 *
 * If the input image is indexed, then "palette" indicates the
 * converted pixel values of the image's palette.
 *
 * If the input image is RGB, and the screen is indexed, then
 * "palette" indicates a 216-color palette to convert the image to.
 */
XImage *XSharpCreateImageFromDIB(Screen *screen, int width, int height,
								 int stride, int pixelFormat,
								 unsigned char *data, int isMask,
								 unsigned long *palette)
{
	Display *dpy = DisplayOfScreen(screen);
	Visual *visual = DefaultVisualOfScreen(screen);
	Colormap colormap = DefaultColormapOfScreen(screen);
	unsigned int depth;
	int format;
	int bitmap_pad;
	XImage *image;
	unsigned char *imageData;
	unsigned char *temp1;
	unsigned char *temp2;
	int line, column;
	unsigned long *tempLine;
	ReadFunc readFunc;
	WriteFunc writeFunc;

	/* Get the depth, format, and pad values */
	if(isMask)
	{
		depth = 1;
		format = XYBitmap;
		bitmap_pad = 8;
	}
	else
	{
		depth = DefaultDepthOfScreen(screen);
		format = ZPixmap;
		if(depth <= 8)
		{
			bitmap_pad = 8;
		}
		else if(depth <= 16)
		{
			bitmap_pad = 16;
		}
		else
		{
			bitmap_pad = 32;
		}
	}

	/* Create the main image structure */
	image = XCreateImage(dpy, visual, depth, format, 0, 0,
						 (unsigned int)width, (unsigned int)height,
						 bitmap_pad, 0);
	if(!image)
	{
		return 0;
	}

	/* Allocate space for the image.  Conventional wisdom says that
	   we should use shared memory for this.  However, on today's
	   hardware, most users will never notice the difference unless
	   the system is displaying full-motion video.  Which we aren't */
	imageData = (unsigned char *)malloc(image->bytes_per_line *
										image->height);
	if(!imageData)
	{
		XDestroyImage(image);
		return 0;
	}
	image->data = (char *)imageData;

	/* Convert the input image into raw X pixels within the XImage.
	   There are probably a lot of things that can be done to speed
	   this up, although for most small icon-sized images, it usually
	   doesn't matter too much */
	if(isMask)
	{
		if(image->byte_order == LSBFirst &&
		   image->bitmap_bit_order == LSBFirst)
		{
			for(line = 0; line < height; ++line)
			{
				temp1 = imageData + line * image->bytes_per_line;
				temp2 = data + line * stride;
				for(column = (int)(image->bytes_per_line); column > 0; --column)
				{
					*temp1++ = reverseByte[*temp2++];
				}
			}
		}
		else if(image->byte_order == MSBFirst &&
		        image->bitmap_bit_order == MSBFirst)
		{
			for(line = 0; line < height; ++line)
			{
				temp1 = imageData + line * image->bytes_per_line;
				temp2 = data + line * stride;
				for(column = (int)(image->bytes_per_line); column > 0; --column)
				{
					*temp1++ = *temp2++;
				}
			}
		}
		else
		{
			for(line = 0; line < height; ++line)
			{
				temp1 = imageData + line * image->bytes_per_line;
				temp2 = data + line * stride;
				for(column = 0; column < width; ++column)
				{
					if(temp2[column / 8] & (0x80 >> (column % 8)))
					{
						XPutPixel(image, column, line, 1);
					}
					else
					{
						XPutPixel(image, column, line, 0);
					}
				}
			}
		}
	}
	else if((pixelFormat & PF_Indexed) != 0)
	{
		for(line = 0; line < height; ++line)
		{
			temp1 = data + line * stride;
			if(pixelFormat == PF_Format1bppIndexed)
			{
				/* Source image has one bit per pixel */
				for(column = 0; column < width; ++column)
				{
					if((temp1[column / 8] & (0x80 >> (column % 8))) != 0)
					{
						XPutPixel(image, column, line, palette[1]);
					}
					else
					{
						XPutPixel(image, column, line, palette[0]);
					}
				}
			}
			else if(pixelFormat == PF_Format4bppIndexed)
			{
				/* Source image has four bits per pixel */
				for(column = 0; column < width; column += 2)
				{
					XPutPixel(image, column, line,
							  palette[(*temp1 >> 4) & 0x0F]);
					if((column + 1) < width)
					{
						XPutPixel(image, column + 1, line,
								  palette[*temp1 & 0x0F]);
					}
					++temp1;
				}
			}
			else
			{
				/* Source image has eight bits per pixel */
				for(column = 0; column < width; ++column)
				{
					XPutPixel(image, column, line, palette[*temp1]);
					++temp1;
				}
			}
		}
	}
	else if(visual->class == TrueColor || visual->class == DirectColor)
	{
		/* Map an RGB image to an RGB-capable display */
		tempLine = (unsigned long *)(malloc(sizeof(unsigned long) * width));
		if(!tempLine)
		{
			XSharpDestroyImage(image);
			return 0;
		}
		readFunc = GetReadFunc(pixelFormat);
		writeFunc = GetWriteFunc(image);
		if(!readFunc || !writeFunc)
		{
			free(tempLine);
			XSharpDestroyImage(image);
			return 0;
		}
		for(line = 0; line < height; ++line)
		{
			(*readFunc)(data + line * stride, tempLine, width);
			(*writeFunc)(dpy, colormap, image, line, tempLine, width);
		}
		free(tempLine);
	}
	else
	{
		/* Map an RGB image to an indexed display (usually 8-bit) */
		tempLine = (unsigned long *)(malloc(sizeof(unsigned long) * width));
		if(!tempLine)
		{
			XSharpDestroyImage(image);
			return 0;
		}
		readFunc = GetReadFunc(pixelFormat);
		if(!readFunc)
		{
			free(tempLine);
			XSharpDestroyImage(image);
			return 0;
		}
		for(line = 0; line < height; ++line)
		{
			(*readFunc)(data + line * stride, tempLine, width);
			Write_Indexed(image, line, tempLine, width, palette);
		}
		free(tempLine);
	}

	/* Return the final image structure to the caller */
	return image;
}

/*
 * Destroy an image that was created by "XSharpCreateImageFromDIB".
 */
void XSharpDestroyImage(XImage *image)
{
	if(image)
	{
		if(image->data)
		{
			free((void *)(image->data));
			image->data = 0;
		}
		XDestroyImage(image);
	}
}

/*
 * Get the size of an image.
 */
void XSharpGetImageSize(XImage *image, int *width, int *height)
{
	if(image)
	{
		*width = (int)(image->width);
		*height = (int)(image->height);
	}
	else
	{
		*width = 1;
		*height = 1;
	}
}

/*
 * Internal structure of X11 region objects.  We have to define
 * this here because there is no public header that contains it.
 */
typedef struct
{
    short x1, x2, y1, y2;

} BOX;
typedef struct
{
    long size;
    long numRects;
    BOX *rects;
    BOX extents;

} REGION;

/*
 * Get the number of rectangles contained in a region structure.
 */
int XSharpGetRegionSize(Region region)
{
	return (int)(((REGION *)region)->numRects);
}

/*
 * Get the bounding area of a particular rectangle within a region structure.
 */
void XSharpGetRegionRect(Region region, int index, XRectangle *rect)
{
	BOX *box = &(((REGION *)region)->rects[index]);
	rect->x = box->x1;
	rect->y = box->y1;
	rect->width = box->x2 - box->x1;
	rect->height = box->y2 - box->y1;
}

/*
 * Send a close request to a window.  Tries to send "WM_DELETE_WINDOW",
 * and if that doesn't work, sends "XKillClent" instead.
 */
void XSharpSendClose(Display *dpy, Window window)
{
	Atom wmDeleteWindow = XInternAtom(dpy, "WM_DELETE_WINDOW", False);
	Atom *protocols;
	int num_protocols, posn;
	int use_kill = 1;

	/* Get the protocols that are supported by the client window */
	protocols = 0;
	num_protocols = 0;
	if(XGetWMProtocols(dpy, window, &protocols, &num_protocols))
	{
		for(posn = 0; posn < num_protocols; ++posn)
		{
			if(protocols[posn] == wmDeleteWindow)
			{
				use_kill = 0;
				break;
			}
		}
		if(protocols)
		{
			XFree(protocols);
		}
	}

	/* Send the close request to the client application */
	if(use_kill)
	{
		XKillClient(dpy, (XID)window);
	}
	else
	{
		XEvent msg;
		memset(&msg, 0, sizeof(msg));
		msg.xclient.type = ClientMessage;
		msg.xclient.window = window;
		msg.xclient.message_type = XInternAtom(dpy, "WM_PROTOCOLS", False);
		msg.xclient.format = 32;
		msg.xclient.data.l[0] = wmDeleteWindow;
		XSendEvent(dpy, window, False, NoEventMask, &msg);
	}
}

/*
 * Send a wakeup message to a window.
 */
void XSharpSendWakeup(Display *dpy, Window window)
{
	Atom wakeupAtom = XInternAtom(dpy, "_XSHARP_WAKEUP", False);
	XEvent msg;
	memset(&msg, 0, sizeof(msg));
	msg.xclient.type = ClientMessage;
	msg.xclient.window = window;
	msg.xclient.message_type = wakeupAtom;
	msg.xclient.format = 32;
	XSendEvent(dpy, window, False, NoEventMask, &msg);
}

#else /* X_DISPLAY_MISSING || !HAVE_SELECT */

int XSharpSupportPresent(void)
{
	return 0;
}

int XNextEventWithTimeout(void *dpy, void *event, int timeout)
{
	/* Nothing to do here */
	return -1;
}

int XSharpUseXft()
{
	/* Nothing to do here */
	return 0;
}

void *XSharpCreateFontSet(void *dpy, const char *family,
						  int pointSize, int style)
{
	/* Nothing to do here */
	return 0;
}

void XSharpFreeFontSet(void *dpy, void *fontSet)
{
	/* Nothing to do here */
}

void XSharpDrawStringSet(void *dpy, unsigned long drawable, void *gc,
					     void *fontSet, int x, int y,
					     const char *str, int style)
{
	/* Nothing to do here */
}

void XSharpTextExtentsSet(void *dpy, void *fontSet, const char *str,
					      void *overall_ink_return,
					      void *overall_logical_return)
{
	/* Nothing to do here */
}

void XSharpFontExtentsSet(void *fontSet,
					      void *max_ink_return,
					      void *max_logical_return)
{
	/* Nothing to do here */
}

void *XSharpCreateFontStruct(void *dpy, const char *family,
							 int pointSize, int style)
{
	/* Nothing to do here */
	return 0;
}

void XSharpFreeFontStruct(void *dpy, void *fontSet)
{
	/* Nothing to do here */
}

void XSharpDrawStringStruct(void *dpy, unsigned long drawable, void *gc,
					        void *fontSet, int x, int y,
					        void *str, long offset, long count, int style)
{
	/* Nothing to do here */
}

void XSharpTextExtentsStruct(void *dpy, void *fontSet,
							 void *str, long offset, long count,
					         void *overall_ink_return,
					         void *overall_logical_return)
{
	/* Nothing to do here */
}

void XSharpFontExtentsStruct(void *fontSet,
					         void *max_ink_return,
					         void *max_logical_return)
{
	/* Nothing to do here */
}

void *XSharpCreateFontXft(void *dpy, const char *family,
						  int pointSize, int style)
{
	/* Nothing to do here */
	return 0;
}

void XSharpFreeFontXft(void *dpy, void *fontSet)
{
	/* Nothing to do here */
}

void XSharpDrawStringXft(void *dpy, unsigned long drawable, void *gc,
					     void *fontSet, int x, int y,
					     const char *str, int style, void *clipRegion,
					     unsigned long colorValue)
{
	/* Nothing to do here */
}

void XSharpTextExtentsXft(void *dpy, void *fontSet, const char *str,
					      void *overall_ink_return,
					      void *overall_logical_return)
{
	/* Nothing to do here */
}

void XSharpFontExtentsXft(void *fontSet,
					      void *max_ink_return,
					      void *max_logical_return)
{
	/* Nothing to do here */
}

char *XSharpGetResources(void *dpy, unsigned long root)
{
	/* Nothing to do here */
	return 0;
}

void XSharpFreeResources(char *value)
{
	/* Nothing to do here */
}

void *XSharpCreateImageFromDIB(void *screen, int width, int height,
							   int stride, int pixelFormat,
							   unsigned char *data, int isMask,
							   unsigned long *palette)
{
	/* Nothing to do here */
	return 0;
}

void XSharpDestroyImage(void *image)
{
	/* Nothing to do here */
}

void XSharpGetImageSize(void *image, int *width, int *height)
{
	*width = 1;
	*height = 1;
}

int XSharpGetRegionSize(void *region)
{
	/* Nothing to do here */
	return 0;
}

void XSharpGetRegionRect(void *region, int index, void *rect)
{
	/* Nothing to do here */
}

void XSharpSendClose(void *dpy, unsigned long window)
{
	/* Nothing to do here */
}

void XSharpSendWakeup(void *dpy, unsigned long window)
{
	/* Nothing to do here */
}

#endif /* X_DISPLAY_MISSING || !HAVE_SELECT */
