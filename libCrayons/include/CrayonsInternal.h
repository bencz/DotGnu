/*
 * CLibInternal.h - Internal library header.
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

#ifndef _CRAYONS_INTERNAL_H_
#define _CRAYONS_INTERNAL_H_

#include "Crayons.h"
#include "CrayonsConfig.h"

#include <stdio.h>

#if STDC_HEADERS
	#include <stdlib.h>
	#include <stddef.h>
#elif HAVE_STDLIB_H
	#include <stdlib.h>
#endif

#if HAVE_STRING_H
	#if !STDC_HEADERS && HAVE_MEMORY_H
		#include <memory.h>
	#endif
	#include <string.h>
#endif

#if HAVE_STRINGS_H
	#include <strings.h>
#endif

#if HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif

#if HAVE_SYS_STAT_H
	#include <sys/stat.h>
#endif

#if HAVE_UNISTD_H
	#include <unistd.h>
#endif

#if HAVE_ASSERT_H && CDEBUG
	#include <assert.h>
#else
	#undef CDEBUG
#endif

#include <pixman.h>

#ifdef __cplusplus
extern "C" {
#endif

/* Define function attributes. */
#if (__GNUC__ > 3 || (__GNUC__ == 3 && __GNUC_MINOR__ >= 3)) && defined(__ELF__)
	#ifndef CTESTING
		#define CINTERNAL __attribute__((__visibility__("hidden")))
	#else
		#define CINTERNAL
	#endif
#else
	#define CINTERNAL
#endif
#if (__GNUC__ >= 3 || (__GNUC__ == 2 && __GNUC_MINOR__ >= 5))
	#define CMATH __attribute__((__const__))
#else
	#define CMATH
#endif

/* Define debugging macros. */
#ifdef CDEBUG
	#define CASSERT(foo) assert(foo)
#else
	#define CASSERT(foo)
#endif

/* Define system type. */
#if defined(__CYGWIN__) || defined(__CYGWIN32__)
	#define	C_SYSTEM_WIN32_CYGWIN 1
	#define	C_SYSTEM_WIN32        1
#elif defined(_WIN32) || defined(WIN32)
	#define	C_SYSTEM_WIN32_NATIVE 1
	#define	C_SYSTEM_WIN32        1
#endif

typedef unsigned int CBitField;
typedef CUInt32      CFixedU;
typedef CInt32       CFixed;

typedef struct _tagCPointX CPointX;
struct _tagCPointX
{
	CFixed x;
	CFixed y;
};

typedef struct _tagCLineX CLineX;
struct _tagCLineX
{
	CPointX point1;
	CPointX point2;
};

typedef struct _tagCEdgeX CEdgeX;
struct _tagCEdgeX
{
	CLineX line;
	CBool  clockwise;
	CFixed currentX;
};

typedef struct _tagCTrapezoidX CTrapezoidX;
struct _tagCTrapezoidX
{
	CFixed top;
	CFixed bottom;
	CLineX left;
	CLineX right;
};

typedef struct _tagCTrapezoids CTrapezoids;
struct _tagCTrapezoids
{
	CUInt32      count;
	CUInt32      capacity;
	CTrapezoidX *trapezoids;
};

typedef struct _tagCAffineTransformF CAffineTransformF;
struct _tagCAffineTransformF
{
	CFloat m11;
	CFloat m12;
	CFloat m21;
	CFloat m22;
	CFloat dx;
	CFloat dy;
};

typedef struct _tagCPattern CPattern;
struct _tagCPattern
{
	pixman_image_t     *image;
	CAffineTransformF  *transform;
};

typedef struct _tagCPointArrayX CPointArrayX;
struct _tagCPointArrayX
{
	CUInt32  capacity;
	CUInt32  count;
	CPointX *points;
};

typedef struct _tagCPointArrayF CPointArrayF;
struct _tagCPointArrayF
{
	CUInt32  capacity;
	CUInt32  count;
	CPointF *points;
};

typedef struct _tagCBezierX CBezierX;
struct _tagCBezierX
{
	CPointX a;
	CPointX b;
	CPointX c;
	CPointX d;
};

