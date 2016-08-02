/*
 * csant_fileset.c - File set management routines.
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

#include "csant_defs.h"
#include "csant_fileset.h"
#include "il_regex.h"
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif
#ifdef HAVE_SYS_STAT_H
#include <sys/stat.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Internal definition of the file set structure.
 */
struct _tagCSAntFileSet
{
	unsigned long	numFiles;
	unsigned long	maxFiles;
	char		  **files;

};

/*
 * Stack item for the stack of directories yet to be visited.
 */
typedef struct _tagCSAntDirStack CSAntDirStack;
struct _tagCSAntDirStack
{
	char          *dirName;
	const char    *regex;
	CSAntDirStack *next;

};

/*
 * Add a character to the "regex" string.
 */
static void RegexChar(char **regex, int *regexLen, int *regexMax, char ch)
{
	char *newRegex = (char *)ILRealloc(*regex, *regexMax + 32);
	if(!newRegex)
	{
		CSAntOutOfMemory();
	}
	*regexMax += 32;
	*regex = newRegex;
	newRegex[(*regexLen)++] = ch;
}
#define	REGEX_CHAR(ch)	\
			do { \
				if(regexLen < regexMax) \
				{ \
					regex[regexLen++] = (char)(ch); \
				} \
				else \
				{ \
					RegexChar(&regex, &regexLen, &regexMax, (char)(ch)); \
				} \
			} while (0)

static int CheckIf(CSAntTask *node)
{
	const char *val=CSAntTaskParam(node,"if");
	if(!val)
	{
		return 1;
	}
	if(val && !ILStrICmp(val,"true"))
	{
		return 1;
	}
	return 0;
}

/*
 * Build a regular expression from a wildcard argument specification.
 * Returns non-zero if recursion into sub-directories is specified.
 */
typedef regex_t RegexState;
static int BuildIncludeRegex(const char *arg, RegexState *state)
{
	char *regex = 0;
	int regexLen = 0;
	int regexMax = 0;
	int recursive = 0;
	char ch;

	/* Build the regular expression */
	REGEX_CHAR('^');
	while(*arg != 0)
	{
		ch = *arg++;
		if(ch == '*' && *arg == '*')
		{
			/* Recursive regular expression definition */
			recursive = 1;
			if(arg[1] == '/' || arg[1] == '\\')
			{
				REGEX_CHAR('(');
				REGEX_CHAR('.');
				REGEX_CHAR('*');
			#ifdef IL_WIN32_NATIVE
				REGEX_CHAR('\\');
				REGEX_CHAR('\\');
			#else
				REGEX_CHAR('/');
			#endif
				REGEX_CHAR('|');
				REGEX_CHAR('(');
				REGEX_CHAR(')');
				REGEX_CHAR(')');
				arg += 2;
			}
			else
			{
				REGEX_CHAR('.');
				REGEX_CHAR('*');
				++arg;
			}
		}
		else if(ch == '*')
		{
			/* Match anything that doesn't involve a separator */
			REGEX_CHAR('[');
			REGEX_CHAR('^');
		#ifdef IL_WIN32_NATIVE
			REGEX_CHAR('\\');
			REGEX_CHAR('\\');
		#else
			REGEX_CHAR('/');
		#endif
			REGEX_CHAR(']');
			REGEX_CHAR('*');
		}
		else if(ch == '?')
		{
			/* Match a single character */
			REGEX_CHAR('.');
		}
		else if(ch == '/' || ch == '\\')
		{
			/* Match a directory separator */
			recursive = 1;
		#ifdef IL_WIN32_NATIVE
			REGEX_CHAR('\\');
			REGEX_CHAR('\\');
		#else
			REGEX_CHAR('/');
		#endif
		}
		else if(ch == '[')
		{
			/* Match a set of characters */
			REGEX_CHAR('[');
			while(*arg != 0 && *arg != ']')
			{
				REGEX_CHAR(*arg);
				++arg;
			}
			REGEX_CHAR(']');
			if(*arg != '\0')
			{
				++arg;
			}
		}
		else if(ch == '.' || ch == '^' || ch == '$' ||
				ch == ']' || ch == '\\' || ch == '(' || ch == ')')
		{
			/* Match a special character */
			REGEX_CHAR('\\');
			REGEX_CHAR(ch);
		}
		else
		{
			/* Match an ordinary character */
		#ifdef _WIN32
			if(ch >= 'A' && ch <= 'Z')
			{
				REGEX_CHAR('[');
				REGEX_CHAR(ch);
				REGEX_CHAR(ch - 'A' + 'a');
				REGEX_CHAR(']');
			}
			else if(ch >= 'a' && ch <= 'z')
			{
				REGEX_CHAR('[');
				REGEX_CHAR(ch - 'a' + 'A');
				REGEX_CHAR(ch);
				REGEX_CHAR(']');
			}
			else
			{
				REGEX_CHAR(ch);
			}
		#else
			REGEX_CHAR(ch);
		#endif
		}
	}

	/* Terminate the regular expression */
	if(regexLen == 1)
	{
		/* The regular expression was empty, so match nothing */
		REGEX_CHAR('(');
		REGEX_CHAR(')');
	}
	REGEX_CHAR('$');
	REGEX_CHAR('\0');

	/* Compile the register expression and exit */
	if(IL_regcomp(state, regex, REG_EXTENDED | REG_NOSUB) != 0)
	{
		fprintf(stderr, "Invalid regular expression: %s\n", regex);
		exit(1);
	}
	ILFree(regex);
	return recursive;
}

