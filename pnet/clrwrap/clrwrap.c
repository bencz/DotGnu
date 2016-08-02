/*
 * clrwrap.c - Wrapper for launching the user's configured CLR.
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

/*

This program is used to make an IL program Unix-friendly by placing
a small wrapper script in the same directory as the IL ".exe" file.
It also makes the choice of CLR dynamic, so that the same installed
application can be invoked by multiple CLR implementations.  The
wrapper script contains something like:

	#!/usr/bin/env clrwrap
	program.exe

where "program.exe" is the name of the ".exe" file, relative to the location
of the script.  Remaining lines in the wrapper script are ignored, and
are reserved for future use.  Comment lines may appear in the wrapper
script, beginning with '#'.  Empty lines are also comment lines.  If the
wrapper script does not specify a ".exe" file, then the IL program is
assumed to be "script.exe" where "script" is the name of the script.

When the "clrwrap" program runs, it searches for an appropriate CLR as
follows:

	1. Look for the "--clr=NAME" option on the command-line.  This option
	   is removed from the command-line and "NAME" is invoked as the CLR.
	2. Look for the "CLR_PROGRAM" environment variable.
	3. Use /etc/alternatives/clr if it is present (for Debian compatibility).
	4. Use the compiled-in default (CLR_DEFAULT_PROGRAM macro).

The "clrwrap" program can also be used to launch ordinary IL binaries.
For example:

	clrwrap program.exe

The same sequence as above is used to detect the user's preferred CLR,
and no wrapper script is required.  The "clrwrap" program automatically
detects whether it is supplied a wrapper script or a real IL executable.

As a special case, specifying "--clr=ms" on Windows will invoke the
IL application using Microsoft's native CLR.

*/

#include "il_system.h"
#include "il_utils.h"
#include <stdio.h>
#include <stdlib.h>
#ifndef _WIN32
	#ifdef HAVE_SYS_TYPES_H
		#include <sys/types.h>
	#endif
	#ifdef HAVE_UNISTD_H
		#include <unistd.h>
	#endif
	#ifdef HAVE_SYS_WAIT_H
		#include <sys/wait.h>
	#endif
#endif

/*
 * Some definitions that should make it easier to port this
 * program to other build environments.
 */
#define	CLR_MALLOC(size)			(ILMalloc((size)))
#define	CLR_STRDUP(str)				(ILDupString((str)))
#define	CLR_EXPAND(name,path)		(ILExpandFilename((name), (path)))
#define	CLR_SPAWN(argv)				(ILSpawnProcess((argv)))
#define	CLR_EXISTS(name,retname)	(ILFileExists((name), (retname)))
#define	CLR_MEMMOVE(dst,src,n)		(ILMemMove((dst), (src), (n)))

/*
 * The default program to use to run IL-format binaries.
 */
#ifndef	CLR_DEFAULT_PROGRAM
#define	CLR_DEFAULT_PROGRAM			"ilrun"
#endif

/*
 * The location to look for a Debian "alternatives" symlink.
 */
#ifndef	CLR_ALTERNATIVE
#define	CLR_ALTERNATIVE				"/etc/alternatives/clr"
#endif

/*
 * Forward declarations.
 */
static void OutOfMemory(char *progname);
static char *FindCLR(void);
static char *RemapName(char *name, char *progname);
static char *GetBaseName(char *name);
static char *FindExecutableInGac(char *name);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	char **newargv;
	int newargc;
	char *clrOption = 0;
	int retval;
	char *basename;
	int needScript;

	/* Allocate a new command-line buffer */
	if((newargv = (char **)CLR_MALLOC(sizeof(char *) * (argc + 2))) == 0)
	{
		OutOfMemory(progname);
	}

	/* Determine if we have been supplied a redirect script, or if
	   "clrwrap" has been symlinked, causing it to implicitly be
	   the name of the executable to redirect to */
	basename = GetBaseName(progname);
	needScript = (!ILStrICmp(basename, "clrwrap"));

	/* Copy the arguments to the new buffer, and look for "--clr" */
	newargc = 1;
	if(!needScript)
	{
		newargv[newargc] = FindExecutableInGac(basename);
		if(!(newargv[newargc]))
		{
			fprintf(stderr, "%s: could not find a corresponding IL binary\n",
					progname);
			return 1;
		}
		++newargc;
	}
	while(argc > 1)
	{
		if(!strncmp(argv[1], "--clr=", 6))
		{
			/* The user supplied the "--clr=NAME" form of the option */
			if(argv[1][6] == '\0')
			{
				fprintf(stderr, "%s: missing argument for `--clr' option\n",
						progname);
				return 1;
			}
			clrOption = argv[1] + 6;
		}
		else if(!strcmp(argv[1], "--clr"))
		{
			/* The user supplied the "--clr NAME" form of the option */
			if(argc <= 2)
			{
				fprintf(stderr, "%s: missing argument for `--clr' option\n",
						progname);
				return 1;
			}
			clrOption = argv[2];
			++argv;
			--argc;
		}
		else
		{
			newargv[newargc++] = argv[1];
		}
		++argv;
		--argc;
	}
	newargv[newargc] = 0;

	/* We need at least one argument in the new buffer */
	if(newargc < 2)
	{
		fprintf(stderr, "Usage: %s program [--clr=NAME] [program-args]\n",
				progname);
		return 1;
	}

	/* Locate the CLR to use */
	if(!clrOption)
	{
		clrOption = FindCLR();
	}
	newargv[0] = CLR_EXPAND(clrOption, getenv("PATH"));
	if(!(newargv[0]))
	{
		OutOfMemory(progname);
	}

	/* Remap the name if we were supplied a redirect script */
	if(needScript)
	{
		newargv[1] = RemapName(newargv[1], progname);
		if(!(newargv[1]))
		{
			return 1;
		}
	}

	/* Execute the CLR in the most efficient manner possible */
