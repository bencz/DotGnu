/*
 * il_writer.h - Routines for writing IL executable images.
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

#ifndef	_IL_WRITER_H
#define	_IL_WRITER_H

#include "il_program.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Opaque definition of an IL image writer control structure.
 */
typedef struct _tagILWriter ILWriter;

/*
 * Section flag bits of interest to IL images.
 */
#define	IL_IMAGESECT_CODE			0x00000020
#define	IL_IMAGESECT_INIT_DATA		0x00000040
#define	IL_IMAGESECT_DISCARDABLE	0x02000000
#define	IL_IMAGESECT_EXECUTE		0x20000000
#define	IL_IMAGESECT_READ			0x40000000
#define	IL_IMAGESECT_WRITE			0x80000000

/*
 * Predefined flag combinations for interesting sections.
 */
#define	IL_IMAGESECT_TEXT			(IL_IMAGESECT_CODE | \
									 IL_IMAGESECT_EXECUTE | \
									 IL_IMAGESECT_READ)
#define	IL_IMAGESECT_RSRC			(IL_IMAGESECT_INIT_DATA | \
									 IL_IMAGESECT_READ)
#define	IL_IMAGESECT_RELOC			(IL_IMAGESECT_INIT_DATA | \
									 IL_IMAGESECT_DISCARDABLE | \
									 IL_IMAGESECT_READ)
#define	IL_IMAGESECT_DEBUG			(IL_IMAGESECT_INIT_DATA | \
									 IL_IMAGESECT_DISCARDABLE | \
									 IL_IMAGESECT_READ)
#define	IL_IMAGESECT_SDATA			(IL_IMAGESECT_INIT_DATA | \
									 IL_IMAGESECT_READ | \
									 IL_IMAGESECT_WRITE)
#define	IL_IMAGESECT_TLS			(IL_IMAGESECT_INIT_DATA | \
									 IL_IMAGESECT_READ | \
									 IL_IMAGESECT_WRITE)

/*
 * Offset of entries within the IL runtime header.
 */
#define	IL_IMAGEENTRY_METADATA		8
#define	IL_IMAGEENTRY_RESOURCES		24
#define	IL_IMAGEENTRY_STRONG_NAMES	32
#define	IL_IMAGEENTRY_CODE_MANAGER	40
#define	IL_IMAGEENTRY_VTABLE_FIXUPS	48
#define	IL_IMAGEENTRY_EXPORT_ADDRS	56

/*
 * Flags that modify the behaviour of the writer.
 */
#define	IL_WRITEFLAG_32BIT_ONLY		1	/* Execute on 32-bit systems only */
#define	IL_WRITEFLAG_SUBSYS_CUI		0	/* Command-line subsystem */
#define	IL_WRITEFLAG_SUBSYS_GUI		2	/* GUI subsystem */
#define	IL_WRITEFLAG_JVM_MODE		4	/* Java image output */

/*
 * Create an image writer and attach it to a file stream.
 * If "seekable" is non-zero, then the stream is seekable.
 */
ILWriter *ILWriterCreate(FILE *stream, int seekable, int type, int flags);

/*
 * Set the stream of the image writer if the stream supplied on creation was
 * null. If "seekable" is non-zero, then the stream is seekable. Does nothing
 * if the stream has already been set or if the supplied stream is null.
 */
void ILWriterSetStream(ILWriter *writer, FILE *stream, int seekable);

/*
 * Reset the type and flags of the image writer. If the current or given types
 * are not equal to either IL_IMAGETYPE_DLL or IL_IMAGETYPE_EXE then nothing is
 * changed. If the current or given flags have IL_WRITEFLAG_JVM_MODE set then
 * nothing is changed. Returns 1 on success, 0 on failure.
 */
int ILWriterResetTypeAndFlags(ILWriter *writer, int type, int flags);

/*
 * Set the runtime version string in the metadata header.
 */
void ILWriterSetVersionString(ILWriter *writer, const char *version);

/*
 * Infer the runtime version string from the specified image
 * (which is usually the "mscorlib" assembly).  This ensures that
 * any application linked against the core library will inherit the
 * same runtime version as the core library.
 */
void ILWriterInferVersionString(ILWriter *writer, ILImage *image);

/*
 * Output the metadata from an image structure into an
 * image writer's output stream.  This is typically called
 * just before "ILWriterDestroy".
 */
void ILWriterOutputMetadata(ILWriter *writer, ILImage *image);

/*
 * Flush the remainder of an image and destroy an image writer.
 * Returns 1 if OK, 0 if a write error occurred, or -1 if out
 * of memory.
 */
int ILWriterDestroy(ILWriter *writer);

/*
 * Get the current RVA of the text section.
 */
unsigned long ILWriterGetTextRVA(ILWriter *writer);

/*
 * Write a buffer of bytes to the text section.
 */
void ILWriterTextWrite(ILWriter *writer, const void *buffer,
					   unsigned long size);

/*
 * Align the text section on a 4-byte boundary.
 */
void ILWriterTextAlign(ILWriter *writer);

/*
 * Write a 32-bit value at a particular RVA within the text section.
 */
void ILWriterTextWrite32Bit(ILWriter *writer, unsigned long rva,
							unsigned long value);

/*
 * Write a buffer of bytes to another section.
 */
void ILWriterOtherWrite(ILWriter *writer, const char *name,
						unsigned long flags, const void *buffer,
						unsigned size);

