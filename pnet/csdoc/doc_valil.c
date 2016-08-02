/*
 * doc_valil.c - Validate that an IL program implements a csdoc specification.
 *
 * Copyright (C) 2001, 2008, 2009  Southern Storm Software, Pty Ltd.
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

#include <stdio.h>
#include "il_system.h"
#include "il_utils.h"
#include "il_program.h"
#include "il_dumpasm.h"
#include "il_serialize.h"
#include "doc_tree.h"
#include "doc_backend.h"

#ifdef	__cplusplus
extern	"C" {
#endif

char const ILDocProgramHeader[] =
	"CSDOCVALIL " VERSION " - Validate IL binaries against C# documentation";

char const ILDocProgramName[] = "CSDOCVALIL";

ILCmdLineOption const ILDocProgramOptions[] = {
	{"-fimage", 'f', 1,
		"-fimage=PATH",
		"Specify the IL image path to validate."},
	{"-fextra-types-ok", 'f', 1,
		"-fextra-types-ok",
		"It is OK for extra types to appear in the image."},
	{"-fextra-members-ok", 'f', 1,
		"-fextra-members-ok",
		"It is OK for extra members to appear in the image."},
	{"-fassembly-map", 'f', 1,
		"-fassembly-map=NAME1,NAME2",
		"Map the assembly `NAME1' in the image to `NAME2' in the XML file."},
	{"-fxml", 'f', 1,
		"-fxml",
		"Write the output in XML instead of human-readable text."},
	{"-fignore-assembly-names", 'f', 1,
		"-fignore-assembly-names",
		"Ignore assembly names when validating the input image."},
	{0, 0, 0, 0, 0}
};

/*
 * Flag that is set to indicate XML output.
 */
static int xmlOutput = 0;

/*
 * Flag that is set to ignore assembly names.
 */
static int ignoreAssemblyNames = 0;

char *ILDocDefaultOutput(int numInputs, char **inputs, const char *progname)
{
	/* The default output is always stdout */
	return "-";
}

int ILDocValidateOutput(char *outputPath, const char *progname)
{
	/* Nothing to do here: any pathname is considered valid */
	return 1;
}

/*
 * Append two strings.
 */
static char *AppendString(char *str1, const char *str2)
{
	str1 = (char *)ILRealloc(str1, strlen(str1) + strlen(str2) + 1);
	if(!str1)
	{
		ILDocOutOfMemory(0);
	}
	strcat(str1, str2);
	return str1;
}

/*
 * Concatenate two strings omittint the generic arity of the source string.
 */
static char *StrCatOmitArity(char *dest, const char *src,
							ILUInt32 *numGenParams)
{
	if(src && (*src != '\0'))
	{
		char *destPtr = dest + strlen(dest);
		char ch;

		while((ch = *src) != '\0')
		{
 			if(ch != '`')
			{
				*destPtr = *src;
				++destPtr;
				++src;
			}
			else
			{
				if(numGenParams != 0)
				{
					ILUInt32 arity = 0;

					src++;
					while((ch = *src) != '\0')
					{
						/* We can do it this way because classnames are UTF8 */
						if(ch >= '0' && ch <= '9')
						{
							arity = arity * 10 + (ch - '0');
						}
						else
						{
							/* invalid gereric arity */
							arity = 0;
							break;
						}
						++src;
					}
					*numGenParams = arity;
				}
				break;
			}
		}
		*destPtr = '\0';
	}
	return dest;
}

#if IL_VERSION_MAJOR > 1
/*
 * Append the generic parameters to the name.
 */
static char *AppendGenericParams(char *name, ILProgramItem *item,
								 ILUInt32 *firstGenParam)
{
	ILUInt32 current = *firstGenParam;
	ILGenericPar *genPar;

	while((genPar = ILGenericParGetFromOwner(item, current)) != 0)
	{
		const char *genParName = ILGenericParGetName(genPar);

		if(current == *firstGenParam)
		{			
			char *tempName;

			tempName = (char *)ILMalloc(strlen(genParName) +
										strlen(name) + 2);
			if(!tempName)
			{
				ILDocOutOfMemory(0);
			}
			strcpy(tempName, name);
			strcat(tempName, "<");
			strcat(tempName, genParName);
			ILFree(name);
			name = tempName;
		}
		else
		{
			const char *genParName = ILGenericParGetName(genPar);
			char *tempName;

			tempName = (char *)ILMalloc(strlen(genParName) +
										strlen(name) + 2);
			if(!tempName)
			{
				ILDocOutOfMemory(0);
			}
			strcpy(tempName, name);
			strcat(tempName, ",");
			strcat(tempName, genParName);
			ILFree(name);
			name = tempName;
		}
		current++;
	}
	if(current > *firstGenParam)
	{
		char *tempName;

		tempName = (char *)ILMalloc(strlen(name) + 2);
		if(!tempName)
		{
			ILDocOutOfMemory(0);
		}
		strcpy(tempName, name);
		strcat(tempName, ">");
		ILFree(name);
		name = tempName;
		*firstGenParam = current;
	}
	return name;
}
#endif /* IL_VERSION_MAJOR > 1 */

/*
 * Inner function for retrieving the class name including generic parameters.
 */
static char *GetClassNameInner(ILClass *classInfo, ILUInt32 *firstGenParam)
{
	ILClass *nestedParent = ILClassGetNestedParent(classInfo);
	char *name;

	if(nestedParent)
	{
		char *tempName;

		name = GetClassNameInner(nestedParent, firstGenParam);
		tempName = (char *)ILMalloc(strlen(name) +
									strlen(ILClass_Name(classInfo)) + 2);
		if(!tempName)
		{
			ILDocOutOfMemory(0);
		}
		strcpy(tempName, name);
		strcat(tempName, ".");
		StrCatOmitArity(tempName, ILClass_Name(classInfo), 0);
		ILFree(name);
		name = tempName;
	}
	else
	{
		char *tempName;

		tempName = (char *)ILMalloc(strlen(ILClass_Name(classInfo)) + 1);
		if(!tempName)
		{
			ILDocOutOfMemory(0);
		}
		*tempName = '\0';
		StrCatOmitArity(tempName, ILClass_Name(classInfo), 0);
		name = tempName;
	}
#if IL_VERSION_MAJOR > 1
	if(firstGenParam)
	{
		name = AppendGenericParams(name, ILToProgramItem(classInfo),
								   firstGenParam);
	}
#endif
	return name;
}

/*
 * Get the class name including generic parameters.
 */
static char *GetClassName(ILClass *classInfo, int inclGenParams)
{
	if(inclGenParams)
	{
		ILUInt32 nextGenPar = 0;

		return GetClassNameInner(classInfo, &nextGenPar);
	}
	else
	{
		return GetClassNameInner(classInfo, 0);
	}
}

static const char *GetClassNamespace(ILClass *classInfo)
{
	ILClass *nestedParent;

	while((nestedParent = ILClass_NestedParent(classInfo)) != 0)
	{
		classInfo = nestedParent;
	}
	return ILClass_Namespace(classInfo);
}

/*
 * Get the full name of an image class.
 */
static char *GetFullClassName(ILClass *classInfo)
{
	char *name = GetClassName(classInfo, 1);
	const char *namespace = GetClassNamespace(classInfo);

	if(namespace)
	{
		char *fullName;

		fullName = (char *)ILMalloc(strlen(namespace) +
									strlen(name) + 2);
		if(!fullName)
		{
			ILDocOutOfMemory(0);
		}
		strcpy(fullName, namespace);
		strcat(fullName, ".");
		strcat(fullName, name);
		return fullName;
	}
	else
	{
		return name;
	}
}

/*
 * Resolve a class from a classname.
 */
static ILClass *ResolveClass(ILContext *context, const char *name, const char *namespace)
{
	if(name)
	{
		ILClass *classInfo = 0;
		const char *ptr = name;
		const char *nameEnd = 0;
		ILUInt32 numGen = 0;
		char ch;

		while((ch = *ptr) != '\0')
		{
			if(ch == '<')
			{
				/* Start of generic parameters */
				numGen = 1;
				nameEnd = ptr;

				while((ch = *ptr) != '\0')
				{
					if(ch == ',')
					{
						++numGen;
					}
					else if(ch == '>')
					{
						break;
					}
					++ptr;
				}
			}
			else if(ch == '.')
			{
				/* Looks like a nested class */
				if(numGen == 0)
				{
					char *tempName = ILDupNString(name, ptr - name);

					if(classInfo)
					{
						classInfo = ILClassLookup(ILToProgramItem(classInfo),
												  tempName, 0);
					}
					else
					{
						classInfo = ILClassLookupGlobal(context, tempName, namespace);
					}
					ILFree(tempName);
				}
				else
				{
					char *tempName = ILDupNString(name, nameEnd - name);
					char buffer[261];

					sprintf(buffer, "%s`%i", tempName, numGen);
					if(classInfo)
					{
						classInfo = ILClassLookup(ILToProgramItem(classInfo),
												  buffer, 0);
					}
					else
					{
						classInfo = ILClassLookupGlobal(context, buffer, namespace);
					}
					ILFree(tempName);
					numGen = 0;
				}
				if(classInfo)
				{
					/* Set the start of the mext name to the character
					   following the period. */
					name = ptr + 1;
				}
				else
				{
					/* Failed to resolve the class */
					return 0;
				}
			}		
			++ptr;
		}
		if(ptr > name)
		{
			if(numGen == 0)
			{
				char *tempName = ILDupNString(name, ptr - name);

				if(classInfo)
				{
					classInfo = ILClassLookup(ILToProgramItem(classInfo),
											  tempName, 0);
				}
				else
				{
					classInfo = ILClassLookupGlobal(context, tempName, namespace);
				}
				ILFree(tempName);
			}
			else
			{
				char *tempName = ILDupNString(name, nameEnd - name);
				char buffer[261];

				sprintf(buffer, "%s`%i", tempName, numGen);
				if(classInfo)
				{
					classInfo = ILClassLookup(ILToProgramItem(classInfo),
											  buffer, 0);
				}
				else
				{
					classInfo = ILClassLookupGlobal(context, buffer, namespace);
				}
				ILFree(tempName);
			}
			return classInfo;		
		}
	}
	return 0;
}

/*
 * Determine if an image class is a delegate type.
 */
static int IsDelegateType(ILClass *classInfo)
{
	ILClass *parent = ILClass_UnderlyingParentClass(classInfo);
	const char *name;
	while(parent != 0)
	{
		name = ILClass_Name(parent);
		if(!strcmp(name, "MulticastDelegate"))
		{
			name = ILClass_Namespace(parent);
			if(name && !strcmp(name, "System"))
			{
				return 1;
			}
		}
		parent = ILClass_UnderlyingParentClass(parent);
	}
	return 0;
}

/*
 * Type attribute flags that are relevant during comparisons.
 */
#define	VALID_TYPE_FLAGS	(IL_META_TYPEDEF_VALID_BITS & \
							 ~IL_META_TYPEDEF_HAS_SECURITY & \
						     ~IL_META_TYPEDEF_BEFORE_FIELD_INIT & \
							 ~IL_META_TYPEDEF_SERIALIZABLE)

/*
 * Field attribute flags that are relevant during comparisons.
 */
#define	VALID_FIELD_FLAGS	(0x7FFF & \
							 ~IL_META_FIELDDEF_HAS_SECURITY & \
							 ~IL_META_FIELDDEF_INIT_ONLY)

