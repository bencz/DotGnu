/*
 * CFont.c - Font implementation.
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

#include "CFont.h"
#include "CFontFace.h"
#include "CFontFamily.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/* baseline calculation */
#define _CalcBaseline(font, face) \
	(((font)->pixels * (face)->ascender) / (face)->units_per_EM)

/* default text metrics */
static const CTextMetrics CTextMetrics_Zero = { { 0, 0 }, { 0, 0 }, { 0, 0 } };

/* Structure of a font. */
struct _tagCFont
{
	CFontFace     *face;
	CFloat         points;
	CFloat         pixels;
	CFloat         size;
	CFontStyle     style;
	CGraphicsUnit  unit;
};

/* Get the glyph flags for the given text rendering hint. */
static CGlyphFlag
_GetGlyphFlags(CTextRenderingHint hint)
{
	/* determine and return the depth and hinting */
	switch(hint)
	{
		case CTextRenderingHint_SingleBitPerPixelGridFit:
			{ return CGlyphFlag_Hinting; }

		case CTextRenderingHint_SingleBitPerPixel:
			{ return CGlyphFlag_None; }

		case CTextRenderingHint_AntiAlias:
			{ return CGlyphFlag_AntiAlias; }

		case CTextRenderingHint_SystemDefault:
		case CTextRenderingHint_AntiAliasGridFit:
		case CTextRenderingHint_ClearTypeGridFit:
		default:
			{ return CGlyphFlag_AntiAliasHinting; }
	}
}

/*\
|*| Get the height of a font.
|*|
|*|   _this - this font
|*|
|*|  NOTE: the returned height is in the units of this
|*|        font as given during construction
|*|
|*|  Returns font height.
\*/
static CFloat
CFont_GetHeightInternal(CFont *_this)
{
	/* declarations */
	CUInt32 emHeight;
	CUInt32 lineSpacing;

	/* assertions */
	CASSERT((_this != 0));

	/* get the em height of the face */
	emHeight = _this->face->face->units_per_EM;

	/* get the line spacing of the face */
	lineSpacing = _this->face->face->height;

	/* calculate and return the height of this font */
	return ((_this->size * lineSpacing) / emHeight);
}

