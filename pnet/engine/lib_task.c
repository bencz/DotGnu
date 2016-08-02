/*
 * lib_task.c - Internalcall methods for the "Platform.TaskMethods" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
#include "il_utils.h"
#include "il_sysio.h"
#include "il_thread.h"
#include "il_errno.h"
#ifdef IL_WIN32_PLATFORM
	#include <windows.h>
	#include <io.h>
	#include <fcntl.h>
	#ifdef IL_WIN32_CYGWIN
		#include <unistd.h>
	#endif
	#ifndef IL_WIN32_NATIVE
		#ifdef HAVE_SYS_CYGWIN_H
			#include <sys/cygwin.h>
		#endif
	#endif
#else
	#ifdef HAVE_SYS_TYPES_H
		#include <sys/types.h>
	#endif
	#ifdef HAVE_UNISTD_H
		#include <unistd.h>
	#endif
	#ifdef HAVE_SYS_WAIT_H
		#include <sys/wait.h>
	#endif
	#ifndef WEXITSTATUS
		#define	WEXITSTATUS(status)		((unsigned)(status) >> 8)
	#endif
	#ifndef WIFEXITED
		#define	WIFEXITED(status)		(((status) & 255) == 0)
	#endif
	#ifndef WTERMSIG
		#define	WTERMSIG(status)		(((unsigned)(status)) & 0x7F)
	#endif
	#ifndef WIFSIGNALLED
		#define	WIFSIGNALLED(status)	(((status) & 255) != 0)
	#endif
	#ifndef WCOREDUMP
		#define	WCOREDUMP(status)		(((status) & 0x80) != 0)
	#endif
	#ifdef HAVE_FCNTL
		#include <fcntl.h>
	#endif
	#include <signal.h>
	#include <stdlib.h>
	#include <errno.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * public static void Exit(int exitCode);
 */
void _IL_TaskMethods_Exit(ILExecThread *thread, ILInt32 exitCode)
{
#if !defined(__palmos__)
	exit((int)(exitCode & 0xFF));
#endif
}

/*
 * public static void SetExitCode(int exitCode);
 */
void _IL_TaskMethods_SetExitCode(ILExecThread *thread, ILInt32 exitCode)
{
	thread->process->exitStatus = exitCode;
}

/*
 * public static String[] GetCommandLineArgs();
 */
System_Array *_IL_TaskMethods_GetCommandLineArgs(ILExecThread *thread)
{
	return (System_Array *)(thread->process->commandLineObject);
}

/*
 * public static String GetEnvironmentVariable(String name);
 */
ILString *_IL_TaskMethods_GetEnvironmentVariable
				(ILExecThread *thread, ILString *name)
{
#if !defined(__palmos__)
	char *nameAnsi = ILStringToAnsi(thread, name);
	char *env;
	if(nameAnsi)
	{
		env = getenv(nameAnsi);
		if(env)
		{
			return ILStringCreate(thread, env);
		}
	}
#endif
	return 0;
}

/*
 * Import the "environ" variable, so that we can walk the environment.
 */
extern char **environ;

/*
 * public static int GetEnvironmentCount();
 */
ILInt32 _IL_TaskMethods_GetEnvironmentCount(ILExecThread *thread)
{
	ILInt32 count = 0;
	char **env = environ;
	while(env && *env != 0)
	{
		++count;
		++env;
	}
	return count;
}

/*
 * public static String GetEnvironmentKey(int posn);
 */
ILString *_IL_TaskMethods_GetEnvironmentKey(ILExecThread *thread, ILInt32 posn)
{
	ILInt32 count = _IL_TaskMethods_GetEnvironmentCount(thread);
	char *env;
	int len;
	if(posn >= 0 && posn < count)
	{
		env = environ[posn];
		len = 0;
		while(env[len] != '\0' && env[len] != '=')
		{
			++len;
		}
		return ILStringCreateLen(thread, env, len);
	}
	else
	{
		return 0;
	}
}

/*
 * public static String GetEnvironmentValue(int posn);
 */