/*
 * Method attribute flags that are relevant during comparisons.
 */
#define	VALID_METHOD_FLAGS	(0x7FFF & \
							 ~IL_META_METHODDEF_PINVOKE_IMPL & \
							 ~IL_META_METHODDEF_HAS_SECURITY & \
							 ~IL_META_METHODDEF_NEW_SLOT)

/*
 * Constructor attribute flags that are relevant during comparisons.
 */
#define	VALID_CTOR_FLAGS	(0x7FFF & \
							 ~IL_META_METHODDEF_PINVOKE_IMPL & \
							 ~IL_META_METHODDEF_HAS_SECURITY & \
							 ~IL_META_METHODDEF_HIDE_BY_SIG & \
							 ~IL_META_METHODDEF_RT_SPECIAL_NAME)

/*
 * Event attribute flags that are relevant during comparisons.
 */
#define	VALID_EVENT_FLAGS	(0x7FFF & \
							 ~IL_META_METHODDEF_HAS_SECURITY & \
							 ~IL_META_METHODDEF_HIDE_BY_SIG & \
							 ~IL_META_METHODDEF_SPECIAL_NAME)

#if IL_VERSION_MAJOR > 1
/*
 * Inner function for retrieving the class name including generic parameters.
 */
static char *WithTypeToName(ILType *withType, int shortForm,
							ILProgramItem *scope);
#endif /* IL_VERSION_MAJOR > 1 */

/*
 * Convert an image type into a type name.
 */
