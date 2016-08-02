/*
 * XsharpPCF.c - C support code for client-side rendering of PCF fonts.
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

/*
 * Format flags for PCF files.
 */
#define PCF_DEFAULT_FORMAT			0x00000000
#define PCF_INKBOUNDS				0x00000200
#define PCF_ACCEL_W_INKBOUNDS		0x00000100
#define PCF_COMPRESSED_METRICS		0x00000100
#define	PCF_BYTEORDER_MSBFIRST		0x00000004
#define	PCF_BITORDER_MSBFIRST		0x00000008
#define	PCF_GLYPH_PADDING_MASK		0x00000003
#define	PCF_SCAN_PADDING_MASK		0x00000030

/*
 * Table types for PCF files.
 */
#define PCF_PROPERTIES              (1<<0)
#define PCF_ACCELERATORS            (1<<1)
#define PCF_METRICS                 (1<<2)
#define PCF_BITMAPS                 (1<<3)
#define PCF_INK_METRICS             (1<<4)
#define PCF_BDF_ENCODINGS           (1<<5)
#define PCF_SWIDTHS                 (1<<6)
#define PCF_GLYPH_NAMES             (1<<7)
#define PCF_BDF_ACCELERATORS        (1<<8)

/*
 * Information about a PCF font image once it has been loaded into memory.
 * This is independent of the screen and can be shared between multiple
 * instances of "PCFFontRenderer".
 */
typedef struct
{
	XFontStruct	    fs;
	unsigned char  *data;
	unsigned long   length;
	unsigned long   posn;
	int			    format;
	int			    numGlyphs;
	int				glyphFormat;
	unsigned char **glyphs;

} PCFFontImage;

/*
 * Information that is used to render a PCF font to a screen.
 */
typedef struct
{
	PCFFontImage   *image;
	XImage		   *ximage;
	Pixmap			stipple;
	GC				stippleGC;

} PCFFontRenderer;

/*
 * Read an 8-bit value from the current position in a PCF font image.
 */
static int PCFReadInt8(PCFFontImage *image)
{
	if(image->posn < image->length)
	{
		return (int)(image->data[(image->posn)++]);
	}
	else
	{
		return 0;
	}
}

/*
 * Read a 16-bit value from the current position in a PCF font image.
 */
static int PCFReadInt16(PCFFontImage *image)
{
	int byte1 = PCFReadInt8(image);
	int byte2 = PCFReadInt8(image);
	if((image->format & PCF_BYTEORDER_MSBFIRST) != 0)
	{
		return (int)(short)((byte1 << 8) | byte2);
	}
	else
	{
		return (int)(short)((byte2 << 8) | byte1);
	}
}

/*
 * Read a 32-bit value from the current position in a PCF font image.
 */
static int PCFReadInt32(PCFFontImage *image)
{
	int byte1 = PCFReadInt8(image);
	int byte2 = PCFReadInt8(image);
	int byte3 = PCFReadInt8(image);
	int byte4 = PCFReadInt8(image);
	if((image->format & PCF_BYTEORDER_MSBFIRST) != 0)
	{
		return (int)((byte1 << 24) | (byte2 << 16) | (byte3 << 8) | byte4);
	}
	else
	{
		return (int)((byte4 << 24) | (byte3 << 16) | (byte2 << 8) | byte1);
	}
}

/*
 * Skip a number of bytes in a PCF font image.
 */
static void PCFSkipBytes(PCFFontImage *image, int num)
{
	image->posn += (unsigned long)(long)num;
}

/*
 * Select a particular table in a PCF font image.  Returns zero if
 * the table is not present.  The stream will be positioned just after
 * the 32-bit "format" value.
 */
static int PCFSelectTable(PCFFontImage *image, int table)
{
	int count, index;
	int type, format, size, offset;
	image->posn = 4;
	image->format = PCF_DEFAULT_FORMAT;
	count = PCFReadInt32(image);
	for(index = 0; index < count; ++index)
	{
		type = PCFReadInt32(image);
		format = PCFReadInt32(image);
		size = PCFReadInt32(image);
		offset = PCFReadInt32(image);
		if(type == table)
		{
			image->format = format;
			image->posn = (unsigned long)(long)(offset + 4);
			return 1;
		}
	}
	return 0;
}

/*
 * Read a full metrics value.
 */
