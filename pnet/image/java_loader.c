/*
 * java_loader.c - Load Java .class files and convert them into IL images.
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

#include "program.h"
#include "il_jopcodes.h"

#ifdef IL_CONFIG_JAVA

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Fake RVA to use for the base of a .class or .jar file in memory.
 * RVA's don't mean anything to Java, but it keeps the rest of the
 * image library happy if we attempt to simulate them.
 */
#define	IL_JAVA_BASE_RVA		0x00002000

/*
 * Map the entire contents of a file into memory.
 * Return a load error, or zero if OK.
 */
static int MapEntireFile(ILImage *image, FILE *file, long readAlready)
{
	ILSectionMap *map;
	long position;
	long length;

	/* Determine the current position and total size of the file */
	position = ftell(file);
	if(position == -1L || position < readAlready ||
	   fseek(file, 0L, SEEK_END) != 0)
	{
		/* Cannot seek within the stream */
		return IL_LOADERR_NOT_IL;
	}
	length = ftell(file);
	if(length <= position)
	{
		/* The file is too small to be valid */
		return IL_LOADERR_TRUNCATED;
	}
	position -= readAlready;

	/* Attempt to map the file directly into memory */
	image->len = (unsigned long)(length - position);
	if(ILMapFileToMemory(fileno(file), (unsigned long)position,
						 (unsigned long)length, &(image->mapAddress),
						 &(image->mapLength), &(image->data)))
	{
		image->mapped = -1;
	}
	else
	{
		/* Cannot map the file, so read it into a malloc'ed buffer */
		image->data = (char *)ILMalloc(image->len);
		if(!(image->data))
		{
			return IL_LOADERR_MEMORY;
		}
		if(fseek(file, position, SEEK_SET) != 0)
		{
			return IL_LOADERR_NOT_IL;
		}
		if(fread(image->data, 1, image->len, file) != image->len)
		{
			return IL_LOADERR_TRUNCATED;
		}
		image->mapped = 0;
	}

	/* Create a fake section map for mapping the fake RVA's */
	map = (ILSectionMap *)ILMalloc(sizeof(ILSectionMap));
	if(!map)
	{
		return IL_LOADERR_MEMORY;
	}
	map->virtAddr = IL_JAVA_BASE_RVA;
	map->virtSize = image->len;
	map->realAddr = 0;
	map->realSize = image->len;
	map->next = 0;
	image->map = map;

	/* Ready to go */
	return 0;
}

/*
 * Create the module, assembly, and other information that
 * IL images are expected to have, but which Java doesn't.
 * Returns zero if out of memory.
 */
static int CreateModuleInfo(ILImage *image, const char *filename)
{
	int len;

	/* Get the final part of the filename to use as the
	   name of the module and the name of the assembly */
	if(filename)
	{
		len = strlen(filename);
		while(len > 0 && filename[len - 1] != '/' &&
			  filename[len - 1] != '\\')
		{
			--len;
		}
		filename += len;
	}
	else
	{
		filename = "Java";
	}

	/* Create the module definition token */
	if(!ILModuleCreate(image, 0, filename, 0))
	{
		return 0;
	}

	/* Create the assembly token */
	if(!ILAssemblyCreate(image, 0, filename, 0))
	{
		return 0;
	}

	/* Create the global TypeDef */
	if(!ILClassCreate(ILClassGlobalScope(image), 0, "<Module>", 0, 0))
	{
		return 0;
	}

	/* Ready to go */
	return 1;
}

/*
 * Helper structure for reading a Java class from
 * a region of memory.
 */
typedef struct
{
	unsigned char *data;
	unsigned long  len;

} JavaReader;

/*
 * Read a single-byte quantity from a Java reader.
 */
#define	JAVA_READ_BYTE(reader,var)	\
			do { \
				if((reader)->len < 1) \
				{ \
					error = IL_LOADERR_TRUNCATED; \
					goto cleanup; \
				} \
				(var) = (ILUInt32)(*((reader)->data)); \
				++((reader)->data); \
				--((reader)->len); \
			} while (0)

/*
 * Read a 16-bit quantity from a Java reader.
 */
#define	JAVA_READ_UINT16(reader,var)	\
			do { \
				if((reader)->len < 2) \
				{ \
					error = IL_LOADERR_TRUNCATED; \
					goto cleanup; \
				} \
				(var) = (ILUInt32)(IL_BREAD_UINT16((reader)->data)); \
				((reader)->data) += 2; \
				((reader)->len) -= 2; \
			} while (0)

/*
 * Read a 32-bit quantity from a Java reader.
 */
#define	JAVA_READ_UINT32(reader,var)	\
			do { \
				if((reader)->len < 4) \
				{ \
					error = IL_LOADERR_TRUNCATED; \
					goto cleanup; \
				} \
				(var) = IL_BREAD_UINT32((reader)->data); \
				((reader)->data) += 4; \
				((reader)->len) -= 4; \
			} while (0)

/*
 * Read a 64-bit quantity from a Java reader.
 */
#define	JAVA_READ_INT64(reader,var)	\
			do { \
				if((reader)->len < 8) \
				{ \
					error = IL_LOADERR_TRUNCATED; \
					goto cleanup; \
				} \
				(var) = IL_BREAD_INT64((reader)->data); \
				((reader)->data) += 8; \
				((reader)->len) -= 8; \
			} while (0)

/*
 * Skip a group of bytes.
 */
#define	JAVA_SKIP_BYTES(reader,nbytes)	\
			do { \
				if((reader)->len < (nbytes)) \
				{ \
					return IL_LOADERR_TRUNCATED; \
				} \
				((reader)->data) += (nbytes); \
				((reader)->len) -= (nbytes); \
			} while (0)

/*
 * Convert a Java-style slashed name into an IL-style
 * name and namespace.  Returns zero if out of memory.
 */
static int JavaNameToILName(const char *jname, ILUInt32 jnameLen,
							char **name, char **namespace, char **freeName)
{
	char *buf;
	ILUInt32 posn;
	ILUInt32 lastSlash;

	/* Copy the name into a conversion buffer */
	if((buf = (char *)ILMalloc(jnameLen + 1)) == 0)
	{
		return 0;
	}
	ILMemCpy(buf, jname, jnameLen);
	buf[jnameLen] = '\0';

	/* Convert all slashes into periods */
	lastSlash = 0;
	for(posn = 0; posn < jnameLen; ++posn)
	{
		if(buf[posn] == '/')
		{
			buf[posn] = '.';
			lastSlash = posn + 1;
		}
	}

	/* Split the buffer into name and namespace */
	if(lastSlash)
	{
		*name = buf + lastSlash;
		buf[lastSlash - 1] = '\0';
		*namespace = buf;
	}
	else
	{
		*name = buf;
		*namespace = 0;
	}
	*freeName = buf;
	return 1;
}

/*
 * Create a reference to a named class.
 */
static ILClass *CreateJavaClassRef(ILImage *image, const char *name,
							       const char *namespace, int flags)
{
	ILClass *classInfo;
	ILClass *otherClass;

	/* See if we already have a class with this name and namespace */
	classInfo = ILClassLookup(ILClassGlobalScope(image), name, namespace);
	if(classInfo)
	{
		return classInfo;
	}

	/* Attempt to resolve the class to other images in the context */
	if((flags & IL_LOADFLAG_NO_RESOLVE) == 0)
	{
		otherClass = ILClassLookupGlobal(ILImageToContext(image),
										 name, namespace);
	}
	else
	{
		otherClass = 0;
	}

	/* Create a class reference for the name and namespace */
	classInfo = ILClassCreateRef(ILClassGlobalScope(image), 0,
								 name, namespace);
	if(!classInfo)
	{
		return 0;
	}

	/* Link the reference to the resolved other class, if one was found */
	if(otherClass)
	{
		if(!_ILProgramItemLink(&(classInfo->programItem),
							   &(otherClass->programItem)))
		{
			return 0;
		}
	}

	/* Done */
	return classInfo;
}

/*
 * Resolve a class reference using the constant pool.
 * Returns NULL if out of memory or the index is invalid.
 */
