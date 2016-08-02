/*
 * cvm_config.h - Configure CVM in various ways.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#ifndef	_ENGINE_CVM_CONFIG_H
#define	_ENGINE_CVM_CONFIG_H

#include "il_config.h"

#ifdef IL_USE_CVM

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Turn off assembly code optimisations if this is defined.
 */
/*#define IL_NO_ASM*/

/*
 * Turn off explicit register declarations in the cvm loop if this is defined.
 */
/*#define IL_NO_REGISTERS_USED*/

/*
 * Enable or disable dumping of CVM instructions during execution.
 */
/*#define	IL_DUMP_CVM*/
#define	IL_DUMP_CVM_STREAM	stdout

/*
 * Enable or disable profiling.
 */
/*#define	IL_PROFILE_CVM_INSNS*/
/*#define	IL_PROFILE_CVM_VAR_USAGE*/
#ifdef IL_PROFILE_CVM_INSNS
extern int _ILCVMInsnCount[];
#endif

/*
 * Determine what kind of instruction dumping to perform.
 */
#if defined(IL_DUMP_CVM)
	#define	CVM_DUMP()	\
		_ILDumpCVMInsn(IL_DUMP_CVM_STREAM, method, pc)
	#define CVM_WIDE_DUMP()
	#define CVM_PREFIX_DUMP()
#elif defined(IL_PROFILE_CVM_INSNS)
	#define	CVM_DUMP()	\
		(++(_ILCVMInsnCount[pc[0]]))
	#define CVM_WIDE_DUMP()	\
		(++(_ILCVMInsnCount[pc[1]]))
	#define CVM_PREFIX_DUMP()	\
		(++(_ILCVMInsnCount[((int)(pc[1])) + 256]))
#else
	#define	CVM_DUMP()
	#define CVM_WIDE_DUMP()
	#define CVM_PREFIX_DUMP()
	#ifdef IL_CONFIG_DIRECT
		#define	IL_CVM_DIRECT_ALLOWED
	#endif
#endif

/*
 * Determine what CPU we are compiling for, and any
 * additional optimizations we can use for that CPU.
 */
#if defined(__i386) || defined(__i386__)
	#define	CVM_X86
	#define CVM_LITTLE_ENDIAN
	#define	CVM_LONGS_ALIGNED_WORD
	#define	CVM_REALS_ALIGNED_WORD
	#define	CVM_DOUBLES_ALIGNED_WORD
	#define CVM_WORDS_AND_PTRS_SAME_SIZE
#endif
#if defined(__arm) || defined(__arm__)
	#define	CVM_ARM
	#define	CVM_LONGS_ALIGNED_WORD
	#define CVM_WORDS_AND_PTRS_SAME_SIZE
#if defined(IL_WORDS_LITTLEENDIAN)
	#define CVM_LITTLE_ENDIAN
#endif
#endif
#if defined(__powerpc__) || defined(powerpc) || \
		defined(__powerpc) || defined(PPC)
	#define	CVM_PPC
#endif
#if defined(__x86_64__) || defined(__x86_64) 
	#define CVM_X86_64
	#define CVM_LITTLE_ENDIAN
	#define	CVM_LONGS_ALIGNED_WORD
#endif
#if defined(__ia64) || defined(__ia64__)
	#define	CVM_IA64
#endif

/*
 * Determine the style of interpreter to use, which is one
 * of "IL_CVM_SWITCH", "IL_CVM_TOKEN", or "IL_CVM_DIRECT".
 * These correspond to "simple switch loop", "token threaded
 * based on bytecode", and "direct threaded based on address".
 */
#ifdef HAVE_COMPUTED_GOTO
	#ifdef IL_CVM_DIRECT_ALLOWED
		#define	IL_CVM_DIRECT
		#define	IL_CVM_FLAVOUR "Direct Threaded"
		#if defined(PIC) && defined(HAVE_PIC_COMPUTED_GOTO)
			#define	IL_CVM_PIC_DIRECT
		#endif
	#else
		#define	IL_CVM_TOKEN
		#define	IL_CVM_FLAVOUR "Token Threaded"
		#if defined(PIC) && defined(HAVE_PIC_COMPUTED_GOTO)
			#define	IL_CVM_PIC_TOKEN
		#endif
	#endif
