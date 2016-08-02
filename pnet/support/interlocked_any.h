/*
 * interlocked_any.h - Generic implementation of interlocked functions
 *
 * Copyright (C) 2002, 2009  Southern Storm Software, Pty Ltd.
 *
 * Authors: Thong Nguyen (tum@veridicus.com)
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

/*
 * Some helper types for conversions
 */
typedef union
{
	ILInt32		int32Value;
	ILFloat		floatValue;
} ILInterlockedConv4;

typedef union
{
	ILInt64		int64Value;
	ILDouble	doubleValue;
} ILInterlockedConv8;

#if !defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define ILInterlockedMemoryBarrier() _ILInterlockedMemoryBarrier()
#endif /* !defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */

/*
 * Load a signed 8 bit value from a location.
 */
static IL_INLINE ILInt8 _ILInterlockedLoadI1(const volatile ILInt8 *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI1)
#define ILInterlockedLoadI1(dest)	_ILInterlockedLoadI1((dest))
#define IL_HAVE_INTERLOCKED_LOADI1 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI1) */

static IL_INLINE ILInt8 _ILInterlockedLoadI1_Acquire(const volatile ILInt8 *dest)
{
	ILInt8 result;

	result = ILInterlockedLoadI1(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI1_ACQUIRE)
#define ILInterlockedLoadI1_Acquire(dest)	_ILInterlockedLoadI1_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI1_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI1_ACQUIRE) */

static IL_INLINE ILInt8 _ILInterlockedLoadI1_Release(const volatile ILInt8 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadI1(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI1_RELEASE)
#define ILInterlockedLoadI1_Release(dest)	_ILInterlockedLoadI1_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI1_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI1_RELEASE) */

static IL_INLINE ILInt8 _ILInterlockedLoadI1_Full(const volatile ILInt8 *dest)
{
	ILInt8 result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadI1(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI1_FULL)
#define ILInterlockedLoadI1_Full(dest)	_ILInterlockedLoadI1_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI1_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI1_FULL) */

/*
 * Load an unsigned 8 bit value from a location.
 */
static IL_INLINE ILUInt8 _ILInterlockedLoadU1(const volatile ILUInt8 *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU1)
#define ILInterlockedLoadU1(dest)	_ILInterlockedLoadU1((dest))
#define IL_HAVE_INTERLOCKED_LOADU1 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU1) */

static IL_INLINE ILUInt8 _ILInterlockedLoadU1_Acquire(const volatile ILUInt8 *dest)
{
	ILUInt8 result;

	result = ILInterlockedLoadU1(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU1_ACQUIRE)
#define ILInterlockedLoadU1_Acquire(dest)	_ILInterlockedLoadU1_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU1_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU1_ACQUIRE) */

static IL_INLINE ILUInt8 _ILInterlockedLoadU1_Release(const volatile ILUInt8 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadU1(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU1_RELEASE)
#define ILInterlockedLoadU1_Release(dest)	_ILInterlockedLoadU1_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU1_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU1_RELEASE) */

static IL_INLINE ILUInt8 _ILInterlockedLoadU1_Full(const volatile ILUInt8 *dest)
{
	ILUInt8 result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadU1(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU1_FULL)
#define ILInterlockedLoadU1_Full(dest)	_ILInterlockedLoadU1_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU1_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU1_FULL) */

/*
 * Load a signed 16 bit value from a location.
 */
static IL_INLINE ILInt16 _ILInterlockedLoadI2(const volatile ILInt16 *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI2)
#define ILInterlockedLoadI2(dest)	_ILInterlockedLoadI2((dest))
#define IL_HAVE_INTERLOCKED_LOADI2 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI2) */

static IL_INLINE ILInt16 _ILInterlockedLoadI2_Acquire(const volatile ILInt16 *dest)
{
	ILInt16 result;

	result = ILInterlockedLoadI2(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI2_ACQUIRE)
#define ILInterlockedLoadI2_Acquire(dest)	_ILInterlockedLoadI2_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI2_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI2_ACQUIRE) */

static IL_INLINE ILInt16 _ILInterlockedLoadI2_Release(const volatile ILInt16 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadI2(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI2_RELEASE)
#define ILInterlockedLoadI2_Release(dest)	_ILInterlockedLoadI2_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI2_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI2_RELEASE) */

static IL_INLINE ILInt16 _ILInterlockedLoadI2_Full(const volatile ILInt16 *dest)
{
	ILInt16 result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadI2(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI2_FULL)
#define ILInterlockedLoadI2_Full(dest)	_ILInterlockedLoadI2_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI2_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI2_FULL) */

/*
 * Load an unsigned 16 bit value from a location.
 */
static IL_INLINE ILUInt16 _ILInterlockedLoadU2(const volatile ILUInt16 *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU2)
#define ILInterlockedLoadU2(dest)	_ILInterlockedLoadU2((dest))
#define IL_HAVE_INTERLOCKED_LOADU2 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU2) */

static IL_INLINE ILUInt16 _ILInterlockedLoadU2_Acquire(const volatile ILUInt16 *dest)
{
	ILUInt16 result;

	result = ILInterlockedLoadU2(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU2_ACQUIRE)
#define ILInterlockedLoadU2_Acquire(dest)	_ILInterlockedLoadU2_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU2_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU2_ACQUIRE) */

static IL_INLINE ILUInt16 _ILInterlockedLoadU2_Release(const volatile ILUInt16 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadU2(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU2_RELEASE)
#define ILInterlockedLoadU2_Release(dest)	_ILInterlockedLoadU2_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU2_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU2_RELEASE) */

static IL_INLINE ILUInt16 _ILInterlockedLoadU2_Full(const volatile ILUInt16 *dest)
{
	ILUInt16 result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadU2(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU2_FULL)
#define ILInterlockedLoadU2_Full(dest)	_ILInterlockedLoadU2_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU2_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU2_FULL) */

/*
 * Load a signed 32 bit value from a location.
 */
static IL_INLINE ILInt32 _ILInterlockedLoadI4(const volatile ILInt32 *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI4)
#define ILInterlockedLoadI4(dest)	_ILInterlockedLoadI4((dest))
#define IL_HAVE_INTERLOCKED_LOADI4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI4) */

static IL_INLINE ILInt32 _ILInterlockedLoadI4_Acquire(const volatile ILInt32 *dest)
{
	ILInt32 result;

	result = ILInterlockedLoadI4(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI4_ACQUIRE)
#define ILInterlockedLoadI4_Acquire(dest)	_ILInterlockedLoadI4_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI4_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI4_ACQUIRE) */

static IL_INLINE ILInt32 _ILInterlockedLoadI4_Release(const volatile ILInt32 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadI4(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI4_RELEASE)
#define ILInterlockedLoadI4_Release(dest)	_ILInterlockedLoadI4_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI4_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI4_RELEASE) */

static IL_INLINE ILInt32 _ILInterlockedLoadI4_Full(const volatile ILInt32 *dest)
{
	ILInt32 result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadI4(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI4_FULL)
#define ILInterlockedLoadI4_Full(dest)	_ILInterlockedLoadI4_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI4_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU4_FULL) */

/*
 * Load an unsigned 32 bit value from a location.
 */
static IL_INLINE ILUInt32 _ILInterlockedLoadU4(const volatile ILUInt32 *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU4)
#define ILInterlockedLoadU4(dest)	_ILInterlockedLoadU4((dest))
#define IL_HAVE_INTERLOCKED_LOADU4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU4) */

static IL_INLINE ILUInt32 _ILInterlockedLoadU4_Acquire(const volatile ILUInt32 *dest)
{
	ILUInt32 result;

	result = ILInterlockedLoadU4(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU4_ACQUIRE)
#define ILInterlockedLoadU4_Acquire(dest)	_ILInterlockedLoadU4_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU4_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU4_ACQUIRE) */

static IL_INLINE ILUInt32 _ILInterlockedLoadU4_Release(const volatile ILUInt32 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadU4(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU4_RELEASE)
#define ILInterlockedLoadU4_Release(dest)	_ILInterlockedLoadU4_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU4_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU4_RELEASE) */

static IL_INLINE ILUInt32 _ILInterlockedLoadU4_Full(const volatile ILUInt32 *dest)
{
	ILUInt32 result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadU4(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU4_FULL)
#define ILInterlockedLoadU4_Full(dest)	_ILInterlockedLoadU4_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU4_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU4_FULL) */

/*
 * Load a pointer value from a location.
 */
static IL_INLINE void *_ILInterlockedLoadP(void * const volatile *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADP)
#define ILInterlockedLoadP(dest)	_ILInterlockedLoadP((dest))
#define IL_HAVE_INTERLOCKED_LOADP 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADP) */

static IL_INLINE void *_ILInterlockedLoadP_Acquire(void * const volatile *dest)
{
	void *result;

	result = ILInterlockedLoadP(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADP_ACQUIRE)
#define ILInterlockedLoadP_Acquire(dest)	_ILInterlockedLoadP_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADP_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADP_ACQUIRE) */

static IL_INLINE void *_ILInterlockedLoadP_Release(void * const volatile *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadP(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADP_RELEASE)
#define ILInterlockedLoadP_Release(dest)	_ILInterlockedLoadP_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADP_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADP_RELEASE) */

static IL_INLINE void *_ILInterlockedLoadP_Full(void * const volatile *dest)
{
	void *result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadP(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADP_FULL)
#define ILInterlockedLoadP_Full(dest)	_ILInterlockedLoadP_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADP_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADP_FULL) */

/*
 * Load a single precision floatingpoint value from a location.
 */
static IL_INLINE ILFloat _ILInterlockedLoadR4(const volatile ILFloat *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADR4)
#define ILInterlockedLoadR4(dest)	_ILInterlockedLoadR4((dest))
#define IL_HAVE_INTERLOCKED_LOADR4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR4) */

static IL_INLINE ILFloat _ILInterlockedLoadR4_Acquire(const volatile ILFloat *dest)
{
	ILFloat result;

	result = ILInterlockedLoadR4(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADR4_ACQUIRE)
#define ILInterlockedLoadR4_Acquire(dest)	_ILInterlockedLoadR4_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADR4_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR4_ACQUIRE) */

static IL_INLINE ILFloat _ILInterlockedLoadR4_Release(const volatile ILFloat *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadR4(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADR4_RELEASE)
#define ILInterlockedLoadR4_Release(dest)	_ILInterlockedLoadR4_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADR4_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR4_RELEASE) */

static IL_INLINE ILFloat _ILInterlockedLoadR4_Full(const volatile ILFloat *dest)
{
	ILFloat result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadR4(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADR4_FULL)
#define ILInterlockedLoadR4_Full(dest)	_ILInterlockedLoadR4_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADR4_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR4_FULL) */

#if defined(IL_NATIVE_INT64)
/*
 * Load a signed 64 bit value from a location.
 */
static IL_INLINE ILInt64 _ILInterlockedLoadI8(const volatile ILInt64 *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI8)
#define ILInterlockedLoadI8(dest)	_ILInterlockedLoadI8((dest))
#define IL_HAVE_INTERLOCKED_LOADI8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI8) */

static IL_INLINE ILInt64 _ILInterlockedLoadI8_Acquire(const volatile ILInt64 *dest)
{
	ILInt64 result;

	result = ILInterlockedLoadI8(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI8_ACQUIRE)
#define ILInterlockedLoadI8_Acquire(dest)	_ILInterlockedLoadI8_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI8_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI8_ACQUIRE) */

static IL_INLINE ILInt64 _ILInterlockedLoadI8_Release(const volatile ILInt64 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadI8(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI8_RELEASE)
#define ILInterlockedLoadI8_Release(dest)	_ILInterlockedLoadI8_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI8_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI8_RELEASE) */

static IL_INLINE ILInt64 _ILInterlockedLoadI8_Full(const volatile ILInt64 *dest)
{
	ILInt64 result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadI8(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADI8_FULL)
#define ILInterlockedLoadI8_Full(dest)	_ILInterlockedLoadI8_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADI8_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI8_FULL) */

/*
 * Load an unsigned 64 bit value from a location.
 */
static IL_INLINE ILUInt64 _ILInterlockedLoadU8(const volatile ILUInt64 *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU8)
#define ILInterlockedLoadU8(dest)	_ILInterlockedLoadU8((dest))
#define IL_HAVE_INTERLOCKED_LOADU8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU8) */

static IL_INLINE ILUInt64 _ILInterlockedLoadU8_Acquire(const volatile ILUInt64 *dest)
{
	ILUInt64 result;

	result = ILInterlockedLoadU8(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU8_ACQUIRE)
#define ILInterlockedLoadU8_Acquire(dest)	_ILInterlockedLoadU8_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU8_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU8_ACQUIRE) */

static IL_INLINE ILUInt64 _ILInterlockedLoadU8_Release(const volatile ILUInt64 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadU8(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU8_RELEASE)
#define ILInterlockedLoadU8_Release(dest)	_ILInterlockedLoadU8_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU8_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU8_RELEASE) */

static IL_INLINE ILUInt64 _ILInterlockedLoadU8_Full(const volatile ILUInt64 *dest)
{
	ILUInt64 result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadU8(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADU8_FULL)
#define ILInterlockedLoadU8_Full(dest)	_ILInterlockedLoadU8_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADU8_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU8_FULL) */

/*
 * Load a double precision floatingpoint value from a location.
 */
static IL_INLINE ILDouble _ILInterlockedLoadR8(const volatile ILDouble *dest)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * acquire semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the value is likely to be cached.
	 */
	return *dest;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADR8)
#define ILInterlockedLoadR8(dest)	_ILInterlockedLoadR8((dest))
#define IL_HAVE_INTERLOCKED_LOADR8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR8) */

static IL_INLINE ILDouble _ILInterlockedLoadR8_Acquire(const volatile ILDouble *dest)
{
	ILDouble result;

	result = ILInterlockedLoadR8(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADR8_ACQUIRE)
#define ILInterlockedLoadR8_Acquire(dest)	_ILInterlockedLoadR8_Acquire((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADR8_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR8_ACQUIRE) */

static IL_INLINE ILDouble _ILInterlockedLoadR8_Release(const volatile ILDouble *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedLoadR8(dest);
}
#if !defined(IL_HAVE_INTERLOCKED_LOADR8_RELEASE)
#define ILInterlockedLoadR8_Release(dest)	_ILInterlockedLoadR8_Release((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADR8_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR8_RELEASE) */

static IL_INLINE ILDouble _ILInterlockedLoadR8_Full(const volatile ILDouble *dest)
{
	ILDouble result;

	ILInterlockedMemoryBarrier();
	result = ILInterlockedLoadR8(dest);
	ILInterlockedMemoryBarrier();
	return result;
}
#if !defined(IL_HAVE_INTERLOCKED_LOADR8_FULL)
#define ILInterlockedLoadR8_Full(dest)	_ILInterlockedLoadR8_Full((dest))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_LOADR8_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR8_FULL) */
#else /* !defined(IL_NATIVE_INT64) */
/*
 * Load a signed 64 bit value from a location.
 */
ILInt64 _ILInterlockedLoadI8_Full(const volatile ILInt64 *dest);

/*
 * Load an unsigned 64 bit value from a location.
 */
ILUInt64 _ILInterlockedLoadU8_Full(const volatile ILUInt64 *dest);

/*
 * Load a double precision floatingpoint value from a location.
 */
ILDouble _ILInterlockedLoadR8_Full(const volatile ILDouble *dest);

#if !defined(IL_HAVE_INTERLOCKED_LOADI8)
#define ILInterlockedLoadI8(dest)	_ILInterlockedLoadI8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI8) */
#if !defined(IL_HAVE_INTERLOCKED_LOADI8_ACQUIRE)
#define ILInterlockedLoadI8_Acquire(dest)	_ILInterlockedLoadI8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_LOADI8_RELEASE)
#define ILInterlockedLoadI8_Release(dest)	_ILInterlockedLoadI8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_LOADI8_FULL)
#define ILInterlockedLoadI8_Full(dest)	_ILInterlockedLoadI8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADI8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_LOADU8)
#define ILInterlockedLoadU8(dest)	_ILInterlockedLoadU8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU8) */
#if !defined(IL_HAVE_INTERLOCKED_LOADU8_ACQUIRE)
#define ILInterlockedLoadU8_Acquire(dest)	_ILInterlockedLoadU8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_LOADU8_RELEASE)
#define ILInterlockedLoadU8_Release(dest)	_ILInterlockedLoadU8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_LOADU8_FULL)
#define ILInterlockedLoadU8_Full(dest)	_ILInterlockedLoadU8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADU8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_LOADR8)
#define ILInterlockedLoadR8(dest)	_ILInterlockedLoadR8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR8) */
#if !defined(IL_HAVE_INTERLOCKED_LOADR8_ACQUIRE)
#define ILInterlockedLoadR8_Acquire(dest)	_ILInterlockedLoadR8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_LOADR8_RELEASE)
#define ILInterlockedLoadR8_Release(dest)	_ILInterlockedLoadR8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_LOADR8_FULL)
#define ILInterlockedLoadR8_Full(dest)	_ILInterlockedLoadR8_Full((dest))
#endif /* !defined(IL_HAVE_INTERLOCKED_LOADR8_FULL) */
#endif /* !defined(IL_NATIVE_INT64) */

/*
 * Store a signed 8 bit value to a location.
 */
static IL_INLINE void _ILInterlockedStoreI1(volatile ILInt8 *dest,
											ILInt8 value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI1)
#define ILInterlockedStoreI1(dest, value)	_ILInterlockedStoreI1((dest), (value))
#define IL_HAVE_INTERLOCKED_STOREI1 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI1) */

static IL_INLINE void _ILInterlockedStoreI1_Acquire(volatile ILInt8 *dest,
													ILInt8 value)
{
	ILInterlockedStoreI1(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI1_ACQUIRE)
#define ILInterlockedStoreI1_Acquire(dest, value) \
		_ILInterlockedStoreI1_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI1_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI1_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreI1_Release(volatile ILInt8 *dest,
													ILInt8 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreI1(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI1_RELEASE)
#define ILInterlockedStoreI1_Release(dest, value) \
		_ILInterlockedStoreI1_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI1_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI1_RELEASE) */

static IL_INLINE void _ILInterlockedStoreI1_Full(volatile ILInt8 *dest,
													ILInt8 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreI1(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI1_FULL)
#define ILInterlockedStoreI1_Full(dest, value) \
		_ILInterlockedStoreI1_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI1_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI1_RELEASE) */

/*
 * Store an unsigned 8 bit value to a location.
 */
static IL_INLINE void _ILInterlockedStoreU1(volatile ILUInt8 *dest,
											ILUInt8 value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU1)
#define ILInterlockedStoreU1(dest, value)	_ILInterlockedStoreU1((dest), (value))
#define IL_HAVE_INTERLOCKED_STOREU1 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU1) */

static IL_INLINE void _ILInterlockedStoreU1_Acquire(volatile ILUInt8 *dest,
													ILUInt8 value)
{
	ILInterlockedStoreU1(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU1_ACQUIRE)
#define ILInterlockedStoreU1_Acquire(dest, value) \
		_ILInterlockedStoreU1_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU1_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU1_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreU1_Release(volatile ILUInt8 *dest,
													ILUInt8 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreU1(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU1_RELEASE)
#define ILInterlockedStoreU1_Release(dest, value) \
		_ILInterlockedStoreU1_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU1_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU1_RELEASE) */

static IL_INLINE void _ILInterlockedStoreU1_Full(volatile ILUInt8 *dest,
													ILUInt8 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreU1(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU1_FULL)
#define ILInterlockedStoreU1_Full(dest, value) \
		_ILInterlockedStoreU1_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU1_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU1_FULL) */

/*
 * Store a signed 16 bit value to a location.
 */
static IL_INLINE void _ILInterlockedStoreI2(volatile ILInt16 *dest,
											ILInt16 value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI2)
#define ILInterlockedStoreI2(dest, value)	_ILInterlockedStoreI2((dest), (value))
#define IL_HAVE_INTERLOCKED_STOREI2 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI2) */

static IL_INLINE void _ILInterlockedStoreI2_Acquire(volatile ILInt16 *dest,
													ILInt16 value)
{
	ILInterlockedStoreI2(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI2_ACQUIRE)
#define ILInterlockedStoreI2_Acquire(dest, value) \
		_ILInterlockedStoreI2_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI2_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI2_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreI2_Release(volatile ILInt16 *dest,
													ILInt16 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreI2(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI2_RELEASE)
#define ILInterlockedStoreI2_Release(dest, value) \
		_ILInterlockedStoreI2_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI2_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI2_RELEASE) */

static IL_INLINE void _ILInterlockedStoreI2_Full(volatile ILInt16 *dest,
													ILInt16 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreI2(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI2_FULL)
#define ILInterlockedStoreI2_Full(dest, value) \
		_ILInterlockedStoreI2_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI2_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI2_FULL) */

/*
 * Store an unsigned 16 bit value to a location.
 */
static IL_INLINE void _ILInterlockedStoreU2(volatile ILUInt16 *dest,
											ILUInt16 value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU2)
#define ILInterlockedStoreU2(dest, value)	_ILInterlockedStoreU2((dest), (value))
#define IL_HAVE_INTERLOCKED_STOREU2 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU2) */

static IL_INLINE void _ILInterlockedStoreU2_Acquire(volatile ILUInt16 *dest,
													ILUInt16 value)
{
	ILInterlockedStoreU2(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU2_ACQUIRE)
#define ILInterlockedStoreU2_Acquire(dest, value) \
		_ILInterlockedStoreU2_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU2_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU2_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreU2_Release(volatile ILUInt16 *dest,
													ILUInt16 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreU2(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU2_RELEASE)
#define ILInterlockedStoreU2_Release(dest, value) \
		_ILInterlockedStoreU2_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU2_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU2_RELEASE) */

static IL_INLINE void _ILInterlockedStoreU2_Full(volatile ILUInt16 *dest,
													ILUInt16 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreU2(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU2_FULL)
#define ILInterlockedStoreU2_Full(dest, value) \
		_ILInterlockedStoreU2_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU2_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU2_FULL) */

/*
 * Store a signed 32 bit value to a location.
 */
static IL_INLINE void _ILInterlockedStoreI4(volatile ILInt32 *dest,
											ILInt32 value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI4)
#define ILInterlockedStoreI4(dest, value)	_ILInterlockedStoreI4((dest), (value))
#define IL_HAVE_INTERLOCKED_STOREI4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI4) */

static IL_INLINE void _ILInterlockedStoreI4_Acquire(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInterlockedStoreI4(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI4_ACQUIRE)
#define ILInterlockedStoreI4_Acquire(dest, value) \
		_ILInterlockedStoreI4_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI4_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI4_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreI4_Release(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreI4(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI4_RELEASE)
#define ILInterlockedStoreI4_Release(dest, value) \
		_ILInterlockedStoreI4_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI4_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI4_RELEASE) */

static IL_INLINE void _ILInterlockedStoreI4_Full(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreI4(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI4_FULL)
#define ILInterlockedStoreI4_Full(dest, value) \
		_ILInterlockedStoreI4_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI4_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI4_FULL) */

/*
 * Store an unsigned 32 bit value to a location.
 */
static IL_INLINE void _ILInterlockedStoreU4(volatile ILUInt32 *dest,
											ILUInt32 value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU4)
#define ILInterlockedStoreU4(dest, value)	_ILInterlockedStoreU4((dest), (value))
#define IL_HAVE_INTERLOCKED_STOREU4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU4) */

static IL_INLINE void _ILInterlockedStoreU4_Acquire(volatile ILUInt32 *dest,
													ILUInt32 value)
{
	ILInterlockedStoreU4(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU4_ACQUIRE)
#define ILInterlockedStoreU4_Acquire(dest, value) \
		_ILInterlockedStoreU4_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU4_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU4_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreU4_Release(volatile ILUInt32 *dest,
													ILUInt32 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreU4(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU4_RELEASE)
#define ILInterlockedStoreU4_Release(dest, value) \
		_ILInterlockedStoreU4_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU4_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU4_RELEASE) */

static IL_INLINE void _ILInterlockedStoreU4_Full(volatile ILUInt32 *dest,
													ILUInt32 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreU4(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU4_FULL)
#define ILInterlockedStoreU4_Full(dest, value) \
		_ILInterlockedStoreU4_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU4_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU4_FULL) */

/*
 * Store a pointer value to a location.
 */
static IL_INLINE void _ILInterlockedStoreP(void * volatile *dest,
										   void *value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STOREP)
#define ILInterlockedStoreP(dest, value)	_ILInterlockedStoreP((dest), (value))
#define IL_HAVE_INTERLOCKED_STOREP 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREP) */

static IL_INLINE void _ILInterlockedStoreP_Acquire(void * volatile *dest,
												   void *value)
{
	ILInterlockedStoreP(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREP_ACQUIRE)
#define ILInterlockedStoreP_Acquire(dest, value) \
		_ILInterlockedStoreP_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREP_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREP_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreP_Release(void * volatile *dest,
												   void *value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreP(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STOREP_RELEASE)
#define ILInterlockedStoreP_Release(dest, value) \
		_ILInterlockedStoreP_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREP_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREP_RELEASE) */

static IL_INLINE void _ILInterlockedStoreP_Full(void * volatile *dest,
												void *value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreP(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREP_FULL)
#define ILInterlockedStoreP_Full(dest, value) \
		_ILInterlockedStoreP_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREP_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREP_FULL) */

/*
 * Store a single precision floatingpoint value to a location.
 */
static IL_INLINE void _ILInterlockedStoreR4(volatile ILFloat *dest,
										   ILFloat value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STORER4)
#define ILInterlockedStoreR4(dest, value)	_ILInterlockedStoreR4((dest), (value))
#define IL_HAVE_INTERLOCKED_STORER4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER4) */

static IL_INLINE void _ILInterlockedStoreR4_Acquire(volatile ILFloat *dest,
													ILFloat value)
{
	ILInterlockedStoreR4(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STORER4_ACQUIRE)
#define ILInterlockedStoreR4_Acquire(dest, value) \
		_ILInterlockedStoreR4_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STORER4_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER4_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreR4_Release(volatile ILFloat *dest,
													ILFloat value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreR4(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STORER4_RELEASE)
#define ILInterlockedStoreR4_Release(dest, value) \
		_ILInterlockedStoreR4_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STORER4_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER4_RELEASE) */

static IL_INLINE void _ILInterlockedStoreR4_Full(volatile ILFloat *dest,
												 ILFloat value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreR4(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STORER4_FULL)
#define ILInterlockedStoreR4_Full(dest, value) \
		_ILInterlockedStoreR4_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STORER4_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER4_FULL) */

#if defined(IL_NATIVE_INT64)
/*
 * Store a signed 64 bit value to a location.
 */
static IL_INLINE void _ILInterlockedStoreI8(volatile ILInt64 *dest,
											ILInt64 value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI8)
#define ILInterlockedStoreI8(dest, value)	_ILInterlockedStoreI8((dest), (value))
#define IL_HAVE_INTERLOCKED_STOREI8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI8) */

static IL_INLINE void _ILInterlockedStoreI8_Acquire(volatile ILInt64 *dest,
													ILInt64 value)
{
	ILInterlockedStoreI8(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI8_ACQUIRE)
#define ILInterlockedStoreI8_Acquire(dest, value) \
		_ILInterlockedStoreI8_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI8_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI8_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreI8_Release(volatile ILInt64 *dest,
													ILInt64 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreI8(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI8_RELEASE)
#define ILInterlockedStoreI8_Release(dest, value) \
		_ILInterlockedStoreI8_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI8_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI8_RELEASE) */

static IL_INLINE void _ILInterlockedStoreI8_Full(volatile ILInt64 *dest,
													ILInt64 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreI8(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREI8_FULL)
#define ILInterlockedStoreI8_Full(dest, value) \
		_ILInterlockedStoreI8_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREI8_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI8_FULL) */

/*
 * Store an unsigned 64 bit value to a location.
 */
static IL_INLINE void _ILInterlockedStoreU8(volatile ILUInt64 *dest,
											ILUInt64 value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU8)
#define ILInterlockedStoreU8(dest, value)	_ILInterlockedStoreU8((dest), (value))
#define IL_HAVE_INTERLOCKED_STOREU8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU8) */

static IL_INLINE void _ILInterlockedStoreU8_Acquire(volatile ILUInt64 *dest,
													ILUInt64 value)
{
	ILInterlockedStoreU8(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU8_ACQUIRE)
#define ILInterlockedStoreU8_Acquire(dest, value) \
		_ILInterlockedStoreU8_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU8_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU8_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreU8_Release(volatile ILUInt64 *dest,
													ILUInt64 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreU8(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU8_RELEASE)
#define ILInterlockedStoreU8_Release(dest, value) \
		_ILInterlockedStoreU8_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU8_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU8_RELEASE) */

static IL_INLINE void _ILInterlockedStoreU8_Full(volatile ILUInt64 *dest,
													ILUInt64 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreU8(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STOREU8_FULL)
#define ILInterlockedStoreU8_Full(dest, value) \
		_ILInterlockedStoreU8_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STOREU8_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU8_FULL) */

/*
 * Store a double precision floatingpoint value to a location.
 */
static IL_INLINE void _ILInterlockedStoreR8(volatile ILDouble *dest,
										   ILDouble value)
{
	/*
	 * We should cast away the volatile because gcc will generate code with
	 * release semantics on IA64 otherwise but at least on the x86 family the
	 * volatile semantics needed (and what's volatile is for) gets lost and
	 * the store is likely to be optimized away.
	 */
	*dest = value;
}
#if !defined(IL_HAVE_INTERLOCKED_STORER8)
#define ILInterlockedStoreR8(dest, value)	_ILInterlockedStoreR8((dest), (value))
#define IL_HAVE_INTERLOCKED_STORER8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER8) */

static IL_INLINE void _ILInterlockedStoreR8_Acquire(volatile ILDouble *dest,
													ILDouble value)
{
	ILInterlockedStoreR8(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STORER8_ACQUIRE)
#define ILInterlockedStoreR8_Acquire(dest, value) \
		_ILInterlockedStoreR8_Acquire((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STORER8_ACQUIRE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER8_ACQUIRE) */

static IL_INLINE void _ILInterlockedStoreR8_Release(volatile ILDouble *dest,
													ILDouble value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreR8(dest, value);
}
#if !defined(IL_HAVE_INTERLOCKED_STORER8_RELEASE)
#define ILInterlockedStoreR8_Release(dest, value) \
		_ILInterlockedStoreR8_Release((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STORER8_RELEASE 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER8_RELEASE) */

static IL_INLINE void _ILInterlockedStoreR8_Full(volatile ILDouble *dest,
												 ILDouble value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedStoreR8(dest, value);
	ILInterlockedMemoryBarrier();
}
#if !defined(IL_HAVE_INTERLOCKED_STORER8_FULL)
#define ILInterlockedStoreR8_Full(dest, value) \
		_ILInterlockedStoreR8_Full((dest), (value))
#if defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER)
#define IL_HAVE_INTERLOCKED_STORER8_FULL 1
#endif /* defined(IL_HAVE_INTERLOCKED_MEMORYBARRIER) */
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER8_FULL) */
#else /* !defined(ILNATIVE_INT64) */
/*
 * Store a signed 64 bit value to a location.
 */
void _ILInterlockedStoreI8_Full(volatile ILInt64 *dest, ILInt64 value);

/*
 * Store an unsigned 64 bit value to a location.
 */
void _ILInterlockedStoreU8_Full(volatile ILUInt64 *dest, ILUInt64 value);

/*
 * Store a double precision floatingpoint value to a location.
 */
void _ILInterlockedStoreR8_Full(volatile ILDouble *dest, ILDouble value);

#if !defined(IL_HAVE_INTERLOCKED_STOREI8)
#define ILInterlockedStoreI8(dest, value) \
		_ILInterlockedStoreI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI8) */
#if !defined(IL_HAVE_INTERLOCKED_STOREI8_ACQUIRE)
#define ILInterlockedStoreI8_Acquire(dest, value) \
		_ILInterlockedStoreI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_STOREI8_RELEASE)
#define ILInterlockedStoreI8_Release(dest, value) \
		_ILInterlockedStoreI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_STOREI8_FULL)
#define ILInterlockedStoreI8_Full(dest, value) \
		_ILInterlockedStoreI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREI8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_STOREU8)
#define ILInterlockedStoreU8(dest, value) \
		_ILInterlockedStoreU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU8) */
#if !defined(IL_HAVE_INTERLOCKED_STOREU8_ACQUIRE)
#define ILInterlockedStoreU8_Acquire(dest, value) \
		_ILInterlockedStoreU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_STOREU8_RELEASE)
#define ILInterlockedStoreU8_Release(dest, value) \
		_ILInterlockedStoreU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_STOREU8_FULL)
#define ILInterlockedStoreU8_Full(dest, value) \
		_ILInterlockedStoreU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STOREU8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_STORER8)
#define ILInterlockedStoreR8(dest, value) \
		_ILInterlockedStoreR8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER8) */
#if !defined(IL_HAVE_INTERLOCKED_STORER8_ACQUIRE)
#define ILInterlockedStoreR8_Acquire(dest, value) \
		_ILInterlockedStoreR8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_STORER8_RELEASE)
#define ILInterlockedStoreR8_Release(dest, value) \
		_ILInterlockedStoreR8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_STORER8_FULL)
#define ILInterlockedStoreR8_Full(dest, value) \
		_ILInterlockedStoreR8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_STORER8_FULL) */
#endif /* !defined(ILNATIVE_INT64) */

/*
 * Exchange values
 */

/*
 * Map the definitions for equal argument sizes
 */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4)
#define ILInterlockedExchangeU4(dest, value) \
		ILInterlockedExchangeI4((volatile ILInt32 *)(dest), (ILInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE)
#define ILInterlockedExchangeU4_Acquire(dest, value) \
		ILInterlockedExchangeI4_Acquire((volatile ILInt32 *)(dest), (ILInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE)
#define ILInterlockedExchangeU4_Release(dest, value) \
		ILInterlockedExchangeI4_Release((volatile ILInt32 *)(dest), (ILInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL)
#define ILInterlockedExchangeU4_Full(dest, value) \
		ILInterlockedExchangeI4_Full((volatile ILInt32 *)(dest), (ILInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL 1
#endif

#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEU4)
#define ILInterlockedExchangeI4(dest, value) \
		ILInterlockedExchangeU4((volatile ILUInt32 *)(dest), (ILUInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE)
#define ILInterlockedExchangeI4_Acquire(dest, value) \
		ILInterlockedExchangeU4_Acquire((volatile ILUInt32 *)(dest), (ILUInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE)
#define ILInterlockedExchangeI4_Release(dest, value) \
		ILInterlockedExchangeU4_Release((volatile ILUInt32 *)(dest), (ILUInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL)
#define ILInterlockedExchangeI4_Full(dest, value) \
		ILInterlockedExchangeU4_Full((volatile ILUInt32 *)(dest), (ILUInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL 1
#endif

#if (SIZEOF_FLOAT == 4)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4)
static IL_INLINE ILFloat ILInterlockedExchangeR4(volatile ILFloat *dest,
												 ILFloat value)
{
	ILInterlockedConv4 conv4;
	ILInterlockedConv4 val;

	val.floatValue = value;
	conv4.int32Value = ILInterlockedExchangeI4((volatile ILInt32 *)dest,
											   val.int32Value);
	return conv4.floatValue;
}
#define IL_HAVE_INTERLOCKED_EXCHANGER4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL)
static IL_INLINE ILFloat ILInterlockedExchangeR4_Full(volatile ILFloat *dest,
													  ILFloat value)
{
	ILInterlockedConv4 conv4;
	ILInterlockedConv4 val;

	val.floatValue = value;
	conv4.int32Value = ILInterlockedExchangeI4_Full((volatile ILInt32 *)dest,
													val.int32Value);
	return conv4.floatValue;
}
#define IL_HAVE_INTERLOCKED_EXCHANGER4_FULL 1
#endif
#endif /* (sizeof(ILFloat) == sizeof(ILInt32)) */

#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8)
#define ILInterlockedExchangeU8(dest, value) \
		ILInterlockedExchangeI8((volatile ILInt64 *)(dest), (ILInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE)
#define ILInterlockedExchangeU8_Acquire(dest, value) \
		ILInterlockedExchangeI8_Acquire((volatile ILInt64 *)(dest), (ILInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE)
#define ILInterlockedExchangeU8_Release(dest, value) \
		ILInterlockedExchangeI8_Release((volatile ILInt64 *)(dest), (ILInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL)
#define ILInterlockedExchangeU8_Full(dest, value) \
		ILInterlockedExchangeI8_Full((volatile ILInt64 *)(dest), (ILInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL 1
#endif

#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEU8)
#define ILInterlockedExchangeI8(dest, value) \
		ILInterlockedExchangeU8((volatile ILUInt64 *)(dest), (ILUInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE)
#define ILInterlockedExchangeI8_Acquire(dest, value) \
		ILInterlockedExchangeU8_Acquire((volatile ILUInt64 *)(dest), (ILUInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE)
#define ILInterlockedExchangeI8_Release(dest, value) \
		ILInterlockedExchangeU8_Release((volatile ILUInt64 *)(dest), (ILUInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL)
#define ILInterlockedExchangeI8_Full(dest, value) \
		ILInterlockedExchangeU8_Full((volatile ILUInt64 *)(dest), (ILUInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL 1
#endif

#if (SIZEOF_DOUBLE == 8)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8)
static IL_INLINE ILDouble ILInterlockedExchangeR8(volatile ILDouble *dest,
												  ILDouble value)
{
	ILInterlockedConv8 conv8;
	ILInterlockedConv8 val;

	val.doubleValue = value;
	conv8.int64Value = ILInterlockedExchangeI8((volatile ILInt64 *)dest,
											   val.int64Value);
	return conv8.doubleValue;
}
#define IL_HAVE_INTERLOCKED_EXCHANGER8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL)
static IL_INLINE ILDouble ILInterlockedExchangeR8_Full(volatile ILDouble *dest,
													   ILDouble value)
{
	ILInterlockedConv8 conv8;
	ILInterlockedConv8 val;

	val.doubleValue = value;
	conv8.int64Value = ILInterlockedExchangeI8_Full((volatile ILInt64 *)dest,
													*(ILInt64 *)valuePtr);
	return conv8.doubleValue;
}
#define IL_HAVE_INTERLOCKED_EXCHANGER8_FULL 1
#endif
#endif /* (sizeof(ILDouble) == sizeof(ILInt64)) */

#if defined(IL_NATIVE_INT64)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8)
#define ILInterlockedExchangeP(dest, value) \
		ILInterlockedExchangeI8((volatile ILInt64 *)(dest), (ILInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE)
#define ILInterlockedExchangeP_Acquire(dest, value) \
		ILInterlockedExchangeI8_Acquire((volatile ILInt64 *)(dest), (ILInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE)
#define ILInterlockedExchangeP_Release(dest, value) \
		ILInterlockedExchangeI8_Release((volatile ILInt64 *)(dest), (ILInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL)
#define ILInterlockedExchangeP_Full(dest, value) \
		ILInterlockedExchangeI8_Full((volatile ILInt64 *)(dest), (ILInt64)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP_FULL 1
#endif
#else /* !defined(IL_NATIVE_INT64) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4)
#define ILInterlockedExchangeP(dest, value) \
		ILInterlockedExchangeI4((volatile ILInt32 *)(dest), (ILInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE)
#define ILInterlockedExchangeP_Acquire(dest, value) \
		ILInterlockedExchangeI4_Acquire((volatile ILInt32 *)(dest), (ILInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE)
#define ILInterlockedExchangeP_Release(dest, value) \
		ILInterlockedExchangeI4_Release((volatile ILInt32 *)(dest), (ILInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL) && \
	defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL)
#define ILInterlockedExchangeP_Full(dest, value) \
		ILInterlockedExchangeI4_Full((volatile ILInt32 *)(dest), (ILInt32)(value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP_FULL 1
#endif
#endif /* !defined(IL_NATIVE_INT64) */

/*
 * Exchange two signed 32 bit integers.
 */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEI4)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE)
static IL_INLINE ILInt32 ILInterlockedExchangeI4_Acquire(volatile ILInt32 *dest,
														 ILInt32 value)
{
	ILInt32 retval;

	retval = ILInterlockedExchangeI4(dest, value);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE)
static IL_INLINE ILInt32 ILInterlockedExchangeI4_Release(volatile ILInt32 *dest,
														 ILInt32 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedExchangeI4(dest, value);
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL)
static IL_INLINE ILInt32 ILInterlockedExchangeI4_Full(volatile ILInt32 *dest,
													  ILInt32 value)
{
	ILInt32 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedExchangeI4(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEI4) */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE)
#define ILInterlockedExchangeI4_Acquire(dest, value) \
		ILInterlockedExchangeI4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGE_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE)
#define ILInterlockedExchangeI4_Release(dest, value) \
		ILInterlockedExchangeI4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4)
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE)
#define ILInterlockedExchangeI4(dest, value) \
		ILInterlockedExchangeI4_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI4 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE)
#define ILInterlockedExchangeI4(dest, value) \
		ILInterlockedExchangeI4_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI4 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL)
#define ILInterlockedExchangeI4(dest, value) \
		ILInterlockedExchangeI4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4) */

/*
 * Exchange two unsigned 32 bit integers.
 */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEU4)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE)
static IL_INLINE ILUInt32 ILInterlockedExchangeU4_Acquire(volatile ILUInt32 *dest,
														  ILUInt32 value)
{
	ILUInt32 retval;

	retval = ILInterlockedExchangeU4(dest, value);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE)
static IL_INLINE ILUInt32 ILInterlockedExchangeU4_Release(volatile ILUInt32 *dest,
														  ILUInt32 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedExchangeU4(dest, value);
}
#define IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL)
static IL_INLINE ILUInt32 ILInterlockedExchangeU4_Full(volatile ILUInt32 *dest,
													   ILUInt32 value)
{
	ILUInt32 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedExchangeU4(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEU4) */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE)
#define ILInterlockedExchangeU4_Acquire(dest, value) \
		ILInterlockedExchangeU4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE)
#define ILInterlockedExchangeU4_Release(dest, value) \
		ILInterlockedExchangeU4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4)
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE)
#define ILInterlockedExchangeU4(dest, value) \
		ILInterlockedExchangeU4_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU4 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE)
#define ILInterlockedExchangeU4(dest, value) \
		ILInterlockedExchangeU4_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU4 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL)
#define ILInterlockedExchangeU4(dest, value) \
		ILInterlockedExchangeU4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4) */

/*
 * Exchange two single precision floatingpoint integers.
 */

#if defined(IL_HAVE_INTERLOCKED_EXCHANGER4)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_ACQUIRE)
static IL_INLINE ILFloat ILInterlockedExchangeR4_Acquire(volatile ILFloat *dest,
														 ILFloat value)
{
	ILFloat retval;

	retval = ILInterlockedExchangeR4(dest, value);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGER4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_RELEASE)
static IL_INLINE ILFloat ILInterlockedExchangeR4_Release(volatile ILFloat *dest,
														 ILFloat value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedExchangeR4(dest, value);
}
#define IL_HAVE_INTERLOCKED_EXCHANGER4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_FULL)
static IL_INLINE ILFloat ILInterlockedExchangeR4_Full(volatile ILFloat *dest,
													  ILFloat value)
{
	ILFloat retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedExchangeR4(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGER4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGER4) */

#if defined(IL_HAVE_INTERLOCKED_EXCHANGER4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_ACQUIRE)
#define ILInterlockedExchangeR4_Acquire(dest, value) \
		ILInterlockedExchangeR4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_RELEASE)
#define ILInterlockedExchangeR4_Release(dest, value) \
		ILInterlockedExchangeR4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGER4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4)
#if defined(IL_HAVE_INTERLOCKED_EXCHANGER4_ACQUIRE)
#define ILInterlockedExchangeR4(dest, value) \
		ILInterlockedExchangeR4_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER4 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGER4_RELEASE)
#define ILInterlockedExchangeR4(dest, value) \
		ILInterlockedExchangeR4_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER4 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGER4_FULL)
#define ILInterlockedExchangeR4(dest, value) \
		ILInterlockedExchangeR4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4) */

/*
 * Exchange two signed 64 bit integers.
 */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEI8)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE)
static IL_INLINE ILInt64 ILInterlockedExchangeI8_Acquire(volatile ILInt64 *dest,
														 ILInt64 value)
{
	ILInt64 retval;

	retval = ILInterlockedExchangeI8(dest, value);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE)
static IL_INLINE ILInt64 ILInterlockedExchangeI8_Release(volatile ILInt64 *dest,
														 ILInt64 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedExchangeI8(dest, value);
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL)
static IL_INLINE ILInt64 ILInterlockedExchangeI8_Full(volatile ILInt64 *dest,
													  ILInt64 value)
{
	ILInt64 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedExchangeI8(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEI8) */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE)
#define ILInterlockedExchangeI8_Acquire(dest, value) \
		ILInterlockedExchangeI8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE)
#define ILInterlockedExchangeI8_Release(dest, value) \
		ILInterlockedExchangeI8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8)
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE)
#define ILInterlockedExchangeI8(dest, value) \
		ILInterlockedExchangeI8_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI8 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE)
#define ILInterlockedExchangeI8(dest, value) \
		ILInterlockedExchangeI8_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI8 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL)
#define ILInterlockedExchangeI8(dest, value) \
		ILInterlockedExchangeI8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEI8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8) */

/*
 * Exchange two unsigned 64 bit integers.
 */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEU8)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE)
static IL_INLINE ILUInt64 ILInterlockedExchangeU8_Acquire(volatile ILUInt64 *dest,
														  ILUInt64 value)
{
	ILUInt64 retval;

	retval = ILInterlockedExchangeU8(dest, value);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE)
static IL_INLINE ILUInt64 ILInterlockedExchangeU8_Release(volatile ILUInt64 *dest,
														  ILUInt64 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedExchangeU8(dest, value);
}
#define IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL)
static IL_INLINE ILUInt64 ILInterlockedExchangeU8_Full(volatile ILUInt64 *dest,
													   ILUInt64 value)
{
	ILUInt64 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedExchangeU8(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEU8) */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE)
#define ILInterlockedExchangeU8_Acquire(dest, value) \
		ILInterlockedExchangeU8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE)
#define ILInterlockedExchangeU8_Release(dest, value) \
		ILInterlockedExchangeU8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8)
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE)
#define ILInterlockedExchangeU8(dest, value) \
		ILInterlockedExchangeU8_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU8 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE)
#define ILInterlockedExchangeU8(dest, value) \
		ILInterlockedExchangeU8_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU8 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL)
#define ILInterlockedExchangeU8(dest, value) \
		ILInterlockedExchangeU8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEU8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8) */

/*
 * Exchange two double precision floatingpoint values.
 */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGER8)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_ACQUIRE)
static IL_INLINE ILDouble ILInterlockedExchangeR8_Acquire(volatile ILDouble *dest,
														  ILDouble value)
{
	ILDouble retval;

	retval = ILInterlockedExchangeR8(dest, value);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGER8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_RELEASE)
static IL_INLINE ILDouble ILInterlockedExchangeR8_Release(volatile ILDouble *dest,
														  ILDouble value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedExchangeR8(dest, value);
}
#define IL_HAVE_INTERLOCKED_EXCHANGER8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_FULL)
static IL_INLINE ILDouble ILInterlockedExchangeR8_Full(volatile ILDouble *dest,
													   ILDouble value)
{
	ILDouble retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedExchangeR8(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGER8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGER8) */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGER8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_ACQUIRE)
#define ILInterlockedExchangeR8_Acquire(dest, value) \
		ILInterlockedExchangeR8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_RELEASE)
#define ILInterlockedExchangeR8_Release(dest, value) \
		ILInterlockedExchangeR8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGER8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8)
#if defined(IL_HAVE_INTERLOCKED_EXCHANGER8_ACQUIRE)
#define ILInterlockedExchangeR8(dest, value) \
		ILInterlockedExchangeRU8_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER8 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGER8_RELEASE)
#define ILInterlockedExchangeR8(dest, value) \
		ILInterlockedExchangeR8_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER8 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGER8_FULL)
#define ILInterlockedExchangeR8(dest, value) \
		ILInterlockedExchangeR8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGER8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8) */

/*
 * Exchange two pointers.
 */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEP)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE)
static IL_INLINE void *ILInterlockedExchangeP_Acquire(void * volatile *dest,
													  void *value)
{
	void *retval;

	retval = ILInterlockedExchangeP(dest, value);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE)
static IL_INLINE void *ILInterlockedExchangeP_Release(void * volatile *dest,
													  void *value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedExchangeP(dest, value);
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL)
static IL_INLINE void *ILInterlockedExchangeP_Full(void * volatile *dest,
												   void *value)
{
	void *retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedExchangeP(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEP) */
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL)
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE)
#define ILInterlockedExchangeP_Acquire(dest, value) \
		ILInterlockedExchangeP_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE)
#define ILInterlockedExchangeP_Release(dest, value) \
		ILInterlockedExchangeP_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP)
#if defined(IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE)
#define ILInterlockedExchangeP(dest, value) \
		ILInterlockedExchangeP_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE)
#define ILInterlockedExchangeP(dest, value) \
		ILInterlockedExchangeP_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP 1
#elif defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL)
#define ILInterlockedExchangeP(dest, value) \
		ILInterlockedExchangeP_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_EXCHANGEP 1
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP) */

/*
 * Exchange pointers.
 */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP)
#define ILInterlockedExchangeP(dest, value) \
		_ILInterlockedExchangeP_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE)
#define ILInterlockedExchangeP_Acquire(dest, value) \
		_ILInterlockedExchangeP_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE)
#define ILInterlockedExchangeP_Release(dest, value) \
		_ILInterlockedExchangeP_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL)
#define ILInterlockedExchangeP_Full(dest, value) \
		_ILInterlockedExchangeP_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEP_FULL) */

/*
 * Compare and exchange two values
 */

/*
 * Map the definitions for equal argument sizes
 */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4)
#define ILInterlockedCompareAndExchangeU4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4((volatile ILInt32 *)(dest), (ILInt32)(value), (ILInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE)
#define ILInterlockedCompareAndExchangeU4_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Acquire((volatile ILInt32 *)(dest), (ILInt32)(value), (ILInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE)
#define ILInterlockedCompareAndExchangeU4_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Release((volatile ILInt32 *)(dest), (ILInt32)(value), (ILInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL)
#define ILInterlockedCompareAndExchangeU4_Full(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Full((volatile ILInt32 *)(dest), (ILInt32)(value), (ILInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL 1
#endif

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4)
#define ILInterlockedCompareAndExchangeI4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU4((volatile ILUInt32 *)(dest), (ILUInt32)(value), (ILUInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE)
#define ILInterlockedCompareAndExchangeI4_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU4_Acquire((volatile ILUInt32 *)(dest), (ILUInt32)(value), (ILUInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE)
#define ILInterlockedCompareAndExchangeI4_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU4_Release((volatile ILUInt32 *)(dest), (ILUInt32)(value), (ILUInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL)
#define ILInterlockedCompareAndExchangeI4_Full(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU4_Full((volatile ILUInt32 *)(dest), (ILUInt32)(value), (ILUInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL 1
#endif

