/*
 * new1.cs - Test the valid cases of the "new" operator.
 *
 * Copyright (C) 2002, 2009  Southern Storm Software, Pty Ltd.
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

struct Y
{
	public X x;

	public X X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}
}

enum Color
{
	Red,
	Green,
	Blue
}

class Test
{
	void m1()
	{
		int i1;
		X x1;
		Color c1;
		Test2 t2;
		Object o1;

		// Value type creation using default constructors.
		i1 = new int();
		x1 = new X();
		c1 = new Color();

		// Value type creation using explicit constructors.
		x1 = new X(3);

		// Object creation.
		t2 = new Test2();
		t2 = new Test2(3);
		t2 = new Test2(3L);
		o1 = new Object();
	}

	void m2(Y y)
	{
		y.x = new X(1);
	}

	void m3(Y y)
	{
		y.X = new X(1);
	}

	X m4(Y y)
	{
		return y.x = new X(1);
	}

	void m4()
	{
		X[] x = new X[2];

		x[0] = new X(0);
		x[1] = new X();
	}
}

class Test2
{
	public Test2()
	{
	}

	public Test2(int x)
	{
	}

	public Test2(long x)
	{
	}
}
