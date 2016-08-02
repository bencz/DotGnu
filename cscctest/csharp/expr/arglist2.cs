/*
 * arglist2.cs - Test invalid declarations involving "__arglist".
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
	// "__arglist" must be the last formal parameter.
	void m1(__arglist, String format) {}

	// "__arglist" cannot be used with indexers.
	int this[__arglist] { get { return 0; } }

	// Must use "__arglist" inside a vararg method.
	void m2()
	{
		RuntimeArgumentHandle handle = __arglist;
	}

	// Must use "__arglist(...)" in an invocation expression.
	void m3()
	{
		__arglist(1, 2, 3);
		int x = __arglist(1) + 2;
	}

	// No matching method for call to "__arglist" method.
	void m4()
	{
		m3("format", __arglist(1, 2, 3));
	}

	// Cannot use "ref" or "out" on "__arglist" arguments.
	void m5(__arglist) {}
	void m6()
	{
		int x, y;
		m5(__arglist(ref x, out y));
		m5(ref __arglist(1));
		m5(out __arglist(1));
	}

	// "__arglist" must be the last call argument.
	void m7()
	{
		m5(__arglist(1, 2, 3), "format");
	}

	// "__arglist" must contain l-values or r-values.
	int Prop { set {} }
	void m8()
	{
		m5(__arglist(Prop));
		m5(__arglist(Test));
	}
}
