/*
 * java_gather.c - Type gathering operations for Java
 *
 * Copyright (C) 2001, 2008  Southern Storm Software, Pty Ltd.
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
#include <codegen/cg_nodemap.h>
/*
 * Scan all types and their nested children to declare them.
 */
static void DeclareTypes(ILGenInfo *info, ILScope *parentScope,
						 ILNode *tree, ILNode_List *list,
						 ILNode_ClassDefn *nestedParent)
{
	ILNode_ListIter iterator;
	ILNode *child;
	ILNode_ClassDefn *defn;
	ILScope *scope;
	ILNode *origDefn;
	const char *name;
	const char *package;
	int error;

	ILNode_ListIter_Init(&iterator, tree);
	while((child = ILNode_ListIter_Next(&iterator)) != 0)
	{
		if(yykind(child) == yykindof(ILNode_ClassDefn))
		{
			defn = (ILNode_ClassDefn *)child;
			defn->nestedParent = nestedParent;
			name = defn->name;
			package = defn->namespace;
			if(nestedParent || (package && *package == '\0'))
			{
				package = 0;
			}
			
			error = ILScopeDeclareType(parentScope, child,
								   	   name, package, &scope,
								   	   &origDefn,NULL);

			if(error != IL_SCOPE_ERROR_OK)
			{
				/* Could not declare the type in the global scope */
				switch(error)
				{
					case IL_SCOPE_ERROR_IMPORT_CONFLICT:
					{
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"`%s%s%s' conflicts with imported type",
								(package ? package : ""),
								(package ? "." : ""), name);
					}
					break;

					case IL_SCOPE_ERROR_REDECLARED:
					{
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"`%s%s%s' already declared",
								(package ? package : ""),
								(package ? "." : ""), name);
						CCErrorOnLine(yygetfilename(origDefn),
									  yygetlinenum(origDefn),
									  "previous declaration here");
					}
					break;

					case IL_SCOPE_ERROR_CANT_CREATE_NAMESPACE:
					{
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
									  "`%s' is not a valid package",
									  package);
					}
					break;

					case IL_SCOPE_ERROR_NAME_IS_NAMESPACE:
					{
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"`%s%s%s' cannot be declared as a type",
								(package ? package : ""),
								(package ? "." : ""), name);
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"because it is already declared as package");
					}
					break;

					default:
					{
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"`%s%s%s' cannot be declared as a type",
								(package ? package : ""),
								(package ? "." : ""), name);
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"because it is already declared elsewhere");
					}
					break;
				}
			}
			else
			{
				/* Declare nested types */
				DeclareTypes(info, scope, defn->body, list, defn);

				/* Replace the class body with a scoped body */
				defn->body = ILNode_ScopeChange_create(scope, defn->body);

				/* Add the type to the end of the new top-level list */
				if(!nestedParent)
				{
					ILNode_List_Add(list, child);
				}
			}
		}
	}
}

/*
 * Count the number of classes in a base class list.
 */
static int CountBaseClasses(ILNode *node)
{
	int count = 0;
	while(node != 0)
	{
		++count;
		if(yykind(node) != yykindof(ILNode_ArgList))
		{
			break;
		}
		node = ((ILNode_ArgList *)node)->expr1;
	}
	return count;
}

/*
 * Convert a class definition node into an ILClass value.
 */
static ILClass *NodeToClass(ILNode *node)
{
	if(node)
	{
		ILNode_ClassDefn *defn = (ILNode_ClassDefn *)node;
		if(defn->classInfo != ((ILClass *)1) &&
		   defn->classInfo != ((ILClass *)2))
		{
			return defn->classInfo;
		}
	}
	return 0;
}

/*
 * Find an interface member match in a particular interface.
 */
static ILMember *FindInterfaceMatch(ILClass *interface,
									const char *name,
									ILType *signature,
									int kind)
{
	ILMember *member = 0;
	while((member = ILClassNextMemberMatch
			(interface, member, kind, name, 0)) != 0)
	{
		if(kind == IL_META_MEMBERKIND_METHOD ||
		   kind == IL_META_MEMBERKIND_PROPERTY)
		{
			if(ILTypeIdentical(ILMember_Signature(member), signature))
			{
				return member;
			}
		}
		else if(kind == IL_META_MEMBERKIND_EVENT)
		{
			if(ILTypeIdentical(ILEvent_Type((ILEvent *)member), signature))
			{
				return member;
			}
		}
	}
	return 0;
}

