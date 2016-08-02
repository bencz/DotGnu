/*
 * CBlend.h - Gradient blending header.
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

#ifndef _C_BLEND_H_
#define _C_BLEND_H_

#include "CBrush.h"

#ifdef __cplusplus
extern "C" {
#endif

static const CBlend      CBlend_Zero      = { 0, 0, 0 };
static const CColorBlend CColorBlend_Zero = { 0, 0, 0 };

#define CBlend_TriangularHalfCount 2
#define CBlend_TriangularFullCount 3
#define CBlend_SigmaBellHalfCount  256
#define CBlend_SigmaBellFullCount  511

CINTERNAL CStatus
CBlend_Initialize(CBlend  *_this,
                  CUInt32  count);
CINTERNAL CStatus
CBlend_Copy(CBlend *_this,
            CBlend *copy);
CINTERNAL void
CBlend_SetTriangularShape(CBlend *_this,
                          CFloat  focus,
                          CFloat  scale);
CINTERNAL void
CBlend_SetSigmaBellShape(CBlend *_this,
                         CFloat  focus,
                         CFloat  scale);
CINTERNAL void
CBlend_Finalize(CBlend *_this);
CINTERNAL CStatus
CColorBlend_Copy(CColorBlend *_this,
                 CColorBlend *copy);
CINTERNAL void
CColorBlend_Finalize(CColorBlend *_this);

#ifdef __cplusplus
};
#endif

#endif /* _C_BLEND_H_ */
