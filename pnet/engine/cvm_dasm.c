/*
 * cvm_dasm.c - Mini-disassembler for debugging the CVM bytecode converter.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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

#ifndef IL_WITHOUT_TOOLS

#include "il_dumpasm.h"
#include "engine_private.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Operand types for CVM instructions.
 */
#define	CVM_OPER_NONE				0
#define	CVM_OPER_UINT8				1
#define	CVM_OPER_TWO_UINT8			2
#define	CVM_OPER_WIDE_UINT			3
#define	CVM_OPER_WIDE_TWO_UINT		4
#define	CVM_OPER_UINT32				5
#define	CVM_OPER_INT8				6
#define	CVM_OPER_INT32				7
#define	CVM_OPER_UINT64				8
#define	CVM_OPER_FLOAT32			9
#define	CVM_OPER_FLOAT64			10
#define	CVM_OPER_BRANCH				11
#define	CVM_OPER_BRANCH_LONG		12
#define	CVM_OPER_SWITCH				13
#define	CVM_OPER_CALL				14
#define	CVM_OPER_CALL_NATIVE		15
#define	CVM_OPER_CALL_INTERFACE		16
#define	CVM_OPER_CLASS				17
#define	CVM_OPER_UINT_AND_CLASS		18
#define	CVM_OPER_ITEM				19
#define	CVM_OPER_STRING				20
#define	CVM_OPER_WIDE				21
#define	CVM_OPER_PREFIX				22
#define	CVM_OPER_METHOD				23
#define	CVM_OPER_LD_INTERFACE		24
#define	CVM_OPER_TAIL_CALL			25
#define	CVM_OPER_PACK_VARARGS		26
#define	CVM_OPER_CUSTOM				27
#define	CVM_OPER_TWO_UINT32			28
#define	CVM_OPER_TAIL_INTERFACE		29
#define	CVM_OPER_TYPE				30
#define	CVM_OPER_PTR				31

/*
 * Table of CVM opcodes.  This must be kept in sync with "cvm.h".
 */
typedef struct
{
	const char *name;
	int			operands;

} CVMOpcode;

