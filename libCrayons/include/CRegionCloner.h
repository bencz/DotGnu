/*
 * CRegionCloner.h - Region cloner header.
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

#ifndef _C_REGIONCLONER_H_
#define _C_REGIONCLONER_H_

#include "CRegionInterpreter.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCRegionCloner CRegionCloner;
struct _tagCRegionCloner
{
	CRegionInterpreter _base;
};

CINTERNAL void
CRegionCloner_Initialize(CRegionCloner *_this);
CINTERNAL void
CRegionCloner_Finalize(CRegionCloner *_this);

#ifdef __cplusplus
};
#endif

#endif /* _C_REGIONCLONER_H_ */
