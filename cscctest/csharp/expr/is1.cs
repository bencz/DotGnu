/*
 * is1.cs - Test the valid cases of the "is" operator.
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

interface I
{
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
		bool b1;
		object o1;
		Test t1;
		Test2 t2;
		I i1;
		ValueType v1;
		int x;
		Color col1;

		// Implicit reference conversions.  These are equivalent
		// to a simple test against null.
		b1 = (null is Object);
		if(null is Object) x = 1;
		if(!(null is Object)) x = 2;

		b1 = (o1 is Object);
		if(o1 is Object) x = 1;
		if(!(o1 is Object)) x = 2;

		b1 = (t1 is Object);
		if(t1 is Object) x = 1;
		if(!(t1 is Object)) x = 2;

		b1 = (t2 is Test);
		if(t2 is Test) x = 1;
		if(!(t2 is Test)) x = 2;

		b1 = (t2 is I);
		if(t2 is I) x = 1;
		if(!(t2 is I)) x = 2;

		// Implicit boxing conversions.  These always give true.
		b1 = (3 is Object);
		if(3 is Object) x = 1;
		if(!(3 is Object)) x = 2;

		b1 = (3.0m is Object);
		if(3.0m is Object) x = 1;
		if(!(3.0m is Object)) x = 2;

		b1 = (3 is ValueType);
		if(3 is ValueType) x = 1;
		if(!(3 is ValueType)) x = 2;

		b1 = (Color.Blue is Object);
		if(Color.Blue is Object) x = 1;
		if(!(Color.Blue is Object)) x = 2;

		// Explicit reference conversions.
		b1 = (t1 is Test2);
		if(t1 is Test2) x = 1;
		if(!(t1 is Test2)) x = 2;

		b1 = (t1 is I);
		if(t1 is I) x = 1;
		if(!(t1 is I)) x = 2;

		// Explicit unboxing conversions.
		b1 = (o1 is int);
		if(o1 is int) x = 1;
		if(!(o1 is int)) x = 2;

		// Conversions that are always false.
		b1 = (col1 is I);
		if(col1 is I) x = 1;
		if(!(col1 is I)) x = 2;
	}
}

class Test2 : Test, I
{
}
