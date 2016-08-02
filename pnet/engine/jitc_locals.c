/*
 * jitc_locals.c - Handle function arguments and locals during code generation.
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

#ifdef IL_JITC_DECLARATIONS

/*
 * Define the structure of a local/argument slot.
 */
typedef struct _tagILJitLocalSlot ILJitLocalSlot;
struct _tagILJitLocalSlot
{
	ILJitValue	value;			/* the ILJitValue */
	ILJitValue	refValue;		/* the address of the value */
	ILUInt32	flags;			/* State of the local/arg. */
};

#define ILJitLocalSlotValue(localSlot)	(localSlot).value

/*
 * Define the structure for managing the local slots.
 */
typedef struct _tagILJitLocalSlots ILJitLocalSlots;
struct _tagILJitLocalSlots
{
	ILJitLocalSlot *slots;		/* Pointer to the slots. */
	int				numSlots;	/* Number of used slots. */
	int				maxSlots;	/* Number of allocated slots. */
};

/*
 * Initialize all not yet initialized values in the local slots to 0.
 * Returns 0 on failure.
 */
static int _ILJitLocalSlotsInitLocals(ILJITCoder *jitCoder,
									  ILJitLocalSlots *localSlots);

#define _ILJitLocalSlotsInit(s) \
	do { \
		(s).slots = 0;		\
		(s).numSlots = 0;	\
		(s).maxSlots = 0;	\
	} while (0);

#define _ILJitLocalSlotsDestroy(s) \
	do { \
		if((s).slots)			\
		{						\
			ILFree((s).slots);	\
		}						\
		(s).slots = 0;			\
		(s).numSlots = 0;		\
		(s).maxSlots = 0;		\
	} while (0);

/*
 * definitions used in the slot's flags
 */
#define _IL_JIT_VALUE_NULLCHECKED 	0x00000001
#define _IL_JIT_VALUE_INITIALIZED 	0x00000002
#define _IL_JIT_VALUE_PROTECT	 	0x00000004
#define _IL_JIT_VALUE_LOCAL_MASK 	0x000000FF

/*
 * additional flags used on the evaluation stack.
 */
#define _IL_JIT_VALUE_COPYOF		0x00000100	/* complete copy of the local/arg value */
#define _IL_JIT_VALUE_POINTER_TO	0x00000200	/* pointer to the local/arg value */

/*
 * Set flags for the given local slot.
 */
#define _ILJitLocalSlotSetFlags(s, f)	((s).flags |= (f))

/*
 * Reset flags for the given local slot.
 */
#define _ILJitLocalSlotResetFlags(s, f) ((s).flags &= ~(f))

/*
 * Get a specific slot from a ILJitLocalSlots structure.
 */
#define _ILJitLocalSlotFromSlots(localSlots, n)	((localSlots).slots[(n)])

/*
 * Set flags for all values in the given local slots.
 */
#define _ILJitLocalSlotsSetFlags(s, f)	\
	do { \
		ILInt32 __current; \
		for(__current = 0; __current < (s).numSlots; __current++) \
		{ \
			_ILJitLocalSlotSetFlags(_ILJitLocalSlotFromSlots(s, __current), f); \
		} \
	} while (0)

/*
 * Reset flags for all values in the given local slots.
 */
#define _ILJitLocalSlotsResetFlags(s, f)	\
	do { \
		ILInt32 __current; \
		for(__current = 0; __current < (s).numSlots; __current++) \
		{ \
			_ILJitLocalSlotResetFlags(_ILJitLocalSlotFromSlots(s, __current), f); \
		} \
	} while (0)

/*
 * Set flags for all argument / local values in the jit coder.
 */
#define _ILJitCoderLocalsSetFlags(c, f)	\
	do { \
		_ILJitLocalSlotsSetFlags((c)->jitParams, f); \
		_ILJitLocalSlotsSetFlags((c)->jitLocals, f); \
	} while (0)
	
/*
 * Reset flags for all argument / local values in the jit coder.
 */
#define _ILJitCoderLocalsResetFlags(c, f)	\
	do { \
		_ILJitLocalSlotsResetFlags((c)->jitParams, f); \
		_ILJitLocalSlotsResetFlags((c)->jitLocals, f); \
	} while (0)

#ifdef _IL_JIT_OPTIMIZE_LOCALS

#define _ILJitValuesResetNullChecked(c)	\
	_ILJitCoderLocalsResetFlags(c, _IL_JIT_VALUE_NULLCHECKED)

