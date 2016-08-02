/*
 * lib_diag.c - Internalcall methods for "System.Diagnostics".
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
#include "lib_defs.h"
#include "il_debug.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef IL_CONFIG_DEBUG_LINES

int ILExecProcessDebugHook(ILExecProcess *process,
						   ILExecDebugHookFunc func,
						   void *data)
{
	/* Record the hook details */
	process->debugHookFunc = func;
	process->debugHookData = data;

	/* Turn on debug support in the coder */
	ILCoderEnableDebug(process->coder);

	/* The engine supports debugging */
	return 1;
}

int ILExecProcessWatchMethod(ILExecProcess *process, ILMethod *method)
{
	ILExecDebugWatch *watch;

	/* Lock down the process */
	ILMutexLock(process->lock);

	/* Search the watch list for the method */
	watch = process->debugWatchList;
	while(watch != 0)
	{
		if(watch->method == method)
		{
			++(watch->count);
			ILMutexUnlock(process->lock);
			return 1;
		}
		watch = watch->next;
	}

	/* Add the method to the watch list */
	if((watch = (ILExecDebugWatch *)ILMalloc(sizeof(ILExecDebugWatch))) == 0)
	{
		ILMutexUnlock(process->lock);
		return 0;
	}
	watch->method = method;
	watch->count = 1;
	watch->next = process->debugWatchList;
	process->debugWatchList = watch;

	/* Unlock the process and return */
	ILMutexUnlock(process->lock);
	return 1;
}

void ILExecProcessUnwatchMethod(ILExecProcess *process, ILMethod *method)
{
	ILExecDebugWatch *watch;
	ILExecDebugWatch *prevWatch;

	/* Lock down the process */
	ILMutexLock(process->lock);

	/* Search the watch list for the method */
	watch = process->debugWatchList;
	prevWatch = 0;
	while(watch != 0)
	{
		if(watch->method == method)
		{
			if(--(watch->count) == 0)
			{
				if(prevWatch)
				{
					prevWatch->next = watch->next;
				}
				else
				{
					process->debugWatchList = watch->next;
				}
				ILFree(watch);
			}
			ILMutexUnlock(process->lock);
			return;
		}
		prevWatch = watch;
		watch = watch->next;
	}

	/* Unlock the process and return */
	ILMutexUnlock(process->lock);
}

void ILExecProcessWatchAll(ILExecProcess *process, int flag)
{
	ILMutexLock(process->lock);
	process->debugWatchAll = flag;
	ILMutexUnlock(process->lock);
}

int _ILIsBreak(ILExecThread *thread, ILMethod *method)
{
	ILExecProcess *process = thread->process;
	ILExecDebugWatch *watch;

	/* Lock down the process while we do the test */
	ILMutexLock(process->lock);

	/* We should break if all methods are being watched */
	if(process->debugWatchAll)
	{
		ILMutexUnlock(process->lock);
		return 1;
	}

	/* Search the breakpoint watch list for the method */
	watch = process->debugWatchList;
	while(watch != 0)
	{
		if(watch->method == method)
		{
			ILMutexUnlock(process->lock);
			return 1;
		}
		watch = watch->next;
	}

	/* There is no breakpoint watch registered for this method */
	ILMutexUnlock(process->lock);
	return 0;
}

void _ILBreak(ILExecThread *thread, int type)
{
#ifdef IL_CONFIG_DEBUGGER
	int action;
#ifdef IL_USE_CVM
	unsigned char *start;
#endif

	if(thread->process->debugHookFunc)
	{
#ifdef IL_USE_CVM
		/* Consult the coder to convert the PC into an IL offset */
		if(thread->method && thread->pc != IL_INVALID_PC)
		{
				start = (unsigned char *)ILMethodGetUserData(thread->method);
				if(ILMethodIsConstructor(thread->method))
				{
						start -= ILCoderCtorOffset(thread->process->coder);
				}
				thread->offset = (ILInt32)(ILCoderGetILOffset
								(thread->process->coder, (void *)start,
								(ILUInt32)(thread->pc - start), 0));
		}
		else
		{
				thread->offset = -1;
		}
#endif
		/* Call the debugger to process the breakpoint */
		action = (*(thread->process->debugHookFunc))
			(thread->process->debugHookData,
			 thread, thread->method, thread->offset, type);

		/* Abort the engine if requested by the debugger */
		if(action == IL_HOOK_ABORT)
		{
			exit(7);
		}
	}
#endif /* IL_CONFIG_DEBUGGER */
}

