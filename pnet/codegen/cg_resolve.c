/*
 * cg_resolve.c - Resolve methods, fields, operators, etc.
 *
 * Copyright (C) 2001, 2008  Southern Storm Software, Pty Ltd.
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

#include "cg_nodes.h"
#include "cg_resolve.h"
#include "cg_coerce.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Attributes that are indicate the type of method.
 */
#define	METHOD_TYPE_ATTRS	(IL_META_METHODDEF_STATIC | \
							 IL_META_METHODDEF_SPECIAL_NAME | \
							 IL_META_METHODDEF_RT_SPECIAL_NAME)

/*
 * Internal worker function for locating methods.
 */
static ILMethod *ResolveMethod(ILGenInfo *info, ILClass *classInfo,
							   ILClass *callScope, const char *name,
							   ILType **args, ILNode **nodes, int numArgs,
						       ILType *returnType, ILUInt32 attrs,
							   int normalOrVirtual, int dontInherit,
							   int indirect)
{
	ILMember *member;
	ILMethod *method;
	ILMethod *closestMatch = 0;
	ILType *signature;
	ILType *argType;
	int arg, same;

	while(classInfo != 0)
	{
		classInfo = ILClassResolve(classInfo);
		member = 0;
		while((member = ILClassNextMemberMatch
					(classInfo, member,
					 IL_META_MEMBERKIND_METHOD, name, 0)) != 0)
		{
			/* Filter out members that aren't interesting */
			if((ILMember_Attrs(member) & METHOD_TYPE_ATTRS) != attrs)
			{
				continue;
			}
			method = (ILMethod *)member;
			if(!normalOrVirtual)
			{
				if(ILMethod_IsVirtual(method))
				{
					continue;
				}
			}

			/* Check that this is the signature we are interested in */
			signature = ILMethod_Signature(method);
			if(!ILType_IsMethod(signature))
			{
				continue;
			}
			if(returnType)
			{
				if(!ILTypeIdentical(returnType, ILTypeGetReturn(signature)))
				{
					continue;
				}
			}
			if(numArgs != (int)(ILTypeNumParams(signature)))
			{
				continue;
			}
			same = 1;
			for(arg = 1; arg <= numArgs; ++arg)
			{
				argType = ILTypeGetParam(signature, arg);
				if(!ILTypeIdentical(argType, args[arg - 1]))
				{
					if(nodes)
					{
						/* Allow user-defined conversions when resolving
						   user-defined binary and unary operators */
						if(!ILCanCoerceNodeKind(info, nodes[arg - 1],
												args[arg - 1], argType,
												IL_CONVERT_STANDARD |
													IL_CONVERT_USER_DEFINED,
												indirect))
						{
							break;
						}
					}
					else
					{
						if(!ILCanCoerceKind(info, args[arg - 1], argType,
										    IL_CONVERT_STANDARD, indirect))
						{
							break;
						}
					}
					same = 0;
				}
			}
			if(arg <= numArgs)
			{
				continue;
			}

			/* Check the method's access level against the call scope */
			if(!ILMemberAccessible(member, callScope))
			{
				continue;
			}

			/* We've found a candidate method */
			if(same)
			{
				/* We have an exact match, so return that */
				return method;
			}
			else if(!closestMatch)
			{
				/* This match is close, but try to find a better one */
				closestMatch = method;
			}
		}

		/* Move up to the parent class */
		if(ILClass_IsInterface(classInfo) && !dontInherit)
		{
			/* Scan the parents of this interface */
			ILImplements *impl = 0;
			while((impl = ILClassNextImplements(classInfo, impl)) != 0)
			{
				method = ResolveMethod(info, ILImplements_InterfaceClass(impl),
									   callScope, name, args, nodes, numArgs,
									   returnType, attrs, normalOrVirtual,
									   dontInherit,indirect);
				if(method)
				{
					return method;
				}
			}
			classInfo = 0;
		}
		else
		{
			classInfo = (dontInherit ? 0 : ILClass_ParentClass(classInfo));
		}
	}

	/* Return the closest match if we didn't find an exact match */
	return closestMatch;
}

ILMethod *ILResolveStaticMethod(ILGenInfo *info, ILClass *classInfo,
								ILClass *callScope, const char *name,
								ILType **args, int numArgs)
{
	return ResolveMethod(info, classInfo, callScope, name, args, 0, numArgs, 0,
					     IL_META_METHODDEF_STATIC, 0, 0, 1);
}

ILMethod *ILResolveInstanceMethod(ILGenInfo *info, ILClass *classInfo,
								  ILClass *callScope, const char *name,
								  ILType **args, int numArgs)
{
	return ResolveMethod(info, classInfo, callScope, name,
						 args, 0, numArgs, 0, 0, 1, 0, 1);
}

