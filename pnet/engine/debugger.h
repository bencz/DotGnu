/*
 * debugger.h - debugger definitions.
 *
 * Copyright (C) 2005  Southern Storm Software, Pty Ltd.
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

#ifndef	_DEBUGGER_H
#define	_DEBUGGER_H

#include "il_debugger.h"
#ifdef IL_USE_JIT
#include "jit/jit-except.h"
#endif

#ifdef IL_DEBUGGER

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Run types.
 */
#define	IL_DEBUGGER_RUN_TYPE_STOPPED		0
#define	IL_DEBUGGER_RUN_TYPE_CONTINUE		1
#define	IL_DEBUGGER_RUN_TYPE_STEP			2
#define	IL_DEBUGGER_RUN_TYPE_NEXT			3
#define	IL_DEBUGGER_RUN_TYPE_FINISH			4
#define	IL_DEBUGGER_RUN_TYPE_DETACHED		5
#define	IL_DEBUGGER_RUN_TYPE_UNTIL			6

#define IL_DEBUGGER_COMMAND_MAX_ARG_COUNT	3


/*
 * Definitions for user data types.
 */
#define IL_USER_DATA_IMAGE_ID					0
#define IL_USER_DATA_CLASS_ID					1
#define IL_USER_DATA_SOURCE_FILE_ID				2
#define IL_USER_DATA_SOURCE_FILE_IMAGE_ID		3
#define IL_USER_DATA_MEMBER_ID					4
#define IL_USER_DATA_METHOD_CALL_COUNT			5
#define IL_USER_DATA_METHOD_TIME				6

#define IL_USER_DATA_TABLE_INIT_SIZE			509

typedef struct _tagILUserDataEntry		ILUserDataEntry;
typedef struct _tagILUserData			ILUserData;

struct _tagILUserDataEntry
{
	int			type;
	const void *ptr;
	void       *data;
};

struct _tagILUserData
{
	int 				count;		/* Number of used entries */
	int 				size;		/* Table size */
	int 				shiftsMax;	/* Longest shift because of collisions */
	ILUserDataEntry    *entries;	/* Pointer to the first entry */
};

/*
 * Determine if thread is unbreakable and untouchable in coder's debug hook.
 *
 * Such thread has stopped in debug hook and is now calling managed method
 * e.g. ToString() for displaying watch.
 * 
 * The engine must prevent calling debug hook and changing debugger related
 * stuff of such thread. Otherwise it leads to deadlock or other disfunctions.
 * For example if thread->frame gets changed this affects show_locals command
 * which relies on this value.
 */
#define ILDebuggerIsThreadUnbreakable(thread) \
	(thread->process->debugger->dbthread->runType == IL_DEBUGGER_RUN_TYPE_STOPPED && \
	 thread->process->debugger->dbthread->execThread == thread)

/*
 * Check whether debugger is watching method's assembly.
 */
int ILDebuggerIsAssemblyWatched(ILDebugger *debugger, ILMethod *method);

/*
 * Information about a thread that is under the control of the debugger.
 */
struct _tagILDebuggerThreadInfo
{
	/* User id used to identify this thread */
	int id;

	/* Info from hook function */
	ILExecProcess *process;
	ILExecThread *execThread;
	void *userData;
	ILMethod *method;
	ILInt32 offset;
	int type;

	/* Wakeups the thread to execute command or continue execution */
	ILWaitHandle *event;

	/* Current location in source file */
	const char *sourceFile;
	ILUInt32 line;
	ILUInt32 col;

	/* Stack trace height */
	ILInt32 numFrames;

#ifdef IL_USE_JIT
	/* JIT stack trace */
	jit_stack_trace_t jitStackTrace;
#endif

	/* Method where profiler stopped last time */
	ILMethod *profilerLastMethod;

	/* Previous profiler stop time */
	ILCurrTime profilerLastStopTime;

	int volatile runType;
	ILDebuggerThreadInfo *next;
};

/*
 * Debugger breakpoint placed by user.
 */
struct _tagILBreakpoint
{
	int id;
	ILMethod *method;
	ILUInt32 offset;
	const char *sourceFile;
	ILUInt32 line;
	ILUInt32 col;
	ILBreakpoint *next;
};

/*
 * Information about watched assembly (image).
 */
struct _tagILAssemblyWatch
{
	ILImage *image;

	/* Watch count - 0 if watch is disabled */
	int count;

	ILAssemblyWatch *next;
};

/*
 * Watch info for local variable, field, property or function call.
 */
struct _tagILDebuggerWatch
{
	/* Expression to watch */
	char *expression;

	/* Next watch in this linked list */
	ILDebuggerWatch *next;
};

/*
 * Structure of a debugger instance.
 */
struct _tagILDebugger
{
	/* Lock to serialize access to this object */
	ILMutex *lock;

	/* Reference to current exec process */
	ILExecProcess *process;

	/* Debugger client stream */
	ILDebuggerIO *io;

	/* Current command */
	char *cmd;

	/* Last argument for current command */
	char *lastArg;

	/* Used to signal that command execution finnished */
	ILWaitHandle *event;

	/* Thread that is recieving, sending and sometimes executing commands.
	   Can be 0 if debugging runs in single thread. */
	ILThread *ioThread;

	/* Currently debugged thread followed by other stopped threads */
	ILDebuggerThreadInfo * volatile dbthread;

	/* Linked list of watched assemblies or NULL to watch all */
	ILAssemblyWatch *assemblyWatches;

	/* Linked list of watched local variables, fields, properties or function
	   calls */
	ILDebuggerWatch *watches;
	
	/* Linked list of user breakpoints */
	ILBreakpoint *breakpoint;

	/* Table with additional data attached to various objects */
	ILUserData *userData;

	/* Smallest used id e.g. for project, source file or type */
	int minId;

	/* Flag used to stop all threads after break command */
	int breakAll;

	/* Nonzero to collect profiling information */
	int profiling;

	/* Return IL_HOOK_ABORT from hook function when this flag is set */ 
	int volatile abort;

	/* Nonzero to not start IO thread */
	int dontStartIoThread;

};


#ifdef	__cplusplus
};
#endif

#endif	/* IL_DEBUGGER */

#endif	/* _DEBUGGER_H */