#if (SIZEOF_FLOAT == 4)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4)
static IL_INLINE ILFloat ILInterlockedCompareAndExchangeR4(volatile ILFloat *dest,
														   ILFloat value,
														   ILFloat comparand)
{
	ILInterlockedConv4 conv4;
	ILInterlockedConv4 val;
	ILInterlockedConv4 cmp;

	val.floatValue = value;
	cmp.floatValue = comparand;
	conv4.int32Value = ILInterlockedCompareAndExchangeI4((volatile ILInt32 *)dest,
														 val.int32Value,
														 cmp.int32Value);
	return conv4.floatValue;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL)
static IL_INLINE ILFloat ILInterlockedCompareAndExchangeR4_Full(volatile ILFloat *dest,
																ILFloat value,
																ILFloat comparand)
{
	ILInterlockedConv4 conv4;
	ILInterlockedConv4 val;
	ILInterlockedConv4 cmp;

	val.floatValue = value;
	cmp.floatValue = comparand;
	conv4.int32Value = ILInterlockedCompareAndExchangeI4_Full((volatile ILInt32 *)dest,
															  val.int32Value,
															  cmp.int32Value);
	return conv4.floatValue;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL 1
#endif
#endif /* (sizeof(ILFloat) == sizeof(ILInt32)) */

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8)
#define ILInterlockedCompareAndExchangeU8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8((volatile ILInt64 *)(dest), (ILInt64)(value), (ILInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE)
#define ILInterlockedCompareAndExchangeU8_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Acquire((volatile ILInt64 *)(dest), (ILInt64)(value), (ILInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE)
#define ILInterlockedCompareAndExchangeU8_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Release((volatile ILInt64 *)(dest), (ILInt64)(value), (ILInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL)
#define ILInterlockedCompareAndExchangeU8_Full(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Full((volatile ILInt64 *)(dest), (ILInt64)(value), (ILInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL 1
#endif

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8)
#define ILInterlockedCompareAndExchangeI8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU8((volatile ILUInt64 *)(dest), (ILUInt64)(value), (ILUInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE)
#define ILInterlockedCompareAndExchangeI8_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU8_Acquire((volatile ILUInt64 *)(dest), (ILUInt64)(value), (ILUInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE)
#define ILInterlockedCompareAndExchangeI8_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU8_Release((volatile ILUInt64 *)(dest), (ILUInt64)(value), (ILUInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL)
#define ILInterlockedCompareAndExchangeI8_Full(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU8_Full((volatile ILUInt64 *)(dest), (ILUInt64)(value), (ILUInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL 1
#endif

