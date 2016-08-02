/*
 * ildd.c - Print information about library dependencies.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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
#include <stdlib.h>
#include "il_system.h"
#include "il_image.h"
#include "il_program.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-f", 'f', 0, 0, 0},
	{"--file-names", 'f', 0,
		"--file-names or -f",
		"Print the name of each file that is processed."},
	{"-p", 'p', 0, 0, 0},
	{"--pinvoke", 'p', 0,
		"--pinvoke    or -p",
		"Print detailed information on PInvoke declarations."},
	{"-r", 'r', 0, 0, 0},
	{"--recursive", 'r', 0,
		"--recursive  or -r",
		"Recursively print transitive dependencies."},
	{"-v", 'v', 0, 0, 0},
	{"--version", 'v', 0,
		"--version    or -v",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

/*
 * Forward declarations.
 */
static void usage(const char *progname);
static void version(void);
static void addModule(const char *filename);
static int printDependencies(const char *filename, ILContext *context,
							 int multiple, int pinvoke, int recursive);

/*
 * List of all modules to be scanned.
 */
static char **modules = 0;
static int numModules = 0;
static int maxModules = 0;

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	int filenames = 0;
	int pinvoke = 0;
	int recursive = 0;
	int sawStdin;
	int state, opt;
	char *param;
	int errors;
	int multiple;
	int posn;
	ILContext *context;

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'f':
			{
				filenames = 1;
			}
			break;

			case 'p':
			{
				pinvoke = 1;
			}
			break;

			case 'r':
			{
				recursive = 1;
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

	/* Add the primary modules to be scanned to the list */
	sawStdin = 0;
	multiple = ((argc > 2) || filenames);
	while(argc > 1)
	{
		addModule(argv[1]);
		++argv;
		--argc;
	}

	/* Print the dependencies for all modules */
	errors = 0;
	posn = 0;
	while(posn < numModules)
	{
		errors |= printDependencies
			(modules[posn], context, multiple, pinvoke, recursive);
		++posn;
	}

	/* Destroy the context */
	ILContextDestroy(context);
	
	/* Done */
	return errors;
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILDD " VERSION " - IL Library Dependencies Utility\n");
	fprintf(stdout, "Copyright (c) 2002, 2003 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	fprintf(stdout, "ILDD " VERSION " - IL Library Dependencies Utility\n");
	fprintf(stdout, "Copyright (c) 2002, 2003 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILDD comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Add a module to the global list to be scanned, as long as there
 * isn't already a module with the specified name on the list.
 */
static void addModule(const char *filename)
{
	char **newModules;
	int posn;
	for(posn = 0; posn < numModules; ++posn)
	{
		if(!strcmp(modules[posn], filename))
		{
			return;
		}
	}
	if(numModules >= maxModules)
	{
		newModules = (char **)ILRealloc
			(modules, sizeof(char *) * (numModules + 32));
		if(!newModules)
		{
			exit(1);
		}
		modules = newModules;
		maxModules += 32;
	}
	modules[numModules] = ILDupString(filename);
	if(!(modules[numModules]))
	{
		exit(1);
	}
	++numModules;
}

/*
 * Load an IL image from an input stream and print its dependency information.
 */
static int printDependencies(const char *filename, ILContext *context,
							 int multiple, int pinvoke, int recursive)
{
	ILImage *image;
	ILAssembly *assem;
	ILModule *module;
	const char *name;
	const ILUInt16 *version;
	char *path;
	ILPInvoke *pinv;
	ILMethod *method;
	ILFileDecl *file;
	ILManifestRes *res;
	static ILUInt16 const zeroVersion[4] = {0, 0, 0, 0};

	/* Attempt to load the image into memory */
	if(ILImageLoadFromFile(filename, context, &image,
					       IL_LOADFLAG_FORCE_32BIT |
						   IL_LOADFLAG_NO_RESOLVE, 1) != 0)
	{
		return 1;
	}

	/* Get the assembly version */
	assem = ILAssembly_FromToken(image, IL_META_TOKEN_ASSEMBLY | 1);
	if(assem)
	{
		version = ILAssemblyGetVersion(assem);
	}
	else
	{
		version = zeroVersion;
	}

	/* Print the file header if we have multiple files */
	if(multiple || recursive)
	{
		if(!strcmp(filename, "-"))
		{
			fputs("stdin", stdout);
		}
		else
		{
			fputs(filename, stdout);
		}
		printf(" (version=%d.%d.%d.%d):\n",
			   (int)(version[0]), (int)(version[1]),
			   (int)(version[2]), (int)(version[3]));
	}

	/* Print the assemblies that this file depends upon */
	assem = 0;
	while((assem = (ILAssembly *)ILImageNextToken
				(image, IL_META_TOKEN_ASSEMBLY_REF, assem)) != 0)
	{
		/* Print the assembly name and version */
		putc('\t', stdout);
		name = ILAssembly_Name(assem);
		version = ILAssemblyGetVersion(assem);
		fputs(name, stdout);
		printf("/%d.%d.%d.%d => ",
			   (int)(version[0]), (int)(version[1]),
			   (int)(version[2]), (int)(version[3]));

		/* Search for the full pathname of the referenced assembly */
		path = ILImageSearchPath(name, version, filename, 0, 0, 0, 0, 0, 0);
		if(path)
		{
			fputs(path, stdout);
			if(recursive)
			{
				addModule(path);
			}
			ILFree(path);
		}
		else
		{
			fputs("??", stdout);
		}
		putc('\n', stdout);
	}

	/* Print the module references that this file depends upon */
	module = 0;
	while((module = (ILModule *)ILImageNextToken
				(image, IL_META_TOKEN_MODULE_REF, module)) != 0)
	{
		printf("\tmodule %s\n", ILModule_Name(module));
	}

	/* Print the external file references that this file depends upon */
	file = 0;
	while((file = (ILFileDecl *)ILImageNextToken
				(image, IL_META_TOKEN_FILE, file)) != 0)
	{
		if(ILFileDecl_HasMetaData(file))
		{
			printf("\tfile %s => ", ILFileDecl_Name(file));
			path = ILImageSearchPath(ILFileDecl_Name(file), 0,
									 filename, 0, 0, 0, 0, 0, 0);
			if(path)
			{
				fputs(path, stdout);
				if(recursive)
				{
					addModule(path);
				}
				ILFree(path);
			}
			else
			{
				fputs("??", stdout);
			}
			putc('\n', stdout);
		}
		else
		{
			printf("\tfile %s\n", ILFileDecl_Name(file));
		}
	}

	/* Print the manifest resource information for the file */
	res = 0;
	while((res = (ILManifestRes *)ILImageNextToken
				(image, IL_META_TOKEN_MANIFEST_RESOURCE, res)) != 0)
	{
		if(ILManifestRes_OwnerFile(res) != 0)
		{
			printf("\tresource %s => file %s\n",
				   ILManifestRes_Name(res),
				   ILFileDecl_Name(ILManifestRes_OwnerFile(res)));
		}
		else if(ILManifestRes_OwnerAssembly(res) != 0)
		{
			printf("\tresource %s => assembly %s\n",
				   ILManifestRes_Name(res),
				   ILAssembly_Name(ILManifestRes_OwnerAssembly(res)));
		}
		else
		{
			printf("\tresource %s => (internal)\n", ILManifestRes_Name(res));
		}
	}

	/* Print the external PInvoke'd functions that this file depends upon */
	pinv = 0;
	while(pinvoke && (pinv = (ILPInvoke *)ILImageNextToken
				(image, IL_META_TOKEN_IMPL_MAP, pinv)) != 0)
	{
		/* Print the name of the PInvoke method */
		putc('\t', stdout);
		name = ILPInvoke_Alias(pinv);
		if(!name)
		{
			method = ILPInvokeGetMethod(pinv);
			if(method)
			{
				name = ILMethod_Name(method);
			}
			else
			{
				name = "??";
			}
		}
		fputs(name, stdout);
		fputs(" => ", stdout);

		/* Print the module that contains the PInvoke declaration */
		path = ILPInvokeResolveModule(pinv);
		if(path)
		{
			fputs(path, stdout);
			ILFree(path);
		}
		else
		{
			fputs("??", stdout);
		}
		putc('\n', stdout);
	}

	/* Clean up and exit */
	ILImageDestroy(image);
	return 0;
}

#ifdef	__cplusplus
};
#endif
