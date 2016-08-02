/*
 * Crayons.h - Main library header.
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

#ifndef _CRAYONS_H_
#define _CRAYONS_H_

#include "CrayonsFeatures.h"

#ifdef __cplusplus
extern "C" {
#endif

/*\
|*| NOTE: The CRAYONS_* defines/ifdefs here are a hack to get something akin
|*|       to C# region blocks; they serve a purely aesthetic purpose.
\*/

/******************************************************************************/
#define CRAYONS_VERSION_INFO
#ifdef CRAYONS_VERSION_INFO
/* Make a crayons version number from version number components. */
#define CrayonsMakeVersion(major, minor, micro) \
	(((major) * 10000) + \
	 ((minor) *   100) + \
	 ((micro) *     1))

/* Define the crayons version. */
#define CrayonsVersion \
	(CrayonsMakeVersion \
		(CrayonsVersionMajor, \
		 CrayonsVersionMinor, \
		 CrayonsVersionMicro))
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_BASIC_TYPES
#ifdef CRAYONS_BASIC_TYPES
/* Define the 8-bit alias types. */
typedef CInt8  CSByte;
typedef CUInt8 CByte;

/* Define the boolean type. */
typedef CUInt8 CBool;

/* Define the character types. */
typedef CUInt8  CChar8;
typedef CUInt16 CChar16;
typedef CUInt32 CChar32;

/* Define miscellaneous 32-bit numeric types. */
typedef CUInt32 CColor;
typedef CUInt32 CLanguageID;
typedef CUInt32 CPropertyID;
typedef CUInt32 CGraphicsContainer;

/* Define the basic floating point types. */
typedef float  CFloat;
typedef double CDouble;
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_OPAQUE_TYPES
#ifdef CRAYONS_OPAQUE_TYPES
/* Define opaque types. */
typedef struct _tagCAdjustableArrowCap CAdjustableArrowCap;
typedef struct _tagCImage              CBitmap;
typedef struct _tagCBitmapSurface      CBitmapSurface;
typedef struct _tagCBrush              CBrush;
typedef struct _tagCCustomLineCap      CCustomLineCap;
typedef struct _tagCFont               CFont;
typedef struct _tagCFontFamily         CFontFamily;
typedef struct _tagCFontCollection     CFontCollection;
typedef struct _tagCGraphics           CGraphics;
typedef struct _tagCHatchBrush         CHatchBrush;
typedef struct _tagCImage              CImage;
typedef struct _tagCImageAttributes    CImageAttributes;
typedef struct _tagCLineBrush          CLineBrush;
typedef struct _tagCMatrix             CMatrix;
typedef struct _tagCPath               CPath;
typedef struct _tagCPathBrush          CPathBrush;
typedef struct _tagCPathIterator       CPathIterator;
typedef struct _tagCPen                CPen;
typedef struct _tagCRegion             CRegion;
typedef struct _tagCSolidBrush         CSolidBrush;
typedef struct _tagCStringFormat       CStringFormat;
typedef struct _tagCSurface            CSurface;
typedef struct _tagCTextureBrush       CTextureBrush;
typedef struct _tagCX11Surface         CX11Surface;
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_ENUMERATIONS
#ifdef CRAYONS_ENUMERATIONS
/* Define enumeration types. */
typedef CUInt32 CBrushType;
#define CBrushType_SolidFill      0
#define CBrushType_HatchFill      1
#define CBrushType_TextureFill    2
#define CBrushType_PathGradient   3
#define CBrushType_LinearGradient 4

typedef CUInt32 CColorAdjustType;
#define CColorAdjustType_Default   0
#define CColorAdjustType_Bitmap    1
#define CColorAdjustType_Brush     2
#define CColorAdjustType_Pen       3
#define CColorAdjustType_Text      4
#define CColorAdjustType_Count     5
#define CColorAdjustType_Any       6

typedef CUInt32 CColorChannelFlag;
#define CColorChannelFlag_C    0
#define CColorChannelFlag_M    1
#define CColorChannelFlag_Y    2
#define CColorChannelFlag_K    3
#define CColorChannelFlag_Last 4

typedef CUInt32 CCombineMode;
#define CCombineMode_Replace    0
#define CCombineMode_Intersect  1
#define CCombineMode_Union      2
#define CCombineMode_Xor        3
#define CCombineMode_Exclude    4
#define CCombineMode_Complement 5

typedef CUInt32 CColorMatrixFlag;
#define CColorMatrixFlag_Default   0
#define CColorMatrixFlag_SkipGrays 1
#define CColorMatrixFlag_AltGray   2

typedef CUInt32 CCompositingMode;
#define CCompositingMode_SourceOver 0
#define CCompositingMode_SourceCopy 1
#define CCompositingMode_Xor        2

typedef CUInt32 CCompositingQuality;
#define CCompositingQuality_Default        0
#define CCompositingQuality_HighSpeed      1
#define CCompositingQuality_HighQuality    2
#define CCompositingQuality_GammaCorrected 3
#define CCompositingQuality_AssumeLinear   4

typedef CUInt32 CCoordinateSpace;
#define CCoordinateSpace_World  0
#define CCoordinateSpace_Page   1
#define CCoordinateSpace_Device 2

typedef CUInt32 CDashCap;
#define CDashCap_Flat     0
#define CDashCap_Round    2
#define CDashCap_Triangle 3

typedef CUInt32 CDashStyle;
#define CDashStyle_Solid       0
#define CDashStyle_Dot         1
#define CDashStyle_Dash        2
#define CDashStyle_DashDot     3
#define CDashStyle_DashDashDot 4
#define CDashStyle_Custom      5

typedef CUInt32 CDigitSubstitute;
#define CDigitSubstitute_User        0
#define CDigitSubstitute_None        1
#define CDigitSubstitute_National    2
#define CDigitSubstitute_Traditional 3

typedef CUInt32 CFillMode;
#define CFillMode_Alternate 0
#define CFillMode_Winding   1

typedef CUInt32 CFlushIntention;
#define CFlushIntention_Flush 0
#define CFlushIntention_Sync  1

typedef CUInt32 CFontStyle;
#define CFontStyle_Regular   0
#define CFontStyle_Bold      1
#define CFontStyle_Italic    2
#define CFontStyle_Underline 4
#define CFontStyle_Strikeout 8

typedef CUInt32 CFontFamilyGeneric;
#define CFontFamilyGeneric_Serif     0
#define CFontFamilyGeneric_SansSerif 1
#define CFontFamilyGeneric_Monospace 2

typedef CUInt32 CGraphicsUnit;
#define CGraphicsUnit_World      0
#define CGraphicsUnit_Display    1
#define CGraphicsUnit_Pixel      2
#define CGraphicsUnit_Point      3
#define CGraphicsUnit_Inch       4
#define CGraphicsUnit_Document   5
#define CGraphicsUnit_Millimeter 6

typedef CUInt32 CHatchStyle;
#define CHatchStyle_Horizontal             0
#define CHatchStyle_Vertical               1
#define CHatchStyle_ForwardDiagonal        2
#define CHatchStyle_BackwardDiagonal       3
#define CHatchStyle_Cross                  4
#define CHatchStyle_LargeGrid              CHatchStyle_Cross
#define CHatchStyle_DiagonalCross          5
#define CHatchStyle_Percent05              6
#define CHatchStyle_Percent10              7
#define CHatchStyle_Percent20              8
#define CHatchStyle_Percent25              9
#define CHatchStyle_Percent30              10
#define CHatchStyle_Percent40              11
#define CHatchStyle_Percent50              12
#define CHatchStyle_Percent60              13
#define CHatchStyle_Percent70              14
#define CHatchStyle_Percent75              15
#define CHatchStyle_Percent80              16
#define CHatchStyle_Percent90              17
#define CHatchStyle_LightDownwardDiagonal  18
#define CHatchStyle_LightUpwardDiagonal    19
#define CHatchStyle_DarkDownwardDiagonal   20
#define CHatchStyle_DarkUpwardDiagonal     21
#define CHatchStyle_WideDownwardDiagonal   22
#define CHatchStyle_WideUpwardDiagonal     23
#define CHatchStyle_LightVertical          24
#define CHatchStyle_LightHorizontal        25
#define CHatchStyle_NarrowVertical         26
#define CHatchStyle_NarrowHorizontal       27
#define CHatchStyle_DarkVertical           28
#define CHatchStyle_DarkHorizontal         29
#define CHatchStyle_DashedDownwardDiagonal 30
#define CHatchStyle_DashedUpwardDiagonal   31
#define CHatchStyle_DashedHorizontal       32
#define CHatchStyle_DashedVertical         33
#define CHatchStyle_SmallConfetti          34
#define CHatchStyle_LargeConfetti          35
#define CHatchStyle_ZigZag                 36
#define CHatchStyle_Wave                   37
#define CHatchStyle_DiagonalBrick          38
#define CHatchStyle_HorizontalBrick        39
#define CHatchStyle_Weave                  40
#define CHatchStyle_Plaid                  41
#define CHatchStyle_Divot                  42
#define CHatchStyle_DottedGrid             43
#define CHatchStyle_DottedDiamond          44
#define CHatchStyle_Shingle                45
#define CHatchStyle_Trellis                46
#define CHatchStyle_Sphere                 47
#define CHatchStyle_SmallGrid              48
#define CHatchStyle_SmallCheckerBoard      49
#define CHatchStyle_LargeCheckerBoard      50
#define CHatchStyle_OutlinedDiamond        51
#define CHatchStyle_SolidDiamond           52
#define CHatchStyle_Min                    CHatchStyle_Horizontal
#define CHatchStyle_Max                    CHatchStyle_SolidDiamond

