/*
 * lookup.c - Look up methods and fields by name.
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

/*

The naming scheme is loosely based on the Java signature scheme,
with a few changes to make it C#-friendly.

Types are named using their full name, with namespace separated by
periods.  Nested types must be fully qualified, with the nesting levels
separated by slashes.  e.g. "System.String" and "System.Enum/HashEntry".

Signatures are specified using the following characters:

	V			void
	Z			bool
	b			sbyte
	B			byte
	s			short
	S			ushort
	c			char
	i			int
	I			uint
	j			native int
	J			native uint
	l			long
	L			ulong
	f			float
	d			double
	D			native double
	r			typedref
	&type		byref "type"
	*type		ptr "type"
	vname;		value type called "name"
	oname;		object reference called "name"
	[type		array of "type"
	{n,type		n-dimensional array of "type"
	%(parms)ret	method pointer type
	T			"this" parameter, indicating an instance method

An example of a signature:

	struct System.Decimal
	{
		public System.String ToString();
	}

	"System.Decimal" "ToString" "(T)oSystem.String;"
*/

#include "engine.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Look up a class name that is length-specified.
 */
static ILClass *LookupClass(ILExecThread *thread, const char *className,
							int classNameLen)
{
	ILClass *classInfo = _ILLookupClass(_ILExecThreadProcess(thread),
										className,
										classNameLen);

	/* Make sure that the class has been laid out */
	if(classInfo != 0)
	{
		IL_METADATA_WRLOCK(_ILExecThreadProcess(thread));
		if(!_ILLayoutClass(_ILExecThreadProcess(thread), classInfo))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(thread));
			return 0;
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(thread));
	}

	/* Return the final class structure to the caller */
	return classInfo;
}

/*
 * Look up a class name that is length-specified.
 */
ILClass *_ILLookupClass(ILExecProcess *process,
						const char *className,
						int classNameLen)
{
	int len, dot;
	const char *name;
	int nameLen;
	const char *namespace;
	int namespaceLen;
	ILClass *classInfo;
	ILImage *image;

	/* Get the name of the global type */
	len = 0;
	dot = -1;
	while(len < classNameLen && className[len] != '/' && className[len] != '+')
	{
		if(className[len] == '.')
		{
			dot = len;
		}
		++len;
	}

	/* Break the global type into namespace and name components */
	if(dot != -1)
	{
		name = className + dot + 1;
		nameLen = len - dot - 1;
		namespace = className;
		namespaceLen = dot;
	}
	else
	{
		name = className;
		nameLen = len;
		namespace = 0;
		namespaceLen = 0;
	}

	/* Look for the global type within the process's context */
	classInfo = 0;
	if(classNameLen > 7 && !strncmp(className, "System.", 7))
	{
		/* Try looking in the system image first, to prevent the
		   application from redirecting system types elsewhere */
		image = ILContextGetSystem(process->context);
		if(image)
		{
			classInfo = ILClassLookupLen(ILClassGlobalScope(image),
										 name, nameLen,
										 namespace, namespaceLen);
		}
	}
	if(!classInfo)
	{
		classInfo = ILClassLookupGlobalLen(process->context,
									   	   name, nameLen,
										   namespace, namespaceLen);
	}

	/* Resolve nested type names */
	while(classInfo != 0 && len < classNameLen && (className[len] == '/' || className[len] == '+'))
	{
		++len;
		dot = len;
		while(len < classNameLen && className[len] != '/' && className[len] != '+')
		{
			++len;
		}
		name = className + dot;
		nameLen = len - dot;
		classInfo = ILClassLookupLen(ILToProgramItem(classInfo),
									 name, nameLen, 0, 0);
	}

	/* Return the final class structure to the caller */
	return classInfo;
}

