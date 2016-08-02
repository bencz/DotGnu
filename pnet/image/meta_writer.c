/*
 * meta_writer.c - Write metadata index informtion to an image.
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

#include "program.h"

#ifdef IL_USE_WRITER

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Convert a persistent string pointer into a string table offset.
 */
static ILUInt32 GetPersistString(ILImage *image, const char *str)
{
	if(!str)
	{
		/* Invalid strings are always converted into offset 0 */
		return 0;
	}
	else if(image->type == IL_IMAGETYPE_BUILDING)
	{
		/* Search the string block list for the pointer */
		ILStringBlock *block = image->stringBlocks;
		ILUInt32 offset = 0;
		while(block != 0)
		{
			if(str >= ((const char *)(block + 1)) &&
			   str < (((const char *)(block + 1)) + block->used))
			{
				return offset + (ILUInt32)(str - ((const char *)(block + 1)));
			}
			offset += block->used;
			block = block->next;
		}
		return 0;
	}
	else
	{
		/* Offset into the string pool of a loaded image */
		return (ILUInt32)(str - ((const char *)(image->stringPool)));
	}
}

/*
 * Format a Module token.
 */
static void Format_Module(ILWriter *writer, ILImage *image,
						  ILUInt32 *values, ILModule *module)
{
	if(module->enc)
	{
		/* We have Edit & Continue information */
		values[IL_OFFSET_MODULE_GENERATION] = module->enc->generation;
		if((module->enc->flags & 1) != 0)
		{
			values[IL_OFFSET_MODULE_ENCID] = writer->guidBlob.offset;
			_ILWBufferListAdd(&(writer->guidBlob), module->enc->encId, 16);
		}
		else
		{
			values[IL_OFFSET_MODULE_ENCID] = IL_MAX_UINT32;
		}
		if((module->enc->flags & 2) != 0)
		{
			values[IL_OFFSET_MODULE_ENCBASEID] = writer->guidBlob.offset;
			_ILWBufferListAdd(&(writer->guidBlob), module->enc->encBaseId, 16);
		}
		else
		{
			values[IL_OFFSET_MODULE_ENCBASEID] = IL_MAX_UINT32;
		}
	}
	else
	{
		/* Simple module information */
		values[IL_OFFSET_MODULE_GENERATION] = 0;
		values[IL_OFFSET_MODULE_ENCID] = IL_MAX_UINT32;
		values[IL_OFFSET_MODULE_ENCBASEID] = IL_MAX_UINT32;
	}
	values[IL_OFFSET_MODULE_NAME] = GetPersistString(image, module->name);
	values[IL_OFFSET_MODULE_MVID] = writer->guidBlob.offset;
	_ILWBufferListAdd(&(writer->guidBlob), module->mvid, 16);
}

/*
 * Format a TypeRef token.
 */
static void Format_TypeRef(ILWriter *writer, ILImage *image,
						   ILUInt32 *values, ILClass *info)
{
	ILClass *nestedParent = ILClass_NestedParent(info);
	if(nestedParent)
	{
		/* Nested within something else */
		if(ILClassIsRef(nestedParent))
		{
			values[IL_OFFSET_TYPEREF_SCOPE] = ILClass_Token(nestedParent);
		}
		else
		{
			/* This is the annoying case: a nested TypeRef has has
			   subsequently been turned into a TypeDef.  For now,
			   import from an unknown scope.  Fix this later */
			values[IL_OFFSET_TYPEREF_SCOPE] = 0;
		}
	}
	else if(info->className->scope)
	{
		/* Refers to a type in a foreign scope */
		values[IL_OFFSET_TYPEREF_SCOPE] = info->className->scope->token;
	}
	else if(!ILClassIsRef(info))
	{
		/* This used to be a TypeRef, but now refers to a local type */
		values[IL_OFFSET_TYPEREF_SCOPE] = (IL_META_TOKEN_MODULE | 1);
	}
	else
	{
		/* Unknown scope */
		values[IL_OFFSET_TYPEREF_SCOPE] = 0;
	}
	values[IL_OFFSET_TYPEREF_NAME] =
			GetPersistString(image, info->className->name);
	values[IL_OFFSET_TYPEREF_NAMESPACE] =
			GetPersistString(image, info->className->namespace);
}

/*
 * Search the TypeSpec table for a particular type.
 */
static ILToken TypeToToken(ILImage *image, ILType *type)
{
	ILTypeSpec *spec = 0;
	while((spec = (ILTypeSpec *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_SPEC, spec)) != 0)
	{
		if(ILTypeIdentical(ILTypeSpec_Type(spec), type))
		{
			return ILTypeSpec_Token(spec);
		}
	}
	return 0;
}

/*
 * Convert an ILClass into a TypeRef, TypeDef, or TypeSpec token.
 */
static ILToken ClassToToken(ILImage *image, ILClass *info)
{
	if(!(info->synthetic))
	{
		return info->programItem.token;
	}
	else
	{
		return TypeToToken(image, info->synthetic);
	}
}

static ILToken ProgramItemToToken(ILImage *image, ILProgramItem *item)
{
	ILClass *info;
	ILTypeSpec *spec;

	if((info = _ILProgramItem_ToClass(item)) != 0)
	{
		return ClassToToken(image, info);
	}
	else if((spec = _ILProgramItem_ToTypeSpec(item)) != 0)
	{
		return ILTypeSpec_Token(spec);
	}
	return 0;
}

/*
 * Format a TypeDef token.
 */
