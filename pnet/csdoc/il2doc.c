/*
 * il2doc.c - Convert an IL binary into XML documentation form.
 *
 * Copyright (C) 2003, 2008, 2009  Southern Storm Software, Pty Ltd.
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
#include <stdlib.h>
#include "il_system.h"
#include "il_utils.h"
#include "il_program.h"
#include "il_dumpasm.h"
#include "il_serialize.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-v", 'v', 0, 0, 0},
	{"--version", 'v', 0,
		"--version    or -v",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

static void usage(const char *progname);
static void version(void);
static int dump(const char *filename, ILContext *context);

int main(int argc, char *argv[])
{
	char *progname = argv[0];
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

	/* Create a context to use for image loading */
	context = ILContextCreate();
	if(!context)
	{
		fprintf(stderr, "%s: out of memory\n", progname);
		return 1;
	}

	/* Print the XML header */
	printf("<Libraries><Types>\n");

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
				errors |= dump("-", context);
				sawStdin = 1;
			}
		}
		else
		{
			/* Dump the contents of a regular file */
			errors |= dump(argv[1], context);
		}
		++argv;
		--argc;
	}

	/* Print the XML footer */
	printf("</Types></Libraries>\n");

	/* Destroy the context */
	ILContextDestroy(context);
	
	/* Done */
	return errors;
}

static void usage(const char *progname)
{
	fprintf(stdout, "IL2DOC " VERSION " - IL Image To Doc Conversion\n");
	fprintf(stdout, "Copyright (c) 2003 2007 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("IL2DOC " VERSION " - IL Image To Doc Conversion\n");
	printf("Copyright (c) 2003 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("IL2DOC comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * C# modifier flags for types.
 */
static ILFlagInfo const CSharpTypeFlags[] = {
	{"internal", IL_META_TYPEDEF_NOT_PUBLIC, IL_META_TYPEDEF_VISIBILITY_MASK},
	{"public", IL_META_TYPEDEF_PUBLIC, IL_META_TYPEDEF_VISIBILITY_MASK},
	{"public", IL_META_TYPEDEF_NESTED_PUBLIC,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"private", IL_META_TYPEDEF_NESTED_PRIVATE,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"protected", IL_META_TYPEDEF_NESTED_FAMILY,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"internal", IL_META_TYPEDEF_NESTED_ASSEMBLY,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"protected internal", IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"abstract", IL_META_TYPEDEF_ABSTRACT, 0},
	{"sealed", IL_META_TYPEDEF_SEALED, 0},
	{0, 0, 0},
};

/*
 * C# modifier flags for fields.
 */
static ILFlagInfo const CSharpFieldFlags[] = {
	{"private", IL_META_FIELDDEF_PRIVATE,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"internal", IL_META_FIELDDEF_ASSEMBLY,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"protected", IL_META_FIELDDEF_FAMILY,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"protected internal", IL_META_FIELDDEF_FAM_OR_ASSEM,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"public", IL_META_FIELDDEF_PUBLIC,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"static", IL_META_FIELDDEF_STATIC, 0},
	{"readonly", IL_META_FIELDDEF_INIT_ONLY, 0},
	{"const", IL_META_FIELDDEF_LITERAL, 0},
	{0, 0, 0},
};

/*
 * C# modifier flags for methods.
 */
static ILFlagInfo const CSharpMethodFlags[] = {
	{"private", IL_META_METHODDEF_PRIVATE,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"internal", IL_META_METHODDEF_ASSEM,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"protected", IL_META_METHODDEF_FAMILY,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"protected internal", IL_META_METHODDEF_FAM_OR_ASSEM,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"public", IL_META_METHODDEF_PUBLIC,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"static", IL_META_METHODDEF_STATIC, 0},
	{"virtual", IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT,
				IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT |
						IL_META_METHODDEF_ABSTRACT},
	{"override", IL_META_METHODDEF_VIRTUAL,
				 IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT},
	{"abstract", IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT |
						IL_META_METHODDEF_ABSTRACT,
				 IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT |
						IL_META_METHODDEF_ABSTRACT},
	{0, 0, 0},
};

/*
 * Flags for PrintStringWithFlags and PrintClassName
 */
#define DUMP_GENERIC_PARAMS			IL_DUMP_GENERIC_PARAMS
#define DUMP_OMIT_CONSTRAINTS		64
#define DUMP_OMIT_ARITY				128
#define DUMP_SP						256
#define DUMP_NAME_ONLY				512
#define DUMP_CS_STYLE				1024
#define DUMP_OMIT_NAMESPACE			2048

#define DUMP_STYLE_ILASM			(0)
#define DUMP_STYLE_ILASM_CLASS		(IL_DUMP_GENERIC_PARAMS | DUMP_NAME_ONLY)
#define DUMP_STYLE_CSHARP			(DUMP_OMIT_ARITY | DUMP_CS_STYLE)
#define DUMP_STYLE_CSHARP_CLASS		(IL_DUMP_GENERIC_PARAMS | DUMP_OMIT_ARITY \
									| DUMP_CS_STYLE | DUMP_NAME_ONLY)
#define DUMP_STYLE_CSHARP_METHOD	(IL_DUMP_GENERIC_PARAMS | DUMP_OMIT_ARITY \
									| DUMP_CS_STYLE)
#define DUMP_STYLE_FULLNAME			(IL_DUMP_GENERIC_PARAMS | DUMP_OMIT_ARITY \
									| DUMP_OMIT_CONSTRAINTS | DUMP_CS_STYLE)
#define DUMP_STYLE_NAME				(DUMP_STYLE_FULLNAME | DUMP_OMIT_NAMESPACE)
#define DUMP_STYLE_FULLNAMESP		(DUMP_STYLE_FULLNAME | DUMP_SP)

static void _ILDumpMethodType(FILE *stream, ILImage *image, ILType *type,
						 	  int flags, ILClass *info, const char *methodName,
					  		  ILMethod *methodInfo);

static void PrintType(ILType *type, int csForm, ILParameter *parameter,
					  ILProgramItem *scope);

#if IL_VERSION_MAJOR > 1
static void _DumpGenericConstraintHeader(FILE *stream, ILGenericPar *genPar,
										 int flags)
{
	if((flags & DUMP_CS_STYLE) != 0)
	{
		fputs(" where ", stream);
		fputs(ILGenericParGetName(genPar), stream);
		fputs(": ", stream);
	}
}

static void _DumpGenericConstraint(FILE *stream, ILGenericPar *genPar,
								   ILProgramItem *scope, int flags)
{
	ILUInt32 constraintFlags = ILGenericParGetFlags(genPar);
	ILGenericConstraint *constraint = 0;

	if((flags & DUMP_CS_STYLE) != 0)
	{
		ILUInt32 hasHeader = 0;

		if((constraintFlags & IL_META_GENPARAM_CLASS_CONST) != 0)
		{
			if(!hasHeader)
			{
				_DumpGenericConstraintHeader(stream, genPar, flags);
				hasHeader = 1;
			}
			else
			{
				fputs(", ", stream);
			}
			fputs("class", stream);
		}
		else if((constraintFlags & IL_META_GENPARAM_VALUETYPE_CONST) != 0)
		{
			if(!hasHeader)
			{
				_DumpGenericConstraintHeader(stream, genPar, flags);
				hasHeader = 1;
			}
			else
			{
				fputs(", ", stream);
			}
			fputs("struct", stream);
		}

		flags &= ~IL_DUMP_GENERIC_PARAMS;
		while((constraint = ILGenericParNextConstraint(genPar, constraint)) != 0)
		{
			ILClass *classInfo;
			ILTypeSpec *typeSpec;
			ILProgramItem *item = ILConstraintGetType(constraint);

			if(!hasHeader)
			{
				_DumpGenericConstraintHeader(stream, genPar, flags);
				hasHeader = 1;
			}
			else
			{
				fputs(", ", stream);
			}

			if((classInfo = ILProgramItemToClass(item)) != 0)
			{
				PrintType(ILClassToType(classInfo), 0, 0, scope);
			}
			else if((typeSpec = ILProgramItemToTypeSpec(item)) != 0)
			{
				PrintType(ILTypeSpecGetType(typeSpec), 0, 0, scope);
			}
		}

		if((constraintFlags & IL_META_GENPARAM_CTOR_CONST) != 0)
		{
			if(!hasHeader)
			{
				_DumpGenericConstraintHeader(stream, genPar, flags);
				hasHeader = 1;
			}
			else
			{
				fputs(", ", stream);
			}
			fputs("new()", stream);
		}
	}
	else
	{
		ILUInt32 needSP = 0;
		ILUInt32 numConstraints = 0;

		if((constraintFlags & IL_META_GENPARAM_COVARIANT) != 0)
		{
			putc('+', stream);
			needSP = 1;
		}
		if((constraintFlags & IL_META_GENPARAM_CONTRAVARIANT) != 0)
		{
			putc('-', stream);
			needSP = 1;
		}
		if((constraintFlags & IL_META_GENPARAM_CLASS_CONST) != 0)
		{
			if(needSP)
			{
				putc(' ', stream);
			}
			fputs("class", stream);
			needSP = 1;
		}
		if((constraintFlags & IL_META_GENPARAM_VALUETYPE_CONST) != 0)
		{
			if(needSP)
			{
				putc(' ', stream);
			}
			fputs("valuetype", stream);
			needSP = 1;
		}
		if((constraintFlags & IL_META_GENPARAM_CTOR_CONST) != 0)
		{
			if(needSP)
			{
				putc(' ', stream);
			}
			fputs(".ctor", stream);
			needSP = 1;
		}
		if(needSP)
		{
			putc(' ', stream);
		}

		flags &= ~IL_DUMP_GENERIC_PARAMS;
		while((constraint = ILGenericParNextConstraint(genPar, constraint)) != 0)
		{
			ILClass *classInfo;
			ILTypeSpec *typeSpec;
			ILProgramItem *item = ILConstraintGetType(constraint);

			if(numConstraints == 0)
			{
				putc('(', stream);
			}
			else
			{
				fputs(", ", stream);
			}

			if((classInfo = ILProgramItemToClass(item)) != 0)
			{
				PrintType(ILClassToType(classInfo), 0, 0, scope);
			}
			else if((typeSpec = ILProgramItemToTypeSpec(item)) != 0)
			{
				PrintType(ILTypeSpecGetType(typeSpec), 0, 0, scope);
			}
			++numConstraints;
		}
		if(numConstraints > 0)
		{
			fputs(") ", stream);
		}
	}
}

