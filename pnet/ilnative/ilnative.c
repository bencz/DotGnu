/*
 * ilnative.c - Print information about native methods in IL binaries.
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

#include <stdio.h>
#include "il_system.h"
#include "il_image.h"
#include "il_dumpasm.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-n", 'n', 0, 0, 0},
	{"-v", 'v', 0, 0, 0},
	{"--names-only", 'n', 0,
		"--names-only or -n",
		"Print the names, but not the types, of the methods."},
	{"--version", 'v', 0,
		"--version    or -v",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

static void usage(const char *progname);
static void version(void);
static int printNatives(const char *filename, ILContext *context,
						int namesOnly);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	int namesOnly = 0;
	int sawStdin;
	int state, opt;
	char *param;
	int errors;
	ILContext *context;

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'n':
			{
				namesOnly = 1;
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

	/* Create a context to use for image loading */
	context = ILContextCreate();
	if(!context)
	{
		fprintf(stderr, "%s: out of memory\n", progname);
		return 1;
	}

	/* Load and print information about the input files */
	sawStdin = 0;
	errors = 0;
	while(argc > 1)
	{
		if(!strcmp(argv[1], "-"))
		{
			/* Dump the contents of stdin, but only once */
			if(!sawStdin)
			{
				errors |= printNatives("-", context, namesOnly);
				sawStdin = 1;
			}
		}
		else
		{
			/* Dump the contents of a regular file */
			errors |= printNatives(argv[1], context, namesOnly);
		}
		++argv;
		--argc;
	}

	/* Destroy the context */
	ILContextDestroy(context);
	
	/* Done */
	return errors;
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILNATIVE " VERSION " - IL Image Native Utility\n");
	fprintf(stdout, "Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("ILNATIVE " VERSION " - IL Image Native Utility\n");
	printf("Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILNATIVE comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Dump information about a native method.
 */
static void DumpMethodInfo(ILImage *image, ILMethod *method,
						   ILPInvoke *pinvoke)
{
	/* Dump the method attributes */
	ILDumpFlags(stdout, ILMethod_Attrs(method),
				ILMethodDefinitionFlags, 0);

	/* Dump the PInvoke information, if supplied */
	if(pinvoke)
	{
		fputs("pinvokeimpl(", stdout);
		ILDumpString(stdout, ILModule_Name(ILPInvoke_Module(pinvoke)));
		putc(' ', stdout);
		if(strcmp(ILPInvoke_Alias(pinvoke), ILMethod_Name(method)) != 0)
		{
			fputs("as ", stdout);
			ILDumpString(stdout, ILPInvoke_Alias(pinvoke));
			putc(' ', stdout);
		}
		ILDumpFlags(stdout, ILPInvoke_Attrs(pinvoke),
					ILPInvokeImplementationFlags, 0);
		fputs(") ", stdout);
	}

	/* Dump the method signature */
	ILDumpMethodType(stdout, image, ILMethod_Signature(method), 0,
					 ILMethod_Owner(method), ILMethod_Name(method),
					 method);
	putc(' ', stdout);

	/* Dump the implementation flags */
	ILDumpFlags(stdout, ILMethod_ImplAttrs(method),
				ILMethodImplementationFlags, 0);

	/* Terminate the line */
	putc('\n', stdout);
}

/*
 * Load an IL image an display the native methods.
 */
static int printNatives(const char *filename, ILContext *context,
						int namesOnly)
{
	ILImage *image;
	unsigned long numMethods;
	unsigned long token;
	ILMethod *method;

	/* Attempt to load the image into memory */
	if(ILImageLoadFromFile(filename, context, &image,
					  	   IL_LOADFLAG_FORCE_32BIT |
					  	   IL_LOADFLAG_NO_RESOLVE, 1) != 0)
	{
		return 1;
	}

	/* Walk the MethodDef table and print all methods that are
	   either listed as "PInvoke" or "internalcall".  If a method
	   does not have IL code associated with it, then we classify
	   it as "internalcall" */
	numMethods = ILImageNumTokens(image, IL_META_TOKEN_METHOD_DEF);
	for(token = 1; token <= numMethods; ++token)
	{
		method = (ILMethod *)ILImageTokenInfo
									(image, IL_META_TOKEN_METHOD_DEF | token);
		if(method)
		{
			if(ILMethod_HasPInvokeImpl(method))
			{
				if(namesOnly)
				{
					ILDumpIdentifier(stdout,
									 ILClass_Name(ILMethod_Owner(method)),
									 ILClass_Namespace(ILMethod_Owner(method)),
									 0);
					fputs("::", stdout);
					ILDumpIdentifier(stdout, ILMethod_Name(method), 0, 0);
					putc('\n', stdout);
				}
				else
				{
					DumpMethodInfo(image, method, ILPInvokeFind(method));
				}
			}
			else if(ILMethod_IsInternalCall(method) ||
			        ILMethod_IsNative(method))
			{
				if(namesOnly)
				{
					ILDumpIdentifier(stdout,
									 ILClass_Name(ILMethod_Owner(method)),
									 ILClass_Namespace(ILMethod_Owner(method)),
									 0);
					fputs("::", stdout);
					ILDumpIdentifier(stdout, ILMethod_Name(method), 0, 0);
					putc('\n', stdout);
				}
				else
				{
					DumpMethodInfo(image, method, 0);
				}
			}
		}
	}

	/* Clean up and exit */
	ILImageDestroy(image);
	return 0;
}

#ifdef	__cplusplus
};
#endif
