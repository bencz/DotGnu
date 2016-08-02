/*
 * java_writer.c - Writes java images.
 *
 * Copyright (C) 2002  Sylvain Pasche
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

void _ILWriteJavaHeaders(ILWriter *writer)
{
	unsigned char buffer[8];

	buffer[0] = 0xCA;
	buffer[1] = 0xFE;
	buffer[2] = 0xBA;
	buffer[3] = 0xBE;
	/* Minor version */
	buffer[4] = (unsigned char)(6 >> 8);
	buffer[5] = (unsigned char)6;
	/* Major version */
	buffer[6] = (unsigned char)(45 >> 8);
	buffer[7] = (unsigned char)45;
	ILWriterTextWrite(writer, buffer, 8);
}

void ILJavaInitPool(ILWriter *writer, ILClass *info)
{
	ILClassExt *ext;

	if(_ILClassExtFind(info, _IL_EXT_JAVA_CONSTPOOL) != 0)
		return;
	ext = _ILClassExtCreate(info, _IL_EXT_JAVA_CONSTPOOL);
	if(!ext)
	{
		writer->outOfMemory = 1;
		return;
	}
	ext->un.javaConstPool.size = 1;
	ext->un.javaConstPool.entries = ILMalloc(sizeof(JavaConstEntry));
	if(!ext->un.javaConstPool.entries)
	{
		writer->outOfMemory = 1;
		return;
	}
	ext->un.javaConstPool.entries[0].type = 0;
	ext->un.javaConstPool.entries[0].un.codeList = 0;
	ext->un.javaConstPool.entries[0].length = 0;
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

/*
 * Adds a pool entry to the end of the pool table
 */
static ILUInt32 ILJavaAddPool(ILWriter *writer, JavaConstPool *constPool,
							  JavaConstEntry *entry)
{
	ILUInt32 poolSize;

	poolSize = constPool->size;
	constPool->entries = ILRealloc(constPool->entries,
								   sizeof(JavaConstEntry) * (poolSize + 1));
	if(!constPool->entries)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	constPool->entries[poolSize] = *entry;
	(constPool->size)++;
	return poolSize;
}

void ILJavaAppendCode(ILWriter *writer, ILClass *info, ILMethod *method, 
					const void *buffer, unsigned long size)
{
	JavaCodeList *list;
	JavaConstEntry *entry = JavaGetConstEntry(info, 0);

	/* finds the corresponding class */
	list = entry->un.codeList;
	while(list != 0)
	{
		if((ILMethod*)method == (ILMethod*)list->method)
			break;
		list = list->next;
	}
	if(list == 0)
	{
		/* allocate a new buffer */
		list = ILMalloc(sizeof(JavaCodeList));
		if(!list)
		{
			writer->outOfMemory = 1;
			return;
		}
		list->code = ILMalloc(size);
		if(!list->code)
		{
			writer->outOfMemory = 1;
			return;
		}
		ILMemCpy(list->code, buffer, size);
		list->length = size;
		list->method = method;
		list->next = entry->un.codeList;
		entry->un.codeList = list;
	}
	else
	{
		/* use existing buffer */
		list->code = ILRealloc(list->code, list->length + size);
		ILMemCpy((void *)((const char *)list->code + list->length), buffer, size);
		list->length += size;
	}
}

/* taken from codegen/jv_output.c */
static char *JavaStrAppend(ILWriter *writer, char *str1, const char *str2)
{
	int len;
	char *result;

	if(!str1)
	{
		len = strlen(str2) + 1;
		result = (char *)ILMalloc(len);
		if(!result)
		{
			writer->outOfMemory = 1;
			return 0;
		}
		strcpy(result, str2);
		return result;
	}
	else
	{
		len = strlen(str1);
		result = (char *)ILRealloc(str1, len + strlen(str2) + 1);
		if(!result)
		{
			writer->outOfMemory = 1;
			return 0;
		}
		strcpy(result + len, str2);
		return result;
	}
}

/* taken from codegen/jv_output.c */
static char *JavaGetClassName(ILWriter *writer, ILClass *classInfo)
{
	const char *name = ILClass_Name(classInfo);
	const char *namespace = ILClass_Namespace(classInfo);
	char *result;
	int len;
	char ch;

	if(!namespace)
	{
		return JavaStrAppend(writer, 0, name);
	}
	len = strlen(namespace) + strlen(name) + 2;
	result = (char *)ILMalloc(len);
	if(!result)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	len = 0;
	while((ch = namespace[len]) != '\0')
	{
		if(ch == '.')
		{
			result[len++] = '/';
		}
		else
		{
			result[len++] = ch;
		}
	}
	result[len++] = '/';
	strcpy(result + len, name);
	return result;
}

static char* ILMethodNameToJava(char *name)
{
	char *retName;
	if(!strcmp(name, ".ctor"))
	{
		retName = "<init>";
	}
	else if(!strcmp(name, ".cctor"))
	{
		retName = "<clinit>";
	}
	else
	{
		retName = name;
	}
	return retName;
}

/*
 * Translates an IL type to java, and appends the result into **buffer
 * Used when building Java signatures
 */
static int WriteJavaType(ILWriter *writer, ILType *type, char **buffer)
{
	char *name;

	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{

		case IL_META_ELEMTYPE_BOOLEAN:
		{
			*buffer = JavaStrAppend(writer, *buffer, "Z");
		}
		break;

		case IL_META_ELEMTYPE_I1:
		case IL_META_ELEMTYPE_U1:
		{
			*buffer = JavaStrAppend(writer, *buffer, "B");
		}
		break;

		case IL_META_ELEMTYPE_I2:
		{
			*buffer = JavaStrAppend(writer, *buffer, "S");
		}
		break;

		case IL_META_ELEMTYPE_U2:
		case IL_META_ELEMTYPE_CHAR:
		{
			*buffer = JavaStrAppend(writer, *buffer, "C");
		}
		break;

		case IL_META_ELEMTYPE_I4:
		case IL_META_ELEMTYPE_U4:
		case IL_META_ELEMTYPE_I:
		case IL_META_ELEMTYPE_U:
		{
			*buffer = JavaStrAppend(writer, *buffer, "I");
		}
		break;

		case IL_META_ELEMTYPE_I8:
		case IL_META_ELEMTYPE_U8:
		{
			*buffer = JavaStrAppend(writer, *buffer, "J");
		}
		break;

		case IL_META_ELEMTYPE_R4:
		{
			*buffer = JavaStrAppend(writer, *buffer, "F");
		}
		break;

		case IL_META_ELEMTYPE_R8:
		case IL_META_ELEMTYPE_R:
		{
			*buffer = JavaStrAppend(writer, *buffer, "D");
		}
		break;
		case IL_META_ELEMTYPE_VOID:
		{
			*buffer = JavaStrAppend(writer, *buffer, "V");
		}
		break;
		default:
		{
			/* error */
		}
		break;
		}
	}
	else if(ILType_IsValueType(type) ||
			ILType_IsClass(type))
	{
		*buffer = JavaStrAppend(writer, *buffer, "L");
		name = JavaGetClassName(writer, ILType_ToClass(type));
		if(!name)
		{
			writer->outOfMemory = 1;
			return 0;
		}
		*buffer = JavaStrAppend(writer, *buffer, name);
		ILFree(name);
		JavaStrAppend(writer, *buffer, ";");
	}
	else if(ILType_IsArray(type))
	{
		char *ptr;

		*buffer = JavaStrAppend(writer, *buffer, "[");
		ptr = 0;
		WriteJavaType(writer, ILTypeGetElemType(type), &ptr);
		if(!ptr)
		{
			writer->outOfMemory = 1;
			return 0;
		}
		*buffer = JavaStrAppend(writer, *buffer, ptr);
		ILFree(ptr);
	}
	else if(type == ILType_Invalid)
	{
		/* FIXME: for constructors, void return type is invalid */
		JavaStrAppend(writer, *buffer, "V");
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		/* TODO complex type */
	}
	return 0;
}

