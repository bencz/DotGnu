/*
 * system.c - functions and objects used by the runtime.
 *
 * Copyright (C) 2001, 2011  Southern Storm Software, Pty Ltd.
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

#include "engine_private.h"
#include "lib_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
/* Global lock for trace outputs */
ILMutex *globalTraceMutex;
#endif

#if defined(IL_CONFIG_REFLECTION) && defined(IL_CONFIG_DEBUG_LINES)

/*
 * Find the "stackTrace" field within "System.Exception" and then set.
 */
static int FindAndSetStackTrace(ILExecThread *thread, ILObject *object)
{
	ILField *field;

	/* Find the "stackTrace" field within the "Exception" class */
	field = ILExecThreadLookupField
			(thread, "System.Exception", "stackTrace",
			 "[vSystem.Diagnostics.PackedStackFrame;");
	if(field)
	{
#ifdef IL_USE_CVM
		ILObject *trace;
		ILCallFrame *callFrame;

		/* Push the current frame data onto the stack temporarily
		   so that "GetExceptionStackTrace" can find it */
		if(thread->numFrames < thread->maxFrames)
		{
			callFrame = &(thread->frameStack[(thread->numFrames)++]);
		}
		else if((callFrame = _ILAllocCallFrame(thread)) == 0)
		{
			/* We ran out of memory trying to push the frame */
			return 0;
		}
		callFrame->method = thread->method;
		callFrame->pc = thread->pc;
		callFrame->frame = thread->frame;
		callFrame->permissions = 0;

		/* Get the stack trace and pop the frame */
		trace = (ILObject *)_IL_StackFrame_GetExceptionStackTrace(thread);
		--(thread->numFrames);
		if(!trace)
		{
			/* We ran out of memory obtaining the stack trace */
			return 0;
		}

		/* Write the stack trace into the object */
		*((ILObject **)(((unsigned char *)object) + field->offset)) = trace;
#endif
#ifdef IL_USE_JIT
		System_Array *trace;

		trace = _ILJitGetExceptionStackTrace(thread);

		/* Write the stack trace into the object */
		*((System_Array **)(((unsigned char *)object) + field->offset)) = trace;
#endif
	}
	return 1;
}

/*
 * Set the stack trace for an exception to the current call context.
 */
void _ILSetExceptionStackTrace(ILExecThread *thread, ILObject *object)
{
	ILClass *classInfo;
	if(!object)
	{
		return;
	}
	classInfo = ILExecThreadLookupClass(thread, "System.Exception");
	if(!classInfo)
	{
		return;
	}
	if(!ILClassInheritsFrom(GetObjectClass(object), classInfo))
	{
		return;
	}
	if(!FindAndSetStackTrace(thread, object))
	{
		/* We ran out of memory while allocating the stack trace,
		   but it isn't serious: we can just throw without the trace */
		_ILExecThreadClearException(thread);
	}
}

#else  /* !(IL_CONFIG_REFLECTION && IL_CONFIG_DEBUG_LINES) */

/*
 * Set the stack trace for an exception to the current call context.
 */
void _ILSetExceptionStackTrace(ILExecThread *thread, ILObject *object)
{
	/* Nothing to do here */
}

#endif /* !(IL_CONFIG_REFLECTION && IL_CONFIG_DEBUG_LINES) */

void *_ILSystemExceptionWithClass(ILExecThread *thread, ILClass *classInfo)
{
	ILObject *object;
	
	object = _ILEngineAllocObject(thread, classInfo);
	if(object)
	{
#if defined(IL_CONFIG_REFLECTION) && defined(IL_CONFIG_DEBUG_LINES)
		if(!FindAndSetStackTrace(thread, object))
		{
			/* We ran out of memory: pick up the "OutOfMemoryException" */
			object = _ILExecThreadGetException(thread);
			_ILExecThreadClearException(thread);
		}
#endif /* IL_CONFIG_REFLECTION && IL_CONFIG_DEBUG_LINES */
	}
	else
	{
		/* The system ran out of memory, so copy the "OutOfMemoryException" */
		object = _ILExecThreadGetException(thread);
		_ILExecThreadClearException(thread);
	}
	return object;
}

void *_ILSystemException(ILExecThread *thread, const char *className)
{
	ILClass *classInfo = ILExecThreadLookupClass(thread, className);
	if(!classInfo)
	{
	#ifndef REDUCED_STDIO
		/* Huh?  The required class doesn't exist.  This shouldn't happen */
		fprintf(stderr, "Fatal error: %s is missing from the system library\n",
				className);
		exit(1);
	#endif
	}
	return _ILSystemExceptionWithClass(thread, classInfo);
}

int _ILSystemObjectSetField(ILExecThread *thread, ILObject* obj,
							const char *fieldName, const char *signature,
							ILObject *value)
{
	ILField *field;
	ILClass *classInfo = GetObjectClass(obj);

	field = ILExecThreadLookupFieldInClass(thread, classInfo, fieldName, signature);

	if(field == 0)
	{
		return -1;
	}

	*(((ILObject **)(((char *)obj) + field->offset))) = value;

	return 0;
}

#ifdef	__cplusplus
};
#endif
