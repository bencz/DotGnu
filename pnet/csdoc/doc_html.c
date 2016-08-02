/*
 * doc_html.c - Convert csdoc into HTML.
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
#include "doc_tree.h"
#include "doc_backend.h"
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif
#ifdef HAVE_SYS_STAT_H
#include <sys/stat.h>
#endif
#ifdef HAVE_UNISTD_H
#include <unistd.h>
#endif
#include <errno.h>

#ifdef	__cplusplus
extern	"C" {
#endif

char const ILDocProgramHeader[] =
	"CSDOC2HTML " VERSION " - Convert C# documentation into HTML";

char const ILDocProgramName[] = "CSDOC2HTML";

ILCmdLineOption const ILDocProgramOptions[] = {
	{"-fsingle-file", 'f', 1,
		"-fsingle-file",
		"Write the HTML as a single output file"},
	{"-fmulti-file", 'f', 1,
		"-fmulti-file",
		"Write the HTML to multiple files (default)"},
	{"-fframes", 'f', 1,
		"-fframes",
		"Use frames when outputting multiple files (default)"},
	{"-fno-frames", 'f', 1,
		"-fno-frames",
		"Do not use frames when outputting multiple files"},
	{"-fcombine-members", 'f', 1,
		"-fcombine-members",
		"Write type members into the same file as their type (default)"},
	{"-fseparate-members", 'f', 1,
		"-fseparate-members",
		"Write type members to separate files"},
	{"-fnamespace-directories", 'f', 1,
		"-fnamespace-directories",
		"Write namespace contents in separate subdirectories (default)"},
	{"-fno-namespace-directories", 'f', 1,
		"-fno-namespace-directories",
		"Write all namespaces into a single directory"},
	{"-ftitle", 'f', 1,
		"-ftitle=TITLE",
		"Specify the title to use for the document"},
	{"-fpage-color", 'f', 1,
		"-fpage-color=COLOR",
		"Specify the color to use for the page background"},
	{"-fdefn-color", 'f', 1,
		"-fdefn-color=COLOR",
		"Specify the color to use to highlight type and member definitions"},
	{"-fheader-color", 'f', 1,
		"-fheader-color=COLOR",
		"Specify the color to use in table headers"},
	{"-fsource-xref-tags", 'f', 1,
		"-fsource-xref-tags",
		"Enable generation of tags for source cross reference"},
	{"-fsource-xref-dir", 'f', 1,
		"-fsource-xref-dir=DIR",
		"Specify relative location of directory containing source html file"},
	{0, 0, 0, 0, 0}
};

/*
 * Color values to use for various documentation elements.
 */
static const char *pageColor = "#FFFFFF";
static const char *cartoucheColor = "#C0C0C0";
static const char *headerColor = "#C0C0C0";

/*
 * Flags that affect the HTML output.
 */
static int isSingleFile;
static int separateMembers;
static int namespaceDirectories;
static int useFrames;

static int	sourceXrefTags = 0;
static char	*sourceXrefName = NULL;
static char	*sourceXrefDir = NULL;

/*
 * The next index to use to create unique filenames.
 */
static int nextIndex;

char *ILDocDefaultOutput(int numInputs, char **inputs, const char *progname)
{
	if(!ILDocFlagSet("single-file"))
	{
		/* The default output is the current directory */
		return ".";
	}
	else if(!strcmp(inputs[0], "-"))
	{
		/* The first input is stdin, so the output is too */
		return "-";
	}
	else
	{
		/* Adjust the first input filename to end in ".html" */
		int len = strlen(inputs[0]);
		char *result;
		while(len > 0 && inputs[0][len - 1] != '/' &&
			  inputs[0][len - 1] != '\\' &&
			  inputs[0][len - 1] != '.')
		{
			--len;
		}
		if(len > 0 && inputs[0][len - 1] == '.')
		{
			result = (char *)ILMalloc(len + 5);
			if(!result)
			{
				ILDocOutOfMemory(progname);
			}
			ILMemCpy(result, inputs[0], len);
			strcpy(result + len, "html");
		}
		else
		{
			result = (char *)ILMalloc(strlen(inputs[0]) + 6);
			if(!result)
			{
				ILDocOutOfMemory(progname);
			}
			strcpy(result, inputs[0]);
			strcat(result, ".html");
		}
		return result;
	}
}

int ILDocValidateOutput(char *outputPath, const char *progname)
{
	if(ILDocFlagSet("single-file"))
	{
		/* Any output filename is OK in single-file mode */
		return 1;
	}
#if defined(HAVE_STAT) && defined(S_ISDIR)
	/* The output path must be a directory */
	{
		struct stat st;
		if(stat(outputPath, &st) < 0)
		{
			perror(outputPath);
			return 0;
		}
		else if(!S_ISDIR(st.st_mode))
		{
			errno = ENOTDIR;
			perror(outputPath);
			return 0;
		}
	}
	return 1;
#else /* !HAVE_STAT */
	/* Do not know how to check for directories, so assume it is OK */
	return 1;
#endif /* !HAVE_STAT */
}

/*
 * Open a HTML output stream within a specific directory.
 * Sub-directories are created as necessary.
 */
static FILE *CreateHTMLStream(const char *filename, const char *directory)
{
	char *fullPath;
	FILE *stream;
	int posn;

	/* Construct the full pathname */
	fullPath = (char *)ILMalloc(strlen(directory) + strlen(filename) + 2);
	if(!fullPath)
	{
		ILDocOutOfMemory(0);
	}
	strcpy(fullPath, directory);
	strcat(fullPath, "/");
	strcat(fullPath, filename);

	/* Attempt to open the file for writing */
	if((stream = fopen(fullPath, "w")) != NULL)
	{
		ILFree(fullPath);
		return stream;
	}

	/* Create sub-directories within the pathname */
	posn = strlen(directory) + 1;
	while(fullPath[posn] != '\0')
	{
		if(fullPath[posn] == '/')
		{
			fullPath[posn] = '\0';
		#ifdef IL_WIN32_NATIVE
			mkdir(fullPath);
		#else
			mkdir(fullPath, 0777);
		#endif
			fullPath[posn] = '/';
		}
		++posn;
	}

	/* Attempt to open the file again for writing */
	stream = fopen(fullPath, "w");
	if(stream == NULL)
	{
		perror(fullPath);
		ILFree(fullPath);
		exit(1);
	}
	ILFree(fullPath);
	return stream;
}

/*
 * Open a HTML stream for a specific namespace.
 */
static FILE *CreateNamespaceStream(const char *name, const char *directory)
{
	const char *saveName;
	char *newName;
	int posn;
	FILE *stream;

	/* Allocate space to hold the modified namespace name */
	newName = (char *)ILMalloc(strlen(name) * 2 + 32);
	if(!newName)
	{
		ILDocOutOfMemory(0);
	}

	/* Create the namespace filename */
	if(namespaceDirectories)
	{
		/* The namespace file is within a subdirectory */
		posn = 0;
		saveName = name;
		while(*name != '\0')
		{
			if((*name >= 'A' && *name <= 'Z') ||
			   (*name >= 'a' && *name <= 'z') ||
			   (*name >= '0' && *name <= '9') ||
			   *name == '_')
			{
				newName[posn++] = *name;
			}
			else if(*name == '.')
			{
				newName[posn++] = '/';
				saveName = name + 1;
			}
			++name;
		}
		newName[posn++] = '/';
		name = saveName;
		while(*name != '\0')
		{
			if((*name >= 'A' && *name <= 'Z') ||
			   (*name >= 'a' && *name <= 'z') ||
			   (*name >= '0' && *name <= '9') ||
			   *name == '_')
			{
				newName[posn++] = *name;
			}
			++name;
		}
	}
	else
	{
		/* The namespace file is in the main directory */
		posn = 0;
		while(*name != '\0')
		{
			if((*name >= 'A' && *name <= 'Z') ||
			   (*name >= 'a' && *name <= 'z') ||
			   (*name >= '0' && *name <= '9') ||
			   *name == '_')
			{
				newName[posn++] = *name;
			}
			else if(*name == '.')
			{
				newName[posn++] = '_';
			}
			++name;
		}
	}

	/* Create the stream and return */
	strcpy(newName + posn, ".html");
	stream = CreateHTMLStream(newName, directory);
	ILFree(newName);
	return stream;
}

