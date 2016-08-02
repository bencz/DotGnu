/*
 * ilinternal.c - Generate prototypes and tables for internalcall methods.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

This program is used to generate prototypes and tables for the internalcall
methods that are implemented by the runtime engine.  It is used as follows:

	ilinternal -p mscorlib.dll System.dll >int_proto.h
	ilinternal -t mscorlib.dll System.dll >int_table.c

*/

#include <stdio.h>
#include <stdlib.h>
#include "il_system.h"
#include "il_image.h"
#include "il_dumpasm.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-o", 'o', 1, 0, 0},
	{"--output", 'o', 1,
		"--output file  or -o file",
		"Specify the output file (default is stdout)."},
	{"-p", 'p', 0, 0, 0},
	{"--prototypes", 'p', 0,
		"--prototypes   or -p",
		"Output implementation prototypes."},
	{"-t", 't', 0, 0, 0},
	{"--tables", 't', 0,
		"--tables       or -t",
		"Output method tables."},
	{"-P", 'P', 1, 0, 0},
	{"--prefix", 'P', 1,
		"--prefix name  or -P name",
		"Specify the prefix for all external identifiers."},
	{"-v", 'v', 0, 0, 0},
	{"--version", 'v', 0,
		"--version      or -v",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

/*
 * Output modes.
 */
#define	OUT_MODE_PROTOTYPES		1
#define	OUT_MODE_TABLES			2

static void usage(const char *progname);
static void version(void);
static void printHeader(FILE *outfile);
static int processFile(const char *filename, ILContext *context,
					   const char *prefix, int modes, FILE *outfile);
static void dumpClassTable(FILE *outfile);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	const char *output = 0;
	const char *prefix = "_IL_";
	int sawStdin;
	int state, opt;
	char *param;
	int errors;
	ILContext *context;
	FILE *outfile;
	int modes = 0;

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'o':
			{
				if(strcmp(param, "-") != 0)
				{
					output = param;
				}
				else
				{
					output = 0;
				}
			}
			break;

			case 'p':
			{
				modes |= OUT_MODE_PROTOTYPES;
			}
			break;

			case 't':
			{
				modes |= OUT_MODE_TABLES;
			}
			break;

			case 'P':
			{
				prefix = param;
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

	/* We need at least one input file argument */
	if(argc <= 1)
	{
		usage(progname);
		return 1;
	}

	/* We need at least one mode specification */
	if(!modes)
	{
		usage(progname);
		return 1;
	}

	/* Open the output file and write the header to it */
	if(output)
	{
		if((outfile = fopen(output, "w")) == NULL)
		{
			perror(output);
			return 1;
		}
	}
	else
	{
		outfile = stdout;
	}
	printHeader(outfile);

	/* Create a context to use for image loading */
	context = ILContextCreate();
	if(!context)
	{
		fprintf(stderr, "%s: out of memory\n", progname);
		return 1;
	}

	/* Load and print information about the input files */
	sawStdin = 0;
	errors = 0;
	while(argc > 1)
	{
		if(!strcmp(argv[1], "-"))
		{
			/* Dump the contents of stdin, but only once */
			if(!sawStdin)
			{
				errors |= processFile("-", context, prefix, modes, outfile);
				sawStdin = 1;
			}
		}
		else
		{
			/* Dump the contents of a regular file */
			errors |= processFile(argv[1], context, prefix, modes, outfile);
		}
		++argv;
		--argc;
	}

	/* Destroy the context */
	ILContextDestroy(context);

	/* Output the class table */
	if((modes & OUT_MODE_TABLES) != 0)
	{
		dumpClassTable(outfile);
	}

	/* Close the output file */
	if(output)
	{
		fclose(outfile);
	}
	
	/* Done */
	return errors;
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILINTERNAL " VERSION " - IL InternalCall Utility\n");
	fprintf(stdout, "Copyright (c) 2001, 2002 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] [-p|-t] input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("ILINTERNAL " VERSION " - IL InternalCall Utility\n");
	printf("Copyright (c) 2001, 2002 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILINTERNAL comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Print the header information to the output file.
 */
static void printHeader(FILE *outfile)
{
	fprintf(outfile,
	        "/* This file is automatically generated - do not edit */\n\n");
}

/*
 * Method implementation bit to use to mark a method
 * that will require name mangling.
 */
#define	MANGLE_MARK_BIT		0x2000

/*
 * Get information about a method parameter.
 */
static const char *getParamInfo(ILMethod *method, ILType *signature,
								unsigned long param, ILType **type)
{
	ILParameter *paramInfo;
	*type = ILTypeGetParam(signature, param);
	paramInfo = 0;
	while((paramInfo = ILMethodNextParam(method, paramInfo)) != 0)
	{
		if(ILParameter_Num(paramInfo) == param)
		{
			return ILParameter_Name(paramInfo);
		}
	}
	return 0;
}

/*
 * Dump the C version of an IL type.
 */
static void dumpCType(FILE *outfile, ILType *type, ILMethod *method)
{
	ILClass *classInfo;
	if(ILType_IsPrimitive(type))
	{
		/* Dump a primitive type */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:		fputs("void", outfile); break;
			case IL_META_ELEMTYPE_BOOLEAN:	fputs("ILBool", outfile); break;
			case IL_META_ELEMTYPE_I1:		fputs("ILInt8", outfile); break;
			case IL_META_ELEMTYPE_U1:		fputs("ILUInt8", outfile); break;
			case IL_META_ELEMTYPE_I2:		fputs("ILInt16", outfile); break;
			case IL_META_ELEMTYPE_U2:		fputs("ILUInt16", outfile); break;
			case IL_META_ELEMTYPE_CHAR:		fputs("ILUInt16", outfile); break;
			case IL_META_ELEMTYPE_I4:		fputs("ILInt32", outfile); break;
			case IL_META_ELEMTYPE_U4:		fputs("ILUInt32", outfile); break;
			case IL_META_ELEMTYPE_I8:		fputs("ILInt64", outfile); break;
			case IL_META_ELEMTYPE_U8:		fputs("ILUInt64", outfile); break;
			case IL_META_ELEMTYPE_I:
					fputs("ILNativeInt", outfile); break;
			case IL_META_ELEMTYPE_U:
					fputs("ILNativeUInt", outfile); break;
			case IL_META_ELEMTYPE_R4:		fputs("ILFloat", outfile); break;
			case IL_META_ELEMTYPE_R8:		fputs("ILDouble", outfile); break;
			case IL_META_ELEMTYPE_R:
					fputs("ILNativeFloat", outfile); break;
			case IL_META_ELEMTYPE_TYPEDBYREF:
					fputs("ILTypedRef", outfile); break;
			default: break;
		}
	}
	else if(ILType_IsValueType(type))
	{
		/* Dump a value type */
		classInfo = ILType_ToValueType(type);
		if(ILTypeIsEnum(type))
		{
			dumpCType(outfile, ILTypeGetEnumType(type), method);
		}
		else if(!strcmp(ILClass_Name(classInfo), "Decimal") &&
				ILClass_Namespace(classInfo) != 0 &&
				!strcmp(ILClass_Namespace(classInfo), "System"))
		{
			fputs("ILDecimal *", outfile);
		}
		else
		{
			fputs("void *", outfile);
		}
	}
	else if(ILType_IsClass(type))
	{
		/* Dump a class type */
		if(ILTypeIsStringClass(type))
		{
			/* Strings are either "System_String" or "ILString" */
			if(ILTypeIsStringClass(ILType_FromClass(ILMethod_Owner(method))))
			{
				fputs("System_String *", outfile);
			}
			else
			{
				fputs("ILString *", outfile);
			}
		}
		else
		{
			/* Everything else is "ILObject" */
			fputs("ILObject *", outfile);
		}
	}
	else if(type != 0)
	{
		/* Dump a complex type */
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_BYREF:
			case IL_TYPE_COMPLEX_PTR:
			{
				dumpCType(outfile, ILType_Ref(type), method);
				fputs(" *", outfile);
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY:
			{
				fputs("System_Array *", outfile);
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				fputs("System_MArray *", outfile);
			}
			break;

			case IL_TYPE_COMPLEX_CMOD_REQD:
			case IL_TYPE_COMPLEX_CMOD_OPT:
			{
				dumpCType(outfile, type->un.modifier__.type__, method);
			}
			break;

			case IL_TYPE_COMPLEX_PINNED:
			{
				dumpCType(outfile, ILType_Ref(type), method);
			}
			break;

			case IL_TYPE_COMPLEX_SENTINEL:
			{
				fputs("...", outfile);
			}
			break;

			default:
			{
				fputs("void *", outfile);
			}
			break;
		}
	}
}

/*
 * Dump the name of a class in "mangled" form, suitable for C identifiers.
 */
static void dumpMangledClassName(FILE *outfile, const char *name,
								 const char *namespace)
{
/* -- only dump the class name
	if(namespace)
	{
		while(*namespace != '\0')
		{
			if(*namespace == '.')
			{
				putc('_', outfile);
			}
			else
			{
				putc(*namespace, outfile);
			}
			++namespace;
		}
		putc('_', outfile);
	}
*/
	fputs(name, outfile);
}

/*
 * Dump the name of a class in "mangled" form, suitable for C identifiers.
 */
static void dumpMangledClass(FILE *outfile, ILClass *classInfo)
{
	dumpMangledClassName(outfile, ILClass_Name(classInfo),
						 ILClass_Namespace(classInfo));
}

/*
 * Dump a type in "mangled" form.
 */
static void dumpMangledType(FILE *outfile, ILType *type)
{
	unsigned long numParams;
	unsigned long param;

	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:		putc('V', outfile); break;
			case IL_META_ELEMTYPE_BOOLEAN:	putc('Z', outfile); break;
			case IL_META_ELEMTYPE_I1:		putc('b', outfile); break;
			case IL_META_ELEMTYPE_U1:		putc('B', outfile); break;
			case IL_META_ELEMTYPE_I2:		putc('s', outfile); break;
			case IL_META_ELEMTYPE_U2:		putc('S', outfile); break;
			case IL_META_ELEMTYPE_CHAR:		putc('c', outfile); break;
			case IL_META_ELEMTYPE_I4:		putc('i', outfile); break;
			case IL_META_ELEMTYPE_U4:		putc('I', outfile); break;
			case IL_META_ELEMTYPE_I8:		putc('l', outfile); break;
			case IL_META_ELEMTYPE_U8:		putc('L', outfile); break;
			case IL_META_ELEMTYPE_I:		putc('j', outfile); break;
			case IL_META_ELEMTYPE_U:		putc('J', outfile); break;
			case IL_META_ELEMTYPE_R4:		putc('f', outfile); break;
			case IL_META_ELEMTYPE_R8:		putc('d', outfile); break;
			case IL_META_ELEMTYPE_R:		putc('D', outfile); break;
			case IL_META_ELEMTYPE_TYPEDBYREF: putc('r', outfile); break;
			default:						break;
		}
	}
	else if(ILType_IsClass(type) || ILType_IsValueType(type))
	{
		fputs(ILClass_Name(ILType_ToClass(type)), outfile);
	}
	else if(type != 0)
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_BYREF:
			{
				putc('R', outfile);
				dumpMangledType(outfile, ILType_Ref(type));
			}
			break;

			case IL_TYPE_COMPLEX_PTR:
			{
				putc('p', outfile);
				dumpMangledType(outfile, ILType_Ref(type));
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY:
			{
				putc('a', outfile);
				dumpMangledType(outfile, ILTypeGetElemType(type));
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				putc('A', outfile);
				dumpMangledType(outfile, ILTypeGetElemType(type));
			}
			break;

			case IL_TYPE_COMPLEX_CMOD_REQD:
			case IL_TYPE_COMPLEX_CMOD_OPT:
			{
				dumpMangledType(outfile, type->un.modifier__.type__);
			}
			break;

			case IL_TYPE_COMPLEX_PINNED:
			{
				dumpMangledType(outfile, ILType_Ref(type));
			}
			break;

			case IL_TYPE_COMPLEX_METHOD:
			case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
			{
				numParams = ILTypeNumParams(type);
				for(param = 1; param <= numParams; ++param)
				{
					dumpMangledType(outfile, ILTypeGetParam(type, param));
				}
			}
			break;
		}
	}
}

