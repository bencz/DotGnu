/*
 * as1.cs - Test the valid cases of the "as" operator.
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
		object o1;
		Test t1;
		Test2 t2;
		I i1;
		ValueType v1;

		// Implicit reference conversions.
		o1 = (null as Object);
		o1 = (o1 as Object);
		o1 = (t1 as Object);
		t1 = (t2 as Test);
		i1 = (t2 as I);

		// Implicit boxing conversions.
		o1 = (3 as Object);
		o1 = (3.0m as Object);
		v1 = (3 as ValueType);
		o1 = (Color.Blue as Object);

		// Explicit reference conversions.
		t2 = (t1 as Test2);
		i1 = (t1 as I);
	}
}

class Test2 : Test, I
{
}
