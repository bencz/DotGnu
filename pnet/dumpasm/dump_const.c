/*
 * dump_const.c - Dump constant information for a field, parameter, etc.
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

#ifdef	__cplusplus
extern	"C" {
#endif

void ILDumpConstant(FILE *stream, ILProgramItem *item, int hexFloats)
{
	ILConstant *constant;
	const unsigned char *blob;
	ILUInt32 blobLen;

	/* Get the constant information block for the item */
	constant = ILConstantGetFromOwner(item);
	if(!constant)
	{
		return;
	}

	/* Get the blob that corresponds to the constant value */
	blob = (const unsigned char *)ILConstantGetValue(constant, &blobLen);
	if(!blob)
	{
		/* Treat non-existent blobs as empty: sometimes empty
		   strings are encoded as a constant with no blob */
		blobLen = 0;
	}

	/* Determine how to dump the constant based on its element type */
	fputs(" = ", stream);
	switch(ILConstant_ElemType(constant))
	{
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			if(blobLen > 0)
			{
				fprintf(stream, "bool(%s)",
						(blob[0] ? "true" : "false"));
			}
			else
			{
				fputs("BAD BOOL", stream);
			}
		}
		break;

		case IL_META_ELEMTYPE_I1:
		case IL_META_ELEMTYPE_U1:
		{
			if(blobLen > 0)
			{
				fprintf(stream, "int8(0x%02X)", (int)(blob[0]));
			}
			else
			{
				fputs("BAD BYTE", stream);
			}
		}
		break;

		case IL_META_ELEMTYPE_CHAR:
		{
			if(blobLen > 1)
			{
				fprintf(stream, "char(0x%04lX)",
						(unsigned long)(IL_READ_UINT16(blob)));
			}
			else
			{
				fputs("BAD CHAR", stream);
			}
		}
		break;

		case IL_META_ELEMTYPE_I2:
		case IL_META_ELEMTYPE_U2:
		{
			if(blobLen > 1)
			{
				fprintf(stream, "int16(0x%04lX)",
						(unsigned long)(IL_READ_UINT16(blob)));
			}
			else
			{
				fputs("BAD SHORT", stream);
			}
		}
		break;

		case IL_META_ELEMTYPE_I4:
		case IL_META_ELEMTYPE_U4:
		{
			if(blobLen > 3)
			{
				fprintf(stream, "int32(0x%08lX)",
						(unsigned long)(IL_READ_UINT32(blob)));
			}
			else
			{
				fputs("BAD INT", stream);
			}
		}
		break;

		case IL_META_ELEMTYPE_I8:
		case IL_META_ELEMTYPE_U8:
		{
			if(blobLen > 7)
			{
				fprintf(stream, "int64(0x%08lX%08lX)",
						(unsigned long)(IL_READ_UINT32(blob + 4)),
						(unsigned long)(IL_READ_UINT32(blob)));
			}
			else
			{
				fputs("BAD LONG", stream);
			}
		}
		break;

		case IL_META_ELEMTYPE_R4:
		{
			if(blobLen > 3)
			{
			#ifdef IL_CONFIG_FP_SUPPORTED
				if(hexFloats)
				{
					fprintf(stream, "float32(0x%08lX)",
							(unsigned long)(IL_READ_UINT32(blob)));
				}
				else
				{
					fprintf(stream, "float32(%.30e)",
							(double)(IL_READ_FLOAT(blob)));
				}
			#else	/* !IL_CONFIG_FP_SUPPORTED */
				fprintf(stream, "float32(0x%08lX)",
						(unsigned long)(IL_READ_UINT32(blob)));
			#endif	/* !IL_CONFIG_FP_SUPPORTED */
			}
			else
			{
				fputs("BAD FLOAT", stream);
			}
		}
		break;

		case IL_META_ELEMTYPE_R8:
		{
			if(blobLen > 7)
			{
			#ifdef IL_CONFIG_FP_SUPPORTED
				if(hexFloats)
				{
					fprintf(stream, "float64(0x%08lX%08lX)",
							(unsigned long)(IL_READ_UINT32(blob + 4)),
							(unsigned long)(IL_READ_UINT32(blob)));
				}
				else
				{
					fprintf(stream, "float64(%.30e)",
							(double)(IL_READ_DOUBLE(blob)));
				}
			#else	/* !IL_CONFIG_FP_SUPPORTED */
				fprintf(stream, "float64(0x%08lX%08lX)",
						(unsigned long)(IL_READ_UINT32(blob + 4)),
						(unsigned long)(IL_READ_UINT32(blob)));
			#endif	/* !IL_CONFIG_FP_SUPPORTED */
			}
			else
			{
				fputs("BAD DOUBLE", stream);
			}
		}
		break;

		case IL_META_ELEMTYPE_STRING:
		{
			ILDumpUnicodeString(stream, blob, blobLen / 2);
		}
		break;

		case IL_META_ELEMTYPE_CLASS:
		{
			if(blobLen == 4 && IL_READ_UINT32(blob) == 0)
			{
				/* 32-bit null constant */
				fputs("nullref", stream);
			}
			else if(blobLen == 8 && IL_READ_UINT64(blob) == 0)
			{
				/* 64-bit null constant */
				fputs("nullref", stream);
			}
			else
			{
				fputs("BAD CLASS", stream);
			}
		}
		break;

		default:
		{
			fprintf(stream, "[UNKNOWN TYPE %02X]",
					(int)(ILConstant_ElemType(constant)));
		}
		break;
	}
}

#ifdef	__cplusplus
};
#endif
