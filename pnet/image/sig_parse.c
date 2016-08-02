/*
 * sig_parse.c - Signature parsing for IL image handling.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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

#include "image.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Maximum recursion depth for type parsing.
 */
#define	IL_TYPE_RECURSION_DEPTH		20

/*
 * Parse an array shape.
 */
static ILType *ParseArrayShape(ILContext *context,
							   ILMetaDataRead *reader,
							   ILType *elemType)
{
	ILUInt32 rank;
	ILUInt32 numSizes;
	ILUInt32 numLowBounds;
	ILUInt32 dim;
	ILInt32 value;
	ILType *array;

	/* Parse the rank value */
	rank = ILMetaUncompressData(reader);
	if(!rank)
	{
		/* Invalid rank value */
		return ILType_Invalid;
	}
	array = ILTypeCreateArray(context, rank, elemType);
	if(!array)
	{
		return ILType_Invalid;
	}

	/* Parse the number of specified sizes */
	numSizes = ILMetaUncompressData(reader);
	if(numSizes > rank)
	{
		/* Invalid sizes value */
		return ILType_Invalid;
	}

	/* Parse the size array */
	for(dim = 0; dim < numSizes; ++dim)
	{
		value = ILMetaUncompressInt(reader);
		if(value < 0)
		{
			return ILType_Invalid;
		}
		ILTypeSetSize(array, dim, value);
	}

	/* Parse the number of specified low bounds */
	numLowBounds = ILMetaUncompressData(reader);
	if(numLowBounds > rank)
	{
		/* Invalid low bounds value */
		return ILType_Invalid;
	}

	/* Parse the lower bounds array */
	for(dim = 0; dim < numLowBounds; ++dim)
	{
		value = ILMetaUncompressInt(reader);
		ILTypeSetLowBound(array, dim, value);
	}

	/* Done */
	return array;
}

/*
 * Type categories that are allowed in "ParseElemType".
 */
#define	CATEGORY_CMOD			1
#define	CATEGORY_VOID			2
#define	CATEGORY_BYREF			4
#define	CATEGORY_TYPEDBYREF		8
#define	CATEGORY_PINNED			16
#define	CATEGORY_ALL			(CATEGORY_CMOD | \
								 CATEGORY_VOID | \
								 CATEGORY_BYREF | \
								 CATEGORY_TYPEDBYREF)

/*
 * Signature kinds.
 */
#define	SIGNATURE_METHOD_DEF	0
#define	SIGNATURE_METHOD_REF	1
#define	SIGNATURE_FIELD			2
#define	SIGNATURE_PROPERTY		3

/*
 * Forward declaration.
 */
static ILType *ParseSignature(ILContext *context, ILImage *image,
							  ILMetaDataRead *reader, int *kind,
							  int depth);

int LoadForwardTypeDef(ILImage *image, ILToken token);
/*
 * Parse an element type.  "depth" is used to control the
 * recursion depth so that malicious parties cannot cause
 * this function to recurse indefinitely.
 */
