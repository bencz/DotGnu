/*
 * doc_tree.h - Storage for a documentation tree in memory.
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

#ifndef	_CSDOC_DOC_TREE_H
#define	_CSDOC_DOC_TREE_H

#include "il_xml.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Forward declarations.
 */
typedef struct _tagILDocTree		ILDocTree;
typedef struct _tagILDocLibrary		ILDocLibrary;
typedef struct _tagILDocNamespace	ILDocNamespace;
typedef struct _tagILDocType		ILDocType;
typedef struct _tagILDocInterface	ILDocInterface;
typedef struct _tagILDocAttribute	ILDocAttribute;
typedef struct _tagILDocMember		ILDocMember;
typedef struct _tagILDocParameter	ILDocParameter;
typedef struct _tagILDocText		ILDocText;

/*
 * Information that is stored about an entire documentation tree.
 */
#define	IL_DOC_HASH_SIZE			509
struct _tagILDocTree
{
	ILDocLibrary   *libraries;		/* Libraries within the tree */
	ILDocLibrary   *lastLibrary;	/* Last library within the tree */
	ILDocNamespace *namespaces;		/* Namespaces within the tree */
	ILDocType      *hash[IL_DOC_HASH_SIZE]; /* Type name hash table */

};

/*
 * Information that is stored about a type library.
 */
struct _tagILDocLibrary
{
	ILDocTree	   *tree;			/* Tree that this library belongs to */
	char		   *name;			/* Name of the library, or NULL if none */
	ILDocType	   *types;			/* List of types in the library */
	ILDocLibrary   *next;			/* Next library within the tree */

};

/*
 * Information that is stored about a namespace.
 */
struct _tagILDocNamespace
{
	ILDocTree	   *tree;			/* Tree that this namespace belongs to */
	char		   *name;			/* Name of the namespace ("" if none) */
	ILDocType      *types;			/* Sorted list of types in the namespace */
	ILDocNamespace *next;			/* Next namespace within the tree */

};

/*
 * Type kinds.
 */
typedef enum
{
	ILDocTypeKind_Class,
	ILDocTypeKind_Interface,
	ILDocTypeKind_Struct,
	ILDocTypeKind_Enum,
	ILDocTypeKind_Delegate,

} ILDocTypeKind;

/*
 * Invalid value for "typeAttrs" and "memberAttrs".
 */
#define	ILDocInvalidAttrs	(~((unsigned long)0))

/*
 * Information that is stored about a type.
 */
struct _tagILDocType
{
	ILDocTree	   *tree;			/* Tree that this type belongs to */
	ILDocLibrary   *library;		/* Library that this type belongs to */
	ILDocNamespace *namespace;		/* Namespace that this type belongs to */
	ILDocTypeKind	kind;			/* Kind of type */
	char		   *name;			/* Name, without the namespace qualifier */
	char		   *fullName;		/* Full name of the type */
	int				fullyQualify;	/* Non-zero if name qualify recommended */
	char		   *assembly;		/* Name of the containing assembly */
	char		   *ilasmSignature;	/* ILASM signature for the type */
	char		   *csSignature;	/* C# signature for the type */
	char		   *baseType;		/* Full name of the base type */
	char		   *excludedBaseType; /* Full name of the excluded base type */
	unsigned long	typeAttrs;		/* Metadata attribute flags */
	ILDocInterface *interfaces;		/* List of the type's interfaces */
	ILDocAttribute *attributes;		/* Attributes attached to the type */
	ILDocText      *doc;			/* Text of the type's documentation */
	ILDocMember    *members;		/* List of type members */
	ILDocType	   *next;			/* Next type in the same library */
	ILDocType	   *nextNamespace;	/* Next type in the same namespace */
	ILDocType      *nextHash;		/* Next in type name hash table */

};

/*
 * Information that is stored about an interface.
 */
struct _tagILDocInterface
{
	char		   *name;			/* Name of the interface */
	ILDocInterface *next;			/* Next interface for the type */

};

