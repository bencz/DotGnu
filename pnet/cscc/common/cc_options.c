/*
 * cc_options.c - Command-line option processing.
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

#include <stdio.h>
#include "il_system.h"
#include "il_utils.h"
#include "cc_options.h"
#include "cc_intl.h"
#ifdef HAVE_SYS_STAT_H
#include <sys/stat.h>
#endif
#ifdef HAVE_UNISTD_H
#include <unistd.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Command-line option values.
 */
char *progname;
char *version_name;
int compile_flag = 0;
int assemble_flag = 0;
int preprocess_flag = 0;
int no_preproc_lines_flag = 0;
int preproc_comments_flag = 0;
int debug_flag = 0;
int nostdinc_flag = 0;
int nostdinc_cpp_flag = 0;
int nostdlib_flag = 0;
int undef_flag = 0;
int shared_flag = 0;
int static_flag = 0;
int executable_flag = 0;
int optimize_flag = 0;
int disable_optimizations = 0;
int resources_only = 0;
char **pre_defined_symbols = 0;
int num_pre_defined_symbols = 0;
char **user_defined_symbols = 0;
int num_user_defined_symbols = 0;
char **undefined_symbols = 0;
int num_undefined_symbols = 0;
char **include_dirs = 0;
int num_include_dirs = 0;
char **sys_include_dirs = 0;
int num_sys_include_dirs = 0;
char **sys_cpp_include_dirs = 0;
int num_sys_cpp_include_dirs = 0;
char **link_dirs = 0;
int num_link_dirs = 0;
char **sys_link_dirs = 0;
int num_sys_link_dirs = 0;
char **libraries = 0;
int num_libraries = 0;
char *output_filename = 0;
char **input_files = 0;
int num_input_files = 0;
char *entry_point = 0;
char **extension_flags = 0;
int num_extension_flags = 0;
char **warning_flags = 0;
int num_warning_flags = 0;
int inhibit_warnings = 0;
int all_warnings = 0;
int warnings_as_errors = 0;
char **machine_flags = 0;
int num_machine_flags = 0;
int prog_language = PROG_LANG_DEFAULT;
char *prog_language_name = 0;
int dump_output_format = DUMP_OUTPUT_ONLY;
int verbose_mode = VERBOSE_NONE;
char **files_to_link = 0;
int *files_to_link_temp = 0;
int num_files_to_link = 0;
char **imacros_files = 0;
int num_imacros_files = 0;
int dependency_level = DEP_LEVEL_NONE;
int dependency_gen_flag = 0;
char *dependency_file = 0;
int preproc_show_headers = 0;

/*
 * Add a string to a list of strings.
 */
static void AddString(char ***list, int *num, char *str)
{
	char **newlist = (char **)ILRealloc(*list, sizeof(char *) * (*num + 1));
	if(!newlist)
	{
		CCOutOfMemory();
	}
	*list = newlist;
	(*list)[*num] = str;
	++(*num);
}

/*
 * Determine if a directory exists.
 */
static int DirExists(const char *pathname)
{
#ifdef HAVE_STAT
	struct stat st;
	return (stat(pathname, &st) >= 0);
#else
#ifdef HAVE_ACCESS
	return (access(pathname, 0) >= 0);
#else
	return 1;
#endif
#endif
}