static ILClass *ResolveJavaClass(ILImage *image, JavaConstEntry *constPool,
								 ILUInt32 constPoolEntries, ILUInt32 index,
								 int *error, int flags)
{
	ILUInt32 nameIndex;
	char *name;
	char *namespace;
	char *freeName;
	ILClass *classInfo;

	/* Validate the index */
	if(index >= constPoolEntries ||
	   constPool[index].type != JAVA_CONST_CLASS)
	{
		*error = IL_LOADERR_BAD_META;
		return 0;
	}

	/* If we already have a reference, then return it */
	if(constPool[index].un.classValue.classInfo)
	{
		return constPool[index].un.classValue.classInfo;
	}

	/* Convert the class name into an IL name and namespace */
	nameIndex = constPool[index].un.classValue.nameIndex;
	if(!JavaNameToILName(constPool[nameIndex].un.utf8String,
						 constPool[nameIndex].length,
						 &name, &namespace, &freeName))
	{
		*error = IL_LOADERR_MEMORY;
		return 0;
	}

	/* Create a reference for the class */
	classInfo = CreateJavaClassRef(image, name, namespace, flags);
	ILFree(freeName);
	if(!classInfo)
	{
		*error = IL_LOADERR_MEMORY;
		return 0;
	}

	/* Cache the class reference for next time */
	constPool[index].un.classValue.classInfo = classInfo;

	/* Done */
	return classInfo;
}

/*
 * Resolve a class reference at load time.
 */
static ILClass *ResolveJavaClassLoad(ILImage *image, JavaConstEntry *constPool,
								     ILUInt32 constPoolEntries, ILUInt32 index,
								     int *error, int flags)
{
	ILClass *classInfo;
	classInfo = ResolveJavaClass(image, constPool, constPoolEntries,
								 index, error, flags);
	if(!classInfo && *error == IL_LOADERR_BAD_META)
	{
		META_ERROR("invalid class index");
	}
	return classInfo;
}

/*
 * Helper macro for resolving class references during loading.
 */
#define	ResolveJavaClassRef(index)	\
			(ResolveJavaClassLoad(image, constPool, constPoolEntries, \
							      (index), &error, flags))

/*
 * Helper macro for resolving class definitions.
 */
#define	ResolveJavaClassDefn(index)	\
			(ResolveJavaClassLoad(image, constPool, constPoolEntries, \
							      (index), &error, IL_LOADFLAG_NO_RESOLVE))

/*
 * Parse a Java type specification.  Returns
 * a load error code.
 */
static int ParseJavaType(ILImage *image, int flags,
						 JavaReader *reader, JavaReader *mangled,
						 ILType **type, int showError)
{
	char typeChar;
	unsigned long nameLen;
	char *name;
	char *namespace;
	char *freeName;
	ILClass *classInfo;
	int error;

	if(reader->len >= 1)
	{
		typeChar = (char)(reader->data[0]);
		++(reader->data);
		--(reader->len);
		switch(typeChar)
		{
			case 'B':		*type = ILType_Int8;    return 0;
			case 'S':		*type = ILType_Int16;   return 0;
			case 'C':		*type = ILType_Char;    return 0;
			case 'D':		*type = ILType_Float64; return 0;
			case 'F':		*type = ILType_Float32; return 0;
			case 'Z':		*type = ILType_Boolean; return 0;

			case 'I':
			{
				/* If name mangling information is present, then we
				   may need to change to an alternative IL type */
				if(mangled && mangled->len)
				{
					typeChar = (char)(mangled->data[0]);
					++(mangled->data);
					--(mangled->len);
					switch(typeChar)
					{
						case 'B':	*type = ILType_UInt8;  return 0;
						case 'S':	*type = ILType_UInt16; return 0;
						case 'i':	*type = ILType_Int32;  return 0;
						case 'I':	*type = ILType_UInt32; return 0;
						default:	break;
					}
					break;
				}
				else
				{
					*type = ILType_Int32;
				}
				return 0;
			}
			/* Not reached */

			case 'J':
			{
				/* Choose between "int64" and "unsigned int64" depending
				   upon the name mangling information that is present */
				if(mangled && mangled->len)
				{
					typeChar = (char)(mangled->data[0]);
					++(mangled->data);
					--(mangled->len);
					if(typeChar == 'L')
					{
						*type = ILType_UInt64;
					}
					else if(typeChar == 'l')
					{
						*type = ILType_Int64;
					}
					else
					{
						break;
					}
				}
				else
				{
					*type = ILType_Int64;
				}
				return 0;
			}
			/* Not reached */

			case 'L':
			{
				/* Parse a class name */
				nameLen = 0;
				while(nameLen < reader->len &&
					  reader->data[nameLen] != (unsigned char)';')
				{
					++nameLen;
				}
				if(nameLen >= reader->len)
				{
					break;
				}
				if(!JavaNameToILName((char *)(reader->data), nameLen,
						 			 &name, &namespace, &freeName))
				{
					return IL_LOADERR_MEMORY;
				}
				reader->data += (nameLen + 1);
				reader->len -= (nameLen + 1);

				/* Create a class reference for this name */
				classInfo = CreateJavaClassRef(image, name, namespace, flags);
				ILFree(freeName);
				if(!classInfo)
				{
					return IL_LOADERR_MEMORY;
				}

				/* Convert the class into a type and return it */
				if(mangled && mangled->len > 0)
				{
					/* We have name mangling information, which may alter
					   the interpretation of the reference to "value type" */
					typeChar = (char)(mangled->data[0]);
					++(mangled->data);
					--(mangled->len);
					if(typeChar == 'C')
					{
						*type = ILType_FromClass(classInfo);
					}
					else if(typeChar == 'V')
					{
						*type = ILType_FromValueType(classInfo);
					}
					else
					{
						break;
					}
				}
				else
				{
					*type = ILType_FromClass(classInfo);
				}
				return 0;
			}
			/* Not reached */

			case '[':
			{
				/* Parse an array type */
				error = ParseJavaType(image, flags, reader,
									  mangled, type, showError);
				if(error == 0)
				{
					*type = ILTypeCreateArray(image->context, 1, *type);
					if(*type)
					{
						return 0;
					}
					return IL_LOADERR_MEMORY;
				}
				else
				{
					return error;
				}
			}
			/* Not reached */

			default: break;
		}
	}
	if(showError)
	{
		META_ERROR("invalid type specification");
	}
	return IL_LOADERR_BAD_META;
}

/*
 * Determine if we have a string match in the constant pool.
 */
#define	JAVA_IS_STRING(value,name,len)	\
			((value) < constPoolEntries && \
			 constPool[(value)].type == JAVA_CONST_UTF8 && \
			 constPool[(value)].length == (len) && \
			 !ILMemCmp(constPool[(value)].un.utf8String, (name), (len)))

/*
 * Parse a field or method member, and convert
 * it into IL conventions.  Returns a load error.
 */