ILString *_IL_TaskMethods_GetEnvironmentValue(ILExecThread *thread,
											  ILInt32 posn)
{
	ILInt32 count = _IL_TaskMethods_GetEnvironmentCount(thread);
	char *env;
	int len;
	if(posn >= 0 && posn < count)
	{
		env = environ[posn];
		len = 0;
		while(env[len] != '\0' && env[len] != '=')
		{
			++len;
		}
		if(env[len] == '=')
		{
			++len;
		}
		return ILStringCreate(thread, env + len);
	}
	else
	{
		return 0;
	}
}

/*
 * private static int GetHandleCount(IntPtr processHandle);
 */
ILInt32 _IL_Process_GetHandleCount(ILExecThread *_thread,
								   ILNativeInt processHandle)
{
#ifdef IL_WIN32_PLATFORM
	/* TODO */
	return -1;
#else
	/* Returning -1 tells the caller to assume that there
	   are 3 handles for stdin, stdout, and stderr */
	return -1;
#endif
}

#ifdef IL_WIN32_PLATFORM

/*
 * User data for "EnumCallback".
 */
typedef struct
{
	DWORD	processID;
	HWND	found;

} EnumCallbackData;

/*
 * Callback for "EnumWindows".
 */
static BOOL CALLBACK EnumCallback(HWND hWnd, LPARAM lParam)
{
	EnumCallbackData *data = (EnumCallbackData *)lParam;
	DWORD processID = 0;
	GetWindowThreadProcessId(hWnd, &processID);
	if(data->processID == processID &&
	   GetWindow(hWnd, GW_OWNER) == NULL &&
	   IsWindowVisible(hWnd))
	{
		data->found = hWnd;
		return FALSE;
	}
	return TRUE;
}

#endif /* IL_WIN32_PLATFORM */

/*
 * private static IntPtr GetMainWindowHandle(int processID);
 */
ILNativeInt _IL_Process_GetMainWindowHandle(ILExecThread *_thread,
											ILInt32 processID)
{
#ifdef IL_WIN32_PLATFORM
	EnumCallbackData data;
	data.processID = (DWORD)processID;
	data.found = NULL;
	EnumWindows(EnumCallback, (LPARAM)&data);
	return (ILNativeInt)(data.found);
#else
	/* Non-Win32 platforms don't have a notion of "main window" */
	return 0;
#endif
}

/*
 * private static String GetMainWindowTitle(IntPtr windowHandle);
 */
ILString *_IL_Process_GetMainWindowTitle(ILExecThread * _thread,
										 ILNativeInt windowHandle)
{
#ifdef IL_WIN32_PLATFORM
	int len = GetWindowTextLength((HWND)windowHandle);
	char *buf;
	ILString *str;
	if(len <= 0)
	{
		return 0;
	}
	buf = (char *)ILMalloc(len + 1);
	if(!buf)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	str = ILStringCreate(_thread, buf);
	ILFree(buf);
	return str;
#else
	/* Non-Win32 platforms don't have a notion of "main window" */
	return 0;
#endif
}

/*
 * private static int GetProcessorAffinity(IntPtr processHandle);
 */
ILInt32 _IL_Process_GetProcessorAffinity(ILExecThread *_thread,
										 ILNativeInt processHandle)
{
#ifdef IL_WIN32_PLATFORM
	DWORD processAffinity, systemAffinity;
	if(GetProcessAffinityMask((HANDLE)processHandle,
							  &processAffinity, &systemAffinity))
	{
		return (ILInt32)processAffinity;
	}
	else
	{
		/* Something went wrong - assume execution on CPU #1 */
		return 1;
	}
#else
	/* We have no way to get the affinity on non-Win32 systems,
	   so just assume that the process is always on CPU #1 */
	return 1;
#endif
}

/*
 * private static bool MainWindowIsResponding(IntPtr windowHandle);
 */
ILBool _IL_Process_MainWindowIsResponding(ILExecThread *_thread,
										  ILNativeInt windowHandle)
{
#ifdef IL_WIN32_PLATFORM
	/* Send message zero to the window, to ping it.  If we don't get a
	   response, then assume that the window is no longer responding */
	LRESULT result = 0;
	if(SendMessageTimeout((HWND)windowHandle, 0, 0, 0,
						  SMTO_ABORTIFHUNG, 5000, &result) != 0)
	{
		return 1;
	}
	else
	{
		return 0;
	}
#else
	/* Non-Win32 platforms don't have main windows, so just pretend
	   that the application's "main window" is responding */
	return 1;
#endif
}