ILUInt32 ILJavaSetUTF8String(ILWriter *writer, ILClass *info,
							 const char *value, ILUInt32 len)
{
	JavaConstPool *constPool;
	ILUInt32 constPoolEntries;
	JavaConstEntry *entries;
	JavaConstEntry *poolEntry;
	int index;

	constPool = JavaGetConstPool(info);
	if(!constPool)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	constPoolEntries = constPool->size;
	entries = constPool->entries;
	for(index = 0; index < constPoolEntries; index++)
	{
		if(entries[index].type == JAVA_CONST_UTF8 &&
			entries[index].length == len &&
		   !ILMemCmp(entries[index].un.utf8String, value, len))
		{
			return index;
		}
	}
	poolEntry = (JavaConstEntry*)ILMalloc(sizeof(JavaConstEntry));
	if(!poolEntry)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	poolEntry->type = JAVA_CONST_UTF8;
	poolEntry->length = len;
	poolEntry->un.utf8String = ILMalloc(len);
	if(!poolEntry->un.utf8String)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	ILMemCpy(poolEntry->un.utf8String, value, len);
	return ILJavaAddPool(writer, constPool, poolEntry);
}


ILUInt32 ILJavaSetSignature(ILWriter *writer, ILClass *info, ILType *sig)
{
	int numParams;
	int i;
	ILType *param;
	ILUInt32 index;
	char *sigName = 0;

	if(ILType_IsMethod(sig) || ILType_IsProperty(sig))
	{
		sigName = JavaStrAppend(writer, sigName, "(");
		numParams = ILTypeNumParams(sig);
		for(i = 0; i < numParams; i++)
		{
			param = ILTypeGetParam(sig, i + 1);
			WriteJavaType(writer, param, &sigName);
		}
		sigName = JavaStrAppend(writer, sigName, ")");
		param = ILTypeGetReturn(sig);
		WriteJavaType(writer, param, &sigName);
	}
	else
	{
		WriteJavaType(writer, sig, &sigName);
	}

	index = ILJavaSetUTF8String(writer, info, sigName, strlen(sigName));
	ILFree(sigName);
	return index;
}