static char *TypeToName(ILType *type, int shortForm, ILParameter *parameter,
						ILProgramItem *scope)
{
	char *name;
	ILClass *classInfo;
	int posn, kind;

	/* Try to convert class or value types to their primitive form first. */
	if(ILType_IsClass(type) || ILType_IsValueType(type))
	{
		ILType *tempType = ILClassToPrimitiveType(ILType_ToClass(type));

		if(tempType)
		{
			type = tempType;
		}
	}
	if(ILType_IsPrimitive(type))
	{
		if(shortForm)
		{
			switch(ILType_ToElement(type))
			{
				case IL_META_ELEMTYPE_VOID:		name = "void"; break;
				case IL_META_ELEMTYPE_BOOLEAN:	name = "bool"; break;
				case IL_META_ELEMTYPE_I1:		name = "sbyte"; break;
				case IL_META_ELEMTYPE_U1:		name = "byte"; break;
				case IL_META_ELEMTYPE_I2:		name = "short"; break;
				case IL_META_ELEMTYPE_U2:		name = "ushort"; break;
				case IL_META_ELEMTYPE_CHAR:		name = "char"; break;
				case IL_META_ELEMTYPE_I4:		name = "int"; break;
				case IL_META_ELEMTYPE_U4:		name = "uint"; break;
				case IL_META_ELEMTYPE_I8:		name = "long"; break;
				case IL_META_ELEMTYPE_U8:		name = "ulong"; break;
				case IL_META_ELEMTYPE_I:		name = "IntPtr"; break;
				case IL_META_ELEMTYPE_U:		name = "UIntPtr"; break;
				case IL_META_ELEMTYPE_R4:		name = "float"; break;
				case IL_META_ELEMTYPE_R8:		name = "double"; break;
				case IL_META_ELEMTYPE_TYPEDBYREF:
								name = "TypedReference"; break;
				default:						name = "*Unknown*"; break;
			}
		}
		else
		{
			switch(ILType_ToElement(type))
			{
				case IL_META_ELEMTYPE_VOID:		name = "System.Void"; break;
				case IL_META_ELEMTYPE_BOOLEAN:	name = "System.Boolean"; break;
				case IL_META_ELEMTYPE_I1:		name = "System.SByte"; break;
				case IL_META_ELEMTYPE_U1:		name = "System.Byte"; break;
				case IL_META_ELEMTYPE_I2:		name = "System.Int16"; break;
				case IL_META_ELEMTYPE_U2:		name = "System.UInt16"; break;
				case IL_META_ELEMTYPE_CHAR:		name = "System.Char"; break;
				case IL_META_ELEMTYPE_I4:		name = "System.Int32"; break;
				case IL_META_ELEMTYPE_U4:		name = "System.UInt32"; break;
				case IL_META_ELEMTYPE_I8:		name = "System.Int64"; break;
				case IL_META_ELEMTYPE_U8:		name = "System.UInt64"; break;
				case IL_META_ELEMTYPE_I:		name = "System.IntPtr"; break;
				case IL_META_ELEMTYPE_U:		name = "System.UIntPtr"; break;
				case IL_META_ELEMTYPE_R4:		name = "System.Single"; break;
				case IL_META_ELEMTYPE_R8:		name = "System.Double"; break;
				case IL_META_ELEMTYPE_TYPEDBYREF:
								name = "System.TypedReference"; break;
				default:						name = "*Unknown*"; break;
			}
		}
		name = ILDupString(name);
	}
	else if(ILType_IsClass(type) || ILType_IsValueType(type))
	{
		classInfo = ILType_ToClass(type);
		if(shortForm)
		{
			name = GetClassName(classInfo, 0);
		}
		else
		{
			name = GetFullClassName(classInfo);
		}
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		const char *prefix = 0;
		const char *suffix = 0;
		char buffer[128];

		kind = ILType_Kind(type);
		if(kind == IL_TYPE_COMPLEX_BYREF)
		{
			name = TypeToName(ILType_Ref(type), shortForm, parameter, scope);
			if(parameter && ILParameter_IsOut(parameter))
			{
				prefix = "out ";
			}
			else
			{
				prefix = "ref ";
			}
		}
		else if(kind == IL_TYPE_COMPLEX_PTR)
		{
			name = TypeToName(ILType_Ref(type), shortForm, parameter, scope);
			suffix = "*";
		}
		else if(kind == IL_TYPE_COMPLEX_ARRAY)
		{
			if(parameter)
			{
				/* Look for the "ParamArrayAttribute" marker */
				ILAttribute *attr = 0;
				ILMethod *ctor;
				while((attr = ILProgramItemNextAttribute
							(ILToProgramItem(parameter), attr)) != 0)
				{
					ctor = ILProgramItemToMethod
						(ILAttribute_TypeAsItem(attr));
					if(ctor && !strcmp(ILClass_Name(ILMethod_Owner(ctor)),
									   "ParamArrayAttribute"))
					{
						prefix = "params ";
					}
				}
				parameter = 0;
			}
			name = TypeToName(ILType_Ref(type), shortForm, parameter, scope);
			suffix = "[]";
		}
		else if(kind == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
		{
			if(parameter)
			{
				/* Look for the "ParamArrayAttribute" marker */
				ILAttribute *attr = 0;
				ILMethod *ctor;
				while((attr = ILProgramItemNextAttribute
							(ILToProgramItem(parameter), attr)) != 0)
				{
					ctor = ILProgramItemToMethod
						(ILAttribute_TypeAsItem(attr));
					if(ctor && !strcmp(ILClass_Name(ILMethod_Owner(ctor)),
									   "ParamArrayAttribute"))
					{
						prefix = "params ";
					}
				}
				parameter = 0;
			}
			buffer[0] = '[';
			posn = 1;
			while(type != 0 && ILType_IsComplex(type) &&
			      ILType_Kind(type) == IL_TYPE_COMPLEX_ARRAY_CONTINUE &&
				  posn < (sizeof(buffer) - 8))
			{
				buffer[posn++] = ',';
				type = ILType_ElemType(type);
			}
			buffer[posn++] = ']';
			buffer[posn] = '\0';
			suffix = buffer;
			if(type != 0 && ILType_IsComplex(type) &&
			   ILType_Kind(type) == IL_TYPE_COMPLEX_ARRAY)
			{
				name = TypeToName(ILType_ElemType(type), shortForm, parameter, scope);
			}
			else
			{
				name = ILDupString("*Unknown*");
			}
		}
	#if IL_VERSION_MAJOR > 1
		else if(kind == IL_TYPE_COMPLEX_MVAR)
		{
			ILMethod *method = ILProgramItemToMethod(scope);

			if(method)
			{
				ILUInt32 paramNum = ILType_VarNum(type);
				ILGenericPar *genPar;

				genPar = ILGenericParGetFromOwner(scope, paramNum);
				if(genPar)
				{
					name = ILDupString(ILGenericParGetName(genPar));
				}
				else
				{
					name = ILDupString("*Unknown*");
				}
			}
			else
			{
				name = ILDupString("*Unknown*");
			}
		}
		else if(kind == IL_TYPE_COMPLEX_VAR)
		{
			ILMethod *method = ILProgramItemToMethod(scope);

			if(method)
			{
				ILUInt32 paramNum = ILType_VarNum(type);
				ILGenericPar *genPar;

				genPar = ILGenericParGetFromOwner
							(ILToProgramItem(ILMethod_Owner(method)),
							 paramNum);
				if(genPar)
				{
					name = ILDupString(ILGenericParGetName(genPar));
				}
				else
				{
					name = ILDupString("*Unknown*");
				}
			}
			else
			{
				ILClass *classInfo = ILProgramItemToClass(scope);

				if(classInfo)
				{
					ILUInt32 paramNum = ILType_VarNum(type);
					ILGenericPar *genPar;

					genPar = ILGenericParGetFromOwner(scope, paramNum);
					if(genPar)
					{
						name = ILDupString(ILGenericParGetName(genPar));
					}
					else
					{
						name = ILDupString("*Unknown*");
					}
				}
				else
				{
					name = ILDupString("*Unknown*");
				}
			}
		}
		else if(kind == IL_TYPE_COMPLEX_WITH)
		{
			name = WithTypeToName(type, shortForm, scope);
		}
	#endif /* IL_VERSION_MAJOR > 1 */
		else
		{
			name = ILDupString("*Unknown*");
			suffix = "";
		}
		if(prefix)
		{
			int len = strlen(prefix) + strlen(name) + 1;
			char *tempName;

			if(suffix)
			{
				len += strlen(suffix);
			}
			tempName = (char *)ILMalloc(len);
			if(!tempName)
			{
				ILDocOutOfMemory(0);
			}
			strcpy(tempName, prefix);
			strcat(tempName, name);
			if(suffix)
			{
				strcat(tempName, suffix);
			}
			ILFree(name);
			name = tempName;
		}
		else if(suffix)
		{
			name = (char *)ILRealloc(name, strlen(name) + strlen(suffix) + 1);
			if(!name)
			{
				ILDocOutOfMemory(0);
			}
			strcat(name, suffix);
		}
	}
	else
	{
		name = ILDupString("*Unknown*");
	}
	if(!name)
	{
		ILDocOutOfMemory(0);
	}
	return name;
}

#if IL_VERSION_MAJOR > 1
/*
 * Inner function for retrieving the class name including generic parameters.
 */
static char *WithTypeToNameInner(ILClass *classInfo, int shortForm,
								 ILProgramItem *scope,
								 ILUInt32 *first,
								 ILType *withType)
{
	ILClass *nestedParent = ILClassGetNestedParent(classInfo);
	ILUInt32 arity = 0;
	char *name;

	if(nestedParent)
	{
		char *tempName;

		name = WithTypeToNameInner(nestedParent, shortForm, scope, first,
								   withType);
		tempName = (char *)ILMalloc(strlen(name) +
									strlen(ILClass_Name(classInfo)) + 2);
		if(!tempName)
		{
			ILDocOutOfMemory(0);
		}
		strcpy(tempName, name);
		strcat(tempName, ".");
		StrCatOmitArity(tempName, ILClass_Name(classInfo), &arity);
		ILFree(name);
		name = tempName;
	}
	else
	{
		char *tempName;

		tempName = (char *)ILMalloc(strlen(ILClass_Name(classInfo)) + 1);
		if(!tempName)
		{
			ILDocOutOfMemory(0);
		}
		*tempName = '\0';
		StrCatOmitArity(tempName, ILClass_Name(classInfo), &arity);
		name = tempName;
	}
	if(arity > 0)
	{
		ILUInt32 current = *first;
		ILUInt32 firstGeneric = current;
		ILType *type;

		*first += arity;
		while(arity > 0 &&
			  (type = ILTypeGetWithParamWithPrefixes(withType, current + 1)) != 0)
		{
			char *tempName;

			if(current == firstGeneric)
			{
				name = AppendString(name, "<");
			}
			else
			{
				name = AppendString(name, ",");
			}
			tempName = TypeToName(type, shortForm, 0, scope);
			name = AppendString(name, tempName);
			ILFree(tempName);
			++current;
			--arity;
		}
		if(current > firstGeneric)
		{
			name = AppendString(name, ">");
		}
	}
	return name;
}

static char *WithTypeToName(ILType *withType, int shortForm,
							ILProgramItem *scope)
{
	ILType *mainType = ILTypeGetWithMain(withType);

	if(ILType_IsClass(mainType) || ILType_IsValueType(mainType))
	{
		ILClass *classInfo = ILType_ToClass(mainType);
		ILUInt32 first = 0;
		char *typeName;

		typeName = WithTypeToNameInner(classInfo, shortForm, scope, &first,
									   withType);

		if(!shortForm)
		{
			char *name = ILDupString(GetClassNamespace(classInfo));

			name = AppendString(name, ".");
			name = AppendString(name, typeName);
			ILFree(typeName);
			typeName = name;
		}
		return typeName;
	}
	return 0;
}
#endif /* IL_VERSION_MAJOR > 1 */

/*
 * Match an image type against a name from an XML file.
 * Returns zero if the types do not match.
 */
static int MatchType(ILType *type, const char *typeName,
					 ILParameter *parameter, ILProgramItem *scope)
{
	char *name = TypeToName(type, 1, parameter, scope);
	if(!strcmp(name, typeName))
	{
		ILFree(name);
		return 1;
	}
	else
	{
		/* Try again with the long form of the name */
		ILFree(name);
		name = TypeToName(type, 0, parameter, scope);
		if(!strcmp(name, typeName))
		{
			ILFree(name);
			return 1;
		}
		ILFree(name);
		return 0;
	}
}

/*
 * Match a method parameter signature.
 */
static int MatchSignature(ILMethod *method, ILDocMember *member)
{
	ILDocParameter *param = member->parameters;
	ILType *signature = ILMethod_Signature(method);
	unsigned numParams = ILTypeNumParams(signature);
	unsigned paramNum = 1;
	while(param != 0 && paramNum <= numParams)
	{
		ILParameter *parameter = 0;

		while((parameter = ILMethodNextParam(method, parameter)) != 0)
		{
			if(ILParameterGetNum(parameter) == paramNum)
			{
				break;
			}
		}

		if(!MatchType(ILTypeGetParam(signature, paramNum), param->type,
					  parameter, ILToProgramItem(method)))
		{
			return 0;
		}
		param = param->next;
		++paramNum;
	}
	if(param != 0 || paramNum <= numParams)
	{
		return 0;
	}
	if(!strcmp(member->name, "op_Explicit") ||
	   !strcmp(member->name, "op_Implicit"))
	{
		/* The return type is part of the signature of a conversion */
		if(!MatchType(ILTypeGetReturn(signature), member->returnType, 0,
					  ILToProgramItem(method)))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Match a property parameter signature.
 */
static int MatchPropertySignature(ILMethod *method,
								  ILDocMember *member, int isSet)
{
	ILDocParameter *param = member->parameters;
	ILType *signature = ILMethod_Signature(method);
	unsigned numParams = ILTypeNumParams(signature);
	unsigned paramNum = 1;
	while(param != 0 && paramNum <= numParams)
	{
		ILParameter *parameter = 0;

		while((parameter = ILMethodNextParam(method, parameter)) != 0)
		{
			if(ILParameterGetNum(parameter) == paramNum)
			{
				break;
			}
		}

		if(!MatchType(ILTypeGetParam(signature, paramNum), param->type,
					  parameter, ILToProgramItem(method)))
		{
			return 0;
		}
		param = param->next;
		++paramNum;
	}
	if(isSet)
	{
		/* Matching against a "set" accessor */
		if(param != 0 || paramNum != numParams)
		{
			return 0;
		}
	}
	else
	{
		/* Matching against a "get" accessor */
		if(param != 0 || paramNum <= numParams)
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Print a string to a stream, quoting as necessary for XML.
 */
static void PrintString(const char *str, FILE *stream)
{
	if(xmlOutput)
	{
		char ch;
		while((ch = *str++) != '\0')
		{
			if(ch == '<')
			{
				fputs("&lt;", stream);
			}
			else if(ch == '>')
			{
				fputs("&gt;", stream);
			}
			else if(ch == '&')
			{
				fputs("&amp;", stream);
			}
			else if(ch == '"')
			{
				fputs("&quot;", stream);
			}
			else if(ch == '\'')
			{
				fputs("&apos;", stream);
			}
			else
			{
				putc(ch, stream);
			}
		}
	}
	else
	{
		fputs(str, stream);
	}
}

/*
 * Print the name of an image type.
 */
static void PrintType(FILE *stream, ILType *type, ILParameter *parameter,
					  ILProgramItem *scope)
{
	char *name = TypeToName(type, 0, parameter, scope);
	PrintString(name, stream);
	ILFree(name);
}

/*
 * Print the name of a method, including its signature.
 */
static void PrintMethodName(FILE *stream, ILDocMember *member)
{
	ILDocParameter *param;

	/* Print the type and member name */
	if((member->memberAttrs & IL_META_METHODDEF_STATIC) != 0)
	{
		PrintString("static ", stream);
	}
	if(xmlOutput && member->memberType != ILDocMemberType_Constructor)
	{
		if(member->returnType)
		{
			PrintString(member->returnType, stream);
			putc(' ', stream);
		}
		PrintString(member->name, stream);
	}
	else
	{
		PrintString(member->type->fullName, stream);
		if(member->memberType != ILDocMemberType_Constructor)
		{
			PrintString("::", stream);
			PrintString(member->name, stream);
		}
	}

	/* Print the parameters */
	putc('(', stream);
	param = member->parameters;
	while(param != 0)
	{
		PrintString(param->type, stream);
		if(xmlOutput && param->name)
		{
			putc(' ', stream);
			PrintString(param->name, stream);
		}
		param = param->next;
		if(param != 0)
		{
			fputs(", ", stream);
		}
	}
	putc(')', stream);
	if(!strcmp(member->name, "op_Implicit") ||
	   !strcmp(member->name, "op_Explicit"))
	{
		/* Report the return type too for conversion operators */
		PrintString(" : ", stream);
		PrintString(member->returnType, stream);
	}
}

/*
 * Flag that indicates if a member or type name has already been written.
 */
static int memberNameWritten = 0;
static int typeNameWritten = 0;

/*
 * Print the name and category of a program item.
 */
static void PrintName(FILE *stream, ILDocType *type, ILDocMember *member)
{
	if(member)
	{
		if(xmlOutput && !typeNameWritten)
		{
			fputs("\t<class name=\"", stream);
			PrintString(type->name, stream);
			if(type->assembly)
			{
				fputs("\" assembly=\"", stream);
				PrintString(type->assembly, stream);
			}
			fputs("\" namespace=\"", stream);
			if(type->namespace)
			{
				PrintString(type->namespace->name, stream);
			}
			fputs("\">\n", stream);
			typeNameWritten = 1;
		}
		switch(member->memberType)
		{
			case ILDocMemberType_Constructor:
			{
				if(xmlOutput)
				{
					if(!memberNameWritten)
					{
						fputs("\t\t<ctor signature=\"", stream);
						PrintMethodName(stream, member);
						fputs("\">\n", stream);
						memberNameWritten = 1;
					}
				}
				else
				{
					PrintMethodName(stream, member);
					fputs(" constructor ", stream);
				}
			}
			break;

			case ILDocMemberType_Method:
			{
				if(xmlOutput)
				{
					if(!memberNameWritten)
					{
						fputs("\t\t<method name=\"", stream);
						PrintString(member->name, stream);
						fputs("\" signature=\"", stream);
						PrintMethodName(stream, member);
						fputs("\">\n", stream);
						memberNameWritten = 1;
					}
				}
				else
				{
					PrintMethodName(stream, member);
					fputs(" method ", stream);
				}
			}
			break;

			case ILDocMemberType_Field:
			{
				if(xmlOutput)
				{
					if(!memberNameWritten)
					{
						fputs("\t\t<field name=\"", stream);
						PrintString(member->name, stream);
						fputs("\" type=\"", stream);
						PrintString(member->returnType, stream);
						fputs("\">\n", stream);
						memberNameWritten = 1;
					}
				}
				else
				{
					PrintString(type->fullName, stream);
					fputs("::", stream);
					PrintString(member->name, stream);
					fputs(" field ", stream);
				}
			}
			break;

			case ILDocMemberType_Property:
			{
				if(xmlOutput)
				{
					if(!memberNameWritten)
					{
						fputs("\t\t<property name=\"", stream);
						PrintString(member->name, stream);
						fputs("\" type=\"", stream);
						PrintString(member->returnType, stream);
						fputs("\">\n", stream);
						memberNameWritten = 1;
					}
				}
				else
				{
					PrintString(type->fullName, stream);
					fputs("::", stream);
					PrintString(member->name, stream);
					fputs(" property ", stream);
				}
			}
			break;

			case ILDocMemberType_Event:
			{
				if(xmlOutput)
				{
					if(!memberNameWritten)
					{
						fputs("\t\t<event name=\"", stream);
						PrintString(member->name, stream);
						fputs("\">\n", stream);
						memberNameWritten = 1;
					}
				}
				else
				{
					PrintString(type->fullName, stream);
					fputs("::", stream);
					PrintString(member->name, stream);
					fputs(" event ", stream);
				}
			}
			break;

			case ILDocMemberType_Unknown:
			{
				if(xmlOutput)
				{
					if(!memberNameWritten)
					{
						fputs("\t\t<member name=\"", stream);
						PrintString(member->name, stream);
						fputs("\">\n", stream);
						memberNameWritten = 1;
					}
				}
				else
				{
					PrintString(type->fullName, stream);
					fputs("::", stream);
					PrintString(member->name, stream);
					fputs(" member ", stream);
				}
			}
			break;
		}
	}
	else
	{
		if(xmlOutput)
		{
			if(!typeNameWritten)
			{
				fputs("\t<class name=\"", stream);
				PrintString(type->name, stream);
				if(type->assembly)
				{
					fputs("\" assembly=\"", stream);
					PrintString(type->assembly, stream);
				}
				fputs("\" namespace=\"", stream);
				if(type->namespace)
				{
					PrintString(type->namespace->name, stream);
				}
				fputs("\">\n", stream);
				typeNameWritten = 1;
			}
		}
		else
		{
			PrintString(type->fullName, stream);
			putc(' ', stream);
		}
	}
}

/*
 * Terminate information about a particular member.
 */
static void PrintEndMember(FILE *stream, ILDocType *type, ILDocMember *member)
{
	if(!xmlOutput)
	{
		return;
	}
	if(member)
	{
		if(!memberNameWritten)
		{
			return;
		}
		switch(member->memberType)
		{
			case ILDocMemberType_Constructor:
			{
				fputs("\t\t</ctor>\n", stream);
			}
			break;

			case ILDocMemberType_Method:
			{
				fputs("\t\t</method>\n", stream);
			}
			break;

			case ILDocMemberType_Field:
			{
				fputs("\t\t</field>\n", stream);
			}
			break;

			case ILDocMemberType_Property:
			{
				fputs("\t\t</property>\n", stream);
			}
			break;

			case ILDocMemberType_Event:
			{
				fputs("\t\t</event>\n", stream);
			}
			break;

			case ILDocMemberType_Unknown:
			{
				fputs("\t\t</member>\n", stream);
			}
			break;
		}
		memberNameWritten = 0;
	}
	else if(typeNameWritten)
	{
		fputs("\t</class>\n", stream);
		typeNameWritten = 0;
	}
}

/*
 * Print the parameter signature for a method from an IL image.
 */
static void PrintILSignature(FILE *stream, ILMember *member)
{
	ILType *signature = ILMember_Signature(member);
	const char *memberName = ILMember_Name(member);
	unsigned numParams = ILTypeNumParams(signature);
	unsigned paramNum;
	ILType *paramType;
	putc('(', stream);
	for(paramNum = 1; paramNum <= numParams; ++paramNum)
	{
		ILParameter *parameter = 0;
		ILMethod *method = ILProgramItemToMethod(ILToProgramItem(member));

		if(method)
		{
			while((parameter = ILMethodNextParam(method, parameter)) != 0)
			{
				if(ILParameterGetNum(parameter) == paramNum)
				{
					break;
				}
			}
		}

		paramType = ILTypeGetParam(signature, paramNum);
		PrintType(stream, paramType, parameter, ILToProgramItem(member));
		if(paramNum != numParams)
		{
			fputs(", ", stream);
		}
	}
	putc(')', stream);
	if(!strcmp(memberName, "op_Implicit") ||
	   !strcmp(memberName, "op_Explicit"))
	{
		/* Report the return type too for conversion operators */
		fputs(" : ", stream);
		PrintType(stream, ILTypeGetReturn(signature), 0,
				  ILToProgramItem(member));
	}
}

/*
 * Print the name and category of a type member in an IL image.
 * Returns zero if the member can be ignored (PInvoke, override, etc).
 */
static int PrintILName(FILE *stream, ILDocType *type, ILMember *member)
{
	if(member)
	{
		switch(ILMemberGetKind(member))
		{
			case IL_META_MEMBERKIND_METHOD:
			{
				if(ILMethod_IsConstructor((ILMethod *)member))
				{
					if(xmlOutput)
					{
						PrintILName(stream, type, 0);
						if(!memberNameWritten)
						{
							fputs("\t\t<ctor signature=\"", stream);
							PrintString(type->fullName, stream);
							PrintILSignature(stream, member);
							fputs("\">\n", stream);
							memberNameWritten = 1;
						}
					}
					else
					{
						fputs(type->fullName, stream);
						PrintILSignature(stream, member);
						fputs(" constructor ", stream);
					}
				}
				else
				{
					if(xmlOutput)
					{
						PrintILName(stream, type, 0);
						if(!memberNameWritten)
						{
							fputs("\t\t<method name=\"", stream);
							PrintString(ILMember_Name(member), stream);
							fputs("\" signature=\"", stream);
							if(ILMethod_IsStatic((ILMethod *)member))
							{
								fputs("static ", stream);
							}
							PrintType(stream,
								ILTypeGetReturn(ILMember_Signature(member)),
								0, ILToProgramItem(member));
							putc(' ', stream);
							PrintString(ILMember_Name(member), stream);
							PrintILSignature(stream, member);
							fputs("\">\n", stream);
							memberNameWritten = 1;
						}
					}
					else
					{
						if(ILMethod_IsStatic((ILMethod *)member))
						{
							fputs("static ", stream);
						}
						fputs(type->fullName, stream);
						fputs("::", stream);
						fputs(ILMember_Name(member), stream);
						PrintILSignature(stream, member);
						fputs(" method ", stream);
					}
				}
			}
			break;

			case IL_META_MEMBERKIND_FIELD:
			{
				if(xmlOutput)
				{
					PrintILName(stream, type, 0);
					if(!memberNameWritten)
					{
						fputs("\t\t<field name=\"", stream);
						PrintString(ILMember_Name(member), stream);
						fputs("\" type=\"", stream);
						PrintType(stream, ILField_Type((ILField *)member), 0,
								  ILToProgramItem(ILMember_Owner(member)));
						fputs("\">\n", stream);
						memberNameWritten = 1;
					}
				}
				else
				{
					fputs(type->fullName, stream);
					fputs("::", stream);
					fputs(ILMember_Name(member), stream);
					fputs(" field ", stream);
				}
			}
			break;
	
			case IL_META_MEMBERKIND_PROPERTY:
			{
				if(xmlOutput)
				{
					PrintILName(stream, type, 0);
					if(!memberNameWritten)
					{
						fputs("\t\t<property name=\"", stream);
						PrintString(ILMember_Name(member), stream);
						fputs("\" type=\"", stream);
						PrintType(stream, 
							ILTypeGetReturn(ILMember_Signature(member)),
							0, ILToProgramItem(ILMember_Owner(member)));
						fputs("\">\n", stream);
						memberNameWritten = 1;
					}
				}
				else
				{
					fputs(type->fullName, stream);
					fputs("::", stream);
					fputs(ILMember_Name(member), stream);
					fputs(" property ", stream);
				}
			}
			break;

			case IL_META_MEMBERKIND_EVENT:
			{
				if(xmlOutput)
				{
					PrintILName(stream, type, 0);
					if(!memberNameWritten)
					{
						fputs("\t\t<event name=\"", stream);
						PrintString(ILMember_Name(member), stream);
						fputs("\">\n", stream);
						memberNameWritten = 1;
					}
				}
				else
				{
					fputs(type->fullName, stream);
					fputs("::", stream);
					fputs(ILMember_Name(member), stream);
					fputs(" event ", stream);
				}
			}
			break;
			
			default: return 0;
		}
	}
	else if(xmlOutput && !typeNameWritten)
	{
		fputs("\t<class name=\"", stream);
		PrintString(type->fullName, stream);
		if(type->assembly)
		{
			fputs("\" assembly=\"", stream);
			PrintString(type->assembly, stream);
		}
		fputs("\">\n", stream);
		typeNameWritten = 1;
	}
	return 1;
}

/*
 * Print the end of an IL member description.
 */
static void PrintILEndMember(FILE *stream, ILDocType *type, ILMember *member)
{
	if(!xmlOutput)
	{
		return;
	}
	if(member)
	{
		if(!memberNameWritten)
		{
			return;
		}
		switch(ILMemberGetKind(member))
		{
			case IL_META_MEMBERKIND_METHOD:
			{
				if(ILMethod_IsConstructor((ILMethod *)member))
				{
					fputs("\t\t</ctor>\n", stream);
				}
				else
				{
					fputs("\t\t</method>\n", stream);
				}
			}
			break;
	
			case IL_META_MEMBERKIND_FIELD:
			{
				fputs("\t\t</field>\n", stream);
			}
			break;
	
			case IL_META_MEMBERKIND_PROPERTY:
			{
				fputs("\t\t</property>\n", stream);
			}
			break;
	
			case IL_META_MEMBERKIND_EVENT:
			{
				fputs("\t\t</event>\n", stream);
			}
			break;
		}
		memberNameWritten = 0;
	}
	else if(typeNameWritten)
	{
		fputs("\t</class>\n", stream);
		typeNameWritten = 0;
	}
}

/*
 * Attribute usage flags.
 */
#define	AttrUsage_Assembly		0x0001
#define	AttrUsage_Module		0x0002
#define	AttrUsage_Class			0x0004
#define	AttrUsage_Struct		0x0008
#define	AttrUsage_Enum			0x0010
#define	AttrUsage_Constructor	0x0020
#define	AttrUsage_Method		0x0040
#define	AttrUsage_Property		0x0080
#define	AttrUsage_Field			0x0100
#define	AttrUsage_Event			0x0200
#define	AttrUsage_Interface		0x0400
#define	AttrUsage_Parameter		0x0800
#define	AttrUsage_Delegate		0x1000
#define	AttrUsage_ReturnValue	0x2000
#if IL_VERSION_MAJOR > 1
#define	AttrUsage_GenericParameter	0x4000
#define	AttrUsage_All			0x7FFF
#else /* IL_VERSION_MAJOR == 1 */
#define	AttrUsage_All			0x3FFF
#endif /* IL_VERSION_MAJOR == 1 */
#define	AttrUsage_ClassMembers	0x17FC

/*
 * Append an attribute usage target value to a string.
 */
static char *AppendAttrUsage(char *name, ILInt32 targets)
{
	int needOr = 0;

	/* Handle the easy case first */
	if(targets == AttrUsage_All)
	{
		return AppendString(name, "AttributeTargets.All");
	}

	/* Add the active flag names */
#define	AttrUsage(flag,flagName)	\
		do { \
			if((targets & (flag)) != 0) \
			{ \
				if(needOr) \
				{ \
					name = AppendString(name, " | "); \
				} \
				else \
				{ \
					needOr = 1; \
				} \
				name = AppendString(name, "AttributeTargets." flagName); \
			} \
		} while (0)
	AttrUsage(AttrUsage_Assembly, "Assembly");
	AttrUsage(AttrUsage_Module, "Module");
	AttrUsage(AttrUsage_Class, "Class");
	AttrUsage(AttrUsage_Struct, "Struct");
	AttrUsage(AttrUsage_Enum, "Enum");
	AttrUsage(AttrUsage_Constructor, "Constructor");
	AttrUsage(AttrUsage_Method, "Method");
	AttrUsage(AttrUsage_Property, "Property");
	AttrUsage(AttrUsage_Field, "Field");
	AttrUsage(AttrUsage_Event, "Event");
	AttrUsage(AttrUsage_Interface, "Interface");
	AttrUsage(AttrUsage_Parameter, "Parameter");
	AttrUsage(AttrUsage_Delegate, "Delegate");
	AttrUsage(AttrUsage_ReturnValue, "ReturnValue");
#if IL_VERSION_MAJOR > 1
	AttrUsage(AttrUsage_GenericParameter, "GenericParameter");
#endif /* IL_VERSION_MAJOR > 1 */
	return name;
}

/*
 * Append an attribute value to a name.  Returns NULL
 * if the value is invalid.
 */
static char *AppendAttrValue(char *name, ILSerializeReader *reader,
							 int type, int isUsage)
{
	ILInt32 intValue;
	ILUInt32 uintValue;
	ILInt64 longValue;
	ILUInt64 ulongValue;
	ILFloat floatValue;
	ILDouble doubleValue;
	const char *strValue;
	int strLen, len;
	char buffer[64];

	switch(type)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		{
			intValue = ILSerializeReaderGetInt32(reader, type);
			if(intValue)
			{
				strcpy(buffer, "true");
			}
			else
			{
				strcpy(buffer, "false");
			}
		}
		break;

		case IL_META_SERIALTYPE_I1:
		case IL_META_SERIALTYPE_U1:
		case IL_META_SERIALTYPE_I2:
		case IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_CHAR:
		case IL_META_SERIALTYPE_I4:
		{
			intValue = ILSerializeReaderGetInt32(reader, type);
			if(!isUsage)
			{
				sprintf(buffer, "%ld", (long)intValue);
			}
			else
			{
				return AppendAttrUsage(name, intValue);
			}
		}
		break;

		case IL_META_SERIALTYPE_U4:
		{
			uintValue = ILSerializeReaderGetUInt32(reader, type);
			sprintf(buffer, "%lu", (unsigned long)uintValue);
		}
		break;

		case IL_META_SERIALTYPE_I8:
		{
			longValue = ILSerializeReaderGetInt64(reader);
			sprintf(buffer, "0x%08lX%08lX",
					(unsigned long)((longValue >> 32) & IL_MAX_UINT32),
					(unsigned long)(longValue & IL_MAX_UINT32));
		}
		break;

		case IL_META_SERIALTYPE_U8:
		{
			ulongValue = ILSerializeReaderGetUInt64(reader);
			sprintf(buffer, "0x%08lX%08lX",
					(unsigned long)((ulongValue >> 32) & IL_MAX_UINT32),
					(unsigned long)(ulongValue & IL_MAX_UINT32));
		}
		break;

		case IL_META_SERIALTYPE_R4:
		{
			floatValue = ILSerializeReaderGetFloat32(reader);
			sprintf(buffer, "%.30e", (double)floatValue);
		}
		break;

		case IL_META_SERIALTYPE_R8:
		{
			doubleValue = ILSerializeReaderGetFloat64(reader);
			sprintf(buffer, "%.30e", (double)doubleValue);
		}
		break;

		case IL_META_SERIALTYPE_STRING:
		{
			strLen = ILSerializeReaderGetString(reader, &strValue);
			if(strLen == -1)
			{
				ILFree(name);
				return 0;
			}
			len = strlen(name);
			name = (char *)ILRealloc(name, len + strLen + 3);
			if(!name)
			{
				ILDocOutOfMemory(0);
			}
			name[len++] = '"';
			ILMemCpy(name + len, strValue, strLen);
			name[len + strLen] = '"';
			name[len + strLen + 1] = '\0';
			return name;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_TYPE:
		{
			strLen = ILSerializeReaderGetString(reader, &strValue);
			if(strLen == -1)
			{
				ILFree(name);
				return 0;
			}
			len = strlen(name);
			name = (char *)ILRealloc(name, len + strLen + 9);
			if(!name)
			{
				ILDocOutOfMemory(0);
			}
			strcpy(name + len, "typeof(");
			len += 7;
			ILMemCpy(name + len, strValue, strLen);
			name[len + strLen] = ')';
			name[len + strLen + 1] = '\0';
			return name;
		}
		/* Not reached */

		default:
		{
			if((type & IL_META_SERIALTYPE_ARRAYOF) != 0)
			{
				intValue = ILSerializeReaderGetArrayLen(reader);
				name = AppendString(name, "{");
				while(intValue > 0)
				{
					name = AppendAttrValue(name, reader,
									       type & ~IL_META_SERIALTYPE_ARRAYOF,
										   0);
					if(!name)
					{
						return 0;
					}
					--intValue;
					if(intValue > 0)
					{
						name = AppendString(name, ", ");
					}
				}
				return AppendString(name, "}");
			}
			else
			{
				ILFree(name);
				return 0;
			}
		}
		/* Not reached */
	}

	/* Append the buffer to the name and return */
	return AppendString(name, buffer);
}

/*
 * Convert a program item attribute into a string-form name.
 * Returns NULL if the attribute is private or invalid.
 */
static char *AttributeToName(ILAttribute *attr)
{
	ILMethod *method;
	char *name;
	const void *value;
	ILUInt32 len;
	ILSerializeReader *reader;
	ILUInt32 numParams;
	ILUInt32 numExtras;
	int needComma;
	int type, posn;
	ILMember *member;
	const char *memberName;
	int memberNameLen;
	int isUsage;

	/* Get the attribute constructor and validate it */
	method = ILProgramItemToMethod(ILAttributeTypeAsItem(attr));
	if(!method)
	{
		return 0;
	}

	/* Get the attribute name */
	name = ILDupString(ILClass_Name(ILMethod_Owner(method)));
	if(!name)
	{
		ILDocOutOfMemory(0);
	}

	/* If the attribute is private, and not "TODO", then bail out */
	if((ILClass_IsPrivate(ILMethod_Owner(method)) &&
	    !ILClassIsRef(ILMethod_Owner(method))) &&
	   strcmp(name, "TODOAttribute") != 0)
	{
		ILFree(name);
		return 0;
	}

	/* We need special handling for the first parameter of
	   the "AttributeUsage" attribute */
	isUsage = (!strcmp(name, "AttributeUsageAttribute"));

	/* Get the attribute value and prepare to parse it */
	value = ILAttributeGetValue(attr, &len);
	if(!value)
	{
		return 0;
	}
	reader = ILSerializeReaderInit(method, value, len);
	if(!reader)
	{
		ILDocOutOfMemory(0);
	}

	/* Get the attribute arguments */
	numParams = ILTypeNumParams(ILMethod_Signature(method));
	needComma = 0;
	while(numParams > 0)
	{
		if(!needComma)
		{
			name = AppendString(name, "(");
			needComma = 1;
		}
		else
		{
			name = AppendString(name, ", ");
		}
		type = ILSerializeReaderGetParamType(reader);
		if(type == -1)
		{
			ILFree(name);
			return 0;
		}
		name = AppendAttrValue(name, reader, type, isUsage);
		if(!name)
		{
			return 0;
		}
		--numParams;
	}

	/* Get the extra field and property values */
	numExtras = ILSerializeReaderGetNumExtra(reader);
	while(numExtras > 0)
	{
		if(!needComma)
		{
			name = AppendString(name, "(");
			needComma = 1;
		}
		else
		{
			name = AppendString(name, ", ");
		}
		type = ILSerializeReaderGetExtra(reader, &member, &memberName,
										 &memberNameLen);
		if(type == -1)
		{
			ILFree(name);
			return 0;
		}
		name = (char *)ILRealloc(name, strlen(name) + memberNameLen + 2);
		if(!name)
		{
			ILDocOutOfMemory(0);
		}
		posn = strlen(name);
		ILMemCpy(name + posn, memberName, memberNameLen);
		strcpy(name + posn + memberNameLen, "=");
		name = AppendAttrValue(name, reader, type, 0);
		if(!name)
		{
			return 0;
		}
		--numExtras;
	}

	/* Add the closing parenthesis if necessary */
	if(needComma)
	{
		name = AppendString(name, ")");
	}

	/* Cleanup */
	ILSerializeReaderDestroy(reader);

	/* Return the name to the caller */
	return name;
}

/*
 * Validate the attributes on a program item.
 * Returns zero if a validation error occurred.
 */
static int ValidateAttributes(FILE *stream, ILDocType *type,
							  ILDocMember *member, ILDocAttribute *attrs,
							  ILProgramItem *item)
{
	int valid = 1;
	ILDocAttribute *docAttr;
	ILAttribute *itemAttr;
	char *name;

	/* Scan through the documentation's attributes */
	docAttr = attrs;
	while(docAttr != 0)
	{
		itemAttr = 0;
		while((itemAttr = ILProgramItemNextAttribute(item, itemAttr)) != 0)
		{
			name = AttributeToName(itemAttr);
			if(name)
			{
				if(!strcmp(name, docAttr->name))
				{
					ILFree(name);
					break;
				}
				ILFree(name);
			}
		}
		if(!itemAttr)
		{
			if(xmlOutput)
			{
				PrintName(stream, type, member);
				if(member)
					fputs("\t\t\t<attribute name=\"", stream);
				else
					fputs("\t\t<attribute name=\"", stream);
				PrintString(docAttr->name, stream);
				fputs("\">\n", stream);
				if(member)
					fputs("\t\t\t\t<missing/>\n\t\t\t</attribute>\n", stream);
				else
					fputs("\t\t\t<missing/>\n\t\t</attribute>\n", stream);
			}
			else
			{
				PrintName(stream, type, member);
				fprintf(stream, "should have custom attribute %s\n",
						docAttr->name);
			}
			valid = 0;
		}
		docAttr = docAttr->next;
	}

	/* Scan through the actual attributes */
	itemAttr = 0;
	while((itemAttr = ILProgramItemNextAttribute(item, itemAttr)) != 0)
	{
		name = AttributeToName(itemAttr);
		if(!name)
		{
			continue;
		}
		docAttr = attrs;
		while(docAttr != 0)
		{
			if(!strcmp(name, docAttr->name))
			{
				break;
			}
			docAttr = docAttr->next;
		}
		if(!docAttr && strcmp(name, "TODOAttribute") != 0)
		{
			if(xmlOutput)
			{
				PrintName(stream, type, member);
				if(member)
					fputs("\t\t\t<attribute name=\"", stream);
				else
					fputs("\t\t<attribute name=\"", stream);
				PrintString(name, stream);
				fputs("\">\n", stream);
				if(member)
					fputs("\t\t\t\t<extra/>\n\t\t\t</attribute>\n", stream);
				else
					fputs("\t\t\t<extra/>\n\t\t</attribute>\n", stream);
			}
			else
			{
				PrintName(stream, type, member);
				fprintf(stream, "has extra custom attribute %s\n", name);
			}
			valid = 0;
		}
		else if(!docAttr)
		{
			/* This member is marked as "TODO" */
			if(xmlOutput)
			{
				PrintName(stream, type, member);
				if(member)
					fputs("\t\t\t<todo/>\n", stream);
				else
					fputs("\t\t<todo/>\n", stream);
			}
			else
			{
				PrintName(stream, type, member);
				fprintf(stream, "is unimplemented\n");
			}
			valid = 0;
		}
		ILFree(name);
	}

	/* Done */
	return valid;
}

/*
 * Validate a constructor member against an IL image.
 * Returns zero if a validation error occurred.
 */
static int ValidateConstructor(FILE *stream, ILImage *image,
							   ILClass *classInfo, ILDocType *type,
							   ILDocMember *member)
{
	int valid = 1;
	ILMethod *method;

	/* Locate the constructor method in the class */
	method = 0;
	while((method = (ILMethod *)ILClassNextMemberByKind
				(classInfo, (ILMember *)method,
				 IL_META_MEMBERKIND_METHOD)) != 0)
	{
		if(ILMethod_IsConstructor(method) &&
		   MatchSignature(method, member))
		{
			break;
		}
	}
	if(!method)
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<missing/>\n", stream);
		}
		else
		{
			fputs("is missing\n", stream);
		}
		return 0;
	}

	/* Mark the constructor as seen so we don't pick
	   it up during the "extras" phase */
	ILMemberSetAttrs((ILMember *)method, 0x8000, 0x8000);

	/* Validate the constructor attributes */
	if((ILMethod_Attrs(method) & VALID_CTOR_FLAGS) !=
			(member->memberAttrs & VALID_CTOR_FLAGS))
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<msg>", stream);
		}
		PrintString("should have attributes `", stream);
		ILDumpFlags(stream, (member->memberAttrs & VALID_CTOR_FLAGS),
				    ILMethodDefinitionFlags, 0);
		PrintString("', but has `", stream);
		ILDumpFlags(stream, (ILMethod_Attrs(method) & VALID_CTOR_FLAGS),
				    ILMethodDefinitionFlags, 0);
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}

	/* Validate the constructor attributes */
	if(!ValidateAttributes(stream, type, member, member->attributes,
						   ILToProgramItem(method)))
	{
		valid = 0;
	}

	/* Return the validation result to the caller */
	return valid;
}

/*
 * Validate a method member against an IL image.
 * Returns zero if a validation error occurred.
 */
static int ValidateMethod(FILE *stream, ILImage *image,
						  ILClass *classInfo, ILDocType *type,
					      ILDocMember *member)
{
	int valid = 1;
	ILMethod *method;

	/* Locate the method in the class */
	method = 0;
	while((method = (ILMethod *)ILClassNextMemberByKind
				(classInfo, (ILMember *)method,
				 IL_META_MEMBERKIND_METHOD)) != 0)
	{
	#if IL_VERSION_MAJOR > 1
		char *name = ILDupString(ILMethod_Name(method));
		ILUInt32 firstGenParam = 0;

		name = AppendGenericParams(name, ILToProgramItem(method),
								   &firstGenParam);
	#else /* IL_VERSION_MAJOR == 1 */
		const char *name = ILMethod_Name(method);
	#endif /* IL_VERSION_MAJOR == 1 */

		if(!strcmp(name, member->name) &&
		   MatchSignature(method, member))
		{
			/* Check the static vs instance state */
			if(ILMethod_IsStatic(method))
			{
				if((member->memberAttrs & IL_META_METHODDEF_STATIC) != 0)
				{
				#if IL_VERSION_MAJOR > 1
					ILFree(name);
				#endif /* IL_VERSION_MAJOR > 1 */
					break;
				}
			}
			else
			{
				if((member->memberAttrs & IL_META_METHODDEF_STATIC) == 0)
				{
				#if IL_VERSION_MAJOR > 1
					ILFree(name);
				#endif /* IL_VERSION_MAJOR > 1 */
					break;
				}
			}
		}
	#if IL_VERSION_MAJOR > 1
		ILFree(name);
	#endif /* IL_VERSION_MAJOR > 1 */
	}
	if(!method)
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<missing/>\n", stream);
		}
		else
		{
			fputs("is missing\n", stream);
		}
		return 0;
	}

	/* Mark the method as seen so we don't pick
	   it up during the "extras" phase */
	ILMemberSetAttrs((ILMember *)method, 0x8000, 0x8000);

	/* Validate the method attributes */
	if((ILMethod_Attrs(method) & VALID_METHOD_FLAGS) !=
			(member->memberAttrs & VALID_METHOD_FLAGS))
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<msg>", stream);
		}
		PrintString("should have attributes `", stream);
		ILDumpFlags(stream, (member->memberAttrs & VALID_METHOD_FLAGS),
				    ILMethodDefinitionFlags, 0);
		PrintString("', but has `", stream);
		ILDumpFlags(stream, (ILMethod_Attrs(method) & VALID_METHOD_FLAGS),
				    ILMethodDefinitionFlags, 0);
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}

	/* Match the return type */
	if(!MatchType(ILTypeGetReturn(ILMethod_Signature(method)),
				  member->returnType, 0, ILToProgramItem(method)))
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<msg>", stream);
		}
		PrintString("should have return type `", stream);
		PrintString(member->returnType, stream);
		PrintString("', but has `", stream);
		PrintType(stream, ILTypeGetReturn(ILMethod_Signature(method)),
				  0, ILToProgramItem(method));
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}

	/* Validate the method attributes */
	if(!ValidateAttributes(stream, type, member, member->attributes,
						   ILToProgramItem(method)))
	{
		valid = 0;
	}

	/* Return the validation result to the caller */
	return valid;
}