typedef struct _tagCBezierF CBezierF;
struct _tagCBezierF
{
	CPointF a;
	CPointF b;
	CPointF c;
	CPointF d;
};

typedef struct _tagCFiller   CFiller;
typedef struct _tagCPolygonX CPolygonX;
typedef struct _tagCStroker  CStroker;
typedef CPointF              CVectorF;



typedef struct _tagCPredicateBinary CPredicateBinary;
struct _tagCPredicateBinary
{
	CBool (*Predicate)(CPredicateBinary *_this,
	                   void             *a,
	                   void             *b);
};
typedef struct _tagCPredicateUnary CPredicateUnary;
struct _tagCPredicateUnary
{
	CBool (*Predicate)(CPredicateUnary *_this,
	                   void            *a);
};
typedef struct _tagCOperatorBinary COperatorBinary;
struct _tagCOperatorBinary
{
	void (*Operator)(COperatorBinary *_this,
	                 void           *a,
	                 void           *b);
};
typedef struct _tagCOperatorUnary COperatorUnary;
struct _tagCOperatorUnary
{
	void (*Operator)(COperatorUnary *_this,
	                 void           *a);
};

#define CPredicate_Binary(_this, a, b) ((_this)->Predicate((_this), (a), (b)))
#define CPredicate_Unary(_this, a)     ((_this)->Predicate((_this), (a)))
#define COperator_Binary(_this, a, b)  ((_this)->Operator((_this),  (a), (b)))
#define COperator_Unary(_this, a)      ((_this)->Operator((_this),  (a)))







CINTERNAL void *
CMalloc(CUInt32 size);
CINTERNAL void *
CRealloc(void     *ptr,
         CUInt32  size);
CINTERNAL void *
CCalloc(CUInt32 count,
        CUInt32 size);
CINTERNAL void
CFree(void *ptr);
CINTERNAL void *
CMemSet(void     *dst,
        CByte     value,
        CUInt32  length);
CINTERNAL void *
CMemCopy(void       *dst,
         const void *src,
         CUInt32    length);
CINTERNAL void *
CMemMove(void       *dst,
         const void *src,
         CUInt32    length);
CINTERNAL int
CMemCmp(const void *a,
        const void *b,
        CUInt32    length);








#define CCLAMP(value, min, max)                                                \
	do {                                                                       \
		if((value) < (min))                                                    \
		{                                                                      \
			(value) = (min);                                                   \
		}                                                                      \
		else if((value) > (max))                                               \
		{                                                                      \
			(value) = (max);                                                   \
		}                                                                      \
	} while(0)

#define CMath_PI                           (3.14159265358979323846)
#define CMath_ToRadians(angle)             (((angle) * CMath_PI) / 180)
#define CMath_DotProduct(x1, y1, x2, y2)   (((x1) * (x2)) + ((y1) * (y2)))
#define CMath_CrossProduct(x1, y1, x2, y2) (((x2) * (y1)) - ((x1) * (y2)))
#define CMath_Abs(x) ((x) < 0 ? -(x) : (x))

/*\
|*| When appromixating arcs less-than-or-equal-to 90 degrees, the control
|*| points can be calculated thusly:
|*|
|*|  i - the intersection of the tangents of the end points of the arc
|*|  r - the radius of the arc
|*|  d - the distance from an end point to 'i'
|*|  f - the distance from an end point to 'i', along its tangent,
|*|      where the control point for that end point lies
|*|
|*|  m = (d / r)^2
|*|  n = sqrt(1 + m)
|*|  f = 4 / (3 + 3n)
|*|
|*|
|*| For 90 degree arcs, 'd' equals 'r', therefore:
|*|
|*|  m = 1
|*|  n = sqrt(2)
|*|  f = 4 / (3 + 3*sqrt(2)) ~= 0.552284749830793
|*|
\*/
#define CMath_Arc90Fraction 0.552284749830793

