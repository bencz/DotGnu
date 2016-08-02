/*
 * c_declspec.c - Declaration specifiers for the C programming language.
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

#include <cscc/c/c_internal.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * List of valid storage class specifier combinations and names.
 */
typedef struct
{
	int			specifiers;
	int			applicableKinds;
	const char *name;

} StorageClassInfo;
static StorageClassInfo const storageClasses[] = {
	{C_SPEC_TYPEDEF,
		C_KIND_GLOBAL_NAME | C_KIND_LOCAL_NAME,
		"typedef"},
	{C_SPEC_EXTERN,
		C_KIND_GLOBAL_NAME | C_KIND_LOCAL_NAME | C_KIND_FUNCTION,
		"extern"},
	{C_SPEC_THREAD_SPECIFIC | C_SPEC_STATIC,
		C_KIND_GLOBAL_NAME | C_KIND_LOCAL_NAME,
		"static __declspec(thread)"},
	{C_SPEC_THREAD_SPECIFIC,
		C_KIND_GLOBAL_NAME,
		"__declspec(thread)"},
	{C_SPEC_STATIC | C_SPEC_INLINE,
		C_KIND_FUNCTION,
		"static inline"},
	{C_SPEC_STATIC,
		C_KIND_GLOBAL_NAME | C_KIND_LOCAL_NAME | C_KIND_FUNCTION,
		"static"},
	{C_SPEC_AUTO,
		C_KIND_LOCAL_NAME | C_KIND_PARAMETER_NAME,
		"auto"},
	{C_SPEC_REGISTER,
		C_KIND_LOCAL_NAME | C_KIND_PARAMETER_NAME,
		"register"},
	{C_SPEC_INLINE,
		C_KIND_FUNCTION,
		"inline"},
};
#define	numStorageClasses	(sizeof(storageClasses) / sizeof(StorageClassInfo))

const char *CStorageClassToName(int specifier)
{
	int index;
	for(index = 0; index < numStorageClasses; ++index)
	{
		if(specifier == storageClasses[index].specifiers)
		{
			return storageClasses[index].name;
		}
	}
	return 0;
}

CDeclSpec CDeclSpecEmpty(void)
{
	CDeclSpec result;
	result.specifiers = 0;
	result.dupSpecifiers = 0;
	result.baseType = 0;
	return result;
}