static void PCFReadFullMetrics(PCFFontImage *image, XCharStruct *metrics)
{
	metrics->lbearing = (short)(PCFReadInt16(image));
	metrics->rbearing = (short)(PCFReadInt16(image));
	metrics->width = (short)(PCFReadInt16(image));
	metrics->ascent = (short)(PCFReadInt16(image));
	metrics->descent = (short)(PCFReadInt16(image));
	metrics->attributes = (unsigned short)(PCFReadInt16(image));
}

/*
 * Read a compressed metrics value.
 */
static void PCFReadCompressedMetrics(PCFFontImage *image, XCharStruct *metrics)
{
	metrics->lbearing = (short)(PCFReadInt8(image) - 128);
	metrics->rbearing = (short)(PCFReadInt8(image) - 128);
	metrics->width = (short)(PCFReadInt8(image) - 128);
	metrics->ascent = (short)(PCFReadInt8(image) - 128);
	metrics->descent = (short)(PCFReadInt8(image) - 128);
	metrics->attributes = 0;
}

/*
 * Free the memory used by a PCF font image.
 */
static void PCFFontFree(PCFFontImage *image)
{
	if(image->fs.properties)
	{
		free(image->fs.properties);
	}
	if(image->fs.per_char)
	{
		free(image->fs.per_char);
	}
	if(image->glyphs)
	{
		free(image->glyphs);
	}
	free(image);
}

/*
 * Create a PCF font image from the information in a font image buffer.
 */
void *XSharpPCFCreateImage(unsigned char *data, unsigned long length)
{
	PCFFontImage *image;
	int temp;
	unsigned char *start;

	/* Bail out if the data does not appear to be a PCF font image */
	if(length < 4 || data[0] != 0x01 || data[1] != 'f' ||
	   data[2] != 'c' || data[3] != 'p')
	{
		return 0;
	}

	/* Allocate memory for the image */
	image = (PCFFontImage *)calloc(1, sizeof(PCFFontImage));
	if(!image)
	{
		return 0;
	}

	/* Initialize the image reading data */
	image->data = data;
	image->length = length;
	image->posn = 0;
	image->format = PCF_DEFAULT_FORMAT;

	/* Select the "accelerators" table and then load it */
	temp = PCFSelectTable(image, PCF_BDF_ACCELERATORS);
	if(!temp)
	{
		temp = PCFSelectTable(image, PCF_ACCELERATORS);
	}
	if(temp)
	{
		PCFSkipBytes(image, 6);
		image->fs.direction = (unsigned)(PCFReadInt8(image));
		PCFSkipBytes(image, 1);
		image->fs.ascent = PCFReadInt32(image);
		image->fs.descent = PCFReadInt32(image);
		PCFSkipBytes(image, 4);
		PCFReadFullMetrics(image, &(image->fs.min_bounds));
		PCFReadFullMetrics(image, &(image->fs.max_bounds));
	}

	/* Select the "BDF encodings" table and then load it */
	if(PCFSelectTable(image, PCF_BDF_ENCODINGS))
	{
		image->fs.min_char_or_byte2 = (unsigned)(PCFReadInt16(image));
		image->fs.max_char_or_byte2 = (unsigned)(PCFReadInt16(image));
		image->fs.min_byte1 = (unsigned)(PCFReadInt16(image));
		image->fs.max_byte1 = (unsigned)(PCFReadInt16(image));
		image->fs.default_char = (unsigned)(PCFReadInt16(image) & 0xFFFF);
	}

	/* Select the "metrics" table and then load it */
	if(PCFSelectTable(image, PCF_METRICS))
	{
		if((image->format & PCF_COMPRESSED_METRICS) == 0)
		{
			image->numGlyphs = PCFReadInt32(image);
			image->fs.per_char = (XCharStruct *)malloc
				(sizeof(XCharStruct) * image->numGlyphs);
			if(!(image->fs.per_char))
			{
				PCFFontFree(image);
				return 0;
			}
			for(temp = 0; temp < image->numGlyphs; ++temp)
			{
				PCFReadFullMetrics(image, &(image->fs.per_char[temp]));
			}
		}
		else
		{
			image->numGlyphs = (PCFReadInt16(image) & 0xFFFF);
			image->fs.per_char = (XCharStruct *)malloc
				(sizeof(XCharStruct) * image->numGlyphs);
			if(!(image->fs.per_char))
			{
				PCFFontFree(image);
				return 0;
			}
			for(temp = 0; temp < image->numGlyphs; ++temp)
			{
				PCFReadCompressedMetrics(image, &(image->fs.per_char[temp]));
			}
		}
	}

	/* Collect up pointers to all of the glyph bitmaps */
	if(PCFSelectTable(image, PCF_BITMAPS))
	{
		if(PCFReadInt32(image) != image->numGlyphs)
		{
			PCFFontFree(image);
			return 0;
		}
		image->glyphFormat = image->format;
		image->glyphs = (unsigned char **)malloc
			(sizeof(unsigned char *) * image->numGlyphs);
		if(!(image->glyphs))
		{
			PCFFontFree(image);
			return 0;
		}
		start = image->data + image->posn + 16 +
				(unsigned long)(long)(image->numGlyphs * 4);
		for(temp = 0; temp < image->numGlyphs; ++temp)
		{
			image->glyphs[temp] = start + PCFReadInt32(image);
		}
	}

	/* Sanity-check the values that we read to make sure that
	   we have sufficient information to do something useful */
	temp = (int)((image->fs.max_char_or_byte2 -
				  image->fs.min_char_or_byte2 + 1) *
				 (image->fs.max_byte1 - image->fs.min_byte1 + 1));
	if(!(image->numGlyphs) || image->numGlyphs != temp)
	{
		PCFFontFree(image);
		return 0;
	}
	if((image->glyphFormat & (PCF_BITORDER_MSBFIRST |
							  PCF_GLYPH_PADDING_MASK |
							  PCF_SCAN_PADDING_MASK))
				!= PCF_BITORDER_MSBFIRST)
	{
		fprintf(stderr, "XSharpPCFCreateImage: currently, we only support "
						"PCF fonts built with\n");
		fprintf(stderr, "the command-line `bdftopcf -p1 -u1 -m font.bdf'\n");
		PCFFontFree(image);
		return 0;
	}

	/* Return the PCF font image to the caller */
	return image;
}

