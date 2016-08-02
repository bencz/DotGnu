/*
 * interlocked_mips.h - Implementation of interlocked functions for
 * Mips processors.
 *
 * Copyright (C) 2010  Southern Storm Software, Pty Ltd.
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

#if defined(__mips) || defined(__mips__)

#if defined(__GNUC__)

/*
 * Flush cache and set a memory barrier.
 */
static IL_INLINE void ILInterlockedMemoryBarrier()
{
	__asm__ __volatile__
	(
		"	.set push\n"
		"	.set mips2\n"
		"	.set noreorder\n"
		"	.set nomacro\n"
		"	sync\n"
		"	.set pop\n"
		:
		:
		: "memory"
	);
}
#define IL_HAVE_INTERLOCKED_MEMORYBARRIER 1

/*
 * Exchange two 32 bit integers.
 */
static IL_INLINE ILInt32 ILInterlockedExchangeI4_Full(volatile ILInt32 *dest,
													  ILInt32 value)
{
	ILInt32 retval;
	ILInt32 temp;

	__asm__ __volatile__
	(
		"	.set push\n"
		"	.set mips2\n"
		"	.set noreorder\n"
		"	.set nomacro\n"
		"1:	ll		%0, %1\n"
		"	move	%2, %3\n"
		"	sc		%2, %1\n"
		"	.set pop\n"
		"	beqz	%2, 1b\n"
		: "=&r" (retval), "+R" (*dest), "=&r" (temp)
		: "r" (value)
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
	void *temp;

	__asm__ __volatile__
	(
		"	.set push\n"
		"	.set mips2\n"
		"	.set noreorder\n"
		"	.set nomacro\n"
		"1:	ll		%0, %1\n"
		"	move	%2, %3\n"
		"	sc		%2, %1\n"
		"	.set pop\n"
		"	beqz	%2, 1b\n"
		: "=&r" (retval), "+R" (*dest), "=&r" (temp)
		: "r" (value)
		: "memory"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_EXCHANGEP_FULL 1

/*
 * Compare and exchange two 32bit integers.
 */
static IL_INLINE ILInt32 ILInterlockedCompareAndExchangeI4_Full(volatile ILInt32 *dest,
																ILInt32 value,
																ILInt32 comparand)
{
	ILInt32 retval;
	ILInt32 temp;

	__asm__ __volatile__
	(
		"	.set push\n"
		"	.set mips2\n"
		"	.set noreorder\n"
		"	.set nomacro\n"
		"1:	ll		%0, %1\n"
		"	bne		%0, %4, 2f\n"
		"	move	%2, %3\n"
		"	sc		%2, %1\n"
		"	.set pop\n"
		"	beqz	%2, 1b\n"
		"2:"
		: "=&r" (retval), "+R" (*dest), "=&r" (temp)
		: "r" (value), "r" (comparand)
		: "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEI4_FULL 1

/*
 * Compare and exchange two pointers
 */
static IL_INLINE void *ILInterlockedCompareAndExchangeP_Full(void * volatile *dest,
															 void *value,
															 void *comparand)
{
	void *retval;
	void *temp;

	__asm__ __volatile__
	(
		"	.set push\n"
		"	.set mips2\n"
		"	.set noreorder\n"
		"	.set nomacro\n"
		"1:	ll		%0, %1\n"
		"	bne		%0, %4, 2f\n"
		"	move	%2, %3\n"
		"	sc		%2, %1\n"
		"	.set pop\n"
		"	beqz	%2, 1b\n"
		"2:"
		: "=&r" (retval), "+R" (*dest), "=&r" (temp)
		: "r" (value), "r" (comparand)
		: "cc"
	);

	return retval;
}
#define IL_HAVE_INTERLOCKED_COMPAREANDEXCHANGEP_FULL 1

/*
 * Add two 32 bit integer values.
 */
static IL_INLINE ILInt32 ILInterlockedAddI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;
	ILInt32 temp;

	__asm__ __volatile__
	(
		"	.set push\n"
		"	.set mips2\n"
		"	.set noreorder\n"
		"	.set nomacro\n"
		"1:	ll		%0, %1\n"
		"	add		%2, %0, %3\n"
		"	move	%0, %2\n"
		"	sc		%2, %1\n"
		"	.set pop\n"
		"	beqz	%2, 1b\n"
		: "=&r" (retval), "+R" (*dest), "=&r" (temp)
		: "Ir" (value)
		: "memory"
	);
	return retval;
}
#define IL_HAVE_INTERLOCKED_ADDI4_FULL 1

/*
 * Subtract two 32 bit integer values.
 */
static IL_INLINE ILInt32 ILInterlockedSubI4_Full(volatile ILInt32 *dest,
												 ILInt32 value)
{
	ILInt32 retval;
	ILInt32 temp;

	__asm__ __volatile__
	(
		"	.set push\n"
		"	.set mips2\n"
		"	.set noreorder\n"
		"	.set nomacro\n"
		"1:	ll		%0, %1\n"
		"	sub		%2, %0, %3\n"
		"	move	%0, %2\n"
		"	sc		%2, %1\n"
		"	.set pop\n"
		"	beqz	%2, 1b\n"
		: "=&r" (retval), "+R" (*dest), "=&r" (temp)
		: "Ir" (value)
		: "memory"
	);
	return retval;
}
#define IL_HAVE_INTERLOCKED_SUBI4_FULL 1

#endif /* defined(__GNUC__) */

#endif /* defined(__mips) || defined(__mips__) */
