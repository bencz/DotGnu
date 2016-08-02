/*
 * c_types.h - Type representation for the C programming language.
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

#ifndef	_C_TYPES_H
#define	_C_TYPES_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Kinds of structs or unions that may be encountered by the C parser.
 */
#define	C_STKIND_STRUCT			0
#define	C_STKIND_UNION			1

/*
 * Special bits for marking references to "struct", "union", and "enum"
 * types before we encounter the main body of the type.
 */
#define	IL_META_TYPEDEF_IS_STRUCT		0x04000000
#define	IL_META_TYPEDEF_IS_UNION		0x02000000
#define	IL_META_TYPEDEF_IS_ENUM			0x01000000
#define	IL_META_TYPEDEF_TYPE_BITS		0x07000000

/*
 * Type categories.
 */
#define	C_TYPECAT_FIXED			0		/* Fixed size and layout */
#define	C_TYPECAT_DYNAMIC		1		/* Dynamic size and layout */
#define	C_TYPECAT_COMPLEX		2		/* Complex size and layout */
#define	C_TYPECAT_NO_LAYOUT		3		/* Cannot layout (e.g. open arrays) */

/*
 * Flag bit versions of the above categories.
 */
#define	C_TYPECAT_FIXED_BIT		(1 << C_TYPECAT_FIXED)
#define	C_TYPECAT_DYNAMIC_BIT	(1 << C_TYPECAT_DYNAMIC)
#define	C_TYPECAT_COMPLEX_BIT	(1 << C_TYPECAT_COMPLEX)
#define	C_TYPECAT_NO_LAYOUT_BIT	(1 << C_TYPECAT_NO_LAYOUT)

/*
 * Alignment flags.  The first 5 entries must have the values 1, 2, 4, 8, 16
 * so that a computed size value can be used directly as alignment flags.
 */
#define	C_ALIGN_BYTE			0x0001
#define	C_ALIGN_2				0x0002
#define	C_ALIGN_4				0x0004
#define	C_ALIGN_8				0x0008
#define	C_ALIGN_16				0x0010
#define	C_ALIGN_SHORT			0x0020
#define	C_ALIGN_INT				0x0040
#define	C_ALIGN_LONG			0x0080
#define	C_ALIGN_FLOAT			0x0100
#define	C_ALIGN_DOUBLE			0x0200
#define	C_ALIGN_POINTER			0x0400
#define	C_ALIGN_UNKNOWN			0x0800

/*
 * Special value that is used to indicate that the size of
 * a type must be computed at runtime.
 */
#define	CTYPE_DYNAMIC	IL_MAX_UINT32

/*
 * Special value that is used to indicate that the size of
 * a type is unknown because it is not yet fully defined.
 */
#define	CTYPE_UNKNOWN	(IL_MAX_UINT32 - 1)

/*
 * Layout information that may be retrieved about a type.  The "size"
 * will be CTYPE_DYNAMIC for non-fixed type categories, and CTYPE_UNKNOWN
 * for types that have no layout (e.g. open arrays).
 *
 * The "measureType" field is usually the same as the type being laid out.
 * Under some circumstances it may be different.  e.g. The measuring type
 * for object references and pointers is "IntPtr".
 */
typedef struct
{
	int			category;
	ILUInt32	size;
	ILUInt32	alignFlags;
	ILType	   *measureType;

} CTypeLayoutInfo;

/*
 * Create a struct or union type with a specific name.  If the
 * type already exists, then return it as-is.
 */
ILType *CTypeCreateStructOrUnion(ILGenInfo *info, const char *name,
								 int kind, const char *funcName);

/*
 * Create a C "enum" type with a specific name.  If the
 * "enum" type already exists, then return it as-is.
 */
ILType *CTypeCreateEnum(ILGenInfo *info, const char *name,
						const char *funcName);

/*
 * Create a C array type.  If the array type already exists,
 * then return it as-is.  Returns NULL if the array size is
 * too large to be represented by the CLI implementation,
 * or if the element type has a dynamic size.
 */
ILType *CTypeCreateArray(ILGenInfo *info, ILType *elemType, ILUInt32 size);

/*
 * Create a C array type, using a node as the size value.
 */
ILType *CTypeCreateArrayNode(ILGenInfo *info, ILType *elemType, ILNode *size);

/*
 * Create an open-ended C array type.  If the array type already
 * exists, then return it as-is.  Returns NULL if the element
 * type has a dynamic size.
 */
ILType *CTypeCreateOpenArray(ILGenInfo *info, ILType *elemType);

/*
 * Create a C pointer type.
 */
ILType *CTypeCreatePointer(ILGenInfo *info, ILType *refType);

/*
 * Create a C pointer type which is marked as a decayed pointer
 * to a complex type.
 */
ILType *CTypeCreateComplexPointer(ILGenInfo *info, ILType *refType);

/*
 * Create a C by-ref type.
 */
ILType *CTypeCreateByRef(ILGenInfo *info, ILType *refType);

