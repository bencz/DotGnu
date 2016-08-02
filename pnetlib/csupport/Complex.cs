/*
 * Complex.cs - Support classes for complex number arithmetic.
 *
 * This file is part of the Portable.NET "C language support" library.
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

namespace OpenSystem.C
{

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Size=8, Pack=4)]
[CName("float _Complex")]
public struct FloatComplex
{
	// Internal state.
	[FieldOffset(0)] private float real;
	[FieldOffset(4)] private float imag;

	// Constructors.
	public FloatComplex(float real, float imag)
			{
				this.real = real;
				this.imag = imag;
			}

	// Addition operator.
	public static FloatComplex operator+(FloatComplex a, FloatComplex b)
			{
				return new FloatComplex(a.real + b.real, a.imag + b.imag);
			}

	// Subtraction operator.
	public static FloatComplex operator-(FloatComplex a, FloatComplex b)
			{
				return new FloatComplex(a.real - b.real, a.imag - b.imag);
			}

	// Multiplication operator.
	public static FloatComplex operator*(FloatComplex a, FloatComplex b)
			{
				return new FloatComplex
					(a.real * b.real - a.imag * b.imag,
					 a.real * b.imag + a.imag * b.real);
			}

	// Division operator.
	public static FloatComplex operator/(FloatComplex a, FloatComplex b)
			{
				float divisor = b.real * b.real + b.imag * b.imag;
				return new FloatComplex
					((a.real * b.real + a.imag * b.imag) / divisor,
					 (a.imag * b.real - a.real * b.imag) / divisor);
			}

	// Negation operator.
	public static FloatComplex operator-(FloatComplex a)
			{
				return new FloatComplex(-(a.real), -(a.imag));
			}

	// Conjugate operator.
	public static FloatComplex operator~(FloatComplex a)
			{
				return new FloatComplex(a.real, -(a.imag));
			}

	// Get the real and imaginary components.
	public float Real { get { return real; } }
	public float Imag { get { return imag; } }

	// Conversions.
	public static implicit operator FloatComplex(float value)
			{
				return new FloatComplex(value, 0.0f);
			}
	public static implicit operator FloatComplex(FloatImaginary value)
			{
				return new FloatComplex(0.0f, value.Imag);
			}

} // struct FloatComplex

[StructLayout(LayoutKind.Explicit, Size=4, Pack=4)]
[CName("float _Imaginary")]
public struct FloatImaginary
{
	// Internal state.
	[FieldOffset(0)] private float imag;

	// Constructors.
	public FloatImaginary(float imag)
			{
				this.imag = imag;
			}

	// Get the imaginary component.
	public float Imag { get { return imag; } }

} // struct FloatImaginary

[StructLayout(LayoutKind.Explicit, Size=16, Pack=8)]
[CName("double _Complex")]
public struct DoubleComplex
{
	// Internal state.
	[FieldOffset(0)] private double real;
	[FieldOffset(4)] private double imag;

	// Constructors.
	public DoubleComplex(double real, double imag)
			{
				this.real = real;
				this.imag = imag;
			}

	// Addition operator.
	public static DoubleComplex operator+(DoubleComplex a, DoubleComplex b)
			{
				return new DoubleComplex(a.real + b.real, a.imag + b.imag);
			}

	// Subtraction operator.
	public static DoubleComplex operator-(DoubleComplex a, DoubleComplex b)
			{
				return new DoubleComplex(a.real - b.real, a.imag - b.imag);
			}

	// Multiplication operator.
	public static DoubleComplex operator*(DoubleComplex a, DoubleComplex b)
			{
				return new DoubleComplex
					(a.real * b.real - a.imag * b.imag,
					 a.real * b.imag + a.imag * b.real);
			}

	// Division operator.
	public static DoubleComplex operator/(DoubleComplex a, DoubleComplex b)
			{
				double divisor = b.real * b.real + b.imag * b.imag;
				return new DoubleComplex
					((a.real * b.real + a.imag * b.imag) / divisor,
					 (a.imag * b.real - a.real * b.imag) / divisor);
			}

	// Negation operator.
	public static DoubleComplex operator-(DoubleComplex a)
			{
				return new DoubleComplex(-(a.real), -(a.imag));
			}

	// Conjugate operator.
	public static DoubleComplex operator~(DoubleComplex a)
			{
				return new DoubleComplex(a.real, -(a.imag));
			}

	// Get the real and imaginary components.
	public double Real { get { return real; } }
	public double Imag { get { return imag; } }

	// Conversions.
	public static implicit operator DoubleComplex(double value)
			{
				return new DoubleComplex(value, 0.0);
			}
	public static implicit operator DoubleComplex(FloatComplex value)
			{
				return new DoubleComplex(value.Real, value.Imag);
			}
	public static implicit operator DoubleComplex(FloatImaginary value)
			{
				return new DoubleComplex(0.0, value.Imag);
			}
	public static implicit operator DoubleComplex(DoubleImaginary value)
			{
				return new DoubleComplex(0.0, value.Imag);
			}

} // struct DoubleComplex

[StructLayout(LayoutKind.Explicit, Size=8, Pack=8)]
[CName("double _Imaginary")]
public struct DoubleImaginary
{
	// Internal state.
	[FieldOffset(0)] private double imag;

	// Constructors.
	public DoubleImaginary(double imag)
			{
				this.imag = imag;
			}

	// Get the imaginary component.
	public double Imag { get { return imag; } }

} // struct DoubleImaginary

} // namespace OpenSystem.C
