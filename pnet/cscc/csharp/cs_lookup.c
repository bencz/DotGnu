/*
 * cs_lookup.c - Member lookup routines.
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

#include "cs_internal.h"

#include "cs_lookup_member.c"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Get the method underlying a member, for permission and access checks.
 * Returns NULL if there is no underlying method.
 */
static ILMethod *GetUnderlyingMethod(ILMember *member)
{
	ILMethod *method;

	switch(ILMemberGetKind(member))
	{
		case IL_META_MEMBERKIND_METHOD:
		{
			return (ILMethod *)member;
		}
		/* Not reached */

		case IL_META_MEMBERKIND_PROPERTY:
		{
			method = ILProperty_Getter((ILProperty *)member);
			if(method)
			{
				return method;
			}
			method = ILProperty_Setter((ILProperty *)member);
			if(method)
			{
				return method;
			}
		}
		break;

		case IL_META_MEMBERKIND_EVENT:
		{
			method = ILEvent_AddOn((ILEvent *)member);
			if(method)
			{
				return method;
			}
			method = ILEvent_RemoveOn((ILEvent *)member);
			if(method)
			{
				return method;
			}
		}
		break;
	}

	return 0;
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
					    CSMemberLookupInfo *results,
						int lookInParents, int baseAccess, int literalType,
						int inAttrArg)
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
					if(literalType && kind != CS_MEMBERKIND_TYPE)
					{
						/* In literal type mode, want only types */
						continue;
					}
					if(inAttrArg && kind != IL_META_MEMBERKIND_FIELD)
					{
						/* In attribute argument mode, we only want fields */
						continue;
					}
					if(kind != IL_META_MEMBERKIND_METHOD &&
					   kind != IL_META_MEMBERKIND_FIELD &&
					   kind != IL_META_MEMBERKIND_PROPERTY &&
					   kind != IL_META_MEMBERKIND_EVENT)
					{
						/* This is PInvoke or override, which we don't need */
						continue;
					}
					underlying = GetUnderlyingMethod(member);
					if(underlying &&
					   ILMethod_IsVirtual(underlying) &&
					   !ILMethod_IsNewSlot(underlying))
					{
						/* This is a virtual override: skip it if we
						   aren't looking for a "base" member */
						if(!baseAccess)
						{
							continue;
						}
					}
					if(kind == IL_META_MEMBERKIND_PROPERTY &&
					   ILTypeNumParams(ILMember_Signature(member)) != 0)
					{
						/* This is an indexer, which we do not want here */
						continue;
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
					  info, CS_MEMBERKIND_TYPE_NODE);
		}
		else
		{
			nested = 0;
			while((nested = ILClassNextNested(info, nested)) != 0)
			{
				nestedChild = ILNestedInfoGetChild(nested);
				if(!strcmp(ILClass_Name(nestedChild), name) &&
				   ((genInfo->accessCheck)(nestedChild, accessedFrom)))
				{
					AddMember(results, (ILProgramItem *)nestedChild,
							  info, CS_MEMBERKIND_TYPE);
				}
			}
		}

		/* If this is an interface, then scan its base interfaces */
		if(ILClass_IsInterface(info))
		{
			/* also scan the 'Object' class , as all interface instances
			 * have Objects behind them !!
			 */
			FindMembers(genInfo,ILTypeToClass(genInfo,objectType),
					    name, accessedFrom, results,
						0, baseAccess, literalType, inAttrArg);

			impl = 0;
			while((impl = ILClassNextImplements(info, impl)) != 0)
			{
				FindMembers(genInfo, ILImplements_InterfaceClass(impl),
						    name, accessedFrom, results,
							lookInParents, baseAccess, literalType, inAttrArg);
			}
		}

		/* Move up to the parent */
		info = (lookInParents ? ILClass_ParentRef(info) : 0);
	}
}

int CSIsBaseTypeFor(ILClass *info1, ILClass *info2)
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

int CSSignatureIdentical(ILType *sig1, ILType *sig2)
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
 * Check if the member is implemented by the sealed class and
 * replace the member with the sealed implementation.
 */
static void CheckForSealedImplementation(CSMemberLookupInfo *results,
										 ILClass *classInfo,
										 ILClass *accessedFrom)
{
	CSMemberLookupIter iter;
	CSMemberInfo *member;

	/* Resolve the class to its actual image */
	classInfo = ILClassResolve(classInfo);

	if(!classInfo || !ILClass_IsSealed(classInfo))
	{
		/* The class is not sealed so we are done */
		return;
	}

	/* If the list is empty, then we are done too */
	if(!(results->num))
	{
		return;
	}

	MemberIterInit(&iter, results);
	while((member = MemberIterNext(&iter)) != 0)
	{
		if((member->kind == IL_META_MEMBERKIND_METHOD) &&
		   (member->owner != classInfo) &&
		   (ILMethod_IsVirtual(member->member) ||
		    ILMethod_IsAbstract(member->member)))
		{
			ILMember *memberInfo;

			memberInfo = ILClassNextMemberMatch(classInfo, 0,
												IL_META_MEMBERKIND_METHOD,
												ILMember_Name(member->member),
												ILMember_Signature(member->member));

			if(memberInfo)
			{
				if(ILMemberAccessible(memberInfo, accessedFrom))
				{
					member->owner = classInfo;
					member->member = ILToProgramItem(memberInfo);
				}
			}
		}
	}
}

