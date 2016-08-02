/*
 * throw.c - External API's for throwing and managing exceptions.
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
#include "il_debug.h"

#ifdef	__cplusplus
extern	"C" {
#endif

int ILExecThreadHasException(ILExecThread *thread)
{
	return _ILExecThreadHasException(thread);
}

ILObject *ILExecThreadGetException(ILExecThread *thread)
{
	return _ILExecThreadGetException(thread);
}

void ILExecThreadClearException(ILExecThread *thread)
{
	_ILExecThreadClearException(thread);
}

void ILExecThreadSetException(ILExecThread *thread, ILObject *obj)
{
	_ILExecThreadSetException(thread, obj);
}

void _ILSetExceptionStackTrace(ILExecThread *thread, ILObject *object);

void ILExecThreadThrow(ILExecThread *thread, ILObject *object)
{
	if(!_ILExecThreadHasException(thread))
	{
		_ILExecThreadSetException(thread, object);
	}
}

void ILExecThreadThrowSystem(ILExecThread *thread, const char *typeName,
							 const char *resourceName)
{
	ILMethod *method;
	ILString *resourceString = 0;
	ILObject *object;

	/* Bail out if there already is a pending exception or if the thread is aborting */
	if(_ILExecThreadHasException(thread))
	{
		return;
	}

	/* Look for the "System.Object._" method, which allows us to
	   look up localised error messages */
	if(resourceName)
	{
		method = ILExecThreadLookupMethod(thread, "System.Object", "_",
										  "(oSystem.String;)oSystem.String;");
		if(method)
		{
			resourceString = ILStringCreate(thread, resourceName);
			if(resourceString)
			{
				ILExecThreadCall(thread, method, &resourceString,
								 resourceString);
				if(_ILExecThreadHasException(thread))
				{
					/* The string lookup threw an exception */
					return;
				}
			}
		}
	}

	/* Create the new exception object and throw it */
	if(resourceString)
	{
		object = ILExecThreadNew(thread, typeName, "(ToSystem.String;)V",
								 resourceString);
	}
	else
	{
		object = ILExecThreadNew(thread, typeName, "(T)V");
	}
	_ILSetExceptionStackTrace(thread, object);
	if(!_ILExecThreadHasException(thread))
	{
		_ILExecThreadSetException(thread, object);
	}
}

void ILExecThreadThrowArgRange(ILExecThread *thread, const char *paramName,
							   const char *resourceName)
{
	ILMethod *method;
	ILString *resourceString = 0;
	ILString *paramString;
	ILObject *object;

	/* Bail out if there already is a pending exception or if the thread is aborting */
	if(_ILExecThreadHasException(thread))
	{
		return;
	}

	/* Look for the "System.Object._" method, which allows us to
	   look up localised error messages */
	if(resourceName)
	{
		method = ILExecThreadLookupMethod(thread, "System.Object", "_",
										  "(oSystem.String;)oSystem.String;");
		if(method)
		{
			resourceString = ILStringCreate(thread, resourceName);
			if(resourceString)
			{
				ILExecThreadCall(thread, method, &resourceString,
								 resourceString);
			}
			if(_ILExecThreadHasException(thread))
			{
				/* The string create or lookup threw an exception */
				return;
			}
		}
	}

	/* Convert the parameter name into a string */
	paramString = ILStringCreate(thread, paramName);
	if(_ILExecThreadHasException(thread))
	{
		/* The string create threw an exception */
		return;
	}

	/* Create the new exception object and throw it */
	if(resourceString)
	{
		object = ILExecThreadNew(thread, "System.ArgumentOutOfRangeException",
								 "(ToSystem.String;oSystem.String;)V",
								 paramString, resourceString);
	}
	else
	{
		object = ILExecThreadNew(thread, "System.ArgumentOutOfRangeException",
								 "(ToSystem.String;)V",
								 paramString);
	}
	_ILSetExceptionStackTrace(thread, object);
	if(!_ILExecThreadHasException(thread))
	{
		_ILExecThreadSetException(thread, object);
	}
}

void ILExecThreadThrowArgNull(ILExecThread *thread, const char *paramName)
{
	ILString *paramString;
	ILObject *object;

	/* Bail out if there already is a pending exception or if the thread is aborting */
	if(_ILExecThreadHasException(thread))
	{
		return;
	}

	/* Convert the parameter name into a string */
	paramString = ILStringCreate(thread, paramName);
	if(_ILExecThreadHasException(thread))
	{
		/* The string create threw an exception */
		return;
	}

	/* Create the new exception object and throw it */
	object = ILExecThreadNew(thread, "System.ArgumentNullException",
							 "(ToSystem.String;)V", paramString);
	_ILSetExceptionStackTrace(thread, object);
	if(!_ILExecThreadHasException(thread))
	{
		_ILExecThreadSetException(thread, object);
	}
}

