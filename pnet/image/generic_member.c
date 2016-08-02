/*
 * generic_member.c - Functions related to generic class members.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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

#include "program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILMember *ILClassGetMemberInstance(ILClass *owner, ILMember *member);

/*
 * Function for creating members instances and attaching them to a class.
 */
ILMember *ILMemberCreateInstance(ILMember *memberDef,
								 ILClass *info)
{
	ILImage *image = memberDef->programItem.image;
	ILMember *member;
	unsigned size;

	if(ILMember_IsMethod(memberDef))
	{
		size = sizeof(ILMethodInstance);
	}
	else if(ILMember_IsField(memberDef))
	{
		size = sizeof(ILField);
	}
	else if(ILMember_IsProperty(memberDef))
	{
		size = sizeof(ILProperty);
	}
	else if(ILMember_IsEvent(memberDef))
	{
		size = sizeof(ILEvent);
	}
	else if(ILMember_IsOverride(memberDef))
	{
		size = sizeof(ILOverride);
	}
	else if(ILMember_IsPInvoke(memberDef))
	{
		size = sizeof(ILPInvoke);
	}
	else
	{
		/* Unknown */
		return 0;
	}

	/* Allocate space for the member from the memory stack */
	member = (ILMember *)ILMemStackAllocItem(&(image->memStack), size);
	if(!member)
	{
		return 0;
	}

	/* Attach the member to its owning class */	
	member->owner = info;
	member->nextMember = 0;
	if(info->lastMember)
		info->lastMember->nextMember = member;
	else
		info->firstMember = member;
	info->lastMember = member;

	/* Copy the other member fields from the member definition */
	member->name = memberDef->name;
	member->programItem.image = image;
	member->programItem.token = memberDef->programItem.token;
	member->kind = memberDef->kind;
	member->attributes = memberDef->attributes;
	member->signature = 0;
	member->signatureBlob = 0;

	return member;
}


/*
 * Function for creating parameter instances and attaching them to a method.
 */
ILParameter *ILParameterCreateInstance(ILParameter *paramDef,
									   ILMethod *method)
{
	ILImage *image = paramDef->programItem.image;
	ILParameter *param;
	ILParameter *current;
	ILParameter *prev;

	/* Allocate space for the parameter from the memory stack */
	param = ILMemStackAlloc(&(image->memStack), ILParameter);
	if(!param)
	{
		return 0;
	}

	/* Set the parameter fields */
	param->name = paramDef->name;
	param->programItem.image = image;
	param->attributes = paramDef->attributes;
	param->paramNum = paramDef->paramNum;

	/* Attach the parameter to the method */
	current = method->parameters;
	prev = 0;
	while(current != 0 && current->paramNum < param->paramNum)
	{
		prev = current;
		current = current->next;
	}
	param->next = current;
	if(prev)
	{
		prev->next = param;
	}
	else
	{
		method->parameters = param;
	}

	/* Return the parameter to the caller */
	return param;
}

/*
 * Create a method instance and attach it to a class.
 */
ILMethod *ILMethodCreateInstance(ILImage *image,
								 ILMethod *method,
								 ILClass *classInfo,
								 ILType *classTypeArgs,
								 ILType *methodTypeArgs)
{
	ILParameter *origParam;
	ILMethod *newMethod;
	ILType *signature;
	ILMethodInstance *methodInstance;

	signature = ILTypeInstantiate(image->context,
								  ILMethod_Signature(method),
								  classTypeArgs,
								  methodTypeArgs);
	if(!signature)
	{
		return 0;
	}
	newMethod = 0;
	if(ILClassIsExpanded(classInfo))
	{
		newMethod = ILClassLookupMethodInstance(classInfo, ILMethod_Name(method),
												signature, methodTypeArgs);
	}
	if(!newMethod)
	{
		/* Create a new method */
		newMethod = (ILMethod *)ILMemberCreateInstance((ILMember *)method, classInfo);

		if(!newMethod)
		{
			return 0;
		}	

		/* Copy the original method's properties */
		ILMemberSetSignature((ILMember *)newMethod, signature);
		ILMethodSetImplAttrs(newMethod, ~((ILUInt32)0),
							 ILMethod_ImplAttrs(method));
		ILMethodSetCallConv(newMethod, ILMethod_CallConv(method));
		ILMethodSetRVA(newMethod, ILMethod_RVA(method));
		newMethod->callingConventions |= IL_META_CALLCONV_INSTANTIATION;
		newMethod->userData = 0;
		methodInstance = (ILMethodInstance *)newMethod;
		methodInstance->genMethod = method;
		methodInstance->classTypeArguments = classTypeArgs;
		methodInstance->methodTypeArguments = methodTypeArgs;
		
		/* Copy the original method's parameter blocks */
		origParam = 0;
		while((origParam = ILMethodNextParam(method, origParam)) != 0)
		{
			if(!ILParameterCreateInstance(origParam, newMethod))
			{
				return 0;
			}
		}
	}

	return newMethod;
}