#define ADD_POOL(constName, unionName, fieldName1, fieldVal1, fieldName2,\
                 fieldVal2)\
	do { \
		ILUInt32 constPoolEntries; \
		JavaConstEntry *entries; \
		JavaConstEntry *poolEntry; \
		int index; \
		\
		constPoolEntries = constPool->size; \
		entries = constPool->entries; \
	    for(index = 0; index < constPoolEntries; index++)\
    	{\
	        if(entries[index].type == JAVA_CONST_##constName &&\
        	    entries[index].un.unionName.fieldName1 == (fieldVal1)  &&\
    	        entries[index].un.unionName.fieldName2 == (fieldVal2))\
	        {\
        	    return index;\
    	    }\
	    }\
	    poolEntry = (JavaConstEntry*)ILMalloc(sizeof(JavaConstEntry));\
	\
	    if(!poolEntry)\
	    {\
	        writer->outOfMemory = 1;\
    	    return 0;\
	    }\
	    poolEntry->type = JAVA_CONST_##constName;\
	    poolEntry->un.unionName.fieldName1 = (fieldVal1);\
	    poolEntry->un.unionName.fieldName2 = (fieldVal2);\
		\
	    return ILJavaAddPool(writer, constPool, poolEntry);\
	} while(0);

ILUInt32 ILJavaSetClass(ILWriter *writer, ILClass *info, ILClass *class)
{
	JavaConstPool *constPool;
	ILUInt32 nameIndex;
	char *name = JavaGetClassName(writer, class);

	constPool = JavaGetConstPool(info);
	if(!constPool)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	nameIndex = ILJavaSetUTF8String(writer, info, name, strlen(name));
	if(!nameIndex)
		return 0;

	ADD_POOL(CLASS, classValue, nameIndex, nameIndex, nameIndex, nameIndex);
}

ILUInt32 ILJavaSetClassFromType(ILWriter *writer, ILClass *info, ILType *type)
{
	JavaConstPool *constPool;
	ILUInt32 nameIndex;
	char *name = 0;

	constPool = JavaGetConstPool(info);
	if(!constPool)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	WriteJavaType(writer, type, &name);

	nameIndex = ILJavaSetUTF8String(writer, info, name, strlen(name));
	if(!nameIndex)
		return 0;

	ADD_POOL(CLASS, classValue, nameIndex, nameIndex, nameIndex, nameIndex);
}

