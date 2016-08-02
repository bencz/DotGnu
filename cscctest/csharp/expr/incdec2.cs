/*
 * incdec2.cs - Test invalid increment and decrement operators.
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

struct X
{
	public int x;
}

class Test
{
	void m1()
	{
		object o1 = null;
		X x1;
		object o2;
		X x2;

		// Test invalid uses of increment and decrement.
		++o1;
		++x1;
		--o1;
		--x1;
		o1++;
		x1++;
		o1--;
		x1--;
		o2 = ++o1;
		x2 = ++x1;
		o2 = --o1;
		x2 = --x1;
		o2 = o1++;
		x2 = x1++;
		o2 = o1--;
		x2 = x1--;
	}
}
