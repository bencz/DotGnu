/*
 * pecoff_writer.c - Deal with the ugly parts of writing PE/COFF images.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#ifdef IL_USE_WRITER

#include <time.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Flush the back-patch cache if necessary.
 */
static void FlushBackPatchCache(ILWriter *writer)
{
	if(writer->backpatching)
	{
		/* Seek to the beginning of the back-patch area */
		if(fseek(writer->stream, writer->backpatchSeek, 0) < 0)
		{
			writer->writeFailed = 1;
			return;
		}

		/* Write the modified contents of the back-patch buffer */
		if(fwrite(writer->backpatchBuf, 1, (unsigned)(writer->backpatchLen),
				  writer->stream) != (unsigned)(writer->backpatchLen))
		{
			writer->writeFailed = 1;
			return;
		}

		/* Record the new seek position and exit the back-patching mode */
		writer->currSeek = writer->backpatchSeek + writer->backpatchLen;
		writer->backpatching = 0;
	}
}

/*
 * Write a block of data to an output image.
 */
static void WriteToImage(ILWriter *writer, const void *buffer, unsigned size)
{
	if(writer->outOfMemory || writer->writeFailed)
	{
		/* We cannot continue if we've already had an error */
		return;
	}
	if(writer->seekable)
	{
		/* Flush the back-patching cache, if present */
		FlushBackPatchCache(writer);

		/* This is a seekable stream, so write directly to it */
		if(writer->offset != writer->currSeek)
		{
			/* We back-tracked at some point, so move the write
			   point back to the end of the file */
			if(fseek(writer->stream, writer->offset, 0) < 0)
			{
				writer->writeFailed = 1;
				return;
			}
		}
		if(fwrite(buffer, 1, size, writer->stream) != size)
		{
			writer->writeFailed = 1;
			return;
		}
		writer->offset += (unsigned long)size;
		writer->currSeek = writer->offset;
	}
	else
	{
		/* This is a non-seekable stream, so buffer the data for later */
		if(!_ILWBufferListAdd(&(writer->streamBuffer), buffer, size))
		{
			writer->outOfMemory = 1;
			return;
		}
		writer->offset += size;
		writer->currSeek = writer->offset;
	}
}

/*
 * Flush the contents of the image to the output stream
 * if we were writing to a non-seekable stream.
 */
static void WriteFlush(ILWriter *writer)
{
	ILWBuffer *wbuffer;

	if(writer->outOfMemory || writer->writeFailed)
	{
		/* Cannot continue if we saw an error previously */
		return;
	}
	if(!(writer->seekable))
	{
		/* Only need to flush if we were writing to a non-seekable stream */
		wbuffer = writer->streamBuffer.firstBuffer;
		while(wbuffer != 0)
		{
			if(wbuffer->next)
			{
				if(fwrite(wbuffer->data, 1, IL_WRITE_BUFFER_SIZE,
						  writer->stream) != IL_WRITE_BUFFER_SIZE)
				{
					writer->writeFailed = 1;
					return;
				}
			}
			else if(writer->streamBuffer.bytesUsed > 0)
			{
				if(fwrite(wbuffer->data, 1, writer->streamBuffer.bytesUsed,
						  writer->stream) != writer->streamBuffer.bytesUsed)
				{
					writer->writeFailed = 1;
					return;
				}
			}
			wbuffer = wbuffer->next;
		}
	}
	else
	{
		/* Flush the back-patching cache */
		FlushBackPatchCache(writer);
	}
}

/*
 * Write zero bytes until we have padded to a particular boundary.
 * We assume that "size" is 1024 or less.
 */
static void WriteBlockPadding(ILWriter *writer, unsigned long size)
{
	char buffer[1024];
	if((writer->offset & (size - 1)) != 0)
	{
		ILMemZero(buffer, size);
		WriteToImage(writer, buffer, size - (writer->offset & (size - 1)));
	}
}

/*
 * Write a buffer of bytes into a previous position within the image.
 */
