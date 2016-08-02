/*
 * loc2cul.c - Convert IBM locale files into culture handling classes.
 *
 * Copyright (c) 2003  Southern Storm Software, Pty Ltd
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

Usage: loc2cul [options] file

	--region name			I18N region name
	--id num				Culture identifier number
	--name str				Human-readable culture name (e.g. "en-US")
	--alias str				Human-readable alias name
	--root					Generate definitions for the root culture
	--iso2 str				Two-letter ISO name
	--iso3 str				Three-letter ISO name
	--windows str			Three-letter Windows name
	--ansi num				ANSI code page number
	--ebcdic num			EBCDIC code page number
	--mac num				Mac code page number
	--oem num				OEM code page number
	--separator str			List separator

*/

#include <stdio.h>
#include <string.h>
#include <stdlib.h>

/*
 * Option values.
 */
static char *region = 0;
static int identifier = 0;
static char *name = 0;
static char *alias = 0;
static char *iso2 = 0;
static char *iso3 = 0;
static char *windows = 0;
static const char *filename = 0;
static int ansiCP = 1252;
static int ebcdicCP = 37;
static int macCP = 10000;
static int oemCP = 437;
static const char *separator = ",";

/*
 * Forward declarations.
 */
static void usage(char *progname);
static void loadLocaleDefinition(FILE *file);
/*static void dumpLocaleDefinition(FILE *file);*/
static void printHeader(void);
static void printFooter(void);
static void printDefinition(void);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	FILE *file;
	int len;
	int isRoot = 0;

	/* Process the command-line options */
	while(argc > 1 && argv[1][0] == '-')
	{
		if(!strcmp(argv[1], "--id") && argc > 2)
		{
			identifier = strtol(argv[2], NULL, 0);
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--region") && argc > 2)
		{
			region = argv[2];
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--name") && argc > 2)
		{
			name = argv[2];
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--alias") && argc > 2)
		{
			alias = argv[2];
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--iso2") && argc > 2)
		{
			iso2 = argv[2];
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--iso3") && argc > 2)
		{
			iso3 = argv[2];
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--windows") && argc > 2)
		{
			windows = argv[2];
			++argv;
		}
		else if(!strcmp(argv[1], "--root"))
		{
			isRoot = 1;
		}
		else if(!strcmp(argv[1], "--ansi") && argc > 2)
		{
			ansiCP = strtol(argv[2], NULL, 0);
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--ebcdic") && argc > 2)
		{
			ebcdicCP = strtol(argv[2], NULL, 0);
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--mac") && argc > 2)
		{
			macCP = strtol(argv[2], NULL, 0);
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--oem") && argc > 2)
		{
			oemCP = strtol(argv[2], NULL, 0);
			++argv;
			--argc;
		}
		else if(!strcmp(argv[1], "--separator") && argc > 2)
		{
			separator = argv[2];
			++argv;
			--argc;
		}
		++argv;
		--argc;
	}

	/* Make sure that we have sufficient options */
	if(!region || (!identifier && !isRoot) || !name)
	{
		usage(progname);
		return 1;
	}

	/* Open the locale definition file */
	file = fopen(argv[1], "r");
	if(!file)
	{
		perror(argv[1]);
		return 1;
	}
	filename = argv[1];
	len = strlen(filename);
	while(len > 0 && filename[len - 1] != '/' && filename[len - 1] != '\\')
	{
		--len;
	}
	filename += len;

	/* Load the locale definition information from the file */
	loadLocaleDefinition(file);
	/*dumpLocaleDefinition(stderr);*/

	/* Print the output header */
	printHeader();

	/* Print the definition */
	printDefinition();

	/* Print the output footer */
	printFooter();

	/* Clean up and exit */
	fclose(file);
	return 0;
}

static void usage(char *progname)
{
	fprintf(stderr, "Usage: %s [options] file\n\n", progname);
	fprintf(stderr, "    --region name    I18N region name\n");
	fprintf(stderr, "    --id num         Culture identifier number\n");
	fprintf(stderr, "    --name str       Human-readable culture name\n");
	fprintf(stderr, "    --alias str      Human-readable alias name\n");
	fprintf(stderr, "    --root           Generate definitions for the root culture\n");
	fprintf(stderr, "    --iso2 str       Two-letter ISO name\n");
	fprintf(stderr, "    --iso3 str       Three-letter ISO name\n");
	fprintf(stderr, "    --windows str    Three-letter Windows name\n");
}

/*
 * Token types.
 */
#define	TOKEN_EOF		0
#define	TOKEN_IDENT		1
#define	TOKEN_STRING	2
#define	TOKEN_LBRACE	3
#define	TOKEN_RBRACE	4
#define	TOKEN_COLON		5
#define	TOKEN_INTEGER	6
#define	TOKEN_COMMA		7

/*
 * Add a character to a string.
 */
static void AddChar(char **str, int ch)
{
	int len;
	if(!(*str))
	{
		*str = (char *)(malloc(16));
		if(!(*str))
		{
			fputs("out of memory\n", stderr);
			exit(1);
		}
		len = 0;
	}
	else if((strlen(*str) % 16) == 0)
	{
		*str = (char *)(realloc(*str, strlen(*str) + 16));
		if(!(*str))
		{
			fputs("out of memory\n", stderr);
			exit(1);
		}
		len = strlen(*str);
	}
	else
	{
		len = strlen(*str);
	}
	(*str)[len] = (char)ch;
	(*str)[len + 1] = '\0';
}

/*
 * Read a token from the input stream.
 */
static int readToken(FILE *file, char **name)
{
	int ch;

	/* Skip white space and comments */
	while((ch = getc(file)) != EOF)
	{
		if(ch == '\t' || ch == ' ' || ch == '\r' || ch == '\n')
		{
			continue;
		}
		else if(ch == '/')
		{
			ch = getc(file);
			if(ch != '/')
			{
				ungetc(ch, file);
				ch = '/';
				break;
			}
			else
			{
				while((ch = getc(file)) != EOF && ch != '\n')
				{
					/* Nothing to do here */
				}
				if(ch == EOF)
				{
					break;
				}
			}
		}
		else
		{
			break;
		}
	}
	if(ch == EOF)
	{
		return TOKEN_EOF;
	}

	/* Process the token */
	*name = 0;
	if(ch == '{')
	{
		return TOKEN_LBRACE;
	}
	else if(ch == '}')
	{
		return TOKEN_RBRACE;
	}
	else if(ch == ':')
	{
		return TOKEN_COLON;
	}
	else if(ch == ',')
	{
		return TOKEN_COMMA;
	}
	else if(ch == '"')
	{
		/* Parse a string */
		while((ch = getc(file)) != EOF && ch != '"')
		{
			AddChar(name, ch);
		}
		AddChar(name, '\0');
		return TOKEN_STRING;
	}
	else if(ch >= '0' && ch <= '9')
	{
		/* Parse an integer */
		AddChar(name, ch);
		while((ch = getc(file)) != EOF)
		{
			if(ch >= '0' && ch <= '9')
			{
				AddChar(name, ch);
			}
			else if(ch >= 'A' && ch <= 'F')
			{
				AddChar(name, ch);
			}
			else if(ch >= 'a' && ch <= 'f')
			{
				AddChar(name, ch);
			}
			else if(ch == 'x' || ch == 'X')
			{
				AddChar(name, ch);
			}
			else
			{
				ungetc(ch, file);
				break;
			}
		}
		AddChar(name, '\0');
		return TOKEN_INTEGER;
	}
	else
	{
		/* Parse an identifier */
		AddChar(name, ch);
		while((ch = getc(file)) != EOF)
		{
			if(ch >= '0' && ch <= '9')
			{
				AddChar(name, ch);
			}
			else if(ch >= 'A' && ch <= 'Z')
			{
				AddChar(name, ch);
			}
			else if(ch >= 'a' && ch <= 'z')
			{
				AddChar(name, ch);
			}
			else if(ch == '_')
			{
				AddChar(name, ch);
			}
			else
			{
				ungetc(ch, file);
				break;
			}
		}
		AddChar(name, '\0');
		return TOKEN_IDENT;
	}
}

/*
 * Structure of a node.
 */
typedef struct _tagNode Node;
struct _tagNode
{
	char	*name;
	char	*type;
	Node	*children;
	Node	*parent;
	Node	*next;

};

/*
 * Top-most node in the node tree.
 */
static Node *topNode = 0;

/*
 * Create a new node.
 */
static Node *createNode(char *name, Node *parent)
{
	Node *node = (Node *)malloc(sizeof(Node));
	if(!node)
	{
		fputs("out of memory\n", stderr);
		exit(1);
	}
	node->name = name;
	node->type = 0;
	node->children = 0;
	node->parent = parent;
	node->next = 0;
	if(parent && !(parent->children))
	{
		parent->children = node;
	}
	return node;
}

/*
 * Load the locale definition from a file into memory.
 *
 *    File ::= { TagDecl }
 *    TagDecl ::= TagName '{' { Content } '}'
 *	  TagName ::= IDENT | STRING | INTEGER | TagName ':' TagName
 *    Content ::= ',' | IDENT | STRING | INTEGER | TagDecl
 */
static void loadLocaleDefinition(FILE *file)
{
	int token;
	char *name;
	Node *parent = 0;
	Node *node = 0;
	Node *temp;
	while((token = readToken(file, &name)) != TOKEN_EOF)
	{
		switch(token)
		{
			case TOKEN_IDENT:
			case TOKEN_STRING:
			case TOKEN_INTEGER:
			{
				/* Add a new node to the tree */
				if(!node)
				{
					node = createNode(name, parent);
				}
				else
				{
					temp = createNode(name, parent);
					node->next = temp;
					node = temp;
				}
				if(!topNode)
				{
					topNode = node;
				}
			}
			break;

			case TOKEN_LBRACE:
			{
				/* Enter a sub context */
				parent = node;
				node = 0;
			}
			break;

			case TOKEN_RBRACE:
			{
				/* Exit from the current sub context */
				node = parent;
				if(node)
				{
					parent = node->parent;
				}
				else
				{
					parent = 0;
				}
			}
			break;

			case TOKEN_COLON:
			{
				/* Read type information for the current node */
				token = readToken(file, &name);
				if(node)
				{
					node->type = name;
				}
			}
			break;

			case TOKEN_COMMA:	break;	/* Skip commas */
		}
	}
}

#if 0

/*
 * Dump a locale definition, for debugging.
 */
static void dumpLocaleDefinition(FILE *file)
{
	Node *node = topNode;
	int level = 0;
	int temp;
	while(node != 0)
	{
		for(temp = 0; temp < level; ++temp)
		{
			putc('\t', file);
		}
		fputs(node->name, file);
		if(node->type)
		{
			putc(':', file);
			fputs(node->type, file);
		}
		if(node->children)
		{
			fputs(" {\n", file);
			node = node->children;
			++level;
		}
		else if(node->next)
		{
			putc('\n', file);
			node = node->next;
		}
		else
		{
			putc('\n', file);
			--level;
			for(temp = 0; temp < level; ++temp)
			{
				putc('\t', file);
			}
			fputs("}\n", file);
			node = node->parent;
			if(node)
			{
				while(node && node->next == 0)
				{
					--level;
					for(temp = 0; temp < level; ++temp)
					{
						putc('\t', file);
					}
					fputs("}\n", file);
					node = node->parent;
				}
				if(node)
				{
					node = node->next;
				}
			}
		}
	}
}

#endif

#define	COPYRIGHT_MSG \
" *\n" \
" * Copyright (c) 2003  Southern Storm Software, Pty Ltd\n" \
" *\n" \
" * This program is free software; you can redistribute it and/or modify\n" \
" * it under the terms of the GNU General Public License as published by\n" \
" * the Free Software Foundation; either version 2 of the License, or\n" \
" * (at your option) any later version.\n" \
" *\n" \
" * This program is distributed in the hope that it will be useful,\n" \
" * but WITHOUT ANY WARRANTY; without even the implied warranty of\n" \
" * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\n" \
" * GNU General Public License for more details.\n" \
" *\n" \
" * You should have received a copy of the GNU General Public License\n" \
" * along with this program; if not, write to the Free Software\n" \
" * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA\n" \
" */\n\n"

/*
 * Print the header for the current culture definition.
 */
static void printHeader(void)
{
	if(identifier != 0)
	{
		printf("/*\n * CID%04x.cs - %s culture handler.\n", identifier, name);
	}
	else
	{
		printf("/*\n * RootCulture.cs - root culture handler.\n");
	}
	fputs(COPYRIGHT_MSG, stdout);
	printf("// Generated from \"%s\".\n\n", filename);
	printf("namespace I18N.%s\n{\n\n", region);
	printf("using System;\n");
	printf("using System.Globalization;\n");
	if(strcmp(region, "Common") != 0)
	{
		printf("using I18N.Common;\n");
	}
	printf("\n");
	if(identifier == 0)
	{
		/* This is the root culture, containing fallback defaults
		   for all subsequent culture definitions.  Note: 0x40000000
		   indicates to "CultureInfo" that this is an inherited
		   definition, and that it should not attempt to recursively
		   load the culture again */
		printf("public abstract class RootCulture : CultureInfo\n{\n");
		printf("\tpublic RootCulture(int culture) ");
		printf(": base(0x40000000 + culture) {}\n\n");

		/* Override the name properties */
		printf("\tpublic override String DisplayName\n");
		printf("\t{\n");
		printf("\t\tget\n");
		printf("\t\t{\n");
		printf("\t\t\treturn Manager.GetDisplayName(this);\n");
		printf("\t\t}\n");
		printf("\t}\n");
		printf("\tpublic override String EnglishName\n");
		printf("\t{\n");
		printf("\t\tget\n");
		printf("\t\t{\n");
		printf("\t\t\treturn Manager.GetEnglishName(this);\n");
		printf("\t\t}\n");
		printf("\t}\n");
		printf("\tpublic override String NativeName\n");
		printf("\t{\n");
		printf("\t\tget\n");
		printf("\t\t{\n");
		printf("\t\t\treturn Manager.GetNativeName(this);\n");
		printf("\t\t}\n");
		printf("\t}\n");

		/* Declare the "Country" property for the root */
		printf("\tpublic virtual String Country\n");
		printf("\t{\n");
		printf("\t\tget\n");
		printf("\t\t{\n");
		printf("\t\t\treturn null;\n");
		printf("\t\t}\n");
		printf("\t}\n\n");
	}
	else if((identifier & 0x03FF) == identifier)
	{
		/* This is a neutral culture: inherit from RootCulture */
		printf("public class CID%04x : RootCulture\n{\n", identifier);
		printf("\tpublic CID%04x() ", identifier);
		printf(": base(0x%04X) {}\n", identifier);
		printf("\tpublic CID%04x(int culture) ", identifier);
		printf(": base(culture) {}\n\n");
	}
	else
	{
		/* Specific cultures inherit from the neutral version so
		   as to pick up default fallback behaviours */
		printf("public class CID%04x : CID%04x\n{\n",
			   identifier, (identifier & 0x03FF));
		printf("\tpublic CID%04x() ", identifier);
		printf(": base(0x%04X) {}\n\n", identifier);
	}
	if(identifier != 0)
	{
		/* Declare the name properties */
		printf("\tpublic override String Name\n");
		printf("\t{\n");
		printf("\t\tget\n");
		printf("\t\t{\n");
		printf("\t\t\treturn \"%s\";\n", name);
		printf("\t\t}\n");
		printf("\t}\n");
		if(iso3)
		{
			printf("\tpublic override String ThreeLetterISOLanguageName\n");
			printf("\t{\n");
			printf("\t\tget\n");
			printf("\t\t{\n");
			printf("\t\t\treturn \"%s\";\n", iso3);
			printf("\t\t}\n");
			printf("\t}\n");
		}
		if(windows)
		{
			printf("\tpublic override String ThreeLetterWindowsLanguageName\n");
			printf("\t{\n");
			printf("\t\tget\n");
			printf("\t\t{\n");
			printf("\t\t\treturn \"%s\";\n", windows);
			printf("\t\t}\n");
			printf("\t}\n");
		}
		if(iso2)
		{
			printf("\tpublic override String TwoLetterISOLanguageName\n");
			printf("\t{\n");
			printf("\t\tget\n");
			printf("\t\t{\n");
			printf("\t\t\treturn \"%s\";\n", iso2);
			printf("\t\t}\n");
			printf("\t}\n");
		}

		/* Declare the "Country" property for a culture */
		if(!strcmp(name, "kok-IN"))
		{
			printf("\tpublic override String Country\n");
			printf("\t{\n");
			printf("\t\tget\n");
			printf("\t\t{\n");
			printf("\t\t\treturn \"IN\";\n");
			printf("\t\t}\n");
			printf("\t}\n");
		}
		else if(strlen(name) > 2 && strcmp(name, "kok") != 0)
		{
			printf("\tpublic override String Country\n");
			printf("\t{\n");
			printf("\t\tget\n");
			printf("\t\t{\n");
			printf("\t\t\treturn \"%c%c\";\n", name[3], name[4]);
			printf("\t\t}\n");
			printf("\t}\n");
		}
		printf("\n");
	}
}

/*
 * Print an encoding name, adjusted to look like a type name.
 */
static void printEncodingName(const char *name)
{
	while(*name != '\0')
	{
		if(*name >= 'A' && *name <= 'Z')
		{
			putc(*name - 'A' + 'a', stdout);
		}
		else if(*name == '-')
		{
			putc('_', stdout);
		}
		else
		{
			putc(*name, stdout);
		}
		++name;
	}
}

/*
 * Print the footer for the current culture definition.
 */
static void printFooter(void)
{
	if(identifier != 0)
	{
		printf("}; // class CID%04x\n\n", identifier);
		printf("public class CN");
		printEncodingName(name);
		printf(" : CID%04x\n{\n", identifier);
		printf("\tpublic CN");
		printEncodingName(name);
		printf("() : base() {}\n\n");
		printf("}; // class CN");
		printEncodingName(name);
		if(alias)
		{
			printf("\n\npublic class CN");
			printEncodingName(alias);
			printf(" : CID%04x\n{\n", identifier);
			printf("\tpublic CN");
			printEncodingName(alias);
			printf("() : base() {}\n\n");
			printf("}; // class CN");
			printEncodingName(alias);
		}
	}
	else
	{
		printf("}; // class RootCulture");
	}
	printf("\n\n}; // namespace I18N.%s\n", region);
}

/*
 * Get a top-level definition node with a specific name.
 */
static Node *getNode(const char *name)
{
	Node *node = topNode;
	if(node)
	{
		node = node->children;
	}
	while(node != 0 && strcmp(node->name, name) != 0)
	{
		node = node->next;
	}
	return node;
}

/*
 * Get the string value of an indexed child node.
 */
static const char *getNodeByIndex(Node *node, int index)
{
	node = node->children;
	while(node != 0 && index > 0)
	{
		node = node->next;
		--index;
	}
	if(node)
	{
		return node->name;
	}
	else
	{
		return "";
	}
}

/*
 * Determine if a node has a child with a specific index.
 */
static int hasNodeByIndex(Node *node, int index)
{
	if(!node)
	{
		return 0;
	}
	node = node->children;
	while(node != 0 && index > 0)
	{
		node = node->next;
		--index;
	}
	return (node != 0);
}

/*
 * Print a list of strings, padded to a specific length.
 */
static void printStringList(Node *node, int length)
{
	int first = 1;
	node = node->children;
	while(node != 0)
	{
		if(first)
		{
			first = 0;
			printf("\"%s\"", node->name);
		}
		else
		{
			printf(", \"%s\"", node->name);
		}
		node = node->next;
		--length;
	}
	while(length > 0)
	{
		if(first)
		{
			first = 0;
			printf("\"\"");
		}
		else
		{
			printf(", \"\"");
		}
		--length;
	}
}

/*
 * Modify an ICU-style DateTime pattern into a C#-style pattern.
 */
static const char *modifyPattern(const char *pattern)
{
	char *newPattern = 0;
	while(*pattern != '\0')
	{
		if(*pattern == 'E')
		{
			AddChar(&newPattern, 'd');
		}
		else if(*pattern == 'a')
		{
			AddChar(&newPattern, 't');
			AddChar(&newPattern, 't');
		}
		else
		{
			AddChar(&newPattern, *pattern);
		}
		++pattern;
	}
	AddChar(&newPattern, '\0');
	return newPattern;
}

/*
 * Get a separator character from a date or time string.
 */
static int getSeparator(const char *pattern, int defaultValue)
{
	while(*pattern != 0)
	{
		if(*pattern >= 'A' && *pattern <= 'Z')
		{
			++pattern;
			continue;
		}
		else if(*pattern >= 'a' && *pattern <= 'z')
		{
			++pattern;
			continue;
		}
		else if(*pattern == ' ')
		{
			++pattern;
			continue;
		}
		else
		{
			return *pattern;
		}
	}
	return defaultValue;
}

/*
 * Print the DateTime format information.
 */
static void printDateTimeFormat(void)
{
	Node *ampm;
	Node *patterns;
	Node *dayAbbrev;
	Node *dayNames;
	Node *monthAbbrev;
	Node *monthNames;

	/* Load the format information */
	ampm = getNode("AmPmMarkers");
	patterns = getNode("DateTimePatterns");
	dayAbbrev = getNode("DayAbbreviations");
	dayNames = getNode("DayNames");
	monthAbbrev = getNode("MonthAbbreviations");
	monthNames = getNode("MonthNames");

	/* Do we need to override the DateTime format information? */
	if(!ampm && !patterns && !dayAbbrev && !dayNames &&
	   !monthAbbrev && !monthNames)
	{
		return;
	}

	/* Output the "DateTimeFormat" property information */
	printf("\tpublic override DateTimeFormatInfo DateTimeFormat\n");
	printf("\t{\n");
	printf("\t\tget\n");
	printf("\t\t{\n");
	if(!identifier)
	{
		printf("\t\t\tDateTimeFormatInfo dfi = new DateTimeFormatInfo();\n");
	}
	else
	{
		printf("\t\t\tDateTimeFormatInfo dfi = base.DateTimeFormat;\n");
	}
	if(ampm)
	{
		printf("\t\t\tdfi.AMDesignator = \"%s\";\n", getNodeByIndex(ampm, 0));
		printf("\t\t\tdfi.PMDesignator = \"%s\";\n", getNodeByIndex(ampm, 1));
	}
	if(dayAbbrev)
	{
		printf("\t\t\tdfi.AbbreviatedDayNames = new String[] {");
		printStringList(dayAbbrev, 7);
		printf("};\n");
	}
	if(dayNames)
	{
		printf("\t\t\tdfi.DayNames = new String[] {");
		printStringList(dayNames, 7);
		printf("};\n");
	}
	if(monthAbbrev)
	{
		printf("\t\t\tdfi.AbbreviatedMonthNames = new String[] {");
		printStringList(monthAbbrev, 13);
		printf("};\n");
	}
	if(monthNames)
	{
		printf("\t\t\tdfi.MonthNames = new String[] {");
		printStringList(monthNames, 13);
		printf("};\n");
	}
	if(patterns)
	{
	#define	TIME_PATTERN_FULL		0
	#define	TIME_PATTERN_LONG		1
	#define	TIME_PATTERN_MEDIUM		2
	#define	TIME_PATTERN_SHORT		3
	#define	DATE_PATTERN_FULL		4
	#define	DATE_PATTERN_LONG		5
	#define	DATE_PATTERN_MEDIUM		6
	#define	DATE_PATTERN_SHORT		7
		int dateSep = getSeparator
			(getNodeByIndex(patterns, DATE_PATTERN_SHORT), '/');
		int timeSep = getSeparator
			(getNodeByIndex(patterns, TIME_PATTERN_SHORT), ':');
		printf("\t\t\tdfi.DateSeparator = \"%c\";\n", dateSep);
		printf("\t\t\tdfi.TimeSeparator = \"%c\";\n", timeSep);
		printf("\t\t\tdfi.LongDatePattern = \"%s\";\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_LONG)));
		printf("\t\t\tdfi.LongTimePattern = \"%s\";\n",
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_LONG)));
		printf("\t\t\tdfi.ShortDatePattern = \"%s\";\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_SHORT)));
		printf("\t\t\tdfi.ShortTimePattern = \"%s\";\n",
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_SHORT)));
		printf("\t\t\tdfi.FullDateTimePattern = \"%s %s\";\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_FULL)),
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_FULL)));
		printf("\t\t\tdfi.I18NSetDateTimePatterns(new String[] {\n");
		printf("\t\t\t\t\"d:%s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_SHORT)));
		printf("\t\t\t\t\"D:%s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_FULL)));
		printf("\t\t\t\t\"f:%s %s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_FULL)),
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_FULL)));
		printf("\t\t\t\t\"f:%s %s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_FULL)),
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_LONG)));
		printf("\t\t\t\t\"f:%s %s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_FULL)),
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_MEDIUM)));
		printf("\t\t\t\t\"f:%s %s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_FULL)),
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_SHORT)));
		printf("\t\t\t\t\"F:%s HH%cmm%css\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_FULL)),
			   timeSep, timeSep);
		printf("\t\t\t\t\"g:%s %s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_SHORT)),
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_FULL)));
		printf("\t\t\t\t\"g:%s %s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_SHORT)),
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_LONG)));
		printf("\t\t\t\t\"g:%s %s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_SHORT)),
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_MEDIUM)));
		printf("\t\t\t\t\"g:%s %s\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_SHORT)),
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_SHORT)));
		printf("\t\t\t\t\"G:%s HH%cmm%css\",\n",
			   modifyPattern(getNodeByIndex(patterns, DATE_PATTERN_SHORT)),
			   timeSep, timeSep);
		printf("\t\t\t\t\"m:MMMM dd\",\n");
		printf("\t\t\t\t\"M:MMMM dd\",\n");
		printf("\t\t\t\t\"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'\",\n");
		printf("\t\t\t\t\"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'\",\n");
		printf("\t\t\t\t\"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss\",\n");
		printf("\t\t\t\t\"t:%s\",\n",
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_FULL)));
		printf("\t\t\t\t\"t:%s\",\n",
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_LONG)));
		printf("\t\t\t\t\"t:%s\",\n",
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_MEDIUM)));
		printf("\t\t\t\t\"t:%s\",\n",
			   modifyPattern(getNodeByIndex(patterns, TIME_PATTERN_SHORT)));
		printf("\t\t\t\t\"T:HH%cmm%css\",\n", timeSep, timeSep);
		printf("\t\t\t\t\"u:yyyy'-'MM'-'dd HH':'mm':'ss'Z'\",\n");
		printf("\t\t\t\t\"U:dddd, dd MMMM yyyy HH:mm:ss\",\n");
		printf("\t\t\t\t\"y:yyyy MMMM\",\n");
		printf("\t\t\t\t\"Y:yyyy MMMM\",\n");
		printf("\t\t\t});\n");
	}
	printf("\t\t\treturn dfi;\n");
	printf("\t\t}\n");
	printf("\t\tset\n");
	printf("\t\t{\n");
	printf("\t\t\tbase.DateTimeFormat = value; // not used\n");
	printf("\t\t}\n");
	printf("\t}\n\n");
}

