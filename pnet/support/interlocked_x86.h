/*
 * interlocked_x86.h - Implementation of interlocked functions for
 * intel processors.
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

#if (defined(__i386) || defined(__i386__) || defined(__x86_64__))

#if defined(__GNUC__)

#if defined(__SSE2__) || defined(__sse2__)
#define _IL_INTERLOCKED_X86_MFENCE	"mfence;"
#else
#define _IL_INTERLOCKED_X86_MFENCE	"lock; addl $0,0(%%esp);"
#endif

/*
 * Flush cache and set a memory barrier.
 */
static IL_INLINE void ILInterlockedMemoryBarrier()
{
	__asm__ __volatile__
	(
		_IL_INTERLOCKED_X86_MFENCE
		:::
		"memory"
	);
}
#define IL_HAVE_INTERLOCKED_MEMORYBARRIER 1

/*
 * NOTE: All operations on x86 using the lock prefix have full barrier
 * semantics.
 */

/*
 * Load a 32 bit value from a location with acquire semantics.
 */
static IL_INLINE ILInt32 ILInterlockedLoadI4_Acquire(const volatile ILInt32 *dest)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
	    "movl	%1, %0;"
		_IL_INTERLOCKED_X86_MFENCE
	    : "=r" (retval)
	    : "m" (*dest)
		: "memory"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_LOADI3_ACQUIRE

/*
 * Load a pointer value from a location with acquire semantics.
 */
static IL_INLINE void * ILInterlockedLoadP_Acquire(void * const volatile *dest)
{
	void *retval;

	__asm__ __volatile__ 
	(
#if defined(__x86_64__)
	    "movq	%1, %0;"
#else
	    "movl	%1, %0;"
#endif
		_IL_INTERLOCKED_X86_MFENCE
	    : "=r" (retval)
	    : "m" (*dest)
		: "memory"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_LOADP_ACQUIRE

/*
 * Store a 32 bit value to a location with release semantics.
 */
static IL_INLINE void ILInterlockedStoreI4_Release(volatile ILInt32 *dest,
												 ILInt32 value)
{
	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_X86_MFENCE
		"movl	%1, %0;"
		: "=m" (*dest)
		: "er" (value)
		: "memory"
	);
}
#define IL_HAVE_INTERLOCKED_STOREI4_RELEASE 1

/*
 * Store a pointer value to a location with release semantics.
 */
static IL_INLINE void ILInterlockedStoreP_Release(void * volatile *dest,
												   void *value)
{
	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_X86_MFENCE
#if defined(__x86_64__)
		"movq	%1, %0;"
#else
		"movl	%1, %0;"
#endif
		: "=m" (*dest)
		: "er" (value)
		: "memory"
	);
}
#define IL_HAVE_INTERLOCKED_STOREP_RELEASE 1

/*
 * Exchange two 32 bit integers.
 */
static IL_INLINE ILInt32 ILInterlockedExchangeI4_Full(volatile ILInt32 *dest,
													  ILInt32 value)
{
	ILInt32 retval;

	/*
	 * NOTE: xchg has an implicit lock if a memory operand is involved.
	 */
	__asm__ __volatile__ 
	(
		"xchgl %2, %0;"
		: "=m" (*dest), "=r" (retval)
		: "1" (value), "m" (*dest)
		: "memory"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL 1

/*
 * Exchange pointers.
 */
static IL_INLINE void *ILInterlockedExchangeP_Full(void * volatile *dest,
												   void *value)
{
	void *retval;

	/*
	 * NOTE: xchg has an implicit lock if a memory operand is involved.
	 */
	__asm__ __volatile__ 
	(
#if defined(__x86_64__)
		"xchgq %2, %0;"
#else
		"xchgl %2, %0;"
#endif
		: "=m" (*dest), "=r" (retval)
		: "1" (value), "m" (*dest)
		: "memory"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP_FULL 1

/*
 * Compare and exchange two 32bit integers with full semantics.
 * x86 has full semantics with the lock prefix but gcc might move memory
 * loads before this statement so we have to add the "memory" clobber here.
 */
static IL_INLINE ILInt32 ILInterlockedCompareAndExchangeI4_Full(volatile ILInt32 *dest,
																ILInt32 value,
																ILInt32 comparand)
{
	ILInt32 retval;

	__asm__ __volatile__
	(
		"lock;"
		"cmpxchgl %2, %0"
		: "=m" (*dest), "=a" (retval)
		: "r" (value), "m" (*dest), "a" (comparand)
		: "memory"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL 1

/*
 * Compare and exchange two pointers with full semantics.
 * x86 has full semantics with the lock prefix but gcc might move memory
 * loads before this statement so we have to add the "memory" clobber here.
 */
static IL_INLINE void *ILInterlockedCompareAndExchangeP_Full(void * volatile *dest,
															 void *value,
															 void *comparand)
{
	void *retval;

	__asm__ __volatile__
	(
		"lock;"
#if defined(__x86_64__)
		"cmpxchgq %2, %0;"
#else
		"cmpxchgl %2, %0;"
#endif
		: "=m" (*dest), "=a" (retval)
		: "r" (value), "m" (*dest), "a" (comparand)
		: "memory"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL 1

#if defined(__x86_64__)
/*
 * Compare and exchange two 64bit integers with full semantics.
 * x86 has full semantics with the lock prefix but gcc might move memory
 * loads before this statement so we have to add the "memory" clobber here.
 */
static IL_INLINE ILInt64 ILInterlockedCompareAndExchangeI8_Full(volatile ILInt64 *dest,
																ILInt64 value,
																ILInt64 comparand)
{
	ILInt32 retval;

	__asm__ __volatile__
	(
		"lock;"
		"cmpxchgq %2, %0"
		: "=m" (*dest), "=a" (retval)
		: "r" (value), "m" (*dest), "a" (comparand)
		: "memory"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI8_FULL 1
#endif

/*
 * Add two 32 bit integer values.
 */
static IL_INLINE ILInt32 ILInterlockedAddI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		"lock;"
		"xaddl %0, %1"
		: "=r" (retval), "=m" (*dest)
		: "0" (value), "m" (*dest)
		: "memory"
	);

	return retval + value;
}
#define IL_HAVE_INTERLOCKED_ADDI4_FULL 1

/*
 * 32bit bitwise AND
 */
static IL_INLINE void ILInterlockedAndU4_Full(volatile ILUInt32 *dest,
											  ILUInt32 value)
{
	__asm__ __volatile__ 
	(
		"lock;"
		"andl %1, %0"
		: "=m" (*dest)
		: "er" (value), "m" (*dest)
		: "memory");
}
#define IL_HAVE_INTERLOCKED_ANDU4_FULL 1

/*
 * 32bit bitwise OR
 */
static IL_INLINE void ILInterlockedOrU4_Full(volatile ILUInt32 *dest,
											 ILUInt32 value)
{
	__asm__ __volatile__ 
	(
		"lock;"
		"orl %1, %0"
		: "=m" (*dest)
		: "er" (value), "m" (*dest)
		: "memory");
}
#define IL_HAVE_INTERLOCKED_ORU4_FULL 1

#endif /* defined(__GNUC__) */

#endif /* (defined(__i386) || defined(__i386__) || defined(__x86_64__)) */
