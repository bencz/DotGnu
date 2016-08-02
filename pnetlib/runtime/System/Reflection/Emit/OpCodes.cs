/*
 * OpCodes.cs - Implementation of the "System.Reflection.Emit.OpCodes" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

using System;

public class OpCodes
{
	// Cannot instantiate this class.
	private OpCodes() {}

	public static readonly OpCode Nop =
		 new OpCode("nop", 0x00, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Break =
		 new OpCode("break", 0x01, FlowControl.Break,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Ldarg_0 =
		 new OpCode("ldarg.0", 0x02, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldarg_1 =
		 new OpCode("ldarg.1", 0x03, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldarg_2 =
		 new OpCode("ldarg.2", 0x04, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldarg_3 =
		 new OpCode("ldarg.3", 0x05, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldloc_0 =
		 new OpCode("ldloc.0", 0x06, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldloc_1 =
		 new OpCode("ldloc.1", 0x07, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldloc_2 =
		 new OpCode("ldloc.2", 0x08, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldloc_3 =
		 new OpCode("ldloc.3", 0x09, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Stloc_0 =
		 new OpCode("stloc.0", 0x0A, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Stloc_1 =
		 new OpCode("stloc.1", 0x0B, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Stloc_2 =
		 new OpCode("stloc.2", 0x0C, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Stloc_3 =
		 new OpCode("stloc.3", 0x0D, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Ldarg_S =
		 new OpCode("ldarg.s", 0x0E, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.ShortInlineVar,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldarga_S =
		 new OpCode("ldarga.s", 0x0F, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.ShortInlineVar,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Starg_S =
		 new OpCode("starg.s", 0x10, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.ShortInlineVar,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Ldloc_S =
		 new OpCode("ldloc.s", 0x11, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.ShortInlineVar,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldloca_S =
		 new OpCode("ldloca.s", 0x12, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.ShortInlineVar,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Stloc_S =
		 new OpCode("stloc.s", 0x13, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.ShortInlineVar,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Ldnull =
		 new OpCode("ldnull", 0x14, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushref);

	public static readonly OpCode Ldc_I4_M1 =
		 new OpCode("ldc.i4.m1", 0x15, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_0 =
		 new OpCode("ldc.i4.0", 0x16, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_1 =
		 new OpCode("ldc.i4.1", 0x17, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_2 =
		 new OpCode("ldc.i4.2", 0x18, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_3 =
		 new OpCode("ldc.i4.3", 0x19, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_4 =
		 new OpCode("ldc.i4.4", 0x1A, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_5 =
		 new OpCode("ldc.i4.5", 0x1B, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_6 =
		 new OpCode("ldc.i4.6", 0x1C, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_7 =
		 new OpCode("ldc.i4.7", 0x1D, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_8 =
		 new OpCode("ldc.i4.8", 0x1E, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4_S =
		 new OpCode("ldc.i4.s", 0x1F, FlowControl.Next,
		 			OpCodeType.Macro, OperandType.ShortInlineI,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I4 =
		 new OpCode("ldc.i4", 0x20, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineI,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ldc_I8 =
		 new OpCode("ldc.i8", 0x21, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineI8,
					StackBehaviour.Pop0, StackBehaviour.Pushi8);

	public static readonly OpCode Ldc_R4 =
		 new OpCode("ldc.r4", 0x22, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.ShortInlineR,
					StackBehaviour.Pop0, StackBehaviour.Pushr4);

	public static readonly OpCode Ldc_R8 =
		 new OpCode("ldc.r8", 0x23, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineR,
					StackBehaviour.Pop0, StackBehaviour.Pushr8);

	public static readonly OpCode Dup =
		 new OpCode("dup", 0x25, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Push1_push1);

	public static readonly OpCode Pop =
		 new OpCode("pop", 0x26, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Jmp =
		 new OpCode("jmp", 0x27, FlowControl.Call,
		 			OpCodeType.Primitive, OperandType.InlineMethod,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Call =
		 new OpCode("call", 0x28, FlowControl.Call,
		 			OpCodeType.Primitive, OperandType.InlineMethod,
					StackBehaviour.Varpop, StackBehaviour.Varpush);

	public static readonly OpCode Calli =
		 new OpCode("calli", 0x29, FlowControl.Call,
		 			OpCodeType.Primitive, OperandType.InlineSig,
					StackBehaviour.Varpop, StackBehaviour.Varpush);

	public static readonly OpCode Ret =
		 new OpCode("ret", 0x2A, FlowControl.Return,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Varpop, StackBehaviour.Push0);

	public static readonly OpCode Br_S =
		 new OpCode("br.s", 0x2B, FlowControl.Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Brfalse_S =
		 new OpCode("brfalse.s", 0x2C, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Popi, StackBehaviour.Push0);

	public static readonly OpCode Brtrue_S =
		 new OpCode("brtrue.s", 0x2D, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Popi, StackBehaviour.Push0);

	public static readonly OpCode Beq_S =
		 new OpCode("beq.s", 0x2E, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bge_S =
		 new OpCode("bge.s", 0x2F, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bgt_S =
		 new OpCode("bgt.s", 0x30, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Ble_S =
		 new OpCode("ble.s", 0x31, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Blt_S =
		 new OpCode("blt.s", 0x32, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bne_Un_S =
		 new OpCode("bne.un.s", 0x33, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bge_Un_S =
		 new OpCode("bge.un.s", 0x34, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bgt_Un_S =
		 new OpCode("bgt.un.s", 0x35, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Ble_Un_S =
		 new OpCode("ble.un.s", 0x36, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Blt_Un_S =
		 new OpCode("blt.un.s", 0x37, FlowControl.Cond_Branch,
		 			OpCodeType.Macro, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Br =
		 new OpCode("br", 0x38, FlowControl.Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Brfalse =
		 new OpCode("brfalse", 0x39, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Popi, StackBehaviour.Push0);

	public static readonly OpCode Brtrue =
		 new OpCode("brtrue", 0x3A, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Popi, StackBehaviour.Push0);

	public static readonly OpCode Beq =
		 new OpCode("beq", 0x3B, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bge =
		 new OpCode("bge", 0x3C, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bgt =
		 new OpCode("bgt", 0x3D, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Ble =
		 new OpCode("ble", 0x3E, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Blt =
		 new OpCode("blt", 0x3F, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bne_Un =
		 new OpCode("bne.un", 0x40, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bge_Un =
		 new OpCode("bge.un", 0x41, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Bgt_Un =
		 new OpCode("bgt.un", 0x42, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Ble_Un =
		 new OpCode("ble.un", 0x43, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Blt_Un =
		 new OpCode("blt.un", 0x44, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push0);

	public static readonly OpCode Switch =
		 new OpCode("switch", 0x45, FlowControl.Cond_Branch,
		 			OpCodeType.Primitive, OperandType.InlineSwitch,
					StackBehaviour.Popi, StackBehaviour.Push0);

	public static readonly OpCode Ldind_I1 =
		 new OpCode("ldind.i1", 0x46, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldind_U1 =
		 new OpCode("ldind.u1", 0x47, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldind_I2 =
		 new OpCode("ldind.i2", 0x48, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldind_U2 =
		 new OpCode("ldind.u2", 0x49, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldind_I4 =
		 new OpCode("ldind.i4", 0x4A, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldind_U4 =
		 new OpCode("ldind.u4", 0x4B, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldind_I8 =
		 new OpCode("ldind.i8", 0x4C, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushi8);

	public static readonly OpCode Ldind_I =
		 new OpCode("ldind.i", 0x4D, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldind_R4 =
		 new OpCode("ldind.r4", 0x4E, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushr4);

	public static readonly OpCode Ldind_R8 =
		 new OpCode("ldind.r8", 0x4F, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushr8);

	public static readonly OpCode Ldind_Ref =
		 new OpCode("ldind.ref", 0x50, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushref);

	public static readonly OpCode Stind_Ref =
		 new OpCode("stind.ref", 0x51, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Stind_I1 =
		 new OpCode("stind.i1", 0x52, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Stind_I2 =
		 new OpCode("stind.i2", 0x53, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Stind_I4 =
		 new OpCode("stind.i4", 0x54, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Stind_I8 =
		 new OpCode("stind.i8", 0x55, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi8, StackBehaviour.Push0);

	public static readonly OpCode Stind_R4 =
		 new OpCode("stind.r4", 0x56, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popr4, StackBehaviour.Push0);

	public static readonly OpCode Stind_R8 =
		 new OpCode("stind.r8", 0x57, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popr8, StackBehaviour.Push0);

	public static readonly OpCode Add =
		 new OpCode("add", 0x58, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Sub =
		 new OpCode("sub", 0x59, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Mul =
		 new OpCode("mul", 0x5A, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Div =
		 new OpCode("div", 0x5B, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Div_Un =
		 new OpCode("div.un", 0x5C, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Rem =
		 new OpCode("rem", 0x5D, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Rem_Un =
		 new OpCode("rem.un", 0x5E, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode And =
		 new OpCode("and", 0x5F, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Or =
		 new OpCode("or", 0x60, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Xor =
		 new OpCode("xor", 0x61, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Shl =
		 new OpCode("shl", 0x62, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Shr =
		 new OpCode("shr", 0x63, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Shr_Un =
		 new OpCode("shr.un", 0x64, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Neg =
		 new OpCode("neg", 0x65, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Push1);

	public static readonly OpCode Not =
		 new OpCode("not", 0x66, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Push1);

	public static readonly OpCode Conv_I1 =
		 new OpCode("conv.i1", 0x67, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_I2 =
		 new OpCode("conv.i2", 0x68, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_I4 =
		 new OpCode("conv.i4", 0x69, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_I8 =
		 new OpCode("conv.i8", 0x6A, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi8);

	public static readonly OpCode Conv_R4 =
		 new OpCode("conv.r4", 0x6B, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushr4);

	public static readonly OpCode Conv_R8 =
		 new OpCode("conv.r8", 0x6C, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushr8);

	public static readonly OpCode Conv_U4 =
		 new OpCode("conv.u4", 0x6D, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_U8 =
		 new OpCode("conv.u8", 0x6E, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi8);

	public static readonly OpCode Callvirt =
		 new OpCode("callvirt", 0x6F, FlowControl.Call,
		 			OpCodeType.Objmodel, OperandType.InlineMethod,
					StackBehaviour.Varpop, StackBehaviour.Varpush);

	public static readonly OpCode Cpobj =
		 new OpCode("cpobj", 0x70, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Ldobj =
		 new OpCode("ldobj", 0x71, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popi, StackBehaviour.Push1);

	public static readonly OpCode Ldstr =
		 new OpCode("ldstr", 0x72, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineString,
					StackBehaviour.Pop0, StackBehaviour.Pushref);

	public static readonly OpCode Newobj =
		 new OpCode("newobj", 0x73, FlowControl.Call,
		 			OpCodeType.Objmodel, OperandType.InlineMethod,
					StackBehaviour.Varpop, StackBehaviour.Pushref);

	public static readonly OpCode Castclass =
		 new OpCode("castclass", 0x74, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popref, StackBehaviour.Pushref);

	public static readonly OpCode Isinst =
		 new OpCode("isinst", 0x75, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popref, StackBehaviour.Pushref);

	public static readonly OpCode Conv_R_Un =
		 new OpCode("conv.r.un", 0x76, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushr8);

	public static readonly OpCode Unbox =
		 new OpCode("unbox", 0x79, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popref, StackBehaviour.Push1);

	public static readonly OpCode Throw =
		 new OpCode("throw", 0x7A, FlowControl.Throw,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref, StackBehaviour.Push1);

	public static readonly OpCode Ldfld =
		 new OpCode("ldfld", 0x7B, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineField,
					StackBehaviour.Popref, StackBehaviour.Push1);

	public static readonly OpCode Ldflda =
		 new OpCode("ldflda", 0x7C, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineField,
					StackBehaviour.Popref, StackBehaviour.Pushi);

	public static readonly OpCode Stfld =
		 new OpCode("stfld", 0x7D, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineField,
					StackBehaviour.Popref_pop1, StackBehaviour.Push0);

	public static readonly OpCode Ldsfld =
		 new OpCode("ldsfld", 0x7E, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineField,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldsflda =
		 new OpCode("ldsflda", 0x7F, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineField,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Stsfld =
		 new OpCode("stsfld", 0x80, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineField,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Stobj =
		 new OpCode("stobj", 0x81, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popi_pop1, StackBehaviour.Push0);

	public static readonly OpCode Conv_Ovf_I1_Un =
		 new OpCode("conv.ovf.i1.un", 0x82, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_I2_Un =
		 new OpCode("conv.ovf.i2.un", 0x83, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_I4_Un =
		 new OpCode("conv.ovf.i4.un", 0x84, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_I8_Un =
		 new OpCode("conv.ovf.i8.un", 0x85, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi8);

	public static readonly OpCode Conv_Ovf_U1_Un =
		 new OpCode("conv.ovf.u1.un", 0x86, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_U2_Un =
		 new OpCode("conv.ovf.u2.un", 0x87, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_U4_Un =
		 new OpCode("conv.ovf.u4.un", 0x88, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_U8_Un =
		 new OpCode("conv.ovf.u8.un", 0x89, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi8);

	public static readonly OpCode Conv_Ovf_I_Un =
		 new OpCode("conv.ovf.i.un", 0x8A, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_U_Un =
		 new OpCode("conv.ovf.u.un", 0x8B, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Box =
		 new OpCode("box", 0x8C, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Pop1, StackBehaviour.Pushref);

	[Obsolete("Use OpCodes.Box instead")]
	public static readonly OpCode Boxval =
		 new OpCode("boxval", 0x8C, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Pop1, StackBehaviour.Pushref);

	public static readonly OpCode Newarr =
		 new OpCode("newarr", 0x8D, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popi, StackBehaviour.Pushref);

	public static readonly OpCode Ldlen =
		 new OpCode("ldlen", 0x8E, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref, StackBehaviour.Pushi);

	public static readonly OpCode Ldelema =
		 new OpCode("ldelema", 0x8F, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popref_popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldelem_I1 =
		 new OpCode("ldelem.i1", 0x90, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldelem_U1 =
		 new OpCode("ldelem.u1", 0x91, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldelem_I2 =
		 new OpCode("ldelem.i2", 0x92, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldelem_U2 =
		 new OpCode("ldelem.u2", 0x93, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldelem_I4 =
		 new OpCode("ldelem.i4", 0x94, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldelem_U4 =
		 new OpCode("ldelem.u4", 0x95, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldelem_I8 =
		 new OpCode("ldelem.i8", 0x96, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushi8);

	public static readonly OpCode Ldelem_I =
		 new OpCode("ldelem.i", 0x97, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldelem_R4 =
		 new OpCode("ldelem.r4", 0x98, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushr4);

	public static readonly OpCode Ldelem_R8 =
		 new OpCode("ldelem.r8", 0x99, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushr8);

	public static readonly OpCode Ldelem_Ref =
		 new OpCode("ldelem.ref", 0x9A, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi, StackBehaviour.Pushref);

	public static readonly OpCode Stelem_I =
		 new OpCode("stelem.i", 0x9B, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Stelem_I1 =
		 new OpCode("stelem.i1", 0x9C, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Stelem_I2 =
		 new OpCode("stelem.i2", 0x9D, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Stelem_I4 =
		 new OpCode("stelem.i4", 0x9E, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Stelem_I8 =
		 new OpCode("stelem.i8", 0x9F, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi_popi8, StackBehaviour.Push0);

	public static readonly OpCode Stelem_R4 =
		 new OpCode("stelem.r4", 0xA0, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi_popr4, StackBehaviour.Push0);

	public static readonly OpCode Stelem_R8 =
		 new OpCode("stelem.r8", 0xA1, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi_popr8, StackBehaviour.Push0);

	public static readonly OpCode Stelem_Ref =
		 new OpCode("stelem.ref", 0xA2, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Popref_popi_popref, StackBehaviour.Push0);

	public static readonly OpCode Ldelem_Any =
		 new OpCode("ldelem.any", 0xA3, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popref_popi, StackBehaviour.Push1);

	public static readonly OpCode Stelem_Any =
		 new OpCode("stelem.any", 0xA4, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popref_popi_popref, StackBehaviour.Push0);

	public static readonly OpCode Unbox_Any =
		 new OpCode("unbox.any", 0xA5, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popref, StackBehaviour.Push1);

	public static readonly OpCode Conv_Ovf_I1 =
		 new OpCode("conv.ovf.i1", 0xB3, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_U1 =
		 new OpCode("conv.ovf.u1", 0xB4, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_I2 =
		 new OpCode("conv.ovf.i2", 0xB5, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_U2 =
		 new OpCode("conv.ovf.u2", 0xB6, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_I4 =
		 new OpCode("conv.ovf.i4", 0xB7, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_U4 =
		 new OpCode("conv.ovf.u4", 0xB8, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_I8 =
		 new OpCode("conv.ovf.i8", 0xB9, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi8);

	public static readonly OpCode Conv_Ovf_U8 =
		 new OpCode("conv.ovf.u8", 0xBA, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi8);

	// This is really an Objmodel instruction, but .NET declares it Primitive.
	public static readonly OpCode Refanyval =
		 new OpCode("refanyval", 0xC2, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineType,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Ckfinite =
		 new OpCode("ckfinite", 0xC3, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushr8);

	// This is really an Objmodel instruction, but .NET declares it Primitive.
	public static readonly OpCode Mkrefany =
		 new OpCode("mkrefany", 0xC6, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineType,
					StackBehaviour.Popi, StackBehaviour.Push1);

	// This is really an Objmodel instruction, but .NET declares it Primitive.
	public static readonly OpCode Ldtoken =
		 new OpCode("ldtoken", 0xD0, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineType,
					StackBehaviour.Popi, StackBehaviour.Push1);

	public static readonly OpCode Conv_U2 =
		 new OpCode("conv.u2", 0xD1, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_U1 =
		 new OpCode("conv.u1", 0xD2, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_I =
		 new OpCode("conv.i", 0xD3, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_I =
		 new OpCode("conv.ovf.i", 0xD4, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Conv_Ovf_U =
		 new OpCode("conv.ovf.u", 0xD5, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Add_Ovf =
		 new OpCode("add.ovf", 0xD6, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Add_Ovf_Un =
		 new OpCode("add.ovf.un", 0xD7, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Mul_Ovf =
		 new OpCode("mul.ovf", 0xD8, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Mul_Ovf_Un =
		 new OpCode("mul.ovf.un", 0xD9, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Sub_Ovf =
		 new OpCode("sub.ovf", 0xDA, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Sub_Ovf_Un =
		 new OpCode("sub.ovf.un", 0xDB, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Push1);

	public static readonly OpCode Endfinally =
		 new OpCode("endfinally", 0xDC, FlowControl.Return,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Leave =
		 new OpCode("leave", 0xDD, FlowControl.Branch,
		 			OpCodeType.Primitive, OperandType.InlineBrTarget,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	// This should probably be "Macro" instead of "Primitive", to be
	// consistent with "br.s", "beq.s", etc.  But .NET declares this
	// as "Primitive".
	public static readonly OpCode Leave_S =
		 new OpCode("leave.s", 0xDE, FlowControl.Branch,
		 			OpCodeType.Primitive, OperandType.ShortInlineBrTarget,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Stind_I =
		 new OpCode("stind.i", 0xDF, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Conv_U =
		 new OpCode("conv.u", 0xE0, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	public static readonly OpCode Prefixref =
		 new OpCode("prefixref", 0xFF, FlowControl.Meta,
		 			OpCodeType.Nternal, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Prefix1 =
		 new OpCode("prefix1", 0xFE, FlowControl.Meta,
		 			OpCodeType.Nternal, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Prefix2 =
		 new OpCode("prefix2", 0xFD, FlowControl.Meta,
		 			OpCodeType.Nternal, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Prefix3 =
		 new OpCode("prefix3", 0xFC, FlowControl.Meta,
		 			OpCodeType.Nternal, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Prefix4 =
		 new OpCode("prefix4", 0xFB, FlowControl.Meta,
		 			OpCodeType.Nternal, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Prefix5 =
		 new OpCode("prefix5", 0xFA, FlowControl.Meta,
		 			OpCodeType.Nternal, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Prefix6 =
		 new OpCode("prefix6", 0xF9, FlowControl.Meta,
		 			OpCodeType.Nternal, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Prefix7 =
		 new OpCode("prefix7", 0xF8, FlowControl.Meta,
		 			OpCodeType.Nternal, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Arglist =
		 new OpCode("arglist", 0xFE00, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Ceq =
		 new OpCode("ceq", 0xFE01, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1_pop1, StackBehaviour.Pushi);

	public static readonly OpCode Cgt =
		 new OpCode("cgt", 0xFE02, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi, StackBehaviour.Pushi);

	public static readonly OpCode Cgt_Un =
		 new OpCode("cgt.un", 0xFE03, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi, StackBehaviour.Pushi);

	public static readonly OpCode Clt =
		 new OpCode("clt", 0xFE04, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi, StackBehaviour.Pushi);

	public static readonly OpCode Clt_Un =
		 new OpCode("clt.un", 0xFE05, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi, StackBehaviour.Pushi);

	public static readonly OpCode Ldftn =
		 new OpCode("ldftn", 0xFE06, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineMethod,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	// This is really an Objmodel instruction, but .NET declares it Primitive.
	public static readonly OpCode Ldvirtftn =
		 new OpCode("ldvirtftn", 0xFE07, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineMethod,
					StackBehaviour.Popref, StackBehaviour.Pushi);

	public static readonly OpCode Ldarg =
		 new OpCode("ldarg", 0xFE09, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineVar,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldarga =
		 new OpCode("ldarga", 0xFE0A, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineVar,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Starg =
		 new OpCode("starg", 0xFE0B, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineVar,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Ldloc =
		 new OpCode("ldloc", 0xFE0C, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineVar,
					StackBehaviour.Pop0, StackBehaviour.Push1);

	public static readonly OpCode Ldloca =
		 new OpCode("ldloca", 0xFE0D, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineVar,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	public static readonly OpCode Stloc =
		 new OpCode("stloc", 0xFE0E, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineVar,
					StackBehaviour.Pop1, StackBehaviour.Push0);

	public static readonly OpCode Localloc =
		 new OpCode("localloc", 0xFE0F, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Pushi);

	public static readonly OpCode Endfilter =
		 new OpCode("endfilter", 0xFE11, FlowControl.Return,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi, StackBehaviour.Push0);

	public static readonly OpCode Unaligned =
		 new OpCode("unaligned.", 0xFE12, FlowControl.Meta,
		 			OpCodeType.Prefix, OperandType.ShortInlineI,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Volatile =
		 new OpCode("volatile.", 0xFE13, FlowControl.Meta,
		 			OpCodeType.Prefix, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Tailcall =
		 new OpCode("tail.", 0xFE14, FlowControl.Meta,
		 			OpCodeType.Prefix, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	public static readonly OpCode Initobj =
		 new OpCode("initobj", 0xFE15, FlowControl.Next,
		 			OpCodeType.Objmodel, OperandType.InlineType,
					StackBehaviour.Popi, StackBehaviour.Push0);

	public static readonly OpCode Cpblk =
		 new OpCode("cpblk", 0xFE17, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Initblk =
		 new OpCode("initblk", 0xFE18, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Popi_popi_popi, StackBehaviour.Push0);

	public static readonly OpCode Rethrow =
		 new OpCode("rethrow", 0xFE1A, FlowControl.Throw,
		 			OpCodeType.Objmodel, OperandType.InlineNone,
					StackBehaviour.Pop0, StackBehaviour.Push0);

	// This is really an Objmodel instruction, but .NET declares it Primitive.
	public static readonly OpCode Sizeof =
		 new OpCode("sizeof", 0xFE1C, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineType,
					StackBehaviour.Pop0, StackBehaviour.Pushi);

	// This is really an Objmodel instruction, but .NET declares it Primitive.
	public static readonly OpCode Refanytype =
		 new OpCode("refanytype", 0xFE1D, FlowControl.Next,
		 			OpCodeType.Primitive, OperandType.InlineNone,
					StackBehaviour.Pop1, StackBehaviour.Pushi);

	// Determine if an opcode takes a single-byte argument.
	public static bool TakesSingleByteArgument(OpCode inst)
			{
				OperandType type = (OperandType)(inst.operandType);
				return (type == OperandType.ShortInlineBrTarget ||
						type == OperandType.ShortInlineI ||
						type == OperandType.ShortInlineVar);
			}

}; // struct OpCodes

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