/*
 * Open a HTML stream for either a type or a stand-alone member.
 */
static FILE *TypeOrMemberStream(ILDocType *type, const char *directory,
								int index)
{
	char *newName;
	const char *name;
	int posn;
	FILE *stream;

	/* Allocate space for the filename */
	newName = (char *)ILMalloc(strlen(type->name) +
							   strlen(type->namespace->name) + 64);
	if(!newName)
	{
		ILDocOutOfMemory(0);
	}

	/* Get the directory name */
	if(namespaceDirectories)
	{
		/* Each namespace is in a separate directory */
		posn = 0;
		name = type->namespace->name;
		if(name[0] == '\0')
		{
			name = "Global";
		}
		while(*name != '\0')
		{
			if((*name >= 'A' && *name <= 'Z') ||
			   (*name >= 'a' && *name <= 'z') ||
			   (*name >= '0' && *name <= '9') ||
			   *name == '_')
			{
				newName[posn++] = *name;
			}
			else if(*name == '.')
			{
				newName[posn++] = '/';
			}
			++name;
		}
		newName[posn++] = '/';
		name = type->name;
	}
	else
	{
		/* All namespaces are in the main directory */
		posn = 0;
		name = type->fullName;
	}

	/* Add the type name to the path */
	while(*name != '\0')
	{
		while(*name != '\0')
		{
			if((*name >= 'A' && *name <= 'Z') ||
			   (*name >= 'a' && *name <= 'z') ||
			   (*name >= '0' && *name <= '9'))
			{
				newName[posn++] = *name;
			}
			else if(*name == '_' || *name == '.')
			{
				newName[posn++] = '_';
			}
			++name;
		}
	}

	/* Add the member index and HTML extension */
	if(index != 0)
	{
		sprintf(newName + posn, "_%d.html", index);
	}
	else
	{
		strcpy(newName + posn, ".html");
	}

	/* Create the stream and return */
	stream = CreateHTMLStream(newName, directory);
	ILFree(newName);
	return stream;
}

/*
 * Open a HTML stream for a specific type.
 */
static FILE *CreateTypeStream(ILDocType *type, const char *directory)
{
	return TypeOrMemberStream(type, directory, 0);
}

/*
 * Open a HTML stream for a specific member.
 */
static FILE *CreateMemberStream(ILDocMember *member, const char *directory)
{
	if(!(member->index))
	{
		member->index = nextIndex;
		++nextIndex;
	}
	return TypeOrMemberStream(member->type, directory, member->index);
}

/*
 * Print a literal string to a HTML stream.
 * Returns non-zero if the last character is '\n'.
 */
static int PrintString(FILE *stream, const char *str)
{
	int lastWasNL = 0;
	if(str)
	{
		while(*str != '\0')
		{
			if(*str == '<')
			{
				fputs("&lt;", stream);
			}
			else if(*str == '>')
			{
				fputs("&gt;", stream);
			}
			else if(*str == '&')
			{
				fputs("&amp;", stream);
			}
			else if(*str == '"')
			{
				fputs("&quot;", stream);
			}
			else if(*str == '\'')
			{
				fputs("&apos;", stream);
			}
			else if(*str != '\r')
			{
				putc(*str, stream);
			}
			lastWasNL = (*str == '\n');
			++str;
		}
	}
	return lastWasNL;
}

/*
 * Print a URL-encoded string to a HTML stream.
 */
static void PrintURLString(FILE *stream, const char *str)
{
	if(str)
	{
		while(*str != '\0')
		{
			if((*str >= 'A' && *str <= 'Z') ||
			   (*str >= 'a' && *str <= 'z') ||
			   (*str >= '0' && *str <= '9') ||
			   *str == '.')
			{
				putc(*str, stream);
			}
			else
			{
				fprintf(stream, "%%%02X", ((int)(*str)) & 0xFF);
			}
			++str;
		}
	}
}

/*
 * Print a filename string to a HTML stream.
 */
static void PrintFilenameString(FILE *stream, const char *str)
{
	if(str)
	{
		while(*str != '\0')
		{
			if((*str >= 'A' && *str <= 'Z') ||
			   (*str >= 'a' && *str <= 'z') ||
			   (*str >= '0' && *str <= '9'))
			{
				putc(*str, stream);
			}
			else if(*str == '_' || *str == '.')
			{
				putc('_', stream);
			}
			++str;
		}
	}
}

/*
 * Print either type of string, based on a flag.
 * If "href" is non-zero, then print as a HREF string.
 */
static void PrintEitherString(FILE *stream, const char *str, int href)
{
	if(href)
	{
		PrintURLString(stream, str);
	}
	else
	{
		PrintString(stream, str);
	}
}

/*
 * Print the full name of a type, including its kind.
 */
static void PrintTypeName(FILE *stream, ILDocType *type, int href)
{
	PrintEitherString(stream, type->fullName, href);
	if(href)
	{
		fputs("%20", stream);
	}
	else
	{
		putc(' ', stream);
	}
	switch(type->kind)
	{
		case ILDocTypeKind_Class:		fputs("Class", stream); break;
		case ILDocTypeKind_Interface:	fputs("Interface", stream); break;
		case ILDocTypeKind_Struct:		fputs("Structure", stream); break;
		case ILDocTypeKind_Enum:		fputs("Enum", stream); break;
		case ILDocTypeKind_Delegate:	fputs("Delegate", stream); break;
	}
}

/*
 * Table of builtin types that look better in their
 * short form than in their long form.
 */
typedef struct
{
	const char *longName;
	int         longNameLen;
	const char *shortName;

} ILDocBuiltinType;
static ILDocBuiltinType const builtinTypes[] = {
	{"System.Boolean",		14,		"bool"},
	{"System.Char",			11,		"char"},
	{"System.SByte",		12,		"sbyte"},
	{"System.Byte",			11,		"byte"},
	{"System.Int16",		12,		"short"},
	{"System.UInt16",		13,		"ushort"},
	{"System.Int32",		12,		"int"},
	{"System.UInt32",		13,		"uint"},
	{"System.Int64",		12,		"long"},
	{"System.UInt64",		13,		"ulong"},
	{"System.Single",		13,		"float"},
	{"System.Double",		13,		"double"},
	{"System.Void",			11,		"void"},
};

/*
 * Print a parameter type name.
 */
static void PrintParamTypeName(FILE *stream, char *type, int href)
{
	int posn, len;
	for(posn = 0; posn < (sizeof(builtinTypes) / sizeof(ILDocBuiltinType));
		++posn)
	{
		len = builtinTypes[posn].longNameLen;
		if(!strncmp(type, builtinTypes[posn].longName, len) &&
		   (type[len] == '\0' || type[len] == ' ' ||
		    type[len] == '*' || type[len] == '['))
		{
			fputs(builtinTypes[posn].shortName, stream);
			PrintEitherString(stream, type + len, href);
			return;
		}
	}
	PrintEitherString(stream, type, href);
}