/*
 * Find an interface member match in the interface parents
 * of a specified class.
 */
static ILMember *FindInterfaceMatchInParents(ILClass *classInfo,
											 const char *name,
											 ILType *signature,
											 int kind)
{
	ILImplements *impl = 0;
	ILMember *member;
	ILClass *interface;

	while((impl = ILClassNextImplements(classInfo, impl)) != 0)
	{
		interface = ILImplements_InterfaceClass(impl);
		member = FindInterfaceMatch(interface, name, signature, kind);
		if(member)
		{
			return member;
		}
		member = FindInterfaceMatchInParents(interface, name, signature, kind);
		if(member)
		{
			return member;
		}
	}
	return 0;
}

/*
 * Find the interface member that corresponds to a pariticular
 * member declaration.  Returns NULL if not found.
 */
static ILMember *FindInterfaceDecl(ILNode *node, ILClass *classInfo,
								   ILClass *interface, const char *name,
								   ILType *signature, int kind,
								   ILUInt32 *attrs)
{
	ILUInt32 newAttrs = *attrs;
	ILMember *member;

	/* Check the access modifiers */
	if(interface)
	{
		/* Explicit interface declaration */
		if((newAttrs & IL_META_METHODDEF_STATIC) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
			  "explicit interface member implementation cannot be `static'");
		}
		else if((newAttrs & IL_META_METHODDEF_ABSTRACT) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
			  "explicit interface member implementation cannot be `abstract'");
		}
		else if((newAttrs & IL_META_METHODDEF_VIRTUAL) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
			  "explicit interface member implementation cannot be `virtual'");
		}
		if((newAttrs & IL_META_METHODDEF_MEMBER_ACCESS_MASK) !=
				IL_META_METHODDEF_PRIVATE)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
			  "explicit interface member implementation must be `private'");
		}

		/* Set the correct attributes on the explicit implementation */
		newAttrs &= ~(IL_META_METHODDEF_MEMBER_ACCESS_MASK |
					  IL_META_METHODDEF_STATIC |
					  IL_META_METHODDEF_ABSTRACT );
		newAttrs |= IL_META_METHODDEF_PRIVATE |
					IL_META_METHODDEF_FINAL |
					IL_META_METHODDEF_VIRTUAL |
					IL_META_METHODDEF_NEW_SLOT;
	}
	else
	{
		/* Implicit interface declaration */
		if((newAttrs & IL_META_METHODDEF_STATIC) != 0)
		{
			/* Static members cannot implement interfaces */
			return 0;
		}
		if((newAttrs & IL_META_METHODDEF_VIRTUAL) != 0 &&
		   (newAttrs & IL_META_METHODDEF_NEW_SLOT) == 0)
		{
			/* "override" members do not implement interfaces:
			   the parent class's virtual method does */
			return 0;
		}
		if((newAttrs & IL_META_METHODDEF_MEMBER_ACCESS_MASK) !=
				IL_META_METHODDEF_PUBLIC)
		{
			/* Implicit interface mappings must be "public" */
			return 0;
		}

		/* Make sure that the final method is virtual */
		if((newAttrs & IL_META_METHODDEF_VIRTUAL) != 0)
		{
			newAttrs |= IL_META_METHODDEF_NEW_SLOT;
		}
		else
		{
			newAttrs |= IL_META_METHODDEF_VIRTUAL |
						IL_META_METHODDEF_NEW_SLOT |
						IL_META_METHODDEF_FINAL;
		}
	}

	/* Search for a match amongst the class's interfaces */
	if(interface)
	{
		member = FindInterfaceMatch(interface, name, signature, kind);
		if(!member)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "specified member is not present in `%s'",
						  JavaTypeToName(ILClassToType(interface)));
		}
	}
	else
	{
		member = FindInterfaceMatchInParents(classInfo, name, signature, kind);
	}

	/* Adjust the final attributes and return */
	if(member)
	{
		*attrs = newAttrs;
	}
	return member;
}
#if NOT_NEEDED || TODO
/*
 * Check if the class inherits java.lang.Object . Not all classes in
 * the java compiler needs to inherit java.lang.Object , some classes
 * like the Exceptions cannot :-( .
 */

