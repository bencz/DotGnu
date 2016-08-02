/*
 * CFontFace.h - Font face header.
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

#include "CFontFace.h"
#include "CFontCollection.h"
#include "CAffineTransform.h"
#include "CUtils.h"
#include <fontconfig/fcfreetype.h>
#include <ft2build.h>
#include FT_OUTLINE_H

#ifdef __cplusplus
extern "C" {
#endif

/* marker object for unavailable faces */
CUInt32 CFontFace_UnavailableObject = 0;

/* marker object for null masks */
CUInt32 CGlyphEntry_NullMaskObject = 0;

/* freetype identity matrix */
static const FT_Matrix CFreeTypeMatrix_Identity =
{
	CFixed_One,  CFixed_Zero,
	CFixed_Zero, CFixed_One
};

/* Get the load flags for the given glyph flags. */
static CInt32
_GetLoadFlags(CGlyphFlag flags)
{
	/* declarations */
	CInt32 load;

	/* set the load flags to the default */
	load = (FT_LOAD_NO_BITMAP);

	/* set the depth */
	if((flags & CGlyphFlag_AntiAlias) != 0)
	{
		load |= (FT_LOAD_TARGET_NORMAL);
	}
	else
	{
		load |= (FT_LOAD_TARGET_MONO | FT_LOAD_MONOCHROME);
	}

	/* disable hinting, as needed */
	if((flags & CGlyphFlag_Hinting) == 0)
	{
		load |= (FT_LOAD_NO_HINTING);
	}

	/* return the load flags */
	return load;
}

