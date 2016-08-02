/*
 * java_lookup.c - Lookup routines for the scope and member resolution 
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Gopal.V
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
#include "java_internal.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Extra member kinds.
 */
#define	JAVA_MEMBERKIND_TYPE			20
#define	JAVA_MEMBERKIND_TYPE_NODE		21
#define	JAVA_MEMBERKIND_NAMESPACE		22

/*
 * A list of members that results from a lookup on a type.
 */
typedef struct _tagJavaMemberInfo JavaMemberInfo;
struct _tagJavaMemberInfo
{
	ILProgramItem *member;
	ILClass       *owner;
	short		   kind;
	short		   form;
	JavaMemberInfo  *next;
};
#define	JAVA_MEMBER_LOOKUP_MAX	4
typedef struct _tagJavaMemberLookupInfo JavaMemberLookupInfo;
struct _tagJavaMemberLookupInfo
{
	int			   num;
	JavaMemberInfo  *members;
	JavaMemberInfo  *lastMember;

};

/*
 * Iterator control structure for JavaMemberLookupInfo.
 */
typedef struct
{
	JavaMemberLookupInfo *info;
	JavaMemberInfo       *current;
	JavaMemberInfo       *last;

} JavaMemberLookupIter;

/*
 * Initialize a member results set.
 */
static void InitMembers(JavaMemberLookupInfo *results)
{
	results->num = 0;
	results->members = 0;
	results->lastMember = 0;
}

/*
 * Add a member to a results set.
 */
static void AddMember(JavaMemberLookupInfo *results,
					  ILProgramItem *member, ILClass *owner,
					  int kind)
{
	JavaMemberInfo *info = (JavaMemberInfo *)ILMalloc(sizeof(JavaMemberInfo));
	if(!info)
	{
		CCOutOfMemory();
	}
	info->member = member;
	info->owner = owner;
	info->kind = kind;
	info->form = 0;
	info->next = 0;
	if(results->lastMember)
	{
		results->lastMember->next = info;
	}
	else
	{
		results->members = info;
	}
	results->lastMember = info;
	++(results->num);
}

/*
 * Free the contents of a member lookup results list.
 */
static void FreeMembers(JavaMemberLookupInfo *results)
{
	JavaMemberInfo *info, *next;
	info = results->members;
	while(info != 0)
	{
		next = info->next;
		ILFree(info);
		info = next;
	}
	results->num = 0;
	results->members = 0;
	results->lastMember = 0;
}

/*
 * Initialize a member iterator.
 */
static void MemberIterInit(JavaMemberLookupIter *iter,
						   JavaMemberLookupInfo *results)
{
	iter->info = results;
	iter->current = 0;
	iter->last = 0;
}

/*
 * Get the next item from a member iterator.
 */
static JavaMemberInfo *MemberIterNext(JavaMemberLookupIter *iter)
{
	if(iter->current)
	{
		iter->last = iter->current;
		iter->current = iter->current->next;
	}
	else
	{
		iter->current = iter->info->members;
		iter->last = 0;
	}
	return iter->current;
}

/*
 * Remove the current item from a member iterator.
 */
static void MemberIterRemove(JavaMemberLookupIter *iter)
{
	if(iter->current == iter->info->lastMember)
	{
		iter->info->lastMember = iter->last;
	}
	if(iter->last)
	{
		iter->last->next = iter->current->next;
		ILFree(iter->current);
		iter->current = iter->last;
	}
	else
	{
		iter->info->members = iter->current->next;
		ILFree(iter->current);
		iter->current = 0;
		iter->last = 0;
	}
	--(iter->info->num);
}

int JavaIsBaseTypeFor(ILClass *info1, ILClass *info2)
{
	if(ILClassResolve(info1) == ILClassResolve(info2))
	{
		return 0;
	}
	if(ILClassInheritsFrom(info2, info1))
	{
		return 1;
	}
	if(ILClassImplements(info2, info1))
	{
		return 1;
	}
	return 0;
}