/*
 * Match a pathname against an include regular expression.
 */
static int MatchInclude(char *pathname, RegexState *state)
{
	return (IL_regexec(state, pathname, 0, 0, 0) == 0);
}

/*
 * Add a pathname to a file set.
 */
static void AddToFileSet(CSAntFileSet *fileset, char *pathname)
{
	if(fileset->numFiles >= fileset->maxFiles)
	{
		fileset->files = (char **)ILRealloc(fileset->files,
											sizeof(char *) *
												(fileset->numFiles + 32));
		if(!(fileset->files))
		{
			CSAntOutOfMemory();
		}
		fileset->maxFiles = fileset->numFiles + 32;
	}
	fileset->files[(fileset->numFiles)++] = pathname;
}

/*
 * Remove a pathname from a file set.
 */
static void RemoveFromFileSet(CSAntFileSet *fileset, char *pathname)
{
	unsigned long posn;
	for(posn = 0; posn < fileset->numFiles; ++posn)
	{
		if(!strcmp(fileset->files[posn], pathname))
		{
			ILFree(fileset->files[posn]);
			--(fileset->numFiles);
			while(posn < fileset->numFiles)
			{
				fileset->files[posn] = fileset->files[posn + 1];
				++posn;
			}
			return;
		}
	}
}

/*
 * Determine if a pathname corresponds to a directory.
 */
static int IsDirectory(const char *pathname)
{
#ifdef HAVE_STAT
	struct stat st;
	if(stat(pathname, &st) >= 0 && S_ISDIR(st.st_mode))
	{
		return 1;
	}
	else
	{
		return 0;
	}
#else
	return 0;
#endif
}

/*
 * Add directory and regex information to a directory stack for
 * a particular "includes" specification.
 */
static CSAntDirStack *DirStackAddInclude
			(CSAntDirStack *stack, CSAntFileSet *fileset,
			 CSAntTask *include, const char *baseDir,
			 int addToFileSet)
{
	const char *arg;
	int posn, start;
	char *pathname;
	char *newBase;
	CSAntDirStack *item;

	/* Get the name argument */
	arg = CSAntTaskParam(include, "name");
	if(!arg)
	{
		return stack;
	}

	/* Extract the leading directory information before the wildcards
	   to find the new base to start at */
	posn = 0;
	start = 0;
	while(arg[posn] != '\0')
	{
		while(arg[posn] != '/' && arg[posn] != '\\' && arg[posn] != '\0')
		{
			if(arg[posn] == '*' || arg[posn] == '?' || arg[posn] == '[')
			{
				break;
			}
			++posn;
		}
		if(arg[posn] == '*' || arg[posn] == '?' || arg[posn] == '[')
		{
			break;
		}
		else if(arg[posn] != '\0')
		{
			++posn;
		}
		start = posn;
	}
	if(arg[start] == '\0')
	{
		/* This is a literal pathname with no wildcards */
		pathname = CSAntDirCombine(baseDir, arg);
		if(addToFileSet)
		{
			AddToFileSet(fileset, pathname);
		}
		else
		{
			RemoveFromFileSet(fileset, pathname);
			ILFree(pathname);
		}
		return stack;
	}

	/* Build the new base directory */
	if(start != 0)
	{
		pathname = ILDupNString(arg, start - 1);
		if(!pathname)
		{
			CSAntOutOfMemory();
		}
		newBase = CSAntDirCombine(baseDir, pathname);
		ILFree(pathname);
		arg += start;
	}
	else
	{
		newBase = ILDupString(baseDir);
		if(!newBase)
		{
			CSAntOutOfMemory();
		}
	}

	/* Add the directory and regex to the stack */
	item = (CSAntDirStack *)ILMalloc(sizeof(CSAntDirStack));
	if(!item)
	{
		CSAntOutOfMemory();
	}
	item->dirName = newBase;
	item->regex = arg;
	item->next = stack;
	return item;
}