static void _ILDumpGenericConstraints(FILE *stream, ILProgramItem *scope,
									  ILUInt32 from, ILUInt32 to, int flags)
{
	ILUInt32 current = from;
	ILGenericPar *genPar;

	while((current < to) &&
		  (genPar = ILGenericParGetFromOwner(scope, current)) != 0)
	{
		_DumpGenericConstraint(stream, genPar, scope, flags);
		++current;
	}
}

static void _ILDumpGenericClassConstraints(FILE *stream, ILClass *classInfo,
										   int flags)
{
	ILUInt32 numGenericPars = ILClassGetNumGenericPars(classInfo);

	flags &= ~DUMP_GENERIC_PARAMS;
	if(numGenericPars > 0)
	{
		if((flags & DUMP_CS_STYLE) != 0)
		{
			ILClass *nestedParent;
			ILUInt32 numParentGenericPars = 0;

			nestedParent = ILClass_NestedParent(classInfo);
			if(nestedParent)
			{
				numParentGenericPars = ILClassGetNumGenericPars(nestedParent);
			}
			_ILDumpGenericConstraints(stdout, ILToProgramItem(classInfo),
									  numParentGenericPars, numGenericPars,
									  flags);
		}
		else
		{
			_ILDumpGenericConstraints(stdout, ILToProgramItem(classInfo),
									  0, numGenericPars, flags);
		}
	}
}

static void _ILDumpGenericParms(FILE *stream, ILProgramItem *scope,
								ILUInt32 from, ILUInt32 to, int flags)
{
	ILUInt32 current = from;
	ILGenericPar *genPar;

	while((current < to) &&
		  (genPar = ILGenericParGetFromOwner(scope, current)) != 0)
	{
		if(current == from)
		{
			fputs("&lt;", stream);
		}
		else
		{
			putc(',', stream);
		}
		if((flags & (DUMP_OMIT_CONSTRAINTS | DUMP_CS_STYLE)) == 0)
		{
			_DumpGenericConstraint(stream, genPar, scope, flags);
		}
		fputs(ILGenericParGetName(genPar), stream);
		current++;
	}
	if(current > from)
	{
		fputs("&gt;", stream);
	}
}
#endif /* IL_VERSION_MAJOR > 1 */

static int NestedClassIsAccessible(ILClass *classInfo)
{
	if(classInfo)
	{
		ILUInt32 attrs = ILClassGetAttrs(classInfo);

		switch(attrs & IL_META_TYPEDEF_VISIBILITY_MASK)
		{
			case IL_META_TYPEDEF_NOT_PUBLIC:
			case IL_META_TYPEDEF_NESTED_ASSEMBLY:
			{
				/* The class is only accessible in the same image */
				return 0;
			}
			break;

			case IL_META_TYPEDEF_PUBLIC:
			{
				return 1;
			}
			/* Not reached */

			case IL_META_TYPEDEF_NESTED_PUBLIC:
			{
				/* The parent is accessible so the child is accessible too */
				return 1;
			}
			/* Not reached */

			case IL_META_TYPEDEF_NESTED_PRIVATE:
			{
				/* Nested class accessible from parent, siblings, or children */
				return 0;
			}
			/* Not reached */

			case IL_META_TYPEDEF_NESTED_FAMILY:
			{
				/* Accessible to private or inherited scopes */
				return 1;
			}
			/* Not reached */

			case IL_META_TYPEDEF_NESTED_FAM_AND_ASSEM:
			{
				/* Is not accessible from an other image */
				return 0;
			}
			break;

			case IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM:
			{
				/* Can be inherited from a class in an other image */
				return 1;
			}
			break;
		}
	}
	return 0;
}

static void _ILDumpType(FILE *stream, ILImage *image, ILType *type, int flags)
{
	ILType *elem;

	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:
			{
				fputs("void", stream);
			}
			break;

			case IL_META_ELEMTYPE_BOOLEAN:
			{
				fputs("bool", stream);
			}
			break;

			case IL_META_ELEMTYPE_CHAR:
			{
				fputs("char", stream);
			}
			break;

			case IL_META_ELEMTYPE_I1:
			{
				fputs("int8", stream);
			}
			break;

			case IL_META_ELEMTYPE_U1:
			{
				fputs("unsigned int8", stream);
			}
			break;

			case IL_META_ELEMTYPE_I2:
			{
				fputs("int16", stream);
			}
			break;

			case IL_META_ELEMTYPE_U2:
			{
				fputs("unsigned int16", stream);
			}
			break;

			case IL_META_ELEMTYPE_I4:
			{
				fputs("int32", stream);
			}
			break;

			case IL_META_ELEMTYPE_U4:
			{
				fputs("unsigned int32", stream);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			{
				fputs("int64", stream);
			}
			break;

			case IL_META_ELEMTYPE_U8:
			{
				fputs("unsigned int64", stream);
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				fputs("float32", stream);
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				fputs("float64", stream);
			}
			break;

			case IL_META_ELEMTYPE_TYPEDBYREF:
			{
				fputs("typedref", stream);
			}
			break;

			case IL_META_ELEMTYPE_I:
			{
				fputs("native int", stream);
			}
			break;

			case IL_META_ELEMTYPE_U:
			{
				fputs("native unsigned int", stream);
			}
			break;

			case IL_META_ELEMTYPE_R:
			{
				fputs("native float", stream);
			}
			break;

			case IL_META_ELEMTYPE_SENTINEL:
			{
				fputs("SENTINEL", stream);
			}
			break;

			default:
			{
				fputs("UNKNOWN PRIMITIVE TYPE", stream);
			}
			break;
		}
	}
	else if(ILType_IsClass(type))
	{
		fputs("class ", stream);
		ILDumpClassName(stream, image, ILType_ToClass(type), flags);
	}
	else if(ILType_IsValueType(type))
	{
		fputs("valuetype ", stream);
		ILDumpClassName(stream, image, ILType_ToValueType(type), flags);
	}
	else if(type && ILType_IsComplex(type))
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_BYREF:
			{
				_ILDumpType(stream, image, ILType_Ref(type), flags);
				fputs(" &amp;", stream);
			}
			break;

			case IL_TYPE_COMPLEX_PTR:
			{
				_ILDumpType(stream, image, ILType_Ref(type), flags);
				fputs(" *", stream);
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY:
			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				/* Find the element type and dump it */
				elem = type->un.array__.elemType__;
				while(elem != 0 && ILType_IsComplex(elem) &&
				      (elem->kind__ == IL_TYPE_COMPLEX_ARRAY ||
					   elem->kind__ == IL_TYPE_COMPLEX_ARRAY_CONTINUE))
				{
					elem = elem->un.array__.elemType__;
				}
				_ILDumpType(stream, image, elem, flags);

				/* Dump the dimensions */
				putc('[', stream);
				elem = type;
				while(elem != 0 && ILType_IsComplex(elem) &&
				      (elem->kind__ == IL_TYPE_COMPLEX_ARRAY ||
					   elem->kind__ == IL_TYPE_COMPLEX_ARRAY_CONTINUE))
				{
					if(elem->un.array__.size__ != 0)
					{
						if(elem->un.array__.lowBound__ != 0)
						{
							/* We have both a low bound and a size */
							fprintf(stream, "%ld...%ld",
									elem->un.array__.lowBound__,
									elem->un.array__.lowBound__ +
										elem->un.array__.size__ - 1);
						}
						else
						{
							/* We only have a size */
							fprintf(stream, "%ld", elem->un.array__.size__);
						}
					}
					else if(elem->un.array__.lowBound__ != 0)
					{
						/* We only have a low bound */
						fprintf(stream, "%ld...", elem->un.array__.lowBound__);
					}
					if(elem->kind__ == IL_TYPE_COMPLEX_ARRAY)
					{
						putc(']', stream);
						elem = elem->un.array__.elemType__;
						if(ILType_IsComplex(elem) && elem != 0 &&
						   (elem->kind__ == IL_TYPE_COMPLEX_ARRAY ||
						    elem->kind__ == IL_TYPE_COMPLEX_ARRAY_CONTINUE))
						{
							putc('[', stream);
						}
					}
					else
					{
						putc(',', stream);
						elem = elem->un.array__.elemType__;
					}
				}
			}
			break;

			case IL_TYPE_COMPLEX_CMOD_REQD:
			{
				_ILDumpType(stream, image, type->un.modifier__.type__, flags);
				fputs(" modreq(", stream);
				ILDumpClassName(stream, image,
								type->un.modifier__.info__, flags);
				putc(')', stream);
			}
			break;

			case IL_TYPE_COMPLEX_CMOD_OPT:
			{
				_ILDumpType(stream, image, type->un.modifier__.type__, flags);
				fputs(" modopt(", stream);
				ILDumpClassName(stream, image,
								type->un.modifier__.info__, flags);
				putc(')', stream);
			}
			break;

			case IL_TYPE_COMPLEX_PROPERTY:
			{
				fputs("property ", stream);
				_ILDumpMethodType(stream, image, type, flags, 0, 0, 0);
			}
			break;

			case IL_TYPE_COMPLEX_SENTINEL:
			{
				fputs("...", stream);
			}
			break;

			case IL_TYPE_COMPLEX_PINNED:
			{
				_ILDumpType(stream, image, ILType_Ref(type), flags);
				fputs(" pinned", stream);
			}
			break;

		#if IL_VERSION_MAJOR > 1
			case IL_TYPE_COMPLEX_WITH:
			{
				unsigned long numParams;
				unsigned long param;
				_ILDumpType(stream, image,
						   ILTypeGetWithMainWithPrefixes(type), flags);
				fputs("&lt;", stream);
				numParams = ILTypeNumWithParams(type);
				for(param = 1; param <= numParams; ++param)
				{
					if(param != 1)
					{
						putc(',', stream);
					}
					_ILDumpType(stream, image,
							   ILTypeGetWithParamWithPrefixes(type, param),
							   flags);
				}
				fputs("&gt;", stream);
			}
			break;

			case IL_TYPE_COMPLEX_MVAR:
			{
				fprintf(stream, "!!%d", ILType_VarNum(type));
			}
			break;

			case IL_TYPE_COMPLEX_VAR:
			{
				fprintf(stream, "!%d", ILType_VarNum(type));
			}
			break;
		#endif /* IL_VERSION_MAJOR > 1 */

			default:
			{
				if((type->kind__ & IL_TYPE_COMPLEX_METHOD) != 0)
				{
					fputs("method ", stream);
					_ILDumpMethodType(stream, image, type, flags, 0, 0, 0);
				}
				else
				{
					fprintf(stream, "UNKNOWN COMPLEX TYPE %08X",
							(int)(type->kind__));
				}
			}
			break;
		}
	}
	else
	{
		fputs("UNKNOWN TYPE", stream);
	}
}