CDeclSpec CDeclSpecCombine(CDeclSpec spec1, CDeclSpec spec2)
{
	CDeclSpec result;
	int okSpecifiers;

	/* Swap the values so that the base type is in "spec1" */
	if(!(spec1.baseType) && spec2.baseType)
	{
		result = spec1;
		spec1 = spec2;
		spec2 = result;
	}

	/* Copy any duplicates that we got on the previous call */
	result.dupSpecifiers = spec1.dupSpecifiers | spec2.dupSpecifiers;

	/* Combine the storage classes and common type specifiers */
	result.specifiers =
			((spec1.specifiers | spec2.specifiers) &
					(C_SPEC_STORAGE | C_SPEC_TYPE_COMMON));

	/* Detect duplicates in the storage classes and common type specifiers */
	result.dupSpecifiers |=
			(spec1.specifiers & spec2.specifiers &
			 (C_SPEC_STORAGE | C_SPEC_TYPE_COMMON));

	/* If both specifiers have a base type, then record an error for later */
	if(spec1.baseType && spec2.baseType)
	{
		result.dupSpecifiers |= C_SPEC_MULTIPLE_BASES;
	}

	/* If both are "long", then change one into "long long" */
	if((spec1.specifiers & C_SPEC_LONG) != 0 &&
	   (spec2.specifiers & C_SPEC_LONG) != 0)
	{
		spec2.specifiers =
			(spec2.specifiers & ~C_SPEC_LONG) | C_SPEC_LONG_LONG;
	}

	/* Apply type specifiers to the base type */
	result.baseType = spec1.baseType;
	if(spec1.baseType != 0)
	{
		okSpecifiers = 0;
		if(ILType_IsPrimitive(spec1.baseType))
		{
			switch(ILType_ToElement(spec1.baseType))
			{
				case IL_META_ELEMTYPE_I1:
				{
					/* Look for "unsigned" and "signed" */
					if(((spec1.specifiers | spec2.specifiers)
							& C_SPEC_UNSIGNED) != 0)
					{
						result.baseType = ILType_UInt8;
						okSpecifiers = C_SPEC_UNSIGNED;
					}
					else if(((spec1.specifiers | spec2.specifiers)
								& C_SPEC_SIGNED) != 0)
					{
						result.baseType = ILType_Int8;
						okSpecifiers = C_SPEC_SIGNED;
					}
				}
				break;

				case IL_META_ELEMTYPE_U1:
				{
					/* Look for "unsigned" */
					if(((spec1.specifiers | spec2.specifiers)
							& C_SPEC_UNSIGNED) != 0)
					{
						result.baseType = ILType_UInt8;
						okSpecifiers = C_SPEC_UNSIGNED;
					}
				}
				break;

				case IL_META_ELEMTYPE_I2:
				{
					/* Look for "unsigned" and "signed" */
					if(((spec1.specifiers | spec2.specifiers)
							& C_SPEC_UNSIGNED) != 0)
					{
						result.baseType = ILType_UInt16;
						okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_SHORT;
					}
					else if(((spec1.specifiers | spec2.specifiers)
								& C_SPEC_SIGNED) != 0)
					{
						result.baseType = ILType_Int16;
						okSpecifiers = C_SPEC_SIGNED | C_SPEC_SHORT;
					}
					else
					{
						okSpecifiers = C_SPEC_SHORT;
					}
				}
				break;

				case IL_META_ELEMTYPE_U2:
				{
					/* Look for "unsigned" */
					if(((spec1.specifiers | spec2.specifiers)
							& C_SPEC_UNSIGNED) != 0)
					{
						result.baseType = ILType_UInt16;
						okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_SHORT;
					}
				}
				break;

				case IL_META_ELEMTYPE_I4:
				{
					/* Look for all legal modifier combinations */
					switch((spec1.specifiers | spec2.specifiers)
							& (C_SPEC_SIGNED | C_SPEC_UNSIGNED |
							   C_SPEC_SHORT | C_SPEC_LONG |
							   C_SPEC_LONG_LONG))
					{
						case C_SPEC_SIGNED:
						{
							result.baseType = ILType_Int32;
							okSpecifiers = C_SPEC_SIGNED;
						}
						break;

						case C_SPEC_UNSIGNED:
						{
							result.baseType = ILType_UInt32;
							okSpecifiers = C_SPEC_UNSIGNED;
						}
						break;

						case C_SPEC_SHORT:
						case C_SPEC_SIGNED | C_SPEC_SHORT:
						{
							result.baseType = ILType_Int16;
							okSpecifiers = C_SPEC_SIGNED | C_SPEC_SHORT;
						}
						break;

						case C_SPEC_UNSIGNED | C_SPEC_SHORT:
						{
							result.baseType = ILType_UInt16;
							okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_SHORT;
						}
						break;

						case C_SPEC_LONG:
						case C_SPEC_SIGNED | C_SPEC_LONG:
						{
							result.baseType = ILType_Int;
							okSpecifiers = C_SPEC_SIGNED | C_SPEC_LONG;
						}
						break;

						case C_SPEC_UNSIGNED | C_SPEC_LONG:
						{
							result.baseType = ILType_UInt;
							okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_LONG;
						}
						break;

						case C_SPEC_SIGNED | C_SPEC_LONG_LONG:
						case C_SPEC_SIGNED | C_SPEC_LONG | C_SPEC_LONG_LONG:
						case C_SPEC_LONG_LONG:
						case C_SPEC_LONG | C_SPEC_LONG_LONG:
						{
							result.baseType = ILType_Int64;
							okSpecifiers = C_SPEC_SIGNED | C_SPEC_LONG |
										   C_SPEC_LONG_LONG;
						}
						break;

						case C_SPEC_UNSIGNED | C_SPEC_LONG_LONG:
						case C_SPEC_UNSIGNED | C_SPEC_LONG | C_SPEC_LONG_LONG:
						{
							result.baseType = ILType_UInt64;
							okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_LONG |
										   C_SPEC_LONG_LONG;
						}
						break;
					}
				}
				break;

				case IL_META_ELEMTYPE_U4:
				{
					/* Look for "short", "long", or "long long" */
					switch((spec1.specifiers | spec2.specifiers)
							& (C_SPEC_SHORT | C_SPEC_LONG | C_SPEC_LONG_LONG))
					{
						case C_SPEC_SHORT:
						{
							result.baseType = ILType_UInt16;
							okSpecifiers = C_SPEC_SHORT | C_SPEC_UNSIGNED;
						}
						break;

						case C_SPEC_LONG:
						{
							result.baseType = ILType_UInt;
							okSpecifiers = C_SPEC_LONG | C_SPEC_UNSIGNED;
						}
						break;

						case C_SPEC_LONG_LONG:
						case C_SPEC_LONG | C_SPEC_LONG_LONG:
						{
							result.baseType = ILType_UInt64;
							okSpecifiers = C_SPEC_LONG | C_SPEC_LONG_LONG |
										   C_SPEC_UNSIGNED;
						}
						break;

						default:
						{
							okSpecifiers = C_SPEC_UNSIGNED;
						}
						break;
					}
				}
				break;

				case IL_META_ELEMTYPE_I8:
				{
					/* Look for "signed" and "unsigned" */
					if(((spec1.specifiers | spec2.specifiers)
							& C_SPEC_UNSIGNED) != 0)
					{
						result.baseType = ILType_UInt64;
						okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_LONG |
									   C_SPEC_LONG_LONG;
					}
					else if(((spec1.specifiers | spec2.specifiers)
								& C_SPEC_SIGNED) != 0)
					{
						result.baseType = ILType_Int64;
						okSpecifiers = C_SPEC_SIGNED | C_SPEC_LONG |
									   C_SPEC_LONG_LONG;
					}
					else
					{
						okSpecifiers = C_SPEC_LONG | C_SPEC_LONG_LONG;
					}
				}
				break;

				case IL_META_ELEMTYPE_U8:
				{
					/* Look for "unsigned" */
					if(((spec1.specifiers | spec2.specifiers)
							& C_SPEC_UNSIGNED) != 0)
					{
						result.baseType = ILType_UInt64;
						okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_LONG |
									   C_SPEC_LONG_LONG;
					}
					else
					{
						okSpecifiers = C_SPEC_LONG | C_SPEC_LONG_LONG;
					}
				}
				break;

				case IL_META_ELEMTYPE_I:
				{
					/* Look for "long long", "signed", and "unsigned" */
					if(((spec1.specifiers | spec2.specifiers)
							& C_SPEC_LONG_LONG) != 0)
					{
						if(((spec1.specifiers | spec2.specifiers)
								& C_SPEC_UNSIGNED) != 0)
						{
							result.baseType = ILType_UInt64;
							okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_LONG |
										   C_SPEC_LONG_LONG;
						}
						else if(((spec1.specifiers | spec2.specifiers)
									& C_SPEC_SIGNED) != 0)
						{
							result.baseType = ILType_Int64;
							okSpecifiers = C_SPEC_SIGNED | C_SPEC_LONG |
										   C_SPEC_LONG_LONG;
						}
						else
						{
							result.baseType = ILType_Int64;
							okSpecifiers = C_SPEC_LONG | C_SPEC_LONG_LONG;
						}
					}
					else if(((spec1.specifiers | spec2.specifiers)
								& C_SPEC_UNSIGNED) != 0)
					{
						result.baseType = ILType_UInt;
						okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_LONG;
					}
					else if(((spec1.specifiers | spec2.specifiers)
								& C_SPEC_SIGNED) != 0)
					{
						result.baseType = ILType_Int;
						okSpecifiers = C_SPEC_SIGNED | C_SPEC_LONG;
					}
					else
					{
						okSpecifiers = C_SPEC_LONG | C_SPEC_SIGNED |
									   C_SPEC_UNSIGNED;
					}
				}
				break;

				case IL_META_ELEMTYPE_U:
				{
					/* Look for "long long" and "unsigned" */
					if(((spec1.specifiers | spec2.specifiers)
							& C_SPEC_LONG_LONG) != 0)
					{
						if(((spec1.specifiers | spec2.specifiers)
									& C_SPEC_UNSIGNED) != 0)
						{
							result.baseType = ILType_UInt64;
							okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_LONG |
										   C_SPEC_LONG_LONG;
						}
						else
						{
							result.baseType = ILType_UInt64;
							okSpecifiers = C_SPEC_LONG | C_SPEC_UNSIGNED |
										   C_SPEC_LONG_LONG;
						}
					}
					else if(((spec1.specifiers | spec2.specifiers)
								& C_SPEC_UNSIGNED) != 0)
					{
						result.baseType = ILType_UInt;
						okSpecifiers = C_SPEC_UNSIGNED | C_SPEC_LONG;
					}
					else
					{
						okSpecifiers = C_SPEC_LONG | C_SPEC_UNSIGNED;
					}
				}
				break;

				case IL_META_ELEMTYPE_R4:
				{
					/* Look for "long" and "long long" */
					if(((spec1.specifiers | spec2.specifiers)
							& C_SPEC_LONG_LONG) != 0)
					{
						/* "long long float" == "long double" */
						result.baseType = ILType_Float64;
						okSpecifiers = C_SPEC_LONG | C_SPEC_LONG_LONG;
					}
					else if(((spec1.specifiers | spec2.specifiers)
									& C_SPEC_LONG) != 0)
					{
						/* "long float" == "double" */
						result.baseType = ILType_Float64;
						okSpecifiers = C_SPEC_LONG;
					}
				}
				break;

				case IL_META_ELEMTYPE_R8:
				{
					/* Look for "long" and "long long" */
					if(((spec1.specifiers | spec2.specifiers)
							& (C_SPEC_LONG | C_SPEC_LONG_LONG)) != 0)
					{
						result.baseType = ILType_Float64;
						okSpecifiers = C_SPEC_LONG | C_SPEC_LONG_LONG;
					}
				}
				break;

				case IL_META_ELEMTYPE_R:
				{
					/* Look for "long" and "long long" */
					if(((spec1.specifiers | spec2.specifiers)
							& (C_SPEC_LONG | C_SPEC_LONG_LONG)) != 0)
					{
						/* "long long double" == "long double" */
						result.baseType = ILType_Float64;
						okSpecifiers = C_SPEC_LONG | C_SPEC_LONG_LONG;
					}
				}
				break;
			}
		}
		result.specifiers |=
			okSpecifiers & (spec1.specifiers | spec2.specifiers);
		if(((spec1.specifiers | spec2.specifiers) &
				(C_SPEC_TYPE_CHANGE & ~okSpecifiers)) != 0)
		{
			result.dupSpecifiers |= C_SPEC_INVALID_COMBO;
		}
	}
	else
	{
		/* Copy the type change specifiers, but don't inspect them yet */
		result.specifiers |= ((spec1.specifiers | spec2.specifiers) &
							  C_SPEC_TYPE_CHANGE);
	}

	/* Check for "signed" and "unsigned" or "short" and "long" together */
	if(((spec1.specifiers | spec2.specifiers) &
			(C_SPEC_SIGNED | C_SPEC_UNSIGNED)) ==
					(C_SPEC_SIGNED | C_SPEC_UNSIGNED))
	{
		result.dupSpecifiers |= C_SPEC_SIGN_AND_UNSIGN;
	}
	if(((spec1.specifiers | spec2.specifiers) &
			(C_SPEC_SHORT | C_SPEC_LONG)) == (C_SPEC_SHORT | C_SPEC_LONG))
	{
		result.dupSpecifiers |= C_SPEC_LONG_AND_SHORT;
	}

	/* Check for duplicate "signed", "unsigned", and "short" but don't
	   worry about "long" as we max out the sizes above */
	result.dupSpecifiers |= (spec1.specifiers & spec2.specifiers &
						(C_SPEC_SIGNED | C_SPEC_UNSIGNED | C_SPEC_SHORT));

	/* Done */
	return result;
}