static CVMOpcode const opcodes[256] = {

	/*
	 * Simple opcodes.
	 */
	{"nop",				CVM_OPER_NONE},

	/*
	 * Local variable opcodes.
	 */
	{"iload_0",			CVM_OPER_NONE},
	{"iload_1",			CVM_OPER_NONE},
	{"iload_2",			CVM_OPER_NONE},
	{"iload_3",			CVM_OPER_NONE},
	{"iload",			CVM_OPER_WIDE_UINT},
	{"pload_0",			CVM_OPER_NONE},
	{"pload_1",			CVM_OPER_NONE},
	{"pload_2",			CVM_OPER_NONE},
	{"pload_3",			CVM_OPER_NONE},
	{"pload",			CVM_OPER_WIDE_UINT},
	{"istore_0",		CVM_OPER_NONE},
	{"istore_1",		CVM_OPER_NONE},
	{"istore_2",		CVM_OPER_NONE},
	{"istore_3",		CVM_OPER_NONE},
	{"istore",			CVM_OPER_WIDE_UINT},
	{"pstore_0",		CVM_OPER_NONE},
	{"pstore_1",		CVM_OPER_NONE},
	{"pstore_2",		CVM_OPER_NONE},
	{"pstore_3",		CVM_OPER_NONE},
	{"pstore",			CVM_OPER_WIDE_UINT},
	{"mload",			CVM_OPER_WIDE_TWO_UINT},
	{"mstore",			CVM_OPER_WIDE_TWO_UINT},
	{"waddr",			CVM_OPER_WIDE_UINT},
	{"maddr",			CVM_OPER_WIDE_UINT},

	/*
	 * Argument fixups.
	 */
	{"bfixup",			CVM_OPER_WIDE_UINT},
	{"sfixup",			CVM_OPER_WIDE_UINT},
	{"ffixup",			CVM_OPER_WIDE_UINT},
	{"dfixup",			CVM_OPER_WIDE_UINT},

	/*
	 * Local variable allocation.
	 */
	{"mk_local_1",		CVM_OPER_NONE},
	{"mk_local_2",		CVM_OPER_NONE},
	{"mk_local_3",		CVM_OPER_NONE},
	{"mk_local_n",		CVM_OPER_WIDE_UINT},

	/*
	 * Pointer reads and writes.
	 */
	{"bread",			CVM_OPER_NONE},
	{"ubread",			CVM_OPER_NONE},
	{"sread",			CVM_OPER_NONE},
	{"usread",			CVM_OPER_NONE},
	{"iread",			CVM_OPER_NONE},
	{"fread",			CVM_OPER_NONE},
	{"dread",			CVM_OPER_NONE},
	{"pread",			CVM_OPER_NONE},
	{"mread",			CVM_OPER_WIDE_UINT},
	{"bwrite",			CVM_OPER_NONE},
	{"swrite",			CVM_OPER_NONE},
	{"iwrite",			CVM_OPER_NONE},
	{"fwrite",			CVM_OPER_NONE},
	{"dwrite",			CVM_OPER_NONE},
	{"pwrite",			CVM_OPER_NONE},
	{"mwrite",			CVM_OPER_WIDE_UINT},
	{"bwrite_r",		CVM_OPER_NONE},
	{"swrite_r",		CVM_OPER_NONE},
	{"iwrite_r",		CVM_OPER_NONE},
	{"fwrite_r",		CVM_OPER_NONE},
	{"dwrite_r",		CVM_OPER_NONE},
	{"pwrite_r",		CVM_OPER_NONE},
	{"mwrite_r",		CVM_OPER_WIDE_UINT},

	/*
	 * Stack handling.
	 */
	{"dup",				CVM_OPER_NONE},
	{"dup2",			CVM_OPER_NONE},
	{"dup_n",			CVM_OPER_WIDE_UINT},
	{"dup_word_n",		CVM_OPER_WIDE_UINT},
	{"pop",				CVM_OPER_NONE},
	{"pop2",			CVM_OPER_NONE},
	{"pop_n",			CVM_OPER_WIDE_UINT},
	{"squash",			CVM_OPER_WIDE_TWO_UINT},
	{"ckheight",		CVM_OPER_NONE},
	{"ckheight_n",		CVM_OPER_UINT32},
	{"set_num_args",	CVM_OPER_WIDE_UINT},

	/*
	 * Arithmetic operators.
	 */
	{"iadd",			CVM_OPER_NONE},
	{"iadd_ovf",		CVM_OPER_NONE},
	{"iadd_ovf_un",		CVM_OPER_NONE},
	{"isub",			CVM_OPER_NONE},
	{"isub_ovf",		CVM_OPER_NONE},
	{"isub_ovf_un",		CVM_OPER_NONE},
	{"imul",			CVM_OPER_NONE},
	{"imul_ovf",		CVM_OPER_NONE},
	{"imul_ovf_un",		CVM_OPER_NONE},
	{"idiv",			CVM_OPER_NONE},
	{"idiv_un",			CVM_OPER_NONE},
	{"irem",			CVM_OPER_NONE},
	{"irem_un",			CVM_OPER_NONE},
	{"ineg",			CVM_OPER_NONE},
	{"ladd",			CVM_OPER_NONE},
	{"ladd_ovf",		CVM_OPER_NONE},
	{"ladd_ovf_un",		CVM_OPER_NONE},
	{"lsub",			CVM_OPER_NONE},
	{"lsub_ovf",		CVM_OPER_NONE},
	{"lsub_ovf_un",		CVM_OPER_NONE},
	{"lmul",			CVM_OPER_NONE},
	{"lmul_ovf",		CVM_OPER_NONE},
	{"lmul_ovf_un",		CVM_OPER_NONE},
	{"ldiv",			CVM_OPER_NONE},
	{"ldiv_un",			CVM_OPER_NONE},
	{"lrem",			CVM_OPER_NONE},
	{"lrem_un",			CVM_OPER_NONE},
	{"lneg",			CVM_OPER_NONE},
	{"fadd",			CVM_OPER_NONE},
	{"fsub",			CVM_OPER_NONE},
	{"fmul",			CVM_OPER_NONE},
	{"fdiv",			CVM_OPER_NONE},
	{"frem",			CVM_OPER_NONE},
	{"fneg",			CVM_OPER_NONE},

	/*
	 * Bitwise operators.
	 */
	{"iand",			CVM_OPER_NONE},
	{"ior",				CVM_OPER_NONE},
	{"ixor",			CVM_OPER_NONE},
	{"inot",			CVM_OPER_NONE},
	{"ishl",			CVM_OPER_NONE},
	{"ishr",			CVM_OPER_NONE},
	{"ishr_un",			CVM_OPER_NONE},
	{"land",			CVM_OPER_NONE},
	{"lor",				CVM_OPER_NONE},
	{"lxor",			CVM_OPER_NONE},
	{"lnot",			CVM_OPER_NONE},
	{"lshl",			CVM_OPER_NONE},
	{"lshr",			CVM_OPER_NONE},
	{"lshr_un",			CVM_OPER_NONE},

	/*
	 * Conversion operators.
	 */
	{"i2b",				CVM_OPER_NONE},
	{"i2ub",			CVM_OPER_NONE},
	{"i2s",				CVM_OPER_NONE},
	{"i2us",			CVM_OPER_NONE},
	{"i2l",				CVM_OPER_NONE},
	{"iu2l",			CVM_OPER_NONE},
	{"i2f",				CVM_OPER_NONE},
	{"iu2f",			CVM_OPER_NONE},
	{"l2i",				CVM_OPER_NONE},
	{"l2f",				CVM_OPER_NONE},
	{"lu2f",			CVM_OPER_NONE},
	{"f2i",				CVM_OPER_NONE},
	{"f2iu",			CVM_OPER_NONE},
	{"f2l",				CVM_OPER_NONE},
	{"f2lu",			CVM_OPER_NONE},
	{"f2f",				CVM_OPER_NONE},
	{"f2d",				CVM_OPER_NONE},
	{"i2p_lower",		CVM_OPER_WIDE_UINT},

	/*
	 * Pointer arithmetic and manipulation.
	 */
	{"padd_offset",		CVM_OPER_UINT8},
	{"padd_offset_n",	CVM_OPER_WIDE_TWO_UINT},
	{"padd_i4",			CVM_OPER_NONE},
	{"padd_i4_r",		CVM_OPER_NONE},
	{"padd_i8",			CVM_OPER_NONE},
	{"padd_i8_r",		CVM_OPER_NONE},
	{"psub",			CVM_OPER_NONE},
	{"psub_i4",			CVM_OPER_NONE},
	{"psub_i8",			CVM_OPER_NONE},
	{"cknull",			CVM_OPER_NONE},
	{"cknull_n",		CVM_OPER_WIDE_UINT},
	{"ldrva",			CVM_OPER_UINT32},

	/*
	 * Constant opcodes.
	 */
	{"ldnull",			CVM_OPER_NONE},
	{"ldc_i4_m1",		CVM_OPER_NONE},
	{"ldc_i4_0",		CVM_OPER_NONE},
	{"ldc_i4_1",		CVM_OPER_NONE},
	{"ldc_i4_2",		CVM_OPER_NONE},
	{"ldc_i4_3",		CVM_OPER_NONE},
	{"ldc_i4_4",		CVM_OPER_NONE},
	{"ldc_i4_5",		CVM_OPER_NONE},
	{"ldc_i4_6",		CVM_OPER_NONE},
	{"ldc_i4_7",		CVM_OPER_NONE},
	{"ldc_i4_8",		CVM_OPER_NONE},
	{"ldc_i4_s",		CVM_OPER_INT8},
	{"ldc_i4",			CVM_OPER_INT32},
	{"ldc_i8",			CVM_OPER_UINT64},
	{"ldc_r4",			CVM_OPER_FLOAT32},
	{"ldc_r8",			CVM_OPER_FLOAT64},

	/*
	 * Branch opcodes.
	 */
	{"br",				CVM_OPER_BRANCH},
	{"beq",				CVM_OPER_BRANCH},
	{"bne",				CVM_OPER_BRANCH},
	{"blt",				CVM_OPER_BRANCH},
	{"blt_un",			CVM_OPER_BRANCH},
	{"ble",				CVM_OPER_BRANCH},
	{"ble_un",			CVM_OPER_BRANCH},
	{"bgt",				CVM_OPER_BRANCH},
	{"bgt_un",			CVM_OPER_BRANCH},
	{"bge",				CVM_OPER_BRANCH},
	{"bge_un",			CVM_OPER_BRANCH},
	{"brtrue",			CVM_OPER_BRANCH},
	{"brfalse",			CVM_OPER_BRANCH},
	{"brnull",			CVM_OPER_BRANCH},
	{"brnonnull",		CVM_OPER_BRANCH},
	{"br_peq",			CVM_OPER_BRANCH},
	{"br_pne",			CVM_OPER_BRANCH},
	{"br_long",			CVM_OPER_BRANCH_LONG},
	{"switch",			CVM_OPER_SWITCH},

	/*
	 * Array opcodes.
	 */
	{"bread_elem",		CVM_OPER_NONE},
	{"ubread_elem",		CVM_OPER_NONE},
	{"sread_elem",		CVM_OPER_NONE},
	{"usread_elem",		CVM_OPER_NONE},
	{"iread_elem",		CVM_OPER_NONE},
	{"pread_elem",		CVM_OPER_NONE},
	{"bwrite_elem",		CVM_OPER_NONE},
	{"swrite_elem",		CVM_OPER_NONE},
	{"iwrite_elem",		CVM_OPER_NONE},
	{"pwrite_elem",		CVM_OPER_NONE},
	{"elem_addr_shift_i4", CVM_OPER_UINT8},
	{"elem_addr_mul_i4", CVM_OPER_UINT32},
	{"ckarray_load_i8",	CVM_OPER_NONE},
	{"ckarray_store_i8", CVM_OPER_TWO_UINT8},
	{"array_len",		CVM_OPER_NONE},

	/*
	 * Field opcodes.
	 */
	{"bread_field",		CVM_OPER_UINT8},
	{"ubread_field",	CVM_OPER_UINT8},
	{"sread_field",		CVM_OPER_UINT8},
	{"usread_field",	CVM_OPER_UINT8},
	{"iread_field",		CVM_OPER_UINT8},
	{"pread_field",		CVM_OPER_UINT8},
	{"bwrite_field",	CVM_OPER_UINT8},
	{"swrite_field",	CVM_OPER_UINT8},
	{"iwrite_field",	CVM_OPER_UINT8},
	{"pwrite_field",	CVM_OPER_UINT8},
	{"pread_this",		CVM_OPER_UINT8},
	{"iread_this",		CVM_OPER_UINT8},

	/*
	 * Call management opcodes.
	 */
	{"call",			CVM_OPER_CALL},
	{"call_ctor",		CVM_OPER_CALL},
	{"call_native",		CVM_OPER_CALL_NATIVE},
	{"call_native_void", CVM_OPER_CALL_NATIVE},
	{"call_native_raw",	CVM_OPER_CALL_NATIVE},
	{"call_native_void_raw", CVM_OPER_CALL_NATIVE},
	{"call_virtual",	CVM_OPER_WIDE_TWO_UINT},
	{"call_interface",	CVM_OPER_CALL_INTERFACE},
	{"return",			CVM_OPER_NONE},
	{"return_1",		CVM_OPER_NONE},
	{"return_2",		CVM_OPER_NONE},
	{"return_n",		CVM_OPER_UINT32},
	{"jsr",				CVM_OPER_BRANCH},
	{"ret_jsr",			CVM_OPER_NONE},
	{"push_thread",		CVM_OPER_NONE},
	{"push_thread_raw",	CVM_OPER_NONE},
	{"pushdown",		CVM_OPER_UINT32},
	{"calli",			CVM_OPER_NONE},
	{"jmpi",			CVM_OPER_NONE},

	/*
	 * Class-related opcodes.
	 */
	{"castclass",		CVM_OPER_CLASS},
	{"isinst",			CVM_OPER_CLASS},
	{"castinterface",	CVM_OPER_CLASS},
	{"isinterface",		CVM_OPER_CLASS},
	{"get_static",		CVM_OPER_CLASS},
	{"new",				CVM_OPER_NONE},
	{"new_value",		CVM_OPER_WIDE_TWO_UINT},
	{"ldstr",			CVM_OPER_STRING},
	{"ldtoken",			CVM_OPER_ITEM},
	{"box",				CVM_OPER_UINT_AND_CLASS},
	{"box_ptr",			CVM_OPER_UINT_AND_CLASS},

	/*
	 * Memory-related opcodes.
	 */
	{"memcpy",			CVM_OPER_WIDE_UINT},
	{"memmove",			CVM_OPER_NONE},
	{"memzero",			CVM_OPER_WIDE_UINT},
	{"memset",			CVM_OPER_NONE},

	/*
	 * Argument packing for native calls.
	 */
	{"waddr_native_m1",	CVM_OPER_WIDE_UINT},
	{"waddr_native_0",	CVM_OPER_WIDE_UINT},
	{"waddr_native_1",	CVM_OPER_WIDE_UINT},
	{"waddr_native_2",	CVM_OPER_WIDE_UINT},
	{"waddr_native_3",	CVM_OPER_WIDE_UINT},
	{"waddr_native_4",	CVM_OPER_WIDE_UINT},
	{"waddr_native_5",	CVM_OPER_WIDE_UINT},
	{"waddr_native_6",	CVM_OPER_WIDE_UINT},
	{"waddr_native_7",	CVM_OPER_WIDE_UINT},

	/*
	 * Quick byte loads and stores.
	 */
	{"bload",			CVM_OPER_UINT8},
	{"bstore",			CVM_OPER_UINT8},

	/*
	 * Reserved opcodes.
	 */
	{"reserved_fc",		CVM_OPER_NONE},

	/*
	 * Make the next instruction wider.
	 */
	{"wide",			CVM_OPER_WIDE},

	/*
	 * Breakpoint handling.
	 */
	{"break",			CVM_OPER_UINT8},

	/*
	 * Prefix for two-byte instruction opcodes.
	 */
	{"prefix",			CVM_OPER_PREFIX},
};
static CVMOpcode const prefixOpcodes[0xA0] = {
	/*
	 * Reserved opcodes.
	 */
	{"preserved_00",	CVM_OPER_NONE},

	/*
	 * Prefixed comparison opcodes.
	 */
	{"icmp",			CVM_OPER_NONE},
	{"icmp_un",			CVM_OPER_NONE},
	{"lcmp",			CVM_OPER_NONE},
	{"lcmp_un",			CVM_OPER_NONE},
	{"fcmpl",			CVM_OPER_NONE},
	{"fcmpg",			CVM_OPER_NONE},
	{"pcmp",			CVM_OPER_NONE},
	{"seteq",			CVM_OPER_NONE},
	{"setne",			CVM_OPER_NONE},
	{"setlt",			CVM_OPER_NONE},
	{"setle",			CVM_OPER_NONE},
	{"setgt",			CVM_OPER_NONE},
	{"setge",			CVM_OPER_NONE},

	/*
	 * Prefixed array opcodes.
	 */
	{"lread_elem",		CVM_OPER_NONE},
	{"fread_elem",		CVM_OPER_NONE},
	{"dread_elem",		CVM_OPER_NONE},
	{"lwrite_elem",		CVM_OPER_NONE},
	{"fwrite_elem",		CVM_OPER_NONE},
	{"dwrite_elem",		CVM_OPER_NONE},
	{"get2d",			CVM_OPER_NONE},
	{"set2d",			CVM_OPER_UINT32},

	/*
	 * Prefixed call management opcodes.
	 */
	{"tail_call",		CVM_OPER_TAIL_CALL},
	{"tail_calli",		CVM_OPER_NONE},
	{"tail_callvirt",	CVM_OPER_TWO_UINT32},
	{"tail_callintf",	CVM_OPER_TAIL_INTERFACE},
	{"ldftn",			CVM_OPER_METHOD},
	{"ldvirtftn",		CVM_OPER_UINT32},
	{"ldinterfftn",		CVM_OPER_LD_INTERFACE},
	{"pack_varargs",	CVM_OPER_PACK_VARARGS},

	/*
	 * Prefixed exception handling opcodes.
	 */
	{"enter_try",		CVM_OPER_NONE},
	{"throw",			CVM_OPER_NONE},
	{"throw_caller",	CVM_OPER_NONE},
	{"set_stack_trace",	CVM_OPER_NONE},

	/*
	 * Prefixed typedref handling opcodes.
	 */
	{"mkrefany",		CVM_OPER_CLASS},
	{"refanyval",		CVM_OPER_CLASS},
	{"refanytype",		CVM_OPER_NONE},

	/*
	 * Prefixed conversion opcodes.
	 */
	{"i2b_ovf",			CVM_OPER_NONE},
	{"i2ub_ovf",		CVM_OPER_NONE},
	{"iu2b_ovf",		CVM_OPER_NONE},
	{"iu2ub_ovf",		CVM_OPER_NONE},
	{"i2s_ovf",			CVM_OPER_NONE},
	{"i2us_ovf",		CVM_OPER_NONE},
	{"iu2s_ovf",		CVM_OPER_NONE},
	{"iu2us_ovf",		CVM_OPER_NONE},
	{"i2iu_ovf",		CVM_OPER_NONE},
	{"iu2i_ovf",		CVM_OPER_NONE},
	{"i2ul_ovf",		CVM_OPER_NONE},
	{"l2i_ovf",			CVM_OPER_NONE},
	{"l2ui_ovf",		CVM_OPER_NONE},
	{"lu2i_ovf",		CVM_OPER_NONE},
	{"lu2iu_ovf",		CVM_OPER_NONE},
	{"l2ul_ovf",		CVM_OPER_NONE},
	{"lu2l_ovf",		CVM_OPER_NONE},
	{"f2i_ovf",			CVM_OPER_NONE},
	{"f2iu_ovf",		CVM_OPER_NONE},
	{"f2l_ovf",			CVM_OPER_NONE},
	{"f2lu_ovf",		CVM_OPER_NONE},
	{"i2b_aligned",		CVM_OPER_NONE},
	{"i2s_aligned",		CVM_OPER_NONE},
	{"f2f_aligned",		CVM_OPER_NONE},
	{"f2d_aligned",		CVM_OPER_NONE},

	/*
	 * Prefixed arithmetic opcodes.
	 */
	{"ckfinite",		CVM_OPER_NONE},

	/*
	 * Marhsalling conversion opcodes.
	 */
	{"str2ansi",		CVM_OPER_NONE},
	{"str2utf8",		CVM_OPER_NONE},
	{"ansi2str",		CVM_OPER_NONE},
	{"utf82str",		CVM_OPER_NONE},
	{"str2utf16",		CVM_OPER_NONE},
	{"utf162str",		CVM_OPER_NONE},
	{"delegate2fnptr",	CVM_OPER_NONE},
	{"array2ptr",		CVM_OPER_NONE},
	{"refarray2ansi",	CVM_OPER_NONE},
	{"refarray2utf8",	CVM_OPER_NONE},
	{"tocustom",		CVM_OPER_CUSTOM},
	{"fromcustom",		CVM_OPER_CUSTOM},
	{"array2ansi",		CVM_OPER_NONE},
	{"array2utf8",		CVM_OPER_NONE},
	{"struct2native",	CVM_OPER_TYPE},

	/*
	 * Inline method replacements.
	 */
	{"string_concat_2",	CVM_OPER_NONE},
	{"string_concat_3",	CVM_OPER_NONE},
	{"string_concat_4",	CVM_OPER_NONE},
	{"string_eq",		CVM_OPER_NONE},
	{"string_ne",		CVM_OPER_NONE},
	{"string_get_char",	CVM_OPER_NONE},
	{"type_from_handle", CVM_OPER_NONE},
	{"monitor_enter",	CVM_OPER_NONE},
	{"monitor_exit",	CVM_OPER_NONE},
	{"append_char",		CVM_OPER_METHOD},
	{"is_white_space",	CVM_OPER_NONE},

	/*
	 * Binary value fixups.
	 */
	{"fix_i4_i",		CVM_OPER_NONE},
	{"fix_i4_u",		CVM_OPER_NONE},

	/*
	 * Trigger method unrolling.
	 */
	{"unroll_method",	CVM_OPER_NONE},

	/*
	 * Allocate local stack space.
	 */
	{"local_alloc",		CVM_OPER_NONE},

	/*
	 * Method profiling.
	 */
	{"profile_count",	CVM_OPER_NONE},

	/*
	 * Thread static handling.
	 */
	{"thread_static",	CVM_OPER_TWO_UINT32},

	/*
	 * Argument packing for native calls.
	 */
	{"waddr_native_n",	CVM_OPER_TWO_UINT32},
	
	/*
	 * Method trace opcodes
	 */
	{"trace_in",	CVM_OPER_UINT32},
	{"trace_out",	CVM_OPER_UINT32},

	/*
	 * More prefixed exception handling opcodes.
	 */
	{"start_catch",		CVM_OPER_PTR},
	{"start_finally",	CVM_OPER_PTR},
	{"propagate_abort",	CVM_OPER_NONE},

	/*
	 * Reserved opcodes.
	 */
	{"preserved_65",	CVM_OPER_NONE},
	{"preserved_66",	CVM_OPER_NONE},
	{"preserved_67",	CVM_OPER_NONE},
	{"preserved_68",	CVM_OPER_NONE},
	{"preserved_69",	CVM_OPER_NONE},
	{"preserved_6A",	CVM_OPER_NONE},
	{"preserved_6B",	CVM_OPER_NONE},
	{"preserved_6C",	CVM_OPER_NONE},

	/*
	 * More inline method replacements.
	 */
	{"abs_i4",			CVM_OPER_NONE},
	{"abs_r4",			CVM_OPER_NONE},
	{"abs_r8",			CVM_OPER_NONE},
	{"asin",			CVM_OPER_NONE},
	{"atan",			CVM_OPER_NONE},
	{"atan2",			CVM_OPER_NONE},
	{"ceiling",			CVM_OPER_NONE},
	{"cos",				CVM_OPER_NONE},
	{"cosh",			CVM_OPER_NONE},
	{"exp",				CVM_OPER_NONE},
	{"floor",			CVM_OPER_NONE},
	{"ieeeremainder",	CVM_OPER_NONE},
	{"log",				CVM_OPER_NONE},
	{"log10",			CVM_OPER_NONE},
	{"min_i4",			CVM_OPER_NONE},
	{"max_i4",			CVM_OPER_NONE},
	{"min_r4",			CVM_OPER_NONE},
	{"max_r4",			CVM_OPER_NONE},
	{"min_r8",			CVM_OPER_NONE},
	{"max_r8",			CVM_OPER_NONE},
	{"pow",				CVM_OPER_NONE},
	{"round",			CVM_OPER_NONE},
	{"sign_i4",			CVM_OPER_NONE},
	{"sign_r4",			CVM_OPER_NONE},
	{"sign_r8",			CVM_OPER_NONE},
	{"sin",				CVM_OPER_NONE},
	{"sinh",			CVM_OPER_NONE},
	{"sqrt",			CVM_OPER_NONE},
	{"tan",				CVM_OPER_NONE},
	{"tanh",			CVM_OPER_NONE},

	/*
	 * Unroller support opcodes.
	 */
	{"unroll_stack",	CVM_OPER_NONE},
	{"unroll_stack_return", CVM_OPER_NONE},

	/*
	 * Generics support opcodes.
	 */
	{"repl_word_n",		CVM_OPER_UINT32},
	{"call_virtgen",	CVM_OPER_TWO_UINT32},

	/*
	 * Inlined array functions.
	 */
	{"sarray_copy_aai4", CVM_OPER_UINT32},
	{"sarray_copy_ai4ai4i4", CVM_OPER_UINT32},
	{"sarray_clear_ai4i4", CVM_OPER_UINT32},

	/*
	 * Enghanced method profiling opcodes.
	 */
	{"profile_start",	CVM_OPER_NONE},
	{"profile_end",		CVM_OPER_NONE},

	/*
	 * Reserved opcodes.
	 */
	{"preserved_94",	CVM_OPER_NONE},
	{"preserved_95",	CVM_OPER_NONE},
	{"preserved_96",	CVM_OPER_NONE},
	{"preserved_97",	CVM_OPER_NONE},
	{"preserved_98",	CVM_OPER_NONE},
	{"preserved_99",	CVM_OPER_NONE},
	{"preserved_9A",	CVM_OPER_NONE},
	{"preserved_9B",	CVM_OPER_NONE},
	{"preserved_9C",	CVM_OPER_NONE},
	{"preserved_9D",	CVM_OPER_NONE},
	{"preserved_9E",	CVM_OPER_NONE},
	{"preserved_9F",	CVM_OPER_NONE}
};

