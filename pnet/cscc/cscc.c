/*
 * cscc.c - Front-end that wraps around compiler language plug-ins.
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

/*

Environment variables:

	CSCC_INCLUDE_PATH		Where to look for included system files.
	CSCC_INCLUDE_CPP_PATH	Where to look for included C++ system files.
	CSCC_LIB_PATH			Where to look for link libraries.
	CSCC_PLUGINS_PATH		Where to look for language plug-ins.

*/

#include <stdio.h>
#include "il_system.h"
#include "il_utils.h"
#include "il_linker.h"
#include "common/cc_options.h"
#include "common/cc_intl.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#ifndef IL_WIN32_NATIVE

/*
 * The default system include path.  We look in "lib" first,
 * just in case there is a pre-compiled "dll" for a package.
 */
#define	INCLUDE_PATH	\
			"/usr/local/lib/cscc/lib:" \
			"/usr/local/share/cscc/include:" \
			"/usr/lib/cscc/lib:" \
			"/usr/share/cscc/include"

/*
 * The default system C++ include path.
 */
#define	INCLUDE_CPP_PATH	\
			"/usr/local/share/cscc/include/c++:" \
			"/usr/share/cscc/include/c++"

/*
 * The default system library link path.
 */
#define	LIB_PATH	\
			"/usr/local/lib/cscc/lib:" \
			"/usr/lib/cscc/lib"

/*
 * The default plugins path.
 */
#define	PLUGINS_PATH	\
			"/usr/local/lib/cscc/plugins:" \
			"/usr/lib/cscc/plugins"

#else

/*
 * Disable the Unix-like paths under native Win32.
 */
#define	INCLUDE_PATH		0
#define	INCLUDE_CPP_PATH	0
#define	LIB_PATH			0
#define	PLUGINS_PATH		0

#endif

/*
 * File processing types.
 */
#define	FILEPROC_TYPE_BINARY		0	/* Binary for linking */
#define	FILEPROC_TYPE_IL			1	/* Process with the assembler */
#define	FILEPROC_TYPE_JL			2	/* Process with the assembler (JVM) */
#define	FILEPROC_TYPE_SINGLE		3	/* Process with single-file plugin */
#define	FILEPROC_TYPE_MULTIPLE		4	/* Process with multiple-file plugin */
#define	FILEPROC_TYPE_DONE			5	/* Already processed */
#define	FILEPROC_TYPE_UNKNOWN		6	/* Unknown format: can't process */

/*
 * Global variables.
 */
static char *csharp_compiler = 0;
static char **plugin_list;
static int *file_proc_types;

/*
 * Forward declarations.
 */
static void ParseCommandLine(int argc, char *argv[]);
static char *FindLanguagePlugin(char *name, char *ext);
static int CompareExtensions(const char *ext1, const char *ext2);
static int IsSinglePlugin(const char *filename);
static int ProcessWithAssembler(const char *filename, int jvmMode);
static int ProcessWithPlugin(const char *filename, char *plugin,
							 int filenum, int isMultiple);
static int LinkExecutable(void);