/*
 * Dump parameter type, attribute, marshalling, and name information.
 */
static void DumpParamType(FILE *stream, ILImage *image,
						  ILMethod *method, ILType *paramType,
						  ILUInt32 num, int flags)
{
	ILParameter *param;
	ILFieldMarshal *marshal;
	const void *type;
	ILUInt32 typeLen;
	const char *name;

	/* Get the parameter information block, if one is present */
	param = 0;
	if(method)
	{
		while((param = ILMethodNextParam(method, param)) != 0)
		{
			if(ILParameterGetNum(param) == num)
			{
				break;
			}
		}
	}

	/* Dump the parameter attributes */
	if(param)
	{
		if(ILParameter_IsIn(param))
		{
			fputs("[in] ", stream);
		}
		if(ILParameter_IsOut(param))
		{
			fputs("[out] ", stream);
		}
		if(ILParameter_IsRetVal(param))
		{
			fputs("[retval] ", stream);
		}
		if(ILParameter_IsOptional(param))
		{
			fputs("[opt] ", stream);
		}
	}

	/* Dump the parameter's type */
	_ILDumpType(stream, image, paramType, flags);

	/* Dump the field marshalling information, if present */
    if(param &&
	   (ILParameter_Attrs(param) & IL_META_PARAMDEF_HAS_FIELD_MARSHAL) != 0)
	{
		marshal = ILFieldMarshalGetFromOwner((ILProgramItem *)param);
		if(marshal)
		{
			type = ILFieldMarshalGetType(marshal, &typeLen);
			if(type)
			{
				fputs(" marshal(", stream);
				ILDumpNativeType(stream, type, typeLen,
								 flags | IL_DUMP_XML_QUOTING);
				putc(')', stream);
			}
		}
	}

	/* Dump the parameter's name */
	if(param && num != 0)
	{
		name = ILParameter_Name(param);
		if(name)
		{
			putc(' ', stream);
			ILDumpIdentifier(stream, name, 0, flags);
		}
	}
}

static void DumpParams(FILE *stream, ILImage *image, ILType *type,
					   ILMethod *methodInfo, int flags)
{
	ILType *temp;
	unsigned long num;
	ILUInt32 pnum;
	if(!(type->num__))
	{
		return;
	}
	DumpParamType(stream, image, methodInfo,
				  type->un.method__.param__[0], 1, flags);
	if(type->num__ == 1)
	{
		return;
	}
	fputs(", ", stream);
	DumpParamType(stream, image, methodInfo,
				  type->un.method__.param__[1], 2, flags);
	if(type->num__ == 2)
	{
		return;
	}
	fputs(", ", stream);
	DumpParamType(stream, image, methodInfo,
				  type->un.method__.param__[2], 3, flags);
	temp = type->un.method__.next__;
	num = type->num__ - 3;
	pnum = 4;
	while(num > 4)
	{
		fputs(", ", stream);
		DumpParamType(stream, image, methodInfo,
					  temp->un.params__.param__[0], pnum++, flags);
		fputs(", ", stream);
		DumpParamType(stream, image, methodInfo,
					  temp->un.params__.param__[1], pnum++, flags);
		fputs(", ", stream);
		DumpParamType(stream, image, methodInfo,
					  temp->un.params__.param__[2], pnum++, flags);
		fputs(", ", stream);
		DumpParamType(stream, image, methodInfo,
					  temp->un.params__.param__[3], pnum++, flags);
		num -= 4;
		temp = temp->un.params__.next__;
	}
	if(num > 0)
	{
		fputs(", ", stream);
		DumpParamType(stream, image, methodInfo,
					  temp->un.params__.param__[0], pnum++, flags);
	}
	if(num > 1)
	{
		fputs(", ", stream);
		DumpParamType(stream, image, methodInfo,
					  temp->un.params__.param__[1], pnum++, flags);
	}
	if(num > 2)
	{
		fputs(", ", stream);
		DumpParamType(stream, image, methodInfo,
					  temp->un.params__.param__[2], pnum++, flags);
	}
	if(num > 3)
	{
		fputs(", ", stream);
		DumpParamType(stream, image, methodInfo,
					  temp->un.params__.param__[3], pnum++, flags);
	}
}

/*
 * Internal version of "_ILDumpMethodType" that can also handle
 * instantiations of generic method calls.
 */
static void DumpMethodType(FILE *stream, ILImage *image, ILType *type,
						   int flags, ILClass *info, const char *methodName,
					  	   ILMethod *methodInfo, ILType *withTypes)
{
	ILUInt32 callingConventions;
	ILType *synType;
#if IL_VERSION_MAJOR > 1
	int dumpGenerics;
	unsigned long numWithParams;
	unsigned long withParam;

	/* Determine if we need to dump the generic parameters */
	dumpGenerics = ((flags & IL_DUMP_GENERIC_PARAMS) != 0);

	/* Strip off the "generic parameters" flag so that we don't
	   end up passing it down to the parameter types */
	flags &= ~IL_DUMP_GENERIC_PARAMS;
#endif /* IL_VERSION_MAJOR > 1 */

	/* Dump the calling conventions for the method */
	callingConventions = ILType_CallConv(type);
	ILDumpFlags(stream, callingConventions, ILMethodCallConvFlags, 0);

	/* Dump the return type */
	DumpParamType(stream, image, methodInfo, ILTypeGetReturn(type), 0, flags);
	putc(' ', stream);

	/* Dump the class name and method name */
	if(info)
	{
		synType = ILClass_SynType(info);
		if(synType)
		{
			_ILDumpType(stream, image, synType, flags);
		}
		else
		{
			ILDumpClassName(stream, image, info, flags);
		}
		fputs("::", stream);
	}
	if(methodName)
	{
		if(*methodName != '\0')
		{
			ILDumpIdentifier(stream, methodName, 0, flags);
		}
	}
	else
	{
		putc('*', stream);
	}

#if IL_VERSION_MAJOR > 1
	/* Dump the generic method parameters if necessary */
	if(dumpGenerics && methodInfo)
	{
		_ILDumpGenericParms(stream, ILToProgramItem(methodInfo),
							0, ILType_NumGen(ILMethod_Signature(methodInfo)),
							flags);
	}
	else if(withTypes)
	{
		/* Dump the instantiation types from a method specification */
		fputs("&lt;", stream);
		numWithParams = ILTypeNumParams(withTypes);
		for(withParam = 1; withParam <= numWithParams; ++withParam)
		{
			if(withParam != 1)
			{
				putc(',', stream);
			}
			_ILDumpType(stream, image,
					   ILTypeGetParam(withTypes, withParam), flags);
		}
		fputs("&gt;", stream);
	}
#endif /* IL_VERSION_MAJOR > 1 */

	/* Dump the parameters */
	putc('(', stream);
	DumpParams(stream, image, type, methodInfo, flags);
	putc(')', stream);
}

static void _ILDumpMethodType(FILE *stream, ILImage *image, ILType *type,
						 	  int flags, ILClass *info, const char *methodName,
					  		  ILMethod *methodInfo)
{
	DumpMethodType(stream, image, type, flags, info,
				   methodName, methodInfo, 0);
}

/*
 * Abort if out of memory.
 */
static void ILDocOutOfMemory(int x)
{
	exit(1);
}

/*
 * Append two strings.
 */
static char *AppendString(char *str1, const char *str2)
{
	str1 = (char *)ILRealloc(str1, strlen(str1) + strlen(str2) + 1);
	if(!str1)
	{
		ILDocOutOfMemory(0);
	}
	strcat(str1, str2);
	return str1;
}

/*
 * Attribute usage flags.
 */
#define	AttrUsage_Assembly		0x0001
#define	AttrUsage_Module		0x0002
#define	AttrUsage_Class			0x0004
#define	AttrUsage_Struct		0x0008
#define	AttrUsage_Enum			0x0010
#define	AttrUsage_Constructor	0x0020
#define	AttrUsage_Method		0x0040
#define	AttrUsage_Property		0x0080
#define	AttrUsage_Field			0x0100
#define	AttrUsage_Event			0x0200
#define	AttrUsage_Interface		0x0400
#define	AttrUsage_Parameter		0x0800
#define	AttrUsage_Delegate		0x1000
#define	AttrUsage_ReturnValue	0x2000
#if IL_VERSION_MAJOR > 1
#define	AttrUsage_GenericParameter	0x4000
#define	AttrUsage_All			0x7FFF
#else /* IL_VERSION_MAJOR == 1 */
#define	AttrUsage_All			0x3FFF
#endif /* IL_VERSION_MAJOR == 1 */
#define	AttrUsage_ClassMembers	0x17FC

/*
 * Append an attribute usage target value to a string.
 */
static char *AppendAttrUsage(char *name, ILInt32 targets)
{
	int needOr = 0;

	/* Handle the easy case first */
	if(targets == AttrUsage_All)
	{
		return AppendString(name, "AttributeTargets.All");
	}

	/* Add the active flag names */
#define	AttrUsage(flag,flagName)	\
		do { \
			if((targets & (flag)) != 0) \
			{ \
				if(needOr) \
				{ \
					name = AppendString(name, " | "); \
				} \
				else \
				{ \
					needOr = 1; \
				} \
				name = AppendString(name, "AttributeTargets." flagName); \
			} \
		} while (0)
	AttrUsage(AttrUsage_Assembly, "Assembly");
	AttrUsage(AttrUsage_Module, "Module");
	AttrUsage(AttrUsage_Class, "Class");
	AttrUsage(AttrUsage_Struct, "Struct");
	AttrUsage(AttrUsage_Enum, "Enum");
	AttrUsage(AttrUsage_Constructor, "Constructor");
	AttrUsage(AttrUsage_Method, "Method");
	AttrUsage(AttrUsage_Property, "Property");
	AttrUsage(AttrUsage_Field, "Field");
	AttrUsage(AttrUsage_Event, "Event");
	AttrUsage(AttrUsage_Interface, "Interface");
	AttrUsage(AttrUsage_Parameter, "Parameter");
	AttrUsage(AttrUsage_Delegate, "Delegate");
	AttrUsage(AttrUsage_ReturnValue, "ReturnValue");
	return name;
}

