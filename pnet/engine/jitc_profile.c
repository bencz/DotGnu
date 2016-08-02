/*
 * jitc_profile.c - Profiling functions for the JIT Coder.
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

#ifdef IL_JITC_CODER_INSTANCE

#ifndef IL_CONFIG_REDUCE_CODE
#ifdef ENHANCED_PROFILER
	/* profile timestamp for the current function */
	ILJitValue		profileTimestamp;

	/* profile timestamp for inlined functions */
	ILJitValue		inlineTimestamp;
#endif /* ENHANCED_PROFILER */
#endif /* !IL_CONFIG_REDUCE_CODE */

#endif /* IL_JITC_CODER_INSTANCE */

#ifdef IL_JITC_CODER_INIT

#ifndef IL_CONFIG_REDUCE_CODE
#ifdef ENHANCED_PROFILER
	/* initialize the timestamps */
	coder->profileTimestamp = 0;
	coder->inlineTimestamp = 0;
#endif /* ENHANCED_PROFILER */
#endif /* !IL_CONFIG_REDUCE_CODE */

#endif /* IL_JITC_CODER_INIT */

#ifdef IL_JITC_DECLARATIONS

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
/*
 * Emit the code for start profiling an internal call or inlined method.
 */
static void _ILJitProfileStart(ILJITCoder *jitCoder, ILMethod *method);

/*
 * Emit the code for end profiling an internal call or inlined method.
 */
static void _ILJitProfileEnd(ILJITCoder *jitCoder, ILMethod *method);
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */

#endif	/* IL_JITC_DECLARATIONS */

#ifdef IL_JITC_FUNCTIONS

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
#ifndef ENHANCED_PROFILER
/*
 * Helper function for atomic increment of a 32 bit signed integer.
 */
static ILInt32 _ILJitInterlockedIncrement(volatile ILInt32 *dest)
{
	return ILInterlockedIncrementI4(dest);
}

/*
 * Emit the code to increase the call count of a method.
 */
static void _ILJitProfileIncreaseMethodCallCount(ILJITCoder *jitCoder, ILMethod *method)
{
	ILJitValue callCounter = jit_value_create_nint_constant(jitCoder->jitFunction,
															_IL_JIT_TYPE_VPTR,
															(jit_nint)(&(method->count)));

	jit_insn_call_native(jitCoder->jitFunction,
						 "_ILJitInterlockedIncrement",
						 _ILJitInterlockedIncrement,
						 _ILJitSignature_ILInterlockedIncrement,
						 &callCounter, 1, JIT_CALL_NOTHROW);
}
#endif /* !ENHANCED_PROFILER */

/*
 * Emit the code for start profiling an internal call or inlined method.
 */
static void _ILJitProfileStart(ILJITCoder *jitCoder, ILMethod *method)
{
#ifdef ENHANCED_PROFILER
	ILJitValue timestampAddress;

	if(!jitCoder->inlineTimestamp)
	{
		if(!(jitCoder->inlineTimestamp = jit_value_create(jitCoder->jitFunction,
														  _IL_JIT_TYPE_INT64)))
		{
			return;
		} 
	}
	if(!(timestampAddress = jit_insn_address_of(jitCoder->jitFunction,
												jitCoder->inlineTimestamp)))
	{
		return;
	}
	jit_insn_call_native(jitCoder->jitFunction, "_ILProfilingStart",
						 _ILProfilingStart, _ILJitSignature_ILProfilingStart,
						 &timestampAddress, 1, JIT_CALL_NOTHROW);
#else /* !ENHANCED_PROFILER */
	if(method)
	{
		/* Emit the code to increase the call count of the method*/
		_ILJitProfileIncreaseMethodCallCount(jitCoder, method);
	}
#endif /* !ENHANCED_PROFILER */
}

/*
 * Emit the code for end profiling an internal call or inlined method.
 */
