/*
 * pinvoke.c - Handle PInvoke and "internalcall" methods within the engine.
 *
 * Copyright (C) 2001, 2008, 2011  Southern Storm Software, Pty Ltd.
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

#include "engine.h"
#include "lib_defs.h"
#if defined(IL_WIN32_PLATFORM)
#include <objbase.h>
#endif

#if defined(HAVE_LIBFFI)

#include "ffi.h"

/* Imported from "cvmc_setup.c" */
int _ILCVMCanUseRawCalls(ILMethod *method, int isInternal);

/*
 * The hard work is done using "libffi", which is a generic library
 * for packing up the arguments for an arbitrary function call,
 * making the call, and then getting the return value.
 *
 * The main difference between "PInvoke" and "internalcall" is the
 * calling conventions.  All "internalcall" methods are passed an
 * extra first parameter which points to the "ILExecThread" that
 * the method was invoked within.  This is necessary because many
 * "internalcall" functions deal with runtime data structures.
 *
 * PInvoke methods are assumed to be external to the runtime engine
 * and so don't need "ILExecThread" passed to them.
 *
 * Further information on how this works can be found in "doc/pinvoke.html".
 */

#ifdef	__cplusplus
extern	"C" {
#endif

#if 0
/* Blindly freeing return values is extremely dangerous.  Will fix this
   problem when PInvoke is rewritten from scratch -- Rhys */
#if defined(IL_WIN32_PLATFORM)
	/* MS.NET frees native strings using the COM allocator 
	http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpconmemorymanagement.asp
	*/
	#define ILFreeNativeString(str) CoTaskMemFree(str)
#else
	/* Use the C allocator for other platforms */
	#define ILFreeNativeString(str) free(str)
#endif
#else
	#define ILFreeNativeString(str) do { ; } while (0)
#endif

/*
 * Structure type for passing typed references on the stack.
 */
static ffi_type *typedref_members[] =
{
	&ffi_type_pointer,
	&ffi_type_pointer,
	0
};
static ffi_type ffi_type_typedref =
{
	0,					/* size */
	0,					/* alignment */
	FFI_TYPE_STRUCT,	/* type */
	typedref_members,	/* elements */
};

/*
 * Forward declarations.
 */
static ffi_type *TypeToFFI(ILExecProcess *process, ILType *type, int isInternal);

#ifdef IL_CONFIG_PINVOKE

static ffi_type *StructToFFI(ILExecProcess *process, ILClass *classInfo);

#if !FFI_NO_STRUCTS

/*
 * Populate a list of "ffi" type descriptors with information
 * about the non-static fields of a class.  Returns zero if
 * out of memory.
 */
static int PopulateStructFFI(ILExecProcess *process, ILClass *classInfo,
							  ffi_type **fieldTypes, unsigned *posn)
{
	ILClass *parent;
	ILField *field;
	ILType *type;
	ffi_type *ffi;

	/* Process the parent class first */
	parent = ILClass_ParentClass(classInfo);
	if(parent)
	{
		if(!PopulateStructFFI(process, parent, fieldTypes, posn))
		{
			return 0;
		}
	}

	/* Process the non-static fields in this class */
	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
			(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!ILField_IsStatic(field))
		{
			type = ILTypeGetEnumType(ILField_Type(field));
			if(ILType_IsValueType(type))
			{
				/* Process an embedded structure type */
				ffi = StructToFFI(process, ILType_ToValueType(type));
				if(!ffi)
				{
					return 0;
				}
			}
			else
			{
				/* Process a non-structure type */
				ffi = TypeToFFI(process, type, 0);
			}
			fieldTypes[(*posn)++] = ffi;
		}
	}

	/* Done */
	return 1;
}

#endif /* !FFI_NO_STRUCTS */

/*
 * Convert a "struct" class into a "ffi" type descriptor.
 * Returns zero if out of memory.
 */
