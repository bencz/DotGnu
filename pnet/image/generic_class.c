/*
 * generic_class.c - Functions related to generic class instances.
 *
 * Copyright (C) 2007, 2008  Southern Storm Software, Pty Ltd.
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

ILMethod *ILMethodCreateInstance(ILImage *image,
								 ILMethod *method,
								 ILClass *classInfo,
								 ILType *classTypeArgs,
								 ILType *methodTypeArgs);

ILParameter *ILParameterCreateInstance(ILParameter *paramDef, ILMethod *method);
ILMember *ILMemberCreateInstance(ILMember *memberDef, ILClass *info);

ILClass *ILClassGetUnderlying(ILClass *info)
{
	ILType *synType = info->synthetic;
	if(ILType_IsWith(synType) &&
	   !ILClassIsExpanded(info))
	{
		synType = ILTypeGetWithMain(synType);
		return ILClassFromType(info->programItem.image, 0, synType, 0);
	}
	else
	{
		return info;
	}
}

ILClass *ILClassGetGenericDef(ILClass *info)
{
	ILType *type = info->synthetic;
	if(ILType_IsWith(type))
	{
		return ILType_ToClass(ILTypeGetWithMain(type));
	}
	else
	{
		return 0;
	}
}

/*
 * Expand a class instance.
 */
ILClass *ILClassExpand(ILImage *image, ILClass *classInfo,
					   ILType *classArgs, ILType *methodArgs)
{
	ILType *type = ILClassToType(classInfo);
	return ILClassInstantiate(image, type, classArgs, methodArgs);
}

/*
 * Expand the instantiations in a class.  Returns zero if out of memory.
 */
static int ExpandInstantiations(ILImage *image, ILClass *classInfo,
								ILType *classType, ILType *classParams)
{
	ILClass *origClass;
	ILMember *member;
	ILMethod *newMethod;
	ILField *newField;
	ILEvent *newEvent;
	ILType *signature;
	ILImplements *impl;
	ILClass *tempInfo;

	/* Bail out if not a "with" type, since the instantiation would
	   have already been taken care of by "ILClassFromType" */
	if(!ILType_IsWith(classType))
	{
		return 1;
	}

	/* Find the original class underlying the type */
	origClass = ILClassFromType(image, 0, ILTypeGetWithMain(classType), 0);
	if(!origClass)
	{
		return 0;
	}
	origClass = ILClassResolve(origClass);

	/* Copy across the class attributes */
	ILClassSetAttrs(classInfo, ~((ILUInt32)0), ILClass_Attrs(origClass));
	
	/* Mark this class as being expanded, to deal with circularities */
	classInfo->attributes |= IL_META_TYPEDEF_CLASS_EXPANDED;

	/* Expand the parent class and interfaces */
	if(origClass->parent)
	{
		ILClass *parentClass;

		parentClass = ILClassExpand
			(image, ILClass_ParentClass(origClass), classParams, 0);
		if(!parentClass)
		{
			classInfo->parent = 0;
			return 0;
		}
		classInfo->parent = ILToProgramItem(parentClass);
	}
	impl = _ILClass_Implements(origClass);
	while(impl)
	{
		tempInfo = ILImplements_InterfaceClass(impl);
		tempInfo = ILClassExpand(image, tempInfo, classParams, 0);
		if(!tempInfo)
		{
			return 0;
		}
		if(!ILClassAddImplements(classInfo, ILToProgramItem(tempInfo), 0))
		{
			return 0;
		}
		impl = _ILImplements_NextImplements(impl);
	}

	/* Expand the methods and fields */
	member = 0;
	while((member = ILClassNextMember(origClass, member)) != 0)
	{
		switch(ILMemberGetKind(member))
		{
			case IL_META_MEMBERKIND_METHOD:
			{
				newMethod = ILMethodCreateInstance(image, (ILMethod *)member,
												   classInfo, classParams, 0);

				if(!newMethod)
				{
					return 0;
				}
			}
			break;

			case IL_META_MEMBERKIND_FIELD:
			{
				/* Create a new field */
				newField = (ILField *)ILMemberCreateInstance(member, classInfo);

				if(!newField)
				{
					return 0;
				}

				/* Copy the original field's properties */
				signature = ILTypeInstantiate(image->context,
											  ILMember_Signature(member),
											  classParams, 0);
				if(!signature)
				{
					return 0;
				}
				else
				{
					ILMemberSetSignature((ILMember *)newField, signature);
				}
			}
			break;
			
			case IL_META_MEMBERKIND_EVENT:
			{
				/* Create a new event */
				newEvent = (ILEvent *)ILMemberCreateInstance(member, classInfo);

				if(!newEvent)
				{
					return 0;
				}

				/* Copy the original field's properties */
				signature = ILTypeInstantiate(image->context,
											  ILMember_Signature(member),
											  classParams, 0);
				if(!signature)
				{
					return 0;
				}
				else
				{
					ILMemberSetSignature((ILMember *)newEvent, signature);
				}
			}
			break;

			case IL_META_MEMBERKIND_PROPERTY:
			{
				/* TODO */
			}
			break;

			case IL_META_MEMBERKIND_OVERRIDE:
			{
				/* TODO */
			}
			break;

			case IL_META_MEMBERKIND_PINVOKE:
			{
				/* TODO */
			}
			break;
		}		
	}

	/* Done */
	return 1;
}

