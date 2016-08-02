/*
 * CRegion.c - Region implementation.
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

#include "CRegion.h"
#include "CMatrix.h"
#include "CRegionDisposer.h"
#include "CRegionCloner.h"
#include "CRegionRasterizer.h"
#include "CRegionTransformer.h"
#include "CRegionTranslator.h"

#ifdef __cplusplus
extern "C" {
#endif

static const CRegionMask CRegionMask_Zero =
	{ 0, { 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f } };

#define _OperationPath(_this, type, path)                                      \
	do {                                                                       \
		/* declarations */                                                     \
		CRegionOp   *_op;                                                      \
		CRegionPath *_data;                                                    \
		                                                                       \
		/* create the path node */                                             \
		CStatus_Check                                                          \
			(CRegionPath_Create                                                \
				(&_data, (path)));                                             \
		                                                                       \
		/* create the operation node */                                        \
		if(!(CRegionOp_Alloc(_op)))                                            \
		{                                                                      \
			return CStatus_OutOfMemory;                                        \
		}                                                                      \
		                                                                       \
		/* initialize the operation node */                                    \
		_op->_base = CRegionNode_ ## type;                                     \
		_op->left  = (_this)->head;                                            \
		_op->right = ((CRegionNode *)_data);                                   \
		                                                                       \
		/* reset the head node */                                              \
		(_this)->head = ((CRegionNode *)_op);                                  \
		                                                                       \
		/* handle change event */                                              \
		CRegion_OnChange((_this));                                             \
	} while(0)
#define _OperationRectangle(_this, type, rectangle)                            \
	do {                                                                       \
		/* declarations */                                                     \
		CRegionOp   *_op;                                                      \
		CRegionRect *_data;                                                    \
		                                                                       \
		/* create the rectangle node */                                        \
		CStatus_Check                                                          \
			(CRegionRect_Create                                                \
				(&_data, (rectangle)));                                        \
		                                                                       \
		/* create the operation node */                                        \
		if(!(CRegionOp_Alloc(_op)))                                            \
		{                                                                      \
			return CStatus_OutOfMemory;                                        \
		}                                                                      \
		                                                                       \
		/* initialize the operation node */                                    \
		_op->_base = CRegionNode_ ## type;                                     \
		_op->left  = (_this)->head;                                            \
		_op->right = ((CRegionNode *)_data);                                   \
		                                                                       \
		/* reset the head node */                                              \
		(_this)->head = ((CRegionNode *)_op);                                  \
		                                                                       \
		/* handle change event */                                              \
		CRegion_OnChange((_this));                                             \
	} while(0)
#define _OperationRegion(_this, type, other)                                   \
	do {                                                                       \
		/* declarations */                                                     \
		CRegionOp   *_op;                                                      \
		CRegionNode *_data;                                                    \
		                                                                       \
		/* create the copy nodes */                                            \
		CStatus_Check                                                          \
			(CRegionNode_Clone                                                 \
				((other)->head, &_data));                                      \
		                                                                       \
		/* create the operation node */                                        \
		if(!(CRegionOp_Alloc(_op)))                                            \
		{                                                                      \
			return CStatus_OutOfMemory;                                        \
		}                                                                      \
		                                                                       \
		/* initialize the operation node */                                    \
		_op->_base = CRegionNode_ ## type;                                     \
		_op->left  = (_this)->head;                                            \
		_op->right = ((CRegionNode *)_data);                                   \
		                                                                       \
		/* reset the head node */                                              \
		(_this)->head = ((CRegionNode *)_op);                                  \
		                                                                       \
		/* handle change event */                                              \
		CRegion_OnChange((_this));                                             \
	} while(0)

CINTERNAL void
CRegionData_Free(CRegionNode *node)
{
	/* assertions */
	CASSERT((node != 0));

	/* dispose of path data, as needed */
	if(node->type == CRegionType_Path)
	{
		/* get the path node */
		CRegionPath *rp = ((CRegionPath *)node);

		/* free the point list */
		CFree(rp->points);

		/* free the type list */
		CFree(rp->types);
	}

	/* free the node */
	CFree(node);
}

