/*
 * cg_scope.c - Scope handling declarations.
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

#include "cg_nodes.h"
#include "cg_scope.h"
#include "cg_rbtree.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Data that is associated with a scope item.
 */
struct _tagILScopeData
{
	ILRBTreeNode		rbnode;		/* Red-black node information */
	const char	       *name;		/* Name associated with the item */
	ILNode			   *node;		/* Node associated with the item */
	void			   *data;		/* Data associated with the item */
	void			   *data2;		/* Data associated with the item */

};

/*
 * Information about a "using" directive.
 */
typedef struct _tagILScopeUsingInfo ILScopeUsingInfo;
struct _tagILScopeUsingInfo
{
	ILScope			   *refScope;	/* Referenced using scope */
	ILScopeUsingInfo   *next;		/* Next using declaration */

};

/*
 * Internal structure of a scope record.
 */
struct _tagILScope
{
	ILGenInfo	   *info;		/* Code generator associated with the scope */
	ILScope		   *parent;		/* Parent scope */
	ILRBTree		nameTree;	/* Tree containing all names in the scope */
	ILScopeUsingInfo *using;	/* Using declarations for the scope */
	ILScope			*aliases;	/* aliases lookup scope */
	void		   *userData;	/* user data attached to the scope */

	/* Function for looking up an item within the scope */
	ILScopeData *(*lookup)(ILScope *scope, const char *name);

};

/*
 * Comparison function for finding a name within a scope.
 */
static int NameCompare(void *key, ILRBTreeNode *node)
{
	const char *name = (const char *)key;
	ILScopeData *data = (ILScopeData *)node;
	return strcmp(name, data->name);
}

/*
 * Lookup function for normal scopes.
 */
static ILScopeData *NormalScope_Lookup(ILScope *scope, const char *name)
{
	return (ILScopeData *)ILRBTreeSearch(&(scope->nameTree), (void *)name);
}

/*
 * Lookup function for normal scopes with "using" directives.
 */
static ILScopeData *UsingScope_Lookup(ILScope *scope, const char *name)
{
	ILScopeData *data;
	ILScopeUsingInfo *using;

	/* Try looking in the scope itself */
	data = NormalScope_Lookup(scope, name);
	if(data != 0)
	{
		return data;
	}

	if(scope->aliases)
	{
		data=(*(scope->aliases->lookup))(scope->aliases,name);
		if(data !=0)
		{
			return data;
		}
	}

	/* Search each of the "using" scopes */
	using = scope->using;
	while(using != 0)
	{
		data = (*(using->refScope->lookup))(using->refScope, name);
		if(data != 0)
		{
			return data;
		}
		using = using->next;
	}

	/* Could not find the name */
	return 0;
}

/*
 * Lookup function for type scopes.
 */
static ILScopeData *TypeScope_Lookup(ILScope *scope, const char *name)
{
	return NormalScope_Lookup(scope, name);
}

void ILScopeInit(ILGenInfo *info)
{
	ILMemPoolInitType(&(info->scopePool), ILScope, 0);
	ILMemPoolInitType(&(info->scopeDataPool), ILScopeData, 0);
}

ILScope *ILScopeCreate(ILGenInfo *info, ILScope *parent)
{
	ILScope *scope = ILMemPoolAlloc(&(info->scopePool), ILScope);
	if(!scope)
	{
		ILGenOutOfMemory(info);
	}
	scope->info = info;
	scope->parent = parent;
	ILRBTreeInit(&(scope->nameTree), NameCompare, 0);
	scope->using = 0;
	scope->aliases = 0;
	scope->userData = 0;
	scope->lookup = NormalScope_Lookup;
	return scope;
}

/*
 * Add an item to a particular scope.
 */
static void AddToScope(ILScope *scope, const char *name,
					   int kind, ILNode *node, void *data,
					   void *data2)
{
	ILScopeData *sdata;
	sdata = ILMemPoolAlloc(&(scope->info->scopeDataPool), ILScopeData);
	if(!sdata)
	{
		ILGenOutOfMemory(scope->info);
	}
	sdata->rbnode.kind = kind;
	sdata->name = name;
	sdata->node = node;
	sdata->data = data;
	sdata->data2 = data2;
	ILRBTreeInsert(&(scope->nameTree), &(sdata->rbnode), (void *)name);
}

