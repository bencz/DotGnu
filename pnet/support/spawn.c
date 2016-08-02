/*
 * spawn.c - Spawn child processes.
 *
 * Copyright (C) 2001-2003  Southern Storm Software, Pty Ltd.
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

#include "il_system.h"
#include "il_utils.h"
#include <stdio.h>
#ifdef IL_WIN32_PLATFORM
	#include <windows.h>
	#include <process.h>
	#ifndef _WAIT_CHILD
		#define _WAIT_CHILD WAIT_CHILD
	#endif
#elif !defined(__palmos__)
	#ifdef HAVE_SYS_TYPES_H
		#include <sys/types.h>
	#endif
	#ifdef HAVE_UNISTD_H
		#include <unistd.h>
	#endif
	#ifdef HAVE_SYS_WAIT_H
		#include <sys/wait.h>
	#endif
	#ifdef HAVE_LIMITS_H
		#include <limits.h>
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
	#include <signal.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef IL_WIN32_PLATFORM

/*
 * Use Windows-specific functions to spawn child processes.
 */
int ILSpawnProcess(char *argv[])
{
	int arg;
	int len;
	char **newargv = 0;
	char *tempfile = 0;
	char *quotedprog = 0;
	char *program = argv[0];
	int status;
	char temppath[MAX_PATH];
	char tempname[MAX_PATH + 3];
	FILE *file;
	int useResponseFile;
	int responsePosn;

	/* Scan the command-line to see if it may be too big to
	   fit in the system's command-line buffer.  If so, we
	   must write the options to a temporary file instead */
	arg = 0;
	len = 0;
	useResponseFile = 0;
	while(argv[arg] != 0)
	{
		if(strchr(argv[arg], ' ') != 0)
		{
			/* We must quote this argument: always put it in the option file,
			   so that the options don't get messed up by Microsoft's version
			   of "spawn*", which doesn't quote arguments with spaces */
			useResponseFile = 1;
			break;
		}
		else if(argv[arg][0] == '\0' || argv[arg][0] == '@')
		{
			/* Quote arguments that are empty or start with '@' */
			useResponseFile = 1;
			break;
		}
		else
		{
			/* This argument doesn't need quoting, but may cause overflow */
			len += strlen(argv[arg]);
			if(len >= 64)
			{
				useResponseFile = 1;
				break;
			}
		}
		++len;
		++arg;
	}
	if(!arg && useResponseFile)
	{
		++arg;
	}

	/* Does the command-line need quoting? */
	if(useResponseFile)
	{
		/* Create the temporary option file */
		if(!GetTempPath(sizeof(temppath), temppath))
		{
			return -1;
		}
		if(!GetTempFileName(temppath, "CMD", 0, tempname + 2))
		{
			return -1;
		}
		strcpy(temppath, tempname + 2);
		tempname[0] = '@';
		tempname[1] = '"';
		strcat(tempname, "\"");
		tempfile = temppath;
		if((file = fopen(tempfile, "w")) == NULL)
		{
			return -1;
		}
		responsePosn = arg;
		while(argv[arg] != 0)
		{
			if(argv[arg][0] == '\0' || argv[arg][0] == '@' ||
			   strchr(argv[arg], ' ') != 0)
			{
				putc('"', file);
				fputs(argv[arg], file);
				putc('"', file);
			}
			else
			{
				fputs(argv[arg], file);
			}
			putc('\n', file);
			++arg;
		}
		fclose(file);

		/* Set up a new argument array */
		newargv = (char **)ILMalloc((responsePosn + 2) * sizeof(char *));
		if(!newargv)
		{
			DeleteFile(tempfile);
			return -1;
		}
		quotedprog = (char *)ILMalloc(strlen(argv[0]) + 3);
		if(!quotedprog)
		{
			ILFree(newargv);
			DeleteFile(tempfile);
			return -1;
		}
		quotedprog[0] = '"';
		strcpy(quotedprog + 1, argv[0]);
		strcat(quotedprog, "\"");
		newargv[0] = quotedprog;
		for(arg = 1; arg < responsePosn; ++arg)
		{
			newargv[arg] = argv[arg];
		}
		newargv[responsePosn] = tempname;
		newargv[responsePosn + 1] = 0;
		argv = newargv;
	}

	/* Spawn the child process and wait for it to exit */
	status = spawnvp(_P_WAIT, program, (char * const *)argv);

	/* Delete the temporary option file */
	if(tempfile)
	{
		DeleteFile(tempfile);
	}

	/* Clean up the copied argument array */
	if(newargv)
	{
		ILFree(newargv);
	}
	if(quotedprog)
	{
		ILFree(quotedprog);
	}

	/* Done */
	return status;
}

int ILSpawnProcessWithPipe(char *argv[], void **stream)
{
	/* Not supported */
	return 0;
}

int ILSpawnProcessWaitForExit(int pid, char *argv[])
{
	int status = 1;
	if (_cwait(&status, pid, _WAIT_CHILD) == -1)
	{
		return 0;
	}
	
	return (status == 0);
}

#else
#if defined(HAVE_FORK) && defined(HAVE_EXECV) && (defined(HAVE_WAITPID) || defined(HAVE_WAIT))

/*
 * Determine if a signal is important enough to be reported.
 * Simple exits like SIGINT, SIGTERM, etc are not considered
 * important as they are "normal" ways to abort a process.
 */