static int ClassInheritsJavaObject(ILGenInfo *info,ILClass *classInfo)
{
	ILClass *javaObject=ILClassResolve(ILType_ToClass(
							ILFindNonSystemType(info,"Object","java.lang")));
	if(classInfo)
	{
		return ILClassInheritsFrom(classInfo, javaObject);
	}
	return 0;
}
#endif

/*
 * Create the program structure for a type and all of its base types.
 * Returns the new end of the top-level type list.
 */
static void CreateType(ILGenInfo *info, ILScope *globalScope,
					   ILNode_List *list, ILNode *systemObjectName,
					   ILNode *type)
{
	const char *name;
	const char *package;
	const char *baseName;
	int numBases;
	ILClass **baseList;
	int base;
	ILNode *baseNodeList;
	ILNode *baseNode;
	ILNode *baseTypeNode;
	ILClass *parent;
	ILClass *classInfo;
	int errorReported;
	ILNode_ClassDefn *defn;
	ILNode *savedNamespace;
	ILNode *savedClass;
	ILProgramItem *nestedScope;
	ILNode *node;
	ILNode_ListIter iter;

	/* Get the name and namespace for the type, for error reporting */
	defn = (ILNode_ClassDefn *)type;
	name = defn->name;
	package = defn->namespace;
	if(defn->nestedParent || (package && *package == '\0'))
	{
		package = 0;
	}

	/* If the type is already created, then bail out early */
	if(defn->classInfo != 0)
	{
		if(defn->classInfo == (ILClass *)2)
		{
			CCErrorOnLine(yygetfilename(defn), yygetlinenum(defn),
						  "`%s%s%s' is defined recursively",
						  (package ? package : ""),
						  (package ? "." : ""), name);
		}
		return;
	}

	/* Mark this type as already seen so that we can detect recursion */
	defn->classInfo = (ILClass *)2;

	/* If this is a nested type, then create its nesting parent first */
	if(defn->nestedParent)
	{
		/* this is a backward edge of the class dependency graph,
		 * since we'll be coming back to this very same class by
		 * defn->nestedParent as it's nested child or the forward
		 * edge , let's skip this loop,by returning here */
		if(defn->nestedParent->classInfo==0)
		{
			defn->classInfo=0;
			CreateType(info, globalScope, list,
				   systemObjectName, (ILNode *)(defn->nestedParent));
			return; 
		}
		nestedScope = (ILProgramItem *)(defn->nestedParent->classInfo);
		if(!nestedScope || nestedScope == (ILProgramItem *)1 ||
		   nestedScope == (ILProgramItem *)1)
		{
			nestedScope = ILClassGlobalScope(info->image);
		}
	}
	else
	{
		nestedScope = ILClassGlobalScope(info->image);
	}

	/* Set the namespace and class to use for resolving type names */
	savedNamespace = info->currentNamespace;
	info->currentNamespace = defn->namespaceNode;
	savedClass = info->currentClass;
	info->currentClass = (ILNode *)(defn->nestedParent);

	/* Create all of the base classes */
	numBases = CountBaseClasses(defn->baseClass);
	if(numBases > 0)
	{
		baseList = (ILClass **)ILCalloc(numBases, sizeof(ILClass *));
		if(!baseList)
		{
			CCOutOfMemory();
		}
	}
	else
	{
		baseList = 0;
	}
	baseNodeList = defn->baseClass;
	for(base = 0; base < numBases; ++base)
	{
		/* Get the name of the class to be inherited or implemented */
		if(yykind(baseNodeList) == yykindof(ILNode_ArgList))
		{
			baseNode = ((ILNode_ArgList *)baseNodeList)->expr2;
			baseNodeList = ((ILNode_ArgList *)baseNodeList)->expr1;
		}
		else
		{
			baseNode = baseNodeList;
		}

		if(yyisa(baseNode,ILNode_Identifier))
		{
			baseName=ILQualIdentName(baseNode,0);
		}
		else if(yyisa(baseNode,ILNode_QualIdent))
		{
			baseName=((ILNode_QualIdent*)baseNode)->name;
		}
		else
		{
			baseName=0;
		}
	

		/* Look in the scope for the base class */
		if(JavaSemBaseType(baseNode, info, &baseNode,
						 &baseTypeNode, &(baseList[base])))
		{
			if(baseList[base] == 0)
			{
				baseList[base] = NodeToClass(baseTypeNode);
				if(baseList[base] == 0)
				{
					CreateType(info, globalScope, list,
							   systemObjectName, baseTypeNode);
					baseList[base] = NodeToClass(baseTypeNode);
				}
			}
		}
		else
		{
			/* This is not a valid base class specification */
			CCErrorOnLine(yygetfilename(baseNode), yygetlinenum(baseNode),
						  "invalid base type");
		}
	}
	/* Find the parent class within the base list */
	parent = 0;
	errorReported = 0;
	for(base = 0; base < numBases; ++base)
	{
		if(baseList[base] && !ILClass_IsInterface(baseList[base]))
		{
			if(parent)
			{
				if(!errorReported)
				{
					CCErrorOnLine(yygetfilename(type), yygetlinenum(type),
					  "class inherits from two or more non-interface classes");
					errorReported = 1;
				}
			}
			else
			{
				parent = baseList[base];
			}
		}
	}

	/* Test for interfaces, or find "java.lang.Object" if no parent yet */
   	if((defn->modifiers & IL_META_TYPEDEF_CLASS_SEMANTICS_MASK)
	   		== IL_META_TYPEDEF_INTERFACE)
	{
		if(parent)
		{
			CCErrorOnLine(yygetfilename(type), yygetlinenum(type),
						  "interface inherits from non-interface class");
			parent = 0;
		}
	}
	else if(!parent)
	{
		if(!strcmp(name, "Object") && package != 0 &&
		   !strcmp(package, "java.lang"))
		{
			/* Special case: we are compiling "java.lang.Object" itself */
			parent = ILType_ToClass(ILFindNonSystemType(info,"ObjectWrapper",
										"dotgnu.javawrappers"));
		}
		else
		{
			/* Compiling something else that inherits "System.Object" */
			if(JavaSemBaseType(systemObjectName, info, &systemObjectName,
							 &baseTypeNode, &parent))
			{
				if(!parent)
				{
					parent = NodeToClass(baseTypeNode);
					if(!parent)
					{
						CreateType(info, globalScope, list,
								   systemObjectName, baseTypeNode);
						parent = NodeToClass(baseTypeNode);
					}
				}
			}
			else
			{
				/* Use the builtin library's "System.Object" */
				parent = ILType_ToClass(ILFindNonSystemType(info, "Object","java.lang"));
			}
		}
	}

	/* Restore the namespace and class */
	info->currentNamespace = savedNamespace;
	info->currentClass = savedClass;

	/* Output an error if attempting to inherit from a sealed class */
	if(parent && ILClass_IsSealed(parent))
	{
		CCErrorOnLine(yygetfilename(type), yygetlinenum(type),
					  "inheriting from a sealed parent class");
	}

	/* Create the class information block */
	classInfo = ILClassCreate(nestedScope, 0, name, package, ILToProgramItem(parent));
	if(!classInfo)
	{
		CCOutOfMemory();
	}
	ILClassSetAttrs(classInfo, ~0, defn->modifiers);
	defn->classInfo = classInfo;

	/* Add the interfaces to the class */
	for(base = 0; base < numBases; ++base)
	{
		if(baseList[base] && ILClass_IsInterface(baseList[base]))
		{
			if(!ILClassAddImplements(classInfo,
									 ILToProgramItem(baseList[base]), 0))
			{
				CCOutOfMemory();
			}
		}
	}

	/* Clean up */
	if(baseList)
	{
		ILFree(baseList);
	}

	/* Record the node on the class as user data */
	ILSetProgramItemMapping(info, (ILNode *)defn);

	/* Process the nested types */
	node = defn->body;
	if(node && yyisa(node, ILNode_ScopeChange))
	{
		node = ((ILNode_ScopeChange *)node)->body;
	}
	ILNode_ListIter_Init(&iter, node);
	while((node = ILNode_ListIter_Next(&iter)) != 0)
	{
		if(yyisa(node, ILNode_ClassDefn))
		{
			CreateType(info, globalScope, list, systemObjectName, node);
		}
	}

	/* Add the type to the new top-level list in create order */
	if(!(defn->nestedParent))
	{
		ILNode_List_Add(list, type);
	}
}