/*
 * Dump the name of a function that implements an "internalcall" method.
 */
static void dumpMethodFuncName(FILE *outfile, ILMethod *method, int mangle)
{
	dumpMangledClass(outfile, ILMethod_Owner(method));
	putc('_', outfile);
	if(!strcmp(ILMethod_Name(method), ".ctor"))
	{
		fputs("ctor", outfile);
	}
	else
	{
		fputs(ILMethod_Name(method), outfile);
	}
	if(mangle)
	{
		putc('_', outfile);
		dumpMangledType(outfile, ILMethod_Signature(method));
	}
}

/*
 * Dump a prototype for a method implementation function.
 */
static void dumpMethodPrototype(FILE *outfile, const char *prefix,
								ILMethod *method, int nameMangle)
{
	ILType *signature = ILMethod_Signature(method);
	unsigned long numParams;
	unsigned long param;
	ILType *paramType;
	const char *paramName;
	int suppressThis = 0;

	/* Dump the return type */
	fputs("extern ", outfile);
	paramType = ILTypeGetReturn(signature);
	if(ILMethodIsConstructor(method))
	{
		if(!ILClassIsValueType(ILMethod_Owner(method)))
		{
			dumpCType(outfile, ILType_FromClass(ILMethod_Owner(method)),
					  method);
			suppressThis = 1;
		}
		else
		{
			fputs("void", outfile);
		}
	}
	else if(ILType_IsValueType(paramType) && !ILTypeIsEnum(paramType))
	{
		fputs("void", outfile);
	}
	else
	{
		dumpCType(outfile, paramType, method);
	}
	putc(' ', outfile);

	/* Dump the method function name, including name mangling characters */
	fputs(prefix, outfile);
	dumpMethodFuncName(outfile, method, nameMangle);
	fputs("(ILExecThread * _thread", outfile);

	/* Add the result parameter if the return type is a value type */
	if(ILType_IsValueType(paramType) && !ILTypeIsEnum(paramType))
	{
		fputs(", ", outfile);
		dumpCType(outfile, paramType, method);
		fputs(" _result", outfile);
	}

	/* Add the "this" parameter if necessary */
	if(ILType_HasThis(signature) && !suppressThis)
	{
		fputs(", ", outfile);
		if(ILClassIsValueType(ILMethod_Owner(method)))
		{
			dumpCType(outfile, ILType_FromValueType(ILMethod_Owner(method)),
					  method);
		}
		else
		{
			dumpCType(outfile, ILType_FromClass(ILMethod_Owner(method)),
					  method);
		}
		fputs(" _this", outfile);
	}

	/* Add the parameters */
	numParams = ILTypeNumParams(signature);
	for(param = 1; param <= numParams; ++param)
	{
		paramName = getParamInfo(method, signature, param, &paramType);
		fputs(", ", outfile);
		dumpCType(outfile, paramType, method);
		if(paramName)
		{
			putc(' ', outfile);
			if(!strcmp(paramName, "errno"))
			{
				fputs("error", outfile);
			}
			else
			{
				fputs(paramName, outfile);
			}
		}
		else
		{
			fprintf(outfile, " _p%lu", param);
		}
	}

	/* Terminate the prototype */
	fputs(");\n", outfile);
}

