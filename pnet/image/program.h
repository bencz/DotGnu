/*
 * program.h - Internal definitions related to program information.
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

#ifndef	_IMAGE_PROGRAM_H
#define	_IMAGE_PROGRAM_H

#include "image.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#define METHOD_VTABLE_ITEM_COUNT	4

/*
 * Common fields for program items.
 */
struct _tagILProgramItem
{
	ILImage		   *image;				/* Image we are attached to */
	ILUInt32		token : 31;			/* Assigned token code */
	ILUInt32		linked : 1;			/* Non-zero if linked */
	void		   *attrsOrLink;		/* Attributes or link information */

};

/*
 * Information that is attached to a program item when
 * it is linked to another item.
 */
typedef struct _tagILProgramItemLink ILProgramItemLink;
struct _tagILProgramItemLink
{
	ILAttribute		   *customAttrs;	/* New location of custom attributes */
	ILProgramItem	   *linkedItem;		/* Item we are linked to */
	ILProgramItemLink  *next;			/* Items that link to us */

};

/*
 * Create a link from "item1" to "item2".  Returns zero if out of memory.
 */
int _ILProgramItemLink(ILProgramItem *item1, ILProgramItem *item2);

/*
 * Remove all links to and from "item".
 */
void _ILProgramItemUnlink(ILProgramItem *item);

/*
 * Determine what a program item is linked to.  NULL if no link.
 */
ILProgramItem *_ILProgramItemLinkedTo(ILProgramItem *item);

/*
 * Determine what links to a particular program item from
 * a foreign image.  NULL if no link.
 */
ILProgramItem *_ILProgramItemLinkedBackTo(ILProgramItem *item, ILImage *image);

/*
 * Resolve a program item reference by following links
 * to find the actual item.
 */
ILProgramItem *_ILProgramItemResolve(ILProgramItem *item);

/*
 * Resolve a program item reference by following links to
 * find the actual item, but don't cross image boundaries.
 */
ILProgramItem *_ILProgramItemResolveRef(ILProgramItem *item);

/*
 * Load the attributes for a program item on demand.
 */
void _ILProgramItemLoadAttributes(ILProgramItem *item);

/*
 * Information about a custom attribute.
 */
struct _tagILAttribute
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILProgramItem  *owner;				/* Owner of the custom attribute */
	ILProgramItem  *type;				/* Type for the attribute */
	ILUInt32		value;				/* Offset of the attribute value */
	ILAttribute	   *next;				/* Next attribute on the list */

};

/*
 * Set the value of an attribute to an explicit blob index.
 */
void _ILAttributeSetValueIndex(ILAttribute *attr, ILUInt32 index);

/*
 * Edit & Continue information for a module.
 */
typedef struct _tagILModuleEnc ILModuleEnc;
struct _tagILModuleEnc
{
	ILUInt16		generation;			/* Edit & Continue generation */
	ILUInt16		flags;				/* Flags for GUID fields */
	unsigned char	encId[16];			/* Edit & Continue identifier */
	unsigned char	encBaseId[16];		/* Edit & Continue base identifier */
};

/*
 * Information about a module.
 */
struct _tagILModule
{
	ILProgramItem	programItem;		/* Parent class fields */
	const char	   *name;				/* Name of the module */
	unsigned char  *mvid;				/* GUID for the module */
	ILModuleEnc    *enc;				/* Edit & Continue information */

};

/*
 * Information about an assembly.
 */
struct _tagILAssembly
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILUInt32		hashAlgorithm;		/* Hash algorithm identifier */
	ILUInt16		version[4];			/* Version information */
	ILUInt16		attributes;			/* Attributes for the assembly */
	ILUInt16		refAttributes;		/* Attributes when used as a ref */
	ILUInt32		originator;			/* Blob offset of the originator key */
	const char     *name;				/* Name of the assembly */
	const char     *locale;				/* Locale used by the assembly */
	ILUInt32		hashValue;			/* Blob offset of the hash value */

};

/*
 * Set the value of an originator key to an explicit blob index.
 */
void _ILAssemblySetOrigIndex(ILAssembly *assem, ILUInt32 index);

/*
 * Set the value of a hash value to an explicit blob index.
 */
void _ILAssemblySetHashIndex(ILAssembly *assem, ILUInt32 index);