/*
 * Add a class member to a scope and report errors.
 */
static void AddMemberToScope(ILScope *scope, int memberKind,
							 const char *name, ILMember *member,
							 ILNode *node)
{
	int error = ILScopeDeclareMember(scope, name, memberKind, member, node);
	if(error != IL_SCOPE_ERROR_OK)
	{
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  "member conflicts with a type name in the same scope");
	}
}

/*
 * Search for a member with a specific name to do duplicate testing.
 */
static ILMember *FindMemberByName(ILClass *classInfo, const char *name,
								  ILClass *scope)
{
	ILMember *member;
	ILImplements *impl;
	while(classInfo != 0)
	{
		/* Scan the members of this class */
		member = 0;
		while((member = ILClassNextMemberMatch
				(classInfo, member, 0, name, 0)) != 0)
		{
			if(ILMemberAccessible(member, scope))
			{
				return member;
			}
		}

		/* Scan parent interfaces if this class is itself an interface */
		if(ILClass_IsInterface(classInfo))
		{
			impl = 0;
			while((impl = ILClassNextImplements(classInfo, impl)) != 0)
			{
				member = FindMemberByName(ILImplements_InterfaceClass(impl),
										  name, scope);
				if(member)
				{
					return member;
				}
			}
		}

		/* Move up to the parent of this class */
		classInfo = ILClass_ParentClass(classInfo);
	}
	return 0;
}