typedef CUInt32 CHotkeyPrefix;
#define CHotkeyPrefix_None 0
#define CHotkeyPrefix_Show 1
#define CHotkeyPrefix_Hide 2

typedef CUInt32 CImageFlag;
#define CImageFlag_None              0x00000000
#define CImageFlag_Scalable          0x00000001
#define CImageFlag_HasAlpha          0x00000002
#define CImageFlag_HasTranslucent    0x00000004
#define CImageFlag_PartiallyScalable 0x00000008
#define CImageFlag_ColorSpaceRGB     0x00000010
#define CImageFlag_ColorSpaceCMYK    0x00000020
#define CImageFlag_ColorSpaceGRAY    0x00000040
#define CImageFlag_ColorSpaceYCBCR   0x00000080
#define CImageFlag_ColorSpaceYCCK    0x00000100
#define CImageFlag_HasRealDPI        0x00001000
#define CImageFlag_HasRealPixelSize  0x00002000
#define CImageFlag_ReadOnly          0x00010000
#define CImageFlag_Caching           0x00020000

typedef CUInt32 CImageLockMode;
#define CImageLockMode_None      0
#define CImageLockMode_ReadOnly  1
#define CImageLockMode_WriteOnly 2
#define CImageLockMode_ReadWrite 3

typedef CUInt32 CImageType;
#define CImageType_Unknown  0
#define CImageType_Bitmap   1
#define CImageType_Metafile 2

typedef CUInt32 CInterpolationMode;
#define CInterpolationMode_Default             0
#define CInterpolationMode_LowQuality          1
#define CInterpolationMode_HighQuality         2
#define CInterpolationMode_Bilinear            3
#define CInterpolationMode_Bicubic             4
#define CInterpolationMode_NearestNeighbor     5
#define CInterpolationMode_HighQualityBilinear 6
#define CInterpolationMode_HighQualityBicubic  7

typedef CUInt32 CLineCap;
#define CLineCap_Flat          0x00
#define CLineCap_Square        0x01
#define CLineCap_Round         0x02
#define CLineCap_Triangle      0x03
#define CLineCap_NoAnchor      0x10
#define CLineCap_SquareAnchor  0x11
#define CLineCap_RoundAnchor   0x12
#define CLineCap_DiamondAnchor 0x13
#define CLineCap_ArrowAnchor   0x14
#define CLineCap_AnchorMask    0xF0
#define CLineCap_Custom        0xFF

typedef CUInt32 CLineJoin;
#define CLineJoin_Miter        0
#define CLineJoin_Bevel        1
#define CLineJoin_Round        2
#define CLineJoin_MiterClipped 3

typedef CUInt32 CMatrixOrder;
#define CMatrixOrder_Prepend 0
#define CMatrixOrder_Append  1

typedef CUInt32 CPaletteFlag;
#define CPaletteFlag_HasAlpha  1
#define CPaletteFlag_GrayScale 2
#define CPaletteFlag_Halftone  4

typedef CUInt32 CPathType;
#define CPathType_Start        0x00
#define CPathType_Line         0x01
#define CPathType_Bezier       0x03
#define CPathType_TypeMask     0x07
#define CPathType_DashMode     0x10
#define CPathType_PathMarker   0x20
#define CPathType_CloseSubpath 0x80

typedef CUInt32 CPenAlignment;
#define CPenAlignment_Center 0
#define CPenAlignment_Inset  1
#define CPenAlignment_Outset 2
#define CPenAlignment_Left   3
#define CPenAlignment_Right  4

typedef CUInt32 CPenType;
#define CPenType_SolidFill      CBrushType_SolidFill
#define CPenType_HatchFill      CBrushType_HatchFill
#define CPenType_TextureFill    CBrushType_TextureFill
#define CPenType_PathGradient   CBrushType_PathGradient
#define CPenType_LinearGradient CBrushType_LinearGradient

/*\
|*| CPixelFormat format:
|*|
|*|   byte 0 = index
|*|   byte 1 = bits per pixel
|*|   byte 2 = flags
|*|   byte 3 = reserved
\*/
typedef CUInt32 CPixelFormat;
#define CPixelFormat_Undefined      0x00000000
#define CPixelFormat_DontCare       0x00000000
#define CPixelFormat_Indexed        0x00010000
#define CPixelFormat_Gdi            0x00020000
#define CPixelFormat_Alpha          0x00040000
#define CPixelFormat_PAlpha         0x00080000
#define CPixelFormat_Extended       0x00100000
#define CPixelFormat_Canonical      0x00200000
#define CPixelFormat_1bppIndexed    0x00030101
#define CPixelFormat_4bppIndexed    0x00030402
#define CPixelFormat_8bppIndexed    0x00030803
#define CPixelFormat_16bppGrayScale 0x00101004
#define CPixelFormat_16bppRgb555    0x00021005
#define CPixelFormat_16bppRgb565    0x00021006
#define CPixelFormat_16bppArgb1555  0x00061007
#define CPixelFormat_24bppRgb       0x00021808
#define CPixelFormat_32bppRgb       0x00022009
#define CPixelFormat_32bppArgb      0x0026200A
#define CPixelFormat_32bppPArgb     0x000E200B
#define CPixelFormat_48bppRgb       0x0010300C
#define CPixelFormat_64bppArgb      0x0034400D
#define CPixelFormat_64bppPArgb     0x001C400E
#define CPixelFormat_Max            0x0000000F

typedef CUInt32 CPixelOffsetMode;
#define CPixelOffsetMode_Default     0
#define CPixelOffsetMode_HighSpeed   1
#define CPixelOffsetMode_HighQuality 2
#define CPixelOffsetMode_None        3
#define CPixelOffsetMode_Half        4

typedef CUInt32 CRotateFlipType;
#define CRotateFlipType_RotateNoneFlipNone 0
#define CRotateFlipType_Rotate90FlipNone   1
#define CRotateFlipType_Rotate180FlipNone  2
#define CRotateFlipType_Rotate270FlipNone  3
#define CRotateFlipType_RotateNoneFlipX    4
#define CRotateFlipType_Rotate90FlipX      5
#define CRotateFlipType_Rotate180FlipX     6
#define CRotateFlipType_Rotate270FlipX     7
#define CRotateFlipType_Rotate180FlipXY    CRotateFlipType_RotateNoneFlipNone
#define CRotateFlipType_Rotate270FlipXY    CRotateFlipType_Rotate90FlipNone
#define CRotateFlipType_RotateNoneFlipXY   CRotateFlipType_Rotate180FlipNone
#define CRotateFlipType_Rotate90FlipXY     CRotateFlipType_Rotate270FlipNone
#define CRotateFlipType_Rotate180FlipY     CRotateFlipType_RotateNoneFlipX
#define CRotateFlipType_Rotate270FlipY     CRotateFlipType_Rotate90FlipX
#define CRotateFlipType_RotateNoneFlipY    CRotateFlipType_Rotate180FlipX
#define CRotateFlipType_Rotate90FlipY      CRotateFlipType_Rotate270FlipX

typedef CUInt32 CSmoothingMode;
#define CSmoothingMode_Default     0
#define CSmoothingMode_HighSpeed   1
#define CSmoothingMode_HighQuality 2
#define CSmoothingMode_None        3
#define CSmoothingMode_AntiAlias   4