/*
 * Information that is stored about and attribute.
 */
struct _tagILDocAttribute
{
	char		   *name;			/* Name and parameters for the attribute */
	ILDocAttribute *next;			/* Next attribute for the type */

};

/*
 * Supported member types.
 */
typedef enum
{
	ILDocMemberType_Constructor,
	ILDocMemberType_Method,
	ILDocMemberType_Field,
	ILDocMemberType_Property,
	ILDocMemberType_Event,
	ILDocMemberType_Unknown,

} ILDocMemberType;

/*
 * Information that is stored about a type member.
 */
struct _tagILDocMember
{
	ILDocTree	   *tree;			/* Tree that this member belongs to */
	ILDocType	   *type;			/* Type that this member belongs to */
	char		   *name;			/* Name of the member */
	ILDocMemberType	memberType;		/* Type of member */
	int				fullyQualify;	/* Non-zero if needs full qualification */
	char		   *ilasmSignature;	/* ILASM signature for the type */
	char		   *csSignature;	/* C# signature for the type */
	char		   *returnType;		/* Return type (NULL for "void") */
	char		   *libraryName;	/* Name of the containing library */
	unsigned long	memberAttrs;	/* Metadata attribute flags */
	ILDocParameter *parameters;		/* List of member parameters */
	ILDocAttribute *attributes;		/* Attributes attached to the type */
	int				index;			/* Index value for conversion routines */
	ILDocText      *doc;			/* Text of the type's documentation */
	ILDocMember    *next;			/* Next member within the type */

};

/*
 * Information that is stored about a member parameter.
 */
struct _tagILDocParameter
{
	char		   *name;			/* Name of the parameter */
	char		   *type;			/* Type of the parameter */
	ILDocParameter *next;			/* Next parameter for the member */

};

/*
 * Information that is stored about the documentation text for an item.
 */
struct _tagILDocText
{
	ILDocText	   *parent;			/* Parent of this node (NULL if topmost) */
	ILDocText	   *next;			/* Next in the list of doc nodes */
	int				isTag;			/* Non-zero if a tag, zero if text */
	int				size;			/* Used internally by ILDocTextGetParam */
	ILDocText	   *children;		/* List of children of a tag node */
	char			text[1];		/* Text in the node, or the tag name */

};

/*
 * Create a documentation tree.  Returns NULL if out of memory.
 */
ILDocTree *ILDocTreeCreate(void);

/*
 * Load the contents of an XML stream into a documentation tree.
 * Returns zero if out of memory.
 */
int ILDocTreeLoad(ILDocTree *tree, ILXMLReader *reader);

/*
 * Sort the contents of a documentation tree and collect
 * all of the namespaces.  Returns zero if out of memory.
 */
int ILDocTreeSort(ILDocTree *tree);

/*
 * Destroy a documentation tree.
 */
void ILDocTreeDestroy(ILDocTree *tree);

/*
 * Find a type given its full name.  Returns NULL if
 * the type is not defined in the tree, but may be
 * defined externally.
 */
ILDocType *ILDocTypeFind(ILDocTree *tree, const char *name);

/*
 * Get a particular parameter from a documentation tag node.
 * Returns NULL if not a tag node, or the parameter isn't present.
 */
const char *ILDocTextGetParam(ILDocText *text, const char *name);

/*
 * Get the first element in a documentation list with a given tag node.
 * Returns NULL if the element does not exist.
 */
ILDocText *ILDocTextFirstChild(ILDocText *text, const char *name);

/*
 * Get a named child of a documentation tag node.
 * Returns NULL if the child does not exist.
 */
ILDocText *ILDocTextGetChild(ILDocText *text, const char *name);

/*
 * Get the next child with the same name.
 * Returns NULL if no more children.
 */
ILDocText *ILDocTextNextChild(ILDocText *text, const char *name);

/*
 * Determine if a text node only consists of white space.
 */
int ILDocTextAllSpaces(ILDocText *text);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSDOC_DOC_TREE_H */