#define CFloat_ToFixed(f)  ((CFixed) (((CFloat) (f)) * 65536))
#define CDouble_ToFixed(f) ((CFixed) (((CDouble)(f)) * 65536))
#define CFixed_ToFloat(f)  ((CFloat) (((CFloat) (f)) / 65536))
#define CFixed_ToDouble(f) ((CDouble)(((CDouble)(f)) / 65536))

#define CFixed_Floor(f)    ((CFixed)(((f) +     0)  & ~65535))
#define CFixed_Ceil(f)     ((CFixed)(((f) + 65535)  & ~65535))
#define CFixed_Round(f)    ((CFixed)(((f) + 32768)  & ~65535))
#define CFixed_Trunc(f)    ((CInt32)((f) >> 16))

#define CFixed_Zero        ((CFixed)0)
#define CFixed_One         ((CFixed)65536)
#define CFixed_MinusOne    ((CFixed)-65536)

#define CColor_FromARGB(a, r, g, b)                                            \
	((((a) << 24) & 0xFF000000) |                                              \
	 (((r) << 16) & 0x00FF0000) |                                              \
     (((g) <<  8) & 0x0000FF00) |                                              \
     (((b) <<  0) & 0x000000FF))
#define CColor_IntensityR 0.30
#define CColor_IntensityG 0.59
#define CColor_IntensityB 0.11
#define CColor_IntensityRGB(r, g, b)                                           \
	(((r) * CColor_IntensityR) +                                               \
	 ((g) * CColor_IntensityG) +                                               \
	 ((b) * CColor_IntensityB))
#define CColor_A(color) ((CByte)((color) >> 24))
#define CColor_R(color) ((CByte)((color) >> 16))
#define CColor_G(color) ((CByte)((color) >>  8))
#define CColor_B(color) ((CByte)((color) >>  0))
#define CColor_Black    (0xFF000000)
#define CColor_White    (0xFFFFFFFF)
#define CColor_Empty    (0x00000000)


/* TODO: use configure-time tests to generate this properly */
#define CPixmanPixel_FromARGB(a, r, g, b)                                      \
	(((CColor)((a) << 24)) |                                                   \
	 ((CColor)((r) << 16)) |                                                   \
	 ((CColor)((g) <<  8)) |                                                   \
	 ((CColor)((b) <<  0)))

/* TODO: use configure-time tests to generate this properly */
#define CPixmanPixel_ToARGB(pixel, a, r, g, b)                                 \
	do {                                                                       \
		(a) = ((CByte)((pixel) >> 24));                                        \
		(r) = ((CByte)((pixel) >> 16));                                        \
		(g) = ((CByte)((pixel) >>  8));                                        \
		(b) = ((CByte)((pixel) >>  0));                                        \
	} while(0)

#define CCombineMode_Default(combineMode)                                      \
	do {                                                                       \
		if((combineMode) > CCombineMode_Complement)                            \
		{                                                                      \
			(combineMode) = CCombineMode_Replace;                              \
		}                                                                      \
	} while(0)

#define CFontFamilyGeneric_Default(generic)                                    \
	do {                                                                       \
		if((generic) > CFontFamilyGeneric_Monospace)                           \
		{                                                                      \
			(generic) = CFontFamilyGeneric_Monospace;                          \
		}                                                                      \
	} while(0)

#define CPixelFormat_IsGdi(pixelFormat) \
	(((pixelFormat) & CPixelFormat_Gdi) != 0)
#define CPixelFormat_IsExtended(pixelFormat) \
	(((pixelFormat) & CPixelFormat_Extended) != 0)
#define CPixelFormat_IsCanonical(pixelFormat) \
	(((pixelFormat) & CPixelFormat_Canonical) != 0)
#define CPixelFormat_IsIndexed(pixelFormat) \
	(((pixelFormat) & CPixelFormat_Indexed) != 0)
#define CPixelFormat_HasAlpha(pixelFormat) \
	(((pixelFormat) & CPixelFormat_Alpha) != 0)
#define CPixelFormat_HasPAlpha(pixelFormat) \
	(((pixelFormat) & CPixelFormat_PAlpha) != 0)
#define CPixelFormat_BitsPerPixel(pixelFormat) \
	(((pixelFormat) >> 8) & 0xFF)