/*\
|*| CStatus format:
|*|
|*|    lower half = exception
|*|   higher half = message (optional)
\*/
typedef CUInt32 CStatus;
#define CStatus_OK                              0x00000000
#define CStatus_OutOfMemory                     0x00000001
#define CStatus_Argument                        0x00000002
#define CStatus_ArgumentNull                    0x00000003
#define CStatus_ArgumentOutOfRange              0x00000004
#define CStatus_InvalidOperation                0x00000005
#define CStatus_NotImplemented                  0x00000006
#define CStatus_NotSupported                    0x00000007
#define CStatus_IOError                         0x00000008
#define CStatus_Argument_FontFamilyNotFound     0x00010002
#define CStatus_Argument_InvalidPointCount      0x00020002
#define CStatus_Argument_NeedAtLeast2Points     0x00030002
#define CStatus_Argument_NeedAtLeast3Points     0x00040002
#define CStatus_Argument_NeedAtLeast4Points     0x00050002
#define CStatus_Argument_StyleNotAvailable      0x00060002
#define CStatus_InvalidOperation_ImageLocked    0x00010005
#define CStatus_InvalidOperation_SingularMatrix 0x00020005
#define CStatus_IOError_FileNotFound            0x00010008

typedef CUInt32 CStringAlignment;
#define CStringAlignment_Near   0
#define CStringAlignment_Center 1
#define CStringAlignment_Far    2

typedef CUInt32 CStringFormatFlag;
#define CStringFormatFlag_DirectionRightToLeft  0x0001
#define CStringFormatFlag_DirectionVertical     0x0002
#define CStringFormatFlag_NoFitBlackBox         0x0004
#define CStringFormatFlag_DisplayFormatControl  0x0020
#define CStringFormatFlag_NoFontFallback        0x0400
#define CStringFormatFlag_MeasureTrailingSpaces 0x0800
#define CStringFormatFlag_NoWrap                0x1000
#define CStringFormatFlag_LineLimit             0x2000
#define CStringFormatFlag_NoClip                0x4000

typedef CUInt32 CStringTrimming;
#define CStringTrimming_None              0
#define CStringTrimming_Character         1
#define CStringTrimming_Word              2
#define CStringTrimming_EllipsisCharacter 3
#define CStringTrimming_EllipsisWord      4
#define CStringTrimming_EllipsisPath      5

typedef CUInt32 CTextRenderingHint;
#define CTextRenderingHint_SystemDefault            0
#define CTextRenderingHint_SingleBitPerPixelGridFit 1
#define CTextRenderingHint_SingleBitPerPixel        2
#define CTextRenderingHint_AntiAliasGridFit         3
#define CTextRenderingHint_AntiAlias                4
#define CTextRenderingHint_ClearTypeGridFit         5

typedef CUInt32 CWarpMode;
#define CWarpMode_Perspective 0
#define CWarpMode_Bilinear    1

typedef CUInt32 CWrapMode;
#define CWrapMode_Tile       0
#define CWrapMode_TileFlipX  1
#define CWrapMode_TileFlipY  2
#define CWrapMode_TileFlipXY 3
#define CWrapMode_Clamp      4
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_TRANSPARENT_TYPES
#ifdef CRAYONS_TRANSPARENT_TYPES
/* Define transparent types. */
typedef struct _tagCBitmapData CBitmapData;
struct _tagCBitmapData
{
	CUInt32       width;
	CUInt32       height;
	CUInt32       stride;
	CPixelFormat  pixelFormat;
	CByte        *scan0;
	CUInt32       reserved;
};

typedef struct _tagCBlend CBlend;
struct _tagCBlend
{
	CFloat  *factors;
	CFloat  *positions;
	CUInt32  count;
};

typedef struct _tagCCharacterRange CCharacterRange;
struct _tagCCharacterRange
{
	CUInt32 first;
	CUInt32 length;
};

typedef struct _tagCColorBlend CColorBlend;
struct _tagCColorBlend
{
	CColor  *colors;
	CFloat  *positions;
	CUInt32  count;
};

typedef struct _tagCColorMap CColorMap;
struct _tagCColorMap
{
	CColor oldColor;
	CColor newColor;
};

typedef struct _tagCColorMatrix CColorMatrix;
struct _tagCColorMatrix
{
	CFloat m[5][5];
};

typedef struct _tagCColorPalette CColorPalette;
struct _tagCColorPalette
{
	CPaletteFlag  flags;
	CUInt32       count;
	CColor       *colors;
};

typedef struct _tagCFontMetrics CFontMetrics;
struct _tagCFontMetrics
{
	CInt32 cellAscent;
	CInt32 cellDescent;
	CInt32 lineSpacing;
	CInt32 emHeight;
};

typedef struct _tagCGuid CGuid;
struct _tagCGuid
{
	CUInt32 a;
	CUInt16 b;
	CUInt16 c;
	CByte   d;
	CByte   e;
	CByte   f;
	CByte   g;
	CByte   h;
	CByte   i;
	CByte   j;
	CByte   k;
};

typedef CGuid CImageFormat;

typedef struct _tagCPointI CPointI;
struct _tagCPointI
{
	CInt32 x;
	CInt32 y;
};

typedef struct _tagCPointF CPointF;
struct _tagCPointF
{
	CFloat x;
	CFloat y;
};

typedef struct _tagCPropertyItem CPropertyItem;
struct _tagCPropertyItem
{
	CPropertyID  id;
	CUInt32      length;
	CUInt16      type;
	CByte       *value;
};

typedef struct _tagCRectangleF CRectangleF;
struct _tagCRectangleF
{
	CFloat x;
	CFloat y;
	CFloat width;
	CFloat height;
};

typedef struct _tagCSizeF CSizeF;
struct _tagCSizeF
{
	CFloat width;
	CFloat height;
};

#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_CONSTANTS
#ifdef CRAYONS_CONSTANTS
/* Declare various constants. */
extern _Cconst CGuid CFrameDimension_Page;
extern _Cconst CGuid CFrameDimension_Resolution;
extern _Cconst CGuid CFrameDimension_Time;
extern _Cconst CGuid CImageFormat_MemoryBMP;
extern _Cconst CGuid CImageFormat_BMP;
extern _Cconst CGuid CImageFormat_EMF;
extern _Cconst CGuid CImageFormat_WMF;
extern _Cconst CGuid CImageFormat_JPG;
extern _Cconst CGuid CImageFormat_PNG;
extern _Cconst CGuid CImageFormat_GIF;
extern _Cconst CGuid CImageFormat_TIFF;
extern _Cconst CGuid CImageFormat_EXIF;
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_BITMAP_METHODS
#ifdef CRAYONS_BITMAP_METHODS
/* Declare public bitmap methods. */
CStatus
CBitmap_Create(CBitmap      **_this,
               CUInt32        width,
               CUInt32        height,
               CPixelFormat   format);
CStatus
CBitmap_CreateData(CBitmap      **_this,
                    CByte         *data,
                    CUInt32        width,
                    CUInt32        height,
                    CUInt32        stride,
                    CPixelFormat   format);
CStatus
CBitmap_Destroy(CBitmap **_this);
CStatus
CBitmap_Clone(CBitmap       *_this,
              CBitmap      **clone,
              CUInt32        x,
              CUInt32        y,
              CUInt32        width,
              CUInt32        height,
              CPixelFormat   format);
CStatus
CBitmap_GetPixel(CBitmap *_this,
                 CUInt32  x,
                 CUInt32  y,
                 CColor  *color);
CStatus
CBitmap_SetPixel(CBitmap *_this,
                 CUInt32  x,
                 CUInt32  y,
                 CColor   color);
CStatus
CBitmap_LockBits(CBitmap        *_this,
                 CUInt32         x,
                 CUInt32         y,
                 CUInt32         width,
                 CUInt32         height,
                 CImageLockMode  lockMode,
                 CPixelFormat    format,
                 CBitmapData    *bitmapData);
CStatus
CBitmap_SetResolution(CBitmap *_this,
                      CFloat   dpiX,
                      CFloat   dpiY);
CStatus
CBitmap_UnlockBits(CBitmap     *_this,
                   CBitmapData *bitmapData);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_BITMAPSURFACE_METHODS
#ifdef CRAYONS_BITMAPSURFACE_METHODS
/* Declare public bitmap surface methods. */
CStatus
CBitmapSurface_Create(CBitmapSurface **_this,
                      CBitmap         *image);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_BRUSH_METHODS
#ifdef CRAYONS_BRUSH_METHODS
/* Declare public brush methods. */
CStatus
CBrush_GetBrushType(CBrush     *_this,
                    CBrushType *type);
CStatus
CBrush_Clone(CBrush  *_this,
             CBrush **clone);
CStatus
CBrush_Destroy(CBrush **_this);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_COLORPALETTE_METHODS
#ifdef CRAYONS_COLORPALETTE_METHODS
/* Declare public color palette methods. */
CStatus
CColorPalette_Create(CColorPalette **_this,
                     CColor         *colors,
                     CUInt32         count,
                     CPaletteFlag    flags);