void CCAddPathStrings(char ***list, int *num, char *path,
					  char *standard1, char *standard2)
{
	int len, separator;
	char *newstr;

	/* Add the standard paths, derived from the install location */
	if(standard1)
	{
		if(!CCStringListContains(*list, *num, standard1) &&
		   DirExists(standard1))
		{
			AddString(list, num, standard1);
		}
	}
	if(standard2)
	{
		if(!CCStringListContains(*list, *num, standard2) &&
		   DirExists(standard2))
		{
			AddString(list, num, standard2);
		}
	}

	/* Bail out if no main path */
	if(!path)
	{
		return;
	}

	/* Attempt to discover the correct path separator to use */
#ifdef IL_WIN32_PLATFORM
	if(strchr(path, ';') != 0)
	{
		/* The path already uses ';', so that is probably the separator */
		separator = ';';
	}
	else
	{
		/* Deal with the ambiguity between ':' used as a separator
		   and ':' used to specify a drive letter */
		if(((path[0] >= 'A' && path[0] <= 'Z') ||
		    (path[0] >= 'a' && path[0] <= 'z')) && path[1] == ':')
		{
			/* The path is probably one directory, starting
			   with a drive letter */
			separator = ';';
		}
		else
		{
			/* The path is probably Cygwin-like, using ':' to separate */
			separator = ':';
		}
	}
#else
	separator = ':';
#endif

	/* Split the path into separate components */
	while(*path != '\0')
	{
		if(*path == separator || *path == ' ' || *path == '\t' ||
		   *path == '\r' || *path == '\n')
		{
			++path;
			continue;
		}
		len = 1;
		while(path[len] != '\0' && path[len] != separator)
		{
			++len;
		}
		while(len > 0 && (path[len - 1] == ' ' || path[len - 1] == '\t' ||
						  path[len - 1] == '\r' || path[len - 1] == '\n'))
		{
			--len;
		}
		newstr = (char *)ILMalloc(len + 1);
		if(!newstr)
		{
			CCOutOfMemory();
		}
		ILMemCpy(newstr, path, len);
		newstr[len] = '\0';
		if(!CCStringListContains(*list, *num, newstr) &&
		   DirExists(newstr))
		{
			AddString(list, num, newstr);
		}
		else
		{
			ILFree(newstr);
		}
		path += len;
	}
}

/*
 * Remove all occurrences of a string from a list of strings.
 */
static void RemoveString(char ***list, int *num, char *str)
{
	int posn = 0;
	int posn2;
	while(posn < *num)
	{
		if(!strcmp((*list)[posn], str))
		{
			for(posn2 = posn; posn2 < (*num - 1); ++posn2)
			{
				(*list)[posn2] = (*list)[posn2 + 1];
			}
			--(*num);
		}
		else
		{
			++posn;
		}
	}
}

/*
 * Process a -o option.
 */
static void oOption(char *arg)
{
	output_filename = arg;
}

/*
 * Process a -D option.
 */
static void DOption(char *arg)
{
	AddString(&user_defined_symbols, &num_user_defined_symbols, arg);
}

/*
 * Process a -U option.
 */
static void UOption(char *arg)
{
	RemoveString(&user_defined_symbols, &num_user_defined_symbols, arg);
	RemoveString(&pre_defined_symbols, &num_pre_defined_symbols, arg);
	AddString(&undefined_symbols, &num_undefined_symbols, arg);
}

/*
 * Process a -I option.
 */
static void IOption(char *arg)
{
	AddString(&include_dirs, &num_include_dirs, arg);
}

/*
 * Process a -imacros option.
 */
static void IMacrosOption(char *arg)
{
	AddString(&imacros_files, &num_imacros_files, arg);
}

/*
 * Process a -L option.
 */
static void LOption(char *arg)
{
	AddString(&link_dirs, &num_link_dirs, arg);
}

/*
 * Process a -l option.
 */
static void lOption(char *arg)
{
	AddString(&libraries, &num_libraries, arg);
}

/*
 * Process a -gtk option.
 */
static void gtkOption(char *arg)
{
	lOption("gtk-sharp");
	lOption("gdk-sharp");
	lOption("atk-sharp");
	lOption("pango-sharp");
	lOption("glib-sharp");
	lOption("System.Drawing");
	lOption("System");
}

/*
 * Process a -gnome option.
 */
static void gnomeOption(char *arg)
{
	lOption("gnome-sharp");
	gtkOption(arg);
}

/*
 * Process a -winforms option.
 */
static void winformsOption(char *arg)
{
	lOption("System");
	lOption("System.Drawing");
	lOption("System.Windows.Forms");
}

/*
 * Process a -f option.
 */
static void fOption(char *arg)
{
	AddString(&extension_flags, &num_extension_flags, arg);
}

