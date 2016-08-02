/*
 * meta_index.c - Handle metadata index parsing for an image.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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

#include "program.h"
#include <stdio.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Special token that marks the end of a mixed reference description table.
 */
#define	END_DESC		((ILUInt32)0xFFFFFFFF)

/*
 * Mixed reference type descriptions.  The first entry in each
 * table is the number of bits occupied by the token type code.
 */
static ILUInt32 const ScopeRef_Desc[] =
	{2,
	 IL_META_TOKEN_MODULE,
	 IL_META_TOKEN_MODULE_REF,
	 IL_META_TOKEN_ASSEMBLY_REF,
	 IL_META_TOKEN_TYPE_REF,
	 END_DESC};

static ILUInt32 const TypeDefRefOrSpec_Desc[] =
	{2,
	 IL_META_TOKEN_TYPE_DEF,
	 IL_META_TOKEN_TYPE_REF,
	 IL_META_TOKEN_TYPE_SPEC,
	 1,
	 END_DESC};

static ILUInt32 const MemberRefParent_Desc[] =
	{3,
	 IL_META_TOKEN_TYPE_DEF,
	 IL_META_TOKEN_TYPE_REF,
	 IL_META_TOKEN_MODULE_REF,
	 IL_META_TOKEN_METHOD_DEF,
	 IL_META_TOKEN_TYPE_SPEC,
	 1,
	 1,
	 1,
	 END_DESC};

static ILUInt32 const ConstantRef_Desc[] =
	{2,
	 IL_META_TOKEN_FIELD_DEF,
	 IL_META_TOKEN_PARAM_DEF,
	 IL_META_TOKEN_PROPERTY,
	 1,
	 END_DESC};

static ILUInt32 const CustomOwner_Desc[] =
	{5,
	 IL_META_TOKEN_METHOD_DEF,
	 IL_META_TOKEN_FIELD_DEF,
	 IL_META_TOKEN_TYPE_REF,
	 IL_META_TOKEN_TYPE_DEF,
	 IL_META_TOKEN_PARAM_DEF,
	 IL_META_TOKEN_INTERFACE_IMPL,
	 IL_META_TOKEN_MEMBER_REF,
	 IL_META_TOKEN_MODULE,
	 IL_META_TOKEN_DECL_SECURITY,
	 IL_META_TOKEN_PROPERTY,
	 IL_META_TOKEN_EVENT,
	 IL_META_TOKEN_STAND_ALONE_SIG,
	 IL_META_TOKEN_MODULE_REF,
	 IL_META_TOKEN_TYPE_SPEC,
	 IL_META_TOKEN_ASSEMBLY,
	 IL_META_TOKEN_ASSEMBLY_REF,
	 IL_META_TOKEN_FILE,
	 IL_META_TOKEN_EXPORTED_TYPE,
	 IL_META_TOKEN_MANIFEST_RESOURCE,
	 IL_META_TOKEN_GENERIC_PAR,
	 1,
	 1,
	 1,
	 1,
	 1,
	 1,
	 1,
	 1,
	 1,
	 1,
	 1,
	 1,
	 END_DESC};

static ILUInt32 const CustomName_Desc[] =
	{3,
	 IL_META_TOKEN_TYPE_REF,
	 IL_META_TOKEN_TYPE_DEF,
	 IL_META_TOKEN_METHOD_DEF,
	 IL_META_TOKEN_MEMBER_REF,
	 2,
	 1,
	 1,
	 1,
	 END_DESC};

static ILUInt32 const MarshalDef_Desc[] =
	{1,
	 IL_META_TOKEN_FIELD_DEF,
	 IL_META_TOKEN_PARAM_DEF,
	 END_DESC};

static ILUInt32 const HasDeclSecurity_Desc[] =
	{2,
	 IL_META_TOKEN_TYPE_DEF,
	 IL_META_TOKEN_METHOD_DEF,
	 IL_META_TOKEN_ASSEMBLY,
	 1,
	 END_DESC};

static ILUInt32 const EventOrProperty_Desc[] =
	{1,
	 IL_META_TOKEN_EVENT,
	 IL_META_TOKEN_PROPERTY,
	 END_DESC};

static ILUInt32 const MethodDefOrRef_Desc[] =
	{1,
	 IL_META_TOKEN_METHOD_DEF,
	 IL_META_TOKEN_MEMBER_REF,
	 END_DESC};

static ILUInt32 const FieldOrMethod_Desc[] =
	{1,
	 IL_META_TOKEN_FIELD_DEF,
	 IL_META_TOKEN_METHOD_DEF,
	 END_DESC};

static ILUInt32 const ExportedType_Desc[] =
	{2,
	 IL_META_TOKEN_FILE,
	 IL_META_TOKEN_ASSEMBLY_REF,
	 IL_META_TOKEN_EXPORTED_TYPE,
	 1,
	 END_DESC};

static ILUInt32 const GenericParOwner_Desc[] =
	{1,
	 IL_META_TOKEN_TYPE_DEF,
	 IL_META_TOKEN_METHOD_DEF,
	 END_DESC};

/*
 * Special field types.
 */
#define	STRREF_FIELD		((ILUInt32 *)0x0001)
#define	BLOBREF_FIELD		((ILUInt32 *)0x0003)
#define	GUIDREF_FIELD		((ILUInt32 *)0x0005)
#define	UINT16_FIELD		((ILUInt32 *)0x0007)
#define	UINT32_FIELD		((ILUInt32 *)0x0009)
#define	FIELD_FIELD			((ILUInt32 *)0x000B)
#define	METHOD_FIELD		((ILUInt32 *)0x000D)
#define	PARAM_FIELD			((ILUInt32 *)0x000F)
#define	OPTGEN_FIELD		((ILUInt32 *)0x0011)
#define	END_FIELD			((ILUInt32 *)0)
#define	TKREF_FIELD(name)	((ILUInt32 *)(IL_META_TOKEN_##name | 0x00001001))

/*
 * Field description tables.
 */
