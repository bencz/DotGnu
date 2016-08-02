/*
 * new2.cs - Test the invalid cases of the "new" operator.
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

interface J
{
	void m2();
}

class Test
{
	void m1()
	{
		int i1;
		Color c1;
		Test2 t2;
		Test3 t3;
		Test4 t4;
		Object o1;
		J j1;
		Object[] ao1;

		// Need a type to create objects.
		i1 = new Color.Red();

		// Cannot instantiate interfaces or abstract classes.
		j1 = new J();
		t3 = new Test3();

		// The type must be a class or value type.
		ao1 = new Object[] ();

		// Inaccessible constructor.
		t4 = new Test4();

		// No candidates.
		t2 = new Test2(0.1);
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

abstract class Test3
{
	protected Test3()
	{
	}
}

class Test4
{
	private Test4()
	{
	}
}