static int ParseJavaMember(ILImage *image, JavaReader *reader, int flags,
						   JavaConstEntry *constPool, ILUInt32 constPoolEntries,
						   ILClass *classInfo, ILUInt32 accessFlags,
						   int isMethod)
{
	ILUInt32 value;
	ILUInt32 length;
	ILUInt32 numAttrs;
	char *name;
	ILUInt32 nameLen;
	char *copiedName = 0;
	int error = 0;
	ILField *field;
	ILMethod *method;
	JavaReader sigReader;
	ILType *signature;
	ILType *argType;
	JavaReader mangled;
	ILUInt32 rva;

	/* Parse the name and signature descriptor */
	JAVA_READ_UINT16(reader, value);
	if(value >= constPoolEntries ||
	   constPool[value].type != JAVA_CONST_UTF8)
	{
		META_ERROR((isMethod ? "invalid method name" : "invalid field name"));
		error = IL_LOADERR_BAD_META;
		goto cleanup;
	}
	name = constPool[value].un.utf8String;
	nameLen = constPool[value].length;
	JAVA_READ_UINT16(reader, value);
	if(value >= constPoolEntries ||
	   constPool[value].type != JAVA_CONST_UTF8)
	{
		META_ERROR((isMethod ? "invalid method signature" :
							   "invalid field type"));
		error = IL_LOADERR_BAD_META;
		goto cleanup;
	}
	sigReader.data = (unsigned char *)(constPool[value].un.utf8String);
	sigReader.len = constPool[value].length;

	/* Parse the attributes at the end of the definition */
	mangled.data = 0;
	mangled.len = 0;
	rva = 0;
	JAVA_READ_UINT16(reader, numAttrs);
	while(numAttrs > 0)
	{
		JAVA_READ_UINT16(reader, value);
		JAVA_READ_UINT32(reader, length);
		if(JAVA_IS_STRING(value, "CLI.NameMangle", 14))
		{
			/* The name of the field or method has been mangled to
			   include extension type information */
			if(length == 4)
			{
				JAVA_READ_UINT32(reader, mangled.len);
				length -= 4;
			}
			else
			{
				META_ERROR("name mangling attribute is incorrect");
				error = IL_LOADERR_BAD_META;
				goto cleanup;
			}
		}
		else if(JAVA_IS_STRING(value, "Code", 4))
		{
			/* This is the method's code */
			rva = IL_JAVA_BASE_RVA +
						(ILUInt32)((reader->data - 4) -
								   (unsigned char *)(image->data));
		}
		JAVA_SKIP_BYTES(reader, length);
		--numAttrs;
	}

	/* Copy the name and NUL-terminate it */
	if((copiedName = (char *)ILMalloc(nameLen + 1)) == 0)
	{
		error = IL_LOADERR_MEMORY;
		goto cleanup;
	}
	ILMemCpy(copiedName, name, nameLen);
	copiedName[nameLen] = '\0';

	/* If the name was mangled, then we must extract the type information */
	if(mangled.len)
	{
		if(mangled.len > (ILUInt32)0x80000000 ||
		   (mangled.len + 2) >= nameLen ||
		   copiedName[nameLen - mangled.len - 2] != '_' ||
		   copiedName[nameLen - mangled.len - 1] != '_')
		{
			META_ERROR("name mangling length is incorrect");
			error = IL_LOADERR_BAD_META;
			goto cleanup;
		}
		mangled.data = (unsigned char *)(copiedName + nameLen - mangled.len);
		copiedName[nameLen - mangled.len - 2] = '\0';
	}

	/* Create the method or field block */
	if(isMethod)
	{
		/* Create the method block */
		if(!strcmp(copiedName, "<init>"))
		{
			/* Instance constructors are never virtual */
			method = ILMethodCreate(classInfo, 0, ".ctor",
									(accessFlags & 0xFFFF) &
										~IL_META_METHODDEF_VIRTUAL);
		}
		else if(!strcmp(copiedName, "<clinit>"))
		{
			method = ILMethodCreate(classInfo, 0, ".cctor",
									(accessFlags & 0xFFFF));
		}
		else
		{
			method = ILMethodCreate(classInfo, 0, copiedName,
									(accessFlags & 0xFFFF));
		}
		if(!method)
		{
			error = IL_LOADERR_MEMORY;
			goto cleanup;
		}
		ILMethodSetImplAttrs(method, IL_MAX_UINT32,
							 ((accessFlags >> 16) & 0xFFFF));
		if((accessFlags & IL_META_METHODDEF_VIRTUAL) != 0)
		{
			ILMethodSetCallConv(method, IL_META_CALLCONV_HASTHIS);
		}
		else
		{
			ILMethodSetCallConv(method, IL_META_CALLCONV_DEFAULT);
		}

		/* Set the method's RVA */
		ILMethodSetRVA(method, rva);

		/* Create the method signature */
		signature = ILTypeCreateMethod(image->context, ILType_Void);
		if(!signature)
		{
			error = IL_LOADERR_MEMORY;
			goto cleanup;
		}
		ILTypeSetCallConv(signature, ILMethodGetCallConv(method));
		if(sigReader.len < 1 ||
		   sigReader.data[0] != (unsigned char)'(')
		{
			META_ERROR("method signatures must begin with `('");
			error = IL_LOADERR_BAD_META;
			goto cleanup;
		}
		else
		{
			/* Parse the parameter types */
			++(sigReader.data);
			--(sigReader.len);
			while(sigReader.len > 0 &&
			      sigReader.data[0] != (unsigned char)')')
			{
				error = ParseJavaType(image, flags, &sigReader,
									  &mangled, &argType, 1);
				if(error != 0)
				{
					goto cleanup;
				}
				if(!ILTypeAddParam(image->context, signature, argType))
				{
					error = IL_LOADERR_MEMORY;
					goto cleanup;
				}
			}
			if(sigReader.len < 1)
			{
				META_ERROR("method signatures must contain `)'");
				error = IL_LOADERR_BAD_META;
				goto cleanup;
			}
			++(sigReader.data);
			--(sigReader.len);
			if(sigReader.len < 1)
			{
				META_ERROR("missing return type for method signature");
				error = IL_LOADERR_BAD_META;
				goto cleanup;
			}
			if(sigReader.data[0] == (unsigned char)'V')
			{
				++(sigReader.data);
				--(sigReader.len);
			}
			else
			{
				error = ParseJavaType(image, flags, &sigReader,
									  &mangled, &argType, 1);
				if(error != 0)
				{
					goto cleanup;
				}
				ILTypeSetReturn(signature, argType);
			}
		}
		ILMemberSetSignature((ILMember *)method, signature);
		if(sigReader.len > 0)
		{
			META_ERROR("spurious data at end of method signature");
			error = IL_LOADERR_BAD_META;
		}
	}
	else
	{
		field = ILFieldCreate(classInfo, 0, copiedName, accessFlags);
		if(!field)
		{
			error = IL_LOADERR_MEMORY;
			goto cleanup;
		}
		error = ParseJavaType(image, flags, &sigReader,
							  &mangled, &signature, 1);
		if(error == 0)
		{
			ILMemberSetSignature((ILMember *)field, signature);
			if(sigReader.len > 0)
			{
				META_ERROR("spurious data at end of field type");
				error = IL_LOADERR_BAD_META;
			}
		}
	}

	/* Clean up and exit */
cleanup:
	if(copiedName)
	{
		ILFree(copiedName);
	}
	return error;
}

/*
 * Load a Java class from a region of memory.
 * Returns a load error code, or zero if OK.
 */
