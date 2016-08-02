/*
 * method_cache.h - Method cache implementation.
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

#ifndef	_ENGINE_METHOD_CACHE_H
#define	_ENGINE_METHOD_CACHE_H

#include "il_values.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Opaque method cache type.
 */
typedef struct _tagILCache ILCache;

/*
 * Writing position within a cache.
 */
typedef struct
{
	ILCache		   *cache;			/* Cache this position is attached to */
	unsigned char  *ptr;			/* Current code pointer */
	unsigned char  *limit;			/* Limit of the current page */

} ILCachePosn;

/*
 * Create a method cache.  Returns NULL if out of memory.
 * If "limit" is non-zero, then it specifies the maximum
 * size of the cache in bytes.
 */
ILCache *_ILCacheCreate(long limit,unsigned long cachePageSize);

/*
 * Destroy a method cache.
 */
void _ILCacheDestroy(ILCache *cache);

/*
 * Determine if the cache is full.  The "posn" value should
 * be supplied while translating a method, or be NULL otherwise.
 */
int _ILCacheIsFull(ILCache *cache, ILCachePosn *posn);

/*
 * Start output of a method, returning a cache position.
 * The "align" value indicates the default alignment for
 * the start of the method.  The "method" value is a
 * cookie for referring to the method.  Returns the
 * method entry point, or NULL if the cache is full.
 */
void *_ILCacheStartMethod(ILCache *cache, ILCachePosn *posn,
					      int align, void *method);

/*
 * Return values for "_ILCacheEndMethod".
 */
#define	IL_CACHE_END_OK			0		/* Method is OK */
#define	IL_CACHE_END_RESTART	1		/* Restart is required */
#define	IL_CACHE_END_TOO_BIG	2		/* Method is too big for the cache */

/*
 * End output of a method.  Returns zero if a restart
 */
int _ILCacheEndMethod(ILCachePosn *posn);

/*
 * Allocate "size" bytes of storage in the method cache's
 * auxillary data area.  Returns NULL if insufficient space
 * to satisfy the request.  It may be possible to satisfy
 * the request after a restart.
 */
void *_ILCacheAlloc(ILCachePosn *posn, unsigned long size);

/*
 * Allocate "size" bytes of storage when we aren't currently
 * translating a method.
 */
void *_ILCacheAllocNoMethod(ILCache *cache, unsigned long size);

/*
 * Align the method code on a particular boundary if the
 * difference between the current position and the aligned
 * boundary is less than "diff".  The "nop" value is used
 * to pad unused bytes.
 */
void _ILCacheAlignMethod(ILCachePosn *posn, int align, int diff, int nop);

/*
 * Mark the current method position with a bytecode offset value.
 */
void _ILCacheMarkBytecode(ILCachePosn *posn, ILUInt32 offset);

/*
 * Change to a new exception region within the current method.
 * The cookie will typically be NULL if no exception region.
 */
void _ILCacheNewRegion(ILCachePosn *posn, void *cookie);

/*
 * Set the exception region cookie for the current region.
 */
void _ILCacheSetCookie(ILCachePosn *posn, void *cookie);

/*
 * Find the method that is associated with a particular
 * program counter.  Returns NULL if the PC is not associated
 * with a method within the cache.  The exception region
 * cookie is returned in "*cookie", if "cookie" is not NULL.
 */
void *_ILCacheGetMethod(ILCache *cache, void *pc, void **cookie);

/*
 * Get a list of all methods that are presently in the cache.
 * The list is terminated by a NULL, and must be free'd with
 * "ILFree".  Returns NULL if out of memory.
 */
void **_ILCacheGetMethodList(ILCache *cache);

/*
 * Get the native offset that is associated with a bytecode
 * offset within a method.  The value "start" indicates the
 * entry point for the method.  Returns IL_MAX_UINT32 if
 * the native offset could not be determined.
 */
ILUInt32 _ILCacheGetNative(ILCache *cache, void *start,
						   ILUInt32 offset, int exact);