/*
 * Print the full name of a member, including its kind.
 */
static void PrintMemberName(FILE *stream, ILDocMember *member, int href)
{
	ILDocParameter *param;
	if(member->fullyQualify && member->name && member->returnType &&
	   (!strcmp(member->name, "op_Explicit") ||
	    !strcmp(member->name, "op_Implicit")))
	{
		/* Conversions need to include the return type in the signature */
		PrintParamTypeName(stream, member->returnType, href);
		putc(' ', stream);
	}
	if(member->type->fullyQualify)
	{
		PrintEitherString(stream, member->type->fullName, href);
	}
	else
	{
		PrintEitherString(stream, member->type->name, href);
	}
	if(member->memberType != ILDocMemberType_Constructor)
	{
		putc('.', stream);
		PrintEitherString(stream, member->name, href);
	}
	if(member->fullyQualify)
	{
		/* We need to qualify the name using the parameter types */
		if(href)
		{
			fputs("%28", stream);
		}
		else
		{
			putc('(', stream);
		}
		param = member->parameters;
		while(param != 0)
		{
			if(param != member->parameters)
			{
				PrintEitherString(stream, ", ", href);
			}
			PrintParamTypeName(stream, param->type, href);
			param = param->next;
		}
		if(href)
		{
			fputs("%29", stream);
		}
		else
		{
			putc(')', stream);
		}
	}
	if(href)
	{
		fputs("%20", stream);
	}
	else
	{
		putc(' ', stream);
	}
	switch(member->memberType)
	{
		case ILDocMemberType_Field:		  fputs("Field", stream); break;
		case ILDocMemberType_Method:	  fputs("Method", stream); break;
		case ILDocMemberType_Constructor: fputs("Constructor", stream); break;
		case ILDocMemberType_Property:	  fputs("Property", stream); break;
		case ILDocMemberType_Event:       fputs("Event", stream); break;
		case ILDocMemberType_Unknown:	  break;
	}
}

/*
 * Print an anchor for a namespace.
 */
static void PrintNamespaceAnchor(FILE *stream, ILDocNamespace *namespace)
{
	if(isSingleFile)
	{
		fputs("<A NAME=\"", stream);
		if(namespace->name[0] != '\0')
		{
			PrintURLString(stream, namespace->name);
		}
		else
		{
			PrintURLString(stream, "Global");
		}
		PrintURLString(stream, " Namespace");
		fputs("\">", stream);
	}
}

/*
 * Print an anchor for a type.
 */
static void PrintTypeAnchor(FILE *stream, ILDocType *type)
{
	if(isSingleFile)
	{
		fputs("<A NAME=\"", stream);
		PrintTypeName(stream, type, 1);
		fputs("\">", stream);
	}
}

/*
 * Print an anchor for a member.
 */
static void PrintMemberAnchor(FILE *stream, ILDocMember *member)
{
	if(isSingleFile || !separateMembers)
	{
		fputs("<A NAME=\"", stream);
		PrintMemberName(stream, member, 1);
		fputs("\">", stream);
	}
}

/*
 * Print a back path for namespace name.
 */
static void PrintBackPath(FILE *stream, const char *name)
{
	while(*name != '\0')
	{
		if(*name == '.')
		{
			fputs("../", stream);
		}
		++name;
	}
	fputs("../", stream);
}

/*
 * Print a forward path for a namespace name.
 */
static void PrintForwardPath(FILE *stream, const char *name)
{
	if(name[0] == '\0')
	{
		name = "Global";
	}
	while(*name != '\0')
	{
		if((*name >= 'A' && *name <= 'Z') ||
		   (*name >= 'a' && *name <= 'z') ||
		   (*name >= '0' && *name <= '9') ||
		   *name == '_')
		{
			putc(*name, stream);
		}
		else if(*name == '.')
		{
			putc('/', stream);
		}
		++name;
	}
}

/*
 * Print a reference path to reach the HTML file of a type.
 * Returns zero if no path written.
 */
static int PrintTypePath(FILE *stream, ILDocType *type, ILDocType *from)
{
	/* Single-file output does not need reference paths */
	if(isSingleFile)
	{
		return 0;
	}

	/* Are all namespaces in the same directory? */
	if(!namespaceDirectories)
	{
		PrintFilenameString(stream, type->fullName);
		return 1;
	}

	/* Are the two types in the same namespace? */
	if(from && from->namespace == type->namespace)
	{
		PrintFilenameString(stream, type->name);
		return 1;
	}

	/* Construct a "back path" between "from" and the global directory */
	if(from)
	{
		PrintBackPath(stream, from->namespace->name);
	}

	/* Construct a "forward path" between the global directory and "type" */
	PrintForwardPath(stream, type->namespace->name);
	putc('/', stream);

	/* Print the type name */
	PrintFilenameString(stream, type->name);

	/* Done */
	return 1;
}

/*
 * Print a reference to a namespace from another type.
 */
static void PrintNamespaceReference(FILE *stream, ILDocNamespace *namespace,
							        ILDocType *from, const char *target)
{
	const char *name;
	int len;
	if(namespace->name[0] != '\0')
	{
		name = namespace->name;
	}
	else
	{
		name = "Global";
	}
	fputs("<A HREF=\"", stream);
	if(isSingleFile)
	{
		/* The namespace is in this file */
		putc('#', stream);
		PrintURLString(stream, name);
		PrintURLString(stream, " Namespace");
	}
	else if(!namespaceDirectories)
	{
		/* The namespace will always be in the same directory */
		PrintFilenameString(stream, name);
		fputs(".html", stream);
	}
	else if(from && from->namespace == namespace)
	{
		/* The from type is in the same directory as the namespace */
		len = strlen(name);
		while(len > 0 && name[len - 1] != '.')
		{
			--len;
		}
		PrintFilenameString(stream, name + len);
		fputs(".html", stream);
	}
	else
	{
		/* Construct a "back path" between "from" and the global directory */
		if(from)
		{
			PrintBackPath(stream, from->namespace->name);
		}

		/* Construct a "forward path" from the global directory to
		   the namespace that we are interested in */
		PrintForwardPath(stream, name);
		putc('/', stream);

		/* Output the name of the namespace index file */
		len = strlen(name);
		while(len > 0 && name[len - 1] != '.')
		{
			--len;
		}
		PrintFilenameString(stream, name + len);
		fputs(".html", stream);
	}
	if(target && useFrames)
	{
		fputs("\" TARGET=\"", stream);
		PrintURLString(stream, target);
	}
	fputs("\">", stream);
}

/*
 * Print the filename that contains a namespace.
 */
static void PrintNamespaceFilename(FILE *stream, ILDocNamespace *namespace)
{
	const char *name;
	int len;
	if(namespace->name[0] != '\0')
	{
		name = namespace->name;
	}
	else
	{
		name = "Global";
	}
	if(!namespaceDirectories)
	{
		/* The namespace will always be in the same directory */
		PrintFilenameString(stream, name);
		fputs(".html", stream);
	}
	else
	{
		/* Construct a "forward path" from the global directory to
		   the namespace that we are interested in */
		PrintForwardPath(stream, name);
		putc('/', stream);

		/* Output the name of the namespace index file */
		len = strlen(name);
		while(len > 0 && name[len - 1] != '.')
		{
			--len;
		}
		PrintFilenameString(stream, name + len);
		fputs(".html", stream);
	}
}

