/*
 * interlocked.h - Implementation of interlocked functions.
 *
 * Copyright (C) 2002, 2009, 2010  Southern Storm Software, Pty Ltd.
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

#ifndef _INTERLOCKED_H_
#define _INTERLOCKED_H_

#ifndef IL_INLINE
	#ifdef __GNUC__
		#define IL_INLINE       __inline__
	#elif defined(_MSC_VER)
		#define IL_INLINE	__forceinline
	#else
		#define IL_INLINE
	#endif
#endif

/*
 * Semantics:
 * All Functions without one of the siffixes _Acquire, _Release or
 * _Full have a relaxed memory access order constraint. This means it is not
 * guaranteed that the operation is visible to the public at the point where
 * the source code states with respect to preceding and following loads or
 * stores. This can happen because of possible processor load / store
 * reordering.
 *
 * _Acquire Suffix: Operations with this suffix guarantee that no operations
 * following this instruction are moved before this instruction.
 *
 * _Release Suffix: Operations with this suffix guarantee that no operations
 * preceeding this instruction are moved after this instruction.
 *
 * _Full suffix: The rules for _Acquire and _Release apply.
 *
 * NOTE: The identifier after define: has to be defined if an arch specific
 * version of this function is available.
 *
 * Implemented functions:
 *
 * Full memory barrier.
 * define: IL_HAVE_INTERLOCKED_MEMORYBARRIER
 *
 * void ILInterlockedMemoryBarrier(void)
 *
 * Load a 32 bit value from a location.
 * Returns the 32 bit value from the location specified.
 * define: IL_HAVE_INTERLOCKED_LOAD
 * define: IL_HAVE_INTERLOCKED_LOAD_ACQUIRE
 *
 * ILInt32 ILInterlockedLoad(const volatile ILInt32 *dest)
 *
 * Load a pointer value from a location.
 * Returns the pointer value from the location specified.
 * define: IL_HAVE_INTERLOCKED_LOADPOINTER
 * define: IL_HAVE_INTERLOCKED_LOADPOINTER_ACQUIRE
 *
 * void * ILInterlockedLoadPointer(void * const volatile *dest)
 *
 * Store a 32 bit value to a location.
 * define: IL_HAVE_INTERLOCKED_STORE
 * define: IL_HAVE_INTERLOCKED_STORE_RELEASE
 *
 * void ILInterlockedStore(volatile ILInt32 *dest, ILInt32 value)
 *
 * Store a pointer value to a location.
 * define: IL_HAVE_INTERLOCKED_STOREPOINTER
 * define: IL_HAVE_INTERLOCKED_STOREPOINTER_RELEASE
 *
 * void ILInterlockedStorePointer(void * volatile *dest, void *value)
 *
 * Exchange integers.
 * Returns the original value at *dest.
 * define: IL_HAVE_INTERLOCKED_EXCHANGEI4
 * define: IL_HAVE_INTERLOCKED_EXCHANGEI4_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_EXCHANGEI4_RELEASE
 * define: IL_HAVE_INTERLOCKED_EXCHANGEI4_FULL
 *
 * ILInt32 ILInterlockedExchange(volatile ILInt32 *dest, ILInt32 value)
 *
 * Exchange pointers.
 * Returns the original value at *dest 
 * define: IL_HAVE_INTERLOCKED_EXCHANGEP
 * define: IL_HAVE_INTERLOCKED_EXCHANGEP_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_EXCHANGEP_RELEASE
 * define: IL_HAVE_INTERLOCKED_EXCHANGEP_FULL
 *
 * void *ILInterlockedExchangeP(void * volatile *dest, void *value)
 *
 * Compare and exchange two 32bit integers.
 * Returns the original value at *dest 
 * define: IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4
 * define: IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_RELEASE
 * define: IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL
 *
 * ILInt32 ILInterlockedCompareAndExchangeI4(volatile ILInt32 *dest,
 *											 ILInt32 value,
 *											 ILInt32 comparand)
 *
 * Compare and exchange two pointers.
 * Returns the original value at *dest 
 * define: IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP
 * define: IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_RELEASE
 * define: IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL
 * 
 * void *ILInterlockedCompareAndExchangeP(void * volatile *dest,
 *										  void *value,
 *										  void *comparand)
 *
 * Add the 32 bit values *dest and value and store the result at *dest.
 * Returns the result od the addition (new value at *dest)
 * define: IL_HAVE_INTERLOCKED_ADDI4
 * define: IL_HAVE_INTERLOCKED_ADDI4_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_ADDI4_RELEASE
 * define: IL_HAVE_INTERLOCKED_ADDI4_FULL
 *
 * ILInt32 ILInterlockedAddI4(volatile ILInt32 *dest, ILInt32 value)
 *
 * Substract the 32 bit values *dest and value and store the result at *dest.
 * Returns the result od the subtraction (new value at *dest)
 * define: IL_HAVE_INTERLOCKED_SUBI4
 * define: IL_HAVE_INTERLOCKED_SUBI4_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_SUBI4_RELEASE
 * define: IL_HAVE_INTERLOCKED_SUBI4_FULL
 *
 * ILInt32 ILInterlockedSubI4(volatile ILInt32 *dest, ILInt32 value)
 *
 * NOTE: If not defined by the arch specific definition it is a implemented
 * by a macro that is using ILInterlockedAdd with value negated.
 * (See interlocked_any.h)
 *
 * Increment a 32bit integer.
 * Returns the value of the incremented integer.
 * define: IL_HAVE_INTERLOCKED_INCREMENTI4
 * define: IL_HAVE_INTERLOCKED_INCREMENTI4_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_INCREMENTI4_RELEASE
 * define: IL_HAVE_INTERLOCKED_INCREMENTI4_FULL
 *
 * ILInt32 ILInterlockedIncrementI4(volatile ILInt32 *dest)
 *
 * NOTE: If not defined by the arch specific definition it is a implemented
 * by a macro that is using ILInterlockedAdd with a value of 1.
 * (See interlocked_any.h)
 *
 * Decrement a 32bit integer.
 * Returns the value of the decremented integer.
 * define: IL_HAVE_INTERLOCKED_DECREMENTI4
 * define: IL_HAVE_INTERLOCKED_DECREMENTI4_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_DECREMENTI4_RELEASE
 * define: IL_HAVE_INTERLOCKED_DECREMENTI4_FULL
 *
 * ILInt32 ILInterlockedDecrementI4(volatile ILInt32 *dest)
 *
 * NOTE: If not defined by the arch specific definition it is a implemented
 * by a macro that is using ILInterlockedSub with a value of 1.
 * (See interlocked_any.h)
 *
 * Bitwise AND of *dest and value and the result is stored at *dest
 * define: IL_HAVE_INTERLOCKED_ANDU4
 * define: IL_HAVE_INTERLOCKED_ANDU4_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_ANDU4_RELEASE
 * define: IL_HAVE_INTERLOCKED_ANDU4_FULL
 *
 * void ILInterlockedAndU4(volatile ILUInt32 *dest, ILUInt32 value)
 *
 * Bitwise OR of *dest and value and the result is stored at *dest
 * define: IL_HAVE_INTERLOCKED_ORU4
 * define: IL_HAVE_INTERLOCKED_ORU4_ACQUIRE
 * define: IL_HAVE_INTERLOCKED_ORU4_RELEASE
 * define: IL_HAVE_INTERLOCKED_ORU4_FULL
 *
 * void ILInterlockedOrU4(volatile ILUInt32 *dest, ILUInt32 value)
 *
 * NOTE: If archdependent versions with the _Aquire or _Release suffix
 * are not defined they will be mapped to the version with the _Full suffix.
 *
 * NOTE: The versions using a mutex to protect the value are generally
 * available with the same name and a leading underscore.
 * This is to allow the usage of the version using the mutex even if a
 * native version is available if propper protection requires the
 * consistent availability of more than one interlocked function.
 */