#else /* !HAVE_COMPUTED_GOTO */
	#define	IL_CVM_SWITCH
	#define	IL_CVM_FLAVOUR "Switch Loop"
#endif /* !HAVE_COMPUTED_GOTO */

/*
 * Declare the code necessary to export the direct threading
 * tables from "_ILCVMInterpreter", and to extract addresses
 * for specific opcodes in the CVM coder.
 */
#ifdef IL_CVM_DIRECT
	#ifdef IL_CVM_PIC_DIRECT

		/* We are building a direct interpreter with PIC labels */
		extern const int *_ILCVMMainLabelTable;
		extern const int *_ILCVMPrefixLabelTable;
		extern void *_ILCVMBaseLabel;

		#define CVM_DEFINE_TABLES()	\
					const int *_ILCVMMainLabelTable; \
					const int *_ILCVMPrefixLabelTable; \
					void *_ILCVMBaseLabel

		#define	CVM_EXPORT_TABLES()	\
					do { \
						_ILCVMMainLabelTable = main_label_table; \
						_ILCVMPrefixLabelTable = prefix_label_table; \
						_ILCVMBaseLabel = &&COP_NOP_label; \
					} while (0)

		#define	CVM_LABEL_FOR_OPCODE(opcode)	\
					(_ILCVMBaseLabel + _ILCVMMainLabelTable[(opcode)])
		#define	CVMP_LABEL_FOR_OPCODE(opcode)	\
					(_ILCVMBaseLabel + _ILCVMPrefixLabelTable[(opcode)])

	#else /* !IL_CVM_PIC_DIRECT */

		/* We are building a direct interpreter from non-PIC labels */
		extern void **_ILCVMMainLabelTable;
		extern void **_ILCVMPrefixLabelTable;

		#define CVM_DEFINE_TABLES()	\
					void **_ILCVMMainLabelTable; \
					void **_ILCVMPrefixLabelTable

		#define	CVM_EXPORT_TABLES()	\
					do { \
						_ILCVMMainLabelTable = main_label_table; \
						_ILCVMPrefixLabelTable = prefix_label_table; \
					} while (0)

		#define	CVM_LABEL_FOR_OPCODE(opcode)	\
					(_ILCVMMainLabelTable[(opcode)])
		#define	CVMP_LABEL_FOR_OPCODE(opcode)	\
					(_ILCVMPrefixLabelTable[(opcode)])

	#endif /* !IL_CVM_PIC_DIRECT */
#else /* !IL_CVM_DIRECT */

	/* We are building a non-direct interpreter */
	#define	CVM_DEFINE_TABLES()
	#define	CVM_EXPORT_TABLES()

#endif /* !IL_CVM_DIRECT */

/*
 * Determine if we can unroll the direct threaded interpreter.
 */
#if defined(IL_CVM_DIRECT) && defined(CVM_X86) && \
	defined(__GNUC__) && !defined(IL_NO_ASM) && \
	!defined(IL_CVM_PROFILE_CVM_VAR_USAGE) && \
	defined(IL_CONFIG_UNROLL)
#define	IL_CVM_DIRECT_UNROLLED
#undef	IL_CVM_FLAVOUR
#define	IL_CVM_FLAVOUR "Direct Unrolled (x86)"
#endif
#if defined(IL_CVM_DIRECT) && defined(CVM_X86_64) && \
	defined(__GNUC__) && !defined(IL_NO_ASM) && \
	!defined(IL_CVM_PROFILE_CVM_VAR_USAGE) && \
	defined(IL_CONFIG_UNROLL)
#define	IL_CVM_DIRECT_UNROLLED
#undef	IL_CVM_FLAVOUR
#define	IL_CVM_FLAVOUR "Direct Unrolled (amd64)"
#endif
#if defined(IL_CVM_DIRECT) && defined(CVM_ARM) && \
	defined(__GNUC__) && !defined(IL_NO_ASM) && \
	!defined(IL_CVM_PROFILE_CVM_VAR_USAGE) && \
	defined(IL_CONFIG_UNROLL)