static int LoadJavaClass(ILImage *image, JavaReader *reader, int flags)
{
	ILUInt32 constPoolEntries;
	JavaConstEntry *constPool = 0;
	ILUInt32 index;
	ILUInt32 constType;
	ILUInt32 value;
	ILUInt32 accessFlags;
	unsigned char floatBuf[8];
	ILClass *classInfo;
	ILClass *otherClass;
	ILClassExt *ext;
	int error = 0;

	/* Skip the 8 bytes of header, which we have already parsed */
	JAVA_SKIP_BYTES(reader, 8);

	/* Get the number of constant pool entries */
	JAVA_READ_UINT16(reader, constPoolEntries);
	if(!constPoolEntries)
	{
		META_ERROR("constant pool has zero size");
		return IL_LOADERR_BAD_META;
	}

	/* Allocate an array to hold the constant pool */
	if((constPool = (JavaConstEntry *)ILMemStackAllocItem
			(&(image->memStack),
			 constPoolEntries * sizeof(JavaConstEntry))) == 0)
	{
		return IL_LOADERR_MEMORY;
	}

	/* Parse the constant pool */
	constPool[0].type = 0;
	for(index = 1; index < constPoolEntries; ++index)
	{
		JAVA_READ_BYTE(reader, constType);
		constPool[index].type = (ILUInt16)constType;
		switch(constType)
		{
			case JAVA_CONST_UTF8:
			{
				/* Record the position of this UTF-8 string.  We don't
				   convert it because we don't yet know if it is a class
				   name, method name, user string, or something else */
				JAVA_READ_UINT16(reader, value);
				constPool[index].length = (ILUInt16)value;
				constPool[index].un.utf8String = (char *)(reader->data);
				JAVA_SKIP_BYTES(reader, value);
			}
			break;

			case JAVA_CONST_INTEGER:
			{
				/* Parse a 32-bit integer */
				JAVA_READ_UINT32(reader, value);
				constPool[index].un.intValue = (ILInt32)value;
			}
			break;

			case JAVA_CONST_FLOAT:
			{
				/* Parse a 32-bit floating point value */
				if(reader->len < 4)
				{
					error = IL_LOADERR_TRUNCATED;
					goto cleanup;
				}
				floatBuf[0] = reader->data[3];
				floatBuf[1] = reader->data[2];
				floatBuf[2] = reader->data[1];
				floatBuf[3] = reader->data[0];
				constPool[index].un.floatValue = IL_READ_FLOAT(floatBuf);
				reader->data += 4;
				reader->len -= 4;
			}
			break;

			case JAVA_CONST_LONG:
			{
				/* Parse a 64-bit long integer */
				JAVA_READ_INT64(reader, constPool[index].un.longValue);

				/* Long constants take up two constant pool entries */
				++index;
			}
			break;

			case JAVA_CONST_DOUBLE:
			{
				/* Parse a 64-bit double floating point value */
				if(reader->len < 8)
				{
					error = IL_LOADERR_TRUNCATED;
					goto cleanup;
				}
				floatBuf[0] = reader->data[7];
				floatBuf[1] = reader->data[6];
				floatBuf[2] = reader->data[5];
				floatBuf[3] = reader->data[4];
				floatBuf[4] = reader->data[3];
				floatBuf[5] = reader->data[2];
				floatBuf[6] = reader->data[1];
				floatBuf[7] = reader->data[0];
				constPool[index].un.doubleValue = IL_READ_DOUBLE(floatBuf);
				reader->data += 8;
				reader->len -= 8;

				/* Double constants take up two constant pool entries */
				++index;
			}
			break;

			case JAVA_CONST_CLASS:
			{
				/* Parse a class reference */
				JAVA_READ_UINT16(reader, value);
				constPool[index].un.classValue.nameIndex = value;
				constPool[index].un.classValue.classInfo = 0;
			}
			break;

			case JAVA_CONST_STRING:
			{
				/* Parse a string constant */
				JAVA_READ_UINT16(reader, value);
				constPool[index].un.strValue = value;
			}
			break;

			case JAVA_CONST_FIELDREF:
			case JAVA_CONST_METHODREF:
			case JAVA_CONST_INTERFACEMETHODREF:
			{
				/* Parse a field or method reference */
				JAVA_READ_UINT16(reader, value);
				constPool[index].un.refValue.classIndex = value;
				JAVA_READ_UINT16(reader, value);
				constPool[index].un.refValue.nameAndType = value;
				constPool[index].un.refValue.item = 0;
			}
			break;

			case JAVA_CONST_NAMEANDTYPE:
			{
				/* Parse a name and type specification */
				JAVA_READ_UINT16(reader, value);
				constPool[index].un.nameAndType.name = value;
				JAVA_READ_UINT16(reader, value);
				constPool[index].un.nameAndType.type = value;
			}
			break;

			default:
			{
				META_ERROR("invalid constant type");
				error = IL_LOADERR_BAD_META;
				goto cleanup;
			}
			/* Not reached */
		}
	}

	/* Validate the constant pool, now that we know the type of everything */
	for(index = 1; index < constPoolEntries; ++index)
	{
		switch(constPool[index].type)
		{
			case JAVA_CONST_CLASS:
			{
				/* Validate a class reference */
				value = constPool[index].un.classValue.nameIndex;
				if(value >= constPoolEntries ||
				   constPool[value].type != JAVA_CONST_UTF8)
				{
					META_ERROR("invalid class constant");
					error = IL_LOADERR_BAD_META;
					goto cleanup;
				}
			}
			break;

			case JAVA_CONST_STRING:
			{
				/* Validate a string constant */
				value = constPool[index].un.strValue;
				if(value >= constPoolEntries ||
				   constPool[value].type != JAVA_CONST_UTF8)
				{
					META_ERROR("invalid string constant");
					error = IL_LOADERR_BAD_META;
					goto cleanup;
				}
			}
			break;

			case JAVA_CONST_FIELDREF:
			case JAVA_CONST_METHODREF:
			case JAVA_CONST_INTERFACEMETHODREF:
			{
				/* Validate a field or method reference */
				value = constPool[index].un.refValue.classIndex;
				if(value >= constPoolEntries ||
				   constPool[value].type != JAVA_CONST_CLASS)
				{
					META_ERROR("invalid reference class index");
					error = IL_LOADERR_BAD_META;
					goto cleanup;
				}
				value = constPool[index].un.refValue.nameAndType;
				if(value >= constPoolEntries ||
				   constPool[value].type != JAVA_CONST_NAMEANDTYPE)
				{
					META_ERROR("invalid reference name and type index");
					error = IL_LOADERR_BAD_META;
					goto cleanup;
				}
			}
			break;

			case JAVA_CONST_NAMEANDTYPE:
			{
				/* Parse a name and type specification */
				value = constPool[index].un.nameAndType.name;
				if(value >= constPoolEntries ||
				   constPool[value].type != JAVA_CONST_UTF8)
				{
					META_ERROR("invalid name index in name and type");
					error = IL_LOADERR_BAD_META;
					goto cleanup;
				}
				value = constPool[index].un.nameAndType.type;
				if(value >= constPoolEntries ||
				   constPool[value].type != JAVA_CONST_UTF8)
				{
					META_ERROR("invalid type index in name and type");
					error = IL_LOADERR_BAD_META;
					goto cleanup;
				}
			}
			break;

			default:	break;
		}
	}

	/* Parse the access flags for the class */
	JAVA_READ_UINT16(reader, value);
	if((value & JAVA_ACC_PUBLIC) != 0)
	{
		accessFlags = IL_META_TYPEDEF_PUBLIC;
	}
	else
	{
		accessFlags = IL_META_TYPEDEF_NOT_PUBLIC;
	}
	if((value & JAVA_ACC_FINAL) != 0)
	{
		accessFlags |= IL_META_TYPEDEF_SEALED;
	}
	if((value & JAVA_ACC_INTERFACE) != 0)
	{
		accessFlags |= IL_META_TYPEDEF_INTERFACE;
	}
	if((value & JAVA_ACC_ABSTRACT) != 0)
	{
		accessFlags |= IL_META_TYPEDEF_ABSTRACT;
	}
	accessFlags |= IL_META_TYPEDEF_BEFORE_FIELD_INIT |
				   IL_META_TYPEDEF_SERIALIZABLE;

	/* Create the class definition */
	JAVA_READ_UINT16(reader, value);
	classInfo = ResolveJavaClassDefn(value);
	if(!classInfo)
	{
		goto cleanup;
	}
	if(!ILClassIsRef(classInfo))
	{
		META_ERROR("class is already defined");
		error = IL_LOADERR_BAD_META;
		goto cleanup;
	}

	/* Get the parent class reference */
	JAVA_READ_UINT16(reader, value);
	if(value)
	{
		otherClass = ResolveJavaClassRef(value);
		if(!otherClass)
		{
			goto cleanup;
		}
		if(otherClass == classInfo)
		{
			META_ERROR("classes cannot inherit from themselves");
			error = IL_LOADERR_BAD_META;
			goto cleanup;
		}
	}
	else
	{
		/* This is "java.lang.Object", which has no parent */
		otherClass = 0;
	}

	/* Convert the class from a reference into a definition */
	ILClassCreate(ILClassGlobalScope(image), 0, classInfo->className->name,
				  classInfo->className->namespace,
				  ILToProgramItem(otherClass));
	ILClassSetAttrs(classInfo, IL_MAX_UINT32, accessFlags);

	/* Add the interfaces to the class */
	JAVA_READ_UINT16(reader, index);
	while(index > 0)
	{
		JAVA_READ_UINT16(reader, value);
		otherClass = ResolveJavaClassRef(value);
		if(!otherClass)
		{
			goto cleanup;
		}
		if(!ILClassAddImplements(classInfo, ILToProgramItem(otherClass), 0))
		{
			error = IL_LOADERR_MEMORY;
			goto cleanup;
		}
		--index;
	}

	/* Parse the fields associated with the class */
	JAVA_READ_UINT16(reader, index);
	while(index > 0)
	{
		/* Parse the field access flags */
		JAVA_READ_UINT16(reader, value);
		if((value & JAVA_ACC_PUBLIC) != 0)
		{
			accessFlags = IL_META_FIELDDEF_PUBLIC;
		}
		else if((value & JAVA_ACC_PRIVATE) != 0)
		{
			accessFlags = IL_META_FIELDDEF_PRIVATE;
		}
		else if((value & JAVA_ACC_PROTECTED) != 0)
		{
			accessFlags = IL_META_FIELDDEF_FAMILY;
		}
		else
		{
			/* IL doesn't have "package private".  The closest is "assembly" */
			accessFlags = IL_META_FIELDDEF_ASSEMBLY;
		}
		if((value & JAVA_ACC_STATIC) != 0)
		{
			accessFlags |= IL_META_FIELDDEF_STATIC;
		}
		if((value & JAVA_ACC_FINAL) != 0)
		{
			accessFlags |= IL_META_FIELDDEF_INIT_ONLY;
		}
		if((value & JAVA_ACC_TRANSIENT) != 0)
		{
			accessFlags |= IL_META_FIELDDEF_NOT_SERIALIZED;
		}

		/* Parse the field information */
		error = ParseJavaMember(image, reader, flags, constPool,
							    constPoolEntries, classInfo,
							    accessFlags, 0);
		if(error != 0)
		{
			goto cleanup;
		}

		/* Advance to the next field */
		--index;
	}

	/* Parse the methods associated with the class */
	JAVA_READ_UINT16(reader, index);
	while(index > 0)
	{
		/* Parse the method access flags */
		JAVA_READ_UINT16(reader, value);
		if((value & JAVA_ACC_PUBLIC) != 0)
		{
			accessFlags = IL_META_METHODDEF_PUBLIC;
		}
		else if((value & JAVA_ACC_PRIVATE) != 0)
		{
			accessFlags = IL_META_METHODDEF_PRIVATE;
		}
		else if((value & JAVA_ACC_PROTECTED) != 0)
		{
			accessFlags = IL_META_METHODDEF_FAMILY;
		}
		else
		{
			/* IL doesn't have "package private".  The closest is "assembly" */
			accessFlags = IL_META_METHODDEF_ASSEM;
		}
		if((value & JAVA_ACC_STATIC) != 0)
		{
			accessFlags |= IL_META_METHODDEF_STATIC;
		}
		else
		{
			accessFlags |= IL_META_METHODDEF_VIRTUAL;
		}
		if((value & JAVA_ACC_FINAL) != 0)
		{
			accessFlags |= IL_META_METHODDEF_FINAL;
		}
		if((value & JAVA_ACC_SYNCHRONIZED) != 0)
		{
			accessFlags |= (IL_META_METHODIMPL_SYNCHRONIZED << 16);
		}
		if((value & JAVA_ACC_NATIVE) != 0)
		{
			accessFlags |= (IL_META_METHODIMPL_INTERNAL_CALL << 16);
		}
		if((value & JAVA_ACC_ABSTRACT) != 0)
		{
			accessFlags |= IL_META_METHODDEF_ABSTRACT;
		}
		if((value & JAVA_ACC_STRICT) != 0)
		{
			accessFlags |= (IL_META_METHODIMPL_JAVA_FP_STRICT << 16);
		}
		accessFlags |= (IL_META_METHODIMPL_JAVA << 16);
		accessFlags |= IL_META_METHODDEF_HIDE_BY_SIG;

		/* Parse the method information */
		error = ParseJavaMember(image, reader, flags, constPool,
							    constPoolEntries, classInfo,
							    accessFlags, 1);
		if(error != 0)
		{
			goto cleanup;
		}

		/* Advance to the next method */
		--index;
	}

	/* Attach the constant pool to the class */
	if((ext = _ILClassExtCreate(classInfo, _IL_EXT_JAVA_CONSTPOOL)) == 0)
	{
		error = IL_LOADERR_MEMORY;
		goto cleanup;
	}
	ext->un.javaConstPool.size = constPoolEntries;
	ext->un.javaConstPool.entries = constPool;

	/* Clean up and exit */
cleanup:
	return error;
}