/*
 * Trim a list of members to remove unneeded elements.
 */
static int TrimMemberList(CSMemberLookupInfo *results, int isIndexerList,
						  ILClass *info, ILClass *accessedFrom)
{
	CSMemberLookupIter iter;
	CSMemberInfo *firstMember;
	CSMemberInfo *member;
	CSMemberInfo *testMember;
	CSMemberInfo *tempMember;
	CSMemberInfo *prevMember;

	/* If the list is empty, then we are done */
	if(!(results->num))
	{
		return CS_SEMKIND_VOID;
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
				if(CSIsBaseTypeFor(testMember->owner, member->owner))
				{
					/* "testMember" is in a base type of "member"'s type */
					if(CSSignatureIdentical
							(ILMember_Signature(testMember->member),
						     ILMember_Signature(member->member)))
					{
						/* Remove "testMember" from the method group */
						tempMember = testMember->next;
						prevMember->next = tempMember;
						CSMemberInfoFree(testMember);
						testMember = tempMember;
						--(results->num);
						continue;
					}
				}
				else if(CSIsBaseTypeFor(member->owner, testMember->owner))
				{
					/* "member" is in a base type of "testMember"'s type */
					if(CSSignatureIdentical
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
	if(isIndexerList)
	{
		return CS_SEMKIND_INDEXER_GROUP;
	}
	else if(firstMember->kind == IL_META_MEMBERKIND_METHOD)
	{
		/* All members must be methods, or the list is ambiguous */
		MemberIterInit(&iter, results);
		while((member = MemberIterNext(&iter)) != 0)
		{
			if(member->kind != IL_META_MEMBERKIND_METHOD)
			{
				return CS_SEMKIND_AMBIGUOUS;
			}
		}
		CheckForSealedImplementation(results, info, accessedFrom);
		return CS_SEMKIND_METHOD_GROUP;
	}
	else if(results->num == 1)
	{
		/* Singleton list with a non-method */
		switch(firstMember->kind)
		{
			case CS_MEMBERKIND_TYPE:
			{
				CheckForSealedImplementation(results, info, accessedFrom);
				return CS_SEMKIND_TYPE;
			}
			/* Not reached */

			case IL_META_MEMBERKIND_FIELD:
			{
				return CS_SEMKIND_FIELD;
			}
			/* Not reached */

			case IL_META_MEMBERKIND_PROPERTY:
			{
				return CS_SEMKIND_PROPERTY;
			}
			/* Not reached */

			case IL_META_MEMBERKIND_EVENT:
			{
				return CS_SEMKIND_EVENT;
			}
			/* Not reached */
		}
		return CS_SEMKIND_AMBIGUOUS;
	}
	else
	{
		/* If we have both a field and an event, then favour the field.
		   We get such situations when compiling a class that implements
		   an event using a compiler-supplied private field.  Inside
		   the class, we want the field.  Outside the class we will
		   only see the event */
		if(results->num == 2)
		{
			if(firstMember->kind == IL_META_MEMBERKIND_FIELD &&
			   firstMember->next->kind == IL_META_MEMBERKIND_EVENT)
			{
				/* Remove the event from the results */
				MemberIterInit(&iter, results);
				MemberIterNext(&iter);
				MemberIterNext(&iter);
				MemberIterRemove(&iter);
				return CS_SEMKIND_FIELD;
			}
			if(firstMember->kind == IL_META_MEMBERKIND_EVENT &&
			   firstMember->next->kind == IL_META_MEMBERKIND_FIELD)
			{
				/* Remove the event from the results */
				MemberIterInit(&iter, results);
				MemberIterNext(&iter);
				MemberIterRemove(&iter);
				return CS_SEMKIND_FIELD;
			}
		}

		/* The list is ambiguous */
		return CS_SEMKIND_AMBIGUOUS;
	}
}

/*
 * Perform a member lookup on a type.
 */

static int MemberLookup(ILGenInfo *genInfo, ILClass *info, 
						const char *name,
				        ILClass *accessedFrom, CSMemberLookupInfo *results,
						int lookInParents, int baseAccess, int literalType,
						int inAttrArg)
{
	/* Initialize the results */
	InitMembers(results);

	/* Collect up all members with the specified name */
	if(info)
	{
		FindMembers(genInfo, info, name, accessedFrom, results,
					lookInParents, baseAccess, literalType, inAttrArg);
	}

	/* Trim the list and determine the kind for the result */
	return TrimMemberList(results, 0, info, accessedFrom);
}

/*
 * Process a class and add all indexers to a set of member lookup results.
 */
static void FindIndexers(ILClass *info, ILClass *accessedFrom,
					     CSMemberLookupInfo *results, int baseAccess)
{
	ILImplements *impl;
	ILMember *member;
	ILMethod *method;

	/* Scan up the parent hierarchy until we run out of parents */
	while(info != 0)
	{
		/* Resolve the class to its actual image */
		info = ILClassResolve(info);

		/* Look for all accessible indexers */
		member = 0;
		while((member = ILClassNextMemberByKind
					(info, member, IL_META_MEMBERKIND_PROPERTY)) != 0)
		{
			if(ILMemberAccessible(member, accessedFrom) &&
			   ILTypeNumParams(ILProperty_Signature(member)) != 0)
			{
				method = GetUnderlyingMethod(member);
				if(!method)
				{
					/* Skip indexers with no get or set methods */
					continue;
				}
				if(ILMethod_IsStatic(method))
				{
					/* Static indexers are not legal, so skip them */
					continue;
				}
				if((!baseAccess) && 
					ILMethod_IsVirtual(method) && !ILMethod_IsNewSlot(method))
				{
					/* This is a virtual override, so skip it */
					continue;
				}
				if(ILOverrideFromMethod(method) != 0)
				{
					/* Ignore private interface overrides for now, so that
					   we can pick up a non-interface indexer instead if
					   one exists.  If one doesn't exist, then we will
					   re-find the interface indexer when we scan the
					   base interfaces below.  This situation occurs with
					   code like this:

					        X this[int i] { ... }
							X I.this[int i] { ... }
					*/
					continue;
				}
				AddMember(results, (ILProgramItem *)member, info,
						  IL_META_MEMBERKIND_PROPERTY);
			}
		}

		/* Scan the base interfaces.  Duplicate entries will be cleaned
		   up by the TrimMemberList call in "IndexerLookup" */
		impl = 0;
		while((impl = ILClassNextImplements(info, impl)) != 0)
		{
			FindIndexers(ILImplements_InterfaceClass(impl),
					     accessedFrom, results, baseAccess);
		}

		/* Move up to the parent */
		info = ILClass_ParentRef(info);
	}
}

/*
 * Perform an indexer lookup on a type.
 */
static int IndexerLookup(ILGenInfo *genInfo, ILClass *info,
				         ILClass *accessedFrom, CSMemberLookupInfo *results,
						 int baseAccess)
{
	/* Initialize the results */
	InitMembers(results);

	/* Collect up all indexers */
	if(info)
	{
		FindIndexers(info, accessedFrom, results, baseAccess);
	}

	/* Trim the list and determine the kind for the result */
	return TrimMemberList(results, 1, info, accessedFrom);
}

/*
 * Print an error message that describes an ambiguous name.
 */
static void AmbiguousError(ILNode *node, const char *name,
						   CSMemberLookupInfo *results)
{
	CSMemberLookupIter iter;
	CSMemberInfo *member;
	const char *typeName;
	const char *typeName2;

	/* Print the first line of the error message */
	CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				  "`%s' is ambiguous; possibilities are:", name);

	/* Print the names and signatures of all members */
	MemberIterInit(&iter, results);
	while((member = MemberIterNext(&iter)) != 0)
	{
		switch(member->kind)
		{
			case CS_MEMBERKIND_TYPE:
			{
				typeName = CSTypeToName
						(ILClassToType((ILClass *)(member->member)));
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				              "  type %s", typeName);
			}
			break;

			case CS_MEMBERKIND_TYPE_NODE:
			{
				/* TODO */
			}
			break;

			case CS_MEMBERKIND_NAMESPACE:
			{
				/* TODO */
			}
			break;

			case IL_META_MEMBERKIND_FIELD:
			{
				typeName = CSTypeToName
						(ILField_Type((ILField *)(member->member)));
				typeName2 = CSTypeToName(ILClassToType
						(ILField_Owner((ILField *)(member->member))));
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				              "  field %s %s.%s", typeName, typeName2, name);
			}
			break;

			case IL_META_MEMBERKIND_METHOD:
			{
				typeName = CSItemToName((ILProgramItem *)(member->member));
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				              "  method %s", typeName);
			}
			break;

			case IL_META_MEMBERKIND_PROPERTY:
			{
				typeName = CSItemToName((ILProgramItem *)(member->member));
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				              "  property %s", typeName);
			}
			break;

			case IL_META_MEMBERKIND_EVENT:
			{
				typeName = CSTypeToName
						(ILEvent_Type((ILEvent *)(member->member)));
				typeName2 = CSTypeToName(ILClassToType
						(ILEvent_Owner((ILEvent *)(member->member))));
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				              "  event %s %s.%s", typeName, typeName2, name);
			}
			break;
		}
	}
}