#if (SIZEOF_DOUBLE == 8)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8)
static IL_INLINE ILDouble ILInterlockedCompareAndExchangeR8(volatile ILDouble *dest,
															ILDouble value,
															ILDouble comparand)
{
	ILInterlockedConv8 conv8;
	ILInterlockedConv8 val;
	ILInterlockedConv8 cmp;

	val.doubleValue = value;
	cmp.doubleValue = comparand;
	conv8.int64Value = ILInterlockedCompareAndExchangeI8((volatile ILInt64 *)dest,
														 val.int64Value,
														 cmp.int64Value);
	return conv8.doubleValue;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL)
static IL_INLINE ILDouble ILInterlockedCompareAndExchangeR8_Full(volatile ILDouble *dest,
																 ILDouble value,
																 ILDouble comparand)
{
	ILInterlockedConv8 conv8;
	ILInterlockedConv8 val;
	ILInterlockedConv8 cmp;

	val.doubleValue = value;
	cmp.doubleValue = comparand;
	conv8.int64Value = ILInterlockedCompareAndExchangeI8_Full((volatile ILInt64 *)dest,
															  val.int64Value,
															  cmp.int64Value);
	return conv8.doubleValue;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL 1
#endif
#endif /* (sizeof(ILDouble) == sizeof(ILInt64)) */

#if defined(IL_NATIVE_INT64)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8)
#define ILInterlockedCompareAndExchangeP(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8((volatile ILInt64 *)(dest), (ILInt64)(value), (ILInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE)
#define ILInterlockedCompareAndExchangeP_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Acquire((volatile ILInt64 *)(dest), (ILInt64)(value), (ILInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE)
#define ILInterlockedCompareAndExchangeP_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Release((volatile ILInt64 *)(dest), (ILInt64)(value), (ILInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL)
#define ILInterlockedCompareAndExchangeP_Full(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Full((volatile ILInt64 *)(dest), (ILInt64)(value), (ILInt64)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL 1
#endif
#else /* !defined(IL_NATIVE_INT64) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4)
#define ILInterlockedCompareAndExchangeP(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4((volatile ILInt32 *)(dest), (ILInt32)(value), (ILInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE)
#define ILInterlockedCompareAndExchangeP_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Acquire((volatile ILInt32 *)(dest), (ILInt32)(value), (ILInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE)
#define ILInterlockedCompareAndExchangeP_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Release((volatile ILInt32 *)(dest), (ILInt32)(value), (ILInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL)
#define ILInterlockedCompareAndExchangeP_Full(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Full((volatile ILInt32 *)(dest), (ILInt32)(value), (ILInt32)(comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL 1
#endif
#endif /* !defined(IL_NATIVE_INT64) */

/*
 * Support alternative implementations with only picking up real native
 * implementationd of ILInterlockedCompareAndExchange functions.
 */
#if !defined(IL_HAVE_INTERLOCKED_ADDI4) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4)
static IL_INLINE ILInt32 ILInterlockedAddI4(volatile ILInt32 *dest,
											ILInt32 value)
{
	ILInt32 oldval;
	ILInt32 retval;

	do
	{
		oldval = ILInterlockedLoadI4(dest);
		retval = oldval + value;
	} while(ILInterlockedCompareAndExchangeI4(dest, retval, oldval) != oldval);

	return retval;	
}
#define IL_HAVE_INTERLOCKED_ADDI4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_ADDI4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL)
static IL_INLINE ILInt32 ILInterlockedAddI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 oldval;
	ILInt32 retval;

	do
	{
		oldval = ILInterlockedLoadI4(dest);
		retval = oldval + value;
	} while(ILInterlockedCompareAndExchangeI4_Full(dest, retval, oldval) != oldval);

	return retval;	
}
#define IL_HAVE_INTERLOCKED_ADDI4_FULL 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_ADDI8) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8)
static IL_INLINE ILInt64 ILInterlockedAddI8(volatile ILInt64 *dest,
											ILInt64 value)
{
	ILInt64 oldval;
	ILInt64 retval;

	do
	{
		oldval = ILInterlockedLoadI8(dest);
		retval = oldval + value;
	} while(ILInterlockedCompareAndExchangeI8(dest, retval, oldval) != oldval);

	return retval;	
}
#define IL_HAVE_INTERLOCKED_ADDI8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_ADDI8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL)
static IL_INLINE ILInt64 ILInterlockedAddI8_Full(volatile ILInt64 *dest,
												 ILInt64 value)
{
	ILInt64 oldval;
	ILInt64 retval;

	do
	{
		oldval = ILInterlockedLoadI8(dest);
		retval = oldval + value;
	} while(ILInterlockedCompareAndExchangeI8_Full(dest, retval, oldval) != oldval);

	return retval;	
}
#define IL_HAVE_INTERLOCKED_ADDI8_FULL 1
#endif

