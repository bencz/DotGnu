/*
 * lib_type.c - Internalcall methods for "Type" and related classes.
 *
 * Copyright (C) 2001, 2002, 2008, 2011  Southern Storm Software, Pty Ltd.
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

#include "engine_private.h"
#include "lib_defs.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILImage *_ILClrCallerImage(ILExecThread *thread)
{
	ILImage *systemImage;
	ILImage *image;
	ILMethod *method;
	unsigned long num;

	/* We scan up the stack looking for a method that is
	   not within the system library image.  That method
	   is designated as the caller */
	systemImage = ILContextGetSystem(thread->process->context);
	if(!systemImage)
	{
		systemImage = ILProgramItem_Image(ILExecThreadStackMethod(thread, 0));
	}
	num = 0;
	while((method = ILExecThreadStackMethod(thread, num)) != 0)
	{
		if((image = ILProgramItem_Image(method)) != systemImage)
		{
			return image;
		}
		++num;
	}

	/* All methods are in the system image, so let that be the caller */
	return systemImage;
}

int _ILClrCheckAccess(ILExecThread *thread, ILClass *classInfo,
					  ILMember *member)
{
	ILImage *systemImage;
	ILMethod *method;
	ILClass *methodClass;
	unsigned long num;

	/* We scan up the stack looking for a method that is
	   not within the system library image.  That method
	   is used to perform the security check */
	systemImage = ILContextGetSystem(thread->process->context);
	if(!systemImage)
	{
		systemImage = ILProgramItem_Image(ILExecThreadStackMethod(thread, 0));
	}
	num = 0;
	while((method = ILExecThreadStackMethod(thread, num)) != 0)
	{
		if(ILProgramItem_Image(method) != systemImage)
		{
			break;
		}
		++num;
	}
	if(!method)
	{
		/* All methods are within the system image, so all accesses are OK.
		   We assume that the system library always has ReflectionPermission */
		return 1;
	}

	/* Check for direct accessibility */
	methodClass = ILMethod_Owner(method);
	if(classInfo != 0 && ILClassAccessible(classInfo, methodClass))
	{
		return 1;
	}
	else if(member != 0 && ILMemberAccessible(member, methodClass))
	{
		return 1;
	}

	/* Check that the caller has "ReflectionPermission" */
	/* TODO */
	return 1;
}

int _ILClrCheckItemAccess(ILExecThread *thread, ILProgramItem *item)
{
	ILClass *classInfo;
	ILMember *member;

	/* Is the item a class? */
	classInfo = ILProgramItemToClass(item);
	if(classInfo)
	{
		return _ILClrCheckAccess(thread, classInfo, (ILMember *)0);
	}

	/* Is the item a class member? */
	member = ILProgramItemToMember(item);
	if(member)
	{
		return _ILClrCheckAccess(thread, (ILClass *)0, member);
	}

	/* We assume that it is OK to access other types of items.
	   Usually these are assemblies and modules, which are public */
	return 1;
}

/*
 * Throw a "TypeLoadException" when a type lookup has failed.
 */
static void ThrowTypeLoad(ILExecThread *thread, ILString *name)
{
	ILExecThreadThrowSystem(thread, "System.TypeLoadException",
							(const char *)0);
}

ILObject *_ILGetClrTypeForILType(ILExecThread *thread, ILType *type)
{
	ILClass *classInfo;

	/* Strip custom modifier prefixes from the type */
	type = ILTypeStripPrefixes(type);

	/* Convert the type into an "ILClass" structure */
	classInfo = ILClassFromType(ILProgramItem_Image(thread->method),
								0, type, 0);
	classInfo = ILClassResolve(classInfo);

	/* Get the "ClrType" object for the "ILClass" structure */
	if(classInfo)
	{
		return _ILGetClrType(thread, classInfo);
	}
	else
	{
		return 0;
	}
}

/*
 * Hash table entry information for a reflection mapping.
 */
#ifdef IL_CONFIG_REDUCE_DATA
#define	IL_REFLECTION_HASH_SIZE		8
#else
#define	IL_REFLECTION_HASH_SIZE		512
#endif
typedef struct _tagILClrHash ILClrHash;
struct _tagILClrHash
{
	ILObject   *object;
	ILClrHash  *next;

};

ILObject *_ILClrToObject(ILExecThread *thread, void *item, const char *name)
{
	ILUInt32 hash;
	ILClrHash *entry;
	ILClrHash *newEntry;
	ILClass *classInfo;
	ILObject *object;

	/* Create the reflection hash table */
	if(!(thread->process->reflectionHash))
	{
		thread->process->reflectionHash =
			ILGCAllocPersistent(sizeof(ILClrHash) * IL_REFLECTION_HASH_SIZE);
		if(!(thread->process->reflectionHash))
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
	}

	/* Compute the hash value for the item */
	hash = ILProgramItem_Token((ILProgramItem *)item);
	hash = ((hash + (hash >> 20)) & (IL_REFLECTION_HASH_SIZE - 1));

	/* Look for an object that is already mapped to the item */
	entry = &(((ILClrHash *)(thread->process->reflectionHash))[hash]);
	while(entry != 0)
	{
		if(entry->object != 0 &&
		   ((System_Reflection *)(entry->object))->privateData == item)
		{
			return entry->object;
		}
		entry = entry->next;
	}

	/* Construct a new object of the class "name" to hold the item */
	classInfo = ILExecThreadLookupClass(thread, name);
	if(!classInfo)
	{
		return 0;
	}
	object = _ILEngineAllocObject(thread, classInfo);
	if(!object)
	{
		return 0;
	}
	((System_Reflection *)object)->privateData = item;

	/* Add the object to the reflection hash */
	entry = &(((ILClrHash *)(thread->process->reflectionHash))[hash]);
	if(entry->object == 0)
	{
		/* The main table slot is free, so put the object there */
		entry->object = object;
	}
	else
	{
		/* We need an overflow entry for the item */
		newEntry = (ILClrHash *)ILGCAlloc(sizeof(ILClrHash));
		if(!newEntry)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		newEntry->object = object;
		newEntry->next = entry->next;
		entry->next = newEntry;
	}

	/* Return the reflected object to the caller */
	return object;
}

void *_ILClrFromObject(ILExecThread *thread, ILObject *object)
{
	if(object)
	{
		return ((System_Reflection *)object)->privateData;
	}
	else
	{
		return 0;
	}
}

/*
 * Resolve an assembly name to an image.  Returns NULL if the
 * assembly could not be located.
 */
static ILImage *ResolveAssembly(ILExecThread *thread,
								ILUInt16 *name, ILInt32 len)
{
	ILInt32 posn;
	ILString *str;
	char *utf8;
	ILImage *image ;
	ILInt32 error;

	/* Extract the assembly name, ignoring version and key information
	   which are unlikely to ever be the same between different CLI's */
	while(len > 0 && (name[0] == ' ' || name[0] == '\t'))
	{
		++name;
		--len;
	}
	posn = 0;
	while(posn < len && name[posn] != ' ' && name[posn] != '\t' &&
	      name[posn] != ',')
	{
		++posn;
	}
	if(!posn)
	{
		return 0;
	}

	/* Convert the name into UTF-8 */
	str = ILStringWCreateLen(thread, name, (int)posn);
	utf8 = ILStringToUTF8(thread, str);
	if(!utf8)
	{
		return 0;
	}

	/* Look for an image with a matching assembly name */
	image = ILContextGetAssembly(thread->process->context, utf8);

	if(image != NULL)
	{
		return image;
	}

	error = ILImageLoadAssembly(utf8, thread->process->context, 
									_ILClrCallerImage(thread),
									&image);
	if(error == 0)
	{
		return image;
	}

	return NULL;
}