int JavaSignatureIdentical(ILType *sig1, ILType *sig2)
{
	unsigned long numParams;
	unsigned long paramNum;

	/* Check the number of parameters */
	numParams = ILTypeNumParams(sig1);
	if(numParams != ILTypeNumParams(sig2))
	{
		return 0;
	}

	/* Check each parameter for identity */
	for(paramNum = 1; paramNum <= numParams; ++paramNum)
	{
		if(!ILTypeIdentical(ILTypeGetParam(sig1, paramNum),
							ILTypeGetParam(sig2, paramNum)))
		{
			return 0;
		}
	}

	/* The signatures are identical */
	return 1;
}

/*
 * Trim a list of members to remove unneeded elements.
 */
static int TrimMemberList(JavaMemberLookupInfo *results, int isIndexerList)
{
	JavaMemberLookupIter iter;
	JavaMemberInfo *firstMember;
	JavaMemberInfo *member;
	JavaMemberInfo *testMember;
	JavaMemberInfo *tempMember;
	JavaMemberInfo *prevMember;

	/* If the list is empty, then we are done */
	if(!(results->num))
	{
		return JAVA_SEMKIND_VOID;
	}

	/* Trim the list based on the type of the first member */
	firstMember = results->members;
	if(firstMember->kind == IL_META_MEMBERKIND_METHOD || isIndexerList)
	{
		/* Remove non-methods from the base types */
		if(!isIndexerList)
		{
			MemberIterInit(&iter, results);
			while((member = MemberIterNext(&iter)) != 0)
			{
				if(member->kind != IL_META_MEMBERKIND_METHOD &&
				   member->owner != firstMember->owner)
				{
					MemberIterRemove(&iter);
				}
			}
		}

		/* Filter the remaining members by signature */
		MemberIterInit(&iter, results);
		while((member = MemberIterNext(&iter)) != 0)
		{
			testMember = member->next;
			prevMember = member;
			while(testMember != 0)
			{
				if(JavaIsBaseTypeFor(testMember->owner, member->owner))
				{
					/* "testMember" is in a base type of "member"'s type */
					if(JavaSignatureIdentical
							(ILMember_Signature(testMember->member),
						     ILMember_Signature(member->member)))
					{
						/* Remove "testMember" from the method group */
						tempMember = testMember->next;
						prevMember->next = tempMember;
						ILFree(testMember);
						testMember = tempMember;
						--(results->num);
						continue;
					}
				}
				else if(JavaIsBaseTypeFor(member->owner, testMember->owner))
				{
					/* "member" is in a base type of "testMember"'s type */
					if(JavaSignatureIdentical
							(ILMember_Signature(testMember->member),
						     ILMember_Signature(member->member)))
					{
						/* Remove "member" from the method group */
						MemberIterRemove(&iter);
						break;
					}
				}
				else if(testMember->member == member->member)
				{
					/* We picked up two copies of the same member,
					   which can happen when scanning base interfaces
					   along multiple inheritance paths */
					MemberIterRemove(&iter);
					break;
				}
				prevMember = testMember;
				testMember = testMember->next;
			}
		}

		/* The previous "first member" may have been removed, so reacqurie */
		firstMember = results->members;
	}
	else
	{
		/* This is not a method, so remove members from the base types */
		MemberIterInit(&iter, results);
		while((member = MemberIterNext(&iter)) != 0)
		{
			if(member->owner != firstMember->owner)
			{
				MemberIterRemove(&iter);
			}
		}
	}

	/* Determine whether we have a method list, a non-method,
	   or a list of members which is ambiguous */
	if(firstMember->kind == IL_META_MEMBERKIND_METHOD)
	{
		/* All members must be methods, or the list is ambiguous */
		MemberIterInit(&iter, results);
		while((member = MemberIterNext(&iter)) != 0)
		{
			if(member->kind != IL_META_MEMBERKIND_METHOD)
			{
				return JAVA_SEMKIND_AMBIGUOUS;
			}
		}
		return JAVA_SEMKIND_METHOD_GROUP;
	}
	else if(results->num == 1)
	{
		/* Singleton list with a non-method */
		switch(firstMember->kind)
		{
			case JAVA_MEMBERKIND_TYPE:
			{
				return JAVA_SEMKIND_TYPE;
			}
			/* Not reached */

			case IL_META_MEMBERKIND_FIELD:
			{
				return JAVA_SEMKIND_FIELD;
			}
			/* Not reached */

		}
		return JAVA_SEMKIND_AMBIGUOUS;
	}
	else
	{
		/* The list is ambiguous */
		return JAVA_SEMKIND_AMBIGUOUS;
	}
}
/*
 * Convert a set of member lookup results into a semantic value.
 */
