/*
 * method2.cs - Test method declarations.
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

class Test
{
	static int m1(out int x)
	{
		return 3;
	}

	static int m2(ref int x)
	{
		return 3;
	}

	static void m3(params int[] y)
	{
	}

	static void m4()
	{
		int x;
		m1(out x);
		m2(ref x);
		m3(3);
		m3(3, 4, 5, 6, 7);
		m3();
	}
}