#if !defined(_WIN32) && defined(EXECV)
	execv(newargv[0], newargv);
	perror(newargv[0]);
	return 1;
#else
#if defined(_WIN32)
	if(!strcmp(clrOption, "ms"))
	{
		/* Use Microsoft's CLR.  We assume that we can activate
		   it by merely executing the IL program directly */
		++newargv;
	}
#endif
	retval = CLR_SPAWN(newargv);
	if(retval >= 0)
	{
		return retval;
	}
	else
	{
		return 1;
	}
#endif
}

/*
 * Report that the program is out of memory.
 */
static void OutOfMemory(char *progname)
{
	fputs(progname, stderr);
	fputs(": virtual memory exhausted\n", stderr);
	exit(1);
}

/*
 * Find the CLR to use if one wasn't specified on the command-line.
 */
static char *FindCLR(void)
{
	char *clr;
	char *newExePath = 0;

	if((clr = getenv("CLR_PROGRAM")) == 0)
	{
		if(CLR_EXISTS(CLR_ALTERNATIVE, &newExePath))
		{
			if(newExePath)
			{
				clr = newExePath;
			}
			else
			{
				clr = CLR_ALTERNATIVE;
			}
		}
		else
		{
			clr = CLR_DEFAULT_PROGRAM;
		}
	}

	return clr;
}

/*
 * Format types.
 */
#define	CLR_FORMAT_UNKNOWN		0
#define	CLR_FORMAT_MSDOS		1
#define	CLR_FORMAT_JVM			2
#define	CLR_FORMAT_SCRIPT		3

/*
 * Determine if a character is a space.  This is safe against
 * 8-bit cleanliness bugs in some versions of "isspace".
 */
#define	IsSpace(ch)		((ch) == ' ' || (ch) == '\t' || \
						 (ch) == '\f' || (ch) == '\v')

/*
 * Strip white space and comments from a line.
 */
static void StripLine(char *buf, int len)
{
	int posn = 0;

	/* Skip white space at the start of the line */
	while(posn < len && IsSpace(buf[posn]))
	{
		++posn;
	}
	if(buf[posn] == '#')
	{
		/* This is a comment line */
		buf[0] = '\0';
		return;
	}

	/* Truncate white space from the end of the line */
	while(len > posn && IsSpace(buf[len - 1]))
	{
		--len;
	}

	/* Copy the line into its final position in the buffer */
	if(posn > 0 && posn < len)
	{
		CLR_MEMMOVE(buf, buf + posn, len - posn);
	}
	buf[len - posn] = '\0';
}

/*
 * Read a line from a text file and strip white space and
 * comments from it.  Returns 0 at EOF.
 */
static int ReadLine(FILE *file, char *buf, int posn, int size)
{
	int ch;
	while((ch = getc(file)) != EOF)
	{
		if(ch == '\n')
		{
			/* Unix end of line sequence */
			StripLine(buf, posn);
			return 1;
		}
		else if(ch == '\r')
		{
			ch = getc(file);
			if(ch != '\n' && ch != EOF)
			{
				/* Mac end of line sequence */
				ungetc(ch, file);
			}
			StripLine(buf, posn);
			return 1;
		}
		else if(ch != 0x1A)		/* MS-DOS EOF marker */
		{
			if((posn + 1) < size)
			{
				buf[posn++] = (char)ch;
			}
		}
	}
	StripLine(buf, posn);
	return (posn > 0);
}