/*
 * Add a group of includes to a directory stack.
 */
static CSAntDirStack *DirStackAddIncludes(CSAntFileSet *fileset,
										  CSAntTask *task,
										  const char *name,
										  const char *baseDir,
										  int addToFileSet)
{
	CSAntDirStack *stack = 0;
	CSAntTask *subNode;
	subNode = task->taskChildren;
	while(subNode != 0)
	{
		if(CheckIf(subNode))
		{
			if(!strcmp(subNode->name, name))
			{
				stack = DirStackAddInclude
					(stack, fileset, subNode, baseDir, addToFileSet);
			}
		}
		subNode = subNode->next;
	}
	return stack;
}

/*
 * Scan a directory start to get files to add to or remove from a fileset.
 */
static void ProcessDirStack(CSAntFileSet *fileset, CSAntDirStack *stack,
							int addToFileSet)
{
	RegexState state;
	int haveRegex = 0;
	char *currentDir;
	const char *currentRegex;
	CSAntDirStack *nextStack;
	int recursive = 0;
	CSAntDir *dir;
	const char *filename;
	char *pathname;
	char *tempName;

	/* Pop items from the directory stack and scan them until
	   all matches have been processed */
	while(stack != 0)
	{
		/* Pop the top-most stack item */
		currentDir = stack->dirName;
		currentRegex = stack->regex;
		nextStack = stack->next;
		ILFree(stack);
		stack = nextStack;

		/* Create a new regular expression block if necessary */
		if(currentRegex)
		{
			if(haveRegex)
			{
				IL_regfree(&state);
			}
			recursive = BuildIncludeRegex(currentRegex, &state);
			haveRegex = 1;
		}

		/* Scan the directory for inclusions and exclusions */
		dir = CSAntDirOpen(currentDir, (const char *)0);
		if(dir)
		{
			while((filename = CSAntDirNext(dir)) != 0)
			{
				/* Always exclude CVS-related files and directories */
				if(!strcmp(filename, "CVS") || !strcmp(filename, ".cvsignore"))
				{
					continue;
				}

				/* Build the full pathname */
				pathname = CSAntDirCombine(currentDir, filename);

				/* Different processing is required for directories and files */
				if(recursive && IsDirectory(pathname))
				{
					/* Add/Remove to the fileset if we have a match */
					if(MatchInclude(pathname + strlen(currentDir) + 1, &state))
					{
						tempName = ILDupString(pathname);
						if(!tempName)
						{
							CSAntOutOfMemory();
						}
						if(addToFileSet)
						{
							AddToFileSet(fileset, tempName);
						}
						else
						{
							RemoveFromFileSet(fileset, tempName);
							ILFree(tempName);
						}
					}

					/* Add the directory to the stack for later processing */
					nextStack = (CSAntDirStack *)ILMalloc
									(sizeof(CSAntDirStack));
					if(!nextStack)
					{
						CSAntOutOfMemory();
					}
					nextStack->dirName = pathname;
					nextStack->regex = 0;
					nextStack->next = stack;
					stack = nextStack;
				}
				else
				{
					/* Add/Remove the pathname to the file set if it matches */
					if(MatchInclude(pathname + strlen(currentDir) + 1, &state))
					{
						if(addToFileSet)
						{
							AddToFileSet(fileset, pathname);
						}
						else
						{
							RemoveFromFileSet(fileset, pathname);
							ILFree(pathname);
						}
					}
					else
					{
						ILFree(pathname);
					}
				}
			}
			CSAntDirClose(dir);
		}

		/* Free the current directory name, which we no longer require */
		ILFree(currentDir);
	}

	/* Free the regular expression state */
	if(haveRegex)
	{
		IL_regfree(&state);
	}
}