/*
 * Locate the central directory within a .jar file.
 * Returns a load error code.
 */
static int LocateJarDirectory(ILImage *image, unsigned char **start,
							  unsigned long *length)
{
	unsigned char *data = (unsigned char *)image->data;
	unsigned long posn;
	unsigned long count;
	unsigned long found;

	/* If the file is less than 22 bytes in size, then
	   it is too small to contain a valid .jar file */
	if(image->len < 22)
	{
		META_ERROR("not a valid .jar file");
		return IL_LOADERR_BAD_META;
	}

	/* Scan backwards from the end of the file for the
	   "end of central directory" record.  We search up
	   to 64k to allow for very long zipfile comments */
	posn = image->len - 22;
	count = 0;
	found = 0;
	while(posn > 0 && count < (unsigned long)65536)
	{
		if(data[posn]     == (unsigned char)'P' &&
		   data[posn + 1] == (unsigned char)'K' &&
		   data[posn + 2] == (unsigned char)0x05 &&
		   data[posn + 3] == (unsigned char)0x06)
		{
			found = posn;
			break;
		}
		--posn;
		++count;
	}
	if(!found)
	{
		META_ERROR("could not locate the .jar central directory");
		return IL_LOADERR_BAD_META;
	}

	/* Make sure that this is a single-part .jar file */
	if(IL_READ_UINT16(data + posn + 4) != 0 ||	/* Part number */
	   IL_READ_UINT16(data + posn + 6) != 0 ||	/* Part with central dir */
	   IL_READ_UINT16(data + posn + 8) !=		/* Total dir entries */
	   IL_READ_UINT16(data + posn + 10))		/* Dir entries in this part */
	{
		META_ERROR("multi-part .jar files are not supported");
		return IL_LOADERR_BAD_META;
	}

	/* Find the start and length of the central directory */
	*length = IL_READ_UINT32(data + posn + 12);
	posn = IL_READ_UINT32(data + posn + 16);
	if(posn >= image->len || *length < 4 ||
	   *length > image->len ||
	   (posn + *length) > image->len ||
	   data[posn]     != (unsigned char)'P' ||
	   data[posn + 1] != (unsigned char)'K' ||
	   data[posn + 2] != (unsigned char)0x01 ||
	   data[posn + 3] != (unsigned char)0x02)
	{
		META_ERROR("invalid .jar central directory");
		return IL_LOADERR_BAD_META;
	}

	/* Compute the starting address and return */
	*start = data + posn;
	return 0;
}

/*
 * Load all .class files from a .jar file.
 * Returns a load error.
 */
static int LoadJarClasses(ILImage *image, int flags,
						  unsigned char *dir, unsigned long dirLen)
{
	int error;
	unsigned long entryLen;
	unsigned long nameLen;
	unsigned long filePosn;
	unsigned long fileLen;
	unsigned char *data;
	JavaReader reader;

	while(dirLen >= 46)
	{
		/* Validate the directory entry */
		nameLen = ((unsigned long)IL_READ_UINT16(dir + 28));
		entryLen = 46 + nameLen +
			       ((unsigned long)IL_READ_UINT16(dir + 30)) +
			       ((unsigned long)IL_READ_UINT16(dir + 32));
		if(entryLen > dirLen)
		{
			META_ERROR("truncated directory entry");
			return IL_LOADERR_BAD_META;
		}
		if(dir[0] != (unsigned char)'P' ||
		   dir[1] != (unsigned char)'K' ||
		   dir[2] != (unsigned char)0x01 ||
		   dir[3] != (unsigned char)0x02)
		{
			META_ERROR("invalid directory entry");
			return IL_LOADERR_BAD_META;
		}

		/* Does this look like a .class file that we can load? */
		if(nameLen > 6 && !ILMemCmp(dir + 46 + nameLen - 6, ".class", 6))
		{
			/* Find the start and end of the file's data */
			filePosn = IL_READ_UINT32(dir + 42);
			fileLen = IL_READ_UINT32(dir + 20);
			if(filePosn >= image->len || fileLen > image->len)
			{
				META_ERROR("invalid file location in directory entry");
				return IL_LOADERR_BAD_META;
			}

			/* Validate the compression method, which must be "stored" */
			if(IL_READ_UINT16(dir + 10) != 0)
			{
				META_ERROR("compressed .jar files are not yet supported");
				return IL_LOADERR_BAD_META;
			}

			/* Validate the local file header */
			data = (unsigned char *)image->data + filePosn;
			if((filePosn + 30) > image->len ||
			   data[0] != (unsigned char)'P' ||
			   data[1] != (unsigned char)'K' ||
			   data[2] != (unsigned char)0x03 ||
			   data[3] != (unsigned char)0x04)
			{
				META_ERROR("invalid local file header");
				return IL_LOADERR_BAD_META;
			}
			nameLen = ((unsigned long)IL_READ_UINT16(data + 26)) +
					  ((unsigned long)IL_READ_UINT16(data + 28));
			if((filePosn + 30 + nameLen + fileLen) > image->len)
			{
				META_ERROR("truncated local file header");
				return IL_LOADERR_BAD_META;
			}
			data += 30 + nameLen;

			/* Check the Java class file signature */
			if(fileLen < 8 ||
			   IL_BREAD_UINT32(data) != (ILUInt32)0xCAFEBABE ||
			   (IL_BREAD_UINT16(data + 6) != 45 &&
			    IL_BREAD_UINT16(data + 6) != 46))
			{
				META_ERROR("invalid class within .jar file");
				return IL_LOADERR_BAD_META;
			}

			/* Load the class */
			reader.data = data;
			reader.len = fileLen;
			error = LoadJavaClass(image, &reader, flags);
			if(error != 0)
			{
				return error;
			}
		}

		/* Advance to the next directory entry */
		dir += entryLen;
		dirLen -= entryLen;
	}
	if(dirLen > 0)
	{
		META_ERROR("truncated directory entry");
		return IL_LOADERR_BAD_META;
	}
	return 0;
}