static void Format_TypeDef(ILWriter *writer, ILImage *image,
						   ILUInt32 *values, ILClass *info)
{
	ILToken token;
	ILToken maxToken;
	ILMember *member;
	int haveField, haveMethod;

	/* Set the easy fields */
	values[IL_OFFSET_TYPEDEF_ATTRS] =
		(info->attributes & ~IL_META_TYPEDEF_SYSTEM_MASK);
	values[IL_OFFSET_TYPEDEF_NAME] =
			GetPersistString(image, info->className->name);
	values[IL_OFFSET_TYPEDEF_NAMESPACE] =
			GetPersistString(image, info->className->namespace);
	if(info->parent)
	{
		values[IL_OFFSET_TYPEDEF_PARENT] = ProgramItemToToken(image, info->parent);
	}
	else
	{
		values[IL_OFFSET_TYPEDEF_PARENT] = 0;
	}
	values[IL_OFFSET_TYPEDEF_FIRST_FIELD] =
			((image->tokenCount[IL_META_TOKEN_FIELD_DEF >> 24] + 1) |
					IL_META_TOKEN_FIELD_DEF);
	values[IL_OFFSET_TYPEDEF_FIRST_METHOD] =
			((image->tokenCount[IL_META_TOKEN_METHOD_DEF >> 24] + 1) |
					IL_META_TOKEN_METHOD_DEF);

	/* Search for the field and method identifiers */
	token = info->programItem.token;
	maxToken = (image->tokenCount[IL_META_TOKEN_TYPE_DEF >> 24] |
					IL_META_TOKEN_TYPE_DEF);
	haveField = 0;
	haveMethod = 0;
	while(token <= maxToken && (!haveField || !haveMethod))
	{
		info = image->tokenData[IL_META_TOKEN_TYPE_DEF >> 24]
							   [(token & ~IL_META_TOKEN_MASK) - 1];
		if(!info)
		{
			/* Invalid data in the TypeDef table */
			return;
		}
		if(!haveField)
		{
			member = ILClassNextMemberByKind
							(info, 0, IL_META_MEMBERKIND_FIELD);
			if(member &&
			   (member->programItem.token & IL_META_TOKEN_MASK)
			   		== IL_META_TOKEN_FIELD_DEF)
			{
				values[IL_OFFSET_TYPEDEF_FIRST_FIELD] =
						member->programItem.token;
				haveField = 1;
			}
		}
		if(!haveMethod)
		{
			member = ILClassNextMemberByKind
							(info, 0, IL_META_MEMBERKIND_METHOD);
			if(member &&
			   (member->programItem.token & IL_META_TOKEN_MASK)
			   		== IL_META_TOKEN_METHOD_DEF)
			{
				values[IL_OFFSET_TYPEDEF_FIRST_METHOD] =
						member->programItem.token;
				haveMethod = 1;
			}
		}
		++token;
	}
}

/*
 * Format a FieldDef token.
 */
static void Format_FieldDef(ILWriter *writer, ILImage *image,
						    ILUInt32 *values, ILField *field)
{
	values[IL_OFFSET_FIELDDEF_ATTRS] = field->member.attributes;
	values[IL_OFFSET_FIELDDEF_NAME] =
			GetPersistString(image, field->member.name);
	values[IL_OFFSET_FIELDDEF_SIGNATURE_RAW] = field->member.signatureBlob;
}

/*
 * Format a MethodDef token.
 */
static void Format_MethodDef(ILWriter *writer, ILImage *image,
						     ILUInt32 *values, ILMethod *method)
{
	ILToken token;
	ILToken maxToken;

	/* Set the easy fields */
	values[IL_OFFSET_METHODDEF_RVA] = method->rva;
	values[IL_OFFSET_METHODDEF_IMPL_ATTRS] = method->implementAttrs;
	values[IL_OFFSET_METHODDEF_ATTRS] = method->member.attributes;
	values[IL_OFFSET_METHODDEF_NAME] =
			GetPersistString(image, method->member.name);
	values[IL_OFFSET_METHODDEF_SIGNATURE_RAW] = method->member.signatureBlob;
	values[IL_OFFSET_METHODDEF_FIRST_PARAM] =
			((image->tokenCount[IL_META_TOKEN_PARAM_DEF >> 24] + 1) |
					IL_META_TOKEN_PARAM_DEF);

	/* Get the index of the first parameter.  We may need to
	   search for the next method in the image if this one
	   does not have any parameters associated with it */
	token = method->member.programItem.token;
	maxToken = (image->tokenCount[IL_META_TOKEN_METHOD_DEF >> 24] |
					IL_META_TOKEN_METHOD_DEF);
	while(token <= maxToken)
	{
		method = image->tokenData[IL_META_TOKEN_METHOD_DEF >> 24]
							     [(token & ~IL_META_TOKEN_MASK) - 1];
		if(!method)
		{
			/* Invalid data in the MethodDef table */
			return;
		}
		if(method->parameters)
		{
			values[IL_OFFSET_METHODDEF_FIRST_PARAM] =
					method->parameters->programItem.token;
			break;
		}
		++token;
	}
}

/*
 * Format a ParamDef token.
 */
static void Format_ParamDef(ILWriter *writer, ILImage *image,
						    ILUInt32 *values, ILParameter *param)
{
	values[IL_OFFSET_PARAMDEF_ATTRS] = param->attributes;
	values[IL_OFFSET_PARAMDEF_NUMBER] = param->paramNum;
	values[IL_OFFSET_PARAMDEF_NAME] = GetPersistString(image, param->name);
}

/*
 * Format a InterfaceImpl token.
 */
static void Format_InterfaceImpl(ILWriter *writer, ILImage *image,
						 		 ILUInt32 *values, ILImplements *impl)
{
	values[IL_OFFSET_INTERFACE_TYPE] = impl->implement->programItem.token;
	values[IL_OFFSET_INTERFACE_INTERFACE] =
			ProgramItemToToken(image, impl->interface);
}

/*
 * Format a MemberRef token.
 */
static void Format_MemberRef(ILWriter *writer, ILImage *image,
					 		 ILUInt32 *values, ILMember *member)
{
	ILMemberRef *ref;
	values[IL_OFFSET_MEMBERREF_NAME] = GetPersistString(image, member->name);
	values[IL_OFFSET_MEMBERREF_SIGNATURE_RAW] = member->signatureBlob;
	if(member->kind == IL_META_MEMBERKIND_REF)
	{
		ref = (ILMemberRef *)member;
		values[IL_OFFSET_MEMBERREF_PARENT] =
				ClassToToken(image, ref->ref->owner);
	}
	else
	{
		values[IL_OFFSET_MEMBERREF_PARENT] =
				ClassToToken(image, member->owner);
	}
}

/*
 * Format a Constant token.
 */
static void Format_Constant(ILWriter *writer, ILImage *image,
					 		ILUInt32 *values, ILConstant *constant)
{
	values[IL_OFFSET_CONSTANT_TYPE] = constant->elemType;
	values[IL_OFFSET_CONSTANT_REFERENCE] = constant->ownedItem.owner->token;
	values[IL_OFFSET_CONSTANT_DATA_RAW] = constant->value;
}

/*
 * Format a CustomAttribute token.
 */
static void Format_CustomAttr(ILWriter *writer, ILImage *image,
					 		  ILUInt32 *values, ILAttribute *attr)
{
	values[IL_OFFSET_CUSTOMATTR_OWNER] = attr->owner->token;
	if(attr->type)
	{
		values[IL_OFFSET_CUSTOMATTR_NAME] = attr->type->token;
	}
	else
	{
		values[IL_OFFSET_CUSTOMATTR_NAME] = IL_META_TOKEN_STRING;
	}
	values[IL_OFFSET_CUSTOMATTR_DATA_RAW] = attr->value;
}

