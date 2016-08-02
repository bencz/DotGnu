/*
 * cmdline.c - Command-line option parsing routines.
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
#include "il_utils.h"
#include "il_regex.h"
#include "il_sysio.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#if !defined(__palmos__)

/*
 * Flag that indicates if '/' parsing is suppressed.
 */
static int suppressSlash = 0;

/*
 * Determine if an option list contains '/' compatibility options.
 */
static int HasSlashOptions(const ILCmdLineOption *options)
{
	if(suppressSlash)
	{
		return 0;
	}
	while(options->name != 0)
	{
		if(options->name[0] == '/')
		{
			return 1;
		}
		++options;
	}
	return 0;
}

/*
 * Recognize a '/' compatibility option.  Returns the option value
 * if valid, zero if not recognized, or -1 if an error occurred.
 */
static int RecognizeSlashOption(const ILCmdLineOption *options,
								char *arg, char **param)
{
	int namelen;
	const char *optname;
	int posn, ch1, ch2;

	/* Parse the option into name and value */
	namelen = 1;
	while(arg[namelen] != '\0' && arg[namelen] != ':' && arg[namelen] != '=')
	{
		++namelen;
	}
	if(arg[namelen] == '\0')
	{
		*param = 0;
	}
	else
	{
		*param = arg + namelen + 1;
	}

	/* Look through the option table for a match */
	while(options->name != 0)
	{
		/* Skip non-slash options */
		if(options->name[0] != '/')
		{
			++options;
			continue;
		}

		/* Match the option name, while ignoring case.  If we come
		   across a '*', then match only the prefix up to that point */
		posn = 1;
		optname = options->name;
		while(optname[posn] != '\0')
		{
			ch1 = optname[posn];
			if(ch1 >= 'A' && ch1 <= 'Z')
			{
				ch1 = ch1 - 'A' + 'a';
			}
			if(posn < namelen)
			{
				ch2 = arg[posn];
			}
			else
			{
				ch2 = '\0';
			}
			if(ch2 >= 'A' && ch2 <= 'Z')
			{
				ch2 = ch2 - 'A' + 'a';
			}
			if(ch1 == '*')
			{
				break;
			}
			else if(ch1 != ch2)
			{
				break;
			}
			++posn;
		}

		/* Did we get an option match? */
		if((optname[posn] == '\0' && posn == namelen) || optname[posn] == '*')
		{
			if(options->hasParam)
			{
				if(*param)
				{
					return options->value;
				}
				else
				{
					return -1;
				}
			}
			else
			{
				if(*param)
				{
					return -1;
				}
				else
				{
					return options->value;
				}
			}
		}

		/* Proceed to the next option in the table */
		++options;
	}
	return 0;
}

/*
 * Rearrange a command-line so that '/' options precede regular arguments.
 */
static void RearrangeSlashOptions(const ILCmdLineOption *options,
								  int argc, char **argv)
{
	int posn = 1;
	int slashInsert = 1;
	int temp;
	char *param, *param2;
	while(posn < argc)
	{
		if(argv[posn][0] == '/' &&
		   RecognizeSlashOption(options, argv[posn], &param) != 0)
		{
			/* Shift this option to the beginning of the command-line */
			param = argv[posn];
			for(temp = slashInsert; temp <= posn; ++temp)
			{
				param2 = argv[temp];
				argv[temp] = param;
				param = param2;
			}
			++slashInsert;
		}
		else if(!strcmp(argv[posn], "--"))
		{
			/* Stop when we see "--", as any arguments that follow
			   starting with '/' are probably filenames, not options */
			break;
		}
		++posn;
	}
}