/*
 * Strip down a string to remove assembly version qualifications.
 */
static int StripString(const char *strValue, int strLen)
{
	int posn;
	posn = 0;
	while(posn < (strLen - 11) &&
		  ILMemCmp(strValue + posn, ", Version=1", 11) != 0)
	{
		++posn;
	}
	if(posn >= (strLen - 11))
	{
		return strLen;
	}
	else
	{
		return posn;
	}
}

/*
 * Append an attribute value to a name.  Returns NULL
 * if the value is invalid.
 */
static char *AppendAttrValue(char *name, ILSerializeReader *reader,
							 int type, int isUsage)
{
	ILInt32 intValue;
	ILUInt32 uintValue;
	ILInt64 longValue;
	ILUInt64 ulongValue;
	ILFloat floatValue;
	ILDouble doubleValue;
	const char *strValue;
	int strLen, len;
	char buffer[64];

	switch(type)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		{
			intValue = ILSerializeReaderGetInt32(reader, type);
			if(intValue)
			{
				strcpy(buffer, "true");
			}
			else
			{
				strcpy(buffer, "false");
			}
		}
		break;

		case IL_META_SERIALTYPE_I1:
		case IL_META_SERIALTYPE_U1:
		case IL_META_SERIALTYPE_I2:
		case IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_CHAR:
		case IL_META_SERIALTYPE_I4:
		{
			intValue = ILSerializeReaderGetInt32(reader, type);
			if(!isUsage)
			{
				sprintf(buffer, "%ld", (long)intValue);
			}
			else
			{
				return AppendAttrUsage(name, intValue);
			}
		}
		break;

		case IL_META_SERIALTYPE_U4:
		{
			uintValue = ILSerializeReaderGetUInt32(reader, type);
			sprintf(buffer, "%lu", (unsigned long)uintValue);
		}
		break;

		case IL_META_SERIALTYPE_I8:
		{
			longValue = ILSerializeReaderGetInt64(reader);
			sprintf(buffer, "0x%08lX%08lX",
					(unsigned long)((longValue >> 32) & IL_MAX_UINT32),
					(unsigned long)(longValue & IL_MAX_UINT32));
		}
		break;

		case IL_META_SERIALTYPE_U8:
		{
			ulongValue = ILSerializeReaderGetUInt64(reader);
			sprintf(buffer, "0x%08lX%08lX",
					(unsigned long)((ulongValue >> 32) & IL_MAX_UINT32),
					(unsigned long)(ulongValue & IL_MAX_UINT32));
		}
		break;

		case IL_META_SERIALTYPE_R4:
		{
			floatValue = ILSerializeReaderGetFloat32(reader);
			sprintf(buffer, "%.30e", (double)floatValue);
		}
		break;

		case IL_META_SERIALTYPE_R8:
		{
			doubleValue = ILSerializeReaderGetFloat64(reader);
			sprintf(buffer, "%.30e", (double)doubleValue);
		}
		break;

		case IL_META_SERIALTYPE_STRING:
		{
			strLen = ILSerializeReaderGetString(reader, &strValue);
			if(strLen == -1)
			{
				ILFree(name);
				return 0;
			}
			strLen = StripString(strValue, strLen);
			len = strlen(name);
			name = (char *)ILRealloc(name, len + strLen + 3);
			if(!name)
			{
				ILDocOutOfMemory(0);
			}
			name[len++] = '"';
			ILMemCpy(name + len, strValue, strLen);
			name[len + strLen] = '"';
			name[len + strLen + 1] = '\0';
			return name;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_TYPE:
		{
			strLen = ILSerializeReaderGetString(reader, &strValue);
			if(strLen == -1)
			{
				ILFree(name);
				return 0;
			}
			strLen = StripString(strValue, strLen);
			len = strlen(name);
			name = (char *)ILRealloc(name, len + strLen + 9);
			if(!name)
			{
				ILDocOutOfMemory(0);
			}
			strcpy(name + len, "typeof(");
			len += 7;
			ILMemCpy(name + len, strValue, strLen);
			name[len + strLen] = ')';
			name[len + strLen + 1] = '\0';
			return name;
		}
		/* Not reached */

		default:
		{
			if((type & IL_META_SERIALTYPE_ARRAYOF) != 0)
			{
				intValue = ILSerializeReaderGetArrayLen(reader);
				name = AppendString(name, "{");
				while(intValue > 0)
				{
					name = AppendAttrValue(name, reader,
									       type & ~IL_META_SERIALTYPE_ARRAYOF,
										   0);
					if(!name)
					{
						return 0;
					}
					--intValue;
					if(intValue > 0)
					{
						name = AppendString(name, ", ");
					}
				}
				return AppendString(name, "}");
			}
			else
			{
				ILFree(name);
				return 0;
			}
		}
		/* Not reached */
	}

	/* Append the buffer to the name and return */
	return AppendString(name, buffer);
}

/*
 * Convert a program item attribute into a string-form name.
 * Returns NULL if the attribute is private or invalid.
 */
static char *AttributeToName(ILAttribute *attr)
{
	ILMethod *method;
	char *name;
	const void *value;
	ILUInt32 len;
	ILSerializeReader *reader;
	ILUInt32 numParams;
	ILUInt32 numExtras;
	int needComma;
	int type, posn;
	ILMember *member;
	const char *memberName;
	int memberNameLen;
	int isUsage;

	/* Get the attribute constructor and validate it */
	method = ILProgramItemToMethod(ILAttributeTypeAsItem(attr));
	if(!method)
	{
		return 0;
	}

	/* Get the attribute name */
	name = ILDupString(ILClass_Name(ILMethod_Owner(method)));
	if(!name)
	{
		ILDocOutOfMemory(0);
	}

	/* If the attribute is private, and not "TODO", then bail out */
	if((ILClass_IsPrivate(ILMethod_Owner(method)) &&
	    !ILClassIsRef(ILMethod_Owner(method))) &&
	   strcmp(name, "TODOAttribute") != 0)
	{
		ILFree(name);
		return 0;
	}

	/* We need special handling for the first parameter of
	   the "AttributeUsage" attribute */
	isUsage = (!strcmp(name, "AttributeUsageAttribute"));

	/* Get the attribute value and prepare to parse it */
	value = ILAttributeGetValue(attr, &len);
	if(!value)
	{
		return 0;
	}
	reader = ILSerializeReaderInit(method, value, len);
	if(!reader)
	{
		ILDocOutOfMemory(0);
	}

	/* Get the attribute arguments */
	numParams = ILTypeNumParams(ILMethod_Signature(method));
	needComma = 0;
	while(numParams > 0)
	{
		if(!needComma)
		{
			name = AppendString(name, "(");
			needComma = 1;
		}
		else
		{
			name = AppendString(name, ", ");
		}
		type = ILSerializeReaderGetParamType(reader);
		if(type == -1)
		{
			ILFree(name);
			return 0;
		}
		name = AppendAttrValue(name, reader, type, isUsage);
		if(!name)
		{
			return 0;
		}
		--numParams;
	}

	/* Get the extra field and property values */
	numExtras = ILSerializeReaderGetNumExtra(reader);
	while(numExtras > 0)
	{
		if(!needComma)
		{
			name = AppendString(name, "(");
			needComma = 1;
		}
		else
		{
			name = AppendString(name, ", ");
		}
		type = ILSerializeReaderGetExtra(reader, &member, &memberName,
										 &memberNameLen);
		if(type == -1)
		{
			ILFree(name);
			return 0;
		}
		name = (char *)ILRealloc(name, strlen(name) + memberNameLen + 2);
		if(!name)
		{
			ILDocOutOfMemory(0);
		}
		posn = strlen(name);
		ILMemCpy(name + posn, memberName, memberNameLen);
		strcpy(name + posn + memberNameLen, "=");
		name = AppendAttrValue(name, reader, type, 0);
		if(!name)
		{
			return 0;
		}
		--numExtras;
	}

	/* Add the closing parenthesis if necessary */
	if(needComma)
	{
		name = AppendString(name, ")");
	}

	/* Cleanup */
	ILSerializeReaderDestroy(reader);

	/* Return the name to the caller */
	return name;
}

/*
 * Print a string with XML quoting and an optional SP / omit arity flag.
 */
static void PrintStringLenWithFlags(const char *str, int len, int flags)
{
	int ch;
	int current = 0;

	while(current < len)
	{
		ch = *str;

		if(ch == '.' && ((flags & DUMP_SP) != 0))
		{
			putc('_', stdout);
		}
		else if(ch == '<')
		{
			fputs("&lt;", stdout);
		}
		else if(ch == '>')
		{
			fputs("&gt;", stdout);
		}
		else if(ch == '&')
		{
			fputs("&amp;", stdout);
		}
		else if(ch == '"')
		{
			fputs("&quot;", stdout);
		}
		else if(ch == '\'')
		{
			fputs("&apos;", stdout);
		}
		else if(ch == '`' && ((flags & DUMP_OMIT_ARITY) != 0))
		{
			/* Skip until the end of the string or a period or space is
			   encountered. */
			++str;
			while((ch = *str) != '\0')
			{
				if((ch == '.') || (ch == ' '))
				{
					--str;
					break;
				}
				++str;
			}
			if(ch == '\0')
			{
				/* We reached the end of the string */
				break;
			}
		}
		else
		{
			putc(ch, stdout);
		}
		++str;
		++current;
	}
}

static void PrintStringWithFlags(const char *str, int flags)
{
	int len;

	if(!str)
	{
		return;
	}
	len = strlen(str);
	PrintStringLenWithFlags(str, len, flags);
}
#define	PrintString(str)	PrintStringWithFlags((str), 0)

#if IL_VERSION_MAJOR > 1
static void PrintWithParams(ILType *withType, ILUInt32 from, ILUInt32 to,
							ILProgramItem *scope, int flags)
{
	ILUInt32 current = from;

	while(current < to)
	{
		if(current == from)
		{
			fputs("&lt;", stdout);
		}
		else
		{
			putc(',', stdout);
		}
		PrintType(ILTypeGetWithParamWithPrefixes(withType, current + 1),
				  (flags & DUMP_CS_STYLE), 0, scope);
		current++;
	}
	if(current > from)
	{
		fputs("&gt;", stdout);
	}
}
#endif /* IL_VERSION_MAJOR > 1 */