static JavaSemValue LookupToSem(ILNode *node, const char *name,
							  JavaMemberLookupInfo *results, int kind)
{
	JavaSemValue value;
	if(kind == JAVA_SEMKIND_METHOD_GROUP)
	{
		/* This is a method group.  Fix later: this will leak memory! */
		JavaSemSetMethodGroup(value, results->members);
		return value;
	}
	else if(kind == JAVA_SEMKIND_TYPE)
	{
		/* This is a type */
		JavaSemSetType
			(value, ILClassToType((ILClass *)(results->members->member)));
		FreeMembers(results);
		return value;
	}
	else if(kind != JAVA_SEMKIND_AMBIGUOUS)
	{
		/* Type node, field, property, or event */
		JavaSemSetKind(value, kind, results->members->member);
		FreeMembers(results);
		return value;
	}
	else
	{
		/* Ambiguous lookup */
	//	AmbiguousError(node, name, results);
		FreeMembers(results);
		return JavaSemValueDefault;
	}
}

/*
 * Find a nested class by name within an attached scope.
 * This is needed during type gathering, prior to the
 * creation of the "ILClass" records.
 */
static ILNode *FindNestedClass(ILClass *info, ILNode_ClassDefn *defn,
							   const char *name)
{
	ILNode *body;
	ILScope *scope;
	ILScopeData *data;

	/* Get the tree node that is attached to the class */
	if(!defn)
	{
		defn = (ILNode_ClassDefn *)ILClassGetUserData(info);
		if(!defn)
		{
			return 0;
		}
	}

	/* Get the scope information from the class body */
	body = defn->body;
	if(!body || !yyisa(body, ILNode_ScopeChange))
	{
		return 0;
	}
	scope = ((ILNode_ScopeChange *)body)->scope;

	/* Look for the name within the scope */
	data = ILScopeLookup(scope, name, 0);
	if(!data)
	{
		return 0;
	}

	/* The item must be a declared type */
	if(ILScopeDataGetKind(data) != IL_SCOPE_DECLARED_TYPE)
	{
		return 0;
	}
	body = ILScopeDataGetNode(data);
	if(!body || !yyisa(body, ILNode_ClassDefn))
	{
		return 0;
	}

	/* If the "classInfo" field is already set, then we should
	   fall back to the normal nested class processing as we
	   are no longer within type gathering, or we've already
	   gathered this type previously */
	defn = (ILNode_ClassDefn *)body;
	if(defn->classInfo && defn->classInfo != (ILClass *)1 &&
	   defn->classInfo != (ILClass *)2)
	{
		return 0;
	}

	/* Return the nested class node to the caller */
	return body;
}

/*
 * Process a class and add all members called "name" to a set
 * of member lookup results.
 */
