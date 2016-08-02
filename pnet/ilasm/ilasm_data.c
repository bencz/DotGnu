/*
 * ilasm_data.c - Handle ".data" sections.
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

#include "il_system.h"
#include "il_utils.h"
#include "ilasm_build.h"
#include "ilasm_output.h"
#include "ilasm_data.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Imports.
 */
void ILAsmPrintMessage(const char *filename, long linenum,
					   const char *format, ...);
void ILAsmOutOfMemory(void);
extern char *ILAsmFilename;
extern long  ILAsmLineNum;
extern int   ILAsmErrors;

static int sectionIsData = 1;
static ILUInt32 dataOffset = 0;
static ILUInt32 tlsOffset = 0;
static ILHashTable *nameTable = 0;

void ILAsmDataReset(void)
{
	sectionIsData = 1;
	dataOffset = 0;
	tlsOffset = 0;
	if(nameTable != 0)
	{
		ILHashDestroy(nameTable);
		nameTable = 0;
	}
}

void ILAsmDataSetNormal(void)
{
	sectionIsData = 1;
	if((dataOffset & 3) != 0)
	{
		ILAsmDataPad(4 - (dataOffset & 3));
	}
}

void ILAsmDataSetTLS(void)
{
	sectionIsData = 0;
	if((tlsOffset & 3) != 0)
	{
		ILAsmDataPad(4 - (tlsOffset & 3));
	}
}

typedef struct
{
	const char *name;
	ILUInt32	value;
	char	   *filename;
	long		linenum;

} DataLabelHashEntry;

static unsigned long DataLabelComputeFunc(const void *elem)
{
	return ILHashString(0, ((DataLabelHashEntry *)elem)->name, -1);
}

static unsigned long DataLabelKeyComputeFunc(const void *key)
{
	return ILHashString(0, (const char *)key, -1);
}

static int DataLabelMatchFunc(const void *elem, const void *key)
{
	return !strcmp(((DataLabelHashEntry *)elem)->name, (const char *)key);
}

static void DataLabelFreeFunc(void *elem)
{
	ILFree(elem);
}

void ILAsmDataSetLabel(const char *name)
{
	DataLabelHashEntry *entry;
	if(!nameTable)
	{
		nameTable = ILHashCreate(0, DataLabelComputeFunc,
								 DataLabelKeyComputeFunc,
								 DataLabelMatchFunc,
								 DataLabelFreeFunc);
		if(!nameTable)
		{
			ILAsmOutOfMemory();
		}
	}
	entry = ILHashFindType(nameTable, name, DataLabelHashEntry);
	if(entry)
	{
		ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
						  "data label `%s' already defined", name);
		ILAsmPrintMessage(entry->filename, entry->linenum,
						  "previous definition here");
		ILAsmErrors = 1;
		return;
	}
	if((entry = (DataLabelHashEntry *)ILMalloc(sizeof(DataLabelHashEntry)))
				== 0)
	{
		ILAsmOutOfMemory();
	}
	entry->name = name;
	if(sectionIsData)
	{
		entry->value = dataOffset;
	}
	else
	{
		entry->value = (tlsOffset | (ILUInt32)0x80000000);
	}
	entry->filename = ILAsmFilename;
	entry->linenum = ILAsmLineNum;
	if(!ILHashAdd(nameTable, entry))
	{
		ILAsmOutOfMemory();
	}
}

ILInt64 ILAsmDataResolveLabel(const char *name)
{
	DataLabelHashEntry *entry;
	if(!nameTable ||
	   (entry = ILHashFindType(nameTable, name, DataLabelHashEntry)) == 0)
	{
		/*ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
						  "data label `%s' undefined", name);
		ILAsmErrors = 1;*/
		return -1;
	}
	return entry->value;
}

void ILAsmDataPad(ILUInt32 size)
{
	static unsigned char const zeroes[16] =
		{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
	while(size >= 16)
	{
		ILAsmDataWriteBytes(zeroes, 16);
		size -= 16;
	}
	if(size > 0)
	{
		ILAsmDataWriteBytes(zeroes, size);
	}
}

void ILAsmDataWriteBytes(const ILUInt8 *buf, ILUInt32 len)
{
	if(sectionIsData)
	{
		ILWriterOtherWrite(ILAsmWriter, ".sdata",
						   IL_IMAGESECT_SDATA, buf, len);
		dataOffset += len;
	}
	else
	{
		ILWriterOtherWrite(ILAsmWriter, ".tls",
						   IL_IMAGESECT_TLS, buf, len);
		tlsOffset += len;
	}
}

void ILAsmDataWriteInt8(ILInt32 value, ILUInt32 num)
{
	unsigned char buf[1];
	buf[0] = (unsigned char)value;
	while(num > 0)
	{
		ILAsmDataWriteBytes(buf, 1);
		--num;
	}
}

void ILAsmDataWriteInt16(ILInt32 value, ILUInt32 num)
{
	unsigned char buf[2];
	IL_WRITE_INT16(buf, value);
	while(num > 0)
	{
		ILAsmDataWriteBytes(buf, 2);
		--num;
	}
}

void ILAsmDataWriteInt32(ILInt32 value, ILUInt32 num)
{
	unsigned char buf[4];
	IL_WRITE_INT32(buf, value);
	while(num > 0)
	{
		ILAsmDataWriteBytes(buf, 4);
		--num;
	}
}

void ILAsmDataWriteInt64(ILInt64 value, ILUInt32 num)
{
	unsigned char buf[8];
	IL_WRITE_INT64(buf, value);
	while(num > 0)
	{
		ILAsmDataWriteBytes(buf, 8);
		--num;
	}
}

void ILAsmDataWriteFloat32(ILUInt8 *value, ILUInt32 num)
{
	while(num > 0)
	{
		ILAsmDataWriteBytes(value, 4);
		--num;
	}
}

void ILAsmDataWriteFloat64(ILUInt8 *value, ILUInt32 num)
{
	while(num > 0)
	{
		ILAsmDataWriteBytes(value, 8);
		--num;
	}
}

#ifdef	__cplusplus
};
#endif