int ILCmdLineNextOption(int *argc, char ***argv, int *state,
						const ILCmdLineOption *options, char **param)
{
	char opt;
	int value;

	if(*state == 0)
	{
		/* This is the first time we have been called,
		   so expand response files before we get going */
		ILCmdLineExpand(argc, argv);

		/* If this program supports '/' compatibility options,
		   then rearrange the command-line so that those options
		   are at the front, before all other arguments */
		if(HasSlashOptions(options))
		{
			RearrangeSlashOptions(options, *argc, *argv);
		}

		/* Start parsing the first option */
		*state = 1;
	}

	if(*state == 1)
	{
		/* We are at the start of a new option */
		if(*argc <= 1)
		{
			return 0;
		}
		if((*argv)[1][0] == '/' && HasSlashOptions(options))
		{
			/* Recognise compatibility options that start with '/' */
			value = RecognizeSlashOption(options, (*argv)[1], param);
			if(value > 0)
			{
				--(*argc);
				++(*argv);
				return value;
			}
			else if(value < 0)
			{
				return value;
			}
		}
		if((*argv)[1][0] != '-' || (*argv)[1][1] == '\0')
		{
			return 0;
		}
		if((*argv)[1][1] == '-')
		{
			/* The option begins with "--" */
			if((*argv)[1][2] == '\0')
			{
				--(*argc);
				++(*argv);
				return 0;
			}
			while(options->name != 0)
			{
				if(!strcmp(options->name, (*argv)[1]))
				{
					if(options->hasParam)
					{
						/* Look for a parameter for this option */
						--(*argc);
						++(*argv);
						if(*argc <= 1)
						{
							return -1;
						}
						*param = (*argv)[1];
					}
					--(*argc);
					++(*argv);
					return options->value;
				}
				++options;
			}
			return -1;
		}
		else
		{
			/* The option begins with "-" */
			*state = 2;
		}
	}

	/* If we get here, then we are parsing single-character options */
	opt = (*argv)[1][*state - 1];
	if(opt == '-')
	{
		return -1;
	}
	while(options->name != 0)
	{
		if(options->name[0] == '-' &&
		   opt == options->name[1])
		{
			++(*state);
			if(options->hasParam)
			{
				/* Look for a parameter for this option */
				if((*argv)[1][*state - 1] != '\0')
				{
					*param = (*argv)[1] + *state - 1;
				}
				else
				{
					--(*argc);
					++(*argv);
					if(*argc <= 1)
					{
						return -1;
					}
					*param = (*argv)[1];
				}
				*state = 1;
				--(*argc);
				++(*argv);
				return options->value;
			}
			if((*argv)[1][*state - 1] == '\0')
			{
				*state = 1;
				--(*argc);
				++(*argv);
			}
			return options->value;
		}
		++options;
	}
	return -1;
}

void ILCmdLineHelp(const ILCmdLineOption *options)
{
	const char *str;
	while(options->name != 0)
	{
		if(options->helpString1)
		{
			fprintf(stdout, "    %s\n        ", options->helpString1);
			str = options->helpString2;
			while(*str != '\0')
			{
				if(*str == '\n')
				{
					fputs("\n        ", stdout);
				}
				else
				{
					putc(*str, stdout);
				}
				++str;
			}
			putc('\n', stdout);
		}
		++options;
	}
}

/*
 * Abort due to insufficient memory.
 */
static void OutOfMemory(void)
{
	fputs("virtual memory exhausted\n", stderr);
	exit(1);
}

/*
 * Add a new argument to the command-line that is being built.
 */
static void AddNewArg(int *argc, char ***argv, int *maxArgc, char *value)
{
	if(*argc >= *maxArgc)
	{
		char **newArgs = (char **)ILRealloc
				(*argv, (*maxArgc + 32) * sizeof(char *));
		if(!newArgs)
		{
			OutOfMemory();
		}
		*maxArgc += 32;
		*argv = newArgs;
	}
	(*argv)[*argc] = value;
	++(*argc);
}

/*
 * Do we need to expand wildcard specifications?
 */
#ifdef	IL_WIN32_NATIVE
	#define	IL_EXPAND_WILDCARDS	1
#endif

#ifdef IL_EXPAND_WILDCARDS

/*
 * Expand wildcards from an argument specification.
 */