int main(int argc, char *argv[])
{
	int filenum;
	char *filename;
	int len;
	char *language;
	char *extension;
	char *plugin;
	int status = 0;
	int newstatus;

	/* Parse the command-line options */
	ParseCommandLine(argc, argv);

	/* Find the language plugins to be used for each of the input files */
	plugin_list = (char **)ILCalloc(num_input_files, sizeof(char *));
	file_proc_types = (int *)ILCalloc(num_input_files, sizeof(int));
	if(!plugin_list || !file_proc_types)
	{
		CCOutOfMemory();
	}
	for(filenum = 0; filenum < num_input_files; ++filenum)
	{
		/* Get the filename and determine which language it is expressed in */
		filename = input_files[filenum];
		if(!strcmp(filename, "-"))
		{
			/* Process stdin using the default language */
			language = prog_language_name;
			extension = 0;
		}
		else
		{
			/* Determine how to process the file based on its extension */
			len = strlen(filename);
			while(len > 0 && filename[len - 1] != '/' &&
			      filename[len - 1] != '\\' && filename[len - 1] != ':' &&
				  filename[len - 1] != '.')
			{
				--len;
			}
			if(len > 0 && filename[len - 1] == '.')
			{
				/* Check for special extensions */
				if(CompareExtensions(filename + len, "o") ||
				   CompareExtensions(filename + len, "obj") ||
				   CompareExtensions(filename + len, "dll") ||
				   CompareExtensions(filename + len, "exe") ||
				   CompareExtensions(filename + len, "a"))
				{
					/* This is a binary file to be passed to the linker */
					language = 0;
					extension = 0;
				}
				else if(prog_language != PROG_LANG_DEFAULT)
				{
					/* Use the command-line specified language always */
					language = prog_language_name;
					extension = 0;
				}
				else if(CompareExtensions(filename + len, "il") ||
						CompareExtensions(filename + len, "s") ||
						CompareExtensions(filename + len, "S"))
				{
					/* This is an assembly file */
					language = "il";
					extension = 0;
				}
				else if(CompareExtensions(filename + len, "jl"))
				{
					/* This is an assembly file with JVM bytecode */
					language = "jl";
					extension = 0;
				}
				else if(CompareExtensions(filename + len, "cs"))
				{
					/* This is a C# source file */
					language = "cs";
					extension = "cs";
				}
				else if(CompareExtensions(filename + len, "vb"))
				{
					/* This is a VB source file */
					language = "vb";
					extension = "vb";
				}
				else if(CompareExtensions(filename + len, "java"))
				{
					/* This is a Java source file */
					language = "java";
					extension = "java";
				}
				else if(CompareExtensions(filename + len, "bf") ||
						CompareExtensions(filename + len, "b"))
				{
					/* This is a bf source file */
					language = "bf";
					extension = filename + len;
				}
				else
				{
					/* Use the extension as the language name */
					language = filename + len;
					extension = filename + len;
				}
			}
			else
			{
				/* No extension, so use the default language */
				language = prog_language_name;
				extension = 0;
			}
		}

		/* If no language, then it is a binary that we should skip */
		if(!language)
		{
			continue;
		}

		/* Determine the appropriate way to process the file */
		if(!strcmp(language, "il"))
		{
			/* Assemble this input file using "ilasm" */
			file_proc_types[filenum] = FILEPROC_TYPE_IL;
		}
		else if(!strcmp(language, "jl"))
		{
			/* Assemble this input file using "ilasm" in JVM mode */
			file_proc_types[filenum] = FILEPROC_TYPE_JL;
		}
		else if((plugin = FindLanguagePlugin(language, extension)) != 0)
		{
			/* Compile this input file using a language-specific plug-in */
			plugin_list[filenum] = plugin;
			if(IsSinglePlugin(plugin))
			{
				file_proc_types[filenum] = FILEPROC_TYPE_SINGLE;
			}
			else
			{
				file_proc_types[filenum] = FILEPROC_TYPE_MULTIPLE;
			}
		}
		else
		{
			/* This language is not understood or there is no plug-in for it */
			file_proc_types[filenum] = FILEPROC_TYPE_UNKNOWN;
			fprintf(stderr, _("%s: language %s not recognized or "
							  "plug-in not found\n"), progname, language);
			if(!status)
			{
				status = 1;
			}
		}
	}

	/* Process each of the input files in turn */
	for(filenum = 0; filenum < num_input_files; ++filenum)
	{
		filename = input_files[filenum];
		switch(file_proc_types[filenum])
		{
			case FILEPROC_TYPE_BINARY:
			{
				/* Add the binary to the list of files to be linked */
				CCAddLinkFile(filename, 0);
			}
			break;

			case FILEPROC_TYPE_IL:
			{
				/* Assemble this input file using "ilasm" */
				newstatus = ProcessWithAssembler(filename, 0);
				if(newstatus && !status)
				{
					status = newstatus;
				}
			}
			break;

			case FILEPROC_TYPE_JL:
			{
				/* Assemble this input file using "ilasm" in JVM mode */
				newstatus = ProcessWithAssembler(filename, 1);
				if(newstatus && !status)
				{
					status = newstatus;
				}
			}
			break;

			case FILEPROC_TYPE_SINGLE:
			{
				/* Compile this input file using a language-specific plug-in */
				newstatus = ProcessWithPlugin
								(filename, plugin_list[filenum], filenum, 0);
				if(newstatus && !status)
				{
					status = newstatus;
				}
			}
			break;

			case FILEPROC_TYPE_MULTIPLE:
			{
				/* Compile this input file and all other files for the
				   same language using a language-specific plug-in */
				newstatus = ProcessWithPlugin
								(filename, plugin_list[filenum], filenum, 1);
				if(newstatus && !status)
				{
					status = newstatus;
				}
			}
			break;

			default:	break;
		}
	}

	/* Link the final executable */
	if(!CCStringListContains(extension_flags, num_extension_flags,
							 "syntax-check") &&
	   !CCStringListContains(extension_flags, num_extension_flags,
							 "semantic-check"))
	{
		if(status == 0 && executable_flag)
		{
			status = LinkExecutable();
		}
	}

	/* Delete temporary files that were created prior to the link */
	for(len = 0; len < num_files_to_link; ++len)
	{
		if(files_to_link_temp[len])
		{
			/* Delete this temporary object file */
			ILDeleteFile(files_to_link[len]);
		}
	}

	/* Done */
	return status;
}

/*
 * Change the extension on a filename and return a new filename.
 */
static char *ChangeExtension(char *filename, char *ext)
{
	int len = strlen(filename);
	char *newpath;
	while(len > 0 && filename[len - 1] != '.' &&
	      filename[len - 1] != '/' && filename[len - 1] != '\\' &&
		  filename[len - 1] != ':')
	{
		--len;
	}
	if(len < 1 || filename[len - 1] != '.')
	{
		len = strlen(filename);
	}
	else
	{
		--len;
	}
	newpath = (char *)ILMalloc(len + strlen(ext) + 2);
	if(!newpath)
	{
		CCOutOfMemory();
	}
	ILMemCpy(newpath, filename, len);
	newpath[len] = '.';
	strcpy(newpath + len + 1, ext);
	return newpath;
}

/*
 * Parse the command-line options.
 */
