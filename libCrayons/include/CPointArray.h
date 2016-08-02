/*
 * CPointArray.h - Point array header.
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

#ifndef _C_POINTARRAY_H_
#define _C_POINTARRAY_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

static const CPointArrayX CPointArrayX_Zero;
static const CPointArrayF CPointArrayF_Zero;

CINTERNAL void
CPointArrayX_Initialize(CPointArrayX *_this);
CINTERNAL void
CPointArrayF_Initialize(CPointArrayF *_this);
CINTERNAL void
CPointArrayX_Finalize(CPointArrayX *_this);
CINTERNAL void
CPointArrayF_Finalize(CPointArrayF *_this);
CINTERNAL CStatus
CPointArrayX_AppendPointNoRepeat(CPointArrayX *_this,
                                 CPointX      *point);
CINTERNAL CStatus
CPointArrayF_AppendPointNoRepeat(CPointArrayF *_this,
                                 CPointF      *point);
CINTERNAL CStatus
CPointArrayX_AppendPoint(CPointArrayX *_this,
                         CPointX      *point);
CINTERNAL CStatus
CPointArrayF_AppendPoint(CPointArrayF *_this,
                         CPointF      *point);
CINTERNAL CStatus
CPointArrayX_EnsureCapacity(CPointArrayX *_this,
                            CUInt32       minimum);
CINTERNAL CStatus
CPointArrayF_EnsureCapacity(CPointArrayF *_this,
                            CUInt32       minimum);

#ifdef __cplusplus
};
#endif

#endif /* _C_POINTARRAY_H_ */