/*\
|*| Measure a string.
|*|
|*|     _this - this font
|*|    string - string to measure
|*|    length - length of string
|*|    device - device transformation
|*|      hint - text rendering hint
|*|   metrics - metrics of string (returned)
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFont_MeasureString(CFont              *_this,
                    const CChar16      *string,
                    CUInt32             length,
                    CAffineTransformF  *device,
                    CTextRenderingHint  hint,
                    CTextMetrics       *metrics)
{
	/* declarations */
	CFontFace   *face;
	CGlyphShape *shape;
	CGlyphFlag   flags;
	CFloat       scaleX;
	CFloat       scaleY;
	CStatus      status;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((metrics != 0));

	/* bail out now if there's nothing to do */
	if(string == 0 || length == 0)
	{
		CStatus_CheckGOTO(CStatus_OK, status, GOTO_FailA);
	}

	/* get the depth and hinting */
	flags = _GetGlyphFlags(hint);

	/* get the font face */
	face = _this->face;

	/* measure the string, synchronously */
	CMutex_Lock(face->lock);
	{
		/* declarations */
		CChar32 prev;
		CFloat  minX, minY;
		CFloat  maxX, maxY;
		CFloat  posX, posY;

		/* set the shape of the face */
		CStatus_CheckGOTO
			(CFontFace_SetShape
				(_this->face, device, _this->points, &shape, &scaleX, &scaleY),
			 status,
			 GOTO_FailB);

		/* bail out now if there's nothing to do */
		if(scaleX == 0 || scaleY == 0)
		{
			CStatus_CheckGOTO(CStatus_OK, status, GOTO_FailB);
		}

		/* disable cache memory management */
		CFontFace_DisableMemoryManagement(face);

		/* initialize the bounds and position to the default */
		minX = minY = 0.0f;
		maxX = maxY = 0.0f;
		posX = posY = 0.0f;

		/* initialize the previous character */
		prev = ((CChar32)-1);

		/* calculate the metrics */
		while(length > 0)
		{
			/* declarations */
			CGlyphEntry *entry;
			CChar32      unicode;
			CUInt32      len;
			CFloat       x1, y1;
			CFloat       x2, y2;

			/* skip encoding errors */
			if((len = CUtils_Char16ToChar32(string, &unicode, length)) == 0)
			{
				++string;
				--length;
				continue;
			}

			/* get the current glyph entry, as needed */
			if(unicode != prev)
			{
				/* get the current glyph entry */
				CStatus_CheckGOTO
					(CFontFace_GetGlyphEntry
						(face, unicode, flags, 0, &entry),
					 status,
					 GOTO_FailC);
			}

			/* get the glyph bounds */
			x1 = posX + CVector_X(entry->metrics.bearing);
			y1 = posY + CVector_Y(entry->metrics.bearing);
			x2 =   x1 + CSize_Width(entry->metrics.size);
			y2 =   y1 + CSize_Height(entry->metrics.size);

			/* adjust the total bounds */
			if(prev == ((CChar32)-1))
			{
				minX = x1;
				minY = y1;
				maxX = x2;
				maxY = y2;
			}
			else
			{
				if(minX > x1) { minX = x1; }
				if(minY > y1) { minY = y1; }
				if(maxX < x2) { maxX = x2; }
				if(maxY < y2) { maxY = y2; }
			}

			/* update the current position */
			posX += CVector_X(entry->metrics.advance);
			posY += CVector_Y(entry->metrics.advance);

			/* update the previous character */
			prev = unicode;

			/* update the input pointer */
			string += len;

			/* update the input length */
			length -= len;
		}

		/* reenable cache memory management */
		CFontFace_EnableMemoryManagement(face);

		/* set the metrics */
		if(scaleX == 1.0f && scaleY == 1.0f)
		{
			CSize_Width(metrics->size)  = (maxX - minX);
			CSize_Height(metrics->size) = (maxY - minY);
			CVector_X(metrics->bearing) = (minX);
			CVector_Y(metrics->bearing) = (minY);
			CVector_X(metrics->advance) = (posX);
			CVector_Y(metrics->advance) = (posY);
		}
		else
		{
			/* calculate the inverse scale */
			scaleX = (1.0f / scaleX);
			scaleY = (1.0f / scaleY);

			/* set the metrics */
			CSize_Width(metrics->size)  = ((maxX - minX) * scaleX);
			CSize_Height(metrics->size) = ((maxY - minY) * scaleY);
			CVector_X(metrics->bearing) = ((minX)        * scaleX);
			CVector_Y(metrics->bearing) = ((minY)        * scaleY);
			CVector_X(metrics->advance) = ((posX)        * scaleX);
			CVector_Y(metrics->advance) = ((posY)        * scaleY);
		}
	}
	CMutex_Unlock(face->lock);

	/* return successfully */
	return CStatus_OK;

	/* handle failures */
	{
	GOTO_FailC:
		/* reenable cache memory management */
		CFontFace_EnableMemoryManagement(face);

	GOTO_FailB:
		/* unlock the face */
		CMutex_Unlock(face->lock);

	GOTO_FailA:
		/* set the metrics to the default */
		*metrics = CTextMetrics_Zero;

		/* return status */
		return status;
	}
}