static int UpdateMethodVTable(ILMethod *method, ILMethod *virtualAncestor)
{
	ILClass *owner = ILMethod_Owner(method);
	ILImage *image;
	ILMethodVTable *parentVTable;
	ILMethodVTable *vtable;

	if(method->vtable == 0)
	{
		return 0;
	}
	parentVTable = virtualAncestor->vtable;
	if(!parentVTable)
	{
		return 0;
	}
	image = ILProgramItem_Image(method);
	vtable = method->vtable;
	while(vtable->numSlots < parentVTable->numSlots)
	{
		ILMethodInstance *methodInstance = (ILMethodInstance *)ILMethodGetInstance(virtualAncestor, vtable->numSlots);
		ILMethod *newMethod = ILMethodCreateInstance(image,
													 method,
													 owner, 
													 ILClassGetTypeArguments(owner),
													 methodInstance->methodTypeArguments);
		if(!newMethod)
		{
			return 0;
		}
		if(!ILMethodAddInstance(method, newMethod))
		{
			return 0;
		}
	}

	return 1;
}

static int InitVTable(ILMethod *method)
{
	ILImage *image = ILProgramItem_Image(method);

	method->vtable = ILMemStackAlloc(&(image->memStack), ILMethodVTable);
	if(!method->vtable)
	{
		return 0;
	}
	method->vtable->numSlots = 0;
	method->vtable->numItems = 1;
	method->vtable->lastItem = &method->vtable->firstItem;

	return 1;
}

/*
 * Set the method ancestor and initialize the vtable.
 */
int ILMethodSetVirtualAncestor(ILMethod *method, ILMethod *virtualAncestor)
{
	if(!ILMethod_IsVirtualGeneric(method))
	{
		return 1;
	}	
	if(method->vtable == 0)
	{
		if(!InitVTable(method))
		{
			return 0;
		}
	}
	method->vtable->virtualAncestor = virtualAncestor;

	return 1;
}

ILMethod *ILMethodGetInstance(ILMethod *method, int index)
{
	ILMethodVTableItem *item;
	ILMethodVTable	   *vtable;
	ILMethod		   *ancestor;
	int itemIndex;
	int indexInItem;

	if(method->vtable == 0)
	{
		return 0;
	}
	/* Let's find the top virtual ancestor. */
	ancestor = method->vtable->virtualAncestor;
	if(ancestor != 0)
	{
		while(ancestor->vtable->virtualAncestor != 0)
		{
			ancestor = ancestor->vtable->virtualAncestor;
		}

		/* Ensure that the vtable is up-to-date. */
		if(!UpdateMethodVTable(method, ancestor))
		{
			return 0;
		}
	}
	vtable = method->vtable;
	if(index >= vtable->numSlots)
	{
		return 0;
	}
	item = &(vtable->firstItem);
	itemIndex = index / METHOD_VTABLE_ITEM_COUNT;
	indexInItem = index % METHOD_VTABLE_ITEM_COUNT;
	while(itemIndex-- > 0)
	{
		item = item->nextItem;
	}
	
	return item->data[indexInItem];
}

