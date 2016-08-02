/*
 * interlocked_arm.h - Implementation of interlocked functions for
 * Arm processors.
 *
 * Copyright (C) 2009  Southern Storm Software, Pty Ltd.
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

#if defined(__arm) || defined(__arm__)

#if defined(__GNUC__)

/*
 * Define _IL_INTERLOCKED_ARM_MP to enable multiprocessor by default
 */
#define _IL_INTERLOCKED_ARM_MP 1

#if defined(__ARM_ARCH_6M__) || defined(__ARM_ARCH_7M__) || \
	defined(_IL_INTERLOCKED_ARM_MP)
/*
 * These are the multi core variants which will need an explicit memory barrier
 */
#define _IL_INTERLOCKED_ARM_MEMORYBARRIER	"\tmcr p15, 0, r0, c7, c10, 5\n"
#else /* !defined(__ARM_ARCH_6M__) && !defined(__ARM_ARCH_7M__) */
#define _IL_INTERLOCKED_ARM_MEMORYBARRIER	""
#endif /* !defined(__ARM_ARCH_6M__) && !defined(__ARM_ARCH_7M__) */

/*
 * Flush cache and set a memory barrier.
 */
static IL_INLINE void ILInterlockedMemoryBarrier()
{
	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		:
		:
		: "memory"
	);
}
#define IL_HAVE_INTERLOCKED_MEMORYBARRIER 1

#if defined(__ARM_ARCH_6__) || defined(__ARM_ARCH_6J__) || \
	defined(__ARM_ARCH_6K__) || defined(__ARM_ARCH_6ZK__) || \
	defined(__ARM_ARCH_7__) || defined(__ARM_ARCH_7A__) || \
	defined(__ARM_ARCH_7M__) || defined(__ARM_ARCH_7R__)

/*
 * The versions for the interlocked operations available for all arm cores
 * version 6 and later (except Armv6_M).
 */

/*
 * Exchange two 32 bit integers.
 */
static IL_INLINE ILInt32 ILInterlockedExchangeI4(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex  %0, [%3];"
		"strex  %1, %4, [%3];"
		"teq            %1, #0;"
		"bne            1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value)
		: "cc"
	);
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI4 1

