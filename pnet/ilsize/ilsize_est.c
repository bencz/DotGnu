/*
 * ilsize_est.c - Size estimation for metadata classes.
 *
 * Copyright (C) 2003, 2009  Southern Storm Software, Pty Ltd.
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
#include "il_dumpasm.h"
#include "../image/image.h"
#include "../image/program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Collected size information structure.
 */
typedef struct
{
	unsigned long	meta;			/* On-disk metadata size */
	unsigned long	loadedMeta;		/* In-memory metadata size */
	unsigned long	code;			/* Bytes of method code */

} ILSizeInfo;

/*
 * Get string size information.
 */
static void GetStringSize(ILSizeInfo *info, const char *str)
{
	if(str)
	{
		info->meta += strlen(str) + 1;
	}
}

/*
 * Get blob size information.
 */
static void GetBlobSize(ILSizeInfo *info, ILProgramItem *item, ILUInt32 offset)
{
	ILUInt32 len;
	unsigned char lenbuf[IL_META_COMPRESS_MAX_SIZE];
	if(ILImageGetBlob(ILProgramItem_Image(item), offset, &len))
	{
		info->meta += len;
		info->meta += (unsigned long)(long)ILMetaCompressData(lenbuf, len);
	}
}

/*
 * Get class name size information.
 */
static void GetClassNameSize(ILSizeInfo *info, ILClass *classInfo)
{
	info->meta += sizeof(ILClassName);
	info->meta += strlen(ILClass_Name(classInfo)) + 1;
	if(ILClass_Namespace(classInfo))
	{
		info->meta += strlen(ILClass_Namespace(classInfo)) + 1;
	}
}

/*
 * Get type size information.
 */
static void GetTypeSize(ILSizeInfo *info, ILType *type)
{
	unsigned long num;
	unsigned long posn;

	/* Only complex types have a non-zero memory size */
	if(type == 0 || !ILType_IsComplex(type))
	{
		return;
	}

	/* Account for the size of the complex type header */
	info->loadedMeta += sizeof(ILType);

	/* Account for element information */
	switch(ILType_Kind(type))
	{
		case IL_TYPE_COMPLEX_BYREF:
		case IL_TYPE_COMPLEX_PTR:
		case IL_TYPE_COMPLEX_PINNED:
		{
			GetTypeSize(info, ILType_Ref(type));
		}
		break;

		case IL_TYPE_COMPLEX_ARRAY:
		case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
		{
			GetTypeSize(info, ILType_ElemType(type));
		}
		break;

		case IL_TYPE_COMPLEX_CMOD_REQD:
		case IL_TYPE_COMPLEX_CMOD_OPT:
		{
			GetTypeSize(info, type->un.modifier__.type__);
		}
		break;

		case IL_TYPE_COMPLEX_LOCALS:
		{
			num = ILTypeNumLocals(type);
			for(posn = 0; posn < num; ++posn)
			{
				GetTypeSize(info, ILTypeGetLocalWithPrefixes(type, posn));
			}
		}
		break;

		case IL_TYPE_COMPLEX_WITH:
		{
			num = ILTypeNumWithParams(type);
			GetTypeSize(info, ILTypeGetWithMainWithPrefixes(type));
			for(posn = 1; posn <= num; ++posn)
			{
				GetTypeSize(info, ILTypeGetWithParamWithPrefixes(type, posn));
			}
		}
		break;

		case IL_TYPE_COMPLEX_PROPERTY:
		case IL_TYPE_COMPLEX_METHOD:
		case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
		{
			num = ILTypeNumParams(type);
			GetTypeSize(info, ILTypeGetReturnWithPrefixes(type));
			for(posn = 1; posn <= num; ++posn)
			{
				GetTypeSize(info, ILTypeGetParamWithPrefixes(type, posn));
			}
		}
		break;
	}
}

/*
 * Get the metadata size information for a program item,
 * both on-disk and in-memory.
 */
