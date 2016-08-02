/*
 * enum1.cs - Test the handling of valid operators on enumerated types.
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
	Red, Green, Blue
}

enum Size : long
{
	Small, Large
}

class Test
{
	void m1()
	{
		Color c1 = Color.Red;
		Color c2 = Color.Green;
		Color c3;
		Size s1 = Size.Small;
		Size s2 = Size.Large;
		Size s3;
		int i1;
		long l1;
		bool b1;

		// Bitwise not.
		c3 = ~c1;
		s3 = ~s1;

		// Addition.
		c3 = c1 + 1;
		c3 = 1 + c2;
		s3 = s1 + 1;
		s3 = 1 + s2;

		// Subtraction.
		c3 = c1 - 1;
		i1 = c1 - c2;
		s3 = s1 - 1;
		l1 = s1 - s2;

		// Comparison.
		b1 = (c1 == c2);
		b1 = (c1 != c2);
		b1 = (c1 >= c2);
		b1 = (c1 <= c2);
		b1 = (c1 > c2);
		b1 = (c1 < c2);
		b1 = (s1 == s2);
		b1 = (s1 != s2);
		b1 = (s1 >= s2);
		b1 = (s1 <= s2);
		b1 = (s1 > s2);
		b1 = (s1 < s2);

		// Bitwise binary operators.
		c3 = (c1 & c2);
		s3 = (s1 & s2);
		c3 = (c1 | c2);
		s3 = (s1 | s2);
		c3 = (c1 ^ c2);
		s3 = (s1 ^ s2);
	}
}