static IL_INLINE ILInt32 ILInterlockedExchangeI4_Acquire(volatile ILInt32 *dest,
														 ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex  %0, [%3];"
		"strex  %1, %4, [%3];"
		"teq            %1, #0;"
		"bne            1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE 1

static IL_INLINE ILInt32 ILInterlockedExchangeI4_Release(volatile ILInt32 *dest,
														 ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex  %0, [%3];"
		"strex  %1, %4, [%3];"
		"teq            %1, #0;"
		"bne            1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE 1

static IL_INLINE ILInt32 ILInterlockedExchangeI4_Full(volatile ILInt32 *dest,
													  ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex  %0, [%3];"
		"strex  %1, %4, [%3];"
		"teq            %1, #0;"
		"bne            1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);
	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL 1

/*
 * Exchange pointers.
 */
static IL_INLINE void *ILInterlockedExchangeP(void * volatile *dest,
											  void *value)
{
	void *retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex  %0, [%3];"
		"strex  %1, %4, [%3];"
		"teq        %1, #0;"
		"bne        1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value)
		: "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP 1

static IL_INLINE void *ILInterlockedExchangeP_Acquire(void * volatile *dest,
													  void *value)
{
	void *retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex  %0, [%3];"
		"strex  %1, %4, [%3];"
		"teq        %1, #0;"
		"bne        1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE 1

static IL_INLINE void *ILInterlockedExchangeP_Release(void * volatile *dest,
													  void *value)
{
	void *retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex  %0, [%3];"
		"strex  %1, %4, [%3];"
		"teq        %1, #0;"
		"bne        1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE 1

static IL_INLINE void *ILInterlockedExchangeP_Full(void * volatile *dest,
												   void *value)
{
	void *retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex  %0, [%3];"
		"strex  %1, %4, [%3];"
		"teq        %1, #0;"
		"bne        1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP_FULL 1

/*
 * Compare and exchange two 32bit integers.
 */
static IL_INLINE ILInt32 ILInterlockedCompareAndExchangeI4(volatile ILInt32 *dest,
														   ILInt32 value,
														   ILInt32 comparand)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex			%0, [%3];"
		"teq			%0, %5;"
		"bne			2f;"
		"strex		%1, %4, [%3];"
		"teq			%1, #0;"
		"bne			1b;"
		"2:"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value), "Jr" (comparand)
		: "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4 1

static IL_INLINE ILInt32 ILInterlockedCompareAndExchangeI4_Acquire(volatile ILInt32 *dest,
																   ILInt32 value,
																   ILInt32 comparand)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex			%0, [%3];"
		"teq			%0, %5;"
		"bne			2f;"
		"strex		%1, %4, [%3];"
		"teq			%1, #0;"
		"bne			1b;"
		"2:"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value), "Jr" (comparand)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE 1

static IL_INLINE ILInt32 ILInterlockedCompareAndExchangeI4_Release(volatile ILInt32 *dest,
																   ILInt32 value,
																   ILInt32 comparand)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex			%0, [%3];"
		"teq			%0, %5;"
		"bne			2f;"
		"strex		%1, %4, [%3];"
		"teq			%1, #0;"
		"bne			1b;"
		"2:"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value), "Jr" (comparand)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE 1

static IL_INLINE ILInt32 ILInterlockedCompareAndExchangeI4_Full(volatile ILInt32 *dest,
																ILInt32 value,
																ILInt32 comparand)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex			%0, [%3];"
		"teq			%0, %5;"
		"bne			2f;"
		"strex		%1, %4, [%3];"
		"teq			%1, #0;"
		"bne			1b;"
		"2:"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value), "Jr" (comparand)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL 1

/*
 * Compare and exchange two pointers.
 */
static IL_INLINE void *ILInterlockedCompareAndExchangeP(void * volatile *dest,
														void *value,
														void *comparand)
{
	void *retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex			%0, [%3];"
		"teq			%0, %5;"
		"bne			2f;"
		"strex		%1, %4, [%3];"
		"teq			%1, #0;"
		"bne			1b;"
		"2:"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value), "Jr" (comparand)
		: "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP 1

static IL_INLINE void *ILInterlockedCompareAndExchangeP_Acquire(void * volatile *dest,
																void *value,
																void *comparand)
{
	void *retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex			%0, [%3];"
		"teq			%0, %5;"
		"bne			2f;"
		"strex		%1, %4, [%3];"
		"teq			%1, #0;"
		"bne			1b;"
		"2:"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value), "Jr" (comparand)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE 1

static IL_INLINE void *ILInterlockedCompareAndExchangeP_Release(void * volatile *dest,
																void *value,
																void *comparand)
{
	void *retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex			%0, [%3];"
		"teq			%0, %5;"
		"bne			2f;"
		"strex		%1, %4, [%3];"
		"teq			%1, #0;"
		"bne			1b;"
		"2:"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value), "Jr" (comparand)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE 1

static IL_INLINE void *ILInterlockedCompareAndExchangeP_Full(void * volatile *dest,
															 void *value,
															 void *comparand)
{
	void *retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex			%0, [%3];"
		"teq			%0, %5;"
		"bne			2f;"
		"strex		%1, %4, [%3];"
		"teq			%1, #0;"
		"bne			1b;"
		"2:"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "r" (value), "Jr" (comparand)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL 1

/*
 * Add two 32 bit integer values.
 */
static IL_INLINE ILInt32 ILInterlockedAddI4(volatile ILInt32 *dest,
											ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex		%0, [%3];"
		"add		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4 1

static IL_INLINE ILInt32 ILInterlockedAddI4_Acquire(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex		%0, [%3];"
		"add		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE 1

static IL_INLINE ILInt32 ILInterlockedAddI4_Release(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex		%0, [%3];"
		"add		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4_RELEASE 1

static IL_INLINE ILInt32 ILInterlockedAddI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex		%0, [%3];"
		"add		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4_FULL 1

/*
 * Subtract two 32 bit integer values.
 */
static IL_INLINE ILInt32 ILInterlockedSubI4(volatile ILInt32 *dest,
											ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex		%0, [%3];"
		"sub		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4 1

static IL_INLINE ILInt32 ILInterlockedSubI4_Acquire(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		"1:"
		"ldrex		%0, [%3];"
		"sub		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE 1

static IL_INLINE ILInt32 ILInterlockedSubI4_Release(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex		%0, [%3];"
		"sub		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4_RELEASE 1

static IL_INLINE ILInt32 ILInterlockedSubI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex		%0, [%3];"
		"sub		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4_FULL 1

/*
 * 32bit bitwise AND
 */
static IL_INLINE void ILInterlockedAndU4(volatile ILUInt32 *dest,
										 ILUInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__ 
	(
		"1:"
		"ldrex		%0, [%3];"
		"and		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ANDU4 1

static IL_INLINE void ILInterlockedAndU4_Acquire(volatile ILUInt32 *dest,
												 ILUInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__ 
	(
		"1:"
		"ldrex		%0, [%3];"
		"and		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE 1

static IL_INLINE void ILInterlockedAndU4_Release(volatile ILUInt32 *dest,
												 ILUInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex		%0, [%3];"
		"and		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ANDU4_RELEASE 1

static IL_INLINE void ILInterlockedAndU4_Full(volatile ILUInt32 *dest,
											  ILUInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex		%0, [%3];"
		"and		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ANDU4_FULL 1

/*
 * 32bit bitwise OR
 */
static IL_INLINE void ILInterlockedOrU4(volatile ILUInt32 *dest,
										ILUInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__ 
	(
		"1:"
		"ldrex		%0, [%3];"
		"orr		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ORU4 1

static IL_INLINE void ILInterlockedOrU4_Acquire(volatile ILUInt32 *dest,
												ILUInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__ 
	(
		"1:"
		"ldrex		%0, [%3];"
		"orr		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ORU4_ACQUIRE 1

static IL_INLINE void ILInterlockedOrU4_Release(volatile ILUInt32 *dest,
												ILUInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex		%0, [%3];"
		"orr		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ORU4_RELEASE 1

static IL_INLINE void ILInterlockedOrU4_Full(volatile ILUInt32 *dest,
											 ILUInt32 value)
{
	ILInt32 retval;
	ILInt32 state;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		"1:"
		"ldrex		%0, [%3];"
		"orr		%0,	%0, %4;"
		"strex	%1, %0, [%3];"
		"teq		%1, #0;"
		"bne		1b;"
		_IL_INTERLOCKED_ARM_MEMORYBARRIER
		: "=&r" (retval), "=&r" (state), "+m" (*dest)
		: "r" (dest), "Jr" (value)
		: "memory", "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ORU4_FULL 1

#else /* __ARM_ARCH__ <= 5 || __ARM_ARCH_6M__ */

/*
 * Disable the InterlockedExchange functions on ARM V5 and less because they
 * dont interact correctly with the emulated InterlockedCompareAndExchange
 * versions.
 * We may use them later for TestAndSet implemenations which are not
 * guaranteed to interact correctly with the other functions.
 */
#if 0

/*
 * Exchange two 32 bit integers.
 */
static IL_INLINE ILInt32 ILInterlockedExchangeI4_Full(volatile ILInt32 *dest,
													  ILInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__
	(
		"swp %0, %2, [%3]"
		: "=&r" (retval), "=&r" (dest)
		: "r" (value), "1" (dest)
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

	__asm__ __volatile__
	(
		"swp %0, %2, [%3]"
		: "=&r" (retval), "=&r" (dest)
		: "r" (value), "1" (dest)
		: "memory"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP_FULL 1

#endif /* 0 */

#endif /* __ARM_ARCH__ <= 5  || __ARM_ARCH_6M__ */

#endif /* defined(__GNUC__) */

#endif /* defined(__arm) || defined(__arm__) */
