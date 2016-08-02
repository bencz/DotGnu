/*
 * link_class.c - Convert classes and copy them to the final image.
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

#include "linker.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Copy a class declaration to the destination image.
 */
static int CreateClass(ILLinker *linker, ILClass *classInfo,
					   ILClass *nestedParent)
{
	ILProgramItem *scope;
	ILProgramItem *parent;
	ILClass *newClass;
	const char *name;
	const char *namespace;
	char *newName = 0;
	int isModule = 0;
	ILLibraryFind find;
	ILNestedInfo *nested;

	parent = ILClass_Parent(classInfo);
	if(parent)
	{
		if(!ILProgramItemToTypeSpec(parent))
		{
			/* Parent is a real class */
			ILClass *parentClass;

			parentClass = ILClass_ParentRef(classInfo);
			if(!parentClass)
			{
				return 0;
			}
			parentClass = _ILLinkerConvertClassRef(linker, parentClass);
			if(!parentClass)
			{
				return 0;
			}
			parent = ILToProgramItem(parentClass);
		}
		else
		{
			parent = 0;
		}
	}

	if(nestedParent)
	{
		scope = (ILProgramItem *)nestedParent;
	}
	else
	{
		scope = ILClassGlobalScope(linker->image);
	}
	name = ILClass_Name(classInfo);
	namespace = ILClass_Namespace(classInfo);
	if(!strcmp(name, "<Module>") && !namespace)
	{
		/* Map the "<Module>" class to its final name */
		name = _ILLinkerModuleName(linker);
	}
	else if(ILClass_IsPrivate(classInfo) && linker->isCLink)
	{
		/* Rename the private class to prevent name clashes
		   with definitions in other C object files */
		newName = _ILLinkerNewClassName(linker, classInfo);
		if(newName)
		{
			name = newName;
			namespace = 0;
		}
	}
	newClass = ILClassLookup(scope, name, namespace);
	if(newClass)
	{
		/* The class already exists */
		if(ILClassIsRef(newClass))
		{
			/* Convert the reference into a real class */
			newClass = ILClassCreate(scope, 0, name, namespace, parent);
			if(!newClass)
			{
				_ILLinkerOutOfMemory(linker);
				if(newName)
				{
					ILFree(newName);
				}
				return 0;
			}
		}
		else
		{
			/* Only proceed if this is the "<Module>" class */
			if(!nestedParent && !strcmp(name, _ILLinkerModuleName(linker)) &&
			   namespace == 0)
			{
				isModule = 1;
			}
			else if(!strncmp(ILClass_Name(classInfo), "$ArrayType$", 11))
			{
				/* Array types are not converted */
				if(newName)
				{
					ILFree(newName);
				}

				return 1;
			}
			else if(linker->isCLink)
			{
				/* Duplicate classes are valid in C objects, as they
				   are normally definitions of the same struct type */
				if(newName)
				{
					ILFree(newName);
				}

				/* Record the new class in the original class */
				ILClassSetUserData(classInfo, (void *)newClass);

				return 1;
			}
			else
			{
				/* The class has already been defined */
				ILDumpClassName(stderr, ILClassToImage(classInfo),
								classInfo, 0);
				fputs(" : defined multiple times\n", stderr);
				linker->error = 1;
				if(newName)
				{
					ILFree(newName);
				}
				return 1;
			}
		}
	}
	else if(_ILLinkerLibraryReplacement(linker, &find, classInfo))
	{
		/* The class is identical to one in a C library, so use that instead */
		if(newName)
		{
			ILFree(newName);
		}
		return 1;
	}
	else
	{
		/* Create a new class */
		newClass = ILClassCreate(scope, 0, name, namespace, parent);
		if(!newClass)
		{
			_ILLinkerOutOfMemory(linker);
			if(newName)
			{
				ILFree(newName);
			}
			return 0;
		}
	}
	if(newName)
	{
		ILFree(newName);
	}

	/* Copy the class attributes if this isn't the "<Module>" type */
	if(strcmp(name, _ILLinkerModuleName(linker)) != 0 || namespace != 0)
	{
		ILClassSetAttrs(newClass, ~((ILUInt32)0), ILClass_Attrs(classInfo));
	}

	/* Record the new class in the original class */
	ILClassSetUserData(classInfo, (void *)newClass);

	/* Create the typedef records for the nested classes */
	nested = 0;
	while((nested = ILClassNextNested(classInfo, nested)) != 0)
	{
		if(!CreateClass(linker, ILNestedInfoGetChild(nested), newClass))
		{
			return 0;
		}
	}

	return 1;
}

