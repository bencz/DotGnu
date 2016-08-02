/*
 * jitc_labels.c - Jit coder label management routines.
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
 * declaration of the different label types.
 */
#define _IL_JIT_LABEL_NORMAL 1
#define _IL_JIT_LABEL_STARTCATCH 2
#define _IL_JIT_LABEL_STARTFINALLY 4
#define _IL_JIT_LABEL_STARTFILTER 8

/*
 * Define the structure of a JIT label.
 */
typedef struct _tagILJITLabel ILJITLabel;
struct _tagILJITLabel
{
	ILUInt32	address;		/* Address in the IL code */
	jit_label_t	label;			/* the libjit label */
	ILJITLabel *next;			/* Next label block */
	int			labelType;      /* type of the label. */
	int			stackSize;		/* No. of elements on the stack. */
	ILJitValue *jitStack;		/* Values on the stack. */
};

#endif /* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_CODER_INSTANCE

	/* Handle the labels. */
	ILMemPool		labelPool;
	ILJITLabel     *labelList;
	int				labelOutOfMemory;
	ILMemStack		stackStates;

	/* Handle the switch table. */
	ILJitValue		switchValue;
	int				numSwitch;
	int				maxSwitch;
	jit_label_t		*switchLabels;

#endif	/* IL_JITC_CODER_INSTANCE */

#ifdef	IL_JITC_CODER_INIT

	/* Init the label stuff. */
	ILMemPoolInit(&(coder->labelPool), sizeof(ILJITLabel), 8);
	coder->labelList = 0;
	coder->labelOutOfMemory = 0;

	/* Init the switch stuff. */
	coder->switchValue = 0;
	coder->numSwitch = 0;
	coder->maxSwitch = 0;
	coder->switchLabels = NULL;

#endif	/* IL_JITC_CODER_INIT */

#ifdef	IL_JITC_CODER_DESTROY

	if(coder->switchLabels)
	{
		ILFree(coder->switchLabels);
	}
	ILMemPoolDestroy(&(coder->labelPool));

#endif	/* IL_JITC_CODER_DESTROY */

#ifdef IL_JITC_FUNCTIONS

/*
 * Save the current jitStack status to the label.
 * This is done when the label is referenced the first time.
 */
static int _ILJitLabelSaveStack(ILJITCoder *coder, ILJITLabel *label)
{
	int coderStackHeight = _ILJitStackHeight(coder);
#ifdef	_IL_JIT_ENABLE_INLINE
	int coderStackBase;

	if(coder->currentInlineContext)
	{
		coderStackBase = coder->currentInlineContext->stackBase;
		coderStackHeight -= coderStackBase;
	}
	else
	{
		coderStackBase = 0;
	}
#else	/* !_IL_JIT_ENABLE_INLINE */
	int coderStackBase = 0;
#endif	/* !_IL_JIT_ENABLE_INLINE */

	if(((label->labelType & (_IL_JIT_LABEL_NORMAL |
							 _IL_JIT_LABEL_STARTCATCH)) != 0) &&
		(coderStackHeight > 0))
	{
		int current = 0;
		ILJitValue *stack = ILMemStackAllocItem(&(coder->stackStates),
									coderStackHeight * sizeof(ILJitValue));
		if(!stack)
		{
			return 0;
		}
		/* Now save the current stack state. */
		for(current = 0; current < coderStackHeight; current++)
		{
			ILJitStackItem *stackItem = _ILJitStackItemGet(coder, coderStackBase + current);

			stack[current] = _ILJitStackItemValue(*stackItem);
			if(jit_value_is_constant(_ILJitStackItemValue(*stackItem)))
			{
				/* We have to handle this case different. */
				/* Create a local value of the type of the constant. */
				ILJitValue temp = jit_value_create(coder->jitFunction,
												   jit_value_get_type(_ILJitStackItemValue(*stackItem)));
				/* and store the value of the constant in the new temporary. */
				jit_insn_store(coder->jitFunction, temp, _ILJitStackItemValue(*stackItem));
				/* Now replace the constant with the new temporary. */
				stack[current] = temp;
				_ILJitStackItemSetValue(*stackItem, temp);
			}
			else if(_ILJitStackItemNeedsDupOnLabel(*stackItem))
			{
				ILJitValue temp = jit_insn_dup(coder->jitFunction,
											   _ILJitStackItemValue(*stackItem));
				stack[current] = temp;
				_ILJitStackItemSetValue(*stackItem, temp);
			}
		}
		label->jitStack = stack;
		label->stackSize = coderStackHeight;
	}
	return 1;
}

