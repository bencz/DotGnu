/*
 * il_dumpasm.h - Routines for dumping program structures in assembly code.
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

#ifndef	_IL_DUMPASM_H
#define	_IL_DUMPASM_H

#include "il_program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Flags that can be used to alter the output.
 */
#define	IL_DUMP_SHOW_TOKENS			1
#define	IL_DUMP_QUOTE_NAMES			2
#define	IL_DUMP_CLASS_PREFIX		4
#define	IL_DUMP_GENERIC_PARAMS		8
#define	IL_DUMP_XML_QUOTING			16
#define	IL_DUMP_C_TYPES				32

/*
 * Extra flags for altering the output, in addition to "IL_DUMP_xxx".
 */
#define	ILDASM_REAL_OFFSETS			(1 << 8)
#define	ILDASM_SUPPRESS_PREFIX		(1 << 9)
#define	ILDASM_NO_IL				(1 << 10)
#define	ILDASM_INSTRUCTION_BYTES	(1 << 11)
#define ILDASM_RESOLVE_ALL			(1 << 12)

/*
 * Structure of a flag information block.  This is used when
 * dumping the contents of a flag mask.  If "mask" is zero,
 * then "flag" indicates the flag bit to be selected.  If "mask"
 * is zero, then "flags" indicates the flag bit to be selected
 * if it is zero.  Otherwise, use "(value & mask) == flag" to
 * determine a match.  If "name" starts with "/", then the name
 * is normally suppressed.  If "name" starts with "*", then the
 * value corresponds to an invalid bit value.  Lists are terminated
 * with "name == NULL".  The "mask" field of the final entry
 * indicates the reserved bit positions, and "flag" indicates
 * bit positions that overlap with the reserved space, but
 * which actually correspond to real bits.
 */
typedef struct _tagILFlagInfo
{
	const char	   *name;
	unsigned long	flag;
	unsigned long	mask;

} ILFlagInfo;

/*
 * Built in flag information tables.
 */
extern ILFlagInfo const ILTypeDefinitionFlags[];
extern ILFlagInfo const ILExportedTypeDefinitionFlags[];
extern ILFlagInfo const ILFieldDefinitionFlags[];
extern ILFlagInfo const ILMethodDefinitionFlags[];
extern ILFlagInfo const ILParameterDefinitionFlags[];
extern ILFlagInfo const ILPropertyDefinitionFlags[];
extern ILFlagInfo const ILEventDefinitionFlags[];
extern ILFlagInfo const ILMethodSemanticsFlags[];
extern ILFlagInfo const ILMethodImplementationFlags[];
extern ILFlagInfo const ILMethodCallConvFlags[];
extern ILFlagInfo const ILSecurityFlags[];
extern ILFlagInfo const ILPInvokeImplementationFlags[];
extern ILFlagInfo const ILAssemblyFlags[];
extern ILFlagInfo const ILAssemblyRefFlags[];
extern ILFlagInfo const ILManifestResFlags[];
extern ILFlagInfo const ILFileFlags[];
extern ILFlagInfo const ILVtableFixupFlags[];

/*
 * Dump the contents of a flag mask according to a definition table.
 * If "suppressed" is non-zero, then also dump the suppressed flags.
 */
void ILDumpFlags(FILE *stream, unsigned long flags, const ILFlagInfo *table,
				 int suppressed);

/*
 * Dump an identifier (with an optional namespace) to an output stream.
 * The "flags" value is a combination of the "IL_DUMP_xxx" flags.
 */
void ILDumpIdentifier(FILE *stream, const char *name,
					  const char *nspace, int flags);

/*
 * Dump a class name to an output stream, together with
 * any module/assembly qualifiers that are necessary.
 */
void ILDumpClassName(FILE *stream, ILImage *image, ILClass *info, int flags);

/*
 * Dump a program item which can be a typespec or class like it's used
 * with generics for class' parents or implemented interfaces.
 */
void ILDumpProgramItem(FILE *stream, ILImage *image,
					    ILProgramItem *item, int flags);

