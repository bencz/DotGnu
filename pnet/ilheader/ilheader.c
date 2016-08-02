/*
 * ilheader.c - Dump .h header files for C# assemblies.
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
	{"-a", 'a', 0, 0, 0},
	{"--aliases", 'a', 0,
		"--aliases or -a",
		"Define shortened type aliases, without namespaces."},
	{"-v", 'v', 0, 0, 0},
	{"--version", 'v', 0,
		"--version  or -v",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

static void usage(const char *progname);
static void version(void);
static void GenerateHeader(FILE *stream, ILImage *image, int defineAliases);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	int defineAliases = 0;
	FILE *outstream;
	int closeOut;
	int state, opt;
	char *param;
	ILContext *context;
	ILImage *image;

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'a':
			{
				defineAliases = 1;
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

	/* We need two additional arguments */
	if(argc != 3)
	{
		usage(progname);
		return 1;
	}

	/* Create a context to use for image loading */
	context = ILContextCreate();
	if(!context)
	{
		fprintf(stderr, "%s: out of memory\n", progname);
		return 1;
	}

	/* Load the input file */
	if(ILImageLoadFromFile(argv[1], context, &image,
						   IL_LOADFLAG_NO_RESOLVE, 1) != 0)
	{
		return 1;
	}

	/* Open the output stream */
	if(!strcmp(argv[2], "-"))
	{
		outstream = stdout;
		closeOut = 0;
	}
	else
	{
		outstream = fopen(argv[2], "w");
		if(!outstream)
		{
			perror(argv[2]);
			return 1;
		}
		closeOut = 1;
	}

	/* Generate the header contents */
	GenerateHeader(outstream, image, defineAliases);

	/* Close the output stream */
	if(closeOut)
	{
		fclose(outstream);
	}

	/* Destroy the context */
	ILContextDestroy(context);
	
	/* Done */
	return 0;
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILHEADER " VERSION " - IL Image Header Utility\n");
	fprintf(stdout, "Copyright (c) 2002 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input output.h\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("ILHEADER " VERSION " - IL Image Header Utility\n");
	printf("Copyright (c) 2002 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILHEADER comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Print an identifier, converting illegal characters to underscores,
 * an optionally converting the identifier to upper case.
 */
static void PrintIdentifier(FILE *stream, const char *str, int upper)
{
	while(str && *str != '\0')
	{
		if(*str >= 'a' && *str <= 'z')
		{
			if(upper)
			{
				putc(*str - 'a' + 'A', stream);
			}
			else
			{
				putc(*str, stream);
			}
		}
		else if((*str >= 'A' && *str <= 'Z') ||
			    (*str >= '0' && *str <= '9'))
		{
			putc(*str, stream);
		}
		else
		{
			putc('_', stream);
		}
		++str;
	}
}

#if 0

/*
 * Print a class name as an identifier.
 */
static void PrintClassName(FILE *stream, ILClass *classInfo, int withNS)
{
	const char *name = ILClass_Name(classInfo);
	const char *namespace = ILClass_Namespace(classInfo);
	if(namespace && withNS)
	{
		PrintIdentifier(stream, namespace, 0);
		putc('_', stream);
	}
	PrintIdentifier(stream, name, 0);
}

/*
 * Dump the contents of an enumerated type.
 */
static void DumpEnum(FILE *stream, ILClass *classInfo, int defineAliases)
{
	ILField *field;
	ILConstant *constant;
	const void *value;
	unsigned long len;

	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
				(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		/* Skip fields that aren't enumerated constant definitions */
		if(!ILField_IsStatic(field) || !ILField_IsLiteral(field))
		{
			continue;
		}
		constant = ILConstantGetFromOwner(ILToProgramItem(field));
		if(!constant)
		{
			continue;
		}
		value = ILConstantGetValue(constant, &len);
		if(!value)
		{
			continue;
		}

		/* Define the primary constant name */
		fputs("#define ", stream);
		PrintClassName(stream, classInfo, 1);
		putc('_', stream);
		PrintIdentifier(stream, ILField_Name(field), 0);
		fputs(" ((", stream);
		PrintClassName(stream, classInfo, 1);
		putc(')', stream);

		/* Output the constant value */
		switch(ILConstant_ElemType(constant))
		{
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			{
				fprintf(stream, "0x%02X", (int)(((unsigned char *)value)[0]));
			}
			break;

			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			{
				fprintf(stream, "0x%04X", (int)(IL_READ_UINT16(value)));
			}
			break;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
			{
				fprintf(stream, "0x%08lX",
						(unsigned long)(IL_READ_UINT32(value)));
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
			{
				fprintf(stream, "0x%lX%08lX",
						(unsigned long)(IL_READ_UINT32
							(((unsigned char *)value) + 4)),
						(unsigned long)(IL_READ_UINT32(value)));
			}
			break;

			default:
			{
				fputs("???", stream);
			}
			break;
		}

		/* Terminate the primary constant definition */
		fputs(")\n", stream);

		/* Define the alias name if necessary */
		if(defineAliases && ILClass_Namespace(classInfo) != 0)
		{
			fputs("#define ", stream);
			PrintClassName(stream, classInfo, 0);
			putc('_', stream);
			PrintIdentifier(stream, ILField_Name(field), 0);
			putc(' ', stream);
			PrintClassName(stream, classInfo, 1);
			putc('_', stream);
			PrintIdentifier(stream, ILField_Name(field), 0);
			putc('\n', stream);
		}
	}
	putc('\n', stream);
}

/*
 * Dump the public methods within a class.
 */
static void DumpMethods(FILE *stream, ILClass *classInfo, int defineAliases)
{
	ILMethod *method = 0;
	ILMethod *method2;
	int printed = 0;

	while((method = (ILMethod *)ILClassNextMemberByKind
			(classInfo, (ILMember *)method, IL_META_MEMBERKIND_METHOD)) != 0)
	{
		/* Filter out methods that aren't relevant */
		if(!ILMethod_IsPublic(method))
		{
			continue;
		}
		if(ILMethodIsStaticConstructor(method))
		{
			continue;
		}

		/* Skip this one if another with the same name occurs further along */
		method2 = method;
		while((method2 = (ILMethod *)ILClassNextMemberByKind
			(classInfo, (ILMember *)method2, IL_META_MEMBERKIND_METHOD)) != 0)
		{
			if(!strcmp(ILMethod_Name(method), ILMethod_Name(method2)))
			{
				if(ILMethod_IsPublic(method))
				{
					break;
				}
			}
		}
		if(method2 != 0)
		{
			continue;
		}

		/* Define the method or constructor */
		printed = 1;
		fputs("#define ", stream);
		PrintClassName(stream, classInfo, 1);
		putc('_', stream);
		if(!ILMethodIsConstructor(method))
		{
			PrintIdentifier(stream, ILMethod_Name(method), 0);
			fputs(" __invoke__ ", stream);
			PrintClassName(stream, classInfo, 1);
			putc('.', stream);
			PrintIdentifier(stream, ILMethod_Name(method), 0);
			putc('\n', stream);
		}
		else
		{
			fputs("new __new__ ", stream);
			PrintClassName(stream, classInfo, 1);
			putc('\n', stream);
		}

		/* Define an alias if necessary */
		if(defineAliases && ILClass_Namespace(classInfo) != 0)
		{
			fputs("#define ", stream);
			PrintClassName(stream, classInfo, 0);
			putc('_', stream);
			if(!ILMethodIsConstructor(method))
			{
				PrintIdentifier(stream, ILMethod_Name(method), 0);
				fputs(" __invoke__ ", stream);
				PrintClassName(stream, classInfo, 1);
				putc('.', stream);
				PrintIdentifier(stream, ILMethod_Name(method), 0);
				putc('\n', stream);
			}
			else
			{
				fputs("new __new__ ", stream);
				PrintClassName(stream, classInfo, 1);
				putc('\n', stream);
			}
		}
	}
	if(printed)
	{
		putc('\n', stream);
	}
}

#endif

/*
 * Generate a .h file that defines the contents of an assembly.
 */
static void GenerateHeader(FILE *stream, ILImage *image, int defineAliases)
{
	const char *assemName;
	ILClass *classInfo;
	const char *name;
	const char *namespace;

	/* Output the start of the header file */
	fputs("/* This file is automatically generated - do not edit */\n\n",
		  stream);
	assemName = ILImageGetAssemblyName(image);
	if(assemName)
	{
		fputs("#ifndef _CSHARP_", stream);
		PrintIdentifier(stream, assemName, 1);
		fputs("_H\n#define _CSHARP_", stream);
		PrintIdentifier(stream, assemName, 1);
		fputs("_H\n\n", stream);
	}

	/* Declare all public types */
	classInfo = 0;
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
	{
		/* Filter out classes that aren't relevant */
		if(ILClass_NestedParent(classInfo))
		{
			continue;
		}
		if(!ILClass_IsPublic(classInfo))
		{
			continue;
		}
		name = ILClass_Name(classInfo);
		namespace = ILClass_Namespace(classInfo);
		if((!strcmp(name, "<Module>") || !strcmp(name, "$Module$")) &&
		   !namespace)
		{
			continue;
		}

		/* Define the primary class type */
		fputs("typedef __csharp__(", stream);
		if(namespace)
		{
			fputs(namespace, stream);
			putc('.', stream);
		}
		fputs(name, stream);
		fputs(") ", stream);
		if(namespace)
		{
			PrintIdentifier(stream, namespace, 0);
			putc('_', stream);
			PrintIdentifier(stream, name, 0);
			fputs(";\n", stream);
			if(defineAliases)
			{
				/* Define a shortened alias, without namespace qualification */
				fputs("typedef ", stream);
				PrintIdentifier(stream, namespace, 0);
				putc('_', stream);
				PrintIdentifier(stream, name, 0);
				putc(' ', stream);
				PrintIdentifier(stream, name, 0);
				fputs(";\n", stream);
			}
		}
		else
		{
			PrintIdentifier(stream, name, 0);
			fputs(";\n", stream);
		}
	}
	putc('\n', stream);

#if 0
	/* Declare the contents of public types */
	classInfo = 0;
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
	{
		/* Filter out classes that aren't relevant */
		if(ILClass_NestedParent(classInfo))
		{
			continue;
		}
		if(!ILClass_IsPublic(classInfo))
		{
			continue;
		}
		name = ILClass_Name(classInfo);
		namespace = ILClass_Namespace(classInfo);
		if((!strcmp(name, "<Module>") || !strcmp(name, "$Module$")) &&
		   !namespace)
		{
			continue;
		}
		if(ILTypeIsDelegate(ILType_FromClass(classInfo)))
		{
			continue;
		}

		/* Is this an enumerated type definition? */
		if(ILTypeIsEnum(ILType_FromValueType(classInfo)))
		{
			/* Dump the enumerated constant definitions */
			DumpEnum(stream, classInfo, defineAliases);
			continue;
		}

		/* Dump the methods within the class */
		DumpMethods(stream, classInfo, defineAliases);
	}
#endif

	/* Output the end of the header file */
	if(assemName)
	{
		fputs("#endif /\052 !_CSHARP_", stream);
		PrintIdentifier(stream, assemName, 1);
		fputs("_H */\n", stream);
	}
}

#ifdef	__cplusplus
};
#endif