ILUInt32 ILJavaSetClassFromName(ILWriter *writer, ILClass *info,
								const char *name)
{
	JavaConstPool *constPool;
	ILUInt32 nameIndex;

	constPool = JavaGetConstPool(info);
	if(!constPool)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	nameIndex = ILJavaSetUTF8String(writer, info, name, strlen(name));
	if(!nameIndex)
		return 0;

	ADD_POOL(CLASS, classValue, nameIndex, nameIndex, nameIndex, nameIndex);
}

ILUInt32 ILJavaSetNameAndType(ILWriter *writer, ILClass *info, 
								ILUInt32 nameIndex, ILUInt32 sigIndex)
{
	JavaConstPool *constPool;

	constPool = JavaGetConstPool(info);
	if(!constPool)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	ADD_POOL(NAMEANDTYPE, nameAndType, name, nameIndex, type, sigIndex);
}

ILUInt32 ILJavaSetref(ILWriter *writer, ILClass *info, int type, 
						ILClass *owner,char *name, ILType *sig)
{
	JavaConstPool *constPool;
	ILUInt32 nameIndex;
	ILUInt32 sigIndex;
	ILUInt32 classIndex;
	ILUInt32 nameAndType;

	constPool = JavaGetConstPool(info);
	if(!constPool)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	classIndex = ILJavaSetClass(writer, info, owner);
	name = ILMethodNameToJava(name);
	nameIndex = ILJavaSetUTF8String(writer, info, name, strlen(name));
	sigIndex = ILJavaSetSignature(writer, info, sig);
	nameAndType = ILJavaSetNameAndType(writer, info, nameIndex, sigIndex);

	if(type == JAVA_CONST_FIELDREF)
	{
		ADD_POOL(FIELDREF, refValue, classIndex, classIndex, nameAndType, 
				 nameAndType);
	}
	else if(type == JAVA_CONST_METHODREF)
	{
		ADD_POOL(METHODREF, refValue, classIndex, classIndex, nameAndType, 
				 nameAndType);
	}
	else
	{
		writer->writeFailed = 1;
		return 0;
	}
}

ILUInt32 ILJavaSetrefFromName(ILWriter *writer, ILClass *info, int type, 
							  const char *className, const char *refName,
							  const char *sigName)
{
	JavaConstPool *constPool;
	ILUInt32 refIndex;
	ILUInt32 sigIndex;
	ILUInt32 classIndex;
	ILUInt32 nameAndType;

	constPool = JavaGetConstPool(info);
	if(!constPool)
	{
		writer->outOfMemory = 1;
		return 0;
	}
	classIndex = ILJavaSetClassFromName(writer, info, className);
	refIndex = ILJavaSetUTF8String(writer, info, refName, strlen(refName));
	sigIndex = ILJavaSetUTF8String(writer, info, sigName, strlen(sigName));
	nameAndType = ILJavaSetNameAndType(writer, info, refIndex, sigIndex);

	if(type == JAVA_CONST_FIELDREF)
	{
		ADD_POOL(FIELDREF, refValue, classIndex, classIndex, nameAndType, 
				 nameAndType);
	}
	else if(type == JAVA_CONST_METHODREF)
	{
		ADD_POOL(METHODREF, refValue, classIndex, classIndex, nameAndType, 
				 nameAndType);
	}
	else
	{
		writer->writeFailed = 1;
		return 0;
	}
}

#define ILJAVA_SET(name, typeName, fieldName, constName)\
int ILJavaSet##name(ILWriter *writer, ILClass *info, typeName value)\
{\
	JavaConstPool *constPool;\
    ILUInt32 constPoolEntries;\
    JavaConstEntry *entries;\
    JavaConstEntry *poolEntry;\
    int index, index2;\
\
	constPool = JavaGetConstPool(info);\
	if(!constPool)\
	{\
		writer->outOfMemory = 1;\
		return 0;\
	}\
    constPoolEntries = constPool->size;\
    entries = constPool->entries;\
    for(index = 0; index < constPoolEntries; index++)\
    {\
        if(entries[index].type == JAVA_CONST_##constName &&\
            entries[index].un.fieldName == value)\
        {\
            return index;\
        }\
    }\
    poolEntry = (JavaConstEntry*)ILMalloc(sizeof(JavaConstEntry));\
\
    if(!poolEntry)\
    {\
        writer->outOfMemory = 1;\
        return 0;\
    }\
    poolEntry->type = JAVA_CONST_##constName;\
    poolEntry->un.fieldName = value;\
\
    index = ILJavaAddPool(writer, constPool, poolEntry);\
    /* add an empty pool entry  */\
    if(poolEntry->type == JAVA_CONST_LONG ||\
       poolEntry->type == JAVA_CONST_DOUBLE)\
    {\
        JavaConstEntry *emptyEntry;\
        emptyEntry = (JavaConstEntry*)ILMalloc(sizeof(JavaConstEntry));\
        if(!emptyEntry)\
        {\
            writer->outOfMemory = 1;\
            return 0;\
        }\
        emptyEntry->type = 0;\
        index2 = ILJavaAddPool(writer, constPool, emptyEntry);\
    }\
    return index;\
}

