/*
 * csdoc.c - C# documentation extraction utility.
 *
 * Copyright (C) 2001, 2008  Southern Storm Software, Pty Ltd.
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

This program parses C# source code and outputs an XML document that
describes all classes defined in the program, and their properties.
This XML document must then be processed by some other tool to get
the final documentation in the preferred format (HTML, Texinfo, etc).

The command-line syntax is the same as "cscc", with the following
special flags:

	-flibrary-name=NAME		Specify the library name for the types.
	-fprivate				Dump private definitions as well.

*/

#include "csharp/cs_internal.h"
#include "il_dumpasm.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Entry points for the parser and lexer.
 */
extern int cs_debug;
extern int cs_parse(void);
extern void cs_restart(FILE *infile);

/*
 * Forward declarations.
 */
static void GenerateDocs(ILNode *tree, FILE *stream);

/*
 * Configuration variables that are used by "cc_main.c".
 */
char const CCPluginName[] = "csdoc";
int const CCPluginOptionParseMode = CMDLINE_PARSE_CSCC;
int const CCPluginUsesPreproc = CC_PREPROC_CSHARP;
int const CCPluginJVMSupported = 0;
int const CCPluginSkipCodeGen = 1;
int const CCPluginGenModulesEarly = 0;
int const CCPluginForceStdlib = 0;

int CCPluginInit(void)
{
	CSMetadataOnly = 1;
	/* Nothing to do here */
	return 1;
}

void CCPluginShutdown(int status)
{
	/* Nothing to do here */
}

int CCPluginParse(void)
{
	/*cs_debug = 1;*/
	return cs_parse();
}

void CCPluginRestart(FILE *infile)
{
	CCPreProcessorStream.docComments = 1;
	cs_restart(infile);
}

void CCPluginSemAnalysis(void)
{
	/* Perform type gathering */
	CCCodeGen.typeGather = -1;
	CCParseTree = CSTypeGather(&CCCodeGen, CCCodeGen.globalScope, CCParseTree);
	CCCodeGen.typeGather = 0;

	/* Perform semantic analysis */
	ILNode_SemAnalysis(CCParseTree, &CCCodeGen, &CCParseTree);
}

void CCPluginPostCodeGen(void)
{
	FILE *outfile;

	/* Generate the XML-ized documentation */
	if(output_filename && strcmp(output_filename, "-") != 0)
	{
		outfile = fopen(output_filename, "w");
		if(outfile == NULL)
		{
			perror(output_filename);
			CCHaveErrors = 1;
			return;
		}
		GenerateDocs(CCParseTree, outfile);
		fclose(outfile);
	}
	else
	{
		GenerateDocs(CCParseTree, stdout);
	}
}

int main(int argc, char *argv[])
{
	return CCMain(argc, argv);
}

/*
 * Indent a line by a specific amount.
 */
static void Indent(FILE *stream, int indent)
{
	static char const spaces[] = "                ";
	while(indent >= 16)
	{
		fwrite(spaces, 1, 16, stream);
		indent -= 16;
	}
	if(indent > 0)
	{
		fwrite(spaces, 1, indent, stream);
	}
}

/*
 * Dump a string, with XML quoting.
 */
static void DumpString(const char *str, FILE *stream)
{
	int ch;
	if(!str)
	{
		return;
	}
	while((ch = *str++) != '\0')
	{
		if(ch == '<')
		{
			fputs("&lt;", stream);
		}
		else if(ch == '>')
		{
			fputs("&gt;", stream);
		}
		else if(ch == '&')
		{
			fputs("&amp;", stream);
		}
		else if(ch == '"')
		{
			fputs("&quot;", stream);
		}
		else if(ch == '\'')
		{
			fputs("&apos;", stream);
		}
		else
		{
			putc(ch, stream);
		}
	}
}

/*
 * Dump a length-delimited string, with XML quoting.
 */
static void DumpStringN(const char *str, int len, FILE *stream)
{
	int ch;
	if(!str)
	{
		return;
	}
	while(len-- > 0 && (ch = *str++) != '\0')
	{
		if(ch == '<')
		{
			fputs("&lt;", stream);
		}
		else if(ch == '>')
		{
			fputs("&gt;", stream);
		}
		else if(ch == '&')
		{
			fputs("&amp;", stream);
		}
		else if(ch == '"')
		{
			fputs("&quot;", stream);
		}
		else if(ch == '\'')
		{
			fputs("&apos;", stream);
		}
		else
		{
			putc(ch, stream);
		}
	}
}

/*
 * Dump a class name to a stream.
 */
static void DumpClassName(FILE *stream, ILClass *classInfo)
{
	const char *name = ILClass_Name(classInfo);
	const char *namespace = ILClass_Namespace(classInfo);
	if(namespace)
	{
		DumpString(namespace, stream);
		putc('.', stream);
	}
	DumpString(name, stream);
}

/*
 * Dump a class name to a stream, but omit the namespace if
 * it is the same as another class.
 */
static void DumpClassNameOther(FILE *stream, ILClass *classInfo, ILClass *other)
{
	const char *name = ILClass_Name(classInfo);
	const char *namespace = ILClass_Namespace(classInfo);
	const char *namespace2 = (other ? ILClass_Namespace(other) : 0);
	if(namespace)
	{
		if(!namespace2 || strcmp(namespace, namespace2) != 0)
		{
			DumpString(namespace, stream);
			putc('.', stream);
		}
	}
	DumpString(name, stream);
}

/*
 * Dump the SP form of a class name to a stream.
 */