static ILType *ParseElemType(ILContext *context, ILImage *image,
							 ILMetaDataRead *reader, int depth,
							 int categories)
{
	ILUInt32 value;
	ILType *modifiers = 0;
	ILType *type = 0;
	ILType *tempType;
	ILClass *info;
	int sigKind;

	/* Get the main type value */
	value = ILMetaUncompressData(reader);

	/* Parse compiler modifiers */
	while(value == IL_META_ELEMTYPE_CMOD_REQD ||
	      value == IL_META_ELEMTYPE_CMOD_OPT)
	{
		ILToken token;

		if((categories & CATEGORY_CMOD) == 0)
		{
			/* Compiler modifiers cannot be used here */
			return ILType_Invalid;
		}
		if(value == IL_META_ELEMTYPE_CMOD_REQD)
		{
			sigKind = IL_TYPE_COMPLEX_CMOD_REQD;
		}
		else
		{
			sigKind = IL_TYPE_COMPLEX_CMOD_OPT;
		}
		token = ILMetaUncompressToken(reader);
		info = (ILClass *)ILImageTokenInfo(image, token);
		if(!info)
		{
			/* Not a valid type reference */
			return ILType_Invalid;
		}
		modifiers = ILTypeCreateModifier(context, modifiers, sigKind, info);
		value = ILMetaUncompressData(reader);
	}

	/* Determine how to parse the rest of the type */
	switch(value)
	{
		case IL_META_ELEMTYPE_VOID:
		{
			/* The VOID type can only be used on return and pointer types */
			if((categories & CATEGORY_VOID) != 0)
			{
				type = ILType_Void;
			}
		}
		break;

		case IL_META_ELEMTYPE_BOOLEAN:
		case IL_META_ELEMTYPE_CHAR:
		case IL_META_ELEMTYPE_I1:
		case IL_META_ELEMTYPE_U1:
		case IL_META_ELEMTYPE_I2:
		case IL_META_ELEMTYPE_U2:
		case IL_META_ELEMTYPE_I4:
		case IL_META_ELEMTYPE_U4:
		case IL_META_ELEMTYPE_I8:
		case IL_META_ELEMTYPE_U8:
		case IL_META_ELEMTYPE_R4:
		case IL_META_ELEMTYPE_R8:
		case IL_META_ELEMTYPE_I:
		case IL_META_ELEMTYPE_U:
		case IL_META_ELEMTYPE_R:
		{
			type = ILType_FromElement(value);
		}
		break;

		case IL_META_ELEMTYPE_STRING:
		{
			/* Return a reference to the built-in "System.String" class */
			info = ILClassResolveSystem(image, 0, "String", "System");
			if(info)
			{
				type = ILType_FromClass(info);
			}
		}
		break;

		case IL_META_ELEMTYPE_PTR:
		{
			/* Parse a pointer type */
			if(depth > 0)
			{
				type = ParseElemType(context, image, reader, depth - 1,
									 CATEGORY_CMOD | CATEGORY_VOID);
				if(type != ILType_Invalid)
				{
					type = ILTypeCreateRef(context, IL_TYPE_COMPLEX_PTR, type);
				}
			}
		}
		break;

		case IL_META_ELEMTYPE_BYREF:
		{
			/* The BYREF flag can only be used on parameter and return types */
			if(depth > 0 && (categories & CATEGORY_BYREF) != 0)
			{
				type = ParseElemType(context, image, reader, depth - 1,
									 categories & ~(CATEGORY_BYREF));
				if(type != ILType_Invalid)
				{
					type = ILTypeCreateRef(context, IL_TYPE_COMPLEX_BYREF,
										   type);
				}
			}
		}
		break;

		case IL_META_ELEMTYPE_VALUETYPE:
		{
			ILToken token;

			/* Parse a type token reference as a value type */
			token = ILMetaUncompressToken(reader);
			if(token != 0)
			{
				info = (ILClass *)ILImageTokenInfo(image, token);
				if(info != 0)
				{
					type = ILType_FromValueType(info);
				}
			}
		}
		break;

		case IL_META_ELEMTYPE_CLASS:
		{
			ILToken token;

			/* Parse a type token reference as a class */
			token = ILMetaUncompressToken(reader);
			if(token != 0)
			{
				info = (ILClass *)ILImageTokenInfo(image, token);
				if(info == 0)
				{
					int error;
					/* 
					 * When this class is referenced as a actual generic parameter for a base class,
					 * and it is not already loaded we can get to this point.
					 */
					error = LoadForwardTypeDef(image, token);
					if(error != 0)
					{
						return 0;
					}
					info = (ILClass *)ILImageTokenInfo(image, token);
				}
				if(info != 0)
				{
					type = ILType_FromClass(info);
				}
			}
		}
		break;

		case IL_META_ELEMTYPE_ARRAY:
		{
			/* Parse an array type */
			if(depth > 0)
			{
				type = ParseElemType(context, image, reader, depth - 1, 0);
				if(type != ILType_Invalid)
				{
					type = ParseArrayShape(context, reader, type);
				}
			}
		}
		break;

		case IL_META_ELEMTYPE_SZARRAY:
		{
			/* Parse a simple 1-dimensional array with no size values */
			if(depth > 0)
			{
				type = ParseElemType(context, image, reader,
									 depth - 1, CATEGORY_CMOD);
				if(type != ILType_Invalid)
				{
					type = ILTypeCreateArray(context, 1, type);
				}
			}
		}
		break;

		case IL_META_ELEMTYPE_TYPEDBYREF:
		{
			/* TYPEDBYREF can only be used in parameters and return types */
			if((categories & CATEGORY_TYPEDBYREF) != 0)
			{
				type = ILType_TypedRef;
			}
		}
		break;

		case IL_META_ELEMTYPE_FNPTR:
		{
			/* Parse a method signature */
			if(depth > 0)
			{
				type = ParseSignature(context, image, reader, &sigKind,
									  depth - 1);
				if(type != ILType_Invalid)
				{
					if(sigKind != SIGNATURE_METHOD_DEF &&
					   sigKind != SIGNATURE_METHOD_REF)
					{
						type = ILType_Invalid;
					}
				}
			}
		}
		break;

		case IL_META_ELEMTYPE_OBJECT:
		{
			/* Return a reference to the built-in "System.Object" class */
			info = ILClassResolveSystem(image, 0, "Object", "System");
			if(info)
			{
				type = ILType_FromClass(info);
			}
		}
		break;

		case IL_META_ELEMTYPE_PINNED:
		{
			/* Sometimes pinned directives for local variables
			   are inside the type, instead of at the top level */
			if(depth > 0 && (categories & CATEGORY_PINNED) != 0)
			{
				type = ParseElemType(context, image, reader,
									 depth - 1, categories);
				if(type != ILType_Invalid)
				{
					type = ILTypeCreateRef
								(context, IL_TYPE_COMPLEX_PINNED, type);
				}
			}
		}
		break;

		case IL_META_ELEMTYPE_WITH:
		{
			/* Parse a generic type with parameters */
			if(depth > 0)
			{
				/* Parse the main "with" type */
				tempType = ParseElemType(context, image, reader,
										 depth - 1, categories);
				if(tempType == ILType_Invalid)
				{
					return 0;
				}
				type = ILTypeCreateWith(context, tempType);
				if(!type)
				{
					return 0;
				}

				/* Get the number of parameters */
				if(!(reader->error) && reader->len > 0)
				{
					value = *((reader->data)++);
					--(reader->len);
				}
				else
				{
					reader->error = 1;
					return 0;
				}

				/* Parse the type parameters */
				while(value > 0)
				{
					tempType = ParseElemType(context, image, reader,
											 depth - 1, categories);
					if(tempType == ILType_Invalid)
					{
						return 0;
					}
					if(!ILTypeAddWithParam(context, type, tempType))
					{
						return 0;
					}
					--value;
				}
			}
		}
		break;

		case IL_META_ELEMTYPE_MVAR:
		{
			/* Parse a generic method variable reference */
			if(!(reader->error) && reader->len > 0)
			{
				value = *((reader->data)++);
				--(reader->len);
				type = ILTypeCreateVarNum
					(context, IL_TYPE_COMPLEX_MVAR, (int)value);
			}
			else
			{
				reader->error = 1;
			}
		}
		break;

		case IL_META_ELEMTYPE_VAR:
		{
			/* Parse a generic class variable reference */
			if(!(reader->error) && reader->len > 0)
			{
				value = *((reader->data)++);
				--(reader->len);
				type = ILTypeCreateVarNum
					(context, IL_TYPE_COMPLEX_VAR, (int)value);
			}
			else
			{
				reader->error = 1;
			}
		}
		break;

		default:	break;
	}

	/* Add the compiler modifiers to the type */
	if(type && modifiers)
	{
		type = ILTypeAddModifiers(context, modifiers, type);
	}

	/* Done */
	return type;
}

