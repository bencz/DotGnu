/*
 * cs_gather.c - "Type gathering" support for the C# compiler.
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

#include "cs_internal.h"
#include <codegen/cg_nodemap.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*

Type gathering is a three-phase pass that happens just before semantic
analysis.

The first phase collects up the names of all of the types that are
declared in the program, as opposed to types declared in libraries.

The second phase creates the ILClass type structures to represent the
entire type hierarchy of the program.  During this process, we check
that all types have been declared correctly.

The third phase creates the fields, methods, etc that appear within
each class.  The types of parameters and return values are checked,
but duplicates and access permissions are not.  That is left until
semantic analysis.

*/

/*
 * Clone the filename/linenum information from one node to another.
 */
static void CloneLine(ILNode *dest, ILNode *src)
{
	yysetfilename(dest, yygetfilename(src));
	yysetlinenum(dest, yygetlinenum(src));
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
 * Get the total number of base classes for a class definition
 */
static int NumBases(ILNode_ClassDefn *defn)
{
	int count;

	count = CountBaseClasses(defn->baseClass);
#if IL_VERSION_MAJOR > 1
	if(defn->otherParts)
	{
		ILNode *otherPart;
		ILNode_ListIter iter;

		ILNode_ListIter_Init(&iter, defn->otherParts);
		while((otherPart = ILNode_ListIter_Next(&iter)) != 0)
		{
			if(yyisa(otherPart, ILNode_ClassDefn))
			{
				count += CountBaseClasses(((ILNode_ClassDefn *)otherPart)->baseClass);
			}
		}
	}
#endif /* IL_VERSION_MAJOR > 1 */

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

#define NodeToProgramItem(node)	ILToProgramItem(NodeToClass(node))

/*
 * Get the member visibility for the modifiers.
 */
static ILUInt32 GetMemberVisibilityFromModifiers(ILUInt32 modifiers)
{
	switch(modifiers & CS_MODIFIER_ACCESS_MASK)
	{
		case CS_MODIFIER_PUBLIC:
		{
			return IL_META_METHODDEF_PUBLIC;
		}
		break;

		case CS_MODIFIER_PRIVATE:
		{
			return IL_META_METHODDEF_PRIVATE;
		}
		break;

		case CS_MODIFIER_PROTECTED:
		{
			return IL_META_METHODDEF_FAMILY;
		}
		break;

		case CS_MODIFIER_INTERNAL:
		{
			return IL_META_METHODDEF_ASSEM;
		}
		break;

		case (CS_MODIFIER_PROTECTED | CS_MODIFIER_INTERNAL):
		{
			return IL_META_METHODDEF_FAM_OR_ASSEM;
		}
		break;
	}
	return IL_META_METHODDEF_PRIVATE;
}

/*
 * Adjust the name of a property to include a "get_" or "set_" prefix.
 */
static ILNode *PrefixName(ILNode *name, char *prefix)
{
	ILNode *node;
	if(yykind(name) == yykindof(ILNode_Identifier))
	{
		/* Simple name: just add the prefix */
		node = ILQualIdentSimple
					(ILInternAppendedString
						(ILInternString(prefix, strlen(prefix)),
						 ILInternString(ILQualIdentName(name, 0), -1)).string);
		CloneLine(node, name);
		return node;
	}
	else if(yykind(name) == yykindof(ILNode_QualIdent))
	{
		/* Qualified name: add the prefix to the second component */
		node = ILNode_QualIdent_create(((ILNode_QualIdent *)name)->left,
					(ILInternAppendedString
						(ILInternString(prefix, strlen(prefix)),
						 ILInternString(((ILNode_QualIdent *)name)->name, -1)).string));
		CloneLine(node, name);
		return node;
	}
	else
	{
		/* Shouldn't happen */
		return name;
	}
}

/*
 * Get the full and basic names from a method/property/event name.
 */
static const char *GetFullAndBasicNames(ILNode *name, const char **basicName)
{
	/*
	 * TODO: This doesn't work with explicit implementations of 
	 * generic interface methods.
	 */
	const char *result;
	const char *basic;
	const char *left;

	if(yyisa(name, ILNode_Identifier))
	{
		result = ILQualIdentGetName(name);
		basic = result;
	}
	else if(yyisa(name, ILNode_GenericQualIdent))
	{
		char buffer[261];

		result = ((ILNode_GenericQualIdent *)name)->name;
		sprintf(buffer, "%s`%i", result, ((ILNode_GenericQualIdent *)name)->numTypeArgs);
		if(((ILNode_GenericQualIdent *)name)->left)
		{
			left = GetFullAndBasicNames(((ILNode_GenericQualIdent *)name)->left, 0);
			result = ILQualIdentAppend(left, buffer);
		}
		basic = result;
	}
	else if(yyisa(name, ILNode_QualIdent))
	{
		left = GetFullAndBasicNames(((ILNode_QualIdent *)name)->left, 0);
		result = ILQualIdentAppend(left, ((ILNode_QualIdent *)name)->name);
		basic = ((ILNode_QualIdent *)name)->name;
	}
	else
	{
		/* Shouldn't happen, but do something safe */
		CCErrorOnLine(yygetfilename(name), yygetlinenum(name),
					  _("invalid qualified identifier"));
		result = basic = "x";
	}
	if(basicName)
	{
		*basicName = basic;
	}
	return result;
}

#if IL_VERSION_MAJOR > 1
/*
 * Add the generic type contraints to the generic type parameter.
 */
static void AddGenericConstraints(ILGenInfo *info,
								  ILGenericPar *genPar,
								  ILNode_List *constraints)
{
	if(constraints)
	{
		ILNode_ListIter iter;
		ILNode *constraint;

		ILNode_ListIter_Init(&iter, (ILNode *)constraints);
		while((constraint = ILNode_ListIter_Next(&iter)) != 0)
		{
			ILType *constraintType;

			constraintType = CSSemType(constraint, info, &constraint);
			if(constraintType)
			{
				ILClass *constraintClass = ILClassFromType(info->image, 0, constraintType, 0);

				if(constraintClass)
				{
					if(!ILGenericParAddConstraint(genPar, 0, 
										ILToProgramItem(constraintClass)))
					{
						CCOutOfMemory();
					}
				}
			}
		}
	}
}

/*
 * Add the given generic parameters to the program item.
 */
static void AddTypeFormals(ILGenInfo *info,
						   ILProgramItem *owner,
						   ILNode *typeFormals)
{
	if(owner && typeFormals)
	{
		ILNode_ListIter iter;
		ILNode_GenericTypeParameter *genParam;
		ILNode_GenericTypeParameters *genParams;
		ILMethod *method = ILProgramItemToMethod(owner);

		genParams = (ILNode_GenericTypeParameters *)typeFormals;
		ILNode_ListIter_Init(&iter, genParams->typeParams);
		while((genParam = 
			(ILNode_GenericTypeParameter *)ILNode_ListIter_Next(&iter)) != 0)
		{
			ILGenericPar *genPar = ILGenericParCreate(info->image, 0,
													  owner, genParam->num);
			if(!genPar)
			{
				CCOutOfMemory();
			}
			if(!ILGenericParSetName(genPar, genParam->name))
			{
				CCOutOfMemory();
			}
			ILGenericParSetFlags(genPar, IL_MAX_UINT32, genParam->constraint);
			AddGenericConstraints(info, genPar, genParam->typeConstraints);
			genParam->genPar = genPar;
			if(method)
			{
				genParam->target = ILGenParamTarget_Method;
			}
		}
	}
}

static void _AddTypeFormalWithCheck(ILGenInfo *info,
									ILProgramItem *owner,
									ILNode_GenericTypeParameter *genParam,
									ILUInt32 offset,
									ILUInt32 *overridden)
{
	ILUInt32 current;
	ILGenericPar *genPar;

	genPar = ILGenericParCreate(info->image, 0, owner, genParam->num + offset);
	if(!genPar)
	{
		CCOutOfMemory();
	}
	if(!ILGenericParSetName(genPar, genParam->name))
	{
		CCOutOfMemory();
	}
	ILGenericParSetFlags(genPar, IL_MAX_UINT32, genParam->constraint);
	genParam->genPar = genPar;

	/* Check for duplicate names */
	for(current = 0; current < offset; current++)
	{
		ILGenericPar *genParCheck = ILGenericParGetFromOwner(owner, current);

		if(genParCheck)
		{
			if(!strcmp(ILGenericParGetName(genParCheck), genParam->name))
			{
				char buffer[31];

				sprintf(buffer, "<_P%i>", *overridden);
				if(!ILGenericParSetName(genParCheck, buffer))
				{
					CCOutOfMemory();
				}
				(*overridden)++;
			}
		}
	}
	AddGenericConstraints(info, genPar, genParam->typeConstraints);
}

static void _AddTypeFormalsToClassInner(ILGenInfo *info,
										ILNode_ClassDefn *defn,
										ILProgramItem *owner,
										ILUInt32 *offset,
										ILUInt32 *overridden)
{
	if(defn->nestedParent)
	{
		/* Add the deneric parameters of the nested parents first. */
		_AddTypeFormalsToClassInner(info, defn->nestedParent,
									owner, offset, overridden);
	}
	if(defn->typeFormals)
	{
		ILNode_ListIter iter;
		ILNode_GenericTypeParameter *genParam;
		ILNode_GenericTypeParameters *genParams;

		/* Perform the semantic analysis on the typeFormals */
		ILNode_SemAnalysis(defn->typeFormals, info, &(defn->typeFormals));

		genParams = (ILNode_GenericTypeParameters *)(defn->typeFormals);
		ILNode_ListIter_Init(&iter, genParams->typeParams);
		while((genParam = 
			(ILNode_GenericTypeParameter *)ILNode_ListIter_Next(&iter)) != 0)
		{
			 _AddTypeFormalWithCheck(info, owner, genParam,
									 *offset, overridden);
		}
		*offset += genParams->numTypeParams;
	}
}

/*
 * Add the generic parameters to a class.
 * Handle the generic parameters consistent with the ECMA specs.
 */
static void AddTypeFormalsToClass(ILGenInfo *info,
								  ILNode_ClassDefn *defn)
{	
	ILUInt32 overridden = 0;
	ILUInt32 offset = 0;

	if(defn->nestedParent)
	{
		_AddTypeFormalsToClassInner(info, defn->nestedParent,
									ILToProgramItem(defn->classInfo),
									&offset, &overridden);
	}
	if(defn->typeFormals)
	{
		ILNode_ListIter iter;
		ILNode_GenericTypeParameter *genParam;
		ILNode_GenericTypeParameters *genParams;

		genParams = (ILNode_GenericTypeParameters *)(defn->typeFormals);
		ILNode_ListIter_Init(&iter, genParams->typeParams);
		while((genParam = 
			(ILNode_GenericTypeParameter *)ILNode_ListIter_Next(&iter)) != 0)
		{
			 _AddTypeFormalWithCheck(info, ILToProgramItem(defn->classInfo),
									 genParam, offset, &overridden);

			/* Adjust the generic parameter number */
			genParam->num += offset;
		}
	}
}

static void AddGenericParametersToClass(ILGenInfo *info, ILNode *classDefn)
{
	if(yyisa(classDefn, ILNode_ClassDefn))
	{
		ILNode *savedNamespace;
		ILNode *savedClass;
		ILNode_ClassDefn *defn = (ILNode_ClassDefn *)classDefn;

		if(!(defn->classInfo) ||
		   (defn->classInfo == (ILClass *)1) ||
		   (defn->classInfo == (ILClass *)2))
		{
			return;
		}

		/* Set the namespace and class to use for resolving type names */
		savedNamespace = info->currentNamespace;
		info->currentNamespace = defn->namespaceNode;
		savedClass = info->currentClass;
		info->currentClass = classDefn;

		/* Add the generic type parameters to the class */
		AddTypeFormalsToClass(info, defn);

		/* Restore the previous values. */
		info->currentClass = savedClass;
		info->currentNamespace = savedNamespace;

		/* process the nested classes */
		if(defn->nestedClasses)
		{
			ILNode *child;
			ILNode_ListIter iter;

			ILNode_ListIter_Init(&iter, defn->nestedClasses);
			while((child = ILNode_ListIter_Next(&iter)) != 0)
			{
				AddGenericParametersToClass(info, child);
			}
		}
	}
}
#endif	/* IL_VERSION_MAJOR > 1 */

static ILNode *GetImplicitParent(ILNode_ClassDefn *defn)
{
	switch(defn->modifiers & CS_MODIFIER_TYPE_MASK)
	{
		case CS_MODIFIER_TYPE_STRUCT:
		{
			return ILNode_GlobalNamespace_create(ILNode_SystemType_create("ValueType"));
		}
		break;

		case CS_MODIFIER_TYPE_ENUM:
		{
			return ILNode_GlobalNamespace_create(ILNode_SystemType_create("Enum"));
		}
		break;

		case CS_MODIFIER_TYPE_DELEGATE:
		{
			return ILNode_GlobalNamespace_create(ILNode_SystemType_create("MulticastDelegate"));
		}
		break;
	}
	return 0;
}

static void AddObjectParent(ILGenInfo *info,
							ILNode_ClassDefn *classNode,
							ILClass *classInfo,
							ILNode *systemObjectName)
{
	ILClass *objectClass;
	ILProgramItem *parent = 0;

	objectClass = ILType_ToClass(ILFindSystemType(info, "Object"));
	if(!objectClass)
	{
		ILNode *baseTypeNode = 0;

		/* Compiling something else that inherits "System.Object" */
		if(CSSemBaseType(systemObjectName, info, &systemObjectName,
						 &baseTypeNode, &parent))
		{
			if(!parent)
			{
				parent = NodeToProgramItem(baseTypeNode);
			}
		}
	}
	else
	{
		parent = ILToProgramItem(objectClass);
	}
	if(!parent)
	{
		CCErrorOnLine(yygetfilename(classNode), yygetlinenum(classNode),
					  "could not resolve System.Object");
	}
	else
	{
		/*
		 * Check classInfo is System.Object so we don't have to add the
		 * parent
		 */
		if(!ILProgramItemToTypeSpec(parent) &&
		   ((objectClass = ILProgramItemToClass(parent)) != 0))
		{
			if(ILClassResolve(objectClass) == ILClassResolve(classInfo))
			{
				/* Compiling System.Object so don't set the parent. */
				parent = 0;
			}
		}
	}
	if(parent)
	{
		ILClassSetParent(classInfo, parent);
	}
}

/*
 * Check if an interface is already declared in the implement list and add
 * it to the list if not.
 * If reportError is != 0 an error is reported.
 */
static void AddImplementedInterface(ILGenInfo *info,
									ILNode *node,
									ILProgramItem **implementList,
									ILUInt32 *numImplements,
									ILProgramItem *interface,
									int reportError)
{
	ILUInt32 current;

	for(current = 0; current < *numImplements; ++current)
	{
		if(implementList[current] == interface)
		{
			if(reportError)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					"interface declared multiple times in implement list");
			}
			return;
		}
	}
	implementList[*numImplements] = interface;
	*numImplements += 1;
}