/*
 * Extension information that is only present in some circumstances
 * (e.g. Java classes).
 */

#define _IL_EXT_JAVA_CONSTPOOL		1
#define _IL_EXT_CIL_CUSTOMATTR		2

typedef struct _tagJavaConstEntry JavaConstEntry;
typedef struct _tagJavaConstPool JavaConstPool;
struct _tagJavaConstPool
{
	ILUInt32		size;				/* Java constant pool size */
	JavaConstEntry *entries;			/* Constant pool entries */
};

struct _tagILAttributeUsageAttribute
{
	ILUInt32	validOn;
	ILBool		inherited;
	ILBool		allowMultiple;
};

typedef struct _tagILCustomAttribute ILCustomAttribute;
struct _tagILCustomAttribute
{
	ILClass		   *attributeClass;
	union
	{
		struct _tagILAttributeUsageAttribute attributeUsage;
	} un;
};

typedef struct _tagILClassExt ILClassExt;
struct _tagILClassExt
{
	ILUInt32		kind;				/* Kind of the class extension */
	ILClassExt	   *next;				/* Next entry in the linked list */
	union
	{
		JavaConstPool		javaConstPool;
		ILCustomAttribute	customAttribute;
	} un;
};

/*
 * Allocate a class extension block and attach it to the class given.
 * Returns 0 if out of memory.
 */
ILClassExt *_ILClassExtCreate(ILClass *info, ILUInt32 kind);

/*
 * Get the first class extension block of the given kind attached to the
 * class.
 * Returns 0 if there is no class extenstion block of the given kind is
 * attached to the class.
 */
ILClassExt *_ILClassExtFind(ILClass *info, ILUInt32 kind);

/* 
 * list of java method code 
 */
typedef struct _tagILJavaCodeList JavaCodeList;
struct _tagILJavaCodeList
{
	ILMethod       *method;
	void           *code;
	ILUInt32        length;
	JavaCodeList   *next;
};

/*
 * Structure of a constant pool entry.
 */
struct _tagJavaConstEntry
{
	ILUInt16		type;
	ILUInt16		length;
	union
	{
		JavaCodeList   *codeList;
		char	   *utf8String;
		ILInt32		intValue;
		ILInt64		longValue;
		ILFloat		floatValue;
		ILDouble	doubleValue;
		ILUInt32	strValue;
		struct
		{
			ILUInt32 nameIndex;
			ILClass *classInfo;

		} classValue;
		struct
		{
			ILUInt16 classIndex;
			ILUInt16 nameAndType;
			ILProgramItem *item;

		} refValue;
		struct
		{
			ILUInt32 name;
			ILUInt32 type;

		} nameAndType;

	} un;

};

/*
 * Information about a class name.
 */
struct _tagILClassName
{
	ILImage		   *image;			/* Image containing the class */
	ILToken			token;			/* Token code for the class */
	const char	   *name;			/* Name of the class */
	const char	   *namespace;		/* Name of the class's namespace */
	ILProgramItem  *scope;			/* Scope that the class is defined in */
	ILClassName	   *scopeName;		/* Name of the scope (nested classes) */
};

/*
 * Create a new class name.
 */
ILClassName *_ILClassNameCreate(ILImage *image, ILToken token,
								const char *name, const char *namespace,
								ILProgramItem *scopeItem,
								ILClassName *scopeName);

/*
 * Update class name token information for a class.
 */
void _ILClassNameUpdateToken(ILClass *info);

/*
 * Convert a class name into the actual class, performing dynamic
 * token lookup as necessary.
 */
ILClass *_ILClassNameToClass(ILClassName *className);

/*
 * Look up a class name within a specific scope.
 */
ILClassName *_ILClassNameLookup(ILImage *image, ILProgramItem *scopeItem,
								ILClassName *scopeName, const char *name,
								const char *namespace);

/*
 * Information about a class.
 */
struct _tagILClass
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILUInt32		attributes;			/* IL_META_TYPEDEF_xxx flags */
	ILClassName    *className;			/* Name information for the class */
	ILProgramItem  *parent;				/* Parent item (TypeSpec or Class) */
	ILImplements   *implements;			/* List of implemented interfaces */
	ILMember	   *firstMember;		/* First member owned by the class */
	ILMember	   *lastMember;			/* Last member owned by the class */
	ILNestedInfo   *nestedChildren;		/* List of nested children */
	ILType         *synthetic;			/* Synthetic type for this class */
	ILClassExt     *ext;				/* Extension information */
	void           *userData;			/* Data added by the runtime engine */
