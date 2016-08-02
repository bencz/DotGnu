/*
 * ilasm_main.c - Main entry point for "ilasm".
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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
#include "ilasm_build.h"
#include "ilasm_data.h"
#include "ilasm_output.h"
#if HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#if HAVE_SYS_STAT_H
	#include <sys/stat.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-o", 'o', 1,
		"--output file      or    -o file",
		"Specify the output file to use.  The default is\n"
		"`input.EXT' where EXT depends upon `--format'."},
	{"-e", 'e', 0,
		"--format exe       or    -e",
		"Set the output format to executable."},
	{"-d", 'd', 0,
		"--format dll       or    -d",
		"Set the output format to dynamic link library."},
	{"-j", 'j', 0,
		"--format obj       or    -j",
		"Set the output format to object file."},
	{"-g", 'g', 0, 0, 0},
	{"-n", 'n', 0, 0, 0},
	{"--debug", 'g', 0,
		"--debug            or    -g",
		"Write debug information to the output file."},
	{"--no-debug", 'n', 0,
		"--no-debug         or    -n",
		"Suppress debug information in the output file."},
	{"-s", 's', 0,
		"-fshort-insns      or    -s",
		"Use shorter instructions if possible (default)."},
	{"-l", 'l', 0,
		"-fno-short-insns   or    -l",
		"Use the stated version of an instruction."},
	{"-L", 'L', 1,
		"-fstdlib-name=name or    -L name",
		"Specify the name of the standard library (default is `mscorlib')."},
	{"-J", 'J', 0,
		"-mjvm              or    -J",
		"Assemble to JVM bytecode instead of IL."},
	{"-3", '3', 0,
		"-m32bit-only       or    -3",
		"The resulting output file can only be used on 32-bit systems."},
	{"-c", 'c', 0,
		"-mcui-subsystem    or    -c",
		"Compile for the command-line subsystem (default)."},
	{"-G", 'G', 0,
		"-mgui-subsystem    or    -G",
		"Compile for the GUI subsystem."},
	{"-v", 'v', 0, 0, 0},
	{"--output", 'o', 1, 0, 0},
	{"--format", 'F', 1, 0, 0},
	{"-f", 'f', 1, 0, 0},
	{"-m", 'm', 1, 0, 0},
	{"-D", 'D', 0, 0, 0},		/* Enable debugging of yacc parser */
	{"--version", 'v', 0,
		"--version          or    -v",
		"Print the version of the program"},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},

	/* Options for compatibility with Microsoft's IL assembler */
	{"/ali*", '?', 1, 0, 0},	/* "/alignment=integer" */
	{"/bas*", '?', 1, 0, 0},	/* "/base=integer" */
	{"/clo*", '?', 0, 0, 0},	/* "/clock" */
	{"/deb*", 'g', 0, 0, 0},	/* "/debug" */
	{"/dll", 'd', 0, 0, 0},
	{"/exe", 'e', 0, 0, 0},
	{"/fla*", '?', 1, 0, 0},	/* "/flags=integer" */
	{"/key", '?', 1, 0, 0},		/* "/key:key-identifier" */
	{"/lis*", '?', 0, 0, 0},	/* "/listing" */
	{"/nol*", 'N', 0, 0, 0},	/* "/nologo" */
	{"/out*", 'o', 1, 0, 0},	/* "/output:filename" */
	{"/qui*", 'N', 0, 0, 0},	/* "/quiet" */
	{"/res*", 'R', 1, 0, 0},	/* "/resource:filename" */
	{"/sub*", '?', 1, 0, 0},	/* "/subsystem=integer" */
	{"/?", 'h', 0, 0, 0},
	{"/hel*", 'h', 0, 0, 0},	/* "/help" */

	{0, 0, 0, 0, 0}
};

/*
 * Yacc entry points for the ILASM parser.
 */
extern int ilasm_parse(void);
extern void ilasm_restart(FILE *infile);
extern int ilasm_debug;

/*
 * Global state used by the Lex scanner and the Yacc parser.
 */
extern char *ILAsmFilename;
extern long  ILAsmLineNum;
extern int   ILAsmErrors;
extern int   ILAsmParseHexBytes;
extern int   ILAsmParseJava;

/*
 * Code generation flags.
 */
int ILAsmShortInsns = 1;

static void usage(const char *progname);
static void version(void);
static void reset(void);