/*
 * Merge the current jitStack status with the one saved in the label.
 */
static int _ILJitLabelMergeStack(ILJITCoder *coder, ILJITLabel *label)
{
	int coderStackHeight = _ILJitStackHeight(coder);
#ifdef	_IL_JIT_ENABLE_INLINE
	int coderStackBase;

	if(coder->currentInlineContext)
	{
		coderStackBase = coder->currentInlineContext->stackBase;
		coderStackHeight -= coderStackBase;
	}
	else
	{
		coderStackBase = 0;
	}
#else	/* !_IL_JIT_ENABLE_INLINE */
	int coderStackBase = 0;
#endif	/* !_IL_JIT_ENABLE_INLINE */

	if(label->labelType & (_IL_JIT_LABEL_NORMAL | _IL_JIT_LABEL_STARTCATCH))
	{
		/* Verify that the stack sizes match. */
		if(coderStackHeight != label->stackSize)
		{
			fprintf(stdout, "Stack sizes don't match!\n");
			/* return 0; */
		}
		if(coderStackHeight > 0)
		{
			int current = 0;

			/* Now save the current stack state. */
			for(current = 0; current < coderStackHeight; current++)
			{
				ILJitStackItem *stackItem = _ILJitStackItemGet(coder, coderStackBase + current);

				if(_ILJitStackItemValue(*stackItem) != label->jitStack[current])
				{
					/* store the changed value to the saved stack. */
					jit_insn_store(coder->jitFunction,
								   label->jitStack[current],
								   _ILJitStackItemValue(*stackItem));
				}
			}
		}
	}
	return 1;
}

/*
 * Restore the stack from the label to the coder.
 */
static void _ILJitLabelRestoreStack(ILJITCoder *coder, ILJITLabel *label)
{
	int coderStackHeight = _ILJitStackHeight(coder);
#ifdef	_IL_JIT_ENABLE_INLINE
	int coderStackBase;

	if(coder->currentInlineContext)
	{
		coderStackBase = coder->currentInlineContext->stackBase;
		coderStackHeight -= coderStackBase;
	}
	else
	{
		coderStackBase = 0;
	}
#else	/* !_IL_JIT_ENABLE_INLINE */
	int coderStackBase = 0;
#endif	/* !_IL_JIT_ENABLE_INLINE */

	/* Verify that the stack sizes match. */
	if(coderStackHeight != label->stackSize)
	{
		fprintf(stdout, "Stack sizes don't match!\n");
		/* return 0; */
	}
	if(coderStackHeight > 0)
	{
		int current = 0;

		/* Now restore the stack state. */
		for(current = 0; current < coderStackHeight; current++)
		{
			ILJitStackItem *stackItem = _ILJitStackItemGet(coder, coderStackBase + current);

			_ILJitStackItemInitWithValue(*stackItem, label->jitStack[current]);
		}
	}
	coder->stackTop = coderStackBase + label->stackSize;
}

/*
 * Look for an existing label for the specified IL address.
 * Returns 0 if there is no label for this address.
 */