static void DumpClassNameSP(FILE *stream, ILClass *classInfo)
{
	const char *name = ILClass_Name(classInfo);
	const char *namespace = ILClass_Namespace(classInfo);
	if(namespace)
	{
		while(*namespace != '\0')
		{
			if(*namespace != '.')
			{
				putc(*namespace, stream);
				namespace++;
			}
			else
			{
				putc('_', stream);
				namespace++;
			}
		}
		putc('_', stream);
	}
	DumpString(name, stream);
}

/*
 * Dump the documentation comments for an item.
 */
static void DumpDocComments(FILE *stream, ILNode *attrs, int indent)
{
	ILNode_ListIter iter;
	ILNode *node;
	const char *str;
	int len;
	int sawComment = 0;
	if(attrs && yyisa(attrs, ILNode_AttributeTree))
	{
		attrs = ((ILNode_AttributeTree *)attrs)->sections;
	}
	ILNode_ListIter_Init(&iter, attrs);
	while((node = ILNode_ListIter_Next(&iter)) != 0)
	{
		if(yyisa(node, ILNode_DocComment))
		{
			if(!sawComment)
			{
				Indent(stream, indent);
				fputs("<Docs>", stream);
				sawComment = 1;
			}
			else
			{
				putc('\n', stream);
			}
			str = ((ILNode_DocComment *)node)->str;
			len = ((ILNode_DocComment *)node)->len;
			if(len > 0 && str[0] == ' ')
			{
				/* Strip a leading space, to normalize the line */
				++str;
				--len;
			}
			if(len > 0 && str[len - 1] == '\n')
			{
				/* Strip the newline */
				--len;
			}
			if(len > 0)
			{
				fwrite(str, 1, len, stream);
			}
		}
	}
	if(sawComment)
	{
		fputs("</Docs>\n", stream);
	}
	else
	{
		Indent(stream, indent);
		fputs("<Docs/>\n", stream);
	}
}

/*
 * Dump a specific attribute.
 */
static void DumpAttribute(FILE *stream, ILAttribute *attr,
						  ILClass *owner, int indent)
{
	ILMethod *ctor;
	ILType *signature;

	/* Convert the attribute's type into a constructor reference */
	ctor = ILProgramItemToMethod(ILAttributeTypeAsItem(attr));
	if(!ctor)
	{
		return;
	}
	signature = ILMethod_Signature(ctor);

	/* Output the attribute header */
	Indent(stream, indent);
	fputs("<Attribute>\n", stream);

	/* Output the attribute name start */
	Indent(stream, indent + 2);
	fputs("<AttributeName>", stream);
	DumpClassNameOther(stream, ILMethod_Owner(ctor), owner);

	/* Output the parameters */
	if(ILTypeNumParams(signature) != 0)
	{
		/* TODO: output the constant values for the parameters */
		putc('(', stream);
		putc('?', stream);
		putc(')', stream);
	}

	/* Output the attribute name end */
	fputs("</AttributeName>\n", stream);

	/* Output the attribute footer */
	Indent(stream, indent + 2);
	fputs("<Excluded>0</Excluded>\n", stream);
	Indent(stream, indent);
	fputs("</Attribute>\n", stream);
}

/*
 * Dump the attributes for an item.
 */
static void DumpAttributes(FILE *stream, ILProgramItem *item, int indent)
{
	ILAttribute *attr = ILProgramItemNextAttribute(item, 0);
	ILClass *owner;
	ILMember *member;
	if(attr)
	{
		owner = ILProgramItemToClass(item);
		if(!owner)
		{
			member = ILProgramItemToMember(item);
			if(member)
			{
				owner = ILMember_Owner(member);
			}
			else
			{
				owner = 0;
			}
		}
		Indent(stream, indent);
		fputs("<Attributes>\n", stream);
		do
		{
			DumpAttribute(stream, attr, owner, indent + 2);
		}
		while((attr = ILProgramItemNextAttribute(item, attr)) != 0);
		Indent(stream, indent);
		fputs("</Attributes>\n", stream);
	}
	else
	{
		Indent(stream, indent);
		fputs("<Attributes/>\n", stream);
	}
}

/*
 * C# modifier flags for types.
 */
static ILFlagInfo const CSharpTypeFlags[] = {
	{"new", CS_SPECIALATTR_NEW, 0},
	{"unsafe", CS_SPECIALATTR_UNSAFE, 0},
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
	{"new", CS_SPECIALATTR_NEW, 0},
	{"unsafe", CS_SPECIALATTR_UNSAFE, 0},
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
	{"new", CS_SPECIALATTR_NEW, 0},
	{"unsafe", CS_SPECIALATTR_UNSAFE, 0},
	{"extern", CS_SPECIALATTR_EXTERN, 0},
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
	{"final", IL_META_METHODDEF_FINAL, 0},
	{"virtual", IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT,
				IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT},
	{"override", IL_META_METHODDEF_VIRTUAL,
				 IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT},
	{"abstract", IL_META_METHODDEF_ABSTRACT, 0},
	{0, 0, 0},
};

/*
 * Determine if a member of a class is private.
 */
static int IsMemberPrivate(ILUInt32 modifiers)
{
	modifiers &= IL_META_FIELDDEF_FIELD_ACCESS_MASK;
	return (modifiers == IL_META_FIELDDEF_PRIVATE ||
			modifiers == IL_META_FIELDDEF_ASSEMBLY ||
			modifiers == IL_META_FIELDDEF_FAM_AND_ASSEM);
}

