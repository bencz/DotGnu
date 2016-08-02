%{
/*
 * cs_grammar.y - Input file for yacc that defines the syntax of C#.
 *
 * Copyright (C) 2001, 2002, 2003, 2008  Southern Storm Software, Pty Ltd.
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

/* Rename the lex/yacc symbols to support multiple parsers */
#include "cs_rename.h"

/*#define YYDEBUG 1*/

#include "il_config.h"
#include <stdio.h>
#include "il_system.h"
#include "il_opcodes.h"
#include "il_meta.h"
#include "il_utils.h"
#include "cs_internal.h"
#ifdef HAVE_STDARG_H
	#include <stdarg.h>
#else
	#ifdef HAVE_VARARGS_H
		#include <varargs.h>
	#endif
#endif

#define	YYERROR_VERBOSE

/*
 * An ugly hack to work around missing "-lfl" libraries on MacOSX.
 */
#if defined(__APPLE_CC__) && !defined(YYTEXT_POINTER)
	#define	YYTEXT_POINTER 1
#endif

/*
 * Imports from the lexical analyser.
 */
extern int yylex(void);
#ifdef YYTEXT_POINTER
extern char *cs_text;
#else
extern char cs_text[];
#endif

int CSMetadataOnly = 0;

/*
 * Global state used by the parser.
 */
static unsigned long NestingLevel = 0;
static ILIntString CurrNamespace = {"", 0};
static ILNode_Namespace *CurrNamespaceNode = 0;
static int HaveDecls = 0;

/*
 * Get the global scope.
 */
static ILScope *GlobalScope(void)
{
	if(CCCodeGen.globalScope)
	{
		return CCCodeGen.globalScope;
	}
	else
	{
		CCCodeGen.globalScope = ILScopeCreate(&CCCodeGen, 0);
		return CCCodeGen.globalScope;
	}
}

/*
 * Initialize the global namespace, if necessary.
 */
static void InitGlobalNamespace(void)
{
	if(!CurrNamespaceNode)
	{
		CurrNamespaceNode = (ILNode_Namespace *)ILNode_Namespace_create(0, 0);
		CurrNamespaceNode->localScope = GlobalScope();
	}
}

/*
 * Reset the global state ready for the next file to be parsed.
 */
static void ResetState(void)
{
	NestingLevel = 0;
	CurrNamespace = ILInternString("", 0);
	CurrNamespaceNode = 0;
	HaveDecls = 0;
	ILScopeClearUsing(GlobalScope());
}

static void yyerror(char *msg)
{
	CCPluginParseError(msg, cs_text);
}

/*
 * Determine if an extension has been enabled using "-f".
 */
#define	HaveExtension(name)	\
	(CSStringListContains(extension_flags, num_extension_flags, (name)))

/*
 * Make a simple node and put it into $$.
 */
#define	MakeSimple(classSuffix)	\
	do {	\
		yyval.node = \
			ILNode_##classSuffix##_create(); \
	} while (0)

/*
 * Make a unary node and put it into $$.
 */
#define	MakeUnary(classSuffix,expr)	\
	do {	\
		yyval.node = ILNode_##classSuffix##_create((expr)); \
	} while (0)

/*
 * Make a binary node and put it into $$.
 */
#define	MakeBinary(classSuffix,expr1,expr2)	\
	do {	\
		yyval.node = ILNode_##classSuffix##_create((expr1), (expr2)); \
	} while (0)

/*
 * Make a ternary node and put it into $$.
 */
#define	MakeTernary(classSuffix,expr1,expr2,expr3)	\
	do {	\
		yyval.node = ILNode_##classSuffix##_create((expr1), (expr2), (expr3)); \
	} while (0)

/*
 * Make a quaternary node and put it into $$.
 */
#define	MakeQuaternary(classSuffix,expr1,expr2,expr3,expr4)	\
	do {	\
		yyval.node = ILNode_##classSuffix##_create \
							((expr1), (expr2), (expr3), (expr4)); \
	} while (0)

/*
 * Make a system type name node.
 */
#define	MakeSystemType(name)	\
			(ILNode_GlobalNamespace_create(ILNode_SystemType_create(name)))

/*
 * Clone the filename/linenum information from one node to another.
 */
static void CloneLine(ILNode *dest, ILNode *src)
{
	yysetfilename(dest, yygetfilename(src));
	yysetlinenum(dest, yygetlinenum(src));
}

/*
 * Make a list from an existing list (may be NULL), and a new node
 * (which may also be NULL).
 */
static ILNode *MakeList(ILNode *list, ILNode *node)
{
	if(!node)
	{
		return list;
	}
	else if(!list)
	{
		list = ILNode_List_create();
	}
	ILNode_List_Add(list, node);
	return list;
}

/*
 * Negate an integer node.
 */
static ILNode *NegateInteger(ILNode_Integer *node)
{
	if(node->canneg)
	{
		if(yyisa(node, ILNode_Int32))
		{
			node->isneg = !(node->isneg);
			return (ILNode *)node;
		}
		else if(yyisa(node, ILNode_UInt32))
		{
			return ILNode_Int32_create(node->value, 1, 0);
		}
		else if(yyisa(node, ILNode_Int64))
		{
			node->isneg = !(node->isneg);
			return (ILNode *)node;
		}
		else if(yyisa(node, ILNode_UInt64))
		{
			return ILNode_Int64_create(node->value, 1, 0);
		}
	}
	return ILNode_Neg_create((ILNode *)node);
}

/*
 * The class name stack, which is used to verify the names
 * of constructors and destructors against the name of their
 * enclosing classes.  Also used to check if a class has
 * had a constructor defined for it.
 */
static ILNode **classNameStack = 0;
static int     *classNameCtorDefined = 0;
static ILUInt32 *classNameModifiers = 0;
static int		classNameStackSize = 0;
static int		classNameStackMax = 0;

/*
 * Push an item onto the class name stack.
 */
static void ClassNamePush(ILNode *name, ILUInt32 modifiers)
{
	if(classNameStackSize >= classNameStackMax)
	{
		classNameStack = (ILNode **)ILRealloc
			(classNameStack, sizeof(ILNode *) * (classNameStackMax + 4));
		if(!classNameStack)
		{
			CCOutOfMemory();
		}
		classNameCtorDefined = (int *)ILRealloc
			(classNameCtorDefined, sizeof(int) * (classNameStackMax + 4));
		if(!classNameCtorDefined)
		{
			CCOutOfMemory();
		}
		classNameModifiers = (ILUInt32 *)ILRealloc
			(classNameModifiers, sizeof(ILUInt32) * (classNameStackMax + 4));
		if(!classNameModifiers)
		{
			CCOutOfMemory();
		}
		classNameStackMax += 4;
	}
	classNameStack[classNameStackSize] = name;
	classNameModifiers[classNameStackSize] = modifiers;
	classNameCtorDefined[classNameStackSize++] = 0;
}

/*
 * Pop an item from the class name stack.
 */
static void ClassNamePop(void)
{
	--classNameStackSize;
}

/*
 * Record that a constructor was defined for the current class.
 */
static void ClassNameCtorDefined(void)
{
	classNameCtorDefined[classNameStackSize - 1] = 1;
}

/*
 * Determine if a constructor was defined for the current class.
 */
static int ClassNameIsCtorDefined(void)
{
	return classNameCtorDefined[classNameStackSize - 1];
}

#if IL_VERSION_MAJOR > 1
/*
 * Get the modifiers of the current class.
 */
static ILUInt32 ClassNameGetModifiers(void)
{
	return classNameModifiers[classNameStackSize - 1];
}
#endif	/* IL_VERSION_MAJOR > 1 */

/*
 * Determine if an identifier is identical to
 * the top of the class name stack.
 */
static int ClassNameSame(ILNode *name)
{
	return (strcmp(((ILNode_Identifier *)name)->name,
	   ((ILNode_Identifier *)(classNameStack[classNameStackSize - 1]))->name)
				== 0);
}

/*
 * Setup a fresh array rank.
 */
static void ArrayRanksInit(struct ArrayRanks *ranks,
						   ILUInt32 rank)
{
	ranks->numRanks =1;
	ranks->rankList = 0;
	ranks->ranks[0] = rank;
	ranks->ranks[1] = 0;
	ranks->ranks[2] = 0;
	ranks->ranks[3] = 0;
}

/*
 * Add a new rank to the array ranks.
 */
static void ArrayRanksAddRank(struct ArrayRanks *destRanks,
							  struct ArrayRanks *srcRanks,
							  ILUInt32 rank)
{
	if(srcRanks->numRanks > 4)
	{
		destRanks->numRanks = srcRanks->numRanks + 1;
		ILNode_List_Add(srcRanks->rankList,
						(ILNode *)(ILNativeUInt)rank);
		destRanks->rankList = srcRanks->rankList;
		destRanks->ranks[0] = 0;
		destRanks->ranks[1] = 0;
		destRanks->ranks[2] = 0;
		destRanks->ranks[3] = 0;
	}
	else if(srcRanks->numRanks == 4)
	{
		destRanks->numRanks = srcRanks->numRanks + 1;
		destRanks->rankList = MakeList(0, (ILNode *)(ILNativeUInt)(srcRanks->ranks[0]));
		ILNode_List_Add(destRanks->rankList,
						(ILNode *)(ILNativeUInt)(srcRanks->ranks[1]));
		ILNode_List_Add(destRanks->rankList,
						(ILNode *)(ILNativeUInt)(srcRanks->ranks[2]));
		ILNode_List_Add(destRanks->rankList,
						(ILNode *)(ILNativeUInt)(srcRanks->ranks[3]));
		ILNode_List_Add(destRanks->rankList,
						(ILNode *)(ILNativeUInt)rank);
		destRanks->ranks[0] = 0;
		destRanks->ranks[1] = 0;
		destRanks->ranks[2] = 0;
		destRanks->ranks[3] = 0;
	}
	else
	{
		destRanks->numRanks = srcRanks->numRanks + 1;
		destRanks->rankList = 0;
		destRanks->ranks[0] = srcRanks->ranks[0];
		destRanks->ranks[1] = srcRanks->ranks[1];
		destRanks->ranks[2] = srcRanks->ranks[2];
		destRanks->ranks[3] = srcRanks->ranks[3];
		destRanks->ranks[srcRanks->numRanks] = rank;
	}
}

/*
 * Setup a fresh array type.
 */
static void ArrayTypeInit(struct ArrayType *arrayType,
						  ILNode *type,
						  ILUInt32 rank)
{
	arrayType->type = type;
	ArrayRanksInit(&(arrayType->ranks), rank);
}

/*
 * Inner worker function for creating the array types if the ranks are
 * stored in the list.
 */
static ILNode *ArrayTypeCreateInner(ILNode *type, ILNode_ListIter *iter)
{
	ILNode *node;

	if((node = ILNode_ListIter_Next(iter)) != 0)
	{
		ILNode *arrayType = ArrayTypeCreateInner(type, iter);
		ILNode *currentArrayType;

		currentArrayType = ILNode_ArrayType_create(arrayType,
												   (ILUInt32)(ILNativeUInt)node);
		CloneLine(currentArrayType, type);
		return currentArrayType;
	}
	else
	{
		return type;
	}
}

/*
 * Create the array types needed for the type specified ranks.
 */
static ILNode *ArrayTypeCreate(ILNode *type,
							   struct ArrayRanks *ranks)
{
	if(ranks->numRanks < 5)
	{
		/* The ranks are stored inside the structure */
		ILUInt32 currentLevel = ranks->numRanks;
		ILNode *currentArrayType = type;

		while(currentLevel > 0)
		{
			--currentLevel;

			currentArrayType = ILNode_ArrayType_create(currentArrayType,
													   ranks->ranks[currentLevel]);
			CloneLine(currentArrayType, type);
		}
		return currentArrayType;
	}
	else
	{
		/* The ranks are stored inside the rankList */
		ILNode_ListIter iter;

		ILNode_ListIter_Init(&iter, ranks->rankList);
		return ArrayTypeCreateInner(type, &iter);
	}
}

static void ArrayTypeAddRank(struct ArrayType *destType,
							 struct ArrayType *srcType,
							 ILUInt32 rank)
{
	destType->type = srcType->type;
	ArrayRanksAddRank(&(destType->ranks), &(srcType->ranks), rank);
}

/*
 * Modify an attribute name so that it ends in "Attribute".
 */
static void ModifyAttrName(ILNode *node,int force)
{
	const char *name;
	int namelen;
	
	if(yyisa(node,ILNode_QualIdent))
	{
		name = ((ILNode_QualIdent *)node)->name;
	}
	else if(yyisa(node,ILNode_Identifier))
	{
		name = ((ILNode_Identifier*)node)->name;
	}
	else
	{
		return;
	}

	namelen = strlen(name);
	if(force || (namelen < 9 || strcmp(name + namelen - 9, "Attribute") != 0))
	{
		name = ILInternAppendedString
			(ILInternString(name, namelen),
			 ILInternString("Attribute", 9)).string;

		if(yyisa(node,ILNode_QualIdent))
		{
			((ILNode_QualIdent *)node)->name = name;
		}
		else if(yyisa(node,ILNode_Identifier))
		{
			((ILNode_Identifier*)node)->name = name;
		}
	}
}

/* A hack to rename the indexer during parsing , damn the C# designers,
 * they had to make the variable names resolved later using an attribute
 * public int <name>[int posn] would have been a cleaner design. But
 * This is an UGLY hack and should be removed as soon as someone figures
 * out how .
 */
static ILNode *GetIndexerName(ILGenInfo *info,ILNode_AttributeTree *attrTree,
								ILNode* prefixName)
{
	ILNode_ListIter iter;
	ILNode_ListIter iter2;
	ILNode *temp;
	ILNode *attr;
	ILNode_List *args;
	ILEvalValue evalValue;
	const char* prefix=(prefixName) ? ILQualIdentName(prefixName,0) : NULL;
	int i;

	const char* possibleWays[] = {"IndexerName", "IndexerNameAttribute",
					"System.Runtime.CompilerServices.IndexerNameAttribute",
					"System.Runtime.CompilerServices.IndexerName"};
	int isIndexerName=0;
	
	if(attrTree && attrTree->sections)
	{
		ILNode_ListIter_Init(&iter, attrTree->sections);
		while((temp = ILNode_ListIter_Next(&iter))!=0)
		{	
			if(!(temp != NULL
					&& yyisa(temp, ILNode_AttributeSection) &&
					((ILNode_AttributeSection*)temp)->attrs != NULL))
			{
				continue;
			}
			
			ILNode_ListIter_Init(&iter2, 
				((ILNode_AttributeSection*)(temp))->attrs);
			while((attr = ILNode_ListIter_Next(&iter2))!=0)
			{
				for(i=0;i<sizeof(possibleWays)/sizeof(char*); i++)
				{
					isIndexerName |= !strcmp(
							ILQualIdentName(((ILNode_Attribute*)attr)->name,0)
							,possibleWays[i]);
				}
				if(isIndexerName)
				{
					/* NOTE: we make it 
					[System.Runtime.CompilerServices.IndexerNameAttribute]
					for the sake of resolution...This too is too ugly a 
					hack.
					*/
					ModifyAttrName(((ILNode_Attribute*)attr)->name,0);

					args=(ILNode_List*)((ILNode_AttrArgs*)
						(((ILNode_Attribute*)attr)->args))->positionalArgs;	
					if(yyisa(args->item1, ILNode_ToConst) &&
					   ILNode_EvalConst(args->item1,info,&evalValue))
					{
						if(evalValue.valueType==ILMachineType_String)
						{
							if(!prefix)
							{
								return ILQualIdentSimple(
									ILInternString(evalValue.un.strValue.str,
										evalValue.un.strValue.len).string);
							}
							else 
							{
								return ILNode_QualIdent_create(prefixName,
									ILInternString(evalValue.un.strValue.str,
										evalValue.un.strValue.len).string);
							}
						}
					}
				}
			}
		}
	}
	if(!prefix)
		return ILQualIdentSimple(ILInternString("Item", 4).string);
	else 
		return ILNode_QualIdent_create(prefixName,
									ILInternString("Item",4).string);
}

/*
 * Mask of valid property accessor modifiers
 */
#define CS_PROPERTY_ACCESSOR_MODIFIERS \
		(CS_MODIFIER_PRIVATE | CS_MODIFIER_PROTECTED | CS_MODIFIER_INTERNAL)

static ILUInt32 GetAccessorModifiers(ILUInt32 propertyModifiers,
									 ILUInt32 accessorModifiers,
									 const char *filename,
									 long linenum)
{
	ILUInt32 modifiers;

	if(accessorModifiers == 0)
	{
		if((propertyModifiers & CS_MODIFIER_PROPERTY_INTERFACE) != 0)
		{
			return propertyModifiers | CS_MODIFIER_METHOD_INTERFACE_ACCESSOR;
		}
		else
		{
			return propertyModifiers | CS_MODIFIER_METHOD_PROPERTY_ACCESSOR;
		}
	}
	if((accessorModifiers & ~CS_PROPERTY_ACCESSOR_MODIFIERS) != 0)
	{
		CCErrorOnLine(filename, linenum,
					  "Invalid modifier used for accessor");
		return propertyModifiers;
	}
	if((accessorModifiers & CS_MODIFIER_PRIVATE) != 0)
	{
		if((accessorModifiers & CS_MODIFIER_PROTECTED) != 0)
		{
			CCErrorOnLine(filename, linenum,
						  "cannot use both `private' and `protected'");
		}
		if((accessorModifiers & CS_MODIFIER_INTERNAL) != 0)
		{
			CCErrorOnLine(filename, linenum,
						  "cannot use both `private' and `internal'");
		}
		modifiers = CS_MODIFIER_PRIVATE;
	}
	else
	{
		modifiers = 0;

		if((accessorModifiers & CS_MODIFIER_PROTECTED) != 0)
		{
			if((accessorModifiers & CS_MODIFIER_INTERNAL) != 0)
			{
				modifiers = (CS_MODIFIER_PROTECTED | CS_MODIFIER_INTERNAL);
			}
			else
			{
				modifiers = CS_MODIFIER_PROTECTED;
			}
		}
		else
		{
			modifiers = CS_MODIFIER_INTERNAL;
		}
	}
	/* Now validate the visibility */
	switch(propertyModifiers & CS_MODIFIER_ACCESS_MASK)
	{
		case CS_MODIFIER_PUBLIC:
		{
			/* OK every accessor modifier is allowed */
		}
		break;

		case (CS_MODIFIER_PROTECTED | CS_MODIFIER_INTERNAL):
		{
			if((modifiers == CS_MODIFIER_INTERNAL) ||
			   (modifiers == CS_MODIFIER_PROTECTED) ||
			   (modifiers == CS_MODIFIER_PRIVATE))
			{
				/* OK */
			}
			else
			{
				CCErrorOnLine(filename, linenum,
						"cannot use `protected' `internal' in this context");
			}
		}
		break;

		case CS_MODIFIER_PROTECTED:
		case CS_MODIFIER_INTERNAL:
		{
			if(modifiers != CS_MODIFIER_PRIVATE)
			{
				CCErrorOnLine(filename, linenum,
						"only `private' is allowed in this context");
			}
		}
		break;

		default:
		{
			CCErrorOnLine(filename, linenum,
						"no modifiers are allowed in this context");
		}
	}
	modifiers |= CS_MODIFIER_METHOD_PROPERTY_ACCESSOR;
	return modifiers;
}

/*
 * Adjust the name of a property to include a "get_" or "set_" prefix.
 */
static ILNode *AdjustPropertyName(ILNode *name, char *prefix)
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
 * Create the methods needed by a property definition.
 */
static void CreatePropertyMethods(ILNode_PropertyDeclaration *property,
								  struct PropertyAccessors *accessors)
{
	ILNode_MethodDeclaration *decl;
	ILNode *name;
	ILNode *params;
	ILNode_ListIter iter;
	ILNode *temp;

	/* Create the "get" method */
	if(accessors->getAccessor.present != 0)
	{
		ILUInt32 modifiers;

		name = AdjustPropertyName(property->name, "get_");
		modifiers = GetAccessorModifiers(property->modifiers,
										 accessors->getAccessor.modifiers,
										 accessors->getAccessor.filename,
										 accessors->getAccessor.linenum);
		decl = (ILNode_MethodDeclaration *)
				ILNode_MethodDeclaration_create
						(accessors->getAccessor.attributes,
						 modifiers, property->type,
						 name, 0, property->params,
						 accessors->getAccessor.body);
		yysetfilename(decl, accessors->getAccessor.filename);
		yysetlinenum(decl, accessors->getAccessor.linenum);
		property->getAccessor = (ILNode *)decl;
	}

	/* Create the "set" method */
	if(accessors->setAccessor.present != 0)
	{
		ILUInt32 modifiers;

		name = AdjustPropertyName(property->name, "set_");
		modifiers = GetAccessorModifiers(property->modifiers,
										 accessors->setAccessor.modifiers,
										 accessors->setAccessor.filename,
										 accessors->setAccessor.linenum);
		params = ILNode_List_create();
		ILNode_ListIter_Init(&iter, property->params);
		while((temp = ILNode_ListIter_Next(&iter)) != 0)
		{
			ILNode_List_Add(params, temp);
		}
		ILNode_List_Add(params,
			ILNode_FormalParameter_create(0, ILParamMod_empty, property->type,
					ILQualIdentSimple(ILInternString("value", 5).string)));
		decl = (ILNode_MethodDeclaration *)
			ILNode_MethodDeclaration_create
					(accessors->setAccessor.attributes,
					 modifiers, 0, name, 0, params,
					 accessors->setAccessor.body);
		yysetfilename(decl, accessors->setAccessor.filename);
		yysetlinenum(decl, accessors->setAccessor.linenum);
		property->setAccessor = (ILNode *)decl;
	}
}

#if IL_VERSION_MAJOR > 1
/*
 * Check if the generic constraints have a matching generic type
 * and move the constraint data to that type.
 */
static void MergeGenericConstraints(ILNode_GenericTypeParameters *genericTypeParameters,
									ILNode_List *constraints)
{
	ILNode_ListIter iter;
	ILNode_GenericConstraint *constraint;
	ILNode_GenericTypeParameter *parameter;

	ILNode_ListIter_Init(&iter, constraints);
	while((constraint = (ILNode_GenericConstraint *)ILNode_ListIter_Next(&iter)) != 0)
	{
		ILNode_ListIter iter2;

		ILNode_ListIter_Init(&iter2, genericTypeParameters->typeParams);
		while((parameter = (ILNode_GenericTypeParameter *)ILNode_ListIter_Next(&iter2)) != 0)
		{
			if(!strcmp(constraint->name, parameter->name))
			{
				parameter->constraint = constraint->constraint;
				parameter->typeConstraints = constraint->typeConstraints;
				break;
			}
		}
		if(!parameter)
		{
			/* If we get here a parameter with the name in the constraint could not be found. */
			CCErrorOnLine(yygetfilename(constraint), yygetlinenum(constraint),
						  "`%s' is no generic parameter",
						  constraint->name);
		}
	}
}

/*
 * Convert type arguments to type parameters and check if each argument is an identifier.
 */
static ILNode_GenericTypeParameters *TypeActualsToTypeFormals(ILNode *typeArguments)
{

	ILNode_ListIter iter;
	ILNode *node;
	ILNode* list = 0;
	ILUInt32 count = 0;

	ILNode_ListIter_Init(&iter, typeArguments);
	while((node = ILNode_ListIter_Next(&iter)) != 0)
	{	
		if(yyisa(node, ILNode_Identifier))
		{
			/* Check for duplicates in the list */
			const char *name = ILQualIdentName(node, 0);
			ILNode_ListIter iter2;
			ILNode_GenericTypeParameter *genParam;

			ILNode_ListIter_Init(&iter2, list);
			while((genParam = (ILNode_GenericTypeParameter *)ILNode_ListIter_Next(&iter2)) != 0)
			{
				if(!strcmp(genParam->name, name))
				{
					CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
								  "`%s' declared multiple times in generic parameters",
								  name);
					break;
				}
			}

			/* Add the generic parameter to the list */
			list = MakeList(list,
						(ILNode *)ILNode_GenericTypeParameter_create(count,
																	 name,
																	 0, 0));
			count++;
		}
		else
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "expected an identifier and not a type");
		}
	}
	if(count > 0)
	{
		return (ILNode_GenericTypeParameters *)ILNode_GenericTypeParameters_create(count, (ILNode_List *)list);
	}
	return 0;
}
#endif	/* IL_VERSION_MAJOR > 1 */

