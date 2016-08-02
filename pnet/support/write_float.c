/*
 * write_float.c - Write floating point values to little-endian buffers.
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

void _ILWriteFloat(unsigned char *buf, ILFloat value)
{
	union
	{
		unsigned char bytes[4];
		ILFloat       value;

	} convert;
	convert.value = (ILFloat)1.0;
	if(convert.bytes[3] == (unsigned char)0x3F)
	{
		/* Little-endian host CPU, so retain the byte order */
		convert.value = value;
		buf[0] = convert.bytes[0];
		buf[1] = convert.bytes[1];
		buf[2] = convert.bytes[2];
		buf[3] = convert.bytes[3];
	}
	else
	{
		/* Big-endian host CPU, so flip the bytes */
		convert.value = value;
		buf[3] = convert.bytes[0];
		buf[2] = convert.bytes[1];
		buf[1] = convert.bytes[2];
		buf[0] = convert.bytes[3];
	}
}

void _ILWriteDouble(unsigned char *buf, ILDouble value)
{
	union
	{
		unsigned char bytes[8];
		ILDouble      value;
		struct {
			ILUInt32  first;
			ILUInt32  second;
		} words;

	} convert;
	convert.value = (ILDouble)1.0;
	if(convert.words.first == 0 && convert.words.second == 0x3FF00000)
	{
		/* Little-endian host CPU, so retain the byte order */
		convert.value = value;
		buf[0] = convert.bytes[0];
		buf[1] = convert.bytes[1];
		buf[2] = convert.bytes[2];
		buf[3] = convert.bytes[3];
		buf[4] = convert.bytes[4];
		buf[5] = convert.bytes[5];
		buf[6] = convert.bytes[6];
		buf[7] = convert.bytes[7];
	}
	else if(convert.bytes[3] != 0x3F)
	{
		/* Big-endian host CPU, so flip the bytes */
		convert.value = value;
		buf[7] = convert.bytes[0];
		buf[6] = convert.bytes[1];
		buf[5] = convert.bytes[2];
		buf[4] = convert.bytes[3];
		buf[3] = convert.bytes[4];
		buf[2] = convert.bytes[5];
		buf[1] = convert.bytes[6];
		buf[0] = convert.bytes[7];
	}
	else
	{
		/* Mixed-endian host CPU (e.g. ARM) */
		convert.value = value;
		buf[4] = convert.bytes[0];
		buf[5] = convert.bytes[1];
		buf[6] = convert.bytes[2];
		buf[7] = convert.bytes[3];
		buf[0] = convert.bytes[4];
		buf[1] = convert.bytes[5];
		buf[2] = convert.bytes[6];
		buf[3] = convert.bytes[7];
	}
}

#ifdef	__cplusplus
};
#endif
#endif /* IL_CONFIG_FP_SUPPORTED */