/*
 * Validate a field member against an IL image.
 * Returns zero if a validation error occurred.
 */
static int ValidateField(FILE *stream, ILImage *image,
						 ILClass *classInfo, ILDocType *type,
					     ILDocMember *member)
{
	int valid = 1;
	ILField *field;

	/* Locate the field in the class */
	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
				(classInfo, (ILMember *)field,
				 IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!strcmp(ILField_Name(field), member->name))
		{
			break;
		}
	}
	if(!field)
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<missing/>\n", stream);
		}
		else
		{
			fputs("is missing\n", stream);
		}
		return 0;
	}

	/* Mark the field as seen so we don't pick
	   it up during the "extras" phase */
	ILMemberSetAttrs((ILMember *)field, 0x8000, 0x8000);

	/* Validate the field attributes */
	if((ILField_Attrs(field) & VALID_FIELD_FLAGS) !=
			(member->memberAttrs & VALID_FIELD_FLAGS))
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<msg>", stream);
		}
		PrintString("should have attributes `", stream);
		ILDumpFlags(stream, (member->memberAttrs & VALID_FIELD_FLAGS),
				    ILFieldDefinitionFlags, 0);
		PrintString("', but has `", stream);
		ILDumpFlags(stream, (ILField_Attrs(field) & VALID_FIELD_FLAGS),
				    ILFieldDefinitionFlags, 0);
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}

	/* Match the field type */
	if(!MatchType(ILField_Type(field), member->returnType, 0,
				  ILToProgramItem(ILField_Owner(field))))
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<msg>", stream);
		}
		PrintString("should have type `", stream);
		PrintString(member->returnType, stream);
		PrintString("', but has `", stream);
		PrintType(stream, ILField_Type(field), 0,
				  ILToProgramItem(ILField_Owner(field)));
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}

	/* Validate the field attributes */
	if(!ValidateAttributes(stream, type, member, member->attributes,
						   ILToProgramItem(field)))
	{
		valid = 0;
	}

	/* Return the validation result to the caller */
	return valid;
}