/*
 * Read a pointer from an instruction stream.
 */
static void *CVMReadPointer(unsigned char *pc)
{
#if defined(__i386) || defined(__i386__)
	/* The x86 can read values from non-aligned addresses */
	return *((void **)pc);
#else
	/* We need to be careful about alignment on other platforms */
	if(sizeof(void *) == 4)
	{
		return (void *)(ILNativeUInt)(IL_READ_UINT32(pc));
	}
	else
	{
		return (void *)(ILNativeUInt)(IL_READ_UINT64(pc));
	}
#endif
}

/* internal.c */
const ILMethodTableEntry *_ILFindInternalByAddr(void *addr,
												const char **className);

int _ILDumpCVMInsn(FILE *stream, ILMethod *currMethod, unsigned char *pc)
{
	const CVMOpcode *opcode = &(opcodes[pc[0]]);
	int size = 0;
	unsigned char *dest;
	ILMethod *method;
	ILClass *classInfo;
	ILProgramItem *item;
	ILField *field;
	ILToken token;
	const char *str;
	ILUInt32 strLen;
	unsigned long numCases;
	const ILMethodTableEntry *methodEntry;

	/* Dump the address of the instruction */
	fprintf(stream, "0x%08lX:  ", (unsigned long)pc);

	/* Dump the name of the instruction if it is not a prefix */
	if(opcode->operands != CVM_OPER_BRANCH_LONG &&
	   opcode->operands != CVM_OPER_WIDE &&
	   opcode->operands != CVM_OPER_PREFIX)
	{
		fprintf(stream, "%-20s", opcode->name);
	}

	/* Determine how to dump the instruction operands */
	switch(opcode->operands)
	{
		case CVM_OPER_NONE:
		{
			size = 1;
		}
		break;

		case CVM_OPER_UINT8:
		case CVM_OPER_WIDE_UINT:
		{
			fprintf(stream, "%d", (int)(pc[1]));
			size = 2;
		}
		break;

		case CVM_OPER_TWO_UINT8:
		case CVM_OPER_WIDE_TWO_UINT:
		{
			fprintf(stream, "%d, %d", (int)(pc[1]), (int)(pc[2]));
			size = 3;
		}
		break;

		case CVM_OPER_UINT32:
		{
			fprintf(stream, "%lu", (unsigned long)(IL_READ_UINT32(pc + 1)));
			size = 5;
		}
		break;

		case CVM_OPER_INT8:
		{
			fprintf(stream, "%d", (int)(ILInt8)(pc[1]));
			size = 2;
		}
		break;

		case CVM_OPER_INT32:
		{
			fprintf(stream, "%ld", (long)(IL_READ_INT32(pc + 1)));
			size = 5;
		}
		break;

		case CVM_OPER_UINT64:
		{
			fprintf(stream, "0x%08lX%08lX",
					(unsigned long)(IL_READ_INT32(pc + 5)),
					(unsigned long)(IL_READ_INT32(pc + 1)));
			size = 9;
		}
		break;

		case CVM_OPER_FLOAT32:
		{
#ifdef IL_CONFIG_FP_SUPPORTED
			fprintf(stream, "%g", (double)(IL_READ_FLOAT(pc + 1)));
#else /* !IL_CONFIG_FP_SUPPORTED */
			fprintf(stream, "FLOAT32 value, unsupported");
#endif /* !IL_CONFIG_FP_SUPPORTED */
			size = 5;
		}
		break;

		case CVM_OPER_FLOAT64:
		{
#ifdef IL_CONFIG_FP_SUPPORTED
			fprintf(stream, "%g", (double)(IL_READ_DOUBLE(pc + 1)));
#else /* !IL_CONFIG_FP_SUPPORTED */
			fprintf(stream, "FLOAT64 value, unsupported");
#endif /* !IL_CONFIG_FP_SUPPORTED */
			size = 9;
		}
		break;

		case CVM_OPER_BRANCH:
		{
			dest = pc + (int)(ILInt8)(pc[1]);
			fprintf(stream, "0x%08lX", (unsigned long)dest);
			size = 6;
		}
		break;

		case CVM_OPER_BRANCH_LONG:
		{
			dest = pc + IL_READ_INT32(pc + 2);
			fprintf(stream, "%-20s0x%08lX",
					opcodes[pc[1]].name, (unsigned long)dest);
			size = 6;
		}
		break;

		case CVM_OPER_SWITCH:
		{
			numCases = IL_READ_UINT32(pc + 1);
			dest = pc + IL_READ_INT32(pc + 5);
			fprintf(stream, "%lu, 0x%08lX", numCases, (unsigned long)dest);
			size = 9;
			while(numCases > 0)
			{
				dest = pc + IL_READ_INT32(pc + size);
				fprintf(stream, "\n             %-20s0x%08lX",
						"swlabel", (unsigned long)dest);
				size += 4;
				--numCases;
			}
		}
		break;

		case CVM_OPER_CALL:
		{
			method = (ILMethod *)CVMReadPointer(pc + 1);
			ILDumpMethodType(stream, ILProgramItem_Image(currMethod),
							 ILMethod_Signature(method), 0,
							 ILMethod_Owner(method),
							 ILMethod_Name(method), method);
			size = 1 + sizeof(void *);
		}
		break;

		case CVM_OPER_CALL_NATIVE:
		{
			fprintf(stream, "0x%08lX (",
					(unsigned long)(CVMReadPointer(pc + 1)));
			methodEntry = _ILFindInternalByAddr(CVMReadPointer(pc + 1),
												&str);
			if(methodEntry)
			{
				fprintf(stream, "%s.%s \"%s\"", str, methodEntry->methodName,
						(methodEntry->signature ?
							methodEntry->signature :
							methodEntry[-1].signature));
			}
			else
			{
				putc('?', stream);
			}
			putc(')', stream);
			size = sizeof(void *) * 2 + 1;
		}
		break;

		case CVM_OPER_CALL_INTERFACE:
		{
		#ifdef IL_USE_IMTS
			method = (ILMethod *)CVMReadPointer(pc + 3);
			ILDumpClassName(stream, ILProgramItem_Image(currMethod),
							ILMethod_Owner(method), 0);
			fprintf(stream, ", %d, %d", (int)(pc[1]), method->index);
		#else
			classInfo = (ILClass *)CVMReadPointer(pc + 3);
			ILDumpClassName(stream, ILProgramItem_Image(currMethod),
							classInfo, 0);
			fprintf(stream, ", %d, %d", (int)(pc[1]), (int)(pc[2]));
		#endif
			size = 3 + sizeof(void *);
		}
		break;

		case CVM_OPER_CLASS:
		{
			classInfo = (ILClass *)CVMReadPointer(pc + 1);
			ILDumpClassName(stream, ILProgramItem_Image(currMethod),
							classInfo, 0);
			size = 1 + sizeof(void *);
		}
		break;

		case CVM_OPER_UINT_AND_CLASS:
		{
			classInfo = (ILClass *)CVMReadPointer(pc + 2);
			ILDumpClassName(stream, ILProgramItem_Image(currMethod),
							classInfo, 0);
			fprintf(stream, ", %d", (int)(pc[1]));
			size = 2 + sizeof(void *);
		}
		break;

		case CVM_OPER_ITEM:
		{
			item = (ILProgramItem *)CVMReadPointer(pc + 1);
			if((classInfo = ILProgramItemToClass(item)) != 0)
			{
				ILDumpClassName(stream, ILProgramItem_Image(currMethod),
								classInfo, 0);
			}
			else if((method = ILProgramItemToMethod(item)) != 0)
			{
				ILDumpMethodType(stream, ILProgramItem_Image(currMethod),
								 ILMethod_Signature(method), 0,
								 ILMethod_Owner(method),
								 ILMethod_Name(method), method);
			}
			else if((field = ILProgramItemToField(item)) != 0)
			{
				ILDumpType(stream, ILProgramItem_Image(currMethod),
						   ILField_Type(field), 0);
				putc(' ', stream);
				ILDumpClassName(stream, ILProgramItem_Image(currMethod),
								ILField_Owner(field), 0);
				fputs("::", stream);
				ILDumpIdentifier(stream, ILField_Name(field), 0, 0);
			}
			size = 1 + sizeof(void *);
		}
		break;

		case CVM_OPER_STRING:
		{
			token = IL_READ_UINT32(pc + 1) & ~IL_META_TOKEN_MASK;
			str = ILImageGetUserString(ILProgramItem_Image(currMethod),
									   token, &strLen);
			if(str)
			{
				ILDumpUnicodeString(stream, str, strLen);
			}
			size = 5;
		}
		break;

		case CVM_OPER_PTR:
		{
			fprintf(stream, "0x%lx", (unsigned long)CVMReadPointer(pc + 1));
			size = 1 + sizeof(void *);
		}
		break;

		case CVM_OPER_WIDE:
		{
			opcode = &(opcodes[pc[1]]);
			fprintf(stream, "%-20s", opcode->name);
			switch(opcode->operands)
			{
				case CVM_OPER_WIDE_UINT:
				{
					fprintf(stream, "%lu",
							(unsigned long)IL_READ_UINT32(pc + 2));
					size = 6;
				}
				break;

				case CVM_OPER_WIDE_TWO_UINT:
				{
					fprintf(stream, "%lu, %lu",
							(unsigned long)IL_READ_UINT32(pc + 2),
							(unsigned long)IL_READ_UINT32(pc + 6));
					size = 10;
				}
				break;

				case CVM_OPER_CALL_INTERFACE:
				{
				#ifdef IL_USE_IMTS
					method = (ILMethod *)CVMReadPointer(pc + 10);
					ILDumpClassName(stream, ILProgramItem_Image(currMethod),
									ILMethod_Owner(method), 0);
					fprintf(stream, ", %lu, %lu",
							(unsigned long)IL_READ_UINT32(pc + 2),
							(unsigned long)(method->index));
				#else
					classInfo = (ILClass *)CVMReadPointer(pc + 10);
					ILDumpClassName(stream, ILProgramItem_Image(currMethod),
									classInfo, 0);
					fprintf(stream, ", %lu, %lu",
							(unsigned long)IL_READ_UINT32(pc + 2),
							(unsigned long)IL_READ_UINT32(pc + 6));
				#endif
					size = 10 + sizeof(void *);
				}
				break;

				case CVM_OPER_UINT_AND_CLASS:
				{
					classInfo = (ILClass *)CVMReadPointer(pc + 6);
					ILDumpClassName(stream, ILProgramItem_Image(currMethod),
									classInfo, 0);
					fprintf(stream, ", %lu",
							(unsigned long)(IL_READ_UINT32(pc + 2)));
					size = 6 + sizeof(void *);
				}
				break;

				default:
				{
					size = 2;
				}
				break;
			}
		}
		break;

		case CVM_OPER_PREFIX:
		{
			opcode = &(prefixOpcodes[pc[1]]);
			fprintf(stream, "%-20s", opcode->name);
			switch(opcode->operands)
			{
				case CVM_OPER_NONE:
				{
					size = 2;
				}
				break;

				case CVM_OPER_CLASS:
				{
					classInfo = (ILClass *)CVMReadPointer(pc + 2);
					ILDumpClassName(stream, ILProgramItem_Image(currMethod),
									classInfo, 0);
					size = 2 + sizeof(void *);
				}
				break;

				case CVM_OPER_UINT32:
				{
					fprintf(stream, "%lu",
							(unsigned long)(IL_READ_UINT32(pc + 2)));
					size = 6;
				}
				break;

				case CVM_OPER_METHOD:
				{
					method = (ILMethod *)CVMReadPointer(pc + 2);
					ILDumpMethodType(stream, ILProgramItem_Image(currMethod),
									 ILMethod_Signature(method), 0,
									 ILMethod_Owner(method),
									 ILMethod_Name(method), method);
					size = 2 + sizeof(void *);
				}
				break;

				case CVM_OPER_LD_INTERFACE:
				{
					classInfo = (ILClass *)CVMReadPointer(pc + 6);
					ILDumpClassName(stream, ILProgramItem_Image(currMethod),
									classInfo, 0);
					fprintf(stream, ", %lu",
							(unsigned long)(IL_READ_UINT32(pc + 2)));
					size = 6 + sizeof(void *);
				}
				break;

				case CVM_OPER_TAIL_CALL:
				{
					method = (ILMethod *)CVMReadPointer(pc + 2);
					ILDumpMethodType(stream, ILProgramItem_Image(currMethod),
									 ILMethod_Signature(method), 0,
									 ILMethod_Owner(method),
									 ILMethod_Name(method), method);
					size = 2 + sizeof(void *);
				}
				break;

				case CVM_OPER_PACK_VARARGS:
				{
					fprintf(stream, "%lu, %lu, ",
							(unsigned long)(IL_READ_UINT32(pc + 2)),
							(unsigned long)(IL_READ_UINT32(pc + 6)));
					ILDumpType(stream, ILProgramItem_Image(currMethod),
							   (ILType *)CVMReadPointer(pc + 10), 0);
					size = 10 + sizeof(void *);
				}
				break;

				case CVM_OPER_CUSTOM:
				{
					putc('"', stream);
					fwrite((void *)CVMReadPointer(pc + 6), 1,
						   (unsigned)(IL_READ_UINT32(pc + 2)), stream);
					putc('"', stream);
				}
				break;

				case CVM_OPER_TWO_UINT32:
				{
					fprintf(stream, "%lu, %lu",
							(unsigned long)(IL_READ_UINT32(pc + 2)),
							(unsigned long)(IL_READ_UINT32(pc + 6)));
					pc += 10;
				}
				break;

				case CVM_OPER_TAIL_INTERFACE:
				{
				#ifdef IL_USE_IMTS
					method = (ILMethod *)CVMReadPointer(pc + 10);
					fprintf(stream, "%lu, %lu, ",
							(unsigned long)(IL_READ_UINT32(pc + 2)),
							(unsigned long)(method->index));
					ILDumpClassName(stream, ILProgramItem_Image(currMethod),
									ILMethod_Owner(method), 0);
				#else
					classInfo = (ILClass *)CVMReadPointer(pc + 10);
					fprintf(stream, "%lu, %lu, ",
							(unsigned long)(IL_READ_UINT32(pc + 2)),
							(unsigned long)(IL_READ_UINT32(pc + 6)));
					ILDumpClassName(stream, ILProgramItem_Image(currMethod),
									classInfo, 0);
				#endif
					size = 10 + sizeof(void *);
				}
				break;

				case CVM_OPER_PTR:
				{
					fprintf(stream, "0x%lx",
							(unsigned long)CVMReadPointer(pc + 2));
					size = 2 + sizeof(void *);
				}
				break;

				default:
				{
					size = 2;
				}
				break;
			}
		}
		break;

		default:
		{
			size = 1;
		}
		break;
	}

	/* Terminate the line and return the size */
	putc('\n', stream);
	fflush(stream);
	return size;
}

