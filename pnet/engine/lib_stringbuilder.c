/*
 * lib_stringbuilder.c - Internalcall methods for "System.Text.StringBuilder".
 *
 * Copyright (C) 2001, 2002, 2003  Southern Storm Software, Pty Ltd.
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
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#define MinCapacity		16
#define ResizeRound		31
#define ResizeRoundBig	1023

#define BuildString(obj) (((System_Text_StringBuilder *)obj)->buildString)
#define NeedsCopy(obj) (((System_Text_StringBuilder *)obj)->needsCopy)
#define MaxCapacity(obj) (((System_Text_StringBuilder *)obj)->maxCapacity)

/*
 * calculate the capacity for the new buildString
 *
 */
static IL_INLINE ILInt32 NewCapacity(ILInt32 length, ILInt32 oldCapacity)
{
	/* for stringbuilder >= 2048 chars add 1024 chars */
	if(length > 2047)
	{
		return (length + ResizeRoundBig) & ~ResizeRoundBig;
	}
	else
	{
		while(oldCapacity < length)
		{
			oldCapacity <<= 1;
		}
		return oldCapacity;
	}		
} 

/*
 * Append value to StringBuilder
 * assumes: value != 0
 *          length > 0
 */
static ILObject * Append(ILExecThread * _thread,
							System_Text_StringBuilder * _this,
							ILUInt16 * value,
							ILInt32 length)
{
	ILInt32 newLength = _this->buildString->length + length;
	ILInt32 newCapacity;
	System_String *str;
	if(newLength > _this->buildString->capacity)
	{
		if(newLength > MaxCapacity(_this))
		{
			ILExecThreadThrowArgRange(_thread,
									 "length",
									 "ArgRange_StrCapacity");
			return 0;
		} 
		newCapacity = NewCapacity(newLength, _this->buildString->capacity);

		if(newCapacity > MaxCapacity(_this))
		{
			newCapacity = MaxCapacity(_this);
		}
		str = _IL_String_NewBuilder(_thread, _this->buildString, newCapacity);
		if(!str)
		{
			return (ILObject *)0;
		}
		_this->buildString = str;
		NeedsCopy(_this) = (ILBool)0;
	}
	else
	{
		if(_this->needsCopy)
		{
			System_String *str = _IL_String_NewBuilder(_thread, _this->buildString,
													_this->buildString->capacity);
			if(!str)
			{
				return(ILObject *)0;
			}
			_this->buildString = str;
			_this->needsCopy = (ILBool)0;
		}
	}
	ILMemCpy(StringToBuffer(_this->buildString) + _this->buildString->length,
						value, sizeof(ILUInt16) * length);
	_this->buildString->length = newLength;
	return (ILObject *)_this;
}

/*
 *	extern public StringBuilder Append(String value);
 */
ILObject * _IL_StringBuilder_Append_String(ILExecThread * _thread,
											ILObject * _this,
											ILString * value)
{
	if(value)
	{
		if(((System_String *)value)->length > 0)
		{
			return Append(_thread, (System_Text_StringBuilder *)_this,
							StringToBuffer((System_String *)value),
							((System_String *)value)->length);
		}
	}
	return _this;
}

/*
 *	public StringBuilder Append(String value, int startIndex, int length);
 */	
ILObject * _IL_StringBuilder_Append_Stringii(ILExecThread * _thread,
											ILObject * _this,
											ILString * value,
											ILInt32 startIndex,
											ILInt32 length)
{
	if(value)
	{
		if((startIndex < 0) || (startIndex > ((System_String *)value)->length))
		{
			ILExecThreadThrowArgRange(_thread,
									 "startIndex",
									 "ArgRange_Array");
			return (ILObject *)0;
		}
		if((length < 0) || ((((System_String *)value)->length - startIndex) < length))
		{

			ILExecThreadThrowArgRange(_thread,
									 "length",
									 "ArgRange_Array");
			return (ILObject *)0;
		}
		if(length > 0)
		{
			return Append(_thread, (System_Text_StringBuilder *)_this,
							StringToBuffer((System_String *)value) + startIndex,
							length);
		}
	}
	else
	{
		if(startIndex || length)
		{
			ILExecThreadThrowArgNull(_thread, "value");
			return (ILObject *)0;
		}
	}
	return _this;
}