/*
 * Remap an IL program name if "name" is a redirect script and
 * not an IL binary.  Returns NULL if "name" is unsuitable for
 * use with a CLR.
 */
static char *RemapName(char *name, char *progname)
{
	FILE *file;
	char buffer[BUFSIZ];
	int format;
	int posn;
	char *newname;

	/* Open the file */
	if((file = fopen(name, "rb")) == NULL)
	{
		/* Retry, in case libc does not understand "rb" */
		if((file = fopen(name, "r")) == NULL)
		{
			perror(name);
			return 0;
		}
	}

	/* Read the magic number bytes and check them */
	if(fread(buffer, 1, 2, file) != 2)
	{
		format = CLR_FORMAT_UNKNOWN;
	}
	else if(buffer[0] == 'M' && buffer[1] == 'Z')
	{
		/* This is probably an IL-format binary: let the CLR
		   determine if it is really something else */
		format = CLR_FORMAT_MSDOS;
	}
	else if(buffer[0] == (char)0xCA && buffer[1] == (char)0xFE)
	{
		/* "ilrun" understands how to load JVM binaries */
		format = CLR_FORMAT_JVM;
	}
	else if(buffer[0] == '#' && buffer[1] == '!')
	{
		/* This is a redirect script */
		format = CLR_FORMAT_SCRIPT;
	}
	else
	{
		format = CLR_FORMAT_UNKNOWN;
	}

	/* Determine what to do based on the format */
	switch(format)
	{
		case CLR_FORMAT_UNKNOWN:
		{
			fclose(file);
			fprintf(stderr, "%s: unknown file format\n", name);
			return 0;
		}
		/* Not reached */

		case CLR_FORMAT_MSDOS:
		case CLR_FORMAT_JVM:
		{
			fclose(file);
			return name;
		}
		/* Not reached */

		default: break;
	}

	/* Find the first non-comment line in the script */
	posn = 2;
	while(ReadLine(file, buffer, posn, sizeof(buffer)))
	{
		if(buffer[0] != '\0')
		{
			/* Combine the name of the script with the remap name */
			fclose(file);
			if(buffer[0] == '/')
			{
				/* The remap name is absolute */
				newname = CLR_STRDUP(buffer);
				if(!newname)
				{
					OutOfMemory(progname);
				}
			}
			else
			{
				/* The remap name is relative */
				newname = (char *)CLR_MALLOC(strlen(name) + strlen(buffer) + 1);
				if(!newname)
				{
					OutOfMemory(progname);
				}
				strcpy(newname, name);
				posn = strlen(newname);
				while(posn > 0 && newname[posn - 1] != '/')
				{
					--posn;
				}
				strcpy(newname + posn, buffer);
			}
			return newname;
		}
		posn = 0;
	}
	fclose(file);

	/* We didn't find a remap name, so assume that the name
	   of the script, with ".exe" appended, is the IL binary */
	newname = (char *)CLR_MALLOC(strlen(name) + 5);
	if(!newname)
	{
		OutOfMemory(progname);
	}
	strcpy(newname, name);
	strcat(newname, ".exe");
	return newname;
}

/*
 * Get the base name from an executable name.
 */
static char *GetBaseName(char *name)
{
	int len = strlen(name);
	while(len > 0 && name[len - 1] != '/' && name[len - 1] != '\\')
	{
		--len;
	}
	name += len;
	len = strlen(name);
	if(len > 4 && !ILStrICmp(name + len - 4, ".exe"))
	{
		return ILDupNString(name, len - 4);
	}
	else
	{
		return name;
	}
}

/*
 * Find the name of an IL binary in the global assembly cache.
 */
static char *FindExecutableInGac(char *name)
{
	char *cachePath;
	char *searchName;

	/* Get the path to look in for the global assembly cache */
	cachePath = getenv("CSCC_LIB_PATH");
	if(!cachePath || *cachePath == '\0')
	{
		cachePath = ILGetStandardLibraryPath("cscc/lib");
		if(!cachePath)
		{
			return 0;
		}
	}

	/* Construct the name to search for */
	searchName = (char *)ILMalloc(strlen(name) + 5);
	if(!searchName)
	{
		return 0;
	}
	strcpy(searchName, name);
	strcat(searchName, ".exe");

	/* Search the path for the name and return it */
	return ILSearchPath(cachePath, searchName, 0);
}
