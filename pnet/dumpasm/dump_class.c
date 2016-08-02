/*
 * dump_class.c - Disassemble class information.
 *
 * Copyright (C) 2001, 2008, 2009  Southern Storm Software, Pty Ltd.
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

#include "il_dumpasm.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Dump a PInvoke definition.
 */
static void Dump_PInvoke(FILE *outstream, ILPInvoke *pinvoke, ILMember *member)
{
	if(pinvoke)
	{
		fputs("pinvokeimpl(", outstream);
		ILDumpString(outstream,
					 ILModule_Name(ILPInvoke_Module(pinvoke)));
		putc(' ', outstream);
		if(strcmp(ILPInvoke_Alias(pinvoke), ILMember_Name(member)) != 0)
		{
			fputs("as ", outstream);
			ILDumpString(outstream, ILPInvoke_Alias(pinvoke));
			putc(' ', outstream);
		}
		ILDumpFlags(outstream, ILPInvoke_Attrs(pinvoke),
					ILPInvokeImplementationFlags, 0);
		fputs(") ", outstream);
	}
	else
	{
		fputs("pinvokeimpl() ", outstream);
	}
}

/*
 * Dump custom attributes for generic parameters
 */
static void Dump_GenericParCustomAttrs(ILImage *image, FILE *outstream,
									   int flags, int indent,
									   ILProgramItem *owner)
{
	ILUInt32 numGenericParams = ILGenericParGetNumParams(owner);
	ILUInt32 current = 0;

	for(current = 0; current < numGenericParams; current++)
	{
		ILGenericPar *genPar = ILGenericParGetFromOwner(owner, current);

		if(ILProgramItemNextAttribute(ILToProgramItem(genPar), 0) != 0)
		{
			if(indent == 1)
			{
				fputs("\t", outstream);
			}
			else if(indent == 2)
			{
				fputs("\t\t", outstream);
			}
			fprintf(outstream, ".param type[%X]\n", current + 1);
			ILDAsmDumpCustomAttrs(image, outstream, flags, indent,
								  ILToProgramItem(genPar));
		}
	}
}

/*
 * Dump a method definition.
 */
static void Dump_MethodDef(ILImage *image, FILE *outstream, int flags,
						   ILMethod *method)
{
	ILUInt32 rva;
	ILOverride *over;
	int haveContents;

	/* Skip the method if it is a reference (probably a vararg call site) */
	if((ILMethod_Token(method) & IL_META_TOKEN_MASK) ==
				IL_META_TOKEN_MEMBER_REF)
	{
		return;
	}

	/* Dump the header information for the method */
	fputs("\t.method ", outstream);
	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(outstream, "/*%08lX*/ ",
				(unsigned long)(ILMethod_Token(method)));
	}
	ILDumpFlags(outstream, ILMethod_Attrs(method), ILMethodDefinitionFlags, 0);
	if(ILMethod_HasPInvokeImpl(method))
	{
		Dump_PInvoke(outstream, ILPInvokeFind(method), (ILMember *)method);
	}
	ILDumpMethodType(outstream, image, ILMethod_Signature(method),
					 flags | IL_DUMP_GENERIC_PARAMS,
					 0, ILMethod_Name(method), method);
	putc(' ', outstream);
	ILDumpFlags(outstream, ILMethod_ImplAttrs(method),
				ILMethodImplementationFlags, 0);
	rva = ILMethod_RVA(method);
	haveContents = (ILProgramItem_HasAttrs(method) || rva ||
					ILOverrideFromMethod(method));
	if(haveContents)
	{
		fputs("\n\t{\n", outstream);
	}
	else
	{
		fputs("{}\n", outstream);
	}
	ILDAsmDumpCustomAttrs(image, outstream, flags, 2, ILToProgramItem(method));

	/* Dump the security information, if any */
	if((ILMethod_Attrs(method) & IL_META_METHODDEF_HAS_SECURITY) != 0)
	{
		ILDAsmDumpSecurity(image, outstream, (ILProgramItem *)method, flags);
	}
	
	/* If this a body for an override, then declare it */
	over = ILOverrideFromMethod(method);
	if(over)
	{
		ILMethod *decl = ILOverride_Decl(over);
		fputs("\t\t.override ", outstream);
		ILDumpClassName(outstream, image, ILMethod_Owner(decl), flags);
		fputs("::", outstream);
		ILDumpIdentifier(outstream, ILMethod_Name(decl), 0, flags);
		putc('\n', outstream);
	}

	/* Dump the custom attributes for the generic parameters if present */
	Dump_GenericParCustomAttrs(image, outstream, flags, 2,
							   ILToProgramItem(method));

	/* If we have an RVA, then we need to dump the method's contents */
	if(rva && (flags & ILDASM_NO_IL) == 0)
	{
	#ifdef IL_CONFIG_JAVA
		if(ILMethod_IsJava(method))
		{
			ILDAsmDumpJavaMethod(image, outstream, method, flags);
		}
		else
	#endif
		{
			ILDAsmDumpMethod(image, outstream, method, flags,
							 (ILMethod_Token(method) ==
							 		ILImageGetEntryPoint(image)), 0);
		}
	}

	/* Output the method footer and exit */
	if(haveContents)
	{
		fputs("\t}\n", outstream);
	}
}

