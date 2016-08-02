/*
 * CMath.h - Math header.
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

#ifndef _C_MATH_H_
#define _C_MATH_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

CINTERNAL CMATH CDouble
CMath_Cos(CDouble radians);
CINTERNAL CMATH CDouble
CMath_Sin(CDouble radians);
CINTERNAL CMATH CDouble
CMath_Sqrt(CDouble value);
CINTERNAL CMATH CDouble
CMath_Erf(CDouble value);


#ifdef __cplusplus
};
#endif

#endif /* _C_MATH_H_ */