/*
 * Print a reference to a type from another type.
 */
static void PrintTypeReference(FILE *stream, ILDocType *type,
							   ILDocType *from, const char *target)
{
	fputs("<A HREF=\"", stream);
	if(PrintTypePath(stream, type, from))
	{
		fputs(".html", stream);
	}
	if(isSingleFile)
	{
		putc('#', stream);
		PrintTypeName(stream, type, 1);
	}
	if(target && useFrames)
	{
		fputs("\" TARGET=\"", stream);
		PrintURLString(stream, target);
	}
	fputs("\">", stream);
}

/*
 * Print a reference to a member from another type.
 */
static void PrintMemberReference(FILE *stream, ILDocMember *member,
								 ILDocType *from, const char *target)
{
	fputs("<A HREF=\"", stream);
	if(isSingleFile)
	{
		/* All members are in a single output file */
		putc('#', stream);
		PrintMemberName(stream, member, 1);
	}
	else if(separateMembers)
	{
		/* The member is in a file distinct from its type */
		PrintTypePath(stream, member->type, from);
		if(!(member->index))
		{
			member->index = nextIndex;
			++nextIndex;
		}
		fprintf(stream, "_%d.html", member->index);
	}
	else if(member->type == from)
	{
		/* The member is in the current file */
		putc('#', stream);
		PrintMemberName(stream, member, 1);
	}
	else
	{
		/* The member is in the same file as its type */
		if(PrintTypePath(stream, member->type, from))
		{
			fputs(".html", stream);
		}
		putc('#', stream);
		PrintMemberName(stream, member, 1);
	}
	if(target && useFrames)
	{
		fputs("\" TARGET=\"", stream);
		PrintURLString(stream, target);
	}
	fputs("\">", stream);
}

/*
 * Print a C# signature.
 */
static void PrintSignature(FILE *stream, char *signature)
{
	if(signature)
	{
		fputs("<BLOCKQUOTE>\n", stream);
		fputs("<TABLE COLS=\"1\" ROWS=\"1\" WIDTH=\"100%\">\n", stream);
		fprintf(stream, "<TR><TD BGCOLOR=\"%s\"><PRE>", cartoucheColor);
		PrintString(stream, signature);
		fputs("</PRE></TD></TR>\n", stream);
		fputs("</TABLE>\n</BLOCKQUOTE>\n\n", stream);
	}
}

/*
 * Print a cross-reference.
 */
static void PrintCRef(FILE *stream, ILDocTree *tree, const char *cref,
					  ILDocType *from, int nameOnly)
{
	ILDocType *type;
	if(cref)
	{
		if(cref[0] != '\0' && cref[1] == ':')
		{
			if(cref[0] == 'T')
			{
				/* Print a reference to a type */
				type = ILDocTypeFind(tree, cref + 2);
				if(type)
				{
					PrintTypeReference(stream, type, from, "contents");
					if(nameOnly)
					{
						if(type->fullyQualify)
						{
							PrintString(stream, type->fullName);
						}
						else
						{
							PrintString(stream, type->name);
						}
					}
					else
					{
						PrintTypeName(stream, type, 0);
					}
					fputs("</A>", stream);
					return;
				}
			}
			PrintString(stream, cref + 2);
		}
		else
		{
			PrintString(stream, cref);
		}
	}
}

/*
 * Forward declaration.
 */
static int PrintDocContents(FILE *stream, ILDocType *type,
							ILDocText *contents, int lastWasNL);

#define	LIST_TYPE_BULLET		0
#define	LIST_TYPE_NUMBER		1
#define	LIST_TYPE_TABLE			2

/*
 * Print the contents of a list tag.
 */
static void PrintDocList(FILE *stream, ILDocType *fromType,
					     ILDocText *contents, const char *type,
						 int lastWasNL)
{
	int listType;
	ILDocText *child;
	ILDocText *term;
	ILDocText *description;

	/* Determine the type of list to print */
	if(type && !strcmp(type, "bullet"))
	{
		listType = LIST_TYPE_BULLET;
	}
	else if(type && !strcmp(type, "number"))
	{
		listType = LIST_TYPE_NUMBER;
	}
	else if(type && !strcmp(type, "table"))
	{
		listType = LIST_TYPE_TABLE;
	}
	else
	{
		listType = LIST_TYPE_BULLET;
	}

	/* Print the list header */
	if(!lastWasNL)
	{
		putc('\n', stream);
	}
	if(listType == LIST_TYPE_BULLET)
	{
		fputs("<UL>\n", stream);
	}
	else if(listType == LIST_TYPE_NUMBER)
	{
		fputs("<OL>\n", stream);
	}
	else
	{
		fputs("<TABLE BORDER=\"1\" COLS=\"2\" WIDTH=\"100%\">\n", stream);
	}

	/* Print the list elements */
	child = ILDocTextFirstChild(contents, "TBODY");
	if(child)
	{
		/* Some older lists use this around table bodies */
		child = contents->children;
	}
	else
	{
		child = contents;
	}
	while(child != 0)
	{
		if(child->isTag &&
		   (!strcmp(child->text, "item") ||
		    !strcmp(child->text, "listheader")))
		{
			term = ILDocTextGetChild(child, "term");
			description = ILDocTextGetChild(child, "description");
			if(listType != LIST_TYPE_TABLE)
			{
				fputs("<LI>", stream);
				if(term)
				{
					PrintDocContents(stream, fromType, term->children, 0);
				}
				else
				{
					PrintDocContents(stream, fromType, description->children, 0);
				}
				fputs("</LI>\n", stream);
			}
			else if(!strcmp(child->text, "listheader"))
			{
				fprintf(stream, "<TR BGCOLOR=\"%s\"><TH>", headerColor);
				PrintDocContents(stream, fromType, term->children, 0);
				fputs("</TH><TH>", stream);
				PrintDocContents(stream, fromType, description->children, 0);
				fputs("</TH></TR>\n", stream);
			}
			else
			{
				fputs("<TR><TD>", stream);
				PrintDocContents(stream, fromType, term->children, 0);
				fputs("</TD><TD>", stream);
				PrintDocContents(stream, fromType, description->children, 0);
				fputs("</TD></TR>\n", stream);
			}
		}
		child = child->next;
	}

	/* Print the list footer */
	if(listType == LIST_TYPE_BULLET)
	{
		fputs("</UL>\n", stream);
	}
	else if(listType == LIST_TYPE_NUMBER)
	{
		fputs("</OL>\n", stream);
	}
	else
	{
		fputs("</TABLE>\n", stream);
	}
}

/*
 * Print the contents of a documentation node.
 */