static void FindMembers(ILGenInfo *genInfo, ILClass *info,
						const char *name, ILClass *accessedFrom,
					    JavaMemberLookupInfo *results,
						int lookInParents, int baseAccess, int literalType)
{
	ILImplements *impl;
	ILMember *member;
	ILNestedInfo *nested;
	ILClass *nestedChild;
	int kind;
	ILMethod *underlying;
	ILNode *node;
	ILType *objectType=ILFindSystemType(genInfo,"Object");

	/* Scan up the parent hierarchy until we run out of parents */
	while(info != 0)
	{
		/* Resolve the class to its actual image */
		info = ILClassResolve(info);

		/* Look for all accessible members with the given name */
		if(!(genInfo->inSemType))
		{
			member = 0;
			while((member = ILClassNextMemberMatch
						(info, member, 0, name, 0)) != 0)
			{
				if(ILMemberAccessible(member, accessedFrom))
				{
					kind = ILMemberGetKind(member);
					if(literalType && kind != JAVA_MEMBERKIND_TYPE)
					{
						/* In literal type mode, want only types */
						continue;
					}
					if(kind != IL_META_MEMBERKIND_METHOD &&
					   kind != IL_META_MEMBERKIND_FIELD)
					{
						/* This is PInvoke or override, property or event , 
						 * which we don't need */
						continue;
					}
					if(kind == IL_META_MEMBERKIND_METHOD)
					{
						underlying=(ILMethod*)member;
					   	if(ILMethod_IsVirtual(underlying) &&
						   !ILMethod_IsNewSlot(underlying))
						{
							/* This is a virtual override: skip it if we
							   aren't looking for a "base" member */
							if(!baseAccess)
							{
								continue;
							}
						}
					}
					AddMember(results, (ILProgramItem *)member, info, kind);
				}
			}
		}

		/* Look for all accessible nested classes with the given name */
		node = FindNestedClass(info, 0, name);
		if(node)
		{
			AddMember(results, (ILProgramItem *)node,
					  info, JAVA_MEMBERKIND_TYPE_NODE);
		}
		else
		{
			nested = 0;
			while((nested = ILClassNextNested(info, nested)) != 0)
			{
				nestedChild = ILNestedInfoGetChild(nested);
				if(!strcmp(ILClass_Name(nestedChild), name) &&
				   ILClassAccessible(nestedChild, accessedFrom))
				{
					AddMember(results, (ILProgramItem *)nestedChild,
							  info, JAVA_MEMBERKIND_TYPE);
				}
			}
		}

		/* If this is an interface, then scan its base interfaces */
		if(ILClass_IsInterface(info))
		{
			/* also scan the 'Object' class , as all interface instances
			 * have Objects behind them !!
			 */
			FindMembers(genInfo, ILTypeToClass(genInfo,objectType),
					    name, accessedFrom, results,
						0, baseAccess, literalType);

			impl = 0;
			while((impl = ILClassNextImplements(info, impl)) != 0)
			{
				FindMembers(genInfo, ILImplements_InterfaceClass(impl),
						    name, accessedFrom, results,
							lookInParents, baseAccess, literalType);
			}
		}

		/* Move up to the parent */
		info = (lookInParents ? ILClass_ParentRef(info) : 0);
	}
}

/*
 * Perform a member lookup on a type.
 */

static int MemberLookup(ILGenInfo *genInfo, ILClass *info, 
						const char *name,
				        ILClass *accessedFrom, JavaMemberLookupInfo *results,
						int lookInParents, int baseAccess, int literalType)
{
	/* Initialize the results */
	InitMembers(results);

	/* Collect up all members with the specified name */
	if(info)
	{
		FindMembers(genInfo, info, name, accessedFrom, results,
					lookInParents, baseAccess, literalType);
	}

	/* Trim the list and determine the kind for the result */
	return TrimMemberList(results, 0);
}

/*
 * Find the type with a specific name within a namespace.
 */
