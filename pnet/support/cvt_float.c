/*
 * cvt_float.c - Perform difficult numeric conversions.
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
#ifdef IL_CONFIG_FP_SUPPORTED

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Convert "ulong" into "native float".
 *
 * Some platforms cannot perform the conversion directly,
 * so we need to do it in stages.
 */
ILNativeFloat ILUInt64ToFloat(ILUInt64 value)
{
	if(value < (((ILUInt64)1) << 63))
	{
		return (ILNativeFloat)(ILInt64)value;
	}
	else
	{
		return (ILNativeFloat)((ILInt64)value) +
			   (ILNativeFloat)18446744073709551616.0;
	}
}

/*
 * Convert "native float" into "ulong".
 *
 * Some platforms cannot perform the conversion directly,
 * so we need to do it in stages.
 */
ILUInt64 ILFloatToUInt64(ILNativeFloat value)
{
	if(ILNativeFloatIsFinite(value))
	{
		if(value >= (ILNativeFloat)0.0)
		{
			if(value < (ILNativeFloat)9223372036854775808.0)
			{
				return (ILUInt64)(ILInt64)value;
			}
			else if(value < (ILNativeFloat)18446744073709551616.0)
			{
				ILInt64 temp = (ILInt64)(value - 9223372036854775808.0);
				return (ILUInt64)(temp - IL_MIN_INT64);
			}
			else
			{
				return IL_MAX_UINT64;
			}
		}
		else
		{
			return 0;
		}
	}
	else if(ILNativeFloatIsNaN(value))
	{
		return 0;
	}
	else if(value < (ILNativeFloat)0.0)
	{
		return 0;
	}
	else
	{
		return IL_MAX_UINT64;
	}
}

/*
 * Convert "native float" into "long" with overflow testing.
 */
int ILFloatToInt64Ovf(ILInt64 *result, ILNativeFloat value)
{
	if(ILNativeFloatIsFinite(value))
	{
		if(value >= (ILNativeFloat)-9223372036854775808.0 &&
		   value < (ILNativeFloat)9223372036854775808.0)
		{
			*result = (ILInt64)value;
			return 1;
		}
		else if(value < (ILNativeFloat)0.0)
		{
			/* Account for the range -9223372036854775809.0 to
			   -9223372036854775808.0, which may get rounded
			   off if we aren't careful */
			value += (ILNativeFloat)9223372036854775808.0;
			if(value > (ILNativeFloat)(-1.0))
			{
				*result = (ILInt64)IL_MIN_INT64;
				return 1;
			}
		}
	}
	return 0;
}

/*
 * Convert "native float" into "ulong" with overflow testing.
 */
int ILFloatToUInt64Ovf(ILUInt64 *result, ILNativeFloat value)
{
	if(ILNativeFloatIsFinite(value))
	{
		if(value >= (ILNativeFloat)0.0)
		{
			if(value < (ILNativeFloat)9223372036854775808.0)
			{
				*result = (ILUInt64)(ILInt64)value;
				return 1;
			}
			else if(value < (ILNativeFloat)18446744073709551616.0)
			{
				ILInt64 temp = (ILInt64)(value - 9223372036854775808.0);
				*result = (ILUInt64)(temp - IL_MIN_INT64);
				return 1;
			}
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
#endif /* IL_CONFIG_FP_SUPPORTED */
