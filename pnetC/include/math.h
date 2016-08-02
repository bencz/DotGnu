/*
 * math.h - Floating point math routines.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#ifndef _MATH_H
#define _MATH_H

#include <features.h>

__BEGIN_DECLS

/* The CLI does all floating point calculations in "long double" */
typedef long double float_t;
typedef long double double_t;

/* Helper macros for defining math functions */
#define	__MATHTEST(name)	\
    extern int name##f(float __value); \
    extern int name(double __value); \
    extern int name##l(long double __value)
#define	__MATHFUNC1(name)	\
    extern float name##f(float __value); \
    extern double name(double __value); \
    extern long double name##l(long double __value)
#define	__MATHFUNC2(name)	\
    extern float name##f(float __value1, float __value2); \
    extern double name(double __value1, double __value2); \
    extern long double name##l(long double __value1, long double __value2)
#define __MATHROUND(name,type)	\
    extern type name##f(float __value); \
    extern type name(double __value); \
    extern type name##l(long double __value)

/* Value testing macros and support functions */
__MATHTEST(__isfinite);
__MATHTEST(__isinf);
__MATHTEST(__isnan);
#define	isfinite(x) \
    (sizeof(x) == sizeof(float) ? __isfinitef((x)) : \
     (sizeof(x) == sizeof(double) ? __isfinite((x)) : __isfinitel((x))))
#define	isinf(x) \
    (sizeof(x) == sizeof(float) ? __isinff((x)) : \
     (sizeof(x) == sizeof(double) ? __isinf((x)) : __isinfl((x))))
#define	isnan(x) \
    (sizeof(x) == sizeof(float) ? __isnanf((x)) : \
     (sizeof(x) == sizeof(double) ? __isnan((x)) : __isnanl((x))))

/* Simple math function declarations */
__MATHFUNC1(acos);
__MATHFUNC1(acosh);
__MATHFUNC1(asin);
__MATHFUNC1(asinh);
__MATHFUNC1(atan);
__MATHFUNC1(atanh);
__MATHFUNC2(atan2);
__MATHFUNC1(cbrt);
__MATHFUNC1(ceil);
__MATHFUNC2(copysign);
__MATHFUNC1(cos);
__MATHFUNC1(cosh);
__MATHFUNC1(erf);
__MATHFUNC1(erfc);
__MATHFUNC1(exp);
__MATHFUNC1(exp2);
__MATHFUNC1(expm1);
__MATHFUNC1(fabs);
__MATHFUNC2(fdim);
__MATHFUNC1(floor);
__MATHFUNC2(fmax);
__MATHFUNC2(fmin);
__MATHFUNC2(fmod);
__MATHFUNC2(hypot);
__MATHFUNC1(fgamma);
__MATHROUND(llrint, long long);
__MATHROUND(llround, long long);
__MATHFUNC1(log);
__MATHFUNC1(log10);
__MATHFUNC1(log1p);
__MATHFUNC1(log2);
__MATHFUNC1(logb);
__MATHROUND(lrint, long);
__MATHROUND(lround, long);
__MATHFUNC1(nearbyint);
__MATHFUNC2(nextafter);
__MATHFUNC2(nexttoward);
__MATHFUNC2(pow);
__MATHFUNC2(remainder);
__MATHFUNC1(rint);
__MATHFUNC1(round);
__MATHFUNC1(sin);
__MATHFUNC1(sinh);
__MATHFUNC1(sqrt);
__MATHFUNC1(tan);
__MATHFUNC1(tanh);
__MATHFUNC1(tgamma);
__MATHFUNC1(trunc);

/* Other math function declarations */
extern float fmaf(float __value1, float __value2, float __value3);
extern double fma(double __value1, double __value2, double __value3);
extern long double fmal(long double __value1, long double __value2,
                        long double __value3);
extern float frexpf(float __value1, int *__value2);
extern double frexp(double __value1, int *__value2);
extern long double frexpl(long double __value1, int *__value2);
extern int ilogbf(float __value);
extern int ilogb(double __value);
extern int ilogbl(long double __value);
extern double j0(double __value);
extern double j1(double __value);
extern double jn(int __value1, double __value2);
extern float ldexpf(float __value1, int __value2);
extern double ldexp(double __value1, int __value2);
extern long double ldexpl(long double __value1, int __value2);
extern float modff(float __value1, float *__value2);
extern double modf(double __value1, double *__value2);
extern long double modfl(long double __value1, long double *__value2);
extern float nanf(const char *__value);
extern double nan(const char *__value);
extern long double nanl(const char *__value);
extern float remquof(float __value1, float __value2, int *__value3);
extern double remquo(double __value1, double __value2, int *__value3);
extern long double remquol(long double __value1, long double __value2,
                           int *__value3);
extern double scalb(double __value1, double __value2);
extern float scalblnf(float __value1, long __value2);
extern double scalbln(double __value1, long __value2);
extern long double scalblnl(long double __value1, long __value2);
extern float scalbnf(float __value1, int __value2);
extern double scalbn(double __value1, int __value2);
extern long double scalbnl(long double __value1, int __value2);
extern double y0(double __value);
extern double y1(double __value);
extern double yn(int __value1, double __value2);
extern int signgam;

/* Clean up the macro namespace */
#undef __MATHTEST
#undef __MATHFUNC1
#undef __MATHFUNC2
#undef __MATHROUND

__END_DECLS

#endif  /* !_MATH_H */