/*
 * Get the number of digits after the decimal point for a specific currency.
 */
static int getCurrencyDigits(const char *name)
{
	static struct { const char *name; int digits; int round; }
		currencies[] = {
			{"ADP", 0, 0 }, // Andorran Peseta: ANDORRA (AD)
			{"BHD", 3, 0 }, // Bahraini Dinar: BAHRAIN (BH)
    		{"BIF", 0, 0 }, // Burundi Franc: BURUNDI (BI)
    		{"BYR", 0, 0 }, // Belarussian Ruble: BELARUS (BY)
    		{"CHF", 2, 5 }, // Swiss Franc: LIECHTENSTEIN (LI), SWITZERLAND (CH)
    		{"CLF", 0, 0 }, // Unidades de fomento: CHILE (CL)
    		{"CLP", 0, 0 }, // Chilean Peso: CHILE (CL)
    		{"DJF", 0, 0 }, // Djibouti Franc: DJIBOUTI (DJ)
    		{"GNF", 0, 0 }, // Guinea Franc: GUINEA (GN)
    		{"IQD", 3, 0 }, // Iraqi Dinar: IRAQ (IQ)
    		{"JOD", 3, 0 }, // Jordanian Dinar: JORDAN (JO)
    		{"JPY", 0, 0 }, // Yen: JAPAN (JP)
    		{"KMF", 0, 0 }, // Comoro Franc: COMOROS (KM)
    		{"KRW", 0, 0 }, // Won: KOREA, REPUBLIC OF (KR)
    		{"KWD", 3, 0 }, // Kuwaiti Dinar: KUWAIT (KW)
    		{"LYD", 3, 0 }, // Lybian Dinar: LIBYAN ARAB JAMAHIRIYA (LY)
    		{"MGF", 0, 0 }, // Malagasy Franc: MADAGASCAR (MG)
    		{"OMR", 3, 0 }, // Rial Omani: OMAN (OM)
    		{"PYG", 0, 0 }, // Guarani: PARAGUAY (PY)
    		{"RWF", 0, 0 }, // Rwanda Franc: RWANDA (RW)
    		{"TND", 3, 0 }, // Tunisian Dinar: TUNISIA (TN)
    		{"TRL", 0, 0 }, // Turkish Lira: TURKEY (TR)
    		{"TTD", 0, 0 }, // Trinidad and Tobago Dollar: TRINIDAD AND TOBAGO (TT)
    		{"VUV", 0, 0 }, // Vatu: VANUATU (VU)
    		{"XAF", 0, 0 }, // CFA Franc BEAC: CAMEROON (CM), CENTRAL AFRICAN REPUBLIC (CF), CHAD (TD), CONGO (CG), EQUATORIAL GUINEA (GQ), GABON (GA)
    		{"XOF", 0, 0 }, // CFA Franc BCEAO: BENIN (BJ), BURKINA FASO (BF), COTE D'IVOIRE (CI), GUINEA-BISSAU (GW), MALI (ML), NIGER (NE), SENEGAL (SN), TOGO (TG)
    		{"XPF", 0, 0 }, // CFP Franc: FRENCH POLYNESIA (PF), NEW CALEDONIA (NC), WALLIS AND FUTUNA (WF)
    		{0, 2, 0 } // (All currencies not listed)
	};
	int index = 0;
	while(currencies[index].name && strcmp(currencies[index].name, name) != 0)
	{
		++index;
	}
	return currencies[index].digits;
}

