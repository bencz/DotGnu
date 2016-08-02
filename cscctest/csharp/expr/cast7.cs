/*
 * cast7.cs - Test the invalid casts and coercions for reference types.
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

interface I
{
	void m1();
}

struct X
{
	int x;
}

class Test
{
	public void m1()
	{
		int i1;
		decimal dec1;
		X x1;
		object o1;
		Test t1;
		Test2 t2;
		Test3 t3;

		// Cannot coerce or cast null to a non-reference type.
		i1 = null;
		dec1 = null;
		i1 = (int)null;
		dec1 = (decimal)null;

		// Cannot coerce or cast dis-similar value types
		// with no user-defined conversions.
		x1 = dec1;
		x1 = i1;
		dec1 = x1;
		i1 = x1;
		x1 = (X)dec1;
		x1 = (X)i1;
		dec1 = (X)x1;
		i1 = (X)x1;

		// Casts are necessary for converting down the inheritance tree.
		t1 = o1;
		t2 = o1;
		t2 = t1;

		// Cannot cast across different sub-trees of the inheritance tree.
		t2 = t3;
		t2 = (Test2)t3;
	}
}

class Test2 : Test
{
}

class Test3 : Test
{
}