/*
 * Dump a class in "lookup" form (recursively handle nesting)
 */
static void dumpLookupClass(FILE *outfile, ILClass *classInfo)
{
	const char *namespace;
	ILClass *nestedParent=ILClassGetNestedParent(classInfo);
	if(nestedParent)
	{
		dumpLookupClass(outfile,nestedParent);
		putc('/', outfile);
	}
	namespace = ILClass_Namespace(classInfo);
	if(namespace)
	{
		fputs(namespace, outfile);
		putc('.', outfile);
	}
	fputs(ILClass_Name(classInfo), outfile);
}

/*
 * Dump a type in "lookup" form.
 */
static void dumpLookupType(FILE *outfile, ILType *type, int topLevel)
{
	unsigned long numParams;
	unsigned long param;

	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:		putc('V', outfile); break;
			case IL_META_ELEMTYPE_BOOLEAN:	putc('Z', outfile); break;
			case IL_META_ELEMTYPE_I1:		putc('b', outfile); break;
			case IL_META_ELEMTYPE_U1:		putc('B', outfile); break;
			case IL_META_ELEMTYPE_I2:		putc('s', outfile); break;
			case IL_META_ELEMTYPE_U2:		putc('S', outfile); break;
			case IL_META_ELEMTYPE_CHAR:		putc('c', outfile); break;
			case IL_META_ELEMTYPE_I4:		putc('i', outfile); break;
			case IL_META_ELEMTYPE_U4:		putc('I', outfile); break;
			case IL_META_ELEMTYPE_I8:		putc('l', outfile); break;
			case IL_META_ELEMTYPE_U8:		putc('L', outfile); break;
			case IL_META_ELEMTYPE_I:		putc('j', outfile); break;
			case IL_META_ELEMTYPE_U:		putc('J', outfile); break;
			case IL_META_ELEMTYPE_R4:		putc('f', outfile); break;
			case IL_META_ELEMTYPE_R8:		putc('d', outfile); break;
			case IL_META_ELEMTYPE_R:		putc('D', outfile); break;
			case IL_META_ELEMTYPE_TYPEDBYREF: putc('r', outfile); break;
			default:						break;
		}
	}
	else if(ILType_IsClass(type))
	{
		putc('o', outfile);
		dumpLookupClass(outfile,ILType_ToClass(type));
		putc(';', outfile);
	}
	else if(ILType_IsValueType(type))
	{
		putc('v', outfile);
		dumpLookupClass(outfile,ILType_ToClass(type));
		putc(';', outfile);
	}
	else if(type != 0)
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_BYREF:
			{
				putc('&', outfile);
				dumpLookupType(outfile, ILType_Ref(type), 0);
			}
			break;

			case IL_TYPE_COMPLEX_PTR:
			{
				putc('*', outfile);
				dumpLookupType(outfile, ILType_Ref(type), 0);
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY:
			{
				putc('[', outfile);
				dumpLookupType(outfile, ILTypeGetElemType(type), 0);
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				fprintf(outfile, "{%d,", ILTypeGetRank(type));
				dumpLookupType(outfile, ILTypeGetElemType(type), 0);
			}
			break;

			case IL_TYPE_COMPLEX_CMOD_REQD:
			case IL_TYPE_COMPLEX_CMOD_OPT:
			{
				dumpLookupType(outfile, type->un.modifier__.type__, 0);
			}
			break;

			case IL_TYPE_COMPLEX_PINNED:
			{
				dumpLookupType(outfile, ILType_Ref(type), 0);
			}
			break;

			case IL_TYPE_COMPLEX_METHOD:
			case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
			{
				if(!topLevel)
				{
					putc('%', outfile);
				}
				putc('(', outfile);
				if(ILType_HasThis(type))
				{
					putc('T', outfile);
				}
				numParams = ILTypeNumParams(type);
				for(param = 1; param <= numParams; ++param)
				{
					dumpLookupType(outfile, ILTypeGetParam(type, param), 0);
				}
				putc(')', outfile);
				dumpLookupType(outfile, ILTypeGetReturn(type), 0);
			}
			break;
		}
	}
}