%}

/*
 * Define the structure of yylval.
 */
%union {
	struct
	{
		ILUInt64	value;
		int			type;
		int			canneg;
	}					integer;
	struct
	{
		ILDouble	value;
		int			type;
	}					real;
	ILDecimal  			decimal;
	ILUInt16			charValue;
	ILIntString			string;
	const char		   *name;
	ILUInt32			count;
	ILUInt32			mask;
	ILNode			   *node;
	struct
	{
		ILNode		   *type;
		const char	   *id;
		ILNode         *idNode;
	} catchinfo;
	struct
	{
		ILNode		   *item1;
		ILNode		   *item2;
	} pair;
	ILParameterModifier	pmod;
	struct
	{
		char           *binary;
		char           *unary;

	} opName;
	struct
	{
		ILNode		   *ident;
		ILNode		   *params;

	} indexer;
	struct
	{
		ILNode		   *decl;
		ILNode		   *init;

	} varInit;
	struct
	{
		ILNode		   *body;
		ILNode		   *staticCtors;

	} member;
	struct
	{
		ILAttrTargetType targetType;
		ILNode		   *target;

	} target;
	int					partial;
	ILNode_List		   *nodeList;
	struct
	{
		ILUInt32		count;
		ILNode_List	   *list;
	}					countList;
	struct MemberName	memberName;
	struct MemberAccess	memberAccess;
	ILNode_GenericTypeParameters *genericTypeParameters;
	struct
	{
		ILUInt32		constraint;
		ILNode_List	   *typeConstraints;
	}					constraint;
	struct
	{
		ILNode		   *attributes;
		ILUInt32		modifiers;
	}				attributesAndModifiers;
	struct
	{
		ILNode		   *attributes;
		ILUInt32		modifiers;
	}				typeHeader;
	struct
	{
		ILNode		   *attributes;
		ILUInt32		modifiers;
		ILNode		   *identifier;
		ILNode		   *classBase;
		ILNode_GenericTypeParameters *typeFormals;
	}					classHeader;
	struct
	{
		ILNode		   *attributes;
		ILUInt32		modifiers;
		ILNode		   *type;
	}				memberHeaderStart;
	struct
	{
		ILNode		   *attributes;
		ILUInt32		modifiers;
		ILNode		   *type;
		ILNode		   *identifier;
	}				nonGenericMethodAndPropertyHeaderStart;
	struct
	{
		ILNode		   *attributes;
		ILUInt32		modifiers;
		ILNode		   *type;
		ILNode		   *identifier;
		ILNode_GenericTypeParameters *typeFormals;
	}				genericMethodHeaderStart;
	struct
	{
		ILNode		   *attributes;
		ILUInt32		modifiers;
		ILNode		   *type;
		ILNode		   *identifier;
		ILNode_List	   *args;
		ILNode_GenericTypeParameters *typeFormals;
	}					memberHeader;
	struct ArrayRanks	arrayRanks;
	struct ArrayType	arrayType;
	struct Accessor		accessor;
	struct PropertyAccessors	propertyAccessors;
}

/*
 * simple operator precedence.
 */
%nonassoc IDENTIFIER_OP
%right '=' MUL_ASSIGN_OP DIV_ASSIGN_OP MOD_ASSIGN_OP ADD_ASSIGN_OP SUB_ASSIGN_OP LEFT_ASSIGN_OP RIGHT_ASSIGN_OP AND_ASSIGN_OP XOR_ASSIGN_OP OR_ASSIGN_OP
%left OR_OP
%left AND_OP
%left '|'
%left '&'
%left EQ_OP NE_OP
%left '<' '>' LE_OP GE_OP AS IS GT_OP
%left LEFT_OP RIGHT_OP
%left '+' '-'
%left '*' '/' '%'
%left UN_PLUS UN_MINUS '!' '~' UN_PRE_INC UN_PRE_DEC CAST ADDRESS_OF
%left '.' UN_POST_INC UN_POST_DEC TYPEOF CHECKED UNCHECKED NEW PTR_OP 

/*
 * Primitive lexical tokens.
 */
%token INTEGER_CONSTANT		"an integer value"
%token CHAR_CONSTANT		"a character constant"
%token IDENTIFIER_LEXICAL	"an identifier"
%token STRING_LITERAL		"a string literal"
%token FLOAT_CONSTANT		"a floating point value"
%token DECIMAL_CONSTANT		"a decimal value"
%token DOC_COMMENT			"a documentation comment"
%token DEFAULT_LABEL		"the default label token"

/*
 * Keywords.
 */
%token ABSTRACT				"`abstract'"
%token ADD					"`add'"
%token ARGLIST				"`__arglist'"
%token AS					"`as'"
%token BASE					"`base'"
%token BOOL					"`bool'"
%token BREAK				"`break'"
%token BUILTIN_CONSTANT		"`__builtin_constant'"
%token BYTE					"`byte'"
%token CASE					"`case'"
%token CATCH				"`catch'"
%token CHAR					"`char'"
%token CHECKED				"`checked'"
%token CLASS				"`class'"
%token CONST				"`const'"
%token CONTINUE				"`continue'"
%token DECIMAL				"`decimal'"
%token DEFAULT				"`default'"
%token DELEGATE				"`delegate'"
%token DO					"`do'"
%token DOUBLE				"`double'"
%token ELSE					"`else'"
%token ENUM					"`enum'"
%token EVENT				"`event'"
%token EXPLICIT				"`explicit'"
%token EXTERN				"`extern'"
%token FALSE				"`false'"
%token FINALLY				"`finally'"
%token FIXED				"`fixed'"
%token FLOAT				"`float'"
%token FOR					"`for'"
%token FOREACH				"`foreach'"
%token GET					"`get'"
%token GOTO					"`goto'"
%token IF					"`if'"
%token IMPLICIT				"`implicit'"
%token IN					"`in'"
%token INT					"`int'"
%token INTERFACE			"`interface'"
%token INTERNAL				"`internal'"
%token IS					"`is'"
%token LOCK					"`lock'"
%token LONG					"`long'"
%token LONG_DOUBLE			"`__long_double'"
%token MAKEREF				"`__makeref'"
%token MODULE               "`__module'"
%token NAMESPACE			"`namespace'"
%token NEW					"`new'"
%token NULL_TOK				"`null'"
%token OBJECT				"`object'"
%token OPERATOR				"`operator'"
%token OUT					"`out'"
%token OVERRIDE				"`override'"
%token PARAMS				"`params'"
%token PARTIAL				"`partial'"
%token PRIVATE				"`private'"
%token PROTECTED			"`protected'"
%token PUBLIC				"`public'"
%token READONLY				"`readonly'"
%token REMOVE				"`remove'"
%token REF					"`ref'"
%token REFTYPE				"`__reftype'"
%token REFVALUE				"`__refvalue'"
%token RETURN				"`return'"
%token SBYTE				"`sbyte'"
%token SEALED				"`sealed'"
%token SET					"`set'"
%token SHORT				"`short'"
%token SIZEOF				"`sizeof'"
%token STACKALLOC			"`stackalloc'"
%token STATIC				"`static'"
%token STRING				"`string'"
%token STRUCT				"`struct'"
%token SWITCH				"`switch'"
%token THIS					"`this'"
%token THROW				"`throw'"
%token TRUE					"`true'"
%token TRY					"`try'"
%token TYPEOF				"`typeof'"
%token UINT					"`uint'"
%token ULONG				"`ulong'"
%token UNCHECKED			"`unchecked'"
%token UNSAFE				"`unsafe'"
%token USHORT				"`ushort'"
%token USING				"`using'"
%token VIRTUAL				"`virtual'"
%token VOID					"`void'"
%token VOLATILE				"`volatile'"
%token WHERE				"`where'"
%token WHILE				"`while'"
%token YIELD				"`yield'"

/*
 * Operators.
 */
%token INC_OP				"`++'"
%token DEC_OP				"`--'"
%token LEFT_OP				"`<<'"
%token LE_OP				"`<='"
%token GE_OP				"`>='"
%token EQ_OP				"`=='"
%token NE_OP				"`!='"
%token AND_OP				"`&&'"
%token OR_OP				"`||'"
%token MUL_ASSIGN_OP		"`*='"
%token DIV_ASSIGN_OP		"`/='"
%token MOD_ASSIGN_OP		"`%='"
%token ADD_ASSIGN_OP		"`+='"
%token SUB_ASSIGN_OP		"`-='"
%token LEFT_ASSIGN_OP		"`<<='"
%token RIGHT_ASSIGN_OP		"`>>='"
%token AND_ASSIGN_OP		"`&='"
%token XOR_ASSIGN_OP		"`^='"
%token OR_ASSIGN_OP			"`|='"
%token PTR_OP				"`->'"
%token NULL_COALESCING_OP	"`??'"
%token QUALIFIED_ALIAS_OP	"`::'"

/*
 * Define the yylval types of the various non-terminals.
 */
%type <name>		IDENTIFIER IDENTIFIER_LEXICAL
%type <integer>		INTEGER_CONSTANT
%type <charValue>	CHAR_CONSTANT
%type <real>		FLOAT_CONSTANT
%type <decimal>		DECIMAL_CONSTANT
%type <string>		STRING_LITERAL DOC_COMMENT NamespaceIdentifier
%type <count>		DimensionSeparators DimensionSeparatorList
%type <count>		RankSpecifier
%type <arrayRanks>	RankSpecifiers 
%type <mask>		Modifiers Modifier
%type <partial>		OptPartial

%type <node>		Identifier GenericIdentifierStart
%type <node>		QualifiedIdentifier NonGenericQualifiedIdentifier
%type <memberAccess> SimpleQualifiedIdentifier
%type <memberAccess> GenericQualifiedIdentifierStart
%type <memberAccess> GenericQualifiedIdentifier
%type <node>		QualifiedIdentifierMemberAccessStart

%type <node>		BuiltinType
%type <node>		NonArrayType ArrayType
%type <node>		PointerType
%type <node>		ArrayTypeStart
%type <arrayType>	ArrayTypeContinue
%type <node>		Type
%type <node>		PrimaryTypeExpression
%type <node>		PrimaryMemberAccessExpression
%type <node>		PrimaryMemberAccessStart
%type <node>		PrimaryTypeExpressionPart
%type <node>		LocalVariableType
%type <node>		LocalVariablePointerType
%type <node>		LocalVariableNonArrayType
%type <node>		LocalVariableArrayTypeStart
%type <arrayType>	LocalVariableArrayTypeContinue
%type <node>		LocalVariableArrayType

%type <node>		TypeDeclaration ClassDeclaration ClassBase TypeList
%type <member>		ClassBody OptClassMemberDeclarations
%type <member>		ClassMemberDeclarations ClassMemberDeclaration
%type <member>		StructBody
%type <node> 		StructDeclaration StructInterfaces ModuleDeclaration

%type <node>		ElementAccess
%type <node>		PrimaryArrayCreationExpression
%type <node>		PrimaryNonTypeExpression
%type <node>		PrimarySimpleExpression
%type <node>		PrimaryExpression
%type <node>		SimpleCastExpression CastExpression
%type <node>		Expression
%type <node>		UnaryExpression UnaryNonTypeExpression
%type <node>		MultiplicativeExpression MultiplicativeNonTypeExpression
%type <node>		AdditiveExpression AdditiveNonTypeExpression
%type <node>		ShiftExpression ShiftNonTypeExpression
%type <node>		RightShiftExpressionStart RightShiftExpression
%type <node>		LeftShiftExpression
%type <node>		RelationalExpression RelationalNonTypeExpression
%type <node>		RelationalGTExpression RelationalNonGTExpression
%type <node>		EqualityExpression EqualityNonTypeExpression
%type <node>		AndExpression XorExpression OrExpression
%type <node>		LogicalAndExpression LogicalOrExpression
%type <node>		ConditionalExpression AssignmentExpression
%type <node>		ParenExpression ConstantExpression BooleanExpression
%type <node>		ParenBooleanExpression LiteralExpression
%type <node>		InvocationExpression ExpressionList
%type <node>		ObjectCreationExpression
%type <node>		PreIncrementExpression PreDecrementExpression
%type <node>		PostIncrementExpression PostDecrementExpression
%type <node>		OptArgumentList ArgumentList
%type <node>		Argument PrefixedUnaryExpression PrefixedUnaryNonTypeExpression
%type <node>		AnonymousMethod

