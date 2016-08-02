/*
 * ilfind.c - Find names within IL binaries.
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
#include "il_program.h"
#include "il_utils.h"
#include "il_regex.h"
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"--whole-string", 's', 0,
		"--whole-string or -w",
		"Search for a whole string (default)."},
	{"-w", 'w', 0, 0, 0},
	{"--sub-string", 's', 0,
		"--sub-string   or -s",
		"Search for a sub-string."},
	{"-s", 's', 0, 0, 0},
	{"--file-regex", 'f', 0,
		"--file-regex   or -f",
		"Use a filename regular expression form (default)."},
	{"-f", 'f', 0, 0, 0},
	{"--grep-regex", 'g', 0,
		"--grep-regex   or -g",
		"Use a grep regular expression form."},
	{"-g", 'g', 0, 0, 0},
	{"--no-regex", 'r', 0,
		"--no-regex     or -n",
		"Do not use regular expression matching."},
	{"-n", 'n', 0, 0, 0},
	{"--ignore-case", 'i', 0,
		"--ignore-case  or -i",
		"Ignore case when matching."},
	{"-i", 'i', 0, 0, 0},
	{"--public-only", 'p', 0,
		"--public-only  or -p",
		"Search only the classes that are publicly accessible."},
	{"-p", 'p', 0, 0, 0},
	{"--version", 'v', 0,
		"--version      or -v",
		"Print the version of the program."},
	{"-v", 'v', 0, 0, 0},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

static void usage(const char *progname);
static void version(void);
static int searchFile(const char *filename, ILContext *context,
					  int reportFilenames, int publicOnly);
static void CompileRegex(char *searchString, int wholeString,
						 int regexMatching, int ignoreCase,
						 int fileRegex);
static int MatchRegex(const char *string);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	int wholeString = 1;
	int regexMatching = 1;
	int ignoreCase = 0;
	int fileRegex = 1;
	int publicOnly = 0;
	char *searchString;
	int reportFilenames;
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
			case 'w':
			{
				wholeString = 1;
			}
			break;

			case 's':
			{
				wholeString = 0;
			}
			break;

			case 'f':
			{
				fileRegex = 1;
				regexMatching = 1;
			}
			break;

			case 'g':
			{
				fileRegex = 0;
				regexMatching = 1;
			}
			break;

			case 'n':
			{
				fileRegex = 1;
				regexMatching = 0;
			}
			break;

			case 'i':
			{
				ignoreCase = 1;
			}
			break;

			case 'p':
			{
				publicOnly = 1;
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

	/* We need at least two arguments */
	if(argc <= 2)
	{
		usage(progname);
		return 1;
	}
	searchString = argv[1];
	--argc;
	++argv;

	/* Compile the regular expression for later matching */
	CompileRegex(searchString, wholeString, regexMatching,
				 ignoreCase, fileRegex);

	/* Create a context to use for image loading */
	context = ILContextCreate();
	if(!context)
	{
		fprintf(stderr, "%s: out of memory\n", progname);
		return 1;
	}

	/* Load and search the input files */
	sawStdin = 0;
	errors = 0;
	reportFilenames = (argc > 2);
	while(argc > 1)
	{
		if(!strcmp(argv[1], "-"))
		{
			/* Dump the contents of stdin, but only once */
			if(!sawStdin)
			{
				errors |= searchFile("-", context, reportFilenames, publicOnly);
				sawStdin = 1;
			}
		}
		else
		{
			/* Dump the contents of a regular file */
			errors |= searchFile(argv[1], context, reportFilenames, publicOnly);
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
	fprintf(stdout, "ILFIND " VERSION " - IL Image Name Find Utility\n");
	fprintf(stdout, "Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] pattern input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("ILFIND " VERSION " - IL Image Name Find Utility\n");
	printf("Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILFIND comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Search for a match within a member table (fields, methods, etc).
 */
static void searchForMember(const char *filename, int reportFilenames,
							ILImage *image, unsigned long tokenType,
							const char *type)
{
	unsigned long numTokens;
	unsigned long token;
	ILMember *member;
	ILClass *info;

	numTokens = ILImageNumTokens(image, tokenType);
	for(token = 1; token <= numTokens; ++token)
	{
		member = (ILMember *)(ILImageTokenInfo(image, token | tokenType));
		if(member && MatchRegex(ILMember_Name(member)))
		{
			if(reportFilenames)
			{
				fputs(filename, stdout);
				fputs(": ", stdout);
			}
			fputs(type, stdout);
			putc(' ', stdout);
			info = ILMember_Owner(member);
			if(ILClass_Namespace(info))
			{
				fputs(ILClass_Namespace(info), stdout);
				putc('.', stdout);
			}
			fputs(ILClass_Name(info), stdout);
			putc('.', stdout);
			fputs(ILMember_Name(member), stdout);
			putc('\n', stdout);
		}
	}
}

/*
 * Structure of a namespace list entry.
 */
typedef struct _tagNamespaceEntry NamespaceEntry;
struct _tagNamespaceEntry
{
	const char     *namespace;
	NamespaceEntry *next;

};

/*
 * Load an IL image from an input stream and search it.
 */
static int searchFile(const char *filename, ILContext *context,
					  int reportFilenames, int publicOnly)
{
	ILImage *image;
	unsigned long numTokens;
	unsigned long token;
	ILAssembly *assembly;
	ILClass *info;
	const char *namespace;
	NamespaceEntry *entries = 0;
	NamespaceEntry *entry;
	char *temp;

	/* Attempt to load the image into memory */
	if(ILImageLoadFromFile(filename, context, &image,
						   IL_LOADFLAG_FORCE_32BIT |
						   IL_LOADFLAG_NO_RESOLVE, 1) != 0)
	{
		return 1;
	}

	/* Use a more descriptive name for stdin from now on */
	if(!strcmp(filename, "-"))
	{
		filename = "stdin";
	}

	/* Search the Assembly table for assembly names */
	numTokens = ILImageNumTokens(image, IL_META_TOKEN_ASSEMBLY);
	for(token = 1; token <= numTokens; ++token)
	{
		assembly = (ILAssembly *)(ILImageTokenInfo
						(image, token | IL_META_TOKEN_ASSEMBLY));
		if(assembly && MatchRegex(ILAssembly_Name(assembly)))
		{
			if(reportFilenames)
			{
				fputs(filename, stdout);
				fputs(": ", stdout);
			}
			printf("assembly %s\n", ILAssembly_Name(assembly));
		}
	}

	/* Search the TypeDef table for class names and namespaces */
	numTokens = ILImageNumTokens(image, IL_META_TOKEN_TYPE_DEF);
	for(token = 1; token <= numTokens; ++token)
	{
		info = (ILClass *)(ILImageTokenInfo
						(image, token | IL_META_TOKEN_TYPE_DEF));
		if(info)
		{
			if(publicOnly && !ILClass_IsPublic(info))
			{
				continue;
			}
			namespace = ILClass_Namespace(info);
			if(MatchRegex(ILClass_Name(info)))
			{
				/* Exact match on the name */
				if(reportFilenames)
				{
					fputs(filename, stdout);
					fputs(": ", stdout);
				}
				fputs("class ", stdout);
				if(namespace)
				{
					fputs(namespace, stdout);
					putc('.', stdout);
				}
				fputs(ILClass_Name(info), stdout);
				putc('\n', stdout);
			}
			else if(namespace)
			{
				/* Try to match the namespace */
				if(MatchRegex(namespace))
				{
					/* We report namespaces once per file */
					entry = entries;
					while(entry != 0 &&
					      strcmp(entry->namespace, namespace) != 0)
					{
						entry = entry->next;
					}
					if(entry == 0)
					{
						entry = (NamespaceEntry *)
							(ILMalloc(sizeof(NamespaceEntry)));
						if(entry != 0)
						{
							entry->namespace = namespace;
							entry->next = entries;
							entries = entry;
							if(reportFilenames)
							{
								fputs(filename, stdout);
								fputs(": ", stdout);
							}
							fputs("namespace ", stdout);
							fputs(namespace, stdout);
							putc('\n', stdout);
						}
					}
				}

				/* Try to match on "namespace.name" */
				if((temp = (char *)ILMalloc(strlen(ILClass_Name(info)) +
											strlen(namespace) + 2)) != 0)
				{
					strcpy(temp, namespace);
					strcat(temp, ".");
					strcat(temp, ILClass_Name(info));
					if(MatchRegex(temp))
					{
						if(reportFilenames)
						{
							fputs(filename, stdout);
							fputs(": ", stdout);
						}
						fputs("class ", stdout);
						fputs(temp, stdout);
						putc('\n', stdout);
					}
					ILFree(temp);
				}
			}
		}
	}

	/* Search the FieldDef table for field names */
	searchForMember(filename, reportFilenames, image,
					IL_META_TOKEN_FIELD_DEF, "field");

	/* Search the MethodDef table for method names */
	searchForMember(filename, reportFilenames, image,
					IL_META_TOKEN_METHOD_DEF, "method");

	/* Search the Event table for event names */
	searchForMember(filename, reportFilenames, image,
					IL_META_TOKEN_EVENT, "event");

	/* Search the Property table for property names */
	searchForMember(filename, reportFilenames, image,
					IL_META_TOKEN_PROPERTY, "property");

	/* Clean up and exit */
	ILImageDestroy(image);
	while(entries != 0)
	{
		entry = entries->next;
		ILFree(entries);
		entries = entry;
	}
	return 0;
}

static regex_t RegexState;

/*
 * Compile a regular expression for the search string.
 */
static void CompileRegex(char *searchString, int wholeString,
						 int regexMatching, int ignoreCase,
						 int fileRegex)
{
	char *regexp;
	char *out;
	char lastch;
	int libIgnoreCase = 0;

	/* Allocate temporary space for the full regular expression */
	if((regexp = (char *)ILMalloc(strlen(searchString) * 4 + 20)) == 0)
	{
		fprintf(stderr, "virtual memory exhausted\n");
		exit(1);
	}

	/* Build the full regular expression */
	out = regexp;
	if(wholeString)
	{
		*out++ = '^';
		if(!fileRegex && regexMatching && *searchString == '^')
		{
			++searchString;
		}
	}
	lastch = '\0';
	if(!regexMatching)
	{
		/* Convert a literal string into a regular expression */
		while(*searchString != '\0')
		{
			lastch = *searchString++;
			if(lastch == '.' || lastch == '^' || lastch == '$' ||
			   lastch == '*' || lastch == '[' || lastch == ']' ||
			   lastch == '\\')
			{
				*out++ = '\\';
				*out++ = lastch;
			}
			else if(ignoreCase && lastch >= 'A' && lastch <= 'Z')
			{
				*out++ = '[';
				*out++ = lastch;
				*out++ = lastch - 'A' + 'a';
				*out++ = ']';
			}
			else if(ignoreCase && lastch >= 'a' && lastch <= 'z')
			{
				*out++ = '[';
				*out++ = lastch - 'a' + 'A';
				*out++ = lastch;
				*out++ = ']';
			}
			else
			{
				*out++ = lastch;
			}
		}
		lastch = '\0';
	}
	else if(fileRegex)
	{
		/* Convert the regular expression from file form to grep form */
		while(*searchString != '\0')
		{
			lastch = *searchString++;
			if(lastch == '?')
			{
				*out++ = '.';
			}
			else if(lastch == '*')
			{
				*out++ = '.';
				*out++ = '*';
			}
			else if(lastch == '[')
			{
				*out++ = '[';
				while(*searchString != '\0')
				{
					lastch = *searchString++;
					if(lastch == ']')
					{
						break;
					}
					*out++ = lastch;
				}
				*out++ = ']';
			}
			else if(lastch == '.' || lastch == '^' || lastch == '$' ||
					lastch == ']' || lastch == '\\')
			{
				*out++ = '\\';
				*out++ = lastch;
			}
			else if(ignoreCase && lastch >= 'A' && lastch <= 'Z')
			{
				*out++ = '[';
				*out++ = lastch;
				*out++ = lastch - 'A' + 'a';
				*out++ = ']';
			}
			else if(ignoreCase && lastch >= 'a' && lastch <= 'z')
			{
				*out++ = '[';
				*out++ = lastch - 'a' + 'A';
				*out++ = lastch;
				*out++ = ']';
			}
			else
			{
				*out++ = lastch;
			}
		}
		lastch = '\0';
	}
	else
	{
		/* Copy the regular expression as-is */
		while(*searchString != '\0')
		{
			lastch = *searchString++;
			*out++ = lastch;
		}

		/* Let the regular expression library handle case issues */
		libIgnoreCase = ignoreCase;
	}
	if(wholeString)
	{
		if(fileRegex || !regexMatching || lastch != '$')
		{
			*out++ = '$';
		}
	}
	*out = '\0';

	/* Compile the regular expression */
	if(IL_regcomp(&RegexState, regexp, REG_EXTENDED | REG_NOSUB |
									   (libIgnoreCase ? REG_ICASE : 0)) != 0)
	{
		fprintf(stderr, "Invalid regular expression\n");
		exit(1);
	}
}

/*
 * Test for a regular expression match.
 */
static int MatchRegex(const char *string)
{
	return (IL_regexec(&RegexState, string, 0, 0, 0) == 0);
}

#ifdef	__cplusplus
};
#endif
