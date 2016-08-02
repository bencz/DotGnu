/*
 * il_types.h - Type representation for IL images.
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

#ifndef	_IL_TYPES_H
#define	_IL_TYPES_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Forward declarations.
 */
typedef struct _tagILType      ILType;
typedef struct _tagILClass     ILClass;

/*
 * Invalid type code.
 */
#define	ILType_Invalid		((ILType *)0)

/*
 * Convert a primitive element type into a type representation.
 */
#define	ILType_FromElement(elemType)	\
				((ILType *)((((ILNativeUInt)(elemType)) << 2) | 0x01))

/*
 * Convert a type representation into a primitive element type.
 */
#define	ILType_ToElement(type)	\
				(((ILNativeUInt)(type)) >> 2)

/*
 * Convert a class pointer into a type representation.
 */
#define	ILType_FromClass(info)	\
				((ILType *)(((ILNativeUInt)(info)) | 0x02))

/*
 * Convert a type representation back into a class pointer.
 */
#define	ILType_ToClass(type)	\
				((ILClass *)(((ILNativeUInt)(type)) & ~0x03))

/*
 * Convert a value type pointer into a type representation.
 */
#define	ILType_FromValueType(info)	\
				((ILType *)(((ILNativeUInt)(info)) | 0x03))

/*
 * Convert a type representation back into a value type pointer.
 */
#define	ILType_ToValueType(type)	\
				((ILClass *)(((ILNativeUInt)(type)) & ~0x03))

/*
 * Primitive types.
 */
#define	ILType_Void			ILType_FromElement(IL_META_ELEMTYPE_VOID)
#define	ILType_Boolean		ILType_FromElement(IL_META_ELEMTYPE_BOOLEAN)
#define	ILType_Char			ILType_FromElement(IL_META_ELEMTYPE_CHAR)
#define	ILType_Int8			ILType_FromElement(IL_META_ELEMTYPE_I1)
#define	ILType_UInt8		ILType_FromElement(IL_META_ELEMTYPE_U1)
#define	ILType_Int16		ILType_FromElement(IL_META_ELEMTYPE_I2)
#define	ILType_UInt16		ILType_FromElement(IL_META_ELEMTYPE_U2)
#define	ILType_Int32		ILType_FromElement(IL_META_ELEMTYPE_I4)
#define	ILType_UInt32		ILType_FromElement(IL_META_ELEMTYPE_U4)
#define	ILType_Int64		ILType_FromElement(IL_META_ELEMTYPE_I8)
#define	ILType_UInt64		ILType_FromElement(IL_META_ELEMTYPE_U8)
#define	ILType_Float32		ILType_FromElement(IL_META_ELEMTYPE_R4)
#define	ILType_Float64		ILType_FromElement(IL_META_ELEMTYPE_R8)
#define	ILType_String		ILType_FromElement(IL_META_ELEMTYPE_STRING)
#define	ILType_Int			ILType_FromElement(IL_META_ELEMTYPE_I)
#define	ILType_UInt			ILType_FromElement(IL_META_ELEMTYPE_U)
#define	ILType_Float		ILType_FromElement(IL_META_ELEMTYPE_R)
#define	ILType_TypedRef		ILType_FromElement(IL_META_ELEMTYPE_TYPEDBYREF)
#define	ILType_Sentinel		ILType_FromElement(IL_META_ELEMTYPE_SENTINEL)

/*
 * Special value that can be used to represent the type
 * of the "null" constant in compilers.
 */
#define	ILType_Null			ILType_FromElement(0xFF)

/*
 * Kinds of complex types.
 */
#define	IL_TYPE_COMPLEX_BYREF					1
#define	IL_TYPE_COMPLEX_PTR						2
#define	IL_TYPE_COMPLEX_ARRAY					3
#define	IL_TYPE_COMPLEX_ARRAY_CONTINUE			4
#define	IL_TYPE_COMPLEX_CMOD_REQD				6
#define	IL_TYPE_COMPLEX_CMOD_OPT				7
#define	IL_TYPE_COMPLEX_PROPERTY				8
#define	IL_TYPE_COMPLEX_SENTINEL				9
#define	IL_TYPE_COMPLEX_PINNED					10
#define	IL_TYPE_COMPLEX_LOCALS					11
#define	IL_TYPE_COMPLEX_WITH					12
#define	IL_TYPE_COMPLEX_MVAR					13
#define	IL_TYPE_COMPLEX_VAR						14
#define	IL_TYPE_COMPLEX_METHOD					16
#define	IL_TYPE_COMPLEX_METHOD_SENTINEL			1