/*
 * Convert a set of member lookup results into a semantic value.
 */
static CSSemValue LookupToSem(ILNode *node, const char *name,
							  CSMemberLookupInfo *results, int kind)
{
	CSSemValue value;
	if(kind == CS_SEMKIND_METHOD_GROUP)
	{
		/* This is a method group.  Fix later: this will leak memory! */
		CSSemSetMethodGroup(value, results->members);
		return value;
	}
	else if(kind == CS_SEMKIND_TYPE)
	{
		/* This is a type */
		CSSemSetType
			(value, ILClassToType((ILClass *)(results->members->member)));
		FreeMembers(results);
		return value;
	}
	else if(kind != CS_SEMKIND_AMBIGUOUS)
	{
		/* Type node, field, property, or event */
		CSSemSetKind(value, kind, results->members->member);
		FreeMembers(results);
		return value;
	}
	else
	{
		/* Ambiguous lookup */
		AmbiguousError(node, name, results);
		FreeMembers(results);
		return CSSemValueDefault;
	}
}

/*
 * Find the type with a specific name within a namespace.
 * Restrict the resolution to types only if typesOnly is != 0.
 */
static int FindTypeInNamespace(ILGenInfo *genInfo, const char *name,
							   ILScope *namespaceScope, ILClass *accessedFrom,
							   CSMemberLookupInfo *results, int typesOnly)
{
	ILClass *type;
	ILScopeData *data;
	int scopeKind;
	const char *namespace = ILScopeGetNamespaceName(namespaceScope);
	const char *fullName;
	ILNode_ClassDefn *node;

	/* Look in the current image for the type */
	type = ILClassLookup(ILClassGlobalScope(genInfo->image),
						 name, namespace);
	if(type)
	{
		type = ILClassResolve(type);
	}
	if(type && (genInfo->accessCheck)(type, accessedFrom))
	{
		AddMember(results, (ILProgramItem *)type, 0, CS_MEMBERKIND_TYPE);
		return CS_SEMKIND_TYPE;
	}

	/* Look in the global scope for a declared type */
	data = ILScopeLookup(namespaceScope, name, 0);
	if(data)
	{
		scopeKind = ILScopeDataGetKind(data);
		if(scopeKind == IL_SCOPE_SUBSCOPE)
		{
			if(typesOnly == 0)
			{
				fullName = ILScopeDataGetNamespaceName(data);
				AddMember(results, (ILProgramItem *)ILScopeDataGetSubScope(data),
						  0, CS_MEMBERKIND_NAMESPACE);
				return CS_SEMKIND_NAMESPACE;
			}
			else
			{
				return CS_SEMKIND_VOID;	
			}
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
						      0, CS_MEMBERKIND_TYPE);
					return CS_SEMKIND_TYPE;
				}
			}
			AddMember(results, (ILProgramItem *)node,
				      0, CS_MEMBERKIND_TYPE_NODE);
			return CS_SEMKIND_TYPE_NODE;
		}
	}

	/* Look in any image for the type */
	type = ILClassLookupGlobal(genInfo->context, name, namespace);
	if(type)
	{
		type = ILClassResolve(type);
	}
	if(type && (genInfo->accessCheck)(type, accessedFrom))
	{
		AddMember(results, (ILProgramItem *)type, 0, CS_MEMBERKIND_TYPE);
		return CS_SEMKIND_TYPE;
	}

	/* Now check if it's an existing namespace name */
	if(typesOnly == 0)
	{
		ILScope *scope;

		if(namespace && (*namespace != '\0'))
		{
			fullName = ILInternStringConcat3(ILInternString(namespace, -1),
											 ILInternString(".", 1),
											 ILInternString(name, -1)).string;
		}
		else
		{
			fullName = name;
		}
		scope = ILScopeImportNamespace(genInfo->globalScope, fullName);
		if(scope != 0)
		{
			AddMember(results, (ILProgramItem *)scope,
					  0, CS_MEMBERKIND_NAMESPACE);
			return CS_SEMKIND_NAMESPACE;
		}
	}

	/* Could not find a type or namespace with the specified name */
	return CS_SEMKIND_VOID;
}