ILMethod *ILResolveConstructor(ILGenInfo *info, ILClass *classInfo,
							   ILClass *callScope, ILType **args, int numArgs)
{
	return ResolveMethod(info, classInfo, callScope, ".ctor",
						 args, 0, numArgs, ILType_Void,
					     IL_META_METHODDEF_SPECIAL_NAME |
					     IL_META_METHODDEF_RT_SPECIAL_NAME, 0, 1, 1);
}

/*
 * Get the default call scope.
 */
static ILClass *DefaultCallScope(ILGenInfo *info)
{
	if(info->currentClass)
	{
		return ((ILNode_ClassDefn *)(info->currentClass))->classInfo;
	}
	else
	{
		return 0;
	}
}

ILMethod *ILResolveUnaryOperator(ILGenInfo *info, ILClass *classInfo,
							     const char *name, ILType *argType,
								 ILNode *argNode)
{
	ILType *args[1];
	ILNode *nodes[1];
	args[0] = argType;
	nodes[0] = argNode;
	return ResolveMethod(info, classInfo, DefaultCallScope(info),
						 name, args, nodes, 1, 0,
					     IL_META_METHODDEF_STATIC |
					     IL_META_METHODDEF_SPECIAL_NAME, 0, 0, 1);
}

ILMethod *ILResolveBinaryOperator(ILGenInfo *info, ILClass *classInfo,
								  const char *name, ILType *arg1Type,
								  ILNode *arg1Node, ILType *arg2Type,
								  ILNode *arg2Node)
{
	ILType *args[2];
	ILNode *nodes[2];
	args[0] = arg1Type;
	args[1] = arg2Type;
	nodes[0] = arg1Node;
	nodes[1] = arg2Node;
	return ResolveMethod(info, classInfo, DefaultCallScope(info),
						 name, args, nodes, 2, 0,
					     IL_META_METHODDEF_STATIC |
					     IL_META_METHODDEF_SPECIAL_NAME, 0, 0, 1);
}

ILMethod *ILResolveConversionOperator(ILGenInfo *info, ILClass *classInfo,
									  const char *name, ILType *fromType,
									  ILType *toType)
{
	ILType *args[1];
	args[0] = fromType;
	return ResolveMethod(info, classInfo, DefaultCallScope(info),
						 name, args, 0, 1, toType,
					     IL_META_METHODDEF_STATIC |
					     IL_META_METHODDEF_SPECIAL_NAME, 0, 0, 0);
}

#define  PROPERTY_TYPE_ATTRS (IL_META_PROPDEF_SPECIAL_NAME | \
							  IL_META_PROPDEF_RT_SPECIAL_NAME)

/*
 * Internal worker function for locating Properties.
 */
static ILProperty *ResolveProperty(ILGenInfo *info, ILClass *classInfo,
								   ILClass *callScope, const char *name,
								   ILType *type, ILUInt32 attrs,
								   int dontInherit)
{
	ILMember *member;
	ILProperty *property;

	while(classInfo != 0)
	{
		classInfo = ILClassResolve(classInfo);
		member = 0;
		while((member = ILClassNextMemberMatch
					(classInfo, member,
					 IL_META_MEMBERKIND_PROPERTY, name, 0)) != 0)
		{
			/* Filter out members that aren't interesting */
			if((ILMember_Attrs(member) & PROPERTY_TYPE_ATTRS) != attrs)
			{
				continue;
			}
			property = (ILProperty *)member;
			
			if(type)
			{
				if(!ILTypeIdentical(type,ILProperty_Signature(property)))
				{
					continue;
				}
			}

			/* Check the method's access level against the call scope */
			if(!ILMemberAccessible(member, callScope))
			{
				continue;
			}

			/* We've found a candidate property*/
			return property;
		}
		/* Move up to the parent class */
		if(ILClass_IsInterface(classInfo) && !dontInherit)
		{
			/* Scan the parents of this interface */
			ILImplements *impl = 0;
			while((impl = ILClassNextImplements(classInfo, impl)) != 0)
			{
				property = ResolveProperty(info, ILImplements_InterfaceClass(impl),
										   callScope, name, type, attrs,
										   dontInherit);
				if(property)
				{
					return property;
				}
			}
			classInfo = 0;
		}
		else
		{
			classInfo = (dontInherit ? 0 : ILClass_ParentClass(classInfo));
		}
	}

	/* Error !, not found*/
	return NULL;
}

ILProperty *ILResolveProperty(ILGenInfo *info,ILClass *classInfo,
							  ILClass *callScope,const char *name)
{
	return ResolveProperty(info,classInfo,callScope,name,0,0,0);
}

#ifdef	__cplusplus
};
#endif
