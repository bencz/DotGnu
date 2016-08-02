/*
 * CRegion.h - Region header.
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

#ifndef _C_REGION_H_
#define _C_REGION_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef CUInt32 CRegionType;
#define CRegionType_Intersect  0x00000001
#define CRegionType_Union      0x00000002
#define CRegionType_Xor        0x00000003
#define CRegionType_Exclude    0x00000004
#define CRegionType_Complement 0x00000005
#define CRegionType_Rectangle  0x10000000
#define CRegionType_Path       0x10000001
#define CRegionType_Empty      0x10000002
#define CRegionType_Infinite   0x10000003

typedef struct _tagCRegionNode CRegionNode;
typedef struct _tagCRegionOp   CRegionOp;
typedef struct _tagCRegionRect CRegionRect;
typedef struct _tagCRegionPath CRegionPath;
typedef struct _tagCRegionMask CRegionMask;

struct _tagCRegionNode
{
	CRegionType  type;
};
struct _tagCRegionOp
{
	CRegionNode  _base;
	CRegionNode *left;
	CRegionNode *right;
};
struct _tagCRegionRect
{
	CRegionNode _base;
	CRectangleF rectangle;
};
struct _tagCRegionPath
{
	CRegionNode  _base;
	CPointF     *points;
	CByte       *types;
	CUInt32      count;
	CFillMode    fillMode;
};
struct _tagCRegionMask
{
	pixman_image_t     *image;
	CAffineTransformF   transform;
};
struct _tagCRegion
{
	CRegionNode *head;
	CRegionMask  mask;
};

#define CRegionNode_Default(type) \
	static const CRegionNode CRegionNode_ ## type = \
		{ CRegionType_ ## type }

CRegionNode_Default(Intersect);
CRegionNode_Default(Union);
CRegionNode_Default(Xor);
CRegionNode_Default(Exclude);
CRegionNode_Default(Complement);
CRegionNode_Default(Rectangle);
CRegionNode_Default(Path);
CRegionNode_Default(Empty);
CRegionNode_Default(Infinite);

#define CRegionOp_Alloc(node) \
	((node) = ((CRegionOp *)CMalloc(sizeof(CRegionOp))))
#define CRegionNode_Alloc(node) \
	((node) = ((CRegionNode *)CMalloc(sizeof(CRegionNode))))
#define CRegionRect_Alloc(node) \
	((node) = ((CRegionRect *)CMalloc(sizeof(CRegionRect))))
#define CRegionPath_Alloc(node) \
	((node) = ((CRegionPath *)CMalloc(sizeof(CRegionPath))))

#define CRegionNode_Type(node)   (((CRegionNode *)(node))->type)
#define CRegionNode_IsData(node) ((CRegionNode_Type(node)) & 0x10000000)
#define CRegionNode_IsOp(node)   (!(CRegionNode_IsData((node))))

static const CByte CRegionRect_PathTypes[] =
{
	CPathType_Start,
	CPathType_Line,
	CPathType_Line,
	(CPathType_Line | CPathType_CloseSubpath)
};

#define CRegionRect_RectToPath(points, rectangle)                              \
	do {                                                                       \
		/* declarations */                                                     \
		CPointF *curr;                                                         \
		                                                                       \
		/* get the edges of the rectangle */                                   \
		const CFloat top = CRectangle_Y((rectangle));                          \
		const CFloat left = CRectangle_X((rectangle));                         \
		const CFloat right = CRectangle_Width((rectangle)) + left;             \
		const CFloat bottom = CRectangle_Height((rectangle)) + top;            \
		                                                                       \
		/* get the current point pointer */                                    \
		curr = (points);                                                       \
		                                                                       \
		/* set the first point */                                              \
		CPoint_X(*curr) = left;                                                \
		CPoint_Y(*curr) = top;                                                 \
		                                                                       \
		/* move to the next position */                                        \
		++curr;                                                                \
		                                                                       \
		/* set the second point */                                             \
		CPoint_X(*curr) = right;                                               \
		CPoint_Y(*curr) = top;                                                 \
		                                                                       \
		/* move to the next position */                                        \
		++curr;                                                                \
		                                                                       \
		/* set the third point */                                              \
		CPoint_X(*curr) = right;                                               \
		CPoint_Y(*curr) = bottom;                                              \
		                                                                       \
		/* move to the next position */                                        \
		++curr;                                                                \
		                                                                       \
		/* set the fourth point */                                             \
		CPoint_X(*curr) = left;                                                \
		CPoint_Y(*curr) = bottom;                                              \
	} while(0)

CINTERNAL void
CRegionData_Free(CRegionNode *node);
CINTERNAL CStatus
CRegion_GetMask(CRegion           *_this,
                CAffineTransformF *transform,
                pixman_image_t    *mask);

/*\
|*| NOTE: these declarations should be moved to the public
|*|       header once they're properly implemented
\*/
CStatus
CRegion_CreateData(CRegion **_this,
                   CByte    *data,
                   CUInt32   count);
CStatus
CRegion_CreateHRGN(CRegion **_this,
                   void      *hrgn);

#ifdef __cplusplus
};
#endif

#endif /* _C_REGION_H_ */