/*
 * Find the type in the given namespace scope and all it's parents.
 */
static void FindTypeInUsingNamespace(ILGenInfo *genInfo,
									 ILScope *namespaceScope,
									 const char *using,
									 const char *name,
									 ILClass *accessedFrom,
									 CSMemberLookupInfo *results)
{
	while(namespaceScope)
	{
		ILScope *usingScope = ILScopeFindNamespace(namespaceScope, using);

		if(!usingScope)
		{
			/* We have to check for a matching namespace in the import libs. */
			const char *namespace = ILScopeGetNamespaceName(namespaceScope);

			if(namespace && (*namespace != '\0'))
			{
				const char *fullName;

				fullName = ILInternStringConcat3(ILInternString(namespace, -1),
												 ILInternString(".", 1),
												 ILInternString(name, -1)).string;
				usingScope = ILScopeImportNamespace(genInfo->globalScope, fullName);
			}
			else
			{
				usingScope = ILScopeImportNamespace(genInfo->globalScope, using);
			}
		}
		
		if(usingScope)
		{
			if(FindTypeInNamespace(genInfo, name,
								   usingScope, accessedFrom,
								   results, 1) != CS_SEMKIND_VOID)
			{
				return;
			}
		}
		namespaceScope = ILScopeGetParent(namespaceScope);
	}
}

ILClass *CSGetAccessScope(ILGenInfo *genInfo, int defIsModule)
{
	if(genInfo->currentMethod)
	{
		ILNode_MethodDeclaration *method;

		method = (ILNode_MethodDeclaration *)(genInfo->currentMethod);
		if(method->methodInfo)
		{
			return ILMethod_Owner(method->methodInfo);
		}
	}
	if(genInfo->currentClass &&
	        ((ILNode_ClassDefn *)(genInfo->currentClass))->classInfo)
	{
		ILNode_ClassDefn *currentClass;

		currentClass = (ILNode_ClassDefn *)(genInfo->currentClass);
		while(currentClass)
		{
			if((currentClass->classInfo != (ILClass *)1) &&
			   (currentClass->classInfo != (ILClass *)2))
			{
				return currentClass->classInfo;
			}
			currentClass = currentClass->nestedParent;
		}
	}
	if(defIsModule)
	{
		return ILClassLookup(ILClassGlobalScope(genInfo->image),
							 "<Module>", (const char *)0);
	}
	else
	{
		return 0;
	}
}

