/*
 * interlocked_ppc.h - Implementation of interlocked functions for
 * powerpc processors.
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

#if defined(__powerpc__) || defined(powerpc) || \
	defined(__powerpc) || defined(PPC) || defined(__ppc__)

#if defined(__GNUC__)

/*
 * References:
 * http://www.rdrop.com/users/paulmck/scalability/paper/N2745r.2009.02.22a.html
 */

/*
 * Use the leight weight sync for release semantics if available.
 */
#ifdef __NO_LWSYNC__
#define _IL_INTERLOCKED_PPC_LWSYNC	"\tsync\n"
#else
#define _IL_INTERLOCKED_PPC_LWSYNC	"\tlwsync\n"
#endif

/*
 * Flush cache and set a memory barrier.
 */
static IL_INLINE void ILInterlockedMemoryBarrier()
{
	__asm__ __volatile__
	(
		"sync"
		:
		:
		: "memory"
	);
}
#define IL_HAVE_INTERLOCKED_MEMORYBARRIER 1

/*
 * Load a 32 bit value from a location with acquire semantics.
 */
static IL_INLINE ILInt32 ILInterlockedLoadI4_Acquire(const volatile ILInt32 *dest)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
	    "\tlwz%U1%X1 %0, %1\n"
	    "\tcmpw		%0, %0\n"
	    "\tbne-		1f\n"
    	"1:"
		"\tisync\n"
	    : "=r" (retval)
	    : "m" (*dest)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_LOADI4_ACQUIRE

/*
 * Load a pointer value from a location with acquire semantics.
 */
static IL_INLINE void *ILInterlockedLoadP_Acquire(void * const volatile *dest)
{
	void *retval;

	__asm__ __volatile__ 
	(
	    "\tlwz%U1%X1 %0, %1\n"
	    "\tcmpw		%0, %0\n"
	    "\tbne-		1f\n"
    	"1:"
		"\tisync\n"
	    : "=r" (retval)
	    : "m" (*dest)
		: "memory", "cc"
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
		_IL_INTERLOCKED_PPC_LWSYNC
		"\tstw	%1, %0\n"
		: "=m" (*dest)
		: "r" (value)
		: "memory"
	);
}
#define IL_HAVE_INTERLOCKED_STOREI4_RELEASE 1

/*
 * Store a pointer value to a location with release semantics.
 * TODO: Add support for ppc64
 */
static IL_INLINE void ILInterlockedStoreP_Release(void * volatile *dest,
												  void *value)
{
	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"\tstw	%1, %0\n"
		: "=m" (*dest)
		: "r" (value)
		: "memory"
	);
}
#define IL_HAVE_INTERLOCKED_STOREP_RELEASE 1

/*
 * Exchange two 32 bit integers.
 */
static IL_INLINE ILInt32 ILInterlockedExchangeI4(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
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

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
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

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
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

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL 1

/*
 * Exchange pointers.
 *
 * FIXME: Add support for the 64bit powerpc
 */
static IL_INLINE void *ILInterlockedExchangeP(void * volatile *dest,
											  void *value)
{
	void *retval;

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
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

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
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

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
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

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
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

	__asm__ __volatile__
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tcmpw		%0, %4\n"
		"\tbne		2f\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		"2:"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value), "r" (comparand)
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

	__asm__ __volatile__
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tcmpw		%0, %4\n"
		"\tbne		2f\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		"2:"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value), "r" (comparand)
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

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tcmpw		%0, %4\n"
		"\tbne		2f\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		"2:"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value), "r" (comparand)
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

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tcmpw		%0, %4\n"
		"\tbne		2f\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		"2:"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value), "r" (comparand)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL 1

/*
 * Compare and exchange two pointers.
 * TODO: Add ppc64 support
 */
static IL_INLINE void *ILInterlockedCompareAndExchangeP(void * volatile *dest,
														void *value,
														void *comparand)
{
	void *retval;

	__asm__ __volatile__
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tcmpw		%0, %4\n"
		"\tbne		2f\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		"2:"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value), "r" (comparand)
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

	__asm__ __volatile__
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tcmpw		%0, %4\n"
		"\tbne		2f\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		"2:"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value), "r" (comparand)
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

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tcmpw		%0, %4\n"
		"\tbne		2f\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		"2:"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value), "r" (comparand)
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

	__asm__ __volatile__
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tcmpw		%0, %4\n"
		"\tbne		2f\n"
		"\tstwcx.	%3, 0, %2\n"
		"\tbne-		1b\n"
		"2:"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value), "r" (comparand)
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

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tadd		%0, %0, %3\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4 1

static IL_INLINE ILInt32 ILInterlockedAddI4_Acquire(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tadd		%0, %0, %3\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE 1

static IL_INLINE ILInt32 ILInterlockedAddI4_Release(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tadd		%0, %0, %3\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4_RELEASE 1

static IL_INLINE ILInt32 ILInterlockedAddI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tadd		%0, %0, %3\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
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

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tsubf		%0, %3, %0\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4 1

static IL_INLINE ILInt32 ILInterlockedSubI4_Acquire(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tsubf		%0, %3, %0\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE 1

static IL_INLINE ILInt32 ILInterlockedSubI4_Release(volatile ILInt32 *dest,
													ILInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tsubf		%0, %3, %0\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4_RELEASE 1

static IL_INLINE ILInt32 ILInterlockedSubI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tsubf		%0, %3, %0\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
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

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tand		%0, %0, %3\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ANDU4 1

static IL_INLINE void ILInterlockedAndU4_Full(volatile ILUInt32 *dest,
											  ILUInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tand		%0, %0, %3\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
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

	__asm__ __volatile__ 
	(
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tor		%0, %0, %3\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ORU4 1

static IL_INLINE void ILInterlockedOrU4_Full(volatile ILUInt32 *dest,
											 ILUInt32 value)
{
	ILInt32 retval;

	__asm__ __volatile__ 
	(
		_IL_INTERLOCKED_PPC_LWSYNC
		"1:"
		"\tlwarx	%0, 0, %2\n"
		"\tor		%0, %0, %3\n"
		"\tstwcx.	%0, 0, %2\n"
		"\tbne-		1b\n"
		_IL_INTERLOCKED_PPC_LWSYNC
		: "=&r" (retval), "=m" (*dest)
		: "r" (dest), "r" (value)
		: "memory", "cc"
	);
}
#define IL_HAVE_INTERLOCKED_ORU4_FULL 1

#endif /* defined(__GNUC__) */

#endif /* defined(__powerpc__) || defined(powerpc) || defined(__powerpc) || defined(PPC) || defined(__ppc__) */