/*
 * Print the number format information.
 */
static void printNumberFormat(void)
{
	Node *numElems;

	/* Load the format information */
	numElems = getNode("NumberElements");

	/* Do we need to override the number format information? */
	if(identifier != 0 && !numElems)
	{
		return;
	}

#define	NFI_DECIMAL_SEPARATOR		0
#define	NFI_GROUPING_SEPARATOR		1
#define	NFI_PATTERN_SEPARATOR		2
#define	NFI_PERCENT					3
#define	NFI_ZERO_DIGIT				4
#define	NFI_DIGIT					5
#define	NFI_MINUS_SIGN				6
#define	NFI_EXPONENT_SEPARATOR		7
#define	NFI_PER_MILL				8
#define	NFI_INFINITY				9
#define	NFI_NAN						10
#define	NFI_MONETARY_SEPARATOR		11

	/* Output the "NumberFormat" property information */
	printf("\tpublic override NumberFormatInfo NumberFormat\n");
	printf("\t{\n");
	printf("\t\tget\n");
	printf("\t\t{\n");
	if(!identifier)
	{
		printf("\t\t\tNumberFormatInfo nfi = new NumberFormatInfo();\n");
	}
	else
	{
		printf("\t\t\tNumberFormatInfo nfi = base.NumberFormat;\n");
	}
	if(hasNodeByIndex(numElems, NFI_MONETARY_SEPARATOR))
	{
		printf("\t\t\tnfi.CurrencyDecimalSeparator = \"%s\";\n",
			   getNodeByIndex(numElems, NFI_MONETARY_SEPARATOR));
	}
	if(!identifier)
	{
		/* Fetch the currency information from the region name table */
		printf("\t\t\tRegionNameTable.AddCurrencyInfo(nfi, this);\n");
	}
	if(hasNodeByIndex(numElems, NFI_GROUPING_SEPARATOR))
	{
		printf("\t\t\tnfi.CurrencyGroupSeparator = \"%s\";\n",
			   getNodeByIndex(numElems, NFI_GROUPING_SEPARATOR));
		printf("\t\t\tnfi.NumberGroupSeparator = \"%s\";\n",
			   getNodeByIndex(numElems, NFI_GROUPING_SEPARATOR));
		printf("\t\t\tnfi.PercentGroupSeparator = \"%s\";\n",
			   getNodeByIndex(numElems, NFI_GROUPING_SEPARATOR));
	}
	if(hasNodeByIndex(numElems, NFI_MINUS_SIGN))
	{
		printf("\t\t\tnfi.NegativeSign = \"%s\";\n",
			   getNodeByIndex(numElems, NFI_MINUS_SIGN));
	}
	if(hasNodeByIndex(numElems, NFI_DECIMAL_SEPARATOR))
	{
		printf("\t\t\tnfi.NumberDecimalSeparator = \"%s\";\n",
			   getNodeByIndex(numElems, NFI_DECIMAL_SEPARATOR));
		printf("\t\t\tnfi.PercentDecimalSeparator = \"%s\";\n",
			   getNodeByIndex(numElems, NFI_DECIMAL_SEPARATOR));
	}
	if(hasNodeByIndex(numElems, NFI_PERCENT))
	{
		printf("\t\t\tnfi.PercentSymbol = \"%s\";\n",
			   getNodeByIndex(numElems, NFI_PERCENT));
	}
	if(hasNodeByIndex(numElems, NFI_PER_MILL))
	{
		printf("\t\t\tnfi.PerMilleSymbol = \"%s\";\n",
			   getNodeByIndex(numElems, NFI_PER_MILL));
	}
	printf("\t\t\treturn nfi;\n");
	printf("\t\t}\n");
	printf("\t\tset\n");
	printf("\t\t{\n");
	printf("\t\t\tbase.NumberFormat = value; // not used\n");
	printf("\t\t}\n");
	printf("\t}\n\n");
}

