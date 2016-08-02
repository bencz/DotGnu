/*
 * c_typeout.c - Send types to an assembly output stream.
 *
 * Copyright (C) 2002, 2008, 2009  Southern Storm Software, Pty Ltd.
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

#include <cscc/c/c_internal.h>
#include <image/program.h>
#include "il_dumpasm.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Hash table that holds the classes that are pending output.
 */
static ILHashTable *pendingHash = 0;

/*
 * Compute a hash value for an element or key.
 */
static unsigned long PendingHash_Compute(const void *value)
{
	return (unsigned long)(((ILNativeUInt)value) / sizeof(ILClass));
}

/*
 * Match an element against a key.
 */
static int PendingHash_Match(const void *elem, const void *key)
{
	return (elem == key);
}

/*
 * Determine if a value type corresponds to a C type that must be output.
 */
static int IsCValueType(ILClass *classInfo)
{
	if((classInfo->attributes & IL_META_TYPEDEF_TYPE_BITS) != 0)
	{
		return 1;
	}
	else if(!strncmp(ILClass_Name(classInfo), "array ", 6))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

void CTypeMarkForOutput(ILGenInfo *info, ILType *type)
{
	ILClass *classInfo;
	unsigned long param;
	unsigned long numParams;

	type = ILTypeStripPrefixes(type);
	if(ILType_IsValueType(type))
	{
		classInfo = ILType_ToValueType(type);
		if(IsCValueType(classInfo))
		{
			/* Scan up to find the outermost nesting level */
			while(ILClass_NestedParent(classInfo) != 0)
			{
				classInfo = ILClass_NestedParent(classInfo);
			}

			/* Register the class is it isn't already marked as complete */
			if(!ILClassIsComplete(classInfo))
			{
				/* Register the class to be expanded later */
				if(!pendingHash)
				{
					pendingHash = ILHashCreate(0, PendingHash_Compute,
											   PendingHash_Compute,
											   PendingHash_Match, 0);
					if(!pendingHash)
					{
						ILGenOutOfMemory(info);
					}
				}
				if(!ILHashFind(pendingHash, classInfo))
				{
					if(!ILHashAdd(pendingHash, classInfo))
					{
						ILGenOutOfMemory(info);
					}
				}
			}
		}
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		if(ILType_Kind(type) == IL_TYPE_COMPLEX_PTR)
		{
			/* Mark the referenced type */
			CTypeMarkForOutput(info, ILType_Ref(type));
		}
		else if((ILType_Kind(type) & IL_TYPE_COMPLEX_METHOD) != 0)
		{
			/* Mark the return and parameter types */
			CTypeMarkForOutput(info, ILTypeGetReturn(type));
			numParams = ILTypeNumParams(type);
			for(param = 1; param <= numParams; ++param)
			{
				CTypeMarkForOutput(info, ILTypeGetParam(type, param));
			}
		}
	}
}

void CGenOutputAttributes(ILGenInfo *info, FILE *stream, ILProgramItem *item)
{
	ILAttribute *attr;
	ILMethod *ctor;
	const void *value;
	ILUInt32 valueLen;
	ILUInt32 posn;

	attr = 0;
	while((attr = ILProgramItemNextAttribute(item, attr)) != 0)
	{
		fputs(".custom ", stream);
		ctor = ILProgramItemToMethod(ILAttributeTypeAsItem(attr));
		if(ctor)
		{
			ILDumpMethodType(stream, info->image,
							 ILMethod_Signature(ctor),
							 IL_DUMP_QUOTE_NAMES,
							 ILMethod_Owner(ctor),
							 ILMethod_Name(ctor),
							 ctor);
		}
		if((value = ILAttributeGetValue(attr, &valueLen)) != 0)
		{
			fputs(" = (", stream);
			for(posn = 0; posn < valueLen; ++posn)
			{
				if(posn != 0)
				{
					putc(' ', stream);
				}
				fprintf(stream, "%02X",
						(int)(((const unsigned char *)value)[posn]));
			}
			putc(')', stream);
		}
		putc('\n', stream);
	}
}

/*
 * Store to a global variable.
 */
static void StoreGlobal(ILGenInfo *info, ILClass *classInfo,
						const char *name1, const char *name2,
						FILE *stream)
{
	fputs("\tstsfld\tunsigned int32 ", stream);
	ILDumpClassName(stream, info->image, classInfo, IL_DUMP_QUOTE_NAMES);
	fprintf(stream, "::'%s.%s'\n", name1, name2);
}

/*
 * Output the definition of a pending class and mark any other
 * classes that it may depend upon.
 */
static void OutputPendingClass(ILGenInfo *info, ILClass *classInfo,
							   FILE *stream)
{
	ILClass *parent;
	ILNestedInfo *nested;
	ILField *field;
	ILClassLayout *classLayout;
	ILFieldLayout *fieldLayout;
	ILType *type;
	CTypeLayoutInfo layout;
	ILNode *node;
	int first;

	/* Ignore class references, which will normally be struct's or
	   union's that weren't fully defined in the current module */
	if(ILClassIsRef(classInfo))
	{
		return;
	}

	/* Determine if the type is complex */
	type = ILClassToType(classInfo);
	CTypeGetLayoutInfo(type, &layout);

	/* Output the class header.  We assume that there are no interfaces
	   because C structs, unions, etc do not need interfaces */
	fputs(".class ", stream);
	ILDumpFlags(stream, ILClass_Attrs(classInfo) &
						~IL_META_TYPEDEF_TYPE_BITS, ILTypeDefinitionFlags, 0);
	if(layout.category == C_TYPECAT_COMPLEX)
	{
		fputs("beforefieldinit ", stream);
	}
	ILDumpIdentifier(stream, ILClass_Name(classInfo), 0, IL_DUMP_QUOTE_NAMES);
	parent = ILClass_UnderlyingParentClass(classInfo);
	if(parent)
	{
		fputs(" extends ", stream);
		ILDumpClassName(stream, info->image, parent, IL_DUMP_QUOTE_NAMES);
	}
	fputs("\n{\n", stream);

	/* Output the attributes that are attached to the class */
	CGenOutputAttributes(info, stream, ILToProgramItem(classInfo));

	/* Output the class layout information if it is present */
	classLayout = ILClassLayoutGetFromOwner(classInfo);
	if(classLayout != 0)
	{
		fprintf(stream, ".size %lu\n.pack %lu\n",
				(unsigned long)(ILClassLayoutGetClassSize(classLayout)),
				(unsigned long)(ILClassLayoutGetPackingSize(classLayout)));
	}

	/* Output any nested classes */
	nested = 0;
	while((nested = ILClassNextNested(classInfo, nested)) != 0)
	{
		OutputPendingClass(info, ILNestedInfoGetChild(nested), stream);
	}

	/* Output the fields within this class, and mark their types.
	   We assume that there are no methods, because C structs, unions,
	   etc do not have methods associated with them */
	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
				(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		/* Dump the field definition */
		fputs(".field ", stream);
		fieldLayout = ILFieldLayoutGetFromOwner(field);
		if(fieldLayout != 0)
		{
			fprintf(stream, "[%lu] ",
					(unsigned long)(ILFieldLayoutGetOffset(fieldLayout)));
		}
		ILDumpFlags(stream, ILField_Attrs(field), ILFieldDefinitionFlags, 0);
		ILDumpType(stream, info->image, ILMember_Signature(field),
				   IL_DUMP_QUOTE_NAMES);
		putc(' ', stream);
		ILDumpIdentifier(stream, ILField_Name(field), 0, IL_DUMP_QUOTE_NAMES);
		ILDumpConstant(stream, ILToProgramItem(field), 0);
		putc('\n', stream);

		/* Dump the attributes on the field */
		CGenOutputAttributes(info, stream, ILToProgramItem(field));

		/* Mark the field's type for later output */
		CTypeMarkForOutput(info, ILMember_Signature(field));
	}

	/* If the type is complex, then we need to output some static fields */
	if(layout.category == C_TYPECAT_COMPLEX)
	{
		/* Output the "size.of" field, which defines the type's size */
		fputs(".field public static initonly unsigned int32 'size.of'\n",
			  stream);
		if(CTypeIsStruct(type))
		{
			field = 0;
			first = 1;
			while((field = CTypeNextField(type, field)) != 0)
			{
				if(!first)
				{
					fprintf(stream,
							".field public static initonly unsigned int32 "
							"'%s.offset'\n", ILField_Name(field));
				}
				else
				{
					/* We don't need the offset of the first field,
					   because it will always be zero */
					first = 0;
				}
			}
		}

		/* Output the header for the static constructor */
		fputs(".method private static hidebysig specialname rtspecialname "
			  "void .cctor() cil managed\n{\n", stream);
		info->stackHeight = 0;
		info->maxStackHeight = 0;

		/* Generate code to compute the size and field offsets */
		node = CTypeCreateSizeNode(type);
		ILNode_GenValue(node, info);
		StoreGlobal(info, classInfo, "size", "of", stream);
		ILGenAdjust(info, -1);
		if(CTypeIsStruct(type))
		{
			field = 0;
			first = 1;
			while((field = CTypeNextField(type, field)) != 0)
			{
				if(!first)
				{
					node = CTypeCreateOffsetNode(type, ILField_Name(field));
					ILNode_GenValue(node, info);
					StoreGlobal(info, classInfo, ILField_Name(field),
								"offset", stream);
					ILGenAdjust(info, -1);
				}
				else
				{
					/* We don't need the offset of the first field,
					   because it will always be zero */
					first = 0;
				}
			}
		}

		/* Output the footer for the static constructor */
		fprintf(stream, "\tret\n\t.maxstack %ld\n}\n", info->maxStackHeight);
	}

	/* Output the class footer */
	fputs("} // class ", stream);
	fputs(ILClass_Name(classInfo), stream);
	putc('\n', stream);
}

void CTypeOutputPending(ILGenInfo *info, FILE *stream)
{
	int sawSomething;
	ILHashIter iter;
	ILClass *classInfo;

	/* Bail out if there is no hash table, and hence no pending classes */
	if(!pendingHash)
	{
		return;
	}

	/* Keep scanning the hash table, outputting pending classes,
	   until everything is marked and nothing more gets added */
	do
	{
		sawSomething = 0;
		ILHashIterInit(&iter, pendingHash);
		while((classInfo = ILHashIterNextType(&iter, ILClass)) != 0)
		{
			if(!ILClassIsComplete(classInfo))
			{
				ILClassMarkComplete(classInfo);
				OutputPendingClass(info, classInfo, stream);
				sawSomething = 1;
			}
		}
	}
	while(sawSomething);
}

#ifdef	__cplusplus
};
#endif