/*
 * Validate a property member against an IL image.
 * Returns zero if a validation error occurred.
 */
static int ValidateProperty(FILE *stream, ILImage *image,
						    ILClass *classInfo, ILDocType *type,
					        ILDocMember *member)
{
	int valid = 1;
	ILProperty *property;
	ILMethod *accessor;
	int isSet;
	ILType *propertyType;

	/* Locate the property in the class */
	property = 0;
	accessor = 0;
	propertyType = 0;
	while((property = (ILProperty *)ILClassNextMemberByKind
				(classInfo, (ILMember *)property,
				 IL_META_MEMBERKIND_PROPERTY)) != 0)
	{
		/* Check the name */
		if(strcmp(ILProperty_Name(property), member->name) != 0)
		{
			continue;
		}

		/* Find the get or set accessor for the property */
		accessor = ILPropertyGetGetter(property);
		if(accessor)
		{
			isSet = 0;
			propertyType = ILTypeGetReturn(ILMethod_Signature(accessor));
		}
		else
		{
			accessor = ILPropertyGetSetter(property);
			if(!accessor)
			{
				PrintName(stream, type, member);
				if(xmlOutput)
				{
					fputs("\t\t\t<msg>", stream);
				}
				fputs("does not have a get or set accessor", stream);
				if(xmlOutput)
				{
					fputs("</msg>", stream);
				}
				putc('\n', stream);
				return 0;
			}
			isSet = 1;
			propertyType = ILMethod_Signature(accessor);
			propertyType = ILTypeGetParam
				(propertyType, ILTypeNumParams(propertyType));
		}

		/* Match the property signature */
		if(MatchPropertySignature(accessor, member, isSet))
		{
			break;
		}
	}
	if(!property)
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<missing/>\n", stream);
		}
		else
		{
			fputs("is missing\n", stream);
		}
		return 0;
	}

	/* Mark the property as seen so we don't pick
	   it up during the "extras" phase */
	ILMemberSetAttrs((ILMember *)property, 0x8000, 0x8000);

	/* Validate the property attributes */
	if((ILMethod_Attrs(accessor) & VALID_METHOD_FLAGS) !=
			(member->memberAttrs & VALID_METHOD_FLAGS))
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<msg>", stream);
		}
		PrintString("should have attributes `", stream);
		ILDumpFlags(stream, (member->memberAttrs & VALID_METHOD_FLAGS),
				    ILMethodDefinitionFlags, 0);
		PrintString("', but has `", stream);
		ILDumpFlags(stream, (ILMethod_Attrs(accessor) & VALID_METHOD_FLAGS),
				    ILMethodDefinitionFlags, 0);
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}

	/* Match the property type */
	if(!MatchType(propertyType, member->returnType, 0,
				  ILToProgramItem(ILProperty_Owner(property))))
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<msg>", stream);
		}
		PrintString("should have type `", stream);
		PrintString(member->returnType, stream);
		PrintString("', but has `", stream);
		PrintType(stream, propertyType, 0,
				  ILToProgramItem(ILProperty_Owner(property)));
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}

	/* Validate the property attributes */
	if(!ValidateAttributes(stream, type, member, member->attributes,
						   ILToProgramItem(property)))
	{
		valid = 0;
	}

	/* Return the validation result to the caller */
	return valid;
}