ILJAVA_SET(Integer, ILInt32,  intValue,    INTEGER)
ILJAVA_SET(Long,    ILInt64,  longValue,   LONG)
ILJAVA_SET(Float,   ILFloat,  floatValue,  FLOAT)
ILJAVA_SET(Double,  ILDouble, doubleValue, DOUBLE)
ILJAVA_SET(String,  ILInt32,  strValue,    STRING)

/*
 * Constant pool Output buffer
 */
static unsigned char *poolBuffer = 0;
static ILUInt32       poolOffset = 0;
static ILUInt32       poolLength = 0;

/*
 * Output a single byte to the Constant pool buffer.
 */
static void PoolOutByte(unsigned char byte)
{
    unsigned char *buf = (unsigned char *)ILRealloc(poolBuffer,
													poolLength + 1024);
    if(!buf)
    {
        /* ILAsmOutOfMemory(); */
		return;
    }
    poolBuffer = buf;
    poolLength += 1024;
    poolBuffer[poolOffset++] = byte;
}
#define POOL_OUT_BYTE(byte)	 \
			do { \
				if(poolOffset < poolLength) \
				{ \
					poolBuffer[poolOffset++] = (unsigned char)(byte); \
				} \
				else \
				{ \
					PoolOutByte((unsigned char)(byte)); \
				} \
			} while(0)

#define	POOL_OUT_UINT16(value)	\
			do { \
				POOL_OUT_BYTE((unsigned char)((value) >> 8)); \
				POOL_OUT_BYTE((unsigned char)(value)); \
			} while(0)

#define	POOL_OUT_UINT32(value)	\
			do { \
				POOL_OUT_BYTE((unsigned char)((value) >> 24)); \
				POOL_OUT_BYTE((unsigned char)((value) >> 16)); \
				POOL_OUT_BYTE((unsigned char)((value) >> 8)); \
				POOL_OUT_BYTE((unsigned char)(value)); \
			} while(0)

/*
 * Writes a single pool entry to the constant pool buffer
 */
static void WritePoolEntry(ILWriter *writer, JavaConstEntry *entry)
{
	int count;

	if(entry->type == 0)
	{
		return;
	}
	POOL_OUT_BYTE(entry->type);

	switch(entry->type)
	{
	case JAVA_CONST_UTF8:
	{
		POOL_OUT_UINT16(entry->length);
		count = 0;
		for(count = 0; count < entry->length; count++)
		{
			POOL_OUT_BYTE(entry->un.utf8String[count]);
		}
		ILFree(entry->un.utf8String);
	}
	break;

	case JAVA_CONST_INTEGER:
	{
		/* Write a 32-bit integer */
		POOL_OUT_UINT32(entry->un.intValue);
	}
	break;

	case JAVA_CONST_FLOAT:
	{
		/* Write a 32-bit floating point value */
		unsigned char buffer[4];
		for(count = 0; count < sizeof(buffer); count++)
		{
			POOL_OUT_BYTE(buffer[count]);
		}
	}
	break;

	case JAVA_CONST_LONG:
	{
		unsigned char buffer[8];
				
		for(count = 0; count < sizeof(buffer); count++)
		{
			POOL_OUT_BYTE(buffer[count]);
		}
	}
	break;

	case JAVA_CONST_DOUBLE:
	{
		/* Write a 64-bit double floating point value */
		unsigned char buffer[8];

		for(count = 0; count < sizeof(buffer); count++)
		{
			POOL_OUT_BYTE(buffer[count]);
		}
	}
	break;

	case JAVA_CONST_CLASS:
	{
		/* Write a class reference */
		POOL_OUT_UINT16(entry->un.classValue.nameIndex);
	}
	break;

	case JAVA_CONST_STRING:
	{
		/* Write a string constant */
		POOL_OUT_UINT16(entry->un.strValue);
	}
	break;

	case JAVA_CONST_FIELDREF:
	case JAVA_CONST_METHODREF:
	case JAVA_CONST_INTERFACEMETHODREF:
	{
		/* Write a field or method reference */

		POOL_OUT_UINT16(entry->un.refValue.classIndex);
		POOL_OUT_UINT16(entry->un.refValue.nameAndType);

	}
	break;

	case JAVA_CONST_NAMEANDTYPE:
	{
		/* Write a name and type specification */
		POOL_OUT_UINT16(entry->un.nameAndType.name);
		POOL_OUT_UINT16(entry->un.nameAndType.type);

	}
	break;

	default:
	{
		/* Error: unknown constant type */
		writer->writeFailed = 1;
		return;
	}
	/* Not reached */
	}
}