#define	IL_CVM_DIRECT_UNROLLED
#undef	IL_CVM_FLAVOUR
#define	IL_CVM_FLAVOUR "Direct Unrolled (ARM)"
#endif
#if defined(IL_CVM_DIRECT) && defined(CVM_PPC) && \
	defined(__GNUC__) && !defined(IL_NO_ASM) && \
	!defined(IL_CVM_PROFILE_CVM_VAR_USAGE) && \
	defined(IL_CONFIG_UNROLL)
#define	IL_CVM_DIRECT_UNROLLED
#undef	IL_CVM_FLAVOUR
#define	IL_CVM_FLAVOUR "Direct Unrolled (PPC)"
#endif
#if 0	/* remove this once ia64 unroller is finished */
#if defined(IL_CVM_DIRECT) && defined(CVM_IA64) && \
	defined(__GNUC__) && !defined(IL_NO_ASM) && \
	!defined(IL_CVM_PROFILE_CVM_VAR_USAGE) && \
	defined(IL_CONFIG_UNROLL)
#define	IL_CVM_DIRECT_UNROLLED
#undef	IL_CVM_FLAVOUR
#define	IL_CVM_FLAVOUR "Direct Unrolled (ia64)"
#endif
#endif

/*
 * Macros that can be used to bind important interpreter loop
 * variables to specific CPU registers for greater speed.
 * If we don't do this, then gcc generates VERY bad code for
 * the inner interpreter loop.  It just isn't smart enough to
 * figure out that "pc", "stacktop", and "frame" are the
 * best values to put into registers.
 * If unrolling is done the macro CVM_VMBREAK_BARRIER should
 * be defined to clobber all registers used by the unrolled
 * code that are not saved to the stack before they are used.
 * This makes sure that no values are stored in these registers
 * between execution of two opcodes. This can easily happen
 * with an optimizing compiler like gcc.
 */
#if !defined(IL_NO_REGISTERS_USED)
#if defined(CVM_X86) && defined(__GNUC__) && !defined(IL_NO_ASM)

	#define CVM_REGISTER_ASM_X86 1

	#define CVM_REGISTER_ASM_PC(x)			register x asm ("esi")
	#define CVM_REGISTER_ASM_STACK(x)		register x asm ("edi")
	#define CVM_REGISTER_ASM_FRAME(x)		register x asm ("ebx")
#if defined(IL_CVM_DIRECT_UNROLLED)
	/*
	 * This is the barrier needed to make the interpreter work.
	 */
	#define CVM_VMBREAK_BARRIER()	\
		__asm__ __volatile__ ("" : : : "ecx", "edx")
	/*
	 * The extended barrier makes gcc (version 4.4) build faster code.
	 * (Tested on an intel atom cpu box)
	 * As of gcc 4.4 the interpreter segfaults if built with -O3 on x86
	 *
	#define CVM_VMBREAK_BARRIER()	\
	__asm__ __volatile__ ("" : : : "ecx", "edx", "memory")
	 *
	 * Define arch specific replacements for the cvm interpreter switch
	 * , case and break statements.
	 * For these x86 specific versions all variables with a *VOLATILE* macro
	 * that are not kept in a register (see CVM_REGISTER* macros) MUST be defined
	 * volatile.
	 *
	 * NOTE: The goto statement in the macro is just for the compiler.
	 *
	#define X86_CGOTO(pc) do { __asm__ __volatile__ ("jmp *(%0)" : : "r" (pc)); \
							   goto ** ((void **)(pc)); } while(0)
	#define VM_CGOTO_PREFIXSWITCH(val) X86_CGOTO(pc)
	#define VM_CGOTO_BREAK(val) X86_CGOTO(pc)
	#define VM_CGOTO_BREAKNOEND(val) X86_CGOTO(pc)
	*/