/*
 * Parse a signature blob.  "kind" returns the kind of signature.
 */
static ILType *ParseSignature(ILContext *context, ILImage *image,
							  ILMetaDataRead *reader, int *kind,
							  int depth)
{
	ILUInt32 sigKind;
	ILUInt32 numGenericParams;
	ILUInt32 numParams;
	ILUInt32 param;
	int sawSentinel;
	ILType *paramType;
	ILType *type;

	/* Get the signature kind and calling conventions */
	sigKind = ILMetaUncompressData(reader);

	/* Determine what kind of signature we are dealing with */
	switch(sigKind & IL_META_CALLCONV_MASK)
	{
		case IL_META_CALLCONV_DEFAULT:
		case IL_META_CALLCONV_C:
		case IL_META_CALLCONV_STDCALL:
		case IL_META_CALLCONV_THISCALL:
		case IL_META_CALLCONV_FASTCALL:
		case IL_META_CALLCONV_VARARG:
		case IL_META_CALLCONV_UNMGD:
		case IL_META_CALLCONV_INSTANTIATION:
		{
			/* Parse the number of generic parameters */
			if((sigKind & IL_META_CALLCONV_GENERIC) != 0)
			{
				numGenericParams = ILMetaUncompressData(reader);
				if(numGenericParams > (ILUInt32)0xFF)
				{
					/* Cannot have this many parameters, because "with"
					   types use a single byte for parameter counts */
					break;
				}
			}
			else
			{
				numGenericParams = 0;
			}

			/* Parse the number of method parameters */
			numParams = ILMetaUncompressData(reader);
			if(numParams > (ILUInt32)0xFFFF)
			{
				/* Cannot have this many parameters, because the parameter
				   numbers in ParamDef records don't support it */
				break;
			}

			/* Bail out if the recursion depth has expired */
			if(depth <= 0)
			{
				break;
			}

			/* Parse the return type */
			if((sigKind & IL_META_CALLCONV_MASK) !=
					IL_META_CALLCONV_INSTANTIATION)
			{
				type = ParseElemType(context, image, reader, depth - 1,
									 CATEGORY_CMOD | CATEGORY_VOID |
									 CATEGORY_TYPEDBYREF | CATEGORY_BYREF);
				if(type == ILType_Invalid)
				{
					break;
				}
			}
			else
			{
				/* Instantiation signatures don't have return types */
				type = ILType_Invalid;
			}

			/* Construct the method type */
			type = ILTypeCreateMethod(context, type);
			if(type == ILType_Invalid)
			{
				break;
			}
			ILTypeSetCallConv(type, sigKind);
			ILType_SetNumGen(type, numGenericParams);

			/* Parse the parameters */
			param = 0;
			sawSentinel = 0;
			while(param < numParams)
			{
				if(reader->len >= 1 &&
				   reader->data[0] == IL_META_ELEMTYPE_SENTINEL)
				{
					/* This is the sentinel value for a varargs call site */
					if(sawSentinel || numParams > (unsigned long)0xFFFE)
					{
						return ILType_Invalid;
					}
					sawSentinel = 1;
					--(reader->len);
					++(reader->data);
					if(!ILTypeAddSentinel(context, type))
					{
						return ILType_Invalid;
					}
				}
				else
				{
					/* Parse the next parameter type */
					paramType = ParseElemType(context, image, reader, depth - 1,
										      CATEGORY_CMOD | CATEGORY_BYREF |
										      CATEGORY_TYPEDBYREF);
					if(paramType == ILType_Invalid)
					{
						return ILType_Invalid;
					}
					++param;
					if(!ILTypeAddParam(context, type, paramType))
					{
						return ILType_Invalid;
					}
				}
			}

			/* If we have a sentinel, then we must have VARARG or C */
			if(sawSentinel &&
			   ((type->kind__ >> 8) & IL_META_CALLCONV_MASK)
			   		!= IL_META_CALLCONV_VARARG &&
			   ((type->kind__ >> 8) & IL_META_CALLCONV_MASK)
			   		!= IL_META_CALLCONV_C)
			{
				break;
			}

			/* Return the final method type to the caller */
			*kind = (sawSentinel ? SIGNATURE_METHOD_REF : SIGNATURE_METHOD_DEF);
			return type;
		}
		/* Not reached */

		case IL_META_CALLCONV_FIELD:
		{
			/* Field signature */
			if(depth > 0)
			{
				type = ParseElemType(context, image, reader, depth - 1,
									 CATEGORY_ALL);
				if(type != ILType_Invalid)
				{
					*kind = SIGNATURE_FIELD;
					return type;
				}
			}
		}
		break;

		case IL_META_CALLCONV_PROPERTY:
		{
			/* Parse the number of getter parameters */
			numParams = ILMetaUncompressData(reader);
			if(numParams > (ILUInt32)0xFFFD)
			{
				/* Cannot have this many parameters, because the parameter
				   numbers in ParamDef records don't support it, and we
				   need some spares for "this" and the setter value */
				break;
			}

			/* Bail out if the recursion depth has expired */
			if(depth <= 0)
			{
				break;
			}

			/* Parse the property type */
			type = ParseElemType(context, image, reader, depth - 1, 0);
			if(type == ILType_Invalid)
			{
				break;
			}

			/* Construct the property type */
			type = ILTypeCreateProperty(context, type);
			if(type == ILType_Invalid)
			{
				break;
			}

			/* Parse the parameters */
			for(param = 0; param < numParams; ++param)
			{
				/* Parse the next parameter type */
				paramType = ParseElemType(context, image, reader, depth - 1,
									      CATEGORY_CMOD | CATEGORY_BYREF |
									      CATEGORY_TYPEDBYREF);
				if(paramType == ILType_Invalid)
				{
					return ILType_Invalid;
				}
				if(!ILTypeAddParam(context, type, paramType))
				{
					return ILType_Invalid;
				}
			}

			/* Return the final property type to the caller */
			*kind = SIGNATURE_PROPERTY;
			return type;
		}
		/* Not reached */

		default:	break;
	}

	/* If we get here, then an error occurred */
	return ILType_Invalid;
}

