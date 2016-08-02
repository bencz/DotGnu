/*
 * jv_output.h - Assembly code output routines for Java bytecode.
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

#ifndef	_CODEGEN_JV_OUTPUT_H
#define	_CODEGEN_JV_OUTPUT_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Initialize the Java-specific structures in an ILGenInfo structure.
 */
void JavaGenInit(ILGenInfo *info);

/*
 * Destroy the Java-specific structures in an ILGenInfo structure.
 */
void JavaGenDestroy(ILGenInfo *info);

/*
 * Clear the Java code output state, ready for a new .class file.
 */
void JavaGenClear(ILGenInfo *info);

/*
 * Output a simple instruction.
 */
void JavaGenSimple(ILGenInfo *info, int opcode);

/*
 * Load an "int32" value onto the stack.
 */
void JavaGenInt32(ILGenInfo *info, ILInt32 value);

/*
 * Load a "uint32" value onto the stack.
 */
void JavaGenUInt32(ILGenInfo *info, ILUInt32 value);

/*
 * Load an "int64" value onto the stack.
 */
void JavaGenInt64(ILGenInfo *info, ILInt64 value);

/*
 * Load a "uint64" value onto the stack.
 */
void JavaGenUInt64(ILGenInfo *info, ILUInt64 value);

/*
 * Load a "float32" value onto the stack.
 */
void JavaGenFloat32(ILGenInfo *info, ILFloat value);

/*
 * Load a "float64" value onto the stack.
 */
void JavaGenFloat64(ILGenInfo *info, ILDouble value);

/*
 * Load a string literal onto the stack.
 */
void JavaGenStringConst(ILGenInfo *info, const char *str, int len);

/*
 * Output a jump instruction.
 */
void JavaGenJump(ILGenInfo *info, int opcode, ILLabel *label);

/*
 * Output a label at the current code location.
 */
void JavaGenLabel(ILGenInfo *info, ILLabel *label);

/*
 * Store a value to a local variable.
 */
void JavaGenStoreLocal(ILGenInfo *info, unsigned varNum,
					   ILMachineType type);

/*
 * Load a value from a local variable.
 */
void JavaGenLoadLocal(ILGenInfo *info, unsigned varNum,
					  ILMachineType type);

/*
 * Store a value to an argument variable.
 */
void JavaGenStoreArg(ILGenInfo *info, unsigned varNum,
					 ILMachineType type);

/*
 * Load a value from an argument variable.
 */
void JavaGenLoadArg(ILGenInfo *info, unsigned varNum,
				    ILMachineType type);

/*
 * Increment an integer local variable.
 */
void JavaGenIncLocal(ILGenInfo *info, unsigned varNum, ILInt32 amount);

/*
 * Output a "ret" instruction, taking the address from a specific local.
 */
void JavaGenRet(ILGenInfo *info, unsigned varNum);

/*
 * Get the number of stack positions occupied by a machine type.
 */
int JavaGenTypeSize(ILMachineType type);

/*
 * Duplicate a machine type on the stack, and adjust the stack height.
 */
void JavaGenDup(ILGenInfo *info, ILMachineType type);

/*
 * Load a value from an array element.
 */
void JavaGenLoadArray(ILGenInfo *info, ILMachineType type);

/*
 * Store a value to an array element.
 */
void JavaGenStoreArray(ILGenInfo *info, ILMachineType type);

/*
 * Output a call to a static method given its named signature.
 */
void JavaGenCallByName(ILGenInfo *info, const char *className,
					   const char *methodName, const char *signature);

/*
 * Output a call to a static intrinsic method given its named signature.
 */
void JavaGenCallIntrinsic(ILGenInfo *info, const char *methodName,
						  const char *signature);

/*
 * Output a call to a named virtual method.
 */
void JavaGenCallVirtual(ILGenInfo *info, const char *className,
						const char *methodName, const char *signature);

/*
 * Output a call to a named virtual method in an intrinsc class.
 */
void JavaGenCallVirtIntrinsic(ILGenInfo *info, const char *className,
						      const char *methodName, const char *signature);

/*
 * Output a call to a named interface method.
 */
void JavaGenCallInterface(ILGenInfo *info, const char *className,
						  const char *methodName, const char *signature,
						  long numArgs);

/*
 * Output a call to a method given its description block.
 */
void JavaGenCallByMethod(ILGenInfo *info, ILMethod *method);