#else

#define _ILJitValuesResetNullChecked(c)

#endif	/* _IL_JIT_OPTIMIZE_LOCALS */

#ifdef	_IL_JIT_ENABLE_INLINE

/*
 * Set flags for all argument / local values in the inline context.
 */
#define _ILJitInlineContextLocalsSetFlags(c, f)	\
	do { \
		_ILJitLocalSlotsSetFlags((c)->jitParams, f); \
		_ILJitLocalSlotsSetFlags((c)->jitLocals, f); \
	} while (0)
	
/*
 * Reset flags for all argument / local values in the inline context.
 */
#define _ILJitInlineContextLocalsResetFlags(c, f)	\
	do { \
		_ILJitLocalSlotsResetFlags((c)->jitParams, f); \
		_ILJitLocalSlotsResetFlags((c)->jitLocals, f); \
	} while (0)

#endif	/* _IL_JIT_ENABLE_INLINE */

/*
 * Allocate enough space for "n" slots.
 */
#define	_ILJitLocalsAlloc(s, n)	\
	do { \
		ILUInt32 temp = (ILUInt32)(((n) + 7) & ~7); \
		if(temp > (s).maxSlots) \
		{ \
			ILJitLocalSlot *newSlots = \
				(ILJitLocalSlot *)ILRealloc((s).slots, \
									  temp * sizeof(ILJitLocalSlot)); \
			if(!newSlots) \
			{ \
				return 0; \
			} \
			(s).slots = newSlots; \
			(s).maxSlots = temp; \
		} \
	} while (0)

/*
 * Get the slot for a local value.
 */
#ifdef	_IL_JIT_ENABLE_INLINE
#define _ILJitLocalGet(coder, n) \
	({ \
		ILJitLocalSlot *localSlot; \
		if((coder)->currentInlineContext) \
		{ \
			localSlot = &_ILJitLocalSlotFromSlots((coder)->currentInlineContext->jitLocals, (n)); \
		} \
		else \
		{ \
			localSlot = &_ILJitLocalSlotFromSlots((coder)->jitLocals, (n)); \
		} \
		localSlot; \
	})

#else
#define _ILJitLocalGet(coder, n) (&_ILJitLocalSlotFromSlots((coder)->jitLocals, (n)))
#endif

/*
 * Access the flags member of a local slot.
 */
#define _ILJitLocalFlags(coder, n) _ILJitLocalGet((coder), (n))->flags

/*
 * Access the ptrToValue member of a locals slot.
 */
#define _ILJitLocalPointer(coder, n) _ILJitLocalGet((coder), (n))->refValue

/*
 * Access the value member of a locals slot.
 */
#define _ILJitLocalValue(coder, n) _ILJitLocalGet((coder), (n))->value

/*
 * Get the slot for a param value.
 */
#ifdef	_IL_JIT_ENABLE_INLINE
#define _ILJitParamGet(coder, n) \
	({ \
		ILJitLocalSlot *localSlot; \
		if((coder)->currentInlineContext) \
		{ \
			localSlot = &_ILJitLocalSlotFromSlots((coder)->currentInlineContext->jitParams, (n)); \
		} \
		else \
		{ \
			localSlot = &_ILJitLocalSlotFromSlots((coder)->jitParams, (n)); \
		} \
		localSlot; \
	})

#else
#define _ILJitParamGet(coder, n) (&_ILJitLocalSlotFromSlots((coder)->jitParams, (n)))
#endif

/*
 * Access the flags member of a local slot.
 */
#define _ILJitParamFlags(coder, n) _ILJitParamGet((coder), (n))->flags

/*
 * Access the ptrToValue member of a locals slot.
 */
#define _ILJitParamPointer(coder, n) _ILJitParamGet((coder), (n))->refValue

/*
 * Access the value member of a locals slot.
 */
#define _ILJitParamValue(coder, n) _ILJitParamGet((coder), (n))->value

#ifdef	_IL_JIT_ENABLE_INLINE

/*
 * Create a new jit_value_t with the type of the existing jit_value_t in the
 * local slot and replace the existing one with the new one.
 * Clear the protect flag afterwards.
 */
static int _ILJitLocalSlotNewValue(ILJITCoder *jitCoder,
                                   ILJitLocalSlot *localSlot);

/*
 * Duplicate the jit_value_t in a local slot and clear the protect flag.
 */