static int PrintDocContents(FILE *stream, ILDocType *type,
						    ILDocText *contents, int lastWasNL)
{
	const char *value;
	while(contents != 0)
	{
		if(contents->isTag)
		{
			/* Tag node */
			if(!strcmp(contents->text, "para"))
			{
				lastWasNL = PrintDocContents(stream, type,
											 contents->children, lastWasNL);
				fputs("<P>\n\n", stream);
				lastWasNL = 1;
			}
			else if(!strcmp(contents->text, "b"))
			{
				fputs("<B>", stream);
				PrintDocContents(stream, type, contents->children, 0);
				fputs("</B>", stream);
				lastWasNL = 0;
			}
			else if(!strcmp(contents->text, "c"))
			{
				fputs("<CODE>", stream);
				PrintDocContents(stream, type, contents->children, 0);
				fputs("</CODE>", stream);
				lastWasNL = 0;
			}
			else if(!strcmp(contents->text, "i"))
			{
				fputs("<I>", stream);
				PrintDocContents(stream, type, contents->children, 0);
				fputs("</I>", stream);
				lastWasNL = 0;
			}
			else if(!strcmp(contents->text, "sub"))
			{
				fputs("<SUB>", stream);
				PrintDocContents(stream, type, contents->children, 0);
				fputs("</SUB>", stream);
				lastWasNL = 0;
			}
			else if(!strcmp(contents->text, "sup"))
			{
				fputs("<SUP>", stream);
				PrintDocContents(stream, type, contents->children, 0);
				fputs("</SUP>", stream);
				lastWasNL = 0;
			}
			else if(!strcmp(contents->text, "subscript"))
			{
				fputs("<SUB>", stream);
				PrintString(stream, ILDocTextGetParam(contents, "term"));
				fputs("</SUB>", stream);
				lastWasNL = 0;
			}
			else if(!strcmp(contents->text, "superscript"))
			{
				fputs("<SUP>", stream);
				PrintString(stream, ILDocTextGetParam(contents, "term"));
				fputs("</SUP>", stream);
				lastWasNL = 0;
			}
			else if(!strcmp(contents->text, "code") ||
			        !strcmp(contents->text, "pre"))
			{
				fputs("<PRE>", stream);
				PrintDocContents(stream, type, contents->children, 0);
				fputs("</PRE>\n", stream);
				lastWasNL = 1;
			}
			else if(!strcmp(contents->text, "see"))
			{
				value = ILDocTextGetParam(contents, "langword");
				if(value)
				{
					fputs("<CODE>", stream);
					PrintString(stream, value);
					fputs("</CODE>\n", stream);
				}
				else
				{
					PrintCRef(stream, type->tree,
							  ILDocTextGetParam(contents, "cref"), type, 1);
				}
				lastWasNL = 0;
			}
			else if(!strcmp(contents->text, "paramref"))
			{
				fputs("<I>", stream);
				PrintString(stream, ILDocTextGetParam(contents, "name"));
				fputs("</I>", stream);
				lastWasNL = 0;
			}
			else if(!strcmp(contents->text, "block") ||
					!strcmp(contents->text, "note"))
			{
				fputs("[", stream);
				value = ILDocTextGetParam(contents, "type");
				fputs("<I>", stream);
				if(!value || *value == '\0')
				{
					fputs("Note", stream);
				}
				else
				{
					if(*value >= 'a' && *value <= 'z')
					{
						putc(*value - 'a' + 'A', stream);
						PrintString(stream, value + 1);
					}
					else
					{
						PrintString(stream, value);
					}
				}
				fputs("</I>", stream);
				value = ILDocTextGetParam(contents, "subset");
				if(value && strcmp(value, "none") != 0)
				{
					fputs(" (", stream);
					PrintString(stream, value);
					fputs("): ", stream);
				}
				else
				{
					fputs(": ", stream);
				}
				PrintDocContents(stream, type, contents->children, 0);
				fputs("]<P>\n\n", stream);
			}
			else if(!strcmp(contents->text, "list"))
			{
				PrintDocList(stream, type, contents->children,
							 ILDocTextGetParam(contents, "type"),
							 lastWasNL);
				lastWasNL = 1;
			}
			else
			{
				lastWasNL = PrintDocContents(stream, type,
											 contents->children,
											 lastWasNL);
			}
		}
		else if(!ILDocTextAllSpaces(contents))
		{
			/* Ordinary text node */
			lastWasNL = PrintString(stream, contents->text);
		}
		contents = contents->next;
	}
	return lastWasNL;
}

/*
 * Print the contents of a documentation node, indented an extra level.
 */
static void PrintDocContentsIndent(FILE *stream, ILDocType *type,
								   ILDocText *contents)
{
	fputs("<BLOCKQUOTE>\n", stream);
	if(!PrintDocContents(stream, type, contents, 1))
	{
		putc('\n', stream);
	}
	fputs("</BLOCKQUOTE>\n\n", stream);
}

/*
 * Print the documentation for a node.
 */
static void PrintDocs(FILE *stream, ILDocText *doc,
					  ILDocType *type, ILDocMember *member)
{
	ILDocText *child;
	int example;
	int needComma;
	const char *cref;
	ILDocAttribute *attr;

	/* Print the summary */
	child = ILDocTextFirstChild(doc, "summary");
	if(child)
	{
		fputs("<H4>Summary</H4>\n\n", stream);
		PrintDocContentsIndent(stream, type, child->children);
	}

	/* Print the parameter, return, and value information */
	child = ILDocTextFirstChild(doc, "param");
	if(child)
	{
		fputs("<H4>Parameters</H4>\n\n", stream);
		fputs("<BLOCKQUOTE>\n", stream);
		fputs("<DL>\n", stream);
		while(child != 0)
		{
			/* Print the name of the parameter */
			fputs("<DT>", stream);
			PrintString(stream, ILDocTextGetParam(child, "name"));
			fputs("</DT>\n", stream);

			/* Print the description of the parameter */
			fputs("<DD>", stream);
			PrintDocContents(stream, type, child->children, 1);
			fputs("</DD>\n", stream);

			/* Move on to the next parameter */
			child = ILDocTextNextChild(child, "param");
		}
		fputs("</DL>\n", stream);
		fputs("</BLOCKQUOTE>\n\n", stream);
	}
	child = ILDocTextFirstChild(doc, "returns");
	if(child)
	{
		fputs("<H4>Return Value</H4>\n\n", stream);
		PrintDocContentsIndent(stream, type, child->children);
	}
	child = ILDocTextFirstChild(doc, "value");
	if(child)
	{
		fputs("<H4>Property Value</H4>\n\n", stream);
		PrintDocContentsIndent(stream, type, child->children);
	}

	/* Print the exceptions */
	child = ILDocTextFirstChild(doc, "exception");
	if(child)
	{
		fputs("<H4>Exceptions</H4>\n\n", stream);
		fputs("<BLOCKQUOTE>\n", stream);
		fputs("<TABLE BORDER=\"1\" COLS=\"2\" WIDTH=\"100%\">\n", stream);
		fprintf(stream, "<TR BGCOLOR=\"%s\"><TH>Exception Type</TH>"
						"<TH>Condition</TH></TR>\n", headerColor);
		while(child != 0)
		{
			fputs("<TR><TD>", stream);
			PrintCRef(stream, type->tree,
					  ILDocTextGetParam(child, "cref"), type, 1);
			fputs("</TD><TD>", stream);
			PrintDocContents(stream, type, child->children, 0);
			fputs("</TD></TR>\n", stream);
			child = ILDocTextNextChild(child, "exception");
		}
		fputs("</TABLE>\n", stream);
		fputs("</BLOCKQUOTE>\n\n", stream);
	}

	/* Print the description */
	child = ILDocTextFirstChild(doc, "remarks");
	if(child)
	{
		fputs("<H4>Description</H4>\n\n", stream);
		PrintDocContentsIndent(stream, type, child->children);
	}

	/* Print any additional information */
	child = ILDocTextFirstChild(doc, "devdoc");
	if(child)
	{
		fputs("<H4>Additional Information</H4>\n\n", stream);
		PrintDocContentsIndent(stream, type, child->children);
	}

	/* Print the examples */
	child = ILDocTextFirstChild(doc, "example");
	if(child)
	{
		if(ILDocTextNextChild(child, "example") != 0)
		{
			/* There are multiple examples */
			fputs("<H4>Examples</H4>\n\n", stream);
			fputs("<BLOCKQUOTE>\n", stream);
			example = 1;
			while(child != 0)
			{
				fprintf(stream, "Example %d<P>\n\n", example);
				++example;
				PrintDocContents(stream, type, child->children, 1);
				fputs("<P>\n\n", stream);
				child = ILDocTextNextChild(child, "example");
			}
			fputs("</BLOCKQUOTE>\n\n", stream);
		}
		else
		{
			/* There is only one example */
			fputs("<H4>Example</H4>\n\n", stream);
			PrintDocContentsIndent(stream, type, child->children);
		}
	}

	/* Print the attributes for the item */
	if(member)
	{
		attr = member->attributes;
	}
	else
	{
		attr = type->attributes;
	}
	if(attr)
	{
		fputs("<H4>Attributes</H4>\n\n", stream);
		fputs("<BLOCKQUOTE>\n", stream);
		while(attr != 0)
		{
			fputs("<CODE>", stream);
			PrintString(stream, attr->name);
			fputs("</CODE>", stream);
			attr = attr->next;
			if(attr)
			{
				fputs(", ", stream);
			}
		}
		fputs("\n</BLOCKQUOTE>\n\n", stream);
	}

	/* Print the library for the member, if different from the type's */
	if(member && member->libraryName)
	{
		if(!(member->type->library->name) ||
		   strcmp(member->type->library->name, member->libraryName) != 0)
		{
			fputs("<H4>Library</H4>\n\n", stream);
			fputs("<BLOCKQUOTE>\n", stream);
			PrintString(stream, member->libraryName);
			fputs("\n</BLOCKQUOTE>\n\n", stream);
		}
	}

	/* Print the "See Also" section */
	fputs("<H4>See Also</H4>\n\n", stream);
	fputs("<BLOCKQUOTE>\n", stream);
	needComma = 0;

	child = ILDocTextFirstChild(doc, "seealso");
	while(child != 0)
	{
		cref = ILDocTextGetParam(child, "cref");
		if(cref)
		{
			if(needComma)
			{
				fputs(", ", stream);
			}
			PrintCRef(stream, type->tree, cref, type, 0);
			needComma = 1;
		}
		child = ILDocTextNextChild(child, "seealso");
	}
	if(member)
	{
		if(needComma)
		{
			fputs(", ", stream);
		}
		PrintTypeReference(stream, type, type, "contents");
		PrintTypeName(stream, member->type, 0);
		fputs("</A>", stream);
		needComma = 1;
	}
	if(type && type->namespace)
	{
		if(needComma)
		{
			fputs(", ", stream);
		}
		PrintNamespaceReference(stream, type->namespace, type, "members");
		PrintString(stream, type->namespace->name);
		PrintString(stream, " Namespace");
		fputs("</A>", stream);
	}
	fputs("\n</BLOCKQUOTE>\n\n", stream);
}