/*
 * Format a FieldMarshal token.
 */
static void Format_FieldMarshal(ILWriter *writer, ILImage *image,
					 		    ILUInt32 *values, ILFieldMarshal *marshal)
{
	values[IL_OFFSET_FIELDMARSHAL_TOKEN] = marshal->ownedItem.owner->token;
	values[IL_OFFSET_FIELDMARSHAL_TYPE_RAW] = marshal->type;
}

/*
 * Format a DeclSecurity token.
 */
static void Format_DeclSecurity(ILWriter *writer, ILImage *image,
					 		    ILUInt32 *values, ILDeclSecurity *security)
{
	values[IL_OFFSET_DECLSECURITY_TYPE] = security->type;
	values[IL_OFFSET_DECLSECURITY_TOKEN] = security->ownedItem.owner->token;
	values[IL_OFFSET_DECLSECURITY_DATA_RAW] = security->blob;
}

/*
 * Format a ClassLayout token.
 */
static void Format_ClassLayout(ILWriter *writer, ILImage *image,
					 		   ILUInt32 *values, ILClassLayout *layout)
{
	values[IL_OFFSET_CLASSLAYOUT_PACKING] = layout->packingSize;
	values[IL_OFFSET_CLASSLAYOUT_SIZE] = layout->classSize;
	values[IL_OFFSET_CLASSLAYOUT_TYPE] = layout->ownedItem.owner->token;
}

/*
 * Format a FieldLayout token.
 */
static void Format_FieldLayout(ILWriter *writer, ILImage *image,
					 		   ILUInt32 *values, ILFieldLayout *layout)
{
	values[IL_OFFSET_FIELDLAYOUT_OFFSET] = layout->offset;
	values[IL_OFFSET_FIELDLAYOUT_FIELD] = layout->ownedItem.owner->token;
}

/*
 * Format a StandAloneSig token.
 */
static void Format_StandAloneSig(ILWriter *writer, ILImage *image,
					 		     ILUInt32 *values, ILStandAloneSig *sig)
{
	values[IL_OFFSET_SIGNATURE_VALUE_RAW] = sig->typeBlob;
}

/*
 * Format an EventMap token.
 */
static void Format_EventMap(ILWriter *writer, ILImage *image,
			 		        ILUInt32 *values, ILEventMap *map)
{
	values[IL_OFFSET_EVENTMAP_TYPE] = map->classInfo->programItem.token;
	values[IL_OFFSET_EVENTMAP_EVENT] =
			map->firstEvent->member.programItem.token;
}

/*
 * Format an Event token.
 */
static void Format_Event(ILWriter *writer, ILImage *image,
			 		     ILUInt32 *values, ILEvent *event)
{
	values[IL_OFFSET_EVENT_ATTRS] = event->member.attributes;
	values[IL_OFFSET_EVENT_NAME] = GetPersistString(image, event->member.name);
	if(ILType_IsClass(event->member.signature))
	{
		values[IL_OFFSET_EVENT_TYPE] =
				ILType_ToClass(event->member.signature)->programItem.token;
	}
	else
	{
		values[IL_OFFSET_EVENT_TYPE] =
				TypeToToken(image, event->member.signature);
	}
}

/*
 * Format a PropertyMap token.
 */
static void Format_PropertyMap(ILWriter *writer, ILImage *image,
			 		           ILUInt32 *values, ILPropertyMap *map)
{
	values[IL_OFFSET_PROPMAP_TYPE] = map->classInfo->programItem.token;
	values[IL_OFFSET_PROPMAP_PROPERTY] =
			map->firstProperty->member.programItem.token;
}

/*
 * Format a Property token.
 */
static void Format_Property(ILWriter *writer, ILImage *image,
			 		        ILUInt32 *values, ILProperty *property)
{
	values[IL_OFFSET_PROPERTY_ATTRS] = property->member.attributes;
	values[IL_OFFSET_PROPERTY_NAME] =
			GetPersistString(image, property->member.name);
	values[IL_OFFSET_PROPERTY_SIGNATURE_RAW] = property->member.signatureBlob;
}

/*
 * Format a MethodSemantics token.
 */
static void Format_MethodSemantics(ILWriter *writer, ILImage *image,
			 		        	   ILUInt32 *values, ILMethodSem *sem)
{
	values[IL_OFFSET_METHODSEM_SEMANTICS] = sem->type;
	values[IL_OFFSET_METHODSEM_METHOD] = sem->method->member.programItem.token;
	values[IL_OFFSET_METHODSEM_OWNER] = sem->owner->token;
}

/*
 * Format a MethodImpl token.
 */
static void Format_MethodImpl(ILWriter *writer, ILImage *image,
			 		          ILUInt32 *values, ILOverride *over)
{
	values[IL_OFFSET_METHODIMPL_TYPE] = over->member.owner->programItem.token;
	values[IL_OFFSET_METHODIMPL_METHOD_1] =
			over->body->member.programItem.token;
	values[IL_OFFSET_METHODIMPL_METHOD_2] =
			over->decl->member.programItem.token;
}

/*
 * Format a ModuleRef token.
 */
static void Format_ModuleRef(ILWriter *writer, ILImage *image,
			 		         ILUInt32 *values, ILModule *module)
{
	values[IL_OFFSET_MODULEREF_NAME] = GetPersistString(image, module->name);
}

/*
 * Format a TypeSpec token.
 */
static void Format_TypeSpec(ILWriter *writer, ILImage *image,
			 		        ILUInt32 *values, ILTypeSpec *spec)
{
	values[IL_OFFSET_TYPESPEC_TYPE_RAW] = spec->typeBlob;
}

/*
 * Format an ImplMap token.
 */
static void Format_ImplMap(ILWriter *writer, ILImage *image,
			 		       ILUInt32 *values, ILPInvoke *pinvoke)
{
	values[IL_OFFSET_IMPLMAP_ATTRS] = pinvoke->member.attributes;
	values[IL_OFFSET_IMPLMAP_METHOD] =
			pinvoke->memberInfo->programItem.token;
	values[IL_OFFSET_IMPLMAP_ALIAS] =
			GetPersistString(image, pinvoke->aliasName);
	values[IL_OFFSET_IMPLMAP_MODULE] = pinvoke->module->programItem.token;
}

/*
 * Format a FieldRVA token.
 */
static void Format_FieldRVA(ILWriter *writer, ILImage *image,
			 		        ILUInt32 *values, ILFieldRVA *rva)
{
	values[IL_OFFSET_FIELDRVA_RVA] = rva->rva;
	values[IL_OFFSET_FIELDRVA_FIELD] = rva->ownedItem.owner->token;
}