static void WriteBackPatchBytes(ILWriter *writer, unsigned long posn,
								const char *buffer, unsigned size)
{
	ILWBuffer *wbuffer;

	if(writer->outOfMemory || writer->writeFailed)
	{
		/* Cannot continue if we saw an error previously */
		return;
	}
	if(writer->seekable)
	{
		/* Can we add the bytes to the current back-patch cache? */
		if(writer->backpatching && posn >= writer->backpatchSeek &&
		   (posn + size) <= (writer->backpatchSeek + writer->backpatchLen))
		{
			ILMemCpy(writer->backpatchBuf +
					 (unsigned)(posn - writer->backpatchSeek), buffer, size);
			return;
		}

		/* Flush the current back-patch cache contents */
		FlushBackPatchCache(writer);

		/* Only do caching for small values (usually 32-bit quantities) */
		if(size <= 4 && writer->backpatchBuf)
		{
			/* Align the back-patch cache for better filesystem performance */
			writer->backpatchSeek = posn & ~511;
			writer->backpatchLen = writer->offset - writer->backpatchSeek;
			if(writer->backpatchLen > IL_WRITE_BUFFER_SIZE)
			{
				writer->backpatchLen = IL_WRITE_BUFFER_SIZE;
			}

			/* Read the block to be patched into memory */
			if(fseek(writer->stream, writer->backpatchSeek, 0) < 0)
			{
				writer->writeFailed = 1;
				return;
			}
			if(fread(writer->backpatchBuf, 1, (unsigned)(writer->backpatchLen),
					 writer->stream) != (unsigned)(writer->backpatchLen))
			{
				/* The stream may not have been opened in a read-write mode,
				   so fall back to the simple "seek and write" method */
				writer->currSeek = ~((unsigned long)0);
				ILFree(writer->backpatchBuf);
				writer->backpatchBuf = 0;
				goto fallback;
			}
			writer->currSeek = writer->backpatchSeek + writer->backpatchLen;

			/* Patch the required bytes and return */
			ILMemCpy(writer->backpatchBuf +
					 (unsigned)(posn - writer->backpatchSeek), buffer, size);
			writer->backpatching = 1;
			return;
		}

		/* Seek back in the file and write the change */
	fallback:
		if(posn != writer->currSeek)
		{
			if(fseek(writer->stream, posn, 0) < 0)
			{
				writer->writeFailed = 1;
				return;
			}
		}
		if(fwrite(buffer, 1, size, writer->stream) != size)
		{
			writer->writeFailed = 1;
			return;
		}
		writer->currSeek = posn + size;
	}
	else
	{
		/* Non-seekable stream, so modify the buffered copy */
		wbuffer = writer->streamBuffer.firstBuffer;
		while(posn >= IL_WRITE_BUFFER_SIZE)
		{
			wbuffer = wbuffer->next;
			posn -= IL_WRITE_BUFFER_SIZE;
		}
		while(size > 0)
		{
			wbuffer->data[posn++] = (unsigned char)(*buffer++);
			if(posn >= IL_WRITE_BUFFER_SIZE)
			{
				wbuffer = wbuffer->next;
				posn -= IL_WRITE_BUFFER_SIZE;
			}
			--size;
		}
	}
}

/*
 * Write a 32-bit value into a previous position within the image.
 */
static void WriteBackPatch32(ILWriter *writer, unsigned long posn,
						     unsigned long value)
{
	char buffer[4];
	buffer[0] = (char)(value);
	buffer[1] = (char)(value >> 8);
	buffer[2] = (char)(value >> 16);
	buffer[3] = (char)(value >> 24);
	WriteBackPatchBytes(writer, posn, buffer, 4);
}

/*
 * Write a 16-bit value into a previous position within the image.
 */
static void WriteBackPatch16(ILWriter *writer, unsigned long posn,
						     unsigned long value)
{
	char buffer[2];
	buffer[0] = (char)(value);
	buffer[1] = (char)(value >> 8);
	WriteBackPatchBytes(writer, posn, buffer, 2);
}

/*
 * Write a block of data to a non-text section.
 */
static void WriteToSection(ILWriter *writer, const char *name,
						   unsigned long flags, const void *buffer,
						   unsigned size)
{
	ILWSection *section;
	ILWSection *prev;
	unsigned long newlen;
	unsigned char *newdata;

	/* Bail out if we saw an error previously */
	if(writer->outOfMemory || writer->writeFailed)
	{
		return;
	}

	/* Locate the section information block */
	section = writer->sections;
	prev = 0;
	while(section != 0 && strcmp(section->name, name) != 0)
	{
		prev = section;
		section = section->next;
	}
	if(!section)
	{
		section = (ILWSection *)ILMalloc(sizeof(ILWSection));
		if(!section)
		{
			writer->outOfMemory = 1;
			return;
		}
		strcpy(section->name, name);
		section->buffer = 0;
		section->length = 0;
		section->maxLength = 0;
		section->flags = flags;
		section->next = 0;
		if(prev)
		{
			prev->next = section;
		}
		else
		{
			writer->sections = section;
		}
	}

	/* Extend the size of the buffer if necessary */
	if((section->length + size) > section->maxLength)
	{
		newlen = (section->length + size + IL_WRITE_BUFFER_SIZE - 1) &
						~(IL_WRITE_BUFFER_SIZE - 1);
		newdata = (unsigned char *)ILRealloc(section->buffer, newlen);
		if(!newdata)
		{
			writer->outOfMemory = 1;
			return;
		}
		section->buffer = newdata;
		section->maxLength = newlen;
	}

	/* Copy the data into the buffer */
	ILMemCpy(section->buffer + section->length, buffer, size);
	section->length += size;
}

