/*
 * jitc_stack.c - Coder implementation for JIT stack operations.
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

#ifdef	IL_JITC_DECLARATIONS

#ifdef _IL_JIT_OPTIMIZE_LOCALS

struct _tagILJitStackItem
{
	ILInt32			flags;
	ILJitValue		value;
	ILJitLocalSlot *refValue;
};

#define _ILJitStackItemNew(name) ILJitStackItem name
#define _ILJitStackItemInit(item) \
	do { \
		(item).flags = 0; \
		(item).value = 0; \
		(item).refValue = 0; \
	} while(0)

#define _ILJitStackItemInitWithValue(item, v) \
	do { \
		(item).flags = 0; \
		(item).value = (v); \
		(item).refValue = 0; \
	} while(0)

#define _ILJitStackItemInitWithNotNullValue(item, v) \
	do { \
		(item).flags = _IL_JIT_VALUE_NULLCHECKED; \
		(item).value = (v); \
		(item).refValue = 0; \
	} while(0)

#define _ILJitStackItemDup(coder, dest, src) \
	do { \
		(dest).flags = (src).flags; \
		(dest).value = (src).value; \
		(dest).refValue = (src).refValue; \
	} while(0)

/*
 * Load the address of a field in the object *ptr into dest.
 * We copy the flags of the original pointer because it's a pointer into the same object.
 */
#define _ILJitStackItemFieldAddress(coder, dest, ptr, offset) \
	do { \
		(dest).flags = (ptr).flags; \
		(dest).refValue = (ptr).refValue; \
		if((offset) != 0) \
		{ \
			(dest).value = jit_insn_add_relative((coder)->jitFunction, (ptr).value, (offset)); \
			if((dest).refValue && ((dest).flags & _IL_JIT_VALUE_COPYOF)) \
			{ \
				(dest).flags &= ~_IL_JIT_VALUE_COPYOF; \
				if(!((dest).flags & _IL_JIT_VALUE_POINTER_TO)) \
				{ \
					(dest).refValue = 0; \
				} \
			} \
		} \
		else \
		{ \
			(dest).value = (ptr).value; \
		} \
	} while(0)

#define _ILJitStackItemValue(item) (item).value

#else	/* !_IL_JIT_OPTIMIZE_LOCALS */

typedef ILJitValue ILJitStackItem;

#define _ILJitStackItemNew(name) ILJitStackItem name = 0

#define _ILJitStackItemInit(item)	(item) = 0

#define _ILJitStackItemInitWithValue(item, v)	(item) = (v)

#define _ILJitStackItemInitWithNotNullValue(item, v)	(item) = (v)

#define _ILJitStackItemDup(coder, dest, src) \
	do { \
		(dest) = jit_insn_dup((coder)->jitFunction, (src)); \
	} while(0)

/*
 * Load the address of a field in the object *ptr into dest.
 */
#define _ILJitStackItemFieldAddress(coder, dest, ptr, offset) \
	do { \
		if((offset) != 0) \
		{ \
			(dest) = jit_insn_add_relative((coder)->jitFunction, (ptr), (offset)); \
		} \
		else \
		{ \
			(dest) = (ptr); \
		} \
	} while(0)

#define _ILJitStackItemValue(value) (value)

#endif	/* _IL_JIT_OPTIMIZE_LOCALS */

/*
 * Get the current height of the evaluation stack.
 */
#define _ILJitStackHeight(coder)	((coder)->stackTop < 0 ? 0 : (coder)->stackTop)

/*
 * Get the pointer to an ILJitStackItem on the evaluation stack relative to
 * the stack top.
 * Use this with care because modifying the stack item directly
 * can cause the optimizations and operations to fail.
 * n = 0 is the item on top of the stack, 1 the item below the
 * top item, ...
 */
#define _ILJitStackItemGetTop(coder, n) &((coder)->jitStack[(coder)->stackTop - (n) - 1])

/*
 * Get the pointer to an ILJitStackItem on the evaluation stack.
 */
#define _ILJitStackItemGet(coder, n) &((coder)->jitStack[(n)])

/*
 * Get the number of args on the evaluation stack from an ILCoderMethodInfo
 */
#define _ILJitStackNumArgs(i) ((i)->hasParamArray ? ((i)->numBaseArgs + 1) : ((i)->numBaseArgs + (i)->numVarArgs))