/*
 * FFI marshalling types.
 */
#define	IL_FFI_VOID				0
#define	IL_FFI_UINT8			1
#define	IL_FFI_SINT8			2
#define	IL_FFI_UINT16			3
#define	IL_FFI_SINT16			4
#define	IL_FFI_UINT32			5
#define	IL_FFI_SINT32			6
#define	IL_FFI_UINT64			7
#define	IL_FFI_SINT64			8
#define	IL_FFI_UINTPTR			9
#define	IL_FFI_SINTPTR			10
#define	IL_FFI_FLOAT32			11
#define	IL_FFI_FLOAT64			12
#define	IL_FFI_FLOAT			13
#define	IL_FFI_PTR				14
#define	IL_FFI_TYPEDREF			15

/*
 * Get the FFI marshalling type for an IL type.
 */
static int getFfiType(ILType *type)
{
	/* Handle primitive and enumerated types */
	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:		  return IL_FFI_VOID;
			case IL_META_ELEMTYPE_BOOLEAN:	  return IL_FFI_SINT8;
			case IL_META_ELEMTYPE_I1:		  return IL_FFI_SINT8;
			case IL_META_ELEMTYPE_U1:		  return IL_FFI_UINT8;
			case IL_META_ELEMTYPE_I2:		  return IL_FFI_SINT16;
			case IL_META_ELEMTYPE_U2:		  return IL_FFI_UINT16;
			case IL_META_ELEMTYPE_CHAR:		  return IL_FFI_UINT16;
			case IL_META_ELEMTYPE_I4:		  return IL_FFI_SINT32;
			case IL_META_ELEMTYPE_U4:		  return IL_FFI_UINT32;
			case IL_META_ELEMTYPE_I8:		  return IL_FFI_SINT64;
			case IL_META_ELEMTYPE_U8:		  return IL_FFI_UINT64;
			case IL_META_ELEMTYPE_I:		  return IL_FFI_SINTPTR;
			case IL_META_ELEMTYPE_U:		  return IL_FFI_UINTPTR;
			case IL_META_ELEMTYPE_R4:		  return IL_FFI_FLOAT32;
			case IL_META_ELEMTYPE_R8:		  return IL_FFI_FLOAT64;
			case IL_META_ELEMTYPE_R:		  return IL_FFI_FLOAT;
			case IL_META_ELEMTYPE_TYPEDBYREF: return IL_FFI_TYPEDREF;
			default:						  break;
		}
	}
	else if(ILType_IsValueType(type))
	{
		if(ILTypeIsEnum(type))
		{
			return getFfiType(ILTypeGetEnumType(type));
		}
	}

	/* Everything else is a pointer */
	return IL_FFI_PTR;
}

