/*
 * register.c - Register the engine with the operating system.
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
This module is highly specific to the Linux kernel.  It can be used
to register the runtime engine with the kernel so that "ilrun" is
automatically launched when the user executes an IL executable.
i.e., the user can use:

	program.exe [args]

instead of:

	ilrun program.exe [args]

This also has the added advantage that the shell can perform path
searching for IL executables, so the user does not need to specify
the path to the program executable by hand.

To register "ilrun" with the kernel, do the following as root:

	ilrun --register

An explicit pathname can be supplied to override the default:

	ilrun --register /usr/local/bin/ilrun

To unregister "ilrun", do the following as root:

	ilrun --unregister

Some problems may occur if you are using both "ilrun" and Wine on
the same system.  Both can intercept the handling of DOS-style
executables.  Only one can be registered at any one time, and
the other will need to be launched by hand.
*/

#include "engine.h"
#include "il_utils.h"
#if HAVE_UNISTD_H
#include <unistd.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

#if defined(linux) || defined(__linux) || defined(__linux__)

#include <errno.h>

/*
 * The file to use to register entries.
 */
#define	BINFMT_REGISTER		"/proc/sys/fs/binfmt_misc/register"

/*
 * The file that is registered.
 */
#define	BINFMT_NAME			"/proc/sys/fs/binfmt_misc/DOSWin"

/*
 * Register the "ilrun" program to handle DOS-style executables.
 * Returns non-zero if registration was not possible.
 */
int _ILRegisterWithKernel(const char *progname)
{
	FILE *file;
	char *path;
	static char *modprobe_cmdline[] = {"/sbin/modprobe", "binfmt_misc", 0};

	/* Execute "modprobe" to make sure that "binfmt_misc" is loaded */
	if(geteuid() == 0 || getegid() == 0)
	{
		ILSpawnProcess(modprobe_cmdline);
	}

	/* Expand the program name to include the entire path */
	path = ILExpandFilename(progname, getenv("PATH"));
	if(!path)
	{
		fputs("virtual memory exhausted\n", stderr);
		return 1;
	}

	/* Open the registration file */
	if((file = fopen(BINFMT_REGISTER, "w")) == NULL)
	{
		if(errno == ENOENT)
		{
			/* This may be an earlier version of the Linux kernel
			   that does not support binary format registration */
			fprintf(stderr,
					"%s: the operating system does not support registration\n",
					progname);
		}
		else
		{
			/* Probably access denied */
			perror(BINFMT_REGISTER);
		}
		ILFree(path);
		return 1;
	}

	/* Write the registration details */
	fprintf(file, ":DOSWin:M::MZ::%s:\n", path);
	fclose(file);

	/* Done */
	ILFree(path);
	return 0;
}

/*
 * Unregister the "ilrun" program for DOS-style executables.
 * Returns non-zero if unregistration failed.
 */
int _ILUnregisterFromKernel(void)
{
	FILE *file;

	/* Open the file that corresponds to the registered name */
	if((file = fopen(BINFMT_NAME, "w")) == NULL)
	{
		/* If we can't open it, then it is probably already unregistered.
		   But it may be a permissions issue which we do want to report */
		if(errno != ENOENT)
		{
			perror(BINFMT_NAME);
		}
		return 1;
	}

	/* Write "-1" to the file to remove the entry */
	fprintf(file, "-1\n");
	fclose(file);

	/* Done */
	return 0;
}

#else /* !linux */

int _ILRegisterWithKernel(const char *progname)
{
#ifndef REDUCED_STDIO
	/* We normally won't get here because of the #ifdef's in "ilrun.c",
	   but if we do, then print an error message */
	fprintf(stderr, "%s: the operating system does not support registration\n",
			progname);
#endif
	return 1;
}

int _ILUnregisterFromKernel()
{
	/* If the OS cannot register, then assume that unregistration is OK */
	return 0;
}

#endif /* !linux */

#ifdef	__cplusplus
};
#endif