#endif	/* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_CODER_INSTANCE

	/* Members to manage the evaluation stack. */
	ILJitStackItem *jitStack;
	int				stackSize;
	int				stackTop;

#endif

#ifdef	IL_JITC_CODER_INIT

	/* Initialize the stack management. */
	coder->jitStack = 0;
	coder->stackTop = -1;
	coder->stackSize = 0;

#endif	/* IL_JITC_CODER_INIT */

#ifdef	IL_JITC_CODER_DESTROY

	if(coder->jitStack)
	{
		ILFree(coder->jitStack);
		coder->jitStack = 0;
	}

#endif	/* IL_JITC_CODER_DESTROY */

#ifdef	IL_JITC_FUNCTIONS

/*
 * Allocate enough space for "n" values on the stack.
 */
#define	ALLOC_STACK(coder, n)	\
	do { \
		if((n) > (coder)->stackSize) \
		{ \
			ILJitStackItem *newStack = \
				(ILJitStackItem *)ILRealloc((coder)->jitStack, \
											(n) * sizeof(ILJitStackItem)); \
			if(!newStack) \
			{ \
				return 0; \
			} \
			(coder)->jitStack = newStack; \
			(coder)->stackSize = (n); \
		} \
	} while (0)

#define _JITC_ADJUST(coder, num) \
	do { \
		(coder)->stackTop += (num); \
	} while(0)

#define JITC_ADJUST(coder, num) _JITC_ADJUST((coder), (num))

/*
 * Remove the top most item from the evaluation stack.
 */
#define _ILJitStackRemoveTop(coder) JITC_ADJUST(coder, -1)

/*
 * Remove the top N items from the evaluation stack.
 */
#define _ILJitStackRemoveTopN(coder, n) JITC_ADJUST(coder, -(n))

/*
 * Get the pointer to the ILJitStackItem n elements down the stack and pop
 * the n elements off the evaluation stack.
 * This is used for function calls to get the args and pop them off the
 * evaluation stack.
 */
#define _ILJitStackItemGetAndPop(coder, n) \
	({ \
		JITC_ADJUST(coder, -(n)); \
		_ILJitStackItemGetTop(coder, -1); \
	})

/*
 * Make space for n items x stackItems down the stack.
 * Return a pointer to the first free slot.
 * This is used to make space for arguments to value type ctors.
 */
#define _ILJitStackMakeFreeSlots(coder, n, x) \
	({ \
		int __current; \
		ILJitStackItem *__stackItem = &((coder)->jitStack[(coder)->stackTop - (x)]); \
		(coder)->stackTop += (n); \
		for(__current = 0; __current < (x); __current++) \
		{ \
			(coder)->jitStack[(coder)->stackTop - __current - 1] = \
				(coder)->jitStack[(coder)->stackTop - __current - (n) - 1]; \
		} \
		__stackItem; \
	})

#ifdef _IL_JIT_OPTIMIZE_LOCALS

/*
 * Load the parameter n into the stack item s..
 */
#define _ILJitStackItemLoadArg(coder, s, n) \
	do { \
		(s).flags = (_ILJitParamFlags(coder, n) | _IL_JIT_VALUE_COPYOF); \
		(s).value = _ILJitParamValue(coder, n); \
		(s).refValue = (_ILJitParamGet(coder, n)); \
	} while(0)

/*
 * Find a copy of a local slot on the evaluation stack and replace the value
 * with a duplicate.
 * This is needed if the original value will be modified.
 */
#define _ILJitStackDupLocal(coder, localSlot) \
	do { \
		ILJitValue __dupValue = 0; \
		ILInt32 __stackTop = (coder)->stackTop; \
		ILInt32 __stackPos ; \
		for(__stackPos = 0; __stackPos < __stackTop; ++__stackPos) \
		{ \
			if(((coder)->jitStack[__stackPos].refValue == (localSlot)) && \
				((coder)->jitStack[__stackPos].flags & _IL_JIT_VALUE_COPYOF)) \
			{ \
				__dupValue = jit_insn_dup((coder)->jitFunction, (coder)->jitStack[__stackPos].value); \
				(coder)->jitStack[__stackPos].value = __dupValue; \
				(coder)->jitStack[__stackPos].refValue = 0; \
				(coder)->jitStack[__stackPos].flags &= ~_IL_JIT_VALUE_COPYOF; \
			} \
		} \
	} while(0)

/*
 * Flag all occurances of the value (v) null checked on the stack.
 */
