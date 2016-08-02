/*
 * c_declspec.h - Declaration specifiers for the C programming language.
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

#ifndef	_C_DECLSPEC_H
#define	_C_DECLSPEC_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Storage class and type specifiers.
 */
#define	C_SPEC_TYPEDEF			(1<<0)
#define	C_SPEC_EXTERN			(1<<1)
#define	C_SPEC_STATIC			(1<<2)
#define	C_SPEC_AUTO				(1<<3)
#define	C_SPEC_REGISTER			(1<<4)
#define	C_SPEC_INLINE			(1<<5)
#define	C_SPEC_SHORT			(1<<6)
#define	C_SPEC_LONG				(1<<7)
#define	C_SPEC_LONG_LONG		(1<<8)
#define	C_SPEC_SIGNED			(1<<9)
#define	C_SPEC_UNSIGNED			(1<<10)
#define	C_SPEC_CONST			(1<<11)
#define	C_SPEC_VOLATILE			(1<<12)
#define	C_SPEC_MULTIPLE_BASES	(1<<13)
#define	C_SPEC_LONG_AND_SHORT	(1<<14)
#define	C_SPEC_SIGN_AND_UNSIGN	(1<<15)
#define	C_SPEC_INVALID_COMBO	(1<<16)
#define	C_SPEC_ENUM				(1<<17)
#define C_SPEC_THREAD_SPECIFIC	(1<<18)
#define C_SPEC_BOX				(1<<19)

/*
 * Useful specifier combinations.
 */
#define	C_SPEC_STORAGE		\
			(C_SPEC_TYPEDEF | \
			 C_SPEC_EXTERN | \
			 C_SPEC_STATIC | \
			 C_SPEC_AUTO | \
			 C_SPEC_REGISTER | \
			 C_SPEC_INLINE | \
			 C_SPEC_THREAD_SPECIFIC)
#define	C_SPEC_TYPE_COMMON		\
			(C_SPEC_CONST | \
			 C_SPEC_VOLATILE | \
			 C_SPEC_BOX | \
			 C_SPEC_ENUM)
#define	C_SPEC_TYPE_CHANGE		\
			(C_SPEC_SHORT | \
			 C_SPEC_LONG | \
			 C_SPEC_LONG_LONG | \
			 C_SPEC_SIGNED | \
			 C_SPEC_UNSIGNED)

/*
 * Kinds of declarations, for checking the applicability of specifiers.
 */
#define	C_KIND_GLOBAL_NAME		(1<<0)
#define	C_KIND_LOCAL_NAME		(1<<1)
#define	C_KIND_PARAMETER_NAME	(1<<2)
#define	C_KIND_FUNCTION			(1<<3)

/*
 * Structure of a declaration specifier.
 */
typedef struct
{
	int		specifiers;
	int		dupSpecifiers;
	ILType *baseType;

} CDeclSpec;

/*
 * Set a declaration specifier to a particular flag.
 */
#define	CDeclSpecSet(spec,flag)	\
			do { \
				(spec).specifiers = (flag); \
				(spec).dupSpecifiers = 0; \
				(spec).baseType = ILType_Invalid; \
			} while (0)

/*
 * Set a declaration specifier to a particular base type.
 */
#define	CDeclSpecSetType(spec,type)	\
			do { \
				(spec).specifiers = 0; \
				(spec).dupSpecifiers = 0; \
				(spec).baseType = (type); \
			} while (0)

/*
 * Convert a storage class specifier into a name.
 * Returns NULL for an empty storage class.
 */
const char *CStorageClassToName(int specifier);

/*
 * Create an empty declaration specifier.
 */
CDeclSpec CDeclSpecEmpty(void);

/*
 * Combine two declaration specifiers.
 */
CDeclSpec CDeclSpecCombine(CDeclSpec spec1, CDeclSpec spec2);

/*
 * Finalize a declaration specifier, and output any relevant
 * errors and warnings.  The final "specifiers" value is the
 * storage class, with everything else in "baseType".
 */
CDeclSpec CDeclSpecFinalize(CDeclSpec spec, ILNode *node,
							const char *name, int kind);

/*
 * Information about a declarator.
 */
typedef struct
{
	const char *name;		/* Name represented by the declarator */
	ILNode	   *node;		/* Node that defines the name, for errors */
	ILType	   *type;		/* Type template for the declarator */
	ILType	  **typeHole;	/* The hole in the template for the base type */
	int			isKR;		/* Non-zero if a K&R-style prototype */
	ILNode	   *params;		/* Declared function parameters */
	ILNode	   *attrs;		/* Declared function attributes */

} CDeclarator;

/*
 * Set a declarator to a name.
 */
#define	CDeclSetName(decl,dname,dnode)	\
			do { \
				(decl).name = (dname); \
				(decl).node = (dnode); \
				(decl).type = ILType_Invalid; \
				(decl).typeHole = 0; \
				(decl).isKR = 0; \
				(decl).params = 0; \
				(decl).attrs = 0; \
			} while (0)

/*
 * Create an array declarator, with unspecified size.
 */
CDeclarator CDeclCreateOpenArray(ILGenInfo *info, CDeclarator elem,
								 int gcSpecifier);

/*
 * Create an array declarator, with a specified size.
 */
CDeclarator CDeclCreateArray(ILGenInfo *info, CDeclarator elem,
							 ILNode *size, int gcSpecifier);

/*
 * Create a pointer declarator.  "refType" will be non-NULL when
 * creating a pointer to another pointer/reference type.
 */
CDeclarator CDeclCreatePointer(ILGenInfo *info, int qualifiers,
							   CDeclarator *refType);

/*
 * Create a by-ref '&' declarator.  "refType" will be non-NULL when
 * creating a reference to another pointer/reference type.
 */
CDeclarator CDeclCreateByRef(ILGenInfo *info, int qualifiers,
							 CDeclarator *refType);

/*
 * Create a function prototype declarator.
 */
CDeclarator CDeclCreatePrototype(ILGenInfo *info, CDeclarator decl,
								 ILNode *params, ILNode *attributes);

/*
 * Combine two declarators, where "decl1" is inserted into the
 * type hole in "decl2".  "decl1" is assumed to be a pointer type.
 */
CDeclarator CDeclCombine(CDeclarator decl1, CDeclarator decl2);

/*
 * Combine a set of declaration specifiers and a declarator
 * to produce a final type and function parameter list.
 * "declaredParams" are the K&R-style parameter declarations.
 * "method" is non-NULL to set the signature for a method.
 */
ILType *CDeclFinalize(ILGenInfo *info, CDeclSpec spec, CDeclarator decl,
					  ILNode *declaredParams, ILMethod *method);

#ifdef	__cplusplus
};
#endif

#endif	/* _C_DECLSPEC_H */