static void GetMetadataSize(ILSizeInfo *info, ILProgramItem *item)
{
	ILImage *image = ILProgramItem_Image(item);
	if(!item)
	{
		return;
	}
	info->meta += image->tokenSize[ILProgramItem_Token(item) >> 24];
	switch(ILProgramItem_Token(item) & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_MODULE:
			GetStringSize(info, ILModule_Name((ILModule *)item));
			info->meta += 16;		/* GUID size */
			info->loadedMeta += sizeof(ILModule);
			break;

		case IL_META_TOKEN_MODULE_REF:
			GetStringSize(info, ILModule_Name((ILModule *)item));
			info->loadedMeta += sizeof(ILModule);
			break;

		case IL_META_TOKEN_TYPE_REF:
			info->loadedMeta += sizeof(ILClass);
			info->loadedMeta += sizeof(ILProgramItemLink);
			GetClassNameSize(info, (ILClass *)item);
			break;

		case IL_META_TOKEN_TYPE_DEF:
			info->loadedMeta += sizeof(ILClass);
			GetClassNameSize(info, (ILClass *)item);
			break;

		case IL_META_TOKEN_FIELD_DEF:
			info->loadedMeta += sizeof(ILField);
			GetStringSize(info, ILMember_Name(item));
			GetBlobSize(info, item, ((ILMember *)item)->signatureBlob);
			GetTypeSize(info, ILMember_Signature(item));
			break;

		case IL_META_TOKEN_METHOD_DEF:
			info->loadedMeta += sizeof(ILMethod);
			GetStringSize(info, ILMember_Name(item));
			GetBlobSize(info, item, ((ILMember *)item)->signatureBlob);
			GetTypeSize(info, ILMember_Signature(item));
			break;

		case IL_META_TOKEN_PARAM_DEF:
			info->loadedMeta += sizeof(ILParameter);
			GetStringSize(info, ILParameter_Name((ILParameter *)item));
			break;

		case IL_META_TOKEN_INTERFACE_IMPL:
			info->loadedMeta += sizeof(ILImplements);
			break;

		case IL_META_TOKEN_MEMBER_REF:
			if(ILMemberGetKind((ILMember *)item) == IL_META_MEMBERKIND_METHOD)
			{
				info->loadedMeta += sizeof(ILMethod);
			}
			else
			{
				info->loadedMeta += sizeof(ILField);
			}
			info->loadedMeta += sizeof(ILProgramItemLink);
			GetStringSize(info, ILMember_Name(item));
			GetBlobSize(info, item, ((ILMember *)item)->signatureBlob);
			GetTypeSize(info, ILMember_Signature(item));
			break;

		case IL_META_TOKEN_CONSTANT:
			info->loadedMeta += sizeof(ILConstant);
			GetBlobSize(info, item, ((ILConstant *)item)->value);
			break;

		case IL_META_TOKEN_CUSTOM_ATTRIBUTE:
			info->loadedMeta += sizeof(ILAttribute);
			GetBlobSize(info, item, ((ILAttribute *)item)->value);
			break;

		case IL_META_TOKEN_FIELD_MARSHAL:
			info->loadedMeta += sizeof(ILFieldMarshal);
			GetBlobSize(info, item, ((ILFieldMarshal *)item)->type);
			break;

		case IL_META_TOKEN_DECL_SECURITY:
			info->loadedMeta += sizeof(ILDeclSecurity);
			GetBlobSize(info, item, ((ILDeclSecurity *)item)->blob);
			break;

		case IL_META_TOKEN_CLASS_LAYOUT:
			info->loadedMeta += sizeof(ILClassLayout);
			break;

		case IL_META_TOKEN_FIELD_LAYOUT:
			info->loadedMeta += sizeof(ILFieldLayout);
			break;

		case IL_META_TOKEN_STAND_ALONE_SIG:
			info->loadedMeta += sizeof(ILStandAloneSig);
			GetBlobSize(info, item, ((ILStandAloneSig *)item)->typeBlob);
			GetTypeSize(info, ((ILStandAloneSig *)item)->type);
			break;

		case IL_META_TOKEN_EVENT_MAP:
			info->loadedMeta += sizeof(ILEventMap);
			break;

		case IL_META_TOKEN_EVENT:
			info->loadedMeta += sizeof(ILEvent);
			break;

		case IL_META_TOKEN_PROPERTY_MAP:
			info->loadedMeta += sizeof(ILPropertyMap);
			break;

		case IL_META_TOKEN_PROPERTY:
			info->loadedMeta += sizeof(ILProperty);
			break;

		case IL_META_TOKEN_METHOD_SEMANTICS:
			info->loadedMeta += sizeof(ILMethodSem);
			break;

		case IL_META_TOKEN_METHOD_IMPL:
			info->loadedMeta += sizeof(ILOverride);
			break;

		case IL_META_TOKEN_TYPE_SPEC:
			info->loadedMeta += sizeof(ILTypeSpec);
			GetBlobSize(info, item, ((ILTypeSpec *)item)->typeBlob);
			GetTypeSize(info, ((ILTypeSpec *)item)->type);
			break;

		case IL_META_TOKEN_IMPL_MAP:
			info->loadedMeta += sizeof(ILPInvoke);
			GetStringSize(info, ILPInvoke_Alias((ILPInvoke *)item));
			break;

		case IL_META_TOKEN_FIELD_RVA:
			info->loadedMeta += sizeof(ILFieldRVA);
			break;

		case IL_META_TOKEN_ASSEMBLY:
			info->loadedMeta += sizeof(ILAssembly);
			GetStringSize(info, ILAssembly_Name((ILAssembly *)item));
			GetStringSize(info, ILAssembly_Locale((ILAssembly *)item));
			break;

		case IL_META_TOKEN_ASSEMBLY_REF:
			info->loadedMeta += sizeof(ILAssembly);
			info->loadedMeta += sizeof(ILProgramItemLink);
			GetStringSize(info, ILAssembly_Name((ILAssembly *)item));
			GetStringSize(info, ILAssembly_Locale((ILAssembly *)item));
			break;

		case IL_META_TOKEN_PROCESSOR_DEF:
		case IL_META_TOKEN_PROCESSOR_REF:
			info->loadedMeta += sizeof(ILProcessorInfo);
			break;

		case IL_META_TOKEN_OS_DEF:
		case IL_META_TOKEN_OS_REF:
			info->loadedMeta += sizeof(ILOSInfo);
			break;

		case IL_META_TOKEN_FILE:
			info->loadedMeta += sizeof(ILFileDecl);
			GetStringSize(info, ILFileDecl_Name((ILFileDecl *)item));
			GetBlobSize(info, item, ((ILFileDecl *)item)->hash);
			break;

		case IL_META_TOKEN_EXPORTED_TYPE:
			info->loadedMeta += sizeof(ILExportedType);
			info->loadedMeta += sizeof(ILProgramItemLink);
			GetClassNameSize(info, (ILClass *)item);
			break;

		case IL_META_TOKEN_MANIFEST_RESOURCE:
			info->loadedMeta += sizeof(ILManifestRes);
			GetStringSize(info, ILManifestRes_Name((ILManifestRes *)item));
			break;

		case IL_META_TOKEN_NESTED_CLASS:
			info->loadedMeta += sizeof(ILNestedInfo);
			break;

		case IL_META_TOKEN_GENERIC_PAR:
			info->loadedMeta += sizeof(ILGenericPar);
			GetStringSize(info, ILGenericPar_Name((ILGenericPar *)item));
			break;

		case IL_META_TOKEN_METHOD_SPEC:
			info->loadedMeta += sizeof(ILMethodSpec);
			GetBlobSize(info, item, ((ILMethodSpec *)item)->typeBlob);
			GetTypeSize(info, ((ILMethodSpec *)item)->type);
			break;
	}
}

