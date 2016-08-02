/*
 * ilverify.c - Bulk verification tool for IL programs.
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

#if !defined(__palmos__)

#include <stdio.h>
#include "engine.h"
#include "il_utils.h"
#include "il_dumpasm.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-u", 'u', 0, 0, 0},
	{"-v", 'v', 0, 0, 0},
	{"--unsafe", 'u', 0,
		"--unsafe  or -u",
		"Allow unsafe code."},
	{"--version", 'v', 0,
		"--version or -v",
		"Print the version of the program"},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

static void usage(const char *progname);
static void version(void);
static int verify(const char *filename, ILContext *context, int allowUnsafe);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	int allowUnsafe = 0;
	int sawStdin;
	int state, opt;
	char *param;
	int errors;
	ILExecProcess *process = 0;
	ILExecThread *thread = 0;

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'u':
			{
				allowUnsafe = 1;
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

	/* We need at least one input file argument */
	if(argc <= 1)
	{
		usage(progname);
		return 1;
	}

	/* Initialize the engine, to ensure that the garbage collector is OK */
	ILExecInit(0);

	/* Create the default appdomain for the image loading. */
	if(!(process = ILExecProcessCreateNull()))
	{
		fprintf(stderr, "%s: out of memory\n", progname);
		return 1;
	}

	/* Make sure the current thread is assiciated to the process */
	if(!(thread = ILExecProcessGetMain(process)))
	{
		fprintf(stderr, "%s: out of memory\n", progname);
		return 1;
	}

	/* Load and verify the input files */
	sawStdin = 0;
	errors = 0;
	while(argc > 1)
	{
		if(!strcmp(argv[1], "-"))
		{
			/* Verify the contents of stdin, but only once */
			if(!sawStdin)
			{
				errors |= verify("-", process->context, allowUnsafe);
				sawStdin = 1;
			}
		}
		else
		{
			/* Verify the contents of a regular file */
			errors |= verify(argv[1], process->context, allowUnsafe);
		}
		++argv;
		--argc;
	}

	/* Destroy the engine */
	ILExecDeinit();
	
	/* Done */
	return errors;
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILVERIFY " VERSION " - IL Image Verification Utility\n");
	fprintf(stdout, "Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{
	printf("ILVERIFY " VERSION " - IL Image Verification Utility\n");
	printf("Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILVERIFY comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Import the null coder from "null_coder.c".
 */
extern ILCoderClass const _ILNullCoderClass;
extern ILCoder _ILNullCoder;

/*
 * Print a verification error.
 */
static void printError(ILImage *image, ILMethod *method, const char *msg)
{
#ifndef IL_WITHOUT_TOOLS
	ILDumpMethodType(stdout, image,
					 ILMethod_Signature(method), 0,
					 ILMethod_Owner(method),
					 ILMethod_Name(method),
					 method);
#else
	fputs(ILClass_Name(ILMethod_Owner(method)), stdout);
	fputs(".", stdout);
	fputs(ILMethod_Name(method), stdout);
#endif
	fputs(" - ", stdout);
	fputs(msg, stdout);
	putc('\n', stdout);
}

/*
 * Load an IL image from an input stream and verify all of its methods.
 */
static int verify(const char *filename, ILContext *context, int allowUnsafe)
{
	ILImage *image;
	ILMethod *method;
	ILMethodCode code;
	int result;
	unsigned char *start;

	/* Attempt to load the image into memory */
	if(ILImageLoadFromFile(filename, context, &image,
						   IL_LOADFLAG_FORCE_32BIT, 1) != 0)
	{
		return 0;
	}

	/* Scan the entire MethodDef table and verify everything we find */
	method = 0;
	while((method = (ILMethod *)ILImageNextToken
				(image, IL_META_TOKEN_METHOD_DEF, (void *)method)) != 0)
	{
		/* Skip this method if it does not have IL bytecode */
		if(!ILMethod_RVA(method))
		{
			continue;
		}

		/* Get the IL bytecode for the method */
		if(!ILMethodGetCode(method, &code))
		{
			printError(image, method, "malformed code");
			continue;
		}

		/* Verify the method */
		result = _ILVerify(&_ILNullCoder, &start, method,
						   &code, allowUnsafe, ILExecThreadCurrent());
		if(!result)
		{
			printError(image, method, "could not verify code");
		}
	}

	/* Clean up and exit */
	ILImageDestroy(image);
	return 0;
}

#ifdef	__cplusplus
};
#endif

#else	/* __palmos__ */

int main(int argc, char *argv[])
{
	return 0;
}

#endif	/* __palmos__ */