/*
 * Filter a member lookup results list to include only static entries.
 */
static int FilterStatic(CSMemberLookupInfo *results, int kind)
{
	ILProgramItem *first = results->members->member;
	ILMethod *method;
	CSMemberLookupIter iter;
	CSMemberInfo *member;

	switch(kind)
	{
		case CS_SEMKIND_TYPE:
		{
			/* Nested types are always static members */
		}
		break;

		case CS_SEMKIND_FIELD:
		{
			/* Bail out if the field is not static */
			if(!ILField_IsStatic((ILField *)first))
			{
				return CS_SEMKIND_VOID;
			}
		}
		break;

		case CS_SEMKIND_METHOD_GROUP:
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
				return CS_SEMKIND_VOID;
			}
		}
		break;

		case CS_SEMKIND_PROPERTY:
		{
			/* Bail out if the property is not static */
			method = ILProperty_Getter((ILProperty *)first);
			if(!method)
			{
				method = ILProperty_Setter((ILProperty *)first);
			}
			if(!method || !ILMethod_IsStatic(method))
			{
				return CS_SEMKIND_VOID;
			}
		}
		break;

		case CS_SEMKIND_EVENT:
		{
			/* Bail out if the event is not static */
			method = ILEvent_AddOn((ILEvent *)first);
			if(!method)
			{
				method = ILEvent_RemoveOn((ILEvent *)first);
			}
			if(!method || !ILMethod_IsStatic(method))
			{
				return CS_SEMKIND_VOID;
			}
		}
		break;
	}

	return kind;
}

/*
 * Filter a member lookup results list to include only non-static entries.
 */
static int FilterNonStatic(CSMemberLookupInfo *results, int kind)
{
	ILProgramItem *first = results->members->member;
	ILMethod *method;
	CSMemberLookupIter iter;
	CSMemberInfo *member;

	switch(kind)
	{
		case CS_SEMKIND_TYPE:
		{
			/* Nested types are always static members */
			return CS_SEMKIND_VOID;
		}
		/* Not reached */

		case CS_SEMKIND_FIELD:
		{
			/* Bail out if the field is static */
			if(ILField_IsStatic((ILField *)first))
			{
				return CS_SEMKIND_VOID;
			}
		}
		break;

		case CS_SEMKIND_METHOD_GROUP:
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
				return CS_SEMKIND_VOID;
			}
		}
		break;

		case CS_SEMKIND_PROPERTY:
		{
			/* Bail out if the property is static */
			method = ILProperty_Getter((ILProperty *)first);
			if(!method)
			{
				method = ILProperty_Setter((ILProperty *)first);
			}
			if(!method || ILMethod_IsStatic(method))
			{
				return CS_SEMKIND_VOID;
			}
		}
		break;

		case CS_SEMKIND_EVENT:
		{
			/* Bail out if the event is static */
			method = ILEvent_AddOn((ILEvent *)first);
			if(!method)
			{
				method = ILEvent_RemoveOn((ILEvent *)first);
			}
			if(!method || ILMethod_IsStatic(method))
			{
				return CS_SEMKIND_VOID;
			}
		}
		break;
	}

	return kind;
}

/*
 * Inner version of CSResolveSimpleName and CSResolveSimpleNameQuiet.
 */