/*
 * Process a -m option.
 */
static void mOption(char *arg)
{
	AddString(&machine_flags, &num_machine_flags, arg);
}

/*
 * Process a -W option.
 */
static void WOption(char *arg)
{
	AddString(&warning_flags, &num_warning_flags, arg);
}

/*
 * Process a -e option.
 */
static void eOption(char *arg)
{
	entry_point = arg;
}

/*
 * Process a -x option.
 */
static void xOption(char *arg)
{
	if(!strcmp(arg, "cs") || !strcmp(arg, "csharp"))
	{
		prog_language = PROG_LANG_CSHARP;
	}
	else if(!strcmp(arg, "il") || !strcmp(arg, "assembler"))
	{
		prog_language = PROG_LANG_IL;
	}
	else if(!strcmp(arg, "jl"))
	{
		prog_language = PROG_LANG_JL;
	}
	else if(!strcmp(arg, "c"))
	{
		prog_language = PROG_LANG_C;
	}
	else if(!strcmp(arg, "vb"))
	{
		prog_language = PROG_LANG_VB;
	}
	else if(!strcmp(arg, "visualbasic"))
	{
		prog_language = PROG_LANG_VB;
	}
	else if(!strcmp(arg, "java"))
	{
		prog_language = PROG_LANG_JAVA;
	}
	else if(!strcmp(arg, "bf"))
	{
		prog_language = PROG_LANG_BF;
	}
	else if(!strcmp(arg, "brainfuck"))
	{
		prog_language = PROG_LANG_BF;
	}
	else if(!strcmp(arg, "none"))
	{
		prog_language = PROG_LANG_DEFAULT;
	}
	else
	{
		fprintf(stderr, _("%s: language %s not recognized\n"), progname, arg);
	}
}

/*
 * Process a -d option.
 */
static void dOption(char *arg)
{
	while(*arg != '\0')
	{
		switch(*arg)
		{
			case 'M':	dump_output_format = DUMP_MACROS_ONLY; break;
			case 'N':	dump_output_format = DUMP_MACROS_AND_OUTPUT; break;
			case 'D':	dump_output_format = DUMP_MACROS_AND_OUTPUT; break;

			default:
			{
				fprintf(stderr, _("%s: unrecognized option `-d%c'\n"),
						progname, *arg);
			}
			break;
		}
		++arg;
	}
}

/*
 * Process a -O0 option.
 */
static void O0Option(char *arg)
{
	optimize_flag = 0;
	disable_optimizations = 1;
}

/*
 * Process a -Wp,-MD option.
 */
static void mdOption(char *arg)
{
	dependency_level = DEP_LEVEL_MD;
	dependency_file = arg;
}

/*
 * Process a -Wp,-MMD option.
 */
static void mmdOption(char *arg)
{
	dependency_level = DEP_LEVEL_MMD;
	dependency_file = arg;
}

/*
 * Process a -MF option.
 */
static void mfOption(char *arg)
{
	dependency_file = arg;
}

/*
 * Process a -v option (this is only used if "-v" is the only option).
 * The "-dumpversion" option can also be used for this.
 */
static void vOption(char *arg)
{
	/* We don't translate this because it may be grepped by automatic tools */
	fprintf(stderr, "%s version " VERSION "\n", version_name);
	exit(0);
}

/*
 * Forward declaration.
 */
static void helpOption(char *arg);

/*
 * Table of command-line options.
 */
