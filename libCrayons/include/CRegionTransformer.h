/*
 * CRegionTransformer.h - Region transformer header.
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

#ifndef _C_REGIONTRANSFORMER_H_
#define _C_REGIONTRANSFORMER_H_

#include "CRegionInterpreter.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCRegionTransformer CRegionTransformer;
struct _tagCRegionTransformer
{
	CRegionInterpreter  _base;
	CAffineTransformF  *transform;
};

CINTERNAL void
CRegionTransformer_Initialize(CRegionTransformer *_this,
                              CAffineTransformF  *transform);
CINTERNAL void
CRegionTransformer_Finalize(CRegionTransformer *_this);

#ifdef __cplusplus
};
#endif

#endif /* _C_REGIONTRANSFORMER_H_ */