/*
 * Complex types.
 */
struct _tagILType
{
	short			kind__;		/* Kind of complex type */
	unsigned short	num__;		/* Number of parameters for a method */
	union {
		ILType	   *refType__;	/* Referenced type */
		struct {
			ILType *elemType__;	/* Element type */
			long	size__;		/* Size of the dimension */
			long	lowBound__;	/* Low bound for the dimension */
		} array__;
		struct {
			ILType *retType__;	/* Return type */
			ILType *param__[3];	/* Parameters */
			ILType *next__;		/* Overflow for the rest of the parameters */
			ILUInt32 gparams__;	/* Number of generic parameters */
		} method__;
		struct {
			ILType *param__[4];	/* Overflow parameters */
			ILType *next__;		/* Overflow for the rest of the parameters */
		} params__;
		struct {
			ILClass *info__;	/* Information on the modifier's class */
			ILType  *type__;	/* The type that is being modified */
		} modifier__;
		struct {
			ILType *local__[4];	/* Types for up to 4 locals */
			ILType *next__;		/* Overflow for the rest of the locals */
		} locals__;
		int         num__;		/* Generic variable number */
	} un;
};

/*
 * Determine if a type is primitive.
 */
#define	ILType_IsPrimitive(type)	\
				((((ILNativeUInt)(type)) & 0x03) == 0x01)

/*
 * Determine if a type is a class reference.
 */
#define	ILType_IsClass(type)	\
				((((ILNativeUInt)(type)) & 0x03) == 0x02)

/*
 * Determine if a type is a value type reference.
 */
#define	ILType_IsValueType(type)	\
				((((ILNativeUInt)(type)) & 0x03) == 0x03)

/*
 * Determine if a type is complex.
 */
#define	ILType_IsComplex(type)	\
				((((ILNativeUInt)(type)) & 0x03) == 0x00)

/*
 * Determine if a method type has a non-explicit "this" argument.
 * This assumes that its argument is a complex type.
 */
#define	ILType_HasThis(type)	\
				(((type)->kind__ & (IL_META_CALLCONV_HASTHIS << 8)) != 0 && \
				 ((type)->kind__ & (IL_META_CALLCONV_EXPLICITTHIS << 8)) == 0)

/*
 * Determine if a type is a single-dimensional array with
 * a zero lower bound.
 */
#define	ILType_IsSimpleArray(type)	\
				((type) != 0 && ILType_IsComplex((type)) && \
				 (type)->kind__ == IL_TYPE_COMPLEX_ARRAY && \
				 (type)->un.array__.lowBound__ == 0)

/*
 * Determine if a type is an array of arbitrary dimensions.
 */
#define	ILType_IsArray(type)	\
				((type) != 0 && ILType_IsComplex((type)) && \
				 ((type)->kind__ == IL_TYPE_COMPLEX_ARRAY || \
				  (type)->kind__ == IL_TYPE_COMPLEX_ARRAY_CONTINUE))

/*
 * Determine if a type is a method.
 */
#define	ILType_IsMethod(type)	\
				((type) != 0 && ILType_IsComplex((type)) && \
				 ((type)->kind__ & IL_TYPE_COMPLEX_METHOD) != 0)

/*
 * Determine if a type is a property.
 */
#define	ILType_IsProperty(type)	\
				((type) != 0 && ILType_IsComplex((type)) && \
				 ((type)->kind__ & 0xFF) == IL_TYPE_COMPLEX_PROPERTY)

/*
 * Determine if a type is a "with" instantiation.
 */
#define	ILType_IsWith(type)	\
				((type) != 0 && ILType_IsComplex((type)) && \
				 ((type)->kind__ & 0xFF) == IL_TYPE_COMPLEX_WITH)

/*
 * Determine if a type is a sentinel.
 */
#define	ILType_IsSentinel(type)	\
				((type) == ILType_Sentinel || \
				 ((type) != 0 && ILType_IsComplex((type)) && \
				  ((type)->kind__ & 0xFF) == IL_TYPE_COMPLEX_SENTINEL))

/*
 * Determine if a type is a managed pointer.
 */
#define	ILType_IsRef(type)	\
				((type) != 0 && ILType_IsComplex((type)) && \
				 ILType_Kind((type)) == IL_TYPE_COMPLEX_BYREF)

/*
 * Determine if a type is a pointer.
 */
