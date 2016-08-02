/*
 * c_scope.c - Scope handling for the C programming language.
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

#include <cscc/c/c_internal.h>

#ifdef	__cplusplus
extern	"C" {
#endif

ILScope *CCurrentScope;
ILScope *CGlobalScope;
static ILIntString CurrNamespace = {"", 0};
static const char **usingScopes = 0;
static int numUsingScopes = 0;

/*
 * Make a scope name for a struct or union.
 */
static const char *StructScopeName(const char *name, int structKind)
{
	ILIntString str1;
	ILIntString str2;
	if(structKind == C_STKIND_STRUCT)
	{
		str1.string = "struct ";
		str1.len = 7;
	}
	else
	{
		str1.string = "union ";
		str1.len = 6;
	}
	str2.string = (char *)name;
	str2.len = strlen(name);
	return (ILInternAppendedString(str1, str2)).string;
}

/*
 * Make a scope name for an enum.
 */
static const char *EnumScopeName(const char *name)
{
	ILIntString str1;
	ILIntString str2;
	str1.string = "enum ";
	str1.len = 5;
	str2.string = (char *)name;
	str2.len = strlen(name);
	return (ILInternAppendedString(str1, str2)).string;
}

/*
 * Convert a node into a persistent value that will survive
 * the treecc scope being flushed with "yynodepop()".
 */
static ILNode *PersistNode(ILNode *node)
{
	ILNode *newNode = (ILNode *)ILMalloc(sizeof(ILNode));
	if(!newNode)
	{
		CCOutOfMemory();
	}
	*newNode = *node;
	return newNode;
}

void CScopeGlobalInit(ILGenInfo *info)
{
	CGlobalScope = CCurrentScope = ILScopeCreate(info, 0);
}

/*
 * Find a type in a specific namespace, and then register a scope entry for it.
 */
static ILScopeData *FindInNamespace(ILScope *scope, const char *name,
									const char *namespace)
{
	ILType *type;

	/* Search for an actual type in the namespace */
	type = ILFindNonSystemType(&CCCodeGen, name, namespace);
	if(!type)
	{
		return 0;
	}

	/* Convert builtin classes to their primitive forms */
	type = ILClassToType(ILTypeToClass(&CCCodeGen, type));
	if(!type)
	{
		return 0;
	}

	/* Add an entry to the scope for next time */
	ILScopeDeclareItem(scope, name, C_SCDATA_TYPEDEF, 0, 0, type);
	return ILScopeLookup(scope, name, 0);
}

void *CScopeLookup(const char *name)
{
	ILScopeData *data;
	int using;

	/* If we are pushed into a namespace, then use that as the scope */
	if(CurrNamespace.len > 0)
	{
		/* See if we already have an entry in this namespace */
		data = ILScopeLookupInNamespace
			(CGlobalScope, CurrNamespace.string, name);
		if(data)
		{
			return (void *)data;
		}

		/* Find the type and add it */
		return (void *)FindInNamespace
			(ILScopeFindNamespace(CGlobalScope, CurrNamespace.string),
			 name, CurrNamespace.string);
	}

	/* Use the normal lookup rules to find the name */
	data = ILScopeLookup(CCurrentScope, name, 1);
	if(data)
	{
		return (void *)data;
	}

	/* Look in the "using" namespaces for a type */
	for(using = 0; using < numUsingScopes; ++using)
	{
		data = FindInNamespace(CGlobalScope, name, usingScopes[using]);
		if(data)
		{
			return (void *)data;
		}
	}

	/* We were unable to find the identifier */
	return 0;
}

void *CScopeLookupCurrent(const char *name)
{
	return (void *)ILScopeLookup(CCurrentScope, name, 0);
}

int CScopeIsTypedef(const char *name)
{
	void *data = CScopeLookup(name);
	if(data != 0)
	{
		return (CScopeGetKind(data) == C_SCDATA_TYPEDEF);
	}
	else
	{
		return 0;
	}
}

int CScopeIsNamespace(const char *name)
{
	void *data = CScopeLookup(name);
	if(data != 0)
	{
		return (CScopeGetKind(data) == IL_SCOPE_SUBSCOPE);
	}
	else
	{
		return 0;
	}
}

void *CScopeLookupStructOrUnion(const char *name, int structKind)
{
	return CScopeLookup(StructScopeName(name, structKind));
}

int CScopeHasStructOrUnion(const char *name, int structKind)
{
	return (ILScopeLookup
				(CCurrentScope, StructScopeName(name, structKind), 0) != 0);
}

void CScopeAddStructOrUnion(const char *name, int structKind, ILType *type)
{
	ILScopeDeclareItem(CCurrentScope, StructScopeName(name, structKind),
					   C_SCDATA_STRUCT_OR_UNION, 0, 0, type);
}

void *CScopeLookupEnum(const char *name)
{
	return CScopeLookup(EnumScopeName(name));
}

int CScopeHasEnum(const char *name)
{
	return (ILScopeLookup(CCurrentScope, EnumScopeName(name), 0) != 0);
}

