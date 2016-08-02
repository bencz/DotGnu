/*
 * cc_compat.c - Command-line option processing for compatibility
 *               with other C# compilers.
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

#include <stdio.h>
#include "il_system.h"
#include "il_utils.h"
#include "il_sysio.h"
#include "cc_options.h"
#include "cc_intl.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Temporary options that we'll turn into real cscc options later.
 */
static int checked_value = 0;
static int unsafe_value = 0;
static int noconfig_value = 0;
static int parse_value = 0;
static int latin1_charset = 1;
static int build_dll = 0;
static int gui_subsystem = 0;

/*
 * Process an "--about" option.
 */
static void aboutOption(char *arg)
{
	fprintf(stderr, "%s version " VERSION "\n", version_name);
	exit(0);
}

/*
 * Process a "/help" option.
 */
static void helpOption(char *arg);

/*
 * Process a "/codepage" option.
 */
static void codepageOption(char *arg)
{
	if(!ILStrICmp(arg, "1252") || !ILStrICmp(arg, "28591") ||
	   !ILStrICmp(arg, "reset"))
	{
		latin1_charset = 1;
	}
	else if(!ILStrICmp(arg, "utf8"))
	{
		latin1_charset = 0;
	}
}

/*
 * Add a length-terminated string to a list.
 */
static void AddNString(char ***list, int *num, char *value, int len)
{
	value = ILDupNString(value, len);
	if(!value)
	{
		CCOutOfMemory();
	}
	CCStringListAdd(list, num, value);
}

/*
 * Add a comma-separated list of strings to a list.
 */
static void AddStrings(char ***list, int *num, char *value)
{
	int len;
	while(*value != '\0')
	{
		if(*value == ',' || *value == ';')
		{
			++value;
			continue;
		}
		len = 1;
		while(value[len] != '\0' && value[len] != ',' &&
		      value[len] != ';')
		{
			++len;
		}
		AddNString(list, num, value, len);
		value += len;
	}
}

/*
 * Process a "/define" option.
 */
static void defineOption(char *arg)
{
	AddStrings(&user_defined_symbols, &num_user_defined_symbols, arg);
}

/*
 * Process a "/lib" option.
 */
static void libOption(char *arg)
{
	AddStrings(&link_dirs, &num_link_dirs, arg);
}

/*
 * Process a "-L" option.
 */
static void singleLibOption(char *arg)
{
	CCStringListAdd(&link_dirs, &num_link_dirs, arg);
}

/*
 * Process a "-I" option.
 */
static void includeOption(char *arg)
{
	CCStringListAdd(&include_dirs, &num_include_dirs, arg);
}

/*
 * Process a "-f" option.
 */
static void extensionOption(char *arg)
{
	CCStringListAdd(&extension_flags, &num_extension_flags, arg);
}

/*
 * Process a "/main" option.
 */
static void mainOption(char *arg)
{
	entry_point = arg;
}

/*
 * Process a "/out" option.
 */
static void outOption(char *arg)
{
	output_filename = arg;
}


/* 
 * recurse and add all files which match the current pattern 
 */
static void recurseAndAddFiles(char *pathname, char * pattern)
{
	/* TODO: use regexp for processing */
	ILDir *dir = NULL;
	ILDirEnt *entry = NULL;
	dir = ILOpenDir(pathname);
	
	if(dir == NULL)
	{
		fprintf(stderr, "%s: directory does not exist `%s'\n", progname, pathname);
		return ;
	}

	while((entry = ILReadDir(dir)) != NULL)
	{
		const char * filename = ILDirEntName(entry);
		char * fullpath = NULL;
		
		if(!strcmp(filename,"..") || !strcmp(filename,"."))
		{
			continue;
		}
		
		fullpath = (char *)ILMalloc(strlen(pathname) +
						    (filename ? strlen(filename) + 1 : 0) + 1);
		if(!fullpath)
		{
			// Out of memory
		}
		strcpy(fullpath, pathname);
		if(filename)
		{
			if(fullpath[strlen(fullpath)-1]!='/')
			{
				strcat(fullpath,"/");
			}
			strcat(fullpath, filename);
		}

		if(ILDirEntType(entry)== ILFileType_DIR)
		{
			// goes recursive
			recurseAndAddFiles(fullpath, pattern);
		}
		else
		{
			// pattern matching - should use regex 
			// this works for *.cs anyway could be faster 
			// and less memory.... the point is , it's easy :)
			int len1 = strlen(pattern);
			int len2 = strlen(filename);
			while((len1) && (len2))
			{
				len1--;
				len2--;
				if(pattern[len1] == '*')
				{
					// matches pattern of *.cs :)
					AddNString(&input_files, &num_input_files, fullpath, strlen(fullpath));
					break;
				}
				else if(pattern[len1] != filename[len2])
				{
					break;
				}
			}
		}

		ILFree(fullpath);
	}
	ILCloseDir(dir);
}