ILClass *ILExecThreadLookupClass(ILExecThread *thread, const char *className)
{
	ILType *type;

	/* Sanity-check the arguments */
	if(!thread || !className)
	{
		return 0;
	}

	/* If the class name begins with a type modifier,
	   then we need to look for a synthetic class */
	if(*className == '[' || *className == '{' ||
	   *className == '&' || *className == '*' ||
	   *className == ':')
	{
		type = ILExecThreadLookupType
				  (thread, (*className != ':' ? className : (className + 1)));
		if(type == ILType_Invalid)
		{
			return 0;
		}
		return ILClassFromType(ILContextNextImage(thread->process->context, 0),
							   0, type, 0);
	}

	/* Look up the class */
	return LookupClass(thread, className, strlen(className));
}

ILType *ILExecThreadLookupType(ILExecThread *thread, const char *typeName)
{
	int len;
	ILClass *classInfo;
	ILType *type;

	/* Sanity-check the arguments */
	if(!thread || !typeName)
	{
		return ILType_Invalid;
	}

	/* Determine how to parse the type */
	switch(*typeName++)
	{
		case 'V':		return ILType_Void;
		case 'Z':		return ILType_Boolean;
		case 'b':		return ILType_Int8;
		case 'B':		return ILType_UInt8;
		case 's':		return ILType_Int16;
		case 'S':		return ILType_UInt16;
		case 'c':		return ILType_Char;
		case 'i':		return ILType_Int32;
		case 'I':		return ILType_UInt32;
		case 'j':		return ILType_Int;
		case 'J':		return ILType_UInt;
		case 'l':		return ILType_Int64;
		case 'L':		return ILType_UInt64;
		case 'f':		return ILType_Float32;
		case 'd':		return ILType_Float64;
		case 'D':		return ILType_Float;
		case 'r':		return ILType_TypedRef;

		case '&':
		{
			/* Recognise a reference type */
			type = ILExecThreadLookupType(thread, typeName);
			if(type != ILType_Invalid)
			{
				return ILTypeCreateRef(thread->process->context,
									   IL_TYPE_COMPLEX_BYREF, type);
			}
		}
		break;

		case '*':
		{
			/* Recognise a pointer type */
			type = ILExecThreadLookupType(thread, typeName);
			if(type != ILType_Invalid)
			{
				return ILTypeCreateRef(thread->process->context,
									   IL_TYPE_COMPLEX_PTR, type);
			}
		}
		break;

		case 'v':
		{
			/* Recognise a named value type */
			len = 0;
			while(typeName[len] != '\0' && typeName[len] != ';')
			{
				++len;
			}
			classInfo = LookupClass(thread, typeName, len);
			if(classInfo)
			{
				return ILType_FromValueType(classInfo);
			}
		}
		break;

		case 'o':
		{
			/* Recognise a named object reference type */
			len = 0;
			while(typeName[len] != '\0' && typeName[len] != ';')
			{
				++len;
			}
			classInfo = LookupClass(thread, typeName, len);
			if(classInfo)
			{
				return ILType_FromClass(classInfo);
			}
		}
		break;

		case '[':
		{
			/* Recognise a single-dimensional array type */
			type = ILExecThreadLookupType(thread, typeName);
			if(type != ILType_Invalid)
			{
				return ILTypeFindOrCreateArray
					(thread->process->context, 1, type);
			}
		}
		break;

		case '{':
		{
			/* Recognise a multi-dimensional array type */
			len = 0;
			while(*typeName >= '0' && *typeName <= '9')
			{
				len = len * 10 + (*typeName - '0');
			}
			if(*typeName != ',' || len <= 0)
			{
				return ILType_Invalid;
			}
			type = ILExecThreadLookupType(thread, typeName + 1);
			if(type != ILType_Invalid)
			{
				return ILTypeFindOrCreateArray
					(thread->process->context, len, type);
			}
		}
		break;

		case '%':
		{
			/* Recognise a method pointer type */
			/* TODO */
		}
		break;

		default: break;
	}

	/* If we get here, we have no idea what the type is */
	return ILType_Invalid;
}

/*
 * Inner version of "MatchClassName".
 */
