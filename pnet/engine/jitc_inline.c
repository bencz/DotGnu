/*
 * jitc_inline.c - JIT Coder support for inlining IL methods.
 *
 * Copyright (C) 2006  Southern Storm Software, Pty Ltd.
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

#ifdef	_IL_JIT_ENABLE_INLINE

/*
 * Maximum method code size to inline.
 */
#define _IL_JIT_MAX_INLINE_CODELEN  32

#ifdef	IL_JITC_DECLARATIONS

/*
 * Forward declaration of the inline context.
 */
typedef struct _tagILJitCoderInlineContext ILJITCoderInlineContext;

/*
 * Structure to hold the information for inlining an IL method.
 */
struct _tagILJitCoderInlineContext
{
	/* The previous context level. */
	ILJITCoderInlineContext	   *prevContext;

	/* The method inlined. */
	ILMethod				   *inlineMethod;

	/* The return value for the inlined method. */
	ILJitValue					returnValue;

	/* The stack base on entry of the method. */
	ILInt32						stackBase;

	/* The label to jump to on return. */
	jit_label_t					returnLabel;

#undef	IL_JITC_DECLARATIONS
#define IL_JITC_INLINE_CONTEXT_INSTANCE
#include "jitc_labels.c"
#include "jitc_locals.c"
#undef IL_JITC_INLINE_CONTEXT_INSTANCE
#define	IL_JITC_DECLARATIONS
};

/*
 * Initialize a new inline context.
 */
static void _ILJitCoderInlineContextInit(ILJITCoderInlineContext *inlineContext);

/*
 * Destroy an inline context.
 */
static void _ILJitCoderInlineContextDestroy(ILJITCoderInlineContext *inlineContext);

/*
 * Get a new inline context and make it the current inline context for the
 * coder instance..
 */
static ILJITCoderInlineContext *_ILJitCoderInlineContextPush(ILJITCoder *coder);

/*
 * Pop the current inline context of the context stack and make the previous
 * context the current again.
 */
static ILInt32 _ILJitCoderInlineContextPop(ILJITCoder *coder);

/*
 * Check If a method is inlineable by the jit coder.
 */
static int _ILJitMethodIsInlineable(ILJITCoder *jitCoder, ILMethod *method);

/*
 * Inline the IL method.
 */
static int _ILJitCoderInlineMethod(ILJITCoder *jitCoder,
								   ILMethod *method,
								   ILJitValue this,
								   ILJitStackItem *args,
								   ILInt32 numArgs);

#endif	/* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_CODER_INSTANCE

	/* Handle the inline contexts. */
	ILMemPool		inlineContextPool;
	ILJITCoderInlineContext	   *freeInlineContext;

	/* The current context if inlining a method otherwise 0. */
	ILJITCoderInlineContext	   *currentInlineContext;

#endif	/* IL_JITC_CODER_INSTANCE */

#ifdef	IL_JITC_CODER_INIT

	/* Init the inline context stuff. */
	ILMemPoolInit(&(coder->inlineContextPool), sizeof(ILJITCoderInlineContext), 8);
	coder->freeInlineContext = 0;
	coder->currentInlineContext = 0;
	
#endif	/* IL_JITC_CODER_INIT */

#ifdef	IL_JITC_CODER_DESTROY

	while(coder->freeInlineContext)
	{
		ILJITCoderInlineContext *inlineContext = coder->freeInlineContext;

		coder->freeInlineContext = inlineContext->prevContext;
		_ILJitCoderInlineContextDestroy(inlineContext);

	}
	ILMemPoolDestroy(&(coder->inlineContextPool));

#endif	/* IL_JITC_CODER_DESTROY */

#ifdef	IL_JITC_FUNCTIONS

/*
 * Initialize a new inline context.
 */
static void _ILJitCoderInlineContextInit(ILJITCoderInlineContext *inlineContext)
{
	inlineContext->prevContext = 0;
	inlineContext->inlineMethod = 0;
	inlineContext->returnValue = 0;
	inlineContext->stackBase = 0;

#undef	IL_JITC_FUNCTIONS
#define	IL_JITC_INLINE_CONTEXT_INIT
#include "jitc_labels.c"
#include "jitc_locals.c"
#undef	IL_JITC_INLINE_CONTEXT_INIT
#define	IL_JITC_FUNCTIONS
}

/*
 * Destroy an inline context.
 */
static void _ILJitCoderInlineContextDestroy(ILJITCoderInlineContext *inlineContext)
{
#undef	IL_JITC_FUNCTIONS
#define	IL_JITC_INLINE_CONTEXT_DESTROY
#include "jitc_labels.c"
#include "jitc_locals.c"
#undef	IL_JITC_INLINE_CONTEXT_DESTROY
#define	IL_JITC_FUNCTIONS
}

