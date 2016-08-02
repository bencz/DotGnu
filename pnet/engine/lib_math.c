/*
 * lib_math.c - Internalcall methods for "System.Math", "Single", and "Double".
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

#include "engine.h"
#include "lib_defs.h"
#ifdef HAVE_MATH_H
#include <math.h>
#endif
#ifdef IL_WIN32_NATIVE
#include <float.h>
#else
#ifdef HAVE_IEEEFP_H
#include <ieeefp.h>
#endif
#endif

#ifdef IL_CONFIG_FP_SUPPORTED

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Define the following macro to test compiling without "libm" support.
 */
/*#define	IL_NO_LIBM*/

/*
 * Turn off "libm" if requested.
 */
#ifdef 	IL_NO_LIBM
#undef	HAVE_FINITE
#undef	HAVE_ISNAN
#undef	HAVE_ISINF
#undef	HAVE_CEIL
#undef	HAVE_FLOOR
#undef	HAVE_REMAINDER
#undef	HAVE_ACOS
#undef	HAVE_ASIN
#undef	HAVE_ATAN
#undef	HAVE_ATAN2
#undef	HAVE_COS
#undef	HAVE_COSH
#undef	HAVE_EXP
#undef	HAVE_LOG
#undef	HAVE_LOG10
#undef	HAVE_POW
#undef	HAVE_SIN
#undef	HAVE_SINH
#undef	HAVE_SQRT
#undef	HAVE_TAN
#undef	HAVE_TANH
#endif

/*
 * Useful constant that represents "not a number".
 */
#define	NOT_A_NUMBER	((ILDouble)(0.0 / 0.0))

/*
 * Largest integer that can be represented in an IEEE double / 2.
 */
#define	LARGEST_INT		4503599627370496.0

/*
 * Test for "not a number".
 */
#define	Math_IsNaN(x)			(ILNativeFloatIsNaN((x)))

/*
 * Test for infinities.
 */
#define	Math_IsInf(x)			(ILNativeFloatIsInf((x)))

/*
 * Test for finite values.
 */
#define	Math_Finite(x)			(ILNativeFloatIsFinite((x)))

/*
 * Get the floor or ceiling of a value.
 */
#ifdef HAVE_FLOOR
	#define	Math_Floor(x)	(floor((x)))
#else
	static double Math_Floor(double x);
#endif
#ifdef HAVE_FLOOR
	#define	Math_Ceil(x)	(ceil((x)))
#else
	static double Math_Ceil(double x);
#endif

/*
 * Implement the floor helper function.
 */
#ifndef HAVE_FLOOR
static double Math_Floor(double x)
{
	double temp;
	if(!Math_Finite(x))
	{
		return x;
	}
	else if(x < 0.0)
	{
		temp = -(Math_Floor(-x));
		if(temp != x)
		{
			return temp - 1.0;
		}
		else
		{
			return x;
		}
	}
	else
	{
		/* Note: "y" must be volatile so that the compiler
		   does not optimise the operations away */
		volatile double y = LARGEST_INT + x;
		y -= x;
		if(x < y)
		{
			return y - 1.0;
		}
		else
		{
			return y;
		}
	}
}
#endif /* !HAVE_FLOOR */

/*
 * Implement the ceiling helper function.
 */
#ifndef HAVE_CEIL
static double Math_Ceil(double x)
{
	if(!Math_Finite(x))
	{
		return x;
	}
	else if(x < 0.0)
	{
		return -(Math_Floor(-x));
	}
	else
	{
		double temp = Math_Floor(x);
		if(temp != x)
		{
			return temp + 1.0;
		}
		else
		{
			return x;
		}
	}
}
#endif /* !HAVE_CEIL */

/*
 * Compute the IEEE remainder of two double values.
 */
#ifdef HAVE_REMAINDER
#define	Math_Remainder(x,y)		(remainder((x), (y)))
#else
static double Math_Remainder(double x, double y)
{
	if(Math_IsNaN(x) || Math_IsNaN(y) || y == 0.0)
	{
		return NOT_A_NUMBER;
	}
	else
	{
		double quotient = x / y;
		if(quotient >= 0.0)
		{
			return (x - Math_Ceil(quotient) * y);
		}
		else
		{
			return (x - Math_Floor(quotient) * y);
		}
	}
}
#endif

