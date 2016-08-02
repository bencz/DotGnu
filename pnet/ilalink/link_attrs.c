/*
 * link_attrs.c - Convert custom attributes and copy them to the final image.
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

#include "linker.h"

#ifdef	__cplusplus
extern	"C" {
#endif

int _ILLinkerConvertAttrs(ILLinker *linker, ILProgramItem *oldItem,
						  ILProgramItem *newItem)
{
	ILAttribute *attr;
	ILAttribute *newAttr;
	ILProgramItem *item;
	ILMethod *method;
	const void *blob;
	ILUInt32 blobLen;

	/* Scan through the attributes on the old item */
	attr = 0;
	while((attr = ILProgramItemNextAttribute(oldItem, attr)) != 0)
	{
		/* Create the new attribute block */
		newAttr = ILAttributeCreate(linker->image, 0);
		if(!newAttr)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		ILProgramItemAddAttribute(newItem, newAttr);

		/* Determine how to convert the attribute's type */
		item = ILAttributeTypeAsItem(attr);
		if(item)
		{
			method = ILProgramItemToMethod(item);
			if(method)
			{
				method = (ILMethod *)_ILLinkerConvertMemberRef
							(linker, (ILMember *)method);
				if(!method)
				{
					_ILLinkerOutOfMemory(linker);
					return 0;
				}
				ILAttributeSetType(newAttr, (ILProgramItem *)method);
			}
		}

		/* Copy the attribute's value */
		blob = ILAttributeGetValue(attr, &blobLen);
		if(blob)
		{
			if(!ILAttributeSetValue(newAttr, blob, blobLen))
			{
				_ILLinkerOutOfMemory(linker);
				return 0;
			}
		}
	}

	/* Done */
	return 1;
}

int _ILLinkerConvertSecurity(ILLinker *linker, ILProgramItem *oldItem,
						     ILProgramItem *newItem)
{
	ILDeclSecurity *decl;

	/* Get the security declaration from the old item */
	decl = 0;
	while((decl = ILProgramItemNextDeclSecurity(oldItem, decl)) != 0)
	{
		ILDeclSecurity *newDecl;
		const void *blob;
		ILUInt32 blobLen;

		/* Create a security declaration on the new item */
		newDecl = ILDeclSecurityCreate(linker->image, 0, newItem,
									   ILDeclSecurity_Type(decl));
		if(!newDecl)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}

		/* Copy the security blob */
		blob = ILDeclSecurityGetBlob(decl, &blobLen);
		if(blob)
		{
			if(!ILDeclSecuritySetBlob(newDecl, blob, blobLen))
			{
				_ILLinkerOutOfMemory(linker);
				return 0;
			}
		}
	}

	/* Done */
	return 1;
}