static CSSemValue ResolveSimpleName(ILGenInfo *genInfo, ILNode *node,
							   		const char *name, int literalType,
									int reportErrors)
{
	ILClass *startType;
	ILClass *accessedFrom;
	CSMemberLookupInfo results;
	ILNode_Namespace *namespace;
	ILNode_UsingNamespace *using;
	int result;
	ILNode_ClassDefn *nestedParent;
	ILNode *child;
#if IL_VERSION_MAJOR > 1
	ILNode_ListIter iter;
	ILType *formalType;
#endif /* IL_VERSION_MAJOR > 1 */
	CSSemValue value;
	ILMethod *caller;
	int switchToStatics;
	ILNode *aliasNode;

#if IL_VERSION_MAJOR > 1
	/* Scan the method type formals for a match */
	if(genInfo->currentMethod)
	{
		ILNode_GenericTypeParameters *genParams;

		genParams =  
			(ILNode_GenericTypeParameters *)
				((ILNode_MethodDeclaration *)(genInfo->currentMethod))->typeFormals;
		if(genParams && (genParams->numTypeParams > 0))
		{
			ILNode_GenericTypeParameter *genParam;

			ILNode_ListIter_Init(&iter, genParams->typeParams);
			while((genParam = (ILNode_GenericTypeParameter *)ILNode_ListIter_Next(&iter)) != 0)
			{
				if(!strcmp(genParam->name, name))
				{
					formalType = ILTypeCreateVarNum
						(genInfo->context, IL_TYPE_COMPLEX_MVAR, genParam->num);
					if(!formalType)
					{
						CCOutOfMemory();
					}
					CSSemSetType(value, formalType);
					return value;
				}
			}
		}
	}

	if(genInfo->currentClass)
	{
		ILNode_ClassDefn *defn = (ILNode_ClassDefn *)(genInfo->currentClass);

		while(defn)
		{
			ILNode_GenericTypeParameters *genParams;

			genParams =  
				(ILNode_GenericTypeParameters *)(defn->typeFormals);

			if(genParams)
			{
				/* Scan the type formals for a match */
				ILNode_GenericTypeParameter *genParam;

				ILNode_ListIter_Init(&iter, genParams->typeParams);
				while((genParam = (ILNode_GenericTypeParameter *)ILNode_ListIter_Next(&iter)) != 0)
				{
					if(!strcmp(genParam->name, name))
					{
						formalType = ILTypeCreateVarNum
							(genInfo->context, IL_TYPE_COMPLEX_VAR, genParam->num);
						if(!formalType)
						{
							CCOutOfMemory();
						}
						CSSemSetType(value, formalType);
						return value;
					}
				}
			}
			defn = defn->nestedParent;
		}
	}
#endif /* IL_VERSION_MAJOR > 1 */

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
				CSSemSetTypeNode(value, child);
				return value;
			}
			nestedParent = nestedParent->nestedParent;
		}
	}

	/* Find the type to start looking at and the scope to use for accesses */
	startType = CSGetAccessScope(genInfo, 0);
	accessedFrom = ILClassResolve(CSGetAccessScope(genInfo, 1));

	/* Scan the start type and its nested parents */
	/* Note: do not lookup class members while resolving the simple names
	 * inside an attribute argument */
	switchToStatics = 0;
	while(startType != 0 /*&& !genInfo->inAttrArg*/)
	{
		/* Resolve cross-image references */
		startType = ILClassResolve(startType);

		/* Look for members */
		result = MemberLookup(genInfo, startType, name,
							  accessedFrom, &results, 1, 0, literalType,
							  genInfo->inAttrArg);
		if(result != CS_SEMKIND_VOID)
		{
			if(genInfo->currentMethod && !reportErrors)
			{
				caller = ((ILNode_MethodDeclaration *)(genInfo->currentMethod))
							->methodInfo;
				if(caller && ILMethod_IsStatic(caller))
				{
					/* We are in a static context, so filter out
					   non-static members */
					result = FilterStatic(&results, result);
				}
			}
		}
		if(result != CS_SEMKIND_VOID && switchToStatics)
		{
			result = FilterStatic(&results, result);
		}
		if(result != CS_SEMKIND_VOID)
		{
			return LookupToSem(node, name, &results, result);
		}

		/* Move up to the nested parent and then switch to only
		   looking for static members.  This prevents us from picking
		   up an instance field within a nested parent and then trying
		   to access it via the child's "this" pointer */
		startType = ILClass_NestedParent(startType);
		switchToStatics = 1;
	}

	/* Clear the results buffer */
	InitMembers(&results);

	/* Scan all namespaces that enclose the current context */
	namespace = (ILNode_Namespace *)(genInfo->currentNamespace);
	while(namespace != 0 && !(results.num))
	{
		/* Look for the type in the current namespace */
		result = FindTypeInNamespace(genInfo, name, namespace->localScope,
									 accessedFrom, &results, 0);
		if(result != CS_SEMKIND_VOID)
		{
			/* We have to check if an alias with the same name is declared here too. */
			aliasNode = ILNamespaceResolveAlias(namespace, name, 0);

			if(aliasNode)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "`%s' is defined ambiguously because an alias exists with the same name", name);
			}
			break;
		}

		if(!(genInfo->resolvingAlias))
		{
			/* Check the aliases here. */
			aliasNode = ILNamespaceResolveAlias(namespace, name, 0);
			if(aliasNode)
			{
				if(yykind(aliasNode) == yykindof(ILNode_UsingAlias))
				{
					ILNode *alias = ((ILNode_UsingAlias *)aliasNode)->ref;
					int savedState = genInfo->inSemType;

					genInfo->resolvingAlias = -1;
					genInfo->inSemType = -1;
					value = ILNode_SemAnalysis(alias, genInfo, &aliasNode);
					genInfo->inSemType = savedState;
					genInfo->resolvingAlias = 0;

					/* Now check if the alias resolves to a type or namespace. */
					if((CSSemGetKind(value) == CS_SEMKIND_TYPE) ||
					   (CSSemGetKind(value) == CS_SEMKIND_NAMESPACE))
					{
						return value;
					}
					else
					{
						if(reportErrors)
						{
							CCErrorOnLine(yygetfilename(alias), yygetlinenum(alias),
								"the alias `%s' does not refere to a type or namespace.",
								ILQualIdentName(alias, 0));
						}
						FreeMembers(&results);
						return CSSemValueDefault;
					}
				}
			}
		}

		using = namespace->using;
		while(using != 0)
		{
			FindTypeInUsingNamespace(genInfo,
									 namespace->localScope,
									 using->name,
									 name,
									 accessedFrom,
									 &results);
			using = using->next;
		}

		if(results.num > 1)
		{
			if(reportErrors)
			{
				/* The result is ambiguous */
				AmbiguousError(node, name, &results);
			}
			FreeMembers(&results);
			return CSSemValueDefault;
		}

		/* Move up to the enclosing namespace */
		namespace = namespace->enclosing;
	}

	/* We should have 0, 1, or many types at this point */
	if(results.num > 1 && reportErrors)
	{
		/* The result is ambiguous */
		AmbiguousError(node, name, &results);
	}
	if(results.num != 0)
	{
		/* Return the first type in the results list */
		if(results.members->kind == CS_MEMBERKIND_TYPE)
		{
			return LookupToSem(node, name, &results, CS_SEMKIND_TYPE);
		}
		else if(results.members->kind == CS_MEMBERKIND_TYPE_NODE)
		{
			return LookupToSem(node, name, &results, CS_SEMKIND_TYPE_NODE);
		}
		else
		{
			return LookupToSem(node, name, &results, CS_SEMKIND_NAMESPACE);
		}
	}
	FreeMembers(&results);

	/* Could not resolve the name */
	if(reportErrors)
	{
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  "`%s' is not declared in the current scope", name);
	}
	if (literalType)
	{
		/* Resolve it cleanly if a type was not found */
		return ResolveSimpleName(genInfo, node, name, 0, reportErrors);
	}
	else
	{
		return CSSemValueDefault;
	}
}

