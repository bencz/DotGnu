/*
 * cs_semvalue.c - Semantic value handling.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#include <cscc/csharp/cs_internal.h>

#ifdef	__cplusplus
extern	"C" {
#endif

CSSemValue CSSemValueDefault = {CS_SEMKIND_VOID, 0, 0};
CSSemValue CSSemValueError = {CS_SEMKIND_ERROR, 0, 0};

void _CSSemReplaceWithConstant(ILNode **parent, ILEvalValue *value)
{
	if(!value)
	{
		return;
	}
	switch(value->valueType)
	{
		case ILMachineType_Boolean:
		{
			if(value->un.i4Value)
			{
				*parent = ILNode_True_create();
			}
			else
			{
				*parent = ILNode_False_create();
			}
		}
		break;

		case ILMachineType_Int8:
		{
			if(value->un.i4Value >= 0)
			{
				*parent = ILNode_Int8_create
					((ILUInt64)(ILInt64)(value->un.i4Value), 0, 0);
			}
			else
			{
				*parent = ILNode_Int8_create
					((ILUInt64)(ILInt64)(-(value->un.i4Value)), 1, 0);
			}
		}
		break;

		case ILMachineType_UInt8:
		{
			*parent = ILNode_UInt8_create
				((ILUInt64)(ILInt64)(value->un.i4Value), 0, 0);
		}
		break;

		case ILMachineType_Int16:
		{
			if(value->un.i4Value >= 0)
			{
				*parent = ILNode_Int16_create
					((ILUInt64)(ILInt64)(value->un.i4Value), 0, 0);
			}
			else
			{
				*parent = ILNode_Int16_create
					((ILUInt64)(ILInt64)(-(value->un.i4Value)), 1, 0);
			}
		}
		break;

		case ILMachineType_UInt16:
		{
			*parent = ILNode_UInt16_create
				((ILUInt64)(ILInt64)(value->un.i4Value), 0, 0);
		}
		break;

		case ILMachineType_Char:
		{
			*parent = ILNode_Char_create
				((ILUInt64)(ILInt64)(value->un.i4Value), 0, 0);
		}
		break;

		case ILMachineType_Int32:
		{
			if(value->un.i4Value >= 0)
			{
				*parent = ILNode_Int32_create
					((ILUInt64)(ILInt64)(value->un.i4Value), 0, 0);
			}
			else
			{
				*parent = ILNode_Int32_create
					((ILUInt64)(-((ILInt64)(value->un.i4Value))), 1, 0);
			}
		}
		break;

		case ILMachineType_UInt32:
		{
			*parent = ILNode_UInt32_create
				((ILUInt64)(ILUInt32)(value->un.i4Value), 0, 0);
		}
		break;

		case ILMachineType_Int64:
		{
			if(value->un.i8Value >= 0)
			{
				*parent = ILNode_Int64_create
					((ILUInt64)(value->un.i8Value), 0, 0);
			}
			else
			{
				*parent = ILNode_Int64_create
					((ILUInt64)(-(value->un.i8Value)), 1, 0);
			}
		}
		break;

		case ILMachineType_UInt64:
		{
			*parent = ILNode_UInt64_create
				((ILUInt64)(value->un.i8Value), 0, 0);
		}
		break;

		case ILMachineType_NativeInt:
		{
			if(value->un.i4Value >= 0)
			{
				*parent = ILNode_Int32_create
					((ILUInt64)(ILInt64)(value->un.i4Value), 0, 0);
			}
			else
			{
				*parent = ILNode_Int32_create
					((ILUInt64)(-((ILInt64)(value->un.i4Value))), 1, 0);
			}
			*parent = ILNode_CastSimple_create(*parent,
											   ILMachineType_NativeInt);
		}
		break;

		case ILMachineType_NativeUInt:
		{
			*parent = ILNode_UInt32_create
				((ILUInt64)(ILUInt32)(value->un.i4Value), 0, 0);
			*parent = ILNode_CastSimple_create(*parent,
											   ILMachineType_NativeUInt);
		}
		break;

		case ILMachineType_Float32:
		{
			*parent = ILNode_Float32_create((ILDouble)(value->un.r4Value));
		}
		break;

		case ILMachineType_Float64:
		{
			*parent = ILNode_Float64_create(value->un.r8Value);
		}
		break;

		case ILMachineType_NativeFloat:
		{
			*parent = ILNode_Float_create(value->un.r8Value);
		}
		break;

		case ILMachineType_Decimal:
		{
			*parent = ILNode_Decimal_create(value->un.decValue);
		}
		break;

		case ILMachineType_String:
		{
			*parent = ILNode_String_create(value->un.strValue.str,
										   value->un.strValue.len);
		}
		break;

		case ILMachineType_ObjectRef:
		{
			*parent = ILNode_Null_create();
		}
		break;

		case ILMachineType_UnmanagedPtr:
		{
			*parent = ILNode_NullPtr_create();
		}
		break;

		default: break;
	}
}

#ifdef	__cplusplus
};
#endif