#if !defined(IL_HAVE_INTERLOCKED_SUBI4) && \
	defined(IL_HAVE_INTERLOCKED_ADDI4)
#define ILInterlockedSubI4(dest, value)	ILInterlockedAddI4((dest), -(value))
#define IL_HAVE_INTERLOCKED_SUBI4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_SUBI4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_ADDI4_FULL)
#define ILInterlockedSubI4_Full(dest, value) \
		ILInterlockedAddI4_Full((dest), -(value))
#define IL_HAVE_INTERLOCKED_SUBI4_FULL 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_SUBI8) && \
	defined(IL_HAVE_INTERLOCKED_ADDI8)
#define ILInterlockedSubI8(dest, value)	ILInterlockedAddI8((dest), -(value))
#define IL_HAVE_INTERLOCKED_SUBI8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_SUBI8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_ADDI8_FULL)
#define ILInterlockedSubI8_Full(dest, value) \
		ILInterlockedAddI8_Full((dest), -(value))
#define IL_HAVE_INTERLOCKED_SUBI8_FULL 1
#endif

#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4) && \
	defined(IL_HAVE_INTERLOCKED_ADDI4)
#define ILInterlockedIncrementI4(dest)	ILInterlockedAddI4((dest), 1)
#define IL_HAVE_INTERLOCKED_INCREMENTI4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_ADDI4_FULL)
#define ILInterlockedIncrementI4_Full(dest) \
		ILInterlockedAddI4_Full((dest), 1)