/*
 * Process a "/recurse" option.
 */
static void recurseOption(char *arg)
{
	char * dirPrefix = NULL;
	int i=0;
	if((arg==NULL) || (arg[0])=='\0')
	{
		return;
	}

	dirPrefix = ILCalloc(strlen(arg)+1,sizeof(char));

	while(((*arg)!='\0') && ((*arg)!='*'))
	{
		dirPrefix[i++]=*arg;
		arg++;
	}

	if(*dirPrefix == '\0')
	{
		// /recurse:*.cs starts from current dir
		dirPrefix[0]='.';
	}

	recurseAndAddFiles(dirPrefix,arg);

//cleanup:
	if(dirPrefix)
	{
		ILFree(dirPrefix);
	}
}

/*
 * Process a "/reference" option.
 */
static void referenceOption(char *arg)
{
	char **refs = 0;
	int num_refs = 0;
	int posn, len;
	AddStrings(&refs, &num_refs, arg);
	for(posn = 0; posn < num_refs; ++posn)
	{
		len = strlen(refs[posn]);
		while(len > 0 && refs[posn][len - 1] != '/' &&
		      refs[posn][len - 1] != '\\' &&
			  refs[posn][len - 1] != ':')
		{
			--len;
		}
		CCStringListAdd(&libraries, &num_libraries, refs[posn] + len);
		if(len > 0)
		{
			if(len > 1 && refs[posn][len - 1] != ':' &&
			   refs[posn][len - 2] != '/' &&
			   refs[posn][len - 2] != '\\' &&
			   refs[posn][len - 2] != ':')
			{
				--len;
			}
			AddNString(&link_dirs, &num_link_dirs, refs[posn], len);
		}
	}
}

/*
 * Process a "/resource" option.
 */
static void resourceOption(char *arg)
{
	char **refs = 0;
	int num_refs = 0;
	int posn;
	char *combined;
	AddStrings(&refs, &num_refs, arg);
	for(posn = 0; posn < num_refs; ++posn)
	{
		combined = (char *)ILMalloc(strlen(refs[posn]) + 11);
		if(!combined)
		{
			CCOutOfMemory();
		}
		strcpy(combined, "resources=");
		strcat(combined, refs[posn]);
		CCStringListAdd(&extension_flags, &num_extension_flags, combined);
	}
}

/*
 * Process a "/culture" option.
 */
static void cultureOption(char *arg)
{
	CCStringListAddOption(&extension_flags, &num_extension_flags,
						  "culture=", arg);
}

/*
 * Process a "/target" option.
 */
static void targetOption(char *arg)
{
	if(!ILStrICmp(arg, "exe"))
	{
		build_dll = 0;
		gui_subsystem = 0;
	}
	else if(!ILStrICmp(arg, "winexe"))
	{
		build_dll = 0;
		gui_subsystem = 1;
	}
	else if(!ILStrICmp(arg, "library") || !ILStrICmp(arg, "module"))
	{
		build_dll = 1;
		gui_subsystem = 0;
	}
	else
	{
		fprintf(stderr, "%s: unknown target type `%s'\n", progname, arg);
	}
}

/*
 * Process a "/warn" option.
 */
static void warnLevelOption(char *arg)
{
	if(!strcmp(arg, "4"))
	{
		all_warnings = 1;
	}
}