/*
 * Report a warning for a duplicate specifier.
 */
static void ReportDuplicate(ILNode *node, int specifiers,
						    int flag, const char *specName)
{
	if((specifiers & flag) != 0)
	{
		if(node)
		{
			CCWarningOnLine(yygetfilename(node), yygetlinenum(node),
							_("duplicate `%s'"), specName);
		}
		else
		{
			CCWarning(_("duplicate `%s'"), specName);
		}
	}
}

/*
 * Report an error, when we may not have a node or name to
 * report against.
 */
static void ReportError(ILNode *node, const char *name,
						const char *msg1, const char *msg2)
{
	if(node)
	{
		if(name)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node), msg1, name);
		}
		else
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node), msg2);
		}
	}
	else
	{
		if(name)
		{
			CCError(msg1, name);
		}
		else
		{
			CCError(msg2);
		}
	}
}

/*
 * Report an warning, when we may not have a node or name to
 * report against.
 */
static void ReportWarning(ILNode *node, const char *name,
						  const char *msg1, const char *msg2)
{
	if(node)
	{
		if(name)
		{
			CCWarningOnLine(yygetfilename(node), yygetlinenum(node),
							msg1, name);
		}
		else
		{
			CCWarningOnLine(yygetfilename(node), yygetlinenum(node), msg2);
		}
	}
	else
	{
		if(name)
		{
			CCWarning(msg1, name);
		}
		else
		{
			CCWarning(msg2);
		}
	}
}