/*
 * Maximum size of FFI profile for an internalcall method.
 */
#define	IL_FFI_SIZE		16

/*
 * Get the complete FFI marshalling profile for an internalcall method.
 */
static int getFfiProfile(ILMethod *method, char *profile)
{
	ILType *signature = ILMethod_Signature(method);
	ILType *returnType;
	int size = 0;
	unsigned long numParams;
	unsigned long param;
	int suppressThis = 0;

	/* Get the marshalling information for the return type */
	returnType = ILTypeGetReturn(signature);
	if(ILMethodIsConstructor(method))
	{
		if(!ILClassIsValueType(ILMethod_Owner(method)))
		{
			profile[size++] = IL_FFI_PTR;
			suppressThis = 1;
		}
		else
		{
			profile[size++] = IL_FFI_VOID;
		}
	}
	else if(ILType_IsValueType(returnType) && !ILTypeIsEnum(returnType))
	{
		profile[size++] = IL_FFI_VOID;
	}
	else
	{
		profile[size++] = getFfiType(returnType);
	}

	/* All internalcalls are passed the "thread" as the first argument */
	profile[size++] = IL_FFI_PTR;

	/* Add the result pointer if the return type is a value type */
	if(ILType_IsValueType(returnType) && !ILTypeIsEnum(returnType))
	{
		profile[size++] = IL_FFI_PTR;
	}

	/* Add the pointer argument for the "this" pointer */
	if(ILType_HasThis(signature) && !suppressThis)
	{
		profile[size++] = IL_FFI_PTR;
	}

	/* Convert the parameters into FFI marshalling values */
	numParams = ILTypeNumParams(signature);
	for(param = 1; param <= numParams; ++param)
	{
		profile[size++] = getFfiType(ILTypeGetParam(signature, param));
	}

	/* Return the size of the profile to the caller */
	return size;
}

/*
 * Dump the C name of a FFI marshalling type.
 */
static void dumpFfiType(FILE *outfile, int ffiType)
{
	switch(ffiType)
	{
		case IL_FFI_VOID:		fputs("void", outfile); break;
		case IL_FFI_UINT8:		fputs("ILUInt8", outfile); break;
		case IL_FFI_SINT8:		fputs("ILInt8", outfile); break;
		case IL_FFI_UINT16:		fputs("ILUInt16", outfile); break;
		case IL_FFI_SINT16:		fputs("ILInt16", outfile); break;
		case IL_FFI_UINT32:		fputs("ILUInt32", outfile); break;
		case IL_FFI_SINT32:		fputs("ILInt32", outfile); break;
		case IL_FFI_UINT64:		fputs("ILUInt64", outfile); break;
		case IL_FFI_SINT64:		fputs("ILInt64", outfile); break;
		case IL_FFI_UINTPTR:	fputs("ILNativeInt", outfile); break;
		case IL_FFI_SINTPTR:	fputs("ILNativeUInt", outfile); break;
		case IL_FFI_FLOAT32:	fputs("ILFloat", outfile); break;
		case IL_FFI_FLOAT64:	fputs("ILDouble", outfile); break;
		case IL_FFI_FLOAT:		fputs("ILNativeFloat", outfile); break;
		case IL_FFI_PTR:		fputs("void *", outfile); break;
		case IL_FFI_TYPEDREF:	fputs("ILTypedRef", outfile); break;
		default:				break;
	}
}

