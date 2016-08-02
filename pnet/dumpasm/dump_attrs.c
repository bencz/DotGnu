/*
 * dump_attrs.c - Dump custom attributes.
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

#include "il_dumpasm.h"
#include "il_serialize.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Dump a serialized attribute value.
 */
static int DumpAttrValue(FILE *outstream, ILSerializeReader *reader, int type)
{
	ILInt32 intValue;
	ILUInt32 uintValue;
	ILInt64 longValue;
	ILUInt64 ulongValue;
	ILFloat floatValue;
	ILDouble doubleValue;
	const char *strValue;
	int strLen;

	switch(type)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		{
			intValue = ILSerializeReaderGetInt32(reader, type);
			if(intValue)
			{
				fputs("true", outstream);
			}
			else
			{
				fputs("false", outstream);
			}
		}
		break;

		case IL_META_SERIALTYPE_I1:
		case IL_META_SERIALTYPE_U1:
		{
			intValue = ILSerializeReaderGetInt32(reader, type);
			fprintf(outstream, "%ld /*0x%02lX*/",
					(long)intValue, (long)intValue);
		}
		break;

		case IL_META_SERIALTYPE_I2:
		case IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_CHAR:
		{
			intValue = ILSerializeReaderGetInt32(reader, type);
			fprintf(outstream, "%ld /*0x%04lX*/",
					(long)intValue, (long)intValue);
		}
		break;

		case IL_META_SERIALTYPE_I4:
		{
			intValue = ILSerializeReaderGetInt32(reader, type);
			fprintf(outstream, "%ld /*0x%08lX*/",
					(long)intValue, (long)intValue);
		}
		break;

		case IL_META_SERIALTYPE_U4:
		{
			uintValue = ILSerializeReaderGetUInt32(reader, type);
			fprintf(outstream, "%lu /*0x%08lX*/",
					(unsigned long)uintValue, (unsigned long)uintValue);
		}
		break;

		case IL_META_SERIALTYPE_I8:
		{
			longValue = ILSerializeReaderGetInt64(reader);
			fprintf(outstream, "0x%08lX%08lX",
					(unsigned long)((longValue >> 32) & IL_MAX_UINT32),
					(unsigned long)(longValue & IL_MAX_UINT32));
		}
		break;

		case IL_META_SERIALTYPE_U8:
		{
			ulongValue = ILSerializeReaderGetUInt64(reader);
			fprintf(outstream, "0x%08lX%08lX",
					(unsigned long)((ulongValue >> 32) & IL_MAX_UINT32),
					(unsigned long)(ulongValue & IL_MAX_UINT32));
		}
		break;

		case IL_META_SERIALTYPE_R4:
		{
			floatValue = ILSerializeReaderGetFloat32(reader);
			fprintf(outstream, "%.30e", (double)floatValue);
		}
		break;

		case IL_META_SERIALTYPE_R8:
		{
			doubleValue = ILSerializeReaderGetFloat64(reader);
			fprintf(outstream, "%.30e", (double)doubleValue);
		}
		break;

		case IL_META_SERIALTYPE_STRING:
		{
			strLen = ILSerializeReaderGetString(reader, &strValue);
			if(strLen == -1)
			{
				return 0;
			}
			if(strValue)
			{
				ILDumpStringLen(outstream, strValue, strLen);
			}
			else
			{
				fputs("null", outstream);
			}
		}
		break;

		case IL_META_SERIALTYPE_TYPE:
		{
			strLen = ILSerializeReaderGetString(reader, &strValue);
			if(strLen == -1)
			{
				return 0;
			}
			fputs("typeof(", outstream);
			if(strValue)
			{
				fwrite(strValue, 1, strLen, outstream);
			}
			else
			{
				fputs("null", outstream);
			}
			putc(')', outstream);
		}
		break;

		default:
		{
			if((type & IL_META_SERIALTYPE_ARRAYOF) != 0)
			{
				intValue = ILSerializeReaderGetArrayLen(reader);
				putc('{', outstream);
				while(intValue > 0)
				{
					if(!DumpAttrValue(outstream, reader,
									  type & ~IL_META_SERIALTYPE_ARRAYOF))
					{
						return 0;
					}
					--intValue;
					if(intValue > 0)
					{
						fputs(", ", outstream);
					}
				}
				putc('}', outstream);
			}
			else
			{
				return 0;
			}
		}
		break;
	}
	return 1;
}

/*
 * Dump the readable form of an attribute blob.
 */