/*
 * System.Diagnostics.Debugger class.
 */

/*
 * private static bool InternalIsAttached();
 */
ILBool _IL_Debugger_InternalIsAttached(ILExecThread *thread)
{
	return (thread->process->debugHookFunc != 0);
}

/*
 * public static void Break();
 */
void _IL_Debugger_Break(ILExecThread *thread)
{
#ifdef IL_CONFIG_DEBUGGER
	ILInt32 offset;
	int action;

	if(thread->process->debugHookFunc)
	{
		/* Call out to the debugger with an explicit breakpoint */
		offset = _IL_StackFrame_InternalGetILOffset(thread, 0);
		action = (*(thread->process->debugHookFunc))
			(thread->process->debugHookData,
			 thread, ILExecThreadStackMethod(thread, 1),
			 offset, IL_BREAK_EXPLICIT);
		if(action == IL_HOOK_ABORT)
		{
			/* Abort the application that is being debugged */
			exit(7);
		}
	}
#endif /* IL_CONFIG_DEBUGGER */
}

/*
 * public static bool IsLogging();
 */
ILBool _IL_Debugger_IsLogging(ILExecThread *thread)
{
	/* Debug logging is not supported by this implementation */
	return 0;
}

/*
 * private static bool InternalLaunch();
 */
ILBool _IL_Debugger_InternalLaunch(ILExecThread *thread)
{
	/* In this implementation, the user launches the debugger
	   prior to running the application.  The application itself
	   cannot launch the debugger unless it is already present */
	return (thread->process->debugHookFunc != 0);
}

/*
 * public static void Log(int level, String category, String message);
 */
void _IL_Debugger_Log(ILExecThread *thread, ILInt32 level,
					  ILString *category, ILString *message)
{
	/* Debug logging is not supported by this implementation */
}

/*
 * System.Diagnostics.StackFrame class.
 */

/*
 * private static int InternalGetTotalFrames();
 */
ILInt32 _IL_StackFrame_InternalGetTotalFrames(ILExecThread *thread)
{
	ILInt32 num = 0;
#ifdef IL_USE_CVM
	ILCallFrame *frame = _ILGetCallFrame(thread, 0);
	while(frame != 0)
	{
		++num;
		frame = _ILGetNextCallFrame(thread, frame);
	}
#endif
#ifdef IL_USE_JIT
	num = _ILJitDiagNumFrames(thread);
#endif
	return num;
}

/*
 * private static RuntimeMethodHandle InternalGetMethod(int skipFrames);
 */
void _IL_StackFrame_InternalGetMethod(ILExecThread *thread,
									  void *result, ILInt32 skipFrames)
{
#ifdef IL_USE_CVM
	ILCallFrame *frame = _ILGetCallFrame(thread, skipFrames);
	if(frame)
	{
		*((ILMethod **)result) = frame->method;
	}
	else
	{
		*((ILMethod **)result) = 0;
	}
#endif
#ifdef IL_USE_JIT
	*(ILMethod **)result = _ILJitGetCallingMethod(thread, skipFrames);
#endif
}

/*
 * private static int InternalGetILOffset(ILInt32 skipFrames);
 */
ILInt32 _IL_StackFrame_InternalGetILOffset(ILExecThread *thread,
										   ILInt32 skipFrames)
{
	ILCallFrame *frame = _ILGetCallFrame(thread, skipFrames);
	unsigned char *start;
	if(frame && frame->method && frame->pc != IL_INVALID_PC)
	{
		/* Consult the coder to convert the PC into an IL offset */
		start = (unsigned char *)ILMethodGetUserData(frame->method);
		if(ILMethodIsConstructor(frame->method))
		{
			start -= ILCoderCtorOffset(thread->process->coder);
		}
		return (ILInt32)(ILCoderGetILOffset
					(thread->process->coder, (void *)start,
					 (ILUInt32)(frame->pc - start), 0));
	}
	else
	{
		/* Probably a native function which does not have offsets */
		return -1;
	}
}

/*
 * private static int InternalGetNativeOffset(ILInt32 skipFrames);
 */
ILInt32 _IL_StackFrame_InternalGetNativeOffset(ILExecThread *thread,
											   ILInt32 skipFrames)
{
	ILCallFrame *frame = _ILGetCallFrame(thread, skipFrames);
	unsigned char *start;
	if(frame && frame->method && frame->pc != IL_INVALID_PC)
	{
		/* Convert the PC into a native offset using the method start */
		start = (unsigned char *)ILMethodGetUserData(frame->method);
		if(ILMethodIsConstructor(frame->method))
		{
			start -= ILCoderCtorOffset(thread->process->coder);
		}
		return (ILInt32)(frame->pc - start);
	}
	else
	{
		/* Probably a native function which does not have offsets */
		return -1;
	}
}

