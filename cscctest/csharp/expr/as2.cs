/*
 * as2.cs - Test the invalid cases of the "as" operator.
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
}

class Test
{
	void m1()
	{
		object o1;
		Test t1;
		Test2 t2;
		Test3 t3;

		// The first operand must be a value.
		o1 = (Int32 as Object);

		// The second operand must be a type.
		o1 = (null as t1);

		// The second operand must be a reference type.
		o1 = (3 as Int32);

		// No implicit or explicit conversion.
		t2 = (t3 as Test2);
	}
}

class Test2 : Test, I
{
}

class Test3 : Test
{
}
