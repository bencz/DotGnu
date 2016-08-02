/*
 * delegate1.cs - Test delegate creation.
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
	public string m1(int x)
	{
		return "";
	}

	public static string m2(int x)
	{
		return "";
	}

	public virtual string m3(int x)
	{
		return "";
	}

	public D1 c1()
	{
		return new D1(m1);
	}

	public D1 c2()
	{
		return new D1(m2);
	}

	public D1 c3()
	{
		return new D1(m3);
	}

	public D1 c4(Test t)
	{
		return new D1(t.m3);
	}
}