/*
 * Get metadata size information for an item, plus all of its attributes.
 */
static void GetMetadataSizeWithAttrs(ILSizeInfo *info, ILProgramItem *item)
{
	ILAttribute *attr;
	ILDeclSecurity *decl;

	/* Get the basic size information for the item */
	GetMetadataSize(info, item);

	/* Collect up size information for the attributes */
	attr = 0;
	while((attr = ILProgramItemNextAttribute(item, attr)) != 0)
	{
		GetMetadataSize(info, ILToProgramItem(attr));
	}

	/* Account for the security declaration if there is one */
	decl = 0;
	while((decl = ILProgramItemNextDeclSecurity(item, decl)) != 0)
	{
		GetMetadataSize(info, ILToProgramItem(decl));
	}
}

/*
 * Get metadata size information for a method.
 */
static void GetMethodSize(ILSizeInfo *info, ILMethod *method)
{
	ILParameter *param;
	ILMethodCode code;

	/* Account for the parameters */
	param = 0;
	while((param = ILMethodNextParam(method, param)) != 0)
	{
		GetMetadataSizeWithAttrs(info, ILToProgramItem(param));
	}

	/* Get information about the method's code */
	if(ILMethodGetCode(method, &code))
	{
		info->code += code.codeLen;
		/* TODO: exception block information */
	}
}