#define _ILJitStackSetNullChecked(coder, v) \
	do { \
		ILInt32 __stackTop = (coder)->stackTop; \
		ILInt32 __stackPos ; \
		for(__stackPos = 0; __stackPos < __stackTop; ++__stackPos) \
		{ \
			if((coder)->jitStack[__stackPos].value == (v)) \
			{ \
				(coder)->jitStack[__stackPos].flags |= _IL_JIT_VALUE_NULLCHECKED; \
			} \
		} \
	} while(0)

/*
 * Take the actions needed to preserve items on the stack from changing if the
 * value this stackitem points to is modified during a call.
 */
#define _ILJitStackHandleCallByRefArg(coder, stackItem) \
	do { \
		if((stackItem).refValue && ((stackItem).flags & _IL_JIT_VALUE_POINTER_TO)) \
		{ \
			_ILJitStackDupLocal(coder, (stackItem).refValue); \
		} \
	} while(0)

/*
 * Check if the stack item has to be duplicated on the first invokation of
 * a label.
 */
#define _ILJitStackItemNeedsDupOnLabel(stackItem) \
	((stackItem).refValue && ((stackItem).flags & (_IL_JIT_VALUE_COPYOF | _IL_JIT_VALUE_POINTER_TO)))

/*
 * Set the value in a stack item.
 * The only change allowed for the value are type cnversions.
 */
#define _ILJitStackItemSetValue(stackItem, v) \
	do { \
		if((stackItem).value != (v)) \
		{ \
			(stackItem).value = (v); \
			(stackItem).flags &= ~_IL_JIT_VALUE_COPYOF; \
			if(!((stackItem).flags & _IL_JIT_VALUE_POINTER_TO)) \
			{ \
				(stackItem).refValue = 0; \
			} \
		} \
	} while(0)

/*
 * Get the top most item from the evaluation stack.
 */
#define _ILJitStackGetTop(coder, v) \
	do { \
		ILJitStackItem *__stackValue = _ILJitStackItemGetTop(coder, 0); \
		(v).flags = __stackValue->flags; \
		(v).value = __stackValue->value; \
		(v).refValue = __stackValue->refValue; \
	} while(0)

/*
 * Pop the top most item off the evaluation stack.
 */
#define _ILJitStackPop(coder, v) \
	do { \
		_ILJitStackGetTop(coder, v); \
		_ILJitStackRemoveTop(coder); \
	} while(0)

/*
 * Push the item onto the evaluation stack.
 */
#define _ILJitStackPush(coder, v) \
	do { \
		ILJitStackItem *__stackValue = _ILJitStackItemGetTop(coder, -1); \
		__stackValue->flags = (v).flags; \
		__stackValue->value = (v).value; \
		__stackValue->refValue = (v).refValue; \
		JITC_ADJUST((coder), 1); \
	} while(0)

/*
 * Push the value onto the evaluation stack.
 */
#define _ILJitStackPushValue(coder, v) \
	do { \
		ILJitStackItem *__stackValue = _ILJitStackItemGetTop(coder, -1); \
		__stackValue->flags = 0; \
		__stackValue->value = (v); \
		__stackValue->refValue = 0; \
		JITC_ADJUST((coder), 1); \
	} while(0)

/*
 * Push the value known to be not null onto the evaluation stack.
 */
#define _ILJitStackPushNotNullValue(coder, v) \
	do { \
		ILJitStackItem *__stackValue = _ILJitStackItemGetTop(coder, -1); \
		__stackValue->flags = _IL_JIT_VALUE_NULLCHECKED; \
		__stackValue->value = (v); \
		__stackValue->refValue = 0; \
		JITC_ADJUST((coder), 1); \
	} while(0)

/*
 * Duplicate the top most item on the evaluation stack.
 */
#define _ILJitStackDup(coder) \
	do { \
		_ILJitStackItemNew(__value); \
		_ILJitStackGetTop(coder, __value); \
		_ILJitStackPush(jitCoder, __value); \
	} while(0)

/*
 * Push the argument onto the evaluation stack.
 */
#define _ILJitStackPushArg(coder, n) \
	do { \
		ILJitStackItem *__stackValue = _ILJitStackItemGetTop(coder, -1); \
		__stackValue->flags = (_ILJitParamFlags(coder, n) | _IL_JIT_VALUE_COPYOF); \
		__stackValue->value = _ILJitParamValue(coder, n); \
		__stackValue->refValue = (_ILJitParamGet(coder, n)); \
		JITC_ADJUST((coder), 1); \
	} while(0)