typedef struct
{
	const char *name;
	int nameSize;
	int *flag;
	int value;
	void (*func)(char *arg);
	char *helpName;
	char *helpMsg;

} CmdLineOpt;
static CmdLineOpt const options[] = {
	/* Regular options */
	{"-o",			2,	0,						0,	oOption,
			N_("-o <file>"), N_("Place the output into <file>")},
	{"-D",			2,	0,						0,	DOption,
			N_("-D<name>"), N_("Define the preprocessor symbol <name>")},
	{"-U",			2,	0,						0,	UOption,
			N_("-U<name>"), N_("Undefine the preprocessor symbol <name>")},
	{"-I-",			0,	0,						0,	0,	0},
	{"-I",			2,	0,						0,	IOption,
			N_("-I<dir>"), N_("Add <dir> to the library include path")},
	{"-imacros",	8,	0,						0,	IMacrosOption,
			N_("-imacros <file>"), N_("Preprocess <file> before sources")},
	{"-L",			2,	0,						0,	LOption,
			N_("-L<dir>"), N_("Add <dir> to the library link path")},
	{"-l",			2,	0,						0,	lOption,
			N_("-l<lib>"), N_("Link against the library <lib>")},
	{"-gtk",		0,	0,						0,	gtkOption,
			N_("-gtk"), N_("Link against the Gtk# libraries")},
	{"-gnome",		0,	0,						0,	gnomeOption,
			N_("-gnome"), N_("Link against the Gnome# libraries")},
	{"-winforms",		0,	0,						0,	winformsOption,
			N_("-winforms"), N_("Link against the System.Windows.Forms libraries")},
	{"-f",			2,	0,						0,	fOption, 0, 0},
	{"-m",			2,	0,						0,	mOption, 0, 0},
	{"-Wl,",		4,	0,						0,	0,	0, 0},
	{"-W",			0,	&all_warnings,			1,	0,	0},
	{"-Wall",		0,	&all_warnings,			1,	0,
			"-Wall", N_("Enable all compiler warning messages")},
	{"-Werror",		0,	&warnings_as_errors,	1,	0,
			"-Werror", N_("Treat compiler warning messages as errors")},
	{"-Wp,-MD,",	8,	0,						0,	mdOption, 0, 0},
	{"-Wp,-MMD,",	9,	0,						0,	mmdOption, 0, 0},
	{"-W",			2,	0,						0,	WOption, 0},
	{"-e",			2,	0,						0,	eOption,
			N_("-e<name>"),
			N_("Set the entry point to <name> (default is `main')")},
	{"-x",			2,	0,						0,	xOption,
			N_("-x <lang>"), N_("Compile all input files as language <lang>")},
	{"-dumpversion",0,	0,						0,	vOption, 0, 0},
	{"-d",			2,	0,						0,	dOption,
			"-d[M|N]", N_("Specifiy preprocessor dump modes")},
	{"-c",			0,	&compile_flag,			1,	0,
			"-c", N_("Compile and assemble, but do not link")},
	{"-S",			0,	&assemble_flag,			1,	0,
			"-S", N_("Compile only; do not assemble or link")},
	{"-E",			0,	&preprocess_flag,		1,	0,
			"-E", N_("Preprocess only; do not compile, assemble, or link")},
	{"-P",			0,	&no_preproc_lines_flag,	1,	0,
			"-P", N_("Don't use #line directives in preprocessor output")},
	{"-C",			0,	&preproc_comments_flag,	1,	0,
			"-C", N_("Include comments in preprocessor output")},
	{"-H",			0,	&preproc_show_headers,	1,	0,
			"-H", N_("Show headers as they are preprocessed")},
	{"-M",			0,	&dependency_level,		DEP_LEVEL_M, 0,
			"-M, -MD, -MG, -MM, -MMD", N_("Generate dependency information")},
	{"-MD",			0,	&dependency_level,		DEP_LEVEL_MD, 0, 0, 0},
	{"-MG",			0,	&dependency_gen_flag,	1,	0, 0, 0},
	{"-MM",			0,	&dependency_level,		DEP_LEVEL_MM, 0, 0, 0},
	{"-MMD",		0,	&dependency_level,		DEP_LEVEL_MMD, 0, 0, 0},
	{"-MF",			3,	0,						0,	mfOption, 0, 0},
	{"-g",			0,	&debug_flag,			1,	0,
			"-g", N_("Enable debug symbol information in the output")},
	{"-nostdinc",	0,	&nostdinc_flag,			1,	0,
			"-nostdinc", N_("Don't use standard include directories")},
	{"-nostdinc++",	0,	&nostdinc_cpp_flag,		1,	0,
			"-nostdinc++", N_("Don't use standard C++ include directories")},
	{"-nostdlib",	0,	&nostdlib_flag,			1,	0,
			"-nostdlib", N_("Don't use standard link directories")},
	{"-undef",		0,	&undef_flag,			1,	0,
			"-undef", N_("Undefine all pre-defined preprocessor symbols")},
	{"-shared",		0,	&shared_flag,			1,	0,
			"-shared", N_("Create a shared DLL as output")},
	{"-static",		0,	&static_flag,			1,	0,
			"-static", N_("Statically link against libraries")},
	{"-O",			0,	&optimize_flag,			1,	0,
			"-O, -O1, -O2, -O3",
			N_("Enable optimization at a particular level")},
	{"-O1",			0,	&optimize_flag,			1,	0, 0},
	{"-O2",			0,	&optimize_flag,			2,	0, 0},
	{"-O3",			0,	&optimize_flag,			3,	0, 0},
	{"-O0",			0,	0,						0,	O0Option,
			"-O0", N_("Disable optimization")},
	{"-w",			0,	&inhibit_warnings,		1,	0,
			"-w", N_("Suppress all compiler warning messages")},
	{"-v",			0,	&verbose_mode,			VERBOSE_FILENAMES,	0,
			"-v", N_("Print names of source files as they are compiled")},
	{"-vv",			0,	&verbose_mode,			VERBOSE_CMDLINES,	0,
			"-vv", N_("Print command-lines of tools as they are executed")},
	{"--version",0,	0,							0,	vOption,
			"--version", N_("output version information and exit")},
	{"--help",		0,	0,						0,	helpOption,
			"--help", N_("Display this information")},

	/* Options that exist for gcc compatibility, which don't do anything at
	   the moment.  They may (or may not) be added sometime in the future */
	{"-pipe",				0,	0,	0,	0,  0},
	{"-ansi",				0,	0,	0,	0,  0},
	{"-traditional",		0,	0,	0,	0,  0},
	{"-traditional-cpp",	0,	0,	0,	0,  0},
	{"-trigraphs",			0,	0,	0,	0,  0},
	{"-pedantic",			0,	0,	0,	0,  0},
	{"-pedantic-errors",	0,	0,	0,	0,  0},
	{"-g1",					0,	0,	0,	0,  0},
	{"-g2",					0,	0,	0,	0,  0},
	{"-g3",					0,	0,	0,	0,  0},
	{"-gcoff",				0,	0,	0,	0,  0},
	{"-gxcoff",				0,	0,	0,	0,  0},
	{"-gxcoff+",			0,	0,	0,	0,  0},
	{"-gdwarf",				0,	0,	0,	0,  0},
	{"-gstabs",				0,	0,	0,	0,  0},
	{"-ggdb",				0,	0,	0,	0,  0},
	{"-p",					0,	0,	0,	0,  0},
	{"-pg",					0,	0,	0,	0,  0},
	{"-save-temps",			0,	0,	0,	0,  0},
	{"-a",					0,	0,	0,	0,  0},
	{"-ax",					0,	0,	0,	0,  0},
	{"-b",					2,	0,	0,	0,  0},
	{"-V",					2,	0,	0,	0,  0},
	{"-nostartfiles",		0,	0,	0,	0,  0},
	{"-symbolic",			0,	0,	0,	0,  0},
	{"-Xlinker",			8,	0,	0,	0,  0},
	{"-u",					2,	0,	0,	0,  0},
	{"-B",					2,	0,	0,	0,  0},
};
#define	num_options	(sizeof(options) / sizeof(options[0]))