ILObject *_ILGetTypeFromImage(ILExecThread *thread,
							  ILImage *assemblyImage,
							  ILString *name,
							  ILBool throwOnError,
							  ILBool ignoreCase)
{
	ILInt32 length;
	ILUInt16 *buf;
	ILInt32 posn;
	ILInt32 dot;
	ILInt32 nameStart;
	ILInt32 nameEnd;
	ILInt32 nameSpecial;
	int brackets;
	int rank;
	ILProgramItem *scope;
	ILClass *classInfo;
	ILType *typeInfo;

	/* Validate the parameters */
	if(!name)
	{
		ILExecThreadThrowArgNull(thread, "name");
		return 0;
	}

	/* Extract the buffer portion of the name string */
	length = _ILStringToBuffer(thread, name, &buf);

	/* Turn off "ignoreCase" if the length is greater than 128.
	   Provided for consistency with ECMA, even though we can
	   check for case-insensitive type names of any length */
	if(length > 128)
	{
		ignoreCase = 0;
	}

	/* Check for an assembly suffix, to determine where to
	   start looking for the type */
	posn = 0;
	nameStart = 0;
	nameSpecial = -1;
	brackets = 0;
	while(posn < length && (buf[posn] != (ILUInt16)',' || brackets))
	{
		if(buf[posn] == '\\')
		{
			++posn;
			if(posn >= length)
			{
				/* Invalid escape, so this cannot be a valid type name */
				if(throwOnError)
				{
					ThrowTypeLoad(thread, name);
				}
				return 0;
			}
		}
		else if(buf[posn] == '[')
		{
			if(nameSpecial == -1)
			{
				nameSpecial = posn;
			}
			++brackets;
		}
		else if(buf[posn] == ']')
		{
			if(brackets > 0)
			{
				--brackets;
			}
		}
		else if(buf[posn] == '*' || buf[posn] == '&')
		{
			if(nameSpecial == -1)
			{
				nameSpecial = posn;
			}
		}
		++posn;
	}
	if(nameSpecial == -1)
	{
		nameSpecial = posn;
	}
	nameEnd = posn;
	if(posn < length)
	{
		++posn;
		if(assemblyImage)
		{
			/* We are trying to look in a specific image,
			   so it is an error for the caller to supply
			   an explicit assembly name */
			if(throwOnError)
			{
				ThrowTypeLoad(thread, name);
			}
			return 0;
		}
		else
		{
			assemblyImage = ResolveAssembly(thread, buf + posn, length - posn);
			if(!assemblyImage)
			{
				if(throwOnError)
				{
					ThrowTypeLoad(thread, name);
				}
				return 0;
			}
		}
	}
	if(nameStart >= nameSpecial)
	{
		/* Empty type name means no type */
		if(throwOnError)
		{
			ThrowTypeLoad(thread, name);
		}
		return 0;
	}

	/* Find the initial lookup scope */
	if(assemblyImage)
	{
		scope = ILClassGlobalScope(assemblyImage);
	}
	else
	{
		scope = 0;
	}

	/* Locate the type */
	classInfo = 0;
	posn = nameStart;
	while(posn < nameSpecial)
	{
		/* Find the next '+', which is used to separate nested types.
		   Also find the last dot that indicates the namespace */
		dot = -1;
		while(posn < nameSpecial && buf[posn] != '+')
		{
			if(buf[posn] == '\\')
			{
				++posn;
			}
			else if(buf[posn] == '.')
			{
				dot = posn;
			}
			++posn;
		}
		if(classInfo != 0)
		{
			/* Dots in nested type names are not namespace delimiters */
			dot = -1;
		}

		/* Look for the class within the current scope */
		if(scope != 0)
		{
			if(dot != -1)
			{
				classInfo = ILClassLookupUnicode
					(scope, buf + dot + 1, posn - (dot + 1),
					 buf + nameStart, dot - nameStart, ignoreCase);
			}
			else
			{
				classInfo = ILClassLookupUnicode
					(scope, buf + nameStart, posn - nameStart,
					 0, 0, ignoreCase);
			}
		}
		else
		{
			/* Look in the same image as the caller first */
			scope = ILClassGlobalScope(_ILClrCallerImage(thread));
			if(dot != -1)
			{
				classInfo = ILClassLookupUnicode
					(scope, buf + dot + 1, posn - (dot + 1),
					 buf + nameStart, dot - nameStart, ignoreCase);
			}
			else
			{
				classInfo = ILClassLookupUnicode
					(scope, buf + nameStart, posn - nameStart,
					 0, 0, ignoreCase);
			}

			/* Look in the global scope if not in the caller's image */
			if(!classInfo)
			{
				if(dot != -1)
				{
					classInfo = ILClassLookupGlobalUnicode
						(thread->process->context,
						 buf + dot + 1, posn - (dot + 1),
						 buf + nameStart, dot - nameStart, ignoreCase);
				}
				else
				{
					classInfo = ILClassLookupGlobalUnicode
						(thread->process->context,
						 buf + nameStart, posn - nameStart,
						 0, 0, ignoreCase);
				}
			}
		}
		if(!classInfo)
		{
			/* Could not find the class */
			if(throwOnError)
			{
				ThrowTypeLoad(thread, name);
			}
			return 0;
		}
		scope = (ILProgramItem *)classInfo;

		/* Advance to the next nested type name */
		if(posn < nameSpecial)
		{
			++posn;
			nameStart = posn;
			if(posn >= nameSpecial)
			{
				/* Empty nested type name */
				if(throwOnError)
				{
					ThrowTypeLoad(thread, name);
				}
				return 0;
			}
		}
	}

	/* Check that we have permission to reflect the type */
	if(!_ILClrCheckAccess(thread, classInfo, 0))
	{
		return 0;
	}

	typeInfo = ILClassToType(classInfo);

	/* Resolve special suffixes for array and pointer designations */
	if(nameSpecial < nameEnd)
	{
		posn=nameSpecial;
		brackets=0;
		rank=0;
		while(posn < nameEnd)
		{
			switch(buf[posn])
			{
				case '[':
				{
					rank=1;
					brackets++;
				}
				break;
				
				case ']':
				{
					if(brackets==0)
					{
						if(throwOnError)
						{
							ThrowTypeLoad(thread, name);
						}
						return 0;
					}
					typeInfo = ILTypeFindOrCreateArray(
									thread->process->context,
									(unsigned long) rank,
									typeInfo);
					rank=0;
					brackets--;
				}
				break;

				case ',':
				{
					rank++;
				}
				break;

				case '*':
				{
					typeInfo = ILTypeCreateRef
						(thread->process->context,
						 IL_TYPE_COMPLEX_PTR, typeInfo);
				}
				break;

				case '&':
				{
					typeInfo = ILTypeCreateRef
						(thread->process->context,
						 IL_TYPE_COMPLEX_BYREF, typeInfo);
				}
				break;

				default:
				{
					if(throwOnError)
					{
						ThrowTypeLoad(thread, name);
					}
					return 0;
				}
				break;
			}
			posn++;
		}
	}

	/* Convert the class information block into a "ClrType" instance */
	return _ILGetClrTypeForILType(thread, typeInfo);
}

void _ILClrNotImplemented(ILExecThread *thread)
{
	/* Avoid re-entering the C# class library to create the exception */
	_ILExecThreadSetException
		(thread, _ILSystemException(thread, "System.NotImplementedException"));
}

/*
 * private int GetClrArrayRank();
 */
ILInt32 _IL_ClrType_GetClrArrayRank(ILExecThread *thread, ILObject *_this)
{
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	ILType *synType = (classInfo ? ILClassGetSynType(classInfo) : 0);
	if(synType != 0)
	{
		return (ILInt32)(ILTypeGetRank(synType));
	}
	else
	{
		return 0;
	}
}

/*
 * protected override TypeAttributes GetAttributeFlagsImpl();
 */
ILInt32 _IL_ClrType_GetAttributeFlagsImpl(ILExecThread *thread,
										  ILObject *_this)
{
#ifdef IL_CONFIG_REFLECTION
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	if(classInfo)
	{
		return (ILInt32)(ILClassGetAttrs(classInfo));
	}
	else
	{
		return 0;
	}
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

/*
 * public override Type GetElementType();
 */
ILObject *_IL_ClrType_GetElementType(ILExecThread *thread, ILObject *_this)
{
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	ILType *synType;
	if(classInfo)
	{
		/* Only interesting for array, pointer, and byref types */
		synType = ILClassGetSynType(classInfo);
		if(synType != 0 && ILType_IsComplex(synType))
		{
			switch(ILType_Kind(synType))
			{
				case IL_TYPE_COMPLEX_ARRAY:
				case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
				{
					return _ILGetClrTypeForILType
							(thread, ILTypeGetElemType(synType));
				}
				/* Not reached */

				case IL_TYPE_COMPLEX_BYREF:
				case IL_TYPE_COMPLEX_PTR:
				{
					return _ILGetClrTypeForILType
							(thread, ILType_Ref(synType));
				}
				/* Not reached */
			}
		}
	}
	return 0;
}

#ifdef IL_CONFIG_REFLECTION

/*
 * Determine if we have an interface name match for "GetInterface".
 */
static int InterfaceNameMatch(ILExecThread *thread, ILClass *interface,
                              ILString *name, ILBool ignoreCase)
{
	const char *nameUtf8;

	if (!name) { return 0; }

	if (!(nameUtf8 = ILStringToUTF8(thread, name)))
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}

	if (!ignoreCase)
	{
		return (!strcmp(interface->className->name, nameUtf8));
	}
	else
	{
		return (!ILStrICmp(interface->className->name, nameUtf8));
	}
}

