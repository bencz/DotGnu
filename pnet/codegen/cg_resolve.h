/*
 * cg_resolve.h - Resolve methods, fields, operators, etc.
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

#ifndef	__CODEGEN_CG_RESOLVE_H__
#define	__CODEGEN_CG_RESOLVE_H__

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Resolve a call to a static method.
 */
ILMethod *ILResolveStaticMethod(ILGenInfo *info, ILClass *classInfo,
								ILClass *callScope, const char *name,
								ILType **args, int numArgs);

/*
 * Resolve a call to an instance method.
 */
ILMethod *ILResolveInstanceMethod(ILGenInfo *info, ILClass *classInfo,
								  ILClass *callScope, const char *name,
								  ILType **args, int numArgs);

/*
 * Resolve a call to a constructor.
 */
ILMethod *ILResolveConstructor(ILGenInfo *info, ILClass *classInfo,
							   ILClass *callScope, ILType **args, int numArgs);

/*
 * Resolve a call to a unary operator.
 */
ILMethod *ILResolveUnaryOperator(ILGenInfo *info, ILClass *classInfo,
								 const char *name, ILType *argType,
								 ILNode *argNode);

/*
 * Resolve a call to a binary operator.
 */
ILMethod *ILResolveBinaryOperator(ILGenInfo *info, ILClass *classInfo,
								  const char *name, ILType *arg1Type,
								  ILNode *arg1Node, ILType *arg2Type,
								  ILNode *arg2Node);

/*
 * Resolve a call to a conversion operator.
 */
ILMethod *ILResolveConversionOperator(ILGenInfo *info, ILClass *classInfo,
									  const char *name, ILType *fromType,
									  ILType *toType);

/*
 * Resolve a property 
 */

ILProperty *ILResolveProperty(ILGenInfo *info,ILClass *classInfi,
							  ILClass *callScope,const char *name);
#ifdef	__cplusplus
};
#endif

#endif	/* __CODEGEN_CG_RESOLVE_H__ */