int ILAsmMain(int argc, char *argv[], FILE *newStdin)
{
	char *progname = argv[0];
	const char *outputFile = 0;
	int format = -1;
	int jvmMode = 0;
	int debug = 0;
	int sawStdin;
	int state, opt, len;
	char *param;
	FILE *infile;
	FILE *outfile;
	int flags = IL_WRITEFLAG_SUBSYS_CUI;
	int defFormatIsExe = 0;
	char *resourceFile = 0;

	/* Reset the global option variables, in case we were called
	   multiple times within the same process */
	reset();

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'o':
			{
				outputFile = param;
			}
			break;

			case 'F':
			{
				if(!strcmp(param, "exe"))
				{
					format = IL_IMAGETYPE_EXE;
				}
				else if(!strcmp(param, "dll"))
				{
					format = IL_IMAGETYPE_DLL;
				}
				else if(!strcmp(param, "obj"))
				{
					format = IL_IMAGETYPE_OBJ;
				}
				else
				{
					usage(progname);
					reset();
					return 1;
				}
			}
			break;

			case 'e':
			{
				format = IL_IMAGETYPE_EXE;
			}
			break;

			case 'd':
			{
				format = IL_IMAGETYPE_DLL;
			}
			break;

			case 'j':
			{
				format = IL_IMAGETYPE_OBJ;
			}
			break;

			case 'g':
			{
				debug = 1;
			}
			break;

			case 'n':
			{
				debug = 0;
			}
			break;

			case 'L':
			{
				ILAsmLibraryName = param;
			}
			break;

			case 's':
			{
				ILAsmShortInsns = 1;
			}
			break;

			case 'l':
			{
				ILAsmShortInsns = 0;
			}
			break;

			case '3':
			{
				flags |= IL_WRITEFLAG_32BIT_ONLY;
			}
			break;

			case 'c':
			{
				flags &= ~IL_WRITEFLAG_SUBSYS_GUI;
			}
			break;

			case 'G':
			{
				flags |= IL_WRITEFLAG_SUBSYS_GUI;
			}
			break;

			case 'J':
			{
				jvmMode = 1;
				flags |= IL_WRITEFLAG_JVM_MODE;
			}
			break;

			case 'D':
			{
				ilasm_debug = 1;
			}
			break;

			case 'f':
			{
				/* Parse a flag passed down from the compiler */
				if(!strncmp(param, "stdlib-name=", 12))
				{
					ILAsmLibraryName = param + 12;
				}
				else if(!strcmp(param, "short-insns"))
				{
					ILAsmShortInsns = 1;
				}
				else if(!strcmp(param, "no-short-insns"))
				{
					ILAsmShortInsns = 0;
				}
				else
				{
					/* All other flags are ignored, because they may
					   be for other parts of the compiler chain */
				}
			}
			break;

			case 'm':
			{
				/* Parse a machine flag passed down from the compiler */
				if(!strcmp(param, "32bit-only"))
				{
					flags |= IL_WRITEFLAG_32BIT_ONLY;
				}
				else if(!strcmp(param, "cui-subsystem"))
				{
					flags &= ~IL_WRITEFLAG_SUBSYS_GUI;
				}
				else if(!strcmp(param, "gui-subsystem"))
				{
					flags |= IL_WRITEFLAG_SUBSYS_GUI;
				}
				else if(!strcmp(param, "jvm"))
				{
					jvmMode = 1;
					flags |= IL_WRITEFLAG_JVM_MODE;
				}
				else
				{
					/* All other flags are ignored, because they may
					   be for other parts of the compiler chain */
				}
			}
			break;

			case 'v':
			{
				version();
				reset();
				return 0;
			}
			/* Not reached */

			case '?':
			{
				/* Compatibility option that is ignored */
			}
			break;

			case 'N':
			{
				/* "/nologo" or "/quiet": switch the default format to ".exe"
				   instead of ".obj", to be compatible with MS'es ilasm */
				defFormatIsExe = 1;
			}
			break;

			case 'R':
			{
				/* Resource file specified using "/resource:FILE" */
				resourceFile = param;
			}
			break;

			default:
			{
				usage(progname);
				reset();
				return 1;
			}
			/* Not reached */
		}
	}

	/* We need at least one source file argument */
	if(argc <= 1)
	{
		usage(progname);
		reset();
		return 1;
	}

	/* Determine the default output format from the output file extension */
	if(format == -1)
	{
		format = (defFormatIsExe ? IL_IMAGETYPE_EXE : IL_IMAGETYPE_OBJ);
		if(outputFile)
		{
			len = strlen(outputFile);
			while(len > 0 && outputFile[len - 1] != '/' &&
			      outputFile[len - 1] != '\\' &&
				  outputFile[len - 1] != '.')
			{
				--len;
			}
			if(len > 0 && outputFile[len - 1] == '.')
			{
				if(!ILStrICmp(outputFile + len, "exe"))
				{
					format = IL_IMAGETYPE_EXE;
				}
				else if(!ILStrICmp(outputFile + len, "dll"))
				{
					format = IL_IMAGETYPE_DLL;
				}
				else if(!ILStrICmp(outputFile + len, "obj"))
				{
					format = IL_IMAGETYPE_OBJ;
				}
			}
		}
	}

	/* If we don't have an output filename, then synthesize one */
	if(!outputFile)
	{
		ILIntString ext;
		ILIntString base;
		int len;
		if(format == IL_IMAGETYPE_OBJ)
		{
			ext.string = ".obj";
		}
		else if(format == IL_IMAGETYPE_DLL)
		{
			ext.string = ".dll";
		}
		else
		{
			ext.string = ".exe";
		}
		ext.len = 4;
		len = strlen(argv[1]);
		while(len > 0 && argv[1][len - 1] != '.' &&
		      argv[1][len - 1] != '/' && argv[1][len - 1] != '\\')
		{
			--len;
		}
		if(len <= 0 || argv[1][len - 1] != '.')
		{
			len = strlen(argv[1]);
		}
		else
		{
			--len;
		}
		base.string = argv[1];
		base.len = len;
		outputFile = (ILInternAppendedString(base, ext)).string;
	}

	/* Initialize the image builders and open the output stream */
	if(!strcmp(outputFile, "-"))
	{
		ILAsmBuildInit("stdout");
		ILAsmOutCreate(stdout, 0, format, flags);
		outfile = 0;
	}
	else
	{
		ILAsmBuildInit(outputFile);
		if((outfile = fopen(outputFile, "w+b")) == NULL)
		{
			/* Try again in case the 'b' flag is not understood by libc */
			if((outfile = fopen(outputFile, "w+")) == NULL)
			{
				perror(outputFile);
				reset();
				return 1;
			}
		}
		ILAsmOutCreate(outfile, 1, format, flags);
	}
	ILAsmDebugMode = debug;

	/* Parse the source files */
	sawStdin = 0;
	ILAsmErrors = 0;
	while(argc > 1)
	{
		ILAsmLineNum = 1;
		ILAsmParseHexBytes = 0;
		if(!strcmp(argv[1], "-"))
		{
			/* Parse the contents of stdin, but only once */
			if(!sawStdin)
			{
				ILAsmFilename = "stdin";
				if(newStdin)
				{
					/* Use the replacement stream if our input is
					   actually coming from a pipe */
					ilasm_restart(newStdin);
				}
				else
				{
					ilasm_restart(stdin);
				}
				ILAsmErrors |= ilasm_parse();
				sawStdin = 1;
			}
		}
		else
		{
			/* Parse the contents of a regular file */
			if((infile = fopen(argv[1], "r")) == NULL)
			{
				perror(argv[1]);
				ILAsmErrors = 1;
			}
			else
			{
				ILAsmFilename = argv[1];
				ilasm_restart(infile);
				ILAsmErrors |= ilasm_parse();
				fclose(infile);
			}
		}
		++argv;
		--argc;
	}

	/* Process the MS compatibility resource file specification */
	if(resourceFile)
	{
		if((infile = fopen(resourceFile, "rb")) == NULL)
		{
			/* Try again, in case libc does not understand "rb" */
			infile = fopen(resourceFile, "r");
		}
		if(!infile)
		{
			perror(resourceFile);
			ILAsmErrors = 1;
		}
		else
		{
			ILAsmOutAddResource(resourceFile, infile);
			fclose(infile);
		}
	}

	/* Exit if errors occurred during the parse */
	if(ILAsmErrors)
	{
		reset();
		if(outfile != NULL)
		{
			fclose(outfile);
			ILDeleteFile(outputFile);
		}
		return 1;
	}

	/* Finalize the output */
	if(!ILAsmOutDestroy())
	{
		reset();
		if(outfile != NULL)
		{
			fclose(outfile);
		}
		return 1;
	}
	if(outfile != NULL)
	{
		fclose(outfile);
	}

	/* Change the permissions on the file if it is an executable */
#if !(defined(WIN32) || defined(_WIN32) || defined(__CYGWIN__))
	if(format == IL_IMAGETYPE_EXE && outfile)
	{
		int mask = umask(0);
		umask(mask);
		chmod(outputFile, 0777 & ~mask);
	}
#endif

	/* Done */
	reset();
	return 0;
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILASM " VERSION " - Intermediate Language Assembler\n");
	fprintf(stdout, "Copyright (c) 2001, 2002 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input.il ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("ILASM " VERSION " - Intermediate Language Assembler\n");
	printf("Copyright (c) 2001, 2002 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILASM comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Reset all global variables to their defaults so that we
 * can re-enter the assembler multiple times.
 */
static void reset(void)
{
	ilasm_debug = 0;
	ILAsmFilename = 0;
	ILAsmLineNum = 1;
	ILAsmErrors = 0;
	ILAsmParseHexBytes = 0;
	ILAsmParseJava = 0;
	ILAsmShortInsns = 1;
	ILAsmOutReset();
	ILAsmBuildReset();
	ILAsmDataReset();
}

#ifdef	__cplusplus
};
#endif