/*
 * private static void CloseProcess(IntPtr processHandle, int processID);
 */
void _IL_Process_CloseProcess(ILExecThread *_thread,
							  ILNativeInt processHandle,
							  ILInt32 processID)
{
#ifdef IL_WIN32_PLATFORM
	CloseHandle((HANDLE)processHandle);
#endif
}

/*
 * private static bool CloseMainWindow(IntPtr windowHandle);
 */
ILBool _IL_Process_CloseMainWindow(ILExecThread * _thread,
								   ILNativeInt windowHandle)
{
#ifdef IL_WIN32_PLATFORM
	/* Don't close if the window is currently disabled */
	if((GetWindowLong((HWND)windowHandle, GWL_STYLE) & WS_DISABLED) == 0)
	{
		PostMessage((HWND)windowHandle, WM_CLOSE, 0, 0);
		return 1;
	}
	return 0;
#else
	/* Non-Win32 platforms don't have a notion of "main window" */
	return 0;
#endif
}

/*
 * private static void GetCurrentProcessInfo(out int processID,
 *											 out IntPtr handle);
 */
void _IL_Process_GetCurrentProcessInfo(ILExecThread *_thread,
									   ILInt32 *processID,
									   ILNativeInt *handle)
{
#ifdef IL_WIN32_PLATFORM
	*processID = (ILInt32)(GetCurrentProcessId());
	*handle = (ILNativeInt)(GetCurrentProcess());
#elif defined(HAVE_GETPID)
	*processID = (ILInt32)(getpid());
	*handle = 0;
#else
	*processID = 0;
	*handle = 0;
#endif
}

/*
 * private static void KillProcess(IntPtr processHandle, int processID);
 */
void _IL_Process_KillProcess(ILExecThread *_thread,
							 ILNativeInt processHandle, ILInt32 processID)
{
#ifdef IL_WIN32_PLATFORM
	TerminateProcess((HANDLE)processHandle, (UINT)(-1));
#elif defined(SIGKILL)
	kill((int)processID, SIGKILL);
#endif
}

/*
 * Process start flags.
 */
#define	ProcessStart_CreateNoWindow		0x0001
#define	ProcessStart_ErrorDialog		0x0002
#define	ProcessStart_RedirectStdin		0x0004
#define	ProcessStart_RedirectStdout		0x0008
#define	ProcessStart_RedirectStderr		0x0010
#define	ProcessStart_UseShellExecute	0x0020
#define	ProcessStart_ExecOverTop		0x0040

/*
 * private static bool StartProcess(String filename, String arguments,
 *									String workingDir,
 *									String[] argv, int flags,
 *									int windowStyle, String[] envVars,
 *									String verb, IntPtr errorDialogParent,
 *									out IntPtr processHandle,
 *									out int processID,
 *									out IntPtr stdinHandle,
 *									out IntPtr stdoutHandle,
 *									out IntPtr stderrHandle);
 */