/*
 * Dump the C name of a FFI marshalling type for return values.
 */
static void dumpFfiReturnType(FILE *outfile, int ffiType)
{
	switch(ffiType)
	{
		case IL_FFI_VOID:		fputs("void", outfile); break;
		case IL_FFI_UINT8:
		case IL_FFI_UINT16:
		case IL_FFI_UINT32:		fputs("ILNativeUInt", outfile); break;
		case IL_FFI_SINT16:
		case IL_FFI_SINT8:
		case IL_FFI_SINT32:		fputs("ILNativeInt", outfile); break;
		case IL_FFI_UINT64:		fputs("ILUInt64", outfile); break;
		case IL_FFI_SINT64:		fputs("ILInt64", outfile); break;
		case IL_FFI_UINTPTR:	fputs("ILNativeInt", outfile); break;
		case IL_FFI_SINTPTR:	fputs("ILNativeUInt", outfile); break;
		case IL_FFI_FLOAT32:	fputs("ILFloat", outfile); break;
		case IL_FFI_FLOAT64:	fputs("ILDouble", outfile); break;
		case IL_FFI_FLOAT:		fputs("ILNativeFloat", outfile); break;
		case IL_FFI_PTR:		fputs("void *", outfile); break;
		case IL_FFI_TYPEDREF:	fputs("ILTypedRef", outfile); break;
		default:				break;
	}
}

/*
 * Dump the name of a marshalling function.
 */
static void dumpFfiMarshalerName(FILE *outfile, char *profile, int size)
{
	int index;

	fputs("marshal_", outfile);
	for(index = 0; index < size; ++index)
	{
		switch(profile[index])
		{
			case IL_FFI_VOID:		putc('v', outfile); break;
			case IL_FFI_UINT8:		putc('B', outfile); break;
			case IL_FFI_SINT8:		putc('b', outfile); break;
			case IL_FFI_UINT16:		putc('S', outfile); break;
			case IL_FFI_SINT16:		putc('s', outfile); break;
			case IL_FFI_UINT32:		putc('I', outfile); break;
			case IL_FFI_SINT32:		putc('i', outfile); break;
			case IL_FFI_UINT64:		putc('L', outfile); break;
			case IL_FFI_SINT64:		putc('l', outfile); break;
			case IL_FFI_UINTPTR:	putc('J', outfile); break;
			case IL_FFI_SINTPTR:	putc('j', outfile); break;
			case IL_FFI_FLOAT32:	putc('f', outfile); break;
			case IL_FFI_FLOAT64:	putc('d', outfile); break;
			case IL_FFI_FLOAT:		putc('D', outfile); break;
			case IL_FFI_PTR:		putc('p', outfile); break;
			case IL_FFI_TYPEDREF:	putc('r', outfile); break;
		}
	}
}

/*
 * List of marshalling profiles that have already been seen.
 */
static char **profiles = 0;
static int    numProfiles = 0;
static int    maxProfiles = 0;

/*
 * Dump a marshalling function for a particular method profile.
 */
static void dumpFfiMarshaler(FILE *outfile, char *profile, int size)
{
	int index;
	char **newProfiles;

	/* Determine if we've seen this marshalling profile before */
	for(index = 0; index < numProfiles; ++index)
	{
		if(profiles[index][0] == size &&
		   !ILMemCmp(profiles[index] + 1, profile, size))
		{
			return;
		}
	}
	if(numProfiles >= maxProfiles)
	{
		newProfiles = (char **)ILRealloc
			(profiles, (numProfiles + 64) * sizeof(char *));
		if(!newProfiles)
		{
			fputs("virtual memory exhauted\n", stderr);
			exit(1);
		}
		profiles = newProfiles;
		maxProfiles += 64;
	}
	if((profiles[numProfiles] = (char *)ILMalloc(size + 1)) == 0)
	{
		fputs("virtual memory exhauted\n", stderr);
		exit(1);
	}
	profiles[numProfiles][0] = (char)size;
	ILMemCpy(profiles[numProfiles] + 1, profile, size);
	++numProfiles;

	/* Dump the function heading */
	fputs("#if !defined(HAVE_LIBFFI)\n\n", outfile);
	fputs("static void ", outfile);
	dumpFfiMarshalerName(outfile, profile, size);
	fputs("(void (*fn)(), void *rvalue, void **avalue)\n{\n", outfile);

	/* Assign the return value */
	putc('\t', outfile);
	if(profile[0] != IL_FFI_VOID)
	{
		fputs("*((", outfile);
		dumpFfiReturnType(outfile, profile[0]);
		fputs(" *)rvalue) = ", outfile);
	}

	/* Cast the function pointer to the correct prototype and call it */
	fputs("(*(", outfile);
	dumpFfiType(outfile, profile[0]);
	putc(' ', outfile);
	fputs("(*)(", outfile);
	for(index = 1; index < size; ++index)
	{
		if(index != 1)
		{
			fputs(", ", outfile);
		}
		dumpFfiType(outfile, profile[index]);
	}
	fputs("))fn)", outfile);

	/* Output the arguments */
	putc('(', outfile);
	for(index = 1; index < size; ++index)
	{
		if(index != 1)
		{
			fputs(", ", outfile);
		}
		fputs("*((", outfile);
		dumpFfiType(outfile, profile[index]);
		fprintf(outfile, " *)(avalue[%d]))", index - 1);
	}
	fputs(");\n", outfile);

	/* Dump the function footing */
	fputs("}\n\n#endif\n\n", outfile);
}

