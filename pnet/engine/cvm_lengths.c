/*
 * cvm_lengths.c - Table that defines the lengths of all CVM opcodes.
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

#include "engine.h"
#include "cvm_config.h"
#include "cvm_format.h"

#ifdef	__cplusplus
extern	"C" {
#endif

unsigned char const _ILCVMLengths[512] = {

	/*
	 * Simple opcodes.
	 */
	/* nop */				CVM_LEN_NONE,

	/*
	 * Local variable opcodes.
	 */
	/* iload_0 */			CVM_LEN_NONE,
	/* iload_1 */			CVM_LEN_NONE,
	/* iload_2 */			CVM_LEN_NONE,
	/* iload_3 */			CVM_LEN_NONE,
	/* iload */				CVM_LEN_WIDE_SMALL,
	/* pload_0 */			CVM_LEN_NONE,
	/* pload_1 */			CVM_LEN_NONE,
	/* pload_2 */			CVM_LEN_NONE,
	/* pload_3 */			CVM_LEN_NONE,
	/* pload */				CVM_LEN_WIDE_SMALL,
	/* istore_0 */			CVM_LEN_NONE,
	/* istore_1 */			CVM_LEN_NONE,
	/* istore_2 */			CVM_LEN_NONE,
	/* istore_3 */			CVM_LEN_NONE,
	/* istore */			CVM_LEN_WIDE_SMALL,
	/* pstore_0 */			CVM_LEN_NONE,
	/* pstore_1 */			CVM_LEN_NONE,
	/* pstore_2 */			CVM_LEN_NONE,
	/* pstore_3 */			CVM_LEN_NONE,
	/* pstore */			CVM_LEN_WIDE_SMALL,
	/* mload */				CVM_LEN_DWIDE_SMALL,
	/* mstore */			CVM_LEN_DWIDE_SMALL,
	/* waddr */				CVM_LEN_WIDE_SMALL,
	/* maddr */				CVM_LEN_WIDE_SMALL,

	/*
	 * Argument fixups.
	 */
	/* bfixup */			CVM_LEN_WIDE_SMALL,
	/* sfixup */			CVM_LEN_WIDE_SMALL,
	/* ffixup */			CVM_LEN_WIDE_SMALL,
	/* dfixup */			CVM_LEN_WIDE_SMALL,

	/*
	 * Local variable allocation.
	 */
	/* mk_local_1 */		CVM_LEN_NONE,
	/* mk_local_2 */		CVM_LEN_NONE,
	/* mk_local_3 */		CVM_LEN_NONE,
	/* mk_local_n */		CVM_LEN_WIDE_SMALL,

	/*
	 * Pointer reads and writes.
	 */
	/* bread */				CVM_LEN_NONE,
	/* ubread */			CVM_LEN_NONE,
	/* sread */				CVM_LEN_NONE,
	/* usread */			CVM_LEN_NONE,
	/* iread */				CVM_LEN_NONE,
	/* fread */				CVM_LEN_NONE,
	/* dread */				CVM_LEN_NONE,
	/* pread */				CVM_LEN_NONE,
	/* mread */				CVM_LEN_WIDE_SMALL,
	/* bwrite */			CVM_LEN_NONE,
	/* swrite */			CVM_LEN_NONE,
	/* iwrite */			CVM_LEN_NONE,
	/* fwrite */			CVM_LEN_NONE,
	/* dwrite */			CVM_LEN_NONE,
	/* pwrite */			CVM_LEN_NONE,
	/* mwrite */			CVM_LEN_WIDE_SMALL,
	/* bwrite_r */			CVM_LEN_NONE,
	/* swrite_r */			CVM_LEN_NONE,
	/* iwrite_r */			CVM_LEN_NONE,
	/* fwrite_r */			CVM_LEN_NONE,
	/* dwrite_r */			CVM_LEN_NONE,
	/* pwrite_r */			CVM_LEN_NONE,
	/* mwrite_r */			CVM_LEN_WIDE_SMALL,

	/*
	 * Stack handling.
	 */
	/* dup */				CVM_LEN_NONE,
	/* dup2 */				CVM_LEN_NONE,
	/* dup_n */				CVM_LEN_WIDE_SMALL,
	/* dup_word_n */		CVM_LEN_WIDE_SMALL,
	/* pop */				CVM_LEN_NONE,
	/* pop2 */				CVM_LEN_NONE,
	/* pop_n */				CVM_LEN_WIDE_SMALL,
	/* squash */			CVM_LEN_DWIDE_SMALL,
	/* ckheight */			CVM_LEN_NONE,
	/* ckheight_n */		CVM_LEN_WIDE_SMALL,
	/* set_num_args */		CVM_LEN_WIDE_SMALL,

	/*
	 * Arithmetic operators.
	 */
	/* iadd */				CVM_LEN_NONE,
	/* iadd_ovf */			CVM_LEN_NONE,
	/* iadd_ovf_un */		CVM_LEN_NONE,
	/* isub */				CVM_LEN_NONE,
	/* isub_ovf */			CVM_LEN_NONE,
	/* isub_ovf_un */		CVM_LEN_NONE,
	/* imul */				CVM_LEN_NONE,
	/* imul_ovf */			CVM_LEN_NONE,
	/* imul_ovf_un */		CVM_LEN_NONE,
	/* idiv */				CVM_LEN_NONE,
	/* idiv_un */			CVM_LEN_NONE,
	/* irem */				CVM_LEN_NONE,
	/* irem_un */			CVM_LEN_NONE,
	/* ineg */				CVM_LEN_NONE,
	/* ladd */				CVM_LEN_NONE,
	/* ladd_ovf */			CVM_LEN_NONE,
	/* ladd_ovf_un */		CVM_LEN_NONE,
	/* lsub */				CVM_LEN_NONE,
	/* lsub_ovf */			CVM_LEN_NONE,
	/* lsub_ovf_un */		CVM_LEN_NONE,
	/* lmul */				CVM_LEN_NONE,
	/* lmul_ovf */			CVM_LEN_NONE,
	/* lmul_ovf_un */		CVM_LEN_NONE,
	/* ldiv */				CVM_LEN_NONE,
	/* ldiv_un */			CVM_LEN_NONE,
	/* lrem */				CVM_LEN_NONE,
	/* lrem_un */			CVM_LEN_NONE,
	/* lneg */				CVM_LEN_NONE,
	/* fadd */				CVM_LEN_NONE,
	/* fsub */				CVM_LEN_NONE,
	/* fmul */				CVM_LEN_NONE,
	/* fdiv */				CVM_LEN_NONE,
	/* frem */				CVM_LEN_NONE,
	/* fneg */				CVM_LEN_NONE,

	/*
	 * Bitwise operators.
	 */
	/* iand */				CVM_LEN_NONE,
	/* ior */				CVM_LEN_NONE,
	/* ixor */				CVM_LEN_NONE,
	/* inot */				CVM_LEN_NONE,
	/* ishl */				CVM_LEN_NONE,
	/* ishr */				CVM_LEN_NONE,
	/* ishr_un */			CVM_LEN_NONE,
	/* land */				CVM_LEN_NONE,
	/* lor */				CVM_LEN_NONE,
	/* lxor */				CVM_LEN_NONE,
	/* lnot */				CVM_LEN_NONE,
	/* lshl */				CVM_LEN_NONE,
	/* lshr */				CVM_LEN_NONE,
	/* lshr_un */			CVM_LEN_NONE,

	/*
	 * Conversion operators.
	 */
	/* i2b */				CVM_LEN_NONE,
	/* i2ub */				CVM_LEN_NONE,
	/* i2s */				CVM_LEN_NONE,
	/* i2us */				CVM_LEN_NONE,
	/* i2l */				CVM_LEN_NONE,
	/* iu2l */				CVM_LEN_NONE,
	/* i2f */				CVM_LEN_NONE,
	/* iu2f */				CVM_LEN_NONE,
	/* l2i */				CVM_LEN_NONE,
	/* l2f */				CVM_LEN_NONE,
	/* lu2f */				CVM_LEN_NONE,
	/* f2i */				CVM_LEN_NONE,
	/* f2iu */				CVM_LEN_NONE,
	/* f2l */				CVM_LEN_NONE,
	/* f2lu */				CVM_LEN_NONE,
	/* f2f */				CVM_LEN_NONE,
	/* f2d */				CVM_LEN_NONE,
	/* i2p_lower */			CVM_LEN_WIDE_SMALL,

	/*
	 * Pointer arithmetic and manipulation.
	 */
	/* padd_offset */		CVM_LEN_BYTE,
	/* padd_offset_n */		CVM_LEN_WIDE_SMALL,
	/* padd_i4 */			CVM_LEN_NONE,
	/* padd_i4_r */			CVM_LEN_NONE,
	/* padd_i8 */			CVM_LEN_NONE,
	/* padd_i8_r */			CVM_LEN_NONE,
	/* psub */				CVM_LEN_NONE,
	/* psub_i4 */			CVM_LEN_NONE,
	/* psub_i8 */			CVM_LEN_NONE,
	/* cknull */			CVM_LEN_NONE,
	/* cknull_n */			CVM_LEN_WIDE_SMALL,
	/* ldrva */				CVM_LEN_WORD,

	/*
	 * Constant opcodes.
	 */
	/* ldnull */			CVM_LEN_NONE,
	/* ldc_i4_m1 */			CVM_LEN_NONE,
	/* ldc_i4_0 */			CVM_LEN_NONE,
	/* ldc_i4_1 */			CVM_LEN_NONE,
	/* ldc_i4_2 */			CVM_LEN_NONE,
	/* ldc_i4_3 */			CVM_LEN_NONE,
	/* ldc_i4_4 */			CVM_LEN_NONE,
	/* ldc_i4_5 */			CVM_LEN_NONE,
	/* ldc_i4_6 */			CVM_LEN_NONE,
	/* ldc_i4_7 */			CVM_LEN_NONE,
	/* ldc_i4_8 */			CVM_LEN_NONE,
	/* ldc_i4_s */			CVM_LEN_BYTE,
	/* ldc_i4 */			CVM_LEN_WORD,
	/* ldc_i8 */			CVM_LEN_LONG,
	/* ldc_r4 */			CVM_LEN_FLOAT,
	/* ldc_r8 */			CVM_LEN_DOUBLE,

	/*
	 * Branch opcodes.
	 */
	/* br */				CVM_LEN_BRANCH,
	/* beq */				CVM_LEN_BRANCH,
	/* bne */				CVM_LEN_BRANCH,
	/* blt */				CVM_LEN_BRANCH,
	/* blt_un */			CVM_LEN_BRANCH,
	/* ble */				CVM_LEN_BRANCH,
	/* ble_un */			CVM_LEN_BRANCH,
	/* bgt */				CVM_LEN_BRANCH,
	/* bgt_un */			CVM_LEN_BRANCH,
	/* bge */				CVM_LEN_BRANCH,
	/* bge_un */			CVM_LEN_BRANCH,
	/* brtrue */			CVM_LEN_BRANCH,
	/* brfalse */			CVM_LEN_BRANCH,
	/* brnull */			CVM_LEN_BRANCH,
	/* brnonnull */			CVM_LEN_BRANCH,
	/* br_peq */			CVM_LEN_BRANCH,
	/* br_pne */			CVM_LEN_BRANCH,
	/* br_long */			CVM_LEN_BRANCH,
	/* switch */			0,

	/*
	 * Array opcodes.
	 */
	/* bread_elem */		CVM_LEN_NONE,
	/* ubread_elem */		CVM_LEN_NONE,
	/* sread_elem */		CVM_LEN_NONE,
	/* usread_elem */		CVM_LEN_NONE,
	/* iread_elem */		CVM_LEN_NONE,
	/* pread_elem */		CVM_LEN_NONE,
	/* bwrite_elem */		CVM_LEN_NONE,
	/* swrite_elem */		CVM_LEN_NONE,
	/* iwrite_elem */		CVM_LEN_NONE,
	/* pwrite_elem */		CVM_LEN_NONE,
	/* elem_addr_shift_i4 */ CVM_LEN_BYTE,
	/* elem_addr_mul_i4 */	CVM_LEN_WORD,
	/* ckarray_load_i8 */	CVM_LEN_NONE,
	/* ckarray_store_i8 */ 	CVM_LEN_BYTE2,
	/* array_len */			CVM_LEN_NONE,

	/*
	 * Field opcodes.
	 */
	/* bread_field */		CVM_LEN_BYTE,
	/* ubread_field */		CVM_LEN_BYTE,
	/* sread_field */		CVM_LEN_BYTE,
	/* usread_field */		CVM_LEN_BYTE,
	/* iread_field */		CVM_LEN_BYTE,
	/* pread_field */		CVM_LEN_BYTE,
	/* bwrite_field */		CVM_LEN_BYTE,
	/* swrite_field */		CVM_LEN_BYTE,
	/* iwrite_field */		CVM_LEN_BYTE,
	/* pwrite_field */		CVM_LEN_BYTE,
	/* pread_this */		CVM_LEN_BYTE,
	/* iread_this */		CVM_LEN_BYTE,

	/*
	 * Call management opcodes.
	 */
	/* call */				CVM_LEN_PTR,
	/* call_ctor */			CVM_LEN_PTR,
	/* call_native */		CVM_LEN_PTR2,
	/* call_native_void */	CVM_LEN_PTR2,
	/* call_native_raw */	CVM_LEN_PTR2,
	/* call_native_void_raw */ CVM_LEN_PTR2,
	/* call_virtual */		CVM_LEN_DWIDE_SMALL,
	/* call_interface */	CVM_LEN_DWIDE_PTR_SMALL,
	/* return */			CVM_LEN_NONE,
	/* return_1 */			CVM_LEN_NONE,
	/* return_2 */			CVM_LEN_NONE,
	/* return_n */			CVM_LEN_WORD,
	/* jsr */				CVM_LEN_BRANCH,
	/* ret_jsr */			CVM_LEN_NONE,
	/* push_thread */		CVM_LEN_NONE,
	/* push_thread_raw */	CVM_LEN_NONE,
	/* pushdown */			CVM_LEN_WORD,
	/* calli */				CVM_LEN_NONE,
	/* jmpi */				CVM_LEN_NONE,

	/*
	 * Class-related opcodes.
	 */
	/* castclass */			CVM_LEN_PTR,
	/* isinst */			CVM_LEN_PTR,
	/* castinterface */		CVM_LEN_PTR,
	/* isinterface */		CVM_LEN_PTR,
	/* get_static */		CVM_LEN_PTR,
	/* new */				CVM_LEN_NONE,
	/* new_value */			CVM_LEN_DWIDE_SMALL,
	/* ldstr */				CVM_LEN_WORD,
	/* ldtoken */			CVM_LEN_PTR,
	/* box */				CVM_LEN_WIDE_PTR_SMALL,
	/* box_ptr */			CVM_LEN_WIDE_PTR_SMALL,

	/*
	 * Memory-related opcodes.
	 */
	/* memcpy */			CVM_LEN_WIDE_SMALL,
	/* memmove */			CVM_LEN_NONE,
	/* memzero */			CVM_LEN_WIDE_SMALL,
	/* memset */			CVM_LEN_NONE,

	/*
	 * Argument packing for native calls.
	 */
	/* waddr_native_m1 */	CVM_LEN_WIDE_SMALL,
	/* waddr_native_0 */	CVM_LEN_WIDE_SMALL,
	/* waddr_native_1 */	CVM_LEN_WIDE_SMALL,
	/* waddr_native_2 */	CVM_LEN_WIDE_SMALL,
	/* waddr_native_3 */	CVM_LEN_WIDE_SMALL,
	/* waddr_native_4 */	CVM_LEN_WIDE_SMALL,
	/* waddr_native_5 */	CVM_LEN_WIDE_SMALL,
	/* waddr_native_6 */	CVM_LEN_WIDE_SMALL,
	/* waddr_native_7 */	CVM_LEN_WIDE_SMALL,

	/*
	 * Quick byte loads and stores.
	 */
	/* bload */				CVM_LEN_BYTE,
	/* bstore */			CVM_LEN_BYTE,

	/*
	 * Reserved opcodes.
	 */
	/* reserved_fc */		CVM_LEN_NONE,

	/*
	 * Make the next instruction wider.
	 */
	/* wide */				0,

	/*
	 * Breakpoint handling.
	 */
	/* break */				CVM_LEN_BREAK,

	/*
	 * Prefix for two-byte instruction opcodes.
	 */
	/* prefix */			0,

	/*
	 * Reserved opcodes.
	 */
	/* preserved_00 */		CVMP_LEN_NONE,

	/*
	 * Prefixed comparison opcodes.
	 */
	/* icmp */				CVMP_LEN_NONE,
	/* icmp_un */			CVMP_LEN_NONE,
	/* lcmp */				CVMP_LEN_NONE,
	/* lcmp_un */			CVMP_LEN_NONE,
	/* fcmpl */				CVMP_LEN_NONE,
	/* fcmpg */				CVMP_LEN_NONE,
	/* pcmp */				CVMP_LEN_NONE,
	/* seteq */				CVMP_LEN_NONE,
	/* setne */				CVMP_LEN_NONE,
	/* setlt */				CVMP_LEN_NONE,
	/* setle */				CVMP_LEN_NONE,
	/* setgt */				CVMP_LEN_NONE,
	/* setge */				CVMP_LEN_NONE,

	/*
	 * Prefixed array opcodes.
	 */
	/* lread_elem */		CVMP_LEN_NONE,
	/* fread_elem */		CVMP_LEN_NONE,
	/* dread_elem */		CVMP_LEN_NONE,
	/* lwrite_elem */		CVMP_LEN_NONE,
	/* fwrite_elem */		CVMP_LEN_NONE,
	/* dwrite_elem */		CVMP_LEN_NONE,
	/* get2d */				CVMP_LEN_NONE,
	/* set2d */				CVMP_LEN_WORD,

	/*
	 * Prefixed call management opcodes.
	 */
	/* tail_call */			CVMP_LEN_PTR,
	/* tail_calli */		CVMP_LEN_NONE,
	/* tail_callvirt */		CVMP_LEN_WORD2,
	/* tail_callintf */		CVMP_LEN_WORD2_PTR,
	/* ldftn */				CVMP_LEN_PTR,
	/* ldvirtftn */			CVMP_LEN_WORD,
	/* ldinterfftn */		CVMP_LEN_WORD_PTR,
	/* pack_varargs */		CVMP_LEN_WORD2_PTR,

	/*
	 * Prefixed exception handling opcodes.
	 */
	/* enter_try */			CVMP_LEN_NONE,
	/* throw */				CVMP_LEN_NONE,
	/* throw_caller */		CVMP_LEN_NONE,
	/* set_stack_trace */	CVMP_LEN_NONE,

	/*
	 * Prefixed typedref handling opcodes.
	 */
	/* mkrefany */			CVMP_LEN_PTR,
	/* refanyval */			CVMP_LEN_PTR,
	/* refanytype */		CVMP_LEN_NONE,

	/*
	 * Prefixed conversion opcodes.
	 */
	/* i2b_ovf */			CVMP_LEN_NONE,
	/* i2ub_ovf */			CVMP_LEN_NONE,
	/* iu2b_ovf */			CVMP_LEN_NONE,
	/* iu2ub_ovf */			CVMP_LEN_NONE,
	/* i2s_ovf */			CVMP_LEN_NONE,
	/* i2us_ovf */			CVMP_LEN_NONE,
	/* iu2s_ovf */			CVMP_LEN_NONE,
	/* iu2us_ovf */			CVMP_LEN_NONE,
	/* i2iu_ovf */			CVMP_LEN_NONE,
	/* iu2i_ovf */			CVMP_LEN_NONE,
	/* i2ul_ovf */			CVMP_LEN_NONE,
	/* l2i_ovf */			CVMP_LEN_NONE,
	/* l2ui_ovf */			CVMP_LEN_NONE,
	/* lu2i_ovf */			CVMP_LEN_NONE,
	/* lu2iu_ovf */			CVMP_LEN_NONE,
	/* l2ul_ovf */			CVMP_LEN_NONE,
	/* lu2l_ovf */			CVMP_LEN_NONE,
	/* f2i_ovf */			CVMP_LEN_NONE,
	/* f2iu_ovf */			CVMP_LEN_NONE,
	/* f2l_ovf */			CVMP_LEN_NONE,
	/* f2lu_ovf */			CVMP_LEN_NONE,
	/* i2b_aligned */		CVMP_LEN_NONE,
	/* i2s_aligned */		CVMP_LEN_NONE,
	/* f2f_aligned */		CVMP_LEN_NONE,
	/* f2d_aligned */		CVMP_LEN_NONE,

	/*
	 * Prefixed arithmetic opcodes.
	 */
	/* ckfinite */			CVMP_LEN_NONE,

	/*
	 * Marhsalling conversion opcodes.
	 */
	/* str2ansi */			CVMP_LEN_NONE,
	/* str2utf8 */			CVMP_LEN_NONE,
	/* ansi2str */			CVMP_LEN_NONE,
	/* utf82str */			CVMP_LEN_NONE,
	/* str2utf16 */			CVMP_LEN_NONE,
	/* utf162str */			CVMP_LEN_NONE,
	/* delegate2fnptr */	CVMP_LEN_NONE,
	/* array2ptr */			CVMP_LEN_NONE,
	/* refarray2ansi */		CVMP_LEN_NONE,
	/* refarray2utf8 */		CVMP_LEN_NONE,
	/* tocustom */			CVMP_LEN_WORD2_PTR2,
	/* fromcustom */		CVMP_LEN_WORD2_PTR2,
	/* array2ansi */		CVMP_LEN_NONE,
	/* array2utf8 */		CVMP_LEN_NONE,
	/* struct2native */		CVMP_LEN_PTR,

	/*
	 * Inline method replacements.
	 */
	/* string_concat_2 */	CVMP_LEN_NONE,
	/* string_concat_3 */	CVMP_LEN_NONE,
	/* string_concat_4 */	CVMP_LEN_NONE,
	/* string_eq */			CVMP_LEN_NONE,
	/* string_ne */			CVMP_LEN_NONE,
	/* string_get_char */	CVMP_LEN_NONE,
	/* type_from_handle */	CVMP_LEN_NONE,
	/* monitor_enter */		CVMP_LEN_NONE,
	/* monitor_exit */		CVMP_LEN_NONE,
	/* append_char */		CVMP_LEN_PTR,
	/* is_white_space */	CVMP_LEN_NONE,

	/*
	 * Binary value fixups.
	 */
	/* fix_i4_i */			CVMP_LEN_NONE,
	/* fix_i4_u */			CVMP_LEN_NONE,

	/*
	 * Trigger method unrolling.
	 */
	/* unroll_method */		CVMP_LEN_NONE,

	/*
	 * Allocate local stack space.
	 */
	/* local_alloc */		CVMP_LEN_NONE,

	/*
	 * Method profiling.
	 */
	/* profile_count */		CVMP_LEN_NONE,

	/*
	 * Thread static handling.
	 */
	/* thread_static */		CVMP_LEN_WORD2,
	
	/*
	 * Argument packing for native calls.
	 */
	/* waddr_native_n */	CVMP_LEN_WORD2,

	/*
	 * Method trace opcodes
	 */

	/* trace_in */			CVMP_LEN_WORD,
	/* trace_out */			CVMP_LEN_WORD,

	/*
	 * Reserved opcodes.
	 */
	/* leave_catch */		CVMP_LEN_NONE,
	/* ret_from_finally */	CVMP_LEN_NONE,
	/* preserved_64 */		CVMP_LEN_NONE,
	/* preserved_65 */		CVMP_LEN_NONE,
	/* preserved_66 */		CVMP_LEN_NONE,
	/* preserved_67 */		CVMP_LEN_NONE,
	/* preserved_68 */		CVMP_LEN_NONE,
	/* preserved_69 */		CVMP_LEN_NONE,
	/* preserved_6a */		CVMP_LEN_NONE,
	/* preserved_6b */		CVMP_LEN_NONE,
	/* preserved_6c */		CVMP_LEN_NONE,
	/* abs_i4 */			CVMP_LEN_NONE,
	/* abs_r4 */			CVMP_LEN_NONE,
	/* abs_r8 */			CVMP_LEN_NONE,

	/* asin */				CVMP_LEN_NONE,
	/* atan */				CVMP_LEN_NONE,
	/* atan2 */				CVMP_LEN_NONE,
	/* ceiling */			CVMP_LEN_NONE,
	/* cos */				CVMP_LEN_NONE,
	/* cosh */				CVMP_LEN_NONE,
	/* exp */				CVMP_LEN_NONE,
	/* floor */				CVMP_LEN_NONE,
	/* ieeeremainder */		CVMP_LEN_NONE,
	/* log */				CVMP_LEN_NONE,
	/* log10 */				CVMP_LEN_NONE,
	/* min_i4 */			CVMP_LEN_NONE,
	/* max_i4 */			CVMP_LEN_NONE,
	/* min_r4 */			CVMP_LEN_NONE,
	/* max_r4 */			CVMP_LEN_NONE,
	/* min_r8 */			CVMP_LEN_NONE,

	/* max_r8 */			CVMP_LEN_NONE,
	/* pow */				CVMP_LEN_NONE,
	/* round */				CVMP_LEN_NONE,
	/* sign_i4 */			CVMP_LEN_NONE,
	/* sign_r4 */			CVMP_LEN_NONE,
	/* sign_r8 */			CVMP_LEN_NONE,
	/* sin */				CVMP_LEN_NONE,
	/* sinh */				CVMP_LEN_NONE,
	/* sqrt */				CVMP_LEN_NONE,
	/* tan */				CVMP_LEN_NONE,
	/* tanh */				CVMP_LEN_NONE,

	/*
	 * Unroller support opcodes.
	 */

	/* unroll_stack */		CVMP_LEN_NONE,
	/* unroll_stack_return */ CVMP_LEN_NONE,

	/*
	 * Generics support opcodes.
	 */

	/* repl_word_n */		CVMP_LEN_WORD,
	/* call_virtgen */		CVMP_LEN_WORD2,

	/*
	 * Inlined array functions.
	 */

	/* sarray_copy_aai4 */	CVMP_LEN_WORD,
	/* sarray_copy_ai4ai4i4 */ CVMP_LEN_WORD,
	/* sarray_clear_ai4i4 */ CVMP_LEN_WORD,

	/*
	 * Enghanced method profiling opcodes.
	 */

	/* profile_start */		CVMP_LEN_NONE,
	/* profile_end */		CVMP_LEN_NONE,

	/* preserved_94 */		CVMP_LEN_NONE,
	/* preserved_95 */		CVMP_LEN_NONE,
	/* preserved_96 */		CVMP_LEN_NONE,
	/* preserved_97 */		CVMP_LEN_NONE,
	/* preserved_98 */		CVMP_LEN_NONE,
	/* preserved_99 */		CVMP_LEN_NONE,
	/* preserved_9a */		CVMP_LEN_NONE,
	/* preserved_9b */		CVMP_LEN_NONE,
	/* preserved_9c */		CVMP_LEN_NONE,
	/* preserved_9d */		CVMP_LEN_NONE,
	/* preserved_9e */		CVMP_LEN_NONE,
	/* preserved_9f */		CVMP_LEN_NONE,

	/* preserved_a0 */		CVMP_LEN_NONE,
	/* preserved_a1 */		CVMP_LEN_NONE,
	/* preserved_a2 */		CVMP_LEN_NONE,
	/* preserved_a3 */		CVMP_LEN_NONE,
	/* preserved_a4 */		CVMP_LEN_NONE,
	/* preserved_a5 */		CVMP_LEN_NONE,
	/* preserved_a6 */		CVMP_LEN_NONE,
	/* preserved_a7 */		CVMP_LEN_NONE,
	/* preserved_a8 */		CVMP_LEN_NONE,
	/* preserved_a9 */		CVMP_LEN_NONE,
	/* preserved_aa */		CVMP_LEN_NONE,
	/* preserved_ab */		CVMP_LEN_NONE,
	/* preserved_ac */		CVMP_LEN_NONE,
	/* preserved_ad */		CVMP_LEN_NONE,
	/* preserved_ae */		CVMP_LEN_NONE,
	/* preserved_af */		CVMP_LEN_NONE,

	/* preserved_b0 */		CVMP_LEN_NONE,
	/* preserved_b1 */		CVMP_LEN_NONE,
	/* preserved_b2 */		CVMP_LEN_NONE,
	/* preserved_b3 */		CVMP_LEN_NONE,
	/* preserved_b4 */		CVMP_LEN_NONE,
	/* preserved_b5 */		CVMP_LEN_NONE,
	/* preserved_b6 */		CVMP_LEN_NONE,
	/* preserved_b7 */		CVMP_LEN_NONE,
	/* preserved_b8 */		CVMP_LEN_NONE,
	/* preserved_b9 */		CVMP_LEN_NONE,
	/* preserved_ba */		CVMP_LEN_NONE,
	/* preserved_bb */		CVMP_LEN_NONE,
	/* preserved_bc */		CVMP_LEN_NONE,
	/* preserved_bd */		CVMP_LEN_NONE,
	/* preserved_be */		CVMP_LEN_NONE,
	/* preserved_bf */		CVMP_LEN_NONE,

	/* preserved_c0 */		CVMP_LEN_NONE,
	/* preserved_c1 */		CVMP_LEN_NONE,
	/* preserved_c2 */		CVMP_LEN_NONE,
	/* preserved_c3 */		CVMP_LEN_NONE,
	/* preserved_c4 */		CVMP_LEN_NONE,
	/* preserved_c5 */		CVMP_LEN_NONE,
	/* preserved_c6 */		CVMP_LEN_NONE,
	/* preserved_c7 */		CVMP_LEN_NONE,
	/* preserved_c8 */		CVMP_LEN_NONE,
	/* preserved_c9 */		CVMP_LEN_NONE,
	/* preserved_ca */		CVMP_LEN_NONE,
	/* preserved_cb */		CVMP_LEN_NONE,
	/* preserved_cc */		CVMP_LEN_NONE,
	/* preserved_cd */		CVMP_LEN_NONE,
	/* preserved_ce */		CVMP_LEN_NONE,
	/* preserved_cf */		CVMP_LEN_NONE,

	/* preserved_d0 */		CVMP_LEN_NONE,
	/* preserved_d1 */		CVMP_LEN_NONE,
	/* preserved_d2 */		CVMP_LEN_NONE,
	/* preserved_d3 */		CVMP_LEN_NONE,
	/* preserved_d4 */		CVMP_LEN_NONE,
	/* preserved_d5 */		CVMP_LEN_NONE,
	/* preserved_d6 */		CVMP_LEN_NONE,
	/* preserved_d7 */		CVMP_LEN_NONE,
	/* preserved_d8 */		CVMP_LEN_NONE,
	/* preserved_d9 */		CVMP_LEN_NONE,
	/* preserved_da */		CVMP_LEN_NONE,
	/* preserved_db */		CVMP_LEN_NONE,
	/* preserved_dc */		CVMP_LEN_NONE,
	/* preserved_dd */		CVMP_LEN_NONE,
	/* preserved_de */		CVMP_LEN_NONE,
	/* preserved_df */		CVMP_LEN_NONE,

	/* preserved_e0 */		CVMP_LEN_NONE,
	/* preserved_e1 */		CVMP_LEN_NONE,
	/* preserved_e2 */		CVMP_LEN_NONE,
	/* preserved_e3 */		CVMP_LEN_NONE,
	/* preserved_e4 */		CVMP_LEN_NONE,
	/* preserved_e5 */		CVMP_LEN_NONE,
	/* preserved_e6 */		CVMP_LEN_NONE,
	/* preserved_e7 */		CVMP_LEN_NONE,
	/* preserved_e8 */		CVMP_LEN_NONE,
	/* preserved_e9 */		CVMP_LEN_NONE,
	/* preserved_ea */		CVMP_LEN_NONE,
	/* preserved_eb */		CVMP_LEN_NONE,
	/* preserved_ec */		CVMP_LEN_NONE,
	/* preserved_ed */		CVMP_LEN_NONE,
	/* preserved_ee */		CVMP_LEN_NONE,
	/* preserved_ef */		CVMP_LEN_NONE,

	/* preserved_f0 */		CVMP_LEN_NONE,
	/* preserved_f1 */		CVMP_LEN_NONE,
	/* preserved_f2 */		CVMP_LEN_NONE,
	/* preserved_f3 */		CVMP_LEN_NONE,
	/* preserved_f4 */		CVMP_LEN_NONE,
	/* preserved_f5 */		CVMP_LEN_NONE,
	/* preserved_f6 */		CVMP_LEN_NONE,
	/* preserved_f7 */		CVMP_LEN_NONE,
	/* preserved_f8 */		CVMP_LEN_NONE,
	/* preserved_f9 */		CVMP_LEN_NONE,
	/* preserved_fa */		CVMP_LEN_NONE,
	/* preserved_fb */		CVMP_LEN_NONE,
	/* preserved_fc */		CVMP_LEN_NONE,
	/* preserved_fd */		CVMP_LEN_NONE,
	/* preserved_fe */		CVMP_LEN_NONE,
	/* preserved_ff */		CVMP_LEN_NONE,
};

#ifdef	__cplusplus
};
#endif