/*
 * 	extern public StringBuilder Append(char value);
 */
ILObject * _IL_StringBuilder_Append_c(ILExecThread * _thread,
										ILObject * _this,
										ILUInt16 value)
{
	ILInt32 newLength = BuildString(_this)->length + 1;
	ILInt32 newCapacity;
	System_String *str;
	ILUInt16 *buf;

	if(newLength > BuildString(_this)->capacity)
	{
		if(newLength > MaxCapacity(_this))
		{
			ILExecThreadThrowArgRange(_thread,
									 "value",
									 "ArgRange_StrCapacity");
			return 0;
		} 
		newCapacity = NewCapacity(newLength, BuildString(_this)->capacity);
		if(newCapacity > MaxCapacity(_this))
		{
			newCapacity = MaxCapacity(_this);
		}
		str = _IL_String_NewBuilder(_thread, BuildString(_this), newCapacity);
		if(!str)
		{
			return (ILObject *)0;
		}
		BuildString(_this) = str;
		NeedsCopy(_this) = (ILBool)0;
	}
	else
	{
		if(NeedsCopy(_this))
		{
			System_String * str = _IL_String_NewBuilder(_thread,
											BuildString(_this),
											BuildString(_this)->capacity);
            if(!str)
			{
				return(ILObject *)0;
			}
			BuildString(_this) = str;
			NeedsCopy(_this) = (ILBool)0;
		}
	}
	buf = StringToBuffer(BuildString(_this)) + BuildString(_this)->length;
	*buf = value;
	BuildString(_this)->length++;

	return _this;
}

/*
 *	extern public StringBuilder Append(char value, int repeatCount);
 */
ILObject * _IL_StringBuilder_Append_ci(ILExecThread * _thread,
										ILObject * _this,
										ILUInt16 value, ILInt32 repeatCount)
{
	ILInt32 newLength, newCapacity;
	System_String *str;
	ILUInt16 *buf;
	if(repeatCount < 0)
	{
		ILExecThreadThrowArgRange(_thread,
									 "repeatCount",
									 "ArgRange_NonNegative");
		return 0;
	}
	else
	{
		if(repeatCount == 0)
		{
			return _this;
		}
	}
	newLength = BuildString(_this)->length + repeatCount;

	if(newLength > BuildString(_this)->capacity)
	{
		if(newLength > MaxCapacity(_this))
		{
			ILExecThreadThrowArgRange(_thread,
									 "repeatCount",
									 "ArgRange_StrCapacity");
			return 0;
		} 
		newCapacity = NewCapacity(newLength, BuildString(_this)->capacity);
		if(newCapacity > MaxCapacity(_this))
		{
			newCapacity = MaxCapacity(_this);
		}
		str = _IL_String_NewBuilder(_thread, BuildString(_this), newCapacity);
		if(!str)
		{
			return (ILObject *)0;
		}
		BuildString(_this) = str;
		NeedsCopy(_this) = (ILBool)0;
	}
	else
	{
		if(NeedsCopy(_this))
		{
			System_String * str = _IL_String_NewBuilder(_thread,
											BuildString(_this),
											BuildString(_this)->capacity);
			if(!str)
			{
				return (ILObject *)0;
			}
			BuildString(_this) = str;
			NeedsCopy(_this) = (ILBool)0;
		}
	}
	buf = StringToBuffer(BuildString(_this)) + BuildString(_this)->length;
	while(repeatCount > 0)
	{
		*buf++ = value;
		repeatCount--;
	}
	BuildString(_this)->length = newLength;

	return _this;
}

/*
 *	public StringBuilder Append(char[] value);
 */	
