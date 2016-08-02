/*
 * il_debugger.h - Debugger interface for engine.
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

#ifndef	_IL_DEBUGGER_H
#define	_IL_DEBUGGER_H

/*
 * Compile debugger only if configured in profile and if we have tools support.
 */
#if defined(IL_CONFIG_DEBUGGER) && !defined(IL_WITHOUT_TOOLS)
	#define IL_DEBUGGER 1
#endif

#ifdef IL_DEBUGGER

#include "il_sysio.h"
#include "il_engine.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Opaque type declarations.
 */
typedef struct _tagILDebugger			ILDebugger;
typedef struct _tagILBreakpoint 		ILBreakpoint;
typedef struct _tagILDebuggerThreadInfo		ILDebuggerThreadInfo;
typedef struct _tagILAssemblyWatch 		ILAssemblyWatch;
typedef struct _tagILDebuggerWatch 		ILDebuggerWatch;
typedef struct _tagILDebuggerIO			ILDebuggerIO;

/*
 * Function type that is used for hooking execution stops. (not used yet)
 */
typedef int (*ILDebuggerStopHookFunc)(ILDebugger *debugger,
								   int reserved);

/*
 * Streams and functions for communication with debugger client.
 */
struct _tagILDebuggerIO
{
	/*
	 * Implementation specific data.
	 */
	void *data1;
	void *data2;

	/*
	 * Handle for socket stream implementation.
	 */
	ILSysIOHandle sockfd;

	/*
	 * Stream used for recieving data.
	 * IO fills it with NIL terminated string on recieve.
	 */
	FILE *input;

	/*
	 * Stream used for sending data.
	 * IO sends it's NIL terminated content to client on send.
	 */
	FILE *output;

	/*
	 * Open connection and IO streams. Return 1 on success, 0 on error.
	 */
	int (*open)(ILDebuggerIO *stream, const char *connectionString);

	/*
	 * Close connection and release implementation specific resources.
	 */
	void (*close)(ILDebuggerIO *stream);

	/*
	 * Recieve data from client.
	 * Return 1 on success, 0 on errro.
	 */
	int (*recieve)(ILDebuggerIO *stream);

	/*
	 * Send data to client.
	 * Return 1 on success, 0 on errro.
	 */
	int (*send)(ILDebuggerIO *stream);
};

/*
 * Create debugger and attach it to process.
  * Connect to debugger client and return stream or 0 on error.
 * Supported connectionString formats are following:
 *     tcp://localhost:port_number

 * If 0 is passed as connectionString, then debugger tries to use
 * -debugger command line option or IL_DEBUGGER environment variable.
 * Return 0 on failure.
 */
ILDebugger *ILDebuggerCreate(ILExecProcess *process);

/*
 * Set IO for debugger.
 */
void ILDebuggerSetIO(ILDebugger *debugger, ILDebuggerIO *io);

/*
 * Connect to the debugger client using debuggger IO.
 * Return 1 on success, 0 on failure.
 * If IO was not previously set with ILDebuggerSetIO()
 * debugger tries to create io from known connectionString scheme.
 */
int ILDebuggerConnect(ILDebugger *debugger, char *connectionString);

/*
 * Notify debugger that the process is terminating.
 * This will notify client and terminate IO thread.
 */
void ILDebuggerRequestTerminate(ILDebugger *debugger);

/*
 * Destroy debugger and IO connection to debugger client.
 */
void ILDebuggerDestroy(ILDebugger *debugger);

/*
 * Determine if debugger is attached to exec process.
 */
 int ILDebuggerIsAttached(ILExecProcess *process);

/*
 * Get debugger attached to given process.
 * Return 0 if debugger is not attached.
 */
 ILDebugger *ILDebuggerFromProcess(ILExecProcess *process);

#ifdef	__cplusplus
};
#endif

#endif	/* IL_CONFIG_DEBUGGER */

#endif	/* _IL_DEBUGGER_H */