int _ILLinkerConvertDebug(ILLinker *linker, ILProgramItem *oldItem,
						  ILProgramItem *newItem)
{
	ILDebugContext *dbg;
	ILDebugIter iter;
	ILMetaDataRead reader;
	unsigned char buf[1024];
	ILUInt32 len;
	const char *name;
	ILUInt32 nameIndex;
	ILUInt32 temp1;
	ILUInt32 temp2;
	ILUInt32 temp3;

	/* Bail out if no debug information for the old image */
	if(!ILDebugPresent(ILProgramItem_Image(oldItem)))
	{
		return 1;
	}

	/* Create a debug context for the old image */
	dbg = ILDebugCreate(ILProgramItem_Image(oldItem));
	if(!dbg)
	{
		return 0;
	}

	/* Scan through all debug blocks for the old item */
	ILDebugIterInit(&iter, dbg, ILProgramItem_Token(oldItem));
	while(ILDebugIterNext(&iter))
	{
		/* Initalize a reader for the block */
		reader.data = iter.data;
		reader.len = iter.length;
		reader.error = 0;

		/* Convert the contents of the block */
		switch(iter.type)
		{
			case IL_DEBUGTYPE_LINE_COL:
			case IL_DEBUGTYPE_LINE_OFFSETS:
			{
				/* Line and column/offset information */
				name = ILDebugGetString(dbg, ILMetaUncompressData(&reader));
				if(!name)
				{
					break;
				}
				nameIndex = ILWriterDebugString(linker->writer, name);
				len = 0;
				while(!(reader.error) && reader.len > 0)
				{
					temp1 = ILMetaUncompressData(&reader);
					temp2 = ILMetaUncompressData(&reader);
					if(!len)
					{
						len = ILMetaCompressData(buf, nameIndex);
					}
					len += ILMetaCompressData(buf + len, temp1);
					len += ILMetaCompressData(buf + len, temp2);
					if(len >= (sizeof(buf) - IL_META_COMPRESS_MAX_SIZE * 3))
					{
						ILWriterDebugAdd(linker->writer, newItem,
										 (int)(iter.type), buf, len);
						len = 0;
					}
				}
				if(len > 0)
				{
					ILWriterDebugAdd(linker->writer, newItem,
									 (int)(iter.type), buf, len);
				}
			}
			break;

			case IL_DEBUGTYPE_LINE_COL_OFFSETS:
			{
				/* Line, column, and offset information */
				name = ILDebugGetString(dbg, ILMetaUncompressData(&reader));
				if(!name)
				{
					break;
				}
				nameIndex = ILWriterDebugString(linker->writer, name);
				len = 0;
				while(!(reader.error) && reader.len > 0)
				{
					temp1 = ILMetaUncompressData(&reader);
					temp2 = ILMetaUncompressData(&reader);
					temp3 = ILMetaUncompressData(&reader);
					if(!len)
					{
						len = ILMetaCompressData(buf, nameIndex);
					}
					len += ILMetaCompressData(buf + len, temp1);
					len += ILMetaCompressData(buf + len, temp2);
					len += ILMetaCompressData(buf + len, temp3);
					if(len >= (sizeof(buf) - IL_META_COMPRESS_MAX_SIZE * 4))
					{
						ILWriterDebugAdd(linker->writer, newItem,
										 (int)(iter.type), buf, len);
						len = 0;
					}
				}
				if(len > 0)
				{
					ILWriterDebugAdd(linker->writer, newItem,
									 (int)(iter.type), buf, len);
				}
			}
			break;

			case IL_DEBUGTYPE_VARS:
			{
				/* Local variable information with no scope */
				len = 0;
				while(!(reader.error) && reader.len > 0)
				{
					name = ILDebugGetString(dbg, ILMetaUncompressData(&reader));
					if(!name)
					{
						break;
					}
					nameIndex = ILWriterDebugString(linker->writer, name);
					temp1 = ILMetaUncompressData(&reader);
					len += ILMetaCompressData(buf + len, nameIndex);
					len += ILMetaCompressData(buf + len, temp1);
					if(len >= (sizeof(buf) - IL_META_COMPRESS_MAX_SIZE * 2))
					{
						ILWriterDebugAdd(linker->writer, newItem,
										 (int)(iter.type), buf, len);
						len = 0;
					}
				}
				if(len > 0)
				{
					ILWriterDebugAdd(linker->writer, newItem,
									 (int)(iter.type), buf, len);
				}
			}
			break;

			case IL_DEBUGTYPE_VARS_OFFSETS:
			{
				/* Local variable information with specified scope */
				temp1 = ILMetaUncompressData(&reader);
				temp2 = ILMetaUncompressData(&reader);
				len = 0;
				while(!(reader.error) && reader.len > 0)
				{
					name = ILDebugGetString(dbg, ILMetaUncompressData(&reader));
					if(!name)
					{
						break;
					}
					nameIndex = ILWriterDebugString(linker->writer, name);
					temp3 = ILMetaUncompressData(&reader);
					if(!len)
					{
						len += ILMetaCompressData(buf + len, temp1);
						len += ILMetaCompressData(buf + len, temp2);
					}
					len += ILMetaCompressData(buf + len, nameIndex);
					len += ILMetaCompressData(buf + len, temp3);
					if(len >= (sizeof(buf) - IL_META_COMPRESS_MAX_SIZE * 4))
					{
						ILWriterDebugAdd(linker->writer, newItem,
										 (int)(iter.type), buf, len);
						len = 0;
					}
				}
				if(len > 0)
				{
					ILWriterDebugAdd(linker->writer, newItem,
									 (int)(iter.type), buf, len);
				}
			}
			break;
		}
	}

	/* Done */
	ILDebugDestroy(dbg);
	return 1;
}

#ifdef	__cplusplus
};
#endif