/*
 * Validate an event member against an IL image.
 * Returns zero if a validation error occurred.
 */
static int ValidateEvent(FILE *stream, ILImage *image,
					     ILClass *classInfo, ILDocType *type,
				         ILDocMember *member)
{
	int valid = 1;
	ILEvent *event;
	ILMethod *accessor;

	/* Locate the event in the class */
	event = 0;
	while((event = (ILEvent *)ILClassNextMemberByKind
				(classInfo, (ILMember *)event,
				 IL_META_MEMBERKIND_EVENT)) != 0)
	{
		if(!strcmp(ILEvent_Name(event), member->name))
		{
			break;
		}
	}
	if(!event)
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<missing/>\n", stream);
		}
		else
		{
			fputs("is missing\n", stream);
		}
		return 0;
	}

	/* Mark the event as seen so we don't pick
	   it up during the "extras" phase */
	ILMemberSetAttrs((ILMember *)event, 0x8000, 0x8000);

	/* Find an accessor to use to validate the attributes */
	accessor = ILEventGetAddOn(event);
	if(!accessor)
	{
		accessor = ILEventGetRemoveOn(event);
		if(!accessor)
		{
			accessor = ILEventGetFire(event);
		}
	}

	/* Validate the event attributes */
	if(accessor &&
	   (ILMethod_Attrs(accessor) & VALID_EVENT_FLAGS) !=
			(member->memberAttrs & VALID_EVENT_FLAGS))
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<msg>", stream);
		}
		PrintString("should have attributes `", stream);
		ILDumpFlags(stream, (member->memberAttrs & VALID_EVENT_FLAGS),
				    ILMethodDefinitionFlags, 0);
		PrintString("', but has `", stream);
		ILDumpFlags(stream, (ILMethod_Attrs(accessor) & VALID_EVENT_FLAGS),
				    ILMethodDefinitionFlags, 0);
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}

#if 0
	/* Can't do this yet because ECMA doesn't specify the type in a tag */
	/* Match the event type */
	if(!MatchType(ILEvent_Type(event), member->returnType))
	{
		PrintName(stream, type, member);
		if(xmlOutput)
		{
			fputs("\t\t\t<msg>", stream);
		}
		PrintString("should have type `", stream);
		PrintString(member->returnType, stream);
		PrintString("', but has `", stream);
		PrintType(stream, ILEvent_Type(event));
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}
#endif

	/* Validate the event attributes */
	if(!ValidateAttributes(stream, type, member, member->attributes,
						   ILToProgramItem(event)))
	{
		valid = 0;
	}

	/* Return the validation result to the caller */
	return valid;
}

