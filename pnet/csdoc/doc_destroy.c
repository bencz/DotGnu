/*
 * doc_destroy.c - Destroy a documentation tree.
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

#include "doc_tree.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Helper macro for destroying the contents of a linked list.
 */
#define	DESTROY_LIST(type,list,func)	\
			do { \
				type *current, *next; \
				current = (list); \
				while(current != 0) \
				{ \
					(*func)(current); \
					next = current->next; \
					ILFree(current); \
					current = next; \
				} \
			} while (0)

/*
 * Destroy an interface.
 */
static void DestroyInterface(ILDocInterface *interface)
{
	if(interface->name)
	{
		ILFree(interface->name);
	}
}

/*
 * Destroy an attribute.
 */
static void DestroyAttribute(ILDocAttribute *attribute)
{
	if(attribute->name)
	{
		ILFree(attribute->name);
	}
}

/*
 * Destroy a documentation object.
 */
static void DestroyDoc(ILDocText *doc)
{
	if(doc->children)
	{
		DESTROY_LIST(ILDocText, doc->children, DestroyDoc);
	}
}

/*
 * Destroy a parameter.
 */
static void DestroyParameter(ILDocParameter *parameter)
{
	if(parameter->name)
	{
		ILFree(parameter->name);
	}
	if(parameter->type)
	{
		ILFree(parameter->type);
	}
}

/*
 * Destroy a member.
 */
static void DestroyMember(ILDocMember *member)
{
	if(member->name)
	{
		ILFree(member->name);
	}
	if(member->ilasmSignature)
	{
		ILFree(member->ilasmSignature);
	}
	if(member->csSignature)
	{
		ILFree(member->csSignature);
	}
	if(member->returnType)
	{
		ILFree(member->returnType);
	}
	DESTROY_LIST(ILDocParameter, member->parameters, DestroyParameter);
	DESTROY_LIST(ILDocAttribute, member->attributes, DestroyAttribute);
	DESTROY_LIST(ILDocText, member->doc, DestroyDoc);
}

/*
 * Destroy a type.
 */
static void DestroyType(ILDocType *type)
{
	if(type->name)
	{
		ILFree(type->name);
	}
	if(type->fullName)
	{
		ILFree(type->fullName);
	}
	if(type->assembly)
	{
		ILFree(type->assembly);
	}
	if(type->ilasmSignature)
	{
		ILFree(type->ilasmSignature);
	}
	if(type->csSignature)
	{
		ILFree(type->csSignature);
	}
	if(type->baseType)
	{
		ILFree(type->baseType);
	}
	if(type->excludedBaseType)
	{
		ILFree(type->excludedBaseType);
	}
	DESTROY_LIST(ILDocInterface, type->interfaces, DestroyInterface);
	DESTROY_LIST(ILDocAttribute, type->attributes, DestroyAttribute);
	DESTROY_LIST(ILDocText, type->doc, DestroyDoc);
	DESTROY_LIST(ILDocMember, type->members, DestroyMember);
}

/*
 * Destroy a library.
 */
static void DestroyLibrary(ILDocLibrary *library)
{
	if(library->name)
	{
		ILFree(library->name);
	}
	DESTROY_LIST(ILDocType, library->types, DestroyType);
}

void ILDocTreeDestroy(ILDocTree *tree)
{
	if(tree)
	{
		DESTROY_LIST(ILDocLibrary, tree->libraries, DestroyLibrary);
		ILFree(tree);
	}
}

#ifdef	__cplusplus
};
#endif
