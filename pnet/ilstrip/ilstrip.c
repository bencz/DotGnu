/*
 * ilstrip.c - Strip debug symbol information from IL binaries.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
#include "il_utils.h"
#include "il_sysio.h"
#ifndef IL_WIN32_PLATFORM
	#ifdef HAVE_SYS_TYPES_H
		#include <sys/types.h>
	#endif
	#ifdef HAVE_SYS_STAT_H
		#include <sys/stat.h>
	#endif
	#ifdef HAVE_UNISTD_H
		#include <unistd.h>
	#endif
	#ifdef HAVE_UTIME_H
		#include <utime.h>
	#endif
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-o", 'o', 1, 0, 0},
	{"--output", 'o', 1,
		"--output FILE     or -o FILE",
		"Specify the output filename, if not the same as the input."},
	{"-p", 'p', 0, 0, 0},
	{"--preserve-dates", 'p', 0,
		"--preserve-dates  or -p",
		"Preserve access and modification times."},
	{"-v", 'v', 0, 0, 0},
	{"--verbose", 'v', 0,
		"--verbose         or -v",
		"List all files that are modified."},
	{"-V", 'V', 0, 0, 0},
	{"--version", 'V', 0,
		"--version         or -V",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},

	/* GNU strip compatibility options that we don't use */
	{"-F", '*', 1, 0, 0},
	{"--target", '*', 1, 0, 0},
	{"-I", '*', 1, 0, 0},
	{"--input-target", '*', 1, 0, 0},
	{"-O", '*', 1, 0, 0},
	{"--output-target", '*', 1, 0, 0},
	{"-R", '*', 1, 0, 0},
	{"--remove-section", '*', 1, 0, 0},
	{"-N", '*', 1, 0, 0},
	{"--strip-symbol", '*', 1, 0, 0},
	{"-K", '*', 1, 0, 0},
	{"--keep-symbol", '*', 1, 0, 0},
	{"-s", '*', 0, 0, 0},
	{"--strip-all", '*', 0, 0, 0},
	{"-g", '*', 0, 0, 0},
	{"-S", '*', 0, 0, 0},
	{"--strip-debug", '*', 0, 0, 0},
	{"--strip-unneeded", '*', 0, 0, 0},
	{"-x", '*', 0, 0, 0},
	{"--discard-all", '*', 0, 0, 0},
	{"-X", '*', 0, 0, 0},
	{"--discard-locals", '*', 0, 0, 0},
	{0, 0, 0, 0, 0}
};