ILType *ILTypeFromMethodDefSig(ILContext *context, ILImage *image,
							   unsigned long blobStart, unsigned long blobLen)
{
	ILMetaDataRead reader;
	ILType *type;
	int kind;

	/* Initialize the metadata reader */
	reader.data = (unsigned char *)(image->blobPool + blobStart);
	reader.len = blobLen;
	reader.error = 0;

	/* Parse the signature */
	type = ParseSignature(context, image, &reader,
						  &kind, IL_TYPE_RECURSION_DEPTH);

	/* Return the value to the caller */
	if(!(reader.error) && type && kind == SIGNATURE_METHOD_DEF)
	{
		return type;
	}
	else
	{
		return ILType_Invalid;
	}
}

ILType *ILTypeFromMethodRefSig(ILContext *context, ILImage *image,
							   unsigned long blobStart, unsigned long blobLen)
{
	ILMetaDataRead reader;
	ILType *type;
	int kind;

	/* Initialize the metadata reader */
	reader.data = (unsigned char *)(image->blobPool + blobStart);
	reader.len = blobLen;
	reader.error = 0;

	/* Parse the signature */
	type = ParseSignature(context, image, &reader,
						  &kind, IL_TYPE_RECURSION_DEPTH);

	/* Return the value to the caller */
	if(!(reader.error) && type &&
	   (kind == SIGNATURE_METHOD_DEF || kind == SIGNATURE_METHOD_REF))
	{
		kind = ((type->kind__ >> 8) & IL_META_CALLCONV_MASK);
		if(kind == IL_META_CALLCONV_DEFAULT ||
		   kind == IL_META_CALLCONV_VARARG)
		{
			return type;
		}
	}
	return ILType_Invalid;
}

