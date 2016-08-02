/*
 * CFlattener.h - Flattener header.
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

#ifndef _C_FLATTENER_H_
#define _C_FLATTENER_H_

#include "CBezier.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCFlattener CFlattener;
struct _tagCFlattener
{
	CPointArrayF  array;
	CPointF      *points;
	CByte        *types;
	CUInt32       count;
	CUInt32       capacity;
};

CINTERNAL void
CFlattener_Initialize(CFlattener *_this);
CINTERNAL void
CFlattener_Finalize(CFlattener *_this,
                    CPointF    **points,
                    CByte      **types,
                    CUInt32     *count,
                    CUInt32     *capacity);
CINTERNAL CStatus
CFlattener_Flatten(CFlattener *_this,
                   CPointF    *points,
                   CByte      *types,
                   CUInt32     count,
                   CFloat      tolerance);

#ifdef __cplusplus
};
#endif

#endif /* _C_FLATTENER_H_ */