%type <node>		Statement EmbeddedStatement Block OptStatementList
%type <node>		StatementList ExpressionStatement SelectionStatement
%type <node>		SwitchBlock OptSwitchSections SwitchSections
%type <node>		SwitchSection SwitchLabels SwitchLabel IterationStatement
%type <node>		ForInitializer ForInitializerInner ForCondition
%type <node>		ForIterator ForeachExpression ExpressionStatementList
%type <node>		JumpStatement TryStatement CatchClauses LineStatement
%type <node>		SpecificCatchClauses
%type <node>		SpecificCatchClause OptGeneralCatchClause
%type <node>		GeneralCatchClause FinallyClause LockStatement
%type <node>		UsingStatement ResourceAcquisition FixedStatement
%type <node>		FixedPointerDeclarators FixedPointerDeclarator
%type <node>		InnerEmbeddedStatement InnerExpressionStatement
%type <node>		YieldStatement DefaultValueExpression

%type <memberHeaderStart> MemberHeaderStart
%type <nonGenericMethodAndPropertyHeaderStart> NonGenericMethodAndPropertyHeaderStart
%type <genericMethodHeaderStart> GenericMethodHeaderStart
%type <node>		ConstantDeclaration ConstantDeclarators ConstantDeclarator
%type <node>		FieldDeclaration FieldDeclarators FieldDeclarator
%type <node>		VariableDeclarators VariableDeclarator
%type <node>		VariableInitializer LocalVariableDeclaration
%type <node>		LocalConstantDeclaration
%type <node>		EventFieldDeclaration EventDeclaration 
%type <node>		EventPropertyDeclaration EventDeclarators EventDeclarator
%type <pair>		EventAccessorBlock EventAccessorDeclarations

%type <node>		MethodDeclaration MethodBody
%type <node>		OptFormalParameterList FormalParameterList FormalParameter
%type <pmod>		ParameterModifier
%type <node>		PropertyDeclaration 
%type <propertyAccessors>	AccessorBlock AccessorDeclarations
%type <accessor>	OptGetAccessorDeclaration GetAccessorDeclaration
%type <accessor>	OptSetAccessorDeclaration SetAccessorDeclaration
%type <node>		AccessorBody
%type <node>		AddAccessorDeclaration RemoveAccessorDeclaration
%type <node>		IndexerDeclaration
%type <node>		FormalIndexParameters FormalIndexParameter
%type <node>		FormalIndexParameterList
%type <node>		InterfaceDeclaration InterfaceBase InterfaceBody
%type <node>		OptInterfaceMemberDeclarations /*InterfaceMemberDeclarations
%type <node>		InterfaceMemberDeclarations InterfaceMemberDeclaration*/
%type <node>		InterfaceMemberDeclaration InterfaceMemberDeclarations
%type <node>		InterfaceMethodDeclaration InterfacePropertyDeclaration
%type <node>		InterfaceIndexerDeclaration InterfaceEventDeclaration
%type <propertyAccessors>	InterfaceAccessors InterfaceAccessorBody
%type <mask>		OptNew
%type <node>		EnumDeclaration EnumBody OptEnumMemberDeclarations
%type <node>		EnumMemberDeclarations EnumMemberDeclaration
%type <node>		EnumBase EnumBaseType
%type <node>		DelegateDeclaration ConstructorInitializer
%type <member>		ConstructorDeclaration
%type <node>		DestructorDeclaration
%type <node>		OperatorDeclaration NormalOperatorDeclaration
%type <node>		ConversionOperatorDeclaration
%type <opName>		OverloadableOperator
%type <node>		OptAttributes AttributeSections AttributeSection
%type <node>		AttributeList Attribute AttributeArguments
%type <node>		NonOptAttributes
%type <attributesAndModifiers> OptAttributesAndModifiers AttributesAndModifiers
%type <typeHeader>	OptTypeDeclarationHeader
%type <node>		PositionalArgumentList PositionalArgument NamedArgumentList
%type <node>		NamedArgument AttributeArgumentExpression
%type <node>		OptArrayInitializer ArrayInitializer
%type <node>		OptVariableInitializerList VariableInitializerList
%type <countList>	TypeActuals
%type <genericTypeParameters>	TypeFormals
%type <countList>	TypeFormalList
%type <mask>		PrimaryConstraint ConstructorConstraint
%type <nodeList>	SecondaryConstraints
%type <constraint>  TypeParameterConstraints
%type <node>		TypeParameterConstraintsClause
%type <nodeList>	TypeParameterConstraintsClauses
%type <nodeList>	OptTypeParameterConstraintsClauses
%type <classHeader>	ClassHeader InterfaceHeader StructHeader
%type <memberHeader> MethodHeader DelegateHeader InterfaceMethodHeader
%type <indexer>		IndexerDeclarator
%type <catchinfo>	CatchNameInfo
%type <target>		AttributeTarget

%expect 18

%start CompilationUnit
%%

/*
 * Outer level of the C# input file.
 */

CompilationUnit
	: /* empty */	{
				/* The input file is empty */
				CCTypedWarning("-empty-input",
							   "file contains no declarations");
				ResetState();
			}
	| OuterDeclarationsRecoverable		{
				/* Check for empty input and finalize the parse */
				if(!HaveDecls)
				{
					CCTypedWarning("-empty-input",
								   "file contains no declarations");
				}
				ResetState();
			}
	| OuterDeclarationsRecoverable NonOptAttributes	{
				/* A file that contains declarations and assembly attributes */
				if($2)
				{
					InitGlobalNamespace();
					CCPluginAddStandaloneAttrs
						(ILNode_StandaloneAttr_create
							((ILNode*)CurrNamespaceNode, $2));
				}
				ResetState();
			}
	| NonOptAttributes	{
				/* A file that contains only assembly attributes */
				if($1)
				{
					InitGlobalNamespace();
					CCPluginAddStandaloneAttrs
						(ILNode_StandaloneAttr_create
							((ILNode*)CurrNamespaceNode, $1));
				}
				ResetState();
			}
	;

/*
 * Note: strictly speaking, declarations should be ordered so
 * that using declarations always come before namespace members.
 * We have relaxed this to make error recovery easier.
 */
OuterDeclarations
	: OuterDeclaration
	| OuterDeclarations OuterDeclaration
	;

OuterDeclaration
	: UsingDirective
	| NamespaceMemberDeclaration
	| error			{
				/*
				 * This production recovers from errors at the outer level
				 * by skipping invalid tokens until a namespace, using,
				 * type declaration, or attribute, is encountered.
				 */
			#ifdef YYEOF
				while(yychar != YYEOF)
			#else
				while(yychar >= 0)
			#endif
				{
					if(yychar == NAMESPACE || yychar == USING ||
					   yychar == PUBLIC || yychar == INTERNAL ||
					   yychar == UNSAFE || yychar == SEALED ||
					   yychar == ABSTRACT || yychar == CLASS ||
					   yychar == STRUCT || yychar == DELEGATE ||
					   yychar == ENUM || yychar == INTERFACE ||
					   yychar == '[')
					{
						/* This token starts a new outer-level declaration */
						break;
					}
					else if(yychar == '}' && CurrNamespace.len != 0)
					{
						/* Probably the end of the enclosing namespace */
						break;
					}
					else if(yychar == ';')
					{
						/* Probably the end of an outer-level declaration,
						   so restart the parser on the next token */
						yychar = YYLEX;
						break;
					}
					yychar = YYLEX;
				}
			#ifdef YYEOF
				if(yychar != YYEOF)
			#else
				if(yychar >= 0)
			#endif
				{
					yyerrok;
				}
				NestingLevel = 0;
			}
	;

OuterDeclarationsRecoverable
	: OuterDeclarationRecoverable
	| OuterDeclarationsRecoverable OuterDeclarationRecoverable
	;

OuterDeclarationRecoverable
	: OuterDeclaration
	| '}'				{
				/* Recover from our educated guess that we were at the
				   end of a namespace scope in the error processing code
				   for '}' above.  If the programmer wrote "namespace XXX }"
				   instead of "namespace { XXX }", this code will stop the
				   error processing logic from looping indefinitely */
				if(CurrNamespace.len == 0)
				{
					CCError(_("parse error at or near `}'"));
				}
				else
				{
					CurrNamespace = ILInternString("", 0);
				}
			}
	;

/*
 * Identifiers.
 */

Identifier
	: IDENTIFIER		{
				/* Build an undistinguished identifier node.  At this
				   point, we have no idea of the identifier's type.
				   We leave that up to the semantic analysis phase */
				$$ = ILQualIdentSimple($1);
			}
	;

GenericIdentifierStart
	: Identifier '<'		{ $$ = $1; }
	;

IDENTIFIER
	: IDENTIFIER_LEXICAL	{ $$ = ILInternString($1, strlen($1)).string; }
	| GET					{ $$ = ILInternString("get", 3).string; }
	| SET					{ $$ = ILInternString("set", 3).string; }
	| ADD					{ $$ = ILInternString("add", 3).string; }
	| REMOVE				{ $$ = ILInternString("remove", 6).string; }
	| PARTIAL				{ $$ = ILInternString("partial", 7).string; }
	| WHERE					{ $$ = ILInternString("where", 5).string; }
	| YIELD					{ $$ = ILInternString("yield", 5).string; }
	;

QualifiedIdentifier
	: NonGenericQualifiedIdentifier		{ $$ = $1; }
	| GenericQualifiedIdentifier			{
				MakeQuaternary(GenericQualIdent,
							   $1.parent,
							   $1.memberName.identifier,
							   $1.memberName.numTypeArgs,
							   (ILNode *)($1.memberName.typeArgs));
			}
	;

/*
 * A qualified identifier without a generic reference at the last part
 */
SimpleQualifiedIdentifier
	: QualifiedIdentifierMemberAccessStart IDENTIFIER	{
				$$.parent = $1;
				$$.memberName.identifier = $2;
				$$.memberName.numTypeArgs = 0;
				$$.memberName.typeArgs = 0;
			}
	;

GenericQualifiedIdentifier
	: GenericQualifiedIdentifierStart TypeActuals '>'	{
				$$.parent = $1.parent;
				$$.memberName.identifier = $1.memberName.identifier;
				$$.memberName.numTypeArgs = $2.count;
				$$.memberName.typeArgs = $2.list;
			}
	;

GenericQualifiedIdentifierStart
	: GenericIdentifierStart			{
				$$.parent = 0;
				$$.memberName.identifier = ILQualIdentGetName($1);
				$$.memberName.numTypeArgs = 0;
				$$.memberName.typeArgs = 0;
			}
	| SimpleQualifiedIdentifier '<'		{
				$$.parent = $1.parent;
				$$.memberName.identifier = $1.memberName.identifier;
				$$.memberName.numTypeArgs = $1.memberName.numTypeArgs;
				$$.memberName.typeArgs = $1.memberName.typeArgs;
			}
	;

NonGenericQualifiedIdentifier
	: Identifier						{ $$ = $1; } %prec IDENTIFIER_OP
	| SimpleQualifiedIdentifier			{
				MakeBinary(QualIdent,
						   $1.parent,
						   $1.memberName.identifier);
			} %prec IDENTIFIER_OP
	;

QualifiedIdentifierMemberAccessStart
	: NonGenericQualifiedIdentifier '.'	{
				 $$ = $1;
			}
	| GenericQualifiedIdentifier '.'	{
				MakeQuaternary(GenericQualIdent,
							   $1.parent,
							   $1.memberName.identifier,
							   $1.memberName.numTypeArgs,
							   (ILNode *)($1.memberName.typeArgs));
			}
	;

/*
 * Namespaces.
 */

/*
 * Note: strictly speaking, namespaces don't have attributes.
 * The C# standard allows attributes that apply to the assembly
 * to appear at the global level.  To avoid reduce/reduce conflicts
 * in the grammar, we cannot make the attributes a separate rule
 * at the outer level.  So, we parse assembly attributes as if
 * they were attached to types or namespaces, and detach them
 * during semantic analysis.
 */
NamespaceDeclaration
	: OptAttributes NAMESPACE NamespaceIdentifier {
				int posn, len;

				/* Initialize the global Namespace (CompilationUnit) */
				InitGlobalNamespace();
					
				posn = 0;
				if($1)
				{
					CCPluginAddStandaloneAttrs
						(ILNode_StandaloneAttr_create
							((ILNode*)CurrNamespaceNode, $1));
				}
				while(posn < $3.len)
				{
					/* Extract the next identifier */
					if($3.string[posn] == '.')
					{
						++posn;
						continue;
					}
					len = 0;
					while((posn + len) < $3.len &&
						  $3.string[posn + len] != '.')
					{
						++len;
					}

					/* Push a new identifier onto the end of the namespace */
					if(CurrNamespace.len != 0)
					{
						CurrNamespace = ILInternAppendedString
							(CurrNamespace,
							 ILInternAppendedString
							 	(ILInternString(".", 1),
								 ILInternString($3.string + posn, len)));
					}
					else
					{
						CurrNamespace = ILInternString($3.string + posn, len);
					}

					/* Create a namespace node for the new entered namespace. */
					CurrNamespaceNode = (ILNode_Namespace *)
						ILNode_Namespace_create(CurrNamespace.string,
												CurrNamespaceNode);

					CurrNamespaceNode->localScope =
								ILScopeDeclareNamespace(GlobalScope(),
														CurrNamespace.string);

					/* Move on to the next namespace component */
					posn += len;
				}
			}
			NamespaceBody OptSemiColon	{
				/* Pop the identifier from the end of the namespace */
				if(CurrNamespace.len == $3.len)
				{
					CurrNamespace = ILInternString("", 0);
					while(CurrNamespaceNode->enclosing != 0)
					{
						CurrNamespaceNode = CurrNamespaceNode->enclosing;
					}
				}
				else
				{
					CurrNamespace = ILInternString
						(CurrNamespace.string, CurrNamespace.len - $3.len - 1);
					while(CurrNamespaceNode->name != CurrNamespace.string)
					{
						CurrNamespaceNode = CurrNamespaceNode->enclosing;
					}
				}
			}
	;

NamespaceIdentifier
	: IDENTIFIER		{ $$ = ILInternString($1, strlen($1)); }
	| NamespaceIdentifier '.' IDENTIFIER	{
				$$ = ILInternAppendedString
					($1, ILInternAppendedString
					 		(ILInternString(".", 1),
							 ILInternString($3, strlen($3))));
			}
	;

OptSemiColon
	: /* empty */
	| ';'
	;

NamespaceBody
	: '{' OptNamespaceMemberDeclarations '}'
	;

UsingDirective
	: USING IDENTIFIER '=' QualifiedIdentifier ';'	{
				const char *internedAliasName;
				ILNode *alias;

				InitGlobalNamespace();
				internedAliasName = ILInternString($2, strlen($2)).string;
				alias = ILNamespaceResolveAlias(CurrNamespaceNode, internedAliasName, 0);
				if(alias)
				{
					CCError("the alias `%s' is already declared", $2);
				}
				else
				{
					alias = ILNode_UsingAlias_create(internedAliasName, $4);
					/* NOTE: CSSemGuard is not needed as ILNode_UsingAlias is
					         never Semanalyzed */
					ILNamespaceAddAlias(CurrNamespaceNode, (ILNode_Alias *)alias);
				}
			}
	| USING NamespaceIdentifier ';'		{
				InitGlobalNamespace();
				ILNamespaceAddUsing(CurrNamespaceNode,
					(ILNode_UsingNamespace *)ILNode_UsingNamespace_create($2.string));
			}
	;

OptNamespaceMemberDeclarations
	: /* empty */
	| OuterDeclarations
	;

NamespaceMemberDeclaration
	: NamespaceDeclaration
	| TypeDeclaration			{ CCPluginAddTopLevel($1); }
	;

TypeDeclaration
	: ClassDeclaration			{ $$ = $1; }
	| ModuleDeclaration			{ $$ = $1; }
	| StructDeclaration			{ $$ = $1; }
	| InterfaceDeclaration		{ $$ = $1; }
	| EnumDeclaration			{ $$ = $1; }
	| DelegateDeclaration		{ $$ = $1; }
	;

/*
 * Types.
 */

NonArrayType
	: BuiltinType				{ $$ = $1; }
	| QualifiedIdentifier		{ $$ = $1; }
	| PointerType				{ $$ = $1; }
	;

ArrayTypeStart
	: NonArrayType '['			{ $$ = $1; }
	;

ArrayTypeContinue
	: ArrayTypeStart ']'		{
				ArrayTypeInit(&($$), $1, 1);
			}
	|  ArrayTypeStart DimensionSeparatorList ']' {
				ArrayTypeInit(&($$), $1, $2);
			}
	| ArrayTypeContinue RankSpecifier	{
				ArrayTypeAddRank(&($$), &($1), $2);
			}
	;

ArrayType
	: ArrayTypeContinue	{
				$$ = ArrayTypeCreate($1.type, &($1.ranks));
			}
	;

PointerType
	: Type '*'					{
				MakeUnary(PtrType, $1);
			}
	;

Type
	: NonArrayType				{ $$ = $1; } %prec IDENTIFIER_OP 
	| ArrayType					{ $$ = $1; } %prec IDENTIFIER_OP
	;

RankSpecifiers
	: RankSpecifier				{
				ArrayRanksInit(&($$), $1);
			}
	| RankSpecifiers RankSpecifier {
				ArrayRanksAddRank(&($$), &($1), $2);
			}
	;

RankSpecifier
	: '[' DimensionSeparators ']'	{ $$ = $2; }
	;

TypeActuals
	: Type						{
				 $$.count = 1;
				 $$.list = (ILNode_List *)MakeList(0, $1);
			}
	| TypeActuals ',' Type		{
				$$.count = $1.count + 1;
				$$.list = (ILNode_List *)MakeList((ILNode *)($1.list), $3);
			}
	;

/*
 * Types in local variable declarations must be recognized as
 * expressions to prevent reduce/reduce errors in the grammar.
 * The expressions are converted into types during semantic analysis.
 */

PrimaryTypeExpression
	: PrimaryTypeExpressionPart		{ $$ = $1; }
	| PrimaryMemberAccessExpression	{ $$ = $1; }
	;

PrimaryMemberAccessStart
	: BuiltinType '.'				{ $$ = $1; }
	| PrimaryTypeExpressionPart '.'	{ $$ = $1; }
	| PrimaryMemberAccessExpression '.'	{ $$ = $1; }
	;

PrimaryTypeExpressionPart
	: Identifier					{ $$ = $1; }
	;

PrimaryMemberAccessExpression
	: PrimaryMemberAccessStart PrimaryTypeExpressionPart {
				MakeBinary(MemberAccess, $1, $2);
			}
	;

LocalVariableType
	: LocalVariableNonArrayType		{ $$ = $1; }
	| LocalVariableArrayType		{ $$ = $1; }
	;

LocalVariableNonArrayType
	: BuiltinType					{ $$ = $1; }
	| PrimaryTypeExpression			{ $$ = $1; }
	| LocalVariablePointerType		{ $$ = $1; }
	;

/*
 * This is needed to fix reduce/reduce errors between the array type 
 * declaration and the element access expression.
 */