ILObject * _IL_StringBuilder_Append_ac(ILExecThread * _thread,
										ILObject * _this, System_Array * value)
{
	if(value)
	{
		if(ArrayLength(value) > 0)
		{
			return Append(_thread, (System_Text_StringBuilder *)_this,
							(ILUInt16 *)ArrayToBuffer(value),
							ArrayLength(value));
		}
	}
	return _this;
}

/*
 * public StringBuilder Append(char[] value, int startIndex, int length);
 */
ILObject * _IL_StringBuilder_Append_acii(ILExecThread * _thread,
											ILObject * _this,
											System_Array * value,
											ILInt32 startIndex, ILInt32 length)
{
	if(value)
	{
		if(startIndex < 0 || startIndex > ArrayLength(value))
		{
			ILExecThreadThrowArgRange(_thread,
									 "startIndex",
									 "ArgRange_Array");
			return 0;
		}
		else
		{
			if(length < 0 || (ArrayLength(value) - startIndex) < length)
			{
				ILExecThreadThrowArgRange(_thread,
										 "length",
										 "ArgRange_Array");
				return 0;
			}
		}
		if(length > 0)
		{
			return Append(_thread, (System_Text_StringBuilder *)_this,
							((ILUInt16 *)ArrayToBuffer(value)) + startIndex,
							length);
		}
	}
	else
	{
		if(startIndex != 0 || length != 0)
		{
			ILExecThreadThrowArgNull(_thread, "value");
			return (ILObject *)0;
		}
	}
	return _this;
}

/*
 * public int EnsureCapacity(int capacity)
 */
ILInt32 _IL_StringBuilder_EnsureCapacity(ILExecThread * _thread,
											ILObject * _this,
											ILInt32 capacity)
{
	ILInt32 newCapacity;
	System_String *str;
	if(capacity < 0)
	{
		ILExecThreadThrowArgRange(_thread,
								 	"capacity",
									"ArgRange_NonNegative");
		return 0;
	}
	if(capacity > MaxCapacity(_this))
	{
		ILExecThreadThrowArgRange(_thread,
								 	"capacity",
									"ArgRange_StrCapacity");
		return 0;
	}
	if(capacity < BuildString(_this)->capacity)
	{
		return BuildString(_this)->capacity;
	}
	newCapacity = NewCapacity(capacity, BuildString(_this)->capacity);
	if(newCapacity > MaxCapacity(_this))
	{
		newCapacity = MaxCapacity(_this); 
	}
	str = _IL_String_NewBuilder(_thread, BuildString(_this), newCapacity);
	if(!str)
	{
		return 0;
	}
	BuildString(_this) = str;
	NeedsCopy(_this) = (ILBool)0;
	return newCapacity; 
}

/*
 * Assumes: value != Null
 * index >= 0 <= buildString.length
 * length >= 0
 */
