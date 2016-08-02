/*
 * resgen.c - Resource file generator and reader.
 *
 * Copyright (C) 2001, 2003, 2009  Southern Storm Software, Pty Ltd.
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
#include "resgen.h"
#include "il_system.h"
#include "il_image.h"
#include "il_program.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-t", 't', 0, 0, 0},
	{"-r", 'r', 0, 0, 0},
	{"-i", 'i', 0, 0, 0},
	{"-x", 'x', 0, 0, 0},
	{"-p", 'p', 0, 0, 0},
	{"-T", 'T', 0, 0, 0},
	{"-R", 'R', 0, 0, 0},
	{"-X", 'X', 0, 0, 0},
	{"-P", 'P', 0, 0, 0},
	{"-s", 's', 0, 0, 0},
	{"-l", 'l', 0, 0, 0},
	{"-v", 'v', 0, 0, 0},
	{"-e", 'e', 0, 0, 0},
	{"--text-input", 't', 0,
		"--text-input  or -t",
		"Input files contain text resources."},
	{"--res-input", 'r', 0,
		"--res-input   or -r",
		"Input files contain binary resources."},
	{"--il-input", 'i', 0,
		"--il-input    or -i",
		"Input files contain IL images."},
	{"--xml-input", 'x', 0,
		"--xml-input   or -x",
		"Input files contain XML resources."},
	{"--po-input", 'p', 0,
		"--po-input    or -p",
		"Input files contain GNU gettext .po resources."},
	{"--text-output", 'T', 0,
		"--text-output or -T",
		"Write text resources to the output file."},
	{"--res-output", 'R', 0,
		"--res-output  or -R",
		"Write binary resources to the output file."},
	{"--xml-output", 'X', 0,
		"--xml-output  or -X",
		"Write XML resources to the output file."},
	{"--po-output", 'P', 0,
		"--po-output   or -P",
		"Write GNU gettext .po resources to the output file."},
	{"--latin1", 'l', 0,
		"--latin1      or -l",
		"Interpret text and .po files as Latin-1 rather than UTF-8."},
	{"--sort-names", 's', 0,
		"--sort-names  or -s",
		"Sort the resources by name before writing text or .po output."},
	{"--extract", 'e', 0,
		"--extract     or -X",
		"Extract binary resources without converting them."},
	{"--version", 'v', 0,
		"--version     or -v",
		"Print the version of the program"},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{"-c", 'c', 1, 0, 0},
	{0, 0, 0, 0, 0}
};

/*
 * Input and output formats.
 */
#define	FORMAT_GUESS		0
#define	FORMAT_TEXT			1
#define	FORMAT_RES_BINARY	2
#define	FORMAT_IL			3
#define	FORMAT_XML			4
#define	FORMAT_PO			5

static void usage(const char *progname);
static void version(void);
static int guessFormat(const char *filename, int isOutput);
static int loadResources(const char *filename, FILE *stream,
						 ILContext *context, int format, int latin1);
