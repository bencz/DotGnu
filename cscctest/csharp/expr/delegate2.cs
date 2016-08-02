/*
 * delegate2.cs - Test invalid cases of delegate creation.
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

public delegate string D1(int x);

public class Test
{
	public string m1(int x)
	{
		return "";
	}

	public int m2(int x)
	{
		return 0;
	}

	public int f;

	public D1 c1()
	{
		// Must have one argument.
		return new D1();
	}

	public D1 c2()
	{
		// Must have one argument.
		return new D1(m1, 3);
	}

	public D1 c3()
	{
		// Must refer to a method.
		return new D1(f);
	}

	public D1 c4()
	{
		// Must refer to a method.
		return new D1(3);
	}

	public D1 c5()
	{
		// Signature does not match.
		return new D1(m2);
	}
}