/*
 * Generate documentation for a specific field definition.
 */
static void GenerateDocsForField(FILE *stream, ILNode_FieldDeclaration *decl,
								 int indent)
{
	ILNode_ListIter iterator;
	ILNode *fieldDecl;
	ILField *field;

	/* Bail out if the field is private, and "-fprivate" is not supplied */
	if(IsMemberPrivate(decl->modifiers))
	{
		if(!CCStringListContains(extension_flags, num_extension_flags,
								 "private"))
		{
			return;
		}
	}

	/* Scan all field declarators that are attached to the field definition */
	ILNode_ListIter_Init(&iterator, decl->fieldDeclarators);
	while((fieldDecl = ILNode_ListIter_Next(&iterator)) != 0)
	{
		/* Get the field descriptor */
		field = ((ILNode_FieldDeclarator *)fieldDecl)->fieldInfo;
		if(!field)
		{
			continue;
		}

		/* Output the member header */
		Indent(stream, indent);
		fputs("<Member MemberName=\"", stream);
		DumpString(ILField_Name(field), stream);
		fputs("\">\n", stream);

		/* Output the signature in ILASM form */
		Indent(stream, indent + 2);
		fputs("<MemberSignature Language=\"ILASM\" Value=\".field ", stream);
		ILDumpFlags(stream, ILField_Attrs(field), ILFieldDefinitionFlags, 0);
		ILDumpType(stream, ILProgramItem_Image(field),
				   ILField_Type(field), IL_DUMP_XML_QUOTING);
		putc(' ', stream);
		DumpString(ILField_Name(field), stream);
		fputs("\"/>\n", stream);

		/* Output the signature in C# form */
		Indent(stream, indent + 2);
		fputs("<MemberSignature Language=\"C#\" Value=\"", stream);
		ILDumpFlags(stream, decl->modifiers, CSharpFieldFlags, 0);
		DumpString(CSTypeToName(ILField_Type(field)), stream);
		putc(' ', stream);
		DumpString(ILField_Name(field), stream);
		if(ILField_IsLiteral(field))
		{
			/* TODO: dump the constant value */
		}
		fputs("\"/>\n", stream);

		/* Output the member kind */
		Indent(stream, indent + 2);
		fputs("<MemberType>Field</MemberType>\n", stream);

		/* Dump the attributes for the field */
		DumpAttributes(stream, ILToProgramItem(field), indent + 2);

		/* Output the field's type */
		Indent(stream, indent + 2);
		fputs("<ReturnValue>\n", stream);
		Indent(stream, indent + 4);
		fputs("<ReturnType>", stream);
		DumpString(CSTypeToName(ILField_Type(field)), stream);
		fputs("</ReturnType>\n", stream);
		Indent(stream, indent + 2);
		fputs("</ReturnValue>\n", stream);

		/* Fields don't have parameters */
		Indent(stream, indent + 2);
		fputs("<Parameters/>\n", stream);

		/* Dump the field's constant value if necessary */
		/* TODO: MemberValue */

		/* Dump the doc comments for the field */
		DumpDocComments(stream, decl->attributes, indent + 2);

		/* Output the member footer */
		Indent(stream, indent + 2);
		fputs("<Excluded>0</Excluded>\n", stream);
		Indent(stream, indent);
		fputs("</Member>\n", stream);
	}
}

/*
 * Generate documentation for a specific enumerated member definition.
 */
static void GenerateDocsForEnum(FILE *stream,
								ILNode_EnumMemberDeclaration *decl,
							    int indent)
{
	ILField *field;

	/* Get the field descriptor */
	field = decl->fieldInfo;
	if(!field)
	{
		return;
	}

	/* Output the member header */
	Indent(stream, indent);
	fputs("<Member MemberName=\"", stream);
	DumpString(ILField_Name(field), stream);
	fputs("\">\n", stream);

	/* Output the signature in ILASM form */
	Indent(stream, indent + 2);
	fputs("<MemberSignature Language=\"ILASM\" Value=\".field ", stream);
	ILDumpFlags(stream, ILField_Attrs(field), ILFieldDefinitionFlags, 0);
	ILDumpType(stream, ILProgramItem_Image(field),
			   ILField_Type(field), IL_DUMP_XML_QUOTING);
	putc(' ', stream);
	DumpString(ILField_Name(field), stream);
	fputs("\"/>\n", stream);

	/* Output the signature in C# form */
	Indent(stream, indent + 2);
	fputs("<MemberSignature Language=\"C#\" Value=\"", stream);
	DumpString(ILField_Name(field), stream);
	if(ILField_IsLiteral(field))
	{
		/* TODO */
	}
	fputs("\"/>\n", stream);

	/* Output the member kind */
	Indent(stream, indent + 2);
	fputs("<MemberType>Field</MemberType>\n", stream);

	/* Dump the attributes for the enumerated member */
	DumpAttributes(stream, ILToProgramItem(field), indent + 2);

	/* Output the field's type */
	Indent(stream, indent + 2);
	fputs("<ReturnValue>\n", stream);
	Indent(stream, indent + 4);
	fputs("<ReturnType>", stream);
	DumpString(CSTypeToName(ILField_Type(field)), stream);
	fputs("</ReturnType>\n", stream);
	Indent(stream, indent + 2);
	fputs("</ReturnValue>\n", stream);

	/* Fields don't have parameters */
	Indent(stream, indent + 2);
	fputs("<Parameters/>\n", stream);

	/* Dump the field's constant value if necessary */
	/* TODO: MemberValue */

	/* Dump the doc comments for the field */
	DumpDocComments(stream, decl->attributes, indent + 2);

	/* Output the member footer */
	Indent(stream, indent + 2);
	fputs("<Excluded>0</Excluded>\n", stream);
	Indent(stream, indent);
	fputs("</Member>\n", stream);
}