/*
 * Dump a field definition.
 */
static void Dump_FieldDef(ILImage *image, FILE *outstream, int flags,
						  ILField *field)
{
	ILFieldLayout *layout;
	fputs("\t.field ", outstream);
	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(outstream, "/*%08lX*/ ",
				(unsigned long)(ILField_Token(field)));
	}
	layout = ILFieldLayoutGetFromOwner(field);
	if(layout)
	{
		fprintf(outstream, "[%lu] ",
				(unsigned long)(ILFieldLayout_Offset(layout)));
	}
	ILDumpFlags(outstream, ILField_Attrs(field), ILFieldDefinitionFlags, 0);
	if((ILField_Attrs(field) & IL_META_FIELDDEF_HAS_FIELD_MARSHAL) != 0)
	{
		ILFieldMarshal *marshal = ILFieldMarshalGetFromOwner
										((ILProgramItem *)field);
		if(marshal)
		{
			const void *type;
			ILUInt32 typeLen;
			type = ILFieldMarshalGetType(marshal, &typeLen);
			if(type)
			{
				fputs("marshal(", outstream);
				ILDumpNativeType(outstream, type, typeLen, flags);
				fputs(") ", outstream);
			}
		}
	}
	if(ILField_HasPInvokeImpl(field))
	{
		Dump_PInvoke(outstream, ILPInvokeFindField(field), (ILMember *)field);
	}
	ILDumpType(outstream, image, ILFieldGetTypeWithPrefixes(field), flags);
	putc(' ', outstream);
	ILDumpIdentifier(outstream, ILField_Name(field), 0, flags);
	if((ILField_Attrs(field) & IL_META_FIELDDEF_HAS_FIELD_RVA) != 0)
	{
		ILFieldRVA *rva = ILFieldRVAGetFromOwner(field);
		if(rva)
		{
			fprintf(outstream, " at D_0x%08lX",
					(unsigned long)(ILFieldRVA_RVA(rva)));
		}
	}
	if((ILField_Attrs(field) & IL_META_FIELDDEF_HAS_DEFAULT) != 0)
	{
		ILDumpConstant(outstream, (ILProgramItem *)field, 0);
	}
	putc('\n', outstream);
	if(ILProgramItem_HasAttrs(field))
	{
		ILDAsmDumpCustomAttrs(image, outstream, flags, 1,
							  ILToProgramItem(field));
	}
}

/*
 * Dump a method association for an event or property.
 */
