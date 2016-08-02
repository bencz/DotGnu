/*
 * il_serialize.h - Routines for serializing attribute values.
 *
 * Copyright (C) 2001, 2002, 2009  Southern Storm Software, Pty Ltd.
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

#ifndef	_IL_SERIALIZE_H
#define	_IL_SERIALIZE_H

#include "il_program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Special serialization flag value that is used to mark arrays.
 */
#define	IL_META_SERIALTYPE_ARRAYOF		0x80

/*
 * Opaque definitions of ILSerializeReader and ILSerializeWriter.
 */
typedef struct _tagILSerializeReader ILSerializeReader;
typedef struct _tagILSerializeWriter ILSerializeWriter;

/*
 * Initialize a serialization reader for extracting values
 * from a custom attribute blob for a particular method.
 */
ILSerializeReader *ILSerializeReaderInit(ILMethod *method,
									     const void *blob,
										 ILUInt32 len);

/*
 * Destroy a serialization reader's temporary storage.
 */
void ILSerializeReaderDestroy(ILSerializeReader *reader);

/*
 * Get the serialization type associated with a real type.
 * Returns -1 if the type is not serializable.
 */
int ILSerializeGetType(ILType *type);

/*
 * Get the type of the next parameter, which will be one
 * of the "IL_META_SERIALTYPE_*" values.  This will advance
 * the parameter count.  Returns 0 if there are no more
 * parameters, or -1 if the type is not serializable, or
 * there are insufficient bytes for the value.
 */
int ILSerializeReaderGetParamType(ILSerializeReader *reader);

/*
 * Get a signed serialization value of 32 bits or less.
 */
ILInt32 ILSerializeReaderGetInt32(ILSerializeReader *reader, int type);

/*
 * Get an unsigned serialization value of 32 bits or less.
 */
ILUInt32 ILSerializeReaderGetUInt32(ILSerializeReader *reader, int type);

/*
 * Get a signed 64-bit serialization value.
 */
ILInt64 ILSerializeReaderGetInt64(ILSerializeReader *reader);

/*
 * Get an unsigned 64-bit serialization value.
 */
ILUInt64 ILSerializeReaderGetUInt64(ILSerializeReader *reader);

/*
 * Get a 32-bit floating-point serialization value.
 */
ILFloat ILSerializeReaderGetFloat32(ILSerializeReader *reader);

/*
 * Get a 64-bit floating-point serialization value.
 */
ILDouble ILSerializeReaderGetFloat64(ILSerializeReader *reader);

/*
 * Get a string or type name serialization value.  Returns the
 * length in UTF-8 bytes, or -1 if the blob is badly formatted.
 */
int ILSerializeReaderGetString(ILSerializeReader *reader, const char **str);

/*
 * Get the length of an array.
 */
ILInt32 ILSerializeReaderGetArrayLen(ILSerializeReader *reader);

/*
 * Get the number of extra fields or properties that are specified.
 * Returns -1 if the blob is badly formatted.
 */
int ILSerializeReaderGetNumExtra(ILSerializeReader *reader);

/*
 * Get the next field or property block from a serialization blob.
 * Returns the type of the field, or -1 if the blob is badly formatted.
 * The "member" may be NULL if it could not be fully resolved.
 */
int ILSerializeReaderGetExtra(ILSerializeReader *reader, ILMember **member,
						      const char **name, int *nameLen);

/*
 * Get the boxed prefix header from a serialization blob. Return the
 * serialType on success or -1 on error.
 */
int ILSerializeReaderGetBoxedPrefix(ILSerializeReader *reader);

/*
 * Initialize a serialization writer for writing values
 * to a custom attribute blob.
 */
ILSerializeWriter *ILSerializeWriterInit(void);

/*
 * Destroy a serialization writer's temporary storage.
 */
void ILSerializeWriterDestroy(ILSerializeWriter *writer);

/*
 * Get the final blob value from a serialization writer.
 * Returns NULL if out of memory.
 */
const void *ILSerializeWriterGetBlob(ILSerializeWriter *writer,
									 ILUInt32 *blobLen);

/*
 * Write an integer value of 32 bits or less to a writer.
 */
void ILSerializeWriterSetInt32(ILSerializeWriter *writer,
							   ILInt32 value, int type);

/*
 * Write an unsigned integer value of 32 bits or less to a writer.
 */
void ILSerializeWriterSetUInt32(ILSerializeWriter *writer,
							    ILUInt32 value, int type);

/*
 * Write an integer value of 64 bits in size to a writer.
 */
void ILSerializeWriterSetInt64(ILSerializeWriter *writer, ILInt64 value);

/*
 * Write an unsigned integer value of 64 bits in size to a writer.
 */
void ILSerializeWriterSetUInt64(ILSerializeWriter *writer, ILUInt64 value);

/*
 * Write a 32-bit floating point value to a writer.
 */
void ILSerializeWriterSetFloat32(ILSerializeWriter *writer, ILFloat value);

/*
 * Write a 64-bit floating point value to a writer.
 */
void ILSerializeWriterSetFloat64(ILSerializeWriter *writer, ILDouble value);

/*
 * Write a string value to a writer.
 */
void ILSerializeWriterSetString(ILSerializeWriter *writer,
								const char *str, int len);

/*
 * Write the number of extra fields to a writer.
 */
void ILSerializeWriterSetNumExtra(ILSerializeWriter *writer, int num);

/*
 * Write the name of a extra field to a writer.
 */
void ILSerializeWriterSetField(ILSerializeWriter *writer,
							   const char *name, int type);

/*
 * Write the name of a extra property to a writer.
 */
void ILSerializeWriterSetProperty(ILSerializeWriter *writer,
								  const char *name, int type);

/*
 * Write a boxed prefix header to the serialize stream
 */
void ILSerializeWriterSetBoxedPrefix(ILSerializeWriter *writer, int type);


#ifdef	__cplusplus
};
#endif

#endif	/* _IL_SERIALIZE_H */