/*
 * Find a namespace scope from a name.  If the namespace
 * scope does not exist, then create it if addMissing is != 0.
 */
static ILScope *FindNamespaceScope(ILScope *scope,
								   const char *name,
								   int addIfMissing)
{
	ILScopeData *data;
	int len;
	ILIntString newName;
	int newNameNeedsIntern = 0;

	while(*name != '\0')
	{
		if(*name == '.')
		{
			++name;
			continue;
		}
		len = 0;
		while(name[len] != '\0' && name[len] != '.')
		{
			++len;
		}
		if(name[len] == '\0')
		{
			newName.string = name;
			newName.len = len;
			newNameNeedsIntern = 1;
		}
		else
		{
			newName = ILInternString(name, len);
			newNameNeedsIntern = 0;
		}
		data = ILScopeLookup(scope, newName.string, 0);
		if(data != 0)
		{
			if(data->rbnode.kind == IL_SCOPE_SUBSCOPE)
			{
				/* We already have a namespace with this name */
				scope = (ILScope *)(data->data);
			}
			else
			{
				/* There is something already declared here that
				   is not a namespace */
				return 0;
			}
		}
		else
		{
			if(addIfMissing)
			{
				ILScope *newScope = ILScopeCreate(scope->info, scope);
				AddToScope(scope, newName.string, IL_SCOPE_SUBSCOPE, 0, newScope, 0);
				if(scope)
				{
					if(scope->userData && (*(char *)(scope->userData) != '\0'))
					{
						ILIntString namespace;
						ILIntString dot;

						namespace.string = (const char *)(scope->userData);
						namespace.len = strlen(namespace.string);
						dot.string = ".";
						dot.len = 1;
						newScope->userData = 
							(void *)(ILInternStringConcat3(namespace,
														   dot,
														   newName).string);
					}
					else
					{
						if(newNameNeedsIntern)
						{
							newName = ILInternString(newName.string,
													 newName.len);
						}
						newScope->userData = (void *)newName.string;
					}
				}
				else
				{
					if(newNameNeedsIntern)
					{
						newName = ILInternString(newName.string, newName.len);
					}
					newScope->userData = (void *)newName.string;
				}
				scope = newScope;
			}
			else
			{
				/* The Namespace couldn't be found so bail out. */
				return (ILScope *)0;
			}
		}
		name += len;
	}
	return scope;
}

/*
 * Import a type into a scope.  A new scope is created
 * for the type to access its members and nested children.
 */
static void ImportType(ILScope *scope, ILClass *info, const char *name)
{
	ILScope *newScope;
	ILNestedInfo *nested;
	ILClass *child;
	const char *nestedName;
	int len;
	ILMember *member;
	int kind;
	const char *memberName;

	/* Bail out if the type is already declared */
	if(ILScopeLookup(scope, name, 0) != 0)
	{
		return;
	}

	/* Create a new scope for the type */
	newScope = ILScopeCreate(scope->info, scope);
	newScope->lookup = TypeScope_Lookup;
	newScope->userData = (void *)info;

	/* Add the new scope to the original scope, attached to the type name */
	AddToScope(scope, name, IL_SCOPE_IMPORTED_TYPE, 0, newScope, 0);

	/* Add the nested children to sub-scopes */
	nested = 0;
	while((nested = ILClassNextNested(info, nested)) != 0)
	{
		/* Get the child type from the nested information token */
		child = ILNestedInfoGetChild(nested);

		/* Get the name of the nested type, without prefixes */
		nestedName = ILClass_Name(child);
		len = strlen(nestedName);
		while(len > 0 && nestedName[len - 1] != '$')
		{
			--len;
		}

		/* Import the nested type into a sub-scope */
		ImportType(newScope, child, nestedName + len);
	}

	/* Add the fields, methods, properties, and events to the new scope.
	   If there are duplicates in the imported image, we ignore them */
	member = 0;
	while((member = ILClassNextMember(info, member)) != 0)
	{
		kind = ILMemberGetKind(member);
		memberName = ILMember_Name(member);
		if(kind == IL_META_MEMBERKIND_FIELD)
		{
			if(!ILScopeLookup(newScope, memberName, 0))
			{
				AddToScope(newScope, memberName, IL_SCOPE_FIELD, 0, member, 0);
			}
		}
		else if(kind == IL_META_MEMBERKIND_METHOD)
		{
			/* Duplicates are OK for methods */
			AddToScope(newScope, memberName, IL_SCOPE_METHOD, 0, member, 0);
		}
		else if(kind == IL_META_MEMBERKIND_PROPERTY)
		{
			if(!ILScopeLookup(newScope, memberName, 0))
			{
				AddToScope(newScope, memberName, IL_SCOPE_PROPERTY,
						   0, member, 0);
			}
		}
		else if(kind == IL_META_MEMBERKIND_EVENT)
		{
			if(!ILScopeLookup(newScope, memberName, 0))
			{
				AddToScope(newScope, memberName, IL_SCOPE_EVENT, 0, member, 0);
			}
		}
	}
}

