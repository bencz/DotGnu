/*
 * jitc_gen.h - Helper macros for JIT code generation.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#ifndef	_ENGINE_JITC_GEN_H
#define	_ENGINE_JITC_GEN_H

#ifdef	__cplusplus
extern	"C" {
#endif

#define _JITC_GET_INT8(arg, value) \
	do { \
		unsigned char *__temp = (unsigned char *)(&(value)); \
		__temp[0] = arg[0]; \
	} while(0)

#define _JITC_GET_INT16(arg, value) \
	do { \
		unsigned char *__temp = (unsigned char *)(&(value)); \
		__temp[0] = arg[0]; \
		__temp[1] = arg[1]; \
	} while(0)

#define _JITC_GET_INT32(arg, value) \
	do { \
		unsigned char *__temp = (unsigned char *)(&(value)); \
		__temp[0] = arg[0]; \
		__temp[1] = arg[1]; \
		__temp[2] = arg[2]; \
		__temp[3] = arg[3]; \
	} while(0)

#define _JITC_GET_INT64(arg, value) \
	do { \
		unsigned char *__temp = (unsigned char *)(&(value)); \
		__temp[0] = arg[0]; \
		__temp[1] = arg[1]; \
		__temp[2] = arg[2]; \
		__temp[3] = arg[3]; \
		__temp[4] = arg[4]; \
		__temp[5] = arg[5]; \
		__temp[6] = arg[6]; \
		__temp[7] = arg[7]; \
	} while(0)

#define _JITC_GET_FLOAT32(arg, value) \
	do { \
		unsigned char *__temp = (unsigned char *)(&(value)); \
		__temp[0] = arg[0]; \
		__temp[1] = arg[1]; \
		__temp[2] = arg[2]; \
		__temp[3] = arg[3]; \
	} while(0)

#define _JITC_GET_FLOAT64(arg, value) \
	do { \
		unsigned char *__temp = (unsigned char *)(&(value)); \
		__temp[0] = arg[0]; \
		__temp[1] = arg[1]; \
		__temp[2] = arg[2]; \
		__temp[3] = arg[3]; \
		__temp[4] = arg[4]; \
		__temp[5] = arg[5]; \
		__temp[6] = arg[6]; \
		__temp[7] = arg[7]; \
	} while(0)

#define JITC_GET_INT8(arg, value) _JITC_GET_INT8((arg), (value))
#define JITC_GET_INT16(arg, value) _JITC_GET_INT16((arg), (value))
#define JITC_GET_INT32(arg, value) _JITC_GET_INT32((arg), (value))
#define JITC_GET_INT64(arg, value) _JITC_GET_INT64((arg), (value))
#define JITC_GET_FLOAT32(arg, value) _JITC_GET_FLOAT32((arg), (value))
#define JITC_GET_FLOAT64(arg, value) _JITC_GET_FLOAT64((arg), (value))

#ifdef	__cplusplus
};
#endif

#endif	/* _ENGINE_JITC_GEN_H */
