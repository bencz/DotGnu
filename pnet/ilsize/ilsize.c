/*
 * ilsize.c - Print information about section sizes for IL binaries.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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
#include "il_program.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-d", 'd', 0,
		"-d",
		"Set the output radix to decimal (10)."},
	{"-o", 'o', 0,
		"-o",
		"Set the output radix to octal (8)."},
	{"-x", 'x', 0,
		"-x",
		"Set the output radix to hexadecimal (16)."},
	{"-D", 'D', 0, 0, 0},
	{"-c", 'c', 0, 0, 0},
	{"-v", 'v', 0, 0, 0},
	{"-V", 'v', 0, 0, 0},
	{"--radix", 'r', 1,
		"--radix num",
		"Define the output radix.  The default is 10."},
	{"--detailed", 'D', 0,
		"--detailed    or -D",
		"Use a more detailed form of output."},
	{"--class-sizes", 'c', 0,
		"--class-sizes or -c",
		"Use a more detailed form of output."},
	{"--version", 'v', 0,
		"--version     or -v or -V",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

static void usage(const char *progname);
static void version(void);
static int printSizes(const char *filename, ILContext *context, int radix);
static int printDetailed(const char *filename, ILContext *context, int radix);
static int printClassSizes(const char *filename, ILContext *context);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	int radix = 10;
	int detailed = 0;
	int classSizes = 0;
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
			case 'r':
			{
				radix = atoi(param);
				if(radix != 8 && radix != 10 && radix != 16)
				{
					fprintf(stderr, "%s: invalid radix `%s'\n",
							progname, param);
					return 1;
				}
			}
			break;

			case 'd':
			{
				radix = 10;
			}
			break;

			case 'o':
			{
				radix = 8;
			}
			break;

			case 'x':
			{
				radix = 16;
			}
			break;

			case 'D':
			{
				detailed = 1;
			}
			break;

			case 'c':
			{
				classSizes = 1;
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

	/* Print the headers */
	if(classSizes)
	{
		printf("   meta  loaded    code      class\n");
	}
	else if(!detailed)
	{
		printf("   code    meta     res   other     %s     %s filename\n",
			   (radix == 8 ? "oct" : (radix == 10 ? "dec" : "hex")),
			   (radix == 16 ? "dec" : "hex"));
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
				if(detailed)
				{
					errors |= printDetailed("-", context, radix);
				}
				else if(classSizes)
				{
					errors |= printClassSizes("-", context);
				}
				else
				{
					errors |= printSizes("-", context, radix);
				}
				sawStdin = 1;
			}
		}
		else
		{
			/* Dump the contents of a regular file */
			if(detailed)
			{
				errors |= printDetailed(argv[1], context, radix);
			}
			else if(classSizes)
			{
				errors |= printClassSizes(argv[1], context);
			}
			else
			{
				errors |= printSizes(argv[1], context, radix);
			}
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
	fprintf(stdout, "ILSIZE " VERSION " - IL Image Size Utility\n");
	fprintf(stdout, "Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("ILSIZE " VERSION " - IL Image Size Utility\n");
	printf("Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILSIZE comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Load an IL image and get general size information.
 */
static ILImage *loadImage(const char *filename, ILContext *context, int flags,
						  unsigned long *total, ILUInt32 *code,
						  ILUInt32 *meta, ILUInt32 *res,
						  unsigned long *other)
{
	ILImage *image;
	void *addr;

	/* Attempt to load the image into memory */
	if(ILImageLoadFromFile(filename, context, &image, flags, 1) != 0)
	{
		return 0;
	}

	/* Collect sizes of important sections */
	*total = ILImageLength(image);
	if(!ILImageGetSection(image, IL_SECTION_CODE, &addr, code))
	{
		*code = 0;
	}
	if(!ILImageGetSection(image, IL_SECTION_METADATA, &addr, meta))
	{
		*meta = 0;
	}
	if(!ILImageGetSection(image, IL_SECTION_RESOURCES, &addr, res))
	{
		*res = 0;
	}
	*other = *total - *code - *meta - *res;

	/* Loaded and ready to go */
	return image;
}

/*
 * Load an IL image from an input stream and print its size information.
 */
static int printSizes(const char *filename, ILContext *context, int radix)
{
	ILImage *image;
	unsigned long total;
	ILUInt32 code;
	ILUInt32 meta;
	ILUInt32 res;
	unsigned long other;

	/* Attempt to load the image into memory */
	image = loadImage(filename, context,
					  IL_LOADFLAG_FORCE_32BIT | IL_LOADFLAG_NO_METADATA,
					  &total, &code, &meta, &res, &other);
	if(!image)
	{
		return 1;
	}

	/* Print the size details to stdout */
	if(radix == 10)
	{
		printf("%7lu %7lu %7lu %7lu %7lu %7lx %s\n",
			   (unsigned long)code, (unsigned long)meta, (unsigned long)res,
			   other, total, total, filename);
	}
	else if(radix == 8)
	{
		printf("%7lo %7lo %7lo %7lo %7lo %7lx %s\n",
			   (unsigned long)code, (unsigned long)meta, (unsigned long)res,
			   other, total, total, filename);
	}
	else
	{
		printf("%7lx %7lx %7lx %7lx %7lx %7lu %s\n",
			   (unsigned long)code, (unsigned long)meta, (unsigned long)res,
			   other, total, total, filename);
	}

	/* Clean up and exit */
	ILImageDestroy(image);
	return 0;
}

/*
 * Print a line of detailed output.
 */
static void print(const char *name, int radix, unsigned long value)
{
	if(radix == 10)
	{
		printf("%-20s %7lu\n", name, value);
	}
	else if(radix == 16)
	{
		printf("%-20s %#10lx\n", name, value);
	}
	else
	{
		printf("%-20s %#10lo\n", name, value);
	}
}

/*
 * Print an assembly's version.
 */
static void printVersion(const ILUInt16 *version)
{
	char buffer[64];
	sprintf(buffer, "%d.%d.%d.%d", (int)(version[0]), (int)(version[1]),
			(int)(version[2]), (int)(version[3]));
	printf("version %20s\n", buffer);
}

/*
 * Token tables and their names.
 */
static struct
{
	const char *name;
	ILToken type;

} tokenTables[] = {
	{"modules",				IL_META_TOKEN_MODULE},
	{"type refs",			IL_META_TOKEN_TYPE_REF},
	{"type defs",			IL_META_TOKEN_TYPE_DEF},
	{"fields",				IL_META_TOKEN_FIELD_DEF},
	{"methods",				IL_META_TOKEN_METHOD_DEF},
	{"parameters",			IL_META_TOKEN_PARAM_DEF},
	{"interface decls",		IL_META_TOKEN_INTERFACE_IMPL},
	{"member refs",			IL_META_TOKEN_MEMBER_REF},
	{"constants",			IL_META_TOKEN_CONSTANT},
	{"attributes",			IL_META_TOKEN_CUSTOM_ATTRIBUTE},
	{"marshal decls",		IL_META_TOKEN_FIELD_MARSHAL},
	{"security decls",		IL_META_TOKEN_DECL_SECURITY},
	{"class layout decls",	IL_META_TOKEN_CLASS_LAYOUT},
	{"field layout decls",	IL_META_TOKEN_FIELD_LAYOUT},
	{"stand alone sigs",	IL_META_TOKEN_STAND_ALONE_SIG},
	{"event mappings",		IL_META_TOKEN_EVENT_MAP},
	{"events",				IL_META_TOKEN_EVENT},
	{"property mappings",	IL_META_TOKEN_PROPERTY_MAP},
	{"properties",			IL_META_TOKEN_PROPERTY},
	{"semantic decls",		IL_META_TOKEN_METHOD_SEMANTICS},
	{"overrides",			IL_META_TOKEN_METHOD_IMPL},
	{"module refs",			IL_META_TOKEN_MODULE_REF},
	{"type specs",			IL_META_TOKEN_TYPE_SPEC},
	{"pinvoke decls",		IL_META_TOKEN_IMPL_MAP},
	{"field rva decls",		IL_META_TOKEN_FIELD_RVA},
	{"assemblies",			IL_META_TOKEN_ASSEMBLY},
	{"processor defs",		IL_META_TOKEN_PROCESSOR_DEF},
	{"os defs",				IL_META_TOKEN_OS_DEF},
	{"assembly refs",		IL_META_TOKEN_ASSEMBLY_REF},
	{"processor refs",		IL_META_TOKEN_PROCESSOR_REF},
	{"os refs",				IL_META_TOKEN_OS_REF},
	{"files",				IL_META_TOKEN_FILE},
	{"exported types",		IL_META_TOKEN_EXPORTED_TYPE},
	{"manifest resources",	IL_META_TOKEN_MANIFEST_RESOURCE},
	{"nested classes",		IL_META_TOKEN_NESTED_CLASS},
	{"generic parameters",	IL_META_TOKEN_GENERIC_PAR},
	{"method specs",		IL_META_TOKEN_METHOD_SPEC},
	{0,						0}
};

/*
 * Load an IL image from an input stream and print detailed information.
 */
static int printDetailed(const char *filename, ILContext *context, int radix)
{
	ILImage *image;
	unsigned long total;
	ILUInt32 code;
	ILUInt32 meta;
	ILUInt32 res;
	unsigned long other;
	int index;
	unsigned long num;

	/* Attempt to load the image into memory */
	image = loadImage(filename, context,
					  IL_LOADFLAG_FORCE_32BIT | IL_LOADFLAG_NO_RESOLVE,
					  &total, &code, &meta, &res, &other);
	if(!image)
	{
		return 1;
	}

	/* Print the general information on the file */
	printf("%s  :\n", filename);
	printVersion(ILAssemblyGetVersion((ILAssembly *)
					ILImageTokenInfo(image, IL_META_TOKEN_ASSEMBLY | 1)));
	print("code", radix, code);
	print("meta", radix, meta);
	print("res", radix, res);
	print("other", radix, other);
	print("total", radix, total);

	/* Print count information for the various token types */
	index = 0;
	while(tokenTables[index].name != 0)
	{
		num = ILImageNumTokens(image, tokenTables[index].type);
		if(num > 0)
		{
			print(tokenTables[index].name, radix, num);
		}
		++index;
	}

	/* Add some space between multiple files */
	printf("\n\n");

	/* Clean up and exit */
	ILImageDestroy(image);
	return 0;
}

/*
 * Import a function from "ilsize_est.c".
 */
void _ILDumpClassSizes(ILImage *image);

/*
 * Load an IL image from an input stream and print class size information.
 */
static int printClassSizes(const char *filename, ILContext *context)
{
	ILImage *image;
	unsigned long total;
	ILUInt32 code;
	ILUInt32 meta;
	ILUInt32 res;
	unsigned long other;

	/* Attempt to load the image into memory */
	image = loadImage(filename, context,
					  IL_LOADFLAG_FORCE_32BIT | IL_LOADFLAG_NO_RESOLVE,
					  &total, &code, &meta, &res, &other);
	if(!image)
	{
		return 1;
	}

	/* Print the class size information */
	_ILDumpClassSizes(image);

	/* Clean up and exit */
	ILImageDestroy(image);
	return 0;
}

#ifdef	__cplusplus
};
#endif