static int FindTypeInNamespace(ILGenInfo *genInfo, const char *name,
							   const char *namespace, ILClass *accessedFrom,
							   JavaMemberLookupInfo *results)
{
	ILClass *type;
	ILScopeData *data;
	int scopeKind;
	const char *fullName;
	ILNode_ClassDefn *node;

	/* Look in the current image for the type */
	type = ILClassLookup(ILClassGlobalScope(genInfo->image),
						 name, namespace);
	if(type)
	{
		type = ILClassResolve(type);
	}
	if(type && ILClassAccessible(type, accessedFrom))
	{
		AddMember(results, (ILProgramItem *)type, 0, JAVA_MEMBERKIND_TYPE);
		return JAVA_SEMKIND_TYPE;
	}

	/* Look in the global scope for a declared type */
	data = ILScopeLookupInNamespace(CCCodeGen.globalScope, namespace, name);
	if(data)
	{
		scopeKind = ILScopeDataGetKind(data);
		if(scopeKind == IL_SCOPE_SUBSCOPE)
		{
			if(namespace)
			{
				fullName = ILInternStringConcat3
								(ILInternString(namespace, -1),
								 ILInternString(".", 1),
								 ILInternString((char *)name, -1)).string;
			}
			else
			{
				fullName = (char *)name;
			}
			AddMember(results, (ILProgramItem *)fullName,
					  0, JAVA_MEMBERKIND_NAMESPACE);
			return JAVA_SEMKIND_NAMESPACE;
		}
		else if(scopeKind == IL_SCOPE_DECLARED_TYPE)
		{
			node = (ILNode_ClassDefn *)(ILScopeDataGetNode(data));
			if(!(genInfo->typeGather))
			{
				if(node->classInfo != 0 &&
				   node->classInfo != ((ILClass *)1) &&
				   node->classInfo != ((ILClass *)2))
				{
					AddMember(results, (ILProgramItem *)(node->classInfo),
						      0, JAVA_MEMBERKIND_TYPE);
					return JAVA_SEMKIND_TYPE;
				}
			}
			AddMember(results, (ILProgramItem *)node,
				      0, JAVA_MEMBERKIND_TYPE_NODE);
			return JAVA_SEMKIND_TYPE_NODE;
		}
	}

	/* Look in any image for the type */
	type = ILClassLookupGlobal(genInfo->context, name, namespace);
	if(type)
	{
		type = ILClassResolve(type);
	}
	if(type && ILClassAccessible(type, accessedFrom))
	{
		AddMember(results, (ILProgramItem *)type, 0, JAVA_MEMBERKIND_TYPE);
		return JAVA_SEMKIND_TYPE;
	}

	/* Could not find a type or namespace with the specified name */
	return JAVA_SEMKIND_VOID;
}

ILClass *JavaGetAccessScope(ILGenInfo *genInfo, int defIsModule)
{
	if(genInfo->currentMethod)
	{
		return ILMethod_Owner
		  (((ILNode_MethodDeclaration *)(genInfo->currentMethod))->methodInfo);
	}
	else if(genInfo->currentClass &&
	        ((ILNode_ClassDefn *)(genInfo->currentClass))->classInfo)
	{
		return ((ILNode_ClassDefn *)(genInfo->currentClass))->classInfo;
	}
	else if(defIsModule)
	{
		return ILClassLookup(ILClassGlobalScope(genInfo->image),
							 "<Module>", (const char *)0);
	}
	else
	{
		return 0;
	}
}

JavaSemValue JavaResolveSimpleName(ILGenInfo *genInfo, ILNode *node,
										const char *name, int literalType)
{
	ILClass *startType;
	ILClass *accessedFrom;
	JavaMemberLookupInfo results;
	ILNode_JPackage *package;
	ILNode_JImport *import;
	int result;
	ILNode_ClassDefn *nestedParent;
	ILNode *child;

	/* If we are within type gathering, then search the nesting
	   parents for a nested type that matches our requirements */
	if(genInfo->typeGather)
	{
		nestedParent = (ILNode_ClassDefn *)(genInfo->currentClass);
		while(nestedParent != 0)
		{
			child = FindNestedClass(0, nestedParent, name);
			if(child)
			{
				JavaSemValue value;
				JavaSemSetTypeNode(value, child);
				return value;
			}
			nestedParent = nestedParent->nestedParent;
		}
	}

	/* Find the type to start looking at and the scope to use for accesses */
	startType = JavaGetAccessScope(genInfo, 0);
	accessedFrom = ILClassResolve(JavaGetAccessScope(genInfo, 1));

	/* Scan the start type and its nested parents */
	while(startType != 0)
	{
		/* Resolve cross-image references */
		startType = ILClassResolve(startType);

		/* Look for members */
		result = MemberLookup(genInfo, startType, name,
							  accessedFrom, &results, 1, 0, literalType);
		if(result != JAVA_SEMKIND_VOID)
		{
			return LookupToSem(node, name, &results, result);
		}

		/* Move up to the nested parent */
		startType = ILClass_NestedParent(startType);
	}
	/* Clear the results buffer */
	InitMembers(&results);

	/* Scan all namespaces that enclose the current context */
	package = (ILNode_JPackage *)(genInfo->currentNamespace);
	
	/* Look for the type in the current namespace */
	result = FindTypeInNamespace(genInfo, name, package->name,
								 accessedFrom, &results);
	if(result == JAVA_SEMKIND_VOID)
	{	
		/* Find the types in all using namespaces */
		import = (ILNode_JImport*)package->import;
		while(import != 0)
		{
			FindTypeInNamespace(genInfo, name, import->name,
							accessedFrom, &results);
			import = (ILNode_JImport*)import->next;
		}
	}

	/* We should have 0, 1, or many types at this point */
	if(results.num > 1)
	{
		/* The result is ambiguous */
		//AmbiguousError(node, name, &results);
	}
	if(results.num != 0)
	{
		/* Return the first type in the results list */
		if(results.members->kind == JAVA_MEMBERKIND_TYPE)
		{
			return LookupToSem(node, name, &results, JAVA_SEMKIND_TYPE);
		}
		else if(results.members->kind == JAVA_MEMBERKIND_TYPE_NODE)
		{
			return LookupToSem(node, name, &results, JAVA_SEMKIND_TYPE_NODE);
		}
		else
		{
			return LookupToSem(node, name, &results, JAVA_SEMKIND_NAMESPACE);
		}
	}
	FreeMembers(&results);

	/* Could not resolve the name */
	CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				  "`%s' is not declared in the current scope", name);
	if (literalType)
	{
		/* Resolve it cleanly if a type was not found */
		return JavaResolveSimpleName(genInfo, node, name, 0);
	}
	else
	{
		return JavaSemValueDefault;
	}
}


