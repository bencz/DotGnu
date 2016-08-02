/*
 * CMath.c - Math implementation.
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

#include "CMath.h"
#include <math.h>

#ifdef __cplusplus
extern "C" {
#endif

CINTERNAL CMATH CDouble
CMath_Cos(CDouble radians)
{
	return cos(radians);
}

CINTERNAL CMATH CDouble
CMath_Sin(CDouble radians)
{
	return sin(radians);
}

CINTERNAL CMATH CDouble
CMath_Sqrt(CDouble value)
{
	return sqrt(value);
}

CINTERNAL CMATH CDouble
CMath_Erf(CDouble value)
{
	return erf(value);
}


#ifdef __cplusplus
};
#endif
