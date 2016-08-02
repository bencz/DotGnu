/*
 * CRegionInterpreter.h - Region interpreter header.
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

#ifndef _C_REGIONINTERPRETER_H_
#define _C_REGIONINTERPRETER_H_

#include "CRegionStack.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCRegionInterpreter CRegionInterpreter;
typedef struct _tagCRegionInterpreterClass CRegionInterpreterClass;

struct _tagCRegionInterpreter
{
	const CRegionInterpreterClass *_class;
	CRegionStack                   stack;
};

struct _tagCRegionInterpreterClass
{
	CStatus (*Data)(CRegionInterpreter  *_this,
	                CRegionNode         *node,
	                void               **data);
	CStatus (*Op)(CRegionInterpreter  *_this,
	              CRegionOp           *op,
	              void                *left,
	              void                *right,
	              void               **data);
	void (*FreeData)(void *data);
};

CINTERNAL void
CRegionInterpreter_Initialize(CRegionInterpreter            *_this,
                              const CRegionInterpreterClass *_class);
CINTERNAL void
CRegionInterpreter_Finalize(CRegionInterpreter *_this);
CINTERNAL CStatus
CRegionInterpreter_Interpret(CRegionInterpreter  *_this,
                             CRegionNode         *head,
                             void               **data);

#ifdef __cplusplus
};
#endif

#endif /* _C_REGIONINTERPRETER_H_ */