static void ParseCommandLine(int argc, char *argv[])
{
	char *env;
	char *outname, *temp;
	int len;

	/* Call the centralised option parser */
	CCParseCommandLine(argc, argv, CMDLINE_PARSE_CSCC | CMDLINE_PARSE_COMPAT,
					   "cscc");

	/* Add the system include directories */
	if(!nostdinc_flag)
	{
		env = getenv("CSCC_INCLUDE_PATH");
		if(env && *env != '\0')
		{
			CCAddPathStrings(&sys_include_dirs, &num_sys_include_dirs,
							 env, 0, 0);
		}
		else
		{
			CCAddPathStrings(&sys_include_dirs, &num_sys_include_dirs,
							 INCLUDE_PATH,
							 ILGetStandardLibraryPath("cscc/lib"),
							 ILGetStandardDataPath("cscc/include"));
		}
	}
	if(!nostdinc_cpp_flag)
	{
		env = getenv("CSCC_INCLUDE_CPP_PATH");
		if(env && *env != '\0')
		{
			CCAddPathStrings(&sys_cpp_include_dirs, &num_sys_cpp_include_dirs,
							 env, 0, 0);
		}
		else
		{
			CCAddPathStrings(&sys_cpp_include_dirs, &num_sys_cpp_include_dirs,
							 INCLUDE_CPP_PATH,
							 ILGetStandardDataPath("cscc/include/c++"), 0);
		}
	}

	/* We always want "." on the end of the link path */
	CCStringListAdd(&link_dirs, &num_link_dirs, ".");

	/* Add the system link directories */
	if(!nostdlib_flag)
	{
		env = getenv("CSCC_LIB_PATH");
		if(env && *env != '\0')
		{
			CCAddPathStrings(&sys_link_dirs, &num_sys_link_dirs, env, 0, 0);
		}
		else
		{
			CCAddPathStrings(&sys_link_dirs, &num_sys_link_dirs, LIB_PATH,
							 ILGetStandardLibraryPath("cscc/lib"), 0);
		}
	}

	/* If we are not building an executable, then suppress the entry point */
	if(!executable_flag || shared_flag)
	{
		entry_point = 0;
	}

	/* Determine the default output filename */
	if(!output_filename)
	{
		if(executable_flag)
		{
			/* The default output filename is "a.dll" or "a.exe/a.out" */
			if(shared_flag)
			{
				output_filename = "a.dll";
			}
			else
			{
			#ifdef IL_WIN32_PLATFORM
				output_filename = "a.exe";
			#else
				output_filename = "a.out";
			#endif
			}
		}
		else if(compile_flag)
		{
			/* Use the name of the source file with a ".o" extension */
			output_filename = ChangeExtension(input_files[0], "o");
		}
		else if(assemble_flag)
		{
			/* Use the source file name with a ".il" or ".jl" extension */
			if(!CCStringListContains(machine_flags, num_machine_flags, "jvm"))
			{
				output_filename = ChangeExtension(input_files[0], "il");
			}
			else
			{
				output_filename = ChangeExtension(input_files[0], "jl");
			}
		}
		else if(!preprocess_flag)
		{
			/* Instead of the typical "a.out" used by C/C++ compilers,
			   we use "a.dll" and "a.exe" instead, to reflect the
			   Windows-ish nature of IL binaries */
			if(shared_flag)
			{
				output_filename = "a.dll";
			}
			else
			{
				output_filename = "a.exe";
			}
		}
	}
	else if(executable_flag && !shared_flag)
	{
		/* Set the "shared" flag if the executable name ends in ".dll" */
		int len = strlen(output_filename);
		if(len >= 4 && !ILStrICmp(output_filename + len - 4, ".dll"))
		{
			shared_flag = 1;
		}
	}

	/* Set "prog_language_name" to a reasonable default */
	if(!prog_language_name)
	{
		if(prog_language == PROG_LANG_IL)
		{
			prog_language_name = "il";
		}
		else if(prog_language == PROG_LANG_JL)
		{
			prog_language_name = "jl";
		}
		else if(prog_language == PROG_LANG_C)
		{
			prog_language_name = "c";
		}
		else if(prog_language == PROG_LANG_VB)
		{
			prog_language_name = "vb";
		}
		else if(prog_language == PROG_LANG_JAVA)
		{
			prog_language_name = "java";
		}
		else if(prog_language == PROG_LANG_BF)
		{
			prog_language_name = "bf";
		}
		else
		{
			prog_language_name = "cs";
		}
	}

	/* Set the "-ftarget-assembly-name" and "-fstdlib-name" options
	   if building an executable */
	if(executable_flag)
	{
		outname = output_filename;
		len = strlen(outname);
		while(len > 0 && outname[len - 1] != '/' && outname[len - 1] != '\\')
		{
			--len;
		}
		outname += len;
		len = strlen(outname);
		if(len >= 4 && !ILStrICmp(outname + len - 4, ".dll"))
		{
			len -= 4;
		}
		else if(len >= 4 && !ILStrICmp(outname + len - 4, ".exe"))
		{
			len -= 4;
		}
		temp = (char *)ILMalloc(len + 22);
		if(!temp)
		{
			CCOutOfMemory();
		}
		strcpy(temp, "target-assembly-name=");
		ILMemCpy(temp + 21, outname, len);
		temp[len + 21] = '\0';
		CCStringListAdd(&extension_flags, &num_extension_flags, temp);
#if 0
		if(nostdlib_flag)
		{
			temp = (char *)ILMalloc(len + 13);
			if(!temp)
			{
				CCOutOfMemory();
			}
			strcpy(temp, "stdlib-name=");
			ILMemCpy(temp + 12, outname, len);
			temp[len + 12] = '\0';
			CCStringListAdd(&extension_flags, &num_extension_flags, temp);
		}
#endif
	}

}