/*\
|*| Measure a string.
|*|
|*|     _this - this font
|*|    string - string to measure
|*|    length - length of string
|*|    device - device transformation
|*|      hint - text rendering hint
|*|   metrics - metrics of string (returned)
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFont_DrawString(CFont              *_this,
                 const CChar16      *string,
                 CUInt32             length,
                 CFloat              x,
                 CFloat              y,
                 CAffineTransformF  *device,
                 CTextRenderingHint  hint,
                 pixman_image_t     *clip,
                 pixman_image_t     *mask)
{
	/* declarations */
	CFontFace   *face;
	CGlyphShape *shape;
	CGlyphFlag   flags;
	CFloat       scaleX;
	CFloat       scaleY;
	CStatus      status;
	CFloat       posX;
	CFloat       posY;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((clip  != 0));
	CASSERT((mask  != 0));

	/* bail out now if there's nothing to do */
	CStatus_Require((string != 0 && length != 0), CStatus_OK);

	/* get the depth and hinting */
	flags = _GetGlyphFlags(hint);

	/* get the font face */
	face = _this->face;

	/* initialize the position and move to the baseline */
	posX = (x + 0.5);
	posY = (y + 0.5) + _CalcBaseline(_this, face->face);

	/* draw the string, synchronously */
	CMutex_Lock(face->lock);
	{
		/* declarations */
		CChar32 prev;

		/* set the shape of the face */
		CStatus_CheckGOTO
			(CFontFace_SetShape
				(_this->face, device, _this->points, &shape, &scaleX, &scaleY),
			 status,
			 GOTO_CleanupA);

		/* bail out now if there's nothing to do */
		if(scaleX == 0 || scaleY == 0)
		{
			CStatus_CheckGOTO(CStatus_OK, status, GOTO_CleanupA);
		}

		/* disable cache memory management */
		CFontFace_DisableMemoryManagement(face);

		/* initialize the previous character */
		prev = ((CChar32)-1);

		/* draw the string */
		while(length > 0)
		{
			/* declarations */
			CGlyphEntry *entry;
			CChar32      unicode;
			CUInt32      len;
			int          gX, gY;

			/* skip encoding errors */
			if((len = CUtils_Char16ToChar32(string, &unicode, length)) == 0)
			{
				++string;
				--length;
				continue;
			}
			else
			{
				string += len;
				length -= len;
			}

			/* get the current glyph entry, as needed */
			if(unicode != prev)
			{
				/* get the current glyph entry */
				CStatus_CheckGOTO
					(CFontFace_GetGlyphEntry
						(face, unicode, flags, 1, &entry),
					 status,
					 GOTO_CleanupB);
			}

			/* skip this character, as needed */
			if(entry->mask == CGlyphEntry_NullMask)
			{
				prev = unicode;
				continue;
			}

			/* calculate the pixel position of the glyph */
			gX = (int)(posX + CVector_X(entry->xbearing));
			gY = (int)(posY + CVector_Y(entry->xbearing));

			/* add the glyph to the mask */
			pixman_composite
				(PIXMAN_OPERATOR_ADD, entry->mask, clip, mask, 0, 0, 0, 0,
				 gX, gY, 32767, 32767);

			/* update the current position */
			posX += CVector_X(entry->xadvance);
			posY += CVector_Y(entry->xadvance);

			/* update the previous character */
			prev = unicode;
		}

	GOTO_CleanupB:
		/* enable cache memory management */
		CFontFace_EnableMemoryManagement(face);
	}
GOTO_CleanupA:
	CMutex_Unlock(face->lock);

	/* return status */
	return status;
}

