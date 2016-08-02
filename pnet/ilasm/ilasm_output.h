/*
 * ilasm_output.h - Output method code.
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

#ifndef	_ILASM_OUTPUT_H
#define	_ILASM_OUTPUT_H

#include "il_program.h"
#include "il_utils.h"
#include "il_writer.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Global image writer that is in use.
 */
extern ILWriter *ILAsmWriter;

/*
 * Reset global variables back to their defaults.
 */
void ILAsmOutReset(void);

/*
 * Create the global image writer.
 */
void ILAsmOutCreate(FILE *stream, int seekable, int type, int flags);

/*
 * Flush and destroy the global image writer.
 */
int ILAsmOutDestroy(void);

/*
 * Output a simple instruction that has no arguments.
 */
void ILAsmOutSimple(ILInt32 opcode);

/*
 * Output an instruction that references a variable.
 */
void ILAsmOutVar(ILInt32 opcode, ILInt64 num);

/*
 * Output an instruction that takes an integer argument.
 */
void ILAsmOutInt(ILInt32 opcode, ILInt64 value);

/*
 * Push a 32-bit float value onto the stack.
 */
void ILAsmOutFloat(unsigned char *bytes);

/*
 * Push a 64-bit double value onto the stack.
 */
void ILAsmOutDouble(unsigned char *bytes);

/*
 * Output an instruction that takes a token as an argument.
 */
void ILAsmOutToken(ILInt32 opcode, ILUInt32 token);

/*
 * Output an instruction that pushes a string literal.
 */
void ILAsmOutString(ILIntString interned);

/*
 * Output a branch to a integer label within the current method.
 */
void ILAsmOutBranchInt(ILInt32 opcode, ILInt64 addr);

/*
 * Output a branch to a named label within the current method.
 */
void ILAsmOutBranch(ILInt32 opcode, const char *label);

/*
 * Start output of a switch statement.
 */
void ILAsmOutSwitchStart(void);

/*
 * Output an integer switch label reference within the current method.
 */
void ILAsmOutSwitchRefInt(ILInt64 addr);

/*
 * Output a switch label reference within the current method.
 */
void ILAsmOutSwitchRef(const char *label);

/*
 * End output of a switch statement.
 */
void ILAsmOutSwitchEnd(void);

/*
 * Output a named label at the current position within the method.
 */
void ILAsmOutLabel(const char *label);

/*
 * Convert an integer label name into its string form.
 */
const char *ILAsmOutIntToName(ILInt64 label);

/*
 * Output an integer label name at the current position.
 * Returns the string form of the label name.
 */
const char *ILAsmOutIntLabel(ILInt64 label);

/*
 * Output a unique label name for the current position,
 * and return the string form of the label name.
 */
const char *ILAsmOutUniqueLabel(void);

/*
 * Add debug line information at this point within the method.
 */
void ILAsmOutDebugLine(const char *filename, ILUInt32 line, ILUInt32 column);

/*
 * Start output of an SSA instruction.
 */
void ILAsmOutSSAStart(ILInt32 opcode);

/*
 * Output an SSA value.
 */
void ILAsmOutSSAValue(ILInt64 value);

/*
 * End output of an SSA instruction.
 */
void ILAsmOutSSAEnd(void);

/*
 * Set the maximum stack size for the current method.
 */
void ILAsmOutMaxStack(ILUInt32 maxStack);

/*
 * Set the maximum number of locals for the current method.
 */
void ILAsmOutMaxLocals(ILUInt32 maxLocals);

/*
 * Set the initialization flag for the local variables.
 */
void ILAsmOutZeroInit(void);

/*
 * Add a group of local variables to the current method.
 * This also free's the "vars" list.
 */
void ILAsmOutAddLocals(ILAsmParamInfo *vars);

/*
 * Add a group of parameters to the current method.
 */
void ILAsmOutAddParams(ILAsmParamInfo *vars, ILUInt32 callConv);

/*
 * Look up a local or parameter and return the index.
 */
ILUInt32 ILAsmOutLookupVar(const char *name);

/*
 * Information that is stored for an exception handler block.
 */
typedef struct _tagILAsmOutException ILAsmOutException;
struct _tagILAsmOutException
{
	ILUInt32			flags;
	const char		   *blockStart;
	const char		   *blockEnd;
	ILUInt32			blockOffset;
	ILUInt32			blockLength;
	const char		   *handlerStart;
	const char		   *handlerEnd;
	ILUInt32			handlerOffset;
	ILUInt32			handlerLength;
	ILClass			   *classToCatch;
	const char		   *filterLabel;
	ILUInt32			filterOffset;
	ILAsmOutException  *next;

};

/*
 * Make an exception handler block for the current method.
 */
ILAsmOutException *ILAsmOutMakeException(ILUInt32 flags, ILClass *classInfo,
									     const char *filterLabel,
										 const char *handlerStart,
									     const char *handlerEnd);

/*
 * Add a try block to the current method.
 */