/*
 * Determine if a file is present.  Returns the filename,
 * which may be altered to include an extension.
 */
static char *FilePresent(char *filename)
{
	char *newExePath = 0;
	if(!ILFileExists(filename, &newExePath))
	{
		return 0;
	}
	if(newExePath)
	{
		return newExePath;
	}
	else
	{
		return filename;
	}
}

/*
 * Search a path list for a particular executable.
 */
static char *SearchPath(char *path, char *name,
						char *standard1, char *standard2)
{
	char **list;
	int num, posn, len;
	int namelen = strlen(name);
	char *temppath;
	char *newpath;

	/* Split the path into its components */
	list = 0;
	num = 0;
	CCAddPathStrings(&list, &num, path, standard1, standard2);

	/* Search for the file */
	for(posn = 0; posn < num; ++posn)
	{
		len = strlen(list[posn]);
		temppath = (char *)ILMalloc(len + namelen + 2);
		if(!temppath)
		{
			CCOutOfMemory();
		}
		strncpy(temppath, list[posn], len);
	#ifdef IL_WIN32_NATIVE
		temppath[len] = '\\';
	#else
		temppath[len] = '/';
	#endif
		strcpy(temppath + len + 1, name);
		if((newpath = FilePresent(temppath)) != 0)
		{
			return newpath;
		}
		ILFree(temppath);
	}

	/* Could not find the requested file */
	return 0;
}

/*
 * Find the plugin for a specific key, testing for
 * either the single or multiple file case.
 */
static char *FindKeyPluginEither(char *key, int lowerCase, int singleFile)
{
	char *name;
	char *path;
	char *newpath;
	int len;

	/* Convert the key into lower case if necessary */
	if(lowerCase)
	{
		char *newkey = (char *)ILMalloc(strlen(key) + 1);
		int sawUpper;
		if(!newkey)
		{
			CCOutOfMemory();
		}
		strcpy(newkey, key);
		key = newkey;
		sawUpper = 0;
		while(*newkey != '\0')
		{
			if(*newkey >= 'A' && *newkey <= 'Z')
			{
				*newkey = (*newkey - 'A' + 'a');
				sawUpper = 1;
			}
			++newkey;
		}
		if(!sawUpper)
		{
			/* No upper case characters, so we've already checked this */
			ILFree(key);
			return 0;
		}
	}

	/* Look for an option named "-fplugin-key-path" */
	name = (char *)ILMalloc(strlen(key) + 13);
	if(!name)
	{
		CCOutOfMemory();
	}
	strcpy(name, "plugin-");
	strcat(name, key);
	strcat(name, "-path");
	path = CCStringListGetValue(extension_flags, num_extension_flags, name);
	ILFree(name);
	if(path)
	{
		return path;
	}

	/* Search the CSCC_PLUGINS_PATH */
	name = (char *)ILMalloc(strlen(key) + 9);
	if(!name)
	{
		CCOutOfMemory();
	}
	strcpy(name, "cscc-");
	strcat(name, key);
	if(singleFile)
	{
		strcat(name, "-s");
	}
	path = SearchPath(getenv("CSCC_PLUGINS_PATH"), name, 0, 0);
	if(path)
	{
		ILFree(name);
		return path;
	}

	/* Search the default plugins path */
	path = SearchPath(PLUGINS_PATH, name,
					  ILGetStandardLibraryPath("cscc/plugins"), 0);
	if(path)
	{
		ILFree(name);
		return path;
	}

	/* If argv[0] contains a directory, then look there */
	if(strchr(progname, '/') != 0 || strchr(progname, '\\') != 0)
	{
		len = strlen(progname);
		while(len > 0 && progname[len - 1] != '/' && progname[len - 1] != '\\')
		{
			--len;
		}
		path = (char *)ILMalloc(len + strlen(name) + 1);
		if(!path)
		{
			CCOutOfMemory();
		}
		strncpy(path, progname, len);
		strcpy(path + len, name);
		if((newpath = FilePresent(path)) != 0)
		{
			ILFree(name);
			return newpath;
		}
		ILFree(path);
	}

	/* Search the normal execution PATH */
	path = SearchPath(getenv("PATH"), name, ILGetStandardProgramPath(), 0);
	if(path)
	{
		ILFree(name);
		return path;
	}

	/* Could not find the plugin */
	ILFree(name);
	return 0;
}

/*
 * Find the plugin for a specific key.
 */
static char *FindKeyPlugin(char *key, int lowerCase)
{
	char *path;

	/* Look for a multiple file plugin */
	path = FindKeyPluginEither(key, lowerCase, 0);
	if(path)
	{
		return path;
	}

	/* Look for a single file plugin */
	return FindKeyPluginEither(key, lowerCase, 1);
}

/*
 * Find the plugin for a specific language.
 */