/*
 * Check If a method is inlineable by the jit coder.
 */
static int _ILJitMethodIsInlineable(ILJITCoder *jitCoder, ILMethod *method)
{
	/* Check if we are in debugging mode. */
	if(jitCoder->debugEnabled)
	{
		/* Don't inline anything while in debugging mode. */
		return 0;
	}

	if(method)
	{
		ILJitMethodInfo *jitMethodInfo = (ILJitMethodInfo *)method->userData;
		ILJITCoderInlineContext	   *inlineContext;

		if(!(jitMethodInfo->implementationType & _IL_JIT_IMPL_INLINE_MASK))
		{
			ILMethodCode code;
			ILException *exceptions;

			/* Check if the method is marked not inlineable. */
			if(method->implementAttrs & IL_META_METHODIMPL_NO_INLINING)
			{
				jitMethodInfo->implementationType |= _IL_JIT_IMPL_NOINLINE;
				return 0;
			}

			/* Check if the method is syncronized. */
			if(ILMethod_IsSynchronized(method))
			{
				/* We don't inline syncronized methods. */
				jitMethodInfo->implementationType |= _IL_JIT_IMPL_NOINLINE;
				return 0;
			}

			/* Get the method code */
			if(!ILMethodGetCode(method, &code))
			{
				jitMethodInfo->implementationType |= _IL_JIT_IMPL_NOINLINE;
				return 0;
			}

			if((code.codeLen == 0) || (code.codeLen > _IL_JIT_MAX_INLINE_CODELEN))
			{
				jitMethodInfo->implementationType |= _IL_JIT_IMPL_NOINLINE;
				return 0;
			}

			/* Get the exception list */
			if(!ILMethodGetExceptions(method, &code, &exceptions))
			{
				jitMethodInfo->implementationType |= _IL_JIT_IMPL_NOINLINE;
				return 0;
			}

			/* Now Check if the method has any catch or finally blocks. */
			if(exceptions)
			{
				/* We don't inline methods with catch or finally blocks. */
				jitMethodInfo->implementationType |= _IL_JIT_IMPL_NOINLINE;
				return 0;
			}
			/* Flag the method inlineable. */
			jitMethodInfo->implementationType |= _IL_JIT_IMPL_INLINE;
		}

		if((jitMethodInfo->implementationType & _IL_JIT_IMPL_INLINE_MASK) == _IL_JIT_IMPL_NOINLINE)
		{
			return 0;
		}

		/* Now check if the method is allready somewhere in the currently */
		/* inlined methods to avoid an endless recursion. */
		if(method == ILCCtorMgr_GetCurrentMethod(&(jitCoder->cctorMgr)))
		{
			return 0;
		}

		inlineContext = jitCoder->currentInlineContext;
		while(inlineContext)
		{
			if(method == inlineContext->inlineMethod)
			{
				return 0;
			}
			inlineContext = inlineContext->prevContext;
		}
		return 1;
	}
	return 0;
}

/*
 * Setup the inline context in the coder to inline the given method.
 */
static int _ILJitCoderInlineContextSetup(ILJITCoder *jitCoder,
										 ILMethod *method,
										 ILJitValue this,
										 ILJitStackItem *args,
										 ILInt32 numArgs)
{
	if(method)
	{
		ILJitMethodInfo *jitMethodInfo = (ILJitMethodInfo *)method->userData;
		ILJitType signature;
		ILJitType returnType;
		ILMethodCode code;
		ILJITCoderInlineContext	   *inlineContext;

		/* Get the method code */
		if(!ILMethodGetCode(method, &code))
		{
			return 0;
		}

		/* Get a new inline context. */
		if(!(inlineContext = _ILJitCoderInlineContextPush(jitCoder)))
		{
			return 0;
		}

		/* Setup the return value for the inlined function. */
		if(!(signature = jit_function_get_signature(jitMethodInfo->jitFunction)))
		{
			/* Pop the new created inline context. */
			_ILJitCoderInlineContextPop(jitCoder);
			return 0;
		}

		if(!(returnType = jit_type_get_return(signature)))
		{
			/* Pop the new created inline context. */
			_ILJitCoderInlineContextPop(jitCoder);
			return 0;
		}

		if(returnType == _IL_JIT_TYPE_VOID)
		{
			inlineContext->returnValue = 0;
		}
		else
		{
			if(!(inlineContext->returnValue = jit_value_create(jitCoder->jitFunction,
															   returnType)))
			{
				/* Pop the new created inline context. */
				_ILJitCoderInlineContextPop(jitCoder);
				return 0;
			}
		}
		/* Setup the locals for the inlined function. */
		if(!_ILJitLocalSlotsCreateLocals(jitCoder, 
										 &(inlineContext->jitLocals),
										 code.localVarSig))
		{
			/* Pop the new created inline context. */
			_ILJitCoderInlineContextPop(jitCoder);
			return 0;
		}

		if(!_ILJitLocalSlotsSetupInlineArgs(jitCoder,
											inlineContext,
											this,
											args,
											numArgs))
		{
			/* Pop the new created inline context. */
			_ILJitCoderInlineContextPop(jitCoder);
			return 0;
		}

		if(!_ILJitLocalsInitInlineContext(jitCoder, inlineContext))
		{
			/* Pop the new created inline context. */
			_ILJitCoderInlineContextPop(jitCoder);
			return 0;
		}

		/* Queue the cctor to run. */
		if(!ILCCtorMgr_OnCallMethod(&(jitCoder->cctorMgr), method))
		{
			/* Pop the new created inline context. */
			_ILJitCoderInlineContextPop(jitCoder);
			return 0;
		}

		ILMemPoolClear(&(inlineContext->labelPool));
		inlineContext->labelList = 0;
		inlineContext->inlineMethod = method;
		inlineContext->stackBase = jitCoder->stackTop;
		inlineContext->returnLabel = jit_label_undefined;

		return 1;
	}
	return 0;
}