/*
 * Update an RVA/size directory entry within the IL runtime header.
 */
void ILWriterUpdateHeader(ILWriter *writer, unsigned long entry,
						  unsigned long rva, unsigned long size);

/*
 * Set the entry point token within the IL runtime header.
 */
void ILWriterSetEntryPoint(ILWriter *writer, ILMethod *method);

/*
 * Specify the RVA of a token reference that needs to be fixed
 * up when the image is finally written out.  The contents
 * of code section at "rva" are replaced with the fixed-up
 * token code that corresponds to "item".
 */
void ILWriterSetFixup(ILWriter *writer, unsigned long rva,
					  ILProgramItem *item);

/*
 * Add a string to the debug section's string table.
 * Returns the string offset.
 */
ILUInt32 ILWriterDebugString(ILWriter *writer, const char *str);

/*
 * Add debug information for a token to the debug section.
 */
void ILWriterDebugAdd(ILWriter *writer, ILProgramItem *item, int type,
					  const void *info, ILUInt32 len);

/*
 * Add debug information for a pseudo-token to the debug section.
 */
void ILWriterDebugAddPseudo(ILWriter *writer, unsigned long token, int type,
					  		const void *info, ILUInt32 len);

/*
 * Create a resource section handler for an image, in writing mode.
 * Returns NULL if out of memory.
 */
ILResourceSection *ILResourceSectionCreateWriter(ILImage *image);

/*
 * Add an entry to a resource section handler.  Returns a cookie
 * for adding bytes to the entry, or NULL if out of memory.
 */
void *ILResourceSectionAddEntry(ILResourceSection *section, const char *name);

/*
 * Add bytes to a resource section entry.  Returns zero if out of memory.
 */
int ILResourceSectionAddBytes(void *entry, const void *buffer, int len);

/*
 * Flush the contents of a resource section to a writer.
 */
void ILResourceSectionFlush(ILResourceSection *section, ILWriter *writer);

/*
 * Initialize the constant pool attached to the given class and
 * adds the first entry
 */
void ILJavaInitPool(ILWriter *writer, ILClass *info);

/*
 * Append code to the code buffer of the given class and method.
 * The code buffer of a method is in a linked list inside the first
 * constant pool entry of the class.
 */
void ILJavaAppendCode(ILWriter *writer, ILClass *info, ILMethod *method, const void *buffer,
					  unsigned long size);

/*
 * Set a float value in a Java constant pool entry.
 * Return the allocated pool index, or 0 on failure.
 */
ILUInt32 ILJavaSetUTF8String(ILWriter *writer, ILClass *info, 
							 const char *value, ILUInt32 len);

/*
 * Set a signature value in a Java constant pool entry.
 * Return the allocated pool index, or 0 on failure.
 */
ILUInt32 ILJavaSetSignature(ILWriter *writer, ILClass *info, ILType *sig);

/*
 * Set a class value in a Java constant pool entry.
 * The class is an ILClass type.
 * Return the allocated pool index, or 0 on failure.
 */
ILUInt32 ILJavaSetClass(ILWriter *writer, ILClass *info, ILClass *class);

/*
 * Set a class value in a Java constant pool entry.
 * The class is an ILType type.
 * Return the allocated pool index, or 0 on failure.
 */
ILUInt32 ILJavaSetClassFromType(ILWriter *writer, ILClass *info, ILType *type);

/*
 * Set a class value in a Java constant pool entry.
 * The class is a string in the java form (i.e. "java/lang/Object").
 */
ILUInt32 ILJavaSetClassFromName(ILWriter *writer, ILClass *info,
								const char *name);

/*
 * Set a name and type value in a Java constant pool entry.
 * Return the allocated pool index, or 0 on failure.
 */
ILUInt32 ILJavaSetNameAndType(ILWriter *writer, ILClass *info, ILUInt32 nameIndex, 
							  ILUInt32 sigIndex);
	
/*
 * Set a methodref of fieldref value in a Java constant pool entry.
 * Return the allocated pool index, or 0 on failure.
 */
ILUInt32 ILJavaSetref(ILWriter *writer, ILClass *info, int type, ILClass *owner, char *name,
					  ILType *sig);
/*
 * Set a methodref of fieldref value in a Java constant pool entry.
 * Class, method/field name and signature are string in java form (i.e. "(I)V").
 * Return the allocated pool index, or 0 on failure.
 */
ILUInt32 ILJavaSetrefFromName(ILWriter *writer, ILClass *info, int type, 
							  const char *className, const char *refName,
							  const char *sigName);

/*
 * ILJavaSetXXX: set the corresponding value type in a constant pool entry.
 * For example ILJavaSetInteger sets an integer in the constant pool entry.
 * Return the allocated pool index, or 0 on failure.
 */
#define	ILJAVA_SET_PROTO(name, typeName, fieldName, constName)                  \
int ILJavaSet##name(ILWriter *writer, ILClass *info, typeName value);

ILJAVA_SET_PROTO(Integer, ILInt32,  intValue,    INTEGER)
ILJAVA_SET_PROTO(Long,    ILInt64,  longValue,   LONG)
ILJAVA_SET_PROTO(Float,   ILFloat,  floatValue,  FLOAT)
ILJAVA_SET_PROTO(Double,  ILDouble, doubleValue, DOUBLE)
ILJAVA_SET_PROTO(String,  ILInt32,  strValue,    STRING)

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_WRITER_H */