/*
 * private static String InternalGetDebugInfo(RuntimeMethodHandle method,
 *											  int offset,
 *											  out int line, out int col);
 */
ILString *_IL_StackFrame_InternalGetDebugInfo
				(ILExecThread *thread, void *_method, ILInt32 offset,
				 ILInt32 *line, ILInt32 *col)
{
	ILMethod *method;
	ILDebugContext *dbg;
	const char *filename;

	/* Initialize the return parameters in case of error */
	*line = 0;
	*col = 0;

	/* Validate the method handle */
#ifdef IL_USE_JIT
	method = (ILMethod *)_method;
#else
	method = *((ILMethod **)_method);
#endif
	if(!method)
	{
		return 0;
	}

	/* Bail out if the method's image does not have any debug information */
	if(!ILDebugPresent(ILProgramItem_Image(method)))
	{
		return 0;
	}

	/* Get the symbol debug information */
	if((dbg = ILDebugCreate(ILProgramItem_Image(method))) == 0)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}
	filename = ILDebugGetLineInfo(dbg, ILMethod_Token(method),
								  (ILUInt32)offset,
								  (ILUInt32 *)line, (ILUInt32 *)col);
	ILDebugDestroy(dbg);

	/* No debug information if "filename" is NULL */
	if(!filename)
	{
		return 0;
	}

	/* Convert the filename into a string and exit */
	return ILStringCreate(thread, filename);
}

/*
 * internal static PackedStackFrame[] GetExceptionStackTrace();
 */
System_Array *_IL_StackFrame_GetExceptionStackTrace(ILExecThread *thread)
{
#ifdef IL_USE_CVM
#ifdef IL_CONFIG_REFLECTION
	ILInt32 num;
	ILCallFrame *frame;
	ILObject *array;
	PackedStackFrame *data;
	unsigned char *start;
	ILClass *classInfo;

	/* Get the number of frames on the stack, and also determine
	   where the exception constructors stop and real code starts */
	num = 0;
	frame = _ILGetCallFrame(thread, 0);
	while(frame != 0)
	{
		++num;
		frame = _ILGetNextCallFrame(thread, frame);
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
	frame = _ILGetCallFrame(thread, 0);
	while(frame != 0 && num > 0)
	{
		data->method = frame->method;
		if(frame->method && frame->pc != IL_INVALID_PC)
		{
			/* Find the start of the frame method */
			start = (unsigned char *)ILMethodGetUserData(frame->method);
			if(ILMethodIsConstructor(frame->method))
			{
				start -= ILCoderCtorOffset(thread->process->coder);
			}

			/* Get the native offset from the method start */
			data->nativeOffset = (ILInt32)(frame->pc - start);

			/* Get the IL offset from the coder.  We use the native
			   offset minus 1 because we want the IL offset for the
			   instruction just before the return address, not after */
			data->offset = (ILInt32)ILCoderGetILOffset
				(thread->process->coder, (void *)start,
				 (ILUInt32)(data->nativeOffset - 1), 0);
		}
		else
		{
			/* Probably a native method that does not have offsets */
			data->offset = -1;
			data->nativeOffset = -1;
		}
		++data;
		frame = _ILGetNextCallFrame(thread, frame);
		--num;
	}

	/* Done */
	return (System_Array *)array;
#else
	return 0;
#endif
#endif
#ifdef IL_USE_JIT
	return _ILJitGetExceptionStackTrace(thread);
#endif
}

#else /* !IL_CONFIG_DEBUG_LINES */

int ILExecProcessDebugHook(ILExecProcess *process,
						   ILExecDebugHookFunc func,
						   void *data)
{
	/* The engine does not support debugging */
	return 0;
}

int ILExecProcessWatchMethod(ILExecProcess *process, ILMethod *method)
{
	/* Nothing to do here */
	return 1;
}

void ILExecProcessUnwatchMethod(ILExecProcess *process, ILMethod *method)
{
	/* Nothing to do here */
}

void ILExecProcessWatchAll(ILExecProcess *process, int flag)
{
	/* Nothing to do here */
}

#endif /* !IL_CONFIG_DEBUG_LINES */

#ifdef	__cplusplus
};
#endif