CDeclSpec CDeclSpecFinalize(CDeclSpec spec, ILNode *node,
							const char *name, int kind)
{
	CDeclSpec result;
	int storageClass, test;
	int index;

	/* Validate the storage class specifiers */
	storageClass = (spec.specifiers & C_SPEC_STORAGE);
	result.specifiers = 0;
	result.dupSpecifiers = 0;
	for(index = 0; index < numStorageClasses; ++index)
	{
		test = storageClasses[index].specifiers;
		if((storageClass & test) == test)
		{
			if((storageClasses[index].applicableKinds & kind) != 0)
			{
				result.specifiers = (storageClass & test);
				if((storageClass & ~test) != 0)
				{
					ReportError(node, name,
						_("multiple storage classes in declaration of `%s'"),
						_("multiple storage classes in declaration"));
				}
			}
			else if(node)
			{
				if(name)
				{
					CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					   _("`%s' is not applicable to the declaration of `%s'"),
					   storageClasses[index].name, name);
				}
				else
				{
					CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					   			  _("`%s' is not applicable here"),
					   			  storageClasses[index].name);
				}
			}
			else
			{
				if(name)
				{
					CCError
					  (_("`%s' is not applicable to the declaration of `%s'"),
					   storageClasses[index].name, name);
				}
				else
				{
					CCError(_("`%s' is not applicable here"),
					   		 storageClasses[index].name);
				}
			}
			break;
		}
	}

	/* Report duplicate storage classes and type specifiers */
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_TYPEDEF, "typedef");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_EXTERN, "extern");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_STATIC, "static");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_AUTO, "auto");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_REGISTER, "register");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_INLINE, "inline");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_CONST, "const");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_VOLATILE, "volatile");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_SIGNED, "signed");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_UNSIGNED, "unsigned");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_SHORT, "short");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_THREAD_SPECIFIC,
					"__declspec(thread)");
	ReportDuplicate(node, spec.dupSpecifiers, C_SPEC_BOX, "__box");

	/* Copy the common type qualifiers to "result.specifiers" */
	result.specifiers |= (spec.specifiers & C_SPEC_TYPE_COMMON);

	/* If we don't have a base type yet, then infer the most obvious one */
	if(spec.baseType == 0)
	{
		if((spec.specifiers & C_SPEC_SHORT) != 0)
		{
			if((spec.specifiers & C_SPEC_UNSIGNED) != 0)
			{
				result.baseType = ILType_UInt16;
			}
			else
			{
				result.baseType = ILType_Int16;
			}
		}
		else if((spec.specifiers & C_SPEC_LONG_LONG) != 0)
		{
			if((spec.specifiers & C_SPEC_UNSIGNED) != 0)
			{
				result.baseType = ILType_UInt64;
			}
			else
			{
				result.baseType = ILType_Int64;
			}
		}
		else if((spec.specifiers & C_SPEC_LONG) != 0)
		{
			if((spec.specifiers & C_SPEC_UNSIGNED) != 0)
			{
				result.baseType = ILType_UInt;
			}
			else
			{
				result.baseType = ILType_Int;
			}
		}
		else if((spec.specifiers & C_SPEC_UNSIGNED) != 0)
		{
			result.baseType = ILType_UInt32;
		}
		else if((spec.specifiers & C_SPEC_SIGNED) != 0)
		{
			result.baseType = ILType_Int32;
		}
		else
		{
			ReportWarning(node, name,
					  	  _("type defaults to `int' in declaration of `%s'"),
					  	  _("type defaults to `int' in declaration"));
			result.baseType = ILType_Int32;
		}
	}
	else
	{
		result.baseType = spec.baseType;
	}

	/* If "__box" is present, then convert value types into reference types */
	if((spec.specifiers & C_SPEC_BOX) != 0)
	{
		if(ILTypeIsValue(result.baseType))
		{
			ILClass *classInfo = ILTypeToClass(&CCCodeGen, result.baseType);
			if(!classInfo)
			{
				CCOutOfMemory();
			}
			result.baseType = ILType_FromClass(classInfo);
		}
		else
		{
			ReportError(node, name,
						_("cannot use `__box' in declaration of `%s'"),
						_("cannot use `__box' in declaration"));
		}
	}

	/* Print pending errors that were detected by "CDeclSpecCombine" */
	if((spec.dupSpecifiers & C_SPEC_MULTIPLE_BASES) != 0)
	{
		ReportError(node, name,
					_("two or more data types in declaration of `%s'"),
					_("two or more data types in declaration"));
	}
	if((spec.dupSpecifiers & C_SPEC_SIGN_AND_UNSIGN) != 0)
	{
		ReportError(node, name,
					_("both signed and unsigned specified for `%s'"),
					_("both signed and unsigned specified"));
	}
	if((spec.dupSpecifiers & C_SPEC_LONG_AND_SHORT) != 0)
	{
		ReportError(node, name,
					_("both long and short specified for `%s'"),
					_("both long and short specified"));
	}
	if((spec.dupSpecifiers & (C_SPEC_LONG_AND_SHORT |
							  C_SPEC_SIGN_AND_UNSIGN |
							  C_SPEC_INVALID_COMBO))
			== C_SPEC_INVALID_COMBO)
	{
		ReportError(node, name,
			_("long, short, signed, or unsigned invalid for `%s'"),
			_("long, short, signed, or unsigned invalid here"));
	}

	/* Return the result to the caller */
	return result;
}