/*
 * Destroy a PCF font image that is no longer required.
 */
void XSharpPCFDestroyImage(PCFFontImage *image)
{
	if(image)
	{
		PCFFontFree(image);
	}
}

/*
 * Destroy a PCF font renderer that is no longer required.
 */
void XSharpPCFDestroy(Display *dpy, PCFFontRenderer *renderer)
{
	if(renderer)
	{
		if(renderer->ximage)
		{
			if(renderer->ximage->data)
			{
				free(renderer->ximage->data);
				renderer->ximage->data = 0;
			}
			XDestroyImage(renderer->ximage);
		}
		if(renderer->stippleGC)
		{
			XFreeGC(dpy, renderer->stippleGC);
		}
		if(renderer->stipple)
		{
			XFreePixmap(dpy, renderer->stipple);
		}
	}
}

/*
 * Create a PCF font renderer for a particular display.
 */
void *XSharpPCFCreate(Display *dpy, PCFFontImage *image)
{
	PCFFontRenderer *renderer;
	int width, height;

	/* Create the renderer structure */
	renderer = (PCFFontRenderer *)calloc(1, sizeof(PCFFontRenderer));
	if(!renderer)
	{
		return 0;
	}
	renderer->image = image;

	/* Create an XImage that can be used to hold string bitmaps
	   prior to being sent to the X server */
	width = image->fs.max_bounds.width * 33;
	width = (width + 31) & ~31;
	height = image->fs.ascent + image->fs.descent;
	renderer->ximage = XCreateImage
			(dpy, DefaultVisual(dpy, DefaultScreen(dpy)), 1, ZPixmap,
			 0, 0, width, height, 8, 0);
	if(!(renderer->ximage))
	{
		XSharpPCFDestroy(dpy, renderer);
		return 0;
	}
	renderer->ximage->data = (char *)calloc
		(height * renderer->ximage->bytes_per_line, 1);
	if(!(renderer->ximage->data))
	{
		XSharpPCFDestroy(dpy, renderer);
		return 0;
	}

	/* Create an X pixmap that acts as a stipple for string drawing */
	renderer->stipple =
		XCreatePixmap(dpy, RootWindow(dpy, DefaultScreen(dpy)),
					  width, height, 1);
	renderer->stippleGC = XCreateGC(dpy, renderer->stipple, 0, 0);

	/* Return the renderer to the caller */
	return renderer;
}