#if IL_VERSION_MAJOR > 1
	ILInt32			numGenericPars;		/* Number of generic parameters */
#endif

};

/*
 * Extra class attributes that are used internally.
 */
#define	IL_META_TYPEDEF_REFERENCE		0x80000000	/* Not yet really defined */
#define	IL_META_TYPEDEF_COMPLETE		0x40000000	/* Definition is complete */
#define	IL_META_TYPEDEF_GENERIC_PARS	0x20000000	/* Has generic parameters */
#define	IL_META_TYPEDEF_CLASS_EXPANDED	0x10000000	/* Generics expanded */
#define	IL_META_TYPEDEF_CCTOR_RUNNING	0x08000000	/* .cctor is currenty executed. */
#define	IL_META_TYPEDEF_CCTOR_ONCE		0x04000000	/* .cctor already done */
#define	IL_META_TYPEDEF_CCTOR_MASK		0x0C000000	/* .cctor flags */
#define	IL_META_TYPEDEF_SYSTEM_MASK		0xFC000000	/* System flags */

/*
 * Information about an "implements" clause for a class.
 */
struct _tagILImplements
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILClass		   *implement;			/* Implementing class */
	ILProgramItem  *interface;			/* Implemented interface */
	ILImplements   *nextInterface;		/* Next implemented interface */

};

/*
 * Information about nested classes.
 */
struct _tagILNestedInfo
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILClass		   *parent;				/* Parent in the nesting relationship */
	ILClass		   *child;				/* Child in the nesting relationship */
	ILNestedInfo   *next;				/* Next nesting information block */

};

/*
 * Information about a member.  This is shared by methods,
 * fields, events, properties, and anything else that may
 * eventually be listed as a class member.
 */
struct _tagILMember
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILClass		   *owner;				/* Owning class */
	ILMember	   *nextMember;			/* Next member owned by the class */
	ILUInt16		kind;				/* Kind of member */
	ILUInt16		attributes;			/* Attributes for the member */
	const char     *name;				/* Name of the member */
	ILType		   *signature;			/* Signature for the member */
	ILUInt32		signatureBlob;		/* Blob index for the signature */

};

/*
 * Set the blob index for a member's signature.
 * This should only be used when loading images.
 */
void _ILMemberSetSignatureIndex(ILMember *member, ILUInt32 index);

/*
 * Information about a generic method vtable item.
 */
typedef struct _tagILMethodVTableItem ILMethodVTableItem;
struct _tagILMethodVTableItem
{
       ILMethod *data[METHOD_VTABLE_ITEM_COUNT];
	   ILMethodVTableItem *nextItem;
};

/*
 * Information about a generic method vtable.
 */
typedef struct _tagILMethodVTable ILMethodVTable;
struct _tagILMethodVTable
{
	ILMethod			*virtualAncestor;	/* The virtual ancestor of the method. */
	ILMethodVTableItem	firstItem;
	ILMethodVTableItem	*lastItem;
	int					numSlots;
	int					numItems;
};

/*
 * Information about a method.
 */
struct _tagILMethod
{
	ILMember		member;				/* Common member fields */
	ILUInt16		implementAttrs;		/* Implementation attrs for method */
	ILUInt16		callingConventions;	/* Calling conventions for method */
	ILUInt32		rva;				/* Address of the method's code */
	ILParameter    *parameters;			/* Parameter definitions */
	ILMethodVTable *vtable;				/* VTable for generic methods. */
	void           *userData;			/* User data for the runtime engine */
	ILUInt32		index;				/* Data added by the runtime engine */
	ILUInt32		count;				/* Profile count for the engine */
#ifdef ENHANCED_PROFILER
	ILUInt64		time;				/* Profile time counter for the engine */
#endif
};

/*
 * Information about a generic method instance.
 */
struct _tagILMethodInstance
{
	ILMethod		method;					/* Method fields */
	ILMethod		*genMethod;				/* Corresponding generic Method definition */
	ILType			*classTypeArguments;	/* Class type arguments.  */
	ILType			*methodTypeArguments;	/* Method type arguments. */
};