/*
 * Get the bytecode offset that is associated with a native
 * offset within a method.  The value "start" indicates the
 * entry point for the method.  Returns IL_MAX_UINT32 if
 * the bytecode offset could not be determined.
 */
ILUInt32 _ILCacheGetBytecode(ILCache *cache, void *start,
							 ILUInt32 offset, int exact);

/*
 * Get the number of bytes currently in use in the method cache.
 */
unsigned long _ILCacheGetSize(ILCache *cache);

/*
 * Convert a return address into a program counter value
 * that can be used with "_ILCacheGetMethod".  Normally
 * return addresses point to the next instruction after
 * an instruction that falls within a method region.  This
 * macro corrects for the "off by 1" address.
 */
#define	ILCacheRetAddrToPC(addr)	\
			((void *)(((unsigned char *)(addr)) - 1))

/*
 * Output a single byte to the current method.
 */
#define	ILCacheByte(posn,value)	\
			do { \
				if((posn)->ptr < (posn)->limit) \
				{ \
					*(((posn)->ptr)++) = (unsigned char)(value); \
				} \
			} while (0)

/*
 * Output a 16-bit little-endian word to the current method.
 */
#define	ILCacheWord16(posn,value)	\
			do { \
				if(((posn)->ptr + 1) < (posn)->limit) \
				{ \
					IL_WRITE_INT16((posn)->ptr, (ILInt16)(value)); \
					(posn)->ptr += 2; \
				} \
				else \
				{ \
					(posn)->ptr = (posn)->limit; \
				} \
			} while (0)

/*
 * Output a 32-bit little-endian word to the current method.
 */
#define	ILCacheWord32(posn,value)	\
			do { \
				if(((posn)->ptr + 3) < (posn)->limit) \
				{ \
					IL_WRITE_INT32((posn)->ptr, (ILInt32)(value)); \
					(posn)->ptr += 4; \
				} \
				else \
				{ \
					(posn)->ptr = (posn)->limit; \
				} \
			} while (0)

/*
 * Output a 64-bit little-endian word to the current method.
 */
#define	ILCacheWord64(posn,value)	\
			do { \
				if(((posn)->ptr + 7) < (posn)->limit) \
				{ \
					IL_WRITE_INT64((posn)->ptr, (ILInt64)(value)); \
					(posn)->ptr += 8; \
				} \
				else \
				{ \
					(posn)->ptr = (posn)->limit; \
				} \
			} while (0)

/*
 * Get the output position within the current method.
 */
#define	ILCacheGetPosn(posn)	((posn)->ptr)

/*
 * Determine if there is sufficient space for N bytes in the current method.
 */
#define	ILCacheCheckForN(posn,n)	\
				(((posn)->ptr + (n)) <= (posn)->limit)

/*
 * Helper macros to make the API easier to use internally.
 */
#define	ILCacheCreate			_ILCacheCreate
#define	ILCacheDestroy			_ILCacheDestroy
#define	ILCacheIsFull			_ILCacheIsFull
#define	ILCacheStartMethod		_ILCacheStartMethod
#define	ILCacheEndMethod		_ILCacheEndMethod
#define	ILCacheAlloc			_ILCacheAlloc
#define	ILCacheAllocNoMethod	_ILCacheAllocNoMethod
#define	ILCacheAlignMethod		_ILCacheAlignMethod
#define	ILCacheMarkBytecode		_ILCacheMarkBytecode
#define	ILCacheNewRegion		_ILCacheNewRegion
#define	ILCacheSetCookie		_ILCacheSetCookie
#define	ILCacheGetMethod		_ILCacheGetMethod
#define	ILCacheGetNative		_ILCacheGetNative
#define	ILCacheGetBytecode		_ILCacheGetBytecode
#define	ILCacheGetMethodList	_ILCacheGetMethodList
#define	ILCacheGetSize			_ILCacheGetSize

#ifdef	__cplusplus
};
#endif

#endif	/* _ENGINE_METHOD_CACHE_H */