/*
 * Validate a documentation member against an IL image.
 * Returns zero if a validation error occurred.
 */
static int ValidateMember(FILE *stream, ILImage *image, ILClass *classInfo,
						  ILDocType *type, ILDocMember *member)
{
	int valid = 1;

	/* Determine how to validate the member */
	switch(member->memberType)
	{
		case ILDocMemberType_Constructor:
		{
			valid = ValidateConstructor(stream, image, classInfo,
										type, member);
		}
		break;

		case ILDocMemberType_Method:
		{
			valid = ValidateMethod(stream, image, classInfo,
								   type, member);
		}
		break;

		case ILDocMemberType_Field:
		{
			valid = ValidateField(stream, image, classInfo,
								  type, member);
		}
		break;

		case ILDocMemberType_Property:
		{
			valid = ValidateProperty(stream, image, classInfo,
								     type, member);
		}
		break;

		case ILDocMemberType_Event:
		{
			valid = ValidateEvent(stream, image, classInfo,
								  type, member);
		}
		break;

		case ILDocMemberType_Unknown:
		{
			fprintf(stream, "%s has an unknown type in the XML file\n",
					type->fullName);
			valid = 0;
		}
		break;
	}

	/* Terminate the XML tag for a member if necessary */
	PrintEndMember(stream, type, member);

	/* Done */
	return valid;
}

/*
 * Determine if an image member is visible outside the assembly.
 */
static int MemberIsVisible(ILMember *member)
{
	ILUInt32 attrs;
	ILMethod *method;

	if(ILMember_IsField(member))
	{
		attrs = ILMember_Attrs(member);
	}
	else if(ILMember_IsMethod(member))
	{
		if(!strncmp(ILMember_Name(member), "get_", 4) ||
		   !strncmp(ILMember_Name(member), "set_", 4) ||
		   !strncmp(ILMember_Name(member), "add_", 4) ||
		   !strncmp(ILMember_Name(member), "remove_", 7))
		{
			if(ILMethod_HasSpecialName((ILMethod *)member))
			{
				/* Ignore property and event accessors */
				return 0;
			}
		}
		attrs = ILMember_Attrs(member);
	}
	else if(ILMember_IsProperty(member))
	{
		method = ILPropertyGetGetter((ILProperty *)member);
		if(!method)
		{
			method = ILPropertyGetSetter((ILProperty *)member);
		}
		if(method)
		{
			attrs = ILMethod_Attrs(method);
		}
		else
		{
			attrs = 0;
		}
	}
	else if(ILMember_IsEvent(member))
	{
		method = ILEventGetAddOn((ILEvent *)member);
		if(!method)
		{
			method = ILEventGetRemoveOn((ILEvent *)member);
			if(!method)
			{
				method = ILEventGetFire((ILEvent *)member);
			}
		}
		if(method)
		{
			attrs = ILMethod_Attrs(method);
		}
		else
		{
			attrs = 0;
		}
	}
	else
	{
		return 0;
	}

	switch(attrs & IL_META_METHODDEF_MEMBER_ACCESS_MASK)
	{
		case IL_META_METHODDEF_PUBLIC:
		case IL_META_METHODDEF_FAMILY:
		case IL_META_METHODDEF_FAM_OR_ASSEM:
		{
			return 1;
		}
		/* Not reached */
	}
	return 0;
}

/*
 * Report that a type is of one category when it should be of another.
 */
static void ReportTypeMismatch(FILE *stream, ILDocType *type,
							   const char *isA, const char *shouldBeA)
{
	PrintName(stream, type, 0);
	if(xmlOutput)
	{
		fputs("\t\t<msg>", stream);
	}
	fprintf(stream, "is a %s, but should be a %s", isA, shouldBeA);
	if(xmlOutput)
	{
		fputs("</msg>", stream);
	}
	putc('\n', stream);
}

/*
 * Validate a documentation type against an IL image.
 * Returns zero if a validation error occurred.
 */
static int ValidateType(FILE *stream, ILContext *context, ILDocType *type)
{
	ILImage *image;
	ILClass *classInfo;
	ILClass *tempClass;
	ILClass *parent;
	int valid = 1;
	char *fullName;
	ILDocInterface *interface;
	ILImplements *impl;
	int extrasOK = ILDocFlagSet("extra-members-ok");
	int implemented;
	ILDocMember *member;

	/* Find the type and the image in which is resides */
	classInfo = ResolveClass(context, type->name, type->namespace->name);
	if(!classInfo)
	{
		/* Report that the class is missing */
		PrintName(stream, type, 0);
		if(xmlOutput)
		{
			fputs("\t\t<missing/>\n", stream);
		}
		else
		{
			fputs("is missing\n", stream);
		}

		/* Report that all members are also missing */
		member = type->members;
		while(member != 0)
		{
			PrintName(stream, type, member);
			if(xmlOutput)
			{
				fputs("\t\t\t<missing/>\n", stream);
			}
			else
			{
				fputs("is missing\n", stream);
			}
			PrintEndMember(stream, type, member);
			member = member->next;
		}

		/* Bail out */
		return 0;
	}

	/* Get the image for the class */
	image = ILClassToImage(classInfo);

	/* Validate the type kind */
	switch(type->kind)
	{
		case ILDocTypeKind_Class:
		{
			if(ILClass_IsInterface(classInfo))
			{
				ReportTypeMismatch(stream, type, "interface", "class");
				valid = 0;
			}
			else if(ILClassIsValueType(classInfo))
			{
				if(ILTypeGetEnumType(ILType_FromValueType(classInfo)) !=
						ILType_FromValueType(classInfo))
				{
					ReportTypeMismatch(stream, type, "enum", "class");
					valid = 0;
				}
				else
				{
					ReportTypeMismatch(stream, type, "struct", "class");
					valid = 0;
				}
			}
			else if(IsDelegateType(classInfo))
			{
				ReportTypeMismatch(stream, type, "delegate", "class");
				valid = 0;
			}
		}
		break;

		case ILDocTypeKind_Interface:
		{
			if(ILClass_IsInterface(classInfo))
			{
				break;
			}
			if(ILClassIsValueType(classInfo))
			{
				if(ILTypeGetEnumType(ILType_FromValueType(classInfo)) !=
						ILType_FromValueType(classInfo))
				{
					ReportTypeMismatch(stream, type, "enum", "interface");
					valid = 0;
				}
				else
				{
					ReportTypeMismatch(stream, type, "struct", "interface");
					valid = 0;
				}
			}
			else if(IsDelegateType(classInfo))
			{
				ReportTypeMismatch(stream, type, "delegate", "interface");
				valid = 0;
			}
			else
			{
				ReportTypeMismatch(stream, type, "class", "interface");
				valid = 0;
			}
		}
		break;

		case ILDocTypeKind_Struct:
		{
			if(ILClass_IsInterface(classInfo))
			{
				ReportTypeMismatch(stream, type, "interface", "struct");
				valid = 0;
			}
			else if(ILClassIsValueType(classInfo))
			{
				if(ILTypeGetEnumType(ILType_FromValueType(classInfo)) !=
						ILType_FromValueType(classInfo))
				{
					ReportTypeMismatch(stream, type, "enum", "struct");
					valid = 0;
				}
			}
			else if(IsDelegateType(classInfo))
			{
				ReportTypeMismatch(stream, type, "delegate", "struct");
				valid = 0;
			}
			else
			{
				ReportTypeMismatch(stream, type, "class", "struct");
				valid = 0;
			}
		}
		break;

		case ILDocTypeKind_Enum:
		{
			if(ILClass_IsInterface(classInfo))
			{
				ReportTypeMismatch(stream, type, "interface", "enum");
				valid = 0;
			}
			else if(ILClassIsValueType(classInfo))
			{
				if(ILTypeGetEnumType(ILType_FromValueType(classInfo)) ==
						ILType_FromValueType(classInfo))
				{
					ReportTypeMismatch(stream, type, "struct", "enum");
					valid = 0;
				}
			}
			else if(IsDelegateType(classInfo))
			{
				ReportTypeMismatch(stream, type, "delegate", "enum");
				valid = 0;
			}
			else
			{
				ReportTypeMismatch(stream, type, "class", "enum");
				valid = 0;
			}
		}
		break;

		case ILDocTypeKind_Delegate:
		{
			if(ILClass_IsInterface(classInfo))
			{
				ReportTypeMismatch(stream, type, "interface", "delegate");
				valid = 0;
			}
			else if(ILClassIsValueType(classInfo))
			{
				if(ILTypeGetEnumType(ILType_FromValueType(classInfo)) !=
						ILType_FromValueType(classInfo))
				{
					ReportTypeMismatch(stream, type, "enum", "delegate");
					valid = 0;
				}
				else
				{
					ReportTypeMismatch(stream, type, "struct", "delegate");
					valid = 0;
				}
			}
			else if(!IsDelegateType(classInfo))
			{
				ReportTypeMismatch(stream, type, "class", "delegate");
				valid = 0;
			}
		}
		break;
	}

	/* Validate the type attributes */
	if(!ValidateAttributes(stream, type, 0, type->attributes,
						   ILToProgramItem(classInfo)))
	{
		valid = 0;
	}

	/* Validate the type flags */
	if(type->typeAttrs != ILDocInvalidAttrs &&
	   (ILClass_Attrs(classInfo) & VALID_TYPE_FLAGS) !=
			(type->typeAttrs & VALID_TYPE_FLAGS))
	{
		PrintName(stream, type, 0);
		if(xmlOutput)
		{
			fputs("\t\t<msg>", stream);
		}
		PrintString("should have attributes `", stream);
		ILDumpFlags(stream, (type->typeAttrs & VALID_TYPE_FLAGS),
				    ILTypeDefinitionFlags, 0);
		PrintString("', but has `", stream);
		ILDumpFlags(stream, (ILClass_Attrs(classInfo) & VALID_TYPE_FLAGS),
				    ILTypeDefinitionFlags, 0);
		PrintString("' instead", stream);
		if(xmlOutput)
		{
			fputs("</msg>", stream);
		}
		putc('\n', stream);
		valid = 0;
	}

	/* Validate the base type */
	parent = ILClass_ParentClass(classInfo);
	if(!parent)
	{
		if(type->baseType)
		{
			PrintName(stream, type, 0);
			if(xmlOutput)
			{
				fputs("\t\t<msg>", stream);
			}
			fputs("should have base type ", stream);
			PrintString(type->baseType, stream);
			fputs(", but has no base", stream);
			if(xmlOutput)
			{
				fputs("</msg>", stream);
			}
			putc('\n', stream);
			valid = 0;
		}
	}
	else
	{
		fullName = GetFullClassName(parent);
		if(!(type->baseType))
		{
			PrintName(stream, type, 0);
			if(xmlOutput)
			{
				fputs("\t\t<msg>", stream);
			}
			fputs("should have no base type, but has base ", stream);
			PrintString(fullName, stream);
			if(xmlOutput)
			{
				fputs("</msg>", stream);
			}
			putc('\n', stream);
			valid = 0;
		}
		else if(strcmp(type->baseType, fullName) != 0)
		{
			/* There is a bug in the ECMA "All.xml" file that says
			   that delegates inherit from "Delegate", when other
			   ECMA specs say "MulticastDelegate".  The other specs
			   are the correct ones */
			if(strcmp(type->baseType, "System.Delegate") != 0 ||
			   strcmp(fullName, "System.MulticastDelegate") != 0)
			{
				/* Don't report an error if the base type is excluded */
				if(!(type->excludedBaseType) ||
				   strcmp(type->excludedBaseType, fullName) != 0)
				{
					PrintName(stream, type, 0);
					if(xmlOutput)
					{
						fputs("\t\t<msg>", stream);
					}
					fputs("should have base type ", stream);
					PrintString(type->baseType, stream);
					fputs(" but has base ", stream);
					PrintString(fullName, stream);
					fputs(" instead", stream);
					if(xmlOutput)
					{
						fputs("</msg>", stream);
					}
					putc('\n', stream);
					valid = 0;
				}
			}
		}
		ILFree(fullName);
	}

	/* Validate the interfaces */
	interface = type->interfaces;
	while(interface != 0)
	{
		tempClass = classInfo;
		implemented = 0;
		while(tempClass != 0 && !implemented)
		{
			impl = 0;
			while((impl = ILClassNextImplements(tempClass, impl)) != 0)
			{
				parent = ILImplements_InterfaceClass(impl);
				fullName = GetFullClassName(parent);
				if(!strcmp(fullName, interface->name))
				{
					implemented = 1;
					ILFree(fullName);
					break;
				}
				ILFree(fullName);
			}
			tempClass = ILClass_ParentClass(tempClass);
		}
		if(!implemented)
		{
			PrintName(stream, type, 0);
			if(xmlOutput)
			{
				fputs("\t\t<msg>", stream);
			}
			fputs("should implement ", stream);
			PrintString(interface->name, stream);
			fputs(", but does not", stream);
			if(xmlOutput)
			{
				fputs("</msg>", stream);
			}
			putc('\n', stream);
			valid = 0;
		}
		interface = interface->next;
	}
	if(!extrasOK)
	{
		impl = 0;
		fullName = 0;
		while((impl = ILClassNextImplements(classInfo, impl)) != 0)
		{
			parent = ILImplements_InterfaceClass(impl);
			if(ILClass_IsPrivate(parent))
			{
				/* It is OK to implement a private interface */
				continue;
			}
			fullName = GetFullClassName(parent);
			interface = type->interfaces;
			while(interface != 0)
			{
				if(!strcmp(fullName, interface->name))
				{
					break;
				}
				interface = interface->next;
			}
			if(!interface)
			{
				break;
			}
			ILFree(fullName);
			fullName = 0;
		}
		if(fullName)
		{
			PrintName(stream, type, 0);
			if(xmlOutput)
			{
				fputs("\t\t<msg>", stream);
			}
			fputs("implements ", stream);
			PrintString(fullName, stream);
			fputs(", but should not", stream);
			if(xmlOutput)
			{
				fputs("</msg>", stream);
			}
			putc('\n', stream);
			ILFree(fullName);
			valid = 0;
		}
	}

	/* Mark all members as not yet seen if "extra-members-ok" is not set */
	if(!extrasOK)
	{
		ILMember *classMember = 0;
		while((classMember = ILClassNextMember(classInfo, classMember)) != 0)
		{
			ILMemberSetAttrs(classMember, 0x8000, 0);
		}
	}

	/* Validate the type members */
	member = type->members;
	while(member != 0)
	{
		if(!ValidateMember(stream, image, classInfo, type, member))
		{
			valid = 0;
		}
		member = member->next;
	}

	/* Report on all members not yet seen if "extra-members-ok" is not set */
	if(!extrasOK)
	{
		ILMember *classMember = 0;
		while((classMember = ILClassNextMember(classInfo, classMember)) != 0)
		{
			if((ILMember_Attrs(classMember) & 0x8000) == 0 &&
			   MemberIsVisible(classMember))
			{
				if(PrintILName(stream, type, classMember))
				{
					if(xmlOutput)
					{
						fputs("\t\t\t<extra/>\n", stream);
					}
					else
					{
						fputs("is not documented\n", stream);
					}
					PrintILEndMember(stream, type, classMember);
					valid = 0;
				}
			}
		}
	}

	/* Done */
	return valid;
}