/*
 * Get the text extents for a string, using a PCF font renderer.
 * Since the PCF font contains an XFontStruct, we can use the same
 * algorithm as for XFontStruct-based fonts.
 */
void XSharpTextExtentsPCF(Display *dpy, void *fontSet, ILString *str,
					      long offset, long count,
						  XRectangle *overall_ink_return,
					      XRectangle *overall_logical_return)
{
	PCFFontRenderer *renderer = (PCFFontRenderer *)fontSet;
	XSharpTextExtentsStruct(dpy, &(renderer->image->fs),
						    str, offset, count,
							overall_ink_return,
							overall_logical_return);
}

/*
 * Get the font extents for a PCF font renderer.  Since the PCF font
 * contains an XFontStruct, we can use the same algorithm as for
 * XFontStruct-based fonts.
 */
void XSharpFontExtentsPCF(void *fontSet,
					      XRectangle *max_ink_return,
					      XRectangle *max_logical_return)
{
	PCFFontRenderer *renderer = (PCFFontRenderer *)fontSet;
	return XSharpFontExtentsStruct(&(renderer->image->fs),
								   max_ink_return, max_logical_return);
}

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
 * Draw a string using a client-side PCF font renderer.
 */
void XSharpDrawStringPCF(Display *dpy, Drawable drawable, GC gc,
				         void *fontSet, int x, int y, ILString *str,
						 long offset, long length, int style)
{
	PCFFontRenderer *renderer = (PCFFontRenderer *)fontSet;
	PCFFontImage *image = renderer->image;
	XImage *ximage = renderer->ximage;
	int posn, num;
	int width, height, line;
	XGCValues values;
	ILChar *buffer = ILStringToBuffer(str) + offset;
	unsigned ch;
	XRectangle overall_ink;
	XRectangle overall_logical;
	int line1, line2;
	long origOffset = offset;
	long origLength = length;
	int origX = x;
	int xposn;
	unsigned char *glyph;
	unsigned char *outline;
	XCharStruct *cs;
	int gx, gy, gwidth, gheight;
	int bytesPerLine, xrel;
	int writeMode, byteval, bytesize;

	/* Save the current GC fill settings */
	XGetGCValues(dpy, gc,
				 GCStipple | GCTileStipXOrigin |
				 GCTileStipYOrigin | GCFillStyle, &values);

	/* Determine if we can use a faster bitmap writing mode */
	if(ximage->bitmap_bit_order == ximage->byte_order)
	{
		writeMode = ximage->bitmap_bit_order;
	}
	else
	{
		writeMode = -1;
	}

	/* Break the string up into 32-character sections and draw each section */
	while(length > 0)
	{
		/* Get the dimensions of this section */
		posn = 32;
		if(posn > length)
		{
			posn = length;
		}
		XSharpTextExtentsStruct(dpy, &(image->fs), str, offset, posn,
								&overall_ink, &overall_logical);
		width = overall_ink.width;
		height = overall_logical.height;

		/* Clear enough of the XImage to hold the section we'll be rendering */
		width = (width + ximage->bitmap_pad - 1) & ~(ximage->bitmap_pad - 1);
		line = height;
		while(line > 0)
		{
			--line;
			memset(ximage->data + line * ximage->bytes_per_line, 0, width / 8);
		}

		/* Render the bitmaps for the characters into the XImage */
		xposn = -(overall_ink.x);
		num = posn;
		while(num > 0)
		{
			/* Fetch the next character from the string and convert
			   into into a glyph index */
			ch = *buffer++;
			--num;
			if(ch >= 0x0100)
			{
				ch = '?';
			}
			if(ch < image->fs.min_char_or_byte2 ||
			   ch > image->fs.max_char_or_byte2)
			{
				ch = image->fs.default_char;
			}
			else
			{
				ch -= image->fs.min_char_or_byte2;
			}

			/* Get the bitmap and the dimensions for the glyph */
			glyph = image->glyphs[ch];
			cs = &(image->fs.per_char[ch]);
			gx = xposn + cs->lbearing;
			gy = image->fs.ascent - cs->ascent;
			gwidth = cs->rbearing - cs->lbearing;
			gheight = cs->ascent + cs->descent;

			/* Render the glyph into the XImage */
			bytesPerLine = (gwidth + 7) / 8;
			if(writeMode == LSBFirst)
			{
				while(gheight > 0)
				{
					outline = (unsigned char *)
						(ximage->data + gy * ximage->bytes_per_line + gx / 8);
					byteval = 0;
					bytesize = (gx % 8);
					for(xrel = 0; xrel < bytesPerLine; ++xrel)
					{
						byteval = (byteval << 8) | *glyph++;
						*outline++ |=
							reverseByte[(unsigned char)(byteval >> bytesize)];
					}
					if(bytesize != 0)
					{
						*outline |= reverseByte
							[(unsigned char)(byteval << (8 - bytesize))];
					}
					++gy;
					--gheight;
				}
			}
			else if(writeMode == MSBFirst)
			{
				while(gheight > 0)
				{
					outline = (unsigned char *)
						(ximage->data + gy * ximage->bytes_per_line + gx / 8);
					byteval = 0;
					bytesize = (gx % 8);
					for(xrel = 0; xrel < bytesPerLine; ++xrel)
					{
						byteval = (byteval << 8) | *glyph++;
						*outline++ |= (unsigned char)(byteval >> bytesize);
					}
					if(bytesize != 0)
					{
						*outline |= (unsigned char)(byteval << (8 - bytesize));
					}
					++gy;
					--gheight;
				}
			}
			else
			{
				while(gheight > 0)
				{
					for(xrel = 0; xrel < gwidth; ++xrel)
					{
						if((glyph[xrel / 8] & (0x80 >> (xrel % 8))) != 0)
						{
							XPutPixel(ximage, gx + xrel, gy, 1);
						}
					}
					glyph += bytesPerLine;
					++gy;
					--gheight;
				}
			}

			/* Move on to the next character */
			xposn += cs->width;
		}

		/* Write the XImage into the stipple pixmap */
		XPutImage(dpy, renderer->stipple, renderer->stippleGC, ximage,
				  0, 0, 0, 0, width, height);

		/* Select the stipple and draw the text with it */
		XSetStipple(dpy, gc, renderer->stipple);
		XSetTSOrigin(dpy, gc, x + overall_ink.x, y + overall_logical.y);
		XSetFillStyle(dpy, gc, FillStippled);
		XFillRectangle(dpy, drawable, gc, x + overall_ink.x,
					   y + overall_logical.y, width, height);

		/* Advance to the next section */
		offset += posn;
		length -= posn;
		x += overall_logical.width;
	}

	/* Restore the previous GC fill settings */
	if(values.fill_style == FillStippled ||
	   values.fill_style == FillOpaqueStippled)
	{
		XChangeGC(dpy, gc,
				  GCStipple | GCTileStipXOrigin |
				  GCTileStipYOrigin | GCFillStyle, &values);
	}
	else
	{
		XChangeGC(dpy, gc, GCTileStipXOrigin | GCTileStipYOrigin |
				  GCFillStyle, &values);
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
		line2 = y - (image->fs.ascent / 2);
	}
	else
	{
		line2 = y;
	}

	/* Draw the underline and strike-out */
	if(line1 != y || line2 != y)
	{
		XSharpTextExtentsStruct(dpy, &(image->fs), str, origOffset, origLength,
				 				&overall_ink, &overall_logical);
		XSetLineAttributes(dpy, gc, 1, LineSolid, CapNotLast, JoinMiter);
		if(line1 != y)
		{
			XDrawLine(dpy, drawable, gc, origX, line1,
					  origX + overall_logical.width, line1);
		}
		if(line2 != y)
		{
			XDrawLine(dpy, drawable, gc, origX, line2,
					  origX + overall_logical.width, line2);
		}
	}
}

#else /* X_DISPLAY_MISSING || !HAVE_SELECT */

void *XSharpPCFCreateImage(unsigned char *data, unsigned long length)
{
	/* Nothing to do here */
	return 0;
}

void XSharpPCFDestroyImage(void *image)
{
	/* Nothing to do here */
}

void XSharpPCFDestroy(void *dpy, void *renderer)
{
	/* Nothing to do here */
}

void *XSharpPCFCreate(void *dpy, void *image)
{
	/* Nothing to do here */
}

void XSharpTextExtentsPCF(void *dpy, void *fontSet, void *str,
					      long offset, long count,
						  void *overall_ink_return,
					      void *overall_logical_return)
{
	/* Nothing to do here */
}

#endif /* X_DISPLAY_MISSING || !HAVE_SELECT */