/*
 * Convert the class parents and the implemented interfaces.
 */
static int ConvertClassParents(ILLinker *linker, ILClass *classInfo)
{
	ILClass *newClass;
	ILProgramItem *parent;
	ILImplements *implements;
	ILImplements *newImplements;
	ILNestedInfo *nested;

	newClass = (ILClass *)ILClassGetUserData(classInfo);
	if(!newClass)
	{
		return 1;
	}

	parent = ILClass_Parent(classInfo);
	if(parent)
	{
		ILTypeSpec *parentSpec;

		if((parentSpec = ILProgramItemToTypeSpec(parent)) != 0)
		{
			/* Parent is a TypSpec */
			parent = _ILLinkerConvertProgramItemRef(linker, parent);
			if(!parent)
			{
				return 0;
			}
			ILClassSetParent(newClass, parent);
		}
	}

	/* Copy the interface list */
	implements = 0;
	while((implements = ILClassNextImplements(classInfo, implements)) != 0)
	{
		ILProgramItem *interface;

		interface = _ILLinkerConvertProgramItemRef
					(linker, ILImplementsGetInterface(implements));
		if(!interface)
		{
			return 0;
		}
		newImplements = ILClassAddImplements(newClass, interface, 0);
		if(!newImplements)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		if(!_ILLinkerConvertAttrs(linker, (ILProgramItem *)implements,
								  (ILProgramItem *)newImplements))
		{
			return 0;
		}
	}

	/* Create the parents for the nested classes. */
	nested = 0;
	while((nested = ILClassNextNested(classInfo, nested)) != 0)
	{
		if(!ConvertClassParents(linker, ILNestedInfoGetChild(nested)))
		{
			return 0;
		}
	}

	return 1;
}

/*
 * Convert the class members and attributes.
 */
static int ConvertClassMembers(ILLinker *linker, ILClass *classInfo)
{
	ILClass *newClass;
	ILClassLayout *layout;
	ILMember *member;
	ILNestedInfo *nested;

	newClass = (ILClass *)ILClassGetUserData(classInfo);
	if(!newClass)
	{
		return 1;
	}

	/* Convert the custom attributes */
	if(!_ILLinkerConvertAttrs(linker, (ILProgramItem *)classInfo,
							  (ILProgramItem *)newClass))
	{
		return 0;
	}

	/* Convert the security declarations */
	if(!_ILLinkerConvertSecurity(linker, (ILProgramItem *)classInfo,
							     (ILProgramItem *)newClass))
	{
		return 0;
	}

	/* Convert the debug information that is attached to the class */
	if(!_ILLinkerConvertDebug(linker, (ILProgramItem *)classInfo,
							  (ILProgramItem *)newClass))
	{
		return 0;
	}

	/* Copy the class layout information */
	layout = ILClassLayoutGetFromOwner(classInfo);
	if(layout)
	{
		layout = ILClassLayoutCreate(linker->image, 0, newClass,
									 ILClassLayout_PackingSize(layout),
									 ILClassLayout_ClassSize(layout));
		if(!layout)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
	}

#if IL_VERSION_MAJOR > 1
	/* Convert the generic parameters, if any */
	if(!_ILLinkerConvertGenerics(linker, (ILProgramItem *)classInfo,
							     (ILProgramItem *)newClass))
	{
		return 0;
	}
#endif

	/* Convert the class members */
	member = 0;
	while((member = ILClassNextMember(classInfo, member)) != 0)
	{
		switch(ILMemberGetKind(member))
		{
			case IL_META_MEMBERKIND_METHOD:
			{
				/* Skip MemberRef tokens, which are vararg call sites */
				if((ILMember_Token(member) & IL_META_TOKEN_MASK) !=
						IL_META_TOKEN_MEMBER_REF)
				{
					if(!_ILLinkerConvertMethod(linker, (ILMethod *)member,
											   newClass))
					{
						return 0;
					}
				}
			}
			break;

			case IL_META_MEMBERKIND_FIELD:
			{
				if(!_ILLinkerConvertField(linker, (ILField *)member,
										  newClass))
				{
					return 0;
				}
			}
			break;

			case IL_META_MEMBERKIND_PROPERTY:
			{
				if(!_ILLinkerConvertProperty(linker, (ILProperty *)member,
										     newClass))
				{
					return 0;
				}
			}
			break;

			case IL_META_MEMBERKIND_EVENT:
			{
				if(!_ILLinkerConvertEvent(linker, (ILEvent *)member,
										  newClass))
				{
					return 0;
				}
			}
			break;
		}
	}

	/* Create the members and attributes for the nested classes. */
	nested = 0;
	while((nested = ILClassNextNested(classInfo, nested)) != 0)
	{
		if(!ConvertClassMembers(linker, ILNestedInfoGetChild(nested)))
		{
			return 0;
		}
	}

	/* Clear the userdata in the original class */
	ILClassSetUserData(classInfo, 0);

	return 1;
}