/*
 * Inner function for printing a class name
 */
static void PrintClassNameInner(ILClass *classInfo, ILProgramItem *scope,
								int flags, ILType *withType)
{
	if((flags & DUMP_NAME_ONLY) != 0)
	{
		PrintStringWithFlags(ILClass_Name(classInfo), flags);
	}
	else
	{
		ILClass *nestedParent;

		nestedParent = ILClass_NestedParent(classInfo);
		if(nestedParent)
		{
			if((flags & DUMP_CS_STYLE) != 0)
			{
				PrintClassNameInner(nestedParent, scope, flags, withType);
			}
			else
			{
				PrintClassNameInner(nestedParent, scope,
									(flags & ~DUMP_GENERIC_PARAMS), 0);
			}
			putc((((flags & DUMP_SP) != 0) ? '_' : '.'), stdout);
			PrintStringWithFlags(ILClass_Name(classInfo), flags);
		}
		else
		{
			if((flags & DUMP_OMIT_NAMESPACE) == 0)
			{
				const char *nspace;

				nspace = ILClass_Namespace(classInfo);
				if(nspace)
				{
					PrintStringWithFlags(nspace, flags);
					putc((((flags & DUMP_SP) != 0) ? '_' : '.'), stdout);
				}
			}
			PrintStringWithFlags(ILClass_Name(classInfo), flags);
		}
	}
#if IL_VERSION_MAJOR > 1
	if(withType)
	{
		ILUInt32 numGenericPars = ILClassGetNumGenericPars(classInfo);

		flags &= ~DUMP_GENERIC_PARAMS;
		if((flags & DUMP_CS_STYLE) != 0)
		{
			ILClass *nestedParent;
			ILUInt32 numParentGenericPars = 0;

			nestedParent = ILClass_NestedParent(classInfo);
			if(nestedParent)
			{
				numParentGenericPars = ILClassGetNumGenericPars(nestedParent);
			}
			PrintWithParams(withType, numParentGenericPars, numGenericPars, scope, flags);
		}
		else
		{
			PrintWithParams(withType, 0, numGenericPars, scope, flags);
		}
	}
	else if((flags & DUMP_GENERIC_PARAMS) != 0)
	{
		ILUInt32 numGenericPars = ILClassGetNumGenericPars(classInfo);

		flags &= ~DUMP_GENERIC_PARAMS;
		if(numGenericPars > 0)
		{
			if((flags & DUMP_CS_STYLE) != 0)
			{
				ILClass *nestedParent;
				ILUInt32 numParentGenericPars = 0;

				nestedParent = ILClass_NestedParent(classInfo);
				if(nestedParent)
				{
					numParentGenericPars = ILClassGetNumGenericPars(nestedParent);
				}
				_ILDumpGenericParms(stdout, scope, numParentGenericPars, numGenericPars, flags);
			}
			else
			{
				_ILDumpGenericParms(stdout, scope, 0, numGenericPars, flags);
			}
		}
	}
#endif /* IL_VERSION_MAJOR > 1 */
}

#if IL_VERSION_MAJOR > 1
static void PrintWithType(ILType *withType, ILProgramItem *scope, int flags)
{
	ILType *mainType = ILTypeGetWithMain(withType);

	PrintClassNameInner(ILType_ToClass(mainType), scope, flags, withType);
}
#endif /* IL_VERSION_MAJOR > 1 */

/*
 * Print a full class name with an optional SP flag.
 */
static void PrintClassNameWithFlags(ILClass *classInfo, ILProgramItem *scope, int flags)
{
	ILType *synType = ILClass_SynType(classInfo);

	if(synType && ILType_IsWith(synType))
	{
		PrintClassNameInner(ILType_ToClass(ILTypeGetWithMain(synType)), scope, flags, synType);
	}
	else
	{
		PrintClassNameInner(classInfo, scope, flags, 0);
	}
}
#define	PrintClassName(classInfo, scope)	PrintClassNameWithFlags((classInfo), (scope), 0)

/*
 * Print a ProgramItem that can be an ILClass or ILTypeSpec as used for
 * baseclasses, implemented interfaces or parents of members in generic classes.
 */
static void PrintProgramItemWithFlags(ILProgramItem *item, ILProgramItem *scope, int flags)
{
	ILClass *classInfo;
	ILTypeSpec *spec;

	if((classInfo = ILProgramItemToClass(item)) != 0)
	{
		PrintClassNameWithFlags(classInfo, scope, flags);
	}
#if IL_VERSION_MAJOR > 1
	else if((spec = ILProgramItemToTypeSpec(item)) != 0)
	{
		ILType *type;

		type = ILTypeSpec_Type(spec);
		PrintWithType(type, scope, flags);
	}
#endif
}

/*
 * Dump attribute information for a program item.
 */
static void DumpAttributes(ILProgramItem *item)
{
	ILAttribute *attr = ILProgramItemNextAttribute(item, 0);
	char *name;
	if(!attr)
	{
		fputs("<Attributes/>\n", stdout);
		return;
	}
	fputs("<Attributes>\n", stdout);
	do
	{
		name = AttributeToName(attr);
		if(!name)
			continue;
		fputs("<Attribute><AttributeName>", stdout);
		/* fputs(name, stdout); */
		PrintString(name);
		fputs("</AttributeName></Attribute>", stdout);
		ILFree(name);
	}
	while((attr = ILProgramItemNextAttribute(item, attr)) != 0);
	fputs("</Attributes>\n", stdout);
}

/*
 * Dump base class information.
 */
static void DumpBases(ILClass *classInfo, ILProgramItem *base,
					  ILImplements *impl, const char *extends,
					  const char *extendsNoBase,
					  const char *implements,
					  const char *implementsSeparator,
					  int flags)
{
	if(base)
	{
		fputs(extends, stdout);
		PrintProgramItemWithFlags(base, ILToProgramItem(classInfo), flags);
		if(!impl)
		{
			return;
		}
		fputs(implements, stdout);
	}
	else
	{
		fputs(extendsNoBase, stdout);
	}
	while(impl &&
		  !ILClass_IsPublic(ILImplements_UnderlyingInterfaceClass(impl)))
	{
		impl = ILClassNextImplements(classInfo, impl);
	}
	if(!impl)
	{
		return;
	}
	PrintProgramItemWithFlags(ILImplements_Interface(impl),
							  ILToProgramItem(classInfo), flags);
	while((impl = ILClassNextImplements(classInfo, impl)) != 0)
	{
		if(!ILClass_IsPublic(ILImplements_UnderlyingInterfaceClass(impl)))
		{
			continue;
		}
		fputs(implementsSeparator, stdout);
		PrintProgramItemWithFlags(ILImplements_Interface(impl),
								  ILToProgramItem(classInfo), flags);
	}
}

/*
 * Determine if a class member will be visible outside its assembly.
 */
static int MemberVisible(ILMember *member)
{
	ILUInt32 attrs;
	ILMethod *method;
	if(ILMember_IsMethod(member) || ILMember_IsField(member))
	{
		attrs = ILMember_Attrs(member);
	}
	else if(ILMember_IsProperty(member))
	{
		method = ILProperty_Getter((ILProperty *)member);
		if(!method)
		{
			method = ILProperty_Setter((ILProperty *)member);
		}
		if(!method)
		{
			return 0;
		}
		attrs = ILMethod_Attrs(method);
	}
	else if(ILMember_IsEvent(member))
	{
		method = ILEvent_AddOn((ILEvent *)member);
		if(!method)
		{
			method = ILEvent_RemoveOn((ILEvent *)member);
		}
		if(!method)
		{
			return 0;
		}
		attrs = ILMethod_Attrs(method);
	}
	else
	{
		return 0;
	}
	switch(attrs & IL_META_METHODDEF_MEMBER_ACCESS_MASK)
	{
		case IL_META_METHODDEF_FAMILY:
		case IL_META_METHODDEF_FAM_OR_ASSEM:
		case IL_META_METHODDEF_PUBLIC:
			return 1;
	}
	return 0;
}

/*
 * Print a type.
 */