#define	ILType_IsPointer(type)	\
				((type) != 0 && ILType_IsComplex((type)) && \
				 ILType_Kind((type)) == IL_TYPE_COMPLEX_PTR)

/*
 * Determine if a type is a generic type parameter.
 */
#define	ILType_IsTypeParameter(type)	\
				((type) != 0 && ILType_IsComplex((type)) && \
				 ((type)->kind__ & 0xFF) == IL_TYPE_COMPLEX_VAR)

/*
 * Determine if a type is a generic method parameter.
 */
#define	ILType_IsMethodParameter(type)	\
				((type) != 0 && ILType_IsComplex((type)) && \
				 ((type)->kind__ & 0xFF) == IL_TYPE_COMPLEX_MVAR)

/*
 * Get the kind that is associated with a complex type.
 */
#define	ILType_Kind(type)		((type)->kind__ & 0xFF)

/*
 * Get the referenced type associated with a complex type.
 */
#define	ILType_Ref(type)		((type)->un.refType__)

/*
 * Get the calling conventions associated with a complex type.
 */
#define	ILType_CallConv(type)	((ILUInt32)((type)->kind__ >> 8))

/*
 * Get the element type of an array, but only go down a single level.
 * Use "ILTypeGetElemType" to get the "real" element type.
 */
#define	ILType_ElemType(type)	((type)->un.array__.elemType__)

/*
 * Get the size of an array dimension.
 */
#define	ILType_Size(type)		((type)->un.array__.size__)

/*
 * Get the lower bound of an array dimension.
 */
#define	ILType_LowBound(type)	((type)->un.array__.lowBound__)

/*
 * Get the generic variable number from a type.
 */
#define	ILType_VarNum(type)		((type)->un.num__)

/*
 * Get or set the number of generic parameters for a method signature.
 */
#define	ILType_NumGen(type)		((type)->un.method__.gparams__)
#define	ILType_SetNumGen(type,num)	\
			((type)->un.method__.gparams__ = (ILUInt32)(num))

/*
 * Create a reference type.  Returns NULL if out of memory.
 */
ILType *ILTypeCreateRef(ILContext *context, int kind, ILType *refType);

/*
 * Import a type into an image. Returns NULL if out of memory.
 */
ILType * ILTypeImport(ILImage *image, ILType *type);

/*
 * Create an array type.  Returns NULL if out of memory.
 */
ILType *ILTypeCreateArray(ILContext *context, unsigned long rank,
						  ILType *elem);

/*
 * Create an array type after first checking for a matching synthetic
 * class.  This is more memory efficient than "ILTypeCreateArray",
 * but can only be used when "elem" won't change.  Returns NULL if
 * out of memory.
 */
ILType *ILTypeFindOrCreateArray(ILContext *context, unsigned long rank,
						        ILType *elem);

/*
 * Set the size for a particular array dimension.
 */
void ILTypeSetSize(ILType *array, unsigned long dimension, long value);

/*
 * Set the low bound for a particular array dimension.
 */
void ILTypeSetLowBound(ILType *array, unsigned long dimension, long value);

/*
 * Create a modifier and add it to a list.  Returns the new list,
 * or NULL if out of memory.
 */
ILType *ILTypeCreateModifier(ILContext *context, ILType *list,
							 int kind, ILClass *info);

/*
 * Add a list of modifiers to a type and return the modified type.
 */
ILType *ILTypeAddModifiers(ILContext *context, ILType *modifiers,
						   ILType *type);

/*
 * Create a local variable list.  Returns NULL if out of memory.
 */
ILType *ILTypeCreateLocalList(ILContext *context);

/*
 * Add a type to a local variable list.  Returns zero if out of memory.
 */
int ILTypeAddLocal(ILContext *context, ILType *locals, ILType *type);

/*
 * Get the number of locals within a local variable list.
 */
unsigned long ILTypeNumLocals(ILType *locals);

/*
 * Get the type for a specific local within a local variable list.
 */
ILType *ILTypeGetLocal(ILType *locals, unsigned long index);

/*
 * Get the type for a specific local, with modifier prefixes.
 */
ILType *ILTypeGetLocalWithPrefixes(ILType *locals, unsigned long index);

/*
 * Create a method type with a specific return type.
 * Returns NULL if out of memory.
 */
ILType *ILTypeCreateMethod(ILContext *context, ILType *returnType);

/*
 * Create a property type with a specific type.
 * Returns NULL if out of memory.
 */
