/*
 * doc_sort.c - Sort a documentation tree.
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
 * Helper function for sorting an array of pointers.
 */
static void DocSort(void *base, unsigned long numElements, unsigned size,
					int (*compareFunc)(const void *, const void *))
{
#ifdef HAVE_QSORT
	qsort(base, numElements, size, compareFunc);
#else
	unsigned long posn;
	unsigned long posn2;
	void **temp;
	for(posn = 0; posn < (numElements - 1); ++posn)
	{
		for(posn2 = posn + 1; posn2 < numElements; ++posn2)
		{
			if((*compareFunc)(((unsigned char *)base) + posn * size,
							  ((unsigned char *)base) + posn2 * size) > 0)
			{
				temp = ((void **)base)[posn];
				((void **)base)[posn] = ((void **)base)[posn2];
				((void **)base)[posn2] = temp;
			}
		}
	}
#endif
}

/*
 * Extract a type's namespace.
 */
static void ExtractNamespace(ILDocType *type,
							 char **namespace,
							 int *namespaceLen)
{
	if(type->fullName)
	{
		int len = strlen(type->fullName);

		if(type->name && (*(type->name) != '\0'))
		{
			int nameLen = strlen(type->name);

			if((len > nameLen) && (type->fullName[len - nameLen - 1] == '.'))
			{
				*namespace = type->fullName;
				*namespaceLen = len - nameLen - 1;
				return;
			}
		}
		while(len > 0 && type->fullName[len - 1] != '.')
		{
			--len;
		}
		if(len > 0)
		{
			--len;
		}
		*namespace = type->fullName;
		*namespaceLen = len;
	}
	else
	{
		*namespace = 0;
		*namespaceLen = 0;
	}
}

/*
 * Compare two types based on their full name.
 */
static int TypeNameCompare(const void *e1, const void *e2)
{
	ILDocType *type1 = *((ILDocType **)e1);
	ILDocType *type2 = *((ILDocType **)e2);
	char *namespace1;
	int namespace1Len;
	char *namespace2;
	int namespace2Len;
	if(!(type1->fullName))
	{
		if(type2->fullName)
		{
			return -1;
		}
		else
		{
			return 0;
		}
	}
	else if(!(type2->fullName))
	{
		return 1;
	}
	else
	{
		ExtractNamespace(type1, &namespace1, &namespace1Len);
		ExtractNamespace(type2, &namespace2, &namespace2Len);
		while(namespace1Len > 0 && namespace2Len > 0)
		{
			if(*namespace1 < *namespace2)
			{
				return -1;
			}
			else if(*namespace1 > *namespace2)
			{
				return 1;
			}
			++namespace1;
			++namespace2;
			--namespace1Len;
			--namespace2Len;
		}
		if(namespace1Len > 0)
		{
			return 1;
		}
		else if(namespace2Len > 0)
		{
			return -1;
		}
		return strcmp(type1->name, type2->name);
	}
}

/*
 * Compare two types based on their shortened base name.
 */
static int BaseNameCompare(const void *e1, const void *e2)
{
	ILDocType *type1 = *((ILDocType **)e1);
	ILDocType *type2 = *((ILDocType **)e2);
	if(!(type1->name))
	{
		if(type2->name)
		{
			return -1;
		}
		else
		{
			return 0;
		}
	}
	else if(!(type2->name))
	{
		return 1;
	}
	else
	{
		return strcmp(type1->name, type2->name);
	}
}

/*
 * Compare two members based on their kind and name.
 */
static int MemberNameCompare(const void *e1, const void *e2)
{
	ILDocMember *member1 = *((ILDocMember **)e1);
	ILDocMember *member2 = *((ILDocMember **)e2);
	if(member1->memberType < member2->memberType)
	{
		return -1;
	}
	else if(member1->memberType > member2->memberType)
	{
		return 1;
	}
	if(!(member1->name))
	{
		if(member2->name)
		{
			return -1;
		}
		else
		{
			return 0;
		}
	}
	else if(!(member2->name))
	{
		return 1;
	}
	else
	{
		return strcmp(member1->name, member2->name);
	}
}

/*
 * Compare two namespaces for equality.
 */