#define IL_HAVE_INTERLOCKED_INCREMENTI4_FULL 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8) && \
	defined(IL_HAVE_INTERLOCKED_ADDI8)
#define ILInterlockedIncrementI8(dest)	ILInterlockedAddI8((dest), 1)
#define IL_HAVE_INTERLOCKED_INCREMENTI8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_ADDI8_FULL)
#define ILInterlockedIncrementI8_Full(dest) \
		ILInterlockedAddI8_Full((dest), 1)
#define IL_HAVE_INTERLOCKED_INCREMENTI8_FULL 1
#endif

#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4) && \
	defined(IL_HAVE_INTERLOCKED_SUBI4)
#define ILInterlockedDecrementI4(dest)	ILInterlockedSubI4((dest), 1)
#define IL_HAVE_INTERLOCKED_DECREMENTI4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_SUBI4_FULL)
#define ILInterlockedDecrementI4_Full(dest) \
		ILInterlockedSubI4_Full((dest), 1)
#define IL_HAVE_INTERLOCKED_DECREMENTI4_FULL 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8) && \
	defined(IL_HAVE_INTERLOCKED_SUBI8)
#define ILInterlockedDecrementI8(dest)	ILInterlockedSubI8((dest), 1)
#define IL_HAVE_INTERLOCKED_DECREMENTI8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_SUBI8_FULL)
#define ILInterlockedDecrementI8_Full(dest) \
		ILInterlockedSubI8_Full((dest), 1)
#define IL_HAVE_INTERLOCKED_DECREMENTI8_FULL 1
#endif

#if !defined(IL_HAVE_INTERLOCKED_ANDU4) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4)
static IL_INLINE void ILInterlockedAndU4(volatile ILUInt32 *dest,
										 ILUInt32 value)
{
	ILUInt32 oldval;
	ILUInt32 retval;

	do
	{
		oldval = ILInterlockedLoadU4(dest);
		retval = oldval & value;
	} while(ILInterlockedCompareAndExchangeU4(dest, retval, oldval) != oldval);
}
#define IL_HAVE_INTERLOCKED_ANDU4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_ANDU4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL)
static IL_INLINE void ILInterlockedAndU4_Full(volatile ILUInt32 *dest,
											  ILUInt32 value)
{
	ILUInt32 oldval;
	ILUInt32 retval;

	do
	{
		oldval = ILInterlockedLoadU4(dest);
		retval = oldval & value;
	} while(ILInterlockedCompareAndExchangeU4_Full(dest, retval, oldval) != oldval);
}
#define IL_HAVE_INTERLOCKED_ANDU4_FULL 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_ANDU8) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8)
static IL_INLINE void ILInterlockedAndU8(volatile ILUInt64 *dest,
										 ILUInt64 value)
{
	ILUInt64 oldval;
	ILUInt64 retval;

	do
	{
		oldval = ILInterlockedLoadU8(dest);
		retval = oldval & value;
	} while(ILInterlockedCompareAndExchangeU8(dest, retval, oldval) != oldval);
}
#define IL_HAVE_INTERLOCKED_ANDU8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_ANDU8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL)
static IL_INLINE void ILInterlockedAndU8_Full(volatile ILUInt64 *dest,
											  ILUInt64 value)
{
	ILUInt64 oldval;
	ILUInt64 retval;

	do
	{
		oldval = ILInterlockedLoadU8(dest);
		retval = oldval & value;
	} while(ILInterlockedCompareAndExchangeU8_Full(dest, retval, oldval) != oldval);
}
#define IL_HAVE_INTERLOCKED_ANDU8_FULL 1
#endif

#if !defined(IL_HAVE_INTERLOCKED_ORU4) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4)
static IL_INLINE void ILInterlockedOrU4(volatile ILUInt32 *dest,
										ILUInt32 value)
{
	ILUInt32 oldval;
	ILUInt32 retval;

	do
	{
		oldval = ILInterlockedLoadU4(dest);
		retval = oldval | value;
	} while(ILInterlockedCompareAndExchangeU4(dest, retval, oldval) != oldval);
}
#define IL_HAVE_INTERLOCKED_ORU4 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_ORU4_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL)
static IL_INLINE void ILInterlockedOrU4_Full(volatile ILUInt32 *dest,
											 ILUInt32 value)
{
	ILUInt32 oldval;
	ILUInt32 retval;

	do
	{
		oldval = ILInterlockedLoadU4(dest);
		retval = oldval | value;
	} while(ILInterlockedCompareAndExchangeU4_Full(dest, retval, oldval) != oldval);
}
#define IL_HAVE_INTERLOCKED_ORU4_FULL 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_ORU8) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8)
static IL_INLINE void ILInterlockedOrU8(volatile ILUInt64 *dest,
										ILUInt64 value)
{
	ILUInt64 oldval;
	ILUInt64 retval;

	do
	{
		oldval = ILInterlockedLoadU8(dest);
		retval = oldval | value;
	} while(ILInterlockedCompareAndExchangeU8(dest, retval, oldval) != oldval);
}
#define IL_HAVE_INTERLOCKED_ORU8 1
#endif
#if !defined(IL_HAVE_INTERLOCKED_ORU8_FULL) && \
	defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL)
static IL_INLINE void ILInterlockedOrU8_Full(volatile ILUInt64 *dest,
											 ILUInt64 value)
{
	ILUInt64 oldval;
	ILUInt64 retval;

	do
	{
		oldval = ILInterlockedLoadU8(dest);
		retval = oldval | value;
	} while(ILInterlockedCompareAndExchangeU8_Full(dest, retval, oldval) != oldval);
}
#define IL_HAVE_INTERLOCKED_ORU8_FULL 1
#endif

/*
 * Compare and exchange two signed 32 bit integers.
 */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE)
static IL_INLINE ILInt32 ILInterlockedCompareAndExchangeI4_Acquire(volatile ILInt32 *dest,
																   ILInt32 value,
																   ILInt32 comparand)
{
	ILInt32 retval;

	retval = ILInterlockedCompareAndExchangeI4(dest, value, comparand);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE)
static IL_INLINE ILInt32 ILInterlockedCompareAndExchangeI4_Release(volatile ILInt32 *dest,
																   ILInt32 value,
																   ILInt32 comparand)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedCompareAndExchangeI4(dest, value, comparand);
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL)
static IL_INLINE ILInt32 ILInterlockedCompareAndExchangeI4_Full(volatile ILInt32 *dest,
																ILInt32 value,
																ILInt32 comparand)
{
	ILInt32 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedCompareAndExchangeI4(dest, value, comparand);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4) */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE)
#define ILInterlockedCompareAndExchangeI4_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGE_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE)
#define ILInterlockedCompareAndExchangeI4_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4)
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE)
#define ILInterlockedCompareAndExchangeI4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Acquire((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE)
#define ILInterlockedCompareAndExchangeI4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Release((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL)
#define ILInterlockedCompareAndExchangeI4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI4_Full((dest), (value),, (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4) */

/*
 * Compare and exchange two unsigned 32 bit integers.
 */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE)
static IL_INLINE ILUInt32 ILInterlockedCompareAndExchangeU4_Acquire(volatile ILUInt32 *dest,
																	ILUInt32 value,
																	ILUInt32 comparand)
{
	ILUInt32 retval;

	retval = ILInterlockedCompareAndExchangeU4(dest, value, comparand);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE)
static IL_INLINE ILUInt32 ILInterlockedCompareAndExchangeU4_Release(volatile ILUInt32 *dest,
																	ILUInt32 value,
																	ILUInt32 comparand)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedCompareAndExchangeU4(dest, value, comparand);
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL)
static IL_INLINE ILUInt32 ILInterlockedCompareAndExchangeU4_Full(volatile ILUInt32 *dest,
																 ILUInt32 value,
																 ILUInt32 comparand)
{
	ILUInt32 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedCompareAndExchangeU4(dest, value, comparand);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4) */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE)
#define ILInterlockedCompareAndExchangeU4_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU4_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE)
#define ILInterlockedCompareAndExchangeU4_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU4_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4)
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE)
#define ILInterlockedCompareAndExchangeU4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU4_Acquire((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE)
#define ILInterlockedCompareAndExchangeU4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU4_Release((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL)
#define ILInterlockedCompareAndExchangeU4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU4_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4) */