static void PrintType(ILType *type, int csForm, ILParameter *parameter,
					  ILProgramItem *scope)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:
			{
				if(csForm)
					fputs("void", stdout);
				else
					fputs("System.Void", stdout);
			}
			break;

			case IL_META_ELEMTYPE_BOOLEAN:
			{
				if(csForm)
					fputs("bool", stdout);
				else
					fputs("System.Boolean", stdout);
			}
			break;

			case IL_META_ELEMTYPE_I1:
			{
				if(csForm)
					fputs("sbyte", stdout);
				else
					fputs("System.SByte", stdout);
			}
			break;

			case IL_META_ELEMTYPE_U1:
			{
				if(csForm)
					fputs("byte", stdout);
				else
					fputs("System.Byte", stdout);
			}
			break;

			case IL_META_ELEMTYPE_I2:
			{
				if(csForm)
					fputs("short", stdout);
				else
					fputs("System.Int16", stdout);
			}
			break;

			case IL_META_ELEMTYPE_U2:
			{
				if(csForm)
					fputs("ushort", stdout);
				else
					fputs("System.UInt16", stdout);
			}
			break;

			case IL_META_ELEMTYPE_CHAR:
			{
				if(csForm)
					fputs("char", stdout);
				else
					fputs("System.Char", stdout);
			}
			break;

			case IL_META_ELEMTYPE_I4:
			{
				if(csForm)
					fputs("int", stdout);
				else
					fputs("System.Int32", stdout);
			}
			break;

			case IL_META_ELEMTYPE_U4:
			{
				if(csForm)
					fputs("uint", stdout);
				else
					fputs("System.UInt32", stdout);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			{
				if(csForm)
					fputs("long", stdout);
				else
					fputs("System.Int64", stdout);
			}
			break;

			case IL_META_ELEMTYPE_U8:
			{
				if(csForm)
					fputs("ulong", stdout);
				else
					fputs("System.UInt64", stdout);
			}
			break;

			case IL_META_ELEMTYPE_I:
			{
				if(csForm)
					fputs("IntPtr", stdout);
				else
					fputs("System.IntPtr", stdout);
			}
			break;

			case IL_META_ELEMTYPE_U:
			{
				if(csForm)
					fputs("UIntPtr", stdout);
				else
					fputs("System.UIntPtr", stdout);
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				if(csForm)
					fputs("float", stdout);
				else
					fputs("System.Single", stdout);
			}
			break;

			case IL_META_ELEMTYPE_R8:
			case IL_META_ELEMTYPE_R:
			{
				if(csForm)
					fputs("double", stdout);
				else
					fputs("System.Double", stdout);
			}
			break;

			case IL_META_ELEMTYPE_TYPEDBYREF:
			{
				if(csForm)
					fputs("TypedReference", stdout);
				else
					fputs("System.TypedReference", stdout);
			}
			break;
		}
	}
	else if(ILType_IsClass(type) || ILType_IsValueType(type))
	{
		PrintClassNameWithFlags(ILType_ToClass(type), scope,
								csForm ? DUMP_STYLE_CSHARP : 0);
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_BYREF:
			{
				if(csForm)
				{
					if(parameter && ILParameter_IsOut(parameter))
					{
						fputs("out ", stdout);
					}
					else
					{
						fputs("ref ", stdout);
					}
					PrintType(ILType_Ref(type), csForm, 0, scope);
				}
				else
				{
					PrintType(ILType_Ref(type), csForm, 0, scope);
					fputs("&amp;", stdout);
				}
			}
			break;

			case IL_TYPE_COMPLEX_PTR:
			{
				PrintType(ILType_Ref(type), csForm, 0, scope);
				if(csForm)
				{
					putc(' ', stdout);
				}
				putc('*', stdout);
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY:
			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				unsigned long rank = ILTypeGetRank(type);
				if(csForm && parameter)
				{
					/* Look for the "ParamArrayAttribute" marker */
					ILAttribute *attr = 0;
					ILMethod *ctor;
					while((attr = ILProgramItemNextAttribute
								(ILToProgramItem(parameter), attr)) != 0)
					{
						ctor = ILProgramItemToMethod
							(ILAttribute_TypeAsItem(attr));
						if(ctor && !strcmp(ILClass_Name(ILMethod_Owner(ctor)),
										   "ParamArrayAttribute"))
						{
							fputs("params ", stdout);
						}
					}
				}
				PrintType(ILTypeGetElemType(type), csForm, 0, scope);
				putc('[', stdout);
				while(rank > 1)
				{
					putc(',', stdout);
					--rank;
				}
				putc(']', stdout);
			}
			break;

		#if IL_VERSION_MAJOR > 1
			case IL_TYPE_COMPLEX_WITH:
			{
				PrintWithType(type, scope, csForm ? DUMP_STYLE_CSHARP : DUMP_STYLE_ILASM);
			}
			break;

			case IL_TYPE_COMPLEX_MVAR:
			{
				if(!csForm)
				{
					ILUInt32 paramNum = ILType_VarNum(type);

					fprintf(stdout, "!!%i", paramNum);
				}
				else
				{
					ILMethod *method = ILProgramItemToMethod(scope);

					if(method)
					{
						ILUInt32 paramNum = ILType_VarNum(type);
						ILGenericPar *genPar;

						genPar = ILGenericParGetFromOwner(scope, paramNum);
						if(genPar)
						{
							fputs(ILGenericParGetName(genPar), stdout);
						}
					}
				}
			}
			break;

			case IL_TYPE_COMPLEX_VAR:
			{
				if(!csForm)
				{
					ILUInt32 paramNum = ILType_VarNum(type);

					fprintf(stdout, "!%i", paramNum);
				}
				else
				{
					ILMethod *method = ILProgramItemToMethod(scope);

					if(method)
					{
						ILUInt32 paramNum = ILType_VarNum(type);
						ILGenericPar *genPar;

						genPar = ILGenericParGetFromOwner
									(ILToProgramItem(ILMethod_Owner(method)),
									 paramNum);
						if(genPar)
						{
							fputs(ILGenericParGetName(genPar), stdout);
						}
					}
					else
					{
						ILClass *classInfo = ILProgramItemToClass(scope);

						if(classInfo)
						{
							ILUInt32 paramNum = ILType_VarNum(type);
							ILGenericPar *genPar;

							genPar = ILGenericParGetFromOwner(scope, paramNum);
							if(genPar)
							{
								fputs(ILGenericParGetName(genPar), stdout);
							}
						}
					}
				}
			}
			break;
		#endif /* IL_VERSION_MAJOR > 1 */
		}
	}
}

/*
 * Print parameter information for a method.
 */
static void PrintParams(ILMethod *method, ILType *signature, int csForm)
{
	unsigned long numParams;
	unsigned long param;
	ILParameter *parameter;
	if(csForm)
	{
		putc('(', stdout);
	}
	else
	{
		fputs("<Parameters>\n", stdout);
	}
	numParams = ILTypeNumParams(signature);
	for(param = 1; param <= numParams; ++param)
	{
		if(param > 1 && csForm)
		{
			fputs(", ", stdout);
		}
		parameter = ILMethodNextParam(method, 0);
		while(parameter != 0 && ILParameter_Num(parameter) != param)
		{
			parameter = ILMethodNextParam(method, parameter);
		}
		if(csForm)
		{
			PrintType(ILTypeGetParam(signature, param), 1, parameter,
					  ILToProgramItem(method));
			putc(' ', stdout);
			if(parameter && ILParameter_Name(parameter))
			{
				PrintString(ILParameter_Name(parameter));
			}
			else
			{
				printf("_p%ld", param);
			}
		}
		else
		{
			fputs("<Parameter Name=\"", stdout);
			if(parameter && ILParameter_Name(parameter))
			{
				PrintString(ILParameter_Name(parameter));
			}
			else
			{
				printf("_p%ld", param);
			}
			fputs("\" Type=\"", stdout);
			PrintType(ILTypeGetParam(signature, param), 1, parameter,
					  ILToProgramItem(method));
			fputs("\"/>\n", stdout);
		}
	}
	if(csForm)
	{
		putc(')', stdout);
	}
	else
	{
		fputs("</Parameters>\n", stdout);
	}
}

/*
 * Print parameter information for an indexer.
 */
static void PrintIndexerParams(ILMethod *method, ILType *signature,
							   int isSetter, int csForm)
{
	unsigned long numParams;
	unsigned long param;
	ILParameter *parameter;
	if(csForm)
	{
		putc('[', stdout);
	}
	else
	{
		fputs("<Parameters>\n", stdout);
	}
	numParams = ILTypeNumParams(signature);
	if(isSetter)
	{
		--numParams;
	}
	for(param = 1; param <= numParams; ++param)
	{
		if(param > 1 && csForm)
		{
			fputs(", ", stdout);
		}
		parameter = ILMethodNextParam(method, 0);
		while(parameter != 0 && ILParameter_Num(parameter) != param)
		{
			parameter = ILMethodNextParam(method, parameter);
		}
		if(csForm)
		{
			PrintType(ILTypeGetParam(signature, param), csForm, parameter,
					  ILToProgramItem(method));
			putc(' ', stdout);
			if(parameter && ILParameter_Name(parameter))
			{
				PrintString(ILParameter_Name(parameter));
			}
			else
			{
				printf("_p%ld", param);
			}
		}
		else
		{
			fputs("<Parameter Name=\"", stdout);
			if(parameter && ILParameter_Name(parameter))
			{
				PrintString(ILParameter_Name(parameter));
			}
			else
			{
				printf("_p%ld", param);
			}
			fputs("\" Type=\"", stdout);
			PrintType(ILTypeGetParam(signature, param), csForm, parameter,
					  ILToProgramItem(method));
			fputs("\"/>\n", stdout);
		}
	}
	if(csForm)
	{
		putc(']', stdout);
	}
	else
	{
		fputs("</Parameters>\n", stdout);
	}
}

/*
 * Dump information about a constructor.
 */
static void DumpConstructor(ILClass *classInfo, ILMethod *method)
{
	/* Dump the name */
	fputs("<Member MemberName=\"", stdout);
	PrintString(ILMethod_Name(method));
#if IL_VERSION_MAJOR > 1
	_ILDumpGenericParms(stdout, ILToProgramItem(method),
						0, ILType_NumGen(ILMethod_Signature(method)),
						DUMP_STYLE_CSHARP_METHOD);
#endif /* IL_VERSION_MAJOR > 1 */
	fputs("\">\n", stdout);

	/* Dump the signature information */
	fputs("<MemberSignature Language=\"ILASM\" Value=\"", stdout);
	ILDumpFlags(stdout, ILMethod_Attrs(method), ILMethodDefinitionFlags, 0);
#if IL_VERSION_MAJOR > 1
	_ILDumpMethodType(stdout, ILProgramItem_Image(method),
					 ILMethod_Signature(method), IL_DUMP_GENERIC_PARAMS, 0,
					 ILMethod_Name(method), method);
#else /* IL_VERSION_MAJOR == 1 */
	_ILDumpMethodType(stdout, ILProgramItem_Image(method),
					 ILMethod_Signature(method), 0, 0,
					 ILMethod_Name(method), method);
#endif /* IL_VERSION_MAJOR == 1 */
	fputs("\"/>\n", stdout);
	fputs("<MemberSignature Language=\"C#\" Value=\"", stdout);
	ILDumpFlags(stdout, ILMethod_Attrs(method), CSharpMethodFlags, 0);
	PrintStringWithFlags(ILClass_Name(classInfo), DUMP_OMIT_ARITY);
	PrintParams(method, ILMethod_Signature(method), 1);
	fputs(";\"/>\n", stdout);

	/* Dump the member type */
	fputs("<MemberType>Constructor</MemberType>\n", stdout);

	/* Dump the return type and parameters in XML */
	fputs("<ReturnValue/>\n", stdout);
	PrintParams(method, ILMethod_Signature(method), 0);

	/* Dump the attributes */
	DumpAttributes(ILToProgramItem(method));

	/* Dump the member footer */
	fputs("</Member>\n", stdout);
}

/*
 * Determine if a method is a finalizer.
 */
static int IsFinalizer(ILMethod *method)
{
	ILType *signature;
	if(strcmp(ILMethod_Name(method), "Finalize") != 0)
	{
		return 0;
	}
	if(!ILMethod_IsFamily(method) || !ILMethod_IsVirtual(method))
	{
		return 0;
	}
	signature = ILMethod_Signature(method);
	if(ILTypeGetReturn(signature) != ILType_Void)
	{
		return 0;
	}
	if(ILTypeNumParams(signature) != 0)
	{
		return 0;
	}
	return 1;
}