/*\
|*| Create a glyph shape.
|*|
|*|   _this - this glyph shape (created)
|*|     key - glyph shape members
|*|
|*|  Returns status code.
\*/
static CStatus
CGlyphShape_Create(CGlyphShape **_this,
                   CGlyphShape  *key)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((key   != 0));

	/* allocate the shape */
	if(!(*_this = (CGlyphShape *)CMalloc(sizeof(CGlyphShape))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the members */
	**_this = *key;

	/* initialize the reference count */
	(*_this)->refCount = 1;

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Initialize a glyph shape search key.
|*|
|*|     _this - this glyph shape
|*|    matrix - unscaled transformation matrix
|*|   pointsX - scaled horizontal point size
|*|   pointsY - scaled vertical point size
|*|
|*|  NOTE: this should only be used to initialize search
|*|        keys for shapes and for initializing shape
|*|        construction "key" arguments
\*/
static void
CGlyphShape_Initialize(CGlyphShape *_this,
                       FT_Matrix   *matrix,
                       FT_F26Dot6   pointsX,
                       FT_F26Dot6   pointsY)
{
	/* declarations */
	CGlyphShapeData *sd;

	/* assertions */
	CASSERT((_this != 0));

	/*\
	|*| NOTE: to ensure that any padding within the shape
	|*|       structure doesn't screw up hashing or
	|*|       comparisons, we clear the structure to rid
	|*|       it of any garbage data
	\*/

	/* clear the shape */
	CMemSet(_this, 0x00, sizeof(CGlyphShape));

	/* get the glyph shape data pointer */
	sd = &(_this->data);

	/* initialize the matrix */
	if(matrix == 0)
	{
		sd->matrix.xx = CFixed_One;
		sd->matrix.xy = CFixed_Zero;
		sd->matrix.yx = CFixed_Zero;
		sd->matrix.yy = CFixed_One;
	}
	else
	{
		sd->matrix.xx = matrix->xx;
		sd->matrix.xy = matrix->xy;
		sd->matrix.yx = matrix->yx;
		sd->matrix.yy = matrix->yy;
	}

	/* initialize the point size */
	sd->pointsX = pointsX;
	sd->pointsY = pointsY;

	/* initialize the base */
	_this->_base.hash =
		CUtils_HashBest
			((CByte *)sd, sizeof(CGlyphShapeData), CUtils_HashBest_Init);

	/* initialize the reference count */
	_this->refCount = 1;
}

/*\
|*| Reference a glyph shape.
|*|
|*|   _this - this glyph shape
\*/
static void
CGlyphShape_Reference(CGlyphShape *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* update the reference count */
	++(_this->refCount);
}

/*\
|*| Initialize a glyph entry search key.
|*|
|*|   _this - this glyph key
|*|   shape - shape of the glyph
|*|   index - index of the glyph
|*|   flags - glyph flags
\*/
static void
CGlyphKey_Initialize(CGlyphKey   *_this,
                     CGlyphShape *shape,
                     CGlyphIndex  index,
                     CGlyphFlag   flags)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((shape != 0));

	/* initialize the members */
	_this->shape = shape;
	_this->index = index;
	_this->flags = flags;

	/* initialize the base */
	{
		/* declarations */
		CUInt32 hash;
		CUInt32 tmp[2];

		/* get the initial hash value */
		hash = ((CHashEntry *)shape)->hash;

		/* initialize the data */
		tmp[0] = index;
		tmp[1] = flags;

		/* hash the data */
		hash = CUtils_HashBest(((CByte *)tmp), sizeof(tmp), hash);

		/* initialize the hash value */
		((CCacheEntry *)_this)->hash = hash;

		/* initialize the memory usage */
		((CCacheEntry *)_this)->memory = sizeof(CGlyphEntry);
	}
}

/* Measure the current glyph into the given entry. */
static void
CFontFace_MeasureGlyph(CFontFace   *_this,
                       CGlyphEntry *entry)
{
	/* declarations */
	FT_Glyph_Metrics *metrics;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((entry != 0));

	/* get the glyph metrics */
	metrics = &(_this->face->glyph->metrics);

	/*\
	|*| TODO: add support for vertical layout
	\*/

	/* get the metrics */
	CSize_Width(entry->metrics.size)  = CF26Dot6_ToFloat(metrics->width);
	CSize_Height(entry->metrics.size) = CF26Dot6_ToFloat(metrics->height);
	CVector_X(entry->metrics.bearing) = CF26Dot6_ToFloat(metrics->horiBearingX);
	CVector_Y(entry->metrics.bearing) = CF26Dot6_ToFloat(metrics->horiBearingY);
	CVector_X(entry->metrics.advance) = CF26Dot6_ToFloat(metrics->horiAdvance);
	CVector_Y(entry->metrics.advance) = 0;
}

/* Render the current glyph into the given entry. */
static CStatus
CFontFace_RenderGlyph(CFontFace   *_this,
                      CGlyphEntry *entry,
                      CUInt32     *memory)
{
	/* declarations */
	pixman_format_t *format;
	FT_Outline      *outline;
	FT_GlyphSlot     glyph;
	FT_Bitmap        bitmap;
	FT_BBox          cbox;
	FT_Error         error;
	CBool            antialias;
	int              width;
	int              height;
	int              bpp;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((entry  != 0));
	CASSERT((memory != 0));

	/* get the glyph */
	glyph = _this->face->glyph;

	/* get the antialias flag */
	antialias = ((entry->_base.flags & CGlyphFlag_AntiAlias) != 0);

	/* ensure we have an outline */
	if(glyph->format != FT_GLYPH_FORMAT_OUTLINE)
	{
		CASSERT((glyph->format == FT_GLYPH_FORMAT_PLOTTER));
		entry->mask   = CGlyphEntry_NullMask;
		*memory       = 0;
		return CStatus_OK;
	}

	/* get the outline */
	outline = &(glyph->outline);

	/* get the control box of the outline */
	FT_Outline_Get_CBox(outline, &cbox);

	/* set the transformed metrics */
	CVector_X(entry->xbearing) =  CF26Dot6_ToFloat(cbox.xMin);
	CVector_Y(entry->xbearing) = -CF26Dot6_ToFloat(cbox.yMin);
	CVector_X(entry->xadvance) =  CF26Dot6_ToFloat(glyph->advance.x);
	CVector_Y(entry->xadvance) = -CF26Dot6_ToFloat(glyph->advance.y);

	/* grid fit the control box */
	cbox.xMin = CF26Dot6_Floor(cbox.xMin);
	cbox.yMin = CF26Dot6_Floor(cbox.yMin);
	cbox.xMax = CF26Dot6_Ceil(cbox.xMax);
	cbox.yMax = CF26Dot6_Ceil(cbox.yMax);

	/* calculate the width and height */
	width  = (int)CF26Dot6_Trunc(cbox.xMax - cbox.xMin);
	height = (int)CF26Dot6_Trunc(cbox.yMax - cbox.yMin);

	/* bail out now if there's nothing to do */
	if(width == 0 || height == 0)
	{
		entry->mask   = CGlyphEntry_NullMask;
		*memory       = 0;
		return CStatus_OK;
	}

	/* set up the bitmap and mask formats */
	if(antialias)
	{
		/* set up the bitmap format */
		bitmap.pixel_mode = FT_PIXEL_MODE_GRAY;
		bitmap.num_grays  = 256;
		bitmap.pitch      = (((width +  3) &  ~3) >> 0);

		/* create the mask format */
		if(!(format = pixman_format_create(PIXMAN_FORMAT_NAME_A8)))
		{
			*memory = 0;
			return CStatus_OutOfMemory;
		}

		/* initialize the bpp */
		bpp = 8;
	}
	else
	{
		/* set up the bitmap format */
		bitmap.pixel_mode = FT_PIXEL_MODE_MONO;
		bitmap.num_grays  = 1;
		bitmap.pitch      = (((width + 31) & ~31) >> 3);

		/* create the mask format */
		if(!(format = pixman_format_create(PIXMAN_FORMAT_NAME_A1)))
		{
			*memory = 0;
			return CStatus_OutOfMemory;
		}

		/* initialize the bpp */
		bpp = 1;
	}

	/* calculate the memory usage */
	*memory = (bitmap.pitch * height);

	/* set the bitmap width and height */
	bitmap.width = width;
	bitmap.rows  = height;

	/* allocate the buffer */
	if(!(bitmap.buffer = (unsigned char*)CCalloc(1, *memory)))
	{
		pixman_format_destroy(format);
		*memory = 0;
		return CStatus_OutOfMemory;
	}

	/* reposition the outline to the origin */
	FT_Outline_Translate(outline, -cbox.xMin, -cbox.yMin);

	/* rasterize the outline */
	if((error = FT_Outline_Get_Bitmap(_this->library, outline, &bitmap)))
	{
		pixman_format_destroy(format);
		CFree(bitmap.buffer);
		*memory = 0;
		return CStatus_OutOfMemory;
	}

	/* ensure the buffer is in the right format */
	if(!antialias && CUtils_IsLittleEndian())
	{
		CUtils_ReverseBytes((CByte *)bitmap.buffer, *memory);
	}

	/* create the mask */
	entry->mask =
		pixman_image_create_for_data
			((pixman_bits_t *)bitmap.buffer, format, width, height, bpp,
			 (int)bitmap.pitch);

	/* destroy the pixman format */
	pixman_format_destroy(format);

	/* handle mask creation failures */
	if(entry->mask == 0)
	{
		CFree(bitmap.buffer);
		*memory = 0;
		return CStatus_OutOfMemory;
	}

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Create a font face.
|*|
|*|    _this - this font face (created)
|*|   family - family of face (must hold collection lock)
|*|    style - style of face
\*/
CINTERNAL CStatus
CFontFace_Create(CFontFace   **_this,
                 CFontFamily  *family,
                 CFontStyle    style)
{
	/* declarations */
	CFontCollection *fc;
	CFontFace       *ff;
	FcChar8         *file;
	FcPattern       *match;
	FcCharSet       *charset;
	FcResult         result;
	FT_Error         error;
	CStatus          status;
	int              index;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((family != 0));

	/* get the font collection */
	fc = family->collection;

	/* set this to the default */
	*_this = 0;

	/* get the matching pattern */
	CStatus_Check
		(CFontCollection_MatchStyle
			(fc, style, family->_base.pattern, &match));

	#define CFontFace_FcResultToCStatus(result) \
		(((result) == FcResultMatch)   ? CStatus_OK : \
		 ((result) == FcResultNoMatch) ? CStatus_Argument_FontFamilyNotFound : \
		 (CStatus_OutOfMemory))

	/* get the file name for the font */
	result = FcPatternGetString(match, FC_FILE, 0, &file);

	/* handle file name getter failures */
	CStatus_CheckGOTO
		(CFontFace_FcResultToCStatus(result), status, GOTO_FailA);

	/* get the index for the font */
	result = FcPatternGetInteger(match, FC_INDEX, 0, &index);

	/* handle index getter failures */
	CStatus_CheckGOTO
		(CFontFace_FcResultToCStatus(result), status, GOTO_FailA);

	/* allocate the font face */
	if(!(*_this = (CFontFace *)CMalloc(sizeof(CFontFace))))
	{
		CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailA);
	}

	/* get the font face */
	ff = *_this;

	/* create the freetype library */
	if((error = FT_Init_FreeType(&(ff->library))))
	{
		CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailB);
	}

	/* create the face from the filename and index */
	if((error = FT_New_Face(ff->library, (char *)file, index, &(ff->face))))
	{
		CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailC);
	}

	/* ensure we have a scalable face */
	CASSERT((FT_IS_SCALABLE(ff->face)));

	/* create the unicode hash table */
	{
		/* get the character set */
		result = FcPatternGetCharSet(match, FC_CHARSET, 0, &charset);

		/* copy or create the character set, as needed */
		if(result == FcResultMatch)
		{
			charset = FcCharSetCopy(charset);
		}
		else
		{
			charset =
				FcFreeTypeCharSet
					(ff->face, FcConfigGetBlanks(fc->config));
		}

		/* create the table or handle character set failures */
		if(charset == 0)
		{
			CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailD);
		}
		else if(FcCharSetCount(charset) == 0)
		{
			CStatus_CheckGOTO
				(CStatus_Argument_FontFamilyNotFound, status, GOTO_FailE);
		}
		else
		{
			CStatus_CheckGOTO
				(CUnicodeHashTable_Create(&(ff->unicode), charset, ff->face),
				 status, GOTO_FailE);
			charset = 0;
		}
	}

	/* create the glyph cache */
	CStatus_CheckGOTO(CGlyphCache_Create(&(ff->cache)), status, GOTO_FailF);

	/* create the mutex */
	CStatus_CheckGOTO(CMutex_Create(&(ff->lock)), status, GOTO_FailG);

	/* initialize the family */
	ff->family = family;

	/* initialize the current glyph shape */
	ff->current = 0;

	/* return successfully */
	return CStatus_OK;

	/* handle failures */
	{
	GOTO_FailG:
		/* destroy the glyph cache */
		CGlyphCache_Destroy(&(ff->cache));

	GOTO_FailF:
		/* destroy the unicode hash table */
		CUnicodeHashTable_Destroy(&(ff->unicode));

	GOTO_FailE:
		/* destroy the character set, as needed */
		if(charset != 0) { FcCharSetDestroy(charset); }

	GOTO_FailD:
		/* dispose of the freetype face */
		FT_Done_Face(ff->face);

	GOTO_FailC:
		/* dispose of the freetype library */
		FT_Done_FreeType(ff->library);

	GOTO_FailB:
		/* free the font face */
		CFree(*_this);

		/* null the font face */
		*_this = 0;

	GOTO_FailA:
		/* destroy the matching pattern */
		FcPatternDestroy(match);

		/* handle unavailable case */
		if(status == CStatus_Argument_FontFamilyNotFound)
		{
			*_this = CFontFace_Unavailable;
			status = CStatus_OK;
		}

		/* return status */
		return status;
	}
}