static void ExpandWildcards(int *argc, char ***argv, int *maxArgc, char *value)
{
	int len;
	char *directory;
	char *baseName;
	char *regex;
	int posn, ch;
	regex_t state;
	ILDir *dir;
	ILDirEnt *entry;
	int first;
	int firstArg;
	const char *name;
	char *full;

	/* Split the value into directory and base name components */
	len = strlen(value);
	while(len > 0 && value[len - 1] != '/' && value[len - 1] != '\\' &&
	      value[len - 1] != ':')
	{
		--len;
	}
	directory = ILDupNString(value, len);
	if(!directory)
	{
		OutOfMemory();
	}
	baseName = value + len;

	/* Convert the base name into a regular expression */
	regex = (char *)ILMalloc(strlen(baseName) * 2 + 16);
	if(!regex)
	{
		OutOfMemory();
	}
	if(!strcmp(baseName, "*.*"))
	{
		/* Special case: map "*.*" to "match everything", even
		   those filenames that don't include a ".", to be
		   consistent with standard Windows practice */
		strcpy(regex, "^[^.].*$");
	}
	else
	{
		posn = 0;
		regex[posn++] = '^';
		first = 1;
		while(*baseName != '\0')
		{
			ch = *baseName++;
			if(ch == '?')
			{
				if(first)
				{
					/* Don't match '.' at the start of the name */
					regex[posn++] = '[';
					regex[posn++] = '^';
					regex[posn++] = '.';
					regex[posn++] = ']';
				}
				else
				{
					regex[posn++] = '.';
				}
			}
			else if(ch == '*')
			{
				if(first)
				{
					/* Don't match '.' at the start of the name */
					regex[posn++] = '[';
					regex[posn++] = '^';
					regex[posn++] = '.';
					regex[posn++] = ']';
					regex[posn++] = '.';
					regex[posn++] = '*';
				}
				else
				{
					regex[posn++] = '.';
					regex[posn++] = '*';
				}
			}
			else if(ch == '.' || ch == '^' || ch == '$' || ch == '[' ||
					ch == ']' || ch == '\\' || ch == '(' || ch == ')')
			{
				regex[posn++] = '\\';
				regex[posn++] = (char)ch;
			}
			else
			{
				regex[posn++] = (char)ch;
			}
			first = 0;
		}
		regex[posn++] = '$';
		regex[posn] = '\0';
	}
	if(IL_regcomp(&state, regex, REG_EXTENDED | REG_NOSUB) != 0)
	{
		fprintf(stderr, "Invalid regular expression: %s\n", regex);
		exit(1);
	}

	/* Scan the directory for regular expression matches */
	firstArg = *argc;
	dir = ILOpenDir(directory);
	while(dir && (entry = ILReadDir(dir)) != 0)
	{
		name = ILDirEntName(entry);
		if(IL_regexec(&state, name, 0, 0, 0) == 0)
		{
			/* We have found a match against the regular expression */
			full = (char *)ILMalloc(strlen(directory) + strlen(name) + 1);
			if(!full)
			{
				ILCloseDir(dir);
				OutOfMemory();
			}
			strcpy(full, directory);
			strcat(full, name);
			AddNewArg(argc, argv, maxArgc, full);
		}
		ILFree(entry);
	}
	if(dir)
	{
		ILCloseDir(dir);
	}

	/* If we didn't find any matches, then add the wildcard specification
	   as an argument.  The program will probably report "File not found",
	   which is what we want it to do.  Otherwise, sort the results */
	if(firstArg >= *argc)
	{
		AddNewArg(argc, argv, maxArgc, value);
	}
	else
	{
		int index1, index2;
		char *temp;
		for(index1 = firstArg; index1 < (*argc - 1); ++index1)
		{
			for(index2 = index1 + 1; index2 < *argc; ++index2)
			{
				if(ILStrICmp((*argv)[index1], (*argv)[index2]) > 0)
				{
					temp = (*argv)[index1];
					(*argv)[index1] = (*argv)[index2];
					(*argv)[index2] = temp;
				}
			}
		}
	}

	/* Clean up and exit */
	IL_regfree(&state);
	ILFree(directory);
	ILFree(regex);
}

#endif /* IL_EXPAND_WILDCARDS */