/*
 * Filter a member lookup results list to include only static entries.
 */
static int FilterStatic(JavaMemberLookupInfo *results, int kind)
{
	ILProgramItem *first = results->members->member;
	JavaMemberLookupIter iter;
	JavaMemberInfo *member;

	switch(kind)
	{
		case JAVA_SEMKIND_TYPE:
		{
			/* Nested types are always static members */
		}
		break;

		case JAVA_SEMKIND_FIELD:
		{
			/* Bail out if the field is not static */
			if(!ILField_IsStatic((ILField *)first))
			{
				return JAVA_SEMKIND_VOID;
			}
		}
		break;

		case JAVA_SEMKIND_METHOD_GROUP:
		{
			/* Remove all non-static methods from the group */
			MemberIterInit(&iter, results);
			while((member = MemberIterNext(&iter)) != 0)
			{
				if(member->kind == IL_META_MEMBERKIND_METHOD &&
				   !ILMethod_IsStatic((ILMethod *)(member->member)))
				{
					MemberIterRemove(&iter);
				}
			}
			if(!(results->num))
			{
				return JAVA_SEMKIND_VOID;
			}
		}
		break;
	}

	return kind;
}

/*
 * Filter a member lookup results list to include only non-static entries.
 */
static int FilterNonStatic(JavaMemberLookupInfo *results, int kind)
{
	ILProgramItem *first = results->members->member;
	JavaMemberLookupIter iter;
	JavaMemberInfo *member;

	switch(kind)
	{
		case JAVA_SEMKIND_TYPE:
		{
			/* Nested types are always static members */
			return JAVA_SEMKIND_VOID;
		}
		/* Not reached */

		case JAVA_SEMKIND_FIELD:
		{
			/* Bail out if the field is static */
			if(ILField_IsStatic((ILField *)first))
			{
				return JAVA_SEMKIND_VOID;
			}
		}
		break;

		case JAVA_SEMKIND_METHOD_GROUP:
		{
			/* Remove all static methods from the group */
			MemberIterInit(&iter, results);
			while((member = MemberIterNext(&iter)) != 0)
			{
				if(member->kind == IL_META_MEMBERKIND_METHOD &&
				   ILMethod_IsStatic((ILMethod *)(member->member)))
				{
					MemberIterRemove(&iter);
				}
			}
			if(!(results->num))
			{
				return JAVA_SEMKIND_VOID;
			}
		}
		break;
	}

	return kind;
}
/*
 * Look for a type or sub-namespace with a given name within the 
 * namespace.
 */