/*
 * Format an Assembly token.
 */
static void Format_Assembly(ILWriter *writer, ILImage *image,
			 		        ILUInt32 *values, ILAssembly *assem)
{
	values[IL_OFFSET_ASSEMBLY_HASHALG] = assem->hashAlgorithm;
	values[IL_OFFSET_ASSEMBLY_VER_1] = assem->version[0];
	values[IL_OFFSET_ASSEMBLY_VER_2] = assem->version[1];
	values[IL_OFFSET_ASSEMBLY_VER_3] = assem->version[2];
	values[IL_OFFSET_ASSEMBLY_VER_4] = assem->version[3];
	values[IL_OFFSET_ASSEMBLY_ATTRS] = assem->attributes;
	values[IL_OFFSET_ASSEMBLY_KEY_RAW] = assem->originator;
	values[IL_OFFSET_ASSEMBLY_NAME] = GetPersistString(image, assem->name);
	values[IL_OFFSET_ASSEMBLY_LOCALE] = GetPersistString(image, assem->locale);
}

/*
 * Format a ProcessorDef token.
 */
static void Format_ProcessorDef(ILWriter *writer, ILImage *image,
			 		            ILUInt32 *values, ILProcessorInfo *procinfo)
{
	values[IL_OFFSET_PROCESSORDEF_NUM] = procinfo->number;
}

/*
 * Format an OSDef token.
 */
static void Format_OSDef(ILWriter *writer, ILImage *image,
	 		             ILUInt32 *values, ILOSInfo *osinfo)
{
	values[IL_OFFSET_OSDEF_IDENTIFIER] = osinfo->identifier;
	values[IL_OFFSET_OSDEF_MAJOR] = osinfo->major;
	values[IL_OFFSET_OSDEF_MINOR] = osinfo->minor;
}

/*
 * Format an AssemblyRef token.
 */
static void Format_AssemblyRef(ILWriter *writer, ILImage *image,
			 		           ILUInt32 *values, ILAssembly *assem)
{
	values[IL_OFFSET_ASSEMBLYREF_VER_1] = assem->version[0];
	values[IL_OFFSET_ASSEMBLYREF_VER_2] = assem->version[1];
	values[IL_OFFSET_ASSEMBLYREF_VER_3] = assem->version[2];
	values[IL_OFFSET_ASSEMBLYREF_VER_4] = assem->version[3];
	values[IL_OFFSET_ASSEMBLYREF_ATTRS] = assem->refAttributes;
	values[IL_OFFSET_ASSEMBLYREF_KEY_RAW] = assem->originator;
	values[IL_OFFSET_ASSEMBLYREF_NAME] = GetPersistString(image, assem->name);
	values[IL_OFFSET_ASSEMBLYREF_LOCALE] =
			GetPersistString(image, assem->locale);
	values[IL_OFFSET_ASSEMBLYREF_HASH_RAW] = assem->hashValue;
}

/*
 * Format a ProcessorRef token.
 */
static void Format_ProcessorRef(ILWriter *writer, ILImage *image,
			 		            ILUInt32 *values, ILProcessorInfo *procinfo)
{
	values[IL_OFFSET_PROCESSORREF_NUM] = procinfo->number;
	values[IL_OFFSET_PROCESSORREF_ASSEMBLY] =
			procinfo->assembly->programItem.token;
}

/*
 * Format an OSRef token.
 */
static void Format_OSRef(ILWriter *writer, ILImage *image,
	 		             ILUInt32 *values, ILOSInfo *osinfo)
{
	values[IL_OFFSET_OSREF_IDENTIFIER] = osinfo->identifier;
	values[IL_OFFSET_OSREF_MAJOR] = osinfo->major;
	values[IL_OFFSET_OSREF_MINOR] = osinfo->minor;
	values[IL_OFFSET_OSREF_ASSEMBLY] =
			osinfo->assembly->programItem.token;
}

/*
 * Format a File token.
 */
static void Format_File(ILWriter *writer, ILImage *image,
	 		            ILUInt32 *values, ILFileDecl *decl)
{
	values[IL_OFFSET_FILE_ATTRS] = decl->attributes;
	values[IL_OFFSET_FILE_NAME] = GetPersistString(image, decl->name);
	values[IL_OFFSET_FILE_HASH_RAW] = decl->hash;
}

/*
 * Format an ExportedType token.
 */
static void Format_ExportedType(ILWriter *writer, ILImage *image,
	 		            		ILUInt32 *values, ILExportedType *type)
{
	values[IL_OFFSET_EXPTYPE_ATTRS] = type->classItem.attributes;
	values[IL_OFFSET_EXPTYPE_CLASS] = type->identifier;
	values[IL_OFFSET_EXPTYPE_NAME] =
			GetPersistString(image, type->classItem.className->name);
	values[IL_OFFSET_EXPTYPE_NAMESPACE] =
			GetPersistString(image, type->classItem.className->namespace);
	if(type->classItem.className->scope)
	{
		values[IL_OFFSET_EXPTYPE_FILE] =
			type->classItem.className->scope->token;
	}
	else
	{
		values[IL_OFFSET_EXPTYPE_FILE] = 0;
	}
}

/*
 * Format a ManifestResource token.
 */
static void Format_ManifestResource(ILWriter *writer, ILImage *image,
	 		            		    ILUInt32 *values, ILManifestRes *res)
{
	values[IL_OFFSET_MANIFESTRES_OFFSET] = res->offset;
	values[IL_OFFSET_MANIFESTRES_ATTRS] = res->attributes;
	values[IL_OFFSET_MANIFESTRES_NAME] = GetPersistString(image, res->name);
	values[IL_OFFSET_MANIFESTRES_IMPL] =
			(res->owner ? res->owner->token : 0);
}

/*
 * Format a NestedClass token.
 */
static void Format_NestedClass(ILWriter *writer, ILImage *image,
	 		                   ILUInt32 *values, ILNestedInfo *nested)
{
	values[IL_OFFSET_NESTEDCLASS_CHILD] = nested->child->programItem.token;
	values[IL_OFFSET_NESTEDCLASS_PARENT] = nested->parent->programItem.token;
}

/*
 * Format a GenericPar token.
 */
static void Format_GenericPar(ILWriter *writer, ILImage *image,
	 		                  ILUInt32 *values, ILGenericPar *genPar)
{
	values[IL_OFFSET_GENERICPAR_NUMBER] = genPar->number;
	values[IL_OFFSET_GENERICPAR_FLAGS] = genPar->flags;
	values[IL_OFFSET_GENERICPAR_OWNER] = genPar->ownedItem.owner->token;
	values[IL_OFFSET_GENERICPAR_NAME] = GetPersistString(image, genPar->name);
}

