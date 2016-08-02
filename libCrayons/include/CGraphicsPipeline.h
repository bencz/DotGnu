/*
 * CGraphicsPipeline.h - Graphics pipeline header.
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

#ifndef _C_GRAPHICSPIPELINE_H_
#define _C_GRAPHICSPIPELINE_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCGraphicsPipeline CGraphicsPipeline;
struct _tagCGraphicsPipeline
{
	/*\
	|*| w == world
	|*| p == page
	|*| d == device
	|*| t == transformation
	|*| i == inverse
	|*|
	|*| wt  : w -> p
	|*| wti : p -> w
	|*|
	|*| pt  : p -> d
	|*| pti : d -> p
	|*|
	|*| dt  : w -> d
	|*| dti : d -> w
	\*/
	CAffineTransformF worldTransform;
	CAffineTransformF worldTransformInverse;
	CAffineTransformF pageTransform;
	CAffineTransformF pageTransformInverse;
	CAffineTransformF deviceTransform;
	CAffineTransformF deviceTransformInverse;
};

#define CGraphicsPipeline_World(pipeline) \
			((pipeline).worldTransform)
#define CGraphicsPipeline_WorldInverse(pipeline) \
			((pipeline).worldTransformInverse)
#define CGraphicsPipeline_Page(pipeline) \
			((pipeline).pageTransform)
#define CGraphicsPipeline_PageInverse(pipeline) \
			((pipeline).pageTransformInverse)
#define CGraphicsPipeline_Device(pipeline) \
			((pipeline).deviceTransform)
#define CGraphicsPipeline_DeviceInverse(pipeline) \
			((pipeline).deviceTransformInverse)

CINTERNAL void
CGraphicsPipeline_GetDevice(CGraphicsPipeline *_this,
                            CAffineTransformF *transform);
CINTERNAL void
CGraphicsPipeline_GetDeviceInverse(CGraphicsPipeline *_this,
                                   CAffineTransformF *transform);
CINTERNAL void
CGraphicsPipeline_GetPage(CGraphicsPipeline *_this,
                          CAffineTransformF *transform);
CINTERNAL void
CGraphicsPipeline_GetPageInverse(CGraphicsPipeline *_this,
                                 CAffineTransformF *transform);
CINTERNAL void
CGraphicsPipeline_GetWorld(CGraphicsPipeline *_this,
                           CAffineTransformF *transform);
CINTERNAL void
CGraphicsPipeline_GetWorldInverse(CGraphicsPipeline *_this,
                                  CAffineTransformF *transform);
CINTERNAL void
CGraphicsPipeline_GetSpaceTransform(CGraphicsPipeline *_this,
                                    CCoordinateSpace   dst,
                                    CCoordinateSpace   src,
                                    CAffineTransformF *transform);
CINTERNAL void
CGraphicsPipeline_ResetPage(CGraphicsPipeline *_this);
CINTERNAL void
CGraphicsPipeline_SetPage(CGraphicsPipeline *_this,
                          CGraphicsUnit      pageUnit,
                          CFloat             pageScale,
                          CFloat             dpiX,
                          CFloat             dpiY);
CINTERNAL void
CGraphicsPipeline_ResetWorld(CGraphicsPipeline *_this);
CINTERNAL CStatus
CGraphicsPipeline_SetWorld(CGraphicsPipeline *_this,
                           CAffineTransformF *transform);
CINTERNAL CStatus
CGraphicsPipeline_MultiplyWorld(CGraphicsPipeline *_this,
                                CAffineTransformF *other,
                                CMatrixOrder       order);
CINTERNAL void
CGraphicsPipeline_RotateWorld(CGraphicsPipeline *_this,
                              CFloat             angle,
                              CMatrixOrder       order);
CINTERNAL void
CGraphicsPipeline_ScaleWorld(CGraphicsPipeline *_this,
                             CFloat             sx,
                             CFloat             sy,
                             CMatrixOrder       order);
CINTERNAL void
CGraphicsPipeline_TranslateWorld(CGraphicsPipeline *_this,
                                 CFloat             dx,
                                 CFloat             dy,
                                 CMatrixOrder       order);

#ifdef __cplusplus
};
#endif

#endif /* _C_GRAPHICSPIPELINE_H_ */