/*\
|*| Destroy a font face.
|*|
|*|   _this - this font face
\*/
CINTERNAL void
CFontFace_Destroy(CFontFace **_this)
{
	/* declarations */
	CFontFace *face;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((*_this != 0));

	/* get the face */
	face = *_this;

	/* destroy the mutex */
	CMutex_Destroy(&(face->lock));

	/* destroy the glyph cache */
	CGlyphCache_Destroy(&(face->cache));

	/* destroy the unicode hash table */
	CUnicodeHashTable_Destroy(&(face->unicode));

	/* dispose of the freetype face */
	FT_Done_Face(face->face);

	/* dispose of the freetype library */
	FT_Done_FreeType(face->library);

	/* free the face */
	CFree(*_this);

	/* null the face */
	*_this = 0;
}

/*\
|*| Get a glyph entry for the current shape of a font face.
|*|
|*|     _this - this font face (must be locked)
|*|   unicode - unicode character
|*|     flags - glyph measuring and rendering flags
|*|    render - rendering flag
|*|     entry - glyph entry (returned)
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFontFace_GetGlyphEntry(CFontFace    *_this,
                        CChar32       unicode,
                        CGlyphFlag    flags,
                        CBool         render,
                        CGlyphEntry **entry)
{
	/* declarations */
	CGlyphKey   key;
	CGlyphIndex index;

	/* get the glyph index */
	index = CUnicodeHashTable_GetGlyphIndex(_this->unicode, unicode);

	/* initialize the key */
	CGlyphKey_Initialize
		(&key, _this->current, index, flags);

	/* get the entry */
	if((*entry = CGlyphCache_GetEntry(_this->cache, &key)) != 0)
	{
		/* declarations */
		CGlyphEntry *e;
		CUInt32      memory;
		CStatus      status;
		FT_Error     error;

		/* bail out now if we have what we need */
		CStatus_Require((render != 0), CStatus_OK);

		/* get the entry */
		e = *entry;

		/* bail out now if we have what we need */
		CStatus_Require((e->mask == CGlyphEntry_NullMask), CStatus_OK);

		/* load the glyph */
		if((error = FT_Load_Glyph(_this->face, index, _GetLoadFlags(flags))))
		{
			*entry = 0;
			return CStatus_OutOfMemory;
		}

		/* render the glyph */
		if((status = CFontFace_RenderGlyph(_this, e, &memory)) != CStatus_OK)
		{
			*entry = 0;
			return status;
		}

		/* update the memory usage, as needed */
		if(memory != 0)
		{
			/*\
			|*| NOTE: we remove the entry, then add it back to
			|*|       the cache after updating its memory usage,
			|*|       to ensure that the cache has an accurate
			|*|       measure of the current total memory usage
			\*/

			/* remove the entry from the cache */
			CGlyphCache_RemoveEntry(_this->cache, e);

			/* update the entry memory usage */
			((CCacheEntry *)e)->memory += memory;

			/* add the entry back to the cache */
			if((status = CGlyphCache_AddEntry(_this->cache, e)) != CStatus_OK)
			{
				CGlyphEntry_Destroy(entry);
				return status;
			}
		}
	}
	else
	{
		/* declarations */
		CGlyphEntry *e;
		CStatus      status;
		FT_Error     error;

		/* allocate the entry */
		if(!(*entry = (CGlyphEntry *)CMalloc(sizeof(CGlyphEntry))))
		{
			return CStatus_OutOfMemory;
		}

		/* get the entry */
		e = *entry;

		/* initialize the base */
		e->_base = key;

		/* load the glyph */
		if((error = FT_Load_Glyph(_this->face, index, _GetLoadFlags(flags))))
		{
			CFree(*entry);
			*entry = 0;
			return CStatus_OutOfMemory;
		}

		/* measure the glyph */
		CFontFace_MeasureGlyph(_this, e);

		/* initialize the mask information */
		if(render == 0)
		{
			e->mask = 0;
		}
		else
		{
			/* declarations */
			CUInt32 memory;

			/* render the glyph */
			status = CFontFace_RenderGlyph(_this, e, &memory);

			/* handle rendering failures */
			if(status != CStatus_OK)
			{
				CFree(*entry);
				*entry = 0;
				return status;
			}

			/* update the memory usage */
			((CCacheEntry *)e)->memory += memory;
		}

		/* add the entry to the cache */
		if((status = CGlyphCache_AddEntry(_this->cache, e)) != CStatus_OK)
		{
			CGlyphEntry_Destroy(entry);
			return status;
		}

		/* reference the shape */
		CGlyphShape_Reference(_this->current);
	}

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Set the glyph shape for this face.
|*|
|*|    _this - this font face (must be locked)
|*|   device - world to device transformation
|*|   points - point size
|*|    shape - glyph shape (returned)
|*|   scaleX - extracted horizontal scale (returned)
|*|   scaleY - extracted vertical scale (returned)
|*|
|*|  NOTE: to get metrics back to user space, scale by the
|*|        inverse of the extracted scaling factors
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFontFace_SetShape(CFontFace                *_this,
                   const CAffineTransformF  *device,
                   CFloat                    points,
                   CGlyphShape             **shape,
                   CFloat                   *scaleX,
                   CFloat                   *scaleY)
{
	/* declarations */
	CGlyphShape key;
	CBool       created;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((device != 0));
	CASSERT((shape  != 0));
	CASSERT((scaleX != 0));
	CASSERT((scaleY != 0));

	/* initialize the key */
	if(device == 0 ||
	   CAffineTransformF_Equals(device, &CAffineTransformF_Identity))
	{
		/* set the scale */
		*scaleX = 1.0f;
		*scaleY = 1.0f;

		/* initialize the key */
		CGlyphShape_Initialize
			(&key,
			 0,
			 CFloat_ToF26Dot6(points),
			 CFloat_ToF26Dot6(points));
	}
	else
	{
		/* declarations */
		CAffineTransformF transform;
		FT_Matrix         matrix;

		/* get the transformation */
		transform = *device;

		/* extract the scale */
		CAffineTransformF_ExtractScale
			(&transform, scaleX, scaleY, CMatrixOrder_Append);

		/* bail out now if there's nothing to do */
		if(*scaleX == 0 || *scaleY == 0) { return CStatus_OK; }

		/* initialize the freetype matrix */
		matrix.xx = CFloat_ToFixed(CAffineTransform_XX(transform));
		matrix.xy = CFloat_ToFixed(CAffineTransform_XY(transform));
		matrix.yx = CFloat_ToFixed(CAffineTransform_YX(transform));
		matrix.yy = CFloat_ToFixed(CAffineTransform_YY(transform));

		/* initialize the key */
		CGlyphShape_Initialize
			(&key,
			 &matrix,
			 CFloat_ToF26Dot6(points * (*scaleX)),
			 CFloat_ToF26Dot6(points * (*scaleY)));
	}

	/* set the created flag */
	created = 0;

	/* get the shape */
	if(!(*shape = CGlyphCache_GetShape(_this->cache, &key)))
	{
		/* declarations */
		CStatus status;

		/* create the shape */
		CStatus_Check
			(CGlyphShape_Create(shape, &key));

		/* add the shape to the shape table */
		status = CGlyphCache_AddShape(_this->cache, *shape);

		/* handle hash table failures */
		if(status != CStatus_OK)
		{
			CGlyphShape_Destroy(shape);
			return status;
		}

		/* update the created flag */
		created = 1;
	}

	/* change shapes, as needed */
	if(*shape != _this->current)
	{
		/* declarations */
		CGlyphShapeData *c;
		CGlyphShapeData *s;

		/* reference the shape */
		if(!created) { CGlyphShape_Reference(*shape); }

		/* get the shape data */
		c = &(_this->current->data);
		s = &((*shape)->data);

		/* switch to new shape */
		if(_this->current == 0)
		{
			/* set the transformation */
			FT_Set_Transform(_this->face, &(s->matrix), 0);

			/* set the point size */
			FT_Set_Char_Size
				(_this->face, s->pointsX, s->pointsY,
				 (FT_UInt)CGraphics_DefaultDpi,
				 (FT_UInt)CGraphics_DefaultDpi);
		}
		else
		{
			/* update the transformation, as needed */
			if(CMemCmp(&(c->matrix), &(s->matrix), sizeof(FT_Matrix)) != 0)
			{
				FT_Set_Transform(_this->face, &(s->matrix), 0);
			}

			/* update the point size, as needed */
			if(c->pointsX != s->pointsX || c->pointsY != s->pointsY)
			{
				FT_Set_Char_Size
					(_this->face, s->pointsX, s->pointsY,
					 (FT_UInt)CGraphics_DefaultDpi,
					 (FT_UInt)CGraphics_DefaultDpi);
			}

			/* destroy the current shape */
			CGlyphCache_DestroyShape(_this->cache, &(_this->current));
		}

		/* update the current shape */
		_this->current = *shape;
	}

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Disable cache memory management.
|*|
|*|   _this - this font face
|*|
|*|  NOTE: this method disables the removal of glyph
|*|        entries due to memory pressure
\*/
CINTERNAL void
CFontFace_DisableMemoryManagement(CFontFace *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* disable cache memory management */
	CGlyphCache_DisableMemoryManagement(_this->cache);
}

/*\
|*| Reenable cache memory management.
|*|
|*|   _this - this font face
|*|
|*|  NOTE: this method reenables the removal of glyph
|*|        entries due to memory pressure
\*/
CINTERNAL void
CFontFace_EnableMemoryManagement(CFontFace *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* reenable cache memory management */
	CGlyphCache_EnableMemoryManagement(_this->cache);
}

/*\
|*| Destroy a glyph entry.
|*|
|*|   _this - this glyph entry
|*|
|*|  NOTE: this should only be used on entries which are
|*|        known not to exist in, or have been removed
|*|        from, the glyph cache
\*/
CINTERNAL void
CGlyphEntry_Destroy(CGlyphEntry **_this)
{
	/* declarations */
	CGlyphEntry *entry;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((*_this != 0));

	/* get the entry */
	entry = *_this;

	/* free the mask, as needed */
	if(entry->mask != 0 && entry->mask != CGlyphEntry_NullMask)
	{
		CFree(pixman_image_get_data(entry->mask));
		pixman_image_destroy(entry->mask);
	}

	/* free the entry */
	CFree(entry);

	/* null the entry */
	*_this = 0;
}

/*\
|*| Destroy a glyph shape.
|*|
|*|   _this - this glyph shape
|*|
|*|  NOTE: this should only be used on shapes which are
|*|        known not to exist in, or have been removed
|*|        from, the glyph cache
\*/
CINTERNAL void
CGlyphShape_Destroy(CGlyphShape **_this)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((*_this != 0));

	/* update the reference count */
	--((*_this)->refCount);

	/* free the shape, as needed */
	if((*_this)->refCount == 0)
	{
		CFree(*_this);
	}

	/* null the shape */
	*_this = 0;
}


#ifdef __cplusplus
};
#endif