/*
 * Round a "double" value to the nearest integer, using the
 * "round half even" rounding mode.
 */
#ifdef HAVE_RINT
	#define Math_Round(x)	(rint((x)))
#else
static double Math_Round(double x)
{
	double above, below;
	if(!Math_Finite(x))
	{
		return x;
	}
	above = Math_Ceil(x);
	below = Math_Floor(x);
	if((above - x) < 0.5)
	{
		return above;
	}
	else if((x - below) < 0.5)
	{
		return below;
	}
	else if(Math_Remainder(above, 2.0) == 0.0)
	{
		return above;
	}
	else
	{
		return below;
	}
}
#endif

/*
 * Round a "double" value to the nearest integer, using the
 * "round away from zero" rounding mode.
 */
#ifdef HAVE_ROUND
	#define Math_RoundAwayFromZero(x)	round((x))
#else
static double Math_RoundAwayFromZero(double x)
{
	double above, below;
	if(!Math_Finite(x))
	{
		return x;
	}
	above = Math_Ceil(x);
	below = Math_Floor(x);
	if(x > 0)
	{
		if((x - below) < 0.5)
		{
			return below;
		}
		return above;
	}
	else
	{
		if((above - x) < 0.5)
		{
			return above;
		}
		return below;
	}
}
#endif

/*
 * Round a "double" value towards zero.
 */
#ifdef HAVE_TRUNC
	#define Math_Trunc(x)	trunc((x))
#else
static double Math_Trunc(double x)
{
	if(!Math_Finite(x))
	{
		return x;
	}
	if(x > 0.0)
	{
		return Math_Floor(x);
	}
	else
	{
		return Math_Ceil(x);
	}
}
#endif

/*
 * public static double Acos(double d);
 */
