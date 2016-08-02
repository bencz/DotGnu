/*
 * ser_writer.c - Write serialized attribute values.
 *
 * Copyright (C) 2002, 2009  Southern Storm Software, Pty Ltd.
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

#include "il_serialize.h"
#include "program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Internal structure of a serialization writer.
 */
struct _tagILSerializeWriter
{
	unsigned char	   *blob;
	int					blobLen;
	int					blobMax;
	int					outOfMemory;

};

/*
 * Get some space in a serialization writer's blob.
 */
static unsigned char *GetSpace(ILSerializeWriter *writer, int len)
{
	unsigned char *buf;
	int temp;
	if(writer->outOfMemory)
	{
		return 0;
	}
	if((writer->blobLen + len) > writer->blobMax)
	{
		temp = (writer->blobLen + len + 31) & ~31;
		if((buf = (unsigned char *)ILRealloc(writer->blob, temp)) == 0)
		{
			writer->outOfMemory = 1;
			return 0;
		}
		writer->blob = buf;
		writer->blobMax = temp;
	}
	buf = writer->blob + writer->blobLen;
	writer->blobLen += len;
	return buf;
}

ILSerializeWriter *ILSerializeWriterInit(void)
{
	ILSerializeWriter *writer;

	/* Allocate space for the writer */
	writer = (ILSerializeWriter *)ILMalloc(sizeof(ILSerializeWriter));
	if(!writer)
	{
		return 0;
	}

	/* Initialize the writer */
	writer->blob = 0;
	writer->blobLen = 0;
	writer->blobMax = 0;
	writer->outOfMemory = 0;

	/* The writer is ready to go */
	return writer;
}

void ILSerializeWriterDestroy(ILSerializeWriter *writer)
{
	if(writer->blob)
	{
		ILFree(writer->blob);
	}
	ILFree(writer);
}

const void *ILSerializeWriterGetBlob(ILSerializeWriter *writer,
									 ILUInt32 *blobLen)
{
	*blobLen = writer->blobLen;
	return writer->blob;
}

void ILSerializeWriterSetInt32(ILSerializeWriter *writer,
							   ILInt32 value, int type)
{
	unsigned char *buf;
	switch(type)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		case IL_META_SERIALTYPE_I1:
		case IL_META_SERIALTYPE_U1:
		{
			buf = GetSpace(writer, 1);
			if(buf)
			{
				*buf = (unsigned char)value;
			}
		}
		break;

		case IL_META_SERIALTYPE_I2:
		case IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_CHAR:
		{
			buf = GetSpace(writer, 2);
			if(buf)
			{
				IL_WRITE_UINT16(buf, (ILUInt16)value);
			}
		}
		break;

		case IL_META_SERIALTYPE_I4:
		case IL_META_SERIALTYPE_U4:
		{
			buf = GetSpace(writer, 4);
			if(buf)
			{
				IL_WRITE_INT32(buf, value);
			}
		}
		break;
	}
}

void ILSerializeWriterSetUInt32(ILSerializeWriter *writer,
							    ILUInt32 value, int type)
{
	return ILSerializeWriterSetInt32(writer, (ILInt32)value, type);
}

void ILSerializeWriterSetInt64(ILSerializeWriter *writer, ILInt64 value)
{
	unsigned char *buf = GetSpace(writer, 8);
	if(buf)
	{
		IL_WRITE_INT64(buf, value);
	}
}

void ILSerializeWriterSetUInt64(ILSerializeWriter *writer, ILUInt64 value)
{
	return ILSerializeWriterSetInt64(writer, (ILInt64)value);
}

void ILSerializeWriterSetFloat32(ILSerializeWriter *writer, ILFloat value)
{
#if IL_CONFIG_FP_SUPPORTED
	unsigned char *buf = GetSpace(writer, 4);
	if(buf)
	{
		IL_WRITE_FLOAT(buf, value);
	}
#endif
}

void ILSerializeWriterSetFloat64(ILSerializeWriter *writer, ILDouble value)
{
#if IL_CONFIG_FP_SUPPORTED
	unsigned char *buf = GetSpace(writer, 8);
	if(buf)
	{
		IL_WRITE_DOUBLE(buf, value);
	}
#endif
}

void ILSerializeWriterSetString(ILSerializeWriter *writer,
								const char *str, int len)
{
	if(str)
	{
		unsigned char header[IL_META_COMPRESS_MAX_SIZE];
		int headerLen = ILMetaCompressData(header, (ILUInt32)len);
		unsigned char *buf = GetSpace(writer, headerLen + len);
		if(buf)
		{
			ILMemCpy(buf, header, headerLen);
			ILMemCpy(buf + headerLen, str, len);
		}
	}
	else
	{
		/* Encode the null string in the output */
		unsigned char *buf = GetSpace(writer, 1);
		*buf = (unsigned char)0xFF;
	}
}

void ILSerializeWriterSetNumExtra(ILSerializeWriter *writer, int num)
{
	ILSerializeWriterSetInt32(writer, (ILInt32)num, IL_META_SERIALTYPE_U2);
}

void ILSerializeWriterSetField(ILSerializeWriter *writer,
							   const char *name, int type)
{
	unsigned char *buf = GetSpace(writer, 2);
	if(buf)
	{
		buf[0] = IL_META_SERIALTYPE_FIELD;
		buf[1] = (unsigned char)type;
		ILSerializeWriterSetString(writer, name, strlen(name));
	}
}

void ILSerializeWriterSetProperty(ILSerializeWriter *writer,
								  const char *name, int type)
{
	unsigned char *buf = GetSpace(writer, 2);
	if(buf)
	{
		buf[0] = IL_META_SERIALTYPE_PROPERTY;
		buf[1] = (unsigned char)type;
		ILSerializeWriterSetString(writer, name, strlen(name));
	}
}

/* Boxed function for the object coercions */

void ILSerializeWriterSetBoxedPrefix(ILSerializeWriter *writer,int type)
{
	unsigned char *buf;
	buf = GetSpace(writer, 1);
	if(buf)
	{
		*buf = type;
	}
}

#ifdef	__cplusplus
};
#endif