/*
 * Load the parameter definitions for a method token on demand.
 */
void _ILMethodLoadParams(ILMethod *method);

/*
 * Information about a parameter.
 */
struct _tagILParameter
{
	ILProgramItem	programItem;		/* Parent class fields */
	const char	   *name;				/* Name of the parameter */
	ILUInt16		attributes;			/* Attributes for the parameter */
	ILUInt16		paramNum;			/* Parameter number */
	ILParameter	   *next;				/* Next parameter */

};

/*
 * Information about a field.
 */
struct _tagILField
{
	ILMember		member;				/* Common member fields */
	ILUInt32		offset;				/* Data added by the runtime engine */
	ILUInt32		nativeOffset;		/* Data added by the runtime engine */

};

/*
 * Information about an event.
 */
struct _tagILEvent
{
	ILMember		member;				/* Common member fields */
	ILMethodSem    *semantics;			/* List of semantic methods */

};

/*
 * Information about a property.
 */
struct _tagILProperty
{
	ILMember		member;				/* Common member fields */
	ILMethodSem    *semantics;			/* List of semantic methods */
	ILMethod	   *getter;				/* Cached copy of the getter */
	ILMethod	   *setter;				/* Cached copy of the setter */

};

/*
 * Information about a PInvoke member, which appears
 * after the method member to which is applies.
 */
struct _tagILPInvoke
{
	ILMember		member;				/* Common member fields */
	ILMember	   *memberInfo;			/* Member PInvoke applies to */
	ILModule	   *module;				/* Module function is imported from */
	const char	   *aliasName;			/* Alias for the function */

};

/*
 * Information about an override member.
 */
struct _tagILOverride
{
	ILMember		member;				/* Common member fields */
	ILMethod	   *decl;				/* Declaration for override */
	ILMethod	   *body;				/* Body for override */

};

/*
 * Information about a reference member.
 */
struct _tagILMemberRef
{
	ILMember		member;				/* Common member fields */
	ILMember       *ref;				/* Member that is being referenced */

};

/*
 * Information about an EventMap token.
 */
struct _tagILEventMap
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILClass		   *classInfo;			/* Class that owns the events */
	ILEvent		   *firstEvent;			/* First event in the list */

};

/*
 * Information about a PropertyMap token.
 */
struct _tagILPropertyMap
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILClass		   *classInfo;			/* Class that owns the properties */
	ILProperty	   *firstProperty;		/* First property in the list */

};

/*
 * Information about a MethodSemantics token.
 */
struct _tagILMethodSem
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILProgramItem  *owner;				/* Event or property that owns this */
	ILUInt32		type;				/* Type of semantics declaration */
	ILMethod	   *method;				/* Method that implements semantics */
	ILMethodSem    *next;				/* Next semantics declaration */

};

/*
 * Information about an OS information block.
 */
struct _tagILOSInfo
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILUInt32		identifier;			/* OS identifier */
	ILUInt32		major;				/* OS major version */
	ILUInt32		minor;				/* OS minor version */
	ILAssembly     *assembly;			/* Assembly that owns this block */

};

/*
 * Information about a processor information block.
 */
struct _tagILProcessorInfo
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILUInt32		number;				/* Processor number */
	ILAssembly     *assembly;			/* Assembly that owns this block */

};

/*
 * Information about a TypeSpec token.
 */
struct _tagILTypeSpec
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILType		   *type;				/* Type associated with the TypeSpec */
	ILUInt32		typeBlob;			/* Index into blob heap of the type */
	ILClass		   *classInfo;			/* Class block for the TypeSpec */
	ILClass		   *refInfo;			/* Class referenced by the TypeSpec */

};

/*
 * Set the type blob index for a TypeSpec token.
 * This should only be used when loading images.
 */
void _ILTypeSpecSetTypeIndex(ILTypeSpec *spec, ILUInt32 index);

/*
 * Information about a stand alone signature token.
 */
struct _tagILStandAloneSig
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILType		   *type;				/* Type associated with the signature */
	ILUInt32		typeBlob;			/* Index into blob heap of the type */

};