/*
 * Table of compatibility command-line options.
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
static CmdLineOpt const compatOptions[] = {
	/* Normal option names */
	{"--about",		0,	0,						0,	aboutOption,
			N_("--about"), N_("Show version information")},
	{"/help",		0,	0,						0,	helpOption,
			N_("/help"), N_("Show help information")},
	{"/checked",	0,	&checked_value,			1,	0,
			N_("/checked[+|-]"),
			N_("Specify the default overflow check state")},
	{"/checked+",	0,	&checked_value,			1,	0, 0, 0},
	{"/checked-",	0,	&checked_value,			0,	0, 0, 0},
	{"/codepage",   9,  0,						0,	codepageOption,
			N_("/codepage:<nnn>"),
			N_("Specify the source character set code page")},
	{"/define",     7,  0,						0,	defineOption,
			N_("/define:<symbols>"),
			N_("Define pre-processor symbols")},
	{"/d",     		2,  0,						0,	defineOption, 0, 0},
	{"/debug",		0,	&debug_flag,			1,	0,
			N_("/debug[+|-]"),
			N_("Specify the debug information mode")},
	{"/debug+",		0,	&debug_flag,			1,	0, 0, 0},
	{"/debug:full", 0,  &debug_flag,			1,	0, 0, 0},
	{"/debug:pdbonly", 0, &debug_flag,			1,	0, 0, 0},
	{"/debug-",		0,	&debug_flag,			0,	0, 0, 0},
	{"/lib",		4,	0,						0,	libOption,
			N_("/lib:<dirs>"),
			N_("Specify library search directories")},
	{"/main",		5,	0,						0,	mainOption,
			N_("/main:XXX"),
			N_("Specify the program entry point")},
	{"/m",			2,	0,						0,	mainOption, 0, 0},
	{"/noconfig",	0,	&noconfig_value,		1,	0,
			N_("/noconfig[+|-]"),
			N_("Enable or disable configured standard libraries")},
	{"/noconfig+",	0,	&noconfig_value,		1,	0, 0, 0},
	{"/noconfig-",	0,	&noconfig_value,		0,	0, 0, 0},
	{"/nostdlib",	0,	&nostdlib_flag,			1,	0,
			N_("/nostdlib[+|-]"),
			N_("Enable or disable the standard core library")},
	{"/nostdlib+",	0,	&nostdlib_flag,			1,	0, 0, 0},
	{"/nostdlib-",	0,	&nostdlib_flag,			0,	0, 0, 0},
	{"/optimize",	0,	&optimize_flag,			2,	0,
			N_("/optimize[+|-]"),
			N_("Enable or disable optimizations")},
	{"/optimize+",	0,	&optimize_flag,			2,	0, 0, 0},
	{"/optimize-",	0,	&optimize_flag,			0,	0, 0, 0},
	{"/out",		4,	0,						0,	outOption,
			N_("/out:<file>"),
			N_("Specify the output file")},
	{"--parse",		0,	&parse_value,			1,	0,
			N_("--parse"), N_("Parse the source files only")},
	{"/recurse",	8,	0,						0,	recurseOption,
			N_("/recurse:<spec>"),
			N_("Recurse through sub-directories to find sources")},
	{"/reference",	10,	0,						0,	referenceOption,
			N_("/reference:<files>"),
			N_("Specify referenced library assemblies")},
	{"/r",			2,	0,						0,	referenceOption, 0, 0},
	{"/resource",	9,	0,						0,	resourceOption,
			N_("/resource:<file>"),
			N_("Specify an embedded resource file")},
	{"/res",		4,	0,						0,	resourceOption, 0, 0},
	{"/culture",	8,	0,						0,	cultureOption,
			N_("/culture:<name>"),
			N_("Specify the resource culture name")},
	{"/c",			2,	0,						0,	cultureOption, 0, 0},
	{"/target",		7,	0,						0,	targetOption,
			N_("/target:<type>"),
			N_("Specify target type")},
	{"/t",			2,	0,						0,	targetOption, 0, 0},
	{"--tokenize",	0,	&parse_value,			1,	0, 0, 0},
	{"/unsafe",		0,	&unsafe_value,			1,	0,
			N_("/unsafe[+|-]"),
			N_("Enable or disable unsafe language constructs")},
	{"/unsafe+",	0,	&unsafe_value,			1,	0, 0, 0},
	{"/unsafe-",	0,	&unsafe_value,			0,	0, 0, 0},
	{"/warnaserror", 0,	&warnings_as_errors,	1,	0,
			N_("/warnaserror[+|-]"),
			N_("Specify whether warnings should be treated as errors")},
	{"/warnaserror+", 0, &warnings_as_errors,	1,	0, 0, 0},
	{"/warnaserror-", 0, &warnings_as_errors,	0,	0, 0, 0},
	{"/v",			0,	&verbose_mode,			VERBOSE_FILENAMES, 0,
			N_("/v"), N_("Enable verbose mode")},

	/* Convenient aliases */
	{"/?",			0,	0,						0,	helpOption, 0, 0},
	{"/h",			0,	0,						0,	helpOption, 0, 0},
	{"-help",		0,	0,						0,	helpOption, 0, 0},
	{"--help",		-1,	0,						0,	helpOption, 0, 0},
	{"-checked",	0,	&checked_value,			1,	0, 0, 0},
	{"-checked+",	0,	&checked_value,			1,	0, 0, 0},
	{"-checked-",	0,	&checked_value,			0,	0, 0, 0},
	{"--checked",	0,	&checked_value,			1,	0, 0, 0},
	{"-codepage",   9,  0,						0,	codepageOption, 0, 0},
	{"-define",     7,  0,						0,	defineOption, 0, 0},
	{"-d",     		2,  0,						0,	defineOption, 0, 0},
	{"-debug",		0,	&debug_flag,			1,	0, 0, 0},
	{"-debug+",		0,	&debug_flag,			1,	0, 0, 0},
	{"-debug-",		0,	&debug_flag,			0,	0, 0, 0},
	{"--debug",		0,	&debug_flag,			1,	0, 0, 0},
	{"-f",			-3,	0,						0,	extensionOption, 0, 0},
	{"-g",			-1,	&debug_flag,			1,	0, 0, 0},
	{"-I",			-3,	0,						0,	includeOption, 0, 0},
	{"-lib",		4,	0,						0,	libOption, 0, 0},
	{"-L",			-3,	0,						0,	singleLibOption, 0, 0},
	{"-main",		5,	0,						0,	mainOption, 0, 0},
	{"-m",			-3,	0,						0,	mainOption, 0, 0},
	{"-noconfig",	0,	&noconfig_value,		1,	0, 0, 0},
	{"-noconfig+",	0,	&noconfig_value,		1,	0, 0, 0},
	{"-noconfig-",	0,	&noconfig_value,		0,	0, 0, 0},
	{"-nostdlib",	-1,	&nostdlib_flag,			1,	0, 0, 0},
	{"-nostdlib+",	0,	&nostdlib_flag,			1,	0, 0, 0},
	{"-nostdlib-",	0,	&nostdlib_flag,			0,	0, 0, 0},
	{"--nostdlib",	0,	&nostdlib_flag,			1,	0, 0, 0},
	{"-optimize",	0,	&optimize_flag,			2,	0, 0, 0},
	{"-optimize+",	0,	&optimize_flag,			2,	0, 0, 0},
	{"-optimize-",	0,	&optimize_flag,			0,	0, 0, 0},
	{"-out",		4,	0,						0,	outOption, 0, 0},
	{"-o",			-3,	0,						0,	outOption, 0, 0},
	{"--output",	8,	0,						0,	outOption, 0, 0},
	{"-recurse",	8,	0,						0,	recurseOption, 0, 0},
	{"-reference",	10,	0,						0,	referenceOption, 0, 0},
	{"-r",			2,	0,						0,	referenceOption, 0, 0},
	{"-resource",	9,	0,						0,	resourceOption, 0, 0},
	{"-res",		4,	0,						0,	resourceOption, 0, 0},
	{"--resource",	10,	0,						0,	resourceOption, 0, 0},
	{"--res",		5,	0,						0,	resourceOption, 0, 0},
	{"-culture",	8,	0,						0,	cultureOption, 0, 0},
	{"--culture",	9,	0,						0,	cultureOption, 0, 0},
	{"-target",		7,	0,						0,	targetOption, 0, 0},
	{"-t",			2,	0,						0,	targetOption, 0, 0},
	{"-unsafe",		0,	&unsafe_value,			1,	0, 0, 0},
	{"-unsafe+",	0,	&unsafe_value,			1,	0, 0, 0},
	{"-unsafe-",	0,	&unsafe_value,			0,	0, 0, 0},
	{"--unsafe",	0,	&unsafe_value,			1,	0, 0, 0},
	{"-warnaserror", 0,	&warnings_as_errors,	1,	0, 0, 0},
	{"-warnaserror+", 0, &warnings_as_errors,	1,	0, 0, 0},
	{"-warnaserror-", 0, &warnings_as_errors,	0,	0, 0, 0},
	{"/warn", 		5,	0,						0,	warnLevelOption, 0, 0},
	{"--wlevel", 	8, 0,						0,	warnLevelOption, 0, 0},
	{"--werror", 	0,	&warnings_as_errors,	1,	0, 0, 0},
	{"-v",			-1,	&verbose_mode,			VERBOSE_FILENAMES, 0, 0, 0},

	/* Ignored options */
	{"--fatal",		0,	0,						0,	0, 0, 0},
	{"/fullpaths",	0,	0,						0,	0, 0, 0},
	{"/nowarn",		7,	0,						0,	0, 0, 0},
	{"-nowarn",		7,	0,						0,	0, 0, 0},
	{"--nowarn",	8,	0,						0,	0, 0, 0},
	{"/linkresource", 13, 0,					0,	0, 0, 0},
	{"-linkresource", 13, 0,					0,	0, 0, 0},
	{"--linkresource", 14, 0,					0,	0, 0, 0},
	{"/linkres", 	8,	0,						0,	0, 0, 0},
	{"-linkres", 	8,	0,						0,	0, 0, 0},
	{"--linkres", 	9,	0,						0,	0, 0, 0},
	{"/nologo",		0,	0,						0,	0, 0, 0},
	{"-nologo",		0,	0,						0,	0, 0, 0},
	{"--mcs-debug", 11, 0,						0,	0, 0, 0},
	{"--timestamp", 0, 0,						0,	0, 0, 0},
	{"--debug-args", 12, 0,						0,	0, 0, 0},
	{"--expect-error", 14, 0,					0,	0, 0, 0},
	{"--stacktrace", 0, 0,						0,	0, 0, 0},
	{"-incremental", 0,	0,						0,	0, 0, 0},
	{"-incremental+", 0, 0,						0,	0, 0, 0},
	{"-incremental-", 0, 0,						0,	0, 0, 0},
	{0,				0,	0,						0,	0, 0, 0},
};
#define	num_options	((sizeof(compatOptions) / sizeof(compatOptions[0])) - 1)

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
		msg = _(compatOptions[opt].helpName);
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
		msg = compatOptions[opt].helpName;
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
		msg = _(compatOptions[opt].helpMsg);
		fputs(msg, stdout);
		putc('\n', stdout);
	}

	/* Exit from the program */
	exit(0);
}

