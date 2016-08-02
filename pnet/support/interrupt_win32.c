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

#ifdef IL_INTERRUPT_WIN32

#include "thr_defs.h"
#include "il_thread.h"
#include <windows.h>
#include "interlocked.h"

#ifdef	__cplusplus
extern	"C" {
#endif

static LPTOP_LEVEL_EXCEPTION_FILTER __previousFilter;

static LONG CALLBACK __UnhandledExceptionFilter(EXCEPTION_POINTERS* ExceptionInfo)
{
	ILThread *thread = _ILThreadGetSelf();
	ILInterruptContext context;

	context.instructionAddress = ExceptionInfo->ExceptionRecord->ExceptionAddress;

#ifdef IL_INTERRUPT_HAVE_X86_CONTEXT

	/* Integer registers */
	context.Eax = ExceptionInfo->ContextRecord->Eax;
	context.Ebx = ExceptionInfo->ContextRecord->Ebx;
	context.Ecx = ExceptionInfo->ContextRecord->Ecx;
	context.Edx = ExceptionInfo->ContextRecord->Edx;
	context.Edi = ExceptionInfo->ContextRecord->Edi;
	context.Esi = ExceptionInfo->ContextRecord->Esi;

	/* Control registers */
	context.Ebp = ExceptionInfo->ContextRecord->Ebp;
	context.Eip = ExceptionInfo->ContextRecord->Eip;
	context.Esp = ExceptionInfo->ContextRecord->Esp;

#endif

	if (thread->interruptHandler == 0)
	{
		return EXCEPTION_CONTINUE_SEARCH;
	}

	switch (ExceptionInfo->ExceptionRecord->ExceptionCode)
	{
		case EXCEPTION_ACCESS_VIOLATION:
		
			/* Store the address of the memory the thread was trying to access */
			context.type = IL_INTERRUPT_TYPE_ILLEGAL_MEMORY_ACCESS;
			context.memoryAddress = (void *)ExceptionInfo->ExceptionRecord->ExceptionInformation[1];
			thread->interruptHandler(&context);

			break;

		case EXCEPTION_INT_DIVIDE_BY_ZERO:

			context.type = IL_INTERRUPT_TYPE_INT_DIVIDE_BY_ZERO;
			thread->interruptHandler(&context);

			break;

		case EXCEPTION_INT_OVERFLOW:

			context.type = IL_INTERRUPT_TYPE_INT_OVERFLOW;
			thread->interruptHandler(&context);

			break;
	}

	return EXCEPTION_CONTINUE_SEARCH;
}

void _ILInterruptInit()
{
	__previousFilter = SetUnhandledExceptionFilter(__UnhandledExceptionFilter);
}

void _ILInterruptDeinit()
{
	SetUnhandledExceptionFilter(__previousFilter);
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_INTERRUPT_WIN32 */