static const ILUInt32 * const Fields_Module[] =
	{UINT16_FIELD, STRREF_FIELD, GUIDREF_FIELD,
	 GUIDREF_FIELD, GUIDREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_TypeRef[] =
	{ScopeRef_Desc, STRREF_FIELD, STRREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_TypeDef[] =
	{UINT32_FIELD, STRREF_FIELD, STRREF_FIELD,
	 TypeDefRefOrSpec_Desc, FIELD_FIELD, METHOD_FIELD,
	 END_FIELD};

static const ILUInt32 * const Fields_FieldDef[] =
	{UINT16_FIELD, STRREF_FIELD, BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_MethodDef[] =
	{UINT32_FIELD, UINT16_FIELD, UINT16_FIELD,
	 STRREF_FIELD, BLOBREF_FIELD, PARAM_FIELD,
	 END_FIELD};

static const ILUInt32 * const Fields_ParamDef[] =
	{UINT16_FIELD, UINT16_FIELD, STRREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_InterfaceImpl[] =
	{TKREF_FIELD(TYPE_DEF), TypeDefRefOrSpec_Desc, END_FIELD};

static const ILUInt32 * const Fields_MemberRef[] =
	{MemberRefParent_Desc, STRREF_FIELD, BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_Constant[] =
	{UINT16_FIELD, ConstantRef_Desc, BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_CustomAttr[] =
	{CustomOwner_Desc, CustomName_Desc, BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_FieldMarshal[] =
	{MarshalDef_Desc, BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_DeclSecurity[] =
	{UINT16_FIELD, HasDeclSecurity_Desc, BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_ClassLayout[] =
	{UINT16_FIELD, UINT32_FIELD, TKREF_FIELD(TYPE_DEF), END_FIELD};

static const ILUInt32 * const Fields_FieldLayout[] =
	{UINT32_FIELD, TKREF_FIELD(FIELD_DEF), END_FIELD};

static const ILUInt32 * const Fields_StandAloneSig[] =
	{BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_EventMap[] =
	{TKREF_FIELD(TYPE_DEF), TKREF_FIELD(EVENT), END_FIELD};

static const ILUInt32 * const Fields_Event[] =
	{UINT16_FIELD, STRREF_FIELD, TypeDefRefOrSpec_Desc, END_FIELD};

static const ILUInt32 * const Fields_PropertyMap[] =
	{TKREF_FIELD(TYPE_DEF), TKREF_FIELD(PROPERTY), END_FIELD};

static const ILUInt32 * const Fields_Property[] =
	{UINT16_FIELD, STRREF_FIELD, BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_MethodSemantics[] =
	{UINT16_FIELD, TKREF_FIELD(METHOD_DEF),
	 EventOrProperty_Desc, END_FIELD};

static const ILUInt32 * const Fields_MethodImpl[] =
	{TKREF_FIELD(TYPE_DEF), MethodDefOrRef_Desc,
	 MethodDefOrRef_Desc, END_FIELD};

static const ILUInt32 * const Fields_ModuleRef[] =
	{STRREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_TypeSpec[] =
	{BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_ImplMap[] =
	{UINT16_FIELD, FieldOrMethod_Desc,
	 STRREF_FIELD, TKREF_FIELD(MODULE_REF),
	 END_FIELD};

static const ILUInt32 * const Fields_FieldRVA[] =
	{UINT32_FIELD, TKREF_FIELD(FIELD_DEF), END_FIELD};

static const ILUInt32 * const Fields_Assembly[] =
	{UINT32_FIELD, UINT16_FIELD, UINT16_FIELD,
	 UINT16_FIELD, UINT16_FIELD, UINT32_FIELD,
	 BLOBREF_FIELD, STRREF_FIELD, STRREF_FIELD,
	 END_FIELD};

static const ILUInt32 * const Fields_ProcessorDef[] =
	{UINT32_FIELD, END_FIELD};

static const ILUInt32 * const Fields_OSDef[] =
	{UINT32_FIELD, UINT32_FIELD, UINT32_FIELD, END_FIELD};

static const ILUInt32 * const Fields_AssemblyRef[] =
	{UINT16_FIELD, UINT16_FIELD, UINT16_FIELD,
	 UINT16_FIELD, UINT32_FIELD, BLOBREF_FIELD,
	 STRREF_FIELD, STRREF_FIELD, BLOBREF_FIELD,
	 END_FIELD};

static const ILUInt32 * const Fields_ProcessorRef[] =
	{UINT32_FIELD, TKREF_FIELD(ASSEMBLY_REF), END_FIELD};

static const ILUInt32 * const Fields_OSRef[] =
	{UINT32_FIELD, UINT32_FIELD, UINT32_FIELD,
	 TKREF_FIELD(ASSEMBLY_REF), END_FIELD};

static const ILUInt32 * const Fields_File[] =
	{UINT32_FIELD, STRREF_FIELD, BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_ExportedType[] =
	{UINT32_FIELD, UINT32_FIELD, STRREF_FIELD,
	 STRREF_FIELD, ExportedType_Desc, END_FIELD};

static const ILUInt32 * const Fields_ManifestResource[] =
	{UINT32_FIELD, UINT32_FIELD, STRREF_FIELD,
	 ExportedType_Desc, END_FIELD};

static const ILUInt32 * const Fields_ExeLocation[] =
	{UINT32_FIELD, STRREF_FIELD, STRREF_FIELD,
	 STRREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_NestedClass[] =
	{TKREF_FIELD(TYPE_DEF), TKREF_FIELD(TYPE_DEF), END_FIELD};

static const ILUInt32 * const Fields_GenericPar[] =
	{UINT16_FIELD, UINT16_FIELD, GenericParOwner_Desc, 
	 STRREF_FIELD/*, OPTGEN_FIELD, TypeDefRefOrSpec_Desc,
	 OPTGEN_FIELD, TypeDefRefOrSpec_Desc*/, END_FIELD};

static const ILUInt32 * const Fields_MethodSpec[] =
	{MethodDefOrRef_Desc, BLOBREF_FIELD, END_FIELD};

static const ILUInt32 * const Fields_GenericConstraint[] =
	{TKREF_FIELD(GENERIC_PAR), TypeDefRefOrSpec_Desc, END_FIELD};

/*
 * Table of all field description types.
 */
static const ILUInt32 * const * const FieldDescriptions[] = {
	Fields_Module,				/* 00 */
	Fields_TypeRef,
	Fields_TypeDef,
	0,
	Fields_FieldDef,
	0,
	Fields_MethodDef,
	0,
	Fields_ParamDef,			/* 08 */
	Fields_InterfaceImpl,
	Fields_MemberRef,
	Fields_Constant,
	Fields_CustomAttr,
	Fields_FieldMarshal,
	Fields_DeclSecurity,
	Fields_ClassLayout,
	Fields_FieldLayout,			/* 10 */
	Fields_StandAloneSig,
	Fields_EventMap,
	0,
	Fields_Event,
	Fields_PropertyMap,
	0,
	Fields_Property,
	Fields_MethodSemantics,		/* 18 */
	Fields_MethodImpl,
	Fields_ModuleRef,
	Fields_TypeSpec,
	Fields_ImplMap,
	Fields_FieldRVA,
	0,
	0,
	Fields_Assembly,			/* 20 */
	Fields_ProcessorDef,
	Fields_OSDef,
	Fields_AssemblyRef,
	Fields_ProcessorRef,
	Fields_OSRef,
	Fields_File,
	Fields_ExportedType,
	Fields_ManifestResource,	/* 28 */
	Fields_NestedClass,
	Fields_GenericPar,
	Fields_MethodSpec,
	Fields_GenericConstraint,
	0,
	0,
	0,
	0,							/* 30 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,							/* 38 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
};

#if IL_DEBUG_META

/*
 * Names of the token types.
 */
static const char * const tokenNames[64] = {
	"Module",					/* 00 */
	"TypeRef",
	"TypeDef",
	"Token_03",
	"FieldDef",
	"Token_05",
	"MethodDef",
	"Token_06",
	"ParamDef",					/* 08 */
	"InterfaceImpl",
	"MemberRef",
	"Constant",
	"CustomAttr",
	"FieldMarshal",
	"DeclSecurity",
	"ClassLayout",
	"FieldLayout",				/* 10 */
	"StandAloneSig",
	"EventMap",
	"Token_13",
	"Event",
	"PropertyAssociation",
	"Token_16",
	"Property",
	"MethodSemantics",			/* 18 */
	"MethodImpl",
	"ModuleRef",
	"TypeSpec",
	"ImplMap",
	"FieldRVA",
	"Token_1E",
	"Token_1F",
	"Assembly",					/* 20 */
	"ProcessorDef",
	"OSDef",
	"AssemblyRef",
	"ProcessorRef",
	"OSRef",
	"File",
	"ExportedType",
	"ManifestResource,"			/* 28 */
	"NestedClass",
	"GenericPar",
	"MethodSpec",
	"GenericConstraint",
	"Token_2D",
	"Token_2E",
	"Token_2F",
	"Token_30",					/* 30 */
	"Token_31",
	"Token_32",
	"Token_33",
	"Token_34",
	"Token_35",
	"Token_36",
	"Token_37",
	"Token_38",					/* 38 */
	"Token_39",
	"Token_3A",
	"Token_3B",
	"Token_3C",
	"Token_3D",
	"Token_3E",
	"Token_3F",
};

#endif

/*
 * Determine the size of a token type's metadata index records.
 * Returns zero if the token size information is unknown.
 */
static int TokenSize(ILImage *image, int strRefSize, int blobRefSize,
					 int guidRefSize, const ILUInt32 * const *desc)
{
	int size = 0;
	const ILUInt32 *type;
	ILUInt32 token;
	if(desc)
	{
		while((type = *desc++) != END_FIELD)
		{
			if((((ILUInt32)(ILNativeUInt)type) & 1) != 0)
			{
				/* Simple type, or direct token table index */
				switch((ILUInt32)(ILNativeUInt)type)
				{
					case (ILUInt32)(ILNativeUInt)STRREF_FIELD:
					{
						size += strRefSize;
					}
					break;

					case (ILUInt32)(ILNativeUInt)BLOBREF_FIELD:
					{
						size += blobRefSize;
					}
					break;

					case (ILUInt32)(ILNativeUInt)GUIDREF_FIELD:
					{
						size += guidRefSize;
					}
					break;

					case (ILUInt32)(ILNativeUInt)UINT16_FIELD:
					{
						size += 2;
					}
					break;

					case (ILUInt32)(ILNativeUInt)UINT32_FIELD:
					{
						size += 4;
					}
					break;

					case (ILUInt32)(ILNativeUInt)FIELD_FIELD:
					{
						if(image->tokenCount
								[IL_META_TOKEN_FIELD_DEF >> 24]
									> (ILUInt32)0xFFFE)
						{
							size += 4;
						}
						else
						{
							size += 2;
						}
					}
					break;

					case (ILUInt32)(ILNativeUInt)METHOD_FIELD:
					{
						if(image->tokenCount
								[IL_META_TOKEN_METHOD_DEF >> 24]
									> (ILUInt32)0xFFFE)
						{
							size += 4;
						}
						else
						{
							size += 2;
						}
					}
					break;

					case (ILUInt32)(ILNativeUInt)PARAM_FIELD:
					{
						if(image->tokenCount
								[IL_META_TOKEN_PARAM_DEF >> 24]
									> (ILUInt32)0xFFFE)
						{
							size += 4;
						}
						else
						{
							size += 2;
						}
					}
					break;

					case (ILUInt32)(ILNativeUInt)OPTGEN_FIELD:
					{
						/* If this image has the GenericConstraint
						   table, then skip the next field */
						if(image->tokenCount
								[IL_META_TOKEN_GENERIC_CONSTRAINT >> 24] > 0)
						{
							++desc;
						}
					}
					break;

					default:
					{
						if(image->tokenCount
								[((ILUInt32)(ILNativeUInt)type) >> 24]
									> (ILUInt32)0xFFFF)
						{
							size += 4;
						}
						else
						{
							size += 2;
						}
					}
					break;
				}
			}
			else
			{
				/* Mixed reference type */
				ILUInt32 limit;

				limit = (((ILUInt32)0xFFFF) >> *type++);
				size += 2;
				while((token = *type++) != END_DESC)
				{
					if(token != 1 && token != 2 &&
					   image->tokenCount[token >> 24] > limit)
					{
						size += 2;
						break;
					}
				}
			}
		}
	}
	return size;
}

/*
 * Parse a token type's metadata index record.  Returns a load
 * error, or zero if OK.
 */
static int ParseToken(ILImage *image, int strRefSize, int blobRefSize,
					  int guidRefSize, const ILUInt32 * const *desc,
					  const unsigned char *item, ILUInt32 *values,
					  unsigned long token)
{
	const unsigned char *ptr = item;
	int index = 0;
	const ILUInt32 *type;
	const ILUInt32 *start;
	ILUInt32 limit;
	ILUInt32 temp;
	ILMetaDataRead reader;
	if(desc)
	{
		while((type = *desc++) != END_FIELD)
		{
			if((((ILUInt32)(ILNativeUInt)type) & 1) != 0)
			{
				/* Simple type, or direct token table index */
				switch((ILUInt32)(ILNativeUInt)type)
				{
					case (ILUInt32)(ILNativeUInt)STRREF_FIELD:
					{
						/* Read a string index and validate it */
						if(strRefSize == 2)
						{
							temp = values[index++] = IL_READ_UINT16(ptr);
						}
						else
						{
							temp = values[index++] = IL_READ_UINT32(ptr);
						}
						if(temp >= image->stringPoolSize)
						{
							META_VAL_ERROR("bad string index");
							return IL_LOADERR_BAD_META;
						}
						ptr += strRefSize;
					}
					break;

					case (ILUInt32)(ILNativeUInt)BLOBREF_FIELD:
					{
						/* Read a blob index and validate it.  This
						   results in three fields in the output array */
						if(blobRefSize == 2)
						{
							temp = values[index++] = IL_READ_UINT16(ptr);
						}
						else
						{
							temp = values[index++] = IL_READ_UINT32(ptr);
						}
						if(temp != 0 && temp >= image->blobPoolSize)
						{
							META_VAL_ERROR("bad blob index");
							return IL_LOADERR_BAD_META;
						}
						values[index] = values[index - 1];
						++index;
						if(temp != 0)
						{
							reader.data =
								(unsigned char *)(image->blobPool + temp);
							reader.len = image->blobPoolSize - temp;
							reader.error = 0;
							temp = values[index++] =
								ILMetaUncompressData(&reader);
							if(temp > reader.len || reader.error)
							{
								META_VAL_ERROR("invalid blob length");
								return IL_LOADERR_BAD_META;
							}
							values[index - 2] =
								(ILUInt32)(image->blobPoolSize - reader.len);
						}
						else
						{
							/* Zero is always a valid blob index and
							   indicates "no value" */
							values[index++] = 0;
						}
						ptr += blobRefSize;
					}
					break;

					case (ILUInt32)(ILNativeUInt)GUIDREF_FIELD:
					{
						/* Read an index into the "#GUID" entry */
						ILUInt32 size;

						if(guidRefSize == 2)
						{
							temp = IL_READ_UINT16(ptr);
							ptr += 2;
						}
						else
						{
							temp = IL_READ_UINT32(ptr);
							ptr += 4;
						}
						if(temp != 0)
						{
							if(temp >= (ILUInt32)0x10000000 ||
							   !ILImageGetMetaEntry(image, "#GUID", &size) ||
							   size < 16 || (temp * 16) > (ILUInt32)size)
							{
								META_VAL_ERROR("bad GUID index");
								return IL_LOADERR_BAD_META;
							}
							else
							{
								values[index] = (temp - 1) * 16;
							}
						}
						else
						{
							/* The GUID value is unspecified */
							values[index] = IL_MAX_UINT32;
						}
						++index;
					}
					break;

					case (ILUInt32)(ILNativeUInt)UINT16_FIELD:
					{
						values[index++] = IL_READ_UINT16(ptr);
						ptr += 2;
					}
					break;

					case (ILUInt32)(ILNativeUInt)UINT32_FIELD:
					{
						values[index++] = IL_READ_UINT32(ptr);
						ptr += 4;
					}
					break;

					case (ILUInt32)(ILNativeUInt)FIELD_FIELD:
					{
						/* Read a field index.  If the value is the
						   size of the field table + 1, then it
						   indicates a type with no fields */
						limit = image->tokenCount
									[IL_META_TOKEN_FIELD_DEF >> 24];
						if(limit > (ILUInt32)0xFFFE)
						{
							temp = IL_READ_UINT32(ptr);
							ptr += 4;
						}
						else
						{
							temp = IL_READ_UINT16(ptr);
							ptr += 2;
						}
						if(temp == (limit + 1))
						{
							values[index++] = 0;
						}
						else if(temp > 0 && temp <= limit)
						{
							values[index++] = IL_META_TOKEN_FIELD_DEF | temp;
						}
						else
						{
							META_INDEX_ERROR("FieldDef");
							return IL_LOADERR_BAD_META;
						}
					}
					break;

					case (ILUInt32)(ILNativeUInt)METHOD_FIELD:
					{
						/* Read a method index.  If the value is the
						   size of the method table + 1, then it
						   indicates a type with no method */
						limit = image->tokenCount
									[IL_META_TOKEN_METHOD_DEF >> 24];
						if(limit > (ILUInt32)0xFFFE)
						{
							temp = IL_READ_UINT32(ptr);
							ptr += 4;
						}
						else
						{
							temp = IL_READ_UINT16(ptr);
							ptr += 2;
						}
						if(temp == (limit + 1))
						{
							values[index++] = 0;
						}
						else if(temp > 0 && temp <= limit)
						{
							values[index++] = IL_META_TOKEN_METHOD_DEF | temp;
						}
						else
						{
							META_INDEX_ERROR("MethodDef");
							return IL_LOADERR_BAD_META;
						}
					}
					break;

					case (ILUInt32)(ILNativeUInt)PARAM_FIELD:
					{
						/* Read a parameter index.  If the value is the
						   size of the parameter table + 1, then it
						   indicates a method with no parameters */
						limit = image->tokenCount
									[IL_META_TOKEN_PARAM_DEF >> 24];
						if(limit > (ILUInt32)0xFFFE)
						{
							temp = IL_READ_UINT32(ptr);
							ptr += 4;
						}
						else
						{
							temp = IL_READ_UINT16(ptr);
							ptr += 2;
						}
						if(temp == (limit + 1))
						{
							values[index++] = 0;
						}
						else if(temp > 0 && temp <= limit)
						{
							values[index++] = IL_META_TOKEN_PARAM_DEF | temp;
						}
						else
						{
							META_INDEX_ERROR("ParamDef");
							return IL_LOADERR_BAD_META;
						}
					}
					break;

					case (ILUInt32)(ILNativeUInt)OPTGEN_FIELD:
					{
						/* If this image has the GenericConstraint
						   table, then skip the next field */
						if(image->tokenCount
								[IL_META_TOKEN_GENERIC_CONSTRAINT >> 24] > 0)
						{
							values[index++] = 0;
							++desc;
						}
					}
					break;

					default:
					{
						/* Read a normal token table index */
						limit = image->tokenCount
							[((ILUInt32)(ILNativeUInt)type) >> 24];
						if(limit <= (ILUInt32)0xFFFF)
						{
							temp = IL_READ_UINT16(ptr);
							ptr += 2;
						}
						else
						{
							temp = IL_READ_UINT32(ptr);
							ptr += 4;
						}
						if(temp > 0 && temp <= limit)
						{
							values[index++] =
								((((ILUInt32)(ILNativeUInt)type)
										& 0xFF000000) | temp);
						}
						else
						{
							META_INDEX_ERROR(tokenNames
									[((ILUInt32)(ILNativeUInt)type) >> 24]);
							return IL_LOADERR_BAD_META;
						}
					}
					break;
				}
			}
			else
			{
				/* Mixed reference type */
				start = type;
				limit = (((ILUInt32)0xFFFF) >> *type++);
				while((temp = *type++) != END_DESC)
				{
					if(temp != 1 && temp != 2 &&
					   image->tokenCount[temp >> 24] > limit)
					{
						break;
					}
				}
				if(temp == END_DESC)
				{
					/* The reference is 2 bytes in size */
					temp = IL_READ_UINT16(ptr);
					ptr += 2;
				}
				else
				{
					/* The reference is 4 bytes in size */
					temp = IL_READ_UINT32(ptr);
					ptr += 4;
				}
				values[index] = (temp >> *start);
				temp = (temp & ((((ILUInt32)1) << *start) - 1));
				temp = start[temp + 1];
				if(temp != 1 && temp != 2)
				{
					if(values[index] == 0)
					{
						/* There are some files that seem to use
						   index 0 as a "not specified" value */
						values[index++] = 0;
					}
					else if(values[index] <= image->tokenCount[temp >> 24])
					{
						values[index++] |= temp;
					}
					else
					{
						META_INDEX_ERROR
							(tokenNames[((ILUInt32)temp) >> 24]);
						return IL_LOADERR_BAD_META;
					}
				}
				else if(temp != 2)
				{
					/* The low bits are an invalid type indicator */
					META_VAL_ERROR("invalid token reference");
					return IL_LOADERR_BAD_META;
				}
				else
				{
					/* Custom attribute name that is represented as a string */
					values[index++] = IL_META_TOKEN_STRING;
				}
			}
		}
		return 0;
	}
	else
	{
		META_VAL_ERROR("undocumented token type");
		return IL_LOADERR_UNDOC_META;
	}
}

/*
 * Parse the metadata index blob to determine the number
 * and size of each of the token record types, and
 * their positions within the index blob.
 */
static int ParseMetaIndex(ILImage *image, int loadFlags)
{
	unsigned char *index;
	ILUInt32 size;
	unsigned long len;
	ILUInt8 sizeBits;
	ILUInt64 typesPresent;
	int type;
	int strRefSize;
	int blobRefSize;
	int guidRefSize;
	unsigned char *item;
	int error;
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	unsigned long tokenBase;
	unsigned long token;

	/* Extract the "#~" blob from the metadata */
	index = ILImageGetMetaEntry(image, "#~", &size);
	if(!index)
	{
		index = ILImageGetMetaEntry(image, "#-", &size);
		if(!index)
		{
			META_ERROR("index is missing or an obsolete version");
			return IL_LOADERR_BAD_META;
		}
	}

	/* Pull apart the index header */
	if(size < 24)
	{
		META_ERROR("index header is truncated");
		return IL_LOADERR_BAD_META;
	}
	sizeBits = index[6];

	/* Determine the size of each of the token structures */
	strRefSize = (sizeBits & IL_META_SIZE_FLAG_STRREF) ? 4 : 2;
	guidRefSize = (sizeBits & IL_META_SIZE_FLAG_GUIDREF) ? 4 : 2;
	blobRefSize = (sizeBits & IL_META_SIZE_FLAG_BLOBREF) ? 4 : 2;

	typesPresent = IL_READ_UINT64(index + 8);
	image->sorted = IL_READ_UINT64(index + 16);
	index += 24;
	size -= 24;

	/* Parse the token count table just after the header */
	for(type = 0; type < 64; ++type)
	{
		if((typesPresent & (((ILUInt64)1) << type)) == 0)
		{
			/* This type is not in the table, so skip it */
			continue;
		}
		if(size < 4)
		{
			META_ERROR("token count table is truncated");
			return IL_LOADERR_BAD_META;
		}
		image->tokenCount[type] = IL_READ_UINT32(index);
		if(image->tokenCount[type] >= (unsigned long)0x00FFFFFF)
		{
			META_ERROR("too many tokens");
			return IL_LOADERR_BAD_META;
		}
		index += 4;
		size -= 4;
	}

	for(type = 0; type < 64; ++type)
	{
		if((typesPresent & (((ILUInt64)1) << type)) != 0)
		{
			image->tokenSize[type] =
					TokenSize(image, strRefSize, blobRefSize,
						      guidRefSize, FieldDescriptions[type]);
		   	if(image->tokenSize[type] == 0)
			{
				if(type <= (IL_META_TOKEN_NESTED_CLASS >> 24))
				{
					/* This is a type that we probably need to know about,
					   so we have no choice but to abort */
				#if IL_DEBUG_META
					/* Report errors for the remaining undocumented types */
					while(type < 64)
					{
						if((typesPresent & (((ILUInt64)1) << type)) != 0 &&
						   FieldDescriptions[type] == 0)
						{
							fprintf(stderr, "metadata error: uses %ld "
											"instances of undocumented "
											"token type 0x%08lX\n",
									(unsigned long)(image->tokenCount[type]),
								    (((unsigned long)type) << 24));
						}
						++type;
					}
				#endif
					return IL_LOADERR_UNDOC_META;
				}
				else
				{
					/* Types above "nested class" are not strictly necessary
					   to the understanding of the file.  We will assume that
					   they provide debug and other information that we don't
					   care about at present.  We clear the token counts and
					   sizes of anything else in the table.  This will
					   hopefully make our implementation robust in the face
					   of future additions to the specification */
					while(type < 64)
					{
					#if IL_DEBUG_META
						if((typesPresent & (((ILUInt64)1) << type)) != 0 &&
						   FieldDescriptions[type] == 0)
						{
							fprintf(stderr, "metadata warning: uses %ld "
											"instances of undocumented token "
											"type 0x%08lX - continuing\n",
									(unsigned long)(image->tokenCount[type]),
								    (((unsigned long)type) << 24));
						}
					#endif
						image->tokenCount[type] = 0;
						image->tokenSize[type] = 0;
						typesPresent &= ~(((ILUInt64)1) << type);
						++type;
					}
					break;
				}
			}
		}
	}

	/* Calculate the starting positions of each table */
	for(type = 0; type < 64; ++type)
	{
		if((typesPresent & (((ILUInt64)1) << type)) != 0)
		{
			if((size / (unsigned long)(image->tokenSize[type]))
					< image->tokenCount[type])
			{
				META_ERROR("index is truncated");
				return IL_LOADERR_BAD_META;
			}
			image->tokenStart[type] = index;
			len = ((unsigned long)(image->tokenSize[type])) *
						image->tokenCount[type];
			index += len;
			size -= len;
		}
	}

	/* If the "pre-validate" flag is set, then validate the token
	   index information now.  This checks that all of the references
	   are in range, and that all of the fields have legal values.
	   If there are errors, we report as many of them as possible.
	   If the flag is not set, we validate only when tokens are used */
	if((loadFlags & IL_LOADFLAG_PRE_VALIDATE) != 0)
	{
		error = 0;
		for(type = 0; type < 64; ++type)
		{
			len = image->tokenCount[type];
			size = (unsigned long)(image->tokenSize[type]);
			tokenBase = (((unsigned long)type) << 24);
			for(token = 0; token < len; ++token)
			{
				item = image->tokenStart[type] + (token * size);
				if(error != 0)
				{
					/* We've already seen an error, so ignore the return */
					ParseToken(image, strRefSize, blobRefSize, guidRefSize,
							   FieldDescriptions[type], item, values,
							   tokenBase + token + 1);
				}
				else
				{
					error = ParseToken(image, strRefSize, blobRefSize,
									   guidRefSize, FieldDescriptions[type],
									   item, values, tokenBase + token + 1);
				}
			}
		}
		if(error != 0)
		{
			return error;
		}
	}

	/* Record the size of STRREF's and BLOBREF's for later */
	image->strRefBig = (sizeBits & IL_META_SIZE_FLAG_STRREF) ? -1 : 0;
	image->guidRefBig = (sizeBits & IL_META_SIZE_FLAG_GUIDREF) ? -1 : 0;
	image->blobRefBig = (sizeBits & IL_META_SIZE_FLAG_BLOBREF) ? -1 : 0;

	/* The index has been successfully parsed */
	return 0;
}

int _ILImageParseMeta(ILImage *image, const char *filename, int flags)
{
	int error;

	/* Load the string pool information */
	image->stringPool = ILImageGetMetaEntry(image, "#Strings",
											&(image->stringPoolSize));
	if(!(image->stringPool))
	{
		image->stringPoolSize = 0;
	}
	else if(image->stringPoolSize > 0 &&
	        (image->stringPool)[image->stringPoolSize - 1] != 0)
	{
		META_ERROR("string pool does not end with a NUL");
		return IL_LOADERR_BAD_META;
	}

	/* Load the blob pool information */
	image->blobPool = ILImageGetMetaEntry(image, "#Blob",
										  &(image->blobPoolSize));
	if(!(image->blobPool))
	{
		image->blobPoolSize = 0;
	}

	/* Load the user string pool information */
	image->userStringPool = ILImageGetMetaEntry(image, "#US",
										        &(image->userStringPoolSize));
	if(!(image->userStringPool))
	{
		image->userStringPoolSize = 0;
	}

	/* Parse the metadata index */
	error = ParseMetaIndex(image, flags);
	if(error != 0)
	{
		return error;
	}

	/* Build all of the metadata structures */
	error = _ILImageBuildMetaStructures(image, filename, flags);
	if(error != 0)
	{
		return error;
	}

	/* Ready to go */
	return 0;
}

int _ILImageRawTokenData(ILImage *image, ILToken token,
						 ILUInt32 *values)
{
	if(token < (unsigned long)0x40000000)
	{
		ILToken tokenId = (token & (unsigned long)0x00FFFFFF);
		ILToken tokenType = (token >> 24);
		if(tokenId >= 1 && tokenId <= image->tokenCount[tokenType])
		{
			return (ParseToken(image, (image->strRefBig ? 4 : 2),
						   (image->blobRefBig ? 4 : 2),
						   (image->guidRefBig ? 4 : 2),
						   FieldDescriptions[tokenType],
						   image->tokenStart[tokenType] + (tokenId - 1) *
							 ((unsigned long)(image->tokenSize[tokenType])),
						   values, token) == 0);
		}
	}
	return 0;
}

unsigned long ILImageNumTokens(ILImage *image, ILToken tokenType)
{
	if(tokenType < (unsigned long)0x40000000)
	{
		return image->tokenCount[tokenType >> 24];
	}
	else
	{
		return 0;
	}
}

void *ILImageTokenInfo(ILImage *image, ILToken token)
{
	void **data;
	void *item;
	if(token < (unsigned long)0x40000000)
	{
		ILToken tokenId = (token & (unsigned long)0x00FFFFFF);
		ILToken tokenType = (token >> 24);
		if(tokenId >= 1 && tokenId <= image->tokenCount[tokenType])
		{
			data = image->tokenData[tokenType];
			if(data)
			{
				item = data[tokenId - 1];
				if(item)
				{
					return item;
				}
			}
			if(image->type != IL_IMAGETYPE_BUILDING)
			{
				/* Perform on-demand loading of the token */
				return _ILImageLoadOnDemand(image, token);
			}
		}
	}
	return 0;
}

int _ILImageTokenAlreadyLoaded(ILImage *image, ILToken token)
{
	void **data;
	if(token < (unsigned long)0x40000000)
	{
		ILToken tokenId = (token & (unsigned long)0x00FFFFFF);
		ILToken tokenType = (token >> 24);
		if(tokenId >= 1 && tokenId <= image->tokenCount[tokenType])
		{
			data = image->tokenData[tokenType];
			if(data)
			{
				if(data[tokenId - 1])
				{
					return 1;
				}
			}
		}
	}
	return 0;
}

void *ILImageNextToken(ILImage *image, ILToken tokenType, void *prev)
{
	ILToken token = (prev ? (((ILProgramItem *)prev)->token + 1)
						  : (tokenType | 1));
	return ILImageTokenInfo(image, token);
}

void *ILImageSearchForToken(ILImage *image, ILToken tokenType,
							ILImageCompareFunc compareFunc,
							void *userData)
{
	ILToken maxToken;
	ILToken token;
	ILToken left, right;
	void *item;
	void **data;
	int cmp;

	/* Find the table in question */
	maxToken = (tokenType | image->tokenCount[tokenType >> 24]);
	data = image->tokenData[tokenType >> 24];

	/* Is the table sorted? */
	if((image->sorted & (((ILUInt64)1) << (tokenType >> 24))) != 0)
	{
		/* The table is sorted, so use a binary search */
		left = (tokenType | 1);
		right = maxToken;
		while(left <= right)
		{
			token = ((left + right) / 2);
			item = ILImageTokenInfo(image, token);
			if(!item)
			{
				/* There is a gap in the table: revert to linear search */
				goto linearSearch;
			}
			cmp = (*compareFunc)(item, userData);
			if(!cmp)
			{
				return item;
			}
			else if(cmp < 0)
			{
				left = token + 1;
			}
			else
			{
				right = token - 1;
			}
		}
	}
	else
	{
		/* The table is unsorted, so search linearly */
	linearSearch:
		for(token = (tokenType | 1); token <= maxToken; ++token)
		{
			if(data && (item = data[(token & ~IL_META_TOKEN_MASK) - 1]) != 0)
			{
				if((*compareFunc)(item, userData) == 0)
				{
					return item;
				}
			}
			else
			{
				item = ILImageTokenInfo(image, token);
				if(item && (*compareFunc)(item, userData) == 0)
				{
					return item;
				}
			}
		}
	}

	/* If we get here, then the item could not be found */
	return 0;
}

void _ILImageFreeTokens(ILImage *image)
{
	int tokenType;
	for(tokenType = 0; tokenType < 64; ++tokenType)
	{
		if(image->tokenData[tokenType])
		{
			ILFree(image->tokenData[tokenType]);
		}
	}
}

int _ILImageSetToken(ILImage *image, ILProgramItem *item,
					 unsigned long token, unsigned long tokenKind)
{
	void **data;
	unsigned long tokenId;
	unsigned long tokenType;
	unsigned long count;

	/* Bail out if no image (i.e. the item is attached to the context) */
	if(!image)
	{
		return 1;
	}

	/* Are we loading or building the image? */
	if(image->type != IL_IMAGETYPE_BUILDING)
	{
		/* We are loading an image, so use the supplied token */
		if(!(item->token))
		{
			item->token = (ILUInt32)token;
		}
		if(token < (unsigned long)0x40000000)
		{
			tokenId = (token & (unsigned long)0x00FFFFFF);
			tokenType = (token >> 24);
			if(tokenId >= 1 && tokenId <= image->tokenCount[tokenType])
			{
				data = image->tokenData[tokenType];
				if(data)
				{
					data[tokenId - 1] = (void *)item;
					return 1;
				}
				if((data = (void **)ILCalloc(image->tokenCount[tokenType],
											 sizeof(void *))) != 0)
				{
					image->tokenData[tokenType] = data;
					data[tokenId - 1] = (void *)item;
					return 1;
				}
			}
		}
	}
	else
	{
		/* We are building an image, so allocate a new token */
		tokenType = (tokenKind >> 24);
		count = image->tokenCount[tokenType];
		token = tokenKind | (count + 1);
		item->token = (ILUInt32)token;
		if((count & (unsigned long)0x3F) == 0)
		{
			/* We need to make the token table bigger */
			data = (void **)ILRealloc(image->tokenData[tokenType],
									  sizeof(void *) * (count + 64));
			if(!data)
			{
				return 0;
			}
			image->tokenData[tokenType] = data;
		}
		image->tokenData[tokenType][count] = (void *)item;
		++(image->tokenCount[tokenType]);
		return 1;
	}
	return 0;
}

void _ILImageComputeTokenSizes(ILImage *image)
{
	int strRefSize, blobRefSize, guidRefSize;
	ILModule *module;
	unsigned long numGUIDs;
	int type;

	/* Compute the size of STRREF, BLOBREF, and GUIDREF values */
	if(image->stringPoolSize > (unsigned long)0xFFFF)
	{
		strRefSize = 4;
	}
	else
	{
		strRefSize = 2;
	}
	if(image->blobPoolSize > (unsigned long)0xFFFF)
	{
		blobRefSize = 4;
	}
	else
	{
		blobRefSize = 2;
	}
	module = 0;
	numGUIDs = 0;
	while((module = (ILModule *)ILImageNextToken
				(image, IL_META_TOKEN_MODULE, module)) != 0)
	{
		if(ILModule_MVID(module) != 0)
		{
			++numGUIDs;
		}
		if(ILModule_EncId(module) != 0)
		{
			++numGUIDs;
		}
		if(ILModule_EncBaseId(module) != 0)
		{
			++numGUIDs;
		}
	}
	if(numGUIDs > (unsigned long)0xFFFF)
	{
		guidRefSize = 4;
	}
	else
	{
		guidRefSize = 2;
	}
	image->strRefBig = (strRefSize == 4) ? -1 : 0;
	image->blobRefBig = (blobRefSize == 4) ? -1 : 0;
	image->guidRefBig = (guidRefSize == 4) ? -1 : 0;

	/* Compute the size of all of the token types */
	for(type = 0; type < 64; ++type)
	{
		image->tokenSize[type] =
				TokenSize(image, strRefSize, blobRefSize,
					      guidRefSize, FieldDescriptions[type]);
	}
}

/*
 * Write a 16-bit little-endian value to a buffer.
 */
#define	WRITE16(buf,value)	\
			do { \
				(buf)[0] = (unsigned char)(value); \
				(buf)[1] = (unsigned char)((value) >> 8); \
			} while (0)

/*
 * Write a 32-bit little-endian value to a buffer.
 */
#define	WRITE32(buf,value)	\
			do { \
				(buf)[0] = (unsigned char)(value); \
				(buf)[1] = (unsigned char)((value) >> 8); \
				(buf)[2] = (unsigned char)((value) >> 16); \
				(buf)[3] = (unsigned char)((value) >> 24); \
			} while (0)

void _ILImageRawTokenEncode(ILImage *image, unsigned char *ptr,
							ILToken token, ILUInt32 *values)
{
	const ILUInt32 * const *desc;
	int strRefBig = image->strRefBig;
	int blobRefBig = image->blobRefBig;
	int guidRefBig = image->guidRefBig;
	const ILUInt32 *type;
	const ILUInt32 *start;
	int index;
	ILUInt32 val;
	ILUInt32 val2;
	ILUInt32 temp;
	ILUInt32 limit;
	int bigToken;

	/* Get the descriptor to use to pack the data */
	desc = FieldDescriptions[token >> 24];
	if(!desc)
	{
	 	return;
	}

	/* Pack the data into the buffer */
	index = 0;
	while((type = *desc++) != END_FIELD)
	{
		if((((ILUInt32)(ILNativeUInt)type) & 1) != 0)
		{
			/* Simple type, or direct token table index */
			switch((ILUInt32)(ILNativeUInt)type)
			{
				case (ILUInt32)(ILNativeUInt)STRREF_FIELD:
				{
					/* Write a string index */
					if(strRefBig)
					{
						WRITE32(ptr, values[index]);
						ptr += 4;
					}
					else
					{
						WRITE16(ptr, values[index]);
						ptr += 2;
					}
					++index;
				}
				break;

				case (ILUInt32)(ILNativeUInt)BLOBREF_FIELD:
				{
					/* Write a blob index */
					if(blobRefBig)
					{
						WRITE32(ptr, values[index]);
						ptr += 4;
					}
					else
					{
						WRITE16(ptr, values[index]);
						ptr += 2;
					}
					index += 3;	/* Raw, value, and len */
				}
				break;

				case (ILUInt32)(ILNativeUInt)GUIDREF_FIELD:
				{
					/* Write a GUID reference */
					if(values[index] == IL_MAX_UINT32)
					{
						val = 0;
					}
					else
					{
						val = (values[index] / 16) + 1;
					}
					if(guidRefBig)
					{
						WRITE32(ptr, val);
						ptr += 4;
					}
					else
					{
						WRITE16(ptr, val);
						ptr += 2;
					}
					++index;
				}
				break;

				case (ILUInt32)(ILNativeUInt)UINT16_FIELD:
				{
					WRITE16(ptr, values[index]);
					++index;
					ptr += 2;
				}
				break;

				case (ILUInt32)(ILNativeUInt)UINT32_FIELD:
				{
					WRITE32(ptr, values[index]);
					++index;
					ptr += 4;
				}
				break;

				case (ILUInt32)(ILNativeUInt)FIELD_FIELD:
				{
					/* Write a field index */
					limit = image->tokenCount
								[IL_META_TOKEN_FIELD_DEF >> 24];
					val = values[index++];
					if(!val)
					{
						val = IL_META_TOKEN_FIELD_DEF | limit;
					}
					if(limit > (ILUInt32)0xFFFE)
					{
						WRITE32(ptr, val);
						ptr += 4;
					}
					else
					{
						WRITE16(ptr, val);
						ptr += 2;
					}
				}
				break;

				case (ILUInt32)(ILNativeUInt)METHOD_FIELD:
				{
					/* Write a method index */
					limit = image->tokenCount
								[IL_META_TOKEN_METHOD_DEF >> 24];
					val = values[index++];
					if(!val)
					{
						val = IL_META_TOKEN_METHOD_DEF | limit;
					}
					if(limit > (ILUInt32)0xFFFE)
					{
						WRITE32(ptr, val);
						ptr += 4;
					}
					else
					{
						WRITE16(ptr, val);
						ptr += 2;
					}
				}
				break;

				case (ILUInt32)(ILNativeUInt)PARAM_FIELD:
				{
					/* Write a parameter index */
					limit = image->tokenCount
								[IL_META_TOKEN_PARAM_DEF >> 24];
					val = values[index++];
					if(!val)
					{
						val = IL_META_TOKEN_PARAM_DEF | limit;
					}
					if(limit > (ILUInt32)0xFFFE)
					{
						WRITE32(ptr, val);
						ptr += 4;
					}
					else
					{
						WRITE16(ptr, val);
						ptr += 2;
					}
				}
				break;

				case (ILUInt32)(ILNativeUInt)OPTGEN_FIELD:
				{
					/* If this image has the GenericConstraint
					   table, then skip the next field */
					if(image->tokenCount
							[IL_META_TOKEN_GENERIC_CONSTRAINT >> 24] > 0)
					{
						++index;
						++desc;
					}
				}
				break;

				default:
				{
					/* Write a normal token table index */
					limit = image->tokenCount
						[((ILUInt32)(ILNativeUInt)type) >> 24];
					val = values[index++];
					if(limit <= (ILUInt32)0xFFFF)
					{
						WRITE16(ptr, val);
						ptr += 2;
					}
					else
					{
						WRITE32(ptr, val);
						ptr += 4;
					}
				}
				break;
			}
		}
		else
		{
			/* Mixed reference type */
			start = type;
			limit = (((ILUInt32)0xFFFF) >> *type++);
			bigToken = 0;
			val = 0;
			val2 = 0;
			while((temp = *type++) != END_DESC)
			{
				if(temp != 1 && temp != 2 &&
				   image->tokenCount[temp >> 24] > limit)
				{
					bigToken = 1;
				}
				if(temp == (values[index] & IL_META_TOKEN_MASK))
				{
					val = val2;
				}
				if(temp == 2 &&
				   (values[index] & IL_META_TOKEN_MASK) ==
				   			IL_META_TOKEN_STRING)
				{
					val = val2;
				}
				++val2;
			}
			val |= ((values[index++] & ~IL_META_TOKEN_MASK) << *start);
			if(bigToken)
			{
				/* The reference is 4 bytes in size */
				WRITE32(ptr, val);
				ptr += 4;
			}
			else
			{
				/* The reference is 2 bytes in size */
				WRITE16(ptr, val);
				ptr += 2;
			}
		}
	}
}

#ifdef	__cplusplus
};
#endif
