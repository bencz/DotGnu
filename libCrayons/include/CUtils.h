/*
 * CUtils.h - Utilities header.
 *
 * Copyright (C) 2005  Free Software Foundation, Inc.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

#ifndef _C_UTILS_H_
#define _C_UTILS_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Opaque handle to IO resource. */
typedef void *CIOHandle;

/* Invalid IO handle. */
#define CIOHandle_Invalid ((CIOHandle)-1)

#ifdef C_SYSTEM_WIN32_NATIVE
	#define CUtils_DirectorySeparator '\\'
#else
	#define CUtils_DirectorySeparator '/'
#endif

/*\
|*| NOTE: the fast inline hash should be used like so:
|*|
|*|         hash = init
|*|         foreach(byte in data)
|*|           hash = oper(hash, byte)
\*/

/* initialization value for fast inline hash */
#define CUtils_HashFast_Init  ((CUInt32)0x811C9DC5)

/* prime number value for fast inline hash */
#define CUtils_HashFast_Prime ((CUInt32)16777619)

/* hash operation for fast inline hash */
#define CUtils_HashFast_Oper(hash, byte) \
	((((CUInt32)(hash)) ^ ((CByte)(byte))) * CUtils_HashFast_Prime)


/* initialization value for best hash */
#define CUtils_HashBest_Init ((CUInt32)0x9E3779B9)

CINTERNAL CStatus
CUtils_DeleteFile(const CChar8 *path);
CINTERNAL CBool
CUtils_FileExists(const CChar8 *path);
CINTERNAL CBool
CUtils_DirectoryExists(const CChar8 *path);
CINTERNAL CStatus
CUtils_CreateTemporaryFile(CChar8    **filename,
                           CIOHandle  *handle);
CINTERNAL CStatus
CUtils_CloseIOHandle(CIOHandle handle);
CINTERNAL CStatus
CUtils_WriteIOHandle(CIOHandle    handle,
                     const CByte *memory,
                     CUInt32      length,
                     CUInt32     *written);
CINTERNAL CFloat
CUtils_ConvertUnits(CGraphicsUnit fromUnit,
                    CGraphicsUnit toUnit,
                    CFloat        value);
CINTERNAL CFloat
CUtils_ConvertUnitsDPI(CGraphicsUnit fromUnit,
                       CGraphicsUnit toUnit,
                       CFloat        value,
                       CFloat        fromDpiY,
                       CFloat        toDpiY);
CINTERNAL CUInt32
CUtils_Char16ToChar32(const CChar16 *src,
                      CChar32       *dst,
                      CUInt32        len);
CINTERNAL CStatus
CUtils_Str8ToStr16(const CChar8  *string,
                   CChar16      **result);
CINTERNAL CStatus
CUtils_Str16ToStr8(const CChar16  *string,
                   CChar8        **result);
CINTERNAL CUInt32
CUtils_HashBest(const CByte *data,
                CUInt32      length,
                CUInt32      init);
CINTERNAL CStatus
CUtils_ToPixmanImage(CPixelFormat    format,
                     CByte          *scan0,
                     pixman_image_t *image,
                     CUInt32         x,
                     CUInt32         y,
                     CUInt32         width,
                     CUInt32         height,
                     CUInt32         stride,
                     CColorPalette  *palette);
CINTERNAL CStatus
CUtils_FromPixmanImage(CPixelFormat    format,
                       pixman_image_t *image,
                       CByte          *scan0,
                       CUInt32         x,
                       CUInt32         y,
                       CUInt32         width,
                       CUInt32         height,
                       CUInt32         stride,
                       CColorPalette  *palette);
CINTERNAL CStatus
CUtils_GetPixmanPixelPointer(pixman_image_t  *image,
                             CUInt32          x,
                             CUInt32          y,
                             CColor         **pixel);
CINTERNAL CUInt32
CUtils_FormatToStride(CPixelFormat pixelFormat,
                      CUInt32      width);
CINTERNAL CUInt32
CUtils_BytesPerLine(CPixelFormat pixelFormat,
                    CUInt32      width);
CINTERNAL pixman_transform_t
CUtils_ToPixmanTransform(CAffineTransformF *transform);
CINTERNAL CStatus
CUtils_CreateSolidPattern(pixman_image_t **pattern,
                          CColor           color);
CStatus
CUtils_CreateHatchPattern(pixman_image_t **pattern,
                          const CByte     *bits,
                          CUInt16          width,
                          CUInt16          height,
                          CColor           fg,
                          CColor           bg,
                          CBool            repeat);
CINTERNAL pixman_color_t
CUtils_ToPixmanColor(CColor color);
CINTERNAL CStatus
CUtils_PixmanImageRectangle(pixman_image_t *src,
                            pixman_image_t *dst,
                            CUInt32         x,
                            CUInt32         y,
                            CUInt32         width,
                            CUInt32         height);
CINTERNAL void
CUtils_ReverseBytes(CByte   *bits,
                    CUInt32  length);
CINTERNAL CUInt32
CUtils_NextTwinPrime(CUInt32 num);
CINTERNAL CBool
CUtils_UseGray(CSmoothingMode   smoothing,
                CPixelOffsetMode pixelOffset);
CINTERNAL CBool
CUtils_IsLittleEndian(void);

#ifdef __cplusplus
};
#endif

#endif /* _C_UTILS_H_ */
