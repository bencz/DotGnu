/*
 * link_type.c - Convert a type into a final image type.
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
#include "../image/program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Result codes for "ConvertClassRef".
 */
#define	CONVERT_REF_FAILED		0
#define	CONVERT_REF_MEMORY		1
#define	CONVERT_REF_LIBRARY		2
#define	CONVERT_REF_LOCAL		3

/*
 * Convert a class reference from a foreign image.
 */
static int ConvertClassRef(ILLinker *linker, ILClass *classInfo,
						   ILLibraryFind *find, ILClass **resultInfo)
{
	ILClass *parent;
	ILClass *newClass;
	ILProgramItem *scope;
	ILAssembly *assem;
	ILLibrary *library;
	ILType *synType;
	const char *name;
	const char *namespace;
	char *newName = 0;
	int result;

	/* Is the class nested? */
	parent = ILClassGetNestedParent(classInfo);
	if(parent)
	{
		/* Convert the nested parent, which determines if the top-level
		   class is in a library or should be locally defined */
		result = ConvertClassRef(linker, parent, find, resultInfo);
		if(result == CONVERT_REF_FAILED || result == CONVERT_REF_MEMORY)
		{
			return result;
		}
		else if(result == CONVERT_REF_LIBRARY)
		{
			/* We have a library find context as the parent */
			if(!_ILLinkerFindClass(find, ILClass_Name(classInfo),
								   ILClass_Namespace(classInfo)))
			{
				return CONVERT_REF_FAILED;
			}
			return CONVERT_REF_LIBRARY;
		}
		else
		{
			/* We have an actual TypeDef or TypeRef as the parent */
			scope = ILToProgramItem(*resultInfo);
			newClass = ILClassLookup(scope, ILClass_Name(classInfo),
									 ILClass_Namespace(classInfo));
			if(newClass)
			{
				*resultInfo = newClass;
				return CONVERT_REF_LOCAL;
			}
			newClass = ILClassCreateRef(scope, 0, ILClass_Name(classInfo),
										ILClass_Namespace(classInfo));
			if(!newClass)
			{
				_ILLinkerOutOfMemory(linker);
				return CONVERT_REF_MEMORY;
			}
			*resultInfo = newClass;
			return CONVERT_REF_LOCAL;
		}
	}
	else if((synType = ILClass_SynType(classInfo)) != 0)
	{
		/* Import the synthetic type as a typespec in the current image */
		ILTypeSpec *spec;
		
		spec = _ILLinkerConvertTypeSpec(linker, synType);
		if(!spec)
		{
			return CONVERT_REF_MEMORY;
		}

		newClass = ILTypeSpecGetClassWrapper(spec);
		if(!newClass)
		{
			_ILLinkerOutOfMemory(linker);
			return CONVERT_REF_MEMORY;
		}
		*resultInfo = newClass;
		return CONVERT_REF_LOCAL;
	}
	else if(ILClassIsRef(classInfo))
	{
		/* Converting a top-level class reference to outside the image */
		scope = classInfo->className->scope;
		assem = ILProgramItemToAssembly(scope);
		if(assem && ILAssemblyIsRef(assem) &&
		   (library = _ILLinkerFindLibrary(linker, assem->name)) != 0)
		{
			/* Resolved to a specific library */
			_ILLinkerFindInit(find, linker, library);
			if(_ILLinkerFindClass(find, ILClass_Name(classInfo),
								  ILClass_Namespace(classInfo)))
			{
				return CONVERT_REF_LIBRARY;
			}
			else
			{
				return CONVERT_REF_FAILED;
			}
		}
		_ILLinkerFindInit(find, linker, 0);
		if(_ILLinkerFindClass(find, ILClass_Name(classInfo),
							  ILClass_Namespace(classInfo)))
		{
			/* Resolved to one of the libraries */
			return CONVERT_REF_LIBRARY;
		}
	}
	else if(_ILLinkerLibraryReplacement(linker, find, classInfo))
	{
		/* A local class has been replaced with a C library class */
		return CONVERT_REF_LIBRARY;
	}

	/* If we get here, then we assume that the global class will be
	   in the final image, and so we create a reference to it.  When
	   the link completes, we will scan for dangling TypeRef's */
	scope = ILClassGlobalScope(linker->image);
	name = ILClass_Name(classInfo);
	namespace = ILClass_Namespace(classInfo);
	if(!strcmp(name, "<Module>") && !namespace)
	{
		name = _ILLinkerModuleName(linker);
	}
	else if(ILClass_IsPrivate(classInfo) &&
	        linker->isCLink && !ILClassIsRef(classInfo))
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
		*resultInfo = newClass;
		if(newName)
		{
			ILFree(newName);
		}
		return CONVERT_REF_LOCAL;
	}
	newClass = ILClassCreateRef(scope, 0, name, namespace);
	if(!newClass)
	{
		_ILLinkerOutOfMemory(linker);
		if(newName)
		{
			ILFree(newName);
		}
		return CONVERT_REF_MEMORY;
	}
	*resultInfo = newClass;
	if(newName)
	{
		ILFree(newName);
	}
	return CONVERT_REF_LOCAL;
}

ILClass *_ILLinkerConvertClassRef(ILLinker *linker, ILClass *classInfo)
{
	ILLibraryFind find;
	ILClass *newClass;
	int result = ConvertClassRef(linker, classInfo, &find, &newClass);
	if(result == CONVERT_REF_FAILED)
	{
		/* Report that we couldn't resolve the class */
		ILDumpClassName(stderr, ILClassToImage(classInfo), classInfo, 0);
		fputs(" : unresolved type reference\n", stderr);
		linker->error = 1;
		return 0;
	}
	else if(result == CONVERT_REF_MEMORY)
	{
		/* We ran out of memory while converting the reference */
		return 0;
	}
	else if(result == CONVERT_REF_LIBRARY)
	{
		/* Convert the library reference into a TypeRef */
		return _ILLinkerMakeTypeRef(&find, linker->image);
	}
	else
	{
		/* Return the local or dangling reference */
		return newClass;
	}
}