CSSemValue CSResolveSimpleName(ILGenInfo *genInfo, ILNode *node,
							   const char *name, int literalType)
{
	return ResolveSimpleName(genInfo, node, name, literalType, 1);
}

CSSemValue CSResolveSimpleNameQuiet(ILGenInfo *genInfo, ILNode *node,
									const char *name, int literalType)
{
	return ResolveSimpleName(genInfo, node, name, literalType, 0);
}

/*
 * Look for a type or sub-namespace with a given name within the 
 * namespace.
 */
CSSemValue CSResolveNamespaceMemberName(ILGenInfo *genInfo,
		ILNode *node, CSSemValue value, const char *name)
{
	CSMemberLookupInfo results;
	int result;

	InitMembers(&results);

	/* Look for a type first */
	result = FindTypeInNamespace(genInfo, name,
								 CSSemGetNamespace(value),
								 ILClassResolve(CSGetAccessScope(genInfo, 1)),
								 &results, 0);

	/* Return the result to the caller */
	if(result == CS_SEMKIND_VOID)
	{
		return CSSemValueDefault;
	}
	else
	{
		return LookupToSem(node, name, &results, result);
	}
}

/*
 * Look for a member to a given type
 */
static CSSemValue CSResolveTypeMemberName(ILGenInfo *genInfo,
		ILNode *node, CSSemValue value, const char *name, int literalType)
{
	CSMemberLookupInfo results;
	int result;

	/* Convert the type into a class and perform a lookup */
	result = MemberLookup(genInfo, ILTypeToClass(genInfo, CSSemGetType(value)),
						  name, ILClassResolve(CSGetAccessScope(genInfo, 1)), 
						  &results, 1, CSSemIsBase(value), literalType, 0);

	if(result != CS_SEMKIND_VOID)
	{
		/* Filter the result to only include static definitions */
		result = FilterStatic(&results, result);
	}

	return result == CS_SEMKIND_VOID ?
		CSSemValueDefault : LookupToSem(node, name, &results, result);
}
		
static CSSemValue CSResolveValueMemberName(ILGenInfo *genInfo,
		ILNode *node, CSSemValue value, const char *name, int literalType)
{
	CSMemberLookupInfo results;
	int result;
	
	/* Perform a member lookup based on the expression's type */
	result = MemberLookup(genInfo, ILTypeToClass(genInfo, CSSemGetType(value)),
						  name, ILClassResolve(CSGetAccessScope(genInfo, 1)),
						  &results, 1, CSSemIsBase(value), literalType, 0);

	if(result != CS_SEMKIND_VOID)
	{
		/* Check for instance accesses to enumerated types.
		   Sometimes there can be a property with the same
		   name as an enumerated type, and we pick up the
		   property when we are really looking for a constant */
		if(!ILTypeIsEnum(CSSemGetType(value)) || !strcmp(name, "value__"))
		{
			/* Filter the result to remove static definitions */
			result = FilterNonStatic(&results, result);
		}
	}

	return result == CS_SEMKIND_VOID ?
		CSSemValueDefault : LookupToSem(node, name, &results, result);
}