/*
 * Print the language name resolvers.
 */
static void printLanguageResolvers(void)
{
	Node *langs = getNode("Languages");
	Node *countries = getNode("Countries");
	Node *child;

	/* Bail out if no language or country names are defined for this culture */
	if(!langs && !countries)
	{
		return;
	}

	/* Create the language resolver method */
	if(langs)
	{
		if(!identifier)
		{
			printf("\tpublic virtual String ResolveLanguage(String name)\n");
		}
		else
		{
			printf("\tpublic override String ResolveLanguage(String name)\n");
		}
		printf("\t{\n");
		printf("\t\tswitch(name)\n");
		printf("\t\t{\n");
		child = langs->children;
		while(child != 0)
		{
			if(child->children)
			{
				printf("\t\t\tcase \"%s\": return \"%s\";\n",
					   child->name, child->children->name);
			}
			child = child->next;
		}
		printf("\t\t}\n");
		if(!identifier)
		{
			printf("\t\treturn name;\n");
		}
		else
		{
			printf("\t\treturn base.ResolveLanguage(name);\n");
		}
		printf("\t}\n\n");
	}

	/* Create the country resolver method */
	if(countries)
	{
		if(!identifier)
		{
			printf("\tpublic virtual String ResolveCountry(String name)\n");
		}
		else
		{
			printf("\tpublic override String ResolveCountry(String name)\n");
		}
		printf("\t{\n");
		printf("\t\tswitch(name)\n");
		printf("\t\t{\n");
		child = countries->children;
		while(child != 0)
		{
			if(child->children)
			{
				printf("\t\t\tcase \"%s\": return \"%s\";\n",
					   child->name, child->children->name);
			}
			child = child->next;
		}
		printf("\t\t}\n");
		if(!identifier)
		{
			printf("\t\treturn name;\n");
		}
		else
		{
			printf("\t\treturn base.ResolveCountry(name);\n");
		}
		printf("\t}\n\n");
	}
}