/*
 * Write the MS-DOS stub and PE signature.
 */
static void WriteDosStub(ILWriter *writer)
{
	unsigned char buffer[132];

	/* Clear the stub buffer */
	ILMemZero(buffer, sizeof(buffer));

	/* Write the MS-DOS header */
	buffer[0] = (unsigned char)'M';
	buffer[1] = (unsigned char)'Z';

	/* Write the MS-DOS stub size and relocation information */
	buffer[2]  = 0x90;
	buffer[4]  = 0x03;
	buffer[8]  = 0x04;
	buffer[12] = 0xFF;
	buffer[13] = 0xFF;
	buffer[16] = 0xB8;
	buffer[24] = 0x40;

	/* Write the offset to the PE/COFF header */
	buffer[60] = 0x80;

	/* Write the code for the MS-DOS stub */
	buffer[64] = 0x0E;				/* PUSH CS */
	buffer[65] = 0x1F;				/* POP DS */
	buffer[66] = 0xBA;				/* MOV DX, OFFSET(string) */
	buffer[67] = 0x0E;
	buffer[68] = 0x00;
	buffer[69] = 0xB4;				/* MOV AH, 9 */
	buffer[70] = 0x09;
	buffer[71] = 0xCD;				/* INT 21H */
	buffer[72] = 0x21;
	buffer[73] = 0xB8;				/* MOV AX, 4C01H */
	buffer[74] = 0x01;
	buffer[75] = 0x4C;
	buffer[76] = 0xCD;				/* INT 21H */
	buffer[77] = 0x21;
	strcpy((char *)(buffer + 78),	/* string */
		   "This program cannot be run in DOS mode.\r\r\n$");

	/* Write the PE signature */
	buffer[128] = (unsigned char)'P';
	buffer[129] = (unsigned char)'E';

	/* Write the stub buffer to the output stream */
	WriteToImage(writer, buffer, sizeof(buffer));
}

/*
 * Write the PE/COFF header.
 */