JavaSemValue JavaResolveNamespaceMemberName(ILGenInfo *genInfo,
		ILNode *node, JavaSemValue value, const char *name)
{
	JavaMemberLookupInfo results;
	int result;

	InitMembers(&results);
	result = FindTypeInNamespace(genInfo, name,
			JavaSemGetNamespace(value), 
			ILClassResolve(JavaGetAccessScope(genInfo, 1)),
			&results);
			
	return result == JAVA_SEMKIND_VOID ?
		JavaSemValueDefault : LookupToSem(node, name, &results, result);
}

/*
 * Look for a member to a given type
 */
static JavaSemValue JavaResolveTypeMemberName(ILGenInfo *genInfo,
		ILNode *node, JavaSemValue value, const char *name, int literalType)
{
	JavaMemberLookupInfo results;
	int result;

	/* Convert the type into a class and perform a lookup */
	result = MemberLookup(genInfo, ILTypeToClass(genInfo, JavaSemGetType(value)),
						  name, ILClassResolve(JavaGetAccessScope(genInfo, 1)), 
						  &results, 1, JavaSemIsSuper(value), literalType);

	if(result != JAVA_SEMKIND_VOID)
	{
		/* Filter the result to only include static definitions */
		result = FilterStatic(&results, result);
	}

	return result == JAVA_SEMKIND_VOID ?
		JavaSemValueDefault : LookupToSem(node, name, &results, result);
}
		
static JavaSemValue JavaResolveValueMemberName(ILGenInfo *genInfo,
		ILNode *node, JavaSemValue value, const char *name, int literalType)
{
	JavaMemberLookupInfo results;
	int result;
	
	/* Perform a member lookup based on the expression's type */
	result = MemberLookup(genInfo, 
						ILTypeToClass(genInfo, JavaSemGetType(value)),
					  	name, ILClassResolve(JavaGetAccessScope(genInfo, 1)),
						&results, 1, JavaSemIsSuper(value), literalType);
	if(result != JAVA_SEMKIND_VOID)
	{
		/* Check for instance accesses to enumerated types.
		   Sometimes there can be a property with the same
		   name as an enumerated type, and we pick up the
		   property when we are really looking for a constant */
		if(!ILTypeIsEnum(JavaSemGetType(value)) || !strcmp(name, "value__"))
		{
			/* Filter the result to remove static definitions */
			result = FilterNonStatic(&results, result);
		}
	}

	return result == JAVA_SEMKIND_VOID ?
		JavaSemValueDefault : LookupToSem(node, name, &results, result);
}


JavaSemValue JavaResolveMemberName(ILGenInfo *genInfo, ILNode *node,
							   JavaSemValue value, const char *name,
							   int literalType)
{
	JavaSemValue retSem;
	ILNode *typeNode=NULL;

	/* Determine how to resolve the member from its semantic kind */
	switch(JavaSemGetKind(value))
	{
		case JAVA_SEMKIND_NAMESPACE:
		{
		    retSem = JavaResolveNamespaceMemberName(genInfo, node, value, name);

			if (JavaSemGetKind(retSem) != JAVA_SEMKIND_VOID)
			{
				return retSem;
			}
			
			/*  Fall through to not-found processing  */
			if (!literalType)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "`%s' is not a member of the namespace `%s'",
						  name, JavaSemGetNamespace(value));
			}
		}
		break;

		case JAVA_SEMKIND_TYPE:
		{
			retSem = JavaResolveTypeMemberName(genInfo, node, value, name, 
											literalType);

			if (JavaSemGetKind(retSem) != JAVA_SEMKIND_VOID)
			{
				return retSem;
			}
			
			/*  Fall through to not-found processing  */
			if (!literalType) {
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "`%s' is not a member of the type `%s'",
						  name, JavaTypeToName(JavaSemGetType(value)));
			}
			//static members 
		}
		break;

		case JAVA_SEMKIND_LVALUE:
		case JAVA_SEMKIND_RVALUE:
		{
			retSem = JavaResolveValueMemberName(genInfo, node, value, name,
											literalType);

			if (JavaSemGetKind(retSem) != JAVA_SEMKIND_VOID)
			{
				return retSem;
			}
			
			/*  Fall through to not-found processing  */
			if (!literalType) {
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "`%s' is not an instance member of the type `%s'",
						  name, JavaTypeToName(JavaSemGetType(value)));
			}
		}
		break;
	
		case JAVA_SEMKIND_TYPE_NODE:
		{
			if(genInfo->typeGather)
			{
				if((typeNode =FindNestedClass(NULL,
								(ILNode_ClassDefn*)JavaSemGetTypeNode(value),
								name)))
				{
					JavaSemSetTypeNode(retSem,typeNode);
					return retSem;
				}
			}
			/*  Fall through to not-found processing  */
			if (!literalType) {
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "`%s' is not a nesteed class or member of `%s'",
						  name,((ILNode_ClassDefn*)
								  JavaSemGetTypeNode(value))->name);
			}
		}
		break;

		default:
		{
			if (!literalType) {
				/* This kind of semantic value does not have members */
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "invalid left operand '%s' to `.'",name);
			}
		}
		break;
	}

	if (literalType) {
		/* Try again without the type restrictions - inefficient, but 
		 * only happens in error cases 
		 * The errors will be printed this second time around  */
		return JavaResolveMemberName(genInfo, node, value, name, 0);
	}
	else
	{
		/* If we get here, then something went wrong */
		return JavaSemValueDefault;
	}
}

