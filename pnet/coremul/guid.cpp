/*
 * guid.cpp - Define GUID's for the COR routines.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#define	INITGUID
#include "corole2.h"

#ifdef __cplusplus
extern "C" {
#endif

/*
 * If the platform is Win32, then we don't need IUnknown and IClassFactory.
 */
#ifndef _COR_REAL_WIN32

DEFINE_GUID(IID_IUnknown, 0x00000000, 0x0000, 0x0000, 0xC0,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

DEFINE_GUID(IID_IClassFactory, 0x00000001, 0x0000, 0x0000,
			0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

#endif /* !_COR_REAL_WIN32 */

#ifdef __cplusplus
};
#endif
