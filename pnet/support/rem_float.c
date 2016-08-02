/*
 * rem_float.c - Compute the floating point remainder of two values.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#include "il_values.h"
#ifdef HAVE_MATH_H
#include <math.h>
#endif
#ifdef IL_CONFIG_FP_SUPPORTED

#ifdef	__cplusplus
extern	"C" {
#endif

ILNativeFloat ILNativeFloatRem(ILNativeFloat value1, ILNativeFloat value2)
{
#ifdef HAVE_FMOD
	return fmod(value1, value2);
#else
	/* We can't compute an answer, so default to NaN */
	return (ILNativeFloat)(0.0 / 0.0);
#endif
}

#ifdef	__cplusplus
};
#endif
#endif /* IL_CONFIG_FP_SUPPORTED */
