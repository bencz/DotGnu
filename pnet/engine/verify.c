/*
 * verify.c - Bytecode verifier for the engine.
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

#include "engine.h"
#include "coder.h"
#include "lib_defs.h"
#include "il_coder.h"
#include "il_opcodes.h"
#include "il_align.h"
#include "il_debug.h"
#include "debugger.h"
#ifdef IL_USE_JIT
#include "jitc.h"
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Temporary memory allocator.
 */
typedef struct
{
	char	buffer[256];
	int		posn;
	void   *overflow;

} TempAllocator;

/*
 * Allocate a block of zero'ed memory from a temporary memory allocator.
 */
static void *TempAllocate(TempAllocator *allocator, unsigned long size)
{
	void *ptr;

	/* Round the size to a multiple of the machine word size */
	size = (size + IL_BEST_ALIGNMENT - 1) & ~(IL_BEST_ALIGNMENT - 1);

	/* Can we allocate from the primary buffer? */
	if(size < 256 && (allocator->posn + (int)size) < 256)
	{
		ptr = (void *)(allocator->buffer + allocator->posn);
		allocator->posn += (int)size;
		return ptr;
	}

	/* Allocate from the system heap */
	ptr = ILCalloc(IL_BEST_ALIGNMENT + size, 1);
	if(!ptr)
	{
		return 0;
	}
	*((void **)ptr) = allocator->overflow;
	allocator->overflow = ptr;
	return (void *)(((unsigned char *)ptr) + IL_BEST_ALIGNMENT);
}

/*
 * Destroy a temporary memory allocator.
 */
static void TempAllocatorDestroy(TempAllocator *allocator)
{
	void *ptr, *nextPtr;
	ptr = allocator->overflow;
	while(ptr != 0)
	{
		nextPtr = *((void **)ptr);
		ILFree(ptr);
		ptr = nextPtr;
	}
}

/*
 * Number of bytes needed for a bit mask of a specific size.
 * 3 bits are allocated for each item: "instruction start",
 * "jump target", and "special jump target".
 */
#define	WORDS_FOR_MASK(size)	\
			(((size) * 3 + (sizeof(unsigned long) * 8) - 1) / \
					(sizeof(unsigned long) * 8))
#define	BYTES_FOR_MASK(size)	\
			(WORDS_FOR_MASK((size)) * sizeof(unsigned long))

/*
 * Mark an instruction starting point in a jump mask.
 */
static IL_INLINE void MarkInsnStart(unsigned long *jumpMask, ILUInt32 offset)
{
	offset *= 3;
	jumpMask[offset / (sizeof(unsigned long) * 8)] |=
		(((unsigned long)1) << (offset % (sizeof(unsigned long) * 8)));
}

/*
 * Mark a jump target point in a jump mask.
 */
static IL_INLINE void MarkJumpTarget(unsigned long *jumpMask, ILUInt32 offset)
{
	offset = offset * 3 + 1;
	jumpMask[offset / (sizeof(unsigned long) * 8)] |=
		(((unsigned long)1) << (offset % (sizeof(unsigned long) * 8)));
}

/*
 * Mark a special jump target point in a jump mask.
 */
static IL_INLINE void MarkSpecialJumpTarget(unsigned long *jumpMask,
											ILUInt32 offset)
{
	offset = offset * 3 + 2;
	jumpMask[offset / (sizeof(unsigned long) * 8)] |=
		(((unsigned long)1) << (offset % (sizeof(unsigned long) * 8)));
}

/*
 * Determine if a code offset is an instruction start.
 */
static IL_INLINE int IsInsnStart(unsigned long *jumpMask, ILUInt32 offset)
{
	offset *= 3;
	return ((jumpMask[offset / (sizeof(unsigned long) * 8)] &
		(((unsigned long)1) << (offset % (sizeof(unsigned long) * 8))))
				!= 0);
}

/*
 * Determine if a code offset is a jump target.
 */
static IL_INLINE int IsJumpTarget(unsigned long *jumpMask, ILUInt32 offset)
{
	offset = offset * 3 + 1;
	return ((jumpMask[offset / (sizeof(unsigned long) * 8)] &
		(((unsigned long)1) << (offset % (sizeof(unsigned long) * 8))))
				!= 0);
}

/*
 * Determine if a code offset is a special jump target.
 */
static IL_INLINE int IsSpecialJumpTarget(unsigned long *jumpMask,
										 ILUInt32 offset)
{
	offset = offset * 3 + 2;
	return ((jumpMask[offset / (sizeof(unsigned long) * 8)] &
		(((unsigned long)1) << (offset % (sizeof(unsigned long) * 8))))
				!= 0);
}

/*
 * Clear valid prefixes for an insn
 */
#define CLEAR_VALID_PREFIXES(prefixInfo, p, n) \
	do { \
		(prefixInfo).prefixFlags &= ~(p); \
		(prefixInfo).noFlags &= ~(n); \
	} while (0)

/*
 * Initialize the prefix members indicating valid values.
 */
#define INIT_PREFIXES(prefixInfo) \
	do { \
		(prefixInfo).prefixFlags = 0; \
		(prefixInfo).noFlags = 0; \
	} while (0)

/*
 * Valid prefixes for the different insns.
 */