static void _ILJitProfileEnd(ILJITCoder *jitCoder, ILMethod *method)
{
#ifdef ENHANCED_PROFILER
	ILJitValue thread = _ILJitCoderGetThread(jitCoder);
	jit_label_t label = jit_label_undefined;
	ILJitValue profilingEnabled;
	ILJitValue args[2];

	if(!jitCoder->inlineTimestamp)
	{
		return;
	}

	/*
	 * If the enhanced profiler is selected then don't count if profiling
	 * is disabled (e.g. via DotGNU.Misc.Profiling.StopProfiling())
	 */
	profilingEnabled = jit_insn_load_relative(jitCoder->jitFunction,
											  thread,
											  offsetof(ILExecThread, profilingEnabled),
											  jit_type_sys_int);
	jit_insn_branch_if_not(jitCoder->jitFunction, profilingEnabled, &label);
	if(!(args[0] = jit_value_create_nint_constant(jitCoder->jitFunction,
												  _IL_JIT_TYPE_VPTR,
												  (jit_nint)method)))
	{
		return;
	}
	if(!(args[1] = jit_insn_address_of(jitCoder->jitFunction,
									   jitCoder->inlineTimestamp)))
	{
		return;
	}
	jit_insn_call_native(jitCoder->jitFunction, "_ILProfilingEnd",
						 _ILProfilingEnd, _ILJitSignature_ILProfilingEnd,
						 args, 2, JIT_CALL_NOTHROW);
	jit_insn_label(jitCoder->jitFunction, &label);
#endif  /* ENHANCED_PROFILER */
}
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */

#endif	/* IL_JITC_FUNCTIONS */

#ifdef IL_JITC_CODE

/*
 * Start profiling of the current method.
 */
static void JITCoder_ProfilingStart(ILCoder *coder)
{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	ILJITCoder *jitCoder = ((ILJITCoder *)coder);

#ifdef ENHANCED_PROFILER
	jit_label_t startLabel = jit_label_undefined;
	jit_label_t endLabel = jit_label_undefined;
	ILJitValue timestampAddress;

	if(!jitCoder->profileTimestamp)
	{
		if(!(jitCoder->profileTimestamp = jit_value_create(jitCoder->jitFunction,
														   _IL_JIT_TYPE_INT64)))
		{
			return;
		} 
	}
	jit_insn_label(jitCoder->jitFunction, &startLabel);
	if(!(timestampAddress = jit_insn_address_of(jitCoder->jitFunction,
											 jitCoder->profileTimestamp)))
	{
		return;
	}
	jit_insn_call_native(jitCoder->jitFunction, "_ILProfilingStart",
						 _ILProfilingStart, _ILJitSignature_ILProfilingStart,
						 &timestampAddress, 1, JIT_CALL_NOTHROW);
	jit_insn_label(jitCoder->jitFunction, &endLabel);
	jit_insn_move_blocks_to_start(jitCoder->jitFunction, startLabel, endLabel);
#else /* !ENHANCED_PROFILER */
	ILMethod *method;

	method = ILCCtorMgr_GetCurrentMethod(&(jitCoder->cctorMgr));
	if(method)
	{
		/* Emit the code to increase the call count of the method*/
		_ILJitProfileIncreaseMethodCallCount(jitCoder, method);
	}
#endif /* !ENHANCED_PROFILER */
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */
}

/*
 * End profiling of the current method.
 */
static void JITCoder_ProfilingEnd(ILCoder *coder)
{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
#if defined(ENHANCED_PROFILER)
	ILJITCoder *jitCoder = ((ILJITCoder *)coder);
	ILJitValue thread = _ILJitCoderGetThread(jitCoder);
	jit_label_t label = jit_label_undefined;
	ILJitValue profilingEnabled;
	ILMethod *method;
	ILJitValue args[2];

	method = ILCCtorMgr_GetCurrentMethod(&(jitCoder->cctorMgr));
	if(!method)
	{
		return;
	}

	if(!jitCoder->profileTimestamp)
	{
		return;
	}

	/*
	 * If the enhanced profiler is selected then don't count if profiling
	 * is disabled (e.g. via DotGNU.Misc.Profiling.StopProfiling())
	 */
	profilingEnabled = jit_insn_load_relative(jitCoder->jitFunction,
											  thread,
											  offsetof(ILExecThread, profilingEnabled),
											  jit_type_sys_int);
	jit_insn_branch_if_not(jitCoder->jitFunction, profilingEnabled, &label);
	if(!(args[0] = jit_value_create_nint_constant(jitCoder->jitFunction,
												  _IL_JIT_TYPE_VPTR,
												  (jit_nint)method)))
	{
		return;
	}
	if(!(args[1] = jit_insn_address_of(jitCoder->jitFunction,
									   jitCoder->profileTimestamp)))
	{
		return;
	}
	jit_insn_call_native(jitCoder->jitFunction, "_ILProfilingEnd",
						 _ILProfilingEnd, _ILJitSignature_ILProfilingEnd,
						 args, 2, JIT_CALL_NOTHROW);
	jit_insn_label(jitCoder->jitFunction, &label);
#endif /* !ENHANCED_PROFILER */
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */
}

#endif /* IL_JITC_CODE */

