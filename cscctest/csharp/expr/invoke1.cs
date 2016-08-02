/*
 * invoke1.cs - Test method invocation on value types.
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

public struct X
{
	int x;

	public X(int _x) { x = _x; }

	public int getX() { return x; }

	public virtual int getX2() { return x; }

	public int XProp { get { return x; } }

	public virtual int XProp2 { get { return x; } }
}

class Test
{
	int m1(X x)
	{
		return x.getX();
	}

	X m2()
	{
		return new X(0);
	}

	int m3()
	{
		return m2().getX();
	}

	int m4()
	{
		return m2().getX2();
	}

	int m5(X x)
	{
		return x.XProp;
	}

	int m6()
	{
		return m2().XProp;
	}

	int m7(X x)
	{
		return x.XProp2;
	}

	int m8()
	{
		return m2().XProp2;
	}
	
	String m9(X x)
	{
		return x.ToString();
	}
	
	String m10()
	{
		return m2().ToString();
	}
}
