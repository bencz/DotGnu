/*
 * interlocked.c - Implementation of interlocked functions.
 *
 * Copyright (C) 2010  Southern Storm Software, Pty Ltd.
 *
 * Authors: Klaus Treichel (ktreichel@web.de)
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

#include "thr_defs.h"
#include "interlocked.h"

/*
 * The number of critical sections to be used or protecting interlocked
 * operations.
 * The number MUST be a power of 2
 */
#define _INTERLOCKED_NUM_LOCKS	16

/*
 * The static array of critical sections to be used
 */
static _ILCriticalSection _locks[_INTERLOCKED_NUM_LOCKS];

/*
 * Calculate the lock to be used from a pointer.
 */
#define _LOCK_HASH(dest) \
	((((ILNativeInt)(dest)) ^ (((ILNativeInt)(dest)) >> 12)) & \
	(_INTERLOCKED_NUM_LOCKS - 1))

#define _LOCK(dest) \
	_ILCriticalSection *_lock; \
	_lock = &(_locks[_LOCK_HASH(dest)]); \
	_ILCriticalSectionEnter(_lock);

#define _UNLOCK() \
	_ILCriticalSectionLeave(_lock);

/*
 * Initialize the interlocked system.
 */
void ILInterlockedInit(void)
{
	int i;

	for(i = 0; i < _INTERLOCKED_NUM_LOCKS; i++)
	{
		_ILCriticalSectionCreate(&(_locks[i]));
	}
}

/*
 * Flush cache and set a memory barrier.
 */
void _ILInterlockedMemoryBarrier()
{
	void *temp;
	_LOCK(&temp);
	_UNLOCK();
}

#if !defined(IL_NATIVE_INT64)
/*
 * Load a signed 64 bit value from a location.
 */
ILInt64 _ILInterlockedLoadI8_Full(const volatile ILInt64 *dest)
{
	ILInt64 result;

	_LOCK(dest);

	result = *dest;

	_UNLOCK();

	return result;
}

/*
 * Load an unsigned 64 bit value from a location.
 */
ILUInt64 _ILInterlockedLoadU8_Full(const volatile ILUInt64 *dest)
{
	ILUInt64 result;

	_LOCK(dest);

	result = *dest;

	_UNLOCK();

	return result;
}

/*
 * Load a double precision floatingpoint value from a location.
 */
ILDouble _ILInterlockedLoadR8_Full(const volatile ILDouble *dest)
{
	ILDouble result;

	_LOCK(dest);

	result = *dest;

	_UNLOCK();

	return result;
}

/*
 * Store a signed 64 bit value to a location.
 */
void _ILInterlockedStoreI8_Full(volatile ILInt64 *dest, ILInt64 value)
{
	_LOCK(dest);

	*dest = value;

	_UNLOCK();
}

/*
 * Store an unsigned 64 bit value to a location.
 */
void _ILInterlockedStoreU8_Full(volatile ILUInt64 *dest, ILUInt64 value)
{
	_LOCK(dest);

	*dest = value;

	_UNLOCK();
}

/*
 * Store a double precision floatingpoint value to a location.
 */
void _ILInterlockedStoreR8_Full(volatile ILDouble *dest, ILDouble value)
{
	_LOCK(dest);

	*dest = value;

	_UNLOCK();
}
#endif /* !defined(IL_NATIVE_INT64) */

/*
 * Exchange signed 32 bit integers.
 */
ILInt32 _ILInterlockedExchangeI4_Full(volatile ILInt32 *dest, ILInt32 value)
{
	ILInt32 retval;

	_LOCK(dest);

	retval = *dest;
	*dest = value;

	_UNLOCK();

	return retval;
}

/*
 * Exchange unsigned 32 bit integers.
 */
ILUInt32 _ILInterlockedExchangeU4_Full(volatile ILUInt32 *dest, ILUInt32 value)
{
	ILUInt32 retval;

	_LOCK(dest);

	retval = *dest;
	*dest = value;

	_UNLOCK();

	return retval;
}

/*
 * Exchange signed 64 bit integers.
 */
ILInt64 _ILInterlockedExchangeI8_Full(volatile ILInt64 *dest, ILInt64 value)
{
	ILInt64 retval;

	_LOCK(dest);

	retval = *dest;
	*dest = value;

	_UNLOCK();

	return retval;
}

/*
 * Exchange unsigned 64 bit integers.
 */
ILUInt64 _ILInterlockedExchangeU8_Full(volatile ILUInt64 *dest, ILUInt64 value)
{
	ILUInt64 retval;

	_LOCK(dest);

	retval = *dest;
	*dest = value;

	_UNLOCK();

	return retval;
}

/*
 * Exchange pointers.
 */
void *_ILInterlockedExchangeP_Full(void * volatile *dest, void *value)
{
	void *retval;

	_LOCK(dest);

	retval = *dest;
	*dest = value;

	_UNLOCK();

	return retval;
}

/*
 * Exchange 32 bit foatingpoint values.
 */