/*
 * Compare and exchange two single precision floatingpoint integers.
 */

#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_ACQUIRE)
static IL_INLINE ILFloat ILInterlockedCompareAndExchangeR4_Acquire(volatile ILFloat *dest,
																   ILFloat value,
																   ILFloat comparand)
{
	ILFloat retval;

	retval = ILInterlockedCompareAndExchangeR4(dest, value, comparand);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_RELEASE)
static IL_INLINE ILFloat ILInterlockedCompareAndExchangeR4_Release(volatile ILFloat *dest,
																   ILFloat value,
																   ILFloat comparand)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedCompareAndExchangeR4(dest, value, comparand);
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL)
static IL_INLINE ILFloat ILInterlockedCompareAndExchangeR4_Full(volatile ILFloat *dest,
																ILFloat value,
																ILFloat comparand)
{
	ILFloat retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedCompareAndExchangeR4(dest, value, comparand);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4) */

#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_ACQUIRE)
#define ILInterlockedCompareAndExchangeR4_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeR4_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_RELEASE)
#define ILInterlockedCompareAndExchangeR4_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeR4_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4)
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_ACQUIRE)
#define ILInterlockedCompareAndExchangeR4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeR4_Acquire((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_RELEASE)
#define ILInterlockedCompareAndExchangeR4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeR4_Release((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL)
#define ILInterlockedCompareAndExchangeR4(dest, value, comparand) \
		ILInterlockedCompareAndExchangeR4_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4) */

/*
 * Compare and exchange two signed 64 bit integers.
 */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE)
static IL_INLINE ILInt64 ILInterlockedCompareAndExchangeI8_Acquire(volatile ILInt64 *dest,
																   ILInt64 value,
																   ILInt64 comparand)
{
	ILInt64 retval;

	retval = ILInterlockedCompareAndExchangeI8(dest, value, comparand);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE)
static IL_INLINE ILInt64 ILInterlockedCompareAndExchangeI8_Release(volatile ILInt64 *dest,
																   ILInt64 value,
																   ILInt64 comparand)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedCompareAndExchangeI8(dest, value, comparand);
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL)
static IL_INLINE ILInt64 ILInterlockedCompareAndExchangeI8_Full(volatile ILInt64 *dest,
																ILInt64 value,
																ILInt64 comparand)
{
	ILInt64 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedCompareAndExchangeI8(dest, value, comparand);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8) */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE)
#define ILInterlockedCompareAndExchangeI8_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE)
#define ILInterlockedCompareAndExchangeI8_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8)
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE)
#define ILInterlockedCompareAndExchangeI8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Acquire((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE)
#define ILInterlockedCompareAndExchangeI8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Release((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL)
#define ILInterlockedCompareAndExchangeI8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeI8_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8) */

/*
 * Compare and exchange two unsigned 64 bit integers.
 */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE)
static IL_INLINE ILUInt64 ILInterlockedCompareAndExchangeU8_Acquire(volatile ILUInt64 *dest,
																	ILUInt64 value,
																	ILUInt64 comparand)
{
	ILUInt64 retval;

	retval = ILInterlockedCompareAndExchangeU8(dest, value, comparand);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE)
static IL_INLINE ILUInt64 ILInterlockedCompareAndExchangeU8_Release(volatile ILUInt64 *dest,
																	ILUInt64 value,
																	ILUInt64 comparand)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedCompareAndExchangeU8(dest, value, comparand);
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL)
static IL_INLINE ILUInt64 ILInterlockedCompareAndExchangeU8_Full(volatile ILUInt64 *dest,
																 ILUInt64 value,
																 ILUInt64 comparand)
{
	ILUInt64 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedCompareAndExchangeU8(dest, value, comparand);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8) */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE)
#define ILInterlockedCompareAndExchangeU8_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU8_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE)
#define ILInterlockedCompareAndExchangeU8_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU8_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8)
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE)
#define ILInterlockedCompareAndExchangeU8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU8_Acquire((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE)
#define ILInterlockedCompareAndExchangeU8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU8_Release((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL)
#define ILInterlockedCompareAndExchangeU8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeU8_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8) */

/*
 * Compare and exchange two double precision floatingpoint values.
 */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_ACQUIRE)
static IL_INLINE ILDouble ILInterlockedCompareAndExchangeR8_Acquire(volatile ILDouble *dest,
																	ILDouble value,
																	ILDouble comparand)
{
	ILDouble retval;

	retval = ILInterlockedCompareAndExchangeR8(dest, value, comparand);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_RELEASE)
static IL_INLINE ILDouble ILInterlockedCompareAndExchangeR8_Release(volatile ILDouble *dest,
																	ILDouble value,
																	ILDouble comparand)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedCompareAndExchangeR8(dest, value, comparand);
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL)
static IL_INLINE ILDouble ILInterlockedCompareAndExchangeR8_Full(volatile ILDouble *dest,
																 ILDouble value,
																 ILDouble comparand)
{
	ILDouble retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedCompareAndExchangeR8(dest, value, comparand);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8) */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_ACQUIRE)
#define ILInterlockedCompareAndExchangeR8_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeR8_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_RELEASE)
#define ILInterlockedCompareAndExchangeR8_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeR8_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8)
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_ACQUIRE)
#define ILInterlockedCompareAndExchangeR8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeRU8_Acquire((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_RELEASE)
#define ILInterlockedCompareAndExchangeR8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeR8_Release((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL)
#define ILInterlockedCompareAndExchangeR8(dest, value, comparand) \
		ILInterlockedCompareAndExchangeR8_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8) */

/*
 * Compare and exchange two pointers.
 */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE)
static IL_INLINE void *ILInterlockedCompareAndExchangeP_Acquire(void * volatile *dest,
																void *value,
																void *comparand)
{
	void *retval;

	retval = ILInterlockedCompareAndExchangeP(dest, value, comparand);
	ILInterlockedMemoryBarrier();
	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE)
static IL_INLINE void *ILInterlockedCompareAndExchangeP_Release(void * volatile *dest,
																void *value,
																void *comparand)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedCompareAndExchangeP(dest, value, comparand);
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL)
static IL_INLINE void *ILInterlockedCompareAndExchangeP_Full(void * volatile *dest,
															 void *value,
															 void *comparand)
{
	void *retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedCompareAndExchangeP(dest, value, comparand);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP) */
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL)
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE)
#define ILInterlockedCompareAndExchangeP_Acquire(dest, value, comparand) \
		ILInterlockedCompareAndExchangeP_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE)
#define ILInterlockedCompareAndExchangeP_Release(dest, value, comparand) \
		ILInterlockedCompareAndExchangeP_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP)
#if defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE)
#define ILInterlockedCompareAndExchangeP(dest, value, comparand) \
		ILInterlockedCompareAndExchangeP_Acquire((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE)
#define ILInterlockedCompareAndExchangeP(dest, value, comparand) \
		ILInterlockedCompareAndExchangeP_Release((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP 1
#elif defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL)
#define ILInterlockedCompareAndExchangeP(dest, value, comparand) \
		ILInterlockedCompareAndExchangeP_Full((dest), (value), (comparand))
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP 1
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP) */

/*
 * Add the two signed 32bit integers *dest and value and store the result
 * at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_ADDI4)
#if !defined(IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE)
static IL_INLINE ILInt32 ILInterlockedAddI4_Acquire(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;

	retval = ILInterlockedAddI4(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI4_RELEASE)
static IL_INLINE ILInt32 ILInterlockedAddI4_Release(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedAddI4(dest, value);
}
#define IL_HAVE_INTERLOCKED_ADDI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI4_FULL)
static IL_INLINE ILInt32 ILInterlockedAddI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedAddI4(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_ADDI4) */
#if defined(IL_HAVE_INTERLOCKED_ADDI4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE)
#define ILInterlockedAddI4_Acquire(dest, value) \
		ILInterlockedAddI4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI4_RELEASE)
#define ILInterlockedAddI4_Release(dest, value) \
		ILInterlockedAddI4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_ADDI4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI4)
#if defined(IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE)
#define ILInterlockedAddI4(dest, value) \
		ILInterlockedAddI4_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI4 1
#elif defined(IL_HAVE_INTERLOCKED_ADDI4_RELEASE)
#define ILInterlockedAddI4(dest, value) \
		ILInterlockedAddI4_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI4 1
#elif defined(IL_HAVE_INTERLOCKED_ADDI4_FULL)
#define ILInterlockedAddI4(dest, value) \
		ILInterlockedAddI4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI4 1
#endif /* defined(IL_HAVE_INTERLOCKED_ADDI4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4) */

/*
 * Add the two signed 64bit integers *dest and value and store the result
 * at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_ADDI8)
#if !defined(IL_HAVE_INTERLOCKED_ADDI8_ACQUIRE)
static IL_INLINE ILInt64 ILInterlockedAddI8_Acquire(volatile ILInt64 *dest,
													ILInt64 value)
{
	ILInt64 retval;

	retval = ILInterlockedAddI8(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI8_RELEASE)
static IL_INLINE ILInt64 ILInterlockedAddI8_Release(volatile ILInt64 *dest,
													ILInt64 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedAddI8(dest, value);
}
#define IL_HAVE_INTERLOCKED_ADDI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI8_FULL)
static IL_INLINE ILInt64 ILInterlockedAddI8_Full(volatile ILInt64 *dest,
												 ILInt64 value)
{
	ILInt64 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedAddI8(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_ADDI8) */
#if defined(IL_HAVE_INTERLOCKED_ADDI8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_ADDI8_ACQUIRE)
#define ILInterlockedAddI8_Acquire(dest, value) \
		ILInterlockedAddI8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI8_RELEASE)
#define ILInterlockedAddI8_Release(dest, value) \
		ILInterlockedAddI8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_ADDI8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI8)
#if defined(IL_HAVE_INTERLOCKED_ADDI8_ACQUIRE)
#define ILInterlockedAddI8(dest, value) \
		ILInterlockedAddI8_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI8 1
#elif defined(IL_HAVE_INTERLOCKED_ADDI8_RELEASE)
#define ILInterlockedAddI8(dest, value) \
		ILInterlockedAddI8_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI8 1
#elif defined(IL_HAVE_INTERLOCKED_ADDI8_FULL)
#define ILInterlockedAddI8(dest, value) \
		ILInterlockedAddI8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ADDI8 1
#endif /* defined(IL_HAVE_INTERLOCKED_ADDI8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8) */

/*
 * Subtract the two signed 32bit integers *dest and value and store the
 * result at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_SUBI4)
#if !defined(IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE)
static IL_INLINE ILInt32 ILInterlockedSubI4_Acquire(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;

	retval = ILInterlockedSubI4(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI4_RELEASE)
static IL_INLINE ILInt32 ILInterlockedSubI4_Release(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedSubI4(dest, value);
}
#define IL_HAVE_INTERLOCKED_SUBI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI4_FULL)
static IL_INLINE ILInt32 ILInterlockedSubI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedSubI4(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_SUBI4) */
#if defined(IL_HAVE_INTERLOCKED_SUBI4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE)
#define ILInterlockedSubI4_Acquire(dest, value) \
		ILInterlockedSubI4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI4_RELEASE)
#define ILInterlockedSubI4_Release(dest, value) \
		ILInterlockedSubI4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_SUBI4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI4)
#if defined(IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE)
#define ILInterlockedSubI4(dest, value) \
		ILInterlockedSubI4_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI4 1
#elif defined(IL_HAVE_INTERLOCKED_SUBI4_RELEASE)
#define ILInterlockedSubI4(dest, value) \
		ILInterlockedSubI4_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI4 1
#elif defined(IL_HAVE_INTERLOCKED_SUBI4_FULL)
#define ILInterlockedSubI4(dest, value) \
		ILInterlockedSubI4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI4 1
#endif /* defined(IL_HAVE_INTERLOCKED_SUBI4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4) */

/*
 * Subtract the two signed 64bit integers *dest and value and store the
 * result at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_SUBI8)
#if !defined(IL_HAVE_INTERLOCKED_SUBI8_ACQUIRE)
static IL_INLINE ILInt64 ILInterlockedSubI8_Acquire(volatile ILInt64 *dest,
													ILInt64 value)
{
	ILInt64 retval;

	retval = ILInterlockedSubI8(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI8_RELEASE)
static IL_INLINE ILInt64 ILInterlockedSubI8_Release(volatile ILInt64 *dest,
													ILInt64 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedSubI8(dest, value);
}
#define IL_HAVE_INTERLOCKED_SUBI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI8_FULL)
static IL_INLINE ILInt64 ILInterlockedSubI8_Full(volatile ILInt64 *dest,
												 ILInt64 value)
{
	ILInt64 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedSubI8(dest, value);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_SUBI8) */
#if defined(IL_HAVE_INTERLOCKED_SUBI8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_SUBI8_ACQUIRE)
#define ILInterlockedSubI8_Acquire(dest, value) \
		ILInterlockedSubI8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI8_RELEASE)
#define ILInterlockedSubI8_Release(dest, value) \
		ILInterlockedSubI8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_SUBI8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI8)
#if defined(IL_HAVE_INTERLOCKED_SUBI8_ACQUIRE)
#define ILInterlockedSubI8(dest, value) \
		ILInterlockedSubI8_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI8 1
#elif defined(IL_HAVE_INTERLOCKED_SUBI8_RELEASE)
#define ILInterlockedSubI8(dest, value) \
		ILInterlockedSubI8_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI8 1
#elif defined(IL_HAVE_INTERLOCKED_SUBI8_FULL)
#define ILInterlockedSubI8(dest, value) \
		ILInterlockedSubI8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_SUBI8 1
#endif /* defined(IL_HAVE_INTERLOCKED_SUBI8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8) */