static int _ILJitLocalSlotDupValue(ILJITCoder *jitCoder,
                                   ILJitLocalSlot *localSlot);

#define _ILJitLocalsInitInlineContext(coder, inlineContext) \
		_ILJitLocalSlotsInitLocals((coder), &((inlineContext)->jitLocals))

#endif	/* _IL_JIT_ENABLE_INLINE */

#endif /* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_CODER_INSTANCE

	/* Members to manage the fixed arguments. */
	ILJitLocalSlots	jitParams;

	/* Members to manage the local variables. */
	ILJitLocalSlots jitLocals;
#ifdef _IL_JIT_OPTIMIZE_INIT_LOCALS
	int				localsInitialized;
#endif

#endif	/* IL_JITC_CODER_INSTANCE */

#ifdef	IL_JITC_CODER_INIT

	/* Initialize the parameter management. */
	_ILJitLocalSlotsInit(coder->jitParams)

	/* Initialize the locals management. */
	_ILJitLocalSlotsInit(coder->jitLocals)

#endif	/* IL_JITC_CODER_INIT */

#ifdef	IL_JITC_CODER_DESTROY

	_ILJitLocalSlotsDestroy(coder->jitLocals);

	_ILJitLocalSlotsDestroy(coder->jitParams);

#endif	/* IL_JITC_CODER_DESTROY */

#ifdef IL_JITC_FUNCTIONS

/*
 * Get the pointer to a local.
 * The pointer is created on the first time.
 */
static ILJitValue _ILJitLocalGetPointerTo(ILJITCoder *coder,
										  ILUInt32 localNum)
{
	ILJitLocalSlot *slot = _ILJitLocalGet(coder, localNum);

	/*
	if(!slot->refValue)
	{	
		slot->refValue = jit_insn_address_of(coder->jitFunction, slot->value);
	}
	return slot->refValue;
	*/
	return jit_insn_address_of(coder->jitFunction, slot->value);
}

/*
 * Get the pointer to a parameter.
 * The pointer is created on the first time.
 */
static ILJitValue _ILJitParamGetPointerTo(ILJITCoder *coder,
										  ILUInt32 paramNum)
{
	ILJitLocalSlot *slot = _ILJitParamGet(coder, paramNum);

#ifdef	_IL_JIT_ENABLE_INLINE
	if(slot->flags & _IL_JIT_VALUE_PROTECT)
	{
		if(!(_ILJitLocalSlotDupValue(coder, slot)))
		{
			return 0;
		}
	}
#endif
	/*
	if(!slot->refValue)
	{	
		slot->refValue = jit_insn_address_of(coder->jitFunction, slot->value);
	}
	return slot->refValue;
	*/
	return jit_insn_address_of(coder->jitFunction, slot->value);
}

/*
 * Initialize the local value to 0.
 */
static int _ILJitLocalInit(ILJITCoder *coder, ILJitLocalSlot *slot)
{
	if((slot->flags & _IL_JIT_VALUE_INITIALIZED) == 0)
	{
		ILJitType type = jit_value_get_type(slot->value);

		if(!jit_type_is_struct(type) && !jit_type_is_union(type))
		{
			int typeKind = jit_type_get_kind(type);
			ILJitValue constant = 0;

			if(_JIT_TYPEKIND_IS_FLOAT(typeKind))
			{
				if(!(constant = jit_value_create_nfloat_constant(coder->jitFunction,
															type,
															(jit_nfloat)0)))
				{
					return 0;
				}
				jit_insn_store(coder->jitFunction, slot->value, constant);
			}
			else
			{
				if(_JIT_TYPEKIND_IS_LONG(typeKind))
				{
					if(!(constant = jit_value_create_long_constant(coder->jitFunction,
															  type, (jit_long)0)))
					{
						return 0;
					}
					jit_insn_store(coder->jitFunction, slot->value, constant);
				}
				else
				{
					if(!(constant = jit_value_create_nint_constant(coder->jitFunction,
															  type, (jit_nint)0)))
					{
						return 0;
					}
					jit_insn_store(coder->jitFunction, slot->value, constant);
				}
			}
		}
		slot->flags |= _IL_JIT_VALUE_INITIALIZED;
	}
	return 1;
}

/*
 * Initialize all not yet initialized values in the local slots to 0.
 * Returns 0 on failure.
 */