static int ImportantSignal(unsigned sig)
{
#ifdef SIGSEGV
	if(sig == SIGSEGV) return 1;
#endif
#ifdef SIGILL
	if(sig == SIGILL) return 1;
#endif
#ifdef SIGBUS
	if(sig == SIGBUS) return 1;
#endif
#ifdef SIGABRT
	if(sig == SIGABRT) return 1;
#endif
#ifdef SIGFPE
	if(sig == SIGFPE) return 1;
#endif
#ifdef SIGSTKFLT
	if(sig == SIGSTKFLT) return 1;
#endif
	return 0;
}

/*
 * Use a @file shortcut if you run into arguments
 * that overflow the system limits. This function
 * modifies only template and does not modify argv.
 * Ensure that template is writable.
 */
#if defined(HAVE_LIMITS_H) && defined(HAVE_MKSTEMP)
static int CreateResponseFile(char * argv[], char * template)
{
	int arg;
	int len;
	int useResponseFile;
	int fd = 0;
	FILE *fp = NULL;
	
	/* Scan the command-line to see if it may be too big to
	   fit in the system's command-line buffer.  If so, we
	   must write the options to a temporary file instead */
	arg = 0;
	len = 0;
	useResponseFile = 0;
	while(argv[arg] != 0)
	{
		len += strlen(argv[arg]);
		if(len >= 8192) /* TODO: obtain from configure.in */
		{
			useResponseFile = 1;
			break;
		}
		++len;
		++arg;
	} 
	#if defined(_POSIX_ARG_MAX)
	if(arg >= _POSIX_ARG_MAX)
	{
		useResponseFile = 1;
	}
	#endif

	if(useResponseFile)
	{
		if((fd = mkstemp(template)) == -1)
		{
			perror("mkstemp");
			return 0;
		}
		
		fp = fdopen(fd, "w");

		if(fp == NULL)
		{
			perror("fdopen");
			return 0;
		}

		arg = 1;
		while(argv[arg] != 0)
		{
			fputs(argv[arg], fp);
			putc('\n', fp);
			++arg;
		}
	}
	else
	{
		return 0;
	}

	fclose(fp);
	close(fd);

	return 1;
}
#else
static int CreateResponseFile(char * argv[], char * template)
{
	return 0;
}
#endif

/*
 * Use Unix-specific functions to spawn child processes.
 */
int ILSpawnProcess(char *argv[])
{
	int pid = fork();
	char template[] = "@/tmp/cscc-args-XXXXXX";
	char *newargv[] = {argv[0], template, 0};
	int responseFile = CreateResponseFile(argv, template+1);
	int result = 0;

	if(pid < 0)
	{
		/* Could not fork the child process */
		perror("fork");
		return -1;
	}
	else if(pid == 0)
	{
		/* We are in the child process */
		if(responseFile)
		{
			execvp(argv[0], newargv);
		}
		else
		{
			execvp(argv[0], argv);
		}
		perror(argv[0]);
		exit(1);
		return -1;		/* Keep the compiler happy */
	}
	else
	{
		/* We are in the parent process */
		result = ILSpawnProcessWaitForExit(pid, argv);
		if(responseFile)
		{
			ILDeleteFile(template+1);
		}
		return result;
	}
}

int ILSpawnProcessWithPipe(char *argv[], void **stream)
{
	int pipefds[2];
	int pid;

	/* Initialize the return stream */
	*stream = 0;

	/* Create a pipe to capture the child's stdout */
	if(pipe(pipefds) < 0)
	{
		perror("pipe");
		return -1;
	}

	/* Launch the child process */
	pid = fork();
	if(pid < 0)
	{
		/* Could not fork the child process */
		perror("fork");
		return -1;
	}
	else if(pid == 0)
	{
		/* We are in the child process */
		dup2(pipefds[1], 1);
		close(pipefds[0]);
		close(pipefds[1]);
		execvp(argv[0], argv);
		perror(argv[0]);
		exit(1);
		return -1;		/* Keep the compiler happy */
	}
	else
	{
		/* We are in the parent process */
		close(pipefds[1]);
		*stream = fdopen(pipefds[0], "r");
		if(!(*stream))
		{
			perror("fdopen");
			close(pipefds[0]);
			return -1;
		}
		return pid;
	}
}

int ILSpawnProcessWaitForExit(int pid, char *argv[])
{
	int status = 1;
#ifdef HAVE_WAITPID
	waitpid(pid, &status, 0);
#else
	int result;
	while((result = wait(&status)) != pid && result != -1)
	{
		/* Some other child fell: not the one we are interested in */
	}
#endif
	if(WIFEXITED(status))
	{
		/* Return the child's exit status as the final status */
		return WEXITSTATUS(status);
	}
	else if(WIFSIGNALLED(status) &&
	        (ImportantSignal(WTERMSIG(status)) || WCOREDUMP(status)))
	{
		/* Some kind of signal occurred */
		fprintf(stderr, "%s: exited with signal %d%s\n",
				argv[0], (int)(WTERMSIG(status)),
				(WCOREDUMP(status) ? " (core dumped)" : ""));
		return -1;
	}
	else
	{
		/* Some other kind of signal or error */
		return -1;
	}
}

#else

/*
 * We don't know how to spawn child processes on this system.
 */

int ILSpawnProcess(char *argv[])
{
#ifndef REDUCED_STDIO
	fputs("Don't know how to spawn child processes on this system\n", stderr);
#endif
	return -1;
}

int ILSpawnProcessWithPipe(char *argv[], void **stream)
{
	/* Not supported */
	return 0;
}

int ILSpawnProcessWaitForExit(int pid, char *argv[])
{
	/* Not supported */
	return -1;
}

#endif
#endif

#ifdef	__cplusplus
};
#endif
