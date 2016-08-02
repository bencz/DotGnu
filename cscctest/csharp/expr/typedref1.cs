/*
 * typedref1.cs - Test valid expressions involved typedref values.
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

struct W
{
	int w;
}

class Test
{
	void m1(TypedReference tref)
	{
		Type type = __reftype(tref);
		int x = __refvalue(tref, int);
		Test y = __refvalue(tref, Test);
		Test[] z = __refvalue(tref, Test[]);
		W w = __refvalue(tref, W);
	}

	int fld;
	W fld2;
	static Test fld3;

	void m2()
	{
		int x = 0;
		m1(__makeref(x));
		m1(__makeref(fld));
		m1(__makeref(fld2));
		m1(__makeref(fld3));
	}

	void m3(int[] x, int y)
	{
		m1(__makeref(x[y]));
	}
}