/*
 * Get the full size information for a class and its members.
 */
static void GetClassSize(ILSizeInfo *info, ILClass *classInfo)
{
	ILImage *image = ILProgramItem_Image(classInfo);
	ILImplements *impl;
	ILClassLayout *layout;
	ILMember *member;
	int hasEvents, hasProperties;
	ILFieldLayout *fieldLayout;
	ILFieldRVA *fieldRVA;
	ILConstant *constant;
	ILUInt32 type;

	/* Get the size information for the class itself */
	GetMetadataSizeWithAttrs(info, ILToProgramItem(classInfo));

	/* Collect up size information for the interface declarations */
	impl = 0;
	while((impl = ILClassNextImplements(classInfo, impl)) != 0)
	{
		GetMetadataSize(info, ILToProgramItem(impl));
	}

	/* Account for class layout information */
	layout = ILClassLayoutGetFromOwner(classInfo);
	if(layout)
	{
		GetMetadataSize(info, ILToProgramItem(layout));
	}

	/* Account for the nested class declaration */
	if(ILClass_NestedParent(classInfo) != 0)
	{
		info->meta += image->tokenSize[IL_META_TOKEN_NESTED_CLASS >> 24];
		info->loadedMeta += sizeof(ILNestedInfo);
	}

	/* Collect up size information for the members */
	member = 0;
	hasEvents = 0;
	hasProperties = 0;
	while((member = ILClassNextMember(classInfo, member)) != 0)
	{
		/* Get the basic member size information */
		GetMetadataSizeWithAttrs(info, ILToProgramItem(member));

		/* Deal with additional information hanging off the member */
		switch(ILMemberGetKind(member))
		{
			case IL_META_MEMBERKIND_METHOD:
			{
				GetMethodSize(info, (ILMethod *)member);
			}
			break;

			case IL_META_MEMBERKIND_FIELD:
			{
				/* Account for field layout, RVA, and constant information */
				fieldLayout = ILFieldLayoutGetFromOwner((ILField *)member);
				if(fieldLayout)
				{
					GetMetadataSize(info, ILToProgramItem(fieldLayout));
				}
				fieldRVA = ILFieldRVAGetFromOwner((ILField *)member);
				if(fieldRVA)
				{
					GetMetadataSize(info, ILToProgramItem(fieldRVA));
				}
				constant = ILConstantGetFromOwner((ILProgramItem *)member);
				if(constant)
				{
					GetMetadataSize(info, ILToProgramItem(constant));
				}
			}
			break;

			case IL_META_MEMBERKIND_PROPERTY:
			case IL_META_MEMBERKIND_EVENT:
			{
				/* Account for the method semantics declarations */
				type = 0x8000;
				while(type != 0)
				{
					if(ILMethodSemGetByType(ILToProgramItem(member), type))
					{
						info->meta +=
							image->tokenSize
								[IL_META_TOKEN_METHOD_SEMANTICS >> 24];
						info->loadedMeta += sizeof(ILMethodSem);
					}
					type >>= 1;
				}
				if(ILMemberGetKind(member) == IL_META_MEMBERKIND_EVENT)
				{
					hasEvents = 1;
				}
				else
				{
					hasProperties = 1;
				}
			}
			break;
		}
	}

	/* If we have events or properties, then account for the map entries */
	if(hasEvents)
	{
		info->meta += image->tokenSize[IL_META_TOKEN_EVENT_MAP >> 24];
		info->loadedMeta += sizeof(ILEventMap);
	}
	if(hasProperties)
	{
		info->meta += image->tokenSize[IL_META_TOKEN_PROPERTY_MAP >> 24];
		info->loadedMeta += sizeof(ILPropertyMap);
	}
}

/*
 * Type the size information for the classes in an image.
 */
void _ILDumpClassSizes(ILImage *image)
{
	ILClass *classInfo = 0;
	ILSizeInfo info;
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
	{
		info.meta = 0;
		info.loadedMeta = 0;
		info.code = 0;
		GetClassSize(&info, classInfo);
		printf("%7lu %7lu %7lu ", info.meta, info.loadedMeta, info.code);
		ILDumpClassName(stdout, image, classInfo, 0);
		putc('\n', stdout);
	}
}

#ifdef	__cplusplus
};
#endif