CSSemValue CSResolveMemberName(ILGenInfo *genInfo, ILNode *node,
							   CSSemValue value, const char *name,
							   int literalType)
{
	CSSemValue retSem;
	ILNode *typeNode=NULL;

	/* Determine how to resolve the member from its semantic kind */
	switch(CSSemGetKind(value) & ~CS_SEMKIND_BASE)
	{
		case CS_SEMKIND_NAMESPACE:
		{
		    retSem = CSResolveNamespaceMemberName(genInfo, node, value, name);

			if (CSSemGetKind(retSem) != CS_SEMKIND_VOID)
			{
				return retSem;
			}
			
			/*  Fall through to not-found processing  */
			if (!literalType)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "`%s' is not a member of the namespace `%s'",
						  name,
						  ILScopeGetNamespaceName(CSSemGetNamespace(value)));
			}
		}
		break;

		case CS_SEMKIND_TYPE:
		{
			retSem = CSResolveTypeMemberName(genInfo, node, value, name, 
											literalType);

			if (CSSemGetKind(retSem) != CS_SEMKIND_VOID)
			{
				return retSem;
			}
			
			/*  Fall through to not-found processing  */
			if (!literalType) {
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "`%s' is not a member of the type `%s'",
						  name, CSTypeToName(CSSemGetType(value)));
			}
		}
		break;

		case CS_SEMKIND_LVALUE:
		case CS_SEMKIND_RVALUE:
		{
			retSem = CSResolveValueMemberName(genInfo, node, value, name,
											literalType);

			if (CSSemGetKind(retSem) != CS_SEMKIND_VOID)
			{
				return retSem;
			}
			
			/*  Fall through to not-found processing  */
			if (!literalType) {
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "`%s' is not an instance member of the type `%s'",
						  name, CSTypeToName(CSSemGetType(value)));
			}
		}
		break;
	
		case CS_SEMKIND_TYPE_NODE:
		{
			if(genInfo->typeGather)
			{
				if((typeNode =FindNestedClass(NULL,
								(ILNode_ClassDefn*)CSSemGetTypeNode(value),
								name)))
				{
					CSSemSetTypeNode(retSem,typeNode);
					return retSem;
				}
			}
			/*  Fall through to not-found processing  */
			if (!literalType) {
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "`%s' is not a nesteed class or member of `%s'",
						  name,((ILNode_ClassDefn*)
								  CSSemGetTypeNode(value))->name);
			}
		}
		break;

		default:
		{
			if (!literalType) {
				/* This kind of semantic value does not have members */
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "invalid left operand to `.'");
			}
		}
		break;
	}

	if (literalType) {
		/* Try again without the type restrictions - inefficient, but 
		 * only happens in error cases 
		 * The errors will be printed this second time around  */
		return CSResolveMemberName(genInfo, node, value, name, 0);
	}
	else
	{
		/* If we get here, then something went wrong */
		return CSSemValueDefault;
	}
}

CSSemValue CSResolveMemberNameQuiet(ILGenInfo *genInfo, ILNode *node,
							   CSSemValue value, const char *name,
							   int literalType)
{
	CSSemValue retSem;

	/* Determine how to resolve the member from its semantic kind */
	switch(CSSemGetKind(value) & ~CS_SEMKIND_BASE)
	{
		case CS_SEMKIND_NAMESPACE:
		{
		    retSem = CSResolveNamespaceMemberName(genInfo, node, value, name);

			if (CSSemGetKind(retSem) != CS_SEMKIND_VOID)
			{
				return retSem;
			}
			
			/*  Fall through to not-found processing  */
		}
		break;

		case CS_SEMKIND_TYPE:
		{
			retSem = CSResolveTypeMemberName(genInfo, node, value, name, 
											literalType);

			if (CSSemGetKind(retSem) != CS_SEMKIND_VOID)
			{
				return retSem;
			}
			
			/*  Fall through to not-found processing  */
		}
		break;

		case CS_SEMKIND_LVALUE:
		case CS_SEMKIND_RVALUE:
		{
			retSem = CSResolveValueMemberName(genInfo, node, value, name,
											literalType);

			if (CSSemGetKind(retSem) != CS_SEMKIND_VOID)
			{
				return retSem;
			}
			
			/*  Fall through to not-found processing  */
		}
		break;
	}

	if (literalType) {
		/* Try again without the type restrictions - inefficient, but 
		 * only happens in error cases 
		 * The errors will be printed this second time around  */
		return CSResolveMemberNameQuiet(genInfo, node, value, name, 0);
	}
	else
	{
		/* If we get here, then something went wrong */
		return CSSemValueDefault;
	}
}

CSSemValue CSResolveConstructor(ILGenInfo *genInfo, ILNode *node,
								ILType *objectType)
{
	CSSemValue value;
	ILClass *accessedFrom;
	int result;
	CSMemberLookupInfo results;

	/* Find the accessor scope */
	accessedFrom = ILClassResolve(CSGetAccessScope(genInfo, 1));

	/* Perform a member lookup based on the expression's type */
	result = MemberLookup(genInfo, ILTypeToClass(genInfo, objectType),
						  ".ctor", accessedFrom, &results, 0, 0, 0, 0);
	if(result != CS_SEMKIND_VOID)
	{
		/* Filter the result to remove static definitions */
		result = FilterNonStatic(&results, result);
	}
	if(result != CS_SEMKIND_VOID)
	{
		return LookupToSem(node, ".ctor", &results, result);
	}

	/* There are no applicable constructors */
	CSSemSetValueKind(value, CS_SEMKIND_VOID, ILType_Invalid);
	return value;
}

CSSemValue CSResolveIndexers(ILGenInfo *genInfo, ILNode *node,
							 ILClass *classInfo, int baseAccess)
{
	CSSemValue value;
	ILClass *accessedFrom;
	int result;
	CSMemberLookupInfo results;

	/* Find the accessor scope */
	accessedFrom = ILClassResolve(CSGetAccessScope(genInfo, 1));

	/* Perform a member lookup based on the class */
	result = IndexerLookup(genInfo, classInfo, accessedFrom, &results, 
							baseAccess);
	if(result != CS_SEMKIND_VOID)
	{
		CSSemSetKind(value, result, results.members);
		return value;
	}

	/* There are no applicable indexers */
	CSSemSetValueKind(value, CS_SEMKIND_VOID, ILType_Invalid);
	return value;
}

#ifdef	__cplusplus
};
#endif