ILType *ILTypeFromStandAloneMethodSig(ILContext *context, ILImage *image,
							   		  unsigned long blobStart,
									  unsigned long blobLen)
{
	ILMetaDataRead reader;
	ILType *type;
	int kind;

	/* Initialize the metadata reader */
	reader.data = (unsigned char *)(image->blobPool + blobStart);
	reader.len = blobLen;
	reader.error = 0;

	/* Parse the signature */
	type = ParseSignature(context, image, &reader,
						  &kind, IL_TYPE_RECURSION_DEPTH);

	/* Return the value to the caller */
	if(!(reader.error) && type &&
	   (kind == SIGNATURE_METHOD_DEF || kind == SIGNATURE_METHOD_REF))
	{
		return type;
	}
	return ILType_Invalid;
}

ILType *ILTypeFromFieldSig(ILContext *context, ILImage *image,
						   unsigned long blobStart, unsigned long blobLen)
{
	ILMetaDataRead reader;
	ILUInt32 kind;

	/* Initialize the metadata reader */
	reader.data = (unsigned char *)(image->blobPool + blobStart);
	reader.len = blobLen;
	reader.error = 0;

	/* The signature must start with IL_META_CALLCONV_FIELD */
	kind = ILMetaUncompressData(&reader);
	if(kind != IL_META_CALLCONV_FIELD)
	{
		return ILType_Invalid;
	}

	/* Parse the field type */
	return ParseElemType(context, image, &reader,
						 IL_TYPE_RECURSION_DEPTH, CATEGORY_ALL);
}