#define VALID_LDIND_PREFIX		(IL_CODER_PREFIX_UNALIGNED | IL_CODER_PREFIX_VOLATILE)
#define VALID_STIND_PREFIX		(IL_CODER_PREFIX_UNALIGNED | IL_CODER_PREFIX_VOLATILE)
#define VALID_LDELEM_PREFIX		(IL_CODER_PREFIX_NO)
#define VALID_LDELEMA_PREFIX	(IL_CODER_PREFIX_NO | IL_PREFIX_OP_READONLY)
#define VALID_LDELEM_T_PREFIX	(IL_CODER_PREFIX_NO)
#define VALID_LDOBJ_PREFIX		(IL_CODER_PREFIX_UNALIGNED | IL_CODER_PREFIX_VOLATILE)
#define VALID_STELEM_PREFIX		(IL_CODER_PREFIX_NO)
#define VALID_STELEM_T_PREFIX	(IL_CODER_PREFIX_NO)
#define VALID_LDFLD_PREFIX		(IL_CODER_PREFIX_NO | IL_CODER_PREFIX_UNALIGNED | IL_CODER_PREFIX_VOLATILE)
#define VALID_LDSFLD_PREFIX		(IL_CODER_PREFIX_VOLATILE)
#define VALID_STFLD_PREFIX		(IL_CODER_PREFIX_NO | IL_CODER_PREFIX_UNALIGNED | IL_CODER_PREFIX_VOLATILE)
#define VALID_STSFLD_PREFIX		(IL_CODER_PREFIX_VOLATILE)
#define VALID_STOBJ_PREFIX		(IL_CODER_PREFIX_UNALIGNED | IL_CODER_PREFIX_VOLATILE)
#define VALID_CASTCLASS_PREFIX	(IL_CODER_PREFIX_NO)
#define VALID_CALL_PREFIX		(IL_PREFIX_OP_READONLY)
#define VALID_CALLVIRT_PREFIX	(IL_CODER_PREFIX_NO | IL_PREFIX_OP_READONLY | IL_CODER_PREFIX_CONSTRAINED)
#define VALID_CPBLK_PREFIX		(IL_CODER_PREFIX_UNALIGNED | IL_CODER_PREFIX_VOLATILE)
#define VALID_INITBLK_PREFIX	(IL_CODER_PREFIX_UNALIGNED | IL_CODER_PREFIX_VOLATILE)
#define VALID_LDVIRTFTN_PREFIX	(IL_CODER_PREFIX_NO)
#define VALID_UNBOX_PREFIX		(IL_CODER_PREFIX_NO)

/*
 * Valid no flags for the different insns.
 */
#define VALID_NO_NONE		(0)
#define VALID_NO_ALL		(IL_PREFIX_OP_NO_TYPECHECK | IL_PREFIX_OP_NO_RANGECHECK | IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_LDELEM		(IL_PREFIX_OP_NO_TYPECHECK | IL_PREFIX_OP_NO_RANGECHECK | IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_LDELEMA	(IL_PREFIX_OP_NO_TYPECHECK | IL_PREFIX_OP_NO_RANGECHECK | IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_LDELEM_T	(IL_PREFIX_OP_NO_RANGECHECK | IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_STELEM		(IL_PREFIX_OP_NO_TYPECHECK | IL_PREFIX_OP_NO_RANGECHECK | IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_STELEM_T	(IL_PREFIX_OP_NO_RANGECHECK | IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_LDFLD		(IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_STFLD		(IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_CASTCLASS	(IL_PREFIX_OP_NO_TYPECHECK)
#define VALID_NO_CALLVIRT	(IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_LDVIRTFTN	(IL_PREFIX_OP_NO_NULLCHECK)
#define VALID_NO_UNBOX		(IL_PREFIX_OP_NO_TYPECHECK)

/*
 * Convert a type into an engine type.
 */