/*
 * Inline the IL method.
 */
static int _ILJitCoderInlineMethod(ILJITCoder *jitCoder,
								   ILMethod *method,
								   ILJitValue this,
								   ILJitStackItem *args,
								   ILInt32 numArgs)
{
	if(_ILJitCoderInlineContextSetup(jitCoder,
									 method,
									 this,
									 args,
									 numArgs))
	{
		ILJITCoderInlineContext	   *inlineContext;
		ILMethodCode code;
		unsigned char *start;

		inlineContext = jitCoder->currentInlineContext;
		if(!inlineContext)
		{
			return 0;
		}

		/* Get the method code */
		if(!ILMethodGetCode(method, &code))
		{
			return 0;
		}

		if(!_ILVerify((ILCoder *)jitCoder,
					  &start,
					  method,
					  &code,
					  ILImageIsSecure(ILProgramItem_Image(method)),
					  ILExecThreadCurrent()))
		{
			return 0;
		}

		if(inlineContext != jitCoder->currentInlineContext)
		{
			printf("Inline contexts on start and end don't match!\n");
		}

		if(jitCoder->stackTop != inlineContext->stackBase)
		{
			printf("Stack height on start and end doesn't match! start: %i end: %i\n",
				   inlineContext->stackBase, jitCoder->stackTop);

			jitCoder->stackTop = inlineContext->stackBase;
		}

		/* Set the end label for the inlined method. */
		if(inlineContext->returnLabel != jit_label_undefined)
		{
			jit_insn_label(jitCoder->jitFunction,
						   &(inlineContext->returnLabel));
		}

		/* And push the return value on the stack. */
		if(inlineContext->returnValue)
		{
			_ILJitStackPushValue(jitCoder, inlineContext->returnValue);
		}

		/* Pop the inline context. */
		_ILJitCoderInlineContextPop(jitCoder);

		return 1;
	}
	return 0;
}

/*
 * Pop the current inline context of the context stack and make the previous
 * context the current again.
 */
static ILInt32 _ILJitCoderInlineContextPop(ILJITCoder *coder)
{
	if(coder->currentInlineContext)
	{
		/* Save the current inline context in the freelist.*/
		ILJITCoderInlineContext *inlineContext = coder->currentInlineContext->prevContext;

		coder->currentInlineContext->prevContext = coder->freeInlineContext;
		coder->freeInlineContext = coder->currentInlineContext;
		coder->currentInlineContext = inlineContext;
		return 1;
	}
	return 0;
}

/*
 * Get a new inline context and make it the current inline context for the
 * coder instance..
 */
static ILJITCoderInlineContext *_ILJitCoderInlineContextPush(ILJITCoder *coder)
{
	ILJITCoderInlineContext *inlineContext = 0;

	if((inlineContext = coder->freeInlineContext) == 0)
	{
		if(!(inlineContext = (ILJITCoderInlineContext *)ILMemPoolAllocItem(&(coder->inlineContextPool))))
		{
			return 0;
		}
		/* Now initialize the new allocated inline context. */
		_ILJitCoderInlineContextInit(inlineContext);
	}
	else
	{
		/* Take the inline context from the freelist. */
		coder->freeInlineContext = inlineContext->prevContext;
	}
	/* Make the new inline context the current one. */
	inlineContext->prevContext = coder->currentInlineContext;
	coder->currentInlineContext = inlineContext;
	return inlineContext;
}

#endif	/* IL_JITC_FUNCTIONS */

#endif	/* _IL_JIT_ENABLE_INLINE */