/*
 * Get the java cost pool attached to a class.
 * Returns o if there is no java const pool attached to the class.
 */
static JavaConstPool *JavaGetConstPool(ILClass *info)
{
	ILClassExt *ext;

	if((ext = _ILClassExtFind(info, _IL_EXT_JAVA_CONSTPOOL)) != 0)
	{
		return &(ext->un.javaConstPool);
	}
	return 0;
}

static JavaConstEntry *JavaGetConstEntry(ILClass *info, ILUInt32 index)
{
	ILClassExt *ext;
	
	if((ext = _ILClassExtFind(info, _IL_EXT_JAVA_CONSTPOOL)) != 0)
	{
		if(ext->un.javaConstPool.size > index)
		{
			return &(ext->un.javaConstPool.entries[index]);
		}
	}
	return 0;
}

int _ILImageJavaLoad(FILE *file, const char *filename, ILContext *context,
					 ILImage **image, int flags, char *buffer)
{
	ILUInt16 major;
	int error;
	JavaReader reader;
	unsigned char *jarStart;
	unsigned long jarLength;

	/* Read the file header and determine if this is
	   a .class file or a .jar file.  We assume that
	   the first two bytes have already been read
	   by "ILImageLoad" prior to calling this function */
	if(fread(buffer + 2, 1, 2, file) != 2)
	{
		return IL_LOADERR_TRUNCATED;
	}
	if(IL_BREAD_UINT32(buffer) == (ILUInt32)0xCAFEBABE)
	{
		/* This is a .class file */
		if(fread(buffer, 1, 4, file) != 4)
		{
			return IL_LOADERR_TRUNCATED;
		}
		major = IL_BREAD_UINT16(buffer + 2);
		if(major != 45 && major != 46)
		{
			return IL_LOADERR_NOT_IL;
		}

		/* Create the image structure and initialize it */
		(*image) = ILImageCreate(context);
		if(!(*image))
		{
			return IL_LOADERR_MEMORY;
		}

		/* Map the contents of the file into memory */
		error = MapEntireFile((*image), file, 8);
		if(error != 0)
		{
			ILImageDestroy(*image);
			return error;
		}

		/* Create the module, assembly, and other information */
		if(!CreateModuleInfo(*image, filename))
		{
			ILImageDestroy(*image);
			return IL_LOADERR_MEMORY;
		}

		/* Load the class */
		reader.data = (unsigned char *)((*image)->data);
		reader.len = (*image)->len;
		error = LoadJavaClass(*image, &reader, flags);
		if(error != 0)
		{
			ILImageDestroy(*image);
			return error;
		}

		/* Change the image type to "Java" and return */
		(*image)->type = IL_IMAGETYPE_JAVA;
		return 0;
	}
	else if(buffer[0] == 'P' && buffer[1] == 'K')
	{
		/* Create the image structure and initialize it */
		(*image) = ILImageCreate(context);
		if(!(*image))
		{
			return IL_LOADERR_MEMORY;
		}

		/* Map the contents of the file into memory */
		error = MapEntireFile((*image), file, 4);
		if(error != 0)
		{
			ILImageDestroy(*image);
			return error;
		}

		/* Create the module, assembly, and other information */
		if(!CreateModuleInfo(*image, filename))
		{
			ILImageDestroy(*image);
			return IL_LOADERR_MEMORY;
		}

		/* Locate the central directory */
		error = LocateJarDirectory(*image, &jarStart, &jarLength);
		if(error != 0)
		{
			ILImageDestroy(*image);
			return error;
		}

		/* Load all .class files within the .jar file */
		error = LoadJarClasses(*image, flags, jarStart, jarLength);
		if(error != 0)
		{
			ILImageDestroy(*image);
			return error;
		}

		/* Done */
		return 0;
	}
	else
	{
		/* This is neither a .class file nor a .jar file */
		return IL_LOADERR_NOT_IL;
	}
}

int ILJavaGetConstType(ILClass *info, ILUInt32 index)
{
	if(info)
	{
		JavaConstEntry *entry;
		
		entry = JavaGetConstEntry(info, index);
		if(entry)
		{
			return entry->type;
		}
	}
	return 0;
}

const char *ILJavaGetUTF8String(ILClass *info, ILUInt32 index, ILUInt32 *len)
{
	JavaConstEntry *entry;

	if(!info || (entry = JavaGetConstEntry(info, index)) == 0)
	{
		return 0;
	}
	if(entry->type != JAVA_CONST_UTF8)
	{
		return 0;
	}
	*len = entry->length;
	return entry->un.utf8String;
}

const char *ILJavaGetString(ILClass *info, ILUInt32 index, ILUInt32 *len)
{
	JavaConstEntry *entry;

	if(!info || (entry = JavaGetConstEntry(info, index)) == 0)
	{
		return 0;
	}
	if(entry->type != JAVA_CONST_STRING)
	{
		return 0;
	}
	index = entry->un.strValue;
	return ILJavaGetUTF8String(info, index, len);
}

int ILJavaGetInteger(ILClass *info, ILUInt32 index, ILInt32 *value)
{
	JavaConstEntry *entry;

	if(!info || (entry = JavaGetConstEntry(info, index)) == 0)
	{
		return 0;
	}
	if(entry->type != JAVA_CONST_INTEGER)
	{
		return 0;
	}
	*value = entry->un.intValue;
	return 1;
}

int ILJavaGetLong(ILClass *info, ILUInt32 index, ILInt64 *value)
{
	JavaConstEntry *entry;

	if(!info || (entry = JavaGetConstEntry(info, index)) == 0)
	{
		return 0;
	}
	if(entry->type != JAVA_CONST_LONG)
	{
		return 0;
	}
	*value = entry->un.longValue;
	return 1;
}

int ILJavaGetFloat(ILClass *info, ILUInt32 index, ILFloat *value)
{
	JavaConstEntry *entry;

	if(!info || (entry = JavaGetConstEntry(info, index)) == 0)
	{
		return 0;
	}
	if(entry->type != JAVA_CONST_FLOAT)
	{
		return 0;
	}
	*value = entry->un.floatValue;
	return 1;
}

int ILJavaGetDouble(ILClass *info, ILUInt32 index, ILDouble *value)
{
	JavaConstEntry *entry;

	if(!info || (entry = JavaGetConstEntry(info, index)) == 0)
	{
		return 0;
	}
	if(entry->type != JAVA_CONST_DOUBLE)
	{
		return 0;
	}
	*value = entry->un.doubleValue;
	return 1;
}

ILClass *ILJavaGetClass(ILClass *info, ILUInt32 index, int refOnly)
{
	JavaConstPool *constPool;
	ILClass *classInfo;
	int error;
	
	if(!info || (constPool = JavaGetConstPool(info)) == 0)
	{
		return 0;
	}
	classInfo = ResolveJavaClass(info->programItem.image,
								 constPool->entries,
								 constPool->size,
								 index, &error,
								 (refOnly ? IL_LOADFLAG_NO_RESOLVE : 0));
	if(classInfo && !refOnly && ILClassIsRef(classInfo))
	{
		return 0;
	}
	return classInfo;
}

/*
 * Get the name and type information from a field or method reference.
 */
static int JavaGetNameAndType(ILClass *info, ILUInt32 index,
							  const char **name, ILUInt32 *nameLen,
							  const char **type, ILUInt32 *typeLen)
{
	JavaConstEntry *entry;

	if(!info || (entry = JavaGetConstEntry(info, index)) == 0)
	{
		return 0;
	}
	*name = ILJavaGetUTF8String(info, entry->un.nameAndType.name, nameLen);
	*type = ILJavaGetUTF8String(info, entry->un.nameAndType.type, typeLen);
	return (*name != 0 && *type != 0);
}

/*
 * Perform a simple type match against a signature type code.
 */
#define	SIMPLE_TYPE_MATCH(code)	\
			do { \
				if(*(type->data) == (unsigned char)(code)) \
				{ \
					++(type->data); \
					--(type->len); \
					return 1; \
				} \
			} while (0)

/*
 * Perform a type match with a mandatory mangling code.
 */
#define	MANGLED_TYPE_MATCH(code,mang)	\
			do { \
				if(*(type->data) == (unsigned char)(code)) \
				{ \
					if(mangled->len > 0 && *(mangled->data) == \
							(unsigned char)(mang)) \
					{ \
						++(type->data); \
						--(type->len); \
						++(mangled->data); \
						--(mangled->len); \
						return 1; \
					} \
				} \
			} while (0)

/*
 * Perform a type match with an optional mangling code.
 */