/*
 * Scan the implemented interfaces of a class for a particular name.
 */
static ILClass *GetInterface(ILExecThread *thread, ILClass *classInfo,
							 ILString *name, ILBool ignoreCase)
{
	ILImplements *impl = 0;
	ILClass *interface;
	while((impl = ILClassNextImplements(classInfo, impl)) != 0)
	{
		interface = ILImplements_UnderlyingInterfaceClass(impl);
		if(InterfaceNameMatch(thread, interface, name, ignoreCase))
		{
			return ILImplements_InterfaceClass(impl);
		}
		interface = GetInterface(thread, interface, name, ignoreCase);
		if(interface)
		{
			return interface;
		}
	}
	return 0;
}

#endif /* IL_CONFIG_REFLECTION */

/*
 * public override Type GetInterface(String name, bool ignoreCase);
 */
ILObject *_IL_ClrType_GetInterface(ILExecThread *thread,
								   ILObject *_this, ILString *name,
								   ILBool ignoreCase)
{
#ifdef IL_CONFIG_REFLECTION
	ILClass *classInfo;
	ILClass *interfaceClassInfo;

	/* Bail out if "name" is NULL or the class is invalid in some way */
	if(!name)
	{
		ILExecThreadThrowArgNull(thread, "name");
		return 0;
	}
	classInfo = _ILGetClrClass(thread, _this);
	
	/* Scan all implemented interfaces for the name */
	while(classInfo != 0)
	{
		interfaceClassInfo = GetInterface(thread, classInfo, name, ignoreCase);
		if(interfaceClassInfo)
		{
			return _ILGetClrType(thread, interfaceClassInfo);
		}
		/* Move up to the parent of this class */
		classInfo = ILClass_ParentClass(classInfo);
	}
	return 0;
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

#ifdef IL_CONFIG_REFLECTION

/*
 * Get the maximum number of interfaces (including duplicates)
 * that are implemented by a specific class.
 */
static ILInt32 GetMaxInterfaces(ILClass *classInfo)
{
	ILImplements *impl = 0;
	ILInt32 count = 0;
	
	if(classInfo == NULL)
	{
		return 0;
	}
	
	while((impl = ILClassNextImplements(classInfo, impl)) != 0)
	{
		count += 1 + GetMaxInterfaces(ILImplements_UnderlyingInterfaceClass(impl));
	}

	// Traverse up into the parent class as well
	count += GetMaxInterfaces(ILClass_UnderlyingParentClass(classInfo));
	return count;
}

/*
 * Fill an array with interfaces, excluding duplicates.
 * Returns the new position, or -1 if an exception occurred.
 */
static ILInt32 FillWithInterfaces(ILExecThread *thread, ILClass *classInfo,
								  ILObject **array, ILInt32 posn)
{
	ILImplements *impl = 0;
	ILClass *interface;
	ILObject *clrType;
	ILInt32 index;
	
	if(classInfo == NULL)
	{
		return posn;
	}
	
	while((impl = ILClassNextImplements(classInfo, impl)) != 0)
	{
		interface = ILImplements_InterfaceClass(impl);
		clrType = _ILGetClrType(thread, interface);
		for(index = 0; index < posn; ++index)
		{
			if(array[index] == clrType)
			{
				break;
			}
		}
		if(index >= posn)
		{
			array[posn++] = clrType;
			posn = FillWithInterfaces(thread, interface, array, posn);
		}
	}

	// Traverse up into the parent class as well
	posn = FillWithInterfaces(thread, ILClass_ParentClass(classInfo), array, posn);
	return posn;
}

#endif /* IL_CONFIG_REFLECTION */

/*
 * public override Type[] GetInterfaces();
 */
System_Array *_IL_ClrType_GetInterfaces(ILExecThread *thread,
										ILObject *_this)
{
#ifdef IL_CONFIG_REFLECTION
	ILClass *classInfo;
	System_Array *array;
	ILInt32 count;

	/* Bail out if the class is invalid in some way */
	classInfo = _ILGetClrClass(thread, _this);
	if(!classInfo)
	{
		return 0;
	}

	/* Allocate an array big enough to hold all interfaces */
	count = GetMaxInterfaces(classInfo);
	array = (System_Array *)ILExecThreadNew(thread, "[oSystem.Type;",
										    "(Ti)V", (ILVaInt)count);
	if(!array)
	{
		return 0;
	}

	/* Fill the array, and check for duplicates */
	count = FillWithInterfaces(thread, classInfo,
							   (ILObject **)(ArrayToBuffer(array)), 0);
	if(count < 0)
	{
		/* An exception occurred */
		return 0;
	}

	/* Shorten the array to its final length and return */
	ArrayLength(array) = count;
	return array;
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

/*
 * Type categories: this must be kept in sync with the
 * definition of the "System.Reflection.ClrTypeCategory"
 * enumeration in "pnetlib".
 */
#define	ClrTypeCategory_Primitive		0
#define	ClrTypeCategory_Class			1
#define	ClrTypeCategory_ValueType		2
#define	ClrTypeCategory_Enum			3
#define	ClrTypeCategory_Array			4
#define	ClrTypeCategory_ByRef			5
#define	ClrTypeCategory_Pointer			6
#define	ClrTypeCategory_Method			7
#define	ClrTypeCategory_COMObject		8
#define	ClrTypeCategory_Other			9

/*
 * private ClrTypeCategory GetClrTypeCategory();
 */
ILInt32 _IL_ClrType_GetClrTypeCategory(ILExecThread *thread, ILObject *_this)
{
#ifdef IL_CONFIG_REFLECTION
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	ILType *synType;
	if(classInfo)
	{
		synType = ILClassGetSynType(classInfo);
		if(synType != 0 && ILType_IsComplex(synType))
		{
			switch(ILType_Kind(synType))
			{
				case IL_TYPE_COMPLEX_ARRAY:
				case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
						 return ClrTypeCategory_Array;

				case IL_TYPE_COMPLEX_BYREF:
						 return ClrTypeCategory_ByRef;

				case IL_TYPE_COMPLEX_PTR:
						 return ClrTypeCategory_Pointer;

				case IL_TYPE_COMPLEX_METHOD:
				case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
						 return ClrTypeCategory_Method;

				default: return ClrTypeCategory_Other;
			}
		}
		if(ILClassIsValueType(classInfo))
		{
			ILType *type = ILClassToType(classInfo);
			if(ILType_IsPrimitive(type))
			{
				switch(ILType_ToElement(type))
				{
					case IL_META_ELEMTYPE_BOOLEAN:
					case IL_META_ELEMTYPE_I1:
					case IL_META_ELEMTYPE_U1:
					case IL_META_ELEMTYPE_I2:
					case IL_META_ELEMTYPE_U2:
					case IL_META_ELEMTYPE_CHAR:
					case IL_META_ELEMTYPE_I4:
					case IL_META_ELEMTYPE_U4:
					case IL_META_ELEMTYPE_I8:
					case IL_META_ELEMTYPE_U8:
					case IL_META_ELEMTYPE_R4:
					case IL_META_ELEMTYPE_R8:
						return ClrTypeCategory_Primitive;
				}
			}
			else if(ILTypeGetEnumType(type) != type)
			{
				return ClrTypeCategory_Enum;
			}
			return ClrTypeCategory_ValueType;
		}
	}
	return ClrTypeCategory_Class;
#else
	_ILClrNotImplemented(thread);
	return ClrTypeCategory_Class;
#endif 
}

/*
 * public override bool IsSubclassOf(Type c);
 */
ILBool _IL_ClrType_IsSubclassOf(ILExecThread *thread,
							    ILObject *_this,
							    ILObject *otherType)
{
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	ILClass *otherClassInfo;
	if(otherType)
	{
		otherClassInfo = _ILGetClrClass(thread, otherType);
		if(classInfo && otherClassInfo && classInfo != otherClassInfo)
		{
			return ILClassInheritsFrom(classInfo, otherClassInfo);
		}
		else
		{
			return 0;
		}
	}
	else
	{
		ILExecThreadThrowArgNull(thread, "c");
		return 0;
	}
}

/*
 * private bool IsClrNestedType();
 */
ILBool _IL_ClrType_IsClrNestedType(ILExecThread *thread, ILObject *_this)
{
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	if(classInfo && ILClass_NestedParent(classInfo) != 0)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * private Type GetClrBaseType();
 */
ILObject *_IL_ClrType_GetClrBaseType(ILExecThread *thread, ILObject *_this)
{
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	if(classInfo)
	{
		ILClass *parent = ILClass_ParentClass(classInfo);
		if(parent)
		{
			return _ILGetClrType(thread, parent);
		}
		else
		{
			return 0;
		}
	}
	else
	{
		return 0;
	}
}

/*
 * private Assembly GetClrAssembly();
 */
ILObject *_IL_ClrType_GetClrAssembly(ILExecThread *thread, ILObject *_this)
{
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	ILImage *image = (classInfo ? ILClassToImage(classInfo) : 0);
	void *item;
	if(image)
	{
		item = ILImageTokenInfo(image, (IL_META_TOKEN_ASSEMBLY | 1));
		if(item)
		{
			return _ILClrToObject(thread, item, "System.Reflection.Assembly");
		}
		/* TODO: if the image does not have an assembly manifest,
		   then look for the parent assembly */
	}
	return 0;
}

/*
 * private Module GetClrModule();
 */
ILObject *_IL_ClrType_GetClrModule(ILExecThread *thread, ILObject *_this)
{
#ifdef IL_CONFIG_REFLECTION
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	ILImage *image = (classInfo ? ILClassToImage(classInfo) : 0);
	void *item;
	if(image)
	{
		item = ILImageTokenInfo(image, (IL_META_TOKEN_MODULE | 1));
		if(item)
		{
			return _ILClrToObject(thread, item, "System.Reflection.Module");
		}
	}
	return 0;
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

/*
 * Output a string to a Unicode name buffer, or compute the length
 * of the buffer.  Returns the length.
 */
static ILInt32 NameOutputString(ILUInt16 *buf, const char *str, int quoteDot)
{
	int slen, sposn;
	ILInt32 len = 0;
	unsigned long ch;
	slen = strlen(str);
	sposn = 0;
	while(sposn < slen)
	{
		ch = ILUTF8ReadChar(str, slen, &sposn);
		if(ch == '\\' || (ch == '.' && quoteDot) || ch == '+' ||
		   ch == ',' || ch == '[' || ch == ']' || ch == '&' || ch == '*')
		{
			/* We need to quote this character */
			if(buf)
			{
				buf[0] = (ILUInt16)'\\';
				buf[1] = (ILUInt16)ch;
				buf += 2;
			}
			len += 2;
		}
		else if(ch < (unsigned long)0x10000)
		{
			/* Ordinary character */
			if(buf)
			{
				*buf++ = (ILUInt16)ch;
			}
			++len;
		}
		else if(ch < ((unsigned long)0x110000))
		{
			/* Surrogate-based character */
			if(buf)
			{
				ILUTF16WriteChar(buf, ch);
				buf += 2;
			}
			len += 2;
		}
	}
	return len;
}

/*
 * Output a class name to a Unicode name buffer, or compute the length.
 * Returns the computed length.
 */
static ILInt32 NameOutputClassName(ILUInt16 *buf, ILClass *classInfo,
								   int fullyQualified)
{
	ILClass *nestedParent;
	ILClass *info;
	ILInt32 len;
	const char *namespace;
	if(ILClass_IsGenericInstance(classInfo))
	{
		info = classInfo;
		classInfo = ILClassGetGenericDef(classInfo);
	}
	else
	{
		info = 0;
	}
	if(fullyQualified)
	{
		nestedParent = ILClass_NestedParent(classInfo);
		if(nestedParent != 0)
		{
			len = NameOutputClassName(buf, ILClassResolve(nestedParent),
									  fullyQualified);
			if(buf != 0)
			{
				buf[len++] = (ILUInt16)'+';
			}
			else
			{
				++len;
			}
		}
		else
		{
			namespace = ILClass_Namespace(classInfo);
			if(namespace)
			{
				len = NameOutputString(buf, namespace, 0);
				if(buf != 0)
				{
					buf[len++] = (ILUInt16)'.';
				}
				else
				{
					++len;
				}
			}
			else
			{
				len = 0;
			}
		}
	}
	else
	{
		len = 0;
	}
	if(buf != 0)
	{
		len += NameOutputString(buf + len, ILClass_Name(classInfo), 1);
	}
	else
	{
		len += NameOutputString(0, ILClass_Name(classInfo), 1);
	}
	if(info)
	{
		int numParams, posn;
		ILType *type;
		ILType *subType;

		type = ILClassGetTypeArguments(info);
		if(buf != 0)
		{
			buf[len++] = (ILUInt16)'[';
		}
		else
		{
			++len;
		}
		numParams = ILTypeNumWithParams(type);
		for(posn = 1; posn <= numParams; posn++)
		{
			if(posn != 1)
			{
				if(buf != 0)
				{
					buf[len++] = (ILUInt16)',';
				}
				else
				{
					++len;
				}
			}
			subType = ILTypeGetWithParamWithPrefixes(type, posn);
			classInfo = ILClassFromType(ILClassToImage(info),
										0, subType, 0);
			if(buf != 0)
			{
				len += NameOutputClassName(buf + len, classInfo, fullyQualified);
			}
			else
			{
				len += NameOutputClassName(0, classInfo, fullyQualified);
			}
		}
		if(buf != 0)
		{
			buf[len++] = (ILUInt16)']';
		}
		else
		{
			++len;
		}		
	}

	return len;
}

/*
 * Output the suffixes for a type, or compute the buffer length.
 * Returns the computed length.
 */
static ILInt32 NameOutputTypeSuffixes(ILUInt16 *buf, ILType *type)
{
	ILInt32 len = 0;
	ILInt32 rank;
	int kind;
	if(type != 0 && ILType_IsComplex(type))
	{
		kind = ILType_Kind(type);
		if(kind == IL_TYPE_COMPLEX_ARRAY)
		{
			len += NameOutputTypeSuffixes(buf, ILType_ElemType(type));
			if(buf != 0)
			{
				buf[len++] = (ILUInt16)'[';
				buf[len++] = (ILUInt16)']';
			}
			else
			{
				len += 2;
			}
		}
		else if(kind == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
		{
			rank = ILTypeGetRank(type);
			len += NameOutputTypeSuffixes(buf, ILTypeGetElemType(type));
			if(buf != 0)
			{
				buf[len++] = (ILUInt16)'[';
			}
			else
			{
				++len;
			}
			while(rank > 1)
			{
				if(buf != 0)
				{
					buf[len++] = (ILUInt16)',';
				}
				else
				{
					++len;
				}
				--rank;
			}
			if(buf != 0)
			{
				buf[len++] = (ILUInt16)']';
			}
			else
			{
				++len;
			}
		}
		else if(kind == IL_TYPE_COMPLEX_BYREF)
		{
			len += NameOutputTypeSuffixes(buf, ILType_Ref(type));
			if(buf != 0)
			{
				buf[len++] = (ILUInt16)'&';
			}
			else
			{
				++len;
			}
		}
		else if(kind == IL_TYPE_COMPLEX_PTR)
		{
			len += NameOutputTypeSuffixes(buf, ILType_Ref(type));
			if(buf != 0)
			{
				buf[len++] = (ILUInt16)'*';
			}
			else
			{
				++len;
			}
		}
		else if(kind == IL_TYPE_COMPLEX_CMOD_REQD ||
		        kind == IL_TYPE_COMPLEX_CMOD_OPT)
		{
			len += NameOutputTypeSuffixes(buf, type->un.modifier__.type__);
		}
	}
	return len;
}

/*
 * Get the name of a type in either regular or fully-qualified form.
 */
static ILString *GetTypeName(ILExecThread *thread, ILObject *_this,
							 int fullyQualified)
{
	ILClass *classInfo;
	ILClass *elemInfo;
	ILInt32 len;
	ILString *str;
	ILUInt16 *buf;
	ILType *synType;
	ILType *inner;
	int kind;

	/* Get the ILClass structure for the runtime type */
	classInfo = _ILGetClrClass(thread, _this);
	if(!classInfo)
	{
		/* Shouldn't happen, but do something sane anyway */
		return 0;
	}

	/* Find the innermost element type if this is a complex type */
	elemInfo = classInfo;
	synType = ILClassGetSynType(classInfo);
	inner = synType;
	while(inner != 0 && ILType_IsComplex(inner))
	{
		kind = ILType_Kind(inner);
		if(kind == IL_TYPE_COMPLEX_ARRAY ||
		   kind == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
		{
			inner = ILType_ElemType(inner);
		}
		else if(kind == IL_TYPE_COMPLEX_BYREF ||
		        kind == IL_TYPE_COMPLEX_PTR)
		{
			inner = ILType_Ref(inner);
		}
		else if(kind == IL_TYPE_COMPLEX_CMOD_REQD ||
		        kind == IL_TYPE_COMPLEX_CMOD_OPT)
		{
			inner = inner->un.modifier__.type__;
		}
		else
		{
			break;
		}
	}
	if(inner != 0)
	{
		elemInfo = ILClassFromType(ILProgramItem_Image(thread->method),
								   0, inner, 0);
		if(!elemInfo)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		elemInfo = ILClassResolve(elemInfo);
	}

	/* Compute the size of the full name */
	len = NameOutputClassName(0, elemInfo, fullyQualified);
	if(classInfo != elemInfo)
	{
		len += NameOutputTypeSuffixes(0, synType);
	}

	/* Allocate a string to hold the full name */
	str = (ILString *)ILExecThreadNew(thread, "System.String", "(Tci)V",
						  			  (ILVaInt)' ', (ILVaInt)len);
	if(!str)
	{
		return 0;
	}

	/* Write the name into the string */
	if(_ILStringToBuffer(thread, str, &buf))
	{
		len = NameOutputClassName(buf, elemInfo, fullyQualified);
		if(classInfo != elemInfo)
		{
			NameOutputTypeSuffixes(buf + len, synType);
		}
	}
	return str;
}

/*
 * private String GetClrFullName();
 */
ILString *_IL_ClrType_GetClrFullName(ILExecThread *thread, ILObject *_this)
{
	return GetTypeName(thread, _this, 1);
}

/*
 * private Guid GetClrGUID();
 */
void _IL_ClrType_GetClrGUID(ILExecThread *thread,
						    void *result, ILObject *_this)
{
	/* We don't use GUID's in this system, as they are intended for
	   use with COM, which we don't have.  Besides, they are a stupid
	   way to specify globally-unique names.  URI's are much better */
	ILMemZero(result, 16);
}

/*
 * private Type GetClrNestedDeclaringType();
 */
ILObject *_IL_ClrType_GetClrNestedDeclaringType
					(ILExecThread *thread, ILObject *_this)
{
#ifdef IL_CONFIG_REFLECTION
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	ILClass *nestedParent = (classInfo ? ILClass_NestedParent(classInfo) : 0);
	if(nestedParent)
	{
		return _ILGetClrType(thread, nestedParent);
	}
	else
	{
		return 0;
	}
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

/*
 * private String GetClrName();
 */
ILString *_IL_ClrType_GetClrName(ILExecThread *thread, ILObject *_this)
{
#ifdef IL_CONFIG_REFLECTION
	return GetTypeName(thread, _this, 0);
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

/*
 * private String GetClrNamespace();
 */
ILString *_IL_ClrType_GetClrNamespace(ILExecThread *thread, ILObject *_this)
{
#ifdef IL_CONFIG_REFLECTION
	ILClass *classInfo = _ILGetClrClass(thread, _this);
	ILClass *nestedParent;
	ILType *synType;
	const char *namespace;
	int kind;
	if(classInfo)
	{
		/* Find the innermost element type if the class is synthesised */
		synType = ILClassGetSynType(classInfo);
		while(synType != 0 && ILType_IsComplex(synType))
		{
			kind = ILType_Kind(synType);
			if(kind == IL_TYPE_COMPLEX_ARRAY ||
			   kind == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
			{
				synType = ILType_ElemType(synType);
			}
			else if(kind == IL_TYPE_COMPLEX_BYREF ||
			        kind == IL_TYPE_COMPLEX_PTR)
			{
				synType = ILType_Ref(synType);
			}
			else
			{
				break;
			}
		}
		if(synType != 0)
		{
			classInfo = ILClassFromType(ILProgramItem_Image(thread->method),
									    0, synType, 0);
			if(!classInfo)
			{
				ILExecThreadThrowOutOfMemory(thread);
				return 0;
			}
			classInfo = ILClassResolve(classInfo);
		}

		/* Find the outermost type in the nesting levels, to
		   determine the namespace in which nested types reside */
		while((nestedParent = ILClass_NestedParent(classInfo)) != 0)
		{
			classInfo = ILClassResolve(nestedParent);
		}
		namespace = ILClass_Namespace(classInfo);
		if(namespace)
		{
			return ILStringCreate(thread, namespace);
		}
		else
		{
			return 0;
		}
	}
	else
	{
		return 0;
	}
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

#ifdef IL_CONFIG_REFLECTION

/*
 * Values for "System.Reflection.MemberTypes".
 */
typedef enum
{
    MT_Constructor         = 0x0001,
    MT_Event               = 0x0002,
    MT_Field               = 0x0004,
    MT_Method              = 0x0008,
    MT_Property            = 0x0010,
    MT_TypeInfo            = 0x0020,
    MT_Custom              = 0x0040,
    MT_NestedType          = 0x0080,
    MT_All                 = 0x00BF

} MemberTypes;

/*
 * Values for "System.Reflection.BindingFlags".
 */
typedef enum
{
    BF_Default              = 0x00000000,
    BF_IgnoreCase           = 0x00000001,
    BF_DeclaredOnly         = 0x00000002,
    BF_Instance             = 0x00000004,
    BF_Static               = 0x00000008,
    BF_Public               = 0x00000010,
    BF_NonPublic            = 0x00000020,
    BF_FlattenHierarchy     = 0x00000040,
    BF_InvokeMethod         = 0x00000100,
    BF_CreateInstance       = 0x00000200,
    BF_GetField             = 0x00000400,
    BF_SetField             = 0x00000800,
    BF_GetProperty          = 0x00001000,
    BF_SetProperty          = 0x00002000,
    BF_PutDispProperty      = 0x00004000,
    BF_PutRefDispProperty   = 0x00008000,
    BF_ExactBinding         = 0x00010000,
    BF_SuppressChangeType   = 0x00020000,
    BF_OptionalParamBinding = 0x00040000,
    BF_IgnoreReturn         = 0x01000000

} BindingFlags;

/*
 * Values for "System.Reflection.CallingConventions".
 */
typedef enum
{
	CC_Standard				= 0x01,
	CC_VarArgs				= 0x02,
	CC_Any					= 0x03,
	CC_Mask					= 0x1F,
	CC_HasThis				= 0x20,
	CC_ExplicitThis			= 0x40

} CallingConventions;

/*
 * Determine if a member kind matches a particular member type set.
 */
static int MemberTypeMatch(ILMember *member, ILInt32 memberTypes)
{
	switch(member->kind)
	{
		case IL_META_MEMBERKIND_METHOD:
		{
			if(ILMethod_IsConstructor((ILMethod *)member) ||
			   ILMethod_IsStaticConstructor((ILMethod *)member))
			{
				return ((memberTypes & (ILInt32)MT_Constructor) != 0);
			}
			else
			{
				return ((memberTypes & (ILInt32)MT_Method) != 0);
			}
		}
		/* Not reached */

		case IL_META_MEMBERKIND_FIELD:
			return ((memberTypes & (ILInt32)MT_Field) != 0);

		case IL_META_MEMBERKIND_PROPERTY:
			return ((memberTypes & (ILInt32)MT_Property) != 0);

		case IL_META_MEMBERKIND_EVENT:
			return ((memberTypes & (ILInt32)MT_Event) != 0);

		default: break;
	}
	return 0;
}

/*
 * Match binding attributes and calling conventions against a member.
 */
static int BindingAttrMatch(ILMember *member, ILInt32 bindingAttrs,
							ILInt32 callConv)
{
	ILMethod *method;

	/* Convert properties and events into their underlying semantic methods */
	if(member->kind == IL_META_MEMBERKIND_PROPERTY)
	{
		method = ILPropertyGetGetter((ILProperty *)member);
		if(!method)
		{
			method = ILPropertyGetSetter((ILProperty *)member);
			if(!method)
			{
				return 0;
			}
		}
		member = &(method->member);
	}
	else if(member->kind == IL_META_MEMBERKIND_EVENT)
	{
		method = ILEventGetAddOn((ILEvent *)member);
		if(!method)
		{
			method = ILEventGetRemoveOn((ILEvent *)member);
			if(!method)
			{
				method = ILEventGetFire((ILEvent *)member);
				if(!method)
				{
					return 0;
				}
			}
		}
		member = &(method->member);
	}

	/* Check the access level against the binding attributes */
	if((bindingAttrs & (ILInt32)BF_Public) != 0 &&
	   (bindingAttrs & (ILInt32)BF_NonPublic) == 0)
	{
		/* Only look for public members */
		if((member->attributes & IL_META_METHODDEF_MEMBER_ACCESS_MASK)
				!= IL_META_METHODDEF_PUBLIC)
		{
			return 0;
		}
	}
	else if((bindingAttrs & (ILInt32)BF_Public) == 0 &&
	        (bindingAttrs & (ILInt32)BF_NonPublic) != 0)
	{
		/* Only look for non-public members */
		if((member->attributes & IL_META_METHODDEF_MEMBER_ACCESS_MASK)
				== IL_META_METHODDEF_PUBLIC)
		{
			return 0;
		}
	}
	else if((bindingAttrs & (ILInt32)BF_Public) == 0 &&
	        (bindingAttrs & (ILInt32)BF_NonPublic) == 0)
	{
		/* Don't look for public or non-public members */
		return 0;
	}

	/* Check for instance and static properties */
	if((bindingAttrs & (ILInt32)BF_Static) != 0 &&
	   (bindingAttrs & (ILInt32)BF_Instance) == 0)
	{
		/* Look for static members only */
		if((member->attributes & IL_META_METHODDEF_STATIC) == 0)
		{
			return 0;
		}
	}
	else if((bindingAttrs & (ILInt32)BF_Static) == 0 &&
	        (bindingAttrs & (ILInt32)BF_Instance) != 0)
	{
		/* Look for instance members only */
		if((member->attributes & IL_META_METHODDEF_STATIC) != 0)
		{
			return 0;
		}
	}
	else if((bindingAttrs & (ILInt32)BF_Static) == 0 &&
	        (bindingAttrs & (ILInt32)BF_Instance) == 0)
	{
		/* Don't look for static or instance members */
		return 0;
	}

	/* If we have a method, then match the calling conventions also */
	if((method = ILProgramItemToMethod(&(member->programItem))) != 0)
	{
		int mconv = method->callingConventions;
		if((callConv & (ILInt32)CC_Mask) == CC_Standard)
		{
			if((mconv & IL_META_CALLCONV_MASK) != IL_META_CALLCONV_DEFAULT)
			{
				return 0;
			}
		}
		else if((callConv & (ILInt32)CC_Mask) == CC_VarArgs)
		{
			if((mconv & IL_META_CALLCONV_MASK) != IL_META_CALLCONV_VARARG)
			{
				return 0;
			}
		}
		else if((callConv & (ILInt32)CC_Mask) != CC_Any)
		{
			if((mconv & IL_META_CALLCONV_MASK) != IL_META_CALLCONV_DEFAULT &&
			   (mconv & IL_META_CALLCONV_MASK) != IL_META_CALLCONV_VARARG)
			{
				return 0;
			}
		}
		if((callConv & (ILInt32)CC_HasThis) != 0)
		{
			if((mconv & IL_META_CALLCONV_HASTHIS) == 0)
			{
				return 0;
			}
		}
		if((callConv & (ILInt32)CC_ExplicitThis) != 0)
		{
			if((mconv & IL_META_CALLCONV_EXPLICITTHIS) == 0)
			{
				return 0;
			}
		}
	}

	/* The member matches the binding attributes */
	return 1;
}

/*
 * Match binding attributes against a class.
 */
static int BindingAttrClassMatch(ILClass *classInfo, ILInt32 bindingAttrs)
{
	/* Check the access level against the binding attributes */
	if((bindingAttrs & (ILInt32)BF_Public) != 0 &&
	   (bindingAttrs & (ILInt32)BF_NonPublic) == 0)
	{
		/* Only look for public classes */
		if((classInfo->attributes & IL_META_TYPEDEF_VISIBILITY_MASK)
				!= IL_META_TYPEDEF_PUBLIC &&
		   (classInfo->attributes & IL_META_TYPEDEF_VISIBILITY_MASK)
				!= IL_META_TYPEDEF_NESTED_PUBLIC)
		{
			return 0;
		}
	}
	else if((bindingAttrs & (ILInt32)BF_Public) == 0 &&
	        (bindingAttrs & (ILInt32)BF_NonPublic) != 0)
	{
		/* Only look for non-public classes */
		if((classInfo->attributes & IL_META_TYPEDEF_VISIBILITY_MASK)
				== IL_META_TYPEDEF_PUBLIC ||
		   (classInfo->attributes & IL_META_TYPEDEF_VISIBILITY_MASK)
				== IL_META_TYPEDEF_NESTED_PUBLIC)
		{
			return 0;
		}
	}
	else if((bindingAttrs & (ILInt32)BF_Public) == 0 &&
	        (bindingAttrs & (ILInt32)BF_NonPublic) == 0)
	{
		/* Don't look for public or non-public classes */
		return 0;
	}

	/* The class matches the binding attributes */
	return 1;
}

/*
 * Convert a program item into a reflection object of the appropriate type.
 */
static ILObject *ItemToClrObject(ILExecThread *thread, ILProgramItem *item)
{
	ILMethod *method;
	ILField *field;
	ILProperty *property;
	ILEvent *event;
	ILClass *classInfo;

	/* Is this a method? */
	method = ILProgramItemToMethod(item);
	if(method)
	{
		if(ILMethod_IsConstructor(method) ||
		   ILMethod_IsStaticConstructor(method))
		{
			return _ILClrToObject
				(thread, method, "System.Reflection.ClrConstructor");
		}
		else
		{
			return _ILClrToObject
				(thread, method, "System.Reflection.ClrMethod");
		}
	}

	/* Is this a field? */
	field = ILProgramItemToField(item);
	if(field)
	{
		return _ILClrToObject(thread, field, "System.Reflection.ClrField");
	}

	/* Is this a property? */
	property = ILProgramItemToProperty(item);
	if(property)
	{
		return _ILClrToObject
			(thread, property, "System.Reflection.ClrProperty");
	}

	/* Is this an event? */
	event = ILProgramItemToEvent(item);
	if(event)
	{
		return _ILClrToObject(thread, event, "System.Reflection.ClrEvent");
	}

	/* Is this a nested type? */
	classInfo = ILProgramItemToClass(item);
	if(classInfo)
	{
		return _ILGetClrType(thread, classInfo);
	}

	/* Don't know what this is */
	return 0;
}

/*
 * Match the parameters for a method or property.
 */
static int ParameterTypeMatch(ILExecThread *thread, ILImage *image,
							  ILType *signature, System_Array *types,
							  int needExact)
{
	ILObject **items;
	ILInt32 paramNum;
	ILClass *classInfo;
	ILType *typeInfo;

	/* Check the number of parameters */
	if(ILTypeNumParams(signature) != ArrayLength(types))
	{
		return 0;
	}

	/* Scan the parameters and check for matches */
	items = (ILObject **)ArrayToBuffer(types);
	for(paramNum = 0; paramNum < ArrayLength(types); ++paramNum)
	{
		if(items[paramNum] == 0 && !needExact)
		{
			typeInfo = ILType_Null;
		}
		else
		{
			classInfo = _ILGetClrClass(thread, items[paramNum]);
			if(!classInfo)
			{
				return 0;
			}
			typeInfo = ILClassToType(classInfo);
		}
		if(needExact)
		{
			if(!ILTypeIdentical(typeInfo,
								ILTypeGetParam(signature, paramNum + 1)))
			{
				return 0;
			}
		}
		else
		{
			if(!ILTypeAssignCompatibleNonBoxing(image, typeInfo,
									   ILTypeGetParam(signature, paramNum + 1)))
			{
				return 0;
			}
		}
	}

	/* We have a parameter match */
	return 1;
}

/*
 * Get the method underlying a member, for permission and access checks.
 * Returns NULL if there is no underlying method.
 */
static ILMethod *GetUnderlyingMethod(ILMember *member)
{
	ILMethod *method;

	switch(ILMemberGetKind(member))
	{
		case IL_META_MEMBERKIND_METHOD:
		{
			return (ILMethod *)member;
		}
		/* Not reached */

		case IL_META_MEMBERKIND_PROPERTY:
		{
			method = ILProperty_Getter((ILProperty *)member);
			if(method)
			{
				return method;
			}
			method = ILProperty_Setter((ILProperty *)member);
			if(method)
			{
				return method;
			}
		}
		break;

		case IL_META_MEMBERKIND_EVENT:
		{
			method = ILEvent_AddOn((ILEvent *)member);
			if(method)
			{
				return method;
			}
			method = ILEvent_RemoveOn((ILEvent *)member);
			if(method)
			{
				return method;
			}
		}
		break;
	}

	return 0;
}

/* 
 * Check if member1 override member2 or vice versa , return overriding 
 * member , else return NULL.
 *
 * TODO: handle "new" instance methods, fields and methods
 */
static ILMember * CheckMemberOverride(ILMember *member1, ILMember *member2)
{
	ILMethod *method1, *method2;
	ILClass *class1, *class2;

	method1=GetUnderlyingMethod(member1);
	method2=GetUnderlyingMethod(member2);
	if(!method1 || !method2) return NULL;

	if(!ILTypeIdentical(ILMethod_Signature(method1) , 
			ILMethod_Signature(method2)))
	{
		return NULL;
	}
	
	class1=ILMember_Owner(member1);
	class2=ILMember_Owner(member2);
	/* Note: I'm assuming here that the object heirarchy says it all */
	if(ILClassInheritsFrom(class1,class2))
	{
		return member1;
	}
	if(ILClassInheritsFrom(class2, class1))
	{
		return member2;
	}
	return NULL;
}

#endif /* IL_CONFIG_REFLECTION */

/*
 * private MemberInfo GetMemberImpl(String name, MemberTypes memberTypes,
 *								    BindingFlags bindingAttrs,
 *								    Binder binder,
 *								    CallingConventions callConv,
 *								    Type[] types,
 *								    ParameterModifier[] modifiers);
 */
ILObject *_IL_ClrType_GetMemberImpl(ILExecThread *thread,
								    ILObject *_this,
								    ILString *name,
								    ILInt32 memberTypes,
								    ILInt32 bindingAttrs,
								    ILObject *binder,
								    ILInt32 callConv,
								    System_Array *types,
								    System_Array *modifiers)
{
#ifdef IL_CONFIG_REFLECTION
	ILClass *classInfo;
	char *nameUtf8;
	ILMember *member;
	ILMember *other;
	ILProgramItem *foundItem;
	ILProgramItem *foundExact;
	ILNestedInfo *nested;
	ILClass *child;
	int inexactIsAmbig;

	/* Validate the parameters */
	if(!name)
	{
		ILExecThreadThrowArgNull(thread, "name");
		return 0;
	}
	if(bindingAttrs == 0)
	{
		bindingAttrs = (ILInt32)(BF_Public | BF_Instance | BF_Static);
	}

	/* Convert the name into a UTF-8 string */
	nameUtf8 = ILStringToUTF8(thread, name);
	if(!nameUtf8)
	{
		return 0;
	}

	/* If the name is greater than 128 characters in length,
	   then turn off the "ignore case" flag.  This is an ECMA
	   requirement, even though we can handle arbitrarily
	   sized member names just fine */
	if(strlen(nameUtf8) >= 128)
	{
		bindingAttrs &= ~(ILInt32)BF_IgnoreCase;
	}

	/* Convert the type into an ILClass structure */
	classInfo = _ILGetClrClass(thread, _this);
	if(!classInfo)
	{
		return 0;
	}

	/* Scan the class hierarchy looking for a member match */
	foundItem = 0;
	foundExact = 0;
	inexactIsAmbig = 0;
	do
	{
		member = classInfo->firstMember;
		while(member != 0)
		{
			/* Check for a name match first */
			if((bindingAttrs & (ILInt32)BF_IgnoreCase) == 0)
			{
				if(strcmp(member->name, nameUtf8) != 0)
				{
					goto advance;
				}
			}
			else
			{
				if(ILStrICmp(member->name, nameUtf8) != 0)
				{
					goto advance;
				}
			}
	
			/* Filter based on the member type */
			if(!MemberTypeMatch(member, memberTypes))
			{
				goto advance;
			}
	
			/* Filter based on the binding attributes and calling conventions */
			if(!BindingAttrMatch(member, bindingAttrs, callConv))
			{
				goto advance;
			}

			/* Check that we have reflection access for this member */
			if(!_ILClrCheckAccess(thread, (ILClass *)0, member))
			{
				goto advance;
			}

			/* Filter based on the parameter types */
			if(types &&
			   (member->kind == IL_META_MEMBERKIND_METHOD ||
			    member->kind == IL_META_MEMBERKIND_PROPERTY))
			{
				/* Check for an exact match */
				if(!ParameterTypeMatch(thread, member->programItem.image,
				                       member->signature, types, 1))
				{
					/* Check for an inexact match if neccessary */
					if(!foundExact && (bindingAttrs & BF_ExactBinding) == 0)
					{
						if(!ParameterTypeMatch(thread,
						                       member->programItem.image,
						                       member->signature, types, 0))
						{
							goto advance;
						}
					}
					else
					{
						goto advance;
					}
				}
				else
				{
					/* This is a candidate member */
					if(foundExact)
					{
						other = ILProgramItemToMember(foundExact);
						if(member->kind == other->kind &&
						   (other = CheckMemberOverride(other, member)))
						{
							foundExact = &(other->programItem);
							goto advance;
						}
						/* The member match is ambiguous */
						goto ambiguous;
					}
					foundExact = &(member->programItem);
				}
			}
			/* This is a candidate member */
			if(!foundExact && foundItem)
			{
				other = ILProgramItemToMember(foundItem);

				if(member->kind == other->kind &&
				   (other = CheckMemberOverride(other, member)))
				{
					foundItem = &(other->programItem);
					goto advance;
				}
				/* The member match is ambiguous */
				inexactIsAmbig = 1;
			}
			foundItem = &(member->programItem);

		advance:
			/* Advance to the next member */
			member = member->nextMember;

			/* Final ambiguouity check */
			if(!member && !foundExact && inexactIsAmbig)
			{
				goto ambiguous;
			}
			continue;

		ambiguous:
			/* The member match is ambiguous */
			ILExecThreadThrowSystem
					(thread, "System.Reflection.AmbiguousMatchException",
					 (const char *)0);
			return 0;
		}

		/* Scan the nested types also */
		if((memberTypes & (ILInt32)MT_NestedType) != 0)
		{
			nested = 0;
			while((nested = ILClassNextNested(classInfo, nested)) != 0)
			{
				child = ILNestedInfoGetChild(nested);
				if(child)
				{
					/* Filter the child based on its name */
					if((bindingAttrs & (ILInt32)BF_IgnoreCase) == 0)
					{
						if(strcmp(ILClass_Name(child), nameUtf8) != 0)
						{
							continue;
						}
					}
					else
					{
						if(ILStrICmp(ILClass_Name(child), nameUtf8) != 0)
						{
							continue;
						}
					}

					/* Filter the child based on its access level */
					if(!BindingAttrClassMatch(child, bindingAttrs))
					{
						continue;
					}

					/* Check that we have reflection access for this child */
					if(!_ILClrCheckAccess(thread, child, (ILMember *)0))
					{
						continue;
					}
	
					/* We have a candidate nested type */
					if(foundItem)
					{
						/* The match is ambiguous */
						goto ambiguous;
					}
					foundItem = &(child->programItem);
				}
			}
		}

		/* Move up the class hierarchy */
		classInfo = ILClass_ParentClass(classInfo);
	}
	while(classInfo != 0 &&
	      (bindingAttrs & (ILInt32)BF_DeclaredOnly) == 0);

	/* Return the item that was found to the caller */
	if(foundExact)
	{
		return ItemToClrObject(thread, foundExact);
	}
	else if(foundItem)
	{
		return ItemToClrObject(thread, foundItem);
	}
	else
	{
		return 0;
	}
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

/*
 * private Object GetMembersImpl(MemberTypes memberTypes,
 *								 BindingFlags bindingAttr,
 *								 Type arrayType, String name);
 */
ILObject *_IL_ClrType_GetMembersImpl(ILExecThread *thread,
									 ILObject *_this,
									 ILInt32 memberTypes,
									 ILInt32 bindingAttrs,
									 ILObject *arrayType,
									 ILString *name)
{
#ifdef IL_CONFIG_REFLECTION
	ILClass *classInfo;
	char *nameUtf8;
	ILMember *member;
	ILObject *foundObject;
	ILNestedInfo *nested;
	ILClass *child;
	System_Array *array;
	System_Array *newArray;
	ILClass *arrayClass;
	ILInt32 numFound;

	/* Validate the parameters */
	if(bindingAttrs == 0)
	{
		bindingAttrs = (ILInt32)(BF_Public | BF_Instance | BF_Static);
	}

	/* Convert the name into a UTF-8 string */
	if(name)
	{
		nameUtf8 = ILStringToUTF8(thread, name);
		if(!nameUtf8)
		{
			return 0;
		}

		/* If the name is greater than 128 characters in length,
		   then turn off the "ignore case" flag.  This is an ECMA
		   requirement, even though we can handle arbitrarily
		   sized member names just fine */
		if(strlen(nameUtf8) >= 128)
		{
			bindingAttrs &= ~(ILInt32)BF_IgnoreCase;
		}
	}
	else
	{
		nameUtf8 = 0;
	}

	/* Allocate the initial array */
	arrayClass = _ILGetClrClass(thread, arrayType);
	if(!arrayClass)
	{
		return 0;
	}
	array = (System_Array *)_ILEngineAlloc(thread, arrayClass,
										   sizeof(System_Array) +
										   sizeof(void *) * 4);
	if(!array)
	{
		return 0;
	}
	ArrayLength(array) = 4;

	/* Convert the type into an ILClass structure */
	classInfo = _ILGetClrClass(thread, _this);
	if(!classInfo)
	{
		return 0;
	}

	/* Scan the class hierarchy looking for member matches */
	numFound = 0;
	do
	{
		member = classInfo->firstMember;
		while(member != 0)
		{
			/* Check for a name match first */
			if(nameUtf8)
			{
				if((bindingAttrs & (ILInt32)BF_IgnoreCase) == 0)
				{
					if(strcmp(member->name, nameUtf8) != 0)
					{
						member = member->nextMember;
						continue;
					}
				}
				else
				{
					if(ILStrICmp(member->name, nameUtf8) != 0)
					{
						member = member->nextMember;
						continue;
					}
				}
			}
	
			/* Filter based on the member type */
			if(!MemberTypeMatch(member, memberTypes))
			{
				member = member->nextMember;
				continue;
			}
	
			/* Filter based on the binding attributes */
			if(!BindingAttrMatch(member, bindingAttrs, CC_Any))
			{
				member = member->nextMember;
				continue;
			}
	
			/* Check that we have reflection access for this member */
			if(!_ILClrCheckAccess(thread, (ILClass *)0, member))
			{
				member = member->nextMember;
				continue;
			}
	
			/* Add the member to the array */
			foundObject = ItemToClrObject(thread, &(member->programItem));
			if(!foundObject)
			{
				return 0;
			}
			if(numFound >= ArrayLength(array))
			{
				newArray = (System_Array *)_ILEngineAlloc
						(thread, arrayClass,
					     sizeof(System_Array) +
						 	sizeof(void *) * (ArrayLength(array) + 4));
				if(!newArray)
				{
					return 0;
				}
				ILMemCpy(ArrayToBuffer(newArray), ArrayToBuffer(array),
						 sizeof(void *) * ArrayLength(array));
				ArrayLength(newArray) = ArrayLength(array) + 4;
				array = newArray;
			}
			((void **)ArrayToBuffer(array))[numFound++] = foundObject;
	
			/* Advance to the next member */
			member = member->nextMember;
		}

		/* Scan the nested types also */
		if((memberTypes & (ILInt32)MT_NestedType) != 0)
		{
			nested = 0;
			while((nested = ILClassNextNested(classInfo, nested)) != 0)
			{
				child = ILNestedInfoGetChild(nested);
				if(child)
				{
					/* Filter the child based on its name */
					if(nameUtf8)
					{
						if((bindingAttrs & (ILInt32)BF_IgnoreCase) == 0)
						{
							if(strcmp(ILClass_Name(child), nameUtf8) != 0)
							{
								continue;
							}
						}
						else
						{
							if(ILStrICmp(ILClass_Name(child), nameUtf8) != 0)
							{
								continue;
							}
						}
					}

					/* Filter the child based on its access level */
					if(!BindingAttrClassMatch(child, bindingAttrs))
					{
						continue;
					}

					/* Check that we have reflection access for this child */
					if(!_ILClrCheckAccess(thread, child, (ILMember *)0))
					{
						continue;
					}
	
					/* Add the nested type to the array */
					foundObject = ItemToClrObject
						(thread, &(child->programItem));
					if(!foundObject)
					{
						return 0;
					}
					if(numFound >= ArrayLength(array))
					{
						newArray = (System_Array *)_ILEngineAlloc
								(thread, arrayClass,
							     sizeof(System_Array) +
								 	sizeof(void *) * (ArrayLength(array) + 4));
						if(!newArray)
						{
							return 0;
						}
						ILMemCpy(ArrayToBuffer(newArray), ArrayToBuffer(array),
								 sizeof(void *) * ArrayLength(array));
						ArrayLength(newArray) = ArrayLength(array) + 4;
						array = newArray;
					}
					((void **)ArrayToBuffer(array))[numFound++] = foundObject;
				}
			}
		}

		/* Move up the class hierarchy */
		classInfo = ILClass_ParentClass(classInfo);
	}
	while(classInfo != 0 &&
	      (bindingAttrs & (ILInt32)BF_DeclaredOnly) == 0);

	/* Truncate the array to its actual length and return it */
	ArrayLength(array) = numFound;
	return (ILObject *)array;
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

/*
 * public static Type GetType(String name, bool throwOnError, bool ignoreCase);
 */
ILObject *_IL_Type_GetType(ILExecThread *thread, ILString *name,
						   ILBool throwOnError, ILBool ignoreCase)
{
#ifdef IL_CONFIG_REFLECTION
	return _ILGetTypeFromImage(thread, 0, name, throwOnError, ignoreCase); 
#else
	_ILClrNotImplemented(thread);
	return 0;
#endif
}

/*
 * public static RuntimeTypeHandle GetTypeHandle(Object o);
 */
void _IL_Type_GetTypeHandle(ILExecThread *thread, void *handle, ILObject *obj)
{
	if(obj)
	{
		*((ILClass **)handle) = GetObjectClass(obj);
	}
	else
	{
		ILExecThreadThrowArgNull(thread, "obj");
	}
}

/*
 * public static Type GetTypeFromHandle(RuntimeTypeHandle handle);
 */
ILObject *_IL_Type_GetTypeFromHandle(ILExecThread *thread, void *handle)
{
#ifdef IL_USE_JIT
	ILClass *classInfo = (ILClass *)handle;
#else
	ILClass *classInfo = *((ILClass **)handle);
#endif
	if(classInfo)
	{
		return _ILGetClrType(thread, classInfo);
	}
	else
	{
		return 0;
	}
}

#ifdef IL_CONFIG_REFLECTION

/*
 * private static MemberInfo[] InternalGetSerializableMembers(type);
 */
System_Array *_IL_FormatterServices_InternalGetSerializableMembers
			(ILExecThread *_thread, ILObject *type)
{
	ILClass *classInfo;
	ILClass *info;
	ILField *field;
	ILInt32 size;
	System_Array *array;
	ILObject **buf;

	/* Convert the type into an ILClass structure */
	classInfo = _ILGetClrClass(_thread, type);

	/* Count the number of serializable fields in the type */
	info = classInfo;
	size = 0;
	while(info != 0)
	{
		field = 0;
		while((field = (ILField *)ILClassNextMemberByKind
					(info, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
		{
			if(!ILField_IsStatic(field) && !ILField_IsNotSerialized(field))
			{
				++size;
			}
		}
		info = ILClass_ParentClass(info);
	}

	/* Allocate an array to hold the serializable fields */
	array = (System_Array *)ILExecThreadNew
				(_thread, "[oSystem.Reflection.MemberInfo;",
				 "(Ti)V", (ILVaInt)size);
	if(!array)
	{
		return 0;
	}
	buf = (ILObject **)(ArrayToBuffer(array));

	/* Populate the array */
	info = classInfo;
	size = 0;
	while(info != 0)
	{
		field = 0;
		while((field = (ILField *)ILClassNextMemberByKind
					(info, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
		{
			if(!ILField_IsStatic(field) && !ILField_IsNotSerialized(field))
			{
				buf[size++] = ItemToClrObject
					(_thread, &(field->member.programItem));
			}
		}
		info = ILClass_ParentClass(info);
	}

	/* Return the final array to the caller */
	return array;
}

#endif /* IL_CONFIG_REFLECTION */

/*
 * protected override bool HasGenericArgumentsImpl();
 */
ILBool _IL_ClrType_HasGenericArgumentsImpl(ILExecThread *thread,
										   ILObject *_this)
{
	/* TODO */
	return 0;
}

/*
 * protected override bool HasGenericParametersImpl();
 */
ILBool _IL_ClrType_HasGenericParametersImpl(ILExecThread *thread,
										    ILObject *_this)
{
	/* TODO */
	return 0;
}

/*
 * public override Type[] GetGenericArguments();
 */
System_Array *_IL_ClrType_GetGenericArguments(ILExecThread *thread,
										      ILObject * _this)
{
	/* TODO */
	return 0;
}

/*
 * public override Type BindGenericParameters(Type[] inst);
 */
ILObject *_IL_ClrType_BindGenericParameters(ILExecThread *thread,
										    ILObject *_this,
											System_Array *inst)
{
	/* TODO */
	return 0;
}

/*
 * public override Type GetGenericTypeDefinition();
 */
ILObject *_IL_ClrType_GetGenericTypeDefinition(ILExecThread *thread,
											   ILObject *_this)
{
	/* TODO */
	return 0;
}

#ifdef	__cplusplus
};
#endif
