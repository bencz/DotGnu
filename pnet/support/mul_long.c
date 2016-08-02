/*
 * mul_long.c - Multiply 64-bit values with overflow testing.
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

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Multiply two uint64 values and check for overflow.
 * This would be easier if there was a portable way
 * to multiply two 64-bit values to get a 128-bit
 * result.
 */
int ILUInt64MulOvf(ILUInt64 *product, ILUInt64 value1, ILUInt64 value2)
{
	ILUInt32 high1, low1, high2, low2, orig;
	ILUInt64 temp;
	ILUInt32 result1, result2, result3, result4;
	high1 = (ILUInt32)(value1 >> 32);
	low1  = (ILUInt32)value1;
	high2 = (ILUInt32)(value2 >> 32);
	low2  = (ILUInt32)value2;
	temp = ((ILUInt64)low1) * ((ILUInt64)low2);
	result1 = (ILUInt32)temp;
	result2 = (ILUInt32)(temp >> 32);
	temp = ((ILUInt64)low1) * ((ILUInt64)high2);
	orig = result2;
	result2 += (ILUInt32)temp;
	if(result2 < orig)
		result3 = (((ILUInt32)(temp >> 32)) + 1);
	else
		result3 = ((ILUInt32)(temp >> 32));
	temp = ((ILUInt64)high1) * ((ILUInt64)low2);
	orig = result2;
	result2 += (ILUInt32)temp;
	if(result2 < orig)
	{
		orig = result3;
		result3 += (((ILUInt32)(temp >> 32)) + 1);
		if(result3 < orig)
			result4 = 1;
		else
			result4 = 0;
	}
	else
	{
		orig = result3;
		result3 += ((ILUInt32)(temp >> 32));
		if(result3 < orig)
			result4 = 1;
		else
			result4 = 0;
	}
	temp = ((ILUInt64)high1) * ((ILUInt64)high2);
	orig = result3;
	result3 += (ILUInt32)temp;
	if(result3 < orig)
		result4 += ((ILUInt32)(temp >> 32)) + 1;
	else
		result4 += ((ILUInt32)(temp >> 32));
	if(result3 != 0 || result4 != 0)
	{
		return 0;
	}
	*product = (((ILUInt64)result2) << 32) | ((ILUInt64)result1);
	return 1;
}

/*
 * Multiply two int64 values and check for overflow.
 * This would be easier if there was a portable way
 * to multiply two 64-bit values to get a 128-bit
 * result.
 */
int ILInt64MulOvf(ILInt64 *product, ILInt64 value1, ILInt64 value2)
{
	ILUInt64 temp;
	if(value1 >= 0 && value2 >= 0)
	{
		/* Both values are positive */
		if(!ILUInt64MulOvf(&temp, (ILUInt64)value1, (ILUInt64)value2))
		{
			return 0;
		}
		if(temp > ((ILUInt64)IL_MAX_INT64))
		{
			return 0;
		}
		*product = (ILInt64)temp;
		return 1;
	}
	else if(value1 >= 0)
	{
		/* The first value is positive */
		if(!ILUInt64MulOvf(&temp, (ILUInt64)value1, (ILUInt64)-value2))
		{
			return 0;
		}
		if(temp > (((ILUInt64)IL_MAX_INT64) + 1))
		{
			return 0;
		}
		*product = -((ILInt64)temp);
		return 1;
	}
	else if(value2 >= 0)
	{
		/* The second value is positive */
		if(!ILUInt64MulOvf(&temp, (ILUInt64)-value1, (ILUInt64)value2))
		{
			return 0;
		}
		if(temp > (((ILUInt64)IL_MAX_INT64) + 1))
		{
			return 0;
		}
		*product = -((ILInt64)temp);
		return 1;
	}
	else
	{
		/* Both values are negative */
		if(!ILUInt64MulOvf(&temp, (ILUInt64)-value1, (ILUInt64)-value2))
		{
			return 0;
		}
		if(temp > ((ILUInt64)IL_MAX_INT64))
		{
			return 0;
		}
		*product = (ILInt64)temp;
		return 1;
	}
}

#ifdef	__cplusplus
};
#endif
