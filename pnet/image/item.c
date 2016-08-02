/*
 * item.c - Process program item information from an image file.
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

#include "program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILAttribute *ILProgramItemNextAttribute(ILProgramItem *item,
										ILAttribute *attr)
{
	if(attr)
	{
		return attr->next;
	}
	else if(!(item->linked))
	{
		attr = (ILAttribute *)(item->attrsOrLink);
	}
	else
	{
		attr = ((ILProgramItemLink *)(item->attrsOrLink))->customAttrs;
	}
	if(!attr && item->image->type != IL_IMAGETYPE_BUILDING)
	{
		/* We need to perform on-demand loading of the attributes */
		_ILProgramItemLoadAttributes(item);
		if(!(item->linked))
		{
			attr = (ILAttribute *)(item->attrsOrLink);
		}
		else
		{
			attr = ((ILProgramItemLink *)(item->attrsOrLink))->customAttrs;
		}
	}
	return attr;
}

void ILProgramItemAddAttribute(ILProgramItem *item, ILAttribute *attr)
{
	ILAttribute *temp;
	attr->owner = item;
	attr->next = 0;
	if(!(item->linked))
	{
		/* The item is not linked, so we can add the attribute directly */
		if(item->attrsOrLink != 0)
		{
			temp = (ILAttribute *)(item->attrsOrLink);
			while(temp->next != 0)
			{
				temp = temp->next;
			}
			temp->next = attr;
		}
		else
		{
			item->attrsOrLink = (void *)attr;
		}
	}
	else
	{
		/* The item is linked, so add it to the new position of the list */
		ILProgramItemLink *link = (ILProgramItemLink *)(item->attrsOrLink);
		if(link->customAttrs != 0)
		{
			temp = link->customAttrs;
			while(temp->next != 0)
			{
				temp = temp->next;
			}
			temp->next = attr;
		}
		else
		{
			link->customAttrs = attr;
		}
	}
}

ILAttribute *ILProgramItemRemoveAttribute(ILProgramItem *item,
										  ILAttribute *attr)
{
	ILAttribute *current;

	/* Find the start of the attribute list */
	if(item->linked)
	{
		/* Check if first attribute is the one we are looking for */
		if(((ILProgramItemLink *)(item->attrsOrLink))->customAttrs == attr)
		{
			/* Yes it is. So remove it from the list and return the next one. */
			((ILProgramItemLink *)(item->attrsOrLink))->customAttrs = attr->next;
			return attr->next;
		}
		/* Set the first attribute as staring point */
		current = ((ILProgramItemLink *)(item->attrsOrLink))->customAttrs;
	}
	else
	{
		/* Check if first attribute is the one we are looking for */
		if(((ILAttribute *)(item->attrsOrLink)) == attr)
		{
			/* Yes it is. So remove it from the list and return the next one. */
			item->attrsOrLink = ((void *)attr->next);
			return attr->next;
		}
		/* Set the first attribute as staring point */
		current = (ILAttribute *)(item->attrsOrLink);
	}

	/* Find the attribute and remove it from the list */
	if(current != 0)
	{
		while(current->next != 0 && current->next != attr)
		{
			current = current->next;
		}
		if(current->next == attr)
		{
			current->next = attr->next;
		}
	}

	/* Return the next attribute to the caller */
	return attr->next;
}

unsigned long ILProgramItemNumAttributes(ILProgramItem *item)
{
	unsigned long count = 0;
	ILAttribute *attr = ILProgramItemNextAttribute(item, 0);
	while(attr != 0)
	{
		++count;
		attr = attr->next;
	}
	return count;
}

