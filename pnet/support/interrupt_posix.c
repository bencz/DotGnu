/*
 * interlocked.h - Implementation of interlocked functions.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

#include "interrupt.h"

#ifdef IL_INTERRUPT_POSIX

#include "thr_defs.h"
#include "il_thread.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#if defined(IL_INTERRUPT_HAVE_X86_CONTEXT)
	#include <sys/ucontext.h>

	/* older glibc's (<=2.1.2) have EAX instead of REG_EAX */
	#if !defined(REG_EAX) && defined(EAX)
		#define REG_EAX EAX
	#endif
	#if !defined(REG_EBX) && defined(EBX)
		#define REG_EBX EBX
	#endif
	#if !defined(REG_ECX) && defined(ECX)
		#define REG_ECX ECX
	#endif
	#if !defined(REG_EDX) && defined(EDX)
		#define REG_EDX EDX
	#endif
	#if !defined(REG_EDI) && defined(EDI)
		#define REG_EDI EDI
	#endif
	#if !defined(REG_ESI) && defined(ESI)
		#define REG_ESI ESI
	#endif
	#if !defined(REG_EBP) && defined(EBP)
		#define REG_EBP EBP
	#endif
	#if !defined(REG_EIP) && defined(EIP)
		#define REG_EIP EIP
	#endif
	#if !defined(REG_ESP) && defined(ESP)
		#define REG_ESP ESP
	#endif

#elif defined(IL_INTERRUPT_HAVE_X86_64_CONTEXT)
	#include <sys/ucontext.h>
#endif

#if defined(IL_INTERRUPT_SUPPORTS)

#if defined(HAVE_SIGACTION)

static void __sigaction_handler(int signo, siginfo_t *info, void *ctx)
{
	ILThread *thread;
	ILInterruptContext context;
#if defined(IL_INTERRUPT_HAVE_X86_CONTEXT)
	ucontext_t *uc;
	uc = (ucontext_t *)ctx;
#elif defined(IL_INTERRUPT_HAVE_X86_64_CONTEXT)
	ucontext_t *uc;
	uc = (ucontext_t *)ctx;
#elif defined(IL_INTERRUPT_HAVE_ARM_CONTEXT)
	ucontext_t *uc;
	uc = (ucontext_t *)ctx;
#endif

	thread = _ILThreadGetSelf();

#if defined(IL_INTERRUPT_HAVE_X86_CONTEXT)

	#if defined(linux) || defined(__linux) || defined(__linux__)	
		/* Integer registers */
		context.Eax = uc->uc_mcontext.gregs[REG_EAX];
		context.Ebx = uc->uc_mcontext.gregs[REG_EBX];
		context.Ecx = uc->uc_mcontext.gregs[REG_ECX];
		context.Edx = uc->uc_mcontext.gregs[REG_EDX];
		context.Edi = uc->uc_mcontext.gregs[REG_EDI];
		context.Esi = uc->uc_mcontext.gregs[REG_ESI];

		/* Control registers */
		context.Ebp = uc->uc_mcontext.gregs[REG_EBP];
		context.Eip = uc->uc_mcontext.gregs[REG_EIP];
		context.Esp = uc->uc_mcontext.gregs[REG_ESP];
		
	#elif defined(__FreeBSD__)
		/* Integer registers */
		context.Eax = uc->uc_mcontext.mc_eax;
		context.Ebx = uc->uc_mcontext.mc_ebx;
		context.Ecx = uc->uc_mcontext.mc_ecx;
		context.Edx = uc->uc_mcontext.mc_edx;
		context.Edi = uc->uc_mcontext.mc_edi;
		context.Esi = uc->uc_mcontext.mc_esi;

		/* Control registers */
		context.Ebp = uc->uc_mcontext.mc_ebp;
		context.Eip = uc->uc_mcontext.mc_eip;
		context.Esp = uc->uc_mcontext.mc_esp;	
	#endif
	
	context.instructionAddress = (void *)context.Eip;
#elif defined(IL_INTERRUPT_HAVE_X86_64_CONTEXT)
	/* Integer registers */
	context.Rax = uc->uc_mcontext.gregs[REG_RAX];
	context.Rbx = uc->uc_mcontext.gregs[REG_RBX];
	context.Rcx = uc->uc_mcontext.gregs[REG_RCX];
	context.Rdx = uc->uc_mcontext.gregs[REG_RDX];
	context.Rdi = uc->uc_mcontext.gregs[REG_RDI];
	context.Rsi = uc->uc_mcontext.gregs[REG_RSI];
	context.R8 = uc->uc_mcontext.gregs[REG_R8];
	context.R9 = uc->uc_mcontext.gregs[REG_R9];
	context.R10 = uc->uc_mcontext.gregs[REG_R10];
	context.R11 = uc->uc_mcontext.gregs[REG_R11];
	context.R12 = uc->uc_mcontext.gregs[REG_R12];
	context.R13 = uc->uc_mcontext.gregs[REG_R13];
	context.R14 = uc->uc_mcontext.gregs[REG_R14];
	context.R15 = uc->uc_mcontext.gregs[REG_R15];

	context.Rbp = uc->uc_mcontext.gregs[REG_RBP];
	context.Rip = uc->uc_mcontext.gregs[REG_RIP];
	context.Rsp = uc->uc_mcontext.gregs[REG_RSP];

	context.instructionAddress = (void *)context.Rip;
#elif defined(IL_INTERRUPT_HAVE_ARM_CONTEXT)
	context.R0 = uc->uc_mcontext.arm_r0;
	context.R1 = uc->uc_mcontext.arm_r1;
	context.R2 = uc->uc_mcontext.arm_r2;
	context.R3 = uc->uc_mcontext.arm_r3;
	context.R4 = uc->uc_mcontext.arm_r4;
	context.R5 = uc->uc_mcontext.arm_r5;
	context.R6 = uc->uc_mcontext.arm_r6;
	context.R7 = uc->uc_mcontext.arm_r7;
	context.R8 = uc->uc_mcontext.arm_r8;
	context.R9 = uc->uc_mcontext.arm_r9;
	context.R10 = uc->uc_mcontext.arm_r10;

	context.Rfp = uc->uc_mcontext.arm_fp;
	context.Rip = uc->uc_mcontext.arm_ip;
	context.Rsp = uc->uc_mcontext.arm_sp;
	context.Rlr = uc->uc_mcontext.arm_lr;
	context.Rpc = uc->uc_mcontext.arm_pc;

	context.instructionAddress = (void *)context.Rpc;
#else
	context.instructionAddress = 0;
#endif

	switch (signo)
	{	
		#if defined(IL_INTERRUPT_SUPPORTS_ILLEGAL_MEMORY_ACCESS)
		case SIGSEGV:
		case SIGBUS:

			thread = _ILThreadGetSelf();

			context.memoryAddress = info->si_addr;
			context.type = IL_INTERRUPT_TYPE_ILLEGAL_MEMORY_ACCESS;

			thread->interruptHandler(&context);
			
			break;
		#endif

		#if defined(IL_INTERRUPT_SUPPORTS_ANY_ARITH)
		case SIGFPE:

			switch (info->si_code)
			{
				#if defined(IL_INTERRUPT_SUPPORTS_INT_DIVIDE_BY_ZERO)
				case FPE_INTDIV:
					context.type = IL_INTERRUPT_TYPE_INT_DIVIDE_BY_ZERO;
					context.instructionAddress = info->si_addr;
					context.memoryAddress = 0;
					thread->interruptHandler(&context);
					break;
				#endif

				#if defined(IL_INTERRUPT_SUPPORTS_INT_OVERFLOW)
				case FPE_INTOVF:
					context.type = IL_INTERRUPT_TYPE_INT_OVERFLOW;
					context.instructionAddress = info->si_addr;
					context.memoryAddress = 0;
					thread->interruptHandler(&context);
					break;
				#endif
			}

			break;
		#endif
	}
	
}