static int SameNamespace(char *namespace1, int namespace1Len,
						 char *namespace2, int namespace2Len)
{
	if(namespace1Len == namespace2Len &&
	   !ILMemCmp(namespace1, namespace2, namespace1Len))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILDocTreeSort(ILDocTree *tree)
{
	unsigned long numTypes;
	unsigned long index;
	unsigned long numMembers;
	unsigned long index2;
	ILDocLibrary *library;
	ILDocType *type;
	ILDocType **typeArray;
	char *prevNamespace;
	int prevNamespaceLen;
	char *namespace;
	int namespaceLen;
	ILDocNamespace *namespaceNode;
	ILDocNamespace *lastNamespaceNode;
	ILDocType *lastInNamespace;
	ILDocMember *member;
	ILDocMember **memberArray;

	/* Count the number of types in the tree */
	numTypes = 0;
	library = tree->libraries;
	while(library != 0)
	{
		type = library->types;
		while(type != 0)
		{
			++numTypes;
			type = type->next;
		}
		library = library->next;
	}

	/* Bail out if no types */
	if(!numTypes)
	{
		return 1;
	}

	/* Build an array containing all types */
	typeArray = (ILDocType **)ILMalloc(sizeof(ILDocType *) * numTypes);
	if(!typeArray)
	{
		return 0;
	}
	index = 0;
	library = tree->libraries;
	while(library != 0)
	{
		type = library->types;
		while(type != 0)
		{
			typeArray[index] = type;
			++index;
			type = type->next;
		}
		library = library->next;
	}

	/* Sort the array on namespace and name */
	DocSort(typeArray, numTypes, sizeof(ILDocType *), TypeNameCompare);

	/* Build the namespace tree, with all types in their correct order */
	prevNamespace = 0;
	prevNamespaceLen = -1;
	namespaceNode = 0;
	lastInNamespace = 0;
	for(index = 0; index < numTypes; ++index)
	{
		/* Extract the namespace from the current type */
		ExtractNamespace(typeArray[index], &namespace, &namespaceLen);

		/* Do we need to create a new namespace node? */
		if(!SameNamespace(prevNamespace, prevNamespaceLen,
						  namespace, namespaceLen))
		{
			lastNamespaceNode = namespaceNode;
			namespaceNode = (ILDocNamespace *)ILMalloc(sizeof(ILDocNamespace));
			if(!namespaceNode)
			{
				ILFree(typeArray);
				return 0;
			}
			namespaceNode->tree = tree;
			namespaceNode->name = (char *)ILMalloc(namespaceLen + 1);
			if(!(namespaceNode->name))
			{
				ILFree(namespaceNode);
				ILFree(typeArray);
				return 0;
			}
			if(namespaceLen > 0)
			{
				ILMemCpy(namespaceNode->name, namespace, namespaceLen);
			}
			namespaceNode->name[namespaceLen] = '\0';
			namespaceNode->types = 0;
			namespaceNode->next = 0;
			if(lastNamespaceNode)
			{
				lastNamespaceNode->next = namespaceNode;
			}
			else
			{
				tree->namespaces = namespaceNode;
			}
			lastInNamespace = 0;
		}

		/* Attach the type to the current namespace node */
		if(lastInNamespace)
		{
			lastInNamespace->nextNamespace = typeArray[index];
		}
		else
		{
			namespaceNode->types = typeArray[index];
		}
		lastInNamespace = typeArray[index];
		typeArray[index]->namespace = namespaceNode;

		/* Record the namespace name for the next iteration */
		prevNamespace = namespace;
		prevNamespaceLen = namespaceLen;
	}

	/* Sort the members within each type */
	for(index = 0; index < numTypes; ++index)
	{
		/* Count the number of members */
		numMembers = 0;
		member = typeArray[index]->members;
		while(member != 0)
		{
			++numMembers;
			member = member->next;
		}

		/* Skip the type if it has zero or one members */
		if(numMembers <= 1)
		{
			continue;
		}

		/* Allocate an array to hold all of the member pointers */
		memberArray = (ILDocMember **)ILMalloc
							(sizeof(ILDocMember *) * numMembers);
		if(!memberArray)
		{
			ILFree(typeArray);
			return 0;
		}

		/* Copy the member pointers into the array */
		index2 = 0;
		member = typeArray[index]->members;
		while(member != 0)
		{
			memberArray[index2] = member;
			++index2;
			member = member->next;
		}

		/* Sort the member array on kind and then name */
		DocSort(memberArray, numMembers, sizeof(ILDocMember *),
				MemberNameCompare);

		/* Build a new member list based on the sort order */
		typeArray[index]->members = memberArray[0];
		for(index2 = 1; index2 < numMembers; ++index2)
		{
			memberArray[index2 - 1]->next = memberArray[index2];
			if(MemberNameCompare(&(memberArray[index2 - 1]),
								 &(memberArray[index2])) == 0)
			{
				/* We have duplicate names, so fully qualify the member */
				memberArray[index2 - 1]->fullyQualify = 1;
				memberArray[index2]->fullyQualify = 1;
			}
		}
		memberArray[numMembers - 1]->next = 0;

		/* Free the member array, which we no longer require */
		ILFree(memberArray);
	}

	/* Determine if there are types in different namespaces
	   with the same base name.  If so, we must fully qualify
	   the name in the output documentation format */
	DocSort(typeArray, numTypes, sizeof(ILDocType *), BaseNameCompare);
	for(index = 1; index < numTypes; ++index)
	{
		if(!BaseNameCompare(&(typeArray[index - 1]), &(typeArray[index])))
		{
			typeArray[index - 1]->fullyQualify = 1;
			typeArray[index]->fullyQualify = 1;
		}
	}

	/* Clean up and exit */
	ILFree(typeArray);
	return 1;
}

#ifdef	__cplusplus
};
#endif
