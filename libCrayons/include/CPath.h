/*
 * CPath.h - Path header.
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

#ifndef _C_PATH_H_
#define _C_PATH_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

struct _tagCPath
{
	CUInt32    capacity;
	CUInt32    count;
	CPointF   *points;
	CUInt8    *types;
	CBitField  winding   : 1;
	CBitField  newFigure : 1;
	CBitField  hasCurves : 1;
};

#if 0
struct _tagCPathIterator
{
	CPath   *path;
	/* TODO */
};
#endif

/*\
|*| Ensure the capacity of point and type lists of a path.
|*|
|*|    path - the path
|*|   count - the total minimum capacity required
|*|
|*|  Returns status code on error.
\*/
#define _EnsurePathCapacity(path, minimum)                                     \
	do {                                                                       \
		/* reallocate the lists, as needed */                                  \
		if((minimum) > (path)->capacity)                                       \
		{                                                                      \
			/* declarations */                                                 \
			CPointF *tmpP;                                                    \
			CByte   *tmpT;                                                    \
			CUInt32  newSize;                                                 \
			CUInt32  newCapacity;                                             \
			                                                                   \
			/* get the capacity */                                             \
			const CUInt32 capacity = (path)->capacity;                        \
			                                                                   \
			/* calculate the new capacity */                                   \
			newCapacity = (((capacity + (minimum)) + 31) & ~31);               \
			                                                                   \
			/* calculate the optimal capacity, as needed */                    \
			if(capacity != 0)                                                  \
			{                                                                  \
				/* calculate a new capacity candidate */                       \
				const CUInt32 newCapacity2 = (capacity << 1);                 \
				                                                               \
				/* use the larger candidate capacity */                        \
				if(newCapacity < newCapacity2)                                 \
				{                                                              \
					newCapacity = newCapacity2;                                \
				}                                                              \
			}                                                                  \
			                                                                   \
			/* calculate the new points size */                                \
			newSize = (newCapacity * sizeof(CPointF));                        \
			                                                                   \
			/* create the new points list */                                   \
			if(!(tmpP = (CPointF *)CMalloc(newSize)))                        \
			{                                                                  \
				return CStatus_OutOfMemory;                                   \
			}                                                                  \
			                                                                   \
			/* calculate the new types size */                                 \
			newSize = (newCapacity * sizeof(CByte));                          \
			                                                                   \
			/* create the new types list */                                    \
			if(!(tmpT = (CByte *)CMalloc(newSize)))                          \
			{                                                                  \
				CFree(tmpP);                                                  \
				return CStatus_OutOfMemory;                                   \
			}                                                                  \
			                                                                   \
			/* copy existing data, as needed */                                \
			if((path)->count != 0)                                             \
			{                                                                  \
				/* copy the points and types */                                \
				CMemCopy                                                      \
					(tmpP, (path)->points,                                     \
					 ((path)->count * sizeof(CPointF)));                      \
				CMemCopy                                                      \
					(tmpT, (path)->types,                                      \
					 ((path)->count  * sizeof(CByte)));                       \
			}                                                                  \
			                                                                   \
			/* free existing lists, as needed */                               \
			if(capacity != 0)                                                  \
			{                                                                  \
				/* free the point and type lists */                            \
				CFree((path)->points);                                        \
				CFree((path)->types);                                         \
			}                                                                  \
			                                                                   \
			/* update the capacity */                                          \
			(path)->capacity = newCapacity;                                    \
			                                                                   \
			/* set the point and type lists */                                 \
			(path)->points = tmpP;                                             \
			(path)->types  = tmpT;                                             \
		}                                                                      \
	} while(0)

CINTERNAL void
CPath_TransformAffine(CPath             *_this,
                      CAffineTransformF *transform);
CINTERNAL CStatus
CPath_Stroke(CPath    *_this,
             CPath    *stroke,
             CStroker *stroker);
CINTERNAL CStatus
CPath_Fill(CPath       *_this,
           CTrapezoids *trapezoids);

#ifdef __cplusplus
};
#endif

#endif /* _C_PATH_H_ */