static void WritePECOFFHeader(ILWriter *writer)
{
	unsigned char header[20];
	unsigned char opthdr[224];
	time_t timestamp = time(0);

	/* Build the header */
	header[0]  = 0x4C;								/* Machine type (386) */
	header[1]  = 0x01;
	header[2]  = 0x00;								/* Number of sections */
	header[3]  = 0x00;
	IL_WRITE_UINT32(header + 4, timestamp);			/* Time file was created */
	IL_WRITE_UINT32(header + 8, 0);					/* Symbol table pointer */
	IL_WRITE_UINT32(header + 12, 0);				/* Number of symbols */
	if(writer->type == IL_IMAGETYPE_OBJ)			/* Optional header size */
	{
		/* Object files do not have optional headers */
		IL_WRITE_UINT16(header + 16, 0);
	}
	else
	{
		/* Executables and DLL's do have optional headers */
		IL_WRITE_UINT16(header + 16, sizeof(opthdr));
	}
	if(writer->type == IL_IMAGETYPE_OBJ)			/* Characteristics */
	{
		IL_WRITE_UINT16(header + 18, 0x0000);
	}
	else if(writer->type == IL_IMAGETYPE_DLL)
	{
		IL_WRITE_UINT16(header + 18, 0x210E);
	}
	else
	{
		IL_WRITE_UINT16(header + 18, 0x010E);
	}

	/* Write the PE/COFF header to the output image */
	writer->peOffset = writer->offset;
	WriteToImage(writer, header, sizeof(header));

	/* Build the optional header if not an object file */
	if(writer->type != IL_IMAGETYPE_OBJ)
	{
		/* Executable or DLL image */
		ILMemZero(opthdr, sizeof(opthdr));
		IL_WRITE_UINT16(opthdr, 0x010B);			/* Magic number */
		opthdr[2]  = 0x06;							/* Linker major number */
		opthdr[3]  = 0x00;							/* Linker minor number */
		IL_WRITE_UINT32(opthdr + 20, 0x00002000);	/* RVA of code section */
		if(writer->type == IL_IMAGETYPE_DLL)		/* Preferred image base */
		{
			IL_WRITE_UINT32(opthdr + 28, 0x10000000);
		}
		else
		{
			IL_WRITE_UINT32(opthdr + 28, 0x00400000);
		}
		IL_WRITE_UINT32(opthdr + 32, 8192);			/* Section alignment */
		IL_WRITE_UINT32(opthdr + 36, 512);			/* File alignment */
		IL_WRITE_UINT16(opthdr + 40, 4);			/* OS Major */
		IL_WRITE_UINT16(opthdr + 42, 0);			/* OS Minor */
		IL_WRITE_UINT16(opthdr + 44, 0);			/* User Major */
		IL_WRITE_UINT16(opthdr + 46, 0);			/* User Minor */
		IL_WRITE_UINT16(opthdr + 48, 4);			/* System Major */
		IL_WRITE_UINT16(opthdr + 50, 0);			/* System Minor */
		IL_WRITE_UINT32(opthdr + 60, 1024);			/* Combined header size */
		if((writer->flags & IL_WRITEFLAG_SUBSYS_GUI) != 0)
		{
			IL_WRITE_UINT16(opthdr + 68, 2);		/* Subsystem (WindowsGUI) */
		}
		else
		{
			IL_WRITE_UINT16(opthdr + 68, 3);		/* Subsystem (WindowsCUI) */
		}
		IL_WRITE_UINT32(opthdr + 72, 1048576);		/* Stack reserve size */
		IL_WRITE_UINT32(opthdr + 76, 4096);			/* Stack commit size */
		IL_WRITE_UINT32(opthdr + 80, 1048576);		/* Heap reserve size */
		IL_WRITE_UINT32(opthdr + 84, 4096);			/* Heap commit size */
		IL_WRITE_UINT32(opthdr + 88, 0);			/* Loader flags */
		IL_WRITE_UINT32(opthdr + 92, 16);			/* Number of data dirs */

		/* Build data directories */
		IL_WRITE_UINT32(opthdr + 192, 0x00002000);	/* IAT RVA */
		IL_WRITE_UINT32(opthdr + 196, 8);			/* IAT size */
		IL_WRITE_UINT32(opthdr + 208, 0x00002008);	/* Runtime header RVA */
		IL_WRITE_UINT32(opthdr + 212, 72);			/* Runtime header size */

		/* Write the optional header to the output image */
		writer->optOffset = writer->offset;
		WriteToImage(writer, opthdr, sizeof(opthdr));
	}
	else
	{
		/* Object file with no optional header */
		writer->optOffset = 0;
	}

	/* The section table will begin here */
	writer->sectOffset = writer->offset;
}

/*
 * Write the IL runtime header.
 */
static void WriteRuntimeHeader(ILWriter *writer)
{
	char buffer[72];

	/* Clear the buffer before we start */
	ILMemZero(buffer, sizeof(buffer));

	/* If not an object file, then leave 8 bytes for back-patching
	   later with the import address table */
	if(writer->type != IL_IMAGETYPE_OBJ)
	{
		WriteToImage(writer, buffer, 8);
	}

	/* Build as much of the IL runtime header as we know about at present */
	IL_WRITE_UINT32(buffer, sizeof(buffer));	/* Size of the header */
	IL_WRITE_UINT16(buffer + 4, 2);				/* Major version */
	IL_WRITE_UINT16(buffer + 6, 0);				/* Minor version */
	if((writer->flags & IL_WRITEFLAG_32BIT_ONLY) != 0)
	{
		IL_WRITE_UINT32(buffer + 16, 3);		/* Flags (CIL & 32-bit) */
	}
	else
	{
		IL_WRITE_UINT32(buffer + 16, 1);		/* Flags (CIL only) */
	}

	/* Write the header */
	writer->runtimeOffset = writer->offset;
	WriteToImage(writer, buffer, sizeof(buffer));
}