/*
 * Search for a member with a specific name and/or signature.
 */
static ILMember *FindMemberBySignature(ILClass *classInfo, const char *name,
									   ILType *signature, ILMember *notThis,
									   ILClass *scope, int interfaceOverride)
{
	ILMember *member;
	ILImplements *impl;
	int kind = ILMemberGetKind(notThis);

	while(classInfo != 0)
	{
		/* Scan the members of this class */
		member = 0;
		while((member = ILClassNextMemberMatch
				(classInfo, member, 0, name, 0)) != 0)
		{
			if(member != notThis &&
			   ILMemberAccessible(member, scope) &&
			   (!interfaceOverride || classInfo == scope))
			{
				if(ILMemberGetKind(member) != kind)
				{
					return member;
				}
				else if(JavaSignatureIdentical(ILMemberGetSignature(member),
										     signature))
				{
					return member;
				}
			}
		}

		/* Scan parent interfaces if this class is itself an interface */
		if(ILClass_IsInterface(classInfo))
		{
			impl = 0;
			while((impl = ILClassNextImplements(classInfo, impl)) != 0)
			{
				member = FindMemberBySignature
					(ILImplements_InterfaceClass(impl),
					 name, signature, notThis, scope, interfaceOverride);
				if(member)
				{
					return member;
				}
			}
		}

		/* Move up to the parent of this class */
		classInfo = ILClass_ParentClass(classInfo);
	}
	return 0;
}

/*
 * Report duplicate definitions.
 */
static void ReportDuplicates(ILNode *node, ILMember *newMember,
							 ILMember *existingMember, ILClass *classInfo,
							 ILUInt32 modifiers, const char *name)
{
	/* TODO: we need better error messages here */

	if(ILMember_Owner(existingMember) == classInfo)
	{
		/* The duplicate is in the same class */
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
		  			  "declaration of `%s' conflicts with an existing member",
					  name);
	}
	else
	{
		/* The duplicate is in a parent class, and "new" wasn't specified */
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
		  "declaration of `%s' hides an inherited member",name);
	}
}

static int MethodIsEntrypoint(ILGenInfo *info, ILMethod* method)
{
	ILType * sig;
	ILType *elemType , *type;
	ILUInt32 attrs=IL_META_METHODDEF_PUBLIC | IL_META_METHODDEF_STATIC |
				   IL_META_METHODDEF_HIDE_BY_SIG; 

	elemType=ILFindNonSystemType(info,"String" ,"java.lang");
	if(!elemType)
	{
		CCOutOfMemory();
		return 0;
	}

	type=ILTypeCreateArray(info->context,1,elemType);
	if(!type)
	{
		CCOutOfMemory();
		return 0;
	}
	
	sig=ILMethod_Signature(method);
	
	if(!strcmp(ILMethod_Name(method),"main") && 
	  ((ILMethod_Attrs(method) & attrs) == attrs) &&
	  (ILTypeGetReturn(sig) == ILType_Void) &&
	  (ILTypeNumParams(sig) == 1) &&
	  (ILTypeIdentical(ILTypeGetParam(sig,1),type)))
	  {
			return 1;
	  }
	return 0;
}

/*
 * Create a method definition.
 */