#define	OPT_MANGLED_TYPE_MATCH(code,mang)	\
			do { \
				if(*(type->data) == (unsigned char)(code)) \
				{ \
					if(mangled->len == 0) \
					{ \
						++(type->data); \
						--(type->len); \
						return 1; \
					} \
					if(*(mangled->data) == (unsigned char)(mang)) \
					{ \
						++(type->data); \
						--(type->len); \
						++(mangled->data); \
						--(mangled->len); \
						return 1; \
					} \
				} \
			} while (0)

/*
 * Match an object reference or value type against an IL type.
 */
static int JavaRefMatch(ILClass *classInfo, int manglingCode,
						JavaReader *type, JavaReader *mangled)
{
	const char *jname;
	ILUInt32 jnameLen;
	char *name;
	char *namespace;
	char *freeName;

	/* The signature code must be 'L' */
	if(*(type->data) != (unsigned char)'L')
	{
		return 0;
	}
	++(type->data);
	--(type->len);

	/* Match the mangling code */
	if(mangled->len > 0)
	{
		if(*(mangled->data) != (unsigned char)manglingCode)
		{
			return 0;
		}
		++(mangled->data);
		--(mangled->len);
	}
	else if(manglingCode != 'C')
	{
		return 0;
	}

	/* Extract the Java name for the class */
	jname = (const char *)(type->data);
	jnameLen = 0;
	while(jnameLen < type->len && jname[jnameLen] != ';')
	{
		++jnameLen;
	}
	if(jnameLen >= type->len)
	{
		return 0;
	}
	type->data += jnameLen + 1;
	type->len -= jnameLen + 1;

	/* Convert the Java name into an IL name */
	if(!JavaNameToILName(jname, jnameLen, &name, &namespace, &freeName))
	{
		return 0;
	}

	/* Match the name information against the signature class */
	if(!strcmp(ILClass_Name(classInfo), name))
	{
		jname = ILClass_Namespace(classInfo);
		if(namespace && jname && !strcmp(namespace, jname))
		{
			ILFree(freeName);
			return 1;
		}
		else if(!namespace && !jname)
		{
			ILFree(freeName);
			return 1;
		}
	}
	ILFree(freeName);
	return 0;
}

/*
 * Match a Java signature against an IL type - inner version.
 */
static int JavaSigMatch(ILType *signature, JavaReader *type,
						JavaReader *mangled)
{
	if(!(type->len))
	{
		return 0;
	}
	if(ILType_IsPrimitive(signature))
	{
		/* Primitive type */
		switch(ILType_ToElement(signature))
		{
			case IL_META_ELEMTYPE_VOID:
			{
				SIMPLE_TYPE_MATCH('V');
			}
			break;

			case IL_META_ELEMTYPE_BOOLEAN:
			{
				SIMPLE_TYPE_MATCH('Z');
			}
			break;

			case IL_META_ELEMTYPE_I1:
			{
				SIMPLE_TYPE_MATCH('B');
			}
			break;

			case IL_META_ELEMTYPE_U1:
			{
				MANGLED_TYPE_MATCH('I', 'B');
			}
			break;

			case IL_META_ELEMTYPE_I2:
			{
				SIMPLE_TYPE_MATCH('S');
			}
			break;

			case IL_META_ELEMTYPE_U2:
			{
				MANGLED_TYPE_MATCH('I', 'S');
			}
			break;

			case IL_META_ELEMTYPE_CHAR:
			{
				SIMPLE_TYPE_MATCH('C');
			}
			break;

			case IL_META_ELEMTYPE_I4:
			{
				OPT_MANGLED_TYPE_MATCH('I', 'i');
			}
			break;

			case IL_META_ELEMTYPE_U4:
			{
				MANGLED_TYPE_MATCH('I', 'I');
			}
			break;

			case IL_META_ELEMTYPE_I8:
			{
				OPT_MANGLED_TYPE_MATCH('J', 'l');
			}
			break;

			case IL_META_ELEMTYPE_U8:
			{
				MANGLED_TYPE_MATCH('J', 'L');
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				SIMPLE_TYPE_MATCH('F');
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				SIMPLE_TYPE_MATCH('D');
			}
			break;

			default: break;
		}
	}
	else if(ILType_IsClass(signature))
	{
		/* Class reference */
		return JavaRefMatch(ILType_ToClass(signature), 'C', type, mangled);
	}
	else if(ILType_IsValueType(signature))
	{
		/* Value type reference */
		return JavaRefMatch(ILType_ToClass(signature), 'V', type, mangled);
	}
	else if(signature != 0 && ILType_IsComplex(signature))
	{
		if(ILType_IsSimpleArray(signature))
		{
			/* Match a simple 1-dimensional array */
			if(*(type->data) != (unsigned char)'[')
			{
				return 0;
			}
			++(type->data);
			--(type->len);
			return JavaSigMatch(ILType_ElemType(signature), type, mangled);
		}
		else if(ILType_IsMethod(signature))
		{
			/* Match a method signature */
			unsigned long numParams;
			unsigned long param;
			if(*(type->data) != (unsigned char)'(')
			{
				return 0;
			}
			++(type->data);
			--(type->len);
			numParams = ILTypeNumParams(signature);
			for(param = 1; param <= numParams; ++param)
			{
				if(!JavaSigMatch(ILTypeGetParam(signature, param),
							     type, mangled))
				{
					return 0;
				}
			}
			if(*(type->data) != (unsigned char)')')
			{
				return 0;
			}
			++(type->data);
			--(type->len);
			return JavaSigMatch(ILTypeGetReturn(signature), type, mangled);
		}
	}
	return 0;
}

/*
 * Match a Java signature against an IL type.
 */
static int JavaSignatureMatch(ILType *signature, JavaReader *type,
							  JavaReader *mangled)
{
	if(!JavaSigMatch(signature, type, mangled))
	{
		return 0;
	}
	return (type->len == 0 && mangled->len == 0);
}

/*
 * Resolve a class member.
 */