LocalVariableArrayTypeStart
	: PrimaryTypeExpression '['		{ $$ = $1; }
	;

LocalVariableArrayTypeContinue
	: BuiltinType RankSpecifier				{
				ArrayTypeInit(&($$), $1, $2);
			 }
	| LocalVariablePointerType RankSpecifier {
				ArrayTypeInit(&($$), $1, $2);
			 }
	| LocalVariableArrayTypeStart ']'		{
				ArrayTypeInit(&($$), $1, 1);
			}
	| LocalVariableArrayTypeStart DimensionSeparatorList ']' {
				ArrayTypeInit(&($$), $1, $2);
			}
	| LocalVariableArrayTypeContinue RankSpecifier	{
				ArrayTypeAddRank(&($$), &($1), $2);
			}
	;

LocalVariableArrayType
	: LocalVariableArrayTypeContinue	{
				$$ = ArrayTypeCreate($1.type, &($1.ranks));
			}
	;

/*
 * Pointer types
 */
LocalVariablePointerType
	: BuiltinType '*'					{
 				MakeUnary(PtrType, $1);
			}
	| PrimaryTypeExpression '*'			{
				MakeUnary(PtrType, $1);
			} 
	| LocalVariableArrayType '*'		{
				MakeUnary(PtrType, $1);
			}
	| LocalVariablePointerType '*'		{
				MakeUnary(PtrType, $1);
			} 
			;

DimensionSeparators
	: /* empty */					{ $$ = 1; }
	| DimensionSeparatorList		{ $$ = $1; }
	;

DimensionSeparatorList
	: ','							{ $$ = 2; }
	| DimensionSeparatorList ','	{ $$ = $1 + 1; }
	;

/*
 * The C# standard does not have "void" here.  It handles void
 * types elsewhere in the grammar.  However, the grammar is a lot
 * simpler if we make "void" a builtin type and then filter it
 * out later in semantic analysis.
 */
BuiltinType
	: VOID			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_VOID); }
	| BOOL			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_BOOLEAN); }
	| SBYTE			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_I1); }
	| BYTE			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_U1); }
	| SHORT			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_I2); }
	| USHORT		{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_U2); }
	| INT			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_I4); }
	| UINT			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_U4); }
	| LONG			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_I8); }
	| ULONG			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_U8); }
	| CHAR			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_CHAR); }
	| FLOAT			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_R4); }
	| DOUBLE		{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_R8); }
	| LONG_DOUBLE	{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_R); }
	| DECIMAL		{ MakeUnary(SystemType,"Decimal"); }
	| OBJECT		{ MakeUnary(SystemType,"Object"); }
	| STRING		{ MakeUnary(SystemType,"String"); }
	;

/*
 * Expressions.
 */

PrimaryArrayCreationExpression
	: NEW ArrayTypeStart ExpressionList ']' OptArrayInitializer	{
				$$ = ILNode_NewExpression_create($2, $3, 0, $5);
			}
	| NEW ArrayTypeStart ExpressionList ']' RankSpecifiers OptArrayInitializer	{
				ILNode *arrayType;

				arrayType = ArrayTypeCreate($2, &($5));
				$$ = ILNode_NewExpression_create(arrayType, $3, 0, $6);
			}
	| NEW ArrayType ArrayInitializer		{
				$$ = ILNode_NewExpression_create($2, 0, 0, $3);
			}
	;

ElementAccess
	: BASE '[' ExpressionList ']'	{ MakeUnary(BaseElement, $3); }
	| LocalVariableArrayTypeStart ExpressionList ']' {
				MakeBinary(ArrayAccess, $1, $2);
			}
	| PrimaryNonTypeExpression '[' ExpressionList ']' {
				MakeBinary(ArrayAccess, $1, $3);
			}
	;

PrimaryNonTypeExpression
	: PrimarySimpleExpression			{ $$ = $1; }
	| PrimaryArrayCreationExpression	{ $$ = $1; }
	;

PrimarySimpleExpression
	: LiteralExpression				{ $$ = $1; }
	| PrimaryNonTypeExpression '.' PrimaryTypeExpressionPart {
				MakeBinary(MemberAccess, $1, $3);
			}
	| '(' Expression ')'			{ $$ = $2; }
	| SimpleCastExpression			{ $$ = $1; }
	| InvocationExpression			{ $$ = $1; }
	| ElementAccess					{ $$ = $1; }
	| ARGLIST						{ MakeSimple(VarArgList); }
	| THIS							{ MakeSimple(This); }
	| BASE '.' Identifier			{ MakeUnary(BaseAccess, $3); }
	| PostIncrementExpression		{ $$ = $1; }
	| PostDecrementExpression		{ $$ = $1; }
	| ObjectCreationExpression		{ $$ = $1; }
	| TYPEOF '(' Type ')'			{ MakeUnary(TypeOf, $3); }
	| SIZEOF '(' Type ')'			{
				/*
				 * This is only safe if it is used on one of the following
				 * builtin types: sbyte, byte, short, ushort, int, uint,
				 * long, ulong, float, double, char, bool.  We leave the
				 * check to the semantic analysis phase.
				 */
				MakeUnary(SizeOf, $3);
			}
	| CHECKED '(' Expression ')'	{ MakeUnary(Overflow, $3); }
	| UNCHECKED '(' Expression ')'	{ MakeUnary(NoOverflow, $3); }
	| PrimaryExpression PTR_OP Identifier	{
				MakeBinary(DerefField, $1, $3);
			}
	| STACKALLOC ArrayTypeStart Expression ']'	{
				MakeBinary(StackAlloc, $2, $3);
			}
	| BUILTIN_CONSTANT '(' STRING_LITERAL ')'	{
				/*
				 * Get the value of a builtin constant.
				 */
				$$ = CSBuiltinConstant($3.string);
			}
	| MAKEREF '(' Expression ')'			{ MakeUnary(MakeRefAny, $3); }
	| REFTYPE '(' Expression ')'			{ MakeUnary(RefType, $3); }
	| REFVALUE '(' Expression ',' Type ')'	{ MakeBinary(RefValue, $3, $5); }
	| MODULE			{ $$ = ILQualIdentSimple("<Module>"); }
	| DELEGATE AnonymousMethod				{ $$ = $2; }
	| PrimaryMemberAccessStart  DEFAULT			{
				$$ = ILNode_DefaultConstructor_create($1, 0, 0);
			}
	| DefaultValueExpression			{ $$ = $1; }
	;

PrimaryExpression
	: PrimaryNonTypeExpression			{ $$ = $1; }
	| PrimaryTypeExpression				{ $$ = $1; }
	;

LiteralExpression
	: TRUE						{ MakeSimple(True); }
	| FALSE						{ MakeSimple(False); }
	| NULL_TOK					{ MakeSimple(Null); }
	| INTEGER_CONSTANT			{
				switch($1.type)
				{
					case CS_NUMTYPE_INT32:
					{
						$$ = ILNode_Int32_create($1.value, 0, $1.canneg);
					}
					break;

					case CS_NUMTYPE_UINT32:
					{
						$$ = ILNode_UInt32_create($1.value, 0, $1.canneg);
					}
					break;

					case CS_NUMTYPE_INT64:
					{
						$$ = ILNode_Int64_create($1.value, 0, $1.canneg);
					}
					break;

					default:
					{
						$$ = ILNode_UInt64_create($1.value, 0, $1.canneg);
					}
					break;
				}
			}
	| FLOAT_CONSTANT			{
				if($1.type == CS_NUMTYPE_FLOAT32)
				{
					$$ = ILNode_Float32_create($1.value);
				}
				else
				{
					$$ = ILNode_Float64_create($1.value);
				}
			}
	| DECIMAL_CONSTANT			{
				$$ = ILNode_Decimal_create($1);
			}
	| CHAR_CONSTANT				{
				$$ = ILNode_Char_create((ILUInt64)($1), 0, 1);
			}
	| STRING_LITERAL			{
				$$ = ILNode_String_create($1.string, $1.len);
			}
	;

DefaultValueExpression
	: DEFAULT '(' Type ')'				{
				$$ = ILNode_DefaultConstructor_create($3, 0, 0);
			}
	;

InvocationExpression
	: PrimaryExpression '(' OptArgumentList ')'		{ 
				/* Check for "__arglist", which is handled specially */
				if(!yyisa($1, ILNode_VarArgList))
				{
					MakeBinary(InvocationExpression, $1, $3); 
				}
				else
				{
					MakeUnary(VarArgExpand, $3); 
				}
			}
	;

ObjectCreationExpression
	: NEW Type '(' OptArgumentList ')'	{ 
				MakeBinary(ObjectCreationExpression, $2, $4); 
			}
	;

PreIncrementExpression
	: INC_OP PrefixedUnaryExpression	{ MakeUnary(PreInc, $2); }
	;

PreDecrementExpression
	: DEC_OP PrefixedUnaryExpression	{ MakeUnary(PreDec, $2); }
	;

PostIncrementExpression
	: PrimaryExpression INC_OP		{ MakeUnary(PostInc, $1); }
	;

PostDecrementExpression
	: PrimaryExpression DEC_OP		{ MakeUnary(PostDec, $1); }
	;


OptArgumentList
	: /* empty */						{ $$ = 0; }
	| ArgumentList						{ $$ = $1; }
	;

ArgumentList
	: Argument							{ $$ = $1; }
	| ArgumentList ',' Argument			{ MakeBinary(ArgList, $1, $3); }
	;

Argument
	: Expression			{ MakeBinary(Argument, ILParamMod_empty, $1); }
	| OUT Expression		{ MakeBinary(Argument, ILParamMod_out, $2); }
	| REF Expression		{ MakeBinary(Argument, ILParamMod_ref, $2); }
	;

ExpressionList
	: Expression						{ $$ = $1; }
	| ExpressionList ',' Expression		{ MakeBinary(ArgList, $1, $3); }
	;

/*
 * There is a slight ambiguity in the obvious definition of
 * UnaryExpression that creates shift/reduce conflicts when
 * casts are employed.  For example, the parser cannot tell
 * the diference between the following two cases:
 *
 *		(Expr1) - Expr2		-- parse as subtraction.
 *		(Type) -Expr		-- parse as negation and cast.
 *
 * Splitting the definition into two parts fixes the conflict.
 * It is not possible to use one of the operators '-', '+',
 * '*', '&', '++', or '--' after a cast type unless parentheses
 * are involved:
 *
 *		(Type)(-Expr)
 *
 * As a special exception, if the cast involves a builtin type
 * name such as "int", "double", "bool", etc, then the prefix
 * operators can be used.  i.e. the following will be parsed
 * correctly:
 *
 *		(int)-Expr
 *
 * whereas the following requires parentheses because "System"
 * may have been redeclared with a new meaning in a local scope:
 *
 *		(System.Int32)(-Expr)
 *
 * It is very difficult to resolve this in any other way because
 * the compiler does not know if an identifier is a type or not
 * until later.
 */

SimpleCastExpression
	: '(' PrimaryTypeExpression ')'		 { $$ = $2; }
	;

CastExpression
	: '(' BuiltinType ')' PrefixedUnaryExpression	{
				MakeBinary(UserCast, $2, $4);
			}
	| '(' LocalVariablePointerType ')' PrefixedUnaryExpression	{
				MakeBinary(UserCast, $2, $4);
			}
	| '(' LocalVariableArrayType ')' PrefixedUnaryExpression	{
				MakeBinary(UserCast, $2, $4);
			}
	| SimpleCastExpression UnaryExpression	{
				MakeBinary(UserCast, $1, $2);
			}
	;

UnaryExpression
	: UnaryNonTypeExpression			{ $$ = $1; }
	| PrimaryTypeExpression				{ $$ = $1; }
	;

UnaryNonTypeExpression
	: PrimaryNonTypeExpression			{ $$ = $1; }
	| '!' PrefixedUnaryExpression		{ 
				MakeUnary(LogicalNot,ILNode_ToBool_create($2)); 
	}
	| '~' PrefixedUnaryExpression		{ MakeUnary(Not, $2); }
	| CastExpression					{ $$ = $1; }
	;

PrefixedUnaryExpression
	: PrefixedUnaryNonTypeExpression	{ $$ = $1; }
	| PrimaryTypeExpression				{ $$ = $1; }
	;

PrefixedUnaryNonTypeExpression
	: UnaryNonTypeExpression			{ $$ = $1; }
	| '+' PrefixedUnaryExpression	{ MakeUnary(UnaryPlus, $2); } %prec UN_PLUS
	| '-' PrefixedUnaryExpression			{
				/* We create negate nodes carefully so that integer
				   and float constants can be negated in-place */
				if(yyisa($2, ILNode_Integer))
				{
					$$ = NegateInteger((ILNode_Integer *)$2);
				}
				else if(yyisa($2, ILNode_Real))
				{
					((ILNode_Real *)($2))->value =
							-(((ILNode_Real *)($2))->value);
					$$ = $2;
				}
				else if(yyisa($2, ILNode_Decimal))
				{
					ILDecimalNeg(&(((ILNode_Decimal *)($2))->value),
								 &(((ILNode_Decimal *)($2))->value));
					$$ = $2;
				}
				else
				{
					MakeUnary(Neg, $2);
				}
			}  %prec UN_MINUS
	| PreIncrementExpression			{ $$ = $1; }
	| PreDecrementExpression			{ $$ = $1; }
	| '*' PrefixedUnaryExpression		{ MakeBinary(Deref, $2, 0); }
	| '&' PrefixedUnaryExpression		{ MakeUnary(AddressOf, $2); } %prec ADDRESS_OF
	;

MultiplicativeExpression
	: MultiplicativeNonTypeExpression	{ $$ = $1; }
	| PrimaryTypeExpression				{ $$ = $1; }
	;

MultiplicativeNonTypeExpression
	: PrefixedUnaryNonTypeExpression	{ $$ = $1; }
	| MultiplicativeNonTypeExpression '*' PrefixedUnaryExpression	{
				MakeBinary(Mul, $1, $3);
			}
	| PrimaryTypeExpression '*' PrefixedUnaryExpression	{
				/* This one is to pick up the cases where the first part is
				   a PrimaryTypeExpression. */
				MakeBinary(Mul, $1, $3);
			}
	| MultiplicativeExpression '/' PrefixedUnaryExpression	{
				MakeBinary(Div, $1, $3);
			}
	| MultiplicativeExpression '%' PrefixedUnaryExpression	{
				MakeBinary(Rem, $1, $3);
			}
	;

AdditiveExpression
	: AdditiveNonTypeExpression		{ $$ = $1; }
	| PrimaryTypeExpression			{ $$ = $1; }
	;

AdditiveNonTypeExpression
	: MultiplicativeNonTypeExpression	{ $$ = $1; }
	| AdditiveExpression '+' MultiplicativeExpression	{
				MakeBinary(Add, $1, $3);
			}
	| AdditiveExpression '-' MultiplicativeExpression	{
				MakeBinary(Sub, $1, $3);
			}
	;

ShiftExpression
	: ShiftNonTypeExpression		{ $$ = $1; }
	| PrimaryTypeExpression			{ $$ = $1; }
	;

ShiftNonTypeExpression
	: AdditiveNonTypeExpression		{ $$ = $1; }
	| LeftShiftExpression			{ $$ = $1; }
	| RightShiftExpression			{ $$ = $1; }
	;

LeftShiftExpression
	: ShiftExpression LEFT_OP AdditiveExpression	{
				MakeBinary(Shl, $1, $3);
			}
	;

RightShiftExpression
	: RightShiftExpressionStart '>' AdditiveExpression	{
				MakeBinary(Shr, $1, $3);
			} %prec RIGHT_OP
	;

RightShiftExpressionStart
	: PrimaryTypeExpression '>'		{ $$ = $1; }
	| AdditiveNonTypeExpression '>'	{ $$ = $1; }
	| LeftShiftExpression '>'		{ $$ = $1; }
	| RightShiftExpression '>'		{ $$ = $1; }
	;

RightShift
	: '>' '>'
	;

/*
 * Removed the part with the generic references for now.
 * TODO: Readd the generic reference detection in relational
 * expressions. (Klaus)
 *
 * Relational expressions also recognise generic type references.
 * We have to put them here instead of in the more logical place
 * of "PrimaryExpression" to prevent reduce/reduce conflicts.
 *
 * This has some odd consequences.  An expression such as "A + B<C>"
 * will be parsed as "(A + B)<C>" instead of "A + (B<C>)".  To get
 * around this, we insert the generic type parameters into the
 * right-most part of the sub-expression, which should put the
 * parameters back where they belong.  A similar problem happens
 * with method invocations that involve generic method parameters.
 */
RelationalExpression
	: RelationalNonTypeExpression	{ $$ = $1; }
	| PrimaryTypeExpression			{ $$ = $1; }
	;

RelationalNonTypeExpression
	: ShiftNonTypeExpression		{ $$ = $1; }
	| RelationalNonGTExpression		{ $$ = $1; }
	| RelationalGTExpression		{ $$ = $1; }
	;

RelationalNonGTExpression
	: RelationalExpression '<' ShiftExpression		{
				MakeBinary(Lt, $1, $3);
			}
	| RelationalExpression LE_OP ShiftExpression	{
				MakeBinary(Le, $1, $3);
			}
	| RelationalExpression GE_OP ShiftExpression	{
				MakeBinary(Ge, $1, $3);
			}
	| RelationalExpression IS Type					{
				MakeBinary(IsUntyped, $1, $3);
			}
	| RelationalExpression AS Type					{
				MakeBinary(AsUntyped, $1, $3);
			}
	;

RelationalGTExpression
	: RightShiftExpressionStart ShiftExpression	{
				MakeBinary(Gt, $1, $2);
			} %prec GT_OP
	| RelationalGTExpression '>' ShiftExpression	{
				MakeBinary(Gt, $1, $3);
			} %prec GT_OP
	| RelationalNonGTExpression '>' ShiftExpression	{
				MakeBinary(Gt, $1, $3);
			} %prec GT_OP
	; 

EqualityExpression
	: EqualityNonTypeExpression		{ $$ = $1; }
	| PrimaryTypeExpression			{ $$ = $1; }
	;

EqualityNonTypeExpression
	: RelationalNonTypeExpression	{ $$ = $1; }
	| EqualityExpression EQ_OP RelationalExpression	{
				MakeBinary(Eq, $1, $3);
			}
	| EqualityExpression NE_OP RelationalExpression	{
				MakeBinary(Ne, $1, $3);
			}
	;

AndExpression
	: EqualityExpression			{ $$ = $1; }
	| AndExpression '&' EqualityExpression	{
				MakeBinary(And, $1, $3);
			}
	;