/*
 * Increment the signed 32bit integer at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_INCREMENTI4)
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE)
static IL_INLINE ILInt32 ILInterlockedIncrementI4_Acquire(volatile ILInt32 *dest)
{
	ILInt32 retval;

	retval = ILInterlockedIncrementI4(dest);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE)
static IL_INLINE ILInt32 ILInterlockedIncrementI4_Release(volatile ILInt32 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedIncrementI4(dest);
}
#define IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_FULL)
static IL_INLINE ILInt32 ILInterlockedIncrementI4_Full(volatile ILInt32 *dest)
{
	ILInt32 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedIncrementI4(dest);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_INCREMENTI4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_INCREMENTI4) */
#if defined(IL_HAVE_INTERLOCKED_INCREMENTI4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE)
#define ILInterlockedIncrementI4_Acquire(dest) \
		ILInterlockedIncrementI4_Full((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE)
#define ILInterlockedIncrementI4_Release(dest) \
		ILInterlockedIncrementI4_Full((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_INCREMENTI4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4)
#if defined(IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE)
#define ILInterlockedIncrementI4(dest) \
		ILInterlockedIncrementI4_Acquire((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI4 1
#elif defined(IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE)
#define ILInterlockedIncrementI4(dest) \
		ILInterlockedIncrementI4_Release((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI4 1
#elif defined(IL_HAVE_INTERLOCKED_INCREMENTI4_FULL)
#define ILInterlockedIncrementI4(dest) \
		ILInterlockedIncrementI4_Full((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI4 1
#endif /* defined(IL_HAVE_INTERLOCKED_INCREMENTI4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4) */

/*
 * Increment the signed 64bit integer at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_INCREMENTI8)
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_ACQUIRE)
static IL_INLINE ILInt64 ILInterlockedIncrementI8_Acquire(volatile ILInt64 *dest)
{
	ILInt64 retval;

	retval = ILInterlockedIncrementI8(dest);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_INCREMENTI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_RELEASE)
static IL_INLINE ILInt64 ILInterlockedIncrementI8_Release(volatile ILInt64 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedIncrementI8(dest);
}
#define IL_HAVE_INTERLOCKED_INCREMENTI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_FULL)
static IL_INLINE ILInt64 ILInterlockedIncrementI8_Full(volatile ILInt64 *dest)
{
	ILInt64 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedIncrementI8(dest);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_INCREMENTI8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_INCREMENTI8) */
#if defined(IL_HAVE_INTERLOCKED_INCREMENTI8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_ACQUIRE)
#define ILInterlockedIncrementI8_Acquire(dest) \
		ILInterlockedIncrementI8_Full((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_RELEASE)
#define ILInterlockedIncrementI8_Release(dest) \
		ILInterlockedIncrementI8_Full((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_INCREMENTI8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8)
#if defined(IL_HAVE_INTERLOCKED_INCREMENTI8_ACQUIRE)
#define ILInterlockedIncrementI8(dest) \
		ILInterlockedIncrementI8_Acquire((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI8 1
#elif defined(IL_HAVE_INTERLOCKED_INCREMENTI8_RELEASE)
#define ILInterlockedIncrementI8(dest) \
		ILInterlockedIncrementI8_Release((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI8 1
#elif defined(IL_HAVE_INTERLOCKED_INCREMENTI8_FULL)
#define ILInterlockedIncrementI8(dest) \
		ILInterlockedIncrementI8_Full((dest))
#define IL_HAVE_INTERLOCKED_INCREMENTI8 1
#endif /* defined(IL_HAVE_INTERLOCKED_INCREMENTI8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8) */

/*
 * Decrement the signed 32bit integer at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_DECREMENTI4)
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE)
static IL_INLINE ILInt32 ILInterlockedDecrementI4_Acquire(volatile ILInt32 *dest)
{
	ILInt32 retval;

	retval = ILInterlockedDecrementI4(dest);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE)
static IL_INLINE ILInt32 ILInterlockedDecrementI4_Release(volatile ILInt32 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedDecrementI4(dest);
}
#define IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_FULL)
static IL_INLINE ILInt32 ILInterlockedDecrementI4_Full(volatile ILInt32 *dest)
{
	ILInt32 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedDecrementI4(dest);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_DECREMENTI4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_DECREMENTI4) */
#if defined(IL_HAVE_INTERLOCKED_DECREMENTI4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE)
#define ILInterlockedDecrementI4_Acquire(dest) \
		ILInterlockedDecrementI4_Full((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE)
#define ILInterlockedDecrementI4_Release(dest) \
		ILInterlockedDecrementI4_Full((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_DECREMENTI4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4)
#if defined(IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE)
#define ILInterlockedDecrementI4(dest) \
		ILInterlockedDecrementI4_Acquire((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI4 1
#elif defined(IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE)
#define ILInterlockedDecrementI4(dest) \
		ILInterlockedDecrementI4_Release((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI4 1
#elif defined(IL_HAVE_INTERLOCKED_DECREMENTI4_FULL)
#define ILInterlockedDecrementI4(dest) \
		ILInterlockedDecrementI4_Full((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI4 1
#endif /* defined(IL_HAVE_INTERLOCKED_DECREMENTI4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4) */

/*
 * Decrement the signed 64bit integer at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_DECREMENTI8)
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_ACQUIRE)
static IL_INLINE ILInt64 ILInterlockedDecrementI8_Acquire(volatile ILInt64 *dest)
{
	ILInt64 retval;

	retval = ILInterlockedDecrementI8(dest);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_DECREMENTI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_RELEASE)
static IL_INLINE ILInt64 ILInterlockedDecrementI8_Release(volatile ILInt64 *dest)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedDecrementI8(dest);
}
#define IL_HAVE_INTERLOCKED_DECREMENTI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_FULL)
static IL_INLINE ILInt64 ILInterlockedDecrementI8_Full(volatile ILInt64 *dest)
{
	ILInt64 retval;

	ILInterlockedMemoryBarrier();
	retval = ILInterlockedDecrementI8(dest);
	ILInterlockedMemoryBarrier();

	return retval;
}
#define IL_HAVE_INTERLOCKED_DECREMENTI8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_DECREMENTI8) */
#if defined(IL_HAVE_INTERLOCKED_DECREMENTI8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_ACQUIRE)
#define ILInterlockedDecrementI8_Acquire(dest) \
		ILInterlockedDecrementI8_Full((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_RELEASE)
#define ILInterlockedDecrementI8_Release(dest) \
		ILInterlockedDecrementI8_Full((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_DECREMENTI8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8)
#if defined(IL_HAVE_INTERLOCKED_DECREMENTI8_ACQUIRE)
#define ILInterlockedDecrementI8(dest) \
		ILInterlockedDecrementI8_Acquire((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI8 1
#elif defined(IL_HAVE_INTERLOCKED_DECREMENTI8_RELEASE)
#define ILInterlockedDecrementI8(dest) \
		ILInterlockedDecrementI8_Release((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI8 1
#elif defined(IL_HAVE_INTERLOCKED_DECREMENTI8_FULL)
#define ILInterlockedDecrementI8(dest) \
		ILInterlockedDecrementI8_Full((dest))
#define IL_HAVE_INTERLOCKED_DECREMENTI8 1
#endif /* defined(IL_HAVE_INTERLOCKED_DECREMENTI8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8) */

/*
 * Bitwise and of the two unsigned 32bit integers *dest and value and store
 * the result at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_ANDU4)
#if !defined(IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE)
static IL_INLINE void ILInterlockedAndU4_Acquire(volatile ILUInt32 *dest,
												 ILUInt32 value)
{
	ILInterlockedAndU4(dest, value);
	ILInterlockedMemoryBarrier();
}
#define IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU4_RELEASE)
static IL_INLINE void ILInterlockedAndU4_Release(volatile ILUInt32 *dest,
												 ILUInt32 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedAndU4(dest, value);
}
#define IL_HAVE_INTERLOCKED_ANDU4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU4_FULL)
static IL_INLINE void ILInterlockedAndU4_Full(volatile ILUInt32 *dest,
											  ILUInt32 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedAndU4(dest, value);
	ILInterlockedMemoryBarrier();
}
#define IL_HAVE_INTERLOCKED_ANDU4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_ANDU4) */
#if defined(IL_HAVE_INTERLOCKED_ANDU4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE)
#define ILInterlockedAndU4_Acquire(dest, value) \
		ILInterlockedAndU4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU4_RELEASE)
#define ILInterlockedAndU4_Release(dest, value) \
		ILInterlockedAndU4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_ANDU4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU4)
#if defined(IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE)
#define ILInterlockedAndU4(dest, value) \
		ILInterlockedAndU4_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU4 1
#elif defined(IL_HAVE_INTERLOCKED_ANDU4_RELEASE)
#define ILInterlockedAndU4(dest, value) \
		ILInterlockedAndU4_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU4 1
#elif defined(IL_HAVE_INTERLOCKED_ANDU4_FULL)
#define ILInterlockedAndU4(dest, value) \
		ILInterlockedAndU4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU4 1
#endif /* defined(IL_HAVE_INTERLOCKED_ANDU4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4) */

/*
 * Bitwise and of the two unsigned 64bit integers *dest and value and store
 * the result at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_ANDU8)
#if !defined(IL_HAVE_INTERLOCKED_ANDU8_ACQUIRE)
static IL_INLINE void ILInterlockedAndU8_Acquire(volatile ILUInt64 *dest,
												 ILUInt64 value)
{
	ILInterlockedAndU8(dest, value);
	ILInterlockedMemoryBarrier();
}
#define IL_HAVE_INTERLOCKED_ANDU8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU8_RELEASE)
static IL_INLINE void ILInterlockedAndU8_Release(volatile ILUInt64 *dest,
												 ILUInt64 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedAndU8(dest, value);
}
#define IL_HAVE_INTERLOCKED_ANDU8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU8_FULL)
static IL_INLINE void ILInterlockedAndU8_Full(volatile ILUInt64 *dest,
											  ILUInt64 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedAndU8(dest, value);
	ILInterlockedMemoryBarrier();
}
#define IL_HAVE_INTERLOCKED_ANDU8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_ANDU8) */
#if defined(IL_HAVE_INTERLOCKED_ANDU8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_ANDU8_ACQUIRE)
#define ILInterlockedAndU8_Acquire(dest, value) \
		ILInterlockedAndU8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU8_RELEASE)
#define ILInterlockedAndU8_Release(dest, value) \
		ILInterlockedAndU8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_ANDU8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU8)
#if defined(IL_HAVE_INTERLOCKED_ANDU8_ACQUIRE)
#define ILInterlockedAndU8(dest, value) \
		ILInterlockedAndU8_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU8 1
#elif defined(IL_HAVE_INTERLOCKED_ANDU8_RELEASE)
#define ILInterlockedAndU8(dest, value) \
		ILInterlockedAndU8_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU8 1
#elif defined(IL_HAVE_INTERLOCKED_ANDU8_FULL)
#define ILInterlockedAndU8(dest, value) \
		ILInterlockedAndU8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ANDU8 1
#endif /* defined(IL_HAVE_INTERLOCKED_ANDU8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8) */

/*
 * Bitwise or of the two unsigned 32bit integers *dest and value and store
 * the result at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_ORU4)
#if !defined(IL_HAVE_INTERLOCKED_ORU4_ACQUIRE)
static IL_INLINE void ILInterlockedOrU4_Acquire(volatile ILUInt32 *dest,
												 ILUInt32 value)
{
	ILInterlockedOrU4(dest, value);
	ILInterlockedMemoryBarrier();
}
#define IL_HAVE_INTERLOCKED_ORU4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU4_RELEASE)
static IL_INLINE void ILInterlockedOrU4_Release(volatile ILUInt32 *dest,
												 ILUInt32 value)
{
	ILInterlockedMemoryBarrier();
	return ILInterlockedOrU4(dest, value);
}
#define IL_HAVE_INTERLOCKED_ORU4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU4_FULL)
static IL_INLINE void ILInterlockedOrU4_Full(volatile ILUInt32 *dest,
											  ILUInt32 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedOrU4(dest, value);
	ILInterlockedMemoryBarrier();
}
#define IL_HAVE_INTERLOCKED_ORU4_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_ORU4) */
#if defined(IL_HAVE_INTERLOCKED_ORU4_FULL)
#if !defined(IL_HAVE_INTERLOCKED_ORU4_ACQUIRE)
#define ILInterlockedOrU4_Acquire(dest, value) \
		ILInterlockedOrU4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU4_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU4_RELEASE)
#define ILInterlockedOrU4_Release(dest, value) \
		ILInterlockedOrU4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU4_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_ORU4_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_ORU4)
#if defined(IL_HAVE_INTERLOCKED_ORU4_ACQUIRE)
#define ILInterlockedOrU4(dest, value) \
		ILInterlockedOrU4_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU4 1
#elif defined(IL_HAVE_INTERLOCKED_ORU4_RELEASE)
#define ILInterlockedOrU4(dest, value) \
		ILInterlockedOrU4_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU4 1
#elif defined(IL_HAVE_INTERLOCKED_ORU4_FULL)
#define ILInterlockedOrU4(dest, value) \
		ILInterlockedOrU4_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU4 1
#endif /* defined(IL_HAVE_INTERLOCKED_ORU4_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4) */

/*
 * Bitwise and of the two unsigned 64bit integers *dest and value and store
 * the result at *dest.
 */
#if defined(IL_HAVE_INTERLOCKED_ORU8)
#if !defined(IL_HAVE_INTERLOCKED_ORU8_ACQUIRE)
static IL_INLINE void ILInterlockedOrU8_Acquire(volatile ILUInt64 *dest,
												 ILUInt64 value)
{
	ILInterlockedOrU8(dest, value);
	ILInterlockedMemoryBarrier();
}
#define IL_HAVE_INTERLOCKED_ORU8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU8_RELEASE)
static IL_INLINE void ILInterlockedOrU8_Release(volatile ILUInt64 *dest,
												 ILUInt64 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedOrU8(dest, value);
}
#define IL_HAVE_INTERLOCKED_ORU8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU8_FULL)
static IL_INLINE void ILInterlockedOrU8_Full(volatile ILUInt64 *dest,
											  ILUInt64 value)
{
	ILInterlockedMemoryBarrier();
	ILInterlockedOrU8(dest, value);
	ILInterlockedMemoryBarrier();
}
#define IL_HAVE_INTERLOCKED_ORU8_FULL 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8_FULL) */
#endif /* defined(IL_HAVE_INTERLOCKED_ORU8) */
#if defined(IL_HAVE_INTERLOCKED_ORU8_FULL)
#if !defined(IL_HAVE_INTERLOCKED_ORU8_ACQUIRE)
#define ILInterlockedOrU8_Acquire(dest, value) \
		ILInterlockedOrU8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU8_ACQUIRE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU8_RELEASE)
#define ILInterlockedOrU8_Release(dest, value) \
		ILInterlockedOrU8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU8_RELEASE 1
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8_RELEASE) */
#endif /* defined(IL_HAVE_INTERLOCKED_ORU8_FULL) */
#if !defined(IL_HAVE_INTERLOCKED_ORU8)
#if defined(IL_HAVE_INTERLOCKED_ORU8_ACQUIRE)
#define ILInterlockedOrU8(dest, value) \
		ILInterlockedOrU8_Acquire((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU8 1
#elif defined(IL_HAVE_INTERLOCKED_ORU8_RELEASE)
#define ILInterlockedOrU8(dest, value) \
		ILInterlockedOrU8_Release((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU8 1
#elif defined(IL_HAVE_INTERLOCKED_ORU8_FULL)
#define ILInterlockedOrU8(dest, value) \
		ILInterlockedOrU8_Full((dest), (value))
#define IL_HAVE_INTERLOCKED_ORU8 1
#endif /* defined(IL_HAVE_INTERLOCKED_ORU8_FULL) */
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8) */

/*
 * Backup declarations if no native implementation is available
 */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4)
#define ILInterlockedExchangeI4(dest, value) \
		_ILInterlockedExchangeI4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE)
#define ILInterlockedExchangeI4_Acquire(dest, value) \
		_ILInterlockedExchangeI4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE)
#define ILInterlockedExchangeI4_Release(dest, value) \
		_ILInterlockedExchangeI4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL)
#define ILInterlockedExchangeI4_Full(dest, value) \
		_ILInterlockedExchangeI4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4)
#define ILInterlockedExchangeU4(dest, value) \
		_ILInterlockedExchangeU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE)
#define ILInterlockedExchangeU4_Acquire(dest, value) \
		_ILInterlockedExchangeU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE)
#define ILInterlockedExchangeU4_Release(dest, value) \
		_ILInterlockedExchangeU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL)
#define ILInterlockedExchangeU4_Full(dest, value) \
		_ILInterlockedExchangeU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4)
#define ILInterlockedExchangeR4(dest, value) \
		_ILInterlockedExchangeR4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_ACQUIRE)
#define ILInterlockedExchangeR4_Acquire(dest, value) \
		_ILInterlockedExchangeR4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_RELEASE)
#define ILInterlockedExchangeR4_Release(dest, value) \
		_ILInterlockedExchangeR4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_FULL)
#define ILInterlockedExchangeR4_Full(dest, value) \
		_ILInterlockedExchangeR4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8)
#define ILInterlockedExchangeI8(dest, value) \
		_ILInterlockedExchangeI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE)
#define ILInterlockedExchangeI8_Acquire(dest, value) \
		_ILInterlockedExchangeI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE)
#define ILInterlockedExchangeI8_Release(dest, value) \
		_ILInterlockedExchangeI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL)
#define ILInterlockedExchangeI8_Full(dest, value) \
		_ILInterlockedExchangeI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEI8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8)
#define ILInterlockedExchangeU8(dest, value) \
		_ILInterlockedExchangeU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE)
#define ILInterlockedExchangeU8_Acquire(dest, value) \
		_ILInterlockedExchangeU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE)
#define ILInterlockedExchangeU8_Release(dest, value) \
		_ILInterlockedExchangeU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL)
#define ILInterlockedExchangeU8_Full(dest, value) \
		_ILInterlockedExchangeU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGEU8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8)
#define ILInterlockedExchangeR8(dest, value) \
		_ILInterlockedExchangeR8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_ACQUIRE)
#define ILInterlockedExchangeR8_Acquire(dest, value) \
		_ILInterlockedExchangeR8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_RELEASE)