ILScope *ILScopeImportNamespace(ILScope *scope, const char *namespace)
{
	if(scope && namespace)
	{
		ILImage *image = 0;
		int namespaceLen = strlen(namespace);

		if(namespaceLen == 0)
		{
			return 0;
		}

		if(ILClassNamespaceIsValid(scope->info->context, namespace))
		{
			return FindNamespaceScope(scope->info->globalScope,
									  namespace, 1);
		}

		while((image = ILContextNextImage(scope->info->context, image)) != 0)
		{
			unsigned long numTokens;
			unsigned long token;
			ILClass *info;
			const char *namespaceTest = 0;

			numTokens = ILImageNumTokens(image, IL_META_TOKEN_TYPE_DEF);
			for(token = 1; token <= numTokens; ++token)
			{
				info = (ILClass *)(ILImageTokenInfo
								(image, token | IL_META_TOKEN_TYPE_DEF));

				if(info && (namespaceTest != ILClass_Namespace(info)))
				{
					namespaceTest = ILClass_Namespace(info);
					if(namespaceTest)
					{
						int testLen = strlen(namespaceTest);

						if(!strncmp(namespace, namespaceTest, namespaceLen))
						{
							if((testLen == namespaceLen) ||
							   ((testLen > namespaceLen) && (namespaceTest[namespaceLen] == '.')))
							{
								return FindNamespaceScope(scope->info->globalScope,
														  namespace, 1);
							}
						}
					}
				}
			}
		}
	}
	return 0;
}

void ILScopeImport(ILScope *scope, ILImage *image)
{
	unsigned long numTokens;
	unsigned long token;
	ILClass *info;
	ILScope *namespaceScope = 0;
	const char *namespaceName = 0;
	const char *namespaceTest;
	ILAssembly *assem;
	ILImage *otherImage;
	char dummyName[128];

	/* Have we already imported this image? */
	sprintf(dummyName, ".img.%lX", (long)image);
	if(ILScopeLookup(scope, dummyName, 0))
	{
		return;
	}
	AddToScope(scope, dummyName, IL_SCOPE_DUMMY, 0, 0, 0);

	/* Process imported assemblies first */
	assem = 0;
	while((assem = (ILAssembly *)ILImageNextToken
				(image, IL_META_TOKEN_ASSEMBLY_REF, (void *)assem)) != 0)
	{
		otherImage = ILAssemblyToImage(assem);
		if(otherImage)
		{
			ILScopeImport(scope, otherImage);
		}
	}

	/* Scan the entire TypeDef table for top-level types */
	numTokens = ILImageNumTokens(image, IL_META_TOKEN_TYPE_DEF);
	for(token = 1; token <= numTokens; ++token)
	{
		info = (ILClass *)(ILImageTokenInfo
								(image, token | IL_META_TOKEN_TYPE_DEF));
		if(info && !ILClass_NestedParent(info) &&
		   strcmp(ILClass_Name(info), "<Module>") != 0)
		{
			/* Find the scope corresponding to the namespace */
			namespaceTest = ILClass_Namespace(info);
			if(namespaceTest)
			{
				if(!namespaceName ||
				   !(namespaceName == namespaceTest ||
				     !strcmp(namespaceName, namespaceTest)))
				{
					namespaceName = namespaceTest;
					namespaceScope = FindNamespaceScope(scope,
														namespaceTest, 1);
				}
			}
			else
			{
				namespaceScope = scope;
				namespaceName = 0;
			}

			/* Import the type into the namespace scope */
			if(namespaceScope != 0)
			{
				ImportType(namespaceScope, info, ILClass_Name(info));
			}
			else
			{
				/* There is some kind of conflict between the namespace
				   name and a type declaration - ignore it for now */
				namespaceName = 0;
			}
		}
	}
}