ILType *_ILLinkerConvertType(ILLinker *linker, ILType *type)
{
	ILClass *classInfo;
	ILType *newType;
	unsigned long param;
	unsigned long numLocals;
	ILType *paramType;

	if(ILType_IsPrimitive(type))
	{
		/* Primitive types always map as themselves */
		return type;
	}
	else if(ILType_IsClass(type))
	{
		/* Convert the class reference */
		classInfo = _ILLinkerConvertClassRef(linker, ILType_ToClass(type));
		if(classInfo)
		{
			return ILType_FromClass(classInfo);
		}
	}
	else if(ILType_IsValueType(type))
	{
		/* Convert the value type reference */
		classInfo = _ILLinkerConvertClassRef
						(linker, ILType_ToValueType(type));
		if(classInfo)
		{
			return ILType_FromValueType(classInfo);
		}
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		/* Convert a complex type */
		newType = ILMemPoolCalloc(&(linker->context->typePool), ILType);
		if(!newType)
		{
			_ILLinkerOutOfMemory(linker);
			return ILType_Invalid;
		}
		*newType = *type;
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_BYREF:
			case IL_TYPE_COMPLEX_PTR:
			case IL_TYPE_COMPLEX_PINNED:
			{
				if((newType->un.refType__ =
					_ILLinkerConvertType(linker, type->un.refType__)) != 0)
				{
					return newType;
				}
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY:
			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				if((newType->un.array__.elemType__ =
					_ILLinkerConvertType
						(linker, type->un.array__.elemType__)) != 0)
				{
					return newType;
				}
			}
			break;

			case IL_TYPE_COMPLEX_CMOD_REQD:
			case IL_TYPE_COMPLEX_CMOD_OPT:
			{
				newType->un.modifier__.info__ = _ILLinkerConvertClassRef
								(linker, type->un.modifier__.info__);
				newType->un.modifier__.type__ = _ILLinkerConvertType
								(linker, type->un.modifier__.type__);
				if(newType->un.modifier__.info__ != 0&&
				   newType->un.modifier__.type__ != 0)
				{
					return newType;
				}
			}
			break;

			case IL_TYPE_COMPLEX_SENTINEL:
			{
				return newType;
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_LOCALS:
			{
				newType->num__ = 0;
				newType->un.locals__.next__ = 0;
				numLocals = ILTypeNumLocals(type);
				for(param = 0; param < numLocals; ++param)
				{
					paramType = _ILLinkerConvertType
						(linker, ILTypeGetLocalWithPrefixes(type, param));
					if(!paramType)
					{
						return ILType_Invalid;
					}
					if(!ILTypeAddLocal(linker->context, newType, paramType))
					{
						_ILLinkerOutOfMemory(linker);
						return ILType_Invalid;
					}
				}
				return newType;
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_VAR:
			case IL_TYPE_COMPLEX_MVAR:
			{
				newType->un.num__ = ILType_VarNum(type);
				return newType;
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_WITH:
			case IL_TYPE_COMPLEX_PROPERTY:
			case IL_TYPE_COMPLEX_METHOD:
			case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
			{
				newType->num__ = 0;
				newType->un.method__.next__ = 0;
				if(type->un.method__.retType__)
				{
					newType->un.method__.retType__ =
						_ILLinkerConvertType
							(linker, type->un.method__.retType__);
					if(!(newType->un.method__.retType__))
					{
						return ILType_Invalid;
					}
				}
				for(param = 1; param <= (ILUInt32)(type->num__); ++param)
				{
					paramType = _ILLinkerConvertType
						(linker, ILTypeGetParamWithPrefixes(type, param));
					if(!paramType)
					{
						return ILType_Invalid;
					}
					if(!ILTypeAddParam(linker->context, newType, paramType))
					{
						_ILLinkerOutOfMemory(linker);
						return ILType_Invalid;
					}
				}
				return newType;
			}
			/* Not reached */
		}
	}
	return ILType_Invalid;
}

ILTypeSpec *_ILLinkerConvertTypeSpec(ILLinker *linker, ILType *type)
{
	ILTypeSpec *spec;

	/* Map the type to the new image */
	type = _ILLinkerConvertType(linker, type);
	if(!type)
	{
		return 0;
	}

	/* Create a new TypeSpec within the output image */
	spec = ILTypeSpecCreate(linker->image, 0, type);
	if(!spec)
	{
		_ILLinkerOutOfMemory(linker);
	}
	return spec;
}

static ILProgramItem *ConvertProgramItemRef(ILLinker *linker,
											ILProgramItem *item)
{
	ILClass *info;
	ILTypeSpec *spec;

	if((spec = ILProgramItemToTypeSpec(item)) != 0)
	{
		spec = _ILLinkerConvertTypeSpec(linker, ILTypeSpec_Type(spec));
		if(!spec)
		{
			return 0;
		}
		return ILToProgramItem(spec);
	}
	else if((info = ILProgramItemToClass(item)) != 0)
	{
		info = _ILLinkerConvertClassRef(linker, info);
		if(!info)
		{
			return 0;
		}
		return ILToProgramItem(info);
	}
	return 0;
}

ILProgramItem *_ILLinkerConvertProgramItemRef(ILLinker *linker,
											  ILProgramItem *item)
{
	return ConvertProgramItemRef(linker, item);
}

#ifdef	__cplusplus
};
#endif