static ILJITLabel *_ILJitLabelFind(ILJITCoder *coder, ILUInt32 address)
{
#ifdef	_IL_JIT_ENABLE_INLINE
	ILJITLabel *label = coder->labelList;

	if(coder->currentInlineContext)
	{
		label = coder->currentInlineContext->labelList;
	}
	else
	{
		label = coder->labelList;
	}
#else	/* !_IL_JIT_ENABLE_INLINE */
	ILJITLabel *label = coder->labelList;
#endif	/* !_IL_JIT_ENABLE_INLINE */

	while(label != 0)
	{
		if(label->address == address)
		{
			return label;
		}
		label = label->next;
	}
	return 0;
}

/*
 * Look for a label for a specific IL address.  Create
 * a new label if necessary.
 * This function handles the jit stack changes on label invocation too.
 */
static ILJITLabel *_ILJitLabelGet(ILJITCoder *coder, ILUInt32 address,
													 int labelType)
{
	ILJITLabel *label = _ILJitLabelFind(coder, address);
;
	if(!label)
	{
	#ifdef	_IL_JIT_ENABLE_INLINE
		if(coder->currentInlineContext)
		{
			label = ILMemPoolAlloc(&(coder->currentInlineContext->labelPool), ILJITLabel);
		}
		else
		{
			label = ILMemPoolAlloc(&(coder->labelPool), ILJITLabel);
		}
	#else	/* !_IL_JIT_ENABLE_INLINE */
		label = ILMemPoolAlloc(&(coder->labelPool), ILJITLabel);
	#endif	/* !_IL_JIT_ENABLE_INLINE */
		if(label)
		{
			label->stackSize = 0;
			label->jitStack = 0;
			label->address = address;
			label->label = jit_label_undefined;
			label->labelType = labelType;
			if(!_ILJitLabelSaveStack(coder, label))
			{
				coder->labelOutOfMemory = 1;
				return 0;
			}
		#ifdef	_IL_JIT_ENABLE_INLINE
			if(coder->currentInlineContext)
			{
				label->next = coder->currentInlineContext->labelList;
				coder->currentInlineContext->labelList = label;
				_ILJitLocalSlotsHandleProtectedValues(coder,
													  coder->currentInlineContext);
			}
			else
			{
				label->next = coder->labelList;
				coder->labelList = label;
			}
		#else	/* !_IL_JIT_ENABLE_INLINE */
			label->next = coder->labelList;
			coder->labelList = label;
		#endif	/* !_IL_JIT_ENABLE_INLINE */
#ifdef _IL_JIT_OPTIMIZE_INIT_LOCALS
			if((coder->localsInitialized == 0) && (address != 0))
			{
				/* Initialize the locals. */
				if(!_ILJitLocalsInit(coder))
				{
					return 0;
				}
				coder->localsInitialized = 1;
			}
#endif
		}
		else
		{
			coder->labelOutOfMemory = 1;
		}
	}
	else
	{
		if(!_ILJitLabelMergeStack(coder, label))
		{
			/* We have a stack size mismatch!!! */
			coder->labelOutOfMemory = 1;
			return 0;
		}
	}
	return label;
}

#endif /* IL_JITC_FUNCTIONS */

#ifdef	IL_JITC_INLINE_CONTEXT_INSTANCE

	/* Handle the labels. */
	ILMemPool		labelPool;
	ILJITLabel     *labelList;

#endif	/* IL_JITC_INLINE_CONTEXT_INSTANCE */

#ifdef	IL_JITC_INLINE_CONTEXT_INIT

	/* Init the label stuff. */
	ILMemPoolInit(&(inlineContext->labelPool), sizeof(ILJITLabel), 8);
	inlineContext->labelList = 0;

#endif	/* IL_JITC_INLINE_CONTEXT_INIT */

#ifdef	IL_JITC_INLINE_CONTEXT_DESTROY

	ILMemPoolDestroy(&(inlineContext->labelPool));

#endif	/* IL_JITC_INLINE_CONTEXT_DESTROY */