/*
 * Find the parameter information block for a particular method parameter.
 */
static ILParameter *FindMethodParam(ILMethod *method, ILUInt32 param)
{
	ILParameter *paramInfo = 0;
	while((paramInfo = ILMethodNextParam(method, paramInfo)) != 0)
	{
		if(ILParameter_Num(paramInfo) == param)
		{
			return paramInfo;
		}
	}
	return 0;
}

/*
 * Convert operator names back into their C# form.
 */
static const char *ConvertOperatorNames(ILMethod *method)
{
	const char *name = ILMethod_Name(method);
	if(!ILMethod_HasSpecialName(method) || strncmp(name, "op_", 3) != 0)
	{
		return name;
	}
	if(!strcmp(name, "op_Addition") || !strcmp(name, "op_UnaryPlus"))
	{
		return "operator +";
	}
	if(!strcmp(name, "op_Subtraction") || !strcmp(name, "op_UnaryNegation"))
	{
		return "operator -";
	}
	if(!strcmp(name, "op_LogicalNot"))
	{
		return "operator !";
	}
	if(!strcmp(name, "op_OnesComplement"))
	{
		return "operator ~";
	}
	if(!strcmp(name, "op_Increment"))
	{
		return "operator ++";
	}
	if(!strcmp(name, "op_Decrement"))
	{
		return "operator --";
	}
	if(!strcmp(name, "op_True"))
	{
		return "operator true";
	}
	if(!strcmp(name, "op_False"))
	{
		return "operator false";
	}
	if(!strcmp(name, "op_Multiply"))
	{
		return "operator *";
	}
	if(!strcmp(name, "op_Division"))
	{
		return "operator /";
	}
	if(!strcmp(name, "op_Modulus"))
	{
		return "operator %";
	}
	if(!strcmp(name, "op_BitwiseAnd"))
	{
		return "operator &amp;";
	}
	if(!strcmp(name, "op_BitwiseOr"))
	{
		return "operator |";
	}
	if(!strcmp(name, "op_ExclusiveOr"))
	{
		return "operator ^";
	}
	if(!strcmp(name, "op_LeftShift"))
	{
		return "operator &lt;&lt;";
	}
	if(!strcmp(name, "op_RightShift"))
	{
		return "operator &gt;&gt;";
	}
	if(!strcmp(name, "op_Equality"))
	{
		return "operator ==";
	}
	if(!strcmp(name, "op_Inequality"))
	{
		return "operator !=";
	}
	if(!strcmp(name, "op_GreaterThan"))
	{
		return "operator &gt;";
	}
	if(!strcmp(name, "op_LessThan"))
	{
		return "operator &lt;";
	}
	if(!strcmp(name, "op_GreaterThanOrEqual"))
	{
		return "operator &gt;=";
	}
	if(!strcmp(name, "op_LessThanOrEqual"))
	{
		return "operator &lt;=";
	}
	if(!strcmp(name, "op_Implicit"))
	{
		return "implicit operator";
	}
	if(!strcmp(name, "op_Explicit"))
	{
		return "explicit operator";
	}
	return name;
}

/*
 * Generate documentation for a specific method definition.
 */
