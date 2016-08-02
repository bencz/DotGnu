/*
 * link_field.c - Convert a field and copy it to the final image.
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

#include "linker.h"

#ifdef	__cplusplus
extern	"C" {
#endif

int _ILLinkerConvertField(ILLinker *linker, ILField *field, ILClass *newClass)
{
	ILField *newField;
	const char *name = ILField_Name(field);
	ILType *type = ILFieldGetTypeWithPrefixes(field);
	char *newName = 0;
	ILPInvoke *pinvoke;
	ILModule *module;

	/* Rename the field if it is within the "<Module>" class and private */
	if(ILField_IsPrivate(field) && ILField_IsStatic(field) &&
	   _ILLinkerIsModule(ILMember_Owner(field)))
	{
		newName = _ILLinkerNewMemberName(linker, (ILMember *)field);
		if(newName)
		{
			name = newName;
		}
	}

	/* See if we already have a definition of this field in the class */
	newField = 0;
	type = _ILLinkerConvertType(linker, type);
	if(!type)
	{
		if(newName)
		{
			ILFree(newName);
		}
		return 0;
	}
	if((newField = (ILField *)ILClassNextMemberMatch
			(newClass, (ILMember *)0,
			 IL_META_MEMBERKIND_FIELD, name, type)) != 0)
	{
		/* Bail out if the field is already defined.  This shouldn't
		   happen very often because duplicate classes are trapped long
		   before control gets to here.  Global fields may result in
		   this code being used, however */
		if((ILField_Token(newField) & IL_META_TOKEN_MASK)
				!= IL_META_TOKEN_MEMBER_REF)
		{
			if(newName)
			{
				ILFree(newName);
			}
			return 1;
		}

		/* Set the type to the new value, just in case the previous
		   reference did not involve modifiers */
		ILMemberSetSignature((ILMember *)newField, type);

		/* Allocate a new token for the field */
		if(!ILFieldNewToken(newField))
		{
			_ILLinkerOutOfMemory(linker);
			if(newName)
			{
				ILFree(newName);
			}
			return 0;
		}
	}
	if(!newField)
	{
		/* Create the field within the new class */
		newField = ILFieldCreate(newClass, 0, name, ILField_Attrs(field));
		if(!newField)
		{
			_ILLinkerOutOfMemory(linker);
			if(newName)
			{
				ILFree(newName);
			}
			return 0;
		}

		/* Apply the converted type to the field */
		ILMemberSetSignature((ILMember *)newField, type);
	}
	else
	{
		/* Set the attribute flags to their correct values */
		ILMemberSetAttrs((ILMember *)newField, ~((ILUInt32)0),
						 ILField_Attrs(field));
	}
	if(newName)
	{
		ILFree(newName);
	}

	/* Update the symbol definition if this is in the global module */
	if(_ILLinkerIsModule(ILMember_Owner(newField)) ||
	   _ILLinkerIsGlobalScope(ILMember_Owner(newField)))
	{
		_ILLinkerUpdateSymbol(linker, ILMember_Name(newField),
							  (ILMember *)newField);
	}

	/* Convert the attributes that are attached to the field */
	if(!_ILLinkerConvertAttrs(linker, (ILProgramItem *)field,
							  (ILProgramItem *)newField))
	{
		return 0;
	}

	/* Convert the debug information that is attached to the field */
	if(!_ILLinkerConvertDebug(linker, (ILProgramItem *)field,
							  (ILProgramItem *)newField))
	{
		return 0;
	}

	/* Convert the field marshalling and layout information */
	if(!_ILLinkerConvertMarshal(linker, (ILProgramItem *)field,
								(ILProgramItem *)newField, 0))
	{
		return 0;
	}

	/* Convert the PInvoke information for the method */
	pinvoke = ILPInvokeFindField(field);
	if(pinvoke)
	{
		module = ILPInvoke_Module(pinvoke);
		if(module)
		{
			module = ILModuleRefCreateUnique(linker->image,
											 ILModule_Name(module));
			if(!module)
			{
				_ILLinkerOutOfMemory(linker);
				return 0;
			}
		}
		if(!ILPInvokeFieldCreate(newField, 0, ILPInvoke_Attrs(pinvoke),
							     module, ILPInvoke_Alias(pinvoke)))
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
	}

	/* Done */
	return 1;
}