static int extractResource(const char *filename, const char *resourceName);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	int inputFormat = FORMAT_GUESS;
	int outputFormat = FORMAT_GUESS;
	int latin1 = 0;
	int sortNames = 0;
	int extract = 0;
	char *outputFile = 0;
	int currFormat;
	int sawStdin;
	int state, opt;
	char *param;
	FILE *file;
	int errors;
	ILContext *context;

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'c':
			{
				/* Probably "-compile", which we ignore */
			}
			break;

			case 't':
			{
				inputFormat = FORMAT_TEXT;
			}
			break;

			case 'r':
			{
				inputFormat = FORMAT_RES_BINARY;
			}
			break;

			case 'i':
			{
				inputFormat = FORMAT_IL;
			}
			break;

			case 'x':
			{
				inputFormat = FORMAT_XML;
			}
			break;

			case 'p':
			{
				inputFormat = FORMAT_PO;
			}
			break;

			case 'T':
			{
				outputFormat = FORMAT_TEXT;
			}
			break;

			case 'R':
			{
				outputFormat = FORMAT_RES_BINARY;
			}
			break;

			case 'X':
			{
				outputFormat = FORMAT_XML;
			}
			break;

			case 'P':
			{
				outputFormat = FORMAT_PO;
			}
			break;

			case 'l':
			{
				latin1 = 1;
			}
			break;

			case 's':
			{
				sortNames = 1;
			}
			break;

			case 'e':
			{
				extract = 1;
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

	/* Skip a "/compile" option, if present, for backwards
	   compatibility with tools from other CLI vendors */
	if(argc >= 2 && !ILStrICmp(argv[1], "/compile"))
	{
		++argv;
		--argc;
	}

	/* We need at least one input file and one output file argument */
	if(argc <= 2)
	{
		usage(progname);
		return 1;
	}

	/* Handle the "--extract" case, which is different from usual */
	if(extract)
	{
		if(argc != 3)
		{
			usage(progname);
			return 1;
		}
		return extractResource(argv[1], argv[2]);
	}

	/* Strip the output file from the command-line */
	outputFile = argv[argc - 1];
	--argc;

	/* Create a context to use for image loading */
	context = ILContextCreate();
	if(!context)
	{
		fprintf(stderr, "%s: out of memory\n", progname);
		return 1;
	}

	/* Load resource information from the input files */
	sawStdin = 0;
	errors = 0;
	while(argc > 1)
	{
		if(!strcmp(argv[1], "-"))
		{
			/* Dump the contents of stdin, but only once */
			if(!sawStdin)
			{
				currFormat = ((inputFormat != FORMAT_GUESS)
									? inputFormat : FORMAT_TEXT);
				errors |= loadResources
					("stdin", stdin, context, currFormat, latin1);
				sawStdin = 1;
			}
		}
		else
		{
			/* Dump the contents of a regular file */
			if((file = fopen(argv[1], "rb")) == NULL)
			{
				/* Try again in case libc did not understand the 'b' */
				if((file = fopen(argv[1], "r")) == NULL)
				{
					perror(argv[1]);
					errors = 1;
					++argv;
					--argc;
					continue;
				}
			}
			currFormat = ((inputFormat != FORMAT_GUESS)
								? inputFormat : guessFormat(argv[1], 0));
			errors |= loadResources
				(argv[1], file, context, currFormat, latin1);
			fclose(file);
		}
		++argv;
		--argc;
	}

	/* Destroy the context */
	ILContextDestroy(context);

	/* Bail out if there were errors while parsing the input files */
	if(errors)
	{
		return errors;
	}

	/* Write the resources to the output file */
	if(outputFormat == FORMAT_GUESS)
	{
		if(!strcmp(outputFile, "-"))
		{
			outputFormat = FORMAT_TEXT;
		}
		else
		{
			outputFormat = guessFormat(outputFile, 1);
		}
	}
	switch(outputFormat)
	{
		case FORMAT_TEXT:
		{
			if(!strcmp(outputFile, "-"))
			{
				/* Write text resources to stdout */
				if(sortNames)
				{
					ILResWriteSortedText(stdout, latin1);
				}
				else
				{
					ILResWriteText(stdout, latin1);
				}
			}
			else
			{
				/* Write text resources to a specified file */
				if((file = fopen(outputFile, "w")) == NULL)
				{
					perror(outputFile);
					return 1;
				}
				if(sortNames)
				{
					ILResWriteSortedText(file, latin1);
				}
				else
				{
					ILResWriteText(file, latin1);
				}
				fclose(file);
			}
		}
		break;

		case FORMAT_RES_BINARY:
		{
			if(!strcmp(outputFile, "-"))
			{
				/* Write binary resources to stdout */
				ILResWriteBinary(stdout);
			}
			else
			{
				/* Write binary resources to a specified file */
				/* BUG 1841: Support Cygwin/text-mode file systems */
				if((file = fopen(outputFile, "wb")) == NULL) 
				{
					if((file = fopen(outputFile, "w")) == NULL)
					{
						perror(outputFile);
						return 1;
					}
				}
				ILResWriteBinary(file);
				fclose(file);
			}
		}
		break;

		case FORMAT_XML:
		{
			if(!strcmp(outputFile, "-"))
			{
				/* Write XML resources to stdout */
				ILResWriteXML(stdout);
			}
			else
			{
				/* Write XML resources to a specified file */
				if((file = fopen(outputFile, "w")) == NULL)
				{
					perror(outputFile);
					return 1;
				}
				ILResWriteXML(file);
				fclose(file);
			}
		}
		break;

		case FORMAT_PO:
		{
			if(!strcmp(outputFile, "-"))
			{
				/* Write .po resources to stdout */
				if(sortNames)
				{
					ILResWriteSortedPO(stdout, latin1);
				}
				else
				{
					ILResWritePO(stdout, latin1);
				}
			}
			else
			{
				/* Write .po resources to a specified file */
				if((file = fopen(outputFile, "w")) == NULL)
				{
					perror(outputFile);
					return 1;
				}
				if(sortNames)
				{
					ILResWriteSortedPO(file, latin1);
				}
				else
				{
					ILResWritePO(file, latin1);
				}
				fclose(file);
			}
		}
		break;

		default:
		{
			fprintf(stderr, "%s: unknown output resource format\n",
					outputFile);
			return 1;
		}
		/* Not reached */
	}
	
	/* Done */
	return 0;
}

