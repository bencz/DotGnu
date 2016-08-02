/*
 * cc_options.h - Command-line option processing.
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

#ifndef	_CSCC_CC_OPTIONS_H
#define	_CSCC_CC_OPTIONS_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Supported programming languages for "prog_language".
 */
#define	PROG_LANG_DEFAULT		0
#define	PROG_LANG_CSHARP		1
#define	PROG_LANG_IL			2
#define	PROG_LANG_JL			3
#define	PROG_LANG_C				4
#define	PROG_LANG_VB			5
#define	PROG_LANG_JAVA			6
#define	PROG_LANG_BF			7
#define	PROG_LANG_OTHER			8

/*
 * Supported output/macro dump formats for "dump_output_format".
 */
#define	DUMP_OUTPUT_ONLY		0
#define	DUMP_MACROS_ONLY		1
#define	DUMP_MACROS_AND_OUTPUT	2

/*
 * Verbosity.
 */
#define	VERBOSE_NONE			0
#define	VERBOSE_FILENAMES		1
#define	VERBOSE_CMDLINES		2

/*
 * Pre-processor dependency levels.
 */
#define	DEP_LEVEL_NONE			0
#define	DEP_LEVEL_M				1
#define	DEP_LEVEL_MD			2
#define	DEP_LEVEL_MM			3
#define	DEP_LEVEL_MMD			4

/*
 * Command-line option values.
 */
extern char *progname;
extern char *version_name;
extern int compile_flag;
extern int assemble_flag;
extern int preprocess_flag;
extern int no_preproc_lines_flag;
extern int preproc_comments_flag;
extern int debug_flag;
extern int nostdinc_flag;
extern int nostdinc_cpp_flag;
extern int nostdlib_flag;
extern int undef_flag;
extern int shared_flag;
extern int static_flag;
extern int executable_flag;
extern int optimize_flag;
extern int disable_optimizations;
extern int resources_only;
extern char **pre_defined_symbols;
extern int num_pre_defined_symbols;
extern char **user_defined_symbols;
extern int num_user_defined_symbols;
extern char **undefined_symbols;
extern int num_undefined_symbols;
extern char **include_dirs;
extern int num_include_dirs;
extern char **sys_include_dirs;
extern int num_sys_include_dirs;
extern char **sys_cpp_include_dirs;
extern int num_sys_cpp_include_dirs;
extern char **link_dirs;
extern int num_link_dirs;
extern char **sys_link_dirs;
extern int num_sys_link_dirs;
extern char **libraries;
extern int num_libraries;
extern char *output_filename;
extern char **input_files;
extern int num_input_files;
extern char *entry_point;
extern char **extension_flags;
extern int num_extension_flags;
extern char **warning_flags;
extern int num_warning_flags;
extern int inhibit_warnings;
extern int all_warnings;
extern int warnings_as_errors;
extern char **machine_flags;
extern int num_machine_flags;
extern int prog_language;
extern char *prog_language_name;
extern int dump_output_format;
extern int verbose_mode;
extern char **files_to_link;
extern int *files_to_link_temp;
extern int num_files_to_link;
extern char **imacros_files;
extern int num_imacros_files;
extern int dependency_level;
extern int dependency_gen_flag;
extern char *dependency_file;
extern int preproc_show_headers;

/*
 * Add a path to a list of strings.
 */
void CCAddPathStrings(char ***list, int *num, char *path,
					  char *standard1, char *standard2);

/*
 * Command-line parsing modes.
 */
#define	CMDLINE_PARSE_CSCC				0	/* The cscc front-end */
#define	CMDLINE_PARSE_PLUGIN			1	/* Plugin */
#define	CMDLINE_PARSE_PLUGIN_CPP		2	/* Plugin with C++ includes */
#define	CMDLINE_PARSE_PLUGIN_NOSTDINC	3	/* Plugin with its own includes */
#define	CMDLINE_PARSE_PLUGIN_NOSTDLIB	8	/* Plugin with its own libraries */
#define	CMDLINE_PARSE_COMPAT			16	/* Cscc with compat options */

/*
 * Parse the command-line options.
 */
void CCParseCommandLine(int argc, char *argv[], int mode, char *versname);

/*
 * Report out of memory and abort the program.
 */
void CCOutOfMemory(void);

/*
 * Determine if a list of strings contains a particular value.
 */
int CCStringListContains(char **list, int num, const char *value);

/*
 * Determine if a list of strings contains a particular inverse value.
 */
int CCStringListContainsInv(char **list, int num, const char *value);

/*
 * Look for a "name=value" option in a list of strings and return the value.
 * Returns NULL if the name is not present, or has no value associated with it.
 */
char *CCStringListGetValue(char **list, int num, const char *name);

/*
 * Add a file to the "files_to_link" list.  If "isTemp"
 * is non-zero, then it is a temporary file that should
 * be deleted after the link has completed.
 */
void CCAddLinkFile(char *filename, int isTemp);

/*
 * Add a string to a list of strings.
 */
void CCStringListAdd(char ***list, int *num, char *str);

/*
 * Add an option/string concatenation to a list of strings.
 */
void CCStringListAddOption(char ***list, int *num, char *option, char *str);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSCC_CC_OPTIONS_H */
