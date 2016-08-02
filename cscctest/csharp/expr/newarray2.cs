/*
 * newarray2.cs - Test the invalid cases of the "new" operator with arrays.
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
	void m1(Object dim)
	{
		int[][] i1;
		int[] i2;
		int[,] i3;

		// Dimension specifier must be at the outermost level.
		i1 = new int [] [3];

		// Cannot coerce array index to "int", "uint", "long", or "ulong".
		i2 = new int [3.0];
		i3 = new int [3, dim];
		i3 = new int [dim, 3];
	}
}