/*
 * Print spaces for an indent level.
 */
static void PrintIndent(FILE *stream, int indent)
{
	while(indent > 0)
	{
		fputs("&nbsp;", stream);
		--indent;
	}
}

/*
 * Print the inheritance tree for a type.
 */
static int PrintInheritTree(FILE *stream, ILDocType *type,
						    char *base, char *excluded, int indent)
{
	ILDocType *baseType;
	if(excluded)
	{
		baseType = ILDocTypeFind(type->tree, excluded);
	}
	else
	{
		baseType = ILDocTypeFind(type->tree, base);
	}
	if(baseType)
	{
		if(baseType->baseType)
		{
			indent = PrintInheritTree(stream, type,
									  baseType->baseType,
									  baseType->excludedBaseType,
									  indent);
		}
		PrintIndent(stream, indent);
		PrintTypeReference(stream, baseType, type, "contents");
		if(baseType->fullyQualify)
		{
			PrintString(stream, baseType->fullName);
		}
		else
		{
			PrintString(stream, baseType->name);
		}
		fputs("</A>", stream);
		if(excluded)
		{
			PrintString(stream, " (excluded)");
		}
		fputs("<BR>\n", stream);
	}
	else
	{
		PrintIndent(stream, indent);
		if(excluded)
		{
			PrintString(stream, excluded);
			PrintString(stream, " (excluded)");
		}
		else
		{
			PrintString(stream, base);
		}
		fputs("<BR>\n", stream);
	}
	return indent + 2;
}

/*
 * Convert the contents of a type into texinfo.
 */
