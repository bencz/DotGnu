/*
 * event1.cs - Test event declarations.
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

namespace System
{
	public interface IAsyncResult
	{
	}

	public delegate void AsyncCallback(IAsyncResult result);
}

public delegate string D1(int x);

public class Test
{
	private D1 x;

	// Compiler provides the add/remove implementation.
	public event D1 c1;
	public static event D1 c2;
	public virtual event D1 c3;

	// Programmer provides the add/remove implementation.
	public event D1 p1
		{
			add
			{
				x += value;
			}
			remove
			{
				x -= value;
			}
		}
}

public class Test2 : Test
{
	// Override c3 with a compiler-supplied implementation.
	public override event D1 c3;
}

public class Test3 : Test2
{
	// Override c3 with a programmer-supplied implementation.
	public override event D1 c3
		{
			add
			{
			}
			remove
			{
			}
		}
}

public class Test4 : Test
{
	public string m1(int x)
		{
			return "";
		}

	public void Add(Test t)
		{
			t.c1 += new D1(m1);
			t.c3 += new D1(m1);
			c2 += new D1(m1);
		}

	public void Sub(Test t)
		{
			t.c1 -= new D1(m1);
			t.c3 -= new D1(m1);
			c2 -= new D1(m1);
		}
}