CStatus
CColorPalette_Destroy(CColorPalette **_this);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_FONTCOLLECTION_METHODS
#ifdef CRAYONS_FONTCOLLECTION_METHODS
CStatus
CFontCollection_CreateInstalled(CFontCollection **_this);
CStatus
CFontCollection_CreatePrivate(CFontCollection **_this);
CStatus
CFontCollection_Destroy(CFontCollection **_this);
CStatus
CFontCollection_AddFontFile(CFontCollection *_this,
                            _Cconst CChar16 *filename);
CStatus
CFontCollection_AddFontMemory(CFontCollection *_this,
                              _Cconst CByte   *memory,
                              CUInt32          length);
CStatus
CFontCollection_GetFamilyList(CFontCollection   *_this,
                              CFontFamily     ***families,
                              CUInt32           *count);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_FONTFAMILY_METHODS
#ifdef CRAYONS_FONTFAMILY_METHODS
CStatus
CFontFamily_CreateName(CFontFamily     **_this,
                       _Cconst CChar16  *name,
                       CFontCollection  *collection);
CStatus
CFontFamily_CreateGeneric(CFontFamily        **_this,
                          CFontFamilyGeneric   generic);
CStatus
CFontFamily_Destroy(CFontFamily **_this);
CStatus
CFontFamily_GetMetrics(CFontFamily  *_this,
                       CFontStyle    style,
                       CFontMetrics *metrics);
CStatus
CFontFamily_GetName(CFontFamily  *_this,
                    CChar16     **name);
CStatus
CFontFamily_IsStyleAvailable(CFontFamily *_this,
                             CFontStyle   style,
                             CBool       *available);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_FONT_METHODS
#ifdef CRAYONS_FONT_METHODS
CStatus
CFont_Create(CFont         **_this,
             CFontFamily    *family,
             CFontStyle      style,
             CFloat          size,
             CGraphicsUnit   unit);
CStatus
CFont_Destroy(CFont **_this);
CStatus
CFont_Clone(CFont  *_this,
            CFont **clone);
CStatus
CFont_Equals(CFont *_this,
             CFont *other,
             CBool *equal);
CStatus
CFont_GetFontFamily(CFont        *_this,
                    CFontFamily **family);
CStatus
CFont_GetHashCode(CFont   *_this,
                  CUInt32 *hash);
CStatus
CFont_GetHeight(CFont  *_this,
                CFloat *height);
CStatus
CFont_GetHeightDPI(CFont  *_this,
                   CFloat  dpiY,
                   CFloat *height);
CStatus
CFont_GetHeightGraphics(CFont     *_this,
                        CGraphics *graphics,
                        CFloat    *height);
CStatus
CFont_GetName(CFont    *_this,
              CChar16 **name);
CStatus
CFont_GetSize(CFont  *_this,
              CFloat *size);
CStatus
CFont_GetSizeInPoints(CFont  *_this,
                      CFloat *points);
CStatus
CFont_GetStyle(CFont      *_this,
               CFontStyle *style);
CStatus
CFont_GetUnit(CFont         *_this,
              CGraphicsUnit *unit);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_GRAPHICS_METHODS
#ifdef CRAYONS_GRAPHICS_METHODS
/* Declare public graphics methods. */
CStatus
CGraphics_Create(CGraphics **_this,
                 CSurface   *surface);
CStatus
CGraphics_Destroy(CGraphics **_this);
CStatus
CGraphics_GetTransform(CGraphics *_this,
                       CMatrix   *matrix);
CStatus
CGraphics_MultiplyTransform(CGraphics    *_this,
                            CMatrix      *matrix,
                            CMatrixOrder  order);
CStatus
CGraphics_ResetTransform(CGraphics *_this);
CStatus
CGraphics_RotateTransform(CGraphics    *_this,
                          CFloat        angle,
                          CMatrixOrder  order);
CStatus
CGraphics_ScaleTransform(CGraphics    *_this,
                         CFloat        sx,
                         CFloat        sy,
                         CMatrixOrder  order);
CStatus
CGraphics_SetTransform(CGraphics *_this,
                       CMatrix   *matrix);
CStatus
CGraphics_TranslateTransform(CGraphics    *_this,
                             CFloat        dx,
                             CFloat        dy,
                             CMatrixOrder  order);
CStatus
CGraphics_GetClip(CGraphics *_this,
                  CRegion   *region);
CStatus
CGraphics_GetClipBounds(CGraphics   *_this,
                        CRectangleF *clipBounds);
CStatus
CGraphics_GetVisibleClipBounds(CGraphics   *_this,
                               CRectangleF *bounds);
CStatus
CGraphics_IsClipEmpty(CGraphics *_this,
                      CBool     *empty);
CStatus
CGraphics_IsVisibleClipEmpty(CGraphics *_this,
                             CBool     *empty);
CStatus
CGraphics_IsVisiblePoint(CGraphics *_this,
                         CPointF    point,
                         CBool     *visible);
CStatus
CGraphics_IsVisibleRectangle(CGraphics   *_this,
                             CRectangleF  rect,
                             CBool       *visible);
CStatus
CGraphics_ResetClip(CGraphics *_this);
CStatus
CGraphics_SetClipGraphics(CGraphics    *_this,
                          CGraphics    *graphics,
                          CCombineMode  combineMode);
CStatus
CGraphics_SetClipPath(CGraphics    *_this,
                      CPath        *path,
                      CCombineMode  combineMode);
CStatus
CGraphics_SetClipRegion(CGraphics    *_this,
                        CRegion      *region,
                        CCombineMode  combineMode);
CStatus
CGraphics_SetClipRectangle(CGraphics    *_this,
                           CRectangleF   rect,
                           CCombineMode  combineMode);
CStatus
CGraphics_TranslateClip(CGraphics *_this,
                        CFloat     dx,
                        CFloat     dy);
CStatus
CGraphics_GetCompositingMode(CGraphics        *_this,
                             CCompositingMode *compositingMode);
CStatus
CGraphics_SetCompositingMode(CGraphics        *_this,
                             CCompositingMode  compositingMode);
CStatus
CGraphics_GetCompositingQuality(CGraphics           *_this,
                                CCompositingQuality *compositingQuality);
CStatus
CGraphics_SetCompositingQuality(CGraphics           *_this,
                                CCompositingQuality  compositingQuality);
CStatus
CGraphics_GetDpiX(CGraphics *_this,
                  CFloat    *dpiX);
CStatus
CGraphics_GetDpiY(CGraphics *_this,
                  CFloat    *dpiY);
CStatus
CGraphics_GetInterpolationMode(CGraphics          *_this,
                               CInterpolationMode *interpolationMode);
CStatus
CGraphics_SetInterpolationMode(CGraphics          *_this,
                               CInterpolationMode  interpolationMode);
CStatus
CGraphics_GetPageScale(CGraphics *_this,
                       CFloat    *pageScale);
CStatus
CGraphics_SetPageScale(CGraphics *_this,
                       CFloat     pageScale);
CStatus
CGraphics_GetPageUnit(CGraphics     *_this,
                      CGraphicsUnit *pageUnit);
CStatus
CGraphics_SetPageUnit(CGraphics     *_this,
                      CGraphicsUnit  pageUnit);
CStatus
CGraphics_GetPixelOffsetMode(CGraphics        *_this,
                             CPixelOffsetMode *pixelOffsetMode);
CStatus
CGraphics_SetPixelOffsetMode(CGraphics        *_this,
                             CPixelOffsetMode  pixelOffsetMode);
CStatus
CGraphics_GetRenderingOrigin(CGraphics *_this,
                             CPointI   *renderingOrigin);
CStatus
CGraphics_SetRenderingOrigin(CGraphics *_this,
                             CPointI    renderingOrigin);
CStatus
CGraphics_GetSmoothingMode(CGraphics      *_this,
                           CSmoothingMode *smoothingMode);
CStatus
CGraphics_SetSmoothingMode(CGraphics      *_this,
                           CSmoothingMode  smoothingMode);
CStatus
CGraphics_GetTextContrast(CGraphics *_this,
                          CUInt32   *textContrast);
CStatus
CGraphics_SetTextContrast(CGraphics *_this,
                          CUInt32    textContrast);
CStatus
CGraphics_GetTextRenderingHint(CGraphics          *_this,
                               CTextRenderingHint *textRenderingHint);
CStatus
CGraphics_SetTextRenderingHint(CGraphics          *_this,
                               CTextRenderingHint  textRenderingHint);
CStatus
CGraphics_DrawXBM(CGraphics     *_this,
                  _Cconst CByte *bits,
                  CFloat         x,
                  CFloat         y,
                  CUInt16        width,
                  CUInt16        height,
                  CColor         color,
                  CBool          transform);
CStatus
CGraphics_DrawImage(CGraphics *_this,
                    CImage    *image,
                    CFloat     x,
                    CFloat     y);