int _ILLinkerConvertConstant(ILLinker *linker, ILProgramItem *oldItem,
							 ILProgramItem *newItem)
{
	ILConstant *constant;

	constant = ILConstantGetFromOwner(oldItem);
	if(constant)
	{
		ILConstant *newConstant;
		const void *blob;
		ILUInt32 blobLen;

		newConstant = ILConstantCreate(linker->image, 0, newItem,
									   ILConstant_ElemType(constant));
		if(!newConstant)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		blob = ILConstantGetValue(constant, &blobLen);
		if(blob)
		{
			if(!ILConstantSetValue(newConstant, blob, blobLen))
			{
				_ILLinkerOutOfMemory(linker);
				return 0;
			}
		}
	}
	return 1;
}

int _ILLinkerConvertMarshal(ILLinker *linker, ILProgramItem *oldItem,
						    ILProgramItem *newItem, int isParam)
{
	ILFieldMarshal *marshal;
	ILFieldMarshal *newMarshal;
	ILFieldLayout *layout;
	ILFieldRVA *rva;
	const void *blob;
	ILUInt32 blobLen;

	/* Convert the marshalling information */
	marshal = ILFieldMarshalGetFromOwner(oldItem);
	if(marshal)
	{
		newMarshal = ILFieldMarshalCreate(linker->image, 0, newItem);
		if(!newMarshal)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		blob = ILFieldMarshalGetType(marshal, &blobLen);
		if(blob)
		{
			if(!ILFieldMarshalSetType(newMarshal, blob, blobLen))
			{
				_ILLinkerOutOfMemory(linker);
				return 0;
			}
		}
	}

	/* Convert the constant information */
	if(!_ILLinkerConvertConstant(linker, oldItem, newItem))
	{
		return 0;
	}

	/* Bail out now if we are processing a parameter */
	if(isParam)
	{
		return 1;
	}

	/* Convert the field layout information */
	layout = ILFieldLayoutGetFromOwner((ILField *)oldItem);
	if(layout)
	{
		layout = ILFieldLayoutCreate(linker->image, 0, (ILField *)newItem,
									 ILFieldLayout_Offset(layout));
		if(!layout)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
	}

	/* Convert the field RVA information */
	rva = ILFieldRVAGetFromOwner((ILField *)oldItem);
	if(rva)
	{
		unsigned long rvaValue;
		unsigned long dataRVA;
		ILUInt32 dataSize;
		unsigned long tlsRVA;
		ILUInt32 tlsSize;

		/* Convert the RVA value from the old image to the new one */
		rvaValue = ILFieldRVA_RVA(rva);
		dataRVA = ILImageGetSectionAddr(ILProgramItem_Image(oldItem),
										IL_SECTION_DATA);
		dataSize = ILImageGetSectionSize(ILProgramItem_Image(oldItem),
										 IL_SECTION_DATA);
		tlsRVA = ILImageGetSectionAddr(ILProgramItem_Image(oldItem),
									   IL_SECTION_TLS);
		tlsSize = ILImageGetSectionSize(ILProgramItem_Image(oldItem),
										IL_SECTION_TLS);
		if(rvaValue >= dataRVA && rvaValue < (dataRVA + dataSize))
		{
			rvaValue = rvaValue - dataRVA + linker->dataLength;
		}
		else if(rvaValue >= tlsRVA && rvaValue < (tlsRVA + tlsSize))
		{
			rvaValue = rvaValue - tlsRVA + linker->tlsLength;
			rvaValue |= (unsigned long)0x80000000;
		}
		else
		{
			rvaValue = 0;
		}

		/* Create an RVA token in the new image */
		rva = ILFieldRVACreate(linker->image, 0, (ILField *)newItem,
							   (ILUInt32)rvaValue);
		if(!rva)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
	}

	/* Done */
	return 1;
}

#ifdef	__cplusplus
};
#endif