static void ConvertType(FILE *stream, ILDocType *type,
						const char *namespaceName,
						const char *outputPath)
{
	ILDocMember *member;
	ILDocMemberType lastMemberType;
	const char *heading;
	FILE *memberStream;
	ILDocInterface *interface;
	ILDocType *tempType;
	int indent;

	/* Validate that the type node is more or less meaningful */
	if(!(type->name) || !(type->fullName))
	{
		return;
	}

	/* Output the node heading */
	if(isSingleFile)
	{
		fputs("<HR>\n\n", stream);
	}
	PrintTypeAnchor(stream, type);
	fputs("<H3>", stream);
	PrintTypeName(stream, type, 0);
	fputs("</H3>\n\n", stream);

	/* Print the C# definition for the type */
	PrintSignature(stream, type->csSignature);

	/* Print the type's inheritance relationships */
	if(type->baseType || type->interfaces)
	{
		fputs("<H4>Base Types</H4>\n\n", stream);
		fputs("<BLOCKQUOTE>\n", stream);
		if(type->baseType)
		{
			indent = PrintInheritTree(stream, type, type->baseType,
									  type->excludedBaseType, 0);
			PrintIndent(stream, indent);
			if(type->fullyQualify)
			{
				PrintString(stream, type->fullName);
			}
			else
			{
				PrintString(stream, type->name);
			}
			fputs("<P>\n\n", stream);
		}
		if(type->interfaces)
		{
			fputs("This type implements ", stream);
			interface = type->interfaces;
			while(interface != 0)
			{
				if(interface != type->interfaces)
				{
					if(interface->next != 0)
					{
						/* Three or more interfaces */
						fputs(", ", stream);
					}
					else if(type->interfaces->next == interface)
					{
						/* Last when exactly two interfaces */
						fputs(" and ", stream);
					}
					else
					{
						/* Last when three or more interfaces */
						fputs(", and ", stream);
					}
				}
				tempType = ILDocTypeFind(type->tree, interface->name);
				if(tempType)
				{
					PrintTypeReference(stream, tempType, type, "contents");
					if(tempType->fullyQualify)
					{
						PrintString(stream, tempType->fullName);
					}
					else
					{
						PrintString(stream, tempType->name);
					}
					fputs("</A>", stream);
				}
				else
				{
					PrintString(stream, interface->name);
				}
				interface = interface->next;
			}
			fputs(".\n", stream);
		}
		fputs("</BLOCKQUOTE>\n\n", stream);
	}

	/* Print the assembly information for the type */
	if(type->assembly)
	{
		fputs("<H4>Assembly</H4>\n\n", stream);
		fputs("<BLOCKQUOTE>\n", stream);
		PrintString(stream, type->assembly);
		fputs("\n</BLOCKQUOTE>\n\n", stream);
	}

	/* Print the library information for the type */
	if(type->library->name)
	{
		fputs("<H4>Library</H4>\n\n", stream);
		fputs("<BLOCKQUOTE>\n", stream);
		PrintString(stream, type->library->name);
		fputs("\n</BLOCKQUOTE>\n\n", stream);
	}

	/* Print the documentation for the type */
	PrintDocs(stream, type->doc, type, 0);

	/* Output a menu for all of the type members */
	member = type->members;
	if(member != 0)
	{
		fputs("<H4>Members</H4>\n\n", stream);
		fputs("<BLOCKQUOTE>\n", stream);
		lastMemberType = ILDocMemberType_Unknown;
		while(member != 0)
		{
			heading = 0;
			switch(member->memberType)
			{
				case ILDocMemberType_Field:
				{
					if(lastMemberType != ILDocMemberType_Field)
					{
						heading = "Fields";
					}
				}
				break;

				case ILDocMemberType_Method:
				{
					/* Print a new section header if necessary */
					if(lastMemberType != ILDocMemberType_Method)
					{
						heading = "Methods";
					}
				}
				break;

				case ILDocMemberType_Constructor:
				{
					/* Print a new section header if necessary */
					if(lastMemberType != ILDocMemberType_Constructor)
					{
						heading = "Constructors";
					}
				}
				break;

				case ILDocMemberType_Property:
				{
					/* Print a new section header if necessary */
					if(lastMemberType != ILDocMemberType_Property)
					{
						heading = "Properties";
					}
				}
				break;

				case ILDocMemberType_Event:
				{
					/* Print a new section header if necessary */
					if(lastMemberType != ILDocMemberType_Event)
					{
						heading = "Events";
					}
				}
				break;

				case ILDocMemberType_Unknown: break;
			}
			if(heading)
			{
				fputs("<P>\n\n", stream);
				if(type->fullyQualify)
				{
					PrintString(stream, type->fullName);
				}
				else
				{
					PrintString(stream, type->name);
				}
				putc(' ', stream);
				fputs(heading, stream);
				fputs("<P>\n\n", stream);
			}
			PrintMemberReference(stream, member, type, "contents");
			PrintMemberName(stream, member, 0);
			fputs("</A><BR>\n", stream);
			lastMemberType = member->memberType;
			member = member->next;
		}
		fputs("</BLOCKQUOTE>\n\n", stream);
	}

	/* Print information about each of the members */
	member = type->members;
	while(member != 0)
	{
		/* Print the section header */
		if(isSingleFile || !separateMembers)
		{
			/* The member is in the same file as its type */
			fputs("<HR>\n\n", stream);
			memberStream = stream;
		}
		else
		{
			/* The member is in a different file */
			memberStream = CreateMemberStream(member, outputPath);
			fputs("<HTML>\n<HEAD>\n<TITLE>", memberStream);
			PrintMemberName(memberStream, member, 0);
			fputs("</TITLE>\n</HEAD>\n", memberStream);
			fprintf(memberStream, "<BODY BGCOLOR=\"%s\">\n", pageColor);
		}
		PrintMemberAnchor(memberStream, member);
		fputs("<H3>", memberStream);
		PrintMemberName(memberStream, member, 0);
		fputs("</H3>\n\n", memberStream);

		/* Print the signature for the member */
		PrintSignature(memberStream, member->csSignature);

		if (sourceXrefTags && (member->memberType == ILDocMemberType_Method || member->memberType == ILDocMemberType_Constructor))
		{
			fprintf(memberStream, "<a HREF=\"%s/%s#", sourceXrefDir, sourceXrefName);
			if (type->namespace)
			{
				fprintf(memberStream, "%s.", type->namespace->name);
			}
			fputs(type->name, memberStream);
			if (member->memberType == ILDocMemberType_Method)
			{
				fprintf(memberStream, ".%s", member->name);
			}
			fputs("\">Source Code</a>", memberStream);
		}

		/* Print the documentation for the member */
		PrintDocs(memberStream, member->doc, type, member);

		/* Finalize the member-specific stream */
		if(memberStream != stream)
		{
			fputs("</BODY>\n</HTML>\n", memberStream);
			fclose(memberStream);
		}

		/* Advance to the next member */
		member = member->next;
	}
}