static char *out_of_memory_message = "virtual memory exhausted";

/* Imports from "cc_compat.c" */
int CCNeedsCompatParser(int argc, char *argv[]);
void CCParseWithCompatParser(int argc, char *argv[]);

void CCParseCommandLine(int argc, char *argv[], int mode, char *versname)
{
	int stdin_is_input;
	int opt;
	int nostdlib_mode = ((mode & CMDLINE_PARSE_PLUGIN_NOSTDLIB) != 0);
	int compat_mode = ((mode & CMDLINE_PARSE_COMPAT) != 0);
	char *optname;
	char *value;
	int outlen;
	mode &= ~(CMDLINE_PARSE_PLUGIN_NOSTDLIB | CMDLINE_PARSE_COMPAT);

	/* Get the translated out of memory message */
	out_of_memory_message = _("virtual memory exhausted");

	/* Save the program's name away for later error reporting */
	progname = argv[0];
	version_name = versname;

	/* Expand response file references in the command-line */
	ILCmdLineExpand(&argc, &argv);

	/* If the only option is "-v", then print the version and exit.
	   Otherwise, "-v" is interpreted as enabling "verbose mode" */
	if(argc == 2 && !strcmp(argv[1], "-v"))
	{
		vOption(0);
	}

	/* Set up the initial pre-defined symbols */
	if(mode == CMDLINE_PARSE_CSCC)
	{
		AddString(&pre_defined_symbols, &num_pre_defined_symbols, "__CSCC__");
	}

	/* Do we need to use the compatibility parser? */
	stdin_is_input = 0;
	if(compat_mode && CCNeedsCompatParser(argc, argv))
	{
		CCParseWithCompatParser(argc, argv);
		goto done_compat;
	}

	/* Scan the command-line and process the options */
	while(argc > 1)
	{
		if(argv[1][0] != '-' || argv[1][1] == '\0')
		{
			/* This is an input filename */
			AddString(&input_files, &num_input_files, argv[1]);
			if(!strcmp(argv[1], "-"))
			{
				stdin_is_input = 1;
			}
			++argv;
			--argc;
			continue;
		}
		else if(argv[1][1] == '-' && argv[1][2] == '\0')
		{
			/* This is "--", which is used as a separator in plugins,
			   but is ignored in "cscc" itself */
			++argv;
			--argc;
			if(mode != CMDLINE_PARSE_CSCC)
			{
				break;
			}
			continue;
		}
		if(mode != CMDLINE_PARSE_CSCC)
		{
			/* Process system include options that are specific to plugins */
			if(!strncmp(argv[1], "-J", 2) ||
			   !strncmp(argv[1], "-K", 2) ||
			   !strncmp(argv[1], "-N", 2) ||
			   !strncmp(argv[1], "-F", 2) ||
			   !strcmp(argv[1], "-W"))
			{
				optname = argv[1];
				if(argv[1][2] != '\0')
				{
					value = argv[1] + 2;
				}
				else
				{
					if(argc <= 2)
					{
						fprintf(stderr, _("%s: argument to `%s' is missing\n"),
								progname, argv[1]);
						exit(1);
					}
					--argc;
					++argv;
					value = argv[1];
				}
				if(!strncmp(optname, "-J", 2))
				{
					if(!nostdinc_flag &&
					   (mode == CMDLINE_PARSE_PLUGIN ||
					    mode == CMDLINE_PARSE_PLUGIN_CPP))
					{
						AddString(&include_dirs, &num_include_dirs, value);
					}
					--argc;
					++argv;
					continue;
				}
				else if(!strncmp(optname, "-K", 2))
				{
					if(!nostdinc_cpp_flag && mode == CMDLINE_PARSE_PLUGIN_CPP)
					{
						AddString(&include_dirs, &num_include_dirs, value);
					}
					--argc;
					++argv;
					continue;
				}
				else if(!strncmp(optname, "-N", 2))
				{
					if(!nostdlib_flag && !nostdlib_mode)
					{
						AddString(&link_dirs, &num_link_dirs, value);
					}
					--argc;
					++argv;
					continue;
				}
				else if(!strcmp(optname, "-W"))
				{
					AddString(&warning_flags, &num_warning_flags,
							  value);
					--argc;
					++argv;
					continue;
				}
			}
		}
		for(opt = 0; opt < num_options; ++opt)
		{
			if(options[opt].nameSize)
			{
				if(!strncmp(argv[1], options[opt].name, options[opt].nameSize))
				{
					if(argv[1][options[opt].nameSize] == '\0')
					{
						--argc;
						++argv;
						if(argc <= 1)
						{
							fprintf(stderr,
								    _("%s: argument to `%s' is missing\n"),
									progname, options[opt].name);
							exit(1);
						}
						if(options[opt].func)
						{
							(*(options[opt].func))(argv[1]);
						}
					}
					else if(options[opt].func)
					{
						(*(options[opt].func))(argv[1] + options[opt].nameSize);
					}
					break;
				}
			}
			else if(!strcmp(argv[1], options[opt].name))
			{
				if(options[opt].flag)
				{
					*(options[opt].flag) = options[opt].value;
				}
				else if(options[opt].func)
				{
					(*(options[opt].func))(0);
				}
				break;
			}
		}
		if(opt >= num_options)
		{
			fprintf(stderr, _("%s: unrecognized option `%s'\n"),
				    progname, argv[1]);
		}
		--argc;
		++argv;
	}

	/* Collect up the remaining source input files after "--" */
	while(argc > 1)
	{
		AddString(&input_files, &num_input_files, argv[1]);
		if(!strcmp(argv[1], "-"))
		{
			stdin_is_input = 1;
		}
		--argc;
		++argv;
	}

	/* Add either "DEBUG" or "RELEASE" to the pre-defined symbol set */
done_compat:
	if(mode == CMDLINE_PARSE_CSCC)
	{
		if(debug_flag)
		{
			AddString(&pre_defined_symbols, &num_pre_defined_symbols,
					  "DEBUG");
		}
		else
		{
			AddString(&pre_defined_symbols, &num_pre_defined_symbols,
					  "RELEASE");
		}
	}

	/* Turn on -E if -M or -MM is supplied */
	if(dependency_level == DEP_LEVEL_M || dependency_level == DEP_LEVEL_MM)
	{
		preprocess_flag = 1;
	}

	/* Do we have any input files? */
	if(CCStringListGetValue(extension_flags, num_extension_flags,
							"resources"))
	{
		resources_only = (!compile_flag && !assemble_flag &&
						  !preprocess_flag && !num_input_files);
	}
	if(!num_input_files && !resources_only)
	{
		fprintf(stderr, _("%s: No input files\n"), progname);
		exit(1);
	}

	/* Verify the combinations of "-c", "-S", "-E", etc */
	if(preprocess_flag)
	{
		compile_flag = 0;
		assemble_flag = 0;
	}
	else if(assemble_flag)
	{
		compile_flag = 0;
	}
	if(mode != CMDLINE_PARSE_CSCC && !preprocess_flag)
	{
		/* We are always assembling if we are a plug-in */
		compile_flag = 0;
		assemble_flag = 1;
	}
	if(stdin_is_input && !preprocess_flag)
	{
		fprintf(stderr,
				_("%s: -E required when input is from standard input\n"),
				progname);
		exit(1);
	}
	if(mode == CMDLINE_PARSE_CSCC)
	{
		if((compile_flag || assemble_flag) && output_filename != 0 &&
		   num_input_files > 1)
		{
			fprintf(stderr, _("%s: cannot specify -o with -c or -S and "
							  "multiple compilations\n"), progname);
			exit(1);
		}
	}
	if(!compile_flag && !assemble_flag && !preprocess_flag)
	{
		if(mode != CMDLINE_PARSE_CSCC)
		{
			assemble_flag = 1;
		}
		else
		{
			executable_flag = 1;
		}
	}

	/* Clear the pre-defined symbols if "-undef" was set */
	if(undef_flag)
	{
		if(pre_defined_symbols)
		{
			ILFree(pre_defined_symbols);
		}
		pre_defined_symbols = 0;
		num_pre_defined_symbols = 0;
	}

	/* Force "-fstdlib-name=corlib" if compiling "corlib.dll" or if
	   "-lcorlib" is supplied on the command-line.  Needed for
	   interoperability with Mono build scripts */
	outlen = (output_filename ? strlen(output_filename) : 0);
	if(output_filename != 0 && outlen >= 10 &&
	   !ILStrICmp(output_filename + outlen - 10, "corlib.dll") &&
	   (outlen == 10 || output_filename[outlen - 11] == '/' ||
	    output_filename[outlen - 11] == '\\'))
	{
		AddString(&extension_flags, &num_extension_flags,
				  "stdlib-name=corlib");
	}
	else if(CCStringListContains(libraries, num_libraries, "corlib"))
	{
		AddString(&extension_flags, &num_extension_flags,
				  "stdlib-name=corlib");
	}
}

