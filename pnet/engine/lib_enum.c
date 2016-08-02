/*
 * lib_enum.c - Internalcall methods for the "System.Enum" class.
 *
 * Copyright (C) 2002, 2009  Southern Storm Software, Pty Ltd.
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

#include "engine.h"
#include "lib_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Get the raw form of an enumerated value.
 */
static ILInt64 GetRawEnumValue(ILExecThread *thread, ILObject *value)
{
	ILType *valueType = ILClassToType(GetObjectClass(value));
	ILType *underlying = ILTypeGetEnumType(valueType);
	if(ILType_IsPrimitive(underlying))
	{
		switch(ILType_ToElement(underlying))
		{
			case IL_META_ELEMTYPE_I1:
			{
				ILInt8 newValue;
				if(ILExecThreadUnbox(thread, valueType, value, &newValue))
				{
					return (ILInt64)newValue;
				}
			}
			break;

			case IL_META_ELEMTYPE_U1:
			{
				ILUInt8 newValue;
				if(ILExecThreadUnbox(thread, valueType, value, &newValue))
				{
					return (ILInt64)newValue;
				}
			}
			break;

			case IL_META_ELEMTYPE_I2:
			{
				ILInt16 newValue;
				if(ILExecThreadUnbox(thread, valueType, value, &newValue))
				{
					return (ILInt64)newValue;
				}
			}
			break;

			case IL_META_ELEMTYPE_U2:
			{
				ILUInt16 newValue;
				if(ILExecThreadUnbox(thread, valueType, value, &newValue))
				{
					return (ILInt64)newValue;
				}
			}
			break;

			case IL_META_ELEMTYPE_I4:
			{
				ILInt32 newValue;
				if(ILExecThreadUnbox(thread, valueType, value, &newValue))
				{
					return (ILInt64)newValue;
				}
			}
			break;

			case IL_META_ELEMTYPE_U4:
			{
				ILUInt32 newValue;
				if(ILExecThreadUnbox(thread, valueType, value, &newValue))
				{
					return (ILInt64)newValue;
				}
			}
			break;

			case IL_META_ELEMTYPE_I8:
			{
				ILInt64 newValue;
				if(ILExecThreadUnbox(thread, valueType, value, &newValue))
				{
					return (ILInt64)newValue;
				}
			}
			break;

			case IL_META_ELEMTYPE_U8:
			{
				ILUInt64 newValue;
				if(ILExecThreadUnbox(thread, valueType, value, &newValue))
				{
					return (ILInt64)newValue;
				}
			}
			break;
		}
	}
	return 0;
}

/*
 * Pack the raw form of an enumerated value into an object.
 */
static ILObject *PackRawEnumValue(ILExecThread *thread, ILType *enumType,
								  ILInt64 value)
{
	ILType *underlying = ILTypeGetEnumType(enumType);
	if(ILType_IsPrimitive(underlying))
	{
		switch(ILType_ToElement(underlying))
		{
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			{
				ILInt8 newValue = (ILInt8)value;
				return ILExecThreadBox(thread, enumType, &newValue);
			}
			/* Not reached */

			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			{
				ILInt16 newValue = (ILInt16)value;
				return ILExecThreadBox(thread, enumType, &newValue);
			}
			/* Not reached */

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
			{
				ILInt32 newValue = (ILInt32)value;
				return ILExecThreadBox(thread, enumType, &newValue);
			}
			/* Not reached */

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
			{
				return ILExecThreadBox(thread, enumType, &value);
			}
			/* Not reached */
		}
	}
	return 0;
}

/*
 * Get the mask value to use for an enumerated type.
 */
static ILInt64 GetEnumMaskValue(ILType *enumType)
{
	enumType = ILTypeGetEnumType(enumType);
	if(ILType_IsPrimitive(enumType))
	{
		switch(ILType_ToElement(enumType))
		{
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:		return (ILInt64)0xFF;

			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:		return (ILInt64)0xFFFF;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:		return (ILInt64)IL_MAX_UINT32;
		}
	}
	return (ILInt64)IL_MAX_UINT64;
}

/*
 * Get the next enumerated value field from a class.
 */