#define ILInterlockedExchangeR8_Release(dest, value) \
		_ILInterlockedExchangeR8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_FULL)
#define ILInterlockedExchangeR8_Full(dest, value) \
		_ILInterlockedExchangeR8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_EXCHANGER8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4)
#define ILInterlockedCompareAndExchangeI4(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeI4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE)
#define ILInterlockedCompareAndExchangeI4_Acquire(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeI4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE)
#define ILInterlockedCompareAndExchangeI4_Release(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeI4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL)
#define ILInterlockedCompareAndExchangeI4_Full(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeI4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4)
#define ILInterlockedCompareAndExchangeU4(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeU4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE)
#define ILInterlockedCompareAndExchangeU4_Acquire(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeU4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE)
#define ILInterlockedCompareAndExchangeU4_Release(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeU4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL)
#define ILInterlockedCompareAndExchangeU4_Full(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeU4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4)
#define ILInterlockedCompareAndExchangeR4(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeR4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_ACQUIRE)
#define ILInterlockedCompareAndExchangeR4_Acquire(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeR4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_RELEASE)
#define ILInterlockedCompareAndExchangeR4_Release(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeR4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL)
#define ILInterlockedCompareAndExchangeR4_Full(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeR4_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8)
#define ILInterlockedCompareAndExchangeI8(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeI8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE)
#define ILInterlockedCompareAndExchangeI8_Acquire(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeI8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE)
#define ILInterlockedCompareAndExchangeI8_Release(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeI8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL)
#define ILInterlockedCompareAndExchangeI8_Full(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeI8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8)
#define ILInterlockedCompareAndExchangeU8(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeU8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE)
#define ILInterlockedCompareAndExchangeU8_Acquire(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeU8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE)
#define ILInterlockedCompareAndExchangeU8_Release(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeU8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL)
#define ILInterlockedCompareAndExchangeU8_Full(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeU8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEU8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8)
#define ILInterlockedCompareAndExchangeR8(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeR8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_ACQUIRE)
#define ILInterlockedCompareAndExchangeR8_Acquire(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeR8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_RELEASE)
#define ILInterlockedCompareAndExchangeR8_Release(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeR8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL)
#define ILInterlockedCompareAndExchangeR8_Full(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeR8_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGER8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP)
#define ILInterlockedCompareAndExchangeP(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeP_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE)
#define ILInterlockedCompareAndExchangeP_Acquire(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeP_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE)
#define ILInterlockedCompareAndExchangeP_Release(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeP_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL)
#define ILInterlockedCompareAndExchangeP_Full(dest, value, comparand) \
		_ILInterlockedCompareAndExchangeP_Full((dest), (value), (comparand))
#endif /* !defined(IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_ADDI4)
#define ILInterlockedAddI4(dest, value) \
		_ILInterlockedAddI4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE)
#define ILInterlockedAddI4_Acquire(dest, value) \
		_ILInterlockedAddI4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI4_RELEASE)
#define ILInterlockedAddI4_Release(dest, value) \
		_ILInterlockedAddI4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI4_FULL)
#define ILInterlockedAddI4_Full(dest, value) \
		_ILInterlockedAddI4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_ADDI8)
#define ILInterlockedAddI8(dest, value) \
		_ILInterlockedAddI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI8_ACQUIRE)
#define ILInterlockedAddI8_Acquire(dest, value) \
		_ILInterlockedAddI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI8_RELEASE)
#define ILInterlockedAddI8_Release(dest, value) \
		_ILInterlockedAddI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ADDI8_FULL)
#define ILInterlockedAddI8_Full(dest, value) \
		_ILInterlockedAddI8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ADDI8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_SUBI4)
#define ILInterlockedSubI4(dest, value) \
		_ILInterlockedAddI4_Full((dest), -(value))
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE)
#define ILInterlockedSubI4_Acquire(dest, value) \
		_ILInterlockedAddI4_Full((dest), -(value))
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI4_RELEASE)
#define ILInterlockedSubI4_Release(dest, value) \
		_ILInterlockedAddI4_Full((dest), -(value))
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI4_FULL)
#define ILInterlockedSubI4_Full(dest, value) \
		_ILInterlockedAddI4_Full((dest), -(value))
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_SUBI8)
#define ILInterlockedSubI8(dest, value) \
		_ILInterlockedAddI8_Full((dest), -(value))
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI8_ACQUIRE)
#define ILInterlockedSubI8_Acquire(dest, value) \
		_ILInterlockedAddI8_Full((dest), -(value))
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI8_RELEASE)
#define ILInterlockedSubI8_Release(dest, value) \
		_ILInterlockedAddI8_Full((dest), -(value))
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_SUBI8_FULL)
#define ILInterlockedSubI8_Full(dest, value) \
		_ILInterlockedAddI8_Full((dest), -(value))
#endif /* !defined(IL_HAVE_INTERLOCKED_SUBI8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4)
#define ILInterlockedIncrementI4(dest) \
		_ILInterlockedAddI4_Full((dest), 1)
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE)
#define ILInterlockedIncrementI4_Acquire(dest) \
		_ILInterlockedAddI4_Full((dest), 1)
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE)
#define ILInterlockedIncrementI4_Release(dest) \
		_ILInterlockedAddI4_Full((dest), 1)
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_FULL)
#define ILInterlockedIncrementI4_Full(dest) \
		_ILInterlockedAddI4_Full((dest), 1)
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8)
#define ILInterlockedIncrementI8(dest) \
		_ILInterlockedAddI8_Full((dest), 1)
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_ACQUIRE)
#define ILInterlockedIncrementI8_Acquire(dest) \
		_ILInterlockedAddI8_Full((dest), 1)
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_RELEASE)
#define ILInterlockedIncrementI8_Release(dest) \
		_ILInterlockedAddI8_Full((dest), 1)
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_FULL)
#define ILInterlockedIncrementI8_Full(dest) \
		_ILInterlockedAddI8_Full((dest), 1)
#endif /* !defined(IL_HAVE_INTERLOCKED_INCREMENTI8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4)
#define ILInterlockedDecrementI4(dest) \
		_ILInterlockedAddI4_Full((dest), -1)
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE)
#define ILInterlockedDecrementI4_Acquire(dest) \
		_ILInterlockedAddI4_Full((dest), -1)
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE)
#define ILInterlockedDecrementI4_Release(dest) \
		_ILInterlockedAddI4_Full((dest), -1)
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_FULL)
#define ILInterlockedDecrementI4_Full(dest) \
		_ILInterlockedAddI4_Full((dest), -1)
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8)
#define ILInterlockedDecrementI8(dest) \
		_ILInterlockedAddI8_Full((dest), -1)
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_ACQUIRE)
#define ILInterlockedDecrementI8_Acquire(dest) \
		_ILInterlockedAddI8_Full((dest), -1)
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_RELEASE)
#define ILInterlockedDecrementI8_Release(dest) \
		_ILInterlockedAddI8_Full((dest), -1)
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_FULL)
#define ILInterlockedDecrementI8_Full(dest) \
		_ILInterlockedAddI8_Full((dest), -1)
#endif /* !defined(IL_HAVE_INTERLOCKED_DECREMENTI8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_ANDU4)
#define ILInterlockedAndU4(dest, value) \
		_ILInterlockedAndU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE)
#define ILInterlockedAndU4_Acquire(dest, value) \
		_ILInterlockedAndU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU4_RELEASE)
#define ILInterlockedAndU4_Release(dest, value) \
		_ILInterlockedAndU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU4_FULL)
#define ILInterlockedAndU4_Full(dest, value) \
		_ILInterlockedAndU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_ANDU8)
#define ILInterlockedAndU8(dest, value) \
		_ILInterlockedAndU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU8_ACQUIRE)
#define ILInterlockedAndU8_Acquire(dest, value) \
		_ILInterlockedAndU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU8_RELEASE)
#define ILInterlockedAndU8_Release(dest, value) \
		_ILInterlockedAndU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ANDU8_FULL)
#define ILInterlockedAndU8_Full(dest, value) \
		_ILInterlockedAndU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ANDU8_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_ORU4)
#define ILInterlockedOrU4(dest, value) \
		_ILInterlockedOrU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4) */
#if !defined(IL_HAVE_INTERLOCKED_ORU4_ACQUIRE)
#define ILInterlockedOrU4_Acquire(dest, value) \
		_ILInterlockedOrU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU4_RELEASE)
#define ILInterlockedOrU4_Release(dest, value) \
		_ILInterlockedOrU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU4_FULL)
#define ILInterlockedOrU4_Full(dest, value) \
		_ILInterlockedOrU4_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU4_FULL) */

#if !defined(IL_HAVE_INTERLOCKED_ORU8)
#define ILInterlockedOrU8(dest, value) \
		_ILInterlockedOrU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8) */
#if !defined(IL_HAVE_INTERLOCKED_ORU8_ACQUIRE)
#define ILInterlockedOrU8_Acquire(dest, value) \
		_ILInterlockedOrU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8_ACQUIRE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU8_RELEASE)
#define ILInterlockedOrU8_Release(dest, value) \
		_ILInterlockedOrU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8_RELEASE) */
#if !defined(IL_HAVE_INTERLOCKED_ORU8_FULL)
#define ILInterlockedOrU8_Full(dest, value) \
		_ILInterlockedOrU8_Full((dest), (value))
#endif /* !defined(IL_HAVE_INTERLOCKED_ORU8_FULL) */