/*
 * Dump all of the "internalcall" methods for a class.
 */
static void dumpClassInternals(FILE *outfile, const char *prefix,
							   int modes, ILClass *classInfo)
{
	ILMethod *method;
	ILMethod *nextMethod;
	int needMangle;
	char profile[IL_FFI_SIZE];
	int profileSize;

	/* Scan the class and declare the method prototypes */
	method = 0;
	while((method = (ILMethod *)ILClassNextMemberByKind
				(classInfo, (ILMember *)method,
				 IL_META_MEMBERKIND_METHOD)) != 0)
	{
		if(ILMethod_IsInternalCall(method))
		{
			/* See if we have another internalcall method with the
			   same name, to determine if mangling is necessary */
			if((ILMethod_ImplAttrs(method) & MANGLE_MARK_BIT) != 0)
			{
				/* This was marked when we saw a previous method
				   with the same name */
				needMangle = 1;
			}
			else
			{
				/* Scan forward from here for other methods that
				   have the same name as this one */
				nextMethod = method;
				needMangle = 0;
				while((nextMethod = (ILMethod *)ILClassNextMemberByKind
							(classInfo, (ILMember *)nextMethod,
							IL_META_MEMBERKIND_METHOD)) != 0)
				{
					if(ILMethod_IsInternalCall(nextMethod) &&
					   !strcmp(ILMethod_Name(method),
					   		   ILMethod_Name(nextMethod)))
					{
						needMangle = 1;
						ILMethodSetImplAttrs(method,
											 MANGLE_MARK_BIT,
											 MANGLE_MARK_BIT);
						ILMethodSetImplAttrs(nextMethod,
											 MANGLE_MARK_BIT,
											 MANGLE_MARK_BIT);
					}
				}
			}

			/* Dump the prototype for the method */
			if((modes & OUT_MODE_PROTOTYPES) != 0)
			{
				dumpMethodPrototype(outfile, prefix, method, needMangle);
			}

			/* Dump marshalling information for the method */
			if((modes & OUT_MODE_TABLES) != 0)
			{
				profileSize = getFfiProfile(method, profile);
				dumpFfiMarshaler(outfile, profile, profileSize);
			}
		}
	}
	if((modes & OUT_MODE_PROTOTYPES) != 0)
	{
		putc('\n', outfile);
	}

	/* Scan the class and declare the method table */
	if((modes & OUT_MODE_TABLES) != 0)
	{
		fprintf(outfile, "#ifndef _IL_%s_suppressed\n\n",
				ILClass_Name(classInfo));
		fputs("IL_METHOD_BEGIN(", outfile);
		dumpMangledClass(outfile, classInfo);
		fputs("_Methods)\n", outfile);
		method = 0;
		while((method = (ILMethod *)ILClassNextMemberByKind
					(classInfo, (ILMember *)method,
					 IL_META_MEMBERKIND_METHOD)) != 0)
		{
			if(ILMethod_IsInternalCall(method))
			{
				profileSize = getFfiProfile(method, profile);
				needMangle = ((ILMethod_ImplAttrs(method) &
									MANGLE_MARK_BIT) != 0);
				if(ILMethodIsConstructor(method))
				{
					/* Dump method table information for a constructor */
					fputs("\tIL_CONSTRUCTOR(\".ctor\", \"", outfile);
					dumpLookupType(outfile, ILMethod_Signature(method), 1);
					fputs("\", ", outfile);
					if(ILClassIsValueType(classInfo))
					{
						/* Value type constructors use a regular method call */
						fputs(prefix, outfile);
						dumpMethodFuncName(outfile, method, needMangle);
						fputs(", ", outfile);
						dumpFfiMarshalerName(outfile, profile, profileSize);
						fputs(", 0, 0)\n", outfile);
					}
					else
					{
						/* Object constructors use manual allocation */
						fputs("0, 0, ", outfile);
						fputs(prefix, outfile);
						dumpMethodFuncName(outfile, method, needMangle);
						fputs(", ", outfile);
						dumpFfiMarshalerName(outfile, profile, profileSize);
						fputs(")\n", outfile);
					}
				}
				else
				{
					/* Dump method table information for a regular method */
					fputs("\tIL_METHOD(\"", outfile);
					fputs(ILMethod_Name(method), outfile);
					fputs("\", \"", outfile);
					dumpLookupType(outfile, ILMethod_Signature(method), 1);
					fputs("\", ", outfile);
					fputs(prefix, outfile);
					dumpMethodFuncName(outfile, method, needMangle);
					fputs(", ", outfile);
					dumpFfiMarshalerName(outfile, profile, profileSize);
					fputs(")\n", outfile);
				}
			}
		}
		fputs("IL_METHOD_END\n\n#endif\n\n", outfile);
	}
}