ILType *ILTypeFromPropertySig(ILContext *context, ILImage *image,
							  unsigned long blobStart, unsigned long blobLen)
{
	ILMetaDataRead reader;
	ILType *type;
	int kind;

	/* Initialize the metadata reader */
	reader.data = (unsigned char *)(image->blobPool + blobStart);
	reader.len = blobLen;
	reader.error = 0;

	/* Parse the signature */
	type = ParseSignature(context, image, &reader,
						  &kind, IL_TYPE_RECURSION_DEPTH);

	/* Return the value to the caller */
	if(!(reader.error) && type && kind == SIGNATURE_PROPERTY)
	{
		return type;
	}
	else
	{
		return ILType_Invalid;
	}
}

ILType *ILTypeFromTypeSpec(ILContext *context, ILImage *image,
						   unsigned long blobStart, unsigned long blobLen)
{
	ILMetaDataRead reader;
	ILType *type;

	/* Initialize the metadata reader */
	reader.data = (unsigned char *)(image->blobPool + blobStart);
	reader.len = blobLen;
	reader.error = 0;

	/* Parse the signature */
	type = ParseElemType(context, image, &reader,
						 IL_TYPE_RECURSION_DEPTH,
						 CATEGORY_CMOD | CATEGORY_VOID);

	/* Return the value to the caller */
	if(!(reader.error) && type)
	{
		return type;
	}
	else
	{
		return ILType_Invalid;
	}
}

ILType *ILTypeFromLocalVarSig(ILImage *image, unsigned long blobOffset)
{
	ILContext *context = image->context;
	ILMetaDataRead reader;
	ILUInt32 kind;
	ILType *locals;
	ILType *nextLocal;
	int isPinned;

	/* Initialize the metadata reader */
	reader.data = (const unsigned char *)
			ILImageGetBlob(image, blobOffset, &(reader.len));
	if(!(reader.data))
	{
		return ILType_Invalid;
	}
	reader.error = 0;

	/* The signature must start with IL_META_CALLCONV_LOCAL_SIG */
	kind = ILMetaUncompressData(&reader);
	if(kind != IL_META_CALLCONV_LOCAL_SIG || reader.error)
	{
		return ILType_Invalid;
	}

	/* Parse the number of local variables */
	kind = ILMetaUncompressData(&reader);
	if(reader.error || kind > (ILUInt32)0xFFFF)
	{
		return ILType_Invalid;
	}

	/* Create the local variable list */
	locals = ILTypeCreateLocalList(context);
	if(locals == ILType_Invalid)
	{
		return ILType_Invalid;
	}

	/* Parse the local variable types */
	while(kind > 0)
	{
		/* Determine if the type is "pinned" */
		isPinned = 0;
		while(reader.len > 0 && reader.data[0] == IL_META_ELEMTYPE_PINNED)
		{
			isPinned = 1;
			++(reader.data);
			--(reader.len);
		}

		/* Parse the main part of the type */
		nextLocal = ParseElemType(context, image, &reader,
						 		  IL_TYPE_RECURSION_DEPTH,
								  CATEGORY_ALL | CATEGORY_PINNED);
		if(nextLocal == ILType_Invalid)
		{
			return ILType_Invalid;
		}

		/* Add the "pinned" flag to the type if necessary */
		if(isPinned)
		{
			nextLocal = ILTypeCreateRef(context, IL_TYPE_COMPLEX_PINNED,
										nextLocal);
			if(nextLocal == ILType_Invalid)
			{
				return ILType_Invalid;
			}
		}

		/* Add the type to the local list */
		if(!ILTypeAddLocal(context, locals, nextLocal))
		{
			return ILType_Invalid;
		}

		/* One less local to parse */
		--kind;
	}

	/* Return the list of locals to the caller */
	return locals;
}

#ifdef	__cplusplus
};
#endif