static int MatchClassNameInner(ILClass *classInfo, const char *name)
{
	ILClass *nestedParent;
	int len, complen, dot;
	const char *temp;

	/* Match the nested parent first */
	nestedParent = ILClass_NestedParent(classInfo);
	if(nestedParent)
	{
		len = MatchClassNameInner(nestedParent, name);
		if(!len || name[len] != '/')
		{
			return 0;
		}
		++len;
	}
	else
	{
		len = 0;
	}

	/* Extract the next component from the name */
	complen = len;
	dot = -1;
	while(name[complen] != '\0' && name[complen] != ';' &&
	      name[complen] != '/')
	{
		if(name[complen] == '.')
		{
			dot = complen;
		}
		++complen;
	}
	if(complen <= len)
	{
		/* Empty components are not valid class names */
		return 0;
	}

	/* Match the component against this class */
	if(nestedParent == 0)
	{
		if(dot != -1)
		{
			/* Match both name and namespace */
			temp = ILClass_Namespace(classInfo);
			if(strlen(temp) != (dot - len) ||
			   strncmp(temp, name + len, dot - len) != 0)
			{
				return 0;
			}
			temp = ILClass_Name(classInfo);
			if(strlen(temp) != (complen - dot - 1) ||
			   strncmp(temp, name + dot + 1, complen - dot - 1) != 0)
			{
				return 0;
			}
		}
		else
		{
			/* This class should not have a namespace */
			if(ILClass_Namespace(classInfo) != 0)
			{
				return 0;
			}
			temp = ILClass_Name(classInfo);
			if(strlen(temp) != (complen - len) ||
			   strncmp(temp, name + len, complen - len) != 0)
			{
				return 0;
			}
		}
	}
	else
	{
		/* Nested classes only match the name */
		temp = ILClass_Name(classInfo);
		if(strlen(temp) != (complen - len) ||
		   strncmp(temp, name + len, complen - len) != 0)
		{
			return 0;
		}
	}

	/* The whole name has been matched */
	return complen;
}

/*
 * Match a class name against a supplied signature name.
 * Returns the number of characters matched, or zero.
 */
static int MatchClassName(ILClass *classInfo, const char *name)
{
	int len = MatchClassNameInner(classInfo, name);
	if(!len || name[len] != ';')
	{
		return 0;
	}
	else
	{
		return len;
	}
}

/*
 * Count the number of ranks in an array type and extract
 * the inner-most element type.  Returns -1 if the array
 * ranks are not as expected.
 */