void CScopeAddEnum(const char *name, ILType *type)
{
	ILScopeDeclareItem(CCurrentScope, EnumScopeName(name), C_SCDATA_ENUM,
					   0, 0, type);
}

void CScopeAddEnumConst(const char *name, ILNode *node,
						ILInt32 value, ILType *type)
{
	ILScopeDeclareItem(CCurrentScope, name, C_SCDATA_ENUM_CONSTANT,
					   PersistNode(node), (void *)(ILNativeInt)value, type);
}

void CScopeAddTypedef(const char *name, ILType *type, ILNode *node)
{
	ILScopeDeclareItem(CCurrentScope, name,
					   C_SCDATA_TYPEDEF, PersistNode(node), 0, type);
}

void CScopeAddFunction(const char *name, ILNode *node, ILType *signature)
{
	ILScopeDeclareItem(CGlobalScope, name,
					   C_SCDATA_FUNCTION, PersistNode(node), 0, signature);
}

void CScopeAddFunctionForward(const char *name, int kind,
							  ILNode *node, ILType *signature)
{
	ILScopeDeclareItem(CGlobalScope, name, kind, PersistNode(node),
					   0, signature);
}

void CScopeAddInferredFunction(const char *name, ILType *signature)
{
	ILScopeDeclareItem(CGlobalScope, name,
					   C_SCDATA_FUNCTION_INFERRED, 0, 0, signature);
}

void CScopeUpdateFunction(void *data, int kind, ILNode *node, ILType *signature)
{
	ILScopeDataModify((ILScopeData *)data, kind, PersistNode(node),
					  0, signature);
}

void CScopeAddParam(const char *name, unsigned index, ILType *type)
{
	ILScopeDeclareItem(CCurrentScope, name,
					   C_SCDATA_PARAMETER_VAR, 0,
					   (void *)(ILNativeUInt)index, type);
}

void CScopeAddLocal(const char *name, ILNode *node,
					unsigned index, ILType *type)
{
	ILScopeDeclareItem(CCurrentScope, name,
					   C_SCDATA_LOCAL_VAR, PersistNode(node),
					   (void *)(ILNativeUInt)index, type);
}

void CScopeAddGlobal(const char *name, ILNode *node, ILType *type)
{
	ILScopeDeclareItem(CGlobalScope, name,
					   C_SCDATA_GLOBAL_VAR, PersistNode(node), 0, type);
}

void CScopeAddGlobalForward(const char *name, ILNode *node, ILType *type)
{
	ILScopeDeclareItem(CGlobalScope, name,
					   C_SCDATA_GLOBAL_VAR_FORWARD,
					   PersistNode(node), 0, type);
}

void CScopeUpdateGlobal(void *data, int kind, ILNode *node, ILType *type)
{
	ILScopeDataModify((ILScopeData *)data, kind,
					  PersistNode(node), 0, type);
}

void CScopeAddUndeclared(const char *name)
{
	ILScopeDeclareItem(CGlobalScope, name, C_SCDATA_UNDECLARED, 0, 0, 0);
}

void CScopeUsingNamespace(const char *name)
{
	const char *interned = (ILInternString((char *)name, -1)).string;
	int using;
	ILScopeDeclareNamespace(CGlobalScope, interned);
	for(using = 0; using < numUsingScopes; ++using)
	{
		if(usingScopes[using] == interned)
		{
			/* We already have this namespace */
			return;
		}
	}
	if((usingScopes = (const char **)ILRealloc
			(usingScopes, (numUsingScopes + 1) * sizeof(char *))) == 0)
	{
		CCOutOfMemory();
	}
	usingScopes[numUsingScopes++] = interned;
}

void CScopePushNamespace(const char *name)
{
	if(CurrNamespace.len != 0)
	{
		CurrNamespace = ILInternAppendedString
			(CurrNamespace,
			 ILInternAppendedString
			 	(ILInternString(".", 1), ILInternString(name, -1)));
	}
	else
	{
		CurrNamespace = ILInternString(name, -1);
	}
}

void CScopePopNamespace(const char *name)
{
	if(CurrNamespace.len == strlen(name))
	{
		CurrNamespace = ILInternString("", 0);
	}
	else
	{
		CurrNamespace = ILInternString
			(CurrNamespace.string, CurrNamespace.len - strlen(name) - 1);
	}
}

int CScopeGetKind(void *data)
{
	return ILScopeDataGetKind((ILScopeData *)data);
}

ILType *CScopeGetType(void *data)
{
	return (ILType *)(ILScopeDataGetData2((ILScopeData *)data));
}

ILNode *CScopeGetNode(void *data)
{
	return ILScopeDataGetNode((ILScopeData *)data);
}

unsigned CScopeGetIndex(void *data)
{
	return (unsigned)(ILNativeUInt)(ILScopeDataGetData1((ILScopeData *)data));
}

ILInt32 CScopeGetEnumConst(void *data)
{
	return (ILInt32)(ILNativeInt)(ILScopeDataGetData1((ILScopeData *)data));
}

#ifdef	__cplusplus
};
#endif