void _ILWriteHeaders(ILWriter *writer)
{
	/* If not an object file, output the MS-DOS stub and PE signature */
	if(writer->type != IL_IMAGETYPE_OBJ)
	{
		WriteDosStub(writer);
	}

	/* Write the PE/COFF header and optional header */
	WritePECOFFHeader(writer);

	/* Pad with zeroes until we reach a multiple of 512 (object files)
	   or 1024 bytes (executables and DLL's).  This should leave enough
	   room for all of the section table entries we are likely to have */
	if(writer->type == IL_IMAGETYPE_OBJ)
	{
		WriteBlockPadding(writer, 512);
	}
	else
	{
		WriteBlockPadding(writer, 1024);
	}

	/* Back-patch the combined header size */
	if(writer->optOffset != 0)
	{
		WriteBackPatch32(writer, writer->optOffset + 60, writer->offset);
	}

	/* Remember the location of the first section (".text") for later */
	writer->firstSection = writer->offset;

	/* Write the IL runtime header */
	WriteRuntimeHeader(writer);
}

/*
 * Back-patch information about a section.
 */
static void WriteSection(ILWriter *writer, unsigned long table,
						 const char *name, unsigned long virtAddr,
						 unsigned long realAddr, unsigned long virtSize,
						 unsigned long flags)
{
	/* Write the name */
	WriteBackPatchBytes(writer, table, name, strlen(name));

	/* Write the virtual size and address information */
	if(writer->type != IL_IMAGETYPE_OBJ)
	{
		WriteBackPatch32(writer, table + 8, virtSize);
	}
	WriteBackPatch32(writer, table + 12, virtAddr);

	/* Write the real size and address information */
	if(writer->type == IL_IMAGETYPE_OBJ)
	{
		WriteBackPatch32(writer, table + 16, (virtSize + 3) & ~3);
	}
	else
	{
		WriteBackPatch32(writer, table + 16, (virtSize + 511) & ~511);
	}
	WriteBackPatch32(writer, table + 20, realAddr);

	/* Write the section flags */
	WriteBackPatch32(writer, table + 36, flags);
}