static void GenerateDocsForMethod(FILE *stream, ILNode_MethodDeclaration *decl,
								  int indent)
{
	ILMethod *method = decl->methodInfo;
	ILType *signature;
	ILType *returnType;
	ILUInt32 num, param;
	ILParameter *paramInfo;
	int isConstructor;
	const char *methodName;

	/* Bail out if the definition was not fully built */
	if(!method)
	{
		return;
	}

	/* Bail out if the method is private, and "-fprivate" is not supplied */
	if(IsMemberPrivate(decl->modifiers))
	{
		if(!CCStringListContains(extension_flags, num_extension_flags,
								 "private"))
		{
			return;
		}
	}

	/* Output the member header */
	Indent(stream, indent);
	fputs("<Member MemberName=\"", stream);
	DumpString(ILMethod_Name(method), stream);
	fputs("\">\n", stream);

	/* Is this method a constructor? */
	isConstructor = (!strcmp(ILMethod_Name(method), ".ctor") ||
	   				 !strcmp(ILMethod_Name(method), ".cctor"));

	/* Output the signature in ILASM form */
	Indent(stream, indent + 2);
	fputs("<MemberSignature Language=\"ILASM\" Value=\"", stream);
	if(!isConstructor)
	{
		fputs(".method ", stream);
	}
	ILDumpFlags(stream, ILMethod_Attrs(method), ILMethodDefinitionFlags, 0);
	signature = ILMethod_Signature(method);
	ILDumpMethodType(stream, ILProgramItem_Image(method), signature,
			         IL_DUMP_XML_QUOTING, 0, ILMethod_Name(method), method);
	putc(' ', stream);
	ILDumpFlags(stream, ILMethod_ImplAttrs(method),
				ILMethodImplementationFlags, 0);
	fputs("\"/>\n", stream);

	/* Output the signature in C# form */
	Indent(stream, indent + 2);
	fputs("<MemberSignature Language=\"C#\" Value=\"", stream);
	ILDumpFlags(stream, decl->modifiers, CSharpMethodFlags, 0);
	if(!isConstructor)
	{
		DumpString(CSTypeToName(ILTypeGetReturn(signature)), stream);
		putc(' ', stream);
	}
	if(isConstructor)
	{
		methodName = ILClass_Name(ILMethod_Owner(method));
	}
	else
	{
		methodName = ConvertOperatorNames(method);
	}
	DumpString(methodName, stream);
	putc('(', stream);
	num = ILTypeNumParams(signature);
	for(param = 1; param <= num; ++param)
	{
		if(param != 1)
		{
			fputs(", ", stream);
		}
		DumpString(CSTypeToName(ILTypeGetParam(signature, param)), stream);
		paramInfo = FindMethodParam(method, param);
		if(paramInfo)
		{
			putc(' ', stream);
			DumpString(ILParameter_Name(paramInfo), stream);
		}
	}
	putc(')', stream);
	fputs(";\"/>\n", stream);

	/* Output the member kind */
	Indent(stream, indent + 2);
	if(isConstructor)
	{
		fputs("<MemberType>Constructor</MemberType>\n", stream);
	}
	else
	{
		fputs("<MemberType>Method</MemberType>\n", stream);
	}

	/* Dump the attributes for the method */
	DumpAttributes(stream, ILToProgramItem(method), indent + 2);

	/* Output the method's type */
	Indent(stream, indent + 2);
	returnType = ILTypeGetReturn(signature);
	if(returnType != ILType_Void)
	{
		fputs("<ReturnValue>\n", stream);
		Indent(stream, indent + 4);
		fputs("<ReturnType>", stream);
		DumpString(CSTypeToName(returnType), stream);
		fputs("</ReturnType>\n", stream);
		Indent(stream, indent + 2);
		fputs("</ReturnValue>\n", stream);
	}
	else
	{
		fputs("<ReturnValue/>\n", stream);
	}

	/* Dump the method parameters */
	Indent(stream, indent + 2);
	if(num != 0)
	{
		fputs("<Parameters>\n", stream);
		for(param = 1; param <= num; ++param)
		{
			Indent(stream, indent + 4);
			fputs("<Parameter ", stream);
			paramInfo = FindMethodParam(method, param);
			if(paramInfo)
			{
				fputs("Name=\"", stream);
				DumpString(ILParameter_Name(paramInfo), stream);
				fputs("\" ", stream);
			}
			fputs("Type=\"", stream);
			DumpString(CSTypeToName(ILTypeGetParam(signature, param)), stream);
			fputs("\"/>\n", stream);
		}
		Indent(stream, indent + 2);
		fputs("</Parameters>\n", stream);
	}
	else
	{
		fputs("<Parameters/>\n", stream);
	}

	/* Dump the doc comments for the method */
	/* TODO: get comments from parent classes for virtual override's */
	DumpDocComments(stream, decl->attributes, indent + 2);

	/* Output the member footer */
	Indent(stream, indent + 2);
	fputs("<Excluded>0</Excluded>\n", stream);
	Indent(stream, indent);
	fputs("</Member>\n", stream);
}

/*
 * Generate documentation for a specific property definition.
 */