/*
 * Collect the base classes for one class definition.
 * The implement list must be large enough to hold all implemented
 * interfaces of this class definition.
 */
static void CollectBaseClasses(ILGenInfo *info,
							   ILNode_ClassDefn *defn,
							   ILProgramItem **parent,
							   ILProgramItem **implementList,
							   ILUInt32 *numImplements,
							   int baseClassAllowed)
{
	int errorReported;
	ILUInt32 currentBase;
	ILUInt32 currentImpl;
	ILNode *savedNamespace;
	ILNode *savedClass;
	ILNode *baseNode;
	ILNode *baseNodeList;

	*parent = 0;
	errorReported = 0;

	/* Set the namespace and class to use for resolving type names */
	savedNamespace = info->currentNamespace;
	savedClass = info->currentClass;
	info->currentNamespace = defn->namespaceNode;
	info->currentClass = (ILNode *)defn;

	currentImpl = 0;
	baseNodeList = defn->baseClass;
	for(currentBase = 0; currentBase < *numImplements; ++currentBase)
	{
		ILNode *baseTypeNode;
		ILProgramItem *baseItem;

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

		baseItem = 0;
		/* Look in the scope for the base class */
		if(CSSemBaseType(baseNode, info, &baseNode,
						 &baseTypeNode, &baseItem))
		{
			/* All class nodes should have a valid classinfo at
			   this point. */
			if(baseItem == 0)
			{
				baseItem = NodeToProgramItem(baseTypeNode);

				if(baseItem == 0)
				{
					/* This is not a valid base class specification */
					CCErrorOnLine(yygetfilename(baseNode), yygetlinenum(baseNode),
								  "invalid base type");
				}
			}

			if(baseItem)
			{
				ILClass *underlying;

				underlying = ILProgramItemToUnderlyingClass(baseItem);
				if(underlying)
				{
					underlying = ILClassResolve(underlying);
				}
				if(!underlying)
				{
					CCOutOfMemory();
				}
				if(currentBase == 0)
				{
					/* Handle the first item in the base list */
					if(!ILClass_IsInterface(underlying))
					{
						if(baseClassAllowed)
						{
							*parent = baseItem;
						}
						else
						{
							CCErrorOnLine(yygetfilename(baseNode),
										  yygetlinenum(baseNode),
										  "base class not allowed in this scope");

						}
					}
					else
					{
						/* First base in the list is an interface */
						AddImplementedInterface(info, baseNode,
												implementList, &currentImpl,
												baseItem, 1);
					}
				}
				else
				{
					if(!ILClass_IsInterface(underlying))
					{
						/*
						 * Non interface class found later in the base list.
						 */
						if(!parent)
						{
							CCErrorOnLine(yygetfilename(defn),
										  yygetlinenum(defn),
							  "base class must be the first class in the base list");
							if(baseClassAllowed)
							{
								*parent = baseItem;
							}
						}
						else
						{
							if(!errorReported)
							{
								CCErrorOnLine(yygetfilename(defn),
											  yygetlinenum(defn),
								"more than one non-interface classes in the base class list");
								errorReported = 1;
							}
						}
					}
					else
					{
						AddImplementedInterface(info, baseNode,
												implementList, &currentImpl,
												baseItem, 1);
					}
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

	/* Restore the namespace, class */
	info->currentNamespace = savedNamespace;
	info->currentClass = savedClass;

	*numImplements = currentImpl;
}

static void AddBaseClasses(ILGenInfo *info,
						   ILNode_ClassDefn *classNode,
						   ILNode *systemObjectName)
{
	ILClass *classInfo = classNode->classInfo;

#if IL_VERSION_MAJOR > 1
	if(classNode->partialParent)
	{
		/* This is handled by the main part of the partial class */
		return;
	}
#endif /* IL_VERSION_MAJOR > 1 */

	if(classInfo && (classInfo != (ILClass *)1) &&
					(classInfo != (ILClass *)2))
	{
		int baseClassAllowed;
		ILProgramItem *parent = 0;
		ILUInt32 numBases = NumBases(classNode);

		/* Only classes can have an explicit  base class */
		baseClassAllowed = ((classNode->modifiers & CS_MODIFIER_TYPE_MASK) ==
						   CS_MODIFIER_TYPE_CLASS);
		if(numBases > 0)
		{
			if(!strcmp(ILClass_Name(classInfo), "<Module>"))
			{
				CCErrorOnLine(yygetfilename(classNode), yygetlinenum(classNode),
							  "Modules must not have any base type specifications");
				numBases = 0;
			}
		}

		if(numBases > 0)
		{
			int base;
			ILProgramItem *baseList[numBases];

			ILMemZero(baseList, numBases * sizeof(ILClass *));

			numBases = CountBaseClasses(classNode->baseClass);
			if(numBases > 0)
			{
				CollectBaseClasses(info, classNode, &parent, baseList,
								   &numBases, baseClassAllowed);
			}

		#if IL_VERSION_MAJOR > 1
			if(classNode->otherParts)
			{
				ILUInt32 numPartBases;
				ILNode_ListIter iter;
				ILNode *part;

				ILNode_ListIter_Init(&iter, classNode->otherParts);
				while((part = ILNode_ListIter_Next(&iter)) != 0)
				{
					if(yykind(part) == yykindof(ILNode_ClassDefn))
					{
						ILNode_ClassDefn *partDefn;

						partDefn = (ILNode_ClassDefn *)part;
						numPartBases = CountBaseClasses(partDefn->baseClass);
						if(numPartBases > 0)
						{
							ILProgramItem *partParent;
							ILProgramItem *partBaseList[numPartBases];

							partParent = 0;
							ILMemZero(partBaseList,
									  numPartBases * sizeof(ILClass *));

							CollectBaseClasses(info, partDefn, &partParent,
											   partBaseList, &numPartBases,
											   baseClassAllowed);
							if(!parent)
							{
								parent = partParent;
							}
							else
							{
								if(partParent && (partParent != parent))
								{
									CCErrorOnLine(yygetfilename(partDefn),
												  yygetlinenum(partDefn),
									"Base class mismatch in partial class");
								}
							}

							/* Add the interfaces to the main part list */
							for(base = 0; base < numPartBases; ++base)
							{
								/* Filter duplicates if present */
								AddImplementedInterface(info, part,
														baseList, &numBases,
														partBaseList[base],
														0);
							}
						}
					}
				}
			}
		#endif /* IL_VERSION_MAJOR > 1 */
			if(parent)
			{
				if(ILClass_IsInterface(classInfo))
				{
					CCErrorOnLine(yygetfilename(classNode),
								  yygetlinenum(classNode),
					"interface inherits from non-interface class");
				}
				else
				{
					ILClass *underlying;

					underlying = ILProgramItemToUnderlyingClass(parent);
					if(underlying)
					{
						underlying = ILClassResolve(underlying);
					}
					if(!underlying)
					{
						CCOutOfMemory();
					}
					if(ILClass_IsSealed(underlying))
					{
						CCErrorOnLine(yygetfilename(classNode),
									  yygetlinenum(classNode),
					  "inheriting from a sealed parent class");
					}
					else
					{
						/* Set the parent of the class */
						ILClassSetParent(classInfo, parent);
					}
				}
			}

			/* Add the interfaces to the class */
			for(base = 0; base < numBases; ++base)
			{
				if(baseList[base])
				{
					if(!ILClassAddImplements(classInfo, baseList[base], 0))
					{
						CCOutOfMemory();
					}
				}
			}
		}

		if(!parent && baseClassAllowed)
		{
			/* We have to add the System.Object parent */
			AddObjectParent(info, classNode, classInfo, systemObjectName);
		}
	}
	else
	{
		CCErrorOnLine(yygetfilename(classNode), yygetlinenum(classNode),
					  "class not completely layed out");
	}

	/* Now process the nested classes */
	if(classNode->nestedClasses)
	{
		ILNode *child;
		ILNode_ListIter iter;

		ILNode_ListIter_Init(&iter, classNode->nestedClasses);
		while((child = ILNode_ListIter_Next(&iter)) != 0)
		{
			if(yykind(child) == yykindof(ILNode_ClassDefn))
			{
				AddBaseClasses(info,
							   (ILNode_ClassDefn*)child,
							   systemObjectName);
			}
		}
	}
}

static ILUInt32 GetTypeAttrs(ILGenInfo *info, ILNode_ClassDefn *defn)
{
	ILUInt32 attrs;

	/* Validate the modifiers */
	if((defn->modifiers & CS_MODIFIER_TYPE_MASK) == CS_MODIFIER_TYPE_DELEGATE)
	{
		attrs = CSModifiersToDelegateAttrs((ILNode *)defn,
										   defn->modifiers & CS_MODIFIER_MASK,
										   (defn->nestedParent != 0));

		/*
		 * Delegates should not be serializable by default but cscc behaved
		 * like this.
		 */
		attrs |= IL_META_TYPEDEF_SERIALIZABLE;
		attrs |= IL_META_TYPEDEF_SEALED;
	}
	else
	{
		attrs = CSModifiersToTypeAttrs((ILNode *)defn,
									   defn->modifiers & CS_MODIFIER_MASK,
									   (defn->nestedParent != 0));

		/*
		 * Add type specific flags to the attributes.
		 */
		switch(defn->modifiers & CS_MODIFIER_TYPE_MASK)
		{
			case CS_MODIFIER_TYPE_STRUCT:
			{
				/*
				 * NOTE: Default sequential layout is not in the ECMA specs.
				 * But it's the way we and MS layout structs by default.
				 */
				attrs |= IL_META_TYPEDEF_LAYOUT_SEQUENTIAL;

				/*
				 * Structs should not be serializable by default but cscc behaved
				 * like this.
				 */
				attrs |= IL_META_TYPEDEF_SERIALIZABLE;

				attrs |= IL_META_TYPEDEF_SEALED;
			}
			break;

			case CS_MODIFIER_TYPE_INTERFACE:
			{
				attrs |= (IL_META_TYPEDEF_INTERFACE | IL_META_TYPEDEF_ABSTRACT);
			}
			break;

			case CS_MODIFIER_TYPE_ENUM:
			{
				/*
				 * Enums should not be serializable by default but cscc behaved
				 * like this.
				 */
				attrs |= IL_META_TYPEDEF_SERIALIZABLE;
				attrs |= IL_META_TYPEDEF_SEALED;
			}
			break;
		}

#if IL_VERSION_MAJOR > 1
		/* Process the "static" modifier */
		if((defn->modifiers & CS_MODIFIER_STATIC) != 0)
		{
			if((defn->modifiers & CS_MODIFIER_TYPE_MASK) ==  CS_MODIFIER_TYPE_CLASS)
			{
				if(defn->modifiers & CS_MODIFIER_SEALED)
				{
					CCErrorOnLine(yygetfilename(defn), yygetlinenum(defn),
								  "static classes must not be sealed");
				}
				if(defn->modifiers & CS_MODIFIER_ABSTRACT)
				{
					CCErrorOnLine(yygetfilename(defn), yygetlinenum(defn),
								  "static classes must not be abstract");
				}
				/* Static classes are sealed and abstract */
				attrs |= (IL_META_TYPEDEF_SEALED | IL_META_TYPEDEF_ABSTRACT);
			}
			else
			{
				CCErrorOnLine(yygetfilename(defn), yygetlinenum(defn),
					  "`static' modifier is not permitted on non classes");
			}
		}
#endif /* IL_VERSION_MAJOR > 1 */

	}

	return attrs;
}

/*
 * Forward declaration
 */
static void CreateType(ILGenInfo *info, ILScope *globalScope,
					   ILNode_List *list, ILNode *systemObjectName,
					   ILNode *type);

/*
 * Create the nested types of a type
 */
static void CreateNestedTypes(ILGenInfo *info, ILScope *globalScope,
							  ILNode_List *list, ILNode *systemObjectName,
							  ILNode_ClassDefn *nestedParent)
{
	ILNode *node;
	ILNode_ListIter iter;

	node = nestedParent->body;
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
}

static void AddTypeToList(ILNode_List *list, ILNode_ClassDefn *defn)
{
	ILNode *type;

	type = (ILNode *)defn;
	if(!(defn->nestedParent))
	{
		ILNode_List_Add(list, type);
	}
	else
	{
		if(!defn->nestedParent->nestedClasses)
		{
			defn->nestedParent->nestedClasses = ILNode_List_create();
		}
		ILNode_List_Add(defn->nestedParent->nestedClasses, type);
	}
}

static void AddDefaultCtor(ILNode_ClassDefn *defn)
{
	/* Determine if we need to add a default constructor */
#if IL_VERSION_MAJOR > 1
	/* Don't add the default constructor for static classes. */
	if(((defn->modifiers & (CS_MODIFIER_STATIC | CS_MODIFIER_CTOR_DEFINED)) == 0) &&
	   ((defn->modifiers & CS_MODIFIER_TYPE_MASK) == CS_MODIFIER_TYPE_CLASS))
#else	/* IL_VERSION_MAJOR == 1 */
	if(((defn->modifiers & CS_MODIFIER_CTOR_DEFINED) == 0) &&
	   ((defn->modifiers & CS_MODIFIER_TYPE_MASK) == CS_MODIFIER_TYPE_CLASS))
#endif	/* IL_VERSION_MAJOR == 1 */
	{
		ILUInt32 ctorMods;
		ILNode *empty;
		ILNode *cname;
		ILNode *nonstaticInit;
		ILNode *baseInit;
		ILNode *invokation;
		ILNode *compound;
		ILNode *ctor;
		ILNode *body;
		ILNode *node;

		ctorMods = (((defn->modifiers & CS_MODIFIER_ABSTRACT) != 0)
							? CS_MODIFIER_PROTECTED : CS_MODIFIER_PUBLIC);
		ctorMods |= CS_MODIFIER_METHOD_CONSTRUCTOR;
		cname = ILQualIdentSimple(ILInternString(".ctor", 5).string);
		empty = ILNode_Empty_create();
		nonstaticInit = ILNode_NonStaticInit_create();
		baseInit = ILNode_BaseInit_create();
		invokation = ILNode_InvocationExpression_create(baseInit, 0);
		compound = ILNode_Compound_CreateFrom(nonstaticInit, invokation);
		body = ILNode_NewScope_create(compound);
		ctor = ILNode_MethodDeclaration_create(0, ctorMods, 0, cname, 0,
											   empty, body);

		node = defn->body;
		if(!node)
		{
			node = ILNode_List_create();
			ILNode_List_Add(node, ctor);
			/* TODO: Create a scope change here */
			defn->body = node;
		}
		else
		{
			if(node && yyisa(node, ILNode_ScopeChange))
			{
				if(!((ILNode_ScopeChange *)node)->body)
				{
					/*
					 * A empty class declaration so create the member list now
					 */
					((ILNode_ScopeChange *)node)->body = ILNode_List_create();
				}
				node = ((ILNode_ScopeChange *)node)->body;
			}
			ILNode_List_Add(node, ctor);
		}
	}
}

/*
 * Create the program structure for a type and all of its base types.
 * Returns the new end of the top-level type list.
 */
static void CreateType(ILGenInfo *info, ILScope *globalScope,
					   ILNode_List *list, ILNode *systemObjectName,
					   ILNode *type)
{
	const char *name;
	const char *namespace;
	ILNode *baseNodeList;
	ILNode *baseNode;
	ILNode *baseTypeNode;
	ILProgramItem *parent;
	ILProgramItem *implicitParent;
	ILClass *classInfo;
	ILNode_ClassDefn *defn;
	ILNode *savedNamespace;
	ILNode *savedClass;
	ILProgramItem *nestedScope;

	/* Get the name and namespace for the type, for error reporting */
	defn = (ILNode_ClassDefn *)type;
	name = defn->name;
	namespace = defn->namespace;
	if(defn->nestedParent || (namespace && *namespace == '\0'))
	{
		namespace = 0;
	}

	/* If the type is already created, then bail out early */
	if(defn->classInfo != 0)
	{
		if(defn->classInfo == (ILClass *)2)
		{
			CCErrorOnLine(yygetfilename(defn), yygetlinenum(defn),
						  "`%s%s%s' is defined recursively",
						  (namespace ? namespace : ""),
						  (namespace ? "." : ""), name);
		}
		return;
	}

#if IL_VERSION_MAJOR > 1
	if(defn->partialParent)
	{
		/* This is an additional part of a partial type declaration */
		if(defn->partialParent->classInfo == 0)
		{
			/* This should not happen but i'm kindof paranoid here */
			CreateType(info, globalScope, list,
				   systemObjectName, (ILNode *)(defn->partialParent));
		}
		/*
		 * Get the classInfo of the partial parent and use this one for this
		 * part too since this one is for the same class.
		 */
		defn->classInfo = defn->partialParent->classInfo;

		/* Process the nested types */
		CreateNestedTypes(info, globalScope, list, systemObjectName, defn);

		/* Add the type to the list of nested classes in the nested parent if it's a nested type */
		if(defn->nestedParent)
		{
			AddTypeToList(list, defn);
		}

		return;
	}
#endif /* IL_VERSION_MAJOR > 1 */

	/* Mark this type as already seen so that we can detect recursion */
	defn->classInfo = (ILClass *)2;

	/* If this is a nested type, then create its nesting parent first */
	if(defn->nestedParent)
	{
		/* this is a backward edge of the class dependency graph,
		 * since we'll be coming back to this very same class by
		 * defn->nestedParent as it's nested child or the forward
		 * edge , let's skip this loop,by returning here */
		if(defn->nestedParent->classInfo == 0)
		{
			defn->classInfo = 0;
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

	/* Set the default accessibility if none was specified */
	if((defn->modifiers & CS_MODIFIER_ACCESS_MASK) == 0)
	{
		defn->modifiers |= (defn->nestedParent ? CS_MODIFIER_PRIVATE :
												 CS_MODIFIER_INTERNAL);
	}

	/* Set the namespace and class to use for resolving type names */
	savedNamespace = info->currentNamespace;
	info->currentNamespace = defn->namespaceNode;
	savedClass = info->currentClass;
	info->currentClass = (ILNode *)defn;


	/*
	 * Handle the implicit parents.
	 * They are known to be no interfaces and not generic.
	 */
	implicitParent = 0;
	if((baseNode = GetImplicitParent(defn)) != 0)
	{
		if(CSSemBaseType(baseNode, info, &baseNode,
						 &baseTypeNode, &implicitParent))
		{
			if(implicitParent == 0)
			{
				implicitParent = NodeToProgramItem(baseTypeNode);
				if(implicitParent == 0)
				{
					CreateType(info, globalScope, list,
							   systemObjectName, baseTypeNode);
					implicitParent = NodeToProgramItem(baseTypeNode);
				}
			}
		}
	}

	/* Test for interfaces, or find "System.Object" if no parent yet */
	/*
	 * NOTE: This done too to make sure that all base classes and interfaces
	 * are created before any class that inherits from the class or implamants
	 * an interface. Otherwise members for implemented interface methods are
	 * not created virtual sealed.
	 */
	parent = 0;
	baseNodeList = defn->baseClass;
	while(baseNodeList)
	{
		ILProgramItem *baseItem = 0;

		/* Get the name of the class to be inherited or implemented */
		if(yykind(baseNodeList) == yykindof(ILNode_ArgList))
		{
			baseNode = ((ILNode_ArgList *)baseNodeList)->expr2;
			baseNodeList = ((ILNode_ArgList *)baseNodeList)->expr1;
		}
		else
		{
			baseNode = baseNodeList;
			baseNodeList = 0;
		}

		/* Look in the scope for the base class */
		if(CSSemBaseType(baseNode, info, &baseNode,
						 &baseTypeNode, &baseItem))
		{
			if(baseItem == 0)
			{
				baseItem = NodeToProgramItem(baseTypeNode);
				if(baseItem == 0)
				{
					CreateType(info, globalScope, list,
							   systemObjectName, baseTypeNode);
					baseItem = NodeToProgramItem(baseTypeNode);
				}
			}
			if(baseItem)
			{
				ILClass *underlying;

				underlying = ILProgramItemToUnderlyingClass(baseItem);
				if(underlying)
				{
					underlying = ILClassResolve(underlying);
				}
				if(!underlying)
				{
					CCOutOfMemory();
				}
				if(!ILClass_IsInterface(underlying))
				{
					parent = baseItem;
				}
			}
		}
		else
		{
			/* If we get here the base type might be a member of a
			   nested parent that is not fully qualified (including
			   all nested parents).
			   The right gathering order is preserved because the
			   nested parent creates it's base types first including their
			   nested children.
			   The case that the base class does not exist is handled in
			   AddBaseClasses then. */
		}
	}

	if(!parent &&
	   ((defn->modifiers & CS_MODIFIER_TYPE_MASK) == CS_MODIFIER_TYPE_CLASS))
	{
		/* Compiling something else that inherits "System.Object" */
		/* Use the builtin library's "System.Object" as parent class */
		ILClass *objectClass;

		objectClass = ILType_ToClass(ILFindSystemType(info, "Object"));

		if(!objectClass)
		{
			ILNode_Namespace *namespaceNode;

			/* Change to the global namespace to resolve "System.Object" */
			namespaceNode = (ILNode_Namespace *)(info->currentNamespace);
			while(namespaceNode->enclosing != 0)
			{
				namespaceNode = namespaceNode->enclosing;
				info->currentNamespace = (ILNode *)namespaceNode;
			}

			if(CSSemBaseType(systemObjectName, info, &systemObjectName,
							 &baseTypeNode, &parent))
			{
				/* check if the parent is not yet created and if the resolved
				   baseTypeNode is not equal to the just processed classNode
				   what means we are currently processing System.Object itself. */
				if(!parent && (baseTypeNode != type))
				{
					parent = NodeToProgramItem(baseTypeNode);
					if(!parent)
					{
						CreateType(info, globalScope, list,
								   systemObjectName, baseTypeNode);
						parent = NodeToProgramItem(baseTypeNode);
					}
				}
			}
		}
	}

	/* Create the class information block */
	if((defn->modifiers & CS_MODIFIER_TYPE_MASK) != CS_MODIFIER_TYPE_MODULE)
	{
		ILUInt32 attrs;

		attrs = GetTypeAttrs(info, defn);
		classInfo = ILClassCreate(nestedScope, 0, name, namespace, implicitParent);
		if(!classInfo)
		{
			CCOutOfMemory();
		}
		ILClassSetAttrs(classInfo, ~0, attrs);

		/* Add the default ctor to the class if needed */
		AddDefaultCtor(defn);
	}
	else
	{
		/* Retrieve the "<Module>" type, which was already created */
		classInfo = ILClassLookup(ILClassGlobalScope(info->image), name, 0);
	}
	defn->classInfo = classInfo;

	/* Restore the namespace, class, and type formals */
	info->currentNamespace = savedNamespace;
	info->currentClass = savedClass;

	/* Record the node on the class as user data */
	ILSetProgramItemMapping(info, (ILNode *)defn);

	/* Process the nested types */
	CreateNestedTypes(info, globalScope, list, systemObjectName, defn);

	/* Add the type to the new top-level list in create order */
	AddTypeToList(list, defn);
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
								  ILClass *scope, ILMember *notThis)
{
	while(classInfo != 0)
	{
		/* Scan the members of this class */
		ILMember *member;
		ILImplements *impl;

		member = 0;
		while((member = ILClassNextMemberMatch
				(classInfo, member, 0, name, 0)) != 0)
		{
			if(ILMemberAccessible(member, scope) && member != notThis)
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
				member = FindMemberByName
					(ILClassResolve(ILImplements_InterfaceClass(impl)),
					 name, scope, notThis);
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
	int kind = ILMemberGetKind(notThis);

	while(classInfo != 0)
	{
		/* Scan the members of this class */
		ILMember *member;

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
				else if(CSSignatureIdentical(ILMemberGetSignature(member),
										     signature))
				{
					if(ILMember_IsMethod(member) &&
					   ILMethod_HasSpecialName((ILMethod *)member) &&
					   !strncmp(name, "op_", 3))
					{
						/* This is an operator, which includes the
						   return type in its signature definition */
						if(ILTypeIdentical
							  (ILTypeGetReturn(ILMemberGetSignature(member)),
							   ILTypeGetReturn(signature)))
						{
							return member;
						}
					}
					else
					{
						return member;
					}
				}
			}
		}

		/* Scan parent interfaces if this class is itself an interface */
		if(ILClass_IsInterface(classInfo))
		{
			ILImplements *impl;

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
	else if((modifiers & CS_MODIFIER_NEW) == 0)
	{
		/* The duplicate is in a parent class, and "new" wasn't specified */
		CCWarningOnLine(yygetfilename(node), yygetlinenum(node),
		  "declaration of `%s' hides an inherited member, "
		  "and `new' was not present", name);
	}
}

/*
 * Report a warning for an unnecessary "new" keyword on a declaration.
 */
static void ReportUnnecessaryNew(ILNode *node, const char *name)
{
	CCWarningOnLine(yygetfilename(node), yygetlinenum(node),
			        "declaration of `%s' includes unnecessary `new' keyword",
					name);
}

static ILUInt32 GetFieldAttrs(ILNode_FieldDeclaration *field)
{
	ILUInt32 attrs;

	if((field->modifiers & CS_MODIFIER_FIELD_CONST) != 0)
	{
		attrs = CSModifiersToConstAttrs(field->type,
										field->modifiers & CS_MODIFIER_MASK);
	}
	else
	{
		attrs = CSModifiersToFieldAttrs(field->type,
										field->modifiers & CS_MODIFIER_MASK);

		if((field->modifiers & CS_MODIFIER_FIELD_SPECIAL_NAME) != 0)
		{
			attrs |= IL_META_FIELDDEF_SPECIAL_NAME;
		}
		if((field->modifiers & CS_MODIFIER_FIELD_RT_SPECIAL_NAME) != 0)
		{
			attrs |= IL_META_FIELDDEF_RT_SPECIAL_NAME;
		}
	}

	return attrs;
}

/*
 * Create a field definition.
 */
static void CreateField(ILGenInfo *info, ILNode_ClassDefn *classNode,
						ILNode_FieldDeclaration *field)
{
	ILNode_ListIter iterator;
	ILNode_FieldDeclarator *decl;
	ILField *fieldInfo;
	const char *name;
	ILType *tempType;
	ILType *modifier;
	ILMember *member;
	ILClass *classInfo;
	ILUInt32 fieldAttrs;

#if IL_VERSION_MAJOR > 1
	ILUInt32 classModifiers;

	if(classNode->partialParent)
	{
		classModifiers = classNode->partialParent->modifiers;
	}
	else
	{
		classModifiers = classNode->modifiers;
	}
	if((classModifiers & CS_MODIFIER_STATIC) != 0)
	{
		/* Constants are static by default */
		if((field->modifiers & CS_MODIFIER_FIELD_CONST) == 0)
		{
			/* Only static fields or constants are allowed */
			if((field->modifiers & CS_MODIFIER_STATIC) == 0)
			{
				CCErrorOnLine(yygetfilename(field), yygetlinenum(field),
						  "only static fields are allowed in static classes");
			}
			switch(field->modifiers & CS_MODIFIER_ACCESS_MASK)
			{
				case CS_MODIFIER_PROTECTED:
				{
					CCErrorOnLine(yygetfilename(field), yygetlinenum(field),
						"no protected fields are allowed in static classes");
				}
				break;

				case (CS_MODIFIER_PROTECTED | CS_MODIFIER_INTERNAL):
				{
					CCErrorOnLine(yygetfilename(field), yygetlinenum(field),
					"no protected internal fields are allowed in static classes");
				}
				break;
			}
		}
	}
#endif /* IL_VERSION_MAJOR > 1 */

	/* Set the default accessibility if none was specified */
	if((field->modifiers & CS_MODIFIER_ACCESS_MASK) == 0)
	{
		field->modifiers |= CS_MODIFIER_PRIVATE;
	}

	/* Get the field meadata flags */
	fieldAttrs = GetFieldAttrs(field);

	/* Get the class information block */
	classInfo = classNode->classInfo;

	/* Get the field's type */
	tempType = CSSemType(field->type, info, &(field->type));

	/* Add the "volatile" modifier if necessary */
	if((field->modifiers & CS_MODIFIER_VOLATILE) != 0)
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
		/* Set the field's owner for later semantic analysis */
		decl->owner = field;

		/* Get the name of the field */
		name = ILQualIdentName(decl->name, 0);

		/* Look for duplicates */
		member = FindMemberByName(classInfo, name, classInfo, 0);

		/* Create the field information block */
		fieldInfo = ILFieldCreate(classInfo, 0, name, fieldAttrs & 0xFFFF);
		if(!fieldInfo)
		{
			CCOutOfMemory();
		}
		decl->fieldInfo = fieldInfo;
		ILMemberSetSignature((ILMember *)fieldInfo, tempType);
		ILSetProgramItemMapping(info, (ILNode *)decl);

		/* Report on duplicates */
		if(member)
		{
			ReportDuplicates(decl->name, (ILMember *)fieldInfo,
							 member, classInfo, field->modifiers, name);
		}
		else if((field->modifiers & CS_MODIFIER_NEW) != 0)
		{
			ReportUnnecessaryNew(decl->name, name);
		}

		/* Add the field to the current scope */
		AddMemberToScope(info->currentScope, IL_SCOPE_FIELD,
						 name, (ILMember *)fieldInfo, decl->name);
	}
}

/*
 * Find an interface member match in a particular interface.
 */
static ILMember *FindInterfaceMatch(ILClass *interface,
									const char *name,
									ILType *signature,
									int kind)
{
	ILMember *member;

#if IL_VERSION_MAJOR > 1
	if(ILClassNeedsExpansion(interface))
	{
		ILType *classType;
		ILClass *instanceInfo;

		classType = ILClass_SynType(interface);
		instanceInfo = ILClassInstantiate(ILProgramItem_Image(interface),
										  classType, classType, 0);
		if(!instanceInfo)
		{
			return 0;
		}
		else
		{
			interface = instanceInfo;
		}
	}
#endif	/* IL_VERSION_MAJOR > 1 */

	member = 0;
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

	while((impl = ILClassNextImplements(classInfo, impl)) != 0)
	{
		ILMember *member;
		ILClass *interface;

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
		else if((newAttrs & CS_SPECIALATTR_OVERRIDE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
			  "explicit interface member implementation cannot be `override'");
		}
		else if((newAttrs & IL_META_METHODDEF_VIRTUAL) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
			  "explicit interface member implementation cannot be `virtual'");
		}
		if((newAttrs & CS_SPECIALATTR_NEW) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
			  "explicit interface member implementation cannot be `new'");
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
					  IL_META_METHODDEF_ABSTRACT);
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
						  CSTypeToName(ILClassToType(interface)));
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

/*
 * Get the full name of an explicit interface override member.
 */
static const char *GetFullExplicitName(ILClass *interface, const char *memberName)
{
	const char *name;
	if(ILClass_Namespace(interface) != 0)
	{
		name = ILInternStringConcat3
				(ILInternString
					(ILClass_Namespace(interface), -1),
				 ILInternString(".", 1),
				 ILInternString
				 	(ILClass_Name(interface), -1)).string;
		name = ILInternStringConcat3
				(ILInternString(name, -1),
				 ILInternString(".", 1),
				 ILInternString(memberName, -1)).string;
	}
	else
	{
		name = ILInternStringConcat3
				(ILInternString(ILClass_Name(interface), -1),
				 ILInternString(".", 1),
				 ILInternString(memberName, -1)).string;
	}
	return name;
}

/*
 * Determine if a declaration of "Finalize" will override
 * the one in "Object" or if it is on a separate "new" slot.
 */
static int IsRealFinalizer(ILClass *classInfo)
{
	ILClass *parent = ILClass_UnderlyingParentClass(classInfo);
	ILMethod *method;
	ILType *signature;
	while(parent != 0)
	{
		if(ILTypeIsObjectClass(ILType_FromClass(parent)))
		{
			/* We've found the declaration in "System.Object" */
			return 1;
		}
		method = 0;
		while((method = (ILMethod *)ILClassNextMemberMatch
				(parent, (ILMember *)method,
				 IL_META_MEMBERKIND_METHOD, "Finalize", 0)) != 0)
		{
			signature = ILMethod_Signature(method);
			if(ILTypeGetReturn(signature) == ILType_Void &&
			   ILTypeNumParams(signature) == 0)
			{
				if((ILMethod_Attrs(method) &
						(IL_META_METHODDEF_NEW_SLOT |
						 IL_META_METHODDEF_VIRTUAL)) !=
					 IL_META_METHODDEF_VIRTUAL)
				{
					/* We've found something other than the real "Finalize" */
					return 0;
				}
			}
		}
		parent = ILClass_UnderlyingParentClass(parent);
	}
	return 0;
}

static ILUInt32 GetMethodAttrs(ILNode_MethodDeclaration *method)
{
	ILUInt32 attrs;

	switch(method->modifiers & CS_MODIFIER_METHOD_TYPE_MASK)
	{
		case CS_MODIFIER_METHOD_NORMAL:
		{
			attrs = CSModifiersToMethodAttrs((ILNode *)method,
											 method->modifiers);
		}
		break;

		case CS_MODIFIER_METHOD_CONSTRUCTOR:
		{
			attrs = CSModifiersToConstructorAttrs((ILNode *)method,
												  method->modifiers);
		}
		break;

		case CS_MODIFIER_METHOD_DESTRUCTOR:
		{
			attrs = CSModifiersToDestructorAttrs((ILNode *)method,
												 method->modifiers);

			/* Add the override modifier by default */
			method->modifiers |= CS_MODIFIER_OVERRIDE;
		}
		break;

		case CS_MODIFIER_METHOD_OPERATOR:
		{
			attrs = CSModifiersToOperatorAttrs((ILNode *)method,
											   method->modifiers);
		}
		break;

		case CS_MODIFIER_METHOD_INTERFACE:
		{
			attrs = IL_META_METHODDEF_PUBLIC |
					IL_META_METHODDEF_VIRTUAL |
					IL_META_METHODDEF_ABSTRACT |
					IL_META_METHODDEF_HIDE_BY_SIG |
					IL_META_METHODDEF_NEW_SLOT;
		}
		break;

		case CS_MODIFIER_METHOD_INTERFACE_ACCESSOR:
		{
			attrs =  IL_META_METHODDEF_PUBLIC |
					 IL_META_METHODDEF_VIRTUAL |
					 IL_META_METHODDEF_ABSTRACT |
					 IL_META_METHODDEF_HIDE_BY_SIG |
					 IL_META_METHODDEF_SPECIAL_NAME |
					 IL_META_METHODDEF_NEW_SLOT;
		}
		break;

		case CS_MODIFIER_METHOD_EVENT_ACCESSOR:
		{
			attrs = CSModifiersToEventAttrs((ILNode *)method,
											method->modifiers);
		}
		break;

		case CS_MODIFIER_METHOD_PROPERTY_ACCESSOR:
		{
			attrs = CSModifiersToPropertyAttrs((ILNode *)method,
											   method->modifiers);
		}
		break;
	}

	return attrs;
}

/*
 * Create a method definition.
 */
static void CreateMethod(ILGenInfo *info, ILNode_ClassDefn *classNode,
						 ILNode_MethodDeclaration *method)
{
	const char *name;
	const char *basicName;
	ILUInt32 attrs;
	ILUInt32 thisAccess;
	ILUInt32 baseAccess;
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
	ILClass *classInfo;
#if IL_VERSION_MAJOR > 1
	ILNode *savedMethod;
	ILUInt32 classModifiers;

	if(classNode->partialParent)
	{
		classModifiers = classNode->partialParent->modifiers;
	}
	else
	{
		classModifiers = classNode->modifiers;
	}
	if((classModifiers & CS_MODIFIER_STATIC) != 0)
	{
		/* Only static methods are allowed */
		if((method->modifiers & CS_MODIFIER_STATIC) == 0)
		{
			CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
					"only static methods are allowed in static classes");
		}
		switch(method->modifiers & CS_MODIFIER_ACCESS_MASK)
		{
			case CS_MODIFIER_PROTECTED:
			{
				CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
					"no protected methods are allowed in static classes");
			}
			break;

			case (CS_MODIFIER_PROTECTED | CS_MODIFIER_INTERNAL):
			{
				CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
				"no protected internal methods are allowed in static classes");
			}
			break;
		}
		switch(method->modifiers & CS_MODIFIER_METHOD_TYPE_MASK)
		{
			case CS_MODIFIER_METHOD_OPERATOR:
			{
				CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
								"no operators are allowed in static classes");
			}
			break;
		}
	}

	/* Save the current method context. */
	savedMethod = info->currentMethod;
	/* and set the method context for generic type parameter resolution */
	info->currentMethod = (ILNode *)method;

#endif /* IL_VERSION_MAJOR > 1 */

	/* Get the method flags */
	attrs = GetMethodAttrs(method);

	/* Get the class information block */
	classInfo = classNode->classInfo;

	/* Get the name of the method, and the interface member (if any) */
	interface = 0;
	interfaceMember = 0;
	if(yykind(method->name) == yykindof(ILNode_Identifier) ||
	   yykind(method->name) == yykindof(ILNode_GenericQualIdent))
	{
		/* Simple method name */
		name = GetFullAndBasicNames(method->name, &basicName);
	}
	else
	{
		/* Qualified method name that overrides some interface method */
		name = GetFullAndBasicNames(method->name, &basicName);
		signature = CSSemType(((ILNode_QualIdent *)(method->name))->left, info,
							  &(((ILNode_QualIdent *)(method->name))->left));
		if(signature)
		{
			if(!ILType_IsClass(signature) ||
			   !ILClass_IsInterface(ILClassResolve(ILType_ToClass(signature))))
			{
				CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
							  "`%s' is not an interface",
							  CSTypeToName(signature));
			}
			else
			{
				interface = ILClassResolve(ILType_ToClass(signature));

				/* Modify the method name to include the fully-qualified
				   form of the interface's class name */
				name = GetFullExplicitName(interface, basicName);
			}
		}
		if(ILClass_IsInterface(classInfo))
		{
			CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
				  "cannot use explicit interface member implementations "
				  "within interfaces");
		}
	}

	/* Special-case the constructor for "ParamArrayAttribute", because
	   we may have already created it as a reference.  Needed to resolve
	   order of compilation issues in "mscorlib.dll". */
	if(!strcmp(name, ".ctor") &&
	   !strcmp(ILClass_Name(classInfo), "ParamArrayAttribute") &&
	   ILClass_Namespace(classInfo) != 0 &&
	   !strcmp(ILClass_Namespace(classInfo), "System"))
	{
		methodInfo = 0;
		while((methodInfo = (ILMethod *)ILClassNextMemberMatch
					(classInfo, (ILMember *)methodInfo,
					 IL_META_MEMBERKIND_METHOD, ".ctor", 0)) != 0)
		{
			if((ILMethod_Token(methodInfo) & IL_META_TOKEN_MASK) ==
					IL_META_TOKEN_MEMBER_REF)
			{
				if(!ILMethodNewToken(methodInfo))
				{
					CCOutOfMemory();
				}
				break;
			}
		}
	}
	else
	{
		methodInfo = 0;
	}

	/* Create the method information block */
	if(!methodInfo)
	{
		methodInfo = ILMethodCreate(classInfo, 0, name, (attrs & 0xFFFF));
		if(!methodInfo)
		{
			CCOutOfMemory();
		}
	}
	method->methodInfo = methodInfo;
	ILSetProgramItemMapping(info, (ILNode *)method);

	/* Get the return type */
	tempType = CSSemTypeVoid(method->type, info, &(method->type));

	/* Special handling for "Finalize" to be consistent with the ECMA spec */
	if(!strcmp(name, "Finalize") &&
	   (method->params == 0 || yyisa(method->params, ILNode_Empty)) &&
	   tempType == ILType_Void &&
	   (method->modifiers & CS_MODIFIER_METHOD_TYPE_MASK) != CS_MODIFIER_METHOD_DESTRUCTOR)
	{
		if((method->modifiers & CS_MODIFIER_OVERRIDE) != 0)
		{
			if(IsRealFinalizer(classInfo))
			{
				CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
							  "do not override `Object.Finalize'; provide "
							  "a destructor instead");
			}
		}
		else if(!ILTypeIsObjectClass(ILType_FromClass(classInfo)))
		{
			method->modifiers |= CS_MODIFIER_NEW;
			attrs |= IL_META_METHODDEF_NEW_SLOT;
		}
	}

	/* Create the method signature type */
	signature = ILTypeCreateMethod(info->context, tempType);
	if(!signature)
	{
		CCOutOfMemory();
	}
	if((method->modifiers & CS_MODIFIER_STATIC) == 0)
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
		if(fparam->pmod == ILParamMod_arglist)
		{
			argListParam = paramNum;
			++paramNum;
			continue;
		}
		tempType = CSSemType(fparam->type, info, &(fparam->type));

		/* Add a "byref" node to the type if "out" or "ref" */
		if(fparam->pmod == ILParamMod_out ||
		   fparam->pmod == ILParamMod_ref)
		{
			tempType = ILTypeCreateRef(info->context,
									   IL_TYPE_COMPLEX_BYREF, tempType);
			if(!tempType)
			{
				CCOutOfMemory();
			}
			if(info->outputIsJava)
			{
				CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
				  	"`%s' parameters not permitted when compiling "
						"to Java bytecode",
				    (fparam->pmod == ILParamMod_out ? "out" : "ref"));
			}
		}

		/* Add the parameter type to the method signature */
		if(!ILTypeAddParam(info->context, signature, tempType))
		{
			CCOutOfMemory();
		}

		/* Create a parameter definition in the metadata to record the name */
		parameter = ILParameterCreate
				(methodInfo, 0, ILQualIdentName(fparam->name, 0),
			     ((fparam->pmod == ILParamMod_out) ? IL_META_PARAMDEF_OUT : 0),
				 paramNum);
		if(!parameter)
		{
			CCOutOfMemory();
		}

		/* Add "System.ParamArrayAttribute" if the parameter is "params" */
		if(fparam->pmod == ILParamMod_params)
		{
			ILGenItemAddAttribute(info, (ILProgramItem *)parameter,
								  "ParamArrayAttribute");
		}

		/* Advance to the next parameter */
		++paramNum;
	}

	/* Mark the method as "vararg" if "__arglist" was present */
	if(argListParam != 0)
	{
		if(info->outputIsJava)
		{
			CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
				  "`__arglist' is disallowed when compiling to Java bytecode");
		}
		if((argListParam + 1) != paramNum)
		{
			CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
						  "`__arglist' must be the last formal parameter");
		}
		ILTypeSetCallConv(signature, ILType_CallConv(signature) |
									 IL_META_CALLCONV_VARARG);
		ILMethodSetCallConv(methodInfo, ILType_CallConv(signature));
	}