/*
 * Table of class names that we have seen.
 */
typedef struct
{
	char *name;
	char *namespace;

} ClassNameInfo;
static ClassNameInfo *classTable = 0;
static unsigned long classTableSize = 0;

/*
 * Determine if a class is already in the class table.
 */
static int classAlreadyPresent(const char *name, const char *nspace)
{
	unsigned long posn;
	for(posn = 0; posn < classTableSize; ++posn)
	{
		if(strcmp(name, classTable[posn].name) != 0)
		{
			continue;
		}
		if(nspace && classTable[posn].namespace &&
		   !strcmp(nspace, classTable[posn].namespace))
		{
			return 1;
		}
		else if(nspace == classTable[posn].namespace)
		{
			return 1;
		}
	}
	return 0;
}

/*
 * Load an IL image and print stub information for internalcall methods.
 */
static int processFile(const char *filename, ILContext *context,
					   const char *prefix, int modes, FILE *outfile)
{
	ILImage *image;
	unsigned long numTypes;
	unsigned long token;
	ILMethod *method;
	ILClass *classInfo;

	/* Attempt to load the image into memory */
	if(ILImageLoadFromFile(filename, context, &image,
					  	   IL_LOADFLAG_FORCE_32BIT |
					  	   IL_LOADFLAG_NO_RESOLVE, 1) != 0)
	{
		return 1;
	}

	/* Walk the TypeDef table and dump information for all
	   types that contain "internalcall" methods */
	numTypes = ILImageNumTokens(image, IL_META_TOKEN_TYPE_DEF);
	for(token = 1; token <= numTypes; ++token)
	{
		classInfo = (ILClass *)ILImageTokenInfo
							(image, IL_META_TOKEN_TYPE_DEF | token);
		if(classInfo)
		{
			/* If we already saw this class in another assembly, skip it */
			if(classAlreadyPresent(ILClass_Name(classInfo),
								   ILClass_Namespace(classInfo)))
			{
				continue;
			}

			/* Determine if this class has "internalcall" methods */
			method = 0;
			while((method = (ILMethod *)ILClassNextMemberByKind
						(classInfo, (ILMember *)method,
						 IL_META_MEMBERKIND_METHOD)) != 0)
			{
				if(ILMethod_IsInternalCall(method))
				{
					break;
				}
			}
			if(method != 0)
			{
				dumpClassInternals(outfile, prefix, modes, classInfo);
				classTable = (ClassNameInfo *)ILRealloc
					(classTable, sizeof(ClassNameInfo) * (classTableSize + 1));
				if(!classTable)
				{
					fprintf(stderr, "%s: out of memory\n", filename);
					exit(1);
				}
				classTable[classTableSize].name =
					ILDupString(ILClass_Name(classInfo));
				if(!(classTable[classTableSize].name))
				{
					fprintf(stderr, "%s: out of memory\n", filename);
					exit(1);
				}
				if(ILClass_Namespace(classInfo))
				{
					classTable[classTableSize].namespace =
						ILDupString(ILClass_Namespace(classInfo));
					if(!(classTable[classTableSize].namespace))
					{
						fprintf(stderr, "%s: out of memory\n", filename);
						exit(1);
					}
				}
				else
				{
					classTable[classTableSize].namespace = 0;
				}
				++classTableSize;
			}
		}
	}


	/* Clean up and exit */
	ILImageDestroy(image);
	return 0;
}

/*
 * Dump the "internalcall" class table.
 */
static void dumpClassTable(FILE *outfile)
{
	unsigned long index, index2;
	ClassNameInfo temp;

	/* Sort the class table into ascending order of class name */
	for(index = 0; index < (classTableSize - 1); ++index)
	{
		for(index2 = (index + 1); index2 < classTableSize; ++index2)
		{
			if(strcmp(classTable[index].name,
					  classTable[index2].name) > 0)
			{
				temp = classTable[index];
				classTable[index] = classTable[index2];
				classTable[index2] = temp;
			}
		}
	}

	/* Output the internal class table for the engine */
	fputs("typedef struct\n{\n", outfile);
	fputs("\tconst char *name;\n", outfile);
	fputs("\tconst char *namespace;\n", outfile);
	fputs("\tconst ILMethodTableEntry *entry;\n\n", outfile);
	fputs("} InternalClassInfo;\n", outfile);
	fputs("static InternalClassInfo const internalClassTable[] = {\n", outfile);
	for(index = 0; index < classTableSize; ++index)
	{
		fprintf(outfile, "#ifndef _IL_%s_suppressed\n",
				classTable[index].name);
		fputs("\t{\"", outfile);
		fputs(classTable[index].name, outfile);
		fputs("\", \"", outfile);
		if(classTable[index].namespace)
		{
			fputs(classTable[index].namespace, outfile);
		}
		fputs("\", ", outfile);
		dumpMangledClassName(outfile, classTable[index].name,
							 classTable[index].namespace);
		fputs("_Methods},\n#endif\n", outfile);
	}
	fputs("};\n", outfile);
	fputs("#define numInternalClasses (sizeof(internalClassTable) / sizeof(InternalClassInfo))\n", outfile);
}

#ifdef	__cplusplus
};
#endif
