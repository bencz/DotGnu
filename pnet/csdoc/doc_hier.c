/*
 * doc_hier.c - Convert csdoc into a tree hierarchy diagram.
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
#include "doc_tree.h"
#include "doc_backend.h"

#ifdef	__cplusplus
extern	"C" {
#endif

char const ILDocProgramHeader[] =
  "CSDOC2HIER " VERSION " - Convert C# documentation into a hierarchy diagram";

char const ILDocProgramName[] = "CSDOC2HIER";

ILCmdLineOption const ILDocProgramOptions[] = {
	{"-fby-library", 'f', 1,
		"-fby-library",
		"Organise the output by library"},
	{0, 0, 0, 0, 0}
};

char *ILDocDefaultOutput(int numInputs, char **inputs, const char *progname)
{
	/* Always output to stdout by default */
	return "-";
}

int ILDocValidateOutput(char *outputPath, const char *progname)
{
	/* All output streams are valid by default */
	return 1;
}

/*
 * Dump a number of spaces for indenting purposes.
 */
static void DumpIndent(FILE *outstream, int indent)
{
	while(indent > 0)
	{
		putc(' ', outstream);
		--indent;
	}
}

/*
 * Dump the children of a specified type.  If "library" is not
 * NULL, then only process children in the specified library.
 */
static void DumpChildren(FILE *outstream, ILDocTree *tree,
						 ILDocType *type, ILDocLibrary *library,
						 int indent)
{
	ILDocType *child;
	if(library)
	{
		child = library->types;
		while(child != 0)
		{
			if(child->baseType && !strcmp(type->fullName, child->baseType))
			{
				DumpIndent(outstream, indent);
				fputs(child->fullName, outstream);
				putc('\n', outstream);
				DumpChildren(outstream, tree, child, library, indent + 4);
			}
			child = child->next;
		}
	}
	else
	{
		library = tree->libraries;
		while(library != 0)
		{
			child = library->types;
			while(child != 0)
			{
				if(child->baseType && !strcmp(type->fullName, child->baseType))
				{
					DumpIndent(outstream, indent);
					fputs(child->fullName, outstream);
					putc('\n', outstream);
					DumpChildren(outstream, tree, child, 0, indent + 4);
				}
				child = child->next;
			}
			library = library->next;
		}
	}
}

/*
 * Dump a specific top-level type.
 */
static void DumpTopLevelType(FILE *outstream, ILDocTree *tree,
							 ILDocType *type, ILDocLibrary *library)
{
	int indent = 4;
	if(library)
	{
		fputs("    ", outstream);
		indent = 8;
	}
	fputs(type->fullName, outstream);
	putc('\n', outstream);
	DumpChildren(outstream, tree, type, library, indent);
}

int ILDocConvert(ILDocTree *tree, int numInputs, char **inputs,
				 char *outputPath, const char *progname)
{
	FILE *outstream;
	int closeout;
	int byLibrary;
	ILDocLibrary *library;
	ILDocNamespace *namespace;
	ILDocType *type;
	ILDocType *parent;

	/* Output the output stream */
	if(!strcmp(outputPath, "-"))
	{
		outstream = stdout;
		closeout = 0;
	}
	else if((outstream = fopen(outputPath, "w")) == NULL)
	{
		perror(outputPath);
		return 0;
	}
	else
	{
		closeout = 1;
	}

	/* Determine if we need to organise the output by library */
	byLibrary = ILDocFlagSet("by-library");

	/* Process all top-level classes */
	if(byLibrary)
	{
		library = tree->libraries;
		while(library != 0)
		{
			fputs("Library: ", outstream);
			if(library->name)
			{
				fputs(library->name, outstream);
			}
			else
			{
				fputs("Unknown", outstream);
			}
			putc('\n', outstream);
			putc('\n', outstream);
			type = library->types;
			while(type != 0)
			{
				if(!(type->baseType))
				{
					parent = 0;
				}
				else if((parent = ILDocTypeFind(tree, type->baseType)) != 0)
				{
					if(parent->library != library)
					{
						parent = 0;
					}
				}
				if(!parent)
				{
					DumpTopLevelType(outstream, tree, type, library);
				}
				type = type->next;
			}
			putc('\n', outstream);
			library = library->next;
		}
	}
	else
	{
		namespace = tree->namespaces;
		while(namespace != 0)
		{
			type = namespace->types;
			while(type != 0)
			{
				if(!(type->baseType))
				{
					parent = 0;
				}
				else
				{
					parent = ILDocTypeFind(tree, type->baseType);
				}
				if(!parent)
				{
					DumpTopLevelType(outstream, tree, type, 0);
				}
				type = type->nextNamespace;
			}
			namespace = namespace->next;
		}
	}

	/* Clean up and exit */
	if(closeout)
	{
		fclose(outstream);
	}
	return 1;
}

#ifdef	__cplusplus
};
#endif