ILDeclSecurity *ILProgramItemNextDeclSecurity(ILProgramItem *item,
											  ILDeclSecurity *security)
{
	ILToken token;
	ILImage *image;
	ILDeclSecurity *newSecurity;

	if(!security)
	{
		return ILDeclSecurityGetFromOwner(item);
	}
	image = security->ownedItem.programItem.image;
	token = security->ownedItem.programItem.token;
	++token;
	while((newSecurity = ILDeclSecurity_FromToken(image, token)) != 0)
	{
		if(newSecurity->ownedItem.owner == security->ownedItem.owner)
		{
			return newSecurity;
		}
		if(image->type != IL_IMAGETYPE_BUILDING)
		{
			/*
			 * In loaded images the security records have to be sorted on
			 * owner. So if the owner changes we are at the end of the list.
			 */
			return 0;
		}
		++token;
	}
	return 0;
}

ILImage *ILProgramItemGetImage(ILProgramItem *item)
{
	return item->image;
}

ILToken ILProgramItemGetToken(ILProgramItem *item)
{
	return item->token;
}

int _ILProgramItemLink(ILProgramItem *item1, ILProgramItem *item2)
{
	ILProgramItemLink *link;
	ILProgramItemLink *link2;

	/* Bail out if item1 and item2 are identical */
	if(item1 == item2)
	{
		return 1;
	}

	/* Create the link from item1 to item2 */
	if(!(item1->linked))
	{
		/* Create a new link entry for the item */
		link = ILMemStackAlloc(&(item1->image->memStack), ILProgramItemLink);
		if(!link)
		{
			return 0;
		}
		link->customAttrs = (ILAttribute *)(item1->attrsOrLink);
		link->linkedItem = item2;
		link->next = 0;
		item1->attrsOrLink = (void *)link;
		item1->linked = 1;
	}
	else
	{
		/* Remove the current link, if present */
		link = ((ILProgramItemLink *)(item1->attrsOrLink));
		if(link->linkedItem != 0)
		{
			link2 = ((ILProgramItemLink *)(link->linkedItem->attrsOrLink));
			link2 = link2->next;
			while(link2 != 0)
			{
				if(link2->linkedItem == item1)
				{
					link2->linkedItem = 0;
					break;
				}
				link2 = link2->next;
			}
		}

		/* Replace the current link entry for the item */
		link->linkedItem = item2;
	}

	/* Create the reverse link from item2 to item1 */
	if(!(item2->linked))
	{
		/* Create a new link entry for the second item */
		link = ILMemStackAlloc(&(item2->image->memStack), ILProgramItemLink);
		if(!link)
		{
			((ILProgramItemLink *)(item1->attrsOrLink))->linkedItem = 0;
			return 0;
		}
		link->customAttrs = (ILAttribute *)(item2->attrsOrLink);
		link->linkedItem = 0;
		link->next = 0;
		item2->attrsOrLink = (void *)link;
		item2->linked = 1;
	}
	else
	{
		link = (ILProgramItemLink *)(item2->attrsOrLink);
	}
	link2 = link->next;
	while(link2 != 0)
	{
		if(link2->linkedItem == 0)
		{
			link2->linkedItem = item1;
			return 1;
		}
		link = link2;
		link2 = link2->next;
	}
	link2 = ILMemStackAlloc(&(item2->image->memStack), ILProgramItemLink);
	if(!link2)
	{
		((ILProgramItemLink *)(item1->attrsOrLink))->linkedItem = 0;
		return 0;
	}
	link2->customAttrs = 0;
	link2->linkedItem = item1;
	link2->next = 0;
	link->next = link2;
	return 1;
}

void _ILProgramItemUnlink(ILProgramItem *item)
{
	ILProgramItemLink *link;
	ILProgramItemLink *link2;

	/* Bail out if the item is not currently linked to anything */
	if(!(item->linked))
	{
		return;
	}

	/* Remove the link from this item */
	link = (ILProgramItemLink *)(item->attrsOrLink);
	if(link->linkedItem)
	{
		link2 = ((ILProgramItemLink *)(link->linkedItem->attrsOrLink))->next;
		while(link2 != 0)
		{
			if(link2->linkedItem == item)
			{
				link2->linkedItem = 0;
				break;
			}
			link2 = link2->next;
		}
		link->linkedItem = 0;
	}

	/* Remove all links to this item */
	link = link->next;
	while(link != 0)
	{
		link2 = ((ILProgramItemLink *)(link->linkedItem->attrsOrLink));
		link2->linkedItem = 0;
		link->linkedItem = 0;
		link = link->next;
	}
}