/*
 * Set the type blob index for a stand alone signature token.
 * This should only be used when loading images.
 */
void _ILStandAloneSigSetTypeIndex(ILStandAloneSig *sig, ILUInt32 index);

/*
 * Information about an item that is "owned" by another token.
 * Used for constants, field marshaling, security, etc.
 */
typedef struct _tagILOwnedItem ILOwnedItem;
struct _tagILOwnedItem
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILProgramItem  *owner;				/* Owner of the constant */
};

/*
 * Information about a constant token.
 */
struct _tagILConstant
{
	ILOwnedItem		ownedItem;			/* Parent class fields */
	ILUInt32		elemType;			/* Element type for the constant */
	ILUInt32		value;				/* Blob offset of the value */

};

/*
 * Set the value of an constant to an explicit blob index.
 */
void _ILConstantSetValueIndex(ILConstant *constant, ILUInt32 index);

/*
 * Information about a field RVA token.
 */
struct _tagILFieldRVA
{
	ILOwnedItem		ownedItem;			/* Parent class fields */
	ILUInt32		rva;				/* RVA value for the field */

};

/*
 * Information about a field layout token.
 */
struct _tagILFieldLayout
{
	ILOwnedItem		ownedItem;			/* Parent class fields */
	ILUInt32		offset;				/* Offset value for the field */

};

/*
 * Information about a field marshal token.
 */
struct _tagILFieldMarshal
{
	ILOwnedItem		ownedItem;			/* Parent class fields */
	ILUInt32		type;				/* Blob offset for the type */

};

/*
 * Set the type of an field marshaller to an explicit blob index.
 */
void _ILFieldMarshalSetTypeIndex(ILFieldMarshal *marshal, ILUInt32 index);

/*
 * Information about a class layout token.
 */
struct _tagILClassLayout
{
	ILOwnedItem		ownedItem;			/* Parent class fields */
	ILUInt32		packingSize;		/* Packing size for the class */
	ILUInt32		classSize;			/* Total size for the class */

};

/*
 * Information about a security token.
 */
struct _tagILDeclSecurity
{
	ILOwnedItem		ownedItem;			/* Parent class fields */
	ILUInt32		type;				/* Capability type */
	ILUInt32		blob;				/* Data for the security blob */

};

/*
 * Set the value of a security record to an explicit blob index.
 */
void _ILDeclSecuritySetBlobIndex(ILDeclSecurity *security, ILUInt32 index);

/*
 * Information about a file declaration.
 */
struct _tagILFileDecl
{
	ILProgramItem	programItem;		/* Parent class fields */
	const char     *name;				/* Name of the file */
	ILUInt32		attributes;			/* Attributes for the file */
	ILUInt32		hash;				/* Hash value in the blob pool */

};

/*
 * Set the hash value of a file declaration to an explicit blob index.
 */
void _ILFileDeclSetHashIndex(ILFileDecl *decl, ILUInt32 index);

/*
 * Information about a manifest resource declaration.
 */
struct _tagILManifestRes
{
	ILProgramItem	programItem;		/* Parent class fields */
	const char     *name;				/* Name of the resource */
	ILUInt32		attributes;			/* Attributes for the resource */
	ILProgramItem  *owner;				/* Owner of the resource data */
	ILUInt32		offset;				/* Offset for file owners */

};

/*
 * Information about an exported type declaration.
 */
struct _tagILExportedType
{
	ILClass			classItem;			/* Parent class fields */
	ILUInt32		identifier;			/* Foreign identifier for the type */

};

/*
 * Information about a generic parameter.
 */
struct _tagILGenericPar
{
	ILOwnedItem		ownedItem;			/* Parent class fields */
	ILUInt16		number;				/* Parameter number */
	ILUInt16		flags;				/* Parameter flags */
	const char     *name;				/* Parameter name */
	ILGenericConstraint  *firstConstraint;	/* Parameter constraints */
	
};

/*
 * Information about a generic constraint.
 */
struct _tagILGenericConstraint
{
	ILOwnedItem		ownedItem;			/* Parent class fields */
	ILProgramItem  *parameter;			/* Generic parameter to modify */
	ILProgramItem  *constraint;			/* Constraint to apply */
	ILGenericConstraint *nextConstraint;    /* Next constraint for this parameter */
	
};