static CStatus
CRegionPath_Create(CRegionPath **_this,
                   CPath        *path)
{
	/* declarations */
	CPointF     *points;
	CByte       *types;
	CUInt32      count;
	CFillMode    fillMode;

	/* assertions */
	CASSERT((_this != 0));

	/* get the fill mode */
	CStatus_Check
		(CPath_GetFillMode
			(path, &fillMode));

	/* get the path data */
	CStatus_Check
		(CPath_GetPathData
			(path, &points, &types, &count));

	/* create the node */
	if(!(CRegionPath_Alloc(*_this)))
	{
		CFree(points);
		CFree(types);
		return CStatus_OutOfMemory;
	}

	/* initialize the node */
	(*_this)->_base    = CRegionNode_Path;
	(*_this)->points   = points;
	(*_this)->types    = types;
	(*_this)->count    = count;
	(*_this)->fillMode = fillMode;

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CRegionRect_Create(CRegionRect **_this,
                   CRectangleF   rectangle)
{
	/* assertions */
	CASSERT((_this     != 0));

	/* create the node */
	if(!(CRegionRect_Alloc(*_this)))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the node */
	(*_this)->_base     = CRegionNode_Rectangle;
	(*_this)->rectangle = rectangle;

	/* return successfully */
	return CStatus_OK;
}


static CStatus
CRegionNode_Clone(CRegionNode  *_this,
                  CRegionNode **clone)
{
	/* declarations */
	CRegionCloner cloner;
	CStatus       status;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((clone != 0));

	/* initialize the cloner */
	CRegionCloner_Initialize(&cloner);

	/* clone the node */
	status =
		CRegionInterpreter_Interpret
			(((CRegionInterpreter *)(&cloner)), _this, (void **)clone);

	/* finalize the cloner */
	CRegionCloner_Finalize(&cloner);

	/* return status */
	return status;
}

/* Handle change events. */
static void
CRegion_OnChange(CRegion *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* clear the mask */
	if(_this->mask.image != 0)
	{
		pixman_image_destroy(_this->mask.image);
		_this->mask = CRegionMask_Zero;
	}
}

/* Generate the mask for this region. */
static CStatus
CRegion_GenerateMask(CRegion           *_this,
                     CAffineTransformF *transform,
                     CUInt32            width,
                     CUInt32            height,
                     CBool              gray)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* clear the mask, as needed */
	if(_this->mask.image != 0)
	{
		/* declarations */
		CUInt32 w;
		CUInt32 h;
		CBool   g;

		/* get the mask image information */
		w = pixman_image_get_width(_this->mask.image);
		h = pixman_image_get_height(_this->mask.image);
		g = (pixman_image_get_depth(_this->mask.image) != 1);

		/* bail out now if the mask is okay as is */
		if(w == width && h == height && g == gray)
		{
			/* bail out now if everything matches */
			CStatus_Require
				((!CAffineTransformF_Equals
					(transform, &(_this->mask.transform))),
				 CStatus_OK);
		}

		/* destroy the current mask image */
		pixman_image_destroy(_this->mask.image);

		/* reset the mask */
		_this->mask = CRegionMask_Zero;
	}

	/* generate a new mask */
	{
		/* declarations */
		CRegionRasterizer rast;
		CStatus           status;

		/* initialize the rasterizer */
		CStatus_Check
			(CRegionRasterizer_Initialize
				(&rast, transform, width, height, gray));

		/* generate the mask image */
		status =
			CRegionInterpreter_Interpret
				((CRegionInterpreter *)&rast,
				 _this->head, (void **)&(_this->mask.image));

		/* finalize the rasterizer */
		CRegionRasterizer_Finalize(&rast);

		/* handle mask generation failures */
		CStatus_Check(status);

		/* set the mask transform */
		_this->mask.transform = *transform;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Initialize this region. */
static void
CRegion_Initialize(CRegion     *_this,
                   CRegionNode *head)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((head  != 0));

	/* initialize the region */
	_this->head = head;

	/* initialize the mask */
	_this->mask = CRegionMask_Zero;
}

/* Finalize this region. */
static void
CRegion_Finalize(CRegion *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the region */
	{
		/* declarations */
		CRegionNode *node;

		/* get the head node */
		node = _this->head;

		/* dispose of nodes, as needed */
		if(node != 0)
		{
			if(CRegionNode_IsData(node))
			{
				CRegionData_Free(node);
			}
			else
			{
				/* declarations */
				CRegionDisposer  disposer;
				void            *data;

				/* set the data to the default */
				data = 0;

				/* initialize the disposer */
				CRegionDisposer_Initialize(&disposer);

				/* dispose of the nodes */
				CRegionInterpreter_Interpret
					(((CRegionInterpreter *)(&disposer)), _this->head, &data);

				/* finalize the disposer */
				CRegionDisposer_Finalize(&disposer);
			}
		}

		/* reset the head pointer */
		_this->head = 0;

		/* finalize the mask */
		if(_this->mask.image != 0)
		{
			pixman_image_destroy(_this->mask.image);
			_this->mask = CRegionMask_Zero;
		}
	}
}

/* Create an infinite region. */
CStatus
CRegion_Create(CRegion **_this)
{
	/* declarations */
	CRegionNode *node;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the region */
	if(!(*_this = (CRegion *)CMalloc(sizeof(CRegion))))
	{
		return CStatus_OutOfMemory;
	}

	/* create the node */
	if(!(CRegionNode_Alloc(node)))
	{
		CFree(*_this);
		*_this = 0;
		return CStatus_OutOfMemory;
	}

	/* initialize the node */
	*node = CRegionNode_Infinite;

	/* initialize the region */
	CRegion_Initialize(*_this, node);

	/* return successfully */
	return CStatus_OK;
}

/* Create a path region. */
CStatus
CRegion_CreatePath(CRegion **_this,
                   CPath    *path)
{
	/* declarations */
	CRegionPath *node;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* create the path node */
	CStatus_Check
		(CRegionPath_Create
			(&node, path));

	/* allocate the region */
	if(!(*_this = (CRegion *)CMalloc(sizeof(CRegion))))
	{
		CRegionData_Free((CRegionNode *)node);
		return CStatus_OutOfMemory;
	}

	/* initialize the region */
	CRegion_Initialize(*_this, (CRegionNode *)node);

	/* return successfully */
	return CStatus_OK;
}

/* Create a rectangular region. */
CStatus
CRegion_CreateRectangle(CRegion     **_this,
                        CRectangleF   rectangle)
{
	/* declarations */
	CRegionRect *node;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* create the rectangle node */
	CStatus_Check
		(CRegionRect_Create
			(&node, rectangle));

	/* allocate the region */
	if(!(*_this = (CRegion *)CMalloc(sizeof(CRegion))))
	{
		CRegionData_Free((CRegionNode *)node);
		return CStatus_OutOfMemory;
	}

	/* initialize the region */
	CRegion_Initialize(*_this, (CRegionNode *)node);

	/* return successfully */
	return CStatus_OK;
}

/* Create a region from serialized region data. */
CStatus
CRegion_CreateData(CRegion **_this,
                   CByte    *data,
                   CUInt32   count)
{
#if 0
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the region */
	if(!(*_this = (CRegion *)CMalloc(sizeof(CRegion))))
	{
		return CStatus_OutOfMemory;
	}

	/* TODO */

	/* return successfully */
	return CStatus_OK;
#else
	return CStatus_NotImplemented;
#endif
}

/* Create a region from a GDI region. */
CStatus
CRegion_CreateHRGN(CRegion **_this,
                   void      *hrgn)
{
#if 0
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the region */
	if(!(*_this = (CRegion *)CMalloc(sizeof(CRegion))))
	{
		return CStatus_OutOfMemory;
	}

	/* TODO: add support for wine and native win32 */

	/* return successfully */
	return CStatus_OK;
#else
	return CStatus_NotImplemented;
#endif
}

/* Destroy a region. */
CStatus
CRegion_Destroy(CRegion **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a this pointer */
	CStatus_Require(((*_this) != 0), CStatus_ArgumentNull);

	/* finalize the region */
	CRegion_Finalize(*_this);

	/* free the region */
	CFree(*_this);

	/* null the this pointer */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/* Clone this region. */
CStatus
CRegion_Clone(CRegion  *_this,
              CRegion **clone)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a clone pointer pointer */
	CStatus_Require((clone != 0), CStatus_ArgumentNull);

	/* create the clone region */
	CStatus_Check(CRegion_Create(clone));

	/* clone the members */
	{
		/* declarations */
		CStatus status;

		/* copy the nodes */
		status = CRegion_CombineRegion(*clone, _this, CCombineMode_Replace);

		/* handle status */
		if(status != CStatus_OK)
		{
			/* destroy the clone */
			CRegion_Destroy(clone);

			/* return status */
			return status;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Form the combination of this region with a path. */
CStatus
CRegion_CombinePath(CRegion      *_this,
                    CPath        *path,
                    CCombineMode  combineMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a path pointer */
	CStatus_Require((path != 0), CStatus_ArgumentNull);

	/* ensure the combination mode is in range */
	CCombineMode_Default(combineMode);

	/* form the combination */
	{
		/* declarations */
		CRegionType t;

		/* get the head node type */
		t = CRegionNode_Type(_this->head);

		/* form the combination */
		switch(combineMode)
		{
			case CCombineMode_Intersect:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((t != CRegionType_Empty), CStatus_OK);

				/* replace, as needed */
				if(t == CRegionType_Infinite) { goto _Replace; }

				/* form the intersection */
				_OperationPath(_this, Intersect, path);
			}
			break;
			case CCombineMode_Union:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((t != CRegionType_Infinite), CStatus_OK);

				/* replace, as needed */
				if(t == CRegionType_Empty) { goto _Replace; }

				/* form the union */
				_OperationPath(_this, Union, path);
			}
			break;
			case CCombineMode_Xor:
			{
				/* replace, as needed */
				if(t == CRegionType_Empty) { goto _Replace; }

				/* form the xor */
				_OperationPath(_this, Xor, path);
			}
			break;
			case CCombineMode_Exclude:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((t != CRegionType_Empty), CStatus_OK);

				/* form the exclusion */
				_OperationPath(_this, Exclude, path);
			}
			break;
			case CCombineMode_Complement:
			{
				/* replace, as needed */
				if(t == CRegionType_Empty) { goto _Replace; }

				/* form the empty region, as needed */
				if(t == CRegionType_Infinite)
				{
					return CRegion_MakeEmpty(_this);
				}

				/* form the complement */
				_OperationPath(_this, Complement, path);
			}
			break;
			case CCombineMode_Replace:
			default:
			{
				/* provide local jump target */
				_Replace:

				/* add new path */
				{
					/* declarations */
					CRegionPath *data;

					/* create the path node */
					CStatus_Check
						(CRegionPath_Create
							(&data, path));

					/* dispose of current region data */
					CRegion_Finalize(_this);

					/* reset the head node */
					_this->head = (CRegionNode *)data;
				}
			}
			break;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Form the combination of this region with a rectangle. */
CStatus
CRegion_CombineRectangle(CRegion      *_this,
                         CRectangleF   rectangle,
                         CCombineMode  combineMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure the combination mode is in range */
	CCombineMode_Default(combineMode);

	/* form the combination */
	{
		/* declarations */
		CRegionType t;

		/* get the head node type */
		t = CRegionNode_Type(_this->head);

		/* form the combination */
		switch(combineMode)
		{
			case CCombineMode_Intersect:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((t != CRegionType_Empty), CStatus_OK);

				/* replace, as needed */
				if(t == CRegionType_Infinite) { goto _Replace; }

				/* form the intersection */
				_OperationRectangle(_this, Intersect, rectangle);
			}
			break;
			case CCombineMode_Union:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((t != CRegionType_Infinite), CStatus_OK);

				/* replace, as needed */
				if(t == CRegionType_Empty) { goto _Replace; }

				/* form the union */
				_OperationRectangle(_this, Union, rectangle);
			}
			break;
			case CCombineMode_Xor:
			{
				/* replace, as needed */
				if(t == CRegionType_Empty) { goto _Replace; }

				/* form the xor */
				_OperationRectangle(_this, Xor, rectangle);
			}
			break;
			case CCombineMode_Exclude:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((t != CRegionType_Empty), CStatus_OK);

				/* form the exclusion */
				_OperationRectangle(_this, Exclude, rectangle);
			}
			break;
			case CCombineMode_Complement:
			{
				/* replace, as needed */
				if(t == CRegionType_Empty) { goto _Replace; }

				/* form the empty region, as needed */
				if(t == CRegionType_Infinite)
				{
					return CRegion_MakeEmpty(_this);
				}

				/* form the complement */
				_OperationRectangle(_this, Complement, rectangle);
			}
			break;
			case CCombineMode_Replace:
			default:
			{
				/* provide local jump target */
				_Replace:

				/* add new rectangle */
				{
					/* declarations */
					CRegionRect *data;

					/* create the rectangle node */
					CStatus_Check
						(CRegionRect_Create
							(&data, rectangle));

					/* dispose of current region data */
					CRegion_Finalize(_this);

					/* reset the head node */
					_this->head = (CRegionNode *)data;
				}
			}
			break;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Form the combination of this region with another region. */
CStatus
CRegion_CombineRegion(CRegion      *_this,
                      CRegion      *other,
                      CCombineMode  combineMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an other pointer */
	CStatus_Require((other != 0), CStatus_ArgumentNull);

	/* ensure the combination mode is in range */
	CCombineMode_Default(combineMode);

	/* form the combination */
	{
		/* declarations */
		CRegionType tt;
		CRegionType ot;

		/* get the head node types */
		tt = CRegionNode_Type(_this->head);
		ot = CRegionNode_Type(other->head);

		/* form the combination */
		switch(combineMode)
		{
			case CCombineMode_Intersect:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((tt != CRegionType_Empty), CStatus_OK);
				CStatus_Require((ot != CRegionType_Infinite), CStatus_OK);

				/* replace, as needed */
				if(tt == CRegionType_Infinite) { goto _Replace; }

				/* form the empty region, as needed */
				if(ot == CRegionType_Empty)
				{
					return CRegion_MakeEmpty(_this);
				}

				/* form the intersection */
				_OperationRegion(_this, Intersect, other);
			}
			break;
			case CCombineMode_Union:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((ot != CRegionType_Empty), CStatus_OK);
				CStatus_Require((tt != CRegionType_Infinite), CStatus_OK);

				/* replace, as needed */
				if(tt == CRegionType_Empty) { goto _Replace; }

				/* form the infinite region, as needed */
				if(ot == CRegionType_Infinite)
				{
					return CRegion_MakeInfinite(_this);
				}

				/* form the union */
				_OperationRegion(_this, Union, other);
			}
			break;
			case CCombineMode_Xor:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((ot != CRegionType_Empty), CStatus_OK);

				/* replace, as needed */
				if(tt == CRegionType_Empty) { goto _Replace; }

				/* form the xor */
				_OperationRegion(_this, Xor, other);
			}
			break;
			case CCombineMode_Exclude:
			{
				/* bail out now if there's nothing to do */
				CStatus_Require((tt != CRegionType_Empty), CStatus_OK);
				CStatus_Require((ot != CRegionType_Empty), CStatus_OK);

				/* form the empty region, as needed */
				if(ot == CRegionType_Infinite)
				{
					return CRegion_MakeEmpty(_this);
				}

				/* form the exclusion */
				_OperationRegion(_this, Exclude, other);
			}
			break;
			case CCombineMode_Complement:
			{
				/* replace, as needed */
				if(tt == CRegionType_Empty) { goto _Replace; }

				/* form the empty region, as needed */
				if(tt == CRegionType_Infinite || ot == CRegionType_Empty)
				{
					return CRegion_MakeEmpty(_this);
				}

				/* form the complement */
				_OperationRegion(_this, Complement, other);
			}
			break;
			case CCombineMode_Replace:
			default:
			{
				/* provide local jump target */
				_Replace:

				/* add new region */
				{
					/* declarations */
					CRegionNode *data;

					/* copy the other tree */
					CStatus_Check
						(CRegionNode_Clone
							(other->head, &data));

					/* dispose of current region data */
					CRegion_Finalize(_this);

					/* reset the head node */
					_this->head = (CRegionNode *)data;
				}
			}
			break;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Determine if two regions are equal after applying a transformation. */
CStatus
CRegion_Equals(CRegion   *_this,
               CRegion   *other,
               CGraphics *graphics,
               CBool     *eq)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Get the bounds of this region on a particular graphics context. */
CStatus
CRegion_GetBounds(CRegion     *_this,
                  CGraphics   *graphics,
                  CRectangleF *bounds)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Get the raw region data for this region. */
CStatus
CRegion_GetData(CRegion  *_this,
                CByte   **data,
                CUInt32  *count)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Get an array of rectangles which represents this region. */
CStatus
CRegion_GetRegionScans(CRegion      *_this,
                       CMatrix      *matrix,
                       CRectangleF **scans,
                       CUInt32      *count)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Determine if this region is empty on a particular graphics context. */
CStatus
CRegion_IsEmpty(CRegion   *_this,
                CGraphics *graphics,
                CBool     *empty)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Determine if this region is infinite on a particular graphics context. */
CStatus
CRegion_IsInfinite(CRegion   *_this,
                   CGraphics *graphics,
                   CBool     *infinite)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Determine if a point is contained within this region. */
CStatus
CRegion_IsVisiblePoint(CRegion   *_this,
                       CGraphics *graphics,
                       CPointF    point,
                       CBool     *visible)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Determine if any part of a rectangle is contained within this region. */
CStatus
CRegion_IsVisibleRectangle(CRegion     *_this,
                           CGraphics   *graphics,
                           CRectangleF  rectangle,
                           CBool       *visible)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Make this region empty. */
CStatus
CRegion_MakeEmpty(CRegion *_this)
{
	/* declarations */
	CRegionNode *data;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* create the empty node */
	if(!(CRegionNode_Alloc(data)))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the empty node */
	*data = CRegionNode_Empty;

	/* dispose of current region data */
	CRegion_Finalize(_this);

	/* reset the head node */
	_this->head = data;

	/* return successfully */
	return CStatus_OK;
}

/* Make this region infinite. */
CStatus
CRegion_MakeInfinite(CRegion *_this)
{
	/* declarations */
	CRegionNode *data;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* create the infinite node */
	if(!(CRegionNode_Alloc(data)))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the infinite node */
	*data = CRegionNode_Infinite;

	/* dispose of current region data */
	CRegion_Finalize(_this);

	/* reset the head node */
	_this->head = data;

	/* return successfully */
	return CStatus_OK;
}

/* Transform this region by a matrix. */
CStatus
CRegion_Transform(CRegion *_this,
                  CMatrix *matrix)
{
	/* declarations */
	CRegionTransformer  transformer;
	CAffineTransformF   t;
	CStatus             status;
	void               *data;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the matrix transformation */
	CStatus_Check
		(CMatrix_GetTransform
			(matrix, &t));

	/* bail out now if there's nothing to do */
	CStatus_Require
		((!CAffineTransformF_Equals(&t, &CAffineTransformF_Identity)),
		 CStatus_OK);

	/* initialize the transformer */
	CRegionTransformer_Initialize(&transformer, &t);

	/* transform the region */
	status =
		CRegionInterpreter_Interpret
			(((CRegionInterpreter *)(&transformer)), _this->head, &data);

	/* finalize the transformer */
	CRegionTransformer_Finalize(&transformer);

	/* handle change event */
	CRegion_OnChange((_this));

	/* return status */
	return status;
}

/* Translate this region by a specific amount. */
CStatus
CRegion_Translate(CRegion *_this,
                  CFloat   dx,
                  CFloat   dy)
{
	/* declarations */
	CRegionTranslator  translator;
	CStatus            status;
	void              *data;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* bail out now if there's nothing to do */
	CStatus_Require(((dx != 0.0f) || (dy != 0.0f)), CStatus_OK);

	/* initialize the translator */
	CRegionTranslator_Initialize(&translator, dx, dy);

	/* translate the region */
	status =
		CRegionInterpreter_Interpret
			(((CRegionInterpreter *)(&translator)), _this->head, &data);

	/* finalize the translator */
	CRegionTranslator_Finalize(&translator);

	/* handle change event */
	CRegion_OnChange((_this));

	/* return status */
	return status;
}

/* Get the mask for this region. */
CINTERNAL CStatus
CRegion_GetMask(CRegion           *_this,
                CAffineTransformF *transform,
                pixman_image_t    *mask)
{
	/* declarations */
	CUInt32 w;
	CUInt32 h;
	CBool   g;

	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));
	CASSERT((mask      != 0));

	/* get the mask image information */
	w = pixman_image_get_width(mask);
	h = pixman_image_get_height(mask);
	g = (pixman_image_get_depth(mask) != 1);

	/* ensure we have a mask for the given settings */
	CStatus_Check
		(CRegion_GenerateMask
			(_this, transform, w, h, g));

	/* copy the mask data */
	{
		/* declarations */
		CByte   *dst;
		CByte   *src;
		CUInt32  s;

		/* get the mask data */
		dst = (CByte *)pixman_image_get_data(mask);
		src = (CByte *)pixman_image_get_data(_this->mask.image);

		/* get the stride */
		s = pixman_image_get_stride(mask);

		/* copy the data */
		CMemCopy(dst, src, (h * s));
	}

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