/*
 * Push the address of an argument onto the evaluation stack.
 */
#define _ILJitStackPushAddressOfArg(coder, n) \
	do { \
		ILJitStackItem *__stackValue = _ILJitStackItemGetTop(coder, -1); \
		__stackValue->flags = (_IL_JIT_VALUE_NULLCHECKED | _IL_JIT_VALUE_POINTER_TO); \
		__stackValue->value = _ILJitParamGetPointerTo(coder, n); \
		__stackValue->refValue = (_ILJitParamGet(coder, n)); \
		JITC_ADJUST((coder), 1); \
	} while(0)

/*
 * Pop the top most item off the evaluation stack and store it in an argument.
 */
#define _ILJitStackPopToArg(coder, n) \
	do { \
		_ILJitStackItemNew(__value) ; \
		ILJitLocalSlot *__refValue; \
		_ILJitStackPop(coder, __value); \
		__refValue = (_ILJitParamGet(coder, n)); \
		_ILJitStackDupLocal(coder, __refValue); \
		if(__value.flags & _IL_JIT_VALUE_NULLCHECKED) \
		{ \
			_ILJitParamStoreNotNullValue((coder), (n), _ILJitStackItemValue(__value)); \
		} \
		else \
		{ \
			_ILJitParamStoreValue((coder), (n), _ILJitStackItemValue(__value)); \
		} \
	} while(0)

/*
 * Push the local value onto the evaluation stack.
 */
#define _ILJitStackPushLocal(coder, n) \
	do { \
		ILJitStackItem *__stackValue = _ILJitStackItemGetTop(coder, -1); \
		__stackValue->flags = (_ILJitLocalFlags(coder, n) | _IL_JIT_VALUE_COPYOF); \
		__stackValue->value = _ILJitLocalValue(coder, n); \
		__stackValue->refValue = (_ILJitLocalGet(coder, n)); \
		JITC_ADJUST((coder), 1); \
	} while(0)

/*
 * Push the address of a local value onto the evaluation stack.
 */
#define _ILJitStackPushAddressOfLocal(coder, n) \
	do { \
		ILJitStackItem *__stackValue = _ILJitStackItemGetTop(coder, -1); \
		__stackValue->flags = (_IL_JIT_VALUE_NULLCHECKED | _IL_JIT_VALUE_POINTER_TO); \
		__stackValue->value = _ILJitLocalGetPointerTo(coder, n); \
		__stackValue->refValue = (_ILJitLocalGet(coder, n)); \
		JITC_ADJUST((coder), 1); \
	} while(0)

/*
 * Pop the top most item off the evaluation stack and store it in a local
 * value.
 */
#define _ILJitStackPopToLocal(coder, n) \
	do { \
		_ILJitStackItemNew(__value); \
		ILJitLocalSlot *__refValue; \
		_ILJitStackPop(coder, __value); \
		__refValue = (_ILJitLocalGet(coder, n)); \
		__refValue->flags |= _IL_JIT_VALUE_INITIALIZED; \
		_ILJitStackDupLocal(coder, __refValue); \
		if(__value.flags & _IL_JIT_VALUE_NULLCHECKED) \
		{ \
			_ILJitLocalStoreNotNullValue((coder), (n), _ILJitStackItemValue(__value)); \
		} \
		else \
		{ \
			_ILJitLocalStoreValue((coder), (n), _ILJitStackItemValue(__value)); \
		} \
	} while(0)

/*
 * Check the stack item for NULL and throw a NullReference exception then.
 */
#define _ILJitStackItemCheckNull(coder, stackItem) \
	do { \
		if(!((stackItem).flags & _IL_JIT_VALUE_NULLCHECKED)) \
		{ \
			jit_insn_check_null((coder)->jitFunction, _ILJitStackItemValue(stackItem)); \
			(stackItem).flags |= _IL_JIT_VALUE_NULLCHECKED; \
			if((stackItem).refValue && ((stackItem).flags & _IL_JIT_VALUE_COPYOF)) \
			{ \
				((stackItem).refValue)->flags |= _IL_JIT_VALUE_NULLCHECKED; \
			} \
			_ILJitStackSetNullChecked(coder, _ILJitStackItemValue(stackItem)); \
		} \
	} while(0)

/*
 * Store a value relative to a stack item.
 */