/*
 * Information about a generic method specification.
 */
struct _tagILMethodSpec
{
	ILProgramItem	programItem;		/* Parent class fields */
	ILMember       *method;				/* Method specification applies to */
	ILType         *type;				/* Instantiation information */
	ILUInt32		typeBlob;			/* Blob offset of the type */

};

/*
 * Set the value of a MethodSpec type to an explicit blob index.
 */
void _ILMethodSpecSetTypeIndex(ILMethodSpec *spec, ILUInt32 index);

/*
 * Some internal helper macros to convert a program item.
 */
#define _ILProgramItem_ToAssemblyDef(item) \
	(ILAssembly *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_ASSEMBLY) ? \
			(item) : 0)

#define _ILProgramItem_ToAssemblyRef(item) \
	(ILAssembly *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_ASSEMBLY_REF) ? \
			(item) : 0)

#define _ILProgramItem_ToAttribute(item) \
	(ILAttribute *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_ATTRIBUTE) ? \
			(item) : 0)

#define _ILProgramItem_ToExportedType(item) \
	(ILClass *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_EXPORTED_TYPE) ? \
			(item) : 0)

#define _ILProgramItem_ToEventDef(item) \
	(ILEvent *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_EVENT) ? \
			(item) : 0)

#define _ILProgramItem_ToFieldDef(item) \
	(ILField *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_FIELD_DEF) ? \
			(item) : 0)

#define _ILProgramItem_ToMemberRef(item) \
	(ILMember *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MEMBER_REF) ? \
			(item) : 0)

#define _ILProgramItem_ToMethodDef(item) \
	(ILMethod *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_METHOD_DEF) ? \
			(item) : 0)

#define _ILProgramItem_ToMethodImpl(item) \
	(ILOverride *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_METHOD_IMPL) ? \
			(item) : 0)

#define _ILProgramItem_ToModuleDef(item) \
	(ILModule *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MODULE) ? \
			(item) : 0)

#define _ILProgramItem_ToModuleRef(item) \
	(ILModule *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MODULE_REF) ? \
			(item) : 0)

#define _ILProgramItem_ToParamDef(item) \
	(ILParameter *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_PARAM_DEF) ? \
			(item) : 0)

#define _ILProgramItem_ToPropertyDef(item) \
	(ILProperty *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_PROPERTY) ? \
			(item) : 0)

#define _ILProgramItem_ToTypeDef(item) \
	(ILClass *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_DEF) ? \
			(item) : 0)

#define _ILProgramItem_ToTypeRef(item) \
	(ILClass *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_REF) ? \
			(item) : 0)

#define _ILProgramItem_ToTypeSpec(item) \
	(ILTypeSpec *)((item) && \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_SPEC) ? \
			(item) : 0)

#define _ILProgramItem_ToAssembly(item) \
	(ILAssembly *)((item) && \
			((((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_ASSEMBLY) || \
			(((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_ASSEMBLY_REF)) ? \
			(item) : 0)

#define _ILProgramItem_ToClass(item) \
	(ILClass *)((item) && \
			((((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_DEF) || \
	        (((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_REF) || \
	        (((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_EXPORTED_TYPE)) ? \
			(item) : 0)

#define _ILProgramItem_ToModule(item) \
	(ILModule *)((item) && \
			((((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MODULE) || \
	        (((item)->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MODULE_REF)) ? \
			(item) : 0)

#define _ILToProgramItem(x)	(&((x)->programItem))

/*
 * Internal macros for accessing members in external opaque types
 */
#define _ILClass_Synthetic(info)	((info)->synthetic)
#define _ILClass_Implements(info)	((info)->implements)
#define _ILImplements_NextImplements(impl)	((impl)->nextInterface)
#define _ILTypeSpec_Type(spec)	((spec)->type)
#define _ILMember_Owner(member)	((member)->owner)
#define _ILMethod_Owner(method)	((method)->member.owner)

/*
 * Misc internal macros
 */
#define _ILClass_IsInterface(info)	\
			(((info)->attributes & IL_META_TYPEDEF_CLASS_SEMANTICS_MASK) \
					== IL_META_TYPEDEF_INTERFACE)


#ifdef	__cplusplus
};
#endif

#endif	/* _IMAGE_PROGRAM_H */
