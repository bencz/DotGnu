/*
 * ilasm_data.h - Handle ".data" sections.
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

#ifndef	_ILASM_DATA_H
#define	_ILASM_DATA_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Reset the data output routines to the startup default.
 */
void ILAsmDataReset(void);

/*
 * Set the output data section to normal initialized data.
 */
void ILAsmDataSetNormal(void);

/*
 * Set the output data section to thread-specific data.
 */
void ILAsmDataSetTLS(void);

/*
 * Set a label within the current output data section.
 */
void ILAsmDataSetLabel(const char *name);

/*
 * Resolve a data section label to a data RVA.
 * Returns '-1' if label is not found
 */
ILInt64 ILAsmDataResolveLabel(const char *name);

/*
 * Pad the data section with a number of zero bytes.
 */
void ILAsmDataPad(ILUInt32 size);

/*
 * Write a buffer of bytes to the data section.
 */
void ILAsmDataWriteBytes(const ILUInt8 *buf, ILUInt32 len);

/*
 * Write an "int8" value to the data section a certain number of times.
 */
void ILAsmDataWriteInt8(ILInt32 value, ILUInt32 num);

/*
 * Write an "int16" value to the data section a certain number of times.
 */
void ILAsmDataWriteInt16(ILInt32 value, ILUInt32 num);

/*
 * Write an "int32" value to the data section a certain number of times.
 */
void ILAsmDataWriteInt32(ILInt32 value, ILUInt32 num);

/*
 * Write an "int64" value to the data section a certain number of times.
 */
void ILAsmDataWriteInt64(ILInt64 value, ILUInt32 num);

/*
 * Write a "float32" value to the data section a certain number of times.
 */
void ILAsmDataWriteFloat32(ILUInt8 *value, ILUInt32 num);

/*
 * Write a "float64" value to the data section a certain number of times.
 */
void ILAsmDataWriteFloat64(ILUInt8 *value, ILUInt32 num);

#ifdef	__cplusplus
};
#endif

#endif /* _ILASM_DATA */