/*
 * Process a --help option.
 */
static void helpOption(char *arg)
{
	int opt;
	char *msg;
	int size, maxSize;

	/* Print the help header */
	printf(_("Usage: %s [options] file ...\n"), progname);
	printf(_("Options:\n"));

	/* Scan the option table to determine the width of the tab column */
	maxSize = 0;
	for(opt = 0; opt < num_options; ++opt)
	{
		msg = _(options[opt].helpName);
		if(!msg)
		{
			continue;
		}
		size = strlen(msg);
		if(size > maxSize)
		{
			maxSize = size;
		}
	}

	/* Dump the help messages in the option table */
	for(opt = 0; opt < num_options; ++opt)
	{
		msg = options[opt].helpName;
		if(!msg)
		{
			continue;
		}
		putc(' ', stdout);
		putc(' ', stdout);
		msg = _(msg);
		size = 0;
		while(*msg != '\0')
		{
			putc(*msg, stdout);
			++size;
			++msg;
		}
		while(size < maxSize)
		{
			putc(' ', stdout);
			++size;
		}
		putc(' ', stdout);
		putc(' ', stdout);
		msg = _(options[opt].helpMsg);
		fputs(msg, stdout);
		putc('\n', stdout);
	}

	/* Exit from the program */
	exit(0);
}