/*
 * Print the TextInfo object.
 */
static void printTextInfo()
{
	/* If none of the TextInfo property values are different, then bail out */
	if(ansiCP == 1252 && ebcdicCP == 37 && macCP == 10000 &&
	   oemCP == 437 && !strcmp(separator, ","))
	{
		return;
	}

	/* Output the definition of the private TextInfo class */
	printf("\tprivate class PrivateTextInfo : _I18NTextInfo\n");
	printf("\t{\n");
	printf("\t\tpublic PrivateTextInfo(int culture) : base(culture) {}\n\n");
	if(ansiCP != 1252)
	{
		printf("\t\tpublic override int ANSICodePage\n");
		printf("\t\t{\n");
		printf("\t\t\tget\n");
		printf("\t\t\t{\n");
		printf("\t\t\t\treturn %d;\n", ansiCP);
		printf("\t\t\t}\n");
		printf("\t\t}\n");
	}
	if(ebcdicCP != 37)
	{
		printf("\t\tpublic override int EBCDICCodePage\n");
		printf("\t\t{\n");
		printf("\t\t\tget\n");
		printf("\t\t\t{\n");
		printf("\t\t\t\treturn %d;\n", ebcdicCP);
		printf("\t\t\t}\n");
		printf("\t\t}\n");
	}
	if(macCP != 10000)
	{
		printf("\t\tpublic override int MacCodePage\n");
		printf("\t\t{\n");
		printf("\t\t\tget\n");
		printf("\t\t\t{\n");
		printf("\t\t\t\treturn %d;\n", macCP);
		printf("\t\t\t}\n");
		printf("\t\t}\n");
	}
	if(oemCP != 437)
	{
		printf("\t\tpublic override int OEMCodePage\n");
		printf("\t\t{\n");
		printf("\t\t\tget\n");
		printf("\t\t\t{\n");
		printf("\t\t\t\treturn %d;\n", oemCP);
		printf("\t\t\t}\n");
		printf("\t\t}\n");
	}
	if(strcmp(separator, ",") != 0)
	{
		printf("\t\tpublic override String ListSeparator\n");
		printf("\t\t{\n");
		printf("\t\t\tget\n");
		printf("\t\t\t{\n");
		printf("\t\t\t\treturn \"%s\";\n", separator);
		printf("\t\t\t}\n");
		printf("\t\t}\n");
	}
	printf("\n\t}; // class PrivateTextInfo\n\n");

	/* Output the definition of the "TextInfo" property */
	printf("\tpublic override TextInfo TextInfo\n");
	printf("\t{\n");
	printf("\t\tget\n");
	printf("\t\t{\n");
	printf("\t\t\treturn new PrivateTextInfo(LCID);\n");
	printf("\t\t}\n");
	printf("\t}\n\n");
}

/*
 * Print the definition of the culture, using the loaded locale rules.
 */
static void printDefinition(void)
{
	printDateTimeFormat();
	printNumberFormat();
	printLanguageResolvers();
	printTextInfo();
}
