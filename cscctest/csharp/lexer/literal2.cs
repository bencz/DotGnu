/*
 * literal2.cs - Test the handling of invalid literals in the lexer.
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
	// Integer literals.
	public const int i1 = 0x123456u;
	public const int i2 = 0x123456l;
	public const int i3 = 2147483648;

	// Unsigned integer literals.
	public const uint ui1 = 0x100000000;
	public const uint ui2 = 0x100000000u;
	public const uint ui3 = -3;

	// Long integer literals.
	public const long l1 = 123456ul;
	public const long l2 = 9223372036854775808;

	// Unsigned long integer literals.
	public const ulong ul1 = 0x10000000000000000;	// 2^64

	// Decimal literals.
	public const decimal dc1 = 1e30m;
	public const decimal dc2 = 79228162514264337593543950336m;

	// Character literals.
	public const char c1 = '\c';
	public const char c2 = '\x';
	public const char c3 = '\u';
	public const char c4 = '\u3';
	public const char c5 = '\u03';
	public const char c6 = '\u003';
	public const char c7 = '\u00003';
	public const char c8 = '\U';
	public const char c9 = '\U3';
	public const char c10 = '\U03';
	public const char c11 = '\U003';
	public const char c12 = '\U00003';
	public const char c13 = '\U000003';
	public const char c14 = '\U0000003';
	public const char c15 = '\U000000003';
	public const char c16 = '\U00101234';
}