static int CountRanks(ILType *type, ILType **elemType)
{
	int rank = 1;
	while(ILType_Kind(type) == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
	{
		if(ILType_Size(type) != 0 || ILType_LowBound(type) != 0)
		{
			return -1;
		}
		type = ILType_ElemType(type);
		++rank;
	}
	if(ILType_Size(type) != 0 || ILType_LowBound(type) != 0)
	{
		return -1;
	}
	*elemType = ILType_ElemType(type);
	return rank;
}

/*
 * Match a type against a supplied signature name.  Returns the
 * number of name characters matched, or zero if not matched.
 */
static int MatchTypeName(ILType *type, const char *name)
{
	int len, rank;
	ILType *elemType;
	int kind;

	if(ILType_IsPrimitive(type))
	{
		/* Match a primitive type */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:	    if(*name == 'V') return 1; break;
			case IL_META_ELEMTYPE_BOOLEAN:	if(*name == 'Z') return 1; break;
			case IL_META_ELEMTYPE_I1:		if(*name == 'b') return 1; break;
			case IL_META_ELEMTYPE_U1:		if(*name == 'B') return 1; break;
			case IL_META_ELEMTYPE_I2:		if(*name == 's') return 1; break;
			case IL_META_ELEMTYPE_U2:		if(*name == 'S') return 1; break;
			case IL_META_ELEMTYPE_CHAR:		if(*name == 'c') return 1; break;
			case IL_META_ELEMTYPE_I4:		if(*name == 'i') return 1; break;
			case IL_META_ELEMTYPE_U4:		if(*name == 'I') return 1; break;
			case IL_META_ELEMTYPE_I:		if(*name == 'j') return 1; break;
			case IL_META_ELEMTYPE_U:		if(*name == 'J') return 1; break;
			case IL_META_ELEMTYPE_I8:		if(*name == 'l') return 1; break;
			case IL_META_ELEMTYPE_U8:		if(*name == 'L') return 1; break;
			case IL_META_ELEMTYPE_R4:		if(*name == 'f') return 1; break;
			case IL_META_ELEMTYPE_R8:		if(*name == 'd') return 1; break;
			case IL_META_ELEMTYPE_R:		if(*name == 'D') return 1; break;
			case IL_META_ELEMTYPE_TYPEDBYREF: if(*name == 'r') return 1; break;
		}
		return 0;
	}
	else if(ILType_IsValueType(type))
	{
		/* Match a value type */
		if(*name != 'v')
		{
			return 0;
		}
		len = MatchClassName(ILType_ToValueType(type), name + 1);
		if(len != 0)
		{
			return len + 2;
		}
		else
		{
			return 0;
		}
	}
	else if(ILType_IsClass(type))
	{
		/* Match an object reference */
		if(*name != 'o')
		{
			return 0;
		}
		len = MatchClassName(ILType_ToClass(type), name + 1);
		if(len != 0)
		{
			return len + 2;
		}
		else
		{
			return 0;
		}
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		/* Match some other kind of complex type */
		kind = ILType_Kind(type);
		if(kind == IL_TYPE_COMPLEX_BYREF)
		{
			/* Match a reference type */
			if(*name != '&')
			{
				return 0;
			}
			len = MatchTypeName(ILType_Ref(type), name + 1);
			if(len != 0)
			{
				return len + 1;
			}
			else
			{
				return 0;
			}
		}
		else if(kind == IL_TYPE_COMPLEX_PTR)
		{
			/* Match a pointer type */
			if(*name != '*')
			{
				return 0;
			}
			len = MatchTypeName(ILType_Ref(type), name + 1);
			if(len != 0)
			{
				return len + 1;
			}
			else
			{
				return 0;
			}
		}
		else if(kind == IL_TYPE_COMPLEX_ARRAY)
		{
			/* Match a single-dimensional array type */
			if(*name != '[')
			{
				return 0;
			}
			if(ILType_Size(type) != 0 ||
			   ILType_LowBound(type) != 0)
			{
				return 0;
			}
			len = MatchTypeName(ILType_ElemType(type), name + 1);
			if(len != 0)
			{
				return len + 1;
			}
			else
			{
				return 0;
			}
		}
		else if(kind == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
		{
			/* Match a multi-dimensional array type */
			if(*name != '{')
			{
				return 0;
			}
			len = 1;
			rank = 0;
			while(name[len] != '\0' && name[len] != ',')
			{
				if(name[len] >= '0' && name[len] <= '9')
				{
					rank = rank * 10 + (int)(name[len++]);
				}
				else
				{
					return 0;
				}
			}
			if(name[len] != ',')
			{
				return 0;
			}
			++len;
			if(rank != CountRanks(type, &elemType))
			{
				return 0;
			}
			rank = MatchTypeName(elemType, name + len);
			if(!rank)
			{
				return 0;
			}
			else
			{
				return len + rank;
			}
		}
		else if((kind & IL_TYPE_COMPLEX_METHOD) != 0)
		{
			/* Match a method pointer type */
			if(*name != '%')
			{
				return 0;
			}
			/* TODO */
			return 0;
		}
		else
		{
			/* Don't know how to match this type */
			return 0;
		}
	}
	else
	{
		/* Unrecognised type */
		return 0;
	}
}

/*
 * Match a method signature against a supplied signature name.
 */
static int MatchSignatureName(ILType *signature, const char *name)
{
	ILUInt32 param, numParams;
	ILType *paramType;
	int nameLen;
	int matchLen;

	/* Match the parameters */
	numParams = ILTypeNumParams(signature);
	for(param = 1; param <= numParams; ++param)
	{
		paramType = ILTypeGetParam(signature, param);
		nameLen = MatchTypeName(paramType, name);
		if(!nameLen)
		{
			return 0;
		}
		name += nameLen;
	}

	/* Match the return type */
	if(*name != ')')
	{
		return 0;
	}
	matchLen = MatchTypeName(ILTypeGetReturn(signature), name + 1);
	if(matchLen == 0 || name[matchLen + 1] != '\0')
	{
		return 0;
	}
	else
	{
		return 1;
	}
}

ILMethod *ILExecThreadLookupMethod(ILExecThread *thread,
								   const char *typeName,
								   const char *methodName,
								   const char *signature)
{
	ILClass *classInfo;
	classInfo = ILExecThreadLookupClass(thread, typeName);
	return ILExecThreadLookupMethodInClass
			(thread, classInfo, methodName, signature);
}

ILMethod *ILExecThreadLookupMethodInClass(ILExecThread *thread,
										  ILClass *classInfo,
								   		  const char *methodName,
								   		  const char *signature)
{
	ILMethod *method;
	ILType *methodSignature;

	/* Sanity-check the arguments */
	if(!methodName || !signature || *signature != '(')
	{
		return 0;
	}

	/* Resolve the method within the type or any of its ancestors */
	while(classInfo != 0)
	{
		classInfo = ILClassResolve(classInfo);
		method = 0;
		while((method = (ILMethod *)ILClassNextMemberByKind
					(classInfo, (ILMember *)method,
					 IL_META_MEMBERKIND_METHOD)) != 0)
		{
			if(!strcmp(ILMethod_Name(method), methodName))
			{
				methodSignature = ILMethod_Signature(method);
				if(ILType_HasThis(methodSignature))
				{
					/* The method has a "this" parameter */
					if(signature[1] != 'T')
					{
						continue;
					}
					if(MatchSignatureName(methodSignature, signature + 2))
					{
						return method;
					}
				}
				else
				{
					/* The method does not have a "this" parameter */
					if(MatchSignatureName(methodSignature, signature + 1))
					{
						return method;
					}
				}
			}
		}
		classInfo = ILClass_ParentRef(classInfo);
	}

	/* Could not find the method */
	return 0;
}

ILField *ILExecThreadLookupField(ILExecThread *thread,
								 const char *typeName,
								 const char *fieldName,
								 const char *signature)
{
	ILClass *classInfo;
	classInfo = ILExecThreadLookupClass(thread, typeName);
	return ILExecThreadLookupFieldInClass
		(thread, classInfo, fieldName, signature);	
}

ILField *ILExecThreadLookupFieldInClass(ILExecThread *thread,
										ILClass *classInfo,
										const char *fieldName,
										const char *signature)
{
	ILField *field;
	ILType *fieldType;
	int matchCount;

	/* Sanity-check the arguments */
	if(!fieldName || !signature)
	{
		return 0;
	}

	/* Resolve the field within the type or any of its ancestors */
	while(classInfo != 0)
	{
		classInfo = ILClassResolve(classInfo);
		field = 0;
		while((field = (ILField *)ILClassNextMemberByKind
			(classInfo, (ILMember *)field,
			IL_META_MEMBERKIND_FIELD)) != 0)
		{
			if(!strcmp(ILField_Name(field), fieldName))
			{
				fieldType = ILField_Type(field);
				matchCount = MatchTypeName(fieldType, signature);
				if(matchCount != 0 && signature[matchCount] == '\0')
				{
					return field;
				}
			}
		}
		classInfo = ILClass_ParentRef(classInfo);
	}

	/* Could not find the field */
	return 0;
}

int _ILLookupTypeMatch(ILType *type, const char *signature)
{
	int matchCount;
	if(*signature == '(')
	{
		/* Match a method signature */
		if(type == 0 || !ILType_IsComplex(type) ||
		   ILType_Kind(type) != IL_TYPE_COMPLEX_METHOD)
		{
			return 0;
		}
		if(ILType_HasThis(type))
		{
			/* The method has a "this" parameter */
			if(signature[1] != 'T')
			{
				return 0;
			}
			if(MatchSignatureName(type, signature + 2))
			{
				return 1;
			}
		}
		else
		{
			/* The method does not have a "this" parameter */
			if(MatchSignatureName(type, signature + 1))
			{
				return 1;
			}
		}
	}
	else
	{
		/* Match a field signature */
		matchCount = MatchTypeName(type, signature);
		if(matchCount != 0 && signature[matchCount] == '\0')
		{
			return 1;
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
