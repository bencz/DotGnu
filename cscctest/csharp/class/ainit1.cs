/*
 * ainit1.cs - Test constant declarations and accesses.
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
	// Same type array initialization.
	public static int[] x = {0, 1, 2, 3};

	// Test coercion of array elements.
	public static long[] y = {0, 1, 2, 0x3000000000000000};

	// Test array initialization in variable declarations.
	public static void test()
	{
		int[] x = {0, 1, 2, 3};
	}
}