static void CreateMethod(ILGenInfo *info, ILClass *classInfo,
						 ILNode_MethodDeclaration *method)
{
	const char *name;
	const char *basicName;
	ILType *tempType;
	ILMethod *methodInfo;
	ILType *signature;
	ILNode_ListIter iterator;
	ILNode *param;
	ILNode_FormalParameter *fparam;
	ILUInt32 paramNum;
	ILUInt32 argListParam;
	ILParameter *parameter;
	ILMember *member;
	ILClass *interface;
	ILMember *interfaceMember;
	ILClass *class1, *class2;
	
	/* Get the name of the method, and the interface member (if any) */
	interface = 0;
	interfaceMember = 0;
	if(yykind(method->name) != yykindof(ILNode_Identifier))
	{
		CCError("Duh !");
	}
	/* Simple method name */
	name = ILQualIdentName(method->name, 0);
	basicName = name;
	/* (only simple names in Java) */

	methodInfo = 0;

	/* Create the method information block */
	if(!methodInfo)
	{
		methodInfo = ILMethodCreate(classInfo, 0, name,
									(method->modifiers & 0xFFFF));
		if(!methodInfo)
		{
			CCOutOfMemory();
		}
	}
	method->methodInfo = methodInfo;
	ILSetProgramItemMapping(info, (ILNode *)method);

	/* Get the return type */
	tempType = JavaSemTypeVoid(method->type, info, &(method->type));

	/* Create the method signature type */
	signature = ILTypeCreateMethod(info->context, tempType);
	if(!signature)
	{
		CCOutOfMemory();
	}
	if((method->modifiers & IL_META_METHODDEF_STATIC) == 0)
	{
		ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
		ILMethodSetCallConv(methodInfo, IL_META_CALLCONV_HASTHIS);
	}

	/* Create the parameters for the method */
	argListParam = 0;
	paramNum = 1;
	ILNode_ListIter_Init(&iterator, method->params);
	while((param = ILNode_ListIter_Next(&iterator)) != 0)
	{
		/* Get the type of the parameter */
		fparam = (ILNode_FormalParameter *)param;
		tempType = JavaSemType(fparam->type, info, &(fparam->type));

		/* Add the parameter type to the method signature */
		if(!ILTypeAddParam(info->context, signature, tempType))
		{
			CCOutOfMemory();
		}

		/* Create a parameter definition in the metadata to record the name */
		parameter = ILParameterCreate
				(methodInfo, 0, ILQualIdentName(fparam->name, 0), 0,
				 paramNum);
		if(!parameter)
		{
			CCOutOfMemory();
		}

		/* Advance to the next parameter */
		++paramNum;
	}

	/* Set the signature for the method */
	ILMemberSetSignature((ILMember *)methodInfo, signature);

	/* Add the method to the current scope */
	AddMemberToScope(info->currentScope, IL_SCOPE_METHOD,
					 name, (ILMember *)methodInfo, method->name);

	/* Process interface overrides */
	if(!ILClass_IsInterface(classInfo))
	{
		paramNum = method->modifiers;
		interfaceMember = FindInterfaceDecl
			((ILNode *)method, classInfo, interface,
			 basicName, signature, IL_META_MEMBERKIND_METHOD,
			 &paramNum);
		if(interfaceMember)
		{
			ILMemberSetAttrs((ILMember *)methodInfo, 0xFFFF,
							 (paramNum & 0xFFFF));
			if(interface)
			{
				/* Create an "ILOverride" block to associate the
				   explicit member implementation with the method
				   in the interface that it is implementing */
				interfaceMember = ILMemberImport(info->image, interfaceMember);
				if(!interfaceMember)
				{
					CCOutOfMemory();
				}
				if(!ILOverrideCreate(classInfo, 0,
									 (ILMethod *)interfaceMember,
									 methodInfo))
				{
					CCOutOfMemory();
				}
			}
		}
	}

	/* Look for duplicates and report on them */
	member = FindMemberBySignature(classInfo, name, signature,
								   (ILMember *)methodInfo, classInfo,
								   (interface != 0));
	if(member)
	{
		if(ILMember_IsMethod(member) &&
		   ILMethod_IsVirtual((ILMethod *)member) &&
		   !ILMethod_IsNewSlot(methodInfo))
		{
			/* Check for the correct form of virtual method overrides */
			if((method->modifiers & IL_META_METHODDEF_MEMBER_ACCESS_MASK)
			 !=	(ILMember_Attrs(member) & 
					 IL_META_METHODDEF_MEMBER_ACCESS_MASK))
			{
				class1=ILMember_Owner(member);
				class2=ILMethod_Owner(methodInfo);
				CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
					"cannot change the access modifiers while overriding "
					"method '%s%s%s.%s' with '%s%s%s.%s' ",
					ILClass_Namespace(class1) ? 
					ILClass_Namespace(class1) : "" ,
					ILClass_Namespace(class1) ? "." : "",
					ILClass_Name(class1),
					name,
					ILClass_Namespace(class2) ? 
						ILClass_Namespace(class2) : "" ,
					ILClass_Namespace(class2) ? "." : "",
					ILClass_Name(class2),
						name);
			}
		}
		else if(ILMember_Owner(member) == classInfo ||
		        (!ILMethodIsConstructor(methodInfo) &&
				 !ILMethodIsStaticConstructor(methodInfo)))
		{
			ReportDuplicates(method->name, (ILMember *)methodInfo,
							 member, classInfo, method->modifiers, name);
		}
	}
	else if(ILMethod_IsVirtual(methodInfo)) // if nothing overrides
	{
		ILMemberSetAttrs((ILMember*)methodInfo,0,IL_META_METHODDEF_NEW_SLOT);
	}
}

