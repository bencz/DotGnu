/*
 * dump_data.c - Dump ".data" blocks.
 *
 * Copyright (C) 2003, 2009  Southern Storm Software, Pty Ltd.
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
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Dump the contents of a data section, with labels inserted
 * wherever fields reference the data.
 */
static void DumpData(FILE *outstream, ILImage *image, const char *heading,
					 unsigned long rva, void *addr, unsigned long len,
					 unsigned long *fieldRVAs, unsigned long numFieldRVAs)
{
	while(len > 0)
	{
		/* Scan for the next field >= the current address */
		while(numFieldRVAs > 0 && *fieldRVAs < rva)
		{
			++fieldRVAs;
			--numFieldRVAs;
		}
		if(numFieldRVAs > 0 && *fieldRVAs >= (rva + len))
		{
			numFieldRVAs = 0;
		}

		/* Dump data before the next field */
		if(numFieldRVAs > 0 && *fieldRVAs != rva)
		{
			fprintf(outstream, "%s D_0x%08lX = bytearray", heading, rva);
			ILDAsmDumpBinaryBlob(outstream, image, addr,
								 (ILUInt32)(*fieldRVAs - rva));
			putc('\n', outstream);
			addr = (void *)(((unsigned char *)addr) + (*fieldRVAs - rva));
			len -= (*fieldRVAs - rva);
			rva = *fieldRVAs;
		}

		/* Dump the field or the remaining data */
		fprintf(outstream, "%s D_0x%08lX = bytearray", heading, rva);
		if(numFieldRVAs > 1)
		{
			ILDAsmDumpBinaryBlob(outstream, image, addr,
								 (ILUInt32)(fieldRVAs[1] - rva));
			addr = (void *)(((unsigned char *)addr) + (fieldRVAs[1] - rva));
			len -= (fieldRVAs[1] - rva);
			rva = fieldRVAs[1];
		}
		else
		{
			ILDAsmDumpBinaryBlob(outstream, image, addr, len);
			len = 0;
		}
		putc('\n', outstream);
	}
}

void ILDAsmDumpDataSections(FILE *outstream, ILImage *image)
{
	unsigned long *fieldRVAs;
	unsigned long numFieldRVAs;
	unsigned long posn;
	unsigned long posn2;
	unsigned long temp;
	ILFieldRVA *rva;
	void *dataAddr;
	unsigned long dataRVA;
	ILUInt32 dataLen;
	void *tlsAddr;
	unsigned long tlsRVA;
	ILUInt32 tlsLen;

	/* Collect all field RVA values so that we know where to
	   insert the data labels */
	numFieldRVAs = ILImageNumTokens(image, IL_META_TOKEN_FIELD_RVA);
	if(numFieldRVAs > 0)
	{
		fieldRVAs = (unsigned long *)ILMalloc
				(sizeof(unsigned long) * numFieldRVAs);
		if(!fieldRVAs)
		{
			numFieldRVAs = 0;
		}
		else
		{
			rva = 0;
			posn = 0;
			while((rva = (ILFieldRVA *)ILImageNextToken
						(image, IL_META_TOKEN_FIELD_RVA, rva)) != 0)
			{
				fieldRVAs[posn++] = ILFieldRVA_RVA(rva);
			}
		}
	}
	else
	{
		fieldRVAs = 0;
	}

	/* Sort the RVA list into ascending order */
	if(numFieldRVAs > 1)
	{
		for(posn = 0; posn < (numFieldRVAs - 1); ++posn)
		{
			for(posn2 = posn + 1; posn2 < numFieldRVAs; ++posn2)
			{
				if(fieldRVAs[posn] > fieldRVAs[posn2])
				{
					temp = fieldRVAs[posn];
					fieldRVAs[posn] = fieldRVAs[posn2];
					fieldRVAs[posn2] = temp;
				}
			}
		}
	}

	/* Find the extents of the ".data" and ".tls" sections */
	if(ILImageGetSection(image, IL_SECTION_DATA, &dataAddr, &dataLen))
	{
		dataRVA = ILImageGetSectionAddr(image, IL_SECTION_DATA);
	}
	else
	{
		dataAddr = 0;
		dataLen = 0;
		dataRVA = 0;
	}
	if(ILImageGetSection(image, IL_SECTION_TLS, &tlsAddr, &tlsLen))
	{
		tlsRVA = ILImageGetSectionAddr(image, IL_SECTION_TLS);
	}
	else
	{
		tlsAddr = 0;
		tlsLen = 0;
		tlsRVA = 0;
	}

	/* Dump the ".data" section */
	if(dataLen > 0)
	{
		DumpData(outstream, image, ".data", dataRVA, dataAddr, dataLen,
				 fieldRVAs, numFieldRVAs);
	}

	/* Dump the ".tls" section */
	if(tlsLen > 0)
	{
		DumpData(outstream, image, ".data tls", tlsRVA, tlsAddr, tlsLen,
				 fieldRVAs, numFieldRVAs);
	}

	/* Free the "fieldRVAs" array and exit */
	if(fieldRVAs)
	{
		ILFree(fieldRVAs);
	}
}

#ifdef	__cplusplus
};
#endif