CStatus
CGraphics_DrawImageRect(CGraphics *_this,
                        CImage    *image,
                        CFloat     x,
                        CFloat     y,
                        CFloat     width,
                        CFloat     height);
CStatus
CGraphics_DrawImagePoints(CGraphics *_this,
                          CImage    *image,
                          CPointF   *dst,
                          CUInt32    count);
CStatus
CGraphics_DrawImageRectPoints(CGraphics     *_this,
                              CImage        *image,
                              CPointF        dst,
                              CRectangleF    src,
                              CGraphicsUnit  srcUnit);
CStatus
CGraphics_DrawImageRectPointsAttributes(CGraphics           *_this,
                                        CImage              *image,
                                        CPointF             *dst,
                                        CUInt32              count,
                                        CRectangleF          src,
                                        CGraphicsUnit        srcUnit,
                                        CImageAttributes    *atts);
CStatus
CGraphics_DrawImageRectRectAttributes(CGraphics           *_this,
                                      CImage              *image,
                                      CRectangleF          dst,
                                      CRectangleF          src,
                                      CGraphicsUnit        srcUnit,
                                      CImageAttributes    *atts);
CStatus
CGraphics_DrawArc(CGraphics   *_this,
                  CPen        *pen,
                  CRectangleF  rect,
                  CFloat       startAngle,
                  CFloat       sweepAngle);
CStatus
CGraphics_DrawBezier(CGraphics *_this,
                     CPen      *pen,
                     CPointF    a,
                     CPointF    b,
                     CPointF    c,
                     CPointF    d);
CStatus
CGraphics_DrawBeziers(CGraphics *_this,
                      CPen      *pen,
                      CPointF   *points,
                      CUInt32    count);
CStatus
CGraphics_DrawClosedCurve(CGraphics *_this,
                          CPen      *pen,
                          CPointF   *points,
                          CUInt32    count,
                          CFloat     tension);
CStatus
CGraphics_DrawCurve(CGraphics *_this,
                    CPen      *pen,
                    CPointF   *points,
                    CUInt32    count,
                    CUInt32    offset,
                    CUInt32    numberOfSegments,
                    CFloat     tension);
CStatus
CGraphics_DrawEllipse(CGraphics   *_this,
                      CPen        *pen,
                      CRectangleF  rect);
CStatus
CGraphics_DrawLine(CGraphics *_this,
                   CPen      *pen,
                   CPointF    start,
                   CPointF    end);
CStatus
CGraphics_DrawLines(CGraphics *_this,
                    CPen      *pen,
                    CPointF   *points,
                    CUInt32    count);
CStatus
CGraphics_DrawPath(CGraphics *_this,
                   CPen      *pen,
                   CPath     *path);
CStatus
CGraphics_DrawPie(CGraphics   *_this,
                  CPen        *pen,
                  CRectangleF  rect,
                  CFloat       startAngle,
                  CFloat       sweepAngle);
CStatus
CGraphics_DrawPolygon(CGraphics *_this,
                      CPen      *pen,
                      CPointF   *points,
                      CUInt32    count);
CStatus
CGraphics_DrawRectangle(CGraphics   *_this,
                        CPen        *pen,
                        CRectangleF  rect);
CStatus
CGraphics_DrawRectangles(CGraphics   *_this,
                         CPen        *pen,
                         CRectangleF *rects,
                         CUInt32      count);
CStatus
CGraphics_Clear(CGraphics *_this,
                CColor     color);
CStatus
CGraphics_FillClosedCurve(CGraphics *_this,
                          CBrush    *brush,
                          CPointF   *points,
                          CUInt32    count,
                          CFloat     tension,
                          CFillMode  fillMode);
CStatus
CGraphics_FillEllipse(CGraphics   *_this,
                      CBrush      *brush,
                      CRectangleF  rect);
CStatus
CGraphics_FillPath(CGraphics *_this,
                   CBrush    *brush,
                   CPath     *path);
CStatus
CGraphics_FillPie(CGraphics   *_this,
                  CBrush      *brush,
                  CRectangleF  rect,
                  CFloat       startAngle,
                  CFloat       sweepAngle);
CStatus
CGraphics_FillPolygon(CGraphics *_this,
                      CBrush    *brush,
                      CPointF   *points,
                      CUInt32    count,
                      CFillMode  fillMode);
CStatus
CGraphics_FillRectangle(CGraphics   *_this,
                        CBrush      *brush,
                        CRectangleF  rect);
CStatus
CGraphics_FillRectangles(CGraphics   *_this,
                         CBrush      *brush,
                         CRectangleF *rects,
                         CUInt32      count);
CStatus
CGraphics_FillRegion(CGraphics *_this,
                     CBrush    *brush,
                     CRegion   *region);
CStatus
CGraphics_BeginContainer(CGraphics          *_this,
                         CGraphicsContainer *container);
CStatus
CGraphics_BeginContainer2(CGraphics          *_this,
                          CRectangleF         dst,
                          CRectangleF         src,
                          CGraphicsUnit       unit,
                          CGraphicsContainer *container);
CStatus
CGraphics_EndContainer(CGraphics          *_this,
                       CGraphicsContainer  container);
CStatus
CGraphics_Restore(CGraphics *_this,
                  CUInt32    state);
CStatus
CGraphics_Save(CGraphics *_this,
               CUInt32   *state);
CStatus
CGraphics_DrawString(CGraphics       *_this,
                     CBrush          *brush,
                     _Cconst CChar16 *s,
                     CUInt32          length,
                     CFont           *font,
                     CRectangleF      layoutRect,
                     CStringFormat   *format);
CStatus
CGraphics_MeasureCharacterRanges(CGraphics        *_this,
                                 _Cconst CChar16  *s,
                                 CUInt32           length,
                                 CFont            *font,
                                 CRectangleF       layoutRect,
                                 CStringFormat    *format,
                                 CRegion         **regions,
                                 CUInt32          *count);
CStatus
CGraphics_MeasureString(CGraphics       *_this,
                        _Cconst CChar16 *s,
                        CUInt32          length,
                        CFont           *font,
                        CRectangleF      layoutRect,
                        CStringFormat   *format,
                        CUInt32         *charactersFitted,
                        CUInt32         *linesFilled,
                        CSizeF          *size);
CStatus
CGraphics_MeasureStringSimple(CGraphics       *_this,
                              _Cconst CChar16 *s,
                              CUInt32          length,
                              CFont           *font,
                              CSizeF          *size);
CStatus
CGraphics_DrawStringSimple(CGraphics       *_this,
                           CBrush          *brush,
                           _Cconst CChar16 *s,
                           CUInt32          length,
                           CFont           *font,
                           CRectangleF      layoutRect);
CStatus
CGraphics_Flush(CGraphics       *_this,
                CFlushIntention  intention);
CStatus
CGraphics_GetHdc(CGraphics  *_this,
                 void      **hdc);
CStatus
CGraphics_GetNearestColor(CGraphics *_this,
                          CColor     color,
                          CColor    *nearest);
CStatus
CGraphics_ReleaseHdc(CGraphics *_this,
                     void      *hdc);
CStatus
CGraphics_TransformPoints(CGraphics        *_this,
                          CCoordinateSpace  dst,
                          CCoordinateSpace  src,
                          CPointF          *points,
                          CUInt32           count);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_HATCHBRUSH_METHODS
#ifdef CRAYONS_HATCHBRUSH_METHODS
/* Declare public hatch brush methods. */
CStatus
CHatchBrush_Create(CHatchBrush **_this,
                   CHatchStyle   style,
                   CColor        foreground,
                   CColor        background);
CStatus
CHatchBrush_GetBackgroundColor(CHatchBrush *_this,
                               CColor      *background);
CStatus
CHatchBrush_GetForegroundColor(CHatchBrush *_this,
                               CColor      *foreground);
CStatus
CHatchBrush_GetHatchStyle(CHatchBrush *_this,
                          CHatchStyle *style);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_IMAGE_METHODS
#ifdef CRAYONS_IMAGE_METHODS
/* Declare public image methods. */
CStatus
CImage_Destroy(CImage **_this);
CStatus
CImage_GetFlags(CImage     *_this,
                CImageFlag *flags);
CStatus
CImage_GetHeight(CImage  *_this,
                 CUInt32 *height);
CStatus
CImage_GetHorizontalResolution(CImage *_this,
                               CFloat *dpiX);
CStatus
CImage_GetImageType(CImage     *_this,
                    CImageType *type);
CStatus
CImage_GetPhysicalDimension(CImage *_this,
                            CSizeF *size);
CStatus
CImage_GetPixelFormat(CImage       *_this,
                      CPixelFormat *pixelFormat);