void CCOutOfMemory(void)
{
	fputs(progname, stderr);
	fputs(": ", stderr);
	fputs(out_of_memory_message, stderr);
	putc('\n', stderr);
	exit(1);
}

int CCStringListContains(char **list, int num, const char *value)
{
	while(num > 0)
	{
		if(!strcmp(list[0], value))
		{
			return 1;
		}
		++list;
		--num;
	}
	return 0;
}

int CCStringListContainsInv(char **list, int num, const char *value)
{
	while(num > 0)
	{
		if(list[0][0] == 'n' && list[0][1] == 'o' && list[0][2] == '-' &&
		   !strcmp(list[0] + 3, value))
		{
			return 1;
		}
		++list;
		--num;
	}
	return 0;
}

char *CCStringListGetValue(char **list, int num, const char *name)
{
	int namelen = strlen(name);
	while(num > 0)
	{
		if(!strncmp(list[0], name, namelen) &&
		   list[0][namelen] == '=' &&
		   list[0][namelen + 1] != '\0')
		{
			return list[0] + namelen + 1;
		}
		++list;
		--num;
	}
	return 0;
}

void CCAddLinkFile(char *filename, int isTemp)
{
	int *newlist;
	AddString(&files_to_link, &num_files_to_link, filename);
	newlist = (int *)ILRealloc(files_to_link_temp,
							   sizeof(int) * num_files_to_link);
	if(!newlist)
	{
		CCOutOfMemory();
	}
	files_to_link_temp = newlist;
	files_to_link_temp[num_files_to_link - 1] = isTemp;
}

void CCStringListAdd(char ***list, int *num, char *str)
{
	AddString(list, num, str);
}

void CCStringListAddOption(char ***list, int *num, char *option, char *str)
{
	char * concat = malloc(strlen(option) + strlen(str) + 1);
	strcpy(concat, option);
	strcat(concat, str);
	AddString(list, num, concat);
}

#ifdef	__cplusplus
};
#endif