#elif defined(HAVE_SIGNAL)

static void __signal_handler(int signal)
{
	ILThread *thread;
	ILInterruptContext context;

	thread = _ILThreadGetSelf();

	switch (signal)
	{
		#if defined(IL_INTERRUPT_SUPPORTS_ILLEGAL_MEMORY_ACCESS)
		case SIGSEGV:
		case SIGBUS:
			context.memoryAddress = 0
			context.instructionAddress = 0
			context.type = IL_INTERRUPT_TYPE_ILLEGAL_MEMORY_ACCESS;
			thread->interruptHandler(&context);
			break;
		#endif
	}
}

#endif

#endif /* IL_INTERRUPT_SUPPORTS_ILLEGAL_MEMORY_ACCESS */

void _ILInterruptInit()
{
#if defined(IL_INTERRUPT_SUPPORTS)	
	#if defined(HAVE_SIGACTION)

		/* Use SIGACTION */
		struct sigaction sa;

		sa.sa_sigaction = __sigaction_handler;
		sigemptyset(&sa.sa_mask);
		sa.sa_flags = SA_SIGINFO;
		
		/* Registers memory violation handlers */
		#ifdef IL_INTERRUPT_SUPPORTS_ILLEGAL_MEMORY_ACCESS
			sigaction(SIGSEGV, &sa, 0);
			sigaction(SIGBUS, &sa, 0);
		#endif

		/* Registers FPE exception handlers */
		#if defined(IL_INTERRUPT_SUPPORTS_ANY_ARITH)
			sigemptyset(&sa.sa_mask);
			sa.sa_flags = SA_SIGINFO | SA_NODEFER;
			sigaction(SIGFPE, &sa, 0);
		#endif
	#elif defined(HAVE_SIGNAL)

		/* Use SIGNAL */
		#warning sigaction() not available, using signal() which may be inaccurate

		/* Register memory violation handlers */
		signal(SIGSEGV, __signal_handler);
		signal(SIGBUS, __signal_handler);
	#endif
#endif
}

void _ILInterruptDeinit()
{
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_INTERRUPT_POSIX */

