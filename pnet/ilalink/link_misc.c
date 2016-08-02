/*
 * link_misc.c - Convert misc program items and copy them to the final image.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

/*
 * Convert the semantic declarations that are attached
 * to a property or event.
 */
static int ConvertSemantics(ILLinker *linker, ILProgramItem *oldItem,
							ILProgramItem *newItem)
{
	ILUInt32 type = 1;
	ILMethod *method;
	while(type < 0x100)
	{
		method = ILMethodSemGetByType(oldItem, type);
		if(method)
		{
			method = (ILMethod *)_ILLinkerConvertMemberRef
						(linker, (ILMember *)method);
			if(!method)
			{
				return 0;
			}
			if(!ILMethodSemCreate(newItem, 0, type, method))
			{
				_ILLinkerOutOfMemory(linker);
				return 0;
			}
		}
		type <<= 1;
	}
	return 1;
}

int _ILLinkerConvertProperty(ILLinker *linker, ILProperty *property,
						     ILClass *newClass)
{
	ILProperty *newProperty;
	ILType *signature;

	/* Convert the property's signature */
	signature = _ILLinkerConvertType(linker, ILProperty_Signature(property));
	if(!signature)
	{
		return 0;
	}

	/* Create the new property record */
	newProperty = ILPropertyCreate(newClass, 0, ILProperty_Name(property),
								   ILProperty_Attrs(property), signature);
	if(!newProperty)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}

	/* Convert the constant attached to the property */
	if(!_ILLinkerConvertConstant(linker, (ILProgramItem *)property,
								 (ILProgramItem *)newProperty))
	{
		return 0;
	}

	/* Convert the method semantics on the property */
	if(!ConvertSemantics(linker, (ILProgramItem *)property,
						 (ILProgramItem *)newProperty))
	{
		return 0;
	}

	/* Convert the attributes that are attached to the property */
	if(!_ILLinkerConvertAttrs(linker, (ILProgramItem *)property,
						      (ILProgramItem *)newProperty))
	{
		return 0;
	}

	/* Convert the debug information that is attached to the property */
	if(!_ILLinkerConvertDebug(linker, (ILProgramItem *)property,
						      (ILProgramItem *)newProperty))
	{
		return 0;
	}

	/* Done */
	return 1;
}

int _ILLinkerConvertEvent(ILLinker *linker, ILEvent *event,
						  ILClass *newClass)
{
	ILType *type;
	ILClass *classInfo;
	ILEvent *newEvent;

	/* Convert the event's class type */
	type = ILEvent_Type(event);
	if(type != ILType_Invalid)
	{
		classInfo = _ILLinkerConvertClassRef(linker, ILType_ToClass(type));
		if(!classInfo)
		{
			return 0;
		}
	}
	else
	{
		classInfo = 0;
	}

	/* Create the new event record */
	newEvent = ILEventCreate(newClass, 0, ILEvent_Name(event),
							 ILEvent_Attrs(event), classInfo);
	if(!newEvent)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}

	/* Convert the method semantics on the event */
	if(!ConvertSemantics(linker, (ILProgramItem *)event,
						 (ILProgramItem *)newEvent))
	{
		return 0;
	}

	/* Convert the attributes that are attached to the event */
	if(!_ILLinkerConvertAttrs(linker, (ILProgramItem *)event,
						      (ILProgramItem *)newEvent))
	{
		return 0;
	}

	/* Convert the debug information that is attached to the event */
	if(!_ILLinkerConvertDebug(linker, (ILProgramItem *)event,
						      (ILProgramItem *)newEvent))
	{
		return 0;
	}

	/* Done */
	return 1;
}

#ifdef	__cplusplus
};
#endif