static char *FindLanguagePlugin(char *name, char *ext)
{
	char *plugin;

	/* Special check for C# so we only fetch the compiler once.
	   This increases efficiency slightly if there are multiple
	   .cs files specified on the command-line */
	if(!strcmp(name, "csharp") || !strcmp(name, "cs"))
	{
		if(csharp_compiler)
		{
			return csharp_compiler;
		}
		csharp_compiler = FindKeyPlugin("cs", 0);
		if(!csharp_compiler)
		{
			csharp_compiler = FindKeyPlugin("csharp", 0);
		}
		return csharp_compiler;
	}

	/* Look for the plugin based on the language name first */
	if(name)
	{
		plugin = FindKeyPlugin(name, 0);
		if(plugin)
		{
			return plugin;
		}
	}

	/* Look for the plugin based on the extension */
	if(ext)
	{
		plugin = FindKeyPlugin(ext, 0);
		if(plugin)
		{
			return plugin;
		}
		plugin = FindKeyPlugin(ext, 1);
		if(plugin)
		{
			return plugin;
		}
	}

	/* Could not find a suitable plugin */
	return 0;
}

/*
 * Compare two file extensions for equality, while ignoring case.
 */
static int CompareExtensions(const char *ext1, const char *ext2)
{
	while(*ext1 != '\0' && *ext2 != '\0')
	{
		if(*ext1 >= 'A' && *ext1 <= 'Z')
		{
			if((*ext1 - 'A' + 'a') != *ext2)
			{
				return 0;
			}
		}
		else
		{
			if(*ext1 != *ext2)
			{
				return 0;
			}
		}
		++ext1;
		++ext2;
	}
	return (*ext1 == '\0' && *ext2 == '\0');
}

/*
 * Determine if a filename corresponds to a single-file plugin.
 */
static int IsSinglePlugin(const char *filename)
{
	int len = strlen(filename);

	/* Strip off the ".exe" extension if we are on a Windows system */
	if(len > 4 &&
	   (filename[len - 1] == 'e' || filename[len - 1] == 'E') &&
	   (filename[len - 2] == 'x' || filename[len - 2] == 'X') &&
	   (filename[len - 3] == 'e' || filename[len - 3] == 'E') &&
	   filename[len - 4] == '.')
	{
		len -= 4;
	}

	/* Determine if the executable name ends in "-s" */
	if(len > 2 &&
	   (filename[len - 1] == 's' || filename[len - 1] == 'S') &&
	   filename[len - 2] == '-')
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Add an argument to a list of command-line arguments.
 */
static void AddArgument(char ***list, int *num, char *str)
{
	if(((*num) & 14) == 0)
	{
		char **newlist = (char **)ILRealloc(*list, sizeof(char *) *
												   (*num + 16));
		if(!newlist)
		{
			CCOutOfMemory();
		}
		*list = newlist;
	}
	(*list)[*num] = str;
	(*list)[*num + 1] = 0;
	++(*num);
}

/*
 * Dump a command-line in verbose mode.
 */
static void DumpCmdLine(char **argv)
{
	if(verbose_mode == VERBOSE_CMDLINES)
	{
		int posn;
		for(posn = 0; argv[posn] != 0; ++posn)
		{
			if(posn != 0)
			{
				putc(' ', stderr);
			}
			fputs(argv[posn], stderr);
		}
		putc('\n', stderr);
	}
}

/*
 * Import the assembler code from "libILAsm".
 */
int ILAsmMain(int argc, char *argv[], FILE *newStdin);

/*
 * Process an input file using the assembler.
 */
static int ProcessWithAssembler(const char *filename, int jvmMode)
{
	char **cmdline;
	int cmdline_size;
	int posn, status;
	char *obj_output;

	/* Build the assembler command-line */
	cmdline = 0;
	cmdline_size = 0;
	AddArgument(&cmdline, &cmdline_size, "ilasm");
	if(executable_flag)
	{
		obj_output = ChangeExtension((char *)filename, "objtmp");
		CCAddLinkFile(obj_output, 1);
	}
	else if(compile_flag)
	{
		obj_output = output_filename;
	}
	else
	{
		obj_output = ChangeExtension((char *)filename, "obj");
	}
	AddArgument(&cmdline, &cmdline_size, "-o");
	AddArgument(&cmdline, &cmdline_size, obj_output);
	if(debug_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "-g");
	}
	for(posn = 0; posn < num_extension_flags; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-f");
		AddArgument(&cmdline, &cmdline_size, extension_flags[posn]);
	}
	if(jvmMode)
	{
		AddArgument(&cmdline, &cmdline_size, "-m");
		AddArgument(&cmdline, &cmdline_size, "jvm");
	}
	for(posn = 0; posn < num_machine_flags; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-m");
		AddArgument(&cmdline, &cmdline_size, machine_flags[posn]);
	}
	AddArgument(&cmdline, &cmdline_size, "--");
	AddArgument(&cmdline, &cmdline_size, (char *)filename);
	AddArgument(&cmdline, &cmdline_size, 0);

	/* Execute the assembler */
	DumpCmdLine(cmdline);
	ILCmdLineSuppressSlash();
	status = ILAsmMain(cmdline_size - 1, cmdline, 0);
	ILFree(cmdline);
	if(status != 0)
	{
		ILDeleteFile(obj_output);
		return status;
	}
	return 0;
}