ILBool _IL_Process_StartProcess(ILExecThread *_thread,
								ILString *filename,
								ILString *arguments,
								ILString *workingDir,
								System_Array *argv,
								ILInt32 flags,
								ILInt32 windowStyle,
								System_Array *envVars,
								ILString *verb,
								ILNativeInt errorDialogParent,
								ILNativeInt *processHandle,
								ILInt32 *processID,
								ILNativeInt *stdinHandle,
								ILNativeInt *stdoutHandle,
								ILNativeInt *stderrHandle)
{
#ifdef IL_WIN32_PLATFORM

#ifdef IL_WIN32_CYGWIN
	#define	GET_OSF(fd)		((HANDLE)(get_osfhandle((fd))))
	#define	MAKE_PIPE(fds)	(pipe((fds)))
#else
	#define	GET_OSF(fd)		((HANDLE)(_get_osfhandle((fd))))
	#define	MAKE_PIPE(fds)	(_pipe((fds), 0, _O_BINARY))
#endif

	const char *fname;
	const char *workdir = 0;
	char *args;
	STARTUPINFO startupInfo;
	PROCESS_INFORMATION processInfo;
	char *env = 0;
	ILBool result;
	int pipefds[2];
	int cleanups[8];
	int numCleanups = 0;
	int closeAfterFork[8];
	int numCloseAfterFork = 0;
	int index;
#if defined(HAVE_SYS_CYGWIN_H) && defined(HAVE_CYGWIN_CONV_TO_WIN32_PATH)
	char cygFname[4096];
	char cygWorkdir[4096];
#endif

	/* Clear errno, because we will check for it when something fails */
	ILSysIOSetErrno(0);

	/* Convert the parameters into something that the OS can understand */
	fname = ILStringToAnsi(_thread, filename);
	if(!fname)
	{
		return 0;
	}

#if defined(HAVE_SYS_CYGWIN_H) && defined(HAVE_CYGWIN_CONV_TO_WIN32_PATH)
	/* Use the Cygwin-supplied function to convert the path
	   that CreateProcess() will understand */
	if(cygwin_conv_to_win32_path(fname, cygFname) == 0)
	{
		fname = cygFname;
	}
#endif

	if(((System_String *) workingDir)->length)
	{
		workdir = ILStringToAnsi(_thread, workingDir);
		if(!workdir)
		{
			return 0;
		}
#if defined(HAVE_SYS_CYGWIN_H) && defined(HAVE_CYGWIN_CONV_TO_WIN32_PATH)
		/* Use convert workdir for CreateProcess() under cygwin */
		if(cygwin_conv_to_win32_path(workdir, cygWorkdir) == 0)
		{
			workdir = cygWorkdir;
		}
#endif
	}
	args = ILStringToAnsi(_thread, arguments);
	if(!args)
	{
		return 0;
	}
	ILMemZero(&startupInfo, sizeof(startupInfo));
	startupInfo.cb = sizeof(STARTUPINFO);
	startupInfo.dwFlags = STARTF_USESHOWWINDOW;
	startupInfo.wShowWindow = (WORD)windowStyle;

	/* Redirect stdin, stdout, and stderr if necessary */
	*stdinHandle = (ILNativeInt)(ILSysIOHandle_Invalid);
	*stdoutHandle = (ILNativeInt)(ILSysIOHandle_Invalid);
	*stderrHandle = (ILNativeInt)(ILSysIOHandle_Invalid);
	if((flags & (ProcessStart_RedirectStdin |
				 ProcessStart_RedirectStdout |
				 ProcessStart_RedirectStderr)) != 0)
	{
		startupInfo.dwFlags |= STARTF_USESTDHANDLES;
		if((flags & ProcessStart_RedirectStdin) != 0)
		{
			MAKE_PIPE(pipefds);
			*stdinHandle = (ILNativeInt)(pipefds[1]);
			SetHandleInformation(GET_OSF(pipefds[1]),
								 HANDLE_FLAG_INHERIT, 0);
			startupInfo.hStdInput = GET_OSF(pipefds[0]);
			cleanups[numCleanups++] = pipefds[0];
			cleanups[numCleanups++] = pipefds[1];
			closeAfterFork[numCloseAfterFork++] = pipefds[0];
		}
		else
		{
			startupInfo.hStdInput = GET_OSF(0);
		}
		if((flags & ProcessStart_RedirectStdout) != 0)
		{
			MAKE_PIPE(pipefds);
			*stdoutHandle = (ILNativeInt)(pipefds[0]);
			SetHandleInformation(GET_OSF(pipefds[0]),
								 HANDLE_FLAG_INHERIT, 0);
			startupInfo.hStdOutput = GET_OSF(pipefds[1]);
			cleanups[numCleanups++] = pipefds[0];
			cleanups[numCleanups++] = pipefds[1];
			closeAfterFork[numCloseAfterFork++] = pipefds[1];
		}
		else
		{
			startupInfo.hStdOutput = GET_OSF(1);
		}
		if((flags & ProcessStart_RedirectStderr) != 0)
		{
			MAKE_PIPE(pipefds);
			*stderrHandle = (ILNativeInt)(pipefds[0]);
			SetHandleInformation(GET_OSF(pipefds[0]),
								 HANDLE_FLAG_INHERIT, 0);
			startupInfo.hStdError = GET_OSF(pipefds[1]);
			cleanups[numCleanups++] = pipefds[0];
			cleanups[numCleanups++] = pipefds[1];
			closeAfterFork[numCloseAfterFork++] = pipefds[1];
		}
		else
		{
			startupInfo.hStdError = GET_OSF(2);
		}
	}

	/* TODO: shell execution, environment variables, and ExecOverTop */

	/* Launch the process */
	*processID = -1;
	*processHandle = 0;
	result = 0;
	if(CreateProcess(fname, args, NULL, NULL, TRUE, 0, env, workdir,
					 &startupInfo, &processInfo))
	{
		*processHandle = (ILNativeInt)(processInfo.hProcess);
		*processID = (ILInt32)(processInfo.dwProcessId);
		result = 1;
	}
	else
	{
		/* TODO: it could be useful to report error details using GetLastError().
		   We can't do ILSysIOSetErrno(GetLastError()), because errno is related
		   to IO function, while GetLastError() is windows-specific error code */
		ILSysIOSetErrno(ENOENT);
	}

	/* Clean up and exit */
	if(env)
	{
		ILFree(env);
	}
	if(result)
	{
		for(index = 0; index < numCloseAfterFork; ++index)
		{
			close(closeAfterFork[index]);
		}
	}
	else
	{
		for(index = 0; index < numCleanups; ++index)
		{
			close(cleanups[index]);
		}
	}
	return result;

#elif defined(HAVE_FORK) && defined(HAVE_EXECV) && (defined(HAVE_WAITPID) || defined(HAVE_WAIT))
#define	IL_USING_FORK	1

	const char *fname;
	const char *workdir = 0;
	char **args;
	char **newEnviron = 0;
	int varNum = 0;
	int stdinFds[2] = {-1, -1};
	int stdoutFds[2] = {-1, -1};
	int stderrFds[2] = {-1, -1};
	ILBool result = 0;
	int pid;
	ILInt32 argc;
	const char *ansi;
	int pipefds[2];
#define	FreeStringList(list,size)	\
		do { \
			if((list)) \
			{ \
				int __posn = (int)(size); \
				while(__posn > 0) \
				{ \
					--__posn; \
					ILFree((list)[__posn]); \
				} \
				ILFree((list)); \
			} \
		} while (0)

	/* Convert the parameters into something that the OS can understand */
	fname = ILStringToAnsi(_thread, filename);
	if(!fname)
	{
		return 0;
	}
	if(((System_String *) workingDir)->length)
	{
		workdir = ILStringToAnsi(_thread, workingDir);
		if(!workdir)
		{
			return 0;
		}
	}
	args = (char **)ILCalloc(ArrayLength(argv) + 1, sizeof(char *));
	if(!args)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	argc = 0;
	while(argc < ArrayLength(argv))
	{
		ansi = ILStringToAnsi
			(_thread, ((ILString **)ArrayToBuffer(argv))[argc]);
		if(!ansi)
		{
			FreeStringList(args, argc);
			return 0;
		}
		args[argc] = (char *)ILMalloc(strlen(ansi) + 1);
		if(!(args[argc]))
		{
			FreeStringList(args, argc);
			return 0;
		}
		strcpy(args[argc], ansi);
		++argc;
	}
	args[argc] = 0;

	/* Convert the environment */
	if(envVars)
	{
		newEnviron = (char **)ILCalloc(ArrayLength(envVars) + 1, sizeof(char *));
		if(!newEnviron)
		{
			ILExecThreadThrowOutOfMemory(_thread);
			FreeStringList(args, argc);
			return 0;
		}
		while(varNum < (int)(ArrayLength(envVars)))
		{
			ansi = ILStringToAnsi
				(_thread, ((ILString **)ArrayToBuffer(envVars))[varNum]);
			if(!ansi)
			{
				FreeStringList(args, argc);
				FreeStringList(newEnviron, varNum);
				return 0;
			}
			newEnviron[varNum] = (char *)ILMalloc(strlen(ansi) + 1);
			if(!(newEnviron[varNum]))
			{
				FreeStringList(args, argc);
				FreeStringList(newEnviron, varNum);
				return 0;
			}
			strcpy(newEnviron[varNum], ansi);
			++varNum;
		}
		newEnviron[varNum] = 0;
	}

	/* Redirect stdin, stdout, and stderr as necessary */
	*stdinHandle = (ILNativeInt)(ILSysIOHandle_Invalid);
	*stdoutHandle = (ILNativeInt)(ILSysIOHandle_Invalid);
	*stderrHandle = (ILNativeInt)(ILSysIOHandle_Invalid);
	if((flags & ProcessStart_RedirectStdin) != 0)
	{
		if(pipe(stdinFds) < 0)
		{
			return 0;
		}
	#if HAVE_FCNTL
		fcntl(stdinFds[1], F_SETFD, 1);
	#endif
		*stdinHandle = (ILNativeInt)(stdinFds[1]);
	}
	if((flags & ProcessStart_RedirectStdout) != 0)
	{
		if(pipe(stdoutFds) < 0)
		{
			if((flags & ProcessStart_RedirectStdin) != 0)
			{
				close(stdinFds[0]);
				close(stdinFds[1]);
			}
			return 0;
		}
	#if HAVE_FCNTL
		fcntl(stdoutFds[0], F_SETFD, 1);
	#endif
		*stdoutHandle = (ILNativeInt)(stdoutFds[0]);
	}
	if((flags & ProcessStart_RedirectStderr) != 0)
	{
		if(pipe(stderrFds) < 0)
		{
			if((flags & ProcessStart_RedirectStdin) != 0)
			{
				close(stdinFds[0]);
				close(stdinFds[1]);
			}
			if((flags & ProcessStart_RedirectStdout) != 0)
			{
				close(stdoutFds[0]);
				close(stdoutFds[1]);
			}
			return 0;
		}
	#if HAVE_FCNTL
		fcntl(stderrFds[0], F_SETFD, 1);
	#endif
		*stderrHandle = (ILNativeInt)(stderrFds[0]);
	}

	/* Open the pipe for returning errno */
	if(pipe(pipefds) < 0)
	{
		return 0;
	}
	
	/* Fork and execute the process */
	*processID = -1;
	*processHandle = 0;
	pid = fork();
	if(pid == 0)
	{
		/* We are in the child process */
		if((flags & ProcessStart_RedirectStdin) != 0)
		{
			dup2(stdinFds[0], 0);
			close(stdinFds[0]);
		}
		if((flags & ProcessStart_RedirectStdout) != 0)
		{
			dup2(stdoutFds[1], 1);
			close(stdoutFds[1]);
		}
		if((flags & ProcessStart_RedirectStderr) != 0)
		{
			dup2(stderrFds[1], 2);
			close(stderrFds[1]);
		}
		if(newEnviron)
		{
			extern char **environ;
			environ = newEnviron;
		}
		close(pipefds[0]);
		
	#ifdef HAVE_FCNTL
		fcntl(pipefds[1],F_SETFD,1);
	#endif
		if(workdir)
		{
			if(ILChangeDir(workdir) != IL_ERRNO_Success)
			{
				/* Send errno to parent process */
				write(pipefds[1],&errno, sizeof(errno));
				exit(1);
			}
		}

		execvp(fname, args);
		write(pipefds[1],&errno, sizeof(errno));
		exit(1);
	}
	else if(pid > 0)
	{
		/* We are in the parent process */
		if((flags & ProcessStart_RedirectStdin) != 0)
		{
			close(stdinFds[0]);
		}
		if((flags & ProcessStart_RedirectStdout) != 0)
		{
			close(stdoutFds[1]);
		}
		if((flags & ProcessStart_RedirectStderr) != 0)
		{
			close(stderrFds[1]);
		}
		*processID = (ILInt32)pid;
		close(pipefds[1]);
		errno = 0;
		read(pipefds[0],&errno,sizeof(errno));
		close(pipefds[0]);
		result = (errno == 0);
	}
	else
	{
		/* An error occurred during the fork */
		if((flags & ProcessStart_RedirectStdin) != 0)
		{
			close(stdinFds[0]);
			close(stdinFds[1]);
		}
		if((flags & ProcessStart_RedirectStdout) != 0)
		{
			close(stdoutFds[0]);
			close(stdoutFds[1]);
		}
		if((flags & ProcessStart_RedirectStderr) != 0)
		{
			close(stderrFds[0]);
			close(stderrFds[1]);
		}
		close(pipefds[0]);
		close(pipefds[1]);
	}

	/* Clean up and exit */
	FreeStringList(args, argc);
	FreeStringList(newEnviron, varNum);
	return result;

#else
	/* Don't know how to spawn processes on this platform */
	return 0;
#endif
}