#define _ILJitStackItemStoreRelative(coder, dest, offset, v) \
	do { \
		if((dest).refValue && ((dest).flags & _IL_JIT_VALUE_POINTER_TO)) \
		{ \
			_ILJitStackDupLocal(coder, (dest).refValue); \
		} \
		jit_insn_store_relative((coder)->jitFunction, (dest).value, (jit_nint)(offset), (v)); \
	} while(0)

/*
 * Copy an amount of data relative to a stack item.
 */
#define _ILJitStackItemMemCpy(coder, dest, src, size) \
	do { \
		if((dest).refValue && ((dest).flags & _IL_JIT_VALUE_POINTER_TO)) \
		{ \
			_ILJitStackDupLocal(coder, (dest).refValue); \
		} \
		jit_insn_memcpy((coder)->jitFunction, (dest).value, (src), (size)); \
	} while(0)

/*
 * Move an amount of data relative to a stack item.
 */
#define _ILJitStackItemMemMove(coder, dest, src, size) \
	do { \
		if((dest).refValue && ((dest).flags & _IL_JIT_VALUE_POINTER_TO)) \
		{ \
			_ILJitStackDupLocal(coder, (dest).refValue); \
		} \
		jit_insn_memmove((coder)->jitFunction, (dest).value, (src), (size)); \
	} while(0)

/*
 * Set size bytes relative to a stack item to v.
 */
#define _ILJitStackItemMemSet(coder, dest, v, size) \
	do { \
		if((dest).refValue && ((dest).flags & _IL_JIT_VALUE_POINTER_TO)) \
		{ \
			_ILJitStackDupLocal(coder, (dest).refValue); \
		} \
		jit_insn_memset((coder)->jitFunction, (dest).value, (v), (size)); \
	} while(0)

#else /* !_IL_JIT_OPTIMIZE_LOCALS */

/*
 * Load the parameter n into the stack item s..
 */
#define _ILJitStackItemLoadArg(coder, s, n) \
	do { \
		(s) = _ILJitParamValue(coder, n); \
	} while(0)

/*
 * Check if the stack item has to be duplicated on the first invokation of
 * a label.
 */
#define _ILJitStackItemNeedsDupOnLabel(stackItem)	(0)

/*
 * Flag all occurances of the value (v) null checked on the stack.
 * This is a NOP without optimizations enabled.
 */
#define _ILJitStackSetNullChecked(coder, v)

/*
 * Take the actions needed to preserve items on the stack from changing if the
 * value this stackitem points to is modified during a call.
 * This is a NOP without optimizations enabled.
 */
#define _ILJitStackHandleCallByRefArg(coder, stackItem)

/*
 * Set the value in a stack item.
 * The only change allowed for the value are type cnversions.
 */
#define _ILJitStackItemSetValue(stackItem, v) \
	do { \
		(stackItem) = (v); \
	} while(0)

/*
 * Get the top most item from the evaluation stack.
 */
#define _ILJitStackGetTop(coder, value) \
	do { \
		(value) = (coder)->jitStack[(coder)->stackTop - 1]; \
	} while(0)

/*
 * Pop the top most item off the evaluation stack.
 */
#define _ILJitStackPop(coder, value) \
	do { \
		_ILJitStackGetTop(coder, value); \
		_ILJitStackRemoveTop(coder); \
	} while(0)

/*
 * Push the item onto the evaluation stack.
 */
#define _ILJitStackPush(coder, value) \
	do { \
		(coder)->jitStack[(coder)->stackTop] = _ILJitValueConvertToStackType((coder)->jitFunction, value); \
		JITC_ADJUST((coder), 1); \
	} while(0)

/*
 * Push the value onto the evaluation stack.
 */
#define _ILJitStackPushValue(coder, value) _ILJitStackPush(coder, value)

/*
 * Push the value known to be not null onto the evaluation stack.
 */
#define _ILJitStackPushNotNullValue(coder, value) _ILJitStackPush(coder, value)

/*
 * Duplicate the top most item on the evaluation stack.
 */
#define _ILJitStackDup(coder) \
	do { \
		_ILJitStackItemNew(__value) ; \
		_ILJitStackGetTop(coder, __value); \
		__value = jit_insn_dup((coder)->jitFunction, __value); \
		_ILJitStackPush(coder, __value); \
	} while(0)

/*
 * Push the argument onto the evaluation stack.
 */
