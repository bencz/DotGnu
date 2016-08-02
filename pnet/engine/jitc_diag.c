/*
 * jitc_diag.c - Jit coder diagnostic routines.
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

/*
 * Get the ILMethod for the call frame up n slots.
 * Returns 0 if the function at that slot is not a jitted function.
 */
ILMethod *_ILJitGetCallingMethod(ILExecThread *thread, ILUInt32 frames)
{
	ILExecProcess *process = _ILExecThreadProcess(thread);
	ILJITCoder *jitCoder;
	ILJitFunction jitFunction = 0;
	jit_unwind_context_t	unwindContext;

	if(!process)
	{
		return 0;
	}
	if(!(jitCoder = _ILCoderToILJITCoder(process->coder)))
	{
		return 0;
	}
	/* Find the first callframe that has a jitFunction assigned. */
	/* This callframe is usually the jitFunction for the internalcall. */
	if(!jit_unwind_init(&unwindContext, jitCoder->context))
	{
		return 0;
	}	
	do
	{
		if((jitFunction = jit_unwind_get_function(&unwindContext)))
		{
			break;
		}
		if(!jit_unwind_next(&unwindContext))
		{
			return 0;
		}
	} while(1);

	/* The unwind context is now at the first frame with a jitFunction */
	/* assigned. */
	/* Now we have to find the next frame with a jitFunction assigned. */
	while(frames > 0)
	{
		if(jit_unwind_next(&unwindContext) == 0)
		{
			return 0;
		}
		if((jitFunction = jit_unwind_get_function(&unwindContext)))
		{
			frames--;
			if(frames == 0)
			{
				break;
			}
		}
	};
	/* And we return the ILMethod assigned to that jitFunction. */
	return (ILMethod *)jit_function_get_meta(jitFunction, IL_JIT_META_METHOD);
}

/*
 * Get the number of stack frames associated with an ILMethod on the current
 * call stack.
 */
ILInt32 _ILJitDiagNumFrames(ILExecThread *thread)
{
	ILJITCoder *jitCoder = (ILJITCoder *)(_ILExecThreadProcess(thread)->coder);
	ILInt32 num = 0;

	if(jitCoder)
	{
		jit_stack_trace_t stackTrace = jit_exception_get_stack_trace();
		if(stackTrace)
		{
			ILUInt32 size = jit_stack_trace_get_size(stackTrace);
			ILUInt32 current = 0;
			ILJitFunction func;

			for(current = 0; current < size; ++current)
			{
				if((func = jit_stack_trace_get_function(jitCoder->context,
														stackTrace, current)))
				{
					if(jit_function_get_meta(func, IL_JIT_META_METHOD) != 0)
					{
						++num;
					}
				}
			}
			jit_stack_trace_free(stackTrace);
		}
	}
	return num;
}

/*
 * Get the current PackedStackFrame.
 */
System_Array *_ILJitGetExceptionStackTrace(ILExecThread *thread)
{
	ILInt32 num = 0;
	ILJITCoder *jitCoder = (ILJITCoder *)(_ILExecThreadProcess(thread)->coder);
	jit_stack_trace_t stackTrace = jit_exception_get_stack_trace();

	if(!jitCoder)
	{
		return 0;
	}

	if(stackTrace)
	{
		/* Get the number of frames on the stack, and also determine
		   where the exception constructors stop and real code starts */
		ILUInt32 size = jit_stack_trace_get_size(stackTrace);
		ILJitFunction jitFunction;
		ILMethod *method;
		ILObject *array;
		PackedStackFrame *data;
		ILClass *classInfo;
		ILUInt32 current;

		for(current = 0; current < size; ++current)
		{
			jitFunction = jit_stack_trace_get_function(jitCoder->context,
													   stackTrace, current);
			if(jitFunction)
			{
				if(jit_function_get_meta(jitFunction, IL_JIT_META_METHOD) != 0)
				{
					++num;
				}

			}
		}

		/* Put an upper limit on the number of frames so that we
		   don't arbitrarily blow up the exception handling system */
		if(num > 256)
		{
			num = 256;
		}

		/* Allocate an array for the packed stack data.  We cannot
		   use "ILExecThreadNew" because it will re-enter the engine.
		   If we are throwing "StackOverflowException", then we will
		   get an infinite recursive loop */
		classInfo = ILExecThreadLookupClass
				(thread, "[vSystem.Diagnostics.PackedStackFrame;");
		if(!classInfo)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		array = _ILEngineAlloc(thread, classInfo,
							   sizeof(System_Array) +
							   		num * sizeof(PackedStackFrame));
		if(!array)
		{
			return 0;
		}
		ArrayLength(array) = num;

		/* Fill the array with the packed stack data */
		data = (PackedStackFrame *)ArrayToBuffer(array);
		current = 0;
		while(current < size && num > 0)
		{
			jitFunction = jit_stack_trace_get_function(jitCoder->context,
													   stackTrace, current);
			if(jitFunction)
			{
				if((method = (ILMethod *)jit_function_get_meta(jitFunction,
													IL_JIT_META_METHOD)) != 0)
				{
					data->method = method;
					data->offset = jit_stack_trace_get_offset(jitCoder->context,
															  stackTrace,
															  current);
					/* Get the native pc  */
					/* TODO: make nativeOffset a void *. */
					data->nativeOffset = (ILInt32)jit_stack_trace_get_pc(stackTrace,
																		 current);
					++data;
					--num;
				}
			}
			++current;
		}
		jit_stack_trace_free(stackTrace);

		/* Done */
		return (System_Array *)array;
	}
	return 0;
}