/*
 * Output a call to a virtual method given its description block.
 */
void JavaGenCallVirtByMethod(ILGenInfo *info, ILMethod *method);

/*
 * Output a call to an interface method given its description block.
 */
void JavaGenCallInterfaceByMethod(ILGenInfo *info, ILMethod *method,
								  long numArgs);

/*
 * Output a call to a special method given its description block.
 */
void JavaGenCallSpecialByMethod(ILGenInfo *info, ILMethod *method);

/*
 * Call a method using one of the 4 above functions, depending
 * upon the type of method block we are given.
 */
void JavaGenCallMethod(ILGenInfo *info, ILMethod *method, long startArgs);

/*
 * Output a "new" instruction.
 */
void JavaGenNewObj(ILGenInfo *info, const char *className);

/*
 * Output a "new" instruction for an intrinsic class.
 */
void JavaGenNewIntrinsic(ILGenInfo *info, const char *className);

/*
 * Output a call to a constructor given its named signature.
 */
void JavaGenCallCtor(ILGenInfo *info, const char *className,
					 const char *methodName, const char *signature);

/*
 * Output a call to an intrinsic constructor given its named signature.
 */
void JavaGenCallCtorIntrinsic(ILGenInfo *info, const char *className,
					 		  const char *methodName, const char *signature);

/*
 * Generate an instruction that refers to a class.
 */
void JavaGenClassRef(ILGenInfo *info, int opcode, ILClass *classInfo);

/*
 * Generate an instruction that refers to a type.
 */
void JavaGenTypeRef(ILGenInfo *info, int opcode, ILType *type);

/*
 * Generate an instruction that refers to a class by name.
 */
void JavaGenClassName(ILGenInfo *info, int opcode, const char *className);

/*
 * Output a "new array" instruction.
 */
void JavaGenNewArray(ILGenInfo *info, ILType *elemType);

/*
 * Output an instruction to generate a new multi-dimensional array.
 */
void JavaGenNewMultiArray(ILGenInfo *info, ILType *type, int rank);

/*
 * Output an instruction that refers to a field.
 */
void JavaGenFieldRef(ILGenInfo *info, int opcode, ILField *field);

/*
 * Output the correct form of return instruction based on a machine type.
 */
void JavaGenReturnInsn(ILGenInfo *info, ILMachineType type);

/*
 * Start the allocation of a method's frame.
 */
void JavaGenStartFrame(ILGenInfo *info);

/*
 * Mark the end of the method arguments and the start of the locals.
 */
void JavaGenStartLocals(ILGenInfo *info);

/*
 * Add a frame slot of a particular size.
 */
void JavaGenAddFrameSlot(ILGenInfo *info, ILMachineType machineType);

/*
 * Get the number of local variable slots needed for the current method.
 */
unsigned JavaGenNumLocals(ILGenInfo *info);

/*
 * Flush the peephole optimization queue.
 */
void JavaGenFlush(ILGenInfo *info);

/*
 * Adjust the height of the operand stack.
 */
#define	JavaGenAdjust(info,amount)	ILGenAdjust((info), (amount))

/*
 * Extend the height of the operand stack to account for temporary values.
 */
#define	JavaGenExtend(info,amount)	ILGenExtend((info), (amount))

/*
 * Convert the name of a class information block into
 * a Java class name.  The return value must be free'd
 * with "ILFree".
 */
char *JavaGetClassName(ILGenInfo *info, ILClass *classInfo);

/*
 * Append two strings.  "str1" is realloc'ed to be large enough.
 */
char *JavaStrAppend(ILGenInfo *info, char *str1, const char *str2);

/*
 * Generate the start of a "switch" instruction.
 */
void JavaGenSwitchStart(ILGenInfo *info, int opcode, 
						ILLabel *label, ILUInt32 offset);

/*
 * Generate a label reference for a "tableswitch" instruction.
 */
void JavaGenSwitchRef(ILGenInfo *info, ILLabel *label, int comma);

/*
 * Generate a label reference for a "lookupswitch" instruction.
 */
void JavaGenSwitchLookupRef(ILGenInfo *info, ILInt32 value,
						    ILLabel *label, int comma);

/*
 * Generate the end of a "switch" instruction.
 */
void JavaGenSwitchEnd(ILGenInfo *info);

#ifdef	__cplusplus
};
#endif

#endif	/* _CODEGEN_JV_OUTPUT_H */