#define _ILJitStackPushArg(coder, n) \
	do { \
		_ILJitStackItemNew(__value) ; \
		__value = _ILJitParamValue(coder, n); \
		__value = jit_insn_dup((coder)->jitFunction, __value); \
		_ILJitStackPush(coder, __value); \
	} while(0)

/*
 * Push the address of an argument onto the evaluation stack.
 */
#define _ILJitStackPushAddressOfArg(coder, n) \
	do { \
		_ILJitStackItemNew(__value) ; \
		__value = _ILJitParamGetPointerTo(coder, n); \
		_ILJitStackPush(coder, __value); \
	} while(0)

/*
 * Pop the top most item off the evaluation stack and store it in an argument.
 */
#define _ILJitStackPopToArg(coder, n) \
	do { \
		_ILJitStackItemNew(__value) ; \
		_ILJitStackPop(coder, __value); \
		_ILJitParamStoreValue((coder), (n), __value); \
	} while(0)

/*
 * Push the local value onto the evaluation stack.
 */
#define _ILJitStackPushLocal(coder, n) \
	do { \
		_ILJitStackItemNew(__value) ; \
		__value = _ILJitLocalValue(coder, n); \
		__value = jit_insn_dup((coder)->jitFunction, __value); \
		_ILJitStackPush(coder, __value); \
	} while(0)

/*
 * Push the address of a local value onto the evaluation stack.
 */
#define _ILJitStackPushAddressOfLocal(coder, n) \
	do { \
		_ILJitStackItemNew(__value) ; \
		__value = _ILJitLocalGetPointerTo(coder, n); \
		_ILJitStackPush(coder, __value); \
	} while(0)

/*
 * Pop the top most item off the evaluation stack and store it in a local
 * value.
 */
#define _ILJitStackPopToLocal(coder, n) \
	do { \
		_ILJitStackItemNew(__value) ; \
		_ILJitStackPop(coder, __value); \
		_ILJitLocalStoreValue((coder), (n), __value); \
	} while(0)

/*
 * Check the stack item for NULL and throw a NullReference exception then.
 */
#define _ILJitStackItemCheckNull(coder, stackItem) \
	do { \
		jit_insn_check_null((coder)->jitFunction, _ILJitStackItemValue(stackItem)); \
	} while(0)

/*
 * Store a value relative to a stack item.
 */
#define _ILJitStackItemStoreRelative(coder, dest, offset, v) \
	do { \
		jit_insn_store_relative((coder)->jitFunction, (dest), (jit_nint)(offset), (v)); \
	} while(0)

/*
 * Copy an amount of data relative to s stack item.
 */
#define _ILJitStackItemMemCpy(coder, dest, src, size) \
	do { \
		jit_insn_memcpy((coder)->jitFunction, (dest), (src), (size)); \
	} while(0)

/*
 * Move an amount of data relative to a stack item.
 */
#define _ILJitStackItemMemMove(coder, dest, src, size) \
	do { \
		jit_insn_memmove((coder)->jitFunction, (dest), (src), (size)); \
	} while(0)

/*
 * Set size bytes relative to a stack item to v.
 */
#define _ILJitStackItemMemSet(coder, dest, v, size) \
	do { \
		jit_insn_memset((coder)->jitFunction, (dest), (v), (size)); \
	} while(0)

#endif /* _IL_JIT_OPTIMIZE_LOCALS */

#endif	/* IL_JITC_FUNCTIONS */

#ifdef IL_JITC_CODE

/*
 * Refresh the coder's notion of the stack contents.
 */
static void JITCoder_StackRefresh(ILCoder *coder, ILEngineStackItem *stack,
							      ILUInt32 stackSize)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#ifdef	_IL_JIT_ENABLE_INLINE
	if(jitCoder->currentInlineContext)
	{
		stackSize += jitCoder->currentInlineContext->stackBase;
	}
#endif	/* _IL_JIT_ENABLE_INLINE */
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"StackRefresh: %i\n", 
			stackSize);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	jitCoder->stackTop = stackSize;
}

/*
 * Duplicate the top element on the stack.
 */
static void JITCoder_Dup(ILCoder *coder, ILEngineType engineType, ILType *type)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"Dup: \n");
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackDup(jitCoder);
}

/*
 * Pop the top element on the stack.
 */
static void JITCoder_Pop(ILCoder *coder, ILEngineType engineType, ILType *type)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	_ILJitStackRemoveTop(jitCoder);
}

#endif	/* IL_JITC_CODE */