void ILExecThreadThrowOutOfMemory(ILExecThread *thread)
{
	/* Note: Eventhough the exception maybe of OutOfMemory, this may
	 * be an accidental error */
	_ILSetExceptionStackTrace(thread, thread->process->outOfMemoryObject);
	if(!_ILExecThreadHasException(thread))
	{
		_ILExecThreadSetException(thread, thread->process->outOfMemoryObject);
	}
}

ILObject *_ILExecThreadNewThreadAbortException(ILExecThread *thread, ILObject *stateInfo)
{
	ILObject *object;

	/* Bail out if there already is a pending exception */
	object = ILExecThreadNew(thread, "System.Threading.ThreadAbortException",
						"(ToSystem.Object;)V", stateInfo);
	
	_ILSetExceptionStackTrace(thread, object);
	
	return object;
}

int ILExecThreadIsThreadAbortException(ILExecThread *thread, ILObject *object)
{
	return GetObjectClass(object) == thread->process->threadAbortClass;
}

void ILExecThreadPrintException(ILExecThread *thread)
{
#ifndef REDUCED_STDIO
	ILObject *exception;
	ILString *str;
	char *ansistr;
	ILClass *classInfo;
	ILClass *exceptionClass;
	ILField *field;
	ILObject *stackTrace;
	ILInt32 length;
	ILInt32 posn;
	PackedStackFrame *frames;
	ILMethod *method;
	ILImage *image;
	ILDebugContext *dbg;
	const char *filename;
	ILUInt32 line;
	ILUInt32 col;

	/* Get the exception object from the thread.  If there is no object,
	   then assume that memory is exhausted */
	exception = _ILExecThreadGetException(thread);
	_ILExecThreadClearException(thread);
	if(!exception)
	{
		exception = thread->process->outOfMemoryObject;
	}

	/* Attempt to use "ToString" to format the exception, but not
	   if we know that the exception is reporting out of memory */
	if(exception != thread->process->outOfMemoryObject)
	{
		str = ILObjectToString(thread, exception);
		if(str != 0 && (ansistr = ILStringToAnsi(thread, str)) != 0)
		{
			fputs("Uncaught exception: ", stderr);
			fputs(ansistr, stderr);
			putc('\n', stderr);
			return;
		}
	}

	/* Print the class information for the exception */
	fputs("Uncaught exception: ", stderr);
	classInfo = GetObjectClass(exception);
	if(ILClass_Namespace(classInfo))
	{
		fputs(ILClass_Namespace(classInfo), stderr);
		putc('.', stderr);
	}
	fputs(ILClass_Name(classInfo), stderr);
	putc('\n', stderr);

	/* Extract the stack trace from the exception object */
	exceptionClass = ILExecThreadLookupClass(thread, "System.Exception");
	stackTrace = 0;
	if(exceptionClass && ILClassInheritsFrom(classInfo, exceptionClass))
	{
		field = 0;
		while((field = (ILField *)ILClassNextMemberByKind
					(exceptionClass, (ILMember *)field,
					 IL_META_MEMBERKIND_FIELD)) != 0)
		{
			if(!strcmp(ILField_Name(field), "stackTrace"))
			{
				stackTrace = *((ILObject **)(((unsigned char *)exception) +
											 field->offset));
				break;
			}
		}
	}

	/* Print the exception stack trace */
	if(stackTrace)
	{
		frames = (PackedStackFrame *)ArrayToBuffer(stackTrace);
		length = ArrayLength(stackTrace);
		for(posn = 0; posn < length; ++posn)
		{
			method = frames[posn].method;
			if(method)
			{
				fputs("\tat ", stderr);
				classInfo = ILMethod_Owner(method);
				if(ILClass_Namespace(classInfo))
				{
					fputs(ILClass_Namespace(classInfo), stderr);
					putc('.', stderr);
				}
				fputs(ILClass_Name(classInfo), stderr);
				putc('.', stderr);
				fputs(ILMethod_Name(method), stderr);
				putc('(', stderr);
				putc('?', stderr);
				putc(')', stderr);
				image = ILProgramItem_Image(method);
				if(ILDebugPresent(image) && (dbg = ILDebugCreate(image)) != 0)
				{
					filename = ILDebugGetLineInfo
						(dbg, ILMethod_Token(method),
						 frames[posn].offset, &line, &col);
					if(filename)
					{
						fputs(" in ", stderr);
						fputs(filename, stderr);
						fprintf(stderr, ":%ld", (long)line);
					}
					ILDebugDestroy(dbg);
				}
				putc('\n', stderr);
			}
		}
	}
#endif
}

#ifdef	__cplusplus
};
#endif