int _ILLinkerConvertClasses(ILLinker *linker, ILImage *image)
{
	ILClass *classInfo;

	/* Scan the TypeDef table and convert each top-level type that we find */
	classInfo = 0;
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
	{
		if(ILClassGetNestedParent(classInfo) == 0)
		{
			if(!CreateClass(linker, classInfo, 0))
			{
				return 0;
			}
		}
	}

	/* Now set parents and interfaces in the new classes */
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
	{
		if(ILClassGetNestedParent(classInfo) == 0)
		{
			if(!ConvertClassParents(linker, classInfo))
			{
				return 0;
			}
		}
	}

	/* Now copy members and attributes to the new classes */
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
	{
		if(ILClassGetNestedParent(classInfo) == 0)
		{
			if(!ConvertClassMembers(linker, classInfo))
			{
				return 0;
			}
		}
	}

	/* Done */
	return 1;
}

char *_ILLinkerNewClassName(ILLinker *linker, ILClass *classInfo)
{
	char buf[64];
	const char *name = ILClass_Name(classInfo);
	char *newName;
	if(!strcmp(name, "init-on-demand"))
	{
		/* This class must not be renamed - there should be
		   only one instance of this name per link process */
		return 0;
	}
	sprintf(buf, "-%lu-%lu", (unsigned long)(linker->imageNum),
			(unsigned long)(ILClass_Token(classInfo) & ~IL_META_TOKEN_MASK));
	newName = (char *)ILMalloc(strlen(name) + strlen(buf) + 1);
	if(!newName)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}
	strcpy(newName, name);
	strcat(newName, buf);
	return newName;
}

char *_ILLinkerNewMemberName(ILLinker *linker, ILMember *member)
{
	char buf[64];
	const char *name = ILMember_Name(member);
	char *newName;
	sprintf(buf, "-%lu-%lu", (unsigned long)(linker->imageNum),
			(unsigned long)(ILMember_Token(member) & ~IL_META_TOKEN_MASK));
	newName = (char *)ILMalloc(strlen(name) + strlen(buf) + 1);
	if(!newName)
	{
		_ILLinkerOutOfMemory(linker);
		return 0;
	}
	strcpy(newName, name);
	strcat(newName, buf);
	return newName;
}

int _ILLinkerLibraryReplacement(ILLinker *linker, ILLibraryFind *find,
								ILClass *classInfo)
{
	_ILLinkerFindInit(find, linker, 0);
	if(ILClass_NestedParent(classInfo) == 0 &&
	   _ILLinkerFindClass(find, ILClass_Name(classInfo),
						  ILClass_Namespace(classInfo)))
	{
		if(linker->isCLink)
		{
			return 1;
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