static void DumpMethodAssociation(ILImage *image, FILE *outstream,
								  int flags, ILMethod *method)
{
	ILDumpMethodType(outstream, image, ILMethod_Signature(method), flags,
					 ILMethod_Owner(method), ILMethod_Name(method), 0);
}

/*
 * Dump an event definition.
 */
static void Dump_EventDef(ILImage *image, FILE *outstream, int flags,
						  ILEvent *event)
{
	ILMethod *method;

	/* Dump the event header */
	fputs("\t.event ", outstream);
	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(outstream, "/*%08lX*/ ",
				(unsigned long)(ILEvent_Token(event)));
	}
	ILDumpFlags(outstream, ILEvent_Attrs(event), ILEventDefinitionFlags, 0);
	if(ILEvent_Type(event) != ILType_Invalid)
	{
		ILDumpType(outstream, image, ILEvent_Type(event), flags);
		putc(' ', outstream);
	}
	ILDumpIdentifier(outstream, ILEvent_Name(event), 0, flags);
	fputs("\n\t{\n", outstream);

	/* Dump the custom attributes */
	if(ILProgramItem_HasAttrs(event))
	{
		ILDAsmDumpCustomAttrs(image, outstream, flags, 1,
							  ILToProgramItem(event));
	}

	/* Dump the event methods */
	if((method = ILEvent_AddOn(event)) != 0)
	{
		fputs("\t\t.addon ", outstream);
		DumpMethodAssociation(image, outstream, flags, method);
		putc('\n', outstream);
	}
	if((method = ILEvent_RemoveOn(event)) != 0)
	{
		fputs("\t\t.removeon ", outstream);
		DumpMethodAssociation(image, outstream, flags, method);
		putc('\n', outstream);
	}
	if((method = ILEvent_Fire(event)) != 0)
	{
		fputs("\t\t.fire ", outstream);
		DumpMethodAssociation(image, outstream, flags, method);
		putc('\n', outstream);
	}
	if((method = ILEvent_Other(event)) != 0)
	{
		fputs("\t\t.other ", outstream);
		DumpMethodAssociation(image, outstream, flags, method);
		putc('\n', outstream);
	}

	/* Dump the event footer */
	fputs("\t}\n", outstream);
}

/*
 * Dump a property definition.
 */
static void Dump_PropertyDef(ILImage *image, FILE *outstream, int flags,
						     ILProperty *property)
{
	ILMethod *method;

	/* Dump the property header */
	fputs("\t.property ", outstream);
	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(outstream, "/*%08lX*/ ",
				(unsigned long)(ILProperty_Token(property)));
	}
	ILDumpFlags(outstream, ILProperty_Attrs(property),
				ILPropertyDefinitionFlags, 0);

	/* Dump the calling conventions from the get/set method,
	   because the property signature doesn't contain them */
	if((method = ILProperty_Getter(property)) != 0)
	{
		ILDumpFlags(outstream, ILMethod_CallConv(method),
					ILMethodCallConvFlags, 0);
	}
	else if((method = ILProperty_Setter(property)) != 0)
	{
		ILDumpFlags(outstream, ILMethod_CallConv(method),
					ILMethodCallConvFlags, 0);
	}

	/* Dump the property type */
	ILDumpMethodType(outstream, image, ILProperty_Signature(property), flags,
					 0, ILProperty_Name(property), 0);
	if((ILProperty_Attrs(property) & IL_META_PROPDEF_HAS_DEFAULT) != 0)
	{
		ILDumpConstant(outstream, (ILProgramItem *)property, 0);
	}
	fputs("\n\t{\n", outstream);

	/* Dump the custom attributes */
	if(ILProgramItem_HasAttrs(property))
	{
		ILDAsmDumpCustomAttrs(image, outstream, flags, 1,
							  ILToProgramItem(property));
	}

	/* Dump the property methods */
	if((method = ILProperty_Getter(property)) != 0)
	{
		fputs("\t\t.get ", outstream);
		DumpMethodAssociation(image, outstream, flags, method);
		putc('\n', outstream);
	}
	if((method = ILProperty_Setter(property)) != 0)
	{
		fputs("\t\t.set ", outstream);
		DumpMethodAssociation(image, outstream, flags, method);
		putc('\n', outstream);
	}
	if((method = ILProperty_Other(property)) != 0)
	{
		fputs("\t\t.other ", outstream);
		DumpMethodAssociation(image, outstream, flags, method);
		putc('\n', outstream);
	}

	/* Dump the event footer */
	fputs("\t}\n", outstream);
}