/*
 * Writes the whole constant pool of a given class
 */
void JavaWriteConstantPool(ILWriter *writer, JavaConstEntry *entry,
								  ILUInt32 poolSize)
{
	int i;

	POOL_OUT_BYTE(poolSize >> 8);
	POOL_OUT_BYTE(poolSize);

	for (i = 1; i < poolSize; i++)
	{
		WritePoolEntry(writer, &entry[i]);
	}
	ILWriterTextWrite(writer, poolBuffer, poolOffset);
}

/*
 * Class Output buffer
 */
static unsigned char *buffer = 0;
static ILUInt32       offset = 0;
static ILUInt32       length = 0;

/*
 * Output a single byte to the class buffer.
 */
static void OutByte(unsigned char byte)
{
    unsigned char *buf = (unsigned char *)ILRealloc(buffer, length + 1024);
    if(!buf)
    {
        /* ILAsmOutOfMemory(); */
		return;
    }
    buffer = buf;
    length += 1024;
    buffer[offset++] = byte;
}
#define OUT_BYTE(byte)  \
            do { \
                if(offset < length) \
                { \
                    buffer[offset++] = (unsigned char)(byte); \
                } \
                else \
                { \
                    OutByte((unsigned char)(byte)); \
                } \
            } while(0)

#define	OUT_UINT16(value)	\
			do { \
				OUT_BYTE((unsigned char)((value) >> 8)); \
				OUT_BYTE((unsigned char)(value)); \
			} while(0)

#define	OUT_UINT32(value)	\
			do { \
				OUT_BYTE((unsigned char)((value) >> 24)); \
				OUT_BYTE((unsigned char)((value) >> 16)); \
				OUT_BYTE((unsigned char)((value) >> 8)); \
				OUT_BYTE((unsigned char)(value)); \
			} while(0)

/*
 * Writes a java member: field or method
 */
static void WriteJavaMember(ILWriter *writer, ILClass *class, ILMember *member,
						   int isMethod)
{
	char *name;
	int index;
	int numMemberAttrs;
	int count;
	int i;

	/* Write member name */
	name = (char*)ILMember_Name(member);
	if(isMethod)
	{
		name = ILMethodNameToJava(name);
	}
	index = ILJavaSetUTF8String(writer, class, name, strlen(name));
	if(!index)
	{
		writer->writeFailed = 1;
		return;
	}
	OUT_UINT16(index);

	/* Write signature / field type */
	if(isMethod)
	{
		index = ILJavaSetSignature(writer, class, ILMember_Signature(member));
	}
	else
	{
		char *sigName = 0;

		WriteJavaType(writer, ILField_Type((ILField*)member), &sigName);
		index = ILJavaSetUTF8String(writer, class, sigName, strlen(sigName));
		ILFree(sigName);
	}
	if(!index)
	{
		writer->writeFailed = 1;
		return;
	}
	OUT_UINT16(index);

	/* Write method attributes */
	if(isMethod)
	{
		JavaConstEntry *entry = JavaGetConstEntry(class, 0);
		JavaCodeList *list;

		numMemberAttrs = 1; /* only code for the moment */
		OUT_UINT16(numMemberAttrs);

		/* finds the corresponding class */
		list = entry->un.codeList;
		while(list != 0)
		{
			if((ILMethod*)member == (ILMethod*)list->method)
				break;
			list = list->next;
		}
		if(!list)
		{
			writer->writeFailed = 1;
			return;
		}
		count = list->length;
		for (i = 0; i < count; i++)
		{
			OUT_BYTE(((char*)list->code)[i]);
		}
	}
	else
	{
		numMemberAttrs = 0; /* no attributes for fields yet */
		OUT_UINT16(numMemberAttrs);
	}
}