CDeclarator CDeclCreateOpenArray(ILGenInfo *info, CDeclarator elem,
								 int gcSpecifier)
{
	CDeclarator result;
	ILType *type;
	ILType **hole;

	/* Create a C#-style array reference, which we will turn into a
	   C-style array type when "CDeclFinalize" is called.  For now,
	   this is just a place-holder */
	type = ILTypeCreateArray(info->context, 1, ILType_Invalid);
	if(!type)
	{
		ILGenOutOfMemory(info);
	}
	ILTypeSetSize(type, 0, (long)(-1L));
	hole = &(ILType_ElemType(type));

	/* Mark the type with the GC specifier if necessary */
	if(gcSpecifier == ILGC_Managed)
	{
		*hole = CTypeAddManaged(info, *hole);
		hole = &((*hole)->un.modifier__.type__);
	}
	else if(gcSpecifier == ILGC_Unmanaged)
	{
		*hole = CTypeAddUnmanaged(info, *hole);
		hole = &((*hole)->un.modifier__.type__);
	}

	/* Insert "type" into the hole in "elem" to create "result" */
	result.name = elem.name;
	result.node = elem.node;
	if(elem.typeHole != 0)
	{
		result.type = elem.type;
		*hole = *(elem.typeHole);
		if(*(elem.typeHole) == ILType_Invalid)
		{
			result.typeHole = hole;
		}
		else
		{
			result.typeHole = elem.typeHole;
		}
		*(elem.typeHole) = type;
	}
	else
	{
		result.type = type;
		result.typeHole = hole;
	}
	result.isKR = 0;
	result.params = elem.params;
	result.attrs = elem.attrs;
	return result;
}

CDeclarator CDeclCreateArray(ILGenInfo *info, CDeclarator elem,
							 ILNode *size, int gcSpecifier)
{
	CDeclarator result;
	ILType *type;
	ILType **hole;

	/* Create a C#-style array reference, which we will turn into a
	   C-style array type when "CDeclFinalize" is called.  For now,
	   this is just a place-holder */
	type = ILTypeCreateArray(info->context, 1, ILType_Invalid);
	if(!type)
	{
		ILGenOutOfMemory(info);
	}
	ILTypeSetSize(type, 0, (long)(ILNativeInt)size);
	hole = &(ILType_ElemType(type));

	/* Mark the type with the GC specifier if necessary */
	if(gcSpecifier == ILGC_Managed)
	{
		*hole = CTypeAddManaged(info, *hole);
		hole = &((*hole)->un.modifier__.type__);
	}
	else if(gcSpecifier == ILGC_Unmanaged)
	{
		*hole = CTypeAddUnmanaged(info, *hole);
		hole = &((*hole)->un.modifier__.type__);
	}

	/* Insert "type" into the hole in "elem" to create "result" */
	result.name = elem.name;
	result.node = elem.node;
	if(elem.typeHole != 0)
	{
		result.type = elem.type;
		*hole = *(elem.typeHole);
		if(*(elem.typeHole) == ILType_Invalid)
		{
			result.typeHole = hole;
		}
		else
		{
			result.typeHole = elem.typeHole;
		}
		*(elem.typeHole) = type;
	}
	else
	{
		result.type = type;
		result.typeHole = hole;
	}
	result.isKR = 0;
	result.params = elem.params;
	result.attrs = elem.attrs;
	return result;
}