XorExpression
	: AndExpression					{ $$ = $1; }
	| XorExpression '^' AndExpression		{
				MakeBinary(Xor, $1, $3);
			}
	;

OrExpression
	: XorExpression					{ $$ = $1; }
	| OrExpression '|' XorExpression		{
				MakeBinary(Or, $1, $3);
			}
	;

LogicalAndExpression
	: OrExpression					{ $$ = $1; }
	| LogicalAndExpression AND_OP OrExpression	{
				MakeBinary(LogicalAnd, $1, $3);
			}
	;

LogicalOrExpression
	: LogicalAndExpression			{ $$ = $1; }
	| LogicalOrExpression OR_OP LogicalAndExpression	{
				MakeBinary(LogicalOr, $1, $3);
			}
	;

ConditionalExpression
	: LogicalOrExpression			{ $$ = $1; }
	| LogicalOrExpression '?' Expression ':' Expression	{
				MakeTernary(Conditional, ILNode_ToBool_create($1), $3, $5);
			}
	;

AssignmentExpression
	: PrefixedUnaryExpression '=' Expression	{
				MakeBinary(Assign, $1, $3);
			}
	| PrefixedUnaryExpression ADD_ASSIGN_OP Expression {
				MakeUnary(AssignAdd, ILNode_Add_create($1, $3));
			}
	| PrefixedUnaryExpression SUB_ASSIGN_OP Expression {
				MakeUnary(AssignSub, ILNode_Sub_create($1, $3));
			}
	| PrefixedUnaryExpression MUL_ASSIGN_OP Expression {
				MakeUnary(AssignMul, ILNode_Mul_create($1, $3));
			}
	| PrefixedUnaryExpression DIV_ASSIGN_OP Expression {
				MakeUnary(AssignDiv, ILNode_Div_create($1, $3));
			}
	| PrefixedUnaryExpression MOD_ASSIGN_OP Expression {
				MakeUnary(AssignRem, ILNode_Rem_create($1, $3));
			}
	| PrefixedUnaryExpression AND_ASSIGN_OP Expression {
				MakeUnary(AssignAnd, ILNode_And_create($1, $3));
			}
	| PrefixedUnaryExpression OR_ASSIGN_OP Expression {
				MakeUnary(AssignOr, ILNode_Or_create($1, $3));
			}
	| PrefixedUnaryExpression XOR_ASSIGN_OP Expression {
				MakeUnary(AssignXor, ILNode_Xor_create($1, $3));
			}
	| PrefixedUnaryExpression LEFT_ASSIGN_OP Expression {
				MakeUnary(AssignShl, ILNode_Shl_create($1, $3));
			}
	| PrefixedUnaryExpression RIGHT_ASSIGN_OP Expression {
				MakeUnary(AssignShr, ILNode_Shr_create($1, $3));
			}
	;

Expression
	: ConditionalExpression		{ $$ = $1; }
	| AssignmentExpression		{ $$ = $1; }
	;

ParenExpression
	: '(' Expression ')'		{ $$ = $2; }
	| '(' error ')'		{
				/*
				 * This production recovers from errors in expressions
				 * that are used with "switch".  Return 0 as the value.
				 */
				MakeTernary(Int32, 0, 0, 1);
				yyerrok;
			}
	;

ConstantExpression
	: Expression		{ MakeUnary(ToConst, $1); }
	;

BooleanExpression
	: Expression		{ MakeUnary(ToBool, $1); }
	;

ParenBooleanExpression
	: '(' BooleanExpression ')'		{ $$ = $2; }
	| '(' error ')'		{
				/*
				 * This production recovers from errors in boolean
				 * expressions that are used with "if", "while", etc.
				 * Default to "false" as the error condition's value.
				 */
				MakeSimple(False);
				yyerrok;
			}
	;

/*
 * Array initialization.
 */

OptArrayInitializer
	: /* empty */			{ $$ = 0; }
	| ArrayInitializer		{ $$ = $1; }
	;

ArrayInitializer
	: '{' OptVariableInitializerList '}' { $$ = $2; }
	| '{' VariableInitializerList ',' '}' { $$ = $2; }
	;

OptVariableInitializerList
	: /* empty */				{ $$ = ILNode_List_create(); }
	| VariableInitializerList	{ $$ = $1; }
	;

