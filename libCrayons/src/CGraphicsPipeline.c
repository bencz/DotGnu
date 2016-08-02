/*
 * CGraphicsPipeline.c - Graphics pipeline implementation.
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

#include "CGraphicsPipeline.h"
#include "CAffineTransform.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Handle changes to this graphics pipeline. */
static void
CGraphicsPipeline_OnChange(CGraphicsPipeline *_this)
{
	/* declarations */
	CAffineTransformF dt;
	CAffineTransformF dti;

	/* assertions */
	CASSERT((_this != 0));

	/* get the transformations and inverses */
	dt  = _this->worldTransform;
	dti = _this->pageTransformInverse;

	/* calculate the device transformation */
	CAffineTransformF_Multiply
		(&dt, &(_this->pageTransform), CMatrixOrder_Append);

	/* calculate the inverse device transformation */
	CAffineTransformF_Multiply
		(&dti, &(_this->worldTransformInverse), CMatrixOrder_Append);

	/* set the device transformation and inverse */
	_this->deviceTransform        = dt;
	_this->deviceTransformInverse = dti;
}

/* Get the device transformation of this graphics pipeline */
CINTERNAL void
CGraphicsPipeline_GetDevice(CGraphicsPipeline *_this,
                            CAffineTransformF *transform)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* get the device transformation */
	*transform = _this->deviceTransform;
}

/* Get the inverse device transformation of this graphics pipeline */
CINTERNAL void
CGraphicsPipeline_GetDeviceInverse(CGraphicsPipeline *_this,
                                   CAffineTransformF *transform)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* get the inverse device transformation */
	*transform = _this->deviceTransformInverse;
}

/* Get the page transformation of this graphics pipeline */
CINTERNAL void
CGraphicsPipeline_GetPage(CGraphicsPipeline *_this,
                          CAffineTransformF *transform)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* get the page transformation */
	*transform = _this->pageTransform;
}

/* Get the inverse page transformation of this graphics pipeline */
CINTERNAL void
CGraphicsPipeline_GetPageInverse(CGraphicsPipeline *_this,
                                 CAffineTransformF *transform)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* get the inverse page transformation */
	*transform = _this->pageTransformInverse;
}

/* Get the world transformation of this graphics pipeline */
CINTERNAL void
CGraphicsPipeline_GetWorld(CGraphicsPipeline *_this,
                           CAffineTransformF *transform)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* get the world transformation */
	*transform = _this->worldTransform;
}

/* Get the inverse world transformation of this graphics pipeline */
CINTERNAL void
CGraphicsPipeline_GetWorldInverse(CGraphicsPipeline *_this,
                                  CAffineTransformF *transform)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* get the inverse world transformation */
	*transform = _this->worldTransformInverse;
}

/* Get the transformation from one coordinate space to another. */
CINTERNAL void
CGraphicsPipeline_GetSpaceTransform(CGraphicsPipeline *_this,
                                    CCoordinateSpace   dst,
                                    CCoordinateSpace   src,
                                    CAffineTransformF *transform)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));
	CASSERT((src       != dst));

	/* get the transformation from source space to destination space */
	switch(src)
	{
		case CCoordinateSpace_World:
		{
			/* get the transformation from world space to destination space */
			if(dst == CCoordinateSpace_Device)
			{
				*transform = _this->deviceTransform;
			}
			else /* dst == CCoordinateSpace_Page */
			{
				*transform = _this->worldTransform;
			}
		}
		break;

		case CCoordinateSpace_Page:
		{
			/* get the transformation from page space to destination space */
			if(dst == CCoordinateSpace_World)
			{
				*transform = _this->worldTransformInverse;
			}
			else /* dst == CCoordinateSpace_Device */
			{
				*transform = _this->pageTransform;
			}
		}
		break;

		case CCoordinateSpace_Device:
		{
			/* get the transformation from device space to destination space */
			if(dst == CCoordinateSpace_Page)
			{
				*transform = _this->pageTransformInverse;
			}
			else /* dst == CCoordinateSpace_World */
			{
				*transform = _this->deviceTransformInverse;
			}
		}
		break;
	}
}

/* Reset the page transformation of this graphics pipeline. */
CINTERNAL void
CGraphicsPipeline_ResetPage(CGraphicsPipeline *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* set the page transformation and inverse */
	_this->pageTransform        = CAffineTransformF_Identity;
	_this->pageTransformInverse = CAffineTransformF_Identity;

	/* update the device transformation and inverse */
	_this->deviceTransform        = _this->worldTransform;
	_this->deviceTransformInverse = _this->worldTransformInverse;
}

