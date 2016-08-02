/*
 * misc_token.c - Handle miscellaneous tokens within images.
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

#include "program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILTypeSpec *ILTypeSpecCreate(ILImage *image, ILToken token, ILType *type)
{
	ILTypeSpec *spec;

	if(token == 0)
	{		
		/* There shall be no duplicates in the TypeSpecs. */
		spec = 0;

		while((spec = (ILTypeSpec *)ILImageNextToken(image,
													 IL_META_TOKEN_TYPE_SPEC,
													 spec)))
		{
			if(ILTypeIdentical(spec->type, type))
			{
				return spec;
			}
		}
	}

	/* Allocate space for the TypeSpec block from the memory stack */
	spec = ILMemStackAlloc(&(image->memStack), ILTypeSpec);
	if(!spec)
	{
		return 0;
	}

	/* Set the TypeSpec information fields */
	spec->programItem.image = image;
	spec->type = type;
	spec->typeBlob = 0;
	spec->classInfo = 0;
	spec->refInfo = 0;

	/* Assign a token code to the TypeSpec information block */
	if(!_ILImageSetToken(image, &(spec->programItem), token,
						 IL_META_TOKEN_TYPE_SPEC))
	{
		return 0;
	}

	/* Return the TypeSpec information block to the caller */
	return spec;
}

ILTypeSpec *ILTypeSpecImport(ILImage *image, ILTypeSpec *spec)
{
	ILType *type;

	if(spec->programItem.image == image)
	{
		return spec;
	}
	if((type = ILTypeImport(image, ILTypeSpec_Type(spec))) == 0)
	{
		return 0;
	}
	return ILTypeSpecCreate(image, 0, type);
}

ILType *ILTypeSpecGetType(ILTypeSpec *spec)
{
	return spec->type;
}

void ILTypeSpecSetClass(ILTypeSpec *spec, ILClass *classInfo)
{
	spec->classInfo = classInfo;
}

ILClass *ILTypeSpecGetClass(ILTypeSpec *spec)
 {
	if(spec)
	{
		if(!(spec->refInfo))
		{
			/* Make a synthetic class that corresponds to the type */
			spec->refInfo = ILClassFromType(spec->programItem.image, 0,
											spec->type, ILClassResolveSystem);
		}
		return spec->refInfo;
	}
	return 0;
}

ILClass *ILTypeSpecGetClassRef(ILTypeSpec *spec)
{
	if(spec)
	{
		ILClass *info = ILTypeSpecGetClass(spec);

		if(!info || info->programItem.image == spec->programItem.image)
		{
			return info;
		}
		info = ILClassImport(spec->programItem.image, info);
		if(info)
		{
			info->programItem.token = spec->programItem.token;
			info->synthetic = spec->type;
		}
		return info;
	}
	return 0;
}

ILClass *ILTypeSpecGetClassWrapper(ILTypeSpec *spec)
{
	if(spec)
	{
		if(!(spec->classInfo))
		{
			/* Make a wrapper class that corresponds to the type */
			spec->classInfo = ILClassCreateWrapper(ILToProgramItem(spec),
												   spec->programItem.token,
												   spec->type);
		}
		return spec->classInfo;
	}
	return 0;
}

void _ILTypeSpecSetTypeIndex(ILTypeSpec *spec, ILUInt32 index)
{
	spec->typeBlob = index;
}

ILStandAloneSig *ILStandAloneSigCreate(ILImage *image, ILToken token,
									   ILType *type)
{
	ILStandAloneSig *sig;

	/* Allocate space for the signature block from the memory stack */
	sig = ILMemStackAlloc(&(image->memStack), ILStandAloneSig);
	if(!sig)
	{
		return 0;
	}

	/* Set the signature definition fields */
	sig->programItem.image = image;
	sig->type = type;
	sig->typeBlob = 0;

	/* Assign a token code to the signature information block */
	if(!_ILImageSetToken(image, &(sig->programItem), token,
						 IL_META_TOKEN_STAND_ALONE_SIG))
	{
		return 0;
	}

	/* Return the signature information block to the caller */
	return sig;
}

ILType *ILStandAloneSigGetType(ILStandAloneSig *sig)
{
	return sig->type;
}