void ILAsmOutAddTryBlock(const char *blockStart, const char *blockEnd,
						 ILAsmOutException *handlers);

/*
 * Finalize processing for a method.
 */
void ILAsmOutFinalizeMethod(ILMethod *method);

/*
 * Output a resource stream to the image being constructed.
 */
void ILAsmOutAddResource(const char *name, FILE *stream);

/*
 * Declare a local variable name for debug symbol information.
 */
void ILAsmOutDeclareVarName(const char *name, ILUInt32 index);

/*
 * Push into a nested local variable scope.
 */
void ILAsmOutPushVarScope(const char *name);

/*
 * Pop out of a nested local variable scope.
 */
void ILAsmOutPopVarScope(const char *name);

/*
 * Initialize the constant pool attached to the current class
 */
void ILJavaAsmInitPool();

/*
 * Output a simple java instruction that has no arguments.
 */
void ILJavaAsmOutSimple(ILInt32 opcode);

/*
 * Output a java instruction that references a variable.
 */
void ILJavaAsmOutVar(ILInt32 opcode, ILInt64 num) ;

/*
 * Output a java instruction that increments a local variable argument.
 */
void ILJavaAsmOutInc(ILInt32 opcode, ILInt64 index, ILInt64 val) ;

/*
 * Output a java instruction that takes an integer argument.
 */
void ILJavaAsmOutInt(ILInt32 opcode, ILInt64 value);

/*
 * Output a java instruction that push an integer constant.
 */
void ILJavaAsmOutConstInt32(ILInt32 opcode, ILInt64 value);

/*
 * Output a java instruction that push an integer constant.
 */
void ILJavaAsmOutConstInt64(ILInt32 opcode, ILInt64 value);

/*
 * Output a java instruction that push a floating point constant.
 */
void ILJavaAsmOutConstFloat32(ILInt32 opcode, ILUInt8 *value);

/*
 * Output a java instruction that push a floating point constant.
 */
void ILJavaAsmOutConstFloat64(ILInt32 opcode, ILUInt8 *value);

/*
 * Output a java instruction that push an integer constant.
 */
void ILJavaAsmOutString(ILIntString interned);

/*
 * Output a java instruction that takes an argument specified by a IL token.
 */
void ILJavaAsmOutToken(ILInt32 opcode, ILUInt32 token);

/*
 * Output a java instruction that takes a constant pool method or field 
 * argument.
 */
void ILJavaAsmOutRef(ILInt32 opcode, int isMethod, const char *className, 
					 const char *refName, const char *sigName);

/*
 * Output a java instruction that takes a constant pool type argument.
 */
void ILJavaAsmOutType(ILInt32 opcode, const char *className);

/*
 * Output a newarray java instruction.
 */
void ILJavaAsmOutNewarray(ILInt32 opcode, ILInt64 type);

/*
 * Output a multinewarray java instruction.
 */
void ILJavaAsmOutMultinewarray(ILInt32 opcode, ILType *type, ILInt64 dim);

/*
 * Output a multinewarray java instruction.
 * The type (typeName) is a string in java form (i.e. "java/lang/Object").
 */
void ILJavaAsmOutMultinewarrayFromName(ILInt32 opcode, const char *typeName, ILInt64 dim);

/* 
 * Set the table switch default label.
 */
void ILJavaAsmOutTableSwitchDefaultRefInt(ILInt64 addr);

/*
 * Set the table switch default label.
 */
void ILJavaAsmOutTableSwitchDefaultRef(const char *label);

/*
 * Start output of a java table switch statement.
 */
void ILJavaAsmOutTableSwitchStart(ILInt64 low);

/*
 * Output an integer java table switch label reference within the current method.
 */
void ILJavaAsmOutTableSwitchRefInt(ILInt64 addr);

/*
 * Output a java table switch label reference within the current method.
 */
void ILJavaAsmOutTableSwitchRef(const char *label);

/*
 * End output of a java table switch statement.
 */
void ILJavaAsmOutTableSwitchEnd(ILUInt64 low);

/*
 * Set the lookup switch default label.
 */
void ILJavaAsmOutLookupSwitchDefaultRefInt(ILInt64 addr);

/*
 * Set the lookup switch default label.
 */
void ILJavaAsmOutLookupSwitchDefaultRef(const char *label);

/*
 * Start output of a java lookup switch statement.
 */
void ILJavaAsmOutLookupSwitchStart(void);

/*
 * Output an integer java lookup switch label reference within the current method.
 */
void ILJavaAsmOutLookupSwitchRefInt(ILInt64 match, ILInt64 addr);

/*
 * Output a java lookup switch label reference within the current method.
 */
void ILJavaAsmOutLookupSwitchRef(ILInt64 match, const char *label);

/*
 * End output of a java lookup switch statement.
 */
void ILJavaAsmOutLookupSwitchEnd(void);

#ifdef	__cplusplus
};
#endif

#endif	/* _ILASM_OUTPUT_H */