#define CPixelFormat_PaletteSize(pixelFormat) \
	(2 << (CPixelFormat_BitsPerPixel((pixelFormat))))

#define CRectangle_ContainsPoint(r, p) \
	((CPoint_X(p) >= (CRectangle_X(r))) && \
	 (CPoint_X(p) <= (CRectangle_X(r) + CRectangle_Width(r))) && \
	 (CPoint_Y(p) >= (CRectangle_Y(r))) && \
	 (CPoint_Y(p) <= (CRectangle_Y(v) + CRectangle_Height(r))))

#define CAffineTransform_XX(transform) ((transform).m11)
#define CAffineTransform_XY(transform) ((transform).m12)
#define CAffineTransform_YX(transform) ((transform).m21)
#define CAffineTransform_YY(transform) ((transform).m22)
#define CAffineTransform_DX(transform) ((transform).dx)
#define CAffineTransform_DY(transform) ((transform).dy)


#define CPoint_X(point)              ((point).x)
#define CPoint_Y(point)              ((point).y)
#define CLine_Point1(line)           ((line).point1)
#define CLine_Point2(line)           ((line).point2)
#define CLine_X1(line)               (CPoint_X(CLine_Point1(line)))
#define CLine_Y1(line)               (CPoint_Y(CLine_Point1(line)))
#define CLine_X2(line)               (CPoint_X(CLine_Point2(line)))
#define CLine_Y2(line)               (CPoint_Y(CLine_Point2(line)))
#define CEdge_Line(edge)             ((edge).line)
#define CEdge_Clockwise(edge)        ((edge).clockwise)
#define CEdge_CurrentX(edge)         ((edge).currentX)
#define CEdge_Point1(edge)           (CLine_Point1(CEdge_Line(edge)))
#define CEdge_Point2(edge)           (CLine_Point2(CEdge_Line(edge)))
#define CEdge_X1(edge)               (CLine_X1(CEdge_Line(edge)))
#define CEdge_Y1(edge)               (CLine_Y1(CEdge_Line(edge)))
#define CEdge_X2(edge)               (CLine_X2(CEdge_Line(edge)))
#define CEdge_Y2(edge)               (CLine_Y2(CEdge_Line(edge)))
#define CVector_X(vector)            (CPoint_X(vector))
#define CVector_Y(vector)            (CPoint_Y(vector))
#define CRectangle_X(rectangle)      ((rectangle).x)
#define CRectangle_Y(rectangle)      ((rectangle).y)
#define CRectangle_Width(rectangle)  ((rectangle).width)
#define CRectangle_Height(rectangle) ((rectangle).height)
#define CSize_Width(size)            ((size).width)
#define CSize_Height(size)           ((size).height)
#define CTrapezoid_Top(trapezoid)    ((trapezoid).top)
#define CTrapezoid_Bottom(trapezoid) ((trapezoid).bottom)
#define CTrapezoid_Left(trapezoid)   ((trapezoid).left)
#define CTrapezoid_Right(trapezoid)  ((trapezoid).right)

#define CPointArray_Count(array)    ((array).count)
#define CPointArray_Points(array)   ((array).points)
#define CPointArray_Point(array, i) ((array).points[(i)])

#define CTrapezoids_Count(t)      ((t).count)
#define CTrapezoids_Trapezoids(t) ((t).trapezoids)

#define CGraphics_DefaultDpi 96.0f

#define CFiller_TOLERANCE 0.1f

#define CStatus_CheckGOTO(status, save, target)                                \
	do {                                                                       \
		if(((save) = (status)) != CStatus_OK) { goto target; }                 \
	} while(0)
#define CStatus_Check(status)                                                  \
	do {                                                                       \
		const CStatus _status_ = (status);                                     \
		if(_status_ != CStatus_OK) { return _status_; }                        \
	} while(0)
#define CStatus_Require(cond, status)                                          \
	do {                                                                       \
		if(!(cond)) { return (status); }                                       \
	} while(0)

#ifdef __cplusplus
};
#endif

#endif /* _CRAYONS_INTERNAL_H_ */