int ILScopeUsing(ILScope *scope, const char *identifier)
{
	ILScope *namespaceScope = FindNamespaceScope(scope, identifier, 1);
	ILScopeUsingInfo *using;
	if(!namespaceScope)
	{
		return 0;
	}
	/* Add the "using" declaration to the scope as an attribute.
	   We allocate from the data pool, because it isn't worth
	   creating a special pool just for "using" declarations */
	using = ILMemPoolAlloc(&(scope->info->scopeDataPool), ILScopeUsingInfo);
	if(!using)
	{
		ILGenOutOfMemory(scope->info);
	}
	using->refScope = namespaceScope;
	using->next = scope->using;
	scope->using = using;

	/* Change the lookup function to one which handles "using" clauses */
	scope->lookup = UsingScope_Lookup;
	return 1;
}

void ILScopeClearUsing(ILScope *scope)
{
	scope->using = 0;
	scope->aliases = 0;
}

ILScopeData *ILScopeLookup(ILScope *scope, const char *identifier, int up)
{
	ILScopeData *data;
	while(scope != 0)
	{
		/* Look for the name in this scope */
		data = (*(scope->lookup))(scope, identifier);
		if(data != 0)
		{
			/* We have found a match for the name */
			return data;
		}

		/* Move up to the parent scope and try again */
		if(up)
		{
			scope = scope->parent;
		}
		else
		{
			break;
		}
	}
	return 0;
}

ILScopeData *ILScopeLookupInNamespace(ILScope *globalScope,
									  const char *namespace,
									  const char *identifier)
{
	ILScope *scope;
	if(!namespace)
	{
		scope = globalScope;
	}
	else
	{
		scope = FindNamespaceScope(globalScope, namespace, 0);
		if(!scope)
		{
			return 0;
		}
	}
	return ILScopeLookup(scope, identifier, 0);
}

void ILScopeDeclareItem(ILScope *scope, const char *name, int kind,
						ILNode *node, void *data1, void *data2)
{
	AddToScope(scope, name, kind, node, data1, data2);
}

ILScopeData *ILScopeNextItem(ILScopeData *data)
{
	return (ILScopeData *)(ILRBTreeNext(&(data->rbnode)));
}

const char *ILScopeGetNamespaceName(ILScope *scope)
{
	if(scope)
	{
		return (const char *)(scope->userData);
	}
	return 0;
}

ILScope *ILScopeDeclareNamespace(ILScope *globalScope, const char *namespace)
{
	return FindNamespaceScope(globalScope, namespace, 1);
}

ILScope *ILScopeFindNamespace(ILScope *globalScope, const char *namespace)
{
	return FindNamespaceScope(globalScope, namespace, 0);
}