static ILField *GetNextEnumField(ILClass *classInfo, ILField *last,
								 ILInt64 mask, ILInt64 *value)
{
	ILConstant *constValue;
	const void *constBuf;
	ILUInt32 constBufLen;

	while((last = (ILField *)ILClassNextMemberByKind
					(classInfo, (ILMember *)last,
					 IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(ILField_IsStatic(last) && ILField_IsLiteral(last))
		{
			/* This is a candidate: check for a constant value */
			constValue = ILConstantGetFromOwner((ILProgramItem *)last);
			if(!constValue)
			{
				continue;
			}
			constBuf = ILConstantGetValue(constValue, &constBufLen);
			if(!constBuf)
			{
				continue;
			}

			/* Decode the constant value */
			switch(ILConstantGetElemType(constValue))
			{
				case IL_META_ELEMTYPE_I1:
				{
					if(constBufLen >= 1)
					{
						*value = (ILInt64)((*((ILInt8 *)constBuf)) 
													& (ILInt8) mask);
						return last;
					}
				}
				break;

				case IL_META_ELEMTYPE_U1:
				{
					if(constBufLen >= 1)
					{
						*value = (ILInt64)((*((ILUInt8 *)constBuf)) 
													& (ILUInt8) mask);
						return last;
					}
				}
				break;

				case IL_META_ELEMTYPE_I2:
				{
					if(constBufLen >= 2)
					{
						*value = (ILInt64)((IL_READ_INT16(constBuf)) 
													& (ILInt16) mask);
						return last;
					}
				}
				break;

				case IL_META_ELEMTYPE_U2:
				{
					if(constBufLen >= 2)
					{
						*value = (ILInt64)((IL_READ_UINT16(constBuf)) 
													& (ILUInt16) mask);
						return last;
					}
				}
				break;

				case IL_META_ELEMTYPE_I4:
				{
					if(constBufLen >= 4)
					{
						*value = (ILInt64)((IL_READ_INT32(constBuf)) 
													& (ILInt32) mask);
						return last;
					}
				}
				break;

				case IL_META_ELEMTYPE_U4:
				{
					if(constBufLen >= 4)
					{
						*value = (ILInt64)((IL_READ_UINT32(constBuf)) 
													& (ILUInt32) mask);
						return last;
					}
				}
				break;

				case IL_META_ELEMTYPE_I8:
				{
					if(constBufLen >= 8)
					{
						*value = IL_READ_INT64(constBuf);
						return last;
					}
				}
				break;

				case IL_META_ELEMTYPE_U8:
				{
					if(constBufLen >= 8)
					{
						*value = (ILInt64)IL_READ_UINT64(constBuf);
						return last;
					}
				}
				break;
			}
		}
	}
	return 0;
}

/*
 * private Object GetEnumValue();
 */
ILObject *_IL_Enum_GetEnumValue(ILExecThread *thread, ILObject *_this)
{
	ILClass *classInfo = GetObjectClass(_this);
	ILType *underlying = ILTypeGetEnumType(ILType_FromValueType(classInfo));
	return ILExecThreadBox(thread, underlying, (void *)_this);
}

/*
 * private static String GetEnumName(Type enumType, Object value);
 */
ILString *_IL_Enum_GetEnumName(ILExecThread *thread, ILObject *enumType,
							   ILObject *value)
{
	ILClass *classInfo = _ILGetClrClass(thread, enumType);
	ILInt64 enumValue = GetRawEnumValue(thread, value);
	ILField *field = 0;
	ILInt64 mask = GetEnumMaskValue(ILType_FromValueType(classInfo));
	ILInt64 fieldValue;
	while((field = GetNextEnumField(classInfo, field, mask, &fieldValue)) != 0)
	{
		if(fieldValue == enumValue)
		{
			return ILStringCreate(thread, ILField_Name(field));
		}
	}
	return 0;
}

/*
 * private static bool IsEnumValue(Type enumType, Object value);
 */
ILBool _IL_Enum_IsEnumValue(ILExecThread *thread, ILObject *enumType,
							ILObject *value)
{
	ILClass *classInfo = _ILGetClrClass(thread, enumType);
	ILInt64 enumValue = GetRawEnumValue(thread, value);
	ILField *field = 0;
	ILInt64 mask = GetEnumMaskValue(ILType_FromValueType(classInfo));
	ILInt64 fieldValue;
	while((field = GetNextEnumField(classInfo, field, mask, &fieldValue)) != 0)
	{
		if(fieldValue == enumValue)
		{
			return 1;
		}
	}
	return 0;
}

/*
 * private static Object GetEnumValueFromName(Type enumType,
 *											  String name,
 *											  bool ignoreCase);
 */
ILObject *_IL_Enum_GetEnumValueFromName(ILExecThread *thread,
										ILObject *enumType,
										ILString *_name,
										ILBool ignoreCase)
{
	char *name;
	ILClass *classInfo;
	ILField *field;
	ILInt64 mask;
	ILInt64 fieldValue;

	/* Convert the string into a name we can compare */
	name = ILStringToUTF8(thread, _name);
	if(!name)
	{
		return 0;
	}

	/* Search for a name match */
	classInfo = _ILGetClrClass(thread, enumType);
	mask = GetEnumMaskValue(ILType_FromValueType(classInfo));
	field = 0;
	while((field = GetNextEnumField(classInfo, field, mask, &fieldValue)) != 0)
	{
		if(ignoreCase)
		{
			if(!ILStrICmp(ILField_Name(field), name))
			{
				return PackRawEnumValue(thread, ILClassToType(classInfo),
										fieldValue);
			}
		}
		else
		{
			if(!strcmp(ILField_Name(field), name))
			{
				return PackRawEnumValue(thread, ILClassToType(classInfo),
										fieldValue);
			}
		}
	}

	/* Try to convert the value as an integer */
	fieldValue = 0;
	if(name[0] == '0' && (name[1] == 'x' || name[1] == 'X') && name[2] != '\0')
	{
		/* Hexadecimal integer value */
		name += 2;
		while(*name != '\0')
		{
			fieldValue *= 16;
			if(*name >= '0' && *name <= '9')
			{
				fieldValue += (ILInt64)(*name - '0');
			}
			else if(*name >= 'a' && *name <= 'f')
			{
				fieldValue += (ILInt64)(*name - 'a' + 10);
			}
			else if(*name >= 'A' && *name <= 'F')
			{
				fieldValue += (ILInt64)(*name - 'A' + 10);
			}
			else
			{
				break;
			}
			name++;
		}
	}
	else
	{
		/* Decimal integer value */
		while(*name != '\0')
		{
			fieldValue *= 10;
			if(*name >= '0' && *name <= '9')
			{
				fieldValue += (ILInt64)(*name - '0');
				name++;
			}
			else
			{
				break;
			}
		}
	}
	if(*name == '\0')
	{
		return PackRawEnumValue(thread, ILClassToType(classInfo),
								fieldValue & mask);
	}

	/* We weren't able to convert the value */
	return 0;
}

/*
 * private static Object EnumValueOr(Object value1, Object value2);
 */
ILObject *_IL_Enum_EnumValueOr(ILExecThread *thread,
							   ILObject *value1,
							   ILObject *value2)
{
	ILInt64 value = GetRawEnumValue(thread, value1) |
					GetRawEnumValue(thread, value2);
	return PackRawEnumValue(thread, ILClassToType(GetObjectClass(value1)),
							value);
}

/*
 * private static Object EnumIntToObject(Type enumType, int value);
 */
ILObject *_IL_Enum_EnumIntToObject(ILExecThread *thread,
								   ILObject *enumType,
								   ILInt32 value)
{
	ILClass *classInfo = _ILGetClrClass(thread, enumType);
	ILType *underlying = ILTypeGetEnumType(ILType_FromValueType(classInfo));
	if(ILType_IsPrimitive(underlying))
	{
		switch(ILType_ToElement(underlying))
		{
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			{
				ILInt8 newValue = (ILInt8)value;
				return ILExecThreadBox
					(thread, ILType_FromValueType(classInfo), &newValue);
			}
			/* Not reached */

			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			{
				ILInt16 newValue = (ILInt16)value;
				return ILExecThreadBox
					(thread, ILType_FromValueType(classInfo), &newValue);
			}
			/* Not reached */

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
			{
				return ILExecThreadBox
					(thread, ILType_FromValueType(classInfo), &value);
			}
			/* Not reached */

			case IL_META_ELEMTYPE_I8:
			{
				ILInt64 newValue = (ILInt64)value;
				return ILExecThreadBox
					(thread, ILType_FromValueType(classInfo), &newValue);
			}
			/* Not reached */

			case IL_META_ELEMTYPE_U8:
			{
				ILUInt64 newValue = (ILUInt64)(ILUInt32)value;
				return ILExecThreadBox
					(thread, ILType_FromValueType(classInfo), &newValue);
			}
			/* Not reached */
		}
	}
	return 0;
}

/*
 * private static Object EnumLongToObject(Type enumType, int value);
 */
ILObject *_IL_Enum_EnumLongToObject(ILExecThread *thread,
								    ILObject *enumType,
								    ILInt64 value)
{
	ILClass *classInfo = _ILGetClrClass(thread, enumType);
	return PackRawEnumValue(thread, ILClassToType(classInfo), value);
}

/*
 * Information that is recorded for a field match.
 */
typedef struct
{
	ILInt64		value;
	ILField	   *field;

} EnumFieldMatch;

/*
 * private static String FormatEnumWithFlags(Type enumType, Object value);
 */
ILString *_IL_Enum_FormatEnumWithFlags(ILExecThread *thread,
									   ILObject *enumType,
									   ILObject *value)
{
	ILClass *classInfo;
	ILInt64 enumValue;
	ILField *field;
	ILInt64 mask;
	ILInt64 fieldValue;
	ILInt64 leftOver;
	EnumFieldMatch matchBuf[4];
	EnumFieldMatch *matches = matchBuf;
	EnumFieldMatch *newMatchBuf;
	int numMatches = 0;
	int maxMatches = 4;
	int index, index2;
	ILString *result;
	ILString *comma;
	ILString *temp;

	/* Get the class and enumeration mask */
	classInfo = _ILGetClrClass(thread, enumType);
	mask = GetEnumMaskValue(ILType_FromValueType(classInfo));

	/* Convert the value from object form into raw form */
	enumValue = GetRawEnumValue(thread, value);
	leftOver = enumValue;
	
	/* Find all fields that overlap with the incoming value */
	field = 0;
	while((field = GetNextEnumField(classInfo, field, mask, &fieldValue)) != 0)
	{
		/* Check for the easy case of an exact match */
		if(fieldValue == enumValue)
		{			
			if(maxMatches > 4)
			{
				ILFree(matches);
			}
			return ILStringCreate(thread, ILField_Name(field));
		}

		/* Ignore the field if there is no overlap with the value */
		if((fieldValue & enumValue) != fieldValue)
		{
			continue;
		}

		/* Filter out existing values that are a subset of this one */
		for(index = 0; index < numMatches; ++index)
		{
			if((matches[index].value & fieldValue) == matches[index].value)
			{
				for(index2 = index; index2 < (numMatches - 1); ++index2)
				{
					matches[index2] = matches[index2 + 1];
				}
				--numMatches;
				--index;
			}
		}

		/* Add the field to the match list */
		if(numMatches >= maxMatches)
		{
			newMatchBuf = (EnumFieldMatch *)ILMalloc
					(sizeof(EnumFieldMatch) * (maxMatches + 4));
			if(!newMatchBuf)
			{
				if(maxMatches > 4)
				{
					ILFree(matches);
				}
				ILExecThreadThrowOutOfMemory(thread);
				return 0;
			}
			ILMemCpy(newMatchBuf, matches, sizeof(EnumFieldMatch) * maxMatches);
			if(maxMatches > 4)
			{
				ILFree(matches);
			}
			matches = newMatchBuf;
			maxMatches += 4;
		}
		matches[numMatches].value = fieldValue;
		matches[numMatches].field = field;
		++numMatches;

		/* Remove the current bits from "leftOver" */
		leftOver &= ~fieldValue;
	}

	/* If there are left over bits or no matches,
	   then format the value as decimal */
	if(leftOver != 0 || !numMatches)
	{
		if(maxMatches > 4)
		{
			ILFree(matches);
		}
		return ILObjectToString(thread, value);
	}

	/* Build a string that contains the field names separated by commas */
	result = ILStringCreate(thread, ILField_Name(matches[0].field));
	if(!result)
	{
		if(maxMatches > 4)
		{
			ILFree(matches);
		}
		return 0;
	}
	comma = 0;
	for(index = 1; index < numMatches; ++index)
	{
		if(!comma)
		{
			comma = ILStringCreate(thread, ", ");
			if(!comma)
			{
				if(maxMatches > 4)
				{
					ILFree(matches);
				}
				return 0;
			}
		}
		temp = ILStringCreate(thread, ILField_Name(matches[index].field));
		if(!temp)
		{
			if(maxMatches > 4)
			{
				ILFree(matches);
			}
			return 0;
		}
		result = ILStringConcat3(thread, result, comma, temp);
		if(!result)
		{
			if(maxMatches > 4)
			{
				ILFree(matches);
			}
			return 0;
		}
	}

	/* Finished */
	if(maxMatches > 4)
	{
		ILFree(matches);
	}
	return result;
}

#ifdef	__cplusplus
};
#endif