CStatus
CImage_GetRawFormat(CImage *_this,
                    CGuid  *format);
CStatus
CImage_GetVerticalResolution(CImage *_this,
                             CFloat *dpiY);
CStatus
CImage_GetWidth(CImage  *_this,
                CUInt32 *width);
CStatus
CImage_Clone(CImage  *_this,
             CImage **clone);
CStatus
CImage_GetBounds(CImage        *_this,
                 CGraphicsUnit  pageUnit,
                 CRectangleF   *bounds);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_LINEBRUSH_METHODS
#ifdef CRAYONS_LINEBRUSH_METHODS
/* Declare public line brush methods. */
CStatus
CLineBrush_Create(CLineBrush  **_this,
                  CRectangleF   rectangle,
                  CColor        startColor,
                  CColor        endColor,
                  CFloat        angle,
                  CBool         isAngleScalable,
                  CWrapMode     wrapMode);
CStatus
CLineBrush_GetBlend(CLineBrush *_this,
                    CBlend     *blend);
CStatus
CLineBrush_SetBlend(CLineBrush *_this,
                    CBlend      blend);
CStatus
CLineBrush_GetColors(CLineBrush *_this,
                     CColor     *startColor,
                     CColor     *endColor);
CStatus
CLineBrush_SetColor(CLineBrush *_this,
                    CColor      startColor,
                    CColor      endColor);
CStatus
CLineBrush_GetColorBlend(CLineBrush  *_this,
                         CColorBlend *colorBlend);
CStatus
CLineBrush_SetColorBlend(CLineBrush  *_this,
                         CColorBlend  colorBlend);
CStatus
CLineBrush_GetGammaCorrection(CLineBrush *_this,
                              CBool      *gammaCorrection);
CStatus
CLineBrush_SetGammaCorrection(CLineBrush *_this,
                              CBool       gammaCorrection);
CStatus
CLineBrush_GetRectangle(CLineBrush  *_this,
                        CRectangleF *rectangle);
CStatus
CLineBrush_GetWrapMode(CLineBrush *_this,
                       CWrapMode  *wrapMode);
CStatus
CLineBrush_SetWrapMode(CLineBrush *_this,
                       CWrapMode   wrapMode);
CStatus
CLineBrush_GetTransform(CLineBrush *_this,
                        CMatrix    *matrix);
CStatus
CLineBrush_MultiplyTransform(CLineBrush   *_this,
                             CMatrix      *matrix,
                             CMatrixOrder  order);
CStatus
CLineBrush_ResetTransform(CLineBrush *_this);
CStatus
CLineBrush_RotateTransform(CLineBrush   *_this,
                           CFloat        angle,
                           CMatrixOrder  order);
CStatus
CLineBrush_ScaleTransform(CLineBrush   *_this,
                          CFloat        sx,
                          CFloat        sy,
                          CMatrixOrder  order);
CStatus
CLineBrush_SetTriangularShape(CLineBrush *_this,
                              CFloat      focus,
                              CFloat      scale);
CStatus
CLineBrush_SetSigmaBellShape(CLineBrush *_this,
                             CFloat      focus,
                             CFloat      scale);
CStatus
CLineBrush_SetTransform(CLineBrush *_this,
                        CMatrix    *matrix);
CStatus
CLineBrush_TranslateTransform(CLineBrush   *_this,
                              CFloat        dx,
                              CFloat        dy,
                              CMatrixOrder  order);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_MATRIX_METHODS
#ifdef CRAYONS_MATRIX_METHODS
/* Declare public matrix methods. */
CStatus
CMatrix_Create(CMatrix **_this);
CStatus
CMatrix_CreateParallelogram(CMatrix     **_this,
                            CRectangleF   rect,
                            CPointF       tl,
                            CPointF       tr,
                            CPointF       bl);
CStatus
CMatrix_CreateElements(CMatrix **_this,
                       CFloat    m11,
                       CFloat    m12,
                       CFloat    m21,
                       CFloat    m22,
                       CFloat    dx,
                       CFloat    dy);
CStatus
CMatrix_Destroy(CMatrix **_this);
CStatus
CMatrix_GetDeterminant(CMatrix *_this,
                       CFloat  *determinant);
CStatus
CMatrix_GetInverse(CMatrix *_this,
                   CMatrix *inverse);
CStatus
CMatrix_Multiply(CMatrix      *_this,
                 CMatrix      *other,
                 CMatrixOrder  order);
CStatus
CMatrix_Equals(CMatrix *_this,
               CMatrix *other,
               CBool   *eq);
CStatus
CMatrix_NotEquals(CMatrix *_this,
                  CMatrix *other,
                  CBool   *ne);
CStatus
CMatrix_Rotate(CMatrix      *_this,
               CFloat        angle,
               CMatrixOrder  order);
CStatus
CMatrix_Scale(CMatrix      *_this,
              CFloat        scaleX,
              CFloat        scaleY,
              CMatrixOrder  order);
CStatus
CMatrix_Shear(CMatrix      *_this,
              CFloat        shearX,
              CFloat        shearY,
              CMatrixOrder  order);
CStatus
CMatrix_Translate(CMatrix      *_this,
                  CFloat        offsetX,
                  CFloat        offsetY,
                  CMatrixOrder  order);
CStatus
CMatrix_TransformPoints(CMatrix *_this,
                        CPointF *points,
                        CUInt32  count);
CStatus
CMatrix_TransformVectors(CMatrix *_this,
                         CPointF *points,
                         CUInt32  count);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_PATH_METHODS
#ifdef CRAYONS_PATH_METHODS
/* Declare public path methods. */
CStatus
CPath_Create(CPath **_this);
CStatus
CPath_Destroy(CPath **_this);
CStatus
CPath_GetFillMode(CPath     *_this,
                  CFillMode *fillMode);
CStatus
CPath_SetFillMode(CPath     *_this,
                  CFillMode  fillMode);
CStatus
CPath_GetPoints(CPath    *_this,
                CPointF **points,
                CUInt32  *count);
CStatus
CPath_GetTypes(CPath    *_this,
               CByte   **types,
               CUInt32  *count);
CStatus
CPath_GetPathData(CPath    *_this,
                  CPointF **points,
                  CByte   **types,
                  CUInt32  *count);
CStatus
CPath_SetPathData(CPath   *_this,
                  CPointF *points,
                  CByte   *types,
                  CUInt32  count);
CStatus
CPath_AddArc(CPath  *_this,
             CFloat  x,
             CFloat  y,
             CFloat  width,
             CFloat  height,
             CFloat  startAngle,
             CFloat  sweepAngle);
CStatus
CPath_AddBezier(CPath  *_this,
                CFloat  x1,
                CFloat  y1,
                CFloat  x2,
                CFloat  y2,
                CFloat  x3,
                CFloat  y3,
                CFloat  x4,
                CFloat  y4);
CStatus
CPath_AddBeziers(CPath   *_this,
                 CPointF *points,
                 CUInt32  count);
CStatus
CPath_AddClosedCardinalCurve(CPath   *_this,
                             CPointF *points,
                             CUInt32  count,
                             CFloat   tension);
CStatus
CPath_AddCardinalCurve(CPath   *_this,
                       CPointF *points,
                       CUInt32  count,
                       CUInt32  offset,
                       CUInt32  numberOfSegments,
                       CFloat   tension);
CStatus
CPath_AddEllipse(CPath  *_this,
                 CFloat  x,
                 CFloat  y,
                 CFloat  width,
                 CFloat  height);
CStatus
CPath_AddLine(CPath  *_this,
              CFloat  x1,
              CFloat  y1,
              CFloat  x2,
              CFloat  y2);
CStatus
CPath_AddLines(CPath   *_this,
               CPointF *points,
               CUInt32  count);
CStatus
CPath_AddPath(CPath *_this,
              CPath *path,
              CBool  connect);
CStatus
CPath_AddPie(CPath  *_this,
             CFloat  x,
             CFloat  y,
             CFloat  width,
             CFloat  height,
             CFloat  startAngle,
             CFloat  sweepAngle);
CStatus
CPath_AddPolygon(CPath   *_this,
                 CPointF *points,
                 CUInt32  count);
CStatus
CPath_AddRectangle(CPath  *_this,
                   CFloat  x,
                   CFloat  y,
                   CFloat  width,
                   CFloat  height);
CStatus
CPath_AddRectangles(CPath       *_this,
                    CRectangleF *rects,
                    CUInt32      count);
CStatus
CPath_AddString(CPath         *_this,
                CChar16       *s,
                CUInt32        length,
                CFontFamily   *family,
                CFontStyle     style,
                CFloat         emSize,
                CRectangleF    layoutRect,
                CStringFormat *format);