ILProgramItem *_ILProgramItemLinkedTo(ILProgramItem *item)
{
	if(item->linked)
	{
		return ((ILProgramItemLink *)(item->attrsOrLink))->linkedItem;
	}
	else
	{
		return 0;
	}
}

ILProgramItem *_ILProgramItemLinkedBackTo(ILProgramItem *item, ILImage *image)
{
	ILProgramItemLink *link;

	/* Bail out if the item is not currently linked to anything */
	if(!(item->linked))
	{
		return 0;
	}

	/* Scan through the links looking for a reverse link to the image */
	link = ((ILProgramItemLink *)(item->attrsOrLink))->next;
	while(link != 0)
	{
		if(link->linkedItem && link->linkedItem->image == image)
		{
			return link->linkedItem;
		}
		link = link->next;
	}

	/* No reverse link to the image */
	return 0;
}

ILProgramItem *_ILProgramItemResolve(ILProgramItem *item)
{
	ILProgramItem *newItem;
	while(item != 0 && item->linked)
	{
		newItem = ((ILProgramItemLink *)(item->attrsOrLink))->linkedItem;
		if(!newItem)
		{
			break;
		}
		item = newItem;
	}
	return item;
}

ILProgramItem *_ILProgramItemResolveRef(ILProgramItem *item)
{
	ILProgramItem *newItem;
	while(item != 0 && item->linked)
	{
		newItem = ((ILProgramItemLink *)(item->attrsOrLink))->linkedItem;
		if(!newItem || item->image != newItem->image)
		{
			break;
		}
		item = newItem;
	}
	return item;
}

ILAttribute *ILProgramItemToAttribute(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) ==
					IL_META_TOKEN_CUSTOM_ATTRIBUTE)
	{
		return (ILAttribute *)item;
	}
	else
	{
		return 0;
	}
}

ILModule *ILProgramItemToModule(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MODULE ||
	        (item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MODULE_REF)
	{
		return (ILModule *)item;
	}
	else
	{
		return 0;
	}
}

ILAssembly *ILProgramItemToAssembly(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_ASSEMBLY ||
	        (item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_ASSEMBLY_REF)
	{
		return (ILAssembly *)item;
	}
	else
	{
		return 0;
	}
}

ILClass *ILProgramItemToClass(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_DEF ||
	        (item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_REF ||
	        (item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_EXPORTED_TYPE)
	{
		return (ILClass *)item;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_SPEC)
	{
		return ILTypeSpecGetClass((ILTypeSpec *)item);
	}
	else
	{
		return 0;
	}
}

ILClass *ILProgramItemToUnderlyingClass(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	switch(item->token & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_TYPE_DEF:
		case IL_META_TOKEN_TYPE_REF:
		case IL_META_TOKEN_EXPORTED_TYPE:
		{
			ILClass *info = (ILClass *)item;

			if(info->synthetic)
			{
				ILType *type;

				type = ILTypeGetWithMain(info->synthetic);
				if(!type)
				{
					return 0;
				}
				type = ILTypeGetWithMain(type);
				if(type)
				{
					return ILType_ToClass(type);
				}
				return 0;
			}
			return info;
		}
		break;

		case IL_META_TOKEN_TYPE_SPEC:
		{
			ILType *type;

			type = _ILTypeSpec_Type((ILTypeSpec *)item);
			if(type)
			{
				type = ILTypeGetWithMain(type);
				if(type)
				{
					return ILType_ToClass(type);
				}
			}
		}
		break;
	}
	return 0;
}

ILProgramItem *ILProgramItemFromType(ILImage *image, ILType *type)
{
	if(!type)
	{
		return 0;
	}
	if(ILType_IsComplex(type))
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_WITH:
			case IL_TYPE_COMPLEX_ARRAY:
			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			case IL_TYPE_COMPLEX_PTR:
			case IL_TYPE_COMPLEX_METHOD:
			case (IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL):
			{
				ILTypeSpec *spec;

				spec = ILTypeSpecCreate(image, 0, type);
				return ILToProgramItem(spec);
			}
		}
	}
	else
	{
		ILClass *info;

		info = ILClassFromType(image, 0, type, 0);
		if(info)
		{
			info = ILClassImport(image, info);
		}
		return ILToProgramItem(info);
	}
	return 0;
}