ILDouble _IL_Math_Acos(ILExecThread *thread, ILDouble d)
{
#ifdef HAVE_ACOS
	return (ILDouble)(acos((double)d));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Asin(double d);
 */
ILDouble _IL_Math_Asin(ILExecThread *thread, ILDouble d)
{
#ifdef HAVE_ASIN
	return (ILDouble)(asin((double)d));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Atan(double d);
 */
ILDouble _IL_Math_Atan(ILExecThread *thread, ILDouble d)
{
#ifdef HAVE_ATAN
	return (ILDouble)(atan((double)d));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Atan2(double y, double x);
 */
ILDouble _IL_Math_Atan2(ILExecThread *thread, ILDouble y, ILDouble x)
{
#ifdef HAVE_ATAN2
	return (ILDouble)(atan2((double)y, (double)x));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Ceiling(double a);
 */
ILDouble _IL_Math_Ceiling(ILExecThread *thread, ILDouble a)
{
	return (ILDouble)(Math_Ceil((double)a));
}

/*
 * public static double Cos(double d);
 */
ILDouble _IL_Math_Cos(ILExecThread *thread, ILDouble d)
{
#ifdef HAVE_COS
	return (ILDouble)(cos((double)d));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Cosh(double value);
 */
ILDouble _IL_Math_Cosh(ILExecThread *thread, ILDouble value)
{
#ifdef HAVE_COSH
	return (ILDouble)(cosh((double)value));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Exp(double d);
 */
ILDouble _IL_Math_Exp(ILExecThread *thread, ILDouble d)
{
#ifdef HAVE_EXP
	return (ILDouble)(exp((double)d));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Floor(double d);
 */
ILDouble _IL_Math_Floor(ILExecThread *thread, ILDouble d)
{
	return (ILDouble)(Math_Floor((double)d));
}

/*
 * public static double IEEERemainder(double x, double y);
 */
ILDouble _IL_Math_IEEERemainder(ILExecThread *thread, ILDouble x, ILDouble y)
{
	return (ILDouble)(Math_Remainder((double)x, (double)y));
}

/*
 * public static double Log(double d);
 */
ILDouble _IL_Math_Log(ILExecThread *thread, ILDouble d)
{
#ifdef HAVE_LOG
	return (ILDouble)(log((double)d));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Log10(double d);
 */
ILDouble _IL_Math_Log10(ILExecThread *thread, ILDouble d)
{
#ifdef HAVE_LOG10
	return (ILDouble)(log10((double)d));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Pow(double x, double y);
 */
ILDouble _IL_Math_Pow(ILExecThread *thread, ILDouble x, ILDouble y)
{
#ifdef HAVE_POW
	return (ILDouble)(pow((double)x, (double)y));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Round(double a);
 */
ILDouble _IL_Math_Round(ILExecThread *thread, ILDouble a)
{
	return (ILDouble)Math_Round((double)a);
}

/*
 * private static double RoundDouble(double value, int digits);
 */
ILDouble _IL_Math_RoundDouble(ILExecThread *thread, ILDouble value,
							  ILInt32 digits)
{
	double rounded;
	double power;
	rounded = Math_Round((double)value);
	if(digits == 0 || rounded == (double)value)
	{
		/* Simple rounding, or the value is already an integer */
		return rounded;
	}
	else
	{
	#ifdef HAVE_POW
		power = pow(10, (double)digits);
	#else
		power = 1.0;
		while(digits > 0)
		{
			power *= 10.0;
			--digits;
		}
	#endif
		return (Math_Round(((double)value) * power) / power);
	}
}

/*
 * private static double RoundDoubleToNearest(double value);
 */
ILDouble _IL_Math_RoundDoubleAwayFromZero_d(ILExecThread *thread,
											ILDouble value)
{
	return (ILDouble)Math_RoundAwayFromZero(value);
}

/*
 * private static double RoundDoubleToNearest(double value, int digits);
 */
ILDouble _IL_Math_RoundDoubleAwayFromZero_di(ILExecThread *thread,
											 ILDouble value,
											 ILInt32 digits)
{
	double rounded;
	double power;
	rounded = Math_RoundAwayFromZero((double)value);
	if(digits == 0 || rounded == (double)value)
	{
		/* Simple rounding, or the value is already an integer */
		return rounded;
	}
	else
	{
	#ifdef HAVE_POW
		power = pow(10, (double)digits);
	#else
		power = 1.0;
		while(digits > 0)
		{
			power *= 10.0;
			--digits;
		}
	#endif
		return (Math_RoundAwayFromZero(((double)value) * power) / power);
	}
}

/*
 * public static double Sin(double a);
 */
ILDouble _IL_Math_Sin(ILExecThread *thread, ILDouble a)
{
#ifdef HAVE_SIN
	return (ILDouble)(sin((double)a));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Sinh(double a);
 */
ILDouble _IL_Math_Sinh(ILExecThread *thread, ILDouble a)
{
#ifdef HAVE_SINH
	return (ILDouble)(sinh((double)a));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Sqrt(double a);
 */
ILDouble _IL_Math_Sqrt(ILExecThread *thread, ILDouble a)
{
#ifdef HAVE_SQRT
	return (ILDouble)(sqrt((double)a));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Tan(double a);
 */
ILDouble _IL_Math_Tan(ILExecThread *thread, ILDouble a)
{
#ifdef HAVE_TAN
	return (ILDouble)(tan((double)a));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * public static double Tanh(double value);
 */
ILDouble _IL_Math_Tanh(ILExecThread *thread, ILDouble value)
{
#ifdef HAVE_TANH
	return (ILDouble)(tanh((double)value));
#else
	return NOT_A_NUMBER;
#endif
}

/*
 * pubic static double Truncate(double d)
 */
ILDouble _IL_Math_Truncate(ILExecThread *thread, ILDouble d)
{
	return Math_Trunc((double)d);
}

/*
 * public static bool IsNaN(float f);
 */
ILBool _IL_Single_IsNaN(ILExecThread *thread, ILFloat f)
{
	return Math_IsNaN(f);
}

/*
 * private static int TestInfinity(float f);
 */
ILInt32 _IL_Single_TestInfinity(ILExecThread *thread, ILFloat f)
{
	return Math_IsInf(f);
}

/*
 * public static bool IsNaN(double d);
 */
ILBool _IL_Double_IsNaN(ILExecThread *thread, ILDouble d)
{
	return Math_IsNaN(d);
}

/*
 * private static int TestInfinity(double d);
 */
ILInt32 _IL_Double_TestInfinity(ILExecThread *thread, ILDouble d)
{
	return Math_IsInf(d);
}

#ifdef	__cplusplus
};
#endif
#endif /* IL_CONFIG_FP_SUPPORTED */