/* Set the page transformation of this graphics pipeline. */
CINTERNAL void
CGraphicsPipeline_SetPage(CGraphicsPipeline *_this,
                          CGraphicsUnit      pageUnit,
                          CFloat             pageScale,
                          CFloat             dpiX,
                          CFloat             dpiY)
{
	/* declarations */
	CFloat sx;
	CFloat sy;
	CFloat sxi;
	CFloat syi;

	/* assertions */
	CASSERT((_this != 0));

	/* factor in the page scaling factor */
	if(pageScale != 1.0f)
	{
		dpiX *= pageScale;
		dpiY *= pageScale;
	}

	/* calculate the scaling factors */
	switch(pageUnit)
	{
		case CGraphicsUnit_Display:
		{
			sx  = (dpiX  / 75.0f);
			sy  = (dpiY  / 75.0f);
			sxi = (75.0f / dpiX);
			syi = (75.0f / dpiY);
		}
		break;

		case CGraphicsUnit_Point:
		{
			sx  = (dpiX  / 72.0f);
			sy  = (dpiY  / 72.0f);
			sxi = (72.0f / dpiX);
			syi = (72.0f / dpiY);
		}
		break;

		case CGraphicsUnit_Inch:
		{
			sx  = (dpiX / 1.0f);
			sy  = (dpiY / 1.0f);
			sxi = (1.0f / dpiX);
			syi = (1.0f / dpiY);
		}
		break;

		case CGraphicsUnit_Document:
		{
			sx  = (dpiX   / 300.0f);
			sy  = (dpiY   / 300.0f);
			sxi = (300.0f / dpiX);
			syi = (300.0f / dpiY);
		}
		break;

		case CGraphicsUnit_Millimeter:
		{
			sx  = (dpiX  / 25.4f);
			sy  = (dpiY  / 25.4f);
			sxi = (25.4f / dpiX);
			syi = (25.4f / dpiY);
		}
		break;

		case CGraphicsUnit_World:
		case CGraphicsUnit_Pixel:
		default:
		{
			sx  = (pageScale / 1.0f);
			sy  = (pageScale / 1.0f);
			sxi = (1.0f      / pageScale);
			syi = (1.0f      / pageScale);
		}
		break;
	}

	/* set the page transformation */
	CAffineTransformF_SetElements
		(&(_this->pageTransform), sx, 0.0f, 0.0f, sy, 0.0f, 0.0f);

	/* set the inverse page transformation */
	CAffineTransformF_SetElements
		(&(_this->pageTransformInverse), sxi, 0.0f, 0.0f, syi, 0.0f, 0.0f);

	/* update the device transformation and inverse */
	CGraphicsPipeline_OnChange(_this);
}

/* Reset the world transformation of this graphics pipeline. */
CINTERNAL void
CGraphicsPipeline_ResetWorld(CGraphicsPipeline *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* set the world transformation and inverse */
	_this->worldTransform        = CAffineTransformF_Identity;
	_this->worldTransformInverse = CAffineTransformF_Identity;

	/* update the device transformation and inverse */
	_this->deviceTransform        = _this->pageTransform;
	_this->deviceTransformInverse = _this->pageTransformInverse;
}

/* Set the world transformation of this graphics pipeline. */
CINTERNAL CStatus
CGraphicsPipeline_SetWorld(CGraphicsPipeline *_this,
                           CAffineTransformF *transform)
{
	/* declarations */
	CAffineTransformF inverse;

	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* get the inverse transformation */
	CStatus_Check
		(CAffineTransformF_GetInverse
			(transform, &inverse));

	/* set the world transformation and inverse */
	_this->worldTransform        = *transform;
	_this->worldTransformInverse =  inverse;

	/* update the device transformation and inverse */
	CGraphicsPipeline_OnChange(_this);

	/* return successfully */
	return CStatus_OK;
}

/* Multiply the world transformation by another transformation. */
CINTERNAL CStatus
CGraphicsPipeline_MultiplyWorld(CGraphicsPipeline *_this,
                                CAffineTransformF *other,
                                CMatrixOrder       order)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((other != 0));

	/* multiply the inverse transformation */
	CStatus_Check
		(CAffineTransformF_MultiplyInverse
			(&(_this->worldTransformInverse), other, order));

	/* multiply the transformation */
	CAffineTransformF_Multiply
		(&(_this->worldTransform), other, order);

	/* update the device transformation and inverse */
	CGraphicsPipeline_OnChange(_this);

	/* return successfully */
	return CStatus_OK;
}

/* Rotate the world transformation of this graphics pipeline. */
CINTERNAL void
CGraphicsPipeline_RotateWorld(CGraphicsPipeline *_this,
                              CFloat             angle,
                              CMatrixOrder       order)
{
	/* assertions */
	CASSERT((_this != 0));

	/* rotate the transformation */
	CAffineTransformF_Rotate
		(&(_this->worldTransform), angle, order);

	/* rotate the inverse transformation */
	CAffineTransformF_RotateInverse
		(&(_this->worldTransformInverse), angle, order);

	/* update the device transformation and inverse */
	CGraphicsPipeline_OnChange(_this);
}

/* Scale the world transformation of this graphics pipeline. */
CINTERNAL void
CGraphicsPipeline_ScaleWorld(CGraphicsPipeline *_this,
                             CFloat             sx,
                             CFloat             sy,
                             CMatrixOrder       order)
{
	/* assertions */
	CASSERT((_this != 0));

	/* scale the transformation */
	CAffineTransformF_Scale
		(&(_this->worldTransform), sx, sy, order);

	/* scale the inverse transformation */
	CAffineTransformF_ScaleInverse
		(&(_this->worldTransformInverse), sx, sy, order);

	/* update the device transformation and inverse */
	CGraphicsPipeline_OnChange(_this);
}

/* Translate the world transformation of this graphics pipeline. */
CINTERNAL void
CGraphicsPipeline_TranslateWorld(CGraphicsPipeline *_this,
                                 CFloat             dx,
                                 CFloat             dy,
                                 CMatrixOrder       order)
{
	/* assertions */
	CASSERT((_this != 0));

	/* translate the transformation */
	CAffineTransformF_Translate
		(&(_this->worldTransform), dx, dy, order);

	/* translate the inverse transformation */
	CAffineTransformF_TranslateInverse
		(&(_this->worldTransformInverse), dx, dy, order);

	/* update the device transformation and inverse */
	CGraphicsPipeline_OnChange(_this);
}


#ifdef __cplusplus
};
#endif