#endif
#elif defined(CVM_X86_64) && defined(__GNUC__) && !defined(IL_NO_ASM)

	#define CVM_REGISTER_ASM_X86_64 1

	/* 16 registers - so we can avoid using esi, edi and ebx. */
	#define CVM_REGISTER_ASM_PC(x)			register x asm("r12")
	#define CVM_REGISTER_ASM_STACK(x)		register x asm("r14") 
	#define CVM_REGISTER_ASM_FRAME(x)		register x asm("r15") 
#if defined(IL_CVM_DIRECT_UNROLLED)
	#define CVM_VMBREAK_BARRIER()	\
		__asm__ __volatile__ ("" : : : "rax", "rbx", "rcx", "rdx", "rsi", "rdi")
#endif
#elif defined(CVM_ARM) && defined(__GNUC__) && !defined(IL_NO_ASM)

	#define CVM_REGISTER_ASM_ARM 1

    #define CVM_REGISTER_ASM_PC(x)			register x asm ("r4")
    #define CVM_REGISTER_ASM_STACK(x)		register x asm ("r5")
    #define CVM_REGISTER_ASM_FRAME(x)		register x asm ("r6")
#if defined(IL_CVM_DIRECT_UNROLLED)
	/*
	 * NOTE: The "memory" clobber is only gor optimization purposes with gcc 4.4.
	 * It may be removed again if performance is bad with other gcc versions.
	 */
	#define CVM_VMBREAK_BARRIER()   \
		__asm__ __volatile__ ("" : : : "r0", "r1", "r2", "r3", "r12", "memory")
#endif
#elif defined(CVM_PPC) && defined(__GNUC__) && !defined(IL_NO_ASM)

	#define CVM_REGISTER_ASM_PPC 1

	#define CVM_REGISTER_ASM_PC(x)			register x asm ("r18")
	#define CVM_REGISTER_ASM_STACK(x)		register x asm ("r19")
	#define CVM_REGISTER_ASM_FRAME(x)		register x asm ("r20")
#if defined(IL_CVM_DIRECT_UNROLLED)
	#define CVM_VMBREAK_BARRIER()   \
		__asm__ __volatile__ ("" : : : "r3", "r4", "r5", "r6", "r7", "r8", "r9", "r10", "r11", "r12", "memory")
#endif
#elif defined(CVM_IA64) && defined(__GNUC__) && !defined(IL_NO_ASM)

	#define CVM_REGISTER_ASM_IA64 1

	#define CVM_REGISTER_ASM_PC(x)			register x asm ("r4")
	#define CVM_REGISTER_ASM_STACK(x)		register x asm ("r5")
	#define CVM_REGISTER_ASM_FRAME(x)		register x asm ("r6")
#else
	#define IL_NO_REGISTERS_USED 1
#endif
#endif /* !defined(IL_NO_REGISTERS_USED) */

#if defined(IL_NO_REGISTERS_USED)
	#define CVM_REGISTER_ASM_PC(x)			x
	#define CVM_REGISTER_ASM_STACK(x)		x
	#define CVM_REGISTER_ASM_FRAME(x)		x
#endif

#if defined(IL_CVM_DIRECT)
#if !defined(CVM_VMBREAK_BARRIER)
	#define CVM_VMBREAK_BARRIER()
#endif
#endif

/*
 * The constructor offset value.
 */
#ifdef IL_CVM_DIRECT
	#define	CVM_CTOR_OFFSET		(3 * sizeof(void *))
#else
	#define	CVM_CTOR_OFFSET		6
#endif

/*
 * If the interpeter does not use explicit register declarations it might be
 * necessary to flush the variables on each opcode.
 */
#if defined(IL_CVM_DIRECT_UNROLLED) && defined(IL_NO_REGISTERS_USED)
#if defined(CVM_X86)
#define IL_VMCASE_BARRIER
#endif
#endif

/*
 * predict the outcome of an expression to allow the compiler to
 * optimize better.
 * The result must be a compile time constant. Usually either 0 or 1.
 */
#if (__GNUC__ >= 3)
#define IL_EXPECT(expr, result) __builtin_expect((expr), (result))
#else
#define IL_EXPECT(expr, result) (expr)
#endif

#ifdef	__cplusplus
};
#endif

#endif /* IL_USE_CVM */

#endif	/* _ENGINE_CVM_CONFIG_H */