ILType *ILProgramItemToType(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	switch(item->token & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_TYPE_DEF:
		case IL_META_TOKEN_TYPE_REF:
		case IL_META_TOKEN_EXPORTED_TYPE:
		{
			ILType *type;
			ILClass *info = (ILClass *)item;

			if((type = _ILClass_Synthetic(info)) != 0)
			{
				return type;
			}
			return ILClassToType(info);
		}
		break;

		case IL_META_TOKEN_TYPE_SPEC:
		{
			return _ILTypeSpec_Type((ILTypeSpec *)item);
		}
		break;
	}

	return 0;
}

ILMember *ILProgramItemToMember(ILProgramItem *item)
{
	ILToken tokenType = (item ? (item->token & IL_META_TOKEN_MASK) : 0);
	if(tokenType == IL_META_TOKEN_FIELD_DEF ||
	   tokenType == IL_META_TOKEN_METHOD_DEF ||
	   tokenType == IL_META_TOKEN_EVENT ||
	   tokenType == IL_META_TOKEN_PROPERTY ||
	   tokenType == IL_META_TOKEN_IMPL_MAP ||
	   tokenType == IL_META_TOKEN_MEMBER_REF)
	{
		return (ILMember *)item;
	}
	else
	{
		return 0;
	}
}

ILMethod *ILProgramItemToMethod(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_METHOD_DEF)
	{
		return (ILMethod *)item;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MEMBER_REF)
	{
		if(ILMember_IsMethod(item))
		{
			return (ILMethod *)item;
		}
		else if(ILMember_IsRef(item))
		{
			ILMember *ref = ILMemberResolveRef((ILMember *)item);
			if(ILMember_IsMethod(ref))
			{
				return (ILMethod *)ref;
			}
			else
			{
				return 0;
			}
		}
		else
		{
			return 0;
		}
	}
	else
	{
		return 0;
	}
}

ILParameter *ILProgramItemToParameter(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_PARAM_DEF)
	{
		return (ILParameter *)item;
	}
	else
	{
		return 0;
	}
}

ILField *ILProgramItemToField(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_FIELD_DEF)
	{
		return (ILField *)item;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MEMBER_REF)
	{
		if(ILMember_IsField(item))
		{
			return (ILField *)item;
		}
		else if(ILMember_IsRef(item))
		{
			ILMember *ref = ILMemberResolveRef((ILMember *)item);
			if(ILMember_IsField(ref))
			{
				return (ILField *)ref;
			}
			else
			{
				return 0;
			}
		}
		else
		{
			return 0;
		}
	}
	else
	{
		return 0;
	}
}

ILEvent *ILProgramItemToEvent(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_EVENT)
	{
		return (ILEvent *)item;
	}
	else
	{
		return 0;
	}
}

ILProperty *ILProgramItemToProperty(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_PROPERTY)
	{
		return (ILProperty *)item;
	}
	else
	{
		return 0;
	}
}

ILPInvoke *ILProgramItemToPInvoke(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_IMPL_MAP)
	{
		return (ILPInvoke *)item;
	}
	else
	{
		return 0;
	}
}

ILOverride *ILProgramItemToOverride(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_METHOD_IMPL)
	{
		return (ILOverride *)item;
	}
	else
	{
		return 0;
	}
}