/*
 * Dump a class name with generic parameter information.
 */
static void DumpClassName(FILE *outstream, ILImage *image,
						  ILClass *info, int flags, int withNamespace)
{
	ILType *type;

	/* Use a different approach if the class is a type specification */
	type = ILClassGetSynType(info);
	if(type)
	{
		ILDumpType(outstream, image, type, flags);
		return;
	}

	/* Dump the main part of the class name */
	if(withNamespace)
	{
		ILDumpClassName(outstream, image, info, flags);
	}
	else
	{
		ILDumpIdentifier(outstream, ILClass_Name(info), 0, flags);
	}

	/* Dump the generic parameters, if any are present */
	ILDAsmDumpGenericParams(image, outstream,
							ILToProgramItem(info), flags);
}

/*
 * Dump information about a type definition and its nested classes.
 */
static void Dump_TypeAndNested(ILImage *image, FILE *outstream,
							   int flags, ILClass *info)
{
	ILMember *member;
	ILImplements *impl;
	ILNestedInfo *nested;
	int first;
	int isModule = 0;
	ILClassLayout *layout;
	unsigned long size;
	ILOverride *over;
	ILMethod *decl;
	ILMethod *body;

	/* Dump the namespace if this class is not nested */
	if(!ILClass_NestedParent(info) && ILClass_Namespace(info))
	{
		fputs(".namespace ", outstream);
		ILDumpIdentifier(outstream, ILClass_Namespace(info), 0, flags);
		fputs("\n{\n", outstream);
	}

	/* Dump the type header, if it is not "<Module>" */
	if(strcmp(ILClass_Name(info), "<Module>") != 0 ||
	   ILClass_Namespace(info) != 0)
	{
		ILProgramItem *parent;

		fputs(".class ", outstream);
		ILDumpFlags(outstream, ILClass_Attrs(info), ILTypeDefinitionFlags, 0);
		DumpClassName(outstream, image, info, flags, 0);
		if((parent = ILClass_Parent(info)) != 0)
		{
			fputs("\n    extends ", outstream);
			ILDumpProgramItem(outstream, image, parent, flags);
		}
		first = 1;
		impl = 0;
		while((impl = ILClassNextImplements(info, impl)) != 0)
		{
			ILProgramItem *interface;

			interface = ILImplementsGetInterface(impl);
			if(first)
			{
				fputs("\n    implements ", outstream);
				first = 0;
			}
			else
			{
				fputs(",\n", outstream);
				fputs("               ", outstream);
			}
			ILDumpProgramItem(outstream, image, interface, flags);
		}
		fputs("\n{\n", outstream);

		/* Dump the security information, if any */
		if((ILClass_Attrs(info) & IL_META_TYPEDEF_HAS_SECURITY) != 0)
		{
			ILDAsmDumpSecurity(image, outstream, (ILProgramItem *)info, flags);
		}

		/* Dump the class layout information, if any */
		layout = ILClassLayoutGetFromOwner(info);
		if(layout)
		{
			size = (unsigned long)(ILClassLayout_PackingSize(layout));
			if(size != 0)
			{
				fprintf(outstream, "\t.pack %lu\n", size);
			}
			size = (unsigned long)(ILClassLayout_ClassSize(layout));
			if(size != 0)
			{
				fprintf(outstream, "\t.size %lu\n", size);
			}
		}

		/* Dump the custom attributes for the class */
		if(ILProgramItem_HasAttrs(info))
		{
			ILDAsmDumpCustomAttrs(image, outstream, flags, 1,
								  ILToProgramItem(info));
		}

		/* Dump the custom attributes for the generic parameters if present */
		Dump_GenericParCustomAttrs(image, outstream, flags, 1,
								   ILToProgramItem(info));
	}
	else
	{
		isModule = 1;
		if(ILClassNextMember(info, 0) != 0)
		{
			fputs("// .class ", outstream);
			ILDumpFlags(outstream, ILClass_Attrs(info),
						ILTypeDefinitionFlags, 0);
			ILDumpClassName(outstream, image, info, flags);
			fputs("\n// { \n", outstream);
		}
	}

	/* Dump the nested classes */
	nested = 0;
	while((nested = ILClassNextNested(info, nested)) != 0)
	{
		Dump_TypeAndNested(image, outstream, flags,
						   ILNestedInfoGetChild(nested));
	}

	/* Dump the class members */
	member = 0;
	while((member = ILClassNextMember(info, member)) != 0)
	{
		switch(ILMemberGetKind(member))
		{
			case IL_META_MEMBERKIND_METHOD:
			{
				Dump_MethodDef(image, outstream, flags, (ILMethod *)member);
			}
			break;

			case IL_META_MEMBERKIND_FIELD:
			{
				Dump_FieldDef(image, outstream, flags, (ILField *)member);
			}
			break;

			case IL_META_MEMBERKIND_EVENT:
			{
				Dump_EventDef(image, outstream, flags, (ILEvent *)member);
			}
			break;

			case IL_META_MEMBERKIND_PROPERTY:
			{
				Dump_PropertyDef(image, outstream, flags, (ILProperty *)member);
			}
			break;
		}
	}

	/* Dump overrides that don't have bodies in this class */
	over = 0;
	while((over = (ILOverride *)ILClassNextMemberByKind
				(info, (ILMember *)over, IL_META_MEMBERKIND_OVERRIDE)) != 0)
	{
		body = ILOverride_Body(over);
		if(ILMethod_Owner(body) != info)
		{
			decl = ILOverride_Decl(over);
			fputs("\t.override ", outstream);
			ILDumpClassName(outstream, image, ILMethod_Owner(decl), flags);
			fputs("::", outstream);
			ILDumpIdentifier(outstream, ILMethod_Name(decl), 0, flags);
			fputs(" with ", outstream);
			ILDumpMethodType(outstream, image, ILMethod_Signature(body), flags,
							 ILMethod_Owner(body), ILMethod_Name(body), body);
			putc('\n', outstream);
		}
	}

	/* Dump the type footer, if it is not "<Module>" */
	if(!isModule)
	{
		fputs("}\n", outstream);
	}
	else if(ILClassNextMember(info, 0) != 0)
	{
		fputs("// }\n", outstream);
	}

	/* Dump the namespace footer if this class is not nested */
	if(!ILClass_NestedParent(info) && ILClass_Namespace(info))
	{
		fputs("}\n", outstream);
	}
}

/*
 * Dump information about a type definition.
 */
static void Dump_TypeDef(ILImage *image, FILE *outstream, int flags,
						 unsigned long token, ILClass *info,
						 unsigned long refToken)
{
	/* Ignore the type if it is nested: we'll get it elsewhere */
	if(ILClass_IsPublic(info) || ILClass_IsPrivate(info))
	{
		Dump_TypeAndNested(image, outstream, flags, info);
	}
}

void ILDAsmDumpClasses(ILImage *image, FILE *outstream, int flags)
{
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_TYPE_DEF,
					 (ILDAsmWalkFunc)Dump_TypeDef, 0);
}

#ifdef	__cplusplus
};
#endif