/*
 * Process an input file using a plug-in.
 */
static int ProcessWithPlugin(const char *filename, char *plugin,
						     int filenum, int isMultiple)
{
	char **cmdline;
	int cmdline_size;
	int posn, status;
	char *asm_output;
	char *obj_output;
	int saveAsm;
	int outputIndex = -1;
	FILE *newStdin = 0;
	int pipePid = 0;
	int canPipe;
	char **pluginCmdline = 0;
	static char * const depLevels[] = {0, "-M", "-MD", "-MM", "-MMD"};

	/* Get the default dependency filename, if necessary */
	if(!preprocess_flag && !dependency_file &&
	   (dependency_level == DEP_LEVEL_MD ||
	    dependency_level == DEP_LEVEL_MMD))
	{
		if((compile_flag || assemble_flag) && output_filename)
		{
			dependency_file = ChangeExtension(output_filename, "d");
		}
		else
		{
			dependency_file = ChangeExtension((char *)filename, "d");
		}
	}

	/* Build the command-line for the plug-in */
	cmdline = 0;
	cmdline_size = 0;
	AddArgument(&cmdline, &cmdline_size, plugin);
	if(preprocess_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "-E");
		if(no_preproc_lines_flag)
		{
			AddArgument(&cmdline, &cmdline_size, "-P");
		}
		if(preproc_comments_flag)
		{
			AddArgument(&cmdline, &cmdline_size, "-C");
		}
		if(dependency_level != 0)
		{
			AddArgument(&cmdline, &cmdline_size, depLevels[dependency_level]);
			if(dependency_gen_flag)
			{
				AddArgument(&cmdline, &cmdline_size, "-MG");
			}
		}
		if(dump_output_format == DUMP_MACROS_ONLY)
		{
			AddArgument(&cmdline, &cmdline_size, "-dM");
		}
		else if(dump_output_format == DUMP_MACROS_AND_OUTPUT)
		{
			AddArgument(&cmdline, &cmdline_size, "-dD");
		}
	}
	else if(dependency_level != 0)
	{
		/* Probably -MD or -MMD, which implies normal compilation */
		AddArgument(&cmdline, &cmdline_size, depLevels[dependency_level]);
		if(dependency_gen_flag)
		{
			AddArgument(&cmdline, &cmdline_size, "-MG");
		}
		if(dependency_file)
		{
			AddArgument(&cmdline, &cmdline_size, "-MF");
			AddArgument(&cmdline, &cmdline_size, dependency_file);
		}
	}
	if(preproc_show_headers)
	{
		AddArgument(&cmdline, &cmdline_size, "-H");
	}
	if(debug_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "-g");
	}
	if(disable_optimizations)
	{
		AddArgument(&cmdline, &cmdline_size, "-O0");
	}
	else if(optimize_flag == 1)
	{
		AddArgument(&cmdline, &cmdline_size, "-O");
	}
	else if(optimize_flag == 2)
	{
		AddArgument(&cmdline, &cmdline_size, "-O2");
	}
	else if(optimize_flag == 3)
	{
		AddArgument(&cmdline, &cmdline_size, "-O3");
	}
	if(undef_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "-undef");
	}
	for(posn = 0; posn < num_user_defined_symbols; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-D");
		AddArgument(&cmdline, &cmdline_size, user_defined_symbols[posn]);
	}
	for(posn = 0; posn < num_pre_defined_symbols; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-D");
		AddArgument(&cmdline, &cmdline_size, pre_defined_symbols[posn]);
	}
	for(posn = 0; posn < num_undefined_symbols; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-U");
		AddArgument(&cmdline, &cmdline_size, undefined_symbols[posn]);
	}
	if(nostdinc_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "-nostdinc");
	}
	if(nostdinc_cpp_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "-nostdinc++");
	}
	for(posn = 0; posn < num_include_dirs; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-I");
		AddArgument(&cmdline, &cmdline_size, include_dirs[posn]);
	}
	for(posn = 0; posn < num_sys_include_dirs; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-J");
		AddArgument(&cmdline, &cmdline_size, sys_include_dirs[posn]);
	}
	for(posn = 0; posn < num_sys_cpp_include_dirs; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-K");
		AddArgument(&cmdline, &cmdline_size, sys_cpp_include_dirs[posn]);
	}
	for(posn = 0; posn < num_imacros_files; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-imacros");
		AddArgument(&cmdline, &cmdline_size, imacros_files[posn]);
	}
	if(nostdlib_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "-nostdlib");
	}
	for(posn = 0; posn < num_link_dirs; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-L");
		AddArgument(&cmdline, &cmdline_size, link_dirs[posn]);
	}
	for(posn = 0; posn < num_sys_link_dirs; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-N");
		AddArgument(&cmdline, &cmdline_size, sys_link_dirs[posn]);
	}
	for(posn = 0; posn < num_libraries; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-l");
		AddArgument(&cmdline, &cmdline_size, libraries[posn]);
	}
	if(all_warnings)
	{
		AddArgument(&cmdline, &cmdline_size, "-Wall");
	}
	if(warnings_as_errors)
	{
		AddArgument(&cmdline, &cmdline_size, "-Werror");
	}
	for(posn = 0; posn < num_warning_flags; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-W");
		AddArgument(&cmdline, &cmdline_size, warning_flags[posn]);
	}
	if(inhibit_warnings)
	{
		AddArgument(&cmdline, &cmdline_size, "-w");
	}
	for(posn = 0; posn < num_extension_flags; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-f");
		AddArgument(&cmdline, &cmdline_size, extension_flags[posn]);
	}
	for(posn = 0; posn < num_machine_flags; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-m");
		AddArgument(&cmdline, &cmdline_size, machine_flags[posn]);
	}
	if(verbose_mode == VERBOSE_FILENAMES)
	{
		AddArgument(&cmdline, &cmdline_size, "-v");
	}
	else if(verbose_mode == VERBOSE_CMDLINES)
	{
		AddArgument(&cmdline, &cmdline_size, "-vv");
	}

	/* Add the output filename to the command-line */
	if(preprocess_flag)
	{
		if(output_filename)
		{
			AddArgument(&cmdline, &cmdline_size, "-o");
			AddArgument(&cmdline, &cmdline_size, output_filename);
		}
		asm_output = 0;
	}
	else
	{
		AddArgument(&cmdline, &cmdline_size, "-o");
		outputIndex = cmdline_size;
		if(assemble_flag)
		{
			if(output_filename)
			{
				/* Use the supplied assembly code output filename */
				AddArgument(&cmdline, &cmdline_size, output_filename);
				asm_output = output_filename;
			}
			else if(CCStringListContains
						(machine_flags, num_machine_flags, "jvm"))
			{
				/* Create a JL assembly output name based on the input */
				asm_output = ChangeExtension((char *)filename, "jl");
				AddArgument(&cmdline, &cmdline_size, asm_output);
			}
			else
			{
				/* Create an IL assembly output name based on the input */
				asm_output = ChangeExtension((char *)filename, "il");
				AddArgument(&cmdline, &cmdline_size, asm_output);
			}
		}
		else if(CCStringListContains(machine_flags, num_machine_flags, "jvm"))
		{
			asm_output = ChangeExtension((char *)filename, "jltmp");
			AddArgument(&cmdline, &cmdline_size, asm_output);
		}
		else
		{
			asm_output = ChangeExtension((char *)filename, "iltmp");
			AddArgument(&cmdline, &cmdline_size, asm_output);
		}
	}

	/* Output the "--" separator, in case some filenames start with "-" */
	AddArgument(&cmdline, &cmdline_size, "--");

	/* Add the name of the input file to the command-line */
	AddArgument(&cmdline, &cmdline_size, (char *)filename);

	/* If this is a multiple-file plugin, then add the names of all
	   other source files for the same plugin to the command-line */
	if(isMultiple)
	{
		for(posn = filenum + 1; posn < num_input_files; ++posn)
		{
			if(file_proc_types[posn] == FILEPROC_TYPE_MULTIPLE &&
			   !strcmp(plugin, plugin_list[posn]))
			{
				/* Add the name to the command-line */
				AddArgument(&cmdline, &cmdline_size, input_files[posn]);

				/* Mark the file as done so we don't process it again */
				file_proc_types[posn] = FILEPROC_TYPE_DONE;
			}
		}
	}

	/* Terminate the command-line */
	AddArgument(&cmdline, &cmdline_size, 0);

	/* Determine if we need to save the assembly stream */
	saveAsm = CCStringListContains(extension_flags, num_extension_flags,
							       "save-asm");

	/* Determine if we might be able to pipe the output into the assembler */
	canPipe = 1;
	if(assemble_flag || preprocess_flag || saveAsm)
	{
		canPipe = 0;
	}
	if(CCStringListContains(extension_flags, num_extension_flags,
							"syntax-check"))
	{
		canPipe = 0;
	}

	/* Execute the plugin, using a pipe if possible */
	if(canPipe)
	{
		cmdline[outputIndex] = "-";
		DumpCmdLine(cmdline);
		status = ILSpawnProcessWithPipe(cmdline, (void **)&newStdin);
		cmdline[outputIndex] = asm_output;
		if(status < 0)
		{
			/* Failed to spawn the process */
			status = 1;
			ILFree(cmdline);
		}
		else if(!status)
		{
			/* This platform does not support pipes: fall back */
			status = ILSpawnProcess(cmdline);
			if(status < 0)
			{
				status = 1;
			}
			ILFree(cmdline);
		}
		else
		{
			/* We won't have an on-disk assembly output file */
			asm_output = 0;
			pipePid = status;
			status = 0;
			pluginCmdline = cmdline;
		}
	}
	else
	{
		DumpCmdLine(cmdline);
		status = ILSpawnProcess(cmdline);
		if(status < 0)
		{
			status = 1;
		}
		ILFree(cmdline);
	}
	if(status != 0)
	{
		if(asm_output != 0)
		{
			ILDeleteFile(asm_output);
		}
		return status;
	}

	/* If we were assembling, preprocessing, or syntax-checking
	   the file, then stop now */
	if(assemble_flag || preprocess_flag)
	{
		return 0;
	}
	if(CCStringListContains(extension_flags, num_extension_flags,
							"syntax-check"))
	{
		if(asm_output != 0)
		{
			ILDeleteFile(asm_output);
		}
		return 0;
	}

	/* Build the assembler command-line */
	cmdline = 0;
	cmdline_size = 0;
	AddArgument(&cmdline, &cmdline_size, "ilasm");
	if(executable_flag)
	{
		obj_output = ChangeExtension((char *)filename, "objtmp");
		CCAddLinkFile(obj_output, 1);
	}
	else if(compile_flag)
	{
		obj_output = output_filename;
	}
	else
	{
		obj_output = ChangeExtension((char *)filename, "obj");
	}
	AddArgument(&cmdline, &cmdline_size, "-o");
	AddArgument(&cmdline, &cmdline_size, obj_output);
	if(debug_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "-g");
	}
	for(posn = 0; posn < num_extension_flags; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-f");
		AddArgument(&cmdline, &cmdline_size, extension_flags[posn]);
	}
	for(posn = 0; posn < num_machine_flags; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-m");
		AddArgument(&cmdline, &cmdline_size, machine_flags[posn]);
	}
	AddArgument(&cmdline, &cmdline_size, "--");
	if(newStdin)
	{
		/* Use the pipe output of the plugin as the assembler's input */
		AddArgument(&cmdline, &cmdline_size, "-");
	}
	else
	{
		AddArgument(&cmdline, &cmdline_size, asm_output);
	}
	AddArgument(&cmdline, &cmdline_size, 0);

	/* Execute the assembler */
	DumpCmdLine(cmdline);
	ILCmdLineSuppressSlash();
	status = ILAsmMain(cmdline_size - 1, cmdline, newStdin);
	ILFree(cmdline);
	if(newStdin)
	{
		/* Close the pipe to the language plugin */
		int newStatus;
		fclose(newStdin);
		newStatus = ILSpawnProcessWaitForExit(pipePid, cmdline);
		ILFree(pluginCmdline);
		if(status == 0)
		{
			status = newStatus;
		}
	}
	if(status != 0)
	{
		if(!saveAsm && asm_output)
		{
			ILDeleteFile(asm_output);
		}
		ILDeleteFile(obj_output);
		return status;
	}
	if(!saveAsm && asm_output)
	{
		ILDeleteFile(asm_output);
	}

	/* Done */
	return 0;
}