void ILCmdLineExpand(int *argc, char ***argv)
{
	int arg;
	int newArgc;
	int maxArgc;
	char **newArgv;
	int len;
	char *filename;
	FILE *file;
	char buffer[BUFSIZ];
	char *temp;
	char *respfile;

	/* See if we have any response file or wildcard references first */
	for(arg = 1; arg < *argc; ++arg)
	{
		if((*argv)[arg][0] == '@')
		{
			break;
		}
#ifdef IL_EXPAND_WILDCARDS
		if(strchr((*argv)[arg], '?') != 0 ||
		   strchr((*argv)[arg], '*') != 0)
		{
			break;
		}
#endif
	}
	if(arg >= *argc)
	{
		/* No response files or wildcards, so nothing further to do */
		return;
	}

	/* Build a new command-line */
	newArgc = 0;
	maxArgc = 0;
	newArgv = 0;
	AddNewArg(&newArgc, &newArgv, &maxArgc, (*argv)[0]);
	for(arg = 1; arg < *argc; ++arg)
	{
		if((*argv)[arg][0] == '@')
		{
			/* Response file reference */
			respfile = (*argv)[arg] + 1;
			if(*respfile == '"')
			{
				++respfile;
				len = strlen(respfile);
				if(len > 0 && respfile[len - 1] == '"')
				{
					--len;
				}
			}
			else
			{
				len = strlen(respfile);
			}
			if(len > 0)
			{
				filename = ILDupNString(respfile, len);
				if(!filename)
				{
					OutOfMemory();
				}
				if((file = fopen(filename, "r")) == NULL)
				{
					perror(filename);
					exit(1);
				}
				while(fgets(buffer, BUFSIZ, file))
				{
					len = strlen(buffer);
					while(len > 0 &&
					      (buffer[len - 1] == '\r' || buffer[len - 1] == '\n' ||
						   buffer[len - 1] == '\t' || buffer[len - 1] == ' '))
					{
						--len;
					}
					buffer[len] = '\0';
					if(*buffer == '"' && len >= 2 && buffer[len - 1] == '"')
					{
						/* Quoted argument in response file */
						buffer[len - 1] = '\0';
						temp = ILDupString(buffer + 1);
					}
					else if (len > 0)
					{
						/* Ordinary argument in response file */
						temp = ILDupString(buffer);
					}
					else
					{
						/* Empty line in response file */
						continue;
					}
					if(!temp)
					{
						OutOfMemory();
					}
					AddNewArg(&newArgc, &newArgv, &maxArgc, temp);
				}
				fclose(file);
			}
			else
			{
				/* Badly formed response filename */
				fputs((*argv)[arg], stderr);
				fputs(": bad response filename specification\n", stderr);
				exit(1);
			}
		}
#ifdef IL_EXPAND_WILDCARDS
		else if(strchr((*argv)[arg], '?') != 0 ||
		        strchr((*argv)[arg], '*') != 0)
		{
			/* Expand wildcards within the argument */
			ExpandWildcards(&newArgc, &newArgv, &maxArgc, (*argv)[arg]);
		}
#endif
		else
		{
			/* Ordinary argument */
			AddNewArg(&newArgc, &newArgv, &maxArgc, (*argv)[arg]);
		}
	}
	AddNewArg(&newArgc, &newArgv, &maxArgc, 0);

	/* Return the new command-line to the caller */
	*argc = newArgc - 1;
	*argv = newArgv;
}

void ILCmdLineSuppressSlash(void)
{
	suppressSlash = 1;
}

#else /* __palmos__ */

/*
 * Stub out the command-line handling under PalmOS.
 */

int ILCmdLineNextOption(int *argc, char ***argv, int *state,
						const ILCmdLineOption *options, char **param)
{
	return -1;
}

void ILCmdLineHelp(const ILCmdLineOption *options)
{
}

void ILCmdLineExpand(int *argc, char ***argv)
{
}

void ILCmdLineSuppressSlash(void)
{
}

#endif /* __palmos__ */

#ifdef	__cplusplus
};
#endif