/*
 * Dump information about a method.
 */
static void DumpMethod(ILMethod *method)
{
	/* Dump the name */
	fputs("<Member MemberName=\"", stdout);
	PrintString(ILMethod_Name(method));
#if IL_VERSION_MAJOR > 1
	_ILDumpGenericParms(stdout, ILToProgramItem(method),
						0, ILType_NumGen(ILMethod_Signature(method)),
						DUMP_STYLE_CSHARP_METHOD);
#endif /* IL_VERSION_MAJOR > 1 */
	fputs("\">\n", stdout);

	/* Dump the signature information */
	fputs("<MemberSignature Language=\"ILASM\" Value=\".method ", stdout);
	ILDumpFlags(stdout, ILMethod_Attrs(method), ILMethodDefinitionFlags, 0);
#if IL_VERSION_MAJOR > 1
	_ILDumpMethodType(stdout, ILProgramItem_Image(method),
					 ILMethod_Signature(method), IL_DUMP_GENERIC_PARAMS, 0,
					 ILMethod_Name(method), method);
#else /* IL_VERSION_MAJOR == 1 */
	_ILDumpMethodType(stdout, ILProgramItem_Image(method),
					 ILMethod_Signature(method), 0, 0,
					 ILMethod_Name(method), method);
#endif /* IL_VERSION_MAJOR == 1 */
	fputs("\"/>\n", stdout);
	if(IsFinalizer(method))
	{
		fputs("<MemberSignature Language=\"C#\" Value=\"~", stdout);
		PrintStringWithFlags(ILClass_Name(ILMethod_Owner(method)), DUMP_OMIT_ARITY);
		fputs("()", stdout);
	}
	else
	{
		fputs("<MemberSignature Language=\"C#\" Value=\"", stdout);
		if(!ILClass_IsInterface(ILMethod_Owner(method)))
		{
			ILDumpFlags(stdout, ILMethod_Attrs(method), CSharpMethodFlags, 0);
		}
		PrintType(ILTypeGetReturn(ILMethod_Signature(method)), 1, 0,
				  ILToProgramItem(method));
		putc(' ', stdout);
		PrintString(ILMethod_Name(method));
	#if IL_VERSION_MAJOR > 1
		_ILDumpGenericParms(stdout, ILToProgramItem(method),
							0, ILType_NumGen(ILMethod_Signature(method)),
							DUMP_STYLE_CSHARP_METHOD);
	#endif /* IL_VERSION_MAJOR > 1 */
		PrintParams(method, ILMethod_Signature(method), 1);
	#if IL_VERSION_MAJOR > 1
		_ILDumpGenericConstraints(stdout, ILToProgramItem(method),
								  0, ILType_NumGen(ILMethod_Signature(method)),
								  DUMP_STYLE_CSHARP);
	#endif /* IL_VERSION_MAJOR > 1 */
	}
	fputs(";\"/>\n", stdout);

	/* Dump the member type */
	fputs("<MemberType>Method</MemberType>\n", stdout);

	/* Dump the return type and parameters in XML */
	fputs("<ReturnValue><ReturnType>", stdout);
	PrintType(ILTypeGetReturn(ILMethod_Signature(method)), 1, 0,
			  ILToProgramItem(method));
	fputs("</ReturnType></ReturnValue>\n", stdout);
	PrintParams(method, ILMethod_Signature(method), 0);

	/* Dump the attributes */
	DumpAttributes(ILToProgramItem(method));

	/* Dump the member footer */
	fputs("</Member>\n", stdout);
}

/*
 * Dump information about a field.
 */
static void DumpField(ILField *field)
{
	/* Dump the name */
	fputs("<Member MemberName=\"", stdout);
	PrintString(ILField_Name(field));
	fputs("\">\n", stdout);

	/* Dump the signature information */
	fputs("<MemberSignature Language=\"ILASM\" Value=\".field ", stdout);
	ILDumpFlags(stdout, ILField_Attrs(field), ILFieldDefinitionFlags, 0);
	_ILDumpType(stdout, ILProgramItem_Image(field), ILField_Type(field), 0);
	putc(' ', stdout);
	PrintString(ILField_Name(field));
	/* TODO: constant value */
	fputs("\"/>\n", stdout);
	fputs("<MemberSignature Language=\"C#\" Value=\"", stdout);
	if(ILField_IsLiteral(field))
	{
		ILDumpFlags(stdout, ILField_Attrs(field) &
						~(IL_META_FIELDDEF_STATIC |
				 		  IL_META_FIELDDEF_INIT_ONLY),
					CSharpFieldFlags, 0);
	}
	else
	{
		ILDumpFlags(stdout, ILField_Attrs(field), CSharpFieldFlags, 0);
	}
	PrintType(ILField_Type(field), 1, 0,
			  ILToProgramItem(ILField_Owner(field)));
	putc(' ', stdout);
	PrintString(ILField_Name(field));
	/* TODO: constant value */
	fputs(";\"/>\n", stdout);

	/* Dump the field type */
	fputs("<ReturnValue><ReturnType>", stdout);
	PrintType(ILField_Type(field), 1, 0,
			  ILToProgramItem(ILField_Owner(field)));
	fputs("</ReturnType></ReturnValue>\n", stdout);

	/* Dump the member type */
	fputs("<MemberType>Field</MemberType>\n", stdout);

	/* Dump the attributes */
	DumpAttributes(ILToProgramItem(field));

	/* Dump the member footer */
	fputs("</Member>\n", stdout);
}

/*
 * Dump information about a property.
 */
static void DumpProperty(ILProperty *property)
{
	ILMethod *getter;
	ILMethod *setter;
	ILMethod *either;
	ILType *type;
	int isIndexer;

	/* Extract interesting information from the property */
	getter = ILProperty_Getter(property);
	setter = ILProperty_Setter(property);
	if(!getter && !setter)
	{
		return;
	}
	if(getter)
	{
		either = getter;
		type = ILTypeGetReturn(ILMethod_Signature(getter));
		isIndexer = (ILTypeNumParams(ILMethod_Signature(getter)) > 0);
	}
	else
	{
		either = setter;
		type = ILTypeGetParam(ILMethod_Signature(setter),
							  ILTypeNumParams(ILMethod_Signature(setter)));
		isIndexer = (ILTypeNumParams(ILMethod_Signature(setter)) > 1);
	}

	/* Dump the name */
	fputs("<Member MemberName=\"", stdout);
	PrintString(ILProperty_Name(property));
	fputs("\">\n", stdout);

	/* Dump the signature information */
	fputs("<MemberSignature Language=\"ILASM\" Value=\".property ", stdout);
	_ILDumpType(stdout, ILProgramItem_Image(property), type, 0);
	putc(' ', stdout);
	PrintString(ILProperty_Name(property));
	fputs(" {", stdout);
	if(getter)
	{
		putc(' ', stdout);
		ILDumpFlags(stdout, ILMethod_Attrs(getter),
					ILMethodDefinitionFlags, 0);
	#if IL_VERSION_MAJOR > 1
		_ILDumpMethodType(stdout, ILProgramItem_Image(getter),
					     ILMethod_Signature(getter), IL_DUMP_GENERIC_PARAMS, 0,
					     ILMethod_Name(getter), getter);
	#else /* IL_VERSION_MAJOR == 1 */
		_ILDumpMethodType(stdout, ILProgramItem_Image(getter),
					     ILMethod_Signature(getter), 0, 0,
					     ILMethod_Name(getter), getter);
	#endif /* IL_VERSION_MAJOR == 1 */
	}
	if(setter)
	{
		putc(' ', stdout);
		ILDumpFlags(stdout, ILMethod_Attrs(setter),
					ILMethodDefinitionFlags, 0);
	#if IL_VERSION_MAJOR > 1
		_ILDumpMethodType(stdout, ILProgramItem_Image(setter),
					     ILMethod_Signature(setter), IL_DUMP_GENERIC_PARAMS, 0,
					     ILMethod_Name(setter), setter);
	#else /* IL_VERSION_MAJOR == 1 */
		_ILDumpMethodType(stdout, ILProgramItem_Image(setter),
					     ILMethod_Signature(setter), 0, 0,
					     ILMethod_Name(setter), setter);
	#endif /* IL_VERSION_MAJOR == 1 */
	}
	fputs(" }\"/>\n", stdout);
	fputs("<MemberSignature Language=\"C#\" Value=\"", stdout);
	if(!ILClass_IsInterface(ILProperty_Owner(property)))
	{
		ILDumpFlags(stdout, ILMethod_Attrs(either), CSharpMethodFlags, 0);
	}
	PrintType(type, 1, 0, ILToProgramItem(ILProperty_Owner(property)));
	putc(' ', stdout);
	if(isIndexer)
	{
		fputs("this", stdout);
		PrintIndexerParams(either, ILMethod_Signature(either),
						   (either == setter), 1);
	}
	else
	{
		PrintString(ILProperty_Name(property));
	}
	fputs(" { ", stdout);
	if(getter)
	{
		fputs("get; ", stdout);
	}
	if(setter)
	{
		fputs("set; ", stdout);
	}
	fputs("}\"/>\n", stdout);

	/* Dump the member type */
	fputs("<MemberType>Property</MemberType>\n", stdout);

	/* Dump the property type and indexer parameters */
	fputs("<ReturnValue><ReturnType>", stdout);
	PrintType(type, 1, 0, ILToProgramItem(ILProperty_Owner(property)));
	fputs("</ReturnType></ReturnValue>\n", stdout);
	if(isIndexer)
	{
		PrintIndexerParams(either, ILMethod_Signature(either),
						   (either == setter), 0);
	}

	/* Dump the attributes */
	DumpAttributes(ILToProgramItem(property));

	/* Dump the member footer */
	fputs("</Member>\n", stdout);
}

/*
 * Dump information about an event.
 */