/*
 * Define a barrier for the used compiler to prevent that the operation
 * in question is moved around by the compiler's optimizer.
 */
#if defined(__GNUC__)
#define ILInterlockedCompilerBarrier	__asm__ __volatile__ ("" : : : "memory")
#else
#define ILInterlockedCompilerBarrier
#endif

/*
 * Initialize the interlocked system.
 */
void ILInterlockedInit(void);

/*
 * Backup functions if the operation is not supported by the processor.
 */

/*
 * Flush cache and set a memory barrier.
 */
void _ILInterlockedMemoryBarrier();

/*
 * Exchange signed 32 bit integers.
 */
ILInt32 _ILInterlockedExchangeI4_Full(volatile ILInt32 *dest, ILInt32 value);

/*
 * Exchange unsigned 32 bit integers.
 */
ILUInt32 _ILInterlockedExchangeU4_Full(volatile ILUInt32 *dest, ILUInt32 value);

/*
 * Exchange signed 64 bit integers.
 */
ILInt64 _ILInterlockedExchangeI8_Full(volatile ILInt64 *dest, ILInt64 value);

/*
 * Exchange unsigned 64 bit integers.
 */
ILUInt64 _ILInterlockedExchangeU8_Full(volatile ILUInt64 *dest, ILUInt64 value);