static void usage(const char *progname);
static void version(void);
static int stripFile(const char *progname, const char *input,
					 const char *output, int preserveDates, int verbose);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	const char *output = 0;
	int preserveDates = 0;
	int verbose = 0;
	int errors;
	int state, opt;
	char *param;

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'o':
			{
				output = param;
			}
			break;

			case 'p':
			{
				preserveDates = 1;
			}
			break;

			case 'v':
			{
				verbose = 1;
			}
			break;

			case 'V':
			{
				version();
				return 0;
			}
			/* Not reached */

			case '*': break;	/* Ignore compatibility options */

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

	/* Can only have one argument if an output file is specified */
	if(output && argc != 2)
	{
		usage(progname);
		return 1;
	}

	/* Strip the specified files */
	errors = 0;
	while(argc > 1)
	{
		if(output && !strcmp(output, argv[1]))
		{
			output = 0;
		}
		errors |= stripFile(progname, argv[1], output, preserveDates, verbose);
		++argv;
		--argc;
	}
	
	/* Done */
	return errors;
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILSTRIP " VERSION " - IL Binary Strip Utility\n");
	fprintf(stdout, "Copyright (c) 2003 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	fprintf(stdout, "ILSTRIP " VERSION " - IL Binary Strip Utility\n");
	fprintf(stdout, "Copyright (c) 2003 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILSTRIP comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Report that a file has the wrong format.
 */
static void wrongFormat(const char *progname, const char *input, FILE *file)
{
	fclose(file);
	fputs(progname, stderr);
	fputs(": ", stderr);
	fputs(input, stderr);
	fputs(": File format not recognized\n", stderr);
}

/*
 * Open a file, trying both binary and text versions.
 */
static FILE *openFile(const char *progname, const char *filename,
					  const char *openBinary, const char *openText)
{
	FILE *file;
	if((file = fopen(filename, openBinary)) == 0)
	{
		/* Try again, in case the "b" flag is not understood by libc */
		if((file = fopen(filename, openText)) == 0)
		{
			fputs(progname, stderr);
			fputs(": ", stderr);
			perror(filename);
			return 0;
		}
	}
	return file;
}

/*
 * Strip an IL binary.
 */
static int stripFile(const char *progname, const char *input,
					 const char *output, int preserveDates, int verbose)
{
	FILE *infile;
	FILE *outfile;
	char buffer[4096];
	ILUInt32 len;
	ILUInt32 offset;
	ILUInt32 optHeader;
	ILUInt32 numSectionsOffset;
	ILUInt32 numSections;
	ILUInt32 headerSize;
	ILUInt32 debugRVA;
	ILUInt32 debugStart;
	const char *tempFile;
	char *nameBuf;

	/* Attempt to open the specified file */
	if((infile = openFile(progname, input, "rb", "r")) == 0)
	{
		return 1;
	}

	/* Read the first 4k of the file so that we can parse the headers */
	len = (ILUInt32)fread(buffer, 1, sizeof(buffer), infile);

	/* Do we have an MS-DOS stub and PE signature?  Note: other "strip"
	   programs support object files and ".a" archives, but we don't yet */
	if(len < 64)
	{
		wrongFormat(progname, input, infile);
		return 1;
	}
	if(buffer[0] != 'M' || buffer[1] != 'Z')
	{
		wrongFormat(progname, input, infile);
		return 1;
	}
	offset = IL_READ_UINT32(buffer + 60);
	if((offset + 4) >= len)
	{
		wrongFormat(progname, input, infile);
		return 1;
	}
	if(buffer[offset] != 'P' || buffer[offset + 1] != 'E' ||
	   buffer[offset + 2] != '\0' || buffer[offset + 3] != '\0')
	{
		wrongFormat(progname, input, infile);
		return 1;
	}
	offset += 4;

	/* Process the PE/COFF header and skip to the start of the section table */
	if((offset + 20) > len)
	{
		wrongFormat(progname, input, infile);
		return 1;
	}
	numSectionsOffset = offset + 2;
	numSections = IL_READ_UINT16(buffer + offset + 2);
	headerSize = IL_READ_UINT16(buffer + offset + 16);
	if(headerSize < 216 || headerSize > 1024)
	{
		wrongFormat(progname, input, infile);
		return 1;
	}
	optHeader = offset + 20;
	offset += 20 + headerSize;
	if(offset >= len)
	{
		wrongFormat(progname, input, infile);
		return 1;
	}

	/* Determine if the last section is ".ildebug".  If it isn't,
	   then the binary has already been stripped of debug symbols */
	if((offset + 40 * numSections) > len)
	{
		wrongFormat(progname, input, infile);
		return 1;
	}
	if(numSections == 0 ||
	   ILMemCmp(buffer + offset + (numSections - 1) * 40, ".ildebug", 8) != 0)
	{
		/* We exit successfully if the file is already stripped */
		if(output)
		{
			/* Copy the entire contents to the output */
			if((outfile = openFile(progname, output, "wb", "w")) == 0)
			{
				fclose(infile);
				return 1;
			}
			while(len > 0)
			{
				fwrite(buffer, 1, (int)len, outfile);
				if(len < sizeof(buffer))
				{
					len = 0;
				}
				else
				{
					len = (ILUInt32)fread(buffer, 1, sizeof(buffer), infile);
				}
			}
			fclose(outfile);
		}
		fclose(infile);
		return 0;
	}

	/* Patch the header to remove the ".ildebug" section, and determine
	   how many bytes will remain in the file once the section is gone */
	debugRVA = IL_READ_UINT32(buffer + offset + (numSections - 1) * 40 + 12);
	debugStart = IL_READ_UINT32(buffer + offset + (numSections - 1) * 40 + 20);
	ILMemZero(buffer + offset + (numSections - 1) * 40, 40);
	IL_WRITE_UINT32(buffer + optHeader + 56, debugRVA);	/* New image size */
	IL_WRITE_UINT16(buffer + numSectionsOffset, numSections - 1);
	if(debugStart < len)
	{
		len = debugStart;
	}

	/* Create the temporary output filename */
	if(output)
	{
		tempFile = output;
		nameBuf = 0;
	}
	else
	{
		if((nameBuf = (char *)ILMalloc(strlen(input) + 5)) == 0)
		{
			fputs("virtual memory exhausted\n", stderr);
			fclose(infile);
			return 1;
		}
		strcpy(nameBuf, input);
		strcat(nameBuf, ".tmp");
		tempFile = nameBuf;
	}

	/* Print what we are doing if the verbose flag is set */
	if(verbose)
	{
		printf("copy from %s(pecoff-cli) to %s(pecoff-cli)\n",
			   input, tempFile);
	}

	/* Open the temporary file */
	if((outfile = openFile(progname, tempFile, "wb", "w")) == 0)
	{
		fclose(infile);
		if(nameBuf)
		{
			ILFree(nameBuf);
		}
		return 1;
	}

	/* Copy the contents across, with the ".ildebug" section removed */
	offset = 0;
	while(offset < debugStart && len > 0)
	{
		fwrite(buffer, 1, (int)len, outfile);
		offset += len;
		if(offset < debugStart && len >= sizeof(buffer))
		{
			len = (debugStart - offset);
			if(len > sizeof(buffer))
			{
				len = sizeof(buffer);
			}
			len = fread(buffer, 1, (int)len, infile);
		}
		else
		{
			len = 0;
		}
	}

	/* Close the streams */
	fclose(infile);
	fclose(outfile);

#if !defined(IL_WIN32_PLATFORM) && defined(HAVE_STAT) && defined(HAVE_UTIME)
	/* Copy the date information across if necessary */
	if(preserveDates)
	{
		struct stat st;
		struct utimbuf ut;
		if(stat(input, &st) >= 0)
		{
			ut.actime = st.st_atime;
			ut.modtime = st.st_mtime;
			utime(tempFile, &ut);
		}
	}
#endif

	/* Rename the temporary file, to replace the original input */
	if(!output)
	{
		ILRenameDir(tempFile, input);
	}
	if(nameBuf)
	{
		ILFree(nameBuf);
	}

	/* Finished */
	return 0;
}

#ifdef	__cplusplus
};
#endif