CDeclarator CDeclCreatePointer(ILGenInfo *info, int qualifiers,
							   CDeclarator *refType)
{
	CDeclarator result;
	ILType *type;

	/* Create the actual pointer type */
	if(refType)
	{
		type = CTypeCreatePointer(info, refType->type);
		result.typeHole = refType->typeHole;
	}
	else
	{
		type = CTypeCreatePointer(info, ILType_Invalid);
		result.typeHole = &(ILType_Ref(type));
	}

	/* Wrap the pointer type in the "const" and "volatile" qualifiers */
	if((qualifiers & C_SPEC_CONST) != 0)
	{
		type = CTypeAddConst(info, type);
	}
	if((qualifiers & C_SPEC_VOLATILE) != 0)
	{
		type = CTypeAddVolatile(info, type);
	}

	/* Construct the return value (the type hole was set above) */
	result.name = 0;
	result.node = 0;
	result.type = type;
	result.isKR = 0;
	if(refType)
	{
		result.params = refType->params;
		result.attrs = refType->attrs;
	}
	else
	{
		result.params = 0;
		result.attrs = 0;
	}
	return result;
}

CDeclarator CDeclCreateByRef(ILGenInfo *info, int qualifiers,
							 CDeclarator *refType)
{
	CDeclarator result;
	ILType *type;

	/* Create the actual reference type */
	if(refType)
	{
		type = CTypeCreateByRef(info, refType->type);
		result.typeHole = refType->typeHole;
	}
	else
	{
		type = CTypeCreateByRef(info, ILType_Invalid);
		result.typeHole = &(ILType_Ref(type));
	}

	/* Wrap the reference type in the "const" and "volatile" qualifiers */
	if((qualifiers & C_SPEC_CONST) != 0)
	{
		type = CTypeAddConst(info, type);
	}
	if((qualifiers & C_SPEC_VOLATILE) != 0)
	{
		type = CTypeAddVolatile(info, type);
	}

	/* Construct the return value (the type hole was set above) */
	result.name = 0;
	result.node = 0;
	result.type = type;
	result.isKR = 0;
	if(refType)
	{
		result.params = refType->params;
		result.attrs = refType->attrs;
	}
	else
	{
		result.params = 0;
		result.attrs = 0;
	}
	return result;
}

/*
 * Print an error that warns about parameter redeclaration.
 */
static void ParamRedeclared(ILNode *newName, ILNode *oldName)
{
	CCErrorOnLine(yygetfilename(newName), yygetlinenum(newName),
				  _("redeclaration of `%s'"),
				  ILQualIdentName(newName, 0));
	if(oldName)
	{
		CCErrorOnLine(yygetfilename(oldName), yygetlinenum(oldName),
					  _("`%s' previously declared here"),
					  ILQualIdentName(newName, 0));
	}
}

/*
 * Turn a parameter list into a function signature.  "declaredParams"
 * is the list of parameters that were declared using old-style K&R
 * conventions, or NULL if no such list.  "method" is the method block
 * for a global function header, or NULL if building a pointer type.
 */
static ILType *ParamsToSignature(ILGenInfo *info, ILNode *params,
								 ILNode *declaredParams, ILMethod *method,
								 ILType ***hole)
{
	ILType *signature;
	ILNode_ListIter iter;
	ILNode_ListIter iter2;
	ILNode_FormalParameter *formal;
	ILNode_FormalParameter *formal2;
	ILNode_FormalParameter *formal3;
	ILNode *name;
	ILType *type;

	/* Create the bare function signature and find the return type hole */
	signature = ILTypeCreateMethod(info->context, ILType_Invalid);
	if(!signature)
	{
		ILGenOutOfMemory(info);
	}
	*hole = &(signature->un.method__.retType__);

	/* Create a default declared parameter list */
	if(!declaredParams)
	{
		declaredParams = ILNode_List_create();
	}

	/* Add the formal parameters to the signature */
	if(params)
	{
		ILNode_ListIter_Init(&iter, params);
		while((formal = (ILNode_FormalParameter *)
					ILNode_ListIter_Next(&iter)) != 0)
		{
			/* Process special arguments */
			if(formal->pmod == ILParamMod_arglist)
			{
				/* The signature includes a "..." argument */
				ILTypeSetCallConv(signature, IL_META_CALLCONV_VARARG);
				continue;
			}
			else if(formal->type && ILTypeIdentical
					 (ILType_Void, ((ILNode_MarkType *)(formal->type))->type))
			{
				/* Skip "void" arguments, which exist to distinguish
				   ANSI C functions with no arguments from K&R prototypes
				   that may have undeclared arguments */
				if(formal->name)
				{
					CCErrorOnLine(yygetfilename(formal->name),
								  yygetlinenum(formal->name),
								  _("parameter `%s' has incomplete type"),
								  ILQualIdentName(formal->name, 0));
				}
				continue;
			}

			/* Get the name and type information from the formal parmameter */
			name = formal->name;
			if(formal->type)
			{
				type = ((ILNode_MarkType *)(formal->type))->type;
			}
			else
			{
				type = ILType_Invalid;
			}

			/* If we have a name, then look for a declared type */
			if(name)
			{
				ILNode_ListIter_Init(&iter2, declaredParams);
				formal3 = 0;
				while((formal2 = (ILNode_FormalParameter *)
							ILNode_ListIter_Next(&iter2)) != 0)
				{
					if(ILQualIdentName(name, 0) ==
					   ILQualIdentName(formal2->name, 0))
					{
						if(formal3 != 0)
						{
							/* Two or more K&R declarations for the name */
							formal2->pmod = ILParamMod_ref;
						}
						else
						{
							formal3 = formal2;
						}
					}
				}
				formal2 = formal3;
				if(formal2 != 0)
				{
					if(formal2->pmod == ILParamMod_out)
					{
						/* We've processed this parameter already */
						ParamRedeclared(name, formal2->name);
					}
					else if(type == ILType_Invalid)
					{
						/* Retrieve the type from the declared params list */
						type = ((ILNode_MarkType *)(formal2->type))->type;

						/* Mark the declared parameter as already seen */
						formal2->pmod = ILParamMod_out;
					}
					else
					{
						/* Given a type in both ANSI-style and K&R-style */
						ParamRedeclared(name, formal2->name);
					}
				}
			}
			else
			{
				formal2 = 0;
			}

			/* If we don't have a type yet, then assume "int" */
			if(type == ILType_Invalid)
			{
				type = ILType_Int32;
			}

			/* Add an entry to "declaredParams" to allow us to detect
			   multiple declarations of the same parameter in ANSI mode */
			if(name != 0 && formal2 == 0)
			{
				ILNode_List_Add(declaredParams,
					ILNode_FormalParameter_create
						(0, ILParamMod_out,
						 ILNode_MarkType_create(0, type), name));
			}

			/* Decay array types to their pointer forms */
			type = CTypeDecay(info, type);

			/* Pass complex types by pointer, not by value */
			if(CTypeIsComplex(type))
			{
				type = CTypeCreateComplexPointer(info, type);
			}

			/* Add the new parameter to the function signature */
			if(!ILTypeAddParam(info->context, signature, type))
			{
				ILGenOutOfMemory(info);
			}

			/* Add a parameter information block to the method for the name */
			if(name && method)
			{
				if(!ILParameterCreate(method, 0, ILQualIdentName(name, 0), 0,
									  (ILUInt32)ILTypeNumParams(signature)))
				{
					ILGenOutOfMemory(info);
				}
			}
		}
	}

	/* Check that all declared parameters have been used */
	ILNode_ListIter_Init(&iter, declaredParams);
	while((formal = (ILNode_FormalParameter *)
				ILNode_ListIter_Next(&iter)) != 0)
	{
		if(formal->pmod == ILParamMod_empty)
		{
			CCErrorOnLine
				(yygetfilename(formal->name), yygetlinenum(formal->name),
				_("declaration for parameter `%s', but no such parameter"),
				ILQualIdentName(formal->name, 0));
		}
		else if(formal->pmod == ILParamMod_ref)
		{
			ParamRedeclared(formal->name, 0);
		}
	}

	/* The signature is ready to go */
	return signature;
}