/*
 * Determine if a command-line looks like something that should
 * be processed using the compatibility parser.
 */
int CCNeedsCompatParser(int argc, char *argv[])
{
	int len, posn;
	const CmdLineOpt *opt;

	/* Check the program name to see if it is "csc" or "mcs".
	   This may happen if "cscc" is symlink'ed */
	len = strlen(argv[0]);
	while(len > 0 && argv[0][len - 1] != '/' && argv[0][len - 1] != '\\')
	{
		--len;
	}
	if(!ILStrICmp(argv[0] + len, "csc") ||
	   !ILStrICmp(argv[0] + len, "csc.exe") ||
	   !ILStrICmp(argv[0] + len, "mcs") ||
	   !ILStrICmp(argv[0] + len, "mcs.exe"))
	{
		return 1;
	}

	/* Look for options that may indicate compatibility behaviour */
	for(posn = 1; posn < argc; ++posn)
	{
		/* Skip the argument if it doesn't begin with '-' or '/' */
		if(argv[posn][0] != '-' && argv[posn][0] != '/')
		{
			continue;
		}

		/* Search for the option in "compatOptions" */
		opt = compatOptions;
		while(opt->name)
		{
			if(opt->nameSize < 0)
			{
				/* Overlaps with a cscc option: use the cscc version */
				++opt;
				continue;
			}
			if(opt->nameSize)
			{
				if(!ILStrNICmp(argv[posn], opt->name, opt->nameSize) &&
				   (argv[posn][opt->nameSize] == ':' ||
				    argv[posn][opt->nameSize] == '=' ||
				    argv[posn][opt->nameSize] == '\0'))
				{
					return 1;
				}
			}
			else if(!ILStrICmp(argv[posn], opt->name))
			{
				return 1;
			}
			++opt;
		}
	}

	/* If we get here, then we want the default command-line behaviour */
	return 0;
}

