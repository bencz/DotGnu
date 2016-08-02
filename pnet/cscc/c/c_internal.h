/*
 * c_internal.h - Internal definitions for the C compiler front end.
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

#ifndef	_CSCC_C_INTERNAL_H
#define	_CSCC_C_INTERNAL_H

#include <cscc/common/cc_main.h>
#include <cscc/c/c_defs.h>
#include <cscc/c/c_lexutils.h>
#include <cscc/c/c_types.h>
#include <cscc/c/c_declspec.h>
#include <cscc/c/c_function.h>
#include <codegen/cg_scope.h>
#include <cscc/c/c_scope.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Duplicate a block of memory to be stored as an extra field
 * in a semantic analysis value block.
 */
void *CSemDupExtra(const void *buf, unsigned int len);

/*
 * Clone the line number information from "oldNode" onto "newNode".
 */
void CGenCloneLine(ILNode *newNode, ILNode *oldNode);

/*
 * Generate "crt0" glue logic if the current module has "main".
 */
void CGenCrt0(ILGenInfo *info, FILE *stream);

/*
 * Begin the code generation process.
 */
void CGenBeginCode(ILGenInfo *info);

/*
 * End the code generation process, flushing remaining definitions.
 */
void CGenEndCode(ILGenInfo *info);

/*
 * Output the attributes that are attached to a program item.
 */
void CGenOutputAttributes(ILGenInfo *info, FILE *stream, ILProgramItem *item);

/*
 * Register the builtin "clang" library so the compiler can use it.
 */
void CGenRegisterLibrary(ILGenInfo *info);

/*
 * Determine if it is possible to coerce "fromType" to "toType".
 */
int CCanCoerce(ILType *fromType, ILType *toType);

/*
 * Determine if it is possible to coerce the value "fromValue" to "toType".
 */
int CCanCoerceValue(CSemValue fromValue, ILType *toType);

/*
 * Coerce a node from the type represented by "fromValue" to "toType".
 * Returns a new semantic value that describes the coerced value.
 */
CSemValue CCoerceNode(ILGenInfo *info, ILNode *node, ILNode **parent,
				      CSemValue fromValue, ILType *toType);

/*
 * Cast a node from the type represented by "fromValue" to "toType".
 * Returns a new semantic value that describes the coerced value.
 */
CSemValue CCastNode(ILGenInfo *info, ILNode *node, ILNode **parent,
			        CSemValue fromValue, ILType *toType);

/*
 * Generate code for computing the size of a type and pushing it
 * onto the runtime stack.
 */
void CGenSizeOf(ILGenInfo *info, ILType *type);

/*
 * Get the size of an array initializer for a particular array or struct type.
 * The "init" parameter is guaranteed to be a node of type "ILNode_CArrayInit".
 */
ILUInt32 CArrayInitializerSize(ILType *type, ILNode *init);

/*
 * Inhibit treecc node allocation rollback.
 */
void CInhibitNodeRollback(void);

/*
 * Add the offset of a field within a complex type to the value on the stack.
 */
void CAddComplexFieldOffset(ILGenInfo *info, ILType *type, ILField *field);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSCC_C_INTERNAL_H */