#if IL_VERSION_MAJOR > 1
	/* Set the number of generic parameters in the method signature */
	/* and mark the method as "generic" if generic type parameters are */
	/* present */
	if(method->typeFormals)
	{
		ILNode_GenericTypeParameters *genParams;

		genParams = (ILNode_GenericTypeParameters *)(method->typeFormals);
		ILTypeSetCallConv(signature, ILType_CallConv(signature) |
									 IL_META_CALLCONV_GENERIC);
		ILType_SetNumGen(signature, genParams->numTypeParams);
		ILMethodSetCallConv(methodInfo, ILType_CallConv(signature));

		AddTypeFormals(info, ILToProgramItem(methodInfo),
					   method->typeFormals);
	}
#endif	/* IL_VERSION_MAJOR > 1 */

	/* Set the signature for the method */
	ILMemberSetSignature((ILMember *)methodInfo, signature);

	/* Add the method to the current scope */
	AddMemberToScope(info->currentScope, IL_SCOPE_METHOD,
					 name, (ILMember *)methodInfo, method->name);

	/* Process interface overrides */
	if(!ILClass_IsInterface(classInfo))
	{
		paramNum = attrs;
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

	/* Ignore property methods with "specialname", as they are
	   tested elsewhere for duplicates */
	if(!ILMethod_HasSpecialName(methodInfo) ||
	   (strncmp(ILMethod_Name(methodInfo), "get_", 4) != 0 &&
	    strncmp(ILMethod_Name(methodInfo), "set_", 4) != 0))
	{
		/* If "override" is supplied, then look for its "virtual" */
		if((method->modifiers & CS_MODIFIER_OVERRIDE) != 0)
		{
			if(!ILMemberGetBase((ILMember *)methodInfo))
			{
				CCErrorOnLine(yygetfilename(method), yygetlinenum(method),
							  "`override' used on a method with no "
							  "corresponding `virtual'");
			}
		}

		/* If "-fno-hidebysig" was set on the command-line, and there is
		   something else in this class with the same name, then we need to
		   add "hidebysig" to the method (used for VB-style libraries) */
		if(CSNoHideBySig)
		{
			member = FindMemberByName(classInfo, name, classInfo,
								      (ILMember *)methodInfo);
			if(member)
			{
				ILMemberSetAttrs((ILMember *)methodInfo,
								 IL_META_METHODDEF_HIDE_BY_SIG,
								 IL_META_METHODDEF_HIDE_BY_SIG);
				if(ILMember_IsMethod(member))
				{
					ILMemberSetAttrs(member,
									 IL_META_METHODDEF_HIDE_BY_SIG,
									 IL_META_METHODDEF_HIDE_BY_SIG);
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
				if(ILMember_Owner(member) == classInfo)
				{
					ReportDuplicates(method->name, (ILMember *)methodInfo,
									 member, classInfo,
									 method->modifiers, name);
				}

				/* Check for the correct form of virtual method overrides */
				if((method->modifiers & CS_MODIFIER_OVERRIDE) == 0)
				{
					if((method->modifiers & CS_MODIFIER_NEW) == 0)
					{
						/* Report absent new keyword warning. */
						ReportDuplicates(method->name, (ILMember *)methodInfo,
										 member, classInfo,
										 method->modifiers, name);

						/* Add new slot modifier. */
						method->modifiers |= CS_MODIFIER_NEW;
					}

					/* Set the method to use a new vtable slot. */
					ILMemberSetAttrs((ILMember *)methodInfo,
									 IL_META_METHODDEF_VTABLE_LAYOUT_MASK,
									 IL_META_METHODDEF_NEW_SLOT);
				}
				else
				{
					/* Get the access modifiers for this and the base methods */
					thisAccess = (attrs &
								  IL_META_METHODDEF_MEMBER_ACCESS_MASK);
					baseAccess = (ILMember_Attrs(member) &
								  IL_META_METHODDEF_MEMBER_ACCESS_MASK);

					/* Check for legal modifiers for overrides */
					if((method->modifiers & CS_MODIFIER_OVERRIDE) != 0 &&
					   (thisAccess != baseAccess) &&
					   ((ILProgramItem_Image(member) ==
					     ILProgramItem_Image(methodInfo)) ||
					    (thisAccess != IL_META_METHODDEF_FAMILY) ||
					    (baseAccess != IL_META_METHODDEF_FAM_OR_ASSEM)))
					{
						class1 = ILMember_Owner(member);
						class2 = ILMethod_Owner(methodInfo);
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
			}
			else if(ILMember_Owner(member) == classInfo ||
			        (!ILMethodIsConstructor(methodInfo) &&
					 !ILMethodIsStaticConstructor(methodInfo)))
			{
				ReportDuplicates(method->name, (ILMember *)methodInfo,
								 member, classInfo, method->modifiers, name);
			}
		}
		else if((method->modifiers & CS_MODIFIER_NEW) != 0)
		{
			ReportUnnecessaryNew(method->name, name);
		}
	}
#if IL_VERSION_MAJOR > 1
	/* Restore the previous method context */
	info->currentMethod = savedMethod;
#endif	/* IL_VERSION_MAJOR > 1 */
}

/*
 * Create an enumerated type member definition.
 */
static void CreateEnumMember(ILGenInfo *info, ILClass *classInfo,
						     ILNode_EnumMemberDeclaration *enumMember)
{
	ILField *fieldInfo;
	const char *name;
	ILType *tempType;
	ILMember *member;

	/* Get the field's type, which is the same as its enclosing class */
	tempType = ILType_FromValueType(classInfo);

	/* Get the name of the field */
	name = ILQualIdentName(enumMember->name, 0);

	/* Check for the reserved field name "value__" */
	if(!strcmp(name, "value__"))
	{
		CCErrorOnLine(yygetfilename(enumMember), yygetlinenum(enumMember),
			  "the identifier `value__' is reserved in enumerated types");
		return;
	}

	/* Look for duplicates */
	member = FindMemberByName(classInfo, name, classInfo, 0);

	/* Create the field information block */
	fieldInfo = ILFieldCreate(classInfo, 0, name,
							  IL_META_FIELDDEF_PUBLIC |
							  IL_META_FIELDDEF_STATIC |
							  IL_META_FIELDDEF_LITERAL);
	if(!fieldInfo)
	{
		CCOutOfMemory();
	}
	enumMember->fieldInfo = fieldInfo;
	ILMemberSetSignature((ILMember *)fieldInfo, tempType);
	ILSetProgramItemMapping(info, (ILNode *)enumMember);

	/* Report on duplicates within this class only */
	if(member && ILMember_Owner(member) == classInfo)
	{
		ReportDuplicates((ILNode *)enumMember, (ILMember *)fieldInfo,
						 member, classInfo, 0, name);
	}
}

/*
 * Determine if a property is virtual by inspecting its get or set methods.
 */
static int PropertyIsVirtual(ILProperty *property)
{
	ILMethod *method;

	/* Check the "get" method */
	method = ILPropertyGetGetter(property);
	if(method)
	{
		return ILMethod_IsVirtual(method);
	}

	/* Check the "set" method */
	method = ILPropertyGetSetter(property);
	if(method)
	{
		return ILMethod_IsVirtual(method);
	}

	/* No "get" or "set", so assume that it isn't virtual */
	return 0;
}

/*
 * Determine if an event is virtual by inspecting its add or remove methods.
 */
static int EventIsVirtual(ILEvent *event)
{
	ILMethod *method;

	/* Check the "add" method */
	method = ILEventGetAddOn(event);
	if(method)
	{
		return ILMethod_IsVirtual(method);
	}

	/* Check the "remove" method */
	method = ILEventGetRemoveOn(event);
	if(method)
	{
		return ILMethod_IsVirtual(method);
	}

	/* No "add" or "remove", so assume that it isn't virtual */
	return 0;
}

/*
 * Create a property definition.
 */
static void CreateProperty(ILGenInfo *info, ILNode_ClassDefn *classNode,
						   ILNode_PropertyDeclaration *property,
						   const char **defaultMemberName)
{
	const char *name;
	const char *basicName;
	ILUInt32 thisAccess;
	ILUInt32 baseAccess;
	ILType *propType;
	ILType *tempType;
	ILProperty *propertyInfo;
	ILType *signature;
	ILNode_ListIter iterator;
	ILNode *param;
	ILNode_FormalParameter *fparam;
	ILUInt32 paramNum;
	ILMember *member;
	int interfaceOverride;
	ILMethod *baseMethod;
	ILClass *class1, *class2;
	ILClass *classInfo;

#if IL_VERSION_MAJOR > 1
	ILUInt32 classModifiers;

	if(classNode->partialParent)
	{
		classModifiers = classNode->partialParent->modifiers;
	}
	else
	{
		classModifiers = classNode->modifiers;
	}
	if((classModifiers & CS_MODIFIER_STATIC) != 0)
	{
		/* Only static methods are allowed */
		if((property->modifiers & CS_MODIFIER_STATIC) == 0)
		{
			CCErrorOnLine(yygetfilename(property), yygetlinenum(property),
					"only static properties are allowed in static classes");
		}
		switch(property->modifiers & CS_MODIFIER_ACCESS_MASK)
		{
			case CS_MODIFIER_PROTECTED:
			{
				CCErrorOnLine(yygetfilename(property), yygetlinenum(property),
					"no protected properties are allowed in static classes");
			}
			break;

			case (CS_MODIFIER_PROTECTED | CS_MODIFIER_INTERNAL):
			{
				CCErrorOnLine(yygetfilename(property), yygetlinenum(property),
				"no protected internal properties are allowed in static classes");
			}
			break;
		}
	}
#endif /* IL_VERSION_MAJOR > 1 */

	/* Get the class information block */
	classInfo = classNode->classInfo;

	/* Create the get and set methods */
	if(property->getAccessor)
	{
		CreateMethod(info, classNode,
				     (ILNode_MethodDeclaration *)(property->getAccessor));
	}
	if(property->setAccessor)
	{
		CreateMethod(info, classNode,
				     (ILNode_MethodDeclaration *)(property->setAccessor));
	}

	/* Get the name of the property */
	if(yykind(property->name) == yykindof(ILNode_Identifier) ||
	   yykind(property->name) == yykindof(ILNode_GenericQualIdent))
	{
		/* Simple property name */
		name = GetFullAndBasicNames(property->name, &basicName);
		interfaceOverride = 0;
	}
	else
	{
		/* Qualified property name that overrides some interface property */
		name = GetFullAndBasicNames(property->name, &basicName);
		signature = CSSemType
				(((ILNode_QualIdent *)(property->name))->left, info,
			     &(((ILNode_QualIdent *)(property->name))->left));
		if(signature)
		{
			if(!ILType_IsClass(signature) ||
			   !ILClass_IsInterface(ILType_ToClass(signature)))
			{
				CCErrorOnLine(yygetfilename(property), yygetlinenum(property),
							  "`%s' is not an interface",
							  CSTypeToName(signature));
			}
			else
			{
				name = GetFullExplicitName(ILType_ToClass(signature),
										   basicName);
			}
		}
		if(ILClass_IsInterface(classInfo))
		{
			CCErrorOnLine(yygetfilename(property), yygetlinenum(property),
				  "cannot use explicit interface member implementations "
				  "within interfaces");
		}
		interfaceOverride = 1;
	}

	/* Get the property type */
	propType = CSSemType(property->type, info, &(property->type));

	/* Create the property signature type */
	signature = ILTypeCreateProperty(info->context, propType);
	if(!signature)
	{
		CCOutOfMemory();
	}

	/* Create the parameters for the property */
	paramNum = 1;
	ILNode_ListIter_Init(&iterator, property->params);
	while((param = ILNode_ListIter_Next(&iterator)) != 0)
	{
		/* Get the type of the parameter */
		fparam = (ILNode_FormalParameter *)param;
		if(fparam->pmod == ILParamMod_arglist)
		{
			CCErrorOnLine(yygetfilename(property), yygetlinenum(property),
						  "`__arglist' cannot be used with indexers");
			++paramNum;
			continue;
		}
		tempType = CSSemType(fparam->type, info, &(fparam->type));

		/* Add the parameter type to the property signature */
		if(!ILTypeAddParam(info->context, signature, tempType))
		{
			CCOutOfMemory();
		}

		/* Return the name of this indexer for use in the
		   "DefaultMember" attribute on the containing class */
		if((property->modifiers & CS_MODIFIER_ACCESS_MASK)
				!= CS_MODIFIER_PRIVATE)
		{
			*defaultMemberName = basicName;
		}

		/* Move on to the next parameter */
		++paramNum;
	}

	/* Create the property information block */
	propertyInfo = ILPropertyCreate(classInfo, 0, name, 0, signature);
	if(!propertyInfo)
	{
		CCOutOfMemory();
	}
	property->propertyInfo = propertyInfo;
	ILSetProgramItemMapping(info, (ILNode *)property);

	/* Add the method semantics to the property */
	if(property->getAccessor)
	{
		if(!ILMethodSemCreate((ILProgramItem *)propertyInfo, 0,
					  IL_META_METHODSEM_GETTER,
					  ((ILNode_MethodDeclaration *)(property->getAccessor))
					  		->methodInfo))
		{
			CCOutOfMemory();
		}
	}
	if(property->setAccessor)
	{
		if(!ILMethodSemCreate((ILProgramItem *)propertyInfo, 0,
					  IL_META_METHODSEM_SETTER,
					  ((ILNode_MethodDeclaration *)(property->setAccessor))
					  		->methodInfo))
		{
			CCOutOfMemory();
		}
	}

	/* Add the property to the current scope */
	AddMemberToScope(info->currentScope, IL_SCOPE_PROPERTY,
					 name, (ILMember *)propertyInfo, property->name);

	/* If "override" is supplied, then look for its "virtual" */
	if((property->modifiers & CS_MODIFIER_OVERRIDE) != 0)
	{
		if(!ILMemberGetBase((ILMember *)propertyInfo))
		{
			CCErrorOnLine(yygetfilename(property), yygetlinenum(property),
						  "`override' used on a property with no "
						  "corresponding `virtual'");
		}
	}

	/* Look for duplicates and report on them */
	member = FindMemberBySignature(classInfo, name, signature,
								   (ILMember *)propertyInfo, classInfo,
								   interfaceOverride);
	if(member)
	{
		if(ILMember_IsProperty(member) &&
		   PropertyIsVirtual((ILProperty *)member) &&
		   (property->modifiers & CS_MODIFIER_NEW) == 0)
		{
			if(ILMember_Owner(member) == classInfo)
			{
				ReportDuplicates(property->name, (ILMember *)propertyInfo,
								 member, classInfo,
								 property->modifiers, name);
			}

			/* Check for the correct form of virtual method overrides */
			if((property->modifiers & CS_MODIFIER_OVERRIDE) == 0)
			{
				/* Report absent new keyword warning. */
				ReportDuplicates(property->name, (ILMember *)propertyInfo,
								 member, classInfo,
								 property->modifiers, name);

				/* Add new slot modifier for property. */
				property->modifiers |= CS_MODIFIER_NEW;

				/* Set the getter to use a new vtable slot. */
				if(property->getAccessor)
				{
					((ILNode_MethodDeclaration *)property->getAccessor)
						->modifiers |= CS_MODIFIER_NEW;
					ILMemberSetAttrs((ILMember *)(ILProperty_Getter(propertyInfo)),
									 IL_META_METHODDEF_VTABLE_LAYOUT_MASK,
									 IL_META_METHODDEF_NEW_SLOT);
				}

				/* Set the setter to use a new vtable slot. */
				if(property->setAccessor)
				{
					((ILNode_MethodDeclaration *)property->setAccessor)
						->modifiers |= CS_MODIFIER_NEW;
					ILMemberSetAttrs((ILMember *)(ILProperty_Setter(propertyInfo)),
									 IL_META_METHODDEF_VTABLE_LAYOUT_MASK,
									 IL_META_METHODDEF_NEW_SLOT);
				}
			}
			else
			{
				/* Get the access modifiers for this property */
				thisAccess = GetMemberVisibilityFromModifiers(property->modifiers);

				/* Get a base getter or setter */
				baseMethod = ILProperty_Getter((ILProperty *)member);
				if(!baseMethod)
				{
					baseMethod = ILProperty_Setter((ILProperty *)member);
				}

				/* Get the access modifiers for the base property */
				baseAccess = (ILMember_Attrs(baseMethod) &
				              IL_META_METHODDEF_MEMBER_ACCESS_MASK);

				/* Check for legal modifiers for overrides */
				if((property->modifiers & CS_MODIFIER_OVERRIDE) != 0 &&
				   (thisAccess != baseAccess) &&
				   ((ILProgramItem_Image(member) ==
				     ILProgramItem_Image(propertyInfo)) ||
				    (thisAccess != IL_META_METHODDEF_FAMILY) ||
				    (baseAccess != IL_META_METHODDEF_FAM_OR_ASSEM)))
				{
					class1 = ILMember_Owner(member);
					class2 = ILMethod_Owner(propertyInfo);
					CCErrorOnLine(yygetfilename(property), yygetlinenum(property),
						"cannot change the access modifiers while overriding "
						"property '%s%s%s.%s' with '%s%s%s.%s' ",
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
		}
		else
		{
			ReportDuplicates(property->name, (ILMember *)propertyInfo,
							 member, classInfo, property->modifiers, name);
		}
	}
	else if((property->modifiers & CS_MODIFIER_NEW) != 0)
	{
		ReportUnnecessaryNew(property->name, name);
	}
}

static ILUInt32 GetEventAccessorModifiers(ILNode_EventDeclaration *event)
{
	ILUInt32 modifiers;

	/* Validate the modifiers */
	if((event->modifiers & CS_MODIFIER_EVENT_INTERFACE) != 0)
	{
		modifiers = (event->modifiers & CS_MODIFIER_MASK) | CS_MODIFIER_METHOD_INTERFACE;
	}
	else
	{
		modifiers = (event->modifiers & CS_MODIFIER_MASK) | CS_MODIFIER_METHOD_EVENT_ACCESSOR;
	}

	return modifiers;
}

/*
 * Create the methods needed by an event declarator.
 */
static void CreateEventDeclMethods(ILNode_EventDeclaration *event,
								   ILNode_EventDeclarator *decl,
								   ILUInt32 accessorModifiers)
{
	ILNode_MethodDeclaration *method;
	ILNode *eventName;
	ILNode *name;
	ILNode *param;
	ILNode *addParams;
	ILNode *removeParams;

	/* Get the name of the event */
	eventName = ((ILNode_FieldDeclarator *)(decl->fieldDeclarator))->name;

	/* Create the parameter information for the "add" and "remove" methods */
	addParams = ILNode_List_create();
	param = ILNode_FormalParameter_create(0, ILParamMod_empty, event->type,
				ILQualIdentSimple(ILInternString("value", 5).string));
	CloneLine(param, (ILNode *)decl);
	ILNode_List_Add(addParams, param);

	removeParams = ILNode_List_create();
	param = ILNode_FormalParameter_create(0, ILParamMod_empty, event->type,
				ILQualIdentSimple(ILInternString("value", 5).string));
	CloneLine(param, (ILNode *)decl);
	ILNode_List_Add(removeParams, param);

	/* Create the "add" method */
	name = PrefixName(eventName, "add_");
	method = (ILNode_MethodDeclaration *)(decl->addAccessor);
	if(!method && event->needFields)
	{
		/* Field-based event that needs a pre-defined body */
		method = (ILNode_MethodDeclaration *)
			ILNode_MethodDeclaration_create
					(0, accessorModifiers, 0, name, 0, addParams, 0);
		method->body = ILNode_NewScope_create
							(ILNode_AssignAdd_create
								(ILNode_Add_create(eventName, 
									ILQualIdentSimple
										(ILInternString("value", 5).string))));
		decl->addAccessor = (ILNode *)method;
	}
	else if(!method)
	{
		/* Abstract interface definition */
		method = (ILNode_MethodDeclaration *)
			ILNode_MethodDeclaration_create
					(0, accessorModifiers, 0, name, 0, addParams, 0);
		decl->addAccessor = (ILNode *)method;
	}
	else
	{
		/* Regular class definition */
		method->modifiers = accessorModifiers;
		method->type = 0;
		method->name = name;
		method->params = addParams;
	}

	/* Create the "remove" method */
	name = PrefixName(eventName, "remove_");
	method = (ILNode_MethodDeclaration *)(decl->removeAccessor);
	if(!method && event->needFields)
	{
		/* Field-based event that needs a pre-defined body */
		method = (ILNode_MethodDeclaration *)
			ILNode_MethodDeclaration_create
					(0, accessorModifiers, 0, name, 0, removeParams, 0);
		method->body = ILNode_NewScope_create
							(ILNode_AssignSub_create
								(ILNode_Sub_create(eventName, 
									ILQualIdentSimple
										(ILInternString("value", 5).string))));
		decl->removeAccessor = (ILNode *)method;
	}
	else if(!method)
	{
		/* Abstract interface definition */
		method = (ILNode_MethodDeclaration *)
			ILNode_MethodDeclaration_create
					(0, accessorModifiers, 0, name, 0, removeParams, 0);
		decl->removeAccessor = (ILNode *)method;
	}
	else
	{
		/* Regular class definition */
		method->modifiers = accessorModifiers;
		method->type = 0;
		method->name = name;
		method->params = removeParams;
	}
}

/*
 * Create the methods needed by an event definition.
 */
static void CreateEventMethods(ILNode_EventDeclaration *event)
{
	ILNode_ListIter iter;
	ILNode *decl;
	ILUInt32 accessorModifiers;

	accessorModifiers = GetEventAccessorModifiers(event);

	if(yyisa(event->eventDeclarators, ILNode_EventDeclarator))
	{
		ILNode_EventDeclarator *eventDeclarator;

		eventDeclarator = (ILNode_EventDeclarator *)(event->eventDeclarators);

		/* A single declarator indicates a property-style event */
		event->needFields = 0;

		/* Create the methods for the event declarator */
		CreateEventDeclMethods(event, eventDeclarator, accessorModifiers);
	}
	else
	{
		/* A list of declarators indicates a field-style event */
		event->needFields =
			((event->modifiers & (CS_MODIFIER_ABSTRACT | CS_MODIFIER_EVENT_INTERFACE)) == 0);

		/* Scan the list and create the methods that we require */
		ILNode_ListIter_Init(&iter, event->eventDeclarators);
		while((decl = ILNode_ListIter_Next(&iter)) != 0)
		{
			ILNode_EventDeclarator *eventDeclarator;

			eventDeclarator = (ILNode_EventDeclarator *)decl;
			CreateEventDeclMethods(event, eventDeclarator, accessorModifiers);
		}
	}
}

/*
 * Create an event definition from a specific declarator.
 */
static void CreateEventDecl(ILGenInfo *info, ILNode_ClassDefn *classNode,
						    ILNode_EventDeclaration *event,
							ILType *eventType,
							ILNode_EventDeclarator *eventDecl)
{
	const char *name;
	const char *basicName;
	ILUInt32 thisAccess;
	ILUInt32 baseAccess;
	ILNode *eventName;
	ILEvent *eventInfo;
	ILType *signature;
	ILMember *member;
	int interfaceOverride;
	ILMethod *baseMethod;
	ILClass *class1, *class2;
	ILClass *classInfo;

#if IL_VERSION_MAJOR > 1
	ILUInt32 classModifiers;

	if(classNode->partialParent)
	{
		classModifiers = classNode->partialParent->modifiers;
	}
	else
	{
		classModifiers = classNode->modifiers;
	}
	if((classModifiers & CS_MODIFIER_STATIC) != 0)
	{
		/* Only static fields or constants are allowed */
		if((event->modifiers & CS_MODIFIER_STATIC) == 0)
		{
			CCErrorOnLine(yygetfilename(event), yygetlinenum(event),
						  "only static events are allowed in static classes");
		}
		switch(event->modifiers & CS_MODIFIER_ACCESS_MASK)
		{
			case CS_MODIFIER_PROTECTED:
			{
				CCErrorOnLine(yygetfilename(event), yygetlinenum(event),
					"no protected events are allowed in static classes");
			}
			break;

			case (CS_MODIFIER_PROTECTED | CS_MODIFIER_INTERNAL):
			{
				CCErrorOnLine(yygetfilename(event), yygetlinenum(event),
					"no protected internal events are allowed in static classes");
			}
			break;
		}
	}
#endif /* IL_VERSION_MAJOR > 1 */

	/* Create the event accessor methods */
	CreateEventMethods(event);

	/* Get the class information block */
	classInfo = classNode->classInfo;

	/* Set the back link for use by code generation */
	eventDecl->backLink = event;

	/* Create the add and remove methods */
	if(eventDecl->addAccessor)
	{
		CreateMethod(info, classNode,
				     (ILNode_MethodDeclaration *)(eventDecl->addAccessor));
	}
	if(eventDecl->removeAccessor)
	{
		CreateMethod(info, classNode,
				     (ILNode_MethodDeclaration *)(eventDecl->removeAccessor));
	}

	/* TODO: event initializers */

	/* Get the name of the event */
	eventName = ((ILNode_FieldDeclarator *)(eventDecl->fieldDeclarator))->name;
	if(yykind(eventName) == yykindof(ILNode_Identifier) ||
	   yykind(eventName) == yykindof(ILNode_GenericQualIdent))
	{
		/* Simple event name */
		name = GetFullAndBasicNames(eventName, &basicName);
		interfaceOverride = 0;
	}
	else
	{
		/* Qualified event name that overrides some interface event */
		name = GetFullAndBasicNames(eventName, &basicName);
		signature = CSSemType
				(((ILNode_QualIdent *)eventName)->left, info,
			     &(((ILNode_QualIdent *)eventName)->left));
		if(signature)
		{
			if(!ILType_IsClass(signature) ||
			   !ILClass_IsInterface(ILType_ToClass(signature)))
			{
				CCErrorOnLine(yygetfilename(eventName),
							  yygetlinenum(eventName),
							  "`%s' is not an interface",
							  CSTypeToName(signature));
			}
			else
			{
				name = GetFullExplicitName(ILType_ToClass(signature),
										   basicName);
			}
		}
		if(ILClass_IsInterface(classInfo))
		{
			CCErrorOnLine(yygetfilename(eventName), yygetlinenum(eventName),
				  "cannot use explicit interface member implementations "
				  "within interfaces");
		}
		interfaceOverride = 1;
	}

	/* Cannot create an event called "value", because it will
	   conflict with the name of the add/remove parameter */
	if(!strcmp(name, "value"))
	{
		CCErrorOnLine(yygetfilename(eventName), yygetlinenum(eventName),
			  		  "cannot declare an event called `value'");
	}

	/* Look for duplicates */
	member = FindMemberByName(classInfo, name, classInfo, 0);

	/* Create the event information block */
	eventInfo = ILEventCreate(classInfo, 0, name, 0,
							  ILTypeToClass(info, eventType));
	if(!eventInfo)
	{
		CCOutOfMemory();
	}
	eventDecl->eventInfo = eventInfo;
	ILSetProgramItemMapping(info, (ILNode *)eventDecl);

	/* Add the method semantics to the event */
	if(eventDecl->addAccessor)
	{
		if(!ILMethodSemCreate((ILProgramItem *)eventInfo, 0,
					  IL_META_METHODSEM_ADD_ON,
					  ((ILNode_MethodDeclaration *)(eventDecl->addAccessor))
					  		->methodInfo))
		{
			CCOutOfMemory();
		}
	}
	if(eventDecl->removeAccessor)
	{
		if(!ILMethodSemCreate((ILProgramItem *)eventInfo, 0,
					  IL_META_METHODSEM_REMOVE_ON,
					  ((ILNode_MethodDeclaration *)(eventDecl->removeAccessor))
					  		->methodInfo))
		{
			CCOutOfMemory();
		}
	}

	/* Add the event to the current scope */
	AddMemberToScope(info->currentScope, IL_SCOPE_EVENT,
					 name, (ILMember *)eventInfo, eventName);

	/* If "override" is supplied, then look for its "virtual" */
	if((event->modifiers & CS_MODIFIER_OVERRIDE) != 0)
	{
		if(!ILMemberGetBase((ILMember *)eventInfo))
		{
			CCErrorOnLine(yygetfilename(event), yygetlinenum(event),
						  "`override' used on an event with no "
						  "corresponding `virtual'");
		}
	}

	/* Report on the duplicates */
	if(member)
	{
		if(ILMember_IsEvent(member) &&
		   EventIsVirtual((ILEvent *)member) &&
		   (event->modifiers & CS_MODIFIER_NEW) == 0)
		{
			if(ILMember_Owner(member) == classInfo)
			{
				ReportDuplicates(eventName, (ILMember *)eventInfo,
								 member, classInfo,
								 event->modifiers, name);
			}

			/* Check for the correct form of virtual method overrides */
			if((event->modifiers & CS_MODIFIER_OVERRIDE) == 0)
			{
				/* Report absent new keyword warning. */
				ReportDuplicates(eventName, (ILMember *)eventInfo,
								 member, classInfo,
								 event->modifiers, name);

				/* Add new slot modifier for event. */
				event->modifiers |= CS_MODIFIER_NEW;

				/* Set the adder to use a new vtable slot. */
				if(eventDecl->addAccessor)
				{
					((ILNode_MethodDeclaration *)eventDecl->addAccessor)
						->modifiers |= CS_MODIFIER_NEW;
					ILMemberSetAttrs((ILMember *)(ILEvent_AddOn(eventInfo)),
									 IL_META_METHODDEF_VTABLE_LAYOUT_MASK,
									 IL_META_METHODDEF_NEW_SLOT);
				}

				/* Set the remover to use a new vtable slot. */
				if(eventDecl->removeAccessor)
				{
					((ILNode_MethodDeclaration *)eventDecl->removeAccessor)
						->modifiers |= CS_MODIFIER_NEW;
					ILMemberSetAttrs((ILMember *)(ILEvent_RemoveOn(eventInfo)),
									 IL_META_METHODDEF_VTABLE_LAYOUT_MASK,
									 IL_META_METHODDEF_NEW_SLOT);
				}
			}
			else
			{
				/* Get the access modifiers for this event */
				thisAccess = GetMemberVisibilityFromModifiers(event->modifiers);

				/* Get a base adder or remover */
				baseMethod = ILEvent_AddOn((ILEvent *)member);
				if(!baseMethod)
				{
					baseMethod = ILEvent_RemoveOn((ILEvent *)member);
				}

				/* Get the access modifiers for the base event */
				baseAccess = (ILMember_Attrs(baseMethod) &
				              IL_META_METHODDEF_MEMBER_ACCESS_MASK);

				/* Check for legal modifiers for overrides */
				if((event->modifiers & CS_MODIFIER_OVERRIDE) != 0 &&
				   (thisAccess != baseAccess) &&
				   ((ILProgramItem_Image(member) ==
				     ILProgramItem_Image(eventInfo)) ||
				    (thisAccess != IL_META_METHODDEF_FAMILY) ||
				    (baseAccess != IL_META_METHODDEF_FAM_OR_ASSEM)))
				{
					class1 = ILMember_Owner(member);
					class2 = ILMethod_Owner(eventInfo);
					CCErrorOnLine(yygetfilename(event), yygetlinenum(event),
						"cannot change the access modifiers while overriding "
						"event '%s%s%s.%s' with '%s%s%s.%s' ",
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
		}
		else
		{
			ReportDuplicates(eventName, (ILMember *)eventInfo,
							 member, classInfo, event->modifiers, name);
		}
	}
	else if((event->modifiers & CS_MODIFIER_NEW) != 0)
	{
		ReportUnnecessaryNew(eventName, name);
	}

	/* Create the hidden field for the event if necessary.  We must do
	   this after checking for duplicates so we don't get a false match */
	if(event->needFields)
	{
		ILUInt32 attrs = IL_META_FIELDDEF_PRIVATE;
		ILField *field;
		if((event->modifiers & CS_MODIFIER_STATIC) != 0)
		{
			attrs |= IL_META_FIELDDEF_STATIC;
		}
		field = ILFieldCreate(classInfo, 0, name, attrs);
		if(!field)
		{
			CCOutOfMemory();
		}
		ILMemberSetSignature((ILMember *)field, eventType);
		eventDecl->backingField = field;
	}
}

/*
 * Test for a delegate type.  We cannot use "ILTypeIsDelegate"
 * because this may be called on a delegate type that hasn't
 * had its "Invoke" method yet.
 */
static int FuzzyIsDelegate(ILType *type)
{
	if(ILType_IsClass(type))
	{
		ILClass *classInfo = ILClassResolve(ILType_ToClass(type));
		ILClass *parent = ILClass_UnderlyingParentClass(classInfo);
		if(parent)
		{
			const char *namespace = ILClass_Namespace(parent);
			if(namespace && !strcmp(namespace, "System") &&
			   !strcmp(ILClass_Name(parent), "MulticastDelegate"))
			{
				return 1;
			}
		}
	}
	return 0;
}


/*
 * Create an event definition.
 */
static void CreateEvent(ILGenInfo *info, ILNode_ClassDefn *classNode,
						ILNode_EventDeclaration *event)
{
	ILNode_ListIter iter;
	ILNode *decl;
	ILType *eventType;

	/* Get the event type and check that it is a delegate */
	eventType = CSSemType(event->type, info, &(event->type));
	if(!FuzzyIsDelegate(eventType))
	{
		CCErrorOnLine(yygetfilename(event), yygetlinenum(event),
  			"`%s' is not a delegate type", CSTypeToName(eventType));
	}

	/* Process the event declarators */
	if(yyisa(event->eventDeclarators, ILNode_EventDeclarator))
	{
		/* Create the methods for the event declarator */
		CreateEventDecl(info, classNode, event, eventType,
					    (ILNode_EventDeclarator *)(event->eventDeclarators));
	}
	else
	{
		/* Scan the list and create the methods that we require */
		ILNode_ListIter_Init(&iter, event->eventDeclarators);
		while((decl = ILNode_ListIter_Next(&iter)) != 0)
		{
			CreateEventDecl(info, classNode, event, eventType,
							(ILNode_EventDeclarator *)decl);
		}
	}
}

/*
 * Create a formal parameter with a system type
 */
static ILNode_FormalParameter* CreateFormalParameter(char *systemtype,
													 const char *name)
{
	ILNode *param=ILNode_FormalParameter_create(
						NULL,ILParamMod_empty, 
						ILNode_SystemType_create(systemtype),
		 				ILQualIdentSimple(ILInternString(name, -1).string));
	return (ILNode_FormalParameter*)param;
}

/* 
 * Append parameters in `from' to `to' , the onlyPtrs flag if enabled
 * appends only out or ref parameters
 */
static void AppendParameters(ILNode *from,ILNode_List *to,int onlyPtrs)
{
	ILNode_ListIter iter;
	ILNode_FormalParameter *node;
	if(!yyisa(from,ILNode_List)) return;
	ILNode_ListIter_Init(&iter,from);
	while((node = (ILNode_FormalParameter*)ILNode_ListIter_Next(&iter))!=0)
	{
		if((!onlyPtrs) || (node->pmod==ILParamMod_ref || 
							node->pmod == ILParamMod_out))
		{
			ILNode_FormalParameter *param= (ILNode_FormalParameter*)
											ILNode_FormalParameter_create(NULL,
													node->pmod,node->type,
													node->name);
			CloneLine((ILNode *)param, (ILNode *)node);
			ILNode_List_Add(to, param);
		}
	}
}

/*
 * Create a delegate member definition.
 */
static void CreateDelegateMember(ILGenInfo *info, ILNode_ClassDefn *classNode,
								 ILNode_DelegateMemberDeclaration *member)
{
	ILMethod *method;
	ILType *signature;
	ILNode_MethodDeclaration *decl;
	ILNode_List *params;
	ILClass *classInfo;

	/* Get the class information block */
	classInfo = classNode->classInfo;

	/* Create the delegate constructor */
	method = ILMethodCreate(classInfo, 0, ".ctor",
						    IL_META_METHODDEF_PUBLIC |
						    IL_META_METHODDEF_HIDE_BY_SIG |
						    IL_META_METHODDEF_SPECIAL_NAME |
						    IL_META_METHODDEF_RT_SPECIAL_NAME);
	if(!method)
	{
		CCOutOfMemory();
	}
	member->ctorMethod = method;
	signature = ILTypeCreateMethod(info->context, ILType_Void);
	if(!signature)
	{
		CCOutOfMemory();
	}
	if(!ILTypeAddParam(info->context, signature,
					   ILFindSystemType(info, "Object")))
	{
		CCOutOfMemory();
	}
	if(!ILTypeAddParam(info->context, signature, ILType_Int))
	{
		CCOutOfMemory();
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
	ILMethodSetCallConv(method, IL_META_CALLCONV_HASTHIS);
	ILMemberSetSignature((ILMember *)method, signature);
	ILMethodSetImplAttrs(method, ~((ILUInt32)0),
						 IL_META_METHODIMPL_RUNTIME);
	if(!ILParameterCreate(method, 0, "object", 0, 1))
	{
		CCOutOfMemory();
	}
	if(!ILParameterCreate(method, 0, "method", 0, 2))
	{
		CCOutOfMemory();
	}

	/* Create the "Invoke" method */
	decl = (ILNode_MethodDeclaration *)ILNode_MethodDeclaration_create
		(0, CS_MODIFIER_PUBLIC |
			CS_MODIFIER_VIRTUAL |
			CS_MODIFIER_METHOD_HIDE_BY_SIG,
		 member->returnType,
		 ILQualIdentSimple(ILInternString("Invoke", -1).string),
		 0, member->params, 0);
	
	CloneLine((ILNode *)decl, (ILNode *)member);

	CreateMethod(info, classNode, decl);
	method = member->invokeMethod = decl->methodInfo;
	if(method)
	{
		ILMethodSetImplAttrs(method, ~((ILUInt32)0),
							 IL_META_METHODIMPL_RUNTIME);
	}

	/* TODO: asynchronous interface for delegates */
	/* Clone the params list */
	params=(ILNode_List*)ILNode_List_create();
	AppendParameters(member->params,params,0);

	ILNode_List_Add(params,
						CreateFormalParameter("AsyncCallback","callback"));
	ILNode_List_Add(params,
						CreateFormalParameter("Object","object"));

	/* Create the "BeginInvoke" method */
	decl = (ILNode_MethodDeclaration*) ILNode_MethodDeclaration_create
			(0,
			CS_MODIFIER_PUBLIC |
			CS_MODIFIER_VIRTUAL |
			CS_MODIFIER_METHOD_HIDE_BY_SIG |
			CS_MODIFIER_METHOD_COMPILER_CONTROLED,
			ILNode_SystemType_create("IAsyncResult"),
			ILQualIdentSimple(ILInternString("BeginInvoke", -1).string),
			0,
			(ILNode*)params,
			0);

	CloneLine((ILNode *)decl, (ILNode *)member);

	CreateMethod(info, classNode, decl);
	method = member->beginInvokeMethod = decl->methodInfo;
	if(method)
	{
		ILMethodSetImplAttrs(method, ~((ILUInt32)0),
							 IL_META_METHODIMPL_RUNTIME);
	}

	/* Clone the params list */
	params=(ILNode_List*)ILNode_List_create();
	
	/* Append only the managed parameters */
	AppendParameters(member->params,params,1);

	ILNode_List_Add(params,
						CreateFormalParameter("IAsyncResult","result"));

	/* Create the "EndInvoke" method */
	decl = (ILNode_MethodDeclaration*) ILNode_MethodDeclaration_create
			(0,
			CS_MODIFIER_PUBLIC |
			CS_MODIFIER_VIRTUAL |
			CS_MODIFIER_METHOD_HIDE_BY_SIG |
			CS_MODIFIER_METHOD_COMPILER_CONTROLED,
			member->returnType,
			ILQualIdentSimple(ILInternString("EndInvoke", -1).string),
			0,
			(ILNode*)params,
			0);
	CloneLine((ILNode *)decl, (ILNode *)member);

	CreateMethod(info, classNode, decl);
	method = member->endInvokeMethod = decl->methodInfo;
	if(method)
	{
		ILMethodSetImplAttrs(method, ~((ILUInt32)0),
							 IL_META_METHODIMPL_RUNTIME);
	}
}

/*
 * Check abstract method overrides.
 */
static void CheckAbstractOverrides(ILGenInfo *info, ILClass *classInfo,
								   ILNode *node)
{
	ILClass *parent;
	ILMethod *method;
	ILClass *tempClass;
	ILMethod *method2;

	/* Scan up through the parents and look for all "abstract" methods */
	parent = ILClass_ParentClass(classInfo);
	while(parent != 0)
	{
		method = 0;
		while((method = (ILMethod *)ILClassNextMemberByKind
				(parent, (ILMember *)method, IL_META_MEMBERKIND_METHOD)) != 0)
		{
			/* Skip non-abstract methods */
			if(!ILMethod_IsAbstract(method))
			{
				continue;
			}

			/* Scan from "classInfo" to look for an override for this method */
			tempClass = classInfo;
			method2 = 0;
			while(tempClass != 0 && tempClass != parent)
			{
				method2 = (ILMethod *)ILClassNextMemberMatch
					(tempClass, 0, IL_META_MEMBERKIND_METHOD,
					 ILMethod_Name(method), ILMethod_Signature(method));
				if(method2 && !ILMethod_IsNewSlot(method2))
				{
					break;
				}
				tempClass = ILClass_ParentClass(tempClass);
				method2 = 0;
			}

			/* If we didn't find a match, then report an error */
			if(!method2)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "no override for abstract member `%s'",
							  CSItemToName(ILToProgramItem(method)));
			}
		}
		parent = ILClass_ParentClass(parent);
	}
}

/*
 * Fix up methods with the "public final virtual" attributes that
 * we thought were interface member overrides that turned out not to
 * be because there is an explicit override with the same signature.
 */
static void FixNonInterfaceMethods(ILClass *classInfo)
{
	ILMethod *method = 0;
	ILOverride *override;
	ILMethod *overMethod;
	while((method = (ILMethod *)ILClassNextMemberByKind
			(classInfo, (ILMember *)method, IL_META_MEMBERKIND_METHOD)) != 0)
	{
		/* Skip methods that aren't marked as "public final virtual" */
		if(!ILMethod_IsPublic(method) || !ILMethod_IsFinal(method) ||
		   !ILMethod_IsVirtual(method))
		{
			continue;
		}

		/* Look for an "Override" block with the same signature */
		override = 0;
		while((override = (ILOverride *)ILClassNextMemberByKind
					(classInfo, (ILMember *)override,
					 IL_META_MEMBERKIND_OVERRIDE)) != 0)
		{
			overMethod = ILOverrideGetDecl(override);
			if(!strcmp(ILMethod_Name(overMethod), ILMethod_Name(method)) &&
			   ILTypeIdentical(ILMethod_Signature(overMethod),
							   ILMethod_Signature(method)))
			{
				/* We've found a match, so assume that the
				   "final virtual" flags are incorrect */
				ILMemberSetAttrs((ILMember *)method,
								 IL_META_METHODDEF_VIRTUAL |
								 IL_META_METHODDEF_NEW_SLOT |
								 IL_META_METHODDEF_FINAL, 0);
				break;
			}
		}
	}
}

/*
 * Create the members of a class node.
 */
static void CreateMembers(ILGenInfo *info, ILScope *globalScope,
						  ILNode_ClassDefn *classNode)
{
	ILClass *classInfo;
	ILNode *body;
	ILScope *scope;
	ILScope *savedScope;
	ILNode *savedClass;
	ILNode *savedNamespace;
	ILNode_ListIter iterator;
	ILNode *member;
	const char *defaultMemberName;

	/* Get the class information block, and bail out if not defined */
	classInfo = classNode->classInfo;
	if(!classInfo || classInfo == ((ILClass *)1) ||
	   classInfo == ((ILClass *)2))
	{
		return;
	}

	/* Get the class body and the scope it is declared within */
	body = classNode->body;
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
	info->currentClass = (ILNode *)classNode;
	savedNamespace = info->currentNamespace;
	info->currentNamespace = classNode->namespaceNode;

	/* Iterate over the member definitions in the class body */
	defaultMemberName = 0;
	ILNode_ListIter_Init(&iterator, body);
	while((member = ILNode_ListIter_Next(&iterator)) != 0)
	{
		if(yykind(member) == yykindof(ILNode_FieldDeclaration))
		{
			CreateField(info, classNode, (ILNode_FieldDeclaration *)member);
		}
		else if(yykind(member) == yykindof(ILNode_MethodDeclaration))
		{
			CreateMethod(info, classNode,
						 (ILNode_MethodDeclaration *)member);
		}
		else if(yykind(member) == yykindof(ILNode_EnumMemberDeclaration))
		{
			CreateEnumMember(info, classInfo,
							 (ILNode_EnumMemberDeclaration *)member);
		}
		else if(yykind(member) == yykindof(ILNode_PropertyDeclaration))
		{
			CreateProperty(info, classNode,
						   (ILNode_PropertyDeclaration *)member,
						   &defaultMemberName);
		}
		else if(yykind(member) == yykindof(ILNode_EventDeclaration))
		{
			CreateEvent(info, classNode,
						(ILNode_EventDeclaration *)member);
		}
		else if(yykind(member) == yykindof(ILNode_DelegateMemberDeclaration))
		{
			CreateDelegateMember(info, classNode,
								 (ILNode_DelegateMemberDeclaration *)member);
		}
		else if(yykind(member) == yykindof(ILNode_ClassDefn))
		{
			/* Create nested classes only after completing members of
			 * the existing class to resolve overrides correctly */
		}
		else
		{
			CCErrorOnLine(yygetfilename(member), yygetlinenum(member),
				  "internal error - do not know how to declare this member");
		}
	}

	/* process the nested classes in the order of creation , rather than
	 * occurrence in source */
	if(classNode->nestedClasses)
	{
		ILNode_ListIter_Init(&iterator, classNode->nestedClasses);
		while((member = ILNode_ListIter_Next(&iterator)) != 0)
		{
			if(yykind(member) == yykindof(ILNode_ClassDefn))
			{
				CreateMembers(info, globalScope, (ILNode_ClassDefn *)member);
			}
		}
	}

#if IL_VERSION_MAJOR > 1
	/* If this is a top level class create the members for the other parts too */
	if(!(classNode->nestedParent) && (classNode->otherParts))
	{
		ILNode_ListIter_Init(&iterator, classNode->otherParts);
		while((member = ILNode_ListIter_Next(&iterator)) != 0)
		{
			if(yykind(member) == yykindof(ILNode_ClassDefn))
			{
				CreateMembers(info, globalScope, (ILNode_ClassDefn *)member);
			}
		}
	}
#endif /* IL_VERSION_MAJOR > 1 */

	/* Add the "DefaultMember" attribute to the class if necessary */
	if(defaultMemberName)
	{
		classNode->defaultMemberName = defaultMemberName;
	}

	/* If the class is not abstract then make sure that all abstract
	   methods in ancestor classes have been implemented here */
	if(!ILClass_IsAbstract(classInfo))
	{
		CheckAbstractOverrides(info, classInfo, (ILNode *)classNode);
	}

	/* Fix up "public final virtual" methods that we thought we
	   interface implementations but which turn out not to be */
	FixNonInterfaceMethods(classInfo);

	/* Return to the original scope */
	info->currentScope = savedScope;
	info->currentClass = savedClass;
	info->currentNamespace = savedNamespace;
}

#if IL_VERSION_MAJOR > 1
static void DeclareTypes(ILGenInfo *info, ILScope *parentScope,
						 ILNode *tree, ILNode_List *list,
						 ILNode_ClassDefn *nestedParent);

static const char *FlagTypeToName(ILUInt32 modifiers)
{
	switch(modifiers & CS_MODIFIER_TYPE_MASK)
	{
		case CS_MODIFIER_TYPE_CLASS:
		{
			return "class";
		}
		break;

		case CS_MODIFIER_TYPE_STRUCT:
		{
			return "struct";
		}
		break;

		case CS_MODIFIER_TYPE_INTERFACE:
		{
			return "interface";
		}
		break;

		case CS_MODIFIER_TYPE_ENUM:
		{
			return "enum";
		}
		break;

		case CS_MODIFIER_TYPE_DELEGATE:
		{
			return "delegate";
		}
		break;

		case CS_MODIFIER_TYPE_MODULE:
		{
			return "module";
		}
		break;
	}
	return "Unknown";
}
/*
 * Handle partial class declarations
 */
static void DeclareTypePart(ILGenInfo *info, ILNode_List *list,
							ILScope *scope, ILNode_ClassDefn *defn,
							ILNode_ClassDefn *existingDefn,
							ILNode_ClassDefn *nestedParent,
							const char *name, const char *namespace)
{
	if((defn->modifiers & CS_MODIFIER_PARTIAL) != 0)
	{
		if((existingDefn->modifiers & CS_MODIFIER_PARTIAL) == 0)
		{
			CCErrorOnLine(yygetfilename(defn), yygetlinenum(defn),
						  "`%s%s%s' already declared not partial",
						  (namespace ? namespace : ""),
						  (namespace ? "." : ""), name);
			CCErrorOnLine(yygetfilename(existingDefn),
						  yygetlinenum(existingDefn),
						  "the declaration was here");
			return;
		}

		if((existingDefn->modifiers & CS_MODIFIER_TYPE_MASK) !=
		   (defn->modifiers & CS_MODIFIER_TYPE_MASK))
		{
			/* class, struct or interface mismatch */
			CCErrorOnLine(yygetfilename(defn), yygetlinenum(defn),
						  "partial `%s' is already declared as partial `%s'",
						  FlagTypeToName(defn->modifiers),
						  FlagTypeToName(existingDefn->modifiers));
			CCErrorOnLine(yygetfilename(existingDefn),
						  yygetlinenum(existingDefn),
						  "the partial `%s' declaration was here",
						  FlagTypeToName(existingDefn->modifiers));
		}

		if((defn->modifiers & CS_MODIFIER_ACCESS_MASK) == 0)
		{
			/* OK */
		}
		else if((existingDefn->modifiers & CS_MODIFIER_ACCESS_MASK) == 0)
		{
			/*
			 * The first part has no accessibility defined so set the
			 * accessibility of the first part to the same definition as
			 * this part.
			 */
			existingDefn->modifiers |= (defn->modifiers & CS_MODIFIER_ACCESS_MASK);
		}
		else if((existingDefn->modifiers & CS_MODIFIER_ACCESS_MASK) !=
				(defn->modifiers & CS_MODIFIER_ACCESS_MASK))
		{
			/* accessibility mismatch */
			CCErrorOnLine(yygetfilename(defn), yygetlinenum(defn),
						  "accessibility mismatch with an other part of the %s",
						  FlagTypeToName(defn->modifiers));
			CCErrorOnLine(yygetfilename(existingDefn),
						  yygetlinenum(existingDefn),
						  "the other declaration was here");
		}

		if((defn->modifiers & CS_MODIFIER_ABSTRACT) != 0)
		{
			existingDefn->modifiers |= CS_MODIFIER_ABSTRACT;
		}

		if((defn->modifiers & CS_MODIFIER_SEALED) != 0)
		{
			existingDefn->modifiers |= CS_MODIFIER_SEALED;
		}

		if((defn->modifiers & CS_MODIFIER_STATIC) != 0)
		{
			existingDefn->modifiers |= CS_MODIFIER_STATIC;
		}

		/*
		 * set the flag if a default ctor is defined in the first part
		 * if there is one in any other part so that no default ctor is
		 * created automatically for classes.
		 */
		if((defn->modifiers & CS_MODIFIER_CTOR_DEFINED) != 0)
		{
			existingDefn->modifiers |= CS_MODIFIER_CTOR_DEFINED;
		}

		/* Remember the first declared partial declaration */
		defn->partialParent = existingDefn;

		/* Add this part to the list of other parts in the main part */
		if(!(existingDefn->otherParts))
		{
			existingDefn->otherParts = ILNode_List_create();
		}
		ILNode_List_Add(existingDefn->otherParts, (ILNode *)defn);

		/* Declare nested types in this part */
		DeclareTypes(info, scope, defn->body, list, existingDefn);

		/* Replace the class body with a scoped body */
		defn->body = ILNode_ScopeChange_create(scope, defn->body);

		/* Add the type to the end of the new top-level list */
		if(!nestedParent)
		{
			ILNode_List_Add(list, (ILNode *)defn);
		}
	}
	else
	{
		CCErrorOnLine(yygetfilename(defn), yygetlinenum(defn),
					  "`%s%s%s' already declared",
					  (namespace ? namespace : ""),
					  (namespace ? "." : ""), name);
		CCErrorOnLine(yygetfilename(existingDefn),
					  yygetlinenum(existingDefn),
					  "previous declaration here");
	}
}
#endif	/* IL_VERSION_MAJOR > 1 */

/*
 * Scan all types and their nested children to declare them.
 */
static void DeclareTypes(ILGenInfo *info, ILScope *parentScope,
						 ILNode *tree, ILNode_List *list,
						 ILNode_ClassDefn *nestedParent)
{
	ILNode_ListIter iterator;
	ILNode *child;

	ILNode_ListIter_Init(&iterator, tree);
	while((child = ILNode_ListIter_Next(&iterator)) != 0)
	{
		if(yykind(child) == yykindof(ILNode_ClassDefn))
		{
			ILNode_ClassDefn *defn;
			ILScope *scope;
			ILScope *aliasScope;
			ILNode *origDefn;
			const char *name;
			const char *namespace;
			int error;

			defn = (ILNode_ClassDefn *)child;
			defn->nestedParent = nestedParent;
			name = defn->name;
			namespace = defn->namespace;
			if(nestedParent || (namespace && *namespace == '\0'))
			{
				namespace = 0;
			}
			
			aliasScope=((ILNode_Namespace*)(defn->namespaceNode))->localScope;
			
		#if IL_VERSION_MAJOR > 1
			if(defn->typeFormals)
			{
				/* Adjust the type name to be CLSCompliant. */
				ILUInt32 numTypeParams;

				numTypeParams = ((ILNode_GenericTypeParameters *)(defn->typeFormals))->numTypeParams;
				if(numTypeParams > 0)
				{
					char buffer[261];

					sprintf(buffer, "%s`%i", name, numTypeParams);
			 		name = ILInternString(buffer, -1).string;
					defn->name = name;
				}
			}
		#endif	/* IL_VERSION_MAJOR > 1 */

			error = ILScopeDeclareType(parentScope, child,
									   name, namespace, &scope,
									   &origDefn, aliasScope);

			if(error != IL_SCOPE_ERROR_OK)
			{
				/* Could not declare the type in the global scope */
				switch(error)
				{
					case IL_SCOPE_ERROR_IMPORT_CONFLICT:
					{
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"`%s%s%s' conflicts with imported type",
								(namespace ? namespace : ""),
								(namespace ? "." : ""), name);
					}
					break;

					case IL_SCOPE_ERROR_REDECLARED:
					{
					#if IL_VERSION_MAJOR > 1
						DeclareTypePart(info, list, scope, defn,
										(ILNode_ClassDefn *)origDefn,
										nestedParent, name, namespace);
					#else  /* IL_VERSION_MAJOR == 1 */
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"`%s%s%s' already declared",
								(namespace ? namespace : ""),
								(namespace ? "." : ""), name);
						CCErrorOnLine(yygetfilename(origDefn),
									  yygetlinenum(origDefn),
									  "previous declaration here");
					#endif /* IL_VERSION_MAJOR == 1 */
					}
					break;

					case IL_SCOPE_ERROR_CANT_CREATE_NAMESPACE:
					{
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
									  "`%s' is not a valid namespace",
									  namespace);
					}
					break;

					case IL_SCOPE_ERROR_NAME_IS_NAMESPACE:
					{
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"`%s%s%s' cannot be declared as a type",
								(namespace ? namespace : ""),
								(namespace ? "." : ""), name);
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"because it is already declared as namespace");
					}
					break;

					default:
					{
						CCErrorOnLine(yygetfilename(child), yygetlinenum(child),
								"`%s%s%s' cannot be declared as a type",
								(namespace ? namespace : ""),
								(namespace ? "." : ""), name);
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

ILNode *CSTypeGather(ILGenInfo *info, ILScope *globalScope, ILNode *tree)
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
	systemObject = ILNode_SystemType_create("Object");
	ILNode_ListIter_Init(&iterator, tree);
	while((child = ILNode_ListIter_Next(&iterator)) != 0)
	{
		CreateType(info, globalScope, list, systemObject, child);
	}

	info->typeGather = 0;

#if IL_VERSION_MAJOR > 1
	/* Add the generic type parameters to each class */
	ILNode_ListIter_Init(&iterator, list);
	while((child = ILNode_ListIter_Next(&iterator)) != 0)
	{
		AddGenericParametersToClass(info, child);
	}
#endif	/* IL_VERSION_MAJOR > 1 */

	ILNode_ListIter_Init(&iterator, list);
	while((child = ILNode_ListIter_Next(&iterator)) != 0)
	{
		AddBaseClasses(info,
					   (ILNode_ClassDefn*)child,
					   systemObject);
	}

	/* Create the class members within each type */
	ILNode_ListIter_Init(&iterator, list);
	while((child = ILNode_ListIter_Next(&iterator)) != 0)
	{
		CreateMembers(info, globalScope, (ILNode_ClassDefn *)child);
	}

	/* Return the new top-level list to the caller */
	return (ILNode *)list;
}

#ifdef	__cplusplus
};
#endif