CStatus
CPath_ClearMarkers(CPath *_this);
CStatus
CPath_Clone(CPath  *_this,
            CPath **clone);
CStatus
CPath_CloseAllFigures(CPath *_this);
CStatus
CPath_CloseFigure(CPath *_this);
CStatus
CPath_Flatten(CPath   *_this,
              CMatrix *matrix,
              CFloat   flatness);
CStatus
CPath_GetCount(CPath   *_this,
               CUInt32 *count);
CStatus
CPath_GetBounds(CPath       *_this,
                CMatrix     *matrix,
                CPen        *pen,
                CRectangleF *bounds);
CStatus
CPath_GetLastPoint(CPath   *_this,
                   CPointF *point);
CStatus
CPath_IsOutlineVisible(CPath     *_this,
                       CFloat     x,
                       CFloat     y,
                       CPen      *pen,
                       CGraphics *graphics,
                       CBool     *visible);
CStatus
CPath_IsVisible(CPath     *_this,
                CFloat     x,
                CFloat     y,
                CGraphics *graphics,
                CBool     *visible);
CStatus
CPath_Reset(CPath *_this);
CStatus
CPath_Reverse(CPath *_this);
CStatus
CPath_SetMarker(CPath *_this);
CStatus
CPath_StartFigure(CPath *_this);
CStatus
CPath_Transform(CPath   *_this,
                CMatrix *matrix);
CStatus
CPath_Warp(CPath     *_this,
           CMatrix   *matrix,
           CPointF   *dstPoints,
           CUInt32    dstLength,
           CFloat     srcX,
           CFloat     srcY,
           CFloat     srcWidth,
           CFloat     srcHeight,
           CWarpMode  warpMode,
           CFloat     flatness);
CStatus
CPath_Widen(CPath   *_this,
            CPen    *pen,
            CMatrix *matrix,
            CFloat   flatness);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_PATHBRUSH_METHODS
#ifdef CRAYONS_PATHBRUSH_METHODS
/* Declare public path brush methods. */
CStatus
CPathBrush_Create(CPathBrush **_this,
                  CPath       *path);
CStatus
CPathBrush_GetBlend(CPathBrush *_this,
                    CBlend     *blend);
CStatus
CPathBrush_SetBlend(CPathBrush *_this,
                    CBlend      blend);
CStatus
CPathBrush_GetCenterColor(CPathBrush *_this,
                          CColor     *centerColor);
CStatus
CPathBrush_SetCenterColor(CPathBrush *_this,
                          CColor      centerColor);
CStatus
CPathBrush_GetCenterPoint(CPathBrush *_this,
                          CPointF    *centerPoint);
CStatus
CPathBrush_SetCenterPoint(CPathBrush *_this,
                          CPointF     centerPoint);
CStatus
CPathBrush_GetColorBlend(CPathBrush  *_this,
                         CColorBlend *colorBlend);
CStatus
CPathBrush_SetColorBlend(CPathBrush  *_this,
                         CColorBlend  colorBlend);
CStatus
CPathBrush_GetFocusPoint(CPathBrush *_this,
                         CPointF    *focusPoint);
CStatus
CPathBrush_SetFocusPoint(CPathBrush *_this,
                         CPointF     focusPoint);
CStatus
CPathBrush_GetRectangle(CPathBrush  *_this,
                        CRectangleF *rectangle);
CStatus
CPathBrush_GetSurroundColors(CPathBrush  *_this,
                             CColor     **surroundColors,
                             CUInt32     *count);
CStatus
CPathBrush_SetSurroundColors(CPathBrush *_this,
                             CColor     *surroundColors,
                             CUInt32     count);
CStatus
CPathBrush_GetWrapMode(CPathBrush *_this,
                       CWrapMode  *wrapMode);
CStatus
CPathBrush_SetWrapMode(CPathBrush *_this,
                       CWrapMode   wrapMode);
CStatus
CPathBrush_GetTransform(CPathBrush *_this,
                        CMatrix    *matrix);
CStatus
CPathBrush_MultiplyTransform(CPathBrush   *_this,
                             CMatrix      *matrix,
                             CMatrixOrder  order);
CStatus
CPathBrush_ResetTransform(CPathBrush *_this);
CStatus
CPathBrush_RotateTransform(CPathBrush   *_this,
                           CFloat        angle,
                           CMatrixOrder  order);
CStatus
CPathBrush_ScaleTransform(CPathBrush   *_this,
                          CFloat        sx,
                          CFloat        sy,
                          CMatrixOrder  order);
CStatus
CPathBrush_SetTriangularShape(CPathBrush *_this,
                              CFloat      focus,
                              CFloat      scale);
CStatus
CPathBrush_SetSigmaBellShape(CPathBrush *_this,
                             CFloat      focus,
                             CFloat      scale);
CStatus
CPathBrush_SetTransform(CPathBrush *_this,
                        CMatrix    *matrix);
CStatus
CPathBrush_TranslateTransform(CPathBrush   *_this,
                              CFloat        dx,
                              CFloat        dy,
                              CMatrixOrder  order);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_PEN_METHODS
#ifdef CRAYONS_PEN_METHODS
/* Declare public pen methods. */
CStatus
CPen_Create(CPen   **_this,
            CBrush  *brush,
            CFloat   width);
CStatus
CPen_Destroy(CPen **_this);
CStatus
CPen_GetAlignment(CPen          *_this,
                  CPenAlignment *alignment);
CStatus
CPen_SetAlignment(CPen          *_this,
                  CPenAlignment  alignment);
CStatus
CPen_GetBrush(CPen    *_this,
              CBrush **brush);
CStatus
CPen_SetBrush(CPen   *_this,
              CBrush *brush);
CStatus
CPen_SetCaps(CPen     *_this,
             CLineCap  startCap,
             CLineCap  endCap,
             CDashCap  dashCap);
CStatus
CPen_GetColor(CPen   *_this,
              CColor *color);
CStatus
CPen_GetCompoundArray(CPen     *_this,
                      CFloat  **compoundArray,
                      CUInt32  *count);
CStatus
CPen_SetCompoundArray(CPen           *_this,
                      _Cconst CFloat *compoundArray,
                      CUInt32         count);
CStatus
CPen_GetCustomEndCap(CPen            *_this,
                     CCustomLineCap **customEndCap);
CStatus
CPen_SetCustomEndCap(CPen           *_this,
                     CCustomLineCap *customEndCap);
CStatus
CPen_GetCustomStartCap(CPen            *_this,
                       CCustomLineCap **customStartCap);
CStatus
CPen_SetCustomStartCap(CPen           *_this,
                       CCustomLineCap *customStartCap);
CStatus
CPen_GetDashCap(CPen     *_this,
                CDashCap *dashCap);
CStatus
CPen_SetDashCap(CPen     *_this,
                CDashCap  dashCap);
CStatus
CPen_GetDashOffset(CPen   *_this,
                   CFloat *dashOffset);
CStatus
CPen_SetDashOffset(CPen   *_this,
                   CFloat  dashOffset);
CStatus
CPen_GetDashPattern(CPen     *_this,
                    CFloat  **dashPattern,
                    CUInt32  *count);
CStatus
CPen_SetDashPattern(CPen           *_this,
                    _Cconst CFloat *dashPattern,
                    CUInt32         count);
CStatus
CPen_GetDashStyle(CPen       *_this,
                  CDashStyle *dashStyle);
CStatus
CPen_SetDashStyle(CPen       *_this,
                  CDashStyle  dashStyle);
CStatus
CPen_GetEndCap(CPen     *_this,
               CLineCap *endCap);
CStatus
CPen_SetEndCap(CPen     *_this,
               CLineCap  endCap);
CStatus
CPen_GetLineJoin(CPen      *_this,
                 CLineJoin *lineJoin);
CStatus
CPen_SetLineJoin(CPen      *_this,
                 CLineJoin  lineJoin);
CStatus
CPen_GetMiterLimit(CPen   *_this,
                   CFloat *miterLimit);
CStatus
CPen_SetMiterLimit(CPen   *_this,
                   CFloat  miterLimit);
CStatus
CPen_GetPenType(CPen     *_this,
                CPenType *type);
CStatus
CPen_GetStartCap(CPen     *_this,
                 CLineCap *startCap);
CStatus
CPen_SetStartCap(CPen     *_this,
                 CLineCap  startCap);
CStatus
CPen_GetWidth(CPen   *_this,
              CFloat *width);
CStatus
CPen_SetWidth(CPen   *_this,
              CFloat  width);
CStatus
CPen_Clone(CPen  *_this,
           CPen **clone);
CStatus
CPen_GetTransform(CPen    *_this,
                  CMatrix *matrix);
CStatus
CPen_MultiplyTransform(CPen         *_this,
                       CMatrix      *matrix,
                       CMatrixOrder  order);
