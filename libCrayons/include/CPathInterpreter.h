/*
 * CPathInterpreter.h - Path interpreter header.
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

#ifndef _C_PATHINTERPRETER_H_
#define _C_PATHINTERPRETER_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCPathInterpreter      CPathInterpreter;
typedef struct _tagCPathInterpreterClass CPathInterpreterClass;


struct _tagCPathInterpreter
{
	const CPathInterpreterClass *_class;
};

struct _tagCPathInterpreterClass
{
	/*\
	|*| Move to the given coordinates.
	|*|
	|*|      x - the x coordinate
	|*|      y - the y coordinate
	|*|   type - the type of the point
	|*|
	|*|  Returns status code.
	\*/
	CStatus (*Move)(CPathInterpreter *_this,
	                CFloat            x,
	                CFloat            y,
                    CPathType         type);

	/*\
	|*| Line to the given coordinates.
	|*|
	|*|      x - the x coordinate
	|*|      y - the y coordinate
	|*|   type - the type of the point
	|*|
	|*|  Returns status code.
	\*/
	CStatus (*Line)(CPathInterpreter *_this,
	                CFloat            x,
	                CFloat            y,
                    CPathType         type);

	/*\
	|*| Curve to the given coordinates.
	|*|
	|*|     x1 - the first x coordinate
	|*|     y1 - the first y coordinate
	|*|     x2 - the second x coordinate
	|*|     y2 - the second y coordinate
	|*|     x3 - the third x coordinate
	|*|     y3 - the third y coordinate
	|*|   type - the type of the third point
	|*|
	|*|  Returns status code.
	\*/
	CStatus (*Curve)(CPathInterpreter *_this,
	                 CFloat            x1,
	                 CFloat            y1,
	                 CFloat            x2,
	                 CFloat            y2,
	                 CFloat            x3,
	                 CFloat            y3,
                     CPathType         type);

	/*\
	|*| Close the path.
	|*|
	|*|  Returns status code.
	\*/
	CStatus (*Close)(CPathInterpreter *_this);

	/*\
	|*| Sentinel string used to catch missing methods in class tables.
	\*/
	const char *sentinel;
};

CINTERNAL CStatus
CPathInterpreter_Interpret(CPathInterpreter *_this,
                           const CPointF    *points,
                           const CByte      *types,
                           CUInt32           count);

#define CPathInterpreter_Move(_this, x, y, t) \
	((_this)->_class->Move((_this), (x), (y), (t)))
#define CPathInterpreter_Line(_this, x, y, t) \
	((_this)->_class->Line((_this), (x), (y), (t)))
#define CPathInterpreter_Curve(_this, x1, y1, x2, y2, x3, y3, t) \
	((_this)->_class->Curve((_this), (x1), (y1), (x2), (y2), (x3), (y3), (t)))
#define CPathInterpreter_Close(_this) \
	((_this)->_class->Close((_this)))

#ifdef __cplusplus
};
#endif

#endif /* _C_PATHINTERPRETER_H_ */