static void GenerateDocsForProperty(FILE *stream,
									ILNode_PropertyDeclaration *decl,
								    int indent)
{
	ILProperty *property = decl->propertyInfo;
	ILType *signature;
	ILType *returnType;
	ILUInt32 num, param;
	ILParameter *paramInfo;
	int isIndexer;
	ILMethod *getter;
	ILMethod *setter;
	ILType *methodSig;
	const char *propertyName;

	/* Bail out if the definition was not fully built */
	if(!property)
	{
		return;
	}

	/* Bail out if the property is private, and "-fprivate" is not supplied */
	if(IsMemberPrivate(decl->modifiers))
	{
		if(!CCStringListContains(extension_flags, num_extension_flags,
								 "private"))
		{
			return;
		}
	}

	/* Output the member header */
	Indent(stream, indent);
	fputs("<Member MemberName=\"", stream);
	DumpString(ILProperty_Name(property), stream);
	fputs("\">\n", stream);

	/* Is this property an indexer? */
	signature = ILProperty_Signature(property);
	isIndexer = (ILTypeNumParams(signature) != 0);

	/* Find the getter and setter methods */
	getter = ILProperty_Getter(property);
	setter = ILProperty_Setter(property);

	/* Output the signature in ILASM form */
	Indent(stream, indent + 2);
	fputs("<MemberSignature Language=\"ILASM\" Value=\"", stream);
	fputs(".property ", stream);
	ILDumpFlags(stream, ILProperty_Attrs(property),
				ILPropertyDefinitionFlags, 0);
	ILDumpMethodType(stream, ILProgramItem_Image(property), signature,
			         IL_DUMP_XML_QUOTING, 0, ILProperty_Name(property), getter);
	fputs(" { ", stream);
	if(getter)
	{
		fputs(".get ", stream);
		ILDumpFlags(stream, ILMethod_Attrs(getter), ILMethodDefinitionFlags, 0);
		methodSig = ILMethod_Signature(getter);
		ILDumpMethodType(stream, ILProgramItem_Image(getter), signature,
				         IL_DUMP_XML_QUOTING, 0, ILMethod_Name(getter), getter);
		putc(' ', stream);
		ILDumpFlags(stream, ILMethod_ImplAttrs(getter),
					ILMethodImplementationFlags, 0);
	}
	if(setter)
	{
		fputs(".set ", stream);
		ILDumpFlags(stream, ILMethod_Attrs(setter), ILMethodDefinitionFlags, 0);
		methodSig = ILMethod_Signature(setter);
		ILDumpMethodType(stream, ILProgramItem_Image(setter), signature,
				         IL_DUMP_XML_QUOTING, 0, ILMethod_Name(setter), setter);
		putc(' ', stream);
		ILDumpFlags(stream, ILMethod_ImplAttrs(setter),
					ILMethodImplementationFlags, 0);
	}
	fputs("}\"/>\n", stream);

	/* Output the signature in C# form */
	Indent(stream, indent + 2);
	fputs("<MemberSignature Language=\"C#\" Value=\"", stream);
	ILDumpFlags(stream, decl->modifiers, CSharpMethodFlags, 0);
	DumpString(CSTypeToName(ILTypeGetReturn(signature)), stream);
	putc(' ', stream);
	propertyName = ILProperty_Name(property);
	if(isIndexer)
	{
		/* Convert the property name back into a name involving "this" */
		if(!strcmp(propertyName, "Item"))
		{
			fputs("this", stream);
		}
		else
		{
			int len = strlen(propertyName);
			if(len > 5 && !strcmp(propertyName + len - 5, ".Item"))
			{
				DumpStringN(propertyName, len - 4, stream);
				fputs("this", stream);
			}
			else
			{
				DumpString(propertyName, stream);
			}
		}
	}
	else
	{
		DumpString(propertyName, stream);
	}
	num = ILTypeNumParams(signature);
	if(isIndexer)
	{
		putc('[', stream);
		for(param = 1; param <= num; ++param)
		{
			if(param != 1)
			{
				fputs(", ", stream);
			}
			DumpString(CSTypeToName(ILTypeGetParam(signature, param)), stream);
			if(getter || setter)
			{
				paramInfo = FindMethodParam((getter ? getter : setter), param);
				if(paramInfo)
				{
					putc(' ', stream);
					DumpString(ILParameter_Name(paramInfo), stream);
				}
			}
		}
		putc(']', stream);
	}
	fputs(" { ", stream);
	if(getter)
	{
		fputs("get; ", stream);
	}
	if(setter)
	{
		fputs("set; ", stream);
	}
	fputs("}\"/>\n", stream);

	/* Output the member kind */
	Indent(stream, indent + 2);
	fputs("<MemberType>Property</MemberType>\n", stream);

	/* Dump the attributes for the property */
	DumpAttributes(stream, ILToProgramItem(property), indent + 2);

	/* Output the property's type */
	Indent(stream, indent + 2);
	returnType = ILTypeGetReturn(signature);
	if(returnType != ILType_Void)
	{
		fputs("<ReturnValue>\n", stream);
		Indent(stream, indent + 4);
		fputs("<ReturnType>", stream);
		DumpString(CSTypeToName(returnType), stream);
		fputs("</ReturnType>\n", stream);
		Indent(stream, indent + 2);
		fputs("</ReturnValue>\n", stream);
	}
	else
	{
		fputs("<ReturnValue/>\n", stream);
	}

	/* Dump the property parameters */
	Indent(stream, indent + 2);
	if(num != 0)
	{
		fputs("<Parameters>\n", stream);
		for(param = 1; param <= num; ++param)
		{
			Indent(stream, indent + 4);
			fputs("<Parameter ", stream);
			if(getter || setter)
			{
				paramInfo = FindMethodParam((getter ? getter : setter), param);
				if(paramInfo)
				{
					fputs("Name=\"", stream);
					DumpString(ILParameter_Name(paramInfo), stream);
					fputs("\" ", stream);
				}
			}
			fputs("Type=\"", stream);
			DumpString(CSTypeToName(ILTypeGetParam(signature, param)), stream);
			fputs("\"/>\n", stream);
		}
		Indent(stream, indent + 2);
		fputs("</Parameters>\n", stream);
	}
	else
	{
		fputs("<Parameters/>\n", stream);
	}

	/* Dump the doc comments for the property */
	/* TODO: get comments from parent classes for virtual override's */
	DumpDocComments(stream, decl->attributes, indent + 2);

	/* Output the member footer */
	Indent(stream, indent + 2);
	fputs("<Excluded>0</Excluded>\n", stream);
	Indent(stream, indent);
	fputs("</Member>\n", stream);
}

/*
 * Generate documentation for a specific event definition.
 */