void _ILWriteFinal(ILWriter *writer)
{
	unsigned long imageSize;
	unsigned long offset;
	ILWSection *section;
	ILWSection *prevSection;
	ILWSection *debugSection;
	int numSections;
	unsigned long table;
	unsigned long textLength;
	unsigned long realTextSize;
	unsigned long importTable;
	unsigned long importHintTable;
	unsigned long entryPoint;
	unsigned char entry[20];
	unsigned long dataSection;
	unsigned long tlsSection;
	unsigned long relocSection;

	/* Set the entry point token code */
	if(writer->entryPoint)
	{
		WriteBackPatch32(writer, writer->runtimeOffset + 20,
						 ILMethod_Token(writer->entryPoint));
	}

	/* If this is not an object file, then add the import table entries.
	   This turns the file into a valid Windows executable so that older
	   versions of Windows will launch the runtime engine automatically */
	if(writer->type != IL_IMAGETYPE_OBJ)
	{
		/* Align the import table on a DWORD boundary */
		WriteBlockPadding(writer, 4);

		/* Determine where the tables will begin */
		importTable = ILWriterGetTextRVA(writer);
		importHintTable = importTable + 40 + 8;
		if((importHintTable & 0x0F) != 0)
		{
			importHintTable = ((importHintTable + 15) & 0xFFFFFFF0);
		}

		/* Format and write the first import table entry */
		ILMemZero(entry, sizeof(entry));
		offset = importTable + 40;
		entry[0] = (unsigned char)offset;			/* ImportLookupTable */
		entry[1] = (unsigned char)(offset >> 8);
		entry[2] = (unsigned char)(offset >> 16);
		entry[3] = (unsigned char)(offset >> 24);
		offset = importHintTable + 14;
		entry[12] = (unsigned char)offset;			/* Name */
		entry[13] = (unsigned char)(offset >> 8);
		entry[14] = (unsigned char)(offset >> 16);
		entry[15] = (unsigned char)(offset >> 24);
		entry[16] = (unsigned char)0x00;			/* ImportAddressTable */
		entry[17] = (unsigned char)0x20;
		entry[18] = (unsigned char)0x00;
		entry[19] = (unsigned char)0x00;
		WriteToImage(writer, entry, 20);

		/* Write a zero entry to terminate the import table */
		ILMemZero(entry, sizeof(entry));
		WriteToImage(writer, entry, 20);

		/* Write the import lookup table */
		entry[0] = (unsigned char)importHintTable;
		entry[1] = (unsigned char)(importHintTable >> 8);
		entry[2] = (unsigned char)(importHintTable >> 16);
		entry[3] = (unsigned char)(importHintTable >> 24);
		WriteToImage(writer, entry, 8);

		/* Align the hint table on a 16-byte boundary */
		WriteBlockPadding(writer, 16);

		/* Write out the hint table */
		entry[0] = (unsigned char)0x00;
		entry[1] = (unsigned char)0x00;
		if(writer->type == IL_IMAGETYPE_EXE)
		{
			strcpy((char *)(entry + 2), "_CorExeMain");
		}
		else
		{
			strcpy((char *)(entry + 2), "_CorDllMain");
		}
		WriteToImage(writer, entry, 14);

		/* Write the name of the DLL we are importing from */
		ILMemZero(entry, sizeof(entry));
		strcpy((char *)entry, "mscoree.dll");
		WriteToImage(writer, entry, 16);

		/* Create the entry point for the native x86 part of the binary */
		entryPoint = ILWriterGetTextRVA(writer);
		entry[0] = (unsigned char)0xFF;			/* JMP */
		entry[1] = (unsigned char)0x25;
		if(writer->type == IL_IMAGETYPE_DLL)	/* Virtual address of IAT */
		{
			offset = 0x10000000 + 0x2000;
		}
		else
		{
			offset = 0x00400000 + 0x2000;
		}
		entry[2] = (unsigned char)offset;
		entry[3] = (unsigned char)(offset >> 8);
		entry[4] = (unsigned char)(offset >> 16);
		entry[5] = (unsigned char)(offset >> 24);
		WriteToImage(writer, entry, 6);

		/* Back-patch the start of the .text section to create
		   the import address table (IAT) */
		WriteBackPatch32(writer, writer->firstSection, importHintTable);

		/* Back-patch the entry point RVA */
		WriteBackPatch32(writer, writer->optOffset + 16, entryPoint);

		/* Back-patch the optional header with the import table position */
		WriteBackPatch32(writer, writer->optOffset + 104, importTable);
		WriteBackPatch32(writer, writer->optOffset + 108,
						 entryPoint - importTable - 3);

		/* Create the ".reloc" section to relocate the entry point */
		entryPoint += 2;	/* Relocate the operand, not the instruction */
		offset = entryPoint & ~((unsigned long)4095);
		entry[0] = (unsigned char)offset;			/* Page for relocation */
		entry[1] = (unsigned char)(offset >> 8);
		entry[2] = (unsigned char)(offset >> 16);
		entry[3] = (unsigned char)(offset >> 24);
		entry[4] = 0x0C;							/* Size of relocation */
		entry[5] = 0x00;
		entry[6] = 0x00;
		entry[7] = 0x00;
		offset = (entryPoint & (unsigned long)4095) | (unsigned long)0x3000;
		entry[8] = (unsigned char)offset;			/* Offset of relocation */
		entry[9] = (unsigned char)(offset >> 8);
		entry[10] = 0x00;
		entry[11] = 0x00;
		ILWriterOtherWrite(writer, ".reloc", IL_IMAGESECT_RELOC, entry, 12);
	}

	/* Record the final length of the ".text"/".text$il" section */
	textLength = writer->offset - writer->firstSection;

	/* Pad ".text"/".text$il" with zeroes until we reach
	   an alignment boundary */
	if(writer->type == IL_IMAGETYPE_OBJ)
	{
		/* Object files only need DWORD-alignment of sections */
		WriteBlockPadding(writer, 4);
		realTextSize = ((textLength + 3) & ~3);
	}
	else
	{
		/* Executables and DLL's need 512-byte alignment */
		WriteBlockPadding(writer, 512);
		realTextSize = ((textLength + 511) & ~511);
	}

	/* If we have ".ildebug" data, then move it to the end of the list */
	section = writer->sections;
	prevSection = 0;
	debugSection = 0;
	while(section != 0)
	{
		if(!strcmp(section->name, ".ildebug"))
		{
			debugSection = section;
			if(prevSection)
			{
				prevSection->next = section->next;
			}
			else
			{
				writer->sections = section->next;
			}
			section = section->next;
		}
		else
		{
			prevSection = section;
			section = section->next;
		}
	}
	if(debugSection)
	{
		if(prevSection)
		{
			prevSection->next = debugSection;
		}
		else
		{
			writer->sections = debugSection;
		}
		debugSection->next = 0;
	}

	/* Dump the contents of other sections */
	section = writer->sections;
	while(section != 0)
	{
		WriteToImage(writer, section->buffer, section->length);
		if(writer->type == IL_IMAGETYPE_OBJ)
		{
			WriteBlockPadding(writer, 4);
		}
		else
		{
			WriteBlockPadding(writer, 512);
		}
		section = section->next;
	}

	/* Back-patch the final image size */
	if(writer->optOffset != 0)
	{
		imageSize = 8192;
		imageSize += (textLength + 8191) & ~8191;
		section = writer->sections;
		while(section != 0)
		{
			imageSize += (section->length + 8191) & ~8191;
			section = section->next;
		}
		WriteBackPatch32(writer, writer->optOffset + 56, imageSize);
	}

	/* Back-patch the section table */
	numSections = 0;
	table = writer->sectOffset;
	imageSize = 8192;
	offset = writer->firstSection;
	if(writer->type == IL_IMAGETYPE_OBJ)
	{
		/* When we are writing an object file, we name the text
		   section ".text$il" to distinguish it from native objects */
		WriteSection(writer, table, ".text$il",
					 imageSize, offset, textLength,
					 IL_IMAGESECT_TEXT);
	}
	else
	{
		WriteSection(writer, table, ".text",
					 imageSize, offset, textLength,
					 IL_IMAGESECT_TEXT);
	}
	table += 40;
	imageSize += (textLength + 8191) & ~8191;
	offset += realTextSize;
	section = writer->sections;
	numSections = 1;
	dataSection = 0;
	tlsSection = 0;
	relocSection = 0;
	while(section != 0)
	{
		if(!strcmp(section->name, ".sdata"))
		{
			dataSection = imageSize;
		}
		else if(!strcmp(section->name, ".tls"))
		{
			tlsSection = imageSize;
		}
		else if(!strcmp(section->name, ".reloc"))
		{
			relocSection = imageSize;
		}
		WriteSection(writer, table, section->name,
					 imageSize, offset, section->length, section->flags);
		table += 40;
		imageSize += (section->length + 8191) & ~8191;
		if(writer->type == IL_IMAGETYPE_OBJ)
		{
			offset += (section->length + 3) & ~3;
		}
		else
		{
			offset += (section->length + 511) & ~511;
		}
		section = section->next;
		++numSections;
	}

	/* Back-patch the number of sections */
	WriteBackPatch16(writer, writer->peOffset + 2, (unsigned long)numSections);

	/* Back-patch summary information about code and data sections */
	if(writer->optOffset != 0)
	{
		/* Length of all code sections */
		WriteBackPatch32(writer, writer->optOffset + 4, realTextSize);

		if(writer->sections != 0)
		{
			/* Length of all data sections */
			WriteBackPatch32(writer, writer->optOffset + 8,
							 offset - writer->firstSection - realTextSize);

			/* Base of data sections when loaded */
			WriteBackPatch32(writer, writer->optOffset + 24,
							 0x2000 + ((textLength + 8191) & ~8191));
		}
	}

	/* Back-patch the position of the relocation table */
	if(writer->type != IL_IMAGETYPE_OBJ)
	{
		WriteBackPatch32(writer, writer->optOffset + 136, relocSection);
		WriteBackPatch32(writer, writer->optOffset + 140, 12);
	}

	/* Write the field RVA fixups, now that the data sections are in place */
	_ILWriteFieldRVAFixups(writer, dataSection, tlsSection);

	/* Flush the image to the output stream */
	WriteFlush(writer);
}

