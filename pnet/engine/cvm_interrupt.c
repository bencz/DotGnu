/*
 * cvm_interrupt.c - Opcodes for interrupt handling.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * Author: Thong Nguyen (tum@veridicus.com)
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

#if defined(IL_USE_INTERRUPT_BASED_X)

#if defined(IL_CVM_LOCALS)
IL_JMP_BUFFER backupJumpBuffer;
#endif

#if defined(IL_CVM_PRELUDE)

IL_MEMCPY(&backupJumpBuffer, &thread->exceptionJumpBuffer, sizeof(IL_JMP_BUFFER));

#if (defined(IL_NO_REGISTERS_USED) && defined(IL_USE_INTERRUPT_BASED_X))
do
{
	/* Make sure the compiler allocates these locals on the stack */
	volatile void *addr;
	addr = &pc;
	addr = &stacktop;
	addr = &frame;
}
while (0);
#endif

switch (IL_SETJMP(thread->exceptionJumpBuffer))
{
	case 0:
	{
		break;
	}	

	case _IL_INTERRUPT_NULL_POINTER:
	{
		INTERRUPT_RESTORE_FROM_THREAD();

		NULL_POINTER_EXCEPTION();
	}

	case _IL_INTERRUPT_INT_DIVIDE_BY_ZERO:
	{
		INTERRUPT_RESTORE_FROM_THREAD();

		ZERO_DIV_EXCEPTION();
	}

	case _IL_INTERRUPT_INT_OVERFLOW:	
	{
		/* Interrupt based overflow detection only detects division overflows
		   which are artithmatic exception in the CLI */

		INTERRUPT_RESTORE_FROM_THREAD();

		ARITHMETIC_EXCEPTION();
	}
}

#endif

#endif /* IL_USE_INTERRUPT_BASED_X */
