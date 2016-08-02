/*
 * const2.cs - Test the handling of constant expansion.
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
	// Constants of various types.
	public const bool True  = true;
	public const bool False = false;
	public const int i = -123456;
	public const uint ui = 123456;
	public const long l = -0x100000000;
	public const ulong ul = 9223372036854775807;
	public const float f = 1234.56e23f;
	public const double d = .123e23d;
	public const decimal dc1 = 79228162514264337593543950335m;
	public const decimal dc2 = -123.45e23m;
	public const char c = 'a';
	public const string s1 = null;
	public const string s2 = "";
	public const string s3 = "Hello World!";
	public const object o1 = null;
	public const Test   o2 = null;

	// Evaluate the constants.
	bool m1() { return True; }
	bool m2() { return False; }
	int m3() { return i; }
	uint m4() { return ui; }
	long m5() { return l; }
	ulong m6() { return ul; }
	float m7() { return f; }
	double m8() { return d; }
	decimal m9() { return dc1; }
	decimal m10() { return dc2; }
	char m11() { return c; }
	string m12() { return s1; }
	string m13() { return s2; }
	string m14() { return s3; }
	object m15() { return o1; }
	object m16() { return o2; }
}
