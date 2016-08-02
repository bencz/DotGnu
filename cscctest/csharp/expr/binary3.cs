/*
 * binary3.cs - Test user-defined binary arithmetic operators.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

struct X
{
	public int x;
	public X(int _x) { x = _x; }
}

struct Y
{
	int y;
	public Y(int _y) { y = _y; }

	public static Y operator-(Y y, X x)
	{
		return new Y(y.y - x.x);
	}
	public static X operator-(Y y1, Y y2)
	{
		return new X(y1.y - y2.y);
	}
}

class Test
{
	void m1()
	{
		Y y1 = new Y(1);
		Y y2 = new Y(2);
		X x = y1 - y2;
		Y y = y1 - x;
	}
}
