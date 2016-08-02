/*
 * cast5.cs - Test the invalid casts and coercions for enumerated types.
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

class Test
{
	void m1()
	{
		Color col1;
		ColorSmall cols1;
		sbyte sb1 = 3;
		byte b1 = 3;
		short s1 = 3;
		ushort us1 = 3;
		int i1 = 3;
		uint ui1 = 3;
		long l1 = 3;
		ulong ul1 = 3;
		char c1 = '3';
		float f1 = 3.0f;
		double d1 = 3.0;
		decimal dc1 = 3.0m;

		// Only zero can be implicitly coerced.
		col1 = 1;
		cols1 = 1;

		// Try to coerce numeric types to enumerated types.
		col1 = sb1;
		col1 = b1;
		col1 = s1;
		col1 = us1;
		col1 = i1;
		col1 = ui1;
		col1 = l1;
		col1 = ul1;
		col1 = c1;
		col1 = f1;
		col1 = d1;
		col1 = dc1;
		cols1 = sb1;
		cols1 = b1;
		cols1 = s1;
		cols1 = us1;
		cols1 = i1;
		cols1 = ui1;
		cols1 = l1;
		cols1 = ul1;
		cols1 = c1;
		cols1 = f1;
		cols1 = d1;
		cols1 = dc1;

		// Try to coerce enumerated types to numeric types.
		sb1 = col1;
		b1  = col1;
		s1  = col1;
		us1 = col1;
		i1  = col1;
		ui1 = col1;
		l1  = col1;
		ul1 = col1;
		c1  = col1;
		f1  = col1;
		d1  = col1;
		dc1 = col1;
		sb1 = cols1;
		b1  = cols1;
		s1  = cols1;
		us1 = cols1;
		i1  = cols1;
		ui1 = cols1;
		l1  = cols1;
		ul1 = cols1;
		c1  = cols1;
		f1  = cols1;
		d1  = cols1;
		dc1 = cols1;

		// Try to coerce an enumerated type to another enumerated type.
		col1 = cols1;
		cols1 = col1;
	}
}