static ILObject * Insert(ILExecThread * _thread,
							System_Text_StringBuilder * _this,
							ILInt32 index,
							ILInt32 length)
{
	ILInt32 newLength, newCapacity;
	System_String *str;
	if(index < 0 || index > _this->buildString->length)
	{
		ILExecThreadThrowArgRange(_thread,
								 	"index",
									"ArgRange_StringIndex");
		return (ILObject *)0;
	}
	newLength = _this->buildString->length + length;
	if(newLength > _this->buildString->capacity)
	{
		if(newLength > _this->maxCapacity)
		{
			ILExecThreadThrowArgRange(_thread,
									 "value",
									 "ArgRange_StrCapacity");
			return 0;
		}
		newCapacity = NewCapacity(newLength, _this->buildString->capacity);
		if(newCapacity > _this->maxCapacity)
		{
			newCapacity = _this->maxCapacity;
		}
		str = _IL_String_NewBuilder(_thread, _this->buildString, newCapacity);
		if(!str)
		{
			return 0;
		}
		if(index > 0)
		{
			ILMemCpy(StringToBuffer(str),
						StringToBuffer(_this->buildString),
						sizeof(ILUInt16) * (index - 1));
	
		}
		ILMemCpy(StringToBuffer(str) + index + length,
					StringToBuffer(_this->buildString) + index,
					sizeof(ILUInt16) * (_this->buildString->length - index));
		str->length = newLength,
		_this->buildString = str;
	}
	else
	{
		if(_this->needsCopy)
		{
			System_String * str = _IL_String_NewBuilder(_thread,
												_this->buildString,
												_this->buildString->capacity);
			if(!str)
			{
				return 0;
			}
			if(index > 0)
			{
				ILMemCpy(StringToBuffer(str),
							StringToBuffer(_this->buildString),
							sizeof(ILUInt16) * (index - 1));
			}
			ILMemCpy(StringToBuffer(str) + index + length,
						StringToBuffer(_this->buildString) + index,
						sizeof(ILUInt16) * (_this->buildString->length - index));
			str->length = newLength,
			_this->buildString = str;
		}
		else
		{
			ILMemMove(StringToBuffer(_this->buildString) + index + length,
					StringToBuffer(_this->buildString) + index,
					sizeof(ILUInt16) * (_this->buildString->length - index));
			_this->buildString->length = newLength;
		}
	}
	return (ILObject *)_this;
}

/*
 * public StringBuilder Insert(int index, String value)
 */
ILObject * _IL_StringBuilder_Insert_iString(ILExecThread * _thread,
											ILObject * _this,
											ILInt32 index,
											ILString * value)
{
	if(value)
	{
		if(((System_String *)value)->length > 0)
		{
			if(Insert(_thread, (System_Text_StringBuilder *)_this,
							index,
							((System_String *)value)->length))
			{
				ILMemCpy(StringToBuffer(BuildString(_this)) + index,
						StringToBuffer(value),
						sizeof(ILUInt16) * ((System_String *)value)->length);
	
			}
			else
			{
				return (ILObject *)0;
			}
		}
	}
	return _this;
}

/*
 * public StringBuilder Insert(int index, char value)
 */
ILObject * _IL_StringBuilder_Insert_ic(ILExecThread * _thread,
										ILObject * _this,
										ILInt32 index,
										ILUInt16 value)
{
	if(Insert(_thread, (System_Text_StringBuilder *)_this,
				index,
				1))
	{
		*(StringToBuffer(BuildString(_this)) + index) = value;
	}
	else
	{
		return (ILObject *)0;
	}
	return _this;
}

/*
 * 	public StringBuilder Insert(int index, char[] value)
 */
ILObject * _IL_StringBuilder_Insert_iac(ILExecThread * _thread,
										ILObject * _this,
										ILInt32 index,
										System_Array * value)
{
	if(value)
	{
		if(ArrayLength(value))
		{
			if(Insert(_thread, (System_Text_StringBuilder *)_this,
							index,
							ArrayLength(value)))
			{
				ILMemCpy(StringToBuffer(BuildString(_this)) + index,
						ArrayToBuffer(value),
						sizeof(ILUInt16) * ArrayLength(value));
	
			}
			else
			{
				return (ILObject *)0;
			}
		}
	}
	return _this;
}

/*
 *  public StringBuilder Insert(int index, char[] value,
 *								int startIndex, int length)
 */
ILObject * _IL_StringBuilder_Insert_iacii(ILExecThread * _thread,
											ILObject * _this,
											ILInt32 index,
											System_Array * value,
											ILInt32 startIndex,
											ILInt32 length)
{
	if(value)
	{
		if(startIndex < 0 || startIndex > ArrayLength(value))
		{
			ILExecThreadThrowArgRange(_thread,
									 "startIndex",
									 "ArgRange_Array");
			return 0;
		}
		if((ArrayLength(value) - startIndex) < length)
		{
			ILExecThreadThrowArgRange(_thread,
									 "length",
									 "ArgRange_Array");
			return 0;
		}
		if(length > 0)
		{
			if(Insert(_thread, (System_Text_StringBuilder *)_this,
							index,
							length))
			{
				ILMemCpy(StringToBuffer(BuildString(_this)) + index,
						((ILInt16 *)ArrayToBuffer(value)) + startIndex,
						sizeof(ILUInt16) * length);
			}
			else
			{
				return (ILObject *)0;
			}
		}
	}
	else
	{
		if(startIndex != 0 || length != 0)
		{
			ILExecThreadThrowArgNull(_thread, "value");
			return (ILObject *)0;
		}
	}
	return _this;
}