/*
 * Dump a simple NUL-terminated string to an output stream.
 */
void ILDumpString(FILE *stream, const char *str);

/*
 * Dump a length-specified string to an output stream.
 */
void ILDumpStringLen(FILE *stream, const char *str, int len);

/*
 * Dump a Unicode string blob value.
 */
void ILDumpUnicodeString(FILE *stream, const char *str,
						 unsigned long numChars);

/*
 * Dump a length-specified string to the output stream using xml quoting.
 */
void ILDumpXmlStringLen(FILE *stream, const char *str, int len);

/*
 * Dump a simple NUL-terminated string to the output stream using xml quoting.
 */
void ILDumpXmlString(FILE *stream, const char *str);

/*
 * Dump a GUID to an output stream.
 */
void ILDumpGUID(FILE *stream, const unsigned char *guid);

/*
 * Dump a type to an output stream.
 */
void ILDumpType(FILE *stream, ILImage *image, ILType *type, int flags);

/*
 * Dump a method type to an output stream.  Insert a
 * class name and method name at the appropriate position.
 */
void ILDumpMethodType(FILE *stream, ILImage *image, ILType *type, int flags,
					  ILClass *info, const char *methodName,
					  ILMethod *methodInfo);

/*
 * Dump a generic method call specification to an output stream.
 */
void ILDumpMethodSpec(FILE *stream, ILImage *image,
					  ILMethodSpec *spec, int flags);

/*
 * Dump a native type to an output stream.
 */
void ILDumpNativeType(FILE *stream, const void *type,
					  unsigned long len, int flags);

/*
 * Dump the constant value associated with a program item.
 * If "hexFloats" is non-zero, then dump floating-point
 * values in hexadecimal.
 */
void ILDumpConstant(FILE *stream, ILProgramItem *item, int hexFloats);

/*
 * Dump a binary blob to an output stream.
 */
void ILDAsmDumpBinaryBlob(FILE *outstream, ILImage *image,
						  const void *blob, ILUInt32 blobLen);

/*
 * Walk a list of tokens and call a supplied callback for each one.
 */
typedef void (*ILDAsmWalkFunc)(ILImage *image, FILE *outstream, int flags,
							   unsigned long token, void *data,
							   unsigned long refToken);
void ILDAsmWalkTokens(ILImage *image, FILE *outstream, int flags,
					  unsigned long tokenKind, ILDAsmWalkFunc callback,
					  unsigned long refToken);

/*
 * Dump the contents of a method.
 */
void ILDAsmDumpMethod(ILImage *image, FILE *outstream,
					  ILMethod *method, int flags,
					  int isEntryPoint, int dumpOffsets);

/*
 * Dump the contents of a Java method.
 */
void ILDAsmDumpJavaMethod(ILImage *image, FILE *outstream,
					      ILMethod *method, int flags);

/*
 * Dump custom attributes for a program item.
 */
void ILDAsmDumpCustomAttrs(ILImage *image, FILE *outstream, int flags,
						   int indent, ILProgramItem *item);

/*
 * Dump global definitions such as modules, assemblies, etc.
 */
void ILDAsmDumpGlobal(ILImage *image, FILE *outstream, int flags);

/*
 * Dump all class definitions.
 */
void ILDAsmDumpClasses(ILImage *image, FILE *outstream, int flags);

/*
 * Dump the security information associated with a program item.
 */
void ILDAsmDumpSecurity(ILImage *image, FILE *outstream,
						ILProgramItem *item, int flags);

/*
 * Dump the generic parameters of a generic class or method.
 */
void ILDAsmDumpGenericParams(ILImage *image, FILE *outstream,
							 ILProgramItem *item, int flags);

/*
 * Dump the ".data" and ".tls" sections.
 */
void ILDAsmDumpDataSections(FILE *outstream, ILImage *image);


#ifdef	__cplusplus
};
#endif

#endif	/* _IL_DUMPASM_H */