VariableInitializerList
	: VariableInitializer {	
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	| VariableInitializerList ',' VariableInitializer {
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
	;

VariableInitializer
	: Expression				{ $$ = $1; }
	| ArrayInitializer			{ MakeUnary(ArrayInit, $1); }
	;

OptComma
	: /* empty */
	| ','
	;

/*
 * Statements.
 */

Statement
	: Identifier ':' LineStatement		{
				/* Convert the identifier into a "GotoLabel" node */
				ILNode *label = ILNode_GotoLabel_create(ILQualIdentName($1, 0));

				/* Build a compound statement */
				$$ = ILNode_Compound_CreateFrom(label, $3);
			}
	| LocalVariableDeclaration ';'	{ $$ = $1; }
	| LocalConstantDeclaration ';'	{ $$ = $1; }
	| InnerEmbeddedStatement		{ $$ = $1; }
	;

EmbeddedStatement
	: InnerEmbeddedStatement		{
			#ifdef YYBISON
				if(debug_flag)
				{
					$$ = ILNode_LineInfo_create($1);
					yysetlinenum($$, @1.first_line);
				}
				else
			#endif
				{
					$$ = $1;
				}
			}
	;

InnerEmbeddedStatement
	: Block							{ $$ = $1; }
	| ';'							{ MakeSimple(Empty); }
	| InnerExpressionStatement ';'	{ $$ = $1; }
	| SelectionStatement			{ $$ = $1; }
	| IterationStatement			{ $$ = $1; }
	| JumpStatement					{ $$ = $1; }
	| TryStatement					{ $$ = $1; }
	| CHECKED Block					{ MakeUnary(Overflow, $2); }
	| UNCHECKED Block				{ MakeUnary(NoOverflow, $2); }
	| LockStatement					{ $$ = $1; }
	| UsingStatement				{ $$ = $1; }
	| FixedStatement				{ $$ = $1; }
	| UNSAFE Block					{ MakeUnary(Unsafe, $2); }
	| YieldStatement				{ $$ = $1; }
	| error ';'		{
				/*
				 * This production recovers from parse errors in statements,
				 * by seaching for the end of the current statement.
				 */
				MakeSimple(Empty);
				yyerrok;
			}
	;

LocalVariableDeclaration
	: LocalVariableType VariableDeclarators		{
				$$ = ILNode_LocalVarDeclaration_create($1, $2);
			}
	;

VariableDeclarators
	: VariableDeclarator							{
				$$ = $1;
			}	
	| VariableDeclarators ',' VariableDeclarator	{
				if(!yyisa($1, ILNode_List))
				{
					$$ = ILNode_List_create();
					ILNode_List_Add($$, $1);
				}
				else
				{
					$$ = $1;
				}
				ILNode_List_Add($$, $3);
			}
	;

VariableDeclarator
	: Identifier							{ $$ = $1; }
	| Identifier '=' VariableInitializer	{
				MakeBinary(VariableDeclarator, $1, $3);
			}
	;

LocalConstantDeclaration
	: CONST Type ConstantDeclarators		{
				$$ = ILNode_LocalConstDeclaration_create($2, $3);
			}
	;

Block
	: '{' OptStatementList '}'		{
				ILNode *temp;
			#ifdef YYBISON
				if(yykind($2) == yykindof(ILNode_Empty) && debug_flag)
				{
					temp = ILNode_LineInfo_create($2);
					yysetlinenum(temp, @1.first_line);
				}
				else
			#endif
				{
					temp = $2;
				}

				/* Wrap the block in a new local variable scope */
				$$ = ILNode_NewScope_create(temp);
				yysetfilename($$, yygetfilename(temp));
				yysetlinenum($$, yygetlinenum(temp));
			}
	| '{' error '}'		{
				/*
				 * This production recovers from parse errors in
				 * a block, by closing off the block on error.
				 */
				MakeSimple(Empty);
				yyerrok;
			}
	;

OptStatementList
	: /* empty */				{ MakeSimple(Empty); }
	| StatementList				{ $$ = $1; }
	;

StatementList
	: LineStatement					{ $$ = $1; }
	| StatementList LineStatement	{ $$ = ILNode_Compound_CreateFrom($1, $2); }
	;

LineStatement
	: Statement		{
			#ifdef YYBISON
				if(debug_flag)
				{
					$$ = ILNode_LineInfo_create($1);
					yysetlinenum($$, @1.first_line);
				}
				else
			#endif
				{
					$$ = $1;
				}
	  		}
	;

ExpressionStatement
	: InnerExpressionStatement		{
			#ifdef YYBISON
				if(debug_flag)
				{
					$$ = ILNode_LineInfo_create($1);
					yysetlinenum($$, @1.first_line);
				}
				else
			#endif
				{
					$$ = $1;
				}
			}
	;

InnerExpressionStatement
	: InvocationExpression				{ $$ = $1; }
	| ObjectCreationExpression			{ $$ = $1; }
	| AssignmentExpression				{ $$ = $1; }
	| PostIncrementExpression			{ $$ = $1; }
	| PostDecrementExpression			{ $$ = $1; }
	| PreIncrementExpression			{ $$ = $1; }
	| PreDecrementExpression			{ $$ = $1; }
	;

SelectionStatement
	: IF ParenBooleanExpression EmbeddedStatement	{
				MakeTernary(If, ILNode_ToBool_create($2), $3,
							ILNode_Empty_create());
			}
	| IF ParenBooleanExpression EmbeddedStatement ELSE EmbeddedStatement	{
				MakeTernary(If, ILNode_ToBool_create($2), $3, $5);
			}
	| SWITCH ParenExpression SwitchBlock	{
				MakeTernary(Switch, $2, $3, 0);
			}
	;

SwitchBlock
	: '{' OptSwitchSections '}'		{ $$ = $2; }
	| '{' error '}'		{
				/*
				 * This production recovers from parse errors in the
				 * body of a switch statement.
				 */
				$$ = 0;
				yyerrok;
			}
	;

OptSwitchSections
	: /* empty */				{ $$ = 0; }
	| SwitchSections			{ $$ = $1; }
	;

SwitchSections
	: SwitchSection					{ 
				$$ = ILNode_SwitchSectList_create();
				ILNode_List_Add($$, $1);
			}
	| SwitchSections SwitchSection	{
				/* Append the new section to the list */
				ILNode_List_Add($1, $2);
				$$ = $1;
			}
	;

SwitchSection
	: SwitchLabels StatementList	{ MakeBinary(SwitchSection, $1, $2); }
	;

SwitchLabels
	: SwitchLabel					{
				/* Create a new label list with one element */
				$$ = ILNode_CaseList_create();
				ILNode_List_Add($$, $1);
			}
	| SwitchLabels SwitchLabel		{
				/* Append the new label to the list */
				ILNode_List_Add($1, $2);
				$$ = $1;
			}
	;

SwitchLabel
	: CASE ConstantExpression ':'	{ MakeUnary(CaseLabel, $2); }
	| DEFAULT_LABEL					{ MakeSimple(DefaultLabel); }
	;

IterationStatement
	: WHILE ParenBooleanExpression EmbeddedStatement	{
				MakeBinary(While, ILNode_ToBool_create($2), $3);
			}
	| DO EmbeddedStatement WHILE ParenBooleanExpression ';'	{
				MakeBinary(Do, $2, ILNode_ToBool_create($4));
			}
	| FOR '(' ForInitializer ForCondition ForIterator EmbeddedStatement	{
				MakeQuaternary(For, $3, ILNode_ToBool_create($4), $5, $6);
				$$ = ILNode_NewScope_create($$);
			}
	| FOREACH '(' Type Identifier IN ForeachExpression EmbeddedStatement	{
				$$ = ILNode_NewScope_create
					(ILNode_Foreach_create($3, ILQualIdentName($4, 0),
										   $4, $6, $7));
			}
	;

ForInitializer
	: ForInitializerInner ';'	{ $$ = $1; }
	| ';'						{ MakeSimple(Empty); }
	| error ';'		{
				/*
				 * This production recovers from errors in the initializer
				 * of a "for" statement.
				 */
				MakeSimple(Empty);
				yyerrok;
			}
	;

ForInitializerInner
	: LocalVariableDeclaration	{ $$ = $1; }
	| ExpressionStatementList	{ $$ = $1; }
	;

ForCondition
	: BooleanExpression ';'		{ $$ = $1; }
	| ';'						{ MakeSimple(True); }
	| error ';'		{
				/*
				 * This production recovers from errors in the condition
				 * of a "for" statement.
				 */
				MakeSimple(False);
				yyerrok;
			}
	;

ForIterator
	: ExpressionStatementList ')'	{ $$ = $1; }
	| ')'							{ MakeSimple(Empty); }
	| error ')'		{
				/*
				 * This production recovers from errors in the interator
				 * of a "for" statement.
				 */
				MakeSimple(Empty);
				yyerrok;
			}
	;

ForeachExpression
	: Expression ')'			{ $$ = $1; }
	| error ')'		{
				/*
				 * This production recovers from errors in the expression
				 * used within a "foreach" statement.
				 */
				MakeSimple(Null);
				yyerrok;
			}
	;

ExpressionStatementList
	: ExpressionStatement		{ $$ = $1; }
	| ExpressionStatementList ',' ExpressionStatement	{
				$$ = ILNode_Compound_CreateFrom($1, $3);
			}
	;

JumpStatement
	: BREAK ';'					{ MakeSimple(Break); }
	| CONTINUE ';'				{ MakeSimple(Continue); }
	| GOTO Identifier ';'		{
				/* Convert the identifier node into a "Goto" node */
				$$ = ILNode_Goto_create(ILQualIdentName($2, 0));
			}
	| GOTO CASE ConstantExpression ';'	{ MakeUnary(GotoCase, $3); }
	| GOTO DEFAULT ';'					{ MakeSimple(GotoDefault); }
	| RETURN ';'						{ MakeSimple(Return); }
	| RETURN Expression ';'				{ MakeUnary(ReturnExpr, $2); }
	| THROW ';'							{ MakeSimple(Throw); }
	| THROW Expression ';'				{ MakeUnary(ThrowExpr, $2); }
	;

TryStatement
	: TRY Block CatchClauses				{ MakeTernary(Try, $2, $3, 0); }
	| TRY Block FinallyClause				{ MakeTernary(Try, $2, 0, $3); }
	| TRY Block CatchClauses FinallyClause	{ MakeTernary(Try, $2, $3, $4); }
	;

CatchClauses
	: SpecificCatchClauses OptGeneralCatchClause	{
				if($2)
				{
					ILNode_List_Add($1, $2);
				}
				$$ = $1;
			}
	| GeneralCatchClause	{
				$$ = ILNode_CatchClauses_create();
				ILNode_List_Add($$, $1);
			}
	;

SpecificCatchClauses
	: SpecificCatchClause		{
				$$ = ILNode_CatchClauses_create();
				ILNode_List_Add($$, $1);
			}
	| SpecificCatchClauses SpecificCatchClause	{
				ILNode_List_Add($1, $2);
				$$ = $1;
			}
	;

SpecificCatchClause
	: CATCH CatchNameInfo Block	{
				$$ = ILNode_CatchClause_create($2.type, $2.id, $2.idNode, $3);
			}
	;

CatchNameInfo
	: '(' Type Identifier ')' {
				$$.type = $2;
				$$.id = ILQualIdentName($3, 0);
				$$.idNode = $3;
			}
	| '(' Type ')'			  {
				$$.type = $2;
				$$.id = 0;
				$$.idNode = 0;
			}
	| '(' error ')'	{
				/*
				 * This production recovers from errors in catch
				 * variable name declarations.
				 */
				$$.type = ILNode_Error_create();
				$$.id = 0;
				$$.idNode = 0;
				yyerrok;
			}
	;

OptGeneralCatchClause
	: /* empty */				{ $$ = 0; }
	| GeneralCatchClause		{ $$ = $1; }
	;

GeneralCatchClause
	: CATCH Block		{
				$$ = ILNode_CatchClause_create(0, 0, 0, $2);
			}
	;

FinallyClause
	: FINALLY Block		{ MakeUnary(FinallyClause, $2); }
	;

LockStatement
	: LOCK ParenExpression EmbeddedStatement	{
				MakeBinary(Lock, $2, $3);
			}
	;

UsingStatement
	: USING ResourceAcquisition EmbeddedStatement	{
				MakeBinary(UsingStatement, $2, $3);
				$$ = ILNode_NewScope_create($$);
			}
	;

ResourceAcquisition
	: '(' LocalVariableType VariableDeclarators ')'	{ 
			MakeBinary(ResourceDeclaration, $2, $3);
		}
	| '(' Expression ')'				{
			$$ = $2;
		}
	| '(' error ')'		{
				/*
				 * This production recovers from errors in resource
				 * acquisition declarations.
				 */
				MakeSimple(Error);
				yyerrok;
			}
	;

/* unsafe code */
FixedStatement
	: FIXED '(' Type FixedPointerDeclarators ')' EmbeddedStatement	{
				MakeTernary(Fixed, $3, $4, $6);
			}
	;

FixedPointerDeclarators
	: FixedPointerDeclarator		{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	| FixedPointerDeclarators ',' FixedPointerDeclarator	{
				$$ = $1;
				ILNode_List_Add($1, $3);
			}
	;

FixedPointerDeclarator
	: Identifier '=' Expression	{
				/*
				 * Note: we have to handle two cases here.  One where
				 * the expression has the form "&expr", and the other
				 * where it doesn't have that form.  We cannot express
				 * these as two different rules, or it creates a
				 * reduce/reduce conflict with "UnaryExpression".
				 */
				if(yykind($3) == yykindof(ILNode_AddressOf))
				{
					MakeBinary(FixAddress, $1,$3);
				}
				else
				{
					MakeBinary(FixExpr, $1, $3);
				}
			}
	;

YieldStatement
	: YIELD RETURN Expression ';'		{
				$$ = ILNode_Empty_create();
				CCError(_("`yield return' is not yet supported"));
			}
	| YIELD BREAK ';'		{
				$$ = ILNode_Empty_create();
				CCError(_("`yield break' is not yet supported"));
			}
	;

/*
 * Attributes.
 */

OptAttributes
	: /* empty */ 		{ $$ = 0; }
	| AttributeSections	{ CSValidateDocs($1); MakeUnary(AttributeTree, $1); }
	;

NonOptAttributes
	: AttributeSections	{ CSValidateDocs($1); MakeUnary(AttributeTree, $1); }
	;

AttributeSections
	: AttributeSection	{
				$$ = ILNode_List_create();
				if($1)
				{
					ILNode_List_Add($$, $1);
				}
			}
	| AttributeSections AttributeSection	{
				$$ = $1;
				if($2)
				{
					ILNode_List_Add($1, $2);
				}
			}
	;

AttributeSection
	: '[' AttributeList OptComma ']'					{
				MakeTernary(AttributeSection, ILAttrTargetType_None, 0, $2);
			}
	| '[' AttributeTarget AttributeList OptComma ']'	{
				MakeTernary(AttributeSection, $2.targetType, $2.target, $3);
			}
	| DOC_COMMENT		{ MakeBinary(DocComment, $1.string, $1.len); }
	| '[' error ']'		{
				/*
				 * This production recovers from errors in attributes.
				 */
				$$ = 0;
				yyerrok;
			}
	;

AttributeTarget
	: QualifiedIdentifier ':'	{
				$$.targetType = ILAttrTargetType_Named;
				$$.target = $1;
			}
	| EVENT ':'					{
				$$.targetType = ILAttrTargetType_Event;
				$$.target = 0;
			}
	| RETURN ':'				{
				$$.targetType = ILAttrTargetType_Return;
				$$.target = 0;
			}
	;

AttributeList
	: Attribute	{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	| AttributeList ',' Attribute	{
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
	;

Attribute
	: QualifiedIdentifier						{ 
				MakeBinary(Attribute, $1, 0);
			}
	| QualifiedIdentifier AttributeArguments	{ 
				MakeBinary(Attribute, $1, $2);
			}
	;

AttributeArguments
	: '(' ')' {	$$=0; /* empty */ }
	| '(' PositionalArgumentList ')'			{
				MakeBinary(AttrArgs, $2, 0);
			}
	| '(' PositionalArgumentList ',' NamedArgumentList ')'	{
				MakeBinary(AttrArgs, $2, $4);
			}
	| '(' NamedArgumentList ')'	{
				MakeBinary(AttrArgs, 0, $2);
			}
	;

PositionalArgumentList
	: PositionalArgument		{
				$$ = ILNode_List_create ();
				ILNode_List_Add ($$, $1);
			}
	| PositionalArgumentList ',' PositionalArgument	{
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
	;

PositionalArgument
	: AttributeArgumentExpression {$$ = $1;}
	;

NamedArgumentList
	: NamedArgument		{
				$$ = ILNode_List_create ();
				ILNode_List_Add($$, $1);
			}
	| NamedArgumentList ',' NamedArgument	{
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
	;

NamedArgument
	: Identifier '=' AttributeArgumentExpression	{
				MakeBinary(NamedArg, $1, $3);
			}
	;

AttributeArgumentExpression
	: Expression			{ $$ = ILNode_ToAttrConst_create($1); }
	;

/*
 * Modifiers.
 */

Modifiers
	: Modifier				{ $$ = $1; }
	| Modifiers Modifier	{
				if(($1 & $2) != 0)
				{
					/* A modifier was used more than once in the list */
					CSModifiersUsedTwice(yycurrfilename(), yycurrlinenum(),
										 ($1 & $2));
				}
				$$ = ($1 | $2);
			}
	;

Modifier
	: NEW			{ $$ = CS_MODIFIER_NEW; }
	| PUBLIC		{ $$ = CS_MODIFIER_PUBLIC; }
	| PROTECTED		{ $$ = CS_MODIFIER_PROTECTED; }
	| INTERNAL		{ $$ = CS_MODIFIER_INTERNAL; }
	| PRIVATE		{ $$ = CS_MODIFIER_PRIVATE; }
	| ABSTRACT		{ $$ = CS_MODIFIER_ABSTRACT; }
	| SEALED		{ $$ = CS_MODIFIER_SEALED; }
	| STATIC		{ $$ = CS_MODIFIER_STATIC; }
	| READONLY		{ $$ = CS_MODIFIER_READONLY; }
	| VIRTUAL		{ $$ = CS_MODIFIER_VIRTUAL; }
	| OVERRIDE		{ $$ = CS_MODIFIER_OVERRIDE; }
	| EXTERN		{ $$ = CS_MODIFIER_EXTERN; }
	| UNSAFE		{ $$ = CS_MODIFIER_UNSAFE; }
	| VOLATILE		{ $$ = CS_MODIFIER_VOLATILE; }
	;

OptAttributesAndModifiers
	: /* empty */ 		{ $$.attributes = 0; $$.modifiers = 0; }
	| AttributesAndModifiers		{
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
			}
	;

AttributesAndModifiers
	: NonOptAttributes		{ $$.attributes = $1; $$.modifiers = 0; }
	| Modifiers				{ $$.attributes = 0; $$.modifiers = $1; }
	| NonOptAttributes Modifiers	{
				$$.attributes = $1;
				$$.modifiers = $2;
			}
	;

OptTypeDeclarationHeader
	: OptAttributesAndModifiers OptPartial	{
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers | $2;
			}
	;

/*
 * Class declarations.
 */
ClassHeader
	: OptTypeDeclarationHeader CLASS Identifier ClassBase	{
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.identifier = $3;
				$$.classBase = $4;
				$$.typeFormals = 0;
			}
	| OptTypeDeclarationHeader CLASS GenericIdentifierStart
			TypeFormals ClassBase OptTypeParameterConstraintsClauses {
#if IL_VERSION_MAJOR > 1
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.identifier = $3;
				$$.classBase = $5;
				$$.typeFormals = $4;
				MergeGenericConstraints($4, $6);
#else	/* IL_VERSION_MAJOR == 1 */
				CCErrorOnLine(yygetfilename($3), yygetlinenum($3),
							  "generics are not supported in this version");
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.identifier = $3;
				$$.classBase = $5;
				$$.typeFormals = 0;
#endif	/* IL_VERSION_MAJOR == 1 */
			}

ClassDeclaration
	: ClassHeader {
				/* Enter a new nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				ClassNamePush($1.identifier, $1.modifiers);
			}
			ClassBody OptSemiColon	{
				ILNode *classBody = ($3).body;
				ILUInt32 modifiers = $1.modifiers;

				/* Exit the current nesting level */
				--NestingLevel;

				/* Determine if we need to add a default constructor */
				if(ClassNameIsCtorDefined())
				{
					modifiers |= CS_MODIFIER_CTOR_DEFINED;
				}

				/* Create the class definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1.attributes,			/* OptAttributes */
							 modifiers | CS_MODIFIER_TYPE_CLASS,
							 ILQualIdentName($1.identifier, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 (ILNode *)$1.typeFormals, /* TypeFormals */
							 $1.classBase,			/* ClassBase */
							 classBody,
							 ($3).staticCtors);
				CloneLine($$, $1.identifier);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

TypeFormals
	: TypeFormalList '>'			{ 
				$$ = (ILNode_GenericTypeParameters *)
						ILNode_GenericTypeParameters_create($1.count, $1.list);
			}
	| error '>'		{
				/*
				 * This production recovers from errors in the typeformals
				 * of a "for" statement.
				 */
				$$ = (ILNode_GenericTypeParameters *)
						ILNode_GenericTypeParameters_create(0, 0);
				yyerrok;
			}
	;

TypeFormalList
	: OptAttributes IDENTIFIER					{
				ILNode *node;
				ILNode_GenericTypeParameter *genPar;
				node = ILNode_GenericTypeParameter_create(0, $2, 0, 0);
				genPar = (ILNode_GenericTypeParameter *)node;
				/* Set the custom attributes attached to the generic parameter */
				genPar->attributes = $1;
				$$.count = 1;
				$$.list = (ILNode_List *)MakeList(0, node);
			}
	| TypeFormalList ',' OptAttributes IDENTIFIER	{
				/* Check for duplicates in the list */
				const char *name = $4;
				ILNode_ListIter iter;
				ILNode *node;
				ILNode_GenericTypeParameter *genPar;
				ILNode_ListIter_Init(&iter, $1.list);
				while((node = ILNode_ListIter_Next(&iter)) != 0)
				{
					genPar = (ILNode_GenericTypeParameter *)node;
					if(!strcmp(genPar->name, name))
					{
						CCErrorOnLine(yygetfilename($1.list), yygetlinenum($1.list),
						  "`%s' declared multiple times in generic parameters",
						  name);
						break;
					}
				}

				/* Add the generic parameter to the list */
				node = ILNode_GenericTypeParameter_create($1.count, name,
														  0, 0);
				/* Set the custom attributes attached to the generic parameter */
				genPar = (ILNode_GenericTypeParameter *)node;
				genPar->attributes = $3;
				$$.list = (ILNode_List *)MakeList((ILNode *)($1.list), node);
				$$.count = $1.count + 1;
			}
	;

OptTypeParameterConstraintsClauses
	: /* EMPTY */						{ $$ = 0; }
	| TypeParameterConstraintsClauses	{ $$ = $1; }
	;

TypeParameterConstraintsClauses
	: TypeParameterConstraintsClause	{ $$ = (ILNode_List *)MakeList(0, $1); }
	| TypeParameterConstraintsClauses TypeParameterConstraintsClause	{
				 		$$ = (ILNode_List *)MakeList((ILNode *)$1, $2);
					}
	;

TypeParameterConstraintsClause
	: WHERE Identifier ':' TypeParameterConstraints	{
						$$ = ILNode_GenericConstraint_create
								(ILQualIdentName($2, 0),
								 $4.constraint,
								 $4.typeConstraints);
					}
	;

TypeParameterConstraints
	: PrimaryConstraint					{
						$$.constraint = $1;
						if($1 == IL_META_GENPARAM_VALUETYPE_CONST)
						{
							$$.constraint |= IL_META_GENPARAM_CTOR_CONST;
						}
						$$.typeConstraints = 0;
					}
	| SecondaryConstraints				{ $$.constraint = 0; $$.typeConstraints = $1; }
	| ConstructorConstraint				{ $$.constraint = $1; $$.typeConstraints = 0; }
	| PrimaryConstraint ',' SecondaryConstraints	{
						$$.constraint = $1;
						if($1 == IL_META_GENPARAM_VALUETYPE_CONST)
						{
							$$.constraint |= IL_META_GENPARAM_CTOR_CONST;
						}
						$$.typeConstraints = $3;
					}
	| PrimaryConstraint ',' ConstructorConstraint	{
						if($1 == IL_META_GENPARAM_VALUETYPE_CONST)
						{
							CCError(_("new() can't be used together with struct because new() is implied by struct"));
						}
						$$.constraint = ($1 | $3);
						$$.typeConstraints = 0;
					}
	| SecondaryConstraints ',' ConstructorConstraint	{
						$$.constraint = $3;
						$$.typeConstraints = $1;
					}
	| PrimaryConstraint ',' SecondaryConstraints ',' ConstructorConstraint	{
						if($1 == IL_META_GENPARAM_VALUETYPE_CONST)
						{
							CCError(_("new() can't be used together with struct because new() is implied by struct"));
						}
						$$.constraint = ($1 | $5);
						$$.typeConstraints = $3;
					}
	;

SecondaryConstraints
	: Type							{
						$$ = (ILNode_List *)MakeList(0,
								 	ILNode_GenericTypeConstraint_create($1));
					}
	| SecondaryConstraints ',' Type {
						$$ = (ILNode_List *)MakeList((ILNode *)$1,
									ILNode_GenericTypeConstraint_create($3));
					}
	;

PrimaryConstraint
	: CLASS							{ $$ = IL_META_GENPARAM_CLASS_CONST; }
	| STRUCT						{ $$ = IL_META_GENPARAM_VALUETYPE_CONST; }
	;

ConstructorConstraint
	: NEW '(' ')'					{ $$ = IL_META_GENPARAM_CTOR_CONST; }
	;

ModuleDeclaration
	: MODULE {
				/* Enter a new nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				$<node>$ = ILQualIdentSimple("<Module>");
				ClassNamePush($<node>$, 0);
			}
			ClassBody OptSemiColon	{
				ILNode *classBody = ($3).body;

				/* Get the default modifiers */
				ILUInt32 modifiers = CS_MODIFIER_PUBLIC;

				/* Exit the current nesting level */
				--NestingLevel;

				/* Create the class definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							(0,						/* OptAttributes */
							 modifiers | CS_MODIFIER_TYPE_MODULE,
							 ILInternString("<Module>", -1).string,
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 0,						/* TypeFormals */
							 0,						/* ClassBase */
							 classBody,
							 ($3).staticCtors);
				CloneLine($$, $<node>2);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

ClassBase
	: /* empty */		{ $$ = 0; }
	| ':' TypeList		{ $$ = $2; }
	;

TypeList
	: Type					{ $$ = $1; }
	| TypeList ',' Type		{
				if(yykind($1) == yykindof(ILNode_ArgList))
				{
					/* Make sure the declaration order is preserved */
					ILNode_ArgList *argList;

					argList = (ILNode_ArgList *)$1;
					while(yykind(argList->expr1) == yykindof(ILNode_ArgList))
					{
						argList = (ILNode_ArgList *)(argList->expr1);
					}
					argList->expr1 = ILNode_ArgList_create($3, argList->expr1);
					$$ = $1;
				}
				else
				{
					$$ = ILNode_ArgList_create($3, $1);
				}
			}
	;

ClassBody
	: '{' OptClassMemberDeclarations '}'	{ $$ = $2; }
	| '{' error '}'		{
				/*
				 * This production recovers from errors in class bodies.
				 */
				yyerrok;
				$$.body = 0;
				$$.staticCtors = 0;
			}
	;

OptClassMemberDeclarations
	: /* empty */					{ $$.body = 0; $$.staticCtors = 0; }
	| ClassMemberDeclarations		{ $$ = $1; }
	;

ClassMemberDeclarations
	: ClassMemberDeclaration		{
				$$.body = MakeList(0, $1.body);
				$$.staticCtors = MakeList(0, $1.staticCtors);
			}
	| ClassMemberDeclarations ClassMemberDeclaration	{
				$$.body = MakeList($1.body, $2.body);
				$$.staticCtors = MakeList($1.staticCtors, $2.staticCtors);
			}
	;

ClassMemberDeclaration
	: ConstantDeclaration		{ $$.body = $1; $$.staticCtors = 0; }
	| FieldDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	| MethodDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	| PropertyDeclaration		{ $$.body = $1; $$.staticCtors = 0; }
	| EventDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	| IndexerDeclaration		{ $$.body = $1; $$.staticCtors = 0; }
	| OperatorDeclaration		{ $$.body = $1; $$.staticCtors = 0; }
	| ConstructorDeclaration	{ $$ = $1; }
	| DestructorDeclaration		{ $$.body = $1; $$.staticCtors = 0; }
	| TypeDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	;

OptPartial
	: /* empty */				{ $$ = 0; }
	| PARTIAL					{
#if IL_VERSION_MAJOR > 1
				$$ = CS_MODIFIER_PARTIAL;
#else /* IL_VERSION_MAJOR == 1 */
				$$ = 0;
				CCError(_("partial types are not supported in this version"));
#endif /* IL_VERSION_MAJOR == 1 */
			}
	;

/*
 * Members
 */
MemberHeaderStart
	: OptAttributesAndModifiers Type	{
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.type = $2;
			}
	;

NonGenericMethodAndPropertyHeaderStart
	: MemberHeaderStart NonGenericQualifiedIdentifier	{
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.type = $1.type;
				$$.identifier = $2;
			}
	;

GenericMethodHeaderStart
	: MemberHeaderStart GenericQualifiedIdentifier		{
				ILNode_GenericTypeParameters *typeFormals;
				/*
				 * We have to convert the TypeActuals for the last part
				 * to TypeFormals here.
				 */
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.type = $1.type;
				typeFormals = TypeActualsToTypeFormals((ILNode *)($2.memberName.typeArgs));
				$$.identifier =	ILNode_GenericQualIdent_create(
									$2.parent,
									$2.memberName.identifier,
									$2.memberName.numTypeArgs,
									(ILNode *)typeFormals);
				$$.typeFormals = typeFormals;
			}
	;

/*
 * Constants.
 */

ConstantDeclaration
	: OptAttributesAndModifiers CONST Type ConstantDeclarators ';' {
				$$ = ILNode_FieldDeclaration_create($1.attributes,
													$1.modifiers | CS_MODIFIER_FIELD_CONST,
													$3, $4);
			}
	;

ConstantDeclarators
	: ConstantDeclarator							{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	| ConstantDeclarators ',' ConstantDeclarator    {
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
	;

ConstantDeclarator
	: Identifier '=' ConstantExpression				{
				MakeBinary(FieldDeclarator, $1, $3);
			}
	;

/*
 * Fields.
 */

FieldDeclaration
	: MemberHeaderStart FieldDeclarators ';'	{
				$$ = ILNode_FieldDeclaration_create($1.attributes,
													$1.modifiers,
													$1.type, $2);
			}
	;

FieldDeclarators
	: FieldDeclarator						{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}	
	| FieldDeclarators ',' FieldDeclarator {
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
		
	;

FieldDeclarator
	: Identifier							{
				MakeBinary(FieldDeclarator, $1, 0);
			}
	| Identifier '=' VariableInitializer	{
				MakeBinary(FieldDeclarator, $1, $3);
			}
	;

/*
 * Methods.
 */
MethodHeader
	: NonGenericMethodAndPropertyHeaderStart
			'(' OptFormalParameterList ')'	{
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.type = $1.type;
				$$.args = (ILNode_List *)$3;
				$$.identifier = $1.identifier;
				$$.typeFormals = 0;
			}
	| GenericMethodHeaderStart
			'(' OptFormalParameterList ')' OptTypeParameterConstraintsClauses {
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.type = $1.type;
				$$.args = (ILNode_List *)$3;
				$$.identifier = $1.identifier;
#if IL_VERSION_MAJOR > 1
				$$.typeFormals = $1.typeFormals;
				MergeGenericConstraints($1.typeFormals, $5);
#else	/* IL_VERSION_MAJOR == 1 */
				$$.typeFormals = 0;
				CCErrorOnLine(yygetfilename($1.identifier), yygetlinenum($1.identifier),
							  "generics are not supported in this version");
#endif	/* IL_VERSION_MAJOR == 1 */
			}

MethodDeclaration
	: MethodHeader MethodBody	{
				if($1.modifiers & CS_MODIFIER_PRIVATE  && yyisa($1.identifier, ILNode_QualIdent))
				{
					// NOTE: clean this up later
					CCErrorOnLine(yygetfilename($1.type), yygetlinenum($1.type),
						"`private' cannot be used in this context");
				}
				$$ = ILNode_MethodDeclaration_create
						($1.attributes,
						 $1.modifiers,
						 $1.type,
						 $1.identifier,
						 (ILNode *)$1.typeFormals,
						 (ILNode *)$1.args,
						 $2);
				CloneLine($$, $1.type);
			}
	;

MethodBody
	: Block			{ $$ = $1; }
	| ';'			{ $$ = 0; }
	;

OptFormalParameterList
	: /* empty */			{ MakeSimple(Empty); }
	| FormalParameterList	{ $$ = $1; }
	;

FormalParameterList
	: FormalParameter							{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	| FormalParameterList ',' FormalParameter	{
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
	;

FormalParameter
	: OptAttributes ParameterModifier Type Identifier		{
				$$ = ILNode_FormalParameter_create($1, $2, $3, $4);
			}
	| ARGLIST	{
				$$ = ILNode_FormalParameter_create(0, ILParamMod_arglist, 0, 0);
			}
	;

ParameterModifier
	: /* empty */	{ $$ = ILParamMod_empty;}
	| REF			{ $$ = ILParamMod_ref;}
	| OUT			{ $$ = ILParamMod_out;}
	| PARAMS		{ $$ = ILParamMod_params;}
	;

/*
 * Properties.
 */

PropertyDeclaration
	: NonGenericMethodAndPropertyHeaderStart
			StartAccessorBlock AccessorBlock	{
				/* Create the property declaration */
				$$ = ILNode_PropertyDeclaration_create($1.attributes,
								   $1.modifiers, $1.type, $1.identifier, 0, 0, 0,
								   (($3.getAccessor.present ? 1 : 0) |
								    ($3.setAccessor.present ? 2 : 0)));
				CloneLine($$, $1.identifier);

				/* Create the property method declarations */
				CreatePropertyMethods((ILNode_PropertyDeclaration *)($$), &($3));
			}
	;

StartAccessorBlock
	: '{'
	;

AccessorBlock
	: AccessorDeclarations '}'	{
				$$ = $1;
			}
	| error '}'		{
				/*
				 * This production recovers from errors in accessor blocks.
				 */
				$$.getAccessor.present = 0;
				$$.getAccessor.modifiers = 0;
				$$.getAccessor.attributes = 0;
				$$.getAccessor.body = 0;
				$$.getAccessor.filename = yycurrfilename();
				$$.getAccessor.linenum = yycurrlinenum();
				$$.setAccessor.present = 0;
				$$.setAccessor.modifiers = 0;
				$$.setAccessor.attributes = 0;
				$$.setAccessor.body = 0;
				$$.setAccessor.filename = yycurrfilename();
				$$.setAccessor.linenum = yycurrlinenum();
				yyerrok;
			}
	;

AccessorDeclarations
	: GetAccessorDeclaration OptSetAccessorDeclaration		{
				$$.getAccessor = $1; 
				$$.setAccessor = $2;
			}
	| SetAccessorDeclaration OptGetAccessorDeclaration		{
				$$.getAccessor = $2; 
				$$.setAccessor = $1;
			}
	;

OptGetAccessorDeclaration
	: /* empty */				{
				$$.present = 0;
				$$.modifiers = 0;
				$$.attributes = 0;
				$$.body = 0;
				$$.filename = yycurrfilename();
				$$.linenum = yycurrlinenum();
			}
	| GetAccessorDeclaration	{ $$ = $1;}
	;

GetAccessorDeclaration
	: OptAttributesAndModifiers GET AccessorBody {
				$$.present = 1;
				$$.modifiers = $1.modifiers;
				$$.attributes = $1.attributes;
				$$.body = $3;
				$$.filename = yycurrfilename();
			#ifdef YYBISON
				$$.linenum = @2.first_line;
			#else
				$$.linenum = yycurrlinenum();
			#endif
			}
	;

OptSetAccessorDeclaration
	: /* empty */				{
				$$.present = 0;
				$$.modifiers = 0;
				$$.attributes = 0;
				$$.body = 0;
				$$.filename = yycurrfilename();
				$$.linenum = yycurrlinenum();
			}
	| SetAccessorDeclaration	{ $$ = $1; }
	;

SetAccessorDeclaration
	: OptAttributesAndModifiers SET AccessorBody {
				$$.present = 1;
				$$.modifiers = $1.modifiers;
				$$.attributes = $1.attributes;
				$$.body = $3;
				$$.filename = yycurrfilename();
			#ifdef YYBISON
				$$.linenum = @2.first_line;
			#else
				$$.linenum = yycurrlinenum();
			#endif
			}
	;

AccessorBody
	: Block				{ $$ = $1; }
	| ';'				{ $$ = 0; }
	;

/*
 * Events.
 */

EventDeclaration
	: EventFieldDeclaration
	| EventPropertyDeclaration
	;

EventFieldDeclaration
	: OptAttributesAndModifiers EVENT Type EventDeclarators ';'	{
				$$ = ILNode_EventDeclaration_create($1.attributes, $1.modifiers, $3, $4);
				CloneLine($$, $3);
			}
	;

EventDeclarators
	: EventDeclarator						{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}	
	| EventDeclarators ',' EventDeclarator {
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
		
	;

EventDeclarator
	: Identifier							{
				ILNode *field;

				field = ILNode_FieldDeclarator_create($1, 0);
				$$ = ILNode_EventDeclarator_create(field, 0, 0);
			}
	| Identifier '=' VariableInitializer	{
				ILNode *field;

				field = ILNode_FieldDeclarator_create($1, $3);
				$$ = ILNode_EventDeclarator_create(field, 0, 0);
			}
	;

EventPropertyDeclaration
	: OptAttributesAndModifiers EVENT Type QualifiedIdentifier
			StartAccessorBlock EventAccessorBlock	{
				ILNode *fieldDeclarator;
				ILNode *eventDeclarator;

				fieldDeclarator = ILNode_FieldDeclarator_create($4, 0);
				eventDeclarator = ILNode_EventDeclarator_create(fieldDeclarator,
																$6.item1,
																$6.item2);
				$$ = ILNode_EventDeclaration_create($1.attributes,
													$1.modifiers, $3,
													eventDeclarator);
			}
	;

EventAccessorBlock
	: EventAccessorDeclarations '}'	{
				$$ = $1;
			}
	| error '}'		{
				/*
				 * This production recovers from errors in accessor blocks.
				 */
				$$.item1 = 0;
				$$.item2 = 0;
				yyerrok;
			}
	;

EventAccessorDeclarations
	: AddAccessorDeclaration RemoveAccessorDeclaration {
				$$.item1 = $1;
				$$.item2 = $2;
			}
	| RemoveAccessorDeclaration AddAccessorDeclaration {
				$$.item1 = $2;
				$$.item2 = $1;
			}
	;

AddAccessorDeclaration
	: OptAttributes ADD AccessorBody {
				$$ = ILNode_MethodDeclaration_create
						($1, 0, 0, 0, 0, 0, $3);
			#ifdef YYBISION
				yysetlinenum($$, @2.first_line);
			#endif
			}
	;

RemoveAccessorDeclaration
	: OptAttributes REMOVE AccessorBody {
				$$ = ILNode_MethodDeclaration_create
						($1, 0, 0, 0, 0, 0, $3);
			#ifdef YYBISION
				yysetlinenum($$, @2.first_line);
			#endif
			}
	;

/*
 * Indexers.
 */

IndexerDeclaration
	: MemberHeaderStart IndexerDeclarator
			StartAccessorBlock AccessorBlock		{
				ILNode* name=GetIndexerName(&CCCodeGen,(ILNode_AttributeTree*)$1.attributes,
							$2.ident);

			#if IL_VERSION_MAJOR > 1
				if(ClassNameGetModifiers() & CS_MODIFIER_STATIC)
				{
					if(!($1.modifiers & CS_MODIFIER_STATIC))
					{
						CCError(_("only static indexers are allowed in static classes"));
					}
					if($1.modifiers & CS_MODIFIER_PROTECTED)
					{
						CCError(_("no protected or protected internal indexers are allowed in static classes"));
					}
				}
			#endif	/* IL_VERSION_MAJOR > 1 */

				$$ = ILNode_PropertyDeclaration_create($1.attributes,
								   $1.modifiers, $1.type, name, $2.params,
								   0, 0,
								   (($4.getAccessor.present ? 1 : 0) |
								    ($4.setAccessor.present ? 2 : 0)));
				CloneLine($$, $2.ident);

				/* Create the property method declarations */
				CreatePropertyMethods((ILNode_PropertyDeclaration *)($$), &($4));
			}
	;

IndexerDeclarator
	: THIS FormalIndexParameters		{
				$$.ident = ILQualIdentSimple(NULL);
				$$.params = $2;
			}
	| QualifiedIdentifierMemberAccessStart THIS FormalIndexParameters	{
				$$.ident = $1;
				$$.params = $3;
			}
	;

FormalIndexParameters
	: '[' FormalIndexParameterList ']'		{ $$ = $2; }
	| '[' error ']'		{
				/*
				 * This production recovers from errors in indexer parameters.
				 */
				$$ = 0;
				yyerrok;
			}
	;

FormalIndexParameterList
	: FormalIndexParameter								{
				$$ = ILNode_List_create ();
				ILNode_List_Add($$, $1);
			}
	| FormalIndexParameterList ',' FormalIndexParameter	{
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
	;

FormalIndexParameter
	: OptAttributes ParameterModifier Type Identifier 					{
				$$ = ILNode_FormalParameter_create($1, $2, $3, $4);
			}
	| ARGLIST	{
				$$ = ILNode_FormalParameter_create(0, ILParamMod_arglist, 0, 0);
			}
	;

/*
 * Operators.
 */
OperatorDeclaration
	: NormalOperatorDeclaration		{ $$ = $1; }
	| ConversionOperatorDeclaration	{ $$ = $1; }
	;

NormalOperatorDeclaration
	: MemberHeaderStart OPERATOR OverloadableOperator
			'(' Type Identifier ')'	Block {
				ILNode *params;

				/* Validate the name of the unary operator */
				if($3.unary == 0)
				{
					CCError("overloadable unary operator expected");
					$3.unary = $3.binary;
				}

				/* Build the formal parameter list */
				params = ILNode_List_create();
				ILNode_List_Add(params,
					ILNode_FormalParameter_create(0, ILParamMod_empty, $5, $6));

				/* Create a method definition for the operator */
				$$ = ILNode_MethodDeclaration_create
						($1.attributes,
						 $1.modifiers | CS_MODIFIER_METHOD_OPERATOR, $1.type,
						 ILQualIdentSimple(ILInternString($3.unary, -1).string),
						 0, params, $8);
				CloneLine($$, $1.type);
			}
	| MemberHeaderStart OPERATOR OverloadableOperator
			'(' Type Identifier ',' Type Identifier ')' Block	{
				ILNode *params;

				/* Validate the name of the binary operator */
				if($3.binary == 0)
				{
					CCError("overloadable binary operator expected");
					$3.binary = $3.unary;
				}

				/* Build the formal parameter list */
				params = ILNode_List_create();
				ILNode_List_Add(params,
					ILNode_FormalParameter_create
						(0, ILParamMod_empty, $5, $6));
				ILNode_List_Add(params,
					ILNode_FormalParameter_create
						(0, ILParamMod_empty, $8, $9));

				/* Create a method definition for the operator */
				$$ = ILNode_MethodDeclaration_create
						($1.attributes,
						 $1.modifiers | CS_MODIFIER_METHOD_OPERATOR, $1.type,
						 ILQualIdentSimple
						 	(ILInternString($3.binary, -1).string),
						 0, params, $11);
				CloneLine($$, $1.type);
			}
	;

OverloadableOperator
	: '+'		{ $$.binary = "op_Addition"; $$.unary = "op_UnaryPlus"; }
	| '-'		{ $$.binary = "op_Subtraction"; $$.unary = "op_UnaryNegation"; }
	| '!'		{ $$.binary = 0; $$.unary = "op_LogicalNot"; }
	| '~'		{ $$.binary = 0; $$.unary = "op_OnesComplement"; }
	| INC_OP	{ $$.binary = 0; $$.unary = "op_Increment"; }
	| DEC_OP	{ $$.binary = 0; $$.unary = "op_Decrement"; }
	| TRUE		{ $$.binary = 0; $$.unary = "op_True"; }
	| FALSE		{ $$.binary = 0; $$.unary = "op_False"; }
	| '*'		{ $$.binary = "op_Multiply"; $$.unary = 0; }
	| '/'		{ $$.binary = "op_Division"; $$.unary = 0; }
	| '%'		{ $$.binary = "op_Modulus"; $$.unary = 0; }
	| '&'		{ $$.binary = "op_BitwiseAnd"; $$.unary = 0; }
	| '|'		{ $$.binary = "op_BitwiseOr"; $$.unary = 0; }
	| '^'		{ $$.binary = "op_ExclusiveOr"; $$.unary = 0; }
	| LEFT_OP	{ $$.binary = "op_LeftShift"; $$.unary = 0; }
	| RightShift{ $$.binary = "op_RightShift"; $$.unary = 0; }
	| EQ_OP		{ $$.binary = "op_Equality"; $$.unary = 0; }
	| NE_OP		{ $$.binary = "op_Inequality"; $$.unary = 0; }
	| '>'		{ $$.binary = "op_GreaterThan"; $$.unary = 0; }
	| '<'		{ $$.binary = "op_LessThan"; $$.unary = 0; }
	| GE_OP		{ $$.binary = "op_GreaterThanOrEqual"; $$.unary = 0; }
	| LE_OP		{ $$.binary = "op_LessThanOrEqual"; $$.unary = 0; }
	;

ConversionOperatorDeclaration
	: OptAttributesAndModifiers IMPLICIT OPERATOR Type
			'(' Type Identifier ')' Block	{
				ILNode *params;

				/* Build the formal parameter list */
				params = ILNode_List_create();
				ILNode_List_Add(params,
					ILNode_FormalParameter_create(0, ILParamMod_empty, $6, $7));

				/* Create a method definition for the operator */
				$$ = ILNode_MethodDeclaration_create
						($1.attributes,
						 $1.modifiers | CS_MODIFIER_METHOD_OPERATOR, $4,
						 ILQualIdentSimple
						 	(ILInternString("op_Implicit", -1).string),
						 0, params, $9);
				CloneLine($$, $4);
			}
	| OptAttributesAndModifiers EXPLICIT OPERATOR Type
			'(' Type Identifier ')' Block	{
				ILNode *params;

				/* Build the formal parameter list */
				params = ILNode_List_create();
				ILNode_List_Add(params,
					ILNode_FormalParameter_create(0, ILParamMod_empty, $6, $7));

				/* Create a method definition for the operator */
				$$ = ILNode_MethodDeclaration_create
						($1.attributes,
						 $1.modifiers | CS_MODIFIER_METHOD_OPERATOR, $4,
						 ILQualIdentSimple
						 	(ILInternString("op_Explicit", -1).string),
						 0, params, $9);
				CloneLine($$, $4);
			}
	;

/*
 * Constructors and destructors.
 */

ConstructorDeclaration
	: OptAttributesAndModifiers Identifier
			'(' OptFormalParameterList ')' ConstructorInitializer MethodBody {
				ILNode *ctorName;
				ILNode *cname;
				ILNode *initializer = $6;
				ILNode *body;

			#if IL_VERSION_MAJOR > 1
				if(ClassNameGetModifiers() & CS_MODIFIER_STATIC)
				{
					if(!($1.modifiers & CS_MODIFIER_STATIC))
					{
						CCError(_("no instance constructors are allowed in static classes"));
					}
				}
			#endif	/* IL_VERSION_MAJOR > 1 */

				if(($1.modifiers & CS_MODIFIER_STATIC) != 0)
				{
					cname = ILQualIdentSimple
								(ILInternString(".cctor", 6).string);
					initializer = 0;
				}
				else
				{
					cname = ILQualIdentSimple
								(ILInternString(".ctor", 5).string);
					ClassNameCtorDefined();
				}
				ctorName = $2;
				if(!ClassNameSame(ctorName))
				{
					CCErrorOnLine(yygetfilename($2), yygetlinenum($2),
						"constructor name does not match class name");
				}
				if($7 && yykind($7) == yykindof(ILNode_NewScope))
				{
					/* Push the initializer into the body scope */
					body = $7;
					((ILNode_NewScope *)body)->stmt =
						ILNode_Compound_CreateFrom
							(initializer, ((ILNode_NewScope *)body)->stmt);
				}
				else if($7 || ($1.modifiers & CS_MODIFIER_EXTERN) == 0)
				{
					/* Non-scoped body: create a new scoped body */
					body = ILNode_NewScope_create
								(ILNode_Compound_CreateFrom(initializer, $7));
					CCWarningOnLine(yygetfilename($2), yygetlinenum($2),
						"constructor without body should be declared 'extern'");
				}
				else
				{
					/* Extern constructor with an empty body */
					body = 0;
				}
				if(($1.modifiers & CS_MODIFIER_STATIC) != 0)
				{
					if(!yyisa($4,ILNode_Empty))
					{
						CCErrorOnLine(yygetfilename($2), yygetlinenum($2),
								"Static constructors cannot have parameters");
					}
					$$.body = 0;
					$$.staticCtors = body;
				}
				else
				{
					$$.body = ILNode_MethodDeclaration_create
						  ($1.attributes, $1.modifiers | CS_MODIFIER_METHOD_CONSTRUCTOR,
						   0 /* "void" */, cname, 0, $4, body);
					CloneLine($$.body, $2);
					$$.staticCtors = 0;
				}
			}
	;

ConstructorInitializer
	: /* empty */							{
				$$ = ILNode_Compound_CreateFrom
						(ILNode_NonStaticInit_create(),
						 ILNode_InvocationExpression_create
							(ILNode_BaseInit_create(), 0));
			}
	| ':' BASE '(' OptArgumentList ')'		{
				$$ = ILNode_Compound_CreateFrom
						(ILNode_NonStaticInit_create(),
						 ILNode_InvocationExpression_create
							(ILNode_BaseInit_create(), $4));
			}
	| ':' THIS '(' OptArgumentList ')'		{
				MakeBinary(InvocationExpression, ILNode_ThisInit_create(), $4);
			}
	;

DestructorDeclaration
	: OptAttributesAndModifiers '~' Identifier '(' ')' Block		{
				ILNode *dtorName;
				ILNode *name;
				ILNode *body;

			#if IL_VERSION_MAJOR > 1
				if(ClassNameGetModifiers() & CS_MODIFIER_STATIC)
				{
					if(!($1.modifiers & CS_MODIFIER_STATIC))
					{
						CCError(_("no destructors are allowed in static classes"));
					}
				}
			#endif	/* IL_VERSION_MAJOR > 1 */

				dtorName = $3;

				/* Validate the destructor name */
				if(!ClassNameSame(dtorName))
				{
					CCErrorOnLine(yygetfilename($3), yygetlinenum($3),
						"destructor name does not match class name");
				}

				/* Build the name of the "Finalize" method */
				name = ILQualIdentSimple(ILInternString("Finalize", -1).string);

				/* Destructors must always call their parent finalizer
				   even if an exception occurs.  We force this to happen
				   by wrapping the method body with a try block whose
				   finally clause always calls its parent */
				/* Note: BaseDestructor filters out these calls for 
						 System.Object class */
				body = ILNode_BaseDestructor_create(
							ILNode_InvocationExpression_create
							(ILNode_BaseAccess_create(name), 0));
				body = ILNode_Try_create
							($6, 0, ILNode_FinallyClause_create(body));

				/* Construct the finalizer declaration */
				$$ = ILNode_MethodDeclaration_create
							($1.attributes,
							 $1.modifiers | CS_MODIFIER_METHOD_DESTRUCTOR,
							 0 /* void */,
							 ILQualIdentSimple
							 	(ILInternString("Finalize", -1).string),
							 0, 0, body);
				CloneLine($$, $3);
			}
	;

/*
 * Structs.
 */
StructHeader
	: OptTypeDeclarationHeader STRUCT Identifier StructInterfaces	{
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.identifier = $3;
				$$.classBase = $4;
				$$.typeFormals = 0;
			}
	| OptTypeDeclarationHeader STRUCT GenericIdentifierStart
			TypeFormals StructInterfaces OptTypeParameterConstraintsClauses {
#if IL_VERSION_MAJOR > 1
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.identifier = $3;
				$$.classBase = $5;
				$$.typeFormals = $4;
				MergeGenericConstraints($4, $6);
#else	/* IL_VERSION_MAJOR == 1 */
				CCErrorOnLine(yygetfilename($3), yygetlinenum($3),
							  "generics are not supported in this version");
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.identifier = $3;
				$$.classBase = $5;
				$$.typeFormals = 0;
#endif	/* IL_VERSION_MAJOR == 1 */
			}

StructDeclaration
	: StructHeader {
				/* Enter a new nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				ClassNamePush($1.identifier, $1.modifiers);
			}
			StructBody OptSemiColon	{
				/* Exit the current nesting level */
				--NestingLevel;

				/* Create the class definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1.attributes,			/* OptAttributes */
							 $1.modifiers | CS_MODIFIER_TYPE_STRUCT,
							 ILQualIdentName($1.identifier, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 (ILNode *)$1.typeFormals, /* TypeFormals */
							 ($1).classBase,		/* ClassBase */
							 ($3).body,				/* StructBody */
							 ($3).staticCtors);		/* StaticCtors */
				CloneLine($$, $1.identifier);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

StructInterfaces
	: /* empty */			{ $$ = 0; }
	| ':' TypeList			{ $$ = $2; }
	;

StructBody
	: '{' OptClassMemberDeclarations '}'	{ $$ = $2; }
	| '{' error '}'		{
				/*
				 * This production recovers from errors in struct declarations.
				 */
				$$.body = 0;
				$$.staticCtors = 0;
				yyerrok;
			}
	;

/*
 * Interfaces.
 */
InterfaceHeader
	: OptTypeDeclarationHeader INTERFACE Identifier InterfaceBase	{
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.identifier = $3;
				$$.classBase = $4;
				$$.typeFormals = 0;
			}
	| OptTypeDeclarationHeader INTERFACE GenericIdentifierStart
			TypeFormals InterfaceBase OptTypeParameterConstraintsClauses {
#if IL_VERSION_MAJOR > 1
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.identifier = $3;
				$$.classBase = $5;
				$$.typeFormals = $4;
				MergeGenericConstraints($4, $6);
#else	/* IL_VERSION_MAJOR == 1 */
				CCErrorOnLine(yygetfilename($3), yygetlinenum($3),
							  "generics are not supported in this version");
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.identifier = $3;
				$$.classBase = $5;
				$$.typeFormals = 0;
#endif	/* IL_VERSION_MAJOR == 1 */
			}

InterfaceDeclaration
	: InterfaceHeader {
				/* Increase the nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				ClassNamePush($1.identifier, $1.modifiers);
			}
			InterfaceBody OptSemiColon	{
				/* Exit from the current nesting level */
				--NestingLevel;

				/* Create the interface definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1.attributes,			/* OptAttributes */
							 $1.modifiers | CS_MODIFIER_TYPE_INTERFACE,
							 ILQualIdentName($1.identifier, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 (ILNode *)$1.typeFormals, /* TypeFormals */
							 $1.classBase,			/* ClassBase */
							 $3,					/* InterfaceBody */
							 0);					/* StaticCtors */
				CloneLine($$, $1.identifier);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

InterfaceBase
	: /* empty */	{ $$ = 0; }
	| ':' TypeList	{ $$ = $2; }
	;

InterfaceBody
	: '{' OptInterfaceMemberDeclarations '}'		{ $$ = $2;}
	| '{' error '}'		{
				/*
				 * This production recovers from errors in interface
				 * declarations.
				 */
				$$ = 0;
				yyerrok;
			}
	;

OptInterfaceMemberDeclarations
	: /* empty */						{ $$ = 0;}
	| InterfaceMemberDeclarations		{ $$ = $1;}
	;

InterfaceMemberDeclarations
	: InterfaceMemberDeclaration		{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	| InterfaceMemberDeclarations InterfaceMemberDeclaration	{
				ILNode_List_Add($1, $2);
				$$ = $1;
			}
	;

InterfaceMemberDeclaration
	: InterfaceMethodDeclaration		{ $$ = $1;}
	| InterfacePropertyDeclaration		{ $$ = $1;}
	| InterfaceEventDeclaration			{ $$ = $1;}
	| InterfaceIndexerDeclaration		{ $$ = $1;}
	;

InterfaceMethodHeader
	: OptAttributes OptNew Type Identifier '(' OptFormalParameterList ')' {
				$$.attributes = $1;
				$$.modifiers = $2;
				$$.type = $3;
				$$.identifier = $4;
				$$.args = (ILNode_List *)$6;
				$$.typeFormals = 0;
			}
	| OptAttributes OptNew Type GenericIdentifierStart TypeFormals
			'(' OptFormalParameterList ')' OptTypeParameterConstraintsClauses {
#if IL_VERSION_MAJOR > 1
				$$.attributes = $1;
				$$.modifiers = $2;
				$$.type = $3;
				$$.identifier = $4;
				$$.args = (ILNode_List *)$7;
				$$.typeFormals = $5;
				MergeGenericConstraints($5, $9);
#else	/* IL_VERSION_MAJOR == 1 */
				CCErrorOnLine(yygetfilename($5), yygetlinenum($5),
							  "generics are not supported in this version");
				$$.attributes = $1;
				$$.modifiers = $2;
				$$.type = $3;
				$$.identifier = $4;
				$$.args = (ILNode_List *)$7;
				$$.typeFormals = 0;
#endif	/* IL_VERSION_MAJOR == 1 */
			}

InterfaceMethodDeclaration
	: InterfaceMethodHeader ';' {
				$$ = ILNode_MethodDeclaration_create
						($1.attributes,
						 $1.modifiers | CS_MODIFIER_METHOD_INTERFACE,
						 $1.type,
						 $1.identifier,
						 (ILNode *)$1.typeFormals,
						 (ILNode *)$1.args,
						 0);
				CloneLine($$, $1.identifier);
			}
	;

OptNew
	: /* empty */	{ $$ = 0; }
	| NEW 			{ $$ = CS_MODIFIER_NEW; }
	;

InterfacePropertyDeclaration
	: OptAttributes OptNew Type Identifier
			StartInterfaceAccessorBody InterfaceAccessorBody	{
				$$ = ILNode_PropertyDeclaration_create
								($1,
								 $2 | CS_MODIFIER_PROPERTY_INTERFACE,
								 $3, $4, 0, 0, 0,
								 (($6.getAccessor.present ? 1 : 0) |
								  ($6.setAccessor.present ? 2 : 0)));
				CloneLine($$, $4);

				/* Create the property method declarations */
				CreatePropertyMethods((ILNode_PropertyDeclaration *)($$), &($6));
			}
	;

StartInterfaceAccessorBody
	: '{'
	;

InterfaceAccessorBody
	: InterfaceAccessors '}'	{
				$$ = $1;
			}
	| error '}'		{
				/*
				 * This production recovers from errors in interface
				 * accessor declarations.
				 */
				$$.getAccessor.present = 0;
				$$.getAccessor.modifiers = 0;
				$$.getAccessor.attributes = 0;
				$$.getAccessor.body = 0;
				$$.getAccessor.filename = yycurrfilename();
				$$.getAccessor.linenum = yycurrlinenum();
				$$.setAccessor.present = 0;
				$$.setAccessor.modifiers = 0;
				$$.setAccessor.attributes = 0;
				$$.setAccessor.body = 0;
				$$.setAccessor.filename = yycurrfilename();
				$$.setAccessor.linenum = yycurrlinenum();
				yyerrok;
			}
	;

InterfaceAccessors
	: OptAttributes GET ';'			{
				$$.getAccessor.present = 1;
				$$.getAccessor.modifiers = 0;
				$$.getAccessor.attributes = $1;
				$$.getAccessor.body = 0;
				$$.getAccessor.filename = yycurrfilename();
				$$.setAccessor.present = 0;
				$$.setAccessor.modifiers = 0;
				$$.setAccessor.attributes = 0;
				$$.setAccessor.body = 0;
				$$.setAccessor.filename = yycurrfilename();
			#ifdef YYBISON
				$$.getAccessor.linenum = @2.first_line;
				$$.setAccessor.linenum = @2.first_line;
			#else
				$$.getAccessor.linenum = yycurrlinenum();
				$$.setAccessor.linenum = yycurrlinenum();
			#endif
			}
	| OptAttributes SET ';'			{
				$$.getAccessor.present = 0;
				$$.getAccessor.modifiers = 0;
				$$.getAccessor.attributes = 0;
				$$.getAccessor.body = 0;
				$$.getAccessor.filename = yycurrfilename();
				$$.setAccessor.present = 1;
				$$.setAccessor.modifiers = 0;
				$$.setAccessor.attributes = $1;
				$$.setAccessor.body = 0;
				$$.setAccessor.filename = yycurrfilename();
			#ifdef YYBISON
				$$.getAccessor.linenum = @2.first_line;
				$$.setAccessor.linenum = @2.first_line;
			#else
				$$.getAccessor.linenum = yycurrlinenum();
				$$.setAccessor.linenum = yycurrlinenum();
			#endif
			}
	| OptAttributes GET ';' OptAttributes SET ';'	{
				$$.getAccessor.present = 1;
				$$.getAccessor.modifiers = 0;
				$$.getAccessor.attributes = $1;
				$$.getAccessor.body = 0;
				$$.getAccessor.filename = yycurrfilename();
				$$.setAccessor.present = 1;
				$$.setAccessor.modifiers = 0;
				$$.setAccessor.attributes = $4;
				$$.setAccessor.body = 0;
				$$.setAccessor.filename = yycurrfilename();
			#ifdef YYBISON
				$$.getAccessor.linenum = @2.first_line;
				$$.setAccessor.linenum = @5.first_line;
			#else
				$$.getAccessor.linenum = yycurrlinenum();
				$$.setAccessor.linenum = yycurrlinenum();
			#endif
			}
	| OptAttributes SET ';' OptAttributes GET ';'	{
				$$.getAccessor.present = 1;
				$$.getAccessor.modifiers = 0;
				$$.getAccessor.attributes = $4;
				$$.getAccessor.body = 0;
				$$.getAccessor.filename = yycurrfilename();
				$$.setAccessor.present = 1;
				$$.setAccessor.modifiers = 0;
				$$.setAccessor.attributes = $1;
				$$.setAccessor.body = 0;
				$$.setAccessor.filename = yycurrfilename();
			#ifdef YYBISON
				$$.getAccessor.linenum = @5.first_line;
				$$.setAccessor.linenum = @2.first_line;
			#else
				$$.getAccessor.linenum = yycurrlinenum();
				$$.setAccessor.linenum = yycurrlinenum();
			#endif
			}
	;

InterfaceEventDeclaration
	: OptAttributes OptNew EVENT Type Identifier ';'		{
				ILUInt32 modifiers;

				modifiers = $2 | CS_MODIFIER_EVENT_INTERFACE;
				$$ = ILNode_EventDeclaration_create
							($1, modifiers, $4,
							 ILNode_EventDeclarator_create
							 	(ILNode_FieldDeclarator_create($5, 0), 0, 0));
			}
	;

InterfaceIndexerDeclaration
	: OptAttributes OptNew Type THIS FormalIndexParameters
			StartInterfaceAccessorBody InterfaceAccessorBody	{
				ILUInt32 modifiers = $2 | CS_MODIFIER_PROPERTY_INTERFACE;
				ILNode* name=GetIndexerName(&CCCodeGen,(ILNode_AttributeTree*)$1,
								ILQualIdentSimple(NULL));
				$$ = ILNode_PropertyDeclaration_create
								($1, modifiers, $3, name, $5, 0, 0,
								 (($7.getAccessor.present ? 1 : 0) |
								  ($7.setAccessor.present ? 2 : 0)));
				CloneLine($$, $3);

				/* Create the property method declarations */
				CreatePropertyMethods((ILNode_PropertyDeclaration *)($$), &($7));
			}
	;

/*
 * Enums.
 */

EnumDeclaration
	: OptAttributesAndModifiers ENUM Identifier EnumBase {
				/* Enter a new nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				ClassNamePush($3, $1.modifiers);
			}
			EnumBody OptSemiColon	{
				ILNode *bodyList;
				ILNode *fieldDecl;

				/* Exit the current nesting level */
				--NestingLevel;

				/* Add an instance field called "value__" to the body,
				   which is used to hold the enumerated value */
				bodyList = $6;
				if(!bodyList)
				{
					bodyList = ILNode_List_create();
				}
				fieldDecl = ILNode_List_create();
				ILNode_List_Add(fieldDecl,
					ILNode_FieldDeclarator_create
						(ILQualIdentSimple("value__"), 0));
				MakeBinary(FieldDeclarator, $1.attributes, 0);
				ILNode_List_Add(bodyList,
					ILNode_FieldDeclaration_create
						(0, CS_MODIFIER_PUBLIC |
							CS_MODIFIER_FIELD_SPECIAL_NAME |
							CS_MODIFIER_FIELD_RT_SPECIAL_NAME, $4, fieldDecl));

				/* Create the class definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1.attributes,			/* OptAttributes */
							 $1.modifiers | CS_MODIFIER_TYPE_ENUM,
							 ILQualIdentName($3, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 0,						/* TypeFormals */
							 0,						/* ClassBase */
							 bodyList,				/* EnumBody */
							 0);					/* StaticCtors */
				CloneLine($$, $3);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

EnumBase
	: /* empty */			{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_I4); }
	| ':' EnumBaseType		{ $$ = $2; }
	;

EnumBaseType
	: BYTE					{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_U1); }
	| SBYTE					{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_I1); }
	| SHORT					{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_I2); }
	| USHORT				{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_U2); }
	| INT					{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_I4); }
	| UINT					{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_U4); }
	| LONG					{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_I8); }
	| ULONG					{ MakeUnary(PrimitiveType, IL_META_ELEMTYPE_U8); }
	;

EnumBody
	: '{' OptEnumMemberDeclarations '}'				{
				$$ = $2;
			}
	| '{' EnumMemberDeclarations ',' '}'				{
				$$ = $2;
			}
	| '{' error '}'		{
				/*
				 * This production recovers from errors in enum declarations.
				 */
				$$ = 0;
				yyerrok;
			}
	;

OptEnumMemberDeclarations
	: /* empty */				{ $$ = 0;}
	| EnumMemberDeclarations	{ $$ = $1;}
	;

EnumMemberDeclarations
	: EnumMemberDeclaration		{
			$$ = ILNode_List_create ();
			ILNode_List_Add($$, $1);
		}
	| EnumMemberDeclarations ',' EnumMemberDeclaration	{
			ILNode_List_Add($1, $3);
			$$ = $1;
		}
	;

EnumMemberDeclaration
	: OptAttributes Identifier		{
			$$ = ILNode_EnumMemberDeclaration_create($1, $2, 0);
		}
	| OptAttributes Identifier '=' ConstantExpression	{
			$$ = ILNode_EnumMemberDeclaration_create($1, $2, $4);
		}
	;

/*
 * Delegates.
 */
DelegateHeader
	: OptAttributesAndModifiers DELEGATE Type Identifier
				'(' OptFormalParameterList ')' {
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.type = $3;
				$$.identifier = $4;
				$$.args = (ILNode_List *)$6;
				$$.typeFormals = 0;
			}
	| OptAttributesAndModifiers DELEGATE Type GenericIdentifierStart TypeFormals
				'(' OptFormalParameterList ')' OptTypeParameterConstraintsClauses {
#if IL_VERSION_MAJOR > 1
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.type = $3;
				$$.identifier = $4;
				$$.args = (ILNode_List *)$7;
				$$.typeFormals = $5;
				MergeGenericConstraints($5, $9);
#else	/* IL_VERSION_MAJOR == 1 */
				CCErrorOnLine(yygetfilename($4), yygetlinenum($4),
							  "generics are not supported in this version");
				$$.attributes = $1.attributes;
				$$.modifiers = $1.modifiers;
				$$.type = $3;
				$$.identifier = $4;
				$$.args = (ILNode_List *)$7;
				$$.typeFormals = 0;
#endif	/* IL_VERSION_MAJOR == 1 */
			}

DelegateDeclaration
	: DelegateHeader ';'	{
				ILNode *bodyList;

				/* Construct the body of the delegate class */
				bodyList = ILNode_List_create();
				ILNode_List_Add(bodyList,
					ILNode_DelegateMemberDeclaration_create($1.type,
															(ILNode *)$1.args));

				/* Create the class definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1.attributes,			/* OptAttributes */
							 $1.modifiers | CS_MODIFIER_TYPE_DELEGATE,
							 ILQualIdentName($1.identifier, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 (ILNode *)$1.typeFormals, /* TypeFormals */
							 0,						/* ClassBase */
							 bodyList,				/* Body */
							 0);					/* StaticCtors */
				CloneLine($$, $1.identifier);

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

/*
 * Anonymous method declarations.
 */

AnonymousMethod
	: Block			{
				$$ = ILNode_Null_create();
				CCError(_("anonymous methods are not yet supported"));
			}
	| '(' OptFormalParameterList ')' Block	{
				$$ = ILNode_Null_create();
				CCError(_("anonymous methods are not yet supported"));
			}
	;