JavaSemValue JavaResolveConstructor(ILGenInfo *genInfo, ILNode *node,
								ILType *objectType)
{
	JavaSemValue value;
	ILClass *accessedFrom;
	int result;
	JavaMemberLookupInfo results;

	/* Find the accessor scope */
	accessedFrom = ILClassResolve(JavaGetAccessScope(genInfo, 1));

	/* Perform a member lookup based on the expression's type */
	result = MemberLookup(genInfo, ILTypeToClass(genInfo, objectType),
						  ".ctor", accessedFrom, &results, 0, 0, 0);
	if(result != JAVA_SEMKIND_VOID)
	{
		/* Filter the result to remove static definitions */
		result = FilterNonStatic(&results, result);
	}
	if(result != JAVA_SEMKIND_VOID)
	{
		return LookupToSem(node, ".ctor", &results, result);
	}

	/* There are no applicable constructors */
	JavaSemSetValueKind(value, JAVA_SEMKIND_VOID, ILType_Invalid);
	return value;
}

void *JavaCreateMethodGroup(ILMethod *method)
{
	JavaMemberLookupInfo results;

	/* Clear the results buffer */
	InitMembers(&results);

	/* Add the method as a group member */
	AddMember(&results, ILToProgramItem(method),
			  ILMethod_Owner(method), IL_META_MEMBERKIND_METHOD);

	/* Return the group to the caller */
	return results.members;
}

ILProgramItem *JavaGetGroupMember(void *group, unsigned long n)
{
	JavaMemberInfo *member = (JavaMemberInfo *)group;
	while(member != 0)
	{
		if(n <= 0)
		{
			return (ILProgramItem *)(member->member);
		}
		--n;
		member = member->next;
	}
	return 0;
}

void *JavaRemoveGroupMember(void *group, unsigned long n)
{
	JavaMemberInfo *member = (JavaMemberInfo *)group;
	JavaMemberInfo *last = 0;
	while(member != 0)
	{
		if(n <= 0)
		{
			if(last)
			{
				last->next = member->next;
				ILFree(member);
				return group;
			}
			else
			{
				last = member->next;
				ILFree(member);
				return (void *)last;
			}
		}
		--n;
		last = member;
		member = member->next;
	}
	return group;
}

void JavaSetGroupMemberForm(void *group, unsigned long n, int form)
{
	JavaMemberInfo *member = (JavaMemberInfo *)group;
	while(member != 0)
	{
		if(n <= 0)
		{
			member->form = (short)form;
			return;
		}
		--n;
		member = member->next;
	}
}

int JavaGetGroupMemberForm(void *group, unsigned long n)
{
	JavaMemberInfo *member = (JavaMemberInfo *)group;
	while(member != 0)
	{
		if(n <= 0)
		{
			return member->form;
		}
		--n;
		member = member->next;
	}
	return 0;
}


#ifdef	__cplusplus
};
#endif