/*
 * Link the final executable.
 */
static int LinkExecutable(void)
{
	char **cmdline;
	int cmdline_size;
	int posn, status;

	/* Build the linker command-line */
	cmdline = 0;
	cmdline_size = 0;
	AddArgument(&cmdline, &cmdline_size, "ilalink");
	AddArgument(&cmdline, &cmdline_size, "-o");
	AddArgument(&cmdline, &cmdline_size, output_filename);
	if(shared_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "--format");
		AddArgument(&cmdline, &cmdline_size, "dll");
	}
	else
	{
		AddArgument(&cmdline, &cmdline_size, "--format");
		AddArgument(&cmdline, &cmdline_size, "exe");
	}
	if(entry_point)
	{
		AddArgument(&cmdline, &cmdline_size, "--entry-point");
		AddArgument(&cmdline, &cmdline_size, entry_point);
	}
	if(nostdlib_flag)
	{
		AddArgument(&cmdline, &cmdline_size, "--no-stdlib");
	}
	for(posn = 0; posn < num_extension_flags; ++posn)
	{
		if(!resources_only ||
		   strncmp(extension_flags[posn], "resources=", 10) != 0)
		{
			AddArgument(&cmdline, &cmdline_size, "-f");
			AddArgument(&cmdline, &cmdline_size, extension_flags[posn]);
		}
	}
	for(posn = 0; posn < num_machine_flags; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-m");
		AddArgument(&cmdline, &cmdline_size, machine_flags[posn]);
	}
	for(posn = 0; posn < num_link_dirs; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-L");
		AddArgument(&cmdline, &cmdline_size, link_dirs[posn]);
	}
	for(posn = 0; posn < num_sys_link_dirs; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-L");
		AddArgument(&cmdline, &cmdline_size, sys_link_dirs[posn]);
	}
	for(posn = 0; posn < num_libraries; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, "-l");
		AddArgument(&cmdline, &cmdline_size, libraries[posn]);
	}
	if(resources_only)
	{
		AddArgument(&cmdline, &cmdline_size, "--resources-only");
	}

	AddArgument(&cmdline, &cmdline_size, "--");
	for(posn = 0; posn < num_files_to_link; ++posn)
	{
		AddArgument(&cmdline, &cmdline_size, files_to_link[posn]);
	}
	if(resources_only)
	{
		for(posn = 0; posn < num_extension_flags; ++posn)
		{
			if(!strncmp(extension_flags[posn], "resources=", 10))
			{
				AddArgument(&cmdline, &cmdline_size,
							extension_flags[posn] + 10);
			}
		}
	}
	AddArgument(&cmdline, &cmdline_size, 0);

	/* Execute the linker */
	DumpCmdLine(cmdline);
	ILCmdLineSuppressSlash();
	status = ILLinkerMain(cmdline_size - 1, cmdline);
	ILFree(cmdline);
	if(status != 0)
	{
		ILDeleteFile(output_filename);
		return status;
	}

	/* Done */
	return 0;
}

#ifdef	__cplusplus
};
#endif