int ILStandAloneSigIsLocals(ILStandAloneSig *sig)
{
	return (sig->type && ILType_IsComplex(sig->type) &&
	        ILType_Kind(sig->type) == IL_TYPE_COMPLEX_LOCALS);
}

void _ILStandAloneSigSetTypeIndex(ILStandAloneSig *sig, ILUInt32 index)
{
	sig->typeBlob = index;
}

ILConstant *ILConstantCreate(ILImage *image, ILToken token,
							 ILProgramItem *owner, ILUInt32 elemType)
{
	ILConstant *constant;
	ILField *field;
	ILParameter *param;
	ILProperty *property;

	/* Allocate space for the constant block from the memory stack */
	constant = ILMemStackAlloc(&(image->memStack), ILConstant);
	if(!constant)
	{
		return 0;
	}

	/* Set the constant information fields */
	constant->ownedItem.programItem.image = image;
	constant->ownedItem.owner = owner;
	constant->elemType = elemType;
	constant->value = 0;

	/* Assign a token code to the constant information block */
	if(!_ILImageSetToken(image, &(constant->ownedItem.programItem), token,
						 IL_META_TOKEN_CONSTANT))
	{
		return 0;
	}

	/* If the owner is a Field, Parameter, or Property, then
	   set the "HAS_DEFAULT" flag within the definition */
	if((field = _ILProgramItem_ToFieldDef(owner)) != 0)
	{
		field->member.attributes |= IL_META_FIELDDEF_HAS_DEFAULT;
	}
	else if((param = _ILProgramItem_ToParamDef(owner)) != 0)
	{
		param->attributes |= IL_META_PARAMDEF_HAS_DEFAULT;
	}
	else if((property = _ILProgramItem_ToPropertyDef(owner)) != 0)
	{
		property->member.attributes |= IL_META_PROPDEF_HAS_DEFAULT;
	}

	/* Return the constant information block to the caller */
	return constant;
}

ILProgramItem *ILConstantGetOwner(ILConstant *constant)
{
	return constant->ownedItem.owner;
}

ILUInt32 ILConstantGetElemType(ILConstant *constant)
{
	return constant->elemType;
}

int ILConstantSetValue(ILConstant *constant, const void *blob,
					   unsigned long len)
{
	if(constant->ownedItem.programItem.image->type == IL_IMAGETYPE_BUILDING)
	{
		constant->value = (ILUInt32)(ILImageAddBlob
										(constant->ownedItem.programItem.image,
									     blob, len));
		return (constant->value != 0);
	}
	else
	{
		/* We cannot use this function when loading images.
		   Use "_ILConstantSetValueIndex" instead */
		return 1;
	}
}

void _ILConstantSetValueIndex(ILConstant *constant, ILUInt32 index)
{
	constant->value = index;
}

const void *ILConstantGetValue(ILConstant *constant, ILUInt32 *len)
{
	return ILImageGetBlob(constant->ownedItem.programItem.image,
						  constant->value, len);
}

/*
 * Compare owned item tokens looking for a match.  We compare
 * the low bits of the token first, and then the high bits.
 * This is necessary to simulate the ECMA-style sorting which
 * uses the encoded token value rather than the raw token
 * values that we are using here.
 */
