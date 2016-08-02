/*
 * cg_output.h - Assembly code output routines.
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

#ifndef	_CODEGEN_CG_OUTPUT_H
#define	_CODEGEN_CG_OUTPUT_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Output a simple instruction.
 */
void ILGenSimple(ILGenInfo *info, int opcode);

/*
 * Output an instruction that has a single-byte argument.
 */
void ILGenByteInsn(ILGenInfo *info, int opcode, int arg);

/*
 * Output an instruction that has a single-short argument.
 */
void ILGenShortInsn(ILGenInfo *info, int opcode, ILUInt32 arg);

/*
 * Output an instruction that has a 32-bit word argument.
 */
void ILGenWordInsn(ILGenInfo *info, int opcode, ILUInt32 arg);

/*
 * Output an instruction that has a 64-bit dword argument.
 */
void ILGenDWordInsn(ILGenInfo *info, int opcode, ILUInt64 arg);

/*
 * Load a "float32" value onto the stack.
 */
void ILGenLoadFloat32(ILGenInfo *info, ILFloat value);

/*
 * Load a "float64" value onto the stack.
 */
void ILGenLoadFloat64(ILGenInfo *info, ILDouble value);

/*
 * Load a string literal onto the stack.
 */
void ILGenLoadString(ILGenInfo *info, const char *str, int len);

/*
 * Allocate a new local variable in the current method.
 */
void ILGenAllocLocal(ILGenInfo *info, ILType *type, const char *name);

/*
 * Output a jump instruction.
 */
void ILGenJump(ILGenInfo *info, int opcode, ILLabel *label);

/*
 * Output a "br_or_leave" pseudo-instruction.  The assembler
 * will use "leave" if the destination is marked as a leave
 * label, or "br" otherwise.  This is needed to handle certain
 * kinds of "goto" statements that refer to labels that have
 * not yet been seen, but which may be outside a "try" block.
 */
void ILGenBrOrLeaveJump(ILGenInfo *info, ILLabel *label);

/*
 * Output a label at the current code location.
 */
void ILGenLabel(ILGenInfo *info, ILLabel *label);

/*
 * Output a leave label at the current code location.
 */
void ILGenLeaveLabel(ILGenInfo *info, ILLabel *label);

/*
 * Get a new label number.
 */
ILLabel ILGenNewLabel(ILGenInfo *info);

/*
 * Output a call to a method given its named signature.
 */
void ILGenCallByName(ILGenInfo *info, const char *name);

/*
 * Output a call to a named virtual method.
 */
void ILGenCallVirtual(ILGenInfo *info, const char *name);

/*
 * Output a call to a method given its description block.
 */
void ILGenCallByMethod(ILGenInfo *info, ILMethod *method);

/*
 * Output a call to a method with an optional call site signature.
 */
void ILGenCallByMethodSig(ILGenInfo *info, ILMethod *method,
					      ILType *callSiteSig);

/*
 * Output a call to a constructor given its description block.
 */
void ILGenCtorByMethod(ILGenInfo *info, ILMethod *method,
					   ILType *callSiteSig);

/*
 * Output a call to a virtual method given its description block.
 */
void ILGenCallVirtByMethod(ILGenInfo *info, ILMethod *method);

/*
 * Output a call to a virtual method with an optional call site signature.
 */
void ILGenCallVirtByMethodSig(ILGenInfo *info, ILMethod *method,
							  ILType *callSiteSig);

/*
 * Output a call to a normal or virtual method given its description block.
 */
void ILGenCallMethod(ILGenInfo *info, ILMethod *method);

/*
 * Output a "newobj" instruction and call a constructor.
 */
void ILGenNewObj(ILGenInfo *info, const char *className,
				 const char *signature);

/*
 * Create a new delegate instance.
 */
void ILGenNewDelegate(ILGenInfo *info, ILClass *classInfo);

/*
 * Load the address of a method onto the stack.
 */
void ILGenLoadMethod(ILGenInfo *info, int opcode, ILMethod *method);

/*
 * Output an instruction that takes a class token as an argument.
 */
void ILGenClassToken(ILGenInfo *info, int opcode, ILClass *classInfo);

/*
 * Output an instruction that takes a class name as an argument.
 */
void ILGenClassName(ILGenInfo *info, int opcode, const char *className);

/*
 * Output an instruction that takes a type token as an argument.
 */
void ILGenTypeToken(ILGenInfo *info, int opcode, ILType *type);

/*
 * Output an array creation expression for a single-dimensional
 * array of a particular element type.
 */
void ILGenArrayNew(ILGenInfo *info, ILType *type);

/*
 * Output a constructor call for a multi-dimensional array type.
 */
void ILGenArrayCtor(ILGenInfo *info, ILType *type);

/*
 * Output a method call to get an item from a multi-dimensional array.
 */
void ILGenArrayGet(ILGenInfo *info, ILType *type);

/*
 * Output a method call to set an item within a multi-dimensional array.
 */
void ILGenArraySet(ILGenInfo *info, ILType *type);

/*
 * Output an instruction that refers to a field.
 */
void ILGenFieldRef(ILGenInfo *info, int opcode, ILField *field);

/*
 * Flush the peephole optimization queue.
 */
void ILGenFlush(ILGenInfo *info);

/*
 * Output module and assembly information for the image.
 */
void ILGenModulesAndAssemblies(ILGenInfo *info);

/*
 * Generate the start of a "switch" instruction.
 */
void ILGenSwitchStart(ILGenInfo *info);

/*
 * Generate a label reference for a "switch" instruction.
 */
void ILGenSwitchRef(ILGenInfo *info, ILLabel *label, int comma);

/*
 * Generate the end of a "switch" instruction.
 */
void ILGenSwitchEnd(ILGenInfo *info);

#ifdef	__cplusplus
};
#endif

#endif	/* _CODEGEN_CG_OUTPUT_H */