/*
 * Determine if an extra type is known to be a non-standard extra.
 */
static int IsNonStandardExtra(ILClass *classInfo)
{
	ILAttribute *attr = 0;
	ILMethod *method;
	while((attr = ILProgramItemNextAttribute
				(ILToProgramItem(classInfo), attr)) != 0)
	{
		method = ILProgramItemToMethod(ILAttributeTypeAsItem(attr));
		if(method && !strcmp(ILClass_Name(ILMethod_Owner(method)),
							 "NonStandardExtraAttribute"))
		{
			return 1;
		}
	}
	return 0;
}

/*
 * Determine if we have an assembly match between a type
 * and an image.
 */
#define	AssemblyMatch(name1,name2)	\
			(ignoreAssemblyNames || \
			 (!(name1) || !(name2) || !ILStrICmp((name1), (name2))))

/*
 * Scan an image to check for global public types that are defined
 * in the image, but not within the documentation.  Returns the number
 * of extra types that were found.
 */
static int CheckForExtraTypes(FILE *stream, ILImage *image,
							  ILDocTree *tree, const char *assemName,
							  const char *progname)
{
	ILClass *classInfo;
	int numExtras = 0;
	char *fullName;
	ILDocType *type;

	/* Scan the TypeDef table for global public types */
	classInfo = 0;
	while((classInfo = (ILClass *)ILImageNextToken
				(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
	{
		if(ILClass_IsPublic(classInfo) && !ILClass_NestedParent(classInfo))
		{
			/* Build the full class name */
			fullName = GetFullClassName(classInfo);

			/* Look for a type in the documentation */
			type = ILDocTypeFind(tree, fullName);
			if(!type)
			{
				/* This is an extra type */
				if(!IsNonStandardExtra(classInfo))
				{
					++numExtras;
					if(xmlOutput)
					{
						fputs("\t<class name=\"", stream);
						PrintString(fullName, stream);
						fputs("\" assembly=\"", stream);
						PrintString(ILImageGetAssemblyName
							(ILProgramItem_Image(classInfo)), stream);
						fputs("\">\n", stream);
						fprintf(stream, "\t\t<extra/>\n");
						fprintf(stream, "\t</class>\n");
					}
					else
					{
						fprintf(stream, "%s is not documented\n", fullName);
					}
				}
			}
			else if(!AssemblyMatch(type->assembly, assemName))
			{
				if(xmlOutput)
				{
					fputs("\t<class name=\"", stream);
					PrintString(fullName, stream);
					fputs("\" assembly=\"", stream);
					PrintString(ILImageGetAssemblyName
						(ILProgramItem_Image(classInfo)), stream);
					fputs("\">\n", stream);
					fprintf(stream, "\t\t<msg>\n");
					fprintf(stream, "%s should be in the assembly %s, "
								"but was instead found in the assembly %s\n",
							type->fullName, type->assembly, assemName);
					fprintf(stream, "</msg>\n");
					fprintf(stream, "\t</class>\n");
				}
				else
				{
					fprintf(stream, "%s should be in the assembly %s, "
								"but was instead found in the assembly %s\n",
							type->fullName, type->assembly, assemName);
				}
				++numExtras;
			}
			ILFree(fullName);
		}
	}

	/* Return the number of extra types to the caller */
	return numExtras;
}

int ILDocConvert(ILDocTree *tree, int numInputs, char **inputs,
				 char *outputPath, const char *progname)
{
	const char *imageFilename;
	ILContext *context;
	ILImage *image;
	ILDocNamespace *namespace;
	ILDocType *type;
	FILE *stream;
	int closeStream;
	int numTypes;
	int numValidated;
	int numExtraTypes;
	const char *assemName;
	int assemNameLen;
	const char *mapName;
	int imageNum;

	/* Set various global flags */
	xmlOutput = ILDocFlagSet("xml");
	ignoreAssemblyNames = ILDocFlagSet("ignore-assembly-names");

	/* Load the IL images to be validated */
	imageFilename = ILDocFlagValue("image");
	if(!imageFilename)
	{
		fprintf(stderr, "%s: `-fimage=PATH' must be specified\n", progname);
		return 0;
	}
	imageNum = 0;
	context = ILContextCreate();
	if(!context)
	{
		ILDocOutOfMemory(progname);
	}
	while((imageFilename = ILDocFlagValueN("image", imageNum)) != 0)
	{
		if(ILImageLoadFromFile(imageFilename, context, &image,
							   IL_LOADFLAG_FORCE_32BIT, 1) != 0)
		{
			return 0;
		}
		++imageNum;
	}
	if(imageNum != 1 && !ignoreAssemblyNames)
	{
		fprintf(stderr, "%s: `-fignore-assembly-names' must be used with multiple `-fimage=PATH' options\n", progname);
		return 0;
	}

	/* Open the output stream */
	if(!strcmp(outputPath, "-"))
	{
		stream = stdout;
		closeStream = 0;
	}
	else if((stream = fopen(outputPath, "w")) == NULL)
	{
		perror(outputPath);
		ILContextDestroy(context);
		return 0;
	}
	else
	{
		closeStream = 1;
	}

	/* Get the name of the image's assembly */
	assemName = (imageNum == 1 ? ILImageGetAssemblyName(image) : 0);
	mapName = ILDocFlagValue("assembly-map");
	if(assemName && mapName)
	{
		assemNameLen = strlen(assemName);
		if(!strncmp(assemName, mapName, assemNameLen) &&
		   mapName[assemNameLen] == ',')
		{
			assemName = mapName + assemNameLen + 1;
		}
	}

	/* Output the XML header */
	if(xmlOutput)
	{
		fputs("<?xml version=\"1.0\"?>\n", stream);
		fputs("<class_status>\n", stream);
	}

	/* Process every type in the documentation tree */
	numTypes = 0;
	numValidated = 0;
	namespace = tree->namespaces;
	while(namespace != 0)
	{
		type = namespace->types;
		while(type != 0)
		{
			if(AssemblyMatch(type->assembly, assemName))
			{
				++numTypes;
				if(ValidateType(stream, context, type))
				{
					++numValidated;
				}
				PrintEndMember(stream, type, 0);
			}
			type = type->nextNamespace;
		}
		namespace = namespace->next;
	}

	/* Process public types that are in the image, but not the documentation */
	if(!ILDocFlagSet("extra-types-ok"))
	{
		image = 0;
		numExtraTypes = 0;
		while((image = ILContextNextImage(context, image)) != 0)
		{
			assemName = ILImageGetAssemblyName(image);
			numExtraTypes += CheckForExtraTypes(stream, image, tree,
											    assemName, progname);
		}
	}
	else
	{
		numExtraTypes = 0;
	}

	/* Print a summary of how many types were validated */
	if(xmlOutput)
	{
		fprintf(stream,
		        "\t<summary types=\"%d\" validated=\"%d\" extra=\"%d\"/>\n",
				numTypes, numValidated, numExtraTypes);
	}
	else
	{
		if(numExtraTypes != 0)
		{
			fprintf(stream, "\n%d types in document, %d were validated, "
							"%d undocumented types in image\n",
					numTypes, numValidated, numExtraTypes);
		}
		else
		{
			fprintf(stream, "\n%d types in document, %d were validated\n",
					numTypes, numValidated);
		}
	}

	/* Output the XML footer */
	if(xmlOutput)
	{
		fputs("</class_status>\n", stream);
	}

	/* Clean up and exit */
	if(closeStream)
	{
		fclose(stream);
	}
	ILContextDestroy(context);
	return 1;
}

#ifdef	__cplusplus
};
#endif
