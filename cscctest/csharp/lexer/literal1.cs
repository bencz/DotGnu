/*
 * literal1.cs - Test the handling of valid literals in the lexer.
 *
 * "C# Language Specification", Draft 13, Section 9.4.4, "Literals"
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

class Test
{
	// Boolean literals.
	public const bool True  = true;
	public const bool False = false;

	// Integer literals.
	public const int i1 = 0;
	public const int i2 = 012;		// Octal warning, recognised as decimal.
	public const int i3 = 123456;
	public const int i4 = -123456;
	public const int i5 = 0x123456;
	public const int i6 = 0x7FFFFFFF;
	public const int i7 = 0x7abcdef0;
	public const int i8 = 2147483647;
	public const int i9 = -2147483648;

	// Unsigned integer literals.
	public const uint ui1 = 0;
	public const uint ui2 = 123456;
	public const uint ui3 = 123456u;
	public const uint ui4 = 0x123456U;
	public const uint ui5 = 2147483647;
	public const uint ui6 = 2147483648;
	public const uint ui7 = 0xFFFFFFFF;

	// Long integer literals.
	public const long l1 = 0;
	public const long l2 = 123456;
	public const long l3 = 2147483647;
	public const long l4 = 2147483648;
	public const long l5 = -2147483648;
	public const long l6 = 0xFFFFFFFF;
	public const long l7 = 0x100000000;
	public const long l8 = -0x100000000;
	public const long l9 = 123456l;
	public const long l10 = 123456L;
	public const long l11 = 9223372036854775807;
	public const long l12 = -9223372036854775808;

	// Unsigned long integer literals.
	public const ulong ul1 = 0;
	public const ulong ul2 = 123456;
	public const ulong ul3 = 2147483647;
	public const ulong ul4 = 2147483648;
	public const ulong ul5 = 0xFFFFFFFF;
	public const ulong ul6 = 0x100000000;
	public const ulong ul7 = 123456l;
	public const ulong ul8 = 123456uL;
	public const ulong ul9 = 9223372036854775807;
	public const ulong ul10 = 9223372036854775808;
	public const ulong ul11 = 0xFFFFFFFFFFFFFFFF;
	public const ulong ul12 = 123456u;
	public const ulong ul13 = 123456ul;
	public const ulong ul14 = 123456uL;
	public const ulong ul15 = 123456Ul;
	public const ulong ul16 = 123456UL;
	public const ulong ul17 = 123456lu;
	public const ulong ul18 = 123456lU;
	public const ulong ul19 = 123456Lu;
	public const ulong ul20 = 123456LU;

	// Single-precision floating-point literals.
	public const float f1 = 0.0f;
	public const float f2 = -1.0F;
	public const float f3 = 1234.56e23f;
	public const float f4 = 1234.56e+23F;
	public const float f5 = 1234.56e-23f;
	public const float f6 = .123F;
	public const float f7 = .123e23f;
	public const float f8 = 123e23F;
	public const float f9 = 123e+23f;
	public const float f10 = 123e-23F;
	public const float f11 = 123f;

	// Double-precision floating-point literals.
	public const double d1 = 0.0d;
	public const double d2 = -1.0D;
	public const double d3 = 1234.56e23;
	public const double d4 = 1234.56e+23d;
	public const double d5 = 1234.56e-23D;
	public const double d6 = .123;
	public const double d7 = .123e23d;
	public const double d8 = 123e23D;
	public const double d9 = 123e+23;
	public const double d10 = 123e-23d;
	public const double d11 = 123D;

	// Decimal literals.
	public const decimal dc1 = 0.0m;
	public const decimal dc2 = -1.0M;
	public const decimal dc3 = 1234.56e23m;
	public const decimal dc4 = 1234.56e+23M;
	public const decimal dc5 = 1234.56e-23m;
	public const decimal dc6 = .123M;
	public const decimal dc7 = .123e23m;
	public const decimal dc8 = 123e23M;
	public const decimal dc9 = 123e+23m;
	public const decimal dc10 = 123e-23M;
	public const decimal dc11 = 123m;
	public const decimal dc12 = 79228162514264337593543950335m;
	public const decimal dc13 = -79228162514264337593543950335m;
	public const decimal dc14 = -792281625.14264337593543950335m;
	public const decimal dc15 = -7.9228162514264337593543950335m;
	public const decimal dc16 = -0.79228162514264337593543950335m;
	public const decimal dc17 = 0.0000000000000000000000000001m;
	public const decimal dc18 = 0.00000000000000000000000000001m; // == 0.0m

	// Character literals.
	public const char c1 = 'a';
	public const char c2 = '\'';
	public const char c3 = '\"';
	public const char c4 = '\\';
	public const char c5 = '\0';
	public const char c6 = '\a';
	public const char c7 = '\b';
	public const char c8 = '\f';
	public const char c9 = '\n';
	public const char c10 = '\r';
	public const char c11 = '\t';
	public const char c12 = '\v';
	public const char c13 = '\x3';
	public const char c14 = '\x03';
	public const char c15 = '\x003';
	public const char c16 = '\x0003';
	public const char c17 = '\u0003';
	public const char c18 = '\U00000003';
	public const char c19 = '\033';		// Octal character warning.

	// String literals.
	public const string s1 = null;
	public const string s2 = "";
	public const string s3 = "Hello World!";
	public const string s4 = @"Hello \tWo""rld!";
	public const string s5 = @"line 1
line 2
line 3";
	public const string s6 = @"
#bad
";
	public const string s7 = "\U00101234";

	// Object literals.
	public const object o1 = null;
	public const Test   o2 = null;
}