/*
 * Exchange pointers.
 */
void *_ILInterlockedExchangeP_Full(void * volatile *dest, void *value);

/*
 * Exchange 32 bit foatingpoint values.
 */
ILFloat _ILInterlockedExchangeR4_Full(volatile ILFloat *dest, ILFloat value);

/*
 * Exchange 64 bit foatingpoint values.
 */
ILDouble _ILInterlockedExchangeR8_Full(volatile ILDouble *dest, ILDouble value);

/*
 * Compare and exchange signed 32 bit integers.
 */
ILInt32 _ILInterlockedCompareAndExchangeI4_Full(volatile ILInt32 *dest,
												ILInt32 value,
												ILInt32 comparand);

/*
 * Compare and exchange unsigned 32 bit integers.
 */
ILUInt32 _ILInterlockedCompareAndExchangeU4_Full(volatile ILUInt32 *dest,
												 ILUInt32 value,
												 ILUInt32 comparand);

/*
 * Compare and exchange signed 64 bit integers.
 */
ILInt64 _ILInterlockedCompareAndExchangeI8_Full(volatile ILInt64 *dest,
												ILInt64 value,
												ILInt64 comparand);

/*
 * Compare and exchange unsigned 64 bit integers.
 */
ILUInt64 _ILInterlockedCompareAndExchangeU8_Full(volatile ILUInt64 *dest,
												 ILUInt64 value,
												 ILUInt64 comparand);

/*
 * Compare and exchange pointers.
 */
void *_ILInterlockedCompareAndExchangeP_Full(void * volatile *dest,
											 void *value,
											 void *comparand);

/*
 * Compare and exchange 32 bit floatingpoint values.
 */
ILFloat _ILInterlockedCompareAndExchangeR4_Full(volatile ILFloat *dest,
												ILFloat value,
												ILFloat comparand);

/*
 * Compare and exchange 64 bit floatingpoint values.
 */
ILDouble _ILInterlockedCompareAndExchangeR8_Full(volatile ILDouble *dest,
												 ILDouble value,
												 ILDouble comparand);

/*
 * Add two signed 32 bit integers.
 */
ILInt32 _ILInterlockedAddI4_Full(volatile ILInt32 *dest, ILInt32 value);

/*
 * Add two signed 64 bit integers.
 */
ILInt64 _ILInterlockedAddI8_Full(volatile ILInt64 *dest, ILInt64 value);

/*
 * Bitwise and two unsigned 32 bit integers.
 */
void _ILInterlockedAndU4_Full(volatile ILUInt32 *dest, ILUInt32 value);

/*
 * Bitwise and two unsigned 64 bit integers.
 */
void _ILInterlockedAndU8_Full(volatile ILUInt64 *dest, ILUInt64 value);

/*
 * Bitwise or two unsigned 32 bit integers.
 */
void _ILInterlockedOrU4_Full(volatile ILUInt32 *dest, ILUInt32 value);

/*
 * Bitwise or two unsigned 64 bit integers.
 */
void _ILInterlockedOrU8_Full(volatile ILUInt64 *dest, ILUInt64 value);

/* TODO: implement native interlocked functions for other processors */

#include "interlocked_x86.h"
#include "interlocked_arm.h"
#include "interlocked_ppc.h"
#include "interlocked_mips.h"
#include "interlocked_any.h"

#endif /* _INTERLOCKED_H_ */