/*
 * Format a GenericConstraint token.
 */
static void Format_GenericConstraint(ILWriter *writer, ILImage *image,
	 		                         ILUInt32 *values,
									 ILGenericConstraint *genCon)
{
	values[IL_OFFSET_GENERICCON_PARAM] =
			(genCon->parameter ? ((ILProgramItem *)genCon->parameter)->token : 0);
	values[IL_OFFSET_GENERICCON_CONSTRAINT] =
			(genCon->constraint ? genCon->constraint->token : 0);
}

/*
 * Format a MethodSpec token.
 */
static void Format_MethodSpec(ILWriter *writer, ILImage *image,
	 		                  ILUInt32 *values, ILMethodSpec *spec)
{
	values[IL_OFFSET_METHODSPEC_METHOD] = spec->method->programItem.token;
	values[IL_OFFSET_METHODSPEC_INST_RAW] = spec->typeBlob;
}

/*
 * Array of all formatting routines for the known token types.
 */
typedef void (*ILFormatFunc)(ILWriter *writer, ILImage *image,
							 ILUInt32 *values, void *data);
static ILFormatFunc const Formatters[64] = {
	(ILFormatFunc)Format_Module,				/* 00 */
	(ILFormatFunc)Format_TypeRef,
	(ILFormatFunc)Format_TypeDef,
	0,
	(ILFormatFunc)Format_FieldDef,
	0,
	(ILFormatFunc)Format_MethodDef,
	0,
	(ILFormatFunc)Format_ParamDef,				/* 08 */
	(ILFormatFunc)Format_InterfaceImpl,
	(ILFormatFunc)Format_MemberRef,
	(ILFormatFunc)Format_Constant,
	(ILFormatFunc)Format_CustomAttr,
	(ILFormatFunc)Format_FieldMarshal,
	(ILFormatFunc)Format_DeclSecurity,
	(ILFormatFunc)Format_ClassLayout,
	(ILFormatFunc)Format_FieldLayout,			/* 10 */
	(ILFormatFunc)Format_StandAloneSig,
	(ILFormatFunc)Format_EventMap,
	0,
	(ILFormatFunc)Format_Event,
	(ILFormatFunc)Format_PropertyMap,
	0,
	(ILFormatFunc)Format_Property,
	(ILFormatFunc)Format_MethodSemantics,		/* 18 */
	(ILFormatFunc)Format_MethodImpl,
	(ILFormatFunc)Format_ModuleRef,
	(ILFormatFunc)Format_TypeSpec,
	(ILFormatFunc)Format_ImplMap,
	(ILFormatFunc)Format_FieldRVA,
	0,
	0,
	(ILFormatFunc)Format_Assembly,				/* 20 */
	(ILFormatFunc)Format_ProcessorDef,
	(ILFormatFunc)Format_OSDef,
	(ILFormatFunc)Format_AssemblyRef,
	(ILFormatFunc)Format_ProcessorRef,
	(ILFormatFunc)Format_OSRef,
	(ILFormatFunc)Format_File,
	(ILFormatFunc)Format_ExportedType,
	(ILFormatFunc)Format_ManifestResource,		/* 28 */
	(ILFormatFunc)Format_NestedClass,
	(ILFormatFunc)Format_GenericPar,
	(ILFormatFunc)Format_MethodSpec,
	(ILFormatFunc)Format_GenericConstraint,
	0,
	0,
	0,
	0,											/* 30 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,											/* 38 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
};

/*
 * Write signature information for a field definition.
 */
static int SigWrite_FieldDef(ILField *field)
{
	if(field->member.signature && !(field->member.signatureBlob))
	{
		field->member.signatureBlob = ILTypeToFieldSig
				(field->member.programItem.image,
				 field->member.signature);
		if(!(field->member.signatureBlob))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Write signature information for a method definition.
 */
static int SigWrite_MethodDef(ILMethod *method)
{
	if(method->member.signature && !(method->member.signatureBlob))
	{
		method->member.signatureBlob = ILTypeToMethodSig
				(method->member.programItem.image,
				 method->member.signature);
		if(!(method->member.signatureBlob))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Write signature information for a member reference.
 */
static int SigWrite_MemberRef(ILMember *member)
{
	if(member->signature && !(member->signatureBlob))
	{
		if(member->kind == IL_META_MEMBERKIND_METHOD)
		{
			member->signatureBlob = ILTypeToMethodSig
					(member->programItem.image, member->signature);
		}
		else if(member->kind == IL_META_MEMBERKIND_FIELD)
		{
			member->signatureBlob = ILTypeToFieldSig
					(member->programItem.image, member->signature);
		}
		else if(member->kind == IL_META_MEMBERKIND_PROPERTY)
		{
			member->signatureBlob = ILTypeToOtherSig
					(member->programItem.image, member->signature);
		}
		else if(member->kind == IL_META_MEMBERKIND_REF)
		{
			ILMember *ref = ((ILMemberRef *)member)->ref;
			if(ref->kind == IL_META_MEMBERKIND_METHOD)
			{
				member->signatureBlob = ILTypeToMethodSig
						(member->programItem.image, ref->signature);
			}
			else if(ref->kind == IL_META_MEMBERKIND_FIELD)
			{
				member->signatureBlob = ILTypeToFieldSig
						(member->programItem.image, ref->signature);
			}
		}
		if(!(member->signatureBlob))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Write signature information for a stand alone signature.
 */
static int SigWrite_StandAloneSig(ILStandAloneSig *sig)
{
	if(sig->type && !(sig->typeBlob))
	{
		if(ILType_IsMethod(sig->type))
		{
			sig->typeBlob = ILTypeToMethodSig
					(sig->programItem.image, sig->type);
		}
		else
		{
			sig->typeBlob = ILTypeToOtherSig
					(sig->programItem.image, sig->type);
		}
		if(!(sig->typeBlob))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Write signature information for a property definition.
 */
static int SigWrite_Property(ILProperty *property)
{
	if(property->member.signature && !(property->member.signatureBlob))
	{
		property->member.signatureBlob = ILTypeToOtherSig
				(property->member.programItem.image,
				 property->member.signature);
		if(!(property->member.signatureBlob))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Write signature information for a type specification.
 */
static int SigWrite_TypeSpec(ILTypeSpec *spec)
{
	if(spec->type && !(spec->typeBlob))
	{
		spec->typeBlob = ILTypeToOtherSig(spec->programItem.image, spec->type);
		if(!(spec->typeBlob))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Write signature information for a method specification.
 */
static int SigWrite_MethodSpec(ILMethodSpec *spec)
{
	if(spec->type && !(spec->typeBlob))
	{
		spec->typeBlob =
			ILTypeToMethodSig(spec->programItem.image, spec->type);
		if(!(spec->typeBlob))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Array of all signature writing routines for the known token types.
 */
typedef int (*ILSigWriteFunc)(void *data);
static ILSigWriteFunc const SigWriters[64] = {
	0,											/* 00 */
	0,
	0,
	0,
	(ILSigWriteFunc)SigWrite_FieldDef,
	0,
	(ILSigWriteFunc)SigWrite_MethodDef,
	0,
	0,											/* 08 */
	0,
	(ILSigWriteFunc)SigWrite_MemberRef,
	0,
	0,
	0,
	0,
	0,
	0,											/* 10 */
	(ILSigWriteFunc)SigWrite_StandAloneSig,
	0,
	0,
	0,
	0,
	0,
	(ILSigWriteFunc)SigWrite_Property,
	0,											/* 18 */
	0,
	0,
	(ILSigWriteFunc)SigWrite_TypeSpec,
	0,
	0,
	0,
	0,
	0,											/* 20 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,											/* 28 */
	0,
	0,
	(ILSigWriteFunc)SigWrite_MethodSpec,
	0,
	0,
	0,
	0,
	0,											/* 30 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,											/* 38 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
};

/*
 * Sort the InterfaceImpl table.
 */
static int Sort_InterfaceImpl(ILImplements **impl1, ILImplements **impl2)
{
	if((*impl1)->implement->programItem.token <
			(*impl2)->implement->programItem.token)
	{
		return -1;
	}
	else if((*impl1)->implement->programItem.token >
				(*impl2)->implement->programItem.token)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Sort a table that has owned items within it.
 */
static int Sort_OwnedItem(ILOwnedItem **item1, ILOwnedItem **item2)
{
	ILToken token1 = (*item1)->owner->token;
	ILToken token2 = (*item2)->owner->token;
	ILToken tokenNum1 = (token1 & ~IL_META_TOKEN_MASK);
	ILToken tokenNum2 = (token2 & ~IL_META_TOKEN_MASK);

	/* Compare the bottom parts of the token first, because
	   the table must be sorted on its encoded value, not
	   on the original value.  Encoded values put the token
	   type in the low order bits */
	if(tokenNum1 < tokenNum2)
	{
		return -1;
	}
	else if(tokenNum1 > tokenNum2)
	{
		return 1;
	}
	else if(token1 < token2)
	{
		return -1;
	}
	else if(token1 > token2)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Sort the CustomAttr table.
 */
static int Sort_CustomAttr(ILAttribute **attr1, ILAttribute **attr2)
{
	ILToken token1 = (*attr1)->owner->token;
	ILToken token2 = (*attr2)->owner->token;
	ILToken tokenNum1 = (token1 & ~IL_META_TOKEN_MASK);
	ILToken tokenNum2 = (token2 & ~IL_META_TOKEN_MASK);
	int type1, type2;

	/* Compare the bottom parts of the token first, because
	   the table must be sorted on its encoded value, not
	   on the original value.  Encoded values put the token
	   type in the low order bits */
	if(tokenNum1 < tokenNum2)
	{
		return -1;
	}
	else if(tokenNum1 > tokenNum2)
	{
		return 1;
	}

	/* Convert the token types into encoded type values */
	switch(token1 & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_METHOD_DEF:				type1 =  0; break;
		case IL_META_TOKEN_FIELD_DEF:				type1 =  1; break;
		case IL_META_TOKEN_TYPE_REF:				type1 =  2; break;
		case IL_META_TOKEN_TYPE_DEF:				type1 =  3; break;
		case IL_META_TOKEN_PARAM_DEF:				type1 =  4; break;
		case IL_META_TOKEN_INTERFACE_IMPL:			type1 =  5; break;
		case IL_META_TOKEN_MEMBER_REF:				type1 =  6; break;
		case IL_META_TOKEN_MODULE:					type1 =  7; break;
		case IL_META_TOKEN_DECL_SECURITY:			type1 =  8; break;
		case IL_META_TOKEN_PROPERTY:				type1 =  9; break;
		case IL_META_TOKEN_EVENT:					type1 = 10; break;
		case IL_META_TOKEN_STAND_ALONE_SIG:			type1 = 11; break;
		case IL_META_TOKEN_MODULE_REF:				type1 = 12; break;
		case IL_META_TOKEN_TYPE_SPEC:				type1 = 13; break;
		case IL_META_TOKEN_ASSEMBLY:				type1 = 14; break;
		case IL_META_TOKEN_ASSEMBLY_REF:			type1 = 15; break;
		case IL_META_TOKEN_FILE:					type1 = 16; break;
		case IL_META_TOKEN_EXPORTED_TYPE:			type1 = 17; break;
		case IL_META_TOKEN_MANIFEST_RESOURCE:		type1 = 18; break;
		default:									type1 = 19; break;
	}
	switch(token2 & IL_META_TOKEN_MASK)
	{
		case IL_META_TOKEN_METHOD_DEF:				type2 =  0; break;
		case IL_META_TOKEN_FIELD_DEF:				type2 =  1; break;
		case IL_META_TOKEN_TYPE_REF:				type2 =  2; break;
		case IL_META_TOKEN_TYPE_DEF:				type2 =  3; break;
		case IL_META_TOKEN_PARAM_DEF:				type2 =  4; break;
		case IL_META_TOKEN_INTERFACE_IMPL:			type2 =  5; break;
		case IL_META_TOKEN_MEMBER_REF:				type2 =  6; break;
		case IL_META_TOKEN_MODULE:					type2 =  7; break;
		case IL_META_TOKEN_DECL_SECURITY:			type2 =  8; break;
		case IL_META_TOKEN_PROPERTY:				type2 =  9; break;
		case IL_META_TOKEN_EVENT:					type2 = 10; break;
		case IL_META_TOKEN_STAND_ALONE_SIG:			type2 = 11; break;
		case IL_META_TOKEN_MODULE_REF:				type2 = 12; break;
		case IL_META_TOKEN_TYPE_SPEC:				type2 = 13; break;
		case IL_META_TOKEN_ASSEMBLY:				type2 = 14; break;
		case IL_META_TOKEN_ASSEMBLY_REF:			type2 = 15; break;
		case IL_META_TOKEN_FILE:					type2 = 16; break;
		case IL_META_TOKEN_EXPORTED_TYPE:			type2 = 17; break;
		case IL_META_TOKEN_MANIFEST_RESOURCE:		type2 = 18; break;
		default:									type2 = 19; break;
	}

	/* Compare the encoded token types */
	if(type1 < type2)
	{
		return -1;
	}
	else if(type1 > type2)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Sort the MethodSemantics table.
 */
static int Sort_MethodSemantics(ILMethodSem **sem1, ILMethodSem **sem2)
{
	ILToken token1 = (*sem1)->owner->token;
	ILToken token2 = (*sem2)->owner->token;
	ILToken tokenNum1 = (token1 & ~IL_META_TOKEN_MASK);
	ILToken tokenNum2 = (token2 & ~IL_META_TOKEN_MASK);

	/* Compare the bottom parts of the token first, because
	   the table must be sorted on its encoded value, not
	   on the original value.  Encoded values put the token
	   type in the low order bits */
	if(tokenNum1 < tokenNum2)
	{
		return -1;
	}
	else if(tokenNum1 > tokenNum2)
	{
		return 1;
	}
	else if(token1 < token2)
	{
		return -1;
	}
	else if(token1 > token2)
	{
		return 1;
	}

	/* Sort on the type also.  This may not be strictly
	   necessary, but it is better to be paranoid */
	if((*sem1)->type < (*sem2)->type)
	{
		return -1;
	}
	else if((*sem1)->type > (*sem2)->type)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Sort the MethodImpl table.
 */
static int Sort_MethodImpl(ILOverride **over1, ILOverride **over2)
{
	ILToken token1 = (*over1)->member.owner->programItem.token;
	ILToken token2 = (*over2)->member.owner->programItem.token;
	if(token1 < token2)
	{
		return -1;
	}
	else if(token1 > token2)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Sort the ImplMap table.
 */
static int Sort_ImplMap(ILPInvoke **pinvoke1, ILPInvoke **pinvoke2)
{
	ILToken token1 = (*pinvoke1)->memberInfo->programItem.token;
	ILToken token2 = (*pinvoke2)->memberInfo->programItem.token;
	ILToken tokenNum1 = (token1 & ~IL_META_TOKEN_MASK);
	ILToken tokenNum2 = (token2 & ~IL_META_TOKEN_MASK);

	/* Compare the bottom parts of the token first, because
	   the table must be sorted on its encoded value, not
	   on the original value.  Encoded values put the token
	   type in the low order bits */
	if(tokenNum1 < tokenNum2)
	{
		return -1;
	}
	else if(tokenNum1 > tokenNum2)
	{
		return 1;
	}
	else if(token1 < token2)
	{
		return -1;
	}
	else if(token1 > token2)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Sort the NestedClass table.
 */
static int Sort_NestedClass(ILNestedInfo **nested1, ILNestedInfo **nested2)
{
	ILToken token1 = (*nested1)->child->programItem.token;
	ILToken token2 = (*nested2)->child->programItem.token;
	if(token1 < token2)
	{
		return -1;
	}
	else if(token1 > token2)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Sort the GenericPar table.
 */
static int Sort_GenericPar(ILGenericPar **genPar1, ILGenericPar **genPar2)
{
	ILToken token1 = (*genPar1)->ownedItem.owner->token;
	ILToken token2 = (*genPar2)->ownedItem.owner->token;
	ILToken tokenNum1 = (token1 & ~IL_META_TOKEN_MASK);
	ILToken tokenNum2 = (token2 & ~IL_META_TOKEN_MASK);

	/* Compare the bottom parts of the token first, because
	   the table must be sorted on its encoded value, not
	   on the original value.  Encoded values put the token
	   type in the low order bits */
	if(tokenNum1 < tokenNum2)
	{
		return -1;
	}
	else if(tokenNum1 > tokenNum2)
	{
		return 1;
	}
	else if(token1 < token2)
	{
		return -1;
	}
	else if(token1 > token2)
	{
		return 1;
	}
	else
	{
		if((*genPar1)->number < (*genPar2)->number)
		{
			return -1;
		}
		else if((*genPar1)->number > (*genPar2)->number)
		{
			return 1;
		}
		else
		{
			/* We never should get here. */
			return 0;
		}
	}
}

/*
 * Array of all sorting routines for the known token types.
 */
typedef int (*ILSortFunc)(const void *e1, const void *e2);
static ILSortFunc const SortFuncs[64] = {
	0,											/* 00 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,											/* 08 */
	(ILSortFunc)Sort_InterfaceImpl,
	0,
	(ILSortFunc)Sort_OwnedItem,
	(ILSortFunc)Sort_CustomAttr,
	(ILSortFunc)Sort_OwnedItem,
	(ILSortFunc)Sort_OwnedItem,
	(ILSortFunc)Sort_OwnedItem,
	(ILSortFunc)Sort_OwnedItem,					/* 10 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	(ILSortFunc)Sort_MethodSemantics,			/* 18 */
	(ILSortFunc)Sort_MethodImpl,
	0,
	0,
	(ILSortFunc)Sort_ImplMap,
	(ILSortFunc)Sort_OwnedItem,
	0,
	0,
	0,											/* 20 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,											/* 28 */
	(ILSortFunc)Sort_NestedClass,
	(ILSortFunc)Sort_GenericPar,
	0,
	(ILSortFunc)Sort_OwnedItem,
	0,
	0,
	0,
	0,											/* 30 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
	0,											/* 38 */
	0,
	0,
	0,
	0,
	0,
	0,
	0,
};

/*
 * Add an RVA fixup for field references to data sections.
 */
static void AddRVAFixup(ILWriter *writer, unsigned long fixupRVA,
						unsigned long valueRVA)
{
	ILFixup *fixup = ILMemPoolAlloc(&(writer->fixups), ILFixup);
	if(fixup)
	{
		fixup->kind = IL_FIXUP_FIELD_RVA;
		fixup->rva = fixupRVA;
		fixup->un.value = valueRVA;
		fixup->next = 0;
		if(writer->lastFixup)
		{
			writer->lastFixup->next = fixup;
		}
		else
		{
			writer->firstFixup = fixup;
		}
		writer->lastFixup = fixup;
	}
	else
	{
		writer->outOfMemory = 1;
	}
}

int _ILWriteMetadataIndex(ILWriter *writer, ILImage *image)
{
	unsigned char buffer[256];
	int sizeFlags;
	ILUInt64 tokenFlags;
	ILUInt64 sortedFlags;
	int tokenType;
	int posn;
	int size;
	ILToken token;
	ILToken tokenKind;
	ILUInt32 values[IL_IMAGE_TOKEN_COLUMNS];
	ILFormatFunc func;
	ILSigWriteFunc sigFunc;
	ILSortFunc sortFunc;

	/* Write signature information now that we know what is
	   really a definition and what is really a reference */
	for(tokenType = 0; tokenType < 64; ++tokenType)
	{
		sigFunc = SigWriters[tokenType];
		if(sigFunc)
		{
			for(token = 0; token < image->tokenCount[tokenType]; ++token)
			{
				if(image->tokenData[tokenType] &&
				   image->tokenData[tokenType][token] != 0)
				{
					if(!((*sigFunc)(image->tokenData[tokenType][token])))
					{
						return 0;
					}
				}
			}
		}
	}

	/* Sort metadata tables that need it */
	sortedFlags = 0;
	for(tokenType = 0; tokenType < 64; ++tokenType)
	{
		sortFunc = SortFuncs[tokenType];
		if(sortFunc)
		{
			/* Set the sort flag in the metadata header */
			sortedFlags |= (((ILUInt64)1) << tokenType);

			/* Sort the table */
			if(image->tokenCount[tokenType] > 1)
			{
			#ifdef HAVE_QSORT
				qsort(image->tokenData[tokenType],
					  image->tokenCount[tokenType],
					  sizeof(void *), sortFunc);
			#else
				unsigned long posn;
				unsigned long posn2;
				unsigned long count;
				void **data;
				void *item1;
				void *item2;
				count = image->tokenCount[tokenType];
				data = image->tokenData[tokenType];
				for(posn = 0; posn < (count - 1); ++posn)
				{
					for(posn2 = (posn + 1); posn2 < count; ++posn2)
					{
						item1 = &(data[posn]);
						item2 = &(data[posn2]);
						if((*sortFunc)(item1, item2) > 0)
						{
							item1 = data[posn];
							data[posn] = data[posn2];
							data[posn2] = item1;
						}
					}
				}
			#endif
			}

			/* Assign new token codes to the items within the sorted table */
			tokenKind = (((ILToken)tokenType) << 24);
			for(token = 0; token < image->tokenCount[tokenType]; ++token)
			{
				((ILProgramItem *)(image->tokenData[tokenType][token]))->token
						= (tokenKind | (token + 1));
			}
		}
	}

	/* Determine the size flags for the header.  The GUIDREF
	   size is guessed based on the number of modules.  Since
	   most assemblies have only 1 module, the GUIDREF flag
	   will very rarely be set */
	sizeFlags = 0;
	if(image->stringPoolSize > 65535)
	{
		sizeFlags |= IL_META_SIZE_FLAG_STRREF;
		image->strRefBig = -1;
	}
	else
	{
		image->strRefBig = 0;
	}
	if(image->blobPoolSize > 65535)
	{
		sizeFlags |= IL_META_SIZE_FLAG_BLOBREF;
		image->blobRefBig = -1;
	}
	else
	{
		image->blobRefBig = 0;
	}
	if(image->tokenCount[IL_META_TOKEN_MODULE >> 24] > (65535 / 3))
	{
		sizeFlags |= IL_META_SIZE_FLAG_GUIDREF;
		image->guidRefBig = -1;
	}
	else
	{
		image->guidRefBig = 0;
	}

	/* Compute the sizes of all token types */
	_ILImageComputeTokenSizes(image);

	/* Collect flag bits for all of the token types that are in use */
	tokenFlags = 0;
	for(tokenType = 0; tokenType < 64; ++tokenType)
	{
		if(image->tokenCount[tokenType] != 0 && Formatters[tokenType] != 0)
		{
			tokenFlags |= (((ILUInt64)1) << tokenType);
		}
	}

	/* Write the metadata index header */
	IL_WRITE_UINT32(buffer, 0);				/* Reserved */
#if IL_VERSION_MAJOR > 1
	buffer[4] = 0x02;						/* Major version */
#else
	buffer[4] = 0x01;						/* Major version */
#endif
	buffer[5] = 0x00;						/* Minor version */
	buffer[6] = (unsigned char)sizeFlags;	/* Section size flags */
	buffer[7] = 0x00;						/* Reserved */
	IL_WRITE_UINT64(buffer + 8, tokenFlags);
	IL_WRITE_UINT64(buffer + 16, sortedFlags);
	if(!_ILWBufferListAdd(&(writer->indexBlob), buffer, 24))
	{
		writer->outOfMemory = 1;
		return 1;
	}

	/* Write out the token count table */
	posn = 0;
	for(tokenType = 0; tokenType < 64; ++tokenType)
	{
		if((tokenFlags & (((ILUInt64)1) << tokenType)) != 0)
		{
			IL_WRITE_UINT32(buffer + posn, image->tokenCount[tokenType]);
			posn += 4;
			if(posn >= (int)(sizeof(buffer)))
			{
				if(!_ILWBufferListAdd(&(writer->indexBlob), buffer, posn))
				{
					writer->outOfMemory = 1;
					return 1;
				}
				posn = 0;
			}
		}
	}

	/* Write out the tokens themselves */
	for(tokenType = 0; tokenType < 64; ++tokenType)
	{
		if((tokenFlags & (((ILUInt64)1) << tokenType)) != 0)
		{
			size = image->tokenSize[tokenType];
			tokenKind = (((ILToken)tokenType) << 24);
			func = Formatters[tokenType];
			for(token = 0; token < image->tokenCount[tokenType]; ++token)
			{
				/* Flush the buffer if not enough room for the next token */
				if((posn + size) > sizeof(buffer))
				{
					if(!_ILWBufferListAdd(&(writer->indexBlob), buffer, posn))
					{
						writer->outOfMemory = 1;
						return 1;
					}
					posn = 0;
				}

				/* Format the token into the "values" array */
				if(image->tokenData[tokenType] &&
				   image->tokenData[tokenType][token] != 0)
				{
					(*func)(writer, image, values,
							image->tokenData[tokenType][token]);
					if(tokenKind == IL_META_TOKEN_FIELD_RVA)
					{
						/* We need to add an RVA fixup to reposition
						   field RVA's once we know where the ".sdata"
						   and ".tls" sections reside */
						AddRVAFixup(writer, writer->indexBlob.offset + posn,
									((ILFieldRVA *)(image->tokenData
										[tokenType][token]))->rva);
					}
				}
				else
				{
					return 0;
				}

				/* Encode the "values" array into "buffer" */
				_ILImageRawTokenEncode(image, buffer + posn,
									   (tokenKind | (token + 1)), values);
				posn += size;
			}
		}
	}

	/* Flush the remainder of the index information */
	if(posn > 0)
	{
		if(!_ILWBufferListAdd(&(writer->indexBlob), buffer, posn))
		{
			writer->outOfMemory = 1;
			return 1;
		}
	}

	/* Successful */
	return 1;
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_USE_WRITER */
