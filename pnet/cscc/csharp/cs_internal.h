/*
 * cs_internal.h - Internal definitions for the C# compiler front end.
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

#ifndef	_CSCC_CS_INTERNAL_H
#define	_CSCC_CS_INTERNAL_H

#include <il_profile.h>
#include <cscc/csharp/cs_defs.h>
#include <cscc/common/cc_main.h>
#include <cscc/csharp/cs_lookup_member.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Structure to hold up to 4 following array ranks.
 * If the number of ranks exceeds 4 an ILNode_List that holds all ranks
 * for the arrays is created.
 */
struct ArrayRanks
{
	ILUInt32	numRanks;
	ILNode	   *rankList;
	ILUInt32	ranks[4];
};

struct ArrayType
{
	ILNode			   *type;
	struct ArrayRanks	ranks;
};

/*
 * Structure for member accesses.
 */
struct MemberName
{
	const char	   *identifier;
	ILUInt32		numTypeArgs;
	ILNode_List	   *typeArgs;
};

struct MemberAccess
{
	ILNode			   *parent;
	struct MemberName	memberName;
};

/*
 * Structures for accessors.
 */
struct Accessor
{
	int			present;	/* 0 if the acessor is not present 1 otherwise */
	ILUInt32	modifiers;	/* Accessor modifiers */
	ILNode	   *attributes;	/* Attributes for the acessor */
	ILNode	   *body;		/* Body of the accessor */
	char 	   *filename;
	long		linenum;
};

struct PropertyAccessors
{
	struct Accessor getAccessor;
	struct Accessor setAccessor;
};

/*
 * Modifier mask bits.
 */
#define	CS_MODIFIER_PUBLIC			(1<<0)
#define	CS_MODIFIER_PRIVATE			(1<<1)
#define	CS_MODIFIER_PROTECTED		(1<<2)
#define	CS_MODIFIER_INTERNAL		(1<<3)
#define	CS_MODIFIER_ACCESS_MASK		(CS_MODIFIER_PUBLIC | \
									 CS_MODIFIER_PRIVATE | \
									 CS_MODIFIER_PROTECTED | \
									 CS_MODIFIER_INTERNAL)
#define	CS_MODIFIER_NEW				(1<<4)
#define	CS_MODIFIER_ABSTRACT		(1<<5)
#define	CS_MODIFIER_SEALED			(1<<6)
#define	CS_MODIFIER_STATIC			(1<<7)
#define	CS_MODIFIER_READONLY		(1<<8)
#define	CS_MODIFIER_VIRTUAL			(1<<9)
#define	CS_MODIFIER_OVERRIDE		(1<<10)
#define	CS_MODIFIER_EXTERN			(1<<11)
#define	CS_MODIFIER_UNSAFE			(1<<12)
#define	CS_MODIFIER_VOLATILE		(1<<13)
#if IL_VERSION_MAJOR > 1
#define	CS_MODIFIER_PARTIAL			(1<<14)
#endif /* IL_VERSION_MAJOR > 1 */
#define	CS_MODIFIER_MASK			(0x0000FFFF)

/* Type specific modifier flags */
/* Set if an instance  constructor is defined for the class */
#define	CS_MODIFIER_CTOR_DEFINED	0x00010000
#define CS_MODIFIER_TYPE_CLASS		0x00100000
#define CS_MODIFIER_TYPE_STRUCT		0x00200000
#define CS_MODIFIER_TYPE_INTERFACE	0x00300000
#define CS_MODIFIER_TYPE_ENUM		0x00400000
#define CS_MODIFIER_TYPE_DELEGATE	0x00500000
#define CS_MODIFIER_TYPE_MODULE		0x00600000
#define CS_MODIFIER_TYPE_MASK		0x00700000

/* Event specific modifier flags */
/* Set for interface events */
#define	CS_MODIFIER_EVENT_INTERFACE				0x00010000

/*
 * Field specific modifier flags
 */
#define	CS_MODIFIER_FIELD_CONST					0x00010000
#define	CS_MODIFIER_FIELD_SPECIAL_NAME			0x00020000
#define	CS_MODIFIER_FIELD_RT_SPECIAL_NAME		0x00040000