/*
 * Invoke the handler for an option.
 */
static void InvokeOption(const CmdLineOpt *opt, char *arg)
{
	if(opt->func)
	{
		(*(opt->func))(arg);
	}
	else if(opt->flag)
	{
		*(opt->flag) = opt->value;
	}
}

/*
 * Determine if a string starting with '/' looks like an absolute
 * Unix-style pathname.
 */
static int LooksLikeAbsolutePath(const char *str)
{
	++str;
	while(*str != '\0')
	{
		if(*str == '/')
		{
			return 1;
		}
		else if(*str == ':' || *str == '=')
		{
			break;
		}
		++str;
	}
	return 0;
}

/*
 * Parse the command-line options with the compatibility parser.
 */
void CCParseWithCompatParser(int argc, char *argv[])
{
	int posn, size;
	const CmdLineOpt *opt;

	/* Look for options that may indicate compatibility behaviour */
	for(posn = 1; posn < argc; ++posn)
	{
		/* If the argument doesn't begin with '-' or '/', then it
		   is a source input file */
		if(argv[posn][0] != '-' && argv[posn][0] != '/')
		{
			CCStringListAdd(&input_files, &num_input_files, argv[posn]);
			continue;
		}
		if(argv[posn][0] == '/' && LooksLikeAbsolutePath(argv[posn]))
		{
			CCStringListAdd(&input_files, &num_input_files, argv[posn]);
			continue;
		}

		/* Search for the option in "compatOptions" */
		opt = compatOptions;
		while(opt->name)
		{
			size = opt->nameSize;
			if(size < 0)
			{
				size = (-size) - 1;
			}
			if(size)
			{
				if(!ILStrNICmp(argv[posn], opt->name, size) &&
				   (argv[posn][size] == ':' || argv[posn][size] == '='))
				{
					/* This is an option with arguments */
					InvokeOption(opt, argv[posn] + size + 1);
					break;
				}
				else if(!ILStrNICmp(argv[posn], opt->name, size) &&
				        argv[posn][size] != '\0' && opt->nameSize < 0)
				{
					/* Option with arguments, cscc-style */
					InvokeOption(opt, argv[posn] + size);
					break;
				}
				else if(!ILStrICmp(argv[posn], opt->name))
				{
					/* Option with a value in the next argument */
					if((posn + 1) < argc)
					{
						++posn;
						InvokeOption(opt, argv[posn]);
						break;
					}
				}
			}
			else if(!ILStrICmp(argv[posn], opt->name))
			{
				/* This is a stand-alone option */
				InvokeOption(opt, 0);
				break;
			}
			++opt;
		}
		if(!(opt->name))
		{
			fprintf(stderr, _("%s: unrecognized option `%s'\n"),
				    progname, argv[posn]);
		}
	}

	/* Add extra options that don't fit the standard mould */
	if(checked_value)
	{
		CCStringListAdd(&extension_flags, &num_extension_flags, "checked");
	}
	if(unsafe_value)
	{
		CCStringListAdd(&extension_flags, &num_extension_flags, "unsafe");
	}
	if(!noconfig_value)
	{
		CCStringListAdd(&libraries, &num_libraries, "System.Xml");
		CCStringListAdd(&libraries, &num_libraries, "System");
	}
	if(parse_value)
	{
		CCStringListAdd(&extension_flags, &num_extension_flags,
						"syntax-check");
	}
	if(latin1_charset)
	{
		CCStringListAdd(&extension_flags, &num_extension_flags,
					    "latin1-charset");
	}
	if(build_dll)
	{
		shared_flag = 1;
	}
	if(gui_subsystem)
	{
		CCStringListAdd(&extension_flags, &num_extension_flags,
					    "gui-subsystem");
	}
}

#ifdef	__cplusplus
};
#endif