static void GenerateDocsForEvent(FILE *stream, ILNode_EventDeclaration *decl,
								 int indent)
{
	ILNode_ListIter iterator;
	ILNode *eventDecl;
	ILEvent *event;

	/* Bail out if the field is private, and "-fprivate" is not supplied */
	if(IsMemberPrivate(decl->modifiers))
	{
		if(!CCStringListContains(extension_flags, num_extension_flags,
								 "private"))
		{
			return;
		}
	}

	/* Scan all event declarators that are attached to the event definition */
	ILNode_ListIter_Init(&iterator, decl->eventDeclarators);
	while((eventDecl = ILNode_ListIter_Next(&iterator)) != 0)
	{
		/* Get the event descriptor */
		event = ((ILNode_EventDeclarator *)eventDecl)->eventInfo;
		if(!event)
		{
			continue;
		}

		/* Output the member header */
		Indent(stream, indent);
		fputs("<Member MemberName=\"", stream);
		DumpString(ILEvent_Name(event), stream);
		fputs("\">\n", stream);

		/* Output the signature in ILASM form */
		Indent(stream, indent + 2);
		fputs("<MemberSignature Language=\"ILASM\" Value=\".event ", stream);
		ILDumpFlags(stream, decl->modifiers, ILMethodDefinitionFlags, 0);
		fputs("event ", stream);
		DumpString(ILEvent_Name(event), stream);
		/* TODO: add/remove methods */
		fputs("\"/>\n", stream);

		/* Output the signature in C# form */
		Indent(stream, indent + 2);
		fputs("<MemberSignature Language=\"C#\" Value=\"", stream);
		ILDumpFlags(stream, decl->modifiers, CSharpMethodFlags, 0);
		DumpString(CSTypeToName(ILEvent_Type(event)), stream);
		putc(' ', stream);
		DumpString(ILEvent_Name(event), stream);
		/* TODO: add/remove methods */
		fputs("\"/>\n", stream);

		/* Output the member kind */
		Indent(stream, indent + 2);
		fputs("<MemberType>Event</MemberType>\n", stream);

		/* Dump the attributes for the event */
		DumpAttributes(stream, ILToProgramItem(event), indent + 2);

		/* Events don't have return types or parameters */
		Indent(stream, indent + 2);
		fputs("<ReturnValue/>\n", stream);
		Indent(stream, indent + 2);
		fputs("<Parameters/>\n", stream);

		/* Dump the doc comments for the event */
		DumpDocComments(stream, decl->attributes, indent + 2);

		/* Output the member footer */
		Indent(stream, indent + 2);
		fputs("<Excluded>0</Excluded>\n", stream);
		Indent(stream, indent);
		fputs("</Member>\n", stream);
	}
}

/*
 * Generate documentation for a specific class definition and its members.
 */