/*
 * Property specific modifier flags
 */
#define	CS_MODIFIER_PROPERTY_INTERFACE			0x00010000

/*
 * Method specific modifier flags
 */
#define	CS_MODIFIER_METHOD_SPECIAL_NAME			0x00010000
#define	CS_MODIFIER_METHOD_RT_SPECIAL_NAME		0x00020000
#define	CS_MODIFIER_METHOD_COMPILER_CONTROLED	0x00040000
#define	CS_MODIFIER_METHOD_HIDE_BY_SIG			0x00080000
#define	CS_MODIFIER_METHOD_NORMAL				0x00000000
#define	CS_MODIFIER_METHOD_CONSTRUCTOR			0x00100000
#define	CS_MODIFIER_METHOD_DESTRUCTOR			0x00200000
#define	CS_MODIFIER_METHOD_OPERATOR				0x00300000
#define	CS_MODIFIER_METHOD_INTERFACE			0x00400000
#define	CS_MODIFIER_METHOD_INTERFACE_ACCESSOR	0x00500000
#define	CS_MODIFIER_METHOD_EVENT_ACCESSOR		0x00600000
#define	CS_MODIFIER_METHOD_PROPERTY_ACCESSOR	0x00700000
#define	CS_MODIFIER_METHOD_TYPE_MASK			0x00700000


/*
 * Special attribute flags.
 */
#define	CS_SPECIALATTR_NEW			0x08000000
#define	CS_SPECIALATTR_UNSAFE		0x04000000
#define	CS_SPECIALATTR_EXTERN		0x02000000
#define	CS_SPECIALATTR_OVERRIDE		0x01000000
#define	CS_SPECIALATTR_VOLATILE		0x00800000
#define	CS_SPECIALATTR_DESTRUCTOR	0x00400000

/*
 * Flag bit that is used to distinguish args from locals.
 */
#define	CS_LOCAL_IS_ARG				0x80000000

/*
 * Type values that are used to classify the size of numeric values.
 */
#define	CS_NUMTYPE_INT32			0
#define	CS_NUMTYPE_UINT32			1
#define	CS_NUMTYPE_INT64			2
#define	CS_NUMTYPE_UINT64			3
#define	CS_NUMTYPE_FLOAT32			4
#define	CS_NUMTYPE_FLOAT64			5

/*
 * A flag that is set to 1 for metadata-only compiles.
 */
extern int CSMetadataOnly;

/*
 * A flag that is set to 1 to disable generics support.
 */
extern int CSNoGenerics;

/*
 * A flag that is set to 1 to use latin1 encoding
 */
extern int CSLatin1Charset;

/*
 * A flag that is set to use VB-style "hidebysig" processing on methods.
 */
extern int CSNoHideBySig;

/*
 * Determine if a type or parameter node contains unsafe types.
 */
int CSHasUnsafeType(ILNode *node);

/*
 * Report that some modifiers have been specified more than once.
 */
void CSModifiersUsedTwice(char *filename, long linenum, ILUInt32 modifiers);

/*
 * Convert modifiers into attribute flag masks for program elements.
 */
ILUInt32 CSModifiersToTypeAttrs(ILNode *node, ILUInt32 modifiers, int isNested);
ILUInt32 CSModifiersToDelegateAttrs(ILNode *node, ILUInt32 modifiers,
									int isNested);
ILUInt32 CSModifiersToConstAttrs(ILNode *node, ILUInt32 modifiers);
ILUInt32 CSModifiersToFieldAttrs(ILNode *node, ILUInt32 modifiers);
ILUInt32 CSModifiersToMethodAttrs(ILNode *node, ILUInt32 modifiers);
ILUInt32 CSModifiersToEventAttrs(ILNode *node, ILUInt32 modifiers);
ILUInt32 CSModifiersToPropertyAttrs(ILNode *node, ILUInt32 modifiers);
ILUInt32 CSModifiersToOperatorAttrs(ILNode *node, ILUInt32 modifiers);
ILUInt32 CSModifiersToConstructorAttrs(ILNode *node, ILUInt32 modifiers);
ILUInt32 CSModifiersToDestructorAttrs(ILNode *node, ILUInt32 modifiers);