int ILDocConvert(ILDocTree *tree, int numInputs, char **inputs,
				 char *outputPath, const char *progname)
{
	FILE *stream;
	FILE *nsStream;
	FILE *typeStream;
	int closeStream;
	ILDocNamespace *namespace;
	char *namespaceName;
	ILDocType *type;
	const char *title;
	const char *color;
	int	len;

	sourceXrefTags = ILDocFlagSet("source-xref-tags");
	sourceXrefDir = (char *) ILDocFlagValue("source-xref-dir");
	if (!sourceXrefDir)
	{
		sourceXrefDir = "..";
	}
	else
	{
		sourceXrefTags = 1;		/* implicit enable if directory specified */
		/* Adjust the first input filename to end in ".html" */
		len = strlen(inputs[0]);
		while(len > 0 && inputs[0][len - 1] != '/' &&
			  inputs[0][len - 1] != '\\' &&
			  inputs[0][len - 1] != '.')
		{
			--len;
		}
		if(len > 0 && inputs[0][len - 1] == '.')
		{
			sourceXrefName = (char *)ILMalloc(len + 5);
			if(!sourceXrefName)
			{
				ILDocOutOfMemory(progname);
			}
			ILMemCpy(sourceXrefName, inputs[0], len);
			strcpy(sourceXrefName + len, "html");
		}
		else
		{
			sourceXrefName = (char *)ILMalloc(strlen(inputs[0]) + 6);
			if(!sourceXrefName)
			{
				ILDocOutOfMemory(progname);
			}
			strcpy(sourceXrefName, inputs[0]);
			strcat(sourceXrefName, ".html");
		}
	}

	/* Attempt to open the output stream */
	isSingleFile = ILDocFlagSet("single-file");
	if(isSingleFile)
	{
		if(!strcmp(outputPath, "-"))
		{
			stream = stdout;
			closeStream = 0;
			outputPath = inputs[0];
			if(!strcmp(outputPath, "-"))
			{
				outputPath = "stdout";
			}
		}
		else if((stream = fopen(outputPath, "w")) == NULL)
		{
			perror(outputPath);
			return 0;
		}
		else
		{
			closeStream = 1;
		}
	}
	else
	{
		stream = stdout;
		closeStream = 0;
	}

	/* Process HTML-specific options */
	separateMembers = ILDocFlagSet("separate-members");
	namespaceDirectories = !ILDocFlagSet("no-namespace-directories");
	useFrames = (!ILDocFlagSet("no-frames") && !isSingleFile);
	title = ILDocFlagValue("title");
	if(!title)
	{
		if(isSingleFile)
		{
			title = outputPath;
		}
		else
		{
			title = inputs[0];
		}
	}
	color = ILDocFlagValue("page-color");
	if(color && *color != '\0')
	{
		pageColor = color;
	}
	color = ILDocFlagValue("defn-color");
	if(color && *color != '\0')
	{
		cartoucheColor = color;
	}
	color = ILDocFlagValue("header-color");
	if(color && *color != '\0')
	{
		headerColor = color;
	}
	nextIndex = 1;

	/* Set up the frame set and index pages */
	if(isSingleFile)
	{
		/* No frames required */
		fputs("<HTML>\n<HEAD><TITLE>", stream);
		PrintString(stream, title);
		fputs("</TITLE></HEAD>\n", stream);
		fprintf(stream, "<BODY BGCOLOR=\"%s\">\n", pageColor);
		fputs("<H1>", stream);
		PrintString(stream, title);
		fputs("</H1>\n\n", stream);
	}
	else if(useFrames)
	{
		/* Output the frame set to "index.html" */
		stream = CreateHTMLStream("index.html", outputPath);
		fputs("<HTML>\n<HEAD><TITLE>", stream);
		PrintString(stream, title);
		fputs("</TITLE></HEAD>\n", stream);
		fputs("<FRAMESET COLS=\"150,*\">\n", stream);
		fputs("  <FRAMESET ROWS=\"50%,50%\">\n", stream);
		fputs("    <FRAME SRC=\"namespaces.html\" NAME=\"namespaces\">\n",
		      stream);
		if(tree->namespaces)
		{
			/* Initialize the members frame with the first namespace */
			fputs("    <FRAME SRC=\"", stream);
			PrintNamespaceFilename(stream, tree->namespaces);
			fputs("\" NAME=\"members\">\n", stream);
		}
		else
		{
			/* There are no namespaces, so initialize the member
			   frame to contain an empty document */
			fputs("    <FRAME SRC=\"empty.html\" NAME=\"members\">\n", stream);
		}
		fputs("  </FRAMESET>\n", stream);
		fputs("  <FRAME SRC=\"begin.html\" NAME=\"contents\">\n", stream);
		fputs("</FRAMESET>\n", stream);
		fputs("</HTML>\n", stream);
		fclose(stream);

		/* Create the "empty.html" file if there are no namespaces */
		if(!(tree->namespaces))
		{
			stream = CreateHTMLStream("empty.html", outputPath);
			fprintf(stream, "<HTML>\n<BODY BGCOLOR=\"%s\">\n</BODY>\n</HTML>\n",
					pageColor);
			fclose(stream);
		}

		/* Create the "namespaces.html" file */
		stream = CreateHTMLStream("namespaces.html", outputPath);
		fprintf(stream, "<HTML>\n<BODY BGCOLOR=\"%s\">\n",
				pageColor);
		fputs("<I>Namespaces</I><P>\n", stream);
		namespace = tree->namespaces;
		while(namespace != 0)
		{
			PrintNamespaceReference(stream, namespace, 0, "members");
			if(namespace->name[0] == '\0')
			{
				PrintString(stream, "Global");
			}
			else
			{
				PrintString(stream, namespace->name);
			}
			fputs("</A><BR>\n", stream);
			namespace = namespace->next;
		}
		fputs("</BODY>\n</HTML>\n", stream);
		fclose(stream);

		/* Create the "begin.html" file */
		stream = CreateHTMLStream("begin.html", outputPath);
		fputs("<HTML>\n<HEAD><TITLE>", stream);
		PrintString(stream, title);
		fputs("</TITLE></HEAD>\n", stream);
		fprintf(stream, "<BODY BGCOLOR=\"%s\">\n", pageColor);
		fputs("<H1>", stream);
		PrintString(stream, title);
		fputs("</H1>\n\n", stream);
	}
	else
	{
		/* Not using frames, so create the main "index.html" */
		stream = CreateHTMLStream("index.html", outputPath);
		fputs("<HTML>\n<HEAD><TITLE>", stream);
		PrintString(stream, title);
		fputs("</TITLE></HEAD>\n", stream);
		fprintf(stream, "<BODY BGCOLOR=\"%s\">\n", pageColor);
		fputs("<H1>", stream);
		PrintString(stream, title);
		fputs("</H1>\n\n", stream);
	}

	/* Dump a menu that refers to all namespaces in the documentation tree */
	fputs("<BLOCKQUOTE>\n", stream);
	namespace = tree->namespaces;
	while(namespace != 0)
	{
		PrintNamespaceReference(stream, namespace, 0, "members");
		if(namespace->name[0] == '\0')
		{
			PrintString(stream, "Global");
		}
		else
		{
			PrintString(stream, namespace->name);
		}
		PrintString(stream, " Namespace");
		fputs("</A><BR>\n", stream);
		namespace = namespace->next;
	}
	fputs("</BLOCKQUOTE>\n\n", stream);

	/* Dump all types within the documentation tree, ordered by namespace */
	namespace = tree->namespaces;
	while(namespace != 0)
	{
		/* Get the full form of the namespace name */
		if(namespace->name[0] == '\0')
		{
			namespaceName = "Global";
		}
		else
		{
			namespaceName = namespace->name;
		}

		/* Get the stream to use to dump the namespace */
		if(isSingleFile)
		{
			nsStream = stream;
		}
		else if(useFrames)
		{
			nsStream = CreateNamespaceStream(namespaceName, outputPath);
			fprintf(nsStream, "<HTML>\n<BODY BGCOLOR=\"%s\">\n",
				    pageColor);
		}
		else
		{
			nsStream = CreateNamespaceStream(namespaceName, outputPath);
			fputs("<HTML>\n<HEAD>\n<TITLE>", nsStream);
			PrintString(nsStream, namespaceName);
			fputs(" Namespace</TITLE>\n</HEAD>\n", nsStream);
			fprintf(nsStream, "<BODY BGCOLOR=\"%s\">\n", pageColor);
		}

		/* Print the namespace section header */
		PrintNamespaceAnchor(nsStream, namespace);
		if(useFrames)
		{
			fputs("<I>", nsStream);
		}
		else
		{
			fputs("<H2>", nsStream);
		}
		PrintString(nsStream, namespaceName);
		PrintString(nsStream, " Namespace");
		if(useFrames)
		{
			fputs("</I><P>\n\n", nsStream);
		}
		else
		{
			fputs("</H2>\n\n", nsStream);
		}

		/* Print a menu of all types in the namespace */
		if(!useFrames)
		{
			fputs("<BLOCKQUOTE>\n", nsStream);
		}
		type = namespace->types;
		while(type != 0)
		{
			PrintTypeReference(nsStream, type, type, "contents");
			if(useFrames)
			{
				/* Print the short form of the name when using frames */
				PrintString(nsStream, type->name);
			}
			else
			{
				PrintTypeName(nsStream, type, 0);
			}
			fputs("</A><BR>\n", nsStream);
			type = type->nextNamespace;
		}
		if(!useFrames)
		{
			fputs("</BLOCKQUOTE>\n\n", nsStream);
		}

		/* Close the namespace stream if multi-file output */
		if(!isSingleFile)
		{
			fputs("</BODY>\n</HTML>\n", nsStream);
			fclose(nsStream);
		}

		/* Convert all types into HTML */
		if(isSingleFile)
		{
			type = namespace->types;
			while(type != 0)
			{
				ConvertType(stream, type, namespaceName, outputPath);
				type = type->nextNamespace;
			}
		}
		else
		{
			type = namespace->types;
			while(type != 0)
			{
				/* Create the type stream and write out the header */
				typeStream = CreateTypeStream(type, outputPath);
				fputs("<HTML>\n<HEAD>\n<TITLE>", typeStream);
				PrintTypeName(typeStream, type, 0);
				fputs("</TITLE>\n</HEAD>\n", typeStream);
				fprintf(typeStream, "<BODY BGCOLOR=\"%s\">\n", pageColor);

				/* Convert the type and its members */
				ConvertType(typeStream, type, namespaceName, outputPath);

				/* Write out the type footer */
				fputs("</BODY>\n</HTML>\n", typeStream);
				fclose(typeStream);

				/* Move on to the next type in the namespace */
				type = type->nextNamespace;
			}
		}

		/* Move on to the next namespace */
		namespace = namespace->next;
	}

	/* Print the end matter for the main HTML stream */
	fputs("</BODY>\n</HTML>\n", stream);

	/* Clean up and exit */
	if(closeStream)
	{
		fclose(stream);
	}
	return 1;
}

#ifdef	__cplusplus
};
#endif