static ILMember *JavaResolveMember(ILClass *classInfo, int kind,
								   int refOnly, int isStatic,
								   const char *name, ILUInt32 nameLen,
								   const char *type, ILUInt32 typeLen)
{
	ILMember *member;
	const char *memberName;
	ILUInt32 memberNameLen;
	ILUInt32 sigPosn;
	JavaReader signature;
	JavaReader mangled;
	char *nameCopy;
	ILType *sigType;
	ILType *paramType;

	/* Resolve the class as far as we can */
	classInfo = ILClassResolve(classInfo);

	/* Determine if the name looks like it contains signature information.
	   Leading and trailing "__" sequences are not signature indicators */
	memberName = name;
	memberNameLen = nameLen;
	sigPosn = 0;
	if(memberNameLen >= 1 && memberName[0] == '_')
	{
		++memberName;
		--memberNameLen;
	}
	while(memberNameLen > 2)
	{
		if(memberName[0] == '_' && memberName[1] == '_')
		{
			/* Record the location of the last "__" sequence */
			sigPosn = (ILUInt32)((memberName + 2) - name);
		}
		++memberName;
		--memberNameLen;
	}

	/* Scan the class, looking for a member match */
	member = 0;
	while((member = ILClassNextMemberByKind(classInfo, member, kind)) != 0)
	{
		/* Get the member's name */
		memberName = ILMember_Name(member);
		memberNameLen = (ILUInt32)(strlen(memberName));

		/* Check for a simple match, with no name signature information */
		if(memberNameLen == nameLen && !ILMemCmp(memberName, name, nameLen))
		{
			signature.data = (unsigned char *)type;
			signature.len = typeLen;
			mangled.data = 0;
			mangled.len = 0;
			if(JavaSignatureMatch(ILMember_Signature(member),
							      &signature, &mangled))
			{
				return member;
			}
		}

		/* Check for a more complex signature match */
		if(sigPosn != 0 && memberNameLen == (sigPosn - 2) &&
		   !ILMemCmp(memberName, name, memberNameLen))
		{
			signature.data = (unsigned char *)type;
			signature.len = typeLen;
			mangled.data = (unsigned char *)(name + sigPosn);
			mangled.len = nameLen - sigPosn;
			if(JavaSignatureMatch(ILMember_Signature(member),
							      &signature, &mangled))
			{
				return member;
			}
		}
	}

	/* Bail out if we are looking for an actual definition */
	if(!refOnly || !ILClassIsRef(classInfo))
	{
		return 0;
	}

	/* Create a reference place-holder for the member */
	signature.data = (unsigned char *)type;
	signature.len = typeLen;
	if(sigPosn != 0)
	{
		mangled.data = (unsigned char *)(name + sigPosn);
		mangled.len = nameLen - sigPosn;
		nameLen = sigPosn - 2;
	}
	else
	{
		mangled.data = 0;
		mangled.len = 0;
	}
	nameCopy = (char *)ILMalloc(nameLen + 1);
	if(!nameCopy)
	{
		return 0;
	}
	ILMemCpy(nameCopy, name, nameLen);
	nameCopy[nameLen] = '\0';
	member = 0;
	if(kind == IL_META_MEMBERKIND_METHOD)
	{
		/* Create a method reference */
		ILMethod *method;
		if(!strcmp(nameCopy, "<init>"))
		{
			/* Instance constructors are never virtual */
			method = ILMethodCreate(classInfo, (ILToken)IL_MAX_UINT32,
									".ctor",
									IL_META_METHODDEF_PUBLIC);
			isStatic = 0;
		}
		else if(!strcmp(nameCopy, "<clinit>"))
		{
			method = ILMethodCreate(classInfo, (ILToken)IL_MAX_UINT32,
									".cctor",
									IL_META_METHODDEF_PUBLIC |
									IL_META_METHODDEF_STATIC);
			isStatic = 1;
		}
		else
		{
			method = ILMethodCreate(classInfo, (ILToken)IL_MAX_UINT32,
									nameCopy,
									(isStatic ?
										(IL_META_METHODDEF_PUBLIC |
										 IL_META_METHODDEF_STATIC) :
										(IL_META_METHODDEF_PUBLIC |
										 IL_META_METHODDEF_VIRTUAL)));
		}
		if(!method)
		{
			ILFree(nameCopy);
			return 0;
		}
		if(signature.len == 0 || *(signature.data) != (unsigned char)'(')
		{
			ILFree(nameCopy);
			return 0;
		}
		++(signature.data);
		--(signature.len);
		sigType = ILTypeCreateMethod(ILClassToContext(classInfo), ILType_Void);
		if(!sigType)
		{
			ILFree(nameCopy);
			return 0;
		}
		if(isStatic)
		{
			ILTypeSetCallConv(sigType, IL_META_CALLCONV_DEFAULT);
		}
		else
		{
			ILTypeSetCallConv(sigType, IL_META_CALLCONV_HASTHIS);
		}
		ILMethodSetCallConv(method, ILType_CallConv(sigType));
		while(signature.len > 0 && *(signature.data) != (unsigned char)')')
		{
			if(ParseJavaType(ILClassToImage(classInfo), IL_LOADFLAG_NO_RESOLVE,
							 &signature, &mangled, &paramType, 0) != 0)
			{
				ILFree(nameCopy);
				return 0;
			}
			if(!ILTypeAddParam(ILClassToContext(classInfo),
							   sigType, paramType))
			{
				ILFree(nameCopy);
				return 0;
			}
		}
		if(signature.len == 0 || *(signature.data) != (unsigned char)')')
		{
			ILFree(nameCopy);
			return 0;
		}
		++(signature.data);
		--(signature.len);
		if(signature.len > 0 && *(signature.data) == (unsigned char)'V')
		{
			++(signature.data);
			--(signature.len);
		}
		else
		{
			if(ParseJavaType(ILClassToImage(classInfo), IL_LOADFLAG_NO_RESOLVE,
							 &signature, &mangled, &paramType, 0) != 0)
			{
				ILFree(nameCopy);
				return 0;
			}
			ILTypeSetReturn(sigType, paramType);
		}
		member = (ILMember *)method;
		ILMemberSetSignature(member, sigType);
	}
	else
	{
		/* Create a field reference */
		ILField *field = ILFieldCreate
				(classInfo, (ILToken)IL_MAX_UINT32, nameCopy,
			     IL_META_FIELDDEF_PUBLIC |
				 	(isStatic ? IL_META_FIELDDEF_STATIC : 0));
		if(field)
		{
			if(ParseJavaType(ILClassToImage(classInfo), IL_LOADFLAG_NO_RESOLVE,
							 &signature, &mangled, &sigType, 0) == 0)
			{
				member = (ILMember *)field;
				ILMemberSetSignature(member, sigType);
			}
		}
	}
	ILFree(nameCopy);
	return member;
}

ILMethod *ILJavaGetMethod(ILClass *info, ILUInt32 index,
						  int refOnly, int isStatic)
{
	int entryType;
	JavaConstEntry *entry;
	ILClass *classInfo;
	const char *name;
	ILUInt32 nameLen;
	const char *type;
	ILUInt32 typeLen;
	char *nameCopy;

	/* Make sure that the entry is of an appropriate type */
	if(!info || (entry = JavaGetConstEntry(info, index)) == 0)
	{
		return 0;
	}
	entryType = entry->type;
	if(entryType != JAVA_CONST_METHODREF &&
	   entryType != JAVA_CONST_INTERFACEMETHODREF)
	{
		return 0;
	}

	/* Resolve the reference if necessary */
	if(!(entry->un.refValue.item))
	{
		/* Resolve the class that contains the member */
		classInfo = ILJavaGetClass(info, entry->un.refValue.classIndex,
								   refOnly);
		if(!classInfo)
		{
			return 0;
		}

		/* Get the name and type information */
		if(!JavaGetNameAndType(info, entry->un.refValue.nameAndType,
							   &name, &nameLen, &type, &typeLen))
		{
			return 0;
		}
		nameCopy = 0;
		if(nameLen >= 6 && !ILMemCmp(name, "<init>", 6))
		{
			/* We are looking up an instance constructor */
			if(nameLen == 6)
			{
				name = ".ctor";
				nameLen = 5;
			}
			else
			{
				/* We need to preserve the name mangling information */
				nameCopy = (char *)ILMalloc(nameLen - 1);
				if(!nameCopy)
				{
					return 0;
				}
				ILMemCpy(nameCopy, ".ctor", 5);
				ILMemCpy(nameCopy + 5, name + 6, nameLen - 6);
				--nameLen;
			}
		}
		else if(nameLen == 8 && !ILMemCmp(name, "<clinit>", 8))
		{
			/* We are looking up a static constructor */
			name = ".cctor";
			nameLen = 6;
		}

		/* Find the member within the class */
		entry->un.refValue.item = (ILProgramItem *)
			JavaResolveMember(classInfo, IL_META_MEMBERKIND_METHOD,
							  refOnly, isStatic,
							  name, nameLen, type, typeLen);
		if(!(entry->un.refValue.item))
		{
			if(nameCopy)
			{
				ILFree(nameCopy);
			}
			return 0;
		}
		if(nameCopy)
		{
			ILFree(nameCopy);
		}
	}

	/* Return the method to the caller */
	if(refOnly)
	{
		return (ILMethod *)(entry->un.refValue.item);
	}
	else
	{
		return (ILMethod *)(ILMemberResolveRef
					((ILMember *)(entry->un.refValue.item)));
	}
}

ILField *ILJavaGetField(ILClass *info, ILUInt32 index,
						int refOnly, int isStatic)
{
	JavaConstEntry *entry;
	ILClass *classInfo;
	const char *name;
	ILUInt32 nameLen;
	const char *type;
	ILUInt32 typeLen;

	/* Make sure that the entry is of an appropriate type */
	if(!info || (entry = JavaGetConstEntry(info, index)) == 0)
	{
		return 0;
	}
	if(entry->type != JAVA_CONST_FIELDREF)
	{
		return 0;
	}

	/* Resolve the reference if necessary */
	if(!(entry->un.refValue.item))
	{
		/* Resolve the class that contains the member */
		classInfo = ILJavaGetClass(info, entry->un.refValue.classIndex,
								   refOnly);
		if(!classInfo)
		{
			return 0;
		}

		/* Get the name and type information */
		if(!JavaGetNameAndType(info, entry->un.refValue.nameAndType,
							   &name, &nameLen, &type, &typeLen))
		{
			return 0;
		}

		/* Find the member within the class */
		entry->un.refValue.item = (ILProgramItem *)
			JavaResolveMember(classInfo, IL_META_MEMBERKIND_FIELD,
							  refOnly, isStatic,
							  name, nameLen, type, typeLen);
		if(!(entry->un.refValue.item))
		{
			return 0;
		}
	}

	/* Return the field to the caller */
	if(refOnly)
	{
		return (ILField *)(entry->un.refValue.item);
	}
	else
	{
		return (ILField *)(ILMemberResolveRef
					((ILMember *)(entry->un.refValue.item)));
	}
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_CONFIG_JAVA */