CDeclarator CDeclCreatePrototype(ILGenInfo *info, CDeclarator decl,
								 ILNode *params, ILNode *attributes)
{
	ILType *signature;
	ILType **returnHole;

	/* Bail out if we already have parameters in the declarator
	   and we aren't declaring a function pointer return value */
	if(decl.params)
	{
		if(!ILType_IsPointer(decl.type) || !(decl.typeHole))
		{
			CCError(_("cannot declare a function returning a function"));
			return decl;
		}
	}

	/* If the declarator has a type hole, then create a signature
	   inside it, and replace the hole with the return type */
	if(decl.typeHole && !(decl.params))
	{
		signature = ParamsToSignature(info, params, 0, 0, &returnHole);
		*(decl.typeHole) = signature;
		decl.typeHole = returnHole;
		return decl;
	}

	/* Deal with functions that return function pointers */
	if(decl.params)
	{
		signature = ParamsToSignature(info, params, 0, 0, &returnHole);
		*returnHole = decl.type;
		decl.type = signature;
		return decl;
	}

	/* Modify the declarator and return it */
	if(params)
	{
		decl.params = params;
		decl.isKR = 0;
	}
	else
	{
		decl.params = ILNode_List_create();
		decl.isKR = 1;
	}
	decl.attrs = attributes;
	return decl;
}

CDeclarator CDeclCombine(CDeclarator decl1, CDeclarator decl2)
{
	CDeclarator result;
	result.name = decl2.name;
	result.node = decl2.node;
	if(decl2.typeHole)
	{
		result.type = decl2.type;
		*(decl2.typeHole) = decl1.type;
		result.typeHole = decl1.typeHole;
	}
	else
	{
		result.type = decl1.type;
		result.typeHole = decl1.typeHole;
	}
	result.isKR = 0;
	result.params = decl2.params;
	result.attrs = decl2.attrs;
	return result;
}

/*
 * Replace references to C#-style array types in a type declarator tree.
 */