ILType *ILTypeCreateProperty(ILContext *context, ILType *propType);

/*
 * Add a parameter to a method or property type.  Returns
 * zero if out of memory.
 */
int ILTypeAddParam(ILContext *context, ILType *method, ILType *paramType);

/*
 * Add a varargs sentinel to a method type.  Returns zero
 * if out of memory.
 */
int ILTypeAddSentinel(ILContext *context, ILType *method);

/*
 * Get the number of parameters associated with a method or
 * property type.
 */
unsigned long ILTypeNumParams(ILType *method);

/*
 * Get a specific parameter from a method or property type.
 * The index 0 indicates the return type.  Returns NULL if
 * the parameter index is invalid.
 */
ILType *ILTypeGetParam(ILType *method, unsigned long index);

/*
 * Get a specific parameter, including its modifier prefixes.
 */
ILType *ILTypeGetParamWithPrefixes(ILType *method, unsigned long index);

/*
 * Set the calling conventions for a method or property type.
 */
void ILTypeSetCallConv(ILType *type, ILUInt32 callConv);

/*
 * Set the return type for a method or property type.
 */
void ILTypeSetReturn(ILType *type, ILType *retType);

/*
 * Get the return type for a method or property type.
 */
ILType *ILTypeGetReturn(ILType *type);

/*
 * Get the return type for a method or property type,
 * including its modifier prefixes.
 */
ILType *ILTypeGetReturnWithPrefixes(ILType *type);

/*
 * Parse a type from a MethodDefSig within the signature blob.
 * Returns NULL if the signature is invalid, or out of memory.
 */
ILType *ILTypeFromMethodDefSig(ILContext *context, ILImage *image,
							   unsigned long blobStart, unsigned long blobLen);

/*
 * Parse a type from a MethodRefSig within the signature blob.
 * Returns NULL if the signature is invalid, or out of memory.
 */
ILType *ILTypeFromMethodRefSig(ILContext *context, ILImage *image,
							   unsigned long blobStart, unsigned long blobLen);

/*
 * Parse a type from a StandAloneMethodSig within the signature blob.
 * Returns NULL if the signature is invalid, or out of memory.
 */
ILType *ILTypeFromStandAloneMethodSig(ILContext *context, ILImage *image,
							   		  unsigned long blobStart,
									  unsigned long blobLen);

/*
 * Parse a type from a FieldSig within the signature blob.
 * Returns NULL if the signature is invalid, or out of memory.
 */
ILType *ILTypeFromFieldSig(ILContext *context, ILImage *image,
						   unsigned long blobStart, unsigned long blobLen);

/*
 * Parse a type from a PropertySig within the signature blob.
 * Returns NULL if the signature is invalid, or out of memory.
 */
ILType *ILTypeFromPropertySig(ILContext *context, ILImage *image,
							  unsigned long blobStart, unsigned long blobLen);

/*
 * Parse a type from a TypeSpec within the signature blob.
 * Returns NULL if the signature is invalid, or out of memory.
 */
ILType *ILTypeFromTypeSpec(ILContext *context, ILImage *image,
						   unsigned long blobStart, unsigned long blobLen);

/*
 * Parse a list of locals from within the signature block.
 * Returns NULL if the list is invalid, or out of memory.
 */
ILType *ILTypeFromLocalVarSig(ILImage *image, unsigned long blobOffset);

/*
 * Strip prefixes from a type to get to the actual type.
 */
ILType *ILTypeStripPrefixes(ILType *type);

/*
 * Determine if two types are identical after stripping prefixes.
 */
int ILTypeIdentical(ILType *type1, ILType *type2);

/*
 * Get the name form of a type.  The return value should
 * be free'd with "ILFree".
 */
char *ILTypeToName(ILType *type);

/*
 * Get the underlying type for an enumerated type.
 * If the type is not enumerated, then return as-is.
 */
ILType *ILTypeGetEnumType(ILType *type);

/*
 * Get the element type for an array.
 */
ILType *ILTypeGetElemType(ILType *type);

/*
 * Get the rank of an element.
 */
int ILTypeGetRank(ILType *type);

/*
 * Convert a type into a blob offset for the encoded form
 * of a method signature.  Returns zero if out of memory.
 */
unsigned long ILTypeToMethodSig(ILImage *image, ILType *type);

/*
 * Convert a type into a blob offset for the encoded form
 * of a field signature.  Returns zero if out of memory.
 */
unsigned long ILTypeToFieldSig(ILImage *image, ILType *type);