static void GenerateDocsForClass(FILE *stream, ILNode_ClassDefn *defn,
							     char *libName, int indent)
{
	ILClass *classInfo = defn->classInfo;
	ILClass *parent;
	ILImplements *impl;
	ILNode_ListIter iterator;
	ILNode *body;
	ILNode *member;
	int isEnum=0;

	/* Bail out if this is a bad class definition */
	if(!classInfo)
	{
		return;
	}

	/* Bail out if the class is private, and "-fprivate" is not supplied */
	if(ILClass_IsPrivate(classInfo) ||
	   ILClass_IsNestedPrivate(classInfo) ||
	   ILClass_IsNestedAssembly(classInfo) ||
	   ILClass_IsNestedFamAndAssem(classInfo))
	{
		if(!CCStringListContains(extension_flags, num_extension_flags,
								 "private"))
		{
			return;
		}
	}

	/* Output the type header */
	Indent(stream, indent);
	fputs("<Type Name=\"", stream);
	DumpString(defn->name, stream);
	fputs("\" FullName=\"", stream);
	DumpClassName(stream, classInfo);
	fputs("\" FullNameSP=\"", stream);
	DumpClassNameSP(stream, classInfo);
	fputs("\">\n", stream);
	indent += 2;

	/* Output the type signature, in ilasm form */
	Indent(stream, indent);
	fputs("<TypeSignature Language=\"ILASM\" Value=\".class ", stream);
	ILDumpFlags(stream, ILClass_Attrs(classInfo), ILTypeDefinitionFlags, 0);
	DumpString(defn->name, stream);
	parent = ILClass_UnderlyingParentClass(classInfo);
	if(parent)
	{
		fputs(" extends ", stream);
		DumpClassName(stream, parent);
	}
	impl = ILClassNextImplements(classInfo, 0);
	if(impl)
	{
		fputs(" implements ", stream);
		DumpClassName(stream, ILImplements_UnderlyingInterfaceClass(impl));
		while((impl = ILClassNextImplements(classInfo, impl)) != 0)
		{
			fputs(", ", stream);
			DumpClassName(stream, ILImplements_UnderlyingInterfaceClass(impl));
		}
	}
	fputs("\"/>\n", stream);

	if(ILClassIsValueType(classInfo))
	{
		if(parent && !strcmp(ILClass_Name(parent), "Enum") &&
		   ILClass_Namespace(parent) != 0 &&
		   !strcmp(ILClass_Namespace(parent), "System"))
		{
			isEnum=1;
		}
	}

	/* Output the type signature, in C# form */
	Indent(stream, indent);
	fputs("<TypeSignature Language=\"C#\" Value=\"", stream);
	if(isEnum)
	{
		ILDumpFlags(stream,defn->modifiers & ~(IL_META_TYPEDEF_SEALED), 
							CSharpTypeFlags,0);
	}
	else
	{
		ILDumpFlags(stream, defn->modifiers, CSharpTypeFlags, 0);
	}
	if(ILClassIsValueType(classInfo))
	{
		if(isEnum)
		{
			fputs("enum ", stream);
		}
		else
		{
			fputs("struct ", stream);
		}
	}
	else if(ILClass_IsInterface(classInfo))
	{
		fputs("interface ", stream);
	}
	else
	{
		fputs("class ", stream);
	}
	DumpString(ILClass_Name(classInfo), stream);
	if(!isEnum && parent && !ILTypeIsObjectClass(ILClassToType(parent)))
	{
		fputs(": ", stream);
		DumpClassNameOther(stream, parent, classInfo);
		impl = ILClassNextImplements(classInfo, 0);
		if(impl)
		{
			fputs(", ", stream);
		}
	}
	else if(!isEnum)
	{
		impl = ILClassNextImplements(classInfo, 0);
		if(impl)
		{
			fputs(": ", stream);
		}
	}
	if(!isEnum && impl)
	{
		DumpClassNameOther(stream,
						   ILImplements_UnderlyingInterfaceClass(impl),
						   classInfo);
		while((impl = ILClassNextImplements(classInfo, impl)) != 0)
		{
			fputs(", ", stream);
			DumpClassNameOther(stream,
							   ILImplements_UnderlyingInterfaceClass(impl),
							   classInfo);
		}
	}
	fputs("\"/>\n", stream);

	/* Dump the library name */
	if(libName)
	{
		Indent(stream, indent);
		fprintf(stream, "<MemberOfPackage>%s</MemberOfPackage>\n", libName);
	}

	/* Dump the raw type definition flags */
	Indent(stream, indent);
	fprintf(stream, "<TypeKind>%lu</TypeKind>\n",
			(unsigned long)(ILClass_Attrs(classInfo)));

	/* Dump the doc comments for the type */
	DumpDocComments(stream, defn->attributes, indent);

	/* Dump the base type name */
	if(parent)
	{
		Indent(stream, indent);
		fputs("<Base>\n", stream);
		Indent(stream, indent + 2);
		fputs("<BaseTypeName>", stream);
		DumpClassName(stream, parent);
		fputs("</BaseTypeName>\n", stream);
		Indent(stream, indent);
		fputs("</Base>\n", stream);
	}
	else
	{
		Indent(stream, indent);
		fputs("<Base/>\n", stream);
	}

	/* Dump the interfaces */
	impl = ILClassNextImplements(classInfo, 0);
	if(impl)
	{
		Indent(stream, indent);
		fputs("<Interfaces>\n", stream);
		do
		{
			Indent(stream, indent + 2);
			fputs("<Interface>\n", stream);
			Indent(stream, indent + 4);
			fputs("<InterfaceName>", stream);
			DumpClassName(stream, ILImplements_UnderlyingInterfaceClass(impl));
			fputs("</InterfaceName>\n", stream);
			Indent(stream, indent + 4);
			fputs("<Excluded>0</Excluded>\n", stream);
			Indent(stream, indent + 2);
			fputs("</Interface>\n", stream);
		}
		while((impl = ILClassNextImplements(classInfo, impl)) != 0);
		Indent(stream, indent);
		fputs("</Interfaces>\n", stream);
	}
	else
	{
		Indent(stream, indent);
		fputs("<Interfaces/>\n", stream);
	}

	/* Dump the attributes for the type */
	DumpAttributes(stream, ILToProgramItem(classInfo), indent);

	/* Dump the class members */
	Indent(stream, indent);
	fputs("<Members>\n", stream);
	indent += 2;
	body = defn->body;
	if(body && yykind(body) == yykindof(ILNode_ScopeChange))
	{
		body = ((ILNode_ScopeChange *)body)->body;
	}
	ILNode_ListIter_Init(&iterator, body);
	while((member = ILNode_ListIter_Next(&iterator)) != 0)
	{
		if(yykind(member) == yykindof(ILNode_FieldDeclaration))
		{
			GenerateDocsForField(stream, (ILNode_FieldDeclaration *)member,
								 indent);
		}
		else if(yykind(member) == yykindof(ILNode_MethodDeclaration))
		{
			GenerateDocsForMethod(stream, (ILNode_MethodDeclaration *)member,
								  indent);
		}
		else if(yykind(member) == yykindof(ILNode_EnumMemberDeclaration))
		{
			GenerateDocsForEnum(stream, (ILNode_EnumMemberDeclaration *)member,
							    indent);
		}
		else if(yykind(member) == yykindof(ILNode_PropertyDeclaration))
		{
			GenerateDocsForProperty(stream,
								    (ILNode_PropertyDeclaration *)member,
								    indent);
		}
		else if(yykind(member) == yykindof(ILNode_EventDeclaration))
		{
			GenerateDocsForEvent(stream,
								 (ILNode_EventDeclaration *)member,
								 indent);
		}
		else if(yykind(member) == yykindof(ILNode_ClassDefn))
		{
			GenerateDocsForClass(stream, (ILNode_ClassDefn *)member,
								 libName, indent);
		}
	}
	indent -= 2;
	Indent(stream, indent);
	fputs("</Members>\n", stream);

	/* Output the type footer */
	Indent(stream, indent);
	fputs("<TypeExcluded>0</TypeExcluded>\n", stream);
	indent -= 2;
	Indent(stream, indent);
	fputs("</Type>\n", stream);
}

/*
 * Generate documentation for a parse tree to a given output stream.
 */
static void GenerateDocs(ILNode *tree, FILE *stream)
{
	ILNode_ListIter iterator;
	ILNode *child;
	char *libName;

	/* Determine the library name from the command-line options */
	libName = CCStringListGetValue(extension_flags, num_extension_flags,
								   "library-name");

	/* Output the library header information */
	fputs("<Libraries><Types", stream);
	if(libName)
	{
		fprintf(stream, " Library=\"%s\"", libName);
	}
	fputs(">\n", stream);

	/* Scan all top-level types */
	ILNode_ListIter_Init(&iterator, tree);
	while((child = ILNode_ListIter_Next(&iterator)) != 0)
	{
		if(yykind(child) == yykindof(ILNode_ClassDefn))
		{
			GenerateDocsForClass(stream, (ILNode_ClassDefn *)child,
								 libName, 2);
		}
	}

	/* Output the library footer information */
	fputs("</Types></Libraries>\n", stream);
}

#ifdef	__cplusplus
};
#endif
