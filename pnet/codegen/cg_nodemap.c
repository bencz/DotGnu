/*
 * cg_nodemap.c - Map nodes to and from IL program items.
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

#include "cg_nodes.h"
#include "cg_nodemap.h"

#ifdef	__cplusplus
extern	"C" {
#endif

typedef struct
{
	ILProgramItem	*item;
	ILNode			*node;

} ItemHashElem;

static unsigned long ItemHash_Compute(const void *elem)
{
	unsigned long value = (unsigned long)(((ItemHashElem *)elem)->item);
	return (value ^ (value >> 8));
}

static unsigned long ItemHash_KeyCompute(const void *key)
{
	unsigned long value = (unsigned long)key;
	return (value ^ (value >> 8));
}

static int ItemHash_Match(const void *elem, const void *key)
{
	return (((ItemHashElem *)elem)->item == (ILProgramItem *)key);
}

void ILProgramItemHashCreate(ILGenInfo *info)
{
	info->itemHash = ILHashCreate(0, ItemHash_Compute, ItemHash_KeyCompute,
						          ItemHash_Match, ILFree);
	if(!(info->itemHash))
	{
		ILGenOutOfMemory(info);
	}
}

ILProgramItem *ILNodeToProgramItem(ILNode *node)
{
	if(!node)
	{
		return 0;
	}

	switch(yykind(node))
	{
		case yykindof(ILNode_ClassDefn):
		{
			return (ILProgramItem *)(((ILNode_ClassDefn *)node)->classInfo);
		}
		/* Not reached */

		case yykindof(ILNode_FieldDeclarator):
		{
			return (ILProgramItem *)
				(((ILNode_FieldDeclarator *)node)->fieldInfo);
		}
		/* Not reached */

		case yykindof(ILNode_EventDeclarator):
		{
			return (ILProgramItem *)
				(((ILNode_EventDeclarator *)node)->eventInfo);
		}
		/* Not reached */

		case yykindof(ILNode_MethodDeclaration):
		{
			return (ILProgramItem *)
				(((ILNode_MethodDeclaration *)node)->methodInfo);
		}
		/* Not reached */

		case yykindof(ILNode_PropertyDeclaration):
		{
			return (ILProgramItem *)
				(((ILNode_PropertyDeclaration *)node)->propertyInfo);
		}
		/* Not reached */

		case yykindof(ILNode_EnumMemberDeclaration):
		{
			return (ILProgramItem *)
				(((ILNode_EnumMemberDeclaration *)node)->fieldInfo);
		}
		/* Not reached */
	}

	return 0;
}

ILNode *ILProgramItemToNode(ILGenInfo *info, ILProgramItem *item)
{
	ILClass *classInfo;
	ILMethod *method;
	ItemHashElem *elem;

	/* Check classes and methods, which have user data fields */
	if((classInfo = ILProgramItemToClass(item)) != 0)
	{
		return (ILNode *)ILClassGetUserData(classInfo);
	}
	if((method = ILProgramItemToMethod(item)) != 0)
	{
		return (ILNode *)ILMethodGetUserData(method);
	}

	/* Look up the node mapping hash */
	elem = ILHashFindType(info->itemHash, item, ItemHashElem);
	if(elem)
	{
		return elem->node;
	}

	/* We don't have a node for this program item */
	return 0;
}

void ILSetProgramItemMapping(ILGenInfo *info, ILNode *node)
{
	ILProgramItem *item;
	ILClass *classInfo;
	ILMethod *method;
	ItemHashElem *elem;

	/* Get the item associated with the node */
	item = ILNodeToProgramItem(node);
	if(!item)
	{
		return;
	}

	/* Handle classes and methods as a special case */
	if((classInfo = ILProgramItemToClass(item)) != 0)
	{
		ILClassSetUserData(classInfo, node);
		return;
	}
	if((method = ILProgramItemToMethod(item)) != 0)
	{
		ILMethodSetUserData(method, node);
		return;
	}

	/* Add the item to the node mapping hash */
	if((elem = (ItemHashElem *)ILMalloc(sizeof(ItemHashElem))) == 0)
	{
		ILGenOutOfMemory(info);
	}
	elem->item = item;
	elem->node = node;
	if(!ILHashAdd(info->itemHash, elem))
	{
		ILGenOutOfMemory(info);
	}
}

ILNode *ILEnterProgramItemContext(ILGenInfo *info, ILProgramItem *item,
							      ILScope *globalScope,
								  ILGenItemContext *context)
{
	ILNode *node;
	ILNode_ClassDefn *classNode;
	ILClass *classInfo;
	ILNode *body;

	/* Save the current context */
	context->currentScope = info->currentScope;
	context->currentClass = info->currentClass;
	context->currentNamespace = info->currentNamespace;
#if IL_VERSION_MAJOR > 1
	context->currentTypeFormals = info->currentTypeFormals;
	context->currentMethodFormals = info->currentMethodFormals;
#endif	/* IL_VERSION_MAJOR > 1 */
	context->overflowInsns = info->overflowInsns;
	context->overflowChanged = info->overflowChanged;

	/* Bail out if we don't have a node for the program item */
	node = ILProgramItemToNode(info, item);
	if(!node)
	{
		return 0;
	}

	/* Modify the current context to match the destination node */
	if((classInfo = ILProgramItemToClass(item)) == 0)
	{
		/* This is a class member, so the owning class is the context */
		classInfo = ILMember_Owner((ILMember *)item);
		classNode = (ILNode_ClassDefn *)
			(ILProgramItemToNode(info, ILToProgramItem(classInfo)));
		if(!classNode)
		{
			return 0;
		}
		info->currentClass = (ILNode *)classNode;
		info->currentNamespace =
			((ILNode_ClassDefn *)classNode)->namespaceNode;
	}
	else
	{
		/* This is a class, so its nested parent is the context */
		info->currentClass =
			(ILNode *)(((ILNode_ClassDefn *)node)->nestedParent);
		info->currentNamespace = ((ILNode_ClassDefn *)node)->namespaceNode;
		classNode = (ILNode_ClassDefn *)(info->currentClass);
	}
	if(classNode)
	{
		body = classNode->body;
		if(body && yyisa(body, ILNode_ScopeChange))
		{
			info->currentScope = ((ILNode_ScopeChange *)body)->scope;
		}
		else
		{
			info->currentScope = globalScope;
		}
	#if IL_VERSION_MAJOR > 1
		info->currentTypeFormals = classNode->typeFormals;
		info->currentMethodFormals = 0;
	#endif	/* IL_VERSION_MAJOR > 1 */
	}
	else
	{
		info->currentScope = globalScope;
	#if IL_VERSION_MAJOR > 1
		info->currentTypeFormals = 0;
		info->currentMethodFormals = 0;
	#endif	/* IL_VERSION_MAJOR > 1 */
	}
	info->overflowInsns = info->overflowGlobal;
	info->overflowChanged = 0;

	/* Ready to go */
	return node;
}

void ILLeaveProgramItemContext(ILGenInfo *info, ILGenItemContext *context)
{
	info->currentScope = context->currentScope;
	info->currentClass = context->currentClass;
	info->currentNamespace = context->currentNamespace;
#if IL_VERSION_MAJOR > 1
	info->currentTypeFormals = context->currentTypeFormals;
	info->currentMethodFormals = context->currentMethodFormals;
#endif	/* IL_VERSION_MAJOR > 1 */
	info->overflowInsns = context->overflowInsns;
	info->overflowChanged = context->overflowChanged;
}

#ifdef	__cplusplus
};
#endif