ILEventMap *ILProgramItemToEventMap(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_EVENT_MAP)
	{
		return (ILEventMap *)item;
	}
	else
	{
		return 0;
	}
}

ILPropertyMap *ILProgramItemToPropertyMap(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_PROPERTY_MAP)
	{
		return (ILPropertyMap *)item;
	}
	else
	{
		return 0;
	}
}

ILMethodSem *ILProgramItemToMethodSem(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) ==
				IL_META_TOKEN_METHOD_SEMANTICS)
	{
		return (ILMethodSem *)item;
	}
	else
	{
		return 0;
	}
}

ILOSInfo *ILProgramItemToOSInfo(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_OS_DEF ||
	        (item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_OS_REF)
	{
		return (ILOSInfo *)item;
	}
	else
	{
		return 0;
	}
}

ILProcessorInfo *ILProgramItemToProcessorInfo(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_PROCESSOR_DEF ||
	        (item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_PROCESSOR_REF)
	{
		return (ILProcessorInfo *)item;
	}
	else
	{
		return 0;
	}
}

ILTypeSpec *ILProgramItemToTypeSpec(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_SPEC)
	{
		return (ILTypeSpec *)item;
	}
	else
	{
		return 0;
	}
}

ILStandAloneSig *ILProgramItemToStandAloneSig(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_STAND_ALONE_SIG)
	{
		return (ILStandAloneSig *)item;
	}
	else
	{
		return 0;
	}
}

ILConstant *ILProgramItemToConstant(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_CONSTANT)
	{
		return (ILConstant *)item;
	}
	else
	{
		return 0;
	}
}

ILFieldRVA *ILProgramItemToFieldRVA(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_FIELD_RVA)
	{
		return (ILFieldRVA *)item;
	}
	else
	{
		return 0;
	}
}

ILFieldLayout *ILProgramItemToFieldLayout(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_FIELD_LAYOUT)
	{
		return (ILFieldLayout *)item;
	}
	else
	{
		return 0;
	}
}

ILFieldMarshal *ILProgramItemToFieldMarshal(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_FIELD_MARSHAL)
	{
		return (ILFieldMarshal *)item;
	}
	else
	{
		return 0;
	}
}

ILClassLayout *ILProgramItemToClassLayout(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_CLASS_LAYOUT)
	{
		return (ILClassLayout *)item;
	}
	else
	{
		return 0;
	}
}

ILDeclSecurity *ILProgramItemToDeclSecurity(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_DECL_SECURITY)
	{
		return (ILDeclSecurity *)item;
	}
	else
	{
		return 0;
	}
}

ILFileDecl *ILProgramItemToFileDecl(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_FILE)
	{
		return (ILFileDecl *)item;
	}
	else
	{
		return 0;
	}
}

ILManifestRes *ILProgramItemToManifestRes(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) ==
					IL_META_TOKEN_MANIFEST_RESOURCE)
	{
		return (ILManifestRes *)item;
	}
	else
	{
		return 0;
	}
}

ILExportedType *ILProgramItemToExportedType(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) ==
					IL_META_TOKEN_EXPORTED_TYPE)
	{
		return (ILExportedType *)item;
	}
	else
	{
		return 0;
	}
}

ILGenericPar *ILProgramItemToGenericPar(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_GENERIC_PAR)
	{
		return (ILGenericPar *)item;
	}
	else
	{
		return 0;
	}
}

ILMethodSpec *ILProgramItemToMethodSpec(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_METHOD_SPEC)
	{
		return (ILMethodSpec *)item;
	}
	else
	{
		return 0;
	}
}

ILGenericConstraint *ILProgramItemToGenericConstraint(ILProgramItem *item)
{
	if(!item)
	{
		return 0;
	}
	else if((item->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_GENERIC_CONSTRAINT)
	{
		return (ILGenericConstraint *)item;
	}
	else
	{
		return 0;
	}
}


#ifdef	__cplusplus
};
#endif