static ffi_type *StructToFFI(ILExecProcess *process, ILClass *classInfo)
{
#if !FFI_NO_STRUCTS
	ILClass *current;
	ILField *field;
	unsigned numFields;
	ffi_type *descr;
	ffi_type **fieldTypes;
	ILUInt32 explicitSize;
	ILUInt32 explicitAlignment;

	/* Count the number of non-static fields in the class */
	numFields = 0;
	if(!ILClass_IsExplicitLayout(classInfo))
	{
		current = classInfo;
		while(current != 0)
		{
			field = 0;
			while((field = (ILField *)ILClassNextMemberByKind
				(current, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
			{
				if(!ILField_IsStatic(field))
				{
					++numFields;
				}
			}
			current = ILClass_ParentClass(current);
		}
		explicitSize = 0;
		explicitAlignment = 0;
	}
	else
	{
		/* Use the explicit layout information from "layout.c" */
		explicitSize = _ILLayoutClassReturn(process, classInfo, &explicitAlignment);
	}

	/* Allocate space for the struct's type descriptor */
	descr = (ffi_type *)ILMalloc(sizeof(ffi_type) +
								 sizeof(ffi_type *) * (numFields + 1));
	if(!descr)
	{
		return 0;
	}
	fieldTypes = (ffi_type **)(descr + 1);

	/* Initialize the main descriptor's fields */
	descr->size = (size_t)explicitSize;
	descr->alignment = (unsigned short)explicitAlignment;
	descr->type = FFI_TYPE_STRUCT;
	descr->elements = fieldTypes;

	/* Populate the "fieldTypes" table with the "ffi" type descriptors */
	fieldTypes[numFields] = 0;
	if(!ILClass_IsExplicitLayout(classInfo))
	{
		numFields = 0;
		if(!PopulateStructFFI(process, classInfo, fieldTypes, &numFields))
		{
			ILFree(descr);
			return 0;
		}
	}

	/* Return the descriptor to the caller */
	return descr;
#else
	char *name = ILTypeToName(ILType_FromValueType(classInfo));
	if(name)
	{
		fprintf(stderr, "Cannot pass structures of type `%s' by value to "
						"PInvoke methods\n", name);
		ILFree(name);
	}
	return 0;
#endif /* !FFI_NO_STRUCTS */
}

#endif /* IL_CONFIG_PINVOKE */

/*
 * Convert an IL type into an "ffi" type descriptor.
 */
static ffi_type *TypeToFFI(ILExecProcess *process, ILType *type, int isInternal)
{
	if(ILType_IsPrimitive(type))
	{
		/* Determine how to marshal a primitive type */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:		return &ffi_type_void;
			case IL_META_ELEMTYPE_BOOLEAN:	return &ffi_type_sint8;
			case IL_META_ELEMTYPE_I1:		return &ffi_type_sint8;
			case IL_META_ELEMTYPE_U1:		return &ffi_type_uint8;
			case IL_META_ELEMTYPE_I2:		return &ffi_type_sint16;
			case IL_META_ELEMTYPE_U2:		return &ffi_type_uint16;
			case IL_META_ELEMTYPE_CHAR:		return &ffi_type_uint16;
			case IL_META_ELEMTYPE_I4:		return &ffi_type_sint32;
			case IL_META_ELEMTYPE_U4:		return &ffi_type_uint32;
		#ifdef IL_NATIVE_INT32
			case IL_META_ELEMTYPE_I:		return &ffi_type_sint32;
			case IL_META_ELEMTYPE_U:		return &ffi_type_uint32;
		#else
			case IL_META_ELEMTYPE_I:		return &ffi_type_sint64;
			case IL_META_ELEMTYPE_U:		return &ffi_type_uint64;
		#endif
			case IL_META_ELEMTYPE_I8:		return &ffi_type_sint64;
			case IL_META_ELEMTYPE_U8:		return &ffi_type_uint64;
			case IL_META_ELEMTYPE_R4:		return &ffi_type_float;
			case IL_META_ELEMTYPE_R8:		return &ffi_type_double;
		#ifdef IL_NATIVE_FLOAT
			case IL_META_ELEMTYPE_R:		return &ffi_type_longdouble;
		#else
			case IL_META_ELEMTYPE_R:		return &ffi_type_double;
		#endif
			case IL_META_ELEMTYPE_TYPEDBYREF: return &ffi_type_typedref;
		}
		return &ffi_type_pointer;
	}
#ifdef IL_CONFIG_PINVOKE
	else if(!isInternal && ILType_IsValueType(type))
	{
		/* Structure that is passed by value to a PInvoke method */
		ffi_type *ffi = StructToFFI(process, ILClassResolve(ILType_ToValueType(type)));
		return (ffi ? ffi : &ffi_type_pointer);
	}
#endif
	else
	{
		/* Everything else is passed as a pointer */
		return &ffi_type_pointer;
	}
}

void *_ILMakeCifForMethod(ILExecProcess *process, ILMethod *method, int isInternal)
{
	ILType *signature = ILMethod_Signature(method);
	ILType *returnType = ILTypeGetEnumType(ILTypeGetReturn(signature));
	ILType *modReturnType = returnType;
	ILUInt32 numArgs;
	ILUInt32 numParams;
	ffi_cif *cif;
	ffi_type **args;
	ffi_type *rtype;
	ILUInt32 arg;
	ILUInt32 param;

	/* Determine the number of argument blocks that we need */
	numArgs = numParams = ILTypeNumParams(signature);
	if(ILType_HasThis(signature))
	{
		++numArgs;
	}
	if(isInternal)
	{
		/* This is an "internalcall" or "runtime" method
		   which needs an extra argument for the thread */
		++numArgs;
		if(ILType_IsValueType(returnType))
		{
			/* We need an extra argument to pass a pointer to
			   the buffer to use to return the result */
			++numArgs;
			modReturnType = ILType_Void;
		}
	}

	/* Allocate space for the cif */
	cif = (ffi_cif *)ILMalloc(sizeof(ffi_cif) +
							  sizeof(ffi_type *) * numArgs);
	if(!cif)
	{
		return 0;
	}
	args = ((ffi_type **)(cif + 1));

	/* Convert the return type */
	rtype = TypeToFFI(process, modReturnType, isInternal);

	/* Convert the argument types */
	arg = 0;
	if(isInternal)
	{
		/* Pointer argument for the thread */
		args[arg++] = &ffi_type_pointer;
		if(ILType_IsValueType(returnType))
		{
			/* Pointer argument for value type returns */
			args[arg++] = &ffi_type_pointer;
		}
	}
	if(ILType_HasThis(signature))
	{
		/* Pointer argument for "this" */
		args[arg++] = &ffi_type_pointer;
	}
	for(param = 1; param <= numParams; ++param)
	{
		args[arg++] = TypeToFFI(process, ILTypeGetEnumType
									(ILTypeGetParam(signature, param)),
							    isInternal);
	}

	/* Limit the number of arguments if we cannot use raw mode */
	if(!_ILCVMCanUseRawCalls(method, isInternal) &&
	   numArgs > (CVM_MAX_NATIVE_ARGS + 1))
	{
		numArgs = CVM_MAX_NATIVE_ARGS + 1;
	}

	/* Prepare the "ffi_cif" structure for the call */
	if(ffi_prep_cif(cif, FFI_DEFAULT_ABI, numArgs, rtype, args) != FFI_OK)
	{
		fprintf(stderr, "Cannot marshal a type in the definition of %s::%s\n",
				ILClass_Name(ILMethod_Owner(method)), ILMethod_Name(method));
		return 0;
	}

	/* Ready to go */
	return (void *)cif;
}

void *_ILMakeCifForConstructor(ILExecProcess *process, ILMethod *method, int isInternal)
{
	ILType *signature = ILMethod_Signature(method);
	ILUInt32 numArgs;
	ILUInt32 numParams;
	ffi_cif *cif;
	ffi_type **args;
	ffi_type *rtype;
	ILUInt32 arg;
	ILUInt32 param;

	/* Determine the number of argument blocks that we need */
	numArgs = numParams = ILTypeNumParams(signature);
	if(isInternal)
	{
		/* This is an "internalcall" or "runtime" method
		   which needs an extra argument for the thread */
		++numArgs;
	}

	/* Allocate space for the cif */
	cif = (ffi_cif *)ILMalloc(sizeof(ffi_cif) +
							  sizeof(ffi_type *) * numArgs);
	if(!cif)
	{
		return 0;
	}
	args = ((ffi_type **)(cif + 1));

	/* The return value is always a pointer, indicating the object
	   that was just allocated by the constructor */
	rtype = &ffi_type_pointer;

	/* Convert the argument types */
	arg = 0;
	if(isInternal)
	{
		/* Pointer argument for the thread */
		args[arg++] = &ffi_type_pointer;
	}
	for(param = 1; param <= numParams; ++param)
	{
		args[arg++] = TypeToFFI(process, ILTypeGetEnumType
									(ILTypeGetParam(signature, param)),
							    isInternal);
	}

	/* Limit the number of arguments if we cannot use raw mode */
	if(!_ILCVMCanUseRawCalls(method, isInternal) &&
	   numArgs > (CVM_MAX_NATIVE_ARGS + 1))
	{
		numArgs = CVM_MAX_NATIVE_ARGS + 1;
	}

	/* Prepare the "ffi_cif" structure for the call */
	if(ffi_prep_cif(cif, FFI_DEFAULT_ABI, numArgs, rtype, args) != FFI_OK)
	{
		fprintf(stderr, "Cannot marshal a type in the definition of %s::%s\n",
				ILClass_Name(ILMethod_Owner(method)), ILMethod_Name(method));
		return 0;
	}

	/* Ready to go */
	return (void *)cif;
}

#if FFI_CLOSURES

/*
 * Check that we have enough space to push an argument.
 */
#define	CHECK_SPACE(nwords)	\
			do { \
				if((stacktop + (nwords)) > stacklimit) \
				{ \
					_ILExecThreadSetException(thread, _ILSystemException \
						(thread, "System.StackOverflowException")); \
					return 1; \
				} \
			} while (0)

/*
 * User data for "PackDelegateParams".
 */
typedef struct
{
	void     **args;
	ILMethod  *pinvokeInfo;
	int		   needThis;

} PackDelegateUserData;

/*
 * Pack the parameters for a delegate closure call onto the CVM stack.
 */
static int PackDelegateParams(ILExecThread *thread, ILMethod *method,
					          int isCtor, void *_this, void *userData)
{
	void **args = ((PackDelegateUserData *)userData)->args;
	ILMethod *pinvokeInfo = ((PackDelegateUserData *)userData)->pinvokeInfo;
	ILType *signature = ILMethod_Signature(method);
	CVMWord *stacktop, *stacklimit;
	ILUInt32 param, numParams;
	ILType *paramType;
	void *ptr;
	ILUInt32 size, sizeInWords;
	ILNativeFloat tempFloat;
	ILUInt32 marshalType;
	char *customName;
	int customNameLen;
	char *customCookie;
	int customCookieLen;
	char *strValue;

	/* Get the top and extent of the stack */
	stacktop = thread->stackTop;
	stacklimit = thread->stackLimit;

	/* Push the arguments onto the evaluation stack */
	if(ILType_HasThis(signature))
	{
		/* Push the "this" argument */
		CHECK_SPACE(1);
		if(((PackDelegateUserData *)userData)->needThis)
		{
			/* We get the "this" value from the incoming arguments */
			stacktop->ptrValue = *((void **)(*args));
			++args;
		}
		else
		{
			/* We get the "this" value from the delegate object */
			stacktop->ptrValue = _this;
		}
		++stacktop;
	}
	numParams = ILTypeNumParams(signature);
	for(param = 1; param <= numParams; ++param)
	{
		/* Marshal parameters that need special handling */
		marshalType = ILPInvokeGetMarshalType(0, pinvokeInfo, param,
											  &customName, &customNameLen,
											  &customCookie, &customCookieLen,
											  ILTypeGetParam(signature, param));
		if(marshalType != IL_META_MARSHAL_DIRECT)
		{
			switch(marshalType)
			{
				case IL_META_MARSHAL_ANSI_STRING:
				{
					/* Marshal an ANSI string from the native world */
					CHECK_SPACE(1);
					strValue = *((char **)(*args));
					if(strValue)
					{
						stacktop->ptrValue = ILStringCreate(thread, strValue);

						/* Free the native string */
						ILFreeNativeString(strValue);

						if(!(stacktop->ptrValue))
						{
							return 1;
						}
					}
					else
					{
						stacktop->ptrValue = 0;
					}
					++args;
					++stacktop;
				}
				continue;

				case IL_META_MARSHAL_UTF8_STRING:
				{
					/* Marshal a UTF-8 string from the native world */
					CHECK_SPACE(1);
					strValue = *((char **)(*args));
					if(strValue)
					{
						stacktop->ptrValue =
							ILStringCreateUTF8(thread, strValue);

						/* Free the native string */
						ILFreeNativeString(strValue);

						if(!(stacktop->ptrValue))
						{
							return 1;
						}
					}
					else
					{
						stacktop->ptrValue = 0;
					}
					++args;
					++stacktop;
				}
				continue;

				case IL_META_MARSHAL_UTF16_STRING:
				{
					/* Marshal a UTF-16 string from the native world */
					CHECK_SPACE(1);
					strValue = *((char **)(*args));
					if(strValue)
					{
						stacktop->ptrValue =
							ILStringWCreate(thread, (ILUInt16 *)strValue);

						/* Free the native string */
						ILFreeNativeString(strValue);
					
						if(!(stacktop->ptrValue))
						{
							return 1;
						}
					}
					else
					{
						stacktop->ptrValue = 0;
					}
					++args;
					++stacktop;
				}
				continue;

				case IL_META_MARSHAL_CUSTOM:
				{
					/* Marshal a custom value from the native world */
					CHECK_SPACE(1);
					stacktop->ptrValue = _ILCustomToObject
						(thread, *((void **)(*args)),
						 customName, customNameLen,
						 customCookie, customCookieLen);
					if(_ILExecThreadHasException(thread))
					{
						return 1;
					}
					++args;
					++stacktop;
				}
				continue;
			}
		}

		/* Marshal the parameter directly */
		paramType = ILTypeGetEnumType(ILTypeGetParam(signature, param));
		if(ILType_IsPrimitive(paramType))
		{
			/* Process a primitive value */
			switch(ILType_ToElement(paramType))
			{
				case IL_META_ELEMTYPE_VOID:		break;

				case IL_META_ELEMTYPE_BOOLEAN:
				case IL_META_ELEMTYPE_I1:
				case IL_META_ELEMTYPE_U1:
				case IL_META_ELEMTYPE_I2:
				case IL_META_ELEMTYPE_U2:
				case IL_META_ELEMTYPE_CHAR:
				case IL_META_ELEMTYPE_I4:
				case IL_META_ELEMTYPE_U4:
			#ifdef IL_NATIVE_INT32
				case IL_META_ELEMTYPE_I:
				case IL_META_ELEMTYPE_U:
			#endif
				{
					CHECK_SPACE(1);
					stacktop->intValue = *((ILInt32 *)(*args));
					++args;
					++stacktop;
				}
				break;

				case IL_META_ELEMTYPE_I8:
				case IL_META_ELEMTYPE_U8:
			#ifdef IL_NATIVE_INT64
				case IL_META_ELEMTYPE_I:
				case IL_META_ELEMTYPE_U:
			#endif
				{
					CHECK_SPACE(CVM_WORDS_PER_LONG);
					ILMemCpy(stacktop, *args, sizeof(ILInt64));
					++args;
					stacktop += CVM_WORDS_PER_LONG;
				}
				break;

				case IL_META_ELEMTYPE_R4:
				{
					CHECK_SPACE(CVM_WORDS_PER_NATIVE_FLOAT);
					tempFloat = (ILNativeFloat)(*((ILFloat *)(*args)));
					ILMemCpy(stacktop, &tempFloat, sizeof(ILNativeFloat));
					++args;
					stacktop += CVM_WORDS_PER_NATIVE_FLOAT;
				}
				break;

				case IL_META_ELEMTYPE_R8:
				{
					CHECK_SPACE(CVM_WORDS_PER_NATIVE_FLOAT);
					tempFloat = (ILNativeFloat)(*((ILDouble *)(*args)));
					ILMemCpy(stacktop, &tempFloat, sizeof(ILNativeFloat));
					++args;
					stacktop += CVM_WORDS_PER_NATIVE_FLOAT;
				}
				break;

				case IL_META_ELEMTYPE_R:
				{
					CHECK_SPACE(CVM_WORDS_PER_NATIVE_FLOAT);
					ILMemCpy(stacktop, *args, sizeof(ILNativeFloat));
					++args;
					stacktop += CVM_WORDS_PER_NATIVE_FLOAT;
				}
				break;

				case IL_META_ELEMTYPE_TYPEDBYREF:
				{
					CHECK_SPACE(CVM_WORDS_PER_TYPED_REF);
					ILMemCpy(stacktop, *args, sizeof(ILTypedRef));
					++args;
					stacktop += CVM_WORDS_PER_TYPED_REF;
				}
				break;
			}
		}
		else if(ILType_IsClass(paramType))
		{
			/* Process an object reference */
			CHECK_SPACE(1);
			stacktop->ptrValue = *((void **)(*args));
			++args;
			++stacktop;
		}
		else if(ILType_IsValueType(paramType))
		{
			/* Process a value type which was passed by value */
			ptr = *args;
			++args;
			size = ILSizeOfType(thread, paramType);
			sizeInWords = ((size + sizeof(CVMWord) - 1) / sizeof(CVMWord));
			CHECK_SPACE(sizeInWords);
			ILMemCpy(stacktop, ptr, size);
			stacktop += sizeInWords;
		}
		else if(paramType != 0 && ILType_IsComplex(paramType) &&
				(ILType_Kind(paramType) == IL_TYPE_COMPLEX_BYREF || 
				 ILType_Kind(paramType) == IL_TYPE_COMPLEX_PTR))
		{
			/* Process a value that is being passed by reference */
			CHECK_SPACE(1);
			stacktop->ptrValue = *((void **)(*args));
			++args;
			++stacktop;
		}
		else
		{
			/* Assume that everything else is an object reference */
			CHECK_SPACE(1);
			stacktop->ptrValue = *args;
			++args;
			++stacktop;
		}
	}

	/* Update the stack top */
	thread->stackTop = stacktop;
	return 0;
}

/*
 * Unpack the result of a delegate closure call.
 */
static void UnpackDelegateResult(ILExecThread *thread, ILMethod *method,
					             int isCtor, void *result, void *userData)
{
	ILMethod *pinvokeInfo = ((PackDelegateUserData *)userData)->pinvokeInfo;
	ILType *signature = ILMethod_Signature(method);
	ILType *paramType;
	ILUInt32 size, sizeInWords;
	ILNativeFloat tempFloat;
	ILUInt32 marshalType;
	char *customName;
	int customNameLen;
	char *customCookie;
	int customCookieLen;

	/* Marshal return types that need special handling */
	marshalType = ILPInvokeGetMarshalType
			(0, pinvokeInfo, 0, &customName, &customNameLen,
			 &customCookie, &customCookieLen, ILTypeGetReturn(signature));
	if(marshalType != IL_META_MARSHAL_DIRECT)
	{
		switch(marshalType)
		{
			case IL_META_MARSHAL_ANSI_STRING:
			{
				/* Marshal an ANSI string back to the native world */
				*((char **)result) = ILStringToAnsi
					(thread, (ILString *)(thread->stackTop[-1].ptrValue));
				--(thread->stackTop);
			}
			return;

			case IL_META_MARSHAL_UTF8_STRING:
			{
				/* Marshal a UTF-8 string back to the native world */
				*((char **)result) = ILStringToUTF8
					(thread, (ILString *)(thread->stackTop[-1].ptrValue));
				--(thread->stackTop);
			}
			return;

			case IL_META_MARSHAL_UTF16_STRING:
			{
				/* Marshal a UTF-16 string back to the native world */
				*((ILUInt16 **)result) = ILStringToUTF16
					(thread, (ILString *)(thread->stackTop[-1].ptrValue));
				--(thread->stackTop);
			}
			return;

			case IL_META_MARSHAL_FNPTR:
			{
				/* Convert a delegate into a function closure pointer */
				*((void **)result) = _ILDelegateGetClosure
					(thread, (ILObject *)(thread->stackTop[-1].ptrValue));
				--(thread->stackTop);
			}
			return;

			case IL_META_MARSHAL_ARRAY:
			{
				/* Convert an array into a pointer to its first member */
				void *array = thread->stackTop[-1].ptrValue;
				--(thread->stackTop);
				if(array)
				{
					*((void **)result) = ArrayToBuffer(array);
				}
				else
				{
					*((void **)result) = 0;
				}
			}
			return;

			case IL_META_MARSHAL_CUSTOM:
			{
				/* Marshal a custom value to the native world */
				*((void **)result) = _ILObjectToCustom
					(thread, (ILObject *)(thread->stackTop[-1].ptrValue),
					 customName, customNameLen, customCookie, customCookieLen);
				--(thread->stackTop);
			}
			return;
		}
	}

	/* Copy the return value into place */
	paramType = ILTypeGetEnumType(ILTypeGetReturn(signature));
	if(ILType_IsPrimitive(paramType))
	{
		/* Process a primitive value */
		switch(ILType_ToElement(paramType))
		{
			case IL_META_ELEMTYPE_VOID:		break;

			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
		#ifdef IL_NATIVE_INT32
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				*((ILInt32 *)result) = thread->stackTop[-1].intValue;
				--(thread->stackTop);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
		#ifdef IL_NATIVE_INT64
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
			{
				ILMemCpy(result,
						 thread->stackTop - CVM_WORDS_PER_LONG,
						 sizeof(ILInt64));
				thread->stackTop -= CVM_WORDS_PER_LONG;
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				ILMemCpy(&tempFloat,
						 thread->stackTop - CVM_WORDS_PER_NATIVE_FLOAT,
						 sizeof(ILNativeFloat));
				*((ILFloat *)result) = (ILFloat)tempFloat;
				thread->stackTop -= CVM_WORDS_PER_NATIVE_FLOAT;
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				ILMemCpy(&tempFloat,
						 thread->stackTop - CVM_WORDS_PER_NATIVE_FLOAT,
						 sizeof(ILNativeFloat));
				*((ILDouble *)result) = (ILDouble)tempFloat;
				thread->stackTop -= CVM_WORDS_PER_NATIVE_FLOAT;
			}
			break;

			case IL_META_ELEMTYPE_R:
			{
				ILMemCpy(result,
						 thread->stackTop - CVM_WORDS_PER_NATIVE_FLOAT,
						 sizeof(ILNativeFloat));
				thread->stackTop -= CVM_WORDS_PER_NATIVE_FLOAT;
			}
			break;

			case IL_META_ELEMTYPE_TYPEDBYREF:
			{
				ILMemCpy(result,
						 thread->stackTop - CVM_WORDS_PER_TYPED_REF,
						 sizeof(ILTypedRef));
				thread->stackTop -= CVM_WORDS_PER_TYPED_REF;
			}
			break;
		}
	}
	else if(ILType_IsClass(paramType))
	{
		/* Process an object reference */
		*((void **)result) = thread->stackTop[-1].ptrValue;
		--(thread->stackTop);
	}
	else if(ILType_IsValueType(paramType))
	{
		/* Process a value type */
		size = ILSizeOfType(thread, paramType);
		sizeInWords = ((size + sizeof(CVMWord) - 1) / sizeof(CVMWord));
		ILMemCpy(result, thread->stackTop - sizeInWords, size);
		thread->stackTop -= sizeInWords;
	}
	else
	{
		/* Assume that everything else is an object reference */
		*((void **)result) = thread->stackTop[-1].ptrValue;
		--(thread->stackTop);
	}
}

typedef struct {
	ILExecThread *thread;
	ffi_cif *cif;
	void *result;
	void **args;
	System_Delegate *delegate;
} ILDelegateInvokeParams;

/*
 * Do the real delegate invokation.
 */
static void _DelegateInvoke(ILDelegateInvokeParams *params)
{
	ILMethod *method;
	ILType *type;
	ILUInt32 size;
	PackDelegateUserData userData;

	/* If this is a multicast delegate, then execute "prev" first */
	if(params->delegate->prev)
	{
		ILDelegateInvokeParams prevParams;

		prevParams.thread = params->thread;
		prevParams.cif = params->cif;
		prevParams.result = params->result;
		prevParams.args = params->args;
		prevParams.delegate = (System_Delegate *)(params->delegate->prev);
;
		_DelegateInvoke(&prevParams);
		if(_ILExecThreadHasException(params->thread))
		{
			return;
		}
	}

	/* Extract the method from the delegate */
	method = params->delegate->methodInfo;
	if(!method)
	{
		ILExecThreadThrowSystem(params->thread, "System.MissingMethodException",
								(const char *)0);
		return;
	}

	/* Call the method */
	userData.args = params->args;
	userData.pinvokeInfo = (ILMethod *)ILTypeGetDelegateMethod
		(ILType_FromClass(GetObjectClass(params->delegate)));
	userData.needThis = 0;
	if(_ILCallMethod(params->thread, method,
				     UnpackDelegateResult, params->result,
				     0, params->delegate->target,
				     PackDelegateParams, &userData))
	{
		/* An exception occurred, which is already stored in the thread */
		type = ILMethod_Signature(method);
		type = ILTypeGetEnumType(ILTypeGetReturn(type));
		if(type != ILType_Void)
		{
			/* Clear the native return value, because we cannot assume
			   that the native caller knows how to handle exceptions */
			size = ILSizeOfType(params->thread, type);
			ILMemZero(params->result, size);
		}
	}
}

/*
 * Invoke the delegate from a new thread.
 */
static void *_DelegateInvokeFromNewThread(void *params)
{
	ILThread *thread = ILThreadSelf();
	ILClassPrivate *classPrivate = GetObjectClassPrivate(((ILDelegateInvokeParams *)params)->delegate);
	ILExecProcess *process = classPrivate->process;

	if((((ILDelegateInvokeParams *)params)->thread = ILThreadRegisterForManagedExecution(process, thread)))
	{
		_DelegateInvoke((ILDelegateInvokeParams *)params);
		ILThreadUnregisterForManagedExecution(thread);
	}
	return 0;
}

/*
 * Invoke a delegate from a closure.
 */
static void DelegateInvoke(ffi_cif *cif, void *result,
						   void **args, void *delegate)
{
	ILThread *thread = ILThreadSelf();
	ILClassPrivate *classPrivate = GetObjectClassPrivate(delegate);
	ILExecProcess *process = classPrivate->process;
	ILDelegateInvokeParams params;

	params.thread = 0;
	params.cif = cif;
	params.result = result;
	params.args = args;
	params.delegate = (System_Delegate *)delegate;
	if(!thread)
	{
		/* callback was invoked by a non pnet thread. */
		
		ILThreadRunSelf(_DelegateInvokeFromNewThread, (void *)&params);
	}
	else
	{
		params.thread = _ILExecThreadFromThread(thread);

		if(params.thread)
		{
			IL_BEGIN_EXECPROCESS_SWITCH(params.thread, process)
			_DelegateInvoke(&params);
			IL_END_EXECPROCESS_SWITCH(params.thread)
		}
		else
		{
			/* thread is not registerd for managed execution */
			if((params.thread = ILThreadRegisterForManagedExecution(process, thread)))
			{

				_DelegateInvoke(&params);
				ILThreadUnregisterForManagedExecution(thread);
			}
		}
	}
}

/*
 * Invoke a method from a closure.
 */
static void MethodInvoke(ffi_cif *cif, void *result,
						 void **args, void *_method)
{
	ILExecThread *thread = ILExecThreadCurrent();
	ILMethod *method = (ILMethod *)_method;
	ILType *type;
	ILUInt32 size;
	PackDelegateUserData userData;

	/* Call the method */
	userData.args = args;
	userData.pinvokeInfo = method;
	userData.needThis = 0;
	if(_ILCallMethod(thread, method,
				     UnpackDelegateResult, result, 0, 0,
				     PackDelegateParams, &userData))
	{
		/* An exception occurred, which is already stored in the thread */
		type = ILMethod_Signature(method);
		type = ILTypeGetEnumType(ILTypeGetReturn(type));
		if(type != ILType_Void)
		{
			/* Clear the native return value, because we cannot assume
			   that the native caller knows how to handle exceptions */
			size = ILSizeOfType(thread, type);
			ILMemZero(result, size);
		}
	}
}

#endif /* FFI_CLOSURES */

void *_ILMakeClosureForDelegate(ILExecProcess *process, ILObject *delegate, ILMethod *method)
{
#if FFI_CLOSURES
	ILType *signature = ILMethod_Signature(method);
	ILType *returnType = ILTypeGetEnumType(ILTypeGetReturn(signature));
	ILUInt32 numArgs;
	ffi_cif *cif;
	ffi_type **args;
	ffi_type *rtype;
	ILUInt32 arg;
	ILUInt32 param;
	ffi_closure *closure;
	void *closure_code = 0;

	/* Determine the number of argument blocks that we need */
	numArgs = ILTypeNumParams(signature);

	/* Allocate space for the cif */
	cif = (ffi_cif *)ILMalloc(sizeof(ffi_cif) +
							  sizeof(ffi_type *) * (numArgs + 1));
	if(!cif)
	{
		return 0;
	}
	args = ((ffi_type **)(cif + 1));

	/* Convert the return type */
	rtype = TypeToFFI(process, returnType, 0);

	/* Convert the argument types */
	arg = 0;
	if(!delegate && ILType_HasThis(signature))
	{
		/* We need an extra argument for the method's "this" value */
		args[arg++] = &ffi_type_pointer;
	}
	for(param = 1; param <= numArgs; ++param)
	{
		args[arg++] = TypeToFFI(process, ILTypeGetEnumType
									(ILTypeGetParam(signature, param)), 0);
	}

	/* Prepare the "ffi_cif" structure for the call */
	if(ffi_prep_cif(cif, FFI_DEFAULT_ABI, arg, rtype, args) != FFI_OK)
	{
		fprintf(stderr, "Cannot marshal a type in the definition of %s::%s\n",
				ILClass_Name(ILMethod_Owner(method)), ILMethod_Name(method));
		return 0;
	}

	/*
	 * Allocate space for the closure.
	 * TODO: free the closure if all references to the delegate vanished.
	 * NOTE: It's the responsibility of the application to hold a reference to
	 * the delegate as long as the closure is used by the native method.
	 * Not doing this means that delegates and the objects the delegates work
	 * on will never get collected.
	 */
	closure = (ffi_closure *)ffi_closure_alloc(sizeof(ffi_closure), &closure_code);
	if(!closure)
	{
		return 0;
	}

	/* Prepare the closure using the call parameters */
	if(delegate)
	{
		if(ffi_prep_closure_loc(closure, cif, DelegateInvoke, (void *)delegate, closure_code)
				!= FFI_OK)
		{
			fprintf(stderr, "Cannot create a closure for %s::%s\n",
				ILClass_Name(ILMethod_Owner(method)), ILMethod_Name(method));
			return 0;
		}
	}
	else
	{
		if(ffi_prep_closure_loc(closure, cif, MethodInvoke, (void *)method, closure_code)
				!= FFI_OK)
		{
			fprintf(stderr, "Cannot create a closure for %s::%s\n",
				ILClass_Name(ILMethod_Owner(method)), ILMethod_Name(method));
			return 0;
		}
	}

	/* The closure is ready to go */
	return closure_code ? closure_code : (void *)closure;
#else	/* !FFI_CLOSURES */
	/* libffi does not support closures */
	fprintf(stderr, "libffi does not support closures on this arch.\n");
	return 0;
#endif	/* !FFI_CLOSURES */
}

#ifdef	__cplusplus
};
#endif

#endif /* HAVE_LIBFFI */