int ILScopeDeclareType(ILScope *scope, ILNode *node, const char *name,
					   const char *namespace, ILScope **resultScope,
					   ILNode **origDefn, ILScope *attachScope)
{
	ILScope *namespaceScope;
	ILScopeData *data;
	ILScope *usingScope;
	ILScope *typeScope;

	/* Find the scope that is associated with the namespace */
	if(namespace && *namespace != '\0')
	{
		namespaceScope = FindNamespaceScope(scope, namespace, 1);
		if(!namespaceScope)
		{
			return IL_SCOPE_ERROR_CANT_CREATE_NAMESPACE;
		}
	}
	else
	{
		namespaceScope = scope;
	}

	/* Determine if there is a declaration for the name already */
	data = ILScopeLookup(namespaceScope, name, 0);
	if(data != 0)
	{
		if(data->rbnode.kind == IL_SCOPE_DECLARED_TYPE)
		{
			/* Declaration conflicts with a type the user already declared */
			*origDefn = data->node;
			/* return the type scope of the declared type too */
			*resultScope = (ILScope *)(data->data);
			return IL_SCOPE_ERROR_REDECLARED;
		}
		else if(data->rbnode.kind == IL_SCOPE_SUBSCOPE)
		{
			/* There is already a namespace with that name in existence */
			return IL_SCOPE_ERROR_NAME_IS_NAMESPACE;
		}
		else if(data->rbnode.kind == IL_SCOPE_IMPORTED_TYPE)
		{
			/* Conflict with an imported type */
			return IL_SCOPE_ERROR_IMPORT_CONFLICT;
		}
		else
		{
			return IL_SCOPE_ERROR_OTHER;
		}
	}

	/* Create a new scope to hold the "using" context for the type.
	   We must do this because the global "using" context will be
	   cleared at the end of the parse, but we need the information
	   it contains after the parse */
	usingScope = ILScopeCreate(scope->info, scope);
	usingScope->aliases = attachScope;
	usingScope->using = scope->using;
	usingScope->lookup = UsingScope_Lookup;

	/* Create a scope to hold the type itself */
	typeScope = ILScopeCreate(scope->info, usingScope);

	/* Add the type to the namespace scope */
	AddToScope(namespaceScope, name, IL_SCOPE_DECLARED_TYPE,
			   node, typeScope, 0);

	/* Done */
	*resultScope = typeScope;
	return IL_SCOPE_ERROR_OK;
}

/*
 * Resolve a qualified identifier.
 */
static ILScopeData *ResolveQualIdent(ILScope *scope, ILNode *identifier)
{
	ILScopeData *data;

	if(!identifier)
	{
		return 0;
	}
	if(yykind(identifier) == yykindof(ILNode_Identifier))
	{
		/* Singleton identifier */
		return ILScopeLookup(scope, ((ILNode_Identifier *)identifier)->name, 1);
	}
	else if(yykind(identifier) == yykindof(ILNode_QualIdent))
	{
		/* Qualified identifier */
		ILNode_QualIdent *ident = (ILNode_QualIdent *)identifier;
		data = ResolveQualIdent(scope, ident->left);
		if(!data)
		{
			return 0;
		}
		if(data->rbnode.kind == IL_SCOPE_SUBSCOPE)
		{
			/* Search for the name within a namespace */
			return ILScopeLookup((ILScope *)(data->data), ident->name, 0);
		}
		else if(data->rbnode.kind == IL_SCOPE_IMPORTED_TYPE ||
				data->rbnode.kind == IL_SCOPE_DECLARED_TYPE)
		{
			/* Search for a nested type */
			return ILScopeLookup((ILScope *)(data->data), ident->name, 0);
		}
		else
		{
			return 0;
		}
	}
	else
	{
		/* Don't know what kind of node this is */
		return 0;
	}
}

