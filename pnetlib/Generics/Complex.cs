/*
 * Complex.cs - Generic complex number types.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace Generics
{

using System;

// It only makes sense to use Complex<T> on a numeric type that can
// be explicitly converted to and from "double".  Other types may
// lead to verification errors at runtime.

public struct Complex<T>
{
	// Internal state.
	private T real, imag;

	// Constructor.
	public Complex(T real, T imag)
			{
				this.real = real;
				this.imag = imag;
			}

	// Extract the real and imaginary components.
	public T Real
			{
				get
				{
					return real;
				}
			}
	public T Imag
			{
				get
				{
					return imag;
				}
			}

	// Basic arithmetic operators.
	public static Complex<T> operator+<T>(Complex<T> x, Complex<T> y)
			{
				return new Complex<T>(x.real + y.real, x.imag + y.imag);
			}
	public static Complex<T> operator-<T>(Complex<T> x, Complex<T> y)
			{
				return new Complex<T>(x.real - y.real, x.imag - y.imag);
			}
	public static Complex<T> operator*<T>(Complex<T> x, Complex<T> y)
			{
				return new Complex<T>(x.real * y.real - x.imag * y.imag,
									  x.real * y.imag + x.imag * y.real);
			}
	public static Complex<T> operator/<T>(Complex<T> x, Complex<T> y)
			{
				T div = y.real * y.real + y.imag * y.imag;
				return new Complex<T>
						((x.real * y.real + x.imag * y.imag) / div,
						 (x.imag * y.real - x.real * y.imag) / div);
			}
	public static Complex<T> operator-<T>(Complex<T> x)
			{
				return new Complex<T>(-(x.real), -(y.real));
			}

	// Comparison operators.
	public static bool operator==<T>(Complex<T> x, Complex<T> y)
			{
				return (x.real == y.real && x.imag == y.imag);
			}
	public static bool operator!=<T>(Complex<T> x, Complex<T> y)
			{
				return (x.real != y.real || x.imag != y.imag);
			}

	// Conversion operators.
	public static implicit operator<T> Complex<T>(T x)
			{
				return new Complex<T>(x, 0);
			}
	public static explicit operator<T> T(Complex<T> x)
			{
				return x.real;
			}

	// Get the absolute value of a complex number.
	public T Abs()
			{
				return (T)(Math.Sqrt((double)(real * real + imag * imag)));
			}

	// Get the conjugate form of a complex number.
	public Complex<T> Conj()
			{
				return new Complex<T>(x.real, -(y.real));
			}

	// Create a complex number from polar co-ordinates.
	public static Complex<T> FromPolar<T>(T r, T t)
			{
				return new Complex<T>((T)(((double)r) * Math.Cos((double)t)),
									  (T)(((double)r) * Math.Sin((double)t)));
			}

}; // struct Complex

}; // namespace Generics