/*
 * Writes a complete Java class
 */
void WriteJavaClass(ILWriter *writer, ILClass *class)
{
	JavaConstPool *constPool;
	ILUInt32 accessFlags;
	ILUInt32 javaAccessFlags;
	ILUInt32 thisIndex;
	ILUInt32 parentIndex;
	ILClass *parentClass;
	ILImplements *impl;
	ILClass *interface;
	int count;
	ILMember *member;
	ILUInt16 numFields;
	ILUInt16 numMethods;
	ILUInt16 numClassAttrs;

	/* access flags */
	accessFlags = ILClassGetAttrs(class);
	javaAccessFlags = JAVA_ACC_SUPER;
	if((accessFlags & IL_META_TYPEDEF_PUBLIC) != 0)
	{
		javaAccessFlags |= JAVA_ACC_PUBLIC;
	}
	if((accessFlags & IL_META_TYPEDEF_NOT_PUBLIC) != 0)
	{
		javaAccessFlags |= (ILUInt32)JAVA_ACC_PRIVATE;
	}
	if((accessFlags & IL_META_TYPEDEF_SEALED) != 0)
	{
		javaAccessFlags |= JAVA_ACC_FINAL;
	}
	if((accessFlags & IL_META_TYPEDEF_INTERFACE) != 0)
	{
		javaAccessFlags |= JAVA_ACC_INTERFACE;
	}
	if((accessFlags & IL_META_TYPEDEF_ABSTRACT) != 0)
	{
		javaAccessFlags |= JAVA_ACC_ABSTRACT;
	}
	OUT_UINT16(javaAccessFlags);

	/* this class */
	thisIndex = ILJavaSetClass(writer, class, class);
	OUT_UINT16(thisIndex);

	/* parent class */
	parentClass = ILClass_ParentClass(class);
	if(parentClass)
	{
		parentIndex = ILJavaSetClass(writer, class, parentClass);
	}
	else
	{
		/* if class is System.Object, set java.lang.Object as parent */
		if(!strcmp(ILClass_Namespace(class), "System") &&
		   !strcmp(ILClass_Name(class), "Object"))
		{
			parentIndex = ILJavaSetClassFromName(writer, class, 
												 "java/lang/Object");
		} 
		else
		{
			parentIndex = 0;
		}
	}
	OUT_UINT16(parentIndex);

	/* implemented interfaces */
	impl = _ILClass_Implements(class);
	count = 0;
	while(impl)
	{
		count++;
		impl = _ILImplements_NextImplements(impl);
	}
	OUT_UINT16(count);
	impl = _ILClass_Implements(class);
	while(impl)
	{
		int index;

		interface = ILImplements_InterfaceClass(impl);
		index = ILJavaSetClass(writer, class, interface);
		if(!index)
		{
			writer->writeFailed = 1;
			goto cleanup;
		}
		OUT_UINT16(index);
		impl = _ILImplements_NextImplements(impl);
	}

	/* Dump the class members */
	member = 0;
	numFields = 0;
	numMethods = 0;
	while((member = ILClassNextMember(class, member)) != 0)
	{
		switch(ILMemberGetKind(member))
		{
			case IL_META_MEMBERKIND_METHOD:
			{
				numMethods++;
			}
			break;

			case IL_META_MEMBERKIND_FIELD:
			{
				numFields++;
			}
			break;

			case IL_META_MEMBERKIND_EVENT:
			{
			}
			break;

			case IL_META_MEMBERKIND_PROPERTY:
			{
			}
			break;
		}
	}

	/* fields count */
	OUT_UINT16(numFields);
	member = 0;
	while((member = ILClassNextMember(class, member)) != 0)
	{
		if(ILMemberGetKind(member) == IL_META_MEMBERKIND_FIELD)
		{
			/* TODO: member constants */

			/* Write the field access flags */

			accessFlags = ILMember_Attrs(member);
			if((accessFlags & IL_META_FIELDDEF_PUBLIC) != 0)
			{
				javaAccessFlags = JAVA_ACC_PUBLIC;
			}
			else if((accessFlags & IL_META_FIELDDEF_PRIVATE) != 0)
			{
				javaAccessFlags = JAVA_ACC_PRIVATE;
			}
			else if((accessFlags & IL_META_FIELDDEF_FAMILY) != 0)
			{
				javaAccessFlags = JAVA_ACC_PROTECTED;
			}
			else if((accessFlags & IL_META_FIELDDEF_ASSEMBLY) != 0)
			{
				javaAccessFlags = 0;
			} else {
				javaAccessFlags = 0;
			}

			if((accessFlags & IL_META_FIELDDEF_STATIC) != 0)
			{
				javaAccessFlags |= JAVA_ACC_STATIC;
			}
			if((accessFlags & IL_META_FIELDDEF_INIT_ONLY) != 0)
			{
				javaAccessFlags |= JAVA_ACC_FINAL;
			}
			if((accessFlags & IL_META_FIELDDEF_NOT_SERIALIZED) != 0)
			{
				javaAccessFlags |= JAVA_ACC_TRANSIENT;
			}

			OUT_UINT16(javaAccessFlags);
			WriteJavaMember(writer, class, member, 0);
			if(writer->writeFailed)
				goto cleanup;
		}
	}
	OUT_UINT16(numMethods);
	member = 0;
	while((member = ILClassNextMember(class, member)) != 0)
	{
		if(ILMemberGetKind(member) == IL_META_MEMBERKIND_METHOD)
		{
			/* Write the method access flags */
			accessFlags = ILMember_Attrs(member);
			if((accessFlags & IL_META_METHODDEF_PUBLIC) != 0)
			{
				javaAccessFlags = JAVA_ACC_PUBLIC;
			}
			else if((accessFlags & IL_META_METHODDEF_PRIVATE) != 0)
			{
				javaAccessFlags = JAVA_ACC_PRIVATE;
			}
			else if((accessFlags & IL_META_METHODDEF_FAMILY) != 0)
			{
				javaAccessFlags = JAVA_ACC_PROTECTED;
			}
			else if((accessFlags & IL_META_METHODDEF_ASSEM) != 0)
			{
				javaAccessFlags = 0; /* is this correct ? */
			}
			else
			{
				javaAccessFlags = 0;
			}
			if((accessFlags & IL_META_METHODDEF_STATIC) != 0)
			{
				javaAccessFlags |= JAVA_ACC_STATIC;
			}
			if((accessFlags & IL_META_METHODDEF_FINAL) != 0)
			{
				javaAccessFlags |= JAVA_ACC_FINAL;
			}
			if((accessFlags & (IL_META_METHODIMPL_SYNCHRONIZED << 16)) != 0)
			{
				javaAccessFlags |= JAVA_ACC_SYNCHRONIZED;
			}
			if((accessFlags & (IL_META_METHODIMPL_INTERNAL_CALL << 16)) != 0)
			{
				javaAccessFlags |= JAVA_ACC_NATIVE;
			}
			if((accessFlags & IL_META_METHODDEF_ABSTRACT) != 0)
			{
				javaAccessFlags |= JAVA_ACC_ABSTRACT;
			}
			if((accessFlags & (IL_META_METHODIMPL_JAVA_FP_STRICT << 16)) != 0)
			{
				javaAccessFlags |= JAVA_ACC_STRICT;
			}

			/* Write the method information */
			OUT_UINT16(javaAccessFlags);
			WriteJavaMember(writer, class, member, 1);
			if(writer->writeFailed)
				goto cleanup;
		}
	}
	/* Writes class attributes */

	/* TODO */
	numClassAttrs = 0;
	OUT_UINT16(numClassAttrs);


	constPool = JavaGetConstPool(class);
	if(constPool)
	{
		JavaWriteConstantPool(writer, constPool->entries,
							  constPool->size);
	}
	ILWriterTextWrite(writer, buffer, offset);

cleanup:
	offset = 0;
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_CONFIG_JAVA */
