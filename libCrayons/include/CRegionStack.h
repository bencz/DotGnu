/*
 * CRegionStack.h - Region stack header.
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

#ifndef _C_REGIONSTACK_H_
#define _C_REGIONSTACK_H_

#include "CRegion.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCRegionStackNode CRegionStackNode;
struct _tagCRegionStackNode
{
	CUInt32    visited;
	CRegionOp *op;
	void      *left;
	void      *right;
};
static const CRegionStackNode CRegionStackNode_Zero;

#define CRegionStack_Top(stack) ((stack).elements[((stack).count - 1)])
#define CRegionStack_Size 32

typedef struct _tagCRegionStack CRegionStack;
struct _tagCRegionStack
{
	CRegionStackNode  elements[CRegionStack_Size];
	CUInt32           count;
	CRegionStack     *prev;
	CRegionStack     *next;
};
static const CRegionStack CRegionStack_Zero;

#define CRegionStack_Alloc(stack) \
	((stack) = ((CRegionStack *)CMalloc(sizeof(CRegionStack))))

CINTERNAL void
CRegionStack_Initialize(CRegionStack *_this);
CINTERNAL void
CRegionStack_Finalize(CRegionStack *_this);
CINTERNAL void
CRegionStack_Pop(CRegionStack **_this);
CINTERNAL CStatus
CRegionStack_Push(CRegionStack **_this,
                  CRegionOp     *op);
CINTERNAL void
CRegionStack_Reset(CRegionStack **_this);

#ifdef __cplusplus
};
#endif

#endif /* _C_REGIONSTACK_H_ */