static int _ILJitLocalSlotsInitLocals(ILJITCoder *jitCoder,
									  ILJitLocalSlots *localSlots)
{
	ILUInt32 num = localSlots->numSlots;

	if(num > 0)
	{
		ILUInt32 current;
		ILJitLocalSlot *slot;

		for(current = 0; current < num; ++current)
		{
			slot = &_ILJitLocalSlotFromSlots(*localSlots, current);

			if(!_ILJitLocalInit(jitCoder, slot))
			{
				return 0;
			}
		}
	}
	return 1;
}

/*
 * Initialize the not yet initialized local values to 0 and move the
 * initialization sequence to the start of the function.
 */
static int _ILJitLocalsInit(ILJITCoder *coder)
{
	ILUInt32 num = coder->jitLocals.numSlots;

	if(num > 0)
	{
		jit_label_t startLabel = jit_label_undefined;
		jit_label_t endLabel = jit_label_undefined;

		/* 
		 * Create a new block to make sure that the start label will be
		 * placed on a new block alone with the code generated.
		 */
		if(!jit_insn_new_block(coder->jitFunction))
		{
			return 0;
		}

		if(!jit_insn_label(coder->jitFunction, &startLabel))
		{
			return 0;
		}

		if(!_ILJitLocalSlotsInitLocals(coder, &(coder->jitLocals)))
		{
			return 0;
		}

		/*
		 * Create a new block for the end label. This should be done
		 * by jit_insn_label anyways but it doesn't hurt.
		 * The case where this seems to be needed is if all locals are
		 * structs which are not initialized by now.
		 */
		if(!jit_insn_new_block(coder->jitFunction))
		{
			return 0;
		}

		if(!jit_insn_label(coder->jitFunction, &endLabel))
		{
			return 0;
		}

		if(!jit_insn_move_blocks_to_start(coder->jitFunction, startLabel,
															  endLabel))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Create the slots for the declared local variables.
 * Returns zero if out of memory.
 */
static int _ILJitLocalSlotsCreateLocals(ILJITCoder *jitCoder,
										ILJitLocalSlots *localSlots,
										ILStandAloneSig *localVarSig)
{
	if(localVarSig)
	{
		ILType *signature;
		ILType *type;
		ILJitType jitType;
		ILUInt32 num;
		ILUInt32 current;

		/* Determine the number of locals to allocate */
		if(!(signature = ILStandAloneSigGetType(localVarSig)))
		{
			return 0;
		}
		num = ILTypeNumLocals(signature);

		/* Allocate the "jitLocals" array for the variables */
		_ILJitLocalsAlloc(*localSlots, num);

		/* Create the jit values for the local variables */
		for(current = 0; current < num; ++current)
		{
			ILJitLocalSlot *local = &_ILJitLocalSlotFromSlots(*localSlots, current);

			if(!(type = ILTypeGetLocal(signature, current)))
			{
				return 0;
			}
			if(!(jitType = _ILJitGetLocalsType(type, jitCoder->process)))
			{
				return 0;
			}
			if(!(local->value = jit_value_create(jitCoder->jitFunction, jitType)))
			{
				return 0;
			}
			local->flags = 0;
			local->refValue = 0;
		}
		/* Record the number of used locals in the local slots. */
		localSlots->numSlots = num;
	}
	else
	{
		/* Set the number of used locals to 0. */
		localSlots->numSlots = 0;
	}
	return 1;
}

#ifdef IL_DEBUGGER
/*
 * Create debug mark with local's or parameter's address.
 */
static void _ILJitLocalsMarkDebug(ILJITCoder *coder, ILJitValue value,
								  jit_nint type)
{
	jit_value_t data1;
	jit_value_t data2;

	/* Make the variable accessible for debugger */
	jit_value_set_volatile(value);
	jit_value_set_addressable(value);

	data1 = jit_value_create_nint_constant(coder->jitFunction, jit_type_nint,
																		type);

	data2 = jit_insn_address_of(coder->jitFunction,	value);
	jit_insn_mark_breakpoint_variable(coder->jitFunction, data1, data2);
}
#endif /* IL_DEBUGGER */

/*
 * Create the slots for the declared local variables.
 * Returns zero if out of memory.
 */
static int _ILJitLocalsCreate(ILJITCoder *coder, ILStandAloneSig *localVarSig)
{
	if(!_ILJitLocalSlotsCreateLocals(coder, &(coder->jitLocals), localVarSig))
	{
		return 0;
	}

#ifdef IL_DEBUGGER
	if(coder->markBreakpoints)
	{
		ILUInt32 current;

		/* Set the offsets for each of the local variables */
		for(current = 0; current < coder->jitLocals.numSlots; ++current)
		{
			ILJitLocalSlot *local = &_ILJitLocalSlotFromSlots(coder->jitLocals,
																	current);

			_ILJitLocalsMarkDebug(coder, local->value,
											JIT_DEBUGGER_DATA1_LOCAL_VAR_ADDR);
		}
	}
#endif

#ifndef _IL_JIT_OPTIMIZE_INIT_LOCALS
	/* Initialize the locals. */
	if(!_ILJitLocalsInit(coder))
	{
		return 0;
	}
#endif

	return 1;
}

/*
 * Create the slots for the fixed parameters of the current function.
 * Returns 0 on error else 1.
 */
static int _ILJitParamsCreate(ILJITCoder *coder)
{
	ILJitType signature = jit_function_get_signature(coder->jitFunction);
#ifdef IL_DEBUGGER
	int markThis = 0;
#endif

	if(signature)
	{
		ILJitLocalSlot *param = 0;
		ILUInt32 numParams = jit_type_num_params(signature);

#ifdef IL_DEBUGGER
		if(coder->markBreakpoints)
		{
			markThis = ILType_HasThis(ILMethod_Signature(
							ILCCtorMgr_GetCurrentMethod(&(coder->cctorMgr))));
		}
#endif


#ifdef IL_JIT_THREAD_IN_SIGNATURE
		/* We don't include the ILExecThread in the params. */
		if(numParams > 1)
		{
			ILInt32 current = 0;

			/* Allocate the locals for the parameters */
			_ILJitLocalsAlloc(coder->jitParams, numParams - 1);

			for(current = 1; current < numParams; ++current)
			{
				param = _ILJitParamGet(coder, current - 1);

				param->value = jit_value_get_param(coder->jitFunction, current);
				param->flags = 0;
				param->refValue = 0;

#ifdef IL_DEBUGGER
				if(coder->markBreakpoints)
				{
					if(markThis)
					{
						_ILJitLocalsMarkDebug(coder, param->value,
												JIT_DEBUGGER_DATA1_THIS_ADDR);
						markThis = 0;
					}
					else
					{
						_ILJitLocalsMarkDebug(coder, param->value,
												JIT_DEBUGGER_DATA1_PARAM_ADDR);
					}
				}
#endif

			}
			coder->jitParams.numSlots = numParams - 1;
		}
#else
		if(numParams > 0)
		{
			ILInt32 current = 0;

			/* Allocate the locals for the parameters */
			_ILJitLocalsAlloc(coder->jitParams, numParams);

			for(current = 0; current < numParams; ++current)
			{
				param = _ILJitParamGet(coder, current);

				param->value = jit_value_get_param(coder->jitFunction, current);
				param->flags = 0;
				param->refValue = 0;

#ifdef IL_DEBUGGER
				if(coder->markBreakpoints)
				{
					if(markThis)
					{
						_ILJitLocalsMarkDebug(coder, param->value,
												JIT_DEBUGGER_DATA1_THIS_ADDR);
						markThis = 0;
					}
					else
					{
						_ILJitLocalsMarkDebug(coder, param->value,
												JIT_DEBUGGER_DATA1_PARAM_ADDR);
					}
				}
#endif
			}
			coder->jitParams.numSlots = numParams;
		}
#endif
		else
		{
			coder->jitParams.numSlots = 0;
		}
		return 1;
	}
	return 0;
}

/*
 * Get the value of a local slot.
 * The value in the local slot is initialized to 0 if it is not yet initialized.
 */
static ILJitValue _ILJitLocalGetValue(ILJITCoder *coder, ILUInt32 localNum)
{
	ILJitLocalSlot *slot = _ILJitLocalGet(coder, localNum);

	if((slot->flags & _IL_JIT_VALUE_INITIALIZED) == 0)
	{
		if(!_ILJitLocalInit(coder, slot))
		{
			return 0;
		}
	}
	return slot->value;
}

/*
 * Store a value in a local variable.
 */
static void _ILJitLocalStoreValue(ILJITCoder *coder, ILUInt32 localNum,
													 ILJitValue value)
{
	ILJitLocalSlot *slot = _ILJitLocalGet(coder, localNum);

	jit_insn_store(coder->jitFunction, slot->value, 
				   _ILJitValueConvertImplicit(coder->jitFunction,
											  value,
											  jit_value_get_type(slot->value)));

	slot->flags |= _IL_JIT_VALUE_INITIALIZED;
	slot->flags &= ~_IL_JIT_VALUE_NULLCHECKED;
}

#ifdef _IL_JIT_OPTIMIZE_LOCALS

/*
 * Store a value known to be not null in a local variable.
 */
static void _ILJitLocalStoreNotNullValue(ILJITCoder *coder,
										 ILUInt32 paramNum,
										 ILJitValue value)
{
	ILJitLocalSlot *slot = _ILJitLocalGet(coder, paramNum);

	jit_insn_store(coder->jitFunction, slot->value,
				   _ILJitValueConvertImplicit(coder->jitFunction,
											  value,
											  jit_value_get_type(slot->value)));

	slot->flags |= (_IL_JIT_VALUE_INITIALIZED | _IL_JIT_VALUE_NULLCHECKED);
}

#endif

/*
 * Get the value of a parameter slot.
 */
static ILJitValue _ILJitParamGetValue(ILJITCoder *coder, ILUInt32 paramNum)
{
	ILJitLocalSlot *slot = _ILJitParamGet(coder, paramNum);

	return slot->value;
}

/*
 * Store a value in a parameter.
 */
static void _ILJitParamStoreValue(ILJITCoder *coder, ILUInt32 paramNum,
													 ILJitValue value)
{
	ILJitLocalSlot *slot = _ILJitParamGet(coder, paramNum);

#ifdef	_IL_JIT_ENABLE_INLINE
	if(slot->flags & _IL_JIT_VALUE_PROTECT)
	{
		if(!(_ILJitLocalSlotNewValue(coder, slot)))
		{
			return;
		}
	}
#endif
	jit_insn_store(coder->jitFunction, slot->value,
				   _ILJitValueConvertImplicit(coder->jitFunction,
											  value,
											  jit_value_get_type(slot->value)));

	slot->flags &= ~_IL_JIT_VALUE_NULLCHECKED;
}

#ifdef _IL_JIT_OPTIMIZE_LOCALS

/*
 * Store a value known to be not null in a parameter.
 */
static void _ILJitParamStoreNotNullValue(ILJITCoder *coder,
										 ILUInt32 paramNum,
										 ILJitValue value)
{
	ILJitLocalSlot *slot = _ILJitParamGet(coder, paramNum);

#ifdef	_IL_JIT_ENABLE_INLINE
	if(slot->flags & _IL_JIT_VALUE_PROTECT)
	{
		if(!(_ILJitLocalSlotNewValue(coder, slot)))
		{
			return;
		}
	}
#endif	/* _IL_JIT_ENABLE_INLINE */
	jit_insn_store(coder->jitFunction, slot->value,
				   _ILJitValueConvertImplicit(coder->jitFunction,
											  value,
											  jit_value_get_type(slot->value)));

	slot->flags |= _IL_JIT_VALUE_NULLCHECKED;
}

#endif	/* _IL_JIT_OPTIMIZE_LOCALS */

#ifdef	_IL_JIT_ENABLE_INLINE

/*
 * Create a new jit_value_t with the type of the existing jit_value_t in the
 * local slot and replace the existing one with the new one.
 * Clear the protect flag afterwards.
 */
static int _ILJitLocalSlotNewValue(ILJITCoder *jitCoder,
                                   ILJitLocalSlot *localSlot)
{
	ILJitType type;
	ILJitValue value;

	if(!(type = jit_value_get_type(localSlot->value)))
	{
		return 0;
	}
	if(!(value = jit_value_create(jitCoder->jitFunction, type)))
	{
		return 0;
	}
	localSlot->value = value;
	localSlot->refValue = 0;
	localSlot->flags &= ~_IL_JIT_VALUE_PROTECT;
	return 1;
}

/*
 * Duplicate the jit_value_t in a local slot and clear the protect flag.
 */
static int _ILJitLocalSlotDupValue(ILJITCoder *jitCoder,
                                   ILJitLocalSlot *localSlot)
{
	ILJitType type;
	ILJitValue value;

	if(!(type = jit_value_get_type(localSlot->value)))
	{
		return 0;
	}
	if(!(value = jit_value_create(jitCoder->jitFunction, type)))
	{
		return 0;
	}
	if(!(jit_insn_store(jitCoder->jitFunction, value, localSlot->value)))
	{
		return 0;
	}
	localSlot->value = value;
	localSlot->refValue = 0;
	localSlot->flags &= ~_IL_JIT_VALUE_PROTECT;
	return 1;
}

/*
 * Duplicate all values in an inline context that are marked to protect.
 * This has to be done for example before if branch or a branch target is
 * emitted.
 */
static int _ILJitLocalSlotsHandleProtectedValues(ILJITCoder *jitCoder,
												 ILJITCoderInlineContext *inlineContext)
{
	ILJitLocalSlot *slot;
	ILInt32 current;

	for(current = 0; current < inlineContext->jitParams.numSlots; current++)
	{
		slot = &_ILJitLocalSlotFromSlots(inlineContext->jitParams, current);

		if(slot->flags & _IL_JIT_VALUE_PROTECT)
		{
			if(!(_ILJitLocalSlotDupValue(jitCoder, slot)))
			{
				return 0;
			}
		}
	}
	return 1;
}

/*
 * Setup the arguments for an inlined function.
 */
static int _ILJitLocalSlotsSetupInlineArgs(ILJITCoder *jitCoder,
										   ILJITCoderInlineContext *inlineContext,
										   ILJitValue this,
										   ILJitStackItem *args,
										   ILInt32 numArgs)
{
	ILInt32 current;
	ILInt32 currentArg;

	if(this)
	{
		ILJitLocalSlot *arg;

		_ILJitLocalsAlloc(inlineContext->jitParams, numArgs + 1);

		arg = &_ILJitLocalSlotFromSlots(inlineContext->jitParams, 0);

		arg->value = this;
		arg->refValue = 0;
		arg->flags = _IL_JIT_VALUE_NULLCHECKED;

		current = 1;
	}
	else
	{
		_ILJitLocalsAlloc(inlineContext->jitParams, numArgs);
		current = 0;
	}

	for(currentArg = 0; currentArg < numArgs; currentArg++)
	{
		ILJitLocalSlot *arg = &_ILJitLocalSlotFromSlots(inlineContext->jitParams,
														currentArg);

		arg->value = _ILJitStackItemValue(args[currentArg]);
		arg->refValue = 0;
#ifdef _IL_JIT_OPTIMIZE_LOCALS
		if(jit_value_is_constant(arg->value))
		{
			arg->flags = ((args[currentArg].flags & _IL_JIT_VALUE_LOCAL_MASK) | _IL_JIT_VALUE_PROTECT);
		}
		else if(args[currentArg].flags & _IL_JIT_VALUE_COPYOF)
		{
			arg->flags = ((args[currentArg].flags & _IL_JIT_VALUE_LOCAL_MASK) | _IL_JIT_VALUE_PROTECT);
		}
		else
		{
			arg->flags = (args[currentArg].flags & _IL_JIT_VALUE_LOCAL_MASK);
		}
#else
		if(jit_value_is_constant(arg->value))
		{
			arg->flags = _IL_JIT_VALUE_PROTECT;
		}
		else
		{
			arg->flags = 0;
		}
#endif
		/* TODO: look at the bug that happens without this. */
		_ILJitLocalSlotDupValue(jitCoder, arg);
		current++;
	}
	return 1;
}

#endif	/* _IL_JIT_ENABLE_INLINE */

#endif /* IL_JITC_FUNCTIONS */

#ifdef	IL_JITC_INLINE_CONTEXT_INSTANCE

	/* Members to manage the fixed arguments. */
	ILJitLocalSlots	jitParams;

	/* Members to manage the local variables. */
	ILJitLocalSlots jitLocals;

#endif	/* IL_JITC_INLINE_CONTEXT_INSTANCE */

#ifdef	IL_JITC_INLINE_CONTEXT_INIT

	/* Initialize the parameter management. */
	_ILJitLocalSlotsInit(inlineContext->jitParams)

	/* Initialize the locals management. */
	_ILJitLocalSlotsInit(inlineContext->jitLocals)

#endif	/* IL_JITC_INLINE_CONTEXT_INIT */

#ifdef	IL_JITC_INLINE_CONTEXT_DESTROY

	_ILJitLocalSlotsDestroy(inlineContext->jitLocals)

	_ILJitLocalSlotsDestroy(inlineContext->jitParams)

#endif	/* IL_JITC_INLINE_CONTEXT_DESTROY */