static void usage(const char *progname)
{
	fprintf(stdout, "RESGEN " VERSION " - IL Resource Generation Utility\n");
	fprintf(stdout, "Copyright (c) 2001, 2003 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input ... output\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("RESGEN " VERSION " - IL Resource Generation Utility\n");
	printf("Copyright (c) 2001, 2003 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("RESGEN comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

void ILResOutOfMemory(void)
{
	fputs("virtual memory exhausted\n", stderr);
	exit(1);
}

/*
 * Guess the format of a file from its extension.  Returns
 * FORMAT_GUESS if we cannot determine the format.
 */
static int guessFormat(const char *filename, int isOutput)
{
	int len = strlen(filename);
	while(len > 0 && filename[len - 1] != '.')
	{
		--len;
	}
	if(len <= 0)
	{
		return FORMAT_GUESS;
	}
	filename += len;
	if(!ILStrICmp(filename, "txt") ||
	   !ILStrICmp(filename, "text"))
	{
		return FORMAT_TEXT;
	}
	if(!ILStrICmp(filename, "resx") ||
	   !ILStrICmp(filename, "xml"))
	{
		return FORMAT_XML;
	}
	else if(!ILStrICmp(filename, "exe") ||
	        !ILStrICmp(filename, "dll") ||
			!ILStrICmp(filename, "obj") ||
			!ILStrICmp(filename, "o"))
	{
		return (isOutput ? FORMAT_GUESS : FORMAT_IL);
	}
	else if(!ILStrICmp(filename, "resources"))
	{
		return FORMAT_RES_BINARY;
	}
	else if(!ILStrICmp(filename, "po"))
	{
		return FORMAT_PO;
	}
	else
	{
		return FORMAT_GUESS;
	}
}

/*
 * Global hash table, that holds all of the input strings.
 */
ILResHashEntry *ILResHashTable[IL_RES_HASH_TABLE_SIZE];
unsigned long ILResNumStrings = 0;

int ILResAddResource(const char *filename, long linenum,
					 const char *name, int nameLen,
					 const char *value, int valueLen)
{
	unsigned long hash;
	ILResHashEntry *entry;
	int error;

	/* Is the name already in the hash table? */
	hash = (ILHashString(0, name, nameLen) & (IL_RES_HASH_TABLE_SIZE - 1));
	entry = ILResHashTable[hash];
	while(entry != 0)
	{
		if(entry->nameLen == nameLen &&
		   !strncmp(entry->data, name, nameLen))
		{
			/* The value must be identical, or we report an error */
			if(entry->valueLen != valueLen ||
			   (valueLen != 0 &&
			    strncmp(entry->data + entry->nameLen, value, valueLen) != 0))
			{
				error = 1;
			}
			else
			{
				error = 0;
			}

			/* Print the message as either an error or a warning */
			if(linenum > 0)
			{
				fprintf(stderr, "%s:%ld", filename, linenum);
			}
			else
			{
				fputs(filename, stderr);
			}
			if(!error)
			{
				fputs(": warning", stderr);
			}
			fputs(": duplicate definition for `", stderr);
			fwrite(name, 1, nameLen, stderr);
			fputs("'\n", stderr);
			if(entry->linenum > 0)
			{
				fprintf(stderr, "%s:%ld", entry->filename, entry->linenum);
			}
			else
			{
				fputs(entry->filename, stderr);
			}
			fputs(": original definition here\n", stderr);

			/* Return the error state to the caller */
			return error;
		}
		entry = entry->next;
	}

	/* Create a new hash entry */
	entry = (ILResHashEntry *)ILMalloc
				(sizeof(ILResHashEntry) + nameLen + valueLen - 1);
	if(!entry)
	{
		ILResOutOfMemory();
	}
	entry->next = ILResHashTable[hash];
	ILResHashTable[hash] = entry;
	entry->nameLen = nameLen;
	entry->valueLen = valueLen;
	entry->filename = filename;
	entry->linenum = linenum;
	entry->offset = 0;
	entry->position = 0;
	if(nameLen)
	{
		ILMemCpy(entry->data, name, nameLen);
	}
	if(valueLen)
	{
		ILMemCpy(entry->data + nameLen, value, valueLen);
	}
	++ILResNumStrings;

	/* Done */
	return 0;
}

/*
 * Load the resources from a file that is in a specific format.
 */
static int loadResources(const char *filename, FILE *stream,
						 ILContext *context, int format, int latin1)
{
	switch(format)
	{
		case FORMAT_TEXT:
		{
			return ILResLoadText(filename, stream, latin1);
		}
		/* Not reached */

		case FORMAT_RES_BINARY:
		{
			return ILResLoadBinary(filename, stream);
		}
		/* Not reached */

		case FORMAT_IL:
		{
			ILImage *image;
			int loadError;
			void *address;
			ILUInt32 size;

			/* Attempt to load the IL image */
			loadError = ILImageLoad(stream, filename, context, &image,
									IL_LOADFLAG_FORCE_32BIT |
									IL_LOADFLAG_NO_METADATA);
			if(loadError != 0)
			{
				fprintf(stderr, "%s: %s\n", filename,
						ILImageLoadError(loadError));
				return 1;
			}

			/* Extract and parse the resource section */
			if(ILImageGetSection(image, IL_SECTION_RESOURCES, &address, &size))
			{
				loadError = ILResLoadBinaryIL(filename,
											  (unsigned char *)address,
											  size);
			}
			else
			{
				loadError = 0;
			}

			/* Clean up and exit */
			ILImageDestroy(image);
			return loadError;
		}
		/* Not reached */

		case FORMAT_XML:
		{
			return ILResLoadXML(filename, stream);
		}
		/* Not reached */

		case FORMAT_PO:
		{
			return ILResLoadPO(filename, stream, latin1);
		}
		/* Not reached */
	}

	fprintf(stderr, "%s: unknown input resource format\n", filename);
	return 1;
}

/*
 * Compare the names of two hash entries.
 */
static int NameCompare(const void *e1, const void *e2)
{
	ILResHashEntry *entry1 = *((ILResHashEntry **)e1);
	ILResHashEntry *entry2 = *((ILResHashEntry **)e2);
	int cmp;
	if(entry1->nameLen == entry2->nameLen)
	{
		return strncmp(entry1->data, entry2->data, entry1->nameLen);
	}
	else if(entry1->nameLen < entry2->nameLen)
	{
		cmp = strncmp(entry1->data, entry2->data, entry1->nameLen);
		if(cmp != 0)
		{
			return cmp;
		}
		else
		{
			return -1;
		}
	}
	else
	{
		cmp = strncmp(entry1->data, entry2->data, entry2->nameLen);
		if(cmp != 0)
		{
			return cmp;
		}
		else
		{
			return 1;
		}
	}
}

ILResHashEntry **ILResCreateSortedArray(void)
{
	int hash;
	ILResHashEntry *entry;
	ILResHashEntry **table;
	unsigned long posn;

	/* Bail out if there are no strings in the table */
	if(!ILResNumStrings)
	{
		return 0;
	}

	/* Allocate space for the string table */
	if((table = (ILResHashEntry **)ILMalloc
			(sizeof(ILResHashEntry *) * ILResNumStrings)) == 0)
	{
		ILResOutOfMemory();
	}

	/* Populate the table with the hash entries */
	posn = 0;
	for(hash = 0; hash < IL_RES_HASH_TABLE_SIZE; ++hash)
	{
		entry = ILResHashTable[hash];
		while(entry != 0)
		{
			table[posn++] = entry;
			entry = entry->next;
		}
	}

	/* Sort the table into ascending order by name */
#if HAVE_QSORT
	qsort(table, ILResNumStrings, sizeof(ILResHashEntry *), NameCompare);
#else
	{
		unsigned long posn2;
		for(posn = 0; posn < (ILResNumStrings - 1); ++posn)
		{
			for(posn2 = posn + 1; posn2 < ILResNumStrings; ++posn2)
			{
				if(NameCompare(&(table[posn]), &(table[posn2])) > 0)
				{
					entry = table[posn];
					table[posn] = table[posn2];
					table[posn2] = entry;
				}
			}
		}
	}
#endif

	/* Return the sorted table to the caller */
	return table;
}

/*
 * Extract a resource from an IL binary.
 */
static int extractResource(const char *filename, const char *resourceName)
{
	ILContext *context;
	ILImage *image;
	ILManifestRes *res;
	ILUInt32 posn;
	void *address;
	unsigned char *addr;
	ILUInt32 size;
	ILUInt32 reslen;
	int gotres;
	FILE *file;

	/* Create a context and load the image */
	context = ILContextCreate();
	if(!context)
	{
		return 1;
	}
	if(ILImageLoadFromFile(filename, context, &image,
						   IL_LOADFLAG_FORCE_32BIT |
						   IL_LOADFLAG_NO_RESOLVE, 1) != 0)
	{
		ILContextDestroy(context);
		return 1;
	}

	/* Search the manifest resource table for the resource name */
	res = 0;
	posn = 0;
	while((res = (ILManifestRes *)ILImageNextToken
				(image, IL_META_TOKEN_MANIFEST_RESOURCE, (void *)res)) != 0)
	{
		if(ILManifestRes_OwnerFile(res) || ILManifestRes_OwnerAssembly(res))
		{
			continue;
		}
		if(!strcmp(ILManifestRes_Name(res), resourceName))
		{
			break;
		}
		++posn;
	}
	if(!res)
	{
		fprintf(stderr, "%s: could not find the resource `%s'\n",
				filename, resourceName);
		ILContextDestroy(context);
		return 1;
	}

	/* Search for the entry at position "posn" in the resource section */
	gotres = 0;
	if(ILImageGetSection(image, IL_SECTION_RESOURCES, &address, &size))
	{
		addr = (unsigned char *)address;
		while(size >= 4)
		{
			reslen = IL_READ_UINT32(addr);
			if(reslen > (size - 4))
			{
				break;
			}
			if(posn == 0)
			{
				/* We've found the resource that we were looking for */
				addr += 4;
				if((file = fopen(resourceName, "wb")) == NULL)
				{
					if((file = fopen(resourceName, "w")) == NULL)
					{
						perror(resourceName);
						ILContextDestroy(context);
						return 1;
					}
				}
				fwrite(addr, 1, (size_t)reslen, file);
				fclose(file);
				gotres = 1;
				break;
			}
			else
			{
				addr += reslen + 4;
				size -= reslen + 4;
				--posn;
			}
		}
	}
	if(!gotres)
	{
		fprintf(stderr, "%s: could not find the resource `%s'\n",
				filename, resourceName);
		ILContextDestroy(context);
		return 1;
	}

	/* Clean up and exit */
	ILContextDestroy(context);
	return 0;
}

#ifdef	__cplusplus
};
#endif