static ILEngineType TypeToEngineType(ILType *type)
{
	type = ILTypeGetEnumType(type);
	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:		return ILEngineType_I4;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:		return ILEngineType_I8;

			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:		return ILEngineType_I;

			case IL_META_ELEMTYPE_R4:
			case IL_META_ELEMTYPE_R8:
			case IL_META_ELEMTYPE_R:		return ILEngineType_F;

			case IL_META_ELEMTYPE_TYPEDBYREF: return ILEngineType_TypedRef;
		}
		return ILEngineType_I4;
	}
	else if(ILType_IsValueType(type))
	{
		return ILEngineType_MV;
	}
	else if(ILType_IsComplex(type) && type != 0)
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_PTR:
			{
				/* Unsafe pointers are represented as native integers */
				return ILEngineType_I;
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_BYREF:
			{
				/* Reference values are managed pointers */
				return ILEngineType_M;
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_PINNED:
			{
				/* Pinned types are the same as their underlying type */
				return TypeToEngineType(ILType_Ref(type));
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_CMOD_REQD:
			case IL_TYPE_COMPLEX_CMOD_OPT:
			{
				/* Strip the modifier and inspect the underlying type */
				return TypeToEngineType(type->un.modifier__.type__);
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_METHOD:
			case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
			{
				/* Pass method pointers around the system as "I".  Higher
				   level code will also set the "typeInfo" field to reflect
				   the signature so that method pointers become verifiable */
				return ILEngineType_I;
			}
			/* Not reached */
		}
	}
	return ILEngineType_O;
}
ILEngineType _ILTypeToEngineType(ILType *type)
{
	return TypeToEngineType(type);
}

/*
 * Determine if a type is represented as an object reference.
 */
static int IsObjectRef(ILType *type)
{
	if(type == 0)
	{
		/* This is the "null" type, which is always an object reference */
		return 1;
	}
	else if(ILType_IsClass(type))
	{
		return 1;
	}
	else if(ILType_IsArray(type))
	{
		return 1;
	}
	else if(ILType_IsWith(type))
	{
		return IsObjectRef(ILTypeGetWithMain(type));
	}
	else
	{
		return 0;
	}
}

/*
 * Determine if a type contains unsafe pointers.
 */
static IL_INLINE int IsUnsafeType(ILType *type)
{
	if(!type || !ILType_IsComplex(type))
	{
		return 0;
	}
	else if(ILType_Kind(type) == IL_TYPE_COMPLEX_PTR)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

#define	IL_TYPE_COMPLEX_METHOD_REF	15

/*
 * Convert a method reference type into a method block.
 * Returns NULL if not a method reference type.
 */
static ILMethod *MethodRefToMethod(ILType *type)
{
	if(type != 0 && ILType_IsComplex(type) &&
	   ILType_Kind(type) == IL_TYPE_COMPLEX_METHOD_REF)
	{
		return (ILMethod *)(ILType_Ref(type));
	}
	else
	{
		return 0;
	}
}

/*
 * Convert a method block into a method reference type.
 * Returns NULL if out of memory.
 */
static ILType *MethodToMethodRef(TempAllocator *allocator, ILMethod *method)
{
	ILType *type = (ILType *)TempAllocate(allocator, sizeof(ILType));
	if(!type)
	{
		return 0;
	}
	type->kind__ = IL_TYPE_COMPLEX_METHOD_REF;
	type->num__ = 0;
	type->un.refType__ = (ILType *)method;
	return type;
}

/*
 * Determine if a stack item is assignment-compatible with
 * a particular memory slot (argument, local, field, etc).
 */
static int AssignCompatible(ILMethod *method, ILEngineStackItem *item,
							ILType *type, int unsafeAllowed)
{
	ILImage *image;
	ILClass *classInfo;
	ILClass *classInfo2;
	ILMethod *methodRef;
	ILType *objType;

	/* Check for safe and unsafe pointer assignments */
	if(item->engineType == ILEngineType_I)
	{
		methodRef = MethodRefToMethod(item->typeInfo);
		if(methodRef)
		{
			/* Assigning a method reference, obtained via "ldftn"
			   or "ldvirtftn", to a method pointer destination */
			if(ILTypeIdentical(ILMethod_Signature(methodRef), type))
			{
				return 1;
			}
		}
		else if(item->typeInfo != 0 && ILType_IsComplex(item->typeInfo))
		{
			/* May be trying to assign a method pointer to a method type */
			if(ILType_IsMethod(item->typeInfo))
			{
				if(ILTypeIdentical(item->typeInfo, type))
				{
					return 1;
				}
			}
		}
		if(unsafeAllowed)
		{
			if(type != 0 && ILType_IsComplex(type))
			{
				if((ILType_Kind(type) & IL_TYPE_COMPLEX_METHOD) != 0 ||
				   ILType_Kind(type) == IL_TYPE_COMPLEX_PTR)
				{
					return 1;
				}
			}
		}
	}

	/* Check for regular assignments */
	if(item->engineType == ILEngineType_I4 ||
	   item->engineType == ILEngineType_I)
	{
		type = ILTypeGetEnumType(type);
		switch((unsigned long)type)
		{
			case (unsigned long)ILType_Boolean:
			case (unsigned long)ILType_Int8:
			case (unsigned long)ILType_UInt8:
			case (unsigned long)ILType_Int16:
			case (unsigned long)ILType_UInt16:
			case (unsigned long)ILType_Char:
			case (unsigned long)ILType_Int32:
			case (unsigned long)ILType_UInt32:
			case (unsigned long)ILType_Int:
			case (unsigned long)ILType_UInt:	return 1;
			default: break;
		}

		if(!unsafeAllowed)
		{
			return 0;
		}

		/* Allow a native int to be assigned to a complex type */
		if(type != 0 && ILType_IsComplex(type) && 
						item->engineType == ILEngineType_I)
		{
			if(ILType_Kind(type) == IL_TYPE_COMPLEX_PTR ||
			  ILType_Kind(type) == IL_TYPE_COMPLEX_BYREF) 
			{
				return 1;
			}
		}
		return 0;
	}
	else if(item->engineType == ILEngineType_I8)
	{
		type = ILTypeGetEnumType(type);
		return (type == ILType_Int64 || type == ILType_UInt64);
	}
	else if(item->engineType == ILEngineType_F)
	{
		return (type == ILType_Float32 ||
		        type == ILType_Float64 ||
		        type == ILType_Float);
	}
	else if(item->engineType == ILEngineType_O)
	{
		if(!(item->typeInfo))
		{
			/* A "null" constant was pushed, which is
			   compatible with any object reference type */
			return IsObjectRef(type);
		}
		if(!IsObjectRef(type) || !IsObjectRef(item->typeInfo))
		{
			/* Both types must be object references */
			return 0;
		}
		/* make a copy to avoid unecessary complications */
		objType=item->typeInfo;
		if(ILType_IsArray(type) && ILType_IsArray(objType) &&
			(ILTypeGetRank(type) == ILTypeGetRank(objType)))
		{
			objType=ILTypeGetElemType(objType);
			type=ILTypeGetElemType(type);
		}
		image = ILProgramItem_Image(method);
		classInfo = ILClassResolve(ILClassFromType(image, 0, type, 0));
		classInfo2 = ILClassResolve
			(ILClassFromType(image, 0, objType, 0));
		if(classInfo && classInfo2)
		{
			/* Is the type a regular class or an interface? */
			if(!ILClass_IsInterface(classInfo))
			{
				/* Regular class: the value must inherit from the type */
				if(ILClassInheritsFrom(classInfo2, classInfo))
				{
					return 1;
				}

				/* If "classInfo2" is an interface, then the conversion
				   is OK if "type" is "System.Object", because all
				   interfaces inherit from "System.Object", even though
				   the metadata doesn't explicitly say so */
				if(ILClass_IsInterface(classInfo2))
				{
					return ILTypeIsObjectClass(type);
				}

				/* The conversion is not OK */
				return 0;
			}
			else
			{
				/* Interface which the value must implement or inherit from */
				return ILClassImplements(classInfo2, classInfo) ||
				       ILClassInheritsFrom(classInfo2, classInfo);
			}
		}
		else
		{
			return 0;
		}
	}
	else if(item->engineType == ILEngineType_MV)
	{
		/* Can only assign managed values to an exact type destination */
		return ILTypeIdentical(item->typeInfo, type);
	}
	else if(item->engineType == ILEngineType_TypedRef)
	{
		/* The type must be "typedref" */
		return (type == ILType_TypedRef);
	}
	else if(item->engineType == ILEngineType_M ||
	        item->engineType == ILEngineType_T)
	{
		/* Cannot assign managed pointers to variables or fields,
		   unless we are in "unsafe" mode */
		if(!unsafeAllowed)
		{
			return 0;
		}

		/* Allow an assignment to any pointer, reference, or native
		   destination, regardless of type.  This allows C/C++ code
		   to arbitrarily cast pointers via assignment */
		if(type != 0 && ILType_IsComplex(type))
		{
			if(ILType_Kind(type) == IL_TYPE_COMPLEX_PTR ||
			   ILType_Kind(type) == IL_TYPE_COMPLEX_BYREF ||
			   (ILType_Kind(type) & IL_TYPE_COMPLEX_METHOD) != 0)
			{
				return 1;
			}
		}
		else if(type == ILType_Int || type == ILType_UInt)
		{
			return 1;
		}
		return 0;
	}
	else
	{
		/* Invalid type: never assignment-compatible with anything */
		return 0;
	}
}

/*
 * Determine if a type is a sub-class of a specific class.
 */
static int IsSubClass(ILType *type, ILClass *classInfo)
{
	ILClass *typeClass;
	if(type == 0)
	{
		/* The type is "null", which is always a sub-class */
		return 1;
	}
	else if(ILType_IsClass(type) || ILType_IsValueType(type))
	{
		typeClass = ILType_ToClass(type);
		if(ILClassInheritsFrom(typeClass, classInfo) ||
		   ILClassImplements(typeClass, classInfo))
		{
			return 1;
		}
		return 0;
	}
	else if((typeClass = ILClassFromType(ILClassToImage(classInfo),
										 0, type, 0)) != 0)
	{
		if(ILClassInheritsFrom(typeClass, classInfo) ||
		   ILClassImplements(typeClass, classInfo))
		{
			return 1;
		}
		return 0;
	}
	else
	{
		return 0;
	}
}

/*
 * Push the appropriate synchronization object for a synchronized method.
 */
#define PUSH_SYNC_OBJECT() \
	if (isStatic) \
	{ \
		ILCoderPushToken(coder, (ILProgramItem *)ILMethod_Owner(method)); \
		ILCoderCallInlineable(coder, IL_INLINEMETHOD_TYPE_FROM_HANDLE, 0, 0); \
	} \
	else \
	{ \
		ILCoderLoadArg(coder, 0, ILType_FromClass(ILMethod_Owner(method))); \
	}

/*
 * Bailout routines for various kinds of verification failure.
 */
#ifndef IL_CONFIG_REDUCE_CODE
#define	IL_VERIFY_DEBUG
#endif
#ifdef IL_VERIFY_DEBUG
#define	VERIFY_REPORT()	\
			do { \
				fprintf(stderr, "%s::%s [%lX] - %s at %s:%d\n", \
						ILClass_Name(ILMethod_Owner(method)), \
						ILMethod_Name(method), \
						(unsigned long)(offset + ILMethod_RVA(method) + \
										code->headerSize), \
						(insn ? insn->name : "none"), __FILE__, __LINE__); \
			} while (0)
#define	VERIFY_PREFIX_REPORT()	\
			do { \
				fprintf(stderr, "%s::%s [%lX] - invalid prefix for %s at %s:%d\n", \
						ILClass_Name(ILMethod_Owner(method)), \
						ILMethod_Name(method), \
						(unsigned long)(offset + ILMethod_RVA(method) + \
										code->headerSize), \
						(insn ? insn->name : "none"), __FILE__, __LINE__); \
			} while (0)
#else
#define	VERIFY_REPORT()	do {} while (0)
#define	VERIFY_PREFIX_REPORT()	do {} while (0)
#endif
#define	VERIFY_TRUNCATED()		VERIFY_REPORT(); goto cleanup
#define	VERIFY_BRANCH_ERROR()	VERIFY_REPORT(); goto cleanup
#define	VERIFY_INSN_ERROR()		VERIFY_REPORT(); goto cleanup
#define	VERIFY_STACK_ERROR()	VERIFY_REPORT(); goto cleanup
#define	VERIFY_TYPE_ERROR()		VERIFY_REPORT(); goto cleanup
#define	VERIFY_MEMORY_ERROR()	VERIFY_REPORT(); goto cleanup
#define	VERIFY_PREFIX_ERROR()	VERIFY_PREFIX_REPORT(); goto cleanup

/*
 * Declare global definitions that are required by the include files.
 */
#define	IL_VERIFY_GLOBALS
#include "verify_var.c"
#include "verify_const.c"
#include "verify_arith.c"
#include "verify_conv.c"
#include "verify_stack.c"
#include "verify_ptr.c"
#include "verify_obj.c"
#include "verify_branch.c"
#include "verify_call.c"
#include "verify_except.c"
#include "verify_ann.c"
#undef IL_VERIFY_GLOBALS

int _ILVerify(ILCoder *coder, unsigned char **start, ILMethod *method,
			  ILMethodCode *code, int unsafeAllowed, ILExecThread *thread)
{
	TempAllocator allocator;
	ILCoderExceptions coderExceptions;
	ILCoderExceptionBlock *coderException;
	ILCoderExceptionBlock *currentCoderException;
	int numHandlers;
	int extraCodeLen;
	unsigned long *jumpMask;
	unsigned char *pc;
	ILUInt32 len;
	int result;
	unsigned opcode;
	ILUInt32 insnSize;
	int isStatic, isSynchronized;
	int insnType;
	ILUInt32 offset = 0;
	ILEngineStackItem *stack;
	ILUInt32 stackSize;
#ifdef IL_VERIFY_DEBUG
	const ILOpcodeInfo *insn = 0;
	#define	MAIN_OPCODE_TABLE	ILMainOpcodeTable
	#define	PREFIX_OPCODE_TABLE	ILPrefixOpcodeTable
#else
	const ILOpcodeSmallInfo *insn = 0;
	#define	MAIN_OPCODE_TABLE	ILMainOpcodeSmallTable
	#define	PREFIX_OPCODE_TABLE	ILPrefixOpcodeSmallTable
#endif
	ILType *signature;
	ILType *type;
	ILUInt32 numArgs;
	ILUInt32 numLocals;
	ILType *localVars;
	int lastWasJump;
	ILException *exceptions;
	ILException *exception;
	int hasRethrow;
	int tryInlineType;
	int coderFlags;
	unsigned int tryInlineOpcode;
	unsigned char *tryInlinePc;
	ILUInt32 optimizationLevel;
	ILBool lastInsnWasPrefix;
	ILCoderPrefixInfo prefixInfo;
#ifdef IL_CONFIG_DEBUG_LINES
	int haveDebug = ILDebugPresent(ILProgramItem_Image(method));
#else
	int haveDebug = 0;
#endif

	/* Include local variables that are required by the include files */
#define IL_VERIFY_LOCALS
#include "verify_var.c"
#include "verify_const.c"
#include "verify_arith.c"
#include "verify_conv.c"
#include "verify_stack.c"
#include "verify_ptr.c"
#include "verify_obj.c"
#include "verify_call.c"
#include "verify_branch.c"
#include "verify_except.c"
#include "verify_ann.c"
#undef IL_VERIFY_LOCALS

	/* Get the exception list */
	if(!ILMethodGetExceptions(method, code, &exceptions))
	{
		return 0;
	}

	/* Clear the exception management structure */
	ILMemZero(&coderExceptions, sizeof(ILCoderExceptions));
	/*
	 * Initialize the size of the additional code generated for
	 * synchronization.
	 */
	extraCodeLen = 0;
	/* And set the last label to the code length */
	coderExceptions.lastLabel = code->codeLen;
	
	/* Initialize the memory allocator that is used for temporary
	   allocation during bytecode verification */
	ILMemZero(allocator.buffer, sizeof(allocator.buffer));
	allocator.posn = 0;
	allocator.overflow = 0;

	coderFlags = ILCoderGetFlags(coder);
	optimizationLevel = ILCoderGetOptimizationLevel(coder);
	isStatic = ILMethod_IsStatic(method);
	isSynchronized = ILMethod_IsSynchronized(method);

	result = 0;
	if(exceptions || isSynchronized)
	{
		numHandlers = 0;
		exception = exceptions;
		while(exception)
		{
			++numHandlers;
			exception = exception->next;
		}
		if(isSynchronized)
		{
			/* We'll need an extra try and fault block for synchronization */
			++numHandlers;
		}
		/*
		 * Allocate memory for the exception infos.
		 * There might be create 3 coder exception blocks for one
		 * IL exception.
		 * So we allocate memory for the worst case here.
		 */
		coderExceptions.blocks = ILCalloc(sizeof(ILCoderExceptionBlock),
										  numHandlers * 3);
		if(!coderExceptions.blocks)
		{
			return 0;
		}

		/* Now setup the exception structure */
		exception = exceptions;
		while(exception)
		{
			switch(_ILCoderAddExceptionBlock(&coderExceptions, method,
											 exception))
			{
				case IL_CODER_BRANCH_ERR:
				{
					VERIFY_BRANCH_ERROR();
				}
				break;

				case IL_CODER_TYPE_ERR:
				{
					VERIFY_TYPE_ERROR();
				}
				break;
			}
			exception = exception->next;
		}
		/*
		 * Now check if all exception block limits are in the code.
		 */
		len = code->codeLen;
		coderException = coderExceptions.firstBlock;
		if(coderException)
		{
			/*
			 * Check the start offset of the first exception block in the
			 * lowest list.
			 */
			if(coderException->startOffset > len)
			{
				VERIFY_BRANCH_ERROR();
			}
			/*
			 * Look for the last exception block in the lowest list.
			 */
			while(coderException->nextNested)
			{
				coderException = coderException->nextNested;
			}
			/*
			 * Check the end offset of the last exception block in the
			 * lowest list.
			 * All other exceprion blocks end at or before this offset.
			 */
			if(coderException->endOffset > len)
			{
				VERIFY_BRANCH_ERROR();
			}
		}
		if(isSynchronized)
		{
			/*
			 * Wrap the whole function in a try block with a fault handler.
			 */
			ILException tempException;

			tempException.flags = IL_META_EXCEPTION_FAULT;
			tempException.tryOffset = 0;
			tempException.tryLength = len;
			tempException.handlerOffset = len;
			tempException.handlerLength = 1;
			tempException.extraArg = 0;
			tempException.userData = 0;
			tempException.ptrUserData = 0;
			tempException.next = 0;

			switch(_ILCoderAddExceptionBlock(&coderExceptions, method,
											 &tempException))
			{
				case IL_CODER_BRANCH_ERR:
				{
					VERIFY_BRANCH_ERROR();
				}
				break;

				case IL_CODER_TYPE_ERR:
				{
					VERIFY_TYPE_ERROR();
				}
				break;
			}
			extraCodeLen = 2;
		}
	}

restart:
	result = 0;
	labelList = 0;
	hasRethrow = 0;

	/* Reset the prefix information */
	ILMemZero(&prefixInfo, sizeof(ILCoderPrefixInfo));

	/* Allocate the jump target mask */
	jumpMask = (unsigned long *)TempAllocate
					(&allocator, BYTES_FOR_MASK(code->codeLen + extraCodeLen));
	if(!jumpMask)
	{
		VERIFY_MEMORY_ERROR();
	}

	/* Scan the code looking for all jump targets, and validating
	   that all instructions are more or less valid */
	pc = code->code;
	len = code->codeLen;
	while(len > 0)
	{
		/* Mark this position in the jump mask as an instruction start */
		MarkInsnStart(jumpMask, (ILUInt32)(pc - (unsigned char *)(code->code)));

		/* Fetch the instruction size and type */
		opcode = (unsigned)(pc[0]);
		if(opcode != IL_OP_PREFIX)
		{
			/* Regular opcode */
			insnSize = (ILUInt32)(MAIN_OPCODE_TABLE[opcode].size);
			if(len < insnSize)
			{
				VERIFY_TRUNCATED();
			}
			insnType = MAIN_OPCODE_TABLE[opcode].args;
		}
		else
		{
			/* Prefixed opcode */
			if(len < 2)
			{
				VERIFY_TRUNCATED();
			}
			opcode = (unsigned)(pc[1]);
			insnSize = (ILUInt32)(PREFIX_OPCODE_TABLE[opcode].size);
			if(len < insnSize)
			{
				VERIFY_TRUNCATED();
			}
			insnType = PREFIX_OPCODE_TABLE[opcode].args;
			if(opcode == IL_PREFIX_OP_RETHROW)
			{
				hasRethrow = 1;
			}
			opcode += IL_OP_PREFIX;
		}

		/* Determine how to handle this type of instruction */
		switch(insnType)
		{
			case IL_OPCODE_ARGS_SHORT_JUMP:
			{
				/* 8-bit jump offset */
				offset = (ILUInt32)((pc + insnSize) -
										(unsigned char *)(code->code)) +
						 (ILUInt32)(ILInt32)(ILInt8)(pc[1]);
				if(offset >= code->codeLen)
				{
					VERIFY_BRANCH_ERROR();
				}
				MarkJumpTarget(jumpMask, offset);
			}
			break;

			case IL_OPCODE_ARGS_LONG_JUMP:
			{
				/* 32-bit jump offset */
				offset = (ILUInt32)((pc + insnSize) -
										(unsigned char *)(code->code)) +
						 (ILUInt32)(IL_READ_INT32(pc + 1));
				if(offset >= code->codeLen)
				{
					VERIFY_BRANCH_ERROR();
				}
				MarkJumpTarget(jumpMask, offset);
			}
			break;

			case IL_OPCODE_ARGS_SWITCH:
			{
				/* Switch statement */
				if(len < 5)
				{
					VERIFY_TRUNCATED();
				}
				numArgs = IL_READ_UINT32(pc + 1);
				insnSize = 5 + numArgs * 4;
				if(numArgs >= 0x20000000 || len < insnSize)
				{
					VERIFY_TRUNCATED();
				}
				while(numArgs > 0)
				{
					--numArgs;
					offset = (ILUInt32)((pc + insnSize) -
											(unsigned char *)(code->code)) +
							 (ILUInt32)(IL_READ_INT32(pc + 5 + numArgs * 4));
					if(offset >= code->codeLen)
					{
						VERIFY_BRANCH_ERROR();
					}
					MarkJumpTarget(jumpMask, offset);
				}
			}
			break;

			case IL_OPCODE_ARGS_ANN_DATA:
			{
				/* Variable-length annotation data */
				if(opcode == IL_OP_ANN_DATA_S)
				{
					if(len < 2)
					{
						VERIFY_TRUNCATED();
					}
					insnSize = (((ILUInt32)(pc[1])) & 0xFF) + 2;
					if(len < insnSize)
					{
						VERIFY_TRUNCATED();
					}
				}
				else
				{
					if(len < 6)
					{
						VERIFY_TRUNCATED();
					}
					insnSize = (IL_READ_UINT32(pc + 2) + 6);
					if(len < insnSize)
					{
						VERIFY_TRUNCATED();
					}
				}
			}
			break;

			case IL_OPCODE_ARGS_ANN_PHI:
			{
				/* Variable-length annotation data */
				if(len < 3)
				{
					VERIFY_TRUNCATED();
				}
				insnSize = ((ILUInt32)IL_READ_UINT16(pc + 1)) * 2 + 3;
				if(len < insnSize)
				{
					VERIFY_TRUNCATED();
				}
			}
			break;

			case IL_OPCODE_ARGS_INVALID:
			{
				VERIFY_INSN_ERROR();
			}
			break;

			default: break;
		}

		/* Advance to the next instruction */
		pc += insnSize;
		len -= insnSize;
	}

	/* Mark the start and end of exception blocks as special jump targets */
	numHandlers = 0;
	while(numHandlers < coderExceptions.numBlocks)
	{
		coderException = &(coderExceptions.blocks[numHandlers]);
		MarkJumpTarget(jumpMask, coderException->startOffset);
		MarkSpecialJumpTarget(jumpMask, coderException->startOffset);
		MarkJumpTarget(jumpMask, coderException->endOffset);
		MarkSpecialJumpTarget(jumpMask, coderException->endOffset);
		switch(coderException->flags)
		{
			case IL_CODER_HANDLER_TYPE_TRY:
			{
				/* Nothing to do here */
			}
			break;

			case IL_CODER_HANDLER_TYPE_CATCH:
			{
				/* This is a typed catch block */
				classInfo = coderException->un.handlerBlock.exceptionClass;
				/*
				 * This block will be called with an object of the given
				 * type on the stack.
				 */
				SET_TARGET_STACK(coderException->startOffset, classInfo);
			}
			break;

			case IL_CODER_HANDLER_TYPE_FINALLY:
			case IL_CODER_HANDLER_TYPE_FAULT:
			{
				/* This is a finally or fault clause */
				/* The clause will be called with nothing on the stack */
				SET_TARGET_STACK_EMPTY(coderException->startOffset);
			}
			break;

			case IL_CODER_HANDLER_TYPE_FILTER:
			case IL_CODER_HANDLER_TYPE_FILTEREDCATCH:
			{
				/* This is an exception filter or the corresponding catch block */
				/* 
				 * The block will be called with an object on the stack,
				 * so record that in the label list for later
				 */
				classInfo = ILClassResolveSystem(ILProgramItem_Image(method),
												 0, "Object", "System");
				if(!classInfo)
				{
					/* Ran out of memory trying to create "System.Object" */
					VERIFY_MEMORY_ERROR();
				}
				SET_TARGET_STACK(coderException->startOffset, classInfo);
			}
			break;
		}
		++numHandlers;
	}

	/* Make sure that all jump targets are instruction starts */
	len = code->codeLen;
	while(len > 0)
	{
		--len;
		if(IsJumpTarget(jumpMask, len) && !IsInsnStart(jumpMask, len))
		{
			VERIFY_BRANCH_ERROR();
		}
	}

	/* Create the stack.  We need two extra "slop" items to allow for
	   stack expansion during object construction.  See "verify_call.c"
	   for further details */
	stack = (ILEngineStackItem *)TempAllocate
				(&allocator, sizeof(ILEngineStackItem) * (code->maxStack + 2));
	if(!stack)
	{
		VERIFY_MEMORY_ERROR();
	}
	stackSize = 0;

	/* Get the method signature, plus the number of arguments and locals */
	signature = ILMethod_Signature(method);
	numArgs = ILTypeNumParams(signature);
	if(ILType_HasThis(signature))
	{
		/* Account for the "this" argument */
		++numArgs;
	}
	if(code->localVarSig)
	{
		localVars = ILStandAloneSigGetType(code->localVarSig);
		numLocals = ILTypeNumLocals(localVars);
		if(ILTypeNeedsInstantiation(localVars) &&
		   ILMember_IsGenericInstance(method))
		{
			ILType *classTypeArgs = ILMethodGetClassTypeArguments(method);
			ILType *methodTypeArgs = ILMethodGetMethodTypeArguments(method);

			localVars = ILTypeInstantiate(ILImageToContext(ILProgramItem_Image(method)),
										  localVars,
										  classTypeArgs, 
										  methodTypeArgs);
		}
	}
	else
	{
		localVars = 0;
		numLocals = 0;
	}

	/* Set up the coder to process the method */
	if(!ILCoderSetup(coder, start, method, code, &coderExceptions, hasRethrow))
	{
		VERIFY_MEMORY_ERROR();
	}

	if((coderFlags & IL_CODER_FLAG_METHOD_PROFILE) != 0)
	{
		ILCoderProfileStart(coder);
	}

	/* Verify the code */
	pc = code->code;
	len = code->codeLen;
	lastWasJump = 0;

	/* If the method is synchronized then generate the Monitor.Enter call */
	if (isSynchronized)
	{
		PUSH_SYNC_OBJECT();
		ILCoderCallInlineable(coder, IL_INLINEMETHOD_MONITOR_ENTER, 0, 0);
	}

	lastInsnWasPrefix = 0;
	while(len > 0)
	{
		/* Fetch the instruction information block */
		offset = (ILUInt32)(pc - (unsigned char *)(code->code));
		opcode = pc[0];
		if(opcode != IL_OP_PREFIX)
		{
			insn = &(MAIN_OPCODE_TABLE[opcode]);
		}
		else
		{
			opcode = pc[1];
			insn = &(PREFIX_OPCODE_TABLE[opcode]);
			opcode += IL_OP_PREFIX;
		}
		insnSize = (ILUInt32)(insn->size);

		/* Is this a jump target? */
		if(IsJumpTarget(jumpMask, offset))
		{
			/* Validate the stack information */
			VALIDATE_TARGET_STACK(offset);

			/* Notify the coder of a label at this position */
		#ifdef IL_USE_JIT
			ILCoderStackRefresh(coder, stack, stackSize);
			ILCoderLabel(coder, offset);
		#else
			ILCoderLabel(coder, offset);
			ILCoderStackRefresh(coder, stack, stackSize);
		#endif
		}
		else if(lastWasJump)
		{
			/* An instruction just after an opcode that jumps to
			   somewhere else in the flow of control.  As this
			   isn't a jump target, we assume that the stack
			   must be empty at this point.  The validate code
			   will ensure that this is checked */
			VALIDATE_TARGET_STACK(offset);

			/* Reset the coder's notion of the stack to empty */
			ILCoderStackRefresh(coder, stack, stackSize);
		}

		/* Mark this offset if the method has debug information */
		if(haveDebug)
		{
			ILCoderMarkBytecode(coder, offset);
		}

		/* Validate the stack height changes */
		if(stackSize < ((ILUInt32)(insn->popped)) ||
		   (stackSize - ((ILUInt32)(insn->popped)) + ((ILUInt32)(insn->pushed)))
				> code->maxStack)
		{
			VERIFY_STACK_ERROR();
		}

		/*
		 * Check if all prefix flags are zero, otherwise an invalid prefix
		 * was used for the last instruction.
		 */
		if(!lastInsnWasPrefix)
		{
			if((prefixInfo.prefixFlags != 0) || (prefixInfo.noFlags != 0))
			{
				VERIFY_PREFIX_ERROR();
			}
		}

		/* Verify the instruction */
		lastWasJump = 0;
		lastInsnWasPrefix = 0;
		switch(opcode)
		{
			case IL_OP_NOP:   break;

			/* IL breakpoints are ignored - the coder inserts its
			   own breakpoint handlers where required */
			case IL_OP_BREAK: break;

#define	IL_VERIFY_CODE
#include "verify_var.c"
#include "verify_const.c"
#include "verify_arith.c"
#include "verify_conv.c"
#include "verify_stack.c"
#include "verify_ptr.c"
#include "verify_obj.c"
#include "verify_branch.c"
#include "verify_call.c"
#include "verify_except.c"
#include "verify_ann.c"
#undef IL_VERIFY_CODE

		}

		/* Advance to the next instruction */
		pc += insnSize;
		len -= insnSize;
	}

	/* If the last instruction was not a jump, then the code
	   may fall off the end of the method */
	if(!lastWasJump)
	{
		VERIFY_INSN_ERROR();
	}

	/*
	 * Generate the code for the fault block for synchronization.
	 */
	if(isSynchronized)
	{
		coderException = _ILCoderFindExceptionBlock(&coderExceptions, code->codeLen);
		/*
		 * This check is for catching bugs.
		 */
		if(!coderException ||
		   ((coderException->flags & IL_CODER_HANDLER_TYPE_FINALLY) == 0))
		{
			VERIFY_BRANCH_ERROR();
		}
		/*
		 * Insert the start label for the fault handler.
		 */
		ILCoderLabel(coder, code->codeLen);

		/*
		 * Call the Monitor.Exit method.
		 */
		PUSH_SYNC_OBJECT();
		ILCoderCallInlineable(coder, IL_INLINEMETHOD_MONITOR_EXIT, 0, 0);
		/*
		 * Leave the fault block.
		 */
		ILCoderRetFromFinally(coder);
		/*
		 * Insert the end label for the fault handler.
		 */
		ILCoderLabel(coder, code->codeLen + 1);
	}
	
	/* Mark the end of the method */
	ILCoderMarkEnd(coder);

	/* Output the exception handler table, if necessary */
	if(coderExceptions.numBlocks > 0)
	{
		ILCoderOutputExceptionTable(coder, &coderExceptions);
	}

	/* Finish processing using the coder */
	result = ILCoderFinish(coder);

	/* Do we need to restart due to cache exhaustion in the coder? */
	if(result == IL_CODER_END_RESTART)
	{
		TempAllocatorDestroy(&allocator);
		/* Reinitialize the memory allocator that is used for temporary
		   allocation during bytecode verification */
		ILMemZero(allocator.buffer, sizeof(allocator.buffer));
		allocator.posn = 0;
		allocator.overflow = 0;

		/*
		 * Reset the userdata in the exception blocks.
		 */
		numHandlers = 0;
		while(numHandlers < coderExceptions.numBlocks)
		{
			coderException = &(coderExceptions.blocks[numHandlers]);
			coderException->userData = 0;
			coderException->ptrUserData = 0;
			++numHandlers;
		}
		
		goto restart;
	}
#ifdef IL_VERIFY_DEBUG
	else if(result == IL_CODER_END_TOO_BIG)
	{
		ILMethodCode code;
		ILMethodGetCode(method,&code);
		fprintf(stderr,
			"%s::%s - method is too big to be converted (%d%s bytes)\n",
							ILClass_Name(ILMethod_Owner(method)),
							ILMethod_Name(method),
							code.codeLen,
							(code.moreSections != 0) ? "+" : "");
	}
#endif
	result = (result == IL_CODER_END_OK);

	/* Clean up and exit */
cleanup:
	TempAllocatorDestroy(&allocator);
	if(exceptions)
	{
		ILMethodFreeExceptions(exceptions);
	}
	if(coderExceptions.blocks)
	{
		ILFree(coderExceptions.blocks);
	}

	return result;
}

#ifdef	__cplusplus
};
#endif
