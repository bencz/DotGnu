/*
 * doc_main.c - Main entry point for documentation converters.
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
#include "il_program.h"
#include "il_utils.h"
#include "doc_tree.h"
#include "doc_backend.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-o", 'o', 1, 0, 0},
	{"--output", 'o', 1,
		"--output PATH  or -o PATH",
		"Specify the output pathname."},
	{"-f", 'f', 1, 0, 0},
	{"--flag", 'f', 1,
		"--flag FLAG    or -f FLAG",
		"Specify a processing flag for the converter."},
	{"-v", 'v', 0, 0, 0},
	{"--version", 'v', 0,
		"--version      or -v",
		"Print the version of the program"},
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
static void loadXML(ILDocTree *tree, const char *filename,
					FILE *stream, int closeStream, const char *progname);

/*
 * Global variables.
 */
static char **flags;
static int    numFlags;

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	char *outputPath = NULL;
	int sawStdin;
	int state, opt;
	char *param;
	FILE *infile;
	int errors;
	ILDocTree *tree;
	char **inputs;
	int numInputs;

	/* Initialize the flag buffer */
	flags = (char **)ILMalloc(sizeof(char *) * argc);
	if(!flags)
	{
		ILDocOutOfMemory(progname);
	}
	numFlags = 0;

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'o':
			{
				outputPath = param;
			}
			break;

			case 'f':
			{
				flags[numFlags++] = param;
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

	/* Get the default output pathname if necessary */
	if(!outputPath)
	{
		outputPath = ILDocDefaultOutput(argc - 1, argv + 1, progname);
		if(!outputPath)
		{
			return 1;
		}
	}

	/* Validate the output pathname */
	if(!ILDocValidateOutput(outputPath, progname))
	{
		return 1;
	}

	/* Create the documentation tree */
	tree = ILDocTreeCreate();
	if(!tree)
	{
		ILDocOutOfMemory(progname);
	}

	/* Load the XML documentation files into memory */
	sawStdin = 0;
	errors = 0;
	inputs = argv + 1;
	numInputs = argc - 1;
	while(argc > 1)
	{
		if(!strcmp(argv[1], "-"))
		{
			/* Load the contents of stdin, but only once */
			if(!sawStdin)
			{
				loadXML(tree, "stdin", stdin, 0, progname);
				sawStdin = 1;
			}
		}
		else
		{
			/* Load the contents of a regular file */
			if((infile = fopen(argv[1], "r")) == NULL)
			{
				perror(argv[1]);
				errors = 1;
				++argv;
				--argc;
				continue;
			}
			loadXML(tree, argv[1], infile, 1, progname);
		}
		++argv;
		--argc;
	}

	/* Bail out if we got errors while loading the input files */
	if(errors)
	{
		ILDocTreeDestroy(tree);
		return 1;
	}

	/* Sort the contents of the documentation tree */
	if(!ILDocTreeSort(tree))
	{
		ILDocOutOfMemory(progname);
	}

	/* Call the backend to perform the conversion */
	if(!ILDocConvert(tree, numInputs, inputs, outputPath, progname))
	{
		ILDocTreeDestroy(tree);
		return 1;
	}

	/* Done */
	ILDocTreeDestroy(tree);
	return 0;
}

static void usage(const char *progname)
{
	fprintf(stdout, "%s\n", ILDocProgramHeader);
	fprintf(stdout, "Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
	ILCmdLineHelp(ILDocProgramOptions);
}

static void version(void)
{
	printf("%s\n", ILDocProgramHeader);
	printf("Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("%s comes with ABSOLUTELY NO WARRANTY.  This is free software,\n",
		   ILDocProgramName);
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

void ILDocOutOfMemory(const char *progname)
{
	if(progname)
	{
		fputs(progname, stderr);
		fputs(": ", stderr);
	}
	fputs("virtual memory exhausted\n", stderr);
	exit(1);
}

int ILDocFlagSet(const char *flag)
{
	int num;
	for(num = 0; num < numFlags; ++num)
	{
		if(!strcmp(flags[num], flag))
		{
			return 1;
		}
	}
	return 0;
}

const char *ILDocFlagValue(const char *flag)
{
	int num, len1, len2;
	len1 = strlen(flag);
	for(num = 0; num < numFlags; ++num)
	{
		len2 = strlen(flags[num]);
		if(len1 <= len2 && !strncmp(flags[num], flag, len1))
		{
			if(flags[num][len1] == '\0')
			{
				return "";
			}
			else if(flags[num][len1] == '=')
			{
				return flags[num] + len1 + 1;
			}
		}
	}
	return 0;
}

const char *ILDocFlagValueN(const char *flag, int n)
{
	int num, len1, len2;
	len1 = strlen(flag);
	for(num = 0; num < numFlags; ++num)
	{
		len2 = strlen(flags[num]);
		if(len1 <= len2 && !strncmp(flags[num], flag, len1))
		{
			if(flags[num][len1] == '\0')
			{
				if(n == 0)
				{
					return "";
				}
				else
				{
					--n;
				}
			}
			else if(flags[num][len1] == '=')
			{
				if(n == 0)
				{
					return flags[num] + len1 + 1;
				}
				else
				{
					--n;
				}
			}
		}
	}
	return 0;
}

/*
 * XML reader function.
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

/*
 * Load the contents of an XML input file.
 */
static void loadXML(ILDocTree *tree, const char *filename,
				    FILE *stream, int closeStream, const char *progname)
{
	ILXMLReader *reader = ILXMLCreate(xmlRead, stream, 0);
	if(!reader)
	{
		if(closeStream)
		{
			fclose(stream);
		}
		ILDocOutOfMemory(progname);
	}
	if(!ILDocTreeLoad(tree, reader))
	{
		ILXMLDestroy(reader);
		if(closeStream)
		{
			fclose(stream);
		}
		ILDocOutOfMemory(progname);
	}
	ILXMLDestroy(reader);
	if(closeStream)
	{
		fclose(stream);
	}
}

#ifdef	__cplusplus
};
#endif