static void DumpEvent(ILEvent *event)
{
	ILMethod *method;

	/* Get the method with the attribute information */
	method = ILEvent_AddOn(event);
	if(!method)
	{
		method = ILEvent_RemoveOn(event);
		if(!method)
		{
			return;
		}
	}

	/* Dump the name */
	fputs("<Member MemberName=\"", stdout);
	PrintString(ILEvent_Name(event));
	fputs("\">\n", stdout);

	/* Dump the signature information */
	fputs("<MemberSignature Language=\"ILASM\" Value=\".event ", stdout);
	ILDumpFlags(stdout, ILMethod_Attrs(method), ILMethodDefinitionFlags, 0);
	fputs("event ", stdout);
	PrintString(ILEvent_Name(event));
	fputs("\"/>\n", stdout);
	fputs("<MemberSignature Language=\"C#\" Value=\"", stdout);
	if(!ILClass_IsInterface(ILEvent_Owner(event)))
	{
		ILDumpFlags(stdout, ILMethod_Attrs(method), CSharpMethodFlags, 0);
	}
	PrintType(ILEvent_Type(event), 1, 0, ILToProgramItem(ILEvent_Owner(event)));
	putc(' ', stdout);
	PrintString(ILEvent_Name(event));
	fputs("\"/>\n", stdout);

	/* Dump the member type */
	fputs("<MemberType>Event</MemberType>\n", stdout);

	/* Dump the attributes */
	DumpAttributes(ILToProgramItem(event));

	/* Dump the member footer */
	fputs("</Member>\n", stdout);
}

/*
 * Determine if a class inherits from a specific "System" class.
 */
static int InheritsFrom(ILClass *classInfo, const char *name)
{
	classInfo = ILClass_UnderlyingParentClass(classInfo);
	while(classInfo != 0)
	{
		if(!strcmp(ILClass_Name(classInfo), name) &&
		   ILClass_Namespace(classInfo) &&
		   !strcmp(ILClass_Namespace(classInfo), "System"))
		{
			return 1;
		}
		classInfo = ILClass_UnderlyingParentClass(classInfo);
	}
	return 0;
}

/*
 * Determine if a method is a non-property special.  i.e. an operator,
 * add, or remove method.
 */
static int NonPropertySpecial(const char *name)
{
	return (!strncmp(name, "op_", 3) ||
	   		!strncmp(name, "add_", 4) ||
	   		!strncmp(name, "remove_", 7));
}

/*
 * Dump information about a class.
 */
static void DumpClass(ILClass *classInfo)
{
	ILAssembly *assem;
	ILProgramItem *baseClass;
	ILImplements *impl;
	ILMember *member;
	int isDelegate;
	ILMethod *delegateMethod;
	ILType *delegateSignature;
	ILNestedInfo *nestedInfo;

	/* Print the header information */
	fputs("<Type Name=\"", stdout);
	PrintClassNameWithFlags(classInfo, ILToProgramItem(classInfo), DUMP_STYLE_NAME);
	fputs("\" FullName=\"", stdout);
	PrintClassNameWithFlags(classInfo, ILToProgramItem(classInfo), DUMP_STYLE_FULLNAME);
	fputs("\" FullNameSP=\"", stdout);
	PrintClassNameWithFlags(classInfo, ILToProgramItem(classInfo), DUMP_STYLE_FULLNAMESP);
	fputs("\">\n", stdout);

	/* Need to do something different if this is a delegate */
	isDelegate = (ILTypeIsDelegate(ILClassToType(classInfo)));

	/* Output the signature information */
	fputs("<TypeSignature Language=\"ILASM\" Value=\".class ", stdout);
	ILDumpFlags(stdout, ILClass_Attrs(classInfo), ILTypeDefinitionFlags, 0);
	baseClass = ILClass_Parent(classInfo);
	impl = ILClassNextImplements(classInfo, 0);
	PrintClassNameWithFlags(classInfo, ILToProgramItem(classInfo), DUMP_STYLE_ILASM_CLASS);
	if(baseClass || impl)
	{
		DumpBases(classInfo, baseClass, impl, " extends ", " implements ",
				  " implements ", ", ", DUMP_STYLE_ILASM);
	}
	fputs("\"/>\n", stdout);
	fputs("<TypeSignature Language=\"C#\" Value=\"", stdout);
	if(isDelegate)
	{
		ILDumpFlags(stdout, ILClass_Attrs(classInfo) &
							~IL_META_TYPEDEF_SEALED, CSharpTypeFlags, 0);
		fputs("delegate ", stdout);
		delegateMethod = ILTypeGetDelegateMethod(ILClassToType(classInfo));
		delegateSignature = ILMethod_Signature(delegateMethod);
		PrintType(ILTypeGetReturn(delegateSignature), 1, 0, ILToProgramItem(classInfo));
		putc(' ', stdout);
		PrintClassNameWithFlags(classInfo, ILToProgramItem(classInfo), DUMP_STYLE_CSHARP_CLASS);
		PrintParams(delegateMethod, delegateSignature, 1);
	#if IL_VERSION_MAJOR > 1
		_ILDumpGenericClassConstraints(stdout, classInfo, DUMP_STYLE_CSHARP);
	#endif /* IL_VERSION_MAJOR > 1 */
		putc(';', stdout);
	}
	else
	{
		if(ILClass_IsInterface(classInfo))
		{
			ILDumpFlags(stdout, ILClass_Attrs(classInfo) &
								~IL_META_TYPEDEF_ABSTRACT, CSharpTypeFlags, 0);
			fputs("interface ", stdout);
		}
		else if(InheritsFrom(classInfo, "Enum"))
		{
			ILDumpFlags(stdout, ILClass_Attrs(classInfo) &
								~IL_META_TYPEDEF_SEALED, CSharpTypeFlags, 0);
			fputs("enum ", stdout);
			baseClass = 0;
		}
		else if(InheritsFrom(classInfo, "ValueType") &&
		        !ILClass_IsAbstract(classInfo))
		{
			ILDumpFlags(stdout, ILClass_Attrs(classInfo) &
								~IL_META_TYPEDEF_SEALED, CSharpTypeFlags, 0);
			fputs("struct ", stdout);
			baseClass = 0;
		}
		else
		{
			ILDumpFlags(stdout, ILClass_Attrs(classInfo), CSharpTypeFlags, 0);
			fputs("class ", stdout);
		}
		PrintClassNameWithFlags(classInfo, ILToProgramItem(classInfo), DUMP_STYLE_CSHARP_CLASS);
		if(baseClass || impl)
		{
			DumpBases(classInfo, baseClass, impl, " : ", " : ", ", ", ", ", DUMP_STYLE_CSHARP);
		}
	#if IL_VERSION_MAJOR > 1
		_ILDumpGenericClassConstraints(stdout, classInfo, DUMP_STYLE_CSHARP);
	#endif /* IL_VERSION_MAJOR > 1 */
	}
	fputs("\"/>\n", stdout);

	/* Output the assembly information */
	assem = ILAssembly_FromToken(ILProgramItem_Image(classInfo),
								 IL_META_TOKEN_ASSEMBLY | 1);
	if(assem)
	{
		fputs("<AssemblyInfo><AssemblyName>", stdout);
		PrintString(ILAssembly_Name(assem));
		fputs("</AssemblyName></AssemblyInfo>\n", stdout);
	}

	/* Output the base type and interfaces */
	baseClass = ILClass_Parent(classInfo);
	if(baseClass)
	{
		fputs("<Base><BaseTypeName>", stdout);
		PrintProgramItemWithFlags(baseClass, ILToProgramItem(classInfo), DUMP_STYLE_CSHARP);
		fputs("</BaseTypeName></Base>\n", stdout);
	}
	else
	{
		fputs("<Base/>\n", stdout);
	}
	impl = ILClassNextImplements(classInfo, 0);
	if(impl)
	{
		fputs("<Interfaces>\n", stdout);
		do
		{
			if(!ILClass_IsPublic(ILImplements_UnderlyingInterfaceClass(impl)))
			{
				continue;
			}
			fputs("<Interface><InterfaceName>", stdout);
			PrintProgramItemWithFlags(ILImplements_Interface(impl),
									  ILToProgramItem(classInfo),
									  DUMP_STYLE_CSHARP);
			fputs("</InterfaceName></Interface>\n", stdout);
		}
		while((impl = ILClassNextImplements(classInfo, impl)) != 0);
		fputs("</Interfaces>\n", stdout);
	}
	else
	{
		fputs("<Interfaces/>\n", stdout);
	}

	/* Output the attributes */
	DumpAttributes(ILToProgramItem(classInfo));

	/* Output the members */
	member = 0;
	fputs("<Members>\n", stdout);
	while((member = ILClassNextMember(classInfo, member)) != 0)
	{
		/* Skip members that are not visible outside the assembly */
		if(!MemberVisible(member))
		{
			continue;
		}
		if(ILMember_IsMethod(member))
		{
			if(ILMethodIsConstructor((ILMethod *)member))
			{
				DumpConstructor(classInfo, (ILMethod *)member);
			}
			else if(!ILMethod_HasSpecialName((ILMethod *)member) ||
					NonPropertySpecial(ILMember_Name(member)))
			{
				DumpMethod((ILMethod *)member);
			}
		}
		else if(ILMember_IsField(member))
		{
			DumpField((ILField *)member);
		}
		else if(ILMember_IsProperty(member))
		{
			DumpProperty((ILProperty *)member);
		}
		else if(ILMember_IsEvent(member))
		{
			DumpEvent((ILEvent *)member);
		}
	}
	fputs("</Members>\n", stdout);

	/* Print the footer information */
	fputs("</Type>\n", stdout);

	/* Output the public nested classes, if any */
	nestedInfo = 0;
	while((nestedInfo = ILClassNextNested(classInfo, nestedInfo)) != 0)
	{
		ILClass *nestedClass = ILNestedInfoGetChild(nestedInfo);

		if(nestedClass && NestedClassIsAccessible(nestedClass))
		{
			DumpClass(nestedClass);
		}
	}
}

/*
 * Load an IL image and dump its contents in XML.
 */
static int dump(const char *filename, ILContext *context)
{
	ILImage *image;
	ILClass *classInfo;

	/* Attempt to load the image into memory */
	if(ILImageLoadFromFile(filename, context, &image,
					  	   IL_LOADFLAG_FORCE_32BIT |
					  	   IL_LOADFLAG_NO_RESOLVE, 1) != 0)
	{
		return 1;
	}

	/* Dump the top-level public classes */
	classInfo = 0;
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, (void *)classInfo)) != 0)
	{
		if(ILClass_IsPublic(classInfo))
		{
			DumpClass(classInfo);
		}
	}

	/* Clean up and exit */
	/* We can't destroy only the image if we want to dump more than one
	   assembly. We'd have to destroy the whole context and recreate it. */
	/*
	ILImageDestroy(image);
	*/
	return 0;
}

#ifdef	__cplusplus
};
#endif
