/*
 * csant.c - Build tool for C# program compilation.
 *
 * Copyright (C) 2001, 2002, 2003  Southern Storm Software, Pty Ltd.
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

This program is similar in behaviour to "NAnt" (nant.sourceforge.net),
except that it is written in C instead of C#.  This makes it a little
less flexible in some ways.

A core tenet of the Portable.NET design philosophy is that it must
be self-bootstrapping.  That is, the build must not rely upon any
"magic binaries" that must be built with other tools prior to
building Portable.NET.  The only "magic" that we permit is the
C compiler.

"NAnt" is not self-bootstrapping.  It must be built against the C#
system library, but that library itself is built using something
like "NAnt".  This creates a "magic binary" dependency.

Hence, we have provided this C version to bootstrap the compilation
of the C# system library.  Application programmers can then build
and use "NAnt" to achieve as much flexibility as they desire.

*/

#include "csant_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-f", 'f', 1, 0, 0},
	{"--file", 'f', 1,
		"--file name            or -f name",
		"Specify the name of the build file to use."},
	{"-b", 'b', 1, 0, 0},
	{"--base-src-dir", 'b', 1,
		"--base-src-dir name    or -b name",
		"Specify the base source directory for the build tree."},
	{"-B", 'B', 1, 0, 0},
	{"--base-build-dir", 'B', 1,
		"--base-build-dir name  or -B name",
		"Specify the base build directory for the build tree."},
	{"-D", 'D', 1, 0, 0},
	{"--define", 'D', 1,
		"-Dname=value           or --define name=value",
		"Define the property `name' and set it to `value'."},
	{"-p", 'p', 1, 0, 0},
	{"--profile", 'p', 1,
		"--profile name         or -p name",
		"Specify the definition profile to use."},
	{"-n", 'n', 0, 0, 0},
	{"--just-print", 'n', 0,
		"--just-print           or -n",
		"Print the names of the commands, but do not execute them."},
	{"-d", 'd', 0, 0, 0},
	{"--dummy-doc", 'd', 0,
		"--dummy-doc            or -d",
		"Output dummy documentation files."},
	{"-k", 'k', 0, 0, 0},
	{"--keep-going", 'k', 0,
		"--keep-going           or -k",
		"Keep processing even after an error."},
	{"-s", 's', 0, 0, 0},
	{"--silent", 's', 0,
		"--silent               or -s",
		"Do not print the names of commands as they are executed."},
	{"-c", 'c', 0, 0, 0},
	{"--csc-redirect", 'c', 0,
		"--csc-redirect         or -c",
		"Treat <csc> tags as <compile> tags (for NAnt compatibility)."},
	{"-m", 'm', 0, 0, 0},
	{"--mono-corlib", 'm', 0,
		"--mono-corlib          or -m",
		"Use Mono's corlib instead of mscorlib during C# compiles."},
	{"-i", 'i', 0, 0, 0},
	{"--install", 'i', 0,
		"--install              or -i",
		"Install assemblies with `ilgac' instead of compiling."},
	{"-u", 'u', 0, 0, 0},
	{"--uninstall", 'u', 0,
		"--uninstall            or -u",
		"Uninstall assemblies with `ilgac' instead of compiling."},
	{"--quiet", 's', 0, 0, 0},
	{"-C", 'C', 1, 0, 0},
	{"--compiler", 'C', 1,
		"--compiler name        or -C name",
		"Specify which compiler to use [`cscc' (default), `csc', or `msc']."},
	{"-a", 'a', 1, 0, 0},
	{"--assembly-cache", 'a', 1,
		"--assembly-cache dir   or -a dir",
		"Specify the location of the assembly cache directory."},
	{"-v", 'v', 0, 0, 0},
	{"--version", 'v', 0,
		"--version              or -v",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

static char *progname;

static void usage(const char *progname);
static void version(void);
static char *defaultBuildFile(const char *baseDir);
static int xmlRead(void *data, void *buffer, int len);

int main(int argc, char *argv[])
{
	int state, opt;
	char *buildFilename = NULL;
	char *profileFilename = NULL;
	char *param;
	char *temp;
	FILE *infile;
	int errors;
	ILXMLReader *reader;

	/* Parse the command-line arguments */
	progname = argv[0];
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'a':
			{
				CSAntCacheDir = param;
			}
			break;

			case 'b':
			{
				CSAntBaseSrcDir = param;
			}
			break;

			case 'B':
			{
				CSAntBaseBuildDir = param;
			}
			break;

			case 'f':
			{
				buildFilename = param;
			}
			break;

			case 'D':
			{
				temp = strchr(param, '=');
				if(temp)
				{
					CSAntDefineProperty(param, (int)(temp - param),
										temp + 1, 1);
				}
				else
				{
					CSAntDefineProperty(param, strlen(param), "", 1);
				}
			}
			break;

			case 'p':
			{
				profileFilename = param;
			}
			break;

			case 'n':
			{
				CSAntJustPrint = 1;
			}
			break;

			case 'd':
			{
				CSAntDummyDoc = 1;
			}
			break;

			case 'k':
			{
				CSAntKeepGoing = 1;
			}
			break;

			case 's':
			{
				CSAntSilent = 1;
			}
			break;

			case 'c':
			{
				CSAntRedirectCsc = 1;
			}
			break;

			case 'm':
			{
				CSAntForceCorLib = 1;
			}
			break;

			case 'i':
			{
				CSAntInstallMode = 1;
			}
			break;

			case 'u':
			{
				CSAntUninstallMode = 1;
			}
			break;

			case 'C':
			{
				CSAntCompiler = param;
			}
			break;

			case 'v':
			{
				version();
				return 0;
			}
			/* Not reached */

			default:
			{
				usage(progname);
				return 1;
			}
			/* Not reached */
		}
	}

	/* Process the targets and make-style defines */
	while(argc > 1)
	{
		temp = strchr(argv[1], '=');
		if(temp)
		{
			/* This is a property definition */
			CSAntDefineProperty(argv[1], (int)(temp - argv[1]), temp + 1, 1);
		}
		else
		{
			/* This is a target name */
			CSAntAddBuildTarget(argv[1]);
		}
		++argv;
		--argc;
	}

	/* Get the default build file if none was specified */
	if(!buildFilename)
	{
		buildFilename = defaultBuildFile(CSAntBaseSrcDir);
		if(!buildFilename)
		{
			fprintf(stderr, "%s: could not locate a default build file\n",
					progname);
			return 1;
		}
	}

	/* Get the default compiler from the environment if necessary */
	if(!CSAntCompiler)
	{
		CSAntCompiler = getenv("CSANT_COMPILER");
		if(!CSAntCompiler)
		{
			CSAntCompiler = "cscc";
		}
	}

	/* Process the build file to get all of the build rules */
	errors = 0;
	if((infile = fopen(buildFilename, "r")) == NULL)
	{
		perror(argv[1]);
		errors = 1;
	}
	else
	{
		if((reader = ILXMLCreate(xmlRead, infile, 0)) == 0)
		{
			CSAntOutOfMemory();
		}
		if(!CSAntParseFile(reader, buildFilename))
		{
			errors = 1;
		}
		ILXMLDestroy(reader);
		fclose(infile);
	}

	/* Process the profile file to get additional build rules */
	if(errors == 0 && profileFilename != 0)
	{
		if((infile = fopen(profileFilename, "r")) == NULL)
		{
			perror(argv[1]);
			errors = 1;
		}
		else
		{
			if((reader = ILXMLCreate(xmlRead, infile, 0)) == 0)
			{
				CSAntOutOfMemory();
			}
			if(!CSAntParseProfileFile(reader, profileFilename))
			{
				errors = 1;
			}
			ILXMLDestroy(reader);
			fclose(infile);
		}
	}

	/* Bail out if there were errors parsing the build file */
	if(errors)
	{
		return 1;
	}

	/* Execute the build rules */
	if(!CSAntBuild(buildFilename))
	{
		return 1;
	}

	/* Done */
	return 0;
}