/*
 * Create a C type reference for "builtin_va_list".
 */
ILType *CTypeCreateVaList(ILGenInfo *info);

/*
 * Create a C type reference for "void *".
 */
ILType *CTypeCreateVoidPtr(ILGenInfo *info);

/*
 * Create a C type reference for "char *".
 */
ILType *CTypeCreateCharPtr(ILGenInfo *info);

/*
 * Create a C type reference for "wchar_t *".
 */
ILType *CTypeCreateWCharPtr(ILGenInfo *info);

/*
 * Mark a C type with a "const" qualifier.
 */
ILType *CTypeAddConst(ILGenInfo *info, ILType *type);

/*
 * Mark a C type with a "volatile" qualifier.
 */
ILType *CTypeAddVolatile(ILGenInfo *info, ILType *type);

/*
 * Mark a C type as a function pointer type.
 */
ILType *CTypeAddFunctionPtr(ILGenInfo *info, ILType *type);

/*
 * Mark a C type with the "__gc" modifier.
 */
ILType *CTypeAddManaged(ILGenInfo *info, ILType *type);

/*
 * Mark a C type with the "__nogc" modifier.
 */
ILType *CTypeAddUnmanaged(ILGenInfo *info, ILType *type);

/*
 * Strip the "__gc" or "__nogc" modifier from a type.
 */
ILType *CTypeStripGC(ILType *type);

/*
 * Determine if a type returned from "CTypeCreateStruct",
 * "CTypeCreateUnion", or "CTypeCreateEnum" has already
 * been defined by the compiler.
 */
int CTypeAlreadyDefined(ILType *type);

/*
 * Define a struct or union type with a specific name.  Returns NULL
 * if the structure is already defined.
 */
ILType *CTypeDefineStructOrUnion(ILGenInfo *info, const char *name,
								 int kind, const char *funcName);

/*
 * Define an anonymous "struct" or "union" type within a given parent
 * and function.
 */
ILType *CTypeDefineAnonStructOrUnion(ILGenInfo *info, ILType *parent,
							         const char *funcName, int kind);

/*
 * Define an "enum" type with a specific name.  Returns NULL
 * if the enum is already defined.
 */
ILType *CTypeDefineEnum(ILGenInfo *info, const char *name,
						const char *funcName);

/*
 * Define an anonymous "enum" type at the global level or within
 * a specified function.
 */
ILType *CTypeDefineAnonEnum(ILGenInfo *info, const char *funcName);

/*
 * Resolve an anonymous enumerated type to its underlying type.
 * Regular enumerated types are left as-is.
 */
ILType *CTypeResolveAnonEnum(ILType *type);

/*
 * Define a new field within a "struct" or "union".  Returns NULL
 * if the type is dynamic in size.
 */
ILField *CTypeDefineField(ILGenInfo *info, ILType *structType,
					 	  const char *fieldName, ILType *fieldType);

/*
 * Define a new bit field within a "struct" or "union".
 */
int CTypeDefineBitField(ILGenInfo *info, ILType *structType,
				 	    const char *fieldName, ILType *fieldType,
						ILUInt32 numBits, ILUInt32 maxBits);

/*
 * End the definition of a "struct" or "union".  This will update
 * the type size to reflect the maximum alignment.  If "renameAnon"
 * is non-zero, then we need to rename a top-level anonymous struct.
 */
ILType *CTypeEndStruct(ILGenInfo *info, ILType *structType, int renameAnon);

/*
 * Look up a field name within a "struct" or "union" type.
 * "*bitFieldSize" will be zero for a non bit field.  Returns
 * NULL if the field could not be found.
 */
ILField *CTypeLookupField(ILGenInfo *info, ILType *structType,
						  const char *fieldName, ILUInt32 *bitFieldStart,
						  ILUInt32 *bitFieldSize);

/*
 * Define a new enumerated constant within an "enum" type.
 */
void CTypeDefineEnumConst(ILGenInfo *info, ILType *enumType,
					 	  const char *constName, ILInt32 constValue);

/*
 * Remove qualifiers from a C type.
 */
ILType *CTypeWithoutQuals(ILType *type);

/*
 * Determine if a C type has the "const" qualifier.
 */
int CTypeIsConst(ILType *type);

/*
 * Determine if a C type has the "volatile" qualifier.
 */
int CTypeIsVolatile(ILType *type);

/*
 * Determine if a C type has the "__gc" qualifier.
 */
int CTypeIsManaged(ILType *type);

/*
 * Determine if a C type has the "__nogc" qualifier.
 */
int CTypeIsUnmanaged(ILType *type);

/*
 * Determine if a C type is a decayed pointer to a complex type.
 */
int CTypeIsComplexPointer(ILType *type);

/*
 * Determine if a C type is primitive (this includes enumerated types).
 */
int CTypeIsPrimitive(ILType *type);

/*
 * Determine if a C type is primitive and integer.
 */
int CTypeIsInteger(ILType *type);

