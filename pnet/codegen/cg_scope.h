/*
 * cg_scope.h - Scope handling declarations.
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

#ifndef	_CODEGEN_CG_SCOPE_H
#define	_CODEGEN_CG_SCOPE_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Kinds of items that may appear in a scope attached to a name.
 */
#define	IL_SCOPE_SUBSCOPE		1	/* Named sub-scope */
#define	IL_SCOPE_LINKED_SCOPE	2	/* Sub-scope linked with "using" */
#define	IL_SCOPE_IMPORTED_TYPE	3	/* Scope is an imported type */
#define	IL_SCOPE_DECLARED_TYPE	4	/* Scope is a declared type */
#define	IL_SCOPE_METHOD			5	/* Item is a method */
#define	IL_SCOPE_FIELD			6	/* Item is a field */
#define	IL_SCOPE_PROPERTY		7	/* Item is a property */
#define	IL_SCOPE_EVENT			8	/* Item is an event */
#define	IL_SCOPE_LOCAL			9	/* Item is a local variable */
#define	IL_SCOPE_LOCAL_CONST	10	/* Item is a local variable */
#define	IL_SCOPE_ALIAS			11	/* Item is a local variable */
#define	IL_SCOPE_DUMMY			12	/* Used internally */

/*
 * Error codes for scope definitions.
 */
#define	IL_SCOPE_ERROR_OK						0
#define	IL_SCOPE_ERROR_IMPORT_CONFLICT			1
#define	IL_SCOPE_ERROR_REDECLARED				2
#define	IL_SCOPE_ERROR_CANT_CREATE_NAMESPACE	3
#define	IL_SCOPE_ERROR_NAME_IS_NAMESPACE		4
#define	IL_SCOPE_ERROR_OTHER					5

/*
 * Initialize the memory pools used by the scope routines.
 */
void ILScopeInit(ILGenInfo *info);

/*
 * Create a new scope underneath a specific parent scope.
 * If "parent" is NULL, then create a global scope.
 */
ILScope *ILScopeCreate(ILGenInfo *info, ILScope *parent);

/*
 * Import the contents of an IL binary image into a scope.
 */
void ILScopeImport(ILScope *scope, ILImage *image);

/*
 * Add a "using" declaration to a specific scope.  Returns zero
 * if there is something already declared at the identifier which
 * is not a namespace.
 */
int ILScopeUsing(ILScope *scope, const char *identifier);

/*
 * Clear the "using" declarations from a specific scope.
 */
void ILScopeClearUsing(ILScope *scope);

/*
 * Look up an identifier within a specific scope.
 * If "up" is non-zero, then go up to the parent
 * scope if not in the current scope.  Returns
 * NULL if there is no such identifier in use.
 */
ILScopeData *ILScopeLookup(ILScope *scope, const char *identifier, int up);

/*
 * Look up an identifier within a specific namespace.
 */
ILScopeData *ILScopeLookupInNamespace(ILScope *globalScope,
									  const char *namespace,
									  const char *identifier);

/*
 * Get the next item associated with a name within a scope.
 * Returns NULL if no more items with the same name.
 */
ILScopeData *ILScopeNextItem(ILScopeData *data);

/*
 * Declare an explicit item in a scope.
 */
void ILScopeDeclareItem(ILScope *scope, const char *name, int kind,
						ILNode *node, void *data1, void *data2);

/*
 * Declare a namespace within a scope and return the scope associated with
 * that namespace.
 */
ILScope *ILScopeDeclareNamespace(ILScope *globalScope, const char *namespace);

/*
 * Find the scope associated with a namespace.
 */
ILScope *ILScopeFindNamespace(ILScope *globalScope, const char *namespace);

/*
 * Find a namespace in the imported libraries and declare the namespace
 * in the global scope if present.
 */
ILScope *ILScopeImportNamespace(ILScope *scope, const char *namespace);

/*
 * Get the fully qualified namespace name from the scope.
 * The result is guaranteed to be valid only for scopes returned by
 * ILScopeDeclareNamespace, ILScopeFindNamespace and ILScopeImportNamespace.
 */
const char *ILScopeGetNamespaceName(ILScope *scope);

/*
 * Declare a type within a particular scope.  If the name
 * already exists, then an "IL_SCOPE_ERROR_xxx" code is
 * returned.  If there is a declaration for the type already,
 * then the node will be returned in "origDefn". The attach
 * scope is added between the scope and resultScope scopes.
 * scope->..attachScope->resultScope
 */
int ILScopeDeclareType(ILScope *scope, ILNode *node, const char *name,
					   const char *namespace, ILScope **resultScope,
					   ILNode **origDefn, ILScope *attachScope);

/*
 * Resolve a type identifier to an "ILClass *" record or to
 * an "ILNode *" parse tree node.  Returns zero if not found.
 */
int ILScopeResolveType(ILScope *scope, ILNode *identifier,
					   const char *namespace, ILClass **classInfo,
					   ILNode **nodeInfo);

/*
 * Declare a class member within a particular scope.
 * Returns a scope error code if already declared.
 */
int ILScopeDeclareMember(ILScope *scope, const char *name,
						 int memberKind, ILMember *member, ILNode *node);

/*
 * Declare a local variable within a particular scope.
 * Returns a scope error code if already declared.
 */
int ILScopeDeclareLocal(ILScope *scope, const char *name,
						unsigned long index, ILNode *node);

/* Declare a local const variable within a particular scope.
 * Returns a scope error code if already declared.
 */
int ILScopeDeclareLocalConst(ILScope *scope, const char *name,
						ILNode *guarded, ILNode *node);

/* Declare an alias in a particular scope, this returns a scope
 * error if already declared.
 */
int ILScopeDeclareAlias(ILScope *scope, const char *name,
						ILNode *node, ILNode *valueNode);
/*
 * Get the kind value associated with a scope item.
 */
int ILScopeDataGetKind(ILScopeData *data);

/*
 * Get the node associated with a scope item.
 */
ILNode *ILScopeDataGetNode(ILScopeData *data);

/*
 * Get the class structure associated with a scope item.
 */
ILClass *ILScopeDataGetClass(ILScopeData *data);

/*
 * Get the fully qualified namespace name associated with a scope item.
 */
const char *ILScopeDataGetNamespaceName(ILScopeData *data);

/*
 * Get the sub-scope structure associated with a scope item.
 */
ILScope *ILScopeDataGetSubScope(ILScopeData *data);

/*
 * Get the member structure associated with a scope item.
 */
ILMember *ILScopeDataGetMember(ILScopeData *data);

/*
 * Get the index of a local variable scope item.
 */
unsigned long ILScopeDataGetIndex(ILScopeData *data);

/*
 * Get the "data1" value from a scope item.
 */
void *ILScopeDataGetData1(ILScopeData *data);

/*
 * Get the "data2" value from a scope item.
 */
void *ILScopeDataGetData2(ILScopeData *data);

/*
 * Modify the contents of a scope item.
 */
void ILScopeDataModify(ILScopeData *data, int kind, ILNode *node,
					   void *data1, void *data2);

/*
 * Get the parent of a scope.
 */
ILScope *ILScopeGetParent(ILScope *scope);

/*
 * Return next local variable in scope.
 * Call with prev=NULL to begin iteration.
 */
ILScopeData *ILScopeLocalsIter(ILScope *scope, ILScopeData *prev, int *iter,
							   unsigned long *index, const char **name);


#ifdef	__cplusplus
};
#endif

#endif	/* _CODEGEN_CG_SCOPE_H */