CSAntFileSet *CSAntFileSetLoad(CSAntTask *task, const char *name,
							   const char *configBaseDir)
{
	CSAntFileSet *fileset;
	CSAntTask *node;
	const char *arg;
	const char *arg2;
	char *baseDir;
	char *pathname;
	CSAntDirStack *stack;

	/* Find the sub-node */
	node = task->taskChildren;
	while(node != 0 && strcmp(node->name, name) != 0)
	{
		node = node->next;
	}
	if(!node)
	{
		return 0;
	}

	/* Construct the starting base directory */
	arg = CSAntTaskParam(node, "basedir");
	if(arg)
	{
		baseDir = CSAntDirCombine(configBaseDir, arg);
	}
	else
	{
		baseDir = CSAntDirCombine(configBaseDir, 0);
	}

	/* Construct the file set */
	fileset = (CSAntFileSet *)ILMalloc(sizeof(CSAntFileSet));
	if(!fileset)
	{
		CSAntOutOfMemory();
	}
	fileset->numFiles = 0;
	fileset->maxFiles = 0;
	fileset->files = 0;

	/* Build the directory stack for the "includes" specification */
	stack = DirStackAddIncludes(fileset, node, "includes", baseDir, 1);

	/* Process the directory stack for the "includes" specification */
	ProcessDirStack(fileset, stack, 1);

	/* Build the directory stack for the "excludes" specification */
	stack = DirStackAddIncludes(fileset, node, "excludes", baseDir, 0);

	/* Process the directory stack for the "excludes" specification */
	ProcessDirStack(fileset, stack, 0);

	/* Add simple files that are named using the <file> tag */
	node = node->taskChildren;
	while(node != 0)
	{
		if(!strcmp(node->name, "file") && CheckIf(node))
		{
			arg = CSAntTaskParam(node, "name");
			if(arg)
			{
				arg2 = CSAntTaskParam(node, "basedir");
				if(arg2)
				{
					pathname = CSAntDirCombine(arg2, arg);
				}
				else
				{
					pathname = CSAntDirCombine(baseDir, arg);
				}
				AddToFileSet(fileset, pathname);
			}
		}
		node = node->next;
	}

	/* We don't need the base directory any more */
	ILFree(baseDir);

	/* Done */
	return fileset;
}

void CSAntFileSetDestroy(CSAntFileSet *fileset)
{
	unsigned long posn;
	if(!fileset)
	{
		return;
	}
	for(posn = 0; posn < fileset->numFiles; ++posn)
	{
		ILFree(fileset->files[posn]);
	}
	if(fileset->files)
	{
		ILFree(fileset->files);
	}
	ILFree(fileset);
}

unsigned long CSAntFileSetSize(CSAntFileSet *fileset)
{
	if(fileset)
	{
		return fileset->numFiles;
	}
	else
	{
		return 0;
	}
}

char *CSAntFileSetFile(CSAntFileSet *fileset, unsigned long num)
{
	if(fileset && num < fileset->numFiles)
	{
		return fileset->files[num];
	}
	else
	{
		return 0;
	}
}

int CSAntFileSetNewer(CSAntFileSet *fileset, const char *filename)
{
#ifdef HAVE_STAT
	struct stat st1;
	struct stat st2;
	unsigned long posn;

	/* Assume that the file is not newer if the fileset is non-existent */
	if(!fileset)
	{
		return 0;
	}

	/* If the file doesn't exist at all, then assume
	   that the fileset is newer */
	if(stat(filename, &st1) < 0)
	{
		return 1;
	}

	/* Process each of the files in the fileset in turn */
	for(posn = 0; posn < fileset->numFiles; ++posn)
	{
		if(stat(fileset->files[posn], &st2) >= 0 &&
		   st2.st_mtime > st1.st_mtime)
		{
			return 1;
		}
	}

	/* Not newer */
	return 0;
#else
	/* We don't know how to determine newness, so it is always new */
	return 1;
#endif
}

CSAntFileSet *CSAntFileSetAdd(CSAntFileSet *fileset, const char *filename)
{
	char *pathname;

	/* Construct a new file set if necessary */
	if(!fileset)
	{
		fileset = (CSAntFileSet *)ILMalloc(sizeof(CSAntFileSet));
		if(!fileset)
		{
			CSAntOutOfMemory();
		}
		fileset->numFiles = 0;
		fileset->maxFiles = 0;
		fileset->files = 0;
	}

	/* Duplicate the filename string */
	pathname = ILDupString(filename);
	if(!pathname)
	{
		CSAntOutOfMemory();
	}

	/* Add the filename to the file set */
	AddToFileSet(fileset, pathname);
	return fileset;
}

#ifdef	__cplusplus
};
#endif