/*
 * Convert a built-in constant name into a constant node.
 */
ILNode *CSBuiltinConstant(const char *name);

/*
 * Gather information about all types in the program.
 * Returns a new top-level list for the program with
 * the classes re-organised so that parent classes and
 * interfaces precede classes that inherit them.
 */
ILNode *CSTypeGather(ILGenInfo *info, ILScope *globalScope, ILNode *tree);

/*
 * Validate a block of documentation comments to ensure
 * that all XML tags are properly balanced.
 */
void CSValidateDocs(ILNode *docList);

/*
 * Determine if two method signatures have identical parameters.
 * Ignore the return type and the static vs instance property.
 */
int CSSignatureIdentical(ILType *sig1, ILType *sig2);

/*
 * Determine if "info1" is a base type for "info2".
 */
int CSIsBaseTypeFor(ILClass *info1, ILClass *info2);

/*
 * Get the scope to use for access checks in the current context.
 */
ILClass *CSGetAccessScope(ILGenInfo *genInfo, int defIsModule);

/*
 * Resolve a simple name to a semantic value.
 */
CSSemValue CSResolveSimpleName(ILGenInfo *genInfo, ILNode *node,
							   const char *name, int literalType);

/* 
 * Resolve a simple name to a semantic value ignoring errors if any.
 */
CSSemValue CSResolveSimpleNameQuiet(ILGenInfo *genInfo, ILNode *node,
										const char *name, int literalType);
/*
 * Resolve a namespace member name
 */
CSSemValue CSResolveNamespaceMemberName(ILGenInfo *genInfo,
		        ILNode *node, CSSemValue value, const char *name);

/*
 * Resolve a member name to a semantic value. 
 */
CSSemValue CSResolveMemberName(ILGenInfo *genInfo, ILNode *node,
							   CSSemValue value, const char *name,
							   int literalType);
CSSemValue CSResolveMemberNameQuiet(ILGenInfo *genInfo, ILNode *node,
							   CSSemValue value, const char *name,
							   int literalType);

/*
 * Resolve an instance constructor reference to a semantic value.
 */
CSSemValue CSResolveConstructor(ILGenInfo *genInfo, ILNode *node,
								ILType *objectType);

/*
 * Resolve an indexer reference to a semantic value.  The return
 * value is an indexer group, or "void".
 */
CSSemValue CSResolveIndexers(ILGenInfo *genInfo, ILNode *node,
							 ILClass *classInfo, int baseAccess);

/*
 * Create a method group that contains a single method.
 */
void *CSCreateMethodGroup(ILMethod *method);

/*
 * Get the n'th member from a method or indexer group.
 * Returns NULL at the end of the group.
 */
ILProgramItem *CSGetGroupMember(void *group, unsigned long n);

/*
 * Remove the n'th member from a method group.
 * Returns the new group.
 */
void *CSRemoveGroupMember(void *group, unsigned long n);

/*
 * Set the candidate form for the n'th member of a method group.
 */
void CSSetGroupMemberForm(void *group, unsigned long n, int form);

/*
 * Get the candidate form for the n'th member of a method group.
 */
int CSGetGroupMemberForm(void *group, unsigned long n);

/*
 * Add a statement to the static constructor for the current class.
 */
void CSAddStaticCtor(ILGenInfo *info, ILNode *stmt);

/*
 * Add a statement to the initializer constructor for the current class.
 */
void CSAddInitCtor(ILGenInfo *info, ILNode *stmt);

/*
 * Process the attributes on a program item.
 */
void CSProcessAttrs(ILGenInfo *info, ILProgramItem *item,
					ILNode *attributes, int target);

/*
 * Process the attributes on a type definition.
 */
void CSProcessAttrsForClass(ILGenInfo *info, ILNode_ClassDefn *defn,
							int mainTarget);

/*
 * Process the attributes on a method parameter.
 */
void CSProcessAttrsForParam(ILGenInfo *info, ILMethod *method,
							unsigned long paramNum,
							ILNode *attributes);

/*
 * Add the "DefaultMember" attribute to a class.
 */
void CSAddDefaultMemberAttr(ILGenInfo *info, ILClass *classInfo,
							const char *name);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSCC_CS_INTERNAL_H */