/*
 * Determine if a C type is a structure.
 */
int CTypeIsStruct(ILType *type);

/*
 * Determine if a C type is a union.
 */
int CTypeIsUnion(ILType *type);

/*
 * Get the kind of a structure or union type.  -1 if not a struct or union.
 */
int CTypeGetStructKind(ILType *type);

/*
 * Determine if a C type is an enumerated type.
 */
int CTypeIsEnum(ILType *type);

/*
 * Determine if a C type is an anonymous enumerated type.
 */
int CTypeIsAnonEnum(ILType *type);

/*
 * Determine if a C type is an array (open or with a specified size).
 */
int CTypeIsArray(ILType *type);

/*
 * Determine if a C type is an open-ended array.
 */
int CTypeIsOpenArray(ILType *type);

/*
 * Determine if a C type is a pointer type.
 */
int CTypeIsPointer(ILType *type);

/*
 * Determine if a C type is a by-ref type.
 */
int CTypeIsByRef(ILType *type);

/*
 * Determine if a C type is a function type.
 */
int CTypeIsFunction(ILType *type);

/*
 * Determine if a C type is a function pointer type.
 */
int CTypeIsFunctionPtr(ILType *type);

/*
 * Determine if a C type is an object reference.
 */
int CTypeIsReference(ILType *type);

/*
 * Convert a C type into an element type.
 */
int CTypeToElementType(ILType *type);

/*
 * Get the number of elements within an array type.  Returns IL_MAX_UINT32
 * if the array has a complex size that needs to be computed at runtime.
 */
ILUInt32 CTypeGetNumElems(ILType *type);

/*
 * Get the element type of an array type.
 */
ILType *CTypeGetElemType(ILType *type);

/*
 * Get the type that is referenced by a pointer type.
 */
ILType *CTypeGetPtrRef(ILType *type);

/*
 * Get the type that is referenced by a by-ref type.
 */
ILType *CTypeGetByRef(ILType *type);

/*
 * Decay array types into their corresponding pointer types.
 */
ILType *CTypeDecay(ILGenInfo *info, ILType *type);

/*
 * Determine if two C types are identical, including qualifiers.
 */
int CTypeIsIdentical(ILType *type1, ILType *type2);

/*
 * Convert a C type into a name that can be displayed in an
 * error message.  Use "ILFree" to free the return value.
 */
char *CTypeToName(ILGenInfo *info, ILType *type);

/*
 * Resolve a qualified identifier that describes a C# type
 * name into a C type reference.  Returns NULL if it isn't
 * possible to resolve the identifier.
 */
ILType *CTypeFromCSharp(ILGenInfo *info, const char *assembly, ILNode *node);

/*
 * Mark a type as needing to be output at code generation time.
 */
void CTypeMarkForOutput(ILGenInfo *info, ILType *type);

/*
 * Output pending type definitions to an assembly output stream.
 * This must be called outside the context of a method.
 */
void CTypeOutputPending(ILGenInfo *info, FILE *stream);

/*
 * Enumerate over the fields within a structure or union type.
 */
ILField *CTypeNextField(ILType *type, ILField *last);

/*
 * Get the layout information for a type.
 */
void CTypeGetLayoutInfo(ILType *type, CTypeLayoutInfo *info);

/*
 * Get the maximal alignment value that corresponds to a set of flags.
 * Returns zero if the alignment is unknown.
 */
ILUInt32 CTypeGetMaxAlign(ILUInt32 alignFlags);

/*
 * Create a node that describes the size of a type in terms of
 * primitive sizeof/alignof operations.  Returns NULL if the
 * type does not have any layout information.
 */
ILNode *CTypeCreateSizeNode(ILType *type);

/*
 * Create a node that describes the offset of a field within a type.
 * Returns NULL for the first field, or if the type doesn't have fields.
 * This is only meanginful for types with "complex" layout as "fixed"
 * and "dynamic" types can use the IL field opcodes instead.
 */
ILNode *CTypeCreateOffsetNode(ILType *type, const char *name);

/*
 * Get the complex size designator for an array, or NULL if
 * the type is not an array or does not have a complex size.
 */
ILNode *CTypeGetComplexArraySize(ILType *type);

/*
 * Get the field to use to measure the alignment of a type.
 * The field is contained within a structure that looks like this:
 *
 *		struct {
 *			char pad;
 *			type value;
 *		};
 *
 * The offset of the "value" field will be the alignment value for "type".
 */
ILField *CTypeGetMeasureField(ILGenInfo *info, ILType *type);

/*
 * Determine if a type is complex.
 */
int CTypeIsComplex(ILType *type);

/*
 * Get a parameter type, after stripping complex pointer markers.
 */
ILType *CTypeGetParam(ILType *signature, unsigned long param);

/*
 * Get the return type, after stripping complex pointer markers.
 */
ILType *CTypeGetReturn(ILType *signature);

#ifdef	__cplusplus
};
#endif

#endif	/* _C_TYPES_H */