int ILMethodAddInstance(ILMethod *method, ILMethod *methodInstance)
{
	ILImage	 *image = ILProgramItem_Image(method);
	ILMethodVTableItem *item;
	int		indexInItem;
	int		lastItemCount;

	/* The method vtable should have been initialized when laying out the class */
	if(method->vtable == 0)
	{
		return 0;
	}
	lastItemCount = (method->vtable->numItems * METHOD_VTABLE_ITEM_COUNT) - method->vtable->numSlots;
	if(lastItemCount == 0)
	{		
		item = ILMemStackAlloc(&(image->memStack), ILMethodVTableItem);
		if(!item)
		{
			return 0;
		}
		method->vtable->numItems++;
		method->vtable->lastItem->nextItem = item;
		method->vtable->lastItem = item;
	}
	else
	{
		item = method->vtable->lastItem;
	}
	methodInstance->index = method->vtable->numSlots;
	method->vtable->numSlots++;
	indexInItem = methodInstance->index % METHOD_VTABLE_ITEM_COUNT;
	item->data[indexInItem] = methodInstance;

	return 1;
}

ILMember *ILMemberResolveToInstance(ILMember *member, ILMethod *methodCaller)
{
	ILClass *owner;

	owner = ILClassResolveToInstance(member->owner, methodCaller);
	if(owner == 0)
	{
		return 0;
	}
	member = ILMemberResolve(member);
	if(member->owner != owner)
	{
		member = ILClassGetMemberInstance(owner, member);
	}

	return member;
}

int ILMemberIsGenericInstance(ILMember *member)
{
	if(!member->owner)
	{
		return 0;
	}
	if(ILClass_IsGenericInstance(member->owner))
	{
		return 1;
	}

	if(ILMember_IsMethod(member))
	{
		return ((ILMethod_CallConv((ILMethod *)member) & IL_META_CALLCONV_INSTANTIATION) != 0);
	}
	
	return 0;
}

ILMethod *ILMethodSpecToMethod(ILMethodSpec *mspec, ILMethod *methodCaller)
{
	ILType *classTypeArgs = 0;
	ILType *methodTypeArgs = 0;
	ILMethod *newMethod;
	ILMethod *genMethod;
	ILMethod *virtAncestor;
	ILType *mspecTypeArgs;
	ILImage *image;

	if(ILMember_IsGenericInstance((ILMember *)methodCaller))
	{
		ILMethodInstance *methodInstance = (ILMethodInstance *)methodCaller;

		classTypeArgs = methodInstance->classTypeArguments;
		methodTypeArgs = methodInstance->methodTypeArguments;
	}

	image = ILProgramItem_Image(mspec);
	mspecTypeArgs = ILTypeInstantiate(ILImageToContext(image),
				  					  ILMethodSpec_Type(mspec),
									  classTypeArgs,
									  methodTypeArgs);
	if(!mspecTypeArgs)
	{
		return 0;
	}
	genMethod = (ILMethod *)ILMemberResolveToInstance(mspec->method, methodCaller);
	if(!genMethod)
	{
		return 0;
	}
	virtAncestor = genMethod;
	if(ILMethod_IsVirtualGeneric(genMethod))
	{		
		while(virtAncestor->vtable->virtualAncestor != 0)
		{
			virtAncestor = virtAncestor->vtable->virtualAncestor;
		}
	}
	newMethod = ILMethodCreateInstance(image, 
									   virtAncestor,
									   ILMethod_Owner(virtAncestor),
									   classTypeArgs,
									   mspecTypeArgs);
	if(!newMethod)
	{
		return 0;
	}
	if(ILMethod_IsVirtualGeneric(virtAncestor))
	{
		if(!ILMethodAddInstance(virtAncestor, newMethod))
		{
			return 0;
		}
		newMethod = ILMethodGetInstance(genMethod, newMethod->index);
		if(!newMethod)
		{
			return 0;
		}
	}

	return newMethod;
}

ILType *ILMethodGetClassTypeArguments(ILMethod *method)
{
	if(ILMember_IsGenericInstance(method))
	{
		return ((ILMethodInstance*)method)->classTypeArguments;
	}
	else
	{
		return 0;
	}
}

ILType *ILMethodGetMethodTypeArguments(ILMethod *method)
{
	if(ILMember_IsGenericInstance(method))
	{
		return ((ILMethodInstance*)method)->methodTypeArguments;
	}
	else
	{
		return 0;
	}
}

#ifdef	__cplusplus
};
#endif