unsigned long ILWriterGetTextRVA(ILWriter *writer)
{
	return ((writer->offset - writer->firstSection) + 0x2000);
}

void ILWriterTextWrite(ILWriter *writer, const void *buffer, unsigned long size)
{
	/* Break the write up into chunks.  This is mostly paranoia on my
	   part, just in case someone tries to compile this on an embedded
	   machine where "unsigned" is less than 32 bits in size */
	while(size > 0)
	{
		if(size > 2048)
		{
			WriteToImage(writer, buffer, 2048);
			size -= 2048;
			buffer = (const void *)(((char *)buffer) + 2048);
		}
		else
		{
			WriteToImage(writer, buffer, size);
			size = 0;
		}
	}
}

void ILWriterTextAlign(ILWriter *writer)
{
	WriteBlockPadding(writer, 4);
}

void ILWriterTextWrite32Bit(ILWriter *writer, unsigned long rva,
							unsigned long value)
{
	unsigned long offset = rva + writer->firstSection - 0x2000;
	WriteBackPatch32(writer, offset, value);
}

void ILWriterOtherWrite(ILWriter *writer, const char *name,
						unsigned long flags, const void *buffer,
						unsigned size)
{
	WriteToSection(writer, name, flags, buffer, size);
}

void ILWriterUpdateHeader(ILWriter *writer, unsigned long entry,
						  unsigned long rva, unsigned long size)
{
	WriteBackPatch32(writer, writer->runtimeOffset + entry, rva);
	WriteBackPatch32(writer, writer->runtimeOffset + entry + 4, size);
}