static ILType *ReplaceArrayTypes(ILGenInfo *info, ILType *type)
{
	CTypeLayoutInfo layout;
	ILType *elemType;
	char *name;

	if(type != 0 && ILType_IsComplex(type))
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_PTR:
			{
				/* If the pointed-to type is a function, then remove the
				   pointer reference and replace with "IsFunctionPointer" */
				if(ILType_IsMethod(ILType_Ref(type)))
				{
					type = ReplaceArrayTypes(info, ILType_Ref(type));
					type = CTypeAddFunctionPtr(info, type);
				}
				else if(ILType_IsClass(ILType_Ref(type)))
				{
					/* Pointing to a class reference.  Remove the pointer.
					   This turns types like "String *" into "String" for
					   compatibility with Managed C++ */
					type = ILType_Ref(type);
				}
				else
				{
					ILType_Ref(type) =
						ReplaceArrayTypes(info, ILType_Ref(type));
				}
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY:
			{
				/* Check for "__gc" and "__nogc" modifiers */
				if(CTypeIsManaged(ILType_ElemType(type)))
				{
					/* Turn an unmanaged array into its managed counterpart */
					elemType = ReplaceArrayTypes
						(info, CTypeStripGC(ILType_ElemType(type)));
					if(ILTypeIsReference(elemType) || ILTypeIsValue(elemType))
					{
						goto managed;
					} 
					else
					{
						name = CTypeToName(info, elemType);
						CCError(_("__gc used with unmanaged type `%s'"), name);
						ILFree(name);
					}
				}
				else if(CTypeIsUnmanaged(ILType_ElemType(type)))
				{
					/* We expect the element type to be unmanaged */
					elemType = ReplaceArrayTypes
						(info, CTypeStripGC(ILType_ElemType(type)));
					if(ILTypeIsReference(elemType))
					{
						name = CTypeToName(info, elemType);
						CCError(_("__nogc used with managed type `%s'"), name);
						ILFree(name);
						goto managed;
					}
				}
				else
				{
					/* Determine the array type from the element type */
					elemType = ReplaceArrayTypes(info, ILType_ElemType(type));
					if(ILTypeIsReference(elemType))
					{
					managed:
						if(ILType_Size(type) != (long)(-1))
						{
							CCError(_("cannot specify a size for managed "
									  "array types"));
						}
						ILTypeSetSize(type, 0, 0);
						ILType_ElemType(type) = elemType;
						break;
					}
				}

				/* Report an error if the type has unknown layout */
				CTypeGetLayoutInfo(elemType, &layout);
				if(layout.category == C_TYPECAT_NO_LAYOUT)
				{
					name = CTypeToName(info, elemType);
					CCError(_("storage size of `%s' is not known"), name);
					ILFree(name);
					elemType = ILType_Int32;
				}

				/* Construct a new C-style array type */
				if(ILType_Size(type) == (long)(-1L))
				{
					type = CTypeCreateOpenArray(info, elemType);
				}
				else
				{
					type = CTypeCreateArrayNode
						(info, elemType,
						 (ILNode *)(ILNativeInt)ILType_Size(type));
					if(!type)
					{
						CCError(_("size of array is too large"));
						type = CTypeCreateArray(info, elemType, 1);
					}
				}
			}
			break;

			case IL_TYPE_COMPLEX_CMOD_OPT:
			case IL_TYPE_COMPLEX_CMOD_REQD:
			{
				/* Replace array types in the modifier's argument */
				type->un.modifier__.type__ =
					ReplaceArrayTypes(info, type->un.modifier__.type__);
			}
			break;

			case IL_TYPE_COMPLEX_METHOD:
			case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
			{
				/* Replace array types in the function's return type,
				   and then decay the return type */
				type->un.method__.retType__ =
					CTypeDecay(info, ReplaceArrayTypes
						(info, type->un.method__.retType__));
				if(CTypeIsComplex(type->un.method__.retType__))
				{
					type->un.method__.retType__ =
						CTypeCreateComplexPointer
							(info, type->un.method__.retType__);
				}
			}
			break;

			default: break;
		}
	}
	return type;
}

ILType *CDeclFinalize(ILGenInfo *info, CDeclSpec spec, CDeclarator decl,
					  ILNode *declaredParams, ILMethod *method)
{
	ILType *type;
	ILType *signature;
	ILType **returnHole;

	/* Get the base type from "spec".  The default type is assumed to
	   be "int", for functions that don't have a return type specified */
	if(spec.baseType != ILType_Invalid)
	{
		type = spec.baseType;
	}
	else
	{
		type = ILType_Int32;
	}

	/* Add the "const" and "volatile" qualifiers from "spec" */
	if((spec.specifiers & C_SPEC_CONST) != 0)
	{
		type = CTypeAddConst(info, type);
	}
	if((spec.specifiers & C_SPEC_VOLATILE) != 0)
	{
		type = CTypeAddVolatile(info, type);
	}

	/* Insert the base type into the type hole in "decl" */
	if(decl.typeHole)
	{
		*(decl.typeHole) = type;
		type = decl.type;
	}

	/* Replace C#-style array types with C-style array types */
	type = ReplaceArrayTypes(info, type);

	/* If we have a method declaration, then build a function prototype */
	if(method && !(decl.params) && CTypeIsFunction(type))
	{
		/* We probably got the type by stripping it from another
		   function using "typeof" */
		ILMemberSetSignature((ILMember *)method, type);
		ILMethodSetCallConv(method, ILType_CallConv(type));
	}
	else if(method)
	{
		if(!(decl.params))
		{
			CCError(_("missing parameters for function declaration"));
		}
		signature = ParamsToSignature(info, decl.params, declaredParams,
									  method, &returnHole);
		*returnHole = CTypeDecay(info, type);
		if(CTypeIsComplex(*returnHole))
		{
			*returnHole = CTypeCreateComplexPointer(info, *returnHole);
		}
		type = signature;
		ILMemberSetSignature((ILMember *)method, signature);
		ILMethodSetCallConv(method, ILType_CallConv(signature));
	}
	else if(decl.params)
	{
		/* Create a function signature pointer type */
		signature = ParamsToSignature(info, decl.params, 0, 0, &returnHole);
		*returnHole = CTypeDecay(info, type);
		if(CTypeIsComplex(*returnHole))
		{
			*returnHole = CTypeCreateComplexPointer(info, *returnHole);
		}
		type = signature;
	}

	/* Return the final type to the caller */
	return type;
}

#ifdef	__cplusplus
};
#endif