static void usage(const char *progname)
{
	fprintf(stdout, "CSANT " VERSION " - C# compilation build tool\n");
	fprintf(stdout, "Copyright (c) 2001, 2002, 2003 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] [target ...]\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("CSANT " VERSION " - C# compilation build tool\n");
	printf("Copyright (c) 2001, 2002, 2003 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("CSANT comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Locate the name of the default build file.
 */
static char *defaultBuildFile(const char *baseDir)
{
	CSAntDir *dir;
	const char *name;
	char *combined;
	if(!baseDir)
	{
		baseDir = ".";
	}
	dir = CSAntDirOpen(baseDir, "*.csant");
	if(!dir)
	{
		return 0;
	}
	name = CSAntDirNext(dir);
	if(name)
	{
		combined = CSAntDirCombine(baseDir, name);
		CSAntDirClose(dir);
		return combined;
	}
	CSAntDirClose(dir);
	dir = CSAntDirOpen(baseDir, "*.build");
	if(!dir)
	{
		return 0;
	}
	name = CSAntDirNext(dir);
	if(name)
	{
		combined = CSAntDirCombine(baseDir, name);
	}
	else
	{
		combined = 0;
	}
	CSAntDirClose(dir);
	return combined;
}

/*
 * Read a block of bytes from a file-based XML input stream.
 */
static int xmlRead(void *data, void *buffer, int len)
{
	if(!feof((FILE *)data))
	{
		return fread(buffer, 1, len, (FILE *)data);
	}
	else
	{
		return 0;
	}
}

void CSAntOutOfMemory(void)
{
	fputs(progname, stderr);
	fputs(": virtual memory exhausted\n", stderr);
	exit(1);
}

char *CSAntGetProgramName(void)
{
	return progname;
}

#ifdef	__cplusplus
};
#endif