static int OwnedItemCompareRaw(ILUInt32 *values, ILToken searchFor,
							   int valueField)
{
	ILToken token1 = searchFor;
	ILToken token1Stripped = (token1 & ~IL_META_TOKEN_MASK);
	ILToken token2 = (ILToken)(values[valueField]);
	ILToken token2Stripped = (token2 & ~IL_META_TOKEN_MASK);
	if(token1Stripped < token2Stripped)
	{
		return -1;
	}
	else if(token1Stripped > token2Stripped)
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
static int OwnedItemCompare(void *item, void *userData)
{
	ILOwnedItem *owned = (ILOwnedItem *)item;
	ILToken token1 = (owned->owner ? owned->owner->token : 0);
	ILToken token1Stripped = (token1 & ~IL_META_TOKEN_MASK);
	ILToken token2 = ((ILProgramItem *)userData)->token;
	ILToken token2Stripped = (token2 & ~IL_META_TOKEN_MASK);
	if(token1Stripped < token2Stripped)
	{
		return -1;
	}
	else if(token1Stripped > token2Stripped)
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
 * Search for an owned item.
 */
static void *SearchForOwnedItem(ILImage *image, ILToken tokenType,
								void *owner, int valueField)
{
	ILUInt32 token, num;
	if(!owner)
	{
		return 0;
	}
	if(image->type == IL_IMAGETYPE_BUILDING)
	{
		return ILImageSearchForToken(image, tokenType, OwnedItemCompare, owner);
	}
	num = _ILSearchForRawToken(image, OwnedItemCompareRaw, tokenType, &token,
							   ((ILProgramItem *)owner)->token,
							   valueField);
	/*
	 * Check if the number of owned items of this token type is valid or no
	 * owned item was found.
	 */
	if((num < 1) ||
	   ((num > 1) && (tokenType != IL_META_TOKEN_DECL_SECURITY)))
	{
		return 0;
	}
	return ILImageTokenInfo(image, token);
}

ILConstant *ILConstantGetFromOwner(ILProgramItem *owner)
{
	ILField *field;
	ILParameter *param;
	ILProperty *property;

	/* Filter out program items that obviously don't have a value,
	   to avoid searching the constant table unnecessarily */
	if((field = _ILProgramItem_ToFieldDef(owner)) != 0)
	{
		if((field->member.attributes & IL_META_FIELDDEF_HAS_DEFAULT) == 0)
		{
			return 0;
		}
	}
	else if((param = _ILProgramItem_ToParamDef(owner)) != 0)
	{
		if((param->attributes & IL_META_PARAMDEF_HAS_DEFAULT) == 0)
		{
			return 0;
		}
	}
	else if((property = _ILProgramItem_ToPropertyDef(owner)) != 0)
	{
		if((property->member.attributes & IL_META_PROPDEF_HAS_DEFAULT) == 0)
		{
			return 0;
		}
	}

	/* Search for the constant token */
	return (ILConstant *)SearchForOwnedItem
				(owner->image, IL_META_TOKEN_CONSTANT,
				 (void *)owner, IL_OFFSET_CONSTANT_REFERENCE);
}

ILFieldRVA *ILFieldRVACreate(ILImage *image, ILToken token,
							 ILField *owner, ILUInt32 rva)
{
	ILFieldRVA *frva;

	/* Allocate space for the field RVA block from the memory stack */
	frva = ILMemStackAlloc(&(image->memStack), ILFieldRVA);
	if(!frva)
	{
		return 0;
	}

	/* Set the field RVA information fields */
	frva->ownedItem.programItem.image = image;
	frva->ownedItem.owner = &(owner->member.programItem);
	frva->rva = rva;

	/* Assign a token code to the field RVA information block */
	if(!_ILImageSetToken(image, &(frva->ownedItem.programItem), token,
						 IL_META_TOKEN_FIELD_RVA))
	{
		return 0;
	}

	/* Set the HAS_FIELD_RVA flag on the owner */
	ILMemberSetAttrs((ILMember *)owner,
					 IL_META_FIELDDEF_HAS_FIELD_RVA,
					 IL_META_FIELDDEF_HAS_FIELD_RVA);

	/* Return the field RVA information block to the caller */
	return frva;
}

ILField *ILFieldRVAGetOwner(ILFieldRVA *rva)
{
	return (ILField *)(rva->ownedItem.owner);
}

ILUInt32 ILFieldRVAGetRVA(ILFieldRVA *rva)
{
	return rva->rva;
}

void ILFieldRVASetRVA(ILFieldRVA *rva, ILUInt32 value)
{
	rva->rva = value;
}

ILFieldRVA *ILFieldRVAGetFromOwner(ILField *owner)
{
	if((owner->member.attributes & IL_META_FIELDDEF_HAS_FIELD_RVA) == 0)
	{
		return 0;
	}
	return (ILFieldRVA *)SearchForOwnedItem
				(owner->member.programItem.image,
				 IL_META_TOKEN_FIELD_RVA,
				 (void *)owner, IL_OFFSET_FIELDRVA_FIELD);
}

ILFieldLayout *ILFieldLayoutCreate(ILImage *image, ILToken token,
							       ILField *owner, ILUInt32 offset)
{
	ILFieldLayout *layout;

	/* Allocate space for the field layout block from the memory stack */
	layout = ILMemStackAlloc(&(image->memStack), ILFieldLayout);
	if(!layout)
	{
		return 0;
	}

	/* Set the field layout information fields */
	layout->ownedItem.programItem.image = image;
	layout->ownedItem.owner = &(owner->member.programItem);
	layout->offset = offset;

	/* Assign a token code to the field layout information block */
	if(!_ILImageSetToken(image, &(layout->ownedItem.programItem), token,
						 IL_META_TOKEN_FIELD_LAYOUT))
	{
		return 0;
	}

	/* Return the field layout information block to the caller */
	return layout;
}

ILField *ILFieldLayoutGetOwner(ILFieldLayout *layout)
{
	return (ILField *)(layout->ownedItem.owner);
}

ILUInt32 ILFieldLayoutGetOffset(ILFieldLayout *layout)
{
	return layout->offset;
}

void ILFieldLayoutSetOffset(ILFieldLayout *layout, ILUInt32 offset)
{
	layout->offset = offset;
}

ILFieldLayout *ILFieldLayoutGetFromOwner(ILField *owner)
{
	/* Field layout information can only exist on explicit-layout classes,
	   so bail out early if we have some other kind of class */
	if(!ILClass_IsExplicitLayout(owner->member.owner))
	{
		return 0;
	}

	/* Search for the layout token */
	return (ILFieldLayout *)SearchForOwnedItem
				(owner->member.programItem.image,
				 IL_META_TOKEN_FIELD_LAYOUT,
				 (void *)owner, IL_OFFSET_FIELDLAYOUT_FIELD);
}

ILFieldMarshal *ILFieldMarshalCreate(ILImage *image, ILToken token,
							         ILProgramItem *owner)
{
	ILFieldMarshal *marshal;
	ILField *field;
	ILParameter *param;

	/* Allocate space for the field marshal block from the memory stack */
	marshal = ILMemStackAlloc(&(image->memStack), ILFieldMarshal);
	if(!marshal)
	{
		return 0;
	}

	/* Set the field marshal information fields */
	marshal->ownedItem.programItem.image = image;
	marshal->ownedItem.owner = owner;
	marshal->type = 0;

	/* Assign a token code to the field marshal information block */
	if(!_ILImageSetToken(image, &(marshal->ownedItem.programItem), token,
						 IL_META_TOKEN_FIELD_MARSHAL))
	{
		return 0;
	}

	/* If the owner is a Field or Parameter, then set the
	   "HAS_FIELD_MARSHAL" flag within the definition */
	if((field = _ILProgramItem_ToFieldDef(owner)) != 0)
	{
		field->member.attributes |= IL_META_FIELDDEF_HAS_FIELD_MARSHAL;
	}
	else if((param = _ILProgramItem_ToParamDef(owner)) != 0)
	{
		param->attributes |= IL_META_PARAMDEF_HAS_FIELD_MARSHAL;
	}

	/* Return the field marshal information block to the caller */
	return marshal;
}

ILProgramItem *ILFieldMarshalGetOwner(ILFieldMarshal *marshal)
{
	return marshal->ownedItem.owner;
}

int ILFieldMarshalSetType(ILFieldMarshal *marshal, const void *blob,
					      unsigned long len)
{
	if(marshal->ownedItem.programItem.image->type == IL_IMAGETYPE_BUILDING)
	{
		marshal->type = (ILUInt32)(ILImageAddBlob
										(marshal->ownedItem.programItem.image,
									  	 blob, len));
		return (marshal->type != 0);
	}
	else
	{
		/* We cannot use this function when loading images.
		   Use "_ILFieldMarshalSetTypeIndex" instead */
		return 1;
	}
}

void _ILFieldMarshalSetTypeIndex(ILFieldMarshal *marshal, ILUInt32 index)
{
	marshal->type = index;
}

const void *ILFieldMarshalGetType(ILFieldMarshal *marshal, ILUInt32 *len)
{
	return ILImageGetBlob(marshal->ownedItem.programItem.image,
						  marshal->type, len);
}

ILFieldMarshal *ILFieldMarshalGetFromOwner(ILProgramItem *owner)
{
	ILField *field;
	ILParameter *param;

	/* Filter out members that obviously don't have marshal information */
	if((field = _ILProgramItem_ToFieldDef(owner)) != 0)
	{
		if((field->member.attributes & IL_META_FIELDDEF_HAS_FIELD_MARSHAL) == 0)
		{
			return 0;
		}
	}
	else if((param = _ILProgramItem_ToParamDef(owner)) != 0)
	{
		if((param->attributes & IL_META_PARAMDEF_HAS_FIELD_MARSHAL) == 0)
		{
			return 0;
		}
	}

	/* Search for the token */
	return (ILFieldMarshal *)SearchForOwnedItem
				(owner->image, IL_META_TOKEN_FIELD_MARSHAL,
				 (void *)owner, IL_OFFSET_FIELDMARSHAL_TOKEN);
}

ILClassLayout *ILClassLayoutCreate(ILImage *image, ILToken token,
							       ILClass *owner,
								   ILUInt32 packingSize,
								   ILUInt32 classSize)
{
	ILClassLayout *layout;

	/* Allocate space for the class layout block from the memory stack */
	layout = ILMemStackAlloc(&(image->memStack), ILClassLayout);
	if(!layout)
	{
		return 0;
	}

	/* Set the class layout information fields */
	layout->ownedItem.programItem.image = image;
	layout->ownedItem.owner = &(owner->programItem);
	layout->packingSize = packingSize;
	layout->classSize = classSize;

	/* Assign a token code to the class layout information block */
	if(!_ILImageSetToken(image, &(layout->ownedItem.programItem), token,
						 IL_META_TOKEN_CLASS_LAYOUT))
	{
		return 0;
	}

	/* Return the class layout information block to the caller */
	return layout;
}

ILClass *ILClassLayoutGetOwner(ILClassLayout *layout)
{
	return (ILClass *)(layout->ownedItem.owner);
}

ILUInt32 ILClassLayoutGetPackingSize(ILClassLayout *layout)
{
	return layout->packingSize;
}

void ILClassLayoutSetPackingSize(ILClassLayout *layout, ILUInt32 size)
{
	layout->packingSize = size;
}

ILUInt32 ILClassLayoutGetClassSize(ILClassLayout *layout)
{
	return layout->classSize;
}

void ILClassLayoutSetClassSize(ILClassLayout *layout, ILUInt32 size)
{
	layout->classSize = size;
}

ILClassLayout *ILClassLayoutGetFromOwner(ILClass *owner)
{
	/* Class layout information cannot exist on auto-layout classes */
	if(ILClass_IsAutoLayout(owner))
	{
		return 0;
	}

	/* Search for the layout token */
	return (ILClassLayout *)SearchForOwnedItem
				(owner->programItem.image, IL_META_TOKEN_CLASS_LAYOUT,
				 (void *)owner, IL_OFFSET_CLASSLAYOUT_TYPE);
}

ILDeclSecurity *ILDeclSecurityCreate(ILImage *image, ILToken token,
							         ILProgramItem *owner, ILUInt32 type)
{
	ILDeclSecurity *security;
	ILClass *classInfo;
	ILMethod *method;

	/* Allocate space for the security record from the memory stack */
	security = ILMemStackAlloc(&(image->memStack), ILDeclSecurity);
	if(!security)
	{
		return 0;
	}

	/* Set the security record fields */
	security->ownedItem.programItem.image = image;
	security->ownedItem.owner = owner;
	security->type = type;
	security->blob = 0;

	/* Assign a token code to the security record */
	if(!_ILImageSetToken(image, &(security->ownedItem.programItem), token,
						 IL_META_TOKEN_DECL_SECURITY))
	{
		return 0;
	}

	/* If the owner is a TypeDef or MethodDef, then set the
	   "HAS_SECURITY" flag within the definition */
	if((classInfo = _ILProgramItem_ToTypeDef(owner)) != 0)
	{
		classInfo->attributes |= IL_META_TYPEDEF_HAS_SECURITY;
	}
	else if((method = _ILProgramItem_ToMethodDef(owner)) != 0)
	{
		method->member.attributes |= IL_META_METHODDEF_HAS_SECURITY;
	}

	/* Return the security record to the caller */
	return security;
}

ILProgramItem *ILDeclSecurityGetOwner(ILDeclSecurity *security)
{
	return security->ownedItem.owner;
}

ILUInt32 ILDeclSecurityGetType(ILDeclSecurity *security)
{
	return security->type;
}

int ILDeclSecuritySetBlob(ILDeclSecurity *security, const void *blob,
					      ILUInt32 len)
{
	if(security->ownedItem.programItem.image->type == IL_IMAGETYPE_BUILDING)
	{
		security->blob = ILImageAddBlob(security->ownedItem.programItem.image,
									  	blob, len);
		return (security->blob != 0);
	}
	else
	{
		/* We cannot use this function when loading images.
		   Use "_ILDeclSecuritySetBlobIndex" instead */
		return 1;
	}
}

void _ILDeclSecuritySetBlobIndex(ILDeclSecurity *security, ILUInt32 index)
{
	security->blob = index;
}

const void *ILDeclSecurityGetBlob(ILDeclSecurity *security, ILUInt32 *len)
{
	return ILImageGetBlob(security->ownedItem.programItem.image,
						  security->blob, len);
}

ILDeclSecurity *ILDeclSecurityGetFromOwner(ILProgramItem *owner)
{
	return (ILDeclSecurity *)SearchForOwnedItem
				(owner->image, IL_META_TOKEN_DECL_SECURITY,
				 (void *)owner, IL_OFFSET_DECLSECURITY_TOKEN);
}

ILFileDecl *ILFileDeclCreate(ILImage *image, ILToken token,
							 const char *name, ILUInt32 attrs)
{
	ILFileDecl *decl;

	/* Allocate space for the file declaration from the memory stack */
	decl = ILMemStackAlloc(&(image->memStack), ILFileDecl);
	if(!decl)
	{
		return 0;
	}

	/* Set the file declaration fields */
	decl->programItem.image = image;
	decl->name = _ILContextPersistString(image, name);
	decl->attributes = attrs;
	decl->hash = 0;

	/* Assign a token code to the file declaration */
	if(!_ILImageSetToken(image, &(decl->programItem), token,
						 IL_META_TOKEN_FILE))
	{
		return 0;
	}

	/* Return the file declaration to the caller */
	return decl;
}

const char *ILFileDeclGetName(ILFileDecl *decl)
{
	return decl->name;
}

ILUInt32 ILFileDeclGetAttrs(ILFileDecl *decl)
{
	return decl->attributes;
}

int ILFileDeclSetHash(ILFileDecl *decl, const void *hash, ILUInt32 len)
{
	if(decl->programItem.image->type == IL_IMAGETYPE_BUILDING)
	{
		decl->hash = ILImageAddBlob(decl->programItem.image, hash, len);
		return (decl->hash != 0);
	}
	else
	{
		/* We cannot use this function when loading images.
		   Use "_ILFileDeclSetHashIndex" instead */
		return 1;
	}
}

void _ILFileDeclSetHashIndex(ILFileDecl *decl, ILUInt32 index)
{
	decl->hash = index;
}

const void *ILFileDeclGetHash(ILFileDecl *decl, ILUInt32 *len)
{
	return ILImageGetBlob(decl->programItem.image, decl->hash, len);
}

ILImage *ILFileDeclToImage(ILFileDecl *decl)
{
	if(decl->programItem.linked)
	{
		/* We have cached a previous image */
		return _ILProgramItemResolve(&(decl->programItem))->image;
	}
	else
	{
		/* Search for a matching ILModule in another image */
		ILImage *image;
		ILModule *newModule = 0;
		image = decl->programItem.image->context->firstImage;
		while(image != 0 && newModule == 0)
		{
			newModule = 0;
			while((newModule = (ILModule *)ILImageNextToken
						(image, IL_META_TOKEN_MODULE, newModule)) != 0)
			{
				if(!ILStrICmp(newModule->name, decl->name))
				{
					break;
				}
			}
			image = image->nextImage;
		}
		if(!newModule)
		{
			return 0;
		}

		/* Link the reference to the foreign module */
		_ILProgramItemLink(&(decl->programItem), &(newModule->programItem));

		/* Return the new module's image to the caller */
		return newModule->programItem.image;
	}
}

ILManifestRes *ILManifestResCreate(ILImage *image, ILToken token,
								   const char *name, ILUInt32 attrs, int offset)
{
	ILManifestRes *res;

	/* Allocate space for the manifest resource from the memory stack */
	res = ILMemStackAlloc(&(image->memStack), ILManifestRes);
	if(!res)
	{
		return 0;
	}

	/* Set the manifest resource fields */
	res->programItem.image = image;
	res->name = _ILContextPersistString(image, name);
	res->attributes = attrs;
	res->owner = 0;
	res->offset = offset;

	/* Assign a token code to the manifest resource */
	if(!_ILImageSetToken(image, &(res->programItem), token,
						 IL_META_TOKEN_MANIFEST_RESOURCE))
	{
		return 0;
	}

	/* Return the manifest resource to the caller */
	return res;
}

const char *ILManifestResGetName(ILManifestRes *res)
{
	return res->name;
}

void ILManifestResSetOwnerFile(ILManifestRes *res, ILFileDecl *decl,
							   ILUInt32 offset)
{
	res->owner = &(decl->programItem);
	res->offset = offset;
}

void ILManifestResSetOwnerAssembly(ILManifestRes *res, ILAssembly *assem)
{
	res->owner = &(assem->programItem);
}

ILFileDecl *ILManifestResGetOwnerFile(ILManifestRes *res)
{
	if(res->owner &&
	   (res->owner->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_FILE)
	{
		return (ILFileDecl *)(res->owner);
	}
	else
	{
		return 0;
	}
}

ILAssembly *ILManifestResGetOwnerAssembly(ILManifestRes *res)
{
	if(res->owner &&
	   (res->owner->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_ASSEMBLY_REF)
	{
		return (ILAssembly *)(res->owner);
	}
	else
	{
		return 0;
	}
}

ILUInt32 ILManifestResGetOffset(ILManifestRes *res)
{
	return res->offset;
}

ILUInt32 ILManifestResGetAttrs(ILManifestRes *res)
{
	return res->attributes;
}

ILExportedType *ILExportedTypeCreate(ILImage *image, ILToken token,
									 ILUInt32 attributes,
									 const char *name,
									 const char *namespace,
									 ILProgramItem *scope)
{
	ILExportedType *type;

	/* Allocate space for the exported type from the memory stack */
	type = ILMemStackAlloc(&(image->memStack), ILExportedType);
	if(!type)
	{
		return 0;
	}

	/* Set the exported type fields */
	type->classItem.programItem.image = image;
	type->classItem.attributes = attributes | IL_META_TYPEDEF_REFERENCE;
	type->classItem.className =
		_ILClassNameLookup(image, scope, 0, name, namespace);
	if(!(type->classItem.className))
	{
		type->classItem.className =
			_ILClassNameCreate(image, token, name, namespace, scope, 0);
	}
	if(!(type->classItem.className))
	{
		return 0;
	}
	type->identifier = 0;

	/* Assign a token code to the exported type */
	if(!_ILImageSetToken(image, &(type->classItem.programItem), token,
						 IL_META_TOKEN_EXPORTED_TYPE))
	{
		return 0;
	}
	_ILClassNameUpdateToken(&(type->classItem));

	/* Return the exported type to the caller */
	return type;
}

void ILExportedTypeSetId(ILExportedType *type, ILUInt32 identifier)
{
	type->identifier = identifier;
}

ILUInt32 ILExportedTypeGetId(ILExportedType *type)
{
	return type->identifier;
}

void ILExportedTypeSetScopeFile(ILExportedType *type, ILFileDecl *decl)
{
	type->classItem.className->scope = (ILProgramItem *)decl;
}

void ILExportedTypeSetScopeAssembly(ILExportedType *type, ILAssembly *assem)
{
	type->classItem.className->scope = (ILProgramItem *)assem;
}

void ILExportedTypeSetScopeType(ILExportedType *type, ILExportedType *scope)
{
	type->classItem.className->scope = (ILProgramItem *)scope;
}

ILExportedType *ILExportedTypeFind(ILImage *image,
								   const char *name,
								   const char *namespace)
{
	ILExportedType *type;
	if(!name)
	{
		return 0;
	}
	type = 0;
	while((type = (ILExportedType *)ILImageNextToken
				(image, IL_META_TOKEN_EXPORTED_TYPE, type)) != 0)
	{
		if(!strcmp(type->classItem.className->name, name))
		{
			if(!namespace)
			{
				if(!(type->classItem.className->namespace))
				{
					return type;
				}
			}
			else if(type->classItem.className->namespace)
			{
				if(!strcmp(type->classItem.className->namespace, namespace))
				{
					return type;
				}
			}
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
