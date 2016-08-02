/*
 * pm_output.h - Assembly code output routines for the Parrot Machine.
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

#ifndef	_CODEGEN_PM_OUTPUT_H
#define	_CODEGEN_PM_OUTPUT_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Code is normally generated as follows:
 *
 *	PMRegister dest, src1, src2;
 *  ILMachineType commonType;
 *
 *  src1 = PMGenValue(node->expr1, info);
 *  src2 = PMGenValue(node->expr2, info);
 *
 *  commonType = ILCommonType(info, PMGenGetType(src1), PMGenGetType(src2), 0);
 *  dest = PMGenDestBinary(info, src1, src2, commonType);
 *
 *  PMGenBinary(info, PM_ADD, dest, src1, src2);
 *
 *  return dest;
 */

/*
 * Operation names.
 */
enum PMOperation
{
	PM_CLONE,
	PM_SET_ADDR,
	PM_IF_EQ,
	PM_IF_NE,
	PM_IF_LT,
	PM_IF_LE,
	PM_IF_GT,
	PM_IF_GE,
	PM_ADD,
	PM_SUB,
	PM_MUL,
	PM_DIV,
	PM_REM,
	PM_NEG,
	PM_AND,
	PM_OR,
	PM_XOR,
	PM_NOT,
	PM_SHL,
	PM_SHR,
	PM_USHR,
	PM_LNOT,
	PM_DEFINED,
};

/*
 * Virtual register number.
 */
typedef unsigned PMRegister;

/*
 * Start code generation for a new method.
 */
void PMGenStartMethod(ILGenInfo *info, ILMethod *method, ILType *localVarSig);

/*
 * End code generation for the current method.
 */
void PMGenEndMethod(ILGenInfo *info);

/*
 * Generate return code.  "retValue" is ignored if the method returns "void".
 */
void PMGenReturn(ILGenInfo *info, PMRegister retValue);

/*
 * Print a virtual register number to the assembly output stream.
 */
void PMGenRegister(ILGenInfo *info, PMRegister reg);

/*
 * Allocate a temporary virtual register number of a specific type.
 */
PMRegister PMGenTempReg(ILGenInfo *info, ILMachineType type);

/*
 * Get a virtual register number for a particular argument.
 */
PMRegister PMGenArgReg(ILGenInfo *info, ILUInt32 num);

/*
 * Get a virtual register number for a declared local variable.
 */
PMRegister PMGenLocalReg(ILGenInfo *info, ILUInt32 num);

/*
 * Mark a virtual register so that it cannot be used as an
 * operator destination.
 */
void PMGenMarkRegNonDest(ILGenInfo *info, PMRegister reg);

/*
 * Get the type associated with a virtual register number.
 */
ILMachineType PMGenRegType(ILGenInfo *info, PMRegister reg);

/*
 * Get a destination register for a unary operation.  This may be
 * the same as "arg" if "arg" is a temporary.
 */
PMRegister PMGenDestUnary(ILGenInfo *info, PMRegister arg, ILMachineType type);

/*
 * Get a destination register for a binary operation.  This may be
 * the same as "arg1" or "arg2" if one of them is a temporary.
 */
PMRegister PMGenDestBinary(ILGenInfo *info, PMRegister arg1,
						   PMRegister arg2, ILMachineType type);

/*
 * Copy a value from one register to another.
 */
void PMGenCopy(ILGenInfo info, PMRegster dest, PMRegister src);

/*
 * Fetch a keyed value from an aggregate.
 */
void PMGenFetchKeyed(ILGenInfo info, PMRegster dest, PMRegister aggregate,
					 PMRegister key);

/*
 * Store a keyed value to an aggregate.
 */
void PMGenStoreKeyed(ILGenInfo info, PMRegister aggregate, PMRegister key,
					 PMRegister src);

/*
 * Generate code for a unary operation.
 */
void PMGenUnary(ILGenInfo *info, PMOperation oper, PMRegister dest,
				PMRegister src);

/*
 * Generate code for a binary operation.
 */
void PMGenBinary(ILGenInfo *info, PMOperation oper, PMRegister dest,
				 PMRegister src1, PMRegister src2);

/*
 * Generate a label.
 */
void PMGenLabel(ILGenInfo *info, ILLabel *label);

/*
 * Branch to a label on a condition.
 */
void PMGenBranch(ILGenInfo *info, PMOperation oper, PMRegister src1,
				 PMRegister src2, ILLabel *label);

/*
 * Branch to a label if a value is true or false.
 */
void PMGenBranchTrue(ILGenInfo *info, PMRegister src, ILLabel *label);
void PMGenBranchFalse(ILGenInfo *info, PMRegister src, ILLabel *label);

/*
 * Jump unconditionally to a label.
 */
void PMGenJump(ILGenInfo *info, ILLabel *label);

/*
 * Load the "null" constant into a register.
 */
PMRegister PMGenNull(ILGenInfo *info);

/*
 * Load an integer constant into a register.
 */
PMRegister PMGenInt32(ILGenInfo *info, ILInt32 num);
PMRegister PMGenUInt32(ILGenInfo *info, ILUInt32 num);
PMRegister PMGenInt64(ILGenInfo *info, ILInt64 num);
PMRegister PMGenUInt64(ILGenInfo *info, ILUInt64 num);

/*
 * Load a floating-point constant into a register.
 */
PMRegister PMGenFloat32(ILGenInfo *info, ILFloat num);
PMRegister PMGenFloat64(ILGenInfo *info, ILDouble num);

/*
 * Load a string constant into a register (this will be a
 * Parrot string, which will normally need to be converted
 * into a language-level string object).
 */
PMRegister PMGenString(ILGenInfo *info, const char *str, int len);

#ifdef	__cplusplus
};
#endif

#endif	/* _CODEGEN_PM_OUTPUT_H */
