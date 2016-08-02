/*
 * jitc_string.c - Jit coder string handling routines.
 *
 * Copyright (C) 2008  Southern Storm Software, Pty Ltd.
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

#ifdef	IL_JITC_DECLARATIONS

/*
 * The simple array header size.
 */
#define _IL_JIT_STRING_HEADERSIZE	((ILInt32)(ILNativeInt)StringToBuffer(0))

/*
 * Validate the string index.
 */
#define JITC_START_CHECK_STRING_INDEX(jitCoder, length, index) \
	{ \
		jit_label_t __label = jit_label_undefined; \
		jit_label_t __okLabel = jit_label_undefined; \
		ILJitValue  __length = (length); \
		ILJitValue  __index = (index); \
		ILJitValue  __temp; \
		\
		AdjustSign(jitCoder->jitFunction, &(__length), 1, 0); \
		AdjustSign(jitCoder->jitFunction, &(__index), 1, 0); \
		\
		__temp = jit_insn_le(jitCoder->jitFunction, __length, __index); \
		jit_insn_branch_if(jitCoder->jitFunction, __temp, &__label);

#define JITC_END_CHECK_STRING_INDEX(jitCoder) \
		jit_insn_branch(jitCoder->jitFunction, &__okLabel); \
		jit_insn_label(jitCoder->jitFunction, &__label); \
		_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_INDEX_OUT_OF_RANGE); \
		jit_insn_label(jitCoder->jitFunction, &__okLabel); \
	}

/*
 * Allocate a string with the given length.
 */
static System_String *_ILJitStringAlloc(ILClass *stringClass,
										ILUInt32 numChars);

/*
 * Get the pointer to the firct character in a string.
 */
static ILJitValue _ILJitStringGetStart(ILJitFunction jitFunction,
									   ILJitValue string);

/*
 * Get the ength of a string.
 */
static ILJitValue _ILJitStringGetLength(ILJitFunction jitFunction,
										ILJitValue string);


/*
 * Inline function to create a new string with the given length 
 */
static int _ILJitSystemStringNew(ILJITCoder *jitCoder,
								 ILMethod *method,
								 ILCoderMethodInfo *methodInfo,
								 ILJitStackItem *args,
								 ILInt32 numArgs);

/*
 * Inline function to get the character at the specified indexe in a
 * string.
 */
static int _ILJitSystemStringChars(ILJITCoder *jitCoder,
								   ILMethod *method,
								   ILCoderMethodInfo *methodInfo,
								   ILJitStackItem *args,
								   ILInt32 numArgs);

#endif	/* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_FUNCTIONS

/*
 * Allocate a string with the given length.
 */
static System_String *_ILJitStringAlloc(ILClass *stringClass,
										ILUInt32 numChars)
{
	ILUInt64 totalSize;
	ILInt32 roundLen;
	System_String *ptr;

	 /* Add space for the 0 character and round to a multiple of 8 */
	roundLen = ((numChars + 8) & ~7);
	totalSize = (((ILUInt64)roundLen) * ((ILUInt64)sizeof(ILUInt16))) +
				_IL_JIT_STRING_HEADERSIZE;
	if(totalSize > (ILUInt64)IL_MAX_UINT32)
	{
		/* The array is too large. */
		/* Throw an "OutOfMemoryException" */
		ILRuntimeExceptionThrowOutOfMemory();
		return (System_String *)0;
	}
	ptr = (System_String *)_ILJitAllocAtomic(stringClass, (ILUInt32)totalSize);
	ptr->capacity = roundLen - 1;
	ptr->length = (ILInt32)numChars;
	return ptr;
}

/*
 * Get the pointer to the first character in a string.
 */
static ILJitValue _ILJitStringGetStart(ILJitFunction jitFunction,
									   ILJitValue string)
{
	return jit_insn_add_relative(jitFunction, string,
								 _IL_JIT_STRING_HEADERSIZE);
}

/*
 * Get the length of a string.
 */
static ILJitValue _ILJitStringGetLength(ILJitFunction jitFunction,
										ILJitValue string)
{
	ILJitValue len;

	len = jit_insn_load_relative(jitFunction,
								 string,
								 offsetof(System_String, length),
								 _IL_JIT_TYPE_INT32);
	return len;
}

/*
 * Inline function to create a new string with the given length 
 */
static int _ILJitSystemStringNew(ILJITCoder *jitCoder,
								 ILMethod *method,
								 ILCoderMethodInfo *methodInfo,
								 ILJitStackItem *args,
								 ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILClass *stringClass = ILMethod_Owner(method);
	ILJitValue newString;
	ILJitValue callArgs[2];

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), stringClass))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if(jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout, "Inline System.String::NewString\n");
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	/* We call the alloc functions. */
	/* They thow an out of memory exception so we don't need to care. */
	callArgs[0] = jit_value_create_nint_constant(jitCoder->jitFunction,
												 _IL_JIT_TYPE_VPTR,
												 (jit_nint)stringClass);
	if(!callArgs[0])
	{
		return 0;
	}
	callArgs[1] = _ILJitValueConvertImplicit(jitCoder->jitFunction,
						 					 _ILJitStackItemValue(args[0]),
											 _IL_JIT_TYPE_UINT32);
	if(!callArgs[1])
	{
		return 0;
	}
	newString = jit_insn_call_native(jitCoder->jitFunction,
									 "_ILJitStringAlloc",
									 _ILJitStringAlloc,
									 _ILJitSignature_ILJitStringAlloc,
				 					 callArgs, 2, 0);
	if(!newString)
	{
		return 0;
	}

	_ILJitStackPushValue(jitCoder, newString);
	return 1;
}

/*
 * Inline function to get the character at the specified indexe in a
 * string.
 */
static int _ILJitSystemStringChars(ILJITCoder *jitCoder,
								   ILMethod *method,
								   ILCoderMethodInfo *methodInfo,
								   ILJitStackItem *args,
								   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILClass *stringClass = ILMethod_Owner(method);
	ILJitValue length;
	ILJitValue stringBase;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), stringClass))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if(jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout, "Inline System.String::get_Chars\n");
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackItemCheckNull(jitCoder, args[0]);
	length = _ILJitStringGetLength(jitCoder->jitFunction,
								   _ILJitStackItemValue(args[0]));
	JITC_START_CHECK_STRING_INDEX(jitCoder, length, _ILJitStackItemValue(args[1]))
	stringBase = _ILJitStringGetStart(jitCoder->jitFunction,
									  _ILJitStackItemValue(args[0]));
	returnValue = jit_insn_load_elem(jitCoder->jitFunction,
									 stringBase,
									 _ILJitStackItemValue(args[1]),
									 _IL_JIT_TYPE_CHAR);
	JITC_END_CHECK_STRING_INDEX(jitCoder)

	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

#endif	/* IL_JITC_FUNCTIONS */

