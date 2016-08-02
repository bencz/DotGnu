/*
 * CGraphics.c - Graphics implementation.
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

#include "CGraphics.h"
#include "CBrush.h"
#include "CFiller.h"
#include "CFont.h"
#include "CImage.h"
#include "CMatrix.h"
#include "CPath.h"
#include "CPen.h"
#include "CRegion.h"
#include "CStroker.h"
#include "CSurface.h"
#include "CTrapezoids.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/*\
|*| NOTE: The defines/ifdefs here are a hack to get something akin to C# region
|*|       blocks; they serve a purely aesthetic purpose.
\*/

/******************************************************************************/
#define CGraphics_LIFETIME
#ifdef CGraphics_LIFETIME
/* Create a graphics context. */
CStatus
CGraphics_Create(CGraphics **_this,
                 CSurface   *surface)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a surface pointer */
	CStatus_Require((surface != 0), CStatus_ArgumentNull);

	/* allocate the graphics context */
	if(!(*_this = (CGraphics *)CMalloc(sizeof(CGraphics))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the graphics context */
	{
		/* declarations */
		CStatus status;

		/* get the graphics context */
		CGraphics *gc = *_this;

		/* initialize the transformation pipeline */
		CGraphicsPipeline_ResetWorld(&(gc->pipeline));
		CGraphicsPipeline_ResetPage(&(gc->pipeline));

		/* initialize the clipping region */
		if((status = CRegion_Create(&(gc->clip))) != CStatus_OK)
		{
			CFree(*_this);
			*_this = 0;
			return status;
		}

		/* initialize the rendering path */
		if((status = CPath_Create(&(gc->path))) != CStatus_OK)
		{
			CRegion_Destroy(&(gc->clip));
			CFree(*_this);
			*_this = 0;
			return status;
		}

		/* initialize the stroking path */
		if((status = CPath_Create(&(gc->stroke))) != CStatus_OK)
		{
			CPath_Destroy(&(gc->path));
			CRegion_Destroy(&(gc->clip));
			CFree(*_this);
			*_this = 0;
			return status;
		}

		/* reference the surface */
		CSurface_Reference(surface);

		/* initialize the remaining members */
		gc->surface                    = surface;
		gc->compositingMode            = CCompositingMode_SourceOver;
		gc->compositingQuality         = CCompositingQuality_Default;
		gc->pageUnit                   = CGraphicsUnit_Display;
		gc->pageScale                  = 1.0;
		gc->interpolationMode          = CInterpolationMode_Default;
		gc->pixelOffsetMode            = CPixelOffsetMode_Default;
		CPoint_X(gc->renderingOrigin)  = 0;
		CPoint_Y(gc->renderingOrigin)  = 0;
		gc->smoothingMode              = CSmoothingMode_Default;
		gc->textContrast               = 0;
		gc->textRenderingHint          = CTextRenderingHint_SystemDefault;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Destroy a graphics context. */
CStatus
CGraphics_Destroy(CGraphics **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a this pointer */
	CStatus_Require((*_this != 0), CStatus_ArgumentNull);

	/* finalize the graphics context */
	{
		/* get the graphics context */
		CGraphics *gc = *_this;

		/* finalize, as needed */
		if(gc->surface != 0)
		{
			/* destroy the surface */
			CSurface_Destroy(&(gc->surface));

			/* destroy the clip */
			CRegion_Destroy(&(gc->clip));

			/* destroy the rendering path */
			CPath_Destroy(&(gc->path));
		}
	}

	/* dispose of the graphics context */
	CFree(*_this);

	/* null the graphics context */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}
#endif
/******************************************************************************/











/******************************************************************************/
#define CGraphics_WORLD
#ifdef CGraphics_WORLD
/* Get the transformation matrix of this graphics context. */
CStatus
CGraphics_GetTransform(CGraphics *_this,
                       CMatrix   *matrix)
{
	/* declarations */
	CAffineTransformF t;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a matrix pointer */
	CStatus_Require((matrix != 0), CStatus_ArgumentNull);

	/* get the world transformation, synchronously */
	CSurface_Lock(_this->surface);
	{
		CGraphicsPipeline_GetWorld(&(_this->pipeline), &t);
	}
	CSurface_Unlock(_this->surface);

	/* set the matrix transformation */
	CMatrix_SetTransform(matrix, &t);

	/* return successfully */
	return CStatus_OK;
}

/* Multiply the transformation matrix by another matrix. */
CStatus
CGraphics_MultiplyTransform(CGraphics    *_this,
                            CMatrix      *matrix,
                            CMatrixOrder  order)
{
	/* declarations */
	CAffineTransformF t;
	CStatus           status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a matrix pointer */
	CStatus_Require((matrix != 0), CStatus_ArgumentNull);

	/* get the matrix transformation */
	CMatrix_GetTransform(matrix, &t);

	/* multiply the world transformation, synchronously */
	CSurface_Lock(_this->surface);
	{
		status =
			CGraphicsPipeline_MultiplyWorld
				(&(_this->pipeline), &t, order);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Reset the transformation matrix of this graphics context. */
CStatus
CGraphics_ResetTransform(CGraphics *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* reset the world transformation and inverse, synchronously */
	CSurface_Lock(_this->surface);
	{
		CGraphicsPipeline_ResetWorld(&(_this->pipeline));
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Rotate the transformation matrix of this graphics context. */
CStatus
CGraphics_RotateTransform(CGraphics    *_this,
                          CFloat        angle,
                          CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* rotate the world transformation and inverse, synchronously */
	CSurface_Lock(_this->surface);
	{
		CGraphicsPipeline_RotateWorld(&(_this->pipeline), angle, order);
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Scale the transformation matrix of this graphics context. */
CStatus
CGraphics_ScaleTransform(CGraphics    *_this,
                         CFloat        sx,
                         CFloat        sy,
                         CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* scale the world transformation and inverse, synchronously */
	CSurface_Lock(_this->surface);
	{
		CGraphicsPipeline_ScaleWorld(&(_this->pipeline), sx, sy, order);
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the transformation matrix of this graphics context. */
CStatus
CGraphics_SetTransform(CGraphics *_this,
                       CMatrix   *matrix)
{
	/* declarations */
	CAffineTransformF t;
	CStatus           status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a matrix pointer */
	CStatus_Require((matrix != 0), CStatus_ArgumentNull);

	/* get the matrix transformation */
	CMatrix_GetTransform(matrix, &t);

	/* set the world transformation, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CGraphicsPipeline_SetWorld(&(_this->pipeline), &t);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Translate the transformation matrix of this graphics context. */
CStatus
CGraphics_TranslateTransform(CGraphics    *_this,
                             CFloat        dx,
                             CFloat        dy,
                             CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* translate the world transformation and inverse, synchronously */
	CSurface_Lock(_this->surface);
	{
		CGraphicsPipeline_TranslateWorld(&(_this->pipeline), dx, dy, order);
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}
#endif
/******************************************************************************/












/******************************************************************************/
#define CGraphics_METAFILE
#ifdef CGraphics_METAFILE
#if 0
/* Add a metafile comment. */
CStatus
CGraphics_AddMetafileComment(CGraphics *_this,
                             CByte     *data,
                             CUInt32    count)
{
	return CStatus_NotImplemented;
}

/* Enumerate the contents of a metafile. */
CStatus
CGraphics_EnumerateMetafile(CGraphics             *_this,
                            CMetafile             *metafile,
                            CPointF                dst,
                            CImageAttributes      *atts,
                            CMetafileEnumCallback  callback,
                            void                  *callbackData)
{
	return CStatus_NotImplemented;
}
CStatus
CGraphics_EnumerateMetafile(CGraphics             *_this,
                            CMetafile             *metafile,
                            CPointF               *dst,
                            CUInt32                count,
                            CImageAttributes      *atts,
                            CMetafileEnumCallback  callback,
                            void                  *callbackData)
{
	return CStatus_NotImplemented;
}
CStatus
CGraphics_EnumerateMetafile(CGraphics             *_this,
                            CMetafile             *metafile,
                            CRectangleF            dst,
                            CImageAttributes      *atts,
                            CMetafileEnumCallback  callback,
                            void                  *callbackData)
{
	return CStatus_NotImplemented;
}
CStatus
CGraphics_EnumerateMetafile(CGraphics             *_this,
                            CMetafile             *metafile,
                            CPointF                dst,
                            CRectangleF            src,
                            CGraphicsUnit          srcUnit,
                            CImageAttributes      *atts,
                            CMetafileEnumCallback  callback,
                            void                  *callbackData)
{
	return CStatus_NotImplemented;
}
CStatus
CGraphics_EnumerateMetafile(CGraphics             *_this,
                            CMetafile             *metafile,
                            CPointF               *dst,
                            CUInt32                count,
                            CRectangleF            src,
                            CGraphicsUnit          srcUnit,
                            CImageAttributes      *atts,
                            CMetafileEnumCallback  callback,
                            void                  *callbackData)
{
	return CStatus_NotImplemented;
}
CStatus
CGraphics_EnumerateMetafile(CGraphics             *_this,
                            CMetafile             *metafile,
                            CRectangleF            dst,
                            CRectangleF            src,
                            CGraphicsUnit          srcUnit,
                            CImageAttributes      *atts,
                            CMetafileEnumCallback  callback,
                            void                  *callbackData)
{
	return CStatus_NotImplemented;
}
#endif
#endif
/******************************************************************************/












/******************************************************************************/
#define CGraphics_CLIP
#ifdef CGraphics_CLIP
/* Get the clipping region of this graphics context. */
CStatus
CGraphics_GetClip(CGraphics *_this,
                  CRegion   *region)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the region to the clipping region, synchronously */
	CSurface_Lock(_this->surface);
	{
		status =
			CRegion_CombineRegion
				(region, _this->clip, CCombineMode_Replace);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Get the clipping bounds of this graphics context. */
CStatus
CGraphics_GetClipBounds(CGraphics   *_this,
                        CRectangleF *clipBounds)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the clipping bounds, synchronously */
	CSurface_Lock(_this->surface);
	{
		status =
			CRegion_GetBounds
				(_this->clip, _this, clipBounds);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Get the visible clipping bounds of this graphics context. */
CStatus
CGraphics_GetVisibleClipBounds(CGraphics   *_this,
                               CRectangleF *bounds)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a bounds pointer */
	CStatus_Require((bounds != 0), CStatus_ArgumentNull);

	/* get the visible clipping bounds, synchronously */
	CSurface_Lock(_this->surface);
	{
		/* declarations */
		CRectangleF visible;

		/* get the surface bounds */
		visible = CSurface_GetBoundsF(_this->surface);

		/* get the clipping bounds */
		status = CRegion_GetBounds(_this->clip, _this, bounds);

		/* obtain the intersection of the visible area and the clip bounds */
		/* TODO */
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Determine if the clipping region is empty. */
CStatus
CGraphics_IsClipEmpty(CGraphics *_this,
                      CBool     *empty)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an empty flag pointer */
	CStatus_Require((empty != 0), CStatus_ArgumentNull);

	/* determine if the clipping region is empty, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CRegion_IsEmpty(_this->clip, _this, empty);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Determine if the visible clipping region is empty. */
CStatus
CGraphics_IsVisibleClipEmpty(CGraphics *_this,
                             CBool     *empty)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an empty flag pointer */
	CStatus_Require((empty != 0), CStatus_ArgumentNull);

	/* determine if the visible clipping region is empty, synchronously */
	CSurface_Lock(_this->surface);
	{
		/* declarations */
		CRectangleF visible;

		/* get the surface bounds */
		visible = CSurface_GetBoundsF(_this->surface);

		/* determine if the visible area is within the clipping region */
		status =
			CRegion_IsVisibleRectangle
				(_this->clip, _this, visible, empty);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Determine if a point is within the visible clip region. */
CStatus
CGraphics_IsVisiblePoint(CGraphics *_this,
                         CPointF    point,
                         CBool     *visible)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a visible flag pointer */
	CStatus_Require((visible != 0), CStatus_ArgumentNull);

	/* determine if the visible clipping region is empty, synchronously */
	CSurface_Lock(_this->surface);
	{
		/* declarations */
		CRectangleF v;

		/* get the surface bounds */
		v = CSurface_GetBoundsF(_this->surface);

		/* determine if the point is within the visible bounds */
		if(!(CRectangle_ContainsPoint(v, point)))
		{
			/* set the visible flag to false */
			*visible = 0;

			/* set the status */
			status = CStatus_OK;
		}
		else
		{
			/* determine if the point is within the clipping region */
			status =
				CRegion_IsVisiblePoint
					(_this->clip, _this, point, visible);
		}
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Determine if any part of a rectangle is within the visible clip region. */
CStatus
CGraphics_IsVisibleRectangle(CGraphics   *_this,
                             CRectangleF  rect,
                             CBool       *visible)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Reset the clipping region. */
CStatus
CGraphics_ResetClip(CGraphics *_this)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* reset the clipping region, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CRegion_MakeInfinite(_this->clip);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Set the clipping region to that of another graphics context. */
CStatus
CGraphics_SetClipGraphics(CGraphics    *_this,
                          CGraphics    *graphics,
                          CCombineMode  combineMode)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a graphics context pointer */
	CStatus_Require((graphics != 0), CStatus_ArgumentNull);

	/* set the clipping region, synchronously */
	if(_this->surface == graphics->surface)
	{
		/* set the clipping region */
		if(_this == graphics)
		{
			/* TODO: how should we handle this case? */
			status = CStatus_Argument;
		}
		else
		{
			CSurface_Lock(_this->surface);
			{
				status =
					CRegion_CombineRegion
						(_this->clip, graphics->clip, combineMode);
			}
			CSurface_Unlock(_this->surface);
		}
	}
	else
	{
		CSurface_Lock(_this->surface);
		CSurface_Lock(graphics->surface);
		{
			status =
				CRegion_CombineRegion
					(_this->clip, graphics->clip, combineMode);
		}
		CSurface_Lock(graphics->surface);
		CSurface_Unlock(_this->surface);
	}

	/* return status */
	return status;
}

/* Set the clipping region to a given path. */
CStatus
CGraphics_SetClipPath(CGraphics    *_this,
                      CPath        *path,
                      CCombineMode  combineMode)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a path pointer */
	CStatus_Require((path != 0), CStatus_ArgumentNull);

	/* set the clipping region, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CRegion_CombinePath(_this->clip, path, combineMode);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Set the clipping region to a given region. */
CStatus
CGraphics_SetClipRegion(CGraphics    *_this,
                        CRegion      *region,
                        CCombineMode  combineMode)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a region pointer */
	CStatus_Require((region != 0), CStatus_ArgumentNull);

	/* set the clipping region, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CRegion_CombineRegion(_this->clip, region, combineMode);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Set the clipping region to a given rectangle. */
CStatus
CGraphics_SetClipRectangle(CGraphics    *_this,
                           CRectangleF   rect,
                           CCombineMode  combineMode)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the clipping region, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CRegion_CombineRectangle(_this->clip, rect, combineMode);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Translate the clipping region by a specific amount. */
CStatus
CGraphics_TranslateClip(CGraphics *_this,
                        CFloat     dx,
                        CFloat     dy)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* translate the clipping region, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CRegion_Translate(_this->clip, dx, dy);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Get the clipping mask. */
static CStatus
CGraphics_GetClipMask(CGraphics       *_this,
                      pixman_image_t **mask)
{
	/* declarations */
	CBool gray;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((mask  != 0));

	/* determine if we should use gray values */
	gray = CUtils_UseGray(_this->smoothingMode, _this->pixelOffsetMode);

	/* get the surface mask */
	CStatus_Check
		(CSurface_GetClipMask
			(_this->surface, mask, gray));

	/* get the clipping mask */
	CStatus_Check
		(CRegion_GetMask
			(_this->clip, &CGraphicsPipeline_Device(_this->pipeline), *mask));

	/* return successfully */
	return CStatus_OK;
}

/* Get the compositing mask. */
static CStatus
CGraphics_GetCompositingMask(CGraphics       *_this,
                             pixman_image_t **mask)
{
	/* declarations */
	CBool gray;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((mask  != 0));

	/* determine if we should use gray values */
	gray = CUtils_UseGray(_this->smoothingMode, _this->pixelOffsetMode);

	/* get the surface mask */
	CStatus_Check
		(CSurface_GetCompositingMask
			(_this->surface, mask, gray));

	/* reset the compositing mask */
	{
		/* declarations */
		CByte   *data;
		CUInt32  height;
		CUInt32  stride;

		/* get the mask information */
		data   = (CByte *)pixman_image_get_data(*mask);
		height = (CUInt32)pixman_image_get_height(*mask);
		stride = (CUInt32)pixman_image_get_stride(*mask);

		/* reset the mask */
		CMemSet(data, 0x00, (height * stride));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the text compositing mask. */
static CStatus
CGraphics_GetTextCompositingMask(CGraphics       *_this,
                                 pixman_image_t **mask)
{
	/* declarations */
	CBool gray;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((mask  != 0));

	/* determine if we should use gray values */
	switch(_this->textRenderingHint)
	{
		case CTextRenderingHint_SingleBitPerPixelGridFit:
		case CTextRenderingHint_SingleBitPerPixel:
		{
			gray = 0;
		}
		break;
		case CTextRenderingHint_SystemDefault:
		case CTextRenderingHint_AntiAlias:
		case CTextRenderingHint_AntiAliasGridFit:
		case CTextRenderingHint_ClearTypeGridFit:
		default:
		{
			gray = 1;
		}
		break;
	}

	/* get the surface mask */
	CStatus_Check
		(CSurface_GetCompositingMask
			(_this->surface, mask, gray));

	/* reset the compositing mask */
	{
		/* declarations */
		CByte   *data;
		CUInt32  height;
		CUInt32  stride;

		/* get the mask information */
		data   = (CByte *)pixman_image_get_data(*mask);
		height = (CUInt32)pixman_image_get_height(*mask);
		stride = (CUInt32)pixman_image_get_stride(*mask);

		/* reset the mask */
		CMemSet(data, 0x00, (height * stride));
	}

	/* return successfully */
	return CStatus_OK;
}
#endif
/******************************************************************************/












/******************************************************************************/
#define CGraphics_PROPERTIES
#ifdef CGraphics_PROPERTIES
/* Get the compositing mode of this graphics context. */
CStatus
CGraphics_GetCompositingMode(CGraphics        *_this,
                             CCompositingMode *compositingMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a compositing mode pointer */
	CStatus_Require((compositingMode != 0), CStatus_ArgumentNull);

	/* get the compositing mode, synchronously */
	CSurface_Lock(_this->surface);
	{
		*compositingMode = _this->compositingMode;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the compositing mode of this graphics context. */
CStatus
CGraphics_SetCompositingMode(CGraphics        *_this,
                             CCompositingMode  compositingMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the compositing mode, synchronously */
	CSurface_Lock(_this->surface);
	{
		_this->compositingMode = compositingMode;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Get the compositing quality of this graphics context. */
CStatus
CGraphics_GetCompositingQuality(CGraphics           *_this,
                                CCompositingQuality *compositingQuality)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a compositing quality pointer */
	CStatus_Require((compositingQuality != 0), CStatus_ArgumentNull);

	/* get the compositing quality, synchronously */
	CSurface_Lock(_this->surface);
	{
		*compositingQuality = _this->compositingQuality;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the compositing quality of this graphics context. */
CStatus
CGraphics_SetCompositingQuality(CGraphics           *_this,
                                CCompositingQuality  compositingQuality)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the compositing quality, synchronously */
	CSurface_Lock(_this->surface);
	{
		_this->compositingQuality = compositingQuality;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Get the horizontal resolution of this graphics context. */
CStatus
CGraphics_GetDpiX(CGraphics *_this,
                  CFloat    *dpiX)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a horizontal resolution pointer */
	CStatus_Require((dpiX != 0), CStatus_ArgumentNull);

	/* get the horizontal resolution, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CSurface_GetDpiX(_this->surface, dpiX);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Get the vertical resolution of this graphics context. */
CStatus
CGraphics_GetDpiY(CGraphics *_this,
                  CFloat    *dpiY)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a vertical resolution pointer */
	CStatus_Require((dpiY != 0), CStatus_ArgumentNull);

	/* get the vertical resolution, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CSurface_GetDpiY(_this->surface, dpiY);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Get the interpolation mode of this graphics context. */
CStatus
CGraphics_GetInterpolationMode(CGraphics          *_this,
                               CInterpolationMode *interpolationMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an interpolation mode pointer */
	CStatus_Require((interpolationMode != 0), CStatus_ArgumentNull);

	/* get the interpolation mode, synchronously */
	CSurface_Lock(_this->surface);
	{
		*interpolationMode = _this->interpolationMode;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the interpolation mode of this graphics context. */
CStatus
CGraphics_SetInterpolationMode(CGraphics          *_this,
                               CInterpolationMode  interpolationMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the interpolation mode, synchronously */
	CSurface_Lock(_this->surface);
	{
		_this->interpolationMode = interpolationMode;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Get the page scaling factor of this graphics context. */
CStatus
CGraphics_GetPageScale(CGraphics *_this,
                       CFloat    *pageScale)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a page scaling factor pointer */
	CStatus_Require((pageScale != 0), CStatus_ArgumentNull);

	/* get the page scaling factor, synchronously */
	CSurface_Lock(_this->surface);
	{
		*pageScale = _this->pageScale;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the page scaling factor of this graphics context. */
CStatus
CGraphics_SetPageScale(CGraphics *_this,
                       CFloat     pageScale)
{
	/* declarations */
	CStatus status;
	CFloat  dpiX;
	CFloat  dpiY;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the page scaling factor, synchronously */
	CSurface_Lock(_this->surface);
	{
		/* set the page scaling factor */
		_this->pageScale = pageScale;

		/* get the horizontal resolution */
		CStatus_CheckGOTO
			(CSurface_GetDpiX(_this->surface, &dpiX), status, GOTO_Cleanup);

		/* get the vertical resolution */
		CStatus_CheckGOTO
			(CSurface_GetDpiY(_this->surface, &dpiY), status, GOTO_Cleanup);

		/* update the pipeline */
		CGraphicsPipeline_SetPage
			(&(_this->pipeline), _this->pageUnit, pageScale, dpiX, dpiY);
	}
GOTO_Cleanup:
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Get the page unit of this graphics context. */
CStatus
CGraphics_GetPageUnit(CGraphics     *_this,
                      CGraphicsUnit *pageUnit)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a page unit pointer */
	CStatus_Require((pageUnit != 0), CStatus_ArgumentNull);

	/* get the page unit, synchronously */
	CSurface_Lock(_this->surface);
	{
		*pageUnit = _this->pageUnit;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the page unit of this graphics context. */
CStatus
CGraphics_SetPageUnit(CGraphics     *_this,
                      CGraphicsUnit  pageUnit)
{
	/* declarations */
	CStatus status;
	CFloat  dpiX;
	CFloat  dpiY;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the page unit, synchronously */
	CSurface_Lock(_this->surface);
	{
		/* set the page unit */
		_this->pageUnit = pageUnit;

		/* get the horizontal resolution */
		CStatus_CheckGOTO
			(CSurface_GetDpiX(_this->surface, &dpiX), status, GOTO_Cleanup);

		/* get the vertical resolution */
		CStatus_CheckGOTO
			(CSurface_GetDpiY(_this->surface, &dpiY), status, GOTO_Cleanup);

		/* update the pipeline */
		CGraphicsPipeline_SetPage
			(&(_this->pipeline), pageUnit, _this->pageScale, dpiX, dpiY);
	}
GOTO_Cleanup:
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Get the pixel offset mode of this graphics context. */
CStatus
CGraphics_GetPixelOffsetMode(CGraphics        *_this,
                             CPixelOffsetMode *pixelOffsetMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pixel offset mode pointer */
	CStatus_Require((pixelOffsetMode != 0), CStatus_ArgumentNull);

	/* get the pixel offset mode, synchronously */
	CSurface_Lock(_this->surface);
	{
		*pixelOffsetMode = _this->pixelOffsetMode;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the pixel offset mode of this graphics context. */
CStatus
CGraphics_SetPixelOffsetMode(CGraphics        *_this,
                             CPixelOffsetMode  pixelOffsetMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the pixel offset mode, synchronously */
	CSurface_Lock(_this->surface);
	{
		_this->pixelOffsetMode = pixelOffsetMode;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Get the rendering origin of this graphics context. */
CStatus
CGraphics_GetRenderingOrigin(CGraphics *_this,
                             CPointI   *renderingOrigin)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a rendering origin pointer */
	CStatus_Require((renderingOrigin != 0), CStatus_ArgumentNull);

	/* get the rendering origin, synchronously */
	CSurface_Lock(_this->surface);
	{
		*renderingOrigin = _this->renderingOrigin;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the rendering origin of this graphics context. */
CStatus
CGraphics_SetRenderingOrigin(CGraphics *_this,
                             CPointI    renderingOrigin)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the rendering origin, synchronously */
	CSurface_Lock(_this->surface);
	{
		_this->renderingOrigin = renderingOrigin;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Get the smoothing mode of this graphics context. */
CStatus
CGraphics_GetSmoothingMode(CGraphics      *_this,
                           CSmoothingMode *smoothingMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a smoothing mode pointer */
	CStatus_Require((smoothingMode != 0), CStatus_ArgumentNull);

	/* get the smoothing mode, synchronously */
	CSurface_Lock(_this->surface);
	{
		*smoothingMode = _this->smoothingMode;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the smoothing mode of this graphics context. */
CStatus
CGraphics_SetSmoothingMode(CGraphics      *_this,
                           CSmoothingMode  smoothingMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the smoothing mode, synchronously */
	CSurface_Lock(_this->surface);
	{
		_this->smoothingMode = smoothingMode;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Get the text contrast of this graphics context. */
CStatus
CGraphics_GetTextContrast(CGraphics *_this,
                          CUInt32   *textContrast)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a text contrast pointer */
	CStatus_Require((textContrast != 0), CStatus_ArgumentNull);

	/* get the text contrast, synchronously */
	CSurface_Lock(_this->surface);
	{
		*textContrast = _this->textContrast;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the text contrast of this graphics context. */
CStatus
CGraphics_SetTextContrast(CGraphics *_this,
                          CUInt32    textContrast)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the text contrast, synchronously */
	CSurface_Lock(_this->surface);
	{
		_this->textContrast = textContrast;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Get the text rendering hint of this graphics context. */
CStatus
CGraphics_GetTextRenderingHint(CGraphics          *_this,
                               CTextRenderingHint *textRenderingHint)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a text rendering hint pointer */
	CStatus_Require((textRenderingHint != 0), CStatus_ArgumentNull);

	/* get the text rendering hint, synchronously */
	CSurface_Lock(_this->surface);
	{
		*textRenderingHint = _this->textRenderingHint;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}

/* Set the text rendering hint of this graphics context. */
CStatus
CGraphics_SetTextRenderingHint(CGraphics          *_this,
                               CTextRenderingHint  textRenderingHint)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the text rendering hint, synchronously */
	CSurface_Lock(_this->surface);
	{
		_this->textRenderingHint = textRenderingHint;
	}
	CSurface_Unlock(_this->surface);

	/* return successfully */
	return CStatus_OK;
}
#endif
/******************************************************************************/












/******************************************************************************/
#define CGraphics_IMAGING
#ifdef CGraphics_IMAGING

static const pixman_transform_t CPixmanTransform_Identity =
{{
	{ CFixed_One,  CFixed_Zero, CFixed_Zero },
	{ CFixed_Zero, CFixed_One,  CFixed_Zero },
	{ CFixed_Zero, CFixed_Zero, CFixed_One  }
}};

/* Draw an xbm glyph. */
CStatus
CGraphics_DrawXBM(CGraphics   *_this,
                  const CByte *bits,
                  CFloat       x,
                  CFloat       y,
                  CUInt16      width,
                  CUInt16      height,
                  CColor       color,
                  CBool        transform)
{
	/* declarations */
	pixman_image_t *pattern;
	CStatus         status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a bit list pointer */
	CStatus_Require((bits != 0), CStatus_ArgumentNull);

	/* ensure we have a valid xbm size */
	CStatus_Require((width != 0 && height != 0), CStatus_Argument);

	/* bail out now if there's nothing to do */
	CStatus_Require((color != 0), CStatus_OK);

	/* create the pattern */
	CStatus_Check
		(CUtils_CreateHatchPattern
			(&pattern, bits, width, height, color, 0, 0));

	/* paint to the surface synchronously */
	CSurface_Lock(_this->surface);
	{
		/* declarations */
		pixman_image_t *clip;

		/* get the clip mask */
		status = CGraphics_GetClipMask(_this, &clip);

		/* handle masking failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			pixman_image_destroy(pattern);
			return status;
		}

		/* set the pattern transformation, as needed */
		if(transform)
		{
			/* declarations */
			pixman_transform_t xform;

			/* get the transformation */
			xform =
				CUtils_ToPixmanTransform
					(&(CGraphicsPipeline_DeviceInverse(_this->pipeline)));

			/* set the image transformation */
			if(pixman_image_set_transform(pattern, &xform))
			{
				CSurface_Unlock(_this->surface);
				pixman_image_destroy(pattern);
				return CStatus_OutOfMemory;
			}
		}

		/* draw the xbm */
		status =
			CSurface_Composite
				(_this->surface, x, y, width, height, pattern, clip,
				 _this->interpolationMode, _this->compositingMode);
	}
	CSurface_Unlock(_this->surface);

	/* destroy the pattern */
	pixman_image_destroy(pattern);

	/* return status */
	return status;
}

/* Draw an image. */
CStatus
CGraphics_DrawImage(CGraphics *_this,
                    CImage    *image,
                    CFloat     x,
                    CFloat     y)
{
	/* declarations */
	CSizeF size;

	/* get the size of the image */
	CStatus_Check
		(CImage_GetPhysicalDimension
			(image, &size));

	/* draw the image */
	return
		CGraphics_DrawImageRect
			(_this, image, x, y, CSize_Width(size), CSize_Height(size));
}

/* Draw an image. */
CStatus
CGraphics_DrawImageRect(CGraphics *_this,
                        CImage    *image,
                        CFloat     x,
                        CFloat     y,
                        CFloat     width,
                        CFloat     height)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an image pointer */
	CStatus_Require((image != 0), CStatus_ArgumentNull);

	/* paint to the surface synchronously */
	CSurface_Lock(_this->surface);
	{
		/* declarations */
		pixman_image_t *mask;

		/* get the clip mask */
		status =
			CGraphics_GetClipMask
				(_this, &mask);

		/* handle masking failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* draw the image synchronously */
		CMutex_Lock(image->lock);
		{
			/* declarations */
			pixman_transform_t transform;

			/* get the transformation */
			transform =
				CUtils_ToPixmanTransform
					(&(CGraphicsPipeline_DeviceInverse(_this->pipeline)));

			/* set the image transformation */
			if(pixman_image_set_transform(image->image, &transform))
			{
				CMutex_Unlock(image->lock);
				CSurface_Unlock(_this->surface);
				return CStatus_OutOfMemory;
			}

			/* composite the image */
			status =
				CSurface_Composite
					(_this->surface, x, y, width, height, image->image, mask,
					 _this->interpolationMode, _this->compositingMode);

			/* reset the image transformation */
			pixman_image_set_transform
				(image->image,
				 ((pixman_transform_t *)&CPixmanTransform_Identity));
		}
		CMutex_Unlock(image->lock);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}


/* Draw an image. */
CStatus
CGraphics_DrawImagePoints(CGraphics *_this,
                          CImage    *image,
                          CPointF   *dst,
                          CUInt32    count)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Draw an image. */
CStatus
CGraphics_DrawImageRectPoints(CGraphics     *_this,
                              CImage        *image,
                              CPointF        dst,
                              CRectangleF    src,
                              CGraphicsUnit  srcUnit)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Draw an image. */
CStatus
CGraphics_DrawImageRectPointsAttributes(CGraphics           *_this,
                                        CImage              *image,
                                        CPointF             *dst,
                                        CUInt32              count,
                                        CRectangleF          src,
                                        CGraphicsUnit        srcUnit,
                                        CImageAttributes    *atts)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Draw an image. */
CStatus
CGraphics_DrawImageRectRectAttributes(CGraphics           *_this,
                                      CImage              *image,
                                      CRectangleF          dst,
                                      CRectangleF          src,
                                      CGraphicsUnit        srcUnit,
                                      CImageAttributes    *atts)
{
	/* TODO */
	return CStatus_NotImplemented;
}
#endif
/******************************************************************************/












/******************************************************************************/
#define CGraphics_DRAWING
#ifdef CGraphics_DRAWING

/* Composite the image. */
static CStatus
CGraphics_Composite(CGraphics      *_this,
                    CInt32          x,
					CInt32          y,
                    CUInt32         width,
                    CUInt32         height,
                    CPattern       *pattern,
                    pixman_image_t *mask)
{
	/* declarations */
	CStatus  status;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));
	CASSERT((mask    != 0));

	/* apply transformation, as needed */
	if(pattern->transform != 0)
	{
		/* declarations */
		CAffineTransformF  affine;
		pixman_transform_t transform;

		/* get the pattern transformation */
		affine = *(pattern->transform);

		/* apply the device transformation */
		CAffineTransformF_Multiply
			(&affine, &CGraphicsPipeline_DeviceInverse(_this->pipeline),
			 CMatrixOrder_Append);

		/* get the pixman transformation */
		transform = CUtils_ToPixmanTransform(&affine);

		/* set the image transformation */
		pixman_image_set_transform(pattern->image, &transform);
	}

	/* composite the image */
	status =
		CSurface_Composite
			(_this->surface, x, y, width, height, pattern->image, mask,
			 _this->interpolationMode, _this->compositingMode);

	/* reset transformation, as needed */
	if(pattern->transform != 0)
	{
		pixman_image_set_transform
			(pattern->image,
			 ((pixman_transform_t *)&CPixmanTransform_Identity));
	}

	/* return status */
	return status;
}

/* Fill the given path. */
static CStatus
CGraphics_Fill2(CGraphics *_this,
                CPath     *path,
                CPattern  *pattern)
{
	/* declarations */
	CStatus status;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((path    != 0));
	CASSERT((pattern != 0));

	/* fill the given path */
	{
		/* declarations */
		CUInt32         width;
		CUInt32         height;
		CTrapezoids     trapezoids;
		pixman_image_t *clip;
		pixman_image_t *mask;

		/* get the clipping mask */
		CStatus_Check
			(CGraphics_GetClipMask
				(_this, &clip));

		/* get the compositing mask */
		CStatus_Check
			(CGraphics_GetCompositingMask
				(_this, &mask));

		/* initialize the trapezoids */
		CTrapezoids_Initialize(&trapezoids);

		/*\
		|*| TODO: special case identity transformation
		\*/

		/* transform the path */
		CPath_TransformAffine
			(path, &CGraphicsPipeline_Device(_this->pipeline));

		/* fill the path */
		status =
			CPath_Fill
				(path, &trapezoids);

		/* handle fill failures */
		if(status != CStatus_OK)
		{
			CTrapezoids_Finalize(&trapezoids);
			return status;
		}

		/* composite the trapezoids */
		pixman_composite_trapezoids
			(PIXMAN_OPERATOR_ADD, clip, mask, 0, 0,
			 ((pixman_trapezoid_t *)CTrapezoids_Trapezoids(trapezoids)),
			 CTrapezoids_Count(trapezoids));

		/* finalize the trapezoids */
		CTrapezoids_Finalize(&trapezoids);

		/*\
		|*| TODO: calculate and use trapezoid bounds
		\*/

		/* get the mask dimensions */
		width = pixman_image_get_width(mask);
		height = pixman_image_get_height(mask);

		/* composite the image */
		status =
			CGraphics_Composite
				(_this, 0, 0, width, height, pattern, mask);
	}

	/* return status */
	return status;
}

/* Stroke the current path. */
static CStatus
CGraphics_Stroke(CGraphics *_this,
                 CPen      *pen)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((pen   != 0));

	/* perform the stroke */
	{
		/* declarations */
		CStatus  status;
		CStroker stroker;
		CPattern pattern;

		/* get the source pattern */
		CStatus_Check
			(CPen_GetPattern
				(pen, &pattern));

		/* reset the stroke path */
		CStatus_Check
			(CPath_Reset
				(_this->stroke));

		/* initialize the stroker */
		CStatus_Check
			(CStroker_Initialize
				(&stroker, pen, &CGraphicsPipeline_Device(_this->pipeline)));

		/* stroke the path */
		status =
			CPath_Stroke
				(_this->path, _this->stroke, &stroker);

		/* finalize the stroker */
		CStroker_Finalize(&stroker);

		/* handle stroke failures */
		CStatus_Check(status);

		/* fill the path */
		CStatus_Check
			(CGraphics_Fill2
				(_this, _this->stroke, &pattern));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Draw an arc. */
CStatus
CGraphics_DrawArc(CGraphics   *_this,
                  CPen        *pen,
                  CRectangleF  rect,
                  CFloat       startAngle,
                  CFloat       sweepAngle)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the arc to the path */
		status =
			CPath_AddArc
				(_this->path,
				 CRectangle_X(rect),
				 CRectangle_Y(rect),
				 CRectangle_Width(rect),
				 CRectangle_Height(rect),
				 startAngle, sweepAngle);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a Bezier spline. */
CStatus
CGraphics_DrawBezier(CGraphics *_this,
                     CPen      *pen,
                     CPointF    a,
                     CPointF    b,
                     CPointF    c,
                     CPointF    d)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the bezier to the path */
		status =
			CPath_AddBezier
				(_this->path,
				 CPoint_X(a), CPoint_Y(a),
				 CPoint_X(b), CPoint_Y(b),
				 CPoint_X(c), CPoint_Y(c),
				 CPoint_X(d), CPoint_Y(d));

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a series of Bezier splines. */
CStatus
CGraphics_DrawBeziers(CGraphics *_this,
                      CPen      *pen,
                      CPointF   *points,
                      CUInt32    count)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* ensure we have a point list */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the beziers to the path */
		status =
			CPath_AddBeziers
				(_this->path, points, count);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a closed cardinal spline. */
CStatus
CGraphics_DrawClosedCurve(CGraphics *_this,
                          CPen      *pen,
                          CPointF   *points,
                          CUInt32    count,
                          CFloat     tension)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* ensure we have a point list */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the cardinal curve to the path */
		status =
			CPath_AddClosedCardinalCurve
				(_this->path, points, count, tension);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a cardinal spline. */
CStatus
CGraphics_DrawCurve(CGraphics *_this,
                    CPen      *pen,
                    CPointF   *points,
                    CUInt32    count,
                    CUInt32    offset,
                    CUInt32    numberOfSegments,
                    CFloat     tension)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* ensure we have a point list */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the cardinal curve to the path */
		status =
			CPath_AddCardinalCurve
				(_this->path, points, count, offset, numberOfSegments, tension);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw an ellipse. */
CStatus
CGraphics_DrawEllipse(CGraphics   *_this,
                      CPen        *pen,
                      CRectangleF  rect)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the ellipse to the path */
		status =
			CPath_AddEllipse
				(_this->path,
				 CRectangle_X(rect),
				 CRectangle_Y(rect),
				 CRectangle_Width(rect),
				 CRectangle_Height(rect));

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a line between two points. */
CStatus
CGraphics_DrawLine(CGraphics *_this,
                   CPen      *pen,
                   CPointF    start,
                   CPointF    end)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the line to the path */
		status =
			CPath_AddLine
				(_this->path,
				 CPoint_X(start), CPoint_Y(start),
				 CPoint_X(end),   CPoint_Y(end));

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a series of connected line segments. */
CStatus
CGraphics_DrawLines(CGraphics *_this,
                    CPen      *pen,
                    CPointF   *points,
                    CUInt32    count)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* ensure we have a point list */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the lines to the path */
		status =
			CPath_AddLines
				(_this->path, points, count);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a path object. */
CStatus
CGraphics_DrawPath(CGraphics *_this,
                   CPen      *pen,
                   CPath     *path)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* ensure we have a path pointer */
	CStatus_Require((path != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the path to the path */
		status =
			CPath_AddPath
				(_this->path, path, 0);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a pie shape. */
CStatus
CGraphics_DrawPie(CGraphics   *_this,
                  CPen        *pen,
                  CRectangleF  rect,
                  CFloat       startAngle,
                  CFloat       sweepAngle)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the pie to the path */
		status =
			CPath_AddPie
				(_this->path,
				 CRectangle_X(rect),
				 CRectangle_Y(rect),
				 CRectangle_Width(rect),
				 CRectangle_Height(rect),
				 startAngle, sweepAngle);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a polygon. */
CStatus
CGraphics_DrawPolygon(CGraphics *_this,
                      CPen      *pen,
                      CPointF   *points,
                      CUInt32    count)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* ensure we have a point list */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the polygon to the path */
		status =
			CPath_AddPolygon
				(_this->path, points, count);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a rectangle. */
CStatus
CGraphics_DrawRectangle(CGraphics   *_this,
                        CPen        *pen,
                        CRectangleF  rect)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the rectangle to the path */
		status =
			CPath_AddRectangle
				(_this->path, 
				 CRectangle_X(rect),
				 CRectangle_Y(rect),
				 CRectangle_Width(rect),
				 CRectangle_Height(rect));

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Draw a series of rectangles. */
CStatus
CGraphics_DrawRectangles(CGraphics   *_this,
                         CPen        *pen,
                         CRectangleF *rects,
                         CUInt32      count)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pen pointer */
	CStatus_Require((pen != 0), CStatus_ArgumentNull);

	/* ensure we have a rectangle list */
	CStatus_Require((rects != 0), CStatus_ArgumentNull);

	/* perform the stroke synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the rectangles to the path */
		status =
			CPath_AddRectangles
				(_this->path, rects, count);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* stroke the path */
		status = CGraphics_Stroke(_this, pen);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}
#endif
/******************************************************************************/












/******************************************************************************/
#define CGraphics_FILLING
#ifdef CGraphics_FILLING
/* Fill the current path. */
static CStatus
CGraphics_Fill(CGraphics *_this,
               CBrush    *brush,
               CFillMode  fillMode)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((brush != 0));

	/* perform the stroke */
	{
		/* declarations */
		CPattern pattern;

		/* get the source pattern */
		CStatus_Check
			(CBrush_GetPattern
				(brush, &pattern));

		/* set the fill mode */
		CStatus_Check
			(CPath_SetFillMode
				(_this->path, fillMode));

		/* fill the path */
		CStatus_Check
			(CGraphics_Fill2
				(_this, _this->path, &pattern));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Clear the entire drawing surface. */
CStatus
CGraphics_Clear(CGraphics *_this,
                CColor     color)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* perform the clear synchronously */
	CSurface_Lock(_this->surface);
	{
		/* clear the surface */
		status =
			CSurface_Clear
				(_this->surface, color);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Fill a closed cardinal spline. */
CStatus
CGraphics_FillClosedCurve(CGraphics *_this,
                          CBrush    *brush,
                          CPointF   *points,
                          CUInt32    count,
                          CFloat     tension,
                          CFillMode  fillMode)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* ensure we have a point list */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* perform the fill synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the cardinal curve to the path */
		status =
			CPath_AddClosedCardinalCurve
				(_this->path, points, count, tension);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* fill the path */
		status =
			CGraphics_Fill
				(_this, brush, fillMode);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Fill an ellipse. */
CStatus
CGraphics_FillEllipse(CGraphics   *_this,
                      CBrush      *brush,
                      CRectangleF  rect)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* perform the fill synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the ellipse to the path */
		status =
			CPath_AddEllipse
				(_this->path,
				 CRectangle_X(rect),
				 CRectangle_Y(rect),
				 CRectangle_Width(rect),
				 CRectangle_Height(rect));

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* fill the path */
		status =
			CGraphics_Fill
				(_this, brush, CFillMode_Alternate);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Fill the interior of a path. */
CStatus
CGraphics_FillPath(CGraphics *_this,
                   CBrush    *brush,
                   CPath     *path)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* ensure we have a path pointer */
	CStatus_Require((path != 0), CStatus_ArgumentNull);

	/* perform the fill synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the path to the path */
		status =
			CPath_AddPath
				(_this->path, path, 0);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* fill the path */
		status =
			CGraphics_Fill
				(_this, brush, CFillMode_Alternate);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Fill a pie shape. */
CStatus
CGraphics_FillPie(CGraphics   *_this,
                  CBrush      *brush,
                  CRectangleF  rect,
                  CFloat       startAngle,
                  CFloat       sweepAngle)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* perform the fill synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the pie to the path */
		status =
			CPath_AddPie
				(_this->path,
				 CRectangle_X(rect),
				 CRectangle_Y(rect),
				 CRectangle_Width(rect),
				 CRectangle_Height(rect),
				 startAngle, sweepAngle);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* fill the path */
		status =
			CGraphics_Fill
				(_this, brush, CFillMode_Alternate);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Fill a polygon. */
CStatus
CGraphics_FillPolygon(CGraphics *_this,
                      CBrush    *brush,
                      CPointF   *points,
                      CUInt32    count,
                      CFillMode  fillMode)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* ensure we have a point list */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* perform the fill synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the polygon to the path */
		status =
			CPath_AddPolygon
				(_this->path, points, count);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* fill the path */
		status =
			CGraphics_Fill
				(_this, brush, fillMode);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Fill a rectangle. */
CStatus
CGraphics_FillRectangle(CGraphics   *_this,
                        CBrush      *brush,
                        CRectangleF  rect)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* perform the fill synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the rectangle to the path */
		status =
			CPath_AddRectangle
				(_this->path, 
				 CRectangle_X(rect),
				 CRectangle_Y(rect),
				 CRectangle_Width(rect),
				 CRectangle_Height(rect));

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* fill the path */
		status =
			CGraphics_Fill
				(_this, brush, CFillMode_Alternate);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Fill a series of rectangles. */
CStatus
CGraphics_FillRectangles(CGraphics   *_this,
                         CBrush      *brush,
                         CRectangleF *rects,
                         CUInt32      count)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* ensure we have a rectangle list */
	CStatus_Require((rects != 0), CStatus_ArgumentNull);

	/* perform the fill synchronously */
	CSurface_Lock(_this->surface);
	{
		/* reset the path */
		status = CPath_Reset(_this->path);

		/* handle reset failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* add the rectangles to the path */
		status =
			CPath_AddRectangles
				(_this->path, rects, count);

		/* handle pathing failures */
		if(status != CStatus_OK)
		{
			CSurface_Unlock(_this->surface);
			return status;
		}

		/* fill the path */
		status =
			CGraphics_Fill
				(_this, brush, CFillMode_Alternate);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Fill a region. */
CStatus
CGraphics_FillRegion(CGraphics *_this,
                     CBrush    *brush,
                     CRegion   *region)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* ensure we have a region pointer */
	CStatus_Require((region != 0), CStatus_ArgumentNull);

	/* perform the fill synchronously */
	CSurface_Lock(_this->surface);
	{
		/* declarations */
		pixman_image_t *clip;
		pixman_image_t *mask;
		CPattern        pattern;
		CUInt32         w;
		CUInt32         h;
		CBool           gray;

		/* determine if we should use gray values */
		gray = CUtils_UseGray(_this->smoothingMode, _this->pixelOffsetMode);

		/* get the clip mask */
		CStatus_CheckGOTO
			(CSurface_GetClipMask
				(_this->surface, &clip, gray),
			 status,
			 GOTO_Cleanup);

		/* get the compositing mask */
		CStatus_CheckGOTO
			(CSurface_GetCompositingMask
				(_this->surface, &mask, gray),
			 status,
			 GOTO_Cleanup);

		/* get the region mask */
		CStatus_CheckGOTO
			(CRegion_GetMask
				(region, &CGraphicsPipeline_Device(_this->pipeline), mask),
			 status,
			 GOTO_Cleanup);

		/* get the width and height */
		w = (CUInt32)pixman_image_get_width(mask);
		h = (CUInt32)pixman_image_get_width(mask);

		/* clip the region mask */
		pixman_composite
			(PIXMAN_OPERATOR_IN_REVERSE, clip, 0, mask, 0, 0, 0, 0, 0, 0, w, h);

		/* get the pattern */
		CStatus_CheckGOTO
			(CBrush_GetPattern
				(brush, &pattern),
			 status,
			 GOTO_Cleanup);

		/* composite the region */
		status =
			CGraphics_Composite
				(_this, 0, 0, w, h, &pattern, mask);
	}
GOTO_Cleanup:
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}
#endif
/******************************************************************************/











/******************************************************************************/
#define CGraphics_STACK
#ifdef CGraphics_STACK
/* Save the current contents of the graphics context in a container. */
CStatus
CGraphics_BeginContainer(CGraphics          *_this,
                         CGraphicsContainer *container)
{
	/* TODO */
	return CStatus_NotImplemented;
}
CStatus
CGraphics_BeginContainer2(CGraphics          *_this,
                          CRectangleF         dst,
                          CRectangleF         src,
                          CGraphicsUnit       unit,
                          CGraphicsContainer *container)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Reset the graphics state back to a previous container level. */
CStatus
CGraphics_EndContainer(CGraphics          *_this,
                       CGraphicsContainer  container)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Restore to a previous save point. */
CStatus
CGraphics_Restore(CGraphics *_this,
                  CUInt32    state)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Save the current graphics state. */
CStatus
CGraphics_Save(CGraphics *_this,
               CUInt32   *state)
{
	/* TODO */
	return CStatus_NotImplemented;
}
#endif
/******************************************************************************/












/******************************************************************************/
#define CGraphics_TEXT
#ifdef CGraphics_TEXT
/* Draw a string. */
CStatus
CGraphics_DrawString(CGraphics     *_this,
                     CBrush        *brush,
                     const CChar16 *s,
                     CUInt32        length,
                     CFont         *font,
                     CRectangleF    layoutRect,
                     CStringFormat *format)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Measure the character ranges for a string. */
CStatus
CGraphics_MeasureCharacterRanges(CGraphics      *_this,
                                 const CChar16  *s,
                                 CUInt32         length,
                                 CFont          *font,
                                 CRectangleF     layoutRect,
                                 CStringFormat  *format,
                                 CRegion       **regions,
                                 CUInt32        *count)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Measure the size of a string. */
CStatus
CGraphics_MeasureString(CGraphics     *_this,
                        const CChar16 *s,
                        CUInt32        length,
                        CFont         *font,
                        CRectangleF    layoutRect,
                        CStringFormat *format,
                        CUInt32       *charactersFitted,
                        CUInt32       *linesFilled,
                        CSizeF        *size)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/*\
|*| NOTE: This is just a temporary solution until the above
|*|       methods are implemented and support is added for
|*|       a text layout system which can handle the complex
|*|       requirements of text boxes and rich text boxes.
\*/

/* Measure the size of a string. */
CStatus
CGraphics_MeasureStringSimple(CGraphics     *_this,
                              const CChar16 *s,
                              CUInt32        length,
                              CFont         *font,
                              CSizeF        *size)
{
	/* declarations */
	CTextMetrics metrics;
	CStatus      status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a string pointer */
	CStatus_Require((s != 0), CStatus_ArgumentNull);

	/* ensure we have a font pointer */
	CStatus_Require((font != 0), CStatus_ArgumentNull);

	/* ensure we have a size pointer */
	CStatus_Require((size != 0), CStatus_ArgumentNull);

	/* set the size to the default */
	CSize_Width(*size)  = 0;
	CSize_Height(*size) = 0;

	/* bail out now if there's nothing to do */
	CStatus_Require((length != 0), CStatus_OK);

	/* measure the string synchronously */
	CSurface_Lock(_this->surface);
	{
		status =
			CFont_MeasureString
				(font, s, length, &CGraphicsPipeline_Device(_this->pipeline),
				 _this->textRenderingHint, &metrics);
	}
	CSurface_Unlock(_this->surface);

	/* handle measuring failures */
	if(status != CStatus_OK) { return status; }

	/* get the font height */
	CStatus_Check
		(CFont_GetHeight
			(font, &CSize_Height(*size)));

	/* calculate the width */
	CSize_Width(*size) =
		(CVector_X(metrics.bearing) + CVector_X(metrics.advance));

	/* return successfully */
	return CStatus_OK;
}

/* Draw a string. */
CStatus
CGraphics_DrawStringSimple(CGraphics     *_this,
                           CBrush        *brush,
                           const CChar16 *s,
                           CUInt32        length,
                           CFont         *font,
                           CRectangleF    layoutRect)
{
	/* declarations */
	CPattern pattern;
	CStatus  status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* ensure we have a string pointer */
	CStatus_Require((s != 0), CStatus_ArgumentNull);

	/* ensure we have a font pointer */
	CStatus_Require((font != 0), CStatus_ArgumentNull);

	/* bail out now if there's nothing to do */
	CStatus_Require((length != 0), CStatus_OK);

	/* get the source pattern */
	CStatus_Check
		(CBrush_GetPattern
			(brush, &pattern));

	/* draw the string synchronously */
	CSurface_Lock(_this->surface);
	{
		/* declarations */
		pixman_image_t *clip;
		pixman_image_t *mask;
		CUInt32         width;
		CUInt32         height;

		/* get the clipping mask */
		CStatus_CheckGOTO
			(CGraphics_GetClipMask(_this, &clip),
			 status,
			 GOTO_Cleanup);

		/* get the text compositing mask */
		CStatus_CheckGOTO
			(CGraphics_GetTextCompositingMask(_this, &mask),
			 status,
			 GOTO_Cleanup);

		/* draw the string to the mask */
		CStatus_CheckGOTO
			(CFont_DrawString
				(font, s, length,
				 CRectangle_X(layoutRect),
				 CRectangle_Y(layoutRect),
				 &CGraphicsPipeline_Device(_this->pipeline),
				 _this->textRenderingHint, clip, mask),
			 status,
			 GOTO_Cleanup);

		/* get the layout bounds */
		width  = (CUInt32)CRectangle_X(layoutRect);
		height = (CUInt32)CRectangle_X(layoutRect);

		/* ensure that the bounds are non-zero */
		if(width  == 0) { width  = 32767; }
		if(height == 0) { height = 32767; }

		/* composite the string */
		status =
			CGraphics_Composite
				(_this, 0, 0, width, height, &pattern, mask);
	}
GOTO_Cleanup:
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}
#endif
/******************************************************************************/












/******************************************************************************/
#define CGraphics_UTILITIES
#ifdef CGraphics_UTILITIES
/* Flush graphics operations to the display device. */
CStatus
CGraphics_Flush(CGraphics       *_this,
                CFlushIntention  intention)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* flush the surface, synchronously */
	CSurface_Lock(_this->surface);
	{
		status = CSurface_Flush(_this->surface, intention);
	}
	CSurface_Unlock(_this->surface);

	/* return status */
	return status;
}

/* Get the HDC associated with this graphics context. */
CStatus
CGraphics_GetHdc(CGraphics  *_this,
                 void      **hdc)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Get the nearest color that is supported by this graphics context. */
CStatus
CGraphics_GetNearestColor(CGraphics *_this,
                          CColor     color,
                          CColor    *nearest)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a nearest color pointer */
	CStatus_Require((nearest != 0), CStatus_ArgumentNull);

	/* TODO: is there anything to do here? */

	/* get the nearest color */
	*nearest = color;

	/* return successfully */
	return CStatus_OK;
}

/* Release a HDC that was obtained via a previous call to "GetHdc". */
CStatus
CGraphics_ReleaseHdc(CGraphics *_this,
                     void      *hdc)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Transform points from one coordinate space to another. */
CStatus
CGraphics_TransformPoints(CGraphics        *_this,
                          CCoordinateSpace  dst,
                          CCoordinateSpace  src,
                          CPointF          *points,
                          CUInt32           count)
{
	/* declarations */
	CAffineTransformF t;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a point list pointer */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* bail out now if there's nothing to transform */
	CStatus_Require((count != 0), CStatus_OK);

	/* bail out now if there's nothing to do */
	CStatus_Require((src != dst), CStatus_OK);

	/* get the transformation, synchronously */
	CSurface_Lock(_this->surface);
	{
		/* get the transformation from source space to destination space */
		CGraphicsPipeline_GetSpaceTransform(&(_this->pipeline), dst, src, &t);
	}
	CSurface_Unlock(_this->surface);

	/* apply the transformation to the point list */
	CAffineTransformF_TransformPoints(&t, points, count);

	/* return successfully */
	return CStatus_OK;
}
#endif
/******************************************************************************/


#ifdef __cplusplus
};
#endif
