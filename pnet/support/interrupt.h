/*
 * interlocked.h - Implementation of interlocked functions.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#ifndef _INTERRUPT_H
#define _INTERRUPT_H

#include "il_config.h"
#include "il_values.h"
#include "il_thread.h"

#if defined(__i386) || defined(__i386__)

struct _tagILInterruptContext
{
	int type;
	
	void *memoryAddress;
	void *instructionAddress;
	
	/* Integer registers */
	unsigned int Eax;
	unsigned int Ebx;	
	unsigned int Ecx;
	unsigned int Edx;
	unsigned int Edi;
	unsigned int Esi;

	/* Control registers */
	unsigned int Ebp;
	unsigned int Eip;
	unsigned int Esp;
};

#elif defined(__x86_64__) || defined(__x86_64) 

/*
 * Note: The SysV ABI specifies that the long type is 64 bits wide.
 * On 64 bit Windows the long datatype is still 32 bits wide.
 * So we have to use ILUInt64 for the general purpose registers to be
 * portable.
 * We don't save the xmm and floating point registers in the context
 * for now.
 */
struct _tagILInterruptContext
{
	int type;
	
	void *memoryAddress;
	void *instructionAddress;

	/* General purpose registers */
	ILUInt64	Rax;
	ILUInt64	Rbx;
	ILUInt64	Rcx;
	ILUInt64	Rdx;
	ILUInt64	Rsi;
	ILUInt64	Rdi;
	ILUInt64	R8;
	ILUInt64	R9;
	ILUInt64	R10;
	ILUInt64	R11;
	ILUInt64	R12;
	ILUInt64	R13;
	ILUInt64	R14;
	ILUInt64	R15;

	/* Control registers */
	ILUInt64	Rbp;
	ILUInt64	Rip;
	ILUInt64	Rsp;
};

#elif defined(__arm) || defined(__arm__)

struct _tagILInterruptContext
{
	int type;
	
	void *memoryAddress;
	void *instructionAddress;

	/* General purpose registers */
	unsigned long R0;
	unsigned long R1;
	unsigned long R2;
	unsigned long R3;
	unsigned long R4;
	unsigned long R5;
	unsigned long R6;
	unsigned long R7;
	unsigned long R8;
	unsigned long R9;
	unsigned long R10;

	/* Control registers */
	unsigned long Rfp;
	unsigned long Rip;
	unsigned long Rsp;
	unsigned long Rlr;
	unsigned long Rpc;
};

#else

struct _tagILInterruptContext
{
	int type;

	void *memoryAddress;
	void *instructionAddress;
};

#endif

#if defined(USE_INTERRUPT_BASED_CHECKS)
#if defined(HAVE_SETJMP_H)
#include <setjmp.h>
#endif
#if (defined(HAVE_SIGSETJMP) || defined(HAVE_SETJMP_H)) && \
	defined(HAVE_SIGLONGJMP)
#define IL_SETJMP(buf) sigsetjmp(buf, 1)
#define IL_LONGJMP(buf, arg) siglongjmp(buf, arg)
#define IL_JMP_BUFFER sigjmp_buf
#elif (defined(HAVE_SETJMP) || defined(HAVE_SETJMP_H)) \
	&& defined(HAVE_LONGJMP)
/*
 * NOTE: Posix doesn't specify if the signal mask is saved and restored
 * using these functions.
 * You have to check carefully if the signalmask is saved and restored
 * because this is needed for using longjump pretty safe from inside of a
 * signal handler.
 */
#define IL_SETJMP(buf)			 setjmp(buf)
#define IL_LONGJMP(buf, arg)	longjmp(buf, arg)
#define IL_JMP_BUFFER jmp_buf
#else
#undef USE_INTERRUPT_BASED_CHECKS
#endif
#endif

#if defined(USE_INTERRUPT_BASED_CHECKS)
	#if defined(WIN32) && !(defined(__CYGWIN32__) || defined(__CYGWIN))
		#define IL_INTERRUPT_SUPPORTS 1
		#define IL_INTERRUPT_SUPPORTS_ILLEGAL_MEMORY_ACCESS 1
		#define IL_INTERRUPT_SUPPORTS_INT_DIVIDE_BY_ZERO 1
		#define IL_INTERRUPT_SUPPORTS_INT_OVERFLOW 1
		#define IL_INTERRUPT_SUPPORTS_ANY_ARITH 1

		#define IL_INTERRUPT_WIN32 1
		#if defined(__i386) || defined(__i386__)
			#define IL_INTERRUPT_HAVE_X86_CONTEXT 1
		#endif
	#elif defined(linux) || defined(__linux) || defined(__linux__) \
		|| defined(__FreeBSD__) && (defined(HAVE_SIGNAL) \
		|| defined(HAVE_SIGACTION))

		#define IL_INTERRUPT_SUPPORTS 1
		#define IL_INTERRUPT_SUPPORTS_ILLEGAL_MEMORY_ACCESS 1
		#define IL_INTERRUPT_SUPPORTS_ANY_ARITH 1

		#define IL_INTERRUPT_POSIX 1

		#ifdef HAVE_SIGACTION
			#define IL_INTERRUPT_SUPPORTS_INT_DIVIDE_BY_ZERO
			#define IL_INTERRUPT_SUPPORTS_INT_OVERFLOW 1
		#endif

		#if defined(HAVE_SIGACTION) && defined(HAVE_SYS_UCONTEXT_H)
			#if (defined(__i386) || defined(__i386__))
				#define IL_INTERRUPT_HAVE_X86_CONTEXT 1
			#elif defined(__x86_64__) || defined(__x86_64) 
				#define IL_INTERRUPT_HAVE_X86_64_CONTEXT 1
			#elif defined(__arm) || defined(__arm__)
				#define IL_INTERRUPT_HAVE_ARM_CONTEXT 1
			#endif
		#endif
	#endif
#endif

#endif /* _INTERRUPT_H */