ILFloat _ILInterlockedExchangeR4_Full(volatile ILFloat *dest, ILFloat value)
{
	ILFloat retval;

	_LOCK(dest);

	retval = *dest;
	*dest = value;

	_UNLOCK();

	return retval;
}

/*
 * Exchange 64 bit foatingpoint values.
 */
ILDouble _ILInterlockedExchangeR8_Full(volatile ILDouble *dest, ILDouble value)
{
	ILDouble retval;

	_LOCK(dest);

	retval = *dest;
	*dest = value;

	_UNLOCK();

	return retval;
}

/*
 * Compare and exchange signed 32 bit integers.
 */
ILInt32 _ILInterlockedCompareAndExchangeI4_Full(volatile ILInt32 *dest,
												ILInt32 value,
												ILInt32 comparand)
{
	ILInt32 retval;

	_LOCK(dest);

	retval = *dest;

	if (retval == comparand)
	{
		*dest = value;
	}

	_UNLOCK();

	return retval;
}

/*
 * Compare and exchange unsigned 32 bit integers.
 */
ILUInt32 _ILInterlockedCompareAndExchangeU4_Full(volatile ILUInt32 *dest,
												 ILUInt32 value,
												 ILUInt32 comparand)
{
	ILUInt32 retval;

	_LOCK(dest);

	retval = *dest;

	if(retval == comparand)
	{
		*dest = value;
	}

	_UNLOCK();

	return retval;
}

/*
 * Compare and exchange signed 64 bit integers.
 */
ILInt64 _ILInterlockedCompareAndExchangeI8_Full(volatile ILInt64 *dest,
												ILInt64 value,
												ILInt64 comparand)
{
	ILInt64 retval;

	_LOCK(dest);

	retval = *dest;

	if (retval == comparand)
	{
		*dest = value;
	}

	_UNLOCK();

	return retval;
}

/*
 * Compare and exchange unsigned 64 bit integers.
 */
ILUInt64 _ILInterlockedCompareAndExchangeU8_Full(volatile ILUInt64 *dest,
												 ILUInt64 value,
												 ILUInt64 comparand)
{
	ILUInt64 retval;

	_LOCK(dest);

	retval = *dest;

	if(retval == comparand)
	{
		*dest = value;
	}

	_UNLOCK();

	return retval;
}

/*
 * Compare and exchange pointers.
 */
void *_ILInterlockedCompareAndExchangeP_Full(void * volatile *dest,
											 void *value,
											 void *comparand)
{
	void *retval;

	_LOCK(dest);

	retval = (void *)*dest;

	if(retval == comparand)
	{
		*dest = value;
	}

	_UNLOCK();

	return retval;
}

/*
 * Compare and exchange 32 bit floatingpoint values.
 */
ILFloat _ILInterlockedCompareAndExchangeR4_Full(volatile ILFloat *dest,
												ILFloat value,
												ILFloat comparand)
{
	ILFloat retval;

	_LOCK(dest);

	retval = *dest;

	if(retval == comparand)
	{
		*dest = value;
	}

	_UNLOCK();

	return retval;
}

/*
 * Compare and exchange 64 bit floatingpoint values.
 */
ILDouble _ILInterlockedCompareAndExchangeR8_Full(volatile ILDouble *dest,
												 ILDouble value,
												 ILDouble comparand)
{
	ILDouble retval;

	_LOCK(dest);

	retval = *dest;

	if(retval == comparand)
	{
		*dest = value;
	}

	_UNLOCK();

	return retval;
}

/*
 * Add two signed 32 bit integers.
 */
ILInt32 _ILInterlockedAddI4_Full(volatile ILInt32 *dest, ILInt32 value)
{
	ILInt32 retval;

	_LOCK(dest);

	retval = *dest + value;
	*dest = retval;

	_UNLOCK();

	return retval;
}

/*
 * Add two signed 64 bit integers.
 */
ILInt64 _ILInterlockedAddI8_Full(volatile ILInt64 *dest, ILInt64 value)
{
	ILInt64 retval;

	_LOCK(dest);

	retval = *dest + value;
	*dest = retval;

	_UNLOCK();

	return retval;
}

/*
 * Bitwise and two unsigned 32 bit integers.
 */
void _ILInterlockedAndU4_Full(volatile ILUInt32 *dest, ILUInt32 value)
{
	_LOCK(dest);

	*dest &= value;

	_UNLOCK();
}

/*
 * Bitwise and two unsigned 64 bit integers.
 */
void _ILInterlockedAndU8_Full(volatile ILUInt64 *dest, ILUInt64 value)
{
	_LOCK(dest);

	*dest &= value;

	_UNLOCK();
}

/*
 * Bitwise or two unsigned 32 bit integers.
 */
void _ILInterlockedOrU4_Full(volatile ILUInt32 *dest, ILUInt32 value)
{
	_LOCK(dest);

	*dest |= value;

	_UNLOCK();
}

/*
 * Bitwise or two unsigned 64 bit integers.
 */
void _ILInterlockedOrU8_Full(volatile ILUInt64 *dest, ILUInt64 value)
{
	_LOCK(dest);

	*dest |= value;

	_UNLOCK();
}