/*
 * private static bool WaitForExit(IntPtr processHandle, int processID,
 *								   int milliseconds, out int exitCode);
 */
ILBool _IL_Process_WaitForExit(ILExecThread *_thread,
							   ILNativeInt processHandle,
							   ILInt32 processID, ILInt32 milliseconds,
							   ILInt32 *exitCode)
{
#ifdef IL_WIN32_PLATFORM

	DWORD result;
	result = WaitForSingleObject((HANDLE)processHandle, (DWORD)milliseconds);
	if(result == WAIT_OBJECT_0)
	{
		if(GetExitCodeProcess((HANDLE)processHandle, &result))
		{
			*exitCode = (ILInt32)result;
			return 1;
		}
		else
		{
			*exitCode = 1;
			return 1;
		}
	}
	else
	{
		*exitCode = 0;
		return 0;
	}

#elif defined(IL_USING_FORK)

	int status, result;
	status = 1;
	if(milliseconds < 0 || milliseconds == (ILInt32)0x7FFFFFFF)
	{
		/* Wait indefinitely for the process to exit */
		while((result = (int)waitpid((int)processID, &status, 0)) !=
				(int)processID)
		{
			if(result == -1)
			{
				if(errno != EINTR)
				{
					return 0;
				}
			}
		}
	}
	else if(milliseconds == 0)
	{
		/* Test and return immediately */
		result = (int)waitpid((int)processID, &status, WNOHANG);
		if(result != (int)processID)
		{
			return 0;
		}
	}
	else
	{
		/* Wait for a specified timeout period */
		do
		{
			result = (int)waitpid((int)processID, &status, WNOHANG);
			if(result == (int)processID)
			{
				break;
			}
			else if(result == -1)
			{
				if(errno != EINTR)
				{
					return 0;
				}
			}
			ILThreadSleep(100);
			milliseconds -= 100;
		}
		while(milliseconds > 0);
		if(milliseconds <= 0)
		{
			return 0;
		}
	}
	if(WIFEXITED(status))
	{
		*exitCode = (ILInt32)(WEXITSTATUS(status));
	}
	else
	{
		/* Exited because of a signal */
		*exitCode = 127;
	}
	return 1;

#else
	/* Don't know how to wait for processes on this platform */
	return 0;
#endif
}

/*
 * private static bool WaitForInputIdle(IntPtr processHandle,
 *										int processID, int milliseconds);
 */
ILBool _IL_Process_WaitForInputIdle(ILExecThread *_thread,
									ILNativeInt processHandle,
									ILInt32 processID,
									ILInt32 milliseconds)
{
#ifdef IL_WIN32_PLATFORM
	return WaitForInputIdle((HANDLE)processHandle, (DWORD)milliseconds);
#else
	/* "Idle" has no meaning on non-Win32 platforms so just pretend
	   that the process is fully initialized and ready to go */
	return 1;
#endif
}

/*
 * public static Errno GetErrno();
 */
ILInt32 _IL_Process_GetErrno(ILExecThread *thread)
{
	return ILSysIOGetErrno();
}

/*
 * public static String GetErrnoMessage(Errno error);
 */
ILString *_IL_Process_GetErrnoMessage(ILExecThread *thread, ILInt32 error)
{
	const char *msg = ILSysIOGetErrnoMessage(error);
	if(msg)
	{
		return ILStringCreate(thread, msg);
	}
	else
	{
		return 0;
	}
}
#ifdef	__cplusplus
};
#endif
