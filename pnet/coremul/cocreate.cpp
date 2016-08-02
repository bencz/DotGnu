/*
 * cocreate.cpp - Create COM object instances.
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

#include "corwin32.h"

/*
 * If the platform is Win32, then we don't need to implement this.
 */
#ifndef _COR_REAL_WIN32

EXTERN_C STDMETHODIMP CoCreateInstance
			(const CLSID *rclsid, const IID *riid, void **ppv)
{
	// TODO
	return RESULT(CLASS_E_CLASSNOTAVAILABLE);
}

#endif /* !_COR_REAL_WIN32 */
