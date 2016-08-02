/*
 * c_function.h - Declare and output functions for the C language.
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

#ifndef	_C_FUNCTION_H
#define	_C_FUNCTION_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Create a new function and return its method block.
 */
ILMethod *CFunctionCreate(ILGenInfo *info, const char *name, ILNode *node,
						  CDeclSpec spec, CDeclarator decl,
						  ILNode *declaredParams);

/*
 * Declare the parameters of a function into a scope.
 */
void CFunctionDeclareParams(ILGenInfo *info, ILMethod *method);

/*
 * Output a function that has been fully parsed.
 */
void CFunctionOutput(ILGenInfo *info, ILMethod *method, ILNode *body,
					 int initGlobalVars);

/*
 * Flush a list of global initializers.
 */
void CFunctionFlushInits(ILGenInfo *info, ILNode *list);

/*
 * Pre-declare some standard functions.  e.g. "printf", "scanf", etc.
 */
void CFunctionPredeclare(ILGenInfo *info);

/*
 * Get the natural passing type for a type, in either normal or vararg mode.
 */
ILType *CFunctionNaturalType(ILGenInfo *info, ILType *type, int vararg);

/*
 * Record that we saw a "setjmp" reference in the current function
 * while we were performing semantic analysis.
 */
void CFunctionSawSetJmp(void);

/*
 * Generate code for a "setjmp" operation.  It is assumed that the
 * pointer to the "jmp_buf" is already on the stack.
 */
void CGenSetJmp(ILGenInfo *info);

/*
 * Get the current method that is being analysed or compiled.
 */
ILMethod *CFunctionGetCurrent(void);

/*
 * Allocate an index for a local variable in the current function.
 */
unsigned CGenAllocLocal(ILGenInfo *info, ILType *type);

/*
 * Define a weak alias for a function.
 */
void CFunctionWeakAlias(ILGenInfo *info, const char *name, ILNode *node,
						ILType *signature, const char *aliasFor,
						int isPrivate);

/*
 * Define a strong alias for a function.
 */
void CFunctionStrongAlias(ILGenInfo *info, const char *name, ILNode *node,
						  ILType *signature, const char *aliasFor,
						  int isPrivate);

/*
 * Declare a PInvoke import.
 */
void CFunctionPInvoke(ILGenInfo *info, const char *name, ILNode *node,
					  ILType *signature, const char *moduleName,
					  const char *aliasName, ILUInt32 flags, int isPrivate);

/*
 * Determine if an attribute is present in a list.
 */
int CAttrPresent(ILNode *attrs, const char *name, const char *altName);

/*
 * Get the value of a string attribute from a list.
 */
const char *CAttrGetString(ILNode *attrs, const char *name,
						   const char *altName);

/*
 * Get the value of an integer attribute from a list.
 */
ILInt32 CAttrGetInt(ILNode *attrs, const char *name,
					const char *altName, ILInt32 defValue);

#ifdef	__cplusplus
};
#endif

#endif	/* _C_FUNCTION_H */