/*
 * Convert a type into a blob offset for the encoded form of
 * some other kind of signature.  Returns zero if out of memory.
 */
unsigned long ILTypeToOtherSig(ILImage *image, ILType *type);

/*
 * Determine if a type is "System.String".
 */
int ILTypeIsStringClass(ILType *type);

/*
 * Determine if a type is "System.Object".
 */
int ILTypeIsObjectClass(ILType *type);

/*
 * Determine if a type is a "system/corlib" class.
 */
int ILTypeIsSystemClass(ILType *type, const char *namespace, const char *name);

/*
 * Determine if two types are assignment-compatible within
 * the context of a particular image when assigning a value
 * of type "src" to a location of type "dest".  If "src" is
 * NULL, it indicates an assignment of "null" to "dest".
 */
int ILTypeAssignCompatible(ILImage *image, ILType *src, ILType *dest);

/*
 * Determine if two types are assignment-compatible without
 * performing boxing conversions.
 */
int ILTypeAssignCompatibleNonBoxing(ILImage *image, ILType *src, ILType *dest);

/*
 * Determine if a type contains a particular system modifier.
 */
int ILTypeHasModifier(ILType *type, ILClass *classInfo);

/*
 * Determine if a type is a reference type.
 */
int ILTypeIsReference(ILType *type);

/*
 * Determine if a type is an enumerated type.
 */
int ILTypeIsEnum(ILType *type);

/*
 * Determine if a type is a value type (including primitive numeric types).
 */
int ILTypeIsValue(ILType *type);

/*
 * Determine if a type is a delegate class with an "Invoke" method.
 */
int ILTypeIsDelegate(ILType *type);

/*
 * Get the "Invoke" method associated with a delegate type.
 * Returns NULL if not a delegate type.
 */
void *ILTypeGetDelegateMethod(ILType *type);

/*
 * Get the "BeginInvoke" method associated with a delegate type.
 * Returns NULL if not a delegate type.
 */
void *ILTypeGetDelegateBeginInvokeMethod(ILType *type);

/*
 * Get the "EndInvoke" method associated with a delegate type.
 * Returns NULL if not a delegate type.
 */
void *ILTypeGetDelegateEndInvokeMethod(ILType *type);

/*
 * Determine if we have a signature match between a delegate
 * type and a particular method.
 */
int ILTypeDelegateSignatureMatch(ILType *type, void *method);

/*
 * Determine if a type is "System.Delegate" or a subclass.
 */
int ILTypeIsDelegateSubClass(ILType *type);

/*
 * Create generic variable reference.  Returns NULL if out of memory.
 */
ILType *ILTypeCreateVarNum(ILContext *context, int kind, int num);

/*
 * Create a "with" generic type reference.  Returns NULL if out of memory.
 */
ILType *ILTypeCreateWith(ILContext *context, ILType *mainType);

/*
 * Add a parameter to a "with" generic type reference.  Returns zero
 * if out of memory.
 */
int ILTypeAddWithParam(ILContext *context, ILType *type, ILType *paramType);

/*
 * Get the number of "with" parameters on a generic type reference.
 */
unsigned long ILTypeNumWithParams(ILType *type);

/*
 * Get a specific "with" parameter.  1 is the first parameter.
 */
ILType *ILTypeGetWithParam(ILType *type, unsigned long num);

/*
 * Get a specific "with" parameter with prefixes.
 */
ILType *ILTypeGetWithParamWithPrefixes(ILType *type, unsigned long num);

/*
 * Get the main "with" type for a generic type reference.
 */
ILType *ILTypeGetWithMain(ILType *type);

/*
 * Get the main "with" type with prefixes.
 */
ILType *ILTypeGetWithMainWithPrefixes(ILType *type);

/*
 * Set the main "with" type for a generic reference 
 */
void ILTypeSetWithMain(ILType *type, ILType *mainType);

/*
 * Determine if a type needs to be instantiated because it involves
 * generic parameter references.
 */
int ILTypeNeedsInstantiation(ILType *type);

/*
 * Instantiate the generic parameters in a type.  "classParams" should be
 * a "with" type, and "methodParams" should be a method spec instantiation.
 * Returns NULL if out of memory.
 */
ILType *ILTypeInstantiate(ILContext *context, ILType *type,
						  ILType *classParams, ILType *methodParams);

/*
 * Returns a hash code value for the given type.
 */
unsigned long ILTypeHash(ILType *type);

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_TYPES_H */