/*
 * Instruction profiling array.
 */
int _ILCVMInsnCount[512];

/*
 * Dump the instruction profile array.
 */
int _ILDumpInsnProfile(FILE *stream)
{
	int insn, insn2, temp;
	int sawCounts = 0;
	int indices[512];

	/* Zero "wide" and "prefix", because they aren't important */
	_ILCVMInsnCount[COP_WIDE] = 0;
	_ILCVMInsnCount[COP_PREFIX] = 0;

	/* Sort the instruction count table into decreasing order */
	for(insn = 0; insn < 512; ++insn)
	{
		indices[insn] = insn;
	}
	for(insn = 0; insn < 511; ++insn)
	{
		for(insn2 = insn + 1; insn2 < 512; ++insn2)
		{
			if(_ILCVMInsnCount[indices[insn]] <
					_ILCVMInsnCount[indices[insn2]])
			{
				temp = indices[insn];
				indices[insn] = indices[insn2];
				indices[insn2] = temp;
			}
		}
	}

	/* Dump the contents of the sorted instruction table */
	for(insn = 0; insn < 512; ++insn)
	{
		insn2 = indices[insn];
		if(_ILCVMInsnCount[insn2])
		{
			if(insn2 < 256)
			{
				fprintf(stream, "%-20s  %d\n", opcodes[insn2].name,
						_ILCVMInsnCount[insn2]);
			}
			else
			{
				fprintf(stream, "%-20s  %d\n",
						prefixOpcodes[insn2 - 256].name,
						_ILCVMInsnCount[insn2]);
			}
			sawCounts = 1;
		}
	}

	/* Indicate to the caller whether we have count information or not */
	return sawCounts;
}