static void DumpAttrBlob(FILE *outstream, ILImage *image, ILMethod *method,
						 const void *blob, ILUInt32 blobLen)
{
	ILClass *classInfo;
	const char *name;
	int nameLen;
	ILSerializeReader *reader;
	ILUInt32 numParams;
	int numExtra;
	int type, needComma;
	ILMember *member;

	/* Dump the name of the attribute */
	classInfo = ILMethod_Owner(method);
	name = ILClass_Name(classInfo);
	nameLen = strlen(name);
	if(!strcmp(name + nameLen - 9, "Attribute"))
	{
		fwrite(name, 1, nameLen - 9, outstream);
	}
	else
	{
		fputs(name, outstream);
	}

	/* Initialize the serialization reader */
	reader = ILSerializeReaderInit(method, blob, blobLen);
	if(!reader)
	{
		fputs("(?)", outstream);
		return;
	}

	/* Dump the parameters */
	numParams = ILTypeNumParams(ILMethod_Signature(method));
	needComma = 0;
	putc('(', outstream);
	while(numParams > 0)
	{
		if(needComma)
		{
			fputs(", ", outstream);
		}
		else
		{
			needComma = 1;
		}
		type = ILSerializeReaderGetParamType(reader);
		if(type != -1)
		{
			if(!DumpAttrValue(outstream, reader, type))
			{
				ILSerializeReaderDestroy(reader);
				return;
			}
		}
		else
		{
			fputs("?)", outstream);
			ILSerializeReaderDestroy(reader);
			return;
		}
		--numParams;
	}
	putc(')', outstream);

	/* Dump the extra field and property specifications */
	numExtra = ILSerializeReaderGetNumExtra(reader);
	while(numExtra > 0)
	{
		fputs(", ", outstream);
		type = ILSerializeReaderGetExtra(reader, &member, &name, &nameLen);
		if(type == -1)
		{
			putc('?', outstream);
			break;
		}
		fwrite(name, 1, nameLen, outstream);
		putc('=', outstream);
		if(!DumpAttrValue(outstream, reader, type))
		{
			putc('?', outstream);
			break;
		}
		--numExtra;
	}

	/* Clean up and exit */
	ILSerializeReaderDestroy(reader);
}

void ILDAsmDumpCustomAttrs(ILImage *image, FILE *outstream, int flags,
						   int indent, ILProgramItem *item)
{
	ILAttribute *attr = 0;
	ILProgramItem *type;
	const void *value;
	ILUInt32 valueLen;
	ILClass *classInfo;
	ILMethod *method;
	ILTypeSpec *spec;
	ILType *rawType;

	while((attr = ILProgramItemNextAttribute(item, attr)) != 0)
	{
		/* Output the ".custom" header */
		if(indent == 1)
		{
			fputs("\t", outstream);
		}
		else if(indent == 2)
		{
			fputs("\t\t", outstream);
		}
		fputs(".custom ", outstream);
		if((flags & IL_DUMP_SHOW_TOKENS) != 0)
		{
			fprintf(outstream, "/*%08lX*/ ",
					(long)(ILProgramItem_Token(attr)));
		}

		/* Output the type */
		method = 0;
		if(ILAttributeTypeIsItem(attr))
		{
			type = ILAttributeTypeAsItem(attr);
			if((spec = ILProgramItemToTypeSpec(type)) != 0)
			{
				rawType = ILTypeSpec_Type(spec);
				ILDumpType(outstream, image, rawType, flags);
			}
			else if((classInfo = ILProgramItemToClass(type)) != 0)
			{
				ILDumpClassName(outstream, image, classInfo, flags);
			}
			else if((method = ILProgramItemToMethod(type)) != 0)
			{
				ILDumpMethodType(outstream, image,
								 ILMethod_Signature(method), flags,
					 			 ILMethod_Owner(method),
								 ILMethod_Name(method),
								 method);
			}
			else
			{
				fputs("UNKNOWNTYPE", outstream);
			}
		}
		else
		{
			fputs("STRING", outstream);
		}

		/* Output the value */
		if((value = ILAttributeGetValue(attr, &valueLen)) != 0)
		{
			fputs(" =", outstream);
			ILDAsmDumpBinaryBlob(outstream, image, value, valueLen);
		}

		/* Terminate the line */
		putc('\n', outstream);

		/* Output a readable version of the value */
		if(value && method)
		{
			if(indent == 1)
			{
				fputs("\t", outstream);
			}
			else if(indent == 2)
			{
				fputs("\t\t", outstream);
			}
			fputs("// ", outstream);
			DumpAttrBlob(outstream, image, method, value, valueLen);
			putc('\n', outstream);
		}
	}
}

#ifdef	__cplusplus
};
#endif
