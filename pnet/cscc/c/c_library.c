/*
 * c_library.c - Register the builtin support library.
 *
 * Copyright (C) 2002, 2008  Southern Storm Software, Pty Ltd.
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

#include <cscc/c/c_internal.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Helper macro that aborts if we run out of memory.
 */
#define	ABORT_IF(var, call)	\
			do { \
				(var) = (call); \
				if(!(var)) \
				{ \
					ILGenOutOfMemory(info); \
				} \
			} while (0)

/*
 * Create a builtin library class with the correct set of attributes.
 */
static ILClass *CreateClass(ILGenInfo *info, ILProgramItem *scope,
						    const char *name, ILClass *parent)
{
	ILClass *classInfo;
	ABORT_IF(classInfo, ILClassCreate
				(scope, 0, name, "OpenSystem.C", ILToProgramItem(parent)));
	ILClassSetAttrs(classInfo, ~0,
				    IL_META_TYPEDEF_PUBLIC |
				    IL_META_TYPEDEF_SEALED |
				    IL_META_TYPEDEF_BEFORE_FIELD_INIT |
					IL_META_TYPEDEF_SERIALIZABLE);
	return classInfo;
}

/*
 * Add a constructor to a class with specific argument types.
 */
static int AddConstructor(ILClass *classInfo, ILType *arg1, ILType *arg2)
{
	ILMethod *method;
	ILType *signature;
	method = ILMethodCreate(classInfo, 0, ".ctor",
					  	    IL_META_METHODDEF_PUBLIC |
					  	    IL_META_METHODDEF_HIDE_BY_SIG |
							IL_META_METHODDEF_SPECIAL_NAME |
							IL_META_METHODDEF_RT_SPECIAL_NAME);
	if(!method)
	{
		return 0;
	}
	signature = ILTypeCreateMethod(ILClassToContext(classInfo), ILType_Void);
	if(!signature)
	{
		return 0;
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
	if(arg1 != ILType_Invalid)
	{
		if(!ILTypeAddParam(ILClassToContext(classInfo), signature, arg1))
		{
			return 0;
		}
	}
	if(arg2 != ILType_Invalid)
	{
		if(!ILTypeAddParam(ILClassToContext(classInfo), signature, arg2))
		{
			return 0;
		}
	}
	ILMemberSetSignature((ILMember *)method, signature);
	return 1;
}

/*
 * Add a constructor to a class with four specific argument types.
 */
static int AddConstructor4(ILClass *classInfo, ILType *arg1, ILType *arg2,
						   ILType *arg3, ILType *arg4)
{
	ILMethod *method;
	ILType *signature;
	method = ILMethodCreate(classInfo, 0, ".ctor",
					  	    IL_META_METHODDEF_PUBLIC |
					  	    IL_META_METHODDEF_HIDE_BY_SIG |
							IL_META_METHODDEF_SPECIAL_NAME |
							IL_META_METHODDEF_RT_SPECIAL_NAME);
	if(!method)
	{
		return 0;
	}
	signature = ILTypeCreateMethod(ILClassToContext(classInfo), ILType_Void);
	if(!signature)
	{
		return 0;
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
	if(!ILTypeAddParam(ILClassToContext(classInfo), signature, arg1))
	{
		return 0;
	}
	if(!ILTypeAddParam(ILClassToContext(classInfo), signature, arg2))
	{
		return 0;
	}
	if(!ILTypeAddParam(ILClassToContext(classInfo), signature, arg3))
	{
		return 0;
	}
	if(!ILTypeAddParam(ILClassToContext(classInfo), signature, arg4))
	{
		return 0;
	}
	ILMemberSetSignature((ILMember *)method, signature);
	return 1;
}

void CGenRegisterLibrary(ILGenInfo *info)
{
	ILImage *image;
	ILProgramItem *scope;
	ILClass *objectClass;
	ILClass *attributeClass;
	ILClass *exceptionClass;
	ILClass *valueTypeClass;
	ILType *stringType;
	ILClass *classInfo;

	/* Create the "OpenSystem.C" simulated assembly */
	image = ILGenCreateBasicImage(info->context, "OpenSystem.C");
	scope = ILClassGlobalScope(image);

	/* Find the "Object", "Attribute", and "Exception" classes */
	objectClass = ILTypeToClass(info, ILFindSystemType(info, "Object"));
	attributeClass = ILTypeToClass(info, ILFindSystemType(info, "Atribute"));
	exceptionClass = ILTypeToClass(info, ILFindSystemType(info, "Exception"));
	valueTypeClass = ILTypeToClass(info, ILFindSystemType(info, "ValueType"));
	stringType = ILFindSystemType(info, "String");

	/* Create "OpenSystem.C.Crt0" */
	classInfo = CreateClass(info, scope, "Crt0", objectClass);
	AddConstructor(classInfo, ILType_Invalid, ILType_Invalid);

	/* Create "OpenSystem.C.FileTable" */
	classInfo = CreateClass(info, scope, "FileTable", objectClass);
	AddConstructor(classInfo, ILType_Invalid, ILType_Invalid);

	/* Create "OpenSystem.C.IsConst" */
	classInfo = CreateClass(info, scope, "IsConst", objectClass);
	AddConstructor(classInfo, ILType_Invalid, ILType_Invalid);

	/* Create "OpenSystem.C.IsFunctionPointer" */
	classInfo = CreateClass(info, scope, "IsFunctionPointer", objectClass);
	AddConstructor(classInfo, ILType_Invalid, ILType_Invalid);

	/* Create "OpenSystem.C.IsManaged" */
	classInfo = CreateClass(info, scope, "IsManaged", objectClass);
	AddConstructor(classInfo, ILType_Invalid, ILType_Invalid);

	/* Create "OpenSystem.C.IsUnmanaged" */
	classInfo = CreateClass(info, scope, "IsUnmanaged", objectClass);
	AddConstructor(classInfo, ILType_Invalid, ILType_Invalid);

	/* Create "OpenSystem.C.IsComplexPointer" */
	classInfo = CreateClass(info, scope, "IsComplexPointer", objectClass);
	AddConstructor(classInfo, ILType_Invalid, ILType_Invalid);

	/* Create "OpenSystem.C.BitFieldAttribute" */
	classInfo = CreateClass(info, scope, "BitFieldAttribute", attributeClass);
	AddConstructor4(classInfo, stringType, stringType,
					ILType_Int32, ILType_Int32);

	/* Create "OpenSystem.C.WeakAliasForAttribute" */
	classInfo = CreateClass(info, scope, "WeakAliasForAttribute",
							attributeClass);
	AddConstructor(classInfo, ILFindSystemType(info, "String"), ILType_Invalid);

	/* Create "OpenSystem.C.InitializerAttribute" */
	classInfo = CreateClass(info, scope, "InitializerAttribute",
						    attributeClass);
	AddConstructor(classInfo, ILType_Invalid, ILType_Invalid);

	/* Create "OpenSystem.C.InitializerOrderAttribute" */
	classInfo = CreateClass(info, scope, "InitializerOrderAttribute",
						    attributeClass);
	AddConstructor(classInfo, ILType_Int32, ILType_Invalid);

	/* Create "OpenSystem.C.FinalizerAttribute" */
	classInfo = CreateClass(info, scope, "FinalizerAttribute", attributeClass);
	AddConstructor(classInfo, ILType_Invalid, ILType_Invalid);

	/* Create "OpenSystem.C.FinalizerOrderAttribute" */
	classInfo = CreateClass(info, scope, "FinalizerOrderAttribute",
							attributeClass);
	AddConstructor(classInfo, ILType_Int32, ILType_Invalid);

	/* Create "OpenSystem.C.ModuleAttribute" */
	classInfo = CreateClass(info, scope, "ModuleAttribute",
							attributeClass);
	AddConstructor(classInfo, ILType_Int32, ILType_Invalid);

	/* Create "OpenSystem.C.LongJmpException" */
	classInfo = CreateClass(info, scope, "LongJmpException", exceptionClass);
	AddConstructor(classInfo, ILType_Int32, ILType_Int32);
}

#ifdef	__cplusplus
};
#endif