/*
 *  public StringBuilder Insert(int index, String value, int count)
 */
ILObject * _IL_StringBuilder_Insert_iStringi(ILExecThread * _thread,
												ILObject * _this,
												ILInt32 index,
												ILString * value,
												ILInt32 count)
{
	if(count < 0)
	{
			ILExecThreadThrowArgRange(_thread,
									 "count",
									 "ArgRange_NonNegative");
			return 0;
	}
	if(value)
	{
		if(((System_String *)value)->length > 0 && count > 0)
		{
			if(Insert(_thread, (System_Text_StringBuilder *)_this,
							index,
							((System_String *)value)->length * count))
			{
				ILUInt16 *buf = StringToBuffer(BuildString(_this)) + index;
				while(count > 0)
				{
					ILMemCpy(buf,
						StringToBuffer((System_String *)value),
						sizeof(ILUInt16) * ((System_String *)value)->length);
					buf += ((System_String *)value)->length;
					count--;
				}
			}
			else
			{
				return (ILObject *)0;
			}
		}
	}
	return _this;
}

/*
 * public StringBuilder Replace(char oldChar, char newChar)
 */
ILObject * _IL_StringBuilder_Replace_cc(ILExecThread * _thread,
										ILObject * _this,
										ILUInt16 oldChar,
										ILUInt16 newChar)
{
	ILInt32 length;
	ILUInt16 *buf;
	if(oldChar != newChar)
	{
		if(NeedsCopy(_this))
		{
			System_String * str = _IL_String_NewBuilder(_thread,
												BuildString(_this),
												BuildString(_this)->capacity);
			if(!str)
			{
				return 0;
			}
			BuildString(_this) = str;
			NeedsCopy(_this) = (ILBool)0;
		}
		length = BuildString(_this)->length;
		buf = StringToBuffer(BuildString(_this));
		while(length > 0)
		{
			if(*buf == oldChar)
			{
				*buf = newChar;
			}
			buf++;
			length--;
		}
	}
	return _this;
}

/*
 * public StringBuilder Replace(char oldChar, char newChar,
 *								 int startIndex, int count);
 */
ILObject * _IL_StringBuilder_Replace_ccii(ILExecThread * _thread,
											ILObject * _this,
											ILUInt16 oldChar,
											ILUInt16 newChar,
											ILInt32 startIndex,
											ILInt32 count)
{
	ILUInt16 *buf;
	if(startIndex < 0 || startIndex > BuildString(_this)->length)
	{
		ILExecThreadThrowArgRange(_thread,
									 "startIndex",
									 "ArgRange_StringIndex");
		return 0;
	}
	if((BuildString(_this)->length - startIndex) < count)
	{
		ILExecThreadThrowArgRange(_thread,
									 "count",
									 "ArgRange_StringRange");
		return 0;
	}
	if(oldChar != newChar)
	{
		if(NeedsCopy(_this))
		{
			System_String * str = _IL_String_NewBuilder(_thread,
												BuildString(_this),
												BuildString(_this)->capacity);
			if(!str)
			{
				return 0;
			}
			BuildString(_this) = str;
			NeedsCopy(_this) = (ILBool)0;
		}
		buf = StringToBuffer(BuildString(_this)) + startIndex;
		while(count > 0)
		{
			if(*buf == oldChar)
			{
				*buf = newChar;
			}
			buf++;
			count--;
		}
	}
	return _this;
}

#ifdef	__cplusplus
};
#endif