CStatus
CPen_ResetTransform(CPen *_this);
CStatus
CPen_RotateTransform(CPen         *_this,
                     CFloat        angle,
                     CMatrixOrder  order);
CStatus
CPen_ScaleTransform(CPen         *_this,
                    CFloat        sx,
                    CFloat        sy,
                    CMatrixOrder  order);
CStatus
CPen_SetTransform(CPen    *_this,
                  CMatrix *matrix);
CStatus
CPen_TranslateTransform(CPen         *_this,
                        CFloat        dx,
                        CFloat        dy,
                        CMatrixOrder  order);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_REGION_METHODS
#ifdef CRAYONS_REGION_METHODS
/* Declare public region methods. */
CStatus
CRegion_Create(CRegion **_this);
CStatus
CRegion_CreatePath(CRegion **_this,
                   CPath    *path);
CStatus
CRegion_CreateRectangle(CRegion     **_this,
                        CRectangleF   rectangle);
CStatus
CRegion_Destroy(CRegion **_this);
CStatus
CRegion_Clone(CRegion  *_this,
              CRegion **clone);
CStatus
CRegion_CombinePath(CRegion      *_this,
                    CPath        *path,
                    CCombineMode  combineMode);
CStatus
CRegion_CombineRectangle(CRegion      *_this,
                         CRectangleF   rectangle,
                         CCombineMode  combineMode);
CStatus
CRegion_CombineRegion(CRegion      *_this,
                      CRegion      *other,
                      CCombineMode  combineMode);
CStatus
CRegion_Equals(CRegion   *_this,
               CRegion   *other,
               CGraphics *graphics,
               CBool     *eq);
CStatus
CRegion_GetBounds(CRegion     *_this,
                  CGraphics   *graphics,
                  CRectangleF *bounds);
CStatus
CRegion_GetData(CRegion  *_this,
                CByte   **data,
                CUInt32  *count);
CStatus
CRegion_GetRegionScans(CRegion      *_this,
                       CMatrix      *matrix,
                       CRectangleF **scans,
                       CUInt32      *count);
CStatus
CRegion_IsEmpty(CRegion   *_this,
                CGraphics *graphics,
                CBool     *empty);
CStatus
CRegion_IsInfinite(CRegion   *_this,
                   CGraphics *graphics,
                   CBool     *infinite);
CStatus
CRegion_IsVisiblePoint(CRegion   *_this,
                       CGraphics *graphics,
                       CPointF    point,
                       CBool     *visible);
CStatus
CRegion_IsVisibleRectangle(CRegion     *_this,
                           CGraphics   *graphics,
                           CRectangleF  rectangle,
                           CBool       *visible);
CStatus
CRegion_MakeEmpty(CRegion *_this);
CStatus
CRegion_MakeInfinite(CRegion *_this);
CStatus
CRegion_Transform(CRegion *_this,
                  CMatrix *matrix);
CStatus
CRegion_Translate(CRegion *_this,
                  CFloat   dx,
                  CFloat   dy);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_SOLIDBRUSH_METHODS
#ifdef CRAYONS_SOLIDBRUSH_METHODS
/* Declare public solid brush methods. */
CStatus
CSolidBrush_Create(CSolidBrush **_this,
                   CColor        color);
CStatus
CSolidBrush_GetColor(CSolidBrush *_this,
                     CColor      *color);
CStatus
CSolidBrush_SetColor(CSolidBrush *_this,
                     CColor       color);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_STRINGFORMAT_METHODS
#ifdef CRAYONS_STRINGFORMAT_METHODS
/* Declare public string format methods. */
CStatus
CStringFormat_Create(CStringFormat      **_this,
                     CStringFormatFlag    flags,
                     CLanguageID          language);
CStatus
CStringFormat_Destroy(CStringFormat **_this);
CStatus
CStringFormat_CreateGenericDefault(CStringFormat **_this);
CStatus
CStringFormat_CreateGenericTypographic(CStringFormat **_this);
CStatus
CStringFormat_Clone(CStringFormat  *_this,
                    CStringFormat **clone);
CStatus
CStringFormat_GetAlignment(CStringFormat    *_this,
                           CStringAlignment *alignment);
CStatus
CStringFormat_SetAlignment(CStringFormat    *_this,
                           CStringAlignment  alignment);
CStatus
CStringFormat_GetCharacterRanges(CStringFormat    *_this,
                                 CCharacterRange **characterRanges,
                                 CUInt32          *count);
CStatus
CStringFormat_SetCharacterRanges(CStringFormat   *_this,
                                 CCharacterRange *characterRanges,
                                 CUInt32          count);
CStatus
CStringFormat_GetDigitSubstitution(CStringFormat    *_this,
                                   CDigitSubstitute *method,
                                   CLanguageID      *language);
CStatus
CStringFormat_SetDigitSubstitution(CStringFormat    *_this,
                                   CDigitSubstitute  method,
                                   CLanguageID       language);
CStatus
CStringFormat_GetFormatFlags(CStringFormat     *_this,
                             CStringFormatFlag *formatFlags);
CStatus
CStringFormat_SetFormatFlags(CStringFormat     *_this,
                             CStringFormatFlag  formatFlags);
CStatus
CStringFormat_GetHotkeyPrefix(CStringFormat *_this,
                              CHotkeyPrefix *hotkeyPrefix);
CStatus
CStringFormat_SetHotkeyPrefix(CStringFormat *_this,
                              CHotkeyPrefix  hotkeyPrefix);
CStatus
CStringFormat_GetLineAlignment(CStringFormat    *_this,
                               CStringAlignment *lineAlignment);
CStatus
CStringFormat_SetLineAlignment(CStringFormat    *_this,
                               CStringAlignment  lineAlignment);
CStatus
CStringFormat_GetTabStops(CStringFormat  *_this,
                          CFloat         *firstTabOffset,
                          CFloat        **tabStops,
                          CUInt32        *count);
CStatus
CStringFormat_SetTabStops(CStringFormat *_this,
                          CFloat         firstTabOffset,
                          CFloat        *tabStops,
                          CUInt32        count);
CStatus
CStringFormat_GetTrimming(CStringFormat   *_this,
                          CStringTrimming *trimming);
CStatus
CStringFormat_SetTrimming(CStringFormat   *_this,
                          CStringTrimming  trimming);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_SURFACE_METHODS
#ifdef CRAYONS_SURFACE_METHODS
/* Declare public surface methods. */
CStatus
CSurface_Reference(CSurface *_this);
CStatus
CSurface_Destroy(CSurface **_this);
CStatus
CSurface_GetBounds(CSurface *_this,
                   CUInt32  *x,
                   CUInt32  *y,
                   CUInt32  *width,
                   CUInt32  *height);
CStatus
CSurface_SetBounds(CSurface *_this,
                   CUInt32   x,
                   CUInt32   y,
                   CUInt32   width,
                   CUInt32   height);
#endif
/******************************************************************************/



/******************************************************************************/
#define CRAYONS_TEXTUREBRUSH_METHODS
#ifdef CRAYONS_TEXTUREBRUSH_METHODS
/* Declare public texture brush methods. */
CStatus
CTextureBrush_Create(CTextureBrush **_this,
                     CImage         *image,
                     CRectangleF     rectangle,
                     CWrapMode       wrapMode);
CStatus
CTextureBrush_GetImage(CTextureBrush  *_this,
                       CImage        **image);
CStatus
CTextureBrush_GetWrapMode(CTextureBrush *_this,
                          CWrapMode     *wrapMode);
CStatus
CTextureBrush_SetWrapMode(CTextureBrush *_this,
                          CWrapMode      wrapMode);
CStatus
CTextureBrush_GetTransform(CTextureBrush *_this,
                           CMatrix       *matrix);
CStatus
CTextureBrush_MultiplyTransform(CTextureBrush *_this,
                                CMatrix       *matrix,
                                CMatrixOrder   order);
CStatus
CTextureBrush_ResetTransform(CTextureBrush *_this);
CStatus
CTextureBrush_RotateTransform(CTextureBrush *_this,
                              CFloat         angle,
                              CMatrixOrder   order);
CStatus
CTextureBrush_ScaleTransform(CTextureBrush *_this,
                             CFloat         sx,
                             CFloat         sy,
                             CMatrixOrder   order);
CStatus
CTextureBrush_SetTransform(CTextureBrush *_this,
                           CMatrix       *matrix);
CStatus
CTextureBrush_TranslateTransform(CTextureBrush *_this,
                                 CFloat         dx,
                                 CFloat         dy,
                                 CMatrixOrder   order);
#endif
/******************************************************************************/

#ifdef __cplusplus
};
#endif

#endif /* _CRAYONS_H_ */