/*
 * Variable profiling arrays.
 */
int _ILCVMVarLoadCounts[256];
int _ILCVMVarLoadDepths[256];
int _ILCVMVarStoreCounts[256];
int _ILCVMVarStoreDepths[256];

/*
 * Dump an array in decreasing order of value.
 */
static int DumpVarArray(FILE *stream, const char *name, int *array)
{
	int indices[256];
	int index, index2, temp;

	/* Output the array name */
	fprintf(stream, "\n%s:\n\n", name);

	/* Initialize the "indices" array */
	index2 = -1;
	for(index = 0; index < 256; ++index)
	{
		if(array[index] != 0)
		{
			index2 = index;
		}
		indices[index] = index;
	}
	if(index2 == -1)
	{
		return 0;
	}

	/* Sort the incoming array */
	for(index = 0; index < 255; ++index)
	{
		for(index2 = index + 1; index2 < 256; ++index2)
		{
			if(array[indices[index]] < array[indices[index2]])
			{
				temp = indices[index];
				indices[index] = indices[index2];
				indices[index2] = temp;
			}
		}
	}

	/* Dump the contents of the array */
	for(index = 0; index < 256; ++index)
	{
		if(array[indices[index]] != 0)
		{
			fprintf(stream, "%d\t%d\n", indices[index], array[indices[index]]);
		}
	}
	return 1;
}

/*
 * Dump the variable profile arrays.
 */
int _ILDumpVarProfile(FILE *stream)
{
	return DumpVarArray(stream, "variable load indices", _ILCVMVarLoadCounts)
	    && DumpVarArray(stream, "variable load depths", _ILCVMVarLoadDepths)
		&& DumpVarArray(stream, "variable store indices", _ILCVMVarStoreCounts)
		&& DumpVarArray(stream, "variable store depths", _ILCVMVarStoreDepths);
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_WITHOUT_TOOLS */