int ILScopeResolveType(ILScope *scope, ILNode *identifier,
					   const char *namespace, ILClass **classInfo,
					   ILNode **nodeInfo)
{
	ILScopeData *data;
	ILScope *namespaceScope;

	/* If we have a namespace and the identifier is a singleton,
	   then try looking in the namespace first */
	if(namespace && identifier &&
	   yykind(identifier) == yykindof(ILNode_Identifier))
	{
		namespaceScope = FindNamespaceScope(scope, namespace, 0);
		if(namespaceScope)
		{
			data = ILScopeLookup(namespaceScope,
						((ILNode_Identifier *)identifier)->name, 0);
			if(data)
			{
				if(data->rbnode.kind == IL_SCOPE_DECLARED_TYPE)
				{
					*classInfo = 0;
					*nodeInfo = data->node;
					return 1;
				}
				else if(data->rbnode.kind == IL_SCOPE_IMPORTED_TYPE)
				{
					*classInfo = (ILClass *)((ILScope *)(data->data))->userData;
					*nodeInfo = 0;
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}
	}

	/* See if we have a declaration for the identifier */
	data = ResolveQualIdent(scope, identifier);
	if(data)
	{
		if(data->rbnode.kind == IL_SCOPE_DECLARED_TYPE)
		{
			*classInfo = 0;
			*nodeInfo = data->node;
			return 1;
		}
		else if(data->rbnode.kind == IL_SCOPE_IMPORTED_TYPE)
		{
			*classInfo = (ILClass *)(((ILScope *)(data->data))->userData);
			*nodeInfo = 0;
			return 1;
		}
		else
		{
			return 0;
		}
	}

	/* This is not a declared or imported type.  It may be
	   a built-in "System" library type instead */
	if(identifier != 0 && yykind(identifier) == yykindof(ILNode_QualIdent))
	{
		ILNode_QualIdent *qident = (ILNode_QualIdent *)identifier;
		if(qident->left != 0 &&
		   yykind(qident->left) == yykindof(ILNode_Identifier))
		{
			*classInfo = ILClassLookup
					(ILClassGlobalScope(scope->info->libImage),
				     qident->name,
				     ((ILNode_Identifier *)(qident->left))->name);
			if((*classInfo) != 0)
			{
				/* Import the library type into the main image */
				*classInfo = ILClassImport(scope->info->image, *classInfo);
				if(!(*classInfo))
				{
					ILGenOutOfMemory(scope->info);
				}

				/* Return the details to the caller */
				*nodeInfo = 0;
				return 1;
			}
		}
	}

	/* Could not find the type */
	return 0;
}

int ILScopeDeclareMember(ILScope *scope, const char *name,
						 int memberKind, ILMember *member, ILNode *node)
{
	ILScopeData *data;

	/* Determine if there is a declaration for the name already */
	data = ILScopeLookup(scope, name, 0);
	if(data != 0)
	{
		if(data->rbnode.kind == IL_SCOPE_DECLARED_TYPE)
		{
			/* Declaration conflicts with a type the user already declared */
			return IL_SCOPE_ERROR_REDECLARED;
		}
		else if(data->rbnode.kind == IL_SCOPE_SUBSCOPE)
		{
			/* There is already a namespace with that name in existence */
			return IL_SCOPE_ERROR_NAME_IS_NAMESPACE;
		}
		else if(data->rbnode.kind == IL_SCOPE_IMPORTED_TYPE)
		{
			/* Conflict with an imported type */
			return IL_SCOPE_ERROR_IMPORT_CONFLICT;
		}
	}

	/* Add the member to the scope */
	AddToScope(scope, name, memberKind, node, member, 0);

	/* Done */
	return IL_SCOPE_ERROR_OK;
}

int ILScopeDeclareLocal(ILScope *scope, const char *name,
						unsigned long index, ILNode *node)
{
	ILScopeData *data;

	/* Determine if there is a declaration for the name already */
	data = ILScopeLookup(scope, name, 0);
	if(data != 0)
	{
		if(data->rbnode.kind == IL_SCOPE_DECLARED_TYPE)
		{
			/* Declaration conflicts with a type the user already declared */
			return IL_SCOPE_ERROR_REDECLARED;
		}
		else if(data->rbnode.kind == IL_SCOPE_SUBSCOPE)
		{
			/* There is already a namespace with that name in existence */
			return IL_SCOPE_ERROR_NAME_IS_NAMESPACE;
		}
		else if(data->rbnode.kind == IL_SCOPE_IMPORTED_TYPE)
		{
			/* Conflict with an imported type */
			return IL_SCOPE_ERROR_IMPORT_CONFLICT;
		}
		else
		{
			/* Something else is declared here */
			return IL_SCOPE_ERROR_OTHER;
		}
	}

	/* Add the local to the scope */
	AddToScope(scope, name, IL_SCOPE_LOCAL, node, (void *)index, 0);

	/* Done */
	return IL_SCOPE_ERROR_OK;
}

int ILScopeDeclareLocalConst(ILScope *scope, const char *name,
						ILNode *guarded, ILNode *node)
{
	ILScopeData *data;

	/* Determine if there is a declaration for the name already */
	data = ILScopeLookup(scope, name, 0);
	if(data != 0)
	{
		if(data->rbnode.kind == IL_SCOPE_DECLARED_TYPE)
		{
			/* Declaration conflicts with a type the user already declared */
			return IL_SCOPE_ERROR_REDECLARED;
		}
		else if(data->rbnode.kind == IL_SCOPE_SUBSCOPE)
		{
			/* There is already a namespace with that name in existence */
			return IL_SCOPE_ERROR_NAME_IS_NAMESPACE;
		}
		else if(data->rbnode.kind == IL_SCOPE_IMPORTED_TYPE)
		{
			/* Conflict with an imported type */
			return IL_SCOPE_ERROR_IMPORT_CONFLICT;
		}
		else
		{
			/* Something else is declared here */
			return IL_SCOPE_ERROR_OTHER;
		}
	}

	/* Add the local to the scope */
	AddToScope(scope, name, IL_SCOPE_LOCAL_CONST, node, (void *)guarded, 0);

	/* Done */
	return IL_SCOPE_ERROR_OK;
}

/* Declare an alias to do name => valueNode mapping */
int ILScopeDeclareAlias(ILScope *scope, const char *name,
						ILNode *node, ILNode *valueNode)
{
	ILScopeData *data;

	/* Determine if there is a declaration for the name already */
	data = ILScopeLookup(scope, name, 0);
	if(data != 0)
	{
		if(data->rbnode.kind == IL_SCOPE_DECLARED_TYPE)
		{
			/* Declaration conflicts with a type the user already declared */
			return IL_SCOPE_ERROR_REDECLARED;
		}
		else if(data->rbnode.kind == IL_SCOPE_SUBSCOPE)
		{
			/* There is already a namespace with that name in existence */
			return IL_SCOPE_ERROR_NAME_IS_NAMESPACE;
		}
		else if(data->rbnode.kind == IL_SCOPE_IMPORTED_TYPE)
		{
			/* Conflict with an imported type */
			return IL_SCOPE_ERROR_IMPORT_CONFLICT;
		}
		else
		{
			/* Something else is declared here */
			return IL_SCOPE_ERROR_OTHER;
		}
	}

	/* Add the local to the scope */
	AddToScope(scope, name, IL_SCOPE_ALIAS, node, (void*)valueNode, 0);

	/* Done */
	return IL_SCOPE_ERROR_OK;
}

int ILScopeDataGetKind(ILScopeData *data)
{
	return data->rbnode.kind;
}

ILNode *ILScopeDataGetNode(ILScopeData *data)
{
	return data->node;
}

ILClass *ILScopeDataGetClass(ILScopeData *data)
{
	if(data->rbnode.kind == IL_SCOPE_DECLARED_TYPE)
	{
		ILNode *node = data->node;
		return ((ILNode_ClassDefn *)node)->classInfo;
	}
	else if(data->rbnode.kind == IL_SCOPE_IMPORTED_TYPE)
	{
		return (ILClass *)(((ILScope *)(data->data))->userData);
	}
	else
	{
		return 0;
	}
}

const char *ILScopeDataGetNamespaceName(ILScopeData *data)
{
	if(data)
	{
		if(data->rbnode.kind == IL_SCOPE_SUBSCOPE)
		{
			return (const char *)(((ILScope *)(data->data))->userData);
		}
	}
	return 0;
}

ILScope *ILScopeDataGetSubScope(ILScopeData *data)
{
	return (ILScope *)(data->data);
}

ILMember *ILScopeDataGetMember(ILScopeData *data)
{
	return (ILMember *)(data->data);
}

unsigned long ILScopeDataGetIndex(ILScopeData *data)
{
	return (unsigned long)(data->data);
}

void *ILScopeDataGetData1(ILScopeData *data)
{
	return data->data;
}

void *ILScopeDataGetData2(ILScopeData *data)
{
	return data->data2;
}

void ILScopeDataModify(ILScopeData *data, int kind, ILNode *node,
					   void *data1, void *data2)
{
	data->rbnode.kind = kind;
	data->node = node;
	data->data = data1;
	data->data2 = data2;
}

ILScope *ILScopeGetParent(ILScope *scope)
{
	return scope->parent;
}

ILScopeData *ILScopeLocalsIter(ILScope *scope, ILScopeData *prev, int *iter,
							   unsigned long *index, const char **name)
{
	while((prev = (ILScopeData *) ILRBTreeIterNext(&(scope->nameTree),
												(ILRBTreeNode *)prev, iter)))
	{
		if(prev->rbnode.kind == IL_SCOPE_LOCAL)
		{
			*index = (unsigned long) prev->data;
			*name = prev->name;
			return prev;
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