/*
 * Create a field definition.
 */
static void CreateField(ILGenInfo *info, ILClass *classInfo,
						ILNode_FieldDeclaration *field)
{
	ILNode_ListIter iterator;
	ILNode_FieldDeclarator *decl;
	ILField *fieldInfo;
	const char *name;
	ILType *tempType;
	ILType *modifier;
	ILMember *member;

	/* Get the field's type */
	tempType = JavaSemType(field->type, info, &(field->type));

	/* Add the "volatile" modifier if necessary */
	if((field->modifiers & JAVA_SPECIALATTR_VOLATILE) != 0)
	{
		modifier = ILFindNonSystemType(info, "IsVolatile",
									   "System.Runtime.CompilerServices");
		if(ILType_IsClass(modifier))
		{
			modifier = ILTypeCreateModifier(info->context, 0,
											IL_TYPE_COMPLEX_CMOD_REQD,
											ILType_ToClass(modifier));
			if(!modifier)
			{
				CCOutOfMemory();
			}
			tempType = ILTypeAddModifiers(info->context, modifier, tempType);
		}
	}

	/* Iterator over the field declarators and create each field in turn */
	ILNode_ListIter_Init(&iterator, field->fieldDeclarators);
	while((decl = (ILNode_FieldDeclarator *)
						ILNode_ListIter_Next(&iterator)) != 0)
	{
		ILType *suffixedType=tempType;

		/* Set the field's owner for later semantic analysis */
		decl->owner = field;

		if(yyisa(decl->name,ILNode_TypeSuffixDeclarator))
		{
			int i;
			ILNode_TypeSuffixDeclarator *suffix=
							(ILNode_TypeSuffixDeclarator*)	decl->name;
			for(i=0;i<suffix->dims;i++)
			{
				suffixedType=ILTypeCreateArray(info->context,1,
										suffixedType);
			}
			decl->name=suffix->name;
		}
			
		/* Get the name of the field */
		name = ILQualIdentName(decl->name, 0);

		/* Look for duplicates */
		member = FindMemberByName(classInfo, name, classInfo);

		/* Create the field information block */
		fieldInfo = ILFieldCreate(classInfo, 0, name,
								  (field->modifiers & 0xFFFF));
		if(!fieldInfo)
		{
			CCOutOfMemory();
		}
		decl->fieldInfo = fieldInfo;
		ILMemberSetSignature((ILMember *)fieldInfo, suffixedType);
		ILSetProgramItemMapping(info, (ILNode *)decl);

		/* Report on duplicates */
		if(member)
		{
			ReportDuplicates(decl->name, (ILMember *)fieldInfo,
							 member, classInfo, field->modifiers, name);
		}

		/* Add the field to the current scope */
		AddMemberToScope(info->currentScope, IL_SCOPE_FIELD,
						 name, (ILMember *)fieldInfo, decl->name);
	}
}

/*
 * Create the members of a class node.
 */