ILClass *ILClassInstantiate(ILImage *image, ILType *classType,
							ILType *classArgs, ILType *methodArgs)
{
	ILClass *classInfo;
	ILType *type;

	/* Bail out early if the type does not need instantiation */
	if(!ILType_IsWith(classType) && !ILTypeNeedsInstantiation(classType))
	{
		return ILClassFromType(image, 0, classType, 0);
	}

	/* Search for a synthetic type that matches the expanded
	   form of the class type, in case we already instantiated
	   this class previously.  We do this in such a way that we
	   won't need to call "ILTypeInstantiate" unless necessary */
	classInfo = _ILTypeToSyntheticInstantiation(image, classType, classArgs, methodArgs);
	if(classInfo)
	{
		if((classInfo->attributes & IL_META_TYPEDEF_CLASS_EXPANDED) == 0)
		{
			classArgs = ILClassToType(classInfo);
			if(!ExpandInstantiations(image, classInfo, classType, classArgs))
			{
				return 0;
			}
		}
		return classInfo;
	}

	/* Instantiate the class type */
	type = ILTypeInstantiate(image->context, classType, classArgs, methodArgs);
	if(!type)
	{
		return 0;
	}

	/* Create a synthetic type for the expanded form */
	classInfo = ILClassFromType(image, 0, type, 0);
	if(!classInfo)
	{
		return 0;
	}
	if(!ExpandInstantiations(image, classInfo, type, type))
	{
		return 0;
	}
	return classInfo;
}

ILType *ILClassGetTypeArguments(ILClass *info)
{
	if((info->attributes & IL_META_TYPEDEF_CLASS_EXPANDED) != 0)
	{
		return info->synthetic;
	}
	else
	{
		return 0;
	}
}

int ILClassNeedsExpansion(ILClass *info)
{
	ILType *type = info->synthetic;
	if(ILType_IsWith(type) &&
	  (info->attributes & IL_META_TYPEDEF_CLASS_EXPANDED) == 0)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILClassIsExpanded(ILClass *info)
{
	return ((info->attributes & IL_META_TYPEDEF_CLASS_EXPANDED) != 0);
}

/* FIXME: Make this function more efficient. */
ILMember *ILClassGetMemberInstance(ILClass *owner, ILMember *member)
{
	ILMember *memberInst = owner->firstMember;

	if(!ILClass_IsGenericInstance(owner))
	{
		/* simply return the member if the owner is no generic
		   instantiation */
		return member;
	}

	while(memberInst != 0)
	{
		if(ILProgramItem_Token(memberInst) == ILProgramItem_Token(member))
		{
			return memberInst;
		}
		memberInst = memberInst->nextMember;
	}

	return 0;
}

/* FIXME: Make this function more efficient. */
ILMethod *ILClassLookupMethodInstance(ILClass *owner, const char *name,
									  ILType *signature, ILType  *methodArgs)
{
	ILMember *member = 0;

	while((member = ILClassNextMemberByKind(owner,
										    member,
											IL_META_MEMBERKIND_METHOD)) != 0)
	{
		if(strcmp(member->name, name) != 0)
		{
			continue;
		}
		if(!ILTypeIdentical(member->signature, signature))
		{
			continue;
		}
		if(ILMember_IsGenericInstance(member))
		{
			ILMethodInstance *methodInstance = (ILMethodInstance *)member;

			if (ILTypeIdentical(methodInstance->methodTypeArguments, methodArgs))
			{
				return (ILMethod *)member;
			}
		}
	}

	return 0;
}

ILClass *ILClassResolveToInstance(ILClass *classInfo, ILMethod *methodCaller)
{
	ILType *classTypeArgs = 0;
	ILType *methodTypeArgs = 0;
	ILType *classType;

	classInfo = ILClassResolve(classInfo);
	classType = ILClass_SynType(classInfo);
	if(!classType ||
	   (!ILTypeNeedsInstantiation(classType) &&
	   !ILClassNeedsExpansion(classInfo)))
	{
		return classInfo;
	}
	if(ILMember_IsGenericInstance(methodCaller))
	{
		ILMethodInstance *methodInstance = (ILMethodInstance *)methodCaller;

		classTypeArgs = methodInstance->classTypeArguments;
		methodTypeArgs = methodInstance->methodTypeArguments;
	}
	classInfo = ILClassExpand(ILClassToImage(classInfo), classInfo,
							  classTypeArgs, methodTypeArgs);

	return classInfo;
}

#ifdef	__cplusplus
};
#endif
