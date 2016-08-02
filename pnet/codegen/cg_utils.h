/*
 * cg_utils.h - Handy utilities for the code generator.
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

#ifndef	_CODEGEN_CG_UTILS_H
#define	_CODEGEN_CG_UTILS_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Push a 32-bit integer constant onto the stack.
 */
void ILGenInt32(ILGenInfo *info, ILInt32 value);

/*
 * Push an unsigned 32-bit integer constant onto the stack.
 */
void ILGenUInt32(ILGenInfo *info, ILUInt32 value);

/*
 * Push a 64-bit integer constant onto the stack.
 */
void ILGenInt64(ILGenInfo *info, ILInt64 value);

/*
 * Push an unsigned 64-bit integer constant onto the stack.
 */
void ILGenUInt64(ILGenInfo *info, ILUInt64 value);

/*
 * Push a native integer constant onto the stack.
 */
void ILGenIntNative(ILGenInfo *info, ILInt32 value);

/*
 * Push an unsigned native integer constant onto the stack.
 */
void ILGenUIntNative(ILGenInfo *info, ILUInt32 value);

/*
 * Load a value from a local variable onto the stack.
 */
void ILGenLoadLocal(ILGenInfo *info, unsigned num);

/*
 * Store a value from the stack top to a local variable.
 */
void ILGenStoreLocal(ILGenInfo *info, unsigned num);

/*
 * Load the address of a local variable onto the stack.
 */
void ILGenLoadLocalAddr(ILGenInfo *info, unsigned num);

/*
 * Load a value from an argument variable onto the stack.
 */
void ILGenLoadArg(ILGenInfo *info, unsigned num);

/*
 * Store a value from the stack top to an argument variable.
 */
void ILGenStoreArg(ILGenInfo *info, unsigned num);

/*
 * Load the address of an argument variable onto the stack.
 */
void ILGenLoadArgAddr(ILGenInfo *info, unsigned num);

/*
 * Generate code to output a constant value.
 */
void ILGenConst(ILGenInfo *info, ILEvalValue *value);

/*
 * Load the contents of an array element onto the stack.
 */
void ILGenLoadArray(ILGenInfo *info, ILMachineType elemMachineType,
					ILType *elemType);

/*
 * Prepare to store the contents of an array element,
 * before the value is pushed.  Returns the number of
 * stack values popped from the stack during the prepare.
 */
int ILGenStoreArrayPrepare(ILGenInfo *info, ILMachineType elemMachineType,
					 	   ILType *elemType);

/*
 * Store the contents of an array element from the stack.
 */
void ILGenStoreArray(ILGenInfo *info, ILMachineType elemMachineType,
					 ILType *elemType);

/*
 * Load the contents of a managed pointer onto the stack.
 */
void ILGenLoadManaged(ILGenInfo *info, ILMachineType machineType,
					  ILType *type);

/*
 * Store a value to a managed pointer.
 */
void ILGenStoreManaged(ILGenInfo *info, ILMachineType machineType,
					   ILType *type);

#ifdef	__cplusplus
};
#endif

#endif	/* _CODEGEN_CG_UTILS_H */