void ILWriterSetEntryPoint(ILWriter *writer, ILMethod *method)
{
	writer->entryPoint = method;
}

void ILWriterSetFixup(ILWriter *writer, unsigned long rva,
					  ILProgramItem *item)
{
	ILFixup *fixup = ILMemPoolAlloc(&(writer->fixups), ILFixup);
	if(fixup)
	{
		fixup->kind = IL_FIXUP_TOKEN;
		fixup->rva = rva;
		fixup->un.item = item;
		fixup->next = 0;
		if(writer->lastFixup)
		{
			writer->lastFixup->next = fixup;
		}
		else
		{
			writer->firstFixup = fixup;
		}
		writer->lastFixup = fixup;
	}
	else
	{
		writer->outOfMemory = 1;
	}
}

void _ILWriteTokenFixups(ILWriter *writer)
{
	ILFixup *fixup = writer->firstFixup;
	unsigned long offset;
	unsigned long token;
	while(fixup != 0)
	{
		if(fixup->kind == IL_FIXUP_TOKEN)
		{
			offset = fixup->rva + writer->firstSection - 0x2000;
			token = fixup->un.item->token;
			WriteBackPatch32(writer, offset, token);
		}
		fixup = fixup->next;
	}
}

void _ILWriteFieldRVAFixups(ILWriter *writer, unsigned long dataSection,
							unsigned long tlsSection)
{
	ILFixup *fixup = writer->firstFixup;
	unsigned long offset;
	unsigned long rva;
	while(fixup != 0)
	{
		if(fixup->kind == IL_FIXUP_FIELD_RVA)
		{
			offset = (writer->indexRVA + fixup->rva) +
					 (writer->firstSection - 0x2000);
			rva = fixup->un.value;
			if((rva & (ILUInt32)0x80000000) == 0)
			{
				/* The RVA is in the ".sdata" section */
				rva += dataSection;
			}
			else
			{
				/* The RVA is in the ".tls" section */
				rva = (rva & (ILUInt32)0x7FFFFFFF) + tlsSection;
			}
			WriteBackPatch32(writer, offset, rva);
		}
		fixup = fixup->next;
	}
}

void ILWriterSetStream(ILWriter *writer, FILE *stream, int seekable)
{
	if (!(writer->stream) && stream)
	{
		writer->stream = stream;
		if (seekable)
		{
			WriteFlush(writer);
		}
		writer->seekable = seekable;
	}
}

int ILWriterResetTypeAndFlags(ILWriter *writer, int type, int flags)
{
	if (type != IL_IMAGETYPE_DLL &&
	    type != IL_IMAGETYPE_EXE)
	{
		return 0;
	}
	if (writer->type != IL_IMAGETYPE_DLL &&
	    writer->type != IL_IMAGETYPE_EXE)
	{
		return 0;
	}
	if ((flags & IL_WRITEFLAG_JVM_MODE) != 0 ||
	    (writer->flags & IL_WRITEFLAG_JVM_MODE) != 0)
	{
		return 0;
	}
	if (writer->type != type)
	{
		writer->type = type;
		if(type == IL_IMAGETYPE_DLL)
		{
			WriteBackPatch16(writer, (writer->peOffset)+18, 0x210E);
			WriteBackPatch32(writer, (writer->optOffset)+28, 0x10000000);
		}
		else
		{
			WriteBackPatch16(writer, (writer->peOffset)+18, 0x010E);
			WriteBackPatch32(writer, (writer->optOffset)+28, 0x00400000);
		}
	}
	if (writer->flags != flags)
	{
		writer->flags = flags;
		if((flags & IL_WRITEFLAG_SUBSYS_GUI) != 0)
		{
			WriteBackPatch16(writer, (writer->optOffset)+68, 2);
		}
		else
		{
			WriteBackPatch16(writer, (writer->optOffset)+68, 3);
		}
		if((flags & IL_WRITEFLAG_32BIT_ONLY) != 0)
		{
			WriteBackPatch32(writer, (writer->runtimeOffset)+16, 3);
		}
		else
		{
			WriteBackPatch32(writer, (writer->runtimeOffset)+16, 1);
		}
	}
	return 1;
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_USE_WRITER */
