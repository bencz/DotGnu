/*
 * incdec1.cs - Test increment and decrement operators.
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

enum Color
{
	Red,
	Green,
	Blue
}

enum ColorSmall : sbyte
{
	Red,
	Green,
	Blue
}

public struct X
{
	public static X operator++(X x) { return x; }
	public static X operator--(X x) { return x; }
}

public class Y
{
	public static Y operator++(Y y) { return y; }
	public static Y operator--(Y y) { return y; }
}

class Test
{
	void m1()
	{
		sbyte sb1 = 34;
		byte b1 = 42;
		short s1 = -126;
		ushort us1 = 67;
		int i1 = -1234;
		uint ui1 = 54321;
		long l1 = -9876543210;
		ulong ul1 = 9876543210;
		char c1 = 'A';
		float f1 = 1.5f;
		double d1 = 6.7d;
		decimal dc1 = 3.5m;
		Color cl1 = Color.Red;
		ColorSmall cs1 = ColorSmall.Red;
		X x1;
		Y y1 = null;
		sbyte sb2;
		byte b2;
		short s2;
		ushort us2;
		int i2;
		uint ui2;
		long l2;
		ulong ul2;
		char c2;
		float f2;
		double d2;
		decimal dc2;
		Color cl2;
		ColorSmall cs2;
		X x2;
		Y y2;

		// Pre-increment with discarded result.
		++sb1;
		++b1;
		++s1;
		++us1;
		++i1;
		++ui1;
		++l1;
		++ul1;
		++c1;
		++f1;
		++d1;
		++dc1;
		++cl1;
		++cs1;
		++x1;
		++y1;

		// Pre-decrement with discarded result.
		--sb1;
		--b1;
		--s1;
		--us1;
		--i1;
		--ui1;
		--l1;
		--ul1;
		--c1;
		--f1;
		--d1;
		--dc1;
		--cl1;
		--cs1;
		--x1;
		--y1;

		// Post-increment with discarded result.
		sb1++;
		b1++;
		s1++;
		us1++;
		i1++;
		ui1++;
		l1++;
		ul1++;
		c1++;
		f1++;
		d1++;
		dc1++;
		cl1++;
		cs1++;
		x1++;
		y1++;

		// Post-decrement with discarded result.
		sb1--;
		b1--;
		s1--;
		us1--;
		i1--;
		ui1--;
		l1--;
		ul1--;
		c1--;
		f1--;
		d1--;
		dc1--;
		cl1--;
		cs1--;
		x1--;
		y1--;

		// Pre-increment with used result.
		sb2 = ++sb1;
		b2  = ++b1;
		s2  = ++s1;
		us2 = ++us1;
		i2  = ++i1;
		ui2 = ++ui1;
		l2  = ++l1;
		ul2 = ++ul1;
		c2  = ++c1;
		f2  = ++f1;
		d2  = ++d1;
		dc2 = ++dc1;
		cl2 = ++cl1;
		cs2 = ++cs1;
		x2  = ++x1;
		y2  = ++y1;

		// Pre-decrement with used result.
		sb2 = --sb1;
		b2  = --b1;
		s2  = --s1;
		us2 = --us1;
		i2  = --i1;
		ui2 = --ui1;
		l2  = --l1;
		ul2 = --ul1;
		c2  = --c1;
		f2  = --f1;
		d2  = --d1;
		dc2 = --dc1;
		cl2 = --cl1;
		cs2 = --cs1;
		x2  = --x1;
		y2  = --y1;

		// Post-increment with used result.
		sb2 = sb1++;
		b2  = b1++;
		s2  = s1++;
		us2 = us1++;
		i2  = i1++;
		ui2 = ui1++;
		l2  = l1++;
		ul2 = ul1++;
		c2  = c1++;
		f2  = f1++;
		d2  = d1++;
		dc2 = dc1++;
		cl2 = cl1++;
		cs2 = cs1++;
		x2  = x1++;
		y2  = y1++;

		// Post-decrement with used result.
		sb2 = sb1--;
		b2  = b1--;
		s2  = s1--;
		us2 = us1--;
		i2  = i1--;
		ui2 = ui1--;
		l2  = l1--;
		ul2 = ul1--;
		c2  = c1--;
		f2  = f1--;
		d2  = d1--;
		dc2 = dc1--;
		cl2 = cl1--;
		cs2 = cs1--;
		x2  = x1--;
		y2  = y1--;
	}
}