static void CreateMembers(ILGenInfo *info, ILScope *globalScope,
						  ILNode *classNode)
{
	ILClass *classInfo;
	ILNode *body;
	ILScope *scope;
	ILScope *savedScope;
	ILNode *savedClass;
	ILNode *savedNamespace;
	ILNode_ListIter iterator;
	ILNode *member;
	
	ILNode *helperMain=NULL;

	/* Get the class information block, and bail out if not defined */
	classInfo = ((ILNode_ClassDefn *)classNode)->classInfo;
	if(!classInfo || classInfo == ((ILClass *)1) ||
	   classInfo == ((ILClass *)2))
	{
		return;
	}

	/* Get the class body and the scope it is declared within */
	body = ((ILNode_ClassDefn *)classNode)->body;
	if(body && yykind(body) == yykindof(ILNode_ScopeChange))
	{
		scope = ((ILNode_ScopeChange *)body)->scope;
		body = ((ILNode_ScopeChange *)body)->body;
	}
	else
	{
		scope = globalScope;
	}

	/* Set the new scope for use by the semantic analysis routines */
	savedScope = info->currentScope;
	info->currentScope = scope;
	savedClass = info->currentClass;
	info->currentClass = classNode;
	savedNamespace = info->currentNamespace;
	info->currentNamespace = ((ILNode_ClassDefn *)classNode)->namespaceNode;

	/* Iterate over the member definitions in the class body */
	ILNode_ListIter_Init(&iterator, body);
	while((member = ILNode_ListIter_Next(&iterator)) != 0)
	{
		if(yykind(member) == yykindof(ILNode_FieldDeclaration))
		{
			CreateField(info, classInfo, (ILNode_FieldDeclaration *)member);
		}
		else if(yykind(member) == yykindof(ILNode_MethodDeclaration))
		{
			CreateMethod(info, classInfo,
						 (ILNode_MethodDeclaration *)member);
			if(MethodIsEntrypoint(info,
					((ILNode_MethodDeclaration*)member)->methodInfo))
			{
				ILNode *list=ILNode_List_create();
				ILNode *helperBody;
		
				ILNode_List_Add(list,ILNode_FormalParameter_create(NULL,
						ILParamMod_empty,ILNode_JArrayType_create(
						ILNode_ILSystemType_create("String"),1),
						ILQualIdentSimple(ILInternString("args",4).string)));
		
				helperBody=ILNode_JMain_create(
							(((ILNode_MethodDeclaration*)member)->methodInfo));

				helperMain=ILNode_MethodDeclaration_create(0,
											IL_META_METHODDEF_PUBLIC |
											IL_META_METHODDEF_STATIC |
											IL_META_METHODDEF_HIDE_BY_SIG,
											0,
											ILQualIdentSimple("Main"),
											0,
											list,
											helperBody);
			}
		}
		else if(yykind(member) == yykindof(ILNode_ClassDefn))
		{
			CreateMembers(info, globalScope, member);
		}
		else
		{
			CCErrorOnLine(yygetfilename(member), yygetlinenum(member),
				  "internal error - do not know how to declare this member");
		}
	}
		
	if(helperMain)
	{
		ILNode_List_Add(body,helperMain);
		CreateMethod(info,classInfo,(ILNode_MethodDeclaration*)helperMain);
	}

	/* Return to the original scope */
	info->currentScope = savedScope;
	info->currentClass = savedClass;
	info->currentNamespace = savedNamespace;
}


ILNode *JavaTypeGather(ILGenInfo *info, ILScope *globalScope, ILNode *tree)
{
	ILNode_ListIter iterator;
	ILNode *child;
	ILNode_List *list;
	ILNode *systemObject;

	/* Create a new top-level list for the program */
	list = (ILNode_List *)ILNode_List_create();

	/* Scan all top-level types to declare them */
	DeclareTypes(info, globalScope, tree, list, 0);

	/* Create the top-level types, and re-order them so that the
	   base types are listed before types that inherit them */
	tree = (ILNode *)list;
	list = (ILNode_List *)ILNode_List_create();
	systemObject = ILNode_JSystemType_create("Object");

	ILNode_ListIter_Init(&iterator, tree);
	while((child = ILNode_ListIter_Next(&iterator)) != 0)
	{
		CreateType(info, globalScope, list, systemObject, child);
	}

	/* Create the class members within each type */
	ILNode_ListIter_Init(&iterator, list);
	while((child = ILNode_ListIter_Next(&iterator)) != 0)
	{
		CreateMembers(info, globalScope, child);
	}

	return (ILNode*) list;
}