/* Create a font. */
CStatus
CFont_Create(CFont         **_this,
             CFontFamily    *family,
             CFontStyle      style,
             CFloat          size,
             CGraphicsUnit   unit)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a font family pointer */
	CStatus_Require((family != 0), CStatus_ArgumentNull);

	/* allocate the font */
	if(!(*_this = (CFont *)CCalloc(1, sizeof(CFont))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the font */
	{
		/* declarations */
		CFont   *font;
		CStatus  status;

		/* get the font */
		font = *_this;

		/* get the font face from the family */
		status = CFontFamily_GetFace(family, style, &(font->face));

		/* handle face getter failures */
		if(status != CStatus_OK || font->face == CFontFace_Unavailable)
		{
			CFree(*_this);
			*_this = 0;
			return (status ? status : CStatus_Argument_StyleNotAvailable);
		}

		/* reference the family */
		CFontFamily_Reference(family);

		/* initialize the simple members */
		font->style = style;
		font->size  = size;
		font->unit  = unit;

		/* initialize the points */
		font->points = CUtils_ConvertUnits(unit, CGraphicsUnit_Point, size);

		/* initialize the pixels */
		font->pixels =
			CUtils_ConvertUnits
				(unit, CGraphicsUnit_Pixel, CFont_GetHeightInternal(font));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Destroy a font. */
CStatus
CFont_Destroy(CFont **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a this pointer */
	CStatus_Require((*_this != 0), CStatus_ArgumentNull);

	/* finalize the font */
	{
		/* declarations */
		CFontFamily *family;

		/* get the font family */
		family = (*_this)->face->family;

		/* dereference the font family */
		CFontFamily_Destroy(&family);
	}

	/* free the font */
	CFree(*_this);

	/* null the font */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/* Clone a font. */
CStatus
CFont_Clone(CFont  *_this,
            CFont **clone)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a clone pointer pointer */
	CStatus_Require((clone != 0), CStatus_ArgumentNull);

	/* allocate the clone */
	if(!(*clone = (CFont *)CCalloc(1, sizeof(CFont))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the clone */
	{
		/* declarations */
		CFont *font;

		/* get the font */
		font = *clone;

		/* initialize the face */
		font->face = _this->face;

		/* reference the family */
		CFontFamily_Reference(font->face->family);

		/* initialize the remaining members */
		font->points = _this->points;
		font->pixels = _this->pixels;
		font->style  = _this->style;
		font->size   = _this->size;
		font->unit   = _this->unit;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Determine if fonts are equal. */
CStatus
CFont_Equals(CFont *_this,
             CFont *other,
             CBool *equal)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an other pointer */
	CStatus_Require((other != 0), CStatus_ArgumentNull);

	/* ensure we have an equal pointer */
	CStatus_Require((equal != 0), CStatus_ArgumentNull);

	/* compare the fonts */
	*equal = (CMemCmp(_this, other, sizeof(CFont)) == 0);

	/* return successfully */
	return CStatus_OK;
}

/* Get the font family of a font. */
CStatus
CFont_GetFontFamily(CFont        *_this,
                    CFontFamily **family)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a family pointer */
	CStatus_Require((family != 0), CStatus_ArgumentNull);

	/* get the family */
	*family = _this->face->family;

	/* reference the family */
	CFontFamily_Reference(*family);

	/* return successfully */
	return CStatus_OK;
}

/* Get the hash code for a font. */
CStatus
CFont_GetHashCode(CFont   *_this,
                  CUInt32 *hash)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a hash pointer */
	CStatus_Require((hash != 0), CStatus_ArgumentNull);

	/* hash the font */
	*hash =
		CUtils_HashBest
			((CByte *)_this, sizeof(CFont), CUtils_HashBest_Init);

	/* return successfully */
	return CStatus_OK;
}

/* Get the pixel height of a font. */
CStatus
CFont_GetHeight(CFont  *_this,
                CFloat *height)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a height pointer */
	CStatus_Require((height != 0), CStatus_ArgumentNull);

	/* get the pixel height */
	*height = _this->pixels;

	/* return successfully */
	return CStatus_OK;
}

/* Get the pixel height of a font for the given resolution. */
CStatus
CFont_GetHeightDPI(CFont  *_this,
                   CFloat  dpiY,
                   CFloat *height)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a height pointer */
	CStatus_Require((height != 0), CStatus_ArgumentNull);

	/* calculate the height using the given dpi */
	*height =
		CUtils_ConvertUnitsDPI
			(_this->unit, CGraphicsUnit_Pixel, CFont_GetHeightInternal(_this),
			 CGraphics_DefaultDpi, dpiY);

	/* return successfully */
	return CStatus_OK;
}

/* Get the height in the units and resolution of a graphics context. */
CStatus
CFont_GetHeightGraphics(CFont     *_this,
                        CGraphics *graphics,
                        CFloat    *height)
{
	/* declarations */
	CFloat        dpiY;
	CGraphicsUnit unit;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a graphics context pointer */
	CStatus_Require((graphics != 0), CStatus_ArgumentNull);

	/* ensure we have a height pointer */
	CStatus_Require((height != 0), CStatus_ArgumentNull);

	/* get the dpi of the graphics context */
	CStatus_Check((CGraphics_GetDpiY(graphics, &dpiY)));

	/* get the unit of the graphics context */
	CStatus_Check((CGraphics_GetPageUnit(graphics, &unit)));

	/* calculate the height using the graphics context settings */
	*height =
		CUtils_ConvertUnitsDPI
			(_this->unit, unit, CFont_GetHeightInternal(_this),
			 CGraphics_DefaultDpi, dpiY);

	/* return successfully */
	return CStatus_OK;
}

/* Get the name of a font. */
CStatus
CFont_GetName(CFont    *_this,
              CChar16 **name)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the family name */
	CStatus_Check(CFontFamily_GetName(_this->face->family, name));

	/* return successfully */
	return CStatus_OK;
}

/* Get the size of a font. */
CStatus
CFont_GetSize(CFont  *_this,
              CFloat *size)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a size pointer */
	CStatus_Require((size != 0), CStatus_ArgumentNull);

	/* get the size */
	*size = _this->size;

	/* return successfully */
	return CStatus_OK;
}

/* Get the point size of a font. */
CStatus
CFont_GetSizeInPoints(CFont  *_this,
                      CFloat *points)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a points pointer */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* get the point size */
	*points = _this->points;

	/* return successfully */
	return CStatus_OK;
}

/* Get the style of a font. */
CStatus
CFont_GetStyle(CFont      *_this,
               CFontStyle *style)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a style pointer */
	CStatus_Require((style != 0), CStatus_ArgumentNull);

	/* get the font style */
	*style = _this->style;

	/* return successfully */
	return CStatus_OK;
}

/* Get the units of a font. */
CStatus
CFont_GetUnit(CFont         *_this,
              CGraphicsUnit *unit)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a unit pointer */
	CStatus_Require((unit != 0), CStatus_ArgumentNull);

	/* get the unit */
	*unit = _this->unit;

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
