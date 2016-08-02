/*
 * newarray1.cs - Test the valid cases of the "new" operator with arrays.
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

using System;

struct X
{
	private int x;

	public X(int _x) { x = _x; }
}

enum Color
{
	Red,
	Green,
	Blue
}

class Test
{
	void m1(int dim, uint dim2, long dim3, ulong dim4)
	{
		int[] i1;
		X[] x1;
		Color[,] c1;
		Test[] t1;
		Test[][] t2;
		Object[,,] o1;
		Object[,,][][,,,] o2;
		Object[,] o3;

		i1 = new int [3];
		x1 = new X [dim];
		c1 = new Color [3, dim];
		t1 = new Test [3];
		t2 = new Test [3][];
		o1 = new Object [dim, dim, dim];
		o2 = new Object [dim, dim, dim][][,,,];

		i1 = new int [dim2];
		i1 = new int [dim3];
		i1 = new int [dim4];
		o3 = new Object [3, dim2];
		o3 = new Object [3, dim3];
		o3 = new Object [3, dim4];
		o3 = new Object [dim2, 3];
		o3 = new Object [dim3, 3];
		o3 = new Object [dim4, 3];
	}
}
