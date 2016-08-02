/*
 * custom.c - Handle custom marshaling operations.
 *
 * Copyright (C) 2003, 2011  Southern Storm Software, Pty Ltd.
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
 * Get a custom marshalling class.  This isn't terribly efficient right now.
 */
static ILClass *GetCustomMarshaller(ILExecThread *thread, ILClass **customRet,
									const char *customName, int customNameLen)
{
	ILString *str;
	ILObject *type;
	ILClass *classInfo;
	ILClass *custom;

	/* Bail out if no name */
	if(!customName || !customNameLen)
	{
		return 0;
	}

	/* Convert the name into a string */
	str = ILStringCreateLen(thread, customName, customNameLen);
	if(!str)
	{
		return 0;
	}

	/* Look up the marshalling type */
	type = _IL_Type_GetType(thread, str, 0, 0);
	if(!type)
	{
		return 0;
	}

	/* Convert the type object into an ILClass structure */
	classInfo = _ILGetClrClass(thread, type);
	if(!classInfo)
	{
		return 0;
	}

	/* Check to make sure that the class implements "ICustomMarshaler" */
	custom = ILExecThreadLookupClass
		(thread, "System.Runtime.InteropServices.ICustomMarshaler");
	if(!custom)
	{
		return 0;
	}
	if(!ILClassImplements(classInfo, custom))
	{
		return 0;
	}
	*customRet = custom;
	return classInfo;
}

/*
 * Get a custom marshalling instance.
 */
static ILObject *GetMarshallingInstance(ILExecThread *thread,
										ILClass *classInfo,
										const char *customCookie,
										int customCookieLen)
{
	ILMethod *method;
	ILString *str;
	ILObject *result;

	/* Find the "GetInstance" method */
	method = ILExecThreadLookupMethodInClass
		(thread, classInfo, "GetInstance",
		 "(oSystem.String;)oSystem.Runtime.InteropServices.ICustomMarshaler;");
	if(!method)
	{
		return 0;
	}

	/* Convert the cookie into a string */
	if(customCookie && customCookieLen)
	{
		str = ILStringCreateLen(thread, customCookie, customCookieLen);
		if(!str)
		{
			return 0;
		}
	}
	else
	{
		str = 0;
	}

	/* Call the "GetInstance" method */
	result = 0;
	if(ILExecThreadCall(thread, method, &result, str))
	{
		return 0;
	}
	else
	{
		return result;
	}
}

/*
 * Throw a custom marshalling error.
 */
static void ThrowCustomError(ILExecThread *thread)
{
	if(!_ILExecThreadHasException(thread))
	{
		ILExecThreadThrowSystem(thread, "System.InvalidOperationException", 0);
	}
}

void *_ILObjectToCustom(ILExecThread *thread, ILObject *obj,
						const char *customName, int customNameLen,
						const char *customCookie, int customCookieLen)
{
	ILClass *classInfo;
	ILClass *custom;
	ILObject *marshal;
	ILMethod *method;
	void *result;

	/* Bail out immediately if the object is NULL */
	if(!obj)
	{
		return 0;
	}

	/* Find the custom marshalling instance */
	classInfo = GetCustomMarshaller
		(thread, &custom, customName, customNameLen);
	if(!classInfo)
	{
		ThrowCustomError(thread);
		return 0;
	}
	marshal = GetMarshallingInstance
		(thread, classInfo, customCookie, customCookieLen);
	if(!marshal)
	{
		ThrowCustomError(thread);
		return 0;
	}

	/* Call the marshalling method and return */
	method = ILExecThreadLookupMethodInClass
		(thread, custom, "MarshalManagedToNative", "(ToSystem.Object;)j");
	if(!method)
	{
		ThrowCustomError(thread);
		return 0;
	}
	result = 0;
	if(ILExecThreadCallVirtual(thread, method, &result, marshal, obj))
	{
		result = 0;
	}
	return result;
}

ILObject *_ILCustomToObject(ILExecThread *thread, void *ptr,
							const char *customName, int customNameLen,
							const char *customCookie, int customCookieLen)
{
	ILClass *classInfo;
	ILClass *custom;
	ILObject *marshal;
	ILMethod *method;
	void *result;

	/* Bail out immediately if the pointer is NULL */
	if(!ptr)
	{
		return 0;
	}

	/* Find the custom marshalling instance */
	classInfo = GetCustomMarshaller
		(thread, &custom, customName, customNameLen);
	if(!classInfo)
	{
		ThrowCustomError(thread);
		return 0;
	}
	marshal = GetMarshallingInstance
		(thread, classInfo, customCookie, customCookieLen);
	if(!marshal)
	{
		ThrowCustomError(thread);
		return 0;
	}

	/* Call the marshalling method and return */
	method = ILExecThreadLookupMethodInClass
		(thread, custom, "MarshalNativeToManaged", "(Tj)oSystem.Object;");
	if(!method)
	{
		ThrowCustomError(thread);
		return 0;
	}
	result = 0;
	if(ILExecThreadCallVirtual(thread, method, &result, marshal, ptr))
	{
		result = 0;
	}
	return result;
}

#ifdef	__cplusplus
};
#endif
